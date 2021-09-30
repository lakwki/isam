using System;
using System.Collections.Generic;
using com.next.common.web.commander;
using com.next.infra.web;
using com.next.isam.webapp.commander.account;
using com.next.isam.reporter.accounts;
using System.IO;
using com.next.infra.util;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using com.next.isam.dataserver.model.account;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.webapp.reporter
{
    public partial class LGHoldPaymentReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public class PaymentStatus
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                this.ddlOffice.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", GeneralCriteria.ALL.ToString(), "--All--", GeneralCriteria.ALL.ToString());
                txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

                IList statuslist = new List<PaymentStatus>()
                {
                 new PaymentStatus(){ ID=1, Name="ALL"},
                 new PaymentStatus(){ ID=2, Name="FULLY SETTLED"},
                 new PaymentStatus(){ ID=3, Name="OUTSTANDING"},
                };
                ddlStatus.DataSource = statuslist;
                ddlStatus.DataTextField = "Name";
                ddlStatus.DataValueField = "ID";
                ddlStatus.DataBind();
            }
        }

        private LGHoldPaymentDs getLGList()
        {
            int officeId = int.Parse(ddlOffice.SelectedValue);
            string LGNo = String.IsNullOrEmpty(txt_lgNo.Text) ? null : txt_lgNo.Text;
            int vendorId = txt_Supplier.VendorId > 0 ? txt_Supplier.VendorId : -1;
            string itemNo = String.IsNullOrEmpty(txt_itemNo.Text) ? null : txt_itemNo.Text;
            string contractNo = String.IsNullOrEmpty(txt_contractNo.Text) ? null : txt_contractNo.Text;
            int paymentStatus = int.Parse(ddlStatus.SelectedValue);

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetLGHoldPaymentList);
            Context.Items.Add(AccountCommander.Param.officeId, officeId);
            Context.Items.Add(AccountCommander.Param.lgNo, LGNo);
            Context.Items.Add(AccountCommander.Param.vendorId, vendorId);
            Context.Items.Add(AccountCommander.Param.itemNo, itemNo);
            Context.Items.Add(AccountCommander.Param.contractNo, contractNo);
            Context.Items.Add(AccountCommander.Param.advancePaymentSettlementStatus, paymentStatus);
            forwardToScreen(null);

            return (LGHoldPaymentDs)Context.Items[AccountCommander.Param.lgHoldPaymentList];
        }

        protected void btn_Print_Click(object sender, EventArgs e)
        {
            LGHoldPaymentDs lgList = getLGList();

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "LGHoldPaymentReport.xlsm";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(this.ApplPhysicalPath + @"reporter\tmpReport\LGHoldPaymentReport-{0}-{1}.xlsm", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet1");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");


            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 1, CommonUtil.getUserByKey(this.LogonUserId).DisplayName, CellValues.String);
            OpenXmlUtil.setCellValue(document, templateWorksheetID, 1, 2, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), CellValues.Date);
            int noOfDetailCols = 16;

            int[] list = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 3, noOfDetailCols);

            int startingRow = 2;
            foreach (LGHoldPaymentDs.LGHoldPaymentRow r in lgList.LGHoldPayment.Rows)
            {
                string address = string.Empty;
                List<OpenXmlCell> cellValueList = new List<OpenXmlCell>();
                cellValueList.Add(new OpenXmlCell(startingRow, 1, CellValues.SharedString, list[0], r.OfficeCode));
                cellValueList.Add(new OpenXmlCell(startingRow, 2, CellValues.SharedString, list[1], r.LGNo));
                cellValueList.Add(new OpenXmlCell(startingRow, 3, CellValues.SharedString, list[2], r.Name));
                cellValueList.Add(new OpenXmlCell(startingRow, 4, CellValues.SharedString, list[3], r.DisplayName));
                cellValueList.Add(new OpenXmlCell(startingRow, 5, CellValues.SharedString, list[4], DateTimeUtility.getDateString(r.SubmittedDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 6, CellValues.SharedString, list[5], r.ItemNo));
                cellValueList.Add(new OpenXmlCell(startingRow, 7, CellValues.SharedString, list[6], r.ContractNo));
                cellValueList.Add(new OpenXmlCell(startingRow, 8, CellValues.SharedString, list[7], r.Description));
                cellValueList.Add(new OpenXmlCell(startingRow, 9, CellValues.SharedString, list[8], r.InvoiceNo));
                cellValueList.Add(new OpenXmlCell(startingRow, 10, CellValues.SharedString, list[9], DateTimeUtility.getDateString(r.DeliveryDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 11, CellValues.SharedString, list[10], r.PaymentTermDesc));
                cellValueList.Add(new OpenXmlCell(startingRow, 12, CellValues.SharedString, list[11], r.Currency));
                cellValueList.Add(new OpenXmlCell(startingRow, 13, CellValues.SharedString, list[12], r.AmountOnHold.ToString()));
                cellValueList.Add(new OpenXmlCell(startingRow, 14, CellValues.SharedString, list[13], r.IsDueDateNull() ? string.Empty : DateTimeUtility.getDateString(r.DueDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 15, CellValues.SharedString, list[14], r.IsReleaseDateNull() ? string.Empty : DateTimeUtility.getDateString(r.ReleaseDate)));
                cellValueList.Add(new OpenXmlCell(startingRow, 16, CellValues.SharedString, list[15], r.IsRemarkNull() ? string.Empty : r.Remark));
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