using System;
using com.next.common.domain;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class LCApplicationRef : DomainData  
    {
        private int lcApplicationId;
        private int lcApplicationNo;
        private UserRef createUser;
        private DateTime createDate;

        public int LCApplicationId
        {
            get { return lcApplicationId; }
            set { lcApplicationId = value; }
        }

        public int LCApplicationNo
        {
            get { return lcApplicationNo; }
            set { lcApplicationNo = value; }
        }

        public UserRef CreateUser
        {
            get { return createUser; }
            set { createUser = value; }
        }

        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }

        public int Status { get; set; }        
    }
}
