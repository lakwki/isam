using System;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class NSLedProfitabilitiesDef : DomainData
    {
        public string ItemNo { get; set; }
        public int OfficeId { get; set; }
        public int CustomerId { get; set; }
        public int RangePlanId { get; set; }
        public int SeasonId { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public bool IsEndOfLife { get; set; }
        public string ProductTeamName { get; set; }
        public string ActualSaleSeason { get; set; }
        public string ExpectedSaleSeason { get; set; }
        public string ActualSaleSeasonWithPhase { get; set; }
        public int ActualSaleSeasonId { get; set; }
        public int ActualSaleSeasonSplitId { get; set; }
        public int ExpectedSaleSeasonId { get; set; }
        public int ExpectedSaleSeasonSplitId { get; set; }
        public int WeekCount { get; set; }
        public int TotalQty { get; set; }
        public int QtySold { get; set; }
        public int FPQty { get; set; }
        public decimal FPQtyPct { get; set; }
        public int MDQty { get; set; }
        public decimal MDQtyPct { get; set; }
        public string RetailSellingPrice { get; set; }
        public string MDPrice { get; set; }
        public int UKQty { get; set; }
        public decimal UKQtyPct { get; set; }
        public string UKDepartment { get; set; }
        public int IntQty { get; set; }
        public decimal IntQtyPct { get; set; }
        public decimal TotalNSCommissionAmt { get; set; }
        public decimal TotalFOBCost { get; set; }
        public decimal TotalProductionCost { get; set; }
        public decimal TotalEstimatedComm { get; set; }
        public decimal TotalFreightCost { get; set; }
        public decimal TotalSettlementDiscount { get; set; }
        public decimal TotalDutyCost { get; set; }
        public decimal EstimatedProfitPencentage { get; set; }
        public decimal AvgRetailSellingPrice { get; set; }
        public decimal MinRetailSellingPrice { get; set; }
        public decimal MaxRetailSellingPrice { get; set; }
        public bool HasDuty { get; set; }
        public int SeasonCount { get; set; }
        public int LaunchYear { get; set; }
        public int LaunchWeek { get; set; }
        public decimal VATFactor { get; set; }
        public int OfficeSortId { get; set; }
        public int FPWeekCount { get; set; }
        public DateTime FirstInvoiceDate { get; set; }
        public int IsNotYetLaunched { get; set; }
        public int NSLedPhaseId { get; set; }
        public string PricePoint { get; set; }
        public int PriceStartingPoint { get; set; }
        public string QtyRange { get; set; }
        public int QtyStartingPoint { get; set; }
        public int MDYear { get; set; }
        public int MDWeek { get; set; }
        public bool IsMD { get; set; }
        public int NoOfWeekUntilNextMD { get; set; }
        public string Comment { get; set; }
        public string SellThruRemark { get; set; }
        public int ShippedYear { get; set; } // for not yet launched items
        public int ShippedPeriod { get; set; } // for not yet launched items
        public decimal StockProvisionRate { get; set; }
    }
}
