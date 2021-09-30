using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.IO;

using com.next.isam.reporter.dataserver;
using com.next.isam.reporter.NSSreport;
using com.next.common.domain;
using com.next.common.datafactory.worker;
using com.next.infra.util;
using com.next.isam.dataserver.worker;

using xmlDoc = DocumentFormat.OpenXml;
using xmlPackaging = DocumentFormat.OpenXml.Packaging;
using xmlSpreadsheet = DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;


namespace com.next.isam.reporter.NSSReport
{
    public class NSSReportManager
    {
        private static NSSReportManager _instance;
        //private CommonWorker commonWorker;

        public NSSReportManager()
        {
          //  commonWorker = CommonWorker.Instance;
        }

        public static NSSReportManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NSSReportManager();
                }
                return _instance;
            }
        }

        // NSS Weekly Sales Report for Simon & Fiaz
        public void generateNNSWeeklyReport()
        {
            string tempFolder = WebConfig.getValue("appSettings", "NSS_WEEKLY_REPORT_TEMP_FOLDER");
            string reportFolder = WebConfig.getValue("appSettings", "NSS_WEEKLY_REPORT_FOLDER");
            if (reportFolder.EndsWith("\\"))
                reportFolder = reportFolder.Substring(0, reportFolder.Length - 1);
            if (tempFolder.EndsWith("\\"))
                tempFolder = tempFolder.Substring(0, tempFolder.Length - 1);

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);
            if (!Directory.Exists(reportFolder))
                Directory.CreateDirectory(reportFolder);

            if (!string.IsNullOrEmpty(reportFolder) && !string.IsNullOrEmpty(tempFolder))
            {
                SortedList<string, ArrayList> reportList = new SortedList<string, ArrayList>();
                reportList.Add("Report 1 - Weekly Sales Report", getNSSWeeklySalesReport(tempFolder));
                sendNSSReport(reportList);
            }
        }

        private ArrayList getNSSWeeklySalesReport(string folder)
        {
            ArrayList path = new ArrayList();

            string sourceFileDir = WebConfig.getValue("appSettings", "APP_REPORT_TEMPLATE_FOLDER");
            string destFileDir = WebConfig.getValue("appSettings", "NSS_WEEKLY_REPORT_TEMP_FOLDER");
            string worksheetID = string.Empty;
            SpreadsheetDocument document = null;
            int fiscalYear = 0;
            int fiscalPeriod = 0;
            int weekNo = 0;

            try
            {
                SortedList<DateTime, AccountFinancialCalenderDef> reportPeriod = new SortedList<DateTime, AccountFinancialCalenderDef>();
                DateTime reportDate = DateTime.Now.Date;
                int dow = reportDate.DayOfWeek.GetHashCode();
                DateTime reportCutoffDate = reportDate.AddDays(-(dow + 2));
                
                AccountFinancialCalenderDef period, currentPeriod;
                currentPeriod = GeneralWorker.Instance.getAccountPeriodByDate(9, reportCutoffDate);
                reportPeriod.Add(currentPeriod.StartDate, currentPeriod);
                period = GeneralWorker.Instance.getAccountPeriodByDate(9, reportCutoffDate.AddDays(7));
                if (!reportPeriod.ContainsKey(period.StartDate))
                    reportPeriod.Add(period.StartDate, period);     // include starting point of next period

                ArrayList attachment = new ArrayList();
                ArrayList mainAttachment = new ArrayList();
                string summaryTable = string.Empty;
                string sourceFileName = string.Empty;
                string sheetName = string.Empty;
                foreach (AccountFinancialCalenderDef p in reportPeriod.Values)
                {
                    bool forCurrentPeriod = (p.Period == currentPeriod.Period);
                    fiscalYear = p.BudgetYear;
                    fiscalPeriod = p.Period;
                    weekNo = (forCurrentPeriod ? (int)decimal.Ceiling(((reportCutoffDate - currentPeriod.StartDate).Days + 2) / 7) : 0);
                    int numOfWeek = ((p.EndDate - p.StartDate).Days + 1) / 7;

                    for (int rpt = 0; rpt < 4; rpt++)
                    {
                        switch (rpt)
                        {
                            case 0: // Weekly Sales Summary Report
                                sourceFileName = "WeeklySales.xlsx";
                                sheetName = "WeeklySalesSummary";
                                break;
                            case 1: // Weekly Sales Summary (New format) Report
                                sourceFileName = "WeeklySales-New Format.xlsx";
                                sheetName = "WeeklySalesSummary";
                                break;
                            case 2: // Weekly Sales Detail Report
                                sourceFileName = "WeeklySalesDetail.xlsx";
                                sheetName = "WeeklySalesDetail";
                                break;
                            case 3: // Weekly Sales Slipped Order
                                if (weekNo == 0) continue;
                                sourceFileName = "WeeklySalesSlippedOrders.xlsx";
                                sheetName = "SlippedOrder";
                                break;
                            default: break;
                        }

                        string destFile = String.Format(destFileDir + "{0} P{1}-" + sourceFileName.Replace(".", "-{2}."), fiscalYear, fiscalPeriod, DateTime.Now.ToString("yyyyMMdd"));
                        if (File.Exists(sourceFileDir + "\\" + sourceFileName) && Directory.Exists(destFileDir))
                            File.Copy(sourceFileDir + "\\" + sourceFileName, destFile, true);

                        document = SpreadsheetDocument.Open(destFile, true);
                        worksheetID = OpenXmlUtil.getWorksheetId(document, sheetName);
                        OpenXmlUtil.setCellValue(document, worksheetID, 1, 1, fiscalYear.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, worksheetID, 2, 1, fiscalPeriod.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, worksheetID, 3, 1, p.StartDate.ToString("yyyy-MM-dd"), CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, worksheetID, 4, 1, p.EndDate.ToString("yyyy-MM-dd"), CellValues.SharedString);

                        // Fill in weekly data into Excel
                        switch (rpt)
                        {
                            case 0: // Weekly Sales Summary Report
                            case 1: // Weekly Sales Summary (New format) Report
                                summaryTable += createWeeklySalesSummaryReport(document, worksheetID, p, weekNo, forCurrentPeriod, numOfWeek, (rpt==0));
                                break;
                            case 2: // Weekly Sales Detail Report
                                ArrayList monthToDateData = ReporterWorker.Instance.getNssWeeklySalesDetailData(fiscalYear, fiscalPeriod, weekNo);
                                putWeeklySalesDetailData(document, worksheetID, monthToDateData);
                                if (numOfWeek < 5)   // hide the week 5 columns
                                    OpenXmlUtil.hideColumns(document, worksheetID, 33, 37);
                                break;
                            case 3: // Weekly Sales Slipped Order
                                ArrayList weeklySlippedOrder = ReporterWorker.Instance.getNssWeeklySalesSlippedOrderData(fiscalYear, fiscalPeriod, weekNo);
                                putWeeklySalesSlippedOrderData(document, worksheetID, weeklySlippedOrder);
                                break;
                            default: break;
                        }
                        document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
                        document.WorkbookPart.Workbook.Save();
                        document.Close();
                        document.Dispose();
                        document = null;
                        attachment.Add(destFile);

                        if (rpt == 0)
                        {
                            mainAttachment.Add(destFile);
                            if (mainAttachment.Count == reportPeriod.Values.Count)
                            {
                                string remark = string.Empty;
                                int wkno = ((int)decimal.Ceiling(((reportCutoffDate - currentPeriod.StartDate).Days + 2) / 7));
                                if (reportPeriod.Values.Count > 1)
                                    remark = string.Format("<br>Please also find {0} P{1} starting as attached<br>", reportPeriod.Values[1].BudgetYear, reportPeriod.Values[1].Period);
                                NoticeHelper.sendNssWeeklySalesReportToSimon(reportPeriod.Values[0].BudgetYear, reportPeriod.Values[0].Period, wkno, reportDate, mainAttachment, summaryTable, remark);
                            }
                        }
                    }
                 }
                NoticeHelper.sendNssWeeklySalesReportToIT(reportDate, attachment);
                backupAttachment(attachment);
            }

            catch (Exception e)
            {
                string moreInfo = (fiscalYear != 0 && fiscalPeriod != 0 ? " for " + fiscalYear.ToString() + " P" + fiscalPeriod.ToString() + " Week " + weekNo.ToString() : string.Empty);
                MailHelper.sendErrorAlert(e, "Handler<br><br> Error in generating NSS Weekly Sales Report" + moreInfo);
            }
            finally
            {
                if (document != null)
                {
                    document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
                    document.WorkbookPart.Workbook.Save();
                    document.Close();
                    document.Dispose();
                }
            }
            return path;
        }

        private void backupAttachment(ArrayList attachment)
        {
            string tempFolder = WebConfig.getValue("appSettings", "NSS_WEEKLY_REPORT_TEMP_FOLDER");
            string reportFolder = WebConfig.getValue("appSettings", "NSS_WEEKLY_REPORT_FOLDER");
            if (!reportFolder.EndsWith("\\"))
                reportFolder += "\\";
            if (!tempFolder.EndsWith("\\"))
                tempFolder += "\\";

            string weeklyFolder = reportFolder + DateTime.Now.Date.ToString("yyyyMMdd");
            if (!Directory.Exists(weeklyFolder))
                Directory.CreateDirectory(weeklyFolder);
            string report1Folder = weeklyFolder + "\\1.WeeklySales (To Simon and Jo)";
            if (!Directory.Exists(report1Folder))
                Directory.CreateDirectory(report1Folder);

            if (!string.IsNullOrEmpty(report1Folder) && !string.IsNullOrEmpty(tempFolder))
                foreach (string filePath in attachment)
                {
                    string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                    string destFile = report1Folder + "\\" + fileName;
                    File.Copy(filePath, destFile, true);
                    if (File.Exists(destFile))
                        File.Delete(filePath);
                }
        }

        private string convertListToHtmlTable(List<string[]> list)
        {   // Row 0 is the column header
            string html = string.Empty;
            int colCount = 0;
            foreach (string[] row in list)
                colCount = (row.Length > colCount ? row.Length : colCount);
            int r = 0;
            int lastRow = list.Count - 1;
            foreach (string[] row in list)
            {
                if (html == string.Empty)
                    html += "<TR STYLE='FONT-SIZE:16;TEXT-ALIGN:CENTER;HEIGHT:42px;FONT-WEIGHT:bold;BACKGROUND-COLOR:#4F81BD;COLOR:WHITE;VERTICAL-ALIGN:BOTTOM;'>";
                else
                    html += "<TR STYLE='FONT-SIZE:16;TEXT-ALIGN:RIGHT;HEIGHT:18px;'>";

                string bThick = "solid windowtext 1.5pt";
                string bThin = "solid #95B3D7 1.0pt";
                string bDouble = "double #4F81BD 2.25pt";
                string bNone = "none";
                for (int c = 0; c < row.Length; c++)
                {
                    string style = string.Empty;
                    if (r > 0)
                    {
                        style += "TEXT-ALIGN:" + (c == 0 || (r == 0 && c == 1) ? "CENTER" : "RIGHT") + ";";
                        style += "BACKGROUND-COLOR:" + ((r + 1) % 2 == 0 ? "#DCE6F1" : "#FFFFFF") + ";";
                        style += "FONT-WEIGHT:" + (r == lastRow ? "BOLD" : "NORMAL") + ";";
                        style += "FONT-STYLE:" + ((r == lastRow && c == 0) || c == 2 ? "ITALIC" : "NORMAL") + ";";
                    }
                    style += "BORDER-TOP:" + (r == 0 && c > 0 ? bThick : (r == lastRow ? bDouble : bThin)) + ";";
                    style += "BORDER-LEFT:" + (c == 1 ? bThick : (r > 0 && c == 0 ? bThin : bNone)) + ";";
                    style += "BORDER-BOTTOM:" + ((r == lastRow || r == 0) && c > 0 ? bThick : (r == lastRow ? bThin : bNone)) + ";";
                    style += "BORDER-RIGHT:" + (c == 2 || (r == 0 && c == 1) ? bThick : bNone) + ";";
                    style += "PADDING:0pt 5pt 0pt 5pt;";
                    style += "WIDTH:" + (c == 1 ? "100" : "60") + "px;";
                    html += "<TD nowrap " + (c == row.Length - 1 && row.Length < colCount ? "COLSPAN='" + (colCount - row.Length + 1).ToString() + "'" : "") + "STYLE='" + style + "'" + ">";
                    html += row[c];
                    html += "</TD>";
                }
                html += "</TR>";
                r++;
            }

            html = "<TABLE BORDER=0 CELLPADDING=0 CELLSPACING=0 STYLE='font-family:calibri,sans-serif;border-collapse:collapse;'>" + html + "</TABLE>";
            return HttpUtility.HtmlDecode(html);
        }

        private string createWeeklySalesSummaryReport(SpreadsheetDocument document, string worksheetID, AccountFinancialCalenderDef period, int weekNo, bool forCurrentPeriod, int numOfWeek, bool isSimplifyReport)
        {
            ArrayList weeklyData = null, prevWeeklyData = null, startingPointData = null;
            string summaryTable = string.Empty;
            int rowPerWeek = 15;
            int offset = 2;
            int maxWeek = 0;

            // fill in sheet header
            string heading = string.Concat("Weekly Sales in USD - P", period.Period, " ", period.BudgetYear);
            OpenXmlUtil.setCellValue(document, worksheetID, 1, 2, heading, CellValues.SharedString);
            for (int wk = 0; wk <= weekNo; wk++)
            {
                int row = offset + wk * rowPerWeek + 1;
                int col = 2;
                ArrayList data = ReporterWorker.Instance.getNssWeeklySalesSummaryData(period.BudgetYear, period.Period, wk);
                if (data.Count > 0)
                {
                    if (wk == 0)
                        startingPointData = data;
                    else
                        prevWeeklyData = weeklyData;
                    weeklyData = data;
                    putWeeklySalesSummaryData(document, worksheetID, row, col, weeklyData, period, wk, numOfWeek, isSimplifyReport);
                    putWeeklySalesVarianceSummaryData(document, worksheetID, row, (isSimplifyReport ? 10 : 25), weeklyData, prevWeeklyData, startingPointData, period, wk, numOfWeek);
                    maxWeek = wk;
                }
                else
                    break;
            }

            // hide the un-neccessary region
            int noOfRow = offset + (maxWeek + 1) * rowPerWeek;
            OpenXmlUtil.hideRows(document, worksheetID, noOfRow + 1, 99);
            if (maxWeek == 0)// starting point -  hide the variance section
                if (isSimplifyReport)
                    OpenXmlUtil.hideColumns(document, worksheetID, 10, 22);
                else
                    OpenXmlUtil.hideColumns(document, worksheetID, 25, 37);

            if (numOfWeek < 5)   // hide the week 5 figure
                if (isSimplifyReport)
                    OpenXmlUtil.hideColumns(document, worksheetID, new int[] { 7, 19, 20 });
                else
                    OpenXmlUtil.hideColumns(document, worksheetID, new int[] { 19, 20, 21, 22, 34, 35 });

            if (forCurrentPeriod && isSimplifyReport)
            {   // Build the weekly sales variance summary table
                List<string[]> list = new List<string[]>();
                list.Add(new string[] { "Office", "Wk" + maxWeek.ToString() });
                if (prevWeeklyData != null && weeklyData != null)
                {
                    int col = maxWeek * 4;
                    decimal prevTotal = 0, total = 0;
                    string variance, percentage;
                    for (int i = 0; i < weeklyData.Count; i++)
                    {
                        string office = ((object[])weeklyData[i])[0].ToString();
                        decimal prevSalesAmt = decimal.Parse(((object[])prevWeeklyData[i])[col].ToString());
                        decimal SalesAmt = decimal.Parse(((object[])weeklyData[i])[col].ToString());
                        variance = (SalesAmt - prevSalesAmt).ToString("#,000");
                        percentage = decimal.Round((prevSalesAmt == 0 ? 0 : (SalesAmt - prevSalesAmt) / prevSalesAmt), 2).ToString("#0%");
                        list.Add(new string[] { office, variance, percentage });
                        prevTotal += prevSalesAmt;
                        total += SalesAmt;
                    }
                    percentage = decimal.Round((prevTotal == 0 ? 0 : (total - prevTotal) / prevTotal), 2).ToString("0%");
                    variance = (total - prevTotal).ToString("#,000");
                    list.Add(new string[] { "Variance", variance, percentage });
                }
                summaryTable = convertListToHtmlTable(list);
            }
            return summaryTable;
        }


        void putWeeklySalesSummaryData(SpreadsheetDocument doc, string worksheetId, int row, int col, ArrayList weeklyData,  AccountFinancialCalenderDef period, int weekNo, int numOfWeek, bool simplify)
        {
            int r, c, i, j, s;
            List<decimal> columnTotal = new List<decimal>();
            decimal rowTotal = 0;
            decimal value = 0;
            decimal grandTotal = 0;

            // fill in table header
            string heading = string.Concat("P", period.Period , " ", (weekNo == 0 ? "Starting Point" : "Week " + weekNo.ToString()), " (As of ", period.StartDate.AddDays(weekNo * 7 + 5).ToString("dd/MM/yyyy"));
            heading += (weekNo == numOfWeek ? "" : string.Concat(" - Wk", weekNo + 1, " includes Order Book in ", period.BudgetYear - (period.Period <= 2 ? 1 : 0), "-P", period.Period + (period.Period <= 2 ? 12 : 0) - 2, " & unconfirmed + order book in ", period.BudgetYear - (period.Period <= 1 ? 1 : 0), "-P", (period.Period + (period.Period <= 1 ? 12 : 0) - 1), ")"));
            OpenXmlUtil.setCellValue(doc, worksheetId, row, col, heading, CellValues.SharedString);
            int step = (simplify ? 4 : 1);
            // fill in column header
            for (c = 1; c <= 5; c++)
            {
                string wkHeader = string.Concat(period.StartDate.AddDays((c - 1) * 7).ToString("dd/MM"), " - ", period.StartDate.AddDays((c - 1) * 7 + 6).ToString("dd/MM"), "\n Wk", c, (c <= weekNo ? " (Actual)" : ""));
                OpenXmlUtil.setCellValue(doc, worksheetId, row + 1, col + (simplify ? c : 4 * (c - 1) + 1), wkHeader, CellValues.SharedString);
            }
            // fill in detail Figure
            for (c = 1; c < (0.0 + ((object[])weeklyData[0]).Length) / step; c++)
                columnTotal.Add(0);
            r = row + 3;
            for (i = 0; i < weeklyData.Count; i++)
            {
                object[] columns = (object[])weeklyData[i];
                rowTotal = 0;
                OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col, columns[0].ToString(), CellValues.SharedString);
                c = 0;
                for (j = 1; j < columns.Length; j++)
                    if (!simplify || (simplify && j % step == 0))
                    {
                        value = decimal.Parse(columns[j].ToString());
                        OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + 1 + c, value.ToString(), CellValues.Number);
                        if (j % 4 == 0)
                            rowTotal += value;
                        columnTotal[c] += value;
                        c++;
                    }
                OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + 1 + c, rowTotal.ToString(), CellValues.Number);
                grandTotal += rowTotal;
            }
            for (c = 1; c <= columnTotal.Count; c++)
            {
                OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + c, columnTotal[c - 1].ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(doc, worksheetId, r + i + 1, col + c, (grandTotal == 0 ? 0 : (columnTotal[c - 1] / grandTotal)).ToString(), CellValues.Number);
            }
            OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + c, grandTotal.ToString(), CellValues.Number);
        }

        void putWeeklySalesVarianceSummaryData(SpreadsheetDocument doc, string worksheetId, int row, int col,
            ArrayList thisWeekData, ArrayList previousWeekData, ArrayList startingPointData, AccountFinancialCalenderDef period, int weekNo, int numOfWeek)
        {
            if (previousWeekData != null && weekNo > 0)
            {
                int r, c, i, j;
                List<decimal> thisRowTotal = new List<decimal>(), prevRowTotal = new List<decimal>(), startingRowTotal = new List<decimal>();
                List<Decimal> thisColumnTotal = new List<decimal>(), prevColumnTotal = new List<decimal>(), StartingColumnTotal = new List<decimal>();
                decimal thisValue = 0, prevValue = 0, startingValue = 0, percentage = 0, variance = 0;
                decimal varianceGrandTotal = 0, startingGrandTotal = 0;

                // fill in table header
                int step = 4;// (simplify ? 4 : 1);
                string heading = string.Concat("P", period.Period , " ", "Week " + weekNo.ToString(), " Result vs P", period.Period, (weekNo == 1 ? " Starting Point" : " Week " + (weekNo - 1).ToString()), " Result");
                OpenXmlUtil.setCellValue(doc, worksheetId, row, col, heading, CellValues.SharedString);
                // fill in column header
                for (c = 1; c <= 5; c++)
                {
                    string wkHeader = "Wk" + c.ToString();
                    OpenXmlUtil.setCellValue(doc, worksheetId, row + 1, col + c * 2 - 1, wkHeader, CellValues.SharedString);
                    thisColumnTotal.Add(0);
                    prevColumnTotal.Add(0);
                    StartingColumnTotal.Add(0);
                }
                r = row + 3;
                for (i = 0; i < thisWeekData.Count; i++)
                {
                    object[] thisWeekColumns = (object[])thisWeekData[i];
                    object[] prevWeekColumns = (object[])previousWeekData[i];
                    object[] startingWeekColumns = (object[])startingPointData[i];
                    thisRowTotal.Add(0);
                    prevRowTotal.Add(0);
                    startingRowTotal.Add(0);

                    c = col;
                    OpenXmlUtil.setCellValue(doc, worksheetId, r + i, c, thisWeekColumns[0].ToString(), CellValues.SharedString);
                    c = c + 1;
                    for (j = 0; j < thisWeekColumns.Length / step; j++)
                    {
                        thisValue = decimal.Parse(thisWeekColumns[(j + 1) * step].ToString());
                        prevValue = decimal.Parse(prevWeekColumns[(j + 1) * step].ToString());
                        startingValue = decimal.Parse(startingWeekColumns[(j + 1) * step].ToString());
                        variance = thisValue - prevValue;
                        percentage = (prevValue == 0 ? 0 : variance / prevValue);
                        OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + j * 2 + 1, variance.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + j * 2 + 2, percentage.ToString(), CellValues.Number);

                        thisColumnTotal[j] += thisValue;
                        prevColumnTotal[j] += prevValue;
                        StartingColumnTotal[j] += startingValue;

                        thisRowTotal[i] += thisValue;
                        prevRowTotal[i] += prevValue;
                        startingRowTotal[i] += startingValue;
                        varianceGrandTotal += thisValue - startingValue;
                        startingGrandTotal += startingValue;
                    }
                    variance = thisRowTotal[i] - startingRowTotal[i];
                    percentage = (startingRowTotal[i] == 0 ? 0 : variance / startingRowTotal[i]);
                    OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + j * 2 + 1, variance.ToString(), CellValues.Number);
                    OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + j * 2 + 2, percentage.ToString(), CellValues.Number);
                }
                for (j = 0; j < thisColumnTotal.Count; j++)
                {
                    variance = thisColumnTotal[j] - prevColumnTotal[j];
                    percentage = (prevColumnTotal[j] == 0 ? 0 : variance / prevColumnTotal[j]);
                    OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + j * 2 + 1, variance.ToString(), CellValues.Number);
                    OpenXmlUtil.setCellValue(doc, worksheetId, r + i + 1, col + j * 2 + 1, percentage.ToString(), CellValues.Number);
                }
                OpenXmlUtil.setCellValue(doc, worksheetId, r + i, col + j * 2 + 1, varianceGrandTotal.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(doc, worksheetId, r + i + 1, col + j * 2 + 1, (startingGrandTotal == 0 ? 0 : varianceGrandTotal / startingGrandTotal).ToString(), CellValues.Number);
            }
        }

        private void putWeeklySalesDetailData(SpreadsheetDocument doc, string worksheetId, ArrayList groupedDetailData)
        {
            putDataToWorksheet(doc, worksheetId, groupedDetailData, 7, 2, new int[] { 10, 15, 20, 25, 30, 35, 40, 42 }, new int[] { 4, 9, 14, 19, 24, 29, 34, 39, 41 });
        }


        private void putWeeklySalesSlippedOrderData(SpreadsheetDocument doc, string worksheetId, ArrayList slippedOrderData)
        {
            putDataToWorksheet(doc, worksheetId, slippedOrderData, 5, 2, new int[] { 7 }, new int[] { 5,6 });
        }


        private void putDataToWorksheet(SpreadsheetDocument doc, string worksheetId, ArrayList data, int row, int col, int[] numericColumn, int[] dateColumn)
        {
            int r;
            DateTime date;
            CellValues dataType;
            List<int[]> style = new List<int[]>();
            int styleId;

            if (data.Count > 0)
            {
                object[] columns = (object[])data[0];
                style.Add(OpenXmlUtil.getStyleIdList(doc, worksheetId, row, columns.Length + 1));         // Odd 
                style.Add(OpenXmlUtil.getStyleIdList(doc, worksheetId, row + 1, columns.Length + 1));     // Even
                style.Add(OpenXmlUtil.getStyleIdList(doc, worksheetId, row + 3, columns.Length + 1));     // Ending
                for (r = 0; r < data.Count; r++)
                {
                    columns = (object[])data[r];
                    for (int c = 0; c < columns.Length; c++)
                    {
                        styleId = style[r % 2][c + 1];
                        string val = columns[c].ToString();
                        dataType = CellValues.SharedString;
                        if (numericColumn != null && numericColumn.Contains(c))
                            dataType = CellValues.Number;
                        else if (dateColumn != null && dateColumn.Contains(c))
                            if (DateTime.TryParse(val, out date))
                                val = date.ToString("dd/MM/yyyy");
                            else
                                val = HttpUtility.HtmlEncode(val);
                        OpenXmlUtil.setCellValue(doc, worksheetId, row + r, col + c, val, dataType, styleId);
                    }
                }
                for (int c = 0; c < columns.Length; c++)
                    OpenXmlUtil.setCellValue(doc, worksheetId, row + r, col + c, string.Empty, CellValues.SharedString, style[2][c + 1]);
            }
        }
   
       //private ArrayList getNSSDeliveryPerformanceByPeriodReport(string folder)
        //{
        //    ArrayList path = new ArrayList();
        //    return path;
        //}

        //private ArrayList getNSSDeliveryPerformanceOnTimeReport(string folder)
        //{
        //    ArrayList path = new ArrayList();
        //    return path;
        //}

        private void sendNSSReport(SortedList<string, ArrayList> reportList)
        {
        }
        private void reportFolderHouseKeeping(string folder)
        {
            if (Directory.Exists(folder))
            {
                DirectoryInfo dir;
                string[] folderNames = Directory.GetDirectories(folder, "*.*", SearchOption.TopDirectoryOnly);
                if (folderNames != null)
                    foreach (string name in folderNames)
                    {
                        dir = new DirectoryInfo(name);
                        if ((DateTime.Now - dir.CreationTime).TotalDays > 14)
                            dir.Delete(true);
                    }
            }

        }

    }
}
