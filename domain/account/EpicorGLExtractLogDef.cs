using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class EpicorGLExtractLogDef : DomainData
    {
        public EpicorGLExtractLogDef() { }
        public string Company { get; set; }
        public int FiscalYear { get; set; }
        public int FiscalPeriod { get; set; }
        public long RecordId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
