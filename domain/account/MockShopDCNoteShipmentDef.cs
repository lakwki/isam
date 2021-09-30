using System;
using System.Text;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class MockShopDCNoteShipmentDef
    {
        private int dcNoteId;
        private int dcNoteShipmentId;
        private int officeId;
        private int tradingAgencyId;
        private int deptId;
        private int productTeamId;
        private int contractId;
        private int productId;
        private int shipmentId;
        private int seasonId;
        private int vendorId;
        private int countryOfOriginId;
        private int packingUnitId;
        private int sellCurrencyId;
        private int termOfPurchaseId;
        private decimal totalCourierCharge;
        private decimal totalShippedAmount;
        private int totalShippedQty;
        private decimal nslCommissionPercent;
        private decimal nslCommissionAmt;
        private int status;

        public MockShopDCNoteShipmentDef()
        {

        }

        public int DCNoteId
        {
            get { return dcNoteId; }
            set { dcNoteId = value; }
        }

        public int DCNoteShipmentId
        {
            get { return dcNoteShipmentId; }
            set { dcNoteShipmentId = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int TradingAgencyId
        {
            get { return tradingAgencyId; }
            set { tradingAgencyId = value; }
        }

        public int DeptId
        {
            get { return deptId; }
            set { deptId = value; }
        }

        public int ProductTeamId
        {
            get { return productTeamId; }
            set { productTeamId = value; }
        }

        public int ContractId
        {
            get { return contractId; }
            set { contractId = value; }
        }

        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public int SeasonId
        {
            get { return seasonId; }
            set { seasonId = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
        }

        public int CountryOfOriginId
        {
            get { return countryOfOriginId; }
            set { countryOfOriginId = value; }
        }

        public int PackingUnitId
        {
            get { return packingUnitId; }
            set { packingUnitId = value; }
        }

        public int SellCurrencyId
        {
            get { return sellCurrencyId; }
            set { sellCurrencyId = value; }
        }

        public int TermOfPurchaseId
        {
            get { return termOfPurchaseId; }
            set { termOfPurchaseId = value; }
        }

        public decimal TotalCourierCharge
        {
            get { return totalCourierCharge; }
            set { totalCourierCharge = value; }
        }

        public decimal TotalShippedAmount
        {
            get { return totalShippedAmount; }
            set { totalShippedAmount = value; }
        }

        public int TotalShippedQty
        {
            get { return totalShippedQty; }
            set { totalShippedQty = value; }
        }

        public decimal NSLCommissionPercent
        {
            get { return nslCommissionPercent; }
            set { nslCommissionPercent = value; }
        }

        public decimal NSLCommissionAmt
        {
            get { return nslCommissionAmt; }
            set { nslCommissionAmt = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
