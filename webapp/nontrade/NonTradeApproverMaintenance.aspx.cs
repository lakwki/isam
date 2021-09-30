using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.infra.util;
using com.next.infra.web;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.account;
using com.next.isam.domain.types;
using com.next.isam.domain.nontrade;
using com.next.isam.appserver.account;

namespace com.next.isam.webapp.nontrade
{
    public partial class NonTradeApproverMaintenance : com.next.isam.webapp.usercontrol.PageTemplate
    {
        ArrayList vwApproverList
        {
            get { return (ArrayList)ViewState["vwApproverList"]; }
            set { ViewState["vwApproverList"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //ArrayList officeList = WebUtil.getNTOfficeList(this.LogonUserId);
                ArrayList officeList = NonTradeManager.Instance.getNTUserOfficeList(this.LogonUserId, NTRoleType.DEPARTMENT_APPROVER_ADMIN.Id, GeneralCriteria.ALL);
                officeList.Sort(new ArrayListHelper.Sorter("OfficeCode"));

                ddl_Office.bindList(officeList, "OfficeCode", "OfficeId", this.LogonUserHomeOffice.OfficeId.ToString());

                txt_Approver.setWidth(300);
                txt_Approver.initControl(webservices.UclSmartSelection.SelectionList.user);

                if (officeList.Count == 0)
                {
                    btn_add.Enabled = false;
                    txt_Approver.Enabled = false;
                    vwApproverList = new ArrayList();
                }
                else
                    vwApproverList = NonTradeManager.Instance.getNTApproverListByOfficeId(officeList);
                gv_Approver.DataSource = vwApproverList;
                gv_Approver.DataBind();
            }
        }

        protected void AddApprover_Click(object sender, EventArgs e)
        {
            NTApproverDef approverDef = new NTApproverDef();

            if (ddl_Office.SelectedValue != "" && txt_Approver.UserId !=int.MinValue)
            {
                approverDef.Office = CommonUtil.getOfficeRefByKey(int.Parse(ddl_Office.SelectedValue));
                approverDef.Approver = CommonUtil.getUserByKey(txt_Approver.UserId);

                NonTradeManager.Instance.updateNTApprover(approverDef, this.LogonUserId);

                vwApproverList.Insert(0, approverDef);

                gv_Approver.DataSource = vwApproverList;
                gv_Approver.DataBind();
            }

        }


        protected void ApproverRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "removeApprover")
            {                
                NTApproverDef approverDef = (NTApproverDef)vwApproverList[Convert.ToInt32(e.CommandArgument)];
                approverDef.Status = GeneralStatus.INACTIVE.Code;

                NonTradeManager.Instance.updateNTApprover(approverDef, this.LogonUserId);

                vwApproverList.Remove(approverDef);
                gv_Approver.DataSource = vwApproverList;
                gv_Approver.DataBind();
            }
        }

        protected void txt_Approver_change(object sender, EventArgs e)
        {
            if (txt_Approver.UserId == int.MinValue)
            {
                row_department.Visible = false;
                return;
            }

            row_department.Visible = true;
            UserRef user = CommonUtil.getUserByKey(txt_Approver.UserId);
            txt_Department.Text = user.Department.OfficeDescription;
        }
    }
}