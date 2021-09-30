using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using com.next.infra.util;

using com.next.common.domain;
using com.next.common.domain.module;
using com.next.common.domain.types;

using com.next.common.datafactory;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.web.commander;

using com.next.infra.web;

using com.next.isam.dataserver.worker;
using com.next.isam.dataserver.model.shipping;
using com.next.isam.appserver.common;
using com.next.isam.appserver.order;
using com.next.isam.appserver.shipping;
using com.next.isam.domain.types;
using com.next.isam.domain.common;
using com.next.isam.domain.shipping;
//using com.next.isam.domain.shipping.lc;
using com.next.isam.domain.product;
using com.next.isam.domain.order;
using com.next.isam.webapp.commander.shipment;
using com.next.isam.webapp.commander;
using com.next.isam.reporter.helper;
using com.next.isam.reporter.lcreport;


namespace com.next.isam.webapp.shipping
{
    public partial class LCAppRequest_Ship : com.next.isam.webapp.usercontrol.PageTemplate
    {

        private bool HasAccessRightsTo_SuperView;
        private bool HasAccessRightsTo_View;
        private bool HasAccessRightsTo_Approve;
        private bool HasAccessRightsTo_Reject;
        private bool HasAccessRightsTo_Apply;
        private ArrayList userOfficeIdList;
        private ArrayList userDepartmentIdList;
        private ArrayList userProductTeamIdList;
        private ArrayList lcApplicationStatusIdList;

        private string sortExpression
        {
            get { return (string)ViewState["SortExpression"]; }
            set { ViewState["SortExpression"] = value; }
        }

        private SortDirection sortDirection
        {
            get { return (SortDirection)ViewState["SortDirection"]; }
            set { ViewState["SortDirection"] = value; }
        }

        private ArrayList vw_otherDelivery
        {
            get { return (ArrayList)ViewState["LCShipmentOtherDelivery"]; }
            set { ViewState["LCShipmentOtherDelivery"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ArrayList userOfficeList;
            ArrayList userDepartmentList;
            ArrayList userProductTeamList;
            ArrayList lcApplicationStatusList;
            int nUserId;

            HasAccessRightsTo_SuperView = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcApplication.Id, ISAMModule.lcApplication.SuperView);
            HasAccessRightsTo_View = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcApplication.Id, ISAMModule.lcApplication.View);
            HasAccessRightsTo_Approve = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcApplication.Id, ISAMModule.lcApplication.Approve);
            HasAccessRightsTo_Reject = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcApplication.Id, ISAMModule.lcApplication.Reject);
            HasAccessRightsTo_Apply = CommonUtil.isAuthenticated(this.LogonUserId, this.APPID, ISAMModule.lcApplication.Id, ISAMModule.lcApplication.Apply);

