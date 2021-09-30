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

namespace com.next.common.web
{
    public partial class SmartCalendar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string date = "";
                string target = "";
                DateTime dt = new DateTime(2000, 1, 1);

                if (Request.QueryString["date"] != null)
                    date = (string)Request.QueryString["date"];

                if (Request.QueryString["target"] != null)
                    target = (string)Request.QueryString["target"];

                ViewState["target"] = target;

                for (int i = 1; i <= 12; i++)
                {
                    cobMonth.Items.Add(new ListItem(dt.ToString("MMMM"), i.ToString()));
                    dt = dt.AddMonths(1);
                }

                for (int i = DateTime.Now.Year - 50; i <= DateTime.Now.Year + 5; i++)
                {
                    cobYear.Items.Add(i.ToString());
                }

                try
                {
                    DateTime.Parse(date);
                    myCal.VisibleDate = Convert.ToDateTime(date);
                }
                catch
                {
                    myCal.VisibleDate = DateTime.Now;
                }

                setDate(this.myCal.VisibleDate.Month.ToString(), myCal.VisibleDate.Year.ToString());
            }
        }

        protected void myCal_DayRender(object sender, DayRenderEventArgs e)
        {
            string dayStr = e.Day.Date.ToString("dd/MM/yyyy");
            string target = (string)ViewState["target"];

            if (e.Day.Date.Month == myCal.VisibleDate.Month && e.Day.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                e.Cell.ForeColor = System.Drawing.Color.Red;
            }
            e.Cell.Attributes.Add("onclick", "setDate('" + target + "', '" + dayStr + "')");
            e.Cell.Attributes.Add("style", "cursor: hand;");
        }

        protected void cobMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.myCal.VisibleDate = Convert.ToDateTime("1/" + cobMonth.SelectedItem.Value + "/" + cobYear.SelectedItem.Value);
            Trace.Warn(this.myCal.VisibleDate.ToString("dd/MM/yyyy") + cobMonth.SelectedItem.Value.ToString());
        }

        protected void cobYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.myCal.VisibleDate = Convert.ToDateTime("1/" + cobMonth.SelectedItem.Value + "/" + cobYear.SelectedItem.Value);
            Trace.Warn(this.myCal.VisibleDate.ToString("dd/MM/yyyy") + cobMonth.SelectedItem.Value.ToString());
        }

        protected void myCal_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            setDate(e.NewDate.Month.ToString(), e.NewDate.Year.ToString());
        }

        private void setDate(string iMonth, string iYear)
        {
            ListItem cobMonthItem = cobMonth.Items.FindByValue(iMonth);
            ListItem cobYearItem = cobYear.Items.FindByValue(iYear);

            if (cobMonthItem != null)
            {
                cobMonth.SelectedItem.Selected = false;
                cobMonthItem.Selected = true;
            }

            if (cobYearItem != null)
            {
                cobYear.SelectedItem.Selected = false;
                cobYearItem.Selected = true;
            }
        }
    }
}
