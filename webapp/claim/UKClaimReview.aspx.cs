using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using com.next.infra.web;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.claim;
using com.next.common.domain.module;
using com.next.infra.util;

namespace com.next.isam.webapp.claim
{
    public partial class UKClaimReview : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            if (!IsPostBack)
            {
                UKClaimDef def = null;
                string guid = string.Empty;
                string previousPage = string.Empty;
                if (Context.Items["GUID"] != null)
                {
                    guid = Context.Items["GUID"].ToString();
                    previousPage = "UKClaim.ReviewList";
                }
                else if (!string.IsNullOrEmpty(Request.Params["Guid"]))
                    guid = EncryptionUtility.DecryptParam(Request.Params["Guid"].ToString());

                if (!string.IsNullOrEmpty(guid))
                {
                    Context.Items.Clear();
                    Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                    //Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.ReviewUKClaimDN);
                    Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKClaimByGuid);
                    Context.Items.Add(AccountCommander.Param.guid, guid);
                    //Context.Items.Add(AccountCommander.Param.claim, def);
                    forwardToScreen(null);
                    def = (UKClaimDef)Context.Items[AccountCommander.Param.claim];
                    if (def != null)
                    {
                        if (def.WorkflowStatus.Id == ClaimWFS.NEW.Id || def.WorkflowStatus.Id == ClaimWFS.SUBMITTED.Id)
                        {
                            Context.Items.Clear();
                            Context.Items.Add(AccountCommander.Param.claim, def);
                            Context.Items.Add(AccountCommander.Param.action, "REVIEW");
                            forwardToScreen("UKClaim.Edit");
                        }
                        else
                            lblMessage.Text = "Next Claim Debit Note Cannot be Reviewed.";
                    }
                    else
                        lblMessage.Text = "Next Claim Debit Note Cannot Be Found.";
                    this.divMessage.Attributes.Add("style", "block");
                }
                else
                {
                    lblMessage.Text = "Next Claim Debit Note Cannot Be Found.";
                    this.divMessage.Attributes.Add("style", "block");
                }


            }

        }
    }
}