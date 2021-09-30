using System;
using com.next.common.domain;
using com.next.isam.domain.types;
using com.next.common.domain.industry.vendor;
using com.next.isam.domain.product;
using com.next.isam.domain.ils;
using com.next.isam.domain.common;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class NSLedSelfBilledSupplierCodeDef : DomainData
    {
        public int SeqId { get; set; }
        public string UKSupplierCode { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int OfficeId { get; set; }
        public string Remark { get; set; }
    }
}
