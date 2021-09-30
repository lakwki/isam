using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class LogoInterfaceRequestDef : DomainData
    {
        private int requestId;
        private OfficeRef office;
        private int fiscalYear;
        private int period;
        private UserRef submitUser;
        private DateTime submitTime;

        public LogoInterfaceRequestDef() { }

        public int RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }

        public OfficeRef Office
        {
            get { return office; }
            set { office = value; }
        }

        public int FiscalYear
        {
            get { return fiscalYear; }
            set { fiscalYear = value; }
        }

        public int Period
        {
            get { return period; }
            set { period = value; }
        }

        public UserRef SubmitUser
        {
            get { return submitUser; }
            set { submitUser = value; }
        }

        public DateTime SubmitTime
        {
            get { return submitTime; }
            set { submitTime = value; }
        }

    }
}
