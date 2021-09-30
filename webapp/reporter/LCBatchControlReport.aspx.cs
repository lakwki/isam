using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.webapp.commander;
using com.next.isam.domain.types;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.lcreport;
using com.next.isam.reporter.common;
using com.next.isam.reporter;

using com.next.isam.dataserver.worker;
using com.next.isam.appserver.common;
using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;


namespace com.next.isam.webapp.reporter
{
    public partial class LCBatchControlReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
       protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                initControl();
            }

        }

        protected void btn_Preview_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "LC Batch Control Report");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "LC Batch Control Report");
        }


        protected void ddl_OfficeGroup_SelectedIndexChanged(object sender, EventArgs e)
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

        void initControl()
        {
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

            //ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            this.ddl_OfficeGroup.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());
            this.ddl_HandlingOffice.bindList(CommonManager.Instance.getDGHandlingOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());

            ddl_CO.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());

            uclVendor.setWidth(305);
            uclVendor.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

        }

        private ReportClass genReport()
        {
            int i;

            
            int lcBatchNoFrom, lcBatchNoTo;
            int lcApplicationNoFrom = -1;
            int lcApplicationNoTo = -1;
            string lcNoFrom = "";
            string lcNoTo = "";
            DateTime lcAppliedDateFrom = DateTime.MinValue;
            DateTime lcAppliedDateTo = DateTime.MinValue;
            DateTime lcIssueDateFrom = DateTime.MinValue;
            DateTime lcIssueDateTo = DateTime.MinValue;
            DateTime lcExpiryDateFrom = DateTime.MinValue;
            DateTime lcExpiryDateTo = DateTime.MinValue;
            DateTime lcApplicationDateFrom = DateTime.MinValue;
            DateTime lcApplicationDateTo = DateTime.MinValue;


            if (!(int.TryParse(txt_LCBatchNoFrom.Text.ToUpper().Replace("LCB", ""), out lcBatchNoFrom) 
                && int.TryParse(txt_LCBatchNoTo.Text.ToUpper().Replace("LCB", ""), out lcBatchNoTo)))
            {
                lcBatchNoFrom = int.MinValue;
                lcBatchNoTo = int.MinValue;
            }
            
            if (!(DateTime.TryParse(txt_LCAppliedDateFrom.Text, out lcAppliedDateFrom) && DateTime.TryParse(txt_LCAppliedDateTo.Text,out lcAppliedDateTo)))
            {
                lcAppliedDateFrom = DateTime.MinValue;
                lcAppliedDateTo = DateTime.MinValue;
            }
            if (!(DateTime.TryParse(txt_LCIssueDateFrom.Text, out lcIssueDateFrom) && DateTime.TryParse(txt_LCIssueDateTo.Text,out lcIssueDateTo)))
            {
                lcIssueDateFrom = DateTime.MinValue;
                lcIssueDateTo = DateTime.MinValue;
            }
            if (!(DateTime.TryParse(txt_LCExpiryDateFrom.Text, out lcExpiryDateFrom) && DateTime.TryParse(txt_LCExpiryDateTo.Text,out lcExpiryDateTo)))
            {
                lcExpiryDateFrom = DateTime.MinValue;
                lcExpiryDateTo = DateTime.MinValue;
            }
            if (!(DateTime.TryParse(txt_LCApplicationDateFrom.Text, out lcApplicationDateFrom) && DateTime.TryParse(txt_LCApplicationDateTo.Text, out lcApplicationDateTo)))
            {
                lcApplicationDateFrom = DateTime.MinValue;
                lcApplicationDateTo = DateTime.MinValue;
            }
            if (txt_LCAppNoFrom.Text.Trim() != "")
            {
                lcApplicationNoFrom = Convert.ToInt32(txt_LCAppNoFrom.Text);
                lcApplicationNoTo = Convert.ToInt32(txt_LCAppNoTo.Text);
            }
            if (txt_LCNoFrom.Text.Trim() != "")
            {
                lcNoFrom = txt_LCNoFrom.Text.Trim();
                lcNoTo = txt_LCNoTo.Text.Trim();
            }

            //ArrayList coIdList = new ArrayList();
            //if (ddl_CO.SelectedValue!="-1")
            //    coIdList.Add(Convert.ToInt32(ddl_CO.SelectedValue));
            //else
            //    for (i=1; i<ddl_CO.Items.Count; i++)
            //        coIdList.Add (Convert.ToInt32(ddl_CO.Items[i].Value));
            int coId;
            string coName="";
            if (ddl_CO.SelectedValue == "-1")
            {
                coId = int.MinValue;
                coName = "";
            }
            else
            {
                coId = int.Parse(ddl_CO.SelectedValue);
                coName = ddl_CO.selectedText;
            }

            /*
            ArrayList officeIdList = new ArrayList();
            string OfficeName="";
            if (ddl_Office.SelectedValue != "-1")
            {
                officeIdList.Add(Convert.ToInt32(ddl_Office.SelectedValue));
                OfficeName = ddl_Office.selectedText;
            }
            else
            {
                for (i = 1; i < ddl_Office.Items.Count; i++)
                    officeIdList.Add(Convert.ToInt32(ddl_Office.Items[i].Value));
                OfficeName = "";
            }
            */
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

            int vendorId;
            if (uclVendor.VendorId <= 0)
                vendorId=int.MinValue;
            else
                vendorId = uclVendor.VendorId;
               
   
            LCBatchControlRpt rpt = LCReportManager.Instance.getLCBatchControlReport(lcBatchNoFrom, lcBatchNoTo, lcAppliedDateFrom, lcAppliedDateTo, 
            lcIssueDateFrom, lcIssueDateTo, lcExpiryDateFrom, lcExpiryDateTo, coId, officeIdList, handlingOfficeId, vendorId  , this.LogonUserId, lcApplicationNoFrom, lcApplicationNoTo,
            lcApplicationDateFrom, lcApplicationDateTo, lcNoFrom, lcNoTo, coName, officeName, handlingOfficeName);


            return rpt;
        }

    }
}
