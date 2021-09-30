using System;
using System.Collections;
using System.Web;
using com.next.isam.reporter.accounts;
using com.next.isam.appserver.account;
using com.next.isam.appserver.common;
using com.next.isam.domain.types;
using com.next.common.domain;
using com.next.isam.reporter.helper;
using com.next.common.web.commander;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.domain.common;

namespace com.next.isam.webapp.account
{
    public partial class APAdjustment : com.next.isam.webapp.usercontrol.PageTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.LogonUserId, OfficeStructureType.PRODUCTCODE.Type);
                ArrayList officeGroupList = CommonManager.Instance.getReportOfficeGroupListByAccessibleOfficeIdList(userOfficeList);

                //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId");
                this.ddl_Office.bindList(officeGroupList, "GroupName", "OfficeGroupId");
                //if (userOfficeList.Count == 1)
                if (officeGroupList.Count == 1)
                {
                    //OfficeRef oref = (OfficeRef)userOfficeList[0];
                    ReportOfficeGroupRef oref = (ReportOfficeGroupRef)officeGroupList[0];

                    //this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                    this.ddl_Office.SelectedValue = oref.OfficeGroupId.ToString();
                }

                this.radDraft.Checked = true;
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                ReportClass report = AccountManager.Instance.generateAdjustmentNote(int.Parse(ddl_Office.SelectedValue), AdjustmentType.PURCHASE_ADJUSTMENT.Id, this.radDraft.Checked ? true : false, this.radDraft.Checked ? DateTime.MinValue : txtIssueDate.DateTime, this.LogonUserId);
                ReportHelper.export(report, HttpContext.Current.Response,
                        CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "A/P Adjustment");
            }
        }

        protected void valIssueDate_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (radOfficial.Checked && txtIssueDate.DateTime == DateTime.MinValue)
                args.IsValid = false;
        }

    }
}
