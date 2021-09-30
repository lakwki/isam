using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.common.datafactory.worker;
using com.next.isam.appserver.account;
using com.next.isam.appserver.claim;
using System.Web.UI.WebControls;
using com.next.isam.webapp.commander.account;
using com.next.isam.webapp.commander;
using com.next.infra.web;
using com.next.isam.domain.common;
using com.next.isam.appserver.common;
using com.next.common.domain.module;
using com.next.isam.domain.claim;
using com.next.common.domain.types;
using com.next.infra.util;
using System.Web;

namespace com.next.isam.webapp.claim
{
    public partial class UKClaimApproval : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            if (!this.IsPostBack)
            {
                TypeCollector officeIds = TypeCollector.Inclusive;
                officeIds.append(1);
                List<UKClaimDef> list = UKClaimManager.Instance.getUKClaimApprovalList(officeIds);
                this.vwSearchResult = list;

                this.gv_UKClaim.DataSource = list;
                this.gv_UKClaim.DataBind();

            }
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

        protected void gv_UKClaim_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UKClaimDef def = vwSearchResult[e.Row.RowIndex];

                QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

                List<QAIS.ClaimRequestDef> list = new List<QAIS.ClaimRequestDef>();
                QAIS.ClaimRequestDef selectedRequest = null;

                selectedRequest = svc.GetClaimRequestByKey(def.ClaimRequestId);
                list.Add(selectedRequest);
            
                ImageButton imgBtn = (ImageButton)e.Row.FindControl("imgEdit");
                imgBtn.Attributes.Add("onclick", "window.open('../claim/AttachmentList.aspx?claimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.ClaimId.ToString())) + "', 'attachmentlist', 'width=500,height=500,scrollbars=1,status=0');return false;");

                if (def.ContractNo != string.Empty)
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo + "/" + def.ContractNo;
                else
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo;

                ((Label)e.Row.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);
                ((Label)e.Row.FindControl("lbl_ClaimType")).Text = def.Type.Name;
                ((Label)e.Row.FindControl("lbl_Vendor")).Text = def.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_UKDebitNoteNo")).Text = def.UKDebitNoteNo;
                ((Label)e.Row.FindControl("lbl_Amount")).Text = def.Currency.CurrencyCode + " " + def.Amount.ToString("#,###.00");
                if (def.UKDebitNoteDate != DateTime.MinValue)
                    ((Label)e.Row.FindControl("lbl_DebitNoteDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteDate);
                ((Label)e.Row.FindControl("lbl_FormNo")).Text = selectedRequest.FormNo;
                ((Label)e.Row.FindControl("lbl_NSRechargePercent")).Text = selectedRequest.NSRechargePercent.ToString("0.00") + "%"; 
            }

        }


    }
}
