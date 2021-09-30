using System;
using System.Collections;
using System.Web;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.datafactory.worker;
using com.next.isam.appserver.account;
using com.next.isam.appserver.common;
using System.Web.UI.WebControls;
using com.next.common.domain.types;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.dataserver;
using com.next.isam.reporter.helper;
using com.next.infra.util;
using com.next.common.domain.module;
using com.next.common.domain.industry.vendor;
using System.Web.UI;
using com.next.isam.webapp.commander;
using com.next.common.appserver;
using com.next.infra.web;
using com.next.isam.webapp.commander.account;

namespace com.next.isam.webapp.nontrade
{
    public partial class NTRoleList : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ArrayList roleList = NonTradeManager.Instance.getNTRoleList();
                this.vwRoleList = roleList;
                this.bindGrid();
            }
        }

        private void bindGrid()
        {
            gv_NTRole.DataSource = this.vwRoleList;
            gv_NTRole.DataBind();
        }

        ArrayList vwRoleList
        {
            get { return (ArrayList)ViewState["RoleList"]; }
            set { ViewState["RoleList"] = value; }
        }



        protected void gv_NTRole_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NTRoleRef row = (NTRoleRef)this.vwRoleList[e.Row.RowIndex];

                ((ImageButton)e.Row.FindControl("lnk_Edit")).CommandArgument = e.Row.RowIndex.ToString();
                ((Label)e.Row.FindControl("lbl_RoleId")).Text = row.RoleId.ToString();
                ((Label)e.Row.FindControl("lbl_RoleName")).Text = row.RoleName;
            }
        }

        protected void gv_NTRole_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            NTRoleRef rf = (NTRoleRef)this.vwRoleList[int.Parse((string)e.CommandArgument)];
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetNTRoleByKey);
            Context.Items.Add(AccountCommander.Param.ntRoleId, rf.RoleId);
            forwardToScreen("NTRole.Edit");
        }
    }
}
