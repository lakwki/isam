using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.product;
using com.next.common.domain.industry.vendor;
//using com.next.isam.domain.shipping;
//using com.next.isam.domain.types;



namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class LCBatchDetailRef : DomainData
    {
        public LCBatchDetailRef()
        {
        }


        public LCBatchRef LCBatch { get; set; }
        public int LCApplicationId { get; set; }
        public int ShipmentId { get; set; }
        public CustomerDestinationDef CustomerDestination { get; set; }
        public ShipmentMethodRef ShipmentMethod { get; set; }
        public CustomerDef Customer { get; set; }
        public DateTime SupplierAtWarehouseDate { get; set; }
        public decimal QACommissionPercent { get; set; }
        public decimal VendorPaymentDiscountPercent { get; set; }
        public decimal LabTestIncome { get; set; }
        public ProductDef Product {get;set;}
        public PackingUnitRef PackingUnit { get; set; }
        public int POQty { get; set; }
        public decimal ReducedSupplierGmtPrice { get; set; }
        public VendorRef Vendor { get; set; }
        public CurrencyRef Currency { get; set; }
        public int WorkflowStatusId {get;set;}
        public BankRef AdvisingBank { get; set; }
        public BankBranchRef BankBranch { get; set; }
        public TermOfPurchaseRef TermOfPurchase { get; set; }
        public PurchaseLocationRef PurchaseLocation { get; set; }
        public PaymentTermRef PaymentTerm { get; set; }
        public ProductCodeDef ProductTeam { get; set; }
        public decimal ExpectedDeductAmt { get; set; }
        public decimal ActualDeductAmt { get; set; }

    }
}
