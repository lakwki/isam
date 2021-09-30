using System.Collections;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.order;

namespace com.next.isam.appserver.order
{
    public class OrderManager
    {
        private static OrderManager _instance;
        private OrderSelectWorker orderSelectWorker;
        private OrderWorker orderWorker;

        public OrderManager()
        {            
            orderSelectWorker = OrderSelectWorker.Instance;
            orderWorker = OrderWorker.Instance;
        }

        public static OrderManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OrderManager();
                }
                return _instance;
            }
        }

        public ContractDef getContractByContractNo(string contractNo)
        {
            return orderSelectWorker.getContractByContractNo(contractNo);
        }

        public ContractDef getContractByKey(int contractId)
        {
            return orderSelectWorker.getContractByKey(contractId);
        }

        public ShipmentDef getShipmentByKey(int shipmentId)
        {
            return orderSelectWorker.getShipmentByKey(shipmentId);
        }

        public ICollection getSplitShipmentByShipmentId(int shipmentId)
        {
            return orderSelectWorker.getSplitShipmentByShipmentId(shipmentId);
        }

        public SplitShipmentDef getSplitShipmentByKey(int splitShipmentId)
        {
            return orderSelectWorker.getSplitShipmentByKey(splitShipmentId);
        }


        public ArrayList getShipmentListByContractNo(string contractNo)
        {
            return orderSelectWorker.getShipmentListByContractNo(contractNo);
        }

        public void updateSplitShipmentList(ICollection splitShipments, int userId)
        {
            orderWorker.updateSplitShipmentList(splitShipments, userId);
        }

        /*
        public void updateShipmentDetailList(ArrayList shipmentDetails, int userId)
        {
            orderWorker.updateShipmentDetailList(shipmentDetails, userId);
        }

        public void updateSplitShipmentDetailList(ICollection splitShipmentDetails, int userId)
        {
            orderWorker.updateSplitShipmentDetailList(splitShipmentDetails, userId);
        }

        public void updateShipmentSummaryTotal(int shipmentId, int userId)
        {
            ShippingWorker.Instance.updateShipmentSummaryTotal(shipmentId, userId);
        }
        */

        public ShipmentDef getShipmentByContractNoAndDeliveryNo(string contractNo, int deliveryNo)
        {
            return orderSelectWorker.getShipmentByContractNoAndDeliveryNo(contractNo, deliveryNo);
        }

    }
}
