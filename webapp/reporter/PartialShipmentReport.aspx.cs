using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.isam.domain.common;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.shipping;
using com.next.isam.webapp.commander;
using com.next.isam.domain.types;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class PartialShipmentReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            this.ddl_OPRFabricType.bindList((ArrayList)OPRFabricType.getCollectionValueForReport(), "OPRFabricReportSelectionName", "OPRFabricTypeId");
            ddl_termOfPurchase.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_PackingMethod.bindList(WebUtil.getPackingMethodList(), "PackingMethodDescription", "PackingMethodId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_PaymentTerm.bindList(WebUtil.getPaymentTermList(), "PaymentTermDescription", "PaymentTermId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_SZOrder.Visible = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.HKShippingReport) || 
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.OffshoreShippingReport) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.MerchandiserReport) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.HKAccountsReport) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.QAAdminReport);
            if (!ddl_SZOrder.Visible)
            {
                ddl_SZOrder.SelectedIndex = 2;
            }

            txt_Supplier.setWidth(305);            
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            uclOfficeProductTeamSelection.UserId = this.LogonUserId;

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

            ucl_SortingOrder1.addItem("Customer", "Customer", false);
            ucl_SortingOrder1.addItem("Product Team", "ProductTeam", false);
            ucl_SortingOrder1.addItem("Season", "Season", false);
            ucl_SortingOrder1.addItem("Office", "Office", false);
            ucl_SortingOrder1.addItem("Supplier", "Supplier", false);
            ucl_SortingOrder1.addItem("Item No.", "ItemNo", false);
            ucl_SortingOrder1.addItem("Contract No.", "c.ContractNo", true);
            ucl_SortingOrder1.addItem("Delivery No.", "s.DeliveryNo", true);
            ucl_SortingOrder1.addItem("Customer At-Warehouse Date", "s.CustomerAtWarehouseDate", false);
            ucl_SortingOrder1.addItem("P.O. Qty", "s.TotalPOQty", false);
            ucl_SortingOrder1.addItem("Loading Port", "LoadingPort", false);
            ucl_SortingOrder1.addItem("Destination", "CustomerDestination", false);
            ucl_SortingOrder1.addItem("Currency", "SellCurrency", false);
            ucl_SortingOrder1.addItem("Amount", "AmountUSD", false);
            ucl_SortingOrder1.addItem("Packing Method", "PackingMethod", false);
            ucl_SortingOrder1.addItem("Shipment Method", "ShipmentMethod", false);
            ucl_SortingOrder1.addItem("OPR Type", "OPRType", false);

            ListItemCollection list = new ListItemCollection();
            foreach (CountryOfOriginRef r in CommonUtil.getCountryOfOriginList())
                list.Add(new ListItem(r.Name, r.CountryOfOriginId.ToString()));
            uclCountryOfOrigin.bindList(list);
            uclCountryOfOrigin.SetAllCheckBoxStatus(true);
            uclCountryOfOrigin.setWidth(150);
            //uclCountryOfOrigin.setTitleText("C/O");
            //uclCountryOfOrigin.enableTitle(true);
            //uclCountryOfOrigin.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId");

            list.Clear();
            ArrayList destList = WebUtil.getCustomerDestinationList();
            destList.Sort(new ArrayListHelper.Sorter("DestinationDesc"));
            foreach (CustomerDestinationDef d in destList)
                list.Add(new ListItem(d.DestinationDesc, d.CustomerDestinationId.ToString()));
            uclDestination.bindList(list);
            uclDestination.SetAllCheckBoxStatus(true);
            uclDestination.setWidth(250);

            list.Clear();
            foreach (ShipmentPortRef r in WebUtil.getShipmentPortList())
                list.Add(new ListItem(r.ShipmentPortDescription, r.ShipmentPortId.ToString()));
            uclLoadingPort.bindList(list);
            uclLoadingPort.SetAllCheckBoxStatus(true);
            uclLoadingPort.setWidth(200);
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "PartialShipment");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "PartialShipment");
        }

        protected ReportClass genReport()
        {
            DateTime atWHDateFrom = DateTime.MinValue;
            DateTime atWHDateTo = DateTime.MinValue;
            DateTime bookInWHDateFrom = DateTime.MinValue;
            DateTime bookInWHDateTo = DateTime.MinValue;
            int vendorId = -1;
            int packingMethodId = Convert.ToInt32(ddl_PackingMethod.SelectedValue);
            int paymentTermId = Convert.ToInt32(ddl_PaymentTerm.SelectedValue);
            int termOfPurchaseId = Convert.ToInt32(ddl_termOfPurchase.SelectedValue);
            int oprTypeId = Convert.ToInt32(ddl_OPRFabricType.SelectedValue);
            int isNSLSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isDualSourcingOrder = Convert.ToInt32(ddl_DualSource.SelectedValue);
            int seasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            int isDFOrder = Convert.ToInt32(ddl_DFOrder.SelectedValue);
            int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue);
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);

            if (txt_AtWHDateFrom.Text.Trim() != "")
            {
                atWHDateFrom = DateTimeUtility.getDate(txt_AtWHDateFrom.Text.Trim());
                if (txt_AtWHDateTo.Text.Trim() == "")
                    txt_AtWHDateTo.Text = txt_AtWHDateFrom.Text;
                atWHDateTo = DateTimeUtility.getDate(txt_AtWHDateTo.Text.Trim());
            }
            if (txt_BookInWHDateFrom.Text.Trim() != "")
            {
                bookInWHDateFrom = DateTimeUtility.getDate(txt_BookInWHDateFrom.Text.Trim());
                if (txt_BookInWHDateTo.Text.Trim() == "")
                    txt_BookInWHDateTo.Text = txt_BookInWHDateFrom.Text;
                bookInWHDateTo = DateTimeUtility.getDate(txt_BookInWHDateTo.Text.Trim());
            }

            TypeCollector productTeamList = uclOfficeProductTeamSelection.getProductCodeList();            
            TypeCollector officeIdList = uclOfficeProductTeamSelection.getOfficeList();

            if (!officeIdList.contains(OfficeId.DG.Id))
                handlingOfficeId = -1;

            if (txt_Supplier.VendorId != int.MinValue)
                vendorId = txt_Supplier.VendorId;

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

            string countryOfOriginNameList = string.Empty;
            TypeCollector countryOfOriginIdList = uclCountryOfOrigin.getSelectedItem(out countryOfOriginNameList);
            if (countryOfOriginNameList == "ALL")
                countryOfOriginIdList = TypeCollector.Inclusive;
            string customerDestinationNameList = string.Empty;
            TypeCollector customerDestinationIdList = uclDestination.getSelectedItem(out customerDestinationNameList);
            if (customerDestinationNameList == "ALL")
                customerDestinationIdList = TypeCollector.Inclusive;
            string shipmentPortNameList = string.Empty;
            TypeCollector shipmentPortIdList = uclLoadingPort.getSelectedItem(out shipmentPortNameList);
            if (shipmentPortNameList == "ALL")
                shipmentPortIdList = TypeCollector.Inclusive;

            return ShippingReportManager.Instance.getPartialShipmentReport(atWHDateFrom, atWHDateTo, officeIdList, productTeamList, handlingOfficeId, vendorId, 
                    isNSLSZOrder, isDualSourcingOrder, isLDPOrder, isSampleOrder, isDFOrder, isUTOrder, termOfPurchaseId, seasonId,
                    countryOfOriginIdList, shipmentPortIdList, customerDestinationIdList, 
                    oprTypeId, packingMethodId, shipmentMethodCollector, customerTypeCollector, bookInWHDateFrom, bookInWHDateTo, paymentTermId, ucl_SortingOrder1.SortingField, this.LogonUserId);
        }
    }
}
