using System;
using System.Data;
using System.Web.UI.WebControls;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.appserver.common;
using com.next.isam.domain.common;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using com.next.infra.util;
using com.next.common.domain.types;


namespace com.next.isam.webapp.reporter
{
    public partial class CarbonFootprintReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.ddl_Office.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());
                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);

                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 1).ToString(), (calenderDef.BudgetYear - 1).ToString()));
                this.ddl_FiscalYear.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_FiscalYear.selectByValue(calenderDef.BudgetYear.ToString());

                for (int i = 1; i <= 12; i++)
                {
                    this.ddl_Period.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(), i.ToString()));
                }
                
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            Page.Validate();

            if (Page.IsValid)
            {
                string sourceFileDir = this.ApplPhysicalPath + @"reporter\";
                string sourceFileName = "CarbonFootprintReport.xlsx";
                string uId = DateTime.Now.ToString("yyyyMMddss");
                string destFile = String.Format(this.ApplPhysicalPath + @"reporter\tmpReport\CarbonFootprintReport-{0}-{1}.xlsx", this.LogonUserId.ToString(), uId);
                File.Copy(sourceFileDir + sourceFileName, destFile, true);

                processFile(destFile);

                WebHelper.outputFileAsHttpRespone(Response, destFile, true);
            }
        }

        private void fillCarbonFootprintReportDataCategory(SpreadsheetDocument document, string worksheetID, CarbonFootprintReportDataCategory category, int detailColCount, int mainRowNo)
        {
            worksheetID = OpenXmlUtil.getWorksheetId(document, worksheetID);
            if (worksheetID == string.Empty)
                Console.WriteLine("failure open spreadsheet");
            DataSet dataSet = CarbonFootprintManager.Instance.getCarbonFootprintReportDataByCategory(category, this.ddl_Office.selectedValueToInt, this.ddl_FiscalYear.selectedValueToInt, this.ddl_Period.selectedValueToInt);

            int i = 2;
            decimal noOfUnit = 0;

            foreach (DataRow r in dataSet.Tables[0].Rows)
            {
                OpenXmlUtil.setCellValue(document, worksheetID, i, 1, r[0].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 2, OfficeId.getName(int.Parse(r[1].ToString())), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 3, r[2].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 4, r[3].ToString(), CellValues.Date);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 5, r[4].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 6, r[5].ToString(), CellValues.Number);
                noOfUnit += decimal.Parse(r[5].ToString());
                i++;
            }
            OpenXmlUtil.setTableReference(document, worksheetID, "A1:F" + (i - 1).ToString());

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Data Entry");

            OpenXmlUtil.setCellValue(document, worksheetID, mainRowNo, 5, noOfUnit.ToString(), CellValues.Number);
        }


        private void fillCarbonFootprintReportTravelDataCategory(SpreadsheetDocument document, string worksheetID, CarbonFootprintReportDataCategory category, int detailColCount, int mainRowNo)
        {
            worksheetID = OpenXmlUtil.getWorksheetId(document, worksheetID);
            if (worksheetID == string.Empty)
                Console.WriteLine("failure open spreadsheet");
            DataSet dataSet = CarbonFootprintManager.Instance.getCarbonFootprintReportDataByCategory(category, this.ddl_Office.selectedValueToInt, this.ddl_FiscalYear.selectedValueToInt, this.ddl_Period.selectedValueToInt);

            int i = 2;
            decimal cnt = 0;

            foreach (DataRow r in dataSet.Tables[0].Rows)
            {
                OpenXmlUtil.setCellValue(document, worksheetID, i, 1, "TEMS", CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 2, r[0].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 3, r[1].ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 4, r[2].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 5, r[3].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 6, r[4].ToString(), CellValues.Date);
                cnt += 1;
                i++;
            }
            OpenXmlUtil.setTableReference(document, worksheetID, "A1:E" + (i - 1).ToString());

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Data Entry");

            OpenXmlUtil.setCellValue(document, worksheetID, mainRowNo, 5, cnt.ToString(), CellValues.Number);

        }

        private void fillCarbonFootprintReportCompanyMileageCategory(SpreadsheetDocument document, string worksheetID, CarbonFootprintReportDataCategory category, int detailColCount, int mainRowNo)
        {
            worksheetID = OpenXmlUtil.getWorksheetId(document, worksheetID);
            if (worksheetID == string.Empty)
                Console.WriteLine("failure open spreadsheet");
            DataSet dataSet = CarbonFootprintManager.Instance.getCarbonFootprintReportDataByCategory(category, this.ddl_Office.selectedValueToInt, this.ddl_FiscalYear.selectedValueToInt, this.ddl_Period.selectedValueToInt);

            int i = 2;
            decimal noOfUnit = 0;

            foreach (DataRow r in dataSet.Tables[0].Rows)
            {
                OpenXmlUtil.setCellValue(document, worksheetID, i, 1, r[0].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 2, OfficeId.getName(int.Parse(r[1].ToString())), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 3, r[2].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 4, r[3].ToString(), CellValues.Date);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 5, r[4].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 6, r[5].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 7, r[6].ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 8, r[7].ToString(), CellValues.Number);
                noOfUnit += decimal.Parse(r[7].ToString());
                i++;
            }
            OpenXmlUtil.setTableReference(document, worksheetID, "A1:H" + (i - 1).ToString());

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Data Entry");

            OpenXmlUtil.setCellValue(document, worksheetID, mainRowNo, 5, noOfUnit.ToString(), CellValues.Number);

        }


        private void fillCarbonFootprintReportTransportationDataCategory(SpreadsheetDocument document, string worksheetID, CarbonFootprintReportDataCategory category, int detailColCount, int mainRowNo)
        {
            worksheetID = OpenXmlUtil.getWorksheetId(document, worksheetID);
            if (worksheetID == string.Empty)
                Console.WriteLine("failure open spreadsheet");
            DataSet dataSet = CarbonFootprintManager.Instance.getCarbonFootprintReportDataByCategory(category, this.ddl_Office.selectedValueToInt, this.ddl_FiscalYear.selectedValueToInt, this.ddl_Period.selectedValueToInt);

            int i = 2;
            decimal cnt = 0;

            foreach (DataRow r in dataSet.Tables[0].Rows)
            {
                OpenXmlUtil.setCellValue(document, worksheetID, i, 1, "TEMS", CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 2, r[0].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 3, r[1].ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 4, r[2].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 5, r[3].ToString(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 6, r[4].ToString(), CellValues.Number);
                cnt += int.Parse(r[4].ToString());
                i++;
            }
            OpenXmlUtil.setTableReference(document, worksheetID, "A1:E" + (i - 1).ToString());

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Data Entry");

            OpenXmlUtil.setCellValue(document, worksheetID, mainRowNo, 5, cnt.ToString(), CellValues.Number);

        }

        private void processFile(string file)
        {
            string worksheetID = string.Empty;

            SpreadsheetDocument document = SpreadsheetDocument.Open(file, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Data Entry");

            OpenXmlUtil.setCellValue(document, worksheetID, 3, 5, DateTime.Now.ToString("dd/MM/yyyy"), CellValues.Date);

            fillCarbonFootprintReportDataCategory(document, "Electricity_Consumption", CarbonFootprintReportDataCategory.Electricity, 6, 8);
            fillCarbonFootprintReportDataCategory(document, "Water Consumption", CarbonFootprintReportDataCategory.Water, 6, 17);
            fillCarbonFootprintReportCompanyMileageCategory(document, "Diesel Cars", CarbonFootprintReportDataCategory.CompanyDiesel, 6, 21);
            fillCarbonFootprintReportCompanyMileageCategory(document, "Petrol Cars", CarbonFootprintReportDataCategory.CompanyPetrol, 6, 22);
            fillCarbonFootprintReportCompanyMileageCategory(document, "Petrol Hire Car", CarbonFootprintReportDataCategory.CompanyPetrol, 6, 32);
            fillCarbonFootprintReportTravelDataCategory(document, "Air Travel Long", CarbonFootprintReportDataCategory.AirTravelLong, 4, 27);
            fillCarbonFootprintReportTravelDataCategory(document, "Air Travel Short", CarbonFootprintReportDataCategory.AirTravelShort, 4, 28);
            fillCarbonFootprintReportTransportationDataCategory(document, "Taxi Hire", CarbonFootprintReportDataCategory.Taxi, 4, 35);
            fillCarbonFootprintReportTransportationDataCategory(document, "Bus", CarbonFootprintReportDataCategory.Bus, 4, 29);
            fillCarbonFootprintReportTransportationDataCategory(document, "National Rail Network", CarbonFootprintReportDataCategory.Train, 4, 34);

            /*
            OpenXmlUtil.copyAndInsertRow(document, worksheetID, 7, 33);
            OpenXmlUtil.copyAndInsertRow(document, worksheetID, 33, 33);
            */

            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();
        }


        protected void valCustom_ServerValidate(object source, ServerValidateEventArgs args)
        {

        }
    }
}
