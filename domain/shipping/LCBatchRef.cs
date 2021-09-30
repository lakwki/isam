using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;


namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class LCBatchRef : DomainData
    {
        public LCBatchRef()
        {
        }

        public int LCBatchId { get; set; }
        public int LCBatchNo { get; set; }
        public int GroupId { get; set; }
        public int IssuingBankId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Status { get; set; }
    }

    public class LCBatchAssignRef : DomainData
    {
        public LCBatchAssignRef()
        {
        }

        public LCBatchRef LCBatch { get; set; }
        public int VendorId { get; set; }
        public int CurrencyId { get; set; }
    }

}
