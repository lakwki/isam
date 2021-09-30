using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.module;
using com.next.isam.dataserver.worker;
using com.next.common.web.commander;
using com.next.isam.domain.common;
using com.next.isam.appserver.shipping;
using com.next.isam.domain.types;
using com.next.isam.domain.shipping;
using com.next.isam.domain.product;
using com.next.infra.util;

namespace com.next.isam.webapp.shipping
{
    public partial class LCAppRequest : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private bool HasAccessRightsTo_View;
        private bool HasAccessRightsTo_SuperView;
        private bool HasAccessRightsTo_Submit;
        private ArrayList userOfficeIdList;
        private ArrayList userDepartmentList;
        private ArrayList userProductTeamList;
        private ArrayList COIdList;


        protected void Page_Load(object sender, EventArgs e)
        {
            ArrayList userOfficeList;
            ArrayList coList;
            int UserId;

            userOfficeIdList = new ArrayList();
            userDepartmentList = new ArrayList();
            userProductTeamList = new ArrayList();
            COIdList = new ArrayList();
            
            HasAccessRightsTo_View = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcRequest.Id, ISAMModule.lcRequest.View);
            HasAccessRightsTo_SuperView = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcRequest.Id, ISAMModule.lcRequest.SuperView);
            HasAccessRightsTo_Submit = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcRequest.Id, ISAMModule.lcRequest.Submit);

            //********** For Testing **************************
            //if (this.LogonUserId == 574)
            //    HasAccessRightsTo_SuperView = true;
            //********** For Testing **************************
            UserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);

            txt_VendorName.setWidth(300);
            txt_VendorName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

