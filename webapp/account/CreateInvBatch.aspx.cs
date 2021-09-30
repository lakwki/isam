using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.web;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.isam.appserver.account;
using com.next.isam.domain.account;
using com.next.isam.domain.order;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.infra.util;

namespace com.next.isam.webapp.account
{
    public partial class CreateInvBatch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public int iInvTotal = 0;
        public double dInvTotalAmt = 0;

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

        private ArrayList vwUpdatedList
        {
            set
            {
                ViewState["UpdatedList"] = value;
            }
            get
            {
                return (ArrayList)ViewState["UpdatedList"];
            }
        }

        private void search()
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetInvoiceListForInvoiceBatch);

            Context.Items.Add(AccountCommander.Param.officeId, ddl_Office.SelectedValue);


            Context.Items.Add(AccountCommander.Param.tradingAgencyId, ddl_TradingAgency.SelectedValue);
            Context.Items.Add(AccountCommander.Param.orderType, ddl_OrderType.SelectedValue);
            Context.Items.Add(AccountCommander.Param.currencyId, ddl_Currency.SelectedValue);

            if (txt_RegDateFrom.Text.Trim() != "")
            {
                Context.Items.Add(AccountCommander.Param.salesScanDateFrom, DateTimeUtility.getDate(txt_RegDateFrom.Text.Trim()));
                if (txt_RegDateTo.Text.Trim() == "")
                    txt_RegDateTo.Text = txt_RegDateFrom.Text;
                Context.Items.Add(AccountCommander.Param.salesScanDateTo, DateTimeUtility.getDate(txt_RegDateTo.Text.Trim()));
            }

            if (txt_SubmitDateFrom.Text.Trim() != "")
            {
                Context.Items.Add(AccountCommander.Param.submittedOnFrom, DateTimeUtility.getDate(txt_SubmitDateFrom.Text.Trim()));
                if (txt_SubmitDateTo.Text.Trim() == "")
                    txt_SubmitDateTo.Text = txt_SubmitDateFrom.Text;
                Context.Items.Add(AccountCommander.Param.submittedOnTo, DateTimeUtility.getDate(txt_SubmitDateTo.Text.Trim()));
            }

            if (txt_BatchNo.Text.Trim() != "")
                Context.Items.Add(AccountCommander.Param.batchNo, txt_BatchNo.Text.Trim());

            if (txt_InvoiceNo.Text.Trim() != "")
            {
                string invoicePrefix = txt_InvoiceNo.Text.Trim().Substring(0, 3);
                int invoiceSeq = 0;
                int invoiceYear = 0;
                int.TryParse(txt_InvoiceNo.Text.Trim().Substring(4, 5), out invoiceSeq);
                int.TryParse(txt_InvoiceNo.Text.Trim().Substring(10, 4), out invoiceYear);

                Context.Items.Add(AccountCommander.Param.invoicePrefix, invoicePrefix);
                Context.Items.Add(AccountCommander.Param.invoiceSeqFrom, invoiceSeq);
                Context.Items.Add(AccountCommander.Param.invoiceSeqTo, invoiceSeq);
                Context.Items.Add(AccountCommander.Param.invoiceYear, invoiceYear);
            }

            if (rad_Registered.Checked)
                Context.Items.Add(AccountCommander.Param.invoiceBatchStatus, 1);
            else if (rad_Sent.Checked)
                Context.Items.Add(AccountCommander.Param.invoiceBatchStatus, 2);
            else
                Context.Items.Add(AccountCommander.Param.invoiceBatchStatus, 0);

            forwardToScreen(null);

            ArrayList invoiceList = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];

            pnl_result.Visible = true;

            vwSearchResult = invoiceList;
            txt_NoOfInv.Text = vwSearchResult.Count.ToString();

            gv_Inv.DataSource = invoiceList;
            gv_Inv.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ddl_Office.bindList(CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type), "OfficeCode", "OfficeId");
                ddl_TradingAgency.bindList(WebUtil.getTradingAgencyList(), "ShortName", "TradingAgencyId");
                ddl_OrderType.bindList(WebUtil.getOrderTypeList(), "Name", "Code", "", "--All--", GeneralCriteria.ALLSTRING.ToString());
                ddl_Currency.bindList(CommonUtil.getCurrencyList(), "CurrencyCode", "CurrencyId", "", "--All--", GeneralCriteria.ALL.ToString());
            }
        }

        #region GridView Event
        protected void InvDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContractShipmentListJDef def = (ContractShipmentListJDef)vwSearchResult[e.Row.RowIndex];
                Label lbl = (Label) e.Row.Cells[8].FindControl("lbl_ContractNo");

                lbl.Text = def.ContractNo + "-" + def.DeliveryNo;

                if (def.IsMockShopSample == 1 || def.IsPressSample == 1)
                {
                    lbl = (Label)e.Row.Cells[3].FindControl("lbl_Remark");
                    lbl.Text += def.IsMockShopSample == 1 ? "M" : "";
                    lbl.Text += def.IsPressSample == 1 ? "P" : "";
                }

                if (def.SalesScanDate == DateTime.MinValue || def.IsSelfBilledOrder || def.EInoviceBatchId  > 0 || ((def.Customer.OPSKey != "R" && def.Customer.OPSKey != "D") || def.CustomerDestination.DestinationCode == "CN" ))
                {
                    CheckBox ckb = (CheckBox)e.Row.Cells[1].FindControl("ckb_Inv");
                    ckb.Enabled = false;
                }

                if (def.EInoviceBatchId > 0)
                {
                    EInvoiceBatchDef invoiceBatchDef = null;
                    invoiceBatchDef = AccountManager.Instance.getEInvoiceBatchByKey(def.EInoviceBatchId);

                    lbl = (Label)e.Row.Cells[11].FindControl("lbl_BatchNo");
                    lbl.Text = invoiceBatchDef.EInvoiceBatchNo;

                    lbl = (Label)e.Row.Cells[12].FindControl("lbl_SubmitDate");
                    lbl.Text = DateTimeUtility.getDateString(invoiceBatchDef.SubmittedOn);

                    lbl = (Label)e.Row.Cells[13].FindControl("lbl_Status");
                    lbl.Text = "Sent To NUK";
                }
                else
                {
                    lbl = (Label)e.Row.Cells[13].FindControl("lbl_Status");
                    lbl.Text = "Registered";
                }
                
                double dInvAmt = double.Parse(((Label)e.Row.FindControl("lbl_InvAmt")).Text);

                dInvTotalAmt += dInvAmt;
            }
        }

        protected void InvoiceRowDelete(object sender, GridViewDeleteEventArgs arg)
        {
            ContractShipmentListJDef def = (ContractShipmentListJDef)vwSearchResult[arg.RowIndex];
            vwSearchResult.Remove(def);


            if (def.EInoviceBatchId > 0)
            {
                if (vwUpdatedList == null)
                    vwUpdatedList = new ArrayList();

                vwUpdatedList.Add(def);
            }

            gv_Inv.DataSource = vwSearchResult;
            gv_Inv.DataBind();
            txt_NoOfInv.Text  = vwSearchResult.Count.ToString();
        }
        #endregion

        #region Button Events

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            search();
        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            vwSearchResult = null;
            gv_Inv.DataSource = vwSearchResult;
            gv_Inv.DataBind();
            pnl_result.Visible = false;

            ddl_Office.SelectedIndex = -1;
            ddl_TradingAgency.SelectedIndex = -1;
            ddl_OrderType.SelectedIndex = -1;
            ddl_Currency.SelectedIndex = -1;
            txt_RegDateFrom.Text = "";
            txt_RegDateTo.Text = "";
            txt_SubmitDateFrom.Text = "";
            txt_SubmitDateTo.Text = "";
            txt_BatchNo.Text = "";
            txt_InvoiceNo.Text = "";
            rad_Registered.Checked = false;
            rad_Sent.Checked = false;
            rad_All.Checked = true;
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            ArrayList selectedInvoice = new ArrayList();
            CheckBox ckb;
            foreach (GridViewRow row in gv_Inv.Rows)
            {
                ckb = (CheckBox) row.Cells[1].FindControl("ckb_Inv");
                if (ckb.Checked)
                    selectedInvoice.Add(vwSearchResult[row.RowIndex]);
            }

            if (selectedInvoice.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invbatcherror", "alert('No invoice is selected.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.CreateEInvoiceBatch);

            Context.Items.Add(AccountCommander.Param.officeId, ddl_Office.SelectedValue);

            Context.Items.Add(AccountCommander.Param.invoiceList, selectedInvoice);
            
            forwardToScreen(null);

            search();

        }

        protected void btn_Modify_Click(object sender, EventArgs e)
        {
            gv_Inv.Columns[0].Visible = true;
            btn_Submit.Visible = false;
            btn_Modify.Visible = false;
            btn_ReGen.Visible = true;
            btn_Cancel.Visible = true;
        }


        protected void btn_ReGen_Click(object sender, EventArgs e)
        {
            if (vwUpdatedList == null || vwUpdatedList.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invBatchErr", "alert('No invoice batch is modified.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.DeleteFromEInvoiceBatch);            

            Context.Items.Add(AccountCommander.Param.invoiceList, vwUpdatedList);

            forwardToScreen(null);

            vwUpdatedList.Clear();

            btn_Submit.Visible = true;
            btn_Modify.Visible = true;
            btn_ReGen.Visible = false;
            btn_Cancel.Visible = false;
            gv_Inv.Columns[0].Visible = false;

        }


        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (vwUpdatedList != null)
            {
                foreach (ContractShipmentListJDef def in vwUpdatedList)
                {
                    vwSearchResult.Add(def);
                }

                vwUpdatedList.Clear();
            }
            
            btn_Submit.Visible = true;
            btn_Modify.Visible = true;
            btn_ReGen.Visible = false;
            btn_Cancel.Visible = false;
            gv_Inv.Columns[0].Visible = false;

            gv_Inv.DataSource = vwSearchResult;
            gv_Inv.DataBind();
        }

        #endregion
    }
}
