using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using com.next.common.datafactory.worker;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.shipping;
using com.next.isam.appserver.shipping;

using com.next.infra.util;
using com.next.common.domain;
using com.next.common.domain.module;
using com.next.common.domain.types;

using com.next.common.datafactory;
using com.next.common.datafactory.worker.industry;
using com.next.common.web.commander;

using com.next.infra.web;

using com.next.isam.dataserver.model.shipping;
using com.next.isam.appserver.common;
using com.next.isam.appserver.order;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.common;
using com.next.isam.domain.product;
using com.next.isam.webapp.commander.shipment;
using com.next.isam.webapp.commander;
using com.next.common.web.commander;


namespace com.next.isam.webapp.shipping
{
    public partial class LCBatchUpdate : com.next.isam.webapp.usercontrol.PageTemplate  //System.Web.UI.Page
    {
        private bool HasAccessRightsTo_Edit;

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

        private string DecryptParameter(string parameter)
        {
            return parameter;   //WebUtil.DecryptParameter(parameter);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HasAccessRightsTo_Edit = (DecryptParameter(this.Request.QueryString["HasEditRights"]) == "YES");

            if (!IsPostBack)
            {
                int nLCBatchId;


                // Loading parameters

                this.txt_LCBatchId.Text = DecryptParameter(this.Request.QueryString["LCBatchId"]);
                if (this.txt_LCBatchId.Text == "")
                    nLCBatchId = -1;
                else
                    nLCBatchId = int.Parse(this.txt_LCBatchId.Text);
                this.txt_UserId.Text = DecryptParameter(this.Request.QueryString["UserId"]);


                HasAccessRightsTo_Edit = (DecryptParameter(this.Request.QueryString["HasEditRights"]) == "YES");

                // Load LC Batch Summary
                LoadLCBatch(nLCBatchId);

                btn_Update.Enabled = HasAccessRightsTo_Edit;
                btn_Cancel.Enabled = HasAccessRightsTo_Edit;
            }
        }

        protected void LoadLCBatch(int LCBatchId)
        {
           LoadLCBatchSummary(LCBatchId);
           gv_ShipmentLC.DataSource = getShipmentLCInfo(LCBatchId);
           gv_ShipmentLC.DataBind();
        }
        
        protected void LoadLCBatchSummary(int LCBatchId)
        {
            string sLCBatchNo;
            string sAppliedBy;

            LCBatchSummaryRef LCBatchSummary = LCWorker.Instance.getLCBatchSummaryByLCBatchId(LCBatchId);


            sLCBatchNo = LCBatchSummary.LCBatch.LCBatchNo.ToString();
            if (sLCBatchNo!="") sLCBatchNo = "LCB" + sLCBatchNo.PadLeft(6, char.Parse("0"));
            sAppliedBy = GeneralWorker.Instance.getUserByKey(LCBatchSummary.LCBatch.CreatedBy).DisplayName;
            
            this.txt_LCBatchNo.Text = sLCBatchNo;
            this.txt_Office.Text = LCBatchSummary.Office.OfficeCode;
            this.txt_Supplier.Text = LCBatchSummary.Vendor.Name;
            this.txt_Currency.Text = LCBatchSummary.Currency.CurrencyCode;
            this.txt_POAmt.Text = LCBatchSummary.TotalPOAmount.ToString("N02");
            this.txt_AppDate.Text = DateTimeUtility.getDateString(LCBatchSummary.LCBatch.CreatedOn);
            this.txt_AppliedBy.Text = sAppliedBy;
            this.txt_Status.Text = LCBatchSummary.WorkflowStatus.Name;
        }


        protected ArrayList vwShipmentLCInfoList
        {
            get { return (ArrayList)ViewState["ShipmentLCInfo"]; }
            set { ViewState["ShipmentLCInfo"] = value; }
        }

        protected ArrayList vwShipmentSelected
        {
            get { return (ArrayList)ViewState["ShipmentSelected"]; }
            set { ViewState["ShipmentSelected"] = value; }
        }

        protected ArrayList getShipmentLCInfo(int LCBatchId)
        {
            this.vwShipmentLCInfoList = LCWorker.Instance.getLCApplicationShipmentByLCBatchId(LCBatchId);
            if (vwShipmentSelected == null)
            {
                vwShipmentSelected = new ArrayList();
                foreach (LCShipmentRef shipment in vwShipmentLCInfoList)
                    vwShipmentSelected.Add(true);
            }
            return this.vwShipmentLCInfoList;
        }

        protected void gv_ShipmentLC_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LCShipmentRef LCShipment = (LCShipmentRef)vwShipmentLCInfoList[e.Row.RowIndex];

