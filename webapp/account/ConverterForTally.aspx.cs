using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using com.next.isam.webapp.commander;
using com.next.infra.web;
using com.next.infra.util;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.account;
using com.next.common.datafactory.worker;
using com.next.isam.appserver.common;
using com.next.isam.appserver.helper;
using com.next.isam.domain.account;


namespace com.next.isam.webapp.account
{
    public partial class ConverterForTally : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                listUploadHistory();
            }

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
 
        protected void gv_History_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TallyUploadHistoryDef uploadRecord = (TallyUploadHistoryDef)vwFileNameList[e.Row.RowIndex];
                if (uploadRecord!=null)
                {
                    ((Label)e.Row.FindControl("lbl_FileName")).Text = uploadRecord.UploadFileName;
                    if (uploadRecord.SequenceNoStart == uploadRecord.SequenceNoEnd)
                        ((Label)e.Row.FindControl("lbl_Sequence")).Text = uploadRecord.SequenceNoStart.ToString();
                    else
                        ((Label)e.Row.FindControl("lbl_Sequence")).Text = uploadRecord.SequenceNoStart.ToString() + " - " + uploadRecord.SequenceNoEnd.ToString();
                    ((Label)e.Row.FindControl("lbl_UserName")).Text = GeneralWorker.Instance.getUserByKey(uploadRecord.CreatedBy).DisplayName;
                    ((Label)e.Row.FindControl("lbl_UploadTime")).Text = uploadRecord.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
        }

        protected void btnListHistory_Click(object sender, EventArgs e)
        {
            int uploadFileTypeId = UploadFileType.TallyInterfaceFile.FileTypeId;
            if (uploadFileTypeId == 0)
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadMsg", "alert('Please select the upload file type.');", true);
            else
            {
                listUploadHistory();
            }
        }

        protected void downloadXmlToClient(string xml, string fileNameWithoutExtension)
        {
            this.EnableViewState = false;
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "text/xml";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + fileNameWithoutExtension + ".xml");
            Response.Charset = "";
            Response.Write(xml);
            Response.Flush();
            //Response.End();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();

        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            string serverFilePath = "";

            this.valFile.Enabled = true;
            int uploadFileTypeId = UploadFileType.TallyInterfaceFile.FileTypeId;
            if (uploadFileTypeId == 0)
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadMsg", "alert('Please select the upload file type.');", true);
            else
                if (updFile.Value != String.Empty)
                {
                    serverFilePath = copyUploadFileToServer(updFile.Value, uploadFileTypeId);
                    convertAndDownloadFile(serverFilePath);
                    forwardToScreen(null);
                    listUploadHistory();
                    
                }
        }

        protected void listUploadHistory()
        {
            string uploadPath = WebConfig.getValue("appSettings", "TALLY_INTERFACE_FOLDER");

            ArrayList nameList = AccountDataUploadManager.Instance.getRecentTallyUploadHistory();

            vwFileNameList = nameList;
            gv_History.DataSource = vwFileNameList;
            gv_History.DataBind();
        }

        private string copyUploadFileToServer(string clientFilePath, int uploadFileTypeId)
        {
            string destinationPath = "";
            string serverFileName = "";
            string[] fileSubstring = clientFilePath.Split('\\');
            string uploadPath = WebConfig.getValue("appSettings", "TALLY_INTERFACE_FOLDER");
            string fileName = fileSubstring[fileSubstring.Length - 1].ToString();

            if (updFile.PostedFile != null)
            {
                try
                {
                    if (updFile.PostedFile.InputStream.Length > 0)
                    {
                        serverFileName = FileUploadManager.Instance.generateServerFileName(fileName, UploadFileType.TallyInterfaceFile.FileTypeId, this.LogonUserId);
                        destinationPath = uploadPath + serverFileName;
                        updFile.PostedFile.SaveAs(destinationPath);
                    }
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return destinationPath;
        }

        #region Conversion routine

        public string convertAndDownloadFile(string fileFullPath)
        {
            UploadFileRef uploadFile;
            string xml = string.Empty;
            try
            {
                string serverFileName = fileFullPath.Substring(fileFullPath.LastIndexOf('\\') + 1);
                uploadFile = AccountDataUploadManager.Instance.getUploadFileInfo(serverFileName);
                if (uploadFile.FileType == UploadFileType.TallyInterfaceFile && uploadFile.FileName != string.Empty && uploadFile.UploadUser != null)
                {
                    string defaultFileName = uploadFile.FileName;
                    if (defaultFileName.LastIndexOf('.') > 0)
                        defaultFileName = defaultFileName.Substring(0, defaultFileName.LastIndexOf('.'));

                    ArrayList journal = new ArrayList();
                    int status = AccountDataUploadManager.Instance.getSunAccountJournalInExcel(fileFullPath, this.LogonUserId, journal);
                    switch (status)
                    {
                        case -1:    // Incorrect Data Format
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Conversion Status", "alert('Incorrect Data Format.');", true);
                            break;
                        case -2:   // Invalid Excel/work sheet
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Conversion Status", "alert('Invalid work sheet.');", true);
                            break;
                        case -3:   // Invalid Path
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Conversion Status", "alert('Invalid Path.');", true);
                            break;
                        case 0: // No Record found
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Conversion Status", "alert('No Record Found.');", true);
                            break;
                        default:
                            xml = AccountDataUploadManager.Instance.convertJournalToXML(journal, uploadFile.FileName, this.LogonUserId);
                            if (string.IsNullOrEmpty(xml))
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Conversion Status", "alert('Fail to convert file ' + '" + uploadFile.FileName + "');", true);
                            else
                            {
                                downloadXmlToClient(xml, defaultFileName);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, "");
            }
            return xml;
        }

        #endregion


    }
}
