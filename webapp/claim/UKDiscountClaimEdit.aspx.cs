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
using System.Text;
using iTextSharp.text.pdf.parser;
using com.next.common.datafactory.worker;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;
using System.Linq;

namespace com.next.isam.webapp.claim
{
    public partial class UKDiscountClaimEdit : com.next.isam.webapp.usercontrol.PageTemplate
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
                UKDiscountClaimDef def = (UKDiscountClaimDef)Context.Items[AccountCommander.Param.claim];
                Action = (string)Context.Items[AccountCommander.Param.action];
                if (def.ClaimId <= 0)
                {
                    Action = "CREATE";
                }
                this.txt_UKDNReceivedDate.Width = 70;
                this.txt_UKDNDate.Width = 70;
                this.ddl_Office.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "", GeneralCriteria.ALL.ToString());
                this.ddl_PaymentOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "SAME AS OFFICE", GeneralCriteria.ALL.ToString());
                this.ddl_HandlingOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "", GeneralCriteria.ALL.ToString());

                this.ddl_TermOfPurchase.Items.Add(new ListItem("-- ALL --", GeneralCriteria.ALL.ToString()));
                this.ddl_TermOfPurchase.Items.Add(new ListItem("FOB", "1"));
                this.ddl_TermOfPurchase.Items.Add(new ListItem("VM", "2"));

                this.ddl_Currency.bindList(WebUtil.getNewCurrencyListForExchangeRate(), "CurrencyCode", "CurrencyId", GeneralCriteria.ALL.ToString(), "--", GeneralCriteria.ALL.ToString());
                this.ddl_Currency.SelectedValue = CurrencyId.USD.Id.ToString();
                this.uclProductTeam.setWidth(300);
                this.uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
                this.ddl_Office.Attributes.Add("onchange", "updateHandlingOffice();");
                this.chk_AppliedDiscount.Checked = def.IsUKDiscount;
                this.btn_Send_Alert.Visible = !def.IsUKDiscount;

                initControls(def, Action);
                this.vwAction = Action;
                this.btn_Cancel.Attributes.Add("onclick", "window.location='UKDiscountClaimSearch.aspx';return false;");
            }
            Action = this.vwAction;
        }

        private void initControls(UKDiscountClaimDef def, string action)
        {
            this.vwUKDiscountClaimDef = def;
            vwAction = action;
            vwRefundList = null;

            this.bindRecord(def);

            this.btn_ViewLog.Attributes.Add("onclick", "window.open('ViewDiscountLog.aspx?claimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.ClaimId.ToString())) + "', 'loglist', 'width=800,height=400,scrollbars=1,status=0');return false;");
            this.btn_ViewAttachment.Attributes.Add("onclick", "window.open('UKDiscountClaimAttachmentList.aspx?claimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.ClaimId.ToString())) + "', 'attachmentlist', 'width=500,height=500,scrollbars=1,status=0');return false;");
            this.btn_Add.Attributes.Add("onclick", getRefundButtonEvent(null));
            this.setControl(action);
        }

        private void setControl(string action)
        {
            this.btn_Send_Alert.Visible = (action != "CREATE") && !this.chk_AppliedDiscount.Checked;
            this.btn_ViewLog.Visible = (action != "CREATE");
            this.btn_ViewAttachment.Visible = (action != "CREATE");
            this.btn_New.Visible = (action == "EDIT");
            this.Panel4.Visible = (action == "EDIT");
            this.txt_RemarkHistory.Visible = (vwUKDiscountClaimDef.ClaimId != -1);

            switch (action)
            {
                case "CREATE":
                    allowInputClaimInfo(true);
                    /*
                    this.row_Refund.Style.Add("display", "none");
                    */
                    break;
                case "REVIEW":
                    allowInputClaimInfo(false);
                    break;
                case "EDIT":
                    allowInputClaimInfo((vwUKDiscountClaimDef.WorkflowStatus.Id != UKDiscountClaimWFS.CLEARED.Id));
                    this.chk_HasUKDN.Enabled = (string.IsNullOrEmpty(vwUKDiscountClaimDef.UKDebitNoteNo));
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
            this.ddl_Supplier.Enabled = enable;
            this.txt_ItemNo.Enabled = enable;
            this.txt_ContractNo.Enabled = enable;
            this.txt_Qty.Enabled = enable;
            this.txt_Amt.Enabled = enable;
            /*
            this.txt_Desc.Enabled = enable;
            */
            this.ddl_Currency.Enabled = enable;

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
            {
                this.ddl_Supplier.Enabled = false;
                this.txt_ItemNo.Enabled = false;
                this.txt_ContractNo.Enabled = false;
            }
        }

        protected void btn_Save_Click(object sender, EventArgs e)
        {
            validationType = "SAVE";
            Page.Validate();
            if (Page.IsValid)
            {
                UKDiscountClaimDef claim = vwUKDiscountClaimDef;

                int lastWFSId = claim.WorkflowStatus.Id;

                claim.HasUKDN = this.chk_HasUKDN.Checked;
                if (chk_HasUKDN.Checked)
                {
                    claim.UKDebitNoteNo = this.txt_UKDNNo.Text;
                    claim.UKDebitNoteDate = (string.IsNullOrEmpty(this.txt_UKDNDate.Text) ? DateTime.MinValue : DateTime.Parse(this.txt_UKDNDate.Text));
                    claim.UKDebitNoteReceivedDate = (string.IsNullOrEmpty(this.txt_UKDNReceivedDate.Text) ? DateTime.MinValue : DateTime.Parse(this.txt_UKDNReceivedDate.Text));
                }
                else
                {
                    claim.UKDebitNoteNo = string.Empty;
                    claim.UKDebitNoteDate = DateTime.MinValue;
                    claim.UKDebitNoteReceivedDate = DateTime.MinValue;
                }

                claim.WorkflowStatus = (Action == "CREATE" ? UKDiscountClaimWFS.OUTSTANDING : claim.WorkflowStatus);

                claim.Qty = int.Parse(this.txt_Qty.Text);
                claim.CurrencyId = this.ddl_Currency.selectedValueToInt;
                claim.Amount = decimal.Parse(this.txt_Amt.Text);
                if (this.txt_Desc.Text.Trim() != string.Empty)
                {
                    if (claim.Remark == string.Empty)
                        claim.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + this.txt_Desc.Text.Trim();
                    else
                        claim.Remark += ("\n" + "[" + DateTime.Today.ToString("dd/MM") + "] " + this.txt_Desc.Text.Trim());
                }

                claim.ItemNo = this.txt_ItemNo.Text.Trim();
                claim.ContractNo = this.txt_ContractNo.Text.Trim();
                claim.OfficeId = this.ddl_Office.selectedValueToInt;

                claim.PaymentOfficeId = this.ddl_PaymentOffice.selectedValueToInt;
                if (this.ddl_PaymentOffice.selectedValueToInt == -1)
                    claim.PaymentOfficeId = claim.OfficeId;

                claim.VendorId = this.ddl_Supplier.selectedValueToInt;
                claim.HandlingOfficeId = this.ddl_HandlingOffice.selectedValueToInt;
                claim.ProductTeamId = (this.uclProductTeam.ProductCodeId == int.MinValue ? GeneralCriteria.ALL : this.uclProductTeam.ProductCodeId);
                claim.TermOfPurchaseId = this.ddl_TermOfPurchase.selectedValueToInt;

                UKDiscountClaimDef updatedDiscountClaim = updateUKDiscountClaim(claim);

                if (this.upd_UKDebitNote.Enabled && this.upd_UKDebitNote.HasFile)
                {
                    string fileUploaded = uploadUKDebitNote();
                }

                PrevAction = Action;
                initControls(updatedDiscountClaim, (Action == "CREATE" ? "EDIT" : Action));
            }

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

            string filename = outputFolder + System.IO.Path.GetFileNameWithoutExtension(this.upd_UKDebitNote.FileName) + System.IO.Path.GetExtension(this.upd_UKDebitNote.FileName).ToLower();
            this.upd_UKDebitNote.SaveAs(filename);
            File.Copy(filename, WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + "backup\\" + tsFolderName + "_" + System.IO.Path.GetFileNameWithoutExtension(this.upd_UKDebitNote.FileName) + System.IO.Path.GetExtension(this.upd_UKDebitNote.FileName).ToLower(), true);

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

        private ArrayList getUKDebitNoteQuery()
        {
            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", "UK Discount"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Debit Note No", this.txt_UKDNNo.Text.Trim()));
            queryStructs.Add(new QueryStructDef("Item No", this.txt_ItemNo.Text.Trim()));
            queryStructs.Add(new QueryStructDef("Qty", this.txt_Qty.Text));
            return queryStructs;
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.initControls(vwUKDiscountClaimDef, Action);
        }

        protected void btn_New_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.CreateUKDiscountClaim);
            forwardToScreen("UKDiscountClaim.Edit");
        }

        protected void btn_Add_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.hid_RefundResult.Value))
            {
                UKDiscountClaimRefundDef def = new UKDiscountClaimRefundDef();
                mapRefundToDef(this.hid_RefundResult.Value, def);
                vwRefundList.Add(def);
                //bindClaimRefundList(vwRefundList);
                //// Add Action Log
                //addClaimRefundLog("Add New Claim Refund Record (ID:" + def.ClaimRefundId.ToString() + ")");
            }
        }

        protected void btn_Send_Alert_Click(object sender, EventArgs e)
        {
            UKDiscountClaimDef def = (UKDiscountClaimDef)Context.Items[AccountCommander.Param.claim];

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.SendAlertToPic);
            Context.Items.Add(AccountCommander.Param.contractNo, def.ContractNo);
            forwardToScreen(null);
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

        protected void ddl_Supplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindSupplierProduct(this.txt_ItemNo.Text.Trim(), this.txt_ContractNo.Text.Trim(), int.Parse(this.ddl_Supplier.SelectedValue));
        }


        private void bindRecord(UKDiscountClaimDef def)
        {
            this.chk_HasUKDN.Checked = def.HasUKDN;
            this.lbl_Status.Text = def.WorkflowStatus.Name;
            this.txt_Status.Text = def.WorkflowStatus.Name;
            this.hid_Status.Value = def.WorkflowStatus.Id.ToString();
            this.txt_ContractNo.Text = def.ContractNo;
            this.txt_UKDNNo.Text = def.UKDebitNoteNo;
            this.txt_UKDNDate.Text = DateTimeUtility.getDateString(def.UKDebitNoteDate);
            this.txt_UKDNReceivedDate.Text = DateTimeUtility.getDateString(def.UKDebitNoteReceivedDate);
            this.txt_Qty.Text = def.Qty.ToString();
            this.txt_Amt.Text = def.Amount.ToString();
            /*
            this.txt_Desc.Text = def.Remark;
            */
            this.txt_Desc.Text = string.Empty;
            this.txt_RemarkHistory.Text = def.Remark;

            this.ddl_Currency.selectByValue(def.CurrencyId.ToString());
            this.txt_ItemNo.Text = def.ItemNo;

            bindSupplierProduct(def);
            if (def.OfficeId == def.PaymentOfficeId)
                this.ddl_PaymentOffice.selectByValue("-1");
            else
                this.ddl_PaymentOffice.selectByValue(def.PaymentOfficeId.ToString());

            vwRefundList = loadClaimRefundList(def.ClaimId);
            bindClaimRefundList(vwRefundList);

            this.ddl_Office.selectByValue(def.OfficeId.ToString());
            this.ddl_HandlingOffice.selectByValue(def.HandlingOfficeId.ToString());
        }

        private void bindSupplierProduct(UKDiscountClaimDef claim)
        {
            bindSupplierProduct(claim.ItemNo, claim.ContractNo, claim.VendorId);
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

        private void bindClaimRefundList(List<UKDiscountClaimRefundDef> refundList)
        {
            this.gv_Refund.DataSource = refundList;
            this.gv_Refund.DataBind();
        }

        private List<UKDiscountClaimRefundDef> loadClaimRefundList(int claimId)
        {
            List<UKDiscountClaimRefundDef> activeList = new List<UKDiscountClaimRefundDef>();
            List<UKDiscountClaimRefundDef> list = UKClaimManager.Instance.getUKDiscountClaimRefundListByClaimId(claimId);
          
            return list;
        }

        protected void gv_Refund_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UKDiscountClaimRefundDef def = vwRefundList[e.Row.RowIndex];

                ImageButton imgBtn = (ImageButton)e.Row.FindControl("imgEditRefund");
                imgBtn.CommandArgument = e.Row.RowIndex.ToString();
                imgBtn.Attributes.Add("onclick", getRefundButtonEvent(def));

                imgBtn = (ImageButton)e.Row.FindControl("imgDeleteRefund");
                imgBtn.CommandArgument = e.Row.RowIndex.ToString();
                imgBtn.Attributes.Add("onclick", "return confirm('Are you sure to delete this Next claim refund record?');");

                ((Label)e.Row.FindControl("lbl_ReceivedDate")).Text = DateTimeUtility.getDateString(def.ReceivedDate);
                ((Label)e.Row.FindControl("lbl_Amount")).Text = def.Amount.ToString("N02");
                ((Label)e.Row.FindControl("lbl_Remark")).Text = def.Remark;
            }
        }

        protected void gv_Refund_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUKClaimRefund")
            {
                UKDiscountClaimRefundDef def = vwRefundList[int.Parse((string)e.CommandArgument)];
                if (!string.IsNullOrEmpty(this.hid_RefundResult.Value))
                {
                    mapRefundToDef(this.hid_RefundResult.Value, def);
                    bindClaimRefundList(vwRefundList);
                    addUKDiscountClaimRefundLog("Update Claim Refund Record (ID:" + def.ClaimRefundId.ToString() + ")");
                }
            }
            if (e.CommandName == "DeleteUKClaimRefund")
            {
                int rowId = int.Parse((string)e.CommandArgument);
                UKDiscountClaimRefundDef def = (UKDiscountClaimRefundDef)this.vwRefundList[rowId];
                /*
                ((Label)e.Row.FindControl("lbl_ReceivedDate")).Text = def.ReceivedDate.ToString("dd/MM/yyyy");
                */
                deleteUKDiscountClaimRefundDef(def);
                this.vwRefundList.Remove(def);
                bindClaimRefundList(vwRefundList);
                addUKDiscountClaimRefundLog("Delete Claim Refund Record (ID:" + def.ClaimRefundId.ToString() + ")");
            }
        }

        private string getRefundButtonEvent(UKDiscountClaimRefundDef refund)
        {
            string param = string.Empty;
            if (refund == null)
                param = "ClaimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(vwUKDiscountClaimDef.ClaimId.ToString())); //+ "&UKClaimRefundId=&Amount=&ReceivedDate=&Remark=";
            else
                if (refund.ClaimRefundId > 0)
                param = "ClaimRefundId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(refund.ClaimRefundId.ToString()));

            return "getRefundResult(window.showModalDialog('UKDiscountClaimRefundEdit.aspx?" + param + "', 'Update_UK_Claim_Refund', 'status:No;dialogWidth:600px;dialogHeight:400px;scrollbars:Yes;resizable:Yes;'))";
        }

        private void mapRefundToDef(string refund, UKDiscountClaimRefundDef def)
        {
            if (!string.IsNullOrEmpty(refund))
            {
                char delimiter = refund[0];
                string[] detail = refund.Split(delimiter);
                string rmk = string.Empty;

                def.ClaimId = vwUKDiscountClaimDef.ClaimId;
                def.ClaimRefundId = int.Parse(detail[1]);
                def.ReceivedDate = DateTime.Parse(detail[2]);
                def.Amount = decimal.Parse(detail[3]);
                for (int i = 6; i < detail.Length; i++)
                    rmk += (rmk == string.Empty ? string.Empty : delimiter.ToString()) + detail[i];
                def.Remark = rmk.Trim();
            }
        }

        private void addUKDiscountClaimRefundLog(string description)
        {
            UKDiscountClaimLogDef log = new UKDiscountClaimLogDef();
            log.ClaimId = vwUKDiscountClaimDef.ClaimId;
            log.LogText = description;
            log.UserId = this.LogonUserId;
            log.FromStatusId = vwUKDiscountClaimDef.WorkflowStatus.Id;
            log.ToStatusId = vwUKDiscountClaimDef.WorkflowStatus.Id;
            log.LogDate = DateTime.Now;
            UKClaimManager.Instance.updateUKDiscountClaimLogDef(log, this.LogonUserId);
        }

        #region Commander routines

        private UKDiscountClaimDef updateUKDiscountClaim(UKDiscountClaimDef ukDiscountClaim)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateUKDiscountClaim);
            Context.Items.Add(AccountCommander.Param.claim, ukDiscountClaim);
            forwardToScreen(null);
            return (UKDiscountClaimDef)Context.Items[AccountCommander.Param.claim];
        }

        private void deleteUKDiscountClaimRefundDef(UKDiscountClaimRefundDef def)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.DeleteUKDiscountClaimRefundDef);
            Context.Items.Add(AccountCommander.Param.claimRefund, def);
            forwardToScreen(null);
        }

        #endregion

        private string vwAction
        {
            get { return (string)ViewState["vwAction"]; }
            set { ViewState["vwAction"] = value; }
        }

        private UKDiscountClaimDef vwUKDiscountClaimDef
        {
            get { return (UKDiscountClaimDef)ViewState["vwUKDiscountClaimDef"]; }
            set { ViewState["vwUKDiscountClaimDef"] = value; }
        }

        private List<UKDiscountClaimRefundDef> vwRefundList
        {
            set { ViewState["UKDiscountClaimRefund"] = value; }
            get { return (List<UKDiscountClaimRefundDef>)ViewState["UKDiscountClaimRefund"]; }
        }

        protected void valCustom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (this.txt_ContractNo.Text.Trim() == string.Empty)
                Page.Validators.Add(new ValidationError("Please input contract no"));

            if (this.txt_ItemNo.Text.Trim() == string.Empty)
                Page.Validators.Add(new ValidationError("Please input item no"));

            if (this.ddl_Currency.selectedValueToInt == -1)
            {
                Page.Validators.Add(new ValidationError("Please specify the currency"));
            }

            int qty = 0;
            if (!int.TryParse(this.txt_Qty.Text, out qty))
                Page.Validators.Add(new ValidationError("Please input a valid quantity"));
            else
                if (qty <= 0)
                    Page.Validators.Add(new ValidationError("Please input a valid quantity"));

        }
        

    }
}
