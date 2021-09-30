using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.webapp.commander;
using com.next.isam.domain.types;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.invoice;
using com.next.infra.util;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.common.domain.module;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace com.next.isam.webapp.reporter
{
    public partial class InvoiceListReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                initControl();
            }

        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "invoiceList");
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {

            if (!Page.IsValid)
                return;
            //this.outputOpenXmlReport2();
            ReportHelper.export(genReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "invoiceList");
            
            
        }

        void initControl()
        {
            this.ddl_OPRType.bindList((ArrayList)OPRFabricType.getCollectionValueForReport(), "OPRFabricReportSelectionName", "OPRFabricTypeId");
            ddl_TermOfPurchase.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());

            ddl_CustomerDestination.bindList(WebUtil.getCustomerDestinationList(), "DestinationDesc", "CustomerDestinationId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_BaseCurrency.bindList(WebUtil.getCurrencyListForExchangeRate(), "CurrencyCode", "CurrencyId", "3");


            ArrayList arr_Year = WebUtil.getBudgetYearList();

            AccountFinancialCalenderDef finCalDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Now.AddDays(-10));
            ddl_Year.DataSource = arr_Year;
            ddl_Year.DataBind();
            ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();
            ddl_PeriodFrom.SelectedValue = finCalDef.Period.ToString();
            ddl_PeriodTo.SelectedValue = finCalDef.Period.ToString();
            

            uclOfficeProductTeamSelection.UserId = this.LogonUserId;

            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            cbl_Customer.DataSource = WebUtil.getCustomerList();
            cbl_Customer.DataTextField = "CustomerCode";
            cbl_Customer.DataValueField = "CustomerId";
            cbl_Customer.DataBind();

            cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            cbl_TradingAgency.DataTextField = "ShortName";
            cbl_TradingAgency.DataValueField = "TradingAgencyId";
            cbl_TradingAgency.DataBind();


            cbl_ShipmentMethod.DataSource = WebUtil.getShipmentMethodList();
            cbl_ShipmentMethod.DataTextField = "ShipmentMethodDescription";
            cbl_ShipmentMethod.DataValueField = "ShipmentMethodId";
            cbl_ShipmentMethod.DataBind();

            foreach (ListItem item in cbl_Customer.Items)
            {
                if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
                    item.Selected = true;
            }
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                item.Selected = true;
            }
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                item.Selected = true;
            }

            ucl_SortingOrder1.addItem("Invoice No", "InvoiceNo", true);
            ucl_SortingOrder1.addItem("Invoice Date", "i.InvoiceDate", false);
            ucl_SortingOrder1.addItem("Season", "Season", false);
            ucl_SortingOrder1.addItem("Product Team", "ProductTeam", false);
            ucl_SortingOrder1.addItem("Supplier", "Vendor", false);
            ucl_SortingOrder1.addItem("Contract No.", "c.ContractNo", false);
            ucl_SortingOrder1.addItem("Delivery No.", "s.DeliveryNo", false);
            ucl_SortingOrder1.addItem("Item No.", "ItemNo", false);
            ucl_SortingOrder1.addItem("Destination", "CustomerDestination", false);
            ucl_SortingOrder1.addItem("Term of Purchase", "TermOfPurchase", false);
            ucl_SortingOrder1.addItem("Trading Agency", "TradingAgency", false);
            ucl_SortingOrder1.addItem("Invoice Amount", "s.TotalShippedAmt", false);
            ucl_SortingOrder1.addItem("Total FOB Amount for CMT", "s.TotalShippedNetFOBAmtAfterDiscount", false);
            ucl_SortingOrder1.addItem("Supplier Invoice Amount", "s.TotalShippedSupplierGmtAmtAfterDiscount", false);
            ucl_SortingOrder1.addItem("Supplier Invoice No.", "i.SupplierInvoiceNo", false);
            ucl_SortingOrder1.addItem("Shipment Method", "ShipmentMethod", false);
            ucl_SortingOrder1.addItem("Country of Origin", "CO", false);
            ucl_SortingOrder1.addItem("Port of Loading", "LoadingPort", false);

        }

        private ReportClass genReport()
        {
            string invoicePrefix = "";
            int invoiceSeqFrom = 0;
            int invoiceSeqTo = 0;
            int invoiceYear = 0;
            int isSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue);
            int baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);

            if (txt_InvoiceNoFrom.Text.Trim() != "")
            {
                invoicePrefix = WebUtil.getInvoicePrefix(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqFrom = WebUtil.getInvoiceSeq(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqTo = WebUtil.getInvoiceSeq(txt_InvoiceNoTo.Text.Trim());
                invoiceYear = WebUtil.getInvoiceYear(txt_InvoiceNoFrom.Text.Trim());
            }

            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;

            if (rad_InvoiceDate.Checked && txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            DateTime purchaseExtractDateFrom = DateTime.MinValue;
            DateTime purchaseExtractDateTo = DateTime.MinValue;
            if (txt_PurchaseExtractDateFrom.Text.Trim() != "")
            {
                purchaseExtractDateFrom = DateTimeUtility.getDate(txt_PurchaseExtractDateFrom.Text.Trim());
                if (txt_PurchaseExtractDateTo.Text.Trim() == "")
                    txt_PurchaseExtractDateTo.Text = txt_PurchaseExtractDateFrom.Text;
                purchaseExtractDateTo = DateTimeUtility.getDate(txt_PurchaseExtractDateTo.Text.Trim());
            }

            DateTime departDateFrom = DateTime.MinValue;
            DateTime departDateTo = DateTime.MinValue;
            if (txt_DepartDateFrom.Text.Trim() != "")
            {
                departDateFrom = DateTimeUtility.getDate(txt_DepartDateFrom.Text.Trim());
                if (txt_DepartDateTo.Text.Trim() == "")
                    departDateTo = departDateFrom;
                else
                    departDateTo = DateTimeUtility.getDate(txt_DepartDateTo.Text.Trim());
            }

            bool isNextMfgUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.NMLAccountsReport);

            int vendorId = -1;
            if (isNextMfgUser)
            {
                vendorId = 3154;
            }
            else
            {
                if (txt_Supplier.VendorId != int.MinValue)
                {
                    vendorId = txt_Supplier.VendorId;
                }
            }

            int purchaseTermId = Convert.ToInt32(ddl_TermOfPurchase.SelectedValue);

            TypeCollector productTeamList = uclOfficeProductTeamSelection.getProductCodeList();            
            TypeCollector officeIdList = uclOfficeProductTeamSelection.getOfficeList();

            if (!officeIdList.contains(OfficeId.DG.Id))
                handlingOfficeId = -1;

            int seasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            int customerDestination = Convert.ToInt32(ddl_CustomerDestination.SelectedValue);
            int oprFabricType = Convert.ToInt32(ddl_OPRType.SelectedValue);

            int budgetYear = -1;
            int periodFrom = -1;
            int periodTo = -1;

            if (rad_FiscalPeriod.Checked)
            {
                budgetYear = Convert.ToInt32(ddl_Year.SelectedValue);
                periodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
                periodTo = Convert.ToInt32(ddl_PeriodTo.SelectedValue);
            }

            TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (item.Selected)
                    customerTypeCollector.append(Convert.ToInt32(item.Value));
            }

            TypeCollector tradingAgencyCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                if (item.Selected)
                    tradingAgencyCollector.append(Convert.ToInt32(item.Value));
            }

            TypeCollector shipmentMethodCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                if (item.Selected)
                    shipmentMethodCollector.append(Convert.ToInt32(item.Value));
            }

            int accountDocReceipt = int.Parse(ddl_AccDocReceipt.SelectedValue);

            InvoiceListRpt rpt = InvoiceReportManager.Instance.getInvoiceListReport(baseCurrencyId, invoicePrefix, invoiceSeqFrom, invoiceSeqTo, 
                invoiceYear, invoiceDateFrom, invoiceDateTo, purchaseExtractDateFrom, purchaseExtractDateTo, budgetYear, periodFrom, periodTo, 
                departDateFrom, departDateTo, vendorId, purchaseTermId, officeIdList, handlingOfficeId, seasonId, productTeamList, customerDestination, 
                oprFabricType, isSZOrder, isUTOrder, isLDPOrder, isSampleOrder, customerTypeCollector, tradingAgencyCollector, shipmentMethodCollector, 
                accountDocReceipt, txt_VoyageNo.Text.Trim(), ddl_PaymentTerm.selectedValueToInt, ddl_PaymentStatus.selectedValueToInt, ddl_LCPaymentChecked.selectedValueToInt, 
                ddl_ShippingDocReceiptDate.selectedValueToInt, this.LogonUserId, ucl_SortingOrder1.SortingField, rad_Version.SelectedValue);

            return rpt;
        }

        protected void rad_FiscalPeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_FiscalPeriod.Checked)
            {
                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false;

                ddl_Year.Enabled = true;
                ddl_PeriodFrom.Enabled = true;
                ddl_PeriodTo.Enabled = true;
            }
        }

        protected void rad_InvoiceDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_InvoiceDate.Checked)
            {
                txt_InvoiceDateFrom.Enabled = true;
                txt_InvoiceDateTo.Enabled = true;

                ddl_Year.Enabled = false;
                ddl_PeriodFrom.Enabled = false;
                ddl_PeriodTo.Enabled = false;
            }
        }

        protected void ddl_PeriodFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_PeriodTo.SelectedIndex = ddl_PeriodFrom.SelectedIndex;
        }

        protected void rad_Version_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_Version.SelectedValue == "SUN")
            {
                row_InvoiceDateRange.Visible = true;
                col_Period.Visible = true;

                ddl_Year.Enabled = true;
                ddl_PeriodFrom.Enabled = true;
                ddl_PeriodTo.Enabled = true;

                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false ;

            }
            else
            {
                row_InvoiceDateRange.Visible = false;
                col_Period.Visible = false;
                ddl_Year.Enabled = true;
                ddl_PeriodFrom.Enabled = true;
                ddl_PeriodTo.Enabled = true;
                rad_InvoiceDate.Checked = false;
                rad_FiscalPeriod.Checked = true;
                txt_InvoiceDateFrom.Enabled = false;
                txt_InvoiceDateTo.Enabled = false;
            }
        }


        private void outputOpenXmlReport()
        {
            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "InvoiceList.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(this.ApplPhysicalPath + @"reporter\tmpReport\InvoiceList-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;

            //byte[] byteArray = File.ReadAllBytes(sourceFileDir + sourceFileName);
            //MemoryStream mem = new MemoryStream();
            //mem.Write(byteArray, 0, (int)byteArray.Length);

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);
            //SpreadsheetDocument document = SpreadsheetDocument.Open(mem, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet1");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");

            string invoicePrefix = "";
            int invoiceSeqFrom = 0;
            int invoiceSeqTo = 0;
            int invoiceYear = 0;
            int isSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue);
            int baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);

            if (txt_InvoiceNoFrom.Text.Trim() != "")
            {
                invoicePrefix = WebUtil.getInvoicePrefix(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqFrom = WebUtil.getInvoiceSeq(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqTo = WebUtil.getInvoiceSeq(txt_InvoiceNoTo.Text.Trim());
                invoiceYear = WebUtil.getInvoiceYear(txt_InvoiceNoFrom.Text.Trim());
            }

            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;

            if (rad_InvoiceDate.Checked && txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            DateTime purchaseExtractDateFrom = DateTime.MinValue;
            DateTime purchaseExtractDateTo = DateTime.MinValue;
            if (txt_PurchaseExtractDateFrom.Text.Trim() != "")
            {
                purchaseExtractDateFrom = DateTimeUtility.getDate(txt_PurchaseExtractDateFrom.Text.Trim());
                if (txt_PurchaseExtractDateTo.Text.Trim() == "")
                    txt_PurchaseExtractDateTo.Text = txt_PurchaseExtractDateFrom.Text;
                purchaseExtractDateTo = DateTimeUtility.getDate(txt_PurchaseExtractDateTo.Text.Trim());
            }

            DateTime departDateFrom = DateTime.MinValue;
            DateTime departDateTo = DateTime.MinValue;
            if (txt_DepartDateFrom.Text.Trim() != "")
            {
                departDateFrom = DateTimeUtility.getDate(txt_DepartDateFrom.Text.Trim());
                if (txt_DepartDateTo.Text.Trim() == "")
                    departDateTo = departDateFrom;
                else
                    departDateTo = DateTimeUtility.getDate(txt_DepartDateTo.Text.Trim());
            }

            bool isNextMfgUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.NMLAccountsReport);

            int vendorId = -1;
            if (isNextMfgUser)
            {
                vendorId = 3154;
            }
            else
            {
                if (txt_Supplier.VendorId != int.MinValue)
                {
                    vendorId = txt_Supplier.VendorId;
                }
            }

            int purchaseTermId = Convert.ToInt32(ddl_TermOfPurchase.SelectedValue);

            TypeCollector productTeamList = uclOfficeProductTeamSelection.getProductCodeList();
            TypeCollector officeIdList = uclOfficeProductTeamSelection.getOfficeList();

            if (!officeIdList.contains(OfficeId.DG.Id))
                handlingOfficeId = -1;

            int seasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            int customerDestination = Convert.ToInt32(ddl_CustomerDestination.SelectedValue);
            int oprFabricType = Convert.ToInt32(ddl_OPRType.SelectedValue);

            int budgetYear = -1;
            int periodFrom = -1;
            int periodTo = -1;

            if (rad_FiscalPeriod.Checked)
            {
                budgetYear = Convert.ToInt32(ddl_Year.SelectedValue);
                periodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
                periodTo = Convert.ToInt32(ddl_PeriodTo.SelectedValue);
            }

            TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (item.Selected)
                    customerTypeCollector.append(Convert.ToInt32(item.Value));
            }

            TypeCollector tradingAgencyCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                if (item.Selected)
                    tradingAgencyCollector.append(Convert.ToInt32(item.Value));
            }

            TypeCollector shipmentMethodCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                if (item.Selected)
                    shipmentMethodCollector.append(Convert.ToInt32(item.Value));
            }

            int accountDocReceipt = int.Parse(ddl_AccDocReceipt.SelectedValue);


            InvoiceListReportDs ds = InvoiceReportManager.Instance.getInvoiceListDataSet(baseCurrencyId,
                invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, invoiceDateFrom,
                invoiceDateTo, purchaseExtractDateFrom, purchaseExtractDateTo,
                budgetYear, periodFrom, periodTo, departDateFrom, departDateTo, vendorId, purchaseTermId, officeIdList, handlingOfficeId, seasonId, productTeamList,
                customerDestination, oprFabricType, isSZOrder, isUTOrder, isLDPOrder, isSampleOrder,
                customerTypeCollector, tradingAgencyCollector, shipmentMethodCollector, accountDocReceipt, txt_VoyageNo.Text.Trim(),
                ddl_PaymentTerm.selectedValueToInt, ddl_PaymentStatus.selectedValueToInt, ddl_LCPaymentChecked.selectedValueToInt, ddl_ShippingDocReceiptDate.selectedValueToInt,
                this.LogonUserId, ucl_SortingOrder1.SortingField, rad_Version.SelectedValue);

            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(this.LogonUserId).DisplayName, CellValues.String);
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.Date);

            int[] list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 3, 32);
            int qtyTotalStyleId = OpenXmlUtil.getCellStyleId(document, templateWorksheetID, 4, 13);
            int amtTotalStyleId = OpenXmlUtil.getCellStyleId(document, templateWorksheetID, 4, 16);

            int startingRow = 23;
            foreach (InvoiceListReportDs.InvoiceListReportRow r in ds.InvoiceListReport.Rows)
            {
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 1, r.InvoiceNo, CellValues.SharedString, list[0]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 2, DateTimeUtility.getDateString(r.InvoiceDate), CellValues.Date, list[1]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 3, r.Season, CellValues.SharedString, list[2]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 4, r.ProductTeam, CellValues.SharedString, list[3]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 5, r.Vendor, CellValues.SharedString, list[4]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 6, r.ItemNo, CellValues.SharedString, list[5]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 7, r.ContractNo, CellValues.SharedString, list[6]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 8, r.DeliveryNo.ToString(), CellValues.Number, list[7]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 9, (r.IsSequenceNoNull() || r.SequenceNo == 0) ? string.Empty : r.SequenceNo.ToString(), CellValues.Number, list[8]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 10, r.CustomerDestination, CellValues.SharedString, list[9]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 11, r.TermOfPurchase, CellValues.SharedString, list[10]);
                
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 12, r.LoadingPort, CellValues.SharedString, list[11]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 13, r.TotalShippedQty.ToString(), CellValues.Number, list[12]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 14, r.SellCurrency, CellValues.SharedString, list[13]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 15, r.TotalShippedAmt.ToString(), CellValues.Number, list[14]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 16, r.TotalShippedAmtUSD.ToString(), CellValues.Number, list[15]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 17, r.TotalShippedNetFOBAmtAfterDiscount.ToString(), CellValues.Number, list[16]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 18, r.TotalShippedNetFOBAmtAfterDiscountUSD.ToString(), CellValues.Number, list[17]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 19, r.IsSupplierInvoiceNoNull() ? string.Empty : r.SupplierInvoiceNo, CellValues.SharedString, list[18]);
                
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 20, r.TotalShippedSupplierGmtAmtAfterDiscount.ToString(), CellValues.Number, list[19]);
                 
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 21, r.TotalShippedSupplierGmtAmtAfterDiscountUSD.ToString(), CellValues.Number, list[20]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 22, r.IsLCPaymentCheckedDateNull() ? string.Empty : DateTimeUtility.getDateString(r.LCPaymentCheckedDate), CellValues.Date, list[21]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 23, r.ShipmentMethod, CellValues.SharedString, list[22]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 24, r.CO, CellValues.SharedString, list[23]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 25, r.IsDepartDateNull() ? string.Empty : DateTimeUtility.getDateString(r.DepartDate), CellValues.Date, list[24]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 26, r.IsAPDateNull() ? string.Empty : DateTimeUtility.getDateString(r.APDate), CellValues.Date, list[25]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 27, r.PaymentTermId == 1 ? "OA" : "LC", CellValues.SharedString, list[26]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 28, r.IsLCNoNull() ? string.Empty : r.LCNo, CellValues.SharedString, list[27]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 29, r.IsLCIssueDateNull() ? string.Empty : DateTimeUtility.getDateString(r.LCIssueDate), CellValues.Date, list[28]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 30, r.IsLCExpiryDateNull() ? string.Empty : DateTimeUtility.getDateString(r.LCExpiryDate), CellValues.Date, list[29]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 31, r.QAAmt.ToString(), CellValues.Number, list[30]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 32, r.CommSvc.ToString(), CellValues.Number, list[31]);
                

                startingRow += 1;
                System.Diagnostics.Debug.Print("starting row : " + startingRow.ToString());

            }
            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 13, "=SUM(" + OpenXmlUtil.getColumnLetter(13) + "23:" + OpenXmlUtil.getColumnLetter(13) + (startingRow - 1).ToString() + ")", CellValues.String, qtyTotalStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 16, "=SUM(" + OpenXmlUtil.getColumnLetter(16) + "23:" + OpenXmlUtil.getColumnLetter(16) + (startingRow - 1).ToString() + ")", CellValues.String, amtTotalStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 18, "=SUM(" + OpenXmlUtil.getColumnLetter(18) + "23:" + OpenXmlUtil.getColumnLetter(18) + (startingRow - 1).ToString() + ")", CellValues.String, amtTotalStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 21, "=SUM(" + OpenXmlUtil.getColumnLetter(21) + "23:" + OpenXmlUtil.getColumnLetter(21) + (startingRow - 1).ToString() + ")", CellValues.String, amtTotalStyleId);


            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 8, worksheetID, startingRow + 2);
            OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(1, startingRow + 2), OpenXmlUtil.getCellReference(32, startingRow + 2));

            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();

            /*
            using (FileStream fileStream = new FileStream(destFile,
                            System.IO.FileMode.CreateNew))
            {
                mem.WriteTo(fileStream);
            }
            mem.Close();
            mem.Dispose();
            */
            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }


        private void outputOpenXmlReport2()
        {
            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "InvoiceList.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(this.ApplPhysicalPath + @"reporter\tmpReport\InvoiceList-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet1");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");

            string invoicePrefix = "";
            int invoiceSeqFrom = 0;
            int invoiceSeqTo = 0;
            int invoiceYear = 0;
            int isSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue);
            int baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);
            int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);

            if (txt_InvoiceNoFrom.Text.Trim() != "")
            {
                invoicePrefix = WebUtil.getInvoicePrefix(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqFrom = WebUtil.getInvoiceSeq(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqTo = WebUtil.getInvoiceSeq(txt_InvoiceNoTo.Text.Trim());
                invoiceYear = WebUtil.getInvoiceYear(txt_InvoiceNoFrom.Text.Trim());
            }

            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;

            if (rad_InvoiceDate.Checked && txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            DateTime purchaseExtractDateFrom = DateTime.MinValue;
            DateTime purchaseExtractDateTo = DateTime.MinValue;
            if (txt_PurchaseExtractDateFrom.Text.Trim() != "")
            {
                purchaseExtractDateFrom = DateTimeUtility.getDate(txt_PurchaseExtractDateFrom.Text.Trim());
                if (txt_PurchaseExtractDateTo.Text.Trim() == "")
                    txt_PurchaseExtractDateTo.Text = txt_PurchaseExtractDateFrom.Text;
                purchaseExtractDateTo = DateTimeUtility.getDate(txt_PurchaseExtractDateTo.Text.Trim());
            }

            DateTime departDateFrom = DateTime.MinValue;
            DateTime departDateTo = DateTime.MinValue;
            if (txt_DepartDateFrom.Text.Trim() != "")
            {
                departDateFrom = DateTimeUtility.getDate(txt_DepartDateFrom.Text.Trim());
                if (txt_DepartDateTo.Text.Trim() == "")
                    departDateTo = departDateFrom;
                else
                    departDateTo = DateTimeUtility.getDate(txt_DepartDateTo.Text.Trim());
            }

            bool isNextMfgUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.NMLAccountsReport);

            int vendorId = -1;
            if (isNextMfgUser)
            {
                vendorId = 3154;
            }
            else
            {
                if (txt_Supplier.VendorId != int.MinValue)
                {
                    vendorId = txt_Supplier.VendorId;
                }
            }

            int purchaseTermId = Convert.ToInt32(ddl_TermOfPurchase.SelectedValue);

            TypeCollector productTeamList = uclOfficeProductTeamSelection.getProductCodeList();
            TypeCollector officeIdList = uclOfficeProductTeamSelection.getOfficeList();

            if (!officeIdList.contains(OfficeId.DG.Id))
                handlingOfficeId = -1;

            int seasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            int customerDestination = Convert.ToInt32(ddl_CustomerDestination.SelectedValue);
            int oprFabricType = Convert.ToInt32(ddl_OPRType.SelectedValue);

            int budgetYear = -1;
            int periodFrom = -1;
            int periodTo = -1;

            if (rad_FiscalPeriod.Checked)
            {
                budgetYear = Convert.ToInt32(ddl_Year.SelectedValue);
                periodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
                periodTo = Convert.ToInt32(ddl_PeriodTo.SelectedValue);
            }

            TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Customer.Items)
            {
                if (item.Selected)
                    customerTypeCollector.append(Convert.ToInt32(item.Value));
            }

            TypeCollector tradingAgencyCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_TradingAgency.Items)
            {
                if (item.Selected)
                    tradingAgencyCollector.append(Convert.ToInt32(item.Value));
            }

            TypeCollector shipmentMethodCollector = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_ShipmentMethod.Items)
            {
                if (item.Selected)
                    shipmentMethodCollector.append(Convert.ToInt32(item.Value));
            }

            int accountDocReceipt = int.Parse(ddl_AccDocReceipt.SelectedValue);


            InvoiceListReportDs ds = InvoiceReportManager.Instance.getInvoiceListDataSet(baseCurrencyId,
                invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, invoiceDateFrom,
                invoiceDateTo, purchaseExtractDateFrom, purchaseExtractDateTo,
                budgetYear, periodFrom, periodTo, departDateFrom, departDateTo, vendorId, purchaseTermId, officeIdList, handlingOfficeId, seasonId, productTeamList,
                customerDestination, oprFabricType, isSZOrder, isUTOrder, isLDPOrder, isSampleOrder,
                customerTypeCollector, tradingAgencyCollector, shipmentMethodCollector, accountDocReceipt, txt_VoyageNo.Text.Trim(),
                ddl_PaymentTerm.selectedValueToInt, ddl_PaymentStatus.selectedValueToInt, ddl_LCPaymentChecked.selectedValueToInt, ddl_ShippingDocReceiptDate.selectedValueToInt,
                this.LogonUserId, ucl_SortingOrder1.SortingField, rad_Version.SelectedValue);

            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(this.LogonUserId).DisplayName, CellValues.String);
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.Date);

            int[] list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 3, 32);
            int qtyTotalStyleId = OpenXmlUtil.getCellStyleId(document, templateWorksheetID, 4, 13);
            int amtTotalStyleId = OpenXmlUtil.getCellStyleId(document, templateWorksheetID, 4, 16);

            int startingRow = 23;
            foreach (InvoiceListReportDs.InvoiceListReportRow r in ds.InvoiceListReport.Rows)
            {
                List<OpenXmlCell> cellValueList = new List<OpenXmlCell>();
                cellValueList.Add(new OpenXmlCell(startingRow, 1, CellValues.SharedString, list[0], r.InvoiceNo));
                cellValueList.Add(new OpenXmlCell(startingRow, 2, CellValues.SharedString, list[1], DateTimeUtility.getDateString(r.InvoiceDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 3, CellValues.SharedString, list[2], r.Season));
                cellValueList.Add(new OpenXmlCell(startingRow, 4, CellValues.SharedString, list[3], r.ProductTeam));
                cellValueList.Add(new OpenXmlCell(startingRow, 5, CellValues.SharedString, list[4], r.Vendor));
                cellValueList.Add(new OpenXmlCell(startingRow, 6, CellValues.SharedString, list[5], r.ItemNo));
                cellValueList.Add(new OpenXmlCell(startingRow, 7, CellValues.SharedString, list[6], r.ContractNo));
                cellValueList.Add(new OpenXmlCell(startingRow, 8, CellValues.Number, list[7], r.DeliveryNo.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 9, CellValues.Number, list[8], (r.IsSequenceNoNull() || r.SequenceNo == 0) ? string.Empty : r.SequenceNo.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 10, CellValues.SharedString, list[9], r.CustomerDestination));
                cellValueList.Add(new OpenXmlCell(startingRow, 11, CellValues.SharedString, list[10], r.TermOfPurchase));
                cellValueList.Add(new OpenXmlCell(startingRow, 12, CellValues.SharedString, list[11], r.LoadingPort));
                cellValueList.Add(new OpenXmlCell(startingRow, 13, CellValues.Number, list[12], r.TotalShippedQty.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 14, CellValues.SharedString, list[13], r.SellCurrency));
                cellValueList.Add(new OpenXmlCell(startingRow, 15, CellValues.Number, list[14], r.TotalShippedAmt.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 16, CellValues.Number, list[15], r.TotalShippedAmtUSD.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 17, CellValues.Number, list[16], r.TotalShippedNetFOBAmtAfterDiscount.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 18, CellValues.Number, list[17], r.TotalShippedNetFOBAmtAfterDiscountUSD.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 19, CellValues.SharedString, list[18], r.IsSupplierInvoiceNoNull() ? string.Empty : r.SupplierInvoiceNo));
                cellValueList.Add(new OpenXmlCell(startingRow, 20, CellValues.Number, list[19], r.TotalShippedSupplierGmtAmtAfterDiscount.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 21, CellValues.Number, list[20], r.TotalShippedSupplierGmtAmtAfterDiscountUSD.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 22, CellValues.Date, list[21], r.IsLCPaymentCheckedDateNull() ? string.Empty : DateTimeUtility.getDateString(r.LCPaymentCheckedDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 23, CellValues.SharedString, list[22], r.ShipmentMethod));
                cellValueList.Add(new OpenXmlCell(startingRow, 24, CellValues.SharedString, list[23], r.CO));
                cellValueList.Add(new OpenXmlCell(startingRow, 25, CellValues.Date, list[24], r.IsDepartDateNull() ? string.Empty : DateTimeUtility.getDateString(r.DepartDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 26, CellValues.Date, list[25], r.IsAPDateNull() ? string.Empty : DateTimeUtility.getDateString(r.APDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 27, CellValues.SharedString, list[26], r.PaymentTermId == 1 ? "OA" : "LC"));
                cellValueList.Add(new OpenXmlCell(startingRow, 28, CellValues.SharedString, list[27], r.IsLCNoNull() ? string.Empty : r.LCNo));
                cellValueList.Add(new OpenXmlCell(startingRow, 29, CellValues.Date, list[28], r.IsLCIssueDateNull() ? string.Empty : DateTimeUtility.getDateString(r.LCIssueDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 30, CellValues.Date, list[29], r.IsLCExpiryDateNull() ? string.Empty : DateTimeUtility.getDateString(r.LCExpiryDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 31, CellValues.Number, list[30], r.QAAmt.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 32, CellValues.Number, list[31], r.CommSvc.ToString()));

                OpenXmlUtil.createRowAndCells(document, worksheetID, startingRow, 1, cellValueList);

                startingRow += 1;
                System.Diagnostics.Debug.Print("starting row : " + startingRow.ToString());

            }
            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 13, "=SUM(" + OpenXmlUtil.getColumnLetter(13) + "23:" + OpenXmlUtil.getColumnLetter(13) + (startingRow - 1).ToString() + ")", CellValues.String, qtyTotalStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 16, "=SUM(" + OpenXmlUtil.getColumnLetter(16) + "23:" + OpenXmlUtil.getColumnLetter(16) + (startingRow - 1).ToString() + ")", CellValues.String, amtTotalStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 18, "=SUM(" + OpenXmlUtil.getColumnLetter(18) + "23:" + OpenXmlUtil.getColumnLetter(18) + (startingRow - 1).ToString() + ")", CellValues.String, amtTotalStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 21, "=SUM(" + OpenXmlUtil.getColumnLetter(21) + "23:" + OpenXmlUtil.getColumnLetter(21) + (startingRow - 1).ToString() + ")", CellValues.String, amtTotalStyleId);


            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 8, worksheetID, startingRow + 2);
            OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(1, startingRow + 2), OpenXmlUtil.getCellReference(32, startingRow + 2));

            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }


    }
}
