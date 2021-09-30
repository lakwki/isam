using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class FileUploadLogDef : DomainData
    {
        public enum Type
        {
            ShippingDataUpload = 1,
            NSLedNSLedSalesDataUpload = 2
        }

        private int recordId;
        private int fileTypeId;
        private string fileName;
        private string filePath;
        private int isUploaded;
        private int status;
        private UserRef submittedBy;
        private DateTime submittedOn;
        private DateTime uploadedOn;

        public FileUploadLogDef()
        {
            this.recordId = -1;
            this.isUploaded = 0;
            this.status = 1;
        }

        public FileUploadLogDef(int fileTypeId, string fileName, string filePath, UserRef submittedBy)
        {
            this.recordId = -1;
            this.fileTypeId = fileTypeId;
            this.fileName = fileName;
            this.filePath = filePath;
            this.isUploaded = 0;
            this.status = 1;
            this.submittedBy = submittedBy;
            this.submittedOn = DateTime.Now;
            this.uploadedOn = DateTime.MinValue;
        }

        public int RecordId
        {
            get { return recordId; }
            set { recordId = value; }
        }

        public int FileTypeId
        {
            get { return fileTypeId; }
            set { fileTypeId = value; }
        }

        public string FileName
        {
            get { return fileName;  }
            set { fileName = value; }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public int IsUploaded
        {
            get { return isUploaded; }
            set { isUploaded = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public UserRef SubmittedBy
        {
            get { return submittedBy; }
            set { submittedBy = value; }
        }

        public DateTime SubmittedOn
        {
            get { return submittedOn; }
            set { submittedOn = value; }
        }

        public DateTime UploadedOn
        {
            get { return uploadedOn; }
            set { uploadedOn = value; }
        }
    }
}
