using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain.dms;
using com.next.isam.domain.nontrade;
//using com.next.isam.appserver.shipping;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.account;

using com.next.infra.util;
using System.IO;

namespace com.next.isam.webapp.nontrade
{
    public partial class AttachmentList : com.next.isam.webapp.usercontrol.PageTemplate
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                /*
                int shipmentId = 0;
                InvoiceDef invoiceDef = null;
                ShipmentDef shipmentDef = null;
                ContractDef contractDef = null;

                shipmentId = int.Parse(Request.Params["shipmentId"]);
                ViewState["shipmentId"] = shipmentId.ToString();
                
                invoiceDef = ShipmentManager.Instance.getInvoiceByShipmentId(shipmentId);
                shipmentDef = ShipmentManager.Instance.getShipmentById(shipmentId);
                contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
                */
                int invoiceId = int.Parse(Request.Params["invoiceId"]);
                ViewState["invoiceId"] = invoiceId;

                NTInvoiceDef ntInvoice;
                ntInvoice = NonTradeManager.Instance.getNTInvoiceByKey(invoiceId);
                this.lbl_NSLInvoiceNo.Text = ntInvoice.NSLInvoiceNo;
                this.lbl_SupplierInvoiceNo.Text = ntInvoice.InvoiceNo;

                decimal amt = ntInvoice.Amount;
                this.lbl_Currency.Text = ntInvoice.Currency.CurrencyCode;
                this.lbl_Amount.Text = amt.ToString("#,###.00");

                //if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.DMSControlReview))
                //    this.lnkReview.Visible = false;

                //if (shipmentDef.ShippingDocWFS != ShippingDocWFS.REVIEWED && shipmentDef.ShippingDocWFS != ShippingDocWFS.ACCEPTED)
                //    this.lnkReview.Visible = false;

                //ArrayList queryStructs = new ArrayList();
                //queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non-Trade-Expense"));
                //queryStructs.Add(new QueryStructDef("NSL Invoice No", def.NSLInvoiceNo));
                //ArrayList qList = DMSUtil.queryDocument(queryStructs);
                //ImageButton btn= (ImageButton)e.Row.FindControl("img_OpenDoc");

