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
    public class NSLedImportFileDef : DomainData
    {
        public int FileId { get; set; }
        public string Filename { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int OfficeId { get; set; }
        public int CurrencyId { get; set; }
        public string SupplierCode { get; set; }
        public bool IsDutiable { get; set; }
        public decimal TotalNSCommissionAmt { get; set; }
        public int FiscalWeek { get; set; }
        public int FiscalYear { get; set; }
        public int Period { get; set; }
        public int IsInterfaced { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
