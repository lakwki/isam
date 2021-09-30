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
using com.next.infra.web;
using com.next.isam.domain.ils;
using com.next.isam.domain.common;
using com.next.isam.domain.order;
using com.next.isam.appserver.ils;
using com.next.isam.appserver.shipping;
using com.next.common.domain.module;
using com.next.isam.webapp.commander.shipment;
using com.next.infra.util;

namespace com.next.isam.webapp.shipping
{
    public partial class AWBDetail : com.next.isam.webapp.usercontrol.PageTemplate
    {
        string containerNo = "";
        private ArrayList vwResult
        {
            get { return (ArrayList)ViewState["Manifest"]; }
            set { ViewState["Manifest"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["ContainerNo"] != null)
                containerNo = Request.Params["ContainerNo"].ToString();

            if (!Page.IsPostBack)
            {
                if (containerNo != string.Empty)
                {
                    ILSManifestDef ilsManifestDef = ILSUploadManager.Instance.getManifestByKey(containerNo);
                    vwResult = (ArrayList)ShipmentManager.Instance.getILSManifestDetailShipmentList(containerNo);

                    gv_AWB.DataSource = vwResult;
                    gv_AWB.DataBind();

                    lbl_VoyageNo.Text = ilsManifestDef.VoyageNo;
                    lbl_VesselName.Text = ilsManifestDef.VesselName;
                    lbl_ContainerNo.Text = ilsManifestDef.ContainerNo;

                    lbl_DepartInfo.Text = ilsManifestDef.DepartPort;
                    lbl_ArrivalInfo.Text = ilsManifestDef.ArrivalPort;
                    lbl_DepartureDate.Text = DateTimeUtility.getDateString(ilsManifestDef.DepartDate);
                    lbl_ArrivalDate.Text = DateTimeUtility.getDateString(ilsManifestDef.ArrivalDate);
                    lbl_PartnerContainerNo.Text = ilsManifestDef.PartnerContainerNo;
                }
            }
        }

        protected void ManifestDetailRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ILSManifestDetailShipmentDef manifestDetailShipment = (ILSManifestDetailShipmentDef)vwResult[e.Row.RowIndex];

                if (manifestDetailShipment.InWarehouseDate == DateTime.MinValue)
                {
                    Label lbl = (Label)e.Row.FindControl("lbl_WHDate");
                    lbl.Text = string.Empty;
                }
                ShipmentDef shipment = ShipmentManager.Instance.getShipmentById(manifestDetailShipment.ShipmentId);
                if ((manifestDetailShipment.CustomerId == 1 || manifestDetailShipment.CustomerId == 2) ||
                    //(CustomerDestinationDef.isDFOrder(manifestDetailShipment.CustomerDestinationId) || CustomerDestinationDef.isUTurnOrder(manifestDetailShipment.CustomerDestinationId)))
                    (CustomerDestinationDef.isDFOrder(manifestDetailShipment.CustomerDestinationId) || shipment.TermOfPurchase.TermOfPurchaseId==TermOfPurchaseRef.Id.FOB_UT.GetHashCode()))
                {
                    Label lbl = (Label)e.Row.FindControl("lbl_HB");
                    lbl.Text = PackingMethodRef.getDescription("B");
                }
            }
        }

        protected void btn_Print_Click(object sender, EventArgs e)
        {
            ArrayList shipmentIdList = new ArrayList();

            foreach (ILSManifestDetailShipmentDef def in vwResult)
                if (!string.IsNullOrEmpty(def.InvoicePrefix)) shipmentIdList.Add(def.ShipmentId);

            Context.Items.Clear();
            Context.Items.Add(WebParamNames.COMMAND_PARAM, "shipping.shipment");
            Context.Items.Add(WebParamNames.COMMAND_ACTION, ShipmentCommander.Action.PrintInvoice);
            Context.Items.Add(ShipmentCommander.Param.shipmentList, shipmentIdList);

            forwardToScreen(null);
        }

    }
}
