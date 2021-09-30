using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.next.isam.webapp.usercontrol
{
    public partial class UclSortingOrder : System.Web.UI.UserControl
    {
        public string SortingField
        {
            get
            {
                string field = "";
                foreach (ListItem item in ListBox2.Items)
                {
                    if (field != "")
                        field += ",";
                    field += item.Value;
                }
                return field;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public void addItem(string text, string value, bool isDefault)
        {
            ListItem item = new ListItem(text, value);
            if (isDefault)
                ListBox2.Items.Add(item);
            else
                ListBox1.Items.Add(item);
        }

        public void addItem(string text, string value, bool isDefault, int exclusiveGroupNo)
        {
            ListItem item = new ListItem(text, value);
            item.Attributes.Add("Group", exclusiveGroupNo.ToString());
            if (isDefault)
                ListBox2.Items.Add(item);
            else
                ListBox1.Items.Add(item);
        }


        public void clearAllItems()
        {
            ListBox1.Items.Clear();
            ListBox2.Items.Clear();
        }


        protected void btn_ToRight_Click(object sender, EventArgs e)
        {
            string itemGroup;

            if (ListBox1.SelectedIndex != -1)
            {
                ListItem item = ListBox1.Items[ListBox1.SelectedIndex];
                ListBox1.Items.Remove(item);
                ListBox2.SelectedIndex = -1;

                itemGroup = item.Text.ToUpper().Replace("(ASCENDING)", "").Replace("(DESCENDING)", "").Replace("  "," ").Trim();
                for (int i=0; i < ListBox2.Items.Count; i++)
                    if (string.Compare(ListBox2.Items[i].Text.ToUpper().Replace("(ASCENDING)", "").Replace("(DESCENDING)", "").Replace("  ", " ").Trim(), itemGroup) == 0)
                    {
                        ListBox1.Items.Add(ListBox2.Items[i]);
                        ListBox2.Items.Remove(ListBox2.Items[i]);
                    }
                ListBox2.Items.Add(item);
            }
        }

        protected void btn_ToLeft_Click(object sender, EventArgs e)
        {
            if (ListBox2.SelectedIndex != -1)
            {
                ListItem item = ListBox2.Items[ListBox2.SelectedIndex];
                ListBox2.Items.Remove(item);
                ListBox1.SelectedIndex = -1;
                ListBox1.Items.Add(item);
            }
        }
    }
}