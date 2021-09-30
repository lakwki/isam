namespace com.next.common.web
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using com.next.infra.util;

	/// <summary>
	///		Summary description for UclLoginOffice.
	/// </summary>
	public partial class UclLoginOffice : System.Web.UI.UserControl
	{
		
		private static string ckLoginLocation="ckLoginLocation";
		//protected string Width;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				if (Request.Cookies[ckLoginLocation]!=null)
				{
					this.cbxOffice.selectByValue(Request.Cookies[ckLoginLocation].Value);
				}
			}
			// Put user code to initialize the page here
			/*if (StringUtility.isNotEmptyString(Width))
			{*/
			//	this.cbxOffice.Width=Unit.Parse(Width);
			//}
		}
/*
		public short TabIndex
		{
			set {this.txtName.TabIndex = value;}
			get {return this.TabIndex;}
		}*/

		public string ListWidth
		{
			set {this.cbxOffice.Width = Unit.Parse(value);}
			get {return this.cbxOffice.Width.ToString();}
		}

		public string getEmailSuffix()
		{			
			saveCookie();
			return this.cbxOffice.SelectedValue;
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

		private void saveCookie()
		{
			Response.Cookies[ckLoginLocation].Value = this.cbxOffice.SelectedValue;
			Response.Cookies[ckLoginLocation].Expires=DateTime.Now.AddYears(10);
		}
	}
}
