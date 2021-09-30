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
    public partial class UKClaimSearch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private QAIS.ClaimRequestService svc = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            //this.Form.DefaultButton = this.btn_Search.UniqueID;

            if (!this.IsPostBack)
            {
                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
                    this.btn_Create.Enabled = false;

                this.ddl_ClaimType.bindList(UKClaimType.getCollectionValues(), "Name", "Id", GeneralCriteria.ALL.ToString(), "--All--", GeneralCriteria.ALL.ToString());
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
                {
                    this.ddl_Office.Items.Add(new ListItem("HK", "1"));
                    this.ddl_OrderType.Items.Add(new ListItem("VM", "2"));
                }
                else
                {
                    ddl_Office.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--All--", GeneralCriteria.ALL.ToString());
                    ddl_HandlingOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--All--", GeneralCriteria.ALL.ToString());
                    this.ddl_OrderType.Items.Add(new ListItem("-- ALL --", "-1"));
                    this.ddl_OrderType.Items.Add(new ListItem("FOB", "1"));
                    this.ddl_OrderType.Items.Add(new ListItem("VM", "2"));
                }
                this.cblStatus.DataSource = ClaimWFS.getCollectionValues();
                this.cblStatus.DataTextField = "Name";
                this.cblStatus.DataValueField = "Id";
                this.cblStatus.DataBind();
                foreach (ListItem li in this.cblStatus.Items)
                {
                    li.Text += " ";
                    li.Selected = true;
                }

                this.txt_Supplier.setWidth(300);
                this.txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
                this.btn_Search.Attributes.Add("onclick", "return isValidSearch();");
                this.btn_Excel.Attributes.Add("onclick", "return isValidSearch();");
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ukClaimReview.Id) || CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
                    this.btn_Create.Visible = false;
                this.ddl_Office.Attributes.Add("onchange", "updateHandlingOffice();");
            }
            svc = new QAIS.ClaimRequestService();
            ProgressBar.showDMS(this.btn_Excel);
        }

        private List<UKClaimDef> vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (List<UKClaimDef>)ViewState["SearchResult"];
            }
        }

        private void fillSearchContext()
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKClaimList);

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
            Context.Items.Add(AccountCommander.Param.ukClaimTypeId, ddl_ClaimType.SelectedValue);
            Context.Items.Add(AccountCommander.Param.officeId, ddl_Office.SelectedValue);
            Context.Items.Add(AccountCommander.Param.handlingOfficeId, ddl_HandlingOffice.SelectedValue);
            TypeCollector workflowStatusList = TypeCollector.Inclusive;
            foreach (ListItem li in cblStatus.Items)
            {
                if (li.Selected)
                    workflowStatusList.append(int.Parse(li.Value));
            }
            Context.Items.Add(AccountCommander.Param.workflowStatusList, workflowStatusList);                
            Context.Items.Add(AccountCommander.Param.vendorId, this.txt_Supplier.VendorId == int.MinValue ? -1 : this.txt_Supplier.VendorId);
            Context.Items.Add(AccountCommander.Param.contractNo, this.txt_ContractNo.Text.Trim());
            Context.Items.Add(AccountCommander.Param.itemNo, this.txt_ItemNo.Text.Trim());
            Context.Items.Add(AccountCommander.Param.orderType, ddl_OrderType.SelectedValue);
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            fillSearchContext();
            forwardToScreen(null);

            List<UKClaimDef> list = (List<UKClaimDef>)Context.Items[AccountCommander.Param.ukClaimList];

            this.vwSearchResult = list;

            this.gv_UKClaim.DataSource = list;
            this.gv_UKClaim.DataBind();

        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            ddl_Office.SelectedIndex = -1;
            ddl_ClaimType.SelectedIndex = -1;
            ddl_OrderType.SelectedIndex = -1;
            this.txt_Supplier.clear();
            this.txt_DateFrom.Text = String.Empty;
            this.txt_DateTo.Text = String.Empty;
            this.txt_ReceivedDateFrom.Text = String.Empty;
            this.txt_ReceivedDateTo.Text = String.Empty;
            this.txt_UKDebitNoteNo.Text = String.Empty;
            this.txt_ItemNo.Text = String.Empty;
            this.txt_ContractNo.Text = String.Empty;

            foreach (ListItem li in this.cblStatus.Items)
                li.Selected = true;

            List<UKClaimDef> list = new List<UKClaimDef>();
            this.vwSearchResult = list;
            this.gv_UKClaim.DataSource = list;
            this.gv_UKClaim.DataBind();
        }

        protected void gv_UKClaim_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UKClaimDef def = vwSearchResult[e.Row.RowIndex];
                
                ImageButton imgBtn = (ImageButton)e.Row.FindControl("imgEdit");
                /*
                imgBtn.Visible = !(def.WorkflowStatus == ClaimWFS.CANCELLED);
                */
                imgBtn.CommandArgument = e.Row.RowIndex.ToString();

                /*
                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
                    imgBtn.Enabled = false;
                */

                imgBtn = (ImageButton)e.Row.FindControl("imgDelete");
                imgBtn.Visible = !(def.WorkflowStatus == ClaimWFS.CANCELLED || def.WorkflowStatus == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER || (def.WorkflowStatus == ClaimWFS.SUBMITTED && def.Type.Id != UKClaimType.AUDIT_FEE.Id && def.Type.Id != UKClaimType.OTHERS.Id && def.Type.Id != UKClaimType.GB_TEST.Id && def.Type.Id != UKClaimType.BILL_IN_ADVANCE.Id));
                imgBtn.CommandArgument = e.Row.RowIndex.ToString();
                imgBtn.Attributes.Add("onclick", "return confirm('Are you sure to delete this Next claim record?');");

                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
                    imgBtn.Enabled = false;

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ukClaimReview.Id))
                    imgBtn.Visible = false;

                if (def.ContractNo != string.Empty)
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo + "/" + def.ContractNo;
                else
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo;

                ((Label)e.Row.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);
                if (def.Type == UKClaimType.MFRN)
                    ((Label)e.Row.FindControl("lbl_ClaimType")).Text = def.Type.Name + " (" + def.ClaimMonth + ")";
                else
                    ((Label)e.Row.FindControl("lbl_ClaimType")).Text = def.Type.Name;
                if (def.OfficeId == OfficeId.HK.Id && def.TermOfPurchaseId != 1)
                {
                    if (def.SZVendor == null)
                        ((Label)e.Row.FindControl("lbl_Vendor")).Text = def.Vendor.Name + " (Not Set)"; 
                    else
                        ((Label)e.Row.FindControl("lbl_Vendor")).Text = def.Vendor.Name + " (" + def.SZVendor.Name + ")"; 
                }
                else
                {
                    ((Label)e.Row.FindControl("lbl_Vendor")).Text = def.Vendor.Name;
                }
                ProductCodeRef codeRef = CommonUtil.getProductCodeByKey(def.ProductTeamId);
                if (codeRef == null)
                    ((Label)e.Row.FindControl("lbl_ProductTeam")).Text = "N/A";
                else
                    ((Label)e.Row.FindControl("lbl_ProductTeam")).Text = codeRef.CodeDescription;
                ((Label)e.Row.FindControl("lbl_UKDebitNoteNo")).Text = def.UKDebitNoteNo;
                ((Label)e.Row.FindControl("lbl_Currency")).Text = def.Currency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_Amount")).Text = def.Amount.ToString("#,###.00");
                ((Label)e.Row.FindControl("lbl_WorkflowStatus")).Text = def.WorkflowStatus.Name;
                if (def.UKDebitNoteDate != DateTime.MinValue)
                    ((Label)e.Row.FindControl("lbl_DebitNoteDate")).Text = def.UKDebitNoteDate.ToString("dd/MM/yyyy");
                ((Label)e.Row.FindControl("lbl_NSPercent")).Text = "N/A";
                ((Label)e.Row.FindControl("lbl_VendorPercent")).Text = "N/A";
                if (def.ClaimRequestId > 0)
                {
                    QAIS.ClaimRequestDef crDef = svc.GetClaimRequestByKey(def.ClaimRequestId);
                    ((Label)e.Row.FindControl("lbl_NSPercent")).Text = crDef.NSRechargePercent.ToString();
                    ((Label)e.Row.FindControl("lbl_VendorPercent")).Text = crDef.VendorRechargePercent.ToString();
                    if (crDef.NSRechargePercent > 0) e.Row.BackColor = System.Drawing.Color.Yellow;
                }
                if (def.SettlementOption == UKClaimSettlemtType.PROVISION)
                    e.Row.BackColor = System.Drawing.Color.Yellow;
            }
        }

        protected void gv_UKClaim_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUKClaim")
            {
                UKClaimDef def = (UKClaimDef)this.vwSearchResult[int.Parse((string)e.CommandArgument)];

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKClaimByKey);
                Context.Items.Add(AccountCommander.Param.claimId, def.ClaimId);
                forwardToScreen("UKClaim.Edit");
            }
            if (e.CommandName == "DeleteUKClaim")
            {
                UKClaimDef def = (UKClaimDef)this.vwSearchResult[int.Parse((string)e.CommandArgument)];

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.DeleteUKClaim);
                Context.Items.Add(AccountCommander.Param.claimId, def.ClaimId);
                forwardToScreen(null);
                btn_Search_Click(null, null);
            }

        }

        protected void btn_Create_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.CreateUKClaim);
            forwardToScreen("UKClaim.Edit");

        }

        protected void btn_Excel_Click(object sender, EventArgs e)
        {
            this.fillSearchContext();
            forwardToScreen("reporter.UKClaimList");

        }

        protected void btnOSReport_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetOSUKClaimList);
            Context.Items.Add(AccountCommander.Param.officeId, this.ddl_Office.SelectedValue);
            Context.Items.Add(AccountCommander.Param.orderType, this.ddl_OrderType.SelectedValue);
            Context.Items.Add(AccountCommander.Param.cutoffDate, DateTime.Today);
            forwardToScreen("reporter.OSUKClaimList");
        }

        protected void btnMail_Click(object sender, EventArgs e)
        {
            ArrayList selectedList = new ArrayList();

            foreach (GridViewRow row in gv_UKClaim.Rows)
            {
                CheckBox ckb = (CheckBox)row.FindControl("chkMail");
                if (ckb.Checked)
                {
                    UKClaimDef def = (UKClaimDef)vwSearchResult[row.RowIndex];
                    selectedList.Add(def.ClaimId.ToString());
                }
            }

            if (selectedList.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ukdn", "alert('No record(s) were selected.');", true);
                return;
            }
            if (selectedList.Count > 30)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ukdn", "alert('Selection must not exceed 30 claims');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.MailUKClaimDN);

            Context.Items.Add(AccountCommander.Param.ukClaimIdList, selectedList);

            forwardToScreen(null);

            btn_Search_Click(null, null);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ukdn", "alert('Mail has been sent');", true);
        }

    }
}
