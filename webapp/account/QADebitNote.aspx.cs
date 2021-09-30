using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.web;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.order;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.shipment;
using com.next.isam.webapp.commander.account;

namespace com.next.isam.webapp.account
{
    public partial class QADebitNote : com.next.isam.webapp.usercontrol.PageTemplate
    {
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
                initControls();
            }
        }

        void initControls()
        {
            ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
            this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());

            txt_Supplier.setWidth(305);
            txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);
            
        }

        protected ArrayList getSplitShipment(ArrayList list)
        {
            ArrayList shipmentList = new ArrayList();
            foreach (ContractShipmentListJDef contractShipmentDef in list)
            {
                if (contractShipmentDef.SplitCount > 0)
                {
                    ArrayList splitShipmentList = WebUtil.getSplitShipmentByShipmentId(contractShipmentDef.ShipmentId);

                    foreach (SplitShipmentDef split in splitShipmentList)
                    {
                        if (split.IsVirtualSetSplit == 1)
                            continue;

                        ContractShipmentListJDef def = (ContractShipmentListJDef)contractShipmentDef.Clone();
                        def.InvoiceNo = contractShipmentDef.InvoiceNo + split.SplitSuffix;
                        def.Vendor = split.Vendor;
                        def.BuyCurrency = split.BuyCurrency;
                        def.TotalShippedSupplierGarmentAmountAfterDiscount = split.TotalShippedSupplierGarmentAmountAfterDiscount;
                        def.QACommissionPercent = split.QACommissionPercent;
                        def.TotalShippedQuantity = split.TotalShippedQuantity;
                        def.ItemNo = split.Product.ItemNo;
                        def.SupplierInvoiceNo = split.SupplierInvoiceNo;
                        

                        shipmentList.Add(def);
                    }
                }
                else
                {
                    shipmentList.Add(contractShipmentDef);
                }
            }

            return shipmentList;
        }

        protected void gv_Invoice_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContractShipmentListJDef contractShipmentDef = (ContractShipmentListJDef)vwSearchResult[e.Row.RowIndex];

                Label lbl = (Label) e.Row.Cells[4].FindControl("lbl_Amount");
                lbl.Text = Math.Round(contractShipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount * contractShipmentDef.QACommissionPercent / 100, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

            }
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string invoicePrefix = "";
            int invoiceSeqFrom = -1;
            int invoiceSeqTo = -1;
            int invoiceYear = -1;
            int officeId = int.Parse(ddl_Office.SelectedValue);
            int vendorId = -1;

            if (txtNSLInvoiceNoFrom.Text.Trim() != "" && txtNSLInvoiceNoTo.Text.Trim() != "")
            {
                invoicePrefix = WebUtil.getInvoicePrefix(txtNSLInvoiceNoFrom.Text.Trim());
                invoiceSeqFrom = WebUtil.getInvoiceSeq(txtNSLInvoiceNoFrom.Text.Trim());
                invoiceSeqTo = WebUtil.getInvoiceSeq(txtNSLInvoiceNoTo.Text.Trim());
                invoiceYear = WebUtil.getInvoiceYear(txtNSLInvoiceNoFrom.Text.Trim());
            }

            
            if (txt_Supplier.VendorId == int.MinValue)
                vendorId = -1;
            else
                vendorId = txt_Supplier.VendorId;


            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.GetShipmentListForAdvancedSearch);

            Context.Items.Add(ShipmentCommander.Param.invoicePrefix, invoicePrefix);
            Context.Items.Add(ShipmentCommander.Param.invoiceSeqFrom, invoiceSeqFrom);
            Context.Items.Add(ShipmentCommander.Param.invoiceSeqTo, invoiceSeqTo);
            Context.Items.Add(ShipmentCommander.Param.invoiceYear, invoiceYear);
            Context.Items.Add(ShipmentCommander.Param.workflowStatusId, 8);
            
            if (officeId != -1)
            {
                TypeCollector officeList = TypeCollector.Inclusive;
                officeList.append(officeId);

                Context.Items.Add(ShipmentCommander.Param.officeList, officeList);
            }
                
            Context.Items.Add(ShipmentCommander.Param.vendorId, vendorId);

            if (txt_InvoiceDateFrom.Text.Trim() != "")
            {
                Context.Items.Add(ShipmentCommander.Param.invoiceDateFrom, txt_InvoiceDateFrom.Text);
                Context.Items.Add(ShipmentCommander.Param.invoiceDateTo, txt_InvoiceDateTo.Text);
            }

            string orderType = "FV";    // FOB & VM
            if (rbUTOnly.Checked)
                orderType = "X";        // FOB (UT)
            Context.Items.Add(ShipmentCommander.Param.orderType, orderType);

            forwardToScreen(null);

            ArrayList list = (ArrayList)Context.Items[ShipmentCommander.Param.shipmentList];
            this.vwSearchResult = getSplitShipment(list);

            if (vwSearchResult.Count >= 100)
                lbl_Msg.Text = "There are more than 100 shipment matching your search criteria.<br />Only the first 100 search result are shown. ";
            else
                lbl_Msg.Text = "Total " + vwSearchResult.Count.ToString() + " records.";

            gv_Invoice.DataSource = this.vwSearchResult;
            gv_Invoice.DataBind();

            pnl_Result.Visible = true;
        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            txt_InvoiceDateFrom.Text = "";
            txt_InvoiceDateTo.Text = "";
            txt_Supplier.clear();
            txtNSLInvoiceNoFrom.Text = "";
            txtNSLInvoiceNoTo.Text = "";
            ddl_Office.SelectedIndex = -1;
            pnl_Result.Visible = false;
        }

        protected void btn_Print_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList invoiceList = new ArrayList();

                foreach (GridViewRow gvr in gv_Invoice.Rows)
                {
                    if (gvr.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox ckb = (CheckBox)gvr.Cells[0].FindControl("ckb_Inv");
                        if (ckb.Checked)
                        {
                            ContractShipmentListJDef def = (ContractShipmentListJDef)vwSearchResult[gvr.RowIndex];
                            invoiceList.Add(def);
                        }
                    }
                }
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.PrintQADebitNote);

                Context.Items.Add(AccountCommander.Param.invoiceList, invoiceList);

                forwardToScreen(null);
            }
            catch
            {
            }
        }
    }
}
