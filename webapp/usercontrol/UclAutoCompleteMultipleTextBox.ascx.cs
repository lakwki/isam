namespace com.next.isam.webapp.usercontrol
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using com.next.infra.util;

	/// <summary>
	///		Summary description for UclAutoCompleteTextBox.
	/// </summary>
	public partial class UclAutoCompleteMultipleTextBox : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Panel basePanel;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			setDisplay(ConvertUtility.toBool(this.txtIsMultiple.Text));
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

		public void initControl()
		{
			initControl(0, false);
		}

		public void initControl(bool isFirstText)
		{
			initControl(0, isFirstText);
		}

		public void initControl(int width)
		{
			initControl(width, false);
		}

		public void initControl(int width, bool isFirstText)
		{
			this.txt.Attributes.Add("onfocus", "javascript:actb(this,event,'" + this.cbx.ClientID + "'," + isFirstText.ToString().ToLower() + ");");
			this.imgAddMore.Attributes.Add("onclick", "actb_switchPanel(document.all." + this.pnlBase.ClientID + ", document.all." + this.divMultiple.ClientID + ", document.all." + this.txtIsMultiple.ClientID + ");");
			this.txt.Text=this.cbx.selectedText;
			this.txtIsMultiple.Text=bool.FalseString;
			if (width>0)this.txt.Width=width;
		}

		public void bindList(ArrayList items, string dataTextField, string dataValueField, string defaultValue, string optionText, string optionValue)
		{
			cbx.bindList(items, dataTextField, dataValueField, defaultValue, optionText, optionValue);
			this.chkList.DataSource=items;
			this.chkList.DataTextField=dataTextField;
			this.chkList.DataValueField=dataValueField;
			this.chkList.DataBind();
		}

		public Panel PnlBase
		{
			get{return this.pnlBase;}
		}

		public void setDisplay(bool isMultiple)
		{
			WebHelper.formatDisplayStyle(pnlBase, !isMultiple, false);
			WebHelper.formatDisplayStyle(divMultiple, isMultiple, true);
			this.txtIsMultiple.Text=isMultiple.ToString();
		}

		public void setMultiple(bool isMultiple)
		{
			this.imgAddMore.Visible = isMultiple;
		}

		public void selectByValueList(ArrayList list, string propertyName)
		{
			if (list==null)
			{
				this.cbx.SelectedIndex=-1;
				this.txt.Text=cbx.selectedText;
				this.setDisplay(false);
			}
			else if (list.Count==1)
			{
				object propertyValue = ObjectUtility.getPropertyValue(list[0], propertyName);

                this.cbx.selectByValue(propertyValue.ToString());
				this.txt.Text=cbx.selectedText;
				this.setDisplay(false);
			}
			else
			{
				foreach (object obj in list)
				{
					object propertyValue=ObjectUtility.getPropertyValue(obj, propertyName);
					ListItem item=this.chkList.Items.FindByValue(propertyValue.ToString());

					if (item!=null) item.Selected=true;
				}
				this.setDisplay(true);
			}
		}

		public ArrayList getSelectedValues()
		{
			ArrayList list = new ArrayList();

			if (!ConvertUtility.toBool(this.txtIsMultiple.Text))
			{
                list.Add(this.cbx.SelectedValue);
			}
			else
			{
				foreach(ListItem item in this.chkList.Items)
				{
					if (item.Selected) list.Add(item.Value);
				}
			}

			return list;
		}

		public void setRequireFieldValidator(string errMsg, string initialValue)
		{
			this.cVal.Enabled=true;
			this.cVal.ErrorMessage=errMsg;
			vwInitialValue=initialValue;
		}

		public void setRequiredFieldValidatorEnabled(bool isEnable)
		{
			this.cVal.Enabled=isEnable;
		}

		public void setWidth(int px)
		{
			this.txt.Width=px;
		}

		protected void cVal_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (!ConvertUtility.toBool(this.txtIsMultiple.Text))
			{
				if (this.cbx.SelectedValue==this.vwInitialValue )
				{
					args.IsValid=false;
				}
			}
			else
			{
				if (this.chkList.SelectedItem==null)
				{
					args.IsValid=false;
				}
			}
		}

		private string vwInitialValue{set{ViewState["vwInitialValue"]=value;}get{return (string) ViewState["vwInitialValue"];}}
	}
}
