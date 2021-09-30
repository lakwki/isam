using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using com.next.common.web.commander;
using com.next.isam.webapp.commander;
using com.next.isam.domain.types;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.infra.util;
using com.next.common.domain.types;
using System.Web.Script.Services;
using System.Web.Services;
using com.next.isam.domain.shipping;
using com.next.isam.reporter.shipping;
using System.Data;
using System.IO;
using com.next.isam.reporter.helper;
using CrystalDecisions.CrystalReports.Engine;

namespace com.next.isam.webapp.reporter
{
    public partial class TradingAFReport : System.Web.UI.Page
    {
        public int LogonUserId
        {
            get { return ConvertUtility.toInt32(Context.Request.ServerVariables["AUTH_USER"]); }
        }
        
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
            bindCheckBoxList(cbl_LoadingPort, WebUtil.getShipmentPortList(), "ShipmentPortDescription", "ShipmentPortId", "ALL", GeneralCriteria.ALL.ToString());
            bindCheckBoxList(cbl_ShipmentMethod, WebUtil.getShipmentMethodList(), "ShipmentMethodDescription", "ShipmentMethodId", "ALL", GeneralCriteria.ALL.ToString());

            uclOfficeSelection.UserId = this.LogonUserId;

            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
        }

        private void bindCheckBoxList(CheckBoxList cbl, ICollection list, string textField, string valueField, string selectAllFieldText, string selectAllFieldValue)
        {
            if (textField == "ShipmentMethodDescription") 
            {
                List<ShipmentMethodRef> ilist = new List<ShipmentMethodRef>();
                foreach(ShipmentMethodRef item in list)
                {
                    if(item.ShipmentMethodDescription=="AIR"||item.ShipmentMethodDescription=="ECOAIR"||item.ShipmentMethodDescription=="SEA/AIR")
                    {
                        ilist.Add(item);
                    }
                }
                cbl.DataSource = ilist;
            }
            if (textField != "ShipmentMethodDescription") 
            {
                cbl.DataSource = list;
            }
            cbl.DataTextField = textField;
            cbl.DataValueField = valueField;
            cbl.DataBind();
            if (selectAllFieldText != null && selectAllFieldValue != null)
            {
                cbl.Items.Insert(0, new ListItem(selectAllFieldText, selectAllFieldValue));
                cbl.Items[0].Attributes.Add("onclick", "clickAll(this);");
            }
            cbl.Attributes.Add("onclick", "refreshCheckBox(this)");
            foreach (ListItem itm in cbl.Items) itm.Selected = true;
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            
            if (!checking(sender))
            {
                return;
            }

            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "TradingAFReport");

        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!checking(sender))
            {
                return;
            }

            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "TradingAFReport");
        }
        /*protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!checking(sender))
            {
                return;
            }
            List<TradingAFReportDef> tradingAFReport = genReport();
            using (XLWorkbook wb = new XLWorkbook()) 
            {
                DataTable report = new DataTable();
                IXLWorksheet worksheet = wb.AddWorksheet(report, "TradingAFReport");
                for(int i=1; i<=13; i++)
                {
                    worksheet.Column(i).Width = 40;
                }
                worksheet.Cell("A" + 1).Value = "Next Sourcing Limited"; worksheet.Cell("B" + 1).Value = "Trading AF Report";
                worksheet.Cell("A" + 2).Value = "Office";
                TypeCollector officeList = uclOfficeSelection.getOfficeList();
                ICollection ids = officeList.Values;
                foreach (int id in ids) 
                {
                    OfficeRef office = CommonUtil.getOfficeRefByKey(id);
                    worksheet.Cell("B" + 2).Value += office.Description + ", ";
                }
                string customerAtWHDateFrom = txt_CustomerAtWHDateFrom.Text.Trim();
                string customerAtWHDateTo = txt_CustomerAtWHDateTo.Text.Trim();
                string customerAtWHDatePeriod = "";
                worksheet.Cell("A" + 3).Value = "Customer At-Warehouse Date"; 
                if(customerAtWHDateFrom == "" && customerAtWHDateTo == "")
                {
                    customerAtWHDatePeriod = "ALL";
                }
                else if (customerAtWHDateFrom != "" && customerAtWHDateTo != "")
                {
                    customerAtWHDatePeriod = customerAtWHDateFrom + " to " + customerAtWHDateTo;
                }
                else if (customerAtWHDateFrom != "" && customerAtWHDateTo == "") 
                {
                    customerAtWHDatePeriod = customerAtWHDateFrom + " to " + customerAtWHDateFrom;
                }
                worksheet.Cell("B" + 3).Value = customerAtWHDatePeriod;
                worksheet.Cell("A" + 4).Value = "Invoice Date";
                string invoiceDateFrom = txt_InvoiceDateFrom.Text.Trim();
                string invoiceDateTo = txt_InvoiceDateTo.Text.Trim();
                string invoiceDatePeriod = "";
                if (invoiceDateFrom == "" && invoiceDateTo == "")
                {
                    invoiceDatePeriod = "ALL";
                }
                else if (invoiceDateFrom != "" && invoiceDateTo != "")
                {
                    invoiceDatePeriod = invoiceDateFrom + " to " + invoiceDateTo;
                }
                else if (invoiceDateFrom != "" && invoiceDateTo == "")
                {
                    invoiceDatePeriod = invoiceDateFrom + " to " + invoiceDateFrom;
                }
                worksheet.Cell("B" + 4).Value = invoiceDatePeriod;
                worksheet.Cell("A" + 5).Value = "Supplier";
                string supplier = "";
                if (txt_Supplier.VendorId != int.MinValue)
                {
                    supplier = IndustryUtil.getVendorByKey(txt_Supplier.VendorId).Name;
                }
                else 
                {
                    supplier = "ALL";
                }
                worksheet.Cell("B" + 5).Value = supplier;
                worksheet.Cell("A" + 6).Value = "Country Of Origin";
                string country = "";
                if (cbl_CO.Items[0].Selected == true)
                {
                    country = "ALL";
                }
                else
                {
                    for (int i = 1; i < cbl_CO.Items.Count; i++)
                    {
                        if (cbl_CO.Items[i].Selected == true)
                            country += cbl_CO.Items[i].Text + ", ";
                    }
                }
                worksheet.Cell("B" + 6).Value = country;
                worksheet.Cell("A" + 7).Value = "Loading Port";
                string port = "";
                if (cbl_LoadingPort.Items[0].Selected == true)
                {
                    port = "ALL";
                }
                else
                {
                    for (int i = 1; i < cbl_LoadingPort.Items.Count; i++)
                    {
                        if (cbl_LoadingPort.Items[i].Selected == true)
                            port += cbl_LoadingPort.Items[i].Text + ", ";
                    }
                }
                worksheet.Cell("B" + 7).Value = port;
                worksheet.Cell("A" + 8).Value = "Shipment Method";
                string shipmode = "";
                if (cbl_ShipmentMethod.Items[0].Selected == true)
                {
                    shipmode = "ALL";
                }
                else
                {
                    for (int i = 1; i < cbl_ShipmentMethod.Items.Count; i++)
                    {
                        if (cbl_ShipmentMethod.Items[i].Selected == true)
                            shipmode += cbl_ShipmentMethod.Items[i].Text + ", ";
                    }
                }
                worksheet.Cell("B" + 8).Value = shipmode;
                worksheet.Cell("A" + 10).Value = "Office Code";
                worksheet.Cell("B" + 10).Value = "Invoice No.";
                worksheet.Cell("C" + 10).Value = "Invoice Date";
                worksheet.Cell("D" + 10).Value = "Supplier Name";
                worksheet.Cell("E" + 10).Value = "Customer At Warehouse Date";
                worksheet.Cell("F" + 10).Value = "Item No.";
                worksheet.Cell("G" + 10).Value = "Contract No.";
                worksheet.Cell("H" + 10).Value = "Delivery No.";
                worksheet.Cell("I" + 10).Value = "Ship Mode";
                worksheet.Cell("J" + 10).Value = "Order Quality";
                worksheet.Cell("K" + 10).Value = "Shipped Quality";
                worksheet.Cell("L" + 10).Value = "Estimate Freight Cost(USD)";
                worksheet.Cell("M" + 10).Value = "Actual Freight Cost(USD)";
                BuildYellowBackground(10, worksheet);
                int start = 11;
                foreach(TradingAFReportDef AFRecord in tradingAFReport)
                {
                    worksheet.Cell("A" + start).Value = AFRecord.OfficeCode;
                    worksheet.Cell("B" + start).Value = AFRecord.InvoiceId;
                    worksheet.Cell("C" + start).Value = AFRecord.InvoiceDate;
                    worksheet.Cell("D" + start).Value = AFRecord.SupplierName;
                    worksheet.Cell("E" + start).Value = AFRecord.CustomerAtWarehouseDate;
                    worksheet.Cell("F" + start).Value = AFRecord.ItemNo;
                    worksheet.Cell("G" + start).Value = AFRecord.ContractNo;
                    worksheet.Cell("H" + start).Value = AFRecord.DeliveryNo;
                    worksheet.Cell("I" + start).Value = AFRecord.ShipmentMethodDesc;
                    worksheet.Cell("J" + start).Value = AFRecord.TotalOrderQty;
                    worksheet.Cell("K" + start).Value = AFRecord.TotalshippedQty;
                    worksheet.Cell("L" + start).Value = AFRecord.TradingAFEstimationCost;
                    worksheet.Cell("M" + start).Value = AFRecord.TradingAFActualCost;
                    start++;
                }

                Response.Clear();

                Response.Buffer = true;

                Response.Charset = "";

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                Response.AddHeader("content-disposition", "attachment;filename=TradingAFReport.xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {

                    wb.SaveAs(MyMemoryStream);

                    MyMemoryStream.WriteTo(Response.OutputStream);

                    Response.Flush();

                    Response.End();

                }
            }
        }

        private void BuildYellowBackground(int i, IXLWorksheet worksheet)
        {
            string[] columnHead = new string[]{"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M"};
            for(int x=0;x<columnHead.Length;x++)
            {
                worksheet.Cell(columnHead[x] + i).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                worksheet.Cell(columnHead[x] + i).Style.Font.Bold = true;
            }
            if (columnIndex == 1) 
            { 
                worksheet.Cell("E" + i).Style.Fill.SetBackgroundColor(XLColor.Yellow);
            }
        }*/

        ReportClass genReport()
        {
            TypeCollector officeList = uclOfficeSelection.getOfficeList();
            DateTime custAtWHDateFrom = DateTime.MinValue;
            DateTime custAtWHDateTo = DateTime.MinValue;
            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;
            int vendorId = -1;
            TypeCollector countryOfOrigin = TypeCollector.Inclusive;
            TypeCollector loadingPort = TypeCollector.Inclusive;
            TypeCollector shipmentMethod = TypeCollector.Inclusive;

            if (txt_CustomerAtWHDateFrom.Text.Trim() != "")
            {
                custAtWHDateFrom = DateTimeUtility.getDate(txt_CustomerAtWHDateFrom.Text.Trim());
                if (txt_CustomerAtWHDateTo.Text.Trim() == "")
                    txt_CustomerAtWHDateTo.Text = txt_CustomerAtWHDateFrom.Text;
                custAtWHDateTo = DateTimeUtility.getDate(txt_CustomerAtWHDateTo.Text.Trim());
            }
            if (txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            if (txt_Supplier.VendorId != int.MinValue)
                vendorId = txt_Supplier.VendorId;

            foreach (ListItem itm in cbl_CO.Items) if (itm.Selected) countryOfOrigin.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_LoadingPort.Items) if (itm.Selected) loadingPort.append(Convert.ToInt32(itm.Value));
            foreach (ListItem itm in cbl_ShipmentMethod.Items) if (itm.Selected) shipmentMethod.append(Convert.ToInt32(itm.Value));

            return ShippingReportManager.Instance.getTradingAFReport(officeList, custAtWHDateFrom, custAtWHDateTo, invoiceDateFrom, invoiceDateTo, vendorId, countryOfOrigin, loadingPort, shipmentMethod, this.LogonUserId);
        }


        private bool checking(object sender)
        {
            //Please enter search criteria on one of below search criteria.
            int totalOffice = 0;
            int totalcountry = 0;
            int totalport = 0;
            int totalshipment = 0;
            TypeCollector officeList = uclOfficeSelection.getOfficeList();
            ICollection ids = officeList.Values;
            foreach (int id in ids) 
            {
                totalOffice++;
            }
            for (int i = 0; i < cbl_CO.Items.Count; i++)
            {
                if (cbl_CO.Items[i].Selected == true)
                    totalcountry++;
            }
            for (int i = 0; i < cbl_LoadingPort.Items.Count; i++)
            {
                if (cbl_LoadingPort.Items[i].Selected == true)
                    totalport++;
            }
            for (int i = 0; i < cbl_ShipmentMethod.Items.Count; i++)
            {
                if (cbl_ShipmentMethod.Items[i].Selected == true)
                    totalshipment++;
            }
            if ((txt_CustomerAtWHDateFrom.Text != "" && txt_CustomerAtWHDateTo.Text == "") || (txt_CustomerAtWHDateTo.Text != "" && txt_CustomerAtWHDateFrom.Text == "")) 
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Invalid Customer At-WareHouse Date Range');", true);
                return false;
            }
            if ((txt_InvoiceDateFrom.Text != "" && txt_InvoiceDateTo.Text == "") || (txt_InvoiceDateTo.Text != "" && txt_InvoiceDateFrom.Text == ""))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Invalid Invoice Date Range');", true);
                return false;
            }
            /*if (txt_CustomerAtWHDateFrom.Text.Trim()=="" && txt_CustomerAtWHDateTo.Text.Trim() == "" && txt_InvoiceDateFrom.Text.Trim() == "" && txt_InvoiceDateTo.Text.Trim() == "" && txt_Supplier.KeyTextBox.Text.Trim() == "" && totalOffice == 0 && totalcountry==0 && totalport == 0 && totalshipment == 0) 
            {
                correct = false;
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Please enter at least 1 searching criteria.');", true);
            }*/
            if (totalOffice == 0) 
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Please choose at least one or more Office');", true);
                return false;
            }
            if (totalcountry == 0) 
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Please choose ALL or preferred Country Of Origin');", true);
                return false;
            }
            if (totalport == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Please choose ALL or preferred Loading Port');", true);
                return false;
            }
            if (totalshipment == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Error", "alert('Please choose ALL or preferred Shipment Method');", true);
                return false;
            }
            return true;
        }

    }
}