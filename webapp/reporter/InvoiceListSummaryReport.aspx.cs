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
using com.next.isam.reporter.dataserver;
using System.Globalization;
using System.Data.OleDb;
using System.Data;
using System.Text;

namespace com.next.isam.webapp.reporter
{
    public partial class InvoiceListSummaryReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private ArrayList vwLCNumberList
        {
            set
            {
                ViewState["LCNumberList"] = value;
            }
            get
            {
                return (ArrayList)ViewState["LCNumberList"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.btn_Export);
            if (!Page.IsPostBack)
            {
                initControl();
            }
        }

        void initControl()
        {
            this.txt_InvoiceDateFrom.Text = "01/12/2019";
            this.txt_InvoiceDateTo.Text = DateTimeUtility.getDateString(DateTime.Today);
            //this.ddl_OPRType.bindList((ArrayList)OPRFabricType.getCollectionValueForReport(), "OPRFabricReportSelectionName", "OPRFabricTypeId");
            //ddl_TermOfPurchase.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());

            //ddl_CustomerDestination.bindList(WebUtil.getCustomerDestinationList(), "DestinationDesc", "CustomerDestinationId", "", "--All--", GeneralCriteria.ALL.ToString());
            //ddl_Season.bindList(CommonUtil.getSeasonList(), "Code", "SeasonId", "", "--All--", GeneralCriteria.ALL.ToString());
            //ddl_BaseCurrency.bindList(WebUtil.getCurrencyListForExchangeRate(), "CurrencyCode", "CurrencyId", "3");

            //ArrayList arr_Year = WebUtil.getBudgetYearList();

            //AccountFinancialCalenderDef finCalDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Now.AddDays(-10));
            //ddl_Year.DataSource = arr_Year;
            //ddl_Year.DataBind();
            //ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();
            //ddl_PeriodFrom.SelectedValue = finCalDef.Period.ToString();
            //ddl_PeriodTo.SelectedValue = finCalDef.Period.ToString();


            //uclOfficeProductTeamSelection.UserId = this.LogonUserId;

            //txt_Supplier.setWidth(305);
            //txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            //cbl_Customer.DataSource = WebUtil.getCustomerList();
            //cbl_Customer.DataTextField = "CustomerCode";
            //cbl_Customer.DataValueField = "CustomerId";
            //cbl_Customer.DataBind();

            //cbl_TradingAgency.DataSource = WebUtil.getTradingAgencyList();
            //cbl_TradingAgency.DataTextField = "ShortName";
            //cbl_TradingAgency.DataValueField = "TradingAgencyId";
            //cbl_TradingAgency.DataBind();

            //cbl_ShipmentMethod.DataSource = WebUtil.getShipmentMethodList();
            //cbl_ShipmentMethod.DataTextField = "ShipmentMethodDescription";
            //cbl_ShipmentMethod.DataValueField = "ShipmentMethodId";
            //cbl_ShipmentMethod.DataBind();

            //foreach (ListItem item in cbl_Customer.Items)
            //{
            //    if (WebUtil.getCustomerByKey(int.Parse(item.Value)).IsPaymentRequired)
            //        item.Selected = true;
            //}
            //foreach (ListItem item in cbl_TradingAgency.Items)
            //{
            //    item.Selected = true;
            //}
            //foreach (ListItem item in cbl_ShipmentMethod.Items)
            //{
            //    item.Selected = true;
            //}
        }

        protected void btn_upload(object sender, EventArgs e)
        {
            if (uplFile.HasFile)
            {
                txt_lcNumber.Text = string.Empty;
                string path = Path.GetFileName(uplFile.FileName);

                path = path.Replace(" ", "");
                uplFile.SaveAs(WebConfig.getValue("appSettings", "UT_DN_FOLDER") + path);
                String ExcelPath = WebConfig.getValue("appSettings", "UT_DN_FOLDER") + path;
                OleDbConnection mycon = new OleDbConnection("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = " + ExcelPath + "; Extended Properties=Excel 8.0; Persist Security Info = False");
                mycon.Open();

                DataTable dtExcelSchema;
                dtExcelSchema = mycon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                OleDbCommand cmd = new OleDbCommand("select * from [" + SheetName + "]", mycon);
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "L/C Bill Ref No.", "alert('No L/C Bill Ref No. found');", true);
                    return;
                }

                this.vwLCNumberList = new ArrayList();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(ds.Tables[0].Rows[i][3].ToString()) || ds.Tables[0].Rows[i][3].ToString() == "Bill Under Process"
                        || ds.Tables[0].Rows[i][3].ToString() == "LC Number" || ds.Tables[0].Rows[i][3].ToString() == "Bill Number Under Process")
                        continue;

