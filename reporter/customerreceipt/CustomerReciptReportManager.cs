using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Collections;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.common;
using com.next.isam.domain.order;
using com.next.isam.domain.shipping;
using com.next.isam.domain.ils;
using com.next.isam.domain.types;
using com.next.isam.reporter.dataserver;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.types;
using com.next.isam.reporter;
using com.next.infra.util;

namespace com.next.isam.reporter.customerreceipt
{
    public class CustomerReceiptReportManager
    {
        private static CustomerReceiptReportManager _instance;
        private ShippingWorker shippingWorker;

        public CustomerReceiptReportManager()
		{
            shippingWorker = ShippingWorker.Instance;
		}

        public static CustomerReceiptReportManager Instance
		{
			get 
			{
				if (_instance == null)
				{
                    _instance = new CustomerReceiptReportManager();
				}
				return _instance;
			}
		}


        public CustomerReceiptRpt getCustomerReceiptReport (DateTime receiptDateFrom, DateTime receiptDateTo, int fiscalYear, int periodFrom, int periodTo, 
            int baseCurrencyId, ArrayList customerIdList, ArrayList currencyIdList, 
            //ArrayList ReceiptReferenceNoList, 
            string receiptReferenceNo, TypeCollector officeIdList, int handlingOfficeId, ArrayList seasonIdList, 
            //int departmentId, 
            TypeCollector departmentIdList, int vendorId, int productTeamId, ArrayList tradingAgencyIdList, 
            ArrayList purchaseTermIdList, ArrayList paymentTermIdList, int userId
            ,string baseCurrencyName, string customerName, string currencyName, string officeName, string seasonName, string departmentName, 
            string productTeamName, string vendorName, string tradingAgencyName, string purchaseTermName, string paymentTermName, int dateType, int sampleType, string version)
        {
            CustomerReceiptRpt rpt = new CustomerReceiptRpt();
            //CustomerReceiptReportDs rptDs = new CustomerReceiptReportDs();
            CommonWorker commonWorker = CommonWorker.Instance;

            if (fiscalYear == -1) fiscalYear = int.MinValue;
            if (periodFrom == -1) periodFrom = int.MinValue;
            if (periodTo == -1) periodTo = int.MinValue;

            DataSet ds = ReporterWorker.Instance.getCustomerReceiptReportByCriteria(receiptDateFrom, receiptDateTo, fiscalYear, periodFrom, periodTo, 
                            baseCurrencyId, customerIdList, currencyIdList, 
                            //ReceiptReferenceNoList, 
                            receiptReferenceNo, officeIdList, handlingOfficeId, seasonIdList, 
                            //departmentId, 
                            departmentIdList, vendorId, productTeamId, 
                            tradingAgencyIdList, purchaseTermIdList, paymentTermIdList, dateType, sampleType, userId, version);

            rpt.SetDataSource(ds.Tables[0]);

            rpt.SetParameterValue("FiscalYear", (fiscalYear == int.MinValue ? "" : fiscalYear.ToString()));
            rpt.SetParameterValue("PeriodFrom", (periodFrom == int.MinValue ? "" : periodFrom.ToString()));
            rpt.SetParameterValue("PeriodTo", (periodTo == int.MinValue ? "" : periodTo.ToString()));
            rpt.SetParameterValue("ReceiptDateFrom", DateTimeUtility.getDateString(receiptDateFrom));
            rpt.SetParameterValue("ReceiptDateTo", DateTimeUtility.getDateString(receiptDateTo));
            rpt.SetParameterValue("CustomerList", customerName);
            rpt.SetParameterValue("CurrencyList", currencyName);
            rpt.SetParameterValue("ReceiptReferenceNoList", receiptReferenceNo);
            rpt.SetParameterValue("OfficeList", officeName);
            rpt.SetParameterValue("HandlingOffice", (handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).Description));
            rpt.SetParameterValue("SeasonList", seasonName);
            rpt.SetParameterValue("ProductTeamList", productTeamName);
            rpt.SetParameterValue("TradingAgencyList", tradingAgencyName);
            rpt.SetParameterValue("PurchaseTermList", purchaseTermName);
            rpt.SetParameterValue("PaymentTermList", paymentTermName);
            rpt.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            rpt.SetParameterValue("Department", departmentName);
            string sampleTypeDesc = "";
            if (sampleType != -1)
            {
                switch (sampleType)
                {
                    case -1:
                        sampleTypeDesc = "All (Mainline + Mock Shop/Press/Studio Sample Order)";
                        break;
                    case 0:
                        sampleTypeDesc = "Mainline Order";
                        break;
                    case 1:
                        sampleTypeDesc = "Mock Shop/Press/Studio Sample Order";
                        break;
                }
            }
            rpt.SetParameterValue("OrderType", sampleTypeDesc);

            return rpt;
        }





    }
}
