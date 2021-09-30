using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.shipping;
using com.next.isam.webapp.commander;
using com.next.infra.util;


namespace com.next.isam.webapp.reporter
{
    public partial class EziBuyPartialShipmentReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            //ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

            //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());

            this.ddl_Phase.Items.Add(new ListItem("-- All --", "-1".ToString()));
            for (int i = 1; i <= 5; i++)
                this.ddl_Phase.Items.Add(new ListItem(i.ToString(), i.ToString()));

            ddl_termOfPurchase.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_Destination.bindList(WebUtil.getCustomerDestinationList(), "DestinationDesc", "CustomerDestinationId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_CO.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_ShipmentPort.bindList(WebUtil.getShipmentPortList(), "ShipmentPortDescription", "ShipmentPortId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_PackingMethod.bindList(WebUtil.getPackingMethodList(), "PackingMethodDescription", "PackingMethodId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_SZOrder.Visible = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.HKShippingReport) || 
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.OffshoreShippingReport) ||
                CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.MerchandiserReport);
            if (!ddl_SZOrder.Visible)
            {
                ddl_SZOrder.SelectedIndex = 2;
            }

            txt_Supplier.setWidth(305);            
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            uclOfficeProductTeamSelection.UserId = this.LogonUserId;

            cbl_ShipmentMethod.DataSource = WebUtil.getShipmentMethodList();
            cbl_ShipmentMethod.DataTextField = "ShipmentMethodDescription";
            cbl_ShipmentMethod.DataValueField = "ShipmentMethodId";
            cbl_ShipmentMethod.DataBind();

            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                item.Selected = true;
            }

            ucl_SortingOrder1.addItem("Product Team", "ProductTeam", false);
            ucl_SortingOrder1.addItem("Season", "Season", false);
            ucl_SortingOrder1.addItem("Office", "Office", true);
            ucl_SortingOrder1.addItem("Supplier", "Supplier", true);
            ucl_SortingOrder1.addItem("Item No.", "ItemNo", false);
            ucl_SortingOrder1.addItem("Contract No.", "c.ContractNo", false);
            ucl_SortingOrder1.addItem("Delivery No.", "s.DeliveryNo", false);
            ucl_SortingOrder1.addItem("Customer At-Warehouse Date", "s.CustomerAtWarehouseDate", false);
            ucl_SortingOrder1.addItem("P.O. Qty", "s.TotalPOQty", false);
            ucl_SortingOrder1.addItem("Loading Port", "LoadingPort", false);
            ucl_SortingOrder1.addItem("Destination", "CustomerDestination", false);
            ucl_SortingOrder1.addItem("Currency", "SellCurrency", false);
            ucl_SortingOrder1.addItem("Amount", "AmountUSD", false);
            ucl_SortingOrder1.addItem("Packing Method", "PackingMethod", false);
            ucl_SortingOrder1.addItem("Shipment Method", "ShipmentMethod", false);
            ucl_SortingOrder1.addItem("Phase", "PhaseId", false);
            ucl_SortingOrder1.addItem("Origin Forwarder Date", "OriginForwarderDate", true);

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
            int countryOfOriginId = Convert.ToInt32(ddl_CO.SelectedValue);
            int customerDestinationId = Convert.ToInt32(ddl_Destination.SelectedValue);
            int shipmentPortId = Convert.ToInt32(ddl_ShipmentPort.SelectedValue);
            int packingMethodId = Convert.ToInt32(ddl_PackingMethod.SelectedValue);
            int termOfPurchaseId = Convert.ToInt32(ddl_termOfPurchase.SelectedValue);
            int phaseId = Convert.ToInt32(ddl_Phase.SelectedValue);
            int isNSLSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isDualSourcingOrder = Convert.ToInt32(ddl_DualSource.SelectedValue);
            int seasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);

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
            
            if (txt_Supplier.VendorId != int.MinValue)
                vendorId = txt_Supplier.VendorId;

            TypeCollector shipmentMethodCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                if (item.Selected)
                    shipmentMethodCollector.append(Convert.ToInt32(item.Value));
            }

            return ShippingReportManager.Instance.getEziBuyPartialShipmentReport(atWHDateFrom, atWHDateTo, officeIdList, productTeamList, vendorId, isNSLSZOrder, isDualSourcingOrder, isLDPOrder, isSampleOrder, termOfPurchaseId,
                countryOfOriginId, seasonId, shipmentPortId, customerDestinationId, phaseId, packingMethodId, shipmentMethodCollector, bookInWHDateFrom, bookInWHDateTo, ucl_SortingOrder1.SortingField, this.LogonUserId);
        }
    }
}
