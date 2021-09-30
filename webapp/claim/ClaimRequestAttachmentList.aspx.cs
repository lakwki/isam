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

namespace com.next.isam.webapp.claim
{
    public partial class ClaimRequestAttachmentList : com.next.isam.webapp.usercontrol.PageTemplate
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;

            int requestId = 0;
            QAIS.ClaimRequestDef def = null;

            if (!Page.IsPostBack)
                requestId = int.Parse(EncryptionUtility.DecryptParam(Request.Params["requestId"].ToString()));
            else
                requestId = this.vwClaimRequestDef.RequestId;

            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

            def = svc.GetClaimRequestByKey(requestId);
            this.vwClaimRequestDef = def;

            if (!Page.IsPostBack) this.bindRecord();
        }

        private void bindRecord()
        {
            ArrayList queryStructs = new ArrayList();

            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

            QAIS.ClaimRequestDef def = this.vwClaimRequestDef;
                    
            if (def.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                this.lblRequestNo.Text = def.ClaimType.ToString() + " [" + def.ClaimMonth + "]";
            else
                this.lblRequestNo.Text = def.ClaimType.ToString() + " [" + def.ItemNo + "]";
            this.lblRequestDate.Text = DateTimeUtility.getDateString(def.IssueDate);
            this.lblStatus.Text = def.WorkflowStatus.Name;

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Form No", def.FormNo));
            queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
            queryStructs.Add(new QueryStructDef("Claim Type", UKClaimType.getType(def.ClaimType.ToString()).DMSDescription));
            if (def.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                queryStructs.Add(new QueryStructDef("MFRN Month", def.ClaimMonth));

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

        private QAIS.ClaimRequestDef vwClaimRequestDef
        {
            get
            {
                return (QAIS.ClaimRequestDef)ViewState["vwClaimRequestDef"];
            }
            set
            {
                ViewState["vwClaimRequestDef"] = value;
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
                ((LinkButton)e.Row.FindControl("lnk_FileName")).Text = def.FileName;
                ((LinkButton)e.Row.FindControl("lnk_FileName")).CommandArgument = e.Row.RowIndex.ToString();
                ((Label)e.Row.FindControl("lbl_Type")).Text = def.Description.Split(',')[0]; 
                ((Label)e.Row.FindControl("lbl_UploadDate")).Text = def.LastModifyDate.ToString("dd/MM/yyyy HH:mm");
                ((Label)e.Row.FindControl("lbl_MajorId")).Text = def.MajorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_MinorId")).Text = def.MinorVerion.ToString();
                ((Label)e.Row.FindControl("lbl_BuildId")).Text = def.Build.ToString();
                
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
        }



    }
}
