using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.isam.domain.types;
using com.next.isam.appserver.claim;
using com.next.isam.domain.claim;
using com.next.isam.webapp.usercontrol;
using com.next.infra.web;
using com.next.isam.webapp.commander.account;
using com.next.common.web.commander;
using com.next.infra.util;

namespace com.next.isam.webapp.account
{
    public partial class DCNoteVoid : PageTemplate
    {
        public string vwDCNoteNo
        {
            get
            {
                return (ViewState["vwDCNoteNo"] != null) ? (string)ViewState["vwDCNoteNo"] : "";
            }
            set
            {
                ViewState["vwDCNoteNo"] = value;
            }
        }
        public int vwDCNoteId
        {
            get
            {
                return (ViewState["vwDCNoteId"] != null) ? (int)ViewState["vwDCNoteId"] : 0;
            }
            set
            {
                ViewState["vwDCNoteId"] = value;
            }
        }
        protected UKClaimDCNoteDef vwDCNoteDef
        {
            get
            {
                return (ViewState["vwDCNoteDef"] != null) ? (UKClaimDCNoteDef)ViewState["vwDCNoteDef"] : null;
            }
            set
            {
                ViewState["vwDCNoteDef"] = value;
            }
        }
        public string vwOldSupplierName
        {
            get
            {
                return (ViewState["vwOldSupplierName"] != null) ? (string)ViewState["vwOldSupplierName"] : "";
            }
            set
            {
                ViewState["vwOldSupplierName"] = value;
            }
        }
        public int vwOldSupplierId
        {
            get
            {
                return (ViewState["vwOldSupplierId"] != null) ? (int)ViewState["vwOldSupplierId"] : 0;
            }
            set
            {
                ViewState["vwOldSupplierId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request.Params["dnno"] == null || String.IsNullOrEmpty(Request.Params["dnno"].ToString()))
                {
                    uclVendor.Dispose();
                    return;
                }
                vwDCNoteNo = (string)Request.Params["dnno"];
                var def = UKClaimManager.Instance.getUKClaimDCNoteByDCNoteNo(vwDCNoteNo);
                vwDCNoteDef = def;
                if (def == null || def.DCNoteId <= 0)
                {
                    uclVendor.Dispose();
                    return;
                } else {
                    // Check if DNVoid record has created before
                    //UKClaimManager.Instance.voidUKClaimDN(dcNoteDef, this.LogonUserId);
                    if (def.IsVoid)
                    {
                        uclVendor.Dispose();
                        return;
                    }
                }
                vwDCNoteId = def.DCNoteId;
                ddl_VoidType.Items.Add(new ListItem(DebitNoteVoidType.NS_PROVISION.Description, DebitNoteVoidType.NS_PROVISION.SelectedValue));
                ddl_VoidType.Items.Add(new ListItem(DebitNoteVoidType.CHANGE_TO_OTHER_SUPPLIER.Description, DebitNoteVoidType.CHANGE_TO_OTHER_SUPPLIER.SelectedValue));

                uclVendor.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
                if (def.VendorId > 0)
                {
                    uclVendor.VendorId = def.VendorId;
                }
                //uclVendor.setWidth(305);
                var v = IndustryUtil.getVendorByKey(def.VendorId);
                vwOldSupplierName = (v != null) ? v.Name : "";
                vwOldSupplierId = def.VendorId;
                lb_OldSupplierName.Text = "(Orginal Supplier: " + vwOldSupplierName + ")";

                ddl_VoidType.SelectedIndex = 0;
                Page.Header.DataBind();
            }

        }

        protected void btn_Save_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && Context.Request.HttpMethod == "POST")
            {
                saveVoid(vwDCNoteId, ddl_VoidType.SelectedValue, uclVendor.VendorId);
            }
        }

        private void saveVoid(int dcNoteId, string rbVoidTypeValue, int newVendorId)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.VoidDebitNote);
            Context.Items.Add(AccountCommander.Param.dcNote, vwDCNoteDef);            
            DebitNoteVoidType voidType = null;
            if (rbVoidTypeValue == DebitNoteVoidType.NS_PROVISION.SelectedValue)
            {
                voidType = DebitNoteVoidType.NS_PROVISION;
            }
            else if (rbVoidTypeValue == DebitNoteVoidType.CHANGE_TO_OTHER_SUPPLIER.SelectedValue)
            {
                voidType = DebitNoteVoidType.CHANGE_TO_OTHER_SUPPLIER;
                Context.Items.Add(AccountCommander.Param.vendorId, newVendorId);
            }
            Context.Items.Add(AccountCommander.Param.voidType, voidType);

            forwardToScreen(null);
            // Result...
            /*
            UKClaimDCNoteDef dcNoteDef = UKClaimManager.Instance.getUKClaimDCNoteByKey(def.AdjustmentNoteId);
            UKClaimManager.Instance.voidUKClaimDN(dcNoteDef, this.LogonUserId);
            */
            btn_Save.Visible = false;
            ddl_VoidType.Enabled = false;
            uclVendor.Enabled = false;
        }

        /// <summary>
        /// Exclusive for CHANGE_TO_OTHER_SUPPLIER
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cv_NewSupplier_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (ddl_VoidType.SelectedValue == DebitNoteVoidType.CHANGE_TO_OTHER_SUPPLIER.SelectedValue)
            {
                int oldVendorId = vwDCNoteDef.VendorId;
                int newVendorId = uclVendor.VendorId;
                if (oldVendorId == newVendorId)
                {
                    cv_NewSupplier.Text = "Please enter a different supplier";
                    args.IsValid = false;
                }
                else if (newVendorId <= 0)
                {
                    cv_NewSupplier.Text = "Please enter a valid supplier";
                    args.IsValid = false;
                }
            }
        }

    }
}