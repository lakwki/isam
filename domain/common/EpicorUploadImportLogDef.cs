using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class EpicorUploadImportLogDef : DomainData
    {
        private int epicorUploadImportLogId;
        private string epicorCompany;
        private string interfaceModule;
        private string nSSourceType;
        private string documentType;
        private string groupId;
        private string originalFileName;
        private string fileName;
        private string logFilePath;
        private DateTime uploadedOn;
        private DateTime completedOn;
        private string uploadStatus;
        private string uploadRejectReason;
        private string importStatus;
        private int status;
        private DateTime createdOn;
        private int createdBy;
        private int processQueue;

        public int EpicorUploadImportLogId
        {
            get { return epicorUploadImportLogId; }
            set { epicorUploadImportLogId = value; }
        }

        public string EpicorCompany
        {
            get { return epicorCompany; }
            set { epicorCompany = value; }
        }

        public string InterfaceModule
        {
            get { return interfaceModule; }
            set { interfaceModule = value; }
        }

        public string NSSourceType
        {
            get { return nSSourceType; }
            set { nSSourceType = value; }
        }

        public string DocumentType
        {
            get { return documentType; }
            set { documentType = value; }
        }

        public string GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        public string OriginalFileName
        {
            get { return originalFileName; }
            set { originalFileName = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string LogFilePath
        {
            get { return logFilePath; }
            set { logFilePath = value; }
        }

        public DateTime UploadedOn
        {
            get { return uploadedOn; }
            set { uploadedOn = value; }
        }

        public DateTime CompletedOn
        {
            get { return completedOn; }
            set { completedOn = value; }
        }

        public string UploadStatus
        {
            get { return uploadStatus; }
            set { uploadStatus = value; }
        }

        public string UploadRejectReason
        {
            get { return uploadRejectReason; }
            set { uploadRejectReason = value; }
        }

        public string ImportStatus
        {
            get { return importStatus; }
            set { importStatus = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }

        public int CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        public int ProcessQueue
        {
            get { return processQueue; }
            set { processQueue = value; }
        }


    }
}
