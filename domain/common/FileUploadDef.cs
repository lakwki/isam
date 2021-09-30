using System;
using System.Collections.Generic;
using System.Text;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class FileUploadDef
    {
        private int fileId;
        private string fileDescription;
        private string physicalFileName;
        private int shipmentId;
        private int claimId;
        private int status;
        private UserRef createdBy;
        private DateTime createdOn;

        public int FileId
        {
            get { return fileId; }
            set { fileId = value; }
        }

        public string FileDescription
        {
            get { return fileDescription; }
            set { fileDescription = value; }
        }

        public string PhysicalFileName
        {
            get { return physicalFileName; }
            set { physicalFileName = value; }
        }

        public int ShipmentId
        {
            get { return shipmentId; }
            set { shipmentId = value; }
        }

        public int ClaimId
        {
            get { return claimId; }
            set { claimId = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public UserRef CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { createdOn = value; }
        }
    }
}
