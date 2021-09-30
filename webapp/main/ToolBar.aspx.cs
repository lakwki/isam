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
using com.next.common.domain;
using com.next.common.web.commander;

namespace com.next.ecs.webapp
{
	/// <summary>
	/// Summary description for ToolBar.
	/// </summary>
	public partial class ToolBar : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here

			int logonUserId = WebHelper.getLogonUserId(Context);
			UserRef userRef=CommonUtil.getUserByKey(logonUserId);

			if (userRef!=null)
			{
				if (userRef.DisplayName.Length>25)
					this.lblDisplayName.Text=userRef.DisplayName.Substring(0, 24) + "...";
				else
					this.lblDisplayName.Text=userRef.DisplayName;
			}
			else
				this.lblDisplayName.Text="N/A";
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
