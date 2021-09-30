using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.appserver.claim;
using System.Web.UI.WebControls;
using com.next.infra.web;
using com.next.isam.domain.claim;
using com.next.common.domain.types;
using com.next.infra.util;
using com.next.common.domain.dms;
using com.next.common.domain.module;

namespace com.next.isam.webapp.claim
{
    public partial class COOApproval : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;

            if (!this.IsPostBack)
            {
                this.uclVendor.setWidth(360);
                this.uclVendor.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
                this.ddl_AccountCode.Items.Add(new ListItem("3301010"));
                this.ddl_AccountCode.Items.Add(new ListItem("1452030"));
                this.ddl_AccountCode.Items.Add(new ListItem("1452031"));
                this.ddl_AccountCode.Items.Add(new ListItem("1452029"));
                this.ddl_AccountCode.Items.Add(new ListItem("1452021"));
                this.ddl_T9.Items.Add(new ListItem("GENERAL"));
                this.ddl_T9.Items.Add(new ListItem("SPECIFIC"));

                this.ddl_Segment7.Items.Add(new ListItem("A - ACTUAL", "A"));
                this.ddl_Segment7.Items.Add(new ListItem("AR - ACCOUNTS RECLASSIFICATION", "AR"));
                this.ddl_Segment7.Items.Add(new ListItem("GP - GENERAL PROVISION (MANUAL)", "GP"));
                this.ddl_Segment7.Items.Add(new ListItem("IR - INTERNAL RECLASSIFICATION", "IR"));
                this.ddl_Segment7.Items.Add(new ListItem("M - MISCELLANEOUS", "M"));
                this.ddl_Segment7.Items.Add(new ListItem("NP - SPECIFIC OR GENERAL PROVISION", "NP"));
                this.ddl_Segment7.Items.Add(new ListItem("R - RELEASE", "R"));
                this.ddl_Segment7.Items.Add(new ListItem("SP - SPECIFIC PROVISION (MANUAL)", "SP"));
                this.ddl_Segment7.Items.Add(new ListItem("U - UTILIZATION OF PROVISION", "U"));

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

        private List<UKClaimRefundDef> vwRefundSearchResult
        {
            set
            {
                ViewState["RefundSearchResult"] = value;
            }
            get
            {
                return (List<UKClaimRefundDef>)ViewState["RefundSearchResult"];
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

                if (def.ClaimRequestId != -1)
                {
                    selectedRequest = svc.GetClaimRequestByKey(def.ClaimRequestId);
                    list.Add(selectedRequest);
                }
            
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
                ((Label)e.Row.FindControl("lbl_FormNo")).Text = (selectedRequest != null ? selectedRequest.FormNo : string.Empty);
                if (def.SettlementOption == UKClaimSettlemtType.PROVISION)
                    ((Label)e.Row.FindControl("lbl_NSRechargePercent")).Text = "Override As Provision";
                else if (def.SettlementOption == UKClaimSettlemtType.SUPPLIER)
                    ((Label)e.Row.FindControl("lbl_NSRechargePercent")).Text = "Override As Supplier"; 
                else
                    ((Label)e.Row.FindControl("lbl_NSRechargePercent")).Text = (selectedRequest != null ? selectedRequest.NSRechargePercent.ToString("0.00") + "%" : "100.00%"); 
            }

        }

        protected void btnList_Click(object sender, EventArgs e)
        {
            TypeCollector officeIds = TypeCollector.Inclusive;
            /*
            foreach (OfficeRef def in this.LogonUserAccessOffices)
                officeIds.append(def.OfficeId);
            */

            ArrayList officeList = CommonUtil.getOfficeList();
            foreach (OfficeRef office in officeList)
                officeIds.append(office.OfficeId);

            List<UKClaimDef> list = UKClaimManager.Instance.getUKClaimCOOApprovalList(officeIds, this.uclVendor.VendorId);
            this.vwSearchResult = list;

            List<UKClaimRefundDef> refundList = UKClaimManager.Instance.getUKClaimRefundCOOApprovalList(officeIds, this.uclVendor.VendorId);
            this.vwRefundSearchResult = refundList;

            this.gv_UKClaim.DataSource = list;
            this.gv_UKClaim.DataBind();
            this.pnlHeader.Visible = true;

            this.gv_UKClaimRefund.DataSource = refundList;
            this.gv_UKClaimRefund.DataBind();
            this.pnlRefundHeader.Visible = true;

            if (list.Count > 0 || refundList.Count > 0) this.pnlDoc.Visible = true;
        }

        private ArrayList getQueryStringList(UKClaimDef ukClaimDef, string docType)
        {
            ArrayList queryStructs = new ArrayList();
            ArrayList returnList = new ArrayList();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
            QAIS.ClaimRequestDef def = svc.GetClaimRequestByKey(ukClaimDef.ClaimRequestId);

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Item No", ukClaimDef.ItemNo));
            queryStructs.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription));
            if (def.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                queryStructs.Add(new QueryStructDef("MFRN Month", def.ClaimMonth));

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            DocumentInfoDef docDef = null;
            foreach (DocumentInfoDef docInfoDef in qList)
            {
                docDef = docInfoDef;
                break;
            }

            returnList.Add(new QueryStructDef("Supporting Doc Type", docType));
            foreach (FieldInfoDef fiDef in docDef.FieldInfos)
            {
                if (fiDef.FieldName != "Supporting Doc Type" && fiDef.FieldName != "Debit Note No" && fiDef.FieldName != "Qty")
                    returnList.Add(new QueryStructDef(fiDef.FieldName, fiDef.Content));
            }
            if (ukClaimDef.UKDebitNoteNo != string.Empty)
                returnList.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));
            return returnList;
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
                if (!System.IO.Directory.Exists(outputFolder))
                    System.IO.Directory.CreateDirectory(outputFolder);

                FileUpload fu = this.updDoc;
                string filename = string.Empty;
                if (fu.HasFile)
                {
                    filename = Path.GetFileNameWithoutExtension(fu.FileName);
                    string path = outputFolder + filename + Path.GetExtension(fu.FileName).ToLower();
                    fu.SaveAs(path);

                    foreach (GridViewRow r in this.gv_UKClaim.Rows)
                    {
                        if (((CheckBox)r.FindControl("chkUKClaim")).Checked)
                        {
                            UKClaimDef def = this.vwSearchResult[r.RowIndex];
                            ArrayList queryStructs = this.getQueryStringList(def, "Signed Copy By COO");

                            ArrayList qList = DMSUtil.queryDocument(queryStructs);
                            long docId = 0;
                            ArrayList attachmentList = new ArrayList();

                            foreach (DocumentInfoDef docInfoDef in qList)
                            {
                                docId = docInfoDef.DocumentID;
                            }

                            if (docId > 0)
                            {
                                attachmentList.Add(path);
                                string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                            }
                            else
                            {
                                attachmentList.Add(path);
                                string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + def.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);
                            }

                            int newWorkflowStatusId = ClaimWFS.COO_SIGNED_OFF.Id;

                            def.PnLAccountCode = this.ddl_AccountCode.SelectedValue + "," + (this.ddl_AccountCode.SelectedValue == "1452030" ? this.ddl_T9.SelectedValue : string.Empty) + "," + (this.ddl_AccountCode.SelectedValue == "1452030" ? this.ddl_Segment7.SelectedValue : string.Empty);
                            UKClaimManager.Instance.updateUKClaimDef(def, this.LogonUserId);
                            UKClaimManager.Instance.setClaimWorkflowStatus(def.ClaimId, newWorkflowStatusId, this.LogonUserId);

                            UKClaimLogDef logDef = new UKClaimLogDef(def.ClaimId, "Add Attachment [Signed Copy By COO] (" + filename + Path.GetExtension(fu.FileName).ToLower() + ")", this.LogonUserId, def.WorkflowStatus.Id, newWorkflowStatusId);
                            UKClaimManager.Instance.updateUKClaimLogDef(logDef, this.LogonUserId);

                        }
                    }


                    foreach (GridViewRow r in this.gv_UKClaimRefund.Rows)
                    {
                        if (((CheckBox)r.FindControl("chkUKClaimRefund")).Checked)
                        {
                            UKClaimRefundDef def = this.vwRefundSearchResult[r.RowIndex];
                            UKClaimDef claimDef = UKClaimManager.Instance.getUKClaimByKey(def.ClaimId);
                            ArrayList queryStructs = this.getQueryStringList(claimDef, "Signed Copy By COO");

                            ArrayList qList = DMSUtil.queryDocument(queryStructs);
                            long docId = 0;
                            ArrayList attachmentList = new ArrayList();

                            foreach (DocumentInfoDef docInfoDef in qList)
                            {
                                docId = docInfoDef.DocumentID;
                            }

                            if (docId > 0)
                            {
                                attachmentList.Add(path);
                                string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                            }
                            else
                            {
                                attachmentList.Add(path);
                                string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + def.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);
                            }

                            int newWorkflowStatusId = ClaimWFS.COO_SIGNED_OFF.Id;
                            if (claimDef.WorkflowStatus == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER)
                                newWorkflowStatusId = ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id;

                            def.PnLAccountCode = this.ddl_AccountCode.SelectedValue + "," + (this.ddl_AccountCode.SelectedValue == "1452030" ? this.ddl_T9.SelectedValue : string.Empty) + "," + (this.ddl_AccountCode.SelectedValue == "1452030" ? this.ddl_Segment7.SelectedValue : string.Empty);
                            UKClaimManager.Instance.updateUKClaimRefundDef(def, this.LogonUserId);
                            UKClaimManager.Instance.setClaimWorkflowStatus(def.ClaimId, newWorkflowStatusId, this.LogonUserId);

                            UKClaimLogDef logDef = new UKClaimLogDef(def.ClaimId, "Add Attachment [Signed Copy By COO] (" + filename + Path.GetExtension(fu.FileName).ToLower() + ")", this.LogonUserId, claimDef.WorkflowStatus.Id, newWorkflowStatusId);
                            UKClaimManager.Instance.updateUKClaimLogDef(logDef, this.LogonUserId);
                        }
                    }

                }
                FileUtility.clearFolder(outputFolder, false);
                this.btnList_Click(null, null);
            }

        }

        protected void valCustom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!this.updDoc.HasFile)
                Page.Validators.Add(new ValidationError("Please specify the file"));

            int i = 0;
            foreach (GridViewRow r in this.gv_UKClaim.Rows)
            {
                if (((CheckBox)r.FindControl("chkUKClaim")).Checked) i += 1;
            }
            foreach (GridViewRow r in this.gv_UKClaimRefund.Rows)
            {
                if (((CheckBox)r.FindControl("chkUKClaimRefund")).Checked) i += 1;
            }

            if (i == 0)
                Page.Validators.Add(new ValidationError("Please select at least one Next Claim / Refund"));

        }

        protected void gv_UKClaimRefund_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UKClaimRefundDef refundDef = this.vwRefundSearchResult[e.Row.RowIndex];
                UKClaimDef def = UKClaimManager.Instance.getUKClaimByKey(refundDef.ClaimId);

                QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

                List<QAIS.ClaimRequestDef> list = new List<QAIS.ClaimRequestDef>();
                QAIS.ClaimRequestDef selectedRequest = null;

                if (def.ClaimRequestId != -1)
                {
                    selectedRequest = svc.GetClaimRequestByKey(def.ClaimRequestId);
                    list.Add(selectedRequest);
                }

                if (def.ContractNo != string.Empty)
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo + "/" + def.ContractNo;
                else
                    ((Label)e.Row.FindControl("lbl_ItemContractNo")).Text = def.ItemNo;

                ((Label)e.Row.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);
                ((Label)e.Row.FindControl("lbl_ClaimType")).Text = def.Type.Name;
                ((Label)e.Row.FindControl("lbl_Vendor")).Text = def.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_UKDebitNoteNo")).Text = def.UKDebitNoteNo;
                ((Label)e.Row.FindControl("lbl_Amount")).Text = def.Currency.CurrencyCode + " " + refundDef.Amount.ToString("#,###.00");
                if (refundDef.ReceivedDate != DateTime.MinValue)
                    ((Label)e.Row.FindControl("lbl_RefundReceivedDate")).Text = DateTimeUtility.getDateString(refundDef.ReceivedDate);
                ((Label)e.Row.FindControl("lbl_FormNo")).Text = (selectedRequest != null ? selectedRequest.FormNo : string.Empty);
                if (refundDef.SettlementOption == UKClaimSettlemtType.PROVISION)
                    ((Label)e.Row.FindControl("lbl_NSRechargePercent")).Text = "Override As Provision";
                else if (refundDef.SettlementOption == UKClaimSettlemtType.SUPPLIER)
                    ((Label)e.Row.FindControl("lbl_NSRechargePercent")).Text = "Override As Supplier";
                else
                    ((Label)e.Row.FindControl("lbl_NSRechargePercent")).Text = (selectedRequest != null ? selectedRequest.NSRechargePercent.ToString("0.00") + "%" : "100.00%");
            }


        }


    }
}
