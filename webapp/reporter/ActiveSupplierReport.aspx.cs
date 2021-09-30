using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.helper;
using com.next.isam.domain.common;
using com.next.isam.appserver.common;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class ActiveSupplierReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());

            txt_DeliveryDateFrom.Text = DateTimeUtility.getDateString(DateTime.Today);
            txt_DeliveryDateTo.Text = DateTimeUtility.getDateString(DateTime.Today.AddDays(180));
            txt_NoOfDelivery.Text = "5";
            txt_DeliveryDateFrom.Attributes.Add("onblur", "dateDiff()");
            txt_DeliveryDateTo.Attributes.Add("onblur", "dateDiff()");

            txt_Supplier.setWidth(305);
            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();

            foreach (ListItem item in cbl_Customer.Items)
            {
                if (item.Value == CustomerDef.Id.directory.GetHashCode().ToString() || item.Value == CustomerDef.Id.retail.GetHashCode().ToString())
                    item.Selected = true;
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs arg)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "ActiveSupplierReport");
        }

        protected void btn_Export_Click(object sender, EventArgs arg)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "ActiveSupplierReport");
        }

        private ReportClass genReport()
        {
            DateTime deliveryDateFrom = DateTime.Parse(txt_DeliveryDateFrom.Text);
            DateTime deliveryDateTo = DateTime.Parse(txt_DeliveryDateTo.Text);
            DateTime pastDeliveryFrom = deliveryDateFrom.AddMonths(-6);
            int minDelivery = Convert.ToInt32(txt_NoOfDelivery.Text);

            //int officeId = Convert.ToInt32(ddl_Office.SelectedValue);
            int officeGroupId = Convert.ToInt32(ddl_Office.SelectedValue);
            string officeGroupName = ddl_Office.selectedText;
            int productTeamId = uclProductTeam.ProductCodeId == int.MinValue ? -1 : uclProductTeam.ProductCodeId;
            int vendorId = txt_Supplier.VendorId == int.MinValue ? -1 : txt_Supplier.VendorId;
            int orderType = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            
            TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (item.Selected)
                    customerTypeCollector.append(Convert.ToInt32(item.Value));
            }

            TypeCollector workflowstatusList = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_wfs.Items)
            {
                if (item.Selected)
                    workflowstatusList.append(Convert.ToInt32(item.Value));
            }

            return AccountReportManager.Instance.getActiveSupplierReport(officeGroupId, productTeamId, pastDeliveryFrom, deliveryDateFrom, deliveryDateTo,
                minDelivery, vendorId, orderType, workflowstatusList, customerTypeCollector, officeGroupName, this.LogonUserId);
        }
    }
}
