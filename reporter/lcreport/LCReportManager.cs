using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.next.isam.dataserver.worker;
using com.next.isam.domain.common;
using com.next.isam.domain.order;
using com.next.isam.domain.shipping;
using com.next.isam.domain.ils;
using com.next.isam.domain.types;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.isam.reporter;
using com.next.isam.reporter.common;
using com.next.isam.reporter.dataserver;
using com.next.infra.util;


namespace com.next.isam.reporter.lcreport
{
    public class LCReportManager
    {
        private static LCReportManager _instance;
        private ShippingWorker shippingWorker;

        public LCReportManager()
		{
            shippingWorker = ShippingWorker.Instance;
		}

        public static LCReportManager Instance
		{
			get 
			{
				if (_instance == null)
				{
                    _instance = new LCReportManager();
				}
				return _instance;
			}
		}


        public LCBatchControlRpt getLCBatchControlReport(int lcBatchNoFrom, int lcBatchNoTo, DateTime lcAppliedDateFrom, DateTime lcAppliedDateTo, 
            DateTime lcIssueDateFrom, DateTime lcIssueDateTo, DateTime lcExpiryDateFrom, DateTime lcExpiryDateTo, 
            int coId, TypeCollector officeIdList, int handlingOfficeId, int vendorId, int userId, int lcApplicationNoFrom, int lcApplicationNoTo, DateTime lcApplicationDateFrom,
            DateTime lcApplicationDateTo, string lcNoFrom, string lcNoTo, string coName, string officeName, string handlingOfficeName)
        {
           
            LCBatchControlRpt rpt = new LCBatchControlRpt();
            CommonWorker commonWorker = CommonWorker.Instance;

            DataSet ds = ReporterWorker.Instance.getLCBatchContolReport(lcBatchNoFrom, lcBatchNoTo, lcAppliedDateFrom, lcAppliedDateTo, 
            lcIssueDateFrom, lcIssueDateTo, lcExpiryDateFrom, lcExpiryDateTo, coId, officeIdList, handlingOfficeId, vendorId, lcApplicationNoFrom, lcApplicationNoTo , lcApplicationDateFrom,
            lcApplicationDateTo, lcNoFrom, lcNoTo);
            
            rpt.SetDataSource(ds.Tables[0]);


            //rpt.SetParameterValue("OfficeList", "");
            rpt.SetParameterValue("LCApplicationNoFrom", (lcApplicationNoFrom < 0 ? "" : lcApplicationNoFrom.ToString()));
            rpt.SetParameterValue("LCApplicationNoTo", (lcApplicationNoTo < 0 ? "" : lcApplicationNoTo.ToString()));
            rpt.SetParameterValue("LCBatchNoFrom", (lcBatchNoFrom < 0 ? "" : "LCB" + lcBatchNoFrom.ToString().PadLeft(6, char.Parse("0"))));
            rpt.SetParameterValue("LCBatchNoTo", (lcBatchNoTo < 0 ? "" : "LCB" + lcBatchNoTo.ToString().PadLeft(6, char.Parse("0"))));
            rpt.SetParameterValue("LCNoFrom", lcNoFrom);
            rpt.SetParameterValue("LCNoTo", lcNoTo);

            rpt.SetParameterValue("LCApplicationDateFrom", (DateTimeUtility.getDateString(lcApplicationDateFrom)));
            rpt.SetParameterValue("LCApplicationDateTo", (DateTimeUtility.getDateString(lcApplicationDateTo)));
            rpt.SetParameterValue("LCAppliedDateFrom", (DateTimeUtility.getDateString(lcAppliedDateFrom)));
            rpt.SetParameterValue("LCAppliedDateTo", (DateTimeUtility.getDateString(lcAppliedDateTo)));

            rpt.SetParameterValue("LCIssueDateFrom", (DateTimeUtility.getDateString(lcIssueDateFrom)));
            rpt.SetParameterValue("LCIssueDateTo", (DateTimeUtility.getDateString(lcIssueDateTo)));
            rpt.SetParameterValue("LCExpiryDateFrom", (DateTimeUtility.getDateString(lcExpiryDateFrom)));
            rpt.SetParameterValue("LCExpiryDateTo", (DateTimeUtility.getDateString(lcExpiryDateTo)));

            rpt.SetParameterValue("Supplier", (vendorId == int.MinValue ? "ALL" : VendorWorker.Instance.getVendorByKey(vendorId).Name));
            rpt.SetParameterValue("CountryOfOrigin", (coName == "" ? "ALL" : coName));
            rpt.SetParameterValue("Office", (officeName == "" ? "ALL" : officeName));
            rpt.SetParameterValue("HandlingOffice", handlingOfficeName);
            rpt.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return rpt;
        }


