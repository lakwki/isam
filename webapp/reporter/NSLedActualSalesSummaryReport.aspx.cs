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
using com.next.common.domain.types;
using com.next.isam.webapp.webservices;
using com.next.isam.reporter.accounts;
using com.next.infra.util;
using com.next.isam.domain.account;
using com.next.isam.appserver.account;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web.UI.WebControls;

namespace com.next.isam.webapp.reporter
{
    public partial class NSLedActualSalesSummaryReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private string allSheetId = string.Empty;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControl();
            }
        }

        protected void initControl()
        {
            this.ddl_Dept.Items.Add(new ListItem("Womenswear", "1"));
            this.ddl_Dept.Items.Add(new ListItem("Menswear", "2"));
            this.ddl_Dept.Items.Add(new ListItem("Childrenswear", "3"));
            this.ddl_Dept.Items.Add(new ListItem("Footwear", "4"));
            this.ddl_Dept.Items.Add(new ListItem("Accessories", "5"));


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
            exportAdvancePaymentReport(strExportType, exportType);
        }

        protected void btn_Export_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            string strExportType = "Excel";
            CrystalDecisions.Shared.ExportFormatType exportType = CrystalDecisions.Shared.ExportFormatType.Excel;
            exportAdvancePaymentReport(strExportType, exportType);
        }

        private void exportAdvancePaymentReport(string strExportType, CrystalDecisions.Shared.ExportFormatType exportType)
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
            /* exclude unknowns
            phaseList.append(-1); 
            */
            phaseList.append(-1); 

            for (int i = 0; i <= ddl_Phase.SelectedIndex; i++)
                phaseList.append(Int32.Parse(ddl_Phase.Items[i].Value));
            List<NSLedProfitabilitiesDef> list = AccountManager.Instance.getNSLedProfitabilities(TypeCollector.Exclusive, fiscalYear, fiscalWeek, phaseList, ckb_StillSelling.Checked, false, true, ddl_Dept.selectedValueToInt, -1);

            if (list.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "NSLed Actual Sales Summary", "alert('No data found');", true);
                return;
            }

            list = list
                .OrderBy(x => x.NSLedPhaseId)
                .ThenBy(x => x.ItemNo)
                .ThenBy(x => x.OfficeSortId)
                .ToList();

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "NSLedActualSalesSummaryByDept.xlsx";
            string uId = DateTime.Now.ToString("yyyyMMddHHmmss");
            string destFile = string.Format(this.ApplPhysicalPath + @"reporter\tmpReport\NSLedActualSalesSummaryByDept-{0}-{1}.xlsx", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);
            string templateSheetId = OpenXmlUtil.getWorksheetId(document, "Template");
            string summarySheetId = OpenXmlUtil.getWorksheetId(document, "Summary");
            string deptSheetId = OpenXmlUtil.getWorksheetId(document, "by UK Dept");
            string qtySheetId = OpenXmlUtil.getWorksheetId(document, "by order Qty");
            string seasonSheetId = OpenXmlUtil.getWorksheetId(document, "by season");
            string allByUKDeptSheetId = OpenXmlUtil.getWorksheetId(document, "all by UK Dept");
            
            string pricePointSheetId = OpenXmlUtil.getWorksheetId(document, "by price point");
            
            allSheetId = OpenXmlUtil.getWorksheetId(document, "all");

            var actualPhaseItemList = list.Where(y => y.NSLedPhaseId != 999);

            OpenXmlUtil.setCellValue(document, summarySheetId, 2, 2, "NS LED " + ddl_Dept.selectedText + " Summary - Phases " + ddl_Phase.SelectedValue + " to " + actualPhaseItemList.Max(x => x.NSLedPhaseId).ToString(), CellValues.SharedString);
            OpenXmlUtil.setCellValue(document, summarySheetId, 3, 2, "as of " + fiscalYear.ToString() + " Week " + fiscalWeek.ToString(), CellValues.SharedString);

            int startingRow = 6;
            int currentRow = startingRow;
            int[] mainStyleList = OpenXmlUtil.getStyleIdList(document, templateSheetId, 6, 22);

            #endregion

            #region all sheet
            string currentPhase = string.Empty;


            foreach (NSLedProfitabilitiesDef def in list)
            {
                string phase = string.Empty;
                if (def.NSLedPhaseId == -1)
                    phase = "Still selling";
                else if (def.NSLedPhaseId == -2)
                    phase = "Not yet launched";
                else if (def.NSLedPhaseId == 999)
                    phase = "Unknown";
                else
                    phase = string.Format("ph {0}", def.NSLedPhaseId);

                string season = "N/A";
                if (def.SeasonId > 0)
                    season = CommonUtil.getSeasonByKey(def.SeasonId).Code;

                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 2, currentPhase != phase ? phase : string.Empty, CellValues.SharedString, mainStyleList[1]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 3, def.ItemNo, CellValues.String, mainStyleList[2]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 4, def.Description, CellValues.String, mainStyleList[3]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 5, NSLedRangePlanDef.getCustomerText(def.CustomerId), CellValues.String, mainStyleList[4]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 6, OfficeId.getName(def.OfficeId), CellValues.String, mainStyleList[5]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 7, season, CellValues.String, mainStyleList[6]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 8, def.UKDepartment, CellValues.String, mainStyleList[7]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 9, string.Format("£{0} - £{1}", Math.Round(def.MinRetailSellingPrice, 0, MidpointRounding.AwayFromZero).ToString(), Math.Round(def.MaxRetailSellingPrice, 0, MidpointRounding.AwayFromZero).ToString()), CellValues.String, mainStyleList[8]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 10, string.Format("£{0}", Math.Round(def.AvgRetailSellingPrice, 0, MidpointRounding.AwayFromZero).ToString()), CellValues.String, mainStyleList[9]);


                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 11, def.PricePoint, CellValues.String, mainStyleList[10]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 12, string.Format("{0}", def.TotalQty.ToString()), CellValues.Number, mainStyleList[11]);

                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 13, def.QtyRange, CellValues.String, mainStyleList[12]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 14, string.Format("={0}/{1}{2}", def.FPQty.ToString(), OpenXmlUtil.getColumnLetter(12), currentRow), CellValues.Number, mainStyleList[13]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 15, string.Format("={0}/{1}{2}", def.MDQty.ToString(), OpenXmlUtil.getColumnLetter(12), currentRow), CellValues.Number, mainStyleList[14]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 16, string.Format("{0}", def.TotalNSCommissionAmt.ToString()), CellValues.Number, mainStyleList[15]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 17, string.Format("{0}", def.TotalProductionCost.ToString()), CellValues.Number, mainStyleList[16]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 18, string.Format("={1}{0}-{2}{0}", currentRow, OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(17)), CellValues.Number, mainStyleList[17]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 19, string.Format("={1}{0}/{2}{0}", currentRow, OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(16)), CellValues.Number, mainStyleList[18]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 20, string.Format("{0}", def.FPQty), CellValues.Number, mainStyleList[19]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 21, string.Format("{0}", def.FPWeekCount), CellValues.Number, mainStyleList[20]);
                OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 22, string.Format("=IF({0}{1}>999,\"Yes\",\"No\")", OpenXmlUtil.getColumnLetter(12), currentRow), CellValues.Number, mainStyleList[21]);

                

                currentPhase = phase;
                currentRow += 1;
            }
            int endingRow = currentRow - 1;

            currentRow += 1;

            OpenXmlUtil.copyAndInsertRow(document, templateSheetId, 8, allSheetId, currentRow);
            OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 4, string.Format("=COUNTIF({0}{1}:{0}{2},\"<>\"\"\")", OpenXmlUtil.getColumnLetter(4), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 12, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(12), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 16, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(16), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 17, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(17), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 18, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(18), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 19, string.Format("={1}{0}/{2}{0}", currentRow, OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(16)), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allSheetId, currentRow, 20, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(20), startingRow, endingRow), CellValues.Number);

            #endregion

            #region all by uk dept sheet
            currentPhase = string.Empty;
            currentRow = startingRow;

            list = list.OrderBy(x => x.UKDepartment).ToList();

            foreach (NSLedProfitabilitiesDef def in list)
            {
                string phase = string.Empty;
                if (def.NSLedPhaseId == -1)
                    phase = "Still selling";
                else if (def.NSLedPhaseId == -2)
                    phase = "Not yet launched";
                else if (def.NSLedPhaseId == 999)
                    phase = "Unknown";
                else
                    phase = string.Format("ph {0}", def.NSLedPhaseId);

                string season = "N/A";
                if (def.SeasonId > 0)
                    season = CommonUtil.getSeasonByKey(def.SeasonId).Code;

                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 2, phase, CellValues.SharedString, mainStyleList[1]);
                byte[] resizedPicture = NSLedRangePlanDef.ResizeImage(def.Picture, 90, 135);
                OpenXmlUtil.addImage(document, allByUKDeptSheetId, resizedPicture, def.ItemNo, 1, currentRow, 50000, 50000);
                OpenXmlUtil.setRowsHeight(document, allByUKDeptSheetId, currentRow, 110);

                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 3, def.ItemNo, CellValues.String, mainStyleList[2]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 4, def.Description, CellValues.String, mainStyleList[3]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 5, NSLedRangePlanDef.getCustomerText(def.CustomerId), CellValues.String, mainStyleList[4]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 6, OfficeId.getName(def.OfficeId), CellValues.String, mainStyleList[5]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 7, season, CellValues.String, mainStyleList[6]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 8, def.UKDepartment, CellValues.String, mainStyleList[7]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 9, string.Format("£{0} - £{1}", Math.Round(def.MinRetailSellingPrice, 0, MidpointRounding.AwayFromZero).ToString(), Math.Round(def.MaxRetailSellingPrice, 0, MidpointRounding.AwayFromZero).ToString()), CellValues.String, mainStyleList[8]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 10, string.Format("£{0}", Math.Round(def.AvgRetailSellingPrice, 0, MidpointRounding.AwayFromZero).ToString()), CellValues.String, mainStyleList[9]);


                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 11, def.PricePoint, CellValues.String, mainStyleList[10]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 12, string.Format("{0}", def.TotalQty.ToString()), CellValues.Number, mainStyleList[11]);

                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 13, def.QtyRange, CellValues.String, mainStyleList[12]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 14, string.Format("={0}/{1}{2}", def.FPQty.ToString(), OpenXmlUtil.getColumnLetter(12), currentRow), CellValues.Number, mainStyleList[13]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 15, string.Format("={0}/{1}{2}", def.MDQty.ToString(), OpenXmlUtil.getColumnLetter(12), currentRow), CellValues.Number, mainStyleList[14]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 16, string.Format("{0}", def.TotalNSCommissionAmt.ToString()), CellValues.Number, mainStyleList[15]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 17, string.Format("{0}", def.TotalProductionCost.ToString()), CellValues.Number, mainStyleList[16]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 18, string.Format("={1}{0}-{2}{0}", currentRow, OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(17)), CellValues.Number, mainStyleList[17]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 19, string.Format("={1}{0}/{2}{0}", currentRow, OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(16)), CellValues.Number, mainStyleList[18]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 20, string.Format("{0}", def.FPQty), CellValues.Number, mainStyleList[19]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 21, string.Format("{0}", def.FPWeekCount), CellValues.Number, mainStyleList[20]);
                OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 22, string.Format("=IF({0}{1}>999,\"Yes\",\"No\")", OpenXmlUtil.getColumnLetter(12), currentRow), CellValues.Number, mainStyleList[21]);



                currentPhase = phase;
                currentRow += 1;
            }
            endingRow = currentRow - 1;

            currentRow += 1;

            OpenXmlUtil.copyAndInsertRow(document, templateSheetId, 8, allSheetId, currentRow);
            OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 4, string.Format("=COUNTIF({0}{1}:{0}{2},\"<>\"\"\")", OpenXmlUtil.getColumnLetter(4), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 12, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(12), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 16, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(16), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 17, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(17), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 18, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(18), startingRow, endingRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 19, string.Format("={1}{0}/{2}{0}", currentRow, OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(16)), CellValues.Number);
            OpenXmlUtil.setCellValue(document, allByUKDeptSheetId, currentRow, 20, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(20), startingRow, endingRow), CellValues.Number);

            #endregion

            #region summary sheet

            int summaryStartingRow = 6;
            int summaryCurrentRow = summaryStartingRow;

            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 3, string.Format("=all!{0}{1}", OpenXmlUtil.getColumnLetter(4), currentRow), CellValues.Number);
            
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 4, string.Format("=all!{0}{1}", OpenXmlUtil.getColumnLetter(12), currentRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 5, string.Format("=all!{0}{1}", OpenXmlUtil.getColumnLetter(16), currentRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 6, string.Format("=all!{0}{1}", OpenXmlUtil.getColumnLetter(17), currentRow), CellValues.Number);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 7, string.Format("=all!{0}{1}", OpenXmlUtil.getColumnLetter(18), currentRow), CellValues.Number);

            
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 8, string.Format("={1}{0}/{2}{0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number);

            summaryCurrentRow += 2;

            var customerList = (from s in list 
                                group s by new
                                {
                                    s.CustomerId
                                } into cList
                                select new
                                {
                                    CustomerId = cList.First().CustomerId
                                }).OrderBy(x => x.CustomerId);

            int[] summaryStyleList = OpenXmlUtil.getStyleIdList(document, templateSheetId, 32, 9);

            foreach (var r in customerList)
            {

                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 2, NSLedRangePlanDef.getCustomerText(r.CustomerId), CellValues.SharedString, summaryStyleList[1]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 3, string.Format("=COUNTIFS(all!${0}${1}:${0}${2},${3}${4})", OpenXmlUtil.getColumnLetter(5), startingRow, endingRow, OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[2]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 4, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},${2}${3})", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(5), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[3]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 5, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},${2}${3})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(5), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[4]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 6, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},${2}${3})", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(5), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[5]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 7, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},${2}${3})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(5), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[6]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 8, string.Format("={1}{0}/{2}{0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, summaryStyleList[7]);

                summaryCurrentRow += 1;
            }
            OpenXmlUtil.copyAndInsertRow(document, summarySheetId, 7, summarySheetId, summaryCurrentRow);

            summaryCurrentRow += 1;

            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 2, "Order Qty >= 1,000", CellValues.SharedString, summaryStyleList[1]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 3, string.Format("=COUNTIFS(all!{0}:{0},\"Yes\")", OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[2]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 4, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},\"Yes\")", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 5, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},\"Yes\")", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 6, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},\"Yes\")", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 7, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},\"Yes\")", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 8, string.Format("={1}{0}/{2}{0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, summaryStyleList[7]);

            summaryCurrentRow += 1;

            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 2, "Order Qty < 1,000", CellValues.SharedString, summaryStyleList[1]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 3, string.Format("=COUNTIFS(all!{0}:{0},\"No\")", OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[2]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 4, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},\"No\")", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 5, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},\"No\")", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 6, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},\"No\")", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 7, string.Format("=SUMIFS(all!{0}:{0},all!${1}:${1},\"No\")", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(22)), CellValues.Number, summaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 8, string.Format("={1}{0}/{2}{0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, summaryStyleList[7]);

            OpenXmlUtil.copyAndInsertRow(document, summarySheetId, 7, summarySheetId, summaryCurrentRow + 1);

            int summarySubHeadingStyleId = OpenXmlUtil.getCellStyleId(document, templateSheetId, 35, 1);

            summaryCurrentRow += 2;

            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 2, "By Territory", CellValues.SharedString, summarySubHeadingStyleId);
            summaryCurrentRow += 1;

            var officeList = (from s in list
                                group s by new
                                {
                                    s.OfficeId
                                } into oList
                                select new
                                {
                                    OfficeId = oList.First().OfficeId
                                }).OrderBy(x => OfficeId.getName(x.OfficeId));


            foreach (var r in officeList)
            {
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 2, OfficeId.getName(r.OfficeId), CellValues.SharedString, summaryStyleList[1]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 3, string.Format("=COUNTIFS(all!${0}:${0},${1}{2})", OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[2]);

                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 4, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3})", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[3]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 5, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[4]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 6, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3})", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[5]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 7, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[6]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 8, string.Format("={1}{0}/{2}{0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, summaryStyleList[7]);
                
                summaryCurrentRow += 1;
            }
            OpenXmlUtil.copyAndInsertRow(document, summarySheetId, 7, summarySheetId, summaryCurrentRow);

            summaryCurrentRow += 1;

            OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 2, "By Dept (ranked by purchase $'s)", CellValues.SharedString, summarySubHeadingStyleId);
            summaryCurrentRow += 1;

            var ukDeptList = (from s in list
                                group s by new
                                {
                                    s.UKDepartment
                                } into oList
                                select new
                                {
                                    UKDepartment = oList.First().UKDepartment,
                                    TotalProductionCost = oList.Sum((NSLedProfitabilitiesDef c) => c.TotalProductionCost),
                                }).OrderByDescending(x => x.TotalProductionCost);


            foreach (var r in ukDeptList)
            {
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 2, r.UKDepartment, CellValues.SharedString, summaryStyleList[1]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 3, string.Format("=COUNTIFS(all!${0}:${0},${1}{2})", OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[2]);

                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 4, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3})", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[3]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 5, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[4]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 6, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3})", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[5]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 7, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow), CellValues.Number, summaryStyleList[6]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 8, string.Format("={1}{0}/{2}{0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, summaryStyleList[7]);
                OpenXmlUtil.setCellValue(document, summarySheetId, summaryCurrentRow, 9, string.Format("={0}{1}/${0}${2}", OpenXmlUtil.getColumnLetter(6), summaryCurrentRow, summaryStartingRow), CellValues.Number, summaryStyleList[7]);
                
                summaryCurrentRow += 1;
            }

            #endregion

            int[] sectionHeaderSummaryStyleList = OpenXmlUtil.getStyleIdList(document, templateSheetId, 14, 17);
            int[] sectionSummaryStyleList = OpenXmlUtil.getStyleIdList(document, templateSheetId, 15, 17);
            List<int> sectionHeaderRowList = new List<int>();


            #region uk dept sheet

            int currentOfficeId = 0;
            int sectionHeaderRow = 0;
            summaryCurrentRow = summaryStartingRow;

            foreach (var o in officeList)
            {
                var ukDeptByOfficeList = (from s in list
                                          where s.OfficeId == o.OfficeId
                                  group s by new
                                  {
                                      s.UKDepartment
                                  } into dList
                                  select new
                                  {
                                      UKDepartment = dList.First().UKDepartment,
                                  }).OrderBy(x => x.UKDepartment);

                if (currentOfficeId != o.OfficeId)
                {
                    sectionHeaderRow = summaryCurrentRow;
                    sectionHeaderRowList.Add(sectionHeaderRow);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 2, OfficeId.getName(o.OfficeId), CellValues.SharedString, sectionHeaderSummaryStyleList[1]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 3, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(3), summaryCurrentRow + 1, summaryCurrentRow + ukDeptByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[2]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 4, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(4), summaryCurrentRow + 1, summaryCurrentRow + ukDeptByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[3]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 5, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(5), summaryCurrentRow + 1, summaryCurrentRow + ukDeptByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[4]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 6, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(6), summaryCurrentRow + 1, summaryCurrentRow + ukDeptByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[5]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 7, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(7), summaryCurrentRow + 1, summaryCurrentRow + ukDeptByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[6]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 8, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, sectionHeaderSummaryStyleList[7]);
                }

                summaryCurrentRow += 1;
                foreach (var d in ukDeptByOfficeList)
                {

                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 2, d.UKDepartment, CellValues.SharedString, sectionSummaryStyleList[1]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 3, string.Format("=COUNTIFS(all!${0}:${0},${1}{2},all!${3}:${3},${1}{4})", OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[2]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 4, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[3]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 5, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[4]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 6, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[5]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 7, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(8), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[6]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 8, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, sectionSummaryStyleList[7]);

                    summaryCurrentRow += 1;
                }

                currentOfficeId = o.OfficeId;

            }


            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 2, "Grand Total", CellValues.SharedString, sectionHeaderSummaryStyleList[1]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 3, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(3, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[2]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 4, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(4, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 5, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(5, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[4]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 6, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(6, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[5]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 7, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(7, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[6]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 8, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, sectionHeaderSummaryStyleList[7]);


            string currentUKDept = string.Empty;

            summaryCurrentRow = summaryStartingRow;
            sectionHeaderRowList.Clear();

            foreach (var o in ukDeptList)
            {
                var officeByUKDeptList = (from s in list
                                          where s.UKDepartment == o.UKDepartment
                                          group s by new
                                          {
                                              s.OfficeId
                                          } into oList
                                          select new
                                          {
                                              OfficeId = oList.First().OfficeId,
                                          }).OrderBy(x => OfficeId.getName(x.OfficeId));

                if (currentUKDept != o.UKDepartment)
                {
                    sectionHeaderRow = summaryCurrentRow;
                    sectionHeaderRowList.Add(sectionHeaderRow);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 10, o.UKDepartment, CellValues.SharedString, sectionHeaderSummaryStyleList[9]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 11, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(11), summaryCurrentRow + 1, summaryCurrentRow + officeByUKDeptList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[10]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 12, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(12), summaryCurrentRow + 1, summaryCurrentRow + officeByUKDeptList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[11]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 13, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(13), summaryCurrentRow + 1, summaryCurrentRow + officeByUKDeptList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[12]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 14, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(14), summaryCurrentRow + 1, summaryCurrentRow + officeByUKDeptList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[13]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 15, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(15), summaryCurrentRow + 1, summaryCurrentRow + officeByUKDeptList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[14]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 16, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(15), OpenXmlUtil.getColumnLetter(13)), CellValues.Number, sectionHeaderSummaryStyleList[15]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 17, string.Empty, CellValues.Number, sectionHeaderSummaryStyleList[16]);
                }

                summaryCurrentRow += 1;
                foreach (var d in officeByUKDeptList)
                {

                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 10, OfficeId.getName(d.OfficeId), CellValues.SharedString, sectionSummaryStyleList[9]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 11, string.Format("=COUNTIFS(all!${0}:${0},${1}{2},all!${3}:${3},${1}{4})", OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(8), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[10]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 12, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(8), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[11]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 13, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(8), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[12]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 14, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(8), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[13]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 15, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(8), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[14]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 16, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(15), OpenXmlUtil.getColumnLetter(13)), CellValues.Number, sectionSummaryStyleList[15]);
                    OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 17, string.Empty, CellValues.Number, sectionSummaryStyleList[16]);

                    summaryCurrentRow += 1;
                }

                currentUKDept = o.UKDepartment;


            }

            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 10, "Grand Total", CellValues.SharedString, sectionHeaderSummaryStyleList[9]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 11, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(11, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[10]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 12, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(12, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[11]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 13, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(13, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[12]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 14, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(14, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[13]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 15, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(15, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[14]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 16, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(15), OpenXmlUtil.getColumnLetter(13)), CellValues.Number, sectionHeaderSummaryStyleList[15]);
            OpenXmlUtil.setCellValue(document, deptSheetId, summaryCurrentRow, 17, "1", CellValues.Number, sectionHeaderSummaryStyleList[16]);


            foreach(int j in sectionHeaderRowList)
            {
                OpenXmlUtil.setCellValue(document, deptSheetId, j, 17, string.Format("={0}{1}/${0}${2}", OpenXmlUtil.getColumnLetter(14), j, summaryCurrentRow), CellValues.Number, sectionHeaderSummaryStyleList[16]);
            }



            #endregion



            #region price point sheet

            currentOfficeId = 0;
            sectionHeaderRow = 0;
            summaryCurrentRow = summaryStartingRow;
            sectionHeaderRowList.Clear();

            foreach (var o in officeList)
            {
                var pricePointByOfficeList = (from s in list
                                              where s.OfficeId == o.OfficeId
                                              group s by new
                                              {
                                                  s.PricePoint,
                                                  s.PriceStartingPoint
                                              } into dList
                                              select new
                                              {
                                                  PricePoint = dList.First().PricePoint,
                                                  PriceStaringPoint = dList.First().PriceStartingPoint
                                              }).OrderBy(x => x.PriceStaringPoint);

                if (currentOfficeId != o.OfficeId)
                {
                    sectionHeaderRow = summaryCurrentRow;
                    sectionHeaderRowList.Add(sectionHeaderRow);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 2, OfficeId.getName(o.OfficeId), CellValues.SharedString, sectionHeaderSummaryStyleList[1]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 3, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(3), summaryCurrentRow + 1, summaryCurrentRow + pricePointByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[2]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 4, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(4), summaryCurrentRow + 1, summaryCurrentRow + pricePointByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[3]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 5, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(5), summaryCurrentRow + 1, summaryCurrentRow + pricePointByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[4]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 6, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(6), summaryCurrentRow + 1, summaryCurrentRow + pricePointByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[5]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 7, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(7), summaryCurrentRow + 1, summaryCurrentRow + pricePointByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[6]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 8, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, sectionHeaderSummaryStyleList[7]);
                }

                summaryCurrentRow += 1;
                foreach (var d in pricePointByOfficeList)
                {

                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 2, d.PricePoint, CellValues.SharedString, sectionSummaryStyleList[1]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 3, string.Format("=COUNTIFS(all!${0}:${0},${1}{2},all!${3}:${3},${1}{4})", OpenXmlUtil.getColumnLetter(11), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[2]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 4, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(11), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[3]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 5, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(11), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[4]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 6, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(11), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[5]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 7, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(11), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[6]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 8, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, sectionSummaryStyleList[7]);

                    summaryCurrentRow += 1;
                }

                currentOfficeId = o.OfficeId;

            }


            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 2, "Grand Total", CellValues.SharedString, sectionHeaderSummaryStyleList[1]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 3, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(3, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[2]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 4, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(4, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 5, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(5, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[4]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 6, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(6, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[5]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 7, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(7, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[6]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 8, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, sectionHeaderSummaryStyleList[7]);


            string currentPricePoint = string.Empty;

            summaryCurrentRow = summaryStartingRow;
            sectionHeaderRowList.Clear();


            var pricePointList =   (from s in list
                                    group s by new
                                    {
                                        s.PricePoint,
                                        s.PriceStartingPoint
                                    } into oList
                                    select new
                                    {
                                        PricePoint = oList.First().PricePoint,
                                        PriceStartingPoint = oList.First().PriceStartingPoint
                                    }).OrderBy(x => x.PriceStartingPoint);

            foreach (var o in pricePointList)
            {
                var officeByPricePointList =    (from s in list
                                                 where s.PricePoint == o.PricePoint
                                                 group s by new
                                                 {
                                                    s.OfficeId
                                                 } into oList
                                                 select new
                                                 {
                                                    OfficeId = oList.First().OfficeId,
                                                 }).OrderBy(x => OfficeId.getName(x.OfficeId));

                if (currentPricePoint != o.PricePoint)
                {
                    sectionHeaderRow = summaryCurrentRow;
                    sectionHeaderRowList.Add(sectionHeaderRow);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 10, o.PricePoint, CellValues.SharedString, sectionHeaderSummaryStyleList[9]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 11, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(11), summaryCurrentRow + 1, summaryCurrentRow + officeByPricePointList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[10]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 12, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(12), summaryCurrentRow + 1, summaryCurrentRow + officeByPricePointList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[11]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 13, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(13), summaryCurrentRow + 1, summaryCurrentRow + officeByPricePointList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[12]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 14, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(14), summaryCurrentRow + 1, summaryCurrentRow + officeByPricePointList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[13]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 15, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(15), summaryCurrentRow + 1, summaryCurrentRow + officeByPricePointList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[14]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 16, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(15), OpenXmlUtil.getColumnLetter(13)), CellValues.Number, sectionHeaderSummaryStyleList[15]);
                }

                summaryCurrentRow += 1;
                foreach (var d in officeByPricePointList)
                {

                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 10, OfficeId.getName(d.OfficeId), CellValues.SharedString, sectionSummaryStyleList[9]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 11, string.Format("=COUNTIFS(all!${0}:${0},${1}{2},all!${3}:${3},${1}{4})", OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(11), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[10]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 12, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(11), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[11]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 13, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(11), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[12]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 14, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(11), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[13]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 15, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(11), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[14]);
                    OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 16, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(15), OpenXmlUtil.getColumnLetter(13)), CellValues.Number, sectionSummaryStyleList[15]);

                    summaryCurrentRow += 1;
                }

                currentPricePoint = o.PricePoint;


            }

            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 10, "Grand Total", CellValues.SharedString, sectionHeaderSummaryStyleList[9]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 11, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(11, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[10]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 12, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(12, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[11]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 13, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(13, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[12]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 14, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(14, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[13]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 15, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(15, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[14]);
            OpenXmlUtil.setCellValue(document, pricePointSheetId, summaryCurrentRow, 16, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(15), OpenXmlUtil.getColumnLetter(13)), CellValues.Number, sectionHeaderSummaryStyleList[15]);


            #endregion


            #region qty sheet

            currentOfficeId = 0;
            sectionHeaderRow = 0;
            summaryCurrentRow = summaryStartingRow;
            sectionHeaderRowList.Clear();

            foreach (var o in officeList)
            {
                var qtyRangeByOfficeList = (from s in list
                                              where s.OfficeId == o.OfficeId
                                              group s by new
                                              {
                                                  s.QtyRange,
                                                  s.QtyStartingPoint
                                              } into dList
                                              select new
                                              {
                                                  QtyRange = dList.First().QtyRange,
                                                  QtyStartingPoint = dList.First().QtyStartingPoint
                                              }).OrderBy(x => x.QtyStartingPoint);

                if (currentOfficeId != o.OfficeId)
                {
                    sectionHeaderRow = summaryCurrentRow;
                    sectionHeaderRowList.Add(sectionHeaderRow);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 2, OfficeId.getName(o.OfficeId), CellValues.SharedString, sectionHeaderSummaryStyleList[1]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 3, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(3), summaryCurrentRow + 1, summaryCurrentRow + qtyRangeByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[2]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 4, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(4), summaryCurrentRow + 1, summaryCurrentRow + qtyRangeByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[3]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 5, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(5), summaryCurrentRow + 1, summaryCurrentRow + qtyRangeByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[4]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 6, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(6), summaryCurrentRow + 1, summaryCurrentRow + qtyRangeByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[5]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 7, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(7), summaryCurrentRow + 1, summaryCurrentRow + qtyRangeByOfficeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[6]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 8, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, sectionHeaderSummaryStyleList[7]);
                }

                summaryCurrentRow += 1;
                foreach (var d in qtyRangeByOfficeList)
                {

                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 2, d.QtyRange, CellValues.SharedString, sectionSummaryStyleList[1]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 3, string.Format("=COUNTIFS(all!${0}:${0},${1}{2},all!${3}:${3},${1}{4})", OpenXmlUtil.getColumnLetter(13), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[2]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 4, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(13), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[3]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 5, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(13), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[4]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 6, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(13), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[5]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 7, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(13), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(6), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[6]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 8, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, sectionSummaryStyleList[7]);

                    summaryCurrentRow += 1;
                }

                currentOfficeId = o.OfficeId;

            }


            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 2, "Grand Total", CellValues.SharedString, sectionHeaderSummaryStyleList[1]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 3, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(3, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[2]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 4, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(4, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[3]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 5, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(5, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[4]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 6, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(6, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[5]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 7, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(7, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[6]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 8, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(5)), CellValues.Number, sectionHeaderSummaryStyleList[7]);


            string currentQtyRange = string.Empty;

            summaryCurrentRow = summaryStartingRow;
            sectionHeaderRowList.Clear();


            var qtyRangeList = (from s in list
                                  group s by new
                                  {
                                      s.QtyRange,
                                      s.QtyStartingPoint
                                  } into oList
                                  select new
                                  {
                                      QtyRange = oList.First().QtyRange,
                                      QtyStartingPoint = oList.First().QtyStartingPoint
                                  }).OrderBy(x => x.QtyStartingPoint);

            foreach (var o in qtyRangeList)
            {
                var officeByQtyRangeList = (from s in list
                                            where s.QtyRange == o.QtyRange
                                              group s by new
                                              {
                                                  s.OfficeId
                                              } into oList
                                              select new
                                              {
                                                  OfficeId = oList.First().OfficeId,
                                              }).OrderBy(x => OfficeId.getName(x.OfficeId));

                if (currentQtyRange != o.QtyRange)
                {
                    sectionHeaderRow = summaryCurrentRow;
                    sectionHeaderRowList.Add(sectionHeaderRow);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 10, o.QtyRange, CellValues.SharedString, sectionHeaderSummaryStyleList[9]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 11, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(11), summaryCurrentRow + 1, summaryCurrentRow + officeByQtyRangeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[10]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 12, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(12), summaryCurrentRow + 1, summaryCurrentRow + officeByQtyRangeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[11]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 13, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(13), summaryCurrentRow + 1, summaryCurrentRow + officeByQtyRangeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[12]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 14, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(14), summaryCurrentRow + 1, summaryCurrentRow + officeByQtyRangeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[13]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 15, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(15), summaryCurrentRow + 1, summaryCurrentRow + officeByQtyRangeList.Count()), CellValues.Number, sectionHeaderSummaryStyleList[14]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 16, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(15), OpenXmlUtil.getColumnLetter(13)), CellValues.Number, sectionHeaderSummaryStyleList[15]);
                }

                summaryCurrentRow += 1;
                foreach (var d in officeByQtyRangeList)
                {

                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 10, OfficeId.getName(d.OfficeId), CellValues.SharedString, sectionSummaryStyleList[9]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 11, string.Format("=COUNTIFS(all!${0}:${0},${1}{2},all!${3}:${3},${1}{4})", OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(13), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[10]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 12, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(12), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(13), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[11]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 13, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(13), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[12]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 14, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(17), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(13), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[13]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 15, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${2}{5})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(10), summaryCurrentRow, OpenXmlUtil.getColumnLetter(13), sectionHeaderRow), CellValues.Number, sectionSummaryStyleList[14]);
                    OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 16, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(15), OpenXmlUtil.getColumnLetter(13)), CellValues.Number, sectionSummaryStyleList[15]);

                    summaryCurrentRow += 1;
                }

                currentQtyRange = o.QtyRange;


            }

            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 10, "Grand Total", CellValues.SharedString, sectionHeaderSummaryStyleList[9]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 11, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(11, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[10]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 12, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(12, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[11]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 13, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(13, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[12]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 14, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(14, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[13]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 15, sectionHeaderRowList.Count == 0 ? string.Empty : "=SUM(" + OpenXmlUtil.getParamTextFromRowArray(15, sectionHeaderRowList) + ")", CellValues.Number, sectionHeaderSummaryStyleList[14]);
            OpenXmlUtil.setCellValue(document, qtySheetId, summaryCurrentRow, 16, string.Format("={1}{0}/${2}${0}", summaryCurrentRow, OpenXmlUtil.getColumnLetter(15), OpenXmlUtil.getColumnLetter(13)), CellValues.Number, sectionHeaderSummaryStyleList[15]);


            #endregion


            #region season sheet

            // commission

            var seasonList = (from s in list
                                group s by new
                                {
                                    s.SeasonId
                                } into oList
                                select new
                                {
                                    SeasonId = oList.First().SeasonId
                                }).OrderBy(x => x.SeasonId);

            int sectionHeaderStyleId = OpenXmlUtil.getCellStyleId(document, templateSheetId, 22, 2);
            int sectionRowStringStyleId = OpenXmlUtil.getCellStyleId(document, templateSheetId, 23, 2);
            int sectionRowNumberStyleId = OpenXmlUtil.getCellStyleId(document, templateSheetId, 23, 3);
            int sectionRowPercentStyleId = OpenXmlUtil.getCellStyleId(document, templateSheetId, 23, 4);
            int sectionHeaderNumberStyleId = OpenXmlUtil.getCellStyleId(document, templateSheetId, 30, 3);
            int sectionHeaderPercentStyleId = OpenXmlUtil.getCellStyleId(document, templateSheetId, 29, 3);
            int sectionFooterStyleId = OpenXmlUtil.getCellStyleId(document, templateSheetId, 30, 2);

            int summaryByCommissionStartingRow = 6;
            summaryCurrentRow = summaryByCommissionStartingRow;

            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryByCommissionStartingRow - 1, 2, "Commission", CellValues.SharedString, sectionHeaderStyleId);

            int startingSeasonColId = 2;
            int currentSeasonColId = startingSeasonColId;
            foreach (var o in seasonList)
            {
                string season = "N/A";
                if (o.SeasonId > 0)
                    season = CommonUtil.getSeasonByKey(o.SeasonId).Code;

                currentSeasonColId += 1;
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryByCommissionStartingRow - 1, currentSeasonColId, season, CellValues.SharedString, sectionHeaderStyleId);
            }
            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryByCommissionStartingRow - 1, currentSeasonColId + 1, "Total", CellValues.SharedString, sectionHeaderStyleId);

            foreach (var o in officeList)
            {
                currentSeasonColId = startingSeasonColId;
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, 2, OfficeId.getName(o.OfficeId), CellValues.SharedString, sectionRowStringStyleId);
                foreach (var d in seasonList)
                {
                    currentSeasonColId += 1;
                    OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${5}{6})", OpenXmlUtil.getColumnLetter(16), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(currentSeasonColId), summaryByCommissionStartingRow - 1), CellValues.Number, sectionRowNumberStyleId);
                }
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId + 1, string.Format("=SUM({1}{0}:{2}{0})", summaryCurrentRow, OpenXmlUtil.getColumnLetter(startingSeasonColId + 1), OpenXmlUtil.getColumnLetter(startingSeasonColId + seasonList.Count())), CellValues.Number, sectionRowNumberStyleId);
                summaryCurrentRow += 1;
            }

            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, startingSeasonColId, "Total", CellValues.SharedString, sectionFooterStyleId);
            currentSeasonColId = startingSeasonColId;
            foreach (var d in seasonList)
            {
                currentSeasonColId += 1;
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(currentSeasonColId), summaryByCommissionStartingRow, summaryByCommissionStartingRow + officeList.Count() - 1), CellValues.Number, sectionHeaderNumberStyleId);
            }

            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId + 1, string.Format("=SUM({1}{0}:{2}{0})", summaryCurrentRow, OpenXmlUtil.getColumnLetter(startingSeasonColId + 1), OpenXmlUtil.getColumnLetter(startingSeasonColId + seasonList.Count())), CellValues.Number, sectionHeaderNumberStyleId);


            // gross margin

            int summaryByGMStartingRow = summaryCurrentRow + 4;
            summaryCurrentRow = summaryByGMStartingRow;
            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryByGMStartingRow - 1, 2, "Gross Margin", CellValues.SharedString, sectionHeaderStyleId);

            currentSeasonColId = startingSeasonColId;
            foreach (var o in seasonList)
            {
                string season = "N/A";
                if (o.SeasonId > 0)
                    season = CommonUtil.getSeasonByKey(o.SeasonId).Code;

                currentSeasonColId += 1;
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryByGMStartingRow - 1, currentSeasonColId, season, CellValues.SharedString, sectionHeaderStyleId);
            }
            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryByGMStartingRow - 1, currentSeasonColId + 1, "Total", CellValues.SharedString, sectionHeaderStyleId);

            foreach (var o in officeList)
            {
                currentSeasonColId = startingSeasonColId;
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, 2, OfficeId.getName(o.OfficeId), CellValues.SharedString, sectionRowStringStyleId);
                foreach (var d in seasonList)
                {
                    currentSeasonColId += 1;
                    OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId, string.Format("=SUMIFS(all!${0}:${0},all!${1}:${1},${2}{3},all!${4}:${4},${5}{6})", OpenXmlUtil.getColumnLetter(18), OpenXmlUtil.getColumnLetter(6), OpenXmlUtil.getColumnLetter(2), summaryCurrentRow, OpenXmlUtil.getColumnLetter(7), OpenXmlUtil.getColumnLetter(currentSeasonColId), summaryByGMStartingRow - 1), CellValues.Number, sectionRowNumberStyleId);
                }
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId + 1, string.Format("=SUM({1}{0}:{2}{0})", summaryCurrentRow, OpenXmlUtil.getColumnLetter(startingSeasonColId + 1), OpenXmlUtil.getColumnLetter(startingSeasonColId + seasonList.Count())), CellValues.Number, sectionRowNumberStyleId);
                summaryCurrentRow += 1;
            }

            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, startingSeasonColId, "Total", CellValues.SharedString, sectionFooterStyleId);
            currentSeasonColId = startingSeasonColId;
            foreach (var d in seasonList)
            {
                currentSeasonColId += 1;
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId, string.Format("=SUM({0}{1}:{0}{2})", OpenXmlUtil.getColumnLetter(currentSeasonColId), summaryByGMStartingRow, summaryByGMStartingRow + officeList.Count() - 1), CellValues.Number, sectionHeaderNumberStyleId);
            }

            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId + 1, string.Format("=SUM({1}{0}:{2}{0})", summaryCurrentRow, OpenXmlUtil.getColumnLetter(startingSeasonColId + 1), OpenXmlUtil.getColumnLetter(startingSeasonColId + seasonList.Count())), CellValues.Number, sectionHeaderNumberStyleId);

            // GM%

            summaryStartingRow = summaryCurrentRow + 4;
            summaryCurrentRow = summaryStartingRow;
            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryStartingRow - 1, 2, "GM%", CellValues.SharedString, sectionHeaderStyleId);

            currentSeasonColId = startingSeasonColId;
            foreach (var o in seasonList)
            {
                string season = "N/A";
                if (o.SeasonId > 0)
                    season = CommonUtil.getSeasonByKey(o.SeasonId).Code;

                currentSeasonColId += 1;
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryStartingRow - 1, currentSeasonColId, season, CellValues.SharedString, sectionHeaderStyleId);
            }
            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryStartingRow - 1, currentSeasonColId + 1, "Total", CellValues.SharedString, sectionHeaderStyleId);

            int cnt = 0;
            foreach (var o in officeList)
            {
                currentSeasonColId = startingSeasonColId;
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, 2, OfficeId.getName(o.OfficeId), CellValues.SharedString, sectionRowStringStyleId);
                foreach (var d in seasonList)
                {
                    currentSeasonColId += 1;
                    OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId, string.Format("=IFERROR({0}{1}/{0}{2},0)", OpenXmlUtil.getColumnLetter(currentSeasonColId), summaryByGMStartingRow + cnt, summaryByCommissionStartingRow + cnt), CellValues.Number, sectionRowPercentStyleId);
                }
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId + 1, string.Format("=IFERROR({0}{1}/{0}{2},0)", OpenXmlUtil.getColumnLetter(currentSeasonColId + 1), summaryByGMStartingRow + cnt, summaryByCommissionStartingRow + cnt), CellValues.Number, sectionRowPercentStyleId);
                summaryCurrentRow += 1;
                cnt += 1;
            }


            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, startingSeasonColId, "Total", CellValues.SharedString, sectionFooterStyleId);
            currentSeasonColId = startingSeasonColId;
            foreach (var d in seasonList)
            {
                currentSeasonColId += 1;
                OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId, string.Format("=IFERROR({0}{1}/{0}{2},0)", OpenXmlUtil.getColumnLetter(currentSeasonColId), summaryByGMStartingRow + cnt, summaryByCommissionStartingRow + cnt), CellValues.Number, sectionHeaderPercentStyleId);
            }

            OpenXmlUtil.setCellValue(document, seasonSheetId, summaryCurrentRow, currentSeasonColId + 1, string.Format("=IFERROR({0}{1}/{0}{2},0)", OpenXmlUtil.getColumnLetter(currentSeasonColId + 1), summaryByGMStartingRow + cnt, summaryByCommissionStartingRow + cnt), CellValues.Number, sectionHeaderPercentStyleId);

            #endregion

            OpenXmlUtil.saveComplete(document);

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);

        }


    }
}