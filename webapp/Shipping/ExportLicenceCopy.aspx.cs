using System;
using System.Collections;
using System.Web.UI;
using com.next.isam.webapp.commander;
using com.next.isam.webapp.commander.shipment;
using com.next.isam.domain.order;
using com.next.isam.domain.shipping;
using com.next.isam.domain.types;
using com.next.isam.appserver.shipping;

namespace com.next.isam.webapp.shipping
{
    public partial class ExportLicenceCopy : System.Web.UI.Page
    {
        string fromContractNo = string.Empty;
        int fromDeliveryNo = 0;
        int docId = 0;
        string action = "copy";

        ArrayList vwShipmentList
        {
            get { return (ArrayList)ViewState["shipmentList"]; }
            set { ViewState["shipmentList"] = value; }
        }

        DocumentDef vwDocDef
        {
            get { return (DocumentDef)ViewState["docDef"]; }
            set { ViewState["docDef"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["ContractNo"] != null)
                fromContractNo = Request.Params["ContractNo"].ToString();

            if (Request.Params["DlyNo"] != null)
                fromDeliveryNo = Convert.ToInt32(Request.Params["DlyNo"]);

            if (Request.Params["DocId"] != null)
                docId = Convert.ToInt32(Request.Params["DocId"]);

            if (Request.Params["Action"] != null)
                action = Request.Params["Action"].ToString();

            if (!Page.IsPostBack)
            {
                txt_FromContract.Text = fromContractNo;
                txt_FromDlyNo.Text = fromDeliveryNo.ToString();
                txt_ToContract.Text = fromContractNo;

                vwDocDef = ShipmentManager.Instance.getDocumentByKey(docId);
                ArrayList list = new ArrayList();
                list.Add(vwDocDef);
                gv_source.DataSource = list;
                gv_source.DataBind();

                getShipmentAndDoc(fromContractNo);

                if (action == "move")
                {
                    this.Title = "Custom Document Move";
                    lbl_Title.Text = "Custom Document Move";
                }
            }
        }

        protected void btn_ViewDetail_Click(object sender, EventArgs e)
        {
            valCustom.Enabled = false;
            getShipmentAndDoc(txt_ToContract.Text);
            gv_Licence.DataSource = null;
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            valCustom.Enabled = true;

            if (Page.IsValid)
            {
                if (action == "copy")
                {
                    vwDocDef.DocId = int.MinValue;
                }

                vwDocDef.ShipmentId = int.Parse(ddl_DlyNo.SelectedValue);
                Session[ShipmentCommander.Param.documents.ToString()] = vwDocDef;
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "close", "window.close();", true);
            }
        }

        protected void ddl_DlyNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int shipmentId = int.Parse(ddl_DlyNo.SelectedValue);
            ShipmentDef shipmentDef = (ShipmentDef)vwShipmentList[ddl_DlyNo.SelectedIndex];
            displayDocAndShipmentInfo(shipmentDef);
        }

        protected void displayDocAndShipmentInfo(ShipmentDef shipmentDef)
        {
            txt_POQty.Text = shipmentDef.TotalPOQuantity.ToString();
            txt_ShipMethod.Text = shipmentDef.ShipmentMethod.ShipmentMethodDescription;

            ArrayList docList = WebUtil.getDocumentListByShipmentId(shipmentDef.ShipmentId);
            gv_Licence.DataSource = docList;
            gv_Licence.DataBind();
        }

        protected void getShipmentAndDoc(string contractNo)
        {
            ArrayList shipmentList = WebUtil.getShipmentListByContractNo(contractNo);
            vwShipmentList = new ArrayList();

            foreach (ShipmentDef def in shipmentList)
            {
                if (!(contractNo == fromContractNo && def.DeliveryNo == fromDeliveryNo) &&
                    !(def.WorkflowStatus.Id == ContractWFS.CANCELLED.Id || def.WorkflowStatus.Id == ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id))
                {
                    vwShipmentList.Add(def);
                }
            }

            if (vwShipmentList.Count > 0)
            {
                ddl_DlyNo.bindList((ArrayList)vwShipmentList, "DeliveryNo", "ShipmentId");
                displayDocAndShipmentInfo((ShipmentDef)vwShipmentList[0]);
            }
            else
                txt_ToContract.Text = string.Empty;
        }

        protected void serverValidator(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = true;

            if (txt_ToContract.Text.Trim() == "" || ddl_DlyNo.SelectedIndex == -1)
            {
                args.IsValid = false;
                valCustom.ErrorMessage = "Please provide contract no. and delivery no.";
            }

            if (txt_ToContract.Text.Trim() == txt_FromContract.Text && ddl_DlyNo.selectedText == txt_FromDlyNo.Text)
            {
                valCustom.ErrorMessage = "Cannot copy to the same shipment.";
                args.IsValid = false;
            }
        }
    }
}
