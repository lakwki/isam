using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain.module;
using com.next.common.domain.dms;
using com.next.isam.domain.account;
using com.next.infra.web;
using com.next.isam.appserver.account;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.infra.util;

namespace com.next.isam.webapp.account
{
    public partial class ViewAdvancePaymentLog : com.next.isam.webapp.usercontrol.PageTemplate
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            int paymentId = 0;
            AdvancePaymentDef def = null;

            if (!Page.IsPostBack)
                paymentId = int.Parse(EncryptionUtility.DecryptParam(Request.Params["paymentId"].ToString()));
            else
                paymentId = this.vwAdvancePaymentDef.PaymentId;

            def = AccountManager.Instance.getAdvancePaymentByKey(paymentId);
            this.vwAdvancePaymentDef = def;

            if (!Page.IsPostBack) this.bindRecord();
        }

        private void bindRecord()
        {
            AdvancePaymentDef def = this.vwAdvancePaymentDef;
            this.lblPaymentNo.Text = def.PaymentNo;
            this.lblVendor.Text = def.Vendor.Name;

            List<AdvancePaymentActionHistoryDef> list = AccountManager.Instance.getAdvancePaymentActionHistoryList(def.PaymentId);

            this.vwLogList = list;
            this.gvLog.DataSource = list;
            this.gvLog.DataBind();
        }

        private AdvancePaymentDef vwAdvancePaymentDef
        {
            get
            {
                return (AdvancePaymentDef)ViewState["vwAdvancePaymentDef"];
            }
            set
            {
                ViewState["vwAdvancePaymentDef"] = value;
            }
        }

        private List<AdvancePaymentActionHistoryDef> vwLogList
        {
            set
            {
                ViewState["LogList"] = value;
            }
            get
            {
                return (List<AdvancePaymentActionHistoryDef>)ViewState["LogList"];
            }
        }

        protected void gvLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AdvancePaymentActionHistoryDef def = (AdvancePaymentActionHistoryDef)this.vwLogList[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_Action")).Text = def.Description;
                ((Label)e.Row.FindControl("lbl_User")).Text = CommonUtil.getUserByKey(def.ActionBy).DisplayName;
                ((Label)e.Row.FindControl("lbl_Date")).Text = def.ActionOn.ToString("dd/MM/yyyy HH:mm:ss");
            }

        }

    }
}
