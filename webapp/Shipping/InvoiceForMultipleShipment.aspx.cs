using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.web;
using com.next.infra.smartwebcontrol;
using com.next.common.web.commander;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.shipment;
using com.next.isam.domain.types;
using com.next.isam.domain.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.common;
using com.next.isam.appserver.shipping;
using com.next.isam.dataserver.worker;
using com.next.common.domain.module;
using com.next.common.datafactory.worker;
using com.next.infra.util;

namespace com.next.isam.webapp.shipping
{
    public partial class InvoiceForMultipleShipment : com.next.isam.webapp.usercontrol.PageTemplate
    {
        //bool anyInvoicedShipment;   // is any invoice shipment in the input shipment list
        static bool allowGenerateInvoice;   // allow to generate invoice
        static ArrayList inputShipmentIdList;
        bool hasAccessRights_AssignInvoiceNo;
        bool hasAccessRights_Edit;
        bool hasAccessRights_View;
        bool hasAccessRights_Print;
        bool showColorColumn;
        static string displayMode = ""; // 'MS'     - Mock Shop order (simplified); 
        // Empty    - Normal order;
        string DefaultInvoiceMode = ""; // "SINGLE" - All shipment share a single invoice no.       
        // "MULTI"  - Multiple invoice mode, Each shipment have there own invoice no.
        // Empty    - Auto mode, All non-sample order share a single invoice no. and the sample order generate with individual invoice no.
        //ArrayList allDestinationList;
        static ICollection allDestinationList;

        protected enum QueueType : int { Single, Multi, Single_Lipsy, Single_LipsyUS };
        protected class invoiceQueueNode
        {
            public int seqNo { get; set; }
            public string invNo { get; set; }
            public string invPrefix { get; set; }
            public int invSeqNo { get; set; }
            public int invYear { get; set; }
            public int noOfOrder { get; set; }
        }


        ArrayList vwMultiShipmentInvoiceDetail
        {
            get { return (ArrayList)ViewState["MultiShipmentInvoiceDetail"]; }
            set { ViewState["MultiShipmentInvoiceDetail"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string[] shipmentIdArray;
            string para;


            hasAccessRights_Edit = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.multipleShipmentUpdate.Id, ISAMModule.multipleShipmentUpdate.UpdateShipment);
            hasAccessRights_View = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.multipleShipmentUpdate.Id, ISAMModule.multipleShipmentUpdate.View);
            hasAccessRights_Print = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.multipleShipmentUpdate.Id, ISAMModule.multipleShipmentUpdate.PrintReport);
            hasAccessRights_AssignInvoiceNo = false;      // (this.LogonUserId == 574);

            // Disable access control
            hasAccessRights_View = true;
            hasAccessRights_Edit = true;
            hasAccessRights_Print = true;

