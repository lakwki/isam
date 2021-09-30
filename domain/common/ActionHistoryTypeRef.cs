using System;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.common
    
{

    [Serializable()]
    public class ActionHistoryTypeRef : DomainData
    {
        private int actionTypeId;
        private int actionCategoryId;
        private string actionTypeDesc;
        private int status;


        public ActionHistoryTypeRef()
        {
        }

        public int ActionTypeId
        {
            get { return actionTypeId; }
            set {actionTypeId = value;}
        }

        public int ActionCategoryId
        {
            get { return actionCategoryId; }
            set { actionCategoryId = value; }
        }

        public string ActionTypeDesc
        {
            get { return actionTypeDesc; }
            set { actionTypeDesc = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
