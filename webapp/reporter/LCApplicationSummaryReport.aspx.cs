using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

using CrystalDecisions.CrystalReports.Engine;

using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.lcreport;
using com.next.isam.reporter.helper;
using com.next.isam.appserver.common;


namespace com.next.isam.webapp.reporter
{
    
    public partial class LCApplicationSummaryReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                //ddl_Office.bindList(userOfficeList, "Description", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
                this.ddl_OfficeGroup.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "-- All --", GeneralCriteria.ALL.ToString());
                this.ddl_HandlingOffice.bindList(CommonManager.Instance.getDGHandlingOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());

                ddl_LCDetail.Items.Add(new ListItem("--All--", "-1"));
                ddl_LCDetail.Items.Add(new ListItem("Updated", "1"));
                ddl_LCDetail.Items.Add(new ListItem("Not Updated", "0"));

                cbl_Customer.DataSource = WebUtil.getCustomerList();
                cbl_Customer.DataTextField = "CustomerCode";
                cbl_Customer.DataValueField = "CustomerId";
                cbl_Customer.DataBind();
                foreach (ListItem item in cbl_Customer.Items)
                {
                    if(WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                        item.Selected = true;
                }
                txt_Supplier.setWidth(305);
                txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
            }

        }


        protected void btn_Preview_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "LC Application Summary Report");
        }


        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "LC Application Summary Report");
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

        private ReportClass genReport()
        {
            DateTime LCAppDateFrom = DateTime.MinValue;
            DateTime LCAppDateTo = DateTime.MinValue;
            int LCAppNoFrom = int.MinValue;
            int LCAppNoTo = int.MinValue;
            string LCNoFrom = string.Empty;
            string LCNoTo = string.Empty;


            ArrayList customerIdList = new ArrayList();
            string customerName = string.Empty;
            foreach (ListItem item in cbl_Customer.Items)
                if (item.Selected)
                {
                    customerName += (customerName == string.Empty ? "" : ", ") + item.Text;
                    customerIdList.Add(Convert.ToInt32(item.Value));
                }


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
            string handlingOfficeName=string.Empty;
            if (CommonManager.Instance.getDGOfficeGroupIdList().Contains(Convert.ToInt32(ddl_OfficeGroup.SelectedItem.Value)))
                handlingOfficeName = (handlingOfficeId == -1 ? "ALL" : CommonManager.Instance.getDGHandlingOffice(handlingOfficeId).Description);

            int vendorId = int.MinValue;
            string vendorName = "ALL";
            if (txt_Supplier.VendorId != int.MinValue)
            {
                //vendorName = txt_Supplier.ToString();
                vendorName = IndustryUtil.getVendorByKey(txt_Supplier.VendorId).Name;
                vendorId = txt_Supplier.VendorId;
            }

            if (txt_LCAppDateFrom.Text.Trim() != "" || txt_LCAppDateTo.Text.Trim() != "")
            {
                LCAppDateFrom  = DateTime.Parse(txt_LCAppDateFrom.Text);
                LCAppDateTo = DateTime.Parse(txt_LCAppDateTo.Text);

                DateTime.TryParse(txt_LCAppDateFrom.Text, out LCAppDateFrom);
                if (LCAppDateFrom == null)
                    LCAppDateFrom = DateTime.MinValue;

                DateTime.TryParse(txt_LCAppDateTo.Text, out LCAppDateTo);
                if (LCAppDateTo == null)
                    LCAppDateTo = DateTime.MinValue;
            }

            if (txt_LCAppNoFrom.Text == "")
                LCAppNoFrom = int.MinValue;
            else
                LCAppNoFrom = Int32.Parse(txt_LCAppNoFrom.Text);
            if (txt_LCAppNoTo.Text == "")
                LCAppNoTo = int.MinValue;
            else
                LCAppNoTo = Int32.Parse(txt_LCAppNoTo.Text);

            if (txt_LCNoFrom.Text == "")
                LCNoFrom = string.Empty;
            else
                LCNoFrom = txt_LCNoFrom.Text;
            if (txt_LCNoTo.Text == "")
                LCNoTo = string.Empty;
            else
                LCNoTo = txt_LCNoTo.Text;
            
            return  LCReportManager.Instance.getLCApplicationSummaryReport(
                LCAppDateFrom, LCAppDateTo, LCAppNoFrom, LCAppNoTo, LCNoFrom, LCNoTo,
                customerName, customerIdList, officeName, officeIdList, handlingOfficeName, handlingOfficeId, vendorName, vendorId, ddl_LCDetail.selectedText.Replace("-",""),
                this.LogonUserId);
        }

    }
}
