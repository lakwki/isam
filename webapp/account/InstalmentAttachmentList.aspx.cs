using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.account;
using com.next.infra.util;
using com.next.isam.appserver.account;
using System.Collections;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using System.IO;
using com.next.isam.dataserver.worker;

namespace com.next.isam.webapp.account
{
    public partial class InstalmentAttachmentList : com.next.isam.webapp.usercontrol.PageTemplate
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            int paymentId = 1;
            if (!Page.IsPostBack)
            {
                if (Request.Params["PaymentId"] != null)
                {
                    paymentId = int.Parse(Request.Params["PaymentId"].ToString());
                }

                this.vwAdvancePayment = AccountManager.Instance.getAdvancePaymentByKey(paymentId);
                this.bindRecord();
                //if (gvAttachment.Rows.Count != 0) { lnkInstalmentDoc.Visible = false; }
            }
            else
                paymentId = this.vwAdvancePayment.PaymentId;


        }

        private void bindRecord()
        {
            AdvancePaymentDef advPayment = this.vwAdvancePayment;

            this.lblPaymentNo.Text = (vwAdvancePayment.PaymentNo == string.Empty ? "N/A" : vwAdvancePayment.PaymentNo);
            this.lblSupplierName.Text = vwAdvancePayment.Vendor.Name;

            ArrayList queryStructs = new ArrayList();

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Advance Payment"));
            queryStructs.Add(new QueryStructDef("Payment No", advPayment.PaymentNo));
            //queryStructs.Add(new QueryStructDef("Supplier Name", advPayment.Vendor.Name));

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            ArrayList attachmentInfoList = new ArrayList();

            foreach (DocumentInfoDef docInfoDef in qList)
            {
                foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                {
                    FieldInfoDef fiDef = (FieldInfoDef)docInfoDef.FieldInfos[1];
                    if (attDef.FileName.IndexOf(".html") == -1)
                        attDef.Description =docInfoDef.DocumentID.ToString();
                        attachmentInfoList.Add(attDef);
                }
            }

            this.vwAttachmentList = attachmentInfoList;
            this.gvAttachment.DataSource = attachmentInfoList;
            this.gvAttachment.DataBind();
        }

        protected void gvAttachment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AttachmentInfoDef def = (AttachmentInfoDef)this.vwAttachmentList[e.Row.RowIndex];
                ((LinkButton)e.Row.FindControl("lnk_FileName")).Text = def.FileName;
                ((LinkButton)e.Row.FindControl("lnk_FileName")).CommandArgument = e.Row.RowIndex.ToString();
                ((ImageButton)e.Row.FindControl("imgMail")).CommandArgument = e.Row.RowIndex.ToString();
                ((Label)e.Row.FindControl("lbl_FileName")).Text = def.FileName;

                ((Label)e.Row.FindControl("lbl_FileName")).Visible = false;
                /*
                ((LinkButton)e.Row.FindControl("lnk_FileName")).Visible = isEditable;
                ((Label)e.Row.FindControl("lbl_FileName")).Visible = !isEditable;
                */
                //((Label)e.Row.FindControl("lbl_Type")).Text = "Advance Payment";
                ((Label)e.Row.FindControl("lbl_UploadDate")).Text = def.LastModifyDate.ToString("dd/MM/yyyy HH:mm");
                /*((Label)e.Row.FindControl("lbl_MajorId")).Text = def.MajorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_MinorId")).Text = def.MinorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_BuildId")).Text = def.Build.ToString();*/
                ((ImageButton)e.Row.FindControl("imgEdit")).CommandArgument = e.Row.RowIndex.ToString();
                ((ImageButton)e.Row.FindControl("imgRemove")).CommandArgument = e.Row.RowIndex.ToString();
            }
        }

        protected void gvAttachment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            AttachmentInfoDef def = (AttachmentInfoDef)this.vwAttachmentList[int.Parse((string)e.CommandArgument)];

            if (e.CommandName == "OpenAttachment")
            {
                Context.Items.Clear();
                Context.Items.Add("docId", def.Description);
                Context.Items.Add("attId", def.AttachmentID.ToString());
                Context.Items.Add("majorId", def.MajorVerion.ToString());
                Context.Items.Add("minorId", def.MinorVerion.ToString());
                Context.Items.Add("buildId", def.Build.ToString());
                forwardToScreen("dms.Attachment");
            }
            else if (e.CommandName == "ReviseDoc")
            {
                Context.Items.Clear();
                Context.Items.Add(AccountCommander.Param.paymentId, vwAdvancePayment.PaymentId);
                Context.Items.Add(AccountCommander.Param.fileName, def.FileName);
                Context.Items.Add(AccountCommander.Param.docId, def.Description);
                Context.Items.Add(AccountCommander.Param.docType, "Advance Payment");
               
                forwardToScreen("InstalmentDoc.Upload");
            }
            else if (e.CommandName == "MailDoc")
            {
                DocumentInfoDef docInfoDef = DMSUtil.queryDocumentByID(long.Parse(def.Description), def.MajorVerion, def.MinorVerion, def.Build);
                AttachmentInfoDef attDef = null;

                foreach (AttachmentInfoDef attachInfoDef in docInfoDef.AttachmentInfos)
                {
                    if (attachInfoDef.AttachmentID == def.AttachmentID)
                        attDef = attachInfoDef;
                }

                string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputFolder = WebConfig.getValue("appSettings", "INSTALMENT_DOC_FOLDER") + tsFolderName + "\\";
                if (!System.IO.Directory.Exists(outputFolder))
                    System.IO.Directory.CreateDirectory(outputFolder);

                System.Byte[] byteDownload = DMSUtil.getAttachment(attDef.AttachmentID);
                FileStream fs = new System.IO.FileStream(outputFolder + attDef.FileName, FileMode.Create, FileAccess.Write);
                fs.Write(byteDownload, 0, byteDownload.Length);
                fs.Close();

                NoticeHelper.sendInstalmentDocMailArchive(this.vwAdvancePayment, "Advance Payment", outputFolder + attDef.FileName, this.LogonUserId);
                FileUtility.clearFolder(outputFolder, false);

                forwardToScreen(null);
            }
            else if (e.CommandName == "RemoveDoc") 
            {
                ArrayList queryStructs = new ArrayList();

                DocumentInfoDef docInfoDef = DMSUtil.queryDocumentByID(int.Parse(def.Description));

                foreach (FieldInfoDef fiDef in docInfoDef.FieldInfos)
                    queryStructs.Add(new QueryStructDef(fiDef.FieldName, fiDef.Content));

                DMSUtil.DeleteSingleAttachment(int.Parse(def.Description), queryStructs, def.FileName);

                AdvancePaymentActionHistoryDef historyDef = new AdvancePaymentActionHistoryDef();
                historyDef.PaymentId = vwAdvancePayment.PaymentId;
                historyDef.Description = "ADVANCE PAYMENT SUPPORTING DOCUMENT [" + vwAdvancePayment.PaymentNo + "]";
                historyDef.Description += " REMOVE (" + def.Description + ")";
                historyDef.ActionBy = this.LogonUserId;
                historyDef.ActionOn = DateTime.Now;
                historyDef.Status = 1;
                AccountManager.Instance.updateAdvancePaymentActionHistory(historyDef);

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='InstalmentAttachmentList.aspx?PaymentId=" + vwAdvancePayment.PaymentId.ToString() + "';", true);
            }
        }

        protected void lnkInstalmentDoc_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(AccountCommander.Param.paymentId, vwAdvancePayment.PaymentId);
            Context.Items.Add(AccountCommander.Param.docId, "0");
            Context.Items.Add(AccountCommander.Param.docType, "Advance Payment");

            forwardToScreen("InstalmentDoc.Upload");
        }

        private AdvancePaymentDef vwAdvancePayment
        {
            get { return (AdvancePaymentDef)ViewState["vwAdvancePayment"]; }
            set { ViewState["vwAdvancePayment"] = value; }
        }

        private ArrayList vwAttachmentList
        {
            set { ViewState["vwAttachmentList"] = value; }
            get { return (ArrayList)ViewState["vwAttachmentList"]; }
        }

    }
}