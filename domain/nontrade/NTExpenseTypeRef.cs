using System;
using System.Collections ;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public partial class NTExpenseTypeRef:DomainData 
    {
        public NTExpenseTypeRef()
        {
            NatureId = -1;
        }

        public int ExpenseTypeId { get; set; }
        public string ExpenseType { get; set; }
        public string SUNAccountCode { get; set; }
        public string EpicorCode { get; set; }
        public int IsOfficeCode { get; set; }
        public int IsDepartmentCode { get; set; }
        public int IsProductCode { get; set; }
        public int IsSeasonCode { get; set; }
        public int IsStaffCode { get; set; }
        public int IsItemNo { get; set; }
        public int IsDevSampleCostType { get; set; }
        public int IsQtyRequired { get; set; }
        public int OfficeId { get; set; }
        public int IsAllowAccrual { get; set; }
        public string ItemDescriptionHints { get; set; }
        public string SUNDescription { get; set; }
        public string TallyCode { get; set; }
        public int NatureId { get; set; }
        public int Status { get; set; }
        public int isOtherCost { get; set; }
        public int IsSegmentValue { get; set; }

        public string Description
        {
            get
            {
                //return ExpenseType + " (" + SUNAccountCode + ") - [" + EpicorCode + "]" ;
                return ExpenseType + " [" + EpicorCode + "]";
            }
        }

        public string AccountCodeDescription
        {
            get
            {
                return "[" + EpicorCode  + "] - " + ExpenseType;
            }
        }
        public static bool isOtherReceivableRecharge(NTExpenseTypeRef def)
        {
            if (def == null || def.SUNAccountCode != "1311308")
                return false;
            return true;
        }

        public string SimpleAccountCodeDescription
        {
            get
            {
                return EpicorCode + " - " + ExpenseType;
            }
        }

        public class ExpenseTypeComparer : IComparer
        {
            public enum CompareType
            {
                ExpenseType = 1,
                SUNAccountCode = 2,                
            }

            private CompareType compareType;

            public ExpenseTypeComparer(CompareType type)
            {
                compareType = type;
            }

            public int Compare(object x, object y)
            {
                NTExpenseTypeRef xDef = (NTExpenseTypeRef)x;
                NTExpenseTypeRef yDef = (NTExpenseTypeRef)y;

                if (compareType.GetHashCode() == CompareType.ExpenseType.GetHashCode())
                {
                    return xDef.ExpenseType.CompareTo(yDef.ExpenseType);
                }
                else if (compareType.GetHashCode() == CompareType.SUNAccountCode.GetHashCode())
                {
                    return xDef.SUNAccountCode.CompareTo(yDef.SUNAccountCode);
                }
                else
                    return 0;
            }
        }

    }
}
