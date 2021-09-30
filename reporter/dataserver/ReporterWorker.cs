using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.next.infra.persistency.dataaccess;
using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.account;
using com.next.common.domain.module;
using com.next.common.web.commander;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.ARAP;
using com.next.isam.reporter.shipping;
using com.next.isam.reporter.lcreport;
using com.next.isam.reporter.invoice;
using com.next.isam.reporter.NSSreport;
using com.next.isam.dataserver.model.account;
using com.next.isam.dataserver.model.shipping;
using com.next.isam.dataserver.model.nss;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.infra.persistency.transactions;
using com.next.isam.domain.shipping;

namespace com.next.isam.reporter.dataserver
{
    public class ReporterWorker : Worker
    {
        GeneralWorker generalWorker;
        NonTradeWorker nontradeWorker;

        protected ReporterWorker()
        {
            generalWorker = GeneralWorker.Instance;
            nontradeWorker = NonTradeWorker.Instance;
        }

        public static ReporterWorker Instance
        {
            get
            {
                ReporterWorker _instance;
                _instance = new ReporterWorker();
                return _instance;
            }
        }

        public OutstandingTradeReportDs getARListByCriteria(DateTime cutoffDate, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo,
            int budgetYear, int periodFrom, int periodTo, int currencyId, int baseCurrencyId, int exchangeRateYear, int exchangeRatePeriod, string payRefCode, int termOfPurchaseId, string orderType, int officeId,
            TypeCollector officeIdList, int handlingOfficeId, TypeCollector productTeamList, int vendorId, int isSZOrder, int isUTOrder, int isOPROrder, TypeCollector customerIdList, TypeCollector tradingAgencyList, int sampleOrderGroup,
            int dataType, int reportVersion)
        {
            IDataSetAdapter ad = getDataSetAdapter("OutstandingTradeReportApt", "getARListByCriteria");

            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }
            if (invoiceUploadDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = invoiceUploadDateFrom;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = invoiceUploadDateTo;
            }

