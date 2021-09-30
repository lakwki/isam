using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using com.next.infra.util;
using com.next.isam.webapp.commander.account;
using com.next.isam.webapp.commander;
using com.next.common.appserver;
using com.next.common.domain.module;
using com.next.isam.domain.types;
using com.next.isam.domain.claim;
using com.next.infra.web;
using com.next.isam.appserver.claim;
using com.next.isam.appserver.common;
using com.next.isam.dataserver.worker;
using com.next.common.web.commander;
using com.next.common.domain.types;
using com.next.isam.appserver.shipping;
using com.next.common.domain;

namespace com.next.isam.webapp.reporter
{
    public partial class OutstandingDiscountListReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public struct MIME_Type
        {
            public const string Excel = "application/vnd.ms-excel";
            public const string Excel2007 = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            public const string Word = "application/vnd.ms-word";
            public const string PowerPoint = "application/vnd.ms-powerpoint";
        }

        private class OSRptSummaryInfoDef
        {
            public OSRptSummaryInfoDef(int id, string name, decimal totalAmt, decimal aging1TotalAmt, decimal aging2TotalAmt, decimal aging3TotalAmt, decimal aging4TotalAmt, decimal aging5TotalAmt, decimal totalFutureOrderAmt, decimal totalAPOSAmt, string handledBy)
            {
                this.SummaryId = id;
                this.Name = name;
                this.TotalAmt = totalAmt;
                this.Aging1TotalAmt = aging1TotalAmt;
                this.Aging2TotalAmt = aging2TotalAmt;
                this.Aging3TotalAmt = aging3TotalAmt;
                this.Aging4TotalAmt = aging4TotalAmt;
                this.Aging5TotalAmt = aging5TotalAmt;
                this.TotalFutureOrderAmt = totalFutureOrderAmt;
                this.TotalAPOSAmt = totalAPOSAmt;
                this.HandledBy = handledBy;
            }
            public OSRptSummaryInfoDef(int id, string name, decimal totalAmt, decimal aging1TotalAmt, decimal aging2TotalAmt, decimal aging3TotalAmt, decimal aging4TotalAmt, decimal aging5TotalAmt,
                decimal totalAmtOrginal, decimal aging1TotalAmtOrginal, decimal aging2TotalAmtOrginal, decimal aging3TotalAmtOrginal, decimal aging4TotalAmtOrginal, decimal aging5TotalAmtOrginal)
            {
                this.SummaryId = id;
                this.Name = name;
                this.TotalAmt = totalAmt;
                this.Aging1TotalAmt = aging1TotalAmt;
                this.Aging2TotalAmt = aging2TotalAmt;
                this.Aging3TotalAmt = aging3TotalAmt;
                this.Aging4TotalAmt = aging4TotalAmt;
                this.Aging5TotalAmt = aging5TotalAmt;
                this.TotalAmtOriginal = totalAmtOrginal;
                this.Aging1TotalAmtOriginal = aging1TotalAmtOrginal;
                this.Aging2TotalAmtOriginal = aging2TotalAmtOrginal;
                this.Aging3TotalAmtOriginal = aging3TotalAmtOrginal;
                this.Aging4TotalAmtOriginal = aging4TotalAmtOrginal;
                this.Aging5TotalAmtOriginal = aging5TotalAmtOrginal;
            }

            public int SummaryId { get; set; }
            public string Name { get; set; }
            public decimal TotalAmt { get; set; }
            public decimal Aging1TotalAmt { get; set; }
            public decimal Aging2TotalAmt { get; set; }
            public decimal Aging3TotalAmt { get; set; }
            public decimal Aging4TotalAmt { get; set; }
            public decimal Aging5TotalAmt { get; set; }
            public decimal TotalAmtOriginal { get; set; }
            public decimal Aging1TotalAmtOriginal { get; set; }
            public decimal Aging2TotalAmtOriginal { get; set; }
            public decimal Aging3TotalAmtOriginal { get; set; }
            public decimal Aging4TotalAmtOriginal { get; set; }
            public decimal Aging5TotalAmtOriginal { get; set; }
            public decimal TotalFutureOrderAmt { get; set; }
            public decimal TotalAPOSAmt { get; set; }
            public string HandledBy { get; set; }
        }

        bool isFirst = true;
        List<OSRptSummaryInfoDef> summaryList = new List<OSRptSummaryInfoDef>();
        private DateTime cutOffDate;
        private decimal totalUSD = 0;
        private decimal totalAging1 = 0;
        private decimal totalAging2 = 0;
        private decimal totalAging3 = 0;
        private decimal totalAging4 = 0;
        private decimal totalAging5 = 0;
        private int cnt = 0;

        private decimal subtotalUSD = 0;
        private decimal subtotalAging1 = 0;
        private decimal subtotalAging2 = 0;
        private decimal subtotalAging3 = 0;
        private decimal subtotalAging4 = 0;
        private decimal subtotalAging5 = 0;
        private int officeId;
        private ArrayList hardGoodsTeamList;

        //private QAIS.ClaimRequestService svc = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.EnableViewState = false;
                Response.Clear();
                Response.Charset = "";

                Response.Buffer = true;
                Response.ContentType = MIME_Type.Excel;
                Response.AddHeader("Content-Disposition", "attachment;filename=Next Discount Report.xls");

                if (string.IsNullOrEmpty(Request.Params["reportCode"]))
                    tr_ReportCode.Style.Add("display", "none");
                else
                    lbl_ReportCode.Text = "Report Code : " + Request.Params["reportCode"];
                if (string.IsNullOrEmpty(lbl_ReportCode.Text))
                    tr_ReportCode.Attributes.Add("Display", "none");
                if (Request.QueryString.Get("cutOffDate") != null)
                    cutOffDate = ConvertUtility.toDateTime(HttpUtility.UrlDecode(Request.QueryString.Get("cutOffDate")));
                else
                    cutOffDate = (DateTime)Context.Items[AccountCommander.Param.cutoffDate];

                this.lblPrintTime.Text = "Print Date: " + DateTime.Today.ToShortDateString() + "  " + DateTime.Now.ToShortTimeString();

                int userId = 0;
                if (Request.QueryString.Get("userId") != null)
                    userId = int.Parse(Request.QueryString.Get("userId"));
                else
                    userId = userId = this.LogonUserId;
                this.lblPrintUser.Text = "Print By: " + CommonUtil.getUserByKey(userId).DisplayName;

                //string orderType;
                //if (Request.QueryString.Get("termOfPurchaseId") != null)
                //    orderType = Request.QueryString.Get("termOfPurchaseId");
                //else
                //    orderType = Context.Items[AccountCommander.Param.orderType].ToString();
                //if (orderType == "-1")
                //    this.lbl_OrderType.Text = "Order Type : ALL";
                //else if (orderType == "1")
                //    this.lbl_OrderType.Text = "Order Type : FOB";
                //else if (orderType == "2")
                //    this.lbl_OrderType.Text = "Order Type : VM";

                if (Request.QueryString.Get("officeId") != null)
                    this.lbl_Office.Text = "Office : " + (Request.QueryString.Get("officeId") == "-1" ? "ALL" : CommonManager.Instance.getReportOfficeGroupByKey(int.Parse(Request.QueryString.Get("officeId"))).GroupName);
                else
                    this.lbl_Office.Text = "Office : " + (Context.Items[AccountCommander.Param.officeId].ToString() == "-1" ? "ALL" : CommonManager.Instance.getReportOfficeGroupByKey(int.Parse(Context.Items[AccountCommander.Param.officeId].ToString())).GroupName);

