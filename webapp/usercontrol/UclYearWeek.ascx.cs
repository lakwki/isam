using System;
using System.Web.UI.WebControls;

namespace com.next.isam.webapp.usercontrol
{
	public partial class UclYearWeek : System.Web.UI.UserControl
	{
		private bool isInitControl=true;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				if (this.isInitControl)
				{
					initControl();
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		public void initControl()
		{
			initControl(false, false);
		}

		public void initControl(bool isAllowAllYear, bool isAllowAllWeek)
		{
			DateTime currentDate = DateTime.Now;
			int currentYear = currentDate.Year;
			if (isAllowAllYear) this.cbxYear.Items.Add(new ListItem("-- All --", "0"));
			for (int i = -5; i <=5; i++)
			{
				this.cbxYear.Items.Add(((int)(currentYear + i)).ToString());
			}

			if (isAllowAllWeek) this.cbxWeek.Items.Add(new ListItem("-- All --", "0"));
            for (int i = 1; i <=52; i++)
    			this.cbxWeek.Items.Add(new ListItem(i.ToString().PadLeft(2,'0'), i.ToString()));

            this.cbxYear.selectByValue(currentYear.ToString());
		}

		public int Year
		{
			get
			{
				return int.Parse(this.cbxYear.SelectedValue);
			}
		}

		public int Week
		{
			get
			{
				return int.Parse(this.cbxWeek.SelectedValue);
			}
		}

        private DateTime getFirstWeekStartDate()
        {
            DateTime d = new DateTime(int.Parse(this.cbxYear.SelectedValue), 1, 1);
            if (d.DayOfWeek != DayOfWeek.Monday)
            {
                while (d.DayOfWeek != DayOfWeek.Monday)
                    d = d.AddDays(-1);
            }
            return d;
        }

        public DateTime FromDate
        {
            get 
            { 
                return this.getFirstWeekStartDate().AddDays(7 * (int.Parse(this.cbxWeek.SelectedValue) - 1));
            }
        }

        public DateTime ToDate
        {
            get 
            {
                return this.getFirstWeekStartDate().AddDays(6).AddDays(7 * (int.Parse(this.cbxWeek.SelectedValue) - 1));
            }
        }

		public bool IsInitControl
		{
			get
			{
				return isInitControl;	
			}
			set	
			{
				if (isInitControl) this.initControl();
				isInitControl=value;
			}
		}
	}
}
