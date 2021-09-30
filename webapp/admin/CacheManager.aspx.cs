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

using com.next.common.domain.module;
using com.next.isam.webapp.usercontrol;

namespace com.next.isam.webapp.admin
{
	public partial class CacheManager : PageTemplate
	{
        protected Repeater Repeater1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			//PageModuleId = AccessMapper.ISAM.SysAdmin.Id;

			// Put user code to initialize the page here

			this.Repeater1.DataSource=com.next.common.datafactory.worker.GeneralWorker.Instance.getAllCache().Values;
			this.Repeater1.DataBind();
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
			this.Repeater1.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.Repeater1_ItemDataBound);

		}
		#endregion

		private void Repeater1_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			DataGrid grd= (DataGrid) e.Item.FindControl("grdData");
			Hashtable tbl = (Hashtable) e.Item.DataItem;

			grd.DataSource=tbl.Values;
			grd.DataBind();
		}
	}
}
