using System;
using com.next.common.domain;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class UKClaimReasonDef : DomainData
    {
        public UKClaimReasonDef()
        {

        }

        public int ReasonId { get; set; }
        public int ClaimTypeId { get; set; }
        public string ReasonDesc { get; set; }

    }
}
