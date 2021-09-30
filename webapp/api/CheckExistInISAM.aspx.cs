using com.next.infra.util;
using System;
using System.Web.UI;
using com.next.isam.appserver.order;
using com.next.isam.domain.order;

namespace com.next.isam.webapp.api
{
    public partial class CheckExistInISAM : System.Web.UI.Page
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

                    bool exist = false;
                    ShipmentDef def = OrderManager.Instance.getShipmentByContractNoAndDeliveryNo(contractNo, deliveryNo);
                    if (def != null)
                        exist = true;

                    json = "{\"status\": 200,\"exist\":\"" + exist.ToString().ToLower() + "\"}";
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