using System;
using System.Text;
using com.next.common.domain.types;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class QACommissionDNDef
    {
        private int dnId;
        private string dnNo;
        private DateTime dnDate;
        private int fiscalYear;
        private int period;
        private int officeId;
        private int currencyId;
        private int vendorId;
        private decimal totalQACommission;
        private bool isInterfaced;
        private int status;
        private int createdBy;
        private DateTime createdOn;

        public QACommissionDNDef()
        {

        }

        public int DNId
        {
            get { return dnId; }
            set { dnId = value; }
        }

        public string DNNo
        {
            get { return dnNo; }
            set { dnNo = value; }
        }

        public DateTime DNDate
        {
            get { return dnDate; }
            set { dnDate = value; }
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

        public int CurrencyId
        {
            get { return currencyId; }
            set { currencyId = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
        }

        public decimal TotalQACommission
        {
            get { return totalQACommission; }
            set { totalQACommission = value; }
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
