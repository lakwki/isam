using System;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class OtherChargesImportFileDef : DomainData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string VPSName { get; set; }
        public string Description { get; set; }
        public string OriCurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string BilCurrencyCode { get; set; }
        public decimal BilAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string VendorOffice { get; set; }
        public string ErrorMessage { get; set; }
        public string DebitCreditIndicator { get; set; }
        public string Attn { get; set; }
    }
}
