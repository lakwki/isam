using System;
using System.Text;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class UTContractDCNoteShipmentDef
    {
        private int dcNoteId;
        private int dcNoteShipmentId;
        private int officeId;
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
        private decimal commission;
        private decimal margin;
        private decimal serviceFee;
        private decimal supplierCommission;
        private decimal supplierGmtAmt;
        private decimal cnyExchangeRate;
        private decimal supplierGmtAmtInCNY;
        private int status;

        public UTContractDCNoteShipmentDef()
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

        public decimal Commission
        {
            get { return commission; }
            set { commission = value; }
        }

        public decimal Margin
        {
            get { return margin; }
            set { margin = value; }
        }

        public decimal ServiceFee
        {
            get { return serviceFee; }
            set { serviceFee = value; }
        }

        public decimal SupplierCommission
        {
            get { return supplierCommission; }
            set { supplierCommission = value; }
        }

        public decimal SupplierGmtAmt
        {
            get { return supplierGmtAmt; }
            set { supplierGmtAmt = value; }
        }

        public decimal CNYExchangeRate
        {
            get { return cnyExchangeRate; }
            set { cnyExchangeRate = value; }
        }

        public decimal SupplierGmtAmtInCNY
        {
            get { return supplierGmtAmtInCNY; }
            set { supplierGmtAmtInCNY = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
