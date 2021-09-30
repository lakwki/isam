using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.appserver.common;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.lcreport;
using com.next.isam.reporter.helper;
using com.next.infra.util;
using com.next.common.appserver;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;

namespace com.next.isam.webapp.reporter
{
    public partial class OrdersForLCCancellationReport : com.next.isam.webapp.usercontrol.PageTemplate
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
            ArrayList officeList = GeneralManager.Instance.getOfficeList();
            officeList.Sort(new ArrayListHelper.Sorter("OfficeId"));
            this.ddl_Office.bindList(officeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
        }

        private void genReport()
        {
            int officeId = Convert.ToInt32(ddl_Office.SelectedValue);

            DateTime customerAtWHDateFrom = DateTime.MinValue;
            DateTime customerAtWHDateTo = DateTime.MinValue;

            if (txt_CustomerAtWHDateFrom.Text.Trim() != "")
            {
                customerAtWHDateFrom = DateTimeUtility.getDate(txt_CustomerAtWHDateFrom.Text.Trim());
                customerAtWHDateTo = DateTimeUtility.getDate(txt_CustomerAtWHDateTo.Text.Trim());
            }

            this.outputExcel(LCReportManager.Instance.getOrdersForLCCancellationReport(officeId, customerAtWHDateFrom, customerAtWHDateTo));
        }

        private void outputExcel(OrdersForLCCancellationReportDs ds)
        {
            int officeId = -1;
            string officeStr = "ALL";
            if (ddl_Office.selectedValueToInt != -1)
            {
                officeId = int.Parse(ddl_Office.SelectedValue);
                officeStr = CommonUtil.getOfficeRefByKey(officeId).Description;
            }

            string sourceFileDir = this.ApplPhysicalPath + @"report_template\";
            string sourceFileName = "OrdersForLCCancellationReport.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(WebConfig.getValue("appSettings", "UPLOAD_AP_Folder") + @"OrdersForLCCancellationReport-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;
            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            int i = 4;
            worksheetID = OpenXmlUtil.getWorksheetId(document, "Main");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");

            if (worksheetID == string.Empty || templateWorksheetID == string.Empty)
                Console.WriteLine("failure open spreadsheet");

            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(this.LogonUserId).DisplayName, CellValues.String);
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.String);

            int cellStyleId = OpenXmlUtil.getCellStyleId(document, worksheetID, "B1");
            OpenXmlUtil.setCellValue(document, worksheetID, 1, 2, officeStr, CellValues.SharedString, cellStyleId);
            
            foreach (OrdersForLCCancellationReportDs.OrdersForLCCancellationRow r in ds.Tables[0].Rows)
            {
                OpenXmlUtil.setCellValue(document, worksheetID, i, 1, r.ShipmentId.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 2, r.ContractNo, CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 3, r.DeliveryNo.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 4, r.CustomerCode, CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 5, r.CustomerAtWarehouseDate.ToString("dd/MM/yyyy"), CellValues.Date);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 6, r.IsSupplierAtWarehouseDateNull() ? string.Empty : r.SupplierAtWarehouseDate.ToString("dd/MM/yyyy"), CellValues.Date);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 7, r.IsLCApprovalDateNull() ? string.Empty : r.LCApprovalDate.ToString("dd/MM/yyyy"), CellValues.Date);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 8, r.Vendor, CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 9, Regex.Replace(r.Dept, @"\(.*?\)", "").Trim(), CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 10, r.ProductTeam, CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 11, r.CurrencyCode, CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 12, r.IsTotalPOQtyNull() ? "0" : r.TotalPOQty.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 13, r.IsTotalPOAmtNull() ? "0" : r.TotalPOAmt.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 14, r.IsLCBatchNoNull() ? string.Empty : r.LCBatchNo.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 15, r.LCNo, CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 16, r.IsLCBillRefNoNull() ? string.Empty : r.LCBillRefNo, CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 17, r.LCIssueDate.ToString("dd/MM/yyyy"), CellValues.Date);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 18, r.IsLCExpiryDateNull() ? string.Empty : r.LCExpiryDate.ToString("dd/MM/yyyy"), CellValues.Date);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 19, r.LCAmt.ToString(), CellValues.Number);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 20, r.WorkflowStatus, CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 21, r.EbookingCancelled, CellValues.String);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 22, r.UKILSCancelled, CellValues.String);
                i++;
            }

            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();
            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }

        protected void btn_Export_Click(object sender, EventArgs arg)
        {
            if (!Page.IsValid)
                return;

            genReport();
        }
    }
}
