namespace com.next.ecs.webapp.usercontrol
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using System.ComponentModel;

	/// <summary>
	///		Summary description for UclSubHeader.
	/// </summary>
	public partial class UclSubHeader : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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

		[Bindable(true),
		Category("Appearance"),
		DefaultValue("")]
		public string Text
		{
			set{this.lblSubHeader.Text=value;}
			get{return this.lblSubHeader.Text;}
		}

		[Bindable(true),
		Category("Appearance"),
		DefaultValue("")]
		public string ImageUrl
		{
			set{this.imgSubHeader.Src=value;}
			get{return this.imgSubHeader.Src;}
		}
	}
}
