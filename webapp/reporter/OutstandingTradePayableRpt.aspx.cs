using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.common.domain.nss;
using com.next.common.datafactory.worker;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.ARAP;
using com.next.isam.webapp.commander;
using com.next.isam.appserver.common;
using com.next.isam.dataserver.worker;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class OutstandingTradePayableRpt : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControls();
            }
        }


        private void initControls()
        {
            txt_CutOffDate.Text = DateTimeUtility.getDateString(DateTime.Today);

            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());
            //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_TermOfPurchase.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());

            int defaultBaseCurrencyId = 3;  // USD
            ArrayList currencyList = CommonUtil.getCurrencyList();
            ArrayList baseCurrencyList = new ArrayList();
            ArrayList baseCurrencyId = new ArrayList(new int[5] { 1, 2, 3, 4, 12 });    //HKD, GBP, USD, CNY & EUR only
            foreach (CurrencyRef ccy in currencyList)
                if (baseCurrencyId.Contains(ccy.CurrencyId))
                    baseCurrencyList.Add(ccy);
            ddl_BaseCurrency.bindList(baseCurrencyList, "CurrencyCode", "CurrencyId", defaultBaseCurrencyId.ToString());
            ddl_Currency.bindList(currencyList, "CurrencyCode", "CurrencyId", "", "--All--", GeneralCriteria.ALL.ToString());

            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            ArrayList arr_Year = WebUtil.getBudgetYearList();

            AccountFinancialCalenderDef finCalDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Now.AddDays(-10));
            ddl_Year.DataSource = arr_Year;
            ddl_Year.DataBind();
            ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();
            ddl_PeriodFrom.SelectedValue = finCalDef.Period.ToString();
            ddl_PeriodTo.SelectedValue = finCalDef.Period.ToString();

            ExchangeRateDef baseCurrencyRate = NssWorker.Instance.getExchangeRateDefByDateLatest(defaultBaseCurrencyId, ExchangeRateType.PAYABLE_RECEIVABLE.Id, DateTime.Now); 
            AccountFinancialCalenderDef exchangeRatePeriod = CommonUtil.getAccountPeriodByDate(9, baseCurrencyRate.EffDateFrom);
            ddl_ExchangeRateYear.DataSource = arr_Year;
            ddl_ExchangeRateYear.DataBind();
            ddl_ExchangeRateYear.SelectedValue = exchangeRatePeriod.BudgetYear.ToString();
            ddl_ExchangeRatePeriod.SelectedValue = exchangeRatePeriod.Period.ToString();

            ddl_ReceiptRefNo.DataSource = WebUtil.getPaymentReferenceCodeList();
            ddl_ReceiptRefNo.DataBind();
            ddl_ReceiptRefNo.Items.Insert(0, new ListItem("--All--", ""));

            cbl_Customer.DataSource = WebUtil.getCustomerList(-1);
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();

            cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            cbl_TradingAgency.DataTextField = "ShortName";
            cbl_TradingAgency.DataValueField = "TradingAgencyId";
            cbl_TradingAgency.DataBind();

            foreach (ListItem item in cbl_Customer.Items)
            {
                item.Selected = true;
            }
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                item.Selected = true;
            }

        }


        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportClass rpt = genReport();
            if (rpt != null)
                ReportHelper.export(rpt, HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "OutstandingPayableRpt");
            else
            {
                ExchangeRateDef latestExchangeRate = NssWorker.Instance.getExchangeRateDefByDateLatest(ddl_BaseCurrency.selectedValueToInt, ExchangeRateType.PAYABLE_RECEIVABLE.Id, DateTime.Now);
                AccountFinancialCalenderDef latestRatePeriod = CommonUtil.getAccountPeriodByDate(9, latestExchangeRate.EffDateFrom);
                string period = latestRatePeriod.BudgetYear.ToString() + " P" + latestRatePeriod.Period.ToString();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OSPayableReport", "alert('This Exchange Rate is not ready for reporting purpose.\\nPlease select the other exchange rate period.\\nThe latest available Exchange Rate is " + period + ".');", true);
            }
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportClass rpt = genReport();
            if (rpt != null)
                ReportHelper.export(rpt, HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "OutstandingPayableRpt");
            else
            {
                ExchangeRateDef latestExchangeRate = NssWorker.Instance.getExchangeRateDefByDateLatest(ddl_BaseCurrency.selectedValueToInt, ExchangeRateType.PAYABLE_RECEIVABLE.Id, DateTime.Now);
                AccountFinancialCalenderDef latestRatePeriod = CommonUtil.getAccountPeriodByDate(9, latestExchangeRate.EffDateFrom);
                string period = latestRatePeriod.BudgetYear.ToString() + " P" + latestRatePeriod.Period.ToString();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OSPayableReport", "alert('This Exchange Rate is not ready for reporting purpose.\\nPlease select the other exchange rate period.\\nThe latest available Exchange Rate is " + period + ".');", true);
            }
        }


        private ReportClass genReport()
        {
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            DateTime cutoffDate = DateTimeUtility.getDate(txt_CutOffDate.Text.Trim());
            int budgetYear = -1;
            int periodFrom = -1;
            int periodTo = -1;
            int vendorId = -1;
            int isSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue) ;
            int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue) ;
            int isOPROrder = Convert.ToInt32(ddl_OPROrder.SelectedValue) ;
            int exchangeRateYear = Convert.ToInt32(ddl_ExchangeRateYear.SelectedValue);
            int exchangeRatePeriod = Convert.ToInt32(ddl_ExchangeRatePeriod.SelectedValue);
            string orderType = ddl_OrderType.SelectedValue;
            int SampleOrderGroup = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);

            if (rad_InvoiceDate.Checked && txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }
            else if (rad_FiscalPeriod.Checked)
            {
                budgetYear = Convert.ToInt32(ddl_Year.SelectedValue);
                periodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
                periodTo = Convert.ToInt32(ddl_PeriodTo.SelectedValue);
            }

            DateTime invoiceUploadDateFrom = DateTime.MinValue;
            DateTime invoiceUploadDateTo = DateTime.MinValue;
            if (txt_InvoiceUploadDateFrom.Text.Trim() != "")
            {
                invoiceUploadDateFrom = DateTimeUtility.getDate(txt_InvoiceUploadDateFrom.Text.Trim());
                if (txt_InvoiceUploadDateTo.Text.Trim() == "")
                    txt_InvoiceUploadDateTo.Text = txt_InvoiceUploadDateFrom.Text;
                invoiceUploadDateTo = DateTimeUtility.getDate(txt_InvoiceUploadDateTo.Text.Trim());
            }
            DateTime accountReceiptDateFrom = DateTime.MinValue;
            DateTime accountReceiptDateTo = DateTime.MinValue;
            if (txt_AccountReceiptDateFrom.Text.Trim() != "")
            {
                accountReceiptDateFrom = DateTimeUtility.getDate(txt_AccountReceiptDateFrom.Text.Trim());
                if (txt_AccountReceiptDateTo.Text.Trim() == "")
                    txt_AccountReceiptDateTo.Text = txt_AccountReceiptDateFrom.Text;
                accountReceiptDateTo = DateTimeUtility.getDate(txt_AccountReceiptDateTo.Text.Trim());
            }

            if (txt_Supplier.VendorId != int.MinValue)
                vendorId = txt_Supplier.VendorId;

            int purchaseTermId = Convert.ToInt32(ddl_TermOfPurchase.SelectedValue);
            int paymentTermId = Convert.ToInt32(ddl_PaymentTerm.SelectedValue);

            /*
            int officeId = Convert.ToInt32(ddl_Office.SelectedValue);
            
            TypeCollector productTeamList = TypeCollector.Inclusive;
            if (uclProductTeam.ProductCodeId != int.MinValue)
                productTeamList.append(uclProductTeam.ProductCodeId);

            
            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (officeId == -1)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef office in userOfficeList)
                {
                    officeIdList.append(office.OfficeId);

                    if (uclProductTeam.ProductCodeId == int.MinValue)
                    {
                        ArrayList pt = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, office.OfficeId, this.LogonUserId, GeneralCriteria.ALLSTRING);
                        foreach (OfficeStructureRef os in pt)
                        {
                            productTeamList.append(os.OfficeStructureId);
                        }
                    }
                }
            }
            else
            {
                if (uclProductTeam.ProductCodeId == int.MinValue)
                {
                    ArrayList pt = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, officeId, this.LogonUserId, GeneralCriteria.ALLSTRING);
                    foreach (OfficeStructureRef os in pt)
                    {
                        productTeamList.append(os.OfficeStructureId);
                    }
                }
            }
            */
            int officeId = -1;
            int officeGroupId = Convert.ToInt32(ddl_Office.SelectedValue);
            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (officeGroupId == -1)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef office in userOfficeList)
                    officeIdList.append(office.OfficeId);
            }
            else
            {
                if (ddl_Office.selectedText.Contains("+"))
                {
                    ArrayList officeList = CommonWorker.Instance.getOfficeListByReportOfficeGroupId(Convert.ToInt32(officeGroupId));
                    foreach (OfficeRef office in officeList)
                        officeIdList.append(office.OfficeId);
                }
                else
                    officeIdList.append(officeGroupId);
            }

            //if (!officeIdList.contains(OfficeId.DG.Id))
            //    handlingOfficeId = -1;

            TypeCollector productTeamList = TypeCollector.Inclusive;
            if (uclProductTeam.ProductCodeId != int.MinValue)
                productTeamList.append(uclProductTeam.ProductCodeId);
            /*
            else
            {
                foreach (int Id in officeIdList.Values)
                {
                    ArrayList pt = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, Id, this.LogonUserId, GeneralCriteria.ALLSTRING);
                    foreach (OfficeStructureRef os in pt)
                        productTeamList.append(os.OfficeStructureId);
                }
            }
            */

            string payRefCode = ddl_ReceiptRefNo.SelectedValue;
            int currencyId = Convert.ToInt32(ddl_Currency.SelectedValue);
            int baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);

            TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (item.Selected)
                    customerTypeCollector.append(Convert.ToInt32(item.Value));
            }

            TypeCollector tradingAgencyCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                if (item.Selected)
                    tradingAgencyCollector.append(Convert.ToInt32(item.Value));
            }

            OutstandingTradeReport rpt = ARAPReportManager.Instance.getAPReport(cutoffDate, invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo, accountReceiptDateFrom, accountReceiptDateTo, 
                budgetYear, periodFrom, periodTo, currencyId, baseCurrencyId, exchangeRateYear, exchangeRatePeriod, payRefCode, purchaseTermId, paymentTermId, orderType, 
                officeId, officeIdList, handlingOfficeId, productTeamList, vendorId, isSZOrder,
                isUTOrder, isOPROrder,  customerTypeCollector, tradingAgencyCollector, SampleOrderGroup, int.Parse(rad_ReportVersion.SelectedValue), this.LogonUserId);

            return rpt;
        }

        protected void rad_FiscalPeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_FiscalPeriod.Checked)
            {
                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false;

                ddl_Year.Enabled = true;
                ddl_PeriodFrom.Enabled = true;
                ddl_PeriodTo.Enabled = true;
            }
        }

        protected void rad_InvoiceDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_InvoiceDate.Checked)
            {
                txt_InvoiceDateFrom.Enabled = true;
                txt_InvoiceDateTo.Enabled = true;

                ddl_Year.Enabled = false;
                ddl_PeriodFrom.Enabled = false;
                ddl_PeriodTo.Enabled = false;
            }
        }

        protected void ddl_PeriodFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_PeriodTo.SelectedIndex = ddl_PeriodFrom.SelectedIndex;
        }

        protected void rad_ReportVersion_SelectedChanged(object sender, EventArgs e)
        {
            if (rad_ReportVersion.SelectedValue=="0")
            {   // SUN version
                tr_InvoiceDate.Style.Add(HtmlTextWriterStyle.Display, "block");
                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false;
                td_FiscalPeriod.Style.Add(HtmlTextWriterStyle.Display, "block");
            } if (rad_ReportVersion.SelectedValue == "1")
            {   // Epicor version
                tr_InvoiceDate.Style.Add(HtmlTextWriterStyle.Display, "none");
                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false;

                rad_FiscalPeriod.Checked = true;
                td_FiscalPeriod.Style.Add(HtmlTextWriterStyle.Display, "none");
                ddl_Year.Enabled = true;
                ddl_PeriodFrom.Enabled = true;
                ddl_PeriodTo.Enabled = true;
            }
        }


    }
}
