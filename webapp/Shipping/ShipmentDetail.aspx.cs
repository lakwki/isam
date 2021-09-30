using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using com.next.common.appserver;
using com.next.common.domain;
using com.next.common.domain.module;
using com.next.common.web.commander;
using com.next.common.datafactory.worker;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.common;
using com.next.isam.appserver.account;
using com.next.isam.domain.account;
using com.next.isam.domain.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.common;
using com.next.isam.domain.ils;
using com.next.isam.domain.types;
using com.next.isam.domain.product;
using com.next.isam.appserver.shipping;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.shipment;
using com.next.infra.web;
using com.next.infra.util;
using com.next.infra.smartwebcontrol;
using com.next.common.domain.dms;
using com.next.isam.appserver.order;
using com.next.common.domain.types;
using com.next.common.domain.security;

namespace com.next.isam.webapp.shipping
{
    public partial class ShipmentDetail : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private int ShipmentId;
        private decimal totalSellingAmount = 0;
        private decimal totalSupplierAmount = 0;
        private decimal totalSupplierAmountCNY = 0;
        private decimal totalOtherCost = 0;

        // CHOICE total amount checking/updating
        private int amendingShippedQtyTotal = int.MinValue;
        private decimal amendingSalesAmountTotal = decimal.MinValue;
        private decimal amendingPurchaseAmountTotal = decimal.MinValue;
        private bool isChoiceTotalAmountAutoUpdate = false;
        private bool needToInputChoiceTotalAmount = false;

        private decimal totalILSSalesAmount = 0;
        private decimal totalILSQty = 0;
        private ArrayList arrTotalOtherCost = new ArrayList();
        private bool isInvoiceValidToSave = true;

        DomainShipmentDef vwDomainShipmentDef
        {
            get { return (DomainShipmentDef)ViewState["DomainShipmentDef"]; }
            set { ViewState["DomainShipmentDef"] = value; }
        }

        // Pre-NOW version
        //private string decryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        //private string encryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        //private string openPopupWindowFunction = "window.open";
        //NOW version
        //private string decryptParameter(string param) { return WebUtil.DecryptParameter(param); }
        //private string encryptParameter(string param) { return WebUtil.EncryptParameter(param); }
        //private string openPopupWindowFunction = "openPopupWindow";

        protected void Page_Load(object sender, EventArgs e)
        {
           //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "FilterInput", "filterInput(" + WebHelper.isInternalITIpAddress().ToString().ToLower() + ");", true);
            if (!Page.IsPostBack)
            {
                ArrayList inputShipmentIdList;
                string inputShipmentIdParam = null;

                inputShipmentIdParam = decryptParameter(Request.Params["ShipmentIdList"]);
                //inputShipmentIdParam = hid_Parameter_ShipmentIdList.Value.ToString();
                inputShipmentIdList = new ArrayList();
                if (!string.IsNullOrEmpty(inputShipmentIdParam))
                {
                    string[] shipmentIdArray = inputShipmentIdParam.Split(char.Parse("|"));
                    foreach (string id in shipmentIdArray)
                        if (!string.IsNullOrEmpty(id))
                            inputShipmentIdList.Add(int.Parse(id));
                }
                this.lnk_Prev.Visible = (inputShipmentIdList.Count > 1);
                this.lnk_Next.Visible = (inputShipmentIdList.Count > 1);

                inputShipmentIdParam = decryptParameter(Request.Params["ShipmentId"]);

                //inputShipmentIdParam = hid_Parameter_ShipmentId.Value.ToString();
                if (!string.IsNullOrEmpty(inputShipmentIdParam))
                    ShipmentId = int.Parse(inputShipmentIdParam);
                else
                    if (inputShipmentIdList.Count > 0)
                        ShipmentId = (int)inputShipmentIdList[0];
                    else
                    {
                        inputShipmentIdList.Clear();
                        return;
                    }
                inputShipmentIdList.Clear();

                vwDomainShipmentDef = ShipmentManager.Instance.getDomainShipmentDef(ShipmentId);
                isChoiceTotalAmountAutoUpdate = ShippingWorker.Instance.isChoiceActualAmountAutoUpdate(vwDomainShipmentDef.Contract.Customer.CustomerId, vwDomainShipmentDef.Contract.Office.OfficeId);
                needToInputChoiceTotalAmount = (vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.choice.GetHashCode() && "BD,IN,ND,LK,PK,TR,MA,EG,HK,TH,VN,CA".Contains(vwDomainShipmentDef.Contract.Office.OfficeCode));
                BindData(vwDomainShipmentDef);
                SetControl(false);
            }
            else
            {
                isChoiceTotalAmountAutoUpdate = ShippingWorker.Instance.isChoiceActualAmountAutoUpdate(vwDomainShipmentDef.Contract.Customer.CustomerId, vwDomainShipmentDef.Contract.Office.OfficeId);
                needToInputChoiceTotalAmount = (vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.choice.GetHashCode() && "BD,IN,ND,LK,PK,TR,MA,EG,HK,TH,VN,CA".Contains(vwDomainShipmentDef.Contract.Office.OfficeCode));
            }
            isInvoiceValidToSave = true;

            string defaultReceiptDate = decryptParameter(Request.Params["DefaultReceiptDate"]);
            if (string.IsNullOrEmpty(defaultReceiptDate))
                defaultReceiptDate = "";


            //if (hid_Parameter_DefaultReceiptDate.Value.ToString()!= "")
            //{
            //    defaultReceiptDate = hid_Parameter_DefaultReceiptDate.Value.ToString();
            //}


            ckb_PaymentChecked.Attributes.Add("onclick", "javascript:getCurrentDate(this,'" + txt_PaymentCheckDate.ClientID + "');");
            ckb_ImportDutyRecordChecked.Attributes.Add("onclick", "javascript:getCurrentDate(this,'" + txt_ImportDutyRecordCheckedDate.ClientID + "');");
            ckb_InputVATRecordChecked.Attributes.Add("onclick", "javascript:getCurrentDate(this,'" + txt_InputVATRecordCheckedDate.ClientID + "');");
            ckb_OutputVATRecordChecked.Attributes.Add("onclick", "javascript:getCurrentDate(this,'" + txt_OutputVATRecordCheckedDate.ClientID + "');");
            ckb_ShipRecDocDate.Attributes.Add("onclick", "javascript:getDefaultDate(this,'" + txt_ShipRecDocDate.ClientID + "','" + defaultReceiptDate + "');");
            ckb_ShipDocCheck.Attributes.Add("onclick", "javascript:isShipDocCheckValid(this,'ckb_ShipDocCheck');");


            img_UploadedDoc.Attributes.Clear();
            if (vwDomainShipmentDef.Invoice.IsUploadDMSDocument)
                img_UploadedDoc.Attributes.Add("onclick", openPopupWindowFunction + "('../account/AttachmentList.aspx?ShipmentId=" + HttpUtility.UrlEncode(EncryptionUtility.EncryptParam(vwDomainShipmentDef.Shipment.ShipmentId.ToString())) + "', 'attachmentlist', 'width=400,height=300,scrollbars=1,status=0');return false;");
            else
                img_UploadedDoc.Visible = false;
        }


