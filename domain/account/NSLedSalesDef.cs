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
    public class NSLedSalesDef : DomainData
    {
        public int FileId { get; set; }
        public int DetailId { get; set; }
        public string ItemNo { get; set; }
        public string SizeOptionNo { get; set; }
        public string SizeDesc { get; set; }
        public string ItemDesc { get; set; }
        public int DespatchQty { get; set; }
        public decimal DespatchAmt { get; set; }
        public int ReturnQty { get; set; }
        public decimal ReturnAmt { get; set; }
        public int NetQty { get; set; }
        public decimal NetAmt { get; set; }
        public string CountryOfSale { get; set; }
        public decimal VATPercent { get; set; }
        public decimal VAT { get; set; }
        public decimal NSVE { get; set; }
        public decimal CommPercent { get; set; }
        public decimal UKCommAmt { get; set; }
        public decimal NSCommAmt { get; set; }
        public decimal USDExRate { get; set; }
        public decimal UKCommAmtInUSD { get; set; }
        public decimal NSCommAmtInUSD { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CustomerType { get; set; }
        /*
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        */
    }
}