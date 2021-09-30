using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using CrystalDecisions.CrystalReports.Engine;

using com.next.isam.reporter.helper;
using com.next.isam.reporter.lcreport;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.appserver.common;
using com.next.common.domain.module;


namespace com.next.isam.webapp.reporter
{
    public partial class LCShipmentAmendmentReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private bool HasAccessRightsTo_SuperView;
        protected void Page_Load(object sender, EventArgs e)
        {
            HasAccessRightsTo_SuperView = false;

            if (!Page.IsPostBack)
            {
                initControl();
            }
        }


        protected void btn_Preview_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "LC Shipment Amendment Report");
        }


        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "LC Shipment Amendment Report");
        }


        void initControl()
        {
            int nUserId;

            nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(nUserId, OfficeStructureType.PRODUCTCODE.Type);
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            this.ddl_OfficeGroup.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());
            this.ddl_HandlingOffice.bindList(CommonManager.Instance.getDGHandlingOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());
        }

        private ReportClass genReport()
        {
            string lcNoFrom = string.Empty;
            string lcNoTo = string.Empty;
            int lcBatchNoFrom;
            int lcBatchNoTo;
            int lcApplicationNoFrom ;
            int lcApplicationNoTo;
            DateTime lcIssueDateFrom = DateTime.MinValue;
            DateTime lcIssueDateTo = DateTime.MinValue;
            DateTime supplierAwhDateFrom = DateTime.MinValue;
            DateTime supplierAwhDateTo=DateTime.MinValue;

            lcNoFrom = txt_LCNoFrom.Text;
            lcNoTo = txt_LCNoTo.Text;

            if (!(int.TryParse(txt_LCBatchNoFrom.Text.ToUpper().Replace("LCB", ""), out lcBatchNoFrom)
                && int.TryParse(txt_LCBatchNoTo.Text.ToUpper().Replace("LCB", ""), out lcBatchNoTo)))
            {
                lcBatchNoFrom = int.MinValue;
                lcBatchNoTo = int.MinValue;
            }

            if (!(int.TryParse(txt_LCApplicationNoFrom.Text.ToUpper().Replace("LCB", ""), out lcApplicationNoFrom)
                && int.TryParse(txt_LCApplicationNoTo.Text.ToUpper().Replace("LCB", ""), out lcApplicationNoTo)))
            {
                lcApplicationNoFrom = int.MinValue;
                lcApplicationNoTo = int.MinValue;
            }

            if (!(DateTime.TryParse(txt_LCIssueDateFrom.Text, out lcIssueDateFrom) && DateTime.TryParse(txt_LCIssueDateTo.Text, out lcIssueDateTo)))
            {
                lcIssueDateFrom = DateTime.MinValue;
                lcIssueDateTo = DateTime.MinValue;
            }

            if (!(DateTime.TryParse(txt_SupplierAwhDateFrom.Text, out supplierAwhDateFrom) && DateTime.TryParse(txt_SupplierAwhDateTo.Text, out supplierAwhDateTo)))
            {
                supplierAwhDateFrom = DateTime.MinValue;
                supplierAwhDateTo = DateTime.MinValue;
            }

            int officeGroupId = Convert.ToInt32(ddl_OfficeGroup.SelectedValue);
            ArrayList officeList = null;
             if (officeGroupId == -1)
                officeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            else
                officeList = CommonManager.Instance.getOfficeListByReportOfficeGroupId(Convert.ToInt32(officeGroupId));
            string officeName = string.Empty;
           TypeCollector officeIdList = TypeCollector.Inclusive;
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
            

            LCShipmentAmendmentRpt rpt = LCReportManager.Instance.getLCShipmentAmendmentReport(lcNoFrom, lcNoTo, lcBatchNoFrom, lcBatchNoTo,
                    lcApplicationNoFrom, lcApplicationNoTo, lcIssueDateFrom, lcIssueDateTo, supplierAwhDateFrom, supplierAwhDateTo, 
                    officeIdList, officeName, handlingOfficeId, handlingOfficeName, this.LogonUserId);
            
            return rpt; 
        }

        protected void ddl_OfficeGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nOfficeId, nOfficeGroupId;
            int nUserId;

            nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);
            nOfficeGroupId = int.Parse(this.ddl_OfficeGroup.SelectedValue);
            if (nOfficeGroupId == -1) nOfficeGroupId = GeneralCriteria.ALL;

            nOfficeId = nOfficeGroupId;
            if (CommonManager.Instance.getDGOfficeGroupIdList().Contains(Convert.ToInt32(ddl_OfficeGroup.SelectedItem.Value)))
            {
                tr_HandlingOffice.Style.Add("display", "block");
            }
            else
            {
                tr_HandlingOffice.Style.Add("display", "none");
                ddl_HandlingOffice.SelectedIndex = 0;
            }

            return;
        }

    }
}