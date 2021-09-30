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
    public partial class UploadDoc : com.next.isam.webapp.usercontrol.PageTemplate
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
                /*
                ViewState["IsReviewed"] = (Context.Items[AccountCommander.Param.isReviewed]).ToString();
                */

                if (Context.Items[AccountCommander.Param.fileName] != null)
                    ViewState["FileName"] = Context.Items[AccountCommander.Param.fileName].ToString();

                if (int.Parse(ViewState["DocId"].ToString()) == 0)
                    this.lblMode.Text = "Add : " + ViewState["DocType"].ToString();
                else
                    this.lblMode.Text = "Revise : " + ViewState["DocType"].ToString();

                int claimId = int.Parse(Context.Items[AccountCommander.Param.claimId].ToString());

                UKClaimDef def = UKClaimManager.Instance.getUKClaimByKey(claimId);
                this.vwUKClaimDef = def;

                this.lbl_UKDNNo.Text = def.UKDebitNoteNo;
                this.lbl_ClaimType.Text = def.Type.Name;
                this.lbl_Status.Text = def.WorkflowStatus.Name;
                ProgressBar.showDMS(this.btnSave);
            }
        }

        private UKClaimDef vwUKClaimDef
        {
            get
            {
                return (UKClaimDef)ViewState["vwUKClaimDef"];
            }
            set
            {
                ViewState["vwUKClaimDef"] = value;
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
                /*
                UKClaimLogDef logDef = new UKClaimLogDef(vwUKClaimDef.ClaimId, "Revise Attachment [Next Claim DN] (" + Path.GetFileNameWithoutExtension(ViewState["FileName"].ToString()) + Path.GetExtension(this.FileUpload1.FileName).ToLower() + ")", this.LogonUserId, vwUKClaimDef.WorkflowStatus.Id, vwUKClaimDef.WorkflowStatus.Id);
                */
                UKClaimLogDef logDef = new UKClaimLogDef(vwUKClaimDef.ClaimId, "Revise Attachment [" + ViewState["DocType"].ToString() + "] (" + Path.GetFileNameWithoutExtension(this.FileUpload1.FileName) + Path.GetExtension(this.FileUpload1.FileName).ToLower() + ")", this.LogonUserId, vwUKClaimDef.WorkflowStatus.Id, vwUKClaimDef.WorkflowStatus.Id);
                UKClaimManager.Instance.updateUKClaimLogDef(logDef, this.LogonUserId);

            }
            FileUtility.clearFolder(outputFolder, false);
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='attachmentlist.aspx?claimid=" + this.vwUKClaimDef.ClaimId.ToString() + (ViewState["IsReviewed"].ToString() == "Y" ? "&rv=1" : string.Empty) + "';", true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='attachmentlist.aspx?claimid=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(this.vwUKClaimDef.ClaimId.ToString())) + "';", true);
        }

        private ArrayList getQueryStringList(string docType)
        {
            ArrayList queryStructs = new ArrayList();
            ArrayList returnList = new ArrayList();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
            QAIS.ClaimRequestDef def = svc.GetClaimRequestByKey(this.vwUKClaimDef.ClaimRequestId);

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Authorization Form"));
            queryStructs.Add(new QueryStructDef("Form No", def.FormNo));
            queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
            queryStructs.Add(new QueryStructDef("Claim Type", this.vwUKClaimDef.Type.DMSDescription));
            if (def.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                queryStructs.Add(new QueryStructDef("MFRN Month", def.ClaimMonth));
            /*
            queryStructs.Add(new QueryStructDef("Debit Note No", this.vwUKClaimDef.UKDebitNoteNo.Trim()));
            */

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
                if (this.vwUKClaimDef.UKDebitNoteNo != string.Empty)
                {
                    returnList.Add(new QueryStructDef("Debit Note No", this.vwUKClaimDef.UKDebitNoteNo));
                    returnList.Add(new QueryStructDef("Qty", this.vwUKClaimDef.Quantity.ToString()));
                }
            }
            else
            {
                returnList.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                returnList.Add(new QueryStructDef("Supporting Doc Type", docType));
                returnList.Add(new QueryStructDef("Claim Type", this.vwUKClaimDef.Type.DMSDescription));
                returnList.Add(new QueryStructDef("Item No", this.vwUKClaimDef.ItemNo));
                returnList.Add(new QueryStructDef("Debit Note No", this.vwUKClaimDef.UKDebitNoteNo));
                returnList.Add(new QueryStructDef("MFRN Month", this.vwUKClaimDef.ClaimMonth));
                returnList.Add(new QueryStructDef("Form No", this.vwUKClaimDef.UKDebitNoteNo));
                returnList.Add(new QueryStructDef("Supplier Name", this.vwUKClaimDef.Vendor.Name));
                returnList.Add(new QueryStructDef("Qty", this.vwUKClaimDef.Quantity.ToString()));
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
                            string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + this.vwUKClaimDef.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);
                        }

                        int newWorkflowStatusId = this.vwUKClaimDef.WorkflowStatus.Id;
                        if (int.Parse(ViewState["ToStatusId"].ToString()) != -1 && int.Parse(ViewState["ToStatusId"].ToString()) > this.vwUKClaimDef.WorkflowStatus.Id)
                        {
                            UKClaimManager.Instance.setClaimWorkflowStatus(this.vwUKClaimDef.ClaimId, int.Parse(ViewState["ToStatusId"].ToString()), this.LogonUserId);
                            newWorkflowStatusId = int.Parse(ViewState["ToStatusId"].ToString());
                        }

                        UKClaimLogDef logDef = new UKClaimLogDef(vwUKClaimDef.ClaimId, "Add Attachment [" + ViewState["DocType"].ToString() + "] (" + filename + Path.GetExtension(fu.FileName).ToLower() + ")", this.LogonUserId, vwUKClaimDef.WorkflowStatus.Id, newWorkflowStatusId);
                        UKClaimManager.Instance.updateUKClaimLogDef(logDef, this.LogonUserId);

                    }
                    FileUtility.clearFolder(outputFolder, false);
                    /*
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='attachmentlist.aspx?claimId=" + this.vwUKClaimDef.ClaimId.ToString() + (ViewState["IsReviewed"].ToString() == "Y" ? "&rv=1" : string.Empty) + "';", true);
                    */
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='attachmentlist.aspx?claimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(this.vwUKClaimDef.ClaimId.ToString())) + "';", true);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='attachmentlist.aspx?claimId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(this.vwUKClaimDef.ClaimId.ToString())) + "';", true);
        }

    }
}