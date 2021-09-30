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
using System.Windows.Forms;

namespace com.next.isam.webapp.account
{
    public partial class AdvPaymentInstalmentInterestCharges : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private GeneralWorker generalWorker = GeneralWorker.Instance;

        private AdvancePaymentDef vwAdvancePayment
        {
            get { return (AdvancePaymentDef)ViewState["vwAdvancePayment"]; }
            set { ViewState["vwAdvancePayment"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                txtPaymentDateFrom.DateTime = DateTime.MinValue;
                txtPaymentDateTo.DateTime = DateTime.Now;
                txtDebitNoteDate.DateTime = DateTime.Now;

                int paymentId = int.Parse(Request.Params["paymentId"]);
                decimal amt = decimal.Parse(Request.Params["amt"]);

                this.vwAdvancePayment = AccountManager.Instance.getAdvancePaymentByKey(paymentId);

                lblPaymentNo.Text = vwAdvancePayment.PaymentNo;
                lblSupplier.Text = vwAdvancePayment.Vendor.Name;
                lblOfficeName.Text = generalWorker.getOfficeRefByKey(vwAdvancePayment.OfficeId).OfficeCode;
                txtAdvanceRemainingTotal.Text = Math.Round(amt, 2).ToString();
                txtInterestRate.Text = vwAdvancePayment.InterestRate.ToString();
                lblCurrency.Text = vwAdvancePayment.Currency.CurrencyCode;
                lblCurrency2.Text = vwAdvancePayment.Currency.CurrencyCode;
            }
        }

        protected void txtInterestRate_TextChanged(object sender, EventArgs e)
        {

            DateTime from = txtPaymentDateFrom.DateTime;
            DateTime to = txtPaymentDateTo.DateTime;
            decimal r;
            bool RisNumeric = decimal.TryParse(txtAdvanceRemainingTotal.Text, out r);
            decimal ir;
            bool IRisNumeric = decimal.TryParse(txtInterestRate.Text, out ir);
            if (from != DateTime.MinValue && to != DateTime.MinValue && RisNumeric && IRisNumeric)
            {
                double totalDays = ((to - from).TotalDays + 1);
                bool isDebit = ddlDocType.SelectedIndex == 0 ? true : false;
                decimal remainingTotal = decimal.Parse(string.IsNullOrEmpty(txtAdvanceRemainingTotal.Text) ? "0" : txtAdvanceRemainingTotal.Text);
                decimal days = (decimal)totalDays;
                decimal rate = decimal.Parse(string.IsNullOrEmpty(txtInterestRate.Text) ? "0" : txtInterestRate.Text);
                decimal charges = Math.Round(remainingTotal * (days / 365) * (rate / 100), 2);
                lblInterestDays.Text = days.ToString();
                hidInterestDays.Value = days.ToString();
                lblInterestCharges.Text = isDebit ? charges.ToString("N") : (charges * -1).ToString("N");
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                int paymentId = this.vwAdvancePayment.PaymentId;
                string paymentNo = lblPaymentNo.Text;
                DateTime dcNoteDate = txtDebitNoteDate.DateTime;
                bool isDebit = Int32.Parse(ddlDocType.SelectedValue) == 0 ? true : false;
                decimal remainingTotal = decimal.Parse(string.IsNullOrEmpty(txtAdvanceRemainingTotal.Text) ? "0" : txtAdvanceRemainingTotal.Text);
                decimal date = decimal.Parse(hidInterestDays.Value);
                decimal rate = decimal.Parse(string.IsNullOrEmpty(txtInterestRate.Text) ? "0" : txtInterestRate.Text);
                decimal charges = Math.Round(remainingTotal * (date / 365) * (rate / 100), 2);
                if (!isDebit) charges = charges * -1;

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GenerateAdvPaymentInstalmentInterestCharges);

                Context.Items.Add(AccountCommander.Param.officeId, vwAdvancePayment.OfficeId);
                Context.Items.Add(AccountCommander.Param.debitNoteDate, dcNoteDate);
                Context.Items.Add(AccountCommander.Param.paymentId, paymentId);
                Context.Items.Add(AccountCommander.Param.paymentNo, paymentNo);
                Context.Items.Add(AccountCommander.Param.periodFrom, txtPaymentDateFrom.Text);
                Context.Items.Add(AccountCommander.Param.periodTo, txtPaymentDateTo.Text);
                Context.Items.Add(AccountCommander.Param.remainingTotal, remainingTotal);
                Context.Items.Add(AccountCommander.Param.periodDays, date);
                Context.Items.Add(AccountCommander.Param.interestRate, rate);
                Context.Items.Add(AccountCommander.Param.interestCharges, charges);
                Context.Items.Add(AccountCommander.Param.isDebit, isDebit);
                forwardToScreen(null);
                string dcNoteNo = (string)Context.Items[AccountCommander.Param.dcNote];

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Advance Payment", "alert('Advance Payment Instalment Interest Charges " + (isDebit ? "Debit" : "Credit") + " Note generated'); chargesClose('" + dcNoteNo + "');", true);
            }
        }

    }
}