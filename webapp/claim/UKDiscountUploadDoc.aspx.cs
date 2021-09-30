using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.util;
using com.next.infra.web;
using com.next.common.domain.module;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.types;
using com.next.isam.domain.claim;
using com.next.isam.appserver.claim;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using System.IO;

namespace com.next.isam.webapp.claim
{
    public partial class UKDiscountUploadDoc : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            if (!Page.IsPostBack)
            {
                int toStatusId = (int)Context.Items[AccountCommander.Param.toClaimStatusId];
                ViewState["ToStatusId"] = ((int)Context.Items[AccountCommander.Param.toClaimStatusId]).ToString();
                ViewState["DocType"] = (Context.Items[AccountCommander.Param.docType].ToString());
                ViewState["DocId"] = (Context.Items[AccountCommander.Param.docId]).ToString();

                if (Context.Items[AccountCommander.Param.fileName] != null)
                    ViewState["FileName"] = Context.Items[AccountCommander.Param.fileName].ToString();

                if (int.Parse(ViewState["DocId"].ToString()) == 0)
                    this.lblMode.Text = "Add : " + ViewState["DocType"].ToString();
                else
                    this.lblMode.Text = "Revise : " + ViewState["DocType"].ToString();

                int claimId = int.Parse(Context.Items[AccountCommander.Param.claimId].ToString());

                UKDiscountClaimDef def = UKClaimManager.Instance.getUKDiscountClaimByKey(claimId);
                this.vwUKDiscountClaimDef = def;

                this.lbl_UKDNNo.Text = def.UKDebitNoteNo;
                this.lbl_Status.Text = def.WorkflowStatus.Name;
                ProgressBar.showDMS(this.btnSave);
            }
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

        private void reviseDocument()
        {
            string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);

            ArrayList queryStructs = new ArrayList();

            DocumentInfoDef docInfoDef = DMSUtil.queryDocumentByID(int.Parse(ViewState["DocId"].ToString()));

            foreach (FieldInfoDef fiDef in docInfoDef.FieldInfos)
                queryStructs.Add(new QueryStructDef(fiDef.FieldName, fiDef.Content));

            ArrayList attachmentList = new ArrayList();

            if (docInfoDef != null)
            {
                DMSUtil.DeleteSingleAttachment(int.Parse(ViewState["DocId"].ToString()), queryStructs, ViewState["FileName"].ToString());

                string filename = outputFolder + Path.GetFileNameWithoutExtension(FileUpload1.FileName) + Path.GetExtension(this.FileUpload1.FileName).ToLower(); 
                this.FileUpload1.SaveAs(filename);
                attachmentList.Add(filename);
                string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docInfoDef.DocumentID, queryStructs, attachmentList);
                UKDiscountClaimLogDef logDef = new UKDiscountClaimLogDef(vwUKDiscountClaimDef.ClaimId, "Revise Attachment [" + ViewState["DocType"].ToString() + "] (" + Path.GetFileNameWithoutExtension(this.FileUpload1.FileName) + Path.GetExtension(this.FileUpload1.FileName).ToLower() + ")", this.LogonUserId, vwUKDiscountClaimDef.WorkflowStatus.Id, vwUKDiscountClaimDef.WorkflowStatus.Id);
                UKClaimManager.Instance.updateUKDiscountClaimLogDef(logDef, this.LogonUserId);
            }
            FileUtility.clearFolder(outputFolder, false);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='ukdiscountclaimattachmentlist.aspx?claimid=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(this.vwUKDiscountClaimDef.ClaimId.ToString())) + "';", true);
        }

        private ArrayList getQueryStringList(string docType)
        {
            ArrayList queryStructs = new ArrayList();
            ArrayList returnList = new ArrayList();

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Debit Note No", this.vwUKDiscountClaimDef.UKDebitNoteNo));
            queryStructs.Add(new QueryStructDef("Item No", this.vwUKDiscountClaimDef.ItemNo));
            queryStructs.Add(new QueryStructDef("Claim Type", "UK Discount"));

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            DocumentInfoDef docDef = null;
            if (qList.Count > 0)
            {
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
                if (this.vwUKDiscountClaimDef.UKDebitNoteNo != string.Empty)
                {
                    returnList.Add(new QueryStructDef("Debit Note No", this.vwUKDiscountClaimDef.UKDebitNoteNo));
                    returnList.Add(new QueryStructDef("Qty", this.vwUKDiscountClaimDef.Qty.ToString()));
                }
            }
            else
            {
                returnList.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                returnList.Add(new QueryStructDef("Supporting Doc Type", docType));
                returnList.Add(new QueryStructDef("Claim Type", "UK Discount"));
                returnList.Add(new QueryStructDef("Item No", this.vwUKDiscountClaimDef.ItemNo));
                returnList.Add(new QueryStructDef("Debit Note No", this.vwUKDiscountClaimDef.UKDebitNoteNo));
            }
            return returnList;
        }

        protected void btnSave_Click(object sender, System.EventArgs e)
        {
            if (this.FileUpload1.HasFile)
            {
                if (int.Parse(ViewState["DocId"].ToString()) != 0)
                    this.reviseDocument();
                else
                {
                    string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
                    if (!System.IO.Directory.Exists(outputFolder))
                        System.IO.Directory.CreateDirectory(outputFolder);

                    bool isFirst = true;
                    FileUpload fu = this.FileUpload1;
                    string filename = string.Empty;
                    if (fu.HasFile)
                    {
                        filename = Path.GetFileNameWithoutExtension(fu.FileName);
                        ArrayList queryStructs = this.getQueryStringList(ViewState["DocType"].ToString());

                        ArrayList qList = DMSUtil.queryDocument(queryStructs);
                        long docId = 0;
                        ArrayList attachmentList = new ArrayList();

                        foreach (DocumentInfoDef docInfoDef in qList)
                        {
                            docId = docInfoDef.DocumentID;
                        }

                        if (docId > 0)
                        {
                            string path = outputFolder + filename + Path.GetExtension(fu.FileName).ToLower();
                            fu.SaveAs(path);
                            attachmentList.Add(path);
                            string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                        }
                        else
                        {
                            string path = outputFolder + filename + Path.GetExtension(fu.FileName).ToLower();
                            fu.SaveAs(path);
                            attachmentList.Add(path);
                            string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "DISCOUNTCLAIMID-" + this.vwUKDiscountClaimDef.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);
                        }

                        int newWorkflowStatusId = this.vwUKDiscountClaimDef.WorkflowStatus.Id;

                        UKDiscountClaimLogDef logDef = new UKDiscountClaimLogDef(vwUKDiscountClaimDef.ClaimId, "Add Attachment [" + ViewState["DocType"].ToString() + "] (" + filename + Path.GetExtension(fu.FileName).ToLower() + ")", this.LogonUserId, vwUKDiscountClaimDef.WorkflowStatus.Id, newWorkflowStatusId);
                        UKClaimManager.Instance.updateUKDiscountClaimLogDef(logDef, this.LogonUserId);

                    }
                    FileUtility.clearFolder(outputFolder, false);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='ukdiscountclaimattachmentlist.aspx?claimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(this.vwUKDiscountClaimDef.ClaimId.ToString())) + "';", true);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='ukdiscountclaimattachmentlist.aspx?claimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(this.vwUKDiscountClaimDef.ClaimId.ToString())) + "';", true);
        }

    }
}