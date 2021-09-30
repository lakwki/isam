using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CrystalDecisions.CrystalReports.Engine;

using com.next.common.domain;
using com.next.infra.util;
using com.next.infra.web;
using com.next.common.web.commander;
using com.next.isam.webapp.usercontrol;
using com.next.common.appserver;
using com.next.isam.webapp.commander.admin;
//using com.next.isam.reporter.shipping;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.helper;
using com.next.isam.appserver.account;
using com.next.common.datafactory.worker;
using com.next.isam.domain.types;
using com.next.isam.domain.common;
using com.next.isam.dataserver.worker;
using com.next.common.domain.module;

namespace com.next.isam.webapp.admin
{

    public partial class MonthEndAdmin : PageTemplate
    {
        bool ableToAccess_SetReady = false;
        //bool updateMonthEndStart = true;
        bool needResetMonthEndStatus = false;
        protected System.Web.UI.WebControls.DataGrid grdOffice;
        bool isAdmin = false;

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
            {
                com.next.common.datafactory.worker.GeneralWorker.Instance.notifyChanged();
                ArrayList officeList = CommonUtil.getOfficeList();
                this.vwOfficeList = officeList;
                this.grdOffice.DataSource = officeList;
                this.grdOffice.DataBind();

                this.vwFiscalPeriod = getCurrentPeriod();
                lbl_FiscalYear.Text = "???? P?";
                lbl_AutoCutoffTime.Text = "??/??/???? ~ ??/??/????";
                if (this.vwFiscalPeriod != null)
                {
                    lbl_FiscalYear.Text = vwFiscalPeriod.BudgetYear.ToString() + " P" + vwFiscalPeriod.Period.ToString();
                    DateTime autoCutoffTime = AccountManager.Instance.getAutomaticCutoffStartTime(this.vwFiscalPeriod.EndDate.AddDays(1));
                    DateTime autoCutoffEndTime = AccountManager.Instance.getAutomaticCutoffEndTime(this.vwFiscalPeriod.EndDate.AddDays(1));
                    if (autoCutoffTime != DateTime.MinValue)
                    {
                        lbl_AutoCutoffTime.Text = autoCutoffTime.ToString("dd/MM/yyyy HH:mm:ss") + " ~ " + autoCutoffEndTime.ToString("dd/MM/yyyy HH:mm:ss");
                        if (DateTime.Now > autoCutoffEndTime || autoCutoffTime > autoCutoffEndTime)
                            lbl_AutoCutoffTime.Style.Add(HtmlTextWriterStyle.Color, "grey");
                    }
                }
                btnSetReady.Visible = (ableToAccess_SetReady);
                if (needResetMonthEndStatus)
                    resetMonthEndStatus();

                isAdmin = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.systemAdmin.Id);
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grdOffice.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdOffice_ItemCommand);
            this.grdOffice.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.grdOffice_ItemDataBound);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private AccountFinancialCalenderDef getCurrentPeriod()
        {
            AccountFinancialCalenderDef currentPeriod = null;
            bool allCompleted = true;
            foreach (OfficeRef office in this.vwOfficeList)
                if (office.MonthEndStatusId != MonthEndStatus.NOTREADY.Id)
                    allCompleted &= (office.MonthEndStatusId == MonthEndStatus.COMPLETED.Id);
            AccountFinancialCalenderDef periodToday = GeneralWorker.Instance.getAccountPeriodByDate(APPID, DateTime.Now);
            AccountFinancialCalenderDef period10DaysBefore = GeneralWorker.Instance.getAccountPeriodByDate(APPID, DateTime.Now.AddDays(-10));

            SystemParameterRef paramFiscalYear = CommonWorker.Instance.getSystemParameterByName("MONTH_END_ADMIN_FISCAL_YEAR");
            SystemParameterRef paramPeriod = CommonWorker.Instance.getSystemParameterByName("MONTH_END_ADMIN_PERIOD");
            if (paramFiscalYear != null && paramPeriod != null)
                if (!string.IsNullOrEmpty(paramFiscalYear.ParameterValue) && !string.IsNullOrEmpty(paramPeriod.ParameterValue))
                    currentPeriod = GeneralWorker.Instance.getAccountPeriodByYearPeriod(APPID, Convert.ToInt32(paramFiscalYear.ParameterValue),Convert.ToInt32(paramPeriod.ParameterValue));
            if (currentPeriod == null)
            {   // systerm generate the Fiscal Year and period
                if (periodToday.BudgetYear == period10DaysBefore.BudgetYear && periodToday.Period == period10DaysBefore.Period)
                {
                    currentPeriod = periodToday;
                    needResetMonthEndStatus = allCompleted;
                }
                else
                    currentPeriod = period10DaysBefore;
            }
            return currentPeriod;
        }

        void resetMonthEndStatus()
        {
            ArrayList updateList = new ArrayList();
            foreach (OfficeRef office in this.vwOfficeList)
                if (office.MonthEndStatusId != MonthEndStatus.NOTREADY.Id)
                {
                    office.MonthEndStatusId = MonthEndStatus.READY.Id;
                    //if (updateMonthEndStart)
                    //    office.MonthEndStarted = 0;
                    updateList.Add(office);
                }
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.monthEnd");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, MonthEndCommander.Action.resetMonthEndStatus);
            Context.Items.Add("OfficeList", updateList);
            Context.Items.Add("FiscalPeriod", vwFiscalPeriod.BudgetYear.ToString() + " P" + vwFiscalPeriod.Period.ToString());
            this.forwardToScreen("admin.monthEnd");
        }

        private void updateMonthEndStatus(OfficeRef office)
        {
            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.monthEnd");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, MonthEndCommander.Action.updateMonthEndStatus);
            Context.Items.Add("OfficeRef", office);
            Context.Items.Add("FiscalPeriod", vwFiscalPeriod.BudgetYear.ToString() + " P" + vwFiscalPeriod.Period.ToString());
            this.forwardToScreen("admin.monthEnd");
        }
        private void grdOffice_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (WebHelper.isGridNormalItemType(e))
            {
                OfficeRef office = (OfficeRef)e.Item.DataItem;
                //this.vwOfficeList.Add(office);
                Label lblOfficeCode = (Label)e.Item.FindControl("lblOfficeCode");
                Label lblDescription = (Label)e.Item.FindControl("lblDescription");
                //Button btn = (Button)e.Item.FindControl("btnChangeStatus");
                //btn.Text = office.MonthEndStarted == 0 ? "Freeze" : "Resume";
                TextBox txtStatus = (TextBox)e.Item.FindControl("txtStatus");
                TableCell tcStart = (TableCell)e.Item.FindControl("tdStart");
                TableCell tcPause = (TableCell)e.Item.FindControl("tdPause");
                TableCell tcVerify = (TableCell)e.Item.FindControl("tdVerify");
                TableCell tcReady = (TableCell)e.Item.FindControl("tdReady");
                TableCell tcCapture = (TableCell)e.Item.FindControl("tdCapture");
                TableCell tcComplete = (TableCell)e.Item.FindControl("tdComplete");
                TableCell tcFail = (TableCell)e.Item.FindControl("tdFail");
                TableCell tcSendSlippage = (TableCell)e.Item.FindControl("tdSendSlippage");
                TableCell tcInterface = (TableCell)e.Item.FindControl("tdInterface");

                disableButton(tcSendSlippage,false);
                disableButton(tcInterface,false);

                lblOfficeCode.Text = office.OfficeCode;
                lblDescription.Text = office.Description;
                txtStatus.Text = MonthEndStatus.getStatus(office.MonthEndStatusId).Description;
                if (office.MonthEndStatusId == MonthEndStatus.FAILED.Id)
                {   // Highlight the text box
                    txtStatus.BackColor = System.Drawing.Color.FromArgb(255, 255, 0);
                    txtStatus.ForeColor = System.Drawing.Color.FromArgb(255, 0, 0);
                    txtStatus.Font.Bold = true;
                }

                if (office.MonthEndStatusId == MonthEndStatus.READY.Id)
                {
                    enableButton(tcStart);
                    disableButton(tcPause, false);
                    disableButton(tcVerify);
                    disableButton(tcReady);
                    disableButton(tcCapture);
                    disableButton(tcComplete);
                    disableButton(tcFail, false);
                }
                else if (office.MonthEndStatusId == MonthEndStatus.START.Id || office.MonthEndStatusId == MonthEndStatus.VERIFY.Id)
                {
                    disableButton(tcStart, false);
                    enableButton(tcPause);
                    enableButton(tcVerify);
                    enableButton(tcReady);
                    disableButton(tcCapture);
                    disableButton(tcComplete);
                    disableButton(tcFail, false);
                }
                else if (office.MonthEndStatusId == MonthEndStatus.READYTOCAPTURE.Id)
                {
                    disableButton(tcStart, false);
                    disableButton(tcPause);
                    disableButton(tcVerify);
                    disableButton(tcReady);
                    enableButton(tcCapture);
                    disableButton(tcComplete);
                    disableButton(tcFail, false);
                }
                else if (office.MonthEndStatusId == MonthEndStatus.CAPTURED.Id)
                {
                    disableButton(tcStart, false);
                    disableButton(tcPause);
                    disableButton(tcVerify);
                    disableButton(tcReady);
                    disableButton(tcCapture);
                    enableButton(tcComplete);
                    disableButton(tcFail, false);
                    enableButton(tcSendSlippage);
                    enableButton(tcInterface);
                }
                else if (office.MonthEndStatusId == MonthEndStatus.COMPLETED.Id)
                {
                    disableButton(tcStart, false);
                    disableButton(tcPause);
                    disableButton(tcVerify);
                    disableButton(tcReady);
                    disableButton(tcCapture);
                    disableButton(tcComplete);
                    disableButton(tcFail, false);
                }
                else if (office.MonthEndStatusId == MonthEndStatus.FAILED.Id)
                {
                    disableButton(tcStart, false);
                    disableButton(tcPause);
                    disableButton(tcVerify);
                    disableButton(tcReady);
                    disableButton(tcCapture);
                    disableButton(tcComplete, false);
                    enableButton(tcFail, true);
                    enableButton(tcSendSlippage);
                    enableButton(tcInterface);
                }
                else
                {   // Not Ready for month end
                    disableButton(tcStart);
                    disableButton(tcPause, false);
                    disableButton(tcVerify);
                    disableButton(tcReady);
                    disableButton(tcCapture);
                    disableButton(tcComplete);
                    disableButton(tcFail, false);
                    
                    //TableRow r=e.Item.Visible = false;
                    e.Item.Visible = false;
                }
            }
        }

        void enableButton(TableCell cell)
        {
            enableButton(cell, true);
        }

        void enableButton(TableCell cell, bool visible)
        {
            Button enabledBtn = (Button)cell.FindControl(cell.ID.Replace("td", "btn"));
            Button disabledBtn = (Button)cell.FindControl(cell.ID.Replace("td", "btn") + "Disabled");
            enabledBtn.Visible = visible;
            disabledBtn.Visible = false;
        }

        void disableButton(TableCell cell)
        {
            disableButton(cell, true);
        }

        void disableButton(TableCell cell, bool visible)
        {
            Button enabledBtn = (Button)cell.FindControl(cell.ID.Replace("td", "btn"));
            Button disabledBtn = (Button)cell.FindControl(cell.ID.Replace("td", "btn") + "Disabled");
            enabledBtn.Visible = false;
            disabledBtn.Visible = visible;
        }

        private void grdOffice_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            OfficeRef office = (OfficeRef)this.vwOfficeList[e.Item.ItemIndex];
            /*
            if (e.CommandName == "ChangeStatus")
            {
                if (office.MonthEndStarted == 0)
                    office.MonthEndStarted = 1;
                else
                    office.MonthEndStarted = 0;

                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.monthEnd");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, MonthEndCommander.Action.changeStatus);
                Context.Items.Add("OfficeRef", office);
                this.forwardToScreen("admin.monthEnd");
            }
            else 
            */
            if (e.CommandName == "StartMonthEnd" || e.CommandName == "RestartMonthEnd")
            {
                //if (updateMonthEndStart)
                //    office.MonthEndStarted = 1;
                office.MonthEndStatusId = MonthEndStatus.START.Id;
                updateMonthEndStatus(office);
            }
            else if (e.CommandName == "PauseMonthEnd")
            {
                //if (updateMonthEndStart)
                //    office.MonthEndStarted = 0;
                office.MonthEndStatusId = MonthEndStatus.READY.Id;
                updateMonthEndStatus(office);
            }
            else if (e.CommandName == "GenCheckList")
            {
                //CutoffSalesDiscrepancyReport rpt = ShippingReportManager.Instance.getCutoffSalesDiscrepancyReport(office.OfficeId,vwFiscalPeriod.BudgetYear, vwFiscalPeriod.Period, this.LogonUserId);
                CutoffSalesDiscrepancyReport rpt = AccountReportManager.Instance.getCutoffSalesDiscrepancyReport(office.OfficeId, vwFiscalPeriod.BudgetYear, vwFiscalPeriod.Period, this.LogonUserId);
                AccountManager.Instance.sendMonthEndReport(office, vwFiscalPeriod, rpt, this.LogonUserId);
                //if (updateMonthEndStart)
                //    office.MonthEndStarted = 1;
                office.MonthEndStatusId = MonthEndStatus.VERIFY.Id;
                updateMonthEndStatus(office);
            }
            else if (e.CommandName == "SetMonthEndReady")
            {
                //if (updateMonthEndStart)
                //    office.MonthEndStarted = 1;
                office.MonthEndStatusId = MonthEndStatus.READYTOCAPTURE.Id;
                updateMonthEndStatus(office);
                
                this.forwardToScreen("admin.monthEnd");
            }
            else if (e.CommandName == "CaptureSales")
            {
                //MonthEndSlippageReport rpt = AccountReportManager.Instance.getMonthEndSlippageReport(office.OfficeCode, vwFiscalPeriod.BudgetYear, vwFiscalPeriod.Period, this.LogonUserId);
                //AccountManager.Instance.sendMonthEndReport(office, vwFiscalPeriod, rpt, this.LogonUserId);
                //if (updateMonthEndStart)
                //    office.MonthEndStarted = 1;
                office.MonthEndStatusId = MonthEndStatus.CAPTURED.Id;
                //updateMonthEndStatus(office);
                
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.monthEnd");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, MonthEndCommander.Action.salesCutoff);
                Context.Items.Add("OfficeRef", office);
                Context.Items.Add("FiscalYear", vwFiscalPeriod.BudgetYear);
                Context.Items.Add("Period", vwFiscalPeriod.Period);
                this.forwardToScreen("admin.monthEnd");
            }
            else if (e.CommandName == "SetMonthEndCompleted")
            {
                //if (updateMonthEndStart)
                //    office.MonthEndStarted = 0;
                office.MonthEndStatusId = MonthEndStatus.COMPLETED.Id;
                updateMonthEndStatus(office);
            }
            else
            {   // re run the month end web function
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "admin.monthEnd");
                Context.Items.Add("OfficeRef", office);
                Context.Items.Add("FiscalYear", vwFiscalPeriod.BudgetYear);
                Context.Items.Add("Period", vwFiscalPeriod.Period);
                if (e.CommandName == "SendSlippageMail")
                    Context.Items.Add(WebParamNames.COMMAND_ACTION, MonthEndCommander.Action.sendSlippageMail);
                else if (e.CommandName == "SubmitInterfaceBatch")
                    Context.Items.Add(WebParamNames.COMMAND_ACTION, MonthEndCommander.Action.submitInterfaceBatch);
                this.forwardToScreen("admin.monthEnd");
            }
        }

        protected void btnSetReady_OnClick(object sender, EventArgs arg)
        {
            resetMonthEndStatus();
        }

        private AccountFinancialCalenderDef vwFiscalPeriod
        {
            get { return (AccountFinancialCalenderDef)ViewState["vwFiscalPeriod"]; }
            set { ViewState["vwFiscalPeriod"] = value; }
        }

        private ArrayList vwOfficeList
        {
            get
            {
                if (ViewState["vwOfficeList"] == null)
                {
                    ViewState["vwOfficeList"] = new ArrayList();
                }
                return (ArrayList)ViewState["vwOfficeList"];
            }

            set { ViewState["vwOfficeList"] = value; }
        }
    }
}
