using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using com.next.infra.util;
using com.next.common.security;

namespace com.next.ecs.webapp.usercontrol
{
	/// <summary>
	///		Summary description for UclModuleMenu.
	/// </summary>
	public partial class UclModuleMenu : System.Web.UI.UserControl
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

		public void initMenu(int logonUserId)
		{
			int appId=1;
			ICollection menus = SecurityManager.Instance.getAccessibleMenu(appId, logonUserId);

			this.repMenu.DataSource=menus;
			this.repMenu.DataBind();
		}

		protected void repMenu_ItemDataBound(object source, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			if (WebHelper.isRepeaterNormalItemType(e))
			{
				MenuStruct ms = (MenuStruct) e.Item.DataItem;
				Repeater repSubMenu = (Repeater) e.Item.FindControl("repSubMenu");

				repSubMenu.DataSource=ms.MenuItem;
				repSubMenu.DataBind();
			}
		}

	}
}
