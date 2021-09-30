using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

using com.next.infra.web;
using com.next.infra.util;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.common.security;
using com.next.common.web.commander;
using com.next.isam.domain.common;
using com.next.isam.appserver.user;
using com.next.isam.webapp.commander;

namespace com.next.isam.webapp.usercontrol
{
	public class PageTemplate : Page
	{
		protected System.Web.UI.WebControls.TextBox StaticPostBackScrollHorizontalPosition;
		protected System.Web.UI.WebControls.TextBox StaticPostBackScrollVerticalPosition;
		protected System.Web.UI.HtmlControls.HtmlImage imgHeader;
		protected System.Web.UI.HtmlControls.HtmlImage imgHeaderText;
		protected System.Web.UI.WebControls.LinkButton btnLabTestReportKeywordSearch;
		protected System.Web.UI.WebControls.ImageButton btnLabTestReportSearch;
		protected System.Web.UI.WebControls.TextBox txtLabTestReportKeywordSearch;
		protected System.Web.UI.WebControls.Label lblSearchReports;

		protected ushort PageModuleId = 0;
		protected readonly int APPID = AccessMapper.ISAM.Id;

		public PageTemplate()
		{
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
			this.Load += new System.EventHandler(this.Page_Load);
			//this.btnSearch.Click+=new EventHandler(btnSearch_Click);
		}
		#endregion

		private void Page_Load(object sender, System.EventArgs e)
		{
            this.Trace.IsEnabled = Convert.ToBoolean(WebConfig.getValue("appSettings", "debug"));

			if (PageModuleId != 0 && !CommonUtil.isAuthenticated(LogonUserId, APPID, PageModuleId))
			{
				Context.Items.Clear();
				Context.Items.Add("result", "Sorry, you are not allowed to access this page.");
				forwardToScreen("main.ErrorPage");
			}

			if (!Page.IsPostBack)
			{
				int logonUserId = LogonUserId;
			}

			//update the latest user status
			SecurityManager.Instance.stayAlive(this.LogonUserId, AppId.ISAM.Code);

            //keep domain cookie alive
            DomainGroupAccess.keepAccessUserId(this.Page, this.LogonUserId);
		}

		protected void setHeaderImage(string imgPath)
		{
			this.imgHeader.Src=imgPath;
			this.imgHeader.Visible=true;
		}

		protected void setHeaderText(string imgTextPath)
		{
			this.imgHeaderText.Src=imgTextPath;
			this.imgHeaderText.Visible=true;
		}

		protected int LogonUserId
		{
            
			get{
                return ConvertUtility.toInt32(Context.Request.ServerVariables["AUTH_USER"]);
            }
		}

		protected OfficeRef LogonUserHomeOffice
		{
			get
			{
                if (this.LogonUserId == 32347) // COP SH Jody Zhao
                    return CommonUtil.getOfficeRefByKey(OfficeId.SH.Id);
                else if (this.LogonUserId == 89919 || this.LogonUserId == 32224 || this.LogonUserId == 89938 || this.LogonUserId == 89211) // flora wu, sindy deng, eva deng, tony cheng
                    return CommonUtil.getOfficeRefByKey(OfficeId.HK.Id);

				UserRef uRef = CommonUtil.getUserByKey(this.LogonUserId);

				if (uRef != null && uRef.Department != null)
					return uRef.Department.Office;
				else
					return null;
			}
		}

		protected ArrayList LogonUserAccessOffices
		{
			get
			{
				if (ViewState["vwLogonUserAccessOffices"] == null)
					ViewState["vwLogonUserAccessOffices"] = WebUtil.getAccessOfficeByUserId(this.LogonUserId);
				return (ArrayList) ViewState["vwLogonUserAccessOffices"];
			}
		}

		protected string ApplPhysicalPath
		{
			get {return Context.Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString();}
		}

		private void setToolTip(Control c)
		{
			foreach (Control ctrl in c.Controls)
			{
				if (ctrl.GetType().BaseType==typeof(WebControl))
				{
					if (ctrl.Controls.Count>0)
					{
						setToolTip(ctrl);
					}
					else
					{
						WebControl wc = (WebControl) ctrl;

						if (wc.ToolTip!="")
						{
							string s="";
							string param1="ABOVE";
							s+="overlib('" + scriptWrapper(wc.ToolTip) + "'";
							if (param1!=null) s+=", " + param1	+ ")";
							wc.Attributes.Add("onmouseover", s);
							wc.Attributes.Add("onmouseout", "return nd();");
							wc.ToolTip="";
						}
					}
				}
			}
		}

		protected void forwardToScreen(string nextScreen)
		{
			Context.Items.Add(WebParamNames.NEXT_SCREEN_PARAM, nextScreen);
			new HttpHandler().ProcessRequest(Context);
		}

		protected void resetPosition()
		{
			this.StaticPostBackScrollHorizontalPosition.Text="0";
			this.StaticPostBackScrollVerticalPosition.Text="0";
		}

		private string scriptWrapper(string s)
		{
			s=s.Replace("\"", "''");
			s=s.Replace("'", "\\'");

			return s;
		}

        protected void ApplicationLinkbutton_Click(object sender, System.EventArgs e)
        {
            string para = SecurityManager.Instance.getAccessibleToken(this.LogonUserId);
            string strLink = ((LinkButton)sender).CommandArgument + para;

            com.next.infra.util.ClientScript.windowOpen(strLink, this.Page);
        }

        // Pre-NOW version
        public string decryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        public string encryptParameter(string param) { return (string.IsNullOrEmpty(param) ? string.Empty : param); }
        public string openPopupWindowFunction = "window.open";

        //NOW version
        //public string decryptParameter(string param) { return WebUtil.DecryptParameter(param); }
        //public string encryptParameter(string param) { return WebUtil.EncryptParameter(param); }
        //public string openPopupWindowFunction = "openPopupWindow";
	}
}
