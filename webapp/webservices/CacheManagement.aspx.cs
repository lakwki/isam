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
using com.next.infra.util;

namespace com.next.isam.webapp.webservices
{
	/// <summary>
	/// Summary description for CacheManagement1.
	/// </summary>
	public partial class CacheManagement1 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here

			if (!Page.IsPostBack)
			{
				ClientScript cs = new ClientScript();

				try
				{
					if (Request.QueryString["securityId"]=="cachemanagement")
					{
						com.next.common.security.SecurityManager.Instance.notifyChanged();
						com.next.common.datafactory.worker.GeneralWorker.Instance.notifyChanged();
						cs.add("alert('Success');");
					}
					else
					{
						cs.add("alert('fail');");

					}
				}
				catch
				{
					cs.add("alert('fail');");

				}

				cs.add("window.close();");
				cs.register(Page);
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
