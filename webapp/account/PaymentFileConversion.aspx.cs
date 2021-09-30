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
using com.next.infra.util;
using com.next.infra.web;
using com.next.common.web.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.account;

namespace com.next.isam.webapp.account
{
    public partial class PaymentFileConversion : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BankSelectIndexChanged(object sender, System.EventArgs e)
        {
            if (rad_Bank.SelectedValue == "HSBC")
                row_ChargeMethod.Visible = true;
            else
                row_ChargeMethod.Visible = false;
        }

        protected void btn_Process_Click(object sender, EventArgs e)
        {
            if (!FileUpload1.HasFile)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "PaymentFileErr", "alert('Please select a file to upload.');", true);
                return;
            }
            else if (!FileUpload1.FileName.ToLower().EndsWith(".csv"))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "PaymentFileErr", "alert('Incorrect file format.');", true);
                return;
            }

            string filePath = uploadFile();
            int bank = rad_Bank.SelectedValue == "HSBC" ? 1 : 2;
            int chargetMethod = 0;
            if (bank == 1)
                chargetMethod = Convert.ToInt32(ddl_ChargeMethod.SelectedValue);

            ConvertPaymentFileRequestDef request = new ConvertPaymentFileRequestDef(filePath, bank, chargetMethod, CommonUtil.getUserByKey(this.LogonUserId));

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.ConvertPaymentFile);

            Context.Items.Add(AccountCommander.Param.convertPaymentFileRequest, request);
            forwardToScreen(null);


        }

        private string uploadFile()
        {
            string fileName = FileUpload1.FileName;
            string destinationPath = WebConfig.getValue("appSettings", "UPLOAD_EBANKING_TEMP_Folder") + fileName;

            if (FileUpload1.HasFile)
            {
                try
                {
                    FileUpload1.PostedFile.SaveAs(destinationPath);                        
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return destinationPath;
        }

    }
}
