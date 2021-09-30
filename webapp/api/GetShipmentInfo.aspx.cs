using System;
using System.Web.UI;
using com.next.isam.domain.order;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.common;
using com.next.isam.appserver.order;

namespace com.next.isam.webapp.api
{
    public partial class GetShipmentInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string json = "";
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            try
            {
                if (Request.Params["cno"] != null && Request.Params["dno"] != null)
                {
                    string contractNo = Request.Params["cno"].ToString();
                    string strDeliveryNo = Request.Params["dno"].ToString();
                    int deliveryNo = Convert.ToInt32(strDeliveryNo);

                    string customerOPSKey = "X";
                    ContractDef conDef = OrderSelectWorker.Instance.getContractByContractNo(contractNo);
                    if (conDef != null)
                    {
                        CustomerDef cusDef = CommonWorker.Instance.getCustomerByKey(conDef.Customer.CustomerId);
                        if (cusDef.OPSKey == "R" || cusDef.OPSKey == "D")
                            customerOPSKey = cusDef.OPSKey;
                    }
                    ShipmentDef def = OrderManager.Instance.getShipmentByContractNoAndDeliveryNo(contractNo, deliveryNo);
                    string vendorName = string.Empty;
                    if (def != null)
                        vendorName = def.Vendor.Name;
                    json = "{\"status\": 200,\"custCode\":\"" + customerOPSKey + "\",\"vendorName\":\"" + vendorName + "\"}";
                }
            }
            catch (Exception ex)
            {
                json = "{\"status\": 400,\"errorMessage\":\"" + ex.Message + "\"}";
            }
            Response.Write(json);
            Response.End();
        }
    }
}