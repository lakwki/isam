using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.infra.util;
using com.next.infra.web;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.types;
using com.next.isam.domain.nontrade;
using com.next.isam.appserver.account;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.accounts;
using CrystalDecisions.CrystalReports.Engine;

namespace com.next.isam.webapp.nontrade
{
    public partial class NonTradeSearchEngine : com.next.isam.webapp.usercontrol.PageTemplate
    {
        static ReportCriteria reportCriteria;

        private ArrayList vwNTInvoiceSearchResult
        {
            set
            {
                ViewState["NTInvoiceSearchResult"] = value;
            }
            get
            {
                return (ArrayList)ViewState["NTInvoiceSearchResult"];
            }
        }

        private ReportCriteria vwNTInvoiceSearchCriteria
        {
            set
            {
                ViewState["NTInvoiceSearchCriteria"] = value;
            }
            get
            {
                return (ReportCriteria)ViewState["NTInvoiceSearchCriteria"];
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                txt_SupplierName.setWidth(305);
                txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.ntVendor);
                txt_SubmittedBy.setWidth(305);
                txt_SubmittedBy.initControl(webservices.UclSmartSelection.SelectionList.user);

                //ArrayList officeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

                ddl_Office.bindList(WebUtil.getNTOfficeList(this.LogonUserId), "OfficeCode", "OfficeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                AccountFinancialCalenderDef finCalDef = CommonUtil.getCurrentFinancialPeriod(9);
                ddl_Year.DataSource = WebUtil.getBudgetYearList();
                ddl_Year.DataBind();
                ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();
                ddl_PeriodFrom.SelectedValue = finCalDef.Period.ToString();
                ddl_PeriodTo.SelectedValue = finCalDef.Period.ToString();

                ddl_ExpenseType.bindList(WebUtil.getNTExpenseTypeList(), "ExpenseType", "ExpenseTypeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                //ddl_Status.bindList(NTWFS.getCollectionValues(), "Name", "Id", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                cbl_Status.DataSource = NTWFS.getCollectionValues();
                cbl_Status.DataTextField = "Name";
                cbl_Status.DataValueField = "Id";
                cbl_Status.DataBind();
                foreach (ListItem item in cbl_Status.Items)
                    item.Selected = true;

                NTMonthEndStatusDef statusDef = (NTMonthEndStatusDef)WebUtil.getCurrentNTMonthEndStatus(ConvertUtility.createArrayList(this.LogonUserHomeOffice))[0];
                if (statusDef.Status != NTMonthEndStatusDef.OPEN || CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.Id, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.Audit))
                {
                    btnNew.Visible = false;

                    if (statusDef.Status != NTMonthEndStatusDef.OPEN)
                    {
                        div_monthEnd.Visible = true;
                        lbl_Office.Text = this.LogonUserHomeOffice.OfficeCode;
                    }
                }

                if (!CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.Id, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.ViewForAccountsUser))
                    gv_Invoice.Columns[14].Visible = false;
            }
        }

