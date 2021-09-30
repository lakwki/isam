using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.isam.appserver.account;
using com.next.isam.appserver.common;
using System.Web.UI.WebControls;
using com.next.common.domain.types;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.helper;
using com.next.infra.util;
using System.Web.UI;
using com.next.isam.webapp.commander;
using com.next.common.appserver;


namespace com.next.isam.webapp.nontrade
{
    public partial class DebitNoteSearch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {

                //ArrayList userOfficeList = WebUtil.getNTOfficeList(this.LogonUserId);
                ArrayList userOfficeList = NonTradeManager.Instance.getNTUserOfficeList(this.LogonUserId, NTRoleType.RECHARGE_DCNOTE_USER.Id, GeneralCriteria.ALL);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);

                this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId");
                if (userOfficeList.Count == 1)
                {
                    ReportOfficeGroupRef oref = (ReportOfficeGroupRef)officeGroupList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeGroupId.ToString();
                }

                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 3).ToString(), (calenderDef.BudgetYear - 3).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 2).ToString(), (calenderDef.BudgetYear - 2).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 1).ToString(), (calenderDef.BudgetYear - 1).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_Year.selectByValue(calenderDef.BudgetYear.ToString());

                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 3).ToString(), (calenderDef.BudgetYear - 3).ToString()));
                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 2).ToString(), (calenderDef.BudgetYear - 2).ToString()));
                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 1).ToString(), (calenderDef.BudgetYear - 1).ToString()));
                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_FiscalYear.selectByValue(calenderDef.BudgetYear.ToString());

                this.ddl_Month.Items.Add(new ListItem("JAN", "1"));
                this.ddl_Month.Items.Add(new ListItem("FEB", "2"));
                this.ddl_Month.Items.Add(new ListItem("MAR", "3"));
                this.ddl_Month.Items.Add(new ListItem("APR", "4"));
                this.ddl_Month.Items.Add(new ListItem("MAY", "5"));
                this.ddl_Month.Items.Add(new ListItem("JUN", "6"));
                this.ddl_Month.Items.Add(new ListItem("JUL", "7"));
                this.ddl_Month.Items.Add(new ListItem("AUG", "8"));
                this.ddl_Month.Items.Add(new ListItem("SEP", "9"));
                this.ddl_Month.Items.Add(new ListItem("OCT", "10"));
                this.ddl_Month.Items.Add(new ListItem("NOV", "11"));
                this.ddl_Month.Items.Add(new ListItem("DEC", "12"));
                this.ddl_Month.selectByValue("2");

                for (int i = 1; i <= 12; i++)
                {
                    this.ddl_Period.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }

                this.radMonth.Checked = true;

                this.bindGrid();

                this.btn_SendMail.Attributes.Add("onclick", "if(confirm('Confirm to send mail?')) showProgressBarX(); else return false;");
            }
            //this.btn_Search_Click(null, null);
        }

        private void bindGrid()
        {
            if (ddl_Office.Items.Count > 0)
            {
                this.vwNoteList = NonTradeManager.Instance.getNTRechargeDCNoteInvoiceList(int.Parse(ddl_Office.SelectedValue),
                                                    (this.radMonth.Checked) ? 1 : 2,
                                                    (this.radMonth.Checked) ? int.Parse(ddl_Year.SelectedValue) : int.Parse(ddl_FiscalYear.SelectedValue),
                                                    (this.radMonth.Checked) ? int.Parse(ddl_Month.SelectedValue) : int.Parse(ddl_Period.SelectedValue));
                gv_Request.DataSource = this.vwNoteList;
            }
            else
                gv_Request.DataSource = null;
            gv_Request.DataBind();
        }

        ArrayList vwNoteList
        {
            get { return (ArrayList)ViewState["NoteList"]; }
            set { ViewState["NoteList"] = value; }
        }

        protected void gv_Request_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NTRechargeDCNoteInvoiceRef row = (NTRechargeDCNoteInvoiceRef)this.vwNoteList[e.Row.RowIndex];
                NTRechargeDCNoteDef DCNoteDef = row.DCNote;
                
                ((LinkButton)e.Row.FindControl("lnk_DebitNoteNo")).Text = DCNoteDef.RechargeDCNoteNo;
                ((LinkButton)e.Row.FindControl("lnk_DebitNoteNo")).CommandArgument = e.Row.RowIndex.ToString();
                ((LinkButton)e.Row.FindControl("lnk_Excel")).CommandArgument = e.Row.RowIndex.ToString();
                ((LinkButton)e.Row.FindControl("lnk_Excel")).Text = "<img src=\"../images/Icon_Excel.jpg\" border=0>";
                ((Label)e.Row.FindControl("lbl_Office")).Text = DCNoteDef.Office.OfficeCode;
                ((Label)e.Row.FindControl("lbl_IssueDate")).Text = DateTimeUtility.getDateString(DCNoteDef.RechargeDCNoteDate);
                if (DCNoteDef.ToCustomerId != 0)
                    ((Label)e.Row.FindControl("lbl_IssuedTo")).Text = WebUtil.getCustomerByKey(DCNoteDef.ToCustomerId).CustomerDescription;
                else if (DCNoteDef.ToOfficeId != 0 && DCNoteDef.ToCompanyId != 0)
                    ((Label)e.Row.FindControl("lbl_IssuedTo")).Text = GeneralManager.Instance.getCompanyByKey(DCNoteDef.ToCompanyId).CompanyName;
                else if (DCNoteDef.ToVendorId != 0)
                    ((Label)e.Row.FindControl("lbl_IssuedTo")).Text = IndustryUtil.getVendorByKey(DCNoteDef.ToVendorId).Name;
                else if (DCNoteDef.ToNTVendorId != 0)
                    ((Label)e.Row.FindControl("lbl_IssuedTo")).Text = WebUtil.getNTVendorByKey(DCNoteDef.ToNTVendorId).VendorName;

                ((Label)e.Row.FindControl("lbl_Currency")).Text = CurrencyId.getName(DCNoteDef.RechargeCurrencyId);
                ((Label)e.Row.FindControl("lbl_TotalAmt")).Text = Math.Abs(DCNoteDef.RechargeAmount).ToString("#,##0.00");

                CheckBox chb_Mail = ((CheckBox)e.Row.FindControl("chb_Mail"));
                if (DCNoteDef.MailStatus == 0)
                {
                    this.btn_SendMail.Visible = true;
                    this.chkSupportingDoc.Visible = true;
                }
                chb_Mail.Visible = (DCNoteDef.MailStatus == 0);
                chb_Mail.Checked = DCNoteDef.MailStatus == 0 ? false : true;

                if (DCNoteDef.ToVendorId != 0)
                {
                    if (string.IsNullOrEmpty(IndustryUtil.getVendorByKey(DCNoteDef.ToVendorId).eAdviceAddr))
                    {
                        //chb_Mail.Visible = false;
                        chb_Mail.Enabled = false;
                        ((Label)e.Row.FindControl("lbl_VendorEmail")).Text = "Missing";
                        ((Label)e.Row.FindControl("lbl_VendorEmail")).ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        ((Label)e.Row.FindControl("lbl_VendorEmail")).Text = IndustryUtil.getVendorByKey(DCNoteDef.ToVendorId).eAdviceAddr;
                        gv_Request.Columns[9].Visible = true;
                    }
                }
                else if (DCNoteDef.ToNTVendorId != 0)
                {
                    if (string.IsNullOrEmpty(WebUtil.getNTVendorByKey(DCNoteDef.ToNTVendorId).EAdviceEmail))
                    {
                        //chb_Mail.Visible = false;
                        chb_Mail.Enabled = false;
                        ((Label)e.Row.FindControl("lbl_VendorEmail")).Text = "Missing";
                        ((Label)e.Row.FindControl("lbl_VendorEmail")).ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        ((Label)e.Row.FindControl("lbl_VendorEmail")).Text = WebUtil.getNTVendorByKey(DCNoteDef.ToNTVendorId).EAdviceEmail;
                        gv_Request.Columns[9].Visible = true;
                    }
                }

                Panel pnl = ((Panel)e.Row.FindControl("pnl_Invoices"));
                bool isFirst = true;
                foreach (NTInvoiceDef inv in row.InvoiceList)
                {
                    //NTInvoiceDef inv = NonTradeManager.Instance.getNTInvoiceByKey(invoiceId);
                    HyperLink lnk = new HyperLink();
                    lnk.Attributes.Add("onclick", "openInvoiceWindow(" + inv.InvoiceId.ToString() + ");return false;");
                    lnk.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                    lnk.Text = " " + inv.InvoiceNo + "&nbsp;";
                    pnl.Controls.Add(lnk);

                    /*
                    if (!isFirst)
                    {
                        Literal lt = new Literal();
                        lt.Text = "<br/>";
                        pnl.Controls.Add(lt);
                    }

                    HyperLink lnk = new HyperLink();
                    lnk.Attributes.Add("onclick", "openInvoiceWindow(" + inv.InvoiceId.ToString() + ");return false;");
                    lnk.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                    lnk.Text = " " + inv.InvoiceNo + "&nbsp;"; 
                    pnl.Controls.Add(lnk);

                    Image img = new Image();
                    img.ImageUrl = "~/images/icon_edit.gif";
                    img.ToolTip = "Edit Desc";
                    pnl.Controls.Add(img);

                    isFirst = false;
                    */
                }
            }
        }

       
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            this.bindGrid();
        }

        protected void gv_Request_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            NTRechargeDCNoteInvoiceRef rf = (NTRechargeDCNoteInvoiceRef)this.vwNoteList[int.Parse((string)e.CommandArgument)];
            this.getNTDebitNoteReport(rf.DCNote.RechargeDCNoteId, e.CommandName);
        }

        private void getNTDebitNoteReport(int rechargeDCNoteId, string commandName)
        {
            NTRechargeDCNote report = AccountReportManager.Instance.getNTRechargeDCNote(rechargeDCNoteId);
            //report.SetDataSource(ds);
            if (commandName == "OutputPDF")
                ReportHelper.export(report, HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "NT Recharge DC Note");
            else
                ReportHelper.export(report, HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "NT Recharge DC Note");
        }

        protected void CheckBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)gv_Request.HeaderRow.FindControl("chb_All");

            foreach (GridViewRow row in gv_Request.Rows)
            {
                CheckBox chckrw = (CheckBox)row.FindControl("chb_Mail");
                chckrw.Checked = chk.Checked;
            }

        }

        protected void btn_SendMail_Click(object sender, EventArgs e)
        {
            List<NTRechargeDCNoteInvoiceRef> list = new List<NTRechargeDCNoteInvoiceRef>();
            for (int i = 0; i < gv_Request.Rows.Count; i++)
            {
                CheckBox chkBox = (CheckBox)gv_Request.Rows[i].FindControl("chb_Mail");
                if (chkBox.Checked && chkBox.Visible)
                {
                    NTRechargeDCNoteInvoiceRef def = (NTRechargeDCNoteInvoiceRef)this.vwNoteList[i];
                    if(def.DCNote.MailStatus == 0)
                        list.Add(def);
                }
            }

            if (list.Count > 0)
            {
                foreach (NTRechargeDCNoteInvoiceRef d in list)
                {
                    NonTradeManager.Instance.sendNTRechargeDCNoteCover(d, this.chkSupportingDoc.Checked);
                }
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Non-Trade DC Note", "alert('Mail has been sent');", true);
            }
            this.btn_Search_Click(null, null);
        }

    }
}