                ArrayList attachmentInfoList = new ArrayList();
                ArrayList queryStructs = new ArrayList();
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non-Trade-Expense"));
                queryStructs.Add(new QueryStructDef("NSL Invoice No", ntInvoice.NSLInvoiceNo));
                ArrayList qList = DMSUtil.queryDocument(queryStructs);
                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    ViewState["docId"] = docInfoDef.DocumentID.ToString();
                    ArrayList versionList = DMSUtil.queryDocumentByIDWithHistories(docInfoDef.DocumentID);
                    foreach (DocumentInfoDef docInfoVersionDef in versionList)
                    {
                        if (docInfoVersionDef.UpdatedUser != "DMSAPI")
                        {
                            if (docInfoVersionDef.AttachmentInfos.Count > 0)
                            {
                                AttachmentInfoDef attDef = (AttachmentInfoDef)(docInfoVersionDef.AttachmentInfos[0]);
                                if (attDef.FileName.IndexOf(".html") == -1)
                                    attachmentInfoList.Add(attDef);
                            }
                            //foreach (AttachmentInfoDef attDef in docInfoVersionDef.AttachmentInfos)
                            //{
                            //    if (attDef.FileName.IndexOf(".html") == -1)
                            //        attachmentInfoList.Add(attDef);
                            //}
                        }
                    }
                }

                this.vwAttachmentList = attachmentInfoList;
                this.gvAttachment.DataSource = attachmentInfoList;
                this.gvAttachment.DataBind();
                //this.showReviewInfo();

                //if (qList.Count == 0) this.lnkReview.Visible = false;
            }
        }

        private ArrayList vwAttachmentList
        {
            set
            {
                ViewState["AttachmentList"] = value;
            }
            get
            {
                return (ArrayList)ViewState["AttachmentList"];
            }
        }

        /*
        private void showReviewInfo()
        {
            string version = String.Empty;

            if (gvAttachment.Rows.Count > 0)
            {
                int shipmentId = int.Parse(ViewState["shipmentId"].ToString());
                ShipmentDef shipmentDef = OrderManager.Instance.getShipmentByKey(shipmentId);

                for (int i = gvAttachment.Rows.Count - 1; i >= 0; i--)
                {
                    GridViewRow r = gvAttachment.Rows[i];
                    AttachmentInfoDef def = (AttachmentInfoDef)this.vwAttachmentList[i];

                    if (shipmentDef.DocumentReviewedOn != DateTime.MinValue && def.LastModifyDate <= shipmentDef.DocumentReviewedOn && (version == String.Empty || version == def.Version))
                    {
                        ((Label)r.FindControl("lbl_Review")).Text = "Reviewed By " + CommonUtil.getUserByKey(shipmentDef.DocumentReviewedBy).DisplayName + " on " + shipmentDef.DocumentReviewedOn.ToString("dd/MM/yyyy hh:mm");
                        version = def.Version;
                    }

                }
            }
        }
        */

        protected void gvAttachment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                AttachmentInfoDef def = (AttachmentInfoDef)this.vwAttachmentList[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_FileName")).Text = def.FileName;
                ((Label)e.Row.FindControl("lbl_UploadDate")).Text = def.LastModifyDate.ToString("dd/MM/yyyy HH:mm");
                ((Label)e.Row.FindControl("lbl_MajorId")).Text = def.MajorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_MinorId")).Text = def.MinorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_BuildId")).Text = def.Build.ToString();
                ((ImageButton)e.Row.FindControl("imgOpen")).CommandArgument = e.Row.RowIndex.ToString();
                //((ImageButton)e.Row.FindControl("imgMail")).CommandArgument = e.Row.RowIndex.ToString();
            }
        }

        protected void gvAttachment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            AttachmentInfoDef def = (AttachmentInfoDef)this.vwAttachmentList[int.Parse((string)e.CommandArgument)];
            if (e.CommandName == "OpenAttachment")
            {
                Context.Items.Clear();

                Context.Items.Add("docId", ViewState["docId"]);
                Context.Items.Add("attId", def.AttachmentID.ToString());
                Context.Items.Add("majorId", def.MajorVerion.ToString());
                Context.Items.Add("minorId", def.MinorVerion.ToString());
                Context.Items.Add("buildId", def.Build.ToString());
                forwardToScreen("dms.Attachment");
            }
            else if (e.CommandName == "MailDoc")
            {
                Context.Items.Clear();
                string[] s = def.Description.Split(',');
                Context.Items.Add("docId", ViewState["docId"]);
                Context.Items.Add("attId", def.AttachmentID.ToString());
                Context.Items.Add("majorId", def.MajorVerion.ToString());
                Context.Items.Add("minorId", def.MinorVerion.ToString());
                Context.Items.Add("buildId", def.Build.ToString());

                string docType = "Supplier Document";

                DocumentInfoDef docInfoDef = DMSUtil.queryDocumentByID(long.Parse(ViewState["docId"].ToString()), def.MajorVerion, def.MinorVerion, def.Build);
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

                NoticeHelper.sendSupplierDocMailArchive(this.lbl_NSLInvoiceNo.Text, docType, outputFolder + attDef.FileName, this.LogonUserId);
                FileUtility.clearFolder(outputFolder, false);

                forwardToScreen(null);
            }

        }

        /*
        protected void lnkReview_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.MarkDocumentAsReviewed);

            Context.Items.Add(AccountCommander.Param.shipmentId, int.Parse((ViewState["shipmentId"]).ToString()));

            forwardToScreen(null);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AttachmentList", "alert('Record Updated');", true);
        }
        */ 
    }
}
