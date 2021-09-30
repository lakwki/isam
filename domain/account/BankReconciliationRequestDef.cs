using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class BankReconciliationRequestDef : DomainData
    {
        private int requestId;
        private string fileName;
        private string sunDB;
        private string bank;
        private DateTime processDate;
        private int status;
        private UserRef submitUser;
        private DateTime submitDate;

        public BankReconciliationRequestDef() { }

        public BankReconciliationRequestDef(string fileName, string sunDB, string bank, UserRef submitUser) 
        {
            this.requestId = -1;
            this.fileName = fileName;
            this.sunDB = sunDB;
            this.bank = bank;
            this.processDate = DateTime.MinValue;
            this.status = RequestStatus.PENDING.StatusId;
            this.submitUser = submitUser;
            this.submitDate = DateTime.Now;
        }

        public int RequestId 
        { 
            get { return requestId; } 
            set { requestId = value; } 
        }

        public string FileName 
        { 
            get { return fileName; } 
            set { fileName = value; } 
        }

        public string Bank 
        { 
            get { return bank; } 
            set { bank = value; } 
        }

        public string SunDB 
        { 
            get { return sunDB; } 
            set { sunDB = value; } 
        }

        public DateTime ProcessDate 
        { 
            get { return processDate; } 
            set { processDate = value; } 
        }

        public UserRef SubmitUser 
        { 
            get { return submitUser; } 
            set { submitUser = value; } 
        }

        public DateTime SubmitDate 
        { 
            get { return submitDate; } 
            set { submitDate = value; } 
        }

        public int Status 
        { 
            get { return status; } 
            set { status = value; } 
        }
    }
}