                ((Label)e.Row.FindControl("lbl_ContractNo")).Text = LCShipment.ContractNo + LCShipment.SplitSuffix;
                ((Label)e.Row.FindControl("lbl_DlyNo")).Text = LCShipment.DeliveryNo.ToString();
                ((Label)e.Row.FindControl("lbl_ItemNo")).Text = LCShipment.Product.ItemNo;
                ((Label)e.Row.FindControl("lbl_PurchaseTerm")).Text = LCShipment.TermOfPurchase.TermOfPurchaseDescription;
                ((Label)e.Row.FindControl("lbl_PurchaseLocation")).Text = LCShipment.PurchaseLocation.PurchaseLocationDescription;
                ((Label)e.Row.FindControl("lbl_Currency")).Text = LCShipment.Currency.CurrencyCode;
                ((Label)e.Row.FindControl("lbl_POQty")).Text = LCShipment.TotalPOQuantity.ToString("N0");
                ((Label)e.Row.FindControl("lbl_POAmt")).Text = LCShipment.TotalPOAmt.ToString("N02");
                
                ((TextBox)e.Row.FindControl("txt_LCNo")).Text = LCShipment.LCNo;
                ((TextBox)e.Row.FindControl("txt_IssuedDate")).Text = DateTimeUtility.getDateString(LCShipment.LCIssueDate);
                //"ce_IssueDate"  FirstDayOfWeek="Sunday" Format="dd/MM/yyyy" 
                ((TextBox)e.Row.FindControl("txt_ExpiryDate")).Text = DateTimeUtility.getDateString(LCShipment.LCExpiryDate);
                ((TextBox)e.Row.FindControl("txt_LCAmount")).Text = (LCShipment.LCAmt < 0 ? "" : LCShipment.LCAmt.ToString());

                ShipmentDef shipment = ShipmentManager.Instance.getShipmentById(LCShipment.ShipmentId);
                ((Label)e.Row.FindControl("lbl_ContractStatus")).Text = shipment.WorkflowStatus.Name;
                Label lbl = ((Label)e.Row.FindControl("lbl_LCStatus"));
                lbl.Text = LCShipment.WorkflowStatus.Name;
                ((TextBox)e.Row.FindControl("txt_LcCancelDate")).Text = DateTimeUtility.getDateString(LCShipment.LCCancellationDate);
                
