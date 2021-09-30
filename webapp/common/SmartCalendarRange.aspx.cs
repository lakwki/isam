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
    public partial class SmartCalendarRange : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string date = "";
                string dateTo = "";
                string target = "";
                string targetTo = "";
                DateTime dt = new DateTime(2000, 1, 1);

                if (Request.QueryString["date"] != null)
                    date = (string)Request.QueryString["date"];

                if (Request.QueryString["dateTo"] != null)
                    dateTo = (string)Request.QueryString["dateTo"];

                if (Request.QueryString["target"] != null)
                    target = (string)Request.QueryString["target"];

                if (Request.QueryString["targetTo"] != null)
                    targetTo = (string)Request.QueryString["targetTo"];

                ViewState["target"] = target;
                ViewState["targetTo"] = targetTo;

                for (int i = 1; i <= 12; i++)
                {
                    cobMonth.Items.Add(new ListItem(dt.ToString("MMMM"), i.ToString()));
                    cobMonthTo.Items.Add(new ListItem(dt.ToString("MMMM"), i.ToString()));
                    dt = dt.AddMonths(1);
                }

                for (int i = DateTime.Now.Year - 10; i <= DateTime.Now.Year + 2; i++)
                {
                    cobYear.Items.Add(i.ToString());
                    cobYearTo.Items.Add(i.ToString());
                }

                try
                {
                    DateTime.Parse(date);
                    myCal.VisibleDate = Convert.ToDateTime(date);
                    myCal.SelectedDate = Convert.ToDateTime(date);
                }
                catch
                {
                    myCal.VisibleDate = DateTime.Now;
                    myCal.SelectedDate = DateTime.Now;
                }

                try
                {
                    DateTime.Parse(dateTo);
                    myCalTo.VisibleDate = Convert.ToDateTime(dateTo);
                    myCalTo.SelectedDate = Convert.ToDateTime(dateTo);
                }
                catch
                {
                    myCalTo.VisibleDate = DateTime.Now;
                }

                setDate(this.myCal.VisibleDate.Month.ToString(), myCal.VisibleDate.Year.ToString());
                setDateTo(this.myCalTo.VisibleDate.Month.ToString(), myCalTo.VisibleDate.Year.ToString());
            }

            this.Label2.Text = DateTime.Now.ToString();
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

            if (this.myCal.VisibleDate > this.myCalTo.VisibleDate)
            {
                this.myCalTo.VisibleDate = new DateTime(int.Parse(iYear), int.Parse(iMonth), 1);
                this.setDateTo(iMonth, iYear);
            }
        }

        private void setDateTo(string iMonth, string iYear)
        {
            ListItem cobMonthItem = cobMonthTo.Items.FindByValue(iMonth);
            ListItem cobYearItem = cobYearTo.Items.FindByValue(iYear);

            if (cobMonthItem != null)
            {
                cobMonthTo.SelectedItem.Selected = false;
                cobMonthItem.Selected = true;
            }

            if (cobYearItem != null)
            {
                cobYearTo.SelectedItem.Selected = false;
                cobYearItem.Selected = true;
            }
        }

        protected void cobMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.myCal.VisibleDate = Convert.ToDateTime("1/" + cobMonth.SelectedItem.Value + "/" + cobYear.SelectedItem.Value);
        }

        protected void cobYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.myCal.VisibleDate = Convert.ToDateTime("1/" + cobMonth.SelectedItem.Value + "/" + cobYear.SelectedItem.Value);
        }

        protected void myCal_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            setDate(e.NewDate.Month.ToString(), e.NewDate.Year.ToString());
        }

        protected void myCalTo_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            setDateTo(e.NewDate.Month.ToString(), e.NewDate.Year.ToString());
        }

        protected void cobMonthTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.myCalTo.VisibleDate = Convert.ToDateTime("1/" + cobMonthTo.SelectedItem.Value + "/" + cobYearTo.SelectedItem.Value);
        }

        protected void cobYearTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.myCalTo.VisibleDate = Convert.ToDateTime("1/" + cobMonthTo.SelectedItem.Value + "/" + cobYearTo.SelectedItem.Value);
        }

        protected void myCal_SelectionChanged(object sender, EventArgs e)
        {
            setDate(myCal.SelectedDate.Month.ToString(), myCal.SelectedDate.Year.ToString());
            this.myCal.VisibleDate = this.myCal.SelectedDate;
        }

        protected void myCal_DayRender(object sender, DayRenderEventArgs e)
        {
            string dayStr = e.Day.Date.ToString("dd/MM/yyyy");
            string target = (string)ViewState["target"];

            if (e.Day.Date.Month == myCal.VisibleDate.Month && e.Day.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                e.Cell.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void myCalTo_DayRender(object sender, DayRenderEventArgs e)
        {
            string dayStr = this.myCal.VisibleDate.ToString("dd/MM/yyyy");
            string dayStrTo = e.Day.Date.ToString("dd/MM/yyyy");
            string target = (string)ViewState["target"];
            string targetTo = (string)ViewState["targetTo"];

            if (e.Day.Date.Month == myCalTo.VisibleDate.Month && e.Day.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                e.Cell.ForeColor = System.Drawing.Color.Red;
            }
            e.Cell.Attributes.Add("onclick", "setDateRange('" + target + "', '" + dayStr + "', '" + targetTo + "', '" + dayStrTo + "')");
            e.Cell.Attributes.Add("style", "cursor: hand;");
        }
    }
}
