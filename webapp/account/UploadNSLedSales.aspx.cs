using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.util;
using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.account;
using com.next.isam.appserver.common;
using com.next.isam.domain.account;
using com.next.isam.domain.common;
using com.next.common.web.commander;
using System.IO;

namespace com.next.isam.webapp.account
{
    public partial class UploadNSLedSales : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                listUploadHistory();
            }
        }

        private ArrayList vwFileList
        {
            set { ViewState["SearchResult"] = value; }
            get { return (ArrayList)ViewState["SearchResult"]; }
        }

        private string vwUploadFileName
        {
            set { ViewState["UploadFileName"] = value; }
            get { return (string)ViewState["UploadFileName"]; }
        }
 

        protected void gv_History_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NSLedImportFileDef uploadRecord = (NSLedImportFileDef)vwFileList[e.Row.RowIndex];
                if (uploadRecord!=null)
                {
                    ((Label)e.Row.FindControl("lbl_FileName")).Text = uploadRecord.Filename ;
                    ((Label)e.Row.FindControl("lbl_Office")).Text = GeneralWorker.Instance.getOfficeRefByKey(uploadRecord.OfficeId).OfficeCode;
                    ((Label)e.Row.FindControl("lbl_UserName")).Text = GeneralWorker.Instance.getUserByKey(uploadRecord.CreatedBy).DisplayName;
                    ((Label)e.Row.FindControl("lbl_UploadTime")).Text = uploadRecord.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
        }


        protected void btnListHistory_Click(object sender, EventArgs e)
        {
            int uploadFileTypeId = UploadFileType.NSLedSales.FileTypeId;
            if (uploadFileTypeId == 0)
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadMsg", "alert('Please select the upload file type.');", true);
            else
                listUploadHistory();
        }


        protected void btnProcess_Click(object sender, EventArgs e)
        {
            //string customerType = this.ddl_CustomerType.SelectedValue;
            //if (customerType == "")
            //{
            //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadMsg", "alert('Please select the customer type.');", true);
            //    return;
            //}
            string serverFilePath = "";
            this.valFile.Enabled = true;
            string fileName = updFile.Value;
            if (fileName != "")
            {
                serverFilePath = copyUploadFileToServer(fileName);
                if(serverFilePath.StartsWith("Error: "))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadMsg", "alert('Fail to copy file to server.');", true);
                    return;
                }
                uploadExcelFile(serverFilePath);
                forwardToScreen(null);
                listUploadHistory();
            }
        }
    

        protected void listUploadHistory()
        {
            ArrayList list = AccountDataUploadManager.Instance.getNSLedUploadHistory(GeneralCriteria.ALL, DateTime.Now.AddMonths(-12), DateTime.MinValue);
            vwFileList = list;
            gv_History.DataSource = vwFileList;
            gv_History.DataBind();
        }


        private string copyUploadFileToServer(string clientFilePath)
        {
            string destinationPath = "";
            string serverFileName = "";
            string[] fileSubstring = clientFilePath.Split('\\');
            string uploadPath = WebConfig.getValue("appSettings", "NSLED_UPLOAD_FOLDER");
            string fileName = fileSubstring[fileSubstring.Length - 1].ToString();

            if (updFile.PostedFile != null)
            {
                try
                {
                    if (updFile.PostedFile.InputStream.Length > 0)
                    {
                        serverFileName = FileUploadManager.Instance.generateServerFileName(fileName, UploadFileType.NSLedSales.FileTypeId, this.LogonUserId);
                        destinationPath = uploadPath + serverFileName;
                        updFile.PostedFile.SaveAs(destinationPath);
                        FileUploadLogDef logDef = new FileUploadLogDef(FileUploadLogDef.Type.NSLedNSLedSalesDataUpload.GetHashCode(), serverFileName, destinationPath, CommonUtil.getUserByKey(this.LogonUserId));
                        FileUploadManager.Instance.updateFileUploadLog(logDef);
                    }
                }
                catch (Exception e)
                {
                    MailHelper.sendErrorAlert(e, "Fail to copy file '" + (string.IsNullOrEmpty(clientFilePath) ? "" : clientFilePath) + "'");
                    return "Error: " + e.Message;
                }
            }
            return destinationPath;
        }


        public int uploadExcelFile(string fileFullPath)
        {
            int status = 0;
            //string clientAction = "resetUploadStatus();";
            string clientAction = string.Empty;
            string notFoundItemNo = string.Empty;
            string errMsg = string.Empty;
            try
            {
                status = AccountDataUploadManager.Instance.getNSLedSalesFromExcel2020(fileFullPath, this.LogonUserId, out notFoundItemNo);
            }
            catch (Exception e)
            {
                status = -4;
                errMsg = e.Message;
                MailHelper.sendErrorAlert(e, "Fail to upload Excel file '" + (string.IsNullOrEmpty(fileFullPath) ? "" : fileFullPath) + "'");
            }
            switch (status)
            {
                case -1:    // invalid data format
                    clientAction += "alert('Upload failure : Invalid data format.');";
                    break;
                case -2:    // work sheet not found
                    clientAction += "alert('Upload failure : Worksheet not found.');";
                    break;
                case -3:    // invalid path
                    clientAction += "alert('Upload failure : Invalid File Path (" + (new FileInfo(fileFullPath)).Name + ").');";
                    break;
                case -4:    // Unexpect status
                    clientAction += "alert('Upload failure : Server cannot process your request. (" + errMsg + "');";
                    break;
                case -5:    // Duplicated Upload
                    clientAction += "alert('Upload failure : Duplicated Upload. Same Date, Office, Invoice and Type are Uploaded Before.');";
                    break;
                case -6:
                    clientAction += "alert('Upload failure : USD Exchange Rate not defined');";
                    break;
                case -7:
                    clientAction += "alert('Upload failure : Duplicate invoice no detected');";
                    break;
                case -8:
                    clientAction += String.Format("alert('Upload failure : RangePlan does not exist - {0}');", notFoundItemNo);
                    break;
                case -9:
                    clientAction += String.Format("alert('Upload failure : Actual freight USD unit cost is missing  - {0}');", notFoundItemNo);
                    break;
                case -10:
                    clientAction += String.Format("alert('Upload failure : Duty % is missing  - {0}');", notFoundItemNo);
                    break;
                case -11:
                    clientAction += String.Format("alert('Total line amount is not aligned with the cover sheet amount');", notFoundItemNo);
                    break;

                default:
                    if (status >= 0)
                    {
                        clientAction += "alert('Uploaded Sucessfully" + (status == 0 ? " (No Record)" : "") + "');";
                    }
                    else
                        clientAction += "alert('Upload fail');";
                    break;
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Upload Status", clientAction, true);

            return status;
        }

    }
}