        public LCStatusRpt getLCStatusReport(int vendorId, string lcNoFrom, string lcNoTo, string lcBillRefNoFrom, string lcBillRefNoTo, int lcBatchNoFrom, int lcBatchNoTo, 
            DateTime lcIssueDateFrom, DateTime lcIssueDateTo, DateTime lcExpiryDateFrom, DateTime lcExpiryDateTo,
            DateTime lcPaymentCheckDateFrom, DateTime lcPaymentCheckDateTo, int coId, TypeCollector officeIdList, int handlingOfficeId, ArrayList productTeamIdList,
            string coName, string officeName, string handlingOfficeName, string productTeamName, string gbTestResultOption, int userId)
        {

            LCStatusRpt rpt = new LCStatusRpt();
            CommonWorker commonWorker = CommonWorker.Instance;
            int isChinaGBTestRequired;
            int chinaGBTestResult;
            switch (gbTestResultOption.ToUpper())
            {
                //case "FAIL": //  Fail only
                //    isChinaGBTestRequired = 1;
                //    chinaGBTestResult = 0;
                //    break;
                case "PASS": // Pass/Accept only
                    isChinaGBTestRequired = 1;
                    chinaGBTestResult = 1;
                    break;
                case "FAIL (CAN RELEASE PAYMENT)": // Pass/Accept only
                    isChinaGBTestRequired = 1;
                    chinaGBTestResult = 2;
                    break;
                case "FAIL (HOLD PAYMENT)": // Pass/Accept only
                    isChinaGBTestRequired = 1;
                    chinaGBTestResult = 0;
                    break;
                case "FAIL (CANNOT RELEASE PAYMENT)": // Pass/Accept only
                    isChinaGBTestRequired = 1;
                    chinaGBTestResult = 9;
                    break;
                case "NIL": //  NIL - does not have result yet
                    isChinaGBTestRequired = 1;
                    chinaGBTestResult = -1;
                    break;
                case "ALL":
                default:    // -1 => All combination
                    isChinaGBTestRequired = -999;
                    chinaGBTestResult = -999;
                    break;
            }
            LCStatusReportDs ds = ReporterWorker.Instance.getLCStatusReport(lcBatchNoFrom, lcBatchNoTo, lcNoFrom, lcNoTo, lcBillRefNoFrom, lcBillRefNoTo,
                lcIssueDateFrom, lcIssueDateTo, lcExpiryDateFrom, lcExpiryDateTo, lcPaymentCheckDateFrom, lcPaymentCheckDateTo,
                coId, officeIdList, handlingOfficeId, productTeamIdList, vendorId, isChinaGBTestRequired, chinaGBTestResult);

            //rpt.SetDataSource(ds.Tables[0]);
            rpt.SetDataSource((DataTable)ds.LCStatusReport);

            //rpt.SetParameterValue("OfficeList", "");
            rpt.SetParameterValue("LCNoFrom", lcNoFrom);
            rpt.SetParameterValue("LCNoTo", lcNoTo);
            rpt.SetParameterValue("LCBillRefNoFrom", lcBillRefNoFrom);
            rpt.SetParameterValue("LCBillRefNoTo", lcBillRefNoTo);
            rpt.SetParameterValue("LCBatchNoFrom", (lcBatchNoFrom == int.MinValue ? "" : "LCB" + lcBatchNoFrom.ToString().PadLeft(6, char.Parse("0"))));
            rpt.SetParameterValue("LCBatchNoTo", (lcBatchNoTo == int.MinValue ? "" : "LCB" + lcBatchNoTo.ToString().PadLeft(6, char.Parse("0"))));
            rpt.SetParameterValue("LCIssueDateFrom", (DateTimeUtility.getDateString(lcIssueDateFrom)));
            rpt.SetParameterValue("LCIssueDateTo", (DateTimeUtility.getDateString(lcIssueDateTo)));
            rpt.SetParameterValue("LCExpiryDateFrom", (DateTimeUtility.getDateString(lcExpiryDateFrom)));
            rpt.SetParameterValue("LCExpiryDateTo", (DateTimeUtility.getDateString(lcExpiryDateTo)));
            rpt.SetParameterValue("LCPaymentCheckDateFrom", (DateTimeUtility.getDateString(lcPaymentCheckDateFrom)));
            rpt.SetParameterValue("LCPaymentCheckDateTo", (DateTimeUtility.getDateString(lcPaymentCheckDateTo)));

            rpt.SetParameterValue("CountryOfOrigin", (coName == "" ? "ALL" : coName));
            rpt.SetParameterValue("Office", (officeName == "" ? "ALL" : officeName));
            rpt.SetParameterValue("ProductTeam", (productTeamName == "" ? "ALL" : productTeamName));
            rpt.SetParameterValue("Supplier", (vendorId==int.MinValue? "ALL" : VendorWorker.Instance.getVendorByKey(vendorId).Name));
            rpt.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            rpt.SetParameterValue("HandlingOffice", handlingOfficeName);
            rpt.SetParameterValue("Remark", "");
            rpt.SetParameterValue("GBTestOption", gbTestResultOption.ToUpper());

            return rpt;
        }


