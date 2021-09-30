using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.isam.domain.order;
using com.next.isam.domain.account;
using com.next.isam.domain.common;
using com.next.isam.domain.claim;
using com.next.isam.domain.types;
using com.next.isam.domain.nontrade;
using com.next.isam.dataserver.worker;
using com.next.isam.reporter.dataserver;
using com.next.common.domain.general;
using com.next.common.web.commander;
using com.next.infra.util;
using com.next.isam.dataserver.model.account;
using System.Web.UI.WebControls;
using com.next.common.domain.industry.vendor;
using System.Globalization;

namespace com.next.isam.reporter.accounts
{
    public class AccountReportManager
    {
        private static AccountReportManager _instance;
        private CommonWorker commonWorker;
        private AccountWorker accountWorker;
        private NonTradeWorker nontradeWorker;
        private GeneralWorker generalWorker;

        public AccountReportManager()
		{
            commonWorker = CommonWorker.Instance;
            accountWorker = AccountWorker.Instance;
            nontradeWorker = NonTradeWorker.Instance;
            generalWorker = GeneralWorker.Instance;

		}

        public static AccountReportManager Instance
		{
			get 
			{
				if (_instance == null)
				{
                    _instance = new AccountReportManager();
				}
				return _instance;
			}
		}

        public FutureOrderSummaryBySupplierReportDs getFutureOrderSummaryBySupplierReport(int officeId, int vendorId, DateTime dateFrom, DateTime dateTo)
        {
            return ReporterWorker.Instance.getFutureOrderSummaryBySupplierReport(officeId, vendorId, dateFrom, dateTo);
        }

        public ReportClass getShipmentAndCommissionReport(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, DateTime purchaseExtractDateFrom, DateTime purchaseExtractDateTo, int budgetYear,
            int periodFrom, int isActual, int isRealized, int isAccrual, int vendorId, int baseCurrencyId, int countryOfOriginId, int officeId, int handlingOfficeId, TypeCollector officeIdList, int seasonId,
            TypeCollector productTeamList, TypeCollector customerIdList, TypeCollector tradingAgencyList, int departmentId, TypeCollector departmentIdList, int termOfPurchaseId, int isSZOrder, int isUTOrder,
            int isOPROrder, int isLDPOrder, int isNSLTailoring, int isSampleOrder, TypeCollector designSourceList, string MockShopDebitNoteNoFrom, string MockShopDebitNoteNoTo, string supplierInvoiceNo,
            int reportType, string sortField, int userId)
        {   // Shipment and commission statement for CHOICE
            return getShipmentAndCommissionReport(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, invoiceDateFrom,
            invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo, purchaseExtractDateFrom, purchaseExtractDateTo, budgetYear,
            periodFrom, isActual, isRealized, isAccrual, vendorId, baseCurrencyId, countryOfOriginId, officeId, handlingOfficeId, officeIdList, seasonId,
            productTeamList, customerIdList, tradingAgencyList, departmentId, departmentIdList, termOfPurchaseId, isSZOrder, isUTOrder,
            isOPROrder, isLDPOrder, isNSLTailoring, isSampleOrder, designSourceList, MockShopDebitNoteNoFrom, MockShopDebitNoteNoTo, supplierInvoiceNo,
            reportType, 0, sortField, userId, "REPORT");

        }

        public ReportClass getShipmentAndCommissionReport(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, DateTime purchaseExtractDateFrom, DateTime purchaseExtractDateTo, int budgetYear,
            int periodFrom, int isActual, int isRealized, int isAccrual, int vendorId, int baseCurrencyId, int countryOfOriginId, int officeId, int handlingOfficeId, TypeCollector officeIdList, int seasonId,
            TypeCollector productTeamList, TypeCollector customerIdList, TypeCollector tradingAgencyList, int departmentId, TypeCollector departmentIdList, int termOfPurchaseId, int isSZOrder, int isUTOrder,
            int isOPROrder, int isLDPOrder, int isNSLTailoring, int isSampleOrder, TypeCollector designSourceList, string DCNoteNoFrom, string DCNoteNoTo, string supplierInvoiceNo,
            int reportType, string sortField, int userId, string outputType) //report type --- 0 normal, 1 mock shop/studio sample, 2 choice; 
        {   // Shipment and commission statement for Mockshop and Studio Sample
            return getShipmentAndCommissionReport(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, invoiceDateFrom,
                        invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo, purchaseExtractDateFrom, purchaseExtractDateTo, budgetYear,
                        periodFrom, isActual, isRealized, isAccrual, vendorId, baseCurrencyId, countryOfOriginId, officeId, handlingOfficeId, officeIdList, seasonId,
                        productTeamList, customerIdList, tradingAgencyList, departmentId, departmentIdList, termOfPurchaseId, isSZOrder, isUTOrder,
                        isOPROrder, isLDPOrder, isNSLTailoring, isSampleOrder, designSourceList, DCNoteNoFrom, DCNoteNoTo, supplierInvoiceNo,
                        reportType, 0, sortField, userId, "REPORT");
        }

        public ReportClass getShipmentAndCommissionReport(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, DateTime purchaseExtractDateFrom, DateTime purchaseExtractDateTo, int budgetYear,
            int periodFrom, int isActual, int isRealized, int isAccrual, int vendorId, int baseCurrencyId, int countryOfOriginId, int officeId, int handlingOfficeId, TypeCollector officeIdList, int seasonId,
            TypeCollector productTeamList, TypeCollector customerIdList, TypeCollector tradingAgencyList, int departmentId, TypeCollector departmentIdList, int termOfPurchaseId, int isSZOrder, int isUTOrder,
            int isOPROrder, int isLDPOrder, int isNSLTailoring, int isSampleOrder, TypeCollector designSourceList, string DCNoteNoFrom, string DCNoteNoTo, string supplierInvoiceNo,
            int reportType, int reportVersion, string sortField, int userId, string outputType) //report type --- 0 normal, 1 mock shop/studio sample, 2 choice;   report version -- 0 Sun, 1 Epicor
        {

            ReportClass report;
            ShipmentCommissionReportDs reportDs;
            decimal accrualSalesAmt = 0;
            decimal accrualSalesComm = 0;
            /*
            if (reportType == 2)
            {
                reportDs = ReporterWorker.Instance.getShipmentCommissionChoiceReport(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear,
                    invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo, purchaseExtractDateFrom, purchaseExtractDateTo,
                    budgetYear, periodFrom, isActual, isRealized, vendorId, baseCurrencyId, countryOfOriginId, officeId, officeIdList, seasonId,
                    productTeamList, tradingAgencyList, departmentId, departmentIdList, termOfPurchaseId, sortField);

                report = new ShipmentCommissionChoiceReport();
            }
            else
            */
            {
                reportDs = ReporterWorker.Instance.getShipmentCommissionReport(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, invoiceDateFrom,
                        invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo, purchaseExtractDateFrom, purchaseExtractDateTo, budgetYear, periodFrom, isActual, isRealized, isAccrual,
                        vendorId, baseCurrencyId, countryOfOriginId, officeId, handlingOfficeId, officeIdList, seasonId, productTeamList,
                        customerIdList, tradingAgencyList, departmentId, departmentIdList, termOfPurchaseId, isSZOrder, isUTOrder, isOPROrder, isLDPOrder, isNSLTailoring, isSampleOrder,
                        designSourceList, DCNoteNoFrom, DCNoteNoTo, supplierInvoiceNo, reportType == 1, reportVersion, sortField);

                if (reportType == 1)
                {
                    if (isSampleOrder == 2)
                        report = new ShipmentCommissionMockShopReport();
                    else //studio sample
                        report = new ShipmentCommissionStudioSampleReport();
                }
                else
                    report = new ShipmentCommissionReport();
            }

            /*
            if (budgetYear != -1)
            {
                if (isAccrual == 1)
                {
                    ShipmentCommissionReportDs ds = ReporterWorker.Instance.getAccrualShipmentCommission(budgetYear, periodFrom, vendorId, baseCurrencyId, countryOfOriginId, officeId, handlingOfficeId,
                        officeIdList, seasonId, productTeamList, customerIdList, tradingAgencyList, departmentId, departmentIdList, termOfPurchaseId, isSZOrder, isUTOrder, isOPROrder,
                        isLDPOrder, isNSLTailoring, isSampleOrder, designSourceList, DCNoteNoFrom, DCNoteNoTo, supplierInvoiceNo, sortField);
                    reportDs.ShipmentCommissionReport.Merge(ds.ShipmentCommissionReport);
                }
                ReporterWorker.Instance.getAccrualSalesSummary(budgetYear, periodFrom - 1, baseCurrencyId, officeId == -1 ? officeIdList : TypeCollector.createNew(officeId), out accrualSalesAmt, out accrualSalesComm);
            }
            */
            report.SetDataSource(reportDs);

            // Setting report parameters
            if (reportType == 1)
            {
                report.SetParameterValue("DCNoteNoFrom", DCNoteNoFrom == string.Empty ? "" : DCNoteNoFrom);
                report.SetParameterValue("DCNoteNoTo", DCNoteNoTo == string.Empty ? "" : DCNoteNoTo);
            }

            report.SetParameterValue("previousPeriodAccrualSalesAmt", accrualSalesAmt);
            report.SetParameterValue("previousPeriodAccrualSalesComm", accrualSalesComm);

            report.SetParameterValue("InvoiceNoFrom", invoicePrefix == "" ? "" : ShippingWorker.getInvoiceNo(invoicePrefix, invoiceSeqFrom, invoiceYear));
            report.SetParameterValue("InvoiceNoTo", invoicePrefix == "" ? "" : ShippingWorker.getInvoiceNo(invoicePrefix, invoiceSeqTo, invoiceYear));

            report.SetParameterValue("InvoiceDateFrom", DateTimeUtility.getDateString(invoiceDateFrom));
            report.SetParameterValue("InvoiceDateTo", DateTimeUtility.getDateString(invoiceDateTo));
            report.SetParameterValue("InvoiceUploadDateFrom", DateTimeUtility.getDateString(invoiceUploadDateFrom));
            report.SetParameterValue("InvoiceUploadDateTo", DateTimeUtility.getDateString(invoiceUploadDateTo));
            report.SetParameterValue("purchaseExtractDateFrom", DateTimeUtility.getDateString(purchaseExtractDateFrom));
            report.SetParameterValue("purchaseExtractDateTo", DateTimeUtility.getDateString(purchaseExtractDateTo));
            report.SetParameterValue("Office", officeId == -1 ? "" : GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode);
            string office = string.Empty;
            foreach (object id in officeIdList.Values)
                office += (office == string.Empty ? "" : ", ") + GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).Description.Replace("Office", string.Empty).TrimEnd();
            report.SetParameterValue("Office", office);
            if (reportType != 1 && reportType != 2)
                report.SetParameterValue("HandlingOffice", (handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).Description));

            report.SetParameterValue("BaseCurrency", GeneralWorker.Instance.getCurrencyByKey(baseCurrencyId).CurrencyCode);

            report.SetParameterValue("Supplier", vendorId == -1 ? "" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
            report.SetParameterValue("Department", departmentId == -1 ? "" : GeneralWorker.Instance.getProductDepartmentByKey(departmentId).Description);

            if (reportDs.ShipmentCommissionReport.Rows.Count != 0)
            {
                report.SetParameterValue("Season", seasonId == -1 ? "" : reportDs.ShipmentCommissionReport.Rows[0]["SeasonCode"].ToString());
                report.SetParameterValue("CountryOfOrigin", countryOfOriginId == -1 ? "" : reportDs.ShipmentCommissionReport.Rows[0]["CountryOfOrigin"].ToString());
            }
            else
            {
                report.SetParameterValue("Season", seasonId == -1 ? "" : GeneralWorker.Instance.getSeasonByKey(seasonId).Code);
                report.SetParameterValue("CountryOfOrigin", countryOfOriginId == -1 ? "" : GeneralWorker.Instance.getCountryOfOriginByKey(countryOfOriginId).Name);
            }

            if (productTeamList.Values.Count == 1)
            {
                foreach (object productTeamId in productTeamList.Values)
                {
                    ProductCodeDef pc = GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32(productTeamId));
                    report.SetParameterValue("ProductTeam", pc.CodeDescription);
                }
            }
            else
                report.SetParameterValue("ProductTeam", "");

            report.SetParameterValue("BudgetYear", budgetYear);
            report.SetParameterValue("PeriodFrom", periodFrom);
            report.SetParameterValue("IsActual", isActual);
            report.SetParameterValue("IsRealized", isRealized);
            report.SetParameterValue("IsAccrual", isAccrual);

            if (designSourceList.Values.Count == 5)
                report.SetParameterValue("DesignSource", "");
            else
            {
                string source = "";
                foreach (object obj in designSourceList.Values)
                {
                    source += GeneralWorker.Instance.getDesignSourceByKey(Convert.ToInt32(obj)).DesignSourceCode + ", ";
                }
                report.SetParameterValue("DesignSource", source);
            }

            if (customerIdList.Values.Count == 9)
                report.SetParameterValue("Customer", "");
            else
            {
                string cust = "";
                foreach (object obj in customerIdList.Values)
                {
                    cust += commonWorker.getCustomerByKey(Convert.ToInt32(obj)).CustomerCode + ", ";
                }
                cust = cust.Remove(cust.LastIndexOf(", "));
                report.SetParameterValue("Customer", cust);
            }
            if (tradingAgencyList.Values.Count == 4)
                report.SetParameterValue("TradingAgency", "");
            else
            {
                string tradingAgency = "";
                foreach (object obj in tradingAgencyList.Values)
                {
                    tradingAgency += commonWorker.getTradingAgencyByKey(Convert.ToInt32(obj)).ShortName + ", ";
                }
                tradingAgency = tradingAgency.Remove(tradingAgency.LastIndexOf(", "));
                report.SetParameterValue("TradingAgency", tradingAgency);
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
            if (isOPROrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isOPROrder == 1 ? "OPR Order" : " Non-OPR Order";
            }
            if (isLDPOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isLDPOrder == 1 ? "LDP Order" : " Non-LDP Order";
            }
            if (isNSLTailoring != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isNSLTailoring == 1 ? "NSL Tailoring" : " Non-NSL Tailoring";
            }
            if (isSampleOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                switch (isSampleOrder)
                {
                    case 0:
                        orderType += "Mainline Order";
                        break;
                    case 1:
                        orderType += "Mock Shop/Press/Studio Sample Order";
                        break;
                    case 2:
                        orderType += "Mock Shop Sample Order";
                        break;
                    case 3:
                        orderType += "Press Sample Order";
                        break;
                    case 4:
                        orderType += "Mainline And Mock Shop Sample Order";
                        break;
                    case 5:
                        orderType += "Mainline And Press Sample Order";
                        break;
                    case 6:
                        orderType += "Studio Sample Order";
                        break;
                }
            }
            report.SetParameterValue("OrderType", orderType);

            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            report.SetParameterValue("OutputType", outputType);

            return report;
        }

