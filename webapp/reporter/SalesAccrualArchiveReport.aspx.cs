using System;
using System.Collections;
using System.Web;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.reporter.accounts;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.reporter.helper;

namespace com.next.isam.webapp.reporter
{
    public partial class SalesAccrualArchiveReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

                this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId");
                if (userOfficeList.Count == 1)
                {
                    OfficeRef oref = (OfficeRef) userOfficeList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                }

                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 3).ToString(), (calenderDef.BudgetYear - 3).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 2).ToString(), (calenderDef.BudgetYear - 2).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 1).ToString(), (calenderDef.BudgetYear - 1).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_Year.selectByValue(calenderDef.BudgetYear.ToString());
            }
        }

        private ReportClass getReport()
        {
            AccrualArchiveReport rpt = AccountReportManager.Instance.getSalesAccrualArchiveReport(
                int.Parse(this.ddl_Office.SelectedValue), 
                int.Parse(this.ddl_Year.SelectedValue),
                int.Parse(this.ddl_Period.SelectedValue),
                this.LogonUserId);
            return rpt;
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            ReportHelper.export(getReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "Sales Accrual Archive Report");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(getReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "Sales Accrual Archive Report");
        }

    }
}
