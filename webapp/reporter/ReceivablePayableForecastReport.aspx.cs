using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.helper;
using com.next.isam.dataserver.worker;
using com.next.common.domain;
using com.next.isam.reporter.accounts;
using com.next.common.web.commander;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class AccountsReceivablePayableForecast : com.next.isam.webapp.usercontrol.PageTemplate
    {
        bool HasAccessRights_SuperView;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControl();
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport("REPORT"), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "Payable and Receivable Forecast Report");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport("EXCEL"), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "Payable and Receivable Forecast Report");
        }

        void initControl()
        {
            int nUserId;

            HasAccessRights_SuperView = false;
            //HasAccessRights_SuperView = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcApplication.Id, ISAMModule.lcApplication.SuperView);
            //********** For Testing **************************
            //if (this.LogonUserId==574)
            //  HasAccessRights_SuperView = true;
            //********** For Testing **************************
            nUserId = ((HasAccessRights_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);

            txt_ReportDate.Text = DateTimeUtility.getDateString(DateTime.Today);

            //ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

            //cbl_Office.DataSource = WebUtil.getAccessOfficeByUserId(this.LogonUserId);
            cbl_Office.DataSource = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            cbl_Office.DataTextField = "Description";
            cbl_Office.DataValueField = "OfficeId";
            cbl_Office.DataBind();
            foreach (ListItem item in cbl_Office.Items)
            {
                item.Text = item.Text.Replace("Office", "").Trim();
                item.Selected = true;
            }

            ddl_PaymentTerm.bindList(CommonWorker.Instance.getPaymentTermList(), "PaymentTermDescription", "PaymentTermId", "", "--All--", GeneralCriteria.ALL.ToString());
        }

        private ReportClass genReport(string ExportType)
        {
            int i;
            
            DateTime reportDate;
            if (!DateTime.TryParse(txt_ReportDate.Text, out reportDate))
                reportDate = DateTime.MinValue;

            ArrayList officeIdList = new ArrayList();
            ArrayList officeDescList = new ArrayList();
            foreach (ListItem item in cbl_Office.Items)
                if (item.Selected)
                {
                    officeIdList.Add(Convert.ToInt32(item.Value));
                    officeDescList.Add(item.Text);
                }

            ArrayList paymentTermIdList = new ArrayList();
            ArrayList paymentTermDescList = new ArrayList();
            if (ddl_PaymentTerm.SelectedValue != "-1")
            {
                paymentTermIdList.Add(Convert.ToInt32(ddl_PaymentTerm.SelectedValue));
                paymentTermDescList.Add(ddl_PaymentTerm.selectedText);
            }
            else
                for (i = 1; i < ddl_PaymentTerm.Items.Count; i++)
                {
                    paymentTermIdList.Add(Convert.ToInt32(ddl_PaymentTerm.Items[i].Value));
                    paymentTermDescList.Add(ddl_PaymentTerm.Items[i].Text);
                }
                
            ReceivablePayableForecastRpt rpt = AccountReportManager.Instance.getReceivablePayableForecastReport(
                reportDate, officeIdList, officeDescList, paymentTermIdList, paymentTermDescList, this.LogonUserId, ExportType);

            return rpt;
        }
    }
}