                    this.vwLCNumberList.Add(ds.Tables[0].Rows[i][3].ToString());
                }

                mycon.Close();

                if (this.vwLCNumberList.Count > 0)
                {
                    StringBuilder lcNumberList = new StringBuilder();
                    for (int i = 0; i < vwLCNumberList.Count; i++)
                    {
                        lcNumberList.Append(vwLCNumberList[i]);

                        if (i + 1 < vwLCNumberList.Count)
                            lcNumberList.Append(", ");
                    }

                    updatePanel3.Visible = true;
                    txt_lcNumber.Text = lcNumberList.ToString();
                }
                else
                {
                    updatePanel3.Visible = false;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "L/C Bill Ref No.", "alert('No L/C Bill Ref No. found');", true);
                    return;
                }
            }
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            genReport();
        }

        private void genReport()
        {
            //string invoicePrefix = "";
            //int invoiceSeqFrom = 0;
            //int invoiceSeqTo = 0;
            //int invoiceYear = 0;
            //int isSZOrder = Convert.ToInt32(ddl_SZOrder.SelectedValue);
            //int isUTOrder = Convert.ToInt32(ddl_UTOrder.SelectedValue);
            //int baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            //int isLDPOrder = Convert.ToInt32(ddl_LDPOrder.SelectedValue);
            //int isSampleOrder = Convert.ToInt32(ddl_SampleOrder.SelectedValue);
            //int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);

            //if (txt_InvoiceNoFrom.Text.Trim() != "")
            //{
            //    invoicePrefix = WebUtil.getInvoicePrefix(txt_InvoiceNoFrom.Text.Trim());
            //    invoiceSeqFrom = WebUtil.getInvoiceSeq(txt_InvoiceNoFrom.Text.Trim());
            //    invoiceSeqTo = WebUtil.getInvoiceSeq(txt_InvoiceNoTo.Text.Trim());
            //    invoiceYear = WebUtil.getInvoiceYear(txt_InvoiceNoFrom.Text.Trim());
            //}

            DateTime invoiceDateFrom = DateTime.MinValue;
            DateTime invoiceDateTo = DateTime.MinValue;

            if (txt_InvoiceDateFrom.Text.Trim() != "")
            {
                invoiceDateFrom = DateTimeUtility.getDate(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == "")
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = DateTimeUtility.getDate(txt_InvoiceDateTo.Text.Trim());
            }

            //DateTime purchaseExtractDateFrom = DateTime.MinValue;
            //DateTime purchaseExtractDateTo = DateTime.MinValue;
            //if (txt_PurchaseExtractDateFrom.Text.Trim() != "")
            //{
            //    purchaseExtractDateFrom = DateTimeUtility.getDate(txt_PurchaseExtractDateFrom.Text.Trim());
            //    if (txt_PurchaseExtractDateTo.Text.Trim() == "")
            //        txt_PurchaseExtractDateTo.Text = txt_PurchaseExtractDateFrom.Text;
            //    purchaseExtractDateTo = DateTimeUtility.getDate(txt_PurchaseExtractDateTo.Text.Trim());
            //}

            //DateTime departDateFrom = DateTime.MinValue;
            //DateTime departDateTo = DateTime.MinValue;
            //if (txt_DepartDateFrom.Text.Trim() != "")
            //{
            //    departDateFrom = DateTimeUtility.getDate(txt_DepartDateFrom.Text.Trim());
            //    if (txt_DepartDateTo.Text.Trim() == "")
            //        departDateTo = departDateFrom;
            //    else
            //        departDateTo = DateTimeUtility.getDate(txt_DepartDateTo.Text.Trim());
            //}

            //bool isNextMfgUser = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.reports.Id, ISAMModule.reports.NMLAccountsReport);

            //int vendorId = -1;
            //if (isNextMfgUser)
            //{
            //    vendorId = 3154;
            //}
            //else
            //{
            //    if (txt_Supplier.VendorId != int.MinValue)
            //    {
            //        vendorId = txt_Supplier.VendorId;
            //    }
            //}

            //int purchaseTermId = Convert.ToInt32(ddl_TermOfPurchase.SelectedValue);

            //TypeCollector productTeamList = uclOfficeProductTeamSelection.getProductCodeList();
            //TypeCollector officeIdList = uclOfficeProductTeamSelection.getOfficeList();

            //if (!officeIdList.contains(OfficeId.DG.Id))
            //    handlingOfficeId = -1;

            //int seasonId = Convert.ToInt32(ddl_Season.SelectedValue);
            //int customerDestination = Convert.ToInt32(ddl_CustomerDestination.SelectedValue);
            //int oprFabricType = Convert.ToInt32(ddl_OPRType.SelectedValue);

            //int budgetYear = -1;
            //int periodFrom = -1;
            //int periodTo = -1;

            //if (rad_FiscalPeriod.Checked)
            //{
            //    budgetYear = Convert.ToInt32(ddl_Year.SelectedValue);
            //    periodFrom = Convert.ToInt32(ddl_PeriodFrom.SelectedValue);
            //    periodTo = Convert.ToInt32(ddl_PeriodTo.SelectedValue);
            //}

            //TypeCollector customerTypeCollector = TypeCollector.Inclusive;
            //foreach (ListItem item in cbl_Customer.Items)
            //{
            //    if (item.Selected)
            //        customerTypeCollector.append(Convert.ToInt32(item.Value));
            //}

            //TypeCollector tradingAgencyCollector = TypeCollector.Inclusive;
            //foreach (ListItem item in cbl_TradingAgency.Items)
            //{
            //    if (item.Selected)
            //        tradingAgencyCollector.append(Convert.ToInt32(item.Value));
            //}

            //TypeCollector shipmentMethodCollector = TypeCollector.Inclusive;
            //foreach (ListItem item in cbl_ShipmentMethod.Items)
            //{
            //    if (item.Selected)
            //        shipmentMethodCollector.append(Convert.ToInt32(item.Value));
            //}

            TypeCollector lcNumberList = TypeCollector.Inclusive;
            lcNumberList.append(vwLCNumberList);

            //StringBuilder lcNumberList = new StringBuilder();
            //for (int i = 0; i < vwLCNumberList.Count; i++)
            //{
            //    lcNumberList.Append("'" + vwLCNumberList[i] + "'");

            //    if (i + 1 < vwLCNumberList.Count)
            //        lcNumberList.Append(",");
            //}

            //int accountDocReceipt = int.Parse(ddl_AccDocReceipt.SelectedValue);

            InvoiceListSummaryReportDs ds = ReporterWorker.Instance.getInvoiceListSummaryByCriteria(lcNumberList, invoiceDateFrom, invoiceDateTo);

            this.outputExcel(ds, invoiceDateFrom, invoiceDateTo);
        }

        private void outputExcel(InvoiceListSummaryReportDs ds, DateTime invoiceDateFrom, DateTime invoiceDateTo)
        {
            string sourceFileDir = this.ApplPhysicalPath + @"report_template\";
            string sourceFileName = "InvoiceListSummary.xlsx";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(WebConfig.getValue("appSettings", "UPLOAD_AP_Folder") + @"InvoiceListSummary-{0}-{1}.xlsx", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string rawsheetId = string.Empty;
            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);
            int j = 2;
            rawsheetId = OpenXmlUtil.getWorksheetId(document, "Raw");
            worksheetID = OpenXmlUtil.getWorksheetId(document, "Summary");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Format");

            if (rawsheetId == string.Empty || worksheetID == string.Empty || templateWorksheetID == string.Empty)
                Console.WriteLine("failure open spreadsheet");

            // raw data sheet
            int[] row = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 20, 48);

            foreach (InvoiceListSummaryReportDs.InvoiceListSummaryReportRow r in ds.Tables[0].Rows)
            {
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 1, r.InvoiceNo, CellValues.String, row[0]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 2, DateTimeUtility.getDateString(r.InvoiceDate), CellValues.Date, row[1]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 3, r.Season, CellValues.String, row[2]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 4, r.ProdTeam, CellValues.String, row[3]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 5, r.SupplierName, CellValues.String, row[4]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 6, r.ItemNo, CellValues.Number, row[5]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 7, r.ContractNo, CellValues.String, row[6]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 8, r.DlyNo, CellValues.Number, row[7]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 9, r.SeqNo, CellValues.String, row[8]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 10, r.Destination, CellValues.String, row[9]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 11, r.PurchaseTerm, CellValues.String, row[10]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 12, !r.IsLoadingPortNull() ? r.LoadingPort : string.Empty, CellValues.String, row[11]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 13, r.ShippedQty.ToString(), CellValues.Number, row[12]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 14, r.Ccy, CellValues.String, row[13]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 15, r.NSLtoNUKInvAmt.ToString(), CellValues.Number, row[14]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 16, r.NSLtoNUKInvAmtUSD.ToString(), CellValues.Number, row[15]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 17, r.TtlFOBAmtForCMT.ToString(), CellValues.Number, row[16]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 18, r.TtlFOBAmtForCMTUSD.ToString(), CellValues.Number, row[17]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 19, r.SupInvNo, CellValues.String, row[18]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 20, r.SupInvAmt.ToString(), CellValues.Number, row[19]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 21, r.SupInvAmtUSD.ToString(), CellValues.Number, row[20]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 22, !r.IsLCPaymentCheckedDateNull() ? DateTimeUtility.getDateString(r.LCPaymentCheckedDate) : string.Empty, CellValues.Date, row[21]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 23, r.ShipmentMethod, CellValues.String, row[22]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 24, r.CO, CellValues.String, row[23]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 25, !r.IsDepartureDateNull() ? DateTimeUtility.getDateString(r.DepartureDate) : string.Empty, CellValues.Date, row[24]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 26, !r.IsAccountPayableDateNull() ? DateTimeUtility.getDateString(r.AccountPayableDate) : string.Empty, CellValues.Date, row[25]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 27, r.PaymentTerm, CellValues.String, row[26]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 28, !r.IsLCNoNull() ? r.LCNo : string.Empty, CellValues.String, row[27]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 29, !r.IsLCIssueDateNull() ? DateTimeUtility.getDateString(r.LCIssueDate) : string.Empty, CellValues.Date, row[28]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 30, !r.IsLCExpiryDateNull() ? DateTimeUtility.getDateString(r.LCExpiryDate) : string.Empty, CellValues.Date, row[29]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 31, !r.IsLCBillRefNoNull() ? r.LCBillRefNo : string.Empty, CellValues.String, row[30]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 32, r.QACommissionUSD.ToString(), CellValues.Number, row[31]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 33, r.UTCommAndServiceChargeUSD.ToString(), CellValues.Number, row[32]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 34, !r.IsShippingDocReceiptDateNull() ? DateTimeUtility.getDateString(r.ShippingDocReceiptDate) : string.Empty, CellValues.Date, row[33]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 35, !r.IsCostOfGoodSoldSettlementDateNull() ? DateTimeUtility.getDateString(r.CostOfGoodSoldSettlementDate) : string.Empty, CellValues.Date, row[34]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 36, r.InvoiceDateVSShippingDocReceiptDate.ToString(), CellValues.Number, row[35]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 37, r.ShippingDocReceiptDateVSLCCheckDate.ToString(), CellValues.Number, row[36]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 38, r.LCCheckDateVSSettlementDate.ToString(), CellValues.Number, row[37]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 39, r.InvoiceDateVSReportDate.ToString(), CellValues.Number, row[38]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 40, r.AnyManifest, CellValues.String, row[39]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 41, r.IsPaid, CellValues.String, row[40]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 42, !r.IsLCAdditionalRemarkNull() ? r.LCAdditionalRemark : string.Empty, CellValues.String, row[41]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 43, r.InvoiceDate.Year.ToString(), CellValues.Number, row[42]);
                OpenXmlUtil.setCellValue(document, rawsheetId, j, 44, r.InvoiceDate.Month.ToString(), CellValues.Number, row[43]);

                if (r.InvoiceDateVSShippingDocReceiptDate > 50)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 45, "> 50", CellValues.String, row[44]);
                else if (r.InvoiceDateVSShippingDocReceiptDate > 40)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 45, "41 - 50", CellValues.String, row[44]);
                else if (r.InvoiceDateVSShippingDocReceiptDate > 30)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 45, "31 - 40", CellValues.String, row[44]);
                else if (r.InvoiceDateVSShippingDocReceiptDate > 20)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 45, "21 - 30", CellValues.String, row[44]);
                else if (r.InvoiceDateVSShippingDocReceiptDate > 10)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 45, "11 - 20", CellValues.String, row[44]);
                else if (r.InvoiceDateVSShippingDocReceiptDate > 0)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 45, "1 - 10", CellValues.String, row[44]);
                else
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 45, "0", CellValues.String, row[44]);

                if (r.ShippingDocReceiptDateVSLCCheckDate > 50)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 46, "> 50", CellValues.String, row[45]);
                else if (r.ShippingDocReceiptDateVSLCCheckDate > 40)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 46, "41 - 50", CellValues.String, row[45]);
                else if (r.ShippingDocReceiptDateVSLCCheckDate > 30)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 46, "31 - 40", CellValues.String, row[45]);
                else if (r.ShippingDocReceiptDateVSLCCheckDate > 20)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 46, "21 - 30", CellValues.String, row[45]);
                else if (r.ShippingDocReceiptDateVSLCCheckDate > 10)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 46, "11 - 20", CellValues.String, row[45]);
                else if (r.ShippingDocReceiptDateVSLCCheckDate > 0)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 46, "1 - 10", CellValues.String, row[45]);
                else
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 46, "0", CellValues.String, row[45]);

                if (r.LCCheckDateVSSettlementDate > 50)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 47, "> 50", CellValues.String, row[46]);
                else if (r.LCCheckDateVSSettlementDate > 40)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 47, "41 - 50", CellValues.String, row[46]);
                else if (r.LCCheckDateVSSettlementDate > 30)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 47, "31 - 40", CellValues.String, row[46]);
                else if (r.LCCheckDateVSSettlementDate > 20)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 47, "21 - 30", CellValues.String, row[46]);
                else if (r.LCCheckDateVSSettlementDate > 10)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 47, "11 - 20", CellValues.String, row[46]);
                else if (r.LCCheckDateVSSettlementDate > 0)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 47, "1 - 10", CellValues.String, row[46]);
                else
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 47, "0", CellValues.String, row[46]);

                if (r.InvoiceDateVSReportDate > 100)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 48, "> 100", CellValues.String, row[47]);
                else if (r.InvoiceDateVSReportDate > 80)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 48, "81 - 100", CellValues.String, row[47]);
                else if (r.InvoiceDateVSReportDate > 60)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 48, "61 - 80", CellValues.String, row[47]);
                else if (r.InvoiceDateVSReportDate > 40)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 48, "41 - 60", CellValues.String, row[47]);
                else if (r.InvoiceDateVSReportDate > 20)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 48, "21 - 40", CellValues.String, row[47]);
                else if (r.InvoiceDateVSReportDate > 0)
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 48, "1 - 20", CellValues.String, row[47]);
                else
                    OpenXmlUtil.setCellValue(document, rawsheetId, j, 48, "0", CellValues.String, row[47]);

                j++;
            }


            // summary sheet
            int[] titleRow = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 2, 1);
            string title = "LC and Open Account Payment Position as at " + DateTimeUtility.getDateString(invoiceDateTo) + " (Paid & Not yet Paid)";
            OpenXmlUtil.setCellValue(document, worksheetID, 2, 1, title, CellValues.String, titleRow[0]);

            int[] headerRow = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 4, 13);
            int[] row1 = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 5, 13);
            int[] row2 = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 6, 13);
            int[] row3 = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 7, 13);
            int[] row4 = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 8, 13);
            int[] row5 = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 9, 13);
            int[] row6 = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 10, 13);
            int[] row7 = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 11, 13);
            int[] rowTotal = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 12, 13);

            int i = 5;
            int rowCount = 1;
            DateTime weekRangeFrom = invoiceDateTo.AddDays(-7);
            DateTime weekRangeTo = invoiceDateTo;

            string header = "no. of invoice \"presented to bank\" during " + DateTimeUtility.getDateString(weekRangeFrom) + "-" + DateTimeUtility.getDateString(weekRangeTo) + ")";
            OpenXmlUtil.setCellValue(document, worksheetID, 4, 13, header, CellValues.String, headerRow[12]);

            DateTime nextInvoiceDate = DateTime.MinValue;
            string nextShipMode = string.Empty;
            bool sameYearMonth = false;

            int noShippingDocReceiptDate = 0;
            int noShipping1to20Count = 0;
            int noShipping21to40Count = 0;
            int noShipping41to60Count = 0;
            int noShipping61to80Count = 0;
            int noShipping81to100Count = 0;
            int noShippingOver100Count = 0;
            int noShippingSubTotal = 0;
            int noShippingTotal = 0;

            int invDateToDocReceiptDate = 0;
            int invDate1to10Count = 0;
            int invDate11to20Count = 0;
            int invDate21to30Count = 0;
            int invDate31to40Count = 0;
            int invDate41to50Count = 0;
            int invDateOver50Count = 0;
            int invDateSubTotal = 0;
            int invDateTotal = 0;

            int bankDocToHKFromDocReceipt = 0;
            int bankDocToHK1to10Count = 0;
            int bankDocToHK11to20Count = 0;
            int bankDocToHK21to30Count = 0;
            int bankDocToHK31to40Count = 0;
            int bankDocToHK41to50Count = 0;
            int bankDocToHKOver50Count = 0;
            int bankDocToHKSubTotal = 0;
            int bankDocToHKTotal = 0;

            int bankDocReceiptByHKToPayment = 0;
            int bankDocReceiptByHK1to10Count = 0;
            int bankDocReceiptByHK11to20Count = 0;
            int bankDocReceiptByHK21to30Count = 0;
            int bankDocReceiptByHK31to40Count = 0;
            int bankDocReceiptByHK41to50Count = 0;
            int bankDocReceiptByHKOver50Count = 0;
            int bankDocReceiptByHKSubTotal = 0;
            int bankDocReceiptByHKTotal = 0;

            int paidCount = 0;
            int notYetPaidCount = 0;
            int presentToBankCount = 0;

            foreach (InvoiceListSummaryReportDs.InvoiceListSummaryReportRow r in ds.Tables[0].Rows)
            {
                noShippingDocReceiptDate = (invoiceDateTo - r.InvoiceDate).Days; // No Shipping Doc Receipt Date (No of invoice)
                if (noShippingDocReceiptDate > 0 && r.IsShippingDocReceiptDateNull())
                {
                    if (noShippingDocReceiptDate > 100)
                        noShippingOver100Count++;
                    else if (noShippingDocReceiptDate > 80)
                        noShipping81to100Count++;
                    else if (noShippingDocReceiptDate > 60)
                        noShipping61to80Count++;
                    else if (noShippingDocReceiptDate > 40)
                        noShipping41to60Count++;
                    else if (noShippingDocReceiptDate > 20)
                        noShipping21to40Count++;
                    else if (noShippingDocReceiptDate > 0)
                        noShipping1to20Count++;
                }

                if (!r.IsShippingDocReceiptDateNull()) // invoice date to doc receipt date by BD (No of invoice)
                {
                    invDateToDocReceiptDate = (r.ShippingDocReceiptDate - r.InvoiceDate).Days;
                    if (invDateToDocReceiptDate > 0 && r.IsPaid == "N")
                    {
                        if (invDateToDocReceiptDate > 50)
                            invDateOver50Count++;
                        else if (invDateToDocReceiptDate > 40)
                            invDate41to50Count++;
                        else if (invDateToDocReceiptDate > 30)
                            invDate31to40Count++;
                        else if (invDateToDocReceiptDate > 20)
                            invDate21to30Count++;
                        else if (invDateToDocReceiptDate > 10)
                            invDate11to20Count++;
                        else if (invDateToDocReceiptDate > 0)
                            invDate1to10Count++;
                    }
                }

                if (!r.IsLCPaymentCheckedDateNull() && !r.IsShippingDocReceiptDateNull()) // Bank doc to HK from doc receipt date by BD (No of invoice)
                {
                    bankDocToHKFromDocReceipt = (r.LCPaymentCheckedDate - r.ShippingDocReceiptDate).Days;
                    if (bankDocToHKFromDocReceipt > 0 && r.IsPaid == "N")
                    {
                        if (bankDocToHKFromDocReceipt > 50)
                            bankDocToHKOver50Count++;
                        else if (bankDocToHKFromDocReceipt > 40)
                            bankDocToHK41to50Count++;
                        else if (bankDocToHKFromDocReceipt > 30)
                            bankDocToHK31to40Count++;
                        else if (bankDocToHKFromDocReceipt > 20)
                            bankDocToHK21to30Count++;
                        else if (bankDocToHKFromDocReceipt > 10)
                            bankDocToHK11to20Count++;
                        else if (bankDocToHKFromDocReceipt > 0)
                            bankDocToHK1to10Count++;
                    }
                }

                if (!r.IsAccountPayableDateNull() && !r.IsLCPaymentCheckedDateNull()) // Bank doc receipt by HK to payment (No of invoice)
                {
                    bankDocReceiptByHKToPayment = (r.AccountPayableDate - r.LCPaymentCheckedDate).Days;
                    if (bankDocReceiptByHKToPayment > 0 && r.IsPaid == "Y" && r.AccountPayableDate >= weekRangeFrom && r.AccountPayableDate <= weekRangeTo)
                    {
                        if (bankDocReceiptByHKToPayment > 50)
                            bankDocReceiptByHKOver50Count++;
                        else if (bankDocReceiptByHKToPayment > 40)
                            bankDocReceiptByHK41to50Count++;
                        else if (bankDocReceiptByHKToPayment > 30)
                            bankDocReceiptByHK31to40Count++;
                        else if (bankDocReceiptByHKToPayment > 20)
                            bankDocReceiptByHK21to30Count++;
                        else if (bankDocReceiptByHKToPayment > 10)
                            bankDocReceiptByHK11to20Count++;
                        else if (bankDocReceiptByHKToPayment > 0)
                            bankDocReceiptByHK1to10Count++;
                    }
                }

                if (r.IsPaid == "Y" && r.AccountPayableDate >= weekRangeFrom && r.AccountPayableDate <= weekRangeTo)
                {
                    paidCount++;
                }

                if (r.IsPaid == "N" && string.IsNullOrEmpty(r.LCAdditionalRemark))
                {
                    notYetPaidCount++;
                }

                if (r.IsPaid == "N" && r.LCAdditionalRemark == "L/C Presented to Bank")
                {
                    presentToBankCount++;
                }


                if (rowCount < ds.Tables[0].Rows.Count)
                {
                    nextInvoiceDate = DateTime.Parse(ds.Tables[0].Rows[rowCount]["InvoiceDate"].ToString());
                    nextShipMode = ds.Tables[0].Rows[rowCount]["ShipmentMethod"].ToString();
                }

                if (r.InvoiceDate.Year != nextInvoiceDate.Year || r.InvoiceDate.Month != nextInvoiceDate.Month || r.ShipmentMethod != nextShipMode || rowCount == ds.Tables[0].Rows.Count)
                {
                    noShippingSubTotal = noShipping1to20Count + noShipping21to40Count + noShipping41to60Count + noShipping61to80Count + noShipping81to100Count + noShippingOver100Count;
                    noShippingTotal += noShippingSubTotal;

                    invDateSubTotal = invDate1to10Count + invDate11to20Count + invDate21to30Count + invDate31to40Count + invDate41to50Count + invDateOver50Count;
                    invDateTotal += invDateSubTotal;

                    bankDocToHKSubTotal = bankDocToHK1to10Count + bankDocToHK11to20Count + bankDocToHK21to30Count + bankDocToHK31to40Count + bankDocToHK41to50Count + bankDocToHKOver50Count;
                    bankDocToHKTotal += bankDocToHKSubTotal;

                    bankDocReceiptByHKSubTotal = bankDocReceiptByHK1to10Count + bankDocReceiptByHK11to20Count + bankDocReceiptByHK21to30Count + bankDocReceiptByHK31to40Count + bankDocReceiptByHK41to50Count + bankDocReceiptByHKOver50Count;
                    bankDocReceiptByHKTotal += bankDocReceiptByHKSubTotal;

                    if (!sameYearMonth)
                    {
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 1, r.InvoiceDate.Year.ToString(), CellValues.Number, row1[0]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 2, r.InvoiceDate.ToString("MMM", CultureInfo.InvariantCulture), CellValues.String, row1[1]);
                    }
                    else
                    {
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 1, string.Empty, CellValues.String, row1[0]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 2, string.Empty, CellValues.String, row1[1]);
                    }

                    if (r.InvoiceDate.Year == nextInvoiceDate.Year && r.InvoiceDate.Month == nextInvoiceDate.Month) // identify next Year Month set
                        sameYearMonth = true;
                    else
                        sameYearMonth = false;

                    OpenXmlUtil.setCellValue(document, worksheetID, i, 3, r.ShipmentMethod, CellValues.String, row1[2]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 4, noShipping1to20Count.ToString(), CellValues.Number, row1[3]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 5, "1 - 20", CellValues.String, row1[4]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 6, invDate1to10Count.ToString(), CellValues.Number, row1[5]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 7, "1 - 10", CellValues.String, row1[6]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 8, bankDocToHK1to10Count.ToString(), CellValues.Number, row1[7]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 9, "1 - 10", CellValues.String, row1[8]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 10, bankDocReceiptByHK1to10Count.ToString(), CellValues.Number, row1[9]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 11, "1 - 10", CellValues.String, row1[10]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, "Paid", CellValues.String, row1[11]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 13, paidCount.ToString(), CellValues.Number, row1[12]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 4, noShipping21to40Count.ToString(), CellValues.Number, row2[3]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 5, "21 - 40", CellValues.String, row2[4]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 6, invDate11to20Count.ToString(), CellValues.Number, row2[5]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 7, "11 - 20", CellValues.String, row2[6]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 8, bankDocToHK11to20Count.ToString(), CellValues.Number, row2[7]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 9, "11 - 20", CellValues.String, row2[8]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 10, bankDocReceiptByHK11to20Count.ToString(), CellValues.Number, row2[9]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 11, "11 - 20", CellValues.String, row2[10]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, "Not Yet Paid", CellValues.String, row2[11]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 13, notYetPaidCount.ToString(), CellValues.Number, row2[12]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 4, noShipping41to60Count.ToString(), CellValues.Number, row3[3]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 5, "41 - 60", CellValues.String, row3[4]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 6, invDate21to30Count.ToString(), CellValues.Number, row3[5]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 7, "21 - 30", CellValues.String, row3[6]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 8, bankDocToHK21to30Count.ToString(), CellValues.Number, row3[7]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 9, "21 - 30", CellValues.String, row3[8]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 10, bankDocReceiptByHK21to30Count.ToString(), CellValues.Number, row3[9]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 11, "21 - 30", CellValues.String, row3[10]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, "Not Yet Paid (Presented To Bank)", CellValues.String, row3[11]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 13, presentToBankCount.ToString(), CellValues.Number, row3[12]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 4, noShipping61to80Count.ToString(), CellValues.Number, row4[3]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 5, "61 - 80", CellValues.String, row4[4]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 6, invDate31to40Count.ToString(), CellValues.Number, row4[5]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 7, "31 - 40", CellValues.String, row4[6]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 8, bankDocToHK31to40Count.ToString(), CellValues.Number, row4[7]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 9, "31 - 40", CellValues.String, row4[8]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 10, bankDocReceiptByHK31to40Count.ToString(), CellValues.Number, row4[9]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 11, "31 - 40", CellValues.String, row4[10]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, string.Empty, CellValues.String, row4[11]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 13, string.Empty, CellValues.String, row4[12]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 4, noShipping81to100Count.ToString(), CellValues.Number, row5[3]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 5, "81 - 100", CellValues.String, row5[4]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 6, invDate41to50Count.ToString(), CellValues.Number, row5[5]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 7, "41 - 50", CellValues.String, row5[6]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 8, bankDocToHK41to50Count.ToString(), CellValues.Number, row5[7]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 9, "41 - 50", CellValues.String, row5[8]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 10, bankDocReceiptByHK41to50Count.ToString(), CellValues.Number, row5[9]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 11, "41 - 50", CellValues.String, row5[10]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, string.Empty, CellValues.String, row5[11]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 13, string.Empty, CellValues.String, row5[12]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 4, noShippingOver100Count.ToString(), CellValues.Number, row6[3]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 5, "> 100", CellValues.String, row6[4]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 6, invDateOver50Count.ToString(), CellValues.Number, row6[5]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 7, "> 50", CellValues.String, row6[6]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 8, bankDocToHKOver50Count.ToString(), CellValues.Number, row6[7]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 9, "> 50", CellValues.String, row6[8]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 10, bankDocReceiptByHKOver50Count.ToString(), CellValues.Number, row6[9]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 11, "> 50", CellValues.String, row6[10]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, string.Empty, CellValues.String, row6[11]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 13, string.Empty, CellValues.String, row6[12]);
                    i++;
                    if (sameYearMonth) // next set is same year & month --> no under border needed
                    {
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 1, string.Empty, CellValues.String, row7[0]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 2, string.Empty, CellValues.String, row7[1]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 3, "Sub-total", CellValues.String, row7[2]);
                    }
                    else
                    {
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 1, string.Empty, CellValues.String, rowTotal[1]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 2, string.Empty, CellValues.String, rowTotal[1]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 3, "Sub-total", CellValues.String, rowTotal[1]);
                    }
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 4, noShippingSubTotal.ToString(), CellValues.Number, row7[3]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 5, string.Empty, CellValues.String, row7[4]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 6, invDateSubTotal.ToString(), CellValues.Number, row7[5]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 7, string.Empty, CellValues.String, row7[6]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 8, bankDocToHKSubTotal.ToString(), CellValues.Number, row7[7]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 9, string.Empty, CellValues.String, row7[8]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 10, bankDocReceiptByHKSubTotal.ToString(), CellValues.Number, row7[9]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 11, string.Empty, CellValues.String, row7[10]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, string.Empty, CellValues.String, row7[11]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 13, string.Empty, CellValues.String, row7[12]);
                    i++;

                    if (rowCount == ds.Tables[0].Rows.Count) // last row
                    {
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 1, string.Empty, CellValues.String, rowTotal[0]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 2, string.Empty, CellValues.String, rowTotal[1]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 3, "Total", CellValues.String, rowTotal[2]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 4, noShippingTotal.ToString(), CellValues.Number, rowTotal[3]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 5, string.Empty, CellValues.String, rowTotal[4]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 6, invDateTotal.ToString(), CellValues.Number, rowTotal[5]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 7, string.Empty, CellValues.String, rowTotal[6]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 8, bankDocToHKTotal.ToString(), CellValues.Number, rowTotal[7]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 9, string.Empty, CellValues.String, rowTotal[8]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 10, bankDocReceiptByHKTotal.ToString(), CellValues.Number, rowTotal[9]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 11, string.Empty, CellValues.String, rowTotal[10]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 12, string.Empty, CellValues.String, rowTotal[11]);
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 13, string.Empty, CellValues.String, rowTotal[12]);
                    }

                    noShippingDocReceiptDate = 0;
                    noShipping1to20Count = 0;
                    noShipping21to40Count = 0;
                    noShipping41to60Count = 0;
                    noShipping61to80Count = 0;
                    noShipping81to100Count = 0;
                    noShippingOver100Count = 0;
                    noShippingSubTotal = 0;

                    invDateToDocReceiptDate = 0;
                    invDate1to10Count = 0;
                    invDate11to20Count = 0;
                    invDate21to30Count = 0;
                    invDate31to40Count = 0;
                    invDate41to50Count = 0;
                    invDateOver50Count = 0;
                    invDateSubTotal = 0;

                    bankDocToHKFromDocReceipt = 0;
                    bankDocToHK1to10Count = 0;
                    bankDocToHK11to20Count = 0;
                    bankDocToHK21to30Count = 0;
                    bankDocToHK31to40Count = 0;
                    bankDocToHK41to50Count = 0;
                    bankDocToHKOver50Count = 0;
                    bankDocToHKSubTotal = 0;

                    bankDocReceiptByHKToPayment = 0;
                    bankDocReceiptByHK1to10Count = 0;
                    bankDocReceiptByHK11to20Count = 0;
                    bankDocReceiptByHK21to30Count = 0;
                    bankDocReceiptByHK31to40Count = 0;
                    bankDocReceiptByHK41to50Count = 0;
                    bankDocReceiptByHKOver50Count = 0;
                    bankDocReceiptByHKSubTotal = 0;

                    paidCount = 0;
                    notYetPaidCount = 0;
                    presentToBankCount = 0;
                }
                rowCount++;
            }

            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();
            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }


        //protected void rad_FiscalPeriod_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (rad_FiscalPeriod.Checked)
        //    {
        //        txt_InvoiceDateFrom.Enabled = false;
        //        txt_InvoiceDateTo.Enabled = false;

        //        ddl_Year.Enabled = true;
        //        ddl_PeriodFrom.Enabled = true;
        //        ddl_PeriodTo.Enabled = true;
        //    }
        //}

        //protected void rad_InvoiceDate_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (rad_InvoiceDate.Checked)
        //    {
        //        txt_InvoiceDateFrom.Enabled = true;
        //        txt_InvoiceDateTo.Enabled = true;

        //        ddl_Year.Enabled = false;
        //        ddl_PeriodFrom.Enabled = false;
        //        ddl_PeriodTo.Enabled = false;
        //    }
        //}

        //protected void ddl_PeriodFrom_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ddl_PeriodTo.SelectedIndex = ddl_PeriodFrom.SelectedIndex;
        //}

        //protected void rad_Version_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (rad_Version.SelectedValue == "SUN")
        //    {
        //        row_InvoiceDateRange.Visible = true;
        //        col_Period.Visible = true;

        //        ddl_Year.Enabled = true;
        //        ddl_PeriodFrom.Enabled = true;
        //        ddl_PeriodTo.Enabled = true;

        //        txt_InvoiceDateFrom.Enabled = false;
        //        txt_InvoiceDateTo.Enabled = false;
        //    }
        //    else
        //    {
        //        row_InvoiceDateRange.Visible = false;
        //        col_Period.Visible = false;
        //        ddl_Year.Enabled = true;
        //        ddl_PeriodFrom.Enabled = true;
        //        ddl_PeriodTo.Enabled = true;
        //        rad_InvoiceDate.Checked = false;
        //        rad_FiscalPeriod.Checked = true;
        //        txt_InvoiceDateFrom.Enabled = false;
        //        txt_InvoiceDateTo.Enabled = false;
        //    }
        //}

    }
}
