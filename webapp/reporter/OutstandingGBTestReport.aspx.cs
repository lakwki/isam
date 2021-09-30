using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls ;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.appserver.common;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.shipping;
using com.next.isam.reporter.helper;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class OutstandingGBTestReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
                this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());

                txt_Supplier.setWidth(305);
                txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

                cbl_ShipmentMethod.DataSource = WebUtil.getShipmentMethodList();
                cbl_ShipmentMethod.DataTextField = "ShipmentMethodDescription";
                cbl_ShipmentMethod.DataValueField = "ShipmentMethodId";
                cbl_ShipmentMethod.DataBind();
                foreach (ListItem item in cbl_ShipmentMethod.Items)
                    item.Selected = true;

                cbl_PaymentTerm.DataSource =  WebUtil.getPaymentTermList();
                cbl_PaymentTerm.DataTextField = "PaymentTermDescription";
                cbl_PaymentTerm.DataValueField = "PaymentTermId";
                cbl_PaymentTerm.DataBind();
                foreach (ListItem item in cbl_PaymentTerm.Items)
                    item.Selected = true;
            }
        }

        protected void btn_Preview_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "OutstandingGBTestResultReport");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;


            ReportClass rpt =  genReport();
            CrystalDecisions.Shared.ExportOptions eOption = new CrystalDecisions.Shared.ExportOptions();
            eOption.ExportFormatType = CrystalDecisions.Shared.ExportFormatType.ExcelRecord;

            CrystalDecisions.Shared.ExcelDataOnlyFormatOptions formatOption = CrystalDecisions.Shared.ExportOptions.CreateDataOnlyExcelFormatOptions();
            formatOption.ExcelConstantColumnWidth = 5;
            formatOption.ExcelUseConstantColumnWidth = true;
            formatOption.ExportObjectFormatting = true;
            formatOption.MaintainColumnAlignment = true;

            eOption.ExportFormatOptions = formatOption;


            rpt.ExportToHttpResponse(eOption, HttpContext.Current.Response, true, "OutstandingGBTestResultReport");

            //ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "OutstandingGBTestResultReport");
        }

        ReportClass genReport()
        {
            //int officeGroupId = Convert.ToInt32(ddl_OfficeGroup.SelectedValue);
            TypeCollector officeIdList = TypeCollector.Inclusive;
            DateTime custAtWHDateFrom = DateTime.MinValue;
            DateTime custAtWHDateTo = DateTime.MinValue;
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            TypeCollector shipmentMethodIdList = TypeCollector.Inclusive;
            TypeCollector paymentTermIdList = TypeCollector.Inclusive;
            int vendorId = -1;
            int paymentStatus = -1;

            if (txt_Supplier.VendorId != int.MinValue)
            {
                vendorId = txt_Supplier.VendorId;
            }

            
            int officeGroupId = Convert.ToInt32(ddl_Office.SelectedValue);
            ArrayList officeList = null;            
            if (officeGroupId == -1)
                officeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            else
                officeList = CommonManager.Instance.getOfficeListByReportOfficeGroupId(Convert.ToInt32(officeGroupId));
            foreach (OfficeRef office in officeList)
            {
                officeIdList.append(office.OfficeId);
            }

            if (txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            if (txt_CustAWHDateFrom.Text.Trim() != "")
            {
                custAtWHDateFrom = DateTimeUtility.getDate(txt_CustAWHDateFrom.Text.Trim());
                custAtWHDateTo = DateTimeUtility.getDate(txt_CustAWHDateTo.Text.Trim());
            }

            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                if (item.Selected)
                    shipmentMethodIdList.append(int.Parse(item.Value));
            }

            foreach (ListItem item in cbl_PaymentTerm.Items)
            {
                if (item.Selected)
                    paymentTermIdList.append(int.Parse(item.Value));
            }

            if (cbl_PaymentStatus.Items[0].Selected)
                paymentStatus = 1;
            if (cbl_PaymentStatus.Items[1].Selected)
            {
                if (paymentStatus == 1)
                    paymentStatus = -1;
                else
                    paymentStatus = 0;
            }
            return ShippingReportManager.Instance.getOutstandingGBTestReport(officeIdList, invoiceDateFrom, invoiceDateTo, custAtWHDateFrom, custAtWHDateTo, vendorId,
                 shipmentMethodIdList, paymentTermIdList, paymentStatus, this.LogonUserId);
        }
 
    }
}