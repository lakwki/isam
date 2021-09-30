using System;
using System.Collections;
using System.Web.UI.WebControls;
using com.next.isam.webapp.commander;
using com.next.infra.web;
using com.next.infra.util;

namespace com.next.isam.webapp.account
{
    public partial class PaymentUpdate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {                
                this.cbxSunDB.DataSource = WebUtil.getSunDbList();
                this.cbxSunDB.DataBind();

                ListItem item = cbxSunDB.Items.FindByValue("1NS");
                item.Selected = true;
            }
        }

        protected void valFile_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (this.lblBankFileName.Text == "")
            {
                ((CustomValidator)source).ErrorMessage = "The file does not exist or does not contain any data";
                args.IsValid = false;
            }
            else if (this.lblBankFileName.Text == "-1")
            {
                ((CustomValidator)source).ErrorMessage = "The file already exists in the server, please rename the file";
                args.IsValid = false;
            }
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            this.valFile.Enabled = true;
            
            if (updFile.Value != String.Empty)
            {
                Context.Items.Clear();
                /*
                this.bankReconciliation(this.copyFile());
                this.vwSummaryList = (ArrayList)Context.Items[eBankingCommander.Param.summaryList];
                this.repSummaryList.DataSource = vwSummaryList;
                this.repSummaryList.DataBind();
                this.vwJobList = (ArrayList)Context.Items[eBankingCommander.Param.jobList];
                this.repQueueList.DataSource = vwJobList;
                this.repQueueList.DataBind();
                */
            }
            this.valFile.Validate();
        }

        private string copyFile()
        {
            string clientFilePath = updFile.Value;
            string[] fileSubstring = clientFilePath.Split('\\');
            string fileName = fileSubstring[fileSubstring.Length - 1].ToString();
            string bankRecPath = WebConfig.getValue("appSettings", "UPLOAD_EBANKING_RECON_Folder");
            string destinationPath = "";

            if (updFile.PostedFile != null)
            {
                try
                {
                    if (updFile.PostedFile.InputStream.Length > 0)
                    {
                        destinationPath = bankRecPath + fileName;
                        /*
                        if (isUnfinishedJob(destinationPath))
                        {
                            this.repQueueList.DataSource = vwRequestList;
                            this.repQueueList.DataBind();
                            this.lblBankFileName.Text = "-1";
                            return "";
                        }
                        */
                        updFile.PostedFile.SaveAs(destinationPath);
                    }
                    this.lblBankFileName.Text = destinationPath;
                }
                catch (Exception e)
                {
                    this.lblBankFileName.Text = "-1";
                    return e.Message;
                }
            }
            return destinationPath;
        }

        /*
        private bool isFileRequested(string fileName)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account.PaymentUpdate");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.checkFileDuplication);
            Context.Items.Add(eBankingCommander.Param.sourcePath, fileName);
            forwardToScreen(null);

            this.vwRequestList = (ArrayList) Context.Items[AccountCommander.Param.requestList];
            return (this.vwRequestList.Count > 0) ? true : false;
        }
        */

        private ArrayList vwRequestList
        {
            set { ViewState["RequestList"] = value; }
            get { return (ArrayList)ViewState["RequestList"]; }
        }

    }
}
