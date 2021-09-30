using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class CutOffStatusRef : DomainData 
    {
        private int fiscalYear;
        private int period;
        private int cutOffStatusId;
        private string accruedSince;

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

        public int CutOffStatusId
        {
            get { return cutOffStatusId; }
            set { cutOffStatusId = value; }
        }

        public string AccruedSince
        {
            get { return accruedSince; }
            set { accruedSince = value; }
        }

        public string ToToolTipText
        {
            get
            {
                string s = "Recognized Sales in P" + this.period.ToString() + " " + this.fiscalYear.ToString();
                if (cutOffStatusId == CutOffStatus.ACCRUAL.GetHashCode())
                    s += " (Accrual)";
                else if (cutOffStatusId == CutOffStatus.UNDEFINED.GetHashCode())
                    s = "Not Yet Recognized As Sales";
                return s;
            }

            
        }
    }
}
