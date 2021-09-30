using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using com.next.isam.appserver.claim;
using com.next.isam.domain.claim;

namespace com.next.isam.webapp.webservices
{
    /// <summary>
    /// Summary description for MapBIAClaim
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MapBIAClaim : System.Web.Services.WebService
    {

        [WebMethod]
        public void mapBIAClaim(int claimId, int claimRequestId)
        {
            UKClaimDef def = UKClaimManager.Instance.getUKClaimByKey(claimId);
            def.ClaimRequestId = claimRequestId;
            def.WorkflowStatus = ClaimWFS.SUBMITTED;
            UKClaimManager.Instance.updateUKClaimDef(def, 99999);
        }
    }
}
