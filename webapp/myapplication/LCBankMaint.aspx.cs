using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.web;
using com.next.infra.smartwebcontrol;
using com.next.infra.util;
using com.next.isam.webapp.commander.admin;
using com.next.isam.domain.common;
using com.next.common.web.commander;

namespace com.next.isam.webapp.myapplication
{
    public partial class LCBankMaint : com.next.isam.webapp.usercontrol.PageTemplate
    {

        ArrayList vwBankList
        {
            get { return (ArrayList)ViewState["BankList"]; }
            set { ViewState["BankList"] = value; }
        }

        ArrayList vwBranchList
        {
            get { return (ArrayList)ViewState["BranchList"]; }
            set { ViewState["BranchList"] = value; }
        }

        BankRef vwBankRef
        {
            get { return (BankRef)ViewState["BankRef"]; }
            set { ViewState["BankRef"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.systemMaintenance");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, SystemMaintenanceCommander.Action.GetLCBank);                

                forwardToScreen(null);

                vwBankList = (ArrayList)Context.Items[SystemMaintenanceCommander.Param.resultList];

                gv_Bank.DataSource = vwBankList;
                gv_Bank.DataBind();
            }
        }

        protected void btn_CreateBank_Click(object sender, EventArgs e)
        {
            pnl_Bank.Visible = false;
            pnl_NewBank.Visible = true;
        }

        protected void btn_Create_Click(object sender, EventArgs e)
        {
            BankRef bankRef = new BankRef();
            bankRef.BankName = txt_BankName.Text.Trim();
            bankRef.Status = 1;

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.systemMaintenance");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, SystemMaintenanceCommander.Action.UpdateLCBank);
            Context.Items.Add(SystemMaintenanceCommander.Param.bankRef, bankRef);

            forwardToScreen(null);

            vwBankList = (ArrayList)Context.Items[SystemMaintenanceCommander.Param.resultList];

            gv_Bank.DataSource = vwBankList;
            gv_Bank.DataBind();

        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            pnl_Bank.Visible = true;
            pnl_NewBank.Visible = false;
            txt_BankName.Text = "";
        }


        protected void BankRowCommand(object sender, GridViewCommandEventArgs arg)
        {
            if (arg.CommandName == "ShowBranch")
            {
                pnl_Bank.Visible = false;
                pnl_Branch.Visible = true;

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.systemMaintenance");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, SystemMaintenanceCommander.Action.GetBankBranchList);
                Context.Items.Add(SystemMaintenanceCommander.Param.bankId, arg.CommandArgument);

                forwardToScreen(null);

                BankRef bank = (BankRef)Context.Items[SystemMaintenanceCommander.Param.bankRef];
                vwBranchList = (ArrayList)Context.Items[SystemMaintenanceCommander.Param.resultList];
                vwBankRef = bank;
                lbl_BankName.Text = bank.BankName;

                gv_Branch.DataSource = vwBranchList;
                gv_Branch.DataBind();

            }
        }



        protected void BankRowEdit(object sender, GridViewEditEventArgs arg)
        {
            gv_Bank.EditIndex = arg.NewEditIndex;

            gv_Bank.DataSource = vwBankList;
            gv_Bank.DataBind();
        }

        protected void BankRowSave(object sender, GridViewUpdateEventArgs arg)
        {
            GridViewRow row = gv_Bank.Rows[arg.RowIndex];
            
            BankRef bank = new BankRef();
            Label lbl = (Label)row.FindControl("lbl_BankId");
            bank.BankId = Convert.ToInt32(lbl.Text);

            TextBox txt = (TextBox)row.FindControl("txt_BankName");
            bank.BankName = txt.Text.Trim();
            bank.Status = 1;
            
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.systemMaintenance");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, SystemMaintenanceCommander.Action.UpdateLCBank);
            Context.Items.Add(SystemMaintenanceCommander.Param.bankRef, bank);

            forwardToScreen(null);

            vwBankList = (ArrayList)Context.Items[SystemMaintenanceCommander.Param.resultList];

            gv_Bank.DataSource = vwBankList;
            gv_Bank.DataBind();

        }

        protected void BankRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            GridViewRow row = gv_Bank.Rows[arg.RowIndex];
            
            BankRef bank = new BankRef();

            Label lbl = (Label)row.FindControl("lbl_BankId");
            bank.BankId = Convert.ToInt32(lbl.Text);

            TextBox txt = (TextBox)row.FindControl("txt_BankName");
            bank.BankName = txt.Text.Trim();
            bank.Status = 0;

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.systemMaintenance");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, SystemMaintenanceCommander.Action.UpdateLCBank);
            Context.Items.Add(SystemMaintenanceCommander.Param.bankRef, bank);

            forwardToScreen(null);

            vwBankList = (ArrayList)Context.Items[SystemMaintenanceCommander.Param.resultList];

            gv_Bank.DataSource = vwBankList;
            gv_Bank.DataBind();
        }

        protected void BankRowCancelEdit(object sender, GridViewCancelEditEventArgs arg)
        {
            gv_Bank.EditIndex = -1;
            gv_Bank.DataSource = vwBankList;
            gv_Bank.DataBind();
        }

        protected void BranchRowEdit(object sender, GridViewEditEventArgs arg)
        {
            gv_Branch.EditIndex = arg.NewEditIndex;

            gv_Branch.DataSource = vwBranchList;
            gv_Branch.DataBind();
        }