            ad.SelectCommand.Parameters["@currencyId"].Value = currencyId;
            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;
            ad.SelectCommand.Parameters["@exchangeRateYear"].Value = exchangeRateYear;
            ad.SelectCommand.Parameters["@exchangeRatePeriod"].Value = exchangeRatePeriod;
            ad.SelectCommand.Parameters["@payRefCode"].Value = payRefCode;
            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            if (orderType == "")
                ad.SelectCommand.Parameters["@orderType"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@orderType"].Value = orderType;
            ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            //ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.CustomParameters["@tradingAgencyList"] = CustomDataParameter.parse(tradingAgencyList.IsInclusive, tradingAgencyList.Values);
            ad.SelectCommand.Parameters["@budgetYear"].Value = budgetYear;
            ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@cutoffDate"].Value = cutoffDate;
            ad.SelectCommand.Parameters["@isSZOrder"].Value = isSZOrder;
            ad.SelectCommand.Parameters["@isUTOrder"].Value = isUTOrder;
            ad.SelectCommand.Parameters["@isOPROrder"].Value = isOPROrder;
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@SampleOrderGroup"].Value = sampleOrderGroup;
            ad.SelectCommand.Parameters["@dataType"].Value = dataType;
            ad.SelectCommand.Parameters["@reportVersion"].Value = reportVersion;

            OutstandingTradeReportDs dataSet = new OutstandingTradeReportDs();
            ad.SelectCommand.MailSQL = true;
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public OutstandingTradeReportDs getAPListByCriteria(DateTime cutoffDate, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, DateTime accReceiptDateFrom, DateTime accReceiptDateTo,
            int budgetYear, int periodFrom, int periodTo, int currencyId, int baseCurrencyId, int exchangeRateYear, int exchangeRatePeriod, string payRefCode, int termOfPurchaseId, int paymentTermId, string orderType, int officeId,
            TypeCollector officeIdList, int handlingOfficeId, TypeCollector productTeamList, int vendorId, int isSZOrder, int isUTOrder, int isOPROrder, TypeCollector customerIdList, TypeCollector tradingAgencyList, int sampleOrderGroup, int reportVersion, int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("OutstandingTradeReportApt", "getAPListByCriteria");

            ad.SelectCommand.Parameters["@cutoffDate"].Value = cutoffDate;

            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }
            if (invoiceUploadDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = invoiceUploadDateFrom;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = invoiceUploadDateTo;
            }
            if (accReceiptDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@accReceiptDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@accReceiptDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@accReceiptDateFrom"].Value = accReceiptDateFrom;
                ad.SelectCommand.Parameters["@accReceiptDateTo"].Value = accReceiptDateTo;
            }

            ad.SelectCommand.Parameters["@currencyId"].Value = currencyId;
            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;
            ad.SelectCommand.Parameters["@exchangeRateYear"].Value = exchangeRateYear;
            ad.SelectCommand.Parameters["@exchangeRatePeriod"].Value = exchangeRatePeriod;
            ad.SelectCommand.Parameters["@payRefCode"].Value = payRefCode;
            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            if (orderType == "")
                ad.SelectCommand.Parameters["@orderType"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@orderType"].Value = orderType;
            ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.Parameters["@paymentTermId"].Value = paymentTermId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            //ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.CustomParameters["@tradingAgencyList"] = CustomDataParameter.parse(tradingAgencyList.IsInclusive, tradingAgencyList.Values);
            ad.SelectCommand.Parameters["@budgetYear"].Value = budgetYear;
            ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@isSZOrder"].Value = isSZOrder;
            ad.SelectCommand.Parameters["@isUTOrder"].Value = isUTOrder;
            ad.SelectCommand.Parameters["@isOPROrder"].Value = isOPROrder;
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@SampleOrderGroup"].Value = sampleOrderGroup;
            ad.SelectCommand.Parameters["@ReportVersion"].Value = reportVersion;
            ad.SelectCommand.Parameters["@userId"].Value = userId;
            ad.SelectCommand.MailSQL = true;

            OutstandingTradeReportDs dataSet = new OutstandingTradeReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public FutureOrderSummaryBySupplierReportDs getFutureOrderSummaryBySupplierReport(int officeId, int vendorId, DateTime dateFrom, DateTime dateTo)
        {
            IDataSetAdapter dataSetAdapter = getDataSetAdapter("FutureOrderSummaryBySupplierApt", "GetFutureOrderSummaryBySupplier");
            dataSetAdapter.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            dataSetAdapter.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            dataSetAdapter.SelectCommand.Parameters["@DateFrom"].Value = dateFrom;
            dataSetAdapter.SelectCommand.Parameters["@DateTo"].Value = dateTo;
            FutureOrderSummaryBySupplierReportDs ds = new FutureOrderSummaryBySupplierReportDs();
            dataSetAdapter.SelectCommand.MailSQL = true;
            dataSetAdapter.SelectCommand.DbCommand.CommandTimeout = 3600;
            int num = dataSetAdapter.Fill(ds);
            return ds;
        }

        public InvoiceListReportDs getInvoiceListByCriteria(int baseCurrencyId, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, DateTime purchaseExtractDateFrom, DateTime purchaseExtractDateTo,
            int budgetYear, int periodFrom, int periodTo, DateTime departDateFrom, DateTime departDateTo, int vendorId, int termOfPurchaseId,
            TypeCollector officeIdList, int handlingOfficeId, int seasonId, TypeCollector productTeamList, int customerDestinationId, int oprFabricType, int isSZOrder, int isUTOrder, int isLDPOrder, int isSampleOrder,
            TypeCollector customerIdList, TypeCollector tradingAgencyList, TypeCollector shipmentMethodList, int accountDocReceipt, string voyageNo,
            int paymentTermId, int isAccPaid, int isLcPaymentChecked, int shippingDocReceiptDate, string sortingField, string version)
        {
            IDataSetAdapter ad = getDataSetAdapter("InvoiceListReportApt", "GetInvoiceListByCriteria");

            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;

            ad.SelectCommand.Parameters["@invoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@invoiceSeqFrom"].Value = invoiceSeqFrom;
            ad.SelectCommand.Parameters["@invoiceSeqTo"].Value = invoiceSeqTo;
            ad.SelectCommand.Parameters["@invoiceYear"].Value = invoiceYear;
            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }
            if (purchaseExtractDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@purchaseExtractDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@purchaseExtractDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@purchaseExtractDateFrom"].Value = purchaseExtractDateFrom;
                ad.SelectCommand.Parameters["@purchaseExtractDateTo"].Value = purchaseExtractDateTo.AddDays(1);
            }
            if (departDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@departDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@departDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@departDateFrom"].Value = departDateFrom;
                ad.SelectCommand.Parameters["@departDateTo"].Value = departDateTo;
            }
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.Parameters["@seasonId"].Value = seasonId;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.Parameters["@oprFabricType"].Value = oprFabricType;
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.CustomParameters["@tradingAgencyList"] = CustomDataParameter.parse(tradingAgencyList.IsInclusive, tradingAgencyList.Values);
            ad.SelectCommand.CustomParameters["@shipmentMethodList"] = CustomDataParameter.parse(shipmentMethodList.IsInclusive, shipmentMethodList.Values);
            ad.SelectCommand.Parameters["@budgetYear"].Value = budgetYear;
            ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@customerDestinationId"].Value = customerDestinationId;
            ad.SelectCommand.Parameters["@isSZOrder"].Value = isSZOrder;
            ad.SelectCommand.Parameters["@isUTOrder"].Value = isUTOrder;
            ad.SelectCommand.Parameters["@isLDPOrder"].Value = isLDPOrder;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;
            ad.SelectCommand.Parameters["@accountDocReceipt"].Value = accountDocReceipt;
            ad.SelectCommand.Parameters["@voyageNo"].Value = voyageNo;
            ad.SelectCommand.Parameters["@version"].Value = version;
            ad.SelectCommand.Parameters["@PaymentTermId"].Value = paymentTermId;
            ad.SelectCommand.Parameters["@IsAccountPaid"].Value = isAccPaid;
            ad.SelectCommand.Parameters["@IsLCPaymentChecked"].Value = isLcPaymentChecked;
            ad.SelectCommand.Parameters["@shippingDocReceiptDate"].Value = shippingDocReceiptDate;

            if (sortingField != "")
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY " + sortingField;

            InvoiceListReportDs dataSet = new InvoiceListReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public InvoiceListSummaryReportDs getInvoiceListSummaryByCriteria(TypeCollector lcNumberList, DateTime invoiceDateFrom, DateTime invoiceDateTo)
        {
            IDataSetAdapter ad = getDataSetAdapter("InvoiceListSummaryReportApt", "GetInvoiceListSummaryByCriteria");
            ad.SelectCommand.CustomParameters["@lcNumberList"] = CustomDataParameter.parse(lcNumberList.IsInclusive, lcNumberList.Values);

            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }

            InvoiceListSummaryReportDs dataSet = new InvoiceListSummaryReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public ShipmentCommissionReportDs getShipmentCommissionReport(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, DateTime invoiceDateFrom,
            DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, DateTime purchaseExtractDateFrom,
            DateTime purchaseExtractDateTo, int budgetYear, int periodFrom, int isActual, int isRealized, int isAccrual,
            int vendorId, int baseCurrencyId, int countryOfOriginId, int officeId, int handlingOfficeId, TypeCollector officeIdList, int seasonId, TypeCollector productTeamList,
            TypeCollector customerIdList, TypeCollector tradingAgencyList, int departmentId, TypeCollector departmentIdList, int termOfPurchaseId, int isSZOrder, int isUTOrder,
            int isOPROrder, int isLDPOrder, int isNSLTailoring, int isSampleOrder, TypeCollector designSourceList,
            string DCNoteNoFrom, string DCNoteNoTo, string supplierInvoiceNo, bool isSampleReport, int reportVersion, string sortingField)
        {
            IDataSetAdapter ad;
            ad = getDataSetAdapter("ShipmentCommissionReportApt", "getShipmentCommissionReport");

            //ad.SelectCommand.MailSQL = true;
            ad.SelectCommand.Parameters["@invoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@invoiceSeqFrom"].Value = invoiceSeqFrom;
            ad.SelectCommand.Parameters["@invoiceSeqTo"].Value = invoiceSeqTo;
            ad.SelectCommand.Parameters["@invoiceYear"].Value = invoiceYear;

            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }
            if (invoiceUploadDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = invoiceUploadDateFrom;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = invoiceUploadDateTo;
            }
            if (purchaseExtractDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@purchaseExtractDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@purchaseExtractDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@purchaseExtractDateFrom"].Value = purchaseExtractDateFrom;
                ad.SelectCommand.Parameters["@purchaseExtractDateTo"].Value = purchaseExtractDateTo;
            }
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;
            ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            //ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);

            ad.SelectCommand.Parameters["@seasonId"].Value = seasonId;
            ad.SelectCommand.CustomParameters["@tradingAgencyList"] = CustomDataParameter.parse(tradingAgencyList.IsInclusive, tradingAgencyList.Values);
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId;

            ad.SelectCommand.Parameters["@budgetYear"].Value = budgetYear;
            ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;
            //ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;

            ad.SelectCommand.Parameters["@departmentId"].Value = departmentId;
            ad.SelectCommand.CustomParameters["@departmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);

            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@isUTOrder"].Value = isUTOrder;
            ad.SelectCommand.Parameters["@isOPROrder"].Value = isOPROrder;
            ad.SelectCommand.CustomParameters["@designSourceList"] = CustomDataParameter.parse(designSourceList.IsInclusive, designSourceList.Values);

            ad.SelectCommand.Parameters["@isSZOrder"].Value = isSZOrder;
            ad.SelectCommand.Parameters["@isActual"].Value = isActual;
            ad.SelectCommand.Parameters["@isRealized"].Value = isRealized;
            ad.SelectCommand.Parameters["@isAccrual"].Value = isAccrual;
            ad.SelectCommand.Parameters["@isLDPOrder"].Value = isLDPOrder;
            ad.SelectCommand.Parameters["@isNSLTailoring"].Value = isNSLTailoring;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;


            if (DCNoteNoFrom == string.Empty && DCNoteNoTo == string.Empty)
            {
                ad.SelectCommand.Parameters["@DCNoteNoFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@DCNoteNoTo"].Value = DBNull.Value;
            }
            else
            {
                if (DCNoteNoFrom == string.Empty)
                    DCNoteNoFrom = DCNoteNoTo;
                else
                    if (DCNoteNoTo == string.Empty)
                    DCNoteNoTo = DCNoteNoFrom;
                ad.SelectCommand.Parameters["@DCNoteNoFrom"].Value = DCNoteNoFrom;
                ad.SelectCommand.Parameters["@DCNoteNoTo"].Value = DCNoteNoTo;
            }

            if (supplierInvoiceNo == string.Empty)
                ad.SelectCommand.Parameters["@supplierInvoiceNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@supplierInvoiceNo"].Value = supplierInvoiceNo;

            ad.SelectCommand.Parameters["@ReportVersion"].Value = reportVersion;  // 0 - Sun; 1 - Epicor 

            ad.SelectCommand.DbCommand.CommandText += " ORDER BY DCNoteNo ";
            if (sortingField == "InvoiceDate")
                ad.SelectCommand.DbCommand.CommandText += ", InvoiceDate";
            else if (sortingField == "VendorName")
                ad.SelectCommand.DbCommand.CommandText += ", VendorName";
            else
                ad.SelectCommand.DbCommand.CommandText += ", InvoiceNo";
            ad.SelectCommand.MailSQL = true;

            ShipmentCommissionReportDs dataSet = new ShipmentCommissionReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        /*
        public ShipmentCommissionReportDs getShipmentCommissionChoiceReport(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, DateTime invoiceDateFrom,
                DateTime invoiceDateTo, DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, DateTime purchaseExtractDateFrom,
                DateTime purchaseExtractDateTo, int budgetYear, int periodFrom, int isActual, int isRealized,
                int vendorId, int baseCurrencyId, int countryOfOriginId, int officeId, TypeCollector officeIdList, int seasonId, TypeCollector productTeamList,
                TypeCollector tradingAgencyList, int departmentId, TypeCollector departmentIdList, int termOfPurchaseId, string sortingField)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentCommissionReportApt", "getShipmentCommissionChoiceReport");

            //ad.SelectCommand.MailSQL = true;
            ad.SelectCommand.Parameters["@invoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@invoiceSeqFrom"].Value = invoiceSeqFrom;
            ad.SelectCommand.Parameters["@invoiceSeqTo"].Value = invoiceSeqTo;
            ad.SelectCommand.Parameters["@invoiceYear"].Value = invoiceYear;

            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }
            if (invoiceUploadDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = invoiceUploadDateFrom;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = invoiceUploadDateTo;
            }
            if (purchaseExtractDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@purchaseExtractDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@purchaseExtractDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@purchaseExtractDateFrom"].Value = purchaseExtractDateFrom;
                ad.SelectCommand.Parameters["@purchaseExtractDateTo"].Value = purchaseExtractDateTo;
            }
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;
            ad.SelectCommand.Parameters["@officeId"].Value = officeId;

            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            //ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);

            ad.SelectCommand.Parameters["@seasonId"].Value = seasonId;
            ad.SelectCommand.CustomParameters["@tradingAgencyList"] = CustomDataParameter.parse(tradingAgencyList.IsInclusive, tradingAgencyList.Values);

            ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId;

            ad.SelectCommand.Parameters["@budgetYear"].Value = budgetYear;
            ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;
            //ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;

            ad.SelectCommand.Parameters["@departmentId"].Value = departmentId;
            ad.SelectCommand.CustomParameters["@departmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);

            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;

            ad.SelectCommand.Parameters["@isActual"].Value = isActual;
            ad.SelectCommand.Parameters["@isRealized"].Value = isRealized;

            if (sortingField == "InvoiceDate")
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY  InvoiceDate";
            else if (sortingField == "VendorName")
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY  VendorName";
            else
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY  InvoiceNo";


            ShipmentCommissionReportDs dataSet = new ShipmentCommissionReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }


        public ShipmentCommissionReportDs getAccrualShipmentCommission(int budgetYear, int periodFrom,
            int vendorId, int baseCurrencyId, int countryOfOriginId, int officeId, int handlingOfficeId, TypeCollector officeIdList, int seasonId, TypeCollector productTeamList,
            TypeCollector customerIdList, TypeCollector tradingAgencyList, int departmentId, TypeCollector departmentIdList, int termOfPurchaseId, int isSZOrder, int isUTOrder,
            int isOPROrder, int isLDPOrder, int isNSLTailoring, int isSampleOrder, TypeCollector designSourceList,
            string MockShopDebitNoteNoFrom, string MockShopDebitNoteNoTo, string supplierInvoiceNo, string sortingField)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentCommissionReportApt", "getAccrualShipmentCommission");

            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;
            ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            //ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);

            ad.SelectCommand.Parameters["@seasonId"].Value = seasonId;
            ad.SelectCommand.CustomParameters["@tradingAgencyList"] = CustomDataParameter.parse(tradingAgencyList.IsInclusive, tradingAgencyList.Values);
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId;

            ad.SelectCommand.Parameters["@budgetYear"].Value = budgetYear;
            ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;
            //ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;

            ad.SelectCommand.Parameters["@departmentId"].Value = departmentId;
            ad.SelectCommand.CustomParameters["@departmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);

            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@isUTOrder"].Value = isUTOrder;
            ad.SelectCommand.Parameters["@isOPROrder"].Value = isOPROrder;
            ad.SelectCommand.CustomParameters["@designSourceList"] = CustomDataParameter.parse(designSourceList.IsInclusive, designSourceList.Values);

            ad.SelectCommand.Parameters["@isSZOrder"].Value = isSZOrder;
            ad.SelectCommand.Parameters["@isLDPOrder"].Value = isLDPOrder;
            ad.SelectCommand.Parameters["@isNSLTailoring"].Value = isNSLTailoring;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;

            if (MockShopDebitNoteNoFrom == string.Empty && MockShopDebitNoteNoTo == string.Empty)
            {
                ad.SelectCommand.Parameters["@mockShopDNNoFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@mockShopDNNoTo"].Value = DBNull.Value;
            }
            else
            {
                if (MockShopDebitNoteNoFrom == string.Empty)
                    MockShopDebitNoteNoFrom = MockShopDebitNoteNoTo;
                else
                    if (MockShopDebitNoteNoTo == string.Empty)
                        MockShopDebitNoteNoTo = MockShopDebitNoteNoFrom;
                ad.SelectCommand.Parameters["@mockShopDNNoFrom"].Value = MockShopDebitNoteNoFrom;
                ad.SelectCommand.Parameters["@mockShopDNNoTo"].Value = MockShopDebitNoteNoTo;
            }

            if (supplierInvoiceNo == string.Empty)
                ad.SelectCommand.Parameters["@supplierInvoiceNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@supplierInvoiceNo"].Value = supplierInvoiceNo;

            if (sortingField == "InvoiceDate")
                ad.SelectCommand.DbCommand.CommandText += "ORDER BY InvoiceDate";
            else if (sortingField == "VendorName")
                ad.SelectCommand.DbCommand.CommandText += "ORDER BY VendorName";
            else
                ad.SelectCommand.DbCommand.CommandText += "ORDER BY InvoiceNo";
            ad.SelectCommand.MailSQL = true;
            ShipmentCommissionReportDs dataSet = new ShipmentCommissionReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public void getAccrualSalesSummary(int budgetYear, int period, int baseCurrencyId, TypeCollector officeIdList, out decimal totalSalesAmt, out decimal totalSalesComm)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentCommissionReportApt", "getAccrualSalesSummary");

            ad.SelectCommand.Parameters["@budgetYear"].Value = budgetYear;
            ad.SelectCommand.Parameters["@period"].Value = period;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;

            DataSet dataSet = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            totalSalesAmt = dataSet.Tables[0].Rows[0][0] == DBNull.Value ? 0 : Convert.ToDecimal(dataSet.Tables[0].Rows[0][0]);
            totalSalesComm = dataSet.Tables[0].Rows[0][1] == DBNull.Value ? 0 : Convert.ToDecimal(dataSet.Tables[0].Rows[0][1]);
        }
        */

        public CTSSTWReportDs getCTSSTWReport(DateTime CTSDateFrom, DateTime CTSDateTo, DateTime STWDateFrom, DateTime STWDateTo,
            int dualSourcingOrder, int selfBilledOrder, int nslSZOrder, int utOrder, int isLDPOrder, int isSampleOrder, TypeCollector workflowStatusList,
            TypeCollector shipmentMethodList, TypeCollector customerList, int packingMethodId, int countryOfOriginId, TypeCollector officeIdList,
            TypeCollector productTeamList, int handlingOfficeId, int seasonId, int shipmentPortId, int oprType, TypeCollector customerDestination, int termOfPurchaseId, string sortingOrder)
        {
            IDataSetAdapter ad = getDataSetAdapter("CTSSTWReportApt", "GetCTSSTWReport");

            if (CTSDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@CTSDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@CTSDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@CTSDateFrom"].Value = CTSDateFrom;
                ad.SelectCommand.Parameters["@CTSDateTo"].Value = CTSDateTo;
            }
            if (STWDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@STWDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@STWDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@STWDateFrom"].Value = STWDateFrom;
                ad.SelectCommand.Parameters["@STWDateTo"].Value = STWDateTo;
            }

            ad.SelectCommand.Parameters["@dualSourcingOrder"].Value = dualSourcingOrder;
            ad.SelectCommand.Parameters["@selfBilledOrder"].Value = selfBilledOrder;
            ad.SelectCommand.Parameters["@NSLSZOrder"].Value = nslSZOrder;
            ad.SelectCommand.Parameters["@UTOrder"].Value = utOrder;
            ad.SelectCommand.CustomParameters["@status"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);
            ad.SelectCommand.CustomParameters["@shipmentMethodList"] = CustomDataParameter.parse(shipmentMethodList.IsInclusive, shipmentMethodList.Values);
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerList.IsInclusive, customerList.Values);
            ad.SelectCommand.Parameters["@packingMethodId"].Value = packingMethodId;
            ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            //ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId ;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.Parameters["@seasonId"].Value = seasonId;
            ad.SelectCommand.Parameters["@shipmentPortId"].Value = shipmentPortId;
            ad.SelectCommand.Parameters["@oprFabricType"].Value = oprType;
            ad.SelectCommand.CustomParameters["@customerDestination"] = CustomDataParameter.parse(customerDestination.IsInclusive, customerDestination.Values);
            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@isLDPOrder"].Value = isLDPOrder;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;

            if (sortingOrder != "")
            {
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY " + sortingOrder;
            }

            CTSSTWReportDs dataSet = new CTSSTWReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public CutoffSalesDiscrepancyReportDs getCutoffSalesDiscrepancyReport(int officeId, DateTime dateFrom, DateTime dateTo)
        {
            IDataSetAdapter ad = getDataSetAdapter("CutoffSalesDiscrepancyApt", "GetCutoffSalesDiscrepancyReport");

            ad.SelectCommand.Parameters["@DateFrom"].Value = dateFrom;
            ad.SelectCommand.Parameters["@DateTo"].Value = dateTo;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;

            CutoffSalesDiscrepancyReportDs dataSet = new CutoffSalesDiscrepancyReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public MonthEndShipmentDs getMonthEndSlippageReport(string officeCode, int fiscalYear, int period)
        {
            IDataSetAdapter ad = getDataSetAdapter("MonthEndShipmentApt", "GetMonthEndSlippageReport");

            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@Period"].Value = period;
            ad.SelectCommand.Parameters["@OfficeCode"].Value = officeCode;

            MonthEndShipmentDs dataSet = new MonthEndShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public WeeklyShipmentReportDs getWeeklyShipmentReport(DateTime stockToWHDateFrom, DateTime stockToWHDateTo,
            DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo, DateTime bookingDateFrom, DateTime bookingDateTo,
            DateTime bookedAtWHDateFrom, DateTime bookedAtWHDateTo, DateTime departDateFrom, DateTime departDateTo, string voyageNo,
            TypeCollector officeIdList, TypeCollector productTeamIdList, int handlingOfficeId, TypeCollector customerIdList,
            TypeCollector countryOfOriginIdList, TypeCollector customerDestinationIdList, TypeCollector shipmentPortIdList, TypeCollector shipmentMethodIdList, TypeCollector packingMethodIdList, int vendorId,
            TypeCollector termOfPurchaseIdList, TypeCollector oprFabricTypeIdList, int isNextMfgorder, int isDualSourcingOrder, int isLDPOrder, int isSampleOrder,
            string lcNoFrom, string lcNoTo, int paymentTermId, TypeCollector workflowStatusIdList,
            string sortField)
        {
            IDataSetAdapter ad = getDataSetAdapter("WeeklyShipmentReportApt", "GetWeeklyShipmentReport");

            if (stockToWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@stockToWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@stockToWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@stockToWHDateFrom"].Value = stockToWHDateFrom;
                ad.SelectCommand.Parameters["@stockToWHDateTo"].Value = stockToWHDateTo;
            }
            if (customerAtWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@customerAtWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@customerAtWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@customerAtWHDateFrom"].Value = customerAtWHDateFrom;
                ad.SelectCommand.Parameters["@customerAtWHDateTo"].Value = customerAtWHDateTo;
            }
            if (bookingDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@bookingDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@bookingDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@bookingDateFrom"].Value = bookingDateFrom;
                ad.SelectCommand.Parameters["@bookingDateTo"].Value = bookingDateTo;
            }
            if (bookedAtWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@bookedAtWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@bookedAtWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@bookedAtWHDateFrom"].Value = bookedAtWHDateFrom;
                ad.SelectCommand.Parameters["@bookedAtWHDateTo"].Value = bookedAtWHDateTo;
            }
            if (departDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@departDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@departDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@departDateFrom"].Value = departDateFrom;
                ad.SelectCommand.Parameters["@departDateTo"].Value = departDateTo;
            }
            if (voyageNo != "")
                ad.SelectCommand.Parameters["@voyageNo"].Value = voyageNo;
            else
                ad.SelectCommand.Parameters["@voyageNo"].Value = DBNull.Value;

            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            //ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            ad.SelectCommand.CustomParameters["@productTeamIdList"] = CustomDataParameter.parse(productTeamIdList.IsInclusive, productTeamIdList.Values);
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.CustomParameters["@countryOfOriginIdList"] = CustomDataParameter.parse(countryOfOriginIdList.IsInclusive, countryOfOriginIdList.Values);
            ad.SelectCommand.CustomParameters["@customerDestinationIdList"] = CustomDataParameter.parse(customerDestinationIdList.IsInclusive, customerDestinationIdList.Values);
            ad.SelectCommand.CustomParameters["@shipmentPortIdList"] = CustomDataParameter.parse(shipmentPortIdList.IsInclusive, shipmentPortIdList.Values);
            ad.SelectCommand.CustomParameters["@shipmentMethodIdList"] = CustomDataParameter.parse(shipmentMethodIdList.IsInclusive, shipmentMethodIdList.Values);
            ad.SelectCommand.CustomParameters["@packingMethodIdList"] = CustomDataParameter.parse(packingMethodIdList.IsInclusive, packingMethodIdList.Values);
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.CustomParameters["@termOfPurchaseIdList"] = CustomDataParameter.parse(termOfPurchaseIdList.IsInclusive, termOfPurchaseIdList.Values);
            ad.SelectCommand.CustomParameters["@oprFabricTypeIdList"] = CustomDataParameter.parse(oprFabricTypeIdList.IsInclusive, oprFabricTypeIdList.Values);
            ad.SelectCommand.Parameters["@isNextMfgOrder"].Value = isNextMfgorder;
            ad.SelectCommand.Parameters["@isDualSourcingOrder"].Value = isDualSourcingOrder;
            ad.SelectCommand.Parameters["@isLDPOrder"].Value = isLDPOrder;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;

            ad.SelectCommand.Parameters["@LcNoFrom"].Value = lcNoFrom;
            ad.SelectCommand.Parameters["@LcNoTo"].Value = lcNoTo;
            ad.SelectCommand.Parameters["@PaymentTermId"].Value = paymentTermId;
            ad.SelectCommand.CustomParameters["@WorkflowStatusIdList"] = CustomDataParameter.parse(workflowStatusIdList.IsInclusive, workflowStatusIdList.Values);

            if (sortField != "")
            {
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY " + sortField;
            }

            WeeklyShipmentReportDs ds = new WeeklyShipmentReportDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public OutstandingLCReportDs getOutstandingLCReport(TypeCollector officeIdList, int handlingOfficeId, TypeCollector productTeamList, int vendorId,
            int countryOfOriginId, int customerDestinationId, int purchaseTermId, DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo)
        {
            OutstandingLCReportDs dataset = new OutstandingLCReportDs();
            IDataSetAdapter ad = getDataSetAdapter("OutstandingLCReportApt", "GetOutstandingLCReport");

            ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId;
            //ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            //ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;                                    
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.Parameters["@customerDestinationId"].Value = customerDestinationId;
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@purchaseTermId"].Value = purchaseTermId;

            if (customerAtWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@customerAtWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@customerAtWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@customerAtWHDateFrom"].Value = customerAtWHDateFrom;
                ad.SelectCommand.Parameters["@customerAtWHDateTo"].Value = customerAtWHDateTo;
            }

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }

        public OutstandingBookingReportDs getOutstandingBookingReport(TypeCollector officeIdList, TypeCollector productTeamList, int handlingOfficeId,
            int countryOfOriginId, TypeCollector shipmentMethodList, int packingMethodId, DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo,
            TypeCollector customerList, int vendorId, int termOfPurchaseId, int shipmentPortId, int isSampleOrder)
        {
            OutstandingBookingReportDs dataset = new OutstandingBookingReportDs();
            IDataSetAdapter ad = getDataSetAdapter("OutstandingBookingReportApt", "GetOutstandingBookingReport");

            //ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            //ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId;
            ad.SelectCommand.CustomParameters["@shipmentMethodList"] = CustomDataParameter.parse(shipmentMethodList.IsInclusive, shipmentMethodList.Values);
            ad.SelectCommand.Parameters["@packingMethodId"].Value = packingMethodId;

            if (customerAtWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@CustomerAtWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@CustomerAtWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@CustomerAtWHDateFrom"].Value = customerAtWHDateFrom;
                ad.SelectCommand.Parameters["@CustomerAtWHDateTo"].Value = customerAtWHDateTo;
            }
            ad.SelectCommand.Parameters["@shipmentPortId"].Value = shipmentPortId;
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerList.IsInclusive, customerList.Values);
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }

        public PartialShipmentReportDs getPartialShipmentReport(DateTime atWHDateFrom, DateTime atWHDateTo, TypeCollector officeIdList, TypeCollector productTeamList, int handlingOfficeId,
            int vendorId, int isNSLSZOrder, int isDualSourcingOrder, int isLDPOrder, int isSampleOrder, int isDFOrder, int isUTOrder, int termOfPurchaseId, int seasonId,
            TypeCollector countryOfOriginIdList, TypeCollector shipmentPortIdList, TypeCollector customerDestinationIdList, int oprFabricType,
            int packingMethodId, TypeCollector shipmentMethodList, TypeCollector customerIdList, DateTime bookInWHDateFrom, DateTime bookInWHDateTo, int paymentTermId, string sortingField)
        {
            PartialShipmentReportDs dataset = new PartialShipmentReportDs();
            IDataSetAdapter ad = getDataSetAdapter("PartialShipmentReportApt", "GetPartialShipmentReport");

            if (atWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@atWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@atWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@atWHDateFrom"].Value = atWHDateFrom;
                ad.SelectCommand.Parameters["@atWHDateTo"].Value = atWHDateTo;
            }
            if (bookInWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@bookInWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@bookInWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@bookInWHDateFrom"].Value = bookInWHDateFrom;
                ad.SelectCommand.Parameters["@bookInWHDateTo"].Value = bookInWHDateTo;
            }


            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@isNSLSZOrder"].Value = isNSLSZOrder;
            ad.SelectCommand.Parameters["@isDualSourcingOrder"].Value = isDualSourcingOrder;
            ad.SelectCommand.Parameters["@isLDPOrder"].Value = isLDPOrder;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;
            ad.SelectCommand.Parameters["@isDFOrder"].Value = isDFOrder;
            ad.SelectCommand.Parameters["@isUTOrder"].Value = isUTOrder;
            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            //ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId ;
            ad.SelectCommand.CustomParameters["@countryOfOriginIdList"] = CustomDataParameter.parse(countryOfOriginIdList.IsInclusive, countryOfOriginIdList.Values);
            ad.SelectCommand.Parameters["@seasonId"].Value = seasonId;
            //ad.SelectCommand.Parameters["@shipmentPortId"].Value = shipmentPortId ;
            //ad.SelectCommand.Parameters["@customerDestinationId"].Value = customerDestinationId ;
            ad.SelectCommand.CustomParameters["@shipmentPortIdList"] = CustomDataParameter.parse(shipmentPortIdList.IsInclusive, shipmentPortIdList.Values);
            ad.SelectCommand.CustomParameters["@customerDestinationIdList"] = CustomDataParameter.parse(customerDestinationIdList.IsInclusive, customerDestinationIdList.Values);
            ad.SelectCommand.Parameters["@oprFabricType"].Value = oprFabricType;
            ad.SelectCommand.Parameters["@packingMethodId"].Value = packingMethodId;
            ad.SelectCommand.Parameters["@paymentTermId"].Value = paymentTermId;
            ad.SelectCommand.CustomParameters["@shipmentMethodList"] = CustomDataParameter.parse(shipmentMethodList.IsInclusive, shipmentMethodList.Values);
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;

            if (sortingField != "")
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY " + sortingField;

            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }

        public EziBuyPartialShipmentReportDs getEziBuyPartialShipmentReport(DateTime atWHDateFrom, DateTime atWHDateTo, TypeCollector officeIdList, TypeCollector productTeamList,
            int vendorId, int isNSLSZOrder, int isDualSourcingOrder, int isLDPOrder, int isSampleOrder, int termOfPurchaseId, int countryOfOriginId, int seasonId, int shipmentPortId, int customerDestinationId, int phaseId,
            int packingMethodId, TypeCollector shipmentMethodList, DateTime bookInWHDateFrom, DateTime bookInWHDateTo, string sortingField)
        {
            EziBuyPartialShipmentReportDs dataset = new EziBuyPartialShipmentReportDs();
            IDataSetAdapter ad = getDataSetAdapter("EziBuyPartialShipmentReportApt", "GetEziBuyPartialShipmentReport");

            if (atWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@atWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@atWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@atWHDateFrom"].Value = atWHDateFrom;
                ad.SelectCommand.Parameters["@atWHDateTo"].Value = atWHDateTo;
            }
            if (bookInWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@bookInWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@bookInWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@bookInWHDateFrom"].Value = bookInWHDateFrom;
                ad.SelectCommand.Parameters["@bookInWHDateTo"].Value = bookInWHDateTo;
            }

            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@isNSLSZOrder"].Value = isNSLSZOrder;
            ad.SelectCommand.Parameters["@isDualSourcingOrder"].Value = isDualSourcingOrder;
            ad.SelectCommand.Parameters["@isLDPOrder"].Value = isLDPOrder;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;
            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId;
            ad.SelectCommand.Parameters["@seasonId"].Value = seasonId;
            ad.SelectCommand.Parameters["@shipmentPortId"].Value = shipmentPortId;
            ad.SelectCommand.Parameters["@customerDestinationId"].Value = customerDestinationId;
            ad.SelectCommand.Parameters["@phaseId"].Value = phaseId;
            ad.SelectCommand.Parameters["@packingMethodId"].Value = packingMethodId;
            ad.SelectCommand.CustomParameters["@shipmentMethodList"] = CustomDataParameter.parse(shipmentMethodList.IsInclusive, shipmentMethodList.Values);
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;

            if (sortingField != "")
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY " + sortingField;

            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }

        public OutstandingPaymentReportDs getOutstandingPaymentReport(DateTime shipReceiptDateFrom, DateTime shipReceiptDateTo, DateTime STWDateFrom,
            DateTime STWDateTo, DateTime invoiceDateFrom, DateTime invoiceDateTo, int vendorId, TypeCollector officeIdList, TypeCollector productTeamList,
            int termOfPurchaseId, int paymentTermId, ArrayList shippingusersId, int isSampleOrder, int isUTOrder, int isUploadDMS)
        {
            OutstandingPaymentReportDs dataset = new OutstandingPaymentReportDs();
            IDataSetAdapter ad = getDataSetAdapter("OutstandingPaymentReportApt", "GetOustandingPaymentReport");

            if (shipReceiptDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@shipReceiptDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@shipReceiptDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@shipReceiptDateFrom"].Value = shipReceiptDateFrom;
                ad.SelectCommand.Parameters["@shipReceiptDateTo"].Value = shipReceiptDateTo;
            }
            if (STWDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@STWDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@STWDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@STWDateFrom"].Value = STWDateFrom;
                ad.SelectCommand.Parameters["@STWDateTo"].Value = STWDateTo;
            }
            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }

            //ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@paymentTermId"].Value = paymentTermId;
            TypeCollector shippingIdCollector = TypeCollector.Inclusive;
            for (int i = 0; i < shippingusersId.Count; i++) shippingIdCollector.append(Convert.ToInt32(shippingusersId[i].ToString()));
            ad.SelectCommand.CustomParameters["@userIdList"] = CustomDataParameter.parse(shippingIdCollector.IsInclusive, shippingIdCollector.Values);
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;
            ad.SelectCommand.Parameters["@isUTOrder"].Value = isUTOrder;
            ad.SelectCommand.Parameters["@isUploadDMS"].Value = isUploadDMS;

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }


        public NSLSZOrderReportDs getNSLSZOrderReport(DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo, DateTime invoiceDateFrom, DateTime invoiceDateTo,
            DateTime invoiceUploadDateFrom, DateTime invoiceUploadDateTo, string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, TypeCollector officeIdList,
            TypeCollector productTeamList, int shipmentPortId, int customerDestinationId, int packingMethodId, TypeCollector shipmentMethodList, TypeCollector customerIdList,
            TypeCollector shipmentStatusList, int isSZUTOrder, int isDualSourcing, int isOPROrder, int isLDPOrder)
        {
            NSLSZOrderReportDs dataset = new NSLSZOrderReportDs();
            IDataSetAdapter ad = getDataSetAdapter("NSLSZOrderReportApt", "GetNSLSZOrderReport");

            if (customerAtWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@customerAtWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@customerAtWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@customerAtWHDateFrom"].Value = customerAtWHDateFrom;
                ad.SelectCommand.Parameters["@customerAtWHDateTo"].Value = customerAtWHDateTo;
            }
            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }
            if (invoiceUploadDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = invoiceUploadDateFrom;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = invoiceUploadDateTo;
            }

            if (invoicePrefix == String.Empty)
                ad.SelectCommand.Parameters["@invoicePrefix"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@invoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@invoiceSeqFrom"].Value = invoiceSeqFrom;
            ad.SelectCommand.Parameters["@invoiceSeqTo"].Value = invoiceSeqTo;
            ad.SelectCommand.Parameters["@invoiceYear"].Value = invoiceYear;

            ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);

            ad.SelectCommand.Parameters["@shipmentPortId"].Value = shipmentPortId;
            ad.SelectCommand.Parameters["@customerDestinationId"].Value = customerDestinationId;
            ad.SelectCommand.Parameters["@packingMethodId"].Value = packingMethodId;
            ad.SelectCommand.CustomParameters["@shipmentMethodList"] = CustomDataParameter.parse(shipmentMethodList.IsInclusive, shipmentMethodList.Values);
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);

            ad.SelectCommand.CustomParameters["@shipmentStatusList"] = CustomDataParameter.parse(shipmentStatusList.IsInclusive, shipmentStatusList.Values);
            ad.SelectCommand.Parameters["@isSZUTOrder"].Value = isSZUTOrder;
            ad.SelectCommand.Parameters["@isDualSourcing"].Value = isDualSourcing;
            ad.SelectCommand.Parameters["@isOPROrder"].Value = isOPROrder;
            ad.SelectCommand.Parameters["@isLDPOrder"].Value = isLDPOrder;

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }

        public ReceivablePayableForecastDs getReceivablePayableForecastReport(DateTime reportDate, ArrayList officeIdList, ArrayList paymentTermIdList)
        {
            int i;
            ReceivablePayableForecastDs dataset = new ReceivablePayableForecastDs();
            IDataSetAdapter ad = getDataSetAdapter("AccountReportApt", "getReceivablePayableForecastReport");

            if (reportDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ReportDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ReportDate"].Value = reportDate;

            TypeCollector OfficeCollector = TypeCollector.Inclusive;
            for (i = 0; i < officeIdList.Count; i++) OfficeCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(OfficeCollector.IsInclusive, OfficeCollector.Values);

            TypeCollector PaymentTermCollector = TypeCollector.Inclusive;
            for (i = 0; i < paymentTermIdList.Count; i++) PaymentTermCollector.append(Convert.ToInt32(paymentTermIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@paymentTermIdList"] = CustomDataParameter.parse(PaymentTermCollector.IsInclusive, PaymentTermCollector.Values);

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }

        public ActualSalesSummaryDs getActualSalesSummaryReport(int baseCurrencyId, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime invoiceUploadDateFrom,
            DateTime invoiceUploadDateTo, int fiscalYear, int periodFrom, int periodTo, int isActual, int isRealized, int officeId, TypeCollector officeIdList,
            int departmentId, TypeCollector productTeamList, int seasonId, int countryOfOriginId, int shipmentPortId, TypeCollector customerIdList,
            TypeCollector tradingAgencyList, int termOfPurchaseId, int isSZOrder, int isUTOrder, int isOPROrder, int isLDPOrder)
        {
            ActualSalesSummaryDs dataset = new ActualSalesSummaryDs();
            IDataSetAdapter ad = getDataSetAdapter("ActualSalesSummaryApt", "GetActualSalesSummaryReport");

            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }
            if (invoiceUploadDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceUploadDateFrom"].Value = invoiceUploadDateFrom;
                ad.SelectCommand.Parameters["@invoiceUploadDateTo"].Value = invoiceUploadDateTo;
            }
            ad.SelectCommand.Parameters["@fiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@isActual"].Value = isActual;
            ad.SelectCommand.Parameters["@isRealized"].Value = isRealized;
            ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@departmentId"].Value = departmentId;
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.Parameters["@seasonId"].Value = seasonId;
            ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId;
            ad.SelectCommand.Parameters["@shipmentPortId"].Value = shipmentPortId;
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.CustomParameters["@tradingAgencyList"] = CustomDataParameter.parse(tradingAgencyList.IsInclusive, tradingAgencyList.Values);
            ad.SelectCommand.Parameters["@termOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;
            ad.SelectCommand.Parameters["@isSZOrder"].Value = isSZOrder;
            ad.SelectCommand.Parameters["@isUTOrder"].Value = isUTOrder;
            ad.SelectCommand.Parameters["@isOPROrder"].Value = isOPROrder;
            ad.SelectCommand.Parameters["@isLDPOrder"].Value = isLDPOrder;
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }

        public ARAdjustmentNoteDs getARAdjustmentList(int adjustmentNoteId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ARAdjustmentNoteApt", "GetARAdjustmentNote");
            ad.SelectCommand.Parameters["@AdjustmentNoteId"].Value = adjustmentNoteId;

            ARAdjustmentNoteDs dataSet = new ARAdjustmentNoteDs();
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        public APAdjustmentNoteDs getAPAdjustmentList(int adjustmentNoteId)
        {
            IDataSetAdapter ad = getDataSetAdapter("APAdjustmentNoteApt", "GetAPAdjustmentNote");
            ad.SelectCommand.Parameters["@AdjustmentNoteId"].Value = adjustmentNoteId;

            APAdjustmentNoteDs dataSet = new APAdjustmentNoteDs();
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        public UKClaimDCNoteReportDs getUKClaimDCNote(int dcNoteId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteReportApt", "GetUKClaimDCNoteReport");
            ad.SelectCommand.Parameters["@DCNoteId"].Value = dcNoteId;

            UKClaimDCNoteReportDs dataSet = new UKClaimDCNoteReportDs();
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        /*
        public UKClaimDCNoteDs getUKClaimRefundDCNote(int claimRefundId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteReportApt", "GetUKClaimRefundDCNote");
            ad.SelectCommand.Parameters["@ClaimRefundId"].Value = claimRefundId;

            UKClaimDCNoteDs dataSet = new UKClaimDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }
        */

        public AccrualArchiveDs getAccrualArchiveList(int officeId, int fiscalYear, int period)
        {
            IDataSetAdapter ad = getDataSetAdapter("AccrualArchiveApt", "GetAccrualArchiveList");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@Period"].Value = period;

            AccrualArchiveDs dataSet = new AccrualArchiveDs();
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        public SupplierOrderStatusReportDs getSupplierOrderStatusReport(DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo,
            TypeCollector officeList, TypeCollector productTeamList, int vendorId, int paymentTermId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SupplierOrderStatusReportApt", "GetSupplierOrderStatusReport");
            if (customerAtWHDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@customerAtWHDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@customerAtWHDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@customerAtWHDateFrom"].Value = customerAtWHDateFrom;
                ad.SelectCommand.Parameters["@customerAtWHDateTo"].Value = customerAtWHDateTo;
            }

            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@paymentTermId"].Value = paymentTermId;

            SupplierOrderStatusReportDs dataSet = new SupplierOrderStatusReportDs();
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        public QADebitNoteDs getQADebitNoteDetail(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("QADebitNoteApt", "getQADebitNoteDetail");
            ad.SelectCommand.Parameters["@shipmentId"].Value = shipmentId;

            QADebitNoteDs dataSet = new QADebitNoteDs();
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        public SAndBCommissionDebitNoteDs getCommissionDebitNote(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SAndBCommDebitNoteApt", "CommDebitNoteDetail");
            ad.SelectCommand.Parameters["@shipmentId"].Value = shipmentId;

            SAndBCommissionDebitNoteDs dataSet = new SAndBCommissionDebitNoteDs();
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        public ReleaseLockDs getReleaseLockSummary(TypeCollector officeIdList, int handlingOfficeId, DateTime releaseLockDateFrom, DateTime releaseLockDateTo,
            int isSampleOrder, TypeCollector customerIdList, TypeCollector tradingAgencyIdList,
            int reversingEntryRequired, int dcNoteRequired, int ilsTempACRequired)
        {
            IDataSetAdapter ad = getDataSetAdapter("ReleaseLockSummaryApt", "getReleaseLockSummary");
            //ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.Parameters["@ReleaseLockDateFrom"].Value = releaseLockDateFrom;
            ad.SelectCommand.Parameters["@ReleaseLockDateTo"].Value = releaseLockDateTo;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;
            ad.SelectCommand.CustomParameters["@CustomerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.CustomParameters["@TradingAgencyIdList"] = CustomDataParameter.parse(tradingAgencyIdList.IsInclusive, tradingAgencyIdList.Values);
            ad.SelectCommand.Parameters["@ReversingEntryRequired"].Value = reversingEntryRequired;
            ad.SelectCommand.Parameters["@DCNoteRequired"].Value = dcNoteRequired;
            ad.SelectCommand.Parameters["@ILSTempACRequired"].Value = ilsTempACRequired;

            ReleaseLockDs dataset = new ReleaseLockDs();
            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }

        public ReleaseLockDs getReleaseLockReason(TypeCollector officeIdList, DateTime releaseLockDateFrom, DateTime releaseLockDateTo,
            int isSampleOrder, TypeCollector customerIdList, TypeCollector tradingAgencyIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("ReleaseLockReasonApt", "getReleaseLockReason");
            //ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);

            ad.SelectCommand.Parameters["@ReleaseLockDateFrom"].Value = releaseLockDateFrom;
            ad.SelectCommand.Parameters["@ReleaseLockDateTo"].Value = releaseLockDateTo;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;
            ad.SelectCommand.CustomParameters["@CustomerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.CustomParameters["@TradingAgencyIdList"] = CustomDataParameter.parse(tradingAgencyIdList.IsInclusive, tradingAgencyIdList.Values);

            ReleaseLockDs dataset = new ReleaseLockDs();
            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }

        public ArrayList GetPaymentReferenceCodeList()
        {
            int i;
            ArrayList list = new ArrayList();

            IDataSetAdapter ad = getDataSetAdapter("BankAccountApt", "getPaymentReferenceCodeList");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            for (i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                list.Add(dataSet.Tables[0].Rows[i][0].ToString());
            }

            return list;
        }


        public DataSet getCustomerReceiptReportByCriteria(DateTime receiptDateFrom, DateTime receiptDateTo, int fiscalYear, int periodFrom, int periodTo,
            int baseCurrencyId, ArrayList customerIdList, ArrayList currencyIdList,
            //ArrayList ReceiptReferenceNoList,
            string receiptReferenceNo, TypeCollector officeIdList, int handlingOfficeId, ArrayList seasonIdList,
            //int departmentId, 
            TypeCollector departmentIdList, int vendorId, int productTeamId,
            ArrayList tradingAgencyIdList, ArrayList purchaseTermIdList, ArrayList paymentTermIdList, int dateType, int sampleType, int userId, string version)
        {
            int i;

            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("CustomerReceiptReportApt", "getCustomerReceiptReportByCriteria");
            ad.SelectCommand.Parameters["@versionId"].Value = (version == "SUN" ? 0 : 1);

            if (receiptDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@receiptDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@receiptDateFrom"].Value = receiptDateFrom;

            if (receiptDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@receiptDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@receiptDateTo"].Value = receiptDateTo;

            if (fiscalYear == int.MinValue)
                ad.SelectCommand.Parameters["@fiscalYear"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@fiscalYear"].Value = fiscalYear;

            if (periodFrom == int.MinValue)
                ad.SelectCommand.Parameters["@periodFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;

            if (periodTo == int.MinValue)
                ad.SelectCommand.Parameters["@periodTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;

            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;

            TypeCollector customerIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < customerIdList.Count; i++) customerIdCollector.append(Convert.ToInt32(customerIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdCollector.IsInclusive, customerIdCollector.Values);

            TypeCollector currencyIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < currencyIdList.Count; i++) currencyIdCollector.append(Convert.ToInt32(currencyIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@currencyIdList"] = CustomDataParameter.parse(currencyIdCollector.IsInclusive, currencyIdCollector.Values);

            if (receiptReferenceNo == "")
                ad.SelectCommand.Parameters["@receiveRefCode"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@receiveRefCode"].Value = receiptReferenceNo;

            //TypeCollector officeIdCollector = TypeCollector.Inclusive;
            //for (i = 0; i < officeIdList.Count; i++) officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;

            TypeCollector seasonIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < seasonIdList.Count; i++) seasonIdCollector.append(Convert.ToInt32(seasonIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@seasonIdList"] = CustomDataParameter.parse(seasonIdCollector.IsInclusive, seasonIdCollector.Values);

            //if (productTeamId == -1)
            //    ad.SelectCommand.Parameters["@productTeamId"].Value = DBNull.Value;
            //else
            //    ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            //TypeCollector ProductTeamCollector = TypeCollector.Inclusive;
            //for (i = 0; i < productTeamIdList.Count; i++) ProductTeamCollector.append(Convert.ToInt32(productTeamIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@productTeamIdList"] = CustomDataParameter.parse(ProductTeamCollector.IsInclusive, ProductTeamCollector.Values);

            ad.SelectCommand.Parameters["@productTeamId"].Value = (productTeamId == int.MinValue ? -1 : productTeamId);
            ad.SelectCommand.Parameters["@userId"].Value = userId;

            TypeCollector purchaseTermIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < purchaseTermIdList.Count; i++) purchaseTermIdCollector.append(Convert.ToInt32(purchaseTermIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@purchaseTermIdList"] = CustomDataParameter.parse(purchaseTermIdCollector.IsInclusive, purchaseTermIdCollector.Values);

            TypeCollector tradingAgencyIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < tradingAgencyIdList.Count; i++) tradingAgencyIdCollector.append(Convert.ToInt32(tradingAgencyIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@tradingAgencyIdList"] = CustomDataParameter.parse(tradingAgencyIdCollector.IsInclusive, tradingAgencyIdCollector.Values);

            TypeCollector paymentTermIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < paymentTermIdList.Count; i++) paymentTermIdCollector.append(Convert.ToInt32(paymentTermIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@paymentTermIdList"] = CustomDataParameter.parse(paymentTermIdCollector.IsInclusive, paymentTermIdCollector.Values);

            //ad.SelectCommand.Parameters["@departmentId"].Value = departmentId;
            ad.SelectCommand.CustomParameters["@departmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);

            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@dateType"].Value = dateType;
            ad.SelectCommand.Parameters["@sampleType"].Value = sampleType;

            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 7200;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }


        public DataSet getSupplierPaymentReportByCriteria(DateTime PaymentDateFrom, DateTime PaymentDateTo, int fiscalYear, int periodFrom, int periodTo,
                int baseCurrencyId, ArrayList customerIdList, ArrayList currencyIdList, string PaymentReferenceNo, TypeCollector officeIdList, int handlingOfficeId, ArrayList seasonIdList,
                TypeCollector productTeamIdList, ArrayList tradingAgencyIdList, ArrayList purchaseTermIdList, ArrayList paymentTermIdList, TypeCollector departmentIdList, int vendorId, string version)
        {
            int i;

            IDataSetAdapter ad = null;
            if (version == "SUN")
                ad = getDataSetAdapter("AccountReportApt", "getSupplierPaymentByCriteria");
            else
                ad = getDataSetAdapter("AccountReportApt", "getSupplierPaymentByCriteria_Epicor");

            if (PaymentDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = PaymentDateFrom;

            if (PaymentDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = PaymentDateTo;

            if (fiscalYear == int.MinValue)
                ad.SelectCommand.Parameters["@fiscalYear"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@fiscalYear"].Value = fiscalYear;

            if (periodFrom == int.MinValue)
                ad.SelectCommand.Parameters["@periodFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;

            if (periodTo == int.MinValue)
                ad.SelectCommand.Parameters["@periodTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;

            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;

            //ad.SelectCommand.Parameters["@departmentId"].Value = departmentId;
            ad.SelectCommand.CustomParameters["@departmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@vendorId"].Value = vendorId;

            TypeCollector customerIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < customerIdList.Count; i++) customerIdCollector.append(Convert.ToInt32(customerIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdCollector.IsInclusive, customerIdCollector.Values);

            TypeCollector currencyIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < currencyIdList.Count; i++) currencyIdCollector.append(Convert.ToInt32(currencyIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@currencyIdList"] = CustomDataParameter.parse(currencyIdCollector.IsInclusive, currencyIdCollector.Values);

            if (PaymentReferenceNo == "")
                ad.SelectCommand.Parameters["@paymentRefCode"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@paymentRefCode"].Value = PaymentReferenceNo;

            //TypeCollector officeIdCollector = TypeCollector.Inclusive;
            //for (i = 0; i < officeIdList.Count; i++) officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@handlingOfficeId"].Value = handlingOfficeId;

            TypeCollector seasonIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < seasonIdList.Count; i++) seasonIdCollector.append(Convert.ToInt32(seasonIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@seasonIdList"] = CustomDataParameter.parse(seasonIdCollector.IsInclusive, seasonIdCollector.Values);

            //if (productTeamId == -1)
            //    ad.SelectCommand.Parameters["@productTeamId"].Value = DBNull.Value;
            //else
            //    ad.SelectCommand.Parameters["@productTeamId"].Value = productTeamId;
            //TypeCollector ProductTeamCollector = TypeCollector.Inclusive;
            //for (i = 0; i < productTeamIdList.Count; i++) ProductTeamCollector.append(Convert.ToInt32(productTeamIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@productTeamIdList"] = CustomDataParameter.parse(ProductTeamCollector.IsInclusive, ProductTeamCollector.Values);
            ad.SelectCommand.CustomParameters["@productTeamIdList"] = CustomDataParameter.parse(productTeamIdList.IsInclusive, productTeamIdList.Values);

            TypeCollector purchaseTermIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < purchaseTermIdList.Count; i++) purchaseTermIdCollector.append(Convert.ToInt32(purchaseTermIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@purchaseTermIdList"] = CustomDataParameter.parse(purchaseTermIdCollector.IsInclusive, purchaseTermIdCollector.Values);

            TypeCollector tradingAgencyIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < tradingAgencyIdList.Count; i++) tradingAgencyIdCollector.append(Convert.ToInt32(tradingAgencyIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@tradingAgencyIdList"] = CustomDataParameter.parse(tradingAgencyIdCollector.IsInclusive, tradingAgencyIdCollector.Values);

            TypeCollector paymentTermIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < paymentTermIdList.Count; i++) paymentTermIdCollector.append(Convert.ToInt32(paymentTermIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@paymentTermIdList"] = CustomDataParameter.parse(paymentTermIdCollector.IsInclusive, paymentTermIdCollector.Values);

            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public DataSet getLCBatchContolReport(int lcBatchNoFrom, int lcBatchNoTo, DateTime lcBatchDateFrom, DateTime lcBatchDateTo,
                    DateTime lcIssueDateFrom, DateTime lcIssueDateTo, DateTime lcExpiryDateFrom, DateTime lcExpiryDateTo,
                    int coId, TypeCollector officeIdList, int handlingOfficeId, int vendorId, int lcApplicationNoFrom, int lcApplicationNoTo, DateTime lcApplicationDateFrom, DateTime lcApplicationDateTo,
                    string lcNoFrom, string lcNoTo)
        {
            int i;

            IDataSetAdapter ad = getDataSetAdapter("LCReportApt", "GetLCBatchControlReport");

            //TypeCollector officeIdCollector = TypeCollector.Inclusive;
            //for (i = 0; i < officeIdList.Count; i++) officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;

            if (vendorId == int.MinValue)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            if (coId == int.MinValue)
                ad.SelectCommand.Parameters["CoId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["CoId"].Value = coId;

            //TypeCollector coIdCollector = TypeCollector.Inclusive;
            //for (i = 0; i < coIdList.Count; i++) coIdCollector.append(Convert.ToInt32(coIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@COIdList"] = CustomDataParameter.parse(coIdCollector.IsInclusive, coIdCollector.Values);

            if (lcBatchNoFrom == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = lcBatchNoFrom;

            if (lcBatchNoTo == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = lcBatchNoTo;


            if (lcBatchDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@LCBatchDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchDateFrom"].Value = lcBatchDateFrom;

            if (lcBatchDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@LCBatchDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchDateTo"].Value = lcBatchDateTo;


            if (lcIssueDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = lcIssueDateFrom;

            if (lcIssueDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = lcIssueDateTo;


            if (lcExpiryDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcExpiryDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcExpiryDateFrom"].Value = lcExpiryDateFrom;

            if (lcExpiryDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcExpiryDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcExpiryDateTo"].Value = lcExpiryDateTo;

            ad.SelectCommand.Parameters["@LCApplicationNoFrom"].Value = lcApplicationNoFrom;
            ad.SelectCommand.Parameters["@LCApplicationNoTo"].Value = lcApplicationNoTo;

            if (lcApplicationDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@LCApplicationDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@LCApplicationDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@LCApplicationDateFrom"].Value = lcApplicationDateFrom;
                ad.SelectCommand.Parameters["@LCApplicationDateTo"].Value = lcApplicationDateTo;
            }

            if (lcNoFrom == "")
            {
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@LCNoTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = lcNoFrom;
                ad.SelectCommand.Parameters["@LCNoTo"].Value = lcNoTo;
            }

            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public LCStatusReportDs getLCStatusReport(int lcBatchNoFrom, int lcBatchNoTo, string lcNoFrom, string lcNoTo, string lcBillRefNoFrom, string lcBillRefNoTo,
                    DateTime lcIssueDateFrom, DateTime lcIssueDateTo, DateTime lcExpiryDateFrom, DateTime lcExpiryDateTo,
                    DateTime lcPaymentCheckDateFrom, DateTime lcPaymentCheckDateTo, int coId, TypeCollector officeIdList,
                    int handlingOfficeId, ArrayList productTeamIdList, int vendorId, int isChinaGBTestRequired, int chinaGBTestResult)
        {
            int i;

            IDataSetAdapter ad = getDataSetAdapter("LCStatusReportApt", "GetLCStatusReport");

            //TypeCollector officeIdCollector = TypeCollector.Inclusive;
            //for (i = 0; i < officeIdList.Count; i++) officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;

            TypeCollector productTeamIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < productTeamIdList.Count; i++) productTeamIdCollector.append(Convert.ToInt32(productTeamIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@ProductTeamIdList"] = CustomDataParameter.parse(productTeamIdCollector.IsInclusive, productTeamIdCollector.Values);

            if (vendorId == int.MinValue)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            if (coId == int.MinValue)
                ad.SelectCommand.Parameters["CoId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["CoId"].Value = coId;

            //TypeCollector coIdCollector = TypeCollector.Inclusive;
            //for (i = 0; i < coIdList.Count; i++) coIdCollector.append(Convert.ToInt32(coIdList[i].ToString()));
            //ad.SelectCommand.CustomParameters["@COIdList"] = CustomDataParameter.parse(coIdCollector.IsInclusive, coIdCollector.Values);

            if (lcBatchNoFrom == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = lcBatchNoFrom;

            if (lcBatchNoTo == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = lcBatchNoTo;


            if (lcNoFrom == "")
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = lcNoFrom;

            if (lcNoTo == "")
                ad.SelectCommand.Parameters["@LCNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoTo"].Value = lcNoTo;

            if (lcBillRefNoFrom == "")
                ad.SelectCommand.Parameters["@LCBillRefNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBillRefNoFrom"].Value = lcBillRefNoFrom;

            if (lcBillRefNoTo == "")
                ad.SelectCommand.Parameters["@LCBillRefNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBillRefNoTo"].Value = lcBillRefNoTo;


            if (lcIssueDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = lcIssueDateFrom;

            if (lcIssueDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = lcIssueDateTo;


            if (lcExpiryDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcExpiryDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcExpiryDateFrom"].Value = lcExpiryDateFrom;

            if (lcExpiryDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcExpiryDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcExpiryDateTo"].Value = lcExpiryDateTo;

            if (lcPaymentCheckDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcPaymentCheckDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcPaymentCheckDateFrom"].Value = lcPaymentCheckDateFrom;

            if (lcPaymentCheckDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcPaymentCheckDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcPaymentCheckDateTo"].Value = lcPaymentCheckDateTo;

            ad.SelectCommand.Parameters["@isChinaGBTestRequired"].Value = isChinaGBTestRequired.ToString();

            //DataSet ds = new DataSet();
            LCStatusReportDs ds = new LCStatusReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(ds);

            //Filter the China GB Test Result


            LCStatusReportDs.LCStatusReportRow row;
            for (i = ds.LCStatusReport.Rows.Count - 1; i >= 0; i--)
            {
                row = (LCStatusReportDs.LCStatusReportRow)ds.LCStatusReport.Rows[i];
                if (row.IsChinaGBTestRequired == 1)
                    row.ChinaGBTestResult = GeneralWorker.Instance.getChinaGBTestResult(row.ProductId, row.VendorId);
                //row.IsChinaGBTestRequired = (GeneralWorker.Instance.isChinaGBTestRequired(row.CustomerId, row.SeasonId, row.ProductId) ? 1 : 0);
                //if (!((chinaGBTestResult == -999 || row.ChinaGBTestResult == chinaGBTestResult) && (isChinaGBTestRequired == -999 || row.IsChinaGBTestRequired == isChinaGBTestRequired)))
                if (!((chinaGBTestResult == -999 || row.ChinaGBTestResult == chinaGBTestResult) && (isChinaGBTestRequired == -999 || row.IsChinaGBTestRequired == row.IsChinaGBTestRequired)))
                {
                    row.BeginEdit();
                    row.Delete();
                    row.EndEdit();
                    row.AcceptChanges();
                }
            }

            return ds;
        }


        public LCStatusReportDs getLCStatusReport(int vendorId, string lcNoFrom, string lcNoTo, string lcBillRefNo, int userId)
        {
            // all input parameter is mandatory
            IDataSetAdapter ad = getDataSetAdapter("LCStatusReportApt", "GetLCStatusReportForPaymentControl");

            //ad.SelectCommand.CustomParameters["@ShipmentIdList"] = CustomDataParameter.parse(shipmentIdList.IsInclusive, shipmentIdList.Values);
            if (vendorId == int.MinValue)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            if (lcNoFrom == "")
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = lcNoFrom;

            if (lcNoTo == "")
                ad.SelectCommand.Parameters["@LCNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoTo"].Value = lcNoTo;

            if (lcBillRefNo == "")
                ad.SelectCommand.Parameters["@LCBillRefNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBillRefNo"].Value = lcBillRefNo;

            if (userId == int.MinValue)
                ad.SelectCommand.Parameters["@UserId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@UserId"].Value = userId;

            //DataSet ds = new DataSet();
            LCStatusReportDs ds = new LCStatusReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            //Fill in the China GB Test Result
            LCStatusReportDs.LCStatusReportRow row;
            for (int i = ds.LCStatusReport.Rows.Count - 1; i >= 0; i--)
            {
                row = (LCStatusReportDs.LCStatusReportRow)ds.LCStatusReport.Rows[i];
                if (row.IsChinaGBTestRequired == 1)
                    row.ChinaGBTestResult = GeneralWorker.Instance.getChinaGBTestResult(row.ProductId, row.VendorId);
                //row.ChinaGBTestResult = GeneralWorker.Instance.getChinaGBTestResult(row.SeasonId, row.ProductId);
                //row.IsChinaGBTestRequired = (GeneralWorker.Instance.isChinaGBTestRequired(row.CustomerId, row.SeasonId, row.ProductId) ? 1 : 0);
            }

            return ds;
        }


        public DataSet getLCNewApplicationAllocation()
        {

            IDataSetAdapter ad = getDataSetAdapter("LCReportApt", "GetLCNewApplicationAllocationReport");

            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }


        public DataSet getLCApplicationSummaryReport(DateTime lcAppDateFrom, DateTime lcAppDateTo, int lcAppNoFrom, int lcAppNoTo, string lcNoFrom, string lcNoTo,
                TypeCollector customerIdList, TypeCollector officeIdList, int handlingOfficeId, int vendorId, int lcDetailUpdated, int userId)
        {
            DataSet ds = new DataSet();
            IDataSetAdapter ad = getDataSetAdapter("LCReportApt", "GetLCApplicationSummaryReport");

            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.CustomParameters["@CustomerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);

            if (vendorId == int.MinValue)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            if (lcAppDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@LCApplicationDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCApplicationDateFrom"].Value = lcAppDateFrom;
            if (lcAppDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@LCApplicationDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCApplicationDateTo"].Value = lcAppDateTo;


            if (lcAppNoFrom == int.MinValue)
                ad.SelectCommand.Parameters["@LCApplicationNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCApplicationNoFrom"].Value = lcAppNoFrom;
            if (lcAppNoTo == int.MinValue)
                ad.SelectCommand.Parameters["@LCApplicationNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCApplicationNoTo"].Value = lcAppNoTo;


            if (lcNoFrom == "")
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = lcNoFrom;
            if (lcNoTo == "")
                ad.SelectCommand.Parameters["@LCNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoTo"].Value = lcNoTo;

            ad.SelectCommand.Parameters["@LCDetailUpdated"].Value = lcDetailUpdated;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public LCShipmentAmendmentReportDs getLCShipmentAmendmentReport(string lcNoFrom, string lcNoTo, int lcBatchNoFrom, int lcBatchNoTo,
            int lcApplicationNoFrom, int lcApplicationNoTo, DateTime lcIssueDateFrom, DateTime lcIssueDateTo,
            DateTime supplierAwhDateFrom, DateTime supplierAwhDateTo, TypeCollector officeIdList, int handlingOfficeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCShipmentAmendmentReportApt", "GetLCShipmentAmendmentReport");

            if (lcBatchNoFrom == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = lcBatchNoFrom;

            if (lcBatchNoTo == int.MinValue)
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = lcBatchNoTo;

            if (lcNoFrom == "")
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = lcNoFrom;

            if (lcNoTo == "")
                ad.SelectCommand.Parameters["@LCNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoTo"].Value = lcNoTo;

            if (lcApplicationNoFrom == int.MinValue)
                ad.SelectCommand.Parameters["@LCApplicationNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCApplicationNoFrom"].Value = lcApplicationNoFrom;

            if (lcApplicationNoTo == int.MinValue)
                ad.SelectCommand.Parameters["@LCApplicationNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCApplicationNoTo"].Value = lcApplicationNoTo;


            if (lcIssueDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateFrom"].Value = lcIssueDateFrom;

            if (lcIssueDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@lcIssueDateTo"].Value = lcIssueDateTo;


            if (supplierAwhDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@supplierAwhDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@supplierAwhDateFrom"].Value = supplierAwhDateFrom;

            if (supplierAwhDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@supplierAwhDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@supplierAwhDateTo"].Value = supplierAwhDateTo;

            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;

            LCShipmentAmendmentReportDs ds = new LCShipmentAmendmentReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);
            return ds;
        }

        public OrdersForLCCancellationReportDs getOrdersForLCCancellationReport(int officeId, DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo)
        {
            IDataSetAdapter ad = getDataSetAdapter("OrdersForLCCancellationReportApt", "GetOrdersForLCCancellationReport");

            ad.SelectCommand.Parameters["@officeId"].Value = officeId;
            ad.SelectCommand.Parameters["@customerAtWHDateFrom"].Value = customerAtWHDateFrom;
            ad.SelectCommand.Parameters["@customerAtWHDateTo"].Value = customerAtWHDateTo;

            OrdersForLCCancellationReportDs ds = new OrdersForLCCancellationReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);
            return ds;
        }

        public DataSet getOtherCostSummary(DateTime InvoiceDateFrom, DateTime InvoiceDateTo, int FiscalYear, int PeriodNoFrom, int PeriodNoTo,
                DateTime DeliveryDateFrom, DateTime DeliveryDateTo, TypeCollector OfficeIdList, int HandlingOfficeId, int CountryOfOriginId, int SeasonId, TypeCollector TradingAgencyIdList, TypeCollector PurchaseTermIdList,
                int GetActual, int GetAccrual, int GetRealized, int GetNotYetCut, int BaseCurrencyId)
        {
            DataSet ds = new DataSet();
            //IDataSetAdapter ad = getDataSetAdapter("AccountReportApt", "getOtherCostSummaryReport");
            IDataSetAdapter ad = getDataSetAdapter("OtherCostSummaryApt", "getOtherCostSummaryReport");

            if (InvoiceDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = InvoiceDateFrom;
            if (InvoiceDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = InvoiceDateTo;

            if (DeliveryDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeliveryDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeliveryDateFrom"].Value = DeliveryDateFrom;
            if (DeliveryDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeliveryDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeliveryDateTo"].Value = DeliveryDateTo;

            if (FiscalYear == int.MinValue)
                ad.SelectCommand.Parameters["@FiscalYear"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FiscalYear"].Value = FiscalYear;
            if (PeriodNoFrom == int.MinValue)
                ad.SelectCommand.Parameters["@PeriodNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PeriodNoFrom"].Value = PeriodNoFrom;
            if (PeriodNoTo == int.MinValue)
                ad.SelectCommand.Parameters["@PeriodNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PeriodNoTo"].Value = PeriodNoTo;

            if (BaseCurrencyId == int.MinValue)
                ad.SelectCommand.Parameters["@BaseCurrencyId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@BaseCurrencyId"].Value = BaseCurrencyId;

            ad.SelectCommand.Parameters["@GetActual"].Value = (GetActual == int.MinValue ? 0 : GetActual);
            ad.SelectCommand.Parameters["@GetAccrual"].Value = (GetActual == int.MinValue ? 0 : GetAccrual);
            ad.SelectCommand.Parameters["@GetRealized"].Value = (GetActual == int.MinValue ? 0 : GetRealized);
            ad.SelectCommand.Parameters["@GetNotYetCut"].Value = (GetActual == int.MinValue ? 0 : GetNotYetCut);

            ad.SelectCommand.Parameters["@CountryOfOriginId"].Value = CountryOfOriginId;
            ad.SelectCommand.Parameters["@SeasonId"].Value = SeasonId;
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = HandlingOfficeId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(OfficeIdList.IsInclusive, OfficeIdList.Values);
            ad.SelectCommand.CustomParameters["@TradingAgencyIdList"] = CustomDataParameter.parse(TradingAgencyIdList.IsInclusive, TradingAgencyIdList.Values);
            ad.SelectCommand.CustomParameters["@PurchaseTermIdList"] = CustomDataParameter.parse(PurchaseTermIdList.IsInclusive, PurchaseTermIdList.Values);

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            for (int i = 0; i < ds.Tables.Count && i <= 1; i++)
                if (ds.Tables[i].TableName != "OtherCostSummary")
                    ds.Tables[i].TableName = "OtherCostSunAccountCode";
            return ds;
        }


        public DataSet getMonthEndSummary(int FiscalYear, int PeriodNo, int isSampleOrder,
            TypeCollector OfficeIdList, TypeCollector TradingAgencyIdList, TypeCollector PurchaseTermIdList)
        {
            DataSet ds = new DataSet();
            IDataSetAdapter ad = getDataSetAdapter("AccountReportApt", "getMonthEndSummaryReport");

            if (FiscalYear == int.MinValue)
                ad.SelectCommand.Parameters["@FiscalYear"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FiscalYear"].Value = FiscalYear;
            if (PeriodNo == int.MinValue)
                ad.SelectCommand.Parameters["@PeriodNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PeriodNo"].Value = PeriodNo;
            ad.SelectCommand.Parameters["@isSampleOrder"].Value = isSampleOrder;

            //if (BaseCurrencyId == int.MinValue)
            //    ad.SelectCommand.Parameters["@BaseCurrencyId"].Value = DBNull.Value;
            //else
            //    ad.SelectCommand.Parameters["@BaseCurrencyId"].Value = BaseCurrencyId;

            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(OfficeIdList.IsInclusive, OfficeIdList.Values);
            ad.SelectCommand.CustomParameters["@TradingAgencyIdList"] = CustomDataParameter.parse(TradingAgencyIdList.IsInclusive, TradingAgencyIdList.Values);
            ad.SelectCommand.CustomParameters["@PurchaseTermIdList"] = CustomDataParameter.parse(PurchaseTermIdList.IsInclusive, PurchaseTermIdList.Values);

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }


        public STWDateDiscrepancyDs getSTWDateDiscrepancyByCriteria(int budgetYear, int periodFrom, int periodTo,
            DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime ilsStwDateFrom, DateTime ilsStwDateTo, DateTime actualStwDateFrom, DateTime actualStwDateTo,
            int countryOfOriginId, int shipmentPortId, TypeCollector officeIdList, TypeCollector productTeamList, TypeCollector customerIdList, TypeCollector tradingAgencyList,
            int baseCurrencyId, string sortingField)
        {
            IDataSetAdapter ad = getDataSetAdapter("STWDateDiscrepancyApt", "GetSTWDateDiscrepancyByCriteria");

            ad.SelectCommand.Parameters["@baseCurrencyId"].Value = baseCurrencyId;

            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@invoiceDateTo"].Value = invoiceDateTo;
            }
            if (ilsStwDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@IlsStwDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@IlsStwDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@IlsStwDateFrom"].Value = ilsStwDateFrom;
                ad.SelectCommand.Parameters["@IlsStwDateTo"].Value = ilsStwDateTo;
            }
            if (actualStwDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@ActualStwDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@ActualStwDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@ActualStwDateFrom"].Value = actualStwDateFrom;
                ad.SelectCommand.Parameters["@ActualStwDateTo"].Value = actualStwDateTo.AddDays(1);
            }
            ad.SelectCommand.CustomParameters["@officeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@productTeamList"] = CustomDataParameter.parse(productTeamList.IsInclusive, productTeamList.Values);
            ad.SelectCommand.CustomParameters["@customerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.CustomParameters["@tradingAgencyList"] = CustomDataParameter.parse(tradingAgencyList.IsInclusive, tradingAgencyList.Values);
            ad.SelectCommand.Parameters["@budgetYear"].Value = budgetYear;
            ad.SelectCommand.Parameters["@periodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@periodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@countryOfOriginId"].Value = countryOfOriginId;
            ad.SelectCommand.Parameters["@shipmentPortId"].Value = shipmentPortId;

            if (sortingField != "")
                ad.SelectCommand.DbCommand.CommandText += " ORDER BY " + sortingField;

            STWDateDiscrepancyDs dataSet = new STWDateDiscrepancyDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public EziBuyOSPaymentDs getEziBuyAllOSPaymentReport(string Consignee)
        {   // Consignee: 'GT'  - Golbal Textile -> Phase 2 or before
            //          : 'EB'  - Ezibuy         -> Phase 3 on onward
            //          : EMPTY                  -> All season & phase

            TypeCollector OfficeIdList;
            ArrayList list = CommonUtil.getOfficeListByUserId(GeneralCriteria.ALL, GeneralCriteria.ALL);
            OfficeIdList = TypeCollector.Inclusive;
            for (int i = 0; i < list.Count; i++)
                OfficeIdList.append(((OfficeRef)list[i]).OfficeId);
            IDataSetAdapter ad = getDataSetAdapter("EziBuyOSPaymentReportApt", "GetEziBuyAllOSPaymentReport");
            ad.SelectCommand.Parameters["@UserId"].Value = -1;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(OfficeIdList.IsInclusive, OfficeIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = "";
            ad.SelectCommand.Parameters["@ExtractMode"].Value = "NOTICE";
            ad.SelectCommand.Parameters["@SeasonIdFrom"].Value = -1;
            ad.SelectCommand.Parameters["@SeasonIdTo"].Value = -1;
            ad.SelectCommand.Parameters["@PhaseIdFrom"].Value = -1;
            ad.SelectCommand.Parameters["@PhaseIdTo"].Value = -1;
            if (Consignee == "GT")
                //ad.SelectCommand.Parameters["@SeasonIdTo"].Value = 17;
                ad.SelectCommand.Parameters["@PhaseIdTo"].Value = 2;
            else
                if (Consignee == "EB")
                //ad.SelectCommand.Parameters["@SeasonIdFrom"].Value = 18;
                ad.SelectCommand.Parameters["@PhaseIdFrom"].Value = 3;

            EziBuyOSPaymentDs ds = new EziBuyOSPaymentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            return ds;
        }

        public EziBuyOSPaymentDs getEziBuyAllOSPaymentList(int UserId, TypeCollector OfficeIdList, string DepartmentCode)
        {
            DateTime date;

            IDataSetAdapter ad = getDataSetAdapter("EziBuyOSPaymentReportApt", "GetEziBuyAllOSPaymentReport");
            ad.SelectCommand.Parameters["@UserId"].Value = UserId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(OfficeIdList.IsInclusive, OfficeIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = DepartmentCode;
            ad.SelectCommand.Parameters["@ExtractMode"].Value = "ALERT";
            ad.SelectCommand.Parameters["@SeasonIdFrom"].Value = -1;    // All season
            ad.SelectCommand.Parameters["@SeasonIdTo"].Value = -1;
            ad.SelectCommand.Parameters["@PhaseIdFrom"].Value = -1;
            ad.SelectCommand.Parameters["@PhaseIdTo"].Value = -1;

            EziBuyOSPaymentDs ds = new EziBuyOSPaymentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
                loadGoldSealApprovalDateToEzibuyOSPaymentDaataSet(ds);
            /*
            foreach (EziBuyOSPaymentDs.EziBuyOSPaymentRow row in ds.EziBuyOSPayment)
            {
                date = AlertNotificationWorker.Instance.getGoldSealApprovalDate(row.ShipmentId);
                if (date != DateTime.MinValue)
                    row.ApprovalDate = date;
            }
            */
            return ds;
        }

        public void loadGoldSealApprovalDateToEzibuyOSPaymentDaataSet(EziBuyOSPaymentDs EzibuyDs)
        {
            TypeCollector shipmentIdList = TypeCollector.Inclusive;
            foreach (EziBuyOSPaymentDs.EziBuyOSPaymentRow r in EzibuyDs.EziBuyOSPayment)
                shipmentIdList.append(r.ShipmentId);

            NssProgressDs ds = new NssProgressDs();
            IDataSetAdapter ad = getDataSetAdapter("ProductionProgressApt", "GetGoldSealProgressByShipmentIdList");
            ad.SelectCommand.CustomParameters["@ShipmentIdList"] = CustomDataParameter.parse(shipmentIdList.IsInclusive, shipmentIdList.Values);
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (EziBuyOSPaymentDs.EziBuyOSPaymentRow row in EzibuyDs.EziBuyOSPayment)
                    foreach (NssProgressDs.ProgressRow pg in ds.Progress)
                        if (row.ShipmentId == pg.ShipmentId)
                        {
                            if (!pg.IsActualDateNull())
                                row.ApprovalDate = pg.ActualDate;
                            //else
                            //    row.ApprovalDate = DateTime.MinValue;
                            break;
                        }
            }
            return;
        }


        public string getBeneficiaryAccountNoList(int officeId, int currencyId)
        {
            StringBuilder s = new StringBuilder();
            IDataSetAdapter ad = getDataSetAdapter("DebitNoteToNUKParamApt", "GetBeneficiaryAccountNoList");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;

            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected < 1)
                return "N/A";
            else
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                    s.AppendLine((string)r[0]);
                return s.ToString();
            }
        }

        public DebitNoteToNUKParamDef getDebitNoteToNUKParamByKey(int officeId, int currencyId)
        {
            IDataSetAdapter ad = getDataSetAdapter("DebitNoteToNUKParamApt", "GetDebitNoteToNUKParamByKey");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;

            DebitNoteToNUKParamDef def = null;
            DebitNoteToNUKParamDs ds = new DebitNoteToNUKParamDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                def = new DebitNoteToNUKParamDef();
                DebitNoteToNUKParamDs.DebitNoteToNUKParamRow r = ds.DebitNoteToNUKParam[0];
                StringBuilder s = new StringBuilder();
                def.OfficeId = r.OfficeId;
                def.CurrencyId = r.CurrencyId;
                def.SupplierCode = r.SupplierCode;
                def.BeneficiaryAccountNo = r.BeneficiaryAccountNo;
                def.BeneficiaryName = r.BeneficiaryName;
                def.BankName = r.BankName;
                def.SwiftCode = r.SwiftCode;
                if (!r.IsBankAddr1Null()) s.AppendLine(r.BankAddr1);
                if (!r.IsBankAddr2Null()) s.AppendLine(r.BankAddr2);
                if (!r.IsBankAddr3Null()) s.AppendLine(r.BankAddr3);
                if (!r.IsBankAddr4Null()) s.AppendLine(r.BankAddr4);
                def.BankAddress = s.ToString();
            }
            return def;
        }


        public ActiveSupplierReportDs getActiveSupplierReport(int officeGroupId, int productTeamId, DateTime pastDeliveryFrom,
            DateTime deliveryDateFrom, DateTime deliveryDateTo,
            int minDelivery, int vendorId, int orderType, TypeCollector customerIdList, TypeCollector workflowStatusList)
        {
            IDataSetAdapter ad = getDataSetAdapter("ActiveSupplierReportApt", "getActiveSupplierReport");
            ad.SelectCommand.Parameters["@OfficeGroupId"].Value = officeGroupId;
            ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
            ad.SelectCommand.Parameters["@DeliveryDateFrom"].Value = deliveryDateFrom;
            ad.SelectCommand.Parameters["@DeliveryDateTo"].Value = deliveryDateTo;
            ad.SelectCommand.Parameters["@PastDeliveryFrom"].Value = pastDeliveryFrom;
            ad.SelectCommand.Parameters["@OrderType"].Value = orderType;
            ad.SelectCommand.CustomParameters["@CustomerIdList"] = CustomDataParameter.parse(customerIdList.IsInclusive, customerIdList.Values);
            ad.SelectCommand.CustomParameters["@WorkflowStatusIdList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);
            ad.SelectCommand.Parameters["@MinDelivery"].Value = minDelivery;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            ActiveSupplierReportDs dataset = new ActiveSupplierReportDs();
            int recordsAffected = ad.Fill(dataset);

            return dataset;
        }


        public ContainerManifestReportDs getContainerManifest(string voyageNo, DateTime departDate, string departPort, string vesselName, string contractNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContainerManifestApt", "GetContainerManifest");
            ad.SelectCommand.Parameters["@voyageNo"].Value = voyageNo;
            if (departDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@departDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@departDate"].Value = departDate;

            ad.SelectCommand.Parameters["@departPort"].Value = departPort;
            ad.SelectCommand.Parameters["@vesselName"].Value = vesselName;
            ad.SelectCommand.Parameters["@contractNo"].Value = contractNo;

            ContainerManifestReportDs ds = new ContainerManifestReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            return ds;
        }


        public UKClaimSummaryReportDs getUKClaimReport(DateTime ukDNDateFrom, DateTime ukDNDateTo, int fiscalYear, int periodFrom, int periodTo, TypeCollector officeIdList, int productTeamId,
                int vendorId, TypeCollector claimTypeIdList, int claimReasonId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimReportApt", "GetUKClaimSummaryReport");
            if (ukDNDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@UKDNDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@UKDNDateFrom"].Value = ukDNDateFrom;
            if (ukDNDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@UKDNDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@UKDNDateTo"].Value = ukDNDateTo;
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@PeriodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@PeriodTo"].Value = periodTo;

            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
            //ad.SelectCommand.Parameters["@ClaimTypeId"].Value = claimTypeId;
            ad.SelectCommand.CustomParameters["@ClaimTypeIdList"] = CustomDataParameter.parse(claimTypeIdList.IsInclusive, claimTypeIdList.Values);
            //ad.SelectCommand.CustomParameters["@ClaimReasonIdList"] = CustomDataParameter.parse(claimReasonIdList.IsInclusive, claimReasonIdList.Values);
            ad.SelectCommand.Parameters["@ClaimReasonId"].Value = claimReasonId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(claimTypeIdList.IsInclusive, officeIdList.Values);
            UKClaimSummaryReportDs ds = new UKClaimSummaryReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            ad.SelectCommand.MailSQL = true;

            int recordsAffected = ad.Fill(ds);
            return ds;
        }

        public void fillUKClaimRequest(UKClaimSummaryReportDs ds, TypeCollector claimRequestIdList, int claimReasonId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimRequestReportApt", "GetUKClaimReportRequestList");
            ad.SelectCommand.CustomParameters["@ClaimRequestIdList"] = CustomDataParameter.parse(claimRequestIdList.IsInclusive, claimRequestIdList.Values);
            ad.SelectCommand.Parameters["@ClaimReasonId"].Value = claimReasonId;
            //UKClaimSummaryReportDs ds = new UKClaimSummaryReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            //ad.SelectCommand.MailSQL = true;

            int recordsAffected = ad.Fill(ds);
            //return ds;
        }


        public UKClaimPhasingByOfficeDs getUKClaimPhasingReportByOffice(int fiscalYear, int periodFrom, int periodTo, int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimPhasingApt", "GetUKClaimPhasingReportByOffice");

            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@PeriodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@PeriodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            UKClaimPhasingByOfficeDs dataSet = new UKClaimPhasingByOfficeDs();
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public UKClaimPhasingByOfficeClaimReasonDs getUKClaimPhasingReportByOfficeClaimReason(int fiscalYear, int periodFrom, int periodTo, int vendorId, int officeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimPhasingApt", "GetUKClaimPhasingReportByOfficeClaimReason");

            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@PeriodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@PeriodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            //ad.SelectCommand.MailSQL = true;
            UKClaimPhasingByOfficeClaimReasonDs dataSet = new UKClaimPhasingByOfficeClaimReasonDs();
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public UKClaimPhasingByOfficeClaimReasonDs getUKClaimPhasingReportByOfficeClaimType(int fiscalYear, int periodFrom, int periodTo, int vendorId, int officeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimPhasingApt", "GetUKClaimPhasingReportByOfficeClaimType");

            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@PeriodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@PeriodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            //ad.SelectCommand.MailSQL = true;
            UKClaimPhasingByOfficeClaimReasonDs dataSet = new UKClaimPhasingByOfficeClaimReasonDs();
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public UKClaimAuditLogReportDs getUKClaimAuditLogReport(DateTime ukDNDateFrom, DateTime ukDNDateTo, TypeCollector officeIdList, int productTeamId,
                   int vendorId, int claimTypeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimAuditLogReportApt", "GetUKClaimAuditLogReport");
            if (ukDNDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@UKDNDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@UKDNDateFrom"].Value = ukDNDateFrom;
            if (ukDNDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@UKDNDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@UKDNDateTo"].Value = ukDNDateTo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@ClaimTypeId"].Value = claimTypeId;
            ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            UKClaimAuditLogReportDs ds = new UKClaimAuditLogReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            return ds;
        }

        public NonTradeExpenseStatementDs getNonTradeExpenseStatementList(TypeCollector officeIdList, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo,
            string nsRefNoFrom, string nsRefNoTo, int ntVendorId, TypeCollector workflowStatusIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("NonTradeExpenseReportApt", "GetNonTradeExpenseStatementList");
            if (invoiceDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = invoiceDateFrom;
            if (invoiceDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = invoiceDateTo;

            if (dueDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DueDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DueDateFrom"].Value = dueDateFrom;
            if (dueDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DueDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DueDateTo"].Value = dueDateTo;

            ad.SelectCommand.Parameters["@NSRefNoFrom"].Value = nsRefNoFrom;
            ad.SelectCommand.Parameters["@NSRefNoTo"].Value = nsRefNoTo;
            ad.SelectCommand.Parameters["@NTVendorId"].Value = ntVendorId;

            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@WorkflowStatusIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, workflowStatusIdList.Values);

            NonTradeExpenseStatementDs ds = new NonTradeExpenseStatementDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            return ds;

        }

        public UKClaimMFRNQtyAnalysisReportDs getUKClaimMFRNQtyAnalysisReportList(int fiscalYear, int periodFrom, int periodTo)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimMFRNQtyAnalysisReportApt", "GetUKClaimMFRNQtyAnalysisReport");
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@PeriodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@PeriodTo"].Value = periodTo;

            UKClaimMFRNQtyAnalysisReportDs ds = new UKClaimMFRNQtyAnalysisReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            return ds;

        }

        public NTInvoiceSettlementDs GetNTInvoiceDetailForSettlement(TypeCollector officeList, int expenseTypeId, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime dueDateFrom, DateTime dueDateTo,
                string invoiceNoFrom, string invoiceNoTo, string customerNoFrom, string customerNoTo, string nslRefNoFrom, string nslRefNoTo, int vendorId, TypeCollector workflowStatusIdList, //int workflowStatusId, 
                DateTime paymentDateFrom, DateTime paymentDateTo, int currencyId, int paymentMethodId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailForSettlementApt", "GetNTInvoiceDetailForSettlement");
            ad.SelectCommand.CustomParameters["@OfficeList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);
            ad.SelectCommand.Parameters["@ExpenseTypeId"].Value = expenseTypeId;
            if (invoiceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = invoiceDateFrom;
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = invoiceDateTo;
            }
            if (dueDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@DueDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@DueDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@DueDateFrom"].Value = dueDateFrom;
                ad.SelectCommand.Parameters["@DueDateTo"].Value = dueDateTo;
            }
            ad.SelectCommand.Parameters["@InvoiceNoFrom"].Value = invoiceNoFrom;
            ad.SelectCommand.Parameters["@InvoiceNoTo"].Value = invoiceNoTo;
            ad.SelectCommand.Parameters["@CustomerNoFrom"].Value = customerNoFrom;
            ad.SelectCommand.Parameters["@CustomerNoTo"].Value = customerNoTo;
            ad.SelectCommand.Parameters["@NSLRefNoFrom"].Value = nslRefNoFrom;
            ad.SelectCommand.Parameters["@NSLRefNoTo"].Value = nslRefNoTo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            ad.SelectCommand.CustomParameters["@WorkflowStatusIdList"] = CustomDataParameter.parse(workflowStatusIdList.IsInclusive, workflowStatusIdList.Values);

            if (paymentDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = paymentDateFrom;
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = paymentDateTo;
            }
            ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;
            ad.SelectCommand.Parameters["@PaymentMethodId"].Value = paymentMethodId;

            NTInvoiceSettlementDs ds = new NTInvoiceSettlementDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            return ds;
        }

        public NTVendorReportDs getNTVendorReport(int officeId, int ntVendorId, int expenseTypeId, int workflowStatusId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTVendorApt", "GetNTVendorReport");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@NTVendorId"].Value = ntVendorId;
            ad.SelectCommand.Parameters["@ExpenseTypeId"].Value = expenseTypeId;
            ad.SelectCommand.Parameters["@WorkflowStatusId"].Value = workflowStatusId;

            NTVendorReportDs ds = new NTVendorReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            return ds;

        }


        public NTInvoiceSettlementDs GetNTInvoiceDetailForSettlement(TypeCollector invoiceIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("NTInvoiceDetailForSettlementApt", "GetNTInvoiceDetailForSettlementByInvoiceId");
            if (invoiceIdList.Values.Count == 0)
                invoiceIdList.append(-1);
            ad.SelectCommand.CustomParameters["@InvoiceIdList"] = CustomDataParameter.parse(invoiceIdList.IsInclusive, invoiceIdList.Values);
            NTInvoiceSettlementDs ds = new NTInvoiceSettlementDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            return ds;
        }

        //----------------------------------------------------------------------------------------------------------------------------

        #region NonTrade Recharge DC Note

        public string replaceTabWithSpaces(string text, int tabWidth)
        {
            string outText = string.Empty;
            foreach (string line in text.Split(new string[] { "\r\n" }, StringSplitOptions.None))
            {
                string lineWithoutTab = string.Empty;
                foreach (string segment in line.Split('\t'))
                {
                    lineWithoutTab += segment;
                    lineWithoutTab += new string(' ', tabWidth - lineWithoutTab.Length % tabWidth);
                }
                outText += lineWithoutTab + "\r\n";
            }
            return outText;
        }

        public string replaceTabWithSpaces(string text)
        {
            return replaceTabWithSpaces(text, 8);
        }

        public NTRechargeDCNoteDs getNTRechargeDCNoteDs(NTRechargeDCNoteDef dcNote)
        {
            List<NTRechargeDCNoteDef> dcNoteList = new List<NTRechargeDCNoteDef>();
            dcNoteList.Add(dcNote);
            return buildNTRechargeDCNoteDs(dcNoteList, NonTradeWorker.Instance.getNTInvoiceDetailByRechargeDCNoteId(dcNote.RechargeDCNoteId));
        }

        public NTRechargeDCNoteDs generateNTRechargeDCNoteDs(ArrayList ntRechargeDetailList, bool isDraft, DateTime dcNoteDate, int userId, ArrayList conflictDCNoteList)
        {
            //ReportClass report = null;
            NTRechargeDCNoteDs dcNoteDs = null;
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList amendmentList = new ArrayList();
                Hashtable ht_dcNote = new Hashtable();
                ArrayList sunInterfaceQueue = new ArrayList();
                ReporterWorker reportWorker = ReporterWorker.Instance;
                List<NTRechargeDCNoteDef> dcNoteList = new List<NTRechargeDCNoteDef>();
                List<NTInvoiceDetailDef> ntInvoiceDetailList = new List<NTInvoiceDetailDef>();

                foreach (NTInvoiceDetailDef detailDef in ntRechargeDetailList)
                {
                    // Create DC Notes and sum the total amount for them
                    NTRechargeDCNoteDef dcNote = null;
                    NTInvoiceDef invoiceDef = nontradeWorker.getNTInvoiceByKey(detailDef.InvoiceId);
                    string dcNoteNoIndex = nontradeWorker.getDCNoteNoGroupKey(invoiceDef, detailDef);
                    if (ht_dcNote.ContainsKey(dcNoteNoIndex))
                        dcNote = (NTRechargeDCNoteDef)ht_dcNote[dcNoteNoIndex];
                    else
                    {   // New DC Note
                        dcNote = nontradeWorker.createNewDCNoteDef(dcNoteNoIndex, dcNoteDate, detailDef, invoiceDef);
                        ht_dcNote.Add(dcNoteNoIndex, dcNote);
                    }
                    dcNote.RechargeAmount += nontradeWorker.calcRechargeAmount(dcNote, detailDef, invoiceDef);
                }

                int seqNo = 0;
                foreach (NTRechargeDCNoteDef dcNote in ht_dcNote.Values)
                {
                    // Assigning DC Note No.
                    if (dcNote.RechargeAmount < 0)
                    {
                        dcNote.DCIndicator = "C";
                        dcNote.RechargeAmount *= -1;
                    }
                    if (isDraft)
                    {
                        dcNote.RechargeDCNoteId = ++seqNo * -1;   // Dummy DC Note Id
                        dcNote.RechargeDCNoteNo = "[Draft]";    // "[Draft" + (seqNo).ToString() + "]";
                    }
                    else
                        NonTradeWorker.Instance.updateNTRechargeDCNote(dcNote, dcNote.Office.OfficeId, userId);
                    dcNoteList.Add(dcNote);
                }

                foreach (NTInvoiceDetailDef detailDef in ntRechargeDetailList)
                {
                    // Create DC Note detail
                    NTInvoiceDef invoiceDef = nontradeWorker.getNTInvoiceByKey(detailDef.InvoiceId);
                    string dcNoteNoIndex = NonTradeWorker.Instance.getDCNoteNoGroupKey(invoiceDef, detailDef);
                    NTRechargeDCNoteDef dcNote = (NTRechargeDCNoteDef)ht_dcNote[dcNoteNoIndex];
                    if (!isDraft)
                    {
                        int sunInterfaceTypeId = SunInterfaceTypeRef.Id.NonTradeRechargeDebitNote.GetHashCode();
                        if (detailDef.IsPayByHK == 1)
                            sunInterfaceTypeId = SunInterfaceTypeRef.Id.NonTradePaymentOnBehalfDebitNote.GetHashCode();

                        //if (!sunInterfaceQueue.Contains(dcNoteNoIndex)) // 2014-05-22


                        /* disable auto-submit interface 2017-07-18 By Michael Lau

                        if (!sunInterfaceQueue.Contains(dcNote.Office.OfficeId.ToString()))
                        {
                            int fiscalYear = 0;
                            int period = 0;
                            com.next.isam.dataserver.model.nontrade.NTRechargeDCNoteNoParamDs paramDs = nontradeWorker.getRechargeDCNoteNoParam(dcNote.RechargeDCNoteDate, dcNote.Office.OfficeId, dcNote.DCIndicator);
                            if (paramDs.NTRechargeDCNoteNoParam != null)
                            {
                                fiscalYear = (paramDs.NTRechargeDCNoteNoParam[0]).FiscalYear;
                                period = (paramDs.NTRechargeDCNoteNoParam[0]).Period;
                            }
                            NTSunInterfaceQueueDef queueDef = new NTSunInterfaceQueueDef();
                            queueDef.QueueId = -1;
                            queueDef.Office = generalWorker.getOfficeRefByKey(dcNote.Office.OfficeId);
                            queueDef.FiscalYear = fiscalYear;
                            queueDef.Period = period;
                            queueDef.CategoryType = CategoryType.ACTUAL;
                            queueDef.SourceId = 1;
                            queueDef.SubmitTime = DateTime.Now;
                            queueDef.SunInterfaceTypeId = sunInterfaceTypeId;
                            queueDef.User = generalWorker.getUserByKey(userId);
                            nontradeWorker.updateNTSunInterfaceQueue(queueDef);
                            //sunInterfaceQueue.Add(dcNoteNoIndex); 2014-05-22
                            sunInterfaceQueue.Add(dcNote.Office.OfficeId.ToString());
                        }
                        */

                        detailDef.RechargeDCNoteId = dcNote.RechargeDCNoteId;
                    }

                    int originalDCNoteId = nontradeWorker.updateNTInvoiceDetail(detailDef, amendmentList, userId);
                    if (originalDCNoteId != 0 && originalDCNoteId != dcNote.RechargeDCNoteId)
                    {
                        // Recharge DC Note ID conflict, Invoice detail belongs to another DC Note
                        detailDef.RechargeDCNoteId = originalDCNoteId;
                        conflictDCNoteList.Add(detailDef);
                    }
                    else
                        detailDef.RechargeDCNoteId = dcNote.RechargeDCNoteId;
                    ntInvoiceDetailList.Add(detailDef);
                }

                if (conflictDCNoteList.Count == 0)
                {
                    if (!isDraft)
                    {
                        if (amendmentList.Count > 0)
                        {
                            foreach (NTActionHistoryDef historyDef in amendmentList)
                            {
                                nontradeWorker.updateNTActionHistory(historyDef);
                            }
                        }
                    }
                    dcNoteDs = ReporterWorker.Instance.buildNTRechargeDCNoteDs(dcNoteList, ntInvoiceDetailList);
                    ctx.VoteCommit();
                }
                else
                {
                    dcNoteDs = null;
                    ctx.VoteRollback();
                }
                return dcNoteDs;
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

        private NTRechargeDCNoteDs.NTRechargeDCNoteRow createRechargeDCNoteDetail(NTRechargeDCNoteDef dcNote, NTInvoiceDetailDef ntInvoiceDetail, NTInvoiceDef invoiceDef, ref NTRechargeDCNoteDs.NTRechargeDCNoteRow dcNoteDetail)
        {
            NTRechargeDCNoteDs ds = new NTRechargeDCNoteDs();
            ReporterWorker reportWorker = ReporterWorker.Instance;
            CurrencyRef currency = invoiceDef.Currency;
            dcNoteDetail.NSLInvoiceNo = invoiceDef.NSLInvoiceNo;
            dcNoteDetail.InvoiceDetailId = ntInvoiceDetail.InvoiceDetailId;
            dcNoteDetail.Description = ReporterWorker.Instance.replaceTabWithSpaces(ntInvoiceDetail.Description);
            dcNoteDetail.CurrencyCode = currency.CurrencyCode;
            dcNoteDetail.Amount = ntInvoiceDetail.Amount * NonTradeWorker.Instance.getDCNoteRechargeAmountSign(dcNote, invoiceDef);
            dcNoteDetail.DCIndicator = dcNote.DCIndicator;
            //dcNoteDetail.ExchangeRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, currency.CurrencyId, DateTime.Today) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, ntInvoiceDetail.RechargeCurrency.CurrencyId, DateTime.Today);
            //dcNoteDetail.RechargeAmount = Math.Round(ntInvoiceDetail.Amount * dcNoteDetail.ExchangeRate, 2, MidpointRounding.AwayFromZero);
            dcNoteDetail.ExchangeRate = NonTradeWorker.Instance.getRechargeExchangeRate(dcNote, ntInvoiceDetail, invoiceDef);
            dcNoteDetail.RechargeAmount = NonTradeWorker.Instance.calcRechargeAmount(dcNote, ntInvoiceDetail, invoiceDef);

            if (dcNote.ToCustomerId > 0)
            {
                CustomerDef customer = CommonWorker.Instance.getCustomerByKey(dcNote.ToCustomerId);
                if (ntInvoiceDetail.RechargePartyDept.Id == NTRechargePartyDeptType.GROUP_FINANCE.Id)
                    dcNoteDetail.PartyName = "NEXT GROUP PLC";
                else
                    dcNoteDetail.PartyName = customer.CustomerDescription;
                dcNoteDetail.PartyAddress1 = customer.Address1;
                dcNoteDetail.PartyAddress2 = customer.Address2;
                dcNoteDetail.PartyAddress3 = customer.Address3;
                dcNoteDetail.PartyAddress4 = customer.Address4;
            }
            else if (dcNote.ToCompanyId > 0)
            {
                CompanyRef rechargedCompany = GeneralWorker.Instance.getCompanyByKey(dcNote.ToCompanyId);
                dcNoteDetail.PartyName = rechargedCompany.CompanyName;
                dcNoteDetail.PartyAddress1 = rechargedCompany.Address1;
                dcNoteDetail.PartyAddress2 = rechargedCompany.Address2;
                dcNoteDetail.PartyAddress3 = rechargedCompany.Address3;
                dcNoteDetail.PartyAddress4 = rechargedCompany.Address4;
            }
            else if (dcNote.ToNTVendorId > 0)
            {
                dcNoteDetail.PartyName = ntInvoiceDetail.NTVendor.VendorName;
                dcNoteDetail.PartyAddress1 = ntInvoiceDetail.NTVendor.Address;
            }
            else if (dcNote.ToVendorId > 0)
            {
                NTVendorDef vendor = NonTradeWorker.Instance.getNTVendorByKey(dcNote.ToVendorId);
                dcNoteDetail.PartyName = ntInvoiceDetail.Vendor.Name;
                dcNoteDetail.PartyAddress1 = ntInvoiceDetail.Vendor.Address0;
                dcNoteDetail.PartyAddress2 = ntInvoiceDetail.Vendor.Address1;
                dcNoteDetail.PartyAddress3 = ntInvoiceDetail.Vendor.Address2;
                dcNoteDetail.PartyAddress4 = ntInvoiceDetail.Vendor.Address3;
            }
            else
            {
                dcNoteDetail.PartyName = "N/A";
                dcNoteDetail.PartyAddress1 = "N/A";
                dcNoteDetail.PartyAddress2 = string.Empty;
                dcNoteDetail.PartyAddress3 = string.Empty;
                dcNoteDetail.PartyAddress4 = string.Empty;
            }
            dcNoteDetail.RechargeDCNoteNo = dcNote.RechargeDCNoteNo;
            dcNoteDetail.RechargeDCNoteDate = dcNote.RechargeDCNoteDate;
            dcNoteDetail.RechargeCurrencyCode = CurrencyId.getName(dcNote.RechargeCurrencyId);
            dcNoteDetail.RechargeCurrencyId = dcNote.RechargeCurrencyId;
            return dcNoteDetail;
        }

        private NTRechargeDCNoteDs.ContactInfoRow createRechargeDCNoteContactInfo(NTRechargeDCNoteDef dcNote, NTInvoiceDetailDef ntInvoiceDetail, ref NTRechargeDCNoteDs.ContactInfoRow contact)
        {

            NTInvoiceDef invoice = NonTradeWorker.Instance.getNTInvoiceByKey(ntInvoiceDetail.InvoiceId);
            CompanyRef company = null;
            ReporterWorker reportWorker = ReporterWorker.Instance;

            contact.RechargeDCNoteNo = dcNote.RechargeDCNoteNo;

            contact.ContactName = ntInvoiceDetail.RechargeContactPerson;
            contact.Department = "ACCOUNTS DEPARTMENT";
            contact.UKSupplierCode = string.Empty;
            if (ntInvoiceDetail.Customer != null)
                if (ntInvoiceDetail.InvoiceDetailType.Id == NTInvoiceDetailType.CUSTOMER.Id && CustomerDef.isNextRetail(ntInvoiceDetail.Customer.CustomerId))
                {
                    contact.Department = ntInvoiceDetail.RechargePartyDept.Description.ToUpper();
                    DebitNoteToNUKParamDef paramDef = reportWorker.getDebitNoteToNUKParamByKey(dcNote.Office.OfficeId, dcNote.RechargeCurrencyId);
                    contact.UKSupplierCode = (paramDef != null ? paramDef.SupplierCode : "N/A");
                }
            company = GeneralWorker.Instance.getCompanyByKey(dcNote.Company.Id);

            contact.CompanyName = string.Empty;
            contact.CompanyAddress = string.Empty;

            if (company != null)
            {
                contact.CompanyName = company.CompanyName.Trim();
                contact.CompanyAddress = (string.IsNullOrEmpty(company.Address1) ? string.Empty : (company.Address1.Trim())).Trim();
                contact.CompanyAddress += (string.IsNullOrEmpty(company.Address2) ? string.Empty : (contact.CompanyAddress.EndsWith(",") ? " " : ", ") + (string.IsNullOrEmpty(company.Address2) ? string.Empty : company.Address2.Trim()));
                contact.CompanyAddress += (string.IsNullOrEmpty(company.Address3) ? string.Empty : (contact.CompanyAddress.EndsWith(",") ? " " : ", ") + (string.IsNullOrEmpty(company.Address3) ? string.Empty : company.Address3.Trim()));
                contact.CompanyAddress += (string.IsNullOrEmpty(company.Address4) ? string.Empty : (contact.CompanyAddress.EndsWith(",") ? " " : ", ") + (string.IsNullOrEmpty(company.Address4) ? string.Empty : company.Address4.Trim()));
                contact.CompanyAddress += (contact.CompanyAddress.EndsWith(".") ? string.Empty : ".");
                contact.CompanyAddress += "   Tel.: " + company.DirectLine;
                contact.CompanyAddress += "   Fax.: " + company.FaxNo;
            }
            return contact;
        }

        private NTRechargeDCNoteDs.BeneficiaryAccountRow createRechargeDCNoteBeneficiaryAccount(OfficeRef office, NTInvoiceDetailDef detailDef, ref NTRechargeDCNoteDs.BeneficiaryAccountRow account)
        {
            DebitNoteToNUKParamDef paramDef;
            ReporterWorker reportWorker = ReporterWorker.Instance;

            /*
            account.BeneficiaryAccountNo = reportWorker.getBeneficiaryAccountNoList(office.OfficeId, detailDef.RechargeCurrency.CurrencyId);
            */
            account.BeneficiaryAccountNo = reportWorker.getBeneficiaryAccountNoList(detailDef.IntercommOfficeId == -1 ? office.OfficeId : detailDef.IntercommOfficeId, detailDef.RechargeCurrency.CurrencyId);
            account.OfficeId = office.OfficeId;
            account.OfficeName = office.OfficeCode;
            account.CurrencyId = detailDef.RechargeCurrency.CurrencyId;
            account.CurrencyCode = detailDef.RechargeCurrency.CurrencyCode;
            /*
            paramDef = reportWorker.getDebitNoteToNUKParamByKey(office.OfficeId, detailDef.RechargeCurrency.CurrencyId);
            */
            paramDef = reportWorker.getDebitNoteToNUKParamByKey(detailDef.IntercommOfficeId == -1 ? office.OfficeId : detailDef.IntercommOfficeId, detailDef.RechargeCurrency.CurrencyId);
            if (paramDef != null)
            {
                account.SupplierCode = paramDef.SupplierCode;
                account.BeneficiaryName = paramDef.BeneficiaryName;
                account.BankName = paramDef.BankName;
                account.BankAddress = paramDef.BankAddress;
                account.SwiftCode = paramDef.SwiftCode;
            }
            else
            {
                account.SupplierCode = "N/A";
                account.BeneficiaryName = "N/A";
                account.BankName = "N/A";
                account.BankAddress = "N/A";
                account.SwiftCode = "N/A";
            }
            return account;
        }

        private NTRechargeDCNoteDs buildNTRechargeDCNoteDs(List<NTRechargeDCNoteDef> dcNoteList, List<NTInvoiceDetailDef> ntRechargeDetailList)
        {
            // The ntRechargeDetail should be link to dcNote with the RechargeDCNoteId
            try
            {
                NTRechargeDCNoteDs rechargeDs = new NTRechargeDCNoteDs();
                if (dcNoteList != null && ntRechargeDetailList != null)
                {
                    Hashtable ht_dcNote = new Hashtable();
                    foreach (NTRechargeDCNoteDef dcNote in dcNoteList)
                        ht_dcNote.Add(dcNote.RechargeDCNoteId, dcNote);
                    ArrayList contactList = new ArrayList();
                    ArrayList accountList = new ArrayList();
                    foreach (NTInvoiceDetailDef detailDef in ntRechargeDetailList)
                    {
                        NTRechargeDCNoteDef dcNote = (NTRechargeDCNoteDef)ht_dcNote[detailDef.RechargeDCNoteId];
                        NTInvoiceDef invoiceDef = NonTradeWorker.Instance.getNTInvoiceByKey(detailDef.InvoiceId);
                        NTRechargeDCNoteDs.NTRechargeDCNoteRow row = rechargeDs.NTRechargeDCNote.NewNTRechargeDCNoteRow();
                        ReporterWorker.Instance.createRechargeDCNoteDetail(dcNote, detailDef, invoiceDef, ref row);
                        rechargeDs.NTRechargeDCNote.AddNTRechargeDCNoteRow(row);
                        if (!contactList.Contains(dcNote.RechargeDCNoteNo))
                        {
                            contactList.Add(dcNote.RechargeDCNoteNo);
                            NTRechargeDCNoteDs.ContactInfoRow contact = rechargeDs.ContactInfo.NewContactInfoRow();
                            ReporterWorker.Instance.createRechargeDCNoteContactInfo(dcNote, detailDef, ref contact);
                            rechargeDs.ContactInfo.AddContactInfoRow(contact);
                        }
                        if (!accountList.Contains(dcNote.RechargeCurrencyId))
                        {
                            accountList.Add(dcNote.RechargeCurrencyId);
                            NTRechargeDCNoteDs.BeneficiaryAccountRow account = rechargeDs.BeneficiaryAccount.NewBeneficiaryAccountRow();
                            ReporterWorker.Instance.createRechargeDCNoteBeneficiaryAccount(dcNote.Office, detailDef, ref account);
                            rechargeDs.BeneficiaryAccount.AddBeneficiaryAccountRow(account);
                        }
                    }
                }
                return rechargeDs;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
            }

        }

        #endregion

        public NUKSalesCutOffSummaryDs GetNUKSalesCutOffSummary(int fiscalYear, int period, DateTime periodStartDate, DateTime periodEndDate)
        {
            IDataSetAdapter ad = getDataSetAdapter("NUKSalesCutOffSummaryApt", "GetNUKSalesCutOffSummary");
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@Period"].Value = period;
            ad.SelectCommand.Parameters["@PeriodStartDate"].Value = periodStartDate;
            ad.SelectCommand.Parameters["@PeriodEndDate"].Value = periodEndDate;
            NUKSalesCutOffSummaryDs ds = new NUKSalesCutOffSummaryDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            return ds;
        }

        public NUKSalesShippingReportDs GetNUKSalesShippingReport(int fiscalYear, int period, TypeCollector officeList, DateTime ilsActualAWHDateFrom, DateTime ilsActualAWHDateTo, DateTime periodStartDate, DateTime periodEndDate)
        {
            IDataSetAdapter ad = getDataSetAdapter("NUKSalesShippingReportApt", "GetNUKSalesShippingReport");
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@Period"].Value = period;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);
            ad.SelectCommand.Parameters["@ILSActualAWHDateFrom"].Value = ilsActualAWHDateFrom;
            ad.SelectCommand.Parameters["@ILSActualAWHDateTo"].Value = ilsActualAWHDateTo;
            ad.SelectCommand.Parameters["@PeriodStartDate"].Value = periodStartDate;
            ad.SelectCommand.Parameters["@PeriodEndDate"].Value = periodEndDate;

            NUKSalesShippingReportDs ds = new NUKSalesShippingReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            ad.SelectCommand.MailSQL = true;
            int recordsAffected = ad.Fill(ds);
            return ds;
        }

        public EpicorInterfaceLogReportDs getEpicorInterfaceLogReport(DateTime interfaceDateFrom, DateTime interfaceDateTo, TypeCollector officeIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("EpicorInterfaceLogReportApt", "getEpicorInterfaceLogReport");
            //ad.SelectCommand.MailSQL = true;
            if (interfaceDateFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@InterfaceDateFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@InterfaceDateTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@InterfaceDateFrom"].Value = interfaceDateFrom;
                ad.SelectCommand.Parameters["@InterfaceDateTo"].Value = interfaceDateTo;
            }
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);

            EpicorInterfaceLogReportDs dataSet = new EpicorInterfaceLogReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public PaymentStatusEnquiryEpicorDs getPaymentStatusEnquiryEpicorDataSet(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, int handlingOfficeId, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime subDocDateFrom,
            DateTime subDocDateTo, DateTime interfaceDateFrom, DateTime interfaceDateTo, int paymentTermId, int tradingAgencyId, int vendorId,
            TypeCollector paymentStatusList, string contractNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("PaymentStatusEnquiryApt", "GetPaymentStatusEnquiryEpicorList");

            ad.SelectCommand.Parameters["@InvoicePrefix"].Value = invoicePrefix;
            ad.SelectCommand.Parameters["@InvoiceSeqFrom"].Value = invoiceSeqFrom;
            ad.SelectCommand.Parameters["@InvoiceSeqTo"].Value = invoiceSeqTo;
            ad.SelectCommand.Parameters["@InvoiceYear"].Value = invoiceYear;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@PaymentTermId"].Value = paymentTermId;
            ad.SelectCommand.Parameters["@TradingAgencyId"].Value = tradingAgencyId;

            if (invoiceDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = invoiceDateFrom;
            if (invoiceDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = invoiceDateTo;

            if (subDocDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@SubDocDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@SubDocDateFrom"].Value = subDocDateFrom;
            if (subDocDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@SubDocDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@SubDocDateTo"].Value = subDocDateTo;

            if (interfaceDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InterfaceDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InterfaceDateFrom"].Value = interfaceDateFrom;
            if (interfaceDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InterfaceDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InterfaceDateTo"].Value = interfaceDateTo;
            ad.SelectCommand.CustomParameters["@PaymentStatus"] = CustomDataParameter.parse(paymentStatusList.IsInclusive, paymentStatusList.Values);
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;

            PaymentStatusEnquiryEpicorDs dataSet = new PaymentStatusEnquiryEpicorDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            //Fill in the China GB Test Result
            PaymentStatusEnquiryEpicorDs.PaymentStatusEnquiryRow row;
            for (int i = dataSet.PaymentStatusEnquiry.Rows.Count - 1; i >= 0; i--)
            {
                row = (PaymentStatusEnquiryEpicorDs.PaymentStatusEnquiryRow)dataSet.PaymentStatusEnquiry.Rows[i];
                if (row.IsChinaGBTestRequired)
                    row.GBTestResult = GeneralWorker.Instance.getChinaGBTestResult(row.ProductId, row.SupplierID);
            }
            return dataSet;
        }

        public OutstandingGBTestReportDs getOutstandingGBTestReport(TypeCollector officeList, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime customerAWHDateFrom,
            DateTime customerAWHDateTo, int vendorId, TypeCollector shipmentMethodIdList, TypeCollector paymentTermIdList, int paymentStatus)
        {
            IDataSetAdapter ad = getDataSetAdapter("OutstandingGBTestReportApt", "GetOutstandingGBTestReport");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);
            if (invoiceDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateFrom"].Value = invoiceDateFrom;
            if (invoiceDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@InvoiceDateTo"].Value = invoiceDateTo;
            if (customerAWHDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@CustAWHDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@CustAWHDateFrom"].Value = customerAWHDateFrom;
            if (customerAWHDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@CustAWHDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@CustAWHDateTo"].Value = customerAWHDateTo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.CustomParameters["@ShipmentMethodIdList"] = CustomDataParameter.parse(shipmentMethodIdList.IsInclusive, shipmentMethodIdList.Values);
            ad.SelectCommand.CustomParameters["@PaymentTermIdList"] = CustomDataParameter.parse(paymentTermIdList.IsInclusive, paymentTermIdList.Values);
            ad.SelectCommand.Parameters["@PaymentStatus"].Value = paymentStatus;

            OutstandingGBTestReportDs ds = new OutstandingGBTestReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            return ds;

        }

        public TradingAFReportDs getTradingAFReport(TypeCollector officeList, DateTime customerAtWearHouseFrom, DateTime customerAtWearHouseTo, DateTime invoiceFrom, DateTime invoiceTo, int supplierId, TypeCollector countrylist, TypeCollector portlist, TypeCollector shipmentlist)
        {
            IDataSetAdapter ad = getDataSetAdapter("TradingAFReportApt", "GetTradingAFReport");

            ad.SelectCommand.CustomParameters["@officeList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);

            if (customerAtWearHouseFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@customerAtWearHouseFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@customerAtWearHouseTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@customerAtWearHouseFrom"].Value = customerAtWearHouseFrom;
                ad.SelectCommand.Parameters["@customerAtWearHouseTo"].Value = customerAtWearHouseTo;
            }
            if (invoiceFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceFrom"].Value = invoiceFrom;
                ad.SelectCommand.Parameters["@invoiceTo"].Value = invoiceTo;
            }

            ad.SelectCommand.Parameters["@supplierId"].Value = supplierId;

            ad.SelectCommand.CustomParameters["@countryList"] = CustomDataParameter.parse(countrylist.IsInclusive, countrylist.Values);
            ad.SelectCommand.CustomParameters["@portList"] = CustomDataParameter.parse(portlist.IsInclusive, portlist.Values);
            ad.SelectCommand.CustomParameters["@shipmentList"] = CustomDataParameter.parse(shipmentlist.IsInclusive, shipmentlist.Values);
            ad.SelectCommand.MailSQL = true;
            TradingAFReportDs ds = new TradingAFReportDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public UTForcastDs getUTForcastReport(DateTime custAtWarehouseFrom, DateTime custAtWarehouseTo)
        {
            IDataSetAdapter ad = getDataSetAdapter("UTForecastApt", "GetUTForecastReport");
            ad.SelectCommand.Parameters["@atWHDateFrom"].Value = custAtWarehouseFrom;
            ad.SelectCommand.Parameters["@atWHDateTo"].Value = custAtWarehouseTo;
            UTForcastDs dataSet = new UTForcastDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        public UTDiscrepancyDs getUTDiscrepancyReport(TypeCollector officeList, DateTime customerAtWearHouseFrom, DateTime customerAtWearHouseTo, DateTime invoiceFrom, DateTime invoiceTo, int supplierId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UTDiscrepancyApt", "GetUTDiscrepancyReport");

            ad.SelectCommand.CustomParameters["@officeList"] = CustomDataParameter.parse(officeList.IsInclusive, officeList.Values);

            if (customerAtWearHouseFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@customerAtWearHouseFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@customerAtWearHouseTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@customerAtWearHouseFrom"].Value = customerAtWearHouseFrom;
                ad.SelectCommand.Parameters["@customerAtWearHouseTo"].Value = customerAtWearHouseTo;
            }
            if (invoiceFrom == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@invoiceFrom"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@invoiceTo"].Value = DBNull.Value;
            }
            else
            {
                ad.SelectCommand.Parameters["@invoiceFrom"].Value = invoiceFrom;
                ad.SelectCommand.Parameters["@invoiceTo"].Value = invoiceTo;
            }

            ad.SelectCommand.Parameters["@supplierId"].Value = supplierId;

            UTDiscrepancyDs ds = new UTDiscrepancyDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public AdvancePaymentReportDs getAdvancePaymentReport(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, int officeId, int paymentstatusIndex)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentReportApt", "GetAdvancePaymentReport");

            if (paymentDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = paymentDateFrom;

            if (paymentDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = paymentDateTo;

            if (deductionDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeductionDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeductionDateFrom"].Value = deductionDateFrom;

            if (deductionDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeductionDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeductionDateTo"].Value = deductionDateTo;

            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@PaymentStatus"].Value = paymentstatusIndex;

            AdvancePaymentReportDs ds = new AdvancePaymentReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public AdvancePaymentReportDs getAdvancePaymentReportMG(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, int officeId, int paymentstatusIndex)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentReportApt", "GetAdvancePaymentReportMG");

            if (paymentDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = paymentDateFrom;

            if (paymentDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = paymentDateTo;

            if (deductionDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeductionDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeductionDateFrom"].Value = deductionDateFrom;

            if (deductionDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeductionDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeductionDateTo"].Value = deductionDateTo;

            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@PaymentStatus"].Value = paymentstatusIndex;

            AdvancePaymentReportDs ds = new AdvancePaymentReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public AdvancePaymentSummaryReportDs getAdvancePaymentSummaryReport(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, int officeId, int paymentstatusIndex)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentSummaryReportApt", "GetAdvancePaymentSummaryReport");

            if (paymentDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = paymentDateFrom;

            if (paymentDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = paymentDateTo;

            if (deductionDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeductionDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeductionDateFrom"].Value = deductionDateFrom;

            if (deductionDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeductionDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeductionDateTo"].Value = deductionDateTo;

            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@PaymentStatus"].Value = paymentstatusIndex;

            AdvancePaymentSummaryReportDs ds = new AdvancePaymentSummaryReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public AdvancePaymentSummaryReportDs getAdvancePaymentSummaryReportMG(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, int officeId, int paymentstatusIndex)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentSummaryReportApt", "GetAdvancePaymentSummaryReportMG");

            if (paymentDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateFrom"].Value = paymentDateFrom;

            if (paymentDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@PaymentDateTo"].Value = paymentDateTo;

            if (deductionDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeductionDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeductionDateFrom"].Value = deductionDateFrom;

            if (deductionDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DeductionDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DeductionDateTo"].Value = deductionDateTo;

            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@PaymentStatus"].Value = paymentstatusIndex;

            AdvancePaymentSummaryReportDs ds = new AdvancePaymentSummaryReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public AdvancePaymentSummaryReportDs getAdvancePaymentSummaryReportMGThisYear(int officeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentSummaryReportApt", "GetAdvancePaymentSummaryReportMGThisYear");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;

            AdvancePaymentSummaryReportDs ds = new AdvancePaymentSummaryReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public AdvancePaymentSummaryReportDs getAdvancePaymentSummaryReportMGPreviousYear(int officeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentSummaryReportApt", "GetAdvancePaymentSummaryReportMGPreviousYear");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;

            AdvancePaymentSummaryReportDs ds = new AdvancePaymentSummaryReportDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }

        public string getAdvancePaymentFirstContractDate(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentSummaryReportApt", "GetAdvancePaymentFirstContractDate");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;

            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            if (recordsAffected > 0)
                return ds.Tables[0].Rows[0][0].ToString() + " - " + Convert.ToDateTime(ds.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy") + " " + ds.Tables[0].Rows[0][2].ToString() + " " + Convert.ToDecimal(ds.Tables[0].Rows[0][3]).ToString();
            else
                return "N/A";
        }

        public string getAdvancePaymentLastContractDate(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentSummaryReportApt", "GetAdvancePaymentLastContractDate");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;

            DataSet ds = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            if (recordsAffected > 0)
                return ds.Tables[0].Rows[0][0].ToString() + " - " + Convert.ToDateTime(ds.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy") + " " + ds.Tables[0].Rows[0][2].ToString() + " " + Convert.ToDecimal(ds.Tables[0].Rows[0][3]).ToString();
            else
                return "N/A";
        }

        public LGHoldPaymentDs getLGHoldPaymentList(int officeId, string lgNo, int vendorId, string itemNo, string contractNo, int paymentStatusId)
        {
            LGHoldPaymentDs dataSet = new LGHoldPaymentDs();
            IDataSetAdapter ad = getDataSetAdapter("LGHoldPaymentApt", "GetLGHoldPaymentList");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@LGNo"].Value = lgNo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@PaymentStatusId"].Value = paymentStatusId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            dataSet.EnforceConstraints = false;
            int recordsAffected = ad.Fill(dataSet);
            return dataSet;
        }

        public POListByOfficeSupplierReportDs getPOListByOfficeSupplierReport(int fiscalYear, int fiscalPeriod, int officeId, int vendorId, int currencyId, TypeCollector paymentStatusList)
        {
            IDataSetAdapter ad = getDataSetAdapter("POListByOfficeSupplierApt", "GetPOListByOfficeSupplier");
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@FiscalPeriod"].Value = fiscalPeriod;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;
            ad.SelectCommand.CustomParameters["@WorkflowStatusIdList"] = CustomDataParameter.parse(paymentStatusList.IsInclusive, paymentStatusList.Values);

            POListByOfficeSupplierReportDs ds = new POListByOfficeSupplierReportDs();
            ad.SelectCommand.MailSQL = true;
            ad.SelectCommand.DbCommand.CommandTimeout = 3600;
            int recordsAffected = ad.Fill(ds);

            return ds;
        }


        public ArrayList getNssWeeklySalesSummaryData(int fiscalYear, int fiscalPeriod, int weekNo)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("NssWeeklySalesSummaryReportApt", "getNssWeeklySalesSnapshotSummary");
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@FiscalPeriod"].Value = fiscalPeriod;
            ad.SelectCommand.Parameters["@SnapshotNo"].Value = weekNo + 1;

            NssWeeklySalesSummaryDs dataSet = new NssWeeklySalesSummaryDs();
            int recordsAffected = ad.Fill(dataSet);
            foreach (NssWeeklySalesSummaryDs.NssWeeklySalesSummaryRow row in dataSet.NssWeeklySalesSummary)
                list.Add(row.ItemArray);
            return list;
        }


        public ArrayList getNssWeeklySalesDetailData(int fiscalYear, int fiscalPeriod, int weekNo)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("NssWeeklySalesDetailReportApt", "getNssWeeklySalesSnapshotDetail");
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@FiscalPeriod"].Value = fiscalPeriod;
            ad.SelectCommand.Parameters["@MaxSnapshotNo"].Value = weekNo + 1;

            NssWeeklySalesDetailDs dataSet = new NssWeeklySalesDetailDs();
            int recordsAffected = ad.Fill(dataSet);
            foreach (NssWeeklySalesDetailDs.NssWeeklySalesDetailRow row in dataSet.NssWeeklySalesDetail)
                list.Add(row.ItemArray);
            return list;
        }

        public ArrayList getNssWeeklySalesSlippedOrderData(int fiscalYear, int fiscalPeriod, int weekNo)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("NssWeeklySalesSlippedOrderApt", "getNssWeeklySalesSlippedOrder");
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@FiscalPeriod"].Value = fiscalPeriod;
            ad.SelectCommand.Parameters["@SnapshotNo"].Value = weekNo + 1;

            NssWeeklySalesSlippedOrderDs dataSet = new NssWeeklySalesSlippedOrderDs();
            int recordsAffected = ad.Fill(dataSet);
            foreach (NssWeeklySalesSlippedOrderDs.NssWeeklySalesSlippedOrderRow row in dataSet.NssWeeklySalesSlippedOrder)
                list.Add(row.ItemArray);
            return list;
        }


    }
}
