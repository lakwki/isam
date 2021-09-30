using System;
using System.Collections;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.web;
using com.next.infra.util;
using com.next.isam.appserver.account;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.account;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.common.domain;
using com.next.common.web.commander;

namespace com.next.isam.webapp.account
{
    public partial class SuppInvReg : com.next.isam.webapp.usercontrol.PageTemplate
    {
        public int iDIRMax;
        public int iFRAMax;
        public int iRETMax;
        public int iJPMax;
        public int iCHMax;
        public int iMEMax;
        public int iTotalMax;
        public ArrayList userOfficeList;
        public int iScannedInvCount = 0;

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
            userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);


            if (!Page.IsPostBack)
            {
                ddl_PurchaseTerm.bindList(WebUtil.getTermOfPurchaseList(), "TermOfPurchaseDescription", "TermOfPurchaseId", "", "--All--", GeneralCriteria.ALL.ToString());
                ddl_Office.bindList(userOfficeList , "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
                ddl_OfficeFilter.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
                ddl_TradingAgency.bindList(WebUtil.getTradingAgencyList(), "ShortName", "TradingAgencyId", "", "--All--", GeneralCriteria.ALL.ToString());

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetInvoiceList);                
                Context.Items.Add(AccountCommander.Param.purchaseScanDateFrom, DateTime.Today.Date);
                Context.Items.Add(AccountCommander.Param.purchaseScanDateTo, DateTime.Today.Date);
                Context.Items.Add(AccountCommander.Param.workflowStatusId, ContractWFS.INVOICED.Id);
                Context.Items.Add(AccountCommander.Param.officeId, GeneralCriteria.ALL);
                Context.Items.Add(AccountCommander.Param.officeList, userOfficeList);
                forwardToScreen(null);
                
                ArrayList list = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];
                this.vwSearchResult = list;