                if (lbl_Office.Text.Contains("FB"))
                {
                    //tr_HandlingOffice.Style.Add(HtmlTextWriterStyle.Display, "block");
                    //string handlingOfficeName = Request.QueryString.Get("handlingOfficeName");
                    //if (handlingOfficeName != null)
                    //{
                    //    tr_HandlingOffice.Style.Add(HtmlTextWriterStyle.Display, (handlingOfficeName == string.Empty ? "none" : "block"));
                    //    this.lbl_HandlingOffice.Text = "Handling Office : " + handlingOfficeName;
                    //}
                    //else
                    //    if (Context.Items[AccountCommander.Param.handlingOfficeName] != null)
                    //{
                    //    handlingOfficeName = Context.Items[AccountCommander.Param.handlingOfficeName].ToString();
                    //    tr_HandlingOffice.Style.Add(HtmlTextWriterStyle.Display, (handlingOfficeName == string.Empty ? "none" : "block"));
                    //    this.lbl_HandlingOffice.Text = "Handling Office : " + handlingOfficeName;
                    //}
                }

                this.lblCutOffDate.Text = "Cut Off : " + DateTimeUtility.getDateString(cutOffDate);

                int rptOptionId = 0;
                if (Request.QueryString.Get("rptOption") != null)
                    rptOptionId = int.Parse(Request.QueryString.Get("rptOption"));
                else
                    rptOptionId = int.Parse(Context.Items[AccountCommander.Param.rptOption].ToString());

                this.hardGoodsTeamList = CommonUtil.getProductCodeListByCriteria(1286, -1, GeneralCriteria.ALL, -1);
                ArrayList osList = CommonUtil.getProductCodeListByCriteria(31688, -1, GeneralCriteria.ALL, -1);
                this.hardGoodsTeamList.AddRange(osList);

                //svc = new QAIS.ClaimRequestService();