//            if (!Page.IsPostBack && (HasAccessRightsTo_Submit||HasAccessRightsTo_SuperView||HasAccessRightsTo_View))
            if (!Page.IsPostBack)
                {
                //this.btn_Search.Attributes.Add("onclick", "return alert('Please input At Warehouse Date.');");

                if (HasAccessRightsTo_SuperView)
                    userOfficeList = CommonUtil.getOfficeRefListByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralStatus.ACTIVE.Code);
                else
                    userOfficeList = (ArrayList)CommonUtil.getOfficeListByUserId(UserId, OfficeStructureType.PRODUCTCODE.Type);

                foreach (OfficeRef office in userOfficeList)
                {
                    userOfficeIdList.Add(office.OfficeId);
                }

                //userDepartmentList = CommonUtil.getProductDepartmentAllByUserId(UserId);
                //vsUserDepartmentList = userDepartmentList;
                

                userProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALL, UserId, GeneralCriteria.ALLSTRING);
                vsUserProductTeamList = userProductTeamList;
                ddl_ProductTeam.bindList(userProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
                ddl_ProductTeam_Refresh();

                coList = CommonUtil.getCountryOfOriginList();
                foreach (CountryOfOriginRef CO in coList)
                {
                    COIdList.Add(CO.CountryOfOriginId);
                }

                this.ddl_Office.bindList(userOfficeList, "Description", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
                if (userOfficeList.Count == 1)
                {
                    OfficeRef oref = (OfficeRef)userOfficeList[0];
                    this.ddl_Office.SelectedValue = oref.OfficeId.ToString();
                }

                //this.ddl_Department.bindList(userDepartmentList, "Description", "ProductDepartmentId", "", "--All--", GeneralCriteria.ALL.ToString());
                //if (userDepartmentList.Count == 1) this.ddl_Department.SelectedIndex = 1;

                this.ddl_ProductTeam.bindList(userProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
                if (userProductTeamList.Count == 1) this.ddl_ProductTeam.SelectedIndex = 1;

                this.ddl_ProductTeam_Refresh();
                
                //ddl_Office.Attributes.Add("onchange", "cbxOfficeOnChange();");
                this.ddl_CO.bindList(coList, "Name", "CountryOfOriginId", "", "--All--", GeneralCriteria.ALL.ToString());

                this.btn_Submit.Enabled = HasAccessRightsTo_Submit;
                this.btn_Search.Enabled = (HasAccessRightsTo_View || HasAccessRightsTo_SuperView || HasAccessRightsTo_Submit);
            }

        }

        ArrayList vsLCShipmentList
        {
            get { return (ArrayList)ViewState["LCShipmentList"]; }
            set { ViewState["LCShipmentList"] = value; }
        }

        //ArrayList vsUserDepartmentList
        //{
        //    get { return (ArrayList)ViewState["UserDepartmentList"]; }
        //    set { ViewState["UserDepartmentList"] = value; }
        //}

        ArrayList vsUserProductTeamList
        {
            get { return (ArrayList)ViewState["UserProductTeamList"]; }
            set { ViewState["UserProductTeamList"] = value; }
        }

        protected void btn_Apply_Click(object sender, EventArgs e)
        {
            int i;
            ArrayList officeCodeList = new ArrayList();
            ArrayList appList = new ArrayList();
            
            if (HasAccessRightsTo_Submit)
            {
                foreach (GridViewRow r in gv_LC.Rows)
                {
                    if (r.RowType == DataControlRowType.DataRow)
                    {
                        LCShipmentRef lcShipment = (LCShipmentRef)this.vsLCShipmentList[r.RowIndex];
                        if (((CheckBox)r.FindControl("ckb_LC")).Checked)
                        {
                            LCApplicationDef df = new LCApplicationDef();
                            LCWorker.Instance.LCShipmentCopyToLCApplication(lcShipment, df);
                            appList.Add(df);

                            for (i = 0; i < officeCodeList.Count; i++)
                                if (officeCodeList[i].ToString() == lcShipment.Office.OfficeCode) break;

                            if (i == officeCodeList.Count) officeCodeList.Add(lcShipment.Office.OfficeCode);
                        }
                    }
                }

                if (appList.Count > 0)
                {
                    LCManager.Instance.createLCApplication(appList, this.LogonUserId, officeCodeList);
                }
                this.btn_Search_Click(sender, e);
                btn_Submit.Enabled = true;
            }
        }


        protected void btn_Search_Click(object sender, EventArgs e)
        {
            int TotalNumberOfRecord;
            int NumberOfRecordToGet = 100; // -1 -> unlimit
            

            if (this.Page.IsValid)
            {
                pnl_SearchResult.Visible = true;
                ArrayList LCShipmentList = new ArrayList();
                TotalNumberOfRecord = getLCShipmentList(LCShipmentList, NumberOfRecordToGet);
                this.vsLCShipmentList = LCShipmentList;
                gv_LC.DataSource = LCShipmentList;
                gv_LC.DataBind();
                if (gv_LC.Rows.Count > 0)
                {
                    btn_Submit.Visible = true;
                    btn_SelectAll.Visible = btn_DeselectAll.Visible = true;
                }
                else
                {
                    btn_Submit.Visible = false;
                    btn_SelectAll.Visible = btn_DeselectAll.Visible = false;
                }

                if (TotalNumberOfRecord > NumberOfRecordToGet && NumberOfRecordToGet > 0)
                    lbl_RowCount.Text = "Totally " + TotalNumberOfRecord.ToString() + " shipments found. Only the first " + NumberOfRecordToGet.ToString() + " are shown.";
                else
                    if (TotalNumberOfRecord > 0)
                        lbl_RowCount.Text = TotalNumberOfRecord.ToString() + " shipment" + (TotalNumberOfRecord > 1 ? "s" : "") + " found.";
                    else
                        lbl_RowCount.Text = "";
            }
        }


        protected int getLCShipmentList(ArrayList LCShipmentList, int NumberOfRecordToGet)
        {
            int i;
            int vendorId;
            int coId;
            string itemNo;
            int LcAppNoFrom, LcAppNoTo;
            int lcAppSubmitStatus;

            if (this.txt_VendorName.KeyTextBox.Text == "")
                vendorId = -1;
            else
                vendorId = this.txt_VendorName.VendorId;

            itemNo = this.txt_ItemNo.Text;
            coId = int.Parse(this.ddl_CO.SelectedValue);

            
            ArrayList officeIdList = new ArrayList();
            if (ddl_Office.SelectedValue != "-1")
            {
                officeIdList.Add(Convert.ToInt32(ddl_Office.SelectedValue));
            }
            else
            {
                for (i = 1; i < ddl_Office.Items.Count; i++)
                    officeIdList.Add(Convert.ToInt32(ddl_Office.Items[i].Value));
            }

            ArrayList productTeamIdList = new ArrayList();
            if (ddl_ProductTeam.SelectedValue != "-1")
            {
                productTeamIdList.Add(Convert.ToInt32(ddl_ProductTeam.SelectedValue));
            }
            else
            {
                for (i = 1; i < ddl_ProductTeam.Items.Count; i++)
                    productTeamIdList.Add(Convert.ToInt32(ddl_ProductTeam.Items[i].Value));
            }


            if (this.txt_LCAppNoFrom.Text == "") 
                if (this.txt_LCAppNoTo.Text == "")
                    LcAppNoFrom = int.MinValue;
                else
                    LcAppNoFrom = Convert.ToInt32(this.txt_LCAppNoTo.Text);
            else
                LcAppNoFrom = Convert.ToInt32(this.txt_LCAppNoFrom.Text);

            if (this.txt_LCAppNoTo.Text == "")
                if (this.txt_LCAppNoFrom.Text == "")
                    LcAppNoTo = int.MinValue;
                else
                    LcAppNoTo = Convert.ToInt32(this.txt_LCAppNoFrom.Text);
            else
                LcAppNoTo = Convert.ToInt32(this.txt_LCAppNoTo.Text);

            //OutstandingShipmentOnly = (this.ckb_OutstandingShipmentOnly.Checked ? 1 : 0);
            //lcAppSubmitStatus = Convert.ToInt32(this.ddl_LCAppSubmitStatus.SelectedValue);
            if (this.ckb_OutstandingShipmentOnly.Checked)
                lcAppSubmitStatus = 0;
            else
                lcAppSubmitStatus = -1; // all status

            //this.vsLCShipmentList = LCWorker.Instance.getLCShipment(vendorId, itemNo, officeId, coId, this.txt_AtWHDateFrom.DateTime, this.txt_AtWHDateTo.DateTime);
            return LCWorker.Instance.getLCShipment(LCShipmentList, vendorId, itemNo, officeIdList, productTeamIdList, coId, this.txt_AtWHDateFrom.DateTime, this.txt_AtWHDateTo.DateTime,
                    this.txt_LCAppDateFrom.DateTime, this.txt_LCAppDateTo.DateTime, LcAppNoFrom, LcAppNoTo, lcAppSubmitStatus, NumberOfRecordToGet);
        }



        protected void gv_LC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string sSplitSuffix;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LCShipmentRef lcShipment = (LCShipmentRef)this.vsLCShipmentList[e.Row.RowIndex];
                ((CheckBox)e.Row.FindControl("ckb_LC")).Enabled = false;
                sSplitSuffix = lcShipment.SplitSuffix;
                if (sSplitSuffix==null) sSplitSuffix="";
                ((Label)e.Row.FindControl("lbl_ContractNo")).Text = lcShipment.ContractNo + sSplitSuffix;
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = lcShipment.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = lcShipment.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = lcShipment.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_Destination")).Text = lcShipment.CustomerDestination.DestinationDesc;
                ((Label)e.Row.FindControl("lbl_POAwhDate")).Text = DateTimeUtility.getDateString(lcShipment.SupplierAtWarehouseDate);
                ((Label)e.Row.FindControl("lbl_PackMethod")).Text = lcShipment.PackingMethod.PackingMethodDescription;
                ((Label)e.Row.FindControl("lbl_CO")).Text = lcShipment.CountryOfOrigin.Name;
                ((Label)e.Row.FindControl("lbl_ShipMethod")).Text = lcShipment.ShipmentMethod.ShipmentMethodDescription;
                ((Label)e.Row.FindControl("lbl_PurchaseTerm")).Text = lcShipment.TermOfPurchase.TermOfPurchaseDescription;
                ((Label)e.Row.FindControl("lbl_PurchaseLocation")).Text = lcShipment.PurchaseLocation.PurchaseLocationDescription;
                ((Label)e.Row.FindControl("lbl_PortOfLoad")).Text = lcShipment.ShipmentPort.ShipmentPortDescription;
                ((Label)e.Row.FindControl("lbl_TotalPOQuantity")).Text = lcShipment.TotalPOQuantity.ToString ("N00");
                ((Label)e.Row.FindControl("lbl_ShipmentTotalPOAmount")).Text = lcShipment.ShipmentTotalPOAmount.ToString("N02");
                if (lcShipment.AdvisingBank == null)
                    ((Label)e.Row.FindControl("lbl_AdvisingBank")).Text = "";
                else
                    //((Label)e.Row.FindControl("lbl_AdvisingBank")).Text = CommonWorker.Instance.getBankByKey(lcShipment.BankBranch.BankId).BankName;
                    ((Label)e.Row.FindControl("lbl_AdvisingBank")).Text = lcShipment.AdvisingBank.BankName;

                ((Label)e.Row.FindControl("lbl_ShipmentWorkflowStatus")).Text = lcShipment.ShipmentWorkflowStatus.Name;

                //e.Row.Cells[18].BackColor = System.Drawing.Color.Gray;
                //e.Row.Cells[18].Width = 1;
                //e.Row.Cells[18].BorderStyle = BorderStyle.None;
                //e.Row.Cells[18].BorderWidth = 2;
                //e.Row.Cells[18].BorderColor = System.Drawing.Color.Red;


                if (lcShipment.WorkflowStatus == null)
                {
                    ((Label)e.Row.FindControl("lbl_LCApplicationNo")).Text = "";
                    ((Label)e.Row.FindControl("lbl_LCApplicationTotalPOAmount")).Text = "";
                    ((Label)e.Row.FindControl("lbl_LCApplicationWorkflowStatus")).Text = "";
                }
                else
                {
                    ((Label)e.Row.FindControl("lbl_LCApplicationNo")).Text = lcShipment.LCApplication.LCApplicationNo.ToString().PadLeft(6, char.Parse("0"));
                    ((Label)e.Row.FindControl("lbl_LCApplicationTotalPOAmount")).Text = (lcShipment.TotalPOAmt == decimal.MinValue ? "" : lcShipment.TotalPOAmt.ToString("N02"));
                    ((Label)e.Row.FindControl("lbl_LCApplicationWorkflowStatus")).Text = lcShipment.WorkflowStatus.Name;
                }

                ((Label)e.Row.FindControl("lbl_LCNo")).Text = lcShipment.LCNo;

                if (lcShipment.BankBranch != null && (lcShipment.LCNo.ToString().Trim() == "") && (lcShipment.LCApplication.LCApplicationId <= 0))
                {
                    ((CheckBox)e.Row.FindControl("ckb_LC")).Enabled = true;
                }

            }
            else
                if (e.Row.RowType == DataControlRowType.Separator)
                {
                    e.Row.Cells[18].BackColor = System.Drawing.Color.Yellow;
                    e.Row.Cells[18].Width = 1;
                    e.Row.Cells[18].BorderStyle = BorderStyle.Double;
                    e.Row.Cells[18].BorderWidth = 1;
                    e.Row.Cells[18].BorderColor = System.Drawing.Color.Red;
                    e.Row.Cells[18].Height = 20;
                    e.Row.Cells[18].Visible = false;

                }
        }


        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList ProductTeamList;
            int nOfficeId;
            int nUserId;

            nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);
            nOfficeId = int.Parse(this.ddl_Office.SelectedValue);
            if (nOfficeId == -1) nOfficeId = GeneralCriteria.ALL;

            ProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, nOfficeId, nUserId, GeneralCriteria.ALLSTRING);
            this.ddl_ProductTeam.bindList(ProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
            ddl_ProductTeam_Refresh();

            return;
        }



        protected void ddl_ProductTeam_Refresh()
        {
            if ((this.ddl_Office.SelectedIndex == 0 && this.ddl_Office.Items.Count > 2))
            {
                this.ddl_ProductTeam.SelectedIndex = 0;
                this.ddl_ProductTeam.Enabled = false;
            }
            else
            {
                this.ddl_ProductTeam.Enabled = true;
            }
        }
  

    }
}