        protected void BranchRowSave(object sender, GridViewUpdateEventArgs arg)
        {
            GridViewRow row = gv_Branch.Rows[arg.RowIndex];
            
            BankBranchRef  branch = new BankBranchRef();

            Label lbl = (Label)row.FindControl("lbl_BranchId");
            branch.BankBranchId = Convert.ToInt32(lbl.Text);

            branch.BankId = vwBankRef.BankId;

            TextBox txt = (TextBox)row.FindControl("txt_BranchName");
            branch.BranchName = txt.Text.Trim();

            txt = (TextBox)row.FindControl("txt_Address1");
            branch.Address1 = txt.Text.Trim();
            txt = (TextBox)row.FindControl("txt_Address2");
            branch.Address2 = txt.Text.Trim();
            txt = (TextBox)row.FindControl("txt_Address3");
            branch.Address3 = txt.Text.Trim();
            txt = (TextBox)row.FindControl("txt_Address4");
            branch.Address4 = txt.Text.Trim();

            SmartDropDownList ddl = (SmartDropDownList)row.FindControl("ddl_country");
            branch.Country = CommonUtil.getCountryByKey(Convert.ToInt32(ddl.SelectedValue));

            txt = (TextBox)row.FindControl("txt_ContactPerson");
            branch.ContactPerson = txt.Text.Trim();
            txt = (TextBox)row.FindControl("txt_Phone");
            branch.Phone = txt.Text.Trim();

            branch.Status = 1;

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.systemMaintenance");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, SystemMaintenanceCommander.Action.UpdateBankBranch);
            Context.Items.Add(SystemMaintenanceCommander.Param.branchRef, branch);

            forwardToScreen(null);

            vwBranchList = (ArrayList)Context.Items[SystemMaintenanceCommander.Param.resultList];

            gv_Branch.EditIndex = -1;
            gv_Branch.DataSource = vwBranchList;
            gv_Branch.DataBind();
            

        }

        protected void BranchRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            BankBranchRef branch = (BankBranchRef) vwBranchList[arg.RowIndex];
            branch.Status = 0;

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.systemMaintenance");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, SystemMaintenanceCommander.Action.UpdateBankBranch);
            Context.Items.Add(SystemMaintenanceCommander.Param.branchRef, branch);

            forwardToScreen(null);

            vwBranchList = (ArrayList)Context.Items[SystemMaintenanceCommander.Param.resultList];

            gv_Branch.DataSource = vwBranchList;
            gv_Branch.DataBind();
        }

        protected void BranchRowCancelEdit(object sender, GridViewCancelEditEventArgs arg)
        {
            gv_Branch.EditIndex = -1;
            gv_Branch.DataSource = vwBranchList;
            gv_Branch.DataBind();
        }

        protected void BranchRowDataBound(object sender, GridViewRowEventArgs arg)
        {
            if (arg.Row.RowType == DataControlRowType.DataRow && arg.Row.RowState == DataControlRowState.Edit)
            {
                BankBranchRef branch = (BankBranchRef) vwBranchList[arg.Row.RowIndex];
                SmartDropDownList ddl = (SmartDropDownList) arg.Row.FindControl("ddl_country");
                ddl.bindList(CommonUtil.getCountryList(), "Name", "CountryId", branch.Country.CountryId.ToString());
            }
        }

        protected void btn_Back_Click(object sender, EventArgs e)
        {
            pnl_Branch.Visible = false;
            pnl_Bank.Visible = true;
        }

        protected void btn_CreateBranch_Click(object sender, EventArgs e)
        {
            gv_Branch.Visible = false;
            pnl_CreateBranch.Visible = true;
            btn_Back.Visible = false;
            btn_CreateBranch.Visible = false;
            ddl_country.bindList(CommonUtil.getCountryList(), "Name", "CountryId");
        }

        protected void btn_SaveBranch_Click(object sender, EventArgs e)
        {
            BankBranchRef branch = new BankBranchRef();

            branch.BankId = vwBankRef.BankId;
            branch.BankBranchId = -1;
            branch.BranchName = txt_BranchName.Text.Trim();
            branch.Address1 = txt_Address1.Text.Trim();
            branch.Address2 = txt_Address2.Text.Trim();
            branch.Address3 = txt_Address3.Text.Trim();
            branch.Address4 = txt_Address4.Text.Trim();
            branch.Country = CommonUtil.getCountryByKey(Convert.ToInt32( ddl_country.SelectedValue));
            branch.ContactPerson = txt_ContactPerson.Text.Trim();
            branch.Phone = txt_Phone.Text.Trim();
            branch.Status = 1;

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.systemMaintenance");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, SystemMaintenanceCommander.Action.UpdateBankBranch);
            Context.Items.Add(SystemMaintenanceCommander.Param.branchRef, branch);

            forwardToScreen(null);

            vwBranchList = (ArrayList)Context.Items[SystemMaintenanceCommander.Param.resultList];

            gv_Branch.DataSource = vwBranchList;
            gv_Branch.DataBind();

            gv_Branch.Visible = true;
            pnl_CreateBranch.Visible = false;
            btn_CreateBranch.Visible = true;
            btn_Back.Visible = true;
        }

    }
}
