using System;
using System.Collections;
using System.Web;
using System.Web.UI;

using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.lcreport;

using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.appserver.common;
using System.Web.UI.WebControls;

namespace com.next.isam.webapp.reporter
{
    
    public partial class LCStatusReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "LC Status Report");
        }


        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response, 
                CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "LC Status Report");
        }


        void initControl()
        {
            int nUserId;

            //HasAccessRights = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcApplication.Id, ISAMModule.lcApplication.SuperView);

            //********** For Testing **************************
            if (this.LogonUserId==574)
              HasAccessRightsTo_SuperView = true;
            //********** For Testing **************************
            //nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);
            //HasAccessRightsTo_SuperView = tue;
            nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);

            ddl_GBTestResult.Items.Add(new ListItem("--All--", "-1"));
            ddl_GBTestResult.Items.Add(new ListItem("Pass", "1"));
            //ddl_GBTestResult.Items.Add(new ListItem("Fail", "0"));
            ddl_GBTestResult.Items.Add(new ListItem("Fail (Can Release Payment)", "2"));
            ddl_GBTestResult.Items.Add(new ListItem("Fail (Hold Payment)", "0"));
            ddl_GBTestResult.Items.Add(new ListItem("Fail (Cannot Release Payment)", "9"));
            ddl_GBTestResult.Items.Add(new ListItem("Nil", "-999"));
            ddl_GBTestResult.selectByValue("-1");

            ddl_CO.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());

            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(nUserId, OfficeStructureType.PRODUCTCODE.Type);
            //ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            this.ddl_OfficeGroup.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());
            this.ddl_HandlingOffice.bindList(CommonManager.Instance.getDGHandlingOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());

            ArrayList ProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALL, nUserId, GeneralCriteria.ALLSTRING);
            ddl_ProductTeam.bindList(ProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_ProductTeam_Refresh();

            uclVendor.setWidth(305);
            uclVendor.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

        }

        private ReportClass genReport()
        {
            int i;


            int lcBatchNoFrom, lcBatchNoTo;
            string lcNoFrom, lcNoTo;
            string lcBillRefNoFrom, lcBillRefNoTo;
            DateTime lcBatchDateFrom = DateTime.MinValue;
            DateTime lcBatchDateTo = DateTime.MinValue;
            DateTime lcIssueDateFrom = DateTime.MinValue;
            DateTime lcIssueDateTo = DateTime.MinValue;
            DateTime lcExpiryDateFrom = DateTime.MinValue;
            DateTime lcExpiryDateTo = DateTime.MinValue;
            DateTime lcPaymentCheckDateFrom = DateTime.MinValue;
            DateTime lcPaymentCheckDateTo = DateTime.MinValue;



            if (!(int.TryParse(txt_LCBatchNoFrom.Text.ToUpper().Replace("LCB", ""), out lcBatchNoFrom)
                && int.TryParse(txt_LCBatchNoTo.Text.ToUpper().Replace("LCB", ""), out lcBatchNoTo)))
            {
                lcBatchNoFrom = int.MinValue;
                lcBatchNoTo = int.MinValue;
            }

            lcNoFrom = txt_LCNoFrom.Text;
            lcNoTo = txt_LCNoTo.Text;
            lcBillRefNoFrom = txt_LCBillRefNoFrom.Text;
            lcBillRefNoTo = txt_LCBillRefNoTo.Text;

            if (!(DateTime.TryParse(txt_LCIssueDateFrom.Text, out lcIssueDateFrom) && DateTime.TryParse(txt_LCIssueDateTo.Text, out lcIssueDateTo)))
            {
                lcIssueDateFrom = DateTime.MinValue;
                lcIssueDateTo = DateTime.MinValue;
            }
            if (!(DateTime.TryParse(txt_LCExpiryDateFrom.Text, out lcExpiryDateFrom) && DateTime.TryParse(txt_LCExpiryDateTo.Text, out lcExpiryDateTo)))
            {
                lcExpiryDateFrom = DateTime.MinValue;
                lcExpiryDateTo = DateTime.MinValue;
            }
            if (!(DateTime.TryParse(txt_LCPaymentCheckDateFrom.Text, out lcPaymentCheckDateFrom) && DateTime.TryParse(txt_LCPaymentCheckDateTo.Text, out lcPaymentCheckDateTo)))
            {
                lcPaymentCheckDateFrom = DateTime.MinValue;
                lcPaymentCheckDateTo = DateTime.MinValue;
            }


            int coId;
            string coName = "";
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
            string officeName = "";
            if (ddl_Office.SelectedValue != "-1")
            {
                officeIdList.Add(Convert.ToInt32(ddl_Office.SelectedValue));
                officeName = ddl_Office.selectedText;
            }
            else
            {
                for (i = 1; i < ddl_Office.Items.Count; i++)
                    officeIdList.Add(Convert.ToInt32(ddl_Office.Items[i].Value));
                officeName = ""; 
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


            ArrayList productTeamIdList = new ArrayList();
            string productTeamName = "";
            if (ddl_ProductTeam.SelectedValue != "-1")
            {
                productTeamIdList.Add(Convert.ToInt32(ddl_ProductTeam.SelectedValue));
                productTeamName = ddl_ProductTeam.selectedText ;
            }
            else
            {
                for (i = 1; i < ddl_ProductTeam.Items.Count; i++)
                    productTeamIdList.Add(Convert.ToInt32(ddl_ProductTeam.Items[i].Value));
                productTeamName = "";
            }


            int vendorId;
            if (uclVendor.VendorId <= 0)
            {
                vendorId = int.MinValue;
            }
            else
            {
                vendorId = uclVendor.VendorId;
            }

            LCStatusRpt rpt = LCReportManager.Instance.getLCStatusReport(vendorId, lcNoFrom, lcNoTo, lcBillRefNoFrom, lcBillRefNoTo, lcBatchNoFrom, lcBatchNoTo, lcIssueDateFrom,
                                    lcIssueDateTo, lcExpiryDateFrom, lcExpiryDateTo, lcPaymentCheckDateFrom, lcPaymentCheckDateTo, coId, officeIdList, handlingOfficeId, 
                                    productTeamIdList, coName, officeName, handlingOfficeName, productTeamName, ddl_GBTestResult.selectedText.Replace("-",""), this.LogonUserId);
            return rpt;
        }


        protected void ddl_ProductTeam_Refresh()
        {
            if ((this.ddl_OfficeGroup.SelectedIndex == 0 && this.ddl_OfficeGroup.Items.Count > 2) || this.ddl_OfficeGroup.selectedText.Contains("+"))
            {
                this.ddl_ProductTeam.SelectedIndex = 0;
                this.ddl_ProductTeam.Enabled = false;
            }
            else
            {
                this.ddl_ProductTeam.Enabled = true;
            }

        }


        protected void ddl_OfficeGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList ProductTeamList;
            int nOfficeId, nOfficeGroupId;
            int nUserId;

            nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);
            nOfficeGroupId = int.Parse(this.ddl_OfficeGroup.SelectedValue);
            if (nOfficeGroupId == -1) nOfficeGroupId = GeneralCriteria.ALL;

            nOfficeId = nOfficeGroupId; // not allow to get product team if select any office group (more than one office)
            ProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, nOfficeId, nUserId, GeneralCriteria.ALLSTRING);
            this.ddl_ProductTeam.bindList(ProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_ProductTeam_Refresh();

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
