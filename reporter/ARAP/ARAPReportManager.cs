using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using com.next.isam.dataserver.worker;
using com.next.isam.reporter.dataserver;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.infra.util;

namespace com.next.isam.reporter.ARAP
{
    public class ARAPReportManager
    {
        private static ARAPReportManager _instance;
        private AccountWorker accountWorker;

        public ARAPReportManager()
		{
            accountWorker = AccountWorker.Instance;
		}

        public static ARAPReportManager Instance
		{
			get 
			{
				if (_instance == null)
				{
                    _instance = new ARAPReportManager();
				}
				return _instance;
			}
		}


        public OutstandingTradeReport getARReport(DateTime cutoffDate, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo,
            int budgetYear, int periodFrom, int periodTo, int currencyId, int baseCurrencyId, int exchangeRateYear, int exchangeRatePeriod, string payRefCode, int termOfPurchaseId, string orderType, int officeId,
            TypeCollector officeIdList, int handlingOfficeId, TypeCollector productTeamList, int vendorId, int isSZOrder, int isUTOrder, int isOPROrder, TypeCollector customerIdList, TypeCollector tradingAgencyList, int sampleOrderGroup, 
            int dataType, int reportVersion, int userId)
        {
            OutstandingTradeReport report = null;
            OutstandingTradeReportDs reportDs = new OutstandingTradeReportDs();
            CommonWorker commonWorker = CommonWorker.Instance;

            reportDs = ReporterWorker.Instance.getARListByCriteria(cutoffDate, invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo,
            budgetYear, periodFrom, periodTo, currencyId, baseCurrencyId, exchangeRateYear, exchangeRatePeriod, payRefCode, termOfPurchaseId, orderType, officeId,
            officeIdList, handlingOfficeId, productTeamList, vendorId, isSZOrder, isUTOrder, isOPROrder, customerIdList, tradingAgencyList, sampleOrderGroup, dataType, reportVersion);

            bool missingExchangeRate = false;
            foreach (OutstandingTradeReportDs.OutstandingTradeReportRow row in reportDs.Tables[0].Rows)
                if (row.OtherAmount > 0 && row.BaseAmount == 0)
                {   // Exchange rate is undefined
                    missingExchangeRate = true;
                    break;
                }

            if (!missingExchangeRate)
            {
                report = new OutstandingTradeReport();
                report.SetDataSource(reportDs);

                report.SetParameterValue("InvoiceDateFrom", DateTimeUtility.getDateString(invoiceDateFrom));
                report.SetParameterValue("InvoiceDateTo", DateTimeUtility.getDateString(invoiceDateTo));
                report.SetParameterValue("InvoiceUploadDateFrom", DateTimeUtility.getDateString(invoiceUploadDateFrom));
                report.SetParameterValue("InvoiceUploadDateTo", DateTimeUtility.getDateString(invoiceUploadDateTo));
                report.SetParameterValue("AccReceiptDateFrom", "");
                report.SetParameterValue("AccReceiptDateTo", "");
                string officeList = string.Empty;
                foreach (int id in officeIdList.Values)
                    officeList += (officeList == string.Empty ? string.Empty : ", ") + GeneralWorker.Instance.getOfficeRefByKey(id).OfficeCode;
                report.SetParameterValue("Office", officeIdList.Values.Count == 0 ? "" : officeList);
                report.SetParameterValue("HandlingOffice", handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).OfficeCode);

                report.SetParameterValue("Currency", currencyId == -1 ? "" : GeneralWorker.Instance.getCurrencyByKey(currencyId).CurrencyCode);
                report.SetParameterValue("BaseCurrency", GeneralWorker.Instance.getCurrencyByKey(baseCurrencyId).CurrencyCode);

                report.SetParameterValue("PayRefCode", payRefCode);

                if (reportDs.OutstandingTradeReport.Rows.Count != 0)
                {
                    report.SetParameterValue("TermOfPurchase", termOfPurchaseId == -1 ? "" : reportDs.OutstandingTradeReport.Rows[0]["TermOfPurchase"].ToString());
                }
                else
                {
                    report.SetParameterValue("TermOfPurchase", termOfPurchaseId == -1 ? "" : commonWorker.getTermOfPurchaseByKey(termOfPurchaseId).TermOfPurchaseDescription);
                }

                report.SetParameterValue("BudgetYear", budgetYear);
                report.SetParameterValue("PeriodFrom", periodFrom);
                report.SetParameterValue("PeriodTo", periodTo);
                report.SetParameterValue("type", "ar");
                report.SetParameterValue("CutoffDate", DateTimeUtility.getDateString(cutoffDate));
                report.SetParameterValue("ExchangeRateYear", exchangeRateYear.ToString());
                report.SetParameterValue("ExchangeRatePeriod", exchangeRatePeriod.ToString());
                report.SetParameterValue("ReportVersion", (reportVersion == 0 ? "SUN" : (reportVersion == 1 ? "Epicor" : "")));

                report.SetParameterValue("Supplier", vendorId == -1 ? "" : VendorWorker.Instance.getVendorByKey(vendorId).Name);

                string orderTypeList = "";
                if (orderType != "")
                    orderTypeList += orderType == "F" ? "FOB" : "VM";

                if (isSZOrder != -1)
                {
                    orderTypeList += isSZOrder == 1 ? "SZ Order" : "Non-SZ Order";
                }
                if (isUTOrder != -1)
                {
                    if (orderTypeList != "")
                        orderTypeList += ", ";
                    orderTypeList += isUTOrder == 1 ? "UT Order" : "Non-UT Order";
                }
                if (isOPROrder != -1)
                {
                    if (orderTypeList != "")
                        orderTypeList += ", ";
                    orderTypeList += isOPROrder == 1 ? "OPR Order" : " Non-OPR Order";
                }
                if (sampleOrderGroup != -1)
                {
                    if (orderTypeList != "")
                        orderTypeList += ", ";
                    switch (sampleOrderGroup)
                    {
                        case 0:
                            orderTypeList += "Mainline Order";
                            break;
                        case 1:
                            orderTypeList += "Mock Shop/Press Sample Order";
                            break;
                        case 2:
                            orderTypeList += "Mock Shop Sample Order";
                            break;
                        case 3:
                            orderTypeList += "Press Sample Order";
                            break;
                        case 4:
                            orderTypeList += "Mainline And Press Sample Order";
                            break;
                    }
                }
                report.SetParameterValue("OrderType", orderTypeList);
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
                    tradingAgency.Remove(tradingAgency.LastIndexOf(", "));
                    report.SetParameterValue("TradingAgency", tradingAgency);
                }
                report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            }

