using System;
using System.Collections.Generic;
using System.Text;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class VendorBankMappingRef : DomainData
    {
        private int vendorId;
        private int bankId;
        private int bankBranchId;
        private int status;

            public VendorBankMappingRef()
            {

            }

            public int VendorId
            {
                get { return vendorId; }
                set { vendorId = value; }
            }

            public int BankId
            {
                get { return bankId; }
                set { bankId = value; }
            }

            public int BankBranchId
            {
                get { return bankBranchId; }
                set { bankBranchId = value; }
            }

            public int Status
            {
                get { return status; }
                set { status = value; }
            }
    }

}
