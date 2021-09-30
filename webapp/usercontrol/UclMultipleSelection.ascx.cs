using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.webapp.usercontrol
{
    public partial class UclMultipleSelection : System.Web.UI.UserControl
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //gv_Item.DataSource = null;
                //gv_Item.DataBind();
            }
        }


        public void bindList(ListItemCollection list)
        {
            gv_Item.DataSource = list;
            gv_Item.DataBind();
        }


        public TypeCollector getSelectedItemId()
        {
            string desc;
            return getSelectedItem(out desc);
        }

        public string getSelectedItemText()
        {
            string desc;
            getSelectedItem(out desc);
            return desc;
        }

        public TypeCollector getSelectedItem(out string SelectedItemText)
        {
            // no item in grid : return empty collector (ALL)
            // Selected nothing : rerutn the item int.MinValue
            TypeCollector ItemList = TypeCollector.Inclusive;
            SelectedItemText = string.Empty;

            foreach (GridViewRow row in gv_Item.Rows)
            {
                CheckBox ckb = (CheckBox)row.FindControl("ckb_Item");
                Label lbl_id = (Label)row.FindControl("lbl_ItemId");
                Label lbl_desc = (Label)row.FindControl("lbl_ItemDesc");
                if (ckb.Checked)
                {
                    ItemList.append(Convert.ToInt32(lbl_id.Text));
                    SelectedItemText += (SelectedItemText == string.Empty ? string.Empty : ", ") + lbl_desc.Text.Trim();
                }
            }
            if (ItemList.Values.Count == gv_Item.Rows.Count)
                SelectedItemText = "ALL";
            else
                if (ItemList.Values.Count == 0)
                    ItemList.append(int.MinValue);  //select nothing
            return ItemList;
        }

        public void setWidth(int width)
        {
            if (width < 20)
                width = 20;
            pnl_Item.Style.Add("width", width.ToString()+"px");
        }

        public void setHeight(int height)
        {
            pnl_Item.Style.Add("heigth", height.ToString()+"px");
        }

        public void setTitleText(string title)
        {
            lbl_Title.Text = title;
        }

        public void enableTitle(bool enable)
        {
            pnl_Title.Style.Add(HtmlTextWriterStyle.Display, (enable ? "block" : "none"));
        }

        public void SetAllCheckBoxStatus(bool selected)
        {
            foreach (GridViewRow row in gv_Item.Rows)
                ((CheckBox)row.FindControl("ckb_Item")).Checked = selected;
        }

        public void SelectAll_OnClick(object sender, EventArgs e)
        {
            SetAllCheckBoxStatus(true);
        }

        public void DeselectAll_OnClick(object sender, EventArgs e)
        {
            SetAllCheckBoxStatus(false);
        }

    }
}