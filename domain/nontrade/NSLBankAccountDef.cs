using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.domain.types;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public partial class NSLBankAccountDef : DomainData
    {
        public NSLBankAccountDef()
        {
        }

        public int NSLBankAccountId { get; set; }
        public int OfficeId { get; set; }
        public CurrencyRef Currency { get; set; }
        public string SUNAccountCode { get; set; }
        public string EpicorBankId { get; set; }
        public string AccountNo { get; set; }
        public string T0Code { get; set; }
        public string TallyCode { get; set; }
        public int Status { get; set; }
        public int BankOfficeId { get; set; }
        public string Description
        {
            get
            {
                if (T0Code == string.Empty)
                    return AccountNo;
                else
                    return "[" + T0Code + "] " + AccountNo;
            }
        }

        /*
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        */

    }
}


