using System;
using System.Collections;
using System.Web;
using com.next.isam.reporter.accounts;
using com.next.isam.appserver.account;
using com.next.isam.appserver.claim;
using com.next.isam.domain.types;
using com.next.common.domain;
using com.next.isam.reporter.helper;
using com.next.common.web.commander;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.appserver.common;
using com.next.isam.domain.common;
using System.Web.UI.WebControls;
using com.next.common.domain.module;

namespace com.next.isam.webapp.claim
{
    public partial class RechargeSupplierDCNote : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageModuleId = ISAMModule.accountDebitCreditNote.Id;
            if (!this.IsPostBack)
            {
                if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.accountDebitCreditNote.Id, ISAMModule.accountDebitCreditNote.SZAccounts))
                {
                    this.ddl_Office.Items.Add(new ListItem("HK", "1"));
                    this.ddl_OrderType.Items.Add(new ListItem("VM", "2"));
                }
                else
                {
                    ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                    ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);

                    this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId");

                    if (officeGroupList.Count == 1)
                    {
                        ReportOfficeGroupRef oref = (ReportOfficeGroupRef)officeGroupList[0];
                        this.ddl_Office.SelectedValue = oref.OfficeGroupId.ToString();
                    }
                    this.ddl_OrderType.Items.Add(new ListItem("-- ALL --", "-1"));
                    this.ddl_OrderType.Items.Add(new ListItem("FOB", "1"));
                    this.ddl_OrderType.Items.Add(new ListItem("VM", "2"));
                    this.radDraft.Checked = true;
                }
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                UKClaimManager.Instance.populateUKClaimRecharge(int.Parse(ddl_Office.SelectedValue));
                ReportClass report = UKClaimManager.Instance.generateUKClaimDCNote(int.Parse(ddl_Office.SelectedValue), int.Parse(ddl_OrderType.SelectedValue), this.radDraft.Checked ? true : false, this.radDraft.Checked ? DateTime.Today : txtIssueDate.DateTime, this.chkSunMacro.Checked, this.LogonUserId);
                ReportHelper.export(report, HttpContext.Current.Response,
                        CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "Next Claim DC Note");
            }
        }

        protected void valIssueDate_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (radOfficial.Checked && txtIssueDate.DateTime == DateTime.MinValue)
                args.IsValid = false;
        }

    }
}