                gv_Inv.DataSource = list;
                gv_Inv.DataBind();
                lbl_ScannedCount.Text = iScannedInvCount.ToString();
                pnl_Result.Visible = true;
            }

            txt_InvNo.Focus();
        }

        private void search()
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetInvoiceList);

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

            if (txt_InvNoFrom.Text.Trim() != "")
            {
                Context.Items.Add(AccountCommander.Param.invoicePrefix, WebUtil.getInvoicePrefix(txt_InvNoFrom.Text.Trim()));
                Context.Items.Add(AccountCommander.Param.invoiceSeqFrom, WebUtil.getInvoiceSeq(txt_InvNoFrom.Text.Trim()));
                Context.Items.Add(AccountCommander.Param.invoiceSeqTo, WebUtil.getInvoiceSeq(txt_InvNoTo.Text.Trim()));
                Context.Items.Add(AccountCommander.Param.invoiceYear, WebUtil.getInvoiceYear(txt_InvNoFrom.Text.Trim()));
            }
            else
            {
                Context.Items.Add(AccountCommander.Param.invoicePrefix, GeneralCriteria.ALLSTRING);
                Context.Items.Add(AccountCommander.Param.invoiceSeqFrom, GeneralCriteria.ALL);
                Context.Items.Add(AccountCommander.Param.invoiceSeqTo, GeneralCriteria.ALL);
                Context.Items.Add(AccountCommander.Param.invoiceYear, GeneralCriteria.ALL);
            }

            if (txt_UploadDateFrom.Text.Trim() != "")
            {
                Context.Items.Add(AccountCommander.Param.invoiceUploadDateFrom, Convert.ToDateTime(txt_UploadDateFrom.Text.Trim()));
                if (txt_UploadDateTo.Text.Trim() == "")
                    txt_UploadDateTo.Text = txt_UploadDateFrom.Text;
                Context.Items.Add(AccountCommander.Param.invoiceUploadDateTo, Convert.ToDateTime(txt_UploadDateTo.Text.Trim()));
            }
            else
            {
                Context.Items.Add(AccountCommander.Param.invoiceUploadDateFrom, DateTime.MinValue);
                Context.Items.Add(AccountCommander.Param.invoiceUploadDateTo, DateTime.MinValue);
            }

            if (txt_ScannedDateFrom.Text.Trim() != "")
            {
                Context.Items.Add(AccountCommander.Param.purchaseScanDateFrom, Convert.ToDateTime(txt_ScannedDateFrom.Text.Trim()));
                if (txt_ScannedDateTo.Text.Trim() == "")
                    txt_ScannedDateTo.Text = txt_ScannedDateFrom.Text;
                Context.Items.Add(AccountCommander.Param.purchaseScanDateTo, Convert.ToDateTime(txt_ScannedDateTo.Text.Trim()));
            }
            else
            {
                Context.Items.Add(AccountCommander.Param.purchaseScanDateFrom, DateTime.MinValue);
                Context.Items.Add(AccountCommander.Param.purchaseScanDateTo, DateTime.MinValue );
            }

            if (rad_Outstanding.Checked)
            {
                Context.Items.Add(AccountCommander.Param.purchaseScanStatus, 1);
            }
            else if (rad_Registered.Checked)
            {
                Context.Items.Add(AccountCommander.Param.purchaseScanStatus, 2);
            }
            else if (rad_Interfaced.Checked)
            {
                Context.Items.Add(AccountCommander.Param.purchaseScanStatus, 3);
            }

            if (ddl_Office.SelectedValue != GeneralCriteria.ALL.ToString())
                Context.Items.Add(AccountCommander.Param.officeId, ddl_Office.SelectedValue);
            else
            {
                Context.Items.Add(AccountCommander.Param.officeId, GeneralCriteria.ALL);
                Context.Items.Add(AccountCommander.Param.officeList, userOfficeList);
            }

            Context.Items.Add(AccountCommander.Param.tradingAgencyId, ddl_TradingAgency.SelectedValue);
            Context.Items.Add(AccountCommander.Param.termOfPurchaseId, ddl_PurchaseTerm.SelectedValue);
            Context.Items.Add(AccountCommander.Param.workflowStatusId, ContractWFS.INVOICED.Id);

            forwardToScreen(null);

            ArrayList list = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];

            this.vwSearchResult = list;

            gv_Inv.DataSource = list;
            gv_Inv.DataBind();
            lbl_ScannedCount.Text = iScannedInvCount.ToString();
            ddl_OfficeFilter.SelectedIndex = -1;
            pnl_Result.Visible = true;
        }


        protected void InvDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContractShipmentListJDef def = (ContractShipmentListJDef)vwSearchResult[e.Row.RowIndex];
                CheckBox ckb;
                Label lbl;

                if (ddl_OfficeFilter.SelectedValue != GeneralCriteria.ALL.ToString() && def.Office.OfficeId.ToString() != ddl_OfficeFilter.SelectedValue)
                {
                    e.Row.Visible = false;
                    return;
                }

                if (def.PurchaseScanDate == DateTime.MinValue)
                {
                    lbl = (Label)e.Row.Cells[12].FindControl("lbl_DocRecDateByAcc");
                    lbl.Text = "";

                    lbl = (Label)e.Row.Cells[15].FindControl("lbl_ScanDate");
                    lbl.Text = "";

                    if (AccountManager.Instance.isReleaseLockOpened(def.ShipmentId))
                    {
                        ckb = (CheckBox)e.Row.Cells[0].FindControl("ckb_inv");
                        ckb.Enabled = false;
                    }
                }
                else
                {
                    ckb = (CheckBox)e.Row.Cells[0].FindControl("ckb_inv");
                    ckb.Enabled = false;
                    iScannedInvCount++;
                }

                SunInterfaceLogDef interfaceLogDef = AccountManager.Instance.getInitialLogByShipmentId(SunInterfaceTypeRef.Id.Purchase.GetHashCode(), def.ShipmentId, 0);
                if (interfaceLogDef != null)
                {
                    lbl = (Label)e.Row.Cells[13].FindControl("lbl_ExtractDate");
                    lbl.Text = DateTimeUtility.getDateString(interfaceLogDef.CreatedOn);
                }

                if (def.SequenceNo == int.MinValue)
                {
                    lbl = (Label)e.Row.FindControl("lbl_SeqNo");
                    lbl.Text = "";
                }

                ckb = (CheckBox)e.Row.Cells[0].FindControl("ckb_Inv");
                if (ckb.Enabled && def.InvoicePrefix != string.Empty)
                {
                    if (def.InvoicePrefix[2] == 'D')
                    {
                        ckb.Attributes.Add("onclick", "UpdateCount(this,'txt_DIR');");
                        iDIRMax++;
                    }
                    else if (def.InvoicePrefix[2] == 'R')
                    {
                        ckb.Attributes.Add("onclick", "UpdateCount(this,'txt_RET');");
                        iRETMax++;
                    }
                    else if (def.InvoicePrefix[2] == 'M')
                    {
                        ckb.Attributes.Add("onclick", "UpdateCount(this,'txt_ME');");
                        iMEMax++;
                    }
                    else if (def.InvoicePrefix[2] == 'J')
                    {
                        ckb.Attributes.Add("onclick", "UpdateCount(this,'txt_JP');");
                        iJPMax++;
                    }
                    else if (def.InvoicePrefix[2] == 'O')
                    {
                        ckb.Attributes.Add("onclick", "UpdateCount(this,'txt_CH');");
                        iCHMax++;
                    }
                    else
                    {
                        ckb.Attributes.Add("onclick", "UpdateCount(this,'txt_FRA');");
                        iFRAMax++;
                    }
                }
            }
        }

        #region Button Events

        protected void btn_Refresh_Click(object sender, EventArgs e)
        {
            gv_Inv.DataSource = vwSearchResult;
            gv_Inv.DataBind();

            lbl_ScannedCount.Text = iScannedInvCount.ToString();
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
            search();
        }

        protected void btn_Add_Click(object sender, EventArgs e)
        {
            string invoicePrefix = txt_InvNo.Text.Trim().Substring(0, 3);
            int invoiceSeq = 0;
            int invoiceYear = 0;
            int sequenceNo = -1;
            int.TryParse(txt_InvNo.Text.Trim().Substring(4, 5), out invoiceSeq);
            int.TryParse(txt_InvNo.Text.Trim().Substring(10, 4), out invoiceYear);
            if (txt_InvNo.Text.Contains('-'))
            {
                int.TryParse(txt_InvNo.Text.Substring(txt_InvNo.Text.LastIndexOf('-')+1), out sequenceNo);
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetInvoiceListByInvoiceNo);
            Context.Items.Add(AccountCommander.Param.invoicePrefix, invoicePrefix);
            Context.Items.Add(AccountCommander.Param.invoiceSeq, invoiceSeq);
            Context.Items.Add(AccountCommander.Param.invoiceYear, invoiceYear);
            Context.Items.Add(AccountCommander.Param.sequenceNo, sequenceNo);

            forwardToScreen(null);

            ArrayList invoiceList = (ArrayList)Context.Items[AccountCommander.Param.invoiceList];
            ContractShipmentListJDef def;
            if (invoiceList != null && invoiceList.Count > 0)
            {
                def = (ContractShipmentListJDef)invoiceList[0];
                bool hasRight = false;
                foreach (OfficeRef officeRef in userOfficeList)
                {
                    if (officeRef.OfficeId == def.Office.OfficeId)
                    {
                        hasRight = true;
                        break;
                    }
                }
                if (!hasRight)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceinvalid", "alert('Registration failed.');", true);
                    return;
                }
                else if (def.PurchaseScanDate != DateTime.MinValue)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceinvalid", "alert('Invoice already registered.');", true);
                    return;
                }
                else if (AccountManager.Instance.isReleaseLockOpened(def.ShipmentId))
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceinvalid", "alert('This invoice cannot be registered for payment due to release lock request is submitted.');", true);
                    return;
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceinvalid", "alert('Invoice not found or user right insufficient.');", true);
                return;
            }

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.RegisterInvoiceForPayment);

            Context.Items.Add(AccountCommander.Param.invoiceList, invoiceList);

            forwardToScreen(null);
            pnl_Result.Visible = true;

            def.PurchaseScanDate = DateTime.Today;
            def.PurchaseScanBy = CommonUtil.getUserByKey(this.LogonUserId);
            if (vwSearchResult == null)
                vwSearchResult = new ArrayList();
            vwSearchResult.Insert(0, def);
            gv_Inv.DataSource = vwSearchResult;
            gv_Inv.DataBind();

            txt_InvNo.Text = "";
            lbl_ScannedCount.Text = iScannedInvCount.ToString();
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            CheckBox ckb;
            ContractShipmentListJDef def = null;
            ArrayList selectedList = new ArrayList();

            foreach (GridViewRow gvr in gv_Inv.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    ckb = (CheckBox) gvr.Cells[0].FindControl("ckb_inv");

                    if (ckb.Checked)
                    {
                        def = (ContractShipmentListJDef)vwSearchResult[gvr.RowIndex];
                        def.PurchaseScanDate = DateTime.Today.Date;
                        selectedList.Add(vwSearchResult[gvr.RowIndex]);                        
                    }
                }
            }

            if (selectedList.Count > 0)
            {
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.RegisterInvoiceForPayment);

                Context.Items.Add(AccountCommander.Param.invoiceList, selectedList);

                forwardToScreen(null);

                gv_Inv.DataSource = vwSearchResult;
                gv_Inv.DataBind();
                lbl_ScannedCount.Text = iScannedInvCount.ToString();
            }
        }

        protected void btn_Reset_Click(object sender, EventArgs e)
        {
            txt_InvDateFrom.Text = "";
            txt_InvDateTo.Text = "";
            txt_InvNoFrom.Text = "";
            txt_InvNoTo.Text = "";
            txt_UploadDateFrom.Text = "";
            txt_UploadDateTo.Text = "";
            txt_ScannedDateFrom.Text = "";
            txt_ScannedDateTo.Text = "";
            ddl_Office.SelectedIndex = 0;
            ddl_PurchaseTerm.SelectedIndex = 0;
            ddl_TradingAgency.SelectedIndex = 0;
            rad_Outstanding.Checked = false;
            rad_Registered.Checked = false;
            rad_Interfaced.Checked = false;
            rad_All.Checked = true;

            vwSearchResult = null;
            gv_Inv.DataSource = vwSearchResult;
            gv_Inv.DataBind();
            lbl_ScannedCount.Text = iScannedInvCount.ToString();
            pnl_Result.Visible = false;
        }

        protected void lnk_reg_Click(object sender, EventArgs e)
        {
            if (div_InvRegContent.Visible)
                div_InvRegContent.Visible = false;
            else
            {
                div_InvRegContent.Visible = true;
                div_SearchContent.Visible = false;
            }
        }

        protected void lnk_Search_Click(object sender, EventArgs e)
        {
            if (div_SearchContent.Visible)
                div_SearchContent.Visible = false;
            else
            {
                div_InvRegContent.Visible = false;
                div_SearchContent.Visible = true;
            }
        }

        #endregion


    }
}
