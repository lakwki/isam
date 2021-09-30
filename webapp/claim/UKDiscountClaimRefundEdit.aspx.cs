using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.web;
using com.next.isam.domain.claim;
using com.next.isam.appserver.claim;
using com.next.isam.webapp.commander.account;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.infra.util;
using com.next.common.domain.module;

namespace com.next.isam.webapp.claim
{

    public partial class UKDiscountClaimRefundEdit : com.next.isam.webapp.usercontrol.PageTemplate
    {
        bool isSuperAccess = false;
        /*
        string claimId = string.Empty;
        string refundId = string.Empty;
        */

        private int claimId
        {
            get { return (int)ViewState["UKClaimId"]; }
            set { ViewState["UKClaimId"] = value; }
        }

        private int claimRefundId
        {
            get { return (int)ViewState["ClaimRefundId"]; }
            set { ViewState["ClaimRefundId"] = value; }
        }

        private UKDiscountClaimRefundDef vwClaimRefund
        {
            get { return (UKDiscountClaimRefundDef)ViewState["vwClaimRefund"]; }
            set { ViewState["vwClaimRefund"] = value; }
        }

        private UKDiscountClaimDef vwClaim
        {
            get { return (UKDiscountClaimDef)ViewState["vwClaim"]; }
            set { ViewState["vwClaim"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;

            if (!this.IsPostBack)
            {
                UKDiscountClaimRefundDef def;
                /*
                string guid = Request.Params["GuId"];
                */

                string param = null;
                int id;
                
                if (Request.Params["ClaimRefundId"] != null)
                    param = EncryptionUtility.DecryptParam(Request.Params["ClaimRefundId"].ToString());
                if (int.TryParse(param, out id))
                {   
                    // get existing claim refund record
                    Context.Items.Clear();
                    Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                    Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.getUKDiscountClaimRefundByKey);
                    Context.Items.Add(AccountCommander.Param.claimRefundId, id);
                    forwardToScreen(null);
                    def = (UKDiscountClaimRefundDef)Context.Items[AccountCommander.Param.claimRefund];
                    claimRefundId = def.ClaimRefundId;
                    claimId = def.ClaimId;
                }
                else
                {   // new claim refund record
                    param = EncryptionUtility.DecryptParam(Request.Params["ClaimId"].ToString());
                    if (int.TryParse(param, out id))
                        claimId = id;
                    else
                        claimId = -1;

                    claimRefundId = -1;
                    def = new UKDiscountClaimRefundDef();
                    def.ClaimId = claimId;
                    def.Amount = 0;
                    def.ReceivedDate = DateTime.MinValue;
                    def.Remark = string.Empty;
                }
                /*
                claimRefundId = def.ClaimRefundId;
                */

                initControl(def);
                bindRecord(def);
            }
            else 
            {

            }
        }

        private void initControl(UKDiscountClaimRefundDef claimRefund)
        {
            vwClaimRefund = claimRefund;
            bindRecord(claimRefund);
            setControl();
        }

        private void bindRecord(UKDiscountClaimRefundDef def)
        {
            /*
            this.chk_HasUKDN.Checked = def.HasUKDebitNote;
            */
            this.txt_ReceivedDate.Text = (def.ReceivedDate == DateTime.MinValue ? "" : def.ReceivedDate.ToString("dd/MM/yyyy"));
            this.txt_Amt.Text = def.Amount.ToString();
            this.txt_Remark.Text = def.Remark;
        }

        /*
        protected void gv_Refund_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }
        */


        private void setControl()
        {
            UKDiscountClaimRefundDef def = vwClaimRefund;
            //bool state = (def.ClaimId > 0 ? false : true);
            bool state = true;
            this.txt_ReceivedDate.Enabled = state;
            this.txt_Amt.Enabled = state;
            this.txt_Remark.Enabled = state;
            
            /*
            this.txt_CreditNoteNo.Enabled = state;
            this.txt_CreditNoteDate.Enabled = state;
            this.txt_CreditNoteAmt.Enabled = state;
            */
            this.btn_OK.Enabled = state;
            this.btn_Cancel.Enabled = state;
            return;
        }


        protected void valCustom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            decimal amt = 0;
            if (string.IsNullOrEmpty(txt_ReceivedDate.Text))
                Page.Validators.Add(new ValidationError("Please input the Received Date"));
            else
            {
                AccountFinancialCalenderDef calDef = CommonUtil.getAccountPeriodByDate(AppId.ISAM.Code, DateTime.Today);
                if (this.vwClaimRefund.ClaimRefundId <= 0 && DateTime.Parse(txt_ReceivedDate.Text) < calDef.StartDate)
                {
                    Page.Validators.Add(new ValidationError("Received Date must be equal to or later than " + DateTimeUtility.getDateString(calDef.StartDate)));
                }
            }
            if (string.IsNullOrEmpty(txt_Amt.Text))
                Page.Validators.Add(new ValidationError("Please input the Amount"));
            else
            {   
                if (decimal.TryParse(txt_Amt.Text, out amt))
                {
                    /*
                    if (amt < 0)
                        Page.Validators.Add(new ValidationError("Please input a positive Amount"));
                    */
                    if (amt <= 0)
                        Page.Validators.Add(new ValidationError("Please input a valid Amount"));
                }
                else
                    Page.Validators.Add(new ValidationError("Please input a valid Amount"));
            }


            if (this.claimRefundId == -1)
            {
                if (UKClaimManager.Instance.getDiscountRefundSupportingUploadLog(this.claimId).Count < 1)
                    Page.Validators.Add(new ValidationError("Please upload refund supporting first"));
            }
        }

