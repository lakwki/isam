using System;
using System.Data;
using System.Collections;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.common;
using com.next.isam.domain.account;
using com.next.isam.domain.order;
using com.next.isam.domain.shipping;
using com.next.isam.domain.ils;
using com.next.isam.domain.types;
using com.next.isam.reporter.dataserver;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.infra.util;
using System.Collections.Generic;

namespace com.next.isam.reporter.invoice
{
    public class InvoiceReportManager
    {
        private static InvoiceReportManager _instance;
        private ShippingWorker shippingWorker;

        public InvoiceReportManager()
		{
            shippingWorker = ShippingWorker.Instance;
		}

        public static InvoiceReportManager Instance
		{
			get 
			{
				if (_instance == null)
				{
                    _instance = new InvoiceReportManager();
				}
				return _instance;
			}
		}

        public InvoiceRpt getInvoiceReport(ArrayList shipmentIdList, int userId)
        {
            InvoiceRpt invoiceRpt = new InvoiceRpt();
            InvoiceReportDs ds = new InvoiceReportDs();
            int shipmentId = 0;
            ArrayList invoiceForMultiShipmentList = new ArrayList();

            try
            {
                foreach (object obj in shipmentIdList)
                {
                    shipmentId = (int)obj;
                    if (invoiceForMultiShipmentList.Contains(shipmentId))
                        continue;

                    InvoiceDef invoiceDef = shippingWorker.getInvoiceByKey(shipmentId);
                    InvoiceReportDs.InvoiceReportRow row = ds.InvoiceReport.NewInvoiceReportRow();

                    if (invoiceDef != null)
                    {

                        if (invoiceDef.SequenceNo == int.MinValue)
                        {
                            this.addInvoiceRowToReport(invoiceDef, row, ds, -1);
                        }
                        else 
                        {
                            ArrayList invoiceList = shippingWorker.getInvoiceByInvoiceNo(invoiceDef.InvoicePrefix, invoiceDef.InvoiceSeqNo, invoiceDef.InvoiceYear, -1);

                            //this.addInvoiceRowToReport(invoiceDef, row, ds, invoiceList.Count);

                            foreach (InvoiceDef def in invoiceList)
                            {
                                this.addInvoiceRowToReport(def, ds.InvoiceReport.NewInvoiceReportRow(), ds, invoiceList.Count);
                                invoiceForMultiShipmentList.Add(def.ShipmentId);
                            }
                        }
                        
                    }
                }

                invoiceRpt.SetDataSource(ds);
                invoiceRpt.SetParameterValue("userId", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "Invoice Print Error - Shipment Id : " + shipmentId.ToString());
                throw e;
            }
            return invoiceRpt;
        }


