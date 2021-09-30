using System;
using System.Web;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.isam.webapp.commander.account;
using com.next.isam.webapp.commander;
using com.next.isam.domain.account;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.appserver.order;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.account;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.infra.web;
using com.next.isam.appserver.common;
using com.next.common.domain.dms;
using com.next.isam.reporter.accounts;
using com.next.infra.util;
using com.next.common.domain.module;
using com.next.isam.dataserver.model.account;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using com.next.isam.reporter.helper;

namespace com.next.isam.webapp.account
{
    public partial class InvoiceStatusEnquiry : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public int iTotalMax = 0;

        private ArrayList vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (ArrayList)ViewState["SearchResult"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);
                ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId", "", "--All--", GeneralCriteria.ALL.ToString());
                this.ddl_HandlingOffice.bindList(CommonManager.Instance.getDGHandlingOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());
                ddl_TradingAgency.bindList(WebUtil.getTradingAgencyList(), "ShortName", "TradingAgencyId", "", "--All--", GeneralCriteria.ALL.ToString());
                txt_SupplierName.setWidth(300);
                txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
                refreshSupplier();
                ddl_PaymentTerm.bindList(CommonManager.Instance.getPaymentTermList(), "PaymentTermDescription", "PaymentTermId", "", "--All--", GeneralCriteria.ALL.ToString());
                this.btn_Search.Attributes.Add("onclick", "return isValidSearch();");
                this.chkAccepted.Checked = true;
                this.btn_Accept.Visible = false;
            }
        }

        protected void refreshSupplier()
        {
            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountReceivableAndPayable.Id, ISAMModule.accountReceivableAndPayable.PaymentStatusEnquiryNML))
            {
                txt_SupplierName.VendorId = 3154; //    NEXT MANUFACTURING (PVT.) LTD
                txt_SupplierName.Enabled = false;
            }
            else
                txt_SupplierName.Enabled = true;
        }

        protected void InvDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PaymentStatusEnquiryDef def = (PaymentStatusEnquiryDef)vwSearchResult[e.Row.RowIndex];
                ((LinkButton)e.Row.FindControl("lnk_SupplierName")).Text = def.SupplierName;
                ((LinkButton)e.Row.FindControl("lnk_SupplierName")).Attributes.Add("onclick", "openHoldPaymentBySupplierWindow('" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.VendorId.ToString())) + "');return false;");
                ((Label)e.Row.FindControl("lbl_ProdTeamCode")).Text = def.ProductTeamCode;
                ((Label)e.Row.FindControl("lbl_ProdTeamDesc")).Text = def.ProductTeamDescription;
                ((Label)e.Row.FindControl("lbl_ContractNo")).Text = def.ContractNo;
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = def.ItemNo;
                ((Label)e.Row.FindControl("lbl_invNo")).Text = def.InvoiceNo;
                ((Label)e.Row.FindControl("lbl_invDate")).Text = DateTimeUtility.getDateString(def.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_SupplierInvoiceNo")).Text = def.SupplierInvoiceNo;
                ((Label)e.Row.FindControl("lbl_Currency")).Text = def.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_NetAmt")).Text = def.NetAmount.ToString("#,###.00");
                ((Label)e.Row.FindControl("lbl_NetAmtInUSD")).Text = def.NetAmountInUSD.ToString("#,###.00");
                ((Label)e.Row.FindControl("lbl_PaymentStatus")).Text = def.PaymentStatus;
                ((Label)e.Row.FindControl("lbl_LGDueDate")).Text = def.LGDate != DateTime.MinValue ? DateTimeUtility.getDateString(LetterOfGuaranteeDef.getLGDueDate(def.InvoiceDate)) : string.Empty;
                //if (def.ShippingDocReceiptDate != DateTime.MinValue)
                if (def.IsUploadDMSDocument)
                {
                    ((ImageButton)e.Row.FindControl("imgOpen")).CommandArgument = def.ContractNo;
                    ((ImageButton)e.Row.FindControl("imgOpen")).Attributes.Add("onclick", "openAttachments(this, '" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.ShipmentId.ToString())) + "');return false;");
                }
                else
                {
                    ((ImageButton)e.Row.FindControl("imgOpen")).Visible = false;
                }

                if (def.EditLock == false)
                {
                    ((Label)e.Row.FindControl("lbl_LockReleased")).Text = "YES";
                    e.Row.BackColor = System.Drawing.Color.FromArgb(255, 192, 192);
                }
                else
                {
                    ((Label)e.Row.FindControl("lbl_LockReleased")).Text = "NO";

                    if (AccountManager.Instance.getLGDetail(def.ShipmentId, 0) > 0)
                        e.Row.BackColor = System.Drawing.Color.FromArgb(128, 255, 0);
                }

                Label lbl = ((Label)e.Row.FindControl("lbl_IsPaymentHold"));
                if (def.IsPaymentHold)
                {
                    lbl.Text = "YES";
                    lbl.ForeColor = System.Drawing.Color.Red;
                    lbl.Font.Bold = true;

                    ((Label)e.Row.FindControl("lbl_PaymentHoldRemark")).Text = AccountManager.Instance.getSupplierPaymentHoldRemark(def.VendorId);
                }
                else
                {
                    lbl.Text = "NO";
                    lbl.ForeColor = System.Drawing.Color.Black;
                    lbl.Font.Bold = false;
                }


                if (def.DMSWorkflowStatusId == ShippingDocWFS.NOT_READY.Id)
                    ((CheckBox)e.Row.FindControl("ckb_inv")).Enabled = false;

                if (def.IsChinaGBTestRequired)
                {
                    ((Image)e.Row.FindControl("img_GBTestRequired")).Visible = true;

                    int vendorId = def.VendorId;
                    if (def.SplitShipmentId > 0)
                    {
                        ShipmentDef shipmentDef = OrderManager.Instance.getShipmentByKey(def.ShipmentId);
                        vendorId = shipmentDef.Vendor.VendorId;
                    }

                    int testResult = com.next.common.appserver.GeneralManager.Instance.getChinaGBTestResult(def.ProductId, vendorId);

                    if (testResult == 1)
                    {
                        ((Image)e.Row.FindControl("img_GBTestPassed")).Visible = true;
                    }
                    else if (testResult == 0)
                    {
                        ((Image)e.Row.FindControl("img_GBTestFailedHold")).Visible = true;
                    }
                    else if (testResult == 2)
                    {
                        ((Image)e.Row.FindControl("img_GBTestFailedRelease")).Visible = true;
                    }
                    else if (testResult == 9)
                    {
                        ((Image)e.Row.FindControl("img_GBTestFailedCannotRelease")).Visible = true;
                    }
                }

                ((Label)e.Row.FindControl("lbl_DeductionAmt")).Text = def.DeductionAmount.ToString("#,##0.00");
                if (def.DeductionAmount != 0)
                    //e.Row.Cells[17].BackColor = System.Drawing.Color.FromArgb(255, 255, 0);
                    ((TableCell)e.Row.FindControl("lbl_DeductionAmt").Parent).BackColor = System.Drawing.Color.FromArgb(255, 255, 0);
            }
        }

        #region Button Event

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetPaymentStatusEnquiryList);

            if (txt_InvoiceDateFrom.Text.Trim() != String.Empty)
            {
                Context.Items.Add(AccountCommander.Param.invoiceDateFrom, Convert.ToDateTime(txt_InvoiceDateFrom.Text.Trim()));
                if (txt_InvoiceDateTo.Text.Trim() == String.Empty)
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                Context.Items.Add(AccountCommander.Param.invoiceDateTo, Convert.ToDateTime(txt_InvoiceDateTo.Text.Trim()));
            }
            else
            {
                Context.Items.Add(AccountCommander.Param.invoiceDateFrom, DateTime.MinValue);
                Context.Items.Add(AccountCommander.Param.invoiceDateTo, DateTime.MinValue);
            }

            if (txt_SubDocDateFrom.Text.Trim() != String.Empty)
            {
                Context.Items.Add(AccountCommander.Param.subDocDateFrom, Convert.ToDateTime(txt_SubDocDateFrom.Text.Trim()));
                if (txt_SubDocDateTo.Text.Trim() == String.Empty)
                    txt_SubDocDateTo.Text = txt_SubDocDateFrom.Text;
                Context.Items.Add(AccountCommander.Param.subDocDateTo, Convert.ToDateTime(txt_SubDocDateTo.Text.Trim()));
            }
            else
            {
                Context.Items.Add(AccountCommander.Param.subDocDateFrom, DateTime.MinValue);
                Context.Items.Add(AccountCommander.Param.subDocDateTo, DateTime.MinValue);
            }

            if (txt_InterfaceDateFrom.Text.Trim() != String.Empty)
            {
                Context.Items.Add(AccountCommander.Param.interfaceDateFrom, Convert.ToDateTime(txt_InterfaceDateFrom.Text.Trim()));
                if (txt_InterfaceDateTo.Text.Trim() == String.Empty)
                    txt_InterfaceDateTo.Text = txt_InterfaceDateFrom.Text;
                Context.Items.Add(AccountCommander.Param.interfaceDateTo, Convert.ToDateTime(txt_InterfaceDateTo.Text.Trim()));
            }
            else
            {
                Context.Items.Add(AccountCommander.Param.interfaceDateFrom, DateTime.MinValue);
                Context.Items.Add(AccountCommander.Param.interfaceDateTo, DateTime.MinValue);
            }

            if (txt_InvoiceNoFrom.Text.Trim() != String.Empty)
            {
                Context.Items.Add(AccountCommander.Param.invoicePrefix, WebUtil.getInvoicePrefix(txt_InvoiceNoFrom.Text.Trim()));
                Context.Items.Add(AccountCommander.Param.invoiceSeqFrom, WebUtil.getInvoiceSeq(txt_InvoiceNoFrom.Text.Trim()));
                Context.Items.Add(AccountCommander.Param.invoiceSeqTo, WebUtil.getInvoiceSeq(txt_InvoiceNoTo.Text.Trim()));
                Context.Items.Add(AccountCommander.Param.invoiceYear, WebUtil.getInvoiceYear(txt_InvoiceNoFrom.Text.Trim()));
            }

            Context.Items.Add(AccountCommander.Param.officeId, ddl_Office.SelectedValue);
            Context.Items.Add(AccountCommander.Param.handlingOfficeId, ddl_HandlingOffice.SelectedValue);
            Context.Items.Add(AccountCommander.Param.tradingAgencyId, ddl_TradingAgency.SelectedValue);
            Context.Items.Add(AccountCommander.Param.paymentTermId, ddl_PaymentTerm.SelectedValue);
            Context.Items.Add(AccountCommander.Param.vendorId, this.txt_SupplierName.VendorId == int.MinValue ? -1 : this.txt_SupplierName.VendorId);
            Context.Items.Add(AccountCommander.Param.contractNo, this.txt_ContractNo.Text.Trim());

            TypeCollector paymentStatusList = TypeCollector.Inclusive;

            if (this.chkNotReady.Checked) paymentStatusList.append(ShippingDocWFS.NOT_READY.Id);
            if (this.chkReady.Checked) paymentStatusList.append(ShippingDocWFS.READY.Id);
            if (this.chkRejected.Checked) paymentStatusList.append(ShippingDocWFS.REJECTED.Id);
            if (this.chkAccepted.Checked) paymentStatusList.append(ShippingDocWFS.ACCEPTED.Id);
            if (this.chkReviewed.Checked) paymentStatusList.append(ShippingDocWFS.REVIEWED.Id);

            Context.Items.Add(AccountCommander.Param.paymentStatusList, paymentStatusList);

            forwardToScreen(null);

            ArrayList list = (ArrayList)Context.Items[AccountCommander.Param.paymentStatusEnquiryList];

            this.vwSearchResult = list;

            gv_Inv.DataSource = list;
            gv_Inv.DataBind();

            if (list.Count >= 500)
            {
                lbl_Warning.Visible = true;
            }

            pnl_Result.Visible = true;

        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            ddl_Office.SelectedIndex = -1;
            ddl_TradingAgency.SelectedIndex = -1;
            ddl_PaymentTerm.SelectedIndex = -1;
            this.txt_SupplierName.clear();
            refreshSupplier();
            this.txt_InvoiceDateFrom.Text = String.Empty;
            this.txt_InvoiceDateTo.Text = String.Empty;
            this.txt_SubDocDateFrom.Text = String.Empty;
            this.txt_SubDocDateTo.Text = String.Empty;
            this.txt_InterfaceDateFrom.Text = String.Empty;
            this.txt_InterfaceDateTo.Text = String.Empty;
            this.txt_InvoiceNoFrom.Text = String.Empty;
            this.txt_InvoiceNoTo.Text = String.Empty;
            this.txt_ContractNo.Text = String.Empty;
            this.chkAccepted.Checked = true;
            this.chkNotReady.Checked = false;
            this.chkReady.Checked = false;
            this.chkRejected.Checked = false;
            this.chkReviewed.Checked = false;

            vwSearchResult = null;
            lbl_Warning.Visible = false;
            pnl_Result.Visible = false;
        }

        #endregion

        protected void gv_Inv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //PaymentStatusEnquiryDef def = (PaymentStatusEnquiryDef)this.vwSearchResult[int.Parse((string)e.CommandArgument)];

            ArrayList queryStructs = new ArrayList();
            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Shipping - UK Contract"));
            queryStructs.Add(new QueryStructDef("UK Contract - Delivery", (string)e.CommandArgument));

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            long attachmentId = 0;

            foreach (DocumentInfoDef docInfoDef in qList)
            {
                foreach (AttachmentInfoDef aDef in docInfoDef.AttachmentInfos)
                {
                    attachmentId = aDef.AttachmentID;
                }
            }

            if (attachmentId > 0)
            {
                System.Byte[] byteDownload = DMSUtil.getAttachment(attachmentId);

                Response.ContentType = "application/pdf";

                string filename = "download";

                System.String disHeader = "Attachment; Filename=\"" + filename + "\"";
                Response.AppendHeader("Content-Disposition", disHeader);

                Response.BinaryWrite(byteDownload);

                Response.Flush();
            }
        }

        protected void btn_ExcelToEpicor_Click(object sender, EventArgs e)
        {
            exportToExcel("EPICOR");
        }


        protected void btn_Excel_Click(object sender, EventArgs e)
        {
            exportToExcel("STANDARD");
        }

        private void exportToExcel(string type)
        {
            DateTime invoiceDateFrom, invoiceDateTo, subDocDateFrom, subDocDateTo, interfaceDateFrom, interfaceDateTo;
            string invoicePrefix = String.Empty;
            int invoiceSeqFrom = 0, invoiceSeqTo = 0, invoiceYear = 0, officeId = -1, vendorId = -1, paymentTermId = -1, tradingAgencyId = -1, handlingOfficeId = -1;
            string contractNo = String.Empty;

            if (txt_InvoiceDateFrom.Text.Trim() != String.Empty)
            {
                invoiceDateFrom = Convert.ToDateTime(txt_InvoiceDateFrom.Text.Trim());
                if (txt_InvoiceDateTo.Text.Trim() == String.Empty)
                    txt_InvoiceDateTo.Text = txt_InvoiceDateFrom.Text;
                invoiceDateTo = Convert.ToDateTime(txt_InvoiceDateTo.Text.Trim());
            }
            else
            {
                invoiceDateFrom = DateTime.MinValue;
                invoiceDateTo = DateTime.MinValue;
            }

            if (txt_SubDocDateFrom.Text.Trim() != String.Empty)
            {
                subDocDateFrom = Convert.ToDateTime(txt_SubDocDateFrom.Text.Trim());
                if (txt_SubDocDateTo.Text.Trim() == String.Empty)
                    txt_SubDocDateTo.Text = txt_SubDocDateFrom.Text;
                subDocDateTo = Convert.ToDateTime(txt_SubDocDateTo.Text.Trim());
            }
            else
            {
                subDocDateFrom = DateTime.MinValue;
                subDocDateTo = DateTime.MinValue;
            }

            if (txt_SubDocDateFrom.Text.Trim() != String.Empty)
            {
                subDocDateFrom = Convert.ToDateTime(txt_SubDocDateFrom.Text.Trim());
                if (txt_SubDocDateTo.Text.Trim() == String.Empty)
                    txt_SubDocDateTo.Text = txt_SubDocDateFrom.Text;
                subDocDateTo = Convert.ToDateTime(txt_SubDocDateTo.Text.Trim());
            }
            else
            {
                subDocDateFrom = DateTime.MinValue;
                subDocDateTo = DateTime.MinValue;
            }

            if (txt_InterfaceDateFrom.Text.Trim() != String.Empty)
            {
                interfaceDateFrom = Convert.ToDateTime(txt_InterfaceDateFrom.Text.Trim());
                if (txt_InterfaceDateTo.Text.Trim() == String.Empty)
                    txt_InterfaceDateTo.Text = txt_InterfaceDateFrom.Text;
                interfaceDateTo = Convert.ToDateTime(txt_InterfaceDateTo.Text.Trim());
            }
            else
            {
                interfaceDateFrom = DateTime.MinValue;
                interfaceDateTo = DateTime.MinValue;
            }

            if (txt_InvoiceNoFrom.Text.Trim() != String.Empty)
            {
                invoicePrefix = WebUtil.getInvoicePrefix(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqFrom = WebUtil.getInvoiceSeq(txt_InvoiceNoFrom.Text.Trim());
                invoiceSeqTo = WebUtil.getInvoiceSeq(txt_InvoiceNoTo.Text.Trim());
                invoiceYear = WebUtil.getInvoiceYear(txt_InvoiceNoFrom.Text.Trim());
            }

            officeId = int.Parse(ddl_Office.SelectedValue);
            handlingOfficeId = int.Parse(ddl_HandlingOffice.SelectedValue);
            tradingAgencyId = int.Parse(ddl_TradingAgency.SelectedValue);
            paymentTermId = int.Parse(ddl_PaymentTerm.SelectedValue);
            vendorId = (this.txt_SupplierName.VendorId == int.MinValue ? -1 : this.txt_SupplierName.VendorId);
            contractNo = this.txt_ContractNo.Text.Trim();

            TypeCollector paymentStatusList = TypeCollector.Inclusive;

            if (this.chkNotReady.Checked) paymentStatusList.append(ShippingDocWFS.NOT_READY.Id);
            if (this.chkReady.Checked) paymentStatusList.append(ShippingDocWFS.READY.Id);
            if (this.chkRejected.Checked) paymentStatusList.append(ShippingDocWFS.REJECTED.Id);
            if (this.chkAccepted.Checked) paymentStatusList.append(ShippingDocWFS.ACCEPTED.Id);
            if (this.chkReviewed.Checked) paymentStatusList.append(ShippingDocWFS.REVIEWED.Id);

            if (type == "EPICOR")
            {
                //PaymentStatusEnquiryEpicorReport report = new PaymentStatusEnquiryEpicorReport();
                //report.SetDataSource(AccountReportManager.Instance.getPaymentStatusEnquiryEpicorDataSet(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, handlingOfficeId, invoiceDateFrom, invoiceDateTo,
                //    subDocDateFrom, subDocDateTo, interfaceDateFrom, interfaceDateTo, paymentTermId, tradingAgencyId, vendorId, paymentStatusList, contractNo));
                //ReportHelper.export(report, HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.Excel, "PaymentStatusEnquiryEpicorList");
                this.outputEpicorExcel(AccountReportManager.Instance.getPaymentStatusEnquiryEpicorDataSet(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, handlingOfficeId, invoiceDateFrom, invoiceDateTo,
                    subDocDateFrom, subDocDateTo, interfaceDateFrom, interfaceDateTo, paymentTermId, tradingAgencyId, vendorId, paymentStatusList, contractNo));
            }
            else
                this.outputExcel(AccountReportManager.Instance.getPaymentStatusEnquiryDataSet(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, handlingOfficeId, invoiceDateFrom, invoiceDateTo,
                                subDocDateFrom, subDocDateTo, interfaceDateFrom, interfaceDateTo, paymentTermId, tradingAgencyId, vendorId, paymentStatusList, contractNo));
        }

        private void outputEpicorExcel(PaymentStatusEnquiryEpicorDs ds)
        {
            string sourceFileDir = this.ApplPhysicalPath + @"account\";
            string sourceFileName = "PaymentStatusEnquiryEpicorList.xlsx";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(WebConfig.getValue("appSettings", "UPLOAD_AP_Folder") + @"PaymentStatusEnquiryEpicorList-{0}-{1}.xlsx", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;
            StringBuilder deductionRemark = new StringBuilder();

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet1");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");

            if (worksheetID == string.Empty)
                Console.WriteLine("failure open spreadsheet");

            int[] mainStyleList = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 2, 34);

            int i = 2;
            int vendorId = 0;
            int officeId = 0;
            decimal curTotal = 0;
            decimal supTotal = 0;
            decimal deductionTotal = 0;
            int rowCount = 1;
            string nextSupplier = string.Empty;
            foreach (PaymentStatusEnquiryEpicorDs.PaymentStatusEnquiryRow r in ds.Tables[0].Rows)
            {
                curTotal += r.NetAmount;
                supTotal += r.NetAmount_USD;
                deductionTotal += r.DeductionAmt;
                OpenXmlUtil.setCellValue(document, worksheetID, i, 1, r.EpicorCompanyId, CellValues.String, mainStyleList[0]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 2, r.GroupId, CellValues.String, mainStyleList[1]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 3, r.BankAccount, CellValues.String, mainStyleList[2]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 4, r.PaymentMethod.ToString(), CellValues.Number, mainStyleList[3]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 5, r.EpicorSupplierId, CellValues.String, mainStyleList[4]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 6, r.IsPaymentDateNull() ? string.Empty : DateTimeUtility.getDateString(r.PaymentDate), CellValues.String, mainStyleList[5]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 7, r.PaymentNo, CellValues.String, mainStyleList[6]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 8, r.SupplierInvoiceNo, CellValues.String, mainStyleList[7]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 9, r.BankCharge.ToString(), CellValues.Number, mainStyleList[8]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 10, r.IsLCBillRefNoNull() ? string.Empty : r.LCBillRefNo, CellValues.String, mainStyleList[9]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 11, r.AutoPost == 1 ? "True" : "False", CellValues.String, mainStyleList[10]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 12, r.Source, CellValues.String, mainStyleList[11]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 13, r.IsInvoiceDateNull() ? string.Empty : DateTimeUtility.getDateString(r.InvoiceDate), CellValues.String, mainStyleList[12]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 14, r.SupplierName, CellValues.String, mainStyleList[13]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 15, r.InvoiceNo, CellValues.String, mainStyleList[14]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 16, r.ContractNo, CellValues.String, mainStyleList[15]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 17, r.ItemNo, CellValues.String, mainStyleList[16]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 18, r.CurrencyCode, CellValues.String, mainStyleList[17]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 19, r.NetAmount.ToString(), CellValues.Number, mainStyleList[18]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 20, r.NetAmount_USD.ToString(), CellValues.Number, mainStyleList[19]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 21, r.IsLGDateNull() ? string.Empty : DateTimeUtility.getDateString(r.LGDate), CellValues.String, mainStyleList[20]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 22, r.DeductionAmt.ToString(), CellValues.Number, mainStyleList[21]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 23, r.WorkflowStatus, CellValues.String, mainStyleList[22]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 24, r.ReviewedBy, CellValues.String, mainStyleList[23]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 25, DateTimeUtility.getDateString(r.ReviewedOn), CellValues.String, mainStyleList[24]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 26, r.HoldPayment.ToString(), CellValues.String, mainStyleList[25]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 27, r.ReleaseLock == 1 ? "True" : "False", CellValues.String, mainStyleList[26]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 28, r.IsChinaGBTestRequired.ToString(), CellValues.String, mainStyleList[27]);

                if (r.IsChinaGBTestRequired)
                {
                    if (r.GBTestResult == 1)
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 29, "Pass", CellValues.String, mainStyleList[28]);
                    else if (r.GBTestResult == 0)
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 29, "Fail (Hold Payment)", CellValues.String, mainStyleList[28]);
                    else if (r.GBTestResult == 2)
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 29, "Fail (Release Payment)", CellValues.String, mainStyleList[28]);
                    else if (r.GBTestResult == 9)
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 29, "(Cannot Release Payment)", CellValues.String, mainStyleList[28]);
                    else
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 29, string.Empty, CellValues.String, mainStyleList[28]);

                    OpenXmlUtil.setCellValue(document, worksheetID, i, 30, r.ProductTeamCode, CellValues.String, mainStyleList[29]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 31, r.ProductTeamDescription, CellValues.String, mainStyleList[30]);
                }
                else
                {
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 29, string.Empty, CellValues.String, mainStyleList[28]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 30, r.ProductTeamCode, CellValues.String, mainStyleList[29]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 31, r.ProductTeamDescription, CellValues.String, mainStyleList[30]);
                }
                OpenXmlUtil.setCellValue(document, worksheetID, i, 32, r.DeductionRemark, CellValues.String, mainStyleList[31]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 33, r.CO, CellValues.String, mainStyleList[32]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 34, r.NetAmount.ToString(), CellValues.Number, mainStyleList[33]);

                if (rowCount < ds.Tables[0].Rows.Count) //next row supplierid
                {
                    nextSupplier = ds.Tables[0].Rows[rowCount]["EpicorSupplierId"].ToString();
                }

                if (nextSupplier != r.EpicorSupplierId || rowCount == ds.Tables[0].Rows.Count)
                {
                    vendorId = r.SupplierID;

                    string contractNo = r.ParentContractNo;
                    ContractDef contractDef = OrderManager.Instance.getContractByContractNo(contractNo);
                    officeId = contractDef.Office.OfficeId;

                    int[] styleList = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 3, 21);
                    int futureOrderCount = ShipmentManager.Instance.getFutureOrderCountByVendorId(vendorId, -1);
                    int futureOrderCountByOffice = ShipmentManager.Instance.getFutureOrderCountByVendorId(vendorId, officeId);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 14, "Future Orders : ", CellValues.SharedString, styleList[13]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 15, futureOrderCountByOffice.ToString() + " / " + futureOrderCount.ToString(), CellValues.SharedString, styleList[14]);
                    OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(17, i), OpenXmlUtil.getCellReference(18, i));
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 17, "Currency Total : ", CellValues.SharedString, styleList[16]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 19, curTotal.ToString(), CellValues.Number, styleList[18]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 22, deductionTotal.ToString(), CellValues.Number, styleList[18]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 14, "Future Orders Amount : ", CellValues.SharedString, styleList[13]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 15, "TBC", CellValues.SharedString, styleList[14]);
                    OpenXmlUtil.mergeCells(document, worksheetID, OpenXmlUtil.getCellReference(17, i), OpenXmlUtil.getCellReference(18, i));
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 17, "Supplier Total : ", CellValues.SharedString, styleList[16]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 20, supTotal.ToString(), CellValues.Number, styleList[18]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 14, "T & C : ", CellValues.SharedString, styleList[13]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 15, ShipmentManager.Instance.getVendorNSLDocumentCount(vendorId), CellValues.SharedString, styleList[14]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 14, "Next Claims (excluded UKDS) : ", CellValues.SharedString, styleList[13]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 15, ShipmentManager.Instance.getOSNextClaimAmtByVendorId(vendorId, officeId), CellValues.SharedString, styleList[14]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 14, "Next Claims - All Office (excluded UKDS) : ", CellValues.SharedString, styleList[13]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 15, ShipmentManager.Instance.getOSNextClaimAmtByVendorId(vendorId, -1), CellValues.SharedString, styleList[14]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 14, "Advance Payment By Instalment : ", CellValues.SharedString, styleList[13]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 15, ShipmentManager.Instance.getOSAdvancePaymentInstalmentAmt(vendorId), CellValues.SharedString, styleList[14]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 14, "Balance of Advance Payment : ", CellValues.SharedString, styleList[13]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 15, "TBC", CellValues.SharedString, styleList[14]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 14, "Outstanding Payable Amount Before Payment : ", CellValues.SharedString, styleList[13]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 15, "TBC", CellValues.SharedString, styleList[14]);
                    i++;
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 14, "Outstanding Payable Amount After Payment : ", CellValues.SharedString, styleList[13]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 15, "TBC", CellValues.SharedString, styleList[14]);

                    curTotal = 0;
                    supTotal = 0;
                }
                rowCount++;
                i++;
            }

            /*
            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();
            */
            OpenXmlUtil.saveComplete(document);

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }


        private void outputExcel(PaymentStatusEnquiryDs ds)
        {
            string sourceFileDir = this.ApplPhysicalPath + @"account\";
            string sourceFileName = "PaymentStatusEnquiry.xlsx";
            string uId = DateTime.Now.ToString("yyyyMMddss");
            string destFile = String.Format(WebConfig.getValue("appSettings", "UPLOAD_AP_Folder") + @"PaymentStatusEnquiry-{0}-{1}.xlsx", this.LogonUserId.ToString(), uId);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);

            string worksheetID = string.Empty;
            string templateWorksheetID = string.Empty;

            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            worksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet1");
            templateWorksheetID = OpenXmlUtil.getWorksheetId(document, "Sheet2");

            if (worksheetID == string.Empty)
                Console.WriteLine("failure open spreadsheet");

            int[] mainStyleList = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 8, 21);

            int i = 2;
            int vendorId = 0;
            int officeId = 0;
            int idx = 1;
            foreach (PaymentStatusEnquiryDs.PaymentStatusEnquiryRow r in ds.Tables[0].Rows)
            {
                OpenXmlUtil.setCellValue(document, worksheetID, i, 1, r.SunAccountCode, CellValues.String, mainStyleList[0]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 2, r.VendorName, CellValues.String, mainStyleList[1]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 3, r.InvoiceNo, CellValues.String, mainStyleList[2]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 4, r.ProductTeamCode, CellValues.String, mainStyleList[3]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 5, r.ProductTeamDesc, CellValues.String, mainStyleList[4]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 6, r.ContractNo, CellValues.String, mainStyleList[5]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 7, r.ItemNo, CellValues.String, mainStyleList[6]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 8, r.IsSupplierInvoiceNoNull() ? string.Empty : r.SupplierInvoiceNo, CellValues.String, mainStyleList[7]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 9, r.CurrencyCode, CellValues.String, mainStyleList[8]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 10, r.NetAmt.ToString(), CellValues.Number, mainStyleList[9]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 11, r.NetAmtInUSD.ToString(), CellValues.Number, mainStyleList[10]);

                if (r.DMSWorkflowStatusId == 0)
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, "Not Ready To Check", CellValues.String, mainStyleList[11]);
                else if (r.DMSWorkflowStatusId == 1)
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, "Document Ready To Check", CellValues.String, mainStyleList[11]);
                else if (r.DMSWorkflowStatusId == 2)
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, "Accepted", CellValues.String, mainStyleList[11]);
                else if (r.DMSWorkflowStatusId == 3)
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, "Rejected", CellValues.String, mainStyleList[11]);
                else if (r.DMSWorkflowStatusId == 4)
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 12, "Reviewed", CellValues.String, mainStyleList[11]);

                if (r.EditLock == 0)
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 13, "Yes", CellValues.String, mainStyleList[12]);
                else
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 13, "No", CellValues.String, mainStyleList[12]);

                OpenXmlUtil.setCellValue(document, worksheetID, i, 14, r.IsReviewUserNull() ? string.Empty : r.ReviewUser, CellValues.String, mainStyleList[13]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 15, r.IsReviewDateTimeNull() ? string.Empty : DateTimeUtility.getDateString(r.ReviewDateTime), CellValues.String, mainStyleList[14]);

                if (r.IsPaymentHold == true)
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 16, "Yes", CellValues.String, mainStyleList[15]);
                else
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 16, "No", CellValues.String, mainStyleList[15]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 17, DateTimeUtility.getDateString(r.InvoiceDate), CellValues.String, mainStyleList[16]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 18, r.LabTestIncome.ToString(), CellValues.Number, mainStyleList[17]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 19, r.TotalShippedQty.ToString(), CellValues.Number, mainStyleList[18]);
                if (r.IsChinaGBTestRequired)
                {
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 20, "Yes", CellValues.String, mainStyleList[19]);
                    if (r.ChinaGBTestResult == 1)
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 21, "P", CellValues.String, mainStyleList[20]);
                    else if (r.ChinaGBTestResult == 0)
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 21, "F-H", CellValues.String, mainStyleList[20]);
                    else if (r.ChinaGBTestResult == 2)
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 21, "F-R", CellValues.String, mainStyleList[20]);
                    else if (r.ChinaGBTestResult == 9)
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 21, "F", CellValues.String, mainStyleList[20]);
                    else
                        OpenXmlUtil.setCellValue(document, worksheetID, i, 21, "NIL", CellValues.String, mainStyleList[20]);
                }
                else
                {
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 20, "No", CellValues.String, mainStyleList[19]);
                    OpenXmlUtil.setCellValue(document, worksheetID, i, 21, "", CellValues.String, mainStyleList[20]);
                }

                if (idx == ds.Tables[0].Rows.Count)
                {
                    vendorId = r.VendorId;
                    ContractDef contractDef = OrderManager.Instance.getContractByContractNo(r.ParentContractNo);
                    officeId = contractDef.Office.OfficeId;
                }

                idx++;
                i++;
            }
            i++;

            if ((this.txt_SupplierName.VendorId == int.MinValue ? -1 : this.txt_SupplierName.VendorId) != -1)
            {
                int[] styleList = OpenXmlUtil.getStyleIdList(document, templateWorksheetID, 1, 2);
                int futureOrderCount = ShipmentManager.Instance.getFutureOrderCountByVendorId(vendorId, -1);
                int futureOrderCountByOffice = ShipmentManager.Instance.getFutureOrderCountByVendorId(vendorId, officeId);


                OpenXmlUtil.setCellValue(document, worksheetID, i, 2, "Future Orders : ", CellValues.SharedString, styleList[0]);
                OpenXmlUtil.setCellValue(document, worksheetID, i, 3, futureOrderCountByOffice.ToString() + " / " + futureOrderCount.ToString(), CellValues.SharedString, styleList[1]);

                OpenXmlUtil.setCellValue(document, worksheetID, i + 1, 2, "T & C : ", CellValues.SharedString, styleList[0]);
                OpenXmlUtil.setCellValue(document, worksheetID, i + 1, 3, ShipmentManager.Instance.getVendorNSLDocumentCount(vendorId), CellValues.SharedString, styleList[1]);

                OpenXmlUtil.setCellValue(document, worksheetID, i + 2, 2, "Next Claims (excluded UKDS) : ", CellValues.SharedString, styleList[0]);
                OpenXmlUtil.setCellValue(document, worksheetID, i + 2, 3, ShipmentManager.Instance.getOSNextClaimAmtByVendorId(vendorId, officeId), CellValues.SharedString, styleList[1]);

                OpenXmlUtil.setCellValue(document, worksheetID, i + 3, 2, "Next Claims - All Office (excluded UKDS) : ", CellValues.SharedString, styleList[0]);
                OpenXmlUtil.setCellValue(document, worksheetID, i + 3, 3, ShipmentManager.Instance.getOSNextClaimAmtByVendorId(vendorId, -1), CellValues.SharedString, styleList[1]);

                OpenXmlUtil.setCellValue(document, worksheetID, i + 4, 2, "Advance Payment By Instalment : ", CellValues.SharedString, styleList[0]);
                OpenXmlUtil.setCellValue(document, worksheetID, i + 4, 3, ShipmentManager.Instance.getOSAdvancePaymentInstalmentAmt(vendorId), CellValues.SharedString, styleList[1]);

            }

            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }


        private void updateDMSWorkflowStatus(int dmsWorkflowStatusId)
        {
            updateDMSWorkflowStatus(dmsWorkflowStatusId, RejectPaymentReason.NoReason.Id);
        }

        private void updateDMSWorkflowStatus(int dmsWorkflowStatusId, int rejectPaymentReasonId)
        {
            ArrayList selectedList = new ArrayList();

            foreach (GridViewRow row in gv_Inv.Rows)
            {
                CheckBox ckb = (CheckBox)row.Cells[0].FindControl("ckb_inv");
                if (ckb.Checked)
                {
                    PaymentStatusEnquiryDef def = (PaymentStatusEnquiryDef)vwSearchResult[row.RowIndex];

                    selectedList.Add(def.ShipmentId.ToString() + "," + def.SplitShipmentId.ToString());
                }
            }

            if (selectedList.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "inv", "alert('No record(s) were selected.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.UpdateDMSWorkflowStatusId);

            Context.Items.Add(AccountCommander.Param.shipmentIdList, selectedList);
            Context.Items.Add(AccountCommander.Param.dmsWorkflowStatusId, dmsWorkflowStatusId);
            if (RejectPaymentReason.getReason(rejectPaymentReasonId) == null)
                rejectPaymentReasonId = RejectPaymentReason.NoReason.Id;
            Context.Items.Add(AccountCommander.Param.rejectPaymentReasonId, rejectPaymentReasonId);

            forwardToScreen(null);

            btn_Search_Click(null, null);
        }

        private void holdPayment(bool isPaymentHold)
        {
            ArrayList selectedList = new ArrayList();

            foreach (GridViewRow row in gv_Inv.Rows)
            {
                CheckBox ckb = (CheckBox)row.Cells[0].FindControl("ckb_inv");
                if (ckb.Checked)
                {
                    PaymentStatusEnquiryDef def = (PaymentStatusEnquiryDef)vwSearchResult[row.RowIndex];

                    selectedList.Add(def.ShipmentId.ToString() + "," + def.SplitShipmentId.ToString());
                }
            }

            if (selectedList.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "inv", "alert('No record(s) were selected.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.HoldPayment);

            Context.Items.Add(AccountCommander.Param.shipmentIdList, selectedList);
            Context.Items.Add(AccountCommander.Param.isPaymentHold, isPaymentHold);

            forwardToScreen(null);

            btn_Search_Click(null, null);
        }


        protected void btn_Reject_Click(object sender, EventArgs e)
        {
            this.updateDMSWorkflowStatus(ShippingDocWFS.REJECTED.Id, int.Parse(hid_RejectReasonId.Value));
        }

        protected void btn_Accept_Click(object sender, EventArgs e)
        {
            this.updateDMSWorkflowStatus(ShippingDocWFS.ACCEPTED.Id);
        }

        protected void btn_Hold_Click(object sender, EventArgs e)
        {
            this.holdPayment(true);
        }

        protected void btn_Unhold_Click(object sender, EventArgs e)
        {
            this.holdPayment(false);
        }

        private string sortExpression
        {
            get { return (string)ViewState["SortExpression"]; }
            set { ViewState["SortExpression"] = value; }
        }

        private SortDirection sortDirection
        {
            get { return (SortDirection)ViewState["SortDirection"]; }
            set { ViewState["SortDirection"] = value; }

        }

        protected void gv_Inv_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (sortExpression == e.SortExpression)
            {
                sortDirection = (sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending);
            }
            else
            {
                sortExpression = e.SortExpression;
                sortDirection = SortDirection.Ascending;
            }

            PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer.CompareType compareType;

            if (sortExpression == "ContractNo")
                compareType = PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer.CompareType.ContractNo;
            else if (sortExpression == "ItemNo")
                compareType = PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer.CompareType.ItemNo;
            else if (sortExpression == "SupplierInvoiceNo")
                compareType = PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer.CompareType.SupplierInvoiceNo;
            else if (sortExpression == "InvoiceNo")
                compareType = PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer.CompareType.InvoiceNo;
            else if (sortExpression == "SupplierName")
                compareType = PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer.CompareType.SupplierName;
            else if (sortExpression == "NetAmtInUSD")
                compareType = PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer.CompareType.NetAmtInUSD;
            else if (sortExpression == "InvoiceDate")
                compareType = PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer.CompareType.InvoiceDate;
            else
                compareType = PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer.CompareType.InvoiceNo;

            vwSearchResult.Sort(new PaymentStatusEnquiryDef.PaymentStatusEnquiryComparer(compareType, sortDirection));
            this.gv_Inv.DataSource = vwSearchResult;
            this.gv_Inv.DataBind();
        }

        protected void btn_TriggerDMSComplete_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.MarkDMSComplete);
            Context.Items.Add(AccountCommander.Param.officeId, ddl_Office.selectedValueToInt);

            forwardToScreen(null);

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "inv", "alert('This has been completed successfully');", true);
        }
    }
}
