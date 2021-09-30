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
    public partial class UKDiscountClaimAttachmentList : com.next.isam.webapp.usercontrol.PageTemplate
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            int claimId = 0;

            if (!Page.IsPostBack)
                claimId = int.Parse(EncryptionUtility.DecryptParam(Request.Params["claimId"].ToString()));
            else
                claimId = this.vwUKDiscountClaimDef.ClaimId;

            this.vwUKDiscountClaimDef = UKClaimManager.Instance.getUKDiscountClaimByKey(claimId);


            if (!Page.IsPostBack)
            {
                this.bindRecord();
            }
        }

        private void bindRecord()
        {

            if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.UpdateNextClaim))
            {
                this.lnkOtherDoc.Visible = false;
            }

            UKDiscountClaimDef ukClaimDef = this.vwUKDiscountClaimDef;

            this.lblUKDNNo.Text = (ukClaimDef.UKDebitNoteNo == string.Empty ? "N/A" : ukClaimDef.UKDebitNoteNo);
            this.lblStatus.Text = ukClaimDef.WorkflowStatus.Name;

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", "UK Discount"));
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

            this.vwAttachmentList = attachmentInfoList;
            this.gvAttachment.DataSource = attachmentInfoList;
            this.gvAttachment.DataBind();
        }

        private UKDiscountClaimDef vwUKDiscountClaimDef
        {
            get { return (UKDiscountClaimDef)ViewState["vwUKDiscountClaimDef"]; }
            set { ViewState["vwUKDiscountClaimDef"] = value; }
        }

        private ArrayList vwAttachmentList
        {
            set {  ViewState["AttachmentList"] = value; }
            get { return (ArrayList)ViewState["AttachmentList"]; }
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

                ((Label)e.Row.FindControl("lbl_FileName")).Visible = false;

                ((Label)e.Row.FindControl("lbl_Type")).Text = def.Description.Split(',')[0]; 
                ((Label)e.Row.FindControl("lbl_UploadDate")).Text = def.LastModifyDate.ToString("dd/MM/yyyy HH:mm");
                ((Label)e.Row.FindControl("lbl_MajorId")).Text = def.MajorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_MinorId")).Text = def.MinorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_BuildId")).Text = def.Build.ToString();
                ((ImageButton)e.Row.FindControl("imgEdit")).CommandArgument = e.Row.RowIndex.ToString();
                ((ImageButton)e.Row.FindControl("imgEdit")).Visible = true;
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
                Context.Items.Add(AccountCommander.Param.claimId, this.vwUKDiscountClaimDef.ClaimId.ToString());
                forwardToScreen("UKClaim.UKDiscountUpload");
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

                NoticeHelper.sendUKDiscountClaimMailArchive(this.vwUKDiscountClaimDef, docType, outputFolder + attDef.FileName, this.LogonUserId);
                FileUtility.clearFolder(outputFolder, false);

                forwardToScreen(null);
            }
        }

        private void openAddDocumentScreen(string docType)
        {
            Context.Items.Clear();
            Context.Items.Add(AccountCommander.Param.toClaimStatusId, this.vwUKDiscountClaimDef.WorkflowStatusId);
            Context.Items.Add(AccountCommander.Param.docId, "0");
            Context.Items.Add(AccountCommander.Param.docType, docType);
            Context.Items.Add(AccountCommander.Param.claimId, this.vwUKDiscountClaimDef.ClaimId.ToString());
            forwardToScreen("UKClaim.UKDiscountUpload");
        }


        protected void lnkOtherDoc_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen("Other Supporting Doc");
        }

        protected void lnkRefund_Click(object sender, EventArgs e)
        {
            openAddDocumentScreen("Next Claim Refund Supporting Doc");
        }

    }
}
