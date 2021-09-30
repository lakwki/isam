using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using com.next.infra.web;
using com.next.infra.util;
using com.next.common.web.commander;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.account;
using com.next.isam.domain.types ;

namespace com.next.isam.webapp.account
{
    public partial class BankReconciliation : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private ArrayList vwSearchResult
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


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ddl_SunDB.DataSource = WebUtil.getSunDbList();
                ddl_SunDB.DataBind();

                ListItem item = ddl_SunDB.Items.FindByValue("1NS");
                item.Selected = true;

            }
        }

        protected void btn_ListPending_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetBankReconciliationRequestList);

            forwardToScreen(null);

            ArrayList list = (ArrayList)Context.Items[AccountCommander.Param.bankReconList];

            vwSearchResult = list;
            gv_PendingJob.DataSource = vwSearchResult;
            gv_PendingJob.DataBind();
        }

        protected void btn_Process_Click(object sender, EventArgs e)
        {
            if (!FileUpload1.HasFile)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "BankReconErr", "alert('Please select a file to upload.');", true);
                return;
            }

            string filePath = copyFile();
            string bank = "";
            string sunDB;

            if (filePath == "")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "BankReconErr", "alert('Request already submitted.')", true);
                return;
            }

            if (radHSBC.Checked)
                bank = "HSBC";
            else
                bank = "SCB";

            sunDB = ddl_SunDB.SelectedValue;

            BankReconciliationRequestDef request = new BankReconciliationRequestDef(filePath, sunDB, bank, CommonUtil.getUserByKey(this.LogonUserId));

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateBankReconciliationRequest);

            Context.Items.Add(AccountCommander.Param.bankReconRequest, request);

            forwardToScreen(null);

            vwSearchResult.Add(request);
            gv_PendingJob.DataSource = vwSearchResult;
            gv_PendingJob.DataBind();
        }

        protected void BankReconRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            BankReconciliationRequestDef request = (BankReconciliationRequestDef) vwSearchResult[arg.RowIndex];
            request.Status = RequestStatus.CANCEL.StatusId;

            vwSearchResult.Remove(request);

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateBankReconciliationRequest);

            Context.Items.Add(AccountCommander.Param.bankReconRequest, request);

            forwardToScreen(null);

            gv_PendingJob.DataSource = vwSearchResult;
            gv_PendingJob.DataBind();
        }

        private string copyFile()
        {
            
            
            string destinationPath = WebConfig.getValue("appSettings", "UPLOAD_EBANKING_RECON_Folder") + FileUpload1.FileName;

            if (FileUpload1.HasFile)
            {
                try
                {                                            
                        if (isUnfinishedJob(destinationPath))
                        {
                            return "";
                        }

                        FileUpload1.PostedFile.SaveAs(destinationPath);                    
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return destinationPath;
        }

        private bool isUnfinishedJob(string filePath)
        {

            if (vwSearchResult == null)
            {
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetBankReconciliationRequestList);

                forwardToScreen(null);

                ArrayList list = (ArrayList)Context.Items[AccountCommander.Param.bankReconList];

                vwSearchResult = list;
            }

            foreach (BankReconciliationRequestDef request in vwSearchResult)
            {
                if (request.FileName == filePath)
                    return true;
            }

            return false;
        }

    }
}
