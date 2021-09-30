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
	using com.next.infra.smartwebcontrol;
	using com.next.common.web.commander;

	/// <summary>
	///		Summary description for departmentStaff.
	/// </summary>
	public partial class UclDepartmentStaff : System.Web.UI.UserControl
	{
		public com.next.infra.smartwebcontrol.SmartDropDownList cbxDepartment;
		public com.next.infra.smartwebcontrol.SmartDropDownList cbxStaff;
		private bool isInitControl=true;

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
			this.cbxDepartment.bindList(CommonUtil.getDepartmentList(), "Description", "DepartmentId", null,  this.DepartmentOptionText, this.DepartmentOptionValue);
			cbxDepartment_SelectedIndexChanged(null, null);
			if (cbxDepartment.Visible==false)cbxDepartment.Visible=true;
		}

		public void setOptionValue(string departmentOptionText, string departmentOptionValue,
			string staffOptionText, string staffOptionValue)
		{
            this.DepartmentOptionText=departmentOptionText;
			this.DepartmentOptionValue=departmentOptionValue;
			this.StaffOptionText=staffOptionText;
			this.StaffOptionValue=staffOptionValue;
		}

		protected void cbxDepartment_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (cbxDepartment.SelectedIndex>=0)
			{
				int departmentId = Convert.ToInt32(this.cbxDepartment.SelectedValue);

				this.cbxStaff.bindList(CommonUtil.getUserQListByDepartmentId(departmentId), "DisplayName", "UserId", null, this.StaffOptionText, this.StaffOptionValue);
			}
			else
			{
				this.cbxStaff.bindList(new ArrayList(), "DisplayName", "UserId", null, this.StaffOptionText, this.StaffOptionValue);
			}
		}

		public int DepartmentId
		{
			get
			{
				if (this.cbxDepartment.SelectedIndex>=0)
					return Convert.ToInt32(cbxDepartment.SelectedValue);
				else
					return 0;
			}
			set
			{
				if (value>0)
				{
                    this.cbxDepartment.selectByValue(value.ToString());
					this.cbxDepartment_SelectedIndexChanged(null, null);
				}
			}
		}

		public int StaffId
		{
			get
			{
				if (this.cbxStaff.SelectedIndex>=0)
					return Convert.ToInt32(cbxStaff.SelectedValue);
				else
					return 0;
			}
			set
			{
				if (this.cbxDepartment.Visible==true)
				{
					UserRef userRef=CommonUtil.getUserByKey(value);

					if (userRef !=null && userRef.Department!=null)
					{
						this.initControl();
						this.cbxDepartment.selectByValue(userRef.Department.DepartmentId.ToString());
						cbxDepartment_SelectedIndexChanged(null, null);
						this.cbxStaff.selectByValue(value.ToString());
					}
					else
					{
						throw new Exception("Staff not found");

					}
				}
				else
				{
					this.cbxStaff.selectByValue(value.ToString());
				}
			}
		}

		public bool IsInitControl{
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

		public SmartDropDownList Department
		{
			get
			{
				return this.cbxDepartment;
			}
		}

		public void setDefaultDepartment(int deptid)
		{
			this.initControl();

			for (int i=0; i<this.cbxDepartment.Items.Count;i++)
			{
				if(Convert.ToInt32(cbxDepartment.Items[i].Value)!=deptid)
				{
					this.cbxDepartment.Items.RemoveAt(i);
				}
			}
			this.cbxDepartment.selectByValue(deptid.ToString());
			this.cbxDepartment.Visible=false;
			this.cbxStaff.bindList(CommonUtil.getUserQListByDepartmentId(deptid), "DisplayName", "UserId", null, this.StaffOptionText, this.StaffOptionValue);
		}

		private string DepartmentOptionText
		{
			get
			{
				if (ViewState["DepartmentOptionText"]!=null)
					return (string) ViewState["DepartmentOptionText"];
				else
					return null;
			}
			set{ ViewState["DepartmentOptionText"]=value;}
		}

		private string DepartmentOptionValue
		{
			get
			{
				if (ViewState["DepartmentOptionValue"]!=null)
					return (string) ViewState["DepartmentOptionValue"];
				else
					return null;
			}
			set{ ViewState["DepartmentOptionValue"]=value;}
		}

		private string StaffOptionText
		{
			get
			{
				if (ViewState["StaffOptionText"]!=null)
					return (string) ViewState["StaffOptionText"];
				else
					return null;
			}
			set{ ViewState["StaffOptionText"]=value;}
		}

		private string StaffOptionValue
		{
			get
			{
				if (ViewState["StaffOptionValue"]!=null)
					return (string) ViewState["StaffOptionValue"];
				else
					return null;
			}
			set{ ViewState["StaffOptionValue"]=value;}
		}
	}
}