        public LCStatusRpt getLCStatusReport(int vendorId, string lcNoFrom, string lcNoTo, string lcBillRefNo, int userId)
        {

            LCStatusRpt rpt = new LCStatusRpt();
            CommonWorker commonWorker = CommonWorker.Instance;

            //DataSet ds = ReporterWorker.Instance.getLCStatusReport(shipmentIdList);
            //DataSet ds = ReporterWorker.Instance.getLCStatusReport(vendorId, lcNoFrom, lcNoTo, lcBillRefNo, userId);
            LCStatusReportDs ds = ReporterWorker.Instance.getLCStatusReport(vendorId, lcNoFrom, lcNoTo, lcBillRefNo, userId);

            //rpt.SetDataSource(ds.Tables[0]);
            rpt.SetDataSource((DataTable)ds.LCStatusReport);

            rpt.SetParameterValue("LCNoFrom", lcNoFrom);
            rpt.SetParameterValue("LCNoTo", lcNoTo);
            rpt.SetParameterValue("LCBillRefNoFrom", lcBillRefNo);
            rpt.SetParameterValue("LCBillRefNoTo", lcBillRefNo);
            rpt.SetParameterValue("LCBatchNoFrom", "");
            rpt.SetParameterValue("LCBatchNoTo", "");
            rpt.SetParameterValue("LCIssueDateFrom", "");
            rpt.SetParameterValue("LCIssueDateTo", "");
            rpt.SetParameterValue("LCExpiryDateFrom", "");
            rpt.SetParameterValue("LCExpiryDateTo", "");
            rpt.SetParameterValue("LCPaymentCheckDateFrom", "");
            rpt.SetParameterValue("LCPaymentCheckDateTo", "");

            rpt.SetParameterValue("CountryOfOrigin", "");
            rpt.SetParameterValue("Office", "");
            rpt.SetParameterValue("ProductTeam", "");
            rpt.SetParameterValue("Supplier", (vendorId == int.MinValue ? "ALL" : VendorWorker.Instance.getVendorByKey(vendorId).Name));
            rpt.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(userId).DisplayName);
            rpt.SetParameterValue("HandlingOffice", "");
            rpt.SetParameterValue("GBTestOption", "");
            rpt.SetParameterValue("Remark", "L/C Payment Check Date Is Not Empty");

