using com.next.common.domain;
using com.next.common.web.commander;
using com.next.infra.util;
using com.next.isam.appserver.account;
using com.next.isam.domain.account;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.isam.appserver.common;
using com.next.isam.domain.common;
using com.next.common.domain.types;

namespace com.next.isam.webapp.reporter
{
    public partial class NSLedSellThruHistory : com.next.isam.webapp.usercontrol.PageTemplate
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
            uclOfficeSelection.UserId = LogonUserId;
            /*
            ddlOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--ALL--", GeneralCriteria.ALL.ToString());
            */

            //Get Year List
            List<NSLedImportFileDef> fiscalYearList = AccountManager.Instance.getNSLedFiscalYearList();
            for (int i = 0; i < fiscalYearList.Count; i++)
            {
                ddl_Year.Items.Add(new ListItem(fiscalYearList[i].FiscalYear.ToString()));
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
            List<NSLedImportFileDef> fiscalWeekList = AccountManager.Instance.getNSLedFiscalWeekList(int.Parse(ddl_Year.Text));
            for (int i = 0; i < fiscalWeekList.Count; i++)
            {
                ddl_Week.Items.Add(new ListItem(fiscalWeekList[i].FiscalWeek.ToString()));
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string strExportType = "Pdf";
            CrystalDecisions.Shared.ExportFormatType exportType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
            exportAdvancePaymentReport(strExportType, exportType);
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string strExportType = "Excel";
            CrystalDecisions.Shared.ExportFormatType exportType = CrystalDecisions.Shared.ExportFormatType.Excel;
            exportAdvancePaymentReport(strExportType, exportType);
        }

        private void exportAdvancePaymentReport(string strExportType, CrystalDecisions.Shared.ExportFormatType exportType)
        {
            if (exportType == CrystalDecisions.Shared.ExportFormatType.Excel)
            {
                genReportOpenXml();
            }

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

            List<NSLedProfitabilitiesDef> profitList = AccountManager.Instance.getNSLedProfitabilities(uclOfficeSelection.getOfficeList(), fiscalYear, fiscalWeek,
                                                        phaseList, ckb_StillSelling.Checked, ckb_NotYetLaunched.Checked, ckb_EndOfLife.Checked, -1, -1);

            if (profitList.Count == 0)
            {
                return;
            }

            List<NSLedSalesInfoDef> salesList = AccountManager.Instance.getNSLedSalesInfo(fiscalYear, fiscalWeek);
            if (salesList.Count == 0)
            {
                return;
            }

            foreach (NSLedProfitabilitiesDef def in profitList)
            {
                System.Diagnostics.Debug.Print(string.Format("Item#: {0}", def.ItemNo));

                List<NSLedSalesInfoDef> itemList = salesList.Where(x => x.ItemNo == def.ItemNo).OrderBy(x => x.FiscalYear).ThenBy(x => x.FiscalWeek).ToList();
                int currentYear = itemList.Count > 0 ? itemList.First().FiscalYear : -1;
                int currentWeek = itemList.Count > 0 ? itemList.First().FiscalWeek : -1;

            }

            System.Reflection.PropertyInfo listComm = typeof(NSLedProfitabilitiesDef).GetProperty("TotalNSCommissionAmt");
            System.Reflection.PropertyInfo listCost = typeof(NSLedProfitabilitiesDef).GetProperty("TotalProductionCost");
            System.Reflection.PropertyInfo listQtySold = typeof(NSLedProfitabilitiesDef).GetProperty("QtySold");
            System.Reflection.PropertyInfo listTotalQty = typeof(NSLedProfitabilitiesDef).GetProperty("TotalQty");
            System.Reflection.PropertyInfo listWeekCount = typeof(NSLedProfitabilitiesDef).GetProperty("FPWeekCount");

            List<NSLedProfitabilitiesDef> eolList = profitList
                .Where(x => x.IsEndOfLife)
                .OrderByDescending(x => x.IsEndOfLife)
                .ThenBy(x => x.ActualSaleSeasonId)
                .ThenBy(x => x.ActualSaleSeasonSplitId)
                .ThenBy(x => x.ProductTeamName)
                .ThenByDescending(x => (decimal)listComm.GetValue(x, null) - (decimal)listCost.GetValue(x, null) > 0)
                .ThenByDescending(x => x.FPWeekCount)
                .ToList();

            List<NSLedProfitabilitiesDef> ssList = profitList.Where(x => !x.IsEndOfLife && x.LaunchYear > 0)
                .OrderBy(x => x.ProductTeamName)
                .ThenByDescending(x => (int)listWeekCount.GetValue(x, null) > 5)
                .ThenBy(x => x.ActualSaleSeasonId)
                .ThenBy(x => x.ActualSaleSeasonSplitId)
                .ThenByDescending(x => Convert.ToDecimal(listQtySold.GetValue(x, null)) / Convert.ToDecimal(listTotalQty.GetValue(x, null)) >= 0.7m)
                .ThenByDescending(x => Convert.ToDecimal(listQtySold.GetValue(x, null)) / Convert.ToDecimal(listTotalQty.GetValue(x, null)) >= 0.5m &&
                    Convert.ToDecimal(listQtySold.GetValue(x, null)) / Convert.ToDecimal(listTotalQty.GetValue(x, null)) * 100m >= (int)listWeekCount.GetValue(x, null) * 2m)
                .ThenByDescending(x => x.FPWeekCount)
                .ToList();
            eolList.AddRange(ssList);

            List<NSLedProfitabilitiesDef> notYetLaunchedList = profitList.Where(x => !x.IsEndOfLife && x.LaunchYear == 0)
                .OrderBy(x => x.ProductTeamName)
                .ThenByDescending(x => (int)listWeekCount.GetValue(x, null) > 5)
                .ThenBy(x => x.ActualSaleSeasonId)
                .ThenBy(x => x.ActualSaleSeasonSplitId)
                .ThenByDescending(x => Convert.ToDecimal(listQtySold.GetValue(x, null)) / Convert.ToDecimal(listTotalQty.GetValue(x, null)) >= 0.7m)
                .ThenByDescending(x => Convert.ToDecimal(listQtySold.GetValue(x, null)) / Convert.ToDecimal(listTotalQty.GetValue(x, null)) >= 0.5m &&
                    Convert.ToDecimal(listQtySold.GetValue(x, null)) / Convert.ToDecimal(listTotalQty.GetValue(x, null)) * 100m >= (int)listWeekCount.GetValue(x, null) * 2m)
                .ThenByDescending(x => x.FPWeekCount)
                .ToList();
            eolList.AddRange(notYetLaunchedList);

            profitList = eolList;

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "NSLedSellThru_20210329a.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddHHmmss");
            string destFile = string.Format(ApplPhysicalPath + @"reporter\tmpReport\NSLedSellThru-{0}-{1}.xlsm", LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);
            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);
            string tempSheetId = OpenXmlUtil.getWorksheetId(document, "Template");
            string stSheetId = OpenXmlUtil.getWorksheetId(document, "ST");

            int firstDetailRow = 0;
            int endDetailRow = 0;

            int currentPhaseId = 0;
            int currentPhaseStartingRow = 0;

            int[] styleArray = OpenXmlUtil.getStyleIdList(document, tempSheetId, 2, 56);
            decimal seasonalExchangeRate = CommonManager.Instance.getSeasonalExchangeRate(profitList.Max(x => x.SeasonId), 2, 3);

