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
    public partial class NSLedSalesInfoReport_Finance : com.next.isam.webapp.usercontrol.PageTemplate
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
            this.ddl_Duty.Items.Add(new ListItem("All", "-1"));
            this.ddl_Duty.Items.Add(new ListItem("Duty", "1"));
            this.ddl_Duty.Items.Add(new ListItem("Non-Duty", "0"));
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

        private string getSumFormulaFromColList(List<int> list, int row)
        {
            string col = string.Empty;
            string s = string.Empty;
            foreach (int i in list)
            {
                if (i > 0)
                {
                    if (s.Length > 0)
                        s += ",";
                    s += (OpenXmlUtil.getColumnLetter(i) + row.ToString());
                }
            }
            s = "=SUM(" + s + ")";
            return s;
        }

        private int getNoOfWeekByPeriodColumn(int periodCol)
        {
            int period = summaryCol.FindIndex(x => x == periodCol);

            if (period == 1 || period == 4 || period == 7 || period == 10 || period == 12)
                return 5;
            else
                return 4;
        }

        private int getPeriodColumn(int period)
        {
            return summaryCol[period];
        }

        private int getPeriodFirstWeekColumn(int period)
        {
            int periodCol = getPeriodColumn(period);
            return periodCol - getNoOfWeekByPeriodColumn(periodCol);
        }

        List<int> summaryCol = new List<int>() { 0, 12, 17, 22, 28, 33, 38, 44, 49, 54, 60, 65, 71 };

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

            int isDuty = this.ddl_Duty.selectedValueToInt;
            List<NSLedProfitabilitiesDef> profitList = AccountManager.Instance.getNSLedProfitabilities(uclOfficeSelection.getOfficeList(), fiscalYear, fiscalWeek, phaseList, ckb_StillSelling.Checked, ckb_NotYetLaunched.Checked, ckb_EndOfLife.Checked, -1, isDuty);

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
            string sourceFileName = "NSLedSalesInfo_Finance.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddHHmmss");
            string destFile = string.Format(this.ApplPhysicalPath + @"reporter\tmpReport\NSLedSalesInfo-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);
            string tempSheetId = OpenXmlUtil.getWorksheetId(document, "Template");
            string summSheetId = OpenXmlUtil.getWorksheetId(document, "SUMM");
            string currentSheetId = "";
            int currentOfficeId = 0;
            string currentOfficeCode = "";
            uint salesStyleId = 0;
            int transparentStyleId = 0;
            int reportStaringRow = 4;
            int itemStartingRow = 4;
            int lastWeekCol = 0;
            int itemCount = 0;
            int itemNextIndex = 1;
            int bfCol = OpenXmlUtil.getExcelColumnNo("F");
            int cumCol = OpenXmlUtil.getExcelColumnNo("BT");
            int lifeCol = OpenXmlUtil.getExcelColumnNo("BU");
            int sizeTableStartCol = OpenXmlUtil.getExcelColumnNo("BW");
            int profitCol = OpenXmlUtil.getExcelColumnNo("CC");
            int forecastCol = OpenXmlUtil.getExcelColumnNo("CF");
            decimal seasonalExchangeRate = CommonManager.Instance.getSeasonalExchangeRate(profitList.Max(x => x.SeasonId), 2, 3);
            decimal markDownRecovery = 0;
            transparentStyleId = OpenXmlUtil.getCellStyleId(document, tempSheetId, "A17");
            List<NSLedFinanceReportBFValuesDef> overrideBFValuesList = null;

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

                    overrideBFValuesList = AccountManager.Instance.getNSLedFinanceReportBFValues(def.OfficeId, fiscalYear);

                    //Check office exist in execl template
                    if (sheetId.Trim() == "")
                        continue;

                    currentSheetId = sheetId;
                    currentOfficeId = def.OfficeId;
                    currentOfficeCode = officeCode;
                    salesStyleId = (uint)OpenXmlUtil.getCellStyleId(document, currentSheetId, "B2");

                    OpenXmlUtil.setCellValue(document, currentSheetId, 1, OpenXmlUtil.getExcelColumnNo("CB"), seasonalExchangeRate.ToString(), CellValues.Number);
                    //Reset row index
                    itemStartingRow = 4;
                    itemCount = 0;
                }
                #endregion

                #region Item Phase
                #region Item Information
                //Copy Row from template
                for (int i = 1; i <= 17; i++)
                {
                    OpenXmlUtil.copyAndInsertRow(document, tempSheetId, i, currentSheetId, itemStartingRow + i - 1);
                }

                //Insert item info

                bool isContainBFData = false;

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, 2, currentOfficeCode + (def.HasDuty ? " (Duty)" : " (Non-Duty)"), CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, 4, def.ProductTeamName, CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, 2, def.Description + (def.SeasonCount > 1 ? " (repeat)" : ""), CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, 4, def.TotalQty.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, 2, def.ItemNo, CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, 4, def.RetailSellingPrice, CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, 2, def.TotalFOBCost.ToString(), CellValues.Number);

                //OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, 4, "=-(0.3*0.25)*2", CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, 4, (OfficeId.getStockProvisionRate(def.OfficeId) / 100.0m).ToString(), CellValues.Number);

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, 2, def.TotalFreightCost.ToString(), CellValues.Number);
                if (def.WeekCount == 0)
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, 3, "Not Yet Launched", CellValues.SharedString);

                // winnie requested to fill 12% for the following items 2021-05-20

                if (def.TotalDutyCost == 0 && def.OfficeId == OfficeId.SL.Id && (def.ItemNo == "751329" || def.ItemNo == "M05393" || def.ItemNo == "M05394" || def.ItemNo == "165879" || def.ItemNo == "713111" || def.ItemNo == "M06398" || def.ItemNo == "859316" ||
                    def.ItemNo == "M06399" || def.ItemNo == "M06400" || def.ItemNo == "M06650" || def.ItemNo == "M06403" || def.ItemNo == "M05387" || def.ItemNo == "M05390" || def.ItemNo == "711648" ||
                    def.ItemNo == "M05389" || def.ItemNo == "795863" || def.ItemNo == "479332" || def.ItemNo == "954625" || def.ItemNo == "480008" || def.ItemNo == "135913" || def.ItemNo == "366923" ||
                    def.ItemNo == "954378" || def.ItemNo == "366890" || def.ItemNo == "376178" || def.ItemNo == "862247" || def.ItemNo == "546562" || def.ItemNo == "444094" || def.ItemNo == "479932"))

                    def.TotalDutyCost = (0.12m * (def.TotalFOBCost));

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, 2, def.TotalDutyCost.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, 2, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 4).ToString(), (itemStartingRow + 6).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, 4, string.Format("={0}{1}/{2}{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(4), (itemStartingRow + 2).ToString()), CellValues.Number);

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, 2, (def.TotalSettlementDiscount * -1).ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, 4, string.Format("={0}{1}/{2}{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(4), (itemStartingRow + 2).ToString()), CellValues.Number);

                /*
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, 3, (def.EstimatedProfitPencentage / 100).ToString(), CellValues.Number);
                */

                //Add image
                /*
                byte[] resizedPicture = NSLedRangePlanDef.ResizeImage(def.Picture, 90, 135);
                OpenXmlUtil.addImage(document, currentSheetId, resizedPicture, def.ItemNo, 4, itemStartingRow + 10, 50000, 50000);
                */

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
                decimal lastComm = 0, lastPnL = 0, lastBS = 0, lastStock = 0, lastProvision = 0, lastSettlementDiscountForStock = 0;
                decimal weekComm = 0;
                bool isMDHighlighted = false;

                int fromWeek = 0;
                int toWeek = 0;
                AccountManager.Instance.getNSLedRepeatItemParamRange(def.ItemNo, def.SeasonId, out fromWeek, out toWeek);

                var itemList = salesList.Where(x => x.ItemNo == def.ItemNo && (x.FiscalYear * 100 + x.FiscalWeek) >= fromWeek && (x.FiscalYear * 100 + x.FiscalWeek) < toWeek); //reduce list size
                List<decimal> fpSellThroughList = new List<decimal>();
                int lastFPYear = 0;
                int lastFPWeek = 0;
                decimal stockProvisionRate = 0.15m;

                //B/F 
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow, bfCol, (fiscalYear - 1) + " B/F", CellValues.SharedString);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow, cumCol, fiscalYear.ToString(), CellValues.SharedString);

                NSLedFinanceReportBFValuesDef overrideBFValuesDef = overrideBFValuesList.Where(x => x.ItemNo == def.ItemNo && x.SeasonId == def.SeasonId).FirstOrDefault();

                if ((fiscalYear - 1) < 2021)
                    stockProvisionRate = 0.15m;
                else
                    stockProvisionRate = OfficeId.getStockProvisionRate(def.OfficeId) / 100.0m;

                var bfSales = itemList.Where(x => x.FiscalYear < fiscalYear);
                if (bfSales.Count() > 0)
                {
                    isContainBFData = true;

                    if (overrideBFValuesDef != null)
                    {
                        lastSalesQty += overrideBFValuesDef.SoldQty;
                        lastNetQty += overrideBFValuesDef.SoldQty;
                        lastComm += overrideBFValuesDef.NSSales;
                    }
                    else
                    {
                        lastSalesQty += bfSales.Sum(x => x.DespatchQty);
                        lastReturnsQty += bfSales.Sum(x => x.ReturnQty);
                        lastNetQty += bfSales.Sum(x => x.NetQty);
                        lastComm += bfSales.Sum(x => x.NSCommAmtInUSD);
                        lastMDQty += bfSales.Sum(x => x.MDQty);
                    }

                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, bfCol, lastNetQty.ToString(), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, bfCol, lastComm.ToString(), CellValues.Number);

                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.Cost.ToString() : string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(bfCol) + (itemStartingRow + 1).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, bfCol, string.Format("={0}{1}+{0}{2}", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 2).ToString(), (itemStartingRow + 3).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, bfCol, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 2).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.SettlementDiscountAmount.ToString() : string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(bfCol) + (itemStartingRow + 1).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.Provision.ToString() : string.Format("=({0}{1}+{0}{2})*-1*{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 7).ToString(), (itemStartingRow + 8).ToString(), stockProvisionRate.ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.TotalPL.ToString() : string.Format("=({0}{1}+{0}{2}+{0}{3})", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 6).ToString(), (itemStartingRow + 7).ToString()), CellValues.Number);

                    lastPnL += (overrideBFValuesDef != null ? overrideBFValuesDef.TotalPL : (
                                lastComm
                                - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * lastNetQty)
                                + (def.TotalSettlementDiscount / def.TotalQty * lastNetQty)
                                - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost - def.TotalSettlementDiscount) * stockProvisionRate)));

                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, bfCol, string.Format("={0}", lastPnL.ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.Stock.ToString() : string.Format("=({0}{1}+{2}{3})*-1", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 3).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.SettlementDiscountAmountForStock.ToString() : string.Format("=({0}{1}*-1)-{2}{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 6).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.ProvisionForStock.ToString() : string.Format("={0}{1}*-1", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 7).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.TotalBalanceSheet.ToString() : string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 11).ToString(), (itemStartingRow + 13).ToString()), CellValues.Number);

                    lastBS += (overrideBFValuesDef != null ? overrideBFValuesDef.TotalBalanceSheet : (
                        (((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * lastNetQty)) * -1)  // stock
                         + (def.TotalSettlementDiscount - (def.TotalSettlementDiscount / def.TotalQty * lastNetQty)) // settlement discount
                         + ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost - def.TotalSettlementDiscount) * stockProvisionRate)
                        ));

                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, bfCol, string.Format("={0}", lastBS.ToString()), CellValues.Number);


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
                        OpenXmlUtil.setCellStyle(document, currentSheetId, itemStartingRow, bfCol, salesStyleId);
                        isMDHighlighted = true;
                    }
                }
                else
                {
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, bfCol, "0", CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, bfCol, "0", CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.Cost.ToString() : string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(bfCol) + (itemStartingRow + 1).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, bfCol, string.Format("={0}{1}+{0}{2}", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 2).ToString(), (itemStartingRow + 3).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, bfCol, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 2).ToString()), CellValues.Number);
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.SettlementDiscountAmount.ToString() : string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(bfCol) + (itemStartingRow + 1).ToString()), CellValues.Number);

                    lastPnL += (overrideBFValuesDef != null ? overrideBFValuesDef.TotalPL : (
                                lastComm
                                - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * lastNetQty)
                                + (def.TotalSettlementDiscount / def.TotalQty * lastNetQty)
                                - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost - def.TotalSettlementDiscount) * stockProvisionRate)));

                    lastBS += (overrideBFValuesDef != null ? overrideBFValuesDef.TotalBalanceSheet : (
                        (((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * lastNetQty)) * -1)  // stock
                         + (def.TotalSettlementDiscount - (def.TotalSettlementDiscount / def.TotalQty * lastNetQty)) // settlement discount
                         + ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost - def.TotalSettlementDiscount) * stockProvisionRate))
                        );

                    lastStock = (def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) * -1;
                    lastProvision = (def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost - def.TotalSettlementDiscount) * -1 * stockProvisionRate;
                    lastSettlementDiscountForStock = def.TotalSettlementDiscount;

                    if (def.ShippedYear < fiscalYear)
                    {
                        if (def.ShippedYear != -1)
                        {

                            // todo: BD override BF values, for items shipped in 2020 and only started sale in 2021, required to get BF values.

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.Provision.ToString() : string.Format("=({0}{1}+{0}{2})*-1*{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 7).ToString(), (itemStartingRow + 8).ToString(), stockProvisionRate.ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.Stock.ToString() : string.Format("=({0}{1}+{2}{3})*-1", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 3).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.SettlementDiscountAmountForStock.ToString() : string.Format("=({0}{1}*-1)-{2}{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 6).ToString()), CellValues.Number);

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.TotalPL.ToString() : string.Format("=({0}{1}+{0}{2}+{0}{3})", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 6).ToString(), (itemStartingRow + 7).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, bfCol, string.Format("={0}", lastPnL.ToString()), CellValues.Number);

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.ProvisionForStock.ToString() : string.Format("={0}{1}*-1", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 7).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, bfCol, overrideBFValuesDef != null ? overrideBFValuesDef.TotalBalanceSheet.ToString() : string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(bfCol), (itemStartingRow + 11).ToString(), (itemStartingRow + 13).ToString()), CellValues.Number);


                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, bfCol, string.Format("={0}", lastBS.ToString()), CellValues.Number);
                        }

                    }

                    if (def.ShippedYear == -1 || def.ShippedYear >= fiscalYear)
                    {
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, bfCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, bfCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, bfCol, "0", CellValues.Number);

                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, bfCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, bfCol, "0", CellValues.Number);

                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, bfCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, bfCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, bfCol, "0", CellValues.Number);
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
                        if (((fiscalYear * 100) + CommonUtil.getFiscalPeriodByFiscalWeek(i)) < 202105)
                            stockProvisionRate = 0.15m;
                        else
                            stockProvisionRate = OfficeId.getStockProvisionRate(def.OfficeId) / 100.0m;

                        do
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, "0", CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, "0", CellValues.Number);


                            if ((!isContainBFData && def.ShippedYear == fiscalYear && currentCol == this.getPeriodFirstWeekColumn(def.ShippedPeriod)) || (!isContainBFData && def.ShippedYear == fiscalYear && currentCol == this.getPeriodColumn(def.ShippedPeriod))) //  first week of shipped period & shipped period columns
                            {
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, string.Format("=({0}{1}+{0}{2})*-1*{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 7).ToString(), (itemStartingRow + 8).ToString(), stockProvisionRate.ToString()), CellValues.Number);

                                if (!summaryCol.Contains(currentCol)) // skipping period column
                                {
                                    /*
                                    lastPnL += (
                                                lastComm
                                                - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * lastNetQty)
                                                + (def.TotalSettlementDiscount / def.TotalQty * lastNetQty)
                                                - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost - def.TotalSettlementDiscount) * stockProvisionRate));

                                    lastBS += (
                                        (((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * lastNetQty)) * -1)  // stock
                                         + (def.TotalSettlementDiscount - (def.TotalSettlementDiscount / def.TotalQty * lastNetQty)) // settlement discount
                                         + ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost - def.TotalSettlementDiscount) * stockProvisionRate)
                                        );
                                    */
                                }

                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, string.Format("=({0}{1}+{2}{3})*-1", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 3).ToString()), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, string.Format("=({0}{1}*-1)-{2}{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 6).ToString()), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, string.Format("={0}{1}*-1", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 7).ToString()), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, currentCol, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 11).ToString(), (itemStartingRow + 13).ToString()), CellValues.Number);

                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, string.Format("=({0}{1}+{0}{2}+{0}{3})", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 6).ToString(), (itemStartingRow + 7).ToString()), CellValues.Number);
                                /*
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, "0", CellValues.Number);
                                */
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, lastPnL.ToString(), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, currentCol, lastBS.ToString(), CellValues.Number);
                            }
                            else
                            {
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, currentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, "0", CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, currentCol, "0", CellValues.Number);

                            }


                            currentCol++;
                        } while (summaryCol.Contains(currentCol)); //Repeat if need to enter summary

                        if (i == fiscalWeek && def.ShippedPeriod > 0 && def.ShippedYear == fiscalYear)
                        {
                            int pCol = getPeriodColumn(def.ShippedPeriod);

                            if (((fiscalYear * 100) + def.ShippedPeriod) < 202105)
                                stockProvisionRate = 0.15m;
                            else
                                stockProvisionRate = OfficeId.getStockProvisionRate(def.OfficeId) / 100.0m;

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, pCol, string.Format("=({0}{1}+{0}{2})*-1*{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 7).ToString(), (itemStartingRow + 8).ToString(), stockProvisionRate.ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, pCol, string.Format("=({0}{1}+{0}{2}+{0}{3})", OpenXmlUtil.getColumnLetter(pCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 6).ToString(), (itemStartingRow + 7).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, pCol, lastPnL.ToString(), CellValues.Number);

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, pCol, string.Format("=({0}{1}+{2}{3})*-1", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(pCol), (itemStartingRow + 3).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, pCol, string.Format("=({0}{1}*-1)-{2}{3}", OpenXmlUtil.getColumnLetter(2), (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(pCol), (itemStartingRow + 6).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, pCol, string.Format("={0}{1}*-1", OpenXmlUtil.getColumnLetter(pCol), (itemStartingRow + 7).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, pCol, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(pCol), (itemStartingRow + 11).ToString(), (itemStartingRow + 13).ToString()), CellValues.Number);


                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, pCol, lastBS.ToString(), CellValues.Number);

                        }

                    }
                    var tempCol = currentCol - 1; //Check if summary entered
                    currentCol = summaryCol.Where(x => x >= currentCol - 1).Min();

                    if (currentCol == tempCol) //Summary entered
                    {
                        currentCol++;
                    }
                    else //Summary not entered
                    {
                        /*
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, lastPnL.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, currentCol, lastBS.ToString(), CellValues.Number);
                        */

                        currentCol++;


                        foreach (int i in summaryCol)
                        {
                            if (i > 0 && def.ShippedYear == fiscalYear && i >= getPeriodColumn(def.ShippedPeriod))
                            {
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, i, lastPnL.ToString(), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, i, lastBS.ToString(), CellValues.Number);
                            }
                        }
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

                            if ((!isContainBFData && def.ShippedYear == fiscalYear && currentCol == this.getPeriodFirstWeekColumn(def.ShippedPeriod)) || (!isContainBFData && def.ShippedYear == fiscalYear && currentCol == this.getPeriodColumn(def.ShippedPeriod))) //  first week of shipped period & shipped period columns
                            {
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, lastProvision.ToString(), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, (lastProvision * -1).ToString(), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, lastStock.ToString(), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, lastSettlementDiscountForStock.ToString(), CellValues.Number);

                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, lastPnL.ToString(), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, currentCol, lastBS.ToString(), CellValues.Number);
                            }


                            currentCol++;
                            if (summaryCol.Contains(currentCol)) //Skip for P summary
                            {

                                // todo : flora
                                /*
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, lastPnL.ToString(), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, currentCol, lastBS.ToString(), CellValues.Number);
                                */
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, string.Format("={0}", OpenXmlUtil.getColumnLetter(currentCol - 1) + (itemStartingRow + 9).ToString()), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, string.Format("={0}", OpenXmlUtil.getColumnLetter(currentCol - 1) + (itemStartingRow + 15).ToString()), CellValues.Number);


                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 7).ToString()), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 11).ToString()), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 12).ToString()), CellValues.Number);
                                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 13).ToString()), CellValues.Number);

                                currentCol++;
                            }

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
                            //todo: mike

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, weekNetQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, weekComm.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(currentCol) + (itemStartingRow + 1).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, string.Format("={0}{1}+{0}{2}", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 2).ToString(), (itemStartingRow + 3).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 2).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(currentCol) + (itemStartingRow + 1).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 7).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, string.Format("=({0}{1}+{0}{2}+{0}{3})", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 6).ToString(), (itemStartingRow + 7).ToString()), CellValues.Number);

                            /*
                            lastPnL += (
                                        weekComm
                                        - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * weekNetQty)
                                        + (def.TotalSettlementDiscount / def.TotalQty * weekNetQty));
                            */

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, string.Format("={0}", lastPnL.ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 11).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 12).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 13).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, currentCol, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 11).ToString(), (itemStartingRow + 13).ToString()), CellValues.Number);

                            /*
                            lastBS += (
                                ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * weekNetQty)  // stock
                                + (def.TotalSettlementDiscount / def.TotalQty * weekNetQty * -1)
                                );
                            */

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, currentCol, string.Format("={0}", lastBS.ToString()), CellValues.Number);

                            foreach (int j in summaryCol)
                            {
                                if (j > currentCol)
                                {
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, j, lastPnL.ToString(), CellValues.Number);
                                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, j, lastBS.ToString(), CellValues.Number);
                                }
                            }

                            currentCol++;

                            break;
                        }
                        //Get sales data, get the default 'zero' value for null case
                        NSLedSalesInfoDef sales = itemList.Where(x => x.FiscalYear == fiscalYear && x.FiscalWeek == i).FirstOrDefault() ?? new NSLedSalesInfoDef();

                        //Add summary
                        weekSalesQty += sales.NetQty;
                        weekReturnsQty += sales.ReturnQty;
                        weekNetQty += sales.NetQty;
                        weekComm += sales.NSCommAmtInUSD;

                        lastSalesQty += sales.DespatchQty;
                        lastReturnsQty += sales.ReturnQty;
                        lastNetQty += sales.NetQty;
                        lastComm += sales.NSCommAmtInUSD;
                        lastMDQty += sales.MDQty;


                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, sales.NetQty.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, sales.NSCommAmtInUSD.ToString(), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(currentCol) + (itemStartingRow + 1).ToString()), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, string.Format("={0}{1}+{0}{2}", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 2).ToString(), (itemStartingRow + 3).ToString()), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 2).ToString()), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(currentCol) + (itemStartingRow + 1).ToString()), CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, "0", CellValues.Number);
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, string.Format("=({0}{1}+{0}{2}+{0}{3})", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 6).ToString(), (itemStartingRow + 7).ToString()), CellValues.Number);

                        lastPnL += (
                                    sales.NSCommAmtInUSD
                                    - ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * sales.NetQty)
                                    + (def.TotalSettlementDiscount / def.TotalQty * sales.NetQty));

                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, string.Format("={0}", lastPnL.ToString()), CellValues.Number);


                        if ((!isContainBFData && def.ShippedYear == fiscalYear && currentCol == this.getPeriodFirstWeekColumn(def.ShippedPeriod))) //  first week of shipped period
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, string.Format("={2}+(({0}{1})*-1)", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 3).ToString(), lastStock.ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, string.Format("={2}+(({0}{1})*-1)", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 6).ToString(), lastSettlementDiscountForStock.ToString()), CellValues.Number);

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, lastProvision.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, (lastProvision * -1).ToString(), CellValues.Number);
                        }
                        else
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, string.Format("=(({0}{1})*-1)", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 3).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, string.Format("=(({0}{1})*-1)", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 6).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, "0", CellValues.Number);
                        }


                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, currentCol, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 11).ToString(), (itemStartingRow + 13).ToString()), CellValues.Number);

                        lastBS += (
                            ((def.TotalFOBCost + def.TotalFreightCost + def.TotalDutyCost) / def.TotalQty * sales.NetQty)  // stock
                            + (def.TotalSettlementDiscount / def.TotalQty * sales.NetQty * -1)
                            );

                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, currentCol, string.Format("={0}", lastBS.ToString()), CellValues.Number);


                        fpSellThroughList.Add((decimal)((lastNetQty - lastMDQty) / (decimal)def.TotalQty));
                        lastFPYear = sales.FiscalYear;
                        lastFPWeek = sales.FiscalWeek;

                        //if (lastMDQty > 0 && !isMDHighlighted)
                        if (!isMDHighlighted && def.IsMD && (lastFPYear * 100 + lastFPWeek) == (def.MDYear * 100 + def.MDWeek))
                        {
                            OpenXmlUtil.setCellStyle(document, currentSheetId, itemStartingRow, currentCol, salesStyleId);
                            isMDHighlighted = true;
                        }
                        currentCol++;
                        //Update summary
                        if (summaryCol.Contains(currentCol))
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, currentCol, weekSalesQty.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, currentCol, weekComm.ToString(), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, currentCol, string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 7).ToString(), OpenXmlUtil.getColumnLetter(currentCol) + (itemStartingRow + 1).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, currentCol, string.Format("={0}{1}+{0}{2}", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 2).ToString(), (itemStartingRow + 3).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, currentCol, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 2).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, currentCol, string.Format("={0}*{1}*-1", OpenXmlUtil.getColumnLetter(4) + (itemStartingRow + 8).ToString(), OpenXmlUtil.getColumnLetter(currentCol) + (itemStartingRow + 1).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 7).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, currentCol, string.Format("=({0}{1}+{0}{2}+{0}{3})", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 6).ToString(), (itemStartingRow + 7).ToString()), CellValues.Number);

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, currentCol, string.Format("={0}", lastPnL.ToString()), CellValues.Number);
                            /*
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, string.Format("=({0}{1})*-1", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 3).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, string.Format("=({0}{1})*-1", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 6).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, "0", CellValues.Number);
                            */

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 11).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 12).ToString()), CellValues.Number);
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, currentCol, string.Format("=SUM({0}{2}:{1}{2})", OpenXmlUtil.getColumnLetter(currentCol - getNoOfWeekByPeriodColumn(currentCol)), OpenXmlUtil.getColumnLetter(currentCol - 1), (itemStartingRow + 13).ToString()), CellValues.Number);

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, currentCol, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(currentCol), (itemStartingRow + 11).ToString(), (itemStartingRow + 13).ToString()), CellValues.Number);

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, currentCol, string.Format("={0}", lastBS.ToString()), CellValues.Number);
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

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 1), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 2), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 3), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 4), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, cumCol, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 2).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 6), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 7), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 8), CellValues.Number);

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, cumCol, string.Format("={0}{1}", OpenXmlUtil.getColumnLetter(cumCol - 1), (itemStartingRow + 9).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 11), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 12), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 13), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, cumCol, this.getSumFormulaFromColList(summaryCol, itemStartingRow + 14), CellValues.Number);

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, cumCol, string.Format("={0}{1}", OpenXmlUtil.getColumnLetter(cumCol - 1), (itemStartingRow + 15).ToString()), CellValues.Number);




                // life col

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 1).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 2).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 3).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 4).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, lifeCol, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(lifeCol), (itemStartingRow + 4).ToString(), (itemStartingRow + 2).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 6).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 7).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 8).ToString()), CellValues.Number);

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 9, lifeCol, string.Format("={0}{1}", OpenXmlUtil.getColumnLetter(cumCol - 1), (itemStartingRow + 9).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 11, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 11).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 12, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 12).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 13, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 13).ToString()), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 14, lifeCol, string.Format("={0}{2}+{1}{2}", OpenXmlUtil.getColumnLetter(bfCol), OpenXmlUtil.getColumnLetter(cumCol), (itemStartingRow + 14).ToString()), CellValues.Number);

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 15, lifeCol, string.Format("={0}{1}", OpenXmlUtil.getColumnLetter(cumCol - 1), (itemStartingRow + 15).ToString()), CellValues.Number);


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
                    else if (Math.Round(totalPercent / 3 * noOfWeekUntilNextSale + fpSellThroughList[fpSellThroughList.Count - 1], 2, MidpointRounding.AwayFromZero) > 1)
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

                /*
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow, forecastCol, (lastMDQty == 0 ? (noOfWeekUntilNextSale) : 0).ToString(), CellValues.Number);
                */
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow, forecastCol, noOfWeekUntilNextSale.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, forecastCol, growthRate.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, forecastCol, assumedFPSellThru.ToString("0.00"), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 3, forecastCol, assumedMDSellThru.ToString("0.00"), CellValues.Number, OpenXmlUtil.getCellStyleId(document, currentSheetId, itemStartingRow + 2, forecastCol));

                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 4, forecastCol, fcstPortionFPComm.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 5, forecastCol, fcstPortionMDComm.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 6, forecastCol, totalEstimateComm.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 7, forecastCol, totalCost.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 8, forecastCol, totalProfit.ToString(), CellValues.Number);


                for (int i = 0; i <= 15; i++)
                    OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + i, forecastCol + 2, currentOfficeCode + (def.HasDuty ? " (Duty)" : " (Non-Duty)"), CellValues.SharedString, transparentStyleId);


                #endregion

                //Prepare for next item
                itemStartingRow += 17;
                itemCount++;
                lastWeekCol = currentCol - 2;
                if (itemCount == 10 && profitList.Where(x => x.ProductTeamName == def.ProductTeamName).Count() > 10)
                {
                    OpenXmlUtil.insertHorizontalPageBreak(document, currentSheetId, itemStartingRow - 1);
                    itemCount = 0;
                }

                #endregion

                #region Summary
                int detailEndRow = 0;
                int financeCaptionCol = 5;
                int itemMainValCol = 2;

                if (itemNextIndex >= profitList.Count || currentOfficeId != profitList[itemNextIndex].OfficeId)
                {
                    if (!string.IsNullOrEmpty(currentOfficeCode))
                    {
                        //summary duty
                        detailEndRow = itemStartingRow + 1;

                        itemStartingRow += 3;

                        if (isDuty == 1 || isDuty == -1)
                        {
                            for (int i = 1; i <= 17; i++)
                            {
                                OpenXmlUtil.copyAndInsertRow(document, tempSheetId, i + 18, currentSheetId, itemStartingRow + i);
                            }


                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, 2, " ", CellValues.SharedString);

                            for (int j = itemStartingRow + 3; j <= (itemStartingRow + 17); j++)
                            {
                                if (j != 12)
                                    OpenXmlUtil.setCellValue(document, currentSheetId, j, 2, currentOfficeCode + " (Duty)", CellValues.SharedString);
                            }

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, bfCol, (fiscalYear - 1) + " B/F", CellValues.SharedString);

                            for (int i = bfCol; i <= lifeCol; i++)
                            {
                                for (int j = itemStartingRow + 3; j <= (itemStartingRow + 17); j++)
                                {
                                    if (j == itemStartingRow + 7) // margin %
                                        OpenXmlUtil.setCellValue(document, currentSheetId, j, i, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(i), (itemStartingRow + 6).ToString(), (itemStartingRow + 4).ToString()), CellValues.Number);
                                    else if (j != 12)
                                        OpenXmlUtil.setCellValue(document, currentSheetId, j, i, string.Format("=SUMIFS({0}${1}:{0}${2},${4}${1}:${4}${2},${4}{3},${6}${1}:${6}${2},${5}{3})", OpenXmlUtil.getColumnLetter(i), reportStaringRow.ToString(), detailEndRow.ToString(), j.ToString(), OpenXmlUtil.getColumnLetter(financeCaptionCol), OpenXmlUtil.getColumnLetter(itemMainValCol), OpenXmlUtil.getColumnLetter(forecastCol + 2)), CellValues.Number);
                                }
                            }
                        }

                        //summary non-duty

                        if (isDuty == 0 || isDuty == -1)
                        {
                            if (isDuty == -1) itemStartingRow += 17;

                            for (int i = 1; i <= 17; i++)
                            {
                                OpenXmlUtil.copyAndInsertRow(document, tempSheetId, i + 18, currentSheetId, itemStartingRow + i);
                            }

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, 2, " ", CellValues.SharedString);

                            for (int j = itemStartingRow + 3; j <= (itemStartingRow + 17); j++)
                            {
                                if (j != 12)
                                    OpenXmlUtil.setCellValue(document, currentSheetId, j, 2, currentOfficeCode + " (Non-Duty)", CellValues.SharedString);
                            }

                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, bfCol, (fiscalYear - 1) + " B/F", CellValues.SharedString);

                            for (int i = bfCol; i <= lifeCol; i++)
                            {
                                for (int j = itemStartingRow + 3; j <= (itemStartingRow + 17); j++)
                                {
                                    if (j == itemStartingRow + 7) // margin %
                                        OpenXmlUtil.setCellValue(document, currentSheetId, j, i, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(i), (itemStartingRow + 6).ToString(), (itemStartingRow + 4).ToString()), CellValues.Number);
                                    else if (j != 12)
                                        OpenXmlUtil.setCellValue(document, currentSheetId, j, i, string.Format("=SUMIFS({0}${1}:{0}${2},${4}${1}:${4}${2},${4}{3},${6}${1}:${6}${2},${5}{3})", OpenXmlUtil.getColumnLetter(i), reportStaringRow.ToString(), detailEndRow.ToString(), j.ToString(), OpenXmlUtil.getColumnLetter(financeCaptionCol), OpenXmlUtil.getColumnLetter(itemMainValCol), OpenXmlUtil.getColumnLetter(forecastCol + 2)), CellValues.Number);
                                }
                            }
                        }


                        //summary total

                        itemStartingRow += 17;
                        for (int i = 1; i <= 17; i++)
                        {
                            OpenXmlUtil.copyAndInsertRow(document, tempSheetId, i + 18, currentSheetId, itemStartingRow + i);
                        }
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 1, 2, " ", CellValues.SharedString);

                        for (int j = itemStartingRow + 3; j <= (itemStartingRow + 17); j++)
                        {
                            if (j != 12)
                                OpenXmlUtil.setCellValue(document, currentSheetId, j, 2, currentOfficeCode + " (Total)", CellValues.SharedString);
                        }

                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + 2, bfCol, (fiscalYear - 1) + " B/F", CellValues.SharedString);

                        for (int i = bfCol; i <= lifeCol; i++)
                        {
                            for (int j = itemStartingRow + 3; j <= (itemStartingRow + 17); j++)
                            {
                                if (j == itemStartingRow + 7) // margin %
                                    OpenXmlUtil.setCellValue(document, currentSheetId, j, i, string.Format("={0}{1}/{0}{2}", OpenXmlUtil.getColumnLetter(i), (itemStartingRow + 6).ToString(), (itemStartingRow + 4).ToString()), CellValues.Number);
                                else if (j != 12)
                                    OpenXmlUtil.setCellValue(document, currentSheetId, j, i, string.Format("=SUMIFS({0}${1}:{0}${2},${4}${1}:${4}${2},${4}{3})", OpenXmlUtil.getColumnLetter(i), reportStaringRow.ToString(), detailEndRow.ToString(), j.ToString(), OpenXmlUtil.getColumnLetter(financeCaptionCol), OpenXmlUtil.getColumnLetter(itemMainValCol), OpenXmlUtil.getColumnLetter(forecastCol + 2)), CellValues.Number);
                            }
                        }

                        var cols = ((WorksheetPart)document.WorkbookPart.GetPartById(currentSheetId)).Worksheet.GetFirstChild<Columns>();
                        cols.Append(new Column() { Min = (uint)forecastCol + 2, Max = (uint)forecastCol + 2, CustomWidth = true, Width = 30, Hidden = true });

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

            //Delete temp sheet for output
            //document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == OpenXmlUtil.getWorksheetId(document, "SUMM")).Remove();
            //document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == OpenXmlUtil.getWorksheetId(document, "SUMM-Obsolete")).Remove();
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