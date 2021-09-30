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
    public partial class EpicorInterfaceLogReport : com.next.isam.webapp.usercontrol.PageTemplate
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

            //this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());
            //this.ddl_Office.bindList(addOfficeGroup(userOfficeList), "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
            //ddl_HandlingOffice.Visible = true;


        }


        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            //ReportHelper.export(genReport(), HttpContext.Current.Response,
            //        CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "SunInterfaceLogReport");
            ReportClass rpt = genReport();
            rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, HttpContext.Current.Response, true, "EpicorInterfaceLogReport");

        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "EpicorInterfaceLogReport");
        }

        private ReportClass genReport()
        {
            DateTime interfaceDateFrom = DateTime.Now;
            DateTime interfaceDateTo = DateTime.Now;

            if (txt_InterfaceDateFrom.Text.Trim() != "")
            {
                interfaceDateFrom = DateTimeUtility.getDate(txt_InterfaceDateFrom.Text.Trim());
                if (txt_InterfaceDateTo.Text.Trim() == "")
                    txt_InterfaceDateTo.Text = txt_InterfaceDateFrom.Text;
                interfaceDateTo = DateTimeUtility.getDate(txt_InterfaceDateTo.Text.Trim());
            }
            ArrayList officeList = new ArrayList();
            foreach(ListItem itm in cbl_Office.Items)
                if (itm.Selected)
                    officeList.Add(itm);
            ReportClass rpt = AccountReportManager.Instance.getEpicorInterfaceLogReport(interfaceDateFrom, interfaceDateTo, officeList, this.LogonUserId);
            return rpt;
        }


        protected void cbl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            return;
        }

    }
}
