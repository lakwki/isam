using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.isam.webapp.commander;
using com.next.infra.web;
using com.next.isam.webapp.commander.account;

namespace com.next.isam.webapp.account
{
    public partial class UTDebitNote : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId");

                AccountFinancialCalenderDef finCalDef = CommonUtil.getCurrentFinancialPeriod(9);
                ddl_Year.DataSource = WebUtil.getBudgetYearList();
                ddl_Year.DataBind();
                ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();
                ddl_Period.SelectedValue = finCalDef.Period.ToString();

            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            AccountFinancialCalenderDef calDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Parse(txt_DebitNoteDate.Text.Trim()));

            if (calDef.BudgetYear != int.Parse(ddl_Year.SelectedValue) || calDef.Period != int.Parse(ddl_Period.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DebitNoteDate", "alert('Debit note date does not fall into the selected period.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GenerateUTDebitNote);

            Context.Items.Add(AccountCommander.Param.officeId, Convert.ToInt32(ddl_Office.SelectedValue));
            Context.Items.Add(AccountCommander.Param.fiscalYear, Convert.ToInt32(ddl_Year.SelectedValue));
            Context.Items.Add(AccountCommander.Param.period, Convert.ToInt32(ddl_Period.SelectedValue));
            Context.Items.Add(AccountCommander.Param.debitNoteDate, Convert.ToDateTime(txt_DebitNoteDate.Text));
            Context.Items.Add(AccountCommander.Param.isDraft, rad_DocType.SelectedValue == "0" ? true : false);
            Context.Items.Add(AccountCommander.Param.format, "pdf");

            forwardToScreen(null);
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            AccountFinancialCalenderDef calDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Parse(txt_DebitNoteDate.Text.Trim()));

            if (calDef.BudgetYear != int.Parse(ddl_Year.SelectedValue) || calDef.Period != int.Parse(ddl_Period.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DebitNoteDate", "alert('Debit note date does not fall into the selected period.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GenerateUTDebitNote);

            Context.Items.Add(AccountCommander.Param.officeId, Convert.ToInt32(ddl_Office.SelectedValue));
            Context.Items.Add(AccountCommander.Param.fiscalYear, Convert.ToInt32(ddl_Year.SelectedValue));
            Context.Items.Add(AccountCommander.Param.period, Convert.ToInt32(ddl_Period.SelectedValue));
            Context.Items.Add(AccountCommander.Param.debitNoteDate, Convert.ToDateTime(txt_DebitNoteDate.Text));
            Context.Items.Add(AccountCommander.Param.isDraft, rad_DocType.SelectedValue == "0" ? true : false);
            Context.Items.Add(AccountCommander.Param.format, "excel");

            forwardToScreen(null);
        }
    }
}