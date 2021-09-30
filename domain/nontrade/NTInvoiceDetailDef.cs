using System;
using System.Collections.Generic;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.domain.industry.vendor;
using com.next.isam.domain.common;
using System.Text;

namespace com.next.isam.domain.nontrade
{
    [Serializable]
    public partial class NTInvoiceDetailDef : DomainData 
    {
        public NTInvoiceDetailDef()
        {
            ItemDescription1 = string.Empty;
            ItemDescription2 = string.Empty;
            ItemDescription3 = string.Empty;
            ItemDescription4 = string.Empty;
            ItemDescription5 = string.Empty;
            Description = string.Empty;
            ItemNo = string.Empty;
            ContactPerson = string.Empty;
            RechargeContactPerson = string.Empty;
            Status = GeneralCriteria.TRUE;
            IsPayByHK = 0;
            NatureIdForAccrual = 0;
            ContractNo = string.Empty;
            DeliveryNo = 0;
            IntercommOfficeId = -1;

        }

        public NTInvoiceDetailDef(NTInvoiceDetailType type)
        {
            InvoiceDetailType = type;
            ItemDescription1 = string.Empty;
            ItemDescription2 = string.Empty;
            ItemDescription3 = string.Empty;
            ItemDescription4 = string.Empty;
            ItemDescription5 = string.Empty;
            Description = string.Empty;
            ItemNo = string.Empty;
            ContactPerson = string.Empty;
            RechargeContactPerson = string.Empty;
            Status = GeneralCriteria.TRUE;
            IsPayByHK = 0;
            NatureIdForAccrual = 0;
            Quantity = 0;
            ContractNo = string.Empty;
            DeliveryNo = 0;
            IntercommOfficeId = -1;
        }

        public int InvoiceDetailId { get; set; }        
        public int InvoiceId { get; set; }
        public NTExpenseTypeRef ExpenseType { get; set; }
        public NTInvoiceDetailType InvoiceDetailType { get; set; }
        public CostCenterRef CostCenter { get; set; }
        public VendorRef Vendor { get; set; }
        public OfficeRef Office { get; set; }
        public CompanyType Company { get; set; }
        public CustomerDef Customer { get; set; }
        public DepartmentRef Department { get; set; }
        public ProductCodeRef ProductTeam { get; set; }
        public SeasonRef Season { get; set; }
        public UserRef User { get; set; }
        public NTEpicorSegmentValueRef SegmentValue7 { get; set; }
        public NTEpicorSegmentValueRef SegmentValue8 { get; set; }
        public string ItemNo { get; set; }
        public int DevSampleCostTypeId { get; set; }
        public int IsRecharge { get; set; }
        public CurrencyRef RechargeCurrency { get; set; }
        public int RechargeDCNoteId { get; set; }
        public NTRechargePartyDeptType RechargePartyDept { get; set; }
        public string ContactPerson { get; set; }
        public string RechargeContactPerson { get; set; }
        public decimal Amount { get; set; }
        public decimal VAT { get; set; }
        public string ItemDescription1 { get; set; }
        public string ItemDescription2 { get; set; }
        public string ItemDescription3 { get; set; }
        public string ItemDescription4 { get; set; }
        public string ItemDescription5 { get; set; }
        public string Description { get; set; }
        public int IsPayByHK { get; set; }
        public int Status { get; set; }
        public int NatureIdForAccrual { get; set; }
        public int Quantity { get; set; }
        public NTVendorDef NTVendor { get; set; }
        public string ContractNo { get; set; }
        public int DeliveryNo { get; set; }
        public int ConsumptionUnitId { get; set; }
        public decimal NoOfUnitConsumed { get; set; }
        public decimal ConsumptionUnitCost { get; set; }
        public int FuelTypeId { get; set; }
        public int IntercommOfficeId { get; set; }
    }

}