        public ActualSalesSummaryReport getActualSalesSummaryReport(int baseCurrencyId, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime invoiceUploadDateFrom,
            DateTime invoiceUploadDateTo, int fiscalYear, int periodFrom, int periodTo, int isActual, int isRealized, int officeId, TypeCollector officeIdList,
            int departmentId, TypeCollector productTeamList, int seasonId, int countryOfOriginId, int shipmentPortId, TypeCollector customerIdList,
            TypeCollector tradingAgencyList, int termOfPurchaseId, int isSZOrder, int isUTOrder, int isOPROrder, int isLDPOrder, int userId)
        {
            ActualSalesSummaryReport report = new ActualSalesSummaryReport();
            ActualSalesSummaryDs reportDs;



            reportDs = ReporterWorker.Instance.getActualSalesSummaryReport(baseCurrencyId, invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom,
             invoiceUploadDateTo, fiscalYear, periodFrom, periodTo, isActual, isRealized, officeId, officeIdList, departmentId, productTeamList, seasonId,
             countryOfOriginId, shipmentPortId, customerIdList, tradingAgencyList, termOfPurchaseId, isSZOrder, isUTOrder, isOPROrder, isLDPOrder);

            report.SetDataSource(reportDs);

            report.SetParameterValue("InvoiceDateFrom", DateTimeUtility.getDateString(invoiceDateFrom));
            report.SetParameterValue("InvoiceDateTo", DateTimeUtility.getDateString(invoiceDateTo));
            report.SetParameterValue("InvoiceUploadDateFrom", DateTimeUtility.getDateString(invoiceUploadDateFrom));
            report.SetParameterValue("InvoiceUploadDateTo", DateTimeUtility.getDateString(invoiceUploadDateTo));
            report.SetParameterValue("Office", officeId == -1 ? "" : GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode);

            report.SetParameterValue("BaseCurrency", GeneralWorker.Instance.getCurrencyByKey(baseCurrencyId).CurrencyCode);
            report.SetParameterValue("CountryOfOrigin", countryOfOriginId == -1 ? "" : GeneralWorker.Instance.getCountryOfOriginByKey(countryOfOriginId).Name);
            report.SetParameterValue("Season", seasonId == -1 ? "" : GeneralWorker.Instance.getSeasonByKey(seasonId).Code);
            report.SetParameterValue("Department", departmentId == -1 ? "" : GeneralWorker.Instance.getProductDepartmentByKey(departmentId).Description);
            report.SetParameterValue("ShipmentPort", shipmentPortId == -1 ? "" : commonWorker.getShipmentPortByKey(shipmentPortId).ShipmentPortDescription);
            report.SetParameterValue("TermOfPurchase", termOfPurchaseId == -1 ? "" : commonWorker.getTermOfPurchaseByKey(termOfPurchaseId).TermOfPurchaseDescription);
            report.SetParameterValue("Actual", isActual);
            report.SetParameterValue("Realized", isRealized);
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
            if (isOPROrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isOPROrder == 1 ? "OPR Order" : " Non-OPR Order";
            }
            if (isLDPOrder != -1)
            {
                if (orderType != "")
                    orderType += ", ";
                orderType += isLDPOrder == 1 ? "LDP Order" : " Non-LDP Order";
            }
            report.SetParameterValue("OrderType", orderType);

            if (productTeamList.Values.Count == 1)
            {
                foreach (object productTeamId in productTeamList.Values)
                {
                    ProductCodeDef pc = GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32(productTeamId));
                    report.SetParameterValue("ProductTeam", pc.CodeDescription);
                }
            }
            else
                report.SetParameterValue("ProductTeam", "");



            report.SetParameterValue("BudgetYear", fiscalYear);
            report.SetParameterValue("PeriodFrom", periodFrom);
            report.SetParameterValue("PeriodTo", periodTo);

