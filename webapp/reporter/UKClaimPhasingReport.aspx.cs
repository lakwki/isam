using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using com.next.infra.util;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.claim;
using com.next.common.web.commander;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.isam.appserver.claim;
using System.Collections;

namespace com.next.isam.webapp.reporter
{
    public partial class UKClaimPhasingReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public struct MIME_Type
        {
            public const string Excel = "application/vnd.ms-excel";
            public const string Word = "application/vnd.ms-word";
            public const string PowerPoint = "application/vnd.ms-powerpoint";
        }

        private string supplierName = string.Empty;
        private string currencyName = string.Empty;
        private int currencyId = 0;
        private int vendorId = 0;
        private int officeId = 0;
        private int count;

        #region Initialize the period total variables 
        private decimal[,] ttlSub = new decimal[2, 13];
        private decimal[,] ttlOfficeSub = new decimal[2, 13];
        #endregion

        bool isAlternate = false;
        bool isFirst = true;
        bool isSummary = false;
        DateTime shipmentDate = DateTime.MinValue;
        bool isSingleVendor;

        string param_FiscalYear;
        string param_Period;
        string param_VendorId;
        string param_OfficeId;
        string param_ReportType;
        string param_UserId;
        string param_ReportCode;
        List<UKClaimPhasingDef> list = null;
        string reportGroupBy = string.Empty;
        const string claimType = "Claim Type";
        const string claimReason = "Claim Reason";
        const string claimOffice = "Claim Office";
        const int currentYear = 1, lastYear = 0;
        const int paidByVendor = 1, paidByNS = 0;
        string yearlySummaryItemDesc = string.Empty;
        private ArrayList yearlySummaryList, officeSummaryList;


 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.EnableViewState = false;
                Response.Clear();
                Response.Charset = "";

                Response.Buffer = true;
                Response.ContentType = MIME_Type.Excel;
                Response.AddHeader("Content-Disposition", "attachment;filename=UKClaimPhasingReport.xls");

                param_OfficeId = Request.Params["officeId"];
                param_Period = Request.Params["period"];
                param_FiscalYear = Request.Params["fiscalYear"];
                param_ReportType = Request.Params["docType"];
                param_VendorId = Request.Params["vendorId"];
                param_UserId = Request.Params["userId"];
                param_OfficeId = Request.Params["officeId"];
                param_ReportCode = Request.Params["reportCode"];
                if (param_OfficeId == null && param_Period == null && param_FiscalYear == null && param_ReportType == null && param_VendorId == null && param_UserId==null)
                {
                    param_OfficeId = Context.Items[AccountCommander.Param.officeId].ToString();
                    param_Period = Context.Items[AccountCommander.Param.period].ToString();
                    param_FiscalYear = Context.Items[AccountCommander.Param.fiscalYear].ToString();
                    param_ReportType = Context.Items[AccountCommander.Param.docType].ToString();
                    param_VendorId = Context.Items[AccountCommander.Param.vendorId].ToString();
                    param_UserId = (this.LogonUserId == int.MinValue ? -1 : this.LogonUserId).ToString();
                    param_ReportCode = string.Empty;
                    list = (List<UKClaimPhasingDef>)Context.Items[AccountCommander.Param.ukClaimPhasingList];
                }
                string[] period = (param_Period == null ? "0,0".Split(',') : param_Period.Split(','));
                int vendorId = int.Parse(param_VendorId);
                isSingleVendor = (vendorId != -1);
                isSummary = (param_ReportType == "2");
                if (list == null)
                    list = UKClaimManager.Instance.getUKClaimPhasingReport(Int32.Parse(param_FiscalYear), Int32.Parse(period[0]), Int32.Parse(period[1]), int.Parse(param_OfficeId), vendorId, (param_ReportType == "0" ? 1 : 0));

                if (string.IsNullOrEmpty(param_ReportCode))
                    tr_ReportCode.Style.Add("Display", "none");
                this.lbl_ReportCode.Text = param_ReportCode;
                this.lblPrintTime.Text = "Print Date: " + DateTime.Today.ToShortDateString() + "  " + DateTime.Now.ToShortTimeString();
                this.lblPrintTime.Text += "<br>Print By: " + (param_UserId == "-1" ? "" : CommonUtil.getUserByKey(Int32.Parse(param_UserId)).DisplayName);
                this.lbl_Office.Text = (param_OfficeId == "-1" ? "ALL" : OfficeId.getName(int.Parse(param_OfficeId)));
                this.lbl_Period.Text = "Year " + param_FiscalYear +"  P" + period[0] + " - P" + period[1];
                reportGroupBy = (param_ReportType == "0" ? claimReason : claimType);
                this.lbl_ReportType.Text = (param_ReportType == "2" ? "Summary" : "Group By " + reportGroupBy);
                this.lbl_GroupHeader.Text = reportGroupBy;
                this.lbl_Supplier.Text = (vendorId == -1 ? "ALL" : IndustryUtil.getVendorByKey(vendorId).Name);

