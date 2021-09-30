using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.common.datafactory.worker;
using com.next.isam.appserver.account;
using System.Web.UI.WebControls;
using com.next.isam.webapp.commander.account;
using com.next.isam.webapp.commander;
using com.next.infra.web;
using com.next.isam.domain.common;
using com.next.isam.appserver.common;
using com.next.common.domain.module;
using com.next.common.domain.types;
using System.Collections.Generic;
using com.next.common.domain.dms;
using com.next.isam.domain.shipping;
using com.next.infra.util;
using com.next.isam.dataserver.worker;

namespace com.next.isam.webapp.account
{
    public partial class NSLedSalesData : com.next.isam.webapp.usercontrol.PageTemplate
    {
        ArrayList vwNSLedSalesList
        {
            get { return (ArrayList)ViewState["NSLedSalesList"]; }
            set { ViewState["NSLedSalesList"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ddl_Office.bindList(CommonUtil.getOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());

                //Get Year List
                List<NSLedImportFileDef> fiscalYearList = AccountManager.Instance.getNSLedFiscalYearList();
                for (int i = 0; i < fiscalYearList.Count; i++)
                {
                    this.ddl_Year.Items.Add(new ListItem(fiscalYearList[i].FiscalYear.ToString()));
                }

                //Get Fiscal Week List with selected Year
                fiscalWeekDataBind();

                //Get Country List
                List<NSLedSalesDef> countryList = AccountManager.Instance.getNSLedSalesCountry();
                this.ddl_Country.Items.Add(new ListItem("ALL"));
                for (int i = 0; i < countryList.Count; i++)
                {
                    this.ddl_Country.Items.Add(new ListItem(countryList[i].CountryOfSale.ToString()));
                }
            }
        }

        protected void ddl_Year_SelectedIndexChanged(object sender, EventArgs e)
        {
            fiscalWeekDataBind();
        }

        protected void fiscalWeekDataBind()
        {
            ddl_Week.Items.Clear();
            List<NSLedImportFileDef> fiscalWeekList = AccountManager.Instance.getNSLedFiscalWeekList(int.Parse(this.ddl_Year.Text));
            for (int i = 0; i < fiscalWeekList.Count; i++)
            {
                this.ddl_Week.Items.Add(new ListItem(fiscalWeekList[i].FiscalWeek.ToString()));
            }
        }

        protected void gv_NSLedSales_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                NSLedSalesDef def = (NSLedSalesDef)this.vwNSLedSalesList[(this.gv_NSLedSales.PageIndex * this.gv_NSLedSales.PageSize) + e.Row.RowIndex];

                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = def.ItemNo;
                ((Label)e.Row.FindControl("lbl_OptionNo")).Text = def.SizeOptionNo;
                ((Label)e.Row.FindControl("lbl_Size")).Text = def.SizeDesc;
                ((Label)e.Row.FindControl("lbl_Description")).Text = def.ItemDesc;
                ((Label)e.Row.FindControl("lbl_Despatch_Qty")).Text = def.DespatchQty.ToString();
                ((Label)e.Row.FindControl("lbl_Despatch_Value")).Text = def.DespatchAmt.ToString();
                if (def.ReturnQty < 0)
                {
                    ((Label)e.Row.FindControl("lbl_Returns_Qty")).ForeColor = System.Drawing.Color.Red;
                    ((Label)e.Row.FindControl("lbl_Returns_Qty")).Text = "(" + Math.Abs(def.ReturnQty).ToString() + ")";
                }
                else
                    ((Label)e.Row.FindControl("lbl_Returns_Value")).Text = def.ReturnAmt.ToString();

                if (def.ReturnAmt < 0)
                {
                    ((Label)e.Row.FindControl("lbl_Returns_Value")).ForeColor = System.Drawing.Color.Red;
                    ((Label)e.Row.FindControl("lbl_Returns_Value")).Text = "(" + Math.Abs(def.ReturnAmt).ToString() + ")";
                }
                else
                    ((Label)e.Row.FindControl("lbl_Returns_Qty")).Text = def.ReturnAmt.ToString();

                if (def.NetQty < 0)
                {
                    ((Label)e.Row.FindControl("lbl_Net_Qty")).ForeColor = System.Drawing.Color.Red;
                    ((Label)e.Row.FindControl("lbl_Net_Qty")).Text = "(" + Math.Abs(def.NetQty).ToString() + ")";
                }
                else
                    ((Label)e.Row.FindControl("lbl_Net_Qty")).Text = def.NetQty.ToString();

                ((Label)e.Row.FindControl("lbl_Country_Of_Sale")).Text = def.CountryOfSale;

                if (def.NetAmt < 0)
                {
                    ((Label)e.Row.FindControl("lbl_Net_Value")).ForeColor = System.Drawing.Color.Red;
                    ((Label)e.Row.FindControl("lbl_Net_Value")).Text = "(" + Math.Abs(def.NetAmt).ToString() + ")";
                }
                else
                    ((Label)e.Row.FindControl("lbl_Net_Value")).Text = def.NetAmt.ToString();

                ((Label)e.Row.FindControl("lbl_VAT_Percent")).Text = (def.VATPercent * 100).ToString() + "%";

                if (def.VAT < 0)
                {
                    ((Label)e.Row.FindControl("lbl_VAT")).ForeColor = System.Drawing.Color.Red;
                    ((Label)e.Row.FindControl("lbl_VAT")).Text = "(" + Math.Abs(def.VAT).ToString() + ")";
                }
                else
                    ((Label)e.Row.FindControl("lbl_VAT")).Text = def.VAT.ToString();

                if (def.NSVE < 0)
                {
                    ((Label)e.Row.FindControl("lbl_NSVE")).ForeColor = System.Drawing.Color.Red;
                    ((Label)e.Row.FindControl("lbl_NSVE")).Text = "(" + Math.Abs(def.NSVE).ToString() + ")";
                }
                else
                    ((Label)e.Row.FindControl("lbl_NSVE")).Text = def.NSVE.ToString();

                if (def.NSCommAmt < 0)
                {
                    ((Label)e.Row.FindControl("lbl_Commission")).ForeColor = System.Drawing.Color.Red;
                    ((Label)e.Row.FindControl("lbl_Commission")).Text = "(" + Math.Abs(def.NSCommAmt).ToString() + ")";
                }
                else
                    ((Label)e.Row.FindControl("lbl_Commission")).Text = def.NSCommAmt.ToString();

                ((Label)e.Row.FindControl("lbl_USDExRate")).Text = def.USDExRate.ToString();
            }
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            gv_NSLedSales.Visible = true;
            string invoice_No = this.txt_InvoiceNo.Text;
            int fiscalYear = GeneralCriteria.ALL;
            int fiscalWeek = GeneralCriteria.ALL;
            if (rad_weekly.Checked)
            {
                fiscalYear = ddl_Year.selectedValueToInt;
                fiscalWeek = ddl_Week.selectedValueToInt;
            }
            string item_No = this.txt_ItemNo.Text;
            string country = this.ddl_Country.Text;
            int officeId = this.ddl_Office.selectedValueToInt;

            this.vwNSLedSalesList = AccountManager.Instance.getNSLedSales(officeId, invoice_No, fiscalYear, fiscalWeek, item_No, country);
            gv_NSLedSales.DataSource = this.vwNSLedSalesList;
            gv_NSLedSales.DataBind();
        }

        protected void gv_NSLedSales_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_NSLedSales.PageIndex = e.NewPageIndex;
            gv_NSLedSales.DataSource = this.vwNSLedSalesList;
            gv_NSLedSales.DataBind();
        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            txt_InvoiceNo.Text = "";
            txt_ItemNo.Text = "";
            fiscalWeekDataBind();
            ddl_Year.SelectedIndex = 0;
            ddl_Week.SelectedIndex = 0;
            ddl_Country.SelectedIndex = 0;
            gv_NSLedSales.Visible = false;
            this.vwNSLedSalesList = null;
        }

        protected void rad_all_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_all.Checked)
            {
                ddl_Year.Enabled = false;
                ddl_Week.Enabled = false;
            }
        }

        protected void rad_weekly_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_weekly.Checked)
            {
                ddl_Year.Enabled = true;
                ddl_Week.Enabled = true;
            }
        }
    }
}
