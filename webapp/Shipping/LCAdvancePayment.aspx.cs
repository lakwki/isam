using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.isam.dataserver.worker;
using com.next.common.web.commander;
using com.next.isam.domain.common;
using com.next.isam.appserver.shipping;
using com.next.isam.domain.types;
using com.next.isam.domain.shipping;
using com.next.isam.domain.product;
using com.next.infra.util;

namespace com.next.isam.webapp.shipping
{
    public partial class LCAdvancePayment : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private bool HasAccessRightsTo_View;
        private bool HasAccessRightsTo_SuperView;
        private bool HasAccessRightsTo_Submit;
        private ArrayList userOfficeIdList;
        private ArrayList userDepartmentList;
        private ArrayList userProductTeamList;


        protected void Page_Load(object sender, EventArgs e)
        {
            ArrayList userOfficeList;
            int UserId;

            userOfficeIdList = new ArrayList();
            userDepartmentList = new ArrayList();
            userProductTeamList = new ArrayList();

            HasAccessRightsTo_View = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcRequest.Id, ISAMModule.lcRequest.View);
            HasAccessRightsTo_SuperView = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcRequest.Id, ISAMModule.lcRequest.SuperView);
            HasAccessRightsTo_Submit = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcRequest.Id, ISAMModule.lcRequest.Submit);

            //********** For Testing **************************
            //if (this.LogonUserId == 574)
            //    HasAccessRightsTo_SuperView = true;
            //********** For Testing **************************
            UserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);

            txt_VendorName.setWidth(300);
            txt_VendorName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

            if (!Page.IsPostBack)
            {
                if (HasAccessRightsTo_SuperView)
                    userOfficeList = CommonUtil.getOfficeRefListByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralStatus.ACTIVE.Code);
                else
                    userOfficeList = (ArrayList)CommonUtil.getOfficeListByUserId(UserId, OfficeStructureType.PRODUCTCODE.Type);

                foreach (OfficeRef office in userOfficeList)
                {
                    userOfficeIdList.Add(office.OfficeId);
                }

                userProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALL, UserId, GeneralCriteria.ALLSTRING);
                vsUserProductTeamList = userProductTeamList;
                ddl_ProductTeam.bindList(userProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
                ddl_ProductTeam_Refresh();

                this.ddl_Office.bindList(userOfficeList, "Description", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
                if (userOfficeList.Count == 1)
                {
                    OfficeRef oref = (OfficeRef)userOfficeList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                }

                this.ddl_ProductTeam.bindList(userProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
                if (userProductTeamList.Count == 1) this.ddl_ProductTeam.SelectedIndex = 1;

                this.ddl_ProductTeam_Refresh();

                this.btn_Search.Enabled = (HasAccessRightsTo_View || HasAccessRightsTo_SuperView || HasAccessRightsTo_Submit);
            }
        }

        ArrayList vsLCAdvancePaymentList
        {
            get { return (ArrayList)ViewState["LCAdvancePaymentList"]; }
            set { ViewState["LCAdvancePaymentList"] = value; }
        }

        ArrayList vsUserProductTeamList
        {
            get { return (ArrayList)ViewState["UserProductTeamList"]; }
            set { ViewState["UserProductTeamList"] = value; }
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            int TotalNumberOfRecord;
            int NumberOfRecordToGet = 100; // -1 -> unlimit

            if (this.Page.IsValid)
            {
                pnl_SearchResult.Visible = true;
                ArrayList LCAdvancePaymentList = new ArrayList();
                TotalNumberOfRecord = getLCAdvancePaymentList(LCAdvancePaymentList, NumberOfRecordToGet);
                this.vsLCAdvancePaymentList = LCAdvancePaymentList;
                gv_LC.DataSource = LCAdvancePaymentList;
                gv_LC.DataBind();

                if (TotalNumberOfRecord > NumberOfRecordToGet && NumberOfRecordToGet > 0)
                    lbl_RowCount.Text = "Totally " + TotalNumberOfRecord.ToString() + " shipments found. Only the first " + NumberOfRecordToGet.ToString() + " are shown.";
                else
                    if (TotalNumberOfRecord > 0)
                    lbl_RowCount.Text = TotalNumberOfRecord.ToString() + " shipment" + (TotalNumberOfRecord > 1 ? "s" : "") + " found.";
                else
                    lbl_RowCount.Text = "";
            }
        }

        protected int getLCAdvancePaymentList(ArrayList LCAdvancePaymentList, int NumberOfRecordToGet)
        {
            int i;
            int vendorId;
            string itemNo;
            string contractNo;
            string LcNoFrom, LcNoTo;
            int LcBatchNoFrom, LcBatchNoTo;
            //int AdvancePaymentNoFrom, AdvancePaymentNoTo;
            string AdvancePaymentNoFrom, AdvancePaymentNoTo;

            if (this.txt_VendorName.KeyTextBox.Text == "")
                vendorId = -1;
            else
                vendorId = this.txt_VendorName.VendorId;

            itemNo = this.txt_ItemNo.Text;
            contractNo = this.txt_ContractNo.Text;

            ArrayList officeIdList = new ArrayList();
            if (ddl_Office.SelectedValue != "-1")
            {
                officeIdList.Add(Convert.ToInt32(ddl_Office.SelectedValue));
            }
            else
            {
                for (i = 1; i < ddl_Office.Items.Count; i++)
                    officeIdList.Add(Convert.ToInt32(ddl_Office.Items[i].Value));
            }

            ArrayList productTeamIdList = new ArrayList();
            if (ddl_ProductTeam.SelectedValue != "-1")
            {
                productTeamIdList.Add(Convert.ToInt32(ddl_ProductTeam.SelectedValue));
            }
            /*
            else
            {
                for (i = 1; i < ddl_ProductTeam.Items.Count; i++)
                    productTeamIdList.Add(Convert.ToInt32(ddl_ProductTeam.Items[i].Value));
            }
            */

            LcNoFrom = this.txt_LCNoFrom.Text;
            LcNoTo = this.txt_LCNoTo.Text;
            if (this.txt_LCNoFrom.Text == "")
                if (this.txt_LCNoTo.Text == "")
                    LcNoFrom = string.Empty;
                else
                    LcNoFrom = this.txt_LCNoTo.Text.Trim();
            else
                LcNoFrom = this.txt_LCNoFrom.Text.Trim();
            if (this.txt_LCNoTo.Text == "")
                if (this.txt_LCNoFrom.Text == "")
                    LcNoTo = string.Empty;
                else
                    LcNoTo = this.txt_LCNoFrom.Text.Trim();
            else
                LcNoTo = this.txt_LCNoTo.Text.Trim();

            if (this.txt_LCBatchNoFrom.Text == "")
                if (this.txt_LCBatchNoTo.Text == "")
                    LcBatchNoFrom = int.MinValue;
                else
                    LcBatchNoFrom = Convert.ToInt32(this.txt_LCBatchNoTo.Text);
            else
                LcBatchNoFrom = Convert.ToInt32(this.txt_LCBatchNoFrom.Text);

            if (this.txt_LCBatchNoTo.Text == "")
                if (this.txt_LCBatchNoFrom.Text == "")
                    LcBatchNoTo = int.MinValue;
                else
                    LcBatchNoTo = Convert.ToInt32(this.txt_LCBatchNoFrom.Text);
            else
                LcBatchNoTo = Convert.ToInt32(this.txt_LCBatchNoTo.Text);

            /*
            if (this.txt_AdvancePaymentNoFrom.Text == "")
                if (this.txt_AdvancePaymentNoFrom.Text == "")
                    AdvancePaymentNoFrom = int.MinValue;
                else
                    AdvancePaymentNoFrom = Convert.ToInt32(this.txt_AdvancePaymentNoTo.Text);
            else
                AdvancePaymentNoFrom = Convert.ToInt32(this.txt_AdvancePaymentNoFrom.Text);

            if (this.txt_AdvancePaymentNoTo.Text == "")
                if (this.txt_AdvancePaymentNoFrom.Text == "")
                    AdvancePaymentNoTo = int.MinValue;
                else
                    AdvancePaymentNoTo = Convert.ToInt32(this.txt_AdvancePaymentNoFrom.Text);
            else
                AdvancePaymentNoTo = Convert.ToInt32(this.txt_AdvancePaymentNoTo.Text);
            */
            if (this.txt_AdvancePaymentNoFrom.Text == "")
                if (this.txt_AdvancePaymentNoTo.Text == "")
                    AdvancePaymentNoFrom = string.Empty;
                else
                    AdvancePaymentNoFrom = this.txt_AdvancePaymentNoTo.Text.Trim();
            else
                AdvancePaymentNoFrom = this.txt_AdvancePaymentNoFrom.Text.Trim();

            if (this.txt_AdvancePaymentNoTo.Text == "")
                if (this.txt_AdvancePaymentNoFrom.Text == "")
                    AdvancePaymentNoTo = string.Empty;
                else
                    AdvancePaymentNoTo = this.txt_AdvancePaymentNoFrom.Text.Trim();
            else
                AdvancePaymentNoTo = this.txt_AdvancePaymentNoTo.Text.Trim();

            string NSLRefNoFrom = string.Empty;
            string NSLRefNoTo = string.Empty;
            if (this.txt_NSLRefNoFrom.Text == "")
                if (this.txt_NSLRefNoTo.Text == "")
                    NSLRefNoFrom = string.Empty;
                else
                    NSLRefNoFrom = this.txt_NSLRefNoTo.Text.Trim();
            else
                NSLRefNoFrom = this.txt_NSLRefNoFrom.Text.Trim();

            if (this.txt_NSLRefNoTo.Text == "")
                if (this.txt_NSLRefNoFrom.Text == "")
                    NSLRefNoTo = string.Empty;
                else
                    NSLRefNoTo = this.txt_NSLRefNoFrom.Text.Trim();
            else
                NSLRefNoTo = this.txt_NSLRefNoTo.Text.Trim();

            int isC19Order;
            if (this.ckb_C19.Checked && this.ckb_NotC19.Checked)
                isC19Order = -1;
            else if (this.ckb_C19.Checked)
                isC19Order = 1;
            else if (this.ckb_NotC19.Checked)
                isC19Order = 0;
            else
                isC19Order = 2;


            return LCWorker.Instance.getLCAdvancePayment(LCAdvancePaymentList, vendorId, itemNo, contractNo, officeIdList, productTeamIdList, this.txt_AtWHDateFrom.DateTime, this.txt_AtWHDateTo.DateTime,
                this.txt_LCIssueDateFrom.DateTime, this.txt_LCIssueDateTo.DateTime, LcNoFrom, LcNoTo, LcBatchNoFrom, LcBatchNoTo, AdvancePaymentNoFrom, AdvancePaymentNoTo,
                NSLRefNoFrom, NSLRefNoTo, this.txt_ApprovalDateFrom.DateTime, this.txt_ApprovalDateTo.DateTime, isC19Order, NumberOfRecordToGet);
        }

        protected void gv_LC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LCAdvancePaymentRef lcap = (LCAdvancePaymentRef)this.vsLCAdvancePaymentList[e.Row.RowIndex];

                ((Label)e.Row.FindControl("lbl_ContractNo")).Text = lcap.ContractNo;
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = lcap.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_InvoiceDate")).Text = DateTimeUtility.getDateString(lcap.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = lcap.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = lcap.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_DeliveryDate")).Text = DateTimeUtility.getDateString(lcap.SupplierAtWarehouseDate);
                ((Label)e.Row.FindControl("lbl_POQty")).Text = lcap.TotalPoQty.ToString();
                ((Label)e.Row.FindControl("lbl_ShippedQty")).Text = lcap.TotalShippedQty.ToString();
                ((Label)e.Row.FindControl("lbl_InvoiceAmount")).Text = lcap.TotalShippedAmt.ToString();
                ((Label)e.Row.FindControl("lbl_ContractStatus")).Text = lcap.ShipmentStatus.Name;
                ((Label)e.Row.FindControl("lbl_InvoicePaid")).Text = (lcap.ApDate != DateTime.MinValue ? "Y" : "N");
                ((Label)e.Row.FindControl("lbl_LCAppNo")).Text = (lcap.LCApplicationNo > 0 ? lcap.LCApplicationNo.ToString("000000") : string.Empty);
                ((Label)e.Row.FindControl("lbl_LCNo")).Text = lcap.LCNo;
                ((Label)e.Row.FindControl("lbl_LCIssueDate")).Text = DateTimeUtility.getDateString(lcap.LCIssueDate);
                ((Label)e.Row.FindControl("lbl_LCExpiredDate")).Text = DateTimeUtility.getDateString(lcap.LCExpiryDate);
                //((Label)e.Row.FindControl("lbl_PaymentDeductionAmt")).Text = (lcap.AdvancePaymentStatus == null ? string.Empty : lcap.ExpectedDeductAmt.ToString());

                ((Label)e.Row.FindControl("lbl_IsAnyPaymentDeductionInLC")).Text = (lcap.PaymentDeductionAmtInLC == decimal.MinValue || lcap.PaymentDeductionAmtInLC == 0 ? "N" : "Y");
                ((Label)e.Row.FindControl("lbl_PaymentDeductionAmtInLC")).Text = (lcap.PaymentDeductionAmtInLC == decimal.MinValue ? string.Empty : lcap.PaymentDeductionAmtInLC.ToString());

                ((Label)e.Row.FindControl("lbl_PaymentDeductionAmt")).Text = (lcap.AdvancePaymentStatus == null ? string.Empty : lcap.ActualDeductAmt.ToString());
                ((Label)e.Row.FindControl("lbl_AdvancePaymentNo")).Text = lcap.PaymentNo;

                System.Drawing.Color foreColor = System.Drawing.Color.Black;
                DateTime actionDate = DateTime.MinValue;
                if (lcap.AdvancePaymentStatus == NSSAdvancePaymentWFS.PENDING_FOR_APPROVAL)
                {
                    foreColor = System.Drawing.Color.DarkOrange;
                    actionDate = lcap.SubmittedDate;
                }
                else if (lcap.AdvancePaymentStatus == NSSAdvancePaymentWFS.APPROVED)
                {
                    foreColor = System.Drawing.Color.Green;
                    actionDate = lcap.ApprovedDate;
                }
                else if (lcap.AdvancePaymentStatus == NSSAdvancePaymentWFS.REJECTED)
                {
                    foreColor = System.Drawing.Color.Red;
                    actionDate = lcap.RejectDate;
                }
                Label advPayStatus = ((Label)e.Row.FindControl("lbl_AdvancePaymentStatus"));
                advPayStatus.ForeColor = foreColor;
                ((Label)e.Row.FindControl("lbl_AdvancePaymentStatus")).Text = (lcap.AdvancePaymentStatus == null ? string.Empty : lcap.AdvancePaymentStatus.Prefix);
                ((Label)e.Row.FindControl("lbl_NSLRefNo")).Text = lcap.NSLRefNo;

                Label advPayActionDate = (Label)e.Row.FindControl("lbl_AdvPayActionDate");
                advPayActionDate.ForeColor = foreColor;
                advPayActionDate.Text = DateTimeUtility.getDateString(actionDate);
                PaymentTermRef paymentTerm=CommonWorker.Instance.getPaymentTermByKey(lcap.PaymentTermId);
                ((Label)e.Row.FindControl("lbl_PaymentTerm")).Text = paymentTerm.PaymentTermDescription;
            }
        }

        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList ProductTeamList;
            int nOfficeId;
            int nUserId;

            nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);
            nOfficeId = int.Parse(this.ddl_Office.SelectedValue);
            if (nOfficeId == -1) nOfficeId = GeneralCriteria.ALL;

            ProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, nOfficeId, nUserId, GeneralCriteria.ALLSTRING);
            this.ddl_ProductTeam.bindList(ProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_ProductTeam_Refresh();

            return;
        }

        protected void ddl_ProductTeam_Refresh()
        {
            if ((this.ddl_Office.SelectedIndex == 0 && this.ddl_Office.Items.Count > 2))
            {
                this.ddl_ProductTeam.SelectedIndex = 0;
                this.ddl_ProductTeam.Enabled = false;
            }
            else
            {
                this.ddl_ProductTeam.Enabled = true;
            }
        }


        protected void btn_Print_Click(object sender, EventArgs e)
        {
            int TotalNumberOfRecord;
            int NumberOfRecordToGet = -1;

            ArrayList LCAdvancePaymentList = new ArrayList();
            if (this.Page.IsValid)
            {
                TotalNumberOfRecord = getLCAdvancePaymentList(LCAdvancePaymentList, NumberOfRecordToGet);
                this.vsLCAdvancePaymentList = LCAdvancePaymentList;
            }

            string sourceFileDir = Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + @"report_template\";
            string sourceFileName = "lcAdvancePaymentSearchResult.xlsm";
            string timeString = DateTime.Now.ToString("yyMMddhhmmss");
            string destFile = String.Format(this.ApplPhysicalPath + @"reporter\tmpReport\LCAdvancePaymentSearchResult-{0}-{1}.xlsm", this.LogonUserId.ToString(), timeString);
            File.Copy(sourceFileDir + sourceFileName, destFile, true);
            File.SetAttributes(destFile, FileAttributes.Normal);
            SpreadsheetDocument document = SpreadsheetDocument.Open(destFile, true);

            string outputID = string.Empty;
            string templateID = string.Empty;
            int numberOfColumn = 23;
            outputID = OpenXmlUtil.getWorksheetId(document, "Sheet1");
            templateID = OpenXmlUtil.getWorksheetId(document, "Template");

            // set report param display
            int[] styles;
            int detailStartRow = 3;
            int startingCol = 1;
            
            int r = detailStartRow;
            for (int i = 0; i < LCAdvancePaymentList.Count; i++)
            {
                int c = startingCol;
                LCAdvancePaymentRef dataRow = (LCAdvancePaymentRef)LCAdvancePaymentList[i];
                DateTime actionDate = (dataRow.AdvancePaymentStatus==null?DateTime.MinValue
                                    : dataRow.AdvancePaymentStatus.Prefix == "R" ? dataRow.RejectDate
                                    : dataRow.AdvancePaymentStatus.Prefix == "A" ? dataRow.ApprovedDate
                                    : dataRow.AdvancePaymentStatus.Prefix == "P" ? dataRow.SubmittedDate
                                    : DateTime.MinValue);
                styles = OpenXmlUtil.getStyleIdList(document, templateID, 3 + (dataRow.AdvancePaymentStatus == null ? 0 : dataRow.AdvancePaymentStatus.Id) * 2 + i % 2, numberOfColumn + 1);

                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.ContractNo, CellValues.SharedString, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.DeliveryNo.ToString(), CellValues.SharedString, styles[c++]);
                if (dataRow.InvoiceDate != null && dataRow.InvoiceDate!=DateTime.MinValue)
                    OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.InvoiceDate.ToOADate().ToString(), CellValues.Number, styles[c++]);
                else
                    OpenXmlUtil.setCellValue(document, outputID, r, c, string.Empty, CellValues.Date, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.Vendor.Name, CellValues.SharedString, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.Product.ItemNo, CellValues.SharedString, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.SupplierAtWarehouseDate.ToOADate().ToString(), CellValues.Number, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.TotalPoQty.ToString(), CellValues.Number, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.TotalShippedQty.ToString(), CellValues.Number, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.TotalShippedAmt.ToString(), CellValues.Number, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.ShipmentStatus.Name, CellValues.SharedString, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, (dataRow.ApDate != DateTime.MinValue ? "Y" : "N"), CellValues.SharedString, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, (dataRow.LCApplicationNo > 0 ? dataRow.LCApplicationNo.ToString("000000") : string.Empty), CellValues.SharedString, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, (dataRow.LCNo == null ? string.Empty : dataRow.LCNo), CellValues.SharedString, styles[c++]);
                if (dataRow.LCIssueDate != null && dataRow.LCIssueDate!=DateTime.MinValue)
                    OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.LCIssueDate.ToOADate().ToString(), CellValues.Number, styles[c++]);
                else
                    OpenXmlUtil.setCellValue(document, outputID, r, c, string.Empty, CellValues.Date, styles[c++]);
                if (dataRow.LCExpiryDate != null && dataRow.LCExpiryDate != DateTime.MinValue)
                    OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.LCExpiryDate.ToOADate().ToString(), CellValues.Number, styles[c++]);
                else
                    OpenXmlUtil.setCellValue(document, outputID, r, c, string.Empty, CellValues.Date, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, (dataRow.PaymentDeductionAmtInLC == decimal.MinValue ? "N" : "Y"), CellValues.SharedString, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, (dataRow.PaymentDeductionAmtInLC == decimal.MinValue ? string.Empty : dataRow.PaymentDeductionAmtInLC.ToString()), CellValues.Number, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, (dataRow.AdvancePaymentStatus == null ? string.Empty : dataRow.ActualDeductAmt.ToString()), CellValues.Number, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.PaymentNo, CellValues.SharedString, styles[c++]);
                if (actionDate != DateTime.MinValue)
                    OpenXmlUtil.setCellValue(document, outputID, r, c, actionDate.ToOADate().ToString(), CellValues.Number, styles[c++]);
                else
                    OpenXmlUtil.setCellValue(document, outputID, r, c, string.Empty, CellValues.Date, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, dataRow.NSLRefNo, CellValues.SharedString, styles[c++]);
                OpenXmlUtil.setCellValue(document, outputID, r, c, (dataRow.AdvancePaymentStatus == null ? string.Empty : dataRow.AdvancePaymentStatus.Prefix), CellValues.SharedString, styles[c++]);

                PaymentTermRef paymentTerm = CommonWorker.Instance.getPaymentTermByKey(dataRow.PaymentTermId);
                OpenXmlUtil.setCellValue(document, outputID, r, c, paymentTerm.PaymentTermDescription, CellValues.SharedString, styles[c++]);
                r++;
            }
            document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            document.WorkbookPart.Workbook.Save();
            document.Close();
            document.Dispose();

            WebHelper.outputFileAsHttpRespone(Response, destFile, true);
        }


    }
}