        private void addInvoiceRowToReport(InvoiceDef invoiceDef, InvoiceReportDs.InvoiceReportRow row, InvoiceReportDs ds, int totalShipmentInInvoice)
        {
            row.ShipmentId = invoiceDef.ShipmentId;
            row.InvoiceDate = invoiceDef.InvoiceDate;
            row.InvoicePrefix = invoiceDef.InvoicePrefix;
            row.InvoiceSeq = invoiceDef.InvoiceSeqNo;
            row.InvoiceYear = invoiceDef.InvoiceYear;
            if (invoiceDef.SequenceNo != int.MinValue)
            {
                row.SequenceNo = invoiceDef.SequenceNo;
                //ArrayList invoiceList = shippingWorker.getInvoiceByInvoiceNo(invoiceDef.InvoicePrefix, invoiceDef.InvoiceSeqNo, invoiceDef.InvoiceYear, -1);
                row.SequenceNoCount = totalShipmentInInvoice;
            }
            if (invoiceDef.InvoiceRemark != null && invoiceDef.InvoiceRemark != "")
            {
                invoiceDef.InvoiceRemark = invoiceDef.InvoiceRemark.Replace("||", System.Environment.NewLine);
                row.InvoiceRemark = invoiceDef.InvoiceRemark.Replace("\r\n\r\n", "<br>                            <br>");
                row.InvoiceRemark = "<pre>" + row.InvoiceRemark.Replace("\r\n", " </pre><br><pre>") + " </pre>";
            }
            row.SupplierInvoiceNo = invoiceDef.SupplierInvoiceNo;
            row.NSLCommissionAmt = invoiceDef.NSLCommissionAmt;
            row.ExportLicenceFee = invoiceDef.ExportLicenceFee;
            row.QuotaCharge = invoiceDef.QuotaCharge;
            row.ItemDesc1 = invoiceDef.ItemDesc1;
            row.ItemDesc2 = invoiceDef.ItemDesc2;
            row.ItemDesc3 = invoiceDef.ItemDesc3;
            row.ItemDesc4 = invoiceDef.ItemDesc4;
            row.ItemDesc5 = invoiceDef.ItemDesc5;
            row.ItemColour = invoiceDef.ItemColour;
            row.PiecesPerDeliveryUnit = invoiceDef.PiecesPerDeliveryUnit;

            row.IsSelfBilledOrder = invoiceDef.IsSelfBilledOrder;
            if (invoiceDef.CustomerDestination == null)
            {
                row.DestinationCode = "";
                row.DestinationDesc = "";
            }
            else
            {
                row.DestinationCode = invoiceDef.CustomerDestination.DestinationCode;
                row.DestinationDesc = invoiceDef.CustomerDestination.DestinationDesc;
            }

            row.PackingMethodId = invoiceDef.PackingMethod.PackingMethodId;

            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(invoiceDef.ShipmentId);
            row.DeliveryNo = shipmentDef.DeliveryNo;
            row.IsMockShopSample = shipmentDef.IsMockShopSample;
            row.IsPressSample = shipmentDef.IsPressSample;
            row.IsStudioSample = shipmentDef.IsStudioSample;
            row.TotalShippedQty = shipmentDef.TotalShippedQuantity;
            //row.TotalShippedAmt = shipmentDef.TotalShippedAmount;
            row.TotalShippedAmt = shipmentDef.TotalShippedAmountAfterDiscount;
            row.VatPercent = shipmentDef.VatPercent;
            row.NSLCommissionPercent = shipmentDef.NSLCommissionPercentage;
            if (shipmentDef.ShipmentPort != null)
                row.ShipmentPortDesc = shipmentDef.ShipmentPort.ShipmentPortDescription;
            else
                row.ShipmentPortDesc = "";
            if (invoiceDef.ShipFromCountry != null)
                row.ShipmentCountryDesc = invoiceDef.ShipFromCountry.ShipmentCountryDescription;
            else
                row.ShipmentCountryDesc = "";
            row.CurrencyCode = shipmentDef.SellCurrency.CurrencyCode;
            row.CurrencyName = shipmentDef.SellCurrency.Name;
            row.CountryOfOrigin = "";
            row.SpecialOrderTypeID = shipmentDef.SpecialOrderTypeId;

            ArrayList ilsDocList = ILSUploadWorker.Instance.getILSDocumentListByShipmentId(invoiceDef.ShipmentId);
            foreach (ILSDocumentDef ilsDoc in ilsDocList)
            {
                if (ilsDoc.DocumentType == "CO")
                {
                    CountryOfOriginRef coRef = GeneralWorker.Instance.getCountryOfOriginByCode(ilsDoc.DocumentCountry);
                    if (coRef != null)
                    {
                        if (row.CountryOfOrigin == "")
                            row.CountryOfOrigin += coRef.Name;
                        else if (!row.CountryOfOrigin.Contains(coRef.Name))
                            row.CountryOfOrigin += " / " + coRef.Name;
                    }
                }
            }

            bool isCOFromILS = row.CountryOfOrigin != "";
            ArrayList docList = ShippingWorker.Instance.getDocumentListByShipmentId(invoiceDef.ShipmentId);

            foreach (DocumentDef docDef in docList)
            {
                if (docDef.DocumentType.Id == DocumentType.CERTIFICATE_OF_ORIGIN.Id)
                {

                    if (row.IsCODocNoNull() || row.CODocNo == "")
                        row.CODocNo = docDef.DocumentNo;
                    else
                        row.CODocNo += ", " + docDef.DocumentNo;
                    if (!isCOFromILS)
                    {
                        if (row.CountryOfOrigin == "")
                            row.CountryOfOrigin += docDef.Country.Name;
                        else if (!row.CountryOfOrigin.Contains(docDef.Country.Name))
                            row.CountryOfOrigin += " / " + docDef.Country.Name;
                    }
                }
                else if (docDef.DocumentType.Id == DocumentType.EXPORT_LICENCE.Id || docDef.DocumentType.Id == DocumentType.JOINT_LICENCE.Id)
                {
                    if (row.IsELDocNoNull() || row.ELDocNo == "")
                        row.ELDocNo = docDef.DocumentNo;
                    else
                        row.ELDocNo += ", " + docDef.DocumentNo;
                }
                else if (docDef.DocumentType.Id == DocumentType.GSP_FORM.Id)
                {
                    if (row.IsGSPFormNoNull() || row.GSPFormNo == "")
                        row.GSPFormNo = docDef.DocumentNo;
                    else
                        row.GSPFormNo += ", " + docDef.DocumentNo;
                }
            }

            if (row.CountryOfOrigin == "" && shipmentDef.CountryOfOrigin != null)
                row.CountryOfOrigin = shipmentDef.CountryOfOrigin.Name;

            ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
            List<string> ukSupplierCode = CommonWorker.Instance.getNSLedSelfBilledSupplierCodeList();
            if (ukSupplierCode.Contains(contractDef.UKSupplierCode))
            {
                row.IsNSLedSelfBilled = 1;
            }
            else
            {
                row.IsNSLedSelfBilled = 0;
            }
            row.ContractNo = contractDef.ContractNo;
            row.TradingAgencyId = contractDef.TradingAgencyId;
            row.Code = contractDef.Season.Code;
            row.OfficeId = contractDef.Office.OfficeId;
            row.OfficeCode = contractDef.Office.OfficeCode;
            row.PhaseId = contractDef.PhaseId;
            row.PackingUnitDesc = contractDef.PackingUnit.OPSKey;
            row.IsNextMfgOrder = contractDef.IsNextMfgOrder;
            row.IsLDPOrder = contractDef.IsLDPOrder;
            row.CustomerCode = contractDef.Customer.CustomerCode;
            row.CustomerOPSKey = contractDef.Customer.OPSKey;
            row.CustomerShortCode = contractDef.Customer.ShortCode;
            row.SupplierName = shipmentDef.Vendor.Name;
            row.UKSupplierCode = contractDef.UKSupplierCode;
            row.UKProductGroupCode = contractDef.UKProductGroupCode;
            row.BookingRefNo = contractDef.BookingRefNo;

            TradingAgencyDef agencyDef = CommonWorker.Instance.getTradingAgencyByKey(contractDef.TradingAgencyId);
            row.TradingAgencyName = agencyDef.Name;
            row.TradingAgencyAddress1 = agencyDef.Address1;
            row.TradingAgencyAddress2 = agencyDef.Address2;
            row.TradingAgencyAddress3 = agencyDef.Address3;
            row.TradingAgencyAddress4 = agencyDef.Address4;
            row.TelNo = agencyDef.TelNo;
            row.FaxNo = agencyDef.FaxNo;
            row.Remark = agencyDef.Remark;

            row.Consignee = contractDef.Customer.Consignee;
            row.Address1 = contractDef.Customer.Address1;
            row.Address2 = contractDef.Customer.Address2;
            row.Address3 = contractDef.Customer.Address3;
            row.Address4 = contractDef.Customer.Address4;

            //if (CommonWorker.Instance.getCustomerDestinationByKey(shipmentDef.CustomerDestinationId).UTurnOrder==1)
            if (shipmentDef.TermOfPurchase.TermOfPurchaseId == TermOfPurchaseRef.Id.FOB_UT.GetHashCode())
            {
                if (contractDef.Office.OfficeId == OfficeId.HK.Id || contractDef.Office.OfficeId == OfficeId.SH.Id || contractDef.Office.OfficeId == OfficeId.DG.Id)
                {
                    row.TradingAgencyName = "NEXT MANUFACTURING (SHENZHEN) LIMITED";
                    row.TradingAgencyAddress1 = "BLOCK 16, HEJING INDUSTRIAL PARK";
                    row.TradingAgencyAddress2 = "HEPING VILLAGE, HI-TECH AREA FUYONG";
                    row.TradingAgencyAddress3 = "BAO'AN DISTRICT";
                    row.TradingAgencyAddress4 = "SHENZHEN, CHINA";
                }
                CustomerDef hempel = CommonWorker.Instance.getCustomerByKey(CustomerDef.Id.hempel.GetHashCode());
                row.Consignee = hempel.Consignee;
                row.Address1 = hempel.Address1;
                row.Address2 = hempel.Address2;
                row.Address3 = hempel.Address3;
                row.Address4 = hempel.Address4;
                row.TelNo = hempel.TelNo;
                row.FaxNo = hempel.FaxNo;
            }
            else if (contractDef.Customer.CustomerId == CustomerDef.Id.lipsy.GetHashCode() &&
                    (invoiceDef.CustomerDestination.CustomerDestinationId == CustomerDestinationDef.Id.b1.GetHashCode() || invoiceDef.CustomerDestination.CustomerDestinationId == CustomerDestinationDef.Id.b2.GetHashCode()))
            {   // LIPSY - USA WAREHOUSE
                row.Consignee = "LIPSY LTD";
                row.Address1 = "C/O STELLAE INTERNATIONAL INC.";
                row.Address2 = "149-39 GUY R. BREWER BLVD.";
                row.Address3 = "JAMAICA, NY 11434";
                row.Address4 = "USA";
            }
            else if (contractDef.Customer.CustomerId == CustomerDef.Id.ezibuy.GetHashCode() && contractDef.PhaseId >= 3)
            {   // Ezibuy Phase 3 onward 
                row.Consignee = "EZIBUY OPERATIONS LIMITED";
                row.Address1 = "EZIBUY INTERNATIONAL DISTRIBUTION CENTRE";
                row.Address2 = "31 El PRADO DRIVE";
                row.Address3 = "PALMERSTON NORTH 4470";
                row.Address4 = "NEW ZEALAND";
            }

            if ((contractDef.Customer.CustomerId != CustomerDef.Id.directory.GetHashCode() && contractDef.Customer.CustomerId != CustomerDef.Id.retail.GetHashCode()
                && (shipmentDef.TermOfPurchase.TermOfPurchaseId != TermOfPurchaseRef.Id.FOB_UT.GetHashCode()))
                || (contractDef.Customer.CustomerId == CustomerDef.Id.hempel.GetHashCode() && contractDef.Office.OfficeId == OfficeId.SL.Id))
            {
                SettlementBankDetailDef settlementBank = AccountWorker.Instance.getSettlementBankDetail(contractDef.Office.OfficeId,
                    shipmentDef.SellCurrency.CurrencyId, contractDef.TradingAgencyId);
                if (settlementBank != null)
                {
                    row.AccountName = settlementBank.AccountName;
                    row.AccountNo = settlementBank.AccountNo;
                    row.SwiftCode = settlementBank.SwiftCode;
                    row.BankName = settlementBank.BankName;
                    row.BankAddress = settlementBank.BankAddress;
                }
            }

            domain.product.ProductDef product = ProductWorker.Instance.getProductByKey(contractDef.ProductId);
            row.ItemNo = product.ItemNo; 
            row.DesignSourceId = product.DesignSource.DesignSourceId;

            ArrayList manifestDetailList = (ArrayList)ILSUploadWorker.Instance.getILSManifestDetailListByShipmentId(invoiceDef.ShipmentId);
            if (manifestDetailList.Count != 0)
            {
                ILSManifestDetailDef manifestDetailDef = (ILSManifestDetailDef)manifestDetailList[0];
                ILSManifestDef manifestDef = ILSUploadWorker.Instance.getILSManifestByKey(manifestDetailDef.ContainerNo);
                row.VesselName = manifestDef.VesselName;
                row.DepartDate = manifestDef.DepartDate;
                row.VoyageNo = manifestDef.VoyageNo;
                row.ContainerNo = manifestDef.ContainerNo;
            }
        

            ArrayList list = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(invoiceDef.ShipmentId);

            foreach (ShipmentDetailDef detailDef in list)
            {
                InvoiceReportDs.InvoiceReportRow r = ds.InvoiceReport.NewInvoiceReportRow();

                for (int i = 0; i < ds.InvoiceReport.Columns.Count; i++)
                {
                    r[i] = row[i];
                }

                r.SizeOptionNo = detailDef.SizeOption.SizeOptionNo;
                r.SizeDesc = detailDef.SizeOption.SizeDescription;
                r.ShippedQty = detailDef.ShippedQuantity;

                //Update By Alan Cheng, 19/08/2019, Request for printing invoice with discounted unit price
                //r.SellingPrice = detailDef.SellingPrice; //original code
                r.SellingPrice = detailDef.ReducedSellingPrice; //replaced code

                r.OptionColour = detailDef.Colour;

                ds.InvoiceReport.Rows.Add(r);

                        //if (detailDef.ShippingGSPFormTypeId != 0)
                        //{
                        //    ((InvoiceReportDs.InvoiceReportRow)ds.InvoiceReport.Rows[0]).ShippingGSPFormTypeId = detailDef.ShippingGSPFormTypeId;
                        //}
            }

        }

