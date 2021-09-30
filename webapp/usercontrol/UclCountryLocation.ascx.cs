namespace com.next.ecs.webapp.usercontrol
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using com.next.common.domain;
	using com.next.common.web.commander;

	//	public delegate void EnableHander();
	/// <summary>
	///		Summary description for UclCountryLocation.
	/// </summary>
	public partial class UclCountryLocation : System.Web.UI.UserControl
	{
		public com.next.infra.smartwebcontrol.SmartDropDownList cbxLocation;
		public com.next.infra.smartwebcontrol.SmartDropDownList cbxCountry;
		private bool isInitControl=true;
		private bool _isEnable = true;
		/*		private string _cdt = "";
				private string _cdv = "";
				private string _ldt = "";
				private string _ldv = "";
				private bool _iscon = false;
				private bool _islon = false;*/

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if (!Page.IsPostBack)
			{
				if (this.isInitControl)
				{
					initControl();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		public void initControl()
		{
			ArrayList countryRefs = (ArrayList) CommonUtil.getCountryList();

			this.bindCountry(countryRefs);
			cbxCountry_SelectedIndexChanged(null, null);

			//19/3/2004
			/*if (!this.cbxCountry.Visible) this.cbxCountry.Visible=true;
			if (!this.cbxLocation.Visible) this.cbxLocation.Visible=true;*/

		}

		protected void cbxCountry_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int countryId = Convert.ToInt16(this.cbxCountry.SelectedValue);
			ArrayList locationRefs= (ArrayList) CommonUtil.getLocationList(countryId);

			this.bindLocaton(locationRefs);
		}

		public int CountryId
		{
			get
			{
				if (this.cbxCountry.SelectedIndex>=0)
					return Convert.ToInt32(this.cbxCountry.SelectedValue);
				else
					return 0;
			}
			set
			{
				bool presentCountryState = this.cbxCountry.Visible;
				if (this.isInitControl)this.initControl();
				this.cbxCountry.selectByValue(value.ToString());
				this.cbxCountry_SelectedIndexChanged(null, null);
				this.cbxCountry.Visible=presentCountryState;

			}
		}

		public int LocationId
		{
			get
			{
				if (this.cbxLocation.SelectedIndex>=0)
					return Convert.ToInt32(this.cbxLocation.SelectedValue);
				else
					return 0;
			}
			set
			{
				LocationRef locationRef=CommonUtil.getLocationByKey(value);

				if (locationRef !=null && locationRef.Country!=null)
				{
					CountryId=locationRef.Country.CountryId;
					this.cbxLocation.selectByValue(value.ToString());
				}
				else
				{
					throw new Exception("Location not found");

				}
			}
		}

		public bool IsInitControl
		{
			get
			{
				return isInitControl;
			}
			set
			{
				if (isInitControl) this.initControl();
				isInitControl=value;
			}
		}

		public bool IsEnable
		{
			get
			{
				return _isEnable;
			}
			set
			{
				_isEnable=value;
				this.cbxCountry.Enabled=_isEnable;
				this.cbxLocation.Enabled=_isEnable;
			}
		}

		public void setDefaultCountries(ICollection countryIds)
		{
			if (countryIds==null || countryIds.Count==0) return;
			ArrayList ids = (ArrayList)countryIds;
			for (int i=0; i<this.cbxCountry.Items.Count;i++)
			{
				if(!ids.Contains(Convert.ToInt32(cbxCountry.Items[i].Value)))
				{
					this.cbxCountry.Items.RemoveAt(i);
				}
			}
			if (ids.Count==1) this.cbxCountry.Visible=false;
			//cbxCountry.SelectedValue=Convert.ToString((int)ids[0]);
			cbxCountry.selectByValue(Convert.ToString((int)ids[0]));
//			ArrayList locationRefs= (ArrayList) CommonUtil.getLocationList(countryid);
//			this.bindLocaton(locationRefs);
			this.cbxCountry_SelectedIndexChanged(null, null);
		}

		public void setDefaultCountry(int countryid)
		{
			ArrayList ids = new ArrayList();
			ids.Add(countryid);
			this.setDefaultCountries(ids);
/*
			for (int i=0; i<this.cbxCountry.Items.Count;i++)
			{
				if(Convert.ToInt32(cbxCountry.Items[i].Value)!=countryid)
				{
					this.cbxCountry.Items.RemoveAt(i);
				}
			}
			this.cbxCountry.Visible=false;
			ArrayList locationRefs= (ArrayList) CommonUtil.getLocationList(countryid);
			this.bindLocaton(locationRefs);
*/
		}

		public void setOptionValue(string countryText, string countryValue, string locationText, string locationValue)
		{
			this.CountryOptionText=countryText;
			this.CountryOptionValue=countryValue;
			this.LocationOptionText=locationText;
			this.LocationOptionValue=locationValue;
			this.IsCountryOptionEnable=true;
			this.IsLocationOptionEnabled=true;
		}

		public void enableOptionValue(bool countryOption, bool locationdefault)
		{
			this.IsCountryOptionEnable=countryOption;
			this.IsLocationOptionEnabled=locationdefault;
		}

		private void bindLocaton(ICollection c)
		{
			if (this.IsLocationOptionEnabled && this.LocationOptionValue!=null && this.LocationOptionText!=null)
			{
				this.cbxLocation.bindList(c, "LocationName", "LocationId", "", this.LocationOptionText, this.LocationOptionValue);
			}
			else
			{
				this.cbxLocation.bindList(c, "LocationName", "LocationId");
			}
		}

		private void bindCountry(ICollection c)
		{
			if (this.IsCountryOptionEnable && this.CountryOptionText!=null && this.CountryOptionValue!=null)
			{
				this.cbxCountry.bindList(c, "Name", "CountryId", "", this.CountryOptionText, this.CountryOptionValue);
			}
			else
			{
				this.cbxCountry.bindList(c, "Name", "CountryId");
			}
		}

		private bool IsCountryOptionEnable
		{
			get
			{
				if (ViewState["IsCountryOptionEnable"]!=null)
					return (bool) ViewState["IsCountryOptionEnable"];
				else
					return false;
			}
			set{ ViewState["IsCountryOptionEnable"]=value;}
		}

		private bool IsLocationOptionEnabled
		{
			get
			{
				if (ViewState["IsLocationOptionEnabled"]!=null)
					return (bool) ViewState["IsLocationOptionEnabled"];
				else
					return false;
			}
			set{ ViewState["IsLocationOptionEnabled"]=value;}
		}

		private string LocationOptionText
		{
			get
			{
				if (ViewState["LocationOptionText"]!=null)
					return (string) ViewState["LocationOptionText"];
				else
					return null;
			}
			set{ ViewState["LocationOptionText"]=value;}
		}

		private string LocationOptionValue
		{
			get
			{
				if (ViewState["LocationOptionValue"]!=null)
					return (string) ViewState["LocationOptionValue"];
				else
					return null;
			}
			set{ ViewState["LocationOptionValue"]=value;}
		}

		private string CountryOptionText
		{
			get
			{
				if (ViewState["CountryOptionText"]!=null)
					return (string) ViewState["CountryOptionText"];
				else
					return null;
			}
			set{ ViewState["CountryOptionText"]=value;}
		}

		private string CountryOptionValue
		{
			get
			{
				if (ViewState["CountryOptionValue"]!=null)
					return (string) ViewState["CountryOptionValue"];
				else
					return null;
			}
			set{ ViewState["CountryOptionValue"]=value;}
		}
	}
}
