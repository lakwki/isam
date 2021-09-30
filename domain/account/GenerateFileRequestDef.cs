using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{

    [Serializable()]
    public class GenerateFileRequestDef : DomainData
    {
        private int requestId;
        private string fileName;
        private int fileType;
        private UserRef submitUser;
        private DateTime submitDate;
        private int status;

        public enum Type
        {
            PaymentAdvice = 1,
            QADebitNote = 2
        }

        public GenerateFileRequestDef()
        { }

        public GenerateFileRequestDef(string fileName, int fileType, UserRef submitBy)
        {
            this.requestId = -1;
            this.fileName = fileName;
            this.fileType = fileType;            
            this.submitUser = submitBy ;
            this.submitDate = DateTime.Now;
            this.status = RequestStatus.PENDING.StatusId;
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

        public int FileType
        {
            get { return fileType ; }
            set { fileType = value; }
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
