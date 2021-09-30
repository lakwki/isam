using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types ;


namespace com.next.isam.webapp.usercontrol
{
    public partial class UclProductTeamSelection : System.Web.UI.UserControl
    {
        private int userId = -1;
        private ArrayList vwProductCodeList
        {
            set
            {
                ViewState["Result"] = value;
            }
            get
            {
                return (ArrayList)ViewState["Result"];
            }
        }

        public int UserId
        {
            get { return userId; }
            set { this.userId = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                vwProductCodeList = new ArrayList();
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(-1, OfficeStructureType.PRODUCTCODE.Type);
                cbl_Office.DataSource = userOfficeList;
                cbl_Office.DataMember = "OfficeId";
                cbl_Office.DataTextField = "OfficeCode";
                cbl_Office.DataBind();
                //this.ddl_Office.bindList(userOfficeList, "OfficeCode", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());                
                   
                ddl_ProductTeam.DataSource = CommonUtil.getProductGroupList(userId);
                ddl_ProductTeam.DataBind();
            }
        }

        protected void SelectedList_DataBound(Object Sender, RepeaterItemEventArgs e)
        {
            Label lbl = (Label) e.Item.FindControl("lbl_ProdTeam");
            lbl.Text = vwProductCodeList[e.Item.ItemIndex].ToString();
            ImageButton btn = (ImageButton)e.Item.FindControl("btn_Remove");
            btn.CommandArgument = vwProductCodeList[e.Item.ItemIndex].ToString();
        }

        protected void SelectedList_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                vwProductCodeList.Remove(e.CommandArgument);
                rep_SelectedList.DataSource = vwProductCodeList;
                rep_SelectedList.DataBind();
            }
        }

        public TypeCollector getOfficeList()
        {
            TypeCollector officeList = TypeCollector.Inclusive;
            foreach (ListItem item in cbl_Office.Items)
            {
                if (item.Selected)
                {
                    officeList.append(Convert.ToInt32(item.Value));
                }
            }
            return officeList;
        }

        public TypeCollector getProductCodeList()
        {
            TypeCollector productList = TypeCollector.Inclusive;

            if (vwProductCodeList.Count != 0)
            {
                foreach (object obj in vwProductCodeList)
                {
                    ArrayList codeList =  CommonUtil.getProductCodeListByCode(obj.ToString());
                    foreach (object o in codeList)
                    {
                        //productList.append(
                    }
                }
            }

            return productList;
        }

        protected void btn_Add_Click(object sender, EventArgs e)
        {
            if (vwProductCodeList == null)
                vwProductCodeList = new ArrayList();
            if (!vwProductCodeList.Contains(ddl_ProductTeam.SelectedValue))
                vwProductCodeList.Insert(0, ddl_ProductTeam.SelectedValue);
            rep_SelectedList.DataSource = vwProductCodeList;
            rep_SelectedList.DataBind();
            
        }
    }
}