using System;
using System.IO;
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
using com.next.common.domain.module;
using com.next.common.domain.dms;
using com.next.infra.util;
namespace com.next.isam.webapp.account
{
    public partial class GenLogoXMLFilePopup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int requestId = 0;
                if (Request.Params["requestId"] != "-1")
                    requestId = Convert.ToInt32(Request.Params["requestId"]);
                else
                {
                    lbl_error_msg.Visible = true;
                    lbl_error_msg.Text = "File Not Found!";
                    return;
                }

                ArrayList queryStructs = new ArrayList();
                DocumentInfoDef doc = new DocumentInfoDef();
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "LOGO Interface"));
                queryStructs.Add(new QueryStructDef("Request Id", requestId.ToString()));
                ArrayList qList = DMSUtil.queryDocument(queryStructs);
                doc = (DocumentInfoDef)qList[0];
                if (doc.AttachmentInfos.Count > 0)
                {
                    AttachmentInfoDef attDef = (AttachmentInfoDef)doc.AttachmentInfos[0];

                    Context.Items.Clear();
                    string[] s = attDef.Description.Split('|');
                    Context.Items.Add("docId", doc.DocumentID);
                    Context.Items.Add("attId", attDef.AttachmentID.ToString());
                    Context.Items.Add("majorId", attDef.MajorVerion.ToString());
                    Context.Items.Add("minorId", attDef.MinorVerion.ToString());
                    Context.Items.Add("buildId", attDef.Build.ToString());
                }

                long attachmentId;
                long docId;
                long majorId;
                long minorId;
                long buildId;

                if (Context.Request.Params["attId"] != null)
                {
                    attachmentId = long.Parse(EncryptionUtility.DecryptParam(Context.Request.Params["attId"].ToString()));
                    docId = long.Parse(EncryptionUtility.DecryptParam(Context.Request.Params["docId"].ToString()));
                    majorId = long.Parse(EncryptionUtility.DecryptParam(Context.Request.Params["majorId"].ToString()));
                    minorId = long.Parse(EncryptionUtility.DecryptParam(Context.Request.Params["minorId"].ToString()));
                    buildId = long.Parse(EncryptionUtility.DecryptParam(Context.Request.Params["buildId"].ToString()));
                }
                else
                {
                    attachmentId = long.Parse(Context.Items["attId"].ToString());
                    docId = long.Parse(Context.Items["docId"].ToString());
                    majorId = long.Parse(Context.Items["majorId"].ToString());
                    minorId = long.Parse(Context.Items["minorId"].ToString());
                    buildId = long.Parse(Context.Items["buildId"].ToString());
                }
                DocumentInfoDef docInfoDef = DMSUtil.queryDocumentByID(docId, majorId, minorId, buildId);
                AttachmentInfoDef def = null;

                foreach (AttachmentInfoDef attachInfoDef in docInfoDef.AttachmentInfos)
                {
                    if (attachInfoDef.AttachmentID == attachmentId)
                        def = attachInfoDef;
                }

                WebHelper.outputFileAsHttpRespone(Response, def.FileName, DMSUtil.getAttachment(def.AttachmentID));
            }
        }
    }
}