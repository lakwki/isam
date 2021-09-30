using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class TallyUploadHistoryDef : DomainData
    {
        public int TallyUploadHistoryId { get; set; }
        public string UploadFileName { get; set; }
        public int SequenceNoStart { get; set; }
        public int SequenceNoEnd { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
