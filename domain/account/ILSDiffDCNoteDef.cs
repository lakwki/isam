using System;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class ILSDiffDCNoteDef : DomainData
    {
        private int dcNoteId;
        private string dcNoteIndicator;
        private string dcNoteNo;
        private DateTime dcNoteDate;
        private int officeId;
        private int fiscalYear;
        private int period;
        private int currencyId;
        private decimal totalDiffAmt;
        private decimal totalSalesDiffAmt;
        private decimal totalSalesCommissionDiffAmt;
        private bool isInterfaced;
        private int status;
        private int createdBy;
        private DateTime createdOn;

        public ILSDiffDCNoteDef()
        {

        }

        public int DCNoteId
        {
            get { return dcNoteId; }
            set { dcNoteId = value; }
        }

        public string DCNoteIndicator
        {
            get { return dcNoteIndicator; }
            set { dcNoteIndicator = value; }
        }

        public string DCNoteNo
        {
            get { return dcNoteNo; }
            set { dcNoteNo = value; }
        }

        public DateTime DCNoteDate
        {
            get { return dcNoteDate; }
            set { dcNoteDate = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
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

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public decimal TotalDiffAmt
        {
            get { return totalDiffAmt; }
            set { totalDiffAmt = value; }
        }

        public decimal TotalSalesDiffAmt
        {
            get { return totalSalesDiffAmt; }
            set { totalSalesDiffAmt = value; }
        }

        public decimal TotalSalesCommissionDiffAmt
        {
            get { return totalSalesCommissionDiffAmt; }
            set { totalSalesCommissionDiffAmt = value; }
        }

        public bool IsInterfaced
        {
            get { return isInterfaced; }
            set { isInterfaced = value; }
        }
        
        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public int CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }
    }
}
