using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.webapp.usercontrol
{
    public partial class UclOfficeSelection : System.Web.UI.UserControl
    {
        public int UserId
        {
            get { return (int)ViewState["userId"]; }
            set { ViewState["userId"] = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.UserId, OfficeStructureType.PRODUCTCODE.Type);

                gv_Office.DataSource = userOfficeList;
                gv_Office.DataBind();
            }
        }

        public TypeCollector getOfficeList()
        {
            TypeCollector officeList = TypeCollector.Inclusive;
            CheckBox ckb;
            Label lbl_id;

            foreach (GridViewRow row in gv_Office.Rows)
            {
                ckb = (CheckBox)row.FindControl("ckb_Office");
                lbl_id = (Label)row.FindControl("lbl_OfficeId");

                if (ckb.Checked)
                {
                    officeList.append(Convert.ToInt32(lbl_id.Text));
                }
            }

            return officeList;
        }

        public ArrayList getSelectedOffice(string ReturnField_ID_CODE_DESC)
        {
            ArrayList officeList = new ArrayList();
            CheckBox ckb;
            Label lbl_id;
            Label lbl_desc;

            foreach (GridViewRow row in gv_Office.Rows)
            {
                ckb = (CheckBox)row.FindControl("ckb_Office");
                lbl_id = (Label)row.FindControl("lbl_OfficeId");
                lbl_desc = (Label)row.FindControl("lbl_OfficeDesc");

                if (ckb.Checked)
                {
                    if (ReturnField_ID_CODE_DESC == "ID")
                        officeList.Add(Convert.ToInt32(lbl_id.Text));
                    else
                        if (ReturnField_ID_CODE_DESC == "CODE")
                            officeList.Add(ckb.Text);
                        else
                            if (ReturnField_ID_CODE_DESC == "DESC")
                                officeList.Add(lbl_desc.Text.Replace("Office", "").Trim());
                }
            }
            if (officeList.Count == 0)
                if (ReturnField_ID_CODE_DESC == "ID")
                    officeList.Add(-1);
                else
                    officeList.Add("");

            return officeList;
        }

        public int getOfficeTotalCount()
        {
            return gv_Office.Rows.Count;
        }

    }
}