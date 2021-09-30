using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using com.next.isam.domain.order;
using com.next.isam.domain.shipping;
using com.next.infra.web;
using com.next.isam.webapp.commander.account;
using com.next.isam.appserver.order;
using com.next.isam.domain.order;
using com.next.infra.util;
using com.next.isam.domain.account;
using com.next.isam.appserver.account;
using com.next.isam.appserver.shipping;
using System.Collections;

namespace com.next.isam.webapp.account
{
    public partial class AddContract : com.next.isam.webapp.usercontrol.PageTemplate
    {
        private int paymentId;
        protected int vwPaymentId
        {
            get
            {
                return (ViewState["vwPaymentId"] != null) ? (int)ViewState["vwPaymentId"] : 0;
            }
            set
            {
                ViewState["vwPaymentId"] = value;
            }
        }
        /// <summary>
        /// For list object in Session
        /// </summary>
        protected string listIndex
        {
            get { return vwPaymentId + "_ContractList"; }
        }
        /// <summary>
        /// List of available Contracts for the particular Advance Payment
        /// </summary>
        List<ContractShipmentListJDef> vwAllContractList
        {
            get
            {
                return (ViewState["vwAllContractList"] != null) ? (List<ContractShipmentListJDef>)ViewState["vwAllContractList"] : new List<ContractShipmentListJDef>();
            }
            set
            {
                ViewState["vwAllContractList"] = value;
            }
        }
        
        /// <summary>
        /// List of Contracts selected of the particular Advance Payment
        /// </summary>
        List<AdvancePaymentOrderDetailDef> vwAdvancePaymentOrderDetailDefList1
        {
            get
            {
                return (ViewState["vwAdvancePaymentOrderDetailDefList1"] != null) ? (List<AdvancePaymentOrderDetailDef>)ViewState["vwAdvancePaymentOrderDetailDefList1"] : new List<AdvancePaymentOrderDetailDef>();
            }
            set
            {
                ViewState["vwAdvancePaymentOrderDetailDefList1"] = value;
            }
        }

        /// <summary>
        /// List of Contracts to be selected
        /// </summary>
        List<AdvancePaymentOrderDetailDef> vwAdvancePaymentOrderDetailDefList2
        {
            get
            {
                return (ViewState["vwAdvancePaymentOrderDetailDefList2"] != null) ? (List<AdvancePaymentOrderDetailDef>)ViewState["vwAdvancePaymentOrderDetailDefList2"] : new List<AdvancePaymentOrderDetailDef>();
            }
            set
            {
                ViewState["vwAdvancePaymentOrderDetailDefList2"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.Params["PaymentId"] == null)
                {
                    return;
                }
                if (!int.TryParse( HttpUtility.UrlDecode(EncryptionUtility.DecryptParam(Request.Params["PaymentId"]) ), out paymentId))
                {
                    return;
                }
                vwPaymentId = paymentId;
                if (Session[listIndex] == null)
                {
                    return;
                }

                // Initiate for checkbox selection
                List<AdvancePaymentOrderDetailDef> selectedList = (List<AdvancePaymentOrderDetailDef>)Session[listIndex];
                vwAdvancePaymentOrderDetailDefList1 = selectedList;

                ArrayList fullList = ShipmentManager.Instance.getShipmentListByAdvancePaymentId(vwPaymentId);
                vwAllContractList = fullList.Cast<ContractShipmentListJDef>().ToList(); // ListfullList.ToArray(typeof(ContractShipmentListJDef));

                gv_Contractlist.DataSource = vwAllContractList;
                gv_Contractlist.DataBind();

                btn_OK.Enabled = true;
            }
            

        }

        protected void btn_OK_Click(object sender, EventArgs e)
        {
            
            refreshAdvancePaymentOrderDetail();
        }

