using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using com.next.isam.webapp.usercontrol;

namespace com.next.isam.webapp.main
{
	public partial class ErrorPage : PageTemplate
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if (!Page.IsPostBack)
			{
				/*this.uclSubHeader.Text="System Message";
				this.uclSubHeader.ImageUrl="../images/blank.gif";*/
                lblErrorMessage.Text = "Sorry, we cannot process your request at this moment.";

				if (Context.Items["result"] != null)
				{
					this.lblErrorMessage.Text= (string) Context.Items["result"];
				}
				else if (Server.GetLastError() != null)
				{
					Exception exp = Server.GetLastError();

					this.lblErrorMessage.Text=exp.Message;

                    if (exp.InnerException != null) this.lblErrorMessage.Text+="<div>"+exp.InnerException.Message+"</div>";
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