        private UKDiscountClaimRefundDef updateUKDiscountClaimRefund(UKDiscountClaimRefundDef refund)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateUKDiscountClaimRefundDef);
            Context.Items.Add(AccountCommander.Param.claimRefund, refund);
            forwardToScreen(null);
            return (UKDiscountClaimRefundDef)Context.Items[AccountCommander.Param.claimRefund];
        }

        // Button click event

        protected void btn_OK_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            { //Save record
                UKDiscountClaimRefundDef def = vwClaimRefund;
                def.ReceivedDate = (string.IsNullOrEmpty(this.txt_ReceivedDate.Text) ? DateTime.MinValue : DateTime.Parse(this.txt_ReceivedDate.Text));
                def.Amount = decimal.Parse(this.txt_Amt.Text);
                def.Remark = this.txt_Remark.Text.Trim();
                /*
                def.CreditNoteNo = this.txt_CreditNoteNo.Text;
                def.CreditNoteDate = (string.IsNullOrEmpty(this.txt_CreditNoteDate.Text) ? DateTime.MinValue : DateTime.Parse(this.txt_CreditNoteDate.Text));
                def.CreditNoteAmount = decimal.Parse(this.txt_CreditNoteAmt.Text);
                def.IsInterfaced = (def.IsInterfaced == null ? false : def.IsInterfaced);
                def.IsRechargeInterfaced = (def.IsRechargeInterfaced == null ? false : def.IsRechargeInterfaced);
                def.ClaimId = claimId;
                def.ClaimRefundId = int.Parse(claimRefundId);
                Session["UKClaimRefund"] = updateUKClaimRefund(def);
                */
                updateUKDiscountClaimRefund(def);
                //window.returnValue = val;
                //window.close();
                string val = "\t" + def.ClaimRefundId.ToString() + "\t" + def.ReceivedDate.ToString("dd/MM/yyyy") + "\t" + def.Amount.ToString() + "\t" + def.Remark;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "returnRefund", "window.returnValue='"+val+"';window.close()", true);
            }

        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            //this.bindRecord(vwUKClaimDef);
            //initControl(vwClaimRefund);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "returnRefund", "window.returnValue='';window.close()", true);
        }

    }

}