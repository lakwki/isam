using System;
using com.next.common.domain;
using System.Collections.Generic;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class EpicorGLExtractLogListDef : DomainData
    {
        public List<EpicorGLExtractLogDef> EpicorGLExtractLogList { get; set; }
    }
}
