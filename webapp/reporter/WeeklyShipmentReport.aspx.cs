using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.shipping;
using com.next.isam.reporter.helper;
using com.next.infra.util;

namespace com.next.isam.webapp.reporter
{
    public partial class WeeklyShipmentReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            bindCheckBoxList(cbl_CO, CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "ALL", GeneralCriteria.ALL.ToString());
            bindCheckBoxList(cbl_Destination, WebUtil.getCustomerDestinationList(), "DestinationDesc", "CustomerDestinationId", "ALL", GeneralCriteria.ALL.ToString());
            bindCheckBoxList(cbl_LoadingPort, WebUtil.getShipmentPortList(), "ShipmentPortDescription", "ShipmentPortId", "ALL", GeneralCriteria.ALL.ToString());
            bindCheckBoxList(cbl_PackingMethod, WebUtil.getPackingMethodList(), "PackingMethodDescription", "PackingMethodId", "ALL", GeneralCriteria.ALL.ToString());
            bindCheckBoxList(cbl_TermOfPurchase, WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "ALL", GeneralCriteria.ALL.ToString());
            bindCheckBoxList(cbl_ShipmentStatus, ContractWFS.getCollectionValue(), "Name", "Id", "ALL", GeneralCriteria.ALL.ToString());
            bindCheckBoxList(cbl_Customer, WebUtil.getCustomerList(), "CustomerCode", "CustomerId", "ALL", GeneralCriteria.ALL.ToString());
            bindCheckBoxList(cbl_ShipmentMethod, WebUtil.getShipmentMethodList(), "ShipmentMethodDescription", "ShipmentMethodId", "ALL", GeneralCriteria.ALL.ToString());
            ArrayList oprTypeList = new ArrayList();
            foreach (OPRFabricType oprType in OPRFabricType.getCollectionValue())
                oprTypeList.Add(new ListItem(oprType.OPRFabricName, (oprType.OPRFabricTypeId == -1 ? 0 : oprType.OPRFabricTypeId).ToString()));    // change the ID for type 'N/A'(No OPR Fabric) to '0'
            bindCheckBoxList(cbl_OPRType, oprTypeList, "Text", "Value", "ALL", GeneralCriteria.ALL.ToString());

            ucl_SortingOrder.addItem("Customer", "Customer", false);
            ucl_SortingOrder.addItem("Product Team", "ProductTeam", false);
            ucl_SortingOrder.addItem("Season", "Season", false);
            ucl_SortingOrder.addItem("Supplier", "Supplier", false);
            ucl_SortingOrder.addItem("Contract No.", "ContractNo", true);
            ucl_SortingOrder.addItem("Delivery No", "DeliveryNo", true);
            ucl_SortingOrder.addItem("Customer At WH Date", "CustomerAtWarehouseDate", false);
            ucl_SortingOrder.addItem("Item No.", "ItemNo", true);
            ucl_SortingOrder.addItem("Loading Port", "LoadingPort", false);