            //********** For Testing **************************
            //if (this.LogonUserId==574)
            //  HasAccessRightsTo_SuperView = true;
            //********** For Testing **************************
            nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);

            if (!Page.IsPostBack)
            {
                int i;

                userOfficeIdList = new ArrayList();
                userDepartmentIdList = new ArrayList();
                userProductTeamIdList = new ArrayList();
                lcApplicationStatusIdList = new ArrayList();

                btn_LCNewApplicationSummary.Width = 200;
                txt_SupplierName.setWidth(300);
                txt_SupplierName.initControl(com.next.isam.webapp.webservices.UclSmartSelection.SelectionList.garmentVendor);

                userOfficeList = CommonUtil.getOfficeListByUserId(nUserId, GeneralCriteria.ALL);
                //userOfficeList = CommonUtil.getOfficeRefListByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralStatus.ACTIVE.Code);
                //userDepartmentList = CommonUtil.getProductDepartmentAllByUserId(nUserId);
                userDepartmentList = CommonUtil.getProductDepartmentListByCriteria(nUserId, GeneralCriteria.ALL, GeneralCriteria.ALL);

                userProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, GeneralCriteria.ALL, nUserId, GeneralCriteria.ALLSTRING);
                lcApplicationStatusList = LCWFS.getCollectionValues();
                for (i = 0; i < lcApplicationStatusList.Count; i++)
                    if (((LCWFS)lcApplicationStatusList[i]).Id == LCWFS.REJECTED.Id) lcApplicationStatusList.RemoveAt(i);

                vsUserDepartmentList = userDepartmentList;
                vsUserProductTeamList = userProductTeamList;

                this.ddl_Office.bindList(userOfficeList, "Description", "OfficeId", "", "--All--", GeneralCriteria.ALL.ToString());
                if (userOfficeList.Count == 1) this.ddl_Office.SelectedIndex = 1;

                this.ddl_Department.bindList(userDepartmentList, "Description", "ProductDepartmentId", "", "--All--", GeneralCriteria.ALL.ToString());
                if (userDepartmentList.Count == 1) this.ddl_Department.SelectedIndex = 1;

                this.ddl_ProductTeam.bindList(userProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
                if (userProductTeamList.Count == 1) this.ddl_ProductTeam.SelectedIndex = 1;

                this.ddl_ProductTeam_Refresh();

                this.ddl_LcApplicationStatus.bindList(lcApplicationStatusList, "Name", "Id", "");

                this.btn_Search.Enabled = (HasAccessRightsTo_View || HasAccessRightsTo_SuperView || HasAccessRightsTo_Approve || HasAccessRightsTo_Reject || HasAccessRightsTo_Apply);
            }

            this.ddl_Department.Enabled = (this.ddl_Office.SelectedIndex != 0);
            if (!this.ddl_Department.Enabled) this.ddl_Department.SelectedIndex = 0;
            //this.ddl_ProductTeam.Enabled = (this.ddl_Office.SelectedIndex != 0);
        }

        ArrayList vwLCApplicationList
        {
            get { return (ArrayList)ViewState["LCApplicationList"]; }
            set { ViewState["LCApplicationList"] = value; }
        }

        ArrayList vsUserDepartmentList
        {
            get { return (ArrayList)ViewState["UserDepartmentList"]; }
            set { ViewState["UserDepartmentList"] = value; }
        }

        ArrayList vsUserProductTeamList
        {
            get { return (ArrayList)ViewState["UserProductTeamList"]; }
            set { ViewState["UserProductTeamList"] = value; }
        }

        protected void btn_Search_Click(object sender, EventArgs e)
        {
             pnl_SearchResult.Visible = true;

            gv_LC.DataSource = getLCApplicationList();
            gv_LC.DataBind();
            
            refreshButtons();

            if (this.LogonUserId == 574)
            {
                //this.btn_ResendSubmitNotice.Enabled = true;
                this.pnl_DebugArea.Visible = true;
                this.txt_DebugMessage.Text = "";
            }

        }

        protected void refreshButtons()
        {
            LCWFS Status;
            Status = LCWFS.getType(int.Parse(this.ddl_LcApplicationStatus.SelectedValue));
            if (Status.Id == LCWFS.NEW.Id)
            {
                this.btn_Approve.Enabled = HasAccessRightsTo_Approve && true;
                this.btn_Reject.Enabled = HasAccessRightsTo_Reject && true;
                this.btn_Apply.Enabled = HasAccessRightsTo_Apply && false;
                this.btn_UpdateLCInfo.Enabled = (HasAccessRightsTo_Approve && HasAccessRightsTo_Reject && HasAccessRightsTo_Apply);
            }
            else if (Status.Id == LCWFS.APPROVED.Id)
            {
                this.btn_Approve.Enabled = HasAccessRightsTo_Approve && false;
                this.btn_Reject.Enabled = HasAccessRightsTo_Reject && true;
                this.btn_Apply.Enabled = HasAccessRightsTo_Apply && true;
                this.btn_UpdateLCInfo.Enabled = false;
            }
            else if (Status.Id == LCWFS.REJECTED.Id)
            {
                this.btn_Approve.Enabled = HasAccessRightsTo_Approve && true;
                this.btn_Reject.Enabled = HasAccessRightsTo_Reject && false;
                this.btn_Apply.Enabled = HasAccessRightsTo_Apply && false;
                this.btn_UpdateLCInfo.Enabled = false;
            }
            else
            {
                this.btn_Approve.Enabled = HasAccessRightsTo_Approve && false;
                this.btn_Reject.Enabled = HasAccessRightsTo_Reject && false;
                this.btn_Apply.Enabled = HasAccessRightsTo_Apply && false;
                this.btn_UpdateLCInfo.Enabled = false;
            }
        }

        protected ArrayList getLCApplicationList()
        {
            int vendorId;
            string LcAppNoFrom, LcAppNoTo;
            DateTime LcAppDateFrom, LcAppDateTo;
            int i;

            if (this.txt_SupplierName.KeyTextBox.Text == "")
                vendorId = -1;
            else
                vendorId = this.txt_SupplierName.VendorId;

            TypeCollector OfficeIdList = TypeCollector.Inclusive;
            if (this.ddl_Office.SelectedValue == "-1")
                for (i = 1; i < this.ddl_Office.Items.Count; i++)
                    OfficeIdList.append(int.Parse(this.ddl_Office.Items[i].Value));
            else
                OfficeIdList.append(int.Parse(this.ddl_Office.SelectedValue));

            TypeCollector DeptIdList = TypeCollector.Inclusive;
            if (this.ddl_Department.SelectedValue == "-1")
                for (i = 0; i < this.ddl_Department.Items.Count; i++)
                    DeptIdList.append(int.Parse(this.ddl_Department.Items[i].Value));
            else
                DeptIdList.append(int.Parse(this.ddl_Department.SelectedValue));

            TypeCollector ProdTeamIdList = TypeCollector.Inclusive;
            if (this.ddl_ProductTeam.SelectedValue == "-1")
                for (i = 0; i < this.ddl_ProductTeam.Items.Count; i++)
                    ProdTeamIdList.append(int.Parse(this.ddl_ProductTeam.Items[i].Value));
            else
                ProdTeamIdList.append(int.Parse(this.ddl_ProductTeam.SelectedValue));

            TypeCollector WorkflowStatusIdList = TypeCollector.Inclusive;
            if (this.ddl_LcApplicationStatus.SelectedValue == "-1")
                for (i = 1; i < this.ddl_LcApplicationStatus.Items.Count; i++)
                    WorkflowStatusIdList.append(int.Parse(ddl_LcApplicationStatus.Items[i].Value));
            else
                WorkflowStatusIdList.append(int.Parse(ddl_LcApplicationStatus.SelectedValue));

            LcAppDateFrom = (this.txt_LCAppDateFrom.DateTime);
            LcAppDateTo = (this.txt_LCAppDateTo.DateTime);

            if (this.txt_LCAppNoFrom.Text == "")
                LcAppNoFrom = this.txt_LCAppNoTo.Text;
            else
                LcAppNoFrom = this.txt_LCAppNoFrom.Text;

            if (this.txt_LCAppNoTo.Text == "")
                LcAppNoTo = this.txt_LCAppNoFrom.Text;
            else
                LcAppNoTo = this.txt_LCAppNoTo.Text;

            this.vwLCApplicationList = LCWorker.Instance.getLCApplicationShipment(vendorId, OfficeIdList, DeptIdList, ProdTeamIdList, WorkflowStatusIdList, LcAppDateFrom, LcAppDateTo, LcAppNoFrom, LcAppNoTo);

            return this.vwLCApplicationList;
        }


        protected void gv_LC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string sAppNo;
            string sLCBatchNo, sFormattedLCBatchNo;
            string sGroupId;
            int BankId;
            int nStatus;
            string sSplitSuffix;
            //string sYear;
            //LCBatchRef refLCBatch;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                LCShipmentRef lcApp = (LCShipmentRef)this.vwLCApplicationList[e.Row.RowIndex];
                sAppNo = lcApp.LCApplication.LCApplicationNo.ToString();
                if ((lcApp.LCBatch == null ? true : (lcApp.LCBatch.LCBatchId <= 0)))
                {
                    sLCBatchNo = "0";
                    sFormattedLCBatchNo = "";
                    sGroupId = "";
                }
                else
                {
                    sLCBatchNo = lcApp.LCBatch.LCBatchNo.ToString();
                    sFormattedLCBatchNo = "LCB" + sLCBatchNo.PadLeft(6, char.Parse("0"));
                    if (lcApp.LCBatch.GroupId < 0)
                        sGroupId = "";
                    else
                        sGroupId = lcApp.LCBatch.GroupId.ToString();
                }
                ((CheckBox)e.Row.FindControl("ckb_LC")).Enabled = true;
                ((Label)e.Row.FindControl("lbl_ApplicationNo")).Text = sAppNo.PadLeft(6, char.Parse("0"));
                ((Label)e.Row.FindControl("lbl_OfficeCode")).Text = lcApp.Office.OfficeCode;
                ((Label)e.Row.FindControl("lbl_Supplier")).Text = lcApp.Vendor.Name;
                ((Label)e.Row.FindControl("lbl_SupplierId")).Text = lcApp.Vendor.VendorId.ToString();
                sSplitSuffix = lcApp.SplitSuffix;
                if (sSplitSuffix == null) sSplitSuffix = "";
                ((Label)e.Row.FindControl("lbl_ContractNo")).Text = lcApp.ContractNo + sSplitSuffix;
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = lcApp.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_PurchaseTerm")).Text = lcApp.TermOfPurchase.TermOfPurchaseDescription;
                ((Label)e.Row.FindControl("lbl_PurchaseLocation")).Text = lcApp.PurchaseLocation.PurchaseLocationDescription;
                ((Label)e.Row.FindControl("lbl_Ccy")).Text = lcApp.Currency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_CurrencyId")).Text = lcApp.Currency.CurrencyId.ToString();
                ((Label)e.Row.FindControl("lbl_PODeliveryDate")).Text = DateTimeUtility.getDateString(lcApp.SupplierAtWarehouseDate);
                ((Label)e.Row.FindControl("lbl_POQty")).Text = lcApp.TotalPOQuantity.ToString("N00");
                ((Label)e.Row.FindControl("lbl_POAmt")).Text = lcApp.TotalPOAmt.ToString("N02");
                ((Label)e.Row.FindControl("lbl_WorkflowStatus")).Text = lcApp.WorkflowStatus.Name;
                if (lcApp.WorkflowStatus.Id == LCWFS.LC_CANCELLED.Id)
                {
                    Label lbl = ((Label)e.Row.FindControl("lbl_WorkflowStatus"));
                    lbl.Style.Add(HtmlTextWriterStyle.Color, "red");
                    lbl.Style.Add(HtmlTextWriterStyle.FontWeight, "bold");
                }
                ((Label)e.Row.FindControl("lbl_LCBatchNo")).Text = sFormattedLCBatchNo;
                ((TextBox)e.Row.FindControl("txt_LCBatchNo")).Text = sLCBatchNo;

                foreach (IssuingBank ib in IssuingBank.getCollectionValues())
                    ((DropDownList)e.Row.FindControl("ddl_IssuingBank")).Items.Add(new ListItem(ib.ShortName, ib.Id.ToString()));
                if (lcApp.LCBatch == null)
                    BankId = IssuingBank.DefaultBankId;
                else
                    if (lcApp.LCBatch.LCBatchId <= 0)
                        BankId = IssuingBank.DefaultBankId;
                    else
                        BankId = lcApp.LCBatch.IssuingBankId;
                ((DropDownList)e.Row.FindControl("ddl_IssuingBank")).SelectedValue = BankId.ToString();

                nStatus = int.Parse(ddl_LcApplicationStatus.SelectedValue);
                if ((nStatus == LCWFS.NEW.Id || nStatus == LCWFS.REJECTED.Id))
                {
                    ((TextBox)e.Row.FindControl("txt_Group")).Enabled = true;
                    ((DropDownList)e.Row.FindControl("ddl_IssuingBank")).Enabled = true;
                }
                else
                {
                    ((TextBox)e.Row.FindControl("txt_Group")).Enabled = false;
                    ((DropDownList)e.Row.FindControl("ddl_IssuingBank")).Enabled = false;
                }
                if (sGroupId == "") sGroupId = "1";    // default group is 1
                ((TextBox)e.Row.FindControl("txt_Group")).Text = sGroupId;

                LinkButton lb = ((LinkButton)e.Row.FindControl("lnk_LCInfoOfOtherDly"));
                GridView gv = (GridView)e.Row.FindControl("gv_LCInfoOfOtherDly");
                if (lcApp.WorkflowStatus.Id == LCWFS.NEW.Id && string.IsNullOrEmpty(lcApp.LCNo) && lcApp.ContractLCIssued > 0)
                {
                    lb.Visible = gv.Visible = true;
                    bindLCInfoOfOterDly(gv, e.Row.RowIndex);
                }
                else
                    lb.Visible = gv.Visible = false;
            }
        }


        protected void ddl_Office_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList ProductTeamList;
            int nOfficeId;
            int nUserId;
            int nDepartmentId;
            int nProductTeamId;



            nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);
            nOfficeId = int.Parse(this.ddl_Office.SelectedValue);
            //if (nOfficeId ==-1) nOfficeId=GeneralCriteria.ALL;

            ArrayList DepartmentList = new ArrayList();
            DepartmentList = CommonUtil.getProductDepartmentListByCriteria(this.LogonUserId, nOfficeId, GeneralCriteria.ALL);
            nDepartmentId = GeneralCriteria.ALL;
            this.ddl_Department.bindList(DepartmentList, "Description", "ProductDepartmentId", "", "--All--", GeneralCriteria.ALL.ToString());
            if (nDepartmentId == GeneralCriteria.ALL)
                this.ddl_Department.SelectedIndex = 0;
            else
                this.ddl_Department.SelectedValue = nDepartmentId.ToString();
            if (this.ddl_Office.SelectedIndex == 0)
                this.ddl_Department.Enabled = false;

            nProductTeamId = GeneralCriteria.ALL;
            if (this.ddl_Department.Items.Count > 1)
                ProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, nOfficeId, nUserId, GeneralCriteria.ALLSTRING);
            else
                // User cannot access any product team
                ProductTeamList = null;
            //ArrayList ProductTeamList = new ProductTeamList();
            this.ddl_ProductTeam.bindList(ProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
            this.ddl_ProductTeam.SelectedIndex = 0;

            ddl_ProductTeam_Refresh();
            return;
        }

        protected void ddl_Department_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nUserId;
            int nOfficeId;
            int nDepartmentId;
            ArrayList ProductTeamList;

            nUserId = ((HasAccessRightsTo_SuperView) ? GeneralCriteria.ALL : this.LogonUserId);
            nOfficeId = int.Parse(this.ddl_Office.SelectedValue);
            //if (nOfficeId == -1) nOfficeId = GeneralCriteria.ALL;
            nDepartmentId = int.Parse(this.ddl_Department.SelectedValue);
            //if (nDepartmentId == -1) nDepartmentId = GeneralCriteria.ALL;

            ProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(nDepartmentId, GeneralCriteria.ALL, nOfficeId, nUserId, GeneralCriteria.ALLSTRING);
            this.ddl_ProductTeam.bindList(ProductTeamList, "Description", "OfficeStructureId", "", "--All--", GeneralCriteria.ALL.ToString());
            this.ddl_ProductTeam_Refresh();
        }

        protected void ddl_ProductTeam_Refresh()
        {
            if ((this.ddl_Office.SelectedIndex == 0 && this.ddl_Office.Items.Count > 2)
                || (this.ddl_Department.SelectedIndex == 0 && this.ddl_Department.Items.Count > 2))
            {
                this.ddl_ProductTeam.SelectedIndex = 0;
                this.ddl_ProductTeam.Enabled = false;
            }
            else
            {
                this.ddl_ProductTeam.Enabled = true;
            }
        }

        protected void ddl_LcApplicationStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshButtons();
        }
        /*
            protected void ddl_ProductTeam_Init(object sender, EventArgs e)
                {
                    ArrayList userProductTeamList;

                    if (ddl_Office.SelectedValue != "" && ddl_Department.SelectedValue != "")
                    {   // both Office and Department are not --ALL--
                        userProductTeamList = CommonUtil.getProductCodeListWithUserSeasonOfficeStructureByCriteria(GeneralCriteria.ALL, GeneralCriteria.ALL, int.Parse(ddl_Office.SelectedValue), GeneralCriteria.ALL, GeneralCriteria.ALLSTRING);
                        //userProductTeamIdList.Clear;

                        foreach (OfficeStructureRef os in userProductTeamList)
                        {
                            //if (sProductCode!=os.Code)
                            // {
                            userProductTeamIdList.Add(os.OfficeStructureId);
                            //     sProductCode = os.Code;
                            // }
                        }
                        return;
                    }
                    else
                    {// default is --ALL--
                        return;
                    }
                    return;

                }
         */

        protected void btn_ResendSubmitNotice_Click(object sender, EventArgs e)
        {
            int i;
            ArrayList OfficeCodeList = new ArrayList();
            ArrayList Applist = new ArrayList();

            //if (HasAccessRightsTo_Submit)
            {
                foreach (GridViewRow r in gv_LC.Rows)
                {
                    if (r.RowType == DataControlRowType.DataRow)
                    {
                        LCShipmentRef lcShipment = (LCShipmentRef)this.vwLCApplicationList[r.RowIndex];
                        if (((CheckBox)r.FindControl("ckb_LC")).Checked)
                        {
                            LCApplicationDef df = new LCApplicationDef();
                            LCWorker.Instance.LCShipmentCopyToLCApplication(lcShipment, df);
                            Applist.Add(df);

                            for (i = 0; i < OfficeCodeList.Count; i++)
                                if (OfficeCodeList[i].ToString() == lcShipment.Office.OfficeCode) break;

                            if (i == OfficeCodeList.Count) OfficeCodeList.Add(lcShipment.Office.OfficeCode);
                        }
                    }
                }

                if (Applist.Count > 0)
                    //LCManager.Instance.ResendSubmitNoticeForLCApplication(Applist, OfficeCodeList);
                    //this.txt_DebugMessage.Text= LCManager.Instance.ResendSubmitNoticeForLCApplication_TEST(Applist, OfficeCodeList);
                    this.txt_DebugMessage.Text = LCManager.Instance.resendSubmitNoticeForLCApplication_TEST2(Applist, OfficeCodeList);
                //.Attributes.Add("OnClick", "window.open('../Shipping/???.aspx")

            }
        }


        protected void btn_UpdateLCInfo_Click(object sender, EventArgs e)
        {
            ArrayList ShipmentList = new ArrayList();
            foreach (GridViewRow r in gv_LC.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    LCShipmentRef lcShipment = (LCShipmentRef)this.vwLCApplicationList[r.RowIndex];
                    if (((CheckBox)r.FindControl("ckb_LC")).Checked && lcShipment.WorkflowStatus.Id == LCWFS.NEW.Id)
                    {   // copy the LC info from the other delivery of the same contract
                        if (lcShipment.OtherShipmentIdWithLCNo > 0)
                        {
                            DomainShipmentDef shipmentWithLCNo = ShipmentManager.Instance.getDomainShipmentDef(lcShipment.OtherShipmentIdWithLCNo);
                            if (shipmentWithLCNo != null)
                                if (shipmentWithLCNo.Invoice != null)
                                {
                                    lcShipment.LCNo = shipmentWithLCNo.Invoice.LCNo;
                                    lcShipment.LCIssueDate = shipmentWithLCNo.Invoice.LCIssueDate;
                                    //lcShipment.LCExpiryDate = shipmentWithLCNo.Invoice.LCExpiryDate;
                                    //lcShipment.LCAmt = shipmentWithLCNo.Invoice.LCAmount;
                                    //lcShipment.LCBatch = shipmentWithLCNo.LCBatch;
                                    //lcShipment.LCApplication = null;
                                    ShipmentList.Add(lcShipment);
                                }
                        }
                    }
                }
            }

            if (ShipmentList.Count > 0)
            {
                LCManager.Instance.updateLCInfoOfNewApplication(ShipmentList, this.LogonUserId);
            }
            this.btn_Search_Click(sender, e);
        }


        protected void btn_Approve_Click(object sender, EventArgs e)
        {
            int nBankId, nGroupId, nVendorId, nCurrencyId;
            string sGroupId;
            LCBatchAssignRef rfBatchKey;
            //LCBatchRef rfLCBatch;
            //int DummyId;
            //bool bGetDummyBatch;

            ArrayList ShipmentList = new ArrayList();
            ArrayList LCBatchList = new ArrayList();

            LCApplicationDef lcApp = new LCApplicationDef();

            //bGetDummyBatch = false;
            rfBatchKey = null;
            foreach (GridViewRow r in gv_LC.Rows)
            {   // Group the application into different LC Batch into the array list LCBatchList
                // and put the selected application into array list ShipmentList
                if (r.RowType == DataControlRowType.DataRow)
                {
                    LCShipmentRef lcShipment = (LCShipmentRef)this.vwLCApplicationList[r.RowIndex];
                    sGroupId = ((TextBox)r.FindControl("txt_Group")).Text;
                    if (sGroupId == "0")
                        nGroupId = 0;
                    else
                    {
                        if (int.TryParse(sGroupId, out nGroupId) == true)
                            nGroupId = int.Parse(sGroupId);
                        if (nGroupId == 0)
                            nGroupId = -1;
                    }

                    //if (((CheckBox)r.FindControl("ckb_LC")).Checked && ((Label)r.FindControl("lbl_LCBatchNo")).Text == "" && nGroupId > 0)
                    if (((CheckBox)r.FindControl("ckb_LC")).Checked)
                    {   // Allocate a LC Batch to the current application
                        nBankId = int.Parse(((DropDownList)r.FindControl("ddl_IssuingBank")).SelectedValue);
                        nVendorId = int.Parse(((Label)r.FindControl("lbl_SupplierId")).Text);
                        nCurrencyId = int.Parse(((Label)r.FindControl("lbl_CurrencyId")).Text);
                        if ((rfBatchKey == null ? true : (nBankId != rfBatchKey.LCBatch.IssuingBankId || nGroupId != rfBatchKey.LCBatch.GroupId || nVendorId != rfBatchKey.VendorId || nCurrencyId != rfBatchKey.CurrencyId)))
                        {
                            rfBatchKey = null;
                            foreach (LCBatchAssignRef rf in LCBatchList)
                            {   // Locate the BankId, GroupId, VendorId & CurrencyId in the array 'LCBatchlist'
                                if (rf.LCBatch.IssuingBankId == nBankId && rf.LCBatch.GroupId == nGroupId && nVendorId == rf.VendorId && nCurrencyId == rf.CurrencyId)
                                {
                                    rfBatchKey = rf;
                                    break;
                                }
                            }
                            if (rfBatchKey == null)
                            {   // cannot locate any combinateion in the array 'LCBatchKeyList', create a new batch in the array 'LCBatchList' and 'LCBatchKeyList'
                                LCBatchAssignRef rfNewBatchKey = new LCBatchAssignRef();
                                rfNewBatchKey.LCBatch = new LCBatchRef();
                                rfNewBatchKey.LCBatch.IssuingBankId = nBankId;
                                rfNewBatchKey.LCBatch.GroupId = nGroupId;
                                rfNewBatchKey.LCBatch.Status = GeneralStatus.INACTIVE.Code;
                                rfNewBatchKey.VendorId = nVendorId;
                                rfNewBatchKey.CurrencyId = nCurrencyId;
                                LCBatchList.Add(rfNewBatchKey);

                                rfBatchKey = rfNewBatchKey;
                            }
                        }
                        lcShipment.LCBatch = rfBatchKey.LCBatch;
                        ShipmentList.Add(lcShipment);
                    }
                }
            }

            if (ShipmentList.Count > 0)
            {
                LCManager.Instance.approveLCApplication(ShipmentList, LCBatchList, this.LogonUserId);
            }
            this.btn_Search_Click(sender, e);
        }

        protected void btn_Reject_Click(object sender, EventArgs e)
        {
            int nGroupId;
            string sGroupId;

            ArrayList ShipmentList = new ArrayList();
            ArrayList LCBatchList = new ArrayList();

            LCApplicationDef lcApp = new LCApplicationDef();

            //rfBatchKey = null;
            foreach (GridViewRow r in gv_LC.Rows)
            {   // Group the application into different LC Batch into the array list LCBatchList
                // and put the selected application into array list ShipmentList
                if (r.RowType == DataControlRowType.DataRow)
                {
                    LCShipmentRef lcShipment = (LCShipmentRef)this.vwLCApplicationList[r.RowIndex];
                    sGroupId = ((TextBox)r.FindControl("txt_Group")).Text;
                    if (sGroupId == "0")
                        nGroupId = 0;
                    else
                    {
                        if (int.TryParse(sGroupId, out nGroupId) == true)
                            nGroupId = int.Parse(sGroupId);
                        if (nGroupId == 0)
                            nGroupId = -1;
                    }

                    //if (((CheckBox)r.FindControl("ckb_LC")).Checked && ((Label)r.FindControl("lbl_LCBatchNo")).Text == "" && nGroupId > 0)
                    if (((CheckBox)r.FindControl("ckb_LC")).Checked)
                    {
#if false
		                // Allocate a LC Batch to the current application
                        nBankId = int.Parse(((DropDownList)r.FindControl("ddl_IssuingBank")).SelectedValue);
                        nVendorId = int.Parse(((Label)r.FindControl("lbl_SupplierId")).Text);
                        nCurrencyId = int.Parse(((Label)r.FindControl("lbl_CurrencyId")).Text);
                        if ((rfBatchKey == null ? true : (nBankId != rfBatchKey.LCBatch.IssuingBankId || nGroupId != rfBatchKey.LCBatch.GroupId || nVendorId != rfBatchKey.VendorId || nCurrencyId != rfBatchKey.CurrencyId)))
                        {
                            rfBatchKey = null;
                            foreach (LCBatchAssignRef rf in LCBatchList)
                            {   // Locate the BankId, GroupId, VendorId & CurrencyId in the array 'LCBatchlist'
                                if (rf.LCBatch.IssuingBankId == nBankId && rf.LCBatch.GroupId == nGroupId && nVendorId == rf.VendorId && nCurrencyId == rf.CurrencyId)
                                {
                                    rfBatchKey = rf;
                                    break;
                                }
                            }
                            if (rfBatchKey == null)
                            {   // cannot locate any combinateion in the array 'LCBatchKeyList', create a new batch in the array 'LCBatchList' and 'LCBatchKeyList'
                                LCBatchAssignRef rfNewBatchKey = new LCBatchAssignRef();
                                rfNewBatchKey.LCBatch = new LCBatchRef();
                                rfNewBatchKey.LCBatch.IssuingBankId = nBankId;
                                rfNewBatchKey.LCBatch.GroupId = nGroupId;
                                rfNewBatchKey.LCBatch.Status = 0;
                                rfNewBatchKey.VendorId = nVendorId;
                                rfNewBatchKey.CurrencyId = nCurrencyId;
                                LCBatchList.Add(rfNewBatchKey);

                                rfBatchKey = rfNewBatchKey;
                            }
                        }
                        lcShipment.LCBatch = rfBatchKey.LCBatch;
#endif
                        ShipmentList.Add(lcShipment);
                    }
                }
            }

            if (ShipmentList.Count > 0)
            {
                LCManager.Instance.rejectLCApplication(ShipmentList, this.LogonUserId);//LCBatchList, this.LogonUserId);
            }
            this.btn_Search_Click(sender, e);
        }

        protected void btn_Apply_Click(object sender, EventArgs e)
        {
            ArrayList ShipmentList = new ArrayList();
            LCApplicationDef lcApp = new LCApplicationDef();

            foreach (GridViewRow r in gv_LC.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    LCShipmentRef lcShipment = (LCShipmentRef)this.vwLCApplicationList[r.RowIndex];
                    if (((CheckBox)r.FindControl("ckb_LC")).Checked)
                    {
                        ShipmentList.Add(lcShipment);
                    }
                }
            }

            if (ShipmentList.Count > 0)
            {
                LCManager.Instance.applyLCApplication(ShipmentList, this.LogonUserId);
            }
            this.btn_Search_Click(sender, e);
        }

        protected void btn_LCNewApplicationSummary_Click(object sender, EventArgs e)
        {
            ReportHelper.export(LCReportManager.Instance.GenerateLCNewApplicationAllocationReport(), HttpContext.Current.Response, CrystalDecisions.Shared.ExportFormatType.ExcelRecord, "LC New Application Summary");
        }

        protected void gv_LC_OnSort(object sender, GridViewSortEventArgs e)
        {
            if (sortExpression == e.SortExpression)
            {
                sortDirection = (sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending);
            }
            else
            {
                sortExpression = e.SortExpression;
                sortDirection = SortDirection.Ascending;
            }
            LCShipmentRef.ShipmentComparer.CompareType compareType;

            if (sortExpression == "ContractNo")
                compareType = LCShipmentRef.ShipmentComparer.CompareType.ContractNo;
            else if (sortExpression == "LcApplicationNo")
                compareType = LCShipmentRef.ShipmentComparer.CompareType.LcApplicationNo;
            else if (sortExpression == "PoDeliveryDate")
                compareType = LCShipmentRef.ShipmentComparer.CompareType.PoDeliveryDate;
            else
                compareType = LCShipmentRef.ShipmentComparer.CompareType.LcApplicationNo;

            vwLCApplicationList.Sort(new LCShipmentRef.ShipmentComparer(compareType, sortDirection));
            gv_LC.DataSource = vwLCApplicationList;
            gv_LC.DataBind();
        }

        protected void LCInfoOfOtherDlyRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContractShipmentListJDef otherDly = (ContractShipmentListJDef)e.Row.DataItem;
                Label lbl;
                LCBatchRef lcb;
                InvoiceDef inv = ShipmentManager.Instance.getInvoiceByShipmentId(otherDly.ShipmentId);
                LCApplicationDef app = LCWorker.Instance.getLCApplicationShipmentByShipmentId(otherDly.ShipmentId, 0);
                if (app != null)
                {
                    lcb = LCWorker.Instance.getLCBatchByKey(app.LCBatchId);
                    if (lcb != null)
                    {
                        lbl = (Label)e.Row.FindControl("lbl_LcBatchNo");
                        lbl.Text = lcb.LCBatchNo.ToString("LCB00000#");
                    }
                    lbl = (Label)e.Row.FindControl("lbl_LcPoQty");
                    lbl.Text = app.TotalPOQuantity.ToString("#,##0");
                    lbl = (Label)e.Row.FindControl("lbl_LcStatus");
                    lbl.Text = app.WorkflowStatus.Name;
                    if (app.WorkflowStatus == LCWFS.LC_CANCELLED)
                    {
                        DataControlFieldCell cell = ((DataControlFieldCell)lbl.Parent);
                        lbl.Style.Add(HtmlTextWriterStyle.Color, "red");
                        lbl.Style.Add(HtmlTextWriterStyle.FontWeight, "bold");
                    }
                }

                lbl = (Label)e.Row.FindControl("lbl_ContractNo");
                lbl.Text = otherDly.ContractNo;
                lbl = (Label)e.Row.FindControl("lbl_DlyNo");
                lbl.Text = otherDly.DeliveryNo.ToString();
                lbl = (Label)e.Row.FindControl("lbl_PoQty");
                lbl.Text = otherDly.TotalPOQuantity.ToString("#,##0");

                lbl = (Label)e.Row.FindControl("lbl_LcNo");
                lbl.Text = (!string.IsNullOrEmpty(inv.LCNo) ? inv.LCNo : string.Empty);
                lbl = (Label)e.Row.FindControl("lbl_LcIssuedDate");
                lbl.Text = (inv.LCIssueDate != DateTime.MinValue ? DateTimeUtility.getDateString(inv.LCIssueDate) : string.Empty);
                lbl = (Label)e.Row.FindControl("lbl_LcExpiryDate");
                lbl.Text = (inv.LCExpiryDate != DateTime.MinValue ? DateTimeUtility.getDateString(inv.LCExpiryDate) : string.Empty);
                lbl = (Label)e.Row.FindControl("lbl_LcAmount");
                lbl.Text = (!(string.IsNullOrEmpty(inv.LCNo) && inv.LCAmount == 0) ? inv.LCAmount.ToString("#,##0.00") : string.Empty);
            }
        }


        protected void bindLCInfoOfOterDly(object gridView, int itemIndex)
        {
            LCShipmentRef lcApp = (LCShipmentRef)this.vwLCApplicationList[itemIndex];
            DomainShipmentDef lcAppShipment = ShipmentManager.Instance.getDomainShipmentDef(lcApp.ShipmentId);

            ArrayList otherDelivery = new ArrayList();
            int firstDlyWithLCNo = 0;
            foreach (ContractShipmentListJDef delivery in lcAppShipment.OtherDelivery)
                if (delivery.ShipmentId != lcApp.ShipmentId)
                {
                    InvoiceDef inv = ShipmentManager.Instance.getInvoiceByShipmentId(delivery.ShipmentId);
                    if (firstDlyWithLCNo == 0 && !string.IsNullOrEmpty(inv.LCNo))
                        firstDlyWithLCNo = delivery.ShipmentId;
                    otherDelivery.Add(delivery);
                }
            lcApp.OtherShipmentIdWithLCNo = firstDlyWithLCNo;
            vw_otherDelivery = otherDelivery;
            GridView gv = null;
            if (gridView != null) 
                gv = (GridView)gridView;
            if (gv != null)
            {
                gv.DataSource = vw_otherDelivery;
                gv.DataBind();
            }
        }
    }

}