        private ReportCriteria getSearchCriteria()
        {
            ReportCriteria criteria = new ReportCriteria();
            criteria.officeId = int.Parse(ddl_Office.SelectedValue);
            if (rad_FiscalPeriod.Checked)
            {
                criteria.FiscalYear = int.Parse(ddl_Year.SelectedValue);
                criteria.PeriodFrom = int.Parse(ddl_PeriodFrom.SelectedValue);
                criteria.PeriodTo = int.Parse(ddl_PeriodTo.SelectedValue);
            }
            else
            {
                criteria.FiscalYear = GeneralCriteria.ALL;
                criteria.PeriodFrom = GeneralCriteria.ALL;
                criteria.PeriodTo = GeneralCriteria.ALL;
            }
            if (rad_InvoiceDate.Checked)
            {
                criteria.invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text);
                criteria.invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text);
            }
            else
            {
                criteria.invoiceDateFrom = DateTime.MinValue;
                criteria.invoiceDateTo = DateTime.MinValue;
            }
            criteria.expenseTypeId = int.Parse(ddl_ExpenseType.SelectedValue);
            criteria.dueDateFrom = DateTimeUtility.getDate(txt_DueDateFrom.Text);
            criteria.dueDateTo = DateTimeUtility.getDate(txt_DueDateTo.Text);
            criteria.invoiceNoFrom = txt_InvoiceNoFrom.Text.Trim();
            criteria.invoiceNoTo = txt_InvoiceNoTo.Text.Trim();
            criteria.customerNoFrom = txt_CustomerNoFrom.Text.Trim();
            criteria.customerNoTo = txt_CustomerNoTo.Text.Trim();
            criteria.nslRefNoFrom = txt_NSLRefNoFrom.Text.Trim();
            criteria.nslRefNoTo = txt_NSLRefNoTo.Text.Trim();
            criteria.ntVendorId = txt_SupplierName.NTVendorId == int.MinValue ? GeneralCriteria.ALL : txt_SupplierName.NTVendorId;
            criteria.submittedBy = txt_SubmittedBy.UserId == int.MinValue ? GeneralCriteria.ALL : txt_SubmittedBy.UserId;

            //criteria.workflowStatusId = int.Parse(ddl_Status.SelectedValue);
            criteria.workflowStatusIdList = TypeCollector.Inclusive;
            foreach (ListItem itm in cbl_Status.Items)
                if (itm.Selected)
                    criteria.workflowStatusIdList.append(int.Parse(itm.Value));
            criteria.currencyId = GeneralCriteria.ALL;
            criteria.paymentMethodId = GeneralCriteria.ALL;
            return criteria;
        }

        private ArrayList getSearchResult()
        {
            reportCriteria = getSearchCriteria();

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetNTInvoiceList);

            if (reportCriteria.officeId != -1)
                Context.Items.Add(AccountCommander.Param.officeList, TypeCollector.createNew(reportCriteria.officeId));
            else
            {
                TypeCollector officeIdList = TypeCollector.Inclusive;
                ArrayList userOfficeList = WebUtil.getNTOfficeList(this.LogonUserId);

                foreach (OfficeRef office in userOfficeList)
                {
                    officeIdList.append(office.OfficeId);
                }
                Context.Items.Add(AccountCommander.Param.officeList, officeIdList);
            }
            Context.Items.Add(AccountCommander.Param.officeId, reportCriteria.officeId);
            Context.Items.Add(AccountCommander.Param.fiscalYear, reportCriteria.FiscalYear);
            Context.Items.Add(AccountCommander.Param.periodFrom, reportCriteria.PeriodFrom);
            Context.Items.Add(AccountCommander.Param.periodTo, reportCriteria.PeriodTo);
            Context.Items.Add(AccountCommander.Param.invoiceDateFrom, reportCriteria.invoiceDateFrom);
            Context.Items.Add(AccountCommander.Param.invoiceDateTo, reportCriteria.invoiceDateTo);
            Context.Items.Add(AccountCommander.Param.expenseTypeId, reportCriteria.expenseTypeId);
            Context.Items.Add(AccountCommander.Param.dueDateFrom, reportCriteria.dueDateFrom);
            Context.Items.Add(AccountCommander.Param.dueDateTo, reportCriteria.dueDateTo);
            Context.Items.Add(AccountCommander.Param.invoiceNoFrom, reportCriteria.invoiceNoFrom);
            Context.Items.Add(AccountCommander.Param.invoiceNoTo, reportCriteria.invoiceNoTo);
            Context.Items.Add(AccountCommander.Param.customerNoFrom, reportCriteria.customerNoFrom);
            Context.Items.Add(AccountCommander.Param.customerNoTo, reportCriteria.customerNoTo);
            Context.Items.Add(AccountCommander.Param.nslRefNoFrom, reportCriteria.nslRefNoFrom);
            Context.Items.Add(AccountCommander.Param.nslRefNoTo, reportCriteria.nslRefNoTo);
            Context.Items.Add(AccountCommander.Param.vendorId, reportCriteria.ntVendorId);
            Context.Items.Add(AccountCommander.Param.submittedBy, reportCriteria.submittedBy);
            //Context.Items.Add(AccountCommander.Param.workflowStatusId, reportCriteria.workflowStatusId);
            Context.Items.Add(AccountCommander.Param.workflowStatusList, reportCriteria.workflowStatusIdList);
            Context.Items.Add(AccountCommander.Param.userId, this.LogonUserId);

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.Id, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.ViewForAccountsUser))
                Context.Items.Add(AccountCommander.Param.departmentId, -1);
            else
            {
                UserRef user = CommonUtil.getUserByKey(this.LogonUserId);
                Context.Items.Add(AccountCommander.Param.departmentId, user.Department.DepartmentId);
            }

            forwardToScreen(null);

            return (ArrayList)Context.Items[AccountCommander.Param.invoiceList];
        }

        protected void Search(object sender, EventArgs arg)
        {
            ArrayList invoiceList = getSearchResult();
            pnl_Result.Visible = true;
            if (invoiceList.Count >= 200)
                div_Over200.Visible = true;
            else
                div_Over200.Visible = false;
            vwNTInvoiceSearchResult = invoiceList;
            gv_Invoice.DataSource = invoiceList;
            gv_Invoice.DataBind();
            vwNTInvoiceSearchCriteria = reportCriteria;
        }

        protected void Reset_OnClick(object sender, EventArgs e)
        {
            ddl_Office.SelectedIndex = -1;
            AccountFinancialCalenderDef finCalDef = CommonUtil.getCurrentFinancialPeriod(9);
            ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();
            ddl_PeriodFrom.SelectedValue = finCalDef.Period.ToString();
            ddl_PeriodTo.SelectedValue = finCalDef.Period.ToString();
            txt_InvoiceDateFrom.Text = string.Empty;
            txt_InvoiceDateTo.Text = string.Empty;
            ddl_ExpenseType.SelectedIndex = -1;
            txt_DueDateFrom.Text = string.Empty;
            txt_DueDateTo.Text = string.Empty;
            txt_InvoiceNoFrom.Text = string.Empty;
            txt_InvoiceNoTo.Text = string.Empty;
            txt_SupplierName.NTVendorId = int.MinValue;
            txt_SupplierName.clear();
            txt_SubmittedBy.UserId = int.MinValue;
            txt_SubmittedBy.clear();

            txt_CustomerNoFrom.Text = string.Empty;
            txt_CustomerNoTo.Text = string.Empty;
            //ddl_Status.SelectedIndex = -1;
            foreach (ListItem itm in cbl_Status.Items)
                itm.Selected = true;
            txt_NSLRefNoFrom.Text = string.Empty;
            txt_NSLRefNoTo.Text = string.Empty;

            vwNTInvoiceSearchCriteria = null;
            vwNTInvoiceSearchResult = null;
            gv_Invoice.DataSource = null;
            gv_Invoice.DataBind();
            pnl_Result.Visible = false;
            div_Over200.Visible = false;
        }

        protected void InvoiceRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NTInvoiceDef invoiceDef = (NTInvoiceDef)e.Row.DataItem;

                ((LinkButton)e.Row.FindControl("lnk_NSLRefNo")).Attributes.Add("onclick", string.Format("openInvoiceWindow({0}); return false;", invoiceDef.InvoiceId.ToString()));
                ((LinkButton)e.Row.FindControl("lnk_InvoiceNo")).Attributes.Add("onclick", string.Format("openInvoiceWindow({0}); return false;", invoiceDef.InvoiceId.ToString()));
                ((LinkButton)e.Row.FindControl("lnk_CustomerNo")).Attributes.Add("onclick", string.Format("openInvoiceWindow({0}); return false;", invoiceDef.InvoiceId.ToString()));

                if (invoiceDef.DueDate == DateTime.MinValue)
                    ((Label)e.Row.FindControl("lbl_DueDate")).Text = string.Empty;

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.Id, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.ViewForAccountsUser))
                    ((Label)e.Row.FindControl("lbl_HasDebitNote")).Text = NonTradeManager.Instance.isNTInvoiceHasDebitNote(invoiceDef.InvoiceId) ? "YES" : "NO";

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.Id, com.next.common.domain.module.ISAMModule.ntExpenseInvoice.ViewForAccountsUser))
                    ((Label)e.Row.FindControl("lbl_IsFixedAsset")).Text = NonTradeManager.Instance.isNTInvoiceHasFixedAsset(invoiceDef.InvoiceId) ? "YES" : "NO";

                Label lbl = ((Label)e.Row.FindControl("lbl_FirstApprover"));
                if (invoiceDef.AccountFirstApproverId != -1)
                    lbl.Text = CommonUtil.getUserByKey(invoiceDef.AccountFirstApproverId).DisplayName;
            }
        }

        protected void InvoiceOnSort(object sender, GridViewSortEventArgs e)
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

            NTInvoiceDef.NTInvoiceComparer.CompareType compareType;

            if (sortExpression == "Office")
                compareType = NTInvoiceDef.NTInvoiceComparer.CompareType.Office;
            else if (sortExpression == "InvoiceNo")
                compareType = NTInvoiceDef.NTInvoiceComparer.CompareType.InvoiceNo;
            else if (sortExpression == "Vendor")
                compareType = NTInvoiceDef.NTInvoiceComparer.CompareType.Vendor;
            else if (sortExpression == "Requester")
                compareType = NTInvoiceDef.NTInvoiceComparer.CompareType.Requester;
            else
                compareType = NTInvoiceDef.NTInvoiceComparer.CompareType.FirstApprover;

            vwNTInvoiceSearchResult.Sort(new NTInvoiceDef.NTInvoiceComparer(compareType, sortDirection));
            gv_Invoice.DataSource = vwNTInvoiceSearchResult;
            gv_Invoice.DataBind();
        }

        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            int officeId = int.Parse(ddl_Office.SelectedValue);
            //if (officeId == -1)
            //    ddl_ExpenseType.bindList(new ArrayList(), "ExpenseType", "ExpenseTypeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            //else
            //{
            ArrayList expenseTypeList = WebUtil.getNTExpenseTypeListByOfficeId(officeId);
            expenseTypeList.Sort(new NTExpenseTypeRef.ExpenseTypeComparer(NTExpenseTypeRef.ExpenseTypeComparer.CompareType.ExpenseType));
            ddl_ExpenseType.bindList(expenseTypeList, "ExpenseType", "ExpenseTypeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            //}
        }

        protected void rad_InvoiceDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_InvoiceDate.Checked)
            {
                txt_InvoiceDateFrom.Enabled = true;
                txt_InvoiceDateTo.Enabled = true;
                txt_InvoiceNoFrom.Enabled = false;
                txt_InvoiceNoTo.Enabled = false;
                ddl_Year.Enabled = false;
                ddl_PeriodFrom.Enabled = false;
                ddl_PeriodTo.Enabled = false;
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
                ddl_Year.Enabled = true;
                ddl_PeriodFrom.Enabled = true;
                ddl_PeriodTo.Enabled = true;
            }
        }

        /*
        protected void ExportDetail(object sender, EventArgs arg)
        {
            ReportCriteria criteria = vwNTInvoiceSearchCriteria;
            if (criteria == null)
                criteria = getSearchCriteria();
            TypeCollector workflowStatusList = TypeCollector.Inclusive;
            TypeCollector officeIdList = TypeCollector.Inclusive;

            if (criteria.officeId != -1)
                officeIdList = TypeCollector.createNew(criteria.officeId);
            else
            {
                ArrayList userOfficeList = WebUtil.getNTOfficeList(this.LogonUserId);
                if (userOfficeList != null)
                    foreach (OfficeRef office in userOfficeList)
                        officeIdList.append(office.OfficeId);
            }
            TypeCollector invoiceIdList = TypeCollector.Inclusive;
            if (vwNTInvoiceSearchResult == null)
                vwNTInvoiceSearchResult = getSearchResult();
            else
                if (vwNTInvoiceSearchResult.Count == 0)
                    vwNTInvoiceSearchResult = getSearchResult();

            if (vwNTInvoiceSearchResult!=null)
            {
                foreach (NTInvoiceDef invoice in vwNTInvoiceSearchResult)
                    invoiceIdList.append(invoice.InvoiceId);
               //ReportClass rpt = NonTradeManager.Instance.GenNTInvoiceDetailExportList(invoiceIdList, officeIdList, criteria.expenseTypeId, criteria.invoiceDateFrom, criteria.invoiceDateTo, criteria.dueDateFrom, criteria.dueDateTo, criteria.invoiceNoFrom, criteria.invoiceNoTo, 
                ReportClass rpt = AccountReportManager.Instance.GenNTInvoiceDetailExportList("SEARCH", invoiceIdList, officeIdList, criteria.expenseTypeId, criteria.invoiceDateFrom, criteria.invoiceDateTo, criteria.dueDateFrom, criteria.dueDateTo, criteria.invoiceNoFrom, criteria.invoiceNoTo, 
                    criteria.customerNoFrom, criteria.customerNoTo, criteria.nslRefNoFrom, criteria.nslRefNoTo, criteria.ntVendorId, criteria.workflowStatusIdList, criteria.currencyId, criteria.paymentMethodId, DateTime.MinValue, DateTime.MinValue,string.Empty,string.Empty, string.Empty);
                if (invoiceIdList.Values.Count > 0 && rpt != null)
                    ReportHelper.export(rpt, HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "NTInvoiceSearchEngine");
            }
        }
        */

        protected void ExportDetail(object sender, EventArgs arg)
        {
            ArrayList invoiceList = getSearchResult();
            pnl_Result.Visible = true;
            vwNTInvoiceSearchResult = invoiceList;
            gv_Invoice.DataSource = invoiceList;
            gv_Invoice.DataBind();

            ReportCriteria criteria = reportCriteria;
            TypeCollector officeIdList = TypeCollector.Inclusive;

            if (criteria.officeId != -1)
                officeIdList = TypeCollector.createNew(criteria.officeId);
            else
            {
                ArrayList userOfficeList = WebUtil.getNTOfficeList(this.LogonUserId);
                if (userOfficeList != null)
                    foreach (OfficeRef office in userOfficeList)
                        officeIdList.append(office.OfficeId);
            }
            TypeCollector invoiceIdList = TypeCollector.Inclusive;
            if (invoiceList != null)
            {
                foreach (NTInvoiceDef invoice in invoiceList)
                {
                    invoiceIdList.append(invoice.InvoiceId);
                    System.Diagnostics.Debug.WriteLine(invoice.InvoiceId.ToString() + ",");
                }
                ReportClass rpt = AccountReportManager.Instance.GenNTInvoiceDetailExportList("SEARCH", invoiceIdList, officeIdList, criteria.expenseTypeId, criteria.FiscalYear, criteria.PeriodFrom, criteria.PeriodTo, criteria.invoiceDateFrom, criteria.invoiceDateTo,
                    criteria.dueDateFrom, criteria.dueDateTo, DateTime.MinValue, DateTime.MinValue, criteria.invoiceNoFrom, criteria.invoiceNoTo, criteria.customerNoFrom, criteria.customerNoTo, criteria.nslRefNoFrom, criteria.nslRefNoTo,
                    criteria.ntVendorId, criteria.workflowStatusIdList, criteria.currencyId, criteria.paymentMethodId, DateTime.MinValue, DateTime.MinValue, string.Empty, string.Empty, string.Empty, GeneralCriteria.ALL);
                if (invoiceIdList.Values.Count > 0 && rpt != null)
                    ReportHelper.export(rpt, HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "NTInvoiceSearchEngine");
            }
        }

        [Serializable()]
        public partial class ReportCriteria
        {
            //public TypeCollector officeList { get; set; }
            public int officeId { get; set; }
            public int FiscalYear { get; set; }
            public int PeriodFrom { get; set; }
            public int PeriodTo { get; set; }
            public string invoiceNoFrom { get; set; }
            public string invoiceNoTo { get; set; }
            public string customerNoFrom { get; set; }
            public string customerNoTo { get; set; }
            public string nslRefNoFrom { get; set; }
            public string nslRefNoTo { get; set; }
            public DateTime invoiceDateFrom { get; set; }
            public DateTime invoiceDateTo { get; set; }
            public DateTime dueDateFrom { get; set; }
            public DateTime dueDateTo { get; set; }
            public int ntVendorId { get; set; }
            public int currencyId { get; set; }
            public int paymentMethodId { get; set; }
            public int expenseTypeId { get; set; }
            public TypeCollector workflowStatusIdList { get; set; }
            public int submittedBy { get; set; }
        }

    }

}
