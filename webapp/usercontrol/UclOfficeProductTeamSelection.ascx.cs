using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.webapp.usercontrol
{
    public partial class UclOfficeProductTeamSelection : System.Web.UI.UserControl
    {
        
        public int UserId
        {
            get { return (int)ViewState["userId"]; }
            set { ViewState["userId"] = value; }
        }
        private ArrayList arr_ProdTeamList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(this.UserId, OfficeStructureType.PRODUCTCODE.Type);

                gv_Office.DataSource = userOfficeList;
                gv_Office.DataBind();

                arr_ProdTeamList = CommonUtil.getProductGroupList(this.UserId);
                gv_ProductTeam.DataSource = arr_ProdTeamList;
                gv_ProductTeam.DataBind();
            }
        }

        protected void ProductTeamRowDataBound(object sender,GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox ckb = (CheckBox)e.Row.Cells[0].FindControl("ckb_ProdTeam");
                ckb.Text = arr_ProdTeamList[e.Row.RowIndex].ToString();
            }
        }

        public TypeCollector getOfficeList()
        {
            TypeCollector officeList = TypeCollector.Inclusive;
            CheckBox ckb;
            Label lbl_id;

            foreach (GridViewRow row in gv_Office.Rows)
            {
                ckb = (CheckBox) row.FindControl("ckb_Office");
                lbl_id = (Label)row.FindControl("lbl_OfficeId");

                if (ckb.Checked)
                {
                    officeList.append(Convert.ToInt32(lbl_id.Text));
                }
            }
            
            return officeList;
        }



        public TypeCollector getProductCodeList()
        {
            TypeCollector prodCodeList = TypeCollector.Inclusive;
            TypeCollector officeList = getOfficeList();
            string code = "";
            CheckBox ckb;

            foreach (int officeId in officeList.Values)
            {
                ArrayList codeList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, officeId, UserId, GeneralCriteria.ALLSTRING);
                foreach (GridViewRow row in gv_ProductTeam.Rows)
                {
                    ckb = (CheckBox)row.FindControl("ckb_ProdTeam");
                    code = ckb.Text;

                    if (ckb.Checked)
                    {
                        foreach (OfficeStructureRef osRef in codeList)
                        {
                            if (osRef.Code != code)
                                continue;
                            else
                            {
                                prodCodeList.append(osRef.OfficeStructureId);
                            }
                        }
                    }                                        
                }
            }

            if (prodCodeList.Values.Count == 0)
                prodCodeList.append(-1);

            return prodCodeList;
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
                            if(ReturnField_ID_CODE_DESC=="DESC")
                                officeList.Add(lbl_desc. Text.Replace("Office","").Trim());
                }
            }
            if (officeList.Count == 0)
                if (ReturnField_ID_CODE_DESC == "ID")
                    officeList.Add(-1);
                else
                    officeList.Add("");

            return officeList;
        }


        public ArrayList getSelectedProductTeam_old(string ReturnField_ID_CODE)
        {
            CheckBox ckb;
            int i,start;
            OfficeStructureRef osRef;
            
            ArrayList productCodeList = null;
            if (ReturnField_ID_CODE == "ID")
            {
                productCodeList = new ArrayList();
                ArrayList officeList = getSelectedOffice("ID");
                foreach (int officeId in officeList)
                {
                    ArrayList codeList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, officeId, UserId, GeneralCriteria.ALLSTRING);
                    start = 0;
                    foreach (GridViewRow row in gv_ProductTeam.Rows)
                    {
                        ckb = (CheckBox)row.FindControl("ckb_ProdTeam");
                        if (ckb.Checked)
                            for (i = start; i < codeList.Count; i++, start++)
                            {   //codeList MUST be ordered by the field 'Code'
                                osRef = (OfficeStructureRef)codeList[i];
                                if (ckb.Text == osRef.Code)
                                    productCodeList.Add(osRef.OfficeStructureId);
                                else
                                    if (string.Compare(ckb.Text, osRef.Code) < 0)
                                        break;
                            }
                    }
                }
                if (productCodeList.Count == 0)
                    productCodeList.Add(-1);
            }
            else
                if (ReturnField_ID_CODE=="CODE")
                {
                    productCodeList = new ArrayList();
                    foreach (GridViewRow row in gv_ProductTeam.Rows)
                        if ((ckb = (CheckBox)row.FindControl("ckb_ProdTeam")).Checked)
                            productCodeList.Add(ckb.Text);
                    if (productCodeList.Count == 0)
                        productCodeList.Add("");
                }
            return productCodeList;
        }


        public ArrayList getSelectedProductTeam(string ReturnField_ID_CODE)
        {
            CheckBox ckb;
            int i, start;
            OfficeStructureRef osRef;
            Boolean found;

            ArrayList productCodeList = null;
            if (ReturnField_ID_CODE == "ID")
            {
                productCodeList = new ArrayList();
                ArrayList officeList = getSelectedOffice("ID");
                foreach (int officeId in officeList)
                {
                    ArrayList codeList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, officeId, UserId, GeneralCriteria.ALLSTRING);
                    start = 0;
                    foreach (GridViewRow row in gv_ProductTeam.Rows)
                    {
                        ckb = (CheckBox)row.FindControl("ckb_ProdTeam");
                        if (ckb.Checked)
                            for (i = start, found=false ; i < codeList.Count; i++, start++)
                            {   //codeList MUST be ordered by the field 'Code'
                                osRef = (OfficeStructureRef)codeList[i];
                                if (ckb.Text == osRef.Code)
                                {
                                    found = true;
                                    productCodeList.Add(osRef.OfficeStructureId);
                                }
                                else
                                    //if (string.Compare(ckb.Text, osRef.Code) < 0)
                                    if (found)
                                        break;
                            }
                    }
                }
                if (productCodeList.Count == 0)
                    productCodeList.Add(-1);
            }
            else
                if (ReturnField_ID_CODE == "CODE")
                {
                    productCodeList = new ArrayList();
                    foreach (GridViewRow row in gv_ProductTeam.Rows)
                        if ((ckb = (CheckBox)row.FindControl("ckb_ProdTeam")).Checked)
                            productCodeList.Add(ckb.Text);
                    if (productCodeList.Count == 0)
                        productCodeList.Add("");
                }
            return productCodeList;
        }



        public int getOfficeTotalCount()
        {
            return gv_Office.Rows.Count;
        }


        public int getProductCodeTotalCount()
        {
            return gv_ProductTeam.Rows.Count;
        }



    }
}