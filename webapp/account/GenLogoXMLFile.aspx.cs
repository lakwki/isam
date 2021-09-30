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
using com.next.common.domain.types;
using System.Collections.Generic;
using com.next.common.domain.dms;

namespace com.next.isam.webapp.account
{
    public partial class GenLogoXMLFile : com.next.isam.webapp.usercontrol.PageTemplate
    {
        ArrayList vwLogoInterfaceRequestList
        {
            get { return (ArrayList)ViewState["LogoInterfaceRequestList"]; }
            set { ViewState["LogoInterfaceRequestList"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.errorMsg.Text = "";
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList, true);

                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 3).ToString(), (calenderDef.BudgetYear - 3).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 2).ToString(), (calenderDef.BudgetYear - 2).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 1).ToString(), (calenderDef.BudgetYear - 1).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_Year.selectByValue(calenderDef.BudgetYear.ToString());
                this.ddl_Period.SelectedIndex = calenderDef.Period - 1;
                this.bindData();
            }
        }

        protected void btn_Process_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                List<EpicorGLJournalDetailDef> result;
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetTurkeyEpicorGLJournalDetailList);
                Context.Items.Add(AccountCommander.Param.fiscalYear, int.Parse(ddl_Year.SelectedValue));
                Context.Items.Add(AccountCommander.Param.period, int.Parse(ddl_Period.SelectedValue));
                forwardToScreen(null);
                result = (List<EpicorGLJournalDetailDef>)Context.Items[AccountCommander.Param.journal];
                if (result == null)
                {
                    this.errorMsg.Text = "No data found";
                }
                else
                {
                    this.errorMsg.Text = "Fiscal year " + ddl_Year.SelectedValue + " Period " + ddl_Period.SelectedValue + " XML file has been sent to your email. Please check.";
                    bindData(); // reload the list
                }
            }
        }

        protected void bindData()
        {
            vwLogoInterfaceRequestList = AccountManager.Instance.getLogoInterfaceRequestList();
            gv_LogoInterfaceRequest.DataSource = vwLogoInterfaceRequestList;
            gv_LogoInterfaceRequest.DataBind();
        }

        protected void gv_LogoInterfaceRequest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LogoInterfaceRequestDef def = (LogoInterfaceRequestDef)this.vwLogoInterfaceRequestList[(this.gv_LogoInterfaceRequest.PageIndex * this.gv_LogoInterfaceRequest.PageSize) + e.Row.RowIndex];

                ((Label)e.Row.FindControl("lbl_RequestId")).Text = def.RequestId.ToString();
                if (def.FiscalYear == 0)
                    ((Label)e.Row.FindControl("lbl_Period")).Text = "N/A";
                else
                    ((Label)e.Row.FindControl("lbl_Period")).Text = def.FiscalYear.ToString() + "/" + def.Period.ToString().PadLeft(2, '0');
                ((Label)e.Row.FindControl("lbl_User")).Text = def.SubmitUser.DisplayName;
                ((Label)e.Row.FindControl("lbl_CompletedTime")).Text = (def.SubmitTime != DateTime.MinValue ? def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss") : "N/A");
            }
        }

        protected void btn_file_Click(object sender, EventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            DocumentInfoDef doc = new DocumentInfoDef();
            LogoInterfaceRequestDef temp = (LogoInterfaceRequestDef)this.vwLogoInterfaceRequestList[(this.gv_LogoInterfaceRequest.PageIndex * this.gv_LogoInterfaceRequest.PageSize) + int.Parse(btn.CommandArgument)];

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "LOGO Interface"));
            queryStructs.Add(new QueryStructDef("Request Id", temp.RequestId.ToString()));
            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            doc = (DocumentInfoDef)qList[0];
            if (doc.AttachmentInfos.Count > 0)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "myScript", "window.open('GenLogoXMLFilePopup.aspx?requestId=" + temp.RequestId.ToString() + "','_blank', 'toolbar = no, scrollbars = 1');", true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "myScript", "window.open('GenLogoXMLFilePopup.aspx?requestId=" + "-1" + "','_blank', 'toolbar = no, scrollbars = 1, height=20, width=20');", true);
            }
        }

        protected void gv_LogoInterfaceRequest_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_LogoInterfaceRequest.PageIndex = e.NewPageIndex;
            gv_LogoInterfaceRequest.DataSource = this.vwLogoInterfaceRequestList;
            gv_LogoInterfaceRequest.DataBind();
        }

        protected void userGuide_Click(object sender, EventArgs e)
        {
            string fileName = "Logo_Tiger_XML_interface_generation_User_Guide.docx";
            string filePath = Server.MapPath("~/account/Logo_Tiger_XML_interface_generation_User_Guide.docx");
            Response.Clear();

            Response.AppendHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/octet-stream";
            Response.WriteFile(filePath);
            Response.Flush();
            Response.End();

        }

    }
}
