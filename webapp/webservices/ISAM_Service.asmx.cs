using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.order;
using com.next.isam.domain.order;
using com.next.isam.dataserver.worker;

namespace com.next.isam.webapp.webservices
{
    /// <summary>
    /// Summary description for CheckPayYet
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ISAM_Service : System.Web.Services.WebService
    {

        [WebMethod]
        public bool CheckPaidByShipmentId(int shipmentId)
        {
            bool paid = false;
            InvoiceDef invoice = ShipmentManager.Instance.getInvoiceByShipmentId(shipmentId);
            if (invoice.APDate != DateTime.MinValue)
                paid = true;
            return paid;
        }

        [WebMethod]
        public bool CheckLCIssued(int shipmentId)
        {
            bool issued = false;
            InvoiceDef invoice = ShipmentManager.Instance.getInvoiceByShipmentId(shipmentId);
            if (invoice.LCNo.Trim() != string.Empty)
                issued = true;
            return issued;
        }

        [WebMethod]
        public bool CheckShipmentExist(string contractNo, int deliveryNo)
        {
            bool exist = false;
            ShipmentDef def = OrderManager.Instance.getShipmentByContractNoAndDeliveryNo(contractNo, deliveryNo);
            if (def != null)
                exist = true;

            return exist;
        }

    }
}