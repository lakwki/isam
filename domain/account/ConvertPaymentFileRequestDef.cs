using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;


namespace com.next.isam.domain.account
{
    [Serializable()]
    public class ConvertPaymentFileRequestDef : DomainData 
    {
        private int requestId;
        private string fileName;
        private int bank;
        private int chargeMethod;
        private UserRef submitBy;
        private DateTime submittedDate;
        private int status;

        public ConvertPaymentFileRequestDef()
        { }

        public ConvertPaymentFileRequestDef(string fileName, int bank, int chargeMethod, UserRef submitBy)
        {
            this.requestId = -1;
            this.fileName = fileName;
            this.bank = bank;
            this.chargeMethod = chargeMethod;
            this.submitBy = submitBy;
            this.submittedDate = DateTime.Now;
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

        public int Bank
        {
            get { return bank; }
            set { bank = value; }
        }

        public int ChargeMethod
        {
            get { return chargeMethod; }
            set { chargeMethod = value; }
        }

        public UserRef SubmitBy
        {
            get { return submitBy; }
            set { submitBy = value; }
        }

        public DateTime SubmittedDate
        {
            get { return submittedDate; }
            set { submittedDate = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }





    }
}