                this.getReport();
                if (rptOptionId == 2)
                {
                    this.divDetailSection.Visible = false;
                    this.divSupplierSection.Visible = false;
                }
                if (rptOptionId == 3)
                {
                    this.divDetailSection.Visible = false;
                    this.divOfficeSection.Visible = false;
                }
            }
        }

        private void getReport()
        {
            List<UKDiscountClaimDef> list = (List<UKDiscountClaimDef>)Context.Items[AccountCommander.Param.ukClaimList];
            if (list == null)
            {
                int officeId = int.Parse(Request.QueryString.Get("officeId"));
                int vendorId = int.Parse(Request.QueryString.Get("vendorId"));
                int termOfPurchaseId = int.Parse(Request.QueryString.Get("termOfPurchaseId"));
                int handlingOfficeId = int.Parse(Request.QueryString.Get("handlingOfficeId"));
                string handlingOfficeName = Request.QueryString.Get("handlingOfficeName");
                int ncOptionId = int.Parse(Request.QueryString.Get("ncOption"));

                list = UKClaimManager.Instance.getOutstandingUKDiscountClaimReport(officeId, vendorId, termOfPurchaseId, cutOffDate, handlingOfficeId);
            }

            this.vwSearchResult = list;

            this.repUKClaim.DataSource = list;
            this.repUKClaim.DataBind();
            this.lbl_TotalAmountInUSD.Text = totalUSD.ToString("#,##0.00");
            this.lbl_TotalAging1.Text = totalAging1.ToString("#,##0.00");
            this.lbl_TotalAging2.Text = totalAging2.ToString("#,##0.00");
            this.lbl_TotalAging3.Text = totalAging3.ToString("#,##0.00");
            this.lbl_TotalAging4.Text = totalAging4.ToString("#,##0.00");
            this.lbl_TotalAging5.Text = totalAging5.ToString("#,##0.00");

            this.repOfficeSummary.DataSource = summaryList;
            this.repOfficeSummary.DataBind();

            this.lbl_OfficeGrandTotal_AmountInUSD.Text = totalUSD.ToString("#,##0.00");
            this.lbl_OfficeGrandTotal_Aging1.Text = totalAging1.ToString("#,##0.00");
            this.lbl_OfficeGrandTotal_Aging2.Text = totalAging2.ToString("#,##0.00");
            this.lbl_OfficeGrandTotal_Aging3.Text = totalAging3.ToString("#,##0.00");
            this.lbl_OfficeGrandTotal_Aging4.Text = totalAging4.ToString("#,##0.00");
            this.lbl_OfficeGrandTotal_Aging5.Text = totalAging5.ToString("#,##0.00");

            if (this.vwSearchResult.Count > 0)
            {
                this.buildVendorSummary();
            }
            /*
            this.row_RevaluationEUR.Visible = false;
            this.row_RevaluationGBP.Visible = false;
            this.row_RevaluationHeader.Visible = false;
            this.row_RevaluationGrand.Visible = false;
            this.row_RevaluationTotal.Visible = false;
            */

            decimal gbpRate = 0;
            decimal eurRate = 0;
            decimal gbpAccrualAmt = 0;
            decimal eurAccrualAmt = 0;
            decimal usdAccrualAmt = 0;

            if (Context.Items[AccountCommander.Param.gbpRate] != null)
                gbpRate = decimal.Parse(Context.Items[AccountCommander.Param.gbpRate].ToString());
            if (Context.Items[AccountCommander.Param.eurRate] != null)
                eurRate = decimal.Parse(Context.Items[AccountCommander.Param.eurRate].ToString());
            if (Context.Items[AccountCommander.Param.gbpAccrualAmt] != null)
                gbpAccrualAmt = decimal.Parse(Context.Items[AccountCommander.Param.gbpAccrualAmt].ToString());
            if (Context.Items[AccountCommander.Param.eurAccrualAmt] != null)
                eurAccrualAmt = decimal.Parse(Context.Items[AccountCommander.Param.eurAccrualAmt].ToString());
            if (Context.Items[AccountCommander.Param.usdAccrualAmt] != null)
                usdAccrualAmt = decimal.Parse(Context.Items[AccountCommander.Param.usdAccrualAmt].ToString());

            /*
            if (gbpRate != 0 || eurRate != 0)
            {
                this.row_RevaluationEUR.Visible = true;
                this.row_RevaluationGBP.Visible = true;
                this.row_RevaluationHeader.Visible = true;
                this.row_RevaluationGrand.Visible = true;
                this.row_RevaluationTotal.Visible = true;
                this.buildRevaluationSummary(gbpRate, eurRate);
            }
            */
            this.row_RevaluationEUR.Visible = true;
            this.row_RevaluationGBP.Visible = true;
            this.row_RevaluationHeader.Visible = true;
            this.row_Accrual.Visible = true;
            this.row_AccrualTotal.Visible = true;
            this.row_RevaluationGrand.Visible = true;
            this.row_RevaluationTotal.Visible = true;
            this.buildRevaluationSummary(gbpRate, eurRate, gbpAccrualAmt, eurAccrualAmt, usdAccrualAmt);

        }

        private void buildVendorSummary()
        {
            isFirst = true;
            this.summaryList.Clear();
            UKDiscountClaimDef.UKDiscountClaimComparer.CompareType compareType;
            compareType = UKDiscountClaimDef.UKDiscountClaimComparer.CompareType.SupplierName;
            this.vwSearchResult.Sort(new UKDiscountClaimDef.UKDiscountClaimComparer(compareType, SortDirection.Ascending));
            int vendorId = 0;
            int productTeamId = 0;
            string vendorName = string.Empty;
            decimal amt = 0;
            decimal aging1Amt = 0;
            decimal aging2Amt = 0;
            decimal aging3Amt = 0;
            decimal aging4Amt = 0;
            decimal aging5Amt = 0;
            decimal futureOrderAmt = 0;
            decimal aposAmt = 0;
            decimal totalFutureOrderAmt = 0;
            decimal totalAposAmt = 0;

            bool isHardGoods = false;

            foreach (UKDiscountClaimDef def in this.vwSearchResult)
            {
                isHardGoods = false;
                /*
                foreach (ProductCodeRef osDef in this.hardGoodsTeamList)
                {
                    if (osDef.ProductCodeId == def.ProductTeamId && osDef.Code != "NCHB")
                    {
                        isHardGoods = true;
                        break;
                    }
                }
                */

                if (isFirst)
                {
                    vendorId = def.VendorId;
                    productTeamId = def.ProductTeamId;
                }

                if (vendorId != def.VendorId)
                {
                    futureOrderAmt = ShipmentManager.Instance.getFutureOrderAmtByVendorId(vendorId);
                    aposAmt = ShipmentManager.Instance.getOutstandingPaymentAmtByVendorId(vendorId);
                    totalFutureOrderAmt += futureOrderAmt;
                    totalAposAmt += aposAmt;

                    if (UKClaimManager.Instance.isHomeAndBeautyProductTeam(productTeamId))
                        isHardGoods = true;

                    summaryList.Add(new OSRptSummaryInfoDef(vendorId, vendorName, amt, aging1Amt, aging2Amt, aging3Amt, aging4Amt, aging5Amt, futureOrderAmt, aposAmt, isHardGoods ? "Hard Goods" : "Garment"));

                    amt = 0;
                    cnt = 0;
                    aging1Amt = 0;
                    aging2Amt = 0;
                    aging3Amt = 0;
                    aging4Amt = 0;
                    aging5Amt = 0;
                    isHardGoods = false;
                }

                //if (def.Type == UKClaimType.BILL_IN_ADVANCE)
                def.UKDebitNoteReceivedDate = def.UKDebitNoteReceivedDate;

                vendorId = def.Vendor.VendorId;
                productTeamId = def.ProductTeamId;
                vendorName = def.Vendor.Name;
                cnt += 1;
                amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);

                TimeSpan t = new TimeSpan(0, 0, 0);
                if (def.UKDebitNoteDate != DateTime.MinValue)
                    t = cutOffDate.Subtract(def.UKDebitNoteDate);
                //else if (def.Type == UKClaimType.BILL_IN_ADVANCE)
                //    t = cutOffDate.Subtract(def.CreateDate);

                if (t.Days >= 0 && t.Days <= 30)
                    aging1Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                else if (t.Days >= 31 && t.Days <= 60)
                    aging2Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                else if (t.Days >= 61 && t.Days <= 90)
                    aging3Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                else if (t.Days >= 91 && t.Days <= 120)
                    aging4Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                else if (t.Days > 120)
                    aging5Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);


                isFirst = false;
            }

            futureOrderAmt = ShipmentManager.Instance.getFutureOrderAmtByVendorId(vendorId);
            aposAmt = ShipmentManager.Instance.getOutstandingPaymentAmtByVendorId(vendorId);
            totalFutureOrderAmt += futureOrderAmt;
            totalAposAmt += aposAmt;

            if (UKClaimManager.Instance.isHomeAndBeautyProductTeam(productTeamId))
                isHardGoods = true;

            summaryList.Add(new OSRptSummaryInfoDef(vendorId, vendorName, amt, aging1Amt, aging2Amt, aging3Amt, aging4Amt, aging5Amt, futureOrderAmt, aposAmt, isHardGoods ? "Hard Goods" : "Garment"));

            this.repVendorSummary.DataSource = summaryList;
            this.repVendorSummary.DataBind();

            this.lbl_VendorGrandTotal_AmountInUSD.Text = totalUSD.ToString("#,##0.00");
            this.lbl_VendorGrandTotal_Aging1.Text = totalAging1.ToString("#,##0.00");
            this.lbl_VendorGrandTotal_Aging2.Text = totalAging2.ToString("#,##0.00");
            this.lbl_VendorGrandTotal_Aging3.Text = totalAging3.ToString("#,##0.00");
            this.lbl_VendorGrandTotal_Aging4.Text = totalAging4.ToString("#,##0.00");
            this.lbl_VendorGrandTotal_Aging5.Text = totalAging5.ToString("#,##0.00");
            this.lbl_VendorGrandTotal_FutureOrderAmt.Text = totalFutureOrderAmt.ToString("#,##0.00");
            this.lbl_VendorGrandTotal_APOSAmt.Text = totalAposAmt.ToString("#,##0.00");
        }

        private void buildRevaluationSummary(decimal gbpRate, decimal eurRate, decimal gbpAccrualAmt, decimal eurAccrualAmt, decimal usdAccrualAmt)
        {
            isFirst = true;
            this.summaryList.Clear();
            UKDiscountClaimDef.UKDiscountClaimComparer.CompareType compareType;
            compareType = UKDiscountClaimDef.UKDiscountClaimComparer.CompareType.CurrencyId;
            this.vwSearchResult.Sort(new UKDiscountClaimDef.UKDiscountClaimComparer(compareType, SortDirection.Ascending));
            int currencyId = 0;
            decimal amt = 0;
            decimal aging1Amt = 0;
            decimal aging2Amt = 0;
            decimal aging3Amt = 0;
            decimal aging4Amt = 0;
            decimal aging5Amt = 0;
            decimal amtOriginal = 0;
            decimal aging1AmtOriginal = 0;
            decimal aging2AmtOriginal = 0;
            decimal aging3AmtOriginal = 0;
            decimal aging4AmtOriginal = 0;
            decimal aging5AmtOriginal = 0;

            decimal revaluationGrandTotal = totalUSD;
            decimal revaluationGrandAging1Total = totalAging1;
            decimal revaluationGrandAging2Total = totalAging2;
            decimal revaluationGrandAging3Total = totalAging3;
            decimal revaluationGrandAging4Total = totalAging4;
            decimal revaluationGrandAging5Total = totalAging5;

            foreach (UKDiscountClaimDef def in this.vwSearchResult)
            {
                if (isFirst) currencyId = def.Currency.CurrencyId;

                if (currencyId != def.Currency.CurrencyId)
                {
                    summaryList.Add(new OSRptSummaryInfoDef(currencyId, currencyId.ToString(), amt, aging1Amt, aging2Amt, aging3Amt, aging4Amt, aging5Amt, amtOriginal, aging1AmtOriginal, aging2AmtOriginal, aging3AmtOriginal, aging4AmtOriginal, aging5AmtOriginal));
                    amt = 0;
                    cnt = 0;
                    aging1Amt = 0;
                    aging2Amt = 0;
                    aging3Amt = 0;
                    aging4Amt = 0;
                    aging5Amt = 0;
                    amtOriginal = 0;
                    aging1AmtOriginal = 0;
                    aging2AmtOriginal = 0;
                    aging3AmtOriginal = 0;
                    aging4AmtOriginal = 0;
                    aging5AmtOriginal = 0;
                }

                //if (def.Type == UKClaimType.BILL_IN_ADVANCE)
                def.UKDebitNoteReceivedDate = def.UKDebitNoteReceivedDate;

                currencyId = def.Currency.CurrencyId;
                cnt += 1;
                amtOriginal += ((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount);
                amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);

                TimeSpan t = new TimeSpan(0, 0, 0);
                if (def.UKDebitNoteDate != DateTime.MinValue)
                    t = cutOffDate.Subtract(def.UKDebitNoteDate);
                //else if (def.Type == UKClaimType.BILL_IN_ADVANCE)
                //    t = cutOffDate.Subtract(def.CreateDate);

                if (t.Days >= 0 && t.Days <= 30)
                {
                    aging1AmtOriginal += Math.Round(((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount), 2, MidpointRounding.AwayFromZero);
                    aging1Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }
                else if (t.Days >= 31 && t.Days <= 60)
                {
                    aging2AmtOriginal += Math.Round(((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount), 2, MidpointRounding.AwayFromZero);
                    aging2Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }
                else if (t.Days >= 61 && t.Days <= 90)
                {
                    aging3AmtOriginal += Math.Round(((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount), 2, MidpointRounding.AwayFromZero);
                    aging3Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }
                else if (t.Days >= 91 && t.Days <= 120)
                {
                    aging4AmtOriginal += Math.Round(((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount), 2, MidpointRounding.AwayFromZero);
                    aging4Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }
                else if (t.Days > 120)
                {
                    aging5AmtOriginal += Math.Round(((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount), 2, MidpointRounding.AwayFromZero);
                    aging5Amt += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }
                isFirst = false;
            }
            summaryList.Add(new OSRptSummaryInfoDef(currencyId, currencyId.ToString(), amt, aging1Amt, aging2Amt, aging3Amt, aging4Amt, aging5Amt, amtOriginal, aging1AmtOriginal, aging2AmtOriginal, aging3AmtOriginal, aging4AmtOriginal, aging5AmtOriginal));

            this.row_RevaluationEUR.Visible = false;
            this.row_RevaluationGBP.Visible = false;

            this.lbl_USD_Total.Text = 0.ToString("#,##0.00");
            this.lbl_USD_Accrual.Text = usdAccrualAmt.ToString("#,##0.00");
            this.lbl_USD_AccrualTotal.Text = usdAccrualAmt.ToString("#,##0.00");
            this.lbl_GBP_Total.Text = 0.ToString("#,##0.00");
            this.lbl_GBP_Accrual.Text = gbpAccrualAmt.ToString("#,##0.00");
            this.lbl_GBP_AccrualTotal.Text = gbpAccrualAmt.ToString("#,##0.00");
            this.lbl_EUR_Total.Text = 0.ToString("#,##0.00");
            this.lbl_EUR_Accrual.Text = eurAccrualAmt.ToString("#,##0.00");
            this.lbl_EUR_AccrualTotal.Text = eurAccrualAmt.ToString("#,##0.00");

            foreach (OSRptSummaryInfoDef def in summaryList)
            {
                if (def.Name == CurrencyId.GBP.Id.ToString())
                {
                    this.lbl_GBP_Total.Text = def.TotalAmtOriginal.ToString("#,##0.00");
                    this.lbl_GBP_AccrualTotal.Text = (def.TotalAmtOriginal + gbpAccrualAmt).ToString("#,##0.00");

                    if (gbpRate != 0)
                    {
                        this.row_RevaluationGBP.Visible = true;
                        this.lbl_Revalution_GBP.Text = "Revaluation GBP to USD @ " + gbpRate.ToString("#,##0.0000");
                        this.lbl_Currency_GBPTotal.Text = Math.Round((((def.TotalAmtOriginal + gbpAccrualAmt) * gbpRate) - (def.TotalAmt + ((gbpAccrualAmt * gbpRate)))), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");
                        revaluationGrandTotal += Math.Round((((def.TotalAmtOriginal + gbpAccrualAmt) * gbpRate) - (def.TotalAmt + ((gbpAccrualAmt * gbpRate)))), 2, MidpointRounding.AwayFromZero);
                        this.lbl_Currency_GBPAging1.Text = Math.Round((((def.TotalAmtOriginal + gbpAccrualAmt) * gbpRate) - (def.TotalAmt + ((gbpAccrualAmt * gbpRate)))), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");
                        revaluationGrandAging1Total += Math.Round((((def.TotalAmtOriginal + gbpAccrualAmt) * gbpRate) - (def.TotalAmt + ((gbpAccrualAmt * gbpRate)))), 2, MidpointRounding.AwayFromZero);
                    }
                }
                if (def.Name == CurrencyId.EUR.Id.ToString())
                {
                    this.lbl_EUR_Total.Text = def.TotalAmtOriginal.ToString("#,##0.00");
                    this.lbl_EUR_AccrualTotal.Text = (def.TotalAmtOriginal + eurAccrualAmt).ToString("#,##0.00");
                    if (eurRate != 0)
                    {
                        this.row_RevaluationEUR.Visible = true;
                        this.lbl_Revalution_EUR.Text = "Revaluation EUR to USD @ " + eurRate.ToString("#,##0.0000");
                        this.lbl_Currency_EURTotal.Text = Math.Round((((def.TotalAmtOriginal + eurAccrualAmt) * eurRate) - (def.TotalAmt + ((eurRate * eurAccrualAmt)))), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");
                        revaluationGrandTotal += Math.Round((((def.TotalAmtOriginal + eurAccrualAmt) * eurRate) - (def.TotalAmt + ((eurRate * eurAccrualAmt)))), 2, MidpointRounding.AwayFromZero);
                        this.lbl_Currency_EURAging1.Text = Math.Round((((def.TotalAmtOriginal + eurAccrualAmt) * eurRate) - (def.TotalAmt + ((eurRate * eurAccrualAmt)))), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");
                        revaluationGrandAging1Total += Math.Round((((def.TotalAmtOriginal + eurAccrualAmt) * eurRate) - (def.TotalAmt + ((eurRate * eurAccrualAmt)))), 2, MidpointRounding.AwayFromZero);
                    }
                }
                if (def.Name == CurrencyId.USD.Id.ToString())
                {
                    this.lbl_USD_Total.Text = def.TotalAmtOriginal.ToString("#,##0.00");
                    this.lbl_USD_AccrualTotal.Text = (def.TotalAmtOriginal + usdAccrualAmt).ToString("#,##0.00");
                }

                this.lbl_Currency_Accrual.Text = ((gbpAccrualAmt * gbpRate) + (eurRate * eurAccrualAmt) + usdAccrualAmt).ToString("#,##0.00");
                this.lbl_Currency_Aging1_Accrual.Text = ((gbpAccrualAmt * gbpRate) + (eurRate * eurAccrualAmt) + usdAccrualAmt).ToString("#,##0.00");

                this.lbl_Currency_Total.Text = totalUSD.ToString("#,##0.00");
                this.lbl_Currency_Aging1.Text = totalAging1.ToString("#,##0.00");
                this.lbl_Currency_Aging2.Text = totalAging2.ToString("#,##0.00");
                this.lbl_Currency_Aging3.Text = totalAging3.ToString("#,##0.00");
                this.lbl_Currency_Aging4.Text = totalAging4.ToString("#,##0.00");
                this.lbl_Currency_Aging5.Text = totalAging5.ToString("#,##0.00");

                this.lbl_Currency_AccrualTotal.Text = ((gbpAccrualAmt * gbpRate) + (eurRate * eurAccrualAmt) + usdAccrualAmt + totalUSD).ToString("#,##0.00");
                this.lbl_Currency_Aging1_AccrualTotal.Text = ((gbpAccrualAmt * gbpRate) + (eurRate * eurAccrualAmt) + usdAccrualAmt + totalAging1).ToString("#,##0.00");
                this.lbl_Currency_Aging2_AccrualTotal.Text = totalAging2.ToString("#,##0.00");
                this.lbl_Currency_Aging3_AccrualTotal.Text = totalAging3.ToString("#,##0.00");
                this.lbl_Currency_Aging4_AccrualTotal.Text = totalAging4.ToString("#,##0.00");
                this.lbl_Currency_Aging5_AccrualTotal.Text = totalAging5.ToString("#,##0.00");

            }

            revaluationGrandTotal += ((gbpAccrualAmt * gbpRate) + (eurRate * eurAccrualAmt) + usdAccrualAmt);
            revaluationGrandAging1Total += ((gbpAccrualAmt * gbpRate) + (eurRate * eurAccrualAmt) + usdAccrualAmt);

            this.lbl_Currency_GrandTotal.Text = revaluationGrandTotal.ToString("#,##0.00");
            this.lbl_Currency_GrandAging1.Text = revaluationGrandAging1Total.ToString("#,##0.00");
            this.lbl_Currency_GrandAging2.Text = revaluationGrandAging2Total.ToString("#,##0.00");
            this.lbl_Currency_GrandAging3.Text = revaluationGrandAging3Total.ToString("#,##0.00");
            this.lbl_Currency_GrandAging4.Text = revaluationGrandAging4Total.ToString("#,##0.00");
            this.lbl_Currency_GrandAging5.Text = revaluationGrandAging5Total.ToString("#,##0.00");

        }


        private List<UKDiscountClaimDef> vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (List<UKDiscountClaimDef>)ViewState["SearchResult"];
            }
        }


        protected void repUKClaim_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HtmlTableRow tr_Normal = (HtmlTableRow)e.Item.FindControl("trNormal");
            //HtmlTableRow tr_Missing = (HtmlTableRow)e.Item.FindControl("trMissing");
            //HtmlTableRow tr_PendingCancellation = (HtmlTableRow)e.Item.FindControl("trPendingCancellation");
            HtmlTableRow tr_SubTotal = (HtmlTableRow)e.Item.FindControl("trSubTotal");

            if (WebHelper.isRepeaterFooter(e))
            {
                HtmlTableRow tr_Footer = (HtmlTableRow)e.Item.FindControl("trFooter");
                ((Label)e.Item.FindControl("lbl_Subtotal_AmountInUSD")).Text = subtotalUSD.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging1")).Text = subtotalAging1.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging2")).Text = subtotalAging2.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging3")).Text = subtotalAging3.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging4")).Text = subtotalAging4.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging5")).Text = subtotalAging5.ToString("#,##0.00");

                summaryList.Add(new OSRptSummaryInfoDef(officeId, OfficeId.getName(officeId), subtotalUSD, subtotalAging1, subtotalAging2, subtotalAging3, subtotalAging4, subtotalAging5, 0, 0, string.Empty));
            }
            else if (WebHelper.isRepeaterNormalItemType(e))
            {
                UKDiscountClaimDef def = (UKDiscountClaimDef)vwSearchResult[e.Item.ItemIndex];
                //if (def.Type == UKClaimType.BILL_IN_ADVANCE)
                def.UKDebitNoteReceivedDate = def.UKDebitNoteReceivedDate;

                tr_SubTotal.Visible = false;
                if (isFirst) officeId = def.OfficeId;
                if (def.OfficeId != officeId)
                {
                    tr_SubTotal.Visible = true;
                    ((Label)e.Item.FindControl("lbl_Subtotal_AmountInUSD")).Text = subtotalUSD.ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging1")).Text = subtotalAging1.ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging2")).Text = subtotalAging2.ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging3")).Text = subtotalAging3.ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging4")).Text = subtotalAging4.ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_Subtotal_lbl_Aging5")).Text = subtotalAging5.ToString("#,##0.00");
                    summaryList.Add(new OSRptSummaryInfoDef(officeId, OfficeId.getName(officeId), subtotalUSD, subtotalAging1, subtotalAging2, subtotalAging3, subtotalAging4, subtotalAging5, 0, 0, string.Empty));

                    officeId = def.OfficeId;
                    cnt += 1;
                    subtotalUSD = 0;
                    subtotalAging1 = 0;
                    subtotalAging2 = 0;
                    subtotalAging3 = 0;
                    subtotalAging4 = 0;
                    subtotalAging5 = 0;
                }
                //bool isNSCost = false;
                //if (def.ClaimRequestId > 0)
                //{
                //    QAIS.ClaimRequestDef crDef = svc.GetClaimRequestByKey(def.ClaimRequestId);
                //    if (crDef.NSRechargePercent > 0) isNSCost = true;
                //}


                ((Label)e.Item.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);
                ((Label)e.Item.FindControl("lbl_M_Office")).Text = OfficeId.getName(def.OfficeId);
                ((Label)e.Item.FindControl("lbl_PC_Office")).Text = OfficeId.getName(def.OfficeId);
                //((Label)e.Item.FindControl("lbl_HandlingOffice")).Text = OfficeId.getName(def.HandlingOfficeId);
                //((Label)e.Item.FindControl("lbl_M_HandlingOffice")).Text = OfficeId.getName(def.HandlingOfficeId);
                //((Label)e.Item.FindControl("lbl_PC_HandlingOffice")).Text = OfficeId.getName(def.HandlingOfficeId);

                //((Label)e.Item.FindControl("lbl_ClaimType")).Text = (def.Type == UKClaimType.MFRN ? def.Type.Name + " (" + def.ClaimMonth + ")" : def.Type.Name);
                //((Label)e.Item.FindControl("lbl_M_ClaimType")).Text = (def.Type == UKClaimType.MFRN ? def.Type.Name + " (" + def.ClaimMonth + ")" : def.Type.Name);
                //((Label)e.Item.FindControl("lbl_PC_ClaimType")).Text = (def.Type == UKClaimType.MFRN ? def.Type.Name + " (" + def.ClaimMonth + ")" : def.Type.Name);
                ((Label)e.Item.FindControl("lbl_ItemNo")).Text = def.ItemNo;
                ((Label)e.Item.FindControl("lbl_M_ItemNo")).Text = def.ItemNo;
                ((Label)e.Item.FindControl("lbl_PC_ItemNo")).Text = def.ItemNo;
                ((Label)e.Item.FindControl("lbl_UKDNNo")).Text = def.UKDebitNoteNo;
                ((Label)e.Item.FindControl("lbl_M_UKDNNo")).Text = def.UKDebitNoteNo;
                ((Label)e.Item.FindControl("lbl_PC_UKDNNo")).Text = def.UKDebitNoteNo;
                ((Label)e.Item.FindControl("lbl_UKDNDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteDate);
                ((Label)e.Item.FindControl("lbl_M_UKDNDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteDate);
                ((Label)e.Item.FindControl("lbl_PC_UKDNDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteDate);
                ((Label)e.Item.FindControl("lbl_UKDNReceivedDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteReceivedDate);
                ((Label)e.Item.FindControl("lbl_M_UKDNReceivedDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteReceivedDate);
                ((Label)e.Item.FindControl("lbl_PC_UKDNReceivedDate")).Text = DateTimeUtility.getDateString(def.UKDebitNoteReceivedDate);

                if (def.ProductTeamId != 0)
                {
                    ((Label)e.Item.FindControl("lbl_DeptCode")).Text = (def.OfficeId == OfficeId.DG.Id ? CommonUtil.getProductCodeDefByKey(def.ProductTeamId).CodeDescription : CommonUtil.getProductCodeDefByKey(def.ProductTeamId).Code);
                    ((Label)e.Item.FindControl("lbl_M_DeptCode")).Text = (def.OfficeId == OfficeId.DG.Id ? CommonUtil.getProductCodeDefByKey(def.ProductTeamId).CodeDescription : CommonUtil.getProductCodeDefByKey(def.ProductTeamId).Code) + " M";
                    ((Label)e.Item.FindControl("lbl_PC_DeptCode")).Text = (def.OfficeId == OfficeId.DG.Id ? CommonUtil.getProductCodeDefByKey(def.ProductTeamId).CodeDescription : CommonUtil.getProductCodeDefByKey(def.ProductTeamId).Code) + " PC";
                }
                else
                {
                    ((Label)e.Item.FindControl("lbl_DeptCode")).Text = "OTHERS";
                    ((Label)e.Item.FindControl("lbl_M_DeptCode")).Text = "OTHERS";
                    ((Label)e.Item.FindControl("lbl_PC_DeptCode")).Text = "OTHERS";
                }

                //if (def.OfficeId == OfficeId.HK.Id && def.TermOfPurchaseId != 1)
                //{
                //    if (def.SZVendor != null)
                //    {
                //        ((Label)e.Item.FindControl("lbl_Vendor")).Text = def.SZVendor.Name;
                //        ((Label)e.Item.FindControl("lbl_M_Vendor")).Text = def.SZVendor.Name;
                //        ((Label)e.Item.FindControl("lbl_PC_Vendor")).Text = def.SZVendor.Name;
                //    }
                //    else
                //    {
                //        ((Label)e.Item.FindControl("lbl_Vendor")).Text = def.Vendor.Name;
                //        ((Label)e.Item.FindControl("lbl_M_Vendor")).Text = def.Vendor.Name;
                //        ((Label)e.Item.FindControl("lbl_PC_Vendor")).Text = def.Vendor.Name;
                //    }
                //}
                //else
                //{
                ((Label)e.Item.FindControl("lbl_Vendor")).Text = def.Vendor.Name;
                ((Label)e.Item.FindControl("lbl_M_Vendor")).Text = def.Vendor.Name;
                ((Label)e.Item.FindControl("lbl_PC_Vendor")).Text = def.Vendor.Name;
                //}

                if (def.OfficeId == OfficeId.HK.Id || def.OfficeId == OfficeId.CA.Id || def.OfficeId == OfficeId.TH.Id)
                {
                    /*
                    this.lbl_HandledByLabel.Visible = true;

                    this.tdHandledBy.Width = "60";
                    */
                    if (def.TermOfPurchaseId != 1 && def.OfficeId == OfficeId.HK.Id)
                    {
                        ((Label)e.Item.FindControl("lbl_HandledBy")).Text = "Garment";
                        ((Label)e.Item.FindControl("lbl_M_HandledBy")).Text = "Garment";
                        ((Label)e.Item.FindControl("lbl_PC_HandledBy")).Text = "Garment";
                    }
                    else
                    {
                        ((Label)e.Item.FindControl("lbl_HandledBy")).Text = "Garment";
                        ((Label)e.Item.FindControl("lbl_M_HandledBy")).Text = "Garment";
                        ((Label)e.Item.FindControl("lbl_PC_HandledBy")).Text = "Garment";

                        /*
                        foreach (ProductCodeRef osDef in this.hardGoodsTeamList)
                        {
                            if (osDef.ProductCodeId == def.ProductTeamId && osDef.Code != "NCHB")
                            {
                                ((Label)e.Item.FindControl("lbl_HandledBy")).Text = "Hard Goods";
                                ((Label)e.Item.FindControl("lbl_M_HandledBy")).Text = "Hard Goods";
                                ((Label)e.Item.FindControl("lbl_PC_HandledBy")).Text = "Hard Goods";
                                break;
                            }
                        }
                        */

                        if (UKClaimManager.Instance.isHomeAndBeautyProductTeam(def.ProductTeamId))
                        {
                            ((Label)e.Item.FindControl("lbl_HandledBy")).Text = "Hard Goods";
                            ((Label)e.Item.FindControl("lbl_M_HandledBy")).Text = "Hard Goods";
                            ((Label)e.Item.FindControl("lbl_PC_HandledBy")).Text = "Hard Goods";
                        }

                    }
                }
                /*
                else
                    this.tdHandledBy.Width = "5";
                */

                //string t5 = string.Empty;

                //if (def.ProductTeamId == 0)
                //    t5 = "AUD";
                //else if (def.Type == UKClaimType.BILL_IN_ADVANCE && def.DebitNoteNo != string.Empty)
                //{
                //    QAIS.ClaimRequestDef req = svc.GetClaimRequestByKey(int.Parse(def.DebitNoteNo));
                //    if (req.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                //        t5 = "DN";
                //    else
                //    {
                //        int n;
                //        string s = string.Empty;
                //        for (int i = 0; i <= req.FormNo.Length - 1; i++)
                //        {
                //            if (int.TryParse(req.FormNo.Substring(i, 1), out n))
                //            {
                //                s = req.FormNo.Substring(0, i);
                //                break;
                //            }
                //        }
                //        t5 = s;
                //    }
                //}
                //else
                //    t5 = def.T5Code;

                /*
                if (def.Type == UKClaimType.FABRIC_TEST || def.Type == UKClaimType.PENALTY_CHARGE)
                    t5 = "DN";
                */

                //((Label)e.Item.FindControl("lbl_T5")).Text = t5;
                //((Label)e.Item.FindControl("lbl_M_T5")).Text = t5;
                //((Label)e.Item.FindControl("lbl_PC_T5")).Text = t5;

                ((Label)e.Item.FindControl("lbl_ContractNo")).Text = def.ContractNo;
                ((Label)e.Item.FindControl("lbl_M_ContractNo")).Text = def.ContractNo;
                ((Label)e.Item.FindControl("lbl_PC_ContractNo")).Text = def.ContractNo;
                //if (def.Type.Id != UKClaimType.AUDIT_FEE.Id)
                //{
                //    DateTime lastShipmentDate = ShipmentManager.Instance.getLastShipmentDate(def.ItemNo, def.ContractNo, def.Vendor.VendorId);
                //    if (lastShipmentDate != DateTime.MinValue)
                //    {
                //        ((Label)e.Item.FindControl("lbl_LastShipmentDate")).Text = DateTimeUtility.getDateString(lastShipmentDate);
                //        ((Label)e.Item.FindControl("lbl_M_LastShipmentDate")).Text = DateTimeUtility.getDateString(lastShipmentDate);
                //        ((Label)e.Item.FindControl("lbl_PC_LastShipmentDate")).Text = DateTimeUtility.getDateString(lastShipmentDate);
                //    }
                //}

                ((Label)e.Item.FindControl("lbl_Currency")).Text = def.Currency.CurrencyCode;
                ((Label)e.Item.FindControl("lbl_M_Currency")).Text = def.Currency.CurrencyCode;
                ((Label)e.Item.FindControl("lbl_PC_Currency")).Text = def.Currency.CurrencyCode;
                ((Label)e.Item.FindControl("lbl_Qty")).Text = def.Qty.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_M_Qty")).Text = def.Qty.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_PC_Qty")).Text = def.Qty.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_Amount")).Text = ((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount).ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_M_Amount")).Text = ((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount).ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_PC_Amount")).Text = ((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount).ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_AmountInUSD")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_M_AmountInUSD")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_PC_AmountInUSD")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");

                totalUSD += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                subtotalUSD += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);

                int age = 0;
                TimeSpan t = new TimeSpan(0, 0, 0);
                if (def.UKDebitNoteDate != DateTime.MinValue)
                    t = cutOffDate.Subtract(def.UKDebitNoteDate);
                //else if (def.Type == UKClaimType.BILL_IN_ADVANCE)
                //    t = cutOffDate.Subtract(def.CreateDate);

                if (t.Days >= 0 && t.Days <= 30)
                {
                    ((Label)e.Item.FindControl("lbl_Aging1")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_M_Aging1")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_PC_Aging1")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    totalAging1 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                    subtotalAging1 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }
                else if (t.Days >= 31 && t.Days <= 60)
                {
                    ((Label)e.Item.FindControl("lbl_Aging2")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_M_Aging2")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_PC_Aging2")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    totalAging2 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                    subtotalAging2 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }
                else if (t.Days >= 61 && t.Days <= 90)
                {
                    ((Label)e.Item.FindControl("lbl_Aging3")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_M_Aging3")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_PC_Aging3")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    totalAging3 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                    subtotalAging3 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }
                else if (t.Days >= 91 && t.Days <= 120)
                {
                    ((Label)e.Item.FindControl("lbl_Aging4")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_M_Aging4")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_PC_Aging4")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    totalAging4 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                    subtotalAging4 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }
                else if (t.Days > 120)
                {
                    ((Label)e.Item.FindControl("lbl_Aging5")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_M_Aging5")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    ((Label)e.Item.FindControl("lbl_PC_Aging5")).Text = (((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))).ToString("#,##0.00");
                    totalAging5 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                    subtotalAging5 += Math.Round((((def.UKDebitNoteReceivedDate == DateTime.MinValue) ? def.Amount * -1 : def.Amount) * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate)) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, (def.UKDebitNoteReceivedDate == DateTime.MinValue ? DateTime.Today : def.UKDebitNoteReceivedDate))), 2, MidpointRounding.AwayFromZero);
                }

                //QAIS.ClaimRequestDef requestDef = null;
                //((Label)e.Item.FindControl("lbl_QAISStatus")).Text = "N/A";
                //((Label)e.Item.FindControl("lbl_M_QAISStatus")).Text = "N/A";
                //((Label)e.Item.FindControl("lbl_PC_QAISStatus")).Text = "N/A";
                ((Label)e.Item.FindControl("lbl_FormIssueDate")).Text = DateTimeUtility.getDateString(def.IssueDate);
                ((Label)e.Item.FindControl("lbl_M_FormIssueDate")).Text = DateTimeUtility.getDateString(def.IssueDate);
                ((Label)e.Item.FindControl("lbl_PC_FormIssueDate")).Text = DateTimeUtility.getDateString(def.IssueDate);
                //((Label)e.Item.FindControl("lbl_QAISCreateDate")).Text = "N/A";
                //((Label)e.Item.FindControl("lbl_M_QAISCreateDate")).Text = "N/A";
                //((Label)e.Item.FindControl("lbl_PC_QAISCreateDate")).Text = "N/A";

                ((Label)e.Item.FindControl("lbl_IsAuthorized")).Text = def.IsUKDiscount ? "Yes" : "No";
                ((Label)e.Item.FindControl("lbl_M_IsAuthorized")).Text = def.IsUKDiscount ? "Yes" : "No";
                ((Label)e.Item.FindControl("lbl_PC_IsAuthorized")).Text = def.IsUKDiscount ? "Yes" : "No";
                /*
                ((Label)e.Item.FindControl("lbl_STSForm")).Text = "N/A";
                ((Label)e.Item.FindControl("lbl_M_STSForm")).Text = "N/A";
                ((Label)e.Item.FindControl("lbl_PC_STSForm")).Text = "N/A";
                */

                //((Label)e.Item.FindControl("lbl_Remark")).Text = def.Remark.Replace("\n", "<br style=\"mso-data-placement:same-cell;\">");
                //((Label)e.Item.FindControl("lbl_M_Remark")).Text = def.Remark.Replace("\n", "<br style=\"mso-data-placement:same-cell;\">");
                //((Label)e.Item.FindControl("lbl_PC_Remark")).Text = def.Remark.Replace("\n", "<br style=\"mso-data-placement:same-cell;\">");

                //if (def.ClaimRequestId > 0)
                //{
                //    requestDef = svc.GetClaimRequestByKey(def.ClaimRequestId);
                //    ((Label)e.Item.FindControl("lbl_CommentFromMer")).Text = requestDef.Remark.Replace("\n", "<br style=\"mso-data-placement:same-cell;\">");
                //    ((Label)e.Item.FindControl("lbl_M_CommentFromMer")).Text = requestDef.Remark.Replace("\n", "<br style=\"mso-data-placement:same-cell;\">");
                //    ((Label)e.Item.FindControl("lbl_PC_CommentFromMer")).Text = requestDef.Remark.Replace("\n", "<br style=\"mso-data-placement:same-cell;\">");
                //    /*
                //    if (requestDef.NoForm)
                //    {
                //        ((Label)e.Item.FindControl("lbl_Form")).Text = "No";
                //        ((Label)e.Item.FindControl("lbl_M_Form")).Text = "No";
                //        ((Label)e.Item.FindControl("lbl_PC_Form")).Text = "No";
                //    }
                //    else
                //    {
                //        ((Label)e.Item.FindControl("lbl_Form")).Text = "Yes";
                //        ((Label)e.Item.FindControl("lbl_M_Form")).Text = "Yes";
                //        ((Label)e.Item.FindControl("lbl_PC_Form")).Text = "Yes";
                //    }
                //    */
                //    ((Label)e.Item.FindControl("lbl_QAISStatus")).Text = requestDef.WorkflowStatus.Name + (requestDef.BIAStatus == 2 ? " (BIA)" : string.Empty);
                //    ((Label)e.Item.FindControl("lbl_M_QAISStatus")).Text = requestDef.WorkflowStatus.Name + (requestDef.BIAStatus == 2 ? " (BIA)" : string.Empty);
                //    ((Label)e.Item.FindControl("lbl_PC_QAISStatus")).Text = requestDef.WorkflowStatus.Name + (requestDef.BIAStatus == 2 ? " (BIA)" : string.Empty);
                //    ((Label)e.Item.FindControl("lbl_FormIssueDate")).Text = DateTimeUtility.getDateString(requestDef.IssueDate);
                //    ((Label)e.Item.FindControl("lbl_M_FormIssueDate")).Text = DateTimeUtility.getDateString(requestDef.IssueDate);
                //    ((Label)e.Item.FindControl("lbl_PC_FormIssueDate")).Text = DateTimeUtility.getDateString(requestDef.IssueDate);
                //    ((Label)e.Item.FindControl("lbl_QAISCreateDate")).Text = DateTimeUtility.getDateString(requestDef.CreateDate);
                //    ((Label)e.Item.FindControl("lbl_M_QAISCreateDate")).Text = DateTimeUtility.getDateString(requestDef.CreateDate);
                //    ((Label)e.Item.FindControl("lbl_PC_QAISCreateDate")).Text = DateTimeUtility.getDateString(requestDef.CreateDate);

                //    ((Label)e.Item.FindControl("lbl_IsAuthorized")).Text = (requestDef.IsAuthorized ? "Yes" : "No");
                //    ((Label)e.Item.FindControl("lbl_M_IsAuthorized")).Text = (requestDef.IsAuthorized ? "Yes" : "No");
                //    ((Label)e.Item.FindControl("lbl_PC_IsAuthorized")).Text = (requestDef.IsAuthorized ? "Yes" : "No");

                //    /*
                //    if (svc.IsAuthorizationFormAvailable(requestDef.RequestId))
                //    {
                //        ((Label)e.Item.FindControl("lbl_STSForm")).Text = "Yes";
                //        ((Label)e.Item.FindControl("lbl_M_STSForm")).Text = "Yes";
                //        ((Label)e.Item.FindControl("lbl_PC_STSForm")).Text = "Yes";
                //    }
                //    else
                //    {
                //        ((Label)e.Item.FindControl("lbl_STSForm")).Text = "No";
                //        ((Label)e.Item.FindControl("lbl_M_STSForm")).Text = "No";
                //        ((Label)e.Item.FindControl("lbl_PC_STSForm")).Text = "No";
                //    }                      
                //    */
                //    if (requestDef.BIAStatus == 2)
                //    {
                //        UKClaimDef biaClaimDef = UKClaimManager.Instance.getBIAUKClaimByClaimRequestId(requestDef.RequestId);
                //        if (biaClaimDef != null)
                //        {
                //            def.Remark = (biaClaimDef.Remark + "\n" + def.Remark);
                //            ((Label)e.Item.FindControl("lbl_Remark")).Text = def.Remark.Replace("\n", "<br style=\"mso-data-placement:same-cell;\">");
                //            ((Label)e.Item.FindControl("lbl_M_Remark")).Text = def.Remark.Replace("\n", "<br style=\"mso-data-placement:same-cell;\">");
                //            ((Label)e.Item.FindControl("lbl_PC_Remark")).Text = def.Remark.Replace("\n", "<br style=\"mso-data-placement:same-cell;\">");
                //        }
                //    }
                //}
                /*
                ((Label)e.Item.FindControl("lbl_Remark")).Text = def.Remark + (isNSCost ? " [ NS Cost Requires Approval ]" : string.Empty);
                */

                if (def.WorkflowStatus.Id == ClaimWFS.NEW.Id)
                {
                    //if (def.ClaimRequestId > 0)
                    //{
                    //    /*
                    //    if (requestDef.NoForm)
                    //    {
                    //        ((Label)e.Item.FindControl("lbl_Status")).Text = "INCOMPLETE SUPPORT INFORMATION";
                    //        ((Label)e.Item.FindControl("lbl_M_Status")).Text = "INCOMPLETE SUPPORT INFORMATION";
                    //        ((Label)e.Item.FindControl("lbl_PC_Status")).Text = "INCOMPLETE SUPPORT INFORMATION";
                    //    }
                    //    else
                    //    {
                    //        ((Label)e.Item.FindControl("lbl_Status")).Text = "D/N RECEIVED";
                    //        ((Label)e.Item.FindControl("lbl_M_Status")).Text = "D/N RECEIVED";
                    //        ((Label)e.Item.FindControl("lbl_PC_Status")).Text = "D/N RECEIVED";
                    //    }
                    //    */
                    //    ((Label)e.Item.FindControl("lbl_Status")).Text = "D/N RECEIVED";
                    //    ((Label)e.Item.FindControl("lbl_M_Status")).Text = "D/N RECEIVED";
                    //    ((Label)e.Item.FindControl("lbl_PC_Status")).Text = "D/N RECEIVED";
                    //}
                    //else
                    //{
                    ((Label)e.Item.FindControl("lbl_Status")).Text = "D/N RECEIVED";
                    ((Label)e.Item.FindControl("lbl_M_Status")).Text = "D/N RECEIVED";
                    ((Label)e.Item.FindControl("lbl_PC_Status")).Text = "D/N RECEIVED";
                    //}
                }
                /* comment out @2012-08-22 to reflect latest actual status
                else if (def.WorkflowStatus.Id == ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id)
                {
                    ((Label)e.Item.FindControl("lbl_Status")).Text = ClaimWFS.SUBMITTED.Name;
                    ((Label)e.Item.FindControl("lbl_M_Status")).Text = ClaimWFS.SUBMITTED.Name;
                    ((Label)e.Item.FindControl("lbl_PC_Status")).Text = ClaimWFS.SUBMITTED.Name;
                }
                */
                else if (def.WorkflowStatus.Id == ClaimWFS.SUBMITTED.Id)
                {
                    ((Label)e.Item.FindControl("lbl_Status")).Text = ClaimWFS.SUBMITTED.Name;
                    ((Label)e.Item.FindControl("lbl_M_Status")).Text = ClaimWFS.SUBMITTED.Name;
                    ((Label)e.Item.FindControl("lbl_PC_Status")).Text = ClaimWFS.SUBMITTED.Name;

                    //if (def.ClaimRequestId > 0)
                    //{
                    //if (requestDef.WorkflowStatusId == 10)
                    //{
                    //    ((Label)e.Item.FindControl("lbl_Status")).Text = "D/N RECEIVED";
                    //    ((Label)e.Item.FindControl("lbl_M_Status")).Text = "D/N RECEIVED";
                    //    ((Label)e.Item.FindControl("lbl_PC_Status")).Text = "D/N RECEIVED";
                    //}
                    //}
                }
                else
                {
                    ((Label)e.Item.FindControl("lbl_Status")).Text = def.WorkflowStatus.Name;
                    ((Label)e.Item.FindControl("lbl_M_Status")).Text = def.WorkflowStatus.Name;
                    ((Label)e.Item.FindControl("lbl_PC_Status")).Text = def.WorkflowStatus.Name;
                }
                if (def.ClaimId < 0)
                {
                    ((Label)e.Item.FindControl("lbl_Status")).Text = "UK REFUND";
                    ((Label)e.Item.FindControl("lbl_M_Status")).Text = "UK REFUND";
                    ((Label)e.Item.FindControl("lbl_PC_Status")).Text = "UK REFUND";
                }
                //tr_Normal.Visible = (requestDef.WorkflowStatusId != 10);
                //tr_Missing.Visible = !(def.ClaimRequestId > 0 || def.Type.Id == UKClaimType.AUDIT_FEE.Id || def.Type.Id == UKClaimType.GB_TEST.Id || def.Type.Id == UKClaimType.BILL_IN_ADVANCE.Id);
                //tr_PendingCancellation.Visible = (requestDef.WorkflowStatusId == 10);
                isFirst = false;

                decimal futureOrderAmt = ShipmentManager.Instance.getFutureOrderAmtByVendorId(def.Vendor.VendorId);
                decimal osPaymentAmt = ShipmentManager.Instance.getOutstandingPaymentAmtByVendorId(def.Vendor.VendorId);

                ((Label)e.Item.FindControl("lbl_FutureOrder")).Text = futureOrderAmt.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_M_FutureOrder")).Text = futureOrderAmt.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_PC_FutureOrder")).Text = futureOrderAmt.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_APOS")).Text = osPaymentAmt.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_M_APOS")).Text = osPaymentAmt.ToString("#,##0.00");
                ((Label)e.Item.FindControl("lbl_PC_APOS")).Text = osPaymentAmt.ToString("#,##0.00");
            }
        }

        protected void repOfficeSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            OSRptSummaryInfoDef def = summaryList[e.Item.ItemIndex];
            ((Label)e.Item.FindControl("lbl_Summary_Office")).Text = def.Name;
            ((Label)e.Item.FindControl("lbl_OfficeTotal_AmountInUSD")).Text = def.TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_OfficeTotal_Aging1")).Text = def.Aging1TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_OfficeTotal_Aging2")).Text = def.Aging2TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_OfficeTotal_Aging3")).Text = def.Aging3TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_OfficeTotal_Aging4")).Text = def.Aging4TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_OfficeTotal_Aging5")).Text = def.Aging5TotalAmt.ToString("#,##0.00");

        }

        protected void repVendorSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            OSRptSummaryInfoDef def = summaryList[e.Item.ItemIndex];
            ((Label)e.Item.FindControl("lbl_Summary_Vendor")).Text = def.Name;
            ((Label)e.Item.FindControl("lbl_VendorTotal_AmountInUSD")).Text = def.TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_VendorTotal_Aging1")).Text = def.Aging1TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_VendorTotal_Aging2")).Text = def.Aging2TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_VendorTotal_Aging3")).Text = def.Aging3TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_VendorTotal_Aging4")).Text = def.Aging4TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_VendorTotal_Aging5")).Text = def.Aging5TotalAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_VendorTotal_FutureOrderAmt")).Text = def.TotalFutureOrderAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_VendorTotal_APOSAmt")).Text = def.TotalAPOSAmt.ToString("#,##0.00");
            ((Label)e.Item.FindControl("lbl_Vendor_HandledBy")).Text = def.HandledBy;
        }

    }
}