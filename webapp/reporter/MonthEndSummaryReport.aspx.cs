using System;
using System.Collections;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using System.Configuration;
using System.Data;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.webapp.commander;
using com.next.isam.domain.types;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.invoice;

using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.reporter.accounts;
using com.next.isam.dataserver.worker;



namespace com.next.isam.webapp.reporter
{


    public partial class MonthEndSummaryReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        bool HasAccessRights_SuperView = false;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                initControl();
            }

        }

        protected void btn_Preview_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport("PDF"), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "MonthEndSummary");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport("EXCEL"), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "MonthEndSummary");
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
            ddl_PeriodNo.SelectedValue = AccountingPeriod.ToString();

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
            foreach (ListItem item in cbl_Office.Items)
            {
                item.Text = item.Text.Replace("Office", "").Trim();
                item.Selected = true;
            }

        }


        private ReportClass genReport(string ExportFormat)
        {
            int UserId;
            int i;

            // Report Format : 'PDF' or 'EXCEL'
            HasAccessRights_SuperView = false;
            UserId = ((HasAccessRights_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);

            int BaseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            string BaseCurrencyCode = ddl_BaseCurrency.selectedText;
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);

            int FiscalYear = -1;
            int PeriodNo = -1;
            FiscalYear = Convert.ToInt32(ddl_Year.SelectedValue);
            PeriodNo = Convert.ToInt32(ddl_PeriodNo.SelectedValue);

            ArrayList OfficeList=new ArrayList();
            string OfficeName;
            OfficeName="";
            for (i = 0; i < cbl_Office.Items.Count; i++)
            {
                if (cbl_Office.Items[i].Selected)
                {
                    OfficeName += (OfficeName == "" ? "" : ", ") + cbl_Office.Items[i].Text;
                    OfficeList.Add(cbl_Office.Items[i].Value);
                }
            }

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

            MonthEndSummaryRpt rpt = AccountReportManager.Instance.getMonthEndSummaryReport(FiscalYear, PeriodNo, isSampleOrder, 
                OfficeList, OfficeName, TradingAgencyList, TradingAgencyName, PurchaseTermList, PurchaseTermName, ExportFormat, UserId);

            return rpt;
        }


    }
}