using System;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using com.next.infra.util;
using com.next.infra.web;
using com.next.isam.appserver.account;
using com.next.isam.domain.order;
using com.next.isam.domain.claim;
using com.next.isam.domain.nontrade;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.webapp.commander.shipment;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.accounts;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.dms;
using CrystalDecisions.CrystalReports.Engine;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Linq;

namespace com.next.isam.webapp.nontrade
{
    public partial class NonTradeExpenseSettlement : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private ArrayList vwNTSearchResult
        {
            set
            {
                ViewState["NTSearchResult"] = value;
            }
            get
            {
                return (ArrayList)ViewState["NTSearchResult"];
            }
        }

        private ArrayList vwNTCcyDiscrepancyList
        {
            set
            {
                ViewState["NTCcyDiscrepancyList"] = value;
            }
            get
            {
                return (ArrayList)ViewState["NTCcyDiscrepancyList"];
            }
        }

        private ReportCriteria vwNTSearchCriteria
        {
            set
            {
                ViewState["NTSearchCriteria"] = value;
            }
            get
            {
                return (ReportCriteria)ViewState["NTSearchCriteria"];
            }
        }

        private string sortExpression
        {
            get { return (string)ViewState["SortExpression"]; }
            set { ViewState["SortExpression"] = value; }
        }

        private SortDirection sortDirection
        {
            get { return (SortDirection)ViewState["SortDirection"]; }
            set { ViewState["SortDirection"] = value; }
        }

        static ReportCriteria reportCriteria;
        protected int selectedCount = 0;
        protected int uploadCount = 0;

        protected decimal totalNTAmt = 0;
        protected decimal totalSelectedNTAmt = 0;
        protected decimal totalNTSettleAmt = 0;
        protected decimal totalSelectedNTSettleAmt = 0;
        protected int NTSelectedCount = 0;
        protected int NTUploadCount = 0;

