using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.common.web.commander;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.shipping;
using com.next.isam.webapp.commander;
using com.next.infra.util;
using com.next.common.datafactory.worker;

namespace com.next.isam.webapp.reporter
{
    public partial class CTSSTWReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControl();
            }
        }


        void initControl()
        {
            //ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);            
            //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_purchaseTerm.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_PackingMethod.bindList(WebUtil.getPackingMethodList(), "PackingMethodDescription", "PackingMethodId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_CountryOfOrigin.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_ShipmentPort.bindList(WebUtil.getShipmentPortList(), "ShipmentPortDescription", "ShipmentPortId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_OPRType.bindList((ArrayList)OPRFabricType.getCollectionValueForReport(), "OPRFabricReportSelectionName", "OPRFabricTypeId");

            ddl_Destination.bindList(WebUtil.getCustomerDestinationList(), "DestinationDesc", "CustomerDestinationId", "", "--All--", GeneralCriteria.ALL.ToString());

            uclOfficeProductTeamSelection.UserId = this.LogonUserId;

            //uclProductTeam.setWidth(305);
            //uclProductTeam.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.productCode);

            ddl_SZOrder.Visible = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.HKShippingReport) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.OffshoreShippingReport);

            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();

            cbl_ShipmentMethod.DataSource = WebUtil.getShipmentMethodList();
            cbl_ShipmentMethod.DataTextField = "ShipmentMethodDescription";
            cbl_ShipmentMethod.DataValueField = "ShipmentMethodId";
            cbl_ShipmentMethod.DataBind();

            if (!ddl_SZOrder.Visible)
            {
                ddl_SZOrder.SelectedIndex = 0;  // Both SZ & Non-SZ order
            }

            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                    item.Selected = true;
            }

            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                item.Selected = true;
            }

            ucl_sortingOrder.addItem("Customer", "Customer", false);
            ucl_sortingOrder.addItem("Contract No.", "c.ContractNo", false);
            ucl_sortingOrder.addItem("Delivery No.", "s.DeliveryNo", false);
            ucl_sortingOrder.addItem("Product Team", "ProductTeam", false);
            ucl_sortingOrder.addItem("Supplier", "Supplier", false);
            ucl_sortingOrder.addItem("Item No.", "ItemNo", false);
            ucl_sortingOrder.addItem("Loading Port", "ShipmentPort", false);
            ucl_sortingOrder.addItem("Destination", "Destination", false);
            ucl_sortingOrder.addItem("Shipped Qty", "s.TotalShippedQty", false);
            ucl_sortingOrder.addItem("Invoice No", "InvoiceNo", true);
            ucl_sortingOrder.addItem("Invoice Date", "InvoiceDate", false);
            ucl_sortingOrder.addItem("Currency", "Currency", false);
            ucl_sortingOrder.addItem("Invoice Amount", "InvoiceAmount", false);
            ucl_sortingOrder.addItem("Shipment Method", "ShipmentMethod", false);
            ucl_sortingOrder.addItem("OPR Type", "OPRType", false);
            ucl_sortingOrder.addItem("Supplier Invoice No.", "i.SupplierInvoiceNo", false);

        }

        private ReportClass genReport()
        {
            DateTime CTSDateFrom = DateTime.MinValue;
            DateTime CTSDateTo = DateTime.MinValue;
            DateTime STWDateFrom = DateTime.MinValue;
            DateTime STWDateTo = DateTime.MinValue;
            int isSelfBilled = Convert.ToInt32(ddl_SelfBilled.SelectedValue);
            int isSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isDualSourcing = Convert.ToInt32(ddl_DualSource.SelectedValue);
            int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue);
            int packingMethodId = Convert.ToInt32(ddl_PackingMethod.SelectedValue);
            int countryOfOriginId = Convert.ToInt32(ddl_CountryOfOrigin.SelectedValue);
            int seasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            int shipmentPortId = Convert.ToInt32(ddl_ShipmentPort.SelectedValue);
            int oprType = Convert.ToInt32(ddl_OPRType.SelectedValue);
            int termOfPurchaseId = Convert.ToInt32(ddl_purchaseTerm.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);

            if (txt_CTSDateFrom.Text.Trim() != "")
            {                
                CTSDateFrom = DateTimeUtility.getDate(txt_CTSDateFrom.Text);
                if (txt_CTSDateTo.Text.Trim() == "")
                    txt_CTSDateTo.Text = txt_CTSDateFrom.Text;
                CTSDateTo = DateTimeUtility.getDate(txt_CTSDateTo.Text);
            }
            if (txt_STWDateFrom.Text.Trim() != "")
            {
                STWDateFrom = DateTimeUtility.getDate(txt_STWDateFrom.Text);
                if (txt_STWDateTo.Text.Trim() == "")
                    txt_STWDateTo.Text = txt_STWDateFrom.Text;
                STWDateTo = DateTimeUtility.getDate(txt_STWDateTo.Text);
            }

            TypeCollector workflowStatusList = TypeCollector.Inclusive;
            if (ddl_Shipped.SelectedIndex == 0)
            {
                workflowStatusList.append(ContractWFS.PENDING_FOR_APPROVAL.Id);
                workflowStatusList.append(ContractWFS.AMEND.Id);
                workflowStatusList.append(ContractWFS.APPROVED.Id);
                workflowStatusList.append(ContractWFS.PO_PRINTED.Id);
                workflowStatusList.append(ContractWFS.INVOICED.Id);
            }
            else if (ddl_Shipped.SelectedIndex == 1)
            {
               workflowStatusList.append(ContractWFS.PENDING_FOR_APPROVAL.Id);
                workflowStatusList.append(ContractWFS.AMEND.Id);
                workflowStatusList.append(ContractWFS.APPROVED.Id);
                workflowStatusList.append(ContractWFS.PO_PRINTED.Id);
            }
            else
            { 
                workflowStatusList.append(ContractWFS.INVOICED.Id);
                
            }

            TypeCollector officeList = uclOfficeProductTeamSelection.getOfficeList();
            TypeCollector productTeamList = uclOfficeProductTeamSelection.getProductCodeList();

            if (!officeList.contains(OfficeId.DG.Id))
                handlingOfficeId = -1;

            TypeCollector customerDestinationList;
            if (ddl_Destination.SelectedValue == "-1")
            {
                customerDestinationList = TypeCollector.Inclusive;
                ArrayList arr = WebUtil.getCustomerDestinationList();
                foreach (CustomerDestinationDef def in arr)
                {
                    customerDestinationList.append(def.CustomerDestinationId);
                }
            }
            else
            {
                customerDestinationList = TypeCollector.Inclusive;
                customerDestinationList.append(Convert.ToInt32(ddl_Destination.SelectedValue));
            }

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

             

            return ShippingReportManager.Instance.getCTSSTWReport(CTSDateFrom, CTSDateTo, STWDateFrom, STWDateTo, isDualSourcing, isSelfBilled, isSZOrder, isUTOrder,
                isLDPOrder, isSampleOrder, workflowStatusList, shipmentMethodCollector, customerTypeCollector, packingMethodId, countryOfOriginId, officeList, productTeamList, handlingOfficeId,
                seasonId, shipmentPortId, oprType, customerDestinationList, termOfPurchaseId, chk_ShowProductTeam.Checked, ucl_sortingOrder.SortingField, this.LogonUserId);            
        }


        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "CTSSTWReport");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "CTSSTWReport");
        }

    }
}
 