using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.infra.web;
using com.next.isam.webapp.commander;
using com.next.isam.appserver.common;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.OleDb;
using com.next.infra.util;
using com.next.isam.domain.account;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.types;
using com.next.common.domain.industry.vendor;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.account;
using com.next.common.datafactory.worker;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.nontrade;
using com.next.isam.reporter.accounts;

namespace com.next.isam.webapp.account
{
    public partial class SendThirdPartyCustomerDNToUK : com.next.isam.webapp.usercontrol.PageTemplate
    {
        ArrayList vwSalesList
        {
            get { return (ArrayList)ViewState["SalesList"]; }
            set { ViewState["SalesList"] = value; }
        }

        ArrayList vwSupplierList
        {
            get { return (ArrayList)ViewState["SupplierList"]; }
            set { ViewState["SupplierList"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                AccountFinancialCalenderDef finCalDef = CommonUtil.getCurrentFinancialPeriod(9);
                ddl_Year.DataSource = WebUtil.getBudgetYearList();
                ddl_Year.DataBind();
                ddl_Year.SelectedValue = finCalDef.BudgetYear.ToString();
                ddl_Period.SelectedValue = finCalDef.Period.ToString();
            }
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            int FiscalYear = int.Parse(ddl_Year.SelectedValue);
            int Period = int.Parse(ddl_Period.SelectedValue);

            this.vwSalesList = new ArrayList();
            vwSalesList = AccountManager.Instance.getThirdPartyCommissionInvoiceToUK(FiscalYear, Period);

            if (vwSalesList != null)
                btn_Send.Visible = true;
            else
                btn_Send.Visible = false;

            gv_sales.DataSource = vwSalesList;
            gv_sales.DataBind();
        }

        protected void btn_Send_Click(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                bool found = false;
                this.vwSupplierList = new ArrayList();
                foreach (ThirdPartyCustomerDNToUKDef def in vwSalesList)
                {
                    foreach (ThirdPartyCustomerDNToUKDef sup in vwSupplierList)
                    {
                        if (sup.OfficeId == def.OfficeId && sup.Currency == def.Currency)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        vwSupplierList.Add(def);
                    found = false;
                }

                ArrayList shipmentIdList = new ArrayList();

                foreach (ThirdPartyCustomerDNToUKDef sup in vwSupplierList)
                {
                    shipmentIdList = new ArrayList();
                    foreach (ThirdPartyCustomerDNToUKDef def in vwSalesList)
                    {
                        if (sup.OfficeId == def.OfficeId && sup.Currency == def.Currency)
                            shipmentIdList.Add(def.ShipmentId);
                    }
                    string supplierCode = AccountWorker.Instance.getUKPaymentSupplierNo(sup.OfficeId, GeneralWorker.Instance.getCurrencyByCurrencyCode(sup.Currency).CurrencyId);

                    Context.Items.Clear();
                    Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                    Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GenerateThirdPartyCustomerDNToUK);

                    Context.Items.Add(AccountCommander.Param.officeId, sup.OfficeId);
                    Context.Items.Add(AccountCommander.Param.fiscalYear, int.Parse(ddl_Year.SelectedValue));
                    Context.Items.Add(AccountCommander.Param.period, int.Parse(ddl_Period.SelectedValue));
                    Context.Items.Add(AccountCommander.Param.supplierCode, supplierCode);
                    Context.Items.Add(AccountCommander.Param.shipmentIdList, shipmentIdList);

                    forwardToScreen(null);
                }
            }
        }

        protected void gv_sales_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv_sales.PageIndex = e.NewPageIndex;
            gv_sales.DataSource = this.vwSalesList;
            gv_sales.DataBind();
        }
    }
}
