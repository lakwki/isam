using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.next.common.web
{
    public partial class PromptInput : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string title = Request.Params["Title"];
                Page.Title = title;

                string inputField = Request.Params["InputField"];
                string[] inputs = (inputField.Replace("\\n", "<br/>")).Split(char.Parse("|"));

                Table tbl = tab_Input;
                TableCell td;
                TableRow tr;
                Literal lt;
                string dataType;
                string fieldName;
                foreach (string str in inputs)
                {
                    dataType = "TEXT";
                    fieldName = str;
                    if (str[0] == '[')
                    {
                        int pos = str.IndexOf("]");
                        if (pos > 0)
                        {
                            dataType = str.Substring(1, pos - 1);
                            fieldName = str.Substring(pos + 1, str.Length - pos - 1);
                        }
                    }

                    // Create new Row in the table
                    tr = new TableRow();

                    // Prompt message
                    tr.Controls.Add(literalCell("&nbsp;"));
                    tr.Controls.Add(literalCell(fieldName, "FieldLabel2"));
                    tr.Controls.Add(literalCell("&nbsp;"));

                    // Input box
                    TextBox tbx = new TextBox();
                    tbx.Attributes.Add("ControlType", "InputBox");
                    td = new TableCell();
                    td.Controls.Add(tbx);
                    tr.Controls.Add(td);

                    // validation message
                    lt = new Literal();
                    td = new TableCell();
                    Label lbl = new Label();
                    lbl.Visible = true;
                    tbx.Attributes.Add("DataType", dataType);
                    if (dataType == "DECIMAL")
                        lt.Text = "Invalid Number";
                    else if (dataType == "DATE")
                        lt.Text = "Invalid Date";
                    lbl.Attributes.Add("ControlType", "ErrorMessage");
                    lbl.Style.Add(HtmlTextWriterStyle.Color, "Red");
                    lbl.Style.Add(HtmlTextWriterStyle.Display, "None");
                    lbl.Controls.Add(lt);
                    td.Controls.Add(lbl);
                    tr.Controls.Add(td);

                    tbl.Controls.Add(tr);

                    // Add blank row in the table
                    tr = new TableRow();
                    tr.Controls.Add(literalCell("&nbsp;"));
                    tbl.Controls.Add(tr);
                }
            }
        }

        private TableCell literalCell(string text, string css)
        {
            Literal lt;
            TableCell td;
            lt = new Literal();
            lt.Text = text;
            td = new TableCell();
            if (css!=null)
                td.Attributes.Add("class", css);
            td.Controls.Add(lt);
            return td;
        }

        private TableCell literalCell(string text)
        {
            return literalCell(text, null);
        }

        protected void btnComfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "NonTradeSettlement", "returnInput()", true);
        }


        protected void val_Input_validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            bool IsValid=true;
            foreach (TableRow row in tab_Input.Rows)
            {
                TextBox tbx = (TextBox)(row.Controls[1].Controls[0]);
                TableCell validationMsg = (TableCell)(row.Controls[2]);
                string dataType = tbx.Attributes["DataType"];
                if (dataType == "DECIMAL")
                {
                    decimal number;
                    IsValid = !decimal.TryParse(tbx.Text, out number);
                }
                else if(dataType=="DATE")
                {
                    DateTime dt;
                    IsValid = !DateTime.TryParse(tbx.Text, out dt);
                }
                validationMsg.Visible = !IsValid;
            }
            args.IsValid = IsValid;
        }


    
    }
}