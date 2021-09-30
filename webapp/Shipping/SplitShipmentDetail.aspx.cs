using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.account;
using com.next.isam.appserver.order;
using com.next.isam.domain.account;
using com.next.isam.domain.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.webapp.commander.shipment;
using com.next.infra.web;
using com.next.infra.util;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.module;


namespace com.next.isam.webapp.shipping
{
    public partial class SplitShipmentDetail : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private int splitShipmentId;
        private decimal totalShippedQty;
        private decimal totalSellingAmt;
        private decimal totalSupplierAmt;
        DomainSplitShipmentDef vwDomainSplitShipmentDef
        {
            get { return (DomainSplitShipmentDef)ViewState["DomainSplitShipmentDef"]; }
            set { ViewState["DomainSplitShipmentDef"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.Params["SplitShipmentId"] != null)
                {
                    splitShipmentId = Convert.ToInt32(Request.Params["SplitShipmentId"]);
                }
                else
                {
                    return;
                }

                vwDomainSplitShipmentDef = ShipmentManager.Instance.getDomainSplitShipmentDef(splitShipmentId);
                BindData(vwDomainSplitShipmentDef);
                SetControl(false);
            }

            string defaultReceiptDate = "";
            if (Request.Params["DefaultReceiptDate"] != null)
            {
                defaultReceiptDate = Request.Params["DefaultReceiptDate"].ToString();
            }

            ckb_LCPaymentChecked.Attributes.Add("onclick", "javascript:getCurrentDate(this,'" + txt_LCPaymentCheckedDate.ClientID + "');");
            ckb_ShipRecvDocDate.Attributes.Add("onclick", "javascript:getDefaultDate(this,'" + txt_ShipRecvDocDate.ClientID + "','" + defaultReceiptDate + "');");
            ckb_ShipDocCheck.Attributes.Add("onclick", "javascript:isShipDocCheckValid(this,'ckb_ShipDocCheck');");
        }

        void BindData(DomainSplitShipmentDef domainSplitShipmentDef)
        {
            lbl_ContractNo.Text = domainSplitShipmentDef.Contract.ContractNo + domainSplitShipmentDef.SplitShipment.SplitSuffix;
            lbl_DeliveryNo.Text = domainSplitShipmentDef.Shipment.DeliveryNo.ToString();
            lbl_RequirePayment.Text = domainSplitShipmentDef.SplitShipment.IsVirtualSetSplit == 1 ? "NO" : "YES";
            lbl_Vendor.Text = domainSplitShipmentDef.SplitShipment.Vendor.Name;
            lbl_CO.Text = domainSplitShipmentDef.SplitShipment.CountryOfOrigin.Name;
            lbl_PaymentTerm.Text = domainSplitShipmentDef.SplitShipment.PaymentTerm.PaymentTermDescription;
            lbl_POAtWHDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.SupplierAgreedAtWarehouseDate);
            lbl_piece.Text = domainSplitShipmentDef.SplitShipment.PiecesPerPack.ToString();
            lbl_Pack.Text = domainSplitShipmentDef.SplitShipment.PackingUnit.OPSKey;
            lbl_TotalPOQty.Text = domainSplitShipmentDef.SplitShipment.TotalPOQuantity.ToString();
            lbl_TotalShipQty.Text = domainSplitShipmentDef.SplitShipment.TotalShippedQuantity.ToString();
            lbl_currency.Text = domainSplitShipmentDef.SplitShipment.BuyCurrency.CurrencyCode;
            lbl_TotalPOFOBAmt.Text = domainSplitShipmentDef.SplitShipment.TotalNetFOBAmountAfterDiscount.ToString("#,##0.00");
            lbl_TotalPOCMTAmt.Text = domainSplitShipmentDef.SplitShipment.TotalSupplierGarmentAmountAfterDiscount.ToString("#,##0.00");
            lbl_QuotaCat.Text = domainSplitShipmentDef.SplitShipment.QuotaCategoryGroup.OPSKey;
            lbl_ttlFOBAmt.Text = domainSplitShipmentDef.SplitShipment.TotalShippedNetFOBAmountAfterDiscount.ToString("#,##0.00");
            lbl_ttlShipCMTAmt.Text = domainSplitShipmentDef.SplitShipment.TotalShippedSupplierGarmentAmountAfterDiscount.ToString("#,##0.00");
            lbl_MerToShipNote.Text = domainSplitShipmentDef.SplitShipment.ShippingRemark;

