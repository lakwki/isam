using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CrystalDecisions.CrystalReports.Engine;
using com.next.infra.util;
using com.next.infra.web;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.types;
using com.next.isam.domain.nontrade;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.accounts;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;


namespace com.next.isam.webapp.reporter
{
    public partial class NonTradeExpenseStatementList : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                txt_SupplierName.setWidth(305);
                txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.ntVendor);

                ddl_Office.bindList(CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type), "OfficeCode", "OfficeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                ddl_ExpenseType.bindList(WebUtil.getNTExpenseTypeList(), "ExpenseType", "ExpenseTypeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
                ddl_Status.bindList(NTWFS.getCollectionValues(), "Name", "Id", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            }
        }
        

        ReportClass genReport()
        {
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            DateTime dueDateFrom = DateTime.MinValue;
            DateTime dueDateTo = DateTime.MinValue;
            string nsRefNoFrom, nsRefNoTo;
            int officeId = Convert.ToInt32(ddl_Office.SelectedValue);
            int ntVendorId = txt_SupplierName.NTVendorId;
            ntVendorId = (ntVendorId == int.MinValue ? -1 : ntVendorId);
            nsRefNoFrom = txt_NSRefNoFrom.Text;
            nsRefNoTo = txt_NSRefNoTo.Text;

            if (txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            if (txt_DueDateFrom.Text.Trim() != "")
            {
                dueDateFrom = DateTimeUtility.getDate(txt_DueDateFrom.Text.Trim());
                if (txt_DueDateTo.Text.Trim() == "")
                    txt_DueDateTo.Text = txt_DueDateFrom.Text;
                dueDateTo = DateTimeUtility.getDate(txt_DueDateTo.Text.Trim());
            }

            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (officeId == -1)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef office in userOfficeList)
                    officeIdList.append(office.OfficeId);
            }
            else
                officeIdList.append(Convert.ToInt32(ddl_Office.SelectedValue));

            TypeCollector ntStatusList = TypeCollector.Inclusive;
            if (ddl_Status.SelectedValue == "-1")
            {
                foreach (NTWFS status in NTWFS.getCollectionValues())
                    ntStatusList.append(status.Id);
            }
            else
                ntStatusList.append(Convert.ToInt32(ddl_Status.SelectedValue));


            return AccountReportManager.Instance.getNonTradeExpenseStatementList(officeIdList, invoiceDateFrom, invoiceDateTo,
                dueDateFrom, dueDateTo, nsRefNoFrom, nsRefNoTo, ntVendorId, ntStatusList, this.LogonUserId);
        }


        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "NonTradeExpenseStatementList");
        }


        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "NonTradeExpenseStatement");
        }
    }
}