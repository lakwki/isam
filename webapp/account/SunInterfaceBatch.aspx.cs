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

namespace com.next.isam.webapp.account
{
    public partial class SunInterfaceBatch : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList, true);

                this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId");
                if (officeGroupList.Count == 1)
                {
                    ReportOfficeGroupRef oref = (ReportOfficeGroupRef)officeGroupList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeGroupId.ToString();
                }
                /*
                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_Year.selectByValue(calenderDef.BudgetYear.ToString());
                */
                //this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem("2013", "2013"));
                AccountFinancialCalenderDef calenderDef = CommonUtil.getAccountPeriodByDate(9, DateTime.Today);
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 3).ToString(), (calenderDef.BudgetYear - 3).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 2).ToString(), (calenderDef.BudgetYear - 2).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem((calenderDef.BudgetYear - 1).ToString(), (calenderDef.BudgetYear - 1).ToString()));
                this.ddl_Year.Items.Add(new System.Web.UI.WebControls.ListItem(calenderDef.BudgetYear.ToString(), calenderDef.BudgetYear.ToString()));
                this.ddl_Year.selectByValue(calenderDef.BudgetYear.ToString());
                this.ddl_Period.SelectedIndex = calenderDef.Period - 1;

                this.btn_Process.Attributes.Add("onclick", "return confirm('Are you sure to submit your request?');");
                this.ddl_Period_SelectedIndexChanged(null, null);
                this.btn_Process.Enabled = false;
            }
        }

        protected void btn_Process_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GenerateSunInterfaceBatch);
                Context.Items.Add(AccountCommander.Param.officeId, int.Parse(this.ddl_Office.SelectedValue));
                Context.Items.Add(AccountCommander.Param.fiscalYear, int.Parse(this.ddl_Year.SelectedValue));
                Context.Items.Add(AccountCommander.Param.period, int.Parse(this.ddl_Period.SelectedValue));
                ArrayList list = new ArrayList();
                for (int i = 0; i  < this.cblInterfaceType.Items.Count; i++)
                {
                    if (this.cblInterfaceType.Items[i].Selected && this.cblInterfaceType.Items[i].Text.IndexOf("(UT)") == -1)
                        list.Add(int.Parse(this.cblInterfaceType.Items[i].Value));
                }
                Context.Items.Add(AccountCommander.Param.otherCostSunInterfaceTypeList, list);

                forwardToScreen(null);
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SunInterfaceBatch", "alert('Your request has been submitted to the system.');window.location='GenSunAccountInterfaceFile.aspx';", true);
            }
        }

        protected void valCustom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (AccountManager.Instance.isSunInterfaceBatchGenerated(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), int.Parse(this.ddl_Period.SelectedValue)))
                args.IsValid = false;
        }

        private void refreshList()
        {
            for (int i = cblInterfaceType.Items.Count - 1; i >= 0; i--)
                this.cblInterfaceType.Items.RemoveAt(i);

            int[] sunInterfaceTypeIds = new int[SunInterfaceTypeRef.getSunMacroTypeIdList().Count];
            int idx = 0;
            foreach (int tId in SunInterfaceTypeRef.getSunMacroTypeIdList())
            {
                sunInterfaceTypeIds[idx] = tId;
                idx++;
            }

            ListItem li = null;
            SunInterfaceQueueDef queueDef = null;
            SunInterfaceQueueDef otherQueueDef = null;
            bool isFirst = true;
            string journalNo = String.Empty;

            for (int i = 0; i <= sunInterfaceTypeIds.GetUpperBound(0); i++)
            {
                if ((int.Parse(this.ddl_Office.SelectedValue) == OfficeId.HK.Id || int.Parse(this.ddl_Office.SelectedValue) == OfficeId.SH.Id || int.Parse(this.ddl_Office.SelectedValue) == OfficeId.DG.Id || int.Parse(this.ddl_Office.SelectedValue) == 101) &&
                    (sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.Sales.GetHashCode() || sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.SalesCommission.GetHashCode()))
                {
                    isFirst = true;
                    journalNo = String.Empty;

                    queueDef = AccountManager.Instance.getSunInterfaceQueue(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), int.Parse(this.ddl_Period.SelectedValue), sunInterfaceTypeIds[i], CategoryType.ACTUAL.Id, 1, 2);
                    /*
                    if (queueDef != null && queueDef.Period == 1 && queueDef.FiscalYear == 2013 && queueDef.SubmitTime < (new DateTime(2013, 2, 28)))
                        queueDef = null;
                    */

                    if (queueDef != null && queueDef.JournalNo != String.Empty)
                    {
                        journalNo += (isFirst ? String.Empty : ", ") + "Actual : " + queueDef.JournalNo;
                        isFirst = false;
                    }

                    otherQueueDef = AccountManager.Instance.getSunInterfaceQueue(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), int.Parse(this.ddl_Period.SelectedValue), sunInterfaceTypeIds[i], CategoryType.ACCRUAL.Id, 1, 2);
                    if (otherQueueDef != null && otherQueueDef.JournalNo != String.Empty)
                    {
                        journalNo += (isFirst ? String.Empty : ", ") + "Accrual : " + otherQueueDef.JournalNo;
                        isFirst = false;
                    }

                    otherQueueDef = AccountManager.Instance.getSunInterfaceQueue(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), int.Parse(this.ddl_Period.SelectedValue), sunInterfaceTypeIds[i], CategoryType.REALIZED.Id, 1, 2);
                    if (otherQueueDef != null && otherQueueDef.JournalNo != String.Empty)
                    {
                        journalNo += (isFirst ? String.Empty : ", ") + "Realized : " + otherQueueDef.JournalNo;
                        isFirst = false;
                    }

                    otherQueueDef = AccountManager.Instance.getSunInterfaceQueue(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), int.Parse(this.ddl_Period.SelectedValue), sunInterfaceTypeIds[i], CategoryType.REVERSAL.Id, 1, 2);
                    if (otherQueueDef != null && otherQueueDef.JournalNo != String.Empty)
                    {
                        journalNo += (isFirst ? String.Empty : ", ") + "Reversal : " + otherQueueDef.JournalNo;
                        isFirst = false;
                    }

                    li = new ListItem(SunInterfaceTypeRef.getDescription(sunInterfaceTypeIds[i]) + " (UT) [ " + journalNo + " ]", sunInterfaceTypeIds[i].ToString());
                    li.Selected = (queueDef != null ? false : true);
                    li.Enabled = false;
                    this.cblInterfaceType.Items.Add(li);
                }

                isFirst = true;
                journalNo = String.Empty;
                int uTurn;
                if ((int.Parse(this.ddl_Office.SelectedValue) == OfficeId.HK.Id || int.Parse(this.ddl_Office.SelectedValue) == OfficeId.SH.Id || int.Parse(this.ddl_Office.SelectedValue) == OfficeId.DG.Id || int.Parse(this.ddl_Office.SelectedValue) == 101) &&
                    (sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.Sales.GetHashCode() || sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.SalesCommission.GetHashCode()))
                    uTurn = 2;
                else
                    uTurn = 0;

                queueDef = AccountManager.Instance.getSunInterfaceQueue(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), int.Parse(this.ddl_Period.SelectedValue), sunInterfaceTypeIds[i], CategoryType.ACTUAL.Id, uTurn, 2);
                /*
                if (queueDef != null && queueDef.Period == 1 && queueDef.FiscalYear == 2013 && queueDef.SubmitTime < (new DateTime(2013, 2, 28)))
                    queueDef = null;
                */

                if (queueDef != null && queueDef.JournalNo != String.Empty)
                {
                    journalNo += (isFirst ? String.Empty : ", ") + "Actual : " + queueDef.JournalNo;
                    isFirst = false;
                }

                otherQueueDef = AccountManager.Instance.getSunInterfaceQueue(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), int.Parse(this.ddl_Period.SelectedValue), sunInterfaceTypeIds[i], CategoryType.ACCRUAL.Id, uTurn, 2);
                if (otherQueueDef != null && otherQueueDef.JournalNo != String.Empty)
                {
                    journalNo += (isFirst ? String.Empty : ", ") + "Accrual : " + otherQueueDef.JournalNo;
                    isFirst = false;
                }

                otherQueueDef = AccountManager.Instance.getSunInterfaceQueue(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), int.Parse(this.ddl_Period.SelectedValue), sunInterfaceTypeIds[i], CategoryType.REALIZED.Id, uTurn, 2);
                if (otherQueueDef != null && otherQueueDef.JournalNo != String.Empty)
                {
                    journalNo += (isFirst ? String.Empty : ", ") + "Realized : " + otherQueueDef.JournalNo;
                    isFirst = false;
                }

                otherQueueDef = AccountManager.Instance.getSunInterfaceQueue(int.Parse(this.ddl_Office.SelectedValue), int.Parse(this.ddl_Year.SelectedValue), int.Parse(this.ddl_Period.SelectedValue), sunInterfaceTypeIds[i], CategoryType.REVERSAL.Id, uTurn, 2);
                if (otherQueueDef != null && otherQueueDef.JournalNo != String.Empty)
                {
                    journalNo += (isFirst ? String.Empty : ", ") + "Reversal : " + otherQueueDef.JournalNo;
                    isFirst = false;
                }

                li = new ListItem(SunInterfaceTypeRef.getDescription(sunInterfaceTypeIds[i]) + " [ " + journalNo + " ]", sunInterfaceTypeIds[i].ToString());
                li.Selected = (queueDef != null ? false : true);

                if (sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.Sales.GetHashCode() || sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.SalesCommission.GetHashCode() || sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.Purchase.GetHashCode())
                    li.Enabled = false;
                else
                    li.Enabled = li.Selected;

                if (!(sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.QCC.GetHashCode() && int.Parse(this.ddl_Office.SelectedValue) == OfficeId.SL.Id))
                    this.cblInterfaceType.Items.Add(li);
            }
        }

        protected void ddl_Year_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.refreshList();
        }

        protected void ddl_Period_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.refreshList();
        }

        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.refreshList();
        }
    }
}
