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
    public class NSLedFPQtyDef : DomainData
    {
        public int LaunchYear { get; set; }
        public int LaunchWeek{ get; set; }
        public int FPQty{ get; set; }
        public decimal FPSellThrough{ get; set; }
        public decimal GrowthRate { get; set; }
    }
}