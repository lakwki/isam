using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace com.next.isam.webapp.shipping
{
    public partial class InvoiceRemarkGenerator : System.Web.UI.Page
    {
        DataTable dt_SparePart = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Session["SparePartData"] = null;
                gv_InvRemark.DataSource = LoadDataSet();
                gv_InvRemark.DataBind();
            }
        }

        DataSet LoadDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Contract");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("NoOfPackage");            

            dt.Rows.Add(dt.NewRow());

            ds.Tables.Add(dt);

            return ds;
        }


        protected void RemarkRowCommand(object sender, GridViewCommandEventArgs arg)
        {
            if (arg.CommandName == "Add")
            {
                DataTable dt = GetSparePartInfo(-1);
                dt.Rows.Add(dt.NewRow());

                gv_InvRemark.DataSource = dt;
                gv_InvRemark.DataBind();
            }            
        }

        DataTable GetSparePartInfo(int iRemoveAtIndex)
        {
            dt_SparePart = (DataTable) Session["SparePartData"] ;
            if (dt_SparePart == null)
            {
                dt_SparePart = new DataTable();
                dt_SparePart.Columns.Add("Contract");
                dt_SparePart.Columns.Add("Quantity");
                dt_SparePart.Columns.Add("NoOfPackage");
                dt_SparePart.Columns.Add("QuantityUnit");
                dt_SparePart.Columns.Add("NoOfPackageUnit");
            }

            int count = -1;
            foreach (GridViewRow row in gv_InvRemark.Rows)
            {
                count++;

                if (count == iRemoveAtIndex)
                    continue;

                if (dt_SparePart.Rows.Count <= count)
                    dt_SparePart.Rows.Add(dt_SparePart.NewRow());

                dt_SparePart.Rows[count][0] = ((TextBox)row.Cells[1].Controls[1]).Text;
                dt_SparePart.Rows[count][1] = ((TextBox)row.Cells[2].Controls[1]).Text;
                dt_SparePart.Rows[count][2] = ((TextBox)row.Cells[3].Controls[1]).Text;
                dt_SparePart.Rows[count][3] = ((DropDownList)row.Cells[2].Controls[3]).SelectedValue;
                dt_SparePart.Rows[count][4] = ((DropDownList)row.Cells[3].Controls[3]).SelectedValue;
            }

            if (iRemoveAtIndex != -1)
            {
                if (dt_SparePart.Rows.Count > iRemoveAtIndex)
                    dt_SparePart.Rows.RemoveAt(iRemoveAtIndex);
                if (dt_SparePart.Rows.Count == 0)
                    dt_SparePart.Rows.Add(dt_SparePart.NewRow());
            }
            Session["SparePartData"] = dt_SparePart ;

            return dt_SparePart;
        }

        protected void InvRemarkDataBound(object sender, GridViewRowEventArgs arg)
        {
            if (arg.Row.RowType == DataControlRowType.DataRow )
            {
                if (dt_SparePart != null)
                {
                    if (dt_SparePart.Rows.Count > arg.Row.RowIndex)
                    {
                        ((DropDownList)arg.Row.Cells[2].Controls[3]).SelectedValue = dt_SparePart.Rows[arg.Row.RowIndex][3].ToString();
                        ((DropDownList)arg.Row.Cells[3].Controls[3]).SelectedValue = dt_SparePart.Rows[arg.Row.RowIndex][4].ToString();
                    }
                }
            }
        }


        protected void RemarkRowDeleting(object sender, GridViewDeleteEventArgs  arg)
        {
            //dt_SparePart = (DataTable) Session["SparePartData"];
            //string script = "document.getElementById('lbl_TotalQty').value -=" + dt_SparePart.Rows[arg.RowIndex][1] +";";
            //script += "document.getElementById('txt_TotalPack').value -=" + dt_SparePart.Rows[arg.RowIndex][2] + ";";
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceRemark", script, true);
            DataTable dt = GetSparePartInfo(arg.RowIndex);
            gv_InvRemark.DataSource = dt;
            gv_InvRemark.DataBind();
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Session["SparePartData"] = null;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceRemark", "window.close();", true);
        }

        protected void btn_Paste_Click(object sender, EventArgs e)
        {            
            Session["SparePartData"] = GetSparePartInfo(-1);            
            if (Page.IsValid)
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "invoiceRemark", "window.close();", true);
        }

        protected void ServerValidate(object sender, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = true;
            int num;

            foreach (GridViewRow row in gv_InvRemark.Rows)
            {
                foreach (TableCell tc in row.Cells)
                {
                    foreach (Control ctrl in tc.Controls)
                    {
                        if (ctrl.GetType() == typeof(TextBox))
                        {
                            TextBox tb = (TextBox)ctrl;
                            if (tb.Text == "")
                            {
                                args.IsValid = false;
                                break;
                            }
                            else if (tb.ID.Contains("txt_Quantity") || tb.ID.Contains("txt_NoOfPack"))
                            {
                                if (!Int32.TryParse(tb.Text, out num))
                                {
                                    args.IsValid = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
