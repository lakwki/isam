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
using com.next.isam.webapp.commander;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.helper;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class SupplierOrderStatusReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());                                    

            uclProductTeam.setWidth(305);
            uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);
            uclSupplier.setWidth(305);
            uclSupplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            ddl_PaymentTerm.bindList(WebUtil.getPaymentTermList(), "PaymentTermDescription", "PaymentTermId", "", "--All--", GeneralCriteria.ALL.ToString());
            
        }

        protected ReportClass genReport()
        {
            DateTime CustomerAtWHDateFrom = DateTime.MinValue;
            DateTime CustomerAtWHDateTo = DateTime.MinValue;
            int vendorId = -1;
            int paymentTermId = Convert.ToInt32(ddl_PaymentTerm.SelectedValue);

            if (txt_AtWHDateFrom.Text.Trim() != "")
            {
                CustomerAtWHDateFrom = DateTimeUtility.getDate(txt_AtWHDateFrom.Text.Trim());
                CustomerAtWHDateTo = DateTimeUtility.getDate(txt_AtWHDateTo.Text.Trim());
            }

            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (ddl_Office.SelectedValue == "-1")
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

                foreach (OfficeRef oRef in userOfficeList)
                {
                    officeIdList.append(oRef.OfficeId);
                }
            }
            else
            {
                officeIdList.append(Convert.ToInt32(ddl_Office.SelectedValue));
            }
            
            TypeCollector productTeamList = TypeCollector.Inclusive;
            if (uclProductTeam.ProductCodeId != int.MinValue)
            {
                productTeamList.append(uclProductTeam.ProductCodeId);
            }
            else
            {
                foreach (int officeId in officeIdList.Values)
                {
                    ArrayList codeList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, officeId, this.LogonUserId, GeneralCriteria.ALLSTRING);

                    foreach (OfficeStructureRef osRef in codeList)
                    {
                           productTeamList.append(osRef.OfficeStructureId);
                    }
                }
            }

            if (uclSupplier.VendorId != int.MinValue)
                vendorId = uclSupplier.VendorId;

            return AccountReportManager.Instance.getSupplierOrderStatusReport(CustomerAtWHDateFrom, CustomerAtWHDateTo, officeIdList, productTeamList, vendorId, paymentTermId, this.LogonUserId);
        }


        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "SupplierOrderStatusReport");

        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "SupplierOrderStatusReport");

        }
    }
}
