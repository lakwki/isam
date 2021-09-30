using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain.module;
using com.next.common.domain.dms;
using com.next.isam.domain.claim;
using com.next.infra.web;
using com.next.isam.appserver.claim;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.infra.util;
using System.IO;
using com.next.isam.dataserver.worker;

namespace com.next.isam.webapp.claim
{
    public partial class AttachmentList : com.next.isam.webapp.usercontrol.PageTemplate
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            int claimId = 0;
            QAIS.ClaimRequestDef def = null;

            if (!Page.IsPostBack)
                claimId = int.Parse(EncryptionUtility.DecryptParam(Request.Params["claimId"].ToString()));
            else
                claimId = this.vwUKClaimDef.ClaimId;

            this.vwUKClaimDef = UKClaimManager.Instance.getUKClaimByKey(claimId);

            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

            if (this.vwUKClaimDef.ClaimRequestId != -1)
            {
                def = svc.GetClaimRequestByKey(this.vwUKClaimDef.ClaimRequestId);
                this.vwClaimRequestDef = def;
            }
            else if (this.vwUKClaimDef.Type == UKClaimType.BILL_IN_ADVANCE && this.vwUKClaimDef.DebitNoteNo.Trim() != string.Empty)
            {
                def = svc.GetClaimRequestByKey(int.Parse(this.vwUKClaimDef.DebitNoteNo));
                this.vwClaimRequestDef = def;
            }