                this.getReport();
            }
        }

        private void getReport()
        {
            this.vwSearchResult = list;
            this.yearlySummaryList = getUKClaimGroupSummary(reportGroupBy,1);
            this.officeSummaryList = getUKClaimGroupSummary(claimOffice,0);

            this.repUKClaim.DataSource = list;
            this.repUKClaim.DataBind();

            #region Report Summary
            #region disable summary by claim type
                this.trLastYearRework.Visible = false;
                this.trLastYearReject.Visible = false;
                this.trLastYearMFRN.Visible = false;
                this.trLastYearCFS.Visible = false;
                this.trLastYearSafetyIssue.Visible = false;
                this.trLastYearAuditFee.Visible = false;
                this.trLastYearFabricTest.Visible = false;
                this.trLastYearPenaltyCharge.Visible = false;
                this.trLastYearQCC.Visible = false;
                this.trLastYearCHB.Visible = false;
                this.trLastYearGBTest.Visible = false;
                this.trLastYearFIRA.Visible = false;
                this.trLastYearOthers.Visible = false;
                this.trLastYearGrandTotal.Visible = false;

                this.trCurrentYearRework.Visible = false;
                this.trCurrentYearReject.Visible = false;
                this.trCurrentYearMFRN.Visible = false;
                this.trCurrentYearCFS.Visible = false;
                this.trCurrentYearSafetyIssue.Visible = false;
                this.trCurrentYearAuditFee.Visible = false;
                this.trCurrentYearFabricTest.Visible = false;
                this.trCurrentYearPenaltyCharge.Visible = false;
                this.trCurrentYearQCC.Visible = false;
                this.trCurrentYearCHB.Visible = false;
                this.trCurrentYearGBTest.Visible = false;
                this.trCurrentYearFIRA.Visible = false;
                this.trCurrentYearOthers.Visible = false;
                this.trCurrentYearGrandTotal.Visible = false;
                #endregion disable summary by claim type
            #endregion Report Summary

            isFirst = true;

            yearlySummaryItemDesc = (isSingleVendor ? this.lbl_Supplier.Text : "Overall");
            repCurrentYearSummary.DataSource = yearlySummaryList;    //htYearlySummary;
            repCurrentYearSummary.DataBind();
            repLastYearSummary.DataSource = yearlySummaryList;      //htYearlySummary;
            repLastYearSummary.DataBind();
            buildClaimRatioSummary();

            if (!isSingleVendor)
            {
                if (!isSummary)
                {
                    //this.repUKClaimSummary.DataSource = list;
                    this.repUKClaimSummary.DataSource = officeSummaryList;
                    this.repUKClaimSummary.DataBind();
                }
            }
            else
            {
                // Show some of the figure in current year
                this.lbl_CurrentYearItem.Text = this.lbl_Supplier.Text;
                this.trCurrentYearTopMargin.Visible = false;
                this.trCurrentYearHeading.Visible = false;

                this.trCurrentYearClaimRatioTopMargin.Visible = false;
                this.trCurrentYearClaimRatioCurrency.Text = "";
                this.trCurrentYearClaimRatioItem.Text = "";
                this.tdCurrentYearClaimRatioItem.Style.Add(HtmlTextWriterStyle.BorderStyle, "none");
                this.tdCurrentYearClaimRatioCurrency.Style.Add(HtmlTextWriterStyle.BorderStyle, "none");
                this.trCurrentYearClaimRatioPaidAmtBySupplier.Visible = false;  // true;
                this.trCurrentYearClaimRatioPaidPercentBySupplier.Visible = false;
                this.trCurrentYearClaimRatioPaidAmtByNS.Visible = false;    // true;
                this.trCurrentYearClaimRatioPaidPercentByNS.Visible = false;
                this.trCurrentYearClaimRatioGrandTotal.Visible = false;

                // Disable Last Year Summary
                this.trLastYearTopMargin.Visible = false;
                this.trLastYearHeading.Visible = false;
                
                this.trLastYearRework.Visible = false;
                this.trLastYearReject.Visible = false;
                this.trLastYearMFRN.Visible = false;
                this.trLastYearCFS.Visible = false;
                this.trLastYearSafetyIssue.Visible = false;
                this.trLastYearAuditFee.Visible = false;
                this.trLastYearFabricTest.Visible = false;
                this.trLastYearPenaltyCharge.Visible = false;
                this.trLastYearQCC.Visible = false;
                this.trLastYearCHB.Visible = false;
                this.trLastYearGBTest.Visible = false;
                this.trLastYearFIRA.Visible = false;
                this.trLastYearOthers.Visible = false;
                this.trLastYearGrandTotal.Visible = false;
                
                repLastYearSummary.Visible = false;

                this.trLastYearClaimRatioTopMargin.Visible = false;
                this.trLastYearClaimRatioPaidAmtBySupplier.Visible = false;
                this.trLastYearClaimRatioPaidPercentBySupplier.Visible = false;
                this.trLastYearClaimRatioPaidAmtByNS.Visible = false;
                this.trLastYearClaimRatioPaidPercentByNS.Visible = false;
                this.trLastYearClaimRatioGrandTotal.Visible = false;
            }
        }

        private List<UKClaimPhasingDef> vwSearchResult
        {
            set
            {
                ViewState["SearchResult"] = value;
            }
            get
            {
                return (List<UKClaimPhasingDef>)ViewState["SearchResult"];
            }
        }

        protected void repUKClaimSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HtmlTableRow tr_Normal_Vendor = (HtmlTableRow)e.Item.FindControl("trNormal_Vendor");
            HtmlTableRow tr_Normal_NS = (HtmlTableRow)e.Item.FindControl("trNormal_NS");
            HtmlTableRow tr_BlankLine = (HtmlTableRow)e.Item.FindControl("trBlankLine");
            HtmlTableRow tr_Summary = (HtmlTableRow)e.Item.FindControl("trSummary");

            if (WebHelper.isRepeaterHeader(e))
            {
                ((Label)e.Item.FindControl("lbl_GroupHeading")).Text = "Office Summary";
            }
            else if (WebHelper.isRepeaterFooter(e))
            {
                #region Office Summary - Grand total
                    tr_Summary.Visible = true;
                    UKClaimPhasingRef rf = (UKClaimPhasingRef)officeSummaryList[0];
                    
                    UKClaimPhasingDef def = rf.Def;
                    decimal[, ,] officeArray = rf.Array;
                    for (int i = 0; i < 13; i++)
                    {
                        ((HtmlTableCell)e.Item.FindControl("tdVendorSmy" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Attributes["class"] = "subTotalAlt";
                        ((Label)e.Item.FindControl("lbl_VendorSmy" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = (officeArray[currentYear, paidByVendor, i]).ToString("#,##0");
                        ((HtmlTableCell)e.Item.FindControl("tdNSSmy" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Attributes["class"] = "subTotalAlt";
                        ((Label)e.Item.FindControl("lbl_NSSmy" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = (officeArray[currentYear, paidByNS, i]).ToString("#,##0");
                    }
                    ((HtmlTableCell)e.Item.FindControl("tdVendorDummy1")).Attributes["class"] = "subTotalAlt";
                    ((HtmlTableCell)e.Item.FindControl("tdVendorDummy2")).Attributes["class"] = "subTotalAlt";
                    ((HtmlTableCell)e.Item.FindControl("tdVendorPaidBy")).Attributes["class"] = "subTotalAlt";
                    ((HtmlTableCell)e.Item.FindControl("tdVendorSmyDesc")).Attributes["class"] = "subTotalAlt";
                    ((HtmlTableCell)e.Item.FindControl("tdNSDummy1")).Attributes["class"] = "subTotalAlt";
                    ((HtmlTableCell)e.Item.FindControl("tdNSDummy2")).Attributes["class"] = "subTotalAlt";
                    ((HtmlTableCell)e.Item.FindControl("tdNSSmyDesc")).Attributes["class"] = "subTotalAlt";
                    ((HtmlTableCell)e.Item.FindControl("tdNSPaidBy")).Attributes["class"] = "subTotalAlt";
                    ((Label)e.Item.FindControl("lbl_SmyDesc")).Text = "Grand Total (USD)";

                    #endregion Office Summary - Grand total
            }
            else
            {
                tr_Normal_Vendor.Visible = false;
                tr_Normal_NS.Visible = false;

                UKClaimPhasingRef rf = (UKClaimPhasingRef)e.Item.DataItem;
                if (rf.GroupValue != grandTotalGroupName)      // bind to ArrayList
                    if (!isSummary)
                    {   // Detail Report - Group Footer (After calculating total amount)
                        #region Office Group Total
                        UKClaimPhasingDef def = rf.Def;
                        tr_Normal_Vendor.Visible = true;
                        tr_Normal_NS.Visible = true;
                        ((Label)e.Item.FindControl("lbl_Group")).Text = rf.GroupValue;
                        ((Label)e.Item.FindControl("lbl_Currency")).Text = "USD";
                        decimal[, ,] officeArray = rf.Array;
                        for (int i = 0; i < 13; i++)
                        {
                            ((Label)e.Item.FindControl("lbl_" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = officeArray[currentYear, paidByVendor, i].ToString("#,##0");
                            ((Label)e.Item.FindControl("lbl_NS" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = officeArray[currentYear, paidByNS, i].ToString("#,##0");
                        }
                        #endregion Office SubTotal
                    }
            }
        }
   

        protected void repUKClaim_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HtmlTableRow tr_SubTotal = (HtmlTableRow)e.Item.FindControl("trSubTotal");
            HtmlTableRow tr_SubTotalVendor = (HtmlTableRow)e.Item.FindControl("trSubTotal_Vendor");
            HtmlTableRow tr_SubTotalNS = (HtmlTableRow)e.Item.FindControl("trSubTotal_NS");
            HtmlTableRow tr_Office = (HtmlTableRow)e.Item.FindControl("trOffice");
            HtmlTableRow tr_VendorGroupFooter = (HtmlTableRow)e.Item.FindControl("trVendorGroupFooter");
            HtmlTableRow tr_NSGroupFooter = (HtmlTableRow)e.Item.FindControl("trNSGroupFooter");
            HtmlTableRow tr_BlankLine = (HtmlTableRow)e.Item.FindControl("trBlankLine");
            HtmlTableRow tr_SubGroupBlankLine = (HtmlTableRow)e.Item.FindControl("trSubGroupBlankLine");
            OfficeId currentOffice = null;

            if (WebHelper.isRepeaterFooter(e))
            {
                #region Report Footer
                HtmlTableRow tr_Footer = (HtmlTableRow)e.Item.FindControl("trFooter");
                if (isSummary)
                    tr_Footer.Visible = false;
                else
                    tr_Footer.Visible = false;
                #endregion
            }
            else
            {
                UKClaimPhasingDef def = (UKClaimPhasingDef)vwSearchResult[e.Item.ItemIndex];
                UKClaimPhasingDef nextDef = (e.Item.ItemIndex + 1 < vwSearchResult.Count ? (UKClaimPhasingDef)vwSearchResult[e.Item.ItemIndex + 1] : null);
                ((Label)e.Item.FindControl("lbl_Office")).Text = OfficeId.getName(def.OfficeId);
                tr_Office.Visible = false;
                tr_BlankLine.Visible = false;
                tr_SubGroupBlankLine.Visible = false;

                currentOffice = OfficeId.getStatus(def.OfficeId);
                if (isFirst)
                {
                    vendorId = def.VendorId;
                    currencyId = def.CurrencyId;
                }

                if (def.OfficeId != officeId)
                {
                    tr_Office.Visible = true;
                    tr_SubGroupBlankLine.Visible = (officeId > 0);
                }

                supplierName = def.Name;
                currencyName = CurrencyId.getName(def.CurrencyId);
                shipmentDate = def.LatestShipmentDate;
                if (count == 0)
                {
                    ((Label)e.Item.FindControl("lbl_Supplier")).Text = def.Name;
                    ((Label)e.Item.FindControl("lbl_Currency")).Text = CurrencyId.getName(def.CurrencyId);
                }

                HtmlTableRow tr_Normal_Vendor = (HtmlTableRow)e.Item.FindControl("trNormal_Vendor");
                HtmlTableRow tr_Normal_NS = (HtmlTableRow)e.Item.FindControl("trNormal_NS");
                tr_Normal_NS.Visible = false;
                tr_Normal_Vendor.Visible = false;


                decimal[,] detail = new decimal[2, 13];
                decimal[,] detailUSD = new decimal[2, 13];
                #region put detail into array
                detail[paidByVendor, 0] = Decimal.Round( def.TotalVendorAmount);
                detail[paidByVendor, 1] = Decimal.Round( def.P01VendorAmount);
                detail[paidByVendor, 2] = Decimal.Round( def.P02VendorAmount);
                detail[paidByVendor, 3] = Decimal.Round( def.P03VendorAmount);
                detail[paidByVendor, 4] = Decimal.Round( def.P04VendorAmount);
                detail[paidByVendor, 5] = Decimal.Round( def.P05VendorAmount);
                detail[paidByVendor, 6] = Decimal.Round( def.P06VendorAmount);
                detail[paidByVendor, 7] = Decimal.Round( def.P07VendorAmount);
                detail[paidByVendor, 8] = Decimal.Round( def.P08VendorAmount);
                detail[paidByVendor, 9] = Decimal.Round( def.P09VendorAmount);
                detail[paidByVendor, 10] = Decimal.Round( def.P10VendorAmount);
                detail[paidByVendor, 11] = Decimal.Round( def.P11VendorAmount);
                detail[paidByVendor, 12] = Decimal.Round( def.P12VendorAmount);

                detail[paidByNS, 0] = Decimal.Round( def.TotalNSAmount);
                detail[paidByNS, 1] = Decimal.Round( def.P01NSAmount);
                detail[paidByNS, 2] = Decimal.Round( def.P02NSAmount);
                detail[paidByNS, 3] = Decimal.Round( def.P03NSAmount);
                detail[paidByNS, 4] = Decimal.Round( def.P04NSAmount);
                detail[paidByNS, 5] = Decimal.Round( def.P05NSAmount);
                detail[paidByNS, 6] = Decimal.Round( def.P06NSAmount);
                detail[paidByNS, 7] = Decimal.Round( def.P07NSAmount);
                detail[paidByNS, 8] = Decimal.Round( def.P08NSAmount);
                detail[paidByNS, 9] = Decimal.Round( def.P09NSAmount);
                detail[paidByNS, 10] = Decimal.Round( def.P10NSAmount);
                detail[paidByNS, 11] = Decimal.Round( def.P11NSAmount);
                detail[paidByNS, 12] = Decimal.Round( def.P12NSAmount);

                detailUSD[paidByVendor, 0] = Decimal.Round(def.TotalVendorAmountInUSD);
                detailUSD[paidByVendor, 1] = Decimal.Round(def.P01VendorAmountInUSD);
                detailUSD[paidByVendor, 2] = Decimal.Round(def.P02VendorAmountInUSD);
                detailUSD[paidByVendor, 3] = Decimal.Round(def.P03VendorAmountInUSD);
                detailUSD[paidByVendor, 4] = Decimal.Round(def.P04VendorAmountInUSD);
                detailUSD[paidByVendor, 5] = Decimal.Round(def.P05VendorAmountInUSD);
                detailUSD[paidByVendor, 6] = Decimal.Round(def.P06VendorAmountInUSD);
                detailUSD[paidByVendor, 7] = Decimal.Round(def.P07VendorAmountInUSD);
                detailUSD[paidByVendor, 8] = Decimal.Round(def.P08VendorAmountInUSD);
                detailUSD[paidByVendor, 9] = Decimal.Round(def.P09VendorAmountInUSD);
                detailUSD[paidByVendor, 10] = Decimal.Round(def.P10VendorAmountInUSD);
                detailUSD[paidByVendor, 11] = Decimal.Round(def.P11VendorAmountInUSD);
                detailUSD[paidByVendor, 12] = Decimal.Round(def.P12VendorAmountInUSD);

                detailUSD[paidByNS, 0] = Decimal.Round(def.TotalNSAmountInUSD);
                detailUSD[paidByNS, 1] = Decimal.Round(def.P01NSAmountInUSD);
                detailUSD[paidByNS, 2] = Decimal.Round(def.P02NSAmountInUSD);
                detailUSD[paidByNS, 3] = Decimal.Round(def.P03NSAmountInUSD);
                detailUSD[paidByNS, 4] = Decimal.Round(def.P04NSAmountInUSD);
                detailUSD[paidByNS, 5] = Decimal.Round(def.P05NSAmountInUSD);
                detailUSD[paidByNS, 6] = Decimal.Round(def.P06NSAmountInUSD);
                detailUSD[paidByNS, 7] = Decimal.Round(def.P07NSAmountInUSD);
                detailUSD[paidByNS, 8] = Decimal.Round(def.P08NSAmountInUSD);
                detailUSD[paidByNS, 9] = Decimal.Round(def.P09NSAmountInUSD);
                detailUSD[paidByNS, 10] = Decimal.Round(def.P10NSAmountInUSD);
                detailUSD[paidByNS, 11] = Decimal.Round(def.P11NSAmountInUSD);
                detailUSD[paidByNS, 12] = Decimal.Round(def.P12NSAmountInUSD);
                #endregion put detail into array

                if (!isSummary)
                {   // Detail Report - Group Header
                    #region Supplier/Currency SubTotal
                    string backgroundClass = "locked" + (isAlternate ? "Alt" : string.Empty);

                    tr_Normal_Vendor.Visible = true;
                    ((HtmlTableCell)e.Item.FindControl("tdSupplier")).Attributes["class"] = backgroundClass;
                    ((HtmlTableCell)e.Item.FindControl("tdCurrency")).Attributes["class"] = backgroundClass;
                    ((HtmlTableCell)e.Item.FindControl("tdGroupName")).Attributes["class"] = backgroundClass;
                    ((HtmlTableCell)e.Item.FindControl("tdPaidBy")).Attributes["class"] = backgroundClass;
                    ((HtmlTableCell)e.Item.FindControl("tdShipmentDate")).Attributes["class"] = backgroundClass;
                    tr_Normal_NS.Visible = true;
                    ((HtmlTableCell)e.Item.FindControl("tdNSSupplier")).Attributes["class"] = backgroundClass;
                    ((HtmlTableCell)e.Item.FindControl("tdNSCurrency")).Attributes["class"] = backgroundClass;
                    ((HtmlTableCell)e.Item.FindControl("tdNSGroupName")).Attributes["class"] = backgroundClass;
                    ((HtmlTableCell)e.Item.FindControl("tdNSPaidBy")).Attributes["class"] = backgroundClass;
                    ((HtmlTableCell)e.Item.FindControl("tdNSShipmentDate")).Attributes["class"] = backgroundClass;

                    for (int i = 0; i < 13; i++)
                    {
                        ((HtmlTableCell)e.Item.FindControl("td" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Attributes["class"] = backgroundClass;
                        ((HtmlTableCell)e.Item.FindControl("tdNS" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Attributes["class"] = backgroundClass;
                        ((Label)e.Item.FindControl("lbl_" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = detail[paidByVendor, i].ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_NS_" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = detail[paidByNS, i].ToString("#,##0");
                    }
                    ((Label)e.Item.FindControl("lbl_GroupName")).Text = (param_ReportType == "1" ? UKClaimType.getType(def.ClaimTypeId).Name : def.ClaimReason);
                    if (def.LatestShipmentDate == DateTime.MinValue)
                        ((Label)e.Item.FindControl("lbl_ShipmentDate")).Text = "N/A";
                    else
                        ((Label)e.Item.FindControl("lbl_ShipmentDate")).Text = DateTimeUtility.getDateString(def.LatestShipmentDate);
                    #endregion Supplier/Currency SubTotal
                }

                #region Calculate Supplier/Currency & Office SubTotal
                for (int i = 0; i <= paidByVendor; i++)
                    for(int j=0;j<13;j++)
                {
                    ttlSub[i, j] += detailUSD[i, j];
                    ttlOfficeSub[i, j] += detailUSD[i, j];
                }
                #endregion Calculate Supplier/Currency & Office SubTotal
                count += 1;

                tr_SubTotal.Visible = false;
                bool subGroupEnd = true;
                if (nextDef != null)
                    subGroupEnd = (def.VendorId != nextDef.VendorId || def.CurrencyId != nextDef.CurrencyId);
                if (subGroupEnd)
                //if ((def.VendorId != vendorId || def.CurrencyId != currencyId))
                {
                    #region Supplier/Currency SubTotal
                    if (isSummary)
                    {
                        #region Supplier subtotal - Supplier & NS
                        tr_SubTotalVendor.Visible = true;
                        tr_SubTotalNS.Visible = true;
                        ((HtmlTableCell)e.Item.FindControl("tdVendorSubSupplier")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdVendorSubCurrency")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdVendorSubGroupName")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdVendorSubPaidBy")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdVendorSubShipmentDate")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdNSSubSupplier")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdNSSubCurrency")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdNSSubGroupName")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdNSSubPaidBy")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                        ((HtmlTableCell)e.Item.FindControl("tdNSSubShipmentDate")).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                            
                        ((Label)e.Item.FindControl("lbl_VendorSubSupplier")).Text = supplierName;
                        ((Label)e.Item.FindControl("lbl_VendorSubCurrency")).Text = currencyName;
                        if (shipmentDate == DateTime.MinValue)
                            ((Label)e.Item.FindControl("lbl_VendorSubShipmentDate")).Text = "N/A";
                        else
                            ((Label)e.Item.FindControl("lbl_VendorSubShipmentDate")).Text = DateTimeUtility.getDateString(shipmentDate);
                        for (int i = 0; i < 13; i++)
                        {
                            ((HtmlTableCell)e.Item.FindControl("tdVendorSub" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                            ((Label)e.Item.FindControl("lbl_VendorSub" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = ttlSub[paidByVendor, i].ToString("#,##0");
                            ((HtmlTableCell)e.Item.FindControl("tdNSSub" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Attributes["class"] = "subTotal" + (isAlternate ? "Alt" : string.Empty);
                            ((Label)e.Item.FindControl("lbl_NSSub" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = ttlSub[paidByNS, i].ToString("#,##0");
                        }
                        #endregion Supplier subtotal - Supplier & NS
                    }

                    isAlternate = !isAlternate;
                    // Reset subtotal
                    for (int i = 0; i < 13; i++)
                        ttlSub[paidByVendor, i] = ttlSub[paidByNS, i] = 0;
                    count = 0;
                    #endregion Supplier/Currency SubTotal
                }

                // Detail Report - Group Footer (After calculating total amount)
                #region Office SubTotal
                bool groupEnd = true;

                if (nextDef != null)
                    groupEnd = (nextDef.OfficeId != def.OfficeId);
                if (groupEnd)
                {
                    tr_VendorGroupFooter.Visible = true;
                    tr_NSGroupFooter.Visible = true;
                    ((Label)e.Item.FindControl("lbl_GfDesc")).Text = OfficeId.getName(def.OfficeId).ToString() + " Subtotal (USD):";
                    for (int i = 0; i < 13; i++)
                    {
                        ((Label)e.Item.FindControl("lbl_VendorGf" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = ttlOfficeSub[paidByVendor, i].ToString("#,##0");
                        ((Label)e.Item.FindControl("lbl_NSGf" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = ttlOfficeSub[paidByNS, i].ToString("#,##0");
                        ttlOfficeSub[paidByVendor, i] = 0;
                        ttlOfficeSub[paidByNS, i] = 0;
                    }
                }
                #endregion Office SubTotal
                

                if (isSingleVendor)
                {
                    HtmlTableRow tr_GroupHeading = (HtmlTableRow)e.Item.FindControl("trGroupHeading");
                    HtmlTableRow tr_Summary = (HtmlTableRow)e.Item.FindControl("trSummary");
                    HtmlTableRow tr_BlankHeading = (HtmlTableRow)e.Item.FindControl("trBlankHeading");
                    HtmlTableRow tr_CurrentYearHeading = (HtmlTableRow)e.Item.FindControl("trCurrentYearHeading");

                    tr_SubTotal.Visible = false;
                    tr_SubTotalVendor.Visible = false;
                    tr_SubTotalNS.Visible = false;
                    tr_Normal_Vendor.Visible = false;
                    tr_Normal_NS.Visible = false;
                    tr_Office.Visible = false;
                    tr_VendorGroupFooter.Visible = false;
                    tr_NSGroupFooter.Visible = false;
                    tr_BlankLine.Visible = false;
                }
                vendorId = def.VendorId;
                currencyId = def.CurrencyId;
                officeId = def.OfficeId;
                isFirst = false;
            }
        }

        private class UKClaimPhasingRef : DomainData
        {
            decimal[, ,] array;
            UKClaimPhasingDef def;
            public UKClaimPhasingRef()
            {
                array = new decimal[2, 2, 13];
                def = new UKClaimPhasingDef();
            }
            public string GroupValue { get; set; }
            public UKClaimPhasingDef Def { set { def = value; } get { return def; } }
            public decimal[, ,] Array { set { array = value; } get { return array; } }
        }
        private string grandTotalGroupName = "GrandTotal[OnlyShowAtFooter]";


        protected ArrayList getUKClaimGroupSummary(string groupName, int sortByGroupValue)
        {
            UKClaimPhasingRef grandTotalRef = new UKClaimPhasingRef();
            grandTotalRef.GroupValue = grandTotalGroupName;
            grandTotalRef.Def.CurrencyId = -1;
            grandTotalRef.Def.VendorId = -1;
            grandTotalRef.Def.OfficeId = -1;
            Hashtable htGroupList = new Hashtable();
            foreach (UKClaimPhasingDef rawPhasing in vwSearchResult)
            {
                UKClaimPhasingRef rf = null;
                decimal[, ,] groupArray;
                string groupValue = string.Empty;
                object groupKey = null;
                switch (groupName)
                {
                    case claimType: groupKey = rawPhasing.ClaimTypeId; break;
                    case claimReason: groupKey = rawPhasing.ClaimReason; break;
                    case claimOffice: groupKey = rawPhasing.OfficeId; break;
                    default: groupKey = grandTotalGroupName; rf = grandTotalRef; break;
                }
                if (rf == null)
                {
                    if (htGroupList.ContainsKey(groupKey))
                        rf = ((UKClaimPhasingRef)htGroupList[groupKey]);
                    else
                    {
                        htGroupList.Add(groupKey, rf = new UKClaimPhasingRef());
                        switch (groupName)
                        {
                            case claimType: rf.GroupValue = UKClaimType.getType(rawPhasing.ClaimTypeId).Name; break;
                            case claimReason: rf.GroupValue = rawPhasing.ClaimReason; break;
                            case claimOffice: rf.GroupValue = CommonUtil.getOfficeRefByKey(rawPhasing.OfficeId).Description; break;
                            default: rf.GroupValue = grandTotalGroupName; break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(rf.GroupValue))
                    switch (groupName)
                    {
                        case claimType: rf.GroupValue = UKClaimType.getType(rawPhasing.ClaimTypeId).Name; break;
                        case claimReason: rf.GroupValue = rawPhasing.ClaimReason; break;
                        case claimOffice: rf.GroupValue = CommonUtil.getOfficeRefByKey(rawPhasing.OfficeId).Description; break;
                        default: rf.GroupValue = grandTotalGroupName; break;
                    }

                UKClaimPhasingDef groupDef = rf.Def;
                groupArray = rf.Array;

                #region store group total in array
                groupArray[lastYear, paidByNS, 0] += Decimal.Round(rawPhasing.LYTotalNSAmountInUSD);
                groupArray[lastYear, paidByNS, 1] += Decimal.Round(rawPhasing.LYP01NSAmountInUSD);
                groupArray[lastYear, paidByNS, 2] += Decimal.Round(rawPhasing.LYP02NSAmountInUSD);
                groupArray[lastYear, paidByNS, 3] += Decimal.Round(rawPhasing.LYP03NSAmountInUSD);
                groupArray[lastYear, paidByNS, 4] += Decimal.Round(rawPhasing.LYP04NSAmountInUSD);
                groupArray[lastYear, paidByNS, 5] += Decimal.Round(rawPhasing.LYP05NSAmountInUSD);
                groupArray[lastYear, paidByNS, 6] += Decimal.Round(rawPhasing.LYP06NSAmountInUSD);
                groupArray[lastYear, paidByNS, 7] += Decimal.Round(rawPhasing.LYP07NSAmountInUSD);
                groupArray[lastYear, paidByNS, 8] += Decimal.Round(rawPhasing.LYP08NSAmountInUSD);
                groupArray[lastYear, paidByNS, 9] += Decimal.Round(rawPhasing.LYP09NSAmountInUSD);
                groupArray[lastYear, paidByNS, 10] += Decimal.Round(rawPhasing.LYP10NSAmountInUSD);
                groupArray[lastYear, paidByNS, 11] += Decimal.Round(rawPhasing.LYP11NSAmountInUSD);
                groupArray[lastYear, paidByNS, 12] += Decimal.Round(rawPhasing.LYP12NSAmountInUSD);

                groupArray[lastYear, paidByVendor, 0] += Decimal.Round(rawPhasing.LYTotalVendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 1] += Decimal.Round(rawPhasing.LYP01VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 2] += Decimal.Round(rawPhasing.LYP02VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 3] += Decimal.Round(rawPhasing.LYP03VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 4] += Decimal.Round(rawPhasing.LYP04VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 5] += Decimal.Round(rawPhasing.LYP05VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 6] += Decimal.Round(rawPhasing.LYP06VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 7] += Decimal.Round(rawPhasing.LYP07VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 8] += Decimal.Round(rawPhasing.LYP08VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 9] += Decimal.Round(rawPhasing.LYP09VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 10] += Decimal.Round(rawPhasing.LYP10VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 11] += Decimal.Round(rawPhasing.LYP11VendorAmountInUSD);
                groupArray[lastYear, paidByVendor, 12] += Decimal.Round(rawPhasing.LYP12VendorAmountInUSD);

                groupArray[currentYear, paidByNS, 0] += Decimal.Round(rawPhasing.TotalNSAmountInUSD);
                groupArray[currentYear, paidByNS, 1] += Decimal.Round(rawPhasing.P01NSAmountInUSD);
                groupArray[currentYear, paidByNS, 2] += Decimal.Round(rawPhasing.P02NSAmountInUSD);
                groupArray[currentYear, paidByNS, 3] += Decimal.Round(rawPhasing.P03NSAmountInUSD);
                groupArray[currentYear, paidByNS, 4] += Decimal.Round(rawPhasing.P04NSAmountInUSD);
                groupArray[currentYear, paidByNS, 5] += Decimal.Round(rawPhasing.P05NSAmountInUSD);
                groupArray[currentYear, paidByNS, 6] += Decimal.Round(rawPhasing.P06NSAmountInUSD);
                groupArray[currentYear, paidByNS, 7] += Decimal.Round(rawPhasing.P07NSAmountInUSD);
                groupArray[currentYear, paidByNS, 8] += Decimal.Round(rawPhasing.P08NSAmountInUSD);
                groupArray[currentYear, paidByNS, 9] += Decimal.Round(rawPhasing.P09NSAmountInUSD);
                groupArray[currentYear, paidByNS, 10] += Decimal.Round(rawPhasing.P10NSAmountInUSD);
                groupArray[currentYear, paidByNS, 11] += Decimal.Round(rawPhasing.P11NSAmountInUSD);
                groupArray[currentYear, paidByNS, 12] += Decimal.Round(rawPhasing.P12NSAmountInUSD);

                groupArray[currentYear, paidByVendor, 0] += Decimal.Round(rawPhasing.TotalVendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 1] += Decimal.Round(rawPhasing.P01VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 2] += Decimal.Round(rawPhasing.P02VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 3] += Decimal.Round(rawPhasing.P03VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 4] += Decimal.Round(rawPhasing.P04VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 5] += Decimal.Round(rawPhasing.P05VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 6] += Decimal.Round(rawPhasing.P06VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 7] += Decimal.Round(rawPhasing.P07VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 8] += Decimal.Round(rawPhasing.P08VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 9] += Decimal.Round(rawPhasing.P09VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 10] += Decimal.Round(rawPhasing.P10VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 11] += Decimal.Round(rawPhasing.P11VendorAmountInUSD);
                groupArray[currentYear, paidByVendor, 12] += Decimal.Round(rawPhasing.P12VendorAmountInUSD);
                #endregion store group total in array

                #region store total in def
                #region group subtotal
                groupDef.ClaimReason = rawPhasing.ClaimReason;
                groupDef.ClaimTypeId = rawPhasing.ClaimTypeId;
                groupDef.CurrencyId = rawPhasing.CurrencyId;
                groupDef.Name = rawPhasing.Name;
                groupDef.LYTotalNSAmountInUSD += rawPhasing.LYTotalNSAmountInUSD;
                groupDef.LYP01NSAmountInUSD += rawPhasing.LYP01NSAmountInUSD;
                groupDef.LYP02NSAmountInUSD += rawPhasing.LYP02NSAmountInUSD;
                groupDef.LYP03NSAmountInUSD += rawPhasing.LYP03NSAmountInUSD;
                groupDef.LYP04NSAmountInUSD += rawPhasing.LYP04NSAmountInUSD;
                groupDef.LYP05NSAmountInUSD += rawPhasing.LYP05NSAmountInUSD;
                groupDef.LYP06NSAmountInUSD += rawPhasing.LYP06NSAmountInUSD;
                groupDef.LYP07NSAmountInUSD += rawPhasing.LYP07NSAmountInUSD;
                groupDef.LYP08NSAmountInUSD += rawPhasing.LYP08NSAmountInUSD;
                groupDef.LYP09NSAmountInUSD += rawPhasing.LYP09NSAmountInUSD;
                groupDef.LYP10NSAmountInUSD += rawPhasing.LYP10NSAmountInUSD;
                groupDef.LYP11NSAmountInUSD += rawPhasing.LYP11NSAmountInUSD;
                groupDef.LYP12NSAmountInUSD += rawPhasing.LYP12NSAmountInUSD;

                groupDef.LYTotalVendorAmountInUSD += rawPhasing.LYTotalVendorAmountInUSD;
                groupDef.LYP01VendorAmountInUSD += rawPhasing.LYP01VendorAmountInUSD;
                groupDef.LYP02VendorAmountInUSD += rawPhasing.LYP02VendorAmountInUSD;
                groupDef.LYP03VendorAmountInUSD += rawPhasing.LYP03VendorAmountInUSD;
                groupDef.LYP04VendorAmountInUSD += rawPhasing.LYP04VendorAmountInUSD;
                groupDef.LYP05VendorAmountInUSD += rawPhasing.LYP05VendorAmountInUSD;
                groupDef.LYP06VendorAmountInUSD += rawPhasing.LYP06VendorAmountInUSD;
                groupDef.LYP07VendorAmountInUSD += rawPhasing.LYP07VendorAmountInUSD;
                groupDef.LYP08VendorAmountInUSD += rawPhasing.LYP08VendorAmountInUSD;
                groupDef.LYP09VendorAmountInUSD += rawPhasing.LYP09VendorAmountInUSD;
                groupDef.LYP10VendorAmountInUSD += rawPhasing.LYP10VendorAmountInUSD;
                groupDef.LYP11VendorAmountInUSD += rawPhasing.LYP11VendorAmountInUSD;
                groupDef.LYP12VendorAmountInUSD += rawPhasing.LYP12VendorAmountInUSD;

                groupDef.TotalNSAmountInUSD += rawPhasing.TotalNSAmountInUSD;
                groupDef.P01NSAmountInUSD += rawPhasing.P01NSAmountInUSD;
                groupDef.P02NSAmountInUSD += rawPhasing.P02NSAmountInUSD;
                groupDef.P03NSAmountInUSD += rawPhasing.P03NSAmountInUSD;
                groupDef.P04NSAmountInUSD += rawPhasing.P04NSAmountInUSD;
                groupDef.P05NSAmountInUSD += rawPhasing.P05NSAmountInUSD;
                groupDef.P06NSAmountInUSD += rawPhasing.P06NSAmountInUSD;
                groupDef.P07NSAmountInUSD += rawPhasing.P07NSAmountInUSD;
                groupDef.P08NSAmountInUSD += rawPhasing.P08NSAmountInUSD;
                groupDef.P09NSAmountInUSD += rawPhasing.P09NSAmountInUSD;
                groupDef.P10NSAmountInUSD += rawPhasing.P10NSAmountInUSD;
                groupDef.P11NSAmountInUSD += rawPhasing.P11NSAmountInUSD;
                groupDef.P12NSAmountInUSD += rawPhasing.P12NSAmountInUSD;

                groupDef.TotalVendorAmountInUSD += rawPhasing.TotalVendorAmountInUSD;
                groupDef.P01VendorAmountInUSD += rawPhasing.P01VendorAmountInUSD;
                groupDef.P02VendorAmountInUSD += rawPhasing.P02VendorAmountInUSD;
                groupDef.P03VendorAmountInUSD += rawPhasing.P03VendorAmountInUSD;
                groupDef.P04VendorAmountInUSD += rawPhasing.P04VendorAmountInUSD;
                groupDef.P05VendorAmountInUSD += rawPhasing.P05VendorAmountInUSD;
                groupDef.P06VendorAmountInUSD += rawPhasing.P06VendorAmountInUSD;
                groupDef.P07VendorAmountInUSD += rawPhasing.P07VendorAmountInUSD;
                groupDef.P08VendorAmountInUSD += rawPhasing.P08VendorAmountInUSD;
                groupDef.P09VendorAmountInUSD += rawPhasing.P09VendorAmountInUSD;
                groupDef.P10VendorAmountInUSD += rawPhasing.P10VendorAmountInUSD;
                groupDef.P11VendorAmountInUSD += rawPhasing.P11VendorAmountInUSD;
                groupDef.P12VendorAmountInUSD += rawPhasing.P12VendorAmountInUSD;
                #endregion reason subtotal
                #region grand total
                grandTotalRef.Def.LYTotalNSAmountInUSD += rawPhasing.LYTotalNSAmountInUSD;
                grandTotalRef.Def.LYP01NSAmountInUSD += rawPhasing.LYP01NSAmountInUSD;
                grandTotalRef.Def.LYP02NSAmountInUSD += rawPhasing.LYP02NSAmountInUSD;
                grandTotalRef.Def.LYP03NSAmountInUSD += rawPhasing.LYP03NSAmountInUSD;
                grandTotalRef.Def.LYP04NSAmountInUSD += rawPhasing.LYP04NSAmountInUSD;
                grandTotalRef.Def.LYP05NSAmountInUSD += rawPhasing.LYP05NSAmountInUSD;
                grandTotalRef.Def.LYP06NSAmountInUSD += rawPhasing.LYP06NSAmountInUSD;
                grandTotalRef.Def.LYP07NSAmountInUSD += rawPhasing.LYP07NSAmountInUSD;
                grandTotalRef.Def.LYP08NSAmountInUSD += rawPhasing.LYP08NSAmountInUSD;
                grandTotalRef.Def.LYP09NSAmountInUSD += rawPhasing.LYP09NSAmountInUSD;
                grandTotalRef.Def.LYP10NSAmountInUSD += rawPhasing.LYP10NSAmountInUSD;
                grandTotalRef.Def.LYP11NSAmountInUSD += rawPhasing.LYP11NSAmountInUSD;
                grandTotalRef.Def.LYP12NSAmountInUSD += rawPhasing.LYP12NSAmountInUSD;

                grandTotalRef.Def.LYTotalVendorAmountInUSD += rawPhasing.LYTotalVendorAmountInUSD;
                grandTotalRef.Def.LYP01VendorAmountInUSD += rawPhasing.LYP01VendorAmountInUSD;
                grandTotalRef.Def.LYP02VendorAmountInUSD += rawPhasing.LYP02VendorAmountInUSD;
                grandTotalRef.Def.LYP03VendorAmountInUSD += rawPhasing.LYP03VendorAmountInUSD;
                grandTotalRef.Def.LYP04VendorAmountInUSD += rawPhasing.LYP04VendorAmountInUSD;
                grandTotalRef.Def.LYP05VendorAmountInUSD += rawPhasing.LYP05VendorAmountInUSD;
                grandTotalRef.Def.LYP06VendorAmountInUSD += rawPhasing.LYP06VendorAmountInUSD;
                grandTotalRef.Def.LYP07VendorAmountInUSD += rawPhasing.LYP07VendorAmountInUSD;
                grandTotalRef.Def.LYP08VendorAmountInUSD += rawPhasing.LYP08VendorAmountInUSD;
                grandTotalRef.Def.LYP09VendorAmountInUSD += rawPhasing.LYP09VendorAmountInUSD;
                grandTotalRef.Def.LYP10VendorAmountInUSD += rawPhasing.LYP10VendorAmountInUSD;
                grandTotalRef.Def.LYP11VendorAmountInUSD += rawPhasing.LYP11VendorAmountInUSD;
                grandTotalRef.Def.LYP12VendorAmountInUSD += rawPhasing.LYP12VendorAmountInUSD;

                grandTotalRef.Def.TotalNSAmountInUSD += rawPhasing.TotalNSAmountInUSD;
                grandTotalRef.Def.P01NSAmountInUSD += rawPhasing.P01NSAmountInUSD;
                grandTotalRef.Def.P02NSAmountInUSD += rawPhasing.P02NSAmountInUSD;
                grandTotalRef.Def.P03NSAmountInUSD += rawPhasing.P03NSAmountInUSD;
                grandTotalRef.Def.P04NSAmountInUSD += rawPhasing.P04NSAmountInUSD;
                grandTotalRef.Def.P05NSAmountInUSD += rawPhasing.P05NSAmountInUSD;
                grandTotalRef.Def.P06NSAmountInUSD += rawPhasing.P06NSAmountInUSD;
                grandTotalRef.Def.P07NSAmountInUSD += rawPhasing.P07NSAmountInUSD;
                grandTotalRef.Def.P08NSAmountInUSD += rawPhasing.P08NSAmountInUSD;
                grandTotalRef.Def.P09NSAmountInUSD += rawPhasing.P09NSAmountInUSD;
                grandTotalRef.Def.P10NSAmountInUSD += rawPhasing.P10NSAmountInUSD;
                grandTotalRef.Def.P11NSAmountInUSD += rawPhasing.P11NSAmountInUSD;
                grandTotalRef.Def.P12NSAmountInUSD += rawPhasing.P12NSAmountInUSD;

                grandTotalRef.Def.TotalVendorAmountInUSD += rawPhasing.TotalVendorAmountInUSD;
                grandTotalRef.Def.P01VendorAmountInUSD += rawPhasing.P01VendorAmountInUSD;
                grandTotalRef.Def.P02VendorAmountInUSD += rawPhasing.P02VendorAmountInUSD;
                grandTotalRef.Def.P03VendorAmountInUSD += rawPhasing.P03VendorAmountInUSD;
                grandTotalRef.Def.P04VendorAmountInUSD += rawPhasing.P04VendorAmountInUSD;
                grandTotalRef.Def.P05VendorAmountInUSD += rawPhasing.P05VendorAmountInUSD;
                grandTotalRef.Def.P06VendorAmountInUSD += rawPhasing.P06VendorAmountInUSD;
                grandTotalRef.Def.P07VendorAmountInUSD += rawPhasing.P07VendorAmountInUSD;
                grandTotalRef.Def.P08VendorAmountInUSD += rawPhasing.P08VendorAmountInUSD;
                grandTotalRef.Def.P09VendorAmountInUSD += rawPhasing.P09VendorAmountInUSD;
                grandTotalRef.Def.P10VendorAmountInUSD += rawPhasing.P10VendorAmountInUSD;
                grandTotalRef.Def.P11VendorAmountInUSD += rawPhasing.P11VendorAmountInUSD;
                grandTotalRef.Def.P12VendorAmountInUSD += rawPhasing.P12VendorAmountInUSD;
                #endregion grand total
                #endregion store total in def
            }
            #region grand total
            ArrayList groupList = new ArrayList();
            foreach (UKClaimPhasingRef rf in htGroupList.Values)
            {
                for (int i = lastYear; i <= currentYear; i++)
                    for (int j = paidByNS; j <= paidByVendor; j++)
                        for (int k = 0; k < 13; k++)
                            grandTotalRef.Array[i, j, k] += rf.Array[i, j, k];
                groupList.Insert(0, rf);
            }
            htGroupList.Add(grandTotalGroupName, grandTotalRef);
            #endregion grand total

            if (sortByGroupValue==1)
                groupList.Sort(new ArrayListHelper.Sorter("GroupValue"));
            groupList.Insert(0, grandTotalRef);
            return groupList;
        }

        protected void repCurrentYearSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            buildYearlySummary(1,sender, e);
        }

        protected void repLastYearSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            buildYearlySummary(0,sender, e);
        }

        protected void buildYearlySummary(int isCurrentYear, object sender, RepeaterItemEventArgs e)
        {
            string key = string.Empty;
            HtmlTableRow tr_BlankLine = (HtmlTableRow)e.Item.FindControl("trBlankLine");
            HtmlTableRow tr_Normal_Vender = (HtmlTableRow)e.Item.FindControl("trNormal_Vendor");
            HtmlTableRow tr_Normal_NS = (HtmlTableRow)e.Item.FindControl("trNormal_NS");
            HtmlTableRow tr_Footer_Vendor = (HtmlTableRow)e.Item.FindControl("trFooter_Vendor");
            HtmlTableRow tr_Footer_NS = (HtmlTableRow)e.Item.FindControl("trFooter_NS");

            if (WebHelper.isRepeaterHeader(e))
            {
                count = 0;
                isFirst = true;
                isAlternate = false;
            }
            else if (WebHelper.isRepeaterFooter(e))
            {
                #region Report Footer
                tr_Footer_Vendor.Visible = true;
                tr_Footer_NS.Visible = true;
                UKClaimPhasingRef rf = (UKClaimPhasingRef)yearlySummaryList[0];
                UKClaimPhasingDef def = rf.Def;
                ((Label)e.Item.FindControl("lbl_GfDesc")).Text = "Grand Total";
                decimal[, ,] ary = rf.Array;
                for (int i = 0; i < 13; i++)
                {
                    ((Label)e.Item.FindControl("lbl_Gf" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = (ary[isCurrentYear, paidByVendor, i]).ToString("#,##0");
                    ((Label)e.Item.FindControl("lbl_NS_Gf" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = (ary[isCurrentYear, paidByNS, i]).ToString("#,##0");
                }
                #endregion Report Footer
            }
            else if (e.Item.ItemIndex > 0) 
            {
                tr_Normal_Vender.Visible = true;
                tr_Normal_NS.Visible = true;

                UKClaimPhasingRef rf = (UKClaimPhasingRef)e.Item.DataItem;
                UKClaimPhasingDef def = rf.Def;

                if (isFirst)
                {
                    ((Label)e.Item.FindControl("lbl_Desc")).Text = yearlySummaryItemDesc;
                    ((Label)e.Item.FindControl("lbl_Desc")).Style.Add(HtmlTextWriterStyle.FontWeight, "bold");
                    ((Label)e.Item.FindControl("lbl_Currency")).Text = "USD";
                }
                else
                {   // Last Year
                    ((HtmlTableCell)e.Item.FindControl("tdDesc")).Attributes["style"] = "border-right:none;border-bottom:none;";
                    ((HtmlTableCell)e.Item.FindControl("tdCurrency")).Attributes["style"] = "border-left:none;border-bottom:none;";
                    ((HtmlTableCell)e.Item.FindControl("tdNSDesc")).Attributes["style"] = "border-right:none;border-top:none;";
                    ((HtmlTableCell)e.Item.FindControl("tdNSCurrency")).Attributes["style"] = "border-left:none;border-top:none;";
                }

                #region Supplier/Currency SubTotal
                ((HtmlTableCell)e.Item.FindControl("tdGroupName")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                ((HtmlTableCell)e.Item.FindControl("tdPaidBy")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                ((HtmlTableCell)e.Item.FindControl("tdNSGroupName")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                ((HtmlTableCell)e.Item.FindControl("tdNSPaidBy")).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);

                decimal[, ,] ary = rf.Array;
                ((Label)e.Item.FindControl("lbl_GroupName")).Text = rf.GroupValue;//(param_ReportType == "1" ? UKClaimType.getType(def.ClaimTypeId).Name : def.ClaimReason);
                for (int i = 0; i < 13; i++)
                {
                    ((HtmlTableCell)e.Item.FindControl("td" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((Label)e.Item.FindControl("lbl_" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = (ary[isCurrentYear, paidByVendor, i]).ToString("#,##0");
                    ((HtmlTableCell)e.Item.FindControl("tdNS" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Attributes["class"] = "locked" + (isAlternate ? "Alt" : string.Empty);
                    ((Label)e.Item.FindControl("lbl_NS_" + (i == 0 ? "Total" : "P" + i.ToString("00")))).Text = (ary[isCurrentYear, paidByNS, i]).ToString("#,##0");
                }
                #endregion Supplier/Currency SubTotal
                isAlternate = !isAlternate;

                count++;
                isFirst = false;
            }
        }

        protected void buildClaimRatioSummary()
        {
            decimal[, ,] array = ((UKClaimPhasingRef)yearlySummaryList[0]).Array;
            int i;
            for (i = 0; i < 13; i++)
            {
                ((Label)trCurrentYearClaimRatioPaidAmtBySupplier.FindControl("lbl_" + (i == 0 ? "Total" : "P" + i.ToString()) + "PaidBySupplierAmt")).Text = array[1, 1, i].ToString("#,##0");
                ((Label)trCurrentYearClaimRatioPaidPercentBySupplier.FindControl("lbl_" + (i == 0 ? "Total" : "P" + i.ToString()) + "PaidBySupplierPct")).Text = ((array[1, 1, i] + array[1, 0, i]) == 0 ? "N/A" : (100 * array[1, 1, i] / (array[1, 1, i] + array[1, 0, i])).ToString("#,##0.00"));
                ((Label)trCurrentYearClaimRatioPaidAmtByNS.FindControl("lbl_" + (i == 0 ? "Total" : "P" + i.ToString()) + "PaidByNSAmt")).Text = array[1, 0, i].ToString("#,##0");
                ((Label)trCurrentYearClaimRatioPaidPercentByNS.FindControl("lbl_" + (i == 0 ? "Total" : "P" + i.ToString()) + "PaidByNSPct")).Text = ((array[1, 1, i] + array[1, 0, i]) == 0 ? "N/A" : (array[1, 0, i] / (array[1, 1, i] + array[1, 0, i]) * 100).ToString("#,##0.00"));
                ((Label)trCurrentYearClaimRatioGrandTotal.FindControl("lbl_GrandRatio" + (i == 0 ? "Total" : "P" + i.ToString()))).Text = (array[1, 1, i] + array[1, 0, i]).ToString("#,##0");

                ((Label)trLastYearClaimRatioPaidAmtBySupplier.FindControl("lbl_LY_" + (i == 0 ? "Total" : "P" + i.ToString()) + "PaidBySupplierAmt")).Text = array[0, 1, i].ToString("#,##0");
                ((Label)trLastYearClaimRatioPaidPercentBySupplier.FindControl("lbl_LY_" + (i == 0 ? "Total" : "P" + i.ToString()) + "PaidBySupplierPct")).Text = ((array[0, 1, i] + array[0, 0, i]) == 0 ? "N/A" : (100 * array[0, 1, i] / (array[0, 1, i] + array[0, 0, i])).ToString("#,##0.00"));
                ((Label)trLastYearClaimRatioPaidAmtByNS.FindControl("lbl_LY_" + (i == 0 ? "Total" : "P" + i.ToString()) + "PaidByNSAmt")).Text = array[0, 0, i].ToString("#,##0");
                ((Label)trLastYearClaimRatioPaidPercentByNS.FindControl("lbl_LY_" + (i == 0 ? "Total" : "P" + i.ToString()) + "PaidByNSPct")).Text = ((array[0, 1, i] + array[0, 0, i]) == 0 ? "N/A" : (array[0, 0, i] / (array[0, 1, i] + array[0, 0, i]) * 100).ToString("#,##0.00"));
                ((Label)trLastYearClaimRatioGrandTotal.FindControl("lbl_LY_GrandRatio" + (i == 0 ? "Total" : "P" + i.ToString()))).Text = (array[0, 1, i] + array[0, 0, i]).ToString("#,##0");
            }
        }

    }
}