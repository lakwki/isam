using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using com.next.common.appserver;

using com.next.infra.util;
using com.next.infra.web;
using com.next.infra.persistency.dataaccess;
using com.next.common.security;
using com.next.common.domain.types;
using com.next.common.domain.module;

namespace com.next.isam.webapp.main
{
    public partial class Login : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Image imgImportant;
		protected com.next.common.web.UclLoginOffice uclLoginOffice;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
                
				string db = DataSourceFactory.GetDataSource("isam").CreateConnection().ConnectionString;
				string nsl = DataSourceFactory.GetDataSource("nsldb").CreateConnection().ConnectionString;
				string industry = DataSourceFactory.GetDataSource("industry").CreateConnection().ConnectionString;

                string livedb = "Server=ns-db04;Database=ISAM;Trusted_Connection=True";
                string livensl = "Server=ns-db04;Database=nsldb;Trusted_Connection=True";
                string liveindustry = "Server=ns-db04;Database=nslindustry;Trusted_Connection=True";


				string pwd ="797836s#s";

                if (db.ToLower() != livedb.ToLower() || nsl.ToLower() != livensl.ToLower() || industry.ToLower() != liveindustry.ToLower())
				{
					this.lblLoginStatus.Text += "<b>ATTENTION: THIS IS DEVELOPMENT SERVER!</b><br>";
					this.lblLoginStatus.Text += "<span style='font-size: 9px;'>";
					this.lblLoginStatus.Text += db.Replace("User ID=sa;Password=","").Replace(pwd, "") + "";
					this.lblLoginStatus.Text += industry.Replace("User ID=sa;Password=","").Replace(pwd, "") + "";
					this.lblLoginStatus.Text += "" + nsl.Replace("User ID=sa;Password=","").Replace(pwd, "") + "</span>";
					this.lblLoginStatus.Visible=true;
				}
                this.checkLoginToken();
				this.checkDomainAccess();
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

		protected void btn_Login_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (Page.IsValid)
			{
				this.createLogin();
			}
		}

		protected void cvalLogin_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
            string[] logins = this.uclLoginOffice.getEmailSuffix().Split('|');
            int userId = -12345;
            for (int i = 0; i < logins.Length; i++)
            {
                userId = SecurityManager.Instance.login(this.txtLoginName.Value + logins[i], this.txtPassword.Value, AppId.ISAM.Code);
                if (userId > 0) break;
            }

   		    if (userId <= 0)
			{
                if (userId == SecurityManager.LoginErrorCode.InvalidPassword.GetHashCode())
                    ((CustomValidator)source).ErrorMessage = "Please enter a valid User Name and Password.";
                else if (userId == SecurityManager.LoginErrorCode.InvalidAccountInfo.GetHashCode())
                    ((CustomValidator)source).ErrorMessage = "Invalid Account Info";
                else if (userId == SecurityManager.LoginErrorCode.InvalidAccountStatus.GetHashCode())
                    ((CustomValidator)source).ErrorMessage = "Invalid Account Status";
                else if (userId == SecurityManager.LoginErrorCode.InvalidUrlAndRedirect.GetHashCode())
                    ((CustomValidator)source).ErrorMessage = "Invalid URL Redirect";
                else if (userId == SecurityManager.LoginErrorCode.LoginIsLocked.GetHashCode())
                    ((CustomValidator)source).ErrorMessage = "Your account has been locked";

				args.IsValid = false;
			}
			else
			{
				vwUserId = userId;
			}
		}

		private void checkLoginToken()
		{
			int userId;
			string token;

			try
			{
				userId = int.Parse(Request.QueryString[Token.KeywordTokenUserId]);
				token = Request.QueryString[Token.KeywordTokenKey];
			}
			catch
			{
				userId = 0;
				token = "";
			}

			if (userId > 0 && token != "")
			{
				if (SecurityManager.Instance.checkAccessibleToken(userId, token))
				{
					vwUserId = userId;
					this.createLogin();
				}
			}
		}

		private void checkDomainAccess()
		{
			int userId = DomainGroupAccess.getAccessUserId(this.Page);

			if (userId > 0)
			{
				vwUserId = userId;
				this.createLogin();
			}
		}

		private void createLogin()
		{
			int userId = vwUserId;
			bool isDebug = Convert.ToBoolean(WebConfig.getValue("appSettings", "debug"));
			string path = "/" + ((string)Request.ServerVariables["PATH_INFO"]).Split('/')[1];

            FormsAuthentication.SetAuthCookie(userId.ToString(), true, path);
            DomainGroupAccess.setAccessUserId(this.Page, userId);

            if (txtPassword.Value.Equals(AdminManager.DEFAULT_STRONG_PWD) && !isDebug)
			{
				Context.Response.Redirect("../personal/ChangePassword.aspx?NewLogin=1");
			}
			else
			{
				FormsAuthentication.RedirectFromLoginPage(userId.ToString(), false, path);
			}

            SecurityManager.Instance.startToken(userId, AppId.ISAM.Code);            
        }

		private int vwUserId
		{
			get {return (int)ViewState["vwUserId"];}
			set {ViewState["vwUserId"] = value;}
		}


	}
}
