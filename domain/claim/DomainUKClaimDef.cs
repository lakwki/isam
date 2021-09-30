using System;
using System.Collections.Generic;

namespace com.next.isam.domain.claim
{
    [Serializable()]
    public class DomainUKClaimDef
    {
        private UKClaimDef ukClaim;
        private List<UKClaimRefundDef> claimRefunds;

        public UKClaimDef Claim
        {
            get { return ukClaim; }
            set { ukClaim = value; }
        }

        public List<UKClaimRefundDef> ClaimRefunds
        {
            get { return claimRefunds; }
            set { claimRefunds = value; }
        }

    }
}
