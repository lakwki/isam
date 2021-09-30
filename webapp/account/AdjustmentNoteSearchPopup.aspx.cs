using System;
using System.Web.UI;
using com.next.infra.util;
using com.next.isam.webapp.commander;
using com.next.common.web.commander;
using com.next.common.domain;
using System.Web.UI.WebControls;
using com.next.common.domain.types;
using com.next.isam.domain.claim;
using com.next.isam.appserver.claim;
using com.next.isam.appserver.account;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.types;
using com.next.isam.domain.account;

namespace com.next.isam.webapp.account
{
    public partial class AdjustmentNoteSearchPopup : usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ddl_BaseCurrency.bindList(WebUtil.getCurrencyListForExchangeRate(), "CurrencyCode", "CurrencyId", GeneralCriteria.ALL.ToString(), "--ALL--", GeneralCriteria.ALL.ToString());
                int adjustmentNoteId = int.Parse(EncryptionUtility.DecryptParam(Request.Params["adjustmentNoteId"].ToString()));
                int adjustmentTypeId = int.Parse(Request.Params["adjustmentTypeId"].ToString());
                if (adjustmentTypeId == AdjustmentType.PURCHASE_ADJUSTMENT.Id)
                {
                    AdjustmentNoteDef def = AccountWorker.Instance.GetAdjustmentNoteByKey(adjustmentNoteId);
                    this.lbl_Currency.Text = string.Format("From {0} to :", CommonUtil.getCurrencyByKey(def.CurrencyId).CurrencyCode);
                    ListItem removeItem = ddl_BaseCurrency.Items.FindByText(CurrencyId.getName(def.CurrencyId));
                    ddl_BaseCurrency.Items.Remove(removeItem);
                    lbl_type.Text = AdjustmentType.PURCHASE_ADJUSTMENT.Name;
                    lbl_dcNoteNo.Text = def.AdjustmentNoteNo;
                    lbl_vendor.Text = def.PartyName;
                    lbl_Amount.Text = def.Amount.ToString("#,##0.00");
                }
                else if (adjustmentTypeId == AdjustmentType.UKCLAIM.Id)
                {
                    UKClaimDCNoteDef noteDef = UKClaimManager.Instance.getUKClaimDCNoteByKey(adjustmentNoteId);
                    this.lbl_Currency.Text = string.Format("From {0} to :", CommonUtil.getCurrencyByKey(noteDef.CurrencyId).CurrencyCode);
                    ListItem removeItem = ddl_BaseCurrency.Items.FindByText(CurrencyId.getName(noteDef.CurrencyId));
                    ddl_BaseCurrency.Items.Remove(removeItem);
                    lbl_type.Text = AdjustmentType.UKCLAIM.Name;
                    lbl_dcNoteNo.Text = noteDef.DCNoteNo;
                    lbl_vendor.Text = noteDef.PartyName;
                    lbl_Amount.Text = noteDef.SettledAmount.ToString("#,##0.00");
                }
            }
        }

        protected void btn_Confirm_Click(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                int adjustmentNoteId = int.Parse(EncryptionUtility.DecryptParam(Request.Params["adjustmentNoteId"].ToString()));
                int adjustmentTypeId = int.Parse(Request.Params["adjustmentTypeId"].ToString());
                if (adjustmentTypeId == AdjustmentType.PURCHASE_ADJUSTMENT.Id)
                {
                    AdjustmentNoteDef def = AccountWorker.Instance.GetAdjustmentNoteByKey(adjustmentNoteId);
                    int currencyId = int.Parse(ddl_BaseCurrency.SelectedValue);
                    AccountManager.Instance.changePurchaseAdjustmentCurrency(def, currencyId, this.LogonUserId);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Adjustment Note", "alert('Currency has been updated!'); Close();", true);
                }
                else if (adjustmentTypeId == AdjustmentType.UKCLAIM.Id)
                {
                    UKClaimDCNoteDef noteDef = UKClaimManager.Instance.getUKClaimDCNoteByKey(adjustmentNoteId);
                    int currencyId = int.Parse(ddl_BaseCurrency.SelectedValue);

                    UKClaimManager.Instance.changeUKClaimDCNoteCurrency(noteDef, currencyId, this.LogonUserId);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Next Claim", "alert('Currency has been updated!'); Close();", true);
                }

            }
        }
    }
}