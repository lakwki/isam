using System;
using System.Collections;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class NTEpicorSegmentValueRef : DomainData
    {
        public NTEpicorSegmentValueRef()
        {
        }

        public int SegmentValueId { get; set; }
        public int SegmentField { get; set; }
        public string SegmentValue { get; set; }
        public string SegmentName { get; set; }
        public int Status { get; set; }

        public string Description
        {
            get
            {
                return SegmentValue + " - " + SegmentName;
            }
        }
    }
}
