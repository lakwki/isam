using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using com.next.common.web.commander;
using com.next.common.domain.dms;
using com.next.isam.domain.order;
using com.next.isam.appserver.shipping;
using com.next.isam.dataserver.worker;
using com.next.infra.util;

namespace com.next.isam.webapp.shipping
{
    public partial class AttachmentList : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int shipmentId = 0;
                InvoiceDef invoiceDef = null;
                ShipmentDef shipmentDef = null;
                ContractDef contractDef = null;

                shipmentId = int.Parse(EncryptionUtility.DecryptParam(Request.Params["shipmentId"].ToString()));
                ViewState["shipmentId"] = shipmentId.ToString();

                invoiceDef = ShipmentManager.Instance.getInvoiceByShipmentId(shipmentId);
                shipmentDef = ShipmentManager.Instance.getShipmentById(shipmentId);
                contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
                this.lblInvoiceNo.Text = ShipmentManager.getInvoiceNo(invoiceDef.InvoicePrefix, invoiceDef.InvoiceSeqNo, invoiceDef.InvoiceYear);
                this.lblContractNo.Text = contractDef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString();

                ArrayList queryStructs = new ArrayList();

                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Shipping - NS Invoice"));
                queryStructs.Add(new QueryStructDef("NS Invoice no.", invoiceDef.InvoiceNo));
                queryStructs.Add(new QueryStructDef("Customer Name", "S&B"));

                ArrayList qList = DMSUtil.queryDocument(queryStructs);
                ArrayList attachmentInfoList = new ArrayList();

                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    ViewState["docId"] = docInfoDef.DocumentID.ToString();
                    ArrayList versionList = DMSUtil.queryDocumentByIDWithHistories(docInfoDef.DocumentID);
                    foreach (DocumentInfoDef docInfoVersionDef in versionList)
                    {
                        if (docInfoVersionDef.UpdatedUser != "DMSAPI")
                        {
                            foreach (AttachmentInfoDef attDef in docInfoVersionDef.AttachmentInfos)
                            {
                                /*
                                if (attDef.FileName.ToUpper().IndexOf(contractDef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString()) != -1)
                                    attachmentInfoList.Add(attDef);
                                */
                                if (attDef.FileName.IndexOf(".html") == -1)
                                    attachmentInfoList.Add(attDef);
                            }
                        }
                    }
                }

                this.vwAttachmentList = attachmentInfoList;
                this.gvAttachment.DataSource = attachmentInfoList;
                this.gvAttachment.DataBind();
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
                ((ImageButton)e.Row.FindControl("imgMail")).CommandArgument = e.Row.RowIndex.ToString();
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
        }
    }
}