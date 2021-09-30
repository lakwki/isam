using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.shipping;
using com.next.isam.domain.common;
using com.next.isam.domain.ils;
using com.next.isam.domain.order;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.isam.domain.product;
using com.next.infra.persistency.transactions;
using com.next.infra.persistency.dataaccess;
using com.next.infra.util;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.common.domain.industry.vendor;
using com.next.common.datafactory.worker;
using com.next.common.web.commander;
using com.next.isam.dataserver.model.shipping;
using com.next.isam.reporter.shipping;
using com.next.common.domain.dms;

namespace com.next.isam.appserver.shipping
{
    public class ShipmentManager
    {
        private static ShipmentManager _instance;
        private ShippingWorker shippingWorker;
        private OrderSelectWorker orderSelectWorker;
        private OrderWorker orderWorker;
        private AccountWorker accountWorker;
        private AdvancePaymentWorker advancePaymentWorker;
        private GeneralWorker generalWorker;

        public ShipmentManager()
        {
            shippingWorker = ShippingWorker.Instance;
            orderSelectWorker = OrderSelectWorker.Instance;
            orderWorker = OrderWorker.Instance;
            accountWorker = AccountWorker.Instance;
            generalWorker = GeneralWorker.Instance;
            advancePaymentWorker = AdvancePaymentWorker.Instance;
        }

