using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using com.next.isam.appserver.claim;
using com.next.isam.domain.claim;
using com.next.common.domain.types;
using System.Xml.Serialization;
using com.next.isam.appserver.shipping;
using com.next.isam.domain.order;

namespace com.next.isam.webapp.webservices
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UKClaimService : System.Web.Services.WebService
    {

        [WebMethod]
        public void UpdateUKClaimWorkflowStatus(int requestId, int workflowStatusId, int userId)
        {
            UKClaimDef def = UKClaimManager.Instance.getUKClaimByClaimRequestId(requestId);
            if (def == null) return;

            int currentWorkflowStatusId = def.WorkflowStatus.Id;
            if (currentWorkflowStatusId != ClaimWFS.CANCELLED.Id &&
                currentWorkflowStatusId != ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id &&
                currentWorkflowStatusId != ClaimWFS.COO_SIGNED_OFF.Id)
            {
                UKClaimManager.Instance.setClaimWorkflowStatus(def.ClaimId, workflowStatusId, userId);
                UKClaimLogDef logDef = new UKClaimLogDef(def.ClaimId, "Update Next Claim DN", userId, currentWorkflowStatusId, workflowStatusId);
                UKClaimManager.Instance.updateUKClaimLogDef(logDef, userId);
            }

        }

        [WebMethod]
        public decimal getDNAmountByClaimRequestId(int requestId)
        {
            UKClaimDef def = UKClaimManager.Instance.getUKClaimByClaimRequestId(requestId);
            if (def == null) return -1;

            return def.Amount;
        }

        [WebMethod]
        public UKClaimDef getUKClaimByClaimRequestId(int requestId)
        {
            UKClaimDef def = UKClaimManager.Instance.getUKClaimByClaimRequestId(requestId);
            return def;
        }

        [WebMethod]
        public int getBIASettlementStatus(int requestId)
        {
            UKClaimDef def = UKClaimManager.Instance.getBIAUKClaimByClaimRequestId(requestId);
            if (def == null)
                return 0;
            else
            {
                if (def.WorkflowStatus.Id != ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id)
                    return 0;
                else
                {
                    UKClaimBIADiscrepancyDef discrepancyDef = UKClaimManager.Instance.getUKClaimBIADiscrepancyByKey(def.ClaimId);
                    if (discrepancyDef == null || (discrepancyDef != null && discrepancyDef.IsLocked == false))
                        return 1;
                    else
                        return 2;
                }
            }
        }

        [WebMethod]
        public DateTime getLastShipmentDate(string itemNo, string contractNo, int vendorId)
        {
            DateTime d = ShipmentManager.Instance.getLastShipmentDate(itemNo, contractNo.Trim(), vendorId);
            if (d == DateTime.MinValue)
                d = ShipmentManager.Instance.getLastShipmentDate(itemNo, string.Empty, vendorId);
            return d;
        }

        [WebMethod]
        public VendorOrderSummaryRef getVendorSummaryInfo(int vendorId)
        {
            return ShipmentManager.Instance.getVendorOrderSummary(vendorId);
        }

        [WebMethod]
        public GenericOrderSummaryRef getItemOrderSummary(string itemNo, DateTime fromDate, DateTime toDate)
        {
            return ShipmentManager.Instance.getItemOrderSummary(itemNo, fromDate, toDate);
        }

        [WebMethod]
        public object[] GetUKClaimEarlyArrivalList(string officeIds)
        {
            TypeCollector ids = TypeCollector.Inclusive;
            string[] s = officeIds.Split(',');
            for (int i = 0; i < s.Length - 1; i++)
                ids.append(int.Parse(s[i]));

            List<UKClaimDef> list = UKClaimManager.Instance.getUKClaimEarlyArrivalList(ids);
            object[] ukClaimList = new object[list.Count];
            int j = 0;
            foreach (object o in list)
            {
                ukClaimList[j] = o;
                j++;
            }
            return ukClaimList;


        }

    }
}
