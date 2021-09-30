using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.infra.util;
using com.next.infra.web;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.types;
using com.next.isam.domain.nontrade;
using com.next.isam.appserver.account;

namespace com.next.isam.webapp.nontrade
{
    public partial class NonTradeMonthEndAdmin : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        private void BindData()
        {
            ArrayList officeList = NonTradeManager.Instance.getNTUserOfficeList(this.LogonUserId, NTRoleType.MONTH_END_ADMIN.Id, GeneralCriteria.ALL);
            if (officeList.Count == 0)
            {
                OfficeRef office = new OfficeRef();
                office.OfficeId = int.MinValue;
                office.OfficeCode = string.Empty;
                officeList.Add(office);
            }
            ArrayList monthEndStatusList = WebUtil.getCurrentNTMonthEndStatus(officeList);
            monthEndStatusList.Sort(new NTMonthEndStatusDef.NTMonthEndStatusComparer(NTMonthEndStatusDef.NTMonthEndStatusComparer.CompareType.Office, SortDirection.Ascending));

            gv_NTMonthEnd.DataSource = monthEndStatusList;
            gv_NTMonthEnd.DataBind();
        }

        protected void MonthEndStatusDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NTMonthEndStatusDef def = (NTMonthEndStatusDef)e.Row.DataItem;

                if (def.Status == NTMonthEndStatusDef.OPEN)
                    ((Button)e.Row.FindControl("btn_ChangeStatus")).Text = "Close";
                else if (def.Status == NTMonthEndStatusDef.CLOSING)
                {
                    ((Button)e.Row.FindControl("btn_ChangeStatus")).Text = "Complete";
                    AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByYearPeriod(9, def.Period == 12 ? def.FiscalYear + 1 : def.FiscalYear, def.Period == 12 ? 1 : def.Period + 1);
                 
                    decimal exchangeRate = dataserver.worker.CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, calenderDef.StartDate);
                    if (exchangeRate == 0)
                    {
                        div_alert.Visible = true;
                        lbl_FiscalYear.Text = calenderDef.BudgetYear.ToString();
                        lbl_Period.Text = calenderDef.Period.ToString();
                    }
                }
            }
        }

        protected void MonthEndRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ChangeStatus")
            {
                int officeId = Convert.ToInt32(e.CommandArgument);

                ArrayList monthEndStatusList = WebUtil.getCurrentNTMonthEndStatus(ConvertUtility.createArrayList(CommonUtil.getOfficeRefByKey(officeId)));

                NTMonthEndStatusDef statusDef = (NTMonthEndStatusDef)monthEndStatusList[0];
                if (statusDef.Status == NTMonthEndStatusDef.OPEN)
                    statusDef.Status = NTMonthEndStatusDef.CLOSING;
                else if (statusDef.Status == NTMonthEndStatusDef.CLOSING)
                    statusDef.Status = NTMonthEndStatusDef.CLOSED;

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateNTMonthEndStatus);

                Context.Items.Add(AccountCommander.Param.ntMonthEndStatus, statusDef);

                forwardToScreen(null);

                BindData();
            }
        }
    }
}