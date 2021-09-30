using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using com.next.isam.domain.order;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.common;
using com.next.isam.appserver.order;
using com.next.isam.webapp.commander;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using com.next.common.domain;

namespace com.next.isam.webapp.api
{
    public partial class GetDummy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string json = "{cols: [{id: 'CountryId', label: 'Country Id', type: 'number'},{id: 'Name', label: 'Name', type: 'string'},{id: 'Code', label: 'Code', type: 'string'}],rows: [";
            bool isFirst = true;
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            try
            {
                /*
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
                    json = "[{\"status\": \"200\",\"custCode\":\"" + customerOPSKey + "\",\"vendorName\":\"" + vendorName + "\"}]";
                }
                */

                ArrayList list = (ArrayList)WebUtil.getCountryList();




                foreach(CountryRef def in list)
                {
                    json += ((!isFirst ? ",": "") + ("{c:[{v: " + def.CountryId.ToString() + "}, {v: '" + def.Name + "'}, {v : '" + def.Code + "'}]}"));
                    isFirst = false;
                }
                json += "]}";

            }

            catch (Exception ex)
            {
                json = "[{\"status\": 400,\"errorMessage\":\"" + ex.Message + "\"}]";
            }
            Response.Write(json);
            Response.End();
        }
    }
}