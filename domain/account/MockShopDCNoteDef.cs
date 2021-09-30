using System;
using System.Text;
using com.next.common.domain.types;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class MockShopDCNoteDef
    {
        private int dcNoteId;
        private string dcNoteIndicator;
        private string dcNoteNo;
        private DateTime dcNoteDate;
        private int fiscalYear;
        private int period;
        private int officeId;
        private int sellCurrencyId;
        private decimal totalCourierCharge;
        private decimal totalShippedAmt;
        private decimal totalNSLCommissionAmt;
        private bool isInterfaced;
        private int status;

        public MockShopDCNoteDef()
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

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int SellCurrencyId
        {
            get { return sellCurrencyId; }
            set { sellCurrencyId = value; }
        }

        public decimal TotalCourierCharge
        {
            get { return totalCourierCharge; }
            set { totalCourierCharge = value; }
        }

        public decimal TotalShippedAmt
        {
            get { return totalShippedAmt; }
            set { totalShippedAmt = value; }
        }

        public decimal TotalNSLCommissionAmt
        {
            get { return totalNSLCommissionAmt; }
            set { totalNSLCommissionAmt = value; }
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
    }
}
