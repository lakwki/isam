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
    public partial class NTExpenseNatureRef : DomainData
    {
        public NTExpenseNatureRef()
        {
        }

        public int NatureId { get; set; }
        public string Description { get; set; }
    }
}


