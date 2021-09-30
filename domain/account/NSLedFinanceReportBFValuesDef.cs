using System;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class NSLedFinanceReportBFValuesDef : DomainData
    {
        private int currentYear;
        private int officeId;
        private int seasonId;
        private string itemNo;
        private int soldQty;
        private decimal nsSales;
        private decimal cost;
        private decimal margin;
        private decimal settlementDiscountAmt;
        private decimal provision;
        private decimal totalPL;
        private decimal cumPL;
        private decimal stock;
        private decimal settlementDiscountAmtForStock;
        private decimal provisionForStock;
        private decimal totalBS;
        private decimal cumBS;

        public int CurrentYear
        {
            get { return currentYear; }
            set { currentYear = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int SeasonId
        {
            get { return seasonId; }
            set { seasonId = value; }
        }

        public string ItemNo
        {
            get { return itemNo; }
            set { itemNo = value; }
        }

        public int SoldQty
        {
            get { return soldQty; }
            set { soldQty = value; }
        }

        public decimal NSSales
        {
            get { return nsSales; }
            set { nsSales = value; }
        }

        public decimal Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        public decimal Margin
        {
            get { return margin; }
            set { margin = value; }
        }

        public decimal SettlementDiscountAmount
        {
            get { return settlementDiscountAmt; }
            set { settlementDiscountAmt = value; }
        }

        public decimal Provision
        {
            get { return provision; }
            set { provision = value; }
        }

        public decimal TotalPL
        {
            get { return totalPL; }
            set { totalPL = value; }
        }

        public decimal CumulativePL
        {
            get { return cumPL; }
            set { cumPL = value; }
        }

        public decimal Stock
        {
            get { return stock; }
            set { stock = value; }
        }

        public decimal SettlementDiscountAmountForStock
        {
            get { return settlementDiscountAmtForStock; }
            set { settlementDiscountAmtForStock = value; }
        }

        public decimal ProvisionForStock
        {
            get { return provisionForStock; }
            set { provisionForStock = value; }
        }

        public decimal TotalBalanceSheet
        {
            get { return totalBS; }
            set { totalBS = value; }
        }

        public decimal CumulativeBalanceSheet
        {
            get { return cumBS; }
            set { cumBS = value; }
        }
    }
}
