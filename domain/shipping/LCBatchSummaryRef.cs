using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.shipping;
using com.next.isam.domain.types;
using com.next.common.domain.industry.vendor;


namespace com.next.isam.domain.shipping
    {
    [Serializable()]
    public class LCBatchSummaryRef : DomainData
    {
        public LCBatchSummaryRef()
        {
        }

        public LCBatchRef LCBatch { get; set; }
        public VendorRef Vendor { get; set; }
        public CurrencyRef Currency { get; set; }
        public OfficeRef Office { get; set; }
        public LCWFS WorkflowStatus { get; set; }
        public int NoOfShipment { get; set; }
        public decimal TotalPOAmount { get; set; }

    }
}