                setGridRowStatus(e.Row, (bool)vwShipmentSelected[e.Row.RowIndex]);
            }
        }

        protected void btn_Update_Click(object sender, EventArgs e)
        {
            decimal nLCAmount;
            DateTime dDate;

            ArrayList ShipmentLCInfoList = new ArrayList();
            foreach (GridViewRow r in gv_ShipmentLC.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    LCShipmentRef LCShipment = (LCShipmentRef)this.vwShipmentLCInfoList[r.RowIndex];
                    CheckBox ckb =((CheckBox)r.FindControl("ckb_Selected"));
                    vwShipmentSelected[r.RowIndex] = ckb.Checked;
                    if (ckb.Checked)
                    {
                        if (LCShipment.WorkflowStatus.Id == LCWFS.APPLIED.Id || LCShipment.WorkflowStatus.Id == LCWFS.COMPLETED.Id || LCShipment.WorkflowStatus.Id == LCWFS.LC_CANCELLED.Id)
                        {
                            LCShipment.LCNo = ((TextBox)r.FindControl("txt_LCNo")).Text;

                            if (!decimal.TryParse(((TextBox)r.FindControl("txt_LCAmount")).Text, out nLCAmount)) nLCAmount = 0;
                            LCShipment.LCAmt = nLCAmount;

                            dDate = DateTime.MinValue;
                            if (!DateTime.TryParse(((TextBox)r.FindControl("txt_IssuedDate")).Text, out dDate)) dDate = DateTime.MinValue;
                            LCShipment.LCIssueDate = dDate;

                            dDate = DateTime.MinValue;
                            if (!DateTime.TryParse(((TextBox)r.FindControl("txt_ExpiryDate")).Text, out dDate)) dDate = DateTime.MinValue;
                            LCShipment.LCExpiryDate = dDate;

                            if (LCShipment.WorkflowStatus.Id == LCWFS.LC_CANCELLED.Id)
                            {
                                dDate = DateTime.MinValue;
                                if (!DateTime.TryParse(((TextBox)r.FindControl("txt_LcCancelDate")).Text, out dDate)) dDate = DateTime.MinValue;
                                LCShipment.LCCancellationDate = dDate;
                            }
                            ShipmentLCInfoList.Add(LCShipment);
                        }
                    }
                }
            }

            if (ShipmentLCInfoList.Count > 0)
            {
                LCManager.Instance.updateShipmentLCInfo(ShipmentLCInfoList, int.Parse(this.txt_UserId.Text));
                LoadLCBatch(int.Parse(this.txt_LCBatchId.Text));
            }
            else
                LoadLCBatch(int.Parse(this.txt_LCBatchId.Text));
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            // Reload the page
            vwShipmentSelected = null;
            LoadLCBatch(int.Parse(this.txt_LCBatchId.Text));
        }


        protected void btn_CancelLC_Click(object sender, EventArgs e)
        {
            ArrayList cancelList = new ArrayList();
            ArrayList invalidList = new ArrayList();
            LCBatchRef lcBatch = null;
            foreach (GridViewRow r in gv_ShipmentLC.Rows)
                if (r.RowType == DataControlRowType.DataRow)
                {
                    LCShipmentRef LCShipment = (LCShipmentRef)this.vwShipmentLCInfoList[r.RowIndex];
                    CheckBox ckb = ((CheckBox)r.FindControl("ckb_Selected"));
                    if (ckb.Checked)
                        if (LCShipment.WorkflowStatus.Id != LCWFS.LC_CANCELLED.Id)
                            if (!string.IsNullOrEmpty(LCShipment.LCNo))
                            {
                                vwShipmentSelected[r.RowIndex] = ckb.Checked;
                                cancelList.Add(LCShipment);
                                if (lcBatch == null)
                                    lcBatch = LCShipment.LCBatch;
                            }
                }
            if (cancelList.Count > 0)
            {
                int userId = int.Parse(this.txt_UserId.Text);
                LCManager.Instance.cancelLCApplicationInLCBatch(cancelList, userId);
            }
            else
                refreshSelection();
            LoadLCBatch(int.Parse(this.txt_LCBatchId.Text));
        }

        protected void refreshSelection()
        {
            foreach (GridViewRow r in gv_ShipmentLC.Rows)
                if (r.RowType == DataControlRowType.DataRow)
                    vwShipmentSelected[r.RowIndex] = ((CheckBox)r.FindControl("ckb_Selected")).Checked;
        }

        protected void setGridRowStatus(GridViewRow row, bool enable)
        {
            LCShipmentRef LCShipment = (LCShipmentRef)vwShipmentLCInfoList[row.RowIndex];
            if (LCShipment.WorkflowStatus == LCWFS.LC_CANCELLED)
                enable &= (((TextBox)row.FindControl("txt_LcCancelDate")).Text == string.Empty);
            vwShipmentSelected[row.RowIndex] = enable;
            CheckBox ckb = (CheckBox)row.FindControl("ckb_Selected");
            ckb.Checked = enable;
            ((TextBox)row.FindControl("txt_LCNo")).Enabled = enable;
            ((TextBox)row.FindControl("txt_IssuedDate")).Enabled = enable;
            ((TextBox)row.FindControl("txt_ExpiryDate")).Enabled = enable;
            ((TextBox)row.FindControl("txt_LCAmount")).Enabled = enable;
            ((ImageButton)row.FindControl("btn_Clear")).Enabled = enable;
            Label lbl = ((Label)row.FindControl("lbl_LCStatus"));
            if (LCShipment.WorkflowStatus == LCWFS.LC_CANCELLED)
            {
                DataControlFieldCell cell = ((DataControlFieldCell)lbl.Parent);
                lbl.Style.Add(HtmlTextWriterStyle.Color, "red");
                lbl.Style.Add(HtmlTextWriterStyle.FontWeight, "bold");
            }

            TextBox txt; 
            int[] allowStatusId = { LCWFS.APPLIED.Id, LCWFS.COMPLETED.Id, LCWFS.LC_CANCELLED.Id };
            if (!allowStatusId.Contains(LCShipment.WorkflowStatus.Id) || !HasAccessRightsTo_Edit)
            {
                string Reason;
                if (!HasAccessRightsTo_Edit)
                    Reason = "You do not possess access rights to update L/C detail.";
                else
                    Reason = "Disabled as the status is '" + LCShipment.WorkflowStatus.Name + "'";

                txt = ((TextBox)row.FindControl("txt_LCNo"));
                txt.Enabled = false;
                txt.ToolTip = Reason;
                txt = ((TextBox)row.FindControl("txt_IssuedDate"));
                txt.Enabled = false;
                txt.ToolTip = Reason;
                txt = ((TextBox)row.FindControl("txt_ExpiryDate"));
                txt.Enabled = false;
                txt.ToolTip = Reason;
                txt = ((TextBox)row.FindControl("txt_LCAmount"));
                txt.Enabled = false;
                txt.ToolTip = Reason;
                txt = ((TextBox)row.FindControl("txt_LcCancelDate"));
                txt.Enabled = false;
                txt.ToolTip = Reason;
            }
            else 
            {
                txt = ((TextBox)row.FindControl("txt_LcCancelDate"));
                if (LCShipment.WorkflowStatus.Id != LCWFS.LC_CANCELLED.Id)
                    txt.Style.Add(HtmlTextWriterStyle.Display, "none");
                else
                {
                    txt.Enabled = enable;
                    txt.Style.Add(HtmlTextWriterStyle.Display, "");
                }
            }
        }

        protected void gv_ShipmentLC_OnSort(object sender, GridViewSortEventArgs e)
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

            if (sortExpression == "ItemNo")
                compareType = LCShipmentRef.ShipmentComparer.CompareType.ItemNo;
            else //if (sortExpression == "ContractNo")
                compareType = LCShipmentRef.ShipmentComparer.CompareType.ContractNo;

            vwShipmentLCInfoList.Sort(new LCShipmentRef.ShipmentComparer(compareType, sortDirection));
            gv_ShipmentLC.DataSource = vwShipmentLCInfoList;
            gv_ShipmentLC.DataBind();
        }



    }
}
