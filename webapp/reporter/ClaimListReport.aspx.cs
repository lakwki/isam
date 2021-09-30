using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.util;
using com.next.isam.webapp.commander.account;
using com.next.common.domain.module;
using com.next.isam.domain.types;
using com.next.isam.domain.claim;
using com.next.infra.web;
using com.next.isam.appserver.claim;
using com.next.common.web.commander;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.isam.domain.order;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.order;


namespace com.next.isam.webapp.reporter
{
    public partial class ClaimListReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public struct MIME_Type
        {
            public const string Excel = "application/vnd.ms-excel";
            public const string Word = "application/vnd.ms-word";
            public const string PowerPoint = "application/vnd.ms-powerpoint";
        }

        private QAIS.ClaimRequestService svc = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.EnableViewState = false;
                Response.Clear();
                Response.Charset = "";

                Response.Buffer = true;
                Response.ContentType = MIME_Type.Excel;
                Response.AddHeader("Content-Disposition", "attachment;filename=ClaimList.xls");
                this.lblPrintTime.Text = "Print Date: " + DateTime.Today.ToShortDateString() + "  " + DateTime.Now.ToShortTimeString();
                this.lblPrintTime.Text += "<br>Print By: " + CommonUtil.getUserByKey(this.LogonUserId).DisplayName;
                svc = new QAIS.ClaimRequestService();
                this.getReport();
                
            }
        }

        private void getReport()
        {
            List<UKClaimDef> list = (List<UKClaimDef>)Context.Items[AccountCommander.Param.ukClaimList];
            List<UKClaimRefundDef> refundList = (List<UKClaimRefundDef>)Context.Items[AccountCommander.Param.claimRefundList];
            DateTime exRateDate = (DateTime)Context.Items[AccountCommander.Param.debitNoteReceivedDateTo];
            this.ExchangeRateDate = exRateDate;

            UKClaimDCNoteDetailDef dcNoteDetailDef = null;
            UKClaimDCNoteDef dcNoteDef = null;

            foreach(UKClaimDef def in list)
            {
                dcNoteDetailDef = UKClaimManager.Instance.getUKClaimDCNoteDetailByLogicalKey(def.ClaimId, 0);
                if (dcNoteDetailDef != null)
                {
                    dcNoteDef = UKClaimManager.Instance.getUKClaimDCNoteByKey(dcNoteDetailDef.DCNoteId);
                    if (dcNoteDef.SettledAmount != 0)
                    {
                        def.DCNoteNoIssuedToSupplier = dcNoteDef.DCNoteNo;
                        def.SupplierDNDate = dcNoteDef.DCNoteDate;
                        def.SupplierDNSettlementDate = dcNoteDef.SettlementDate;
                    }
                }
                ArrayList shipmentList = OrderManager.Instance.getShipmentListByContractNo(def.ContractNo);
                DateTime dt = DateTime.MinValue;
                foreach(ShipmentDef shipmentDef in shipmentList)
                {
                    InvoiceDef invoiceDef = ShipmentManager.Instance.getInvoiceByShipmentId(shipmentDef.ShipmentId);
                    if (invoiceDef.InvoiceDate > dt)
                        dt = invoiceDef.InvoiceDate;
                }
                def.FirstContractDate = dt;
            }

            foreach (UKClaimRefundDef def in refundList)
            {
                UKClaimDef claimDef = UKClaimManager.Instance.getUKClaimByKey(def.ClaimId);
                if (claimDef.Type != UKClaimType.BILL_IN_ADVANCE)
                {
                    claimDef.UKDebitNoteReceivedDate = def.ReceivedDate;
                    claimDef.Remark = def.Remark;
                    claimDef.Amount = def.Amount * -1;
                    claimDef.SettlementOption = def.SettlementOption;

                    claimDef.DCNoteNoIssuedToSupplier = string.Empty;
                    dcNoteDetailDef = UKClaimManager.Instance.getUKClaimDCNoteDetailByLogicalKey(def.ClaimId, def.ClaimRefundId);
                    if (dcNoteDetailDef != null)
                    {
                        dcNoteDef = UKClaimManager.Instance.getUKClaimDCNoteByKey(dcNoteDetailDef.DCNoteId);
                        if (dcNoteDef.SettledAmount != 0)
                        {
                            claimDef.DCNoteNoIssuedToSupplier = dcNoteDef.DCNoteNo;
                            claimDef.SupplierDNSettlementDate = dcNoteDef.SettlementDate;
                        }
                    }

                    list.Add(claimDef);
                }
            }

            this.vwSearchResult = list;

            this.repUKClaim.DataSource = list;
            this.repUKClaim.DataBind();
        }

        private List<UKClaimDef> vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (List<UKClaimDef>)ViewState["SearchResult"];
            }
        }

        private DateTime ExchangeRateDate
        {
            set
            {
                ViewState["ExRateDate"] = value;
            }
            get
            {
                return (DateTime)ViewState["ExRateDate"];
            }
        }

        protected void repUKClaim_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            UKClaimDef def = (UKClaimDef)vwSearchResult[e.Item.ItemIndex];

            ((Label)e.Item.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);
            ((Label)e.Item.FindControl("lbl_HandlingOffice")).Text = OfficeId.getName(def.HandlingOfficeId);
            ((Label)e.Item.FindControl("lbl_PaymentOffice")).Text = OfficeId.getName(def.PaymentOfficeId);
            ((Label)e.Item.FindControl("lbl_ClaimType")).Text = def.Type.Name;

            string t5 = string.Empty;

            if (def.ProductTeamId == 0)
                t5 = "AUD";
            else
                t5 = def.T5Code;
            /*
            if (def.Type == UKClaimType.FABRIC_TEST || def.Type == UKClaimType.PENALTY_CHARGE)
                t5 = "DN";
            */

            ((Label)e.Item.FindControl("lbl_T5")).Text = t5;

            ((Label)e.Item.FindControl("lbl_UKDNNo")).Text = def.UKDebitNoteNo;
            ((Label)e.Item.FindControl("lbl_UKDNDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteDate);
            ((Label)e.Item.FindControl("lbl_UKDNRecdDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteReceivedDate);

            ((Label)e.Item.FindControl("lbl_OrderType")).Text = (def.TermOfPurchaseId == 1 ? "FOB" : "VM");
            ((Label)e.Item.FindControl("lbl_Vendor")).Text = def.Vendor.Name;
            ProductCodeRef codeRef = CommonUtil.getProductCodeByKey(def.ProductTeamId);
            if (codeRef == null)
                ((Label)e.Item.FindControl("lbl_ProductTeam")).Text = "N/A";
            else
                ((Label)e.Item.FindControl("lbl_ProductTeam")).Text = codeRef.CodeDescription;

            ((Label)e.Item.FindControl("lbl_ItemNo")).Text = def.ItemNo;
            ((Label)e.Item.FindControl("lbl_ContractNo")).Text = def.ContractNo;
            ((Label)e.Item.FindControl("lbl_Status")).Text = def.WorkflowStatus.Name;
            ((Label)e.Item.FindControl("lbl_Currency")).Text = def.Currency.CurrencyCode;
            ((Label)e.Item.FindControl("lbl_Amount")).Text = def.Amount.ToString("#,##0.00");

            /*
            if (this.ExchangeRateDate != DateTime.MinValue)
            {
                decimal baseAmt = def.Amount * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, this.ExchangeRateDate) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, this.ExchangeRateDate);
                ((Label)e.Item.FindControl("lbl_AmountInUSD")).Text = baseAmt.ToString("#,##0.00");
            }
            */

            if (def.UKDebitNoteReceivedDate != DateTime.MinValue)
            {
                decimal baseAmt = def.Amount * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, def.UKDebitNoteReceivedDate) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, def.UKDebitNoteReceivedDate);
                ((Label)e.Item.FindControl("lbl_AmountInUSD")).Text = baseAmt.ToString("#,##0.00");
            }

            ((Label)e.Item.FindControl("lbl_Qty")).Text = def.Quantity.ToString("#,##0");
            if (def.IsReadyForSettlement)
                ((Label)e.Item.FindControl("lbl_IsReadyForSettlement")).Text = "YES";
            else
                ((Label)e.Item.FindControl("lbl_IsReadyForSettlement")).Text = "NO";


            ((Label)e.Item.FindControl("lbl_SupplierDNNo")).Text = def.DCNoteNoIssuedToSupplier;
            if (def.SupplierDNSettlementDate != DateTime.MinValue)
                ((Label)e.Item.FindControl("lbl_SupplierDNSettlementDate")).Text = DateTimeUtility.getDateString(def.SupplierDNSettlementDate);
            if (def.SupplierDNDate != DateTime.MinValue)
                ((Label)e.Item.FindControl("lbl_SupplierDNDate")).Text = DateTimeUtility.getDateString(def.SupplierDNDate);

            if (def.FirstContractDate != DateTime.MinValue)
                ((Label)e.Item.FindControl("lbl_FirstContractDate")).Text = DateTimeUtility.getDateString(def.FirstContractDate);

            ((Label)e.Item.FindControl("lbl_NSRechargePercent")).Text = (def.SettlementOption == UKClaimSettlemtType.PROVISION ?  "100.00" : "0.00");
            ((Label)e.Item.FindControl("lbl_VendorRechargePercent")).Text = (def.SettlementOption == UKClaimSettlemtType.PROVISION ? "0.00" : "100.00");

            if (def.ClaimRequestId > 0)
            {
                QAIS.ClaimRequestDef crDef = svc.GetClaimRequestByKey(def.ClaimRequestId);
                ((Label)e.Item.FindControl("lbl_FormNo")).Text = crDef.FormNo;
                if (def.SettlementOption == UKClaimSettlemtType.DEFAULT)
                {
                    ((Label)e.Item.FindControl("lbl_NSRechargePercent")).Text = crDef.NSRechargePercent.ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_VendorRechargePercent")).Text = crDef.VendorRechargePercent.ToString("#,##0.00");
                }
            }


        }

    }
}