using System;
using com.next.common.web.commander;
using com.next.isam.webapp.usercontrol;
using com.next.common.domain.module;

namespace com.next.isam.webapp.account
{
    public partial class DCNoteIndex : PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ukClaimReview.Id))
            {
                this.tr_Adjustment_Search.Visible = false;
                this.tr_AP_Adjustment.Visible = false;
                this.tr_AR_Adjustment.Visible = false;
                this.tr_MockShop.Visible = false;
                this.tr_UT.Visible = false;
                this.tr_QA.Visible = false;
                this.tr_UKClaimRecharge.Visible = false;
                this.tr_COOApproval.Visible = false;
                this.tr_MFRNBatchUpload.Visible = false;
                this.tr_NextClaimUpload.Visible = false;
                this.tr_ILSDiffDCNote.Visible = false;
                this.tr_AP_OtherChargeDCNote.Visible = false;
                this.tr_AR_OtherChargeDCNote.Visible = false;
                this.tr_SendThirdPartyCustomerDNToUK.Visible = false;
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
            {
                this.tr_AP_Adjustment.Visible = false;
                this.tr_AR_Adjustment.Visible = false;
                this.tr_MockShop.Visible = false;
                this.tr_UT.Visible = false;
                this.tr_QA.Visible = false;
                this.tr_MFRNBatchUpload.Visible = false;
                this.tr_NextClaimUpload.Visible = false;
                this.tr_ILSDiffDCNote.Visible = false;
                this.tr_AP_OtherChargeDCNote.Visible = false;
                this.tr_AR_OtherChargeDCNote.Visible = false;
                this.tr_SendThirdPartyCustomerDNToUK.Visible = false;
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.InternalAudit))
            {
                this.tr_Adjustment_Search.Visible = false;
                this.tr_AP_Adjustment.Visible = false;
                this.tr_AR_Adjustment.Visible = false;
                this.tr_MockShop.Visible = false;
                this.tr_UT.Visible = false;
                this.tr_QA.Visible = false;
                this.tr_COOApproval.Visible = false;
                this.tr_MFRNBatchUpload.Visible = false;
                this.tr_NextClaimUpload.Visible = false;
                this.tr_ILSDiffDCNote.Visible = false;
                this.tr_AP_OtherChargeDCNote.Visible = false;
                this.tr_AR_OtherChargeDCNote.Visible = false;
                this.tr_SendThirdPartyCustomerDNToUK.Visible = false;
            }

        }
    }
}
