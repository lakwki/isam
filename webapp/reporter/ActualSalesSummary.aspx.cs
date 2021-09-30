using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.accounts;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class ActualSalesSummary : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControl();
            }
        }

        void initControl()
        {
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

            this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_BaseCurrency.bindList(CommonUtil.getCurrencyList(), "CurrencyCode", "CurrencyId", "3");
            ddl_CO.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_LoadingPort.bindList(WebUtil.getShipmentPortList(), "ShipmentPortDescription", "ShipmentPortId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_PurchaseTerm.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());

            AccountFinancialCalenderDef finCalDef = CommonUtil.getCurrentFinancialPeriod(9);
            ddl_Year.DataSource = WebUtil.getBudgetYearList();
            ddl_Year.DataBind();
            ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();

            ddl_PeriodFrom.SelectedValue = finCalDef.Period.ToString();
            ddl_PeriodTo.SelectedValue = finCalDef.Period.ToString();

            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);


            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();

            cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            cbl_TradingAgency.DataTextField = "ShortName";
            cbl_TradingAgency.DataValueField = "TradingAgencyId";
            cbl_TradingAgency.DataBind();

            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                item.Selected = true;
            }
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                    item.Selected = true;
            }

        }

        private ReportClass genReport()
        {
            DateTime invoiceUploadDateFrom = DateTime.MinValue;
            DateTime invoiceUploadDateTo = DateTime.MinValue;
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            int isSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue);
            int isOPROrder = Convert.ToInt32(ddl_OPROrder.SelectedValue);
            int fiscalYear = -1;
            int periodFrom = -1;
            int periodTo = -1;
            int isActual = -1;
            int isRealized = -1;
            int countryOfOriginId = Convert.ToInt32(ddl_CO.SelectedValue);
            int officeId = Convert.ToInt32(ddl_Office.SelectedValue);
            int departmentId = ddl_Department.SelectedValue == "" ? -1 : Convert.ToInt32(ddl_Department.SelectedValue);
            int seasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            int shipmentPortId = Convert.ToInt32(ddl_LoadingPort.SelectedValue);
            int baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            int termOfPurchaseId = Convert.ToInt32(ddl_PurchaseTerm.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);

            if (rad_FiscalPeriod.Checked)
            {
                fiscalYear = Convert.ToInt32(ddl_Year.SelectedValue);
                periodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
                periodTo = Convert.ToInt32(ddl_PeriodTo.SelectedValue);
                isActual = ckb_Actual.Checked ? 1 : 0;
                isRealized = ckb_Realized.Checked ? 1 : 0;
            }
            else
            {
                if (txt_InvoiceDateFrom.Text.Trim() != "")
                {
                    invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                    if (txt_InvoiceDateTo.Text.Trim() == "")
                        txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                    invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
                }
            }
            if (txt_InvoiceUploadDateFrom.Text.Trim() != "")
            {
                invoiceUploadDateFrom = DateTimeUtility.getDate(txt_InvoiceUploadDateFrom.Text.Trim());
                if (txt_InvoiceUploadDateTo.Text.Trim() == "")
                    txt_InvoiceUploadDateTo.Text = txt_InvoiceUploadDateFrom.Text;
                invoiceUploadDateTo = DateTimeUtility.getDate(txt_InvoiceUploadDateTo.Text.Trim());
            }

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

            return AccountReportManager.Instance.getActualSalesSummaryReport(baseCurrencyId, invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom,
                invoiceUploadDateTo, fiscalYear, periodFrom, periodTo, isActual, isRealized, officeId, officeIdList, departmentId, productTeamList, seasonId,
                countryOfOriginId, shipmentPortId, customerTypeCollector, tradingAgencyCollector, termOfPurchaseId, isSZOrder, isUTOrder, isOPROrder, isLDPOrder, this.LogonUserId);
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
                ckb_Actual.Enabled = false;
                ckb_Realized.Enabled = false;
            }
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
                ckb_Actual.Enabled = true;
                ckb_Realized.Enabled = true;
            }
        }
        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_Office.SelectedValue == "-1")
            {
                ddl_Department.Items.Clear();
            }
            else
            {
                ddl_Department.Items.Clear();
                ArrayList arr_department = CommonUtil.getProductDepartmentListByCriteria(this.LogonUserId, Convert.ToInt32(ddl_Office.SelectedValue), GeneralCriteria.ALL);
                ddl_Department.bindList(arr_department, "Description", "ProductDepartmentId", "", "--All--", GeneralCriteria.ALL.ToString());
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "ActualSalesSummary");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "ActualSalesSummary");
        }

        protected void ddl_PeriodFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_PeriodTo.SelectedIndex = ddl_PeriodFrom.SelectedIndex;
        }
    }
}
