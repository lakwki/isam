using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.isam.domain.types;
using com.next.isam.domain.nontrade;
using com.next.infra.web;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.accounts ;
using System.IO;
using com.next.infra.util;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using com.next.common.datafactory.worker;

namespace com.next.isam.webapp.nontrade
{
    public partial class NonTradeVendorSearch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                ddl_Office.bindList(WebUtil.getNTOfficeList(this.LogonUserId), "OfficeCode", "OfficeId", this.LogonUserHomeOffice.OfficeId.ToString());

                ddl_ExpenseType.bindList(WebUtil.getNTExpenseTypeListByOfficeId(ddl_Office.selectedValueToInt), "Description", "ExpenseTypeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());

                txt_SupplierName.setWidth(305);
                txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.ntVendor);

                ddl_Status.bindList(NTVendorWFS.getCollectionValues(), "Name", "Id", "", "-- ALL --", GeneralCriteria.ALL.ToString());
            }
        }

        protected void ddl_Office_SelectedIndexChange(object sender, EventArgs e)
        {
            ddl_ExpenseType.bindList(WebUtil.getNTExpenseTypeListByOfficeId(ddl_Office.selectedValueToInt), "Description", "ExpenseTypeId", "", "-- ALL --", GeneralCriteria.ALL.ToString());
        }

        protected void Search_OnClick(object sender, EventArgs e)
        {
            ArrayList ntVendorList = getSearchResult();

            pnl_Result.Visible = true;
            gv_NTVendorList.DataSource = ntVendorList;
            gv_NTVendorList.DataBind();
        }

        protected void Reset_OnClick(object sender, EventArgs e)
        {
            ddl_Office.SelectedIndex = -1;
            ddl_ExpenseType.SelectedIndex = -1;
            txt_SupplierName.NTVendorId = int.MinValue;

            pnl_Result.Visible = false;
            gv_NTVendorList.DataSource = null;
            gv_NTVendorList.DataBind();
        }

        protected NTVendorReport  genReport()
        {
            int officeId = int.Parse(ddl_Office.SelectedValue);
            int ntVendorId = txt_SupplierName.NTVendorId == int.MinValue ? -1 : txt_SupplierName.NTVendorId;
            int expenseTypeId = int.Parse(ddl_ExpenseType.SelectedValue);
            int status = int.Parse(ddl_Status.SelectedValue);


            return AccountReportManager.Instance.getNTVendorReport(officeId, ntVendorId, expenseTypeId, status);
        }


        protected void btn_Export_Click(object sender, EventArgs e)
        {
            ReportHelper.export(genReport(), HttpContext.Current.Response,
                CrystalDecisions.Shared.ExportFormatType.Excel, "Non-Trade Vendor List");
        }

        private ArrayList getSearchResult()
        {
            int officeId = int.Parse(ddl_Office.SelectedValue);
            int ntVendorId = txt_SupplierName.NTVendorId == int.MinValue ? -1 : txt_SupplierName.NTVendorId;
            int expenseTypeId = int.Parse(ddl_ExpenseType.SelectedValue);
            int status = int.Parse(ddl_Status.SelectedValue);

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetNTVendorList);

            Context.Items.Add(AccountCommander.Param.officeId, officeId);
            Context.Items.Add(AccountCommander.Param.ntVendorId, ntVendorId);
            Context.Items.Add(AccountCommander.Param.expenseTypeId, expenseTypeId);
            Context.Items.Add(AccountCommander.Param.workflowStatusId, status);

            forwardToScreen(null);

            return (ArrayList)Context.Items[AccountCommander.Param.ntVendorList];
        }

        /*
        protected void btn_Print_Click(object sender, EventArgs e)
        {
            //ReportHelper.export(genReport(), HttpContext.Current.Response,
            //    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "Non-Trade Vendor List");

            ArrayList ntVendorList = getSearchResult();

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "NTVendor.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(this.ApplPhysicalPath + @"reporter\tmpReport\NTVendor-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet1");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");


            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(this.LogonUserId).DisplayName, CellValues.String);
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.Date);
            int noOfDetailCols = 11;

            int[] list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 3, noOfDetailCols);

            int startingRow = 2;
            foreach (NTVendorDef def in ntVendorList)
            {
                string address = string.Empty;
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 1, def.VendorTypeId == 10 ? "Bulk" : "Non-Trade", CellValues.SharedString, list[0]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 2, def.VendorName, CellValues.SharedString, list[1]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 3, def.EPVendorCode, CellValues.SharedString, list[2]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 4, def.Address, CellValues.SharedString, list[3]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 5, def.Country != null ? def.Country.Name : "N/A", CellValues.SharedString, list[4]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 6, def.Telephone, CellValues.SharedString, list[5]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 7, def.Fax, CellValues.SharedString, list[6]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 8, def.Currency != null ? def.Currency.CurrencyCode : "N/A", CellValues.SharedString, list[7]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 9, def.ExpenseType != null ? def.ExpenseType.Description : "N/A", CellValues.SharedString, list[8]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 10, def.CompanyId != -1 ?  GeneralWorker.Instance.getCompanyOfficeMappingByCriteria(def.CompanyId, def.Office.OfficeId).EpicorCompanyId : "N/A", CellValues.SharedString, list[9]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 11, def.WorkflowStatus.Name, CellValues.SharedString, list[10]);

                startingRow += 1;
                System.Diagnostics.Debug.Print("starting row : " + startingRow.ToString());
            }

            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 5, worksheetID, startingRow + 1);
            OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(1, startingRow + 1), OpenXmlUtil.getCellReference(noOfDetailCols, startingRow + 1));

            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }
        */
        
        protected void btn_Print_Click(object sender, EventArgs e)
        {
            //ReportHelper.export(genReport(), HttpContext.Current.Response,
            //    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "Non-Trade Vendor List");

            ArrayList ntVendorList = getSearchResult();

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "NTVendor.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(this.ApplPhysicalPath + @"reporter\tmpReport\NTVendor-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet1");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");


            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(this.LogonUserId).DisplayName, CellValues.String);
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.Date);
            int noOfDetailCols = 11;

            int[] list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 3, noOfDetailCols);

            int startingRow = 2;
            foreach (NTVendorDef def in ntVendorList)
            {
                string address = string.Empty;
                List<OpenXmlCell> cellValueList = new List<OpenXmlCell>();
                cellValueList.Add(new OpenXmlCell(startingRow, 1, CellValues.SharedString, list[0], def.VendorTypeId == 10 ? "Bulk" : "Non-Trade"));
                cellValueList.Add(new OpenXmlCell(startingRow, 2, CellValues.SharedString, list[1], def.VendorName));
                cellValueList.Add(new OpenXmlCell(startingRow, 3, CellValues.SharedString, list[2], def.EPVendorCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 4, CellValues.SharedString, list[3], def.Address));
                cellValueList.Add(new OpenXmlCell(startingRow, 5, CellValues.SharedString, list[4], def.Country != null ? def.Country.Name : "N/A"));
                cellValueList.Add(new OpenXmlCell(startingRow, 6, CellValues.SharedString, list[5], def.Fax));
                cellValueList.Add(new OpenXmlCell(startingRow, 7, CellValues.SharedString, list[6], def.Telephone));
                cellValueList.Add(new OpenXmlCell(startingRow, 8, CellValues.SharedString, list[7], def.Currency != null ? def.Currency.CurrencyCode : "N/A"));
                cellValueList.Add(new OpenXmlCell(startingRow, 9, CellValues.SharedString, list[8], def.ExpenseType != null ? def.ExpenseType.Description : "N/A"));
                cellValueList.Add(new OpenXmlCell(startingRow, 10, CellValues.SharedString, list[9], def.CompanyId != -1 ? GeneralWorker.Instance.getCompanyOfficeMappingByCriteria(def.CompanyId, def.Office.OfficeId).EpicorCompanyId : "N/A"));
                cellValueList.Add(new OpenXmlCell(startingRow, 11, CellValues.SharedString, list[10], def.WorkflowStatus.Name));

                OpenXmlUtil.createRowAndCells(document, worksheetID, startingRow, 1, cellValueList);

                startingRow += 1;
                System.Diagnostics.Debug.Print("starting row : " + startingRow.ToString());
            }

            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 5, worksheetID, startingRow + 1);
            OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(1, startingRow + 1), OpenXmlUtil.getCellReference(noOfDetailCols, startingRow + 1));

            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }
        
    }
}