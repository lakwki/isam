using System;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class NTVendorExpenseTypeMappingDef : DomainData 
    {
        public NTVendorExpenseTypeMappingDef()
        {
        }

        public NTVendorExpenseTypeMappingDef(int ntVendorId, NTExpenseTypeRef expenseType)
        {
            NTVendorId = ntVendorId;
            ExpenseType = expenseType;
            Status = GeneralCriteria.TRUE;
        }

        public int NTVendorId { get; set; }
        public NTExpenseTypeRef ExpenseType { get ; set ;}
        public int Status { get; set; }

    }
}