        int colCheckBox = 0;
        int colDocumentType = 1;
        int colSupInvNo = 2;
        int colAccountNo = 3;
        int colOffice = 4;
        int colVendor = 5;
        int colInvDate = 6;
        int colNSLInvNo = 7;
        int colCurrency = 8;
        int colPaymentTerm = 9;
        int colChequeNo = 10;
        int colInvAmt = 11;
        int colVAT = 12;
        int colSettleAmt = 13;
        int colSettleDate = 14;
        int colSettleRef = 15;
        int colApprover = 16;
        int colDueDate = 17;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControls();
            }

        }

        protected void initControls()
        {
            /*
            ArrayList officeList = new ArrayList();
            ddl_Office.bindList(NonTradeManager.Instance.getNTOfficeList(this.LogonUserId), "OfficeCode", "OfficeId", "");
            ddl_Office.bindList(NonTradeManager.Instance.getNTUserOfficeList(this.LogonUserId, GeneralCriteria.ALL, GeneralCriteria.ALL), "OfficeCode", "OfficeId", "");
            */

            ddl_Office.bindList(NonTradeManager.Instance.getNTUserOfficeList(this.LogonUserId, NTRoleType.SETTLEMENT_MAINTENANCE.Id, GeneralCriteria.ALL), "OfficeCode", "OfficeId", "");
            txt_SupplierName.setWidth(305);
            txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.ntVendor);
            ArrayList statusList = new ArrayList();
            foreach (NTWFS status in NTWFS.getCollectionValues())
                if (status.Id == NTWFS.ACCOUNTS_APPROVED.Id || status.Id == NTWFS.SETTLED.Id)
                    statusList.Add(status);
            ddl_Status.bindList(statusList, "Name", "Id", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            ddl_Status.selectByValue(NTWFS.ACCOUNTS_APPROVED.Id.ToString());
            ddl_PaymentMethod.bindList(NTPaymentMethodRef.getCollectionValues(), "Name", "Id", "", "-- ALL --", GeneralCriteria.ALL.ToString());

            ArrayList currencyList = NonTradeManager.Instance.getNTInvoiceCurrencyList();
            if (currencyList != null)
            {
                //int currencyId = ((CurrencyRef)currencyList[0]).CurrencyId;
                currencyList.Sort(new ArrayListHelper.Sorter("CurrencyCode"));
                ddl_Currency.bindList(currencyList, "CurrencyCode", "CurrencyId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            }
            ddl_DocumentType.Items.Add(new ListItem("-- ALL --", GeneralCriteria.ALLSTRING));
            ddl_DocumentType.Items.Add(new ListItem("Invoice / Debit Note", "D"));
            ddl_DocumentType.Items.Add(new ListItem("Credit Note", "C"));

            tr_FirstApprover.Visible = (LogonUserHomeOffice.OfficeId == OfficeId.HK.Id);
            //ddl_FirstApprover.Visible = (LogonUserHomeOffice.OfficeId == OfficeId.HK.Id);
            refreshFirstApprover();
            refreshBankAccount();

            hid_SettlementTotalAmount.Value = string.Empty;
            hid_SettlementDate.Value = string.Empty;
        }

        protected void resetControls()
        {
            if (ddl_Office.Items.Count > 0)
                ddl_Office.SelectedIndex = 0;
            txt_SupplierName.clear();
            txt_AccountNoFrom.Text = string.Empty;
            txt_AccountNoTo.Text = string.Empty;
            txt_InvoiceDateFrom.Text = string.Empty;
            txt_InvoiceDateTo.Text = string.Empty;
            txt_DueDateFrom.Text = string.Empty;
            txt_DueDateTo.Text = string.Empty;
            txt_SettlementDateFrom.Text = string.Empty;
            txt_SettlementDateTo.Text = string.Empty;
            //ddl_Status.SelectedIndex = 0;
            ddl_Status.selectByValue(NTWFS.ACCOUNTS_APPROVED.Id.ToString());
            ddl_PaymentMethod.SelectedIndex = 0;
            ddl_Currency.SelectedIndex = 0;
            ddl_DocumentType.SelectedIndex = 0;
            refreshFirstApprover();
            refreshBankAccount();
            hid_SettlementTotalAmount.Value = string.Empty;
            hid_SettlementDate.Value = string.Empty;
        }

        protected void readInvoice()
        {
            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Non-Trade-Expense"));
            //queryStructs.Add(new QueryStructDef("NSL Invoice No", vwDomainNTInvoiceDef.NTInvoice.NSLInvoiceNo));
            ArrayList qList = DMSUtil.queryDocument(queryStructs);

        }


        #region GridView Event
        /*
        protected void NTPaymentRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            vwNTSearchResult.RemoveAt(arg.RowIndex);
            gridDataBind(vwNTSearchResult);
        }
        */
        protected void NTPaymentRowDataBound(object sender, GridViewRowEventArgs e)
        {
            Label lbl;
            string accountNo = string.Empty;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NTSettlement settlement = (NTSettlement)vwNTSearchResult[e.Row.RowIndex];
                NTInvoiceDef def = settlement.ntInvoice;
                CheckBox ckb = (CheckBox)e.Row.Cells[colCheckBox].FindControl("ckb_update");
                Label lblCcy = (Label)e.Row.Cells[colCurrency].FindControl("lbl_Currency");
                TextBox txtSettlementAmt = (TextBox)e.Row.Cells[colSettleAmt].FindControl("txt_NTSettleAmt");
                TextBox txtSettlementDate = (TextBox)e.Row.Cells[colSettleDate].FindControl("txt_NTSettleDate");
                TextBox txtSettlementRef = (TextBox)e.Row.Cells[colSettleRef].FindControl("txt_NTSettleRef");
                TextBox txtChequeNo = (TextBox)e.Row.Cells[colSettleDate].FindControl("txt_ChequeNo");
                Label lbl_Approver = (Label)e.Row.Cells[colApprover].FindControl("lbl_1stApprover");
                Label lbl_DueDate = (Label)e.Row.Cells[colDueDate].FindControl("lbl_DueDate");
                ckb.Checked = settlement.selected;
                //ckb.Checked = false;
                int officeId = -1;
                if (ddl_Office.Items.Count>0)
                    officeId = int.Parse(ddl_Office.SelectedItem.Value);
                if (officeId!=OfficeId.HK.Id && def.IsPayByHK == 1 )
                {
                    ckb.Enabled = false;
                    ckb.Checked = false;
                    if (txtSettlementAmt != null)
                        txtSettlementAmt.Enabled = false;
                    if (txtSettlementDate != null)
                        txtSettlementDate.Enabled = false;
                    if (txtSettlementRef != null)
                        txtSettlementRef.Enabled = false;
                    if (txtChequeNo != null)
                        txtChequeNo.Enabled = false;
                }
                ((Image)e.Row.FindControl("img_PayByHK")).Visible = (def.IsPayByHK==1);
                lblCcy.Text = def.Currency.CurrencyCode;
                lbl = (Label)e.Row.Cells[colDocumentType].FindControl("lbl_DocumentType");
                lbl.Text = (def.DCIndicator == "C" ? "Credit Note" : (def.DCIndicator == "D" ? "Invoice" : ""));

                if (gv_NTPayment.Columns[colSettleDate].Visible)
                {
                    if (string.IsNullOrEmpty(def.CustomerNo))
                        accountNo = string.Empty;
                    else
                    {
                        string payMonth = string.Empty;
                        if (def.InvoiceDate != DateTime.MinValue)
                            payMonth = def.PaymentFromDate.ToString("MMyy");
                        accountNo = def.CustomerNo + (string.IsNullOrEmpty(payMonth) ? string.Empty : "/" + payMonth);
                    }
                    LinkButton lb = (LinkButton)e.Row.Cells[colAccountNo].FindControl("lnk_AccountNo");
                    lb.Text = accountNo;

                    lbl = (Label)e.Row.Cells[colOffice].FindControl("lbl_Office");
                    lbl.Text = settlement.ntInvoice.Office.OfficeCode;

                    lbl = (Label)e.Row.Cells[colVendor].FindControl("lbl_Vendor");
                    lbl.Text = def.NTVendor.VendorName;

                    Decimal settleAmt = (def.SettlementAmount > 0 ? def.SettlementAmount : def.Amount + def.TotalVAT);
                    txtSettlementAmt.Text = settleAmt.ToString("#,##0.00");
                    lbl = (Label)e.Row.Cells[colSettleAmt].FindControl("lbl_NTSettleAmt");
                    lbl.Text = settleAmt.ToString();
                    if (def.SettlementAmount > 0 && def.SettlementBankAccountId > 0)
                    {
                        NSLBankAccountDef bankAccount = NonTradeManager.Instance.getNSLBankAccountByKey(def.SettlementBankAccountId);
                        lbl = (Label)e.Row.Cells[colSettleAmt].FindControl("lbl_SettleCurrency");
                        lbl.Text = bankAccount.Currency.CurrencyCode;
                        lbl.Style.Add(HtmlTextWriterStyle.Display, "block");
                    }

                    lbl = (Label)e.Row.Cells[colVAT].FindControl("lbl_VATAmt");
                    lbl.Text = def.TotalVAT.ToString("#,##0.00");

                    txtSettlementDate.Text = (def.SettlementDate != DateTime.MinValue ? def.SettlementDate.ToString("dd/MM/yyyy") : string.Empty);
                    

                    txtChequeNo.Text = def.ChequeNo;
                    txtChequeNo.Visible = (def.PaymentMethod.Id == NTPaymentMethodRef.CHEQUE.Id);

                    if (settlement.importFromFile && vwNTCcyDiscrepancyList != null && vwNTCcyDiscrepancyList.Contains(def.NSLInvoiceNo))
                    {
                        ckb.Enabled = false;
                        ckb.Checked = false;
                        lblCcy.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        txtSettlementAmt.Enabled = false;
                        txtSettlementAmt.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        txtSettlementDate.Enabled = false;
                        txtSettlementDate.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        txtSettlementRef.Enabled = false;
                        txtSettlementRef.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

                        txtChequeNo.Enabled = false;
                        txtChequeNo.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);
                    }

                }
                ((ImageButton)e.Row.FindControl("lnk_Attachment")).CommandArgument = e.Row.RowIndex.ToString();
                ((ImageButton)e.Row.FindControl("lnk_Attachment")).Attributes.Add("onclick", "openAttachments(this, " + def.InvoiceId.ToString() + ");return false;");
                ((LinkButton)e.Row.FindControl("lnk_SupplierInvoiceNo")).Attributes.Add("onclick", "openInvoiceWindow(" + def.InvoiceId.ToString() + ");return false;");
                ((LinkButton)e.Row.FindControl("lnk_AccountNo")).Attributes.Add("onclick", "openInvoiceWindow(" + def.InvoiceId.ToString() + ");return false;");
                totalNTAmt += def.Amount;
                totalNTSettleAmt += def.SettlementAmount;

                if (ckb.Checked)
                {
                    totalSelectedNTAmt += def.Amount;
                    totalSelectedNTSettleAmt += def.SettlementAmount;
                    NTSelectedCount += 1;
                }

                UserRef approver = CommonUtil.getUserByKey(def.AccountFirstApproverId);
                lbl_Approver.Text = (approver != null ? approver.DisplayName : string.Empty);
                lbl_DueDate.Text = def.DueDate.ToShortDateString();
            }
        }

        private void gridDataBind(ArrayList searchResult)
        {
            gv_NTPayment.DataSource = searchResult;
            gv_NTPayment.DataBind();
        }

        private void gridUpdateSelection ()
        {
            foreach (GridViewRow row in gv_NTPayment.Rows)
                {
                    NTSettlement settlement = (NTSettlement)vwNTSearchResult[row.RowIndex];
                    CheckBox ckb = (CheckBox)row.Cells[colCheckBox].FindControl("ckb_update");
                    settlement.selected = (ckb.Checked);
                }
        }

        protected void SettlementOnSort(object sender, GridViewSortEventArgs e)
        {
            if (sortExpression == e.SortExpression)
            {
                sortDirection = (sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending);
            }
            else
            {
                sortExpression = e.SortExpression;
                sortDirection = SortDirection.Ascending;
            }
            gridUpdateSelection();
            vwNTSearchResult.Sort(new NTSettlement.Comparer(sortExpression, sortDirection));
            gridDataBind(vwNTSearchResult);

        }

        #endregion

        private ArrayList getAllocatedSettlementList()
        {
            NTInvoiceDef inv;
            ArrayList settlementList = new ArrayList();
            ArrayList invList = new ArrayList();
            decimal settlementTotalAmount = decimal.Parse(hid_SettlementTotalAmount.Value);
            DateTime settlementDate = DateTimeUtility.getDate(hid_SettlementDate.Value);
            decimal totalInvAmt = 0;
            decimal allocatedAmt = 0;

            foreach (GridViewRow row in gv_NTPayment.Rows)
            {
                NTSettlement settlement = (NTSettlement)vwNTSearchResult[row.RowIndex];
                inv = settlement.ntInvoice;
                CheckBox ckb = (CheckBox)row.Cells[colCheckBox].FindControl("ckb_update");
                if (settlement.selected = ckb.Checked)
                {
                    totalInvAmt += inv.Amount;
                    settlementList.Add(settlement);
                    TextBox txt = (TextBox)row.Cells[colSettleRef].FindControl("txt_ChequeNo");
                    if (!string.IsNullOrEmpty(txt.Text.Trim()))
                        inv.ChequeNo = txt.Text.Trim();
                    else
                        inv.ChequeNo = string.Empty;
                }
            }
            for (int i = 0; i < settlementList.Count; i++)
            {
                inv = ((NTSettlement)settlementList[i]).ntInvoice;
                inv.SettlementDate = settlementDate;
                inv.SettlementAmount = (i == settlementList.Count - 1 ? settlementTotalAmount - allocatedAmt : decimal.Round(settlementTotalAmount * inv.Amount / totalInvAmt, 2));
                inv.SettlementBankAccountId = ddl_BankAccount.selectedValueToInt;
                inv.UpdateSettlementUserId = this.LogonUserId;
                inv.WorkflowStatus = NTWFS.SETTLED;
                allocatedAmt += inv.SettlementAmount;
                invList.Add(inv);
            }
            return invList;
        }

        private ArrayList getSettlementList()
        {
            NTInvoiceDef inv;
            TextBox txt;
            ArrayList invList = new ArrayList();
            foreach (GridViewRow row in gv_NTPayment.Rows)
            {
                NTSettlement settlement = (NTSettlement)vwNTSearchResult[row.RowIndex];
                inv = settlement.ntInvoice;
                CheckBox ckb = (CheckBox)row.Cells[colCheckBox].FindControl("ckb_update");
                if (settlement.selected = (ckb.Checked))
                {
                    if (settlement.importFromFile && vwNTCcyDiscrepancyList != null && vwNTCcyDiscrepancyList.Contains(inv.NSLInvoiceNo))
                        continue;

                    if (gv_NTPayment.Columns[colSettleDate].Visible)
                    {
                        txt = (TextBox)row.Cells[colSettleDate].FindControl("txt_NTSettleDate");
                        if (!string.IsNullOrEmpty(txt.Text.Trim()))
                            inv.SettlementDate = DateTimeUtility.getDate(txt.Text);
                        else
                            inv.SettlementDate = DateTime.MinValue;

                        txt = (TextBox)row.Cells[colSettleAmt].FindControl("txt_NTSettleAmt");
                        if (!string.IsNullOrEmpty(txt.Text.Trim()))
                            inv.SettlementAmount = Convert.ToDecimal(txt.Text);
                        else
                            inv.SettlementAmount = 0;

                        txt = (TextBox)row.Cells[colSettleRef].FindControl("txt_ChequeNo");
                        if (!string.IsNullOrEmpty(txt.Text.Trim()))
                            inv.ChequeNo = txt.Text.Trim();
                        else
                            inv.ChequeNo = string.Empty;

                        inv.SettlementBankAccountId = ddl_BankAccount.selectedValueToInt;
                        inv.UpdateSettlementUserId = this.LogonUserId;
                        inv.WorkflowStatus = NTWFS.SETTLED;
                    }
                    invList.Add(inv);
                }
            }
            return invList;
        }

        /*
        private void reserveUserInputData()
        {

            NTInvoiceDef def;
            TextBox txt;
            ArrayList tempList = new ArrayList();
            CheckBox ckb;
            foreach (GridViewRow row in gv_NTPayment.Rows)
            {
                NTSettlement settlement = (NTSettlement)vwNTSearchResult[row.RowIndex];
                def = settlement.ntInvoice;
                ckb = (CheckBox)row.Cells[colCheckBox].FindControl("ckb_update");
                settlement.selected = (ckb.Checked);

                txt = (TextBox)row.Cells[colSettleDate].FindControl("txt_NTSettleDate");
                if (!string.IsNullOrEmpty(txt.Text.Trim()))
                    def.SettlementDate = DateTimeUtility.getDate(txt.Text);
                else
                    def.SettlementDate = DateTime.MinValue;

                txt = (TextBox)row.Cells[colSettleAmt].FindControl("txt_NTSettleAmt");
                if (!string.IsNullOrEmpty(txt.Text.Trim()))
                    def.SettlementAmount = Convert.ToDecimal(txt.Text);
                else
                    def.SettlementAmount = 0;

                txt = (TextBox)row.Cells[colChequeNo].FindControl("txt_ChequeNo");
                if (!string.IsNullOrEmpty(txt.Text.Trim()))
                    def.ChequeNo = txt.Text.Trim();
                else
                    def.ChequeNo = string.Empty;

                txt = (TextBox)row.Cells[colSettleRef].FindControl("txt_NTSettleRef");
                if (!string.IsNullOrEmpty(txt.Text.Trim()))
                    def.SettlementRefNo = txt.Text.Trim();
                else
                    def.SettlementRefNo = string.Empty;
                tempList.Add(settlement);
            }
            vwNTSearchResult = tempList;
        }
        */

        protected void val_NTPayment_validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            string msg = settlementValidationMessage("FULL");
            if (!(args.IsValid = string.IsNullOrEmpty(msg)))
                val_NTpayment.ErrorMessage = msg;
        }
        /*
        protected void val_NTPaymentLumpSum_validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            string msg = settlementValidationMessage("LUMPSUM");
            if (!(args.IsValid = string.IsNullOrEmpty(msg)))
                val_NTpaymentLumpSum.ErrorMessage = msg;
        }
        */
        private string settlementValidationMessage(string mode)
        {
            bool lumpSumMode = (mode == "LUMPSUM");
            CheckBox ckb;
            int noOfChecked = 0;
            int noOfCashPayment = 0;
            decimal amt;
            DateTime date;
            TextBox settleAmt, settleDate, chequeNo;
            Label invoiceCurrency, paymentTerm;
            bool currencyConflict = false;
            string bankAccountCurrency = string.Empty;
            string settlementCurrency = string.Empty;
            string validationMessage = string.Empty;

            if (ddl_BankAccount.Items.Count > 0)
                if (ddl_BankAccount.SelectedValue != GeneralCriteria.ALL.ToString())
                    bankAccountCurrency = NonTradeManager.Instance.getNSLBankAccountByKey(ddl_BankAccount.selectedValueToInt).Currency.CurrencyCode;

            settlementCurrency = (lumpSumMode ? string.Empty : bankAccountCurrency);
            foreach (GridViewRow row in gv_NTPayment.Rows)
            {
                ckb = (CheckBox)row.Cells[colCheckBox].FindControl("ckb_Update");
                settleAmt = (TextBox)row.Cells[colSettleAmt].FindControl("txt_NTSettleAmt");
                settleDate = (TextBox)row.Cells[colSettleDate].FindControl("txt_NTSettleDate");
                chequeNo = (TextBox)row.Cells[colChequeNo].FindControl("txt_ChequeNo");
                invoiceCurrency = (Label)row.Cells[colCurrency].FindControl("lbl_Currency");
                paymentTerm = (Label)row.Cells[colPaymentTerm].FindControl("lbl_PaymentTerm");
                settleAmt.BorderColor = System.Drawing.Color.Empty;
                settleDate.BorderColor = System.Drawing.Color.Empty;
                chequeNo.BorderColor = System.Drawing.Color.Empty;
                invoiceCurrency.BackColor=System.Drawing.Color.Empty;

                if (ckb != null)
                {
                    if (ckb.Checked)
                    {
                        noOfChecked++;
                        noOfCashPayment += (paymentTerm.Text == "Cash" ? 1 : 0);
                        if (settlementCurrency == string.Empty)
                            settlementCurrency = invoiceCurrency.Text;
                        if (paymentTerm.Text != "Cash" && invoiceCurrency.Text != settlementCurrency)
                            currencyConflict = true;
                    }
                }
            }

            if (noOfChecked==0)
                validationMessage = "Please select invoice first.";
            else if (bankAccountCurrency==string.Empty && noOfChecked!=noOfCashPayment)
                validationMessage = "Please select the Bank Account.";
            else if (currencyConflict && noOfChecked != noOfCashPayment)
            {
                if (lumpSumMode)
                    validationMessage = "Invoice currency is not the same.";
                else
                    validationMessage = "Invoice currency does not match with the bank account."
                                    + "<br/>If you need to settle it in different currency, you may click the [Pay Using Different Currency] button";
                foreach (GridViewRow row in gv_NTPayment.Rows)
                    if ((ckb = (CheckBox)row.Cells[colCheckBox].FindControl("ckb_Update")) != null)
                    {
                        invoiceCurrency = (Label)row.Cells[colCurrency].FindControl("lbl_Currency");
                        if (ckb.Checked && (lumpSumMode || invoiceCurrency.Text != settlementCurrency))
                            invoiceCurrency.BackColor = System.Drawing.Color.Yellow;
                    }
            }
            else
            {
                bool isValid = true;
                foreach (GridViewRow row in gv_NTPayment.Rows)
                {
                    ckb = (CheckBox)row.Cells[colCheckBox].FindControl("ckb_Update");
                    settleAmt = (TextBox)row.Cells[colSettleAmt].FindControl("txt_NTSettleAmt");
                    settleDate = (TextBox)row.Cells[colSettleDate].FindControl("txt_NTSettleDate");
                    chequeNo = (TextBox)row.Cells[colChequeNo].FindControl("txt_ChequeNo");
                    if (settleAmt.Visible && ckb.Checked)
                        if (settleAmt.Text.Trim() != string.Empty || settleDate.Text.Trim() != string.Empty || (chequeNo.Visible && chequeNo.Text.Trim() != string.Empty))
                        {
                            if (!lumpSumMode)
                            {
                                amt = 0;
                                if (settleAmt.Text == string.Empty || (settleAmt.Text != string.Empty && !decimal.TryParse(settleAmt.Text, out amt)))
                                {
                                    settleAmt.BorderColor = System.Drawing.Color.Red;
                                    isValid = false;
                                }
                                date = DateTime.MinValue;
                                if (settleDate.Text == string.Empty || (settleDate.Text != string.Empty && !DateTime.TryParse(settleDate.Text, null, System.Globalization.DateTimeStyles.None, out date)))
                                {
                                    settleDate.BorderColor = System.Drawing.Color.Red;
                                    isValid = false;
                                }
                            }
                            if (chequeNo.Visible && chequeNo.Text.Trim() == string.Empty)
                            {
                                chequeNo.BorderColor = System.Drawing.Color.Red;
                                isValid = false;
                            }
                        }
                }
                if (!isValid)
                    validationMessage = "Invalid data input.";
            }
            return validationMessage;
        }

        protected ArrayList getNTInvoice(ReportCriteria criteria)
        {
            ArrayList result;
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetNTInvoiceList);
            Context.Items.Add(AccountCommander.Param.officeList, criteria.officeList);
            Context.Items.Add(AccountCommander.Param.fiscalYear, -1);
            Context.Items.Add(AccountCommander.Param.periodFrom, -1);
            Context.Items.Add(AccountCommander.Param.periodTo, -1);
            Context.Items.Add(AccountCommander.Param.invoiceNoFrom, criteria.invoiceNoFrom);
            Context.Items.Add(AccountCommander.Param.invoiceNoTo, criteria.invoiceNoTo);
            Context.Items.Add(AccountCommander.Param.customerNoFrom, criteria.customerNoFrom);
            Context.Items.Add(AccountCommander.Param.customerNoTo, criteria.customerNoTo);
            Context.Items.Add(AccountCommander.Param.invoiceDateFrom, criteria.invoiceDateFrom);
            Context.Items.Add(AccountCommander.Param.invoiceDateTo, criteria.invoiceDateTo);
            Context.Items.Add(AccountCommander.Param.paymentDateFrom, criteria.payDateFrom);
            Context.Items.Add(AccountCommander.Param.paymentDateTo, criteria.payDateTo);
            Context.Items.Add(AccountCommander.Param.expenseTypeId, -1);
            Context.Items.Add(AccountCommander.Param.dueDateFrom, criteria.dueDateFrom);
            Context.Items.Add(AccountCommander.Param.dueDateTo, criteria.dueDateTo);
            Context.Items.Add(AccountCommander.Param.settlementDateFrom, criteria.settlementDateFrom);
            Context.Items.Add(AccountCommander.Param.settlementDateTo, criteria.settlementDateTo);
            Context.Items.Add(AccountCommander.Param.approverId, criteria.approverId);
            Context.Items.Add(AccountCommander.Param.nslRefNoFrom, string.Empty);
            Context.Items.Add(AccountCommander.Param.nslRefNoTo, string.Empty);
            Context.Items.Add(AccountCommander.Param.vendorId, criteria.ntVendorId);
            Context.Items.Add(AccountCommander.Param.workflowStatusList, criteria.workflowStatusList);
            Context.Items.Add(AccountCommander.Param.currencyId, criteria.currencyId);
            Context.Items.Add(AccountCommander.Param.paymentMethodId, criteria.paymentMethodId);
            Context.Items.Add(AccountCommander.Param.includePayByHK, criteria.includePayByHK);
            Context.Items.Add(AccountCommander.Param.dcIndicator, criteria.dcIndicator);

            forwardToScreen(null);

            result = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];
            if (result != null)
                if (result.Count > 0)
                    return result;
            return null;
        }

        private string getCellRefList(int col, string commaDelimitedValues)
        {
            string colPrefix = OpenXmlUtil.getColumnLetter(col);
            string[] source = commaDelimitedValues.Split(',');
            return string.Join(",", source.Select((string x) => colPrefix + x).ToArray());
        }

        protected ArrayList genNTInvoiceDetail(TypeCollector invoiceIdList, ReportCriteria criteria)
        {

            ArrayList list = new ArrayList();
            int expenseTypeId = -1;
            if (invoiceIdList != null && invoiceIdList.Values.Count > 0)
            {
                NTInvoiceSettlementDs nTInvoiceSettlementDs = AccountReportManager.Instance.GenNTInvoiceDetailExportDataSet("SETTLEMENT", invoiceIdList, criteria.officeList, expenseTypeId, GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALL, criteria.invoiceDateFrom, criteria.invoiceDateTo, criteria.dueDateFrom, criteria.dueDateTo, criteria.settlementDateFrom, criteria.settlementDateTo, criteria.invoiceNoFrom, criteria.invoiceNoTo, criteria.customerNoFrom, criteria.customerNoTo, string.Empty, string.Empty, criteria.ntVendorId, criteria.workflowStatusList, criteria.currencyId, criteria.paymentMethodId, criteria.payDateFrom, criteria.payDateTo, criteria.AccountNoFrom.RawText, criteria.AccountNoTo.RawText, criteria.dcIndicator, criteria.approverId);
                string officeText = string.Empty;
                foreach (int value in criteria.officeList.Values)
                {
                    officeText = officeText + ((officeText == string.Empty) ? string.Empty : ", ") + OfficeId.getName(value);
                }
                string workflowStatusText = string.Empty;
                foreach (int value2 in criteria.workflowStatusList.Values)
                {
                    workflowStatusText = workflowStatusText + ((workflowStatusText == string.Empty) ? string.Empty : ",") + NTWFS.getType(value2).Name;
                }
                string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + "report_template\\";
                string sourceFileName = "NTSettlement.xlsm";
                string uId = DateTime.Now.ToString("yyyyMMddss");
                string destFile = string.Format(base.ApplPhysicalPath + "reporter\\tmpReport\\NTSettlement.xlsm-{0}-{1}.xlsm", base.LogonUserId.ToString(), uId);
                File.Copy(sourceFileDir + sourceFileName, destFile, true);
                string worksheetID = string.Empty;
                string templateWorksheetID = string.Empty;

                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(destFile, true);
                worksheetID = OpenXmlUtil.getWorksheetId(spreadsheetDocument, "Sheet1");
                templateWorksheetID = OpenXmlUtil.getWorksheetId(spreadsheetDocument, "Sheet2");
                OpenXmlUtil.setCellValue(spreadsheetDocument, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(base.LogonUserId).DisplayName, CellValues.String);
                OpenXmlUtil.setCellValue(spreadsheetDocument, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.Date);
                int cellStyleId = OpenXmlUtil.getCellStyleId(spreadsheetDocument, worksheetID, "B1");
                OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, 1, 3, officeText, CellValues.SharedString, cellStyleId);
                OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, 2, 3, (criteria.dcIndicator == string.Empty) ? "ALL" : ((criteria.dcIndicator == "D") ? "Invoice" : ((criteria.dcIndicator == "C") ? "Credit Note" : string.Empty)), CellValues.SharedString, cellStyleId);
                OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, 3, 3, (criteria.paymentMethodId == -1) ? "ALL" : NTPaymentMethodRef.getType(criteria.paymentMethodId).Name, CellValues.SharedString, cellStyleId);
                OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, 4, 3, (criteria.currencyId == GeneralCriteria.ALL) ? "ALL" : CurrencyId.getName(criteria.currencyId), CellValues.SharedString, cellStyleId);
                OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, 5, 3, (workflowStatusText == string.Empty) ? "NONE" : workflowStatusText, CellValues.SharedString, cellStyleId);
                OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, 6, 3, (criteria.settlementDateFrom == DateTime.MinValue) ? string.Empty : (criteria.settlementDateFrom.ToString("dd/MM/yyyy") + " - " + criteria.settlementDateTo.ToString("dd/MM/yyyy")), CellValues.SharedString, cellStyleId);
                int[] styleIdList = OpenXmlUtil.getStyleIdList(spreadsheetDocument, templateWorksheetID, 3, 17);
                int[] subtotalStyleIdList = OpenXmlUtil.getStyleIdList(spreadsheetDocument, templateWorksheetID, 4, 17);
                int[] grandTotalStyleIdList = OpenXmlUtil.getStyleIdList(spreadsheetDocument, templateWorksheetID, 5, 17);
                int startingRow = 9;
                int ttlCount = nTInvoiceSettlementDs.NTInvoiceSettlement.Rows.Count;
                int cnt = 0;
                Hashtable ccyTable = new Hashtable();
                string vendorName = string.Empty;
                string ccy = string.Empty;

                int groupStartingRow = startingRow;
                foreach (NTInvoiceSettlementDs.NTInvoiceSettlementRow row in nTInvoiceSettlementDs.NTInvoiceSettlement.Rows)
                {
                    if (vendorName != row.VendorName && startingRow != 9)
                    {
                        OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, startingRow, 11, ccy, CellValues.SharedString, subtotalStyleIdList[10]);
                        OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, startingRow, 12, "=SUM(" + OpenXmlUtil.getColumnLetter(12) + groupStartingRow.ToString() + ":" + OpenXmlUtil.getColumnLetter(12) + (startingRow - 1).ToString() + ")", CellValues.Number, subtotalStyleIdList[11]);
                        OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, startingRow, 13, "=SUM(" + OpenXmlUtil.getColumnLetter(13) + groupStartingRow.ToString() + ":" + OpenXmlUtil.getColumnLetter(13) + (startingRow - 1).ToString() + ")", CellValues.Number, subtotalStyleIdList[12]);
                        if (!ccyTable.ContainsKey(ccy))
                        {
                            ccyTable.Add(ccy, startingRow.ToString());
                        }
                        else
                        {
                            string str3 = (string)ccyTable[ccy];
                            ccyTable[ccy] = str3 + "," + startingRow.ToString();
                        }
                        startingRow += 2;
                        groupStartingRow = startingRow;
                    }
                    vendorName = row.VendorName;
                    ccy = row.CurrencyCode;
                    List<OpenXmlCell> cellValueList = new List<OpenXmlCell>();
                    cellValueList.Add(new OpenXmlCell(startingRow, 1, CellValues.SharedString, styleIdList[0], row.NSLInvoiceNo));
                    cellValueList.Add(new OpenXmlCell(startingRow, 2, CellValues.SharedString, styleIdList[1], (row.InvoiceNo.Trim() == string.Empty) ? row.CustomerNo : row.InvoiceNo));
                    cellValueList.Add(new OpenXmlCell(startingRow, 3, CellValues.Date, styleIdList[2], DateTimeUtility.getDateString(row.InvoiceDate)));
                    cellValueList.Add(new OpenXmlCell(startingRow, 4, CellValues.SharedString, styleIdList[3], row.IsBankAccount_SunAccountCodeNull() ? string.Empty : row.BankAccount_SunAccountCode));
                    cellValueList.Add(new OpenXmlCell(startingRow, 5, CellValues.SharedString, styleIdList[4], row.IsBankAccount_DescriptionNull() ? string.Empty : row.BankAccount_Description));
                    cellValueList.Add(new OpenXmlCell(startingRow, 6, CellValues.SharedString, styleIdList[5], row.IsVendorAccountCodeNull() ? string.Empty : row.VendorAccountCode));
                    cellValueList.Add(new OpenXmlCell(startingRow, 7, CellValues.SharedString, styleIdList[6], row.VendorName));
                    cellValueList.Add(new OpenXmlCell(startingRow, 8, CellValues.SharedString, styleIdList[7], row.ExpenseType));
                    cellValueList.Add(new OpenXmlCell(startingRow, 9, CellValues.SharedString, styleIdList[8], row.IsCostCenterNull() ? string.Empty : row.CostCenter));
                    cellValueList.Add(new OpenXmlCell(startingRow, 10, CellValues.SharedString, styleIdList[9], row.ItemDescription));
                    cellValueList.Add(new OpenXmlCell(startingRow, 11, CellValues.SharedString, styleIdList[10], row.CurrencyCode));
                    cellValueList.Add(new OpenXmlCell(startingRow, 12, CellValues.Number, styleIdList[11], row.Amount.ToString()));
                    cellValueList.Add(new OpenXmlCell(startingRow, 13, CellValues.Number, styleIdList[12], row.VAT.ToString()));
                    cellValueList.Add(new OpenXmlCell(startingRow, 14, CellValues.SharedString, styleIdList[13], row.T9Code));
                    cellValueList.Add(new OpenXmlCell(startingRow, 15, CellValues.SharedString, styleIdList[14], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 16, CellValues.SharedString, styleIdList[15], row.IsChequeNoNull() ? string.Empty : row.ChequeNo));
                    cellValueList.Add(new OpenXmlCell(startingRow, 17, CellValues.SharedString, styleIdList[16], row.Approver));
                    OpenXmlUtil.createRowAndCells(spreadsheetDocument, worksheetID, startingRow, 1, cellValueList);
                    startingRow++;
                    cnt++;
                }
                if (cnt == ttlCount)
                {
                    OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, startingRow, 11, ccy, CellValues.SharedString, subtotalStyleIdList[10]);
                    OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, startingRow, 12, "=SUM(" + OpenXmlUtil.getColumnLetter(12) + groupStartingRow.ToString() + ":" + OpenXmlUtil.getColumnLetter(12) + (startingRow - 1).ToString() + ")", CellValues.Number, subtotalStyleIdList[11]);
                    OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, startingRow, 13, "=SUM(" + OpenXmlUtil.getColumnLetter(13) + groupStartingRow.ToString() + ":" + OpenXmlUtil.getColumnLetter(13) + (startingRow - 1).ToString() + ")", CellValues.Number, subtotalStyleIdList[12]);
                    if (!ccyTable.ContainsKey(ccy))
                    {
                        ccyTable.Add(ccy, startingRow.ToString());
                    }
                    else
                    {
                        string str3 = (string)ccyTable[ccy];
                        ccyTable[ccy] = str3 + "," + startingRow.ToString();
                    }
                    startingRow += 2;
                }
                foreach (string key in ccyTable.Keys)
                {
                    OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, startingRow, 11, key, CellValues.SharedString, grandTotalStyleIdList[10]);
                    OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, startingRow, 12, "=SUM(" + getCellRefList(12, ccyTable[key].ToString()) + ")", CellValues.Number, grandTotalStyleIdList[11]);
                    OpenXmlUtil.setCellValue(spreadsheetDocument, worksheetID, startingRow, 13, "=SUM(" + getCellRefList(13, ccyTable[key].ToString()) + ")", CellValues.Number, grandTotalStyleIdList[12]);
                    startingRow++;
                }
                OpenXmlUtil.copyAndInsertRow(spreadsheetDocument, templateWorksheetID, 7, worksheetID, startingRow + 2);
                OpenXmlUtil.mergeCells(spreadsheetDocument, worksheetID, OpenXmlUtil.getCellReference(1, startingRow + 2), OpenXmlUtil.getCellReference(17, startingRow + 2));
                spreadsheetDocument.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
                spreadsheetDocument.WorkbookPart.Workbook.Save();
                spreadsheetDocument.Close();
                spreadsheetDocument.Dispose();
                WebHelper.outputFileAsHttpRespone(base.Response, destFile, true);
            }
            return list;
        }

        private ReportCriteria getReportCriteria(string searchType)
        {
            ReportCriteria criteria = new ReportCriteria();
            
            criteria.officeList = TypeCollector.Inclusive;
            if (ddl_Office.Items.Count > 0)
            {
                criteria.officeList.append(int.Parse(ddl_Office.SelectedValue));
                criteria.includePayByHK = (ddl_Office.SelectedValue == "1" ? 1 : 0);
            }
            else
            {
                criteria.officeList.append(int.MinValue);
                criteria.includePayByHK = 0;
            }
            criteria.payDateFrom = DateTime.MinValue;
            criteria.payDateTo = DateTime.MinValue;
            criteria.invoiceNoFrom = string.Empty;
            criteria.invoiceNoTo = string.Empty;
            criteria.customerNoFrom = string.Empty;
            criteria.customerNoTo = string.Empty;
            criteria.dcIndicator = string.Empty;
            criteria.invoiceDateFrom = DateTime.MinValue;
            criteria.invoiceDateTo = DateTime.MinValue;
            criteria.dueDateFrom = DateTime.MinValue;
            criteria.dueDateTo = DateTime.MinValue;
            criteria.settlementDateFrom = DateTime.MinValue;
            criteria.settlementDateTo = DateTime.MinValue;
            criteria.approverId = GeneralCriteria.ALL;

            criteria.workflowStatusList = TypeCollector.Inclusive;
            

            criteria.ntVendorId = (txt_SupplierName.NTVendorId == int.MinValue ? GeneralCriteria.ALL : txt_SupplierName.NTVendorId);

            foreach (ListItem itm in ddl_Status.Items)
                if (itm.Value != GeneralCriteria.ALL.ToString() && (itm.Value == ddl_Status.SelectedValue || (ddl_Status.SelectedValue == GeneralCriteria.ALL.ToString())))
                    criteria.workflowStatusList.append(int.Parse(itm.Value));

            criteria.paymentMethodId = int.Parse(ddl_PaymentMethod.SelectedValue);
            criteria.currencyId = int.Parse(ddl_Currency.SelectedValue);
            criteria.dcIndicator = ddl_DocumentType.SelectedValue;

            criteria.invoiceDateFrom = (string.IsNullOrEmpty(txt_InvoiceDateFrom.Text) ? DateTime.MinValue : DateTime.Parse(txt_InvoiceDateFrom.Text));
            criteria.invoiceDateTo = (string.IsNullOrEmpty(txt_InvoiceDateTo.Text) ? DateTime.MinValue : DateTime.Parse(txt_InvoiceDateTo.Text));
            if (criteria.invoiceDateFrom != DateTime.MinValue || criteria.invoiceDateTo != DateTime.MinValue)
            {
                if (criteria.invoiceDateFrom == DateTime.MinValue)
                    criteria.invoiceDateFrom = criteria.invoiceDateTo;
                if (criteria.invoiceDateTo == DateTime.MinValue)
                    criteria.invoiceDateTo = criteria.invoiceDateFrom;
            }
            criteria.dueDateFrom = (string.IsNullOrEmpty(txt_DueDateFrom.Text) ? DateTime.MinValue : DateTime.Parse(txt_DueDateFrom.Text));
            criteria.dueDateTo = (string.IsNullOrEmpty(txt_DueDateTo.Text) ? DateTime.MinValue : DateTime.Parse(txt_DueDateTo.Text));
            if (criteria.dueDateFrom != DateTime.MinValue || criteria.dueDateTo != DateTime.MinValue)
            {
                if (criteria.dueDateFrom == DateTime.MinValue)
                    criteria.dueDateFrom = criteria.dueDateTo;
                if (criteria.dueDateTo == DateTime.MinValue)
                    criteria.dueDateTo = criteria.dueDateFrom;
            }
            criteria.settlementDateFrom = (string.IsNullOrEmpty(txt_SettlementDateFrom.Text) ? DateTime.MinValue : DateTime.Parse(txt_SettlementDateFrom.Text));
            criteria.settlementDateTo = (string.IsNullOrEmpty(txt_SettlementDateTo.Text) ? DateTime.MinValue : DateTime.Parse(txt_SettlementDateTo.Text));
            if (criteria.settlementDateFrom != DateTime.MinValue || criteria.settlementDateTo != DateTime.MinValue)
            {
                if (criteria.settlementDateFrom == DateTime.MinValue)
                    criteria.settlementDateFrom = criteria.settlementDateTo;
                if (criteria.settlementDateTo == DateTime.MinValue)
                    criteria.settlementDateTo = criteria.settlementDateFrom;
            }
            criteria.approverId = (ddl_FirstApprover.Items.Count > 0 ? ddl_FirstApprover.selectedValueToInt : GeneralCriteria.ALL);
            criteria.AccountNoFrom = formatAccountNo(txt_AccountNoFrom.Text.Trim());
            criteria.AccountNoTo = formatAccountNo(txt_AccountNoTo.Text.Trim());

            if (searchType == "AccountNo")
            {
                if (criteria.AccountNoFrom != null || criteria.AccountNoTo != null)
                {
                    if (criteria.AccountNoTo == null)
                        criteria.AccountNoTo = criteria.AccountNoFrom;
                    if (criteria.AccountNoFrom == null)
                        criteria.AccountNoFrom = criteria.AccountNoTo;
                    if (criteria.AccountNoFrom.CustomerNo != string.Empty && criteria.AccountNoTo.CustomerNo != string.Empty)
                    {   // Seach by Accont No.
                        criteria.customerNoFrom = (criteria.AccountNoFrom.CustomerNo != null ? criteria.AccountNoFrom.CustomerNo : string.Empty);
                        criteria.customerNoTo = (criteria.AccountNoTo.CustomerNo != null ? criteria.AccountNoTo.CustomerNo : string.Empty);
                        criteria.payDateFrom = (criteria.AccountNoFrom.DateFrom != null ? criteria.AccountNoFrom.DateFrom : DateTime.MinValue);
                        criteria.payDateTo = (criteria.AccountNoTo.DateTo != null ? criteria.AccountNoTo.DateTo : DateTime.MinValue);
                    }
                    else if (criteria.AccountNoFrom.CustomerNo == string.Empty && criteria.AccountNoTo.CustomerNo == string.Empty)
                    {   // Search by Supplier Invoice No.
                        criteria.invoiceNoFrom = criteria.AccountNoFrom.RawText;
                        criteria.invoiceNoTo = criteria.AccountNoTo.RawText;
                    }
                    else
                    {   // Hybird search - both Supplier Invoice No. and Account No.
                        AccountNo accNo = null, accNoInv = null;
                        if (criteria.AccountNoFrom.CustomerNo != string.Empty)
                        {
                            accNo = criteria.AccountNoFrom;
                            accNoInv = criteria.AccountNoTo;
                        }
                        if (criteria.AccountNoTo.CustomerNo != string.Empty)
                        {
                            accNo = criteria.AccountNoTo;
                            accNoInv = criteria.AccountNoFrom;
                        }
                        criteria.customerNoFrom = accNo.CustomerNo;
                        criteria.customerNoTo = accNo.CustomerNo;
                        criteria.payDateFrom = accNo.DateFrom;
                        criteria.payDateTo = accNo.DateTo;

                        criteria.invoiceNoFrom = accNoInv.RawText;
                        criteria.invoiceNoTo = accNoInv.RawText;
                    }
                }
                else
                {
                    criteria.AccountNoFrom = new AccountNo();
                    criteria.AccountNoTo = new AccountNo();
                }
            }
            else if (searchType == "InvoiceNo")
            {
                if (criteria.AccountNoFrom != null)
                    criteria.invoiceNoFrom = criteria.AccountNoFrom.RawText;
                else
                    criteria.invoiceNoFrom = string.Empty;
                if (criteria.AccountNoTo != null)
                    criteria.invoiceNoTo = criteria.AccountNoTo.RawText;
                else
                    criteria.invoiceNoTo = string.Empty;

                criteria.customerNoFrom = string.Empty;
                criteria.customerNoTo = string.Empty;
                criteria.payDateFrom = DateTime.MinValue;
                criteria.payDateTo = DateTime.MinValue;

            }
            return criteria; 
        }

        /*
        private ArrayList getSearchResult(string recordType)
        {
            ArrayList invList = null;
            TypeCollector invoiceIdList = TypeCollector.Inclusive;
            if (vwNTSearchResult != null)
                foreach (NTSettlement settlement in vwNTSearchResult)
                    invoiceIdList.append(settlement.ntInvoice.InvoiceId);

            // Search by Account No. - Customer + pay date (month & Year)
            reportCriteria = getReportCriteria("AccountNo");
            if (recordType == "SUMMARY")
                invList = getNTInvoice(reportCriteria);
            else if (recordType == "DETAIL")
                invList = genNTInvoiceDetail(invoiceIdList, vwNTSearchCriteria);

            if (invList == null)
            {   // Retry to search by Supplier Invoice No 
                reportCriteria = getReportCriteria("InvoiceNo");
                if (recordType == "SUMMARY")
                    invList = getNTInvoice(reportCriteria);
                else if (recordType == "DETAIL")
                    invList = genNTInvoiceDetail(invoiceIdList, vwNTSearchCriteria);
            }
            vwNTSearchCriteria = reportCriteria;
            return invList;
        }
        */

        private ArrayList getSearchResult(string recordType)
        {
            ArrayList invList = null;

            // Search by Account No. - Customer + pay date (month & Year)
            reportCriteria = getReportCriteria("AccountNo");
            invList = getNTInvoice(reportCriteria);
            if (invList == null)
                // Retry to search by Supplier Invoice No 
                invList = getNTInvoice(reportCriteria=getReportCriteria("InvoiceNo"));

            if (recordType == "DETAIL")
            {
                if (invList != null)
                {   // Export the searched detail to a report
                    TypeCollector invoiceIdList = TypeCollector.Inclusive;
                    foreach (NTInvoiceDef inv in invList)
                        invoiceIdList.append(inv.InvoiceId);
                    invList = genNTInvoiceDetail(invoiceIdList, reportCriteria);
                }
            }
            vwNTSearchCriteria = reportCriteria;
            return invList;
        }

        #region button events

        protected void resetNTSearchResult()
        {
            if (vwNTSearchResult != null)
                vwNTSearchResult.Clear();
            if (vwNTCcyDiscrepancyList != null)
                vwNTCcyDiscrepancyList.Clear();
            //gv_NTPayment.DataSource = vwNTSearchResult;
            //gv_NTPayment.DataBind();
            gridDataBind(vwNTSearchResult);

        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            resetNTSearchResult();
            resetControls();

            pnl_Result.Visible = false;
            pnl_PayLumpSumButtons.Visible = false;
            lbl_Msg.Visible = false;
            lbl_Error.Visible = false;
            //pnl_TotalSelected.Visible = false;
            pnl_TotalNTSelected.Visible = false;
        }

        protected void btn_Save_Click(object sender, EventArgs e)
        {
            string msg = settlementValidationMessage("FULL");
            if (msg == string.Empty)
                //if (val_NTpayment.IsValid)
                saveSettlementInvoice(getSettlementList());
            else
            {
                /*
                val_NTpaymentLumpSum.ErrorMessage = string.Empty;
                */
                val_NTpayment.ErrorMessage = msg;
                val_NTpayment.IsValid = false;
            }
        }

        protected void btn_LumpSumSave_Click(object sender, EventArgs e)
        {
            string msg = settlementValidationMessage("LUMPSUM");
            if (msg == string.Empty)
                //if (val_NTpaymentLumpSum.IsValid)
                saveSettlementInvoice(getAllocatedSettlementList());
            else
            {
/*
                val_NTpayment.ErrorMessage = string.Empty;
                val_NTpaymentLumpSum.ErrorMessage = msg;
                val_NTpaymentLumpSum.IsValid = false;
 */
                val_NTpayment.ErrorMessage = msg;
                val_NTpayment.IsValid = false;
            }
            hid_SettlementTotalAmount.Value = string.Empty;
            hid_SettlementDate.Value = string.Empty;
        }

        private void saveSettlementInvoice(ArrayList invList)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.updateNTInvoiceSettlement);
            Context.Items.Add(AccountCommander.Param.ntInvoiceList, invList);
            forwardToScreen(null);
            ArrayList updateFailedList = (ArrayList)Context.Items[AccountCommander.Param.result];

            if (updateFailedList.Count > 0)
            {
                lbl_Error.Visible = true;
                lbl_Error.Text = "The following record are failed to update : <br />";
                for (int i = 0; i < updateFailedList.Count; i++)
                    lbl_Error.Text += updateFailedList[i].ToString() + "<br />";
            }
            lbl_Msg.Text = (invList.Count - updateFailedList.Count).ToString() + " records updated.";

            vwNTSearchResult = null;
            gridDataBind(vwNTSearchResult);

            pnl_NTPayment.Visible = false;
            pnl_TotalNTSelected.Visible = false;
            pnl_Result.Visible = false;
            pnl_PayLumpSumButtons.Visible = false;
            lbl_Msg.Visible = true;
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            pnl_Result.Visible = true;
            ArrayList newAddInv = new ArrayList();
            ArrayList invList;

            if ((invList = getSearchResult("SUMMARY")) != null)
            {
                ArrayList selectedInvNo = new ArrayList();
                foreach (NTInvoiceDef inv in invList)
                    if (!selectedInvNo.Contains(inv.NSLInvoiceNo))
                        newAddInv.Add(inv);
                if (newAddInv.Count > 0)
                {
                    if (vwNTSearchResult == null)
                        vwNTSearchResult = new ArrayList();
                    else
                        vwNTSearchResult.Clear();

                    foreach (NTInvoiceDef inv in newAddInv)
                    {
                        NTSettlement settlement = new NTSettlement();
                        settlement.ntInvoice = inv;
                        settlement.importFromFile = false;
                        settlement.settled = (inv.SettlementAmount != 0);
                        settlement.selected = false;    // (!settlement.settled);

                        vwNTSearchResult.Insert(0, settlement);
                    }
                    gridDataBind(vwNTSearchResult);

                    NTUploadCount = vwNTSearchResult.Count;
                    lbl_TotalNTSelected.Text = NTSelectedCount.ToString();
                    pnl_TotalNTSelected.Visible = (NTUploadCount > 0);
                    pnl_NTPayment.Visible = true;

                    lbl_Msg.Visible = false;
                    lbl_Error.Visible = false;
                }
                pnl_PayLumpSumButtons.Visible = (invList.Count > 0);
            }
            else
            {   //invoice not found.
                if (vwNTSearchResult == null)
                    vwNTSearchResult = new ArrayList();
                else
                    vwNTSearchResult.Clear();
                gridDataBind(vwNTSearchResult);
                pnl_TotalNTSelected.Visible = false;
                pnl_PayLumpSumButtons.Visible = false;
                //pnl_Result.Visible = false;
            }
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            pnl_Result.Visible = true;
            //ArrayList newAddInv = new ArrayList();

            ArrayList invList;

            invList = getSearchResult("DETAIL");
            pnl_PayLumpSumButtons.Visible = false;
            return;
        }

        protected AccountNo formatAccountNo(string accountNo)
        {
            int yy = 0, mm = 0;
            DateTime payDateFrom, payDateTo;
            AccountNo formatedAccountNo = null;
            if (!string.IsNullOrEmpty(accountNo))
            {
                formatedAccountNo = new AccountNo();
                formatedAccountNo.RawText = accountNo;
                formatedAccountNo.CustomerNo = string.Empty;
                if (accountNo.Length>5)
                    if (int.TryParse(accountNo.Substring(accountNo.Length - 2, 2), out yy)
                    && int.TryParse(accountNo.Substring(accountNo.Length - 4, 2), out mm))
                        if (accountNo.Substring(accountNo.Length - 5, 1) == "/" && mm > 0 && mm <= 12)
                        {
                            payDateFrom = DateTime.ParseExact(yy.ToString("00") + "-" + mm.ToString("00") + "-01", "yy-MM-dd", null);
                            if (mm == 12)
                                payDateTo = DateTime.ParseExact((yy + 1).ToString("00") + "-01-01", "yy-MM-dd", null).Subtract(TimeSpan.Parse("00:00:00.001"));
                            else
                                payDateTo = DateTime.ParseExact(yy.ToString("00") + "-" + (mm + 1).ToString("00") + "-01", "yy-MM-dd", null).Subtract(TimeSpan.Parse("00:00:00.001"));
                            formatedAccountNo.CustomerNo = accountNo.Substring(0, accountNo.Length - 5);
                            formatedAccountNo.DateFrom = payDateFrom;
                            formatedAccountNo.DateTo = payDateTo;
                            formatedAccountNo.Year = yy;
                            formatedAccountNo.Month = mm;
                        }
            }
            return formatedAccountNo;
        }
        

        #endregion


        protected void ddl_Office_OnSelectedIndexChanged(object sender, EventArgs arg)
        {
            refreshBankAccount();
            refreshFirstApprover();
            resetNTSearchResult();

            pnl_Result.Visible = false;
            pnl_PayLumpSumButtons.Visible = false;
            lbl_Msg.Visible = false;
            lbl_Error.Visible = false;
            pnl_TotalNTSelected.Visible = false;
        }

        protected void ddl_Currency_OnSelectedIndexChanged(object sender, EventArgs arg)
        {
            refreshBankAccount();
            resetNTSearchResult();

            pnl_Result.Visible = false;
            pnl_PayLumpSumButtons.Visible = false;
            lbl_Msg.Visible = false;
            lbl_Error.Visible = false;
            pnl_TotalNTSelected.Visible = false;
        }

        protected void refreshBankAccount()
        {
            if (ddl_Office.Items.Count > 0)
            {
                TypeCollector officeIdList = TypeCollector.Inclusive;
                TypeCollector currencyIdList = TypeCollector.Inclusive;
                currencyIdList.append(GeneralCriteria.ALL);
                if (ddl_Office.selectedValueToInt != GeneralCriteria.ALL)
                    officeIdList.append(ddl_Office.selectedValueToInt);
                /*
                else
                    foreach (ListItem itm in ddl_Office.Items)
                        if (ddl_Office.selectedValueToInt != GeneralCriteria.ALL)
                            officeIdList.append(int.Parse(itm.Value));
                */
                ArrayList bankAccountList = NonTradeManager.Instance.getNSLBankAccount(officeIdList, currencyIdList);
                if (bankAccountList.Count > 1)
                    ddl_BankAccount.bindList(bankAccountList, "Description", "NSLBankAccountId", "", "-- Please Select --", GeneralCriteria.ALL.ToString());
                else if (bankAccountList.Count == 1)
                    ddl_BankAccount.bindList(bankAccountList, "Description", "NSLBankAccountId");
                else
                    ddl_BankAccount.Items.Clear();
            }
            else
                ddl_BankAccount.Items.Clear();
        }

        protected void refreshFirstApprover()
        {
            if (ddl_Office.Items.Count > 0)
            {
                int officeId = ddl_Office.selectedValueToInt;
                ArrayList firstApproverList = NonTradeManager.Instance.getNTUserList(NTRoleType.FIRST_APPROVER.Id, GeneralCriteria.ALL, officeId);
                firstApproverList.Sort(new ArrayListHelper.Sorter("DisplayName"));
                ddl_FirstApprover.bindList(firstApproverList, "DisplayName", "UserId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                if (this.LogonUserHomeOffice.OfficeId == OfficeId.HK.Id)
                    ddl_FirstApprover.selectByValue(this.LogonUserId.ToString());
                else
                    ddl_FirstApprover.selectByValue(GeneralCriteria.ALL.ToString());
            }
            else
                ddl_FirstApprover.Items.Clear();
        }

        [Serializable()]
        public partial class NTSettlement
        {
            public NTInvoiceDef ntInvoice { get; set; }
            public bool importFromFile { get; set; }
            public bool settled { get; set; }
            public bool selected { get; set; }
            public string sortText { get; set; }

            public class Comparer : IComparer
            {
                public enum CompareType
                {
                    Nothing = 0,
                    NSLRefNo = 1,
                    InvoiceNo = 2,
                    InvoiceDate = 3,
                    Office = 4,
                    Vendor = 5,
                    Currency = 6,
                    PaymentMethod = 7,
                    ChequeNo = 8,
                    SettlementDate = 9,
                    DueDate = 10,
                    DocumentType = 11,
                    FirstApprover = 12,
                    AccountNo = 13,
                    VendorCOA = 14
                }

                private CompareType compareType;
                private SortDirection direction;

                public Comparer(CompareType type, SortDirection order)
                {
                    compareType = type;
                    direction = order;
                }

                public Comparer(string type, SortDirection order)
                {
                    compareType = ParseType(type);
                    direction = order;
                }

                public int Compare(object x, object y)
                {
                    string textX, textY;
                    NTSettlement refX = (NTSettlement)(direction == SortDirection.Ascending ? x : y);
                    NTSettlement refY = (NTSettlement)(direction == SortDirection.Ascending ? y : x);
                    if (compareType == CompareType.NSLRefNo)
                        return refX.ntInvoice.NSLInvoiceNo.CompareTo(refY.ntInvoice.NSLInvoiceNo);
                    else if (compareType == CompareType.InvoiceNo)
                        return refX.ntInvoice.InvoiceNo.CompareTo(refY.ntInvoice.InvoiceNo);
                    else if (compareType == CompareType.InvoiceDate)
                        return refX.ntInvoice.InvoiceDate.CompareTo(refY.ntInvoice.InvoiceDate);
                    else if (compareType == CompareType.Office)
                        return (refX.ntInvoice.Office.OfficeCode + refX.ntInvoice.NTVendor.VendorName).CompareTo(refY.ntInvoice.Office.OfficeCode + refY.ntInvoice.NTVendor.VendorName);
                    else if (compareType == CompareType.Vendor)
                        return refX.ntInvoice.NTVendor.VendorName.CompareTo(refY.ntInvoice.NTVendor.VendorName);
                    else if (compareType == CompareType.VendorCOA)
                        return refX.ntInvoice.NTVendor.SUNAccountCode.CompareTo(refY.ntInvoice.NTVendor.SUNAccountCode);
                    else if (compareType == CompareType.Currency)
                        return refX.ntInvoice.Currency.CurrencyCode.CompareTo(refY.ntInvoice.Currency.CurrencyCode);
                    else if (compareType == CompareType.PaymentMethod)
                        return refX.ntInvoice.PaymentMethod.Name.CompareTo(refY.ntInvoice.PaymentMethod.Name);
                    else if (compareType == CompareType.ChequeNo)
                        return refX.ntInvoice.ChequeNo.CompareTo(refY.ntInvoice.ChequeNo);
                    else if (compareType == CompareType.SettlementDate)
                        return refX.ntInvoice.SettlementDate.CompareTo(refY.ntInvoice.SettlementDate);
                    else if (compareType == CompareType.DueDate)
                        return refX.ntInvoice.DueDate.CompareTo(refY.ntInvoice.DueDate);
                    else if (compareType == CompareType.DocumentType)
                        return refX.ntInvoice.DCIndicator.CompareTo(refY.ntInvoice.DCIndicator);
                    else if (compareType == CompareType.FirstApprover)
                    {
                        textX = (refX.ntInvoice.AccountFirstApproverId != -1 ? CommonUtil.getUserByKey(refX.ntInvoice.AccountFirstApproverId).DisplayName : string.Empty);
                        textY = (refY.ntInvoice.AccountFirstApproverId != -1 ? CommonUtil.getUserByKey(refY.ntInvoice.AccountFirstApproverId).DisplayName : string.Empty);
                        return textX.CompareTo(textY);
                    }
                    else if (compareType == CompareType.AccountNo)
                    {
                        textX = string.Empty;
                        textY = string.Empty;
                        if (!string.IsNullOrEmpty(refX.ntInvoice.CustomerNo))
                        {
                            string payMonth = (refX.ntInvoice.InvoiceDate != DateTime.MinValue ? refX.ntInvoice.PaymentFromDate.ToString("MMyy") : string.Empty);
                            textX = refX.ntInvoice.CustomerNo + (string.IsNullOrEmpty(payMonth) ? string.Empty : "/" + payMonth);
                        }
                        if (!string.IsNullOrEmpty(refY.ntInvoice.CustomerNo))
                        {
                            string payMonth = (refY.ntInvoice.InvoiceDate != DateTime.MinValue ? refY.ntInvoice.PaymentFromDate.ToString("MMyy") : string.Empty);
                            textY = refY.ntInvoice.CustomerNo + (string.IsNullOrEmpty(payMonth) ? string.Empty : "/" + payMonth);
                        }
                        return textX.CompareTo(textY);
                    }
                    else
                        return 0;
                }

                private CompareType ParseType(string compareTypeString)
                {
                    CompareType type;
                    switch (compareTypeString.Replace(" ", "").Trim())
                    {
                        case "DocumentType":    
                            type = CompareType.DocumentType; break;
                        case "InvoiceNo":       
                            type = CompareType.InvoiceNo; break;
                        case "AccountNo":
                            type = CompareType.AccountNo; break;
                        case "Office":
                            type = CompareType.Office; break;
                        case "Vendor":
                            type = CompareType.Vendor; break;
                        case "VendorCOA":
                            type = CompareType.VendorCOA; break;
                        case "InvoiceDate":
                            type = CompareType.InvoiceDate; break;
                        case "NSLRefNo":
                            type = CompareType.NSLRefNo; break;
                        case "Currency":
                            type = CompareType.Currency; break;
                        case "PaymentMethod":
                            type = CompareType.PaymentMethod; break;
                        case "ChequeNo":
                            type = CompareType.ChequeNo; break;
                        case "SettlementDate":
                            type = CompareType.SettlementDate; break;
                        case "FirstApprover":
                            type = CompareType.FirstApprover; break;
                        case "DueDate":
                            type = CompareType.DueDate; break;
                        default:
                            type = CompareType.Nothing; break;
                    }
                    return type;
                }
            }
        }

        [Serializable()]
        public partial class AccountNo
        {
            public string RawText { get; set; }
            public string CustomerNo { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }

            public AccountNo()
            {
                RawText = string.Empty;
                CustomerNo = string.Empty;
                Year = 0;
                Month = 0;
                DateFrom = DateTime.MinValue;
                DateTo = DateTime.MinValue;
            }
        }

        [Serializable()]
        public partial class ReportCriteria
        {
            public TypeCollector officeList { get; set; }
            public string invoiceNoFrom { get; set; }
            public string invoiceNoTo { get; set; }
            public string customerNoFrom { get; set; }
            public string customerNoTo { get; set; }
            public DateTime invoiceDateFrom { get; set; }
            public DateTime invoiceDateTo { get; set; }
            public DateTime dueDateFrom { get; set; }
            public DateTime dueDateTo { get; set; }
            public DateTime settlementDateFrom { get; set; }
            public DateTime settlementDateTo { get; set; }
            public DateTime payDateFrom { get; set; }
            public DateTime payDateTo { get; set; }
            public int ntVendorId { get; set; }
            public int currencyId { get; set; }
            public int paymentMethodId { get; set; }
            public TypeCollector workflowStatusList { get; set; }
            public string dcIndicator { get; set; }
            public int includePayByHK { get; set; }
            public AccountNo AccountNoFrom { get; set; }
            public AccountNo AccountNoTo { get; set; }
            public int approverId { get; set; }
        }
    }

}