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
    public partial class NTRoleEdit : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                NTRoleRef def = (NTRoleRef)Context.Items[AccountCommander.Param.ntRole];
                this.lbl_RoleId.Text = def.RoleId.ToString();
                this.lbl_RoleName.Text = def.RoleName;
                this.txtUser.setWidth(300);
                this.txtUser.initControl(webservices.UclSmartSelection.SelectionList.user);
                ArrayList officeList = CommonUtil.getOfficeList();
                ArrayList companyList = CommonUtil.getCompanyList(-1);
                this.ddl_Office.bindList(officeList, "OfficeCode", "OfficeId", "-1", "-- ALL --", "-1");
                this.ddl_Company.bindList(companyList, "CompanyName", "CompanyId", "-1", "-- ALL --", "-1");

                this.bindGrid();
            }
        }

        private void bindGrid()
        {
            ArrayList userRoleAccessList = NonTradeManager.Instance.getNTUserRoleAccessList(int.Parse(this.lbl_RoleId.Text));
            this.vwUserRoleAccessList = userRoleAccessList;

            gv_User.DataSource = this.vwUserRoleAccessList;
            gv_User.DataBind();
        }

        ArrayList vwUserRoleAccessList
        {
            get { return (ArrayList)ViewState["UserRoleAccessList"]; }
            set { ViewState["UserRoleAccessList"] = value; }
        }

        protected void gv_User_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NTUserRoleAccessDef row = (NTUserRoleAccessDef)this.vwUserRoleAccessList[e.Row.RowIndex];

                ImageButton imgBtn = (ImageButton)e.Row.FindControl("lnk_Delete");
                imgBtn.CommandArgument = e.Row.RowIndex.ToString();
                imgBtn.Attributes.Add("onclick", "return confirm('Are you sure to delete this access record?');");

                ((Label)e.Row.FindControl("lbl_User")).Text = CommonUtil.getUserByKey(row.UserId).DisplayName;
                ((Label)e.Row.FindControl("lbl_Company")).Text = row.CompanyId == -1 ? "ALL" : CommonUtil.getCompanyByKey(row.CompanyId).CompanyName;
                ((Label)e.Row.FindControl("lbl_Office")).Text = row.OfficeId == -1 ? "ALL" : CommonUtil.getOfficeRefByKey(row.OfficeId).OfficeCode;
            }
        }

        protected void gv_User_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                NTUserRoleAccessDef rf = (NTUserRoleAccessDef)this.vwUserRoleAccessList[int.Parse((string)e.CommandArgument)];
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.DeleteNTUserRoleAccess);
                Context.Items.Add(AccountCommander.Param.ntUserRoleAccess, rf);
                Context.Items.Add(AccountCommander.Param.ntRole, NonTradeManager.Instance.getNTRoleByKey(int.Parse(this.lbl_RoleId.Text)));
                forwardToScreen("NTRole.Edit");
            }
        }

        protected void lnkAdd_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                NTUserRoleAccessDef def = new NTUserRoleAccessDef();
                def.Status = GeneralCriteria.TRUE;
                def.RoleId = int.Parse(this.lbl_RoleId.Text);
                def.CompanyId = this.ddl_Company.selectedValueToInt;
                def.OfficeId = this.ddl_Office.selectedValueToInt;
                def.UserId = this.txtUser.UserId;

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateNTUserRoleAccess);
                Context.Items.Add(AccountCommander.Param.ntUserRoleAccess, def);
                Context.Items.Add(AccountCommander.Param.ntRole, NonTradeManager.Instance.getNTRoleByKey(int.Parse(this.lbl_RoleId.Text)));
                forwardToScreen("NTRole.Edit");
            }
        }

        protected void valCustom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (this.txtUser.UserId <= 0)
                Page.Validators.Add(new ValidationError("Please select a user"));
            if (this.ddl_Company.selectedValueToInt != -1 && this.ddl_Office.selectedValueToInt != -1)
            {
                CompanyOfficeMappingRef companyOfficeMappingDef = GeneralWorker.Instance.getCompanyOfficeMappingByCriteria(this.ddl_Company.selectedValueToInt, this.ddl_Office.selectedValueToInt);
                if (companyOfficeMappingDef == null)
                    Page.Validators.Add(new ValidationError("Invalid Company-Office mapping"));
            }

            NTUserRoleAccessDef def = NonTradeManager.Instance.getNTUserRoleAccessByKey(int.Parse(this.lbl_RoleId.Text), this.ddl_Company.selectedValueToInt, this.ddl_Office.selectedValueToInt, this.txtUser.UserId);
            if (def != null && def.Status == GeneralCriteria.TRUE)
                Page.Validators.Add(new ValidationError("Duplicate record is found"));
        }

        protected void gv_User_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gv_User_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (sortExpression == e.SortExpression)
            {
                sortDirection = (sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending);
            }
            else
            {
                sortExpression = e.SortExpression;
                sortDirection = SortDirection.Ascending;
            }

            NTUserRoleAccessDef.NTUserRoleAccessDefComparer.CompareType compareType = NTUserRoleAccessDef.NTUserRoleAccessDefComparer.CompareType.User;

            if (sortExpression == "User")
                compareType = NTUserRoleAccessDef.NTUserRoleAccessDefComparer.CompareType.User;
            else if (sortExpression == "Office")
                compareType = NTUserRoleAccessDef.NTUserRoleAccessDefComparer.CompareType.Office;
            else if (sortExpression == "Company")
                compareType = NTUserRoleAccessDef.NTUserRoleAccessDefComparer.CompareType.Company;

            this.vwUserRoleAccessList.Sort(new NTUserRoleAccessDef.NTUserRoleAccessDefComparer(compareType, sortDirection));
            this.gv_User.DataSource = vwUserRoleAccessList;
            this.gv_User.DataBind();

        }

        private string sortExpression
        {
            get { return (string)ViewState["SortExpression"]; }
            set { ViewState["SortExpression"] = value; }
        }

        private SortDirection sortDirection
        {
            get { return (SortDirection)ViewState["SortDirection"]; }
            set { ViewState["SortDirection"] = value; }

        }

    }
}
