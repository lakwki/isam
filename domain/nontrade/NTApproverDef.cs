using System;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class NTApproverDef : DomainData
    {
        public NTApproverDef()
        {
            Status = GeneralStatus.ACTIVE.Code;
        }

        public OfficeRef Office { get; set; }
        public UserRef Approver { get; set; }
        public int Status { get; set; }

        public string ApproverName
        {
            get { return Approver.DisplayName; }
        }

        public int ApproverId
        {
            get { return Approver.UserId; }
        }
    }
}
