using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using com.next.common.web.commander;
using com.next.common.domain.module;

namespace com.next.isam.webapp.account
{
    public partial class ARnAPIndex : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.InvoiceRegistrationForPayment))
                {
                    row_InvReg.Visible = true;
                }

                /*
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.PaymentFileConversion))
                {
                    row_PaymentFileConversion.Visible = true;
                }

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.PaymentRecordUpdate))
                {
                    row_BankRec.Visible = true;
                }
                */

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.eInvoiceRegistration))
                {
                    row_eInvReg.Visible = true;
                }

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.CreateInvoiceBatch))
                {
                    row_CreateInvBatch.Visible = true;
                }

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.ReceiptAndSettlementMaintenance))
                {
                    row_ARAPMaint.Visible = true;
                }

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.ePaymentAdvice))
                {
                    row_PaymentAdvice.Visible = true;
                }

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.PaymentStatusEnquiry)
                    || CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.PaymentStatusEnquiryNML))
                    {
                    this.row_PaymentStatusEnquiry.Visible = true;
                }

            }
        }
    }
}
