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

using com.next.common.security;
using com.next.common.domain.module;
using com.next.isam.webapp.usercontrol;

namespace com.next.ecs.webapp.admin
{
	/// <summary>
	/// Summary description for clearMenu.
	/// </summary>
	public partial class ClearMenu : PageTemplate
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			//PageModuleId = AccessMapper.ECS.SysAdmin.Id;
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

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			com.next.common.security.SecurityManager.Instance.notifyChanged();
		}

		protected void Button2_Click(object sender, System.EventArgs e)
		{
			com.next.common.workflow.WorkFlowManager.Instance.notifyChanged(this.LogonUserId);
		}

		protected void Button3_Click(object sender, System.EventArgs e)
		{
			com.next.common.datafactory.worker.GeneralWorker.Instance.notifyChanged();
		}
	}
}
