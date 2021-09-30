using System;
using com.next.common.domain;
using com.next.isam.domain.types;
using com.next.common.domain.industry.vendor;
using com.next.isam.domain.product;
using com.next.isam.domain.ils;
using com.next.isam.domain.common;


namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class AlertNotificationRef : DomainData
    {

        public AlertNotificationRef()
        {
        }

        public int ShipmentId { get; set; }
        public int ContractId { get; set; }
        public string ContractNo { get; set; }
        public int DeliveryNo { get; set; }
        public OfficeRef Office { get; set; }
        public VendorRef Vendor { get; set; }
        public ProductDef Product { get; set; }
        public UserRef Merchandiser { get; set; }
        public ContractWFS WorkflowStatus { get; set; }
        public DateTime CustomerAtWarehouseDate { get; set; }
        public DateTime SupplierAtWarehouseDate { get; set; }
        public DateTime ShippingDocReceiptDate { get; set; }
        public string SupplierInvoiceNo { get; set; }
        public bool IsUploadDMSDocument { get; set; }
        public DateTime ShippingDocCheckedDate { get; set; }
        /*
        public string InvoiceStatus { get; set; }
        public string InvoiceStatusDescription { get; set; }
        (
        */

        public ILSInvoiceUploadStatus ILSInvoiceStatus { get; set; }

        public DateTime ActualAtWarehouseDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public UserRef InvoiceUploadUser { get; set; }
        public TermOfPurchaseRef TermOfPurchase { get; set; }

        public decimal TotalShippedAmt { get; set; }
        public decimal TotalShippedSupplierGmtAmt { get; set; }
        public DateTime ApprovalDate { get; set; }

        public string AlertType { get; set; }
        public int GroupId { get; set; }
        public int DocumentId { get; set; }
        public UserRef User { get; set; }
        public DateTime Date { get; set; }
        public CurrencyRef Currency { get; set; }
        public decimal Amount { get; set; }

        public string Remark { get; set; }
        public string Description { get; set; }
    }
}
