using System;
using com.next.isam.domain.types;
using com.next.common.domain;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class ShipmentSyncRef : DomainData
    {
        private int shipmentId;
        private ContractWFS workflowStatus;

        public int ShipmentId { get { return shipmentId; } set { shipmentId = value; } }
        public ContractWFS WorkflowStatus { get { return workflowStatus; } set { workflowStatus = value;} }
    }
}
