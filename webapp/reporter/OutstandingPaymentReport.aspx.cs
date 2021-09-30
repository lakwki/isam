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
using com.next.isam.reporter.shipping;
using com.next.isam.reporter.helper;
using com.next.isam.appserver.common;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class OutstandingPaymentReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
            this.ddl_OfficeGroup.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_PurchaseTerm.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            ddl_PaymentTerm.bindList(WebUtil.getPaymentTermList(), "PaymentTermDescription", "PaymentTermId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            

            TypeCollector officeType = TypeCollector.Inclusive;
            foreach (OfficeRef def in userOfficeList)
            {
                officeType.append(def.OfficeId);
            }

            TypeCollector jobNatureType = TypeCollector.Inclusive;
            jobNatureType.append(JobNatureId.SHIPPING.Id);
            
            ArrayList shippingUserList = CommonUtil.getUserSelectionListByOfficeJobNature(officeType, jobNatureType, "A");
            shippingUserList.Add(CommonUtil.getUserInfoByKey(99999));
            cbl_ShippingUser.DataSource = shippingUserList;
            cbl_ShippingUser.DataTextField = "DisplayName";
            cbl_ShippingUser.DataValueField = "UserId";
            cbl_ShippingUser.DataBind();

            foreach (ListItem item in cbl_ShippingUser.Items)
            {
                item.Selected = true;
            }

            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);            
        }

        ReportClass genReport()
        {
            int termOfPurchaseId = Convert.ToInt32(ddl_PurchaseTerm.SelectedValue);
            int paymentTermId = Convert.ToInt32(ddl_PaymentTerm.SelectedValue);
            int vendorId = -1;
            DateTime shipReceiptDateFrom = DateTime.MinValue;
            DateTime shipReceiptDateTo = DateTime.MinValue;
            DateTime STWDateFrom = DateTime.MinValue;
            DateTime STWDateTo = DateTime.MinValue;
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            ArrayList shippingusersId = new ArrayList();
            string shippingusersName = string.Empty;
            foreach (ListItem item in cbl_ShippingUser.Items)
                if (item.Selected)
                {
                    shippingusersId.Add(Convert.ToInt32(item.Value));
                    shippingusersName += (shippingusersName == string.Empty ? "" : ", ") + item.Text;
                }
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue);
            int isUploadDMS = Convert.ToInt32(ddlDMS.SelectedValue);

            if (txt_Supplier.VendorId != int.MinValue)
            {
                vendorId = txt_Supplier.VendorId;
            }
            
            TypeCollector productTeamList = TypeCollector.Inclusive;
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
                ArrayList pt = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, office.OfficeId, this.LogonUserId, GeneralCriteria.ALLSTRING);
                foreach (OfficeStructureRef os in pt)
                    productTeamList.append(os.OfficeStructureId);
            }
            officeName = (officeGroupId == -1 ? "ALL" : officeName);


            if (txt_ShipReceiptDateFrom.Text.Trim() != "")
            {
                shipReceiptDateFrom = DateTimeUtility.getDate(txt_ShipReceiptDateFrom.Text.Trim());
                if (txt_ShipReceiptDateTo.Text.Trim() == "")
                    txt_ShipReceiptDateTo.Text = txt_ShipReceiptDateFrom.Text;
                shipReceiptDateTo = DateTimeUtility.getDate(txt_ShipReceiptDateTo.Text.Trim());
            }
            if (txt_StockToWHDateFrom.Text.Trim() != "")
            {
                STWDateFrom = DateTimeUtility.getDate(txt_StockToWHDateFrom.Text.Trim());
                if (txt_StockToWHDateTo.Text.Trim() == "")
                    txt_StockToWHDateTo.Text = txt_StockToWHDateFrom.Text;
                STWDateTo = DateTimeUtility.getDate(txt_StockToWHDateTo.Text.Trim());
            }
            if (txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            return ShippingReportManager.Instance.getOutstandingPaymentReport(shipReceiptDateFrom, shipReceiptDateTo, STWDateFrom, STWDateTo, invoiceDateFrom, invoiceDateTo,
                vendorId, officeName,shippingusersName,officeIdList, productTeamList, termOfPurchaseId, paymentTermId, shippingusersId, isSampleOrder, isUTOrder, isUploadDMS, this.LogonUserId);            
        }



        protected void btn_Submit_Click(object sender, EventArgs arg)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "OutstandingPaymentDoc");
        }

        protected void btn_Export_Click(object sender, EventArgs arg)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "OutstandingPaymentDoc");
        }
    }
}