            if (customerIdList.Values.Count == 9)
                report.SetParameterValue("Customer", "");
            else
            {
                string cust = "";
                foreach (object obj in customerIdList.Values)
                {
                    cust += commonWorker.getCustomerByKey(Convert.ToInt32(obj)).CustomerCode + ", ";
                }
                cust = cust.Remove(cust.LastIndexOf(", "));
                report.SetParameterValue("Customer", cust);
            }
            if (tradingAgencyList.Values.Count == 4)
                report.SetParameterValue("TradingAgency", "");
            else
            {
                string tradingAgency = "";
                foreach (object obj in tradingAgencyList.Values)
                {
                    tradingAgency += commonWorker.getTradingAgencyByKey(Convert.ToInt32(obj)).ShortName + ", ";
                }
                tradingAgency = tradingAgency.Remove(tradingAgency.LastIndexOf(", "));
                report.SetParameterValue("TradingAgency", tradingAgency);
            }

            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return report;
        }


        public ReceivablePayableForecastRpt getReceivablePayableForecastReport(DateTime reportDate, ArrayList officeIdList, ArrayList officeDescList,
                ArrayList paymentTermIdList, ArrayList paymentTermDescList, int userId, string ExportType)
        {
            int i;
            string officeDesc;
            string paymentTermDesc;

            ReceivablePayableForecastRpt report = new ReceivablePayableForecastRpt();
            ReceivablePayableForecastDs reportDs;
            
            reportDs = ReporterWorker.Instance.getReceivablePayableForecastReport(reportDate, officeIdList, paymentTermIdList);
            //reportDs = new ReceivablePayableForecastDs();
            report.SetDataSource(reportDs.Tables[reportDs.Tables.IndexOf("AccountReport")]);
            //report.SetParameterValue("SelectedReportDate",Convert.ToString(reportDate,"dd/MM/yyyy"));
            report.SetParameterValue("SelectedReportDate", reportDate);

            officeDesc = "";
            for(i=0;i<officeDescList.Count;i++) officeDesc += (officeDesc=="" ? "" : ", ") + officeDescList[i];
            report.SetParameterValue("SelectedOffice", officeDesc);

            paymentTermDesc = "";
            for (i=0;i<paymentTermDescList.Count;i++) paymentTermDesc += (paymentTermDesc=="" ? "" : ", ")+paymentTermDescList[i];
            report.SetParameterValue("SelectedPaymentTerm", paymentTermDesc);
            report.SetParameterValue("ExportType", ExportType);
            
            return report;
        }


        public AccrualArchiveReport getSalesAccrualArchiveReport(int officeId, int fiscalYear, int period, int userId)
        {
            AccountFinancialCalenderDef calDef = CommonUtil.getAccountPeriodByYearPeriod(9, fiscalYear, period);

            AccrualArchiveReport report = new AccrualArchiveReport();
            AccrualArchiveDs reportDs;

            reportDs = ReporterWorker.Instance.getAccrualArchiveList(officeId, fiscalYear, period);
            report.SetDataSource(reportDs);
            report.SetParameterValue("Office", OfficeId.getName(officeId));
            report.SetParameterValue("FiscalPeriod", "P" + period.ToString().PadLeft(2, '0') + "/" + fiscalYear.ToString());
            report.SetParameterValue("StartDate", calDef.StartDate);
            report.SetParameterValue("EndDate", calDef.EndDate);

            return report;
        }

        public SupplierOrderStatusReport getSupplierOrderStatusReport(DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo,
            TypeCollector officeList, TypeCollector productTeamList, int vendorId, int paymentTermId, int printUserId)
        {
            SupplierOrderStatusReport report = new SupplierOrderStatusReport();
            SupplierOrderStatusReportDs reportDs = ReporterWorker.Instance.getSupplierOrderStatusReport(customerAtWHDateFrom, customerAtWHDateTo,
                officeList, productTeamList, vendorId, paymentTermId);

            report.SetDataSource(reportDs);

            report.SetParameterValue("customerAtWHDateFrom", DateTimeUtility.getDateString(customerAtWHDateFrom));
            report.SetParameterValue("customerAtWHDateTo", DateTimeUtility.getDateString(customerAtWHDateTo));

            string office = "";
            foreach (object id in officeList.Values)
            {
                office += GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).OfficeCode + ", ";
            }
            if (office != "")
            {
                office = office.Remove(office.LastIndexOf(", "));
            }
            report.SetParameterValue("office", office);

            if (productTeamList.Values.Count == 1)
            {
                foreach (object productTeamId in productTeamList.Values)
                {
                    ProductCodeDef pc = GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32(productTeamId));
                    report.SetParameterValue("productTeam", pc.CodeDescription);
                }
            }
            else
                report.SetParameterValue("productTeam", "");

            report.SetParameterValue("vendor", vendorId == -1 ? "" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
            report.SetParameterValue("paymentTerm", paymentTermId == -1 ? "" : commonWorker.getPaymentTermByKey(paymentTermId).PaymentTermDescription);
            report.SetParameterValue("printUser", GeneralWorker.Instance.getUserByKey(printUserId).DisplayName);

            return report;
        }

        public ReleaseLockSummary getReleaseLockSummary(string officeName, TypeCollector officeIdList, int handlingOfficeId, DateTime releaseLockDateFrom, DateTime releaseLockDateTo,
            int isSampleOrder, TypeCollector customerIdList, TypeCollector tradingAgencyIdList, 
            int reversingEntryRequired, int dcNoteRequired, int ilsTempACRequired, int userId)
        {
            ReleaseLockSummary report = new ReleaseLockSummary();
            ReleaseLockDs ds = ReporterWorker.Instance.getReleaseLockSummary(officeIdList, handlingOfficeId, releaseLockDateFrom, releaseLockDateTo, isSampleOrder, customerIdList,tradingAgencyIdList, 
                reversingEntryRequired, dcNoteRequired , ilsTempACRequired);
            ReleaseLockDs tmpDs = ReporterWorker.Instance.getReleaseLockReason(officeIdList, releaseLockDateFrom, releaseLockDateTo, isSampleOrder, customerIdList, tradingAgencyIdList);
            ds.Tables["ReleaseLockReason"].Merge(tmpDs.Tables["ReleaseLockReason"], true);
            report.SetDataSource(ds);

            //report.SetParameterValue("Office", officeId == -1 ? "" : OfficeId.getName(officeId));
            report.SetParameterValue("Office", officeName);
            report.SetParameterValue("HandlingOffice", handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).OfficeCode);

            report.SetParameterValue("printUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            report.SetParameterValue("releaseLockDateFrom", DateTimeUtility.getDateString(releaseLockDateFrom));
            report.SetParameterValue("releaseLockDateTo", DateTimeUtility.getDateString(releaseLockDateTo));

            string nameList = "";
            foreach (object id in customerIdList.Values)
                if ((int)id != -1)
                    nameList += (nameList == "" ? "" : ", ") + commonWorker.getCustomerByKey((int)id).CustomerCode;
            if (nameList == "")
                nameList = "NIL";
            report.SetParameterValue("Customer", nameList);

            nameList = "";
            foreach (object id in tradingAgencyIdList.Values)
                if ((int)id != -1)
                    nameList += (nameList == "" ? "" : ", ") + commonWorker.getTradingAgencyByKey((int)id).ShortName;
            if (nameList == "")
                nameList = "NIL";
            report.SetParameterValue("TradingAgency", nameList);

            return report;
        }


        public QADebitNote getQADebitNote(ArrayList invoiceList)
        {
            QADebitNote report = new QADebitNote();
            QADebitNoteDs QADs = new QADebitNoteDs();
            ReporterWorker reportWorker = ReporterWorker.Instance;

            foreach (ContractShipmentListJDef def in invoiceList)
            {
                QADebitNoteDs.QADebitNoteRow row = QADs.QADebitNote.NewQADebitNoteRow();

                row.InvoiceNo = def.InvoiceNo;
                row.InvoiceDate = def.InvoiceDate;
                row.ContractNo = def.ContractNo;
                row.ItemNo = def.ItemNo;
                row.VendorId = def.Vendor.VendorId;
                row.Name = def.Vendor.Name;
                row.Addr1 = def.Vendor.Address0;
                row.Addr2 = def.Vendor.Address1;
                row.Addr3 = def.Vendor.Address2;
                row.Addr4 = def.Vendor.Address3;
                row.SupplierInvoiceNo = def.SupplierInvoiceNo;
                row.Season = def.Season.Code;
                row.TotalShippedSupplierGmtAmtAfterDiscount = def.TotalShippedSupplierGarmentAmountAfterDiscount;
                row.QACommissionPercent = Convert.ToInt32(def.QACommissionPercent);
                row.QACommissionAmount = Math.Round(def.TotalShippedSupplierGarmentAmountAfterDiscount * def.QACommissionPercent / 100, 2, MidpointRounding.AwayFromZero);
                row.TotalShippedQty = def.TotalShippedQuantity;
                row.Currency = def.BuyCurrency.CurrencyCode;

                QADs.QADebitNote.AddQADebitNoteRow(row);
            }

            report.SetDataSource(QADs);


            return report;
        }

        public MockShopDebitNote getMockShopDebitNote(int dcNoteId)
        {
            ArrayList debitNoteList = new ArrayList();
            ArrayList debitNoteShipmentList = null;
            ArrayList debitNoteShipmentDetailList = null;

            MockShopDCNoteDef def = accountWorker.getMockShopDCNoteByKey(dcNoteId);
            debitNoteList.Add(def);
            AccountFinancialCalenderDef calDef = GeneralWorker.Instance.getAccountPeriodByDate(AppId.NSS.Code, def.DCNoteDate);

            debitNoteShipmentList = accountWorker.getMockShopDCNoteShipmentByDCNoteId(dcNoteId);
            debitNoteShipmentDetailList = accountWorker.getMockShopDCNoteShipmentDetailByDCNoteId(dcNoteId);
            return this.getMockShopDebitNote(debitNoteList, debitNoteShipmentList, debitNoteShipmentDetailList, false, calDef.BudgetYear, calDef.Period);
        }

        public MockShopDebitNote getStudioDebitNote(int dcNoteId)
        {
            ArrayList debitNoteList = new ArrayList();
            ArrayList debitNoteShipmentList = null;
            ArrayList debitNoteShipmentDetailList = null;

            StudioDCNoteDef def = accountWorker.getStudioDCNoteByKey(dcNoteId);
            debitNoteList.Add(def);
            AccountFinancialCalenderDef calDef = GeneralWorker.Instance.getAccountPeriodByDate(AppId.NSS.Code, def.DCNoteDate);

            debitNoteShipmentList = accountWorker.getStudioDCNoteShipmentByDCNoteId(dcNoteId);
            debitNoteShipmentDetailList = accountWorker.getStudioDCNoteShipmentDetailByDCNoteId(dcNoteId);
            return this.getStudioDebitNote(debitNoteList, debitNoteShipmentList, debitNoteShipmentDetailList, false, calDef.BudgetYear, calDef.Period);
        }


        public MockShopDebitNote getMockShopDebitNote(ArrayList debitNoteList, ArrayList debitNoteShipmentList, ArrayList debitNoteShipmentDetailList,
            bool isDraft, int fiscalYear, int period)
        {
            string sampleType = "M";
            ReporterWorker reportWorker = ReporterWorker.Instance;
            MockShopDebitNote report = new MockShopDebitNote();
            MockShopDebitNoteDs ds = new MockShopDebitNoteDs();
            MockShopDebitNoteDs.MockShopDebitNoteRow row = null;
            MockShopDebitNoteDs.MockShopDCNoteShipmentRow shipmentRow = null;
            MockShopDebitNoteDs.MockShopDCNoteShipmentDetailRow detailRow = null;
            VendorWorker vendorWorker = VendorWorker.Instance;
            OrderSelectWorker orderSelectWorker = OrderSelectWorker.Instance;
            ProductWorker productWorker = ProductWorker.Instance;
            ShippingWorker shippingWorker = ShippingWorker.Instance;

            if (debitNoteList.Count == 0)
            {
                report.SetDataSource(ds);
                report.SetParameterValue("period", period);
                report.SetParameterValue("year", fiscalYear);
                report.SetParameterValue("isDraft", isDraft);
                report.SetParameterValue("sampleType", sampleType);

                return report;
            }

            MockShopDCNoteDef noteDef = (MockShopDCNoteDef)debitNoteList[0];

            int officeId = ((MockShopDCNoteShipmentDef)debitNoteShipmentList[0]).OfficeId;
            OfficeRef office = GeneralWorker.Instance.getOfficeRefByKey(noteDef.OfficeId);
            DateTime debitNoteDate = noteDef.DCNoteDate;

            Hashtable currencyList = new Hashtable();
            Hashtable deptList = new Hashtable();
            Hashtable dcList = new Hashtable();
            ContractDef contractDef = null;
            InvoiceDef invoiceDef = null;
            decimal exchangeRate = 0;
            //Declare by Toby on 2015-04-20
            decimal exchangeRateToHKD = 0;

            foreach (MockShopDCNoteDef dcNote in debitNoteList)
            {
                dcList.Add(dcNote.DCNoteId, dcNote.DCNoteNo);
            }

            foreach (MockShopDCNoteShipmentDef shipment in debitNoteShipmentList)
            {
                row = ds.MockShopDebitNote.NewMockShopDebitNoteRow();

                row.TotalShippedAmt = shipment.TotalShippedAmount;
                row.TotalCourierCharge = shipment.TotalCourierCharge;

                if (!currencyList.Contains(shipment.SellCurrencyId))
                {
                    currencyList.Add(shipment.SellCurrencyId, GeneralWorker.Instance.getCurrencyByKey(shipment.SellCurrencyId).CurrencyCode);
                }
                if (!deptList.Contains(shipment.DeptId))
                {
                    deptList.Add(shipment.DeptId, GeneralWorker.Instance.getProductDepartmentByKey(shipment.DeptId).Description);
                }
                row.SellCurrency = currencyList[shipment.SellCurrencyId].ToString();
                row.Dept = deptList[shipment.DeptId].ToString();
                row.Office = office.Description.ToUpper();
                row.OfficeId = office.OfficeId;
                row.TermOfPurchaseId = shipment.TermOfPurchaseId;
                row.NSLCommissionAmt = shipment.NSLCommissionAmt;

                row.DCNoteNo = dcList[shipment.DCNoteId].ToString();
                row.DCNoteId = shipment.DCNoteId;
                row.DCNoteDate = debitNoteDate;

                DebitNoteToNUKParamDef paramDef = reportWorker.getDebitNoteToNUKParamByKey(officeId, shipment.SellCurrencyId);
                row.BenificiaryAccountNo = reportWorker.getBeneficiaryAccountNoList(officeId, shipment.SellCurrencyId);
                row.SupplierCode = paramDef.SupplierCode;
                row.BeneficiaryName = paramDef.BeneficiaryName;
                row.BankName = paramDef.BankName;
                row.BankAddress = paramDef.BankAddress;
                row.SwiftCode = paramDef.SwiftCode;

                ds.MockShopDebitNote.AddMockShopDebitNoteRow(row);

                invoiceDef = shippingWorker.getInvoiceByKey(shipment.ShipmentId);
                //Modified by Toby to get Mock Shop Exchange Rate by refer Debit Note date instead of Invoice Date.
                //It align the logic for Epicor.
                //exchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, invoiceDef.InvoiceDate);
                exchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, debitNoteDate);
                exchangeRateToHKD = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, shipment.SellCurrencyId, debitNoteDate);

                contractDef = orderSelectWorker.getContractByKey(shipment.ContractId);
                shipmentRow = ds.MockShopDCNoteShipment.NewMockShopDCNoteShipmentRow();
                shipmentRow.DCNoteId = shipment.DCNoteId;
                shipmentRow.DCNoteNo = row.DCNoteNo;
                shipmentRow.Supplier = vendorWorker.getVendorByKey(shipment.VendorId).Name;
                shipmentRow.InvoiceNo = ShippingWorker.getInvoiceNo(invoiceDef.InvoicePrefix, invoiceDef.InvoiceSeqNo, invoiceDef.InvoiceYear);
                shipmentRow.ContractNo = contractDef.ContractNo;
                shipmentRow.PackingUnit = PackingUnitRef.getUnitCode(shipment.PackingUnitId);
                shipmentRow.SellCurrency = row.SellCurrency;
                shipmentRow.Season = contractDef.Season.Code;
                shipmentRow.ItemNo = productWorker.getProductByKey(shipment.ProductId).ItemNo;
                shipmentRow.DCNoteShipmentId = shipment.DCNoteShipmentId;
                shipmentRow.ShipmentId = shipment.ShipmentId;
                shipmentRow.TotalCourierCharge = shipment.TotalCourierCharge;
                shipmentRow.NSLCommissionPercent = shipment.NSLCommissionPercent;
                //shipmentRow.NSLCommissionAmt = Math.Round(shipment.NSLCommissionAmt * invoiceDef.InvoiceSellExchangeRate / exchangeRate, 2, MidpointRounding.AwayFromZero);
                shipmentRow.NSLCommissionAmt = Math.Round(shipment.NSLCommissionAmt, 2, MidpointRounding.AwayFromZero);
                //Amend by Toby on 2015-04-20
                //shipmentRow.TotalShippedAmtUSD = Math.Round(shipment.TotalShippedAmount * invoiceDef.InvoiceSellExchangeRate / exchangeRate, 2, MidpointRounding.AwayFromZero);
                shipmentRow.TotalShippedAmtUSD = Math.Round(shipment.TotalShippedAmount * exchangeRateToHKD / exchangeRate, 2, MidpointRounding.AwayFromZero);
                shipmentRow.TotalShippedAmt = shipment.TotalShippedAmount;
                shipmentRow.InvoiceSellExchangeRate = invoiceDef.InvoiceSellExchangeRate;
                shipmentRow.USDExchangeRate = exchangeRate;
                shipmentRow.ProductTeamCode = contractDef.ProductTeam.Code;
                ds.MockShopDCNoteShipment.AddMockShopDCNoteShipmentRow(shipmentRow);
            }

            foreach (MockShopDCNoteShipmentDetailDef detailDef in debitNoteShipmentDetailList)
            {
                detailRow = ds.MockShopDCNoteShipmentDetail.NewMockShopDCNoteShipmentDetailRow();

                detailRow.DCNoteShipmentId = detailDef.DCNoteShipmentId;
                detailRow.DCNoteShipmentDetailId = detailDef.DCNoteShipmentDetailId;
                detailRow.ShipmentId = detailDef.ShipmentId;
                detailRow.ShipmentDetailId = detailDef.ShipmentDetailId;

                detailRow.SizeOptionId = detailDef.SizeOptionId;
                detailRow.ShippedQty = detailDef.ShippedQty;
                detailRow.SellingPrice = detailDef.SellingPrice;
                detailRow.ShippedAmt = detailDef.SellingPrice * detailDef.ShippedQty;
                //detailRow.ShippedAmtUSD = 
                ds.MockShopDCNoteShipmentDetail.AddMockShopDCNoteShipmentDetailRow(detailRow);
            }

            report.SetDataSource(ds);
            report.SetParameterValue("period", period);
            report.SetParameterValue("year", fiscalYear);
            report.SetParameterValue("isDraft", isDraft);
            report.SetParameterValue("sampleType", sampleType);

            return report;

        }

        public MockShopDebitNote getStudioDebitNote(ArrayList debitNoteList, ArrayList debitNoteShipmentList, ArrayList debitNoteShipmentDetailList,
            bool isDraft, int fiscalYear, int period)
        {
            string sampleType = "S";
            ReporterWorker reportWorker = ReporterWorker.Instance;
            MockShopDebitNote report = new MockShopDebitNote();
            MockShopDebitNoteDs ds = new MockShopDebitNoteDs();
            MockShopDebitNoteDs.MockShopDebitNoteRow row = null;
            MockShopDebitNoteDs.MockShopDCNoteShipmentRow shipmentRow = null;
            MockShopDebitNoteDs.MockShopDCNoteShipmentDetailRow detailRow = null;
            VendorWorker vendorWorker = VendorWorker.Instance;
            OrderSelectWorker orderSelectWorker = OrderSelectWorker.Instance;
            ProductWorker productWorker = ProductWorker.Instance;
            ShippingWorker shippingWorker = ShippingWorker.Instance;

            if (debitNoteList.Count == 0)
            {
                report.SetDataSource(ds);
                report.SetParameterValue("period", period);
                report.SetParameterValue("year", fiscalYear);
                report.SetParameterValue("isDraft", isDraft);
                report.SetParameterValue("sampleType", sampleType);

                return report;
            }

            StudioDCNoteDef noteDef = (StudioDCNoteDef)debitNoteList[0];

            int officeId = ((StudioDCNoteShipmentDef)debitNoteShipmentList[0]).OfficeId;
            OfficeRef office = GeneralWorker.Instance.getOfficeRefByKey(noteDef.OfficeId);
            DateTime debitNoteDate = noteDef.DCNoteDate;

            Hashtable currencyList = new Hashtable();
            Hashtable deptList = new Hashtable();
            Hashtable dcList = new Hashtable();
            ContractDef contractDef = null;
            InvoiceDef invoiceDef = null;
            decimal exchangeRate = 0;
            //Declare by Toby on 2015-04-20
            decimal exchangeRateToHKD = 0;

            foreach (StudioDCNoteDef dcNote in debitNoteList)
            {
                dcList.Add(dcNote.DCNoteId, dcNote.DCNoteNo);
            }

            foreach (StudioDCNoteShipmentDef shipment in debitNoteShipmentList)
            {
                row = ds.MockShopDebitNote.NewMockShopDebitNoteRow();

                row.TotalShippedAmt = shipment.TotalShippedAmount;
                row.TotalCourierCharge = shipment.TotalCourierCharge;

                if (!currencyList.Contains(shipment.SellCurrencyId))
                {
                    currencyList.Add(shipment.SellCurrencyId, GeneralWorker.Instance.getCurrencyByKey(shipment.SellCurrencyId).CurrencyCode);
                }
                if (!deptList.Contains(shipment.DeptId))
                {
                    deptList.Add(shipment.DeptId, GeneralWorker.Instance.getProductDepartmentByKey(shipment.DeptId).Description);
                }
                row.SellCurrency = currencyList[shipment.SellCurrencyId].ToString();
                row.Dept = deptList[shipment.DeptId].ToString();
                row.Office = office.Description.ToUpper();
                row.OfficeId = office.OfficeId;
                row.TermOfPurchaseId = shipment.TermOfPurchaseId;
                row.NSLCommissionAmt = shipment.NSLCommissionAmt;

                row.DCNoteNo = dcList[shipment.DCNoteId].ToString();
                row.DCNoteId = shipment.DCNoteId;
                row.DCNoteDate = debitNoteDate;

                DebitNoteToNUKParamDef paramDef = reportWorker.getDebitNoteToNUKParamByKey(officeId, shipment.SellCurrencyId);
                row.BenificiaryAccountNo = reportWorker.getBeneficiaryAccountNoList(officeId, shipment.SellCurrencyId);
                row.SupplierCode = paramDef.SupplierCode;
                row.BeneficiaryName = paramDef.BeneficiaryName;
                row.BankName = paramDef.BankName;
                row.BankAddress = paramDef.BankAddress;
                row.SwiftCode = paramDef.SwiftCode;

                ds.MockShopDebitNote.AddMockShopDebitNoteRow(row);

                invoiceDef = shippingWorker.getInvoiceByKey(shipment.ShipmentId);
                //Modified by Toby to get Mock Shop Exchange Rate by refer Debit Note date instead of Invoice Date.
                //It align the logic for Epicor.
                //exchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, invoiceDef.InvoiceDate);
                exchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, debitNoteDate);
                exchangeRateToHKD = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, shipment.SellCurrencyId, debitNoteDate);

                contractDef = orderSelectWorker.getContractByKey(shipment.ContractId);
                shipmentRow = ds.MockShopDCNoteShipment.NewMockShopDCNoteShipmentRow();
                shipmentRow.DCNoteId = shipment.DCNoteId;
                shipmentRow.DCNoteNo = row.DCNoteNo;
                shipmentRow.Supplier = vendorWorker.getVendorByKey(shipment.VendorId).Name;
                shipmentRow.InvoiceNo = ShippingWorker.getInvoiceNo(invoiceDef.InvoicePrefix, invoiceDef.InvoiceSeqNo, invoiceDef.InvoiceYear);
                shipmentRow.ContractNo = contractDef.ContractNo;
                shipmentRow.PackingUnit = PackingUnitRef.getUnitCode(shipment.PackingUnitId);
                shipmentRow.SellCurrency = row.SellCurrency;
                shipmentRow.Season = contractDef.Season.Code;
                shipmentRow.ItemNo = productWorker.getProductByKey(shipment.ProductId).ItemNo;
                shipmentRow.DCNoteShipmentId = shipment.DCNoteShipmentId;
                shipmentRow.ShipmentId = shipment.ShipmentId;
                shipmentRow.TotalCourierCharge = shipment.TotalCourierCharge;
                shipmentRow.NSLCommissionPercent = shipment.NSLCommissionPercent;
                //shipmentRow.NSLCommissionAmt = Math.Round(shipment.NSLCommissionAmt * invoiceDef.InvoiceSellExchangeRate / exchangeRate, 2, MidpointRounding.AwayFromZero);
                shipmentRow.NSLCommissionAmt = Math.Round(shipment.NSLCommissionAmt, 2, MidpointRounding.AwayFromZero);
                //Amend by Toby on 2015-04-20
                //shipmentRow.TotalShippedAmtUSD = Math.Round(shipment.TotalShippedAmount * invoiceDef.InvoiceSellExchangeRate / exchangeRate, 2, MidpointRounding.AwayFromZero);
                shipmentRow.TotalShippedAmtUSD = Math.Round(shipment.TotalShippedAmount * exchangeRateToHKD / exchangeRate, 2, MidpointRounding.AwayFromZero);
                shipmentRow.TotalShippedAmt = shipment.TotalShippedAmount;
                shipmentRow.InvoiceSellExchangeRate = invoiceDef.InvoiceSellExchangeRate;
                shipmentRow.USDExchangeRate = exchangeRate;
                shipmentRow.ProductTeamCode = contractDef.ProductTeam.Code;
                ds.MockShopDCNoteShipment.AddMockShopDCNoteShipmentRow(shipmentRow);
            }

            foreach (StudioDCNoteShipmentDetailDef detailDef in debitNoteShipmentDetailList)
            {
                detailRow = ds.MockShopDCNoteShipmentDetail.NewMockShopDCNoteShipmentDetailRow();

                detailRow.DCNoteShipmentId = detailDef.DCNoteShipmentId;
                detailRow.DCNoteShipmentDetailId = detailDef.DCNoteShipmentDetailId;
                detailRow.ShipmentId = detailDef.ShipmentId;
                detailRow.ShipmentDetailId = detailDef.ShipmentDetailId;

                detailRow.SizeOptionId = detailDef.SizeOptionId;
                detailRow.ShippedQty = detailDef.ShippedQty;
                detailRow.SellingPrice = detailDef.SellingPrice;
                detailRow.ShippedAmt = detailDef.SellingPrice * detailDef.ShippedQty;
                //detailRow.ShippedAmtUSD = 
                ds.MockShopDCNoteShipmentDetail.AddMockShopDCNoteShipmentDetailRow(detailRow);
            }

            report.SetDataSource(ds);
            report.SetParameterValue("period", period);
            report.SetParameterValue("year", fiscalYear);
            report.SetParameterValue("isDraft", isDraft);
            report.SetParameterValue("sampleType", sampleType);

            return report;

        }


        public UTDebitNote getUTDebitNote(int dcNoteId)
        {
            UTContractDCNoteDef dcNoteDef = accountWorker.getUTContractDCNoteByKey(dcNoteId);
            ArrayList list = accountWorker.getUTContractShipmentList(dcNoteId);
            return this.getUTDebitNote(ConvertUtility.createArrayList(dcNoteDef), list, false, dcNoteDef.FiscalYear, dcNoteDef.Period);
        }

        public UTDebitNote getUTDebitNote(ArrayList debitNoteList, ArrayList debitNoteShipmentList, bool isDraft, int fiscalYear, int period)
        {
            ReporterWorker reportWorker = ReporterWorker.Instance;
            UTDebitNote report = new UTDebitNote();
            UTDebitNoteDs ds = new UTDebitNoteDs();
            UTDebitNoteDs.UTDebitNoteRow row = null;
            UTDebitNoteDs.UTDCNoteShipmentRow shipmentRow = null;

            VendorWorker vendorWorker = VendorWorker.Instance;
            OrderSelectWorker orderSelectWorker = OrderSelectWorker.Instance;
            ProductWorker productWorker = ProductWorker.Instance;
            ShippingWorker shippingWorker = ShippingWorker.Instance;
            AccountFinancialCalenderDef calcDef = generalWorker.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, period);

            if (debitNoteList.Count == 0)
            {
                report.SetDataSource(ds);
                report.SetParameterValue("period", period);
                report.SetParameterValue("year", fiscalYear);
                report.SetParameterValue("isDraft", isDraft);

                return report;
            }

            UTContractDCNoteDef noteDef = (UTContractDCNoteDef)debitNoteList[0];

            int officeId = ((UTContractDCNoteShipmentDef)debitNoteShipmentList[0]).OfficeId;
            OfficeRef office = GeneralWorker.Instance.getOfficeRefByKey(noteDef.OfficeId);
            DateTime debitNoteDate = noteDef.DCNoteDate;

            Hashtable currencyList = new Hashtable();
            Hashtable deptList = new Hashtable();
            Hashtable dcList = new Hashtable();
            ContractDef contractDef = null;
            InvoiceDef invoiceDef = null;
            ShipmentDef shipmentDef = null;
            decimal exchangeRate = 0;
            decimal exchangeRateToHKD = 0;

            foreach (UTContractDCNoteDef dcNote in debitNoteList)
            {
                dcList.Add(dcNote.DCNoteId, dcNote.DCNoteNo);

                row = ds.UTDebitNote.NewUTDebitNoteRow();
                row.TotalCommission = dcNote.TotalCommission;
                row.TotalMargin = dcNote.TotalMargin;
                row.TotalServiceFee = dcNote.TotalServiceFee;
                row.TotalSupplierCommission = dcNote.TotalSupplierCommission;
                row.TotalSupplierGmtAmt = dcNote.TotalSupplierGmtAmt;
                row.DCNoteNo = dcList[dcNote.DCNoteId].ToString();
                row.DCNoteId = dcNote.DCNoteId;
                row.DCNoteDate = debitNoteDate;
                row.FromDate = calcDef.StartDate;
                row.ToDate = calcDef.EndDate;

                DebitNoteToNUKParamDef paramDef = reportWorker.getDebitNoteToNUKParamByKey(officeId, dcNote.SellCurrencyId);
                row.BenificiaryAccountNo = reportWorker.getBeneficiaryAccountNoList(officeId, dcNote.SellCurrencyId);
                row.BeneficiaryName = paramDef.BeneficiaryName;
                row.BankName = paramDef.BankName;
                row.BankAddress = paramDef.BankAddress;
                row.SwiftCode = paramDef.SwiftCode;

                ds.UTDebitNote.AddUTDebitNoteRow(row);
            }

            foreach (UTContractDCNoteShipmentDef shipment in debitNoteShipmentList)
            {

                invoiceDef = shippingWorker.getInvoiceByKey(shipment.ShipmentId);
                shipmentDef = orderSelectWorker.getShipmentByKey(shipment.ShipmentId);
                exchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, debitNoteDate);
                exchangeRateToHKD = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, shipment.SellCurrencyId, debitNoteDate);

                contractDef = orderSelectWorker.getContractByKey(shipment.ContractId);
                shipmentRow = ds.UTDCNoteShipment.NewUTDCNoteShipmentRow();
                shipmentRow.DCNoteId = shipment.DCNoteId;
                shipmentRow.DCNoteNo = row.DCNoteNo;
                shipmentRow.Supplier = vendorWorker.getVendorByKey(shipment.VendorId).Name;
                shipmentRow.InvoiceNo = ShippingWorker.getInvoiceNo(invoiceDef.InvoicePrefix, invoiceDef.InvoiceSeqNo, invoiceDef.InvoiceYear);
                shipmentRow.ContractNo = contractDef.ContractNo;
                shipmentRow.SellCurrency = GeneralWorker.Instance.getCurrencyByKey(shipment.SellCurrencyId).CurrencyCode;
                shipmentRow.Season = contractDef.Season.Code;
                shipmentRow.ItemNo = productWorker.getProductByKey(shipment.ProductId).ItemNo;
                shipmentRow.DCNoteShipmentId = shipment.DCNoteShipmentId;
                shipmentRow.ShipmentId = shipment.ShipmentId;
                shipmentRow.Commission = shipment.Commission;
                shipmentRow.Margin = shipment.Margin;
                shipmentRow.ServiceFee = shipment.ServiceFee;
                shipmentRow.SupplierGmtAmt = shipment.SupplierGmtAmt;
                shipmentRow.SupplierCommission = shipment.SupplierCommission;
                shipmentRow.InvoiceSellExchangeRate = invoiceDef.InvoiceSellExchangeRate;
                shipmentRow.USDExchangeRate = exchangeRate;
                shipmentRow.ProductTeamCode = contractDef.ProductTeam.Code;
                shipmentRow.DeliveryNo = shipmentDef.DeliveryNo;
                shipmentRow.CNYExchangeRate = shipment.CNYExchangeRate;
                shipmentRow.SupplierGmtAmtInCNY = shipment.SupplierGmtAmtInCNY;
                ds.UTDCNoteShipment.AddUTDCNoteShipmentRow(shipmentRow);
            }

            report.SetDataSource(ds);
            report.SetParameterValue("period", period);
            report.SetParameterValue("year", fiscalYear);
            report.SetParameterValue("isDraft", isDraft);

            return report;
        }

        public UTQADebitNoteReport getQADebitNote(int dcNoteId)
        {
            QACommissionDNDef dcNoteDef = accountWorker.getQACommissionDCNoteByKey(dcNoteId);
            ArrayList list = accountWorker.getQACommissionDNShipmentByDNId(dcNoteId);
            return this.getQACommissiomDebitNote(ConvertUtility.createArrayList(dcNoteDef), list, false, dcNoteDef.FiscalYear, dcNoteDef.Period);
        }

        public UTQADebitNoteReport getQACommissiomDebitNote(ArrayList qaDCNoteVendor, ArrayList qaDCNoteShipmentListVendor, bool isDraft, int fiscalYear, int period)
        {
            ReporterWorker reportWorker = ReporterWorker.Instance;
            UTQADebitNoteReport report = new UTQADebitNoteReport();
            UTQACommissionDNRepDs ds = new UTQACommissionDNRepDs();
            UTQACommissionDNRepDs.UTQADebitNoteRow row = null;
            UTQACommissionDNRepDs.UTQADebitNoteShipmentRow shipmentRow = null;

            int vendorId = ((QACommissionDNShipmentDef)qaDCNoteShipmentListVendor[0]).VendorId;
            VendorWorker vendorWorker = VendorWorker.Instance;
            OrderSelectWorker orderSelectWorker = OrderSelectWorker.Instance;
            ProductWorker productWorker = ProductWorker.Instance;
            AccountFinancialCalenderDef calcDef = generalWorker.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, period);

            QACommissionDNDef noteDef = (QACommissionDNDef)qaDCNoteVendor[0];

            DateTime debitNoteDate = noteDef.DNDate;

            Hashtable currencyList = new Hashtable();
            Hashtable dcList = new Hashtable();
            ContractDef contractDef = null;
            ShipmentDef shipmentDef = null;

            foreach (QACommissionDNDef dcNote in qaDCNoteVendor)
            {
                dcList.Add(dcNote.DNId, dcNote.DNNo);

                row = ds.UTQADebitNote.NewUTQADebitNoteRow();
                VendorRef vendor = vendorWorker.getVendorByKey(vendorId);
                row.Name = vendor.Name;
                row.Add1 = vendor.Address0;
                row.Add2 = vendor.Address1;
                row.Add3 = vendor.Address2;
                row.Add4 = vendor.Address3;
                row.DNNo = dcNote.DNNo;
                row.DNDate = dcNote.DNDate;

                ds.UTQADebitNote.AddUTQADebitNoteRow(row);
            }

            foreach (QACommissionDNShipmentDef shipment in qaDCNoteShipmentListVendor)
            {
                shipmentDef = orderSelectWorker.getShipmentByKey(shipment.ShipmentId);

                contractDef = orderSelectWorker.getContractByKey(shipmentDef.ContractId);
                shipmentRow = ds.UTQADebitNoteShipment.NewUTQADebitNoteShipmentRow();
                shipmentRow.ContractNo = contractDef.ContractNo + "-" + shipmentDef.DeliveryNo;
                shipmentRow.Season = contractDef.Season.Code;
                shipmentRow.ItemNo = productWorker.getProductByKey(contractDef.ProductId).ItemNo;
                shipmentRow.ProductTeam = contractDef.ProductTeam.Code;
                if (!currencyList.Contains(shipment.CurrencyId))
                {
                    currencyList.Add(shipment.CurrencyId, GeneralWorker.Instance.getCurrencyByKey(shipment.CurrencyId).CurrencyCode);
                }
                shipmentRow.Currency = currencyList[shipment.CurrencyId].ToString();
                shipmentRow.ShippedQty = shipment.ShippedQty;
                shipmentRow.SupplierAmt = shipment.SupplierAmount;
                shipmentRow.QACommissionAmt = shipment.QACommissionAmount;
                shipmentRow.QACommissionPercent = shipment.QACommissionPercent;

                ds.UTQADebitNoteShipment.AddUTQADebitNoteShipmentRow(shipmentRow);
            }

            report.SetDataSource(ds);

            report.SetParameterValue("period", period);
            report.SetParameterValue("year", fiscalYear);
            report.SetParameterValue("isDraft", isDraft);
            report.SetParameterValue("periodDate", DateTimeUtility.getDateString(calcDef.StartDate) + " to " + DateTimeUtility.getDateString(calcDef.EndDate.Date));

            return report;
        }

        public UKClaimDCNoteReport getUKClaimDCNote(int dcNoteId)
        {
            UKClaimDCNoteReportDs ds = ReporterWorker.Instance.getUKClaimDCNote(dcNoteId);
            UKClaimDCNoteReport report = new UKClaimDCNoteReport();
            UKClaimDCNoteReportDs.UKClaimDCNoteReportRow r = ds.UKClaimDCNoteReport[0];

            foreach (UKClaimDCNoteReportDs.UKClaimDCNoteReportRow rr in ds.UKClaimDCNoteReport)
            {
                UKClaimDef ukClaimDef = UKClaimWorker.Instance.getUKClaimByKey(rr.ClaimId);

                if (ukClaimDef.Type.Id == UKClaimType.BILL_IN_ADVANCE.Id && ukClaimDef.DebitNoteNo != string.Empty)
                {
                    QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
                    QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(int.Parse(ukClaimDef.DebitNoteNo));
                    if (requestDef.ClaimType.ToString() == UKClaimType.MFRN.Name)
                        rr.UKDebitNoteNo = "MFRN: " + requestDef.FormNo;
                    else
                        rr.UKDebitNoteNo = requestDef.FormNo;
                }
            }

            UKClaimDCNoteReportDs.BenificiaryAccountRow row = ds.BenificiaryAccount.NewBenificiaryAccountRow();
            DebitNoteToNUKParamDef paramDef = ReporterWorker.Instance.getDebitNoteToNUKParamByKey(r.OfficeId, r.CurrencyId);
            row.BenificiaryAccountNo = ReporterWorker.Instance.getBeneficiaryAccountNoList(r.OfficeId, r.CurrencyId);
            row.SupplierCode = paramDef.SupplierCode;
            row.BeneficiaryName = paramDef.BeneficiaryName;
            row.BankName = paramDef.BankName;
            row.BankAddress = paramDef.BankAddress;
            row.SwiftCode = paramDef.SwiftCode;
            row.CurrencyId = r.CurrencyId;
            row.OfficeId = r.OfficeId;
            row.CurrencyCode = GeneralWorker.Instance.getCurrencyByKey(r.CurrencyId).CurrencyCode;
            row.OfficeName = GeneralWorker.Instance.getOfficeRefByKey(r.OfficeId).Description.Replace("Office", string.Empty);
            ds.BenificiaryAccount.AddBenificiaryAccountRow(row);

            report.SetDataSource(ds);
            report.SetParameterValue("isDraft", "N");
            return report;
        }

        /*
        public UKClaimDCNote getUKClaimRefundCreditNote(int claimRefundId)
        {
            UKClaimDCNoteDs ds = ReporterWorker.Instance.getUKClaimRefundDCNote(claimRefundId);
            UKClaimDCNote report = new UKClaimDCNote();

            report.SetDataSource(ds);
            report.SetParameterValue("isDraft", "N");
            return report;
        }
        */

        public SAndBCommissionDebitNote getCommissionDebitNote(ArrayList shipmentIdList)
        {
            SAndBCommissionDebitNote debitNote = new SAndBCommissionDebitNote();
            SAndBCommissionDebitNoteDs ds = new SAndBCommissionDebitNoteDs();
            int shipmentId = 0;
            try
            {
                foreach (object obj in shipmentIdList)
                {
                    shipmentId = (int)obj;

                    SAndBCommissionDebitNoteDs dsDN = ReporterWorker.Instance.getCommissionDebitNote(shipmentId);
                    SAndBCommissionDebitNoteDs.SAndBCommissionDebitNoteRow row = (SAndBCommissionDebitNoteDs.SAndBCommissionDebitNoteRow)dsDN.SAndBCommissionDebitNote.Rows[0];

                    SAndBCommissionDebitNoteDs.SAndBCommissionDebitNoteRow r = ds.SAndBCommissionDebitNote.NewSAndBCommissionDebitNoteRow();

                    r.InvoiceNo = row.InvoiceNo;
                    r.InvoiceDate = row.InvoiceDate;
                    r.ContractNo = row.ContractNo;
                    r.DeliveryNo = row.DeliveryNo;
                    r.OfficeName = row.OfficeName;
                    r.ItemNo = row.ItemNo;
                    r.PackingUnit = row.PackingUnit;
                    r.TotalShippedQty = row.TotalShippedQty;
                    r.TotalShippedAmt = row.TotalShippedAmt;
                    r.CurrencyCode = row.CurrencyCode;
                    r.NSLCommissionAmt = row.NSLCommissionAmt;
                    r.NSLCommissionPercent = row.NSLCommissionPercent;
                    r.SupplierNo = row.SupplierNo;
                    r.AccountName = row.AccountName;
                    r.SwiftCode = row.SwiftCode;
                    r.AccountNo = row.AccountNo;
                    r.BankAddress = row.BankAddress;
                    r.BankName = row.BankName;
                    r.CustomerDesc = row.CustomerDesc;
                    r.CustomerId = row.CustomerId;
                    if (!row.IsAddress1Null()) r.Address4 = row.Address1;
                    if (!row.IsAddress2Null()) r.Address4 = row.Address2;
                    if (!row.IsAddress3Null()) r.Address4 = row.Address3; 
                    if (!row.IsAddress4Null()) r.Address4 = row.Address4;
                    ds.SAndBCommissionDebitNote.Rows.Add(r);
                }

                debitNote.SetDataSource(ds.SAndBCommissionDebitNote.DataSet);
                /*
                debitNote.SetParameterValue("userId", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
                */
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "Invoice Commission Debit Note - Shipment Id : " + shipmentId.ToString());
                throw e;
            }
            return debitNote;
        }

        public SAndBCommissionDebitNote getCommissionDebitNote(int shipmentId)
        {
            SAndBCommissionDebitNote debitNote = new SAndBCommissionDebitNote();
            SAndBCommissionDebitNoteDs ds = new SAndBCommissionDebitNoteDs();

            try
            {
                ds = ReporterWorker.Instance.getCommissionDebitNote(shipmentId);
                debitNote.SetDataSource(ds.SAndBCommissionDebitNote.DataSet);
                /*
                debitNote.SetParameterValue("userId", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
                */
            }
            catch (Exception e)
            {
                NoticeHelper.sendErrorMessage(e, "Invoice Commission Debit Note - Shipment Id : " + shipmentId.ToString());
                throw e;
            }
            return debitNote;
        }

        public OtherCostSummaryRpt getOtherCostSummaryReport(DateTime InvoiceDateFrom, DateTime InvoiceDateTo, int FiscalYear, int PeriodFrom, int PeriodTo,
            DateTime DeliveryDateFrom, DateTime DeliveryDateTo, ArrayList OfficeList, string OfficeName, int HandlingOfficeId, string HandlingOfficeName, int CountryOfOriginId, 
            string CountryOfOriginName,  int SeasonId, string SeasonCode,  ArrayList TradingAgencyList, string TradingAgencyName, ArrayList PurchaseTermList, string PurchaseTermName,
            int BaseCurrencyId, string BaseCurrencyCode, int GetActual, int GetAccrual, int GetRealized, int GetNotYetCut, int UserId)
        {
            //int i;
            OtherCostSummaryRpt report = new OtherCostSummaryRpt();

            TypeCollector OfficeIdList = TypeCollector.Inclusive;
            for (int i = 0; i < OfficeList.Count; i++)
                OfficeIdList.append(Convert.ToInt16(OfficeList[i].ToString()));

            TypeCollector TradingAgencyIdList = TypeCollector.Inclusive;
            for (int i = 0; i < TradingAgencyList.Count; i++)
                TradingAgencyIdList.append(Convert.ToInt16(TradingAgencyList[i].ToString()));

            TypeCollector PurchaseTermIdList = TypeCollector.Inclusive;
            for (int i = 0; i < PurchaseTermList.Count; i++)
                PurchaseTermIdList.append(Convert.ToInt16(PurchaseTermList[i].ToString()));

            report.SetDataSource(ReporterWorker.Instance.getOtherCostSummary(InvoiceDateFrom, InvoiceDateTo, FiscalYear, PeriodFrom, PeriodTo,
                DeliveryDateFrom, DeliveryDateTo, OfficeIdList, HandlingOfficeId, CountryOfOriginId, SeasonId, TradingAgencyIdList, PurchaseTermIdList,
                GetActual, GetAccrual, GetRealized, GetNotYetCut, BaseCurrencyId));

            if (InvoiceDateFrom == DateTime.MinValue)
                report.SetParameterValue("InvoiceDateFrom", "");
            else
                report.SetParameterValue("InvoiceDateFrom", DateTimeUtility.getDateString(InvoiceDateFrom));

            if (InvoiceDateTo == DateTime.MinValue)
                report.SetParameterValue("InvoiceDateTo", "");
            else
                report.SetParameterValue("InvoiceDateTo", DateTimeUtility.getDateString(InvoiceDateTo));

            if (DeliveryDateFrom == DateTime.MinValue)
                report.SetParameterValue("DeliveryDateFrom", "");
            else
                report.SetParameterValue("DeliveryDateFrom", DateTimeUtility.getDateString(DeliveryDateFrom));

            if (DeliveryDateTo == DateTime.MinValue)
                report.SetParameterValue("DeliveryDateTo", "");
            else
                report.SetParameterValue("DeliveryDateTo", DateTimeUtility.getDateString(DeliveryDateTo));

            report.SetParameterValue("FiscalYear", FiscalYear);
            report.SetParameterValue("PeriodFrom", PeriodFrom);
            report.SetParameterValue("PeriodTo", PeriodTo);

            report.SetParameterValue("Office", OfficeName);
            report.SetParameterValue("HandlingOffice", HandlingOfficeName);
            report.SetParameterValue("CountryOfOrigin", CountryOfOriginName);
            report.SetParameterValue("Season", SeasonCode);
            report.SetParameterValue("TradingAgency", TradingAgencyName);
            report.SetParameterValue("PurchaseTerm", PurchaseTermName);

            string CutoffType;
            CutoffType = "";
            CutoffType += (GetActual == 1 ? (CutoffType == "" ? "" : ", ") + "Actual" : "");
            CutoffType += (GetAccrual == 1 ? (CutoffType == "" ? "" : ", ") + "Accrual" : "");
            CutoffType += (GetRealized == 1 ? (CutoffType == "" ? "" : ", ") + "Realized" : "");
            CutoffType += (GetNotYetCut == 1 ? (CutoffType == "" ? "" : ", ") + "Not Yet Cut" : "");
            report.SetParameterValue("CutoffType", CutoffType);
            report.SetParameterValue("BaseCurrencyCode", BaseCurrencyCode);

            report.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(UserId).DisplayName);
    
            return report;
        }


        public MonthEndSummaryRpt getMonthEndSummaryReport(int FiscalYear, int PeriodNo, int isSampleOrder,
            ArrayList OfficeList, string OfficeName, ArrayList TradingAgencyList, string TradingAgencyName, 
            ArrayList PurchaseTermList, string PurchaseTermName, string ExportFormat, int UserId)
        {
            MonthEndSummaryRpt report = new MonthEndSummaryRpt();

            TypeCollector OfficeIdList = TypeCollector.Inclusive;
            for (int i = 0; i < OfficeList.Count; i++)
                OfficeIdList.append(Convert.ToInt16(OfficeList[i].ToString()));

            TypeCollector TradingAgencyIdList = TypeCollector.Inclusive;
            for (int i = 0; i < TradingAgencyList.Count; i++)
                TradingAgencyIdList.append(Convert.ToInt16(TradingAgencyList[i].ToString()));

            TypeCollector PurchaseTermIdList = TypeCollector.Inclusive;
            for (int i = 0; i < PurchaseTermList.Count; i++)
                PurchaseTermIdList.append(Convert.ToInt16(PurchaseTermList[i].ToString()));


            report.SetDataSource(ReporterWorker.Instance.getMonthEndSummary(FiscalYear, PeriodNo, isSampleOrder,
                OfficeIdList, TradingAgencyIdList, PurchaseTermIdList).Tables[0]);  //, BaseCurrencyId).Tables[0]);

            report.SetParameterValue("FiscalYear", FiscalYear);
            report.SetParameterValue("PeriodNo", PeriodNo);
            report.SetParameterValue("Office", OfficeName);
            report.SetParameterValue("TradingAgency", TradingAgencyName);
            report.SetParameterValue("PurchaseTerm", PurchaseTermName);
            //report.SetParameterValue("BaseCurrencyCode", BaseCurrencyCode);
            report.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(UserId).DisplayName);
            report.SetParameterValue("ExportFormat", ExportFormat);

            //string orderType = "ALL";
            //if (isSampleOrder != -1) 
            //    orderType = (isSampleOrder == 1 ? "Mock Shop/Press Sample Order" : "Mainline Order");
            //report.SetParameterValue("OrderType", orderType);
            string orderType = ""; 
            if (isSampleOrder == -1)
                orderType = "ALL";
            else
                switch (isSampleOrder)
                {
                    case 0:
                        orderType = "Mainline Order";
                        break;
                    case 1:
                        orderType = "Mock Shop/Press/Studio Sample Order";
                        break;
                    case 2:
                        orderType = "Mock Shop Sample Order";
                        break;
                    case 3:
                        orderType = "Press Sample Order";
                        break;
                    case 4:
                        orderType = "Mainline And Press Sample Order";
                        break;
                    case 5:
                        orderType = "Studio Sample Order";
                        break;
                }
            report.SetParameterValue("OrderType", orderType);

            return report;
        }

        public ActiveSupplierReport getActiveSupplierReport(int officeGroupId, int productTeamId, DateTime pastDeliveryFrom,
            DateTime deliveryDateFrom, DateTime deliveryDateTo, int minDelivery, int vendorId, int orderType, 
            TypeCollector workflowStatusList, TypeCollector customerIdList, string officeGroupName, int userId)
        {
            ActiveSupplierReport report = new ActiveSupplierReport();
            ActiveSupplierReportDs ds = ReporterWorker.Instance.getActiveSupplierReport(officeGroupId, productTeamId, pastDeliveryFrom,
                deliveryDateFrom, deliveryDateTo, minDelivery, vendorId, orderType, customerIdList, workflowStatusList);
            report.SetDataSource(ds);
                       
            report.SetParameterValue("deliveryDateFrom", DateTimeUtility.getDateString(deliveryDateFrom));
            report.SetParameterValue("deliveryDateTo", DateTimeUtility.getDateString(deliveryDateTo));
            report.SetParameterValue("minDelivery", minDelivery.ToString());

            //report.SetParameterValue("office", officeId == -1 ? "ALL" : OfficeId.getName(officeId));
            report.SetParameterValue("office", officeGroupId == -1 ? "ALL" : officeGroupName);
            report.SetParameterValue("productTeam", productTeamId == -1 ? "ALL" : GeneralWorker.Instance.getProductCodeDefByKey(productTeamId).Code);
            report.SetParameterValue("supplier", vendorId == -1 ? "ALL" : IndustryUtil.getVendorByKey(vendorId).Name);

            if (orderType == -1)
                report.SetParameterValue("orderType", "ALL");
            else if (orderType == 0)
                report.SetParameterValue("orderType", "Mainline Order");
            else
                report.SetParameterValue("orderType", "Mock Shop / Press / Studio Sample");
            string nameList = "";
            foreach (object id in customerIdList.Values)
                if ((int)id != -1)
                    nameList += (nameList == "" ? "" : ", ") + commonWorker.getCustomerByKey((int)id).CustomerCode;
            if (nameList == "")
                nameList = "NIL";
            report.SetParameterValue("customer", nameList);

            nameList = "";
            foreach (int id in workflowStatusList.Values)
                if (id != -1)
                    nameList += (nameList == "" ? "" : ", ") + com.next.isam.domain.types.ContractWFS.getShortName(id);
            report.SetParameterValue("workflowStatus", nameList);
            return report;
        }

        public PaymentStatusEnquiryDs getPaymentStatusEnquiryDataSet(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, int handlingOfficeId, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime subDocDateFrom,
            DateTime subDocDateTo, DateTime interfaceDateFrom, DateTime interfaceDateTo, int paymentTermId, int tradingAgencyId, int vendorId,
            TypeCollector paymentStatusList, string contractNo)
        {
            return accountWorker.getPaymentStatusEnquiryDataSet(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, handlingOfficeId, invoiceDateFrom, invoiceDateTo, subDocDateFrom, subDocDateTo,
                interfaceDateFrom, interfaceDateTo, paymentTermId, tradingAgencyId, vendorId, paymentStatusList, contractNo);
        }

        public PaymentStatusEnquiryEpicorDs getPaymentStatusEnquiryEpicorDataSet(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, int handlingOfficeId, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime subDocDateFrom,
            DateTime subDocDateTo, DateTime interfaceDateFrom, DateTime interfaceDateTo, int paymentTermId, int tradingAgencyId, int vendorId,
            TypeCollector paymentStatusList, string contractNo)
        {
            return ReporterWorker.Instance.getPaymentStatusEnquiryEpicorDataSet(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, handlingOfficeId, invoiceDateFrom, invoiceDateTo, subDocDateFrom, subDocDateTo,
                interfaceDateFrom, interfaceDateTo, paymentTermId, tradingAgencyId, vendorId, paymentStatusList, contractNo);
        }


        public UKClaimSummaryReport getUKClaimSummaryReport(DateTime ukDNDateFrom, DateTime ukDNDateTo, int fiscalYear, int periodFrom, int periodTo, TypeCollector officeIdList, string officeDesc, int productTeamId, string productTeamDesc,
            int vendorId, string vendorDesc, TypeCollector claimTypeIdList, string claimTypeDesc, int claimReasonId, string claimReasonDesc, int printUserId, string reportCode)
        {
            UKClaimSummaryReport report = new UKClaimSummaryReport();
            UKClaimSummaryReportDs reportDs = ReporterWorker.Instance.getUKClaimReport(ukDNDateFrom, ukDNDateTo, fiscalYear, periodFrom, periodTo, officeIdList, productTeamId, vendorId, claimTypeIdList, claimReasonId);
            UKClaimSummaryReportDs.UKClaimReportRow row;
            TypeCollector requestIdList = TypeCollector.Inclusive;
            for (int i = 0; i < reportDs.UKClaimReport.Rows.Count; i++)
                if (!(row = (UKClaimSummaryReportDs.UKClaimReportRow)reportDs.UKClaimReport.Rows[i]).IsClaimRequestIdNull())
                    if (row.ClaimRequestId > 0)
                        requestIdList.append(row.ClaimRequestId);
            ReporterWorker.Instance.fillUKClaimRequest(reportDs, requestIdList, claimReasonId);

            report.SetDataSource(reportDs);
            report.SetParameterValue("UKDNDateFrom", DateTimeUtility.getDateString(ukDNDateFrom));
            report.SetParameterValue("UKDNDateTo", DateTimeUtility.getDateString(ukDNDateTo));
            report.SetParameterValue("Period", (fiscalYear == -1 ? "" : "Year " + fiscalYear.ToString() + "  P" + periodFrom.ToString() + " - P" + periodTo.ToString()));
            //report.SetParameterValue("Vendor", vendorId == -1 ? "" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
            report.SetParameterValue("Vendor", vendorDesc);
            //report.SetParameterValue("ProductTeam", productTeamId == -1 ? "" : GeneralWorker.Instance.getProductCodeDefByKey(productTeamId).CodeDescription);
            report.SetParameterValue("ProductTeam", productTeamDesc);
            //string officeDesc = "";
            //foreach (object id in officeIdList.Values)
            //    officeDesc += (officeDesc == "" ? "" : ",") + GeneralWorker.Instance.getOfficeRefByKey(Convert.ToInt32(id)).OfficeCode;
            report.SetParameterValue("Office", officeDesc);
            UserRef usr=GeneralWorker.Instance.getUserByKey(printUserId);
            report.SetParameterValue("PrintUser", usr == null ? "" : usr.DisplayName);
            report.SetParameterValue("ClaimType", claimTypeDesc);
            report.SetParameterValue("ClaimReason", claimReasonDesc);
            report.SetParameterValue("ReportCode", reportCode);

            return report;
        }

        public UKClaimPhasingByOfficeReport getUKClaimPhasingReportByOffice(int fiscalYear, int periodFrom, int periodTo, int vendorId, string vendorName, int userId, string reportCode)
        {
            UKClaimPhasingByOfficeReport rpt = new UKClaimPhasingByOfficeReport();
            UKClaimPhasingByOfficeDs ds = ReporterWorker.Instance.getUKClaimPhasingReportByOffice(fiscalYear, periodFrom, periodTo, vendorId);
            rpt.SetDataSource(ds);
            rpt.SetParameterValue("FiscalYear", fiscalYear.ToString());
            rpt.SetParameterValue("PeriodFrom", periodFrom.ToString());
            rpt.SetParameterValue("PeriodTo", periodTo.ToString());
            rpt.SetParameterValue("VendorName", vendorName);
            rpt.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            rpt.SetParameterValue("ReportCode", reportCode);

            return rpt;
        }

        public UKClaimPhasingByOfficeClaimReasonReport getUKClaimPhasingReportByOfficeClaimReason(int fiscalYear, int periodFrom, int periodTo, int vendorId, string vendorName, int userId, string reportCode)
        {
            return getUKClaimPhasingReportByOfficeClaimReason(fiscalYear, periodFrom, periodTo, vendorId, vendorName, -1, userId, reportCode);
        }

        public UKClaimPhasingByOfficeClaimReasonReport getUKClaimPhasingReportByOfficeClaimReason(int fiscalYear, int periodFrom, int periodTo, int vendorId, string vendorName, int officeId, int userId, string reportCode)
        {
            UKClaimPhasingByOfficeClaimReasonReport rpt = new UKClaimPhasingByOfficeClaimReasonReport();
            UKClaimPhasingByOfficeClaimReasonDs ds = ReporterWorker.Instance.getUKClaimPhasingReportByOfficeClaimReason(fiscalYear, periodFrom, periodTo, vendorId, officeId);
            rpt.SetDataSource(ds);
            rpt.SetParameterValue("FiscalYear", fiscalYear.ToString());
            rpt.SetParameterValue("PeriodFrom", periodFrom.ToString());
            rpt.SetParameterValue("PeriodTo", periodTo.ToString());
            rpt.SetParameterValue("VendorName", vendorName);
            rpt.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            rpt.SetParameterValue("ReportCode", reportCode);
            rpt.SetParameterValue("OfficeName", (officeId == -1 ? "ALL" : OfficeId.getName(officeId)));
            return rpt;
        }

        public UKClaimPhasingByOfficeClaimTypeReport getUKClaimPhasingReportByOfficeClaimType(int fiscalYear, int periodFrom, int periodTo, int vendorId, string vendorName, int userId, string reportCode)
        {
            return getUKClaimPhasingReportByOfficeClaimType(fiscalYear, periodFrom, periodTo, vendorId, vendorName, -1, userId, reportCode);
        }

        public UKClaimPhasingByOfficeClaimTypeReport getUKClaimPhasingReportByOfficeClaimType(int fiscalYear, int periodFrom, int periodTo, int vendorId, string vendorName, int officeId, int userId, string reportCode)
        {
            UKClaimPhasingByOfficeClaimTypeReport rpt = new UKClaimPhasingByOfficeClaimTypeReport();
            UKClaimPhasingByOfficeClaimReasonDs ds = ReporterWorker.Instance.getUKClaimPhasingReportByOfficeClaimType(fiscalYear, periodFrom, periodTo, vendorId, officeId);
            rpt.SetDataSource(ds);
            rpt.SetParameterValue("FiscalYear", fiscalYear.ToString());
            rpt.SetParameterValue("PeriodFrom", periodFrom.ToString());
            rpt.SetParameterValue("PeriodTo", periodTo.ToString());
            rpt.SetParameterValue("VendorName", vendorName);
            rpt.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            rpt.SetParameterValue("ReportCode", reportCode);
            rpt.SetParameterValue("OfficeName", (officeId == -1 ? "ALL" : OfficeId.getName(officeId)));
            return rpt;
        }

        /*
        public UKClaimAuditLogReport getUKClaimAuditLogReport(DateTime ukDNDateFrom, DateTime ukDNDateTo, TypeCollector officeIdList, string officeDesc, int productTeamId, string productTeamDesc,
            int vendorId, string vendorDesc, int claimTypeId, string claimTypeDesc, int printUserId)
        {
            UKClaimAuditLogReport rpt = new UKClaimAuditLogReport();
            UKClaimAuditLogReportDs ds = ReporterWorker.Instance.getUKClaimAuditLogReport(ukDNDateFrom, ukDNDateTo, officeIdList, productTeamId, vendorId, claimTypeId);
            rpt.SetDataSource(ds);

            rpt.SetDataSource(ds);
            rpt.SetParameterValue("UKDNDateFrom", DateTimeUtility.getDateString(ukDNDateFrom));
            rpt.SetParameterValue("UKDNDateTo", DateTimeUtility.getDateString(ukDNDateTo));
            rpt.SetParameterValue("Vendor", vendorDesc);
            rpt.SetParameterValue("ProductTeam", productTeamDesc);
            rpt.SetParameterValue("Office", officeDesc);
            UserRef usr = GeneralWorker.Instance.getUserByKey(printUserId);
            rpt.SetParameterValue("PrintUser", usr == null ? "" : usr.DisplayName);
            rpt.SetParameterValue("ClaimType", claimTypeDesc);
            return rpt;
        }
        */

        public UKClaimAuditLogReport getUKClaimAuditLogReport(DateTime ukDNDateFrom, DateTime ukDNDateTo, TypeCollector officeIdList, string officeDesc, int productTeamId, string productTeamDesc,
            int vendorId, string vendorDesc, int claimTypeId, string claimTypeDesc, int printUserId, string reportCode)
        {
            UKClaimAuditLogReport rpt = new UKClaimAuditLogReport();
            UKClaimAuditLogReportDs ds = ReporterWorker.Instance.getUKClaimAuditLogReport(ukDNDateFrom, ukDNDateTo, officeIdList, productTeamId, vendorId, claimTypeId);
            rpt.SetDataSource(ds);

            rpt.SetDataSource(ds);
            rpt.SetParameterValue("UKDNDateFrom", DateTimeUtility.getDateString(ukDNDateFrom));
            rpt.SetParameterValue("UKDNDateTo", DateTimeUtility.getDateString(ukDNDateTo));
            rpt.SetParameterValue("Vendor", vendorDesc);
            rpt.SetParameterValue("ProductTeam", productTeamDesc);
            rpt.SetParameterValue("Office", officeDesc);
            UserRef usr = GeneralWorker.Instance.getUserByKey(printUserId);
            rpt.SetParameterValue("PrintUser", usr == null ? "" : usr.DisplayName);
            rpt.SetParameterValue("ClaimType", claimTypeDesc);
            rpt.SetParameterValue("ReportCode", reportCode);

            return rpt;
        }

        public NonTradeExpenseStatementList getNonTradeExpenseStatementList(TypeCollector officeIdList, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo,
            string nsRefNoFrom, string nsRefNoTo, int ntVendorId, TypeCollector workflowStatusIdList, int userId)
        {
            string vendorName = string.Empty;
            string officeList = string.Empty;
            string wfsList = string.Empty;

            NonTradeExpenseStatementList rpt=new NonTradeExpenseStatementList();
            NonTradeExpenseStatementDs ds = ReporterWorker.Instance.getNonTradeExpenseStatementList(officeIdList, invoiceDateFrom, invoiceDateTo, dueDateFrom, dueDateTo, nsRefNoFrom, nsRefNoTo, ntVendorId, workflowStatusIdList);
            rpt.SetDataSource(ds);

            rpt.SetParameterValue("InvoiceDateFrom", (invoiceDateFrom == null ? "" : DateTimeUtility.getDateString(invoiceDateFrom)));
            rpt.SetParameterValue("InvoiceDateTo", (invoiceDateTo == null ? "" : DateTimeUtility.getDateString(invoiceDateTo)));
            rpt.SetParameterValue("DueDateFrom", (dueDateFrom == null ? "" : DateTimeUtility.getDateString(dueDateFrom)));
            rpt.SetParameterValue("DueDateTo", (dueDateTo == null ? "" : DateTimeUtility.getDateString(dueDateTo)));
            rpt.SetParameterValue("NSRefNoFrom", (nsRefNoFrom == null ? "" : nsRefNoFrom));
            rpt.SetParameterValue("NSRefNoTo", (nsRefNoTo == null ? "" : nsRefNoTo));

            if (ntVendorId > 0)
                vendorName = NonTradeWorker.Instance.getNTVendorByKey(ntVendorId).VendorName;
            rpt.SetParameterValue("VendorName", vendorName);

            foreach (int officeId in officeIdList.Values)
                officeList += (officeList == string.Empty ? string.Empty : ", ") + CommonUtil.getOfficeRefByKey(officeId).OfficeCode;
            rpt.SetParameterValue("OfficeList", officeList);

            bool includeAllStatus = true;
            foreach (NTWFS status in NTWFS.getCollectionValues())
                if (!workflowStatusIdList.contains(status.Id))
                    includeAllStatus = false;
            if (includeAllStatus)
                wfsList = "ALL";
            else
                foreach (int wfsId in workflowStatusIdList.Values)
                    wfsList += (wfsList == string.Empty ? string.Empty : ", ") + NTWFS.getType(wfsId).Name; 
            rpt.SetParameterValue("WorkflowStatusList", wfsList);

            UserRef usr = GeneralWorker.Instance.getUserByKey(userId);
            rpt.SetParameterValue("PrintUser", usr == null ? "" : usr.DisplayName);

            return rpt;
        }

        public NTVendorReport getNTVendorReport(int officeId, int ntVendorId, int expenseTypeId, int workflowStatusId)
        {
            NTVendorReport rpt = new NTVendorReport();
            NTVendorReportDs ds = ReporterWorker.Instance.getNTVendorReport(officeId, ntVendorId, expenseTypeId, workflowStatusId);
            rpt.SetDataSource(ds);

            rpt.SetParameterValue("Office", (officeId == -1 ? string.Empty : CommonUtil.getOfficeRefByKey(officeId).OfficeCode));
            rpt.SetParameterValue("NTVendor", (ntVendorId == -1 ? string.Empty : NonTradeWorker.Instance.getNTVendorByKey(ntVendorId).VendorName));
            rpt.SetParameterValue("ExpenseType", (expenseTypeId == -1 ? string.Empty : NonTradeWorker.Instance.getNTExpenseTypeByKey(expenseTypeId).ExpenseType));
            rpt.SetParameterValue("Status", (workflowStatusId == -1 ? string.Empty : NTVendorWFS.getType(workflowStatusId).Name));


            return rpt;
        }

        public NTInvoiceSettlementDs GenNTInvoiceDetailExportDataSet(string exportType, TypeCollector invoiceIdList, TypeCollector officeList, int expenseTypeId, int FiscalYear, int FiscalPeriodFrom, int FiscalPeriodTo, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo, DateTime settlementDateFrom, DateTime settlementDateTo, string invoiceNoFrom, string invoiceNoTo, string customerNoFrom, string customerNoTo, string nslRefNoFrom, string nslRefNoTo, int vendorId, TypeCollector workflowStatusIdList, int currencyId, int paymentMethodId, DateTime paymentDateFrom, DateTime paymentDateTo, string accountNoFrom, string accountNoTo, string dcIndicator, int firstApproverId)
        {
            return ReporterWorker.Instance.GetNTInvoiceDetailForSettlement(invoiceIdList);
        }

        #region NonTrade Recharge DC Note
        
        private NTRechargeDCNoteDs generateNTRechargeDCNoteDs(ArrayList ntRechargeDetailList, bool isDraft, DateTime dcNoteDate, int userId, ArrayList failList)
        {
            return ReporterWorker.Instance.generateNTRechargeDCNoteDs(ntRechargeDetailList, isDraft, dcNoteDate, userId, failList);
        }

        private NTRechargeDCNoteDs getNTRechargeDCNoteDs(NTRechargeDCNoteDef dcNote)
        {
            return ReporterWorker.Instance.getNTRechargeDCNoteDs(dcNote);
        }

        public NTRechargeDCNote generateNTRechargeDCNote(ArrayList ntRechargeDetailList, bool isDraft, DateTime dcNoteDate, int userId, ArrayList failList)
        {
            NTRechargeDCNote report = null;
            NTRechargeDCNoteDs ds = generateNTRechargeDCNoteDs(ntRechargeDetailList, isDraft, dcNoteDate, userId, failList);
            if (ds != null)
            {
                report = new NTRechargeDCNote();
                report.SetDataSource(ds);
            }
            return report;
        }

        public NTRechargeDCNote getNTRechargeDCNote(NTRechargeDCNoteDef dcNote)
        {
            NTRechargeDCNote report = new NTRechargeDCNote();
            NTRechargeDCNoteDs ds = getNTRechargeDCNoteDs(dcNote);
            report.SetDataSource(ds);
            return report;
        }

        public NTRechargeDCNote getNTRechargeDCNote(int dcNoteId)
        {
            return getNTRechargeDCNote(NonTradeWorker.Instance.getNTRechargeDCNoteByKey(dcNoteId));
        }

        public NTRechargeDCNote getNTRechargeDCNote(string dcNoteNo)
        {
            return getNTRechargeDCNote(NonTradeWorker.Instance.getNTRechargeDCNoteByDCNoteNo(dcNoteNo));
        }
        
        #endregion
        /*
        public ReportClass GenNTInvoiceDetailExportList(string exportType, TypeCollector invoiceIdList, TypeCollector officeList, int expenseTypeId,
            int FiscalYear, int FiscalPeriod, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo, 
            string invoiceNoFrom, string invoiceNoTo, string customerNoFrom, string customerNoTo, string nslRefNoFrom, string nslRefNoTo, int vendorId, TypeCollector workflowStatusIdList,
            int currencyId, int paymentMethodId, DateTime paymentDateFrom, DateTime paymentDateTo, string accountNoFrom, string accountNoTo, string dcIndicator)
        {
            return GenNTInvoiceDetailExportList(exportType, invoiceIdList, officeList, expenseTypeId,
            FiscalYear, FiscalPeriod, invoiceDateFrom, invoiceDateTo, dueDateFrom, dueDateTo, DateTime.MinValue, DateTime.MinValue,
            invoiceNoFrom, invoiceNoTo, customerNoFrom, customerNoTo, nslRefNoFrom, nslRefNoTo, vendorId, workflowStatusIdList,
            currencyId, paymentMethodId, paymentDateFrom, paymentDateTo, accountNoFrom, accountNoTo, dcIndicator, -1);
        }
        */
        public ReportClass GenNTInvoiceDetailExportList(string exportType, TypeCollector invoiceIdList, TypeCollector officeList, int expenseTypeId, 
            int FiscalYear, int FiscalPeriodFrom, int FiscalPeriodTo, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo, DateTime settlementDateFrom, DateTime settlementDateTo,
            string invoiceNoFrom, string invoiceNoTo, string customerNoFrom, string customerNoTo, string nslRefNoFrom, string nslRefNoTo, int vendorId, TypeCollector workflowStatusIdList,
            int currencyId, int paymentMethodId, DateTime paymentDateFrom, DateTime paymentDateTo, string accountNoFrom, string accountNoTo, string dcIndicator, int firstApproverId)
        {
            NTInvoiceSettlementDs dataset = null;
            dataset = reporter.dataserver.ReporterWorker.Instance.GetNTInvoiceDetailForSettlement(invoiceIdList);
            if (dataset != null)
            {
                ReportClass rpt;
                if (exportType == "SEARCH")
                    rpt = new NTInvoiceDetailExportList();
                else
                    rpt = new NTInvoiceSettlementDetailList();

                rpt.SetDataSource(dataset);
                rpt.SetParameterValue("ReportType", exportType);
                string offices = string.Empty;
                foreach (int officeId in officeList.Values)
                    offices += (offices == string.Empty ? string.Empty : ", ") + OfficeId.getName(officeId);
                rpt.SetParameterValue("Office", offices);
                rpt.SetParameterValue("SupplierInvoiceAccountNo", accountNoFrom + (string.IsNullOrEmpty(accountNoTo) ? string.Empty : " - " + accountNoTo));
                rpt.SetParameterValue("DueDate", (dueDateFrom == DateTime.MinValue ? string.Empty : dueDateFrom.ToString("dd/MM/yyyy")) + (dueDateTo == DateTime.MinValue ? string.Empty : " - " + dueDateTo.ToString("dd/MM/yyyy")));
                rpt.SetParameterValue("Vendor", (vendorId > 0 ? nontradeWorker.getNTVendorByKey(vendorId).VendorName : string.Empty));
                rpt.SetParameterValue("PaymentMethod", (paymentMethodId == -1 ? "ALL" : NTPaymentMethodRef.getType(paymentMethodId).Name));
                rpt.SetParameterValue("Currency", (currencyId == GeneralCriteria.ALL ? "ALL" : CurrencyId.getName(currencyId)));
                rpt.SetParameterValue("InvoiceDate", (invoiceDateFrom == DateTime.MinValue ? string.Empty : invoiceDateFrom.ToString("dd/MM/yyyy") + " - " + invoiceDateTo.ToString("dd/MM/yyyy")));
                rpt.SetParameterValue("SettlementDate", (settlementDateFrom == DateTime.MinValue ? string.Empty : settlementDateFrom.ToString("dd/MM/yyyy") + " - " + settlementDateTo.ToString("dd/MM/yyyy")));
                rpt.SetParameterValue("InvoiceNo", string.Empty);
                rpt.SetParameterValue("CustomerNo", string.Empty);
                rpt.SetParameterValue("NSLRefNo", string.Empty);
                rpt.SetParameterValue("ExpenseType", string.Empty);
                string period = string.Empty;
                if (FiscalYear > 0 && FiscalPeriodFrom > 0 && FiscalPeriodTo > 0)
                {
                    period = FiscalYear.ToString() + " P";
                    period += (FiscalPeriodFrom <= FiscalPeriodTo ? FiscalPeriodFrom.ToString() : FiscalPeriodTo.ToString());
                    if (FiscalPeriodFrom != FiscalPeriodTo)
                        period += " - " + (FiscalPeriodFrom < FiscalPeriodTo ? FiscalPeriodTo.ToString() : FiscalPeriodFrom.ToString());
                }
                rpt.SetParameterValue("FiscalPeriod", period);
                string status = string.Empty;
                foreach (int wfs in workflowStatusIdList.Values)
                    status += (status == string.Empty ? string.Empty : ",") + NTWFS.getType(wfs).Name;
                rpt.SetParameterValue("Status", (status == string.Empty ? "NONE" : status));
                rpt.SetParameterValue("DocumentType", (dcIndicator == string.Empty ? "ALL" : (dcIndicator == "D" ? "Invoice" : (dcIndicator == "C" ? "Credit Note" : string.Empty))));
                rpt.SetParameterValue("FirstApprover", (firstApproverId > 0 ? CommonUtil.getUserByKey(firstApproverId).DisplayName : string.Empty));
                return rpt;
            }
            else
                return null;
        }

        public CutoffSalesDiscrepancyReport getCutoffSalesDiscrepancyReport(int officeId, int fiscalYear, int period, int userId)
        {
            AccountFinancialCalenderDef cal = CommonUtil.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, period);
            CutoffSalesDiscrepancyReportDs dataset = ReporterWorker.Instance.getCutoffSalesDiscrepancyReport(officeId, cal.StartDate, cal.EndDate);
            CutoffSalesDiscrepancyReport report = new CutoffSalesDiscrepancyReport();
            report.SetDataSource(dataset);
            report.SetParameterValue("FiscalYear", fiscalYear.ToString());
            report.SetParameterValue("Period", period.ToString());
            report.SetParameterValue("StartDate", DateTimeUtility.getDateString(cal.StartDate));
            report.SetParameterValue("EndDate", DateTimeUtility.getDateString(cal.EndDate));
            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            return report;
        }

        public MonthEndSlippageReport getMonthEndSlippageReport(string officeCode, int fiscalYear, int period, int userId)
        {
            AccountFinancialCalenderDef cal = CommonUtil.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, period);
            MonthEndShipmentDs dataset = ReporterWorker.Instance.getMonthEndSlippageReport(officeCode, fiscalYear, period);
            MonthEndSlippageReport report = new MonthEndSlippageReport();
            report.SetDataSource(dataset);
            report.SetParameterValue("OfficeCode", officeCode);
            report.SetParameterValue("FiscalYear", fiscalYear.ToString());
            report.SetParameterValue("Period", period.ToString());
            report.SetParameterValue("StartDate", DateTimeUtility.getDateString(cal.StartDate));
            report.SetParameterValue("EndDate", DateTimeUtility.getDateString(cal.EndDate));
            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            return report;
        }

        public string GetNUKSalesCutOffSummary(int fiscalYear, int period, DateTime periodStartDate, DateTime periodEndDate)
        {
            NUKSalesCutOffSummary report = new NUKSalesCutOffSummary();
            NUKSalesCutOffSummaryDs ds = ReporterWorker.Instance.GetNUKSalesCutOffSummary(fiscalYear, period, periodStartDate, periodEndDate);

            string filePath = WebConfig.getValue("appSettings", "MONTH_END_FOLDER");
            filePath += "\\NUKSales\\";
            filePath += string.Format("{0} P{1} Shipment List.xls", fiscalYear.ToString(), period.ToString());

            report.SetDataSource(ds);
            report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, filePath);
            report.Close();
            report.Dispose();

            return filePath;
        }

        public string GetNUKSalesShippingReport(int fiscalYear, int period, TypeCollector officeList, DateTime ilsActualAWHDateFrom, DateTime ilsActualAWHDateTo,
            DateTime periodStartDate, DateTime periodEndDate)
        {
            NUKSalesShippingReport report = new NUKSalesShippingReport();
            NUKSalesShippingReportDs ds = ReporterWorker.Instance.GetNUKSalesShippingReport(fiscalYear, period, officeList, ilsActualAWHDateFrom, ilsActualAWHDateTo, periodStartDate, periodEndDate);

            string filePath = WebConfig.getValue("appSettings", "MONTH_END_FOLDER");
            filePath += "\\NUKSales\\";
            filePath += string.Format("NUKSales {0}-{1}.xls", ilsActualAWHDateFrom.ToString("MMM dd"), ilsActualAWHDateTo.ToString("MMM dd"));

            report.SetDataSource(ds);
            report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, filePath);
            report.Close();
            report.Dispose();

            return filePath;
        }

        public ReportClass getEpicorInterfaceLogReport(DateTime interfaceDateFrom, DateTime interfaceDateTo, ArrayList officeList, int userId)
        {
            EpicorInterfaceLogReport report = new EpicorInterfaceLogReport();
            EpicorInterfaceLogReportDs reportDs;

            TypeCollector officeIdList = TypeCollector.Inclusive;
            string selectedOffice = string.Empty;
            foreach (ListItem itm in officeList)
            {
                selectedOffice += (selectedOffice == string.Empty ? "" : ", ") + itm.Text.Replace("Office", string.Empty).TrimEnd();
                officeIdList.append(int.Parse(itm.Value));
            }
            reportDs = ReporterWorker.Instance.getEpicorInterfaceLogReport(interfaceDateFrom, interfaceDateTo, officeIdList);
            report.SetDataSource(reportDs);

            // Setting report parameters
            report.SetParameterValue("InterfaceDateFrom", DateTimeUtility.getDateString(interfaceDateFrom));
            report.SetParameterValue("InterfaceDateTo", DateTimeUtility.getDateString(interfaceDateTo));
            report.SetParameterValue("Office", selectedOffice);
            report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return report;
        }

        public AdvancePaymentReport getAdvancePaymentReport(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, string vendorName, int officeId, string officeName, int paymentstatusIndex, string paymentstatusString, string exportType)
        {
            string daterange = string.Empty;

            AdvancePaymentReport rpt = new AdvancePaymentReport();
            AdvancePaymentReportDs ds = ReporterWorker.Instance.getAdvancePaymentReport(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, officeId, paymentstatusIndex);

            AdvancePaymentSummaryReportDs summaryds = ReporterWorker.Instance.getAdvancePaymentSummaryReport(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, officeId, paymentstatusIndex);
            rpt.Subreports[0].SetDataSource(summaryds);
            rpt.Subreports[1].SetDataSource(summaryds);
            rpt.SetDataSource(ds);

            rpt.SetParameterValue("Office", officeName);
            rpt.SetParameterValue("Supplier", vendorName);
            rpt.SetParameterValue("PaymentStatus", paymentstatusString);
            if (paymentDateFrom == DateTime.MinValue && paymentDateTo == DateTime.MinValue)
            {
                daterange = "ALL";
            }
            else
            {
                daterange = DateTimeUtility.getDateString(paymentDateFrom) + " To " + DateTimeUtility.getDateString(paymentDateTo);
            }
            rpt.SetParameterValue("PaymentDate", daterange);

            if (deductionDateFrom == DateTime.MinValue && deductionDateTo == DateTime.MinValue)
            {
                daterange = "ALL";
            }
            else
            {
                daterange = DateTimeUtility.getDateString(deductionDateFrom) + " To " + DateTimeUtility.getDateString(deductionDateTo);
            }
            rpt.SetParameterValue("DeductionDate", daterange);
            rpt.SetParameterValue("ExportType", exportType);
            return rpt;
        }

        /// <summary>
        /// Management ver.
        /// </summary>
        public AdvancePaymentReportMG getAdvancePaymentReportMG(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, string vendorName, int officeId, string officeName, int paymentstatusIndex, string paymentstatusString, string exportType)
        {
            string daterange = string.Empty;

            AdvancePaymentReportMG rpt = new AdvancePaymentReportMG();
            AdvancePaymentReportDs ds = ReporterWorker.Instance.getAdvancePaymentReportMG(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, officeId, paymentstatusIndex);

            AdvancePaymentSummaryReportDs summaryds = ReporterWorker.Instance.getAdvancePaymentSummaryReportMG(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, officeId, paymentstatusIndex);
            AdvancePaymentSummaryReportDs summary2ds = ReporterWorker.Instance.getAdvancePaymentSummaryReportMGThisYear(officeId);
            AdvancePaymentSummaryReportDs summary3ds = ReporterWorker.Instance.getAdvancePaymentSummaryReportMGPreviousYear(officeId);
            rpt.Subreports[0].SetDataSource(summaryds);
            rpt.Subreports[1].SetDataSource(summaryds);
            rpt.Subreports[2].SetDataSource(summary2ds);
            rpt.Subreports[3].SetDataSource(summary3ds);
            rpt.SetDataSource(ds);

            rpt.SetParameterValue("all", " all", "AdvancePaymentSummaryReportMG3.rpt");
            rpt.SetParameterValue("all", "", "AdvancePaymentSummaryReportMG3.rpt - 01");
            rpt.SetParameterValue("Office", officeName);
            rpt.SetParameterValue("Supplier", vendorName);
            rpt.SetParameterValue("PaymentStatus", paymentstatusString);
            if (paymentDateFrom == DateTime.MinValue && paymentDateTo == DateTime.MinValue)
            {
                daterange = "ALL";
            }
            else
            {
                daterange = DateTimeUtility.getDateString(paymentDateFrom) + " To " + DateTimeUtility.getDateString(paymentDateTo);
            }
            rpt.SetParameterValue("PaymentDate", daterange);

            if (deductionDateFrom == DateTime.MinValue && deductionDateTo == DateTime.MinValue)
            {
                daterange = "ALL";
            }
            else
            {
                daterange = DateTimeUtility.getDateString(deductionDateFrom) + " To " + DateTimeUtility.getDateString(deductionDateTo);
            }
            rpt.SetParameterValue("DeductionDate", daterange);
            rpt.SetParameterValue("ExportType", exportType);

            var grantTotal = from ap in ds.AdvancePaymentReport
                             group ap by ap.paymentno into aggregation
                             select new
                             {
                                 GTTotalAdvPaymentAmt = aggregation.Max(
                                    i => (i.advance_amt - i.InterestAmt)
                                 ),
                                 GTAdvance_amt = aggregation.Max(
                                    i => i.advance_amt
                                 ),
                                 GTInterestAmt = aggregation.Max(
                                    i => i.InterestAmt
                                 )
                             };
            grantTotal = grantTotal.ToList();
            string strGTTotalAdvPaymentAmt = "";
            string strGTAdvance_amt = "";
            string strGTInterestAmt = "";
            string specifier = "#,#.##";
            if (grantTotal.Any())
            {
                strGTTotalAdvPaymentAmt = grantTotal.Sum(gt => gt.GTTotalAdvPaymentAmt).ToString(specifier);
                strGTAdvance_amt = grantTotal.Sum(gt => gt.GTAdvance_amt).ToString(specifier);
                strGTInterestAmt = grantTotal.Sum(gt => gt.GTInterestAmt).ToString(specifier);
            }
            rpt.SetParameterValue("GTTotalAdvPaymentAmt", strGTTotalAdvPaymentAmt);
            rpt.SetParameterValue("GTAdvance_amt", strGTAdvance_amt);
            rpt.SetParameterValue("GTInterestAmt", strGTInterestAmt);
            return rpt;
        }

        public AdvancePaymentReportDs getAdvancePaymentReportMGDataSet(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, string vendorName, int officeId, string officeName, int paymentstatusIndex, string paymentstatusString)
        {
            string daterange = string.Empty;

            AdvancePaymentReportMG rpt = new AdvancePaymentReportMG();
            AdvancePaymentReportDs ds = ReporterWorker.Instance.getAdvancePaymentReportMG(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, officeId, paymentstatusIndex);

            return ds;
        }

        public AdvancePaymentSummaryReportDs getAdvancePaymentSummaryReportMG(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, int officeId, int paymentstatusIndex)
        {
            return ReporterWorker.Instance.getAdvancePaymentSummaryReportMG(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, officeId, paymentstatusIndex);
        }

        public AdvancePaymentSummaryReportDs getAdvancePaymentSummaryReportMGThisYear(int officeId)
        {
            return ReporterWorker.Instance.getAdvancePaymentSummaryReportMGThisYear(officeId);
        }

        public AdvancePaymentSummaryReportDs getAdvancePaymentSummaryReportMGPreviousYear(int officeId)
        {
            return ReporterWorker.Instance.getAdvancePaymentSummaryReportMGPreviousYear(officeId);
        }

        public string getAdvancePaymentFirstContractDate(int paymentId)
        {
            return ReporterWorker.Instance.getAdvancePaymentFirstContractDate(paymentId);
        }

        public string getAdvancePaymentLastContractDate(int paymentId)
        {
            return ReporterWorker.Instance.getAdvancePaymentLastContractDate(paymentId);
        }

        public LGHoldPaymentDs getLGHoldPaymentList(int officeId, string lgNo, int vendorId, string itemNo, string contractNo, int paymentStatusId)
        {
            return ReporterWorker.Instance.getLGHoldPaymentList(officeId, lgNo, vendorId, itemNo, contractNo, paymentStatusId);
        }

        public POListByOfficeSupplierReportDs getPOListByOfficeSupplierReport(int fiscalYear, int fiscalPeriod, int officeId, int vendorId, int currencyId, TypeCollector paymentStatusList)
        {
            return ReporterWorker.Instance.getPOListByOfficeSupplierReport(fiscalYear, fiscalPeriod, officeId, vendorId, currencyId, paymentStatusList);
        }

        public ILSDiffDCNoteReport getILSDiffDCNote(ArrayList debitNoteList, ArrayList debitNoteShipmentList, int officeId, int fiscalYear, int period, bool isDraft)
        {
            ReporterWorker reportWorker = ReporterWorker.Instance;
            ILSDiffDCNoteReport report = new ILSDiffDCNoteReport();
            ILSDiffDCNoteReportDs ds = new ILSDiffDCNoteReportDs();
            ILSDiffDCNoteSummaryDs sumDs = new ILSDiffDCNoteSummaryDs();
            ILSDiffDCNoteReportDs.ILSDiffDCNoteReportRow row = null;
            ILSDiffDCNoteSummaryDs.ILSDiffDCNoteSummaryRow shipmentRow = null;
            OrderSelectWorker orderSelectWorker = OrderSelectWorker.Instance;
            ProductWorker productWorker = ProductWorker.Instance;
            ShippingWorker shippingWorker = ShippingWorker.Instance;

            ContractDef contractDef = null;
            InvoiceDef invoiceDef = null;
            AccountFinancialCalenderDef cDef = generalWorker.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, period);

            if (debitNoteList.Count == 0)
            {
                report.SetDataSource(ds);
                report.Subreports[0].SetDataSource(sumDs);
                report.SetParameterValue("period", period);
                report.SetParameterValue("year", fiscalYear);
                report.SetParameterValue("periodEndDate", cDef.EndDate);
                report.SetParameterValue("isDraft", isDraft);
                return report;
            }

            foreach (ILSDiffDCNoteDef ils in debitNoteList)
            {

                OfficeRef office = GeneralWorker.Instance.getOfficeRefByKey(officeId);


                row = ds.ILSDiffDCNoteReport.NewILSDiffDCNoteReportRow();

                row.TotalDiffAmt = ils.TotalDiffAmt;
                row.TotalSalesDiffAmt = ils.TotalSalesDiffAmt;
                row.TotalSalesCommissionDiffAmt = ils.TotalSalesCommissionDiffAmt;
                row.SellCurrency = GeneralWorker.Instance.getCurrencyByKey(ils.CurrencyId).CurrencyCode;
                row.Office = office.Description.ToUpper();
                row.OfficeId = office.OfficeId;
                row.DCNoteNo = ils.DCNoteNo;
                row.DCNoteId = ils.DCNoteId;
                row.DCNoteDate = ils.DCNoteDate;

                DebitNoteToNUKParamDef paramDef = reportWorker.getDebitNoteToNUKParamByKey(officeId, ils.CurrencyId);
                row.BenificiaryAccountNo = reportWorker.getBeneficiaryAccountNoList(officeId, ils.CurrencyId);
                row.SupplierCode = paramDef.SupplierCode;
                row.BeneficiaryName = paramDef.BeneficiaryName;
                row.BankName = paramDef.BankName;
                row.BankAddress = paramDef.BankAddress;
                row.SwiftCode = paramDef.SwiftCode;

                ds.ILSDiffDCNoteReport.AddILSDiffDCNoteReportRow(row);

                foreach (ILSDiffDCNoteShipmentDef shipment in debitNoteShipmentList)
                {
                    invoiceDef = shippingWorker.getInvoiceByKey(shipment.ShipmentId);

                    contractDef = orderSelectWorker.getContractByKey(shipment.ContractId);
                    shipmentRow = sumDs.ILSDiffDCNoteSummary.NewILSDiffDCNoteSummaryRow();
                    shipmentRow.DCNoteId = shipment.DCNoteId;
                    shipmentRow.DCNoteShipmentId = shipment.DCNoteShipmentId;
                    shipmentRow.DCNoteNo = row.DCNoteNo;
                    shipmentRow.ContractNo = contractDef.ContractNo;
                    shipmentRow.ItemNo = productWorker.getProductByKey(shipment.ProductId).ItemNo;
                    shipmentRow.ShipmentId = shipment.ShipmentId;
                    shipmentRow.TransDate = cDef.EndDate;
                    shipmentRow.InvoiceNo = ShippingWorker.getInvoiceNo(invoiceDef.InvoicePrefix, invoiceDef.InvoiceSeqNo, invoiceDef.InvoiceYear);
                    shipmentRow.InvoiceDate = invoiceDef.InvoiceDate;
                    shipmentRow.ProdCode = generalWorker.getProductCodeDefByKey(contractDef.ProductTeam.ProductCodeId).Code;

                    ArrayList qtyDetailList = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(shipment.ShipmentId);
                    int qty = 0;
                    foreach (ShipmentDetailDef temp in qtyDetailList)
                    {
                        qty += temp.ShippedQuantity;
                    }
                    shipmentRow.Qty = qty;
                    shipmentRow.NSSAmt = shipment.NSSAmt;
                    shipmentRow.RecdAmt = shipment.ReceivedAmt;
                    shipmentRow.CurrencyCode = GeneralWorker.Instance.getCurrencyByKey(shipment.CurrencyId).CurrencyCode;
                    shipmentRow.ILSDiff = shipment.ILSDiffAmt;
                    shipmentRow.ILSType = shipment.ILSType;
                    sumDs.ILSDiffDCNoteSummary.AddILSDiffDCNoteSummaryRow(shipmentRow);
                }
            }

            report.Subreports[0].SetDataSource(sumDs);
            report.SetDataSource(ds);
            report.SetParameterValue("period", period);
            report.SetParameterValue("year", fiscalYear);
            report.SetParameterValue("periodEndDate", cDef.EndDate);
            report.SetParameterValue("isDraft", isDraft);

            return report;

        }

        public AdvPaymentInstalmentInterestChargesReport getAdvancePaymentInterestChargeReport(int paymentId, DateTime paymentDate)
        {
            ReporterWorker reportWorker = ReporterWorker.Instance;
            AdvancePaymentDef paymentDef = AdvancePaymentWorker.Instance.getAdvancePaymentByKey(paymentId);
            AdvancePaymentInstalmentDetailDef instalmentDef = AdvancePaymentWorker.Instance.getAdvancePaymentInstalmentDetailByKey(paymentId, paymentDate);
            AdvPaymentInstalmentInterestChargesReport report = new AdvPaymentInstalmentInterestChargesReport();
            DebitNoteToNUKParamDef paramDef = reportWorker.getDebitNoteToNUKParamByKey(paymentDef.OfficeId, paymentDef.Currency.CurrencyId);
            VendorRef vendor = VendorWorker.Instance.getVendorByKey(paymentDef.Vendor.VendorId);
            bool isDebit = instalmentDef.InterestAmt >= 0 ? true : false;

            report.SetParameterValue("paymentNo", paymentDef.PaymentNo);
            report.SetParameterValue("vendorName", vendor.Name);
            report.SetParameterValue("vendorAddr1", vendor.Address0);
            report.SetParameterValue("vendorAddr2", vendor.Address1);
            report.SetParameterValue("vendorAddr3", vendor.Address2);
            report.SetParameterValue("vendorAddr4", vendor.Address3);
            report.SetParameterValue("vendorCountry", vendor.CountryDescription);
            report.SetParameterValue("dcNoteDate", DateTimeUtility.getDateString(instalmentDef.DCNoteDate));
            report.SetParameterValue("dcNoteNo", instalmentDef.DCNoteNo);
            report.SetParameterValue("dcNoteType", isDebit ? "DEBIT NOTE" : "CREDIT NOTE");
            report.SetParameterValue("office", generalWorker.getOfficeRefByKey(paymentDef.OfficeId).Description);
            report.SetParameterValue("periodFrom", DateTimeUtility.getDateString(instalmentDef.InterestFromDate));
            report.SetParameterValue("periodTo", DateTimeUtility.getDateString(instalmentDef.InterestToDate));
            report.SetParameterValue("interestDays", (instalmentDef.InterestToDate.Subtract(instalmentDef.InterestFromDate).Days + 1).ToString());
            report.SetParameterValue("remainingTotal", Math.Round(instalmentDef.RemainingTotalAmt, 2));
            report.SetParameterValue("rate", Math.Round(instalmentDef.InterestRate, 2));
            report.SetParameterValue("currency", paymentDef.Currency.CurrencyCode);
            report.SetParameterValue("bankName", paramDef.BeneficiaryName);
            report.SetParameterValue("banker", paramDef.BankName);
            report.SetParameterValue("bankAddr", paramDef.BankAddress);
            report.SetParameterValue("swift", paramDef.SwiftCode);
            report.SetParameterValue("bankAccountNo", reportWorker.getBeneficiaryAccountNoList(paymentDef.OfficeId, paymentDef.Currency.CurrencyId));
            report.SetParameterValue("interestCharges", (Math.Abs(Math.Round(instalmentDef.InterestAmt, 2))).ToString("#,##0.00"));

            return report;
        }

        public GenericDCNoteReport getGenericDCNoteReport(List<string> dcNoteNoList)
        {
            ReporterWorker reportWorker = ReporterWorker.Instance;
            GenericDCNoteReport report = new GenericDCNoteReport();
            GenericDCNoteReportDs ds = new GenericDCNoteReportDs();
            GenericDCNoteReportDs.GenericDCNoteReportRow row = null;

            foreach (string noteNo in dcNoteNoList)
            {
                GenericDCNoteDef def = accountWorker.getGenericDCNoteByNoteNo(noteNo);
                row = ds.GenericDCNoteReport.NewGenericDCNoteReportRow();

                row.DCNoteNo = def.DCNoteNo;
                row.DCNoteDate = def.DCNoteDate;
                row.Office = GeneralWorker.Instance.getOfficeRefByKey(def.OfficeId).Description;
                row.DebitCreditIndicator = def.DebitCreditIndicator;
                row.Description = def.Description;
                row.OriginalCurrencyCode = GeneralWorker.Instance.getCurrencyByKey(def.OriginalCurrencyId).CurrencyCode;
                row.OriginalAmount = def.OriginalAmount;
                row.BillingCurrencyCode = GeneralWorker.Instance.getCurrencyByKey(def.BillingCurrencyId).CurrencyCode;
                row.Amount = def.Amount;
                row.PartyName = def.PartyName;
                row.PartyAddress1 = def.PartyAddress1;
                row.PartyAddress2 = def.PartyAddress2;
                row.PartyAddress3 = def.PartyAddress3;
                row.PartyAddress4 = def.PartyAddress4;
                row.Attn = def.Attn;

                DebitNoteToNUKParamDef paramDef = reportWorker.getDebitNoteToNUKParamByKey(def.OfficeId, def.BillingCurrencyId);
                row.BenificiaryAccountNo = reportWorker.getBeneficiaryAccountNoList(def.OfficeId, def.BillingCurrencyId);
                row.SupplierCode = paramDef.SupplierCode;
                row.BeneficiaryName = paramDef.BeneficiaryName;
                row.BankName = paramDef.BankName;
                row.BankAddress = paramDef.BankAddress;
                row.SwiftCode = paramDef.SwiftCode;

                ds.GenericDCNoteReport.AddGenericDCNoteReportRow(row);
            }
            report.SetDataSource(ds);
            return report;
        }

    }
}
