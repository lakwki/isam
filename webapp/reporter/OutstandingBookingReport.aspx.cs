using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;

using com.next.isam.webapp.commander;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.shipping;
using com.next.isam.appserver.common;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class OutstandingBookingReport  : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControl();
            }
        }

        protected void initControl()
        {
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_PackingMethod.bindList(WebUtil.getPackingMethodList(), "PackingMethodDescription", "PackingMethodId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_CO.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_TermOfPurchase.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_ShipmentPort.bindList(WebUtil.getShipmentPortList(), "ShipmentPortDescription", "ShipmentPortId", "", "--All--", GeneralCriteria.ALL.ToString());

            txt_Supplier.setWidth(305);
            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);


            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();

            cbl_ShipmentMethod.DataSource = WebUtil.getShipmentMethodList();
            cbl_ShipmentMethod.DataTextField = "ShipmentMethodDescription";
            cbl_ShipmentMethod.DataValueField = "ShipmentMethodId";
            cbl_ShipmentMethod.DataBind();


            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                    item.Selected = true;
            }

            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                item.Selected = true;
            }

        }



        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "OutstandingBooking");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "OutstandingBooking");
        }

        protected ReportClass genReport()
        {
            DateTime POAtWHDateFrom = DateTime.MinValue;
            DateTime POAtWHDateTo = DateTime.MinValue;
            int officeId = Convert.ToInt32(ddl_Office.SelectedValue);
            int vendorId = -1;
            int countryOfOriginId = Convert.ToInt32(ddl_CO.SelectedValue);
            int packingMethodId = Convert.ToInt32(ddl_PackingMethod.SelectedValue);
            int termOfPurchaseId = Convert.ToInt32(ddl_TermOfPurchase.SelectedValue);
            int shipmentPortId = Convert.ToInt32(ddl_ShipmentPort.SelectedValue);
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            string sampleOrderType = ddl_SampleOrder.SelectedItem.Text;
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);

            if (txt_POAtWHDateFrom.Text.Trim() != "")
            {
                POAtWHDateFrom = DateTimeUtility.getDate(txt_POAtWHDateFrom.Text.Trim());
                if (txt_POAtWHDateTo.Text.Trim() == "")
                    txt_POAtWHDateTo.Text = txt_POAtWHDateFrom.Text;
                POAtWHDateTo = DateTimeUtility.getDate(txt_POAtWHDateTo.Text.Trim());
            }

            if (txt_Supplier.VendorId != int.MinValue)
            {
                vendorId = txt_Supplier.VendorId;
            }

            TypeCollector productTeamList = TypeCollector.Inclusive;
            int officeGroupId = Convert.ToInt32(ddl_Office.SelectedValue);
            ArrayList officeList = null;
            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (officeGroupId == -1)
                officeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            else
                officeList = CommonManager.Instance.getOfficeListByReportOfficeGroupId(Convert.ToInt32(officeGroupId));
            string officeName = string.Empty;
            foreach (OfficeRef office in officeList)
            {
                officeIdList.append(office.OfficeId);
                officeName += (officeName == string.Empty ? "" : ", ") + office.Description.Replace(" Office", "");
                ArrayList pt = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, office.OfficeId, this.LogonUserId, GeneralCriteria.ALLSTRING);
                foreach (OfficeStructureRef os in pt)
                    productTeamList.append(os.OfficeStructureId);
            }
            officeName = (officeGroupId == -1 ? "ALL" : officeName);

            if (!officeIdList.contains(OfficeId.DG.Id))
                handlingOfficeId = -1;

            TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (item.Selected)
                    customerTypeCollector.append(Convert.ToInt32(item.Value));
            }
            TypeCollector shipmentMethodCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                if (item.Selected)
                    shipmentMethodCollector.append(Convert.ToInt32(item.Value));
            }

            return ShippingReportManager.Instance.getOutstandingBookingReport(officeName, officeIdList, productTeamList, handlingOfficeId, countryOfOriginId, shipmentMethodCollector,
                packingMethodId, POAtWHDateFrom, POAtWHDateTo, customerTypeCollector, vendorId, termOfPurchaseId, shipmentPortId, isSampleOrder, this.LogonUserId);
        }
    }
}