        protected void btn_Save_Click(object sender, EventArgs e)
        {
            //refreshAdvancePaymentOrderDetail();
        }
        private void refreshAdvancePaymentOrderDetail()
        {
            //vwAdvancePaymentOrderDetailDefList2.Clear();
            List<AdvancePaymentOrderDetailDef> newList = new List<AdvancePaymentOrderDetailDef>();
            List<string> exceptContractNo = new List<string>();
            foreach (GridViewRow row in gv_Contractlist.Rows)
            {
                int dataItemIndex = row.DataItemIndex;
                // Check Box
                CheckBox cb = (CheckBox)row.FindControl("cbSelect");
                if (cb.Checked)
                {
                    int rowIndex = row.DataItemIndex;
                    ContractShipmentListJDef csDef = vwAllContractList[rowIndex];
                    AdvancePaymentOrderDetailDef def = AccountManager.Instance.getAdvancePaymentOrderDetailByKey(vwPaymentId, csDef.ShipmentId);
                    AdvancePaymentOrderDetailDef newDef = new AdvancePaymentOrderDetailDef()
                    {
                        PaymentId = vwPaymentId,
                        ShipmentId = csDef.ShipmentId,
                        IsInitial = false,
                        Remark = "",
                        Status = 0
                    };

                    if (def == null) 
                    {
                        newList.Add(newDef);
                    }
                    else if (def != null && def.Status == 0)
                    {
                        if (def.Status == 0 && def.PaymentId == vwPaymentId)
                        {
                            // Reuse the existing data for same advance payment
                            newDef.Remark = def.Remark;
                        }
                        newList.Add(newDef);                                                
                    }
                    else //(bypass this selection because existing record is active)
                    {
                        //exceptContractNo.Add(((Label)row.FindControl("lbl_ContractNo")).Text);
                    }
                }
            }
            if (exceptContractNo.Count <= 0)
            {
                Session[listIndex] = newList;
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "saveContract", "saveContract();", true);
            }
            else
            {

            }
        }


        private void setContractSelected(List<AdvancePaymentOrderDetailDef> selectedList, int shipmentId, CheckBox cb) //gv_Contractlist
        {
            //List<AdvancePaymentOrderDetailDef> selected = vwAdvancePaymentOrderDetailDefList;
            if(selectedList == null || selectedList.Count <= 0){
                return;
            }
            foreach(AdvancePaymentOrderDetailDef selectedDef in selectedList){
                if (selectedDef.ShipmentId == shipmentId)
                {
                    cb.Checked = true;
                    return;
                }
            }
        }

        /// <summary>
        /// DataBinding with the contract list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_Contractlist_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContractShipmentListJDef def = (ContractShipmentListJDef)e.Row.DataItem;
                if(vwAdvancePaymentOrderDetailDefList1 != null && vwAdvancePaymentOrderDetailDefList1.Count > 0){
                    // Check Box
                    CheckBox cb = (CheckBox)e.Row.FindControl("cbSelect"); // getPaymentTypeName(apDef.PaymentTypeId);
                    setContractSelected(vwAdvancePaymentOrderDetailDefList1, def.ShipmentId, cb);
                }
                Label lbl_ContractNo = (Label)e.Row.FindControl("lbl_ContractNo");
                lbl_ContractNo.Text = def.ContractNo + "-" + def.DeliveryNo;

                Label lbl_ItemNo = (Label)e.Row.FindControl("lbl_ItemNo");
                lbl_ItemNo.Text = def.ItemNo;

                Label lbl_ProductTeam = (Label)e.Row.FindControl("lbl_ProductTeam");
                lbl_ProductTeam.Text = (def.ProductTeam!= null) ? def.ProductTeam.CodeDescription : "";

                Label lbl_CustomerAtWarehouseDate = (Label)e.Row.FindControl("lbl_CustomerAtWarehouseDate");
                lbl_CustomerAtWarehouseDate.Text = DateTimeUtility.getDateString(def.CustomerAgreedAtWarehouseDate);

                //def.InvoiceNo
                InvoiceDef inDef = com.next.isam.appserver.shipping.ShipmentManager.Instance.getInvoiceByShipmentId(def.ShipmentId);
                if (inDef != null)
                {
                    //inDef.LCBillRefNo
                    Label lbl_InvoiceNo = (Label)e.Row.FindControl("lbl_InvoiceNo");
                    lbl_InvoiceNo.Text = inDef.InvoiceNo;
                    Label lbl_InvoiceDate = (Label)e.Row.FindControl("lbl_InvoiceDate");
                    lbl_InvoiceDate.Text = DateTimeUtility.getDateString(inDef.InvoiceDate);
                    Label lbl_LCBillRefNo = (Label)e.Row.FindControl("lbl_LCBillRefNo");
                    lbl_LCBillRefNo.Text = inDef.LCBillRefNo;                    
                }

                Label lbl_PaymentTerm = (Label)e.Row.FindControl("lbl_PaymentTerm");
                lbl_PaymentTerm.Text = (def.PaymentTerm != null) ? def.PaymentTerm.PaymentTermDescription : "";

                Label lbl_WorkflowStatus = (Label)e.Row.FindControl("lbl_WorkflowStatus");
                lbl_WorkflowStatus.Text = def.WorkflowStatus.Name;
            }
        }

        



    }
}