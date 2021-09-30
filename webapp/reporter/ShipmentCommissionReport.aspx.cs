using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.accounts;
using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.appserver.common;
using com.next.isam.dataserver.worker;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class ShipmentCommissionReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            //this.ddl_Office.bindList(addOfficeGroup(userOfficeList), "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_HandlingOffice.Visible = true;

            ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_BaseCurrency.bindList(WebUtil.getCurrencyListForExchangeRate() , "CurrencyCode", "CurrencyId", "3");
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

            cbl_DesignGroup.DataSource = GeneralWorker.Instance.getDesignSourceList();
            cbl_DesignGroup.DataTextField = "DesignSourceCode";
            cbl_DesignGroup.DataValueField = "DesignSourceId";
            cbl_DesignGroup.DataBind();            

            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();

            cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            cbl_TradingAgency.DataTextField = "ShortName";
            cbl_TradingAgency.DataValueField = "TradingAgencyId";
            cbl_TradingAgency.DataBind();

            foreach (ListItem item in cbl_DesignGroup.Items)
            {
                item.Selected = true;
            }

            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
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
            int departmentId = -1; 

            int termOfPurchase = Convert.ToInt32(ddl_termOfPurchase.SelectedValue);
            int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue);
            int isOPROrder = Convert.ToInt32(ddl_OPROrder.SelectedValue);
            int isSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);
            int isTailoring = Convert.ToInt32(ddl_NSLTailoringOrder.SelectedValue);
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);

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
                        
            if (txt_InvoiceUploadDateFrom.Text.Trim() != "" && rad_ReportVersion.SelectedValue == "0")
            {   // Sun Version only
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

            int officeId = -1;
            int handlingOfficeId = -1;
            int officeGroupId = Convert.ToInt32(ddl_Office.SelectedValue);
            if (officeGroupId == OfficeId.DG.Id || officeGroupId == 101)
                handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);
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
            if (ddl_Department.SelectedValue == "" || ddl_Department.SelectedIndex==0)
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
                vendorId = txt_Supplier.VendorId ;

            TypeCollector designSourceList = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_DesignGroup.Items)
            {
                if (item.Selected)
                    designSourceList.append(Convert.ToInt32(item.Value));
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

            string supplierInvoiceNo = txt_SupplierInvoiceNo.Text.Trim();

            //com.next.isam.reporter.accounts .ShipmentCommissionReport  
            ReportClass rpt = AccountReportManager.Instance.getShipmentAndCommissionReport(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear,
                invoiceDateFrom, invoiceDateTo, invoiceUploadDateFrom, invoiceUploadDateTo, purchaseExtractDateFrom, purchaseExtractDateTo,
                budgetYear, periodFrom, isActual, isRealized, isAccrual, vendorId, baseCurrencyId,
                countryOfOriginId, officeId, handlingOfficeId, officeIdList, seasonId, productTeamList, customerTypeCollector, tradingAgencyCollector, departmentId, departmentIdList, termOfPurchase, isSZOrder,
                isUTOrder, isOPROrder, isLDPOrder, isTailoring, isSampleOrder, designSourceList, string.Empty, string.Empty, supplierInvoiceNo, 0,  Int32.Parse(rad_ReportVersion.SelectedValue),  rad_SortField.SelectedValue, this.LogonUserId, "REPORT");

            return rpt;
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
        /*
        private void convertToDepartmentGroupList(ListItemCollection officeDepartmentList)
        { // Multiple Office
            string deptName;
            string deptId;
            ArrayList newDeptGroup = new ArrayList();
            foreach (ListItem deptItem in officeDepartmentList)
                if (deptItem.Text.IndexOf("All") >= 0)
                {
                    ListItem newDept = new ListItem();
                    newDept.Value = deptItem.Value;
                    newDept.Text = deptItem.Text;
                    newDeptGroup.Add(newDept);
                    break;
                }
            foreach (ListItem deptItem in officeDepartmentList)
            {
                bool found;
                if (deptItem.Text.IndexOf("(") > 0)
                {
                    deptName = deptItem.Text.Substring(0, deptItem.Text.IndexOf("(") - 1);
                    deptId = deptItem.Value;
                    found = false;
                    for (int i = 0; i < newDeptGroup.Count; i++)
                    {
                        ListItem itm=((ListItem)newDeptGroup[i]);
                        if (itm.Text.IndexOf("(")>=0)
                            if (itm.Text.Substring(0, itm.Text.IndexOf("(") - 1) == deptName)
                            {
                                itm.Value += "|" + deptId;
                                found = true;
                                break;
                            }
                    }
                    if (!found)
                    {
                        ListItem newDept = new ListItem();
                        newDept.Value = deptItem.Value;
                        newDept.Text = deptItem.Text;
                        //deptItem.Text = deptName;
                        newDeptGroup.Add(newDept);
                    }
                }
            }
            officeDepartmentList.Clear();
            for (int i = 0; i < newDeptGroup.Count; i++)
            {
                ListItem itm = (ListItem)newDeptGroup[i];
                if (itm.Value.IndexOf("|") > 0)
                {
                    itm.Text = itm.Text.Substring(0, itm.Text.IndexOf("(") - 1) + " (All)";
                }
                officeDepartmentList.Add(itm);
            }
        }

        protected ArrayList getProductDepartmentListByOfficeGroupId(int officeGroupId)
        {
            ArrayList arr_department = null;
            ArrayList officeList = CommonWorker.Instance.getOfficeListByReportOfficeGroupId(officeGroupId);
            foreach(OfficeRef office in officeList)
            {
                if (arr_department == null)
                    arr_department = CommonUtil.getProductDepartmentListByCriteria(this.LogonUserId, Convert.ToInt32(office.OfficeId), GeneralCriteria.ALL);
                else
                {
                    ArrayList dept = CommonUtil.getProductDepartmentListByCriteria(this.LogonUserId, Convert.ToInt32(office.OfficeId), GeneralCriteria.ALL);
                    foreach (ProductDepartmentRef rf in dept)
                        arr_department.Add(rf);
                }
            }
            return arr_department;
        }
        */

        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_Department.Items.Clear();
            //uclProductTeam.Enabled = !ddl_Office.selectedText.Contains("+");
            if (ddl_Office.SelectedValue != "-1")
            {
                ArrayList arr_department = WebUtil.getProductDepartmentListByOfficeGroupId(Convert.ToInt32(ddl_Office.SelectedValue), this.LogonUserId);
                ddl_Department.bindList(arr_department, "Description", "ProductDepartmentId", "", "--All--", GeneralCriteria.ALL.ToString());
                WebUtil.convertToDepartmentGroupList(ddl_Department.Items);
                //uclProductTeam.clear();
            }

            if (ddl_Office.SelectedValue == "17" || ddl_Office.SelectedValue == "101" || ddl_Office.SelectedValue=="-1")
                row_HandlingOffice.Visible = true;
            else
                row_HandlingOffice.Visible = false;
        }

        protected void rad_ReportVersion_SelectedChanged(object sender, EventArgs e)
        {
            if (rad_ReportVersion.SelectedValue == "0")
            {   // SUN version
                tr_InvoiceDate.Style.Add(HtmlTextWriterStyle.Display, "block");
                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false;
                tr_InvoiceUploadDate.Style.Add(HtmlTextWriterStyle.Display, "block");
                td_FiscalPeriod.Style.Add(HtmlTextWriterStyle.Display, "block");

            } if (rad_ReportVersion.SelectedValue == "1")
            {   // Epicor version
                tr_InvoiceDate.Style.Add(HtmlTextWriterStyle.Display, "none");
                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false;

                tr_InvoiceUploadDate.Style.Add(HtmlTextWriterStyle.Display, "none");

                rad_FiscalPeriod.Checked = true;
                td_FiscalPeriod.Style.Add(HtmlTextWriterStyle.Display, "none");
                ddl_Year.Enabled = true;
                ddl_PeriodFrom.Enabled = true;
                //ddl_PeriodTo.Enabled = true;
            }
        }

    }
}
