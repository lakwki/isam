using System;
using com.next.common.domain;
using com.next.common.domain.industry.vendor;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.isam.domain.product;
using System.Collections;
using System.Web.UI.WebControls;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class LCAdvancePaymentRef : DomainData
    {
        public LCAdvancePaymentRef()
        {
        }

        public string ContractNo { get; set; }
        public int DeliveryNo { get; set; }
        public VendorRef Vendor { get; set; }
        public ProductDef Product { get; set; }
        public DateTime SupplierAtWarehouseDate { get; set; }
        public int TotalPoQty { get; set; }
        public int TotalShippedQty { get; set; }
        public decimal TotalShippedAmt { get; set; }
        public DateTime InvoiceDate { get; set; }
        public ContractWFS ShipmentStatus { get; set; }
        public DateTime ApDate { get; set; }
        public int LCApplicationNo { get; set; }
        public int LCBatchNo { get; set; }
        public string LCNo { get; set; }
        public DateTime LCIssueDate { get; set; }
        public DateTime LCExpiryDate { get; set; }
        public string PaymentNo { get; set; }
        public decimal ExpectedDeductAmt { get; set; }
        public decimal ActualDeductAmt { get; set; }
        public string NSLRefNo { get; set; }
        public NSSAdvancePaymentWFS AdvancePaymentStatus { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime RejectDate { get; set; }
        public DateTime SubmittedDate { get; set; }
        public int PaymentTermId { get; set; }
        public decimal PaymentDeductionAmtInLC { get; set; }
    }
}
