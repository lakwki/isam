using com.next.isam.dataserver.model.nss;

namespace com.next.isam.dataserver.model.sync
{
    public class DomainShipmentSyncDef
    {
        private NssContractDs dsContract;
        private NssShipmentDs dsShipment;
        private NssShipmentDetailDs dsShipmentDetail;
        private NssSizeOptionDs dsSizeOption;
        private NssSplitShipmentDs dsSplitShipment;
        private NssSplitShipmentDetailDs dsSplitShipmentDetail;
        private NssOtherCostDs dsOtherCost;
        private NssOtherCostDs dsSplitOtherCost;
        private NssProductDs dsProduct;

        public NssContractDs ContractDataSet 
        { 
            get { return dsContract; } 
            set { dsContract = value; } 
        }

        public NssShipmentDs ShipmentDataSet 
        { 
            get { return dsShipment; } 
            set { dsShipment = value; } 
        }

        public NssShipmentDetailDs ShipmentDetailDataSet 
        { 
            get { return dsShipmentDetail; } 
            set { dsShipmentDetail = value; } 
        }

        public NssSizeOptionDs SizeOptionDataSet 
        { 
            get { return dsSizeOption; } 
            set { dsSizeOption = value; } 
        }

        public NssOtherCostDs OtherCostDataSet 
        { 
            get { return dsOtherCost; } 
            set { dsOtherCost = value; } 
        }

        public NssProductDs ProductDataSet 
        { 
            get { return dsProduct; } 
            set { dsProduct = value; } 
        }

        public NssSplitShipmentDs SplitShipmentDataSet 
        { 
            get { return dsSplitShipment; } 
            set { dsSplitShipment = value; } 
        }

        public NssSplitShipmentDetailDs SplitShipmentDetailDataSet 
        { 
            get { return dsSplitShipmentDetail; } 
            set { dsSplitShipmentDetail = value; } 
        }

        public NssOtherCostDs SplitOtherCostDataSet 
        { 
            get { return dsSplitOtherCost; } 
            set { dsSplitOtherCost = value; } 
        }
    }
}
