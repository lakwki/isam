using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;

using com.next.isam.reporter.helper;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.isam.webapp.webservices;
using com.next.isam.reporter.accounts;
using com.next.infra.util;
using com.next.isam.domain.account;
using com.next.isam.appserver.account;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web.UI.WebControls;
using com.next.isam.appserver.common;
using com.next.isam.domain.common;
using com.next.common.domain.types;
using com.next.isam.dataserver.worker;

namespace com.next.isam.webapp.reporter
{
    public partial class NSLedSalesInfoReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            uclOfficeSelection.UserId = this.LogonUserId;

            //Get Year List
            List<NSLedImportFileDef> fiscalYearList = AccountManager.Instance.getNSLedFiscalYearList();
            for (int i = 0; i < fiscalYearList.Count; i++)
            {
                this.ddl_Year.Items.Add(new ListItem(fiscalYearList[i].FiscalYear.ToString()));
            }

            //Get Fiscal Week List with selected Year
            fiscalWeekDataBind();

            //Get Phase ID list
            ArrayList phaseList = AccountManager.Instance.getRangePlanPhaseIdList();
            for (int i = 0; i < phaseList.Count; i++)
                this.ddl_Phase.Items.Add(new ListItem(phaseList[i].ToString()));
            if (phaseList.Count > 0)
                ddl_Phase.SelectedIndex = (phaseList.Count >= 3 ? 2 : phaseList.Count - 1);
        }

        protected void ddl_Year_SelectedIndexChanged(object sender, EventArgs e)
        {
            fiscalWeekDataBind();
        }

        protected void fiscalWeekDataBind()
        {
            ddl_Week.Items.Clear();
            List<NSLedImportFileDef> fiscalWeekList = AccountManager.Instance.getNSLedFiscalWeekList(int.Parse(this.ddl_Year.Text));
            for (int i = 0; i < fiscalWeekList.Count; i++)
            {
                this.ddl_Week.Items.Add(new ListItem(fiscalWeekList[i].FiscalWeek.ToString()));
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            string strExportType = "Pdf";
            CrystalDecisions.Shared.ExportFormatType exportType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            exportReport(strExportType, exportType);
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            string strExportType = "Excel";
            CrystalDecisions.Shared.ExportFormatType exportType = CrystalDecisions.Shared.ExportFormatType.Excel;
            exportReport(strExportType, exportType);
        }

        private void exportReport(string strExportType, CrystalDecisions.Shared.ExportFormatType exportType)
        {
            if (exportType == CrystalDecisions.Shared.ExportFormatType.Excel)
                genReportOpenXml();

        }

        private void genReportOpenXml()
        {
            #region Data Prepare
            int fiscalYear = ddl_Year.selectedValueToInt;
            int fiscalWeek = ddl_Week.selectedValueToInt;

            TypeCollector phaseList = TypeCollector.Inclusive;
            if (ckb_UnknownSeason.Checked)
                phaseList.append(-1);
            for (int i = 0; i <= ddl_Phase.SelectedIndex; i++)
                phaseList.append(Int32.Parse(ddl_Phase.Items[i].Value));
            List<NSLedProfitabilitiesDef> profitList = AccountManager.Instance.getNSLedProfitabilities(uclOfficeSelection.getOfficeList(), fiscalYear, fiscalWeek, phaseList, ckb_StillSelling.Checked, ckb_NotYetLaunched.Checked, ckb_EndOfLife.Checked, -1, -1);

            if (profitList.Count == 0)
                return;

            List<NSLedSalesInfoDef> salesList = AccountManager.Instance.getNSLedSalesInfo(fiscalYear, fiscalWeek);
            if (salesList.Count == 0)
                return;

            /*
            profitList = profitList
                .OrderByDescending(x => x.OfficeId)
                .ThenBy(x => x.ProductTeamName)
                .ThenBy(x => x.IsNotYetLaunched)
                .ThenByDescending(x => x.IsEndOfLife)
                .ThenBy(x => x.ActualSaleSeasonId)
                .ThenBy(x => x.ActualSaleSeasonSplitId)
                .ThenBy(x => x.FirstInvoiceDate)
                .ThenByDescending(x => x.WeekCount)
                .ToList();
            */
            profitList = profitList
                .OrderByDescending(x => x.OfficeId)
                .ThenByDescending(x => x.WeekCount)
                .ThenBy(x => x.ProductTeamName)
                .ToList();

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "NSLedSalesInfo.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddHHmmss");
            string destFile = string.Format(this.ApplPhysicalPath + @"reporter\tmpReport\NSLedSalesInfo-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);
            string tempSheetId = OpenXmlUtil.getWorksheetId(document, "Template");
            string summSheetId = OpenXmlUtil.getWorksheetId(document, "SUMM");
            string currentSheetId = "";
            int currentOfficeId = 0;
            string currentOfficeCode = "";
            uint salesStyle = 0;
            int itemStartingRow = 4;
            int lastWeekCol = 0;
            int itemCount = 0;
            int itemNextIndex = 1;
            List<int> summaryCol = new List<int>() { 0, 12, 17, 22, 28, 33, 38, 44, 49, 54, 60, 65, 71 };
            int bfCol = OpenXmlUtil.getExcelColumnNo("F");
            int cumCol = OpenXmlUtil.getExcelColumnNo("BT");
            int sizeTableStartCol = OpenXmlUtil.getExcelColumnNo("BV");
            int profitCol = OpenXmlUtil.getExcelColumnNo("CB");
            int forecastCol = OpenXmlUtil.getExcelColumnNo("CE");
            decimal seasonalExchangeRate = CommonManager.Instance.getSeasonalExchangeRate(profitList.Max(x => x.SeasonId), 2, 3);
            decimal markDownRecovery = 0;
            SystemParameterRef param = CommonManager.Instance.getSystemParameterByName("NSLED_MARK_DOWN_RECOVERY");
            if (param != null)
                if (!string.IsNullOrEmpty(param.ParameterValue.Trim()))
                    decimal.TryParse(param.ParameterValue.Trim(), out markDownRecovery);

            #endregion

            foreach (NSLedProfitabilitiesDef def in profitList)
            {
                #region New Office Phase
                //Change sheet for new office
                if (currentOfficeId != def.OfficeId)
                {
                    var officeCode = CommonUtil.getOfficeRefByKey(def.OfficeId).OfficeCode;
                    var sheetId = OpenXmlUtil.getWorksheetId(document, officeCode + " Sales Summ");

                    //Check office exist in execl template
                    if (sheetId.Trim() == "")
                        continue;

                    currentSheetId = sheetId;
                    currentOfficeId = def.OfficeId;
                    currentOfficeCode = officeCode;
                    salesStyle = (uint)OpenXmlUtil.getCellStyleId(document, currentSheetId, "B2");
                    OpenXmlUtil.setCellValue(document, currentSheetId, 1, OpenXmlUtil.getExcelColumnNo("CB"), seasonalExchangeRate.ToString(), CellValues.Number);
                    //Reset row index
                    itemStartingRow = 4;
                    itemCount = 0;
                }
                #endregion

                #region Item Phase
                #region Item Information
                //Copy Row from template
                for (int i = 1; i <= 9; i++)
                {
                    OpenXmlUtil.copyAndInsertRow(document, tempSheetId, i, currentSheetId, itemStartingRow + i - 1);
                }

                //Insert item info
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, 2, currentOfficeCode + (def.HasDuty ? " (duty)" : (def.WeekCount == 0 ? " (not yet launched)" : " (non-duty)")), CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, 2, def.Description + (def.SeasonCount > 1 ? " (repeat)" : ""), CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, 2, def.ItemNo, CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, 2, def.TotalQty.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, 2, def.RetailSellingPrice, CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, 2, def.TotalProductionCost.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, 2, def.AvgRetailSellingPrice.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB")), CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, 3, def.ProductTeamName, CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, 2, def.MDPrice, CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, 3, (def.EstimatedProfitPencentage / 100).ToString(), CellValues.Number);

                //Add image
                byte[] resizedPicture = NSLedRangePlanDef.ResizeImage(def.Picture, 90, 135);
                OpenXmlUtil.addImage(document, currentSheetId, resizedPicture, def.ItemNo, 3, itemStartingRow, 50000, 50000);

                //Set size info
                var rangePlanSizeList = AccountWorker.Instance.getNSLedRangePlanSizeOption(def.RangePlanId);
                int sizePositionIndex = 0;
                foreach (string sizeKey in rangePlanSizeList.Keys)
                {
                    var sizeRow = itemStartingRow + sizePositionIndex % 8;
                    var sizeCol = Convert.ToInt32(sizeTableStartCol + 2 * Math.Floor(sizePositionIndex / 8m));

                    OpenXmlUtil.setCellValue(document, currentSheetId, sizeRow, sizeCol, sizeKey, CellValues.SharedString);
                    OpenXmlUtil.setCellValue(document, currentSheetId, sizeRow, sizeCol + 1, ((decimal)rangePlanSizeList[sizeKey] / def.TotalQty).ToString(), CellValues.Number);

                    sizePositionIndex++;
                }
                //Remove style for the unused cell
                if (sizePositionIndex < 9)
                {
                    for (; sizePositionIndex < 16; sizePositionIndex++)
                    {
                        var sizeRow = itemStartingRow + sizePositionIndex % 8;
                        var sizeCol = Convert.ToInt32(sizeTableStartCol + 2 * Math.Floor(sizePositionIndex / 8m));
                        OpenXmlUtil.setCellStyle(document, currentSheetId, sizeRow, sizeCol, 0U);
                        OpenXmlUtil.setCellStyle(document, currentSheetId, sizeRow, sizeCol + 1, 0U);
                    }
                }
                #endregion

                #region Sales Data
                //Insert sales data
                //Prepare
                int lastSalesQty = 0, lastReturnsQty = 0, lastNetQty = 0, lastMDQty = 0;
                int weekSalesQty = 0, weekReturnsQty = 0, weekNetQty = 0;
                decimal lastComm = 0;
                decimal weekComm = 0;
                bool isMDHighlighted = false;

                int fromWeek = 0;
                int toWeek = 0;
                AccountManager.Instance.getNSLedRepeatItemParamRange(def.ItemNo, def.SeasonId, out fromWeek, out toWeek);

                var itemList = salesList.Where(x => x.ItemNo == def.ItemNo && (x.FiscalYear * 100 + x.FiscalWeek) >= fromWeek && (x.FiscalYear * 100 + x.FiscalWeek) < toWeek); //reduce list size
                List<decimal> fpSellThroughList = new List<decimal>();
                int lastFPYear = 0;
                int lastFPWeek = 0;

                //B/F 
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow, bfCol, (fiscalYear - 1) + " B/F", CellValues.SharedString);

                var bfSales = itemList.Where(x => x.FiscalYear < fiscalYear);
                if (bfSales.Count() > 0)
                {
                    lastSalesQty += bfSales.Sum(x => x.DespatchQty);
                    lastReturnsQty += bfSales.Sum(x => x.ReturnQty);
                    lastNetQty += bfSales.Sum(x => x.NetQty);
                    lastComm += bfSales.Sum(x => x.NSCommAmtInUSD);
                    lastMDQty += bfSales.Sum(x => x.MDQty);

                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, bfCol, lastSalesQty.ToString(), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, bfCol, lastReturnsQty.ToString(), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, bfCol, lastNetQty.ToString(), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, bfCol, lastComm.ToString(), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, bfCol, lastSalesQty > 0 ? (-lastReturnsQty / (decimal)lastSalesQty).ToString() : "0", CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, bfCol, (lastNetQty / (decimal)def.TotalQty).ToString(), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, bfCol, ((lastNetQty - lastMDQty) / (decimal)def.TotalQty).ToString(), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, bfCol, lastMDQty.ToString(), CellValues.Number);

                    fpSellThroughList.Add((decimal)((lastNetQty - lastMDQty) / (decimal)def.TotalQty));
                    lastFPYear = bfSales.Max(x => x.FiscalYear);
                    if (lastFPYear < fiscalYear - 1)
                    {
                        lastFPYear = fiscalYear - 1;
                        lastFPWeek = 46;
                    }
                    else if (lastFPYear == fiscalYear - 1)
                    {
                        var bfLYSales = bfSales.Where(x => x.FiscalYear == lastFPYear);
                        lastFPWeek = bfLYSales.Max(x => x.FiscalWeek);
                        
                        if (lastFPWeek >= 46)
                        {
                            for (int i = lastFPWeek; i <= 53; i++) // changed from 52 -> 53
                            {
                                fpSellThroughList.Add((bfSales.Where(x => (x.FiscalYear == lastFPYear && x.FiscalWeek <= i)).Sum(x => (int?)(x.NetQty - x.MDQty)) ?? 0) / (decimal)def.TotalQty);
                                  
                            }
                        }
                        else
                            lastFPWeek = 46;
                    }

                   
                    //if (lastMDQty > 0)
                    if (def.IsMD && (lastFPYear * 100 + lastFPWeek) == (def.MDYear * 100 + def.MDWeek))
                    {
                        OpenXmlUtil.setCellStyle(document, currentSheetId, itemStartingRow, bfCol, salesStyle);
                        isMDHighlighted = true;
                    }
                }

                //Check the first week of the data
                var firstWeekItems = itemList.Where(x => x.FiscalYear == fiscalYear);
                int startWeek = firstWeekItems.Count() == 0 ? 0 : firstWeekItems.Min(x => x.FiscalWeek);
                int currentCol = bfCol + 1;

                if (startWeek == 0) //No sales after new year
                {
                    for (int i = 1; i <= fiscalWeek; i++)
                    {
                        do
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, lastSalesQty > 0 ? (-lastReturnsQty / (decimal)lastSalesQty).ToString() : "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, (lastNetQty / (decimal)def.TotalQty).ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, ((lastNetQty - lastMDQty) / (decimal)def.TotalQty).ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, lastMDQty.ToString(), CellValues.Number);
                            currentCol++;
                        } while (summaryCol.Contains(currentCol)); //Repeat if need to enter summary
                    }
                    var tempCol = currentCol - 1; //Check if summary entered
                    currentCol = summaryCol.Where(x => x >= currentCol - 1).Min();

                    if (currentCol == tempCol) //Summary entered
                    {
                        currentCol++;
                    }
                    else //Summary not entered
                    {
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, lastSalesQty > 0 ? (-lastReturnsQty / (decimal)lastSalesQty).ToString() : "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, (lastNetQty / (decimal)def.TotalQty).ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, ((lastNetQty - lastMDQty) / (decimal)def.TotalQty).ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, lastMDQty.ToString(), CellValues.Number);
                        currentCol++;
                    }
                }
                else
                {
                    //Insert sales data
                    for (int i = 1; i <= 53; i++) // changed from 52 -> 53
                    {
                        //skip until first week, confirm no B/F records
                        if (i < startWeek && lastComm == 0)
                        {
                            currentCol++;
                            if (summaryCol.Contains(currentCol)) //Skip for P summary
                                currentCol++;
                            continue;
                        }
                        else if (i > fiscalWeek) //Quit loop for future week and update summary before exit
                        {
                            var tempCol = currentCol - 1; //for check if summary entered
                            currentCol = summaryCol.Where(x => x >= currentCol - 1).Min();

                            if (currentCol == tempCol)
                            {
                                currentCol++;
                                break;
                            }

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, weekSalesQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, weekReturnsQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, weekNetQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, weekComm.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, lastSalesQty > 0 ? (-lastReturnsQty / (decimal)lastSalesQty).ToString() : "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, (lastNetQty / (decimal)def.TotalQty).ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, ((lastNetQty - lastMDQty) / (decimal)def.TotalQty).ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, lastMDQty.ToString(), CellValues.Number);
                            currentCol++;
                            break;
                        }
                        //Get sales data, get the default 'zero' value for null case
                        NSLedSalesInfoDef sales = itemList.Where(x => x.FiscalYear == fiscalYear && x.FiscalWeek == i).FirstOrDefault() ?? new NSLedSalesInfoDef();

                        //Add summary
                        weekSalesQty += sales.DespatchQty;
                        weekReturnsQty += sales.ReturnQty;
                        weekNetQty += sales.NetQty;
                        weekComm += sales.NSCommAmtInUSD;

                        lastSalesQty += sales.DespatchQty;
                        lastReturnsQty += sales.ReturnQty;
                        lastNetQty += sales.NetQty;
                        lastComm += sales.NSCommAmtInUSD;
                        lastMDQty += sales.MDQty;

                        //Update info
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, sales.DespatchQty.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, sales.ReturnQty.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, sales.NetQty.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, sales.NSCommAmtInUSD.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, lastSalesQty > 0 ? (-lastReturnsQty / (decimal)lastSalesQty).ToString() : "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, (lastNetQty / (decimal)def.TotalQty).ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, ((lastNetQty - lastMDQty) / (decimal)def.TotalQty).ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, lastMDQty.ToString(), CellValues.Number);

                        fpSellThroughList.Add((decimal)((lastNetQty - lastMDQty) / (decimal)def.TotalQty));
                        lastFPYear = sales.FiscalYear;
                        lastFPWeek = sales.FiscalWeek;

                        //if (lastMDQty > 0 && !isMDHighlighted)
                        if (!isMDHighlighted && def.IsMD && (lastFPYear * 100 + lastFPWeek) == (def.MDYear * 100 + def.MDWeek))
                        {
                            OpenXmlUtil.setCellStyle(document, currentSheetId, itemStartingRow, currentCol, salesStyle);
                            isMDHighlighted = true;
                        }
                        currentCol++;
                        //Update summary
                        if (summaryCol.Contains(currentCol))
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, weekSalesQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, weekReturnsQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, weekNetQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, weekComm.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, lastSalesQty > 0 ? (-lastReturnsQty / (decimal)lastSalesQty).ToString() : "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, (lastNetQty / (decimal)def.TotalQty).ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, ((lastNetQty - lastMDQty) / (decimal)def.TotalQty).ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, lastMDQty.ToString(), CellValues.Number);

                            weekSalesQty = 0;
                            weekReturnsQty = 0;
                            weekNetQty = 0;
                            weekComm = 0;
                            currentCol++;
                        }
                    }
                }

                //Show 2 phase for 52 week exit
                var previousCol = summaryCol.Where(x => x < currentCol - 2).Max();
                /*
                var columns = ((WorksheetPart)document.WorkbookPart.GetPartById(currentSheetId))
                                .Worksheet
                                .GetFirstChild<Columns>()
                                .Descendants<Column>()
                                .Where(c => c.Max == Convert.ToUInt32(currentCol) - 2 || c.Max == (previousCol == 0 ? 0 : Convert.ToUInt32(previousCol) - 1));

                */
                var columns = ((WorksheetPart)document.WorkbookPart.GetPartById(currentSheetId))
                                .Worksheet
                                .GetFirstChild<Columns>()
                                .Descendants<Column>()
                                .Where(c => c.Max == Convert.ToUInt32(currentCol) - 1 || c.Max == (previousCol == 0 ? 0 : Convert.ToUInt32(previousCol) - 0));

                foreach (var column in columns)
                    column.Hidden = false;

                //Update cum total
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, cumCol, lastSalesQty.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, cumCol, lastReturnsQty.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, cumCol, lastNetQty.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, cumCol, lastComm.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, cumCol, lastSalesQty > 0 ? (-lastReturnsQty / (decimal)lastSalesQty).ToString() : "0", CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, cumCol, (lastNetQty / (decimal)def.TotalQty).ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, cumCol, ((lastNetQty - lastMDQty) / (decimal)def.TotalQty).ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, cumCol, lastMDQty.ToString(), CellValues.Number);
                #endregion

                #region Profit model
                //Update profit summary
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, profitCol, lastComm.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, profitCol, (-def.TotalProductionCost).ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, profitCol, (lastComm - def.TotalProductionCost).ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, profitCol + 1, lastComm > 0 ? (1 - def.TotalProductionCost / lastComm).ToString() : "0", CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, profitCol, (0.7).ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, profitCol, def.TotalEstimatedComm.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, profitCol, (def.TotalEstimatedComm - def.TotalProductionCost).ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, profitCol + 1, (def.EstimatedProfitPencentage / 100).ToString(), CellValues.Number);
                #endregion

                #region Forecast model

                int year_counter = fiscalYear;
                int week_counter = fiscalWeek;
                int cnt = 0;

                if (lastFPYear > 0)
                {
                    while ((year_counter > lastFPYear || week_counter > lastFPWeek) && cnt < 6)
                    {
                        fpSellThroughList.Add(0);
                        cnt += 1;
                        int noOfWeek = CommonUtil.getWeekCountByFiscalPeriod(year_counter, 12);
                        noOfWeek = (noOfWeek == 5 ? 53 : 52);

                        if (year_counter != lastFPYear && week_counter > 1)
                            week_counter -= 1;
                        else if (year_counter != fiscalYear && week_counter == 1)
                        {
                            week_counter = noOfWeek;
                            year_counter -= 1;
                        }
                        else
                            week_counter -= 1;
                    }
                }

                int noOfWeekUntilNextSale = def.NoOfWeekUntilNextMD;

                /*
                int noOfWeekUntilNextSale = 0;

                //SS MSS,SS EOS,AW MOS,AW EOS, 8,23,34,48,60 = [52+8]
                int[] mdWeekNo = new int[5] { 8, 23, 34, 48, 60 };
                for (int z = 0; z < mdWeekNo.Length; z++)
                {
                    if (mdWeekNo[z] > fiscalWeek)
                    {
                        noOfWeekUntilNextSale = mdWeekNo[z] - fiscalWeek;
                        break;
                    }
                }
                */

                decimal totalPercent = 0;
                decimal growthRate = 0;
                if (fpSellThroughList.Count <= 5)
                {
                    growthRate = 0.03m;

                    if (fpSellThroughList.Count > 0 && (growthRate * noOfWeekUntilNextSale + fpSellThroughList[fpSellThroughList.Count - 1]) > 1)
                        growthRate = (1 - fpSellThroughList[fpSellThroughList.Count - 1]) / noOfWeekUntilNextSale;
                }
                else
                {
                    for (int i = 1; i < 4; i++)
                    {
                        totalPercent += (fpSellThroughList[fpSellThroughList.Count - i] - fpSellThroughList[fpSellThroughList.Count - i - 1]);
                    }
                    if (totalPercent <= 0)
                    {
                        growthRate = 0;
                    }
                    else if ((totalPercent / 3 * noOfWeekUntilNextSale + fpSellThroughList[fpSellThroughList.Count - 1]) > 1)
                    {
                        growthRate = (1 - fpSellThroughList[fpSellThroughList.Count - 1]) / noOfWeekUntilNextSale;
                    }
                    else
                    {
                        growthRate = totalPercent / 3;
                    }
                }

                decimal assumedFPSellThru = growthRate * noOfWeekUntilNextSale + (fpSellThroughList.Count > 0 ? fpSellThroughList[fpSellThroughList.Count - 1] : 0);
                decimal assumedMDSellThru = (1 - assumedFPSellThru) * markDownRecovery;
                decimal actualMDSellThru = def.TotalQty == 0 ? 0 : ((decimal)itemList.Sum(x => x.MDQty) / def.TotalQty);

                decimal fcstPortionFPComm = (def.AvgRetailSellingPrice * noOfWeekUntilNextSale * growthRate * def.TotalQty / def.VATFactor * (decimal)seasonalExchangeRate * 0.5m);
                decimal fcstPortionMDComm = ((def.AvgRetailSellingPrice / 2) * def.TotalQty * ((assumedMDSellThru - actualMDSellThru) > 0 ? (assumedMDSellThru - actualMDSellThru) : 0) / def.VATFactor * (decimal)seasonalExchangeRate * 0.6m);
                decimal totalEstimateComm = fcstPortionFPComm + (def.AvgRetailSellingPrice * def.TotalQty * fpSellThroughList.Count > 0 ? fpSellThroughList[fpSellThroughList.Count - 1] : 0 / def.VATFactor * (decimal)seasonalExchangeRate * 0.5m) + fcstPortionMDComm;
                decimal totalCost = (decimal)def.TotalProductionCost;
                decimal totalProfit = lastComm + totalEstimateComm - totalCost;

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow, forecastCol, (lastMDQty == 0 ? (noOfWeekUntilNextSale) : 0).ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, forecastCol, growthRate.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, forecastCol, assumedFPSellThru.ToString("0.00"), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, forecastCol, assumedMDSellThru.ToString("0.00"), CellValues.Number, OpenXmlUtil.getCellStyleId(document, currentSheetId, itemStartingRow + 2, forecastCol));

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, forecastCol, fcstPortionFPComm.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, forecastCol, fcstPortionMDComm.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, forecastCol, totalEstimateComm.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, forecastCol, totalCost.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, forecastCol, totalProfit.ToString(), CellValues.Number);

                #endregion

                //Prepare for next item
                itemStartingRow += 10;
                itemCount++;
                lastWeekCol = currentCol - 2;
                if (itemCount == 10 && profitList.Where(x => x.ProductTeamName == def.ProductTeamName).Count() > 10)
                {
                    OpenXmlUtil.insertHorizontalPageBreak(document, currentSheetId, itemStartingRow - 1);
                    itemCount = 0;
                }
                #endregion

                #region Summary
                if (itemNextIndex >= profitList.Count || currentOfficeId != profitList[itemNextIndex].OfficeId)
                {
                    if (!string.IsNullOrEmpty(currentOfficeCode))
                    {
                        //summary duty
                        int sumLastSalesQty = 0, sumLastReturnsQty = 0, sumLastNetQty = 0;
                        int sumWeekSalesQty = 0, sumWeekReturnsQty = 0, sumWeekNetQty = 0;
                        decimal sumLastComm = 0;
                        decimal sumWeekComm = 0;

                        itemStartingRow += 3;
                        for (int i = 1; i <= 7; i++)
                        {
                            OpenXmlUtil.copyAndInsertRow(document, tempSheetId, i + 10, currentSheetId, itemStartingRow + i);
                        }
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, 2, currentOfficeCode + " (Duty)", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, 2, currentOfficeCode + " (Duty)", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, 2, currentOfficeCode + " (Duty)", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, 2, currentOfficeCode + " (Duty)", CellValues.SharedString);

                        var bfSummDutySalesByOfficeList = (from s in salesList
                                                           where s.FiscalYear < fiscalYear
                                                           where s.HasDuty == true
                                                           where s.OfficeId == currentOfficeId
                                                           group s by new
                                                           {
                                                               s.OfficeId
                                                           } into officeCommList
                                                           select new
                                                           {
                                                               OfficeId = officeCommList.First().OfficeId,
                                                               SalesQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.DespatchQty),
                                                               ReturnQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.ReturnQty),
                                                               NetQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.NetQty),
                                                               NSCommAmt = officeCommList.Sum((NSLedSalesInfoDef c) => c.NSCommAmtInUSD)
                                                           });

                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, bfCol, (fiscalYear - 1) + " B/F", CellValues.SharedString);

                        foreach (var r in bfSummDutySalesByOfficeList)
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, bfCol, r.SalesQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, bfCol, r.ReturnQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, bfCol, r.NetQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, bfCol, r.NSCommAmt.ToString(), CellValues.Number);

                            sumLastSalesQty += r.SalesQty;
                            sumLastReturnsQty += r.ReturnQty;
                            sumLastNetQty += r.NetQty;
                            sumLastComm += r.NSCommAmt;
                        }
                        //bf end
                        var YearCommListByOfficeByWeekList = (from s in salesList
                                                              where s.FiscalYear == fiscalYear
                                                              where s.OfficeId == currentOfficeId
                                                              where s.HasDuty == true //duty
                                                              group s by new
                                                              {
                                                                  s.FiscalYear,
                                                                  s.FiscalPeriod,
                                                                  s.FiscalWeek,
                                                                  s.OfficeId
                                                              } into officeCommList
                                                              select new
                                                              {
                                                                  FiscalYear = officeCommList.First().FiscalYear,
                                                                  FiscalPeriod = officeCommList.First().FiscalPeriod,
                                                                  FiscalWeek = officeCommList.First().FiscalWeek,
                                                                  OfficeId = officeCommList.First().OfficeId,
                                                                  SalesQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.DespatchQty),
                                                                  ReturnQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.ReturnQty),
                                                                  NetQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.NetQty),
                                                                  NSCommAmt = officeCommList.Sum((NSLedSalesInfoDef c) => c.NSCommAmtInUSD)
                                                              }).OrderBy(item => item.FiscalYear).OrderBy(item => item.FiscalWeek).OrderBy(item => item.OfficeId);

                        //Check the first week of the data
                        var sumFirstWeekItems = YearCommListByOfficeByWeekList.Where(x => x.FiscalYear == fiscalYear);
                        int sumStartWeek = sumFirstWeekItems.Count() == 0 ? 0 : sumFirstWeekItems.Min(x => x.FiscalWeek);
                        int sumCurrentCol = bfCol + 1;

                        if (sumStartWeek == 0) //No sales after new year
                        {
                            for (int i = 1; i <= fiscalWeek; i++)
                            {
                                do
                                {
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, "0", CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, "0", CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, "0", CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, "0", CellValues.Number);
                                    sumCurrentCol++;
                                } while (summaryCol.Contains(sumCurrentCol)); //Repeat if need to enter summary
                            }
                            var tempCol = sumCurrentCol - 1; //Check if summary entered
                            sumCurrentCol = summaryCol.Where(x => x >= sumCurrentCol - 1).Min();

                            if (sumCurrentCol == tempCol) //Summary entered
                            {
                                sumCurrentCol++;
                            }
                            else //Summary not entered
                            {
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, "0", CellValues.Number);
                                sumCurrentCol++;
                            }
                        }
                        else
                        {
                            //Insert sales data
                            for (int i = 1; i <= 53; i++) // changed from 52 to 53
                            {
                                //skip until first week, confirm no B/F records
                                if (i < sumStartWeek)
                                {
                                    sumCurrentCol++;
                                    if (summaryCol.Contains(sumCurrentCol)) //Skip for P summary
                                        sumCurrentCol++;
                                    continue;
                                }
                                else if (i > fiscalWeek) //Quit loop for future week and update summary before exit
                                {
                                    var tempCol = sumCurrentCol - 1; //for check if summary entered
                                    sumCurrentCol = summaryCol.Where(x => x >= sumCurrentCol - 1).Min();

                                    if (sumCurrentCol == tempCol)
                                    {
                                        sumCurrentCol++;
                                        break;
                                    }

                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, sumWeekSalesQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, sumWeekReturnsQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, sumWeekNetQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, sumWeekComm.ToString(), CellValues.Number);
                                    sumCurrentCol++;
                                    break;
                                }
                                //Get sales data, get the default 'zero' value for null case
                                var sales = YearCommListByOfficeByWeekList.Where(x => x.FiscalYear == fiscalYear && x.FiscalWeek == i).FirstOrDefault();

                                if (sales != null)
                                {
                                    //Add summary
                                    sumWeekSalesQty += sales.SalesQty;
                                    sumWeekReturnsQty += sales.ReturnQty;
                                    sumWeekNetQty += sales.NetQty;
                                    sumWeekComm += sales.NSCommAmt;

                                    sumLastSalesQty += sales.SalesQty;
                                    sumLastReturnsQty += sales.ReturnQty;
                                    sumLastNetQty += sales.NetQty;
                                    sumLastComm += sales.NSCommAmt;

                                    //Update info
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, sales.SalesQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, sales.ReturnQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, sales.NetQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, sales.NSCommAmt.ToString(), CellValues.Number);
                                }
                                sumCurrentCol++;

                                //Update summary
                                if (summaryCol.Contains(sumCurrentCol))
                                {
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, sumWeekSalesQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, sumWeekReturnsQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, sumWeekNetQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, sumWeekComm.ToString(), CellValues.Number);

                                    sumWeekSalesQty = 0;
                                    sumWeekReturnsQty = 0;
                                    sumWeekNetQty = 0;
                                    sumWeekComm = 0;
                                    sumCurrentCol++;
                                }
                            }

                            //Update cum total
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, cumCol, sumLastSalesQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, cumCol, sumLastReturnsQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, cumCol, sumLastNetQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, cumCol, sumLastComm.ToString(), CellValues.Number);
                        }

                        //summary non-duty
                        sumLastSalesQty = sumLastReturnsQty = sumLastNetQty = 0;
                        sumWeekSalesQty = sumWeekReturnsQty = sumWeekNetQty = 0;
                        sumLastComm = 0;
                        sumWeekComm = 0;

                        itemStartingRow += 7;
                        for (int i = 1; i <= 7; i++)
                        {
                            OpenXmlUtil.copyAndInsertRow(document, tempSheetId, i + 10, currentSheetId, itemStartingRow + i);
                        }
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, 2, " ", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, 2, currentOfficeCode + " (non-duty)", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, 2, currentOfficeCode + " (non-duty)", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, 2, currentOfficeCode + " (non-duty)", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, 2, currentOfficeCode + " (non-duty)", CellValues.SharedString);

                        bfSummDutySalesByOfficeList = (from s in salesList
                                                       where s.FiscalYear < fiscalYear
                                                       where s.OfficeId == currentOfficeId
                                                       where s.HasDuty == false
                                                       group s by new
                                                       {
                                                           s.OfficeId
                                                       } into officeCommList
                                                       select new
                                                       {
                                                           OfficeId = officeCommList.First().OfficeId,
                                                           SalesQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.DespatchQty),
                                                           ReturnQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.ReturnQty),
                                                           NetQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.NetQty),
                                                           NSCommAmt = officeCommList.Sum((NSLedSalesInfoDef c) => c.NSCommAmtInUSD)
                                                       });

                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, bfCol, (fiscalYear - 1) + " B/F", CellValues.SharedString);

                        foreach (var r in bfSummDutySalesByOfficeList)
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, bfCol, r.SalesQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, bfCol, r.ReturnQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, bfCol, r.NetQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, bfCol, r.NSCommAmt.ToString(), CellValues.Number);

                            sumLastSalesQty += r.SalesQty;
                            sumLastReturnsQty += r.ReturnQty;
                            sumLastNetQty += r.NetQty;
                            sumLastComm += r.NSCommAmt;
                        }
                        //bf end

                        YearCommListByOfficeByWeekList = (from s in salesList
                                                          where s.FiscalYear == fiscalYear
                                                          where s.OfficeId == currentOfficeId
                                                          where s.HasDuty == false //non-duty
                                                          group s by new
                                                          {
                                                              s.FiscalYear,
                                                              s.FiscalPeriod,
                                                              s.FiscalWeek,
                                                              s.OfficeId
                                                          } into officeCommList
                                                          select new
                                                          {
                                                              FiscalYear = officeCommList.First().FiscalYear,
                                                              FiscalPeriod = officeCommList.First().FiscalPeriod,
                                                              FiscalWeek = officeCommList.First().FiscalWeek,
                                                              OfficeId = officeCommList.First().OfficeId,
                                                              SalesQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.DespatchQty),
                                                              ReturnQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.ReturnQty),
                                                              NetQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.NetQty),
                                                              NSCommAmt = officeCommList.Sum((NSLedSalesInfoDef c) => c.NSCommAmtInUSD)
                                                          }).OrderBy(item => item.FiscalYear).OrderBy(item => item.FiscalWeek).OrderBy(item => item.OfficeId);

                        //Check the first week of the data
                        sumFirstWeekItems = YearCommListByOfficeByWeekList.Where(x => x.FiscalYear == fiscalYear);
                        sumStartWeek = sumFirstWeekItems.Count() == 0 ? 0 : sumFirstWeekItems.Min(x => x.FiscalWeek);
                        sumCurrentCol = bfCol + 1;

                        if (sumStartWeek == 0) //No sales after new year
                        {
                            for (int i = 1; i <= fiscalWeek; i++)
                            {
                                do
                                {
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, "0", CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, "0", CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, "0", CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, "0", CellValues.Number);
                                    sumCurrentCol++;
                                } while (summaryCol.Contains(sumCurrentCol)); //Repeat if need to enter summary
                            }
                            var tempCol = sumCurrentCol - 1; //Check if summary entered
                            sumCurrentCol = summaryCol.Where(x => x >= sumCurrentCol - 1).Min();

                            if (sumCurrentCol == tempCol) //Summary entered
                            {
                                sumCurrentCol++;
                            }
                            else //Summary not entered
                            {
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, "0", CellValues.Number);
                                sumCurrentCol++;
                            }
                        }
                        else
                        {
                            //Insert sales data
                            for (int i = 1; i <= 53; i++) // changed from 52 to 53
                            {
                                //skip until first week, confirm no B/F records
                                if (i < sumStartWeek)
                                {
                                    sumCurrentCol++;
                                    if (summaryCol.Contains(sumCurrentCol)) //Skip for P summary
                                        sumCurrentCol++;
                                    continue;
                                }
                                else if (i > fiscalWeek) //Quit loop for future week and update summary before exit
                                {
                                    var tempCol = sumCurrentCol - 1; //for check if summary entered
                                    sumCurrentCol = summaryCol.Where(x => x >= sumCurrentCol - 1).Min();

                                    if (sumCurrentCol == tempCol)
                                    {
                                        sumCurrentCol++;
                                        break;
                                    }

                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, sumWeekSalesQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, sumWeekReturnsQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, sumWeekNetQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, sumWeekComm.ToString(), CellValues.Number);
                                    sumCurrentCol++;
                                    break;
                                }
                                //Get sales data, get the default 'zero' value for null case
                                var sales = YearCommListByOfficeByWeekList.Where(x => x.FiscalYear == fiscalYear && x.FiscalWeek == i).FirstOrDefault();

                                if (sales != null)
                                {
                                    //Add summary
                                    sumWeekSalesQty += sales.SalesQty;
                                    sumWeekReturnsQty += sales.ReturnQty;
                                    sumWeekNetQty += sales.NetQty;
                                    sumWeekComm += sales.NSCommAmt;

                                    sumLastSalesQty += sales.SalesQty;
                                    sumLastReturnsQty += sales.ReturnQty;
                                    sumLastNetQty += sales.NetQty;
                                    sumLastComm += sales.NSCommAmt;

                                    //Update info
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, sales.SalesQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, sales.ReturnQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, sales.NetQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, sales.NSCommAmt.ToString(), CellValues.Number);
                                }
                                sumCurrentCol++;

                                //Update summary
                                if (summaryCol.Contains(sumCurrentCol))
                                {
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, sumWeekSalesQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, sumWeekReturnsQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, sumWeekNetQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, sumWeekComm.ToString(), CellValues.Number);

                                    sumWeekSalesQty = 0;
                                    sumWeekReturnsQty = 0;
                                    sumWeekNetQty = 0;
                                    sumWeekComm = 0;
                                    sumCurrentCol++;
                                }
                            }

                            //Update cum total
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, cumCol, sumLastSalesQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, cumCol, sumLastReturnsQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, cumCol, sumLastNetQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, cumCol, sumLastComm.ToString(), CellValues.Number);
                        }


                        //summary total
                        sumLastSalesQty = sumLastReturnsQty = sumLastNetQty = 0;
                        sumWeekSalesQty = sumWeekReturnsQty = sumWeekNetQty = 0;
                        sumLastComm = 0;
                        sumWeekComm = 0;

                        itemStartingRow += 7;
                        for (int i = 1; i <= 7; i++)
                        {
                            OpenXmlUtil.copyAndInsertRow(document, tempSheetId, i + 10, currentSheetId, itemStartingRow + i);
                        }
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, 2, " ", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, 2, "Total", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, 2, "Total", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, 2, "Total", CellValues.SharedString);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, 2, "Total", CellValues.SharedString);

                        bfSummDutySalesByOfficeList = (from s in salesList
                                                       where s.FiscalYear < fiscalYear
                                                       where s.OfficeId == currentOfficeId
                                                       group s by new
                                                       {
                                                           s.OfficeId
                                                       } into officeCommList
                                                       select new
                                                       {
                                                           OfficeId = officeCommList.First().OfficeId,
                                                           SalesQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.DespatchQty),
                                                           ReturnQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.ReturnQty),
                                                           NetQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.NetQty),
                                                           NSCommAmt = officeCommList.Sum((NSLedSalesInfoDef c) => c.NSCommAmtInUSD)
                                                       });

                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, bfCol, (fiscalYear - 1) + " B/F", CellValues.SharedString);

                        foreach (var r in bfSummDutySalesByOfficeList)
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, bfCol, r.SalesQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, bfCol, r.ReturnQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, bfCol, r.NetQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, bfCol, r.NSCommAmt.ToString(), CellValues.Number);
                        }
                        //bf end
                        YearCommListByOfficeByWeekList = (from s in salesList
                                                          where s.FiscalYear == fiscalYear
                                                          where s.OfficeId == currentOfficeId
                                                          group s by new
                                                          {
                                                              s.FiscalYear,
                                                              s.FiscalPeriod,
                                                              s.FiscalWeek,
                                                              s.OfficeId
                                                          } into officeCommList
                                                          select new
                                                          {
                                                              FiscalYear = officeCommList.First().FiscalYear,
                                                              FiscalPeriod = officeCommList.First().FiscalPeriod,
                                                              FiscalWeek = officeCommList.First().FiscalWeek,
                                                              OfficeId = officeCommList.First().OfficeId,
                                                              SalesQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.DespatchQty),
                                                              ReturnQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.ReturnQty),
                                                              NetQty = officeCommList.Sum((NSLedSalesInfoDef c) => c.NetQty),
                                                              NSCommAmt = officeCommList.Sum((NSLedSalesInfoDef c) => c.NSCommAmtInUSD)
                                                          }).OrderBy(item => item.FiscalYear).OrderBy(item => item.FiscalWeek).OrderBy(item => item.OfficeId);

                        //Check the first week of the data
                        sumFirstWeekItems = YearCommListByOfficeByWeekList.Where(x => x.FiscalYear == fiscalYear);
                        sumStartWeek = sumFirstWeekItems.Count() == 0 ? 0 : sumFirstWeekItems.Min(x => x.FiscalWeek);
                        sumCurrentCol = bfCol + 1;

                        if (sumStartWeek == 0) //No sales after new year
                        {
                            for (int i = 1; i <= fiscalWeek; i++)
                            {
                                do
                                {
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, "0", CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, "0", CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, "0", CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, "0", CellValues.Number);
                                    sumCurrentCol++;
                                } while (summaryCol.Contains(sumCurrentCol)); //Repeat if need to enter summary
                            }
                            var tempCol = sumCurrentCol - 1; //Check if summary entered
                            sumCurrentCol = summaryCol.Where(x => x >= sumCurrentCol - 1).Min();

                            if (sumCurrentCol == tempCol) //Summary entered
                            {
                                sumCurrentCol++;
                            }
                            else //Summary not entered
                            {
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, "0", CellValues.Number);
                                sumCurrentCol++;
                            }
                        }
                        else
                        {
                            //Insert sales data
                            for (int i = 1; i <= 53; i++) // changed from 52 to 53
                            {
                                //skip until first week, confirm no B/F records
                                if (i < sumStartWeek)
                                {
                                    sumCurrentCol++;
                                    if (summaryCol.Contains(sumCurrentCol)) //Skip for P summary
                                        sumCurrentCol++;
                                    continue;
                                }
                                else if (i > fiscalWeek) //Quit loop for future week and update summary before exit
                                {
                                    var tempCol = sumCurrentCol - 1; //for check if summary entered
                                    sumCurrentCol = summaryCol.Where(x => x >= sumCurrentCol - 1).Min();

                                    if (sumCurrentCol == tempCol)
                                    {
                                        sumCurrentCol++;
                                        break;
                                    }

                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, sumWeekSalesQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, sumWeekReturnsQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, sumWeekNetQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, sumWeekComm.ToString(), CellValues.Number);
                                    sumCurrentCol++;
                                    break;
                                }
                                //Get sales data, get the default 'zero' value for null case
                                var sales = YearCommListByOfficeByWeekList.Where(x => x.FiscalYear == fiscalYear && x.FiscalWeek == i).FirstOrDefault();

                                if (sales != null)
                                {
                                    //Add summary
                                    sumWeekSalesQty += sales.SalesQty;
                                    sumWeekReturnsQty += sales.ReturnQty;
                                    sumWeekNetQty += sales.NetQty;
                                    sumWeekComm += sales.NSCommAmt;

                                    sumLastSalesQty += sales.SalesQty;
                                    sumLastReturnsQty += sales.ReturnQty;
                                    sumLastNetQty += sales.NetQty;
                                    sumLastComm += sales.NSCommAmt;

                                    //Update info
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, sales.SalesQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, sales.ReturnQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, sales.NetQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, sales.NSCommAmt.ToString(), CellValues.Number);
                                }
                                sumCurrentCol++;

                                //Update summary
                                if (summaryCol.Contains(sumCurrentCol))
                                {
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, sumCurrentCol, sumWeekSalesQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, sumCurrentCol, sumWeekReturnsQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, sumCurrentCol, sumWeekNetQty.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, sumCurrentCol, sumWeekComm.ToString(), CellValues.Number);

                                    sumWeekSalesQty = 0;
                                    sumWeekReturnsQty = 0;
                                    sumWeekNetQty = 0;
                                    sumWeekComm = 0;
                                    sumCurrentCol++;
                                }
                            }

                            //Update cum total
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, cumCol, sumLastSalesQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, cumCol, sumLastReturnsQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, cumCol, sumLastNetQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, cumCol, sumLastComm.ToString(), CellValues.Number);
                        }
                    }
                }

                #endregion

                itemNextIndex++;
            }

            #region Ending Phase
            //Change wording for SUMM


            OpenXmlUtil.setCellValue(document, summSheetId, "D4", "Commission " + (fiscalYear - 1).ToString() + " B/F", CellValues.SharedString, 0);
            OpenXmlUtil.setCellValue(document, summSheetId, "D6", "Commission " + fiscalYear.ToString(), CellValues.SharedString, 0);
            OpenXmlUtil.setCellValue(document, summSheetId, "B8", "Commission for " + fiscalYear.ToString(), CellValues.SharedString, 0);
            string phasing = (ckb_StillSelling.Checked ? ", " + ckb_StillSelling.Text : "") + (ckb_NotYetLaunched.Checked ? ", " + ckb_NotYetLaunched.Text : "") + (ckb_EndOfLife.Checked ? ", " + ckb_EndOfLife.Text : "");
            phasing = (phasing != "" ? " - " + phasing.Substring(2) : "");
            phasing = ddl_Phase.SelectedValue + (ckb_UnknownSeason.Checked ? " (includes " + ckb_UnknownSeason.Text + ")" : "") + phasing;
            OpenXmlUtil.setCellValue(document, summSheetId, "B3", "Data starting from phase " + phasing, CellValues.SharedString, 0);


            var bfCommListByOfficeList = (from s in salesList
                                          where s.FiscalYear < fiscalYear
                                          group s by new
                                          {
                                              s.OfficeId
                                          } into officeCommList
                                          select new { OfficeId = officeCommList.First().OfficeId, NSCommAmt = officeCommList.Sum((NSLedSalesInfoDef c) => c.NSCommAmtInUSD) });

            foreach (var r in bfCommListByOfficeList)
            {
                OpenXmlUtil.setCellValue(document, summSheetId, 4, this.getSummaryOfficeColId(r.OfficeId), r.NSCommAmt.ToString(), CellValues.Number);
            }

            var ytdCommListByOfficeList = (from s in salesList
                                           group s by new
                                           {
                                               s.OfficeId
                                           } into officeCommList
                                           select new { OfficeId = officeCommList.First().OfficeId, NSCommAmt = officeCommList.Sum((NSLedSalesInfoDef c) => c.NSCommAmtInUSD) });

            foreach (var r in ytdCommListByOfficeList)
            {
                OpenXmlUtil.setCellValue(document, summSheetId, 5, this.getSummaryOfficeColId(r.OfficeId), r.NSCommAmt.ToString(), CellValues.Number);
            }

            int summaryStartingRowId = 12;
            int rowId = summaryStartingRowId;
            int currentWeekNoWithinPeriod = 1;

            var currentYearCommListByOfficeByWeekList = (from s in salesList
                                                         where s.FiscalYear == fiscalYear
                                                         group s by new
                                                         {
                                                             s.FiscalYear,
                                                             s.FiscalPeriod,
                                                             s.FiscalWeek,
                                                             s.OfficeId
                                                         } into officeCommList
                                                         select new
                                                         {
                                                             FiscalYear = officeCommList.First().FiscalYear,
                                                             FiscalPeriod = officeCommList.First().FiscalPeriod,
                                                             FiscalWeek = officeCommList.First().FiscalWeek,
                                                             OfficeId = officeCommList.First().OfficeId,
                                                             NSCommAmt = officeCommList.Sum((NSLedSalesInfoDef c) => c.NSCommAmtInUSD)
                                                         }).OrderBy(item => item.FiscalYear).OrderBy(item => item.FiscalWeek).OrderBy(item => item.OfficeId);


            // build period - week template in summary sheet

            Hashtable periodRowTable = new Hashtable();

            for (int i = 1; i <= fiscalWeek; i++)
            {
                int p = CommonUtil.getFiscalPeriodByFiscalWeek(i);
                int c = CommonUtil.getWeekCountByFiscalPeriod(fiscalYear, p);
                AccountFinancialCalenderDef calDef = CommonUtil.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, p);
                PeriodWeekInfoDef fiscalWeekByDate = CommonUtil.getFiscalWeekByDate(calDef.StartDate);


                periodRowTable.Add(i, rowId);
                OpenXmlUtil.setCellValue(document, summSheetId, rowId, 2, p.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, summSheetId, rowId, 3, i.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, summSheetId, rowId, 4, "Commission", CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, summSheetId, rowId, 14, "=SUM(" + OpenXmlUtil.getColumnLetter(5) + rowId.ToString() + ":" + OpenXmlUtil.getColumnLetter(13) + rowId.ToString() + ")", CellValues.String);
                OpenXmlUtil.setCellValue(document, summSheetId, rowId, 15, "=" + OpenXmlUtil.getColumnLetter(14) + rowId.ToString() + "/1000", CellValues.String);

                rowId += 1;

                currentWeekNoWithinPeriod += 1;
                if (currentWeekNoWithinPeriod > c)
                {
                    currentWeekNoWithinPeriod = 1;
                    rowId += 1;
                }

            }

            rowId += 2;
            OpenXmlUtil.setCellValue(document, summSheetId, rowId, 4, fiscalYear.ToString() + " Total", CellValues.SharedString);
            for (int i = 5; i <= 13; i++)
            {
                OpenXmlUtil.setCellValue(document, summSheetId, rowId, i, "=SUM(" + OpenXmlUtil.getColumnLetter(i) + summaryStartingRowId.ToString() + ":" + OpenXmlUtil.getColumnLetter(i) + (rowId - 1).ToString() + ")", CellValues.String);
                OpenXmlUtil.setCellValue(document, summSheetId, rowId + 1, i, "=" + OpenXmlUtil.getColumnLetter(i) + rowId.ToString() + "-" + OpenXmlUtil.getColumnLetter(i) + (6).ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, summSheetId, rowId + 1, 14, "=" + OpenXmlUtil.getColumnLetter(14) + rowId.ToString() + "-" + OpenXmlUtil.getColumnLetter(14) + (6).ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, summSheetId, rowId + 1, 15, "=" + OpenXmlUtil.getColumnLetter(15) + rowId.ToString() + "-" + OpenXmlUtil.getColumnLetter(15) + (6).ToString(), CellValues.String);

                OpenXmlUtil.setCellValue(document, summSheetId, rowId, 14, "=SUM(" + OpenXmlUtil.getColumnLetter(5) + rowId.ToString() + ":" + OpenXmlUtil.getColumnLetter(13) + rowId.ToString() + ")", CellValues.String);
                OpenXmlUtil.setCellValue(document, summSheetId, rowId, 15, "=" + OpenXmlUtil.getColumnLetter(14) + rowId.ToString() + "/1000", CellValues.String);


            }



            foreach (var r in currentYearCommListByOfficeByWeekList)
            {
                OpenXmlUtil.setCellValue(document, summSheetId, (int)periodRowTable[r.FiscalWeek], this.getSummaryOfficeColId(r.OfficeId), r.NSCommAmt.ToString(), CellValues.Number);
            }


            ArrayList officeList = CommonUtil.getOfficeList();
            foreach (OfficeRef oRef in officeList)
            {
                if (!this.uclOfficeSelection.getOfficeList().contains(oRef.OfficeId))
                {
                    string sheetId = OpenXmlUtil.getWorksheetId(document, OfficeId.getName(oRef.OfficeId) + " Sales Summ");
                    if (sheetId != string.Empty)
                        document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == sheetId).Remove();
                }
            }

            //delete temp sheet for output

            /*
            document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == OpenXmlUtil.getWorksheetId(document, "SUMM")).Remove();
            document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == OpenXmlUtil.getWorksheetId(document, "SUMM-Obsolete")).Remove();
            */

            document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == tempSheetId).Remove();

            OpenXmlUtil.saveComplete(document);
            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
            #endregion
        }


        private int getSummaryOfficeColId(int officeId)
        {
            int colId = 0;

            switch (officeId)
            {
                case 1:
                    colId = 5;
                    break;
                case 19:
                    colId = 6;
                    break;
                case 2:
                    colId = 7;
                    break;
                case 16:
                    colId = 8;
                    break;
                case 7:
                    colId = 9;
                    break;
                case 9:
                    colId = 10;
                    break;
                case 3:
                    colId = 11;
                    break;
                case 8:
                    colId = 12;
                    break;
                case 13:
                    colId = 13;
                    break;

            }
            return colId;

        }
    }
}