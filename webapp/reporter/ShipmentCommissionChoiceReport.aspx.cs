using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.infra.util;
using com.next.isam.webapp.commander;
using com.next.isam.domain.types;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.invoice;
using com.next.isam.reporter.accounts;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.common;

using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;

namespace com.next.isam.webapp.reporter
{
    public partial class ShipmentCommissionChoiceReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControls();
            }
        }


        protected void initControls()
        {
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_BaseCurrency.bindList(WebUtil.getCurrencyListForExchangeRate(), "CurrencyCode", "CurrencyId", "3");
            ddl_CO.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_termOfPurchase.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());

            AccountFinancialCalenderDef finCalDef = CommonUtil.getCurrentFinancialPeriod(9);
            ddl_Year.DataSource = WebUtil.getBudgetYearList();
            ddl_Year.DataBind();
            ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();
            ddl_PeriodFrom.SelectedValue = finCalDef.Period.ToString();


            txt_Supplier.setWidth(305);
            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            cbl_TradingAgency.DataTextField = "ShortName";
            cbl_TradingAgency.DataValueField = "TradingAgencyId";
            cbl_TradingAgency.DataBind();


            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                item.Selected = true;
            }
        }

        protected void rad_InvoiceDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_InvoiceDate.Checked)
            {
                txt_InvoiceDateFrom.Enabled = true;
                txt_InvoiceDateTo.Enabled = true;

                txt_InvoiceNoFrom.Enabled = false;
                txt_InvoiceNoTo.Enabled = false;

                txt_InvoiceUploadDateFrom.Enabled = false;
                txt_InvoiceUploadDateTo.Enabled = false;

                ddl_Year.Enabled = false;
                ddl_PeriodFrom.Enabled = false;

                ckb_Actual.Enabled = false;
                ckb_Realized.Enabled = false;
                ckb_Accrual.Enabled = false;
            }
        }


        protected void rad_FiscalPeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_FiscalPeriod.Checked)
            {
                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false;

                txt_InvoiceNoFrom.Enabled = false;
                txt_InvoiceNoTo.Enabled = false;

                txt_InvoiceUploadDateFrom.Enabled = false;
                txt_InvoiceUploadDateTo.Enabled = false;

                ddl_Year.Enabled = true;
                ddl_PeriodFrom.Enabled = true;

                ckb_Actual.Enabled = true;
                ckb_Realized.Enabled = true;
                ckb_Accrual.Enabled = true;
            }
        }

        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_Department.Items.Clear();
            if (ddl_Office.SelectedValue != "-1")
            {
                ArrayList arr_department = WebUtil.getProductDepartmentListByOfficeGroupId(Convert.ToInt32(ddl_Office.SelectedValue), this.LogonUserId);
                ddl_Department.bindList(arr_department, "Description", "ProductDepartmentId", "", "--All--", GeneralCriteria.ALL.ToString());
                WebUtil.convertToDepartmentGroupList(ddl_Department.Items);
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "ShipmentCommission");

        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "ShipmentCommission");
        }

        private ReportClass genReport()
        {
            string invoicePrefix = "";
            int invoiceSeqFrom = 0;
            int invoiceSeqTo = 0;
            int invoiceYear = 0;
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            DateTime invoiceUploadDateFrom = DateTime.MinValue;
            DateTime invoiceUploadDateTo = DateTime.MinValue;
            DateTime purchaseExtractDateFrom = DateTime.MinValue;
            DateTime purchaseExtractDateTo = DateTime.MinValue;
            int budgetYear = -1;
            int periodFrom = -1;
            int isActual = -1;
            int isRealized = -1;
            int isAccrual = -1;
            int departmentId = -1; // ddl_Department.SelectedValue == "" ? -1 : Convert.ToInt32(ddl_Department.SelectedValue);
            int termOfPurchase = Convert.ToInt32(ddl_termOfPurchase.SelectedValue);


            if (rad_FiscalPeriod.Checked)
            {
                budgetYear = Convert.ToInt32(ddl_Year.SelectedValue);
                periodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
                isActual = ckb_Actual.Checked ? 1 : 0;
                isRealized = ckb_Realized.Checked ? 1 : 0;
                isAccrual = ckb_Accrual.Checked ? 1 : 0;
            }

            if (txt_InvoiceNoFrom.Text.Trim() != "")
            {
                invoicePrefix = WebUtil.getInvoicePrefix(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqFrom = WebUtil.getInvoiceSeq(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqTo = WebUtil.getInvoiceSeq(txt_InvoiceNoTo.Text.Trim());
                invoiceYear = WebUtil.getInvoiceYear(txt_InvoiceNoFrom.Text.Trim());
            }

            if (rad_InvoiceDate.Checked)
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            if (txt_InvoiceUploadDateFrom.Text.Trim() != "")
            {
                invoiceUploadDateFrom = DateTimeUtility.getDate(txt_InvoiceUploadDateFrom.Text.Trim());
                if (txt_InvoiceUploadDateTo.Text.Trim() == "")
                    txt_InvoiceUploadDateTo.Text = txt_InvoiceUploadDateFrom.Text;
                invoiceUploadDateTo = DateTimeUtility.getDate(txt_InvoiceUploadDateTo.Text.Trim());
            }

            if (txt_ExtractDateFrom.Text.Trim() != "")
            {
                purchaseExtractDateFrom = DateTimeUtility.getDate(txt_ExtractDateFrom.Text.Trim());
                if (txt_ExtractDateTo.Text.Trim() == "")
                    txt_ExtractDateTo.Text = txt_ExtractDateFrom.Text;
                purchaseExtractDateTo = DateTimeUtility.getDate(txt_ExtractDateTo.Text.Trim());
            }

            /*
            TypeCollector productTeamList = TypeCollector.Inclusive;
            if (uclProductTeam.ProductCodeId != int.MinValue)
                productTeamList.append(uclProductTeam.ProductCodeId);

            
            int officeId = Convert.ToInt32(ddl_Office.SelectedValue);
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

            TypeCollector productTeamList = TypeCollector.Inclusive;
            if (uclProductTeam.ProductCodeId != int.MinValue)
                productTeamList.append(uclProductTeam.ProductCodeId);
            else
            {
                foreach (int Id in officeIdList.Values)
                {
                    ArrayList pt = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, Id, this.LogonUserId, GeneralCriteria.ALLSTRING);
                    foreach (OfficeStructureRef os in pt)
                        productTeamList.append(os.OfficeStructureId);
                }
            }

            //departmentId = ddl_Department.SelectedValue == "" ? -1 : Convert.ToInt32(ddl_Department.SelectedValue);
            TypeCollector departmentIdList = TypeCollector.Inclusive;
            if (ddl_Department.SelectedValue == "" || ddl_Department.SelectedIndex == 0)
                departmentId = -1;
            else
            {
                string[] idList = ddl_Department.SelectedItem.Value.Split(char.Parse("|"));
                if (idList.Length > 0)
                    foreach (string str in idList)
                        departmentIdList.append(Convert.ToInt32(str));
            }

            int baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            int countryOfOriginId = Convert.ToInt32(ddl_CO.SelectedValue);
            int seasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            int vendorId = -1;

            if (txt_Supplier.VendorId != int.MinValue)
                vendorId = txt_Supplier.VendorId;

            TypeCollector designSourceList = TypeCollector.Inclusive;
            TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            customerTypeCollector.append(com.next.isam.domain.common.CustomerDef.Id.choice.GetHashCode());

            TypeCollector tradingAgencyCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                if (item.Selected)
                    tradingAgencyCollector.append(Convert.ToInt32(item.Value));
            }

            //com.next.isam.reporter.accounts .ShipmentCommissionReport  
            ReportClass rpt = AccountReportManager.Instance.getShipmentAndCommissionReport(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear,
                invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo, purchaseExtractDateFrom, purchaseExtractDateTo,
                budgetYear, periodFrom, isActual, isRealized, isAccrual, vendorId, baseCurrencyId,
                countryOfOriginId, officeId, -1, officeIdList, seasonId, productTeamList, customerTypeCollector, tradingAgencyCollector, departmentId, departmentIdList, termOfPurchase, -1,
                -1, -1, -1, -1, -1, designSourceList, string.Empty, string.Empty, string.Empty, 2, rad_SortField.SelectedValue, this.LogonUserId);

            return rpt;
        }

    }
}
