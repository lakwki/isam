using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using com.next.common.domain;

namespace com.next.isam.reporter.common
{
    [Serializable()]
    public class ReportParameterRef : DomainData 
    {
        string Text { get; set; }
        ArrayList IdList { get; set; }
    }
}