        public InvoiceListRpt getInvoiceListReport(int baseCurrencyId, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, DateTime invoiceDateFrom, 
            DateTime invoiceDateTo, DateTime purchaseExtractDateFrom, DateTime purchaseExtractDateTo,
            int budgetYear, int periodFrom, int periodTo, DateTime departDateFrom, DateTime departDateTo, int vendorId, int termOfPurchaseId, TypeCollector officeIdList, int handlingOfficeId,
            int seasonId, TypeCollector productTeamList, int customerDestinationId, int oprFabricType, int isSZOrder, int isUTOrder, int isLDPOrder, int isSampleOrder, 
            TypeCollector customerIdList, TypeCollector tradingAgencyList, TypeCollector shipmentMethodList, int accountDocReceipt, string voyageNo, 
            int paymentTermId, int isAccPaid, int isLcPaymentChecked, int shippingDocReceiptDate, int userId, string sortingField, string version)
        {
            InvoiceListRpt invoiceRpt = new InvoiceListRpt();
            CommonWorker commonWorker = CommonWorker.Instance;

            InvoiceListReportDs ds = ReporterWorker.Instance.getInvoiceListByCriteria(baseCurrencyId, invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, invoiceDateFrom,
                invoiceDateTo, purchaseExtractDateFrom, purchaseExtractDateTo, budgetYear, periodFrom, 
                periodTo, departDateFrom, departDateTo, vendorId, termOfPurchaseId, officeIdList, handlingOfficeId, seasonId,
                productTeamList, customerDestinationId, oprFabricType, isSZOrder, isUTOrder, isLDPOrder, 
                isSampleOrder, customerIdList, tradingAgencyList, shipmentMethodList, accountDocReceipt, voyageNo, 
                paymentTermId, isAccPaid, isLcPaymentChecked, shippingDocReceiptDate, sortingField, version);
            
            invoiceRpt.SetDataSource(ds);
            invoiceRpt.SetParameterValue("InvoiceNoFrom", invoicePrefix == "" ? "" : ShippingWorker.getInvoiceNo(invoicePrefix, invoiceSeqFrom, invoiceYear));
            invoiceRpt.SetParameterValue("InvoiceNoTo", invoicePrefix == "" ? "" : ShippingWorker.getInvoiceNo(invoicePrefix, invoiceSeqTo, invoiceYear));
            invoiceRpt.SetParameterValue("InvoiceDateFrom", DateTimeUtility.getDateString(invoiceDateFrom));
            invoiceRpt.SetParameterValue("InvoiceDateTo", DateTimeUtility.getDateString(invoiceDateTo));
            invoiceRpt.SetParameterValue("PurchaseExtractDateFrom", DateTimeUtility.getDateString(purchaseExtractDateFrom));
            invoiceRpt.SetParameterValue("PurchaseExtractDateTo", DateTimeUtility.getDateString(purchaseExtractDateTo));

            invoiceRpt.SetParameterValue("CustomerDestination", customerDestinationId == -1 ? "ALL" : commonWorker.getCustomerDestinationByKey(customerDestinationId).DestinationDesc);
            invoiceRpt.SetParameterValue("OPRFabricType", oprFabricType);
            invoiceRpt.SetParameterValue("BaseCurrency", GeneralWorker.Instance.getCurrencyByKey(baseCurrencyId).CurrencyCode);

            if (ds.InvoiceListReport.Rows.Count != 0)
            {
                invoiceRpt.SetParameterValue("Season", seasonId == -1 ? "ALL" : ds.InvoiceListReport.Rows[0]["Season"].ToString());
                invoiceRpt.SetParameterValue("Supplier", vendorId == -1 ? "ALL" : ds.InvoiceListReport.Rows[0]["Vendor"].ToString());
                invoiceRpt.SetParameterValue("TermOfPurchase", termOfPurchaseId == -1 ? "ALL" : ds.InvoiceListReport.Rows[0]["TermOfPurchase"].ToString());
            }
            else
            {
                invoiceRpt.SetParameterValue("Season", seasonId == -1 ? "ALL" : GeneralWorker.Instance.getSeasonByKey(seasonId).Code);
                invoiceRpt.SetParameterValue("Supplier", vendorId == -1 ? "ALL" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
                invoiceRpt.SetParameterValue("TermOfPurchase", termOfPurchaseId == -1 ? "ALL" : commonWorker.getTermOfPurchaseByKey(termOfPurchaseId).TermOfPurchaseDescription);
            }

            string orderType = "";
            if (isSZOrder != -1)
            {
                orderType += isSZOrder == 1 ? "SZ Order" : "Non-SZ Order";
            }
            if (isUTOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isUTOrder == 1 ? "UT Order" : "Non-UT Order";
            }
            if (isLDPOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isLDPOrder == 1 ? "LDP Order" : "Non-LDP Order";
            }
            if (isSampleOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isSampleOrder == 1 ? "Mock Shop/Press/Studio Sample Order" : "Mainline Order";
            }

            invoiceRpt.SetParameterValue("OrderType", orderType == "" ? "ALL" : orderType);
            invoiceRpt.SetParameterValue("BudgetYear", budgetYear);
            invoiceRpt.SetParameterValue("PeriodFrom", periodFrom);
            invoiceRpt.SetParameterValue("PeriodTo", periodTo);

            string office = "";
            foreach (object id in officeIdList.Values)
            {
                office += GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).Description + ", ";
            }
            if (office != "")
            {
                office = office.Remove(office.LastIndexOf(", "));
            }
            invoiceRpt.SetParameterValue("OfficeCode", office);
            invoiceRpt.SetParameterValue("HandlingOffice", (handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).Description));

