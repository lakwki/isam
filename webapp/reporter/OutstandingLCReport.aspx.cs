using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.appserver.common;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.lcreport;
using com.next.isam.reporter.helper;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class OutstandingLCReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            //this.ddl_OfficeGroup.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            this.ddl_OfficeGroup.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());
            this.ddl_HandlingOffice.bindList(CommonManager.Instance.getDGHandlingOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());
            
            ddl_CO.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_PurchaseTerm.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_Destination.bindList(WebUtil.getCustomerDestinationList(), "DestinationDesc", "CustomerDestinationId", "", "--All--", GeneralCriteria.ALL.ToString());

            txt_Supplier.setWidth(305);
            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);                                  
        }

        ReportClass genReport()
        {
            //int officeGroupId = Convert.ToInt32(ddl_OfficeGroup.SelectedValue);
            int countryOfOriginId = Convert.ToInt32(ddl_CO.SelectedValue);
            int termOfPurchaseId = Convert.ToInt32(ddl_PurchaseTerm.SelectedValue);
            int customerDestinationId = Convert.ToInt32(ddl_Destination.SelectedValue);
            int vendorId = -1;
            DateTime supplierAtWHDateFrom = DateTime.MinValue ;
            DateTime supplierAtWHDateTo = DateTime.MinValue ;

            if (txt_Supplier.VendorId != int.MinValue)
            {
                vendorId = txt_Supplier.VendorId;
            }

            TypeCollector productTeamList = TypeCollector.Inclusive;
            if (uclProductTeam.ProductCodeId != int.MinValue)
                productTeamList.append(uclProductTeam.ProductCodeId);

            int officeGroupId = Convert.ToInt32(ddl_OfficeGroup.SelectedValue);
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
            }
            officeName = (officeGroupId == -1 ? "ALL" : officeName);
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);
            string handlingOfficeName = string.Empty;
            if (CommonManager.Instance.getDGOfficeGroupIdList().Contains(Convert.ToInt32(ddl_OfficeGroup.SelectedItem.Value)))
                handlingOfficeName = (handlingOfficeId == -1 ? "ALL" : CommonManager.Instance.getDGHandlingOffice(handlingOfficeId).Description);

            if (txt_CustomerAtWHDateFrom.Text.Trim() != "")
            {
                supplierAtWHDateFrom = DateTimeUtility.getDate(txt_CustomerAtWHDateFrom.Text.Trim());
                supplierAtWHDateTo = DateTimeUtility.getDate(txt_CustomerAtWHDateTo.Text.Trim());
            }

            return LCReportManager.Instance.getOutstandingLCReport(officeName, officeIdList, handlingOfficeName,handlingOfficeId, productTeamList, vendorId, countryOfOriginId, customerDestinationId, termOfPurchaseId,
                supplierAtWHDateFrom, supplierAtWHDateTo, this.LogonUserId, rad_SortField.SelectedValue, rad_SortOrder.SelectedValue);
        }

        protected void btn_Submit_Click(object sender, EventArgs arg)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "OutstandingLC");
        }

        protected void btn_Export_Click(object sender, EventArgs arg)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "OutstandingLC");
        }


        protected void ddl_OfficeGroup_SelectedIndexChanged(object sender, EventArgs arg)
        {
            if (CommonManager.Instance.getDGOfficeGroupIdList().Contains(Convert.ToInt32(ddl_OfficeGroup.SelectedItem.Value)))
            {
                tr_HandlingOffice.Style.Add("display", "block");
            }
            else
            {
                tr_HandlingOffice.Style.Add("display", "none");
                ddl_HandlingOffice.SelectedIndex = 0;
            }
        }
    }
}