        public static ShipmentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShipmentManager();
                }
                return _instance;
            }
        }

        public ShipmentDef getShipmentById(int shipmentId)
        {
            return orderSelectWorker.getShipmentByKey(shipmentId);
        }

        public ArrayList getShipmentList(string contractNo, string invoiceNo, string itemNo, ArrayList officeList)
        {
            TypeCollector officeTypeCollector = TypeCollector.Inclusive;
            foreach (OfficeRef def in officeList)
            {
                officeTypeCollector.append(def.OfficeId);
            }


            return shippingWorker.getShipmentList(contractNo, invoiceNo, itemNo, officeTypeCollector);
        }

        public ArrayList getShipmentList(string contractNo, int deliveryNo, string itemNo, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo,
            int invoiceYear, int vendorId, int customerId, string supplierInvoiceNoFrom, string supplierInvoiceNoTo, TypeCollector officeList, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, int productTeamId, string orderType, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo,
            DateTime customerAtWarehouseDateFrom, DateTime customerAtWarehouseDateTo, DateTime ilsActualAtWHDateFrom, DateTime ilsActualAtWHDateTo,
            DateTime invoiceSentDateFrom, DateTime invoiceSentDateTo,
            int oprTypeId, int customerDestinationId, int countryOfOriginId, int termOfPurchaseId, string docNo, int invoiceUploadUserId,
            TypeCollector workflowStatusList, TypeCollector shipmentMethodList,
            int splitOnly, int szOrderOnly, int sampleOnly, int ldpOrder, int withQCCharge, int isReprocessGoods, int isGBTestRequired, int isQccInspection, int isTradingAirFreight)
        {

            return shippingWorker.getShipmentList(contractNo, deliveryNo, itemNo, invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear,
                         vendorId, customerId, supplierInvoiceNoFrom, supplierInvoiceNoTo, officeList, invoiceDateFrom, invoiceDateTo,
                         productTeamId, orderType, invoiceUploadDateFrom, invoiceUploadDateTo, customerAtWarehouseDateFrom, customerAtWarehouseDateTo,
                         ilsActualAtWHDateFrom, ilsActualAtWHDateTo, invoiceSentDateFrom, invoiceSentDateTo,
                         oprTypeId, customerDestinationId, countryOfOriginId, termOfPurchaseId, docNo, invoiceUploadUserId, workflowStatusList, shipmentMethodList,
                         splitOnly, szOrderOnly, sampleOnly, ldpOrder, withQCCharge, isReprocessGoods, isGBTestRequired, isQccInspection, isTradingAirFreight);
        }


        public ArrayList getShipmentList(string contractNo, int deliveryNo, string itemNo, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo,
            int invoiceYear, int vendorId, TypeCollector customerList, string supplierInvoiceNoFrom, string supplierInvoiceNoTo, TypeCollector officeList,
            DateTime invoiceDateFrom, DateTime invoiceDateTo, int productTeamId, string orderType, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo,
            DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo, DateTime ilsActualAtWHDateFrom, DateTime ilsActualAtWHDateTo,
            DateTime invoiceSentDateFrom, DateTime invoiceSentDateTo,
             int oprTypeId, int customerDestinationId, int countryOfOriginId, int termOfPurchaseId, string docNo, int invoiceUploadUserId,
            TypeCollector workflowStatusList, TypeCollector shipmentMethodList,
            int splitOnly, int szOrderOnly, int sampleOnly, int ldpOrder, int withQCCharge,
            string lcNoFrom, string lcNoTo, DateTime ActualAtWHDateFrom, DateTime ActualAtWHDateTo, int ShippingDocumentReceiptStatus, int LcPaymentCheckStatus,
            TypeCollector shippingUserIdList, string SortingOrder, int userId)
        {
            return shippingWorker.getShipmentList(contractNo, deliveryNo, itemNo, invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear,
                        vendorId, customerList, supplierInvoiceNoFrom, supplierInvoiceNoTo, officeList, invoiceDateFrom, invoiceDateTo,
                        productTeamId, orderType, invoiceUploadDateFrom, invoiceUploadDateTo, customerAtWHDateFrom, customerAtWHDateTo,
                        ilsActualAtWHDateFrom, ilsActualAtWHDateTo, invoiceSentDateFrom, invoiceSentDateTo,
                        oprTypeId, customerDestinationId, countryOfOriginId, termOfPurchaseId, docNo, invoiceUploadUserId, workflowStatusList, shipmentMethodList,
                        splitOnly, szOrderOnly, sampleOnly, ldpOrder, withQCCharge,
                        lcNoFrom, lcNoTo, ActualAtWHDateFrom, ActualAtWHDateTo, ShippingDocumentReceiptStatus, LcPaymentCheckStatus, shippingUserIdList, SortingOrder, userId);
        }

        /// <summary>
        /// 2017-12-12, Alan
        /// </summary>
        /// <param name="PaymentId">For particular Advance Payment(Id)</param>
        /// <returns></returns>
        public ArrayList getShipmentListByAdvancePaymentId(int paymentId)
        {
            return shippingWorker.getShipmentListByAdvancePaymentId(paymentId);
        }

        public ArrayList getInvoiceList(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, ArrayList officeList, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo,
            int termOfPurchaseId, int workflowStatusId, int tradingAgencyId, DateTime purchaseScanDateFrom, DateTime purchaseScanDateTo,
            int purchaseScanStatus, string orderType, int currencyId,
            DateTime salesScanDateFrom, DateTime salesScanDateTo, DateTime submittedOnFrom, DateTime submittedOnTo, string batchNo, int invoiceBatchStatus)
        {
            TypeCollector officeTypeCollector = TypeCollector.Inclusive;
            if (officeList != null)
            {
                foreach (OfficeRef def in officeList)
                {
                    officeTypeCollector.append(def.OfficeId);
                }
            }
            return shippingWorker.getInvoiceList(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, officeTypeCollector, invoiceDateFrom,
                     invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo,
                     termOfPurchaseId, workflowStatusId, tradingAgencyId, purchaseScanDateFrom, purchaseScanDateTo, purchaseScanStatus, orderType, currencyId,
                     salesScanDateFrom, salesScanDateTo, submittedOnFrom, submittedOnTo, batchNo, invoiceBatchStatus);
        }

        public ArrayList getInvoiceListForInvoiceBatch(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, int workflowStatusId, int tradingAgencyId, string orderType, int currencyId,
            DateTime salesScanDateFrom, DateTime salesScanDateTo, DateTime submittedOnFrom, DateTime submittedOnTo, string batchNo, int invoiceBatchStatus)
        {
            return shippingWorker.getInvoiceListForInvoiceBatch(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, invoiceDateFrom,
                invoiceDateTo, workflowStatusId, tradingAgencyId, orderType, currencyId,
                salesScanDateFrom, salesScanDateTo, submittedOnFrom, submittedOnTo, batchNo, invoiceBatchStatus);
        }

        public ArrayList getInvoiceListByLcBillRefNo(string lcBillRefNo, int workflowStatusId)
        {
            return shippingWorker.GetInvoiceListByLcBillRefNo(lcBillRefNo, workflowStatusId);
        }

        public ArrayList getInvoiceListByInvoiceNo(string invoiceNo, int sequenceNo, ArrayList officeList)
        {
            TypeCollector officeTypeCollector = TypeCollector.Inclusive;
            if (officeList != null)
            {
                foreach (OfficeRef def in officeList)
                {
                    officeTypeCollector.append(def.OfficeId);
                }
            }
            return shippingWorker.getInvoiceListByInvoiceNo(invoiceNo, sequenceNo, officeList == null ? null : officeTypeCollector);
        }

        public ArrayList getLast10Shipment(string itemNo, string contractNo)
        {
            return shippingWorker.getLast10Shipment(itemNo, contractNo);
        }

        public DateTime getLastShipmentDate(string itemNo, string contractNo, int vendorId)
        {
            return shippingWorker.getLastShipmentDate(itemNo, contractNo, vendorId);
        }

        public VendorOrderSummaryRef getVendorOrderSummary(int vendorId)
        {
            VendorOrderSummaryRef def = shippingWorker.getVendorOrderSummary(vendorId);
            return def;
        }

        public GenericOrderSummaryRef getItemOrderSummary(string itemNo, DateTime fromDate, DateTime toDate)
        {
            GenericOrderSummaryRef def = shippingWorker.getItemOrderSummary(itemNo, fromDate, toDate);
            return def;
        }

        public ArrayList GetShipmentProductByItemNo(string itemNo, string contractNo, int vendorId)
        {
            return shippingWorker.getShipmentProductByItemNo(itemNo, contractNo, vendorId);
        }

        public ArrayList GetShipmentProductByVendorId(int vendorId)
        {
            return shippingWorker.getShipmentProductByVendorId(vendorId);
        }

        public ContractShipmentListJDef getSplitShipmentByPONo(string contractNo, string splitSuffix, int deliveryNo)
        {
            return shippingWorker.getSplitShipmentByPONo(contractNo, splitSuffix, deliveryNo);
        }

        public ArrayList getLcShipmentByLcBillRefNo(string lcBillRefNo)
        {
            return shippingWorker.getLcShipmentByLcBillRefNo(lcBillRefNo);
        }

        public ArrayList getDocumentListByShipmentId(int shipmentId)
        {
            return shippingWorker.getDocumentListByShipmentId(shipmentId);
        }

        public ArrayList getShipmentDeductionList(int shipmentId)
        {
            return shippingWorker.getShipmentDeductionList(shipmentId);
        }

        public decimal calcShipmentDeductionTotal(ArrayList deductions)
        {
            return shippingWorker.calcShipmentDeductionTotal(deductions);
        }

        public DocumentDef getDocumentByKey(int docId)
        {
            return shippingWorker.getDocumentByKey(docId);
        }

        public InvoiceDef getInvoiceByShipmentId(int shipmentId)
        {
            return shippingWorker.getInvoiceByKey(shipmentId);
        }

        public InvoiceDef getInvoiceByInvoiceNo(string invoicePrefix, int invoiceSeq, int invoiceYear)
        {
            return shippingWorker.getInvoiceByInvoiceNo(invoicePrefix, invoiceSeq, invoiceYear);
        }

        public static string getInvoicePrefix(string invoiceNo)
        {
            return ShippingWorker.getInvoicePrefix(invoiceNo);
        }

        public static int getInvoiceSeq(string invoiceNo)
        {
            return ShippingWorker.getInvoiceSeq(invoiceNo);
        }

        public static string getInvoiceNo(string invoicePrefix, int invoiceSeq, int invoiceYear)
        {
            return ShippingWorker.getInvoiceNo(invoicePrefix, invoiceSeq, invoiceYear);
        }

        public static int getInvoiceYear(string invoiceNo)
        {
            return ShippingWorker.getInvoiceYear(invoiceNo);
        }

        public DomainShipmentDef getDomainShipmentDef(int shipmentId)
        {
            DomainShipmentDef domainDef = new DomainShipmentDef();
            CommonWorker commonWorker = CommonWorker.Instance;

            domainDef.Shipment = orderSelectWorker.getShipmentByKey(shipmentId);
            ArrayList shipmentDetailList = (ArrayList)orderSelectWorker.getShipmentDetailByShipmentId(shipmentId);
            domainDef.Invoice = shippingWorker.getInvoiceByKey(shipmentId);
            domainDef.Contract = orderSelectWorker.getContractByKey(domainDef.Shipment.ContractId);
            domainDef.Product = ProductWorker.Instance.getProductByKey(domainDef.Contract.ProductId);
            domainDef.ShipmentDetails = shipmentDetailList;
            domainDef.SplitShipments = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(shipmentId);
            domainDef.ActionHistoryList = shippingWorker.getActionHistoryByShipmentId(shipmentId);
            domainDef.Manifests = this.getManifestList(shipmentId);
            domainDef.ILSDocuments = ILSUploadWorker.Instance.getILSDocumentListByShipmentId(shipmentId);
            domainDef.Documents = shippingWorker.getDocumentListByShipmentId(shipmentId);
            /*
            domainDef.ILSSummary = new ILSSummaryRef();
            */
            domainDef.CustomerSummary = new CustomerSummaryRef();
            domainDef.ILSManifestDetail = ILSUploadWorker.Instance.getILSManifestDetailListByShipmentId(shipmentId);

            domainDef.OtherDelivery = shippingWorker.getShipmentList(domainDef.Contract.ContractNo, string.Empty, string.Empty, TypeCollector.createNew(domainDef.Contract.Office.OfficeId));

            if (domainDef.Shipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode())
            {
                domainDef.LCApplication = LCWorker.Instance.getLCApplicationShipmentByShipmentId(shipmentId, 0);
                if (domainDef.LCApplication != null && domainDef.LCApplication.Status == 0)
                    domainDef.LCApplication = null;
                if (domainDef.LCApplication != null)
                {
                    domainDef.LCBatch = LCWorker.Instance.getLCBatchByKey(domainDef.LCApplication.LCBatchId);
                    if (domainDef.LCBatch != null && domainDef.LCBatch.Status == 0)
                        domainDef.LCBatch = null;
                }
            }

            domainDef.COGSSunInterfaceLog = accountWorker.getInitialLogByShipmentId(SunInterfaceTypeRef.Id.Purchase.GetHashCode(), shipmentId, 0);
            domainDef.SalesSunInterfaceLog = accountWorker.getInitialLogByShipmentId((domainDef.Shipment.IsMockShopSample == 0 ? SunInterfaceTypeRef.Id.Sales.GetHashCode() : SunInterfaceTypeRef.Id.MockShopSales.GetHashCode()), shipmentId, 0);
            domainDef.SalesCommSunInterfaceLog = accountWorker.getInitialLogByShipmentId((domainDef.Shipment.IsMockShopSample == 0 ? SunInterfaceTypeRef.Id.SalesCommission.GetHashCode() : SunInterfaceTypeRef.Id.MockShopSalesCommission.GetHashCode()), shipmentId, 0);
            domainDef.APSunInterfaceLog = accountWorker.getInitialLogByShipmentId(SunInterfaceTypeRef.Id.AccountPayable.GetHashCode(), shipmentId, 0);
            domainDef.ARSunInterfaceLog = accountWorker.getInitialLogByShipmentId(SunInterfaceTypeRef.Id.AccountReceivable.GetHashCode(), shipmentId, 0);
            domainDef.ReversingEntry = accountWorker.getReversalLogListByShipmentId(GeneralCriteria.ALL, shipmentId);
            domainDef.PaymentDeduction = getShipmentDeductionList(shipmentId);

            domainDef.ILSManifest = new ArrayList();
            foreach (ILSManifestDetailDef ilsManifestDetail in domainDef.ILSManifestDetail)
            {
                domainDef.ILSManifest.Add(ILSUploadWorker.Instance.getILSManifestByKey(ilsManifestDetail.ContainerNo));
            }

            /*
            ILSOrderCopyDef orderCopyDef = ILSUploadWorker.Instance.getILSOrderCopyByShipmentId(shipmentId);
            */
            CustomerOrderCopyDef orderCopyDef = getCustomerOrderCopy(shipmentId, domainDef.Contract);
            ILSPackingListDef packingListDef = ILSUploadWorker.Instance.getILSPackingListByShipmentId(shipmentId);
            ILSInvoiceDef invoiceDef = ILSUploadWorker.Instance.getILSInvoiceByShipmentId(shipmentId);

            ArrayList packingListDetails = null;
            if (packingListDef != null && packingListDef.OrderRefId > 0)
            {
                packingListDetails = ILSUploadWorker.Instance.getILSPackingListDetail(packingListDef.OrderRefId, false);
                domainDef.ILSPackingListDetail = packingListDetails;
            }
            
            domainDef.CustomerSummary.Destination = string.Empty;
            if (packingListDef != null) // get the destination from ILS packing list in first priority
            {
                if (packingListDef.FinalDestination.Trim() == "DF")
                {
                    ShipmentPortRef portRef = commonWorker.getShipmentPortByCode(packingListDef.ArrivalPort);
                    if (portRef != null)
                        domainDef.CustomerSummary.Destination = portRef.ShipmentCountry.ShipmentCountryDescription;
                }
                else
                {
                    if (!string.IsNullOrEmpty(packingListDef.FinalDestination))
                    {
                        UKFinalDestinationDef finalDest = commonWorker.getUKFinalDestinationByCode(packingListDef.FinalDestination);
                        if (finalDest != null && finalDest.UKFinalDestinationCountryId != GeneralCriteria.ALL)
                            domainDef.CustomerSummary.Destination = commonWorker.getShipmentCountryByKey(finalDest.UKFinalDestinationCountryId).ShipmentCountryDescription;
                    }
                }
            }
            else if (orderCopyDef != null)
            {
                if (!string.IsNullOrEmpty(orderCopyDef.FinalDestination))
                {
                    if (orderCopyDef.FinalDestination.Trim() == "DF")
                    {
                        ShipmentPortRef portRef = commonWorker.getShipmentPortByCode(orderCopyDef.ArrivalPort);
                        if (portRef != null)
                            domainDef.CustomerSummary.Destination = portRef.ShipmentCountry.ShipmentCountryDescription;
                    }
                    else
                    {
                        UKFinalDestinationDef finalDest = commonWorker.getUKFinalDestinationByCode(orderCopyDef.FinalDestination);
                        if (finalDest != null && finalDest.UKFinalDestinationCountryId != GeneralCriteria.ALL)
                            domainDef.CustomerSummary.Destination = commonWorker.getShipmentCountryByKey(finalDest.UKFinalDestinationCountryId).ShipmentCountryDescription;
                    }
                }
            }

            if (orderCopyDef != null)
            {
                domainDef.CustomerSummary.DeliveryNo = (orderCopyDef.DeliveryNo == "" ? 0 : int.Parse(orderCopyDef.DeliveryNo));
                domainDef.CustomerSummary.AtWarehouseDate = orderCopyDef.ExFactoryDate;
                domainDef.CustomerSummary.OrderCurrency = orderCopyDef.Currency;
                CountryOfOriginRef coRef = generalWorker.getCountryOfOriginByCode(orderCopyDef.DepartCountry);
                if (coRef != null)
                    domainDef.CustomerSummary.Departure = coRef.Name;
                else
                    domainDef.CustomerSummary.Departure = orderCopyDef.DepartCountry;

             
                domainDef.CustomerSummary.FreightPercentForNSL = orderCopyDef.SupplierFreightPercent;
                domainDef.CustomerSummary.FreightPercentForNUK = orderCopyDef.NextFreightPercent;
                if (orderCopyDef.HangBox != string.Empty)
                    domainDef.CustomerSummary.PackingMethod = orderCopyDef.HangBox;
                if (orderCopyDef.SupplierCode != string.Empty)
                    domainDef.CustomerSummary.SupplierCode = orderCopyDef.SupplierCode;

                int methodId = ShipmentMethodRef.getShipmentMethodIdByDescription(orderCopyDef.TransportMode);
                if (methodId > -1)
                    domainDef.CustomerSummary.TransportMode = commonWorker.getShipmentMethodByKey(methodId).ShipmentMethodDescription;

                if (orderCopyDef.Refurb != null && orderCopyDef.Refurb != string.Empty)
                    domainDef.CustomerSummary.Refurb = (orderCopyDef.Refurb == "Y" ? "YES" : "NO");

                domainDef.CustomerSummary.TotalOrderQuantity = orderCopyDef.TotalOrderQuantity;
                domainDef.CustomerSummary.TotalOrderAmount = orderCopyDef.TotalOrderAmount;

                domainDef.CustomerSummary.PurchaseOrder = orderCopyDef.ContractNo;
                domainDef.CustomerSummary.PromotionStartDate = orderCopyDef.PromotionStartDate;
                domainDef.CustomerSummary.ScheduledDeliveryDate = orderCopyDef.ScheduledDeliveryDate;
                domainDef.CustomerSummary.ShipmentMethodDesc = orderCopyDef.TransportMode;
                domainDef.CustomerSummary.Forwarder = orderCopyDef.Forwarder;
            }

            if (packingListDef != null)
            {
                //domainDef.ILSSummary.DeliveryNo = int.Parse(packingListDef.DeliveryNo);
                domainDef.CustomerSummary.ActualAtWarehouseDate = packingListDef.HandoverDate;

                //domainDef.ILSSummary.Departure = generalWorker.getCountryOfOriginByCode(packingListDef.DepartPort.Substring(0,2)).Name;
                //ShipmentPortRef shipmentPort = commonWorker.getShipmentPortByCode(packingListDef.DepartPort);
                //if (shipmentPort != null)
                //    domainDef.ILSSummary.Departure += ", " + shipmentPort.ShipmentPortDescription;
                //UKFinalDestinationDef finalDest= commonWorker.getUKFinalDestinationByCode(packingListDef.FinalDestination);
                //if (finalDest != null && finalDest.UKFinalDestinationCountryId != GeneralCriteria.ALL)
                //{
                //    domainDef.ILSSummary.Destination = commonWorker.getShipmentCountryByKey(finalDest.UKFinalDestinationCountryId).ShipmentCountryDescription;
                //}
                //else
                //    domainDef.ILSSummary.Destination = string.Empty;
                domainDef.CustomerSummary.TotalShippedQuantity = packingListDef.TotalPieces;
                //domainDef.ILSSummary.PackingMethod = packingListDef.HangBox;
                domainDef.CustomerSummary.SupplierCode = packingListDef.SupplierCode;
                //domainDef.ILSSummary.TransportMode = commonWorker.getShipmentMethodByKey(ShipmentMethodRef.getShipmentMethodId(packingListDef.TransportMode)).ShipmentMethodDescription;
                //domainDef.ILSSummary.Refurb = packingListDef.Refurb == "Y" ? "YES" : "NO";
            }

            if (invoiceDef != null)
            {
                domainDef.CustomerSummary.InvoiceCurrency = invoiceDef.Currency;
                domainDef.CustomerSummary.TotalShippedAmount = invoiceDef.TotalAmount;

                domainDef.ILSInvoiceDetail = ILSUploadWorker.Instance.getILSInvoiceDetailList(invoiceDef.OrderRefId);

                foreach (ILSInvoiceDetailDef invoiceDetailDef in domainDef.ILSInvoiceDetail)
                {
                    domainDef.CustomerSummary.TotalShippedSupplierGarmentAmount += invoiceDetailDef.Qty * invoiceDetailDef.Price;
                }
            }
            return domainDef;
        }

        CustomerOrderCopyDef getCustomerOrderCopy(int shipmentId, ContractDef contract)
        {
            CustomerOrderCopyDef order = null;

            if (contract.Customer.CustomerId == 13)
            {   // Get order copy of Ezibuy
                order = CustomerOrderWorker.Instance.getCustomerOrderCopy(shipmentId, contract);
            }
            else
            {   // Get the order copy from ILS
                ILSOrderCopyDef ils = ILSUploadWorker.Instance.getILSOrderCopyByShipmentId(shipmentId);
                if (ils != null)
                {   // Get order copy from ILS
                    order = new CustomerOrderCopyDef();

                    order.OrderSource = "ILS";
                    order.OrderRefId = ils.OrderRefId;
                    order.ContractNo = ils.ContractNo;
                    order.DeliveryNo = ils.DeliveryNo;
                    order.ItemNo = ils.ItemNo;
                    order.ItemDesc = ils.ItemDesc;
                    order.TransportMode = ils.TransportMode;
                    order.CountryOfOrigin = ils.CountryOfOrigin;
                    order.DepartCountry = ils.DepartCountry;
                    order.ExFactoryDate = ils.ExFactoryDate;
                    order.InWarehouseDate = ils.InWarehouseDate;
                    order.SupplierCode = ils.SupplierCode;
                    order.SupplierName = ils.SupplierName;
                    order.HangBox = ils.HangBox;
                    order.BuyingTerms = ils.BuyingTerms;
                    order.FinalDestination = ils.FinalDestination;
                    order.Currency = ils.Currency;
                    order.NextFreightPercent = ils.NextFreightPercent;
                    order.SupplierFreightPercent = ils.SupplierFreightPercent;
                    order.ArrivalPort = ils.ArrivalPort;
                    order.FranchisePartnerCode = ils.FranchisePartnerCode;
                    order.Refurb = ils.Refurb;
                    order.FileNo = ils.FileNo;
                    order.ImportDate = ils.ImportDate;
                    order.LastSentLoadingPort = ils.LastSentLoadingPort;
                    order.LastSentOfficeCode = ils.LastSentOfficeCode;
                    order.LastSentQuota = ils.LastSentQuota;
                    order.LastSentDocType = ils.LastSentDocType;
                    order.IsValid = ils.IsValid;

                    order.Forwarder = string.Empty;
                    order.ScheduledDeliveryDate = DateTime.MinValue;
                    order.PromotionStartDate = DateTime.MinValue;
                    order.ShipFrom = string.Empty;
                    ArrayList orderCopyDetails = ILSUploadWorker.Instance.getILSOrderCopyDetailList(ils.OrderRefId);
                    foreach (ILSOrderCopyDetailDef dtl in orderCopyDetails)
                    {
                        order.TotalOrderQuantity += dtl.Qty;
                        order.TotalOrderAmount += (dtl.Qty * dtl.Price);
                    }
                }
            }
            return order;
        }

        public DomainSplitShipmentDef getDomainSplitShipmentDef(int splitShipmentId)
        {
            DomainSplitShipmentDef domainSplitShipmentDef = new DomainSplitShipmentDef();

            domainSplitShipmentDef.SplitShipment = orderSelectWorker.getSplitShipmentByKey(splitShipmentId);
            domainSplitShipmentDef.Shipment = orderSelectWorker.getShipmentByKey(domainSplitShipmentDef.SplitShipment.ShipmentId);
            domainSplitShipmentDef.Contract = orderSelectWorker.getContractByKey(domainSplitShipmentDef.Shipment.ContractId);
            domainSplitShipmentDef.SplitShipmentDetail = (ArrayList)orderSelectWorker.getSplitShipmentDetailBySplitShipmentId(splitShipmentId);
            domainSplitShipmentDef.Invoice = shippingWorker.getInvoiceByKey(domainSplitShipmentDef.SplitShipment.ShipmentId);

            if (domainSplitShipmentDef.Shipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode())
            {
                domainSplitShipmentDef.LCApplication = LCWorker.Instance.getLCApplicationShipmentByShipmentId(domainSplitShipmentDef.Shipment.ShipmentId, splitShipmentId);
                if (domainSplitShipmentDef.LCApplication != null)
                    domainSplitShipmentDef.LCBatch = LCWorker.Instance.getLCBatchByKey(domainSplitShipmentDef.LCApplication.LCBatchId);
            }
            return domainSplitShipmentDef;
        }

        public void cancelInvoice(InvoiceDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList amendmentList = new ArrayList();

                shippingWorker.resetShippedQty(def.ShipmentId);

                def.InvoicePrefix = String.Empty;
                def.InvoiceSeqNo = 0;
                def.InvoiceYear = 0;
                def.InvoiceDate = DateTime.MinValue;
                def.InvoiceUploadDate = DateTime.MinValue;
                def.InvoiceUploadUser = null;
                def.NSLCommissionAmt = 0;
                def.InvoiceBuyExchangeRate = 0;
                def.InvoiceSellExchangeRate = 0;
                def.ILSActualAtWarehouseDate = DateTime.MinValue;
                shippingWorker.updateInvoice(def, ActionHistoryType.CANCEL_INVOICE, null, userId);

                amendmentList.Add(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.CANCEL_INVOICE, AmendmentType.INVOICE_DATE, String.Empty, userId));
                accountWorker.doReversal(def.ShipmentId, amendmentList);

                shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.CANCEL_INVOICE, "N/A", GeneralStatus.ACTIVE.Code, userId));

                ctx.VoteCommit();
                SynchronizationWorker.Instance.SyncNssShipment(def.ShipmentId);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private ArrayList getManifestList(int shipmentId)
        {
            bool isFirst = true;
            ILSManifestDef manifestDef = null;
            ArrayList list = ILSUploadWorker.Instance.getILSManifestDetailListByShipmentId(shipmentId);
            ArrayList returnList = new ArrayList();
            foreach (ILSManifestDetailDef detailDef in list)
            {
                if (isFirst)
                {
                    manifestDef = ILSUploadWorker.Instance.getILSManifestByKey(detailDef.ContainerNo);
                    isFirst = false;
                }
                ManifestDef def = new ManifestDef();
                def.ConfirmedToShipDate = detailDef.ConfirmedToShipDate;
                def.DepartureDate = manifestDef.DepartDate;
                def.ContainerNo = manifestDef.ContainerNo;
                def.CarrierContainerNo = manifestDef.PartnerContainerNo;
                def.VoyageNo = manifestDef.VoyageNo;
                def.AirwayBillNo = String.Empty; // To be confirmed by Gary Ng
                returnList.Add(def);
            }
            return returnList;
        }


        public ArrayList getMultiShipmentInvoiceList(ArrayList shipmentIdList)
        {
            ShipmentDef _shipment;
            ContractDef _contract;
            InvoiceDef _invoice;
            ProductDef _product;
            ArrayList _shipmentDetails;
            MultiShipmentInvoiceDef def;
            CommonWorker commonWorker = CommonWorker.Instance;

            ArrayList MultiShipmentInvoiceList = new ArrayList();
            foreach (int shipmentId in shipmentIdList)
            {
                _shipment = orderSelectWorker.getShipmentByKey(shipmentId);
                _shipmentDetails = (ArrayList)orderSelectWorker.getShipmentDetailByShipmentId(shipmentId);
                _invoice = shippingWorker.getInvoiceByKey(shipmentId);
                _contract = orderSelectWorker.getContractByKey(_shipment.ContractId);
                _product = ProductWorker.Instance.getProductByKey(_contract.ProductId);
                if (_shipment != null)
                {
                    def = new MultiShipmentInvoiceDef();
                    def.ShipmentIdList = shipmentIdList;
                    def.Shipment = _shipment;
                    def.Contract = _contract;
                    def.Invoice = _invoice;
                    def.Product = _product;
                    def.ShipmentDetails = _shipmentDetails;
                    MultiShipmentInvoiceList.Add(def);
                }
            }
            return MultiShipmentInvoiceList;
        }


        public ArrayList getMultiShipmentInvoiceDetailList(ArrayList shipmentIdList)
        {
            ShipmentDef _shipment;
            ContractDef _contract;
            InvoiceDef _invoice;
            ProductDef _product;
            ArrayList _shipmentDetails;
            MultiShipmentInvoiceDetailDef msDef;
            int OrderQty, POQty, ShippedQty;
            decimal SellingAmt, SupplierAmt;
            int LineNo, SequenceNo = 0;
            int RowIdx = -1;
            int Remainder;
            int ShipmentStartRow;
            int i;
            string UKProductGroupCode, ContractPrefix;
            bool Invoiced;
            ArrayList ctrList;
            ActionHistoryDef act;
            string remark = "";

            CommonWorker commonWorker = CommonWorker.Instance;
            ArrayList MultiShipmentInvoiceDetailList = new ArrayList();
            foreach (int shipmentId in shipmentIdList)
            {
                _shipment = orderSelectWorker.getShipmentByKey(shipmentId);
                remark += (remark == "" ? "" : ", ") + (string.IsNullOrEmpty(shipmentId.ToString()) ? "NULL" : shipmentId.ToString());
                if (_shipment == null)
                {   // Debuging 
                    remark = "Debug:Err in Multiple Shipment,ShipmentId:count=" + shipmentIdList.Count.ToString() + ",Para=" + remark + "<-Fail]";
                    remark = (remark.Length > 255 ? remark.Substring(0, 250) + " ..." : remark);
                    act = new ActionHistoryDef();
                    act.ActionDate = DateTime.Now;
                    act.ActionHistoryType = ActionHistoryType.SHIPPING_UPDATES;
                    act.ShipmentId = -1;
                    act.SplitShipmentId = -1;
                    act.Status = 0;
                    act.Remark = remark;
                    act.User = generalWorker.getUserByKey(99999);
                    shippingWorker.updateActionHistory(act);
                    break;
                }
                _shipmentDetails = (ArrayList)orderSelectWorker.getShipmentDetailByShipmentId(shipmentId);
                _invoice = shippingWorker.getInvoiceByKey(shipmentId);
                _contract = orderSelectWorker.getContractByKey(_shipment.ContractId);
                _product = ProductWorker.Instance.getProductByKey(_contract.ProductId);
                if (_shipmentDetails != null)
                {
                    OrderQty = 0;
                    POQty = 0;
                    ShippedQty = 0;
                    SellingAmt = 0;
                    SupplierAmt = 0;
                    LineNo = 0;
                    SequenceNo++;
                    Invoiced = (string.IsNullOrEmpty(_invoice.InvoicePrefix) && _invoice.InvoiceSeqNo > 0 && _invoice.InvoiceYear > 0);

                    UKProductGroupCode = null;
                    ctrList = shippingWorker.getLatest10UKProductGroupByItemNo(_product.ItemNo);
                    UKProductGroupCode = _contract.UKProductGroupCode.Trim();
                    if (string.IsNullOrEmpty(UKProductGroupCode))
                        for (i = 0, UKProductGroupCode = ""; i < ctrList.Count; i++)
                        {
                            ContractPrefix = ((ContractShipmentListJDef)ctrList[i]).ContractNo.Substring(0, 2);
                            if (UKProductGroupCode.Length + 3 <= 10 && UKProductGroupCode.IndexOf(ContractPrefix) < 0)
                                UKProductGroupCode += (UKProductGroupCode == "" ? "" : "/") + ContractPrefix;
                            else
                                break;  // maximum length is 10
                        }

                    ShipmentStartRow = RowIdx + 1;
                    foreach (ShipmentDetailDef shpDtl in _shipmentDetails)
                    {
                        msDef = new MultiShipmentInvoiceDetailDef();
                        msDef.Invoiced = Invoiced;
                        msDef.SequenceNo = SequenceNo;
                        msDef.NoOfSize = _shipmentDetails.Count;
                        msDef.LineNo = ++LineNo;
                        msDef.UKProductGroupCode = UKProductGroupCode;
                        msDef.ShipmentIdList = shipmentIdList;
                        msDef.Shipment = _shipment;
                        msDef.Contract = _contract;
                        msDef.Invoice = _invoice;
                        msDef.Product = _product;
                        msDef.ShipmentDetail = shpDtl;
                        MultiShipmentInvoiceDetailList.Add(msDef);
                        RowIdx++;

                        // Calculating total
                        OrderQty += shpDtl.OrderQuantity;
                        POQty += shpDtl.POQuantity;
                        ShippedQty += shpDtl.ShippedQuantity;
                        SellingAmt += shpDtl.SellingPrice * shpDtl.ShippedQuantity;
                        SupplierAmt += shpDtl.SupplierGarmentPrice * shpDtl.ShippedQuantity;
                    }
                    // Shipment total row
                    msDef = new MultiShipmentInvoiceDetailDef();
                    msDef.Invoiced = Invoiced;
                    msDef.SequenceNo = SequenceNo;
                    msDef.NoOfSize = _shipmentDetails.Count;
                    msDef.LineNo = ++LineNo;
                    msDef.UKProductGroupCode = UKProductGroupCode;
                    msDef.ShipmentIdList = shipmentIdList;
                    msDef.Shipment = _shipment;
                    msDef.Contract = _contract;
                    msDef.Invoice = _invoice;
                    msDef.Product = _product;

                    msDef.ShipmentDetail = new ShipmentDetailDef();
                    msDef.ShipmentDetail.OrderQuantity = OrderQty;
                    msDef.ShipmentDetail.POQuantity = POQty;
                    msDef.ShipmentDetail.ShippedQuantity = ShippedQty;
                    msDef.TotalSellingAmount = SellingAmt;
                    msDef.TotalSupplierAmount = SupplierAmt;
                    MultiShipmentInvoiceDetailList.Add(msDef);
                    RowIdx++;
                    for (i = ShipmentStartRow; i < MultiShipmentInvoiceDetailList.Count; i++)
                    {   //Update shipment summary field
                        ((MultiShipmentInvoiceDetailDef)MultiShipmentInvoiceDetailList[i]).NoOfLine = RowIdx - ShipmentStartRow + 1;
                        ((MultiShipmentInvoiceDetailDef)MultiShipmentInvoiceDetailList[i]).TotalSellingAmount = SellingAmt;
                        ((MultiShipmentInvoiceDetailDef)MultiShipmentInvoiceDetailList[i]).TotalSupplierAmount = SupplierAmt;
                    }

                    // Insert empty row
                    msDef = null;
                    // Empty row for formatting
                    Math.DivRem(RowIdx, 2, out Remainder);
                    if (Remainder == 1)
                    {
                        MultiShipmentInvoiceDetailList.Add(msDef);
                        RowIdx++;
                    }
                    // Empty row for separator line
                    MultiShipmentInvoiceDetailList.Add(msDef);
                    RowIdx++;

                }
            }
            if (MultiShipmentInvoiceDetailList.Count > 0)
                return MultiShipmentInvoiceDetailList;
            else
                return null;
        }


        public void updateMultipleShipmentWithSingleInvoiceNo_ToBeRemoved(ArrayList multiShipmentInvoiceList, string invoicePrefix, int invoiceSeq, int invoiceYear, DateTime invoiceDate, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                string invPfx = null;
                int invSeq = 0;
                int invYear = 0;
                DateTime invDate = DateTime.MinValue;
                ArrayList shipDetail = new ArrayList();

                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                invPfx = invoicePrefix;
                invSeq = invoiceSeq;
                invYear = invoiceYear;
                invDate = invoiceDate;
                foreach (InvoiceDef invoice in multiShipmentInvoiceList)
                {
                    shipDetail.Clear();
                    foreach (ShipmentDetailDef row in multiShipmentInvoiceList)
                    {
                        if (row.ShipmentId == invoice.ShipmentId)
                        {
                            shipDetail.Add(row);
                        }
                    }
                    if (shipDetail != null)
                    {
                        if (invPfx != null)
                        {
                            invoice.InvoicePrefix = invPfx;
                            invoice.InvoiceSeqNo = invSeq;
                            invoice.InvoiceYear = invYear;
                            invoice.InvoiceDate = invDate;
                        }
                        updateShipment(invoice, shipDetail, null, userId);
                        if (invPfx == null)
                        {
                            invPfx = invoice.InvoicePrefix;
                            invSeq = invoice.InvoiceSeqNo;
                            invYear = invoice.InvoiceYear;
                            invDate = invoice.InvoiceDate;
                        }
                    }
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }

        }

        public bool isReadyForDMS(int shipmentId)
        {
            ContractShipmentListJDef contractShipment = shippingWorker.getInvoiceByShipmentId(shipmentId);
            return isReadyForDMS(contractShipment);
        }

        public bool isReadyForDMS(ContractShipmentListJDef contractShipment)
        {
            return isReadyForDMS(contractShipment.Office, contractShipment.WorkflowStatus, contractShipment.InvoiceDate, contractShipment.IsUploadDMSDocument);
        }

        public bool isReadyForDMS(DomainSplitShipmentDef split)
        {
            return isReadyForDMS(split.Contract.Office, split.Shipment.WorkflowStatus, split.Invoice.InvoiceDate, split.Invoice.IsUploadDMSDocument);
        }

        public bool isReadyForDMS(DomainShipmentDef shipment)
        {
            return isReadyForDMS(shipment.Contract.Office, shipment.Shipment.WorkflowStatus, shipment.Invoice.InvoiceDate, shipment.Invoice.IsUploadDMSDocument);
        }

        public bool isReadyForDMS(OfficeRef office, ContractWFS WFS, DateTime invoiceDate, bool isUploadDMSDocument)
        {
            bool isReady = false;
            if (WFS.Id == ContractWFS.INVOICED.Id)
            {
                if ("HK,DG,CA,FB".Contains(office.OfficeCode) && invoiceDate >= DateTime.Parse("2011-04-03"))
                    isReady = true;
                else if ("LK,IN,ND".Contains(office.OfficeCode) && isUploadDMSDocument)
                    isReady = true;
                else if ("SH,TH,VN,BD,PK".Contains(office.OfficeCode) && invoiceDate >= DateTime.Parse("2011-08-15"))
                    isReady = true;
                else if ("TR,EG".Contains(office.OfficeCode) && isUploadDMSDocument && invoiceDate >= DateTime.Parse("2011-08-15"))
                    isReady = true;
            }
            return isReady;
        }

        private bool isShipDocInfoValidInSplitShipment(int shipmentId)
        {
            bool valid = true;
            ArrayList splitList = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(shipmentId);
            if (splitList != null)
                foreach (SplitShipmentDef split in splitList)
                    if (split.IsVirtualSetSplit == 0 && (string.IsNullOrEmpty(split.SupplierInvoiceNo)))//|| split.ShippingDocReceiptDate == DateTime.MinValue)
                    {
                        valid = false;
                        break;
                    }
            return valid;
        }


        //public void ShipmentListMassUpdate(ArrayList shipmentIdList, DateTime lcPaymentCheckDate, DateTime shippingDocumentReceiptDate, bool UpdateShippingDocStatus, int userId)
        public void ShipmentListMassUpdate(ArrayList shipmentIdList, DateTime lcPaymentCheckDate, DateTime shippingDocumentReceiptDate, string lcBillRefNo, int userId)
        {
            InvoiceDef inv;
            ShipmentDef shp;
            ContractDef cnt;
            string remark;
            ArrayList amendmentList = new ArrayList();
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UserRef usr = CommonUtil.getUserByKey(userId);
                DateTime nextWorkingDate = DateTimeUtility.getNextWorkingDate(DateTime.Now);
                foreach (int shpId in shipmentIdList)
                {
                    cnt = orderSelectWorker.getContractByShipmentId(shpId);
                    shp = orderSelectWorker.getShipmentByKey(shpId);
                    inv = shippingWorker.getInvoiceByKey(shpId);
                    if (lcPaymentCheckDate != DateTime.MinValue)
                    {
                        inv.LCPaymentCheckedDate = lcPaymentCheckDate;
                        inv.IsLCPaymentChecked = 1;
                    }
                    DateTime prevShippingDocReceiptDate = inv.ShippingDocReceiptDate;
                    //inv.ShippingDocReceiptDate = shippingDocumentReceiptDate;

                    // Update Shipping Doc Receipt Date and Shipping Doc Status for the split shipments
                    decimal splitCheckedNetAmt = 0;
                    decimal splitCheckedTotalNetAmt = 0;
                    bool isSplit = false;
                    bool splitShipDocInfoCompleted = true;
                    ArrayList splitShipmentList = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(shpId);
                    if (splitShipmentList != null)
                        foreach (SplitShipmentDef spt in splitShipmentList)
                            if (spt.IsVirtualSetSplit == 0)
                            {
                                isSplit = true;
                                if (string.IsNullOrEmpty(spt.SupplierInvoiceNo) || (spt.ShippingDocReceiptDate == DateTime.MinValue && shippingDocumentReceiptDate == DateTime.MinValue))
                                    splitShipDocInfoCompleted = false;
                            }
                    if (isSplit)
                    {   // Update Shipping Doc status in split shipments
                        foreach (SplitShipmentDef spt in splitShipmentList)
                            if (spt.IsVirtualSetSplit == 0)
                            {
                                //if (ShipmentManager.Instance.isReadyForDMS(cnt.Office, shp.WorkflowStatus, inv.InvoiceDate, spt.Vendor))
                                if (ShipmentManager.Instance.isReadyForDMS(cnt.Office, shp.WorkflowStatus, inv.InvoiceDate, inv.IsUploadDMSDocument))
                                {
                                    splitCheckedNetAmt = spt.TotalShippedSupplierGarmentAmountAfterDiscount
                                                        - Math.Round(spt.TotalShippedSupplierGarmentAmountAfterDiscount * (spt.QACommissionPercent / 100), 2, MidpointRounding.AwayFromZero);
                                    //- Math.Round(spt.TotalShippedSupplierGarmentAmountAfterDiscount * (spt.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero);

                                    if (spt.ShippingDocWFS.Id == ShippingDocWFS.NOT_READY.Id || spt.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                                    {
                                        splitCheckedTotalNetAmt += splitCheckedNetAmt;
                                        if (spt.ShippingDocReceiptDate == DateTime.MinValue)
                                            spt.ShippingDocReceiptDate = shippingDocumentReceiptDate;
                                        if (splitShipDocInfoCompleted)
                                        {
                                            if (spt.ShippingDocCheckedOn == DateTime.MinValue)
                                            {   // update the shipping doc info in split record
                                                spt.ShippingCheckedTotalNetAmount = splitCheckedNetAmt;
                                                spt.ShippingDocCheckedOn = DateTime.Now;
                                                spt.ShippingDocCheckedBy = usr;
                                                if (spt.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                                                {
                                                    spt.RejectPaymentReasonId = RejectPaymentReason.NoReason.Id;
                                                    spt.AccountDocReceiptDate = nextWorkingDate;
                                                    spt.ShippingDocWFS = ShippingDocWFS.ACCEPTED;
                                                    //if (spt.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.OPENACCOUNT.GetHashCode()
                                                    //    || (spt.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode() 
                                                    //        && (lcPaymentCheckDate!=DateTime.MinValue || spt.LCPaymentCheckedDate != DateTime.MinValue)
                                                    //        )
                                                    //   )
                                                    //{
                                                    //    spt.AccountDocReceiptDate = nextWorkingDate;
                                                    //}
                                                }
                                            }
                                            //else { } not allow to Clear the Shipping Doc Check status
                                        }
                                    }
                                }
                                else
                                {
                                    if (spt.ShippingDocReceiptDate == DateTime.MinValue)
                                        spt.ShippingDocReceiptDate = shippingDocumentReceiptDate;
                                }
                            }
                        orderWorker.addSplitShipmentDMSActionLog(splitShipmentList, userId);
                        orderWorker.updateSplitShipmentList(splitShipmentList, userId);
                    }

                    //if ((shippingDocumentReceiptDate != DateTime.MinValue))
                    {   // Update Shipping Doc status in shipment
                        //if (ShipmentManager.Instance.isReadyForDMS(cnt.Office, shp.WorkflowStatus, inv.InvoiceDate, shp.Vendor))
                        if (ShipmentManager.Instance.isReadyForDMS(cnt.Office, shp.WorkflowStatus, inv.InvoiceDate, inv.IsUploadDMSDocument))
                        {
                            if (shp.ShippingDocWFS.Id == ShippingDocWFS.NOT_READY.Id || shp.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                            {
                                if (shippingDocumentReceiptDate != DateTime.MinValue)
                                    inv.ShippingDocReceiptDate = shippingDocumentReceiptDate;

                                if (!string.IsNullOrEmpty(inv.SupplierInvoiceNo) && inv.ShippingDocReceiptDate != DateTime.MinValue && splitShipDocInfoCompleted)// && inv.PiecesPerDeliveryUnit > 0 && inv.ShippingDocReceiptDate != DateTime.MinValue)
                                {
                                    ShippingDocWFS prevShippingDocWFS = shp.ShippingDocWFS;
                                    UserRef prevCheckedBy = inv.ShippingDocCheckedBy;
                                    DateTime prevCheckedDate = inv.ShippingDocCheckedOn;

                                    decimal totalSupAmt = shp.TotalShippedSupplierGarmentAmountAfterDiscount;
                                    if (isSplit)
                                        inv.ShippingCheckedTotalNetAmount = splitCheckedTotalNetAmt;
                                    else
                                        inv.ShippingCheckedTotalNetAmount = (totalSupAmt
                                                                            - Math.Round(totalSupAmt * (shp.QACommissionPercent / 100), 2, MidpointRounding.AwayFromZero));
                                    //- Math.Round(totalSupAmt * (shp.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero));

                                    inv.ShippingDocCheckedBy = CommonUtil.getUserByKey(userId);
                                    inv.ShippingDocCheckedOn = DateTime.Now;

                                    if (shp.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                                    {
                                        shp.ShippingDocWFS = ShippingDocWFS.ACCEPTED;
                                        shp.RejectPaymentReasonId = RejectPaymentReason.NoReason.Id;
                                        inv.AccountDocReceiptDate = DateTimeUtility.getNextWorkingDate(DateTime.Now);
                                        //if (shp.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.OPENACCOUNT.GetHashCode()
                                        //    || (shp.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode()
                                        //        && lcPaymentCheckDate != DateTime.MinValue))
                                        //{
                                        //    inv.AccountDocReceiptDate = getNextWorkingDate(DateTime.Now);
                                        //}

                                        ArrayList lst = new ArrayList();
                                        lst.Add(shp);
                                        orderWorker.updateShipmentList(lst);

                                        // Add entry to amendment list
                                        if (prevShippingDocWFS.Id != shp.ShippingDocWFS.Id)
                                        {
                                            remark = "Shipping Doc. Status : " + (prevShippingDocWFS == null ? "" : prevShippingDocWFS.Name)
                                                    + " --> " + (shp.ShippingDocWFS == null ? "" : shp.ShippingDocWFS.Name);
                                            amendmentList.Add(new ActionHistoryDef(shp.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                                        }
                                    }
                                    remark = "Shipping Doc. Checked Date : " + (prevCheckedDate == DateTime.MinValue ? "" : prevCheckedDate.ToString("dd/MM/yyyy HH:mm:ss"))
                                           + " --> " + (inv.ShippingDocCheckedOn == DateTime.MinValue ? "" : inv.ShippingDocCheckedOn.ToString("dd/MM/yyyy HH:mm:ss"));
                                    amendmentList.Add(new ActionHistoryDef(inv.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));

                                    remark = "Shipping Doc. Checked By : " + (prevCheckedBy == null ? "" : prevCheckedBy.DisplayName)
                                           + " --> " + (inv.ShippingDocCheckedBy == null ? "" : inv.ShippingDocCheckedBy.DisplayName);
                                    amendmentList.Add(new ActionHistoryDef(inv.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                                }
                            }
                        }
                        else
                            if (shippingDocumentReceiptDate != DateTime.MinValue)
                                inv.ShippingDocReceiptDate = shippingDocumentReceiptDate;
                    }
                    if (lcBillRefNo != string.Empty && lcBillRefNo.Trim()!=inv.LCBillRefNo.Trim())
                    {
                        remark = "L/C Bill Ref. No. : " + (inv.LCBillRefNo == null ? "" : inv.LCBillRefNo) + " --> " + lcBillRefNo;
                        amendmentList.Add(new ActionHistoryDef(inv.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                        inv.LCBillRefNo = lcBillRefNo;
                    }
                    shippingWorker.updateInvoiceNoSync(inv, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);
                }

                if (amendmentList.Count > 0)
                {
                    foreach (ActionHistoryDef actionHistory in amendmentList)
                    {
                        shippingWorker.updateActionHistory(actionHistory);
                    }
                }


                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }

        }

        public void updateShipment(InvoiceDef invoiceDef, ArrayList shipmentDetails, ArrayList documents, int userId)
        {
            //updateShipment(invoiceDef, shipmentDetails, documents, null, null, userId);
            updateShipment(invoiceDef, shipmentDetails, documents, null, null, null, userId);
        }

        //public void updateShipment(InvoiceDef invoiceDef, ArrayList shipmentDetails, ArrayList documents, ShipmentDef shipmentDef, ArrayList splitShipments, int userId)
        public void updateShipment(InvoiceDef invoiceDef, ArrayList shipmentDetails, ArrayList documents, ShipmentDef shipmentDef, ArrayList splitShipments, ArrayList paymentDeduction, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList amendmentList = new ArrayList();

                orderWorker.updateShipmentDetailList(shipmentDetails, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);

                if (documents != null)
                {
                    foreach (DocumentDef docDef in documents)
                    {
                        shippingWorker.updateDocument(docDef, userId);
                    }
                }

                if (paymentDeduction != null)
                {
                    decimal fabricAdvance = decimal.MinValue;
                    foreach (ShipmentDeductionDef deduction in paymentDeduction)
                    {
                        shippingWorker.updateShipmentDeduction(deduction, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);
                        if (deduction.Status == 1 && deduction.DeductionType == PaymentDeductionType.FABRIC_ADVANCE)
                            fabricAdvance = deduction.Amount * deduction.DeductionType.Factor;

                        if (fabricAdvance != decimal.MinValue)
                        {
                            List<AdvancePaymentOrderDetailDef> advPayment = advancePaymentWorker.getAdvancePaymentOrderDetailByShipmentId(shipmentDef.ShipmentId);
                            int idx = 0;
                            foreach(AdvancePaymentOrderDetailDef advPaymentDetailDef in advPayment)
                            {
                                AdvancePaymentDef advPaymentDef = advancePaymentWorker.getAdvancePaymentByKey(advPaymentDetailDef.PaymentId);
                                if (advPaymentDef.PaymentNo == deduction.DocumentNo)
                                {
                                    advPaymentDetailDef.ActualValue = fabricAdvance;
                                    advancePaymentWorker.updateAdvancePaymentOrderDetailList(advPaymentDetailDef.PaymentId, advPayment.GetRange(idx, 1), userId);
                                }
                                idx += 1;
                            }
                        }
                    
                    }
                }

                bool isQtyChanged = false;

                if (amendmentList.Count > 0)
                {
                    foreach (ActionHistoryDef actionHistory in amendmentList)
                    {
                        if (actionHistory.AmendmentType.Id == AmendmentType.ACTUAL_QUANTITY.Id)
                        {
                            isQtyChanged = true;
                        }
                    }
                }


                InvoiceDef liveInvoice = shippingWorker.getInvoiceByKey(invoiceDef.ShipmentId);
                bool ShipDocChecked = (invoiceDef.ShippingDocCheckedOn != DateTime.MinValue);
                bool prevShipDocChecked = (liveInvoice.ShippingDocCheckedOn != DateTime.MinValue);
                ArrayList splitShipmentList = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(invoiceDef.ShipmentId);
                if (splitShipmentList != null)
                    if (splitShipmentList.Count > 0)
                    {
                        foreach (SplitShipmentDef spt in splitShipmentList)
                            if (spt.IsVirtualSetSplit == 0)
                            {
                                //update shipping receipt doc date in split shipment
                                if (liveInvoice.ShippingDocReceiptDate != invoiceDef.ShippingDocReceiptDate)
                                    if (spt.ShippingDocReceiptDate == null || spt.ShippingDocReceiptDate == DateTime.MinValue || spt.ShippingDocReceiptDate == liveInvoice.ShippingDocReceiptDate)
                                        spt.ShippingDocReceiptDate = invoiceDef.ShippingDocReceiptDate;

                                // Update the shipping doc info in Split Shipment
                                if (ShipDocChecked != prevShipDocChecked)
                                {
                                    spt.ShippingDocWFS = shipmentDef.ShippingDocWFS;
                                    spt.RejectPaymentReasonId = shipmentDef.RejectPaymentReasonId;
                                    spt.AccountDocReceiptDate = invoiceDef.AccountDocReceiptDate;
                                    //if (spt.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.OPENACCOUNT.GetHashCode()
                                    //    || (spt.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode()
                                    //        && (invoiceDef.IsLCPaymentChecked == 1 || spt.IsLCPaymentChecked == 1)))
                                    //{
                                    //    spt.AccountDocReceiptDate = invoiceDef.AccountDocReceiptDate;
                                    //}
                                    spt.ShippingDocCheckedBy = invoiceDef.ShippingDocCheckedBy;
                                    spt.ShippingDocCheckedOn = invoiceDef.ShippingDocCheckedOn;
                                    decimal CheckedNetAmt = 0;
                                    if (ShipDocChecked)
                                        CheckedNetAmt = spt.ShippingCheckedTotalNetAmount = spt.TotalShippedSupplierGarmentAmountAfterDiscount
                                                    - Math.Round(spt.TotalShippedSupplierGarmentAmountAfterDiscount * (spt.QACommissionPercent / 100), 2, MidpointRounding.AwayFromZero);
                                    // - Math.Round(spt.TotalShippedSupplierGarmentAmountAfterDiscount * (spt.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero));
                                    if (spt.ShippingCheckedTotalNetAmount != CheckedNetAmt)
                                        spt.ShippingCheckedTotalNetAmount = CheckedNetAmt;
                                }
                            }
                        orderWorker.addSplitShipmentDMSActionLog(splitShipmentList, userId);
                        orderWorker.updateSplitShipmentList(splitShipmentList, userId);
                    }

                // Update the shipping doc status in Shipment table
                if (ShipDocChecked != prevShipDocChecked && shipmentDef != null)    // && splitShipments != null
                {
                    ShipmentDef liveShipment = OrderSelectWorker.Instance.getShipmentByKey(invoiceDef.ShipmentId);
                    ShippingDocWFS DocWFS = ShippingDocWFS.getType(shipmentDef.ShippingDocWFS.Id);
                    ShippingDocWFS prevDocWFS = liveShipment.ShippingDocWFS;

                    // Update info in Shipment table
                    if (DocWFS.Id != prevDocWFS.Id)
                    {
                        string remark = "Shipping Doc. Status : " + (prevDocWFS == null ? "" : prevDocWFS.Name)
                                       + " -> " + (DocWFS == null ? "" : DocWFS.Name);
                        ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                    }
                    liveShipment.ShippingDocWFS = DocWFS;
                    liveShipment.RejectPaymentReasonId = shipmentDef.RejectPaymentReasonId;
                    orderWorker.updateShipmentList(ConvertUtility.createArrayList(liveShipment));
                }

                // Update the shipping doc status in Invoice table
                if (ShipDocChecked != prevShipDocChecked)
                {
                    //if (liveInvoice.ShippingDocCheckedOn != invoiceDef.ShippingDocCheckedOn)
                    {
                        string remark = "Shipping Doc. Checked Date : " + (liveInvoice.ShippingDocCheckedOn == DateTime.MinValue ? "" : liveInvoice.ShippingDocCheckedOn.ToString("dd/MM/yyyy HH:mm:ss"))
                                       + " -> " + (invoiceDef.ShippingDocCheckedOn == DateTime.MinValue ? "" : invoiceDef.ShippingDocCheckedOn.ToString("dd/MM/yyyy HH:mm:ss"));
                        ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                    }
                    //if (liveInvoice.ShippingDocCheckedBy != invoiceDef.ShippingDocCheckedBy)
                    {
                        string remark = "Shipping Doc. Checked By : " + (liveInvoice.ShippingDocCheckedBy == null ? "" : liveInvoice.ShippingDocCheckedBy.DisplayName)
                                       + " -> " + (invoiceDef.ShippingDocCheckedBy == null ? "" : invoiceDef.ShippingDocCheckedBy.DisplayName);
                        ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                    }
                }

                if (isQtyChanged)
                {
                    invoiceDef.IsILSQtyUploadAllowed = false;

                    //update split shipment detail shipped quantity...                    
                    if (splitShipmentList.Count > 0)
                    {
                        ArrayList splitShipmentDetailList;
                        ArrayList updateSplitShipmentDetailList = new ArrayList();
                        foreach (SplitShipmentDef splitShipment in splitShipmentList)
                        {
                            splitShipmentDetailList = (ArrayList)orderSelectWorker.getSplitShipmentDetailBySplitShipmentId(splitShipment.SplitShipmentId);

                            foreach (ShipmentDetailDef shipDetail in shipmentDetails)
                            {
                                foreach (SplitShipmentDetailDef splitShipDetail in splitShipmentDetailList)
                                {
                                    if (shipDetail.SizeOption.SizeOptionId == splitShipDetail.SizeOption.SizeOptionId)
                                    {
                                        if (splitShipDetail.ShippedQuantity == 0 && splitShipDetail.ShippedQuantity != shipDetail.ShippedQuantity)
                                        {
                                            splitShipDetail.ShippedQuantity = shipDetail.ShippedQuantity;
                                            updateSplitShipmentDetailList.Add(splitShipDetail);
                                        }
                                        break;
                                    }
                                }
                            }
                            //this.updateSplitShipment(splitShipment, splitShipmentDetailList, userId);
                            if (updateSplitShipmentDetailList.Count > 0)
                            {
                                orderWorker.updateSplitShipmentDetailList(invoiceDef.ShipmentId, updateSplitShipmentDetailList, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);
                                shippingWorker.updateSplitShipmentSummaryTotal(splitShipment.SplitShipmentId, userId);
                                splitShipment.IsILSQtyUploadAllowed = false;
                                orderWorker.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipment), userId);
                                updateSplitShipmentDetailList.Clear();
                            }
                        }
                        //orderWorker.updateSplitShipmentDetailList(invoiceDef.ShipmentId, updateSplitShipmentDetailList, ActionHistoryType.SHIPPING_UPDATES, new ArrayList(), userId);
                    }
                    shippingWorker.updateShipmentSummaryTotal(invoiceDef.ShipmentId, userId);
                }

                shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);

                if (amendmentList.Count > 0)
                {
                    accountWorker.doReversal(invoiceDef.ShipmentId, amendmentList);

                    foreach (ActionHistoryDef actionHistory in amendmentList)
                    {
                        shippingWorker.updateActionHistory(actionHistory);
                    }
                }

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public void updateSplitShipment(SplitShipmentDef splitShipmentDef, ArrayList splitShipmentDetails, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                bool isQtyChanged = false;
                ArrayList amendmentList = new ArrayList();
                InvoiceDef invoice = shippingWorker.getInvoiceByKey(splitShipmentDef.ShipmentId);
                ShipmentDef shipment = orderSelectWorker.getShipmentByKey(splitShipmentDef.ShipmentId);

                //update shipping receipt doc date in split shipment
                SplitShipmentDef splitShipmentOriginal = orderSelectWorker.getSplitShipmentByKey(splitShipmentDef.SplitShipmentId);
                DateTime latestShippingDocCheckedOn = splitShipmentDef.ShippingDocCheckedOn;
                UserRef latestShippingCheckedBy = splitShipmentDef.ShippingDocCheckedBy;
                //UserRef latesstCheckedBy = null;
                ArrayList splitShipmentList = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(splitShipmentDef.ShipmentId);
                DateTime receiptDate;// = DateTime.MinValue;
                //if (splitShipmentOriginal.ShippingDocReceiptDate != splitShipmentDef.ShippingDocReceiptDate)
                {
                    receiptDate = splitShipmentDef.ShippingDocReceiptDate;
                    foreach (SplitShipmentDef split in splitShipmentList)
                    {
                        if (split.IsVirtualSetSplit == 1)
                            continue;

                        if (split.SplitShipmentId == splitShipmentDef.SplitShipmentId)
                            continue;

                        if (split.ShippingDocCheckedOn > latestShippingDocCheckedOn)
                        {
                            latestShippingDocCheckedOn = split.ShippingDocCheckedOn;
                            latestShippingCheckedBy = split.ShippingDocCheckedBy;
                        }

                        if (split.ShippingDocReceiptDate == DateTime.MinValue)
                        {
                            receiptDate = DateTime.MinValue;
                            //break;  
                        }
                        if (receiptDate != DateTime.MinValue)
                            if (split.ShippingDocReceiptDate > receiptDate)
                            {
                                receiptDate = split.ShippingDocReceiptDate;
                            }
                    }
                    //if (receiptDate != DateTime.MinValue)
                    //{
                    //    //InvoiceDef invoice = shippingWorker.getInvoiceByKey(splitShipmentDef.ShipmentId);
                    //    invoice.ShippingDocReceiptDate = receiptDate;
                    //    //shippingWorker.updateInvoice(invoice, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);
                    //}

                }


                if (splitShipmentDetails != null)
                    orderWorker.updateSplitShipmentDetailList(splitShipmentDef.ShipmentId, splitShipmentDetails, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);

                foreach (ActionHistoryDef actionHistory in amendmentList)
                {
                    shippingWorker.updateActionHistory(actionHistory);
                    if (actionHistory.AmendmentType.Id == AmendmentType.ACTUAL_QUANTITY.Id)
                    {
                        isQtyChanged = true;
                    }
                }

                if (isQtyChanged)
                {
                    splitShipmentDef.IsILSQtyUploadAllowed = false;
                }

                bool isShipDocChecked = (splitShipmentOriginal.ShippingDocCheckedOn == DateTime.MinValue && splitShipmentDef.ShippingDocCheckedOn != DateTime.MinValue);
                bool isShipDocUnChecked = (splitShipmentOriginal.ShippingDocCheckedOn != DateTime.MinValue && splitShipmentDef.ShippingDocCheckedOn == DateTime.MinValue);

                orderWorker.addSplitShipmentDMSActionLog(ConvertUtility.createArrayList(splitShipmentDef), userId);
                orderWorker.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipmentDef), userId);
                shippingWorker.updateSplitShipmentSummaryTotal(splitShipmentDef.SplitShipmentId, userId);

                if (invoice.ShippingDocReceiptDate != receiptDate) //&& receiptDate != DateTime.MinValue
                {
                    string remark = "Shipping Doc. Receipt Date : " + (DateTimeUtility.getDateString(invoice.ShippingDocReceiptDate))
                        + " -> " + (DateTimeUtility.getDateString(receiptDate));
                    ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(splitShipmentDef.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                    invoice.ShippingDocReceiptDate = receiptDate;
                }
                // Update Shipping Doc. info of master shipment record
                if (isShipDocUnChecked || isShipDocChecked)  // && invoice.PiecesPerDeliveryUnit > 0
                // && invoice.ShippingDocReceiptDate != DateTime.MinValue)  ( invoice.ShippingDocReceiptDate has just been updated )
                //|| (prevDocWFS.Id != ShippingDocWFS.NOT_READY.Id && newDocWFS.Id == ShippingDocWFS.NOT_READY.Id)
                {

                    decimal totalSupplierNetAmt = getTheTotalSupplierNetAmtFromAllSplit(splitShipmentDef.ShipmentId);
                    if (isShipDocUnChecked || (isShipDocChecked && totalSupplierNetAmt >= 0 && !string.IsNullOrEmpty(invoice.SupplierInvoiceNo)))
                    {   // Handle the Shipping Doc. Check status and action log
                        string remark;

                        ShippingDocWFS prevDocWFS = shipment.ShippingDocWFS;
                        ShippingDocWFS newDocWFS = splitShipmentDef.ShippingDocWFS;
                        if (newDocWFS.Id != prevDocWFS.Id)
                        {
                            remark = "Shipping Doc. Status : " + (prevDocWFS == null ? "" : prevDocWFS.Name)
                                           + " -> " + (newDocWFS == null ? "" : newDocWFS.Name);
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(splitShipmentDef.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                            shipment.ShippingDocWFS = newDocWFS;
                            orderWorker.updateShipmentList(ConvertUtility.createArrayList(shipment));
                        }

                        DateTime prevCheckDate = invoice.ShippingDocCheckedOn;
                        DateTime newCheckDate = splitShipmentDef.ShippingDocCheckedOn;
                        UserRef prevCheckBy = invoice.ShippingDocCheckedBy;
                        UserRef newCheckBy = (splitShipmentDef.ShippingDocCheckedBy == null ? null : latestShippingCheckedBy);

                        remark = "Shipping Doc. Checked Date : " + (prevCheckDate == DateTime.MinValue ? "" : prevCheckDate.ToString("dd/MM/yyyy HH:mm:ss"))
                            + " -> " + (newCheckDate == DateTime.MinValue ? "" : newCheckDate.ToString("dd/MM/yyyy HH:mm:ss"));
                        ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoice.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));

                        remark = "Shipping Doc. Checked By : " + (prevCheckBy == null ? "" : prevCheckBy.DisplayName)
                                + " -> " + (newCheckBy == null ? "" : newCheckBy.DisplayName);
                        ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(invoice.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));

                        invoice.ShippingDocCheckedOn = newCheckDate; // (newDocWFS.Id == ShippingDocWFS.NOT_READY.Id ? DateTime.MinValue : DateTime.Now);
                        invoice.ShippingDocCheckedBy = newCheckBy; //latestShippingCheckedBy;
                        invoice.ShippingCheckedTotalNetAmount = (isShipDocChecked ? totalSupplierNetAmt : 0);    // (newDocWFS.Id == ShippingDocWFS.NOT_READY.Id ? 0 : totalSupplierNetAmt);
                        //if (prevDocWFS.Id == ShippingDocWFS.REJECTED.Id && splitShipmentDef.ShippingDocWFS.Id == ShippingDocWFS.ACCEPTED.Id)
                        invoice.AccountDocReceiptDate = splitShipmentDef.AccountDocReceiptDate;
                        //if (shipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.OPENACCOUNT.GetHashCode()
                        //    || (shipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode()
                        //        && (splitShipmentDef.IsLCPaymentChecked == 1 || invoice.IsLCPaymentChecked == 1)))
                        //{
                        //    invoice.AccountDocReceiptDate = splitShipmentDef.AccountDocReceiptDate;
                        //}
                    }
                    //shippingWorker.updateInvoice(invoice, ActionHistoryType.SHIPPING_UPDATES, null, userId);
                }
                //else
                //    if (receiptDate != DateTime.MinValue)
                //    {
                //        //InvoiceDef invoice = shippingWorker.getInvoiceByKey(splitShipmentDef.ShipmentId);
                //        invoice.ShippingDocReceiptDate = receiptDate;
                //        shippingWorker.updateInvoice(invoice, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);
                //    }
                shippingWorker.updateInvoice(invoice, ActionHistoryType.SHIPPING_UPDATES, null, userId);


                if (amendmentList.Count > 0)
                {
                    accountWorker.doReversal(splitShipmentDef.ShipmentId, amendmentList);
                }

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        decimal getTheTotalSupplierNetAmtFromAllSplit(int shipmentId)
        {
            // if the shipping document info in all split shipmnet are completed, it return the total supplier net amount for all split shipment. 
            //  otherwise return -1
            bool isReady = true;
            decimal SupTotalAmt, SupNetAmt;
            decimal totalNetAmt = 0;

            ArrayList splitList = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(shipmentId);
            foreach (SplitShipmentDef spt in splitList)
                if (spt.IsVirtualSetSplit == 0)
                {
                    if (string.IsNullOrEmpty(spt.SupplierInvoiceNo) || spt.ShippingDocReceiptDate == DateTime.MinValue
                            || spt.ShippingDocCheckedOn == DateTime.MinValue || spt.ShippingCheckedTotalNetAmount == decimal.MinValue)
                    {
                        isReady = false;
                        break;
                    }
                    SupTotalAmt = spt.TotalShippedSupplierGarmentAmountAfterDiscount;
                    SupNetAmt = (SupTotalAmt
                        - Math.Round(SupTotalAmt * (spt.QACommissionPercent / 100), 2, MidpointRounding.AwayFromZero)
                        - Math.Round(SupTotalAmt * (spt.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero));
                    totalNetAmt += SupNetAmt;
                }
            return (isReady ? totalNetAmt : -1);
        }

        public ArrayList getILSManifestDetailShipmentList(string containerNo)
        {
            return shippingWorker.getILSManifestDetailShipment(containerNo);
        }

        public int prepareToSendEzibuyInvoice(ArrayList shipmentIdList, int userId)
        {
            int sentCount = 0;
            string remark = "";

            foreach (int shipmentId in shipmentIdList)
            {
                TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
                try
                {
                    ctx.Enter();
                    TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                    ArrayList amendmentList = new ArrayList();
                    InvoiceDef invoiceDef = null;
                    ActionHistoryDef actionHistory = null;

                    invoiceDef = shippingWorker.getInvoiceByKey(shipmentId);


                    if (invoiceDef.IsReadyToSendInvoice == false)
                    {
                        invoiceDef.IsReadyToSendInvoice = true;
                        if (invoiceDef.InvoiceSentDate == DateTime.MinValue)
                            remark = "Invoice is ready to be sent";
                        else
                            remark = "Invoice is ready to be re-sent";
                    }
                    else
                        continue;

                    shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);

                    sentCount += 1;

                    actionHistory = new ActionHistoryDef();
                    actionHistory.ActionDate = DateTime.Now;
                    actionHistory.ActionHistoryType = ActionHistoryType.MARK_READY_TO_SEND_INVOICE;
                    actionHistory.ShipmentId = shipmentId;
                    actionHistory.SplitShipmentId = 0;
                    actionHistory.Status = 1;
                    actionHistory.Remark = remark;
                    actionHistory.User = generalWorker.getUserByKey(userId);

                    shippingWorker.updateActionHistory(actionHistory);

                    ctx.VoteCommit();

                }
                catch (Exception e)
                {
                    ctx.VoteRollback();
                    throw e;
                }
                finally
                {
                    ctx.Exit();
                }
            }
            return sentCount;
        }

        public bool printInvoice(ArrayList shipmentIdList, int userId)
        {
            bool isSuccess = true;

            foreach (int shipmentId in shipmentIdList)
            {
                TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
                try
                {
                    ctx.Enter();
                    TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                    ArrayList amendmentList = new ArrayList();
                    InvoiceDef invoiceDef = null;
                    ActionHistoryDef actionHistory = null;

                    invoiceDef = shippingWorker.getInvoiceByKey(shipmentId);
                    invoiceDef.InvoicePrintDate = DateTime.Now;
                    invoiceDef.InvoicePrintUser = generalWorker.getUserByKey(userId);
                    shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);

                    actionHistory = new ActionHistoryDef();
                    actionHistory.ActionDate = DateTime.Now;
                    actionHistory.ActionHistoryType = ActionHistoryType.DOCUMENT_PRINTOUT;
                    actionHistory.ShipmentId = shipmentId;
                    actionHistory.SplitShipmentId = 0;
                    actionHistory.Status = 1;
                    actionHistory.Remark = "Invoice : " + ShippingWorker.getInvoiceNo(invoiceDef.InvoicePrefix, invoiceDef.InvoiceSeqNo, invoiceDef.InvoiceYear);
                    actionHistory.User = generalWorker.getUserByKey(userId);

                    shippingWorker.updateActionHistory(actionHistory);

                    ctx.VoteCommit();
                }
                catch (Exception e)
                {
                    ctx.VoteRollback();
                    isSuccess = false;
                    throw e;
                }
                finally
                {
                    ctx.Exit();
                }
            }
            return isSuccess;
        }

        public string[] getReplacementInvoiceNo(string invoiceNo)
        {
            string s = String.Empty;
            string contractNo = string.Empty;
            string[] msg = new string[2];

            ILSInvoiceDef ilsInvoiceDef = ILSUploadWorker.Instance.getILSInvoiceByInvoiceNo(invoiceNo);
            if (ilsInvoiceDef == null)
                ilsInvoiceDef = ILSUploadWorker.Instance.getILSReplacementInvoice(invoiceNo);

            ILSOrderRefDef ilsOrderRefDef = null;
            InvoiceDef invoiceDef = null;

            if (ilsInvoiceDef != null)
            {
                ilsOrderRefDef = ILSUploadWorker.Instance.getILSOrderRefByKey(ilsInvoiceDef.OrderRefId);
                invoiceDef = ShippingWorker.Instance.getInvoiceByKey(ilsOrderRefDef.ShipmentId);
                ShipmentDef shipment = orderSelectWorker.getShipmentByKey(ilsOrderRefDef.ShipmentId);
                ContractDef contract = orderSelectWorker.getContractByKey(shipment.ContractId);
                contractNo = contract.ContractNo + "-" + shipment.DeliveryNo.ToString();
                if (invoiceDef != null)
                {
                    s = invoiceDef.InvoiceNo;
                }

                if (s == String.Empty)
                {
                   ILSInvoiceUploadStatus status = ILSInvoiceUploadStatus.getType(ilsInvoiceDef.Status);
                    if (status != null)
                        s = status.AlertText;
                    else
                    {   // Undefine status 
                        if (shipment.WorkflowStatus.Id == ContractWFS.CANCELLED.Id)
                            s = "This shipment is stated in CANCELLED status. Please contact IT to clarify.";
                    }
                }
            }

            msg[0] = (s == String.Empty ? "N/A" : s);
            msg[1] = (contractNo == string.Empty ? "N/A" : contractNo);
            return msg;
        }

        public void updateProduct(int shipmentId, ProductDef product, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ProductWorker.Instance.updateProductDef(product, userId);
                ActionHistoryDef action = new ActionHistoryDef();
                action.ActionHistoryType = ActionHistoryType.SHIPPING_UPDATES;
                action.AmendmentType = AmendmentType.ITEM;
                action.ActionDate = DateTime.Now;
                action.ShipmentId = shipmentId;
                action.Status = 1;
                action.User = generalWorker.getUserByKey(userId);
                action.Remark = "Set new Master Item description|ProductId/ItemNo=" + product.ProductId.ToString() + "/" + product.ItemNo
                    + (string.IsNullOrEmpty(product.MasterDescription1) ? "" : "|" + product.MasterDescription1)
                    + (string.IsNullOrEmpty(product.MasterDescription2) ? "" : "|" + product.MasterDescription2)
                    + (string.IsNullOrEmpty(product.MasterDescription3) ? "" : "|" + product.MasterDescription3)
                    + (string.IsNullOrEmpty(product.MasterDescription4) ? "" : "|" + product.MasterDescription4)
                    + (string.IsNullOrEmpty(product.MasterDescription5) ? "" : "|" + product.MasterDescription5)
                    + (string.IsNullOrEmpty(product.RetailDescription) ? "" : "|" + product.RetailDescription);

                shippingWorker.updateActionHistory(action);

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public ArrayList getShipmentToDMSList(int officeId, DateTime docDateFrom,
            DateTime docDateTo, int paymentTermId, int vendorId, int checkStatus, string contractNo, string itemNo)
        {
            return shippingWorker.getShipmentToDMSList(officeId, docDateFrom, docDateTo,
                paymentTermId, vendorId, checkStatus, contractNo, itemNo);
        }


        public void updateShippingDocumentCheckStatus(ArrayList list, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                foreach (string idString in list)
                {
                    int shipmentId = int.Parse(idString);
                    ArrayList amendmentList = new ArrayList();
                    InvoiceDef invoiceDef = shippingWorker.getInvoiceByKey(shipmentId);
                    ShipmentDef def = orderSelectWorker.getShipmentByKey(shipmentId);

                    string remark = "Shipping Doc. Checked Date : " + (invoiceDef.ShippingDocCheckedOn == DateTime.MinValue ? String.Empty : invoiceDef.ShippingDocCheckedOn.ToString("dd/MM/yyyy HH:mm:ss"))
                                   + " -> " + (DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    shippingWorker.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));

                    remark = "Shipping Doc. Checked By : " + (invoiceDef.ShippingDocCheckedBy == null ? String.Empty : invoiceDef.ShippingDocCheckedBy.DisplayName)
                                       + " -> " + (generalWorker.getUserByKey(userId).DisplayName);
                    shippingWorker.updateActionHistory(new ActionHistoryDef(invoiceDef.ShipmentId, 0, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));

                    invoiceDef.ShippingDocCheckedBy = generalWorker.getUserByKey(userId);
                    invoiceDef.ShippingDocCheckedOn = DateTime.Now;
                    invoiceDef.ShippingCheckedTotalNetAmount = def.TotalShippedSupplierGarmentAmountAfterDiscount
                                                                - Math.Round(def.TotalShippedSupplierGarmentAmountAfterDiscount * def.QACommissionPercent / 100, 2, MidpointRounding.AwayFromZero)
                                                                - Math.Round(def.TotalShippedSupplierGarmentAmountAfterDiscount * def.VendorPaymentDiscountPercent / 100, 2, MidpointRounding.AwayFromZero)
                                                                - Math.Round(def.TotalShippedQuantity * def.LabTestIncome, 2, MidpointRounding.AwayFromZero);
                    shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.SHIPPING_UPDATES, amendmentList, userId);
                }

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public ArrayList getActionHistoryByShipmentIdAndType(int shipmentId, int actionHistoryTypeId)
        {
            return shippingWorker.getActionHistoryByShipmentIdAndType(shipmentId, actionHistoryTypeId);
        }

        public DataSet getNSSDiscrepancyDataSet(DateTime startDate)
        {
            TransactionContext ctx = ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
                
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                DataSet ds =  shippingWorker.getNSSDiscrepancyDataSet(startDate);
                ctx.VoteCommit();
                return ds;
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }

        }

        public bool isViaCambodiaQCC(int shipmentId)
        {
            ShipmentDef shipment = OrderSelectWorker.Instance.getShipmentByKey(shipmentId);
            InvoiceDef invoice = shippingWorker.getInvoiceByKey(shipmentId);
            return isViaCambodiaQCC(shipment, invoice);
        }

        public bool isViaCambodiaQCC(ShipmentDef shipment, InvoiceDef invoice)
        {
            bool isCalling = false;
            try
            {

                DateTime awhDate = (invoice.InvoiceDate != DateTime.MinValue ? invoice.InvoiceDate
                        : (invoice.ActualAtWarehouseDate != DateTime.MinValue ? invoice.ActualAtWarehouseDate
                        : (invoice.ILSActualAtWarehouseDate != DateTime.MinValue ? invoice.ILSActualAtWarehouseDate
                        : shipment.SupplierAgreedAtWarehouseDate)));
                isCalling = true;
                return isViaCambodiaQCC(shipment.Vendor.VendorId, awhDate, shipment.ShipmentMethod.ShipmentMethodId);
            }
            catch (Exception e)
            {
                MailHelper.sendGeneralMessage("Error from Cambodia QCC : " + shipment.ShipmentId.ToString() + ", isCalling : " + isCalling.ToString() + ", Vendor : " + shipment.Vendor == null ? "NULL" : shipment.Vendor.Name);
                throw e;
            }
        }

        public bool isViaCambodiaQCC(int vendorId, DateTime d, int ShipmentMethodId)
        {
            bool result = false;

            if (ShipmentMethodId == ShipmentMethodRef.Method.SEA.GetHashCode())
            {
                if (d >= new DateTime(2015, 10, 26))
                {
                    if (vendorId == 4972)
                        result = true;
                }
                if (d >= new DateTime(2015, 11, 1))
                {
                    if (vendorId == 10206 || vendorId == 7928 || vendorId == 7937 || vendorId == 8812 || vendorId == 10469)
                        result = true;
                }

                if (d >= new DateTime(2018, 10, 26))
                {
                    if (vendorId == 7469 || vendorId == 6969 || vendorId == 9693 || vendorId == 10135 || vendorId == 6682 || vendorId == 3904)
                        result = true;
                }

                if (d >= new DateTime(2020, 10, 26))
                {
                    if (vendorId == 7606 || vendorId == 162 || vendorId == 5338 || vendorId == 7712 || vendorId == 10111 || vendorId == 10371 || vendorId == 10257 || vendorId == 10405 || vendorId == 9262)
                        result = true;
                }
            }

            return result;
        }

        public decimal getFutureOrderAmtByVendorId(int vendorId)
        {
            return shippingWorker.getFutureOrderAmtByVendorId(vendorId);
        }

        public int getFutureOrderCountByVendorId(int vendorId, int officeId)
        {
            return shippingWorker.getFutureOrderCountByVendorId(vendorId, officeId);
        }

        public string getVendorNSLDocumentCount(int vendorId)
        {
            return shippingWorker.getVendorNSLDocumentCount(vendorId);
        }

        public string getOSAdvancePaymentInstalmentAmt(int vendorId)
        {
            return shippingWorker.getOSAdvancePaymentInstalmentAmt(vendorId);
        }

        public string getOSNextClaimAmtByVendorId(int vendorId, int officeId)
        {
            return shippingWorker.getOSNextClaimAmtByVendorId(vendorId, officeId);
        }

        public decimal getOutstandingPaymentAmtByVendorId(int vendorId)
        {
            return shippingWorker.getOutstandingPaymentAmtByVendorId(vendorId);
        }

        public void SendInvoiceWithBeneficiaryCert(int shipmentId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(shipmentId);

                if (invoiceDef == null || invoiceDef.InvoiceDate == DateTime.MinValue || invoiceDef.InvoiceSentDate != DateTime.MinValue)
                    return;

                //Check if the invoice is new one to avoid sending the invoice that issued before this function is added.
                DateTime invoiceIssueDate = DateTime.MinValue;
                ArrayList list = shippingWorker.getActionHistoryByShipmentIdAndType(shipmentId, ActionHistoryType.SHIPPING_UPDATES.Id);
                foreach (ActionHistoryDef def in list)
                {
                    if (def.Remark.Contains("Issued Invoice :"))
                    {
                        invoiceIssueDate = def.ActionDate;
                        break;
                    }
                }
                if (invoiceIssueDate == DateTime.MinValue || invoiceIssueDate < DateTime.Now.AddHours(-1))
                    return;

                ContractDef contractDef = OrderSelectWorker.Instance.getContractByShipmentId(invoiceDef.ShipmentId);

                if (contractDef.Customer.CustomerId != CustomerDef.Id.smithbrooks.GetHashCode() 
                    || contractDef.Office.OfficeId != OfficeId.SL.Id)
                    return;

                ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(invoiceDef.ShipmentId);

                ArrayList fileList = new ArrayList();
                string outputFolder = WebConfig.getValue("appSettings", "INVOICEWITHBC_OUTPUT_FOLDER");

                var invoicerpt = reporter.invoice.InvoiceReportManager.Instance.getInvoiceReport(ConvertUtility.createArrayList(invoiceDef.ShipmentId), userId);
                string invoiceFileName = outputFolder + "NSL Sales Invoice_" + contractDef.ContractNo + "-" + shipmentDef.DeliveryNo + ".pdf";

                invoicerpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, invoiceFileName);
                invoicerpt.Dispose();
                fileList.Add(invoiceFileName);

                var rpt = reporter.invoice.BeneficiaryCert.BeneficiaryCertManager.Instance.GetBeneficiaryCertReport(invoiceDef, shipmentDef, contractDef);
                string bcfileName = outputFolder + "NSL Beneficiary Certificate_" + contractDef.ContractNo + "-" + shipmentDef.DeliveryNo + ".pdf";

                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, bcfileName);
                rpt.Dispose();

                fileList.Add(bcfileName);

                ArrayList queryStructs = new ArrayList();
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Shipping - NS Invoice"));
                queryStructs.Add(new QueryStructDef("NS Invoice no.", invoiceDef.InvoiceNo));
                queryStructs.Add(new QueryStructDef("Customer Name", "S&B"));

                string strReturn = DMSUtil.CreateDocument(0, "\\Hong Kong\\Shipping\\NS Invoice\\", invoiceDef.InvoiceNo, "Shipping - NS Invoice", queryStructs, fileList);
                string toAddress = "";
                if (contractDef.Customer.CustomerId == CustomerDef.Id.smithbrooks.GetHashCode())
                {
                    toAddress = "Victoria.Hutton@sportsdirect.com";
                }
                NoticeHelper.sendInvoiceWithBCEmail(fileList, toAddress, contractDef.ContractNo + "-" + shipmentDef.DeliveryNo, invoiceDef.InvoiceNo);

                invoiceDef.InvoiceSentDate = DateTime.Now;
                shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.INVOICE_SENT, null, userId);

                ActionHistoryDef actionHistory = new ActionHistoryDef();
                actionHistory.ShipmentId = invoiceDef.ShipmentId;
                actionHistory.ActionDate = DateTime.Now;
                actionHistory.ActionHistoryType = ActionHistoryType.INVOICE_SENT;
                actionHistory.SplitShipmentId = 0;
                actionHistory.Remark = "Invoice with BeneficiaryCert Sent To Cust : " + invoiceDef.InvoiceNo;
                actionHistory.Status = 1;
                actionHistory.User = generalWorker.getUserByKey(userId);
                ShippingWorker.Instance.updateActionHistory(actionHistory);
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }
    }
}
