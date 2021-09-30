using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.domain.account;
using com.next.isam.domain.nontrade;
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

namespace com.next.isam.webapp.nontrade
{
    public partial class GenSunAccountInterfaceFile : com.next.isam.webapp.usercontrol.PageTemplate
    {
        bool isSuperAccess = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.chkMacro.Enabled = false;
                isSuperAccess = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.sunInterface.Id, ISAMModule.sunInterface.Super);
                this.hidSuper.Value = (isSuperAccess ? "Y" : "N");
                this.ddl_SunInterfaceType.Attributes.Add("onchange", "updateControls();");

                ArrayList userOfficeList = NonTradeManager.Instance.getNTUserOfficeList(this.LogonUserId, NTRoleType.SUN_INTERFACE.Id, GeneralCriteria.ALL);
                /*
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList, true);
                */

                this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId");
                if (userOfficeList.Count == 1)
                {
                    OfficeRef oref = (OfficeRef)userOfficeList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                }
                ArrayList sunInterfaceTypeList = (ArrayList) SunInterfaceTypeRef.getNonTradeCollectionValues();
                foreach (int i in sunInterfaceTypeList)
                    this.ddl_SunInterfaceType.Items.Add(new System.Web.UI.WebControls.ListItem(SunInterfaceTypeRef.getDescription(i), i.ToString()));

                this.bindGrid();

                this.ddl_Office_SelectedIndexChanged(null, null);
                this.ddl_Year_SelectedIndexChanged(null, null);
            }
        }

        private void bindGrid()
        {
            this.vwOutstandingQueueList = NonTradeManager.Instance.getRecentNTSunInterfaceQueue();
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
                NTSunInterfaceQueueDef def = (NTSunInterfaceQueueDef)this.vwOutstandingQueueList[(this.gv_Request.PageIndex * this.gv_Request.PageSize) + e.Row.RowIndex];

                ((Label)e.Row.FindControl("lbl_RequestId")).Text = def.QueueId.ToString();
                ((Label)e.Row.FindControl("lbl_SunInterfaceType")).Text = SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                ((Label)e.Row.FindControl("lbl_Office")).Text = def.Office.OfficeCode;
                ((Label)e.Row.FindControl("lbl_Period")).Text = def.FiscalYear.ToString() + "/" + def.Period.ToString().PadLeft(2, '0');

                ((Label)e.Row.FindControl("lbl_Phase")).Text = def.CategoryType.Name;
                ((Label)e.Row.FindControl("lbl_User")).Text = def.User.DisplayName;
                ((Label)e.Row.FindControl("lbl_RequestTime")).Text = def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss");
                ((Label)e.Row.FindControl("lbl_CompletedTime")).Text = (def.CompleteTime != DateTime.MinValue ? def.CompleteTime.ToString("dd/MM/yyyy HH:mm:ss") : "N/A");
                ((Label)e.Row.FindControl("lbl_MacroEnabled")).Text = (def.SourceId == 2 ? "Y" : "N");
                ((Label)e.Row.FindControl("lbl_JournalNo")).Text = def.JournalNo;
            }
        }

        protected void btn_Process_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                NTSunInterfaceQueueDef def = new NTSunInterfaceQueueDef();
                def.QueueId = -1;
                def.Office = CommonUtil.getOfficeRefByKey(int.Parse(this.ddl_Office.SelectedValue));
                def.SunInterfaceTypeId = int.Parse(this.ddl_SunInterfaceType.SelectedValue);
                def.CategoryType = CategoryType.ACTUAL;
                def.User = CommonUtil.getUserByKey(this.LogonUserId);
                if (!chkMacro.Checked)
                    def.SourceId = 1;
                else
                    def.SourceId = 2;
                def.SubmitTime = DateTime.Now;
                def.FiscalYear = int.Parse(this.ddl_Year.SelectedValue);
                def.Period = int.Parse(this.ddl_Period.SelectedValue);

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GenerateNTSunInterface);
                Context.Items.Add(AccountCommander.Param.ntSunInterfaceQueue, def);

                forwardToScreen(null);
                this.bindGrid();
            }
        }

        protected void valSubmission_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (this.ddl_Period.Items.Count == 0 || this.ddl_Year.Items.Count == 0)
            {
                valSubmission.ErrorMessage = "Invalid Fiscal Year & Period";
                args.IsValid = false;
            }
        }

        protected void gv_Request_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_Request.PageIndex = e.NewPageIndex;
            gv_Request.DataSource = this.vwOutstandingQueueList;
            gv_Request.DataBind();
        }

        private void refreshFiscalYearList()
        {
            int year = 0;
            this.ddl_Year.Items.Clear();
            this.ddl_Period.Items.Clear();

            if (this.ddl_Office.Items.Count > 0)
            {
                List<NTMonthEndStatusDef> list = NonTradeManager.Instance.getActiveNTMonthEndStatusList(int.Parse(this.ddl_Office.SelectedValue), -1, -1);
                foreach (NTMonthEndStatusDef def in list)
                {
                    if ((int.Parse(this.ddl_SunInterfaceType.SelectedValue) == SunInterfaceTypeRef.Id.NonTradeAccrual.GetHashCode() && def.Status == NTMonthEndStatusDef.CLOSING.GetHashCode())
                        || int.Parse(this.ddl_SunInterfaceType.SelectedValue) != SunInterfaceTypeRef.Id.NonTradeAccrual.GetHashCode())
                    {

                        if (year != def.FiscalYear)
                            this.ddl_Year.Items.Add(new ListItem(def.FiscalYear.ToString(), def.FiscalYear.ToString()));
                        year = def.FiscalYear;
                    }
                }
            }
            if (this.ddl_Year.Items.Count > 0) ddl_Year_SelectedIndexChanged(null, null);
        }

        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.refreshFiscalYearList();
        }

        protected void ddl_SunInterfaceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.refreshFiscalYearList();
        }

        protected void ddl_Year_SelectedIndexChanged(object sender, EventArgs e)
        {
            int period = 0;
            this.ddl_Period.Items.Clear();
            if (this.ddl_Year.Items.Count > 0)
            {
                List<NTMonthEndStatusDef> list = NonTradeManager.Instance.getActiveNTMonthEndStatusList(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), -1);
                foreach (NTMonthEndStatusDef def in list)
                {
                    if ((int.Parse(this.ddl_SunInterfaceType.SelectedValue) == SunInterfaceTypeRef.Id.NonTradeAccrual.GetHashCode() && def.Status == NTMonthEndStatusDef.CLOSING.GetHashCode())
                        || int.Parse(this.ddl_SunInterfaceType.SelectedValue) != SunInterfaceTypeRef.Id.NonTradeAccrual.GetHashCode())
                    {
                        if (period != def.Period)
                            this.ddl_Period.Items.Add(new ListItem(def.Period.ToString(), def.Period.ToString()));
                        period = def.Period;
                    }
                }
            }
        }

    }
}
