using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.shipping;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class STWDateDiscrepancyReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                initControl();
            }

        }


        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                ReportHelper.export(genReport("REPORT"), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "STW Date Discrepancy Report");
        }


        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                ReportHelper.export(genReport("EXCEL"), HttpContext.Current.Response, 
                    CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "STW Date Discrepancy Report");
        }


        void initControl()
        {
            ddl_ShipmentPort.bindList(WebUtil.getShipmentPortList(), "ShipmentPortDescription", "ShipmentPortId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            ddl_CountryOfOrigin.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            ddl_BaseCurrency.bindList(WebUtil.getCurrencyListForExchangeRate(), "CurrencyCode", "CurrencyId", "3");

            ddl_Year.DataSource = WebUtil.getBudgetYearList();
            ddl_Year.DataBind();
            AccountFinancialCalenderDef finCalDef = CommonUtil.getCurrentFinancialPeriod(9);
            ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();

            uclOfficeProductTeamSelection.UserId = this.LogonUserId;

            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();

            cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            cbl_TradingAgency.DataTextField = "ShortName";
            cbl_TradingAgency.DataValueField = "TradingAgencyId";
            cbl_TradingAgency.DataBind();

            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                    item.Selected = true;
            }
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                item.Selected = true;
            }

            //ucl_SortingOrder1.addItem("Invoice No", "InvoiceNo", true);
            //ucl_SortingOrder1.addItem("Invoice Date", "i.InvoiceDate", false);
            //ucl_SortingOrder1.addItem("Product Team", "ProductTeam", false);
            //ucl_SortingOrder1.addItem("Contract No.", "c.ContractNo", false);
            //ucl_SortingOrder1.addItem("Delivery No.", "s.DeliveryNo", false);
            //ucl_SortingOrder1.addItem("Item No.", "ItemNo", false);
            //ucl_SortingOrder1.addItem("Trading Agency", "TradingAgency", false);
            //ucl_SortingOrder1.addItem("Invoice Amount", "s.TotalShippedAmt", false);
            //ucl_SortingOrder1.addItem("Supplier Invoice Amount", "s.TotalShippedSupplierGmtAmtAfterDiscount", false);
            //ucl_SortingOrder1.addItem("Supplier Invoice No.", "i.SupplierInvoiceNo", false);
            //ucl_SortingOrder1.addItem("Country of Origin", "CO", false);

        }


        private ReportClass genReport(string exportType)
        {
            // export type : 'REPORT' or 'EXCEL'
            int baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            int selectedCount;

            if (rad_InvoiceDate.Checked && txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            DateTime IlsStwDateFrom = DateTime.MinValue;
            DateTime IlsStwDateTo = DateTime.MinValue;
            if (txt_IlsStwDateFrom.Text.Trim() != "")
            {
                IlsStwDateFrom = DateTimeUtility.getDate(txt_IlsStwDateFrom.Text.Trim());
                if (txt_IlsStwDateTo.Text.Trim() == "")
                    txt_IlsStwDateTo.Text = txt_IlsStwDateFrom.Text;
                IlsStwDateTo = DateTimeUtility.getDate(txt_IlsStwDateTo.Text.Trim());
            }

            DateTime ActualStwDateFrom = DateTime.MinValue;
            DateTime ActualStwDateTo = DateTime.MinValue;
            if (txt_ActualStwDateFrom.Text.Trim() != "")
            {
                ActualStwDateFrom = DateTimeUtility.getDate(txt_ActualStwDateFrom.Text.Trim());
                if (txt_ActualStwDateTo.Text.Trim() == "")
                    txt_ActualStwDateTo.Text = txt_ActualStwDateFrom.Text;
                ActualStwDateTo = DateTimeUtility.getDate(txt_ActualStwDateTo.Text.Trim());
            }

            int budgetYear = -1;
            int periodFrom = -1;
            int periodTo = -1;

            if (rad_FiscalPeriod.Checked)
            {
                budgetYear = Convert.ToInt32(ddl_Year.SelectedValue);
                periodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
                periodTo = Convert.ToInt32(ddl_PeriodTo.SelectedValue);
            }
            
            int countryOfOriginId = Convert.ToInt32(ddl_CountryOfOrigin.SelectedValue);
            int shipmentPortId = Convert.ToInt32(ddl_ShipmentPort.SelectedValue);

            int productTeamTotalCount = uclOfficeProductTeamSelection.getProductCodeTotalCount();
            TypeCollector productTeamList = uclOfficeProductTeamSelection.getProductCodeList();
            ArrayList productTeamCodeList = uclOfficeProductTeamSelection.getSelectedProductTeam("CODE");
            string productTeamString = "";
            if (productTeamList.Values.Count == productTeamTotalCount)
                productTeamString = "ALL";
            else
            {
                foreach (string code in productTeamCodeList) 
                    productTeamString += ", " + code;
                if (productTeamString.Length > 2)
                    productTeamString = productTeamString.Substring(2, productTeamString.Length - 2);
            }


            int officeTotalCount = uclOfficeProductTeamSelection.getOfficeTotalCount();
            TypeCollector officeIdList = uclOfficeProductTeamSelection.getOfficeList();
            ArrayList officeList = uclOfficeProductTeamSelection.getSelectedOffice("DESC");
            string officeString = "";
            if (officeIdList.Values.Count == officeTotalCount)
                officeString = "ALL";
            else
            {
                foreach (string office in officeList) 
                    officeString += ", " + office;
                if (officeString.Length > 2)
                    officeString = officeString.Substring(2, officeString.Length - 2).ToUpper();
            }


            string customerString = "";
            selectedCount = 0;
            TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (item.Selected)
                {
                    customerTypeCollector.append(Convert.ToInt32(item.Value));
                    customerString += ", " + item.Text;
                    selectedCount++;
                }
            }
            if (selectedCount == cbl_Customer.Items.Count)
                customerString = "ALL";
            else
                if (customerString.Length > 2)
                    customerString = customerString.Substring(2, customerString.Length - 2);
            

            string tradingAgencyString = "";
            selectedCount = 0;
            TypeCollector tradingAgencyCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                if (item.Selected)
                {
                    tradingAgencyCollector.append(Convert.ToInt32(item.Value));
                    tradingAgencyString += ", " + item.Text;
                    selectedCount++;
                }
            }
            if (selectedCount == cbl_TradingAgency.Items.Count)
                tradingAgencyString = "ALL";
            else
                if (tradingAgencyString.Length > 2)
                    tradingAgencyString = tradingAgencyString.Substring(2, tradingAgencyString.Length - 2);

            
            STWDateDiscrepancyRpt rpt = ShippingReportManager.Instance.getSTWDateDiscrepancyReport (budgetYear, periodFrom, periodTo, 
                invoiceDateFrom, invoiceDateTo, IlsStwDateFrom, IlsStwDateTo, ActualStwDateFrom, ActualStwDateTo, countryOfOriginId, shipmentPortId, 
                officeIdList, officeString, productTeamList, productTeamString, customerTypeCollector, customerString, tradingAgencyCollector, tradingAgencyString,  
                baseCurrencyId, this.LogonUserId, ucl_SortingOrder1.SortingField, exportType);

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

    }
}
