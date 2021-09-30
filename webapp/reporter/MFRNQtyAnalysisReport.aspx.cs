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
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.dataserver;

namespace com.next.isam.webapp.reporter
{
    public partial class MFRNQtyAnalysisReport : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public struct MIME_Type
        {
            public const string Excel = "application/vnd.ms-excel";
            public const string Word = "application/vnd.ms-word";
            public const string PowerPoint = "application/vnd.ms-powerpoint";
        }

        bool isFirst = true;

        private int totalN1 = 0;
        private int totalN2 = 0;
        private int totalN3 = 0;
        private int totalN4 = 0;
        private int totalN5 = 0;
        private int totalN6 = 0;
        private int totalN7 = 0;
        private int totalN8 = 0;
        private int totalN9 = 0;
        private int totalN10 = 0;
        private int totalN11 = 0;
        private int totalN12 = 0;
        private int totalN13 = 0;
        private int totalN14 = 0;
        private int totalN15 = 0;
        private int totalN16 = 0;
        private int totalN17 = 0;
        private int totalN18 = 0;
        private int totalN19 = 0;
        private int totalN20 = 0;
        private int totalN21 = 0;
        private int totalN22 = 0;
        private int totalN23 = 0;
        private int totalN24 = 0;
        private int totalN25 = 0;
        private int totalN26 = 0;
        private int totalN31 = 0;
        private int totalN36 = 0;
        private int totalN41 = 0;
        private int totalN46 = 0;
        private int totalN51 = 0;
        private int totalNALL = 0;
        private int totalNF = 0;
        private int fiscalYear;
        private int period;

        private UKClaimMFRNQtyAnalysisReportDs reportData; 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.EnableViewState = false;
                Response.Clear();
                Response.Charset = "";

                Response.Buffer = true;
                Response.ContentType = MIME_Type.Excel;
                Response.AddHeader("Content-Disposition", "attachment;filename=MFRNQtyAnalysis.xls");

                if (string.IsNullOrEmpty(Request.Params["reportCode"]))
                    tr_ReportCode.Style.Add("display", "none");
                else
                    lbl_ReportCode.Text = "Report Code : " + Request.Params["reportCode"];
                /*
                if (string.IsNullOrEmpty(lbl_ReportCode.Text))
                    tr_ReportCode.Attributes.Add("Display","none");
                */

                this.lblPrintTime.Text = "Print Date: " + DateTime.Today.ToShortDateString() + "  " + DateTime.Now.ToShortTimeString();

                int userId = 0;
                if (Request.QueryString.Get("userId") != null)
                    userId = int.Parse(Request.QueryString.Get("userId"));
                else
                    userId = userId = this.LogonUserId;
                this.lblPrintUser.Text = "Print By: " + CommonUtil.getUserByKey(userId).DisplayName;