            return report;
        }


        public OutstandingTradeReport getAPReport(DateTime cutoffDate, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, DateTime accReceiptDateFrom, DateTime accReceiptDateTo,
            int budgetYear, int periodFrom, int periodTo, int currencyId, int baseCurrencyId, int exchangeRateYear, int exchangeRatePeriod, string payRefCode, int termOfPurchaseId, int paymentTermId, string orderType, int officeId,
            TypeCollector officeIdList, int handlingOfficeId, TypeCollector productTeamList, int vendorId, int isSZOrder, int isUTOrder, int isOPROrder, TypeCollector customerIdList, TypeCollector tradingAgencyList, int sampleOrderGroup, 
            int reportVersion, int userId)
        {
            OutstandingTradeReport report = null;
            OutstandingTradeReportDs reportDs;
            CommonWorker commonWorker = CommonWorker.Instance;


            reportDs = ReporterWorker.Instance.getAPListByCriteria(cutoffDate, invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo,
                    accReceiptDateFrom, accReceiptDateTo,
            budgetYear, periodFrom, periodTo, currencyId, baseCurrencyId, exchangeRateYear, exchangeRatePeriod, payRefCode, termOfPurchaseId, paymentTermId, orderType, officeId,
            officeIdList, handlingOfficeId, productTeamList, vendorId, isSZOrder, isUTOrder, isOPROrder, customerIdList, tradingAgencyList, sampleOrderGroup, reportVersion, userId);

            bool missingExchangeRate = false;
            foreach (OutstandingTradeReportDs.OutstandingTradeReportRow row in reportDs.Tables[0].Rows)
            {
                if (row.OtherAmount > 0 && row.BaseAmount == 0)
                {   // Exchange rate is undefined
                    missingExchangeRate = true;
                    break;
                }
            }

            if (!missingExchangeRate)
            {
                report = new OutstandingTradeReport();
                report.SetDataSource(reportDs);

                report.SetParameterValue("InvoiceDateFrom", DateTimeUtility.getDateString(invoiceDateFrom));
                report.SetParameterValue("InvoiceDateTo", DateTimeUtility.getDateString(invoiceDateTo));
                report.SetParameterValue("InvoiceUploadDateFrom", DateTimeUtility.getDateString(invoiceUploadDateFrom));
                report.SetParameterValue("InvoiceUploadDateTo", DateTimeUtility.getDateString(invoiceUploadDateTo));
                report.SetParameterValue("AccReceiptDateFrom", DateTimeUtility.getDateString(accReceiptDateFrom));
                report.SetParameterValue("AccReceiptDateTo", DateTimeUtility.getDateString(accReceiptDateTo));
                string officeList = string.Empty;
                foreach (int id in officeIdList.Values)
                    officeList += (officeList == string.Empty ? string.Empty : ", ") + GeneralWorker.Instance.getOfficeRefByKey(id).OfficeCode;
                report.SetParameterValue("Office", officeIdList.Values.Count == 0 ? "" : officeList);
                report.SetParameterValue("HandlingOffice", handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).OfficeCode);

                report.SetParameterValue("Currency", currencyId == -1 ? "" : GeneralWorker.Instance.getCurrencyByKey(currencyId).CurrencyCode);
                report.SetParameterValue("BaseCurrency", GeneralWorker.Instance.getCurrencyByKey(baseCurrencyId).CurrencyCode);

                report.SetParameterValue("PayRefCode", payRefCode);

                if (reportDs.OutstandingTradeReport.Rows.Count != 0)
                {
                    report.SetParameterValue("TermOfPurchase", termOfPurchaseId == -1 ? "" : reportDs.OutstandingTradeReport.Rows[0]["TermOfPurchase"].ToString());
                }
                else
                {
                    report.SetParameterValue("TermOfPurchase", termOfPurchaseId == -1 ? "" : commonWorker.getTermOfPurchaseByKey(termOfPurchaseId).TermOfPurchaseDescription);
                }

                report.SetParameterValue("BudgetYear", budgetYear);
                report.SetParameterValue("PeriodFrom", periodFrom);
                report.SetParameterValue("PeriodTo", periodTo);
                report.SetParameterValue("type", "ap");
                report.SetParameterValue("CutoffDate", DateTimeUtility.getDateString(cutoffDate));
                report.SetParameterValue("ExchangeRateYear", exchangeRateYear.ToString());
                report.SetParameterValue("ExchangeRatePeriod", exchangeRatePeriod.ToString());
                report.SetParameterValue("Supplier", vendorId == -1 ? "" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
                report.SetParameterValue("ReportVersion", (reportVersion == 0 ? "SUN" : (reportVersion == 1 ? "Epicor" : "")));

                string orderTypeList = "";
                if (orderType != "")
                    orderTypeList += orderType == "F" ? "FOB" : "VM";

                if (isSZOrder != -1)
                {
                    orderTypeList += isSZOrder == 1 ? "SZ Order" : "Non-SZ Order";
                }
                if (isUTOrder != -1)
                {
                    if (orderTypeList != "")
                        orderTypeList += ", ";
                    orderTypeList += isUTOrder == 1 ? "UT Order" : "Non-UT Order";
                }
                if (isOPROrder != -1)
                {
                    if (orderTypeList != "")
                        orderTypeList += ", ";
                    orderTypeList += isOPROrder == 1 ? "OPR Order" : " Non-OPR Order";
                }
                if (sampleOrderGroup != -1)
                {
                    if (orderTypeList != "")
                        orderTypeList += ", ";
                    switch (sampleOrderGroup)
                    {
                        case 0:
                            orderTypeList += "Mainline Order";
                            break;
                        case 1:
                            orderTypeList += "Mock Shop/Press Sample Order";
                            break;
                        case 2:
                            orderTypeList += "Mock Shop Sample Order";
                            break;
                        case 3:
                            orderTypeList += "Press Sample Order";
                            break;
                        case 4:
                            orderTypeList += "Mainline And Press Sample Order";
                            break;
                    }
                }
                report.SetParameterValue("OrderType", orderTypeList);

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
                    tradingAgency.Remove(tradingAgency.LastIndexOf(", "));
                    report.SetParameterValue("TradingAgency", tradingAgency);
                }
                report.SetParameterValue("PrintUser", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            }

            return report;
        }

    }
}
