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
using com.next.common.domain.module;
using com.next.isam.appserver.common;
using com.next.isam.domain.common;

namespace com.next.isam.webapp.reporter
{
    public partial class MFRNQtyAnalysisReportPage : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 3).ToString(), (calenderDef.BudgetYear - 3).ToString()));
                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 2).ToString(), (calenderDef.BudgetYear - 2).ToString()));
                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 1).ToString(), (calenderDef.BudgetYear - 1).ToString()));
                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_FiscalYear.selectByValue(calenderDef.BudgetYear.ToString());

                for (int i = 1; i <= 12; i++)
                {
                    this.ddl_PeriodFrom.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(), i.ToString()));
                    this.ddl_PeriodTo.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(), i.ToString()));
                }
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetMFRNQtyAnalysisList);
                Context.Items.Add(AccountCommander.Param.fiscalYear, this.ddl_FiscalYear.SelectedValue);
                Context.Items.Add(AccountCommander.Param.periodFrom, this.ddl_PeriodFrom.SelectedValue);
                Context.Items.Add(AccountCommander.Param.periodTo, this.ddl_PeriodTo.SelectedValue);

                forwardToScreen("reporter.MFRNQtyAnalysisReport");
            }
        }

        protected void valCustom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            decimal d = 0;
            if (int.Parse(this.ddl_PeriodTo.SelectedValue) < int.Parse(this.ddl_PeriodFrom.SelectedValue))
                Page.Validators.Add(new ValidationError("Invalid Period Range"));

        }
    }
}
