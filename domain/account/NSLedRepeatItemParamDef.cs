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
    public class NSLedRepeatItemParamDef : DomainData
    {
        public string ItemNo { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int SeasonId { get; set; }
        public int FiscalYear { get; set; }
        public int FiscalWeek { get; set; }
    }
}
