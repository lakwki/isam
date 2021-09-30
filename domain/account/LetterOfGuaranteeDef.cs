using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class LetterOfGuaranteeDef : DomainData
    {
        private int lgId;
        private string lgNo;
        private DateTime lgDate;
        private int officeId;
        private int vendorId;
        private int uploadedBy;
        private DateTime uploadedDate;
        private int submittedBy;
        private DateTime submittedDate;
        private string remark;
        private int status;
        private int createdBy;
        private DateTime createdOn;
        private int modifiedBy;
        private DateTime modifiedOn;


        public int LGId
        {
            get { return lgId; }
            set { lgId = value; }
        }

        public string LGNo
        {
            get { return lgNo; }
            set { lgNo = value; }
        }

        public DateTime LGDate
        {
            get { return lgDate; }
            set { lgDate = value; }
        }

        public int OfficeId
        {
            get { return officeId; }
            set { officeId = value; }
        }

        public int VendorId
        {
            get { return vendorId; }
            set { vendorId = value; }
        }

        public int UploadedBy
        {
            get { return uploadedBy; }
            set { uploadedBy = value; }
        }

        public DateTime UploadedDate
        {
            get { return uploadedDate; }
            set { uploadedDate = value; }
        }

        public int SubmittedBy
        {
            get { return submittedBy; }
            set { submittedBy = value; }
        }

        public DateTime SubmittedDate
        {
            get { return submittedDate; }
            set { submittedDate = value; }
        }

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public int CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }

        public int ModifiedBy
        {
            get { return modifiedBy; }
            set { modifiedBy = value; }
        }

        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { modifiedOn = value; }
        }

        public static DateTime getLGDueDate(DateTime invoiceDate)
        {
            if (invoiceDate != DateTime.MinValue)
                return invoiceDate.AddDays(60);
            else
                return DateTime.MinValue;
        }

    }
}
