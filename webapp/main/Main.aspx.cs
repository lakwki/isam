using System;

using com.next.infra.web;
using com.next.common.domain.module;
using com.next.common.web.commander;

namespace com.next.isam.webapp.main
{
	public partial class Main : com.next.isam.webapp.usercontrol.PageTemplate
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            /*
			if (Context.Items[WebParamNames.NEXT_SCREEN_PARAM]==null)
			{
				if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ElabModule.testReportEnquiry.Id))
				{
					Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, "testreport.ReportList");
				}
				else if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ElabModule.externalModule.Id))
				{
					Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, "externalresult.MyWorkplace");
				}
				else
				{
					Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, "shipping.Alert");
				}
			}
            */
            /*
            if (this.LogonUserId == 616 || this.LogonUserId == 380 || this.LogonUserId == 1410)
                Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, "shipping.Search");
            else
            */

            if (CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.ukClaimReview.Id))
                Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, "UKClaim.UKClaimApprovalList");
            else
            {
                if (this.LogonUserId == 616)
                    Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, "shipping.Search");
                else
                    Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, "shipping.Alert");
            }
            HttpHandler handler = new HttpHandler();
			handler.ProcessRequest(Context);
			//this.setHeaderImage("../images/banner_labtestreport.gif");
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