                this.getReport();
            }
        }

        private void getReport()
        {

            UKClaimMFRNQtyAnalysisReportDs ds = (UKClaimMFRNQtyAnalysisReportDs)Context.Items[AccountCommander.Param.mfrnQtyAnalysisList];
            if (ds == null)
            {
                int fiscalYear = int.Parse(Request.QueryString.Get("fiscalYear"));
                int periodFrom = int.Parse(Request.QueryString.Get("pFrom"));
                int periodTo = int.Parse(Request.QueryString.Get("pTo"));

                ds = ReporterWorker.Instance.getUKClaimMFRNQtyAnalysisReportList(fiscalYear, periodFrom, periodTo);
            }

            reportData = ds;
            this.repUKClaim.DataSource = ds;
            this.repUKClaim.DataBind();
        }

        protected void repUKClaim_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HtmlTableRow tr_Normal = (HtmlTableRow)e.Item.FindControl("trNormal");
            HtmlTableRow tr_Empty = (HtmlTableRow)e.Item.FindControl("trEmpty");
            HtmlTableRow tr_Total = (HtmlTableRow)e.Item.FindControl("trTotal");
            HtmlTableRow tr_HeaderOne = (HtmlTableRow)e.Item.FindControl("trHeaderOne");
            HtmlTableRow tr_HeaderTwo = (HtmlTableRow)e.Item.FindControl("trHeaderTwo");

            if (WebHelper.isRepeaterFooter(e))
            {
                HtmlTableRow tr_Footer = (HtmlTableRow)e.Item.FindControl("trFooter");
                ((Label)e.Item.FindControl("lblN1Total")).Text = totalN1.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN2Total")).Text = totalN2.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN3Total")).Text = totalN3.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN4Total")).Text = totalN4.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN5Total")).Text = totalN5.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN6Total")).Text = totalN6.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN7Total")).Text = totalN7.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN8Total")).Text = totalN8.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN9Total")).Text = totalN9.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN10Total")).Text = totalN10.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN11Total")).Text = totalN11.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN12Total")).Text = totalN12.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN13Total")).Text = totalN13.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN14Total")).Text = totalN14.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN15Total")).Text = totalN15.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN16Total")).Text = totalN16.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN17Total")).Text = totalN17.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN18Total")).Text = totalN18.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN19Total")).Text = totalN19.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN20Total")).Text = totalN20.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN21Total")).Text = totalN21.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN22Total")).Text = totalN22.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN23Total")).Text = totalN23.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN24Total")).Text = totalN24.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN25Total")).Text = totalN25.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN26Total")).Text = totalN26.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN31Total")).Text = totalN31.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN36Total")).Text = totalN36.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN41Total")).Text = totalN41.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN46Total")).Text = totalN46.ToString("#,##0");
                ((Label)e.Item.FindControl("lblN51Total")).Text = totalN51.ToString("#,##0");
                ((Label)e.Item.FindControl("lblNALLTotal")).Text = totalNALL.ToString("#,##0");
                ((Label)e.Item.FindControl("lblNFTotal")).Text = totalNF.ToString("#,##0");

            }
            else if (WebHelper.isRepeaterNormalItemType(e))
            {
                UKClaimMFRNQtyAnalysisReportDs.UKClaimMFRNQtyAnalysisReportRow r = reportData.UKClaimMFRNQtyAnalysisReport[e.Item.ItemIndex];

                tr_Empty.Visible = false;
                tr_Total.Visible = false;
                tr_HeaderOne.Visible = false;
                tr_HeaderTwo.Visible = false;

                if (isFirst)
                {
                    fiscalYear = r.BudgetYear;
                    period = r.Period;
                    tr_Empty.Visible = true; 
                    tr_HeaderOne.Visible = true;
                    tr_HeaderTwo.Visible = true;
                    ((Label)e.Item.FindControl("lblPeriod")).Text = "P" + r.Period.ToString() + " " + r.BudgetYear.ToString();
                }
                if (r.BudgetYear != fiscalYear || r.Period != period)
                {
                    tr_Empty.Visible = true;
                    tr_Total.Visible = true;
                    tr_HeaderOne.Visible = true;
                    tr_HeaderTwo.Visible = true;
                    ((Label)e.Item.FindControl("lblPeriod")).Text = "P" + r.Period.ToString() + " " + r.BudgetYear.ToString();
                    ((Label)e.Item.FindControl("lblN1Total")).Text = totalN1.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN2Total")).Text = totalN2.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN3Total")).Text = totalN3.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN4Total")).Text = totalN4.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN5Total")).Text = totalN5.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN6Total")).Text = totalN6.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN7Total")).Text = totalN7.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN8Total")).Text = totalN8.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN9Total")).Text = totalN9.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN10Total")).Text = totalN10.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN11Total")).Text = totalN11.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN12Total")).Text = totalN12.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN13Total")).Text = totalN13.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN14Total")).Text = totalN14.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN15Total")).Text = totalN15.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN16Total")).Text = totalN16.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN17Total")).Text = totalN17.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN18Total")).Text = totalN18.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN19Total")).Text = totalN19.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN20Total")).Text = totalN20.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN21Total")).Text = totalN21.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN22Total")).Text = totalN22.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN23Total")).Text = totalN23.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN24Total")).Text = totalN24.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN25Total")).Text = totalN25.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN26Total")).Text = totalN26.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN31Total")).Text = totalN31.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN36Total")).Text = totalN36.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN41Total")).Text = totalN41.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN46Total")).Text = totalN46.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblN51Total")).Text = totalN51.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblNALLTotal")).Text = totalNALL.ToString("#,##0");
                    ((Label)e.Item.FindControl("lblNFTotal")).Text = totalNF.ToString("#,##0");

                    totalN1 = 0;
                    totalN2 = 0;
                    totalN3 = 0;
                    totalN4 = 0;
                    totalN5 = 0;
                    totalN6 = 0;
                    totalN7 = 0;
                    totalN8 = 0;
                    totalN9 = 0;
                    totalN10 = 0;
                    totalN11 = 0;
                    totalN12 = 0;
                    totalN13 = 0;
                    totalN14 = 0;
                    totalN15 = 0;
                    totalN16 = 0;
                    totalN17 = 0;
                    totalN18 = 0;
                    totalN19 = 0;
                    totalN20 = 0;
                    totalN21 = 0;
                    totalN22 = 0;
                    totalN23 = 0;
                    totalN24 = 0;
                    totalN25 = 0;
                    totalN26 = 0;
                    totalN31 = 0;
                    totalN36 = 0;
                    totalN41 = 0;
                    totalN46 = 0;
                    totalN51 = 0;
                    totalNALL = 0;
                    totalNF = 0;
                }

                ((Label)e.Item.FindControl("lbl_Office")).Text = r.OfficeCode;
                ((Label)e.Item.FindControl("lbl_N1")).Text = r.N1.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N2")).Text = r.N2.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N3")).Text = r.N3.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N4")).Text = r.N4.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N5")).Text = r.N5.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N6")).Text = r.N6.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N7")).Text = r.N7.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N8")).Text = r.N8.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N9")).Text = r.N9.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N10")).Text = r.N10.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N11")).Text = r.N11.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N12")).Text = r.N12.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N13")).Text = r.N13.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N14")).Text = r.N14.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N15")).Text = r.N15.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N16")).Text = r.N16.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N17")).Text = r.N17.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N18")).Text = r.N18.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N19")).Text = r.N19.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N20")).Text = r.N20.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N21")).Text = r.N21.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N22")).Text = r.N22.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N23")).Text = r.N23.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N24")).Text = r.N24.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N25")).Text = r.N25.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N26")).Text = r.N26.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N31")).Text = r.N31.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N36")).Text = r.N36.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N41")).Text = r.N41.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N46")).Text = r.N46.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_N51")).Text = r.N51.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_NALL")).Text = r.NALL.ToString("#,##0");
                ((Label)e.Item.FindControl("lbl_NF")).Text = r.NF.ToString("#,##0");

                fiscalYear = r.BudgetYear;
                period = r.Period;

                totalN1 += r.N1;
                totalN2 += r.N2;
                totalN3 += r.N3;
                totalN4 += r.N4;
                totalN5 += r.N5;
                totalN6 += r.N6;
                totalN7 += r.N7;
                totalN8 += r.N8;
                totalN9 += r.N9;
                totalN10 += r.N10;
                totalN11 += r.N11;
                totalN12 += r.N12;
                totalN13 += r.N13;
                totalN14 += r.N14;
                totalN15 += r.N15;
                totalN16 += r.N16;
                totalN17 += r.N17;
                totalN18 += r.N18;
                totalN19 += r.N19;
                totalN20 += r.N20;
                totalN21 += r.N21;
                totalN22 += r.N22;
                totalN23 += r.N23;
                totalN24 += r.N24;
                totalN25 += r.N25;
                totalN26 += r.N26;
                totalN31 += r.N31;
                totalN36 += r.N36;
                totalN41 += r.N41;
                totalN46 += r.N46;
                totalN51 += r.N51;
                totalNALL += r.NALL;
                totalNF += r.NF;

                isFirst = false;
            }
                
        }
    }
}