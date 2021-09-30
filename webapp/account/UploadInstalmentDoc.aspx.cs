using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.util;
using com.next.isam.webapp.commander.account;
using System.Collections;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using System.IO;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.account;
using com.next.isam.appserver.account;

namespace com.next.isam.webapp.account
{
    public partial class UploadInstalmentDoc : com.next.isam.webapp.usercontrol.PageTemplate
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ViewState["PaymentId"] = Context.Items[AccountCommander.Param.paymentId].ToString();
                ViewState["DocId"] = (Context.Items[AccountCommander.Param.docId]).ToString();
                ViewState["DocType"] = (Context.Items[AccountCommander.Param.docType]).ToString();
                if (Context.Items[AccountCommander.Param.fileName] != null)
                    ViewState["FileName"] = Context.Items[AccountCommander.Param.fileName].ToString();

                vwAdvancePayment = AccountManager.Instance.getAdvancePaymentByKey(int.Parse(ViewState["PaymentId"].ToString()));

                lbl_PaymentNo.Text = vwAdvancePayment.PaymentNo;

                lbl_SupplierName.Text = vwAdvancePayment.Vendor.Name;

                if (int.Parse(ViewState["DocId"].ToString()) == 0)
                {
                    this.lblMode.Text = "[Add]";
                    btnRemove.Visible = false;
                }
                else
                {
                    if (int.Parse(ViewState["DocAction"].ToString()) == 1)
                    {
                        this.lblMode.Text = "[Revise]";
                    }
                    else
                    {
                        this.lblMode.Text = "[Remove]";
                    }
                }
                this.lblMode.Text += "Advance Payment Supporting Document";

                ProgressBar.showDMS(this.btnSave);
            }
        }

        private AdvancePaymentDef vwAdvancePayment
        {
            get
            {
                return (AdvancePaymentDef)ViewState["vwAdvancePayment"];
            }
            set
            {
                ViewState["vwAdvancePayment"] = value;
            }
        }

        private void reviseDocument()
        {
            string outputFolder = "";
            string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            outputFolder = WebConfig.getValue("appSettings", "INSTALMENT_DOC_FOLDER") + tsFolderName + "\\";
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

                insertAdvancePaymentHistory("REVISE", vwAdvancePayment, this.LogonUserId, docInfoDef.DocumentID.ToString());

            }
            FileUtility.clearFolder(outputFolder, false);

            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='attachmentlist.aspx?claimid=" + this.vwUKClaimDef.ClaimId.ToString() + (ViewState["IsReviewed"].ToString() == "Y" ? "&rv=1" : string.Empty) + "';", true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='InstalmentAttachmentList.aspx?PaymentId=" + vwAdvancePayment.PaymentId.ToString() + "';", true);
        }


        protected void btnSave_Click(object sender, System.EventArgs e)
        {
            if (this.FileUpload1.HasFile)
            {
                if (int.Parse(ViewState["DocId"].ToString()) != 0)
                {
                        this.reviseDocument();
                }
                else
                {
                    string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string outputFolder = WebConfig.getValue("appSettings", "INSTALMENT_DOC_FOLDER") + tsFolderName + "\\";
                    if (!System.IO.Directory.Exists(outputFolder))
                        System.IO.Directory.CreateDirectory(outputFolder);

                    FileUpload fu = this.FileUpload1;
                    string filename = string.Empty;
                    if (fu.HasFile)
                    {
                        filename = Path.GetFileNameWithoutExtension(fu.FileName);

                        ArrayList queryStructs = new ArrayList();
                        queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Advance Payment"));
                        queryStructs.Add(new QueryStructDef("Payment No", vwAdvancePayment.PaymentNo));
                        /*
                        queryStructs.Add(new QueryStructDef("Supplier Name", vwAdvancePayment.Vendor.Name));
                        */
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
                            string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Account\\Advance Payment\\", this.vwAdvancePayment.PaymentNo + " Instalment Doc", "Advance Payment", queryStructs, attachmentList);
                        }

                        insertAdvancePaymentHistory("NEW", vwAdvancePayment, this.LogonUserId, attachmentList.Count.ToString());
                    }
                    FileUtility.clearFolder(outputFolder, false);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='InstalmentAttachmentList.aspx?PaymentId=" + vwAdvancePayment.PaymentId.ToString() + "';", true);
                }
            }
        }



        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadDoc", "window.location='InstalmentAttachmentList.aspx?PaymentId=" + vwAdvancePayment.PaymentId.ToString() + "';", true);
        }


        protected void insertAdvancePaymentHistory(string docStatus, AdvancePaymentDef advancePayment, int userId, string docId) 
        {
            AdvancePaymentActionHistoryDef historyDef = new AdvancePaymentActionHistoryDef();
            historyDef.PaymentId = advancePayment.PaymentId;
            historyDef.Description = "ADVANCE PAYMENT SUPPORTING DOCUMENT [" + advancePayment.PaymentNo + "]";
            if (docStatus == "NEW")
            {
                historyDef.Description += " ADD ("+ docId +")";
            }
            else if (docStatus == "REVISE")
            {
                historyDef.Description += " REVISE (DOCID:" + docId + ")";
            }
            historyDef.ActionBy = userId;
            historyDef.ActionOn = DateTime.Now;
            historyDef.Status = 1;
            AccountManager.Instance.updateAdvancePaymentActionHistory(historyDef);
        }
    }
}
