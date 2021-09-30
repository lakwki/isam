using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using com.next.common.web.commander;
using com.next.common.domain.module;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{

    public partial class reports : com.next.isam.webapp.usercontrol.PageTemplate
    {
        ArrayList reportGroup = new ArrayList();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initReportRow();
                initReportGroup();
            }
        }

        private void initReportRow()
        {   // Show the report row according to the access rights
            //for hk account
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.HKAccountsReport))
            {
                displayRow(row_ActualSalesSummary);
                displayRow(row_ShipmentCommission);
                displayRow(row_ShipmentCommissionMockShop);
                displayRow(row_ShipmentCommissionStudioSample);
                //displayRow(row_ShipmentCommissionChoice);
                displayRow(row_OutstandingAP);
                displayRow(row_OutstandingAR);
                displayRow(row_SupplierPayment);
                displayRow(row_CustomerReceipt);
                displayRow(row_InvoiceList);
                displayRow(row_ReceivablePayableForecastReport);
                displayRow(row_SalesAccrualArchiveReport);
                displayRow(row_NSLSZOrderReport);
                displayRow(row_SupplierOrderStatusReport);
                displayRow(row_ReleaseLockSummary);
                displayRow(row_OtherCostSummaryReport);
                displayRow(row_PartialShipment);
                displayRow(row_EziBuyPartialShipment);
                displayRow(row_ActiveSupplierReport);
                displayRow(row_LCStatus);
                displayRow(row_UKClaimSummaryReport);
                displayRow(row_UKClaimPhasingReport);
                displayRow(row_OSUKClaimListReport);
                displayRow(row_OSUKDiscountListReport);
                displayRow(row_UKClaimAuditLogReport);
                displayRow(row_MFRNQtyAnalysisReport);
                displayRow(row_EpicorInterfaceLogReport);
                displayRow(row_CarbonFootprintReport);
                displayRow(row_TradingAFReport);
                displayRow(row_AdvancePaymentReport);
                displayRow(row_LGHoldPaymentReport);
                displayRow(row_POListByOfficeSupplierReport);
                displayRow(row_FutureOrderSummaryBySupplierReport);
                displayRow(row_InvoiceListSummary);
                displayRow(row_NSLEDProfitabilityReport);
                displayRow(row_NSLedSalesInfoReport);
                displayRow(row_NSLedSalesInfoReport_Finance);
                displayRow(row_NSLedSellThruHistoryReport);
                displayRow(row_NSLedActualSalesSummaryReport);
            }
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shippingDataUpload.Id, ISAMModule.shippingDataUpload.SuperUser))
            {
                displayRow(row_NSLedSalesInfoReport_Finance_TBC);
                displayRow(row_NSLedSalesInfoReport_FinanceAdj);
            }

            //Shipping Reports for hk account
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.ShippingReportForHKAccounts))
            {
                displayRow(row_CTSSTW);
                displayRow(row_TradingAFReport);
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.SZAccountsReport))
            {
                displayRow(row_LCApplicationSummaryReport);
                displayRow(row_PartialShipment);
                displayRow(row_EziBuyPartialShipment);
                displayRow(row_WeeklyShipment);
                displayRow(row_UKClaimSummaryReport);
                displayRow(row_UKClaimPhasingReport);
                displayRow(row_OSUKClaimListReport);
                displayRow(row_OSUKDiscountListReport);
                displayRow(row_UKClaimAuditLogReport);
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.InternalAudit))
            {
                displayRow(row_OSUKClaimListReport);
                displayRow(row_OSUKDiscountListReport);
            }

            //for offshore account
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.OffshoreAccountsReport))
            {
                displayRow(row_InvoiceList);
                displayRow(row_OutstandingAR);
                displayRow(row_OutstandingAP);
                displayRow(row_SupplierPayment);
                displayRow(row_CustomerReceipt);
                displayRow(row_ReceivablePayableForecastReport);
                displayRow(row_UKClaimSummaryReport);
                displayRow(row_UKClaimPhasingReport);
                displayRow(row_UKClaimAuditLogReport);
                displayRow(row_TradingAFReport);
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.NMLAccountsReport))
            {
                displayRow(row_InvoiceList);
            }

            // for hk shipping
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.HKShippingReport))
            {
                displayRow(row_ShipmentCommission);
                displayRow(row_ShipmentCommissionMockShop);
                displayRow(row_ShipmentCommissionStudioSample);                
                displayRow(row_InvoiceList);
                displayRow(row_WeeklyShipment);
                displayRow(row_OutstandingBooking);
                displayRow(row_PartialShipment);
                displayRow(row_EziBuyPartialShipment);
                displayRow(row_CTSSTW);
                displayRow(row_OutstandingPaymentReport);
                displayRow(row_LCBatchControl);
                displayRow(row_LCStatus);
                displayRow(row_OutstandingLC);
                displayRow(row_MonthEndSummaryReport);
                displayRow(row_StwDateDiscrepancyReport);
                displayRow(row_LCApplicationSummaryReport);
                displayRow(row_LCShipmentAmendment);
                displayRow(row_OrdersForLCCancellationReport);
                displayRow(row_OutstandingGBTestResult);
                displayRow(row_TradingAFReport);
                displayRow(row_UTDiscrepancyReport);
                displayRow(row_InvoiceListSummary);
            }

            //for offshore shipping
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.OffshoreShippingReport))
            {
                displayRow(row_InvoiceList);
                displayRow(row_WeeklyShipment);
                displayRow(row_OutstandingBooking);
                displayRow(row_PartialShipment);
                displayRow(row_EziBuyPartialShipment);
                displayRow(row_CTSSTW);
                displayRow(row_LCBatchControl);
                displayRow(row_LCStatus);
                displayRow(row_OutstandingLC);
                displayRow(row_LCApplicationSummaryReport);
                displayRow(row_TradingAFReport);
                displayRow(row_UTDiscrepancyReport);
                displayRow(row_InvoiceListSummary);
            }

            //for nsl sz
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.NSLSZReport))
            {
                displayRow(row_PartialShipment);
                displayRow(row_EziBuyPartialShipment);
                displayRow(row_CTSSTW);
                displayRow(row_NSLSZOrderReport);
                displayRow(row_WeeklyShipment);
            }


            //for Merchandisser
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.MerchandiserReport))
            {
                displayRow(row_LCApplicationSummaryReport);
                displayRow(row_PartialShipment);
                displayRow(row_EziBuyPartialShipment);
                displayRow(row_WeeklyShipment);
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.UKAccountsReport))
            {
                displayRow(row_ShipmentCommission);
                displayRow(row_OutstandingAP);
                displayRow(row_OutstandingAR);
                displayRow(row_SupplierPayment);
                displayRow(row_CustomerReceipt);
                displayRow(row_ReceivablePayableForecastReport);
                displayRow(row_InvoiceList);
                displayRow(row_WeeklyShipment);
                displayRow(row_OutstandingBooking);
                displayRow(row_PartialShipment);
                displayRow(row_EziBuyPartialShipment);
                displayRow(row_CTSSTW);
                displayRow(row_LCBatchControl);
                displayRow(row_LCStatus);
                displayRow(row_OutstandingLC);
                displayRow(row_LCApplicationSummaryReport);
            }

            //for Non-Trade Expense
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.NonTradeReport))
            {
                displayRow(row_NonTradeExpenseStatementList);
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.QAAdminReport))
            {
                displayRow(row_PartialShipment);
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.GBTestReport))
            {
                displayRow(row_OutstandingGBTestResult);
            }
        }


        private void initReportGroup()
        {   // Show report group name and set the backgroud color of each group. Draw the underline for each report item.
            string groupColor = "#EEEEEE";
            for (int g = 0; g < reportGroup.Count; g++)
            {
                string groupName = null;
                int lastRow = -1;
                HtmlTableRow row;
                HtmlGenericControl tbody = (HtmlGenericControl)reportGroup[g];
                for (int r = 0; r < tbody.Controls.Count; r++)
                    if (tbody.Controls[r].GetType() == typeof(HtmlTableRow))
                        if (((HtmlTableRow)tbody.Controls[r]).Visible)
                            lastRow = r;
                for (int r = 0; r < tbody.Controls.Count; r++)
                    if (tbody.Controls[r].GetType() == typeof(HtmlTableRow))
                        if ((row = (HtmlTableRow)tbody.Controls[r]).Visible)
                            for (int c = 0; c < row.Controls.Count; c++)
                            {
                                HtmlTableCell cell = ((HtmlTableCell)row.Controls[c]);
                                if (c == 0)
                                    cell.InnerHtml = (groupName == null ? (groupName = tbody.Attributes["GroupName"]) : "&nbsp;");
                                if (c > 0 || (r == lastRow))
                                    cell.Style.Add("BORDER-BOTTOM", "1px solid #C0C0C0");
                            }
                tbody.Style.Add("BACKGROUND-COLOR", groupColor);
                groupColor = (groupColor == "white" ? "#EEEEEE" : "white");
            }
        }

        private void displayRow(HtmlTableRow row)
        {   // Show the report link and report group. insert the report group into array reportGroup
            HtmlGenericControl tbody = (HtmlGenericControl)row.Parent;
            row.Visible = true;
            tbody.Visible = true;
            if (!reportGroup.Contains(tbody))
            {   // Get the report tbody to be displayed, and put into array list in order
                int i;
                for (i = 0; i < reportGroup.Count; i++)
                    if (tbody.ClientID.CompareTo(((HtmlGenericControl)reportGroup[i]).ClientID) < 0)
                        break;
                if (i >= reportGroup.Count)
                    reportGroup.Add(tbody);
                else
                    reportGroup.Insert(i, tbody);
            }
        }
    }
}
