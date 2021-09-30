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
using System.Web.Security;

using com.next.infra.util;
using com.next.common.security;
using com.next.common.domain.types;

namespace com.next.isam.webapp.main
{
	/// <summary>
	/// Summary description for Logout.
	/// </summary>
	public partial class Logout : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			string path = "/" + ((string)Request.ServerVariables["PATH_INFO"]).Split('/')[1];
            HttpCookie c = Request.Cookies[FormsAuthentication.FormsCookieName];

			c.Expires = DateTime.Now.AddDays(-100);
            Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
			Response.Cookies.Add(c);

			DomainGroupAccess.removeAccessUserId(this.Page);

			if (Context.Request.ServerVariables["AUTH_USER"] != null)
			{
				SecurityManager.Instance.endToken(int.Parse(Context.Request.ServerVariables["AUTH_USER"]), AppId.ISAM.Code);
			}

			Response.Redirect("../default.aspx");
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
