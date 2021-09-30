using System;
using System.Web.UI.WebControls;

namespace com.next.isam.webapp.usercontrol
{
	public partial class UclYearMonth : System.Web.UI.UserControl
	{
		private bool isInitControl=true;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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

		public void initControl(bool isAllowAllYear, bool isAllowAllMonth)
		{
			DateTime currentDate = DateTime.Now;
			int currentYear = currentDate.Year;
			if (isAllowAllYear) this.cbxYear.Items.Add(new ListItem("-- All --", "0"));
			for (int i = -5; i <=5; i++)
			{
				this.cbxYear.Items.Add(((int)(currentYear + i)).ToString());
			}

			if (isAllowAllMonth) this.cbxMonth.Items.Add(new ListItem("-- All --", "0"));
			this.cbxMonth.Items.Add(new ListItem("January", "1"));
			this.cbxMonth.Items.Add(new ListItem("February", "2"));
			this.cbxMonth.Items.Add(new ListItem("March", "3"));
			this.cbxMonth.Items.Add(new ListItem("April", "4"));
			this.cbxMonth.Items.Add(new ListItem("May", "5"));
			this.cbxMonth.Items.Add(new ListItem("June", "6"));
			this.cbxMonth.Items.Add(new ListItem("July", "7"));
			this.cbxMonth.Items.Add(new ListItem("August", "8"));
			this.cbxMonth.Items.Add(new ListItem("September", "9"));
			this.cbxMonth.Items.Add(new ListItem("October", "10"));
			this.cbxMonth.Items.Add(new ListItem("November", "11"));
			this.cbxMonth.Items.Add(new ListItem("December", "12"));

			this.cbxYear.selectByValue(currentYear.ToString());
			this.cbxMonth.selectByValue(currentDate.Month.ToString());
		}

		public int Year
		{
			get
			{
				return int.Parse(this.cbxYear.SelectedValue);
			}
		}

		public int Month
		{
			get
			{
				return int.Parse(this.cbxMonth.SelectedValue);
			}
		}

        public DateTime FromDate
        {
            get { return new DateTime(int.Parse(this.cbxYear.SelectedValue), int.Parse(this.cbxMonth.SelectedValue), 1); }
        }

        public DateTime ToDate
        {
            get 
            {
                DateTime d = new DateTime(int.Parse(this.cbxYear.SelectedValue), int.Parse(this.cbxMonth.SelectedValue), 1);
                return d.AddMonths(1).AddDays(-1); 
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
