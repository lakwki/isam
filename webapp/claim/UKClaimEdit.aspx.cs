using System;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.IO;
using com.next.infra.util;
using com.next.infra.web;
using com.next.common.domain;
using com.next.common.domain.dms;
using com.next.common.domain.module;
using com.next.common.web.commander;
using com.next.isam.domain.claim;
using com.next.isam.domain.order;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.claim;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.webapp.commander.shipment;
using System.Globalization;
using com.next.common.domain.industry.vendor;
using com.next.common.domain.types;
using System.Web;

namespace com.next.isam.webapp.claim
{
    public partial class UKClaimEdit : com.next.isam.webapp.usercontrol.PageTemplate
    {
        bool isSuperAccess = false;
        string Action = string.Empty;
        string PrevAction = string.Empty;
        string validationType = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;

            if (!this.IsPostBack)
            {
                UKClaimDef def = (UKClaimDef)Context.Items[AccountCommander.Param.claim];
                Action = (string)Context.Items[AccountCommander.Param.action];
                if (def == null)
                {   
                    string guId = Request.Params["Guid"];
                    if (!string.IsNullOrEmpty(guId))
                    {
                        def = getUKClaimByGUId(guId);
                        bool review = (string.IsNullOrEmpty(Request.Params["Review"]) ? false : (Request.Params["Review"].ToLower() == "true"));
                        Action = (review ? "REVIEW" : (def.ClaimId == -1 ? "CREATE" : "EDIT"));
                    }
                    else
                    {   
                        def = createNewUKClaim();
                        Action = "CREATE";
                    }
                }
                this.txt_UKDNReceivedDate.Width = 70;
                this.txt_UKDNDate.Width = 70;
                this.ddl_Office.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "", GeneralCriteria.ALL.ToString());
                this.ddl_PaymentOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "SAME AS OFFICE", GeneralCriteria.ALL.ToString());
                this.ddl_HandlingOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "", GeneralCriteria.ALL.ToString());
                this.ddl_TermOfPurchase.Items.Add(new ListItem("-- ALL --", GeneralCriteria.ALL.ToString()));
                this.ddl_TermOfPurchase.Items.Add(new ListItem("FOB", "1"));
                this.ddl_TermOfPurchase.Items.Add(new ListItem("VM", "2"));
                this.ddl_ClaimType.bindList(UKClaimType.getCollectionValues(), "Name", "Id", "");
                this.ddl_Currency.bindList(WebUtil.getNewCurrencyListForExchangeRate(), "CurrencyCode", "CurrencyId", GeneralCriteria.ALL.ToString(), "--", GeneralCriteria.ALL.ToString());
                this.uclProductTeam.setWidth(300);
                this.uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
                this.txt_Supplier.setWidth(300);
                this.txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
                this.txt_SZSupplier.setWidth(300);
                this.txt_SZSupplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.szVendor);
                this.ddl_Office.Attributes.Add("onchange", "updateHandlingOffice();");
                this.ddl_SettlementOption.bindList(UKClaimSettlemtType.getCollectionValues(), "Name", "Id", string.Empty);

                initControls(def, Action);
                this.vwAction = Action;
                this.btn_Cancel.Attributes.Add("onclick", "window.location='UKClaimSearch.aspx';return false;");

                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
                {
                    this.btn_New.Enabled = false;
                    this.btn_Save.Enabled = false;
                }
            }
            else if (Context.Items[AccountCommander.Param.claimRequestList] != null)
            {
                List<QAIS.ClaimRequestDef> list = (List<QAIS.ClaimRequestDef>)Context.Items[AccountCommander.Param.claimRequestList];
                this.gv_Request.DataSource = list;
                this.gv_Request.DataBind();
            }
            Action = this.vwAction;
        }

        private string vwAction
        {
            get { return (string)ViewState["vwAction"]; }
            set { ViewState["vwAction"] = value; }
        }

        private UKClaimDef vwUKClaimDef
        {
            get {return (UKClaimDef)ViewState["vwUKClaimDef"];}
            set {ViewState["vwUKClaimDef"] = value;}
        }

        private UKClaimBIADiscrepancyDef vwUKClaimBIADiscrepancyDef
        {
            get { return (UKClaimBIADiscrepancyDef)ViewState["vwUKClaimBIADiscrepancyDef"]; }
            set { ViewState["vwUKClaimBIADiscrepancyDef"] = value; }
        }

        private List<QAIS.ClaimRequestDef> vwMapResult
        {
            set {ViewState["MapResult"] = value;}
            get {return (List<QAIS.ClaimRequestDef>)ViewState["MapResult"];}
        }

        private List<UKClaimDef> vwBIAMappingList
        {
            set { ViewState["BIAMappingList"] = value; }
            get { return (List<UKClaimDef>)ViewState["BIAMappingList"]; }
        }

        private List<UKClaimRefundDef> vwRefundList
        {
            set { ViewState["UKClaimRefund"] = value; }
            get { return (List<UKClaimRefundDef>)ViewState["UKClaimRefund"]; }
        }


        #region Access Control

        private void initControls(UKClaimDef def, string action)
        {
            this.vwUKClaimDef = def;
            vwAction = action;
            vwMapResult = null;
            vwBIAMappingList = null;
            vwRefundList = null;
            vwUKClaimBIADiscrepancyDef = null;

            this.bindRecord(def);
            
            this.btn_ViewLog.Attributes.Add("onclick", "window.open('ViewLog.aspx?claimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.ClaimId.ToString())) + "', 'loglist', 'width=800,height=400,scrollbars=1,status=0');return false;");
            this.btn_ViewAttachment.Attributes.Add("onclick", "window.open('AttachmentList.aspx?claimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.ClaimId.ToString())) + "', 'attachmentlist', 'width=500,height=500,scrollbars=1,status=0');return false;");
            this.btn_AddRefund.Attributes.Add("onclick", getRefundButtonEvent(null));
            this.setControl(action);
            this.ddl_ClaimType_SelectedIndexChanged(null, null);
        }

        private void setControl(string action)
        {
            this.btn_ViewLog.Visible = (action != "CREATE");
            this.btn_ViewAttachment.Visible = (action != "CREATE");
            this.btn_New.Visible = (action == "EDIT");

            this.pnlClaimRequestMapping.Visible = (action == "REVIEW" || (this.vwUKClaimDef.ClaimRequestId > 0));
            if (vwUKClaimDef.Type.Id == UKClaimType.BILL_IN_ADVANCE.Id)
                this.pnlClaimRefund.Visible = false;
            else
                this.pnlClaimRefund.Visible = (action == "EDIT");   

            this.pnlBIAMapping.Visible = (vwUKClaimDef.Type.Id == UKClaimType.BILL_IN_ADVANCE.Id && vwUKClaimDef.WorkflowStatus.Id == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id && this.vwBIAMappingList.Count > 0);
            this.pnl_BIADiscrepancy.Visible = (this.vwUKClaimBIADiscrepancyDef != null);
            this.tb_DNToSupplierInfo.Visible = (action != "CREATE" && vwUKClaimDef.WorkflowStatus.Id == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id);
            this.tr_DNCopyUpload.Visible = !(vwUKClaimDef.HasUKDebitNote && vwUKClaimDef.ClaimId != -1);
            this.txt_RemarkHistory.Visible = (vwUKClaimDef.ClaimId != -1);

            if (this.vwUKClaimDef.Type.Id != UKClaimType.BILL_IN_ADVANCE.Id)
            {
                UKClaimBIADiscrepancyDef discrepancyDef = UKClaimManager.Instance.getUKClaimBIADiscrepancyByChildId(vwUKClaimDef.ClaimId);
                if (discrepancyDef != null)
                {
                    this.btn_GoToBIA.Visible = (discrepancyDef.Amount != 0 && !discrepancyDef.IsLocked);
                    this.btn_GoToBIA.Attributes.Add("onclick", "return confirm('The system is now directing you to the associated [Bill In Advance] record');");
                }
            }

            bool enableRadioButton;
            if (vwUKClaimDef.WorkflowStatus.Id == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id || vwUKClaimDef.WorkflowStatus.Id == ClaimWFS.REJECTED.Id)
                enableRadioButton = false;
            else
                enableRadioButton = true;

            for (int i = 0; i < gv_Request.Rows.Count; i++)
            {
                GridViewRow r = gv_Request.Rows[i];
                if (r.RowType == DataControlRowType.DataRow)
                {
                    QAIS.ClaimRequestDef def = vwMapResult[r.RowIndex];
                    ((RadioButton)r.FindControl("radClaimRequest")).Enabled = (enableRadioButton || def.RequestId == vwUKClaimDef.ClaimRequestId);
                }
            }

            switch (action)
            {
                case "CREATE": allowInputClaimInfo(true); 
                    break;
                case "REVIEW": allowInputClaimInfo(false); 
                    break;
                case "EDIT": allowInputClaimInfo((vwUKClaimDef.WorkflowStatus.Id != ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id));
                    this.chk_HasUKDN.Enabled = (string.IsNullOrEmpty(vwUKClaimDef.UKDebitNoteNo));
                    this.txt_UKDNNo.Enabled = this.chk_HasUKDN.Enabled;
                    this.txt_UKDNDate.Enabled = this.chk_HasUKDN.Enabled;
                    this.txt_UKDNReceivedDate.Enabled = this.chk_HasUKDN.Enabled;
                    this.upd_UKDebitNote.Enabled = this.chk_HasUKDN.Enabled;
                    break;
                default: allowInputClaimInfo(false); break;
            }        
            return;
        }

        private void allowInputClaimInfo(bool enable)
        {
            this.chk_HasUKDN.Enabled = enable;
            this.txt_UKDNNo.Enabled = enable;
            this.txt_UKDNDate.Enabled = enable;
            this.txt_UKDNReceivedDate.Enabled = enable;
            this.upd_UKDebitNote.Enabled = enable;

            this.ddl_ClaimType.Enabled = enable;
            this.ddl_Supplier.Enabled = enable;
            this.txt_ItemNo.Enabled = enable;
            this.txt_ContractNo.Enabled = enable;
            this.txt_Qty.Enabled = enable;
            this.txt_Amt.Enabled = enable;
            /*
            this.txt_Desc.Enabled = enable;
            */
            this.ddl_Currency.Enabled = enable;
            this.txt_SZSupplier.Visible = false;

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
            {
                this.txt_Supplier.Enabled = false;
                this.ddl_Supplier.Enabled = false;
                this.ddl_ClaimType.Enabled = false;
                this.txt_ItemNo.Enabled = false;
                this.txt_ContractNo.Enabled = false;
                this.txt_SZSupplier.Visible = true;
            }
        }

        private string getRefundButtonEvent(UKClaimRefundDef refund)
        {
            string param = string.Empty;
            if (refund == null)
                param = "ClaimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(vwUKClaimDef.ClaimId.ToString())); //+ "&UKClaimRefundId=&Amount=&ReceivedDate=&Remark=";
            else
                if (refund.ClaimRefundId > 0)
                    param = "ClaimRefundId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(refund.ClaimRefundId.ToString()));

            return "getRefundResult(window.showModalDialog('UKClaimRefundEdit.aspx?" + param + "', 'Update_UK_Claim_Refund', 'status:No;dialogWidth:600px;dialogHeight:400px;scrollbars:Yes;resizable:Yes;'))";
        }

        #endregion


        #region Data Binding

        private void bindRecord(UKClaimDef def)
        {
            this.chk_HasUKDN.Checked = def.HasUKDebitNote;
            this.lbl_Status.Text = def.WorkflowStatus.Name;
            this.txt_Status.Text = def.WorkflowStatus.Name;
            this.hid_Status.Value = def.WorkflowStatus.Id.ToString();
            this.txt_ContractNo.Text = def.ContractNo;
            this.txt_UKDNNo.Text = def.UKDebitNoteNo;
            this.txt_UKDNDate.Text = DateTimeUtility.getDateString(def.UKDebitNoteDate);
            this.txt_UKDNReceivedDate.Text = DateTimeUtility.getDateString(def.UKDebitNoteReceivedDate);
            this.txt_Qty.Text = def.Quantity.ToString();
            this.txt_Amt.Text = def.Amount.ToString();
            /*
            this.txt_Desc.Text = def.Remark;
            */
            this.txt_Desc.Text = string.Empty;
            this.txt_RemarkHistory.Text = def.Remark;
            this.txtMonth.Text = def.ClaimMonth;
            this.chkIsReadyForSettlement.Checked = def.IsReadyForSettlement;
            this.ddl_SettlementOption.selectByValue(def.SettlementOption.Id.ToString());
            /*
            if (def.WorkflowStatus.Id == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id)
                this.chkIsReadyForSettlement.Enabled = false;
            */
            this.txt_DebitNoteNo.Text = string.Empty;
            this.txt_DebitNoteDate.Text = string.Empty;
            this.txt_DebitNoteAmount.Text = string.Empty;
            this.txt_DebitNoteSettlementDate.Text = string.Empty;
            if (def.WorkflowStatus.Id == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id)
            {
                UKClaimDCNoteDetailDef dcnDetail = UKClaimManager.Instance.getUKClaimDCNoteDetailByLogicalKey(def.ClaimId, 0);
                if (dcnDetail != null)
                {
                    UKClaimDCNoteDef dcNote = UKClaimManager.Instance.getUKClaimDCNoteByKey(dcnDetail.DCNoteId);
                    this.txt_DebitNoteNo.Text = dcNote.DCNoteNo;
                    this.txt_DebitNoteDate.Text = DateTimeUtility.getDateString(dcNote.DCNoteDate);
                    this.txt_DebitNoteAmount.Text = dcNote.SettledAmount.ToString("N02");
                    if (dcNote.SettlementDate == DateTime.MinValue)
                        this.txt_DebitNoteSettlementDate.Text = "N/A";
                    else
                        this.txt_DebitNoteSettlementDate.Text = DateTimeUtility.getDateString(dcNote.SettlementDate);
                }
            }

            this.ddl_Currency.selectByValue(def.Currency.CurrencyId.ToString());
            this.ddl_ClaimType.selectByValue(def.Type.Id.ToString());
            this.txt_ItemNo.Text = def.ItemNo;

            if (def.Type.Id == UKClaimType.AUDIT_FEE.Id || def.Type.Id == UKClaimType.OTHERS.Id)
            {
                this.txt_Supplier.VendorId = def.Vendor.VendorId;
                this.ddl_Office.selectByValue(def.OfficeId.ToString());
                this.ddl_HandlingOffice.selectByValue(def.HandlingOfficeId.ToString());
                this.ddl_TermOfPurchase.selectByValue(def.TermOfPurchaseId.ToString());
            }
            else
            {
                bindSupplierProduct(def);
                this.ddl_Office.selectByValue(def.OfficeId.ToString());
                this.ddl_HandlingOffice.selectByValue(def.HandlingOfficeId.ToString());
            }
            if (def.OfficeId == def.PaymentOfficeId)
                this.ddl_PaymentOffice.selectByValue("-1");
            else
                this.ddl_PaymentOffice.selectByValue(def.PaymentOfficeId.ToString());

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts) && def.SZVendor != null)
                this.txt_SZSupplier.VendorId = def.SZVendor.VendorId;

            bindClaimMappingList(def.ClaimRequestId);
            bindBIAMapping(def);
            bindBIADiscrepancy(def);

            /*
            bindBIADiscrepancy(def.)
            */
            vwRefundList = loadClaimRefundList(def.ClaimId);
            bindClaimRefundList(vwRefundList);

            this.divNoteForTR.Visible = false;
            if (((def.OfficeId == OfficeId.TR.Id || def.OfficeId == OfficeId.EG.Id) && def.UKDebitNoteReceivedDate >= new DateTime(2013, 6, 3))
                || (def.OfficeId == OfficeId.SH.Id && def.Type == UKClaimType.MFRN))
                this.divNoteForTR.Visible = true;
        }

        private void bindSupplierProduct(UKClaimDef claim)
        {
            bindSupplierProduct(claim.ItemNo, claim.ContractNo, (claim.Vendor == null ? -1 : claim.Vendor.VendorId));
        }

        private void bindSupplierProduct(string itemNo, string contractNo)
        {
            bindSupplierProduct(itemNo, contractNo, GeneralCriteria.ALL);
        }

        private void bindSupplierProduct(string itemNo, string contractNo, int vendorId)
        {
            ShipmentProductDef shipment = null;
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.GetShipmentProductByItemNo);
            Context.Items.Add(ShipmentCommander.Param.itemNo, itemNo);
            Context.Items.Add(ShipmentCommander.Param.contractNo, contractNo);
            Context.Items.Add(ShipmentCommander.Param.vendorId, GeneralCriteria.ALL);
            forwardToScreen(null);
            ArrayList shipmentList = (ArrayList)Context.Items[ShipmentCommander.Param.shipmentList];

            if (shipmentList.Count > 0 && vendorId != GeneralCriteria.ALL)
            {
                foreach (ShipmentProductDef shipmentProdDef in shipmentList)
                {
                    if (vendorId == shipmentProdDef.VendorId)
                    {
                        shipment = shipmentProdDef;
                        break;
                    }
                }
            }

            if (shipmentList.Count > 0 && vendorId == GeneralCriteria.ALL)
                shipment = (ShipmentProductDef)shipmentList[0];

            ArrayList vendorList = new ArrayList();
            if (shipment == null)
            {
                ddl_Office.SelectedValue = GeneralCriteria.ALL.ToString();
                ddl_PaymentOffice.selectByValue("-1");
                ddl_HandlingOffice.SelectedValue = GeneralCriteria.ALL.ToString();
                ddl_TermOfPurchase.SelectedValue = GeneralCriteria.ALL.ToString();
                uclProductTeam.clear();
            }
            else
            {
                if (shipment.OfficeId == OfficeId.DG.Id && shipment.HandlingOfficeId == OfficeId.HK.Id)
                {
                    shipment.OfficeId = OfficeId.HK.Id;
                    shipment.HandlingOfficeId = OfficeId.DG.Id;
                }
                if (shipment.OfficeId == OfficeId.DG.Id && shipment.HandlingOfficeId == OfficeId.SH.Id)
                {
                    shipment.OfficeId = OfficeId.SH.Id;
                    shipment.HandlingOfficeId = OfficeId.DG.Id;
                }
                if (shipment.OfficeId == OfficeId.DG.Id && shipment.HandlingOfficeId == OfficeId.VN.Id)
                {
                    shipment.OfficeId = OfficeId.VN.Id;
                    shipment.HandlingOfficeId = OfficeId.VN.Id;
                }
                ddl_Office.SelectedValue = shipment.OfficeId.ToString();
                ddl_PaymentOffice.selectByValue("-1");
                if (shipment.HandlingOfficeId == 41)
                    shipment.HandlingOfficeId = shipment.OfficeId;
                ddl_HandlingOffice.SelectedValue = shipment.HandlingOfficeId.ToString();
                ddl_TermOfPurchase.SelectedValue = shipment.TermOfPurchaseId.ToString();
                uclProductTeam.ProductCodeId = shipment.ProductTeamId;
            }
            ArrayList idList = new ArrayList();
            foreach (ShipmentProductDef def in shipmentList)
            {
                if (def.VendorId != 1572 && def.VendorId != 3164)
                    if (!idList.Contains(def.VendorId))
                    {
                        idList.Add(def.VendorId);
                        vendorList.Add(new ListItem(def.VendorName, def.VendorId.ToString()));
                    }
            }
            if (vendorList.Count == 0)
            {
                this.ddl_Supplier.bindList(vendorList, "Text", "Value", GeneralCriteria.ALL.ToString(), string.Empty, GeneralCriteria.ALL.ToString());
                this.ddl_Supplier.selectByValue(GeneralCriteria.ALL.ToString());
                this.ddl_Supplier.Enabled = false;
            }
            else
            {
                this.ddl_Supplier.bindList(vendorList, "Text", "Value");
                this.ddl_Supplier.selectByValue(vendorId < 0 ? ((ListItem)vendorList[0]).Value : vendorId.ToString());
            }

            if ((vendorId != -1 && vendorList.Count == 0) || (vendorId != -1 && vendorId != int.Parse(this.ddl_Supplier.SelectedValue)))
            {
                VendorRef vendor = IndustryUtil.getVendorByKey(vendorId);

                this.ddl_Supplier.Items.Add(new ListItem(vendor.Name, vendor.VendorId.ToString()));
                this.ddl_Supplier.selectByValue(vendor.VendorId.ToString());
            }

        }

        private void bindBIAMapping(UKClaimDef def)
        {
            if (def.Type.Id == UKClaimType.BILL_IN_ADVANCE.Id && def.WorkflowStatus.Id == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id)
            {
                List<UKClaimDef> list = UKClaimManager.Instance.getUKClaimListByBIAId(def.ClaimId);
                this.vwBIAMappingList = list;
                this.gv_BIAMapping.DataSource = list;
                this.gv_BIAMapping.DataBind();
            }
        }

        private void bindBIADiscrepancy(UKClaimDef def)
        {
            if (def.Type.Id == UKClaimType.BILL_IN_ADVANCE.Id && def.WorkflowStatus.Id == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id)
            {
                UKClaimBIADiscrepancyDef discrepancyDef = UKClaimManager.Instance.getUKClaimBIADiscrepancyByKey(def.ClaimId);

                this.vwUKClaimBIADiscrepancyDef = discrepancyDef;
                if (discrepancyDef != null)
                {
                    if (discrepancyDef.Amount > 0)
                    {
                        this.ddl_Action.Items.Add(new ListItem(BIAActionType.NS_COST.Name, BIAActionType.NS_COST.Id.ToString()));
                        this.ddl_Action.Items.Add(new ListItem(BIAActionType.SUPPLIER_RECHARGE.Name, BIAActionType.SUPPLIER_RECHARGE.Id.ToString()));
                    }
                    else
                    {
                        this.ddl_Action.Items.Add(new ListItem(BIAActionType.NS_PROVISION.Name, BIAActionType.NS_PROVISION.Id.ToString()));
                        this.ddl_Action.Items.Add(new ListItem(BIAActionType.SUPPLIER_REFUND.Name, BIAActionType.SUPPLIER_REFUND.Id.ToString()));
                    }

                    this.ddl_Action.selectByValue(discrepancyDef.ActionType.Id.ToString());
                    this.txt_BIADiscrepancyAmt.Text = discrepancyDef.Amount.ToString();
                    this.txt_BIADiscrepancyRemark.Text = discrepancyDef.Remark;
                    this.chkBIADiscrepancyComplete.Checked = discrepancyDef.IsLocked;

                    this.ddl_Action.Enabled = !discrepancyDef.IsLocked;
                    this.txt_BIADiscrepancyRemark.Enabled = !discrepancyDef.IsLocked;
                    this.chkBIADiscrepancyComplete.Enabled = !discrepancyDef.IsLocked;

                    UKClaimDCNoteDetailDef ukClaimDCNoteDetailDef = UKClaimManager.Instance.getUKClaimDCNoteDetailByLogicalKey(def.ClaimId, -1);
                    if (ukClaimDCNoteDetailDef != null)
                    {
                        UKClaimDCNoteDef ukClaimDCNoteDef = UKClaimManager.Instance.getUKClaimDCNoteByKey(ukClaimDCNoteDetailDef.DCNoteId);
                        txt_BIADiscrepancyDNNo.Text = ukClaimDCNoteDef.DCNoteNo;
                        txt_BIADiscrepancyDNDate.Text = DateTimeUtility.getDateString(ukClaimDCNoteDef.DCNoteDate);
                    }
                }
            }
        }

        private void bindClaimMappingList(int claimRequestId)
        {
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

            List<QAIS.ClaimRequestDef> list = new List<QAIS.ClaimRequestDef>();
            QAIS.ClaimRequestDef selectedRequest = null;
            if (claimRequestId > 0)
            {   
                // insert the selected claim request into the active claim request list
                selectedRequest = svc.GetClaimRequestByKey(claimRequestId);
                if (Action == "REVIEW")
                {
                    object[] activeList = svc.GetActiveClaimRequestListByType(-1, this.ddl_ClaimType.selectedValueToInt, this.ddl_Supplier.selectedValueToInt, this.txt_ItemNo.Text.Trim(), (this.ddl_ClaimType.selectedValueToInt == UKClaimType.MFRN.Id ? this.txtMonth.Text.Trim() : this.txt_UKDNNo.Text.Trim()));
                    foreach (object o in activeList)
                        if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                            list.Add((QAIS.ClaimRequestDef)o);
                }
                if (list.Count == 0)
                    list.Add(selectedRequest);
                else
                    for (int i = 0; i < list.Count; i++)
                        if (selectedRequest.RequestId <= list[i].RequestId)
                        {
                            if (selectedRequest.RequestId < list[i].RequestId)
                                list.Insert(i, selectedRequest);
                            break;
                        }
                        else
                            if (i == list.Count - 1)
                            {
                                list.Add(selectedRequest);
                                break;
                            }
            }
            
            this.vwMapResult = list;
            this.gv_Request.DataSource = list;
            this.gv_Request.DataBind();
        }

        private void bindClaimRefundList(List<UKClaimRefundDef> refundList)
        {
            this.gv_Refund.DataSource = refundList;
            this.gv_Refund.DataBind();
        }

        private List<UKClaimRefundDef> loadClaimRefundList(int claimId)
        {
            UKClaimRefundDef refund;
            List<UKClaimRefundDef> activeList = new List<UKClaimRefundDef>();
            List<UKClaimRefundDef> list = UKClaimManager.Instance.getUKClaimRefundListByClaimId(claimId);
            for (int i = 0; i < list.Count; i++)
                if ((refund=list[i]).Status != 0)
                {
                    UKClaimDCNoteDetailDef dcnDetail = UKClaimManager.Instance.getUKClaimDCNoteDetailByLogicalKey(claimId, refund.ClaimRefundId);
                    if (dcnDetail != null)
                    {
                        UKClaimDCNoteDef dcNote = UKClaimManager.Instance.getUKClaimDCNoteByKey(dcnDetail.DCNoteId);
                        refund.CreditNoteNo = dcNote.DCNoteNo;
                        refund.CreditNoteDate = dcNote.DCNoteDate;
                        refund.CreditNoteAmount = dcnDetail.Amount; //dcNote.SettledAmount ?
                        refund.SettlementDate = dcNote.SettlementDate;
                    }
                    else
                    {
                        refund.CreditNoteNo = string.Empty;
                        refund.CreditNoteDate = DateTime.MinValue;
                        refund.CreditNoteAmount = 0;
                        refund.SettlementDate = DateTime.MinValue;
                    }
                    activeList.Add(refund);
                }
            return activeList;
        }

        protected void gv_Refund_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UKClaimRefundDef def = vwRefundList[e.Row.RowIndex];

                ImageButton imgBtn = (ImageButton)e.Row.FindControl("imgEditRefund");
                imgBtn.CommandArgument = e.Row.RowIndex.ToString();
                imgBtn.Attributes.Add("onclick", getRefundButtonEvent(def));
                imgBtn.Visible = (string.IsNullOrEmpty(def.CreditNoteNo));

                imgBtn = (ImageButton)e.Row.FindControl("imgDeleteRefund");
                imgBtn.CommandArgument = e.Row.RowIndex.ToString();
                imgBtn.Attributes.Add("onclick", "return confirm('Are you sure to delete this Next claim refund record?');");
                imgBtn.Visible = (string.IsNullOrEmpty(def.CreditNoteNo));

                ((Label)e.Row.FindControl("lbl_ReceivedDate")).Text = DateTimeUtility.getDateString(def.ReceivedDate);
                ((Label)e.Row.FindControl("lbl_Amount")).Text = def.Amount.ToString("N02");
                ((Label)e.Row.FindControl("lbl_Remark")).Text = def.Remark;
                ((Label)e.Row.FindControl("lbl_CreditNoteNo")).Text = (string.IsNullOrEmpty(def.CreditNoteNo) ? string.Empty : def.CreditNoteNo.ToString());
                ((Label)e.Row.FindControl("lbl_CreditNoteDate")).Text = (string.IsNullOrEmpty(def.CreditNoteNo) ? string.Empty : def.CreditNoteDate == DateTime.MinValue ? string.Empty : def.CreditNoteDate.ToString("dd/MM/yyyy"));
                ((Label)e.Row.FindControl("lbl_CreditNoteAmount")).Text = (string.IsNullOrEmpty(def.CreditNoteNo) ? string.Empty : def.CreditNoteAmount.ToString("N02"));
                ((Label)e.Row.FindControl("lbl_IsReadyForSettlement")).Text = def.IsReadyForSettlement ? "Yes" : "No";
                ((Label)e.Row.FindControl("lbl_SettlementOption")).Text = def.SettlementOption.Name;
                ((Label)e.Row.FindControl("lbl_RefundSettlementDate")).Text = (string.IsNullOrEmpty(def.CreditNoteNo) ? string.Empty : def.SettlementDate == DateTime.MinValue ? "N/A" : def.SettlementDate.ToString("dd/MM/yyyy"));
                /*
                ((Label)e.Row.FindControl("lbl_Interfaced")).Text = (string.IsNullOrEmpty(def.CreditNoteNo) ? string.Empty : (def.IsInterfaced ? "Yes" : "No"));
                ((Label)e.Row.FindControl("lbl_RechargeInterfaced")).Text = (string.IsNullOrEmpty(def.CreditNoteNo) ? string.Empty : (def.IsRechargeInterfaced ? "Yes" : "No"));
                */
            }
        }

        protected void gv_Refund_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUKClaimRefund")
            {
                UKClaimRefundDef def = vwRefundList[int.Parse((string)e.CommandArgument)];
                if (!string.IsNullOrEmpty(this.hid_RefundResult.Value))
                {
                    mapRefundToDef(this.hid_RefundResult.Value, def);
                    bindClaimRefundList(vwRefundList);
                    addClaimRefundLog("Update Claim Refund Record (ID:" + def.ClaimRefundId.ToString() + ")");
                }
            }
            if (e.CommandName == "DeleteUKClaimRefund")
            {
                int rowId = int.Parse((string)e.CommandArgument);
                UKClaimRefundDef def = (UKClaimRefundDef)this.vwRefundList[rowId];
                /*
                ((Label)e.Row.FindControl("lbl_ReceivedDate")).Text = def.ReceivedDate.ToString("dd/MM/yyyy");
                */
                deleteUKClaimRefundDef(def);
                this.vwRefundList.Remove(def);
                bindClaimRefundList(vwRefundList);
                addClaimRefundLog("Delete Claim Refund Record (ID:" + def.ClaimRefundId.ToString() + ")");
            }
        }

        protected void gv_Request_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                QAIS.ClaimRequestDef def = vwMapResult[e.Row.RowIndex];
                ((RadioButton)e.Row.FindControl("radClaimRequest")).Checked = (def.RequestId == this.vwUKClaimDef.ClaimRequestId);
                ((HiddenField)e.Row.FindControl("hid_ClaimRequestId")).Value = def.RequestId.ToString(); 
                ((Label)e.Row.FindControl("lbl_ClaimType")).Text = def.ClaimType.ToString();
                if (def.ContractNo != string.Empty)
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo + " / " + def.ContractNo;
                else
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo;
                ((Label)e.Row.FindControl("lbl_FormNo")).Text = def.FormNo;
                if (def.IssueDate != DateTime.MinValue)
                    ((Label)e.Row.FindControl("lbl_FormIssueDate")).Text = DateTimeUtility.getDateString(def.IssueDate);
                ((Label)e.Row.FindControl("lbl_Month")).Text = def.ClaimMonth;
                ((Label)e.Row.FindControl("lbl_NSRechargePercent")).Text = def.NSRechargePercent.ToString() + "%";
                ((Label)e.Row.FindControl("lbl_SupplierRechargePercent")).Text = def.VendorRechargePercent.ToString() + "%";
                ((Label)e.Row.FindControl("lbl_Status")).Text = def.WorkflowStatus.Name;
                ((LinkButton)e.Row.FindControl("lnk_Attachment")).CommandArgument = e.Row.RowIndex.ToString();
                ((LinkButton)e.Row.FindControl("lnk_Attachment")).Attributes.Add("onclick", "openAttachments(this, '" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.RequestId.ToString())) + "');return false;");

                if (def.NSRechargePercent > 0)
                    e.Row.BackColor = System.Drawing.Color.Yellow;
            }
        }
        
        #endregion
        

        #region Commander routines

        private UKClaimDef createNewUKClaim()
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.CreateUKClaim);
            forwardToScreen(null);
            return (UKClaimDef)Context.Items[AccountCommander.Param.claim];
        }

        private UKClaimDef getUKClaimByGUId(string guId)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKClaimByGuid);
            Context.Items.Add(AccountCommander.Param.guid, guId);
            forwardToScreen(null);
            return (UKClaimDef)Context.Items[AccountCommander.Param.claim];
        }

        private UKClaimDef getUKClaimByClaimId(int claimId)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKClaimByKey);
            Context.Items.Add(AccountCommander.Param.claimId, claimId);
            forwardToScreen(null);
            return (UKClaimDef)Context.Items[AccountCommander.Param.claim];
        }

        private UKClaimDef updateUKClaim(UKClaimDef ukClaim, UKClaimBIADiscrepancyDef discrepancyDef)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateUKClaim);
            Context.Items.Add(AccountCommander.Param.claim, ukClaim);
            Context.Items.Add(AccountCommander.Param.claimBIADiscrepancy, discrepancyDef);
            forwardToScreen(null);
            return (UKClaimDef)Context.Items[AccountCommander.Param.claim];
        }

        private List<UKClaimDef> getUKClaimListByDebitNoteNo(string DNNo)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKClaimList);
            Context.Items.Add(AccountCommander.Param.ukDebitNoteNo, DNNo);
            forwardToScreen(null);
            return (List<UKClaimDef>)Context.Items[AccountCommander.Param.ukClaimList];
        }

        private void deleteUKClaimRefundDef(UKClaimRefundDef def)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.DeleteUKClaimRefundDef);
            Context.Items.Add(AccountCommander.Param.claimRefund, def);
            forwardToScreen(null);
        }

        #endregion


        #region Misc. Tools

        private void mapRefundToDef(string refund, UKClaimRefundDef def)
        {
            if (!string.IsNullOrEmpty(refund))
            {
                char delimiter = refund[0];
                string[] detail = refund.Split(delimiter);
                string rmk = string.Empty;

                def.ClaimId = vwUKClaimDef.ClaimId;
                def.ClaimRefundId = int.Parse(detail[1]);
                def.ReceivedDate = DateTime.Parse(detail[2]);
                def.Amount = decimal.Parse(detail[3]);
                def.IsReadyForSettlement = (detail[4] == "1" ? true : false);
                def.SettlementOption = UKClaimSettlemtType.getType(int.Parse(detail[5]));
                for (int i = 6; i < detail.Length; i++)
                    rmk += (rmk == string.Empty ? string.Empty : delimiter.ToString()) + detail[i];
                def.Remark = rmk.Trim();
            }
        }

        private void addClaimRefundLog(string description)
        {
            // Add Claim Refund action Log to Next Claim
            UKClaimLogDef log = new UKClaimLogDef(vwUKClaimDef.ClaimId, description, this.LogonUserId, vwUKClaimDef.WorkflowStatus.Id, vwUKClaimDef.WorkflowStatus.Id);
            UKClaimManager.Instance.updateUKClaimLogDef(log, this.LogonUserId);
        }

        #endregion


        #region DMS utilities

        private ArrayList getUKDebitNoteQuery()
        {
            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", this.ddl_ClaimType.selectedText.Trim()));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Debit Note No", this.txt_UKDNNo.Text.Trim()));
            queryStructs.Add(new QueryStructDef("Item No", this.txt_ItemNo.Text.Trim()));
            if (int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.REJECT.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.REWORK.Id)
                queryStructs.Add(new QueryStructDef("Qty", this.txt_Qty.Text));
            return queryStructs;
        }
 
        private string uploadUKDebitNote()
        {
            string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);

            ArrayList queryStructs = getUKDebitNoteQuery();

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            long docId = 0;
            foreach (DocumentInfoDef docInfoDef in qList)
                docId = docInfoDef.DocumentID;

            string filename = outputFolder + Path.GetFileNameWithoutExtension(this.upd_UKDebitNote.FileName) + Path.GetExtension(this.upd_UKDebitNote.FileName).ToLower();
            this.upd_UKDebitNote.SaveAs(filename);
            File.Copy(filename, WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + "backup\\" + tsFolderName + "_" + Path.GetFileNameWithoutExtension(this.upd_UKDebitNote.FileName) + Path.GetExtension(this.upd_UKDebitNote.FileName).ToLower(), true);

            ArrayList attachmentList = new ArrayList();
            attachmentList.Add(filename);
            string uploadMsg;
            if (docId > 0)
                uploadMsg = DMSUtil.UpdateDocumentWithoutExistingAttachments(docId, queryStructs, attachmentList);
            else
                uploadMsg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", this.txt_UKDNNo.Text.Trim(), "UK Claim", queryStructs, attachmentList);
            FileUtility.clearFolder(outputFolder, false);

            return (uploadMsg == string.Empty ? this.upd_UKDebitNote.FileName : string.Empty);
        }

        private void updateDMSIndexFields(UKClaimDef ukClaim)
        {
            long ukDNDocId = 0;
            long authFormId = 0;
            ArrayList updatedQueryStructs = new ArrayList();
            updatedQueryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            updatedQueryStructs.Add(new QueryStructDef("Item No", ukClaim.ItemNo));
            updatedQueryStructs.Add(new QueryStructDef("Debit Note No", ukClaim.UKDebitNoteNo));

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaim.Type.DMSDescription));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Item No", ukClaim.ItemNo));
            queryStructs.Add(new QueryStructDef("Debit Note No", ukClaim.UKDebitNoteNo));
            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            foreach (DocumentInfoDef docInfoDef in qList)
                ukDNDocId = docInfoDef.DocumentID;

            queryStructs.Clear();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
            QAIS.ClaimRequestDef req = svc.GetClaimRequestByKey(ukClaim.ClaimRequestId);

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Authorization Form"));
            queryStructs.Add(new QueryStructDef("Item No", req.ItemNo));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaim.Type.DMSDescription));
            if (req.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                queryStructs.Add(new QueryStructDef("MFRN Month", req.ClaimMonth));
            else
                queryStructs.Add(new QueryStructDef("Form No", req.FormNo));
            qList = DMSUtil.queryDocument(queryStructs);

            foreach (DocumentInfoDef docInfoDef in qList)
            {
                authFormId = docInfoDef.DocumentID;
                foreach (FieldInfoDef fiDef in docInfoDef.FieldInfos)
                {
                    if (fiDef.FieldName != "Supporting Doc Type" && fiDef.FieldName != "Item No" && fiDef.FieldName != "Debit Note No" && fiDef.FieldName != "Qty")
                        updatedQueryStructs.Add(new QueryStructDef(fiDef.FieldName, fiDef.Content));
                }
                if (int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.REJECT.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.REWORK.Id)
                    updatedQueryStructs.Add(new QueryStructDef("Qty", this.txt_Qty.Text));
                break;
            }

            if (authFormId != 0 && ukDNDocId != 0)
            {
                string strReturn = DMSUtil.UpdateDocument(ukDNDocId, updatedQueryStructs);
            }
        }
        
        #endregion

        #region Object Events

        protected void valCustom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            /*
            int submitMode = int.Parse(ViewState["SubmitMode"].ToString());
            string action = ViewState["Action"].ToString();
            */
            bool radChecked = false;
            foreach (GridViewRow r in gv_Request.Rows)
                if (((RadioButton)r.FindControl("radClaimRequest")).Checked) radChecked = true;

            if (Action == "REVIEW")
            {
                if (validationType == "SAVE")
                {   
                    // SAVE : mapped to one claim request
                    if (this.gv_Request.Rows.Count == 0)
                        Page.Validators.Add(new ValidationError("Please click 'Map Claim Request' button to map one claim request to this Next claim record"));
                    else if (radChecked == false)
                        Page.Validators.Add(new ValidationError("The Next Claim must map to one claim request"));
                    if (this.ddl_Supplier.selectedValueToInt == GeneralCriteria.ALL)
                        Page.Validators.Add(new ValidationError("Please select a supplier"));
                }
                if (validationType == "MAP")
                {   
                    // MAP : minimum requirement to get the claim request list for mapping
                    if (this.ddl_ClaimType.selectedValueToInt == GeneralCriteria.ALL)
                        Page.Validators.Add(new ValidationError("Please select a Claim Type to get the Claim Request"));
                    if (this.ddl_Supplier.selectedValueToInt == GeneralCriteria.ALL)
                        Page.Validators.Add(new ValidationError("Please select a supplier to get the claim request"));
                }
            }
            else
            {
                if (int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.AUDIT_FEE.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.OTHERS.Id)
                {
                    if (this.txt_Supplier.VendorId <= 0)
                        Page.Validators.Add(new ValidationError("Please input the supplier"));

                    if (int.Parse(this.ddl_Office.SelectedValue) == -1)
                        Page.Validators.Add(new ValidationError("Invalid Office"));
                    else if (int.Parse(this.ddl_Office.SelectedValue) != OfficeId.DG.Id && int.Parse(this.ddl_Office.SelectedValue) != OfficeId.HK.Id && int.Parse(this.ddl_Office.SelectedValue) != OfficeId.SH.Id && int.Parse(this.ddl_Office.SelectedValue) != OfficeId.VN.Id)
                        this.ddl_HandlingOffice.selectByValue(this.ddl_Office.SelectedValue);
                    else if (int.Parse(this.ddl_Office.SelectedValue) == OfficeId.DG.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.HK.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.VN.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.SH.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.DG.Id)
                        Page.Validators.Add(new ValidationError("Invalid Handling Office"));
                    else if (int.Parse(this.ddl_Office.SelectedValue) == OfficeId.HK.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.HK.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.DG.Id)
                        Page.Validators.Add(new ValidationError("Invalid Handling Office"));
                    else if (int.Parse(this.ddl_Office.SelectedValue) == OfficeId.SH.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.SH.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.DG.Id)
                        Page.Validators.Add(new ValidationError("Invalid Handling Office"));
                    else if (int.Parse(this.ddl_Office.SelectedValue) == OfficeId.VN.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.VN.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.DG.Id)
                        Page.Validators.Add(new ValidationError("Invalid Handling Office"));
                }
                else
                {
                    if (this.ddl_Supplier.Items.Count == 0 || this.ddl_Supplier.selectedValueToInt == GeneralCriteria.ALL)
                        Page.Validators.Add(new ValidationError("Please select a supplier"));
                    else if (int.Parse(this.ddl_Office.SelectedValue) != OfficeId.DG.Id && int.Parse(this.ddl_Office.SelectedValue) != OfficeId.HK.Id && int.Parse(this.ddl_Office.SelectedValue) != OfficeId.SH.Id && int.Parse(this.ddl_Office.SelectedValue) != OfficeId.VN.Id)
                        this.ddl_HandlingOffice.selectByValue(this.ddl_Office.SelectedValue);
                    else if (int.Parse(this.ddl_Office.SelectedValue) == OfficeId.DG.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.HK.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.VN.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.SH.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.DG.Id)
                        Page.Validators.Add(new ValidationError("Invalid Handling Office"));
                    else if (int.Parse(this.ddl_Office.SelectedValue) == OfficeId.HK.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.HK.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.DG.Id)
                        Page.Validators.Add(new ValidationError("Invalid Handling Office"));
                    else if (int.Parse(this.ddl_Office.SelectedValue) == OfficeId.SH.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.SH.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.DG.Id)
                        Page.Validators.Add(new ValidationError("Invalid Handling Office"));
                    else if (int.Parse(this.ddl_Office.SelectedValue) == OfficeId.VN.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.VN.Id && int.Parse(this.ddl_HandlingOffice.SelectedValue) != OfficeId.DG.Id)
                        Page.Validators.Add(new ValidationError("Invalid Handling Office"));
                }

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
                {
                    if (this.txt_SZSupplier.VendorId <= 0)
                        Page.Validators.Add(new ValidationError("Please input the shenzhen supplier"));
                }
            }

            if (int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.AUDIT_FEE.Id && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.OTHERS.Id)
            {
                if (string.IsNullOrEmpty(this.txt_ItemNo.Text))
                    Page.Validators.Add(new ValidationError("Please input the Item No."));
                else
                {
                    if (string.IsNullOrEmpty(this.uclProductTeam.KeyTextBox.Text) || this.ddl_Office.selectedValueToInt == GeneralCriteria.ALL)
                        Page.Validators.Add(new ValidationError("Undefined Office / Product Team ( Please make sure the supplier and item number are correct)"));
                }
            }

            DateTime d;
            if (int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.MFRN.Id)
            {
                if ((!DateTime.TryParseExact(txtMonth.Text + "01", "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d) || this.txtMonth.Text.Trim() == string.Empty))
                    Page.Validators.Add(new ValidationError("Please specify the month for MFRN"));
            }

            int qty;
            if (string.IsNullOrEmpty(this.txt_Qty.Text))
                Page.Validators.Add(new ValidationError("Please input the quantity"));
            else
                if (!int.TryParse(this.txt_Qty.Text, out qty))
                    Page.Validators.Add(new ValidationError("Please input a valid quantity"));
                else
                    if (qty <= 0)
                        Page.Validators.Add(new ValidationError("Please input a valid quantity"));

            decimal amt;
            if (string.IsNullOrEmpty(this.txt_Amt.Text))
                Page.Validators.Add(new ValidationError("Please input the amount"));
            else
                if (!decimal.TryParse(this.txt_Amt.Text, out amt))
                    Page.Validators.Add(new ValidationError("Please input a valid amount"));
                else
                    if (amt <= 0)
                        Page.Validators.Add(new ValidationError("Please input a valid amount"));

            if (this.chk_HasUKDN.Checked && upd_UKDebitNote.Enabled)
            {
                if (string.IsNullOrEmpty(this.txt_UKDNNo.Text))
                    Page.Validators.Add(new ValidationError("Please input the Next Debit Note No."));
                if (string.IsNullOrEmpty(txt_UKDNDate.Text))
                    Page.Validators.Add(new ValidationError("Please input the Next Debit Note Date"));
                if (string.IsNullOrEmpty(txt_UKDNReceivedDate.Text))
                    Page.Validators.Add(new ValidationError("Please input the Next Debit Note Received Date"));
                else
                {
                    AccountFinancialCalenderDef calDef = CommonUtil.getAccountPeriodByDate(AppId.ISAM.Code, DateTime.Today);
                    if (this.vwUKClaimDef.ClaimId <= 0 && DateTime.Parse(txt_UKDNReceivedDate.Text) < calDef.StartDate)
                    {
                        Page.Validators.Add(new ValidationError("Next Debit Note Received Date must be equal to or later than " + DateTimeUtility.getDateString(calDef.StartDate)));
                    }
                }
                if (!upd_UKDebitNote.HasFile)
                    Page.Validators.Add(new ValidationError("Please select the Next Debit Note document to be uploaded"));
                else
                {
                    FileInfo fi = new FileInfo(this.upd_UKDebitNote.FileName);
                    if (fi.Name.ToLower() != this.txt_UKDNNo.Text.Replace("/", string.Empty).ToLower() + ".pdf")
                        Page.Validators.Add(new ValidationError("The filename of the uploaded file must be " + this.txt_UKDNNo.Text + ".pdf"));
                }
            }

            UKClaimDef claim = vwUKClaimDef;
            /*
            if (this.pnl_BIADiscrepancy.Visible)
            {
                if (string.IsNullOrEmpty(this.txt_BIADiscrepancyAmt.Text))
                    Page.Validators.Add(new ValidationError("Please input the BIA discrepancy amount"));
                else
                {
                    if (!decimal.TryParse(this.txt_BIADiscrepancyAmt.Text, out amt))
                        Page.Validators.Add(new ValidationError("Please input a valid BIA discrepancy amount"));
                    else
                    {
                        if (amt == 0)
                            Page.Validators.Add(new ValidationError("Please input a valid BIA discrepancy amount"));
                        else if (amt > 0 && int.Parse(this.ddl_Action.SelectedValue) == BIAActionType.NS_PROVISION.Id)
                            Page.Validators.Add(new ValidationError("Please input a valid BIA discrepancy amount, must be a negative value for NS Provision"));
                        else if (amt < 0 && int.Parse(this.ddl_Action.SelectedValue) != BIAActionType.NS_PROVISION.Id)
                            Page.Validators.Add(new ValidationError("Please input a valid BIA discrepancy amount, must be a positive value for either NS Cost / Supplier Recharge"));
                    }
                }
            }
            */

            if (claim.ClaimRequestId == -1 && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.AUDIT_FEE.Id && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.OTHERS.Id && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.GB_TEST.Id && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.BILL_IN_ADVANCE.Id && this.ddl_Supplier.Items.Count > 0)
            {
                QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

                List<QAIS.ClaimRequestDef> list = new List<QAIS.ClaimRequestDef>();
                object[] activeList = svc.GetActiveClaimRequestListByType(-1, this.ddl_ClaimType.selectedValueToInt, this.ddl_Supplier.selectedValueToInt, this.txt_ItemNo.Text.Trim(), (this.ddl_ClaimType.selectedValueToInt == UKClaimType.MFRN.Id ? this.txtMonth.Text.Trim() : this.txt_UKDNNo.Text.Trim()));
                /*
                object[] activeList = svc.GetClaimRequestListByTypeMapping(-1, this.ddl_ClaimType.selectedValueToInt, this.ddl_Supplier.selectedValueToInt, this.txt_ItemNo.Text.Trim(), (this.ddl_ClaimType.selectedValueToInt == UKClaimType.MFRN.Id ? this.txtMonth.Text.Trim() : this.txt_UKDNNo.Text.Trim()));
                if (activeList.Length != 1)
                    Page.Validators.Add(new ValidationError("There is no exactly matched form data found in QAIS"));
                else
                {
                    foreach (object o in activeList)
                        if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                            claim.ClaimRequestId = ((QAIS.ClaimRequestDef)o).RequestId;
                }
                */
                if (activeList.Length == 1)
                {
                    foreach (object o in activeList)
                        if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                        {
                            // remove sample image requirement @2012-07-23
                            //if (((svc.IsSampleImageAvailable(((QAIS.ClaimRequestDef)o).RequestId) && ((QAIS.ClaimRequestDef)o).IsAuthorized) || (this.ddl_ClaimType.selectedValueToInt == UKClaimType.MFRN.Id && ((QAIS.ClaimRequestDef)o).IsAuthorized)) && UKClaimManager.Instance.getUKClaimByClaimRequestId(((QAIS.ClaimRequestDef)o).RequestId) == null)

                            if (((((QAIS.ClaimRequestDef)o).IsAuthorized) || (this.ddl_ClaimType.selectedValueToInt == UKClaimType.MFRN.Id && ((QAIS.ClaimRequestDef)o).IsAuthorized)) && UKClaimManager.Instance.getUKClaimByClaimRequestId(((QAIS.ClaimRequestDef)o).RequestId) == null)
                                claim.ClaimRequestId = ((QAIS.ClaimRequestDef)o).RequestId;
                        }
                }
            }

            int vendorId = -1;
            if (int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.AUDIT_FEE.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.OTHERS.Id)
                vendorId = this.txt_Supplier.VendorId;
            else if (this.ddl_Supplier.Items.Count > 0)
                vendorId = this.ddl_Supplier.selectedValueToInt;

            if (vendorId != -1)
            {
                //List<UKClaimDef> ukClaimList = UKClaimManager.Instance.getUKClaimListByTypeMapping(-1, int.Parse(this.ddl_ClaimType.SelectedValue), vendorId, this.txt_ItemNo.Text.Trim(), int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.MFRN.Id ? this.txtMonth.Text : this.txt_UKDNNo.Text.Trim(), int.Parse(this.txt_Qty.Text));
                List<UKClaimDef> ukClaimList = UKClaimManager.Instance.getUKClaimListByTypeMapping(-1, int.Parse(this.ddl_ClaimType.SelectedValue), vendorId, this.txt_ItemNo.Text.Trim(), int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.MFRN.Id ? this.txt_UKDNNo.Text.Trim() : this.txt_UKDNNo.Text.Trim(), int.Parse(this.txt_Qty.Text));
                if (ukClaimList.Count > 0 && claim.ClaimId == -1)
                    Page.Validators.Add(new ValidationError("Duplicate record is found, please locate this record from the Next Claim Portal"));
                else
                {
                    foreach (UKClaimDef cDef in ukClaimList)
                    {
                        if (cDef.ClaimId != claim.ClaimId && claim.ClaimRequestId == -1)
                        {
                            Page.Validators.Add(new ValidationError("Duplicate record is found, please locate this record from the Next Claim Portal"));
                            break;
                        }
                    }
                }
            }

            if (this.pnl_BIADiscrepancy.Visible)
            {
                if ((decimal.Parse(this.txt_BIADiscrepancyAmt.Text) < 0 && int.Parse(this.ddl_Action.SelectedValue) != BIAActionType.NS_PROVISION.Id && int.Parse(this.ddl_Action.SelectedValue) != BIAActionType.SUPPLIER_REFUND.Id) ||
                    (decimal.Parse(this.txt_BIADiscrepancyAmt.Text) > 0 && int.Parse(this.ddl_Action.SelectedValue) == BIAActionType.NS_PROVISION.Id && int.Parse(this.ddl_Action.SelectedValue) == BIAActionType.SUPPLIER_REFUND.Id))
                {
                    Page.Validators.Add(new ValidationError("Invalid BIA Action type"));
                }
            }

            if (vendorId == 5574 && claim.ClaimId == -1)
                Page.Validators.Add(new ValidationError("You are not allowed to create new claim record under this vendor- [NINGBO MEIJIA s/b SPRING SOUND]"));
        }

        protected void btn_Save_Click(object sender, EventArgs e)
        {
            validationType = "SAVE";
            Page.Validate();
            if (Page.IsValid)
            { 
                UKClaimDef claim = vwUKClaimDef;
                int requestId = claim.ClaimRequestId;

                QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

                /*
                foreach (GridViewRow r in gv_Request.Rows)
                {
                    if (((RadioButton)r.FindControl("radClaimRequest")).Checked)
                    {
                        requestId = int.Parse(((HiddenField)r.FindControl("hid_ClaimRequestId")).Value);
                    }
                }
                */

                int lastWFSId = claim.WorkflowStatus.Id;
                int lastRequestId = claim.ClaimRequestId;

                claim.HasUKDebitNote = this.chk_HasUKDN.Checked;
                if (chk_HasUKDN.Checked)
                {
                    claim.UKDebitNoteNo = this.txt_UKDNNo.Text;
                    claim.UKDebitNoteDate = (string.IsNullOrEmpty(this.txt_UKDNDate.Text) ? DateTime.MinValue : DateTime.Parse(this.txt_UKDNDate.Text));
                    claim.UKDebitNoteReceivedDate = (string.IsNullOrEmpty(this.txt_UKDNReceivedDate.Text) ? DateTime.MinValue : DateTime.Parse(this.txt_UKDNReceivedDate.Text));
                }
                else
                {
                    if (claim.Type.Id != UKClaimType.BILL_IN_ADVANCE.Id)
                        claim.UKDebitNoteNo = string.Empty;
                    claim.UKDebitNoteDate = DateTime.MinValue;
                    claim.UKDebitNoteReceivedDate = DateTime.MinValue;
                }

                claim.WorkflowStatus = (Action == "CREATE" ? ClaimWFS.NEW : (Action == "REVIEW" ? ClaimWFS.SUBMITTED : claim.WorkflowStatus));

                /*
                if (Action == "CREATE" && (requestId != -1 || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.AUDIT_FEE.Id))
                    claim.WorkflowStatus = ClaimWFS.SUBMITTED;
                */

                claim.Quantity = int.Parse(this.txt_Qty.Text);
                claim.Currency.CurrencyId = this.ddl_Currency.selectedValueToInt;
                claim.Amount = decimal.Parse(this.txt_Amt.Text);
                if (this.txt_Desc.Text.Trim() != string.Empty)
                {
                    if (claim.Remark == string.Empty)
                        claim.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + this.txt_Desc.Text.Trim();
                    else
                        claim.Remark += ("\n" + "[" + DateTime.Today.ToString("dd/MM") + "] " + this.txt_Desc.Text.Trim());
                }
                claim.Type = UKClaimType.getType(this.ddl_ClaimType.selectedValueToInt);
                claim.GUId = (string.IsNullOrEmpty(claim.GUId) ? Guid.NewGuid().ToString() : claim.GUId);
                claim.ClaimRequestId = (requestId == -1 ? claim.ClaimRequestId : requestId);

                if (int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.MFRN.Id)
                    claim.ClaimMonth = this.txtMonth.Text.Trim();
                else
                    claim.ClaimMonth = string.Empty;

                claim.IsReadyForSettlement = this.chkIsReadyForSettlement.Checked;
                claim.SettlementOption = UKClaimSettlemtType.getType(int.Parse(this.ddl_SettlementOption.SelectedValue));
                claim.ItemNo = this.txt_ItemNo.Text.Trim();
                claim.ContractNo = this.txt_ContractNo.Text.Trim();
                claim.PaymentOfficeId = this.ddl_PaymentOffice.selectedValueToInt;
                if (this.ddl_PaymentOffice.selectedValueToInt == -1)
                    claim.PaymentOfficeId = claim.OfficeId;

                if (int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.AUDIT_FEE.Id && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.OTHERS.Id)
                {
                    claim.Vendor = IndustryUtil.getVendorByKey(this.ddl_Supplier.selectedValueToInt);
                    claim.OfficeId = this.ddl_Office.selectedValueToInt;
                    claim.HandlingOfficeId = this.ddl_HandlingOffice.selectedValueToInt;
                    claim.ProductTeamId = (this.uclProductTeam.ProductCodeId == int.MinValue ? GeneralCriteria.ALL : this.uclProductTeam.ProductCodeId);
                    claim.TermOfPurchaseId = this.ddl_TermOfPurchase.selectedValueToInt;
                }
                else
                {
                    claim.Vendor = IndustryUtil.getVendorByKey(this.txt_Supplier.VendorId);
                    claim.OfficeId = this.ddl_Office.selectedValueToInt;
                    claim.HandlingOfficeId = int.Parse(this.ddl_HandlingOffice.SelectedValue);
                    claim.ProductTeamId = 0;
                    ArrayList shipmentProdList = ShipmentManager.Instance.GetShipmentProductByVendorId(this.txt_Supplier.VendorId);
                    if (shipmentProdList.Count > 0)
                    {
                        ShipmentProductDef shipmentProdDef = (ShipmentProductDef)shipmentProdList[0];
                        claim.TermOfPurchaseId = shipmentProdDef.TermOfPurchaseId;
                    }
                    else
                        claim.TermOfPurchaseId = 1;
                }

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
                    claim.SZVendor = IndustryUtil.getVendorByKey(this.txt_SZSupplier.VendorId);

                if (claim.Type.Id == UKClaimType.BILL_IN_ADVANCE.Id && claim.WorkflowStatus.Id == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id)
                {
                    if (this.vwUKClaimBIADiscrepancyDef != null)
                    {
                        UKClaimBIADiscrepancyDef discrepancyDef = this.vwUKClaimBIADiscrepancyDef;
                        discrepancyDef.Amount = decimal.Parse(this.txt_BIADiscrepancyAmt.Text);
                        discrepancyDef.ActionType = BIAActionType.getType(int.Parse(this.ddl_Action.SelectedValue));
                        discrepancyDef.Remark = this.txt_BIADiscrepancyRemark.Text.Trim();
                        discrepancyDef.IsLocked = this.chkBIADiscrepancyComplete.Checked;
                    }
                }
                
                bool isSubmitted = false;
                if (claim.WorkflowStatus.Id == ClaimWFS.NEW.Id && (requestId != -1 || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.AUDIT_FEE.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.OTHERS.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.GB_TEST.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.BILL_IN_ADVANCE.Id))
                {
                    QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(requestId);
                    if (int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.AUDIT_FEE.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.OTHERS.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.GB_TEST.Id || int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.BILL_IN_ADVANCE.Id)
                    {
                        if (int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.GB_TEST.Id)
                            claim.IsReadyForSettlement = false;
                        claim.WorkflowStatus = ClaimWFS.SUBMITTED;
                        isSubmitted = true;
                    }
                    else if (requestDef.WorkflowStatusId == 8)
                    {
                        claim.WorkflowStatus = ClaimWFS.USER_SIGNED_OFF;
                        isSubmitted = true;
                    }
                    /*
                    else if (requestDef.WorkflowStatusId == 10 || ((requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.IsAuthorized && svc.IsSampleImageAvailable(requestDef.RequestId)) || ((requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.IsAuthorized && int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.MFRN.Id))
                        // removed sample image requirement @2012-07-23
                    */
                    else if ((requestDef.WorkflowStatusId == 10 || 
                              ((requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.IsAuthorized) || 
                              ((requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.IsAuthorized && int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.MFRN.Id)) 
                        && requestDef.VendorRechargePercent == 100)
                    {
                        if (requestDef.ItemNo == claim.ItemNo && requestDef.Vendor.VendorId == claim.Vendor.VendorId)
                        {
                            if (requestDef.WorkflowStatusId == 10)
                                svc.sendCancelledClaimNotification(requestDef.RequestId, this.LogonUserId);

                            claim.WorkflowStatus = ClaimWFS.SUBMITTED;
                            isSubmitted = true;
                            if (requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7)
                            {
                                if (claim.Remark == string.Empty)
                                    claim.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier";
                                else
                                    claim.Remark += ("\n" + "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier");
                                claim.IsReadyForSettlement = true;
                            }
                            if (requestDef.WorkflowStatusId == 10)
                                claim.IsReadyForSettlement = false;
                        }
                    }
                }

                if (isSubmitted && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.AUDIT_FEE.Id
                                && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.OTHERS.Id
                                && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.GB_TEST.Id 
                                && int.Parse(this.ddl_ClaimType.SelectedValue) != UKClaimType.BILL_IN_ADVANCE.Id)
                {
                    QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(requestId);
                    if (requestId != -1 && requestDef.WorkflowStatusId != 10)
                        svc.SetClaimRequestStatus(claim.ClaimRequestId, 9, this.LogonUserId);
                }

                UKClaimDef updatedClaim = updateUKClaim(claim, this.vwUKClaimBIADiscrepancyDef);
                /*
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "PNotify.prototype.options.styling = 'jqueryui'; $(function(){new PNotify({title: 'Notice',text: 'Record has been updated successfully'});});", true);
                */

                /*
                if (Action == "CREATE" && requestId != -1)
                    svc.SetClaimRequestStatus(claim.ClaimRequestId, 9, this.LogonUserId);   
                */

                if (Action == "REVIEW" && lastRequestId != claim.ClaimRequestId)
                {
                    svc.SetClaimRequestStatus(claim.ClaimRequestId, 9, this.LogonUserId);
                    if (lastRequestId != -1)
                        svc.SetClaimRequestStatus(lastRequestId, 7, this.LogonUserId);   // reset the status of the original mapped claim request to 'Factory Signed off'
                }
                if (this.upd_UKDebitNote.Enabled && this.upd_UKDebitNote.HasFile)
                {
                    string fileUploaded = uploadUKDebitNote();
                }

                if (this.chk_HasUKDN.Checked && isSubmitted)
                    this.updateDMSIndexFields(updatedClaim);

                if (Action == "REVIEW")
                {
                    Context.Items.Clear();
                    forwardToScreen("UKClaim.ReviewList");
                }
                PrevAction = Action;
                initControls(updatedClaim, (Action == "CREATE" ? "EDIT" : Action));
            }

        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.initControls(vwUKClaimDef, Action);
        }

        protected void btn_New_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.CreateUKClaim);
            forwardToScreen("UKClaim.Edit");
        }

        protected void btn_AddRefund_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.hid_RefundResult.Value))
            {
                UKClaimRefundDef def = new UKClaimRefundDef();
                mapRefundToDef(this.hid_RefundResult.Value, def);
                vwRefundList.Add(def);
                bindClaimRefundList(vwRefundList);
                // Add Action Log
                addClaimRefundLog("Add New Claim Refund Record (ID:" + def.ClaimRefundId.ToString() + ")");
            }
        }

        protected void txt_ItemNo_OnTextChanged(object sender, EventArgs e)
        {
            validationType = "OTHERS";
            if (txt_ItemNo.Text != string.Empty)
                bindSupplierProduct(this.txt_ItemNo.Text.Trim(), this.txt_ContractNo.Text.Trim());
            if (ddl_Supplier.Enabled)
                this.ddl_Supplier.Focus();
            else
                this.txt_ItemNo.Focus();
        }

        #endregion

        protected void ddl_ClaimType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isAudit = (int.Parse(ddl_ClaimType.SelectedValue) == UKClaimType.AUDIT_FEE.Id || int.Parse(ddl_ClaimType.SelectedValue) == UKClaimType.OTHERS.Id);
            bool isMFRN = int.Parse(ddl_ClaimType.SelectedValue) == UKClaimType.MFRN.Id;

            this.txt_ContractNo.Enabled = !isAudit;
            this.txt_ItemNo.Enabled = !isAudit;
            this.ddl_Supplier.Visible = !isAudit;
            this.txt_Supplier.Visible = isAudit;
            /*
            this.ddl_Office.Enabled = isAudit;
            */
            this.ddl_Office.Enabled = true;
            if (sender != null)
            {
                this.ddl_HandlingOffice.selectByValue(GeneralCriteria.ALL.ToString());
                this.ddl_Office.selectByValue(GeneralCriteria.ALL.ToString());
                this.ddl_PaymentOffice.selectByValue("-1");
                this.ddl_TermOfPurchase.selectByValue(GeneralCriteria.ALL.ToString());
                this.uclProductTeam.clear();
                this.txt_Supplier.clear();
                this.ddl_Supplier.Items.Clear();

                if (isAudit)
                {
                    this.txt_ItemNo.Text = string.Empty;
                    this.txt_ContractNo.Text = string.Empty;
                }
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
            {
                this.txt_ItemNo.Enabled = false;
                this.txt_ContractNo.Enabled = false;
            }

            this.divMFRN.Visible = isMFRN;

            if (int.Parse(this.ddl_ClaimType.SelectedValue) == UKClaimType.BILL_IN_ADVANCE.Id)
            {
                this.chk_HasUKDN.Enabled = false;
                this.chk_HasUKDN.Checked = false;
                this.upd_UKDebitNote.Enabled = false;
                this.txt_UKDNNo.Enabled = false;
                this.txt_UKDNNo.Text = string.Empty;
                this.txt_UKDNDate.Enabled = false;
                this.txt_UKDNDate.Text = string.Empty;
                this.txt_UKDNReceivedDate.Enabled = false;
                this.txt_UKDNReceivedDate.Text = string.Empty;
            }
            else
            {
                this.chk_HasUKDN.Enabled = true;
                this.chk_HasUKDN.Checked = true;
            }
        }

        protected void ddl_Supplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindSupplierProduct(this.txt_ItemNo.Text.Trim(), this.txt_ContractNo.Text.Trim(), int.Parse(this.ddl_Supplier.SelectedValue));
        }

        protected void txt_ContractNo_TextChanged(object sender, EventArgs e)
        {
            validationType = "OTHERS";
            if (txt_ItemNo.Text.Trim() != string.Empty)
                bindSupplierProduct(this.txt_ItemNo.Text.Trim(), this.txt_ContractNo.Text.Trim());
            if (ddl_Supplier.Enabled)
                this.ddl_Supplier.Focus();
            else
                this.txt_ItemNo.Focus();
        }

        protected void gv_BIAMapping_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UKClaimDef def = this.vwBIAMappingList[e.Row.RowIndex];
                ((LinkButton)e.Row.FindControl("lnk_Attachment")).CommandArgument = e.Row.RowIndex.ToString();
                ((LinkButton)e.Row.FindControl("lnk_Attachment")).Attributes.Add("onclick", "window.open('AttachmentList.aspx?claimId=" + def.ClaimId.ToString() + "', 'attachmentlist', 'width=500,height=500,scrollbars=1,status=0');return false;");

                ((Label)e.Row.FindControl("lbl_ClaimType")).Text = def.Type.Name;
                ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = (def.ContractNo == string.Empty ? def.ItemNo : def.ItemNo + " / " + def.ContractNo);
                ((Label)e.Row.FindControl("lbl_UKDNNo")).Text = def.UKDebitNoteNo;
                ((Label)e.Row.FindControl("lbl_UKDNDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteDate);
                ((Label)e.Row.FindControl("lbl_Month")).Text = (def.ClaimMonth == string.Empty ? "N/A" : def.ClaimMonth);
                ((Label)e.Row.FindControl("lbl_Status")).Text = def.WorkflowStatus.Name;
                ((Label)e.Row.FindControl("lbl_Amt")).Text = def.Currency.CurrencyCode + " " + def.Amount.ToString("#,##0.00");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                decimal totalTotalBIAMappedAmt = 0;
                int totalCnt = 0;
                string currencyCode = string.Empty;
                foreach (UKClaimDef def in this.vwBIAMappingList)
                {
                    currencyCode = def.Currency.CurrencyCode;
                    totalTotalBIAMappedAmt  += def.Amount;
                    totalCnt += 1;
                }
                ((Label)e.Row.FindControl("lbl_TotalAmt")).Text = currencyCode + " " + totalTotalBIAMappedAmt.ToString("#,##0.00");
                ((Label)e.Row.FindControl("lbl_TotalCnt")).Text = "Count = " + totalCnt.ToString("#,##0");
            }
        }

        protected void btn_GoToBIA_Click(object sender, EventArgs e)
        {
            UKClaimBIADiscrepancyDef discrepancyDef = UKClaimManager.Instance.getUKClaimBIADiscrepancyByChildId(this.vwUKClaimDef.ClaimId);

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetUKClaimByKey);
            Context.Items.Add(AccountCommander.Param.claimId, discrepancyDef.ClaimId);
            forwardToScreen("UKClaim.Edit");
        }

    }
}
