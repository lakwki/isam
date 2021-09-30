using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using com.next.common.domain;
using com.next.common.web.commander;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.helper;
using com.next.isam.webapp.commander.account;
using com.next.infra.web;
using com.next.common.appserver;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.domain.module;
using com.next.isam.appserver.common;
using com.next.isam.domain.common;

namespace com.next.isam.webapp.reporter
{
    public partial class OutstandingDiscountListReportPage : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.radNormal.Checked = true;
                this.radNCAll.Checked = true;
                txt_Supplier.setWidth(305);
                txt_Supplier.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
                {
                    this.ddl_Office.Items.Add(new ListItem("HK", "1"));
                    //this.ddl_OrderType.Items.Add(new ListItem("VM", "2"));
                }
                else
                {
                    /*
                    ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);

                    this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "-1", "--- ALL ---", "-1");
                    if (userOfficeList.Count == 1)
                    {
                        OfficeRef oref = (OfficeRef)userOfficeList[0];
                        this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                    }
                    */
                    ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                    ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);

                    this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId", "-1", "--- ALL ---", "-1");
                    this.ddl_HandlingOffice.bindList(CommonManager.Instance.getDGHandlingOfficeList(), "OfficeCode", "OfficeId", "", "-- All --", GeneralCriteria.ALL.ToString());

                    if (officeGroupList.Count == 1)
                    {
                        ReportOfficeGroupRef oref = (ReportOfficeGroupRef)officeGroupList[0];
                        this.ddl_Office.SelectedValue = oref.OfficeGroupId.ToString();
                    }

                    //this.ddl_OrderType.Items.Add(new ListItem("-- ALL --", "-1"));
                    //this.ddl_OrderType.Items.Add(new ListItem("FOB", "1"));
                    //this.ddl_OrderType.Items.Add(new ListItem("VM", "2"));
                }
                this.txt_CutOffDate.DateTime = DateTime.Today;
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                Context.Items.Clear();
                Context.Items.Add(WebParamNames.COMMAND_PARAM, "account");
                Context.Items.Add(WebParamNames.COMMAND_ACTION, AccountCommander.Action.GetOSUKDiscountClaimList);
                Context.Items.Add(AccountCommander.Param.officeId, this.ddl_Office.SelectedValue);
                //Context.Items.Add(AccountCommander.Param.orderType, this.ddl_OrderType.SelectedValue);
                Context.Items.Add(AccountCommander.Param.cutoffDate, this.txt_CutOffDate.DateTime);
                Context.Items.Add(AccountCommander.Param.gbpRate, this.txtGBP.Text.Trim());
                Context.Items.Add(AccountCommander.Param.eurRate, this.txtEUR.Text.Trim());
                Context.Items.Add(AccountCommander.Param.usdAccrualAmt, this.txtUSDAccrual.Text.Trim());
                Context.Items.Add(AccountCommander.Param.gbpAccrualAmt, this.txtGBPAccrual.Text.Trim());
                Context.Items.Add(AccountCommander.Param.eurAccrualAmt, this.txtEURAccrual.Text.Trim());

                int rptOption = 1;
                if (this.radOffice.Checked)
                    rptOption = 2;
                else if (this.radSupplier.Checked)
                    rptOption = 3;

                int ncOption = -1;
                if (this.radNCClaim.Checked)
                    ncOption = 1;
                else if (this.radNCRefund.Checked)
                    ncOption = 2;

                Context.Items.Add(AccountCommander.Param.rptOption, rptOption.ToString());
                Context.Items.Add(AccountCommander.Param.ncOptionId, ncOption.ToString());

                int vendorId = -1;

                if (txt_Supplier.VendorId != int.MinValue)
                {
                    vendorId = txt_Supplier.VendorId;
                }
                Context.Items.Add(AccountCommander.Param.vendorId, vendorId);

                int handlingOfficeId = Convert.ToInt32(ddl_HandlingOffice.SelectedValue);
                string handlingOfficeName = string.Empty;
                if (CommonManager.Instance.getDGOfficeGroupIdList().Contains(Convert.ToInt32(ddl_Office.SelectedItem.Value)))
                    handlingOfficeName = (handlingOfficeId == -1 ? "ALL" : CommonManager.Instance.getDGHandlingOffice(handlingOfficeId).Description);
                Context.Items.Add(AccountCommander.Param.handlingOfficeId, handlingOfficeId);
                Context.Items.Add(AccountCommander.Param.handlingOfficeName, handlingOfficeName);

                forwardToScreen("reporter.OSUKDiscountList");
            }
        }

        protected void valCustom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            decimal d = 0;
            if (string.IsNullOrEmpty(this.txtGBP.Text))
                Page.Validators.Add(new ValidationError("Please input valid GBP rate"));
            else
                if (!decimal.TryParse(this.txtGBP.Text, out d))
                    Page.Validators.Add(new ValidationError("Please input valid GBP rate"));

            if (string.IsNullOrEmpty(this.txtEUR.Text))
                Page.Validators.Add(new ValidationError("Please input valid EUR rate"));
            else
                if (!decimal.TryParse(this.txtEUR.Text, out d))
                    Page.Validators.Add(new ValidationError("Please input valid EUR rate"));

            if (string.IsNullOrEmpty(this.txtUSDAccrual.Text))
                Page.Validators.Add(new ValidationError("Please input valid USD accrual amount"));
            else
                if (!decimal.TryParse(this.txtUSDAccrual.Text, out d))
                    Page.Validators.Add(new ValidationError("Please input valid USD accrual amount"));

            if (string.IsNullOrEmpty(this.txtGBPAccrual.Text))
                Page.Validators.Add(new ValidationError("Please input valid GBP accrual amount"));
            else
                if (!decimal.TryParse(this.txtGBPAccrual.Text, out d))
                    Page.Validators.Add(new ValidationError("Please input valid GBP accrual amount"));

            if (string.IsNullOrEmpty(this.txtEURAccrual.Text))
                Page.Validators.Add(new ValidationError("Please input valid EUR accrual amount"));
            else
                if (!decimal.TryParse(this.txtEURAccrual.Text, out d))
                    Page.Validators.Add(new ValidationError("Please input valid EUR accrual amount"));
        }

        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CommonManager.Instance.getDGOfficeGroupIdList().Contains(Convert.ToInt32(ddl_Office.SelectedItem.Value)))
            {
                tr_HandlingOffice.Style.Add("display", "block");
            }
            else
            {
                tr_HandlingOffice.Style.Add("display", "none");
                ddl_HandlingOffice.SelectedIndex = 0;
            }
        }



    }
}
