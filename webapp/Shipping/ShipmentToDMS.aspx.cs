using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using com.next.isam.webapp.commander.shipment;
using com.next.isam.webapp.commander;
using com.next.isam.domain.order;
using com.next.isam.domain.shipping;
using com.next.isam.domain.types;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.infra.web;
using com.next.isam.appserver.common;
using com.next.isam.appserver.account;
using com.next.common.domain.dms;
using com.next.isam.reporter.accounts;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.reporter.helper;
using com.next.isam.appserver.shipping;
using com.next.infra.util;

namespace com.next.isam.webapp.shipping
{
    public partial class ShipmentToDMS : com.next.isam.webapp.usercontrol.PageTemplate
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
                txt_SupplierName.setWidth(300);
                txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
                ddl_PaymentTerm.bindList(CommonManager.Instance.getPaymentTermList(), "PaymentTermDescription", "PaymentTermId", "", "--All--", GeneralCriteria.ALL.ToString());
                this.btn_Search.Attributes.Add("onclick", "isValidSearch();");
                this.radUnchecked.Checked = true;
            }
        }

        protected void InvDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ShipmentToDMSDef def = (ShipmentToDMSDef)vwSearchResult[e.Row.RowIndex];
                ((Label)e.Row.FindControl("lbl_SupplierName")).Text = def.SupplierName;
                ((Label)e.Row.FindControl("lbl_ContractNo")).Text = def.ContractNo;
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = def.ItemNo;
                ((Label)e.Row.FindControl("lbl_invNo")).Text = def.InvoiceNo;
                ((Label)e.Row.FindControl("lbl_invDate")).Text = DateTimeUtility.getDateString(def.InvoiceDate);
                ((Label)e.Row.FindControl("lbl_SupplierInvoiceNo")).Text = def.SupplierInvoiceNo;
                ((Label)e.Row.FindControl("lbl_Currency")).Text = def.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_NetAmt")).Text = def.NetAmount.ToString("#,##0.00");
                ((Label)e.Row.FindControl("lbl_SplitCount")).Text = def.SplitCount.ToString();
                ((Label)e.Row.FindControl("lbl_ShippedQty")).Text = def.TotalShippedQty.ToString();
                ((Label)e.Row.FindControl("lbl_Customer")).Text = def.CustomerCode;
                ((Label)e.Row.FindControl("lbl_QACommissionAmt")).Text = def.QACommissionAmount.ToString("#,##0.00") + " (" + def.QACommissionPercent.ToString("0") + "%)";
                ((Label)e.Row.FindControl("lbl_VendorPaymentDiscountAmt")).Text = def.VendorPaymentDiscountAmount.ToString("#,##0.00") + " (" + def.VendorPaymentDiscountPercent.ToString("0") + "%)";
                ((Label)e.Row.FindControl("lbl_LabTestIncome")).Text = def.LabTestIncome.ToString("#,##0.00");

                if (def.SupplierInvoiceNo.Trim() == String.Empty || def.IsUploadDMSDocument == false || def.ShippingDocReceiptDate == DateTime.MinValue || !ShipmentManager.Instance.isReadyForDMS(def.ShipmentId))
                {
                    ((CheckBox)e.Row.FindControl("ckb_inv")).Enabled = false;
                }

                if (def.ShippingDocCheckedBy != null)
                {
                    ((Label)e.Row.FindControl("lbl_Status")).Text = def.ShippingDocCheckedBy.DisplayName + " @" + DateTimeUtility.getDateString(def.ShippingDocCheckedDate);
                    ((CheckBox)e.Row.FindControl("ckb_inv")).Enabled = false;
                }


                //if (def.ShippingDocReceiptDate != DateTime.MinValue)
                if (def.IsUploadDMSDocument)
                {
                    ((ImageButton)e.Row.FindControl("imgOpen")).CommandArgument = def.ContractNo;
                    ((ImageButton)e.Row.FindControl("imgOpen")).Attributes.Add("onclick", "openAttachments(this, '" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(def.ShipmentId.ToString())) + "');return false;");
                }
                else
                {
                    //((CheckBox)e.Row.FindControl("ckb_inv")).Enabled = false;
                    ((ImageButton)e.Row.FindControl("imgOpen")).Visible = false;
                }
            }
        }

        #region Button Event

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.GetShipmentToDMSList);

            if (txt_DocReceiptDateFrom.Text.Trim() != String.Empty)
            {
                Context.Items.Add(ShipmentCommander.Param.shippingDocReceiptDateFrom, Convert.ToDateTime(txt_DocReceiptDateFrom.Text.Trim()));
                if (txt_DocReceiptDateTo.Text.Trim() == String.Empty)
                    txt_DocReceiptDateTo.Text = txt_DocReceiptDateFrom.Text;
                Context.Items.Add(ShipmentCommander.Param.shippingDocReceiptDateTo, Convert.ToDateTime(txt_DocReceiptDateTo.Text.Trim()));
            }
            else
            {
                Context.Items.Add(ShipmentCommander.Param.shippingDocReceiptDateFrom, DateTime.MinValue);
                Context.Items.Add(ShipmentCommander.Param.shippingDocReceiptDateTo, DateTime.MinValue);
            }

            Context.Items.Add(ShipmentCommander.Param.officeId, ddl_Office.SelectedValue);
            Context.Items.Add(ShipmentCommander.Param.paymentTermId, ddl_PaymentTerm.SelectedValue);
            Context.Items.Add(ShipmentCommander.Param.vendorId, this.txt_SupplierName.VendorId == int.MinValue ? -1 : this.txt_SupplierName.VendorId);
            Context.Items.Add(ShipmentCommander.Param.contractNo, this.txt_ContractNo.Text.Trim());
            Context.Items.Add(ShipmentCommander.Param.itemNo, this.txt_ItemNo.Text.Trim());

            int checkStatus = 0;
            if (this.radChecked.Checked) checkStatus = 1;
            Context.Items.Add(ShipmentCommander.Param.shippingDocCheckStatus, checkStatus);

            forwardToScreen(null);

            ArrayList list = (ArrayList)Context.Items[ShipmentCommander.Param.shipmentToDMSList];

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
            ddl_PaymentTerm.SelectedIndex = -1;
            this.txt_SupplierName.clear();
            this.txt_DocReceiptDateFrom.Text = String.Empty;
            this.txt_DocReceiptDateTo.Text = String.Empty;
            this.txt_ContractNo.Text = String.Empty;
            this.txt_ItemNo.Text = String.Empty;
            this.radUnchecked.Checked = true;

            vwSearchResult = null;
            lbl_Warning.Visible = false;
            pnl_Result.Visible = false;
        }

        #endregion

        protected void gv_Inv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
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

        private void updateShippingDocumentCheckStatus()
        {
            ArrayList selectedList = new ArrayList();

            foreach (GridViewRow row in gv_Inv.Rows)
            {
                CheckBox ckb = (CheckBox)row.Cells[0].FindControl("ckb_inv");
                if (ckb.Checked)
                {
                    ShipmentToDMSDef def = (ShipmentToDMSDef)vwSearchResult[row.RowIndex];

                    selectedList.Add(def.ShipmentId.ToString());
                }
            }

            if (selectedList.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "inv", "alert('No record(s) were selected.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.UpdateShippingDocumentCheckStatus);

            Context.Items.Add(ShipmentCommander.Param.shipmentIdList, selectedList);

            forwardToScreen(null);

            btn_Search_Click(null, null);
        }


        protected void btn_Accept_Click(object sender, EventArgs e)
        {
            this.updateShippingDocumentCheckStatus();
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

            ShipmentToDMSDef.ShipmentToDMSComparer.CompareType compareType;

            if (sortExpression == "ContractNo")
                compareType = ShipmentToDMSDef.ShipmentToDMSComparer.CompareType.ContractNo;
            else if (sortExpression == "SupplierInvoiceNo")
                compareType = ShipmentToDMSDef.ShipmentToDMSComparer.CompareType.SupplierInvoiceNo;
            else if (sortExpression == "InvoiceNo")
                compareType = ShipmentToDMSDef.ShipmentToDMSComparer.CompareType.InvoiceNo;
            else if (sortExpression == "SupplierName")
                compareType = ShipmentToDMSDef.ShipmentToDMSComparer.CompareType.SupplierName;
            else if (sortExpression == "NetAmt")
                compareType = ShipmentToDMSDef.ShipmentToDMSComparer.CompareType.NetAmt;
            else if (sortExpression == "InvoiceDate")
                compareType = ShipmentToDMSDef.ShipmentToDMSComparer.CompareType.InvoiceDate;
            else
                compareType = ShipmentToDMSDef.ShipmentToDMSComparer.CompareType.InvoiceNo;

            vwSearchResult.Sort(new ShipmentToDMSDef.ShipmentToDMSComparer(compareType, sortDirection));
            this.gv_Inv.DataSource = vwSearchResult;
            this.gv_Inv.DataBind();
        }
    }
}
