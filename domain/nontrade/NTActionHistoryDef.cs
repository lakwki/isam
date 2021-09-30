using System;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public partial class NTActionHistoryDef : DomainData
    {
        public NTActionHistoryDef()
        {
        }

        public NTActionHistoryDef(int invoiceId, int ntVendorId, string description)
        {
            ActionHistoryId = -1;
            InvoiceId = invoiceId;
            NTVendorId = ntVendorId;
            Description = description;
            ActionOn = DateTime.Now;
            ActionBy = 99999;   // NSS Admin
            Status = GeneralCriteria.TRUE;
        }

        public NTActionHistoryDef(int invoiceId, int ntVendorId, string description, int userId)
        {
            ActionHistoryId = -1;
            InvoiceId = invoiceId;
            NTVendorId = ntVendorId;
            Description = description;
            ActionOn = DateTime.Now;
            ActionBy = (userId != 0 ? userId : 99999);
            Status = GeneralCriteria.TRUE;
        }

        public int ActionHistoryId { get; set; }
        public int InvoiceId { get; set; }
        public int NTVendorId { get; set; }
        public string Description { get; set; }
        public int ActionBy { get; set; }
        public DateTime ActionOn { get; set; }
        public int Status { get; set; }


    }
}