            string productTeam = "";
            foreach (object productTeamId in productTeamList.Values)
            {
                ProductCodeDef pc = GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32(productTeamId));
                if (!productTeam.Contains(pc.Code + ", "))
                    productTeam += pc.Code + ", ";
            }
            if (productTeam != "")
                productTeam = productTeam.Remove(productTeam.LastIndexOf(", "));
            invoiceRpt.SetParameterValue("ProductTeam", productTeam);

            if (customerIdList.Values.Count == 8)
                invoiceRpt.SetParameterValue("Customer", "ALL");
            else
            {
                string cust = "";
                foreach (object obj in customerIdList.Values)
                {
                    cust += commonWorker.getCustomerByKey(Convert.ToInt32(obj)).CustomerCode + ", ";
                }
                cust = cust.Remove(cust.LastIndexOf(", "));
                invoiceRpt.SetParameterValue("Customer", cust);
            }
            if (tradingAgencyList.Values.Count == 4)
                invoiceRpt.SetParameterValue("TradingAgency", "ALL");
            else
            {
                string tradingAgency = "";
                foreach (object obj in tradingAgencyList.Values)
                {
                    tradingAgency += commonWorker.getTradingAgencyByKey(Convert.ToInt32(obj)).ShortName + ", ";
                }
                tradingAgency = tradingAgency.Remove(tradingAgency.LastIndexOf(", "));
                invoiceRpt.SetParameterValue("TradingAgency", tradingAgency);
            }
            if (shipmentMethodList.Values.Count == 5)
                invoiceRpt.SetParameterValue("ShipmentMethod", "ALL");
            else
            {
                string shipmentMethod = "";
                foreach (object obj in shipmentMethodList.Values)
                {
                    shipmentMethod += commonWorker.getShipmentMethodByKey(Convert.ToInt32(obj)).ShipmentMethodDescription + ", ";
                }
                shipmentMethod = shipmentMethod.Remove(shipmentMethod.LastIndexOf(", "));
                invoiceRpt.SetParameterValue("ShipmentMethod", shipmentMethod);
            }
            invoiceRpt.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            invoiceRpt.SetParameterValue("AccountDocReceipt", (accountDocReceipt == 1 ? "Receipted" : (accountDocReceipt == 0 ? "Not Yet Receipt" : "ALL")));
            invoiceRpt.SetParameterValue("VoyageNo", voyageNo);
            invoiceRpt.SetParameterValue("DepartDateFrom", DateTimeUtility.getDateString(departDateFrom));
            invoiceRpt.SetParameterValue("DepartDateTo", DateTimeUtility.getDateString(departDateTo));

