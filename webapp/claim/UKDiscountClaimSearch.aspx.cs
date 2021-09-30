using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using com.next.common.domain;
using com.next.common.web.commander;
using System.Web.UI.WebControls;
using com.next.isam.webapp.commander.account;
using com.next.infra.web;
using com.next.common.domain.module;
using com.next.isam.domain.claim;
using com.next.common.domain.types;
using com.next.infra.util;

namespace com.next.isam.webapp.claim
{
    public partial class UKDiscountClaimSearch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private List<UKDiscountClaimDef> vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (List<UKDiscountClaimDef>)ViewState["SearchResult"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
                    this.btn_Create.Enabled = false;

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
                {
                    this.ddl_Office.Items.Add(new ListItem("HK", "1"));
                }
                else
                {
                    ddl_Office.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--All--", GeneralCriteria.ALL.ToString());
                    ddl_HandlingOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--All--", GeneralCriteria.ALL.ToString());
                }

                this.txt_Supplier.setWidth(300);
                this.txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
                this.btn_Search.Attributes.Add("onclick", "return isValidSearch();");
                this.btn_Excel.Attributes.Add("onclick", "return isValidSearch();");
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ukClaimReview.Id) || CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
                    this.btn_Create.Visible = false;
                this.ddl_Office.Attributes.Add("onchange", "updateHandlingOffice();");
                this.txt_ReceivedDateFrom.DateTime = DateTime.Today.AddMonths(-6);
                this.txt_ReceivedDateTo.DateTime = DateTime.Today;
            }
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            fillSearchContext();
            forwardToScreen(null);

            List<UKDiscountClaimDef> list = (List<UKDiscountClaimDef>)Context.Items[AccountCommander.Param.ukClaimList];

            this.vwSearchResult = list;

            this.gv_UKClaim.DataSource = list;
            this.gv_UKClaim.DataBind();
        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            ddl_Office.SelectedIndex = -1;
            this.txt_Supplier.clear();
            this.txt_DateFrom.Text = String.Empty;
            this.txt_DateTo.Text = String.Empty;
            this.txt_ReceivedDateFrom.Text = String.Empty;
            this.txt_ReceivedDateTo.Text = String.Empty;
            this.txt_UKDebitNoteNo.Text = String.Empty;
            this.txt_ItemNo.Text = String.Empty;
            this.txt_ContractNo.Text = String.Empty;

            this.chkNextDNNo.Checked = false;
            this.chkAppliedUKDiscount.Checked = false;

            List<UKDiscountClaimDef> list = new List<UKDiscountClaimDef>();
            this.vwSearchResult = list;
            this.gv_UKClaim.DataSource = list;
            this.gv_UKClaim.DataBind();
        }

        protected void btn_Excel_Click(object sender, EventArgs e)
        {
            this.fillSearchContext();
            forwardToScreen("reporter.UKDiscountClaimList");
        }

        protected void btn_Create_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.CreateUKDiscountClaim);
            forwardToScreen("UKDiscountClaim.Edit");
        }

        protected void btnMail_Click(object sender, EventArgs e)
        {

        }

        protected void btnClaimUpload_Click(object sender, EventArgs e)
        {

        }

        private void fillSearchContext()
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKDiscountClaimList);

            if (txt_DateFrom.Text.Trim() != String.Empty)
            {
                Context.Items.Add(AccountCommander.Param.debitNoteDateFrom, Convert.ToDateTime(txt_DateFrom.Text.Trim()));
                if (txt_DateTo.Text.Trim() == String.Empty)
                    txt_DateTo.Text = txt_DateFrom.Text;
                Context.Items.Add(AccountCommander.Param.debitNoteDateTo, Convert.ToDateTime(txt_DateTo.Text.Trim()));
            }
            else
            {
                Context.Items.Add(AccountCommander.Param.debitNoteDateFrom, DateTime.MinValue);
                Context.Items.Add(AccountCommander.Param.debitNoteDateTo, DateTime.MinValue);
            }

            if (txt_ReceivedDateFrom.Text.Trim() != String.Empty)
            {
                Context.Items.Add(AccountCommander.Param.debitNoteReceivedDateFrom, Convert.ToDateTime(txt_ReceivedDateFrom.Text.Trim()));
                if (txt_ReceivedDateTo.Text.Trim() == String.Empty)
                    txt_ReceivedDateTo.Text = txt_ReceivedDateFrom.Text;
                Context.Items.Add(AccountCommander.Param.debitNoteReceivedDateTo, Convert.ToDateTime(txt_ReceivedDateTo.Text.Trim()));
            }
            else
            {
                Context.Items.Add(AccountCommander.Param.debitNoteReceivedDateFrom, DateTime.MinValue);
                Context.Items.Add(AccountCommander.Param.debitNoteReceivedDateTo, DateTime.MinValue);
            }

            Context.Items.Add(AccountCommander.Param.ukDebitNoteNo, this.txt_UKDebitNoteNo.Text.Trim());
            Context.Items.Add(AccountCommander.Param.officeId, ddl_Office.SelectedValue);
            Context.Items.Add(AccountCommander.Param.handlingOfficeId, ddl_HandlingOffice.SelectedValue);

            Context.Items.Add(AccountCommander.Param.nextDNNo, chkNextDNNo.Checked);
            Context.Items.Add(AccountCommander.Param.appliedUKDiscount, chkAppliedUKDiscount.Checked);

            Context.Items.Add(AccountCommander.Param.vendorId, this.txt_Supplier.VendorId == int.MinValue ? -1 : this.txt_Supplier.VendorId);
            Context.Items.Add(AccountCommander.Param.contractNo, this.txt_ContractNo.Text.Trim());
            Context.Items.Add(AccountCommander.Param.itemNo, this.txt_ItemNo.Text.Trim());
        }

        protected void gv_UKClaim_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUKDiscountClaim")
            {
                UKDiscountClaimDef def = (UKDiscountClaimDef)this.vwSearchResult[int.Parse((string)e.CommandArgument)];

                Context.Items.Clear();
                Context.Items.Add(AccountCommander.Param.claim, def);
                Context.Items.Add(AccountCommander.Param.action, "EDIT");
                forwardToScreen("UKDiscountClaim.Edit");
            }
            if (e.CommandName == "DeleteUKDiscountClaim")
            {
                UKDiscountClaimDef def = (UKDiscountClaimDef)this.vwSearchResult[int.Parse((string)e.CommandArgument)];

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.DeleteUKDiscountClaim);
                Context.Items.Add(AccountCommander.Param.claimId, def.ClaimId);
                forwardToScreen(null);
                btn_Search_Click(null, null);
            }
            if (e.CommandName == "SendAlertToPic")
            {
                UKDiscountClaimDef def = (UKDiscountClaimDef)this.vwSearchResult[int.Parse((string)e.CommandArgument)];

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.SendAlertToPic);
                Context.Items.Add(AccountCommander.Param.contractNo, def.ContractNo);
                forwardToScreen(null);
            }

        }

        protected void gv_UKClaim_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UKDiscountClaimDef def = vwSearchResult[e.Row.RowIndex];

                ImageButton imgBtn = (ImageButton)e.Row.FindControl("imgEdit");
                imgBtn.CommandArgument = e.Row.RowIndex.ToString();

                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
                    imgBtn.Enabled = false;

                imgBtn = (ImageButton)e.Row.FindControl("imgDelete");
                imgBtn.Visible = (def.WorkflowStatus == UKDiscountClaimWFS.OUTSTANDING);
                imgBtn.CommandArgument = e.Row.RowIndex.ToString();
                imgBtn.Attributes.Add("onclick", "return confirm('Are you sure to delete this record?');");

                Button btn = (Button)e.Row.FindControl("btnSendAlertToPIC");
                btn.Attributes.Add("onclick", "return confirm('Are you sure to send the alert to the merchandiser?');");
                btn.CommandArgument = e.Row.RowIndex.ToString();
                btn.Visible = !def.IsUKDiscount;

                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
                    imgBtn.Enabled = false;

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ukClaimReview.Id))
                    imgBtn.Visible = false;

                if (def.ContractNo != string.Empty)
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo + "/" + def.ContractNo;
                else
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo;

                ((Label)e.Row.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);


                ((Label)e.Row.FindControl("lbl_Vendor")).Text = IndustryUtil.getVendorByKey(def.VendorId).Name;

                ProductCodeRef codeRef = CommonUtil.getProductCodeByKey(def.ProductTeamId);
                if (codeRef == null)
                    ((Label)e.Row.FindControl("lbl_ProductTeam")).Text = "N/A";
                else
                    ((Label)e.Row.FindControl("lbl_ProductTeam")).Text = codeRef.CodeDescription;
                ((Label)e.Row.FindControl("lbl_UKDebitNoteNo")).Text = def.UKDebitNoteNo;
                ((Label)e.Row.FindControl("lbl_Currency")).Text = CurrencyId.getCommonName(def.CurrencyId);
                ((Label)e.Row.FindControl("lbl_Amount")).Text = def.Amount.ToString("#,###.00");
                if (def.UKDebitNoteDate != DateTime.MinValue)
                    ((Label)e.Row.FindControl("lbl_DebitNoteDate")).Text = def.UKDebitNoteDate.ToString("dd/MM/yyyy");
                ((CheckBox)e.Row.FindControl("chkAppliedUKDiscount")).Checked = def.IsUKDiscount;
            }
        }

    }
}
