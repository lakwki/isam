using System;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class CustomerOrderCopyDef : DomainData
    {
        public CustomerOrderCopyDef()
        {
        }

        public string OrderSource { get; set; }
        public int OrderRefId { get; set; }
        public string ContractNo { get; set; }
        public string DeliveryNo { get; set; }
        public string ItemNo { get; set; }
        public string ItemDesc { get; set; }
        public string TransportMode { get; set; }
        public string CountryOfOrigin { get; set; }
        public string DepartCountry { get; set; }
        public DateTime ExFactoryDate { get; set; }
        public DateTime InWarehouseDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string HangBox { get; set; }
        public string BuyingTerms { get; set; }
        public string FinalDestination { get; set; }
        public string Currency { get; set; }
        public int NextFreightPercent { get; set; }
        public int SupplierFreightPercent { get; set; }
        public string ArrivalPort { get; set; }
        public string FranchisePartnerCode { get; set; }
        public string Refurb { get; set; }
        public string FileNo { get; set; }
        public DateTime ImportDate { get; set; }
        public string LastSentLoadingPort { get; set; }
        public string LastSentOfficeCode { get; set; }
        public string LastSentQuota { get; set; }
        public string LastSentDocType { get; set; }
        public bool IsValid { get; set; }

        public string Forwarder { get; set; }
        public DateTime ScheduledDeliveryDate { get; set; }
        public DateTime PromotionStartDate { get; set; }
        public string ShipFrom { get; set; }
        public int TotalOrderQuantity { get; set; }
        public decimal TotalOrderAmount { get; set; }

    }
}