            invoiceRpt.SetParameterValue("PaymentTerm", (paymentTermId == -1 ? "ALL" : (paymentTermId == 1 ? "OPEN ACCOUNT" : (paymentTermId == 2 ? "L/C AT SIGHT" : "N/A"))));
            invoiceRpt.SetParameterValue("PaymentStatus", (isAccPaid == -1 ? "ALL" : (isAccPaid == 1 ? "PAID" : (isAccPaid == 0 ? "NOT YET PAY" : "N/A"))));
            invoiceRpt.SetParameterValue("LCPaymentChecked", (isLcPaymentChecked == -1 ? "ALL" : (isLcPaymentChecked == 1 ? "YES" : (isLcPaymentChecked == 0 ? "NO" : "N/A"))));
            invoiceRpt.SetParameterValue("IsUTOrder",isUTOrder);
            invoiceRpt.SetParameterValue("shippingDocReceiptDate", (shippingDocReceiptDate == -1 ? "ALL" : (shippingDocReceiptDate == 1 ? "YES" : (shippingDocReceiptDate == 0 ? "NO" : "N/A"))));
            return invoiceRpt;
        }



        public InvoiceListReportDs getInvoiceListDataSet(int baseCurrencyId, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, DateTime purchaseExtractDateFrom, DateTime purchaseExtractDateTo,
            int budgetYear, int periodFrom, int periodTo, DateTime departDateFrom, DateTime departDateTo, int vendorId, int termOfPurchaseId, TypeCollector officeIdList, int handlingOfficeId,
            int seasonId, TypeCollector productTeamList, int customerDestinationId, int oprFabricType, int isSZOrder, int isUTOrder, int isLDPOrder, int isSampleOrder,
            TypeCollector customerIdList, TypeCollector tradingAgencyList, TypeCollector shipmentMethodList, int accountDocReceipt, string voyageNo,
            int paymentTermId, int isAccPaid, int isLcPaymentChecked, int shippingDocReceiptDate, int userId, string sortingField, string version)
        {
            InvoiceListRpt invoiceRpt = new InvoiceListRpt();
            CommonWorker commonWorker = CommonWorker.Instance;

            InvoiceListReportDs ds = ReporterWorker.Instance.getInvoiceListByCriteria(baseCurrencyId, invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, invoiceDateFrom,
                invoiceDateTo, purchaseExtractDateFrom, purchaseExtractDateTo, budgetYear, periodFrom,
                periodTo, departDateFrom, departDateTo, vendorId, termOfPurchaseId, officeIdList, handlingOfficeId, seasonId,
                productTeamList, customerDestinationId, oprFabricType, isSZOrder, isUTOrder, isLDPOrder,
                isSampleOrder, customerIdList, tradingAgencyList, shipmentMethodList, accountDocReceipt, voyageNo,
                paymentTermId, isAccPaid, isLcPaymentChecked, shippingDocReceiptDate, sortingField, version);

            return ds;

        }


    }
}
