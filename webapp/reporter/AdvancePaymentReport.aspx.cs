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
using com.next.common.domain.types;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace com.next.isam.webapp.reporter
{
    public partial class AdvancePaymentReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) 
            {
                initControl();
            }
        }

        public class PaymentStatus
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        protected void initControl()
        {
            ddlOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--ALL--", GeneralCriteria.ALL.ToString());
            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(UclSmartSelection.SelectionList.garmentVendor);

            IList statuslist = new List<PaymentStatus>()
            {
             new PaymentStatus(){ ID=1, Name="ALL"},
             new PaymentStatus(){ ID=2, Name="FULLY SETTLED"},
             new PaymentStatus(){ ID=3, Name="OUTSTANDING"},
             new PaymentStatus(){ ID=4, Name="OUTSTANDING + NO RECOVERY PLAN"},
            };
            ddlStatus.DataSource = statuslist;
            ddlStatus.DataTextField = "Name";
            ddlStatus.DataValueField = "ID";
            ddlStatus.DataBind();
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
            int ddlVersionSelectedIndex = rbVersion.SelectedIndex;
            if (exportType == CrystalDecisions.Shared.ExportFormatType.Excel && int.Parse(rbVersion.SelectedValue) == 1)
                genReportOpenXml(ddlVersionSelectedIndex, strExportType);
            /*
            else
                ReportHelper.export(genReport(ddlVersionSelectedIndex, strExportType), HttpContext.Current.Response, exportType, "Advance Payment Report");
            */
        }

        private ReportClass genReport(int selectedIndex, string exportType)
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

        private void genReportOpenXml(int selectedIndex, string exportType)
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
                vendorName = IndustryUtil.getVendorByKey(txt_Supplier.VendorId).Name;
                vendorId = txt_Supplier.VendorId;
            }

            if (ddlOffice.SelectedIndex != 0)
            {
                officeId = int.Parse(ddlOffice.SelectedValue);
                officeName = CommonUtil.getOfficeRefByKey(officeId).Description;
            }

            paymentstatusIndex = int.Parse(ddlStatus.SelectedValue);
            paymentstatusString = ddlStatus.SelectedItem.Text;

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "AdvancePaymentMgtSummary.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(this.ApplPhysicalPath + @"reporter\tmpReport\AdvancePaymentMgtSummary-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;
            string summaryWorksheetID = string.Empty;

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Main");
            summaryWorksheetID = OpenXmlUtil.getWorksheetId(document, "Summary");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");

            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(this.LogonUserId).DisplayName, CellValues.String);
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.Date);

            int criteriaStyleId = OpenXmlUtil.getCellStyleId(document, worksheetID, "C1");

            OpenXmlUtil.setCellValue(document, worksheetID, 1, 3, officeName, CellValues.SharedString, criteriaStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 2, 3, vendorName, CellValues.SharedString, criteriaStyleId);
            OpenXmlUtil.setCellValue(document, worksheetID, 3, 3, paymentstatusString, CellValues.SharedString, criteriaStyleId);

            string dateRange = string.Empty;
            if (paymentDateFrom == DateTime.MinValue && paymentDateTo == DateTime.MinValue)
                dateRange = "ALL";
            else
                dateRange = DateTimeUtility.getDateString(paymentDateFrom) + " To " + DateTimeUtility.getDateString(paymentDateTo);
            OpenXmlUtil.setCellValue(document, worksheetID, 4, 3, dateRange, CellValues.SharedString, criteriaStyleId);

            if (deductionDateFrom == DateTime.MinValue && deductionDateTo == DateTime.MinValue)
                dateRange = "ALL";
            else
                dateRange = DateTimeUtility.getDateString(deductionDateFrom) + " To " + DateTimeUtility.getDateString(deductionDateTo);
            OpenXmlUtil.setCellValue(document, worksheetID, 5, 3, dateRange, CellValues.SharedString, criteriaStyleId);

            int[] list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 3, 24);
            int[] subtotalList = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 4, 24);
            int qtyTotalStyleId = OpenXmlUtil.getCellStyleId(document, templateWorksheetID, 4, 13);
            int amtTotalStyleId = OpenXmlUtil.getCellStyleId(document, templateWorksheetID, 4, 16);
            int summaryTotalStyleId = OpenXmlUtil.getCellStyleId(document, templateWorksheetID, 13, 4);
            int c19StyleId = OpenXmlUtil.getCellStyleId(document, templateWorksheetID, 4, 2);

            AdvancePaymentReportDs ds = AccountReportManager.Instance.getAdvancePaymentReportMGDataSet(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, vendorName, officeId, officeName, paymentstatusIndex, paymentstatusString);

            int startingRow = 8;
            int summaryStartingRow = 4;
            int detailCount = ds.AdvancePaymentReport.Rows.Count;
            int detailCounter = 1;
            int groupDetailCounter = 1;
            int groupDetailCounterAll = 1;
            DateTime maxAPDate = DateTime.MinValue;
            DateTime maxAWHDate = DateTime.MinValue;
            DateTime maxAWHDateWithThreshold = DateTime.MinValue;
            decimal totalActualDeduction = 0;
            Hashtable tblCurrency = new Hashtable();
            int lastMainRow = 0;

            decimal balanceSubtotal = 0;
            decimal actualDeductSubtotal = 0;
            decimal totalNoRecoveryPlan = 0;
            foreach (AdvancePaymentReportDs.AdvancePaymentReportRow r in ds.AdvancePaymentReport.Rows)
            {
                bool isNotFirstDetail = false;
                if (groupDetailCounter > 1)
                    isNotFirstDetail = true;

                if (groupDetailCounterAll > 1)
                {
                    if ((r.IsapdateNull() ? DateTime.MinValue : r.apdate) > maxAPDate)
                        maxAPDate = r.IsapdateNull() ? DateTime.MinValue : r.apdate;
                    if ((r.IsLinePaymentDateNull() ? DateTime.MinValue : r.LinePaymentDate) > maxAWHDate)
                    {
                        maxAWHDate = (r.IsLinePaymentDateNull() ? DateTime.MinValue : r.LinePaymentDate);
                        maxAWHDateWithThreshold = (r.paymenttypeid == 1 ? (r.IscustomeratwarehousedateNull() ? DateTime.MinValue : r.customeratwarehousedate.AddDays(r.officeId == OfficeId.BD.Id ? 30 : 14)) : r.LinePaymentDate);
                    }
                    totalActualDeduction += r.OriActualDeductAmt;
                }
                else
                {
                    maxAPDate = r.IsapdateNull() ? DateTime.MinValue : r.apdate;
                    maxAWHDate = (r.IsLinePaymentDateNull() ? DateTime.MinValue : r.LinePaymentDate);
                    maxAWHDateWithThreshold = (r.paymenttypeid == 1 ? (r.IscustomeratwarehousedateNull() ? DateTime.MinValue : r.customeratwarehousedate.AddDays(r.officeId == OfficeId.BD.Id ? 30 : 14)) : r.LinePaymentDate);

                    totalActualDeduction = r.OriActualDeductAmt;
                }

                balanceSubtotal += r.apremaining;
                actualDeductSubtotal += r.actual_deduct_amt;

                if (detailCounter + 1 <= detailCount &&
                        ((AdvancePaymentReportDs.AdvancePaymentReportRow)ds.AdvancePaymentReport.Rows[detailCounter]).paymentno == r.paymentno)
                    groupDetailCounterAll += 1;

                if (!r.IsapdateNull() || (r.IsapdateNull() && groupDetailCounter == 1 && detailCounter + 1 <= detailCount &&
                        (((AdvancePaymentReportDs.AdvancePaymentReportRow)ds.AdvancePaymentReport.Rows[detailCounter]).paymentno != r.paymentno))

                    || (r.IsapdateNull() && groupDetailCounter == 1 && detailCounter == detailCount))
                {
                    List<OpenXmlCell> cellValueList = new List<OpenXmlCell>();
                    cellValueList.Add(new OpenXmlCell(startingRow, 1, CellValues.SharedString, list[0], isNotFirstDetail ? string.Empty : r.officecode));
                    cellValueList.Add(new OpenXmlCell(startingRow, 2, CellValues.SharedString, isNotFirstDetail ? list[1] : (r.IsC19 == 1 ? c19StyleId : list[1]), isNotFirstDetail ? string.Empty : r.paymentno));
                    cellValueList.Add(new OpenXmlCell(startingRow, 3, CellValues.Date, list[2], isNotFirstDetail ? string.Empty : DateTimeUtility.getDateString(r.paymentdate)));
                    cellValueList.Add(new OpenXmlCell(startingRow, 4, CellValues.SharedString, list[3], isNotFirstDetail ? string.Empty : r.vendorname));
                    cellValueList.Add(new OpenXmlCell(startingRow, 5, CellValues.SharedString, list[4], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 6, CellValues.SharedString, list[5], isNotFirstDetail ? string.Empty : r.PayableTo));
                    cellValueList.Add(new OpenXmlCell(startingRow, 7, CellValues.SharedString, list[6], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 8, CellValues.SharedString, list[7], isNotFirstDetail ? string.Empty : r.currencycode));
                    cellValueList.Add(new OpenXmlCell(startingRow, 9, CellValues.Number, list[8], isNotFirstDetail ? string.Empty : (r.advance_amt - r.InterestAmt).ToString()));
                    cellValueList.Add(new OpenXmlCell(startingRow, 10, CellValues.Number, list[9], isNotFirstDetail ? string.Empty : r.InterestAmt.ToString()));
                    cellValueList.Add(new OpenXmlCell(startingRow, 11, CellValues.Number, list[10], isNotFirstDetail ? string.Empty : r.advance_amt.ToString()));
                    cellValueList.Add(new OpenXmlCell(startingRow, 12, CellValues.Number, list[11], isNotFirstDetail ? string.Empty : r.InterestPercentage.ToString()));
                    cellValueList.Add(new OpenXmlCell(startingRow, 13, CellValues.Date, list[12], r.IsapdateNull() ? string.Empty : DateTimeUtility.getDateString(r.apdate)));
                    cellValueList.Add(new OpenXmlCell(startingRow, 14, CellValues.SharedString, list[13], r.deduct_currency));
                    cellValueList.Add(new OpenXmlCell(startingRow, 15, CellValues.Number, list[14], r.actual_deduct_amt.ToString()));
                    cellValueList.Add(new OpenXmlCell(startingRow, 16, CellValues.Number, list[15], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 17, CellValues.Number, list[16], string.Empty));
                    //cellValueList.Add(new OpenXmlCell(startingRow, 18, CellValues.SharedString, list[17], r.IsRemarkNull() ? string.Empty : (isNotFirstDetail ? string.Empty : r.Remark)));
                    cellValueList.Add(new OpenXmlCell(startingRow, 18, CellValues.SharedString, list[17], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 19, CellValues.SharedString, list[18], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 20, CellValues.SharedString, list[19], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 21, CellValues.SharedString, list[20], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 22, CellValues.SharedString, list[21], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 23, CellValues.SharedString, list[22], string.Empty));
                    cellValueList.Add(new OpenXmlCell(startingRow, 24, CellValues.SharedString, list[23], isNotFirstDetail ? string.Empty : r.FLRefNo));

                    if (!isNotFirstDetail) lastMainRow = startingRow;

                    OpenXmlUtil.createRowAndCells(document, worksheetID, startingRow, 1, cellValueList);
                    OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(4, startingRow), OpenXmlUtil.getCellReference(5, startingRow));
                    OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(6, startingRow), OpenXmlUtil.getCellReference(7, startingRow));

                    if ((detailCounter + 1 <= detailCount &&
                        (((AdvancePaymentReportDs.AdvancePaymentReportRow)ds.AdvancePaymentReport.Rows[detailCounter]).paymentno != r.paymentno))
                        || detailCounter == detailCount)
                    {
                        startingRow += 1;

                        AdvancePaymentSummaryDef def = AccountManager.Instance.getAdvancePaymentSummaryDef(r.PaymentId);
                        decimal noRecoveryPlan = def.NoRecoveryPlanBalance < 0 ? 0 : def.NoRecoveryPlanBalance;
                        totalNoRecoveryPlan += noRecoveryPlan;
                        OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 14, "Sub-total:", CellValues.SharedString, subtotalList[13]);
                        OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 15, actualDeductSubtotal.ToString(), CellValues.Number, subtotalList[14]);
                        OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 16, balanceSubtotal.ToString(), CellValues.Number, subtotalList[15]);
                        OpenXmlUtil.addDataBar(document, worksheetID, OpenXmlUtil.DataBarColor.Orange, startingRow, 16, lastMainRow, 11);
                        OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 17, noRecoveryPlan.ToString(), CellValues.Number, subtotalList[16]);
                        OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 19, r.IsRemarkNull() ? string.Empty : r.Remark, CellValues.SharedString, list[18]);
                        
                        if (r.paymenttypeid == 1)
                        {
                            string firstContractDate = AccountReportManager.Instance.getAdvancePaymentFirstContractDate(r.PaymentId);
                            string lastContractDate = AccountReportManager.Instance.getAdvancePaymentLastContractDate(r.PaymentId);

                            if (firstContractDate == lastContractDate)
                                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 18, firstContractDate, CellValues.SharedString, subtotalList[16]);
                            else
                                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 18, firstContractDate + "\n" + lastContractDate, CellValues.SharedString, subtotalList[17]);
                        }
                        else if (r.paymenttypeid == 2)
                        {
                            StringBuilder strBuilder = new StringBuilder();
                            List<AdvancePaymentInstalmentDetailDef> instalmentDetailList = AccountManager.Instance.getAdvancePaymentInstalmentDetailList(r.PaymentId);
                            foreach (AdvancePaymentInstalmentDetailDef instalmentDetailDef in instalmentDetailList)
                            {
                                if (!instalmentDetailDef.IsInterestCharge)
                                {
                                    strBuilder.AppendLine("INSTALMENT - " + DateTimeUtility.getDateString(instalmentDetailDef.PaymentDate) + " " + r.currencycode + " " + instalmentDetailDef.ExpectedAmount.ToString());
                                }
                            }
                            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 18, strBuilder.ToString(), CellValues.SharedString, subtotalList[17]);
                        }

                        OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 20, maxAPDate == DateTime.MinValue ? "N/A" : maxAPDate.Subtract(r.paymentdate).Days.ToString(), CellValues.Number, subtotalList[19]);
                        OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 21, maxAWHDateWithThreshold == DateTime.MinValue ? "N/A" : maxAWHDateWithThreshold.Subtract(r.paymentdate).Days.ToString(), CellValues.Number, subtotalList[20]);
                        OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 22, r.IsDisplayNameNull() ? string.Empty : r.DisplayName, CellValues.SharedString, list[21]);

                        if (!tblCurrency.ContainsKey(r.currencycode))
                        {
                            AdvancePaymentReport.AdvancePaymentGrandTotalSummary summaryDef = new AdvancePaymentReport.AdvancePaymentGrandTotalSummary();
                            summaryDef.PaymentAmount = (r.advance_amt - r.InterestAmt);
                            summaryDef.TotalAmount = r.advance_amt;
                            summaryDef.InterestAmount = r.InterestAmt;
                            summaryDef.ActualDeductionAmount = actualDeductSubtotal;
                            summaryDef.BalanceAmount = balanceSubtotal;
                            tblCurrency.Add(r.currencycode, summaryDef);
                        }
                        else
                        {
                            AdvancePaymentReport.AdvancePaymentGrandTotalSummary summaryDef = (AdvancePaymentReport.AdvancePaymentGrandTotalSummary)tblCurrency[r.currencycode];
                            summaryDef.PaymentAmount += (r.advance_amt - r.InterestAmt);
                            summaryDef.TotalAmount += r.advance_amt;
                            summaryDef.InterestAmount += r.InterestAmt;
                            summaryDef.ActualDeductionAmount += actualDeductSubtotal;
                            summaryDef.BalanceAmount += balanceSubtotal;
                            tblCurrency[r.currencycode] = summaryDef;
                        }

                        actualDeductSubtotal = 0;
                        balanceSubtotal = 0;

                        if ((r.advance_amt - totalActualDeduction) > (decimal)0.01)
                            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 23, "Last recovery date is pending to provide and we are following up with local team.", CellValues.Date, subtotalList[22]);
                        else
                            OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 23, DateTimeUtility.getDateString(maxAWHDateWithThreshold), CellValues.Date, subtotalList[22]);

                        startingRow += 1;
                        OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 5, worksheetID, startingRow);

                        if (detailCounter + 1 <= detailCount)
                        {
                            groupDetailCounter = 0;
                            groupDetailCounterAll = 1;
                        }
                    }

                    groupDetailCounter += 1;
                    startingRow += 1;
                    System.Diagnostics.Debug.Print("starting row : " + startingRow.ToString());
                }

                detailCounter += 1;
            }

            startingRow += 2;

            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 6, worksheetID, startingRow);
            list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 6, 17);

            foreach (string k in tblCurrency.Keys)
            {
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 8, k, CellValues.SharedString, list[7]);
                AdvancePaymentReport.AdvancePaymentGrandTotalSummary summaryDef = (AdvancePaymentReport.AdvancePaymentGrandTotalSummary)tblCurrency[k];

                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 9, summaryDef.PaymentAmount.ToString(), CellValues.Number, list[8]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 10, summaryDef.InterestAmount.ToString(), CellValues.Number, list[9]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 11, summaryDef.TotalAmount.ToString(), CellValues.Number, list[10]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 15, summaryDef.ActualDeductionAmount.ToString(), CellValues.Number, list[14]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 16, summaryDef.BalanceAmount.ToString(), CellValues.Number, list[15]);
                OpenXmlUtil.setCellValue(document, worksheetID, startingRow, 17, totalNoRecoveryPlan.ToString(), CellValues.Number, list[16]);
                OpenXmlUtil.addDataBar(document, worksheetID, OpenXmlUtil.DataBarColor.Orange, startingRow, 16, startingRow, 11);
                startingRow += 1;
            }

            AdvancePaymentSummaryReportDs summaryds = AccountReportManager.Instance.getAdvancePaymentSummaryReportMG(paymentDateFrom, paymentDateTo, deductionDateFrom, deductionDateTo, vendorId, officeId, paymentstatusIndex);
            tblCurrency = new Hashtable();

            if (summaryds.AdvancePaymentSummaryTable.Rows.Count > 0)
            {
                list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 11, 13);
                if(paymentstatusString == "OUTSTANDING + NO RECOVERY PLAN")
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, 1, 1, "Summary Of Advance Payment to Suppliers By Office - Outstanding + No Recovery Plan", CellValues.SharedString);
                else if (paymentstatusString == "OUTSTANDING")
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, 1, 1, "Summary Of Advance Payment to Suppliers By Office - Outstanding", CellValues.SharedString);
                else if (paymentstatusString == "FULLY SETTLED")
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, 1, 1, "Summary Of Advance Payment to Suppliers By Office - Fully Settled", CellValues.SharedString);
                else
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, 1, 1, "Summary Of Advance Payment to Suppliers By Office - All", CellValues.SharedString);

                foreach(AdvancePaymentSummaryReportDs.AdvancePaymentSummaryTableRow r in summaryds.AdvancePaymentSummaryTable.Rows)
                {
                    List<OpenXmlCell> cellValueList = new List<OpenXmlCell>();
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 1, CellValues.SharedString, list[0], r.officecode));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 2, CellValues.SharedString, list[1], r.vendorname));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 3, CellValues.SharedString, list[2], string.Empty));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 4, CellValues.SharedString, list[3], string.Empty));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 5, CellValues.SharedString, list[4], r.currencycode));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 6, CellValues.Number, list[5], (r.totaladvanceamt - r.totalinterestamt).ToString()));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 7, CellValues.Number, list[6], r.totalinterestamt.ToString()));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 8, CellValues.Number, list[7], r.totaladvanceamt.ToString()));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 9, CellValues.Number, list[8], r.totaldeduceamt.ToString()));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 10, CellValues.Number, list[9], r.totaloutstandingamt.ToString()));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 11, CellValues.Number, list[10], r.futureordercnt.ToString()));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 12, CellValues.Number, list[11], r.totalsales.ToString()));
                    cellValueList.Add(new OpenXmlCell(summaryStartingRow, 13, CellValues.Date, list[12], r.Islast_cus_awh_dateNull() ? "N/A" : r.last_cus_awh_date));
                    OpenXmlUtil.createRowAndCells(document, summaryWorksheetID, summaryStartingRow, 1, cellValueList);
                    OpenXmlUtil.mergeCells(document, summaryWorksheetID, OpenXmlUtil.getCellReference(2, summaryStartingRow), OpenXmlUtil.getCellReference(4, summaryStartingRow));

                    if (!tblCurrency.ContainsKey(r.currencycode))
                        tblCurrency.Add(r.currencycode, summaryStartingRow.ToString());
                    else
                    {
                        string s = (string)tblCurrency[r.currencycode];
                        tblCurrency[r.currencycode] = s + "," + summaryStartingRow.ToString();
                    }

                    summaryStartingRow += 1;
                }

                summaryStartingRow += 1;
                list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 13, 12);
                OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 4, "Summary Total:", CellValues.SharedString, summaryTotalStyleId);

                foreach (string k in tblCurrency.Keys)
                {
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 5, k, CellValues.SharedString, list[4]);
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 6, "=SUM(" + this.getCellRefList(6, tblCurrency[k].ToString()) + ")", CellValues.Number, list[5]);
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 7, "=SUM(" + this.getCellRefList(7, tblCurrency[k].ToString()) + ")", CellValues.Number, list[6]);
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 8, "=SUM(" + this.getCellRefList(8, tblCurrency[k].ToString()) + ")", CellValues.Number, list[7]);
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 9, "=SUM(" + this.getCellRefList(9, tblCurrency[k].ToString()) + ")", CellValues.Number, list[8]);
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 10, "=SUM(" + this.getCellRefList(10, tblCurrency[k].ToString()) + ")", CellValues.Number, list[9]);
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 11, "=SUM(" + this.getCellRefList(11, tblCurrency[k].ToString()) + ")", CellValues.Number, list[10]);
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 12, "=SUM(" + this.getCellRefList(12, tblCurrency[k].ToString()) + ")", CellValues.Number, list[11]);
                    summaryStartingRow += 1;
                }


                summaryStartingRow += 2;

                summaryds = AccountReportManager.Instance.getAdvancePaymentSummaryReportMGThisYear(officeId);
                int summarySubHeadingStyleId = OpenXmlUtil.getCellStyleId(document, templateWorksheetID, 16, 5);

                if (summaryds.AdvancePaymentSummaryTable.Rows.Count > 0)
                {
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 5, ((AdvancePaymentSummaryReportDs.AdvancePaymentSummaryTableRow)summaryds.AdvancePaymentSummaryTable.Rows[0]).BudgetYear.ToString() + " Summary of All Advance Payment", CellValues.SharedString, summarySubHeadingStyleId);
                    summaryStartingRow += 1;
                    OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 17, summaryWorksheetID, summaryStartingRow);
                    list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 18, 10);
                    summaryStartingRow += 1;

                    foreach (AdvancePaymentSummaryReportDs.AdvancePaymentSummaryTableRow r in summaryds.AdvancePaymentSummaryTable.Rows)
                    {
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 5, r.currencycode, CellValues.SharedString, list[4]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 6, r.PaymentAmt.ToString(), CellValues.Number, list[5]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 7, r.InterestAmt.ToString(), CellValues.Number, list[6]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 8, r.TotalAmt.ToString(), CellValues.Number, list[7]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 9, r.ActualDeductionAmt.ToString(), CellValues.Number, list[8]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 10, r.BalanceAmt.ToString(), CellValues.Number, list[9]);
                        summaryStartingRow += 1;
                    }
                }

                summaryStartingRow += 2;

                summaryds = AccountReportManager.Instance.getAdvancePaymentSummaryReportMGPreviousYear(officeId);

                if (summaryds.AdvancePaymentSummaryTable.Rows.Count > 0)
                {
                    OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 5, ((AdvancePaymentSummaryReportDs.AdvancePaymentSummaryTableRow)summaryds.AdvancePaymentSummaryTable.Rows[0]).BudgetYear.ToString() + " Summary of All Advance Payment", CellValues.SharedString, summarySubHeadingStyleId);
                    summaryStartingRow += 1;
                    OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 17, summaryWorksheetID, summaryStartingRow);
                    summaryStartingRow += 1;

                    foreach (AdvancePaymentSummaryReportDs.AdvancePaymentSummaryTableRow r in summaryds.AdvancePaymentSummaryTable.Rows)
                    {
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 5, r.currencycode, CellValues.SharedString, list[4]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 6, r.PaymentAmt.ToString(), CellValues.Number, list[5]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 7, r.InterestAmt.ToString(), CellValues.Number, list[6]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 8, r.TotalAmt.ToString(), CellValues.Number, list[7]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 9, r.ActualDeductionAmt.ToString(), CellValues.Number, list[8]);
                        OpenXmlUtil.setCellValue(document, summaryWorksheetID, summaryStartingRow, 10, r.BalanceAmt.ToString(), CellValues.Number, list[9]);
                        summaryStartingRow += 1;
                    }
                }
            }

            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 25, summaryWorksheetID, summaryStartingRow + 2);
            OpenXmlUtil.mergeCells(document, summaryWorksheetID, OpenXmlUtil.getCellReference(1, summaryStartingRow + 2), OpenXmlUtil.getCellReference(13, summaryStartingRow + 2));

            OpenXmlUtil.copyAndInsertRow(document, templateWorksheetID, 25, worksheetID, startingRow + 2);
            OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(1, startingRow + 2), OpenXmlUtil.getCellReference(21, startingRow + 2));

            /*
            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();
            */

            OpenXmlUtil.saveComplete(document);

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }

        private string getCellRefList(int col, string commaDelimitedValues)
        {
            string colPrefix = OpenXmlUtil.getColumnLetter(col);
            string[] listInput = commaDelimitedValues.Split(',');
            string strOutput = string.Join(",", listInput.Select(x => colPrefix + x).ToArray());
            return strOutput;

        }

        public class AdvancePaymentGrandTotalSummary
        {
            public AdvancePaymentGrandTotalSummary() { }
            public decimal PaymentAmount { get; set; }
            public decimal InterestAmount { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal ActualDeductionAmount { get; set; }
            public decimal BalanceAmount { get; set; }

        }

    }


}