            if (!Page.IsPostBack)
            {
                /*
                if (Request.Params["rv"] != null)
                    ViewState["IsReviewed"] = "Y";
                else
                    ViewState["IsReviewed"] = "N";
                */
                this.bindRecord();
            }
        }

        private ArrayList getBIAAttachmentList()
        {
            ArrayList queryStructs = new ArrayList();
            UKClaimDef ukClaimDef = this.vwUKClaimDef;

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription));
            queryStructs.Add(new QueryStructDef("Item No", ukClaimDef.ItemNo));
            queryStructs.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            ArrayList attachmentInfoList = new ArrayList();

            foreach (DocumentInfoDef docInfoDef in qList)
            {
                foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                {
                    FieldInfoDef fiDef = (FieldInfoDef)docInfoDef.FieldInfos[1];
                    attDef.Description = fiDef.Content + "," + docInfoDef.DocumentID.ToString();
                    if (attDef.FileName.IndexOf(".html") == -1)
                        attachmentInfoList.Add(attDef);
                }
            }

            return attachmentInfoList;
        }

        private void bindRecord()
        {
            this.lnkAddCOOCopy.Visible = false;
            this.lnkAddDirectorCopy.Visible = false;

            if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
            {
                this.lnkAddCOOCopy.Visible = false;
                this.lnkAddCOOInstructions.Visible = false;
                this.lnkAddDirectorCopy.Visible = false;
                this.lnkCNToSupplier.Visible = false;
                this.lnkOtherDoc.Visible = false;
                this.lnkRefund.Visible = false;
            }

            UKClaimDef ukClaimDef = this.vwUKClaimDef;

            this.lblUKDNNo.Text = (ukClaimDef.UKDebitNoteNo == string.Empty ? "N/A" : ukClaimDef.UKDebitNoteNo);
            this.lblStatus.Text = ukClaimDef.WorkflowStatus.Name;

            ArrayList queryStructs = new ArrayList();
            
            if (ukClaimDef.ClaimRequestId == -1 && !(ukClaimDef.Type == UKClaimType.BILL_IN_ADVANCE && ukClaimDef.DebitNoteNo.Trim() != string.Empty))
            {
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                queryStructs.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription));
                /*
                queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
                */
                queryStructs.Add(new QueryStructDef("Item No", ukClaimDef.ItemNo));
                queryStructs.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));
            }
            else
            {
                QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

                QAIS.ClaimRequestDef def = this.vwClaimRequestDef;
                    
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ukClaimReview.Id) && def.NSRechargePercent > 0 && ukClaimDef.WorkflowStatus.Id == ClaimWFS.SUBMITTED.Id) 
                    this.lnkAddDirectorCopy.Visible = true;

                /*
                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ukClaimReview.Id) && def.NSRechargePercent > 0 && ukClaimDef.WorkflowStatus.Id == ClaimWFS.USER_SIGNED_OFF.Id)
                    this.lnkAddCOOCopy.Visible = true;
                */

                if (def.NSRechargePercent > 0 && ukClaimDef.WorkflowStatus.Id == ClaimWFS.USER_SIGNED_OFF.Id)
                    this.lnkAddCOOCopy.Visible = true;


                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                queryStructs.Add(new QueryStructDef("Form No", def.FormNo));
                queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
                queryStructs.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription)); 
                if (def.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                    queryStructs.Add(new QueryStructDef("MFRN Month", def.ClaimMonth));
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ukClaimReview.Id) && ukClaimDef.IsInterfaced == false && ukClaimDef.IsRechargeInterfaced == false)
                this.btnReject.Visible = true;
            else
                this.btnReject.Visible = false;

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            ArrayList attachmentInfoList = new ArrayList();

            foreach (DocumentInfoDef docInfoDef in qList)
            {
                foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                {
                    FieldInfoDef fiDef = (FieldInfoDef)docInfoDef.FieldInfos[1];
                    attDef.Description = fiDef.Content + "," + docInfoDef.DocumentID.ToString();
                    if (attDef.FileName.IndexOf(".html") == -1)
                        attachmentInfoList.Add(attDef);
                }
            }

            if (ukClaimDef.Type == UKClaimType.BILL_IN_ADVANCE && ukClaimDef.DebitNoteNo.Trim() != string.Empty)
                attachmentInfoList.AddRange(this.getBIAAttachmentList());

            this.vwAttachmentList = attachmentInfoList;
            this.gvAttachment.DataSource = attachmentInfoList;
            this.gvAttachment.DataBind();
        }

        private QAIS.ClaimRequestDef vwClaimRequestDef
        {
            get { return (QAIS.ClaimRequestDef)ViewState["vwClaimRequestDef"]; }
            set { ViewState["vwClaimRequestDef"] = value; }
        }

        private UKClaimDef vwUKClaimDef
        {
            get { return (UKClaimDef)ViewState["vwUKClaimDef"]; }
            set { ViewState["vwUKClaimDef"] = value; }
        }

        private ArrayList vwAttachmentList
        {
            set {  ViewState["AttachmentList"] = value; }
            get { return (ArrayList)ViewState["AttachmentList"]; }
        }

        private bool isFileTypeEditable(string fileType)
        {
            if (fileType == "Authorization Form" ||
                fileType == "Other Email Correspondence" ||
                fileType == "Sample" ||
                fileType == "Signed form from Fty" ||
                fileType == "UKClaim Reject Doc")
                return false;
            else
                return true;
        }

        protected void gvAttachment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AttachmentInfoDef def = (AttachmentInfoDef)this.vwAttachmentList[e.Row.RowIndex];
                ((LinkButton)e.Row.FindControl("lnk_FileName")).Text = def.FileName;
                ((LinkButton)e.Row.FindControl("lnk_FileName")).CommandArgument = e.Row.RowIndex.ToString();
                ((ImageButton)e.Row.FindControl("imgMail")).CommandArgument = e.Row.RowIndex.ToString();

                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
                {
                    ((ImageButton)e.Row.FindControl("imgMail")).Enabled = false;
                    ((ImageButton)e.Row.FindControl("imgEdit")).Enabled = false;
                }

                ((Label)e.Row.FindControl("lbl_FileName")).Text = def.FileName;

                bool isEditable = this.isFileTypeEditable(def.Description.Split(',')[0]);
                ((Label)e.Row.FindControl("lbl_FileName")).Visible = false;
                /*
                ((LinkButton)e.Row.FindControl("lnk_FileName")).Visible = isEditable;
                ((Label)e.Row.FindControl("lbl_FileName")).Visible = !isEditable;
                */

                ((Label)e.Row.FindControl("lbl_Type")).Text = def.Description.Split(',')[0]; 
                ((Label)e.Row.FindControl("lbl_UploadDate")).Text = def.LastModifyDate.ToString("dd/MM/yyyy HH:mm");
                ((Label)e.Row.FindControl("lbl_MajorId")).Text = def.MajorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_MinorId")).Text = def.MinorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_BuildId")).Text = def.Build.ToString();
                ((ImageButton)e.Row.FindControl("imgEdit")).CommandArgument = e.Row.RowIndex.ToString();
                ((ImageButton)e.Row.FindControl("imgEdit")).Visible = isEditable;
            }
        }

        protected void gvAttachment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            AttachmentInfoDef def = (AttachmentInfoDef)this.vwAttachmentList[int.Parse((string)e.CommandArgument)];

            if (e.CommandName == "OpenAttachment")
            {
                Context.Items.Clear();
                string[] s = def.Description.Split(',');
                Context.Items.Add("docId", s[1]);
                Context.Items.Add("attId", def.AttachmentID.ToString());
                Context.Items.Add("majorId", def.MajorVerion.ToString());
                Context.Items.Add("minorId", def.MinorVerion.ToString());
                Context.Items.Add("buildId", def.Build.ToString());
                forwardToScreen("dms.Attachment");
            }
            else if (e.CommandName == "ReviseDoc")
            {
                Context.Items.Clear();
                Context.Items.Add(AccountCommander.Param.toClaimStatusId, -1);
                Context.Items.Add(AccountCommander.Param.docId, def.Description.Split(',')[1]);
                Context.Items.Add(AccountCommander.Param.docType, def.Description.Split(',')[0]);
                Context.Items.Add(AccountCommander.Param.fileName, def.FileName);
                Context.Items.Add(AccountCommander.Param.claimId, this.vwUKClaimDef.ClaimId.ToString());
                /*
                Context.Items.Add(AccountCommander.Param.isReviewed, ViewState["IsReviewed"].ToString());
                */
                forwardToScreen("UKClaim.Upload");
            }
            else if (e.CommandName == "MailDoc")
            {
                Context.Items.Clear();
                string[] s = def.Description.Split(',');
                Context.Items.Add("docId", s[1]);
                Context.Items.Add("attId", def.AttachmentID.ToString());
                Context.Items.Add("majorId", def.MajorVerion.ToString());
                Context.Items.Add("minorId", def.MinorVerion.ToString());
                Context.Items.Add("buildId", def.Build.ToString());

                string docType = def.Description.Split(',')[0];

                DocumentInfoDef docInfoDef = DMSUtil.queryDocumentByID(long.Parse(s[1]), def.MajorVerion, def.MinorVerion, def.Build);
                AttachmentInfoDef attDef = null;

                foreach (AttachmentInfoDef attachInfoDef in docInfoDef.AttachmentInfos)
                {
                    if (attachInfoDef.AttachmentID == def.AttachmentID)
                        attDef = attachInfoDef;
                }

                string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
                if (!System.IO.Directory.Exists(outputFolder))
                    System.IO.Directory.CreateDirectory(outputFolder);

                System.Byte[] byteDownload = DMSUtil.getAttachment(attDef.AttachmentID);
                FileStream fs = new System.IO.FileStream(outputFolder + attDef.FileName, FileMode.Create, FileAccess.Write);
                fs.Write(byteDownload, 0, byteDownload.Length);
                fs.Close();

                NoticeHelper.sendNextClaimMailArchive(this.vwUKClaimDef, docType, outputFolder + attDef.FileName, this.LogonUserId);
                FileUtility.clearFolder(outputFolder, false);

                forwardToScreen(null);
            }
        }

        private void openAddDocumentScreen(ClaimWFS newStatus, string docType)
        {
            Context.Items.Clear();
            Context.Items.Add(AccountCommander.Param.toClaimStatusId, newStatus.Id);
            Context.Items.Add(AccountCommander.Param.docId, "0");
            Context.Items.Add(AccountCommander.Param.docType, docType);
            Context.Items.Add(AccountCommander.Param.claimId, this.vwUKClaimDef.ClaimId.ToString());
            /*
            Context.Items.Add(AccountCommander.Param.isReviewed, ViewState["IsReviewed"].ToString());
            */
            forwardToScreen("UKClaim.Upload");
        }

        protected void lnkAddDirectorCopy_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(ClaimWFS.USER_SIGNED_OFF, "Signed Copy By Director");
        }

        protected void lnkAddCOOCopy_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(ClaimWFS.COO_SIGNED_OFF, "Signed Copy By COO");
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(ClaimWFS.REJECTED, "UKClaim Reject Doc");
        }

        protected void lnkRefund_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(this.vwUKClaimDef.WorkflowStatus, "Next Claim Refund Supporting Doc");
        }

        protected void lnkCNToSupplier_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(this.vwUKClaimDef.WorkflowStatus, "CN To Supplier");
        }

        protected void lnkAddCOOInstructions_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(this.vwUKClaimDef.WorkflowStatus, "COO Instructions Copy");
        }

        protected void lnkOtherDoc_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(this.vwUKClaimDef.WorkflowStatus, "Other Supporting Doc");
        }

        /*
        protected void lnk_NSSIGN_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(this.vwUKClaimDef.WorkflowStatus, "Signed Form From NS");
        }

        protected void lnk_GM_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(this.vwUKClaimDef.WorkflowStatus, "Signed Copy By Director");
        }

        protected void lnk_COO_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen(this.vwUKClaimDef.WorkflowStatus, "Signed Copy By COO");
        }
        */

    }
}
