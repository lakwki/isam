using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class SunInterfaceQueueDef : DomainData
    {
        private int queueId;
        private ReportOfficeGroupRef officeGroup;
        private int sunInterfaceTypeId;
        private CategoryType categoryType;
        private int purchaseTerm;
        private int uTurn;
        private int fiscalYear;
        private int period;
        UserRef user;
        private int sourceId;
        DateTime submitTime;
        DateTime completeTime;
        private string journalNo = String.Empty;
        int status;

        public SunInterfaceQueueDef() { }

        public int QueueId 
        { 
            get { return queueId; } 
            set { queueId = value; }  
        }

        public ReportOfficeGroupRef OfficeGroup 
        { 
            get { return officeGroup; } 
            set { officeGroup = value; } 
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

        public int PurchaseTerm
        {
            get { return purchaseTerm; }
            set { purchaseTerm = value; }
        }

        public int UTurn
        {
            get { return uTurn; }
            set { uTurn = value; }
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

        public string UTurnText
        {
            get
            {
                string s = string.Empty;
                if (UTurn == 2) s = "[Non-UT]";
                else if (UTurn == 1) s = "[UT]";
                return s;
            }
        }
    }
}
