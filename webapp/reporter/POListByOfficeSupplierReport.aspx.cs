using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.isam.webapp.webservices;
using com.next.isam.domain.types;
using System.Collections;
using com.next.isam.reporter.accounts;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using com.next.infra.util;
using com.next.isam.webapp.commander;
using com.next.common.domain.types;

namespace com.next.isam.webapp.reporter
{
    public partial class POListByOfficeSupplierReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                initControl();
            }
        }
        private void bindCheckBoxList(CheckBoxList cbl, ICollection list, string textField, string valueField)
        {
            bindCheckBoxList(cbl, list, textField, valueField, null, null);
        }

        private void bindCheckBoxList(CheckBoxList cbl, ICollection list, string textField, string valueField, string selectAllFieldText, string selectAllFieldValue)
        {
            cbl.DataSource = list;
            cbl.DataTextField = textField;
            cbl.DataValueField = valueField;
            cbl.DataBind();
            if (selectAllFieldText != null && selectAllFieldValue != null)
            {
                cbl.Items.Insert(0, new ListItem(selectAllFieldText, selectAllFieldValue));
                cbl.Items[0].Attributes.Add("onclick", "clickAll(this);");
            }
            cbl.Attributes.Add("onclick", "refreshCheckBox(this)");

            foreach (ListItem itm in cbl.Items)
            {
                if (itm.Value != ContractWFS.CANCELLED.Id.ToString() && itm.Value != ContractWFS.REJECTED.Id.ToString() && itm.Value != ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id.ToString())
                    itm.Selected = true;
            }
        }
        public class PaymentStatus
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        protected void initControl()
        {
            AccountFinancialCalenderDef finCalDef = CommonUtil.getCurrentFinancialPeriod(9);
            ddl_Year.DataSource = WebUtil.getBudgetYearList();
            ddl_Year.DataBind();
            ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();
            ddlOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--ALL--", GeneralCriteria.ALL.ToString());
            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(UclSmartSelection.SelectionList.garmentVendor);
            ddl_BaseCurrency.bindList(WebUtil.getCurrencyListForExchangeRate(), "CurrencyCode", "CurrencyId", GeneralCriteria.ALL.ToString(), "--ALL--", GeneralCriteria.ALL.ToString());
            bindCheckBoxList(cbl_ShipmentStatus, ContractWFS.getCollectionValue(), "Name", "Id", "ALL", GeneralCriteria.ALL.ToString());
            //IList statuslist = new List<PaymentStatus>()
            //{
            // new PaymentStatus(){ ID=-1, Name="ALL"},
            // new PaymentStatus(){ ID=ContractWFS.PENDING_FOR_SUBMIT.Id, Name=ContractWFS.PENDING_FOR_SUBMIT.Name},
            // new PaymentStatus(){ ID=ContractWFS.PENDING_FOR_APPROVAL.Id, Name=ContractWFS.PENDING_FOR_APPROVAL.Name},
            // new PaymentStatus(){ ID=ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id, Name=ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Name},
            // new PaymentStatus(){ ID=ContractWFS.AMEND.Id, Name=ContractWFS.AMEND.Name},
            // new PaymentStatus(){ ID=ContractWFS.REJECTED.Id, Name=ContractWFS.REJECTED.Name},
            // new PaymentStatus(){ ID=ContractWFS.APPROVED.Id, Name=ContractWFS.APPROVED.Name},
            // new PaymentStatus(){ ID=ContractWFS.PO_PRINTED.Id, Name=ContractWFS.PO_PRINTED.Name},
            // new PaymentStatus(){ ID=ContractWFS.INVOICED.Id, Name=ContractWFS.INVOICED.Name},
            // new PaymentStatus(){ ID=ContractWFS.CANCELLED.Id, Name=ContractWFS.CANCELLED.Name},
            //};
            //ddlStatus.DataSource = statuslist;
            //ddlStatus.DataTextField = "Name";
            //ddlStatus.DataValueField = "ID";
            //ddlStatus.DataBind();
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            string strExportType = "Excel";
            CrystalDecisions.Shared.ExportFormatType exportType = CrystalDecisions.Shared.ExportFormatType.Excel;
            exportAdvancePaymentReport(strExportType, exportType);
        }

        private void exportAdvancePaymentReport(string strExportType, CrystalDecisions.Shared.ExportFormatType exportType)
        {
            //if (exportType == CrystalDecisions.Shared.ExportFormatType.Excel && rbVersion.SelectedIndex == 1)
            genReportOpenXml(strExportType);
            //else
            //    ReportHelper.export(genReport(ddlVersionSelectedIndex, strExportType), HttpContext.Current.Response, exportType, "Advance Payment Report");

        }

        //private ReportClass genReport(int selectedIndex, string exportType)
        //{
        //    DateTime paymentDateFrom = DateTime.MinValue;
        //    DateTime paymentDateTo = DateTime.MinValue;
        //    DateTime deductionDateFrom = DateTime.MinValue;
        //    DateTime deductionDateTo = DateTime.MinValue;

        //    int vendorId = -1;
        //    string vendorName = "ALL";
        //    int officeId = -1;
        //    string officeName = "ALL";
        //    int paymentstatusIndex = 0;
        //    string paymentstatusString = "ALL";


        //    if (txt_PaymentDateFrom.Text.Trim() != "" || txt_PaymentDateTo.Text.Trim() != "")
        //    {
        //        paymentDateFrom = DateTime.Parse(txt_PaymentDateFrom.Text);
        //        paymentDateTo = DateTime.Parse(txt_PaymentDateTo.Text);
        //    }

        //    if (txt_DeductionDateFrom.Text.Trim() != "" || txt_DeductionDateTo.Text.Trim() != "")
        //    {
        //        deductionDateFrom = DateTime.Parse(txt_DeductionDateFrom.Text);
        //        deductionDateTo = DateTime.Parse(txt_DeductionDateTo.Text);
        //    }


        //    if (txt_Supplier.VendorId != int.MinValue)
        //    {
        //        vendorName = txt_Supplier.ToString();
        //        vendorId = txt_Supplier.VendorId;
        //    }

        //    if (ddlOffice.SelectedIndex != 0)
        //    {
        //        officeId = int.Parse(ddlOffice.SelectedValue);
        //        officeName = CommonUtil.getOfficeRefByKey(officeId).Description;
        //    }


        //    paymentstatusIndex = int.Parse(ddlStatus.SelectedValue);
        //    paymentstatusString = ddlStatus.SelectedItem.Text;

        //    GetAdvancePaymentReportMethod getAdvancePaymentReportMethod;
        //    switch (selectedIndex)
        //    {
        //        case 1:
        //            getAdvancePaymentReportMethod = AccountReportManager.Instance.getAdvancePaymentReportMG;
        //            break;
        //        case 0:
        //        default:
        //            getAdvancePaymentReportMethod = AccountReportManager.Instance.getAdvancePaymentReport;
        //            break;
        //    }

        //    return getAdvancePaymentReportMethod(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, vendorName, officeId, officeName, paymentstatusIndex, paymentstatusString, exportType);
        //}
        private delegate ReportClass GetAdvancePaymentReportMethod(DateTime paymentDateFrom, DateTime paymentDateTo, DateTime deductionDateFrom, DateTime deductionDateTo, int vendorId, string vendorName, int officeId, string officeName, int paymentstatusIndex, string paymentstatusString, string exportType);

        private void genReportOpenXml(string exportType)
        {
            int budgetYear = -1;
            int fiscalPeriod = -1;
            int vendorId = -1;
            string vendorName = "ALL";
            int officeId = -1;
            string officeName = "ALL";
            int baseCurrencyId = -1;
            //int paymentstatusIndex = 0;
            //string paymentstatusString = "ALL";
            TypeCollector shipmentStatus = TypeCollector.Inclusive;

            budgetYear = Convert.ToInt32(ddl_Year.SelectedValue);
            fiscalPeriod = Convert.ToInt32(ddl_Period.SelectedValue);

            if (ddlOffice.SelectedIndex != 0)
            {
                officeId = int.Parse(ddlOffice.SelectedValue);
                officeName = CommonUtil.getOfficeRefByKey(officeId).Description;
            }

            if (txt_Supplier.VendorId != int.MinValue)
            {
                vendorName = IndustryUtil.getVendorByKey(txt_Supplier.VendorId).Name;
                vendorId = txt_Supplier.VendorId;
            }

            if (ddl_BaseCurrency.SelectedIndex != 0)
            {
                baseCurrencyId = Convert.ToInt32(ddl_BaseCurrency.SelectedValue);
            }

            foreach (ListItem itm in cbl_ShipmentStatus.Items) if (itm.Selected) shipmentStatus.append(Convert.ToInt32(itm.Value));

            //paymentstatusIndex = int.Parse(ddlStatus.SelectedValue);
            //paymentstatusString = ddlStatus.SelectedItem.Text;

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "POListByOfficeSupplier.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(this.ApplPhysicalPath + @"reporter\tmpReport\POListByOfficeSupplier-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;
            string summaryWorksheetID = string.Empty;

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Main");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");

            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(this.LogonUserId).DisplayName, CellValues.String);
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.Date);

            int criteriaStyleId = OpenXmlUtil.getCellStyleId(document, worksheetID, "C1");
            int headerStyleId = OpenXmlUtil.getCellStyleId(document, worksheetID, "E5");

            OpenXmlUtil.setCellValue(document, worksheetID, 1, 3, officeName, CellValues.SharedString, criteriaStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 2, 3, vendorName, CellValues.SharedString, criteriaStyleId);
            string desc = string.Empty;
            ArrayList statuslist = (ArrayList)shipmentStatus.Values;
            int keyId;
            for (int i = 0; i < statuslist.Count && desc != "ALL"; i++)
                desc = ((keyId = (int)statuslist[i]) == -1 ? "ALL" : desc + (desc == string.Empty ? "" : ", ") + ContractWFS.getType(keyId).Name);
            OpenXmlUtil.setCellValue(document, worksheetID, 3, 3, desc, CellValues.SharedString, criteriaStyleId);

            int[] list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 7, 22);

            POListByOfficeSupplierReportDs ds = AccountReportManager.Instance.getPOListByOfficeSupplierReport(budgetYear, fiscalPeriod, officeId, vendorId, baseCurrencyId, shipmentStatus);

            string p01 = "N/A";
            string p02 = "N/A";
            string p03 = "N/A";
            string p04 = "N/A";
            string p05 = "N/A";
            string p06 = "N/A";
            string p07 = "N/A";
            string p08 = "N/A";
            string p09 = "N/A";

            int startingRow = 7;
            int detailCount = ds.POListByOfficeSupplierReport.Rows.Count;
            int cnt = 1;
            string currentOfficeCode = string.Empty;
            string currentVendorName = string.Empty;

            foreach (POListByOfficeSupplierReportDs.POListByOfficeSupplierReportRow r in ds.POListByOfficeSupplierReport.Rows)
            {
                if (r.P1_Period != "N/A") p01 = r.P1_Period;
                if (r.P2_Period != "N/A") p02 = r.P2_Period;
                if (r.P3_Period != "N/A") p03 = r.P3_Period;
                if (r.P4_Period != "N/A") p04 = r.P4_Period;
                if (r.P5_Period != "N/A") p05 = r.P5_Period;
                if (r.P6_Period != "N/A") p06 = r.P6_Period;
                if (r.P7_Period != "N/A") p07 = r.P7_Period;
                if (r.P8_Period != "N/A") p08 = r.P8_Period;
                if (r.P9_Period != "N/A") p09 = r.P9_Period;

                if (r.IsOfficeCodeNull())
                    continue;

                if ((currentOfficeCode != r.OfficeCode || currentVendorName != r.SupplierName) && cnt != 1)
                    startingRow += 1;

                List<OpenXmlCell> cellValueList = new List<OpenXmlCell>();
                cellValueList.Add(new OpenXmlCell(startingRow, 1, CellValues.SharedString, list[0], r.OfficeCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 2, CellValues.SharedString, list[1], r.SupplierName));
                cellValueList.Add(new OpenXmlCell(startingRow, 3, CellValues.SharedString, list[2], string.Empty));
                cellValueList.Add(new OpenXmlCell(startingRow, 4, CellValues.SharedString, list[3], string.Empty));
                cellValueList.Add(new OpenXmlCell(startingRow, 5, CellValues.SharedString, list[4], r.P1_Amt == 0 ? string.Empty : r.CurrencyCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 6, CellValues.Number, list[5], r.P1_Amt.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 7, CellValues.SharedString, list[6], r.P2_Amt == 0 ? string.Empty : r.CurrencyCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 8, CellValues.Number, list[7], r.P2_Amt.ToString()));

                cellValueList.Add(new OpenXmlCell(startingRow, 9, CellValues.SharedString, list[8], r.P3_Amt == 0 ? string.Empty : r.CurrencyCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 10, CellValues.Number, list[9], r.P3_Amt.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 11, CellValues.SharedString, list[10], r.P4_Amt == 0 ? string.Empty : r.CurrencyCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 12, CellValues.Number, list[11], r.P4_Amt.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 13, CellValues.SharedString, list[12], r.P5_Amt == 0 ? string.Empty : r.CurrencyCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 14, CellValues.Number, list[13], r.P5_Amt.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 15, CellValues.SharedString, list[14], r.P6_Amt == 0 ? string.Empty : r.CurrencyCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 16, CellValues.Number, list[15], r.P6_Amt.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 17, CellValues.SharedString, list[16], r.P7_Amt == 0 ? string.Empty : r.CurrencyCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 18, CellValues.Number, list[17], r.P7_Amt.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 19, CellValues.SharedString, list[18], r.P8_Amt == 0 ? string.Empty : r.CurrencyCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 20, CellValues.Number, list[19], r.P8_Amt.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 21, CellValues.SharedString, list[20], r.P9_Amt == 0 ? string.Empty : r.CurrencyCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 22, CellValues.Number, list[21], r.P9_Amt.ToString()));
                OpenXmlUtil.createRowAndCells(document, worksheetID, startingRow, 1, cellValueList);
                OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(2, startingRow), OpenXmlUtil.getCellReference(4, startingRow));
                currentOfficeCode = r.OfficeCode;
                currentVendorName = r.SupplierName;
                startingRow += 1;
                cnt += 1;
            }

            OpenXmlUtil.setCellValue(document, worksheetID, 5, 5, p01, CellValues.SharedString, headerStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 5, 7, p02, CellValues.SharedString, headerStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 5, 9, p03, CellValues.SharedString, headerStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 5, 11, p04, CellValues.SharedString, headerStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 5, 13, p05, CellValues.SharedString, headerStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 5, 15, p06, CellValues.SharedString, headerStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 5, 17, p07, CellValues.SharedString, headerStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 5, 19, p08, CellValues.SharedString, headerStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 5, 21, p09, CellValues.SharedString, headerStyleId);

            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 8, worksheetID, startingRow + 2);
            OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(1, startingRow + 2), OpenXmlUtil.getCellReference(22, startingRow + 2));

            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }

        private string getCellRefList(int col, string commaDelimitedValues)
        {
            string colPrefix = OpenXmlUtil.getColumnLetter(col);
            string[] listInput = commaDelimitedValues.Split(',');
            string strOutput = string.Join(",", listInput.Select(x => colPrefix + x).ToArray());
            return strOutput;

        }


    }


}