using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain.module;
using com.next.common.domain.dms;
using com.next.isam.domain.claim;
using com.next.infra.web;
using com.next.isam.appserver.claim;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.infra.util;

namespace com.next.isam.webapp.claim
{
    public partial class ViewDiscountLog : com.next.isam.webapp.usercontrol.PageTemplate
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            int claimId = 0;
            UKDiscountClaimDef def = null;

            if (!Page.IsPostBack)
                claimId = int.Parse(EncryptionUtility.DecryptParam(Request.Params["claimId"].ToString()));
            else
                claimId = this.vwUKDiscountClaimDef.ClaimId;

            def = UKClaimManager.Instance.getUKDiscountClaimByKey(claimId);
            this.vwUKDiscountClaimDef = def;

            if (!Page.IsPostBack) this.bindRecord();
        }

        private void bindRecord()
        {
            UKDiscountClaimDef def = this.vwUKDiscountClaimDef;
            this.lblUKDNNo.Text = def.UKDebitNoteNo;
            this.lblItemNo.Text = def.ItemNo;
            this.lblStatus.Text = def.WorkflowStatus.Name;

            List<UKDiscountClaimLogDef> list = UKClaimManager.Instance.getUKDiscountClaimLogListByClaimId(def.ClaimId);

            this.vwDiscountLogList = list;
            this.gvLog.DataSource = list;
            this.gvLog.DataBind();
        }

        private UKDiscountClaimDef vwUKDiscountClaimDef
        {
            get
            {
                return (UKDiscountClaimDef)ViewState["vwUKDiscountClaimDef"];
            }
            set
            {
                ViewState["vwUKDiscountClaimDef"] = value;
            }
        }

        private List<UKDiscountClaimLogDef> vwDiscountLogList
        {
            set
            {
                ViewState["DiscountLogList"] = value;
            }
            get
            {
                return (List<UKDiscountClaimLogDef>)ViewState["DiscountLogList"];
            }
        }

        protected void gvLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UKDiscountClaimLogDef def = (UKDiscountClaimLogDef)this.vwDiscountLogList[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Action")).Text = def.LogText;
                ((Label)e.Row.FindControl("lbl_FromStatus")).Text = (def.FromStatusId == -1 ? "N/A" : UKDiscountClaimWFS.getType(def.FromStatusId).Name);
                ((Label)e.Row.FindControl("lbl_ToStatus")).Text = UKDiscountClaimWFS.getType(def.ToStatusId).Name;
                ((Label)e.Row.FindControl("lbl_User")).Text = CommonUtil.getUserByKey(def.UserId).DisplayName;
                ((Label)e.Row.FindControl("lbl_Date")).Text = def.LogDate.ToString("dd/MM/yyyy HH:mm:ss");
            }

        }

    }
}