            decimal markDownRecovery = 0;
            SystemParameterRef param = CommonManager.Instance.getSystemParameterByName("NSLED_MARK_DOWN_RECOVERY");
            if (param != null)
                if (!string.IsNullOrEmpty(param.ParameterValue.Trim()))
                    decimal.TryParse(param.ParameterValue.Trim(), out markDownRecovery);

            string lastSeason = "";
            string lastSubSeason = "";
            string lastProductTeam = "";
            int currentRowIndex = 4;
            int maxWeekCount = profitList.Max(x => x.FPWeekCount);
            /*
            if (maxWeekCount > 40)
                maxWeekCount = 40;
            */

            decimal seasonTotalComm = 0;
            decimal seasonTotalEstComm = 0;
            decimal seasonTotalCost = 0;
            decimal seasonTotalFOBCost = 0;
            decimal seasonTotalQtySold = 0;
            decimal seasonTotalQty = 0;
            decimal subSeasonTotalComm = 0;
            decimal subSeasonTotalEstComm = 0;
            decimal subSeasonTotalCost = 0;
            decimal subSeasonTotalFOBCost = 0;
            decimal subSeasonTotalQtySold = 0;
            decimal subSeasonTotalQty = 0;

            Dictionary<string, string> seasonStrToPhaseStr = new Dictionary<string, string>();
            Hashtable tblPhaseRowRange = new Hashtable();

            string phasing = (ckb_StillSelling.Checked ? ", " + ckb_StillSelling.Text : "") + (ckb_NotYetLaunched.Checked ? ", " + ckb_NotYetLaunched.Text : "") + (ckb_EndOfLife.Checked ? ", " + ckb_EndOfLife.Text : "");
            phasing = (phasing != "" ? " - " + phasing.Substring(2) : "");
            phasing = ddl_Phase.SelectedValue + (ckb_UnknownSeason.Checked ? " (includes " + ckb_UnknownSeason.Text + ")" : "") + phasing;
            OpenXmlUtil.setCellValue(document, stSheetId, "A2", "Data starting from phase " + phasing, CellValues.SharedString, 0);

            #endregion

