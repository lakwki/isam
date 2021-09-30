using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
	[Serializable()]
	public class VendorConvertDef : DomainData
	{
        private int vendorUploadRefId;
		private int vendorId;
		private string originalFileName;
		private string uploadedPath;
        private DateTime uploadedOn;
        private string remark;
        private int requiredFormId;
        private int versionNo;
        private bool isContainsAll;
        private bool isUploadDMS;

        public VendorConvertDef()
		{
		}

        public int VendorUploadRefId 
        {
            get { return vendorUploadRefId; }
            set { vendorUploadRefId = value; } 
        }

        public bool IsContainsAll
        {
            get { return isContainsAll; }
            set { isContainsAll = value; }
        }

        public bool IsUploadDMS
        {
            get { return isUploadDMS; }
            set { isUploadDMS = value; }
        }

        public int VendorId 
        {
            get { return vendorId; }
            set { vendorId = value; } 
        }

        public int VersionNo
        {
            get { return versionNo; }
            set { versionNo = value; }
        }

        public DateTime UploadedOn 
        {
            get { return uploadedOn; }
            set { uploadedOn = value; } 
        }


        public string OriginalFileName
        {
            get { return originalFileName; }
            set { originalFileName = value; }
        }

        public string UploadedPath 
        {
            get { return uploadedPath; }
            set { uploadedPath = value; } 
        }

        public string Remark 
        {
            get { return remark; }
            set { remark = value; } 
        }

        public int RequiredFormId
        {
            get { return requiredFormId; }
            set { requiredFormId = value; }
        }

	}
}
