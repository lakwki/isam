namespace com.next.isam.webapp.usercontrol
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for UclAutoCompleteTextBox.
	/// </summary>
	public partial class UclAutoCompleteTextBox : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Panel basePanel;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if (!Page.IsPostBack)
			{
				//initControl();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		public void initControl(bool isFirstText)
		{
			initControl(0, isFirstText);
		}

		public void initControl()
		{
			initControl(0, false);
		}

		public void initControl(int width, bool isFirstText)
		{
			this.txt.Attributes.Add("onfocus", "javascript:actb(this, event, '" + this.cbx.ClientID + "', " + isFirstText.ToString().ToLower() + ");");
			this.txt.Text=this.cbx.selectedText;
			if (width>0)this.txt.Width=width;
		}

		public com.next.infra.smartwebcontrol.SmartDropDownList DropDownList
		{
			get{return this.cbx;}
		}

		public HtmlGenericControl PnlBase
		{
			get{return this.pnlBase;}
		}

		public void selectByValue(string _value)
		{
			this.cbx.selectByValue(_value);
			this.txt.Text=cbx.selectedText;
		}

		public void selectByText(string text)
		{
			this.cbx.selectByText(text);
			this.txt.Text=this.cbx.selectedText;
		}

		public void setRequireFieldValidator(string errMsg, string initalValue)
		{
			this.RequiredFieldValidator1.Enabled=(errMsg != null && errMsg.Trim() != "");
			this.RequiredFieldValidator1.ErrorMessage=errMsg;
			this.RequiredFieldValidator1.InitialValue=initalValue;
		}

		public void setRequiredFieldValidatorEnabled(bool isEnable)
		{
			this.RequiredFieldValidator1.Enabled=isEnable;
		}

		public void setWidth(int px)
		{
			this.txt.Width=px;
		}

		public bool Enabled
		{
			set{txt.Enabled=value;}
		}

		public void setClientOnChangeEvent(string script)
		{
			cbx.Attributes["onClientChange"]=script;
		}
	}
}
