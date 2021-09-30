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
    public partial class NSLedProfitabilitiesReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private string WworksheetID = string.Empty;
        private string MworksheetID = string.Empty;
        private string CworksheetID = string.Empty;
        private string NFworksheetID = string.Empty;
        private string NAworksheetID = string.Empty;

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
            //else
            //    ReportHelper.export(genReport(strExportType), HttpContext.Current.Response, exportType, "Advance Payment Report");

        }
        /*
        private ReportClass genReport(string exportType)
        {
            DateTime paymentDateFrom = DateTime.MinValue;
            DateTime paymentDateTo = DateTime.MinValue;
            DateTime deductionDateFrom = DateTime.MinValue;
            DateTime deductionDateTo = DateTime.MinValue;

            int vendorId = -1;
            string vendorName = "ALL";
            int officeId = -1;
            string officeName = "ALL";
            int paymentstatusIndex = 0;
            string paymentstatusString = "ALL";


            if (txt_PaymentDateFrom.Text.Trim() != "" || txt_PaymentDateTo.Text.Trim() != "")
            {
                paymentDateFrom = DateTime.Parse(txt_PaymentDateFrom.Text);
                paymentDateTo = DateTime.Parse(txt_PaymentDateTo.Text);
            }

            if (txt_DeductionDateFrom.Text.Trim() != "" || txt_DeductionDateTo.Text.Trim() != "")
            {
                deductionDateFrom = DateTime.Parse(txt_DeductionDateFrom.Text);
                deductionDateTo = DateTime.Parse(txt_DeductionDateTo.Text);
            }


            if (txt_Supplier.VendorId != int.MinValue)
            {
                vendorName = txt_Supplier.ToString();
                vendorId = txt_Supplier.VendorId;
            }

            if (ddlOffice.SelectedIndex != 0) 
            {
                officeId = int.Parse(ddlOffice.SelectedValue);
                officeName = CommonUtil.getOfficeRefByKey(officeId).Description;
            }

            
            paymentstatusIndex = int.Parse(ddlStatus.SelectedValue);
            paymentstatusString = ddlStatus.SelectedItem.Text;

            GetAdvancePaymentReportMethod getAdvancePaymentReportMethod;
            switch (selectedIndex)
            {
                case 1:
                    getAdvancePaymentReportMethod = AccountReportManager.Instance.getAdvancePaymentReportMG;
                    break;
                case 0: default:
                    getAdvancePaymentReportMethod = AccountReportManager.Instance.getAdvancePaymentReport;
                    break;
            }

            return getAdvancePaymentReportMethod(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, vendorName, officeId, officeName, paymentstatusIndex, paymentstatusString, exportType);
        }
        private delegate ReportClass GetAdvancePaymentReportMethod(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, string vendorName, int officeId, string officeName, int paymentstatusIndex, string paymentstatusString, string exportType);
        */
        private void genReportOpenXml()
        {
            #region Data Prepare
            int fiscalYear = ddl_Year.selectedValueToInt;
            int fiscalWeek = ddl_Week.selectedValueToInt;
            int phaseId;

            TypeCollector phaseList = TypeCollector.Inclusive;
            if (ckb_UnknownSeason.Checked)
                phaseList.append(-1);
            for (int i = 0; i <= ddl_Phase.SelectedIndex; i++)
                phaseList.append(Int32.Parse(ddl_Phase.Items[i].Value));
            List<NSLedProfitabilitiesDef> list = AccountManager.Instance.getNSLedProfitabilities(uclOfficeSelection.getOfficeList(), fiscalYear, fiscalWeek, phaseList, ckb_StillSelling.Checked, ckb_NotYetLaunched.Checked, ckb_EndOfLife.Checked, -1, -1);

            if (list.Count == 0)
                return;

            list = list
                .OrderByDescending(x => x.ProductTeamName)
                .ThenBy(x => x.OfficeSortId)
                .ThenBy(x => x.IsNotYetLaunched)
                .ThenByDescending(x => x.IsEndOfLife)
                .ThenBy(x => x.ActualSaleSeasonId)
                .ThenBy(x => x.ActualSaleSeasonSplitId)
                .ThenByDescending(x => x.WeekCount)
                .ThenBy(x => x.ItemNo)
                .ThenBy(x => x.FirstInvoiceDate)
                
                .ToList();

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "NSLEDProfitabilities.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddHHmmss");
            string destFile = string.Format(this.ApplPhysicalPath + @"reporter\tmpReport\NSLEDProfitabilities-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            WworksheetID = OpenXmlUtil.getWorksheetId(document, "WWR");
            MworksheetID = OpenXmlUtil.getWorksheetId(document, "MWR");
            CworksheetID = OpenXmlUtil.getWorksheetId(document, "CWR");
            NFworksheetID = OpenXmlUtil.getWorksheetId(document, "NC-F");
            NAworksheetID = OpenXmlUtil.getWorksheetId(document, "NC-A");
            string templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Template");

            // first 1,2 = 8.11
            double col3Width = OpenXmlUtil.getColumnWidth(document, WworksheetID, 3);
            double col4Width = OpenXmlUtil.getColumnWidth(document, WworksheetID, 4);

            //Create cell style id dictionary
            Dictionary<string, int> styleDict = new Dictionary<string, int>();
            uint newItemStyleId = (uint)OpenXmlUtil.getCellStyleId(document, templateWorksheetID, "A1");
            styleDict.Add("WeekNo", OpenXmlUtil.getCellStyleId(document, WworksheetID, "A2"));
            styleDict.Add("Phasing", OpenXmlUtil.getCellStyleId(document, WworksheetID, "A3"));
            styleDict.Add("Season", OpenXmlUtil.getCellStyleId(document, WworksheetID, "A4"));
            styleDict.Add("SeasonFP", OpenXmlUtil.getCellStyleId(document, WworksheetID, "D5"));
            styleDict.Add("SeasonMD", OpenXmlUtil.getCellStyleId(document, WworksheetID, "D6"));
            styleDict.Add("SeasonProfit", OpenXmlUtil.getCellStyleId(document, WworksheetID, "D7"));
            for (int i = 1; i <= 4; i++)
            {
                string alpet = i == 1 ? "A" : i == 2 ? "B" : i == 3 ? "C" : "D";
                for (int j = 9; j <= 19; j++)
                {
                    styleDict.Add(alpet + j, OpenXmlUtil.getCellStyleId(document, WworksheetID, alpet + j));
                }
            }

            //Get ConditionalFormatting, use WworksheetID to prevent null
            ConditionalFormatting conditionalFormatting = OpenXmlUtil.getWorksheetPart(document, WworksheetID).Worksheet.Descendants<ConditionalFormatting>().First(x => x.SequenceOfReferences.InnerText == "D7");

            //Remove unused sheet
            if (!list.Any(x => x.ProductTeamName == "WOMENSWEAR"))
                document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == WworksheetID).Remove();
            if (!list.Any(x => x.ProductTeamName == "MENSWEAR"))
                document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == MworksheetID).Remove();
            if (!list.Any(x => x.ProductTeamName == "CHILDRENSWEAR"))
                document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == CworksheetID).Remove();
            if (!list.Any(x => x.ProductTeamName == "FOOTWEAR"))
                document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == NFworksheetID).Remove();
            if (!list.Any(x => x.ProductTeamName == "NON CLOTHING"))
                document.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == NAworksheetID).Remove();

            string lastProductTeam = ""; //Current product team
            string currentSheetId = "";
            string lastSeason = ""; //Season header
            int seasonCol = 1;  //Column for season header
            int seasonSummaryCol = seasonCol + 3;   //Column for season summary
            int seasonRowItemCount = 0; //Count for items in current season column
            int itemStartingRow = 1;    //Position of item
            int itemStartingCol = 1;    //Position of item
            decimal seasonTotalFP = 0;
            decimal seasonTotalMD = 0;
            decimal seasonTotalQty = 0;
            decimal seasonTotalProfit = 0;
            #endregion

            foreach (NSLedProfitabilitiesDef def in list)
            {
                string currentSeason = getSeasonString(def);
                #region New Product Team Phase
                if (def.ProductTeamName != lastProductTeam) //Product team change
                {
                    if (lastProductTeam != "") //update season summary when changing sheet
                    {
                        updateSeasonSummary(document, currentSheetId, seasonSummaryCol,
                            ref seasonTotalFP, ref seasonTotalMD, ref seasonTotalQty, ref seasonTotalProfit,
                            styleDict, conditionalFormatting);

                        //Add break for end of page
                        insertVerticalBreaks(document, currentSheetId, seasonSummaryCol);
                    }

                    lastProductTeam = def.ProductTeamName;  //record the current team
                    currentSheetId = getWorkSheetIdByDeptName(lastProductTeam); //get sheet id of new team

                    //Get ConditionalFormatting of currentSheetId
                    conditionalFormatting = OpenXmlUtil.getWorksheetPart(document, currentSheetId).Worksheet.Descendants<ConditionalFormatting>().First(x => x.SequenceOfReferences.InnerText == "D7");

                    //Set week no
                    OpenXmlUtil.setCellValue(document, currentSheetId, 2, 1, string.Format("As at Week {0}", fiscalWeek), CellValues.SharedString, styleDict["WeekNo"]);

                    string phasing = (ckb_StillSelling.Checked ? ", " + ckb_StillSelling.Text : "") + (ckb_NotYetLaunched.Checked ? ", " + ckb_NotYetLaunched.Text : "") + (ckb_EndOfLife.Checked ? ", " + ckb_EndOfLife.Text : "");
                    phasing = (phasing != "" ? " - " + phasing.Substring(2) : "");
                    phasing = ddl_Phase.SelectedValue + (ckb_UnknownSeason.Checked ? " (includes " + ckb_UnknownSeason.Text + ")" : "") + phasing;
                    OpenXmlUtil.setCellValue(document, currentSheetId, 3, 1, string.Format("Data starting from  phase {0}", phasing), CellValues.SharedString, styleDict["Phasing"]);

                    seasonCol = 1;  //reset season header
                    seasonSummaryCol = seasonCol + 3;   //reset season summary
                    seasonRowItemCount = 0; //reset season item count
                    itemStartingRow = 1; //reset position of item
                    itemStartingCol = 1; //reset position of item
                }
                #endregion

                #region New Season Phase
                if (seasonCol == 1 || lastSeason != currentSeason || seasonRowItemCount >= 10) //condition for open new season column
                {
                    if (lastSeason != currentSeason && seasonCol != 1) //update season summary when changing season
                    {
                        updateSeasonSummary(document, currentSheetId, seasonSummaryCol,
                            ref seasonTotalFP, ref seasonTotalMD, ref seasonTotalQty, ref seasonTotalProfit,
                            styleDict, conditionalFormatting);

                        seasonSummaryCol = seasonCol + 3; //update new season summary column
                    }

                    //Change column width for new season row
                    var columns = OpenXmlUtil.getWorksheetPart(document, currentSheetId).Worksheet.Elements<Columns>().First();
                    Column column1 = new Column() { Min = Convert.ToUInt32(seasonCol + 2), Max = Convert.ToUInt32(seasonCol + 2), Width = col3Width, CustomWidth = true };
                    Column column2 = new Column() { Min = Convert.ToUInt32(seasonCol + 3), Max = Convert.ToUInt32(seasonCol + 3), Width = col4Width, CustomWidth = true };
                    columns.Append(column1);
                    columns.Append(column2);

                    lastSeason = currentSeason; //update season header
                    OpenXmlUtil.setCellValue(document, currentSheetId, 4, seasonCol, currentSeason, CellValues.SharedString, styleDict["Season"]);

                    if (seasonCol == 1)
                        itemStartingCol = 1; //reset item column position
                    else
                        itemStartingCol += 4; //update new position of item

                    seasonRowItemCount = 0; //reset season item count
                    itemStartingRow = 1; //reset item row position
                    seasonCol += 4; //Prepare for next season column
                }
                #endregion

                #region Item Phase
                //Insert item data
                for (int i = 1; i <= 4; i++)
                {
                    string alpet = i == 1 ? "A" : i == 2 ? "B" : i == 3 ? "C" : "D";
                    for (int j = 9; j <= 19; j++)
                    {
                        CellValues cellValues = CellValues.SharedString;
                        OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + j - 1, itemStartingCol + i - 1,
                            getTextByPosition(def, alpet + j, out cellValues), cellValues, styleDict[alpet + j]);
                        if (alpet + j == "B18" && def.FPWeekCount == 1)
                        {
                            OpenXmlUtil.setCellValue(document, currentSheetId, itemStartingRow + j - 1, itemStartingCol + i - 1, "NEW", CellValues.SharedString);
                            OpenXmlUtil.setCellStyle(document, currentSheetId, itemStartingRow + j - 1, itemStartingCol + i - 1, newItemStyleId);
                        }

                        if (alpet + j == "D18") //Set ConditionalFormatting
                        {
                            conditionalFormatting.SequenceOfReferences.Items.Add(OpenXmlUtil.getCellReference(itemStartingCol + i - 1, itemStartingRow + j - 1));
                        }
                    }
                }
                //Add image
                byte[] resizedPicture = NSLedRangePlanDef.ResizeImage(def.Picture, 90, 135);
                OpenXmlUtil.addImage(document, currentSheetId, resizedPicture, def.ItemNo, itemStartingCol, itemStartingRow + 9 - 1, 50000, 50000);

                //Prepare of season summary
                seasonTotalFP += def.FPQty;
                seasonTotalMD += def.MDQty;
                seasonTotalQty += def.TotalQty;
                seasonTotalProfit += def.TotalNSCommissionAmt - def.TotalProductionCost;

                itemStartingRow += 11; //Prepare for next item
                seasonRowItemCount++;
                #endregion
            }

            #region Ending Phase
            //Season summary update before exist excel
            updateSeasonSummary(document, currentSheetId, seasonSummaryCol, 
                ref seasonTotalFP, ref seasonTotalMD, ref seasonTotalQty, ref seasonTotalProfit, 
                styleDict, conditionalFormatting);

            //Add break for end of page
            insertVerticalBreaks(document, currentSheetId, seasonSummaryCol);

            OpenXmlUtil.saveComplete(document);

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
            #endregion
        }

        private void updateSeasonSummary(SpreadsheetDocument document, string currentSheetId, int seasonSummaryCol, 
            ref decimal seasonTotalFP, ref decimal seasonTotalMD, ref decimal seasonTotalQty, ref decimal seasonTotalProfit, 
            Dictionary<string,int> styleDict, ConditionalFormatting format)
        {
            //Set season summary
            OpenXmlUtil.setCellValue(document, currentSheetId, 5, seasonSummaryCol - 1, "FP Sell Thru", CellValues.SharedString);
            OpenXmlUtil.setCellValue(document, currentSheetId, 5, seasonSummaryCol, seasonTotalQty > 0 ? (seasonTotalFP / seasonTotalQty).ToString() : "0", CellValues.Number, styleDict["SeasonFP"]);

            OpenXmlUtil.setCellValue(document, currentSheetId, 6, seasonSummaryCol - 1, "MD Sell Thru", CellValues.SharedString);
            OpenXmlUtil.setCellValue(document, currentSheetId, 6, seasonSummaryCol, seasonTotalQty > 0 ? (seasonTotalMD / seasonTotalQty).ToString() : "0", CellValues.Number, styleDict["SeasonMD"]);

            OpenXmlUtil.setCellValue(document, currentSheetId, 7, seasonSummaryCol - 1, "Profit / (Loss)", CellValues.SharedString);
            OpenXmlUtil.setCellValue(document, currentSheetId, 7, seasonSummaryCol, seasonTotalProfit.ToString(), CellValues.Number, styleDict["SeasonProfit"]);

            seasonTotalFP = 0;
            seasonTotalMD = 0;
            seasonTotalQty = 0;
            seasonTotalProfit = 0;

            //Set ConditionalFormatting
            format.SequenceOfReferences.Items.Add(OpenXmlUtil.getCellReference(seasonSummaryCol, 7));
        }

        private void insertVerticalBreaks(SpreadsheetDocument document, string sheetId, int lastColId)
        {
            //break every 4 season
            int noOfBreaks = (lastColId / 16);
            if (noOfBreaks > 0)
            {
                for (int i = 1; i <= noOfBreaks; i++)
                    OpenXmlUtil.insertVerticalPageBreak(document, sheetId, i * 16);
            }

        }

        private string getTextByPosition(NSLedProfitabilitiesDef def, string position, out CellValues cellValues)
        {
            cellValues = CellValues.SharedString;
            string value = "";
            switch (position)
            {
                case "A16":
                    value = "UK";
                    break;
                case "A17":
                    value = "INT";
                    break;
                case "A18":
                    value = def.FPWeekCount + " Wks";
                    break;
                case "A19":
                    value = "Model Profitability";
                    break;
                case "B16":
                    value = def.UKQtyPct.ToString();
                    cellValues = CellValues.Number;
                    break;
                case "B17":
                    value = def.IntQtyPct.ToString();
                    cellValues = CellValues.Number;
                    break;
                case "C9":
                    value = "Item No.";
                    break;
                case "C10":
                    value = "Description";
                    break;
                case "C11":
                    value = "Qty";
                    break;
                case "C12":
                    value = "FP Sell Thru";
                    break;
                case "C13":
                    value = "MD Sell Thru";
                    break;
                case "C14":
                    value = "RSV";
                    break;
                case "C15":
                    value = "MD Price";
                    break;
                case "C16":
                    value = "Total Commission";
                    break;
                case "C17":
                    value = "Cost";
                    break;
                case "C18":
                    value = "Profit / (Loss)";
                    break;
                case "C19":
                    value = (def.EstimatedProfitPencentage/100).ToString();
                    cellValues = CellValues.Number;
                    break;
                case "D9":
                    value = def.ItemNo;
                    cellValues = CellValues.Number;
                    break;
                case "D10":
                    value = def.Description + (def.SeasonCount > 1 ? " (repeat)" : "");
                    break;
                case "D11":
                    value = def.TotalQty.ToString();
                    cellValues = CellValues.Number;
                    break;
                case "D12":
                    value = def.FPQtyPct.ToString();
                    cellValues = CellValues.Number;
                    break;
                case "D13":
                    value = def.MDQtyPct.ToString();
                    cellValues = CellValues.Number;
                    break;
                case "D14":
                    value = def.RetailSellingPrice;
                    break;
                case "D15":
                    value = def.MDPrice;
                    break;
                case "D16":
                    value = def.TotalNSCommissionAmt.ToString();
                    cellValues = CellValues.Number;
                    break;
                case "D17":
                    value = (-def.TotalProductionCost).ToString();
                    cellValues = CellValues.Number;
                    break;
                case "D18":
                    value = (def.TotalNSCommissionAmt - def.TotalProductionCost).ToString();
                    cellValues = CellValues.Number;
                    break;
                case "D19":
                    if (def.TotalNSCommissionAmt == 0)
                        value = "0";
                    else
                        value = (1 - def.TotalProductionCost / def.TotalNSCommissionAmt).ToString();
                    cellValues = CellValues.Number;
                    break;
            }
            return value;
        }

        private string getWorkSheetIdByDeptName(string deptName)
        {
            switch (deptName)
            {
                case "WOMENSWEAR":
                    return WworksheetID;
                case "MENSWEAR":
                    return MworksheetID;
                case "CHILDRENSWEAR":
                    return CworksheetID;
                case "FOOTWEAR":
                    return NFworksheetID;
                case "NON CLOTHING":
                    return NAworksheetID;
            }
            return "";
        }

        private string getSeasonString(NSLedProfitabilitiesDef def)
        {
            if (def.WeekCount == 0)
                return string.Format("Not Yet Launched ({0})", CommonUtil.getOfficeRefByKey(def.OfficeId).OfficeCode);
            else if (def.IsEndOfLife)
            {
                return string.Format("End of life ({0}) - {1}", CommonUtil.getOfficeRefByKey(def.OfficeId).OfficeCode, def.ActualSaleSeasonWithPhase);
            }
            else
            {
                return string.Format("Still selling ({0})", CommonUtil.getOfficeRefByKey(def.OfficeId).OfficeCode);
            }
        }
    }
}