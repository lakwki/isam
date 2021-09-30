using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.helper;
using com.next.isam.webapp.commander.account;
using com.next.infra.web;
using com.next.common.appserver;
using CrystalDecisions.CrystalReports.Engine;


namespace com.next.isam.webapp.reporter
{
    public partial class UKClaimPhasingReportPage : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

                this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "-1", "--- ALL ---", "-1");
                if (userOfficeList.Count == 1)
                {
                    OfficeRef oref = (OfficeRef) userOfficeList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                }
                //this.rad_GroupBySupplier.Checked = true;
                uclSupplier.setWidth(305);
                uclSupplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
                for (int i = 3; i >= 0; i--)
                    this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - i).ToString(), (calenderDef.BudgetYear - i).ToString()));
                this.ddl_Year.selectByValue(calenderDef.BudgetYear.ToString());
                refreshPanel();
            }
        }

        protected void btn_Print_Click(object sender, EventArgs e)
        {
            SubmitRequest("PDF");
        }
        protected void btn_Export_Click(object sender, EventArgs e)
        {
            SubmitRequest("EXCEL");
        }

        protected void SubmitRequest(string reportFormat)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKClaimPhasingList);

            Context.Items.Add(AccountCommander.Param.fiscalYear, int.Parse(this.ddl_Year.SelectedValue));
            Context.Items.Add(AccountCommander.Param.vendorId, this.uclSupplier.VendorId <= 0 ? -1 : this.uclSupplier.VendorId);
            Context.Items.Add(AccountCommander.Param.officeId, int.Parse(ddl_Office.SelectedValue));
            if (rad_GroupBySupplier.Checked)
                Context.Items.Add(AccountCommander.Param.docType, (rad_SupplierByClaimReason.Checked ? "0" : (rad_SupplierByClaimType.Checked ? "1" : "2")));
            else
                Context.Items.Add(AccountCommander.Param.docType, this.radDetail.Checked ? "1" : "2");
            Context.Items.Add(AccountCommander.Param.period, ddl_PeriodFrom.Text + "," + ddl_PeriodTo.Text);

            if (rad_GroupByOffice.Checked)
            {
                //Context.Items.Add(AccountCommander.Param.reportGroupType, "Office");
                if (reportFormat == "PDF")
                    ReportHelper.export(genReport("ByOffice"), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "UKClaimPhasingByOffice");
                else if (reportFormat == "EXCEL")
                    ReportHelper.export(genReport("ByOffice"), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "UKClaimPhasingByOffice");
            }
            else if (rad_GroupByOfficeClaimType.Checked)
            {
                if (reportFormat == "PDF")
                    ReportHelper.export(genReport("ByOfficeClaimType"), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "UKClaimPhasingByOfficeClaimType");
                else
                    ReportHelper.export(genReport("ByOfficeClaimType"), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "UKClaimPhasingByOfficeClaimType");
            }
            else
                if (rad_GroupByOfficeClaimReason.Checked)
                {
                    if (reportFormat == "PDF")
                        ReportHelper.export(genReport("ByOfficeClaimReason"), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "UKClaimPhasingByOfficeClaimReason");
                    else
                        ReportHelper.export(genReport("ByOfficeClaimReason"), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "UKClaimPhasingByOfficeClaimReason");
                }
                else
                    if (rad_GroupBySupplier.Checked)
                    {
                        Context.Items.Add(AccountCommander.Param.reportGroupType, "Supplier");
                        forwardToScreen("reporter.UKClaimPhasingReport");
                    }
                    else
                        if (rad_GroupByProductTeam.Checked)
                        {
                            Context.Items.Add(AccountCommander.Param.reportGroupType, "ProductTeam");
                            forwardToScreen("reporter.UKClaimPhasingByProductTeamReport");
                        }
        }

        protected ReportClass genReport(string type)
        {
            int fiscalYear = -1, periodFrom = -1, periodTo = -1;
            int vendorId = -1;
            string vendorName = "ALL";

            fiscalYear = Int32.Parse(ddl_Year.SelectedValue);
            periodFrom = int.Parse(ddl_PeriodFrom.Text);
            periodTo = int.Parse(ddl_PeriodTo.Text);
            if (uclSupplier.VendorId != int.MinValue)
            {
                vendorId = uclSupplier.VendorId;
                vendorName = (uclSupplier.KeyTextBox.Text == "" ? "ALL" : IndustryManager.Instance.getVenderRefByKey(Convert.ToInt32(uclSupplier.KeyTextBox.Text)).Name);
            }
            string reportCode = string.Empty;
            if (type == "ByOfficeClaimReason")
                return AccountReportManager.Instance.getUKClaimPhasingReportByOfficeClaimReason(fiscalYear, periodFrom, periodTo, vendorId, vendorName, int.Parse(ddl_Office.SelectedValue), this.LogonUserId, reportCode);
            else if (type == "ByOffice")
                return AccountReportManager.Instance.getUKClaimPhasingReportByOffice(fiscalYear, periodFrom, periodTo, vendorId, vendorName, this.LogonUserId, string.Empty);
            else
                return AccountReportManager.Instance.getUKClaimPhasingReportByOfficeClaimType(fiscalYear, periodFrom, periodTo, vendorId, vendorName, int.Parse(ddl_Office.SelectedValue), this.LogonUserId, string.Empty);
        }


        protected void radGroupBy_CheckedChanged(object sender, EventArgs e)
        {
            refreshPanel();
        }

        protected void refreshPanel()
        {

            if (rad_GroupByOffice.Checked)
            {
                tr_Office.Style.Add("display", "none");
                tr_Summary.Style.Add("display", "none");
                btn_Export.Style.Add("display", "block");
            }
            else if (rad_GroupByOfficeClaimType.Checked)
            {
                tr_Office.Style.Add("display", "block");
                tr_Summary.Style.Add("display", "none");
                btn_Export.Style.Add("display", "block");
            }
            else if (rad_GroupByOfficeClaimReason.Checked)
            {
                tr_Office.Style.Add("display", "block");
                tr_Summary.Style.Add("display", "none");
                btn_Export.Style.Add("display", "block");
            }
            else if (rad_GroupBySupplier.Checked)
            {
                tr_Office.Style.Add("display", "block");
                tr_Summary.Style.Add("display", "block");
                td_ProductTeamReportType.Style.Add("display", "none");
                td_SupplierReportType.Style.Add("display", "block");
                btn_Export.Style.Add("display", "none");

            }
            else if (rad_GroupByProductTeam.Checked)
            {
                tr_Office.Style.Add("display", "block");
                tr_Summary.Style.Add("display", "block");
                td_ProductTeamReportType.Style.Add("display", "block");
                td_SupplierReportType.Style.Add("display", "none");
                btn_Export.Style.Add("display", "none");
            }
        }


         protected void ddl_PeriodFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ddl_PeriodTo.SelectedIndex = ddl_PeriodFrom.SelectedIndex;
        }


    }
}