        void BindData(DomainShipmentDef domainShipmentDef)
        {
            ContractDef _contract = vwDomainShipmentDef.Contract;
            ShipmentDef _shipment = vwDomainShipmentDef.Shipment;

            if (_contract.NSLPONo != null && _contract.NSLPONo != "")
            {
                txt_PONo.Text = domainShipmentDef.Contract.NSLPONo.Substring(0, domainShipmentDef.Contract.NSLPONo.LastIndexOf("/"));
            }
            txt_Status.Text = _shipment.WorkflowStatus.Name;
            txt_PaymentTerm.Text = _shipment.PaymentTerm.PaymentTermDescription;
            txt_GSPScheme.Text = GSPFormType.getName(_shipment.GSPFormTypeId);
            if (_shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id && (!_shipment.EditLock || !_shipment.PaymentLock))
                img_releaseLock.Visible = true;
            if (_contract.IsDualSourcingOrder == 1)
                img_dualSourcing.Visible = true;
            if (_contract.IsNextMfgOrder == 1)
                img_SZOrder.Visible = true;
            if (_shipment.IsUKDiscount == 1)
                img_discount.Visible = true;
            if (_shipment.WithOPRFabric != 0)
                img_OPRFabric.Visible = true;
            //if (CommonManager.Instance.getCustomerDestinationByKey(_shipment.CustomerDestinationId).UTurnOrder == 1)
            if (_shipment.TermOfPurchase.TermOfPurchaseId == TermOfPurchaseRef.Id.FOB_UT.GetHashCode())
                img_UTurn.Visible = true;
            if (_shipment.IsMockShopSample == 1)
                img_MockShop.Visible = true;
            if (_shipment.IsPressSample == 1)
                img_PressSample.Visible = true;
            if (_shipment.IsStudioSample == 1)
                img_StudioSample.Visible = true;
            if (_contract.IsLDPOrder == 1)
                img_LDP.Visible = true;
            if (_shipment.WithQCCharge == 1)
                img_WithQCCharge.Visible = true;

            if (_shipment.IsTradingAirFreight && _shipment.TradingAirFreightTypeId == 1)
                img_TradingAF.Visible = (_shipment.IsTradingAirFreight);
            else
            {
                if (_shipment.ShipmentMethod.ShipmentMethodId == ShipmentMethodRef.Method.AIR.GetHashCode()
                    || _shipment.ShipmentMethod.ShipmentMethodId == ShipmentMethodRef.Method.SEAorAIR.GetHashCode()
                    || _shipment.ShipmentMethod.ShipmentMethodId == ShipmentMethodRef.Method.ECOAIR.GetHashCode())
                {
                    img_AirShipment.Visible = (_shipment.NSLAirFreightPaymentPercent == 0);
                    img_AirFreightWithPay.Visible = (_shipment.NSLAirFreightPaymentPercent > 0);
                }
            }

            if (_shipment.SpecialOrderTypeId == 1)
                img_ReprocessGoods.Visible = true;

            /*
            if (ShipmentManager.Instance.isViaCambodiaQCC(domainShipmentDef.Shipment, domainShipmentDef.Invoice))
            {
                img_QCCInspection.Visible = true;
                ckb_QCCInspection.Checked = true;
            }
            */

            if (_shipment.IsChinaGBTestRequired)
            {
                img_GBTestRequired.Visible = true;
                ckb_GBTest.Checked = true;
                int testResult = GeneralManager.Instance.getChinaGBTestResult(_contract.ProductId, _shipment.Vendor.VendorId);
                if (testResult > -1)
                {
                    txt_GBTestResult.Visible = true;
                    if (testResult == 1)
                    {
                        img_GBTestPass.Visible = true;
                        txt_GBTestResult.Text = "PASS";
                    }
                    else if (testResult == 0)
                    {
                        img_GBTestFailedHold.Visible = true;
                        txt_GBTestResult.Text = "FAIL(HOLD PAYMENT)";
                    }
                    else if (testResult == 2)
                    {
                        img_GBTestFailedRelease.Visible = true;
                        txt_GBTestResult.Text = "FAIL(RELEASE PAYMENT)";
                    }
                    else if (testResult == 9)
                    {
                        img_GBTestFailedCannotRelease.Visible = true;
                        txt_GBTestResult.Text = "FAIL(CANNOT RELEASE PAYMENT)";
                    }
                }
            }
            txt_ContractNo.Text = _contract.ContractNo;
            txt_NSLDlyNo.Text = _shipment.DeliveryNo.ToString();
            if (_contract.Customer.OPSKey == "E")
            {
                tr_InboundDelNo.Visible = true;
                txt_InboundDelNo.Text = _contract.BookingRefNo;

                tr_Forwarder.Visible = true;
                txt_Forwarder.Text = domainShipmentDef.CustomerSummary.Forwarder;
                txt_SchduleDlyDate.Text = DateTimeUtility.getDateString(domainShipmentDef.CustomerSummary.ScheduledDeliveryDate);
                txt_PromotionStart.Text = DateTimeUtility.getDateString(domainShipmentDef.CustomerSummary.PromotionStartDate);
            }
            txt_NUKDlyNo.Text = domainShipmentDef.CustomerSummary.DeliveryNo == 0 ? "" : domainShipmentDef.CustomerSummary.DeliveryNo.ToString();
            txt_Office.Text = _contract.Office.OfficeCode;
            txt_VendorName.Text = _shipment.Vendor.Name;
            txt_VendorCode.Text = _contract.UKSupplierCode;
            if (_shipment.FactoryId > 0)
            {
                common.domain.industry.vendor.FactoryRef factory = IndustryManager.Instance.getFactoryByFactoryId(_shipment.FactoryId);
                if (factory != null)
                    txt_FactoryName.Text = factory.Name.ToUpper();
            }
            if (_contract.IsNextMfgOrder == 1 && _contract.IsPOIssueToNextMfg == 1)
            {
                //row_VMVendor.Visible = true;
                if (_shipment.VMVendor != null)
                {
                    txt_VMVendorName.Text = _shipment.VMVendor.Name;
                    txt_VMVendorCode.Text = _shipment.VMVendor.OPSSupCod;
                }
            }

            txt_ItemNo.Text = domainShipmentDef.Product.ItemNo;
            txt_Season.Text = _contract.Season.Code;
            txt_Phase.Text = _contract.PhaseId.ToString();
            txt_PiecePerPack.Text = _contract.PiecesPerPack.ToString() + " / " + _contract.PackingUnit.OPSKey;
            txt_TradingAgency.Text = CommonManager.Instance.getTradingAgencyByKey(_contract.TradingAgencyId).ShortName;
            txt_Customer.Text = _contract.Customer.CustomerCode;
            if (_shipment.QuotaCategoryGroup != null)
                txt_NSSQuotaCat.Text = _shipment.QuotaCategoryGroup.OPSKey;
            if (domainShipmentDef.Product.DesignSource != null)
                txt_DesignSource.Text = domainShipmentDef.Product.DesignSource.DesignSourceCode;

            CustomerDestinationDef dest = CommonManager.Instance.getCustomerDestinationByKey(_shipment.CustomerDestinationId);

            if (((_contract.Customer.CustomerId == 1 || _contract.Customer.CustomerId == 2) && dest.DestinationCode != "UK") || _shipment.IsMockShopSample == 1)
            {
                txt_NSSPackMethod.Text = PackingMethodRef.getDescription(PackingMethodRef.Id.fp.GetHashCode());
                txt_NSSRefurbishment.Text = "NO";
            }
            else
            {
                txt_NSSRefurbishment.Text = _contract.PackingMethod.Refurb ? "YES" : "NO";
                txt_NSSPackMethod.Text = _contract.PackingMethod.PackingMethodDescription;
            }
            txt_NUKPackMethod.Text = PackingMethodRef.getDescription(domainShipmentDef.CustomerSummary.PackingMethod);
            if (txt_NSSPackMethod.Text != txt_NUKPackMethod.Text)
                txt_NUKPackMethod.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

            if (_shipment.ShipmentCountry != null && _shipment.ShipmentPort != null)
                txt_NSSShipFromCountry.Text = _shipment.ShipmentCountry.ShipmentCountryDescription + ", " +
                    _shipment.ShipmentPort.ShipmentPortDescription;
            txt_NUKShipFromCountry.Text = domainShipmentDef.CustomerSummary.Departure;
            if (!txt_NSSShipFromCountry.Text.StartsWith(txt_NUKShipFromCountry.Text))
                txt_NUKShipFromCountry.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

            txt_NSSDestination.Text = dest.DestinationDesc;
            txt_NUKDestination.Text = domainShipmentDef.CustomerSummary.Destination;
            if (!txt_NUKDestination.Text.StartsWith(txt_NSSDestination.Text))
                txt_NUKDestination.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

            txt_Team.Text = _contract.ProductTeam.Description;
            txt_Mer.Text = _contract.Merchandiser.DisplayName;
            if (vwDomainShipmentDef.Product.CartonType != null)
                txt_BDCMType.Text = vwDomainShipmentDef.Product.CartonType.CartonTypeDesc;

            txt_QtyPerCarton.Text = vwDomainShipmentDef.Product.QtyPerCarton.ToString();
            txt_NUKRefurbishment.Text = domainShipmentDef.CustomerSummary.Refurb;
            if (txt_NSSRefurbishment.Text != txt_NUKRefurbishment.Text)
                txt_NUKRefurbishment.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

            txt_NSSShipMode.Text = _shipment.ShipmentMethod.ShipmentMethodDescription;
            txt_NUKShipMode.Text = domainShipmentDef.CustomerSummary.TransportMode;
            if (txt_NSSShipMode.Text != txt_NUKShipMode.Text)
                txt_NUKShipMode.BackColor = System.Drawing.Color.FromArgb(255, 255, 102);

            txt_NextFreightPercent.Text = domainShipmentDef.CustomerSummary.FreightPercentForNUK == 999 ? "N/A" : domainShipmentDef.CustomerSummary.FreightPercentForNUK.ToString();
            txt_SupplierFreightPercent.Text = domainShipmentDef.CustomerSummary.FreightPercentForNSL == 999 ? "N/A" : domainShipmentDef.CustomerSummary.FreightPercentForNSL.ToString();

            if (_shipment.ShipmentMethod.ShipmentMethodId == ShipmentMethodRef.Method.AIR.GetHashCode() ||
                _shipment.ShipmentMethod.ShipmentMethodId == ShipmentMethodRef.Method.ECOAIR.GetHashCode() ||
                _shipment.ShipmentMethod.ShipmentMethodId == ShipmentMethodRef.Method.SEAorAIR.GetHashCode())
            {
                if (_shipment.AirFreightPaymentType != null)
                    txt_AirFreigthPaymentType.Text = _shipment.AirFreightPaymentType.AirFreightPaymentTypeDescription;
                txt_NUKAirFreightPaymentPercent.Text = _shipment.NUKAirFreightPaymentPercent.ToString();
                txt_NSLAirFreightPaymentPercent.Text = _shipment.NSLAirFreightPaymentPercent.ToString();
                txt_FTYAirFreightPaymentPercent.Text = _shipment.FTYAirFreightPaymentPercent.ToString();

                txt_OtherAirFreightPaymentRemark.Text = _shipment.OtherAirFreightPaymentRemark;
                txt_OtherAirFreightPaymentPercent.Text = _shipment.OtherAirFreightPaymentPercent.ToString();
            }
            else
            {
                txt_AirFreigthPaymentType.Visible = false;
                tr_AirFreightPayment.Visible = false;
            }

            if (_shipment.IsTradingAirFreight)
            {
                ckb_IsTradingAF.Checked = _shipment.IsTradingAirFreight;
                txt_AirFreightReason.Text = _shipment.TradingAirFreightReason;
                tdAirFreightReason.Visible = true;
                txt_TradingAFEstimateCost.Text = _shipment.TradingAirFreightEstimationCost.ToString("#,##0.00");
                txt_TradingAFActualCode.Text = _shipment.TradingAirFreightActualCost.ToString("#,##0.00");
            }
            else
                tdAirFreightReason.Visible = false;

            ckb_LDPOrder.Checked = _contract.IsLDPOrder == 1;
            ckb_WithQCCharge.Checked = _shipment.WithQCCharge == 1;
            if (_contract.Office.OfficeCode != "TR")
                lbl_BizDesign.Visible = false;
            else
                ckb_IsBizOrder.Checked = _contract.IsBizOrder == 1;

            txt_PurchaseTerm.Text = _shipment.TermOfPurchase.TermOfPurchaseDescription;
            txt_PurchaseCountry.Text = _shipment.PurchaseLocation.PurchaseLocationDescription;
            if (_shipment.CountryOfOrigin != null)
                txt_CO.Text = _shipment.CountryOfOrigin.Name;
            txt_OPRType.Text = OPRFabricType.getName(_shipment.WithOPRFabric);
            txt_NoteFromMer.Text = _shipment.NotesFromMerchandiser;
            txt_MockShopSample.Text = _shipment.MockShopSampleRemark;
            txt_FuCert.Text = _shipment.CustomDocType == 0 ? "NO" : "YES";

            //summary
            if (_contract.Customer.OPSKey == "E")
                lbl_AtWHDate.Text = "Origin Forwarder Date";
            lbl_NUKAtWHDate.Text = DateTimeUtility.getDateString(domainShipmentDef.CustomerSummary.AtWarehouseDate);
            lbl_CtoNSLAtWHDate.Text = DateTimeUtility.getDateString(_shipment.CustomerAgreedAtWarehouseDate);
            lbl_NUKActualAtWHDate.Text = DateTimeUtility.getDateString(domainShipmentDef.Invoice.ILSActualAtWarehouseDate);
            lbl_NSLtoSuppAtWHDate.Text = DateTimeUtility.getDateString(_shipment.SupplierAgreedAtWarehouseDate);

            lbl_NUKCcy.Text = domainShipmentDef.CustomerSummary.OrderCurrency;
            if (domainShipmentDef.CustomerSummary.OrderCurrency != _shipment.SellCurrency.CurrencyCode && domainShipmentDef.CustomerSummary.OrderCurrency != "N/A")
                td_NUKCcy.BgColor = "yellow";
            lbl_CtoNSLCcy.Text = _shipment.SellCurrency.CurrencyCode;
            lbl_NSLtoSuppCcy.Text = _shipment.BuyCurrency.CurrencyCode;
            lbl_NUKActualCcy.Text = domainShipmentDef.CustomerSummary.InvoiceCurrency;
            if (domainShipmentDef.CustomerSummary.InvoiceCurrency != _shipment.SellCurrency.CurrencyCode && domainShipmentDef.CustomerSummary.InvoiceCurrency != "N/A")
                td_NUKActualCcy.BgColor = "yellow";
            lbl_NSLActualCcy.Text = _shipment.SellCurrency.CurrencyCode;
            lbl_SupplierActualCcy.Text = _shipment.BuyCurrency.CurrencyCode;

            lbl_NUKQty.Text = domainShipmentDef.CustomerSummary.TotalOrderQuantity.ToString("#,##0");
            lbl_CtoNSLQty.Text = _shipment.TotalOrderQuantity.ToString("#,##0");
            lbl_NSLtoSuppQty.Text = _shipment.TotalPOQuantity.ToString("#,##0");
            lbl_NUKActualQty.Text = domainShipmentDef.CustomerSummary.TotalShippedQuantity.ToString("#,##0");
            lbl_NSLActualQty.Text = _shipment.TotalShippedQuantity.ToString("#,##0");
            lbl_SupplierActualQty.Text = _shipment.TotalShippedQuantity.ToString("#,##0");

            lbl_NUKSalesAmt.Text = domainShipmentDef.CustomerSummary.TotalOrderAmount.ToString("#,##0.00");
            lbl_CtoNSLSalesAmt.Text = _shipment.TotalOrderAmount.ToString("#,##0.00");
            lbl_NUKActualSalesAmt.Text = domainShipmentDef.CustomerSummary.TotalShippedAmount.ToString("#,##0.00");
            lbl_NSLActualSalesAmt.Text = _shipment.TotalShippedAmount.ToString("#,##0.00");

            lbl_NSLtoSuppAmt.Text = _shipment.TotalPOSupplierGarmentAmountAfterDiscount.ToString("#,##0.00");
            lbl_NUKActualSuppAmt.Text = domainShipmentDef.CustomerSummary.TotalShippedSupplierGarmentAmount.ToString("#,##0.00");
            lbl_SupplierActualSuppAmt.Text = _shipment.TotalShippedSupplierGarmentAmountAfterDiscount.ToString("#,##0.00");

            //Document
            gv_DocInfo.DataSource = domainShipmentDef.Documents;
            gv_DocInfo.DataBind();
            gv_ILSDocInfo.DataSource = domainShipmentDef.ILSDocuments;
            gv_ILSDocInfo.DataBind();

            //Booking            
            ILSWHDate.Text = DateTimeUtility.getDateString(domainShipmentDef.Invoice.ILSActualAtWarehouseDate);

            //Invoice
            lbl_SellCurrency1.Text = domainShipmentDef.Shipment.SellCurrency.CurrencyCode;
            lbl_SellCurrency2.Text = domainShipmentDef.Shipment.SellCurrency.CurrencyCode;
            lbl_BuyCurrency.Text = domainShipmentDef.Shipment.BuyCurrency.CurrencyCode;
            lbl_BuyCurrency2.Text = domainShipmentDef.Shipment.BuyCurrency.CurrencyCode;
            lbl_BuyCurrency3.Text = domainShipmentDef.Shipment.BuyCurrency.CurrencyCode;
            //lbl_BuyCurrency4.Text = domainShipmentDef.Shipment.BuyCurrency.CurrencyCode;
            lbl_BuyCurrency5.Text = domainShipmentDef.Shipment.BuyCurrency.CurrencyCode;
            lbl_BuyCurrency6.Text = domainShipmentDef.Shipment.BuyCurrency.CurrencyCode;

            if (domainShipmentDef.Invoice != null)
            {
                #region display invoice field
                InvoiceDef _invoice = domainShipmentDef.Invoice;

                if (_invoice.InvoiceDate != null)
                {
                    lbl_NSLActualAtWHDate.Text = DateTimeUtility.getDateString(_invoice.InvoiceDate);
                    lbl_SuppActualAtWHDate.Text = lbl_NSLActualAtWHDate.Text;
                    lbl_InvDate.Text = DateTimeUtility.getDateString(_invoice.InvoiceDate);
                    txt_InvDate.Text = lbl_InvDate.Text;
                }

                #region booking tab
                txt_LotShipOrderNo.Text = _invoice.BookingSONo;
                lbl_LotShipOrderNo.Text = _invoice.BookingSONo;
                txt_BookingDate.Text = DateTimeUtility.getDateString(_invoice.BookingDate);
                lbl_BookingDate.Text = DateTimeUtility.getDateString(_invoice.BookingDate);
                lbl_BookingQty.Text = _invoice.BookingQty.ToString("#,##0");
                txt_BookingQty.Text = _invoice.BookingQty.ToString("#,##0");
                lbl_BkInWHDate.Text = DateTimeUtility.getDateString(_invoice.BookingAtWarehouseDate);
                txt_BkInWHDate.Text = DateTimeUtility.getDateString(_invoice.BookingAtWarehouseDate);
                lbl_ActualInWHDate.Text = DateTimeUtility.getDateString(_invoice.ActualAtWarehouseDate);
                txt_ActualInWHDate.Text = DateTimeUtility.getDateString(_invoice.ActualAtWarehouseDate);
                lbl_NotesToQCC.Text = _invoice.QCCRemark;
                txt_NotesToQCC.Text = _invoice.QCCRemark;
                #endregion

                #region invoice tab
                lbl_InvNo.Text = _invoice.InvoiceNo;
                if (_invoice.ShipFromCountry != null)
                {
                    lbl_ShipFrom.Text = _invoice.ShipFromCountry.ShipmentCountryDescription;
                    ddl_ShipFrom.bindList(WebUtil.getShipmentCountryList(), "ShipmentCountryDescription", "ShipmentCountryId", _invoice.ShipFromCountry.ShipmentCountryId.ToString(), "", GeneralCriteria.ALLSTRING);
                }
                else
                {
                    lbl_ShipFrom.Text = "";
                    ddl_ShipFrom.bindList(WebUtil.getShipmentCountryList(), "ShipmentCountryDescription", "ShipmentCountryId", "", "", GeneralCriteria.ALLSTRING);
                }
                if (_invoice.CustomerDestination != null)
                {
                    lbl_ShipTo.Text = _invoice.CustomerDestination.DestinationDesc;
                    ddl_ShipTo.bindList(WebUtil.getFinalDestinationList(), "DestinationDesc", "CustomerDestinationId", _invoice.CustomerDestination.CustomerDestinationId.ToString(), "", GeneralCriteria.ALLSTRING);
                }
                else
                {
                    lbl_ShipTo.Text = "";
                    ddl_ShipTo.bindList(WebUtil.getFinalDestinationList(), "DestinationDesc", "CustomerDestinationId", "", "", GeneralCriteria.ALLSTRING);
                }

                this.hpl_SupplierInvoiceNo.Visible = false;
                /*
                ArrayList queryStructs = new ArrayList();
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Shipping - Supplier Invoice"));
                queryStructs.Add(new QueryStructDef("Supplier Invoice no.", _invoice.SupplierInvoiceNo));
                ArrayList qList = DMSUtil.queryDocument(queryStructs);
                */

                ArrayList qList = new ArrayList();
                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    foreach (AttachmentInfoDef aDef in docInfoDef.AttachmentInfos)
                    {
                        this.hpl_SupplierInvoiceNo.Visible = true;
                        this.lbl_SupplierInvoiceNoCaption.Visible = false;
                        this.hpl_SupplierInvoiceNo.NavigateUrl = aDef.AttachmentLink;
                    }
                }

                lbl_VendorInvoiceNo.Text = _invoice.SupplierInvoiceNo;
                txt_VendorInvoiceNo.Text = _invoice.SupplierInvoiceNo;
                lbl_ShipRemark.Text = _invoice.ShippingRemark;
                txt_ShipRemark.Text = _invoice.ShippingRemark;
                lbl_InvPiece.Text = _invoice.PiecesPerDeliveryUnit.ToString();
                txt_InvPiece.Text = _invoice.PiecesPerDeliveryUnit.ToString();
                lbl_InvPack.Text = PackingMethodRef.getDeliveryUnitDescription(_invoice.PackingMethod.PackingMethodId);
                if (_invoice.PackingMethod.PackingMethodId == 1)
                    ddl_InvPack.SelectedIndex = 0;
                else
                    ddl_InvPack.SelectedIndex = 1;
                lbl_ItemColor.Text = _invoice.ItemColour;
                txt_ItemColor.Text = _invoice.ItemColour;
                lbl_Desc1.Text = _invoice.ItemDesc1;
                lbl_Desc2.Text = _invoice.ItemDesc2;
                lbl_Desc3.Text = _invoice.ItemDesc3;
                lbl_Desc4.Text = _invoice.ItemDesc4;
                lbl_Desc5.Text = _invoice.ItemDesc5;
                txt_Desc1.Text = _invoice.ItemDesc1;
                txt_Desc2.Text = _invoice.ItemDesc2;
                txt_Desc3.Text = _invoice.ItemDesc3;
                txt_Desc4.Text = _invoice.ItemDesc4;
                txt_Desc5.Text = _invoice.ItemDesc5;
                lbl_MasterItemDesc1.Text = (domainShipmentDef.Product.MasterDescription1 == null ? "" : domainShipmentDef.Product.MasterDescription1);
                lbl_MasterItemDesc2.Text = (domainShipmentDef.Product.MasterDescription2 == null ? "" : domainShipmentDef.Product.MasterDescription2);
                lbl_MasterItemDesc3.Text = (domainShipmentDef.Product.MasterDescription3 == null ? "" : domainShipmentDef.Product.MasterDescription3);
                lbl_MasterItemDesc4.Text = (domainShipmentDef.Product.MasterDescription4 == null ? "" : domainShipmentDef.Product.MasterDescription4);
                lbl_MasterItemDesc5.Text = (domainShipmentDef.Product.MasterDescription5 == null ? "" : domainShipmentDef.Product.MasterDescription5);
                txt_RetailDesc.Text = domainShipmentDef.Product.RetailDescription;

                lbl_ExLicenceFee.Text = _invoice.ExportLicenceFee.ToString("#,##0.00");
                txt_ExLicenceFee.Text = _invoice.ExportLicenceFee.ToString("#,##0.00");
                lbl_QuotaCharge.Text = _invoice.QuotaCharge.ToString("#,##0.00");
                txt_QuotaCharge.Text = _invoice.QuotaCharge.ToString("#,##0.00");
                lbl_TotalDuty.Text = (_shipment.TotalShippedAmount - _invoice.ExportLicenceFee - _invoice.QuotaCharge).ToString("#,##0.00");
                lbl_TotalAmount.Text = _shipment.TotalShippedAmount.ToString("#,##0.00");
                if (_contract.Customer.CustomerId == 13)
                {
                    lbl_InvCommTitle.Text = "NSL-to-GT Commission";
                    lbl_InvCommPercent.Text = _shipment.GTCommissionPercent.ToString();
                    lbl_InvCommAmt.Text = (_shipment.GTCommissionPercent * _shipment.TotalShippedAmount / 100).ToString("#,##0.00");
                }
                else
                {
                    lbl_InvCommTitle.Text = "Customer-to-NSL Commission";
                    lbl_InvCommPercent.Text = _shipment.NSLCommissionPercentage.ToString();
                    lbl_InvCommAmt.Text = _invoice.NSLCommissionAmt.ToString("#,##0.00");
                }

                decimal paymentDeduction = ShipmentManager.Instance.calcShipmentDeductionTotal(vwDomainShipmentDef.PaymentDeduction);
                if (vwDomainShipmentDef.SplitShipments.Count > 0 && vwDomainShipmentDef.Shipment.IsVirtualSetSplit == 0)
                {
                    decimal qaAmt = 0;
                    decimal discountAmt = 0;
                    decimal labTestAmt = 0;
                    decimal supplierAmt = 0;
                    foreach (SplitShipmentDef split in vwDomainShipmentDef.SplitShipments)
                    {
                        if (split.IsVirtualSetSplit == 1)
                            continue;
                        qaAmt += Math.Round(split.TotalShippedSupplierGarmentAmountAfterDiscount * split.QACommissionPercent / 100, 2, MidpointRounding.AwayFromZero);
                        discountAmt += Math.Round(split.TotalShippedSupplierGarmentAmountAfterDiscount * split.VendorPaymentDiscountPercent / 100, 2, MidpointRounding.AwayFromZero);
                        labTestAmt += Math.Round(split.TotalShippedQuantity * split.LabTestIncome, 2, MidpointRounding.AwayFromZero);
                        supplierAmt += split.TotalShippedSupplierGarmentAmountAfterDiscount;
                    }
                    lbl_QACommAmt.Text = qaAmt.ToString("#,##0.00");
                    lbl_DiscountAmt.Text = discountAmt.ToString("#,##0.00");
                    lbl_LabTestIncomeAmt.Text = labTestAmt.ToString("#,##0.00");
                    lbl_PaymentDeduction.Text = paymentDeduction.ToString("#,##0.00");
                    lbl_NetAmt.Text = (supplierAmt - qaAmt - discountAmt - labTestAmt - paymentDeduction).ToString("#,##0.00");
                }
                else
                {
                    lbl_QACommPercent.Text = _shipment.QACommissionPercent.ToString() + "%, ";
                    lbl_QACommAmt.Text = Math.Round(_shipment.TotalShippedSupplierGarmentAmountAfterDiscount * (_shipment.QACommissionPercent / 100), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

                    lbl_DiscountPercent.Text = _shipment.VendorPaymentDiscountPercent.ToString() + "%, ";
                    lbl_DiscountAmt.Text = Math.Round(_shipment.TotalShippedSupplierGarmentAmountAfterDiscount * (_shipment.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

                    lbl_LabTestIncome.Text = _shipment.LabTestIncome.ToString("#,##0.000") + ", ";
                    lbl_LabTestIncomeAmt.Text = Math.Round(_shipment.TotalShippedQuantity * _shipment.LabTestIncome, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

                    lbl_PaymentDeduction.Text = paymentDeduction.ToString("#,##0.00");
                    lbl_NetAmt.Text = (_shipment.TotalShippedSupplierGarmentAmountAfterDiscount -
                        Math.Round((_shipment.TotalShippedSupplierGarmentAmountAfterDiscount * (_shipment.QACommissionPercent / 100)), 2, MidpointRounding.AwayFromZero)
                        - Math.Round(_shipment.TotalShippedSupplierGarmentAmountAfterDiscount * (_shipment.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero)
                        - Math.Round(_shipment.TotalShippedQuantity * _shipment.LabTestIncome, 2, MidpointRounding.AwayFromZero)
                        - Math.Round(paymentDeduction, 2, MidpointRounding.AwayFromZero)
                        ).ToString("#,##0.00");
                }
                /*
                this.lbl_BankChargesPercent.Text = _shipment.AdditionalBankChargesPercent.ToString() + "%, ";
                this.lbl_BankChargesAmt.Text = Math.Round(_shipment.TotalShippedNetFOBAmount * (_shipment.AdditionalBankChargesPercent / 100), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");
                */

                lbl_ShipRecDocDate.Text = DateTimeUtility.getDateString(_invoice.ShippingDocReceiptDate);
                txt_ShipRecDocDate.Text = DateTimeUtility.getDateString(_invoice.ShippingDocReceiptDate);
                lbl_ACRecDocDate.Text = DateTimeUtility.getDateString(_invoice.AccountDocReceiptDate);
                lbl_InvSentDate.Text = DateTimeUtility.getDateString(_invoice.InvoiceSentDate);

                ckb_ShipDocCheck.Checked = (_invoice.ShippingDocCheckedOn != DateTime.MinValue);
                if (_invoice.ShippingCheckedTotalNetAmount == 0)
                    txt_ShipDocCheckAmount.Text = "";
                else
                    txt_ShipDocCheckAmount.Text = _invoice.ShippingCheckedTotalNetAmount.ToString("#,##0.00");
                lbl_ShipDocCheckAmount.Enabled = ckb_ShipDocCheck.Checked;
                lbl_ShipDocCheckAmount.Text = txt_ShipDocCheckAmount.Text;

                //if (_invoice.ShippingCheckedTotalNetAmount == 0 && !ckb_ShipDocCheck.Checked)
                //    txt_ShipDocCheckAmount.Text = "";
                //else
                //    txt_ShipDocCheckAmount.Text = _invoice.ShippingCheckedTotalNetAmount.ToString("#,##0.00");

                if (domainShipmentDef.LCApplication != null && domainShipmentDef.LCApplication.WorkflowStatus != LCWFS.REJECTED)
                {
                    if (domainShipmentDef.LCBatch != null)
                        lbl_LCBatchNo.Text = "LCB" + domainShipmentDef.LCBatch.LCBatchNo.ToString("000000");
                    lbl_AppDate.Text = DateTimeUtility.getDateString(domainShipmentDef.LCApplication.LCApprovalDate);
                    lbl_LCAppNo.Text = domainShipmentDef.LCApplication.LCApplicationNo.ToString().PadLeft(6, '0');

                }
                int lgId = AccountManager.Instance.getLGDetail(_shipment.ShipmentId, 0);
                LetterOfGuaranteeDef lgDef = AccountManager.Instance.getLetterOfGuaranteeByKey(lgId);

                lbl_LGDueDate.Text = (lgId > 0) ? DateTimeUtility.getDateString(LetterOfGuaranteeDef.getLGDueDate(_invoice.InvoiceDate)) : string.Empty;
                lbl_LCNo.Text = _invoice.LCNo;
                txt_LCNo.Text = _invoice.LCNo;
                lbl_LCBillRefNo.Text = _invoice.LCBillRefNo;
                txt_LCBillRefNo.Text = _invoice.LCBillRefNo;
                lbl_IssueDate.Text = DateTimeUtility.getDateString(_invoice.LCIssueDate);
                txt_IssueDate.Text = DateTimeUtility.getDateString(_invoice.LCIssueDate);
                lbl_LCExpiryDate.Text = DateTimeUtility.getDateString(_invoice.LCExpiryDate);
                txt_LCExpiryDate.Text = DateTimeUtility.getDateString(_invoice.LCExpiryDate);
                lbl_LCAmount.Text = _invoice.LCAmount.ToString("#,###.#");
                txt_LCAmount.Text = _invoice.LCAmount.ToString("#,###.#");
                if (domainShipmentDef.LCApplication != null)
                {
                    if (domainShipmentDef.LCApplication.WorkflowStatus.Id == LCWFS.LC_CANCELLED.Id && domainShipmentDef.LCApplication.LCCancellationDate != DateTime.MinValue)
                        lbl_LcCancelDate.Text = DateTimeUtility.getDateString(domainShipmentDef.LCApplication.LCCancellationDate);

                    if (domainShipmentDef.LCApplication.DeducedFabricCost != decimal.MinValue)
                    {
                        lbl_DeductionOnLC.Text = (domainShipmentDef.LCApplication.DeducedFabricCost > 0 ? "Y" : "N");
                        if (domainShipmentDef.LCApplication.DeducedFabricCost > 0)
                            lbl_DeductionAmtInLC.Text = domainShipmentDef.LCApplication.DeducedFabricCost.ToString("#,##0.00");
                        else
                            lbl_DeductionAmtInLC.Text = string.Empty;
                    }
                }

                ckb_PaymentChecked.Checked = (_invoice.IsLCPaymentChecked == 1);
                if (_invoice.IsLCPaymentChecked == 1)
                    lbl_PaymentCheckDate.Text = DateTimeUtility.getDateString(_invoice.LCPaymentCheckedDate);
                else
                    lbl_PaymentCheckDate.Text = "";
                txt_PaymentCheckDate.Text = lbl_PaymentCheckDate.Text;
                bindLCInfoOfOterDly();

                //isChoiceTotalAmountAutoUpdate = (_contract.Customer.CustomerId == CustomerDef.Id.choice.GetHashCode() && "HK,SH,TH".Contains(_contract.Office.OfficeCode));
                //if (_contract.Customer.CustomerId == CustomerDef.Id.choice.GetHashCode() && "BD,IN,ND,LK,PK,TR".Contains(_contract.Office.OfficeCode))
                txt_ChoiceActSalesAmt.Text = _invoice.ChoiceOrderTotalShippedAmount.ToString("#,##0.00");
                txt_ChoiceActPurchaseAmt.Text = _invoice.ChoiceOrderTotalShippedSupplierGarmentAmount.ToString("#,##0.00");
                lbl_ChoiceActSalesAmt.Text = _invoice.ChoiceOrderTotalShippedAmount.ToString("#,##0.00");
                lbl_ChoiceActPurchaseAmt.Text = _invoice.ChoiceOrderTotalShippedSupplierGarmentAmount.ToString("#,##0.00");
                lbl_ChoiceNslCommPercent.Text = _shipment.NSLCommissionPercentage.ToString("#,##0.00");
                lbl_ChoiceNslCommAmt.Text = _invoice.ChoiceOrderNSLCommissionAmount.ToString("#,##0.00");
                if (needToInputChoiceTotalAmount)
                {
                    // Show the CHOICE info on screen
                    tr_ChoicePaymentSeperator.Visible = true;
                    tr_ChoicePaymentRow.Visible = true;
                    tr_choiceNslCommRow.Visible = true;
                }
                else
                {
                    // Will not show the CHOICE info on screen
                    tr_ChoicePaymentSeperator.Visible = false;
                    tr_ChoicePaymentRow.Visible = false;
                    tr_choiceNslCommRow.Visible = false;
                }

                if (_shipment.IsMockShopSample == 1 || _shipment.IsPressSample == 1)
                {
                    row_mockShop.Visible = true;
                }
                lbl_CourierCharge.Text = _invoice.CourierChargeToNUK.ToString();
                txt_CourierCharge.Text = _invoice.CourierChargeToNUK.ToString();
                if (_shipment.IsMockShopSample == 1)
                    lbl_DebitNoteNo.Text = AccountManager.Instance.getMockShopDebitNoteNoByShipmentId(_shipment.ShipmentId);
                else if (_shipment.IsStudioSample == 1)
                    lbl_DebitNoteNo.Text = AccountManager.Instance.getStudioSampleDebitNoteNoByShipmentId(_shipment.ShipmentId);
                txt_DebitNoteNo.Text = lbl_DebitNoteNo.Text;
                string dept = GeneralWorker.Instance.getProductDepartmentByKey(_contract.DeptId).Code;
                // allow to edit only if it is an Non-Clothing order from India-North office
                txt_CourierCharge.ReadOnly = (!_shipment.PaymentLock && _contract.Office.OfficeId == 13 && dept == "NC" ? false : true);

                if ((_contract.Customer.CustomerId == 1 || _contract.Customer.CustomerId == 2) &&
                    CustomerDestinationDef.isDFOrder(_shipment.CustomerDestinationId))
                {
                    row_DirectFranchise.Visible = true;
                    lbl_DFDebitNoteNo.Text = _invoice.DirectFranchiseDebitNoteNo;
                    txt_DFDebitNoteNo.Text = _invoice.DirectFranchiseDebitNoteNo;
                    lbl_DocCharge.Text = _invoice.DirectFranchiseDocumentCharge.ToString();
                    txt_DocCharge.Text = _invoice.DirectFranchiseDocumentCharge.ToString();
                    lbl_TransportCharge.Text = _invoice.DirectFranchiseTransportationCharge.ToString();
                    txt_TransportCharge.Text = _invoice.DirectFranchiseTransportationCharge.ToString();
                }

                //if (CommonManager.Instance.getCustomerDestinationByKey(_shipment.CustomerDestinationId).UTurnOrder == 1)
                if (_shipment.TermOfPurchase.TermOfPurchaseId == TermOfPurchaseRef.Id.FOB_UT.GetHashCode())
                {
                    row_uturn.Visible = (_contract.Customer.CustomerId == CustomerDef.Id.hempel.GetHashCode() && !"HK,SH,DG,FB".Contains(_contract.Office.OfficeCode) ? false : true);
                    lbl_ImportDutyActualAmt.Text = _invoice.ImportDutyActualAmt.ToString();
                    txt_ImportDutyActualAmt.Text = _invoice.ImportDutyActualAmt.ToString();
                    if (_invoice.IsImportDutyChecked == 1)
                    {
                        ckb_ImportDutyRecordChecked.Checked = true;
                        lbl_ImportDutyRecordCheckedDate.Text = DateTimeUtility.getDateString(_invoice.ImportDutyCheckedDate);
                        txt_ImportDutyRecordCheckedDate.Text = DateTimeUtility.getDateString(_invoice.ImportDutyCheckedDate);
                    }
                    else
                    {
                        lbl_ImportDutyRecordCheckedDate.Text = "";
                        ckb_ImportDutyRecordChecked.Checked = false;
                    }
                    lbl_ImportDutyCurrency.Text = _invoice.ImportDutyCurrency.CurrencyCode;
                    lbl_ImportDutyCalcAmt.Text = _invoice.ImportDutyCalculatedAmt.ToString();

                    lbl_InputVATActualAmt.Text = _invoice.InputVATActualAmt.ToString();
                    txt_InputVATActualAmt.Text = _invoice.InputVATActualAmt.ToString();
                    if (_invoice.IsInputVATChecked == 1)
                    {
                        ckb_InputVATRecordChecked.Checked = true;
                        lbl_InputVATRecordCheckedDate.Text = DateTimeUtility.getDateString(_invoice.InputVATCheckedDate);
                        txt_InputVATRecordCheckedDate.Text = DateTimeUtility.getDateString(_invoice.InputVATCheckedDate);
                    }
                    else
                    {
                        lbl_InputVATRecordCheckedDate.Text = "";
                        ckb_InputVATRecordChecked.Checked = false;
                    }
                    lbl_InputVATCurrency.Text = _invoice.InputVATCurrency.CurrencyCode;
                    lbl_InputVATCalcAmt.Text = _invoice.InputVATCalculatedAmt.ToString();

                    lbl_OutputVATActualAmt.Text = _invoice.OutputVATActualAmt.ToString();
                    txt_OutputVATActualAmt.Text = _invoice.OutputVATActualAmt.ToString();
                    if (_invoice.IsOutputVATChecked == 1)
                    {
                        ckb_OutputVATRecordChecked.Checked = true;
                        lbl_OutputVATRecordCheckedDate.Text = DateTimeUtility.getDateString(_invoice.OutputVATCheckedDate);
                    }
                    else
                    {
                        lbl_OutputVATRecordCheckedDate.Text = "";
                        ckb_OutputVATRecordChecked.Checked = false;
                    }
                    txt_OutputVATRecordCheckedDate.Text = lbl_OutputVATRecordCheckedDate.Text;
                    lbl_OutputVATCurrency.Text = _invoice.OutputVATCurrency.CurrencyCode;
                    lbl_OutputVATCalcAmt.Text = _invoice.OutputVATCalculatedAmt.ToString();
                }
                #endregion

                txt_InvRemark.Text = _invoice.InvoiceRemark.Replace("||", Environment.NewLine);

                #region Account tab
                lbl_InvScanDate_COGS.Text = DateTimeUtility.getDateString(_invoice.PurchaseScanDate);
                lbl_SettleDate_COGS.Text = DateTimeUtility.getDateString(_invoice.APDate);
                lbl_SettleAmount_COGS.Text = _invoice.APAmt.ToString("#,##0.00");
                lbl_RefNo_COGS.Text = _invoice.APRefNo;

                lbl_InvScanDate_Sales.Text = DateTimeUtility.getDateString(_invoice.SalesScanDate);
                lbl_SettleDate_Sales.Text = DateTimeUtility.getDateString(_invoice.ARDate);
                lbl_SettleAmount_Sales.Text = _invoice.ARAmt.ToString("#,##0.00");
                lbl_RefNo_Sales.Text = _invoice.ARRefNo;

                lbl_SettleDate_SalesComm.Text = DateTimeUtility.getDateString(_invoice.NSLCommissionSettlementDate);
                lbl_SettleAmount_SalesComm.Text = (_invoice.NSLCommissionSettlementDate == DateTime.MinValue ? "" : _invoice.NSLCommissionSettlementAmt.ToString("#,##0.00"));
                lbl_RefNo_SalesComm.Text = _invoice.NSLCommissionRefNo;

                EInvoiceBatchDef invoiceBatchDef = AccountManager.Instance.getEInvoiceBatchByKey(_invoice.EInvoiceBatchId);
                if (invoiceBatchDef != null)
                {
                    lbl_eInvBatchNo_Sales.Text = invoiceBatchDef.EInvoiceBatchNo;
                    lbl_eInvSubmitDate.Text = DateTimeUtility.getDateString(invoiceBatchDef.SubmittedOn);
                }

                if (domainShipmentDef.COGSSunInterfaceLog != null)
                {
                    lbl_InterfaceDate_COGS.Text = DateTimeUtility.getDateString(AccountManager.Instance.getSunInterfaceQueueByKey(domainShipmentDef.COGSSunInterfaceLog.QueueId).CompleteTime);
                    lbl_InterfacedAmt_COGS.Text = domainShipmentDef.COGSSunInterfaceLog.FullOtherAmount.ToString("#,##0.00");
                }
                if (domainShipmentDef.SalesSunInterfaceLog != null)
                {
                    lbl_InterfaceDate_Sales.Text = DateTimeUtility.getDateString(AccountManager.Instance.getSunInterfaceQueueByKey(domainShipmentDef.SalesSunInterfaceLog.QueueId).CompleteTime);
                    lbl_InterfacedAmt_Sales.Text = domainShipmentDef.SalesSunInterfaceLog.FullOtherAmount.ToString("#,##0.00");
                }
                if (domainShipmentDef.SalesCommSunInterfaceLog != null)
                {
                    lbl_InterfaceDate_SalesCom.Text = DateTimeUtility.getDateString(AccountManager.Instance.getSunInterfaceQueueByKey(domainShipmentDef.SalesCommSunInterfaceLog.QueueId).CompleteTime);
                    lbl_InterfaceAmt_SalesCom.Text = domainShipmentDef.SalesCommSunInterfaceLog.FullOtherAmount.ToString("#,##0.00");
                }
                if (domainShipmentDef.APSunInterfaceLog != null)
                {
                    lbl_InterfaceDate_AP.Text = DateTimeUtility.getDateString(AccountManager.Instance.getSunInterfaceQueueByKey(domainShipmentDef.APSunInterfaceLog.QueueId).CompleteTime);
                    lbl_InterfaceAmt_AP.Text = domainShipmentDef.APSunInterfaceLog.FullOtherAmount.ToString("#,##0.00");
                }
                if (domainShipmentDef.ARSunInterfaceLog != null)
                {
                    lbl_InterfaceDate_AR.Text = DateTimeUtility.getDateString(AccountManager.Instance.getSunInterfaceQueueByKey(domainShipmentDef.ARSunInterfaceLog.QueueId).CompleteTime);
                    lbl_InterfaceAmt_AR.Text = domainShipmentDef.ARSunInterfaceLog.FullOtherAmount.ToString("#,##0.00");
                }
                #endregion

                #endregion

            }
            else
            {
                lbl_Desc1.Text = domainShipmentDef.Product.Description1;
                lbl_Desc2.Text = domainShipmentDef.Product.Description2;
                lbl_Desc3.Text = domainShipmentDef.Product.Description3;
                lbl_Desc4.Text = domainShipmentDef.Product.Description4;
                lbl_Desc5.Text = domainShipmentDef.Product.Description5;
                lbl_MasterItemDesc1.Text = domainShipmentDef.Product.MasterDescription1;
                lbl_MasterItemDesc2.Text = domainShipmentDef.Product.MasterDescription2;
                lbl_MasterItemDesc3.Text = domainShipmentDef.Product.MasterDescription3;
                lbl_MasterItemDesc4.Text = domainShipmentDef.Product.MasterDescription4;
                lbl_MasterItemDesc5.Text = domainShipmentDef.Product.MasterDescription5;
                lbl_RetailDesc.Text = domainShipmentDef.Product.RetailDescription;
            }


            //option
            if (_shipment.IsUKDiscount == 1)
            {
                ckb_IsUKDiscount.Checked = true;
                div_UKDiscountReason.Visible = true;
                txt_UKDiscountReason.Text = CommonManager.Instance.getUKDiscountReasonByKey(_shipment.UKDiscountReasonId).UKDiscountReason;
            }
            else
            {
                ckb_IsUKDiscount.Checked = false;
                div_UKDiscountReason.Visible = false;
            }

            if (domainShipmentDef.Contract.Customer.ShortCode == "SB")
                gv_Options.Columns[SizeOptionColumnId.Colour].HeaderText = "S&B Item No.";
            else
                gv_Options.Columns[SizeOptionColumnId.Colour].HeaderText = "Colour";
            gv_Options.DataSource = domainShipmentDef.ShipmentDetails;
            gv_Options.DataBind();

            if (domainShipmentDef.ILSInvoiceDetail != null && domainShipmentDef.ILSInvoiceDetail.Count > 0)
            {
                gv_ILSOption.DataSource = domainShipmentDef.ILSInvoiceDetail;
                gv_ILSOption.DataBind();
            }
            else
                lnk_ILSInvoice.Visible = false;

            if (domainShipmentDef.ILSPackingListDetail != null && domainShipmentDef.ILSPackingListDetail.Count > 0)
            {
                totalILSQty = 0;
                gv_PackingList.DataSource = domainShipmentDef.ILSPackingListDetail;
                gv_PackingList.DataBind();
            }
            else
                lnk_ILSPackingList.Visible = false;

            gv_SplitShipment.DataSource = domainShipmentDef.SplitShipments;
            gv_SplitShipment.DataBind();

            gv_OtherCost.DataSource = WebUtil.getOtherCostTypeList();
            gv_OtherCost.DataBind();
            if (gv_OtherCost.Rows.Count == 0)
                gv_OtherCost.Visible = false;

            gv_AuditLog.DataSource = domainShipmentDef.ActionHistoryList;
            gv_AuditLog.DataBind();

            gv_Manifest.DataSource = domainShipmentDef.ILSManifestDetail;
            gv_Manifest.DataBind();

            if (domainShipmentDef.OtherDelivery.Count > 1)
            {
                gv_OtherDelivery.DataSource = domainShipmentDef.OtherDelivery;
                gv_OtherDelivery.DataBind();
            }
            else
            {
                //tabOtherDel.Visible = false;      // It causes confusion in showing the content of tab page 'Manifest' & 'Deduction'
                gv_OtherDelivery.DataSource = new ArrayList();
                gv_OtherDelivery.DataBind();
            }
            if (domainShipmentDef.ReversingEntry == null || domainShipmentDef.ReversingEntry.Count == 0)
            {
                lbl_ReversingEntry.Visible = false;
                btn_ReversingEntry.Visible = false;
            }
            else
            {
                gv_ReversingEntry.DataSource = domainShipmentDef.ReversingEntry;
                gv_ReversingEntry.DataBind();
            }

            //Payment Deduction
            if (domainShipmentDef.PaymentDeduction.Count > 0)
            {
                gv_PaymentDeduction.DataSource = domainShipmentDef.PaymentDeduction;
                gv_PaymentDeduction.DataBind();
                //gv_PaymentDeductionUpdate.DataSource = domainShipmentDef.PaymentDeduction;
                //gv_PaymentDeductionUpdate.DataBind();
                tcDeductionTitle.Style.Add(HtmlTextWriterStyle.Color, "red");
                tcDeductionTitle.Style.Add(HtmlTextWriterStyle.FontWeight, "bold");
            }
        }


        #region GridView DataBound Event

        public class SizeOptionColumnId : DomainData
        {
            public static int OptionNo = 0;
            public static int Colour = 1;
            public static int Size = 2;
            public static int OrderQty = 3;
            public static int PoQty = 4;
            public static int ShippedQty = 5;
            public static int ShippedQtyInput = 6;
            public static int RatioPack = 7;
            public static int SellingPrice = 8;
            public static int SellingAmt = 9;

            //// Old
            //SupplierPrice = 10;
            //SupplierAmt = 11;
            //OtherCostAmt = 12;
            //NSL2NUKPrice = 13;
            //MerchGSP = 14;
            //ShippingGSP = 15;
            //ShippingGSPInput = 16;
            //// End of old

            public static int SupplierPrice = 10;
            public static int SupplierAmt = 11;
            public static int SupplierPriceCNY = 12;
            public static int SupplierAmtCNY = 13;
            public static int OtherCostAmt = 14;
            public static int NslToNukPrice = 15;
            /*
            public static int MerchandiserGSP = 16;
            public static int ShippingGSP = 17;
            public static int ShippingGSPInput = 18;
            */
        }

        protected void OptionRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.EmptyDataRow)
            {
                bool hideSupplierAmount = (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.HideSupplierAmountInfo));
                e.Row.Cells[SizeOptionColumnId.SupplierPrice].Style.Add(HtmlTextWriterStyle.Display, (hideSupplierAmount ? "none" : "block"));    // Supplier Price
                e.Row.Cells[SizeOptionColumnId.SupplierAmt].Style.Add(HtmlTextWriterStyle.Display, (hideSupplierAmount ? "none" : "block"));    // Supplier Amount
                e.Row.Cells[SizeOptionColumnId.OtherCostAmt].Style.Add(HtmlTextWriterStyle.Display, (hideSupplierAmount ? "none" : "block"));    // Other Cost Amount

                bool isUT = (vwDomainShipmentDef.Shipment.TermOfPurchase.TermOfPurchaseId == TermOfPurchaseRef.Id.FOB_UT.GetHashCode());
                e.Row.Cells[SizeOptionColumnId.SupplierPriceCNY].Style.Add(HtmlTextWriterStyle.Display, (hideSupplierAmount || !isUT ? "none" : "block"));    // Supplier Price (CNY)
                e.Row.Cells[SizeOptionColumnId.SupplierAmtCNY].Style.Add(HtmlTextWriterStyle.Display, (hideSupplierAmount || !isUT ? "none" : "block"));    // Supplier Amount (CNY)
            }

            if (e.Row.RowType == DataControlRowType.Header)
            {
                bool hasOtherCost = false;
                foreach (ShipmentDetailDef detailDef in vwDomainShipmentDef.ShipmentDetails)
                {
                    if (detailDef.OtherCost != null && detailDef.OtherCost.Count > 0)
                    {
                        hasOtherCost = true;
                        break;
                    }
                }
                if (hasOtherCost)
                {
                    ImageButton btn = (ImageButton)e.Row.Cells[SizeOptionColumnId.OtherCostAmt].FindControl("btn_OtherCostBreakDown");
                    if (btn != null)
                        btn.Visible = true;

                    GridView gv = (GridView)e.Row.Cells[SizeOptionColumnId.OtherCostAmt].FindControl("gv_OtherCostSummary");
                    ArrayList otherCostDetail = null;
                    gv.DataSource = getOtherCostSummary(ref otherCostDetail);
                    gv.DataBind();

                    foreach (GridViewRow row in gv.Rows)
                    {
                        GridView gv_Detail = (GridView)row.Cells[2].FindControl("gv_OtherCostDetail");
                        DataTable dt = (DataTable)otherCostDetail[row.RowIndex];
                        gv_Detail.DataSource = dt;
                        gv_Detail.DataBind();

                        if (vwDomainShipmentDef.Contract.Customer.ShortCode == "GT")
                            gv_Detail.Rows[1].Visible = true;

                        decimal total = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            total += Convert.ToDecimal(dr[4]);
                        }
                        Label lbl = (Label)gv_Detail.FooterRow.FindControl("lbl_TotalOtherCost");
                        lbl.Text = decimal.Round(total, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");
                    }
                    //GridView gv = (GridView)e.Row.Cells[11].Controls[3];
                    //gv.DataSource = vwDomainShipmentDef.ShipmentDetails;
                    //gv.DataBind();
                }
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Cell[9] -- Selling Amt
                decimal total;
                ShipmentDetailDef shipmentDetail = (ShipmentDetailDef)vwDomainShipmentDef.ShipmentDetails[e.Row.RowIndex];

                Label lbl = (Label)e.Row.Cells[SizeOptionColumnId.SellingAmt].Controls[1];
                total = shipmentDetail.SellingPrice * shipmentDetail.ShippedQuantity;
                lbl.Text = total.ToString("#,##0.00");
                totalSellingAmount += total;

                TextBox txt = (TextBox)e.Row.Cells[SizeOptionColumnId.ShippedQtyInput].FindControl("txt_ShipQty");
                txt.Text = shipmentDetail.ShippedQuantity.ToString();


                lbl = (Label)e.Row.Cells[SizeOptionColumnId.SupplierAmt].Controls[1];
                total = shipmentDetail.ReducedSupplierGmtPrice * shipmentDetail.ShippedQuantity;
                lbl.Text = total.ToString("#,##0.00");
                totalSupplierAmount += total;

                // Supplier Price and Amount for UT order
                decimal rate = vwDomainShipmentDef.Shipment.QuarterlyExchangeRate;
                rate = (rate == decimal.MinValue ? 0 : rate);
                decimal priceCNY;
                lbl = (Label)e.Row.Cells[SizeOptionColumnId.SupplierPriceCNY].Controls[1];
                if (rate == 0)
                    priceCNY = 0;
                else
                    priceCNY = decimal.Round((shipmentDetail.ReducedSupplierGmtPrice / vwDomainShipmentDef.Shipment.QuarterlyExchangeRate), 2, MidpointRounding.AwayFromZero);
                lbl.Text = (rate == 0 ? "N/A" : priceCNY.ToString("#,##0.00"));

                lbl = (Label)e.Row.Cells[SizeOptionColumnId.SupplierAmtCNY].Controls[1];
                total = priceCNY * shipmentDetail.ShippedQuantity;
                lbl.Text = (rate == 0 ? "N/A" : total.ToString("#,##0.00"));
                totalSupplierAmountCNY += total;

                //other cost
                lbl = (Label)e.Row.Cells[SizeOptionColumnId.OtherCostAmt].Controls[1];
                lbl.Text = (shipmentDetail.ShippedQuantity * shipmentDetail.TotalShippedOtherCost).ToString();
                totalOtherCost += shipmentDetail.ShippedQuantity * shipmentDetail.TotalShippedOtherCost;

                if (shipmentDetail.SellingPrice != shipmentDetail.ReducedSupplierGmtPrice)
                {
                    lbl = (Label)e.Row.Cells[SizeOptionColumnId.SupplierPrice].FindControl("lbl_FtyPrice");
                    lbl.ForeColor = System.Drawing.Color.SeaGreen;
                    lbl = (Label)e.Row.Cells[SizeOptionColumnId.SupplierAmt].FindControl("lbl_OptionFactoryAmt");
                    lbl.ForeColor = System.Drawing.Color.SeaGreen;

                    lbl = (Label)e.Row.Cells[SizeOptionColumnId.SupplierPriceCNY].FindControl("lbl_FtyPriceCNY");
                    lbl.ForeColor = System.Drawing.Color.SeaGreen;
                    lbl = (Label)e.Row.Cells[SizeOptionColumnId.SupplierAmtCNY].FindControl("lbl_OptionFactoryAmtCNY");
                    lbl.ForeColor = System.Drawing.Color.SeaGreen;
                
                }

                // cell 14
                //lbl = (Label)e.Row.Cells[SizeOptionColumnId.MerchandiserGSP].Controls[1];
                //lbl.Text = (shipmentDetail.GSPFormTypeId == 1 ? "GSP" : (shipmentDetail.GSPFormTypeId == 2 ? "GSP+" : "N/A"));
                // cell 16
                //lbl = (Label)e.Row.Cells[SizeOptionColumnId.ShippingGSPInput].FindControl("lbl_ShipGSP");
                //lbl.Text = (shipmentDetail.ShippingGSPFormTypeId == 1 ? "GSP" : (shipmentDetail.ShippingGSPFormTypeId == 2 ? "GSP+" : "N/A"));

                //SmartDropDownList ddl = (SmartDropDownList)e.Row.Cells[SizeOptionColumnId.ShippingGSPInput].FindControl("ddl_ShippingGSP");
                //ddl.bindList(GSPFormType.getGSPFormTypeList(), "Name", "Id", shipmentDetail.ShippingGSPFormTypeId.ToString());

            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                //Cell[3] -- total order qty
                Label lbl = (Label)e.Row.Cells[SizeOptionColumnId.ShippedQty].FindControl("lbl_TotalOrderQty");
                lbl.Text = vwDomainShipmentDef.Shipment.TotalOrderQuantity.ToString("#,##0");

                //Cell[4] -- total po qty
                lbl = (Label)e.Row.Cells[SizeOptionColumnId.ShippedQty].FindControl("lbl_TotalPOQty");
                lbl.Text = vwDomainShipmentDef.Shipment.TotalPOQuantity.ToString("#,##0");

                //Cell[5] -- total shipped qty
                lbl = (Label)e.Row.Cells[SizeOptionColumnId.ShippedQty].FindControl("lbl_TotalShippedQty");
                lbl.Text = vwDomainShipmentDef.Shipment.TotalShippedQuantity.ToString("#,##0");

                //Cell[9] -- Selling amount total
                lbl = (Label)e.Row.Cells[SizeOptionColumnId.SellingAmt].Controls[1];
                lbl.Text = totalSellingAmount.ToString("#,##0.00");

                //Cell[11] - supplier amount total
                lbl = (Label)e.Row.Cells[SizeOptionColumnId.SupplierAmt].Controls[1];
                lbl.Text = totalSupplierAmount.ToString("#,##0.00");

                //New cell - supplier amount CNY total
                decimal rate = vwDomainShipmentDef.Shipment.QuarterlyExchangeRate;
                rate = (rate == decimal.MinValue ? 0 : rate);

                lbl = (Label)e.Row.Cells[SizeOptionColumnId.SupplierAmtCNY].Controls[1];
                lbl.Text = (rate == 0 ? "N/A" : totalSupplierAmountCNY.ToString("#,##0.00"));

                if (totalSellingAmount != totalSupplierAmount)
                    lbl.ForeColor = System.Drawing.Color.SeaGreen;

                //Cell[12] - Other cost total
                lbl = (Label)e.Row.Cells[SizeOptionColumnId.OtherCostAmt].Controls[1];
                lbl.Text = decimal.Round(totalOtherCost, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");
            }
        }

        protected void SplitShipmentDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                SplitShipmentDef def = (SplitShipmentDef)vwDomainShipmentDef.SplitShipments[e.Row.RowIndex];

                if (def.IsVirtualSetSplit == 1)
                {
                    e.Row.Visible = false;
                    return;
                }

                string defaultReceiptDate = decryptParameter(Request.Params["DefaultReceiptDate"]);

                ImageButton btn = (ImageButton)e.Row.Cells[0].FindControl("btnView");
                btn.Attributes.Add("onclick", openPopupWindowFunction + "('SplitShipmentDetail.aspx?SplitShipmentId=" + encryptParameter(def.SplitShipmentId.ToString()) + "&DefaultReceiptDate=" + encryptParameter(defaultReceiptDate)
                    + "','splitshipmentdetail','width=800,height=600,scrollbars=1,resizable=1,status=1');return false;");

                Label lbl = (Label)e.Row.Cells[1].Controls[1];
                lbl.Text = vwDomainShipmentDef.Contract.ContractNo + def.SplitSuffix +
                    "-" + vwDomainShipmentDef.Shipment.DeliveryNo.ToString();

                lbl = (Label)e.Row.Cells[4].FindControl("lbl_NetAmt");

                decimal paymentDeduction = ShipmentManager.Instance.calcShipmentDeductionTotal(vwDomainShipmentDef.PaymentDeduction);
                lbl_PaymentDeduction.Text = paymentDeduction.ToString("#,##0.00");

                lbl.Text = (def.TotalShippedSupplierGarmentAmountAfterDiscount -
                Math.Round((def.QACommissionPercent / 100) * def.TotalShippedSupplierGarmentAmountAfterDiscount, 2, MidpointRounding.AwayFromZero)
                - Math.Round(def.TotalShippedSupplierGarmentAmountAfterDiscount * (def.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero)
                - Math.Round(def.TotalShippedQuantity * def.LabTestIncome, 2, MidpointRounding.AwayFromZero)
                - Math.Round(paymentDeduction, 2, MidpointRounding.AwayFromZero)
                ).ToString("#,##0.00");

                if (def.ShippingDocReceiptDate == DateTime.MinValue)
                {
                    lbl = (Label)e.Row.Cells[6].FindControl("lbl_DocReceiptDate");
                    lbl.Text = "";
                }
            }
        }

        protected void ILSOptionRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //cell[2] -- shipped quantity
                //cell[3] -- selling price
                //cell[4] -- sales amount
                ILSInvoiceDetailDef ilsInvoiceDetail = null;
                ShipmentDetailDef detailDef = null;
                ILSPackingListDetailDef packListDetailDef = null;
                bool isExistInISAM = false;
                Label lbl;

                ilsInvoiceDetail = (ILSInvoiceDetailDef)vwDomainShipmentDef.ILSInvoiceDetail[e.Row.RowIndex];

                foreach (ShipmentDetailDef def in vwDomainShipmentDef.ShipmentDetails)
                {
                    if (def.SizeOption.SizeOptionNo == ilsInvoiceDetail.OptionNo)
                    {
                        detailDef = def;
                        isExistInISAM = true;
                        break;
                    }
                }

                packListDetailDef = null;
                foreach (ILSPackingListDetailDef packListDef in vwDomainShipmentDef.ILSPackingListDetail)
                {
                    if (packListDef.OptionNo == ilsInvoiceDetail.OptionNo)
                    {
                        packListDetailDef = packListDef;
                        break;
                    }
                    else
                        continue;
                }
                //ilsInvoiceDetail = (ILSInvoiceDetailDef)vwDomainShipmentDef.ILSInvoiceDetail[e.Row.RowIndex];

                lbl = (Label)e.Row.Cells[4].Controls[1];
                decimal total = ilsInvoiceDetail.Qty * ilsInvoiceDetail.Price;
                lbl.Text = Math.Round(total, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

                totalILSSalesAmount += total;
                totalILSQty += ilsInvoiceDetail.Qty;

                if (packListDetailDef != null)
                {
                    lbl = (Label)e.Row.FindControl("lbl_ILSSize");
                    lbl.Text = packListDetailDef.optionDescription;
                }


                if (!isExistInISAM || detailDef.SellingPrice != ilsInvoiceDetail.Price || ilsInvoiceDetail.Qty != detailDef.ShippedQuantity || (packListDetailDef != null && packListDetailDef.optionDescription != detailDef.SizeOption.SizeDescription))
                {
                    lnk_ILSInvoice.BackColor = System.Drawing.Color.FromArgb(255, 204, 255);//System.Drawing.Color.Wheat;
                    e.Row.BackColor = System.Drawing.Color.FromArgb(255, 204, 255);
                    e.Row.Font.Bold = true;
                    e.Row.BorderStyle = BorderStyle.None;
                }
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label lbl = (Label)e.Row.Cells[4].Controls[1];
                lbl.Text = totalILSSalesAmount.ToString("#,##0.00");
                lbl = (Label)e.Row.Cells[2].Controls[1];
                lbl.Text = totalILSQty.ToString("#,##0");

            }

        }


