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
    public class UKClaimDCNoteRef : DomainData
    {
        public UKClaimDCNoteRef()
        {
        }


        public int ClaimId { get; set; }
        public int ClaimRefundId { get; set; }
        public int OfficeId { get; set; }
        public int VendorId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }


    }
}
