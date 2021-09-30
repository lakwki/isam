using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using com.next.infra.web;

namespace com.next.isam.webapp.usercontrol
{
    public partial class UclDMSAttachmentDataGrid : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string DocumentType
        {
			get
			{
				if (ViewState["DMSDocumentType"] != null)
					return (string) ViewState["DMSDocumentType"];
				else
					return string.Empty;
			}
			set { ViewState["DMSDocumentType"] = value; }
        }

        public void AddQueryItem(string command, string value)
        {
            List<QueryStructDef> list = this.QueryList;
            list.Add(new QueryStructDef(command, value));
            this.QueryList = list;
        }

        private List<QueryStructDef> QueryList
        {
			get
			{
				if (ViewState["DMSQueryList"] != null)
					return (List<QueryStructDef>) ViewState["DMSQueryList"];
				else
					return new List<QueryStructDef>();
			}
			set { ViewState["DMSQueryList"] = value; }
        }

        public void BindGrid()
        {
            if (this.DocumentType.Trim() == string.Empty || this.QueryList.Count == 0)
                throw new Exception("Missing input parameter(s)");

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", this.DocumentType.Trim()));
            foreach(QueryStructDef def in this.QueryList)
                queryStructs.Add(new QueryStructDef(def.Command, def.Value));
                
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
                ((Label)e.Row.FindControl("lbl_FileName")).Text = def.FileName;
                ((Label)e.Row.FindControl("lbl_FileName")).Visible = false;
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

        private void forwardToScreen(string nextScreen)
        {
            Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, nextScreen);
            new HttpHandler().ProcessRequest(Context);
        }

    }
}