        protected void ILSPackingListRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //cell[2] -- shipped quantity
                //cell[3] -- selling price
                //cell[4] -- sales amount                
                ShipmentDetailDef detailDef = null;
                ILSPackingListDetailDef packListDetailDef = null;
                bool isExistInISAM = false;                

                packListDetailDef = (ILSPackingListDetailDef)vwDomainShipmentDef.ILSPackingListDetail[e.Row.RowIndex];

                foreach (ShipmentDetailDef def in vwDomainShipmentDef.ShipmentDetails)
                {
                    if (def.SizeOption.SizeOptionNo == packListDetailDef.OptionNo)
                    {
                        detailDef = def;
                        isExistInISAM = true;
                        break;
                    }
                }
                
                totalILSQty += packListDetailDef.Qty;


                if (!isExistInISAM || packListDetailDef.Qty != detailDef.ShippedQuantity ||  packListDetailDef.optionDescription != detailDef.SizeOption.SizeDescription)
                {
                    lnk_ILSPackingList.BackColor = System.Drawing.Color.FromArgb(255, 204, 255);
                    e.Row.BackColor = System.Drawing.Color.FromArgb(255, 204, 255);
                    e.Row.Font.Bold = true;
                    e.Row.BorderStyle = BorderStyle.None;
                }
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label lbl = (Label)e.Row.FindControl("lbl_NUKQtyTotal");
                lbl.Text = totalILSQty.ToString("#,##0");
            }
        }


        protected void OtherDeliveryDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContractShipmentListJDef def = (ContractShipmentListJDef)vwDomainShipmentDef.OtherDelivery[e.Row.RowIndex];
                if (def.ShipmentId == this.ShipmentId)
                    e.Row.Visible = false;
                else
                {
                    if (def.InvoiceDate == DateTime.MinValue)
                    {
                        Label lbl = (Label)e.Row.Cells[9].FindControl("lbl_InvoiceDate");
                        lbl.Text = "";
                    }
                }
            }
        }

        protected void ManifestDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnk = (LinkButton)e.Row.Cells[2].Controls[1];
                lnk.Attributes.Add("onclick", openPopupWindowFunction + "('AWBDetail.aspx?ContainerNo=" + encryptParameter(lnk.Text) + "','ManifestDetail','width=700,height=500,scrollbars=1,resizable=1,status=1');return false;");

                Label lbl = null;

                foreach (ILSManifestDef ilsManifest in vwDomainShipmentDef.ILSManifest)
                {
                    if (ilsManifest.ContainerNo == lnk.Text)
                    {
                        lbl = (Label)e.Row.Cells[0].Controls[1];
                        lbl.Text = ilsManifest.VoyageNo;

                        lbl = (Label)e.Row.Cells[1].Controls[1];
                        lbl.Text = ilsManifest.VesselName;

                        lbl = (Label)e.Row.Cells[3].Controls[1];
                        lbl.Text = ilsManifest.PartnerContainerNo;

                        lbl = (Label)e.Row.Cells[5].Controls[1];
                        lbl.Text = DateTimeUtility.getDateString(ilsManifest.DepartDate);
                        
                        lbl = (Label)e.Row.Cells[6].Controls[1];
                        lbl.Text = ilsManifest.DepartPort;
                        lbl = (Label)e.Row.Cells[7].Controls[1];
                        lbl.Text = ilsManifest.ArrivalPort;
                        

                        break;
                    }
                    else
                        continue;
                }
            }
        }

        protected void DocInfoDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DocumentDef docDef = (DocumentDef)vwDomainShipmentDef.Documents[e.Row.RowIndex];

                Label lbl;
                if (docDef.DespatchToUKDate == DateTime.MinValue)
                {
                    lbl = (Label)e.Row.Cells[10].FindControl("lbl_UploadDate");
                    if (lbl != null)
                        lbl.Text = "";
                }

                if (docDef.IssueDate == DateTime.MinValue)
                {
                    lbl = (Label)e.Row.Cells[3].FindControl("lbl_IssueDate");
                    if (lbl != null)
                        lbl.Text = "";
                }

                if (docDef.ExpiryDate == DateTime.MinValue)
                {
                    lbl = (Label)e.Row.Cells[4].FindControl("lbl_ExpiryDate");
                    if (lbl != null)
                        lbl.Text = "";
                }
            }
        }

        protected void DocInfoUpdateDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DocumentDef docDef = (DocumentDef)vwDomainShipmentDef.UpdatedDocuments[e.Row.RowIndex];

                if ((docDef.ShipmentId != vwDomainShipmentDef.Shipment.ShipmentId && docDef.ShipmentId != 0) || (docDef.Status == 0 && docDef.ShipmentId != 0))
                {
                    e.Row.Visible = false;
                    return;
                }

                ImageButton lnk = (ImageButton)e.Row.Cells[0].FindControl("lnk_Copy");
                if (lnk != null)
                {
                    if (docDef.DocId != 0)
                        lnk.Attributes.Add("onclick", "window.showModalDialog('ExportLicenceCopy.aspx?ContractNo=" + encryptParameter(vwDomainShipmentDef.Contract.ContractNo) + "&DlyNo=" + encryptParameter(vwDomainShipmentDef.Shipment.DeliveryNo.ToString()) +
                            "&DocId=" + encryptParameter(docDef.DocId.ToString()) + "&Action=" + encryptParameter("copy") + "' ,'ExportLicence','dialogWidth:800px;dialogHeight:400px;scrollbars:1;resizable:1');");
                    else
                        lnk.Visible = false;
                }


                lnk = (ImageButton)e.Row.Cells[0].FindControl("lnk_Move");
                if (lnk != null)
                {
                    if (docDef.DocId != 0)
                        lnk.Attributes.Add("onclick", "window.showModalDialog('ExportLicenceCopy.aspx?ContractNo=" + encryptParameter(vwDomainShipmentDef.Contract.ContractNo) + "&DlyNo=" + encryptParameter(vwDomainShipmentDef.Shipment.DeliveryNo.ToString()) +
                        "&DocId=" + encryptParameter(docDef.DocId.ToString()) + "&Action=" + encryptParameter("move") + "','ExportLicence','dialogWidth:800px;dialogHeight:400px;scrollbars:1;resizable:1');");
                    else
                        lnk.Visible = false;
                }

                TextBox txt;
                if (docDef.DespatchToUKDate == DateTime.MinValue)
                {
                    txt = (TextBox)e.Row.Cells[11].FindControl("txt_DespatchDate");
                    if (txt != null)
                        txt.Text = "";
                }

                if (docDef.IssueDate == DateTime.MinValue)
                {
                    txt = (TextBox)e.Row.Cells[4].FindControl("txt_IssueDate");
                    if (txt != null)
                        txt.Text = "";
                }

                if (docDef.ExpiryDate == DateTime.MinValue)
                {
                    txt = (TextBox)e.Row.Cells[5].FindControl("txt_ExpiryDate");
                    if (txt != null)
                        txt.Text = "";
                }

                bool isNew = false;
                if (docDef.ShipmentId <= 0)
                    isNew = true;

                if (isNew)
                {
                    SmartDropDownList ddl = (SmartDropDownList)e.Row.Cells[1].Controls[1];
                    ddl.bindList(DocumentType.getDocumentTypeList(), "ShortName", "Id");

                    ddl = (SmartDropDownList)e.Row.Cells[3].FindControl("ddl_DocCountry");
                    ddl.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--", GeneralCriteria.ALL.ToString());

                    ddl = (SmartDropDownList)e.Row.Cells[6].FindControl("ddl_QuotaCat");
                    ddl.bindList(WebUtil.getQuotaCategoryList(), "QuotaCategoryNo", "QuotaCategoryId", "", "--", GeneralCriteria.ALL.ToString());

                    ArrayList packingUnitList = (ArrayList)WebUtil.getPackingUnitList();

                    ddl = (SmartDropDownList)e.Row.Cells[8].FindControl("ddl_QtyOnDocUnit");
                    ddl.bindList(packingUnitList, "OPSKey", "PackingUnitId", "1");

                    ddl = (SmartDropDownList)e.Row.Cells[9].FindControl("ddl_QtyForOrderUnit");
                    ddl.bindList(packingUnitList, "OPSKey", "PackingUnitId", "1");

                    ddl = (SmartDropDownList)e.Row.Cells[10].FindControl("ddl_QtyOnPOUnit");
                    ddl.bindList(packingUnitList, "OPSKey", "PackingUnitId", "1");
                }
                else
                {
                    SmartDropDownList ddl = (SmartDropDownList)e.Row.Cells[1].Controls[1];
                    ddl.bindList(DocumentType.getDocumentTypeList(), "ShortName", "Id", docDef.DocumentType.Id.ToString());

                    ddl = (SmartDropDownList)e.Row.Cells[3].FindControl("ddl_DocCountry");
                    if (docDef.Country != null)
                        ddl.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", docDef.Country.CountryOfOriginId.ToString(), "--", GeneralCriteria.ALL.ToString());
                    else
                        ddl.bindList(CommonUtil.getCountryOfOriginList(), "Name", "CountryOfOriginId", "", "--", GeneralCriteria.ALL.ToString());

                    ddl = (SmartDropDownList)e.Row.Cells[6].FindControl("ddl_QuotaCat");
                    if (docDef.QuotaCategory != null)
                    {
                        ddl.bindList(WebUtil.getQuotaCategoryList(), "QuotaCategoryNo", "QuotaCategoryId", docDef.QuotaCategory.QuotaCategoryId.ToString(), "--", GeneralCriteria.ALL.ToString());
                    }
                    else
                        ddl.bindList(WebUtil.getQuotaCategoryList(), "QuotaCategoryNo", "QuotaCategoryId", "", "--", GeneralCriteria.ALL.ToString());

                    ArrayList packingUnitList = (ArrayList)WebUtil.getPackingUnitList();

                    ddl = (SmartDropDownList)e.Row.Cells[8].FindControl("ddl_QtyOnDocUnit");
                    if (docDef.Unit != null)
                        ddl.bindList(packingUnitList, "OPSKey", "PackingUnitId", docDef.Unit != null ? docDef.Unit.PackingUnitId.ToString() : "-1");

                    ddl = (SmartDropDownList)e.Row.Cells[9].FindControl("ddl_QtyForOrderUnit");
                    ddl.bindList(packingUnitList, "OPSKey", "PackingUnitId", docDef.OrderUnit != null ? docDef.OrderUnit.PackingUnitId.ToString() : "-1");

                    ddl = (SmartDropDownList)e.Row.Cells[10].FindControl("ddl_QtyOnPOUnit");
                    ddl.bindList(packingUnitList, "OPSKey", "PackingUnitId", docDef.POUnit != null ? docDef.POUnit.PackingUnitId.ToString() : "-1");
                }
            }
        }

        protected void ILSDocDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ILSDocumentDef doc = (ILSDocumentDef)vwDomainShipmentDef.ILSDocuments[e.Row.RowIndex];
                Label lbl = null;

                if (doc.StartDate == DateTime.MinValue)
                {
                    lbl = (Label)e.Row.Cells[3].FindControl("lbl_IssueDate");
                    lbl.Text = "";
                }
                if (doc.ExpiryDate == DateTime.MinValue)
                {
                    lbl = (Label)e.Row.Cells[4].FindControl("lbl_ExpiryDate");
                    lbl.Text = "";
                }
                if (doc.ImportDate == DateTime.MinValue)
                {
                    lbl = (Label)e.Row.Cells[8].FindControl("lbl_UploadDate");
                    lbl.Text = "";
                }
            }
        }

        protected void OtherCostDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lbl = (Label)e.Row.Cells[0].FindControl("lbl_OtherCostTypeId");
                Label lblInterfaceDate = (Label)e.Row.Cells[2].FindControl("lbl_OtherCostDate");
                Label lblAmt = (Label)e.Row.Cells[1].FindControl("lbl_Amount");
                int typeId = 0;

                switch (lbl.Text)
                {
                    case "1":
                        typeId = SunInterfaceTypeRef.Id.PackageCost.GetHashCode();
                        break;
                    case "4":
                        typeId = SunInterfaceTypeRef.Id.RepairCost.GetHashCode();
                        break;
                    case "5":
                        typeId = SunInterfaceTypeRef.Id.OutsideLabTestCost.GetHashCode();
                        break;
                    case "6":
                        typeId = SunInterfaceTypeRef.Id.OtherFabricCost.GetHashCode();
                        break;
                    case "7":
                        typeId = SunInterfaceTypeRef.Id.SellingPriceDiscount.GetHashCode();
                        break;
                    case "8":
                        typeId = SunInterfaceTypeRef.Id.FactoryPriceDiscount.GetHashCode();
                        break;
                    case "9":
                        typeId = SunInterfaceTypeRef.Id.PrintDevCost.GetHashCode();
                        break;
                    case "10":
                        typeId = SunInterfaceTypeRef.Id.FreightCost.GetHashCode();
                        break;
                    case "11":
                        typeId = SunInterfaceTypeRef.Id.DutyCost.GetHashCode();
                        break;
                    case "12":
                        typeId = SunInterfaceTypeRef.Id.TransportationCost.GetHashCode();
                        break;
                    case "13":
                        typeId = SunInterfaceTypeRef.Id.FragranceCost.GetHashCode();
                        break;
                    case "14":
                        typeId = SunInterfaceTypeRef.Id.ToolingCost.GetHashCode();
                        break;
                    case "15":
                        typeId = SunInterfaceTypeRef.Id.DesignFee.GetHashCode();
                        break;
                    case "16": 
                        typeId = SunInterfaceTypeRef.Id.GTCommission.GetHashCode(); 
                        break;
                    case "19": 
                        typeId = SunInterfaceTypeRef.Id.FinanceCost.GetHashCode(); 
                        break;
                    case "20":
                        typeId = SunInterfaceTypeRef.Id.AirFreightCost.GetHashCode();
                        break;
                    case "21":
                        typeId = SunInterfaceTypeRef.Id.KitDevelopmentCost.GetHashCode();
                        break;
                    case "24":
                        typeId = SunInterfaceTypeRef.Id.DevelopmentSampleCost.GetHashCode();
                        break;
                    case "25":
                        typeId = SunInterfaceTypeRef.Id.SampleLengthCost.GetHashCode();
                        break;
                    case "26":
                        typeId = SunInterfaceTypeRef.Id.FreightForBodycare.GetHashCode();
                        break;
                    case "27":
                        typeId = SunInterfaceTypeRef.Id.CourierCostForSample.GetHashCode();
                        break;
                    case "28":
                        typeId = SunInterfaceTypeRef.Id.MarginDifference.GetHashCode();
                        break;
                        
                }


                SunInterfaceLogDef sunInterfaceLog = AccountManager.Instance.getInitialLogByShipmentId(typeId, ShipmentId, 0);
                if (sunInterfaceLog != null)
                {
                    lblInterfaceDate.Text = DateTimeUtility.getDateString(sunInterfaceLog.CreatedOn);
                    lblAmt.Text = sunInterfaceLog.OtherAmount.ToString("#,##0.00");
                }

                if (lblAmt.Text == "0.00" || lblAmt.Text == "")
                    e.Row.Visible = false;
            }
        }

        protected void AuditLogRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ActionHistoryDef histDef = (ActionHistoryDef)vwDomainShipmentDef.ActionHistoryList[e.Row.RowIndex];
                ActionHistoryType actionType = histDef.ActionHistoryType;

                Label lbl = (Label)e.Row.Cells[1].FindControl("lbl_ActionType");
                lbl.Text = actionType.Description.Substring(0, actionType.Description.IndexOf("-"));

                lbl = (Label)e.Row.Cells[2].FindControl("lbl_Description");
                lbl.Text = actionType.Description.Substring(actionType.Description.IndexOf("-") + 1);

                lbl = (Label)e.Row.Cells[4].FindControl("lbl_User");
                lbl.Text = (histDef.User == null ? "N/A" : histDef.User.DisplayName);

                if (histDef.SplitShipmentId != 0)
                {
                    lbl = (Label)e.Row.Cells[5].FindControl("lbl_SplitShipment");

                    foreach (SplitShipmentDef split in vwDomainShipmentDef.SplitShipments)
                    {
                        if (split.SplitShipmentId == histDef.SplitShipmentId && split.IsVirtualSetSplit == 0)
                        {
                            lbl.Text = vwDomainShipmentDef.Contract.ContractNo + split.SplitSuffix;
                            break;
                        }
                    }
                }

                if (histDef.ActionHistoryType.Id == ActionHistoryType.INVOICE_SENT.Id)
                {
                    // Read DMS Doc.
                    Image img = (Image)e.Row.Cells[3].FindControl("img_ReadDmsDoc");
                    img.Visible = true;
                    img.Attributes.Add("onclick", openPopupWindowFunction + "('../shipping/AttachmentList.aspx?ShipmentId=" + WebUtil.EncryptParameter(histDef.ShipmentId.ToString()) + "', 'attachmentlist', 'width=400,height=300,scrollbars=1,status=0');return false;");
                }

            }
        }

        protected void DiscrepancyLogRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ActionHistoryDef histDef = (ActionHistoryDef)e.Row.DataItem;

                Label lbl = (Label)e.Row.FindControl("lbl_DeliveredBy");
                lbl.Text = histDef.Remark.Replace("Sent by ", "");
            }
        }

        protected void gv_ReversingEntry_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lbl = (Label)e.Row.Cells[0].FindControl("lbl_type");
                lbl.Text = SunInterfaceTypeRef.getDescription(((SunInterfaceLogDef)vwDomainShipmentDef.ReversingEntry[e.Row.RowIndex]).SunInterfaceTypeId);
                lbl = (Label)e.Row.Cells[0].FindControl("lbl_InterfacedDate");
                lbl.Text = DateTimeUtility.getDateString(AccountManager.Instance.getSunInterfaceQueueByKey(((SunInterfaceLogDef)vwDomainShipmentDef.ReversingEntry[e.Row.RowIndex]).QueueId).CompleteTime);
            }
        }

        protected void PaymentDeductionDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ShipmentDeductionDef deduction = (ShipmentDeductionDef)vwDomainShipmentDef.PaymentDeduction[e.Row.RowIndex];

                if (deduction.ShipmentId == 0 || deduction.Status == 0)
                {
                    e.Row.Visible = false;
                }
                else
                {
                    Label lblDeductType = (Label)e.Row.FindControl("lbl_DeductionType");
                    Label lblDocNo = (Label)e.Row.FindControl("lbl_DocNo");
                    Label lblC19Info = (Label)e.Row.FindControl("lbl_C19Info");
                    Label lblAmt = (Label)e.Row.FindControl("lbl_Amount");

                    if (deduction != null)
                    {
                        if (lblDeductType != null)
                            if (deduction.DeductionType.Id == int.MinValue)
                                lblDeductType.Text = "";

                        if (lblDocNo != null)
                        {
                            if (!deduction.DeductionType.RequireDocumentNo)
                                lblDocNo.Style.Add(HtmlTextWriterStyle.Display, "none");
                            if (deduction.DocumentNo == string.Empty)
                                lblDocNo.Text = "";
                        }

                        if (lblC19Info != null)
                        {
                            lblC19Info.Text = string.Empty;
                            if (!deduction.DeductionType.RequireDocumentNo)
                                lblC19Info.Style.Add(HtmlTextWriterStyle.Display, "none");

                            if (deduction.DeductionType.Id == PaymentDeductionType.FABRIC_ADVANCE.Id)
                            {
                                List<AdvancePaymentDef> advPay = AccountManager.Instance.getAdvancePaymentByCriteria(-1, -1, deduction.DocumentNo, vwDomainShipmentDef.Contract.ContractNo, "", DateTime.MinValue, DateTime.MinValue, 1);
                                if (advPay.Count > 0)
                                    if (advPay[0].IsC19 == 1)
                                        lblC19Info.Text = advPay[0].FLRefNo;
                            }
                        }

                        if (lblAmt != null)
                            if (deduction.Amount == decimal.MinValue)
                                lblAmt.Text = "";
                    }
                    else
                    {
                        lblDeductType.Text = "";
                        lblDocNo.Text = "";
                        lblAmt.Text = "";
                        lblC19Info.Text = "";
                    }
                }
            }
        }

        protected void PaymentDeductionUpdateDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                PaymentDeductionUpdateRowControl(e.Row);
        }

        protected void PaymentDeductionUpdateRowControl(GridViewRow row)
        {
            ShipmentDeductionDef deduction = (ShipmentDeductionDef)vwDomainShipmentDef.UpdatedPaymentDeduction[row.RowIndex];

            if ((deduction.ShipmentId != vwDomainShipmentDef.Shipment.ShipmentId && deduction.ShipmentId != 0) || deduction.Status == 0)
            {
                row.Visible = false;
            }
            else
            {
                SmartDropDownList ddl = (SmartDropDownList)row.FindControl("ddl_DeductionType");
                TextBox txtDocNo = (TextBox)row.FindControl("txt_DocNo");
                TextBox txtAmt = (TextBox)row.FindControl("txt_Amount");
                TextBox txtRemark = (TextBox)row.FindControl("txt_Remark");
                TextBox txtDeductionType = (TextBox)row.FindControl("txt_DeductionType");
                Label lblC19Info = (Label)row.FindControl("lbl_C19Info");
                ImageButton btnDelete = (ImageButton)row.FindControl("lnk_DeletePaymentDeduction");

                PaymentDeductionEdit input = null;
                if (vw_PaymentDeductionEdit != null && vw_PaymentDeductionEdit.ContainsKey(row.DataItemIndex))
                    input = vw_PaymentDeductionEdit[row.RowIndex];

                if (deduction.DeductionType == null)
                    if (ddl != null)
                        ddl.selectByText("N/A");
                ListItemCollection deductionTypeList = new ListItemCollection();
                string selectedValue = "0";
                foreach (PaymentDeductionType dt in PaymentDeductionType.GetDeductionTypeList())
                {   // Exclude the 'Fabric Advance' in the deduction type dropdown list
                    if (dt != PaymentDeductionType.FABRIC_ADVANCE || dt.Id == deduction.DeductionType.Id)
                    {
                        ListItem itm = new ListItem();
                        itm.Value = dt.Id.ToString() + (dt.RequireDocumentNo ? ".0" : "");
                        itm.Text = dt.Name;
                        deductionTypeList.Add(itm);
                        if (dt.Id == deduction.DeductionType.Id)
                            selectedValue = itm.Value;
                    }
                }
                if (input != null && selectedValue != input.DeductionType)
                    selectedValue = input.DeductionType;
                ddl.bindList(deductionTypeList, "Text", "Value", selectedValue);
                PaymentDeductionType deductionType = PaymentDeductionType.getType(getPaymentDeductionTypeId(selectedValue));

                if (txtDocNo != null)
                {
                    if (deductionType.RequireDocumentNo && deduction.DocumentNo != string.Empty)
                        txtDocNo.Text = deduction.DocumentNo;
                    else
                        txtDocNo.Text = string.Empty;
                    if (input != null && txtDocNo.Text != input.DocumentNo)
                        txtDocNo.Text = input.DocumentNo;
                    txtDocNo.Style.Add(HtmlTextWriterStyle.Display, (deductionType.RequireDocumentNo ? "" : "none"));
                }

                if (lblC19Info != null)
                {
                    lblC19Info.Text = string.Empty;
                    if (deductionType.Id == PaymentDeductionType.FABRIC_ADVANCE.Id)
                    {
                        List<AdvancePaymentDef> advPay = AccountManager.Instance.getAdvancePaymentByCriteria(-1, -1, deduction.DocumentNo, vwDomainShipmentDef.Contract.ContractNo, "", DateTime.MinValue, DateTime.MinValue, 1);
                        if (advPay.Count > 0)
                            if (advPay[0].IsC19 == 1)
                                lblC19Info.Text = advPay[0].FLRefNo;
                    }
                }

                if (txtAmt != null)
                {
                    //bool enableAmt = deductionType != PaymentDeductionType.NOT_AVAILIABLE && deductionType != PaymentDeductionType.REMARK;
                    //if (enableAmt && deduction.Amount != decimal.MinValue)
                    if (deductionType != PaymentDeductionType.NOT_AVAILIABLE && deduction.Amount != decimal.MinValue)
                        txtAmt.Text = deduction.Amount.ToString();
                    else
                        txtAmt.Text = string.Empty;
                    if (input != null)
                    {
                        decimal amt;
                        if (!decimal.TryParse(input.Amount, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out amt))
                            txtAmt.Text = input.Amount;
                    }
                    txtAmt.Style.Add(HtmlTextWriterStyle.Display, (deductionType == PaymentDeductionType.NOT_AVAILIABLE ? "none" : ""));

                    if (deductionType == PaymentDeductionType.REMARK)
                        txtAmt.ReadOnly = true;

                    //if (deductionType == PaymentDeductionType.REMARK)
                    //{
                    //    txtAmt.Text = "0";
                    //    txtAmt.ReadOnly = true;
                    //}
                }

                if (txtRemark != null)
                {
                    txtRemark.Text = (deductionType != PaymentDeductionType.NOT_AVAILIABLE ? deduction.Remark : string.Empty);
                    if (input != null)
                        txtRemark.Text = input.Remark;
                    txtRemark.Style.Add(HtmlTextWriterStyle.Display, (deductionType == PaymentDeductionType.NOT_AVAILIABLE ? "none" : ""));
                }

                if (deductionType.Id == PaymentDeductionType.FABRIC_ADVANCE.Id || ckb_ShipDocCheck.Checked)
                {   // set ReadOnly on Deduction Type & Doc No text box
                    if (ddl != null)
                        ddl.Style.Add(HtmlTextWriterStyle.Display, "none");
                    if (txtDeductionType != null)
                    {
                        txtDeductionType.Style.Add(HtmlTextWriterStyle.Display, "");
                        txtDeductionType.Style.Add(HtmlTextWriterStyle.BackgroundColor, System.Drawing.Color.LightGray.Name);
                        txtDeductionType.ReadOnly = true;
                    }
                    if (txtDocNo != null)
                    {
                        txtDocNo.Style.Add(HtmlTextWriterStyle.BackgroundColor, System.Drawing.Color.Gainsboro.Name);
                        txtDocNo.ReadOnly = true;
                    }
                    if (ckb_ShipDocCheck.Checked)
                    {
                        txtAmt.Style.Add(HtmlTextWriterStyle.BackgroundColor, System.Drawing.Color.Gainsboro.Name);
                        txtAmt.ReadOnly = true;
                        ImageButton btnAdd = (ImageButton)gv_PaymentDeductionUpdate.HeaderRow.FindControl("lnk_AddPaymentDeduction");
                        btnAdd.Visible = false;

                    }
                    btnDelete.Visible = false;
                }
                else
                    if (txtDeductionType != null)
                        txtDeductionType.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
        }


        private int getPaymentDeductionTypeId(string dropdownListSelectValue)
        {
            return (int)decimal.Parse((dropdownListSelectValue == "" ? "0" : dropdownListSelectValue));
        }
        #endregion


        #region GridView RowCommand

        protected void OtherDeliveryRowCommand(object sender, GridViewCommandEventArgs arg)
        {
            if (arg.CommandName == "OtherDelivery")
            {
                int shipmentId = Convert.ToInt32(arg.CommandArgument);
                Response.Redirect("ShipmentDetail.aspx?ShipmentId=" + encryptParameter(shipmentId.ToString()));
            }
        }

        protected void CusDocRowCommand(object sender, GridViewCommandEventArgs arg)
        {
            if (arg.CommandName == "Add")
            {
                if (getDocumentUpdates())
                {
                    vwDomainShipmentDef.UpdatedDocuments.Add(new DocumentDef());

                    gv_DocInfoUpdate.DataSource = vwDomainShipmentDef.UpdatedDocuments;
                    gv_DocInfoUpdate.DataBind();
                }
            }
            else if (arg.CommandName == "Copy")
            {
                if (Session[ShipmentCommander.Param.documents.ToString()] != null)
                {
                    vwDomainShipmentDef.UpdatedDocuments.Add(Session[ShipmentCommander.Param.documents.ToString()]);
                    Session[ShipmentCommander.Param.documents.ToString()] = null;
                }
            }
            else if (arg.CommandName == "Move")
            {
                if (Session[ShipmentCommander.Param.documents.ToString()] != null)
                {
                    DocumentDef updatedDoc = (DocumentDef)Session[ShipmentCommander.Param.documents.ToString()];
                    vwDomainShipmentDef.UpdatedDocuments.Add(updatedDoc);
                    Session[ShipmentCommander.Param.documents.ToString()] = null;

                    foreach (DocumentDef docDef in vwDomainShipmentDef.UpdatedDocuments)
                    {
                        if (docDef.DocId == updatedDoc.DocId)
                        {
                            docDef.Status = 0;
                            updatedDoc.DocId = 0;
                            break;
                        }
                    }

                    if (getDocumentUpdates())
                    {
                        gv_DocInfoUpdate.DataSource = vwDomainShipmentDef.UpdatedDocuments;
                        gv_DocInfoUpdate.DataBind();

                        if (vwDomainShipmentDef.UpdatedDocuments.Count == 0)
                            btn_NewDoc.Visible = true;
                    }
                }
            }
        }

        protected void PaymentDeductionRowCommand(object sender, GridViewCommandEventArgs arg)
        {
            if (arg.CommandName == "Add")
            {
                //if (getPaymentDeductionUpdates())
                fillPaymentDeductionUpdates();
                {
                    ShipmentDeductionDef def = new ShipmentDeductionDef();
                    def.DeductionType = PaymentDeductionType.NOT_AVAILIABLE;
                    def.DocumentNo = string.Empty;
                    def.Amount = 0;
                    def.Remark = "";
                    def.Status = 1;
                    vwDomainShipmentDef.UpdatedPaymentDeduction.Add(def);

                    gv_PaymentDeductionUpdate.DataSource = vwDomainShipmentDef.UpdatedPaymentDeduction;
                    gv_PaymentDeductionUpdate.DataBind();
                }
            }
            else if (arg.CommandName == "ChangeType")
            {
                object obj = Convert.ToInt32(arg.CommandArgument);
            }
            //else if (arg.CommandName == "Import")
            //{
            //  if (getPaymentDeductionUpdates())
            //    {
            //
            //    }
            //}
        }
        
        #endregion


        #region GridView RowEditing / Updating

        protected void CusDocRowUpdating(object sender, GridViewUpdateEventArgs arg)
        {
            DocumentDef docDef;
            if (vwDomainShipmentDef.Documents.Count > arg.RowIndex)
                docDef = (DocumentDef)vwDomainShipmentDef.Documents[arg.RowIndex];
            else
                docDef = new DocumentDef();
            DateTime date;
            int num = 0;

            Label lbl = (Label)gv_DocInfo.Rows[arg.RowIndex].Cells[1].FindControl("lbl_DocId");
            docDef.DocId = lbl.Text == "" ? int.MinValue : int.Parse(lbl.Text);

            SmartDropDownList ddl = (SmartDropDownList)gv_DocInfo.Rows[arg.RowIndex].Cells[2].FindControl("ddl_DocType");
            docDef.DocumentType = DocumentType.getType(int.Parse(ddl.SelectedValue));

            TextBox txt = (TextBox)gv_DocInfo.Rows[arg.RowIndex].Cells[3].FindControl("txt_DocNo");
            if (txt.Text.Trim() == "")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Please provide document no.');", true);
                return;
            }
            docDef.DocumentNo = txt.Text;

            ddl = (SmartDropDownList)gv_DocInfo.Rows[arg.RowIndex].Cells[4].FindControl("ddl_DocCountry");
            CountryOfOriginRef co = new CountryOfOriginRef();
            co.CountryOfOriginId = Convert.ToInt32(ddl.SelectedValue);
            co.Name = ddl.selectedText;
            docDef.Country = co;

            txt = (TextBox)gv_DocInfo.Rows[arg.RowIndex].Cells[5].FindControl("txt_IssueDate");
            if (docDef.DocumentType.Id == DocumentType.CERTIFICATE_OF_ORIGIN.Id && txt.Text.Trim() == "")
            {
                date = DateTime.MinValue;
            }
            else if (!DateTime.TryParse(txt.Text, null, System.Globalization.DateTimeStyles.None, out date))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Issue Date is not in correct format.');", true);
                return;
            }
            docDef.IssueDate = date;

            txt = (TextBox)gv_DocInfo.Rows[arg.RowIndex].Cells[6].FindControl("txt_ExpiryDate");
            if (docDef.DocumentType.Id == DocumentType.CERTIFICATE_OF_ORIGIN.Id && txt.Text.Trim() == "")
                date = DateTime.MinValue;
            else if (!DateTime.TryParse(txt.Text, null, System.Globalization.DateTimeStyles.None, out date))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Expiry Date is not in correct format.');", true);
                return;
            }
            docDef.ExpiryDate = date;

            ddl = (SmartDropDownList)gv_DocInfo.Rows[arg.RowIndex].Cells[7].FindControl("ddl_QuotaCat");
            docDef.QuotaCategory = new QuotaCategoryRef(int.Parse(ddl.SelectedValue), "", ddl.selectedText);

            txt = (TextBox)gv_DocInfo.Rows[arg.RowIndex].Cells[8].FindControl("txt_Weight");
            if (txt.Text == "" || !int.TryParse(txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out num) || num == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid Weight.');", true);
                return;
            }

            docDef.Weight = num;

            txt = (TextBox)gv_DocInfo.Rows[arg.RowIndex].Cells[9].FindControl("txt_QtyOnDoc");
            if (txt.Text == "" || !int.TryParse(txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out num) || num == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid Qty on Doc.');", true);
                return;
            }
            docDef.Qty = num;
            ddl = (SmartDropDownList)gv_DocInfo.Rows[arg.RowIndex].Cells[9].FindControl("ddl_QtyOnDocUnit");
            docDef.Unit = new PackingUnitRef(int.Parse(ddl.SelectedValue), "", ddl.selectedText);

            txt = (TextBox)gv_DocInfo.Rows[arg.RowIndex].Cells[10].FindControl("txt_QtyForOrder");
            if (txt.Text == "" || !int.TryParse(txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out num) || num == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid Qty for Order.');", true);
                return;
            }
            docDef.OrderQty = num;
            ddl = (SmartDropDownList)gv_DocInfo.Rows[arg.RowIndex].Cells[10].FindControl("ddl_QtyForOrderUnit");
            docDef.OrderUnit = new PackingUnitRef(int.Parse(ddl.SelectedValue), "", ddl.selectedText);

            txt = (TextBox)gv_DocInfo.Rows[arg.RowIndex].Cells[11].FindControl("txt_EqvPOQty");
            if (txt.Text == "" || !int.TryParse(txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out num) || num == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid PO Qty.');", true);
                return;
            }
            docDef.POQty = num;
            ddl = (SmartDropDownList)gv_DocInfo.Rows[arg.RowIndex].Cells[11].FindControl("ddl_QtyOnPOUnit");
            docDef.POUnit = new PackingUnitRef(int.Parse(ddl.SelectedValue), "", ddl.selectedText);

            txt = (TextBox)gv_DocInfo.Rows[arg.RowIndex].Cells[12].FindControl("txt_DespatchDate");
            if (txt.Text != "")
            {
                if (!DateTime.TryParse(txt.Text, null, System.Globalization.DateTimeStyles.None, out date))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid Despatch Date.');", true);
                    return;
                }
                docDef.DespatchToUKDate = date;
            }
            else
            {
                docDef.DespatchToUKDate = DateTime.MinValue;
            }

            txt = (TextBox)gv_DocInfo.Rows[arg.RowIndex].Cells[13].FindControl("txt_AWBNo");
            docDef.DespatchAWBNo = txt.Text;

            docDef.Status = 1;
            docDef.ShipmentId = vwDomainShipmentDef.Shipment.ShipmentId;

            if (vwDomainShipmentDef.UpdatedDocuments == null)
                vwDomainShipmentDef.UpdatedDocuments = new ArrayList();

            if (!vwDomainShipmentDef.UpdatedDocuments.Contains(docDef))
            {
                vwDomainShipmentDef.UpdatedDocuments.Add(docDef);
            }
            vwDomainShipmentDef.Documents[arg.RowIndex] = docDef;
            gv_DocInfo.EditIndex = -1;
            gv_DocInfo.DataSource = vwDomainShipmentDef.Documents;
            gv_DocInfo.DataBind();
        }

        protected void CusDocRowCancelEdit(object sender, GridViewCancelEditEventArgs arg)
        {
            gv_DocInfo.EditIndex = -1;
            DocumentDef def = (DocumentDef)vwDomainShipmentDef.Documents[arg.RowIndex];
            if (def.DocId == 0)
            {
                vwDomainShipmentDef.Documents.RemoveAt(arg.RowIndex);
            }
            gv_DocInfo.DataSource = vwDomainShipmentDef.Documents;
            gv_DocInfo.DataBind();
        }

        protected void CusDocRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            DocumentDef def = (DocumentDef)vwDomainShipmentDef.UpdatedDocuments[arg.RowIndex];
            if (def.DocId == 0)
                vwDomainShipmentDef.UpdatedDocuments.Remove(def);
            else
                def.Status = 0;


            gv_DocInfoUpdate.DataSource = vwDomainShipmentDef.UpdatedDocuments;
            gv_DocInfoUpdate.DataBind();

        }
        
        protected void PaymentDeductionRowUpdating(object sender, GridViewUpdateEventArgs arg)
        {
            ShipmentDeductionDef deduction;
            decimal amt;
            if (vwDomainShipmentDef.PaymentDeduction.Count > arg.RowIndex)
                deduction = (ShipmentDeductionDef)vwDomainShipmentDef.PaymentDeduction[arg.RowIndex];
            else
                deduction = new ShipmentDeductionDef();

            Label lbl = (Label)gv_PaymentDeduction.Rows[arg.RowIndex].Cells[0].FindControl("lbl_ShipmentId");
            deduction.ShipmentId = lbl.Text == "" ? int.MinValue : int.Parse(lbl.Text);

            SmartDropDownList ddl = (SmartDropDownList)gv_PaymentDeduction.Rows[arg.RowIndex].Cells[1].FindControl("ddl_DeductionType");
            //deduction.DeductionType = PaymentDeductionType.getType(int.Parse(ddl.SelectedValue));
            deduction.DeductionType = PaymentDeductionType.getType(getPaymentDeductionTypeId(ddl.SelectedValue));

            TextBox txt = (TextBox)gv_PaymentDeduction.Rows[arg.RowIndex].Cells[2].FindControl("txt_DocNo");
            if (txt.Text.Trim() == "")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Please provide document no.');", true);
                return;
            }
            deduction.DocumentNo = txt.Text;
            txt.Style.Add(HtmlTextWriterStyle.Display, (deduction.DeductionType.RequireDocumentNo ? "" : "none"));

            txt = (TextBox)gv_PaymentDeduction.Rows[arg.RowIndex].Cells[4].FindControl("txt_Remark");
            deduction.Remark = txt.Text;

            txt = (TextBox)gv_PaymentDeduction.Rows[arg.RowIndex].Cells[3].FindControl("txt_Amt");
            if (! decimal.TryParse(txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out amt))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid Amount on Payment Deduction.');", true);
                return;
            }
            else if (txt.Text == "" || amt == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Please provide deduction amount.');", true);
                return;
            }

            txt.Style.Add(HtmlTextWriterStyle.Display, (deduction.DeductionType.Id == 0 ? "none" : ""));
            deduction.Amount = amt;
            deduction.Status = 1;
            deduction.ShipmentId = vwDomainShipmentDef.Shipment.ShipmentId;

            if (vwDomainShipmentDef.UpdatedPaymentDeduction == null)
                vwDomainShipmentDef.UpdatedPaymentDeduction = new ArrayList();

            if (!vwDomainShipmentDef.UpdatedPaymentDeduction.Contains(deduction))
            {
                vwDomainShipmentDef.UpdatedPaymentDeduction.Add(deduction);
            }
            vwDomainShipmentDef.PaymentDeduction[arg.RowIndex] = deduction.Clone();
            //ShipmentDeductionDef def = new ShipmentDeductionDef();
            //def.DeductionType = deduction.DeductionType;
            //def.DocumentNo = deduction.DocumentNo;
            //def.Amount = deduction.Amount;
            //def.Status = deduction.Status;
            //def.ShipmentId = deduction.ShipmentId;
            //vwDomainShipmentDef.PaymentDeduction[arg.RowIndex] = def;

            gv_PaymentDeduction.EditIndex = -1;
            gv_PaymentDeduction.DataSource = vwDomainShipmentDef.PaymentDeduction;
            gv_PaymentDeduction.DataBind();
        }
        
        protected void PaymentDeductionRowCancelEdit(object sender, GridViewCancelEditEventArgs arg)
        {
            gv_PaymentDeduction.EditIndex = -1;
            ShipmentDeductionDef deduction = (ShipmentDeductionDef)vwDomainShipmentDef.PaymentDeduction[arg.RowIndex];
            if (deduction.ShipmentId == 0)
                vwDomainShipmentDef.PaymentDeduction.RemoveAt(arg.RowIndex);
            gv_PaymentDeduction.DataSource = vwDomainShipmentDef.PaymentDeduction;
            gv_PaymentDeduction.DataBind();
        }
        
        protected void PaymentDeductionRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            ShipmentDeductionDef deduction = (ShipmentDeductionDef)vwDomainShipmentDef.UpdatedPaymentDeduction[arg.RowIndex];
            if (deduction.ShipmentId == 0)
                vwDomainShipmentDef.UpdatedPaymentDeduction.Remove(deduction);
            else
                deduction.Status = 0;
            fillPaymentDeductionUpdates();
            gv_PaymentDeductionUpdate.DataSource = vwDomainShipmentDef.UpdatedPaymentDeduction;
            gv_PaymentDeductionUpdate.DataBind();
        }

        protected void LCInfoOfOtherDlyRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DomainShipmentDef otherDly = (DomainShipmentDef)e.Row.DataItem;
                Label lbl;
                lbl = (Label)e.Row.FindControl("lbl_LcBatchNo");
                lbl.Text = (otherDly.LCBatch != null ? otherDly.LCBatch.LCBatchNo.ToString("LCB00000#") : string.Empty);
                lbl = (Label)e.Row.FindControl("lbl_LcIssuedDate");
                lbl.Text = (otherDly.Invoice.LCIssueDate != DateTime.MinValue ? DateTimeUtility.getDateString(otherDly.Invoice.LCIssueDate) : string.Empty);
                lbl = (Label)e.Row.FindControl("lbl_LcExpiryDate");
                lbl.Text = (otherDly.Invoice.LCExpiryDate != DateTime.MinValue ? DateTimeUtility.getDateString(otherDly.Invoice.LCExpiryDate) : string.Empty);
                lbl = (Label)e.Row.FindControl("lbl_LcAmount");
                lbl.Text = (!(string.IsNullOrEmpty(otherDly.Invoice.LCNo) && otherDly.Invoice.LCAmount == 0) ? otherDly.Invoice.LCAmount.ToString("#,##0.00") : string.Empty);
                lbl = (Label)e.Row.FindControl("lbl_LcApplicationNo");
                lbl.Text = (otherDly.LCApplication != null ? otherDly.LCApplication.LCApplicationNo.ToString("00000#") : string.Empty);
                lbl = (Label)e.Row.FindControl("lbl_PoStatus");
                lbl.Text = (otherDly.Shipment.WorkflowStatus.Name);
            }
        }


        protected void bindLCInfoOfOterDly()
        {
            ArrayList otherDelivery = new ArrayList();
            foreach (ContractShipmentListJDef delivery in vwDomainShipmentDef.OtherDelivery)
            {
                //if (delivery.ShipmentId != vwDomainShipmentDef.Shipment.ShipmentId)
                //{
                DomainShipmentDef def = ShipmentManager.Instance.getDomainShipmentDef(delivery.ShipmentId);
                otherDelivery.Add(def);
                //}
            }
            gv_LCInfoOfOtherDly.DataSource = otherDelivery;
            gv_LCInfoOfOtherDly.DataBind();
        }

        #endregion


        #region Button Event

        protected void btn_Edit_Click(object sender, EventArgs e)
        {
            ArrayList arr = new ArrayList();
            foreach (DocumentDef docDef in vwDomainShipmentDef.Documents)
            {
                arr.Add(docDef.Clone());
            }
            vwDomainShipmentDef.UpdatedDocuments = arr;

            ArrayList copyDeduction = new ArrayList();
            foreach (ShipmentDeductionDef def in vwDomainShipmentDef.PaymentDeduction)
                copyDeduction.Add(def.Clone());
            vwDomainShipmentDef.UpdatedPaymentDeduction = copyDeduction;
            vw_PaymentDeductionEdit = null;

            SetControl(true);
        }

        protected void btn_AddDoc_Click(object sender, EventArgs e)
        {

        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (ddl_InvTemplate.SelectedIndex == 0)
            {
                if (Session["SparePartData"] != null)
                {
                    DataTable dt = (DataTable)Session["SparePartData"];

                    txt_InvRemark.Text += "REMARK: SPARE PARTS ON FREE OF CHARGE " + Environment.NewLine;

                    string sQty = "";
                    string sQtyUnit = "";
                    string sPack = "";
                    int iTtlQtyByPcs = 0;
                    int iTtlQtyBySet = 0;
                    int iTtlQtyByPair = 0;
                    int iTtlQtyByPack = 0;
                    int iTtlPackByPcs = 0;
                    int iTtlPackBySet = 0;
                    int iTtlPackByPair = 0;
                    int iTtlPackByPack = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        txt_InvRemark.Text += dr[0].ToString().PadRight(20) + "   --   ";
                        sQtyUnit = dr[3].ToString();
                        sQty = dr[1].ToString() + " " + sQtyUnit;
                        if (sQtyUnit == "PCS")
                        {
                            iTtlQtyByPcs += Convert.ToInt32(dr[1]);
                            iTtlPackByPcs += Convert.ToInt32(dr[2]);
                        }
                        else if (sQtyUnit == "SET")
                        {
                            iTtlQtyBySet += Convert.ToInt32(dr[1]);
                            iTtlPackBySet += Convert.ToInt32(dr[2]);
                        }
                        else if (sQtyUnit == "PAIR")
                        {
                            iTtlQtyByPair += Convert.ToInt32(dr[1]);
                            iTtlPackByPair += Convert.ToInt32(dr[2]);
                        }
                        else
                        {
                            iTtlQtyByPack += Convert.ToInt32(dr[1]);
                            iTtlPackByPack += Convert.ToInt32(dr[2]);
                        }

                        txt_InvRemark.Text += sQty.PadRight(10) + "   --    ";
                        sPack = dr[2].ToString() + " " + dr[4].ToString();
                        txt_InvRemark.Text += sPack.PadRight(10) + Environment.NewLine;
                    }

                    if (iTtlQtyByPcs != 0)
                    {
                        txt_InvRemark.Text += "Total :".PadLeft(20) + "   --   ";
                        txt_InvRemark.Text += (iTtlQtyByPcs.ToString() + " PCS").PadRight(10) + "   --    ";
                        txt_InvRemark.Text += (iTtlPackByPcs.ToString() + " CTN").PadRight(10) + Environment.NewLine;
                    }
                    if (iTtlQtyBySet != 0)
                    {
                        txt_InvRemark.Text += "Total :".PadLeft(20) + "   --   ";
                        txt_InvRemark.Text += (iTtlQtyBySet.ToString() + " SET").PadRight(10) + "   --    ";
                        txt_InvRemark.Text += (iTtlPackBySet.ToString() + " CTN").PadRight(10) + Environment.NewLine;
                    }
                    if (iTtlQtyByPair != 0)
                    {
                        txt_InvRemark.Text += "Total :".PadLeft(20) + "   --   ";
                        txt_InvRemark.Text += (iTtlQtyByPair.ToString() + " PAIR").PadRight(10) + "   --    ";
                        txt_InvRemark.Text += (iTtlPackByPair.ToString() + " CTN").PadRight(10) + Environment.NewLine;
                    }
                    if (iTtlQtyByPack != 0)
                    {
                        txt_InvRemark.Text += "Total :".PadLeft(20) + "   --   ";
                        txt_InvRemark.Text += (iTtlQtyByPack.ToString() + " PACK").PadRight(10) + "   --    ";
                        txt_InvRemark.Text += (iTtlPackByPack.ToString() + " CTN").PadRight(10) + Environment.NewLine;
                    }
                    Session["SparePartData"] = null;
                }
            }
            else if (ddl_InvTemplate.SelectedIndex == 1)
            {
                txt_InvRemark.Text += "ALL CONTRACTS ARE CLASSED AS DANGEROUS GOODS IN LIMITED QUANTITIES FOR UK ROAD TRANSPORT. " + Environment.NewLine + Environment.NewLine;
                txt_InvRemark.Text += "THAT IS TO SAY THEY MEET ALL REQUIREMENTS OF THE ADR REGULATION 5.4.1.1.4 SPECIAL PROVISIONS FOR DANGEROUS GOODS PACED IN LIMITED QUANTITIES." + Environment.NewLine + Environment.NewLine;
                txt_InvRemark.Text += "NO INFORMATION IS REQUIRED IN THE TRANSPORT DOCUMENT, IF ANY, FOR CARRIAGE OF DANGEROUS GOODS PACKED IN LIMITED QUANTITIES ACCORDING TO CHAPTER 3.4.";
            }
            else if (ddl_InvTemplate.SelectedIndex == 2)
            {
                txt_InvRemark.Text += "ASSAY OFFICE" + Environment.NewLine;
                txt_InvRemark.Text += "PO BOX 151" + Environment.NewLine;
                txt_InvRemark.Text += "NEWHALL STREET" + Environment.NewLine;
                txt_InvRemark.Text += "BIRMINGHAM" + Environment.NewLine;
                txt_InvRemark.Text += "B3 1SB";
            }
            else if (ddl_InvTemplate.SelectedIndex == 3)
            {
             
                txt_InvRemark.Text += "———————————————————————————————————————————————————————————————————————————————————————————" + Environment.NewLine;
                txt_InvRemark.Text += "ITEM/OPT NO.  DESCRIPTION                        QUANTITY      UNIT PRICE     AMOUNT    " + Environment.NewLine;
                txt_InvRemark.Text += "                                                 METERS        USD            USD" + Environment.NewLine;
                txt_InvRemark.Text += "===========================================================================================" + Environment.NewLine;
                txt_InvRemark.Text += "                                                                              DAP UK" + Environment.NewLine;
                txt_InvRemark.Text += "458909        100% POLYESTER FABIRC" + Environment.NewLine;
                txt_InvRemark.Text += "              Cost of Fabirc                     3745          3.54           13,257.30" + Environment.NewLine;
                txt_InvRemark.Text += "              Cost of Freight to UK Port                       0.20              749.00" + Environment.NewLine;
                txt_InvRemark.Text += "              Cost of Freight from UK port to Coat Factory     0.08              299.60" + Environment.NewLine;
                txt_InvRemark.Text += "                                                                              ———————————" + Environment.NewLine;
                txt_InvRemark.Text += "              Total DAP UK                                                    14,305.90" + Environment.NewLine;
                txt_InvRemark.Text += "              UK Cost(Coating cost and freight  from Coating   3.20           11,984.00" + Environment.NewLine;
                txt_InvRemark.Text += "              Factory to Upholsterer warehouse)                               ———————————" + Environment.NewLine;
                txt_InvRemark.Text += "                                                                              26,289.90" + Environment.NewLine;
                txt_InvRemark.Text += "              5% of  Commission                                                1,314.50" + Environment.NewLine;
                txt_InvRemark.Text += "———————————————————————————————————————————————————————————————————————————————————————————" + Environment.NewLine;
                txt_InvRemark.Text += "              Total  for NEXT to pay                                          27,604.40" + Environment.NewLine;
                txt_InvRemark.Text += "                                                                              ===========" + Environment.NewLine;
                txt_InvRemark.Text += "              Say Total : " + Environment.NewLine;
                txt_InvRemark.Text += "              Total Number of Rolls: 70" + Environment.NewLine;
                txt_InvRemark.Text += "              Supplier:Soft furnishings limited" + Environment.NewLine;
                txt_InvRemark.Text += "              This Invoice is for your Customs Clearance and Reference." + Environment.NewLine;
                txt_InvRemark.Text += "" + Environment.NewLine;
                txt_InvRemark.Text += "              Remark:" + Environment.NewLine;
                txt_InvRemark.Text += "              Delivery Address to Coating Factory" + Environment.NewLine;
                txt_InvRemark.Text += "              H&C Whitehead Ltd, Prospect Works - Bradford Road, Bailiff Bridge, Brighouse, " + Environment.NewLine;
                txt_InvRemark.Text += "              West Yorkshire, England, HD64DJ" + Environment.NewLine;
                txt_InvRemark.Text += "              H&C Whitehead Ltd contact info: Tel  01484 712151; www.hcwhitehead.co.uk" + Environment.NewLine;
                txt_InvRemark.Text += "==============================================================================================" + Environment.NewLine;
                txt_InvRemark.Text += "SAY TOTAL: UNITED STATE DOLLARS THIRTY NINE THOUSAND FOUR HUNDRED THIRTY FOUR AND EIGHT" + Environment.NewLine;
                txt_InvRemark.Text += "           FIVE CENTS ONLY" + Environment.NewLine;
                txt_InvRemark.Text += "THIS INVOICE IS FOR YOUR CUSTOMS CLEARANCE AND REFERENCE." + Environment.NewLine;
                txt_InvRemark.Text += "THIS IS COMPUTER-GENERATED INVOICE AND DOES NOT REQUIRE SIGNATURE" + Environment.NewLine;
                txt_InvRemark.Text += "" + Environment.NewLine;
                txt_InvRemark.Text += "BANK DETAILS FOR SETTLEMENT   :" + Environment.NewLine;
                txt_InvRemark.Text += "BENEFICIARY NAME             NEXT SOURCING LTD" + Environment.NewLine;
                txt_InvRemark.Text += "BENEFICIARY A/C NO.          502-153745-274 (USD)" + Environment.NewLine;
                txt_InvRemark.Text += "SWIFT CODE                   HSBCHKHHXXX" + Environment.NewLine;
                txt_InvRemark.Text += "BANK                         HSBC" + Environment.NewLine;
                txt_InvRemark.Text += "BANK ADDRESS                 HONG KONG OFFICE, 1 QUEEN'S ROAD CENTRAL,        " + Environment.NewLine;
                txt_InvRemark.Text += "                             HONG KONG SAR";
            }

        }

        protected void btn_Save_Click(object sender, EventArgs e)
        {
            if (!getDocumentUpdates())
                return;

            //if (!getPaymentDeductionUpdates())
            //    return;

            if (!validateShipmentDetail())
                return;

            //if (vwDomainShipmentDef.Product != null)
            //    if (Page.IsValid && ckb_setMaster.Checked)
            //        updateProduct(vwDomainShipmentDef.Product,vwDomainShipmentDef.Shipment.ShipmentId);

            if (vwDomainShipmentDef.Invoice != null)
            {
                val_Options.Enabled = true;
                val_Invoice.Enabled = true;
                val_Amount.Enabled = true;
                val_PaymentInfo.Enabled = true;
                val_BookingInfo.Enabled = true;

                val_Options.Validate();
                val_Invoice.Validate();
                val_Amount.Validate();
                val_PaymentInfo.Validate();
                val_BookingInfo.Validate();

                if ((vwDomainShipmentDef.Contract.Customer.CustomerId == 1 || vwDomainShipmentDef.Contract.Customer.CustomerId == 2) &&
                    CustomerDestinationDef.isDFOrder(vwDomainShipmentDef.Shipment.CustomerDestinationId))
                {
                    val_DF.Enabled = true;
                    val_DF.Validate();
                }

                //if (CommonManager.Instance.getCustomerDestinationByKey(vwDomainShipmentDef.Shipment.CustomerDestinationId).UTurnOrder == 1)
                if (vwDomainShipmentDef.Shipment.TermOfPurchase.TermOfPurchaseId == TermOfPurchaseRef.Id.FOB_UT.GetHashCode())
                {
                    val_UTurn.Enabled = true;
                    val_UTurn.Validate();
                }


                if (ckb_ShipDocCheck.Checked) // && ckb_ShipDocCheck.Enabled)
                {
                    if (txt_VendorInvoiceNo.Text == "" || txt_ShipRecDocDate.Text == "")    // || txt_InvPiece.Text == ""
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ShippingDocStatusUpdate", "alert('Document cannot be submitted to Accounts follow up as any of below data is not yet input completely:\\n - Supplier Invoice No.;\\n - Shipping Doc. Receipt Date.');", true);
                        //ckb_ShipDocCheck.Checked = false;
                        //txt_ShipDocCheckAmount.Text = "";
                        return;
                    }
                    if (vwDomainShipmentDef.SplitShipments != null)
                        if (vwDomainShipmentDef.SplitShipments.Count > 0)
                        {
                            if (!isShipDocInfoValidInSplitShipment())
                            {
                                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SplitShippingDocStatusUpdate", "alert('Document cannot be submitted to Accounts follow up as the Supplier Invoice No. in the split shipment is not yet input completely');", true);
                                //ckb_ShipDocCheck.Checked = false;
                                //txt_ShipDocCheckAmount.Text = "";
                                return;
                            }
                        }
                }


                if (Page.IsValid)
                {
                    updateShipmentDetail();
                    updateInvoice(vwDomainShipmentDef);
                }
            }
            else
            {
                if (Page.IsValid)
                    updateShipmentDetail();
            }

            if (Page.IsValid)
                if (vwDomainShipmentDef.Product != null && (ckb_setMaster.Checked || vwDomainShipmentDef.Product.RetailDescription != txt_RetailDesc.Text.Trim()))
                    updateProduct(vwDomainShipmentDef.Product, vwDomainShipmentDef.Shipment.ShipmentId, ckb_setMaster.Checked);

            if (Page.IsValid)
            {
                SetControl(false);

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.UpdateShipment);
                Context.Items.Add(ShipmentCommander.Param.invoiceDef, vwDomainShipmentDef.Invoice);
                Context.Items.Add(ShipmentCommander.Param.documents, vwDomainShipmentDef.UpdatedDocuments);
                Context.Items.Add(ShipmentCommander.Param.paymentDeduction, vwDomainShipmentDef.UpdatedPaymentDeduction);
                Context.Items.Add(ShipmentCommander.Param.shipmentDetails, vwDomainShipmentDef.UpdatedShipmentDetails);
                //Context.Items.Add(ShipmentCommander.Param.RejectPaymentReasonId, vwDomainShipmentDef.Shipment.RejectPaymentReasonId);
                //Context.Items.Add(ShipmentCommander.Param.splitShipments, vwDomainShipmentDef.SplitShipments);
                Context.Items.Add(ShipmentCommander.Param.shipmentDef, vwDomainShipmentDef.Shipment);
                Context.Items.Add(ShipmentCommander.Param.splitShipments, vwDomainShipmentDef.SplitShipments);

                forwardToScreen(null);

                vwDomainShipmentDef = ShipmentManager.Instance.getDomainShipmentDef(vwDomainShipmentDef.Shipment.ShipmentId);
                BindData(vwDomainShipmentDef);
                SetControl(false);
            }
            else
            {
                paymentDeductionSetControl();
            }
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            BindData(vwDomainShipmentDef);
            SetControl(false);

            if (vwDomainShipmentDef.UpdatedDocuments != null)
                vwDomainShipmentDef.UpdatedDocuments = null;

            vwDomainShipmentDef.UpdatedPaymentDeduction = null;
            vw_PaymentDeductionEdit = null;

            if (vwDomainShipmentDef.UpdatedShipmentDetails != null)
                vwDomainShipmentDef.UpdatedShipmentDetails = null;

            ckb_ShipRecDocDate.Checked = (lbl_ShipRecDocDate.Text != "");

            clearValidatorMessage();
        }

        protected void btn_NewDoc_Click(object sender, EventArgs e)
        {

            ArrayList docList = new ArrayList();
            docList.Add(new DocumentDef());
            vwDomainShipmentDef.Documents = docList;

            gv_DocInfo.Columns[0].Visible = true;
            btn_NewDoc.Visible = false;

            if (vwDomainShipmentDef.UpdatedDocuments == null)
                vwDomainShipmentDef.UpdatedDocuments = new ArrayList();

            vwDomainShipmentDef.UpdatedDocuments.Add(new DocumentDef());

            gv_DocInfoUpdate.DataSource = vwDomainShipmentDef.UpdatedDocuments;
            gv_DocInfoUpdate.DataBind();

        }

        protected void btn_NewPaymentDeduction_Click(object sender, EventArgs e)
        {

            ArrayList deductionList = new ArrayList();
            ShipmentDeductionDef def = new ShipmentDeductionDef();
            def.DeductionType = PaymentDeductionType.NOT_AVAILIABLE;
            def.DocumentNo = string.Empty;
            def.Amount = 0;
            def.Remark = string.Empty;
            def.Status = 1;
            deductionList.Add(def);
            vwDomainShipmentDef.PaymentDeduction = deductionList;

            gv_PaymentDeduction.Columns[0].Visible = true;
            btn_NewPaymentDeduction.Visible = false;

            if (vwDomainShipmentDef.UpdatedPaymentDeduction == null)
                vwDomainShipmentDef.UpdatedPaymentDeduction = new ArrayList();

            vwDomainShipmentDef.UpdatedPaymentDeduction.Add(def.Clone());

            gv_PaymentDeductionUpdate.DataSource = vwDomainShipmentDef.UpdatedPaymentDeduction;
            gv_PaymentDeductionUpdate.DataBind();

        }

        protected void btn_Print_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.PrintInvoice);
            Context.Items.Add(ShipmentCommander.Param.shipmentList, ConvertUtility.createArrayList(Convert.ToInt32(vwDomainShipmentDef.Shipment.ShipmentId)));

            forwardToScreen(null);
        }

        protected void btn_PrintDN_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.PrintDebitNote);
            Context.Items.Add(ShipmentCommander.Param.shipmentId, vwDomainShipmentDef.Shipment.ShipmentId);
            Context.Items.Add(ShipmentCommander.Param.debitNoteType, "Debit Note");
            forwardToScreen(null);
        }

        protected void btn_Send_Click(object sender, EventArgs e)
        {
            ShipmentId = Convert.ToInt32(vwDomainShipmentDef.Shipment.ShipmentId);
            ShipmentManager.Instance.prepareToSendEzibuyInvoice(ConvertUtility.createArrayList(ShipmentId), this.LogonUserId);
            vwDomainShipmentDef = null;
            vwDomainShipmentDef = ShipmentManager.Instance.getDomainShipmentDef(ShipmentId);
            BindData(vwDomainShipmentDef);
            SetControl(false);
        }

        protected void btn_setItemDescriptionAsMaster_click(object sender, EventArgs e)
        {
            ProductDef product;

            if (vwDomainShipmentDef.Product != null)
            {
                lbl_MasterItemDesc1.Text = txt_Desc1.Text.Trim();
                lbl_MasterItemDesc2.Text = txt_Desc2.Text.Trim();
                lbl_MasterItemDesc3.Text = txt_Desc3.Text.Trim();
                lbl_MasterItemDesc4.Text = txt_Desc4.Text.Trim();
                lbl_MasterItemDesc5.Text = txt_Desc5.Text.Trim();

                product = vwDomainShipmentDef.Product;
                product.MasterDescription1 = txt_Desc1.Text.Trim();
                product.MasterDescription2 = txt_Desc2.Text.Trim();
                product.MasterDescription3 = txt_Desc3.Text.Trim();
                product.MasterDescription4 = txt_Desc4.Text.Trim();
                product.MasterDescription5 = txt_Desc5.Text.Trim();


                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.UpdateProduct);
                Context.Items.Add(ShipmentCommander.Param.product, product);

                forwardToScreen(null);
            }
        }

        protected void btn_DiscrepancyAlert_Click(object sender, EventArgs arg)
        {
            AccountManager.Instance.sendDiscrepancyAlert(vwDomainShipmentDef.Contract.Merchandiser, vwDomainShipmentDef.Shipment.ShipmentId, vwDomainShipmentDef.Shipment.SellCurrency.CurrencyCode,
                vwDomainShipmentDef.Invoice.ARAmt, vwDomainShipmentDef.Invoice.InvoiceNo, this.LogonUserId);
        }

        protected void ckb_ShipDocCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_ShipDocCheck.Checked)
                txt_ShipDocCheckAmount.Text = lbl_NetAmt.Text;
            else
                txt_ShipDocCheckAmount.Text = "";

        }

        protected void updateProduct(ProductDef product, int shipmentId, bool isSetAsMaster)
        {
            if (product != null)
            {
                if (isSetAsMaster)
                {
                    lbl_MasterItemDesc1.Text = txt_Desc1.Text.Trim();
                    lbl_MasterItemDesc2.Text = txt_Desc2.Text.Trim();
                    lbl_MasterItemDesc3.Text = txt_Desc3.Text.Trim();
                    lbl_MasterItemDesc4.Text = txt_Desc4.Text.Trim();
                    lbl_MasterItemDesc5.Text = txt_Desc5.Text.Trim();

                    product.MasterDescription1 = txt_Desc1.Text.Trim();
                    product.MasterDescription2 = txt_Desc2.Text.Trim();
                    product.MasterDescription3 = txt_Desc3.Text.Trim();
                    product.MasterDescription4 = txt_Desc4.Text.Trim();
                    product.MasterDescription5 = txt_Desc5.Text.Trim();
                }
                lbl_RetailDesc.Text = txt_RetailDesc.Text.Trim();
                product.RetailDescription = txt_RetailDesc.Text.Trim();

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.UpdateProduct);
                Context.Items.Add(ShipmentCommander.Param.shipmentId, shipmentId);
                Context.Items.Add(ShipmentCommander.Param.product, product);
                forwardToScreen(null);
            }
        }

        #endregion


        #region Misc Functions

        private DataTable getOtherCostSummary(ref ArrayList otherCostDetail)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("OtherCostType");
            dt.Columns.Add("Supplier");

            DataTable dt_Detail = new DataTable();
            dt_Detail.Columns.Add("Option");
            dt_Detail.Columns.Add("Colour");
            dt_Detail.Columns.Add("TotalShippedQty");
            dt_Detail.Columns.Add("OtherCostAmt");
            dt_Detail.Columns.Add("OtherCostTotalAmt");

            ArrayList otherCostSupplier = new ArrayList();
            otherCostDetail = new ArrayList();

            foreach (ShipmentDetailDef detailDef in vwDomainShipmentDef.ShipmentDetails)
            {

                if (detailDef.OtherCost != null)
                {
                    DataTable temp;

                    foreach (OtherCostDef otherCost in detailDef.OtherCost)
                    {
                        if (!otherCostSupplier.Contains(otherCost.OtherCostType.OtherCostTypeDescription + otherCost.VendorId.ToString()))
                        {
                            otherCostSupplier.Add(otherCost.OtherCostType.OtherCostTypeDescription + otherCost.VendorId.ToString());
                            temp = dt_Detail.Clone();
                            otherCostDetail.Add(temp);
                            DataRow dr = dt.NewRow();
                            dr[0] = otherCost.OtherCostType.OtherCostTypeDescription;
                            if (otherCost.VendorId != -1)
                            {
                                dr[1] = com.next.common.datafactory.worker.industry.VendorWorker.Instance.getVendorByKey(otherCost.VendorId).Name;
                            }
                            dt.Rows.Add(dr);
                        }
                        else
                        {
                            temp = (DataTable)otherCostDetail[otherCostSupplier.IndexOf(otherCost.OtherCostType.OtherCostTypeDescription + otherCost.VendorId.ToString())];
                        }

                        DataRow detailRow = temp.NewRow();
                        detailRow[0] = detailDef.SizeOption.SizeOptionNo;
                        detailRow[1] = detailDef.Colour;
                        detailRow[2] = detailDef.ShippedQuantity.ToString();
                        detailRow[3] = otherCost.OtherCostAmount.ToString();
                        detailRow[4] = (otherCost.OtherCostAmount * detailDef.ShippedQuantity);
                        temp.Rows.Add(detailRow);
                    }
                }
            }

            return dt;
        }

        private bool getDocumentUpdates()
        {
            SmartDropDownList ddl = null;
            TextBox txt = null;
            DocumentDef docDef = null;
            int count = 0;

            int docType = 0;
            int countryId = 0;
            int quotaCatId = 0;
            decimal weight = 0;
            int qty = 0;
            int unitId = 0;
            int poQty = 0;
            int poUnitId = 0;
            int orderQty = 0;
            int orderUnitId = 0;
            string awbNo = "";
            string docNo = "";
            DateTime issueDate = DateTime.MinValue;
            DateTime expiryDate = DateTime.MinValue;
            DateTime despatchDate = DateTime.MinValue;
            bool isDetailRequired = true;

            string errMsg = "";
            bool isValid = true;

            foreach (GridViewRow row in gv_DocInfoUpdate.Rows)
            {
                docDef = (DocumentDef)vwDomainShipmentDef.UpdatedDocuments[count];

                if ((docDef.ShipmentId != vwDomainShipmentDef.Shipment.ShipmentId && docDef.ShipmentId != 0) ||
                    (docDef.ShipmentId != 0 && docDef.Status == 0))
                {
                    count++;
                    continue;
                }

                isDetailRequired = true;
                ddl = (SmartDropDownList)row.Cells[1].FindControl("ddl_DocType");
                docType = int.Parse(ddl.SelectedValue);

                txt = (TextBox)row.Cells[2].FindControl("txt_DocNo");
                docNo = txt.Text.Trim();
                if (docNo == "")
                {
                    errMsg += "Please enter document no.\\r\\n";
                    isValid = false;
                }

                ddl = (SmartDropDownList)row.Cells[3].FindControl("ddl_DocCountry");
                countryId = int.Parse(ddl.SelectedValue);
                if (countryId == -1)
                {
                    errMsg += "Please select country.\\r\\n";
                    isValid = false;
                }
                else if (countryId != vwDomainShipmentDef.Shipment.CountryOfOrigin.CountryOfOriginId)
                {
                    errMsg += "The selected country in document is inconsistent with CO in shipment\\r\\n";
                }

                txt = (TextBox)row.Cells[4].FindControl("txt_IssueDate");
                if (txt.Text.Trim() == "")
                {
                    issueDate = DateTime.MinValue;
                }
                else if (!DateTime.TryParse(txt.Text, null, System.Globalization.DateTimeStyles.None, out issueDate))
                {
                    errMsg += "Issue Date is not in correct format.\\r\\n";
                    isValid = false;
                }

                txt = (TextBox)row.Cells[5].FindControl("txt_ExpiryDate");
                if (txt.Text.Trim() == "")
                {
                    expiryDate = DateTime.MinValue;
                }
                else if (!DateTime.TryParse(txt.Text, null, System.Globalization.DateTimeStyles.None, out expiryDate))
                {
                    errMsg += "Expiry date is not in correct format.\\r\\n";
                    isValid = false;
                }

                ddl = (SmartDropDownList)row.Cells[6].FindControl("ddl_QuotaCat");
                quotaCatId = int.Parse(ddl.SelectedValue);

                txt = (TextBox)row.Cells[7].FindControl("txt_Weight");
                if (!decimal.TryParse(txt.Text.Trim() == "" ? "0" : txt.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out weight))
                {
                    errMsg += "Invalid weight.\\r\\n";
                    isValid = false;
                }

                txt = (TextBox)row.Cells[8].FindControl("txt_QtyOnDoc");
                if (!int.TryParse(txt.Text == "" ? "0" : txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out qty) || qty == 0)
                {
                    isDetailRequired = false;
                }

                ddl = (SmartDropDownList)row.Cells[8].FindControl("ddl_QtyOnDocUnit");
                unitId = int.Parse(ddl.SelectedValue);

                txt = (TextBox)row.Cells[9].FindControl("txt_QtyForOrder");
                if (!int.TryParse(txt.Text == "" ? "0" : txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out orderQty) || orderQty == 0)
                {
                    if (isDetailRequired)
                    {
                        errMsg += "Please provide CO details.";
                        isValid = false;
                        isDetailRequired = false;
                    }
                }
                else
                {
                    if (!isDetailRequired)
                    {
                        isValid = false;
                        errMsg = "Please provide CO details.";
                        isDetailRequired = true;
                    }
                    else if (orderQty != qty)
                    {
                        errMsg += "Qty on doc is different from qty for order !\\r\\n";
                    }
                }


                ddl = (SmartDropDownList)row.Cells[9].FindControl("ddl_QtyForOrderUnit");
                orderUnitId = int.Parse(ddl.SelectedValue);

                txt = (TextBox)row.Cells[10].FindControl("txt_EqvPOQty");
                if (txt.Text == "" || !int.TryParse(txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out poQty) || poQty == 0)
                {
                    if (isDetailRequired)
                    {
                        errMsg += "Please provide CO details.";
                        isValid = false;
                        isDetailRequired = false;
                    }
                }
                else
                {
                    if (!isDetailRequired)
                    {
                        isValid = false;
                        errMsg = "Please provide CO details.";
                        isDetailRequired = true;
                    }
                }

                ddl = (SmartDropDownList)row.Cells[10].FindControl("ddl_QtyOnPOUnit");
                poUnitId = int.Parse(ddl.SelectedValue);
                if (poUnitId != orderUnitId)
                {
                    errMsg += "PO Qty Unit is different from Order Qty Unit ! \\r\\n";
                }

                txt = (TextBox)row.Cells[11].FindControl("txt_DespatchDate");
                if (txt.Text != "")
                {
                    if (!DateTime.TryParse(txt.Text, null, System.Globalization.DateTimeStyles.None, out despatchDate))
                    {
                        errMsg += "Invalid despatch date.\\r\\n";
                        isValid = false;
                    }
                }
                else
                    despatchDate = DateTime.MinValue;

                txt = (TextBox)row.Cells[12].FindControl("txt_AWBNo");
                awbNo = txt.Text;

                if (isValid)
                {
                    docDef.DocumentType = DocumentType.getType(docType);
                    docDef.DocumentNo = docNo;
                    docDef.IssueDate = issueDate;
                    docDef.ExpiryDate = expiryDate;
                    docDef.Weight = weight;
                    docDef.Qty = qty;
                    docDef.Unit = new PackingUnitRef(unitId, "", "");
                    docDef.OrderQty = orderQty;
                    docDef.OrderUnit = new PackingUnitRef(orderUnitId, "", "");
                    docDef.POQty = poQty;
                    docDef.POUnit = new PackingUnitRef(poUnitId, "", "");
                    docDef.DespatchToUKDate = despatchDate;
                    docDef.DespatchAWBNo = awbNo;

                    if (countryId != -1)
                    {
                        CountryOfOriginRef co = new CountryOfOriginRef();
                        co.CountryOfOriginId = countryId;
                        docDef.Country = co;
                    }
                    else
                        docDef.Country = null;

                    if (quotaCatId != -1)
                        docDef.QuotaCategory = new QuotaCategoryRef(quotaCatId, "", "");
                    else
                        docDef.QuotaCategory = null;
                }

                docDef.ShipmentId = vwDomainShipmentDef.Shipment.ShipmentId;
                docDef.Status = 1;
                count++;
            }

            if (errMsg != "")
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "docUpdateErr", "alert('" + errMsg + "');", true);

            return isValid;
        }

        private void paymentDeductionSetControl()
        {
            foreach (GridViewRow r in gv_PaymentDeductionUpdate.Rows)
                PaymentDeductionUpdateRowControl(r);
        }
        
        private bool getPaymentDeductionUpdates()
        {
            string errMsg = string.Empty;
            if ((errMsg = fillPaymentDeductionUpdates()) != string.Empty)
            {
                paymentDeductionSetControl();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "paymentDeductionUpdateErr", "alert(\"" + errMsg + "\");", true);
            }
            return (errMsg == string.Empty);
        }

        private string fillPaymentDeductionUpdates()
        {
            bool isValid = true;
            string errMsg = "";
            string msg = "";
            SortedList uniqueDeductions = new SortedList();
            //SortedList updatedDeductionList = new SortedList();
            SortedList<int, PaymentDeductionEdit> inputList = new SortedList<int, PaymentDeductionEdit>();
            List<PaymentDeductionType> typeAllowZero = new List<PaymentDeductionType>();
            typeAllowZero.Add(PaymentDeductionType.FABRIC_ADVANCE);
            typeAllowZero.Add(PaymentDeductionType.OTHERS_CREDIT);
            typeAllowZero.Add(PaymentDeductionType.OTHERS_DEBIT);

            foreach (GridViewRow row in gv_PaymentDeductionUpdate.Rows)
            {
                ShipmentDeductionDef deduction = (ShipmentDeductionDef)vwDomainShipmentDef.UpdatedPaymentDeduction[row.DataItemIndex];
                if (deduction.Status == 1)
                {
                    string docNo = "";
                    decimal amt = decimal.MinValue;
                    int deductionTypeId = 0;

                    PaymentDeductionType deductionType;

                    SmartDropDownList ddl = (SmartDropDownList)row.Cells[1].FindControl("ddl_DeductionType");
                    TextBox txtDeductionType = (TextBox)row.Cells[1].FindControl("txt_DeductionType");
                    TextBox txtDocNo = (TextBox)row.Cells[2].FindControl("txt_DocNo");
                    TextBox txtAmt = (TextBox)row.Cells[3].FindControl("txt_Amount");
                    TextBox txtRemark = (TextBox)row.Cells[4].FindControl("txt_Remark");
                    deductionTypeId = getPaymentDeductionTypeId(ddl.SelectedValue);
                    deductionType = PaymentDeductionType.getType(deductionTypeId);
                    docNo = txtDocNo.Text.Trim();
                    if (!decimal.TryParse(txtAmt.Text == "" ? "0" : txtAmt.Text, System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out amt))
                        amt = decimal.MinValue;

                    ShipmentDeductionDef def = new ShipmentDeductionDef();
                    def.DeductionType = deductionType;
                    def.DocumentNo = docNo;
                    def.Amount = (amt == decimal.MinValue ? 0 : amt);
                    def.Remark = txtRemark.Text.Trim();

                    PaymentDeductionEdit input = new PaymentDeductionEdit();
                    input.def = def;
                    input.DeductionType = ddl.SelectedValue;
                    input.DocumentNo = txtDocNo.Text;
                    input.Amount = txtAmt.Text;
                    input.Remark = txtRemark.Text.Trim();
                    inputList.Add(row.DataItemIndex, input);

                    //if ((deduction.ShipmentId == vwDomainShipmentDef.Shipment.ShipmentId || deduction.ShipmentId == 0) && (deduction.ShipmentId == 0) && deduction.Status != 0)
                    //if ((deduction.ShipmentId == vwDomainShipmentDef.Shipment.ShipmentId || deduction.ShipmentId == 0) && (deduction.ShipmentId == 0 || deduction.Status != 0))
                    //if ((deduction.ShipmentId == vwDomainShipmentDef.Shipment.ShipmentId || deduction.ShipmentId == 0) && !(deduction.ShipmentId == 0 && deduction.Status == 0))
                    if ((deduction.ShipmentId == vwDomainShipmentDef.Shipment.ShipmentId || deduction.ShipmentId == 0) && deduction.Status == 1)
                    {
                        // Validation & Alert
                        if (deductionType == PaymentDeductionType.NOT_AVAILIABLE)
                        {
                            msg = "- Please select a deduction type.";
                            errMsg += (errMsg.Contains(msg) ? string.Empty : msg + "<br>");
                            isValid = false;
                        }
                        else
                        {
                            bool isHKShippingUser = false;
                            foreach (UserRoleDef role in SecurityReader.Instance.getUserRoleByUserIdAndAppId(this.LogonUserId, AccessMapper.ISAM.Id))
                                if (role.RoleId == 501)   // Role : ISAM-HK-SHIPPING
                                    isHKShippingUser = true;

                            bool isDeductionTypeEditable = true;
                            if (txtDeductionType == null)
                                isDeductionTypeEditable = false;
                            else
                                if (txtDeductionType.ReadOnly && txtDeductionType.Style["Display"] != "none")
                                    isDeductionTypeEditable = false;

                            bool isDocNoEditable = true;
                            if (txtDocNo == null)
                                isDocNoEditable = false;
                            else
                                if (txtDocNo.ReadOnly && txtDocNo.Style["Display"] != "none")
                                    isDocNoEditable = false;

                            bool isAmtEditable = true;
                            if (txtAmt == null)
                                isAmtEditable = false;
                            else
                                if (txtAmt.ReadOnly && txtAmt.Style["Display"] != "none")
                                    isAmtEditable = false;

                            bool c19_Allow = true;      // control BD office user to add the FL deduction
                            if (vwDomainShipmentDef.Contract.Office.OfficeId == OfficeId.BD.Id && this.LogonUserHomeOffice.OfficeId == OfficeId.BD.Id && !isHKShippingUser
                                && (isDeductionTypeEditable || isDocNoEditable || isAmtEditable))
                            {
                                msg = string.Empty;
                                if (deductionTypeId == PaymentDeductionType.C19.Id)
                                {
                                    msg = "- System does not allow adding C19 deduction for BD office";
                                    isValid = false;
                                    c19_Allow = false;
                                }
                                else if (deductionTypeId == PaymentDeductionType.OTHERS_DEBIT.Id)
                                {
                                    if (docNo.StartsWith("FL"))
                                    {
                                        msg = "- System does not allow adding fabric liability deduction for BD office";
                                        isValid = false;
                                    }
                                }
                                else if (deductionTypeId == PaymentDeductionType.FABRIC_LIABILITY.Id)
                                {
                                    msg = "- System does not allow adding Fabric Liability deduction for BD office";
                                    isValid = false;
                                }
                                else if (deductionTypeId == PaymentDeductionType.FABRIC_UTILIZATION.Id)
                                {
                                    msg = "- System does not allow adding Fabric Utilization deduction for BD office";
                                    isValid = false;
                                }

                                if (msg != string.Empty)
                                    errMsg += (errMsg.Contains(msg) ? string.Empty : msg + "<br>");
                            }

                            msg = string.Empty;
                            if (deductionType.Id != PaymentDeductionType.REMARK.Id)
                            {
                                if (amt == decimal.MinValue)
                                    msg = "a valid Amount";
                                else if (amt == 0 && !typeAllowZero.Contains(deductionType))
                                    msg = "Amount";
                                else if (amt < 0)
                                    msg = "a positive Amount";
                            }
                            if (deductionType.RequireDocumentNo && deductionType != PaymentDeductionType.FABRIC_ADVANCE)
                            {
                                if (docNo == "")
                                    msg = "Document No." + (msg != string.Empty ? " and " + msg : "");
                                else if (deductionTypeId == PaymentDeductionType.C19.Id && !AccountManager.Instance.isNSLRefNoInFLContract(txtDocNo.Text))
                                    if (c19_Allow)
                                    {
                                        msg = "a valid Document No (FL Ref. No.)" + (msg != string.Empty ? " and " + msg : "");
                                    }
                            }
                            if (msg != string.Empty)
                            {
                                msg = "- Please enter " + msg + " for '" + deductionType.Name + "'";
                                errMsg += (errMsg.Contains(msg) ? string.Empty : msg + "<br>");
                                isValid = false;
                            }

                            string key = deductionType.Id.ToString() + "." + docNo.Trim();
                            if (uniqueDeductions.ContainsKey(key))
                            {
                                if (deductionType.RequireDocumentNo)
                                    msg = "- Multiple deduction on Document No.'" + docNo + "' (" + deductionType.Name + ") is not allowed";
                                else
                                    msg = "- Multiple deduction on '" + deductionType.Name + "' is not allowed";
                                errMsg += (errMsg.Contains(msg) ? string.Empty : msg + "<br>");
                                isValid = false;
                            }
                            else
                                uniqueDeductions.Add(key, amt);
                        }
                        deduction.DeductionType = deductionType;
                        deduction.DocumentNo = docNo;
                        deduction.Amount = amt;
                        deduction.Remark = txtRemark.Text.Trim();
                        deduction.ShipmentId = vwDomainShipmentDef.Shipment.ShipmentId;
                        //deduction.Status = 1;
                    }
                }
            }
            vw_PaymentDeductionEdit = inputList;
            return errMsg;
        }

        SortedList<int, PaymentDeductionEdit> vw_PaymentDeductionEdit
        {
            get { return (SortedList<int, PaymentDeductionEdit>)ViewState["vw_PaymentDeductionEdit"]; }
            set { ViewState["vw_PaymentDeductionEdit"] = value; }
        }
        
        [Serializable]
        private class PaymentDeductionEdit
        {
            public string DeductionType { get; set; }
            public string DocumentNo { get; set; }
            public string Amount { get; set; }
            public string Remark { get; set; }
            public ShipmentDeductionDef def { get; set; }
        }

        void SetControl(bool isEditable)
        {
            bool isLabelVisible = !isEditable;
            
            if (vwDomainShipmentDef.Invoice != null)
            {
                txt_ActualInWHDate.Visible = isEditable;
                txt_BkInWHDate.Visible = isEditable;
                txt_BookingQty.Visible = isEditable;
                txt_BookingDate.Visible = isEditable;
                txt_LotShipOrderNo.Visible = isEditable;
                //txt_VendorInvoiceNo.Visible = isEditable;
                txt_ShipRemark.Visible = isEditable;
                txt_InvPiece.Visible = isEditable;
                ddl_InvPack.Visible = isEditable;
                txt_ExLicenceFee.Visible = isEditable;
                txt_QuotaCharge.Visible = isEditable;
                if (isEditable)
                {
                    if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.ModifyDirectFranchise))
                    {
                        txt_DFDebitNoteNo.Visible = isEditable;
                        txt_DocCharge.Visible = isEditable;
                        txt_TransportCharge.Visible = isEditable;
                    }
                    if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.ModifyUTOrder))
                    {
                        txt_ImportDutyActualAmt.Visible = isEditable;
                        ckb_ImportDutyRecordChecked.Enabled = isEditable;
                        txt_ImportDutyRecordCheckedDate.Visible = isEditable;
                        txt_InputVATActualAmt.Visible = isEditable;
                        ckb_InputVATRecordChecked.Enabled = isEditable;
                        txt_InputVATRecordCheckedDate.Visible = isEditable;
                        txt_OutputVATActualAmt.Visible = isEditable;
                        ckb_OutputVATRecordChecked.Enabled = isEditable;
                        txt_OutputVATRecordCheckedDate.Visible = isEditable;
                    }
                }
                else
                {
                    txt_ImportDutyActualAmt.Visible = false;
                    ckb_ImportDutyRecordChecked.Enabled = false;
                    txt_ImportDutyRecordCheckedDate.Visible = false;
                    txt_InputVATActualAmt.Visible = false;
                    ckb_InputVATRecordChecked.Enabled = false;
                    txt_InputVATRecordCheckedDate.Visible = false;
                    txt_OutputVATActualAmt.Visible = false;
                    ckb_OutputVATRecordChecked.Enabled = false;
                    txt_OutputVATRecordCheckedDate.Visible = false;
                    txt_DFDebitNoteNo.Visible = false;
                    txt_DocCharge.Visible = false;
                    txt_TransportCharge.Visible = false;
                }
                ddl_ShipFrom.Visible = isEditable;
                ddl_ShipTo.Visible = isEditable;
                //if (ShipmentManager.Instance.isReadyForDMS(vwDomainShipmentDef.Contract.Office, vwDomainShipmentDef.Shipment.WorkflowStatus, vwDomainShipmentDef.Invoice.InvoiceDate, vwDomainShipmentDef.Shipment.Vendor) && vwDomainShipmentDef.Invoice.IsUploadDMSDocument)
                //if (ShipmentManager.Instance.isReadyForDMS(vwDomainShipmentDef.Contract.Office, vwDomainShipmentDef.Shipment.WorkflowStatus, vwDomainShipmentDef.Invoice.InvoiceDate, vwDomainShipmentDef.Shipment.Vendor))
                if (ShipmentManager.Instance.isReadyForDMS(vwDomainShipmentDef))
                {
                    bool allowShipDocCheck = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.DMSControl);
                    bool viewShipDocCheck = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.DMSControlView);
                    if (allowShipDocCheck || viewShipDocCheck)
                        row_ShipDocCheck.Style.Add(HtmlTextWriterStyle.Display, "Block");
                    if (vwDomainShipmentDef.Shipment.ShippingDocWFS.Id == ShippingDocWFS.NOT_READY.Id || vwDomainShipmentDef.Shipment.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                    {
                        ckb_ShipDocCheck.Enabled = (isEditable && allowShipDocCheck);
                        txt_ShipRecDocDate.Visible = (isEditable);
                        txt_VendorInvoiceNo.Visible = (isEditable);
                    }
                    else
                    {
                        ckb_ShipDocCheck.Enabled = false;
                        txt_ShipRecDocDate.Visible = false;
                        txt_VendorInvoiceNo.Visible = false;
                    }
                }
                else
                {
                    row_ShipDocCheck.Style.Add(HtmlTextWriterStyle.Display, "None");
                    ckb_ShipDocCheck.Enabled = false;
                    txt_ShipRecDocDate.Visible = (isEditable);
                    txt_VendorInvoiceNo.Visible = (isEditable);
                }
                ckb_ShipRecDocDate.Visible = txt_ShipRecDocDate.Visible;

                tr_EditInvRmk.Visible = isEditable;
                txt_ItemColor.Visible = isEditable;
                txt_Desc1.Visible = isEditable;
                txt_Desc2.Visible = isEditable;
                txt_Desc3.Visible = isEditable;
                txt_Desc4.Visible = isEditable;
                txt_Desc5.Visible = isEditable;
                txt_RetailDesc.Visible = isEditable;
                txt_NotesToQCC.Visible = isEditable;
                txt_CourierCharge.Visible = isEditable && !txt_CourierCharge.ReadOnly;
                txt_DebitNoteNo.Visible = false; //isEditable
                ckb_PaymentChecked.Enabled = isEditable;
                txt_PaymentCheckDate.Visible = isEditable;
                img_LcOtherDelivery.Visible = (vwDomainShipmentDef.Shipment.PaymentTerm.LCPaymentTermRequestFlag && vwDomainShipmentDef.OtherDelivery.Count > 1);

                if (isEditable && vwDomainShipmentDef.Shipment.PaymentTerm.LCPaymentTermRequestFlag && (vwDomainShipmentDef.LCApplication == null
                    || vwDomainShipmentDef.LCApplication.WorkflowStatus.Id == LCWFS.REJECTED.Id))
                {
                    txt_LCNo.Visible = true;
                    txt_IssueDate.Visible = true;
                    txt_LCExpiryDate.Visible = true;
                    txt_LCAmount.Visible = true;
                    btn_cal_LCExpiryDate.Visible = true;
                }
                else
                {
                    txt_LCNo.Visible = false;
                    txt_IssueDate.Visible = false;
                    txt_LCExpiryDate.Visible = false;
                    txt_LCAmount.Visible = false;
                    btn_cal_LCExpiryDate.Visible = false;
                }
                txt_LCBillRefNo.Visible = isEditable && vwDomainShipmentDef.Shipment.PaymentTerm.LCPaymentTermRequestFlag && !string.IsNullOrEmpty(txt_LCNo.Text);

                //if (vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.choice.GetHashCode()
                //    && "HK,SH,TH".Contains(vwDomainShipmentDef.Contract.Office.OfficeCode))
                //    isChoiceTotalAmountAutoUpdate = true;
                //if (vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.choice.GetHashCode() && "BD,IN,ND,LK,PK,TR".Contains(vwDomainShipmentDef.Contract.Office.OfficeCode))
                if (needToInputChoiceTotalAmount)
                {
                    if (isEditable
                        && (!vwDomainShipmentDef.Shipment.PaymentLock
                           || (vwDomainShipmentDef.Shipment.WorkflowStatus.Id != ContractWFS.INVOICED.Id
                                && string.IsNullOrEmpty(vwDomainShipmentDef.Invoice.InvoicePrefix))
                        ))
                    {
                        // Show text box for editing, system will validate the content
                        txt_ChoiceActPurchaseAmt.Visible = true;
                        txt_ChoiceActSalesAmt.Visible = true;
                        lbl_ChoiceActPurchaseAmt.Visible = false;
                        lbl_ChoiceActSalesAmt.Visible = false;
                    }
                    else
                    {
                        // Hide the text box, system will not validate the content
                        txt_ChoiceActPurchaseAmt.Visible = false;
                        txt_ChoiceActSalesAmt.Visible = false;
                        lbl_ChoiceActPurchaseAmt.Visible = true;
                        lbl_ChoiceActSalesAmt.Visible = true;
                    }
                    lbl_ChoiceNslCommAmt.Visible = true;
                }
                else
                {
                    // Hide the text box, system will not validate the content
                    txt_ChoiceActPurchaseAmt.Visible = false;
                    txt_ChoiceActSalesAmt.Visible = false;
                    lbl_ChoiceNslCommAmt.Visible = false;
                    lbl_ChoiceActPurchaseAmt.Visible = false;
                    lbl_ChoiceActSalesAmt.Visible = false;
                }

                if (isEditable)
                {
                    //if (!vwDomainShipmentDef.Invoice.IsSelfBilledOrder && vwDomainShipmentDef.Shipment.TermOfPurchase.TermOfPurchaseId != TermOfPurchaseRef.Id.FOB_UT.GetHashCode() &&
                    if (!vwDomainShipmentDef.Invoice.IsSelfBilledOrder &&
                        ((vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.APPROVED.Id)
                         || (vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.PO_PRINTED.Id)
                         || (vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id && !vwDomainShipmentDef.Shipment.PaymentLock)
                        )
                       )
                    {
                        if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.ModifyInvoiceDate))
                            //&& ( vwDomainShipmentDef.Contract.Customer.CustomerId!=CustomerDef.Id.hempel.GetHashCode() || CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.IssueHempelInvoice))
                            txt_InvDate.Visible = true;
                    }
                }
                else
                    txt_InvDate.Visible = false;
            }

            if (isEditable && (vwDomainShipmentDef.Documents == null || vwDomainShipmentDef.Documents.Count == 0))
                btn_NewDoc.Visible = true;
            else
                btn_NewDoc.Visible = false;

            if (isEditable && (vwDomainShipmentDef.PaymentDeduction == null || vwDomainShipmentDef.PaymentDeduction.Count == 0))
                btn_NewPaymentDeduction.Visible = true;
            else
                btn_NewPaymentDeduction.Visible = false;

            btn_Save.Visible = isEditable;
            btn_Cancel.Visible = isEditable;
            if (btn_Save.Visible)
            {
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.AllowInputZeroQuantity))
                    btn_Save.OnClientClick = "return (allowToSave() ? confirmIfTotalShippedQtyZero(true) : false);";    // get confirm on zero qty and skip validate at server side
                else
                    btn_Save.OnClientClick = "return allowToSave();";  // Validate at server side
            }

            if (",GT,EB,LIPSY,SB,".Contains("," + vwDomainShipmentDef.Contract.Customer.ShortCode + ","))
            {
                gv_Options.Columns[SizeOptionColumnId.Colour].Visible = true;
            }

            if (isEditable && vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id && vwDomainShipmentDef.Shipment.PaymentLock)
            {
                gv_Options.Columns[SizeOptionColumnId.ShippedQty].Visible = true;
                gv_Options.Columns[SizeOptionColumnId.ShippedQtyInput].Visible = false;
            }
            else
            {
                gv_Options.Columns[SizeOptionColumnId.ShippedQty].Visible = isLabelVisible;
                gv_Options.Columns[SizeOptionColumnId.ShippedQtyInput].Visible = isEditable;
            }

            bool hasSplit = false;
            if (vwDomainShipmentDef.Shipment.SplitCount > 0 && vwDomainShipmentDef.Shipment.IsVirtualSetSplit == 0)
                hasSplit = true;

            if (!hasSplit)
            {
                //tcContract.Tabs[7].Visible = false;       // it causes confusion in showing tbe tab content of 'Other Delivery', 'Manifest' and 'Deduction'
                gv_AuditLog.Columns[5].Visible = false;
            }
            else
            {
                lbl_QACommPercent.Visible = false;
                lbl_SplitShipmentQAComm.Visible = true;

                lbl_LabTestIncome.Visible = false;
                lbl_SplitShipmentLabTest.Visible = true;
            }

            /*
            gv_Options.Columns[SizeOptionColumnId.ShippingGSP ].Visible = isLabelVisible;
            gv_Options.Columns[SizeOptionColumnId.ShippingGSPInput].Visible = isEditable;

            */
            
            //if (gv_Options.Columns[SizeOptionColumnId.ShippedQtyInput].Visible || gv_Options.Columns[SizeOptionColumnId.ShippingGSPInput].Visible)
            if (gv_Options.Columns[SizeOptionColumnId.ShippedQtyInput].Visible)
            {
                gv_Options.DataSource = vwDomainShipmentDef.ShipmentDetails;
                gv_Options.DataBind();
            }
            

            gv_DocInfoUpdate.Visible = isEditable;
            if (isEditable)
            {
                gv_DocInfoUpdate.DataSource = vwDomainShipmentDef.UpdatedDocuments;
                gv_DocInfoUpdate.DataBind();
            }
            gv_DocInfo.Visible = isLabelVisible;

            if (isEditable)
            {   // Payment Deduction Edit mode
                gv_PaymentDeductionUpdate.DataSource = vwDomainShipmentDef.UpdatedPaymentDeduction;
                gv_PaymentDeductionUpdate.DataBind();
                gv_PaymentDeductionUpdate.Visible = true;
                gv_PaymentDeduction.Visible = false;
                if (ckb_ShipDocCheck.Checked)
                {   // Not allow to add new deduction
                    btn_NewPaymentDeduction.Visible = false;
                }
            }
            else
            {   // Payment Deduction Read Only mode
                gv_PaymentDeduction.Visible = true;
                gv_PaymentDeductionUpdate.Visible = false;
            }

            

            bool anyLabTestIncome = false;
            bool anySplitShipment = false;
            if (vwDomainShipmentDef.SplitShipments.Count > 0)
                foreach (SplitShipmentDef spt in vwDomainShipmentDef.SplitShipments)
                {
                    anySplitShipment = anySplitShipment || (spt.IsVirtualSetSplit == 0);
                    anyLabTestIncome = (anyLabTestIncome || (spt.LabTestIncome > 0 && spt.IsVirtualSetSplit == 0));
                }
            if (!anySplitShipment)
                anyLabTestIncome = (vwDomainShipmentDef.Shipment.LabTestIncome != 0);

            if (vwDomainShipmentDef.Contract.Office.OfficeCode == "TR" || vwDomainShipmentDef.Contract.Office.OfficeCode == "PK")
            {
                row_QAComm.Visible = false;
                row_TRDiscount.Visible = true;
                row_LabTest.Visible = anyLabTestIncome;
            }
            else if (vwDomainShipmentDef.Contract.Office.OfficeCode == "HK" || vwDomainShipmentDef.Contract.Office.OfficeCode == "SH" ||
                vwDomainShipmentDef.Contract.Office.OfficeCode == "TH")
            {
                row_QAComm.Visible = true;
                row_TRDiscount.Visible = false;
                row_LabTest.Visible = anyLabTestIncome;
            }
            else if (vwDomainShipmentDef.Contract.Office.OfficeCode == "LK")
            {
                row_QAComm.Visible = false;
                row_TRDiscount.Visible = (vwDomainShipmentDef.Shipment.CountryOfOrigin.CountryOfOriginId == CountryOfOriginRef.eCountryId.Pakistan.GetHashCode());
                row_LabTest.Visible = true;
            }
            else
            {
                row_QAComm.Visible = vwDomainShipmentDef.Shipment.QACommissionPercent != 0;
                row_TRDiscount.Visible = vwDomainShipmentDef.Shipment.VendorPaymentDiscountPercent != 0;
                row_LabTest.Visible = anyLabTestIncome;
            }


            lbl_LCNo.Visible = !txt_LCNo.Visible;
            lbl_LCBillRefNo.Visible = !txt_LCBillRefNo.Visible;
            lbl_IssueDate.Visible = !txt_IssueDate.Visible;
            lbl_LCExpiryDate.Visible = !txt_LCExpiryDate.Visible;
            lbl_LCAmount.Visible = !txt_LCAmount.Visible;
            lbl_PaymentCheckDate.Visible = !txt_PaymentCheckDate.Visible;
            lbl_NotesToQCC.Visible = isLabelVisible;
            lbl_ActualInWHDate.Visible = isLabelVisible;
            lbl_BkInWHDate.Visible = isLabelVisible;
            lbl_BookingQty.Visible = isLabelVisible;
            lbl_BookingDate.Visible = isLabelVisible;
            lbl_LotShipOrderNo.Visible = isLabelVisible;
            lbl_InvDate.Visible = !txt_InvDate.Visible;
            lbl_ShipFrom.Visible = isLabelVisible;
            lbl_ShipTo.Visible = isLabelVisible;
            lbl_VendorInvoiceNo.Visible = !txt_VendorInvoiceNo.Visible; //isLabelVisible
            lbl_ShipRemark.Visible = isLabelVisible;
            lbl_InvPiece.Visible = isLabelVisible;
            lbl_InvPack.Visible = isLabelVisible;
            lbl_ExLicenceFee.Visible = isLabelVisible;
            lbl_QuotaCharge.Visible = isLabelVisible;
            lbl_TotalDuty.Visible = isLabelVisible;
            lbl_DFDebitNoteNo.Visible = !txt_DFDebitNoteNo.Visible;
            lbl_DocCharge.Visible = !txt_DocCharge.Visible;
            lbl_TransportCharge.Visible = !txt_TransportCharge.Visible;
            lbl_CourierCharge.Visible = isLabelVisible || txt_CourierCharge.ReadOnly;
            lbl_DebitNoteNo.Visible = true;     //isLabelVisible
            lbl_ImportDutyActualAmt.Visible = !txt_ImportDutyActualAmt.Visible;
            lbl_ImportDutyRecordCheckedDate.Visible = !txt_ImportDutyRecordCheckedDate.Visible;
            lbl_InputVATActualAmt.Visible = !txt_InputVATActualAmt.Visible;
            lbl_InputVATRecordCheckedDate.Visible = !txt_InputVATRecordCheckedDate.Visible;
            lbl_OutputVATActualAmt.Visible = !txt_OutputVATActualAmt.Visible;
            lbl_OutputVATRecordCheckedDate.Visible = !txt_OutputVATRecordCheckedDate.Visible;
            lbl_ShipRecDocDate.Visible = isLabelVisible || !txt_ShipRecDocDate.Visible;
            txt_InvRemark.ReadOnly = isLabelVisible;
            lbl_ItemColor.Visible = isLabelVisible;
            lbl_Desc1.Visible = isLabelVisible;
            lbl_Desc2.Visible = isLabelVisible;
            lbl_Desc3.Visible = isLabelVisible;
            lbl_Desc4.Visible = isLabelVisible;
            lbl_Desc5.Visible = isLabelVisible;
            lbl_RetailDesc.Visible = isLabelVisible;

            if (isEditable)
            {
                txt_Desc1.Text = lbl_Desc1.Text;
                txt_Desc2.Text = lbl_Desc2.Text;
                txt_Desc3.Text = lbl_Desc3.Text;
                txt_Desc4.Text = lbl_Desc4.Text;
                txt_Desc5.Text = lbl_Desc5.Text;
                txt_RetailDesc.Text = lbl_RetailDesc.Text;
            }

            //Handling the Master Item Description
            pnl_ItemMasterIcon.Style.Add(HtmlTextWriterStyle.Display, (isEditable ? "block" : "none"));
            ProductDef pd = vwDomainShipmentDef.Product;
            if (pd.MasterDescription1 == "" && pd.MasterDescription2 == "" && pd.MasterDescription3 == "" && pd.MasterDescription4 == "" && pd.MasterDescription5 == "")
            {    //btn_copyItemDescFromMaster.Style.Add(HtmlTextWriterStyle.Display,"none");
                btn_copyItemDescFromMaster.ToolTip = "No Master Item Description";
                btn_copyItemDescFromMaster.Enabled = false;
            }
            else
            {   //btn_copyItemDescFromMaster.Style.Add(HtmlTextWriterStyle.Display,"block");
                btn_copyItemDescFromMaster.ToolTip = "Copy Item Description From Master Description";
                btn_copyItemDescFromMaster.Enabled = true;
            }
            //btn_setItemDescriptionAsMaster.Style.Add(HtmlTextWriterStyle.Display, (vwDomainShipmentDef.Invoice.InvoicePrefix != null ? "block" : "none"));
            if (string.IsNullOrEmpty(vwDomainShipmentDef.Invoice.InvoicePrefix))
            {   //btn_setItemDescriptionAsMaster.Style.Add(HtmlTextWriterStyle.Display, "none");
                btn_setItemDescriptionAsMaster.Enabled = false;
                btn_setItemDescriptionAsMaster.ToolTip = "Shipment has not been invoiced yet, you are not allowed to set the item description as master";
            }
            else
            {   //btn_setItemDescriptionAsMaster.Style.Add(HtmlTextWriterStyle.Display, "block");
                btn_setItemDescriptionAsMaster.Enabled = true;
                btn_setItemDescriptionAsMaster.ToolTip = "Set the Item Description as master";
            }
            ckb_setMaster.Checked = false;

            lbl_MasterItemDesc1.Style.Add(HtmlTextWriterStyle.Display, (lbl_MasterItemDesc1.Text == "" ? "none" : "block"));
            lbl_MasterItemDesc2.Style.Add(HtmlTextWriterStyle.Display, (lbl_MasterItemDesc2.Text == "" ? "none" : "block"));
            lbl_MasterItemDesc3.Style.Add(HtmlTextWriterStyle.Display, (lbl_MasterItemDesc3.Text == "" ? "none" : "block"));
            lbl_MasterItemDesc4.Style.Add(HtmlTextWriterStyle.Display, (lbl_MasterItemDesc4.Text == "" ? "none" : "block"));
            lbl_MasterItemDesc5.Style.Add(HtmlTextWriterStyle.Display, (lbl_MasterItemDesc5.Text == "" ? "none" : "block"));

            btn_cal_InvDate.Visible = txt_InvDate.Visible;
            btn_cal_LCIssueDate.Visible = txt_IssueDate.Visible;
            btn_cal_BookingDate.Visible = txt_BookingDate.Visible;
            btn_cal_BkInWHDate.Visible = txt_BkInWHDate.Visible;
            btn_cal_ActualInWHDate.Visible = txt_ActualInWHDate.Visible;

            if (vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
            {
                if ((vwDomainShipmentDef.Invoice.InvoiceUploadUser != null && vwDomainShipmentDef.Invoice.InvoiceUploadUser.UserId == this.LogonUserId &&
                        CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.PrintInvoice)) ||
                    CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.PrintAllInvoice))
                    btn_Print.Visible = true;
                else
                    btn_Print.Visible = false;
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.PrintDebitNoteInShipmentDetail))
            {
                btn_PrintDN.Visible = false;
                if (vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id
                    && vwDomainShipmentDef.Shipment.NSLCommissionPercentage > 0)
                {
                    CutOffStatusRef cutOffStatus = null;
                    if (vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.fashionUK.GetHashCode())
                    {
                        cutOffStatus = AccountManager.Instance.getCutOffStatus(vwDomainShipmentDef.Shipment.ShipmentId);
                    }
                    if (vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.smithbrooks.GetHashCode() 
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.brand_international.GetHashCode() 
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.blues_clothing.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.william_lamb.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.bioworld.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.aykroyd_sons.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.global_licensing.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.paul_dennicci.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.difuzed.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.tvm_fashion.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.brand_design.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.brand_alliance.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.poetic_brands.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.cortina.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.sicem.GetHashCode()
                        || vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.cooneen.GetHashCode()
                        || (vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.fashionUK.GetHashCode() && cutOffStatus != null && (cutOffStatus.FiscalYear > 2020 || (cutOffStatus.FiscalYear == 2020 && cutOffStatus.Period >= 9) || vwDomainShipmentDef.Shipment.ShipmentId == 1236582 || vwDomainShipmentDef.Shipment.ShipmentId == 1236580 || vwDomainShipmentDef.Shipment.ShipmentId == 1252803 || vwDomainShipmentDef.Shipment.ShipmentId == 1239762))
                        //|| vwDomainShipmentDef.Contract.Customer.CustomerId == CustomerDef.Id.fabric_flavours.GetHashCode()
                        ) // Forever New -> No NS Comm.
                    {
                        btn_PrintDN.Text = "Print Debit Note";
                        btn_PrintDN.Visible = true;
                    }
                }
            }

            if (isLabelVisible)
            {
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.ModifyShipment))
                    btn_Edit.Visible = true;
                if (vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.PENDING_FOR_SUBMIT.Id)
                    btn_Edit.Visible = false;

                if (vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id
                    && vwDomainShipmentDef.Contract.Customer.CustomerType.CustomerTypeId == CustomerTypeRef.Type.ezibuy.GetHashCode()
                    && !vwDomainShipmentDef.Invoice.IsReadyToSendInvoice
                    && CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.ModifyShipment))
                {
                    btn_Send.Visible = true;
                    if (vwDomainShipmentDef.Invoice.InvoiceSentDate == DateTime.MinValue)
                    {
                        btn_Send.Text = "Send";
                        btn_Send.ToolTip = "Ready To Send Invoice";
                    }
                    else
                    {
                        btn_Send.Text = "Re-Send";
                        btn_Send.ToolTip = "Ready To Re-Send Invoice";
                    }
                }
                else
                    btn_Send.Visible = false;
            }
            else
            {
                btn_Edit.Visible = false;
                btn_Send.Visible = false;
            }

            if (vwDomainShipmentDef.Contract.Customer.CustomerType.CustomerTypeId == CustomerTypeRef.Type.ezibuy.GetHashCode())
                row_EzibuyInvSent.Visible = true;

            if (btn_Save.Visible)
                btn_Close.Attributes.Add("onclick", "if (confirm('You have not save your changes.\\nConfirm to close?')) window.close(); return false;");
            else
                btn_Close.Attributes.Add("onclick", "window.close(); return false;");

            // PREV & NEXT Button control
            bool found = false;
            string prevShipment, nextShipment, currShipment;
            string inputShipmentIdParam = "";
            string defaultReceiptDate = "";
            ArrayList inputShipmentIdList = new ArrayList();

            prevShipment = nextShipment = currShipment = "";
            //if (!string.IsNullOrEmpty(Request.Params["ShipmentId"]))
            currShipment = decryptParameter(Request.Params["ShipmentId"]);
            //if (!string.IsNullOrEmpty(Request.Params["DefaultReceiptDate"]))
            defaultReceiptDate = decryptParameter(Request.Params["DefaultReceiptDate"]);
            //if (!string.IsNullOrEmpty(Request.Params["ShipmentIdList"]))
            inputShipmentIdParam = decryptParameter(Request.Params["ShipmentIdList"]);
            if (!string.IsNullOrEmpty(inputShipmentIdParam))
            {
                string[] shipmentIdArray = inputShipmentIdParam.Split(char.Parse("|"));
                foreach (string id in shipmentIdArray)
                {
                    if (currShipment == "")
                        currShipment = id;
                    if (found)
                    {
                        nextShipment = id;
                        break;
                    }

                    if (!string.IsNullOrEmpty(id))
                        if (id == currShipment)
                            found = true;
                        else
                            prevShipment = id;
                }
            }
            inputShipmentIdList.Clear();

            if (lnk_Prev.Enabled = (prevShipment != ""))
            {
                lnk_Prev.Attributes.Add("href", "ShipmentDetail.aspx?ShipmentId=" + encryptParameter(prevShipment) + "&ShipmentIdList=" + encryptParameter(inputShipmentIdParam) + (string.IsNullOrEmpty(defaultReceiptDate) ? "" : "&DefaultReceiptDate=" + encryptParameter(defaultReceiptDate)));
                if (btn_Save.Visible)
                    lnk_Prev.Attributes.Add("onclick", "if (!confirm('You have not save your changes.\\nPlease confirm to skip saving and view the previous shipment')) return false; ");
                else
                    lnk_Prev.Attributes.Remove("onclick");
            }
            else
                lnk_Prev.Attributes.Remove("title");
            if (lnk_Next.Enabled = (nextShipment != ""))
            {
                lnk_Next.Attributes.Add("href", "ShipmentDetail.aspx?ShipmentId=" + encryptParameter(nextShipment) + "&ShipmentIdList=" + encryptParameter(inputShipmentIdParam) + (string.IsNullOrEmpty(defaultReceiptDate) ? "" : "&DefaultReceiptDate=" + encryptParameter(defaultReceiptDate)));
                if (btn_Save.Visible)
                    lnk_Next.Attributes.Add("onclick", "if (!confirm('You have not save your changes.\\nPlease confirm to skip saving and view the next shipment.')) return false; ");
                else
                    lnk_Next.Attributes.Remove("onclick");
            }
            else
                lnk_Next.Attributes.Remove("title");

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.SendDiscrepancyAlert) &&
                vwDomainShipmentDef.Invoice.ARDate != DateTime.MinValue && vwDomainShipmentDef.Invoice.ARAmt != 0 && vwDomainShipmentDef.Invoice.ARAmt != vwDomainShipmentDef.Shipment.TotalShippedAmount)
            {
                btn_DiscrepancyAlert.Visible = true;
                gv_DiscrepancyAlertLog.DataSource = ShipmentManager.Instance.getActionHistoryByShipmentIdAndType(ShipmentId, ActionHistoryType.SEND_AR_DISCREPANCY_ALERT.Id);
                gv_DiscrepancyAlertLog.DataBind();
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.AuditView))
            {
                tabDocInfo.Visible = false;
                tabBooking.Visible = false;
                tabInvRemark.Visible = false;
                tabSplitShipment.Visible = false;
                tabOtherDel.Visible = false;
                tabManifest.Visible = false;
                tabAuditLog.Visible = false;
                TabDeduction.Visible = false;

                btn_Edit.Visible = false;
                btn_Send.Visible = false;
                btn_DiscrepancyAlert.Visible = false;

                img_UploadedDoc.Visible = false;
                //tr_UploadDoc.Visible = false;
            }

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.HideSupplierAmountInfo))
            {
                tabAccount.Visible = false;
                row_SupplierAmount.Style.Add(HtmlTextWriterStyle.Display, "none");
                row_QAComm.Style.Add(HtmlTextWriterStyle.Display, "none");
                row_TRDiscount.Style.Add(HtmlTextWriterStyle.Display, "none");
                row_LabTest.Style.Add(HtmlTextWriterStyle.Display, "none");
                row_NetAmountToSupplier.Style.Add(HtmlTextWriterStyle.Display, "none");
                row_ShipDocCheck.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            row_VMVendor.Visible = (vwDomainShipmentDef.Contract.IsNextMfgOrder == 1 && vwDomainShipmentDef.Contract.IsPOIssueToNextMfg == 1);
            row_Factory.Visible = (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.ViewFactory));
        }

        bool updateShipmentDetail()
        {
            TextBox txt;
            SmartDropDownList ddl;
            ShipmentDetailDef shipmentDetail;
            bool isUpdated = false;
            int qty = 0;
            bool isQtyEditable = false;
            bool isValid = true;

            foreach (GridViewRow row in gv_Options.Rows)
            {
                //ddl = (SmartDropDownList)row.Cells[SizeOptionColumnId.ShippingGSPInput].FindControl("ddl_ShippingGSP");
                shipmentDetail = (ShipmentDetailDef)vwDomainShipmentDef.ShipmentDetails[row.RowIndex];

                isQtyEditable = gv_Options.Columns[SizeOptionColumnId.ShippedQtyInput].Visible;
                if (isQtyEditable)
                {
                    txt = (TextBox)row.Cells[SizeOptionColumnId.ShippedQtyInput].FindControl("txt_ShipQty");
                    if ((vwDomainShipmentDef.Invoice != null && vwDomainShipmentDef.Invoice.InvoiceDate == DateTime.MinValue && txt_InvDate.Text != "") &&
                        (txt.Text == "0" || txt.Text == ""))
                    {
                        isValid = false;
                        //sWarningMsg += string.Format("Size option {0} is invoiced with 0 shipped quantity." + Environment.NewLine, shipmentDetail.SizeOption.SizeOptionNo);
                    }
                    if (!int.TryParse(txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out qty))
                    {
                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid shipped quantity.');", true);
                        isValid = false;
                        return false;
                    }
                    else if (qty != shipmentDetail.ShippedQuantity)
                    {
                        shipmentDetail.ShippedQuantity = qty;
                        isUpdated = true;
                    }
                }

                /*
                if (ddl.SelectedValue != shipmentDetail.ShippingGSPFormTypeId.ToString())
                {
                    shipmentDetail.ShippingGSPFormTypeId = Convert.ToInt32(ddl.SelectedValue);
                    isUpdated = true;
                }
                */

                if (isUpdated)
                {
                    if (vwDomainShipmentDef.UpdatedShipmentDetails == null)
                        vwDomainShipmentDef.UpdatedShipmentDetails = new ArrayList();

                    vwDomainShipmentDef.UpdatedShipmentDetails.Add(shipmentDetail);
                }
                isUpdated = false;
            }
            //return true;
            return isValid;
        }

        bool validateShipmentDetail()
        {
            TextBox txt;
            ShipmentDetailDef shipmentDetail;
            int qty, totalQty = 0;
            decimal totalSalesAmt = 0;
            decimal totalPurchaseAmt = 0;
            string sWarningMsg = "";
            bool isQtyEditable = false;

            isQtyEditable = gv_Options.Columns[SizeOptionColumnId.ShippedQtyInput].Visible;
            if (isQtyEditable)
            {
                foreach (GridViewRow row in gv_Options.Rows)
                {
                    //ddl = (SmartDropDownList)row.Cells[16].FindControl("ddl_ShippingGSP");
                    shipmentDetail = (ShipmentDetailDef)vwDomainShipmentDef.ShipmentDetails[row.RowIndex];

                    txt = (TextBox)row.Cells[6].FindControl("txt_ShipQty");
                    if ((vwDomainShipmentDef.Invoice != null && vwDomainShipmentDef.Invoice.InvoiceDate == DateTime.MinValue && txt_InvDate.Text != "") &&
                        (txt.Text == "0" || txt.Text == ""))
                    {
                        sWarningMsg += string.Format("Size option {0} is invoiced with 0 shipped quantity." + Environment.NewLine, shipmentDetail.SizeOption.SizeOptionNo);
                    }
                    if (!int.TryParse(txt.Text, System.Globalization.NumberStyles.AllowThousands, null, out qty))
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Invalid shipped quantity.');", true);
                        return false;
                    }

                    totalQty += qty;
                    totalSalesAmt += qty * shipmentDetail.SellingPrice;
                    totalPurchaseAmt += qty * shipmentDetail.SupplierGarmentPrice;
                }
            }
            //if (sWarningMsg != "")
            //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('"+  sWarningMsg + "');", true);
            //if (totalQty == 0 && !vwDomainShipmentDef.Shipment.PaymentLock && (txt_InvDate.Text != "" || (vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id)))
            //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Total Shipped Quantity is zero. Please confirm your data input.')", true);
            //    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "if (!confirm('Total Shipped Quantity is zero. Please confirm your data input.')) return false", true);
            amendingShippedQtyTotal = totalQty;
            amendingSalesAmountTotal = totalSalesAmt;
            amendingPurchaseAmountTotal = totalPurchaseAmt;

            if (isChoiceTotalAmountAutoUpdate && isQtyEditable)
            {
                txt_ChoiceActSalesAmt.Text = totalSalesAmt.ToString();
                txt_ChoiceActPurchaseAmt.Text = totalPurchaseAmt.ToString();
            }

            return true;
        }

        bool isShipDocInfoValidInSplitShipment()
        {
            bool valid = true;
            if (ckb_ShipDocCheck.Enabled) // && ckb_ShipDocCheck.Checked && vwDomainShipmentDef.Invoice.ShippingDocCheckedOn == DateTime.MinValue)
            {
                UserRef usr = CommonUtil.getUserByKey(this.LogonUserId);
                ArrayList splitList = (ArrayList)OrderManager.Instance.getSplitShipmentByShipmentId(vwDomainShipmentDef.Shipment.ShipmentId);
                if (splitList != null)
                    foreach (SplitShipmentDef split in splitList)
                        if (split.IsVirtualSetSplit == 0 && (string.IsNullOrEmpty(split.SupplierInvoiceNo)))//|| split.ShippingDocReceiptDate == DateTime.MinValue)
                        {
                            valid = false;
                            break;
                        }
            }
            return valid;
        }

        void updateInvoice(DomainShipmentDef domainShipmentDef)
        {
            domainShipmentDef.Invoice.BookingSONo = txt_LotShipOrderNo.Text.Trim();
            if (txt_BookingDate.Text != "")
                domainShipmentDef.Invoice.BookingDate = DateTimeUtility.getDate(txt_BookingDate.Text);
            else
                domainShipmentDef.Invoice.BookingDate = DateTime.MinValue;
            if (txt_BookingQty.Text != "")
                domainShipmentDef.Invoice.BookingQty = int.Parse(txt_BookingQty.Text, System.Globalization.NumberStyles.AllowThousands);
            else
                domainShipmentDef.Invoice.BookingQty = 0;
            if (txt_BkInWHDate.Text != "")
                domainShipmentDef.Invoice.BookingAtWarehouseDate = DateTimeUtility.getDate(txt_BkInWHDate.Text);
            else
                domainShipmentDef.Invoice.BookingAtWarehouseDate = DateTime.MinValue;
            if (txt_ActualInWHDate.Text != "")
                domainShipmentDef.Invoice.ActualAtWarehouseDate = DateTimeUtility.getDate(txt_ActualInWHDate.Text);
            else
                domainShipmentDef.Invoice.ActualAtWarehouseDate = DateTime.MinValue;
            if (txt_InvDate.Text != "")
                domainShipmentDef.Invoice.InvoiceDate = DateTimeUtility.getDate(txt_InvDate.Text);
            else
                domainShipmentDef.Invoice.InvoiceDate = DateTime.MinValue;
            if (ddl_ShipFrom.SelectedIndex > 0)
            {
                domainShipmentDef.Invoice.ShipFromCountry = CommonManager.Instance.getShipmentCountryByKey(int.Parse(ddl_ShipFrom.SelectedValue));
            }
            else
                domainShipmentDef.Invoice.ShipFromCountry = null;
            if (ddl_ShipTo.SelectedIndex > 0)
            {
                domainShipmentDef.Invoice.CustomerDestination = CommonManager.Instance.getCustomerDestinationByKey(int.Parse(ddl_ShipTo.SelectedValue));
            }
            else
                domainShipmentDef.Invoice.CustomerDestination = null;
            domainShipmentDef.Invoice.SupplierInvoiceNo =  WebUtil.filterSpecialCharacter(txt_VendorInvoiceNo.Text.Trim());
            domainShipmentDef.Invoice.ShippingRemark = txt_ShipRemark.Text.Trim();
            if (txt_InvPiece.Text != "")
                domainShipmentDef.Invoice.PiecesPerDeliveryUnit = int.Parse(txt_InvPiece.Text);
            else
                domainShipmentDef.Invoice.PiecesPerDeliveryUnit = 0;

            domainShipmentDef.Invoice.PackingMethod = WebUtil.getPackingMethodByKey(Convert.ToInt32(ddl_InvPack.SelectedValue));
            domainShipmentDef.Invoice.ItemColour = txt_ItemColor.Text.Trim();
            domainShipmentDef.Invoice.ItemDesc1 = txt_Desc1.Text.Trim();
            domainShipmentDef.Invoice.ItemDesc2 = txt_Desc2.Text.Trim();
            domainShipmentDef.Invoice.ItemDesc3 = txt_Desc3.Text.Trim();
            domainShipmentDef.Invoice.ItemDesc4 = txt_Desc4.Text.Trim();
            domainShipmentDef.Invoice.ItemDesc5 = txt_Desc5.Text.Trim();

            if (txt_ExLicenceFee.Text != "")
                domainShipmentDef.Invoice.ExportLicenceFee = decimal.Parse(txt_ExLicenceFee.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
            else
                domainShipmentDef.Invoice.ExportLicenceFee = 0;
            if (txt_QuotaCharge.Text != "")
                domainShipmentDef.Invoice.QuotaCharge = decimal.Parse(txt_QuotaCharge.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
            else
                domainShipmentDef.Invoice.QuotaCharge = 0;

            // Update Shipping Document status info
            domainShipmentDef.Invoice.ShippingDocReceiptDate = DateTimeUtility.getDate(txt_ShipRecDocDate.Text);
            UserRef usr = CommonUtil.getUserByKey(this.LogonUserId);
            DateTime nextWorkingDate = DateTimeUtility.getNextWorkingDate(DateTime.Now);
            //if (ShipmentManager.Instance.isReadyForDMS(domainShipmentDef.Contract.Office, domainShipmentDef.Shipment.WorkflowStatus, domainShipmentDef.Invoice.InvoiceDate, domainShipmentDef.Shipment.Vendor))
            if (ShipmentManager.Instance.isReadyForDMS(domainShipmentDef))
            {
                if (domainShipmentDef.Shipment.ShippingDocWFS.Id == ShippingDocWFS.NOT_READY.Id || domainShipmentDef.Shipment.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                {   // Update shipping Doc status
                    decimal amt;
                    if (!decimal.TryParse(txt_ShipDocCheckAmount.Text, out amt))
                        amt = 0;

                    if (ckb_ShipDocCheck.Checked && domainShipmentDef.Invoice.ShippingDocCheckedOn == DateTime.MinValue)
                    {   // Check the checkbox
                        domainShipmentDef.Invoice.ShippingCheckedTotalNetAmount = amt;
                        domainShipmentDef.Invoice.ShippingDocCheckedOn = DateTime.Now;
                        domainShipmentDef.Invoice.ShippingDocCheckedBy = usr;
                        if (domainShipmentDef.Shipment.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                        {
                            domainShipmentDef.Shipment.RejectPaymentReasonId = RejectPaymentReason.NoReason.Id;
                            domainShipmentDef.Invoice.AccountDocReceiptDate = nextWorkingDate;
                            domainShipmentDef.Shipment.ShippingDocWFS = ShippingDocWFS.ACCEPTED;
                            //if (domainShipmentDef.Shipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.OPENACCOUNT.GetHashCode()
                            //    || (domainShipmentDef.Shipment.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode()
                            //        && ckb_PaymentChecked.Checked))
                            //{
                            //    domainShipmentDef.Invoice.AccountDocReceiptDate = nextWorkingDate;
                            //}
                        }

                        // update the shipping doc info in split record
                        if (domainShipmentDef.SplitShipments != null)
                            foreach (SplitShipmentDef spt in domainShipmentDef.SplitShipments)
                                if (spt.IsVirtualSetSplit == 0)
                                {
                                    decimal totalAmt = spt.TotalShippedSupplierGarmentAmountAfterDiscount;
                                    spt.ShippingCheckedTotalNetAmount = totalAmt
                                            - Math.Round(totalAmt * (spt.QACommissionPercent / 100), 2, MidpointRounding.AwayFromZero)
                                            - Math.Round(spt.TotalShippedQuantity * spt.LabTestIncome, 2, MidpointRounding.AwayFromZero);
                                    //- Math.Round(totalAmt * (spt.VendorPaymentDiscountPercent / 100), 2, MidpointRounding.AwayFromZero);
                                    spt.ShippingDocCheckedOn = DateTime.Now;
                                    spt.ShippingDocCheckedBy = usr;
                                    if (spt.ShippingDocWFS.Id == ShippingDocWFS.REJECTED.Id)
                                    {
                                        spt.RejectPaymentReasonId = RejectPaymentReason.NoReason.Id;
                                        spt.AccountDocReceiptDate = nextWorkingDate;
                                        spt.ShippingDocWFS = ShippingDocWFS.ACCEPTED;
                                        //if (spt.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.OPENACCOUNT.GetHashCode()
                                        //    || (spt.PaymentTerm.PaymentTermId == PaymentTermRef.eTerm.LCATSIGHT.GetHashCode()
                                        //        && (ckb_PaymentChecked.Checked || spt.LCPaymentCheckedDate!=DateTime.MinValue)))
                                        //{
                                        //    spt.AccountDocReceiptDate = nextWorkingDate;
                                        //}
                                    }
                                    if (spt.ShippingDocReceiptDate == DateTime.MinValue)
                                        spt.ShippingDocReceiptDate = DateTimeUtility.getDate(txt_ShipRecDocDate.Text);
                                }
                    }
                    else if (!ckb_ShipDocCheck.Checked && domainShipmentDef.Invoice.ShippingDocCheckedOn != DateTime.MinValue)
                    {   // Uncheck the checkbox
                        domainShipmentDef.Invoice.ShippingCheckedTotalNetAmount = 0;
                        domainShipmentDef.Invoice.ShippingDocCheckedOn = DateTime.MinValue;
                        domainShipmentDef.Invoice.ShippingDocCheckedBy = null;
                    }
                }
            }

            domainShipmentDef.Invoice.InvoiceRemark = txt_InvRemark.Text.Trim();
            domainShipmentDef.Invoice.LCNo = txt_LCNo.Text.Trim();
            domainShipmentDef.Invoice.LCBillRefNo = txt_LCBillRefNo.Text.Trim();
            if (txt_IssueDate.Text != "")
                domainShipmentDef.Invoice.LCIssueDate = DateTimeUtility.getDate(txt_IssueDate.Text);
            else
                domainShipmentDef.Invoice.LCIssueDate = DateTime.MinValue;

            if (txt_LCExpiryDate.Text.Trim() != "")
                domainShipmentDef.Invoice.LCExpiryDate = DateTimeUtility.getDate(txt_LCExpiryDate.Text);
            else
                domainShipmentDef.Invoice.LCExpiryDate = DateTime.MinValue;

            if (txt_LCAmount.Text.Trim() != "")
                domainShipmentDef.Invoice.LCAmount = decimal.Parse(txt_LCAmount.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
            else
                domainShipmentDef.Invoice.LCAmount = 0;

            domainShipmentDef.Invoice.QCCRemark = txt_NotesToQCC.Text.Trim();

            if (ckb_PaymentChecked.Checked)
            {
                domainShipmentDef.Invoice.IsLCPaymentChecked = 1;
                domainShipmentDef.Invoice.LCPaymentCheckedDate = DateTimeUtility.getDate(txt_PaymentCheckDate.Text);
            }
            else
            {
                domainShipmentDef.Invoice.IsLCPaymentChecked = 0;
                domainShipmentDef.Invoice.LCPaymentCheckedDate = DateTime.MinValue;
            }

            //courier charge
            if (domainShipmentDef.Shipment.IsMockShopSample == 1 || domainShipmentDef.Shipment.IsPressSample == 1)
            {
                if (txt_CourierCharge.Text.Trim() != "")
                    domainShipmentDef.Invoice.CourierChargeToNUK = decimal.Parse(txt_CourierCharge.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
                else
                    domainShipmentDef.Invoice.CourierChargeToNUK = 0;
                domainShipmentDef.Invoice.CourierChargeToNUKDebitNoteNo = txt_DebitNoteNo.Text.Trim();
            }

            //direct franchise
            if ((domainShipmentDef.Contract.Customer.CustomerId == 1 || domainShipmentDef.Contract.Customer.CustomerId == 2) &&
                CustomerDestinationDef.isDFOrder(domainShipmentDef.Shipment.CustomerDestinationId))
            {
                domainShipmentDef.Invoice.DirectFranchiseDebitNoteNo = txt_DFDebitNoteNo.Text == "" ? string.Empty : txt_DFDebitNoteNo.Text;
                domainShipmentDef.Invoice.DirectFranchiseDocumentCharge = txt_DocCharge.Text == "" ? 0 : decimal.Parse(txt_DocCharge.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
                domainShipmentDef.Invoice.DirectFranchiseTransportationCharge = txt_TransportCharge.Text == "" ? 0 : decimal.Parse(txt_TransportCharge.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
            }

            //u-turn order
            //if (CommonManager.Instance.getCustomerDestinationByKey(domainShipmentDef.Shipment.CustomerDestinationId).UTurnOrder == 1)
            if (domainShipmentDef.Shipment.TermOfPurchase.TermOfPurchaseId == TermOfPurchaseRef.Id.FOB_UT.GetHashCode())
            {
                domainShipmentDef.Invoice.ImportDutyActualAmt = txt_ImportDutyActualAmt.Text == "" ? 0 : decimal.Parse(txt_ImportDutyActualAmt.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
                if (ckb_ImportDutyRecordChecked.Checked)
                {
                    domainShipmentDef.Invoice.IsImportDutyChecked = 1;
                    domainShipmentDef.Invoice.ImportDutyCheckedDate = DateTimeUtility.getDate(txt_ImportDutyRecordCheckedDate.Text);
                }
                else
                {
                    domainShipmentDef.Invoice.IsImportDutyChecked = 0;
                    domainShipmentDef.Invoice.ImportDutyCheckedDate = DateTime.MinValue;
                }

                domainShipmentDef.Invoice.InputVATActualAmt = txt_InputVATActualAmt.Text == "" ? 0 : decimal.Parse(txt_InputVATActualAmt.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
                if (ckb_InputVATRecordChecked.Checked)
                {
                    domainShipmentDef.Invoice.IsInputVATChecked = 1;
                    domainShipmentDef.Invoice.InputVATCheckedDate = DateTimeUtility.getDate(txt_InputVATRecordCheckedDate.Text);
                }
                else
                {
                    domainShipmentDef.Invoice.IsInputVATChecked = 0;
                    domainShipmentDef.Invoice.InputVATCheckedDate = DateTime.MinValue;
                }

                domainShipmentDef.Invoice.OutputVATActualAmt = txt_OutputVATActualAmt.Text == "" ? 0 : decimal.Parse(txt_OutputVATActualAmt.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
                if (ckb_OutputVATRecordChecked.Checked)
                {
                    domainShipmentDef.Invoice.IsOutputVATChecked = 1;
                    domainShipmentDef.Invoice.OutputVATCheckedDate = DateTimeUtility.getDate(txt_OutputVATRecordCheckedDate.Text);
                }
                else
                {
                    domainShipmentDef.Invoice.IsOutputVATChecked = 0;
                    domainShipmentDef.Invoice.OutputVATCheckedDate = DateTime.MinValue;
                }
            }


            //Choice Order 
            decimal amount = 0;
            if (txt_ChoiceActSalesAmt.Visible || isChoiceTotalAmountAutoUpdate)
                domainShipmentDef.Invoice.ChoiceOrderTotalShippedAmount = (txt_ChoiceActSalesAmt.Text == "" ? 0 : (decimal.TryParse(txt_ChoiceActSalesAmt.Text, out amount) ? amount : 0));
            if (txt_ChoiceActPurchaseAmt.Visible || isChoiceTotalAmountAutoUpdate)
                domainShipmentDef.Invoice.ChoiceOrderTotalShippedSupplierGarmentAmount = (txt_ChoiceActPurchaseAmt.Text == "" ? 0 : (decimal.TryParse(txt_ChoiceActPurchaseAmt.Text, out amount) ? amount : 0));
            
            domainShipmentDef.Invoice.IsSyncToFactory = true;

            if (domainShipmentDef.Invoice.InvoiceUploadUser == null || domainShipmentDef.Invoice.InvoiceUploadUser.UserId == 99999)
                domainShipmentDef.Invoice.InvoiceUploadUser = GeneralManager.Instance.getUserByKey(this.LogonUserId);
        }

        #endregion


        #region Validator

        protected void val_Invoice_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            val_Invoice.ErrorMessage = "";
            args.IsValid = true;
            int num;
            DateTime inputDate;

            //if ((txt_InvDate.Visible && txt_InvDate.Text != "") || vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
            if (((txt_InvDate.Visible && txt_InvDate.Text != "") || vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id) && (lbl_InvDate.Text != txt_InvDate.Text))
            {
                if (!DateTime.TryParse(txt_InvDate.Text, null, System.Globalization.DateTimeStyles.None, out inputDate))
                {
                    args.IsValid = false;
                    val_Invoice.ErrorMessage += "- Invalid Invoice Date.<br />";
                }
                else
                    if (vwDomainShipmentDef.Shipment.IsMockShopSample == 1)
                    {
                        int diff = DateTime.Today.Subtract(inputDate).Days;
                        if (!(0 <= diff && diff <= 100))
                        {
                            args.IsValid = false;
                            val_Invoice.ErrorMessage += "- Invoice Date out of range. It should be within the range of past 3 months<br />";
                        }
                    }
                    else
                    {

                        if (inputDate.Subtract(DateTime.Today).Days > 0)
                        {
                            args.IsValid = false;
                            val_Invoice.ErrorMessage += "- Invoice Date out of range. Future date is not allowed.<br />";
                        }
                        else
                        {
                            if (Math.Abs(inputDate.Subtract(vwDomainShipmentDef.Shipment.CustomerAgreedAtWarehouseDate).Days) > 100)
                            {
                                args.IsValid = false;
                                val_Invoice.ErrorMessage += "- Invoice Date out of range. It should be within the range of ± 3 months of customer at warehouse date.<br />";
                            }
                        }
                    }
            }

            if (txt_InvPiece.Text != "" && !int.TryParse(txt_InvPiece.Text, out num))
            {
                args.IsValid = false;
                val_Invoice.ErrorMessage += "- Invalid No. of Pack.<br />";
            }

            if (vwDomainShipmentDef.Shipment.IsMockShopSample == 1 && string.IsNullOrEmpty(txt_VendorInvoiceNo.Text))
            {
                args.IsValid = false;
                val_Invoice.ErrorMessage += "- Missing Supplier Invoice No.<br />";
            }

            /*
            if (ckb_ShipDocCheck.Checked) // && ckb_ShipDocCheck.Enabled)
            {
                if (txt_VendorInvoiceNo.Text == "" || txt_ShipRecDocDate.Text == "")    // || txt_InvPiece.Text == ""
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ShippingDocStatusUpdate", "alert('Document cannot be submitted to Accounts follow up as any of below data is not yet input completely:\\n - Supplier Invoice No.;\\n - Shipping Doc. Receipt Date.');", true);
                    //ckb_ShipDocCheck.Checked = false;
                    //txt_ShipDocCheckAmount.Text = "";
                    args.IsValid = false;
                    return;
                }
                if (vwDomainShipmentDef.SplitShipments != null)
                    if (vwDomainShipmentDef.SplitShipments.Count > 0)
                    {
                        if (!isShipDocInfoValidInSplitShipment())
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SplitShippingDocStatusUpdate", "alert('Document cannot be submitted to Accounts follow up as the Supplier Invoice No. in the split shipment is not yet input completely');", true);
                            //ckb_ShipDocCheck.Checked = false;
                            //txt_ShipDocCheckAmount.Text = "";
                            args.IsValid = false;
                            return;
                        }
                    }
            }
            */

            if (!args.IsValid)
            {
                isInvoiceValidToSave = false;
                img_tbHeader_Invoice.Visible = true;
                tcContract.ActiveTabIndex = 3;
            }
            else if (isInvoiceValidToSave)
                img_tbHeader_Invoice.Visible = false;
        }

        protected void val_Amount_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            val_Amount.ErrorMessage = "";
            args.IsValid = true;
            decimal num;

            if (txt_ExLicenceFee.Text != "" && !decimal.TryParse(txt_ExLicenceFee.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out num))
            {
                args.IsValid = false;
                val_Amount.ErrorMessage += "- Invalid export licence fee. <br />";
            }

            if (txt_QuotaCharge.Text != "" && !decimal.TryParse(txt_QuotaCharge.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out num))
            {
                args.IsValid = false;
                val_Amount.ErrorMessage += "- Invalid Quota Charge.";
            }

            if (ckb_ShipDocCheck.Enabled && ckb_ShipDocCheck.Checked)
                if (decimal.Parse(txt_ShipDocCheckAmount.Text) < 0)
                {
                    args.IsValid = false;
                    val_Amount.ErrorMessage += " - Negative supplier net payable amount is NOT allowed<br />";
                }

            if (!args.IsValid)
            {
                isInvoiceValidToSave = false;
                img_tbHeader_Invoice.Visible = true;
                tcContract.ActiveTabIndex = 3;
            }
            else if (isInvoiceValidToSave)
                img_tbHeader_Invoice.Visible = false;
        }

        protected void val_PaymentInfo_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            val_PaymentInfo.ErrorMessage = "";
            args.IsValid = true;
            DateTime date;
            decimal amount;


            if (txt_ShipRecDocDate.Text != "" && !DateTime.TryParse(txt_ShipRecDocDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
            {
                args.IsValid = false;
                val_PaymentInfo.ErrorMessage += "- Invalid Date of Shipping Received Doc.<br />";
            }

            if (txt_IssueDate.Visible)
            {
                if (txt_IssueDate.Text != "" && !DateTime.TryParse(txt_IssueDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
                {
                    args.IsValid = false;
                    val_PaymentInfo.ErrorMessage += "- Invalid L/C Issue Date.<br />";
                }
            }

            if (ckb_PaymentChecked.Checked)
            {
                if (!DateTime.TryParse(txt_PaymentCheckDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
                {
                    args.IsValid = false;
                    val_PaymentInfo.ErrorMessage += "- Invalid Payment Checked Date.<br />";
                }
            }


            // Choice Order actual amount validation
            if (amendingShippedQtyTotal >= 0)
            {   // UpdateShipmentDetail has been execute before,-> amendingSalesAmountTotal & amendingPurchaseAmountTotal are defined

                bool valid = true;
                bool isChoiceAmountValid = true;
                bool showDiscrepancyAlert = false;
                decimal maxDiscrepancy = 10; // Default

                SystemParameterRef sysPara = CommonWorker.Instance.getSystemParameterByName("CHOICE_AMT_MAX_DISCREPANCY");
                if (sysPara != null)
                    maxDiscrepancy = decimal.Parse(sysPara.ParameterValue) / 100;

                if (vwDomainShipmentDef.Shipment.Vendor.VendorId == 7401) // udare
                    maxDiscrepancy = 30;

                if (txt_ChoiceActSalesAmt.Visible)
                {
                    valid = decimal.TryParse(txt_ChoiceActSalesAmt.Text, out amount);
                    if (!valid || amount <= 0)
                    {
                        if (txt_InvDate.Text != "" && (txt_ChoiceActSalesAmt.Text == "" || (valid && amount == 0 && amendingSalesAmountTotal > 0)))
                        {
                            args.IsValid = false;
                            val_PaymentInfo.ErrorMessage += "- Choice Order Actual Sales Amount Cannot Be " + ((txt_ChoiceActSalesAmt.Text == "") ? "Empty" : "Zero") + ".<br />";
                            isChoiceAmountValid = false;
                        }
                        else
                            if ((!valid && txt_ChoiceActSalesAmt.Text != "") || amount < 0)
                            {
                                args.IsValid = false;
                                val_PaymentInfo.ErrorMessage += "- Invalid Choice Order Actual Sales Amount.<br />";
                                isChoiceAmountValid = false;
                            }
                    }
                    // Check discrepancy
                    if (isChoiceAmountValid && txt_InvDate.Text != "")
                    {
                        amount = 0;
                        if (decimal.TryParse(txt_ChoiceActSalesAmt.Text, out amount))
                            if ((amendingSalesAmountTotal > 0 && amount > 0 ? Math.Abs(amendingSalesAmountTotal - amount) / amendingSalesAmountTotal : maxDiscrepancy + (amount == 0 ? 0 : 1)) > maxDiscrepancy)
                            {
                                showDiscrepancyAlert = true;
                                args.IsValid = false;
                                val_PaymentInfo.ErrorMessage += "- Large Discrepancy between Sales Amount and Actual Sales Amount.<br />";
                            }
                    }

                }

                isChoiceAmountValid = true;
                if (txt_ChoiceActPurchaseAmt.Visible)
                {
                    valid = decimal.TryParse(txt_ChoiceActPurchaseAmt.Text, out amount);
                    if (!valid || amount <= 0)
                    {
                        if (txt_InvDate.Text != "" && (txt_ChoiceActPurchaseAmt.Text == "" || (valid && amount == 0 && amendingPurchaseAmountTotal > 0)))
                        {
                            args.IsValid = false;
                            val_PaymentInfo.ErrorMessage += "- Choice Order Actual Purchase Amount Cannot Be " + (txt_ChoiceActPurchaseAmt.Text == "" ? "Empty" : "Zero") + ".<br />";
                            isChoiceAmountValid = false;
                        }
                        else
                            if ((!valid && txt_ChoiceActPurchaseAmt.Text != "") || amount < 0)
                            {
                                args.IsValid = false;
                                val_PaymentInfo.ErrorMessage += "- Invalid Choice Order Actual Purchase Amount.<br />";
                                isChoiceAmountValid = false;
                            }
                    }
                    // Check discrepancy
                    if (isChoiceAmountValid && txt_InvDate.Text != "")
                    {
                        amount = 0;
                        if (decimal.TryParse(txt_ChoiceActPurchaseAmt.Text, out amount))
                            if ((amendingPurchaseAmountTotal > 0 && amount > 0 ? Math.Abs(amendingPurchaseAmountTotal - amount) / amendingPurchaseAmountTotal : maxDiscrepancy + (amount == 0 ? 0 : 1)) > maxDiscrepancy)
                            {
                                showDiscrepancyAlert = true;
                                args.IsValid = false;
                                val_PaymentInfo.ErrorMessage += "- Large Discrepancy between Supplier Net Amount and Actual Purchase Amount.<br />";
                            }
                    }

                }
                if (showDiscrepancyAlert)
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('There is Large Discrepancy between Total Shipped Amount and The Actual Amount,\\nPlease verify your data input.')", true);
            }



            if (!args.IsValid)
            {
                isInvoiceValidToSave = false;
                img_tbHeader_Invoice.Visible = true;
                tcContract.ActiveTabIndex = 3;
            }
            else if (isInvoiceValidToSave)
                img_tbHeader_Invoice.Visible = false;
        }

        protected void val_Options_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            val_Options.ErrorMessage = "";
            args.IsValid = true;
            bool isQtyEditable = gv_Options.Columns[SizeOptionColumnId.ShippedQtyInput].Visible;
            bool allowZeroQty = (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.shipmentDetail.Id, ISAMModule.shipmentDetail.AllowInputZeroQuantity));
            if (isQtyEditable && !allowZeroQty && (!string.IsNullOrEmpty(txt_InvDate.Text) || vwDomainShipmentDef.Shipment.WorkflowStatus.Id == ContractWFS.INVOICED.Id))
            {
                //Actual Quantity & amount validation
                if (amendingShippedQtyTotal == 0)
                {
                    args.IsValid = false;
                    val_Options.ErrorMessage += "- Total Shipped Quantity Cannot Be Zero.<br />";
                }

                if (amendingSalesAmountTotal == 0)
                {
                    args.IsValid = false;
                    val_Options.ErrorMessage += "- Total Sales Amount Cannot Be Zero.<br />";
                }
                if (amendingPurchaseAmountTotal == 0)
                {
                    args.IsValid = false;
                    val_Options.ErrorMessage += "- Total Purchase Amount Cannot Be Zero.<br />";
                }
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AlertMsg", "alert('Total Shipped Quantity and Amount Cannot be Zero,\\nPlease verify your data input.')", true);

                if (!args.IsValid)
                {
                    img_tbHeader_Options.Visible = true;
                    tcContract.ActiveTabIndex = 2;
                }
                else
                    img_tbHeader_Options.Visible = false;
            }
        }

        protected void val_CourierCharge_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            val_CourierCharge.ErrorMessage = "";
            args.IsValid = true;
            decimal num;

            if (txt_CourierCharge.Text != "" && !decimal.TryParse(txt_CourierCharge.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out num))
            {
                args.IsValid = false;
                val_DF.ErrorMessage += "- Invalid Courier Charge.<br/>";
            }
        }

        protected void val_DF_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            val_DF.ErrorMessage = "";
            args.IsValid = true;
            decimal num;

            if (txt_DocCharge.Text != "" && !decimal.TryParse(txt_DocCharge.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out num))
            {
                args.IsValid = false;
                val_DF.ErrorMessage += "- Invalid Documentation Charge.<br/>";
            }

            if (txt_TransportCharge.Text != "" && !decimal.TryParse(txt_TransportCharge.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out num))
            {
                args.IsValid = false;
                val_DF.ErrorMessage += "- Invalid Transportation Charge.<br />";
            }
            if (!args.IsValid)
            {
                isInvoiceValidToSave = false;
                img_tbHeader_Invoice.Visible = true;
                tcContract.ActiveTabIndex = 3;
            }
            else if (isInvoiceValidToSave)
                img_tbHeader_Invoice.Visible = false;
        }

        protected void val_BookingInfo_Validate(object source, ServerValidateEventArgs args)
        {
            val_BookingInfo.ErrorMessage = "";
            args.IsValid = true;
            int num;
            DateTime date;

            if (txt_BookingQty.Text != "" && !int.TryParse(txt_BookingQty.Text, System.Globalization.NumberStyles.AllowThousands, null, out num))
            {
                args.IsValid = false;
                val_BookingInfo.ErrorMessage += "- Invalid Booking Quantity.<br />";
            }

            if (txt_BookingDate.Text != "" && !DateTime.TryParse(txt_BookingDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
            {
                args.IsValid = false;
                val_BookingInfo.ErrorMessage += "- Invalid Booking Date. <br />";
            }

            if (txt_BkInWHDate.Text != "" && !DateTime.TryParse(txt_BkInWHDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
            {
                args.IsValid = false;
                val_BookingInfo.ErrorMessage += "- Invalid Booked In-Warehouse Date. <br />";
            }

            if (txt_ActualInWHDate.Text != "" && !DateTime.TryParse(txt_ActualInWHDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
            {
                args.IsValid = false;
                val_BookingInfo.ErrorMessage += "- Invalid Actual In-Warehouse Date. <br />";
            }

            if (!args.IsValid)
            {
                img_tbHeader_Booking.Visible = true;
                tcContract.ActiveTabIndex = 1;
            }
            else
                img_tbHeader_Booking.Visible = false;
        }

        protected void val_UTurn_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            val_UTurn.ErrorMessage = "";
            args.IsValid = true;
            decimal num;
            DateTime date;

            if (txt_ImportDutyActualAmt.Text != "" && !decimal.TryParse(txt_ImportDutyActualAmt.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out num))
            {
                args.IsValid = false;
                val_UTurn.ErrorMessage += "- Invalid Import Duty Amount.<br/>";
            }

            if (ckb_ImportDutyRecordChecked.Checked)
            {
                if (!DateTime.TryParse(txt_ImportDutyRecordCheckedDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
                {
                    args.IsValid = false;
                    val_UTurn.ErrorMessage += "- Invalid Import Duty Record Checked Date. <br />";
                }
            }

            if (txt_InputVATActualAmt.Text != "" && !decimal.TryParse(txt_InputVATActualAmt.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out num))
            {
                args.IsValid = false;
                val_UTurn.ErrorMessage += "- Invalid Input VAT Amount.<br />";
            }

            if (ckb_InputVATRecordChecked.Checked)
            {
                if (!DateTime.TryParse(txt_InputVATRecordCheckedDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
                {
                    args.IsValid = false;
                    val_UTurn.ErrorMessage += "- Invalid Input VAT Record Checked Date.<br />";
                }
            }

            if (txt_OutputVATActualAmt.Text != "" && !decimal.TryParse(txt_OutputVATActualAmt.Text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, null, out num))
            {
                args.IsValid = false;
                val_UTurn.ErrorMessage += "- Invalid Output VAT Amount. <br />";
            }

            if (ckb_OutputVATRecordChecked.Checked)
            {
                if (!DateTime.TryParse(txt_OutputVATRecordCheckedDate.Text, null, System.Globalization.DateTimeStyles.None, out date))
                {
                    args.IsValid = false;
                    val_UTurn.ErrorMessage += "- Invalid Output VAT Record Checked Date. ";
                }
            }
            if (!args.IsValid)
            {
                isInvoiceValidToSave = false;
                img_tbHeader_Invoice.Visible = true;
                tcContract.ActiveTabIndex = 3;
            }
            else if (isInvoiceValidToSave)
                img_tbHeader_Invoice.Visible = false;
        }


        protected void val_PaymentDeduction_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            val_PaymentDeduction.ErrorMessage = string.Empty;
            args.IsValid = true;
            if (vwDomainShipmentDef.UpdatedPaymentDeduction != null)
            {
                if ((val_PaymentDeduction.ErrorMessage = fillPaymentDeductionUpdates()) != string.Empty)
                {
                    args.IsValid = false;
                    foreach (GridViewRow r in gv_PaymentDeductionUpdate.Rows)
                        PaymentDeductionUpdateRowControl(r);
                }

                if (!args.IsValid)
                {
                    img_tbHeader_Deduction.Visible = true;
                    tcContract.ActiveTabIndex = 10;
                }
                else if (isInvoiceValidToSave)
                    img_tbHeader_Deduction.Visible = false;
            }
        }

        protected void clearValidatorMessage()
        {
            img_tbHeader_Invoice.Visible = false;
            img_tbHeader_Booking.Visible = false;
            img_tbHeader_Options.Visible = false;
            img_tbHeader_Deduction.Visible = false;

            val_Invoice.ErrorMessage = string.Empty;
            val_Amount.ErrorMessage = string.Empty;
            val_PaymentInfo.ErrorMessage = string.Empty;
            val_Options.ErrorMessage = string.Empty;
            val_CourierCharge.ErrorMessage = string.Empty;
            val_DF.ErrorMessage = string.Empty;
            val_BookingInfo.ErrorMessage = string.Empty;
            val_UTurn.ErrorMessage = string.Empty;
            val_PaymentDeduction.ErrorMessage = string.Empty;
        }

        #endregion


    }
}
