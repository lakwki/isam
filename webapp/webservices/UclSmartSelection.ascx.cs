namespace com.next.isam.webapp.webservices
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;

    using com.next.infra.util;
    using com.next.common.domain;
    using com.next.common.domain.nss;
    using com.next.common.domain.industry.vendor;
    using com.next.common.domain.industry.fabric;
    using com.next.common.web.commander;
    using com.next.isam.webapp.commander;
    using com.next.isam.domain.nontrade;


    /// <summary>
    ///		Summary description for UclSmartSelection.
    /// </summary>
    public partial class UclSmartSelection : System.Web.UI.UserControl
    {

        public event EventHandler SelectionChanged;

        public enum SelectionList
        {
            fabricVendor,
            garmentVendor,
            szVendor,
            articleNo,
            nonClothingVendor,
            productCode,
            garmentVendorForExternalModule,
            contractNo,
            ntVendor,
            user,
            vendorForRecharge,
            ntVendorCompany
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                init();
            }
        }

        private void init()
        {
            // Put user code to initialize the page here
            string webMethod = "";
            string callBackResult = "callBackResultGeneral";

            switch (this.vwSelectionList)
            {
                case SelectionList.articleNo:
                    webMethod = "getArticleNoList";
                    break;
                case SelectionList.fabricVendor:
                    webMethod = "getFabricVendorList";
                    break;
                case SelectionList.garmentVendor:
                    webMethod = "getGarmentVendorList";
                    break;
                case SelectionList.szVendor:
                    webMethod = "getGarmentVendorList";
                    break;
                case SelectionList.nonClothingVendor:
                    webMethod = "getNonClothingVendorList";
                    break;
                case SelectionList.productCode:
                    webMethod = "getProductCodeList";
                    break;
                case SelectionList.garmentVendorForExternalModule:
                    webMethod = "getGarmentVendorListForExternalModule";
                    break;
                case SelectionList.contractNo:
                    webMethod = "getContractNoList";
                    break;
                case SelectionList.ntVendor:
                    webMethod = "getNTVendorList";
                    break;
                case SelectionList.user:
                    webMethod = "getUserList";
                    break;
                case SelectionList.vendorForRecharge:
                    webMethod = "getVendorListForRecharge";
                    break;
                //case SelectionList.ntVendorCompany:
                //    webMethod = "getNTVendorCompanyList";
                //    break;
            }
            txtName.Attributes.Add("onkeydown", "return txtNameOnKeyDown('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "');");

            string groupControlPrefix = string.Empty;

            if (this.vwSmartControlName != null)
            {
                groupControlPrefix = this.ClientID.Replace(this.vwSmartControlName, "");
            }

            if (this.vwSelectionList == SelectionList.articleNo)
                //txtName.Attributes.Add("onkeyup", "return txtNameOnKeyUp('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "', '" + webMethod + "', " + callBackResult + ", typeof(getTxtId('uclFabricSupplier'))!='undefined' ? getTxtId('uclFabricSupplier').value : '');");
                txtName.Attributes.Add("onkeyup", "return txtNameOnKeyUp('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "', '" + webMethod + "', " + callBackResult + ", typeof(getTxtId('" + groupControlPrefix + "uclFabricSupplier'))!='undefined' ? getTxtId('" + groupControlPrefix + "uclFabricSupplier').value : '');");
            else if (this.vwSelectionList == SelectionList.productCode)
                txtName.Attributes.Add("onkeyup", "return txtNameOnKeyUp('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "', '" + webMethod + "', " + callBackResult + ", document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office').value,'"
                    + HttpContext.Current.User.Identity.Name + "', document.getElementById('ctl00_ContentPlaceHolder1_ddl_Department') == null ? '-1' : document.getElementById('ctl00_ContentPlaceHolder1_ddl_Department').value);");
            else if (this.vwSelectionList == SelectionList.vendorForRecharge)
                txtName.Attributes.Add("onkeyup", "return txtNameOnKeyUp('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "', '" + webMethod + "', " + callBackResult + ",document.getElementById('txt_VendorType').value);");
            else if (this.vwSelectionList == SelectionList.ntVendor)
                txtName.Attributes.Add("onkeyup", "return txtNameOnKeyUp('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "', '" + webMethod + "', " + callBackResult + ",document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office') == null ? '-1' : document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office').value,"
                    + "document.getElementById('txt_VendorStatus') == null ? '-1' : document.getElementById('txt_VendorStatus').value);");
            //else if (this.vwSelectionList == SelectionList.ntVendorCompany)
            //    txtName.Attributes.Add("onkeyup", "return txtNameOnKeyUp('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "', '" + webMethod + "', " + callBackResult + ",document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office') == null ? '-1' : document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office').value, document.getElementById('ddl_BusinessEntity').value,"
            //        + "document.getElementById('txt_VendorStatus') == null ? '-1' : document.getElementById('txt_VendorStatus').value);");
            else if (this.vwSelectionList == SelectionList.user)
                txtName.Attributes.Add("onkeyup", "return txtNameOnKeyUp('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "', '" + webMethod + "', " + callBackResult + ", document.getElementById('txt_UserOfficeId') == null ? '-1' : document.getElementById('txt_UserOfficeId').value);");
            else
                txtName.Attributes.Add("onkeyup", "return txtNameOnKeyUp('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "', '" + webMethod + "', " + callBackResult + ");");

            txtName.Attributes.Add("onfocus", "return txtNameOnFocus('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "');");
            txtName.Attributes.Add("onblur", "return txtNameOnBlur('" + this.vwSelectionList.ToString() + "', '" + this.ClientID + "');");

            string str = "<img id='" + this.ClientID + "_imgEraser' src='../images/eraser.bmp' border='0' alt='Clear' style='cursor:hand;' onclick='clearSmartSelection(\"" + this.vwSelectionList.ToString() + "\", \"" + this.ClientID + "\", \"" + this.ClientID + "OnClear\")'>";
            this.lblClear.Text = str;
        }

        public TextBox KeyTextBox
        {
            get
            {
                return this.txtId;
            }
        }

        public bool Enabled
        {
            set
            {
                this.txtName.Enabled = value;
                this.lblClear.Visible = value;
            }
            get
            {
                this.lblClear.Visible = this.txtName.Enabled;
                return this.txtName.Enabled;
            }
        }

        public void clear()
        {
            txtName.Text = "";
            txtOldName.Text = "";
            KeyTextBox.Text = "";
        }

        public void initControl(SelectionList selectionList)
        {
            this.vwSelectionList = selectionList;
            init();
        }

        public void initControl(SelectionList selectionList, string smartControlName)
        {
            this.vwSelectionList = selectionList;
            this.vwSmartControlName = smartControlName;
            init();
        }


        public void setWidth(int pixel)
        {
            this.txtName.Width = pixel;
            this.txtOldName.Width = pixel;
        }

        public int VendorId
        {
            get
            {
                if (this.vwSelectionList != SelectionList.fabricVendor &&
                    this.vwSelectionList != SelectionList.garmentVendor &&
                    this.vwSelectionList != SelectionList.szVendor &&
                    this.vwSelectionList != SelectionList.nonClothingVendor &&
                    this.vwSelectionList != SelectionList.garmentVendorForExternalModule &&
                    this.vwSelectionList != SelectionList.vendorForRecharge)
                    throw new Exception("Selection List is not in Vendor Mode");

                return ConvertUtility.toInt32(this.txtId.Text);
            }
            set
            {
                if (this.vwSelectionList != SelectionList.fabricVendor &&
                    this.vwSelectionList != SelectionList.garmentVendor &&
                    this.vwSelectionList != SelectionList.szVendor &&
                    this.vwSelectionList != SelectionList.nonClothingVendor &&
                    this.vwSelectionList != SelectionList.garmentVendorForExternalModule &&
                    this.vwSelectionList != SelectionList.vendorForRecharge)
                    throw new Exception("Selection List is not in Vendor Mode");

                VendorRef vendorRef = IndustryUtil.getVendorByKey(value);

                if (vendorRef != null)
                {
                    this.txtName.Text = vendorRef.Name;
                    this.txtOldName.Text = vendorRef.Name;
                    this.txtId.Text = vendorRef.VendorId.ToString();
                }
            }
        }

        public int ProductCodeId
        {
            get
            {
                if (this.vwSelectionList != SelectionList.productCode)
                    throw new Exception("Selection List is not in Product Code Mode");

                return ConvertUtility.toInt32(this.txtId.Text);
            }
            set
            {
                if (this.vwSelectionList != SelectionList.productCode)
                    throw new Exception("Selection List is not in Product Code Mode");

                ProductCodeRef pcRef = CommonUtil.getProductCodeByKey(value);

                if (pcRef != null)
                {
                    this.txtName.Text = pcRef.Code + " " + pcRef.Description;
                    this.txtOldName.Text = pcRef.Code + " " + pcRef.Description;
                    this.txtId.Text = pcRef.ProductCodeId.ToString();
                }
            }
        }

        public int NTVendorId
        {
            get
            {
                if (this.vwSelectionList != SelectionList.ntVendor && this.vwSelectionList != SelectionList.ntVendorCompany)
                    throw new Exception("Selection List is not in NTVendor Mode");

                return ConvertUtility.toInt32(this.txtId.Text);
            }
            set
            {
                if (this.vwSelectionList != SelectionList.ntVendor && this.vwSelectionList != SelectionList.ntVendorCompany)
                    throw new Exception("Selection List is not in NTVendor Mode");

                NTVendorDef vendorDef = WebUtil.getNTVendorByKey(value);

                if (vendorDef != null)
                {
                    this.txtName.Text = vendorDef.VendorName;
                    this.txtOldName.Text = vendorDef.VendorName;
                    this.txtId.Text = vendorDef.NTVendorId.ToString();
                }
            }
        }

        public int UserId
        {
            get
            {
                if (this.vwSelectionList != SelectionList.user)
                    throw new Exception("Selection List is not in User Mode");

                return ConvertUtility.toInt32(this.txtId.Text);
            }
            set
            {
                if (this.vwSelectionList != SelectionList.user)
                    throw new Exception("Selection List is not in User Mode");

                if (value == -1)
                {
                    this.txtName.Text = "UNCLASSIFIED";
                    this.txtOldName.Text = "UNCLASSIFIED";
                    this.txtId.Text = "-1";
                }
                else
                {
                    UserRef uRef = CommonUtil.getUserByKey(value);

                    if (uRef != null)
                    {
                        this.txtName.Text = uRef.DisplayName;
                        this.txtOldName.Text = uRef.DisplayName;
                        this.txtId.Text = uRef.UserId.ToString();
                    }
                }
            }

        }


        public string ArticleNo
        {
            get
            {
                if (this.vwSelectionList != SelectionList.articleNo)
                    throw new Exception("Selection List is not in Article No. Mode");

                return txtName.Text;
            }
        }

        public int ContractId
        {
            get
            {
                if (this.vwSelectionList != SelectionList.contractNo)
                    throw new Exception("Selection List is not in Contract No. Mode");

                return ConvertUtility.toInt32(txtId.Text);
            }
            set
            {
                if (this.vwSelectionList != SelectionList.contractNo)
                    throw new Exception("Selection List is not in Contract No. Mode");

                ContractBaseDef cDef = CommonUtil.getContractBaseDefByKey(value);

                if (cDef != null)
                {
                    this.txtName.Text = cDef.ContractNo;
                    this.txtOldName.Text = cDef.ContractNo;
                    this.txtId.Text = cDef.ContractId.ToString();
                }
            }
        }

        public int FabricId
        {
            get
            {
                if (this.vwSelectionList != SelectionList.articleNo)
                    throw new Exception("Selection List is not in Article No. Mode");

                return ConvertUtility.toInt32(this.txtId.Text);
            }
            set
            {
                if (this.vwSelectionList != SelectionList.articleNo)
                    throw new Exception("Selection List is not in Article No. Mode");

                FabricInfoReferenceRef fref = IndustryUtil.getFabricInfoReferenceByKey(value);

                if (fref != null)
                {
                    this.txtName.Text = fref.ArticleNo;
                    this.txtOldName.Text = fref.ArticleNo;
                    this.txtId.Text = fref.FabricId.ToString();
                }
            }
        }

        public bool AutoPostBack
        {
            get
            {
                return txtId.AutoPostBack;
            }
            set
            {
                txtId.AutoPostBack = value;
            }
        }


        protected void txtIdChange(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, EventArgs.Empty);
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

        public HtmlGenericControl DivBase
        {
            get { return divBase; }
        }

        private SelectionList vwSelectionList
        {
            get { return (SelectionList)ViewState["vwSelectionList"]; }
            set { ViewState["vwSelectionList"] = value; }
        }

        private string vwSmartControlName
        {
            get { return (string)ViewState["vwSmartControlName"]; }
            set { ViewState["vwSmartControlName"] = value; }
        }

    }
}
