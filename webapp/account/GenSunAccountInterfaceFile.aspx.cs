using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.common.datafactory.worker;
using com.next.isam.appserver.account;
using System.Web.UI.WebControls;
using com.next.isam.webapp.commander.account;
using com.next.isam.webapp.commander;
using com.next.infra.web;
using com.next.isam.domain.common;
using com.next.isam.appserver.common;
using com.next.common.domain.module;

namespace com.next.isam.webapp.account
{
    public partial class GenSunAccountInterfaceFile : com.next.isam.webapp.usercontrol.PageTemplate
    {
        bool isSuperAccess = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                isSuperAccess = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.sunInterface.Id, ISAMModule.sunInterface.Super);
                this.hidSuper.Value = (isSuperAccess ? "Y" : "N");

                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList, true);

                //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId");
                this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId");
                if (officeGroupList.Count == 1)
                {
                    //OfficeRef oref = (OfficeRef) userOfficeList[0];
                    ReportOfficeGroupRef oref = (ReportOfficeGroupRef)officeGroupList[0];
                    //this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                    this.ddl_Office.SelectedValue = oref.OfficeGroupId.ToString();
                }
                this.ddl_Office.Attributes.Add("onchange", "return updateControl();");
                ArrayList sunInterfaceTypeList = (ArrayList) SunInterfaceTypeRef.getCollectionValues();
                foreach (int i in sunInterfaceTypeList)
                    this.ddl_SunInterfaceType.Items.Add(new System.Web.UI.WebControls.ListItem(SunInterfaceTypeRef.getDescription(i), i.ToString()));
                /*
                ArrayList categoryList = (ArrayList) CategoryType.getCollectionValues();
                foreach (CategoryType categoryType in categoryList)
                {
                    this.ddl_Phase.Items.Add(new System.Web.UI.WebControls.ListItem(categoryType.Name, categoryType.Id.ToString()));
                }
                */
                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 3).ToString(), (calenderDef.BudgetYear - 3).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 2).ToString(), (calenderDef.BudgetYear - 2).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 1).ToString(), (calenderDef.BudgetYear - 1).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_Year.selectByValue(calenderDef.BudgetYear.ToString());

                this.bindGrid();
                this.ddl_SunInterfaceType_SelectedIndexChanged(null, null);
                this.ddl_Phase_SelectedIndexChanged(null, null);
            }
        }

        private void bindGrid()
        {
            this.vwOutstandingQueueList = AccountManager.Instance.getRecentSunInterfaceQueue();
            gv_Request.DataSource = this.vwOutstandingQueueList;
            gv_Request.DataBind();
        }

        ArrayList vwOutstandingQueueList
        {
            get { return (ArrayList)ViewState["OutstandingQueueList"]; }
            set { ViewState["OutstandingQueueList"] = value; }
        }

        protected void gv_Request_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                SunInterfaceQueueDef def = (SunInterfaceQueueDef)this.vwOutstandingQueueList[(this.gv_Request.PageIndex * this.gv_Request.PageSize) + e.Row.RowIndex];

                ((Label)e.Row.FindControl("lbl_RequestId")).Text = def.QueueId.ToString();
                ((Label)e.Row.FindControl("lbl_SunInterfaceType")).Text = SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                ((Label)e.Row.FindControl("lbl_Office")).Text = def.OfficeGroup.GroupName;
                if (def.FiscalYear == 0)
                    ((Label)e.Row.FindControl("lbl_Period")).Text = "N/A";
                else
                    ((Label)e.Row.FindControl("lbl_Period")).Text = def.FiscalYear.ToString() + "/" + def.Period.ToString().PadLeft(2, '0');
                if (def.PurchaseTerm == 0)
                    ((Label)e.Row.FindControl("lbl_PurchaseTerm")).Text = "ALL";
                else if (def.PurchaseTerm == 1)
                    ((Label)e.Row.FindControl("lbl_PurchaseTerm")).Text = "FOB";
                else if (def.PurchaseTerm == 2)
                    ((Label)e.Row.FindControl("lbl_PurchaseTerm")).Text = "VM";
                if (def.UTurn == 0)
                    ((Label)e.Row.FindControl("lbl_UTurn")).Text = "ALL";
                else if (def.UTurn == 1)
                    ((Label)e.Row.FindControl("lbl_UTurn")).Text = "U-Turn";
                else if (def.UTurn == 2)
                    ((Label)e.Row.FindControl("lbl_UTurn")).Text = "Non U-Turn";

                ((Label)e.Row.FindControl("lbl_Phase")).Text = def.CategoryType.Name;
                ((Label)e.Row.FindControl("lbl_User")).Text = def.User.DisplayName;
                ((Label)e.Row.FindControl("lbl_RequestTime")).Text = def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss");
                ((Label)e.Row.FindControl("lbl_CompletedTime")).Text = (def.CompleteTime != DateTime.MinValue ? def.CompleteTime.ToString("dd/MM/yyyy HH:mm:ss") : "N/A");
                ((Label)e.Row.FindControl("lbl_MacroEnabled")).Text = (def.SourceId == 2 ? "Y" : "N");
                /*
                ((Label)e.Row.FindControl("lbl_JournalNo")).Text = def.JournalNo;
                */
            }
        }

        protected void btn_Process_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {

                SunInterfaceQueueDef def = new SunInterfaceQueueDef();
                def.QueueId = -1;
                def.OfficeGroup = WebUtil.getReportOfficeGroupByKey(int.Parse(this.ddl_Office.SelectedValue));
                def.SunInterfaceTypeId = int.Parse(this.ddl_SunInterfaceType.SelectedValue);
                def.CategoryType = CategoryType.getType(int.Parse(this.ddl_Phase.SelectedValue));
                def.User = CommonUtil.getUserByKey(this.LogonUserId);
                def.UTurn = int.Parse(this.ddl_UTurn.SelectedValue);
                if (!chkMacro.Checked)
                    def.SourceId = 1;
                else
                    def.SourceId = 2;
                def.SubmitTime = DateTime.Now;
                if (int.Parse(ddl_Phase.SelectedValue) == CategoryType.REVERSAL.Id)
                {
                    def.FiscalYear = 0;
                    def.Period = 0;
                    def.PurchaseTerm = 0;
                }
                else if (int.Parse(ddl_Phase.SelectedValue) == CategoryType.DAILY.Id)
                {
                    def.FiscalYear = 0;
                    def.Period = 0;
                    def.PurchaseTerm = int.Parse(this.ddl_PurchaseTerm.SelectedValue);
                }
                else
                {
                    def.FiscalYear = int.Parse(this.ddl_Year.SelectedValue);
                    def.Period = int.Parse(this.ddl_Period.SelectedValue);
                    def.PurchaseTerm = int.Parse(this.ddl_PurchaseTerm.SelectedValue);
                }

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GenerateSunInterface);
                Context.Items.Add(AccountCommander.Param.sunInterfaceQueue, def);

                forwardToScreen(null);

                this.bindGrid();
                /*
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "PNotify.prototype.options.styling = 'jqueryui'; $(function(){new PNotify({title: 'Notice',text: 'Request has been submitted'});});", true);
                */
            }
        }

        protected void ddl_Phase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.Parse(ddl_Phase.SelectedValue) == CategoryType.REVERSAL.Id)
            {
                trPurchaseTerm.Visible = false;
                trUTurn.Visible = true;
                trFiscalYear.Visible = false;
                trPeriod.Visible = false;
            }
            else if (int.Parse(ddl_Phase.SelectedValue) == CategoryType.DAILY.Id)
            {
                trPurchaseTerm.Visible = true;
                trUTurn.Visible = true;
                trFiscalYear.Visible = false;
                trPeriod.Visible = false;
            }
            else
            {
                trPurchaseTerm.Visible = true;
                trUTurn.Visible = true; 
                trFiscalYear.Visible = true;
                trPeriod.Visible = true;
            }
        }

        protected void ddl_SunInterfaceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_Phase.Items.Clear();
            if (SunInterfaceTypeRef.getActualDataCommandName(int.Parse(ddl_SunInterfaceType.SelectedValue)) != "N/A" &&
                SunInterfaceTypeRef.getActualDataCommandName(int.Parse(ddl_SunInterfaceType.SelectedValue)) != "GetActualMockShopSales" &&
                SunInterfaceTypeRef.getActualDataCommandName(int.Parse(ddl_SunInterfaceType.SelectedValue)) != "GetActualMockShopSalesCommission")
                ddl_Phase.Items.Add(new ListItem(CategoryType.ACTUAL.Name, CategoryType.ACTUAL.Id.ToString()));
            if (SunInterfaceTypeRef.getAccrualDataCommandName(int.Parse(ddl_SunInterfaceType.SelectedValue)) != "N/A")
                ddl_Phase.Items.Add(new ListItem(CategoryType.ACCRUAL.Name, CategoryType.ACCRUAL.Id.ToString()));
            if (SunInterfaceTypeRef.getRealizedDataCommandName(int.Parse(ddl_SunInterfaceType.SelectedValue)) != "N/A")
                ddl_Phase.Items.Add(new ListItem(CategoryType.REALIZED.Name, CategoryType.REALIZED.Id.ToString()));
            if (SunInterfaceTypeRef.getDailyDataCommandName(int.Parse(ddl_SunInterfaceType.SelectedValue)) != "N/A")
                ddl_Phase.Items.Add(new ListItem(CategoryType.DAILY.Name, CategoryType.DAILY.Id.ToString()));
            if (SunInterfaceTypeRef.getReversalDataCommandName(int.Parse(ddl_SunInterfaceType.SelectedValue), false) != "N/A")
                ddl_Phase.Items.Add(new ListItem(CategoryType.REVERSAL.Name, CategoryType.REVERSAL.Id.ToString()));
            if (SunInterfaceTypeRef.getConsolidatedDataCommandName(int.Parse(ddl_SunInterfaceType.SelectedValue)) != "N/A")
                ddl_Phase.Items.Add(new ListItem(CategoryType.CONSOLIDATED.Name, CategoryType.CONSOLIDATED.Id.ToString()));
            this.ddl_Phase_SelectedIndexChanged(null, null);
        }

        protected void valSubmission_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (int.Parse(ddl_SunInterfaceType.SelectedValue) == SunInterfaceTypeRef.Id.Purchase.GetHashCode()
                && int.Parse(ddl_Phase.SelectedValue) == CategoryType.DAILY.Id)
            {
                SystemParameterRef paraRef = CommonManager.Instance.getSystemParameterByKey(11);
                if (paraRef.ParameterValue == "Y")
                    args.IsValid = false;
            }
        }

        protected void gv_Request_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_Request.PageIndex = e.NewPageIndex;
            gv_Request.DataSource = this.vwOutstandingQueueList;
            gv_Request.DataBind();
        }
    }
}
