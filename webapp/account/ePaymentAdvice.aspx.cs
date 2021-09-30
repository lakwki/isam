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
using com.next.infra.util;
using com.next.infra.web;
using com.next.common.web.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.account;
using com.next.isam.domain.types;

namespace com.next.isam.webapp.account
{
    public partial class ePaymentAdvice : com.next.isam.webapp.usercontrol.PageTemplate
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


        protected void btn_ShowFolder_Click(object sender, EventArgs e)
        {

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetFileGenerateRequestList);

            forwardToScreen(null);

            vwSearchResult  = (ArrayList) Context.Items[AccountCommander.Param.requestList];

            gv_FileUpload.DataSource = vwSearchResult;
            gv_FileUpload.DataBind();
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {

            foreach (GridViewRow row in gv_FileUpload.Rows)
            {
                CheckBox ckb = (CheckBox) row.Cells[0].FindControl("ckb_Cancel");

                if (!ckb.Checked)
                    continue;

                GenerateFileRequestDef request = (GenerateFileRequestDef)vwSearchResult[row.RowIndex];
                request.Status = RequestStatus.CANCEL.StatusId;
                
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GeneratePaymentAdvice);

                Context.Items.Add(AccountCommander.Param.genPaymentAdviceRequest, request);

                forwardToScreen(null);
            }

            gv_FileUpload.DataSource = vwSearchResult;
            gv_FileUpload.DataBind();
        }

        protected void btn_Process_Click(object sender, EventArgs e)
        {
            if (!FileUpload1.HasFile)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "uploadErr", "alert('Please select a file to upload.');", true);
                return;
            }

            string filePath = WebConfig.getValue("appSettings", "UPLOAD_EBANKING_TEMP_Folder");
            filePath += FileUpload1.FileName;
            FileUpload1.SaveAs(filePath);

            ArrayList duplicateList = getDuplicate(filePath);
            File.Delete(filePath);

            if (duplicateList.Count != 0)
            {
                pnl_Duplicate.Visible = true;
                lbl_duplicateList.Text = "";

                foreach (object obj in duplicateList)
                {
                    lbl_duplicateList.Text += obj.ToString() + "<br />";
                }
                return;
            }
            else
            {
                filePath = WebConfig.getValue("appSettings", "PAYMENT_ADVICE_UPLOAD_Folder");
                filePath += FileUpload1.FileName;
                FileUpload1.PostedFile.SaveAs(filePath);
                

                GenerateFileRequestDef request = new GenerateFileRequestDef(filePath, GenerateFileRequestDef.Type.PaymentAdvice.GetHashCode(),
                      CommonUtil.getUserByKey(this.LogonUserId));

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GeneratePaymentAdvice);

                Context.Items.Add(AccountCommander.Param.genPaymentAdviceRequest, request);

                forwardToScreen(null);

                if (vwSearchResult == null)
                    vwSearchResult = new ArrayList();
                vwSearchResult.Add(request);

                gv_FileUpload.DataSource = vwSearchResult;
                gv_FileUpload.DataBind();
            }
        }

        public ArrayList getDuplicate(string filePath)
        {
            System.IO.StreamReader reader = new StreamReader(filePath);
            string s;
            string[] cols;
            ArrayList list = new ArrayList();
            ArrayList duplicatedList = new ArrayList();

            try
            {                
                s = reader.ReadLine();
                
                while (s != null)
                {
                    s = s.Trim();
                    if (s.Contains("'"))
                        s = s.Replace("'", "''");

                    if (s != "")
                    {
                        cols = s.Split(";".ToCharArray());
                        if (cols[0].Trim() == "DETAILS")
                        {                                                        
                            if (list.Contains(s))
                            {
                                if (!duplicatedList.Contains(s))
                                    duplicatedList.Add(s);
                            }
                            else
                                list.Add(s);                                                        
                        }
                    }
                    s = reader.ReadLine();

                }
                reader.Close();
                return duplicatedList;
            }
            catch (Exception e)
            {
                reader.Close();
                throw e;
            }
        }

    }
}
