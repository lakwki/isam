using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.infra.util;
using com.next.common.domain.industry.vendor;
using com.next.common.web.commander;
using com.next.isam.domain.nontrade;
using com.next.isam.webapp.commander;

namespace com.next.isam.webapp.webservices
{
    public partial class UclSmartSelectionAJAX : System.Web.UI.UserControl
    {

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

        private void init()
        {
            string webMethod = "";

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
            this.hType.Value = webMethod;

            string str = "<img id='" + this.ClientID + "_imgEraser' src='../images/eraser.bmp' border='0' alt='Clear' style='cursor:hand;' onclick='clearSmartSelection(\"" + this.vwSelectionList.ToString() + "\", \"" + this.ClientID + "\", \"" + this.ClientID + "OnClear\")'>";
            this.lblClear.Text = str;
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                init();
            }
        }

        public void initControl(SelectionList selectionList)
        {
            this.vwSelectionList = selectionList;
            init();
        }

        private SelectionList vwSelectionList
        {
            get { return (SelectionList)ViewState["vwSelectionList"]; }
            set { ViewState["vwSelectionList"] = value; }
        }

        public bool Enabled
        {
            set
            {
                this.txtItem.Enabled = value;
                this.lblClear.Visible = value;
            }
            get
            {
                this.lblClear.Visible = this.txtItem.Enabled;
                return this.txtItem.Enabled;
            }
        }

        public int NTVendorId
        {
            get
            {
                if (this.vwSelectionList != SelectionList.ntVendor && this.vwSelectionList != SelectionList.ntVendorCompany)
                    throw new Exception("Selection List is not in NTVendor Mode");

                return ConvertUtility.toInt32(this.hCode.Value);
            }
            set
            {
                if (this.vwSelectionList != SelectionList.ntVendor && this.vwSelectionList != SelectionList.ntVendorCompany)
                    throw new Exception("Selection List is not in NTVendor Mode");

                NTVendorDef vendorDef = WebUtil.getNTVendorByKey(value);

                if (vendorDef != null)
                {
                    this.txtItem.Text = vendorDef.VendorName;
                    /*
                    this.txtOldName.Text = vendorDef.VendorName;
                    */
                    this.hCode.Value = vendorDef.NTVendorId.ToString();
                }
            }
        }

    }
}