            return rpt;
        }

        public OutstandingLCReport getOutstandingLCReport(string officeName, TypeCollector officeIdList, string handlingOfficeName, int handlingOfficeId, TypeCollector productTeamList, int vendorId,
            int countryOfOriginId, int customerDestinationId, int purchaseTermId, DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo, 
            int userId, string sortBy, string sortDirection)
        {
            OutstandingLCReport rpt = new OutstandingLCReport();
            CommonWorker commonWorker = CommonWorker.Instance;

            OutstandingLCReportDs dataset = ReporterWorker.Instance.getOutstandingLCReport(officeIdList, handlingOfficeId, productTeamList, vendorId, countryOfOriginId,
                customerDestinationId, purchaseTermId, customerAtWHDateFrom, customerAtWHDateTo);
            if (dataset.OutstandingLCReport.Rows.Count > 0)
            {
                DataTable sortedTable = null;
                if (sortDirection == "desc")
                {
                    var query =
                        from report in dataset.OutstandingLCReport.AsEnumerable()
                        orderby sortBy == "Office" ? report.Office : sortBy == "ProductTeam" ? report.ProductTeam : sortBy == "ContractNo" ? report.ContractNo : report.DeliveryNo.ToString() descending
                        select report;

                    sortedTable = query.CopyToDataTable();
                }
                else
                {
                    var query =
                        from report in dataset.OutstandingLCReport.AsEnumerable()
                        orderby sortBy == "Office" ? report.Office : sortBy == "ProductTeam" ? report.ProductTeam : sortBy == "ContractNo" ? report.ContractNo : report.DeliveryNo.ToString() ascending
                        select report;
                    sortedTable = query.CopyToDataTable();
                }

                rpt.SetDataSource(sortedTable);
            }
            else
                rpt.SetDataSource(dataset);

            //rpt.SetParameterValue("OfficeCode", officeId == -1 ? "" : GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode);
            rpt.SetParameterValue("OfficeCode", officeName);
            rpt.SetParameterValue("HandlingOfficeName", handlingOfficeName);

            rpt.SetParameterValue("TermOfPurchase", purchaseTermId == -1 ? "" : commonWorker.getTermOfPurchaseByKey(purchaseTermId).TermOfPurchaseDescription);

            if (customerAtWHDateFrom == DateTime.MinValue)
            {
                rpt.SetParameterValue("CustomerAtWHDateFrom", "");
                rpt.SetParameterValue("CustomerAtWHDateTo", "");
            }
            else
            {
                rpt.SetParameterValue("CustomerAtWHDateFrom", DateTimeUtility.getDateString(customerAtWHDateFrom));
                rpt.SetParameterValue("CustomerAtWHDateTo", DateTimeUtility.getDateString(customerAtWHDateTo));
            }

            if (dataset.OutstandingLCReport.Rows.Count != 0)
            {             
                rpt.SetParameterValue("Supplier", vendorId == -1 ? "" : dataset.OutstandingLCReport.Rows[0]["Supplier"].ToString());
                rpt.SetParameterValue("CO", countryOfOriginId == -1 ? "" : dataset.OutstandingLCReport.Rows[0]["CO"].ToString());
                rpt.SetParameterValue("CustomerDestination", customerDestinationId == -1 ? "" : dataset.OutstandingLCReport.Rows[0]["Destination"].ToString());
            }
            else
            {
                rpt.SetParameterValue("Supplier", vendorId == -1 ? "" : VendorWorker.Instance.getVendorByKey(vendorId).Name);
                rpt.SetParameterValue("CO", countryOfOriginId == -1 ? "" : GeneralWorker.Instance.getCountryOfOriginByKey(countryOfOriginId).Name);
                rpt.SetParameterValue("CustomerDestination", customerDestinationId == -1 ? "" : commonWorker.getCustomerDestinationByKey(customerDestinationId).DestinationDesc);
            }

            if (productTeamList.Values.Count == 1)
            {
                foreach (object productTeamId in productTeamList.Values)
                {
                    ProductCodeDef pc = GeneralWorker.Instance.getProductCodeDefByKey(Convert.ToInt32(productTeamId));
                    rpt.SetParameterValue("ProductTeam", pc.CodeDescription);
                }
            }
            else
                rpt.SetParameterValue("ProductTeam", "");
            rpt.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return rpt;
        }



