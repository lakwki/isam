using System;
using System.Collections;
using System.Collections.Generic;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.common;


namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public partial class NTRechargeDCNoteInvoiceRef :DomainData
    {
        public List<NTInvoiceDef> InvoiceList { get; set; }
        public NTRechargeDCNoteDef DCNote { get; set; }
    }
}