            if (!(hasAccessRights_Edit || hasAccessRights_View || hasAccessRights_Print))
            {
                pnl_AccessDenied.Visible = true;
                pnl_MainBlock.Visible = false;
                return;
            }
            btn_Save.Enabled = hasAccessRights_Edit;
            btn_Reset.Enabled = hasAccessRights_Edit;
            btn_Print.Enabled = hasAccessRights_Print;
            if (btn_Save.Enabled)
            {
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.AllowInputZeroQuantity))
                    btn_Save.OnClientClick = "return (allowToSave() ? confirmIfTotalShippedQtyZero(true) : false);";    // get confirm on zero total shipped qty 
                else
                    btn_Save.OnClientClick = "return (allowToSave() ? confirmIfTotalShippedQtyZero(false) : false);";  // block saving if total shipped qty is zero
            }


            if (allDestinationList == null)
                allDestinationList = WebUtil.getFinalDestinationList();

            if (!Page.IsPostBack)
            {
                para = Request.Params["InvoiceMode"];
                if (para == "SINGLE")
                    DefaultInvoiceMode = "SINGLE";
                else
                    if (para == "MULTI")
                        DefaultInvoiceMode = "MULTI";
                    else
                        DefaultInvoiceMode = "";
                if (DefaultInvoiceMode == "MULTI")
                    this.txt_InvoiceNo.Enabled = false;

                inputShipmentIdList = new ArrayList();
                if (!string.IsNullOrEmpty(Request.Params["ShipmentId"]))
                {
                    para = Request.Params["ShipmentId"];
                    shipmentIdArray = para.Split(char.Parse("|"));
                    foreach (string shipmentId in shipmentIdArray)
                        inputShipmentIdList.Add(int.Parse(shipmentId));
                }

                LoadMultiShipmentInvoice(inputShipmentIdList);
                if (inputShipmentIdList.Count == 0 || vwMultiShipmentInvoiceDetail == null)
                {
                    this.btn_Save.Enabled = false;
                    this.btn_Reset.Enabled = false;
                    this.btn_Print.Enabled = false;
                    this.txt_InvoiceNo.Enabled = false;
                    this.txt_InvoiceDate.Enabled = false;
                    if (inputShipmentIdList.Count >= 0)
                        this.lbl_ShipmentNotFound.Text = "This shipment is not ready for invoicing right now.";
                    else
                        this.lbl_ShipmentNotFound.Text = "Shipment cannot be found";
                    this.lbl_ShipmentNotFound.Visible = true;
                }
            }
            else
            {
                if (inputShipmentIdList == null)
                {
                    inputShipmentIdList = new ArrayList();
                    if (!string.IsNullOrEmpty(Request.Params["ShipmentId"]))
                    {
                        para = Request.Params["ShipmentId"];
                        shipmentIdArray = para.Split(char.Parse("|"));
                        foreach (string shipmentId in shipmentIdArray)
                            inputShipmentIdList.Add(int.Parse(shipmentId));
                    }
                    if (inputShipmentIdList.Count == 0)
                    {
                        this.btn_Save.Enabled = false;
                        this.btn_Reset.Enabled = false;
                        this.btn_Print.Enabled = false;
                        this.txt_InvoiceNo.Enabled = false;
                        this.txt_InvoiceDate.Enabled = false;
                    }
                }
            }

        }

        #region DataBinding

        void LoadMultiShipmentInvoice(ArrayList shipmentIdList)
        {
            bool anySampleOrder = false;
            bool anyNonSampleOrder = false;
            bool anySelfBillOrder = false;
            bool anyFobUTOrder = false;
            bool anyInvoicedShipment = false;
            //bool anyDongguanUTurnOrder = false;
            bool allInvoiceDateTheSame = true;
            bool allInvoiceNoTheSame = true;
            string currentInvoiceNo = "";
            string invoiceNo = "";
            DateTime invoiceDate = DateTime.MinValue;
            DateTime lockedInvoiceDate = DateTime.MinValue;
            int shipmentCount = 0;
            int shipmentId = int.MinValue;
            int invoiceNoFound = 0;    // Number of different invoice number found in the list
            int invoiceDateFound = 0;  // Number of different invoice date found in the list
            int lockedInvoiceDateFound = 0;  // Number of different invoice date found for those locked shipment 

            vwMultiShipmentInvoiceDetail = ShipmentManager.Instance.getMultiShipmentInvoiceDetailList(shipmentIdList);
            if (vwMultiShipmentInvoiceDetail == null) return;
            showColorColumn = false;
            foreach (MultiShipmentInvoiceDetailDef msDef in vwMultiShipmentInvoiceDetail)
                if (msDef != null)
                {
                    if (shipmentId != msDef.Shipment.ShipmentId)
                    {
                        shipmentCount++;
                        currentInvoiceNo = msDef.Invoice.InvoiceNo; // ShippingWorker.getInvoiceNo(msDef.Invoice.InvoicePrefix, msDef.Invoice.InvoiceSeqNo, msDef.Invoice.InvoiceYear);
                        showColorColumn = showColorColumn || (msDef.Contract.Customer.CustomerId == 9);     //Show the color column for Lipsy order 
                        anySelfBillOrder = (anySelfBillOrder || msDef.Invoice.IsSelfBilledOrder);
                        anyFobUTOrder = (anyFobUTOrder || msDef.Shipment.TermOfPurchase.TermOfPurchaseId==TermOfPurchaseRef.Id.FOB_UT.GetHashCode());
                        anySampleOrder = anySampleOrder || (msDef.Shipment.IsMockShopSample == 1 || msDef.Shipment.IsStudioSample == 1);
                        anyNonSampleOrder = anyNonSampleOrder || !(msDef.Shipment.IsMockShopSample == 1 || msDef.Shipment.IsStudioSample == 1);
                        anyInvoicedShipment = anyInvoicedShipment || (msDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id);   //(!string.IsNullOrEmpty(msDef.Invoice.InvoicePrefix));
                        //anyDongguanUTurnOrder = anyDongguanUTurnOrder ||
                        //                            (msDef.Contract.Office.OfficeCode == "HK"
                        //                                && ",NCFB,NCFG,NCFM,NCFWC,NCFWF,NCFBL,NCFWB,".Contains("," + msDef.Contract.ProductTeam.Code.Trim() + ",")
                        //                                && CustomerDestinationDef.isUTurnOrder(msDef.Shipment.CustomerDestinationId)
                        //                            );
                        allInvoiceDateTheSame = allInvoiceDateTheSame && (shipmentId == int.MinValue || invoiceDate == msDef.Invoice.InvoiceDate);
                        allInvoiceNoTheSame = allInvoiceNoTheSame && (shipmentId == int.MinValue || invoiceNo == currentInvoiceNo);
                        if (currentInvoiceNo != "" && currentInvoiceNo != invoiceNo)
                        {
                            invoiceNo = currentInvoiceNo;
                            invoiceNoFound++;
                        }
                        if (msDef.Invoice.InvoiceDate != DateTime.MinValue && msDef.Invoice.InvoiceDate != invoiceDate)
                        {
                            invoiceDate = msDef.Invoice.InvoiceDate;
                            invoiceDateFound++;
                        }
                        if (msDef.Shipment.PaymentLock)
                        {
                            if (msDef.Invoice.InvoiceDate != DateTime.MinValue && msDef.Invoice.InvoiceDate != lockedInvoiceDate)
                            {
                                lockedInvoiceDate = msDef.Invoice.InvoiceDate;
                                lockedInvoiceDateFound++;
                            }
                        }
                        shipmentId = msDef.Shipment.ShipmentId;
                    }
                }

            // Initialize Buttons 
            this.btn_Print.Enabled = anyInvoicedShipment && hasAccessRights_Print;
            if (anySampleOrder && !anyNonSampleOrder)
            {   // Mock shop order only
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SetDefaultToZeroShippedQty", "CopyQtyToShippedQty('OrderQty', 'SYSTEM');\n", true);
                displayMode = "MS";
                lbl_InvoiceDate.Text = "Courier Sending Date";
                btn_Save.Text = "Save & Print";
                if (!txt_InvoiceNo.Enabled && txt_InvoiceNo.Text != "")
                    // Invoice has already generated
                    btn_Save.ToolTip = "Save the Detail and Print the Invoice";
                else
                    btn_Save.ToolTip = "Save the Detail, Generate and Print the Invoice";
            }
            else
            {   // Any order other than Mock shop order
                displayMode = "";
                lbl_InvoiceDate.Text = "Invoice Date";
                btn_Save.Text = "Save";
                if (!txt_InvoiceNo.Enabled && txt_InvoiceNo.Text != "")
                    // Invoice has already generated
                    btn_Save.ToolTip = "Save the Detail";
                else
                    btn_Save.ToolTip = "Save the Detail and Generate Invoice";
            }

            // Initialize the content of 'Invoice no.' and 'Invoice date' text box
            if (invoiceNoFound == 1 && allInvoiceNoTheSame)
                // Show 'Invoice Date' and 'Invoice No.' if no conflict
                this.txt_InvoiceNo.Text = invoiceNo;
            else
                this.txt_InvoiceNo.Text = "";

            if (invoiceDateFound == 1 && allInvoiceDateTheSame)
                this.txt_InvoiceDate.Text = DateTimeUtility.getDateString(invoiceDate);
            else
                this.txt_InvoiceDate.Text = string.Empty;

            // Lock/Unlock the input box of 'Invoice Date' and 'Invoice No.'
            txt_InvoiceNo.Attributes.Add("title", "Invoice No. for " + (shipmentCount == 1 ? "this shipment" : "these shipments"));
            txt_InvoiceDate.Attributes.Add("title", "Invoice Date for " + (shipmentCount == 1 ? "this shipment" : "these shipments"));
            txt_InvoiceNo.Style.Add("color", "black");
            txt_InvoiceDate.Style.Add("color", "black");
            allowGenerateInvoice = false;
            if (!anySelfBillOrder && !anyFobUTOrder)  // && !anyDongguanUTurnOrder 
            {
                if (allInvoiceNoTheSame && allInvoiceDateTheSame)
                {
                    if (invoiceNoFound == 0 && invoiceDateFound == 0)
                    {   // No invoiced shipment
                        allowGenerateInvoice = true;
                        txt_InvoiceNo.ReadOnly = false;
                        txt_InvoiceNo.Enabled = true;
                        if (!hasAccessRights_AssignInvoiceNo)
                        {
                            txt_InvoiceNo.ReadOnly = true;
                            txt_InvoiceNo.Enabled = false;
                            txt_InvoiceNo.Attributes.Add("title", "You are not allow to assign Invoice No.");
                        }
                        txt_InvoiceDate.ReadOnly = false;
                        txt_InvoiceDate.Enabled = true;
                        if (!allInvoiceDateTheSame)
                            txt_InvoiceDate.Style.Add("color", "red");
                    }
                    else
                    {   // only one invoice number/date found and there is any non-invoice shipment
                        allowGenerateInvoice = true;
                        txt_InvoiceNo.ReadOnly = true;
                        txt_InvoiceNo.Enabled = false;
                        if (!allInvoiceNoTheSame)
                            txt_InvoiceNo.Style.Add("color", "red");

                        txt_InvoiceDate.ReadOnly = (lockedInvoiceDateFound >= 1);
                        txt_InvoiceDate.Enabled = !(lockedInvoiceDateFound >= 1);
                        if (!allInvoiceDateTheSame)
                            txt_InvoiceDate.Style.Add("color", "red");
                    }
                }
                else
                {   // More than one invoice found, not allow to generate invoice
                    allowGenerateInvoice = false;
                    txt_InvoiceNo.ReadOnly = true;
                    txt_InvoiceNo.Enabled = false;
                    txt_InvoiceNo.Attributes.Add("title", "Invoice No. Conflict");
                    txt_InvoiceNo.Style.Add("color", "red");

                    txt_InvoiceDate.ReadOnly = true;
                    txt_InvoiceDate.Enabled = false;
                    if (!allInvoiceDateTheSame)
                    {
                        txt_InvoiceDate.Style.Add("color", "red");
                        //if (invoiceDateFound > 1)
                        txt_InvoiceDate.Attributes.Add("title", "Invoice Date conflict");
                    }
                }
            }
            else
            {   // Self Bill order exist, not allow to generate invoice
                allowGenerateInvoice = false;
                txt_InvoiceNo.ReadOnly = true;
                txt_InvoiceNo.Enabled = false;
                if (anySelfBillOrder)
                    txt_InvoiceNo.Attributes.Add("title", "Not Allow to Generate Invoice for Self Bill Order");
                else if (anyFobUTOrder)
                    txt_InvoiceNo.Attributes.Add("title", "Not Allow to Generate Invoice for FOB (UT) Order");
                txt_InvoiceNo.Style.Add("color", "red");

                txt_InvoiceDate.ReadOnly = true;
                txt_InvoiceDate.Enabled = false;
                if (anySelfBillOrder)
                    txt_InvoiceDate.Attributes.Add("title", "Not Allow to Input Invoice Date for Self Bill Order");
                else if (anyFobUTOrder)
                    txt_InvoiceDate.Attributes.Add("title", "Not Allow to Input Invoice Date for FOB (UT) Order");
                txt_InvoiceDate.Style.Add("color", "red");
            }

            BindMulitShipmentInvoice();

            return;
        }


        void BindMulitShipmentInvoice()
        {

            // Bind the data grid
            gv_MultiShipmentInvoice.DataSource = vwMultiShipmentInvoiceDetail;
            gv_MultiShipmentInvoice.DataBind();
            if (vwMultiShipmentInvoiceDetail.Count <= 0)
                gv_MultiShipmentInvoice.EmptyDataText = "#Record : " + vwMultiShipmentInvoiceDetail.Count.ToString();
        }


        protected void gv_MultiShipmentInvoice_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            decimal SellingAmt, SupplierAmt;
            int EditBlockRow;
            MultiShipmentInvoiceDetailDef dtl;
            string NewLine;
            bool Lockeded;
            TextBox txtShippedQty;
            Label lblShippedQty;

            NewLine = "<br/>";

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                dtl = (MultiShipmentInvoiceDetailDef)vwMultiShipmentInvoiceDetail[e.Row.RowIndex];
                if (dtl != null)
                {
                    EditBlockRow = 9;
                    Lockeded = dtl.Shipment.PaymentLock;
                    e.Row.VerticalAlign = VerticalAlign.Top;

                    // Shipment Level display control
                    switch (dtl.LineNo)
                    { // Cell display control
                        case 1:
                            e.Row.Cells[0].Style.Add(HtmlTextWriterStyle.Display, "block"); // Sequence No. coloumn
                            e.Row.Cells[0].RowSpan = dtl.NoOfSize + 1;
                            e.Row.Cells[1].Style.Add(HtmlTextWriterStyle.Display, "block"); // Contract No. coloumn
                            e.Row.Cells[2].Style.Add(HtmlTextWriterStyle.Display, "block"); // Delivery No. coloumn
                            e.Row.Cells[3].Style.Add(HtmlTextWriterStyle.Display, "block"); // Item No. coloumn
                            e.Row.Cells[4].Style.Add(HtmlTextWriterStyle.Display, "block"); // Supplier Name coloumn
                            e.Row.Cells[5].Style.Add(HtmlTextWriterStyle.Display, "block"); // Currency coloumn
                            break;
                        case 2:
                            e.Row.Cells[0].Style.Add(HtmlTextWriterStyle.Display, "none");  // Sequence No. coloumn
                            e.Row.Cells[1].Style.Add(HtmlTextWriterStyle.Display, "block"); // show the Edit block
                            e.Row.Cells[1].Font.Bold = false;
                            e.Row.Cells[1].ColumnSpan = 5;
                            e.Row.Cells[1].RowSpan = dtl.NoOfSize;
                            e.Row.Cells[2].Visible = false;
                            e.Row.Cells[3].Visible = false;
                            e.Row.Cells[4].Visible = false;
                            e.Row.Cells[5].Visible = false;
                            break;
                        default:
                            e.Row.Cells[0].Style.Add(HtmlTextWriterStyle.Display, "none");
                            e.Row.Cells[1].Visible = false;
                            e.Row.Cells[2].Visible = false;
                            e.Row.Cells[3].Visible = false;
                            e.Row.Cells[4].Visible = false;
                            e.Row.Cells[5].Visible = false;
                            break;
                    }
                    // Shipment Detail display control
                    if (dtl.LineNo > dtl.NoOfSize)
                    {
                        e.Row.Font.Bold = true;
                        e.Row.Cells[6].Style.Add(HtmlTextWriterStyle.Display, "none");      // Color column
                        e.Row.Cells[7].Style.Add(HtmlTextWriterStyle.Display, "none");      // Option No column
                        e.Row.Cells[8].Style.Add(HtmlTextWriterStyle.Display, "block");     // Size column
                        e.Row.Cells[8].Style.Add(HtmlTextWriterStyle.VerticalAlign, "top");
                        e.Row.Cells[8].ColumnSpan = (showColorColumn ? 3 : 2) - (displayMode == "MS" ? 1 : 0);
                        e.Row.Cells[8].Text = "Total :";
                        for (int i = 1; i < (EditBlockRow - dtl.NoOfSize) * 2; i++)
                            e.Row.Cells[8].Text += NewLine;
                    }
                    else
                    {
                        // Show the colour column if customer is Lipsy
                        e.Row.Cells[6].Style.Add("display", (showColorColumn ? "block" : "none"));                          // Color column
                        e.Row.Cells[7].Style.Add(HtmlTextWriterStyle.Display, (displayMode == "MS" ? "none" : "block"));    // Option No column
                        e.Row.Cells[8].Style.Add(HtmlTextWriterStyle.Display, "block");                                     // Size column
                    }
                    e.Row.Cells[9].Style.Add(HtmlTextWriterStyle.Display, (displayMode == "MS" ? "none" : "block"));    // Order Qty column
                    e.Row.Cells[10].Style.Add(HtmlTextWriterStyle.Display, (displayMode == "MS" ? "none" : "block"));   // PO Qty No column
                    e.Row.Cells[11].Style.Add(HtmlTextWriterStyle.Display, "block");                                    // Shipped Qty column
                    e.Row.Cells[12].Style.Add(HtmlTextWriterStyle.Display, (displayMode == "MS" ? "none" : "block"));   // Selling Price column
                    e.Row.Cells[13].Style.Add(HtmlTextWriterStyle.Display, (displayMode == "MS" ? "none" : "block"));   // Selling Amount column
                    e.Row.Cells[14].Style.Add(HtmlTextWriterStyle.Display, "block");                                    // Supplier Price column
                    e.Row.Cells[15].Style.Add(HtmlTextWriterStyle.Display, "block");                                    // Supplier Amount column


                    ((Label)e.Row.FindControl("lbl_RowIndex")).Text = e.Row.RowIndex.ToString();
                    ((Label)e.Row.FindControl("lbl_LineNo")).Text = dtl.LineNo.ToString();
                    ((Label)e.Row.FindControl("lbl_SequenceNo")).Text = dtl.SequenceNo.ToString();
                    ((Label)e.Row.FindControl("lbl_Locked")).Text = Lockeded.ToString();

                    // Shipment Info column
                    ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Text = dtl.Contract.ContractNo;
                    ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Attributes.Add("OnClick", "window.open('../Shipping/ShipmentDetail.aspx?ShipmentId=" + dtl.Shipment.ShipmentId.ToString() + "&DefaultReceiptDate=" + HttpUtility.UrlEncode(DateTimeUtility.getDateString(DateTime.Today)) + "', 'ShipmentDetail', 'width=800,height=900,scrollbars=1,resizable=1,status=1'); return false;");
                    ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 1).ToString());
                    ((LinkButton)e.Row.FindControl("lnk_ContractNo")).Style.Add(HtmlTextWriterStyle.Display, (dtl.LineNo == 1 ? "block" : "none"));
                    ((Label)e.Row.FindControl("lbl_DeliveryNo")).Text = dtl.Shipment.DeliveryNo.ToString();
                    ((Label)e.Row.FindControl("lbl_ItemNo")).Text = dtl.Product.ItemNo;
                    ((Label)e.Row.FindControl("lbl_Vendor")).Text = dtl.Shipment.Vendor.Name;
                    ((Label)e.Row.FindControl("lbl_Currency")).Text = dtl.Shipment.SellCurrency.CurrencyCode;

                    // Edit block
                    ((Panel)e.Row.FindControl("pnl_EditBlock")).Visible = (dtl.LineNo == 2 ? true : false);
                    ((TextBox)e.Row.FindControl("txt_SupplierInvNo")).Text = (dtl.Invoice.SupplierInvoiceNo == null ? "" : dtl.Invoice.SupplierInvoiceNo);
                    ((TextBox)e.Row.FindControl("txt_SupplierInvNo")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 2).ToString());
                    ((TextBox)e.Row.FindControl("txt_ActualAwhDate")).Text = DateTimeUtility.getDateString(dtl.Invoice.ActualAtWarehouseDate);
                    ((TextBox)e.Row.FindControl("txt_ActualAwhDate")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 3).ToString());

                    ((TextBox)e.Row.FindControl("txt_ItemColor")).Text = dtl.Invoice.ItemColour;
                    ((TextBox)e.Row.FindControl("txt_ItemColor")).Enabled = (displayMode == "MS" ? false : true);
                    ((TextBox)e.Row.FindControl("txt_ItemColor")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 3).ToString());
                    ((TextBox)e.Row.FindControl("txt_ItemDesc1")).Enabled = (displayMode == "MS" ? false : true);
                    ((TextBox)e.Row.FindControl("txt_ItemDesc1")).Text = dtl.Invoice.ItemDesc1;
                    ((TextBox)e.Row.FindControl("txt_ItemDesc1")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 4).ToString());
                    ((TextBox)e.Row.FindControl("txt_ItemDesc2")).Text = dtl.Invoice.ItemDesc2;
                    ((TextBox)e.Row.FindControl("txt_ItemDesc2")).Enabled = (displayMode == "MS" ? false : true);
                    ((TextBox)e.Row.FindControl("txt_ItemDesc2")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 5).ToString());
                    ((TextBox)e.Row.FindControl("txt_ItemDesc3")).Text = dtl.Invoice.ItemDesc3;
                    ((TextBox)e.Row.FindControl("txt_ItemDesc3")).Enabled = (displayMode == "MS" ? false : true);
                    ((TextBox)e.Row.FindControl("txt_ItemDesc3")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 6).ToString());
                    ((TextBox)e.Row.FindControl("txt_ItemDesc4")).Text = dtl.Invoice.ItemDesc4;
                    ((TextBox)e.Row.FindControl("txt_ItemDesc4")).Enabled = (displayMode == "MS" ? false : true);
                    ((TextBox)e.Row.FindControl("txt_ItemDesc4")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 7).ToString());
                    ((TextBox)e.Row.FindControl("txt_ItemDesc5")).Text = dtl.Invoice.ItemDesc5;
                    ((TextBox)e.Row.FindControl("txt_ItemDesc5")).Enabled = (displayMode == "MS" ? false : true);
                    ((TextBox)e.Row.FindControl("txt_ItemDesc5")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 8).ToString());

                    ((TextBox)e.Row.FindControl("txt_PiecesPerUnit")).Text = dtl.Invoice.PiecesPerDeliveryUnit.ToString();
                    ((TextBox)e.Row.FindControl("txt_PiecesPerUnit")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 9).ToString());
                    if (dtl.Invoice.PackingMethod != null)
                        ((DropDownList)e.Row.FindControl("ddl_PackingUnit")).SelectedValue = dtl.Invoice.PackingMethod.PackingMethodId.ToString();
                    ((DropDownList)e.Row.FindControl("ddl_PackingUnit")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 10).ToString());
                    ((TextBox)e.Row.FindControl("txt_UKProductGroupCode")).Text = (dtl.UKProductGroupCode == null ? "" : dtl.UKProductGroupCode);
                    ((TextBox)e.Row.FindControl("txt_UKProductGroupCode")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 11).ToString());

                    ((Panel)e.Row.FindControl("pnl_labelNoOfPack")).Style.Add(HtmlTextWriterStyle.Display, (displayMode == "MS" ? "none" : "block"));
                    ((Panel)e.Row.FindControl("pnl_textNoOfPack")).Style.Add(HtmlTextWriterStyle.Display, (displayMode == "MS" ? "none" : "block"));
                    ((Panel)e.Row.FindControl("pnl_labelUkProdCode")).Style.Add(HtmlTextWriterStyle.Display, (displayMode == "MS" ? "none" : "block"));
                    ((TextBox)e.Row.FindControl("txt_UKProductGroupCode")).Style.Add(HtmlTextWriterStyle.Display, (displayMode == "MS" ? "none" : "block"));
                    ((TextBox)e.Row.FindControl("txt_UKProductGroupCode")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 12).ToString());

                    ((TextBox)e.Row.FindControl("txt_CourierChargeToNUK")).Text = (dtl.Invoice.CourierChargeToNUK.ToString("N02"));
                    ((TextBox)e.Row.FindControl("txt_CourierChargeToNUK")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 13).ToString());
                    string dept = GeneralWorker.Instance.getProductDepartmentByKey(dtl.Contract.DeptId).Code;
                    if (dtl.Contract.Office.OfficeId == 13 && dept == "NC")   // Non-Clothing order from India - North (ND)
                    {   // allow to edit if payment lock is not set
                        ((TextBox)e.Row.FindControl("txt_CourierChargeToNUK")).ReadOnly = dtl.Shipment.PaymentLock;
                        ((TextBox)e.Row.FindControl("txt_CourierChargeToNUK")).Enabled = (!dtl.Shipment.PaymentLock);
                    }
                    else
                    {   // not allow to edit
                        ((TextBox)e.Row.FindControl("txt_CourierChargeToNUK")).ReadOnly = true;
                        ((TextBox)e.Row.FindControl("txt_CourierChargeToNUK")).Enabled = false;
                    }

                    ((TextBox)e.Row.FindControl("txt_NSComm")).Text = (dtl.Shipment.NSLCommissionPercentage.ToString("N02") + "%");
                    ((TextBox)e.Row.FindControl("txt_NSComm")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 14).ToString());
                    ((TextBox)e.Row.FindControl("txt_NSComm")).ReadOnly = true;
                    ((TextBox)e.Row.FindControl("txt_NSComm")).Enabled = false;

                    if (dtl.Invoice.CustomerDestination == null)
                        ((SmartDropDownList)e.Row.FindControl("ddl_FinalDestination")).bindList(allDestinationList, "DestinationDesc", "CustomerDestinationId", "", "", "");
                    else
                        ((SmartDropDownList)e.Row.FindControl("ddl_FinalDestination")).bindList(allDestinationList, "DestinationDesc", "CustomerDestinationId", dtl.Invoice.CustomerDestination.CustomerDestinationId.ToString(), "", "");
                    ((SmartDropDownList)e.Row.FindControl("ddl_FinalDestination")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 16).ToString());
                    ((TextBox)e.Row.FindControl("txt_FinalDestination")).Text = ((SmartDropDownList)e.Row.FindControl("ddl_FinalDestination")).selectedText;
                    ((TextBox)e.Row.FindControl("txt_FinalDestination")).Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 15).ToString());

                    // Shipment Detail info column
                    if (dtl.ShipmentDetail != null)
                    {
                        ((Label)e.Row.FindControl("lbl_OrderQty")).Text = dtl.ShipmentDetail.OrderQuantity.ToString("#,##0");
                        ((Label)e.Row.FindControl("lbl_POQty")).Text = dtl.ShipmentDetail.POQuantity.ToString("#,##0");
                        txtShippedQty = (TextBox)e.Row.FindControl("txt_ShippedQty");
                        txtShippedQty.ReadOnly = (Lockeded || dtl.LineNo > dtl.NoOfSize);
                        txtShippedQty.Enabled = !(Lockeded || dtl.LineNo > dtl.NoOfSize);
                        txtShippedQty.Text = dtl.ShipmentDetail.ShippedQuantity.ToString();
                        txtShippedQty.Attributes.Add("TabIndex", (dtl.SequenceNo * 100 + 20 + dtl.LineNo).ToString());
                        txtShippedQty.Attributes.Add("OrderQty", dtl.ShipmentDetail.OrderQuantity.ToString());
                        txtShippedQty.Attributes.Add("POQty", dtl.ShipmentDetail.POQuantity.ToString());
                        txtShippedQty.Attributes.Add("SequenceNo", dtl.SequenceNo.ToString());
                        txtShippedQty.Attributes.Add("Locked", Lockeded.ToString());
                        txtShippedQty.Attributes.Add("Invoiced", (dtl.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id).ToString());
                        txtShippedQty.Attributes.Add("InitialShippedQty", txtShippedQty.Text);
                        txtShippedQty.Attributes.Add("InitialTotalShippedQty", dtl.Shipment.TotalShippedQuantity.ToString());

                        SellingAmt = dtl.ShipmentDetail.SellingPrice * dtl.ShipmentDetail.ShippedQuantity;
                        SupplierAmt = dtl.ShipmentDetail.SupplierGarmentPrice * dtl.ShipmentDetail.ShippedQuantity;
                        if (dtl.LineNo <= dtl.NoOfSize)
                        {
                            ((Label)e.Row.FindControl("lbl_Color")).Text = dtl.ShipmentDetail.Colour;
                            ((Label)e.Row.FindControl("lbl_Option")).Text = dtl.ShipmentDetail.SizeOption.SizeOptionNo.ToString();
                            ((Label)e.Row.FindControl("lbl_Size")).Text = dtl.ShipmentDetail.SizeOption.SizeDescription;
                            ((Label)e.Row.FindControl("lbl_SellingPrice")).Text = dtl.ShipmentDetail.SellingPrice.ToString("#,##0.00");
                            ((Label)e.Row.FindControl("lbl_SupplierPrice")).Text = dtl.ShipmentDetail.SupplierGarmentPrice.ToString("#,##0.00");
                            txtShippedQty.Attributes.Add("SellingPrice", dtl.ShipmentDetail.SellingPrice.ToString());
                            txtShippedQty.Attributes.Add("SupplierPrice", dtl.ShipmentDetail.SupplierGarmentPrice.ToString());
                            txtShippedQty.Attributes.Add("SubTotalRow", "false");
                        }
                        else
                        {   // Shipment Total row
                            SellingAmt = dtl.TotalSellingAmount;
                            SupplierAmt = dtl.TotalSupplierAmount;
                            txtShippedQty.Attributes.Add("SellingPrice", "");
                            txtShippedQty.Attributes.Add("SupplierPrice", "");
                            txtShippedQty.Attributes.Add("SubTotalRow", "true");
                            txtShippedQty.Enabled = false;
                            txtShippedQty.Style.Add("display", "none");
                            lblShippedQty = (Label)e.Row.FindControl("lbl_ShippedQty");
                            lblShippedQty.Text = dtl.ShipmentDetail.ShippedQuantity.ToString("#,##0");
                            lblShippedQty.Style.Add("display", "block");
                        }
                        ((Label)e.Row.FindControl("lbl_SellingAmt")).Text = SellingAmt.ToString("#,##0.00");
                        ((Label)e.Row.FindControl("lbl_SupplierAmt")).Text = SupplierAmt.ToString("#,##0.00");
                    }
                }
                else
                {   //Blank line Control
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                        e.Row.Cells[i].Visible = false;
                    if (e.Row.RowIndex > 0 && (e.Row.RowIndex + 1) < vwMultiShipmentInvoiceDetail.Count)
                    {
                        if (vwMultiShipmentInvoiceDetail[e.Row.RowIndex - 1] == null)
                        {   // The preceding line is a blank line, draw a line
                            e.Row.Style.Value = "border-style:solid;border-color:black;border-width:thin;";   // Show the border 
                        }
                        // The last and second last row
                        if (vwMultiShipmentInvoiceDetail[e.Row.RowIndex + 1] == null)
                        {   //The sucessive line is also a blank line
                            e.Row.Style.Add(HtmlTextWriterStyle.Display, "none");
                        }
                    }
                }
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[6].Style.Add("display", (showColorColumn ? "block" : "none"));        // Color column
                e.Row.Cells[7].Style.Add("display", (displayMode == "MS" ? "none" : "block"));    // Option No column
                e.Row.Cells[8].Style.Add("display", "block");                                     // Size column
                e.Row.Cells[9].Style.Add("display", (displayMode == "MS" ? "none" : "block"));    // Order Qty column
                e.Row.Cells[10].Style.Add("display", (displayMode == "MS" ? "none" : "block"));   // PO Qty No column
                e.Row.Cells[11].Style.Add("display", "block");                                    // Shipped Qty column
                e.Row.Cells[12].Style.Add("display", (displayMode == "MS" ? "none" : "block"));   // Selling Price column
                e.Row.Cells[13].Style.Add("display", (displayMode == "MS" ? "none" : "block"));   // Selling Amount column
                e.Row.Cells[14].Style.Add("display", "block");                                    // Supplier Price column
                e.Row.Cells[15].Style.Add("display", "block");                                    // Supplier Amount column
            }
        }

        #endregion



        #region UserAction

        protected QueueType assignInvoiceQueue(MultiShipmentInvoiceDetailDef def)
        {
            QueueType queue;

            queue = QueueType.Single;    // All shipment in each queue shares a single invoice no.
            if (DefaultInvoiceMode == "MULTI")
                queue = QueueType.Multi;    // Each shipment have it's own invoice no.
            else
                if (DefaultInvoiceMode == "")
                {
                    if (def.Shipment.IsMockShopSample == 1 || def.Shipment.IsStudioSample == 1)
                        queue = QueueType.Multi;
                    else
                        if (def.Contract.Customer.CustomerType.CustomerTypeId == (int)CustomerTypeRef.Type.lipsy)
                        {
                            queue = QueueType.Single_Lipsy;    // General Lipsy order 
                            if (def.Invoice.CustomerDestination != null)
                                if (def.Invoice.CustomerDestination.CustomerDestinationId == 16)    //USA Warehouse
                                    queue = QueueType.Single_LipsyUS;    // Lipsy order that will ship to 'USA warehouse'
                        }
                        else
                            queue = QueueType.Single;
                }
            return queue;
        }


        protected void btn_Save_Click(object sender, EventArgs e)
        {
            int i;
            GridViewRow row;
            MultiShipmentInvoiceDetailDef def;
            ArrayList invoiceList = new ArrayList();
            ArrayList shipmentDetails = new ArrayList();
            string[] inputInvoiceNo = null;

            DateTime invoiceDate;
            DateTime tempDate;
            string ukProdGroupCode, qtyString;
            int commaPos;
            bool anySampleOrder = false;
            bool anyZeroQtyAlert = false;
            string sAlertMsg = "";
            string sInvoiceAlertMsg = "";
            int noOfInvoice = 0;
            int lastShipmentId;
            string inputInvoiceDate = "";
            int TotalShippedQty = 0;
            invoiceQueueNode[] invoiceQueue;
            invoiceQueueNode q;

            // Initialize the invoice queue
            invoiceQueue = new invoiceQueueNode[4];
            QueueType queue;
            if (txt_InvoiceNo.Text.Trim() != "")
                inputInvoiceNo = txt_InvoiceNo.Text.Split(char.Parse("/"));
            for (i = 0; i < 4; i++)
            {
                invoiceQueue[i] = new invoiceQueueNode();
                invoiceQueue[i].invNo = "";
                invoiceQueue[i].seqNo = int.MinValue;
                invoiceQueue[i].noOfOrder = 0;
                if (inputInvoiceNo == null)
                {
                    invoiceQueue[i].invPrefix = null;
                    invoiceQueue[i].invSeqNo = 0;
                    invoiceQueue[i].invYear = 0;
                }
                else
                {
                    invoiceQueue[i].invPrefix = inputInvoiceNo[0];
                    invoiceQueue[i].invSeqNo = int.Parse(inputInvoiceNo[1]);
                    invoiceQueue[i].invYear = int.Parse(inputInvoiceNo[2]);
                }
            }

            DateTime.TryParse(txt_InvoiceDate.Text, out invoiceDate);
            if (txt_InvoiceDate.Enabled)
            {
                inputInvoiceDate = txt_InvoiceDate.Text;
                if (displayMode == "MS" && txt_InvoiceDate.Text != "")
                {
                    int diff = DateTime.Today.Subtract(invoiceDate.Date).Days;
                    if (!(0 <= diff && diff <= 100))
                    {
                        sInvoiceAlertMsg = lbl_InvoiceDate.Text + " out of range. It should be within the range of past 3 months.\\nInvoice will not be generated";
                        allowGenerateInvoice = false;
                    }
                    else
                    {
                        bool isMissingSupInv = false;
                        foreach (GridViewRow r in gv_MultiShipmentInvoice.Rows)
                            if (r != null)
                            {
                                Label ln;
                                if ((ln = ((Label)r.FindControl("lbl_LineNo"))) != null)
                                    if (ln.Text == "2")
                                        if (isMissingSupInv = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_SupplierInvNo")).Text.Trim()))
                                            break;
                            }
                        if (isMissingSupInv)
                        {
                            sInvoiceAlertMsg = "Missing Supplier Invoice No.. \\nInvoice will not be generated";
                            allowGenerateInvoice = false;
                        }
                    }
                }
            }

            for (i = 0, lastShipmentId = -1; i < gv_MultiShipmentInvoice.Rows.Count; i++)
            {
                // Update each shipment detail to def with the user input data
                def = (MultiShipmentInvoiceDetailDef)vwMultiShipmentInvoiceDetail[i];
                if (def != null)
                    if (def.ShipmentDetail != null)
                    {
                        if (def.ShipmentDetail.ShipmentId > 0 && lastShipmentId != def.ShipmentDetail.ShipmentId)
                        {
                            lastShipmentId = def.ShipmentDetail.ShipmentId;
                            TotalShippedQty = 0;
                        }
                        row = gv_MultiShipmentInvoice.Rows[i];
                        if (def.LineNo == 2)
                        {   // Update content in Edit block
                            def.Invoice.SupplierInvoiceNo = ((TextBox)row.FindControl("txt_SupplierInvNo")).Text.Trim();
                            DateTime.TryParse(((TextBox)row.FindControl("txt_ActualAwhDate")).Text, out tempDate);
                            def.Invoice.ActualAtWarehouseDate = tempDate;
                            def.Invoice.ItemColour = ((TextBox)row.FindControl("txt_ItemColor")).Text.Trim();
                            def.Invoice.ItemDesc1 = ((TextBox)row.FindControl("txt_ItemDesc1")).Text.Trim();
                            def.Invoice.ItemDesc2 = ((TextBox)row.FindControl("txt_ItemDesc2")).Text.Trim();
                            def.Invoice.ItemDesc3 = ((TextBox)row.FindControl("txt_ItemDesc3")).Text.Trim();
                            def.Invoice.ItemDesc4 = ((TextBox)row.FindControl("txt_ItemDesc4")).Text.Trim();
                            def.Invoice.ItemDesc5 = ((TextBox)row.FindControl("txt_ItemDesc5")).Text.Trim();
                            if (((TextBox)row.FindControl("txt_PiecesPerUnit")).Text == "")
                                def.Invoice.PiecesPerDeliveryUnit = 0;
                            else
                                def.Invoice.PiecesPerDeliveryUnit = int.Parse(((TextBox)row.FindControl("txt_PiecesPerUnit")).Text);
                            def.Invoice.PackingMethod.PackingMethodId = int.Parse(((DropDownList)row.FindControl("ddl_PackingUnit")).SelectedValue);
                            if (((TextBox)row.FindControl("txt_CourierChargeToNUK")).Text == "")
                                def.Invoice.CourierChargeToNUK = 0;
                            else
                                def.Invoice.CourierChargeToNUK = decimal.Parse(((TextBox)row.FindControl("txt_CourierChargeToNUK")).Text);
                            ukProdGroupCode = ((TextBox)row.FindControl("txt_UKProductGroupCode")).Text.Trim();
                            def.Contract.UKProductGroupCode = (ukProdGroupCode.Length <= 2 ? ukProdGroupCode : ukProdGroupCode.Substring(0, 2));

                            if (((SmartDropDownList)row.FindControl("ddl_FinalDestination")).SelectedValue == "")
                            {
                                if (def.Invoice.CustomerDestination != null)
                                    def.Invoice.CustomerDestination.CustomerDestinationId = int.MinValue;
                            }
                            else
                                if (def.Invoice.CustomerDestination == null)
                                    def.Invoice.CustomerDestination = CommonWorker.Instance.getCustomerDestinationByKey(Int32.Parse(((SmartDropDownList)row.FindControl("ddl_FinalDestination")).SelectedValue));
                                else
                                    def.Invoice.CustomerDestination.CustomerDestinationId = Int32.Parse(((SmartDropDownList)row.FindControl("ddl_FinalDestination")).SelectedValue);
                        }
                        if (def.LineNo < def.NoOfLine)
                        {   // Shipment Detail row - update detail shipped qty
                            qtyString = ((TextBox)row.FindControl("txt_ShippedQty")).Text;
                            while ((commaPos = qtyString.IndexOf(",")) >= 0)
                                qtyString = qtyString.Substring(0, commaPos + 1) + qtyString.Substring(commaPos + 1, qtyString.Length - commaPos + 1);
                            def.ShipmentDetail.ShippedQuantity = int.Parse(qtyString);
                            TotalShippedQty += int.Parse(qtyString);
                        }
                        else
                        {   // Subtotal row
                            if (TotalShippedQty == 0)
                            {
                                allowGenerateInvoice = false;
                                if ((txt_InvoiceDate.Text != "" || def.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id) && !def.Shipment.PaymentLock)
                                    anyZeroQtyAlert = true;
                                if (txt_InvoiceDate.Text != "" && def.Shipment.WorkflowStatus.Id != ContractWFS.INVOICED.Id)
                                    sAlertMsg = "Invoice will not be generated";
                            }
                            if (displayMode != "MS" && txt_InvoiceDate.Enabled)
                                if (Math.Abs(invoiceDate.Subtract(def.Shipment.CustomerAgreedAtWarehouseDate).Days) > 100)
                                {
                                    sInvoiceAlertMsg = lbl_InvoiceDate.Text + " out of range. It should be within the range of ± 3 months of customer at warehouse date.\\nInvoice will not be generated";
                                    allowGenerateInvoice = false;
                                }
                            // count the number or order in each invoice queue
                            queue = assignInvoiceQueue(def);
                            q = invoiceQueue[(int)queue];
                            q.noOfOrder = (q.noOfOrder == int.MinValue ? 1 : q.noOfOrder + 1);
                        }
                        anySampleOrder = (anySampleOrder || (def.Shipment.IsMockShopSample == 1 || def.Shipment.IsStudioSample == 1));
                    }
            }

            InvoiceDef _invoice = null;
            ContractDef _contract = null;

            for (i = 0; i < vwMultiShipmentInvoiceDetail.Count; i++)
            {
                // Save the detail in def to database
                if ((def = (MultiShipmentInvoiceDetailDef)vwMultiShipmentInvoiceDetail[i]) != null)
                {
                    if (def.LineNo == 1)
                    {   // clear invoice and shipment 
                        _invoice = null;
                        _contract = null;
                        shipmentDetails.Clear();
                    }
                    if (def.LineNo == 2)
                    {
                        _invoice = def.Invoice;
                        _contract = def.Contract;
                    }
                    if (def.LineNo < def.NoOfLine)
                    {   // build the shipment detail array
                        shipmentDetails.Add(def.ShipmentDetail);
                    }
                    else
                        if (def.LineNo == def.NoOfLine)
                        {   // summary row : Save shipment detail / generate invoice
                            queue = assignInvoiceQueue(def);
                            q = invoiceQueue[(int)queue];
                            if (def.Shipment.WorkflowStatus.Id != ContractWFS.INVOICED.Id && !def.Invoice.IsSelfBilledOrder)
                            {
                                if (allowGenerateInvoice && invoiceDate != DateTime.MinValue)
                                {   // not more than one locked invoice in the shipment list -> no conflict in invoice no.
                                    _invoice.InvoiceDate = invoiceDate;
                                    _invoice.InvoicePrefix = q.invPrefix;
                                    _invoice.InvoiceSeqNo = q.invSeqNo;
                                    _invoice.InvoiceYear = q.invYear;
                                    if (queue == QueueType.Multi || q.noOfOrder == 1)
                                        _invoice.SequenceNo = int.MinValue; // Individual invoice no. for each order in the queue-> Sequence no. is always null
                                    else
                                        _invoice.SequenceNo = (q.seqNo == int.MinValue ? 1 : q.seqNo + 1);  // Single invoice no. for all the order in the queue
                                }
                            }
                            // Save Invoice and Shipment Detail Record 
                            Context.Items.Clear();
                            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.UpdateInvoiceForMultipleShipment);
                            Context.Items.Add(ShipmentCommander.Param.invoiceDef, _invoice);
                            Context.Items.Add(ShipmentCommander.Param.contractDef, _contract);
                            Context.Items.Add(ShipmentCommander.Param.documents, null);
                            Context.Items.Add(ShipmentCommander.Param.shipmentDetails, shipmentDetails);
                            forwardToScreen(null);

                            if (def.Shipment.WorkflowStatus.Id != ContractWFS.INVOICED.Id && !def.Invoice.IsSelfBilledOrder)
                                if (queue != QueueType.Multi && !string.IsNullOrEmpty(_invoice.InvoicePrefix))
                                {   // The first invoice of the shipment(s) in the same queue
                                    q.invPrefix = _invoice.InvoicePrefix;
                                    q.invSeqNo = _invoice.InvoiceSeqNo;
                                    q.invYear = _invoice.InvoiceYear;
                                    q.invNo = _invoice.InvoiceNo;
                                    q.seqNo = _invoice.SequenceNo;
                                }
                            if (!string.IsNullOrEmpty(_invoice.InvoicePrefix))
                                noOfInvoice++;
                        }
                }
            }

            LoadMultiShipmentInvoice(inputShipmentIdList);
            //if (!allowGenerateInvoice && inputInvoiceDate != "")
            if (txt_InvoiceDate.Enabled && inputInvoiceDate != "")
                txt_InvoiceDate.Text = inputInvoiceDate;

            if (sInvoiceAlertMsg != "")
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertOfInvoiceDateRange", "alert('" + sInvoiceAlertMsg + "');document.all.txt_InvoiceDate.select();\n", true);
            //else  // Check in client side
            //    if (anyZeroQtyAlert)
            //    {   
            //        sAlertMsg = "Total Shipped Quantity is zero. Please confirm your data input." + (sAlertMsg == "" ? "" : "\\n" + sAlertMsg);
            //        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertOfZeroShippedQty", "alert('" + sAlertMsg + "');\n", true);
            //    }
            if (displayMode == "MS" && hasAccessRights_Print)
            {   // Print Mock Shop Invoice in Client side 
                if (noOfInvoice > 0)
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "PrintInvoiceInClientSide", "document.all.btn_Print.click();\n", true);
            }
        }


        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            LoadMultiShipmentInvoice(inputShipmentIdList);
            return;
        }


        protected void btn_Print_Click(object sender, EventArgs e)
        {
            printInvoice();
            return;
        }


        protected void printInvoice()
        {
            if (hasAccessRights_Print)
            {
                ArrayList shipmentIdList = new ArrayList();
                foreach (MultiShipmentInvoiceDetailDef def in vwMultiShipmentInvoiceDetail)
                {
                    if (def != null)
                        if (!shipmentIdList.Contains(def.Shipment.ShipmentId) && !string.IsNullOrEmpty(def.Invoice.InvoicePrefix))
                            shipmentIdList.Add(def.Shipment.ShipmentId);
                }
                if (shipmentIdList.Count > 0)
                {
                    Context.Items.Clear();
                    Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                    Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.PrintInvoice);
                    Context.Items.Add(ShipmentCommander.Param.shipmentList, shipmentIdList);
                    forwardToScreen("InvoiceForMultipleShipment.aspx");
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Notice", "alert('Print Completed';\n", true);
                }
            }
            return;
        }

        #endregion

    }
}