            lbl_Season.Text = domainSplitShipmentDef.Contract.Season.Code;
            lbl_Phase.Text = domainSplitShipmentDef.Contract.PhaseId.ToString();
            lbl_Color.Text = domainSplitShipmentDef.SplitShipment.Colour;
            txt_Color.Text = domainSplitShipmentDef.SplitShipment.Colour;

            if (domainSplitShipmentDef.SplitShipment.Product != null)
            {
                lbl_Description.Text = domainSplitShipmentDef.SplitShipment.Product.ShortDesc;
                lbl_Desc1.Text = domainSplitShipmentDef.SplitShipment.Product.Description1;
                lbl_Desc2.Text = domainSplitShipmentDef.SplitShipment.Product.Description2;
                lbl_Desc3.Text = domainSplitShipmentDef.SplitShipment.Product.Description3;
                lbl_Desc4.Text = domainSplitShipmentDef.SplitShipment.Product.Description4;
                lbl_Desc5.Text = domainSplitShipmentDef.SplitShipment.Product.Description5;
                lbl_ItemNo.Text = domainSplitShipmentDef.SplitShipment.Product.ItemNo;
                lbl_ItemNoHeader.Text = lbl_ItemNo.Text;
            }


            lbl_SuppInvNo.Text = domainSplitShipmentDef.SplitShipment.SupplierInvoiceNo;
            txt_SuppInvNo.Text = domainSplitShipmentDef.SplitShipment.SupplierInvoiceNo;
            lbl_QACommPercent.Text = domainSplitShipmentDef.SplitShipment.QACommissionPercent.ToString();
            lbl_QACommAmt.Text = Math.Round((domainSplitShipmentDef.SplitShipment.QACommissionPercent / 100) * domainSplitShipmentDef.SplitShipment.TotalShippedSupplierGarmentAmountAfterDiscount, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");
            lbl_DiscountPercent.Text = domainSplitShipmentDef.SplitShipment.VendorPaymentDiscountPercent.ToString();
            lbl_DiscountAmt.Text = Math.Round(domainSplitShipmentDef.SplitShipment.TotalShippedSupplierGarmentAmountAfterDiscount * (domainSplitShipmentDef.SplitShipment.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");
            lbl_LabTestIncome.Text = domainSplitShipmentDef.SplitShipment.LabTestIncome.ToString("#,##0.000");
            lbl_LabTestIncomeAmt.Text = Math.Round((domainSplitShipmentDef.SplitShipment.LabTestIncome * domainSplitShipmentDef.SplitShipment.TotalShippedQuantity), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

            lbl_NetAmtToSupp.Text = (domainSplitShipmentDef.SplitShipment.TotalShippedSupplierGarmentAmountAfterDiscount
                - Math.Round((domainSplitShipmentDef.SplitShipment.QACommissionPercent / 100) * domainSplitShipmentDef.SplitShipment.TotalShippedSupplierGarmentAmountAfterDiscount, 2, MidpointRounding.AwayFromZero)
                - Math.Round(domainSplitShipmentDef.SplitShipment.TotalShippedSupplierGarmentAmountAfterDiscount * (domainSplitShipmentDef.SplitShipment.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero)
                - Math.Round((domainSplitShipmentDef.SplitShipment.LabTestIncome * domainSplitShipmentDef.SplitShipment.TotalShippedQuantity), 2, MidpointRounding.AwayFromZero)).ToString("#,##0.00");
            lbl_LCNo.Text = domainSplitShipmentDef.SplitShipment.LCNo;
            txt_LCNo.Text = domainSplitShipmentDef.SplitShipment.LCNo;

            if (domainSplitShipmentDef.LCApplication != null && domainSplitShipmentDef.LCApplication.WorkflowStatus != LCWFS.REJECTED)
            {
                if (domainSplitShipmentDef.LCBatch != null)
                    lbl_LCBatchNo.Text = "LCB" + domainSplitShipmentDef.LCBatch.LCBatchNo.ToString("000000");
                lbl_LCBatchSubmitDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.LCApplication.LCApprovalDate);
                lbl_LCAppNo.Text = domainSplitShipmentDef.LCApplication.LCApplicationNo.ToString().PadLeft(6, '0');
            }

            if (domainSplitShipmentDef.SplitShipment.LCExpiryDate != DateTime.MinValue)
            {
                lbl_LCExpiryDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.LCExpiryDate);
                txt_LCExpiryDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.LCExpiryDate);
            }

            lbl_LCIssueDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.LCIssueDate);
            txt_LCIssueDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.LCIssueDate);
            lbl_LCAmt.Text = domainSplitShipmentDef.SplitShipment.LCAmount.ToString("#,##0.00");
            txt_LCAmt.Text = domainSplitShipmentDef.SplitShipment.LCAmount.ToString("#,##0.00");
            if (domainSplitShipmentDef.SplitShipment.IsLCPaymentChecked == 1)
            {
                ckb_LCPaymentChecked.Checked = true;
                lbl_LCPaymentCheckedDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.LCPaymentCheckedDate);
                txt_LCPaymentCheckedDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.LCPaymentCheckedDate);
            }
            else
                ckb_LCPaymentChecked.Checked = false;

            if (domainSplitShipmentDef.Invoice != null)
            {
                if (domainSplitShipmentDef.Invoice.InvoiceDate != null && domainSplitShipmentDef.Invoice.InvoiceDate != DateTime.MinValue)
                    lbl_ActualAtWHDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.Invoice.InvoiceDate);
            }

            lbl_ShipRecvDocDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.ShippingDocReceiptDate);
            txt_ShipRecvDocDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.ShippingDocReceiptDate);
            lbl_ACRecvDocDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.AccountDocReceiptDate);

            ckb_ShipDocCheck.Checked = (domainSplitShipmentDef.SplitShipment.ShippingDocCheckedOn != DateTime.MinValue);
            if (domainSplitShipmentDef.SplitShipment.ShippingCheckedTotalNetAmount == 0 && !ckb_ShipDocCheck.Checked)
                txt_ShipDocCheckAmount.Text = "";
            else
                txt_ShipDocCheckAmount.Text = domainSplitShipmentDef.SplitShipment.ShippingCheckedTotalNetAmount.ToString("#,##0.00");
            lbl_ShipDocCheckAmount.Enabled = ckb_ShipDocCheck.Checked;
            lbl_ShipDocCheckAmount.Text = txt_ShipDocCheckAmount.Text;

            SunInterfaceLogDef sunInterfaceLog = AccountManager.Instance.getInitialLogByShipmentId(SunInterfaceTypeRef.Id.Purchase.GetHashCode(),
                vwDomainSplitShipmentDef.Shipment.ShipmentId, splitShipmentId);
            if (sunInterfaceLog != null)
                lbl_SUNInterfaceDate.Text = DateTimeUtility.getDateString(sunInterfaceLog.CreatedOn);
            lbl_BankRefNo.Text = domainSplitShipmentDef.SplitShipment.APRefNo;
            lbl_SettlementAmt.Text = domainSplitShipmentDef.SplitShipment.APAmt.ToString("#,##0.00");
            lbl_SettlementDate.Text = DateTimeUtility.getDateString(domainSplitShipmentDef.SplitShipment.APDate);

            int lgId = AccountManager.Instance.getLGDetail(domainSplitShipmentDef.SplitShipment.ShipmentId, domainSplitShipmentDef.SplitShipment.SplitShipmentId);
            LetterOfGuaranteeDef lgDef = AccountManager.Instance.getLetterOfGuaranteeByKey(lgId);

            lbl_LGDueDate.Text = lgId > 0 ? DateTimeUtility.getDateString(LetterOfGuaranteeDef.getLGDueDate(domainSplitShipmentDef.Invoice.InvoiceDate)) : string.Empty;

            hid_WorkflowStatus.Value = (domainSplitShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id ? "INVOICED" : ""); 
            gv_Options.DataSource = domainSplitShipmentDef.SplitShipmentDetail;
            gv_Options.DataBind();

        }

        #region GridView Events
        protected void OptionRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Cell[6] -- Selling Amt
                decimal total;
                SplitShipmentDetailDef splitShipmentDetail = (SplitShipmentDetailDef)vwDomainSplitShipmentDef.SplitShipmentDetail[e.Row.RowIndex];
                totalShippedQty += splitShipmentDetail.ShippedQuantity;

                Label lbl = (Label)e.Row.Cells[6].Controls[1];
                total = splitShipmentDetail.ReducedSellingPrice * splitShipmentDetail.ShippedQuantity;
                lbl.Text = total.ToString("#,##0.00");
                totalSellingAmt += total;

                lbl = (Label)e.Row.Cells[8].Controls[1];
                total = splitShipmentDetail.ReducedSupplierGmtPrice * splitShipmentDetail.ShippedQuantity;
                lbl.Text = total.ToString("#,##0.00");
                totalSupplierAmt += total;

                if (splitShipmentDetail.ReducedSellingPrice != splitShipmentDetail.ReducedSupplierGmtPrice)
                {
                    lbl = (Label)e.Row.Cells[7].FindControl("lbl_FtyPrice");
                    lbl.ForeColor = System.Drawing.Color.SeaGreen;
                    lbl = (Label)e.Row.Cells[8].FindControl("lbl_OptionFactoryAmt");
                    lbl.ForeColor = System.Drawing.Color.SeaGreen;
                }
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                //Cell[3] -- Total Shipped Qty
                Label lbl = (Label)e.Row.Cells[3].Controls[1];
                lbl.Text = totalShippedQty.ToString("#,##0");
                lbl = (Label)e.Row.Cells[4].Controls[1];
                lbl.Text = totalShippedQty.ToString("#,##0");

                //Cell[6] -- Selling amount total
                lbl = (Label)e.Row.Cells[6].Controls[1];
                lbl.Text = totalSellingAmt.ToString("#,##0.00");

                //Cell[8] - supplier amount total
                lbl = (Label)e.Row.Cells[8].Controls[1];
                lbl.Text = totalSupplierAmt.ToString("#,##0.00");

                if (totalSellingAmt != totalSupplierAmt)
                {
                    lbl.ForeColor = System.Drawing.Color.SeaGreen;
                }
            }
        }

        protected void OptionOnRowEditing(object sender, GridViewEditEventArgs arg)
        {
            gv_Options.EditIndex = arg.NewEditIndex;
            gv_Options.DataSource = vwDomainSplitShipmentDef.SplitShipmentDetail;
            gv_Options.DataBind();
        }

        protected void OptionRowUpdating(object sender, GridViewUpdateEventArgs arg)
        {
            SplitShipmentDetailDef splitShipmentDetailDef = (SplitShipmentDetailDef)vwDomainSplitShipmentDef.SplitShipmentDetail[arg.RowIndex];

            TextBox txt = (TextBox)gv_Options.Rows[arg.RowIndex].Cells[4].Controls[1];

            int num = 0;
            if (txt.Text != "" && !int.TryParse(txt.Text, out num))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid Shipped Qty.');", true);
                return;
            }
            splitShipmentDetailDef.ShippedQuantity = num;

            if (vwDomainSplitShipmentDef.UpdatedSplitShipmentDetail == null)
                vwDomainSplitShipmentDef.UpdatedSplitShipmentDetail = new ArrayList();

            vwDomainSplitShipmentDef.UpdatedSplitShipmentDetail.Add(splitShipmentDetailDef);

            gv_Options.EditIndex = -1;
            gv_Options.DataSource = vwDomainSplitShipmentDef.SplitShipmentDetail;
            gv_Options.DataBind();
        }

        protected void OptionRowCommand(object sender, GridViewCommandEventArgs arg)
        {
            if (arg.CommandName == "CancelSave")
            {
                gv_Options.EditIndex = -1;
                gv_Options.DataSource = vwDomainSplitShipmentDef.SplitShipmentDetail;
                gv_Options.DataBind();
            }
        }
        #endregion

        #region Button Events
        protected void btn_Edit_Click(object sender, EventArgs e)
        {
            SetControl(true);
        }

        decimal getTheTotalCheckedSupplierNetAmtFromAllSplit()
        {   // if the shipping document info in all split shipmnet are completed, it return the total supplier net amount for all split shipment. 
            //  otherwise return -1
            bool isReady = true;
            //decimal SupTotal 
            decimal SupNetAmt;
            decimal totalSupNetAmt = 0;
            SplitShipmentDef thisSpt = vwDomainSplitShipmentDef.SplitShipment;
            if ((ckb_ShipDocCheck.Checked && thisSpt.ShippingDocCheckedOn == DateTime.MinValue))
            {
                UserRef usr = CommonUtil.getUserByKey(this.LogonUserId);
                ArrayList splitList = (ArrayList)OrderManager.Instance.getSplitShipmentByShipmentId(vwDomainSplitShipmentDef.Shipment.ShipmentId);
                if (splitList != null)
                    foreach (SplitShipmentDef spt in splitList)
                        if (spt.IsVirtualSetSplit == 0)
                        {
                            if (spt.SplitShipmentId == thisSpt.SplitShipmentId)
                            {
                                DateTime recDate;
                                if (string.IsNullOrEmpty(txt_SuppInvNo.Text) || !DateTime.TryParse(txt_ShipRecvDocDate.Text, out recDate))
                                {
                                    isReady = false;
                                    break;
                                }
                                SupNetAmt = decimal.Parse(lbl_NetAmtToSupp.Text);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(spt.SupplierInvoiceNo)
                                     || spt.ShippingDocReceiptDate == DateTime.MinValue
                                     || spt.ShippingDocCheckedOn == DateTime.MinValue || spt.ShippingCheckedTotalNetAmount == decimal.MinValue
                                )
                                {
                                    isReady = false;
                                    break;
                                }
                                SupNetAmt = spt.ShippingCheckedTotalNetAmount;
                            }
                            totalSupNetAmt += SupNetAmt;
                        }
            }
            return (isReady ? totalSupNetAmt : -1);
        }


        protected void btn_Save_Click(object sender, EventArgs e)
        {

            if (ckb_ShipDocCheck.Checked) // &&ckb_ShipDocCheck.Enabled
            {
                if (txt_SuppInvNo.Text == "" || txt_ShipRecvDocDate.Text == "")
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SplitShipmentUpdate", "alert('Please input the following items before tick “Shipping Doc. Check Amt” checkbox:\\n - Supplier Invoice No.;\\n - Shipping Doc. Receipt Date.');", true);
                    //ckb_ShipDocCheck.Checked = false;
                    //txt_ShipDocCheckAmount.Text = "";
                    return;
                }
            }

            if (getTheTotalCheckedSupplierNetAmtFromAllSplit() > 0)
            {
                if (string.IsNullOrEmpty(vwDomainSplitShipmentDef.Invoice.SupplierInvoiceNo)
                    // || vwDomainSplitShipmentDef.Invoice.PiecesPerDeliveryUnit <= 0   
                    // || vwDomainSplitShipmentDef.Invoice.ShippingDocReceiptDate == DateTime.MinValue
                    )
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ShipmentDocStatusUpdateFailure", "alert('Shipping document status of this shipment has not been updated completely.');", true);
                }
            }

            if (Page.IsValid)
            {
                updateSplitShipmentShippingDocInfo();
                updateShipmentDetail();
                updateSplitShipment();

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.UpdateSplitShipment);
                Context.Items.Add(ShipmentCommander.Param.splitShipment, vwDomainSplitShipmentDef.SplitShipment);
                Context.Items.Add(ShipmentCommander.Param.splitShipmentDetails, vwDomainSplitShipmentDef.UpdatedSplitShipmentDetail);

                forwardToScreen(null);

                SetControl(false);
                BindData(vwDomainSplitShipmentDef);
            }
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            SetControl(false);
            ckb_LCPaymentChecked.Checked = vwDomainSplitShipmentDef.SplitShipment.IsLCPaymentChecked == 1;
            if (vwDomainSplitShipmentDef.UpdatedSplitShipmentDetail != null)
                vwDomainSplitShipmentDef.UpdatedSplitShipmentDetail = null;

            ckb_ShipRecvDocDate.Checked = (lbl_ShipRecvDocDate.Text != "");
            txt_ShipRecvDocDate.Text = lbl_ShipRecvDocDate.Text;
            txt_ShipDocCheckAmount.Text = lbl_ShipDocCheckAmount.Text;
            ckb_ShipDocCheck.Checked = lbl_ShipDocCheckAmount.Enabled;
        }

        protected void ckb_ShipDocCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_ShipDocCheck.Checked)
                txt_ShipDocCheckAmount.Text = lbl_NetAmtToSupp.Text;
            else
                txt_ShipDocCheckAmount.Text = "";
        }

        #endregion

        void SetControl(bool isEditable)
        {
            txt_Color.Visible = isEditable;
            txt_ShipRecvDocDate.Visible = (isEditable);
            //if (ShipmentManager.Instance.isReadyForDMS(vwDomainSplitShipmentDef.Contract.Office, vwDomainSplitShipmentDef.Shipment.WorkflowStatus, vwDomainSplitShipmentDef.Invoice.InvoiceDate, vwDomainSplitShipmentDef.SplitShipment.Vendor))
            if (ShipmentManager.Instance.isReadyForDMS(vwDomainSplitShipmentDef))
            {
                bool allowShipDocCheck = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.DMSControl);
                bool viewShipDocCheck = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.DMSControlView);
                if (allowShipDocCheck || viewShipDocCheck)
                    tr_ShipDocCheck.Style.Add(HtmlTextWriterStyle.Display, "Block");
                if (vwDomainSplitShipmentDef.SplitShipment.ShippingDocWFS.Id == ShippingDocWFS.NOT_READY.Id || vwDomainSplitShipmentDef.SplitShipment.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                {
                    txt_SuppInvNo.Visible = isEditable;
                    txt_ShipRecvDocDate.Visible = isEditable;
                    ckb_ShipDocCheck.Enabled = (isEditable && allowShipDocCheck);
                }
                else
                {
                    txt_SuppInvNo.Visible = false;
                    txt_ShipRecvDocDate.Visible = false;
                    ckb_ShipDocCheck.Enabled = false;
                }
            }
            else
            {
                tr_ShipDocCheck.Style.Add(HtmlTextWriterStyle.Display, "None");
                txt_SuppInvNo.Visible = isEditable;
                txt_ShipRecvDocDate.Visible = isEditable;
                ckb_ShipDocCheck.Enabled = false;
            }
            btn_Save.Visible = isEditable;
            btn_Cancel.Visible = isEditable;
            if (btn_Save.Visible)
            {
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.AllowInputZeroQuantity))
                    btn_Save.OnClientClick = "return (allowToSave() ? confirmIfTotalShippedQtyZero(true) : false);";    // get confirm on zero total shipped qty 
                else
                    btn_Save.OnClientClick = "return (allowToSave() ? confirmIfTotalShippedQtyZero(false) : false);";  // block saving if total shipped qty is zero
            }
            if (btn_Save.Visible)
                btn_Close.Attributes.Add("onclick", "if (confirm('You have not save your changes.\\nConfirm to close?')) window.close(); return false;");
            else
                btn_Close.Attributes.Add("onclick", "window.close(); return false;");


            if (vwDomainSplitShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id && vwDomainSplitShipmentDef.Shipment.PaymentLock)
                gv_Options.Columns[4].Visible = false;
            else
                gv_Options.Columns[4].Visible = isEditable;

            gv_Options.Columns[3].Visible = !gv_Options.Columns[4].Visible;

            if (gv_Options.Columns[4].Visible)
            {
                gv_Options.DataSource = vwDomainSplitShipmentDef.SplitShipmentDetail;
                gv_Options.DataBind();
            }

            if (isEditable && vwDomainSplitShipmentDef.SplitShipment.PaymentTerm.LCPaymentTermRequestFlag &&
                vwDomainSplitShipmentDef.LCApplication == null)
            {
                txt_LCNo.Visible = true;
                txt_LCIssueDate.Visible = true;
                txt_LCExpiryDate.Visible = true;
                txt_LCAmt.Visible = true;
                txt_LCPaymentCheckedDate.Visible = true;
                ckb_LCPaymentChecked.Enabled = true;
            }
            else
            {
                txt_LCNo.Visible = false;
                txt_LCIssueDate.Visible = false;
                txt_LCExpiryDate.Visible = false;
                txt_LCAmt.Visible = false;
                txt_LCPaymentCheckedDate.Visible = false;
                ckb_LCPaymentChecked.Enabled = false;
            }

            if (vwDomainSplitShipmentDef.Contract.Office.OfficeCode == "TR")
            {
                row_QACom.Visible = false;
                row_TRDiscount.Visible = true;
                row_LabTest.Visible = (vwDomainSplitShipmentDef.SplitShipment.LabTestIncome != 0);
            }
            else if (vwDomainSplitShipmentDef.Contract.Office.OfficeCode == "HK" || vwDomainSplitShipmentDef.Contract.Office.OfficeCode == "SH" ||
                vwDomainSplitShipmentDef.Contract.Office.OfficeCode == "TH")
            {
                row_QACom.Visible = true;
                row_TRDiscount.Visible = false;
                row_LabTest.Visible = (vwDomainSplitShipmentDef.SplitShipment.LabTestIncome != 0);
            }
            else if (vwDomainSplitShipmentDef.Contract.Office.OfficeCode == "LK")
            {
                row_QACom.Visible = false;
                row_TRDiscount.Visible = (vwDomainSplitShipmentDef.Shipment.CountryOfOrigin.CountryOfOriginId == CountryOfOriginRef.eCountryId.Pakistan.GetHashCode());
                row_LabTest.Visible = true;
            }
            else
            {
                row_QACom.Visible = false;
                row_TRDiscount.Visible = false;
                row_LabTest.Visible = (vwDomainSplitShipmentDef.SplitShipment.LabTestIncome != 0);
            }

            bool isLabelVisible = !isEditable;
            lbl_Color.Visible = isLabelVisible;

            lbl_SuppInvNo.Visible = !txt_SuppInvNo.Visible; //isLabelVisible;
            lbl_LCNo.Visible = !txt_LCNo.Visible;
            lbl_LCIssueDate.Visible = !txt_LCIssueDate.Visible;
            lbl_LCPaymentCheckedDate.Visible = !txt_LCPaymentCheckedDate.Visible;
            lbl_LCExpiryDate.Visible = !txt_LCExpiryDate.Visible;
            lbl_LCAmt.Visible = !txt_LCAmt.Visible;
            lbl_ShipRecvDocDate.Visible = !txt_ShipRecvDocDate.Visible; // (isLabelVisible || !txt_ShipRecvDocDate.Visible);
            btn_Edit.Visible = isLabelVisible;

            btn_cal_LCExpiryDate.Visible = txt_LCExpiryDate.Visible;
            ckb_ShipRecvDocDate.Visible = txt_ShipRecvDocDate.Visible;
        }


        protected void updateSplitShipmentShippingDocInfo()
        {
            UserRef usr = CommonUtil.getUserByKey(this.LogonUserId);
            DateTime nextWorkingDate = DateTimeUtility.getNextWorkingDate(DateTime.Now);
            DomainSplitShipmentDef domainDef = vwDomainSplitShipmentDef;

            domainDef.SplitShipment.ShippingDocReceiptDate = DateTimeUtility.getDate(txt_ShipRecvDocDate.Text);
            //if (ShipmentManager.Instance.isReadyForDMS(domainDef.Contract.Office, domainDef.Shipment.WorkflowStatus, domainDef.Invoice.InvoiceDate, domainDef.SplitShipment.Vendor))
            if (ShipmentManager.Instance.isReadyForDMS(domainDef))
            {
                if (ckb_ShipDocCheck.Checked && domainDef.SplitShipment.ShippingDocCheckedOn == DateTime.MinValue)
                {   // Check the checkbox
                    decimal amt;
                    if (!decimal.TryParse(txt_ShipDocCheckAmount.Text, out amt))
                        amt = 0;
                    domainDef.SplitShipment.ShippingCheckedTotalNetAmount = amt;
                    domainDef.SplitShipment.ShippingDocCheckedOn = DateTime.Now;
                    domainDef.SplitShipment.ShippingDocCheckedBy = usr;
                    if (domainDef.SplitShipment.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                    {
                        domainDef.SplitShipment.ShippingDocWFS = ShippingDocWFS.ACCEPTED;
                        domainDef.SplitShipment.RejectPaymentReasonId = RejectPaymentReason.NoReason.Id;
                        domainDef.SplitShipment.AccountDocReceiptDate = nextWorkingDate;
                        //if (domainDef.SplitShipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.OPENACCOUNT.GetHashCode()
                        //    || (domainDef.SplitShipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode()
                        //        && ckb_LCPaymentChecked.Checked))
                        //{
                        //    domainDef.SplitShipment.AccountDocReceiptDate = nextWorkingDate;
                        //}
                    }

                    decimal totalSupNetAmt;
                    if ((totalSupNetAmt = getTheTotalCheckedSupplierNetAmtFromAllSplit()) >= 0)
                    {   // all the shipping doc info in each split shipment are ready 
                        if (!string.IsNullOrEmpty(domainDef.Invoice.SupplierInvoiceNo) && domainDef.Invoice.PiecesPerDeliveryUnit > 0) //&& domainDef.Invoice.ShippingDocReceiptDate != DateTime.MinValue
                        {   // Update the Shipping Doc info in the Shipment/invoice record
                            domainDef.Invoice.ShippingCheckedTotalNetAmount = totalSupNetAmt;
                            domainDef.Invoice.ShippingDocCheckedOn = DateTime.Now;
                            domainDef.Invoice.ShippingDocCheckedBy = usr;
                            if (domainDef.Shipment.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                            {
                                domainDef.Shipment.ShippingDocWFS = ShippingDocWFS.ACCEPTED;
                                domainDef.Shipment.RejectPaymentReasonId = RejectPaymentReason.NoReason.Id;
                                domainDef.Invoice.AccountDocReceiptDate = nextWorkingDate;
                                //if (domainDef.Shipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.OPENACCOUNT.GetHashCode()
                                //    || (domainDef.Shipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode()
                                //        && (ckb_LCPaymentChecked.Checked || domainDef.Invoice.LCPaymentCheckedDate != DateTime.MinValue)))
                                //{
                                //    domainDef.Invoice.AccountDocReceiptDate = nextWorkingDate;
                                //}
                            }
                            if (domainDef.Invoice.ShippingDocReceiptDate == DateTime.MinValue)
                                domainDef.Invoice.ShippingDocReceiptDate = domainDef.SplitShipment.ShippingDocReceiptDate;
                        }
                    }
                }
                else if (!ckb_ShipDocCheck.Checked && domainDef.SplitShipment.ShippingDocCheckedOn != DateTime.MinValue)
                {   // Uncheck the checkbox 
                    domainDef.SplitShipment.ShippingCheckedTotalNetAmount = 0;
                    domainDef.SplitShipment.ShippingDocCheckedOn = DateTime.MinValue;
                    domainDef.SplitShipment.ShippingDocCheckedBy = null;
                    //domainDef.SplitShipment.ShippingDocWFS = ShippingDocWFS.NOT_READY;
                }
            }
            return;
        }

        void updateSplitShipment()
        {
            SplitShipmentDef splitShipment = vwDomainSplitShipmentDef.SplitShipment;

            splitShipment.Colour = txt_Color.Text.Trim();
            splitShipment.SupplierInvoiceNo = txt_SuppInvNo.Text.Trim();

            // Shipping Document Receipt Date
            splitShipment.ShippingDocReceiptDate = DateTimeUtility.getDate(txt_ShipRecvDocDate.Text);

            splitShipment.LCNo = txt_LCNo.Text.Trim();
            splitShipment.LCExpiryDate = DateTimeUtility.getDate(txt_LCExpiryDate.Text);
            splitShipment.LCAmount = decimal.Parse(txt_LCAmt.Text);

            if (ckb_LCPaymentChecked.Checked)
            {
                splitShipment.IsLCPaymentChecked = 1;
                splitShipment.LCPaymentCheckedDate = DateTimeUtility.getDate(txt_LCPaymentCheckedDate.Text);
            }
            else
            {
                splitShipment.IsLCPaymentChecked = 0;
                splitShipment.LCPaymentCheckedDate = DateTime.MinValue;
            }
        }

        bool updateShipmentDetail()
        {
            TextBox txt;
            SplitShipmentDetailDef splitShipmentDetail;
            bool isUpdated = false;
            int qty;
            int totalQty = 0;

            foreach (GridViewRow row in gv_Options.Rows)
            {
                splitShipmentDetail = (SplitShipmentDetailDef)vwDomainSplitShipmentDef.SplitShipmentDetail[row.RowIndex];

                if (gv_Options.Columns[4].Visible)
                {
                    txt = (TextBox)row.Cells[4].FindControl("txt_ShipQty");
                    if (!int.TryParse(txt.Text, out qty))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid shipped quantity.');", true);
                        return false;
                    }
                    else if (qty != splitShipmentDetail.ShippedQuantity)
                    {
                        splitShipmentDetail.ShippedQuantity = qty;
                        isUpdated = true;
                    }
                    totalQty += qty;
                }

                if (isUpdated)
                {
                    if (vwDomainSplitShipmentDef.UpdatedSplitShipmentDetail == null)
                        vwDomainSplitShipmentDef.UpdatedSplitShipmentDetail = new ArrayList();

                    vwDomainSplitShipmentDef.UpdatedSplitShipmentDetail.Add(splitShipmentDetail);
                }
                isUpdated = false;
            }
            //if (totalQty == 0 && !vwDomainSplitShipmentDef.Shipment.PaymentLock && vwDomainSplitShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
            //{
            //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ZeroTotalShippedQtyAlertMessage", "alert('Total Shipped Quantity is zero. Please confirm your data input.')", true);
            //}
            return true;
        }


        #region Validator
        protected void val_LCInfo_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = true;
            decimal num;
            DateTime date;


            if (txt_LCExpiryDate.Text != "" && !DateTime.TryParse(txt_LCExpiryDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
            {
                args.IsValid = false;
                val_LCInfo.ErrorMessage += "- Invalid L/C Expiry Date.<br />";
            }

            if (txt_LCAmt.Text != "" && !decimal.TryParse(txt_LCAmt.Text, out num))
            {
                args.IsValid = false;
                val_LCInfo.ErrorMessage += "- Invalid L/C Amount.<br />";
            }

            if (ckb_LCPaymentChecked.Checked)
            {
                if (!DateTime.TryParse(txt_LCPaymentCheckedDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
                {
                    args.IsValid = false;
                    val_LCInfo.ErrorMessage += "- Invalid L/C Payment Checked Date. <br />";
                }
            }

        }

        protected void val_PaymentInfo_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = true;
            DateTime date;


            if (txt_ShipRecvDocDate.Text != "" && !DateTime.TryParse(txt_ShipRecvDocDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
            {
                args.IsValid = false;
                val_PaymentInfo.ErrorMessage += "- Invalid Shipping Received Document Date.<br />";
            }

        }

        #endregion
    }
}
