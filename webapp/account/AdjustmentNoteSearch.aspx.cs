using System;
using System.Collections;
using System.Web;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.domain.account;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.datafactory.worker;
using com.next.isam.appserver.account;
using com.next.isam.appserver.common;
using System.Web.UI.WebControls;
using com.next.common.domain.types;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.dataserver;
using com.next.isam.reporter.helper;
using com.next.infra.util;
using com.next.common.domain.module;
using com.next.isam.domain.claim;
using com.next.isam.appserver.claim;
using com.next.common.domain.industry.vendor;
using System.Web.UI;
using System.Collections.Generic;

namespace com.next.isam.webapp.account
{
    public partial class AdjustmentNoteSearch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {

                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);


                this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId");
                if (userOfficeList.Count == 1)
                {
                    ReportOfficeGroupRef oref = (ReportOfficeGroupRef)userOfficeList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeGroupId.ToString();
                }

                /*
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

                this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId");
                if (userOfficeList.Count == 1)
                {
                    OfficeRef oref = (OfficeRef) userOfficeList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                }
                */

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
                    this.ddl_AdjustmentType.Items.Add(new ListItem("Next Claim", "3"));
                else
                {
                    this.ddl_AdjustmentType.Items.Add(new ListItem(AdjustmentType.SALES_ADJUSTMENT.Name, AdjustmentType.SALES_ADJUSTMENT.Id.ToString()));
                    this.ddl_AdjustmentType.Items.Add(new ListItem(AdjustmentType.PURCHASE_ADJUSTMENT.Name, AdjustmentType.PURCHASE_ADJUSTMENT.Id.ToString()));
                    this.ddl_AdjustmentType.Items.Add(new ListItem(AdjustmentType.MOCK_SHOP.Name, AdjustmentType.MOCK_SHOP.Id.ToString()));
                    this.ddl_AdjustmentType.Items.Add(new ListItem(AdjustmentType.UKCLAIM.Name, AdjustmentType.UKCLAIM.Id.ToString()));
                    this.ddl_AdjustmentType.Items.Add(new ListItem(AdjustmentType.UT_DN.Name, AdjustmentType.UT_DN.Id.ToString()));
                    this.ddl_AdjustmentType.Items.Add(new ListItem(AdjustmentType.QA_COMMISSION.Name, AdjustmentType.QA_COMMISSION.Id.ToString()));
                    this.ddl_AdjustmentType.Items.Add(new ListItem(AdjustmentType.STUDIO_SAMPLE.Name, AdjustmentType.STUDIO_SAMPLE.Id.ToString()));
                    this.ddl_AdjustmentType.Items.Add(new ListItem(AdjustmentType.ADVANCE_PAYMENT_INTEREST.Name, AdjustmentType.ADVANCE_PAYMENT_INTEREST.Id.ToString()));
                    this.ddl_AdjustmentType.Items.Add(new ListItem(AdjustmentType.OTHER_CHARGE.Name, AdjustmentType.OTHER_CHARGE.Id.ToString()));
                    /*
                    this.ddl_AdjustmentType.Items.Add(new ListItem("Next Claim - Refund", "4"));
                    */
                }

                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 4).ToString(), (calenderDef.BudgetYear - 4).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 3).ToString(), (calenderDef.BudgetYear - 3).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 2).ToString(), (calenderDef.BudgetYear - 2).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 1).ToString(), (calenderDef.BudgetYear - 1).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_Year.selectByValue(calenderDef.BudgetYear.ToString());

                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 4).ToString(), (calenderDef.BudgetYear - 4).ToString()));
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
                this.ddl_Month.selectByValue(DateTime.Today.Month.ToString());

                for (int i = 1; i <= 12; i++)
                {
                    this.ddl_Period.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
                AccountFinancialCalenderDef calcDef = CommonUtil.getAccountPeriodByDate(AppId.ISAM.Code, DateTime.Today);
                this.ddl_Period.selectByValue(calcDef.Period.ToString());

                this.radMonth.Checked = true;
                this.btn_Send.Attributes.Add("onclick", "if (!confirm('Confirm to send selected claim(s)/refund(s) to the supplier?')) return false;showProgressBarX();");

                this.bindGrid();
            }
            /*
            this.btn_Search_Click(null, null);
            */
        }

        private void bindGrid()
        {
            int selectedAdjustmentType = int.Parse(ddl_AdjustmentType.SelectedValue);

            this.vwNoteList = AccountManager.Instance.getAdjustmentNoteList(int.Parse(ddl_Office.SelectedValue),
                                                int.Parse(ddl_AdjustmentType.SelectedValue),
                                                (this.radMonth.Checked) ? 1 : 2,
                                                (this.radMonth.Checked) ? int.Parse(ddl_Year.SelectedValue) : int.Parse(ddl_FiscalYear.SelectedValue),
                                                (this.radMonth.Checked) ? int.Parse(ddl_Month.SelectedValue) : int.Parse(ddl_Period.SelectedValue));
            gv_Request.DataSource = this.vwNoteList;
            gv_Request.DataBind();
            gv_Request.Columns[11].Visible = (selectedAdjustmentType == AdjustmentType.UKCLAIM.Id 
                                              || selectedAdjustmentType == AdjustmentType.PURCHASE_ADJUSTMENT.Id
                                              || selectedAdjustmentType == AdjustmentType.ADVANCE_PAYMENT_INTEREST.Id
                                              || selectedAdjustmentType == AdjustmentType.OTHER_CHARGE.Id);
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
                AdjustmentNoteDef def = (AdjustmentNoteDef)this.vwNoteList[e.Row.RowIndex];

                ((LinkButton)e.Row.FindControl("lnk_AdjustmentNoteNo")).Text = def.AdjustmentNoteNo;
                ((LinkButton)e.Row.FindControl("lnk_AdjustmentNoteNo")).CommandArgument = e.Row.RowIndex.ToString();
                ((LinkButton)e.Row.FindControl("lnk_Excel")).CommandArgument = e.Row.RowIndex.ToString();
                ((LinkButton)e.Row.FindControl("lnk_Excel")).Text = "<img src=\"../images/Icon_Excel.jpg\" border=0>";
                ((Label)e.Row.FindControl("lbl_AdjustmentType")).Text = def.AdjustmentType.Name;
                ((Label)e.Row.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);
                ((Label)e.Row.FindControl("lbl_IssueDate")).Text = DateTimeUtility.getDateString(def.IssueDate);
                ((Label)e.Row.FindControl("lbl_IssuedTo")).Text = def.PartyName; 
                ((Label)e.Row.FindControl("lbl_Currency")).Text = CurrencyId.getName(def.CurrencyId);
                ((Label)e.Row.FindControl("lbl_TotalAmt")).Text = Math.Abs(def.Amount).ToString("#,##0.00");
                LinkButton lnkBtn, lnkUpdateBtn;
                TextBox txtDesc;
                CheckBox chbMail;

                ImageButton lnkAttachment = (ImageButton)e.Row.FindControl("lnk_Attachment");
                lnkAttachment.Visible = false;

                if (def.AdjustmentType.Id == AdjustmentType.UKCLAIM.Id)
                {
                    gv_Request.Columns[0].Visible = true;
                    UKClaimDCNoteDef noteDef = UKClaimManager.Instance.getUKClaimDCNoteByKey(def.AdjustmentNoteId);

                    lnkBtn = (LinkButton)e.Row.FindControl("lnk_SendUKClaimDCNote");
                    lnkUpdateBtn = (LinkButton)e.Row.FindControl("lnk_UpdateCustomRemark");
                    txtDesc = (TextBox)e.Row.FindControl("txt_Description");
                    chbMail = (CheckBox)e.Row.FindControl("chb_Mail");
                    if (!noteDef.IsVoid && noteDef.SettlementDate == DateTime.MinValue)
                    {
                        Button btn_VoidDN = (Button)e.Row.FindControl("btn_VoidDN");
                        btn_VoidDN.Visible = true;
                        string js = String.Format(@"window.open('DebitNoteVoid.aspx?dnno={0}', 'DebitNoteVoid', 'width=800,height=350, scrollbars=1,resizable=1,status=1');return false;", noteDef.DCNoteNo);
                        btn_VoidDN.OnClientClick = js;
                    }

                    if (noteDef.MailStatus == 1)
                    {
                        lnkBtn.Visible = true;
                        lnkUpdateBtn.Visible = true;
                        txtDesc.Visible = true;
                        txtDesc.Text = noteDef.Remark;
                        chbMail.Visible = true;

                        this.btn_Send.Visible = true;

                        lnkUpdateBtn.CommandArgument = e.Row.RowIndex.ToString();

                        VendorRef vendor = IndustryUtil.getVendorByKey(noteDef.VendorId);

                        if (vendor.eAdviceAddr.Trim() == string.Empty)
                        {
                            lnkBtn.Text = "Email Addr Not Defined";
                            lnkBtn.Enabled = false;
                            chbMail.Visible = false;
                            this.btn_Send.Visible = false;
                            if (noteDef.DebitCreditIndicator == "D")
                            {
                                lnkAttachment.Visible = true;
                                lnkAttachment.Attributes.Add("onclick", "window.open('../claim/DNAttachmentList.aspx?dcNoteId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.AdjustmentNoteId.ToString())) + "', 'attachmentlist', 'width=500,height=500,scrollbars=1,status=0');return false;");
                            }
                        }
                        /*
                        else if (noteDef.DebitCreditIndicator == "C" && !UKClaimManager.Instance.IsSignedCNCopyUploaded(noteDef))
                        {
                            lnkBtn.Text = "Signed copy required";
                            lnkBtn.Enabled = false;
                        }
                        */
                        else
                        {
                            lnkBtn.CommandArgument = e.Row.RowIndex.ToString();
                            lnkBtn.Text = "Send To Supplier";
                            lnkBtn.ToolTip = "Email Addr : " + vendor.eAdviceAddr;
                            lnkBtn.Attributes.Add("onclick", "if (!confirm('Confirm to send it to the supplier?')) return false;showProgressBarX();");
                            if (noteDef.DebitCreditIndicator == "D")
                            {
                                lnkAttachment.Visible = true;
                                lnkAttachment.Attributes.Add("onclick", "window.open('../claim/DNAttachmentList.aspx?dcNoteId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.AdjustmentNoteId.ToString())) + "', 'attachmentlist', 'width=500,height=500,scrollbars=1,status=0');return false;");
                            }
                        }
                    }
                    if (noteDef.RevisedCurrencyId != -1)
                    {
                        ((Label)e.Row.FindControl("lbl_Currency")).Text += (" -> " + CurrencyId.getName(noteDef.RevisedCurrencyId));
                        ((Label)e.Row.FindControl("lbl_Currency")).ForeColor = System.Drawing.Color.Blue;
                    }

                    if (noteDef.SettledAmount != 0 && !noteDef.IsVoid && noteDef.RevisedCurrencyId == -1)
                    {
                        Button btnChangeCurrency = (Button)e.Row.FindControl("btn_ChangeCurrency");
                        btnChangeCurrency.Attributes.Add("onclick", "window.open('../account/AdjustmentNoteSearchPopup.aspx?adjustmentNoteId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.AdjustmentNoteId.ToString())) + "&adjustmentTypeId=" + def.AdjustmentType.Id + "', 'changeCurrencyWindow', 'width=400,height=200,scrollbars=1,status=0');return false;");
                        btnChangeCurrency.Visible = true;
                    }
                }
                else if (def.AdjustmentType.Id == AdjustmentType.PURCHASE_ADJUSTMENT.Id)
                {
                    lnkBtn = (LinkButton)e.Row.FindControl("lnk_SendPurchaseDCNote");
                    if (def.MailStatus == 0 || def.MailStatus == int.MinValue)
                    {
                        lnkBtn.Visible = true;
                        VendorRef vendor = IndustryUtil.getVendorByKey(def.VendorId);
                        if (vendor.eAdviceAddr.Trim() == string.Empty)
                        {
                            lnkBtn.Text = "Email Addr Not Defined";
                            lnkBtn.Enabled = false;
                        }
                        else
                        {
                            lnkBtn.CommandArgument = e.Row.RowIndex.ToString();
                            lnkBtn.Text = "Send To Supplier";
                            lnkBtn.ToolTip = "Email Addr : " + vendor.eAdviceAddr;
                            lnkBtn.Attributes.Add("onclick", "if (!confirm('Confirm to send it to the supplier?')) return false;showProgressBarX();");
                        }
                    }
                    gv_Request.Columns[0].Visible = false;
                    gv_Request.Columns[9].Visible = false;      // hide the attachment column

                    if (def.RevisedCurrencyId != -1)
                    {
                        ((Label)e.Row.FindControl("lbl_Currency")).Text += (" -> " + CurrencyId.getName(def.RevisedCurrencyId));
                        ((Label)e.Row.FindControl("lbl_Currency")).ForeColor = System.Drawing.Color.Blue;
                    }

                    if (def.Amount != 0 && def.RevisedCurrencyId == -1)
                    {
                        Button btnChangeCurrency = (Button)e.Row.FindControl("btn_ChangeCurrency");
                        btnChangeCurrency.Attributes.Add("onclick", "window.open('../account/AdjustmentNoteSearchPopup.aspx?adjustmentNoteId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.AdjustmentNoteId.ToString())) + "&adjustmentTypeId=" + def.AdjustmentType.Id + "', 'changeCurrencyWindow', 'width=400,height=200,scrollbars=1,status=0');return false;");
                        btnChangeCurrency.Visible = true;
                    }
                }
                else if (def.AdjustmentType.Id == AdjustmentType.ADVANCE_PAYMENT_INTEREST.Id)
                {
                    lnkBtn = (LinkButton)e.Row.FindControl("lnk_SendAdvancePaymentDCNote");
                    if (def.MailStatus == 0 || def.MailStatus == int.MinValue)
                    {
                        lnkBtn.Visible = true;
                        VendorRef vendor = IndustryUtil.getVendorByKey(def.VendorId);
                        if (vendor.eAdviceAddr.Trim() == string.Empty)
                        {
                            lnkBtn.Text = "Email Addr Not Defined";
                            lnkBtn.Enabled = false;
                        }
                        else
                        {
                            lnkBtn.CommandArgument = e.Row.RowIndex.ToString();
                            lnkBtn.Text = "Send To Supplier";
                            lnkBtn.ToolTip = "Email Addr : " + vendor.eAdviceAddr;
                            lnkBtn.Attributes.Add("onclick", "if (!confirm('Confirm to send it to the supplier?')) return false;showProgressBarX();");
                        }
                    }
                    gv_Request.Columns[0].Visible = false;
                    gv_Request.Columns[9].Visible = false;      // hide the attachment column
                }

                else if (def.AdjustmentType.Id == AdjustmentType.OTHER_CHARGE.Id)
                {
                    gv_Request.Columns[0].Visible = true;
                    GenericDCNoteDef dcNoteDef = AccountManager.Instance.getGenericDCNoteById(def.AdjustmentNoteId);
                    lnkBtn = (LinkButton)e.Row.FindControl("lnk_SendGenericDCNote");
                    chbMail = (CheckBox)e.Row.FindControl("chb_Mail");
                    if (dcNoteDef.IssueTypeId == 1 && (def.MailStatus == 0 || def.MailStatus == int.MinValue))
                    {
                        lnkBtn.Visible = true;
                        VendorRef vendor = IndustryUtil.getVendorByKey(dcNoteDef.VendorId);
                        NTVendorDef ntVendor = NonTradeManager.Instance.getNTVendorByKey(dcNoteDef.NTVendorId);
                        string email = string.Empty;
                        if (vendor != null)
                            email = vendor.eAdviceAddr.Trim();
                        else if (ntVendor != null)
                            email = ntVendor.EAdviceEmail.Trim();

                        if (email == string.Empty)
                        {
                            lnkBtn.Text = "Email Addr Not Defined";
                            lnkBtn.Enabled = false;
                        }
                        else
                        {
                            chbMail.Visible = true;
                            this.btn_Send.Visible = true;

                            lnkBtn.CommandArgument = e.Row.RowIndex.ToString();
                            lnkBtn.Text = "Send To Supplier";
                            lnkBtn.ToolTip = "Email Addr : " + email;
                            lnkBtn.Attributes.Add("onclick", "if (!confirm('Confirm to send it to the supplier?')) return false;showProgressBarX();");
                        }
                    }
                    if (dcNoteDef.IssueTypeId == 3 && (def.MailStatus == 0 || def.MailStatus == int.MinValue))
                    {
                        lnkBtn.Visible = true;
                        chbMail.Visible = true;
                        this.btn_Send.Visible = true;

                        lnkBtn.CommandArgument = e.Row.RowIndex.ToString();
                        lnkBtn.Text = "Send To Supplier";
                        lnkBtn.ToolTip = "Email Addr : shyami@nextsl.com.lk; Sithari@NextSL.com.lk; Winnie_Lui@nextsl.com.hk; pavithrah@nextsl.com.lk";

                        lnkBtn.Attributes.Add("onclick", "if (!confirm('Confirm to send it to the supplier?')) return false;showProgressBarX();");
                    }
                    if (dcNoteDef.Status != 0 && dcNoteDef.SettlementDate == DateTime.MinValue)
                    {
                        Button btn_VoidOtherChargeDN = (Button)e.Row.FindControl("btn_VoidOtherChargeDN");
                        btn_VoidOtherChargeDN.Visible = true;
                        string js = @"return confirm('Are you sure to proceed?');";
                        btn_VoidOtherChargeDN.OnClientClick = js;
                        btn_VoidOtherChargeDN.CommandArgument = e.Row.RowIndex.ToString();
                    }
                    //gv_Request.Columns[0].Visible = false;
                    gv_Request.Columns[9].Visible = false;      // hide the attachment column
                }
                else
                {
                    gv_Request.Columns[0].Visible = false;
                    gv_Request.Columns[9].Visible = false;      // hide the attachment column
                }

            }
        }

        private void getMockShopReport(int adjustmentNoteId, string commandName, string sampleType)
        {
            string reportFileName = (sampleType == "M" ? "MockShop" : "Studio") + "DebitNote";

            if (sampleType == "M")
            {
                if (commandName == "OutputPDF")
                    ReportHelper.export(AccountReportManager.Instance.getMockShopDebitNote(adjustmentNoteId), Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, reportFileName);
                else
                    ReportHelper.export(AccountReportManager.Instance.getMockShopDebitNote(adjustmentNoteId),
                                        HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, reportFileName);
            }
            else if (sampleType == "S")
            {
                if (commandName == "OutputPDF")
                    ReportHelper.export(AccountReportManager.Instance.getStudioDebitNote(adjustmentNoteId), Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, reportFileName);
                else
                    ReportHelper.export(AccountReportManager.Instance.getStudioDebitNote(adjustmentNoteId),
                                        HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, reportFileName);
            }
        }

        private void getUKClaimReport(int adjustmentNoteId, string commandName)
        {
            if (commandName == "OutputPDF")
                ReportHelper.export(AccountReportManager.Instance.getUKClaimDCNote(adjustmentNoteId), Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "UKClaimDebitNote");
            else
                ReportHelper.export(AccountReportManager.Instance.getUKClaimDCNote(adjustmentNoteId),
                                    HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "UKClaimDebitNote");
        }

        private void getAdvancePaymentInterestReport(int adjustmentNoteId, DateTime paymentDate, string commandName)
        {
            adjustmentNoteId = adjustmentNoteId - paymentDate.Day - paymentDate.Month - (paymentDate.Day * paymentDate.Month);
            AdvancePaymentInstalmentDetailDef detailDef = AccountManager.Instance.getAdvancePaymentInstalmentDetailByKey(adjustmentNoteId, paymentDate);
            if (commandName == "OutputPDF")
                ReportHelper.export(AccountReportManager.Instance.getAdvancePaymentInterestChargeReport(adjustmentNoteId, paymentDate), Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, detailDef.DCNoteNo);

            else if (commandName == "OutputPDFToFolder")
            {
                AdvPaymentInstalmentInterestChargesReport rpt = AccountReportManager.Instance.getAdvancePaymentInterestChargeReport(adjustmentNoteId, paymentDate);
                string dcNoteFolder = WebConfig.getValue("appSettings", "INSTALMENT_DOC_FOLDER");
                string filePath = dcNoteFolder + detailDef.DCNoteNo + ".pdf";
                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);
                rpt.Close();
                rpt.Dispose();
            }
            else
                ReportHelper.export(AccountReportManager.Instance.getAdvancePaymentInterestChargeReport(adjustmentNoteId, paymentDate),
                                    HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "AdvancePaymentInterest");
        }

        private void getUTDebitNoteReport(int adjustmentNoteId, string commandName)
        {
            if (commandName == "OutputPDF")
                ReportHelper.export(AccountReportManager.Instance.getUTDebitNote(adjustmentNoteId), Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "UTDebitNote");
            else
                ReportHelper.export(AccountReportManager.Instance.getUTDebitNote(adjustmentNoteId),
                                    HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "UTDebitNote");
        }

        private void getQADebitNoteReport(int adjustmentNoteId, string commandName)
        {
            if (commandName == "OutputPDF")
                ReportHelper.export(AccountReportManager.Instance.getQADebitNote(adjustmentNoteId), Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "QACommission");
            else
                ReportHelper.export(AccountReportManager.Instance.getQADebitNote(adjustmentNoteId),
                                    HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "QACommission");
        }

        /*
        private void getUKClaimRefundReport(int adjustmentNoteId, string commandName)
        {
            if (commandName == "OutputPDF")
                ReportHelper.export(AccountReportManager.Instance.getUKClaimRefundCreditNote(adjustmentNoteId), Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "UKClaimRefundCreditNote");
            else
                ReportHelper.export(AccountReportManager.Instance.getUKClaimRefundCreditNote(adjustmentNoteId),
                                    HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "UKClaimRefundCreditNote");
        }
        */

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            this.bindGrid();
        }

        protected void gv_Request_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int row;
            if (int.TryParse((string)e.CommandArgument, out row))
            {
                AdjustmentNoteDef def = (AdjustmentNoteDef)this.vwNoteList[int.Parse((string)e.CommandArgument)];

                if (e.CommandName == "UpdateCustomRemark")
                {
                    UKClaimDCNoteDef dcNoteDef = UKClaimManager.Instance.getUKClaimDCNoteByKey(def.AdjustmentNoteId);

                    dcNoteDef.Remark = ((TextBox)(gv_Request.Rows[row].FindControl("txt_Description"))).Text.Trim();
                    dcNoteDef.IsCustom = !(dcNoteDef.Remark == string.Empty);

                    UKClaimManager.Instance.updateUKClaimDCNote(dcNoteDef, this.LogonUserId);

                    this.btn_Search_Click(null, null);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SupplierDN", "alert('Custom Remark has been updated');", true);
                }

                else if (e.CommandName == "SendUKClaimDCNote")
                {
                    UKClaimManager.Instance.sendUKClaimDNToSupplier(def.AdjustmentNoteId);
                    this.btn_Search_Click(null, null);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SupplierDN", "alert('Mail has been sent');", true);
                }
                else if (e.CommandName == "SendPurchaseDCNote")
                {
                    if (def.MailStatus == 0)
                    {
                        AccountManager.Instance.sendPurchaseDCNoteToSupplier(def, this.getAPAdjustmentReport(def.AdjustmentNoteId, "OutputPDFToFolder"), this.LogonUserId);
                        this.btn_Search_Click(null, null);
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SupplierDCNote", "alert('Mail has been sent');", true);
                    }
                    else
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SupplierDCNote", "alert('Resend is not allow!');", true);

                }
                else if (e.CommandName == "SendGenericDCNote")
                {
                    if (def.MailStatus == 0)
                    {
                        GenericDCNoteDef dcNoteDef = AccountManager.Instance.getGenericDCNoteById(def.AdjustmentNoteId);
                        if (dcNoteDef.IssueTypeId == 1)
                        {
                            VendorRef vendor = IndustryUtil.getVendorByKey(dcNoteDef.VendorId);
                            NTVendorDef ntVendor = NonTradeManager.Instance.getNTVendorByKey(dcNoteDef.NTVendorId);
                            string email = string.Empty;
                            string vendorName = string.Empty;
                            if (vendor != null)
                            {
                                email = vendor.eAdviceAddr.Trim();
                                vendorName = vendor.Name;
                            }
                            else if (ntVendor != null)
                            {
                                email = ntVendor.EAdviceEmail.Trim();
                                vendorName = ntVendor.VendorName;
                            }

                            AccountManager.Instance.sendGenericDCNoteToParty(dcNoteDef, this.getGenericDCNoteReport(def.AdjustmentNoteId, "OutputPDFToFolder"), vendorName, email, this.LogonUserId);
                        }
                        /*
                        else if (dcNoteDef.IssueTypeId == 2)
                        {

                        }
                        */
                        else if (dcNoteDef.IssueTypeId == 3)
                        {
                            AccountManager.Instance.sendGenericDCNoteToParty(dcNoteDef, this.getGenericDCNoteReport(def.AdjustmentNoteId, "OutputPDFToFolder"), "NEXT MANUFACTURING (PVT) LTD-TESTING", "shyami@nextsl.com.lk; Sithari@NextSL.com.lk; Winnie_Lui@nextsl.com.hk; pavithrah@nextsl.com.lk", this.LogonUserId);
                        }
                        this.btn_Search_Click(null, null);
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "GenericDCNote", "alert('Mail has been sent');", true);
                    }
                    else
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "GenericDCNote", "alert('Resend is not allow!');", true);

                }
                else if (e.CommandName == "SendAdvancePaymentInterestDCNote")
                {
                    if (def.MailStatus == 0)
                    {
                        int dcNoteId = def.AdjustmentNoteId - def.CreatedOn.Day - def.CreatedOn.Month - (def.CreatedOn.Day * def.CreatedOn.Month);
                        string dcNoteFolder = WebConfig.getValue("appSettings", "INSTALMENT_DOC_FOLDER");
                        string filePath = dcNoteFolder + def.AdjustmentNoteNo + ".pdf";
                        this.getAdvancePaymentInterestReport(def.AdjustmentNoteId, def.CreatedOn, "OutputPDFToFolder");

                        AccountManager.Instance.sendAdvancePaymentInterestDCNoteToSupplier(dcNoteId, def.CreatedOn, filePath, this.LogonUserId);

                        this.btn_Search_Click(null, null);
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SupplierDCNote", "alert('Mail has been sent');", true);
                    }
                    else
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SupplierDCNote", "alert('Resend is not allow!');", true);

                }
                else if (e.CommandName == "VoidOtherChargeDN")
                {
                    GenericDCNoteDef dcNoteDef = AccountManager.Instance.getGenericDCNoteById(def.AdjustmentNoteId);
                    dcNoteDef.Status = 0;
                    string voidStr = "";
                    AccountManager.Instance.updateGenericDCNote(dcNoteDef, this.LogonUserId, out voidStr);
                    this.btn_Search_Click(null, null);
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "GenericDCNote", "alert('DN has been voided');", true);
                }
                else
                {
                    if (def.AdjustmentType.Id == AdjustmentType.SALES_ADJUSTMENT.Id)
                        this.getARAdjustmentReport(def.AdjustmentNoteId, e.CommandName);

                    if (def.AdjustmentType.Id == AdjustmentType.PURCHASE_ADJUSTMENT.Id)
                        this.getAPAdjustmentReport(def.AdjustmentNoteId, e.CommandName);

                    if (def.AdjustmentType.Id == AdjustmentType.MOCK_SHOP.Id)
                        this.getMockShopReport(def.AdjustmentNoteId, e.CommandName, "M");

                    if (def.AdjustmentType.Id == AdjustmentType.STUDIO_SAMPLE.Id)
                        this.getMockShopReport(def.AdjustmentNoteId, e.CommandName, "S");

                    if (def.AdjustmentType.Id == AdjustmentType.UKCLAIM.Id)
                        this.getUKClaimReport(def.AdjustmentNoteId, e.CommandName);

                    if (def.AdjustmentType.Id == AdjustmentType.ADVANCE_PAYMENT_INTEREST.Id)
                        this.getAdvancePaymentInterestReport(def.AdjustmentNoteId, def.CreatedOn, e.CommandName);

                    if (def.AdjustmentType.Id == AdjustmentType.OTHER_CHARGE.Id)
                        this.getGenericDCNoteReport(def.AdjustmentNoteId, e.CommandName);

                    if (def.AdjustmentType.Id == AdjustmentType.UT_DN.Id)
                        this.getUTDebitNoteReport(def.AdjustmentNoteId, e.CommandName);

                    if (def.AdjustmentType.Id == AdjustmentType.QA_COMMISSION.Id)
                        this.getQADebitNoteReport(def.AdjustmentNoteId, e.CommandName);

                    /*
                    if (def.AdjustmentType.Id == AdjustmentType.UKCLAIM_REFUND.Id)
                        this.getUKClaimRefundReport(def.AdjustmentNoteId, e.CommandName);
                    */
                }
            }
        }

        private void getARAdjustmentReport(int adjustmentNoteId, string commandName)
        {
            ARAdjustmentNoteDs arDataSet = new ARAdjustmentNoteDs();
            ARAdjustmentNoteDs ds = ReporterWorker.Instance.getARAdjustmentList(adjustmentNoteId);
            foreach (ARAdjustmentNoteDs.ARAdjustmentNoteRow r in ds.ARAdjustmentNote)
            {
                ARAdjustmentNoteDs.ARAdjustmentNoteRow newRow = arDataSet.ARAdjustmentNote.NewARAdjustmentNoteRow();
                newRow.AdjustmentNoteId = r.AdjustmentNoteId;
                newRow.AdjustmentDetailId = r.AdjustmentDetailId;
                newRow.ShipmentId = r.ShipmentId;
                newRow.SplitShipmentId = r.SplitShipmentId;
                newRow.AdjustmentNoteNo = r.AdjustmentNoteNo;
                newRow.ContractNo = r.ContractNo;
                newRow.Currency = r.Currency;
                newRow.DebitCreditIndicator = r.DebitCreditIndicator;
                newRow.DeliveryNo = r.DeliveryNo;
                newRow.InvoiceDate = r.InvoiceDate;
                newRow.InvoiceNo = r.InvoiceNo;
                newRow.IssueDate = r.IssueDate;
                newRow.ItemNo = r.ItemNo;
                newRow.MasterDebitCreditIndicator = r.MasterDebitCreditIndicator;
                newRow.NSLAmt = r.NSLAmt;
                newRow.NUKAmt = r.NUKAmt;
                newRow.OrderRef = r.OrderRef;
                newRow.PartyName = r.PartyName;
                newRow.PartyAddress1 = r.IsPartyAddress1Null() ? String.Empty : r.PartyAddress1;
                newRow.PartyAddress2 = r.IsPartyAddress2Null() ? String.Empty : r.PartyAddress2;
                newRow.PartyAddress3 = r.IsPartyAddress3Null() ? String.Empty : r.PartyAddress3;
                newRow.PartyAddress4 = r.IsPartyAddress4Null() ? String.Empty : r.PartyAddress4;
                arDataSet.ARAdjustmentNote.AddARAdjustmentNoteRow(newRow);
            }
            ARAdjustmentNote report = new ARAdjustmentNote();
            report.SetDataSource(arDataSet);
            report.SetParameterValue("IsDraft", "N");
            if (commandName == "OutputPDF")
                ReportHelper.export(report, Response,
                        CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "A/R Adjustment");
            else
                ReportHelper.export(report,
                                    HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "A/R Adjustment");

        }

        private string getGenericDCNoteReport(int adjustmentNoteId, string commandName)
        {
            GenericDCNoteDef def = AccountManager.Instance.getGenericDCNoteById(adjustmentNoteId);
            List<string> dcNoteNoList = new List<string>();
            dcNoteNoList.Add(def.DCNoteNo);
            GenericDCNoteReport report = null;

            string outputFilePath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + dcNoteNoList[0] + ".pdf";
            using (report = report = AccountReportManager.Instance.getGenericDCNoteReport(dcNoteNoList))
            {
                if (commandName == "OutputPDF")
                    ReportHelper.export(report, Response,
                            CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "Other Charge");
                else if (commandName == "OutputExcel")
                    ReportHelper.export(report,
                                        HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "Other Charge");
                else if (commandName == "OutputPDFToFolder")
                {
                    report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, outputFilePath);
                    report.Close();
                    report.Dispose();
                }
            }
            return outputFilePath;
        }


        private string getAPAdjustmentReport(int adjustmentNoteId, string commandName)
        {
            int officeId = int.MinValue;
            int currencyId = int.MinValue;
            int tradingAgencyId = int.MinValue;
            string filePath = string.Empty;
            /*
            int adjNoteId = int.MinValue;
            string adjNoteNo = string.Empty;
            DateTime issueDate = DateTime.MinValue;
            */

            APAdjustmentNoteDs apDataSet = new APAdjustmentNoteDs();
            APAdjustmentNoteDs ds = ReporterWorker.Instance.getAPAdjustmentList(adjustmentNoteId);
            foreach (APAdjustmentNoteDs.APAdjustmentNoteRow r in ds.APAdjustmentNote)
            {
                APAdjustmentNoteDs.APAdjustmentNoteRow newRow = apDataSet.APAdjustmentNote.NewAPAdjustmentNoteRow();
                newRow.AdjustmentNoteId = r.AdjustmentNoteId;
                newRow.AdjustmentDetailId = r.AdjustmentDetailId;
                newRow.ShipmentId = r.ShipmentId;
                newRow.SplitShipmentId = r.SplitShipmentId;
                newRow.AdjustmentNoteNo = r.AdjustmentNoteNo;
                newRow.ContractNo = r.ContractNo;
                newRow.CurrencyId = r.CurrencyId;
                newRow.CurrencyCode = CurrencyId.getName(r.CurrencyId);
                newRow.OfficeId = r.OfficeId;
                newRow.TradingAgencyId = r.TradingAgencyId;
                newRow.DebitCreditIndicator = r.DebitCreditIndicator;
                newRow.DeliveryNo = r.DeliveryNo;
                newRow.InvoiceDate = r.InvoiceDate;
                newRow.InvoiceNo = r.InvoiceNo;
                newRow.IssueDate = r.IssueDate;
                newRow.ItemNo = r.ItemNo;
                newRow.MasterDebitCreditIndicator = r.MasterDebitCreditIndicator;
                newRow.RevisedAmt = r.RevisedAmt;
                newRow.Amt = r.Amt;
                newRow.PackingUnit = r.PackingUnit;
                newRow.PartyName = r.PartyName;
                newRow.PartyAddress1 = r.IsPartyAddress1Null() ? String.Empty : r.PartyAddress1;
                newRow.PartyAddress2 = r.IsPartyAddress2Null() ? String.Empty : r.PartyAddress2;
                newRow.PartyAddress3 = r.IsPartyAddress3Null() ? String.Empty : r.PartyAddress3;
                newRow.PartyAddress4 = r.IsPartyAddress4Null() ? String.Empty : r.PartyAddress4;
                newRow.Size = r.Size;
                newRow.SellingPrice = r.SellingPrice;
                newRow.NetFOBPrice = r.NetFOBPrice;
                newRow.SupplierGmtPrice = r.SupplierGmtPrice;
                newRow.ShippedQty = r.ShippedQty;
                newRow.RevisedSellingPrice = r.RevisedSellingPrice;
                newRow.RevisedNetFOBPrice = r.RevisedNetFOBPrice;
                newRow.RevisedSupplierGmtPrice = r.RevisedSupplierGmtPrice;
                newRow.RevisedShippedQty = r.RevisedShippedQty;
                newRow.QACommissionPercent = r.QACommissionPercent;
                newRow.QACommissionAmt = r.QACommissionAmt;
                newRow.VendorPaymentDiscountPercent = r.VendorPaymentDiscountPercent;
                newRow.VendorPaymentDiscountAmt = r.VendorPaymentDiscountAmt;
                newRow.LabTestIncome = r.LabTestIncome;
                apDataSet.APAdjustmentNote.AddAPAdjustmentNoteRow(newRow);

                officeId = r.OfficeId;
                currencyId = r.CurrencyId;
                tradingAgencyId = r.TradingAgencyId;
                /*
                issueDate = r.IssueDate;
                adjNoteNo = r.AdjustmentNoteNo;
                adjNoteId = r.AdjustmentNoteId;
                */
            }

            APAdjustmentNoteDs.BenificiaryAccountRow row = apDataSet.BenificiaryAccount.NewBenificiaryAccountRow();
            DebitNoteToNUKParamDef paramDef = ReporterWorker.Instance.getDebitNoteToNUKParamByKey(officeId, currencyId);
            row.BenificiaryAccountNo = ReporterWorker.Instance.getBeneficiaryAccountNoList(officeId, currencyId);
            row.SupplierCode = paramDef.SupplierCode;
            row.BeneficiaryName = paramDef.BeneficiaryName;
            row.BankName = paramDef.BankName;
            row.BankAddress = paramDef.BankAddress;
            row.SwiftCode = paramDef.SwiftCode;
            row.CurrencyId = currencyId;
            row.OfficeId = officeId;
            row.CurrencyCode = GeneralWorker.Instance.getCurrencyByKey(currencyId).CurrencyCode;
            row.OfficeName = GeneralWorker.Instance.getOfficeRefByKey(officeId).Description.Replace("Office", string.Empty);
            apDataSet.BenificiaryAccount.AddBenificiaryAccountRow(row);

            APAdjustmentNote report;
            using (report = new APAdjustmentNote())
            {
                report.SetDataSource(apDataSet);
                report.SetParameterValue("IsDraft", "N");

                if (commandName == "OutputPDF")
                    ReportHelper.export(report, Response,
                            CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "A/P Adjustment");
                else if (commandName == "OutputExcel")
                    ReportHelper.export(report,
                                        HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "A/P Adjustment");
                else if (commandName == "OutputPDFToFolder")
                {
                    string dcNoteFolder = WebConfig.getValue("appSettings", "APDCNOTE_OUTPUT_FOLDER");
                    filePath = dcNoteFolder + "\\" + ((APAdjustmentNoteDs.APAdjustmentNoteRow)apDataSet.APAdjustmentNote.Rows[0]).AdjustmentNoteNo.Replace("/", "-") + ".pdf";
                    report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);
                }
                report.Close();
            }
            return filePath;
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

        protected void btn_Send_Click(object sender, EventArgs e)
        {
            int adjustmentType = int.Parse(ddl_AdjustmentType.SelectedValue);
            List<int> list = new List<int>();
            for (int i = 0; i < gv_Request.Rows.Count; i++)
            {
                CheckBox chckrw = (CheckBox)gv_Request.Rows[i].FindControl("chb_Mail");
                if (chckrw.Checked && chckrw.Visible)
                {
                    AdjustmentNoteDef def = (AdjustmentNoteDef)this.vwNoteList[i];
                    list.Add(def.AdjustmentNoteId);
                }
            }

            if (adjustmentType != AdjustmentType.OTHER_CHARGE.Id)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    UKClaimManager.Instance.sendUKClaimDNToSupplier(list[j]);
                }
            }
            else
            {
                for (int i = 0; i < gv_Request.Rows.Count; i++)
                {
                    CheckBox chckrw = (CheckBox)gv_Request.Rows[i].FindControl("chb_Mail");
                    if (chckrw.Checked && chckrw.Visible)
                    {
                        AdjustmentNoteDef def = (AdjustmentNoteDef)this.vwNoteList[i];
                        GenericDCNoteDef genericDef = AccountManager.Instance.getGenericDCNoteById(def.AdjustmentNoteId);

                        if (genericDef.IssueTypeId == 1)
                        {
                            VendorRef vendor = IndustryUtil.getVendorByKey(def.VendorId);
                            AccountManager.Instance.sendGenericDCNoteToParty(genericDef, this.getGenericDCNoteReport(def.AdjustmentNoteId, "OutputPDFToFolder"), vendor.Name, vendor.eAdviceAddr.Trim(), this.LogonUserId);
                        }
                        /*
                        else if (dcNoteDef.IssueTypeId == 2)
                        {

                        }
                        */
                        else if (genericDef.IssueTypeId == 3)
                        {
                            AccountManager.Instance.sendGenericDCNoteToParty(genericDef, this.getGenericDCNoteReport(def.AdjustmentNoteId, "OutputPDFToFolder"), "NEXT MANUFACTURING (PVT) LTD-TESTING", "Iroshini@nextmfg.lk; shyami@nextsl.com.lk; Ranjith@nextmfg.lk; Sithari@NextSL.com.lk", this.LogonUserId);
                        }
                    }
                }
            }

            this.btn_Search_Click(null, null);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SupplierDN", "alert('Mail has been sent');", true);
        }

    }
}
