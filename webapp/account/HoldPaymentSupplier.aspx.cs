using System;
using System.Web.UI;
using com.next.common.domain.industry.vendor;
using com.next.common.datafactory.worker.industry;
using com.next.infra.web;
using com.next.infra.util;
using com.next.isam.webapp.commander.account;
using com.next.isam.appserver.account;

namespace com.next.isam.webapp.account
{
    public partial class HoldPaymentSupplier : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int supplierId = -1;
                string para = EncryptionUtility.DecryptParam(Request.Params["vendorId"].ToString());

                if (int.TryParse(para, out supplierId))
                {
                    this.vwVendorId = supplierId;
                    VendorRef vendor = VendorWorker.Instance.getVendorByKey(supplierId);
                    lbl_VendorName.Text = vendor.Name;

                    bool isHold = AccountManager.Instance.isSupplierPaymentHold(supplierId);
                    if (isHold)
                    {
                        this.lbl_HoldStatus.Text = "Currently Hold";
                        this.btn_Hold.Visible = false;
                    }
                    else
                    {
                        this.lbl_HoldStatus.Text = "Currently Unhold";
                        this.btn_Unhold.Visible = false;
                    }
                }
                else
                    lbl_VendorName.Text = String.Empty;

            }
        }

        private int vwVendorId
        {
            set
            {
                ViewState["VendorId"] = value;
            }
            get
            {
                return (int)ViewState["VendorId"];
            }
        }

        private void holdPayment(bool isPaymentHold)
        {
            /*
            ArrayList list = new ArrayList();

            ArrayList acceptedList = AccountManager.Instance.getPaymentStatusEnquiryList(String.Empty, 0, 0, 0, -1, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue,
            DateTime.MinValue, DateTime.MinValue, -1, -1, this.vwVendorId, ShippingDocWFS.ACCEPTED.Id, String.Empty);
            list.AddRange(acceptedList);
            ArrayList readyList = AccountManager.Instance.getPaymentStatusEnquiryList(String.Empty, 0, 0, 0, -1, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue,
            DateTime.MinValue, DateTime.MinValue, -1, -1, this.vwVendorId, ShippingDocWFS.READY.Id, String.Empty);
            list.AddRange(readyList);

            ArrayList selectedList = new ArrayList();

            foreach (PaymentStatusEnquiryDef def in list)
            {
                selectedList.Add(def.ShipmentId.ToString() + "," + def.SplitShipmentId.ToString());
            }
            */

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.HoldPayment);

            Context.Items.Add(AccountCommander.Param.vendorId, this.vwVendorId);
            Context.Items.Add(AccountCommander.Param.isPaymentHold, isPaymentHold);
            Context.Items.Add(AccountCommander.Param.remark, this.txt_Remark.Text.Trim());
            forwardToScreen(null);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "HoldPayment", "window.opener.runSearch();window.close();", true);
        }


        protected void btn_Hold_Click(object sender, EventArgs e)
        {
            this.holdPayment(true);
        }

        protected void btn_Unhold_Click(object sender, EventArgs e)
        {
            this.holdPayment(false);
        }

    }
}
