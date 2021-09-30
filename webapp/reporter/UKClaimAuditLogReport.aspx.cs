using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.common.appserver;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.helper;
using com.next.isam.domain.claim;
using com.next.infra.util;
using com.next.isam.appserver.claim;

namespace com.next.isam.webapp.reporter
{
    public partial class UKClaimAuditLogReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControls();
            }
        }

        void initControls()
        {
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            this.ddl_Office.bindList(userOfficeList, "Description", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());                                    

            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            uclSupplier.setWidth(305);
            uclSupplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            ddl_ClaimType.bindList(UKClaimType.getCollectionValues(), "Name", "Id", "", "--All--", GeneralCriteria.ALL.ToString());
        }

        protected ReportClass genReport()
        {
            DateTime issueDateFrom = DateTime.MinValue;
            DateTime issueDateTo = DateTime.MinValue;
            int vendorId = -1;

            if (txt_IssueDateFrom.Text.Trim() != String.Empty)
            {
                issueDateFrom = DateTimeUtility.getDate(txt_IssueDateFrom.Text.Trim());
                issueDateTo = DateTimeUtility.getDate(txt_IssueDateTo.Text.Trim());
            }

            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (int.Parse(ddl_Office.SelectedValue) != -1)
                officeIdList.append(Convert.ToInt32(ddl_Office.SelectedValue));
            else
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef oRef in userOfficeList)
                    officeIdList.append(oRef.OfficeId);
            }
            
            int productTeamId = -1;
            if (uclProductTeam.ProductCodeId != int.MinValue)
                productTeamId = uclProductTeam.ProductCodeId;

            if (uclSupplier.VendorId != int.MinValue)
                vendorId = uclSupplier.VendorId;

            int claimTypeId = -1;
            if (int.Parse(ddl_ClaimType.SelectedValue) != -1)
                claimTypeId = int.Parse(ddl_ClaimType.SelectedValue);

            string officeDesc = (ddl_Office.SelectedIndex == 0 ? "ALL" : ddl_Office.selectedText);
            string productTeamDesc = (uclProductTeam.KeyTextBox.Text == "" ? "ALL" : CommonUtil.getProductCodeDefByKey(int.Parse(uclProductTeam.KeyTextBox.Text)).CodeDescription);
            string vendorDesc = (uclSupplier.KeyTextBox.Text == "" ? "ALL" : IndustryManager.Instance.getVenderRefByKey(Convert.ToInt32(uclSupplier.KeyTextBox.Text)).Name);
            string claimTypeDesc = (ddl_ClaimType.SelectedIndex == 0 ? "ALL" : ddl_ClaimType.selectedText);
            
            return AccountReportManager.Instance.getUKClaimAuditLogReport(issueDateFrom, issueDateTo, officeIdList, officeDesc, productTeamId, productTeamDesc, 
                vendorId, vendorDesc, claimTypeId, claimTypeDesc, this.LogonUserId, string.Empty);
        }

        protected void btn_Preview_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "UKClaimAuditLogReport");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.Excel, "UKClaimAuditLogReport");

        }
    }
}
