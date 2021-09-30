using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using com.next.isam.webapp.commander.account;
using com.next.isam.webapp.commander;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.infra.web;

namespace com.next.isam.webapp.account
{
    public partial class ARShipmentReg : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public int iTotalMax = 0;

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


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ddl_Office.bindList(CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type), "OfficeCode", "OfficeId");
                ddl_TradingAgency.bindList(WebUtil.getTradingAgencyList(), "ShortName", "TradingAgencyId");

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetInvoiceList);

                Context.Items.Add(AccountCommander.Param.officeId, GeneralCriteria.ALL);
                Context.Items.Add(AccountCommander.Param.officeList, CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type));
                Context.Items.Add(AccountCommander.Param.workflowStatusId, ContractWFS.INVOICED.Id);
                Context.Items.Add(AccountCommander.Param.salesScanDateFrom, DateTime.Today.Date);
                Context.Items.Add(AccountCommander.Param.salesScanDateTo, DateTime.Today.Date);

                forwardToScreen(null);

                this.vwSearchResult  = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];
                vwSearchResult.Sort(new ContractShipmentListJDef.ContractShipmentComparer(ContractShipmentListJDef.ContractShipmentComparer.CompareType.SalesScanDate, SortDirection.Descending));
                gv_Inv.DataSource = vwSearchResult;
                gv_Inv.DataBind();
                pnl_Result.Visible = true;
            }

            txt_InvNo.Focus();
        }



        protected void InvDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {                
                ContractShipmentListJDef def = (ContractShipmentListJDef)vwSearchResult[e.Row.RowIndex];

                if (def.SalesScanDate != DateTime.MinValue || def.IsSelfBilledOrder)
                {
                    CheckBox ckb = (CheckBox)e.Row.Cells[0].FindControl("ckb_Inv");
                    ckb.Enabled = false;
                }

            }
        }

        #region Button Event

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetInvoiceListForInvoiceBatch);

            if (txt_InvDateFrom.Text.Trim() != "")
            {
                Context.Items.Add(AccountCommander.Param.invoiceDateFrom, Convert.ToDateTime(txt_InvDateFrom.Text.Trim()));
                if (txt_InvDateTo.Text.Trim() == "")
                    txt_InvDateTo.Text = txt_InvDateFrom.Text;
                Context.Items.Add(AccountCommander.Param.invoiceDateTo, Convert.ToDateTime(txt_InvDateTo.Text.Trim()));
            }
            else
            {
                Context.Items.Add(AccountCommander.Param.invoiceDateFrom, DateTime.MinValue);
                Context.Items.Add(AccountCommander.Param.invoiceDateTo, DateTime.MinValue);
            }

            if (txt_InvoiceNoFrom.Text.Trim() != "")
            {
                Context.Items.Add(AccountCommander.Param.invoicePrefix, WebUtil.getInvoicePrefix(txt_InvoiceNoFrom.Text.Trim()));
                Context.Items.Add(AccountCommander.Param.invoiceSeqFrom, WebUtil.getInvoiceSeq(txt_InvoiceNoFrom.Text.Trim()));
                Context.Items.Add(AccountCommander.Param.invoiceSeqTo, WebUtil.getInvoiceSeq(txt_InvoiceNoTo.Text.Trim()));
                Context.Items.Add(AccountCommander.Param.invoiceYear, WebUtil.getInvoiceYear(txt_InvoiceNoFrom.Text.Trim()));
            }


            if (ddl_Office.SelectedValue != GeneralCriteria.ALL.ToString())
                Context.Items.Add(AccountCommander.Param.officeId, ddl_Office.SelectedValue);
            

            Context.Items.Add(AccountCommander.Param.tradingAgencyId, ddl_TradingAgency.SelectedValue);
            Context.Items.Add(AccountCommander.Param.workflowStatusId, ContractWFS.INVOICED.Id);

            forwardToScreen(null);

            ArrayList list = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];

            this.vwSearchResult = list;

            gv_Inv.DataSource = list;
            gv_Inv.DataBind();

            if (list.Count >= 300)
            {
                lbl_Warning.Visible = true;
            }

            pnl_Result.Visible = true;

        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            ddl_Office.SelectedIndex = -1;
            ddl_TradingAgency.SelectedIndex = -1;
            txt_InvDateFrom.Text = "";
            txt_InvDateTo.Text = "";

            vwSearchResult = null;
            lbl_Warning.Visible = false;
            pnl_Result.Visible = false;
        }


        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            ArrayList selectedList = new ArrayList();

            foreach (GridViewRow row in gv_Inv.Rows)
            {
                CheckBox ckb = (CheckBox) row.Cells[0].FindControl("ckb_Inv");
                if (ckb.Checked)
                    selectedList.Add(vwSearchResult[row.RowIndex]);
            }

            if (selectedList.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "inv", "alert('No invoice selected.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.RegisterAR);

            Context.Items.Add(AccountCommander.Param.invoiceList, selectedList);            

            forwardToScreen(null);

            btn_Search_Click(null, e);
        }

        protected void btn_Add_Click(object sender, EventArgs e)
        {
            string invoicePrefix = txt_InvNo.Text.Trim().Substring(0, 3);
            int invoiceSeq = 0;
            int invoiceYear = 0;
            int.TryParse(txt_InvNo.Text.Trim().Substring(4, 5), out invoiceSeq);
            int.TryParse(txt_InvNo.Text.Trim().Substring(10, 4), out invoiceYear);

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetInvoiceListByInvoiceNo);

            
            Context.Items.Add(AccountCommander.Param.invoicePrefix, invoicePrefix);
            Context.Items.Add(AccountCommander.Param.invoiceSeq, invoiceSeq);
            Context.Items.Add(AccountCommander.Param.invoiceYear, invoiceYear);
            Context.Items.Add(AccountCommander.Param.officeList, CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type));

            forwardToScreen(null);

            ArrayList list = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];
            if (list.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invNotFound", "alert('Invoice not found');" ,true);
                return;
            }

            ContractShipmentListJDef def = (ContractShipmentListJDef)list[0];
            if (def.SalesScanDate != DateTime.MinValue)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invRegError", "alert('Invoice already registered.');", true);
                return;
            }
            else if (def.IsSelfBilledOrder)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invRegError", "alert('Registration failed - self billed order.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.RegisterAR);

            Context.Items.Add(AccountCommander.Param.invoiceList, list);

            forwardToScreen(null);

            txt_InvNo.Text = "";
            txt_InvNo.Focus();

            if (vwSearchResult == null)
                vwSearchResult = new ArrayList();

            def.SalesScanDate = DateTime.Today.Date;
            vwSearchResult.Insert(0, def);
            gv_Inv.DataSource = vwSearchResult;
            gv_Inv.DataBind();

        }

        #endregion
    }
}
