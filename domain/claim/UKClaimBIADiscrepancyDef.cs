using System;
using System.Collections;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using System.Web.UI.WebControls;
using com.next.common.domain.industry.vendor;
using com.next.common.domain.types;
using com.next.common.datafactory.worker;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimBIADiscrepancyDef : DomainData
    {
        public UKClaimBIADiscrepancyDef()
        {
        }

        public int ClaimId { get; set; }
        public BIAActionType ActionType { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public bool IsLocked { get; set; }
    }
}
