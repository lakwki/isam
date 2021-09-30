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
using com.next.infra.web;
using com.next.infra.util;
using System.Globalization;
//using com.next.ecs.domain.types;

namespace com.next.isam.webapp.personal
{
	/// <summary>
	/// Summary description for ChangePassword.
	/// </summary>
	public partial class ChangePassword : com.next.isam.webapp.usercontrol.PageTemplate
	{
		//protected com.next.qcis.webapp.usercontrol.UclSubHeader uclSubHeader;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				/*uclSubHeader.ImageUrl="../images/myhome.gif";
				uclSubHeader.Text="Change Password";*/
                //this.setHeaderText("../images/bannertext_settings.gif");
                //this.setHeaderImage("../images/banner_settings.gif");

				MessageBox.show(this.btnChangePassword, "Are you sure you want to change the password?", MessageBox.Button.OkCancel);

				if (Context.Items.Contains("NewLogin"))
				{
					this.panelNewLogin.Visible=true;
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

		protected void btnChangePassword_Click(object sender, System.EventArgs e)
		{
			this.lblMessage.Visible=false;
			this.lblNoCurrentPassword.Visible=false;
			this.lblNoNewPassword.Visible=false;
			this.lblNoVerifyPassword.Visible=false;
			this.panelNewLogin.Visible=false;

			string currentpassword = this.txtCurrentPwd.Text;
			string newpassword = this.txtNewPwd.Text;
			string verifypassword = this.txtVerifyNewPwd.Text;

			bool isvalid = true;
			if (StringUtility.isEmptyString(currentpassword))
			{
				this.lblNoCurrentPassword.Visible=true;
				isvalid=false;
			}
			if (StringUtility.isEmptyString(newpassword))
			{
				this.lblNoNewPassword.Visible=true;
				isvalid=false;
			}
			if (StringUtility.isEmptyString(verifypassword))
			{
				this.lblNoVerifyPassword.Visible=true;
				isvalid=false;
			}
			if (!isvalid)
			{
				this.lblMessage.Visible=true;
				this.lblMessage.Text="All fields are required";
				return;
			}
			if (isvalid && newpassword.Equals(verifypassword))
			{
				Context.Items.Add(WebParamNames.COMMAND_PARAM, "settings.ChangePassword");
				Context.Items.Add("NewPwd", newpassword);
				Context.Items.Add("OldPwd", currentpassword);
				HttpHandler handler = new HttpHandler();
				handler.ProcessRequest(Context);

				if (Context.Items.Contains("result"))
				{
					int result = (int)Context.Items["result"];
					if (result == 1)
					{
						ClientScript script = new ClientScript();
						script.add("alert('Password changed successfully. Please logout now and re-login with New Password.')");
						script.register(Page);
						Page.Response.Redirect("../main/logout.aspx");
					}
					else if (result == -1)
					{
						this.lblMessage.Visible=true;
						this.lblMessage.Text="User does not exist";
					}
                    else if (result == 0)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = "Password does not match";
                    }
                    else if (result == -2)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = "Password must have at least 8 characters long";
                    }
                    else if (result == -3)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = "Password must contain characters from three of the following four categories:\nEnglish uppercase characters (A through Z)\nEnglish lowercase characters (a through z)\nBase 10 digits (0 through 9)\nNon-alphabetic characters (for example, !, $, #, %, .)";
                    }
                    else if (result == -4)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = "Cannot re-use old passwords";
                    }
					else
					{
						this.lblMessage.Visible=true;
						this.lblMessage.Text="Invalid password";
					}
				}
			}
			else
			{
				this.lblMessage.Visible=true;
				this.lblMessage.Text="New and Re-Type Password do not match";
			}
		}
	}
}
