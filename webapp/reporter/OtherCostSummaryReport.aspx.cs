using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.infra.util;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.helper;
using com.next.isam.appserver.common;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.reporter.accounts;

namespace com.next.isam.webapp.reporter
{
    

    public partial class OtherCostSummaryReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        bool HasAccessRights_SuperView = false;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                initControl();
            }
            else
                tr_HandlingOffice_Refresh();
        }

        protected void btn_Preview_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "OtherCostSummary");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "OtherCostSummary");
        }

        void initControl()
        {
            int FiscalYear;
            int AccountingPeriod;
            DateTime Today;

            Today = DateTime.Today;
            if (Today.Month == 1)
            {
                FiscalYear = Today.Year - 1;
                AccountingPeriod = 12;
            }
            else
            {
                FiscalYear = Today.Year;
                AccountingPeriod = Today.Month - 1;
            }
            ddl_Year.DataSource = WebUtil.getBudgetYearList();
            ddl_Year.DataBind();
            ddl_Year.SelectedValue = FiscalYear.ToString();
            ddl_PeriodFrom.SelectedValue = AccountingPeriod.ToString();
            ddl_PeriodTo.SelectedValue = AccountingPeriod.ToString();

            ddl_BaseCurrency.bindList(WebUtil.getCurrencyListForExchangeRate(), "CurrencyCode", "CurrencyId", "3");
            //ddl_BaseCurrency.bindList(AccountWorker.Instance.getBaseCurrencyList(), "CurrencyCode", "CurrencyId", "");
            //foreach (ListItem item in ddl_BaseCurrency.Items) item.Selected = (item.Text == "USD");

            cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            cbl_TradingAgency.DataTextField = "ShortName";
            cbl_TradingAgency.DataValueField = "TradingAgencyId";
            cbl_TradingAgency.DataBind();
            foreach (ListItem item in cbl_TradingAgency.Items) item.Selected = true;

            cbl_PurchaseTerm.DataSource = WebUtil.getTermOfPurchaseList();
            cbl_PurchaseTerm.DataTextField = "TermOfPurchaseDescription";
            cbl_PurchaseTerm.DataValueField = "TermOfPurchaseId";
            cbl_PurchaseTerm.DataBind();
            foreach (ListItem item in cbl_PurchaseTerm.Items) item.Selected = true;

            cbl_Office.DataSource = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            cbl_Office.DataTextField = "Description";
            cbl_Office.DataValueField = "OfficeId";
            cbl_Office.DataBind();
            for (int i = 0; i < cbl_Office.Items.Count; i++)
            {
                ListItem item = cbl_Office.Items[i];
                item.Text = item.Text.Replace("Office", "").Trim();
                item.Selected = true;
            }
            this.ddl_HandlingOffice.bindList(CommonManager.Instance.getDGHandlingOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());
            tr_HandlingOffice.Style.Add("display", "block");

            ddl_CO.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            //ddl_PeriodFrom.Attributes.Add("onchange", "ctl00_ContentPlaceHolder1_ddl_PeriodTo.refresh;");
        }


        private ReportClass genReport()
        {
            int UserId;
            int i;

            HasAccessRights_SuperView = false;
            UserId = ((HasAccessRights_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);

            int BaseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            string BaseCurrencyCode = ddl_BaseCurrency.selectedText;

            DateTime InvoiceDateFrom = DateTime.MinValue;
            DateTime InvoiceDateTo = DateTime.MinValue;
            if (rad_InvoiceDate.Checked && txt_InvoiceDateFrom.Text.Trim() != "")
            {
                InvoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                InvoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            DateTime DeliveryDateFrom = DateTime.MinValue;
            DateTime DeliveryDateTo = DateTime.MinValue;
            if (rad_DeliveryDate.Checked && txt_DeliveryDateFrom.Text.Trim() != "")
            {
                DeliveryDateFrom = DateTimeUtility.getDate(txt_DeliveryDateFrom.Text.Trim());
                if (txt_DeliveryDateTo.Text.Trim() == "")
                    txt_DeliveryDateTo.Text = txt_DeliveryDateFrom.Text;
                DeliveryDateTo = DateTimeUtility.getDate(txt_DeliveryDateTo.Text.Trim());
            }

            int FiscalYear = -1;
            int PeriodFrom = -1;
            int PeriodTo = -1;
            if (rad_FiscalPeriod.Checked)
            {
                FiscalYear = Convert.ToInt32(ddl_Year.SelectedValue);
                PeriodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
                PeriodTo = Convert.ToInt32(ddl_PeriodTo.SelectedValue);
            }

            ArrayList OfficeList=new ArrayList();
            string OfficeName;
            bool FBSelected = false;
            OfficeName="";
            for (i = 0; i < cbl_Office.Items.Count; i++)
            {
                ListItem itm = cbl_Office.Items[i];
                if (itm.Selected)
                {
                    OfficeName += (OfficeName == "" ? "" : ", ") + itm.Text;
                    OfficeList.Add(itm.Value);
                    if (itm.Value == OfficeId.DG.Id.ToString())
                        FBSelected = true;
                }
            }

            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);
            string handlingOfficeName = string.Empty;
            if (FBSelected)
                handlingOfficeName = (handlingOfficeId == -1 ? "ALL" : CommonManager.Instance.getDGHandlingOffice(handlingOfficeId).Description);

            int CountryOfOriginId = Convert.ToInt32(ddl_CO.SelectedValue);
            string CountryOfOriginName = ddl_CO.selectedText.Replace("--", "").Trim();

            int SeasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            string SeasonCode = ddl_Season.selectedText.Replace("--", "").Trim();

            ArrayList TradingAgencyList = new ArrayList();
            string TradingAgencyName;
            TradingAgencyName = "";
            for (i = 0; i < cbl_TradingAgency.Items.Count; i++)
            {
                if (cbl_TradingAgency.Items[i].Selected)
                {
                    TradingAgencyName += (TradingAgencyName == "" ? "" : ", ") + cbl_TradingAgency.Items[i].Text;
                    TradingAgencyList.Add(cbl_TradingAgency.Items[i].Value);
                }
            }

            ArrayList PurchaseTermList = new ArrayList();
            string PurchaseTermName;
            PurchaseTermName = "";
            for (i = 0; i < cbl_PurchaseTerm.Items.Count; i++)
            {
                if (cbl_PurchaseTerm.Items[i].Selected)
                {
                    PurchaseTermName += (PurchaseTermName == "" ? "" : ", ") + cbl_PurchaseTerm.Items[i].Text;
                    PurchaseTermList.Add(cbl_PurchaseTerm.Items[i].Value);
                }
            }

            int GetActual = int.MinValue;
            int GetAccrual = int.MinValue;
            int GetRealized = int.MinValue;
            int GetNotYetCut = int.MinValue;

            if (cbx_GetActual.Checked)
                GetActual = 1;
            else
                GetActual = 0;

            if (cbx_GetAccrual.Checked) 
                GetAccrual = 1;
            else
                GetAccrual = 0;

            if (cbx_GetRealized.Checked)
                GetRealized = 1;
            else
                GetRealized = 0;

            if (cbx_GetNotYetCut.Checked)
                GetNotYetCut = 1;
            else
                GetNotYetCut = 0;

            OtherCostSummaryRpt rpt = AccountReportManager.Instance.getOtherCostSummaryReport(
                    InvoiceDateFrom, InvoiceDateTo, FiscalYear, PeriodFrom, PeriodTo, DeliveryDateFrom, DeliveryDateTo, 
                    OfficeList, OfficeName, handlingOfficeId, handlingOfficeName, CountryOfOriginId, CountryOfOriginName, SeasonId, SeasonCode, TradingAgencyList, TradingAgencyName, PurchaseTermList, PurchaseTermName, 
                    BaseCurrencyId, BaseCurrencyCode, GetActual, GetAccrual, GetRealized, GetNotYetCut, UserId);

            return rpt;
        }

        protected void rad_FiscalPeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_FiscalPeriod.Checked)
            {
                txt_DeliveryDateFrom.Enabled = false;
                txt_DeliveryDateTo.Enabled = false;

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
                txt_DeliveryDateFrom.Enabled = false;
                txt_DeliveryDateTo.Enabled = false;

                txt_InvoiceDateFrom.Enabled = true;
                txt_InvoiceDateTo.Enabled = true;

                ddl_Year.Enabled = false;
                ddl_PeriodFrom.Enabled = false;
                ddl_PeriodTo.Enabled = false;

            }
        }


        protected void rad_DeliveryDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_DeliveryDate.Checked)
            {
                txt_DeliveryDateFrom.Enabled = true;
                txt_DeliveryDateTo.Enabled = true;

                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false;

                ddl_Year.Enabled = false;
                ddl_PeriodFrom.Enabled = false;
                ddl_PeriodTo.Enabled = false;
            }
        }


        protected void ddl_PeriodFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_PeriodTo.SelectedIndex = ddl_PeriodFrom.SelectedIndex;
        }


        protected void cbl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            tr_HandlingOffice_Refresh();
            return;
            if (CommonManager.Instance.getDGOfficeGroupIdList().Contains(Convert.ToInt32(cbl_Office.SelectedItem.Value)))
            {
                tr_HandlingOffice.Style.Add("display", "block");
            }
            else
            {
                tr_HandlingOffice.Style.Add("display", "none");
                ddl_HandlingOffice.SelectedIndex = 0;
            }

            return;
        }

        private void tr_HandlingOffice_Refresh()
        {
            bool FBSelected = false;
            for (int i = 0; i < cbl_Office.Items.Count; i++)
            {
                ListItem itm = cbl_Office.Items[i];
                if (itm.Value == OfficeId.DG.Id.ToString())
                {
                    FBSelected = true;
                    tr_HandlingOffice.Style.Add(HtmlTextWriterStyle.Display, itm.Selected ? "block" : "none");
                    break;
                }
            }
            if (!FBSelected)
                ddl_HandlingOffice.SelectedIndex = 0;
        }

    }
}