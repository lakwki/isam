using System;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class ILSDiffDCNoteShipmentDef : DomainData
    {
        private int dcNoteId;
        private int fiscalYear;
        private int period;
        private int dcNoteShipmentId;
        private int officeId;
        private int ilsType;
        private int deptId;
        private int productTeamId;
        private int contractId;
        private int productId;
        private int shipmentId;
        private int currencyId;
        private decimal nssAmt;
        private decimal receivedAmt;
        private decimal ilsDiffAmt;
        private int status;
        
        public ILSDiffDCNoteShipmentDef()
        {

        }

        public int DCNoteId
        {
            get { return dcNoteId; }
            set { dcNoteId = value; }
        }

        public int FiscalYear
        {
            get { return fiscalYear; }
            set { fiscalYear = value; }
        }

        public int Period
        {
            get { return period; }
            set { period = value; }
        }

        public int DCNoteShipmentId
        {
            get { return dcNoteShipmentId; }
            set { dcNoteShipmentId = value; }
        }

        public int ILSType
        {
            get { return ilsType; }
            set { ilsType = value; }
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

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public decimal NSSAmt
        {
            get { return nssAmt; }
            set { nssAmt = value; }
        }

        public decimal ReceivedAmt
        {
            get { return receivedAmt; }
            set { receivedAmt = value; }
        }

        public decimal ILSDiffAmt
        {
            get { return ilsDiffAmt; }
            set { ilsDiffAmt = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
