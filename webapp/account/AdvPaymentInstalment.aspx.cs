using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.isam.domain.account;
using com.next.isam.appserver.account;
using com.next.common.web.commander;
using com.next.isam.webapp.webservices;
using com.next.infra.util;
using com.next.infra.smartwebcontrol;
using System.Text.RegularExpressions;
using com.next.infra.web;
using com.next.isam.webapp.commander.account;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain;
using com.next.common.domain.module;

namespace com.next.isam.webapp.account
{
    public partial class AdvPaymentInstalment : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private GeneralWorker generalWorker =  GeneralWorker.Instance;
        private decimal lineTotalAmt = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) 
            {
                int paymentId = -1;
                string strDCNoteNo = string.Empty;

                this.txt_InitiatedBy.setWidth(300);
                this.txt_InitiatedBy.initControl(webservices.UclSmartSelection.SelectionList.user);

                if (vwAdvancePayment != null)
                {
                    paymentId = this.vwAdvancePayment.PaymentId;
                }
                else if (Request.Params["PaymentId"] != null)
                {
                    string strPaymentId = HttpUtility.UrlDecode(EncryptionUtility.DecryptParam(Request.Params["PaymentId"].ToString()));
                    if (Request.Params["dcNo"] != null)
                        strDCNoteNo = Request.Params["dcNo"].ToString();

                    if (strPaymentId != "-1")
                    {
                        paymentId = int.Parse(strPaymentId);
                        FillData(paymentId);
                        btnAttachment.Visible = true;
                        btnLog.Visible = true;
                    }
                    else 
                    {
                        NewData();

                        lblPaymentNo.Text = "<b>New</b>";
                        btnAttachment.Visible = false;
                        btnLog.Visible = false;
                    }
                }
                this.initControls();
                this.btnLog.Attributes.Add("onclick", "window.open('ViewAdvancePaymentLog.aspx?paymentId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(paymentId.ToString())) + "', 'loglist', 'width=800,height=400,scrollbars=1,status=0');return false;");

                if (strDCNoteNo != string.Empty)

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Advance Payment", "window.open('../reporter/tmpreport/" + strDCNoteNo + ".pdf','_blank');", true);
            }
        }

        private void initControls()
        {
            bool isEnabled = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.advancePayment.Id, ISAMModule.advancePayment.Super);
            this.ddlOffice.Enabled = isEnabled;
            this.txt_Supplier.Enabled = isEnabled;
            this.txtOverallDate.Enabled = isEnabled;
            this.txt_PayableTo.Enabled = isEnabled;
            this.txtTotalPaymentAmt.Enabled = isEnabled;
            this.ddlCurrency.Enabled = isEnabled;
            this.txtInterestChargedAmt.Enabled = isEnabled;
            this.txtInterestRate.Enabled = isEnabled;
            this.txt_InitiatedBy.Enabled = isEnabled;
        }

        protected void NewData() 
        {
            BasicData();
            for (int i = 0; i < 5; i++)
            {
                vwInstalments.Add(new AdvancePaymentInstalmentDetailDef());
            }
            rep_InstalmentDetail.DataSource = vwInstalments;
            rep_InstalmentDetail.DataBind();
        }

        protected void BasicData() 
        {
            ddlOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--PLEASE SELECT--", GeneralCriteria.ALL.ToString());
            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(UclSmartSelection.SelectionList.garmentVendor);
            ddlCurrency.bindList(CommonUtil.getCurrencyList(), "CurrencyCode", "CurrencyId", GeneralCriteria.ALL.ToString(), "--PLEASE SELECT--", GeneralCriteria.ALL.ToString());
        }

        protected void FillData(int paymentId)
        {
            BasicData();
            
            this.vwAdvancePayment = AccountManager.Instance.getAdvancePaymentByKey(paymentId);

            lblPaymentNo.Text = vwAdvancePayment.PaymentNo;          
            ddlOffice.SelectedValue = vwAdvancePayment.OfficeId.ToString();
            txt_Supplier.VendorId = vwAdvancePayment.Vendor.VendorId;
            txtOverallDate.DateTime = vwAdvancePayment.PaymentDate;
            txt_PayableTo.Text = vwAdvancePayment.PayableTo; 
            ddlCurrency.SelectedValue = vwAdvancePayment.Currency.CurrencyId.ToString();
            txtTotalPaymentAmt.Text = (vwAdvancePayment.TotalAmount - vwAdvancePayment.InterestChargedAmt).ToString();
            txtInterestChargedAmt.Text = vwAdvancePayment.InterestChargedAmt.ToString();
            lblTotalAmt.Text = vwAdvancePayment.TotalAmount.ToString("#,##0.00");
            txtInterestRate.Text = vwAdvancePayment.InterestRate.ToString();
            txtRemark.Text = vwAdvancePayment.Remark;
            if (vwAdvancePayment.InitiatedBy != null)
                txt_InitiatedBy.UserId = vwAdvancePayment.InitiatedBy.UserId;
            this.vwInstalments = AccountManager.Instance.getAdvancePaymentInstalmentDetailList(vwAdvancePayment.PaymentId);
            rep_InstalmentDetail.DataSource = vwInstalments;
            rep_InstalmentDetail.DataBind();
            lbl_balDiff.Text = BalDiff.ToString("#,##0.00");
        }

        protected void btn_AddInstalment_Click(object sender, EventArgs e)
        {
            updateInstalmentDetail();

            vwInstalments.Add(new AdvancePaymentInstalmentDetailDef());
            rep_InstalmentDetail.DataSource = vwInstalments;
            rep_InstalmentDetail.DataBind();
        }

        protected void btn_RemoveInstalment_Click(object sender, EventArgs e)
        {
            int count = 0;
            foreach (AdvancePaymentInstalmentDetailDef instalSin in vwInstalments) 
            {
                if (instalSin.Status == 1)
                    count++;
            }
            if(count == 1)
            {
                messageScript("At least one payment instalment is required");
                return;
            }
            int index = Convert.ToInt32(((ImageButton)sender).CommandArgument);
            vwInstalments[index].Status = 0;
            rep_InstalmentDetail.Items[index].Visible = false;
        }

        protected void rep_InstalmentDetail_DataBound(object sender, RepeaterItemEventArgs e)
        {
            bool isEnabled = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.advancePayment.Id, ISAMModule.advancePayment.Super);

            if (e.Item.ItemType == ListItemType.Header)
                ((ImageButton)e.Item.FindControl("btn_AddInstalment")).Visible = isEnabled;

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                AdvancePaymentInstalmentDetailDef instalDetail = (AdvancePaymentInstalmentDetailDef)e.Item.DataItem;
                ((Label)e.Item.FindControl("lbl_Seq")).Text = (e.Item.ItemIndex + 1).ToString();
                if (instalDetail.Status == 0)
                    e.Item.Visible = false;

                if (instalDetail.PaymentId == 0 && instalDetail.ExpectedAmount == 0)
                    ((TextBox)e.Item.FindControl("txtExpectedAmt")).Text = string.Empty;

                if (instalDetail.PaymentAmount == 0) 
                    ((TextBox)e.Item.FindControl("txtPayAmt")).Text = string.Empty;
                ((SmartCalendar)e.Item.FindControl("txtPayDate")).DateTime = instalDetail.PaymentDate;
                if (instalDetail.SettlementDate != DateTime.MinValue)
                    ((SmartCalendar)e.Item.FindControl("txtSettleDate")).DateTime = instalDetail.SettlementDate;
                ((TextBox)e.Item.FindControl("txtExpectedAmt")).Enabled = isEnabled;
                ((TextBox)e.Item.FindControl("txtPayAmt")).Enabled = isEnabled;
                ((TextBox)e.Item.FindControl("txtRemark")).Enabled = isEnabled;
                ((SmartCalendar)e.Item.FindControl("txtPayDate")).Enabled = isEnabled;
                ((SmartCalendar)e.Item.FindControl("txtSettleDate")).Enabled = isEnabled;
            }
        }

        protected void updateInstalmentDetail()
        {
            lineTotalAmt = 0;
            AdvancePaymentInstalmentDetailDef instalDef;
            foreach (RepeaterItem item in rep_InstalmentDetail.Items)
            {
                instalDef = vwInstalments[item.ItemIndex];
                if (vwAdvancePayment != null)
                {
                    if (vwAdvancePayment.PaymentId != 0)
                    {
                        instalDef.PaymentId = vwAdvancePayment.PaymentId;
                    }
                }
                SmartCalendar payDate = ((SmartCalendar)item.FindControl("txtPayDate"));
                instalDef.PaymentDate = payDate.DateTime;
                decimal payAmt = 0;
                decimal expectedAmt = 0;
                if (((TextBox)item.FindControl("txtPayAmt")).Text.Trim() != "") 
                {
                   payAmt = decimal.Parse(((TextBox)item.FindControl("txtPayAmt")).Text.Trim());
                }
                instalDef.PaymentAmount = payAmt;

                if (((TextBox)item.FindControl("txtExpectedAmt")).Text.Trim() != "")
                {
                    expectedAmt = decimal.Parse(((TextBox)item.FindControl("txtExpectedAmt")).Text.Trim());
                }
                instalDef.ExpectedAmount = expectedAmt;
                lineTotalAmt += expectedAmt;

                instalDef.Remark = ((TextBox)item.FindControl("txtRemark")).Text.Trim();
                SmartCalendar settleDate = ((SmartCalendar)item.FindControl("txtSettleDate"));
                instalDef.SettlementDate = new DateTime(settleDate.DateTime.Year, settleDate.DateTime.Month, settleDate.DateTime.Day);

                instalDef.Status = vwInstalments[item.ItemIndex].Status;
                vwInstalments[item.ItemIndex] = instalDef;
            }
        }

        private AdvancePaymentDef vwAdvancePayment
        {
            get {return (AdvancePaymentDef)ViewState["vwAdvancePayment"]; }
            set { ViewState["vwAdvancePayment"] = value; }
        }

        private List<AdvancePaymentInstalmentDetailDef> vwInstalments
        {
            get
            {
                if (ViewState["vwInstalments"] == null)
                    ViewState["vwInstalments"] = new List<AdvancePaymentInstalmentDetailDef>();

                return (List<AdvancePaymentInstalmentDetailDef>)ViewState["vwInstalments"];
            }
            set { ViewState["vwInstalments"] = value; }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (button_start(sender))
            {
                return;
            }

            updateInstalmentDetail();

            List<AdvancePaymentActionHistoryDef> historys = new List<AdvancePaymentActionHistoryDef>();
            List<AdvancePaymentInstalmentDetailDef> instalmentModified = new List<AdvancePaymentInstalmentDetailDef>();
            string redirectURL = HttpContext.Current.Request.Url.AbsoluteUri;
            string savenewMess = "";
            if (vwAdvancePayment != null)
            {
                savenewMess = "Save Successfully!";
                AdvancePaymentDef oriPayment = AccountManager.Instance.getAdvancePaymentByKey(vwAdvancePayment.PaymentId);
                int officeRev = int.Parse(ddlOffice.SelectedValue);
                List<AdvancePaymentInstalmentDetailDef> oriInstal = AccountManager.Instance.getAdvancePaymentInstalmentDetailList(vwAdvancePayment.PaymentId);
                int vendorRev = txt_Supplier.VendorId;
                int currRev = int.Parse(ddlCurrency.SelectedValue);
                decimal totalRev = decimal.Parse(txtTotalPaymentAmt.Text.Trim());
                decimal interestChargedRev = decimal.Parse(txtInterestChargedAmt.Text.Trim());
                decimal interestRateRev = decimal.Parse(txtInterestRate.Text.Trim());
                string remarkRev = txtRemark.Text.Trim();
                DateTime paymentDateRev = txtOverallDate.DateTime;
                string payableTo = txt_PayableTo.Text.Trim();
                int change = 0;
                vwAdvancePayment.InitiatedBy = CommonUtil.getUserByKey(this.txt_InitiatedBy.UserId);
                change = 1;

                if (oriPayment.OfficeId != officeRev)
                {
                    vwAdvancePayment.OfficeId = officeRev;
                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "OFFICE: " + generalWorker.getOfficeRefByKey(oriPayment.OfficeId).OfficeCode + " --> " + generalWorker.getOfficeRefByKey(officeRev).OfficeCode));
                    change = 1;
                }

                if (oriPayment.Vendor.VendorId != vendorRev)
                {
                    vwAdvancePayment.Vendor.VendorId = vendorRev;
                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "VENDOR: " + VendorWorker.Instance.getVendorByKey(oriPayment.Vendor.VendorId).Name + " --> " + VendorWorker.Instance.getVendorByKey(vendorRev).Name));
                    change = 1;
                }
                if (oriPayment.PaymentDate != paymentDateRev)
                {
                    vwAdvancePayment.PaymentDate = paymentDateRev;
                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "OVERALL PAYMENT DATE: " + DateTimeUtility.getDateString(oriPayment.PaymentDate) + " --> " + DateTimeUtility.getDateString(paymentDateRev)));
                    change = 1;
                }
                if (oriPayment.Currency.CurrencyId != currRev)
                {
                    vwAdvancePayment.Currency.CurrencyId = currRev;
                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "CURRENCY: " + generalWorker.getCurrencyByKey(oriPayment.Currency.CurrencyId).CurrencyCode + " --> " + generalWorker.getCurrencyByKey(currRev).CurrencyCode));
                    change = 1;
                }
                if (oriPayment.InterestChargedAmt != interestChargedRev)
                {
                    vwAdvancePayment.InterestChargedAmt = interestChargedRev;
                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "INTEREST CHARGED AMT: " + Decimal.Round(oriPayment.InterestChargedAmt, 2) + " --> " + Decimal.Round(interestChargedRev, 2)));
                    change = 1;
                }
                if (oriPayment.InterestRate != interestRateRev)
                {
                    vwAdvancePayment.InterestRate = interestRateRev;
                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "INTEREST RATE: " + Decimal.Round(oriPayment.InterestRate, 2) + " --> " + Decimal.Round(interestRateRev, 2)));
                    change = 1;
                }
                if ((oriPayment.TotalAmount + oriPayment.InterestChargedAmt) != (totalRev + interestChargedRev))
                {
                    vwAdvancePayment.TotalAmount = totalRev + interestChargedRev;
                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "TOTAL AMT: " + Decimal.Round(oriPayment.TotalAmount + oriPayment.InterestChargedAmt, 2) + " --> " + Decimal.Round(totalRev + interestChargedRev, 2)));
                    change = 1;
                }
                if (oriPayment.Remark != remarkRev)
                {
                    vwAdvancePayment.Remark = remarkRev;
                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "REMARK: " + oriPayment.Remark + " --> " + remarkRev));
                    change = 1;
                }
                if (oriPayment.PayableTo != payableTo)
                {
                    vwAdvancePayment.PayableTo = payableTo;
                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "PAYABLE TO: " + oriPayment.PayableTo + " --> " + payableTo));
                    change = 1;
                }

                foreach (AdvancePaymentInstalmentDetailDef instalSin in vwInstalments)
                {
                    if (instalSin.PaymentDate != DateTime.MinValue)
                    {
                        AdvancePaymentInstalmentDetailDef searchResult = AccountManager.Instance.getAdvancePaymentInstalmentDetailByKey(instalSin.PaymentId, instalSin.PaymentDate);
                        if (searchResult != null)
                        {
                            if (instalSin.Status == 0 && searchResult.Status == 1)
                            {
                                instalmentModified.Add(instalSin);
                                historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "REMOVE INSTALMENT: " + DateTimeUtility.getDateString(instalSin.PaymentDate) + " WITH AMT " + Decimal.Round(searchResult.PaymentAmount, 2)));
                                change = 1;
                            }
                            else if (instalSin.Status == 1 && searchResult.Status == 0)
                            {
                                instalmentModified.Add(instalSin);
                                historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "RECOVER INSTALMENT: " + DateTimeUtility.getDateString(instalSin.PaymentDate) + " WITH AMT " + Decimal.Round(instalSin.PaymentAmount, 2)));
                                change = 1;
                            }
                            else if (instalSin.Status == searchResult.Status)
                            {
                                int rowchange = 0;
                                if (instalSin.ExpectedAmount != searchResult.ExpectedAmount)
                                {
                                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "MODIFY INSTALMENT: " + DateTimeUtility.getDateString(instalSin.PaymentDate) + " WITH EXPECTED AMT " + Decimal.Round(searchResult.ExpectedAmount, 2) + " --> " + Decimal.Round(instalSin.ExpectedAmount, 2)));
                                    rowchange = 1;
                                    change = 1;
                                }

                                if (instalSin.PaymentAmount != searchResult.PaymentAmount)
                                {
                                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "MODIFY INSTALMENT: " + DateTimeUtility.getDateString(instalSin.PaymentDate) + " WITH AMT " + Decimal.Round(searchResult.PaymentAmount, 2) + " --> " + Decimal.Round(instalSin.PaymentAmount, 2)));
                                    rowchange = 1;
                                    change = 1;
                                }
                                if (instalSin.Remark != searchResult.Remark)
                                {
                                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "MODIFY INSTALMENT: " + DateTimeUtility.getDateString(instalSin.PaymentDate) + " WITH REMARK '" + searchResult.Remark + "'" + " --> '" + instalSin.Remark + "'"));
                                    rowchange = 1;
                                    change = 1;
                                }
                                if (instalSin.SettlementDate != searchResult.SettlementDate)
                                {
                                    historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "MODIFY INSTALMENT: " + DateTimeUtility.getDateString(instalSin.PaymentDate) + " WITH DEDUCTION DATE '" + (searchResult.SettlementDate == DateTime.MinValue ? string.Empty : DateTimeUtility.getDateString(searchResult.SettlementDate)) + "' --> '" + (instalSin.SettlementDate == DateTime.MinValue ? string.Empty : DateTimeUtility.getDateString(instalSin.SettlementDate)) + "'"));
                                    rowchange = 1;
                                    change = 1;
                                }
                                if (rowchange == 1) 
                                {
                                    instalmentModified.Add(instalSin);
                                }
                            }
                        }
                        else
                        {
                            instalmentModified.Add(instalSin);
                            historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "ADD INSTALMENT: " + DateTimeUtility.getDateString(instalSin.PaymentDate) + " WITH EXPECTED AMT/AMT " + Decimal.Round(instalSin.ExpectedAmount, 2) + "/" + Decimal.Round(instalSin.PaymentAmount, 2)));
                            change = 1;
                        }
                    }
                }
                foreach (AdvancePaymentInstalmentDetailDef instalSin in oriInstal)
                {
                    AdvancePaymentInstalmentDetailDef searchResult = vwInstalments.Find(item=>item.PaymentDate==instalSin.PaymentDate);
                    if (searchResult == null) 
                    {
                        instalSin.Status = 0;
                        instalmentModified.Add(instalSin);
                        historys.Add(NewAdvancePaymentActionHistory(vwAdvancePayment.PaymentId, "REMOVE INSTALMENT: " + DateTimeUtility.getDateString(instalSin.PaymentDate) + " WITH AMT " + Decimal.Round(instalSin.PaymentAmount, 2)));
                        change = 1;
                    }
                }
                if (change == 1)
                    UpdateAdvancePaymentAndInstalment(vwAdvancePayment, instalmentModified, historys);
            }
            else 
            {
                savenewMess = "Create Successfully!";
                int officeNew = int.Parse(ddlOffice.SelectedValue);
                int vendorNew = txt_Supplier.VendorId;
                int currNew = int.Parse(ddlCurrency.SelectedValue);
                decimal totalNew = decimal.Parse(txtTotalPaymentAmt.Text.Trim());
                decimal interestChargedAmtNew = decimal.Parse(txtInterestChargedAmt.Text.Trim());
                decimal interestRateNew = decimal.Parse(txtInterestRate.Text.Trim());
                string remarkNew = txtRemark.Text.Trim();
                DateTime paymentDateNew = txtOverallDate.DateTime;
                AdvancePaymentDef newPayment = new AdvancePaymentDef();
                newPayment.OfficeId = officeNew;
                newPayment.Vendor = VendorWorker.Instance.getVendorByKey(vendorNew);
                newPayment.PaymentDate = paymentDateNew;
                newPayment.PayableTo = txt_PayableTo.Text.Trim();
                newPayment.Currency = generalWorker.getCurrencyByKey(currNew);
                newPayment.TotalAmount = totalNew + interestChargedAmtNew;
                newPayment.InterestChargedAmt = interestChargedAmtNew;
                newPayment.InterestRate = interestRateNew;
                newPayment.InitiatedBy = CommonUtil.getUserByKey(txt_InitiatedBy.UserId);
                newPayment.Remark = remarkNew;
                newPayment.PaymentTypeId = 2;
                newPayment.SubmittedBy = generalWorker.getUserByKey(this.LogonUserId);
                newPayment.Status = 1;
                newPayment.SubmittedOn = DateTime.Now;
                newPayment.WorkflowStatusId = 2;
                vwAdvancePayment = newPayment;

                historys.Add(NewAdvancePaymentActionHistory(0, "NEW INSTALMENT PAYMENT ENTRY"));

                foreach(AdvancePaymentInstalmentDetailDef instalSin in vwInstalments)
                {
                    if (instalSin.Status == 1) 
                    {
                        instalmentModified.Add(instalSin);
                    }
                }

                UpdateAdvancePaymentAndInstalment(vwAdvancePayment, instalmentModified, historys);
                int cutPos = redirectURL.IndexOf("?PaymentId");
                redirectURL = redirectURL.Substring(0, cutPos);
                redirectURL +=  "?PaymentId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(vwAdvancePayment.PaymentId.ToString()));
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "viewDoc", "alert('" + savenewMess + "'); window.open('" + redirectURL + "','_self');", true);
        }

        private bool button_start(object sender)
        {
            bool error = false;
            string newLine = "\\n";
            string message = "Below inputs are not valid"+newLine;
            if (ddlOffice.SelectedIndex == 0) 
            {
                message += "Office [Missing]" + newLine;
                error = true;
            }
            if (txt_Supplier.VendorId == int.MinValue) 
            {
                message += "Vendor [Missing]" + newLine;
                error = true;
            }
            if (this.txt_InitiatedBy.UserId <= 0)
            {
                message += "Initiated By [Missing]" + newLine;
                error = true;
            }

            if (txtOverallDate.DateTime == DateTime.MinValue) 
            {
                message += "Overall Deduction Date [Missing]" + newLine;
                error = true;
            }
            if (ddlCurrency.SelectedIndex == 0)
            {
                message += "Currency [Missing]" + newLine;
                error = true;
            }
            decimal total = 0;
            if (txtTotalPaymentAmt.Text.Trim() == "")
            {
                message += "Total Deduction Amount [Missing]" + newLine;
                error = true;
            }
            else 
            {
                if (!decimal.TryParse(txtTotalPaymentAmt.Text.Trim(), out total))
                {
                    message += "Total Deduction Amount [Invalid decimal format]" + newLine;
                    error = true;
                }
                else
                {
                    if (total < 0)
                    {
                        message += "Total Deduction Amount [Negative Total Amount]" + newLine;
                        error = true;
                    }
                    total = decimal.Parse(txtTotalPaymentAmt.Text.Trim());
                }
            }
            decimal interestAmt = 0;
            if (this.txtInterestChargedAmt.Text.Trim() != "")
            {
                if (!decimal.TryParse(txtInterestChargedAmt.Text.Trim(), out interestAmt))
                {
                    message += "Interest Charged Amount [Invalid decimal format]" + newLine;
                    error = true;
                }
                else
                {
                    if (interestAmt < 0)
                    {
                        message += "Interest Charged Amount [Negative Interest Charged Amount]" + newLine;
                        error = true;
                    }
                    interestAmt = decimal.Parse(txtInterestChargedAmt.Text.Trim());
                }
            }
            decimal interestRate = 0;
            if (this.txtInterestRate.Text.Trim() != "")
            {
                if (!decimal.TryParse(txtInterestRate.Text.Trim(), out interestRate))
                {
                    message += "Interest Rate % [Invalid decimal format]" + newLine;
                    error = true;
                }
                else
                {
                    if (interestRate < 0)
                    {
                        message += "Interest Rate % [Negative Interest Rate]" + newLine;
                        error = true;
                    }
                    interestRate = decimal.Parse(txtInterestRate.Text.Trim());
                }
            }
            /*
            if (txtRemark.Text.Trim() != "")
            {
                string regex = @"^[\w\s-+:?&%!\\()-/'$#@=|\n|\r|\t]{1,50}$";
                Regex specialChara = new Regex(regex);
                if (!specialChara.IsMatch(txtRemark.Text))
                {
                    message += "Remarks [Contains special characters]" + newLine;
                    error = true;
                }
            }
            */
            AdvancePaymentInstalmentDetailDef instalDef;
            int i = 0;
            decimal totalCal = 0;
            List<DateTime> paymentDates = new List<DateTime>();
            foreach (RepeaterItem item in rep_InstalmentDetail.Items)
            {
                instalDef = vwInstalments[item.ItemIndex];
                if (instalDef.Status == 1) 
                {
                    i++;
                    SmartCalendar payDate = ((SmartCalendar)item.FindControl("txtPayDate"));
                    if (payDate.DateTime == DateTime.MinValue)
                    {
                        message += "Payment Instalments Line " + i + " [Empty Expected Date]" + newLine;
                        error = true;
                    }
                    else 
                    {
                        if (paymentDates.Contains(payDate.DateTime))
                        {
                            message += "Payment Instalments Line " + i + " [Repeated Expected Date]" + newLine;
                            error = true;
                        }
                        else 
                        {
                            paymentDates.Add(payDate.DateTime);
                        }
                    }
                    if (((TextBox)item.FindControl("txtPayAmt")).Text.Trim() == "")
                    {
                        message += "Payment Instalments Line " + i + " [Empty Deduction Amount]" + newLine;
                        error = true;
                    }
                    else 
                    {
                        string singleAmt = ((TextBox)item.FindControl("txtPayAmt")).Text.Trim();
                        decimal check_decimal = 0;
                        if (!decimal.TryParse(singleAmt, out check_decimal))
                        {
                            message += "Payment Instalments Line " + i + " [Invalid Decimal Format]" + newLine;
                            error = true;
                        }
                        else
                        {
                            if (check_decimal == 0)
                            {
                                message += "Payment Instalments Line " + i + " [Zero Deduction Amount]" + newLine;
                                error = true;
                            }
                            //totalCal += decimal.Parse(singleAmt);
                        }
                    }

                    if (((TextBox)item.FindControl("txtExpectedAmt")).Text.Trim() == "")
                    {
                        message += "Payment Instalments Line " + i + " [Empty Expected Amount]" + newLine;
                        error = true;
                    }
                    else
                    {
                        string singleAmt = ((TextBox)item.FindControl("txtExpectedAmt")).Text.Trim();
                        decimal check_decimal = 0;
                        if (!decimal.TryParse(singleAmt, out check_decimal))
                        {
                            message += "Payment Instalments Line " + i + " [Invalid Decimal Format]" + newLine;
                            error = true;
                        }
                        else
                        {
                            /*
                            if (check_decimal == 0)
                            {
                                message += "Payment Instalments Line " + i + " [Zero Expected Amount]" + newLine;
                                error = true;
                            }
                            */

                            totalCal += decimal.Parse(singleAmt);
                        }
                    }

                }
            }

            this.lblTotalAmt.Text = (total + interestAmt).ToString("#,##0.00");

            if ((total + interestAmt) != totalCal) 
            {
                message += "Total amount (included interest charge) is not match with overall expected payment instalments amount" + newLine;
                error = true;
            }
            if (error == true) 
            {
                messageScript(message);
            }
            return error;
        }

        private void UpdateAdvancePaymentAndInstalment(AdvancePaymentDef payments, List<AdvancePaymentInstalmentDetailDef> instalments, List<AdvancePaymentActionHistoryDef> historys)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateAdvancePaymentAndInstalment);

            Context.Items.Add(AccountCommander.Param.advancePayment, payments);
            Context.Items.Add(AccountCommander.Param.instalments, instalments);
            Context.Items.Add(AccountCommander.Param.paymenthistory, historys);
            forwardToScreen(null);
        }

        private void messageScript(string message) 
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("alert('");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }

        protected void btnAttachment_Click(object sender, EventArgs e)
        {
            //string redirectURL = HttpContext.Current.Request.Url.AbsoluteUri.Replace("AdvancePaymentSearch.aspx", "InstalmentAttachmentList.aspx");
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "viewDoc", "window.open('" + redirectURL + "','_blank','width=420,height=180');", true);
            string escapeValue = vwAdvancePayment.PaymentId.ToString(); // HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(vwAdvancePayment.PaymentId.ToString()));
            string targetLocation = "InstalmentAttachmentList.aspx?PaymentId=" + escapeValue;
            string targetName = "viewDoc";
            com.next.infra.util.ClientScript.windowOpen(targetLocation, targetName, 420, 300, false, true, false, false, true, Page);
        }

        private AdvancePaymentActionHistoryDef NewAdvancePaymentActionHistory(int paymentId, string description)
        {
            AdvancePaymentActionHistoryDef newhistory = new AdvancePaymentActionHistoryDef();
            newhistory.PaymentId = paymentId;
            newhistory.Description = description;
            newhistory.ActionBy = this.LogonUserId;
            newhistory.ActionOn = DateTime.Now;
            newhistory.Status = 1;
            return newhistory;
        }

        protected decimal BalDiff
        {
            get
            {
                decimal totalDeductionAmt = this.vwInstalments.Where(ex => ex.SettlementDate > DateTime.MinValue).Select(ex => ex.PaymentAmount).Sum();
                return TotalAmount - totalDeductionAmt;
            }
        }

        protected decimal TotalAmount
        {
            get
            {
                return this.vwAdvancePayment.TotalAmount;
            }
        }

        protected void btnCalculation_Click(object sender, EventArgs e)
        {
            int paymentId = this.vwAdvancePayment.PaymentId;
            string amt = lbl_balDiff.Text;
            string targetLocation = "AdvPaymentInstalmentInterestCharges.aspx?paymentId=" + paymentId + "&amt=" + amt;
            string targetName = "Calculation";
            com.next.infra.util.ClientScript.windowOpen(targetLocation, targetName, 800, 500, false, true, false, false, true, Page);
        }

    }
}