            ucl_OfficeProdTeamSelection.UserId = this.LogonUserId;

            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
        }

        private void bindCheckBoxList(CheckBoxList cbl, ICollection list, string textField, string valueField)
        {
            bindCheckBoxList(cbl, list, textField, valueField, null, null);
        }

        private void bindCheckBoxList(CheckBoxList cbl, ICollection list, string textField, string valueField, string selectAllFieldText, string selectAllFieldValue)
        {
            cbl.DataSource = list;
            cbl.DataTextField = textField;
            cbl.DataValueField = valueField;
            cbl.DataBind();
            if (selectAllFieldText != null && selectAllFieldValue != null)
            {
                cbl.Items.Insert(0, new ListItem(selectAllFieldText, selectAllFieldValue));
                cbl.Items[0].Attributes.Add("onclick", "clickAll(this);");
            }
            cbl.Attributes.Add("onclick", "refreshCheckBox(this)");
            if (cbl == cbl_Customer)
            {
                foreach (ListItem item in cbl.Items)
                {
                    if (item.Value != "-1")
                    {
                        if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                            item.Selected = true;
                    }
                }
            }
            else
            {
                foreach (ListItem itm in cbl.Items) itm.Selected = true;
            }
        }

        ReportClass genReport()
        {
            DateTime stockToWHDateFrom = DateTime.MinValue;
            DateTime stockToWHDateTo = DateTime.MinValue;
            DateTime custAtWHDateFrom = DateTime.MinValue;
            DateTime custAtWHDateTo = DateTime.MinValue;
            DateTime bookingDateFrom = DateTime.MinValue;
            DateTime bookingDateTo = DateTime.MinValue;
            DateTime bookedInWHDateFrom = DateTime.MinValue;
            DateTime bookedInWHDateTo = DateTime.MinValue;
            DateTime sailingDateFrom = DateTime.MinValue;
            DateTime sailingDateTo = DateTime.MinValue;
            string voyageNo = "";
            int vendorId = -1;
            TypeCollector countryOfOrigin = TypeCollector.Inclusive;
            TypeCollector destination = TypeCollector.Inclusive;
            TypeCollector loadingPort = TypeCollector.Inclusive;
            TypeCollector packingMethod = TypeCollector.Inclusive;
            TypeCollector termOfPurchase = TypeCollector.Inclusive;
            TypeCollector oprType = TypeCollector.Inclusive;
            TypeCollector customer = TypeCollector.Inclusive;
            TypeCollector shipmentStatus = TypeCollector.Inclusive;
            TypeCollector shipmentMethod = TypeCollector.Inclusive;
            int isNSLSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isDualSourcingOrder = Convert.ToInt32(ddl_DualSource.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);
           
            #region Trial Version
            /* Trial
            int isNSLSZOrder = 2;
            int isDualSourcingOrder = 2;
            int isLDPOrder = 2;
            int isSampleOrder = 2;

            foreach(ListItem itm in cbl_OrderType.Items)
                if (itm.Selected)
                {
                    if (itm.Value == "-1")
                    {
                        isNSLSZOrder = -1;
                        isDualSourcingOrder = -1;
                        isLDPOrder = -1;
                        isSampleOrder = -1;
                        break;
                    }
                    else
                    {
                        int val = int.Parse(itm.Value.Substring(1, 1));
                        switch (itm.Value.Substring(0, 1))
                        {
                            case "M": //Mock Shop/Press Sample or MainLine
                                isSampleOrder -= val;
                                break;
                            case "S": // NSL SZ 
                                isNSLSZOrder -= val;
                                break;
                            case "D": //Dual Sourcing
                                isDualSourcingOrder -= val;
                                break;
                            case "L": // LDP
                                isLDPOrder -= val;
                                break;
                        }
                    }
                }
            */
            #endregion

            if (txt_StockToWHDateFrom.Text.Trim() != "")
            {
                stockToWHDateFrom = DateTimeUtility.getDate(txt_StockToWHDateFrom.Text.Trim());
                if (txt_StockToWHDateTo.Text.Trim() == "")
                    txt_StockToWHDateTo.Text = txt_StockToWHDateFrom.Text;
                stockToWHDateTo = DateTimeUtility.getDate(txt_StockToWHDateTo.Text.Trim());
            }
            if (txt_CustomerAtWHDateFrom.Text.Trim() != "")
            {
                custAtWHDateFrom = DateTimeUtility.getDate(txt_CustomerAtWHDateFrom.Text.Trim());
                if (txt_CustomerAtWHDateTo.Text.Trim() == "")
                    txt_CustomerAtWHDateTo.Text = txt_CustomerAtWHDateFrom.Text;
                custAtWHDateTo = DateTimeUtility.getDate(txt_CustomerAtWHDateTo.Text.Trim());
            }
            if (txt_BookingDateFrom.Text.Trim() != "")
            {
                bookingDateFrom = DateTimeUtility.getDate(txt_BookingDateFrom.Text.Trim());
                if (txt_BookingDateTo.Text.Trim() == "")
                    txt_BookingDateTo.Text = txt_BookingDateFrom.Text;
                bookingDateTo = DateTimeUtility.getDate(txt_BookingDateTo.Text.Trim());
            }
            if (txt_BookedInWHDateFrom.Text.Trim() != "")
            {
                bookedInWHDateFrom = DateTimeUtility.getDate(txt_BookedInWHDateFrom.Text.Trim());
                if (txt_BookedInWHDateTo.Text.Trim() == "")
                    txt_BookedInWHDateTo.Text = txt_BookedInWHDateFrom.Text;
                bookedInWHDateTo = DateTimeUtility.getDate(txt_BookedInWHDateTo.Text.Trim());
            }
            if (txt_SailingDateFrom.Text.Trim() != "")
            {
                sailingDateFrom = DateTimeUtility.getDate(txt_SailingDateFrom.Text.Trim());
                if (txt_SailingDateTo.Text.Trim() == "")
                    txt_SailingDateTo.Text = txt_SailingDateFrom.Text;
                sailingDateTo = DateTimeUtility.getDate(txt_SailingDateTo.Text.Trim());
            }
            voyageNo = txt_VoyageNo.Text.Trim();
            
            TypeCollector officeList = ucl_OfficeProdTeamSelection.getOfficeList();
            TypeCollector productTeamList = ucl_OfficeProdTeamSelection.getProductCodeList();

            if (!officeList.contains(OfficeId.DG.Id))
                handlingOfficeId = -1;

            if (txt_Supplier.VendorId != int.MinValue)
                vendorId = txt_Supplier.VendorId;

            foreach (ListItem itm in cbl_CO.Items) if (itm.Selected) countryOfOrigin.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_Destination.Items) if (itm.Selected) destination.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_LoadingPort.Items) if (itm.Selected) loadingPort.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_PackingMethod.Items) if (itm.Selected) packingMethod.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_TermOfPurchase.Items) if (itm.Selected) termOfPurchase.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_OPRType.Items) if (itm.Selected) oprType.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_Customer.Items) if (itm.Selected) customer.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_ShipmentStatus.Items) if (itm.Selected) shipmentStatus.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_ShipmentMethod.Items) if (itm.Selected) shipmentMethod.append(Convert.ToInt32(itm.Value));

            string lcNoFrom = txt_LCNoFrom.Text;
            string lcNoTo = (txt_LCNoTo.Text.Trim() == "" ? txt_LCNoFrom.Text : txt_LCNoTo.Text);
            int paymentTermId = (ddl_PaymentTerm.selectedValueToInt);


            string sortField = ucl_SortingOrder.SortingField;
            return ShippingReportManager.Instance.getWeeklyShipmentReport(stockToWHDateFrom, stockToWHDateTo, custAtWHDateFrom, custAtWHDateTo,
                bookingDateFrom, bookingDateTo, bookedInWHDateFrom, bookedInWHDateTo, sailingDateFrom, sailingDateTo,
                voyageNo, officeList, productTeamList, handlingOfficeId, customer, countryOfOrigin, destination, loadingPort, shipmentMethod,
                packingMethod, vendorId, termOfPurchase, oprType, isNSLSZOrder, isDualSourcingOrder, isLDPOrder, isSampleOrder,
                lcNoFrom, lcNoTo, paymentTermId, shipmentStatus, sortField, this.LogonUserId);
        }


        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "WeeklyShipmentReport");

        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "WeeklyShipmentReport");
        }
        
    }
}