        public LCNewApplicationAllocationRpt GenerateLCNewApplicationAllocationReport()
        {
            LCNewApplicationAllocationRpt rpt = new LCNewApplicationAllocationRpt();

            //DataSet ds = ReportWorker.Instance.getLCNewApplicationAllocation();

            rpt.SetDataSource(ReporterWorker.Instance.getLCNewApplicationAllocation().Tables[0]);
            return rpt;

        }


        public LCApplicationSummaryReport getLCApplicationSummaryReport(DateTime lcAppDateFrom, DateTime lcAppDateTo, int lcAppNoFrom, int lcAppNoTo, string lcNoFrom, string lcNoTo, 
                string customerName, ArrayList customerIdList,  string officeName, TypeCollector officeIdList, 
                string handlingOfficeName, int handlingOfficeId, string vendorName, int vendorId, string lcDetail, int userId)
        {
            LCApplicationSummaryReport rpt = new LCApplicationSummaryReport();
            //CommonWorker commonWorker = CommonWorker.Instance;

            TypeCollector tcCustomerIdList = TypeCollector.Inclusive;
            foreach (int id in customerIdList)
                tcCustomerIdList.append(id);
            
            //TypeCollector tcOfficeIdList = TypeCollector.Inclusive;
            //foreach (int id in officeIdList)
            //    tcOfficeIdList.append(id);
            int lcDetailUpdated;
            switch (lcDetail.ToUpper())
            {
                case "UPDATED": lcDetailUpdated = 1; break;
                case "NOT UPDATED": lcDetailUpdated = 0; break;
                case "ALL":
                default: lcDetailUpdated = -1; break;
            }
            DataSet ds = ReporterWorker.Instance.getLCApplicationSummaryReport(lcAppDateFrom, lcAppDateTo, lcAppNoFrom, lcAppNoTo, lcNoFrom, lcNoTo,
                tcCustomerIdList, officeIdList, handlingOfficeId, vendorId, lcDetailUpdated, userId);

            rpt.SetDataSource(ds.Tables[0]);

            rpt.SetParameterValue("LCApplicationDateFrom", (DateTimeUtility.getDateString(lcAppDateFrom)));
            rpt.SetParameterValue("LCApplicationDateTo", (DateTimeUtility.getDateString(lcAppDateTo)));
            rpt.SetParameterValue("LCApplicationNoFrom", (lcAppNoFrom < 0 ? string.Empty : lcAppNoFrom.ToString()));
            rpt.SetParameterValue("LCApplicationNoTo", (lcAppNoTo < 0 ? string.Empty : lcAppNoTo.ToString()));
            rpt.SetParameterValue("LCNoFrom", lcNoFrom);
            rpt.SetParameterValue("LCNoTo", lcNoTo);
            rpt.SetParameterValue("Customer", customerName);
            rpt.SetParameterValue("Office", officeName);
            rpt.SetParameterValue("HandlingOffice", handlingOfficeName);
            rpt.SetParameterValue("Supplier", vendorName);
            rpt.SetParameterValue("LCDetail", lcDetail.ToUpper());
            rpt.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return rpt;
        }

        
        public LCShipmentAmendmentRpt getLCShipmentAmendmentReport(string lcNoFrom, string lcNoTo, int lcBatchNoFrom, int lcBatchNoTo,
            int lcApplicationNoFrom, int lcApplicationNoTo, DateTime lcIssueDateFrom, DateTime lcIssueDateTo, 
            DateTime supplierAWHDateFrom, DateTime supplierAWHDateTo, TypeCollector officeIdList, string officeName, int handlingOfficeId, string handlingOfficeName, int userId)
        {

            LCShipmentAmendmentRpt rpt = new LCShipmentAmendmentRpt();
            CommonWorker commonWorker = CommonWorker.Instance;

            LCShipmentAmendmentReportDs ds = ReporterWorker.Instance.getLCShipmentAmendmentReport(lcNoFrom, lcNoTo, lcBatchNoFrom, lcBatchNoTo,
             lcApplicationNoFrom, lcApplicationNoTo, lcIssueDateFrom, lcIssueDateTo, supplierAWHDateFrom, supplierAWHDateTo, officeIdList, handlingOfficeId);

            //rpt.SetDataSource(ds.Tables[0]);
            rpt.SetDataSource((DataTable)ds.LCShipmentAmendment);

            rpt.SetParameterValue("LCNoFrom", (string.IsNullOrEmpty(lcNoFrom) ? "" : lcNoFrom));
            rpt.SetParameterValue("LCNoTo", (string.IsNullOrEmpty(lcNoTo) ? "" : lcNoTo));
            rpt.SetParameterValue("LCBatchNoFrom", (lcBatchNoFrom == int.MinValue ? "" : "LCB" + lcBatchNoFrom.ToString().PadLeft(6, '0')));
            rpt.SetParameterValue("LCBatchNoTo", (lcBatchNoTo == int.MinValue ? "" : "LCB" + lcBatchNoTo.ToString().PadLeft(6, '0')));
            rpt.SetParameterValue("LCApplicationNoFrom", (lcApplicationNoFrom == int.MinValue ? "" : lcApplicationNoFrom.ToString().PadLeft(6, '0')));
            rpt.SetParameterValue("LCApplicationNoTo", (lcApplicationNoTo == int.MinValue ? "" : lcApplicationNoTo.ToString().PadLeft(6, '0')));
            rpt.SetParameterValue("LCIssueDateFrom", (lcIssueDateFrom == null ? "" : DateTimeUtility.getDateString(lcIssueDateFrom)));
            rpt.SetParameterValue("LCIssueDateTo", (lcIssueDateTo == null ? "" : DateTimeUtility.getDateString(lcIssueDateTo)));
            rpt.SetParameterValue("SupplierAwhDateFrom", (supplierAWHDateFrom == null ? "" : DateTimeUtility.getDateString(supplierAWHDateFrom)));
            rpt.SetParameterValue("SupplierAwhDateTo", (supplierAWHDateTo == null ? "" : DateTimeUtility.getDateString(supplierAWHDateTo)));
            rpt.SetParameterValue("Office", (string.IsNullOrEmpty(officeName) ? "" : officeName));
            rpt.SetParameterValue("HandlingOffice", (string.IsNullOrEmpty(handlingOfficeName) ? "" : handlingOfficeName));
            rpt.SetParameterValue("UserName", GeneralWorker.Instance.getUserByKey(userId).DisplayName);

            return rpt;
        }

        public OrdersForLCCancellationReportDs getOrdersForLCCancellationReport(int officeId, DateTime customerAtWHDateFrom, DateTime customerAtWHDateTo)
        {
            return ReporterWorker.Instance.getOrdersForLCCancellationReport(officeId, customerAtWHDateFrom, customerAtWHDateTo);
        }

    }
}
