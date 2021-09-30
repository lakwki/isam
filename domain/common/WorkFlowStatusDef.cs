using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class WorkFlowStatusDef : DomainData
    {
        private int recordId;
        private int workflowStatusId;
        private string description;
        private int recordTypeId;

        public WorkFlowStatusDef()
        {
        }

        public int RecordId
        {
            get { return recordId; }
            set { recordId = value; }
        }

        public int WorkflowStatusId
        {
            get { return workflowStatusId; }
            set { workflowStatusId = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public int RecordTypeId
        {
            get { return recordTypeId; }
            set { recordTypeId = value; }
        }
    }
}
