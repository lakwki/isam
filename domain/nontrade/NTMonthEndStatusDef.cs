using System;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI.WebControls;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class NTMonthEndStatusDef : DomainData
    {
        public  const int NOT_YET_OPEN = -1;
        public  const int CLOSED = 0;
        public  const int OPEN = 1;
        public  const int CLOSING = 2;

        public int RecordId { get; set; }
        public OfficeRef Office { get; set; }
        public int FiscalYear { get; set; }
        public int Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }

        public string StatusDescription
        {
            get
            {
                if (this.Status == NOT_YET_OPEN)
                    return "NOT YET OPEN";
                else if (this.Status == CLOSED)
                    return "CLOSED";
                else if (this.Status == OPEN)
                    return "OPEN";
                else if (this.Status == CLOSING)
                    return "CLOSING";
                else
                    return "N/A";
            }
        }

        public class NTMonthEndStatusComparer : IComparer
        {
            public enum CompareType
            {
                Office = 1,
            }

            private CompareType compareType;
            private SortDirection direction;

            public NTMonthEndStatusComparer(CompareType type, SortDirection order)
            {
                compareType = type;
                direction = order;
            }

            public int Compare(object x, object y)
            {
                NTMonthEndStatusDef defX = (NTMonthEndStatusDef)x;
                NTMonthEndStatusDef defY = (NTMonthEndStatusDef)y;

                if (compareType.GetHashCode() == CompareType.Office.GetHashCode())
                {
                    if (direction == SortDirection.Ascending)
                        return defX.Office.Description.CompareTo(defY.Office.Description);
                    else
                        return defY.Office.Description.CompareTo(defX.Office.Description);
                }

                return 0;
            }
        }

    }
}
