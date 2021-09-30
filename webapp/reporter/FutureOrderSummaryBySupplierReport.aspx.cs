using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.util;
using com.next.isam.domain.common;
using com.next.isam.reporter.accounts;
using com.next.isam.appserver.claim;
using com.next.isam.appserver.account;
using com.next.common.web.commander;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.isam.webapp.webservices;
using CrystalDecisions.Shared;
using System.IO;
using com.next.isam.domain.types;

namespace com.next.isam.webapp.reporter
{
    public partial class FutureOrderSummaryBySupplierReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            AccountFinancialCalenderDef calDef = CommonUtil.getCurrentFinancialPeriod(9);
            ddlOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--ALL--", GeneralCriteria.ALL.ToString());
            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(UclSmartSelection.SelectionList.garmentVendor);
        }

        private int writeSummary(int officeId, int vendorId, FutureOrderSummaryBySupplierReportDs ds, string templateWorksheetID, SpreadsheetDocument document, string worksheetID, int startingRow, int summarySheetStartingRow)
        {
            int counter = 0;
            string summaryWorksheetID = string.Empty;
            summaryWorksheetID = OpenXmlUtil.getWorksheetId(document, "Summary");
            int[] styleIdList = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 10, 14);
            int[] summaryStyleIdList = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 29, 9);
            int cnt = (from w in ds.FutureOrderSummaryBySupplierReport.AsEnumerable()
                         where w.OfficeId == officeId && w.VendorId == vendorId
                         select w).Count();
            List<GenericDataSummaryDef> list = (from w in ds.FutureOrderSummaryBySupplierReport.AsEnumerable()
                                                where w.OfficeId == officeId && w.VendorId == vendorId
                                                select w into s
                                                group s by new
                                                {
                                                    s.CurrencyId
                                                } into itm
                                                select new GenericDataSummaryDef
                                                {
                                                    Id1 = itm.First().CurrencyId,
                                                    Num1 = itm.Count(c => c.OfficeId > 0),
                                                    Num4 = itm.Sum(c => c.SupplierNetAmt),
                                                    Num5 = itm.Sum(c => c.SupplierNetAmt_5_Pct),
                                                    Num6 = itm.Sum(c => c.DeductionAmt)
                                                }).ToList();
            list.AddRange((from itm in UKClaimManager.Instance.getOutstandingUKClaimByCurrency(vendorId, officeId)
                           select new GenericDataSummaryDef
                           {
                               Id1 = itm.Id1,
                               Num2 = itm.Num1
                           }).ToList());
            list.AddRange((from itm in AccountManager.Instance.getOutstandingAdvancePaymentByCurrency(vendorId, officeId)
                           select new GenericDataSummaryDef
                           {
                               Id1 = itm.Id1,
                               Num3 = itm.Num1
                           }).ToList());
            List<GenericDataSummaryDef> summaryList = (from s in list
                                                 group s by new
                                                 {
                                                     s.Id1
                                                 } into itm
                                                 select new GenericDataSummaryDef
                                                 {
                                                     Id1 = itm.First().Id1,
                                                     String1 = CommonUtil.getCurrencyByKey(itm.First().Id1).CurrencyCode,
                                                     Num1 = itm.Sum(c => c.Num1),
                                                     Num2 = itm.Sum(c => c.Num2),
                                                     Num3 = itm.Sum(c => c.Num3),
                                                     Num4 = itm.Sum(c => c.Num4),
                                                     Num5 = itm.Sum(c => c.Num5),
                                                     Num6 = itm.Sum(c => c.Num6)
                                                 } into o
                                                 orderby o.String1
                                                 select o).ToList();
            foreach (GenericDataSummaryDef item in summaryList)
            {
                List<OpenXmlCell> cellValueList = new List<OpenXmlCell>();
                cellValueList.Add(new OpenXmlCell(startingRow, 1, CellValues.SharedString, styleIdList[0], string.Empty));
                cellValueList.Add(new OpenXmlCell(startingRow, 2, CellValues.SharedString, styleIdList[1], string.Empty));
                cellValueList.Add(new OpenXmlCell(startingRow, 3, CellValues.SharedString, styleIdList[2], string.Empty));
                cellValueList.Add(new OpenXmlCell(startingRow, 4, CellValues.SharedString, styleIdList[3], string.Empty));
                cellValueList.Add(new OpenXmlCell(startingRow, 5, CellValues.SharedString, styleIdList[4], string.Empty));
                cellValueList.Add(new OpenXmlCell(startingRow, 6, CellValues.SharedString, styleIdList[5], string.Empty));
                cellValueList.Add(new OpenXmlCell(startingRow, 7, CellValues.SharedString, styleIdList[6], "TOTAL " + item.String1));
                int iStartRow = startingRow - cnt;
                int iEndRow = startingRow - 1;
                cellValueList.Add(new OpenXmlCell(startingRow, 8, CellValues.Number, styleIdList[7], OpenXmlUtil.getSingleColumnFormulaTextWithFilter("SUM", 8, iStartRow, iEndRow, 7, item.String1)));
                cellValueList.Add(new OpenXmlCell(startingRow, 9, CellValues.Number, styleIdList[8], OpenXmlUtil.getSingleColumnFormulaTextWithFilter("SUM", 9, iStartRow, iEndRow, 7, item.String1)));
                cellValueList.Add(new OpenXmlCell(startingRow, 10, CellValues.SharedString, styleIdList[9], string.Empty));
                /*
                cellValueList.Add(new OpenXmlCell(startingRow, 11, CellValues.Number, styleIdList[10], OpenXmlUtil.getSingleColumnFormulaTextWithFilter("COUNT", 7, iStartRow, iEndRow, 7, item.String1)));
                */
                cellValueList.Add(new OpenXmlCell(startingRow, 11, CellValues.Number, styleIdList[10], OpenXmlUtil.getSingleColumnFormulaText("COUNTA", 11, iStartRow, iEndRow)));
                cellValueList.Add(new OpenXmlCell(startingRow, 12, CellValues.Number, styleIdList[11], item.Num2.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 13, CellValues.Number, styleIdList[12], item.Num3.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 14, CellValues.Number, styleIdList[13], OpenXmlUtil.getSingleColumnFormulaTextWithFilter("SUM", 14, iStartRow, iEndRow, 7, item.String1)));
                OpenXmlUtil.createRowAndCells(document, worksheetID, startingRow, 1, cellValueList);
                counter++;
                startingRow++;
                cellValueList = new List<OpenXmlCell>();
                cellValueList.Add(new OpenXmlCell(summarySheetStartingRow, 1, CellValues.SharedString, summaryStyleIdList[0], OfficeId.getName(officeId)));
                cellValueList.Add(new OpenXmlCell(summarySheetStartingRow, 2, CellValues.SharedString, summaryStyleIdList[1], IndustryUtil.getVendorByKey(vendorId).Name));
                cellValueList.Add(new OpenXmlCell(summarySheetStartingRow, 3, CellValues.SharedString, summaryStyleIdList[2], item.String1));
                cellValueList.Add(new OpenXmlCell(summarySheetStartingRow, 4, CellValues.Number, summaryStyleIdList[3], OpenXmlUtil.getSingleColumnFormulaTextWithFilter("SUM", 8, iStartRow, iEndRow, 7, item.String1, "Main")));
                cellValueList.Add(new OpenXmlCell(summarySheetStartingRow, 5, CellValues.Number, summaryStyleIdList[4], OpenXmlUtil.getSingleColumnFormulaTextWithFilter("SUM", 9, iStartRow, iEndRow, 7, item.String1, "Main")));
                cellValueList.Add(new OpenXmlCell(summarySheetStartingRow, 6, CellValues.Number, summaryStyleIdList[5], OpenXmlUtil.getSingleColumnFormulaTextWithFilter("COUNT", 7, iStartRow, iEndRow, 7, item.String1, "Main")));
                cellValueList.Add(new OpenXmlCell(summarySheetStartingRow, 7, CellValues.Number, summaryStyleIdList[6], item.Num2.ToString()));
                cellValueList.Add(new OpenXmlCell(summarySheetStartingRow, 8, CellValues.Number, summaryStyleIdList[7], item.Num3.ToString()));
                cellValueList.Add(new OpenXmlCell(summarySheetStartingRow, 9, CellValues.Number, summaryStyleIdList[8], OpenXmlUtil.getSingleColumnFormulaTextWithFilter("SUM", 14, iStartRow, iEndRow, 7, item.String1, "Main")));
                OpenXmlUtil.createRowAndCells(document, summaryWorksheetID, summarySheetStartingRow, 1, cellValueList);
                summarySheetStartingRow++;
            }
            return counter;
        }

        private void genReportOpenXml(string exportType)
        {
            int vendorId = -1;
            string vendorStr = "ALL";
            int officeId = -1;
            string officeStr = "ALL";
            DateTime dateFrom = DateTime.MinValue;
            DateTime dateTo = DateTime.MinValue;
            if (ddlOffice.selectedValueToInt != -1)
            {
                officeId = int.Parse(ddlOffice.SelectedValue);
                officeStr = CommonUtil.getOfficeRefByKey(officeId).Description;
            }
            if (txt_Supplier.VendorId != int.MinValue)
            {
                vendorStr = IndustryUtil.getVendorByKey(txt_Supplier.VendorId).Name;
                vendorId = txt_Supplier.VendorId;
            }
            if (txt_DateFrom.Text.Trim() != string.Empty)
            {
                dateFrom = DateTimeUtility.getDate(txt_DateFrom.Text);
                if (txt_DateTo.Text.Trim() == string.Empty)
                {
                    txt_DateTo.Text = txt_DateFrom.Text;
                }
                dateTo = DateTimeUtility.getDate(txt_DateTo.Text);
            }
            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + "report_template\\";
            string sourceFileName = "FutureOrderSummaryBySupplier.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = string.Format(base.ApplPhysicalPath + "reporter\\tmpReport\\FOSBS-{0}-{1}.xlsm", base.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, overwrite: true);
            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;
            string summaryWorksheetID = string.Empty;
            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, isEditable: true);
            worksheetID = OpenXmlUtil.getWorksheetId(document, "Main");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");
            summaryWorksheetID = OpenXmlUtil.getWorksheetId(document, "Summary");
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(base.LogonUserId).DisplayName, CellValues.String);
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.String);
            int cellStyleId = OpenXmlUtil.getCellStyleId(document, worksheetID, "B1");
            OpenXmlUtil.setCellValue(document, worksheetID, 1, 2, officeStr, CellValues.SharedString, cellStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 2, 2, vendorStr, CellValues.SharedString, cellStyleId);

            int[] styleIdList = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 5, 14);
            int[] subtotalStyleIdList = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 10, 14);
            FutureOrderSummaryBySupplierReportDs futureOrderSummaryBySupplierReport = AccountReportManager.Instance.getFutureOrderSummaryBySupplierReport(officeId, vendorId, dateFrom, dateTo);
            int startingRow = 5;
            int summaryStartingRow = 5;
            int detailCount = futureOrderSummaryBySupplierReport.FutureOrderSummaryBySupplierReport.Rows.Count;
            int counter = 1;
            int currentOfficeId = -1;
            int currentVendorId = -1;
            string officeName = string.Empty;
            string vendorName = string.Empty;
            bool isNotFirstDetail = false;
            List<string> vendorShipment = new List<string>();
            foreach (FutureOrderSummaryBySupplierReportDs.FutureOrderSummaryBySupplierReportRow row in futureOrderSummaryBySupplierReport.FutureOrderSummaryBySupplierReport.Rows)
            {
                if (currentOfficeId != row.OfficeId || currentVendorId != row.VendorId)
                {
                    if (counter != 1)
                    {
                        startingRow += writeSummary(currentOfficeId, currentVendorId, futureOrderSummaryBySupplierReport, templateWorksheetID, document, worksheetID, startingRow, summaryStartingRow);
                        summaryStartingRow += (from w in futureOrderSummaryBySupplierReport.FutureOrderSummaryBySupplierReport.AsEnumerable()
                                 where w.OfficeId == currentOfficeId && w.VendorId == currentVendorId
                                 select w into c
                                 select c.CurrencyId).Distinct().Count();
                        startingRow++;
                    }
                    officeName = OfficeId.getName(row.OfficeId);
                    vendorName = IndustryUtil.getVendorByKey(row.VendorId).Name;
                    isNotFirstDetail = false;
                    vendorShipment = new List<string>();
                }
                else
                {
                    isNotFirstDetail = true;
                }
                List<OpenXmlCell> list = new List<OpenXmlCell>();
                list.Add(new OpenXmlCell(startingRow, 1, CellValues.SharedString, styleIdList[0], isNotFirstDetail ? string.Empty : officeName));
                list.Add(new OpenXmlCell(startingRow, 2, CellValues.SharedString, styleIdList[1], isNotFirstDetail ? string.Empty : vendorName));
                list.Add(new OpenXmlCell(startingRow, 3, CellValues.SharedString, styleIdList[2], row.ContractNo + row.SplitSuffix));
                list.Add(new OpenXmlCell(startingRow, 4, CellValues.SharedString, styleIdList[3], row.DeliveryNo.ToString()));
                list.Add(new OpenXmlCell(startingRow, 5, CellValues.String, styleIdList[4], DateTimeUtility.getDateString(row.SupplierAtWarehouseDate)));
                list.Add(new OpenXmlCell(startingRow, 6, CellValues.SharedString, styleIdList[5], string.Empty));
                list.Add(new OpenXmlCell(startingRow, 7, CellValues.SharedString, styleIdList[6], CommonUtil.getCurrencyByKey(row.CurrencyId).CurrencyCode));
                list.Add(new OpenXmlCell(startingRow, 8, CellValues.Number, styleIdList[7], row.SupplierNetAmt.ToString()));
                list.Add(new OpenXmlCell(startingRow, 9, CellValues.Number, styleIdList[8], row.SupplierNetAmt_5_Pct.ToString()));
                list.Add(new OpenXmlCell(startingRow, 10, CellValues.SharedString, styleIdList[9], ContractWFS.getType(row.WorkflowStatusId).Name));
                /*
                list.Add(new OpenXmlCell(startingRow, 11, CellValues.SharedString, styleIdList[10], string.Empty));
                */
                string shipment = row.ContractNo + "-" + row.DeliveryNo.ToString();
                list.Add(new OpenXmlCell(startingRow, 11, CellValues.SharedString, styleIdList[10], (vendorShipment.Contains(shipment) ? string.Empty : shipment)));
                vendorShipment.Add(shipment);
                list.Add(new OpenXmlCell(startingRow, 12, CellValues.SharedString, styleIdList[11], string.Empty));
                list.Add(new OpenXmlCell(startingRow, 13, CellValues.SharedString, styleIdList[12], string.Empty));
                list.Add(new OpenXmlCell(startingRow, 14, CellValues.Number, styleIdList[13], row.DeductionAmt.ToString()));
                OpenXmlUtil.createRowAndCells(document, worksheetID, startingRow, 1, list);
                startingRow++;
                counter++;
                currentOfficeId = row.OfficeId;
                currentVendorId = row.VendorId;
            }
            if (futureOrderSummaryBySupplierReport.FutureOrderSummaryBySupplierReport.Rows.Count > 0)
            {
                startingRow += writeSummary(currentOfficeId, currentVendorId, futureOrderSummaryBySupplierReport, templateWorksheetID, document, worksheetID, startingRow, summaryStartingRow);
                summaryStartingRow += (from w in futureOrderSummaryBySupplierReport.FutureOrderSummaryBySupplierReport.AsEnumerable()
                         where w.OfficeId == currentOfficeId && w.VendorId == currentVendorId
                         select w into c
                         select c.CurrencyId).Distinct().Count();
                summaryStartingRow--;

                OpenXmlUtil.addDataBar(document, summaryWorksheetID, OpenXmlUtil.DataBarColor.Green, 4, 5, summaryStartingRow);
                OpenXmlUtil.addDataBar(document, summaryWorksheetID, OpenXmlUtil.DataBarColor.Orange, 6, 5, summaryStartingRow);
            }
            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 23, worksheetID, startingRow + 2);
            OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(1, startingRow + 2), OpenXmlUtil.getCellReference(14, startingRow + 2));
            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 23, summaryWorksheetID, summaryStartingRow + 2);
            OpenXmlUtil.mergeCells(document, summaryWorksheetID, OpenXmlUtil.getCellReference(1, summaryStartingRow + 2), OpenXmlUtil.getCellReference(9, summaryStartingRow + 2));
            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();
            WebHelper.outputFileAsHttpRespone(base.Response, destFile, deleteFile: true);
        }

        private void exportReport(string strExportType, ExportFormatType exportType)
        {
            genReportOpenXml(strExportType);
        }

        private string getCellRefList(int col, string commaDelimitedValues)
        {
            string colPrefix = OpenXmlUtil.getColumnLetter(col);
            string[] source = commaDelimitedValues.Split(',');
            return string.Join(",", source.Select((string x) => colPrefix + x).ToArray());
        }

        protected void val_Custom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (txt_DateFrom.DateTime == DateTime.MinValue || txt_DateTo.DateTime == DateTime.MinValue)
            {
                ((CustomValidator)source).ErrorMessage = "Please input the date range";
                args.IsValid = false;
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string strExportType = "Excel";
                ExportFormatType exportType = ExportFormatType.Excel;
                exportReport(strExportType, exportType);
            }
        }
    }
}