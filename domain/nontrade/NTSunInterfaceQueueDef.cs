using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public class NTSunInterfaceQueueDef : DomainData
    {
        private int queueId;
        private OfficeRef office;
        private int sunInterfaceTypeId;
        private CategoryType categoryType;
        private int fiscalYear;
        private int period;
        UserRef user;
        private int sourceId;
        DateTime submitTime;
        DateTime completeTime;
        private string journalNo = String.Empty;
        int status;

        public NTSunInterfaceQueueDef() { }

        public int QueueId 
        { 
            get { return queueId; } 
            set { queueId = value; } 
        }

        public OfficeRef Office
        { 
            get { return office; }
            set { office = value; } 
        }

        public int SunInterfaceTypeId 
        { 
            get { return sunInterfaceTypeId; } 
            set { sunInterfaceTypeId = value; } 
        }

        public CategoryType CategoryType 
        { 
            get { return categoryType; } 
            set { categoryType = value; } 
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

        public UserRef User 
        { 
            get { return user; } 
            set { user = value; } 
        }

        public int SourceId
        {
            get { return sourceId; }
            set { sourceId = value; }
        }

        public string JournalNo
        {
            get { return journalNo; }
            set { journalNo = value; }
        }

        public DateTime SubmitTime 
        { 
            get { return submitTime; } 
            set { submitTime = value; } 
        }

        public DateTime CompleteTime 
        { 
            get { return completeTime; } 
            set { completeTime = value; } 
        }

        public int Status 
        { 
            get { return status; } 
            set { status = value; } 
        }
    }
}
