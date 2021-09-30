using System;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class ThirdPartyCustomerDNToUKDef : DomainData
    {
        public int ShipmentId { get; set; }
        public int OfficeId { get; set; }
        public string ContractNo { get; set; }
        public int DeliveryNo { get; set; }
        public string InvoiceNo { get; set; }
        public string Currency { get; set; }
        public decimal InvoiceAmt { get; set; }
        public decimal CommissionAmt { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
