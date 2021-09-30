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
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.types;
using com.next.isam.reporter;
using com.next.isam.reporter.common;
using com.next.isam.reporter.dataserver;
using com.next.infra.util;

namespace com.next.isam.reporter.supplierpayment
{
    public class SupplierPaymentReportManager
    {
        private static SupplierPaymentReportManager _instance;
        private ShippingWorker shippingWorker;

        public SupplierPaymentReportManager()
        {
            shippingWorker = ShippingWorker.Instance;
        }

        public static SupplierPaymentReportManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SupplierPaymentReportManager();
                }
                return _instance;
            }
        }

        /*
        public SupplierPaymentRpt Trial_getSupplierPaymentReport(DateTime paymentDateFrom, DateTime paymentDateTo, int fiscalYear, int periodFrom, int periodTo,
            int baseCurrencyId, ArrayList customerIdList, ArrayList currencyIdList, string paymentReferenceNo, ArrayList officeIdList, ArrayList seasonIdList,
            ArrayList productTeamIdList, ArrayList tradingAgencyIdList, ArrayList purchaseTermIdList, ArrayList paymentTermIdList, int userId)
        {
            SupplierPaymentRpt rpt = new SupplierPaymentRpt();
            CommonWorker commonWorker = CommonWorker.Instance;

            if (fiscalYear == -1) fiscalYear = int.MinValue;
            if (periodFrom == -1) periodFrom = int.MinValue;
            if (periodTo == -1) periodTo = int.MinValue;

            DataSet ds = ReporterWorker.Instance.getSupplierPaymentReportByCriteria(paymentDateFrom, paymentDateTo, fiscalYear, periodFrom, periodTo,
                            baseCurrencyId, customerIdList, currencyIdList, paymentReferenceNo, officeIdList, seasonIdList,
                            productTeamIdList, tradingAgencyIdList, purchaseTermIdList, paymentTermIdList, -1, -1);
            rpt.SetDataSource(ds.Tables[0]);
            rpt.SetParameterValue("FiscalYear", "");
            rpt.SetParameterValue("PeriodFrom", "");
            rpt.SetParameterValue("PeriodTo", "");
            rpt.SetParameterValue("PaymentDateFrom", "");
            rpt.SetParameterValue("PaymentDateTo", "");
            rpt.SetParameterValue("CustomerList", "");
            rpt.SetParameterValue("CurrencyList", "");
            rpt.SetParameterValue("PaymentReferenceNoList", "");
            rpt.SetParameterValue("OfficeList", "");
            rpt.SetParameterValue("SeasonList", "");
            rpt.SetParameterValue("ProductTeamList", "");
            rpt.SetParameterValue("TradingAgencyList", "");
            rpt.SetParameterValue("PurchaseTermList", "");
            rpt.SetParameterValue("PaymentTermList", "");

            return rpt;
        }
        */

        public SupplierPaymentRpt getSupplierPaymentReport(DateTime paymentDateFrom, DateTime paymentDateTo, int fiscalYear, int periodFrom, int periodTo,
            int baseCurrencyId, ArrayList customerIdList, ArrayList currencyIdList, string paymentReferenceNo, TypeCollector officeIdList, int handlingOfficeId, ArrayList seasonIdList,
            TypeCollector productTeamIdList, ArrayList tradingAgencyIdList, ArrayList purchaseTermIdList, ArrayList paymentTermIdList, int userId, TypeCollector departmentIdList, int vendorId,
            string baseCurrencyName, string customerName, string currencyName, string officeName, string seasonName,
            string productTeamName, string tradingAgencyName, string purchaseTermName, string paymentTermName, string departmentName, string vendorName, string version)
        {
            SupplierPaymentRpt rpt = new SupplierPaymentRpt();
            CommonWorker commonWorker = CommonWorker.Instance;

            if (fiscalYear == -1) fiscalYear = int.MinValue;
            if (periodFrom == -1) periodFrom = int.MinValue;
            if (periodTo == -1) periodTo = int.MinValue;

            DataSet ds = ReporterWorker.Instance.getSupplierPaymentReportByCriteria(paymentDateFrom, paymentDateTo, fiscalYear, periodFrom, periodTo,
                            baseCurrencyId, customerIdList, currencyIdList, paymentReferenceNo, officeIdList, handlingOfficeId, seasonIdList,
                            productTeamIdList, tradingAgencyIdList, purchaseTermIdList, paymentTermIdList, departmentIdList, vendorId, version);
            rpt.SetDataSource(ds.Tables[0]);

            rpt.SetParameterValue("FiscalYear", (fiscalYear == int.MinValue ? "" : fiscalYear.ToString()));
            rpt.SetParameterValue("PeriodFrom", (periodFrom == int.MinValue ? "" : periodFrom.ToString()));
            rpt.SetParameterValue("PeriodTo", (periodTo == int.MinValue ? "" : periodTo.ToString()));
            rpt.SetParameterValue("PaymentDateFrom", DateTimeUtility.getDateString(paymentDateFrom));
            rpt.SetParameterValue("PaymentDateTo", DateTimeUtility.getDateString(paymentDateTo));
            rpt.SetParameterValue("CustomerList", customerName);
            rpt.SetParameterValue("CurrencyList", currencyName);
            rpt.SetParameterValue("PaymentReferenceNoList", paymentReferenceNo);
            rpt.SetParameterValue("OfficeList", officeName);
            rpt.SetParameterValue("HandlingOffice", (handlingOfficeId == -1 ? "ALL" : commonWorker.getDGHandlingOffice(handlingOfficeId).Description));
            rpt.SetParameterValue("SeasonList", seasonName);
            rpt.SetParameterValue("ProductTeamList", productTeamName);
            rpt.SetParameterValue("TradingAgencyList", tradingAgencyName);
            rpt.SetParameterValue("PurchaseTermList", purchaseTermName);
            rpt.SetParameterValue("PaymentTermList", paymentTermName);
            rpt.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            rpt.SetParameterValue("Version", version);

            return rpt;
        }

    }
}
