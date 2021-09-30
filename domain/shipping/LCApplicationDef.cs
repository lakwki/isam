using System;
using com.next.common.domain;
using com.next.common.domain.industry.vendor;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.isam.domain.product;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class LCApplicationDef : DomainData  
    {
        public int LCApplicationId { get; set; }
        public int LCApplicationNo { get; set; }
        public int ShipmentId { get; set; }
        public int SplitShipmentId { get; set; }
        public int LCBatchId { get; set; }
        public CustomerDef Customer { get; set; }
        public CustomerDestinationDef CustomerDestination { get; set; }
        public ProductDef Product { get; set; }
        public int TotalPOQuantity { get; set; }
        public decimal TotalPOAmount { get; set; }
        public decimal QACommissionPercent { get; set; }
        public decimal VendorPaymentDiscountPercent { get; set; }
        public decimal LabTestIncome { get; set; }
        public ShipmentMethodRef ShipmentMethod { get; set; }
        public DateTime CustomerAtWarehouseDate { get; set; }
        public DateTime SupplierAtWarehouseDate { get; set; }

        public UserRef LCApprover { get; set; }
        public DateTime LCApprovalDate { get; set; }
        public BankBranchRef BankBranch { get; set; }
        public int Status { get; set; }
        public LCWFS WorkflowStatus { get; set; }


        public string ContractNo { get; set; }
        public int DeliveryNo { get; set; }
        public PackingMethodRef PackingMethod { get; set; }
        public PackingUnitRef PackingUnit { get; set; }
        public CountryOfOriginRef CountryOfOrigin { get; set; }
        public ShipmentPortRef ShipmentPort { get; set; }
        public CurrencyRef Currency { get; set; }
        public PaymentTermRef PaymentTerm { get; set; }
        public TermOfPurchaseRef TermOfPurchase { get; set; }
        public PurchaseLocationRef PurchaseLocation { get; set; }
        public VendorRef Vendor { get; set; }
        public BankRef AdvisingBank { get; set; }
        public OfficeRef Office { get; set; }
        public DateTime LCCancellationDate { get; set; }
        public decimal DeducedFabricCost { get; set; }


    }
}
