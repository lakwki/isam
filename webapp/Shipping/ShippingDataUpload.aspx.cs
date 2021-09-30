using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

//using System.Data.OleDb;
using com.next.isam.webapp.commander;
using com.next.infra.web;
using com.next.infra.util;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.Shipping;
using com.next.common.datafactory.worker;
using com.next.common.web.commander;
using com.next.isam.appserver.common;
using com.next.isam.domain.common;
using com.next.common.domain.module;


namespace com.next.isam.webapp.shipping
{
    public partial class ShippingDataUpload : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private ArrayList vwFileNameList
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (ArrayList)ViewState["SearchResult"];
            }
        }
 
        protected void valFile_ServerValidate(object source, ServerValidateEventArgs args)
        { 
            if (this.lblFileName.Text == "")
            {
                ((CustomValidator)source).ErrorMessage = "The file does not exist or does not contain any data";
                args.IsValid = false;
            }
            else if (this.lblFileName.Text == "-1")
            {
                ((CustomValidator)source).ErrorMessage = "The file already exists in the server, please rename the file";
                args.IsValid = false;
            }
        }

        protected void gv_PendingJob_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FileUploadLogDef logDef = (FileUploadLogDef)e.Row.DataItem;

                ((Label)e.Row.FindControl("lbl_FileName")).Text = logDef.FileName.Substring(logDef.FileName.IndexOf("]") + 1).Trim();
                ((Label)e.Row.FindControl("lbl_UserName")).Text = logDef.SubmittedBy.DisplayName;
                ((Label)e.Row.FindControl("lbl_UploadTime")).Text = logDef.SubmittedOn.ToString("dd/MM/yyyy HH:mm:ss");

            }
        }
       
        protected void UploadFileRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            string uploadPath = WebConfig.getValue("appSettings", "SUPPLIER_INVOICE_NO_UPLOAD_FOLDER");
            FileUploadLogDef logDef = (FileUploadLogDef)vwFileNameList[arg.RowIndex];
            FileUploadManager.Instance.removeUploadFile(uploadPath + logDef.FileName);
            vwFileNameList.Remove(logDef);

            logDef.Status = 0;
            FileUploadManager.Instance.updateFileUploadLog(logDef);

            int uploadFileTypeId = int.Parse(this.ddl_FileType.SelectedValue);
            listPendingJob(uploadFileTypeId);
        }
 
        protected void btnListPending_Click(object sender, EventArgs e)
        {
            int uploadFileTypeId = int.Parse(this.ddl_FileType.SelectedValue);
            if (uploadFileTypeId == 0)
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadMsg", "alert('Please select the upload file type.');", true);
            else
            {
                listPendingJob(uploadFileTypeId);
            }
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            string serverFilePath = "";

            this.valFile.Enabled = true;

            int uploadFileTypeId = int.Parse(this.ddl_FileType.SelectedValue);
            if (uploadFileTypeId == 0)
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadMsg", "alert('Please select the upload file type.');", true);
            else
                if (updFile.Value != String.Empty)
                {
                    serverFilePath = copyUploadFileToServer(updFile.Value, uploadFileTypeId);
                    listPendingJob(uploadFileTypeId);
                }
        }

        protected void listPendingJob(int uploadFileTypeId)
        {
            string uploadPath = WebConfig.getValue("appSettings", "SUPPLIER_INVOICE_NO_UPLOAD_FOLDER");
            //ArrayList nameList = FileUploadManager.Instance.getServerFileList(uploadPath, uploadFileTypeId);
            ArrayList fileList = null;
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shippingDataUpload.Id, ISAMModule.shippingDataUpload.SuperUser))
                fileList = FileUploadManager.Instance.getFileUploadLogByCriteria(FileUploadLogDef.Type.ShippingDataUpload.GetHashCode(), string.Empty, -1);
            else
                fileList = FileUploadManager.Instance.getFileUploadLogByCriteria(FileUploadLogDef.Type.ShippingDataUpload.GetHashCode(), string.Empty, this.LogonUserId);

            vwFileNameList = fileList;
            gv_PendingJob.DataSource = vwFileNameList;
            gv_PendingJob.DataBind();

        }
        
        private string copyUploadFileToServer(string clientFilePath, int uploadFileTypeId)
        {
            string destinationPath = "";
            string serverFileName = "";
            string[] fileSubstring = clientFilePath.Split('\\');
            string uploadPath = WebConfig.getValue("appSettings", "SUPPLIER_INVOICE_NO_UPLOAD_FOLDER");
            string fileName = fileSubstring[fileSubstring.Length - 1].ToString();

            if (updFile.PostedFile != null)
            {
                try
                {
                    if (updFile.PostedFile.InputStream.Length > 0)
                    {
                        serverFileName = FileUploadManager.Instance.generateServerFileName(fileName, uploadFileTypeId, this.LogonUserId);
                        destinationPath = uploadPath + serverFileName;
                        updFile.PostedFile.SaveAs(destinationPath);
                        FileUploadLogDef logDef = new FileUploadLogDef(FileUploadLogDef.Type.ShippingDataUpload.GetHashCode(), serverFileName, destinationPath, CommonUtil.getUserByKey(this.LogonUserId));
                        FileUploadManager.Instance.updateFileUploadLog(logDef);
                    }
                    this.lblFileName.Text = destinationPath;
                }
                catch (Exception e)
                {
                    this.lblFileName.Text = "-1";
                    return e.Message;
                }
            }
            return destinationPath;
        }


    }
}