            foreach (NSLedProfitabilitiesDef def in profitList)
            {

                #region New Season Phase
                //Change season
                if (lastSeason != (!def.IsEndOfLife ? (def.LaunchYear == 0 ? "NotYetLaunched" : "Still") : def.ActualSaleSeason))
                {
                    //End previous season
                    if (lastSeason != "")
                    {
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, "", CellValues.SharedString, styleArray[6]);
                        for (int i = 2; i <= maxWeekCount + 6; i++)
                        {
                            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, i, "", CellValues.SharedString, styleArray[7]);
                        }
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 7, "", CellValues.SharedString, styleArray[8]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, "Total", CellValues.SharedString, styleArray[12]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 9, "", CellValues.SharedString, styleArray[13]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 10, "", CellValues.SharedString, styleArray[13]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 11, "", CellValues.SharedString, styleArray[13]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 12, "", CellValues.SharedString, styleArray[13]);
                        OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, maxWeekCount + 13, "=" + OpenXmlUtil.getCellReference(maxWeekCount + 20, currentRowIndex + 1) + "/" + OpenXmlUtil.getCellReference(maxWeekCount + 18, currentRowIndex + 1));
                        OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, maxWeekCount + 13, (uint)styleArray[33]);
                        OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, maxWeekCount + 14, "=" + OpenXmlUtil.getCellReference(maxWeekCount + 30, currentRowIndex + 1));
                        OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, maxWeekCount + 14, (uint)styleArray[33]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 15, "", CellValues.SharedString, styleArray[7]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 16, "", CellValues.SharedString, styleArray[7]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, "", CellValues.SharedString, styleArray[8]);

                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 18, subSeasonTotalComm.ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 19, (-subSeasonTotalCost).ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 20, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 18) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 19) + currentRowIndex.ToString()), CellValues.String, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 21, subSeasonTotalComm == 0 ? "0" : (1 - subSeasonTotalCost / subSeasonTotalComm).ToString(), CellValues.Number, styleArray[23]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 23, (-subSeasonTotalFOBCost).ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 24, (subSeasonTotalQty - subSeasonTotalQtySold).ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 25, ((subSeasonTotalQty - subSeasonTotalQtySold) * (subSeasonTotalFOBCost / subSeasonTotalQty)).ToString(), CellValues.Number, styleArray[22]);

                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 27, subSeasonTotalEstComm.ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 28, (-subSeasonTotalCost).ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 29, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 27) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 28) + currentRowIndex.ToString()), CellValues.String, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 30, subSeasonTotalEstComm == 0 ? "0" : (1 - subSeasonTotalCost / subSeasonTotalEstComm).ToString(), CellValues.Number, styleArray[23]);


                        seasonTotalComm += subSeasonTotalComm;
                        seasonTotalEstComm += subSeasonTotalEstComm;
                        seasonTotalCost += subSeasonTotalCost;
                        seasonTotalFOBCost += subSeasonTotalFOBCost;
                        seasonTotalQty += subSeasonTotalQty;
                        seasonTotalQtySold += subSeasonTotalQtySold;
                        currentRowIndex++;

                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 18, seasonTotalComm.ToString(), CellValues.Number, styleArray[20]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 19, (-seasonTotalCost).ToString(), CellValues.Number, styleArray[20]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 20, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 18) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 19) + currentRowIndex.ToString()), CellValues.String, styleArray[20]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 21, seasonTotalComm == 0 ? "0" : (1 - seasonTotalCost / seasonTotalComm).ToString(), CellValues.Number, styleArray[21]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 23, (-seasonTotalFOBCost).ToString(), CellValues.Number, styleArray[20]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 24, (seasonTotalQty - seasonTotalQtySold).ToString(), CellValues.Number, styleArray[20]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 25, ((seasonTotalQty - seasonTotalQtySold) * (seasonTotalFOBCost / subSeasonTotalQty)).ToString(), CellValues.Number, styleArray[20]);

                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 27, seasonTotalEstComm.ToString(), CellValues.Number, styleArray[20]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 28, (-seasonTotalCost).ToString(), CellValues.Number, styleArray[20]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 29, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 27) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 28) + currentRowIndex.ToString()), CellValues.String, styleArray[20]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 30, seasonTotalEstComm == 0 ? "0" : (1 - seasonTotalCost / seasonTotalEstComm).ToString(), CellValues.Number, styleArray[21]);

                        currentRowIndex++;

                        seasonTotalComm = seasonTotalCost = seasonTotalFOBCost = seasonTotalQty = seasonTotalQtySold = subSeasonTotalComm = subSeasonTotalCost = seasonTotalEstComm = subSeasonTotalEstComm = subSeasonTotalFOBCost = subSeasonTotalQty = subSeasonTotalQtySold = 0;
                        lastSubSeason = "";
                        if (currentPhaseId != -2) // not yet launched
                        {
                            tblPhaseRowRange.Add(currentPhaseId, currentPhaseStartingRow.ToString() + "|" + (currentRowIndex - 2).ToString());
                            if (firstDetailRow == 0)
                                firstDetailRow = currentPhaseStartingRow;
                        }
                    }


                    if (def.IsEndOfLife && !seasonStrToPhaseStr.ContainsKey(def.ActualSaleSeason))
                    {
                        seasonStrToPhaseStr.Add(def.ActualSaleSeason, def.ActualSaleSeasonId == 999 ? "Phase Unknown" : "Phase " + def.NSLedPhaseId);
                    }

                    if (currentPhaseId != -2)
                        currentPhaseStartingRow = currentRowIndex;

                    currentPhaseId = def.NSLedPhaseId;

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, def.IsEndOfLife ? (seasonStrToPhaseStr[def.ActualSaleSeason] + " End of Life (went into " + def.ActualSaleSeason + ")") : (def.LaunchYear == 0 ? "Not-Yet-Launched" : "STILL Selling"), CellValues.SharedString, styleArray[0]);

                    OpenXmlUtil.copyRowCellRange(document, tempSheetId, 4, 1, 8, stSheetId, currentRowIndex, maxWeekCount + 18);
                    OpenXmlUtil.copyRowCellRange(document, tempSheetId, 7, 1, 14, stSheetId, currentRowIndex, maxWeekCount + 27);

                    for (int i = 2; i <= maxWeekCount + 6; i++)
                    {
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, i, "", CellValues.SharedString, styleArray[1]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, 1, "Profitable", CellValues.SharedString, styleArray[53]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, 2, "R", CellValues.SharedString, styleArray[34]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, 3, "Desc", CellValues.SharedString, styleArray[34]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, 4, "O", CellValues.SharedString, styleArray[34]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, 5, "D", CellValues.SharedString, styleArray[34]);

                        if (i > 6)
                        {
                            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, i, string.Format("{0} Wk", StringUtility.addOrdinal(i - 6)), CellValues.SharedString, styleArray[34]);

                            bool isNewColCollection = false;
                            Columns cols = ((WorksheetPart)document.WorkbookPart.GetPartById(stSheetId)).Worksheet.GetFirstChild<Columns>();
                            if (cols == null)
                            {
                                isNewColCollection = true;
                                cols = new Columns();
                            }

                            // hide wk 41 onwards
                            if (i - 6 > 40)
                            {

                                Column col = new Column() { Min = (uint)i, Max = (uint)i, Width = 10, CustomWidth = true, Hidden = true };
                                cols.Append(col);

                            }
                            else
                            {
                                cols.Append(new Column() { Min = (uint)i, Max = (uint)i, CustomWidth = true, Width = 10 });
                            }

                            if (isNewColCollection)
                                ((WorksheetPart)document.WorkbookPart.GetPartById(stSheetId)).Worksheet.Append(cols);


                        }
                    }
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 7, "", CellValues.SharedString, styleArray[2]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, "", CellValues.SharedString, styleArray[0]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 9, "", CellValues.SharedString, styleArray[1]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 10, "", CellValues.SharedString, styleArray[1]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 11, "", CellValues.SharedString, styleArray[1]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 12, "", CellValues.SharedString, styleArray[1]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 13, "", CellValues.SharedString, styleArray[1]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 14, "", CellValues.SharedString, styleArray[1]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 15, "", CellValues.SharedString, styleArray[1]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 16, "", CellValues.SharedString, styleArray[1]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, "", CellValues.SharedString, styleArray[2]);
                    currentRowIndex++;
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 6, "Qty", CellValues.SharedString, styleArray[34]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, "No. of wks on FP sale", CellValues.SharedString, styleArray[3]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 9, "Total Sell Thru", CellValues.SharedString, styleArray[4]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 10, "FP Sell Thru", CellValues.SharedString, styleArray[4]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 11, "MD Sell Thru", CellValues.SharedString, styleArray[4]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 12, "Write off", CellValues.SharedString, styleArray[4]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 13, "Current Profitability (Actual - life to date)", CellValues.SharedString, styleArray[4]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 14, "Estimate profitability (50% on MD)", CellValues.SharedString, styleArray[4]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 15, "Last Week Status", CellValues.SharedString, styleArray[4]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 16, "Actual MD Season (End of Life) /Estimated MD Season (Still Selling)", CellValues.SharedString, styleArray[4]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, "Remark", CellValues.SharedString, styleArray[5]);

                    OpenXmlUtil.copyRowCellRange(document, tempSheetId, 5, 1, 8, stSheetId, currentRowIndex, maxWeekCount + 18);
                    OpenXmlUtil.copyRowCellRange(document, tempSheetId, 8, 1, 14, stSheetId, currentRowIndex, maxWeekCount + 27);


                    OpenXmlUtil.setRowsHeight(document, stSheetId, currentRowIndex, 45.75);

                    currentRowIndex += 1;
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, string.Empty, CellValues.SharedString, styleArray[3]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, string.Empty, CellValues.SharedString, styleArray[3]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, string.Empty, CellValues.SharedString, styleArray[5]);
                    currentRowIndex += 1;
                }
                #endregion

                #region New Sub Season Phase
                //Change sub season
                string subSeasonString = "";
                if (def.IsEndOfLife)
                {
                    subSeasonString = def.TotalNSCommissionAmt - def.TotalProductionCost > 0 ? "Profit making" : "Loss making";
                }
                else
                {
                    /*
                    if (def.WeekCount <= 5)
                    {
                        subSeasonString = "Too soon to tell (5 weeks or less)";
                    }
                    else
                    {
                        if (currentPhaseId == -1) // still selling
                        {
                            if (def.TotalNSCommissionAmt - def.TotalProductionCost > 0)
                                subSeasonString = "Profit making";


                        }
                        else if (currentPhaseId == -2) // not yet launched
                        {



                        }
                    }
                    */

                    if (def.QtySold >= def.TotalQty * 0.7m)
                    {
                        subSeasonString = "Profitable";
                    }
                    else if (def.FPWeekCount <= 5)
                    {
                        subSeasonString = "Too soon to tell (5 weeks or less)";
                    }
                    else if (def.QtySold >= def.TotalQty * 0.5m && (decimal)def.QtySold / def.TotalQty * 100m >= def.FPWeekCount * 2m)
                    {
                        subSeasonString = "Possibly Profitable";
                    }
                    else
                    {
                        subSeasonString = "Unlikely to be Profitable";
                    }
                }
                if (def.ProductTeamName != lastProductTeam)
                {
                    if (lastSubSeason != subSeasonString && lastSubSeason != "")
                    {
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 7, "", CellValues.SharedString, styleArray[5]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, "", CellValues.SharedString, styleArray[9]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, "", CellValues.SharedString, styleArray[11]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 18, subSeasonTotalComm.ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 19, (-subSeasonTotalCost).ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 20, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 18) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 19) + currentRowIndex.ToString()), CellValues.String, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 21, subSeasonTotalComm == 0 ? "0" : (1 - subSeasonTotalCost / subSeasonTotalComm).ToString(), CellValues.Number, styleArray[23]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 23, (-subSeasonTotalFOBCost).ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 24, (subSeasonTotalQty - subSeasonTotalQtySold).ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 25, ((subSeasonTotalQty - subSeasonTotalQtySold) * (subSeasonTotalFOBCost / subSeasonTotalQty)).ToString(), CellValues.Number, styleArray[22]);

                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 27, subSeasonTotalEstComm.ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 28, (-subSeasonTotalCost).ToString(), CellValues.Number, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 29, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 27) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 28) + currentRowIndex.ToString()), CellValues.String, styleArray[22]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 30, subSeasonTotalEstComm == 0 ? "0" : (1 - subSeasonTotalCost / subSeasonTotalEstComm).ToString(), CellValues.Number, styleArray[23]);


                        seasonTotalComm += subSeasonTotalComm;
                        seasonTotalEstComm += subSeasonTotalEstComm;
                        seasonTotalCost += subSeasonTotalCost;
                        seasonTotalFOBCost += subSeasonTotalFOBCost;
                        seasonTotalQty += subSeasonTotalQty;
                        seasonTotalQtySold += subSeasonTotalQtySold;
                        subSeasonTotalComm = subSeasonTotalCost = subSeasonTotalEstComm = subSeasonTotalFOBCost = subSeasonTotalQty = subSeasonTotalQtySold = 0;
                    }

                    if (lastSeason == (!def.IsEndOfLife ? (def.LaunchYear == 0 ? "NotYetLaunched" : "Still") : def.ActualSaleSeason)) //add product team name
                    {
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 7, "", CellValues.SharedString, styleArray[5]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, "", CellValues.SharedString, styleArray[9]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, "", CellValues.SharedString, styleArray[11]);
                        currentRowIndex++;
                    }
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, def.ProductTeamName, CellValues.SharedString, styleArray[32]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 7, "", CellValues.SharedString, styleArray[5]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, "", CellValues.SharedString, styleArray[9]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, "", CellValues.SharedString, styleArray[11]);
                    currentRowIndex++;
                }
                lastSeason = !def.IsEndOfLife ? (def.LaunchYear == 0 ? "NotYetLaunched" : "Still") : def.ActualSaleSeason;

                if (lastSubSeason != subSeasonString || def.ProductTeamName != lastProductTeam) //add empty row
                {
                    if (lastSubSeason != "" && def.ProductTeamName == lastProductTeam)
                    {
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, "", CellValues.SharedString, styleArray[3]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 7, "", CellValues.SharedString, styleArray[5]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, "", CellValues.SharedString, styleArray[9]);
                        OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, "", CellValues.SharedString, styleArray[11]);
                        currentRowIndex++;
                    }
                    lastSubSeason = subSeasonString;
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, subSeasonString, CellValues.SharedString, styleArray[15]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 7, "", CellValues.SharedString, styleArray[5]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, "", CellValues.SharedString, styleArray[9]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, "", CellValues.SharedString, styleArray[11]);
                    currentRowIndex++;
                }
                lastProductTeam = def.ProductTeamName;
                #endregion

                #region Item Phase
                #region Item Information

                //Insert item info
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, def.ItemNo, CellValues.SharedString, styleArray[3]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 2, (def.CustomerId == 2 || def.CustomerId == 3) ? "Ret" : "", CellValues.SharedString, styleArray[54]);
                /*
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 3, def.Description, CellValues.SharedString, (def.CustomerId == 2 || def.CustomerId == 3) ? styleArray[26] : 0);
                */
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 3, def.Description, CellValues.SharedString, styleArray[54]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 4, CommonUtil.getOfficeRefByKey(def.OfficeId).OfficeCode, CellValues.SharedString, styleArray[54]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 5, shortenProductTeam(def.ProductTeamName), CellValues.SharedString, styleArray[getProductTeamColorStyleIndex(def.ProductTeamName, false)]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 6, def.TotalQty.ToString(), CellValues.Number, styleArray[55]);

                #endregion

                #region Sales Data
                //Insert sales data
                //Prepare
                int lastNetQty = 0, lastFPQty = 0, lastMDQty = 0, fpSellWeek = 0;
                decimal lastComm = 0;
                bool isTargetHighlighted = false;
                bool isMDHighlighted = false;

                List<decimal> fpSellThroughList = new List<decimal>();
                int lastFPYear = 0;
                int lastFPWeek = 0;

                int fromWeek = 0;
                int toWeek = 0;
                AccountManager.Instance.getNSLedRepeatItemParamRange(def.ItemNo, def.SeasonId, out fromWeek, out toWeek);

                List<NSLedSalesInfoDef> itemList = salesList.Where(x => x.ItemNo == def.ItemNo && (x.FiscalYear * 100 + x.FiscalWeek) >= fromWeek && (x.FiscalYear * 100 + x.FiscalWeek) < toWeek).OrderBy(x => x.FiscalYear).ThenBy(x => x.FiscalWeek).ToList(); //reduce list size

                int startYear = itemList.Count > 0 ? itemList[0].FiscalYear : -1;
                int startWeek = itemList.Count > 0 ? itemList[0].FiscalWeek : -1;
                int lastYear = startYear;
                int lastWeek = startWeek;
                int dataColIndex = 7;

                //Insert sales data
                foreach (NSLedSalesInfoDef sales in itemList)
                {
                    if (lastYear != sales.FiscalYear)
                    {
                        //Skipped week before change year
                        while (lastWeek + 1 <= 53)
                        {
                            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, dataColIndex, ((decimal)lastNetQty / def.TotalQty).ToString(), CellValues.Number, styleArray[4]);
                            dataColIndex++;
                            lastWeek++;
                            if (!isMDHighlighted)
                                fpSellWeek++;
                        }
                        lastWeek = 0;
                    }
                    if (lastWeek != sales.FiscalWeek)
                    {
                        //Skipped week for same year
                        while (lastWeek + 1 < sales.FiscalWeek)
                        {
                            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, dataColIndex, ((decimal)lastNetQty / def.TotalQty).ToString(), CellValues.Number, styleArray[4]);
                            dataColIndex++;
                            lastWeek++;
                            if (!isMDHighlighted)
                                fpSellWeek++;
                        }
                    }

                    //Add summary
                    lastNetQty += sales.NetQty;
                    lastComm += sales.NSCommAmtInUSD;
                    lastFPQty += (sales.NetQty - sales.MDQty);
                    lastMDQty += sales.MDQty;


                    //Update info

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, dataColIndex, ((decimal)lastNetQty / def.TotalQty).ToString(), CellValues.Number, styleArray[4]);
                    fpSellThroughList.Add((decimal)((lastNetQty - lastMDQty) / (decimal)def.TotalQty));

                    lastFPYear = sales.FiscalYear;
                    lastFPWeek = sales.FiscalWeek;


                    //if (lastMDQty > 0 && !isMDHighlighted)
                    if (def.IsMD && (lastFPYear * 100 + lastFPWeek) == (def.MDYear * 100 + def.MDWeek))
                    {
                        OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, dataColIndex, (uint)styleArray[24]);
                        isMDHighlighted = true;
                    }
                    if ((decimal)lastNetQty / def.TotalQty >= 0.7m && !isTargetHighlighted)
                    {
                        OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, dataColIndex, (uint)styleArray[25]);

                        isTargetHighlighted = true;
                    }

                    if (!isMDHighlighted)
                        fpSellWeek++;

                    lastYear = sales.FiscalYear;
                    lastWeek = sales.FiscalWeek;

                    dataColIndex++;

                    if ((dataColIndex - 7) > def.FPWeekCount - 1)
                        break;
                }

                while (dataColIndex - 7 < def.FPWeekCount)
                {
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, dataColIndex, ((decimal)lastNetQty / def.TotalQty).ToString(), CellValues.Number, styleArray[4]);
                    dataColIndex++;
                    if (!isMDHighlighted)
                        fpSellWeek++;
                }


                #endregion

                #region Summary
                //Update summary

                int year_counter = fiscalYear;
                int week_counter = fiscalWeek;
                int cnt = 0;

                if (lastFPYear > 0)
                {
                    while ((year_counter > lastFPYear || week_counter > lastFPWeek) && cnt < 6)
                    {
                        if (fpSellThroughList.Count > 0)
                            fpSellThroughList.Add(fpSellThroughList[fpSellThroughList.Count - 1]);
                        else
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
                    else if (((totalPercent / 3 * noOfWeekUntilNextSale) + fpSellThroughList[fpSellThroughList.Count - 1]) > 1)
                    {
                        growthRate = (1 - fpSellThroughList[fpSellThroughList.Count - 1]) / noOfWeekUntilNextSale;
                    }
                    else
                    {
                        growthRate = totalPercent / 3;
                    }
                }


                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 7, "", CellValues.SharedString, styleArray[5]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, fpSellWeek + " wks", CellValues.SharedString, styleArray[9]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 9, string.Format("={0}", OpenXmlUtil.getColumnLetter(maxWeekCount + 22) + currentRowIndex.ToString()), CellValues.String, styleArray[10]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 10, (def.TotalQty == 0 ? 0 : ((decimal)itemList.Sum(x => (x.NetQty - x.MDQty)) / def.TotalQty)).ToString(), CellValues.Number, styleArray[10]);

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 11, (def.TotalQty == 0 ? 0 : ((decimal)itemList.Sum(x => x.MDQty) / def.TotalQty)).ToString(), CellValues.Number, styleArray[10]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 12, (1 - (def.TotalQty == 0 ? 0 : (decimal)def.QtySold / def.TotalQty)).ToString(), CellValues.Number, styleArray[10]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 13, string.Format("={0}", OpenXmlUtil.getColumnLetter(maxWeekCount + 21) + currentRowIndex.ToString()), CellValues.String, styleArray[10]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 14, string.Format("={0}", OpenXmlUtil.getColumnLetter(maxWeekCount + 30) + currentRowIndex.ToString()), CellValues.String, styleArray[10]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 15, string.Empty, CellValues.String, styleArray[10]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 16, def.ActualSaleSeason == "Unknown season" ? def.ExpectedSaleSeason : def.ActualSaleSeason, CellValues.String, styleArray[10]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, def.SellThruRemark, CellValues.String, styleArray[11]);

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 18, ((decimal)itemList.Sum(x => x.NSCommAmtInUSD)).ToString(), CellValues.Number, styleArray[16]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 19, (-def.TotalProductionCost).ToString(), CellValues.Number, styleArray[16]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 20, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 18) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 19) + currentRowIndex.ToString()), CellValues.String, styleArray[16]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 21, string.Format("={0}/{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 20) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 18) + currentRowIndex.ToString()), CellValues.String, styleArray[30]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 22, (def.TotalQty == 0 ? 0 : (decimal)def.QtySold / def.TotalQty).ToString(), CellValues.Number, styleArray[30]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 23, def.TotalFOBCost.ToString(), CellValues.Number, styleArray[16]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 24, (def.TotalQty - def.QtySold).ToString(), CellValues.Number, styleArray[35]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 25, ((def.TotalQty - def.QtySold) * (def.TotalFOBCost / def.TotalQty)).ToString(), CellValues.Number, styleArray[16]);

                decimal assumedFPSellThru = growthRate * noOfWeekUntilNextSale + (fpSellThroughList.Count > 0 ? fpSellThroughList[fpSellThroughList.Count - 1] : 0);
                decimal assumedMDSellThru = (1 - assumedFPSellThru) * markDownRecovery;
                decimal actualMDSellThru = def.TotalQty == 0 ? 0 : ((decimal)itemList.Sum(x => x.MDQty) / def.TotalQty);


                decimal fcstPortionFPComm = (def.AvgRetailSellingPrice * noOfWeekUntilNextSale * growthRate * def.TotalQty / def.VATFactor * (decimal)seasonalExchangeRate * 0.5m);
                decimal fcstPortionFPQty = noOfWeekUntilNextSale * growthRate * def.TotalQty;
                decimal fcstPortionMDComm = ((def.AvgRetailSellingPrice / 2) * def.TotalQty * ((assumedMDSellThru - actualMDSellThru) > 0 ? (assumedMDSellThru - actualMDSellThru) : 0) / def.VATFactor * (decimal)seasonalExchangeRate * 0.6m);
                decimal fcstPortionMDQty = def.TotalQty / 2.0m;
                decimal totalEstimateComm = fcstPortionFPComm + (def.AvgRetailSellingPrice * def.TotalQty * fpSellThroughList.Count > 0 ? fpSellThroughList[fpSellThroughList.Count - 1] : 0 / def.VATFactor * (decimal)seasonalExchangeRate * 0.5m) + fcstPortionMDComm;
                decimal totalCost = (decimal)def.TotalProductionCost;
                decimal totalYTDComm = ((decimal)itemList.Sum(x => x.NSCommAmtInUSD));
                /*
                int totalYTDQty = itemList.Sum(x => x.NetQty);
                int totalYTDFPQty = itemList.Sum(x => (x.NetQty - x.MDQty));
                int totalYTDMDQty = (itemList.Sum(x => (int?)x.MDQty) ?? 0);
                */

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 27, (totalYTDComm + totalEstimateComm).ToString(), CellValues.Number, styleArray[16]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 28, (-def.TotalProductionCost).ToString(), CellValues.Number, styleArray[16]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 29, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 27) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 28) + currentRowIndex.ToString()), CellValues.String, styleArray[16]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 30, string.Format("={0}/{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 29) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 27) + currentRowIndex.ToString()), CellValues.String, styleArray[30]);

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 32, string.Format("={0}+{1}", OpenXmlUtil.getCellReference(maxWeekCount + 33, currentRowIndex), OpenXmlUtil.getCellReference(maxWeekCount + 34, currentRowIndex)), CellValues.String, styleArray[30]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 33, string.Format("={0}+({1}*{2})", OpenXmlUtil.getCellReference(maxWeekCount + 10, currentRowIndex), growthRate.ToString(), noOfWeekUntilNextSale.ToString()), CellValues.String, styleArray[30]);

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 34, string.Format("=IF((1-{0}{2})*0.5>{1}{2},(1-{0}{2})*{3},{1}{2})", OpenXmlUtil.getColumnLetter(maxWeekCount + 33), OpenXmlUtil.getColumnLetter(maxWeekCount + 11), currentRowIndex.ToString(), markDownRecovery.ToString()), CellValues.String, styleArray[30]);

                //OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 34, string.Format("=(1-{0})*{1}", OpenXmlUtil.getCellReference(maxWeekCount + 33, currentRowIndex), markDownRecovery.ToString()), CellValues.String, styleArray[30]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 35, string.Format("=1-{0}", OpenXmlUtil.getColumnLetter(maxWeekCount + 32) + currentRowIndex.ToString()), CellValues.String, styleArray[30]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 37, string.Format("={0}/(1-{1})", OpenXmlUtil.getColumnLetter(maxWeekCount + 34) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 33) + currentRowIndex.ToString()), CellValues.String, styleArray[30]);


                string profitableText = "N/A";

                if (def.IsEndOfLife)
                {
                    profitableText = def.TotalNSCommissionAmt - def.TotalProductionCost > 0 ? "Profit making" : "Loss making";
                }
                else
                {
                    if (def.WeekCount <= 5)
                        profitableText = "Too soon to tell";
                    else
                    {
                        if (currentPhaseId == -1) // still selling
                        {
                            if (def.TotalNSCommissionAmt - def.TotalProductionCost > 0)
                                profitableText = "Profit making";
                            else if ((def.TotalNSCommissionAmt - def.TotalProductionCost) < 0 && (totalYTDComm + totalEstimateComm - def.TotalProductionCost) > 0)
                                profitableText = "Possibly profitable";
                            else if ((def.TotalNSCommissionAmt - def.TotalProductionCost) < 0 && (totalYTDComm + totalEstimateComm - def.TotalProductionCost) < 0)
                                profitableText = "Unlikely to be profitable";
                        }
                    }
                }
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 39, profitableText, CellValues.String);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 40, def.Comment, CellValues.String);

                #endregion

                //Prepare for next item
                subSeasonTotalComm += (decimal)itemList.Sum(x => x.NSCommAmtInUSD);
                subSeasonTotalEstComm += (totalYTDComm + totalEstimateComm);
                subSeasonTotalCost += def.TotalProductionCost;
                subSeasonTotalFOBCost += def.TotalFOBCost;
                subSeasonTotalQty += def.TotalQty;
                subSeasonTotalQtySold += def.QtySold;
                currentRowIndex++;
                #endregion
            }

            #region Ending Phase
            //Update season ending summary
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, "", CellValues.SharedString, styleArray[6]);
            for (int i = 2; i <= maxWeekCount + 6; i++)
            {
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, i, "", CellValues.SharedString, styleArray[7]);
            }
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 7, "", CellValues.SharedString, styleArray[8]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 8, "Total", CellValues.SharedString, styleArray[12]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 9, "", CellValues.SharedString, styleArray[13]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 10, "", CellValues.SharedString, styleArray[13]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 11, "", CellValues.SharedString, styleArray[13]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 12, "", CellValues.SharedString, styleArray[13]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 13, "", CellValues.SharedString, styleArray[13]);
            OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, maxWeekCount + 13, "=" + OpenXmlUtil.getCellReference(maxWeekCount + 20, currentRowIndex + 1) + "/" + OpenXmlUtil.getCellReference(maxWeekCount + 18, currentRowIndex + 1));
            OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, maxWeekCount + 13, (uint)styleArray[33]);
            OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, maxWeekCount + 14, "=" + OpenXmlUtil.getCellReference(maxWeekCount + 30, currentRowIndex + 1));
            OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, maxWeekCount + 14, (uint)styleArray[33]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 15, "", CellValues.SharedString, styleArray[13]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 16, "", CellValues.SharedString, styleArray[13]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 17, "", CellValues.SharedString, styleArray[14]);

            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 18, subSeasonTotalComm.ToString(), CellValues.Number, styleArray[22]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 19, (-subSeasonTotalCost).ToString(), CellValues.Number, styleArray[22]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 20, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 18) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 19) + currentRowIndex.ToString()), CellValues.String, styleArray[22]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 21, (subSeasonTotalComm == 0 ? 0 : 1 - subSeasonTotalCost / subSeasonTotalComm).ToString(), CellValues.Number, styleArray[23]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 23, (-subSeasonTotalFOBCost).ToString(), CellValues.Number, styleArray[22]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 24, (subSeasonTotalQty - subSeasonTotalQtySold).ToString(), CellValues.Number, styleArray[22]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 25, ((subSeasonTotalQty - subSeasonTotalQtySold) * (subSeasonTotalFOBCost / subSeasonTotalQty)).ToString(), CellValues.Number, styleArray[22]);

            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 27, subSeasonTotalEstComm.ToString(), CellValues.Number, styleArray[22]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 28, (-subSeasonTotalCost).ToString(), CellValues.Number, styleArray[22]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 29, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 27) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 28) + currentRowIndex.ToString()), CellValues.String, styleArray[22]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 30, subSeasonTotalEstComm == 0 ? "0" : (1 - subSeasonTotalCost / subSeasonTotalEstComm).ToString(), CellValues.Number, styleArray[23]);


            seasonTotalComm += subSeasonTotalComm;
            seasonTotalEstComm += subSeasonTotalEstComm;
            seasonTotalCost += subSeasonTotalCost;
            seasonTotalFOBCost += subSeasonTotalFOBCost;
            seasonTotalQty += subSeasonTotalQty;
            seasonTotalQtySold += subSeasonTotalQtySold;
            currentRowIndex++;

            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 18, seasonTotalComm.ToString(), CellValues.Number, styleArray[20]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 19, (-seasonTotalCost).ToString(), CellValues.Number, styleArray[20]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 20, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 18) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 19) + currentRowIndex.ToString()), CellValues.String, styleArray[20]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 21, (seasonTotalComm == 0 ? 0 : 1 - seasonTotalCost / seasonTotalComm).ToString(), CellValues.Number, styleArray[21]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 23, (-seasonTotalFOBCost).ToString(), CellValues.Number, styleArray[20]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 24, (seasonTotalQty - seasonTotalQtySold).ToString(), CellValues.Number, styleArray[20]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 25, ((seasonTotalQty - seasonTotalQtySold) * (seasonTotalFOBCost / seasonTotalQty)).ToString(), CellValues.Number, styleArray[20]);

            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 27, seasonTotalEstComm.ToString(), CellValues.Number, styleArray[20]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 28, (-seasonTotalCost).ToString(), CellValues.Number, styleArray[20]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 29, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(maxWeekCount + 27) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 28) + currentRowIndex.ToString()), CellValues.String, styleArray[20]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, maxWeekCount + 30, seasonTotalEstComm == 0 ? "0" : (1 - seasonTotalCost / seasonTotalEstComm).ToString(), CellValues.Number, styleArray[21]);

            tblPhaseRowRange.Add(currentPhaseId, currentPhaseStartingRow.ToString() + "|" + (currentRowIndex - 2).ToString());
            endDetailRow = currentRowIndex - 2;

            currentRowIndex++;


            #region EOL Summary Box By Phase

            int firstRow = currentRowIndex;
            int startSummaryRow = 0;
            //Update sheet end summary for eol
            OpenXmlUtil.copyRowCellRange(document, tempSheetId, 10, 1, 35, stSheetId, currentRowIndex, 1);
            currentRowIndex += 1;
            OpenXmlUtil.copyRowCellRange(document, tempSheetId, 11, 1, 35, stSheetId, currentRowIndex, 1);
            currentRowIndex += 1;
            OpenXmlUtil.copyRowCellRange(document, tempSheetId, 12, 1, 35, stSheetId, currentRowIndex, 1);
            currentRowIndex += 1;
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, "", CellValues.SharedString, styleArray[3]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);
            currentRowIndex += 1;


            foreach (string s in eolList.Select(x => (x.NSLedPhaseId.ToString() + "|" + (x.NSLedPhaseId < 0 ? (x.NSLedPhaseId == -1 ? "Still Selling" : " Not Yet Launched") : x.ActualSaleSeason))).Distinct())
            {
                string season = s.Split('|')[1];
                int phaseId = int.Parse(s.Split('|')[0]);

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, phaseId < 0 ? (phaseId == -2 ? "Not Yet Launched" : "STILL selling (included Less than 5 weeks)") : seasonStrToPhaseStr[season] + " End of Life (went into " + season + ")", CellValues.SharedString, styleArray[15]);

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);

                currentRowIndex++;
                startSummaryRow = currentRowIndex;

                string rowRange = (string)tblPhaseRowRange[phaseId];
                int fromRowId = int.Parse(rowRange.Split('|')[0]);
                int toRowId = int.Parse(rowRange.Split('|')[1]);

                foreach (string department in eolList.Where(x => (x.NSLedPhaseId < 0 ? (x.NSLedPhaseId == -1 ? "Still Selling" : " Not Yet Launched") : x.ActualSaleSeason) == season).Select(x => x.ProductTeamName).Distinct())
                {
                    IEnumerable<NSLedProfitabilitiesDef> item = eolList.Where(x => x.ActualSaleSeason == season && x.ProductTeamName == department);


                    OpenXmlUtil.createEmptyCells(document, stSheetId, currentRowIndex, 1, 1, 9);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 3, shortenProductTeam(department), CellValues.SharedString, styleArray[getProductTeamColorStyleIndex(department, true)]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 5, string.Format("=COUNTIF($E${0}:$E${1},{2})", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString()), CellValues.String, styleArray[35]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 6, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 18)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 7, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 19)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 8, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(6) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(7) + currentRowIndex.ToString()), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 9, string.Format("=IFERROR({0}/{1},0)", OpenXmlUtil.getColumnLetter(8) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(6) + currentRowIndex.ToString()), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 11, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 23)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 13, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 24)), CellValues.String, styleArray[51]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 14, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 25)), CellValues.String, styleArray[51]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 16, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 27)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 17, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 28)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 18, string.Format("={0}+{1}", OpenXmlUtil.getCellReference(16, currentRowIndex), OpenXmlUtil.getCellReference(17, currentRowIndex)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 19, string.Format("={0}/{1}", OpenXmlUtil.getCellReference(18, currentRowIndex), OpenXmlUtil.getCellReference(16, currentRowIndex)), CellValues.String, styleArray[46]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 23, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 9)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 24, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 10)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 25, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 11)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 26, string.Format("=IF({0}>0,1-{0},0)", OpenXmlUtil.getColumnLetter(23) + currentRowIndex.ToString()), CellValues.String, styleArray[46]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 28, string.Format("={0}{2}/(1-{1}{2})", OpenXmlUtil.getColumnLetter(25), OpenXmlUtil.getColumnLetter(24), currentRowIndex.ToString()), CellValues.String, styleArray[46]);


                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 30, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 32)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 31, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 33)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 32, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getColumnLetter(3) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 34)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 33, string.Format("=IF({0}>0,1-{0},0)", OpenXmlUtil.getColumnLetter(30) + currentRowIndex.ToString()), CellValues.String, styleArray[46]);


                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);
                    OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 1, (uint)styleArray[3]);
                    currentRowIndex++;
                }
                OpenXmlUtil.createEmptyCells(document, stSheetId, currentRowIndex, 1, 1, 9);

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, "", CellValues.SharedString, styleArray[3]);
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 5,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(5, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(5, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 6,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(6, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(6, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 7,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(7, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(7, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 8,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(8, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(8, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 9,
                    string.Format("=IFERROR({0}/{1},0)", OpenXmlUtil.getCellReference(8, currentRowIndex), OpenXmlUtil.getCellReference(6, currentRowIndex)));

                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 11,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(11, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(11, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 13,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(13, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(11, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 14,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(14, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(11, currentRowIndex - 1)));


                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 16,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(16, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(16, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 17,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(17, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(17, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 18,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(18, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(18, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 19,
                    string.Format("={0}/{1}", OpenXmlUtil.getCellReference(18, currentRowIndex), OpenXmlUtil.getCellReference(16, currentRowIndex)));

                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 23,
                    string.Format("=SUM({3}${0}:{3}${1})/{2}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 9)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 24,
                    string.Format("=SUM({3}${0}:{3}${1})/{2}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 10)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 25,
                    string.Format("=SUM({3}${0}:{3}${1})/{2}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 11)));
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 26, string.Format("=IF({0}>0,1-{0},0)", OpenXmlUtil.getColumnLetter(23) + currentRowIndex.ToString()), CellValues.String, styleArray[48]);

                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 30,
                    string.Format("=SUM({3}${0}:{3}${1})/{2}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 32)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 31,
                    string.Format("=SUM({3}${0}:{3}${1})/{2}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 33)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 32,
                    string.Format("=SUM({3}${0}:{3}${1})/{2}", fromRowId.ToString(), toRowId.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 34)));
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 33, string.Format("=IF({0}>0,1-{0},0)", OpenXmlUtil.getColumnLetter(30) + currentRowIndex.ToString()), CellValues.String, styleArray[48]);


                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 1, (uint)styleArray[3]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 5, (uint)styleArray[36]);

                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 6, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 7, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 8, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 9, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 11, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 13, (uint)styleArray[52]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 14, (uint)styleArray[52]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 16, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 17, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 18, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 19, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 23, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 24, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 25, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 30, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 31, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 32, (uint)styleArray[48]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);
                currentRowIndex++;
            }


            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, "", CellValues.SharedString, styleArray[3]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, 1, "", CellValues.SharedString, styleArray[3]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, 35, "", CellValues.SharedString, styleArray[5]);
            OpenXmlUtil.copyRowCellRange(document, tempSheetId, 13, 1, 35, stSheetId, currentRowIndex + 2, 1);
            #endregion



            #region EOL Summary Box By Dept

            currentRowIndex += 4;

            startSummaryRow = 0;

            //Update sheet end summary for eol
            OpenXmlUtil.copyRowCellRange(document, tempSheetId, 10, 1, 35, stSheetId, currentRowIndex, 1);
            currentRowIndex += 1;
            OpenXmlUtil.copyRowCellRange(document, tempSheetId, 11, 1, 35, stSheetId, currentRowIndex, 1);
            currentRowIndex += 1;
            OpenXmlUtil.copyRowCellRange(document, tempSheetId, 12, 1, 35, stSheetId, currentRowIndex, 1);
            currentRowIndex += 1;
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, "", CellValues.SharedString, styleArray[3]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);
            currentRowIndex += 1;

            foreach (string department in eolList.Select(x => x.ProductTeamName).Distinct())
            {

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, department, CellValues.SharedString, styleArray[15]);

                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);

                currentRowIndex++;
                startSummaryRow = currentRowIndex;


                foreach (string s in eolList.Where(x => x.ProductTeamName == department).Select(x => (x.NSLedPhaseId.ToString() + "|" + (x.NSLedPhaseId < 0 ? (x.NSLedPhaseId == -1 ? "Still Selling" : " Not Yet Launched") : x.ActualSaleSeason))).Distinct())
                {

                    string season = s.Split('|')[1];
                    int phaseId = int.Parse(s.Split('|')[0]);

                    string rowRange = (string)tblPhaseRowRange[phaseId];
                    int fromRowId = int.Parse(rowRange.Split('|')[0]);
                    int toRowId = int.Parse(rowRange.Split('|')[1]);

                    OpenXmlUtil.createEmptyCells(document, stSheetId, currentRowIndex, 1, 1, 9);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 3, phaseId < 0 ? (phaseId == -2 ? "Not Yet Launched" : "STILL selling") : "ph " + phaseId.ToString(), CellValues.SharedString, styleArray[getProductTeamColorStyleIndex(department, true)]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 5, string.Format("=COUNTIF($E${0}:$E${1},{2})", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\""), CellValues.String, styleArray[35]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 6, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(maxWeekCount + 18)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 7, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(maxWeekCount + 19)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 8, string.Format("={0}+{1}", OpenXmlUtil.getColumnLetter(6) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(7) + currentRowIndex.ToString()), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 9, string.Format("=IFERROR({0}/{1},0)", OpenXmlUtil.getColumnLetter(8) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(6) + currentRowIndex.ToString()), CellValues.String, styleArray[46]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 11, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(maxWeekCount + 23)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 13, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(maxWeekCount + 24)), CellValues.String, styleArray[51]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 14, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(maxWeekCount + 25)), CellValues.String, styleArray[51]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 16, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(maxWeekCount + 27)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 17, string.Format("=SUMIF($E${0}:$E${1},{2},${3}${0}:${3}${1})/1000", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(maxWeekCount + 28)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 18, string.Format("={0}+{1}", OpenXmlUtil.getCellReference(16, currentRowIndex), OpenXmlUtil.getCellReference(17, currentRowIndex)), CellValues.String, styleArray[27]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 19, string.Format("={0}/{1}", OpenXmlUtil.getCellReference(18, currentRowIndex), OpenXmlUtil.getCellReference(16, currentRowIndex)), CellValues.String, styleArray[46]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 23, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 9)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 24, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 10)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 25, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 11)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 26, string.Format("=IF({0}>0,1-{0},0)", OpenXmlUtil.getColumnLetter(23) + currentRowIndex.ToString()), CellValues.String, styleArray[46]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 28, string.Format("={0}{2}/(1-{1}{2})", OpenXmlUtil.getColumnLetter(25), OpenXmlUtil.getColumnLetter(24), currentRowIndex.ToString()), CellValues.String, styleArray[46]);


                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 30, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 32)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 31, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 33)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 32, string.Format("=SUMIFS(${4}${0}:${4}${1},$E${0}:$E${1},{2})/{3}", fromRowId.ToString(), toRowId.ToString(), "\"" + shortenProductTeam(department) + "\"", OpenXmlUtil.getColumnLetter(5) + currentRowIndex.ToString(), OpenXmlUtil.getColumnLetter(maxWeekCount + 34)), CellValues.String, styleArray[46]);
                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 33, string.Format("=IF({0}>0,1-{0},0)", OpenXmlUtil.getColumnLetter(30) + currentRowIndex.ToString()), CellValues.String, styleArray[46]);

                    OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);
                    OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 1, (uint)styleArray[3]);

                    currentRowIndex++;
                }

                OpenXmlUtil.createEmptyCells(document, stSheetId, currentRowIndex, 1, 1, 9);


                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, "", CellValues.SharedString, styleArray[3]);
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 5,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(5, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(5, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 6,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(6, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(6, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 7,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(7, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(7, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 8,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(8, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(8, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 9,
                    string.Format("=IFERROR({0}/{1},0)", OpenXmlUtil.getCellReference(8, currentRowIndex), OpenXmlUtil.getCellReference(6, currentRowIndex)));

                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 11,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(11, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(11, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 13,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(13, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(11, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 14,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(14, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(11, currentRowIndex - 1)));


                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 16,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(16, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(16, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 17,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(17, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(17, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 18,
                    string.Format("=SUM({0})", OpenXmlUtil.getCellReference(18, startSummaryRow) + ":" + OpenXmlUtil.getCellReference(18, currentRowIndex - 1)));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 19,
                    string.Format("={0}/{1}", OpenXmlUtil.getCellReference(18, currentRowIndex), OpenXmlUtil.getCellReference(16, currentRowIndex)));


                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 23,
                    string.Format("=SUMIF(E${0}:E${1},{4},{3}${0}:{3}${1})/{2}", firstDetailRow.ToString(), endDetailRow.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 9), "\"" + shortenProductTeam(department) + "\""));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 24,
                    string.Format("=SUMIF(E${0}:E${1},{4},{3}${0}:{3}${1})/{2}", firstDetailRow.ToString(), endDetailRow.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 10), "\"" + shortenProductTeam(department) + "\""));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 25,
                    string.Format("=SUMIF(E${0}:E${1},{4},{3}${0}:{3}${1})/{2}", firstDetailRow.ToString(), endDetailRow.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 11), "\"" + shortenProductTeam(department) + "\""));
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 26, string.Format("=IF({0}>0,1-{0},0)", OpenXmlUtil.getColumnLetter(23) + currentRowIndex.ToString()), CellValues.String, styleArray[48]);

                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 30,
                    string.Format("=SUMIF(E${0}:E${1},{4},{3}${0}:{3}${1})/{2}", firstDetailRow.ToString(), endDetailRow.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 32), "\"" + shortenProductTeam(department) + "\""));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 31,
                    string.Format("=SUMIF(E${0}:E${1},{4},{3}${0}:{3}${1})/{2}", firstDetailRow.ToString(), endDetailRow.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 33), "\"" + shortenProductTeam(department) + "\""));
                OpenXmlUtil.setCellFormulaAppend(document, stSheetId, currentRowIndex, 32,
                    string.Format("=SUMIF(E${0}:E${1},{4},{3}${0}:{3}${1})/{2}", firstDetailRow.ToString(), endDetailRow.ToString(), OpenXmlUtil.getCellReference(5, currentRowIndex), OpenXmlUtil.getColumnLetter(maxWeekCount + 34), "\"" + shortenProductTeam(department) + "\""));
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 33, string.Format("=IF({0}>0,1-{0},0)", OpenXmlUtil.getColumnLetter(30) + currentRowIndex.ToString()), CellValues.String, styleArray[48]);


                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 1, (uint)styleArray[3]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 5, (uint)styleArray[36]);

                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 6, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 7, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 8, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 9, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 11, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 13, (uint)styleArray[52]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 14, (uint)styleArray[52]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 16, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 17, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 18, (uint)styleArray[28]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 19, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 23, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 24, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 25, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 30, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 31, (uint)styleArray[48]);
                OpenXmlUtil.setCellStyle(document, stSheetId, currentRowIndex, 32, (uint)styleArray[48]);
                OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);
                currentRowIndex++;
            }

            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 1, "", CellValues.SharedString, styleArray[3]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, 1, "", CellValues.SharedString, styleArray[3]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex, 35, "", CellValues.SharedString, styleArray[5]);
            OpenXmlUtil.setCellValue(document, stSheetId, currentRowIndex + 1, 35, "", CellValues.SharedString, styleArray[5]);
            OpenXmlUtil.copyRowCellRange(document, tempSheetId, 13, 1, 35, stSheetId, currentRowIndex + 2, 1);

            #endregion

            var columns = ((WorksheetPart)document.WorkbookPart.GetPartById(stSheetId)).Worksheet.GetFirstChild<Columns>();
            columns.Append(new Column() { Min = (uint)maxWeekCount + 7, Max = (uint)maxWeekCount + 7, CustomWidth = true, Width = 11 });
            columns.Append(new Column() { Min = (uint)maxWeekCount + 8, Max = (uint)maxWeekCount + 8, CustomWidth = true, Width = 11 });
            columns.Append(new Column() { Min = (uint)maxWeekCount + 9, Max = (uint)maxWeekCount + 9, CustomWidth = true, Width = 13 });
            columns.Append(new Column() { Min = (uint)maxWeekCount + 16, Max = (uint)maxWeekCount + 16, CustomWidth = true, Width = 25 });
            columns.Append(new Column() { Min = (uint)maxWeekCount + 17, Max = (uint)maxWeekCount + 17, CustomWidth = true, Width = 45 });
            columns.Append(new Column() { Min = (uint)maxWeekCount + 18, Max = (uint)maxWeekCount + 37, Hidden = true, CustomWidth = true, OutlineLevel = 1 });
            // profitable & comment
            columns.Append(new Column() { Min = (uint)maxWeekCount + 39, Max = (uint)maxWeekCount + 40, Hidden = true, Width = 45, OutlineLevel = 1 });

            //Delete temp sheet for output
            document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == tempSheetId).Remove();


            OpenXmlUtil.saveComplete(document);

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
            #endregion
        }

        private string shortenProductTeam(string team)
        {
            switch (team)
            {
                case "CHILDRENSWEAR":
                    return "CWR";
                case "MENSWEAR":
                    return "MWR";
                case "WOMENSWEAR":
                    return "WWR";
                case "FOOTWEAR":
                    return "NC-F";
                case "NON CLOTHING":
                    return "NC-A";
                default:
                    return string.Empty;
            }
        }

        private int getProductTeamColorStyleIndex(string team, bool isSummary)
        {
            switch (team)
            {
                case "CHILDRENSWEAR":
                    return isSummary ? 40 : 44;
                case "MENSWEAR":
                    return isSummary ? 39 : 43;
                case "WOMENSWEAR":
                    return isSummary ? 38 : 42;
                case "FOOTWEAR":
                    return isSummary ? 49 : 50;
                case "NON CLOTHING":
                    return isSummary ? 41 : 45;
                default:
                    return isSummary ? 41 : 45;
            }
        }


    }
}