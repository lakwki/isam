using System;
using System.Collections;
using System.Data;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.infra.persistency.dataaccess;
using com.next.isam.dataserver.model.order;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.common;

namespace com.next.isam.dataserver.worker
{
	public class OrderSelectWorker : Worker
	{
        private static OrderSelectWorker _instance;
		private GeneralWorker generalWorker;
		private CommonWorker commonWorker;
		private VendorWorker vendorWorker;
		private ProductWorker productWorker;
		
		protected OrderSelectWorker()
		{
			generalWorker = GeneralWorker.Instance;
			commonWorker = CommonWorker.Instance;
			vendorWorker = VendorWorker.Instance;
			productWorker = ProductWorker.Instance;
		}

		public static OrderSelectWorker Instance
		{
			get 
			{
                if (_instance == null)
    				_instance = new OrderSelectWorker();
				return _instance;
			}
		}

		public ContractDef getContractByContractNo(string contractNo) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ContractApt", "GetContractByContractNo");
			ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;

			ContractDs dataSet = new ContractDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ContractDef def = new ContractDef();
			ContractMapping(dataSet.Contract[0], def);
			return def;
		}

        public ContractDef getContractByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractApt", "GetContractByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            ContractDs dataSet = new ContractDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ContractDef def = new ContractDef();
            ContractMapping(dataSet.Contract[0], def);
            return def;
        }
		
		public ContractDef getContractByKey(int key) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ContractApt", "GetContractByKey");
			ad.SelectCommand.Parameters["@ContractId"].Value = key.ToString();

			ContractDs dataSet = new ContractDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ContractDef def = new ContractDef();
			ContractMapping(dataSet.Contract[0], def);
			return def;
		}

        public ContractDef getContractByItemNoAndCustomerId(string itemNo, int CustomerId, int isSelfBilling, int isInvoiced)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractApt", "GetContractByItemNoAndCustomerId");
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@CustomerId"].Value = CustomerId.ToString();
            ad.SelectCommand.Parameters["@IsSelfBilling"].Value = isSelfBilling.ToString();
            ad.SelectCommand.Parameters["@IsInvoiced"].Value = isInvoiced.ToString();

            ContractDs dataSet = new ContractDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ContractDef def = new ContractDef();
            ContractMapping(dataSet.Contract[0], def);
            return def;
        }

        public decimal getNSLedUnitPrice(string itemNo, string sizeOptionNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ContractApt", "GetNSLedUnitPrice");
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@SizeOptionNo"].Value = sizeOptionNo;

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return 0;

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (decimal)(dataSet.Tables[0].Rows[0][0]);
        }

		public ArrayList getContractListByCriteria(int officeId, int deptId, int productTeamId, int seasonId, int vendorId, int productId, TypeCollector workflowStatusList)
		{
			IDataSetAdapter ad = getDataSetAdapter("ContractApt", "GetContractListByCriteria");
			ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
			ad.SelectCommand.Parameters["@DeptId"].Value = deptId;
			ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId;
			ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
			ad.SelectCommand.Parameters["@ProductId"].Value = productId;
			ad.SelectCommand.CustomParameters["@WorkflowStatusList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);

			ContractDs dataSet = new ContractDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ContractDs.ContractRow row in dataSet.Contract)
			{
				ContractDef pDef = new ContractDef();
				ContractMapping(row, pDef);
				list.Add(pDef);
			}
			return list;
		}

		public ShipmentRef convertShipmentDefToShipmentRef(ShipmentDef def)
		{
			ShipmentRef rf = new ShipmentRef();
			rf.ShipmentId = def.ShipmentId;
			ShipmentMapping(def, rf);
			return rf;
		}

		public SplitShipmentDef getSplitShipmentByKey(int key) 
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitShipmentApt", "GetSplitShipmentByKey");
			ad.SelectCommand.Parameters["@SplitShipmentId"].Value = key.ToString();

			SplitShipmentDs dataSet = new SplitShipmentDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			SplitShipmentDef def = new SplitShipmentDef();
			SplitShipmentMapping(dataSet.SplitShipment[0], def);
			return def;
		}

		public ICollection getSplitShipmentByShipmentId(int shipmentId)
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitShipmentApt", "GetSplitShipmentByShipmentId");
			ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

			SplitShipmentDs dataSet = new SplitShipmentDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(SplitShipmentDs.SplitShipmentRow row in dataSet.SplitShipment)
			{
				SplitShipmentDef def = new SplitShipmentDef();
				SplitShipmentMapping(row, def);
				list.Add(def);
			}
			return list;
		}

        public SplitShipmentDef getSplitShipmentByPONo(string contractNo, string splitSuffix, int deliveryNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("SplitShipmentApt", "GetSplitShipmentByPONo");
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@SplitSuffix"].Value = splitSuffix;
            ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;

            SplitShipmentDs dataSet = new SplitShipmentDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0)
                return null;

            SplitShipmentDef def = new SplitShipmentDef();

            if (recordsAffected > 0)
            {                
                SplitShipmentMapping(dataSet.SplitShipment.Rows[0], def);
            }
            return def;
        }


		public ICollection getSplitShipmentByContractIdAndSplitProductIdAndWorkflowStatus(int contractId, int splitProductId, TypeCollector workflowStatusList)
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitShipmentApt", "GetSplitShipmentByContractIdAndSplitProductIdAndWorkflowStatus");
			ad.SelectCommand.Parameters["@ContractId"].Value = contractId;
			ad.SelectCommand.Parameters["@SplitProductId"].Value = splitProductId;
			ad.SelectCommand.CustomParameters["@WorkflowStatusList"]=CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);

			SplitShipmentDs dataSet = new SplitShipmentDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(SplitShipmentDs.SplitShipmentRow row in dataSet.SplitShipment)
			{
				SplitShipmentDef def = new SplitShipmentDef();
				SplitShipmentMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public SplitShipmentRef convertSplitShipmentDefToSplitShipmentRef(SplitShipmentDef def)
		{
			SplitShipmentRef rf = new SplitShipmentRef();
			SplitShipmentMapping(def, rf);
			return rf;
		}

		public ShipmentDef getShipmentByKey(int key) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByKey");
			ad.SelectCommand.Parameters["@ShipmentId"].Value = key.ToString();

			ShipmentDs dataSet = new ShipmentDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ShipmentDef def = new ShipmentDef();
			ShipmentMapping(dataSet.Shipment[0], def);
			return def;
		}

		public ICollection getShipmentByContractId(int contractId)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByContractId");
			ad.SelectCommand.Parameters["@ContractId"].Value = contractId;

			ShipmentDs dataSet = new ShipmentDs();
			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ShipmentDs.ShipmentRow row in dataSet.Shipment)
			{
				ShipmentDef def = new ShipmentDef();
				ShipmentMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public ShipmentDef getShipmentByShipmentIdAndWorkflowStatus(int shipmentId, TypeCollector workflowStatusList)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByShipmentIdAndWorkflowStatus");
			ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
			ad.SelectCommand.CustomParameters["@WorkflowStatusList"]=CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);

			ShipmentDs dataSet = new ShipmentDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ShipmentDef def = new ShipmentDef();
			ShipmentMapping(dataSet.Shipment[0], def);
			return def;
		}

		public ICollection getShipmentByContractIdAndWorkflowStatus(int contractId, TypeCollector workflowStatusList)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByContractIdAndWorkflowStatus");
			ad.SelectCommand.Parameters["@ContractId"].Value = contractId;
			ad.SelectCommand.CustomParameters["@WorkflowStatusList"]=CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);

			ShipmentDs dataSet = new ShipmentDs();
			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ShipmentDs.ShipmentRow row in dataSet.Shipment)
			{
				ShipmentDef def = new ShipmentDef();
				ShipmentMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public int getShipmentCountByContractIdAndWorkflowStatus(int contractId, TypeCollector workflowStatusList)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentCountByContractIdAndWorkflowStatus");
			ad.SelectCommand.Parameters["@ContractId"].Value = contractId;
			ad.SelectCommand.CustomParameters["@WorkflowStatusList"]=CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);

			DataSet dataSet = new DataSet();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return 0;
			
			if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
				return 0;
			else
				return (int)(dataSet.Tables[0].Rows[0][0]);
		}



		public ShipmentDetailDef getShipmentDetailByKey(int key) 
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentDetailApt", "GetShipmentDetailByKey");
			ad.SelectCommand.Parameters["@ShipmentDetailId"].Value = key.ToString();

			ShipmentDetailDs dataSet = new ShipmentDetailDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ShipmentDetailDef def = new ShipmentDetailDef();
			ShipmentDetailMapping(dataSet.ShipmentDetail[0], def);
			return def;
		}

		public ICollection getShipmentDetailByShipmentId(int shipmentId)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentDetailApt", "GetShipmentDetailByShipmentId");
			ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

			ShipmentDetailDs dataSet = new ShipmentDetailDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ShipmentDetailDs.ShipmentDetailRow row in dataSet.ShipmentDetail)
			{
				ShipmentDetailDef def = new ShipmentDetailDef();
				ShipmentDetailMapping(row, def);
				list.Add(def);
			}
			return list;
		}


		public SplitShipmentDetailDef getSplitShipmentDetailByKey(int key) 
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitShipmentDetailApt", "GetSplitShipmentDetailByKey");
			ad.SelectCommand.Parameters["@SplitShipmentDetailId"].Value = key.ToString();

			SplitShipmentDetailDs dataSet = new SplitShipmentDetailDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			SplitShipmentDetailDef def = new SplitShipmentDetailDef();
			SplitShipmentDetailMapping(dataSet.SplitShipmentDetail[0], def);
			return def;
		}

		public ICollection getSplitShipmentDetailBySplitShipmentId(int splitShipmentId)
		{
			IDataSetAdapter ad = getDataSetAdapter("SplitShipmentDetailApt", "GetSplitShipmentDetailBySplitShipmentId");
			ad.SelectCommand.Parameters["@SplitShipmentId"].Value = splitShipmentId;

			SplitShipmentDetailDs dataSet = new SplitShipmentDetailDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(SplitShipmentDetailDs.SplitShipmentDetailRow row in dataSet.SplitShipmentDetail)
			{
				SplitShipmentDetailDef def = new SplitShipmentDetailDef();
				SplitShipmentDetailMapping(row, def);
				list.Add(def);
			}
			return list;
		}

        public ICollection getUpdatableSplitShipmentDetailByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SplitShipmentDetailApt", "GetUpdatableSplitShipmentDetailByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            SplitShipmentDetailDs dataSet = new SplitShipmentDetailDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (SplitShipmentDetailDs.SplitShipmentDetailRow row in dataSet.SplitShipmentDetail)
            {
                SplitShipmentDetailDef def = new SplitShipmentDetailDef();
                SplitShipmentDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

		public OtherCostDef getOtherCostByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("OtherCostApt", "GetOtherCostByKey");
			ad.SelectCommand.Parameters["@OtherCostId"].Value = key.ToString();

			OtherCostDs dataSet = new OtherCostDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			OtherCostDef def = new OtherCostDef();
			OtherCostMapping(dataSet.OtherCost[0], def);
			return def;
		}

		public ICollection getOtherCostByShipmentDetailId(int shipmentDetailId)
		{
			IDataSetAdapter ad = getDataSetAdapter("OtherCostApt", "GetOtherCostByShipmentDetailId");
			ad.SelectCommand.Parameters["@ShipmentDetailId"].Value = shipmentDetailId;

			OtherCostDs dataSet = new OtherCostDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(OtherCostDs.OtherCostRow row in dataSet.OtherCost)
			{
				OtherCostDef def = new OtherCostDef();
				OtherCostMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public SplitOtherCostDef getSplitOtherCostByKey(int key) 
		{
			IDataSetAdapter ad = getDataSetAdapter("OtherCostApt", "GetSplitOtherCostByKey");
			ad.SelectCommand.Parameters["@SplitOtherCostId"].Value = key.ToString();

			OtherCostDs dataSet = new OtherCostDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			SplitOtherCostDef def = new SplitOtherCostDef();
			SplitOtherCostMapping(dataSet.OtherCost[0], def);
			return def;
		}

		public ICollection getSplitOtherCostBySplitShipmentDetailId(int splitShipmentDetailId)
		{
			IDataSetAdapter ad = getDataSetAdapter("OtherCostApt", "GetSplitOtherCostBySplitShipmentDetailId");
			ad.SelectCommand.Parameters["@SplitShipmentDetailId"].Value = splitShipmentDetailId;

			OtherCostDs dataSet = new OtherCostDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(OtherCostDs.OtherCostRow row in dataSet.OtherCost)
			{
				SplitOtherCostDef def = new SplitOtherCostDef();
				SplitOtherCostMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		

		public ArrayList getShipmentListByCriteria(int officeId, int deptId, int productTeamId, int seasonId, int vendorId, int contractId, TypeCollector workflowStatusList)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentListByCriteria");

			ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
			ad.SelectCommand.Parameters["@DeptId"].Value = deptId;
			ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId;
			ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
			ad.SelectCommand.Parameters["@ContractId"].Value = contractId;
			ad.SelectCommand.CustomParameters["@WorkflowStatusList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);

			ShipmentDs dataSet = new ShipmentDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ShipmentDs.ShipmentRow row in dataSet.Shipment)
			{
				ShipmentDef def = new ShipmentDef();
				ShipmentMapping(row, def);
				list.Add(def);
			}

			return list;
		}


		public ArrayList getShipmentDetailListByCriteria(int officeId, int deptId, int productTeamId, int seasonId, int vendorId, string designRefNo, TypeCollector workflowStatusList)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentDetailApt", "GetShipmentDetailListByCriteria");

			ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
			ad.SelectCommand.Parameters["@DeptId"].Value = deptId;
			ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;
			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId;
			ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
			ad.SelectCommand.Parameters["@DesignRef"].Value = designRefNo;
			ad.SelectCommand.CustomParameters["@WorkflowStatusList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);

			ShipmentDetailDs dataSet = new ShipmentDetailDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ShipmentDetailDs.ShipmentDetailRow row in dataSet.ShipmentDetail)
			{
				ShipmentDetailDef def = new ShipmentDetailDef();
				ShipmentDetailMapping(row, def);
				list.Add(def);
			}

			return list;
		}



		public ArrayList getSyncContractList()
		{
			IDataSetAdapter ad = getDataSetAdapter("ContractApt", "GetSyncContractList");

			ContractDs dataSet = new ContractDs();
			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);
			ArrayList list = new ArrayList();
			foreach(ContractDs.ContractRow row in dataSet.Contract)
			{
				ContractDef def = new ContractDef();
				ContractMapping(row, def);
				list.Add(def);
			}
			return list;
		}

		public ShipmentDef getShipmentByContractNoAndDeliveryNo(string contractNo, int deliveryNo)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByContractNoAndDeliveryNo");
			ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
			ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo.ToString();

			ShipmentDs dataSet = new ShipmentDs();
			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ShipmentDef def = new ShipmentDef();
			ShipmentMapping(dataSet.Shipment[0], def);
			return def;
		}

		public int getShipmentIdByContractNoAndDeliveryNoWithNOLOCK(string contractNo, int deliveryNo)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentIdByContractNoAndDeliveryNoWithNOLOCK");
			ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
			ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo.ToString();

			DataSet dataSet = new DataSet();
			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return 0;

			if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
				return 0;
			else
				return (int)(dataSet.Tables[0].Rows[0][0]);
		}



		#region Mapping Functions

		internal void ContractMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(ContractDs.ContractRow) &&
				target.GetType() == typeof(ContractDef))
			{
				ContractDs.ContractRow row = (ContractDs.ContractRow) source;
				ContractDef def = (ContractDef) target;

				def.ContractId = row.ContractId;
				def.ContractNo = row.ContractNo;
				if (!row.IsNSLPONoNull())
					def.NSLPONo = row.NSLPONo;
				else
                    def.NSLPONo = String.Empty;
				def.TradingAgencyId = row.TradingAgencyId;
				def.SupplierAssignTypeId = row.SupplierAssignTypeId;
				def.ProductId = row.ProductId;
                def.DeptId = row.DeptId;
				def.Season = generalWorker.getSeasonByKey(row.SeasonId);
				def.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
				def.Merchandiser = generalWorker.getUserByKey(row.MerchandiserId);
				def.ProductTeam = generalWorker.getProductCodeRefByKey(row.ProductTeamId);
				def.PhaseId = row.PhaseId;
				def.Customer = commonWorker.getCustomerByKey(row.CustomerId);
				if (!row.IsBookingRefNoNull()) 
					def.BookingRefNo = row.BookingRefNo;
				else
                    def.BookingRefNo = String.Empty;
				def.BookingReceivedDate = row.BookingReceivedDate;
				def.PiecesPerPack = row.PiecesPerPack;
				def.PackingUnit = commonWorker.getPackingUnitByKey(row.PackingUnitId);
				def.PackingMethod = commonWorker.getPackingMethodByKey(row.PackingMethodId);
				def.SetSplitCount = row.SetSplitCount;
                if (!row.IsUKSupplierCodeNull())
                {
                    /*
                    if (row.ContractId == 814526 || row.ContractId == 814527) // FE8428723, FE8428710, overrided as bulk duty orders. 2020-12-07
                        def.UKSupplierCode = "D63760";
                    else
                        def.UKSupplierCode = row.UKSupplierCode;
                    */
                    def.UKSupplierCode = row.UKSupplierCode;
                }
                else
                    def.UKSupplierCode = String.Empty;
				def.IsVirtualSetSplit = row.IsVirtualSetSplit;
				def.IsNextMfgOrder = row.IsNextMfgOrder;
				def.IsPOIssueToNextMfg = row.IsPOIssueToNextMfg;
				def.IsDualSourcingOrder = row.IsDualSourcingOrder;
                def.IsLDPOrder = row.IsLDPOrder;
                def.IsBizOrder = row.IsBizOrder;
                if (row.IsIsShortGameNull())
                    def.IsShortGame = 0;
                else
                    def.IsShortGame = row.IsShortGame;

                if (row.IsUKProductGroupCodeNull())
                    def.UKProductGroupCode = String.Empty;
                else
                    def.UKProductGroupCode = row.UKProductGroupCode;

                if (!row.IsIsEnvSustainableNull())
                    def.IsEnvSustainable = row.IsEnvSustainable;
                else
                    def.IsEnvSustainable = 0;

			} else 
            if (source.GetType() == typeof(ContractDef) &&  target.GetType() == typeof(ContractDs.ContractRow))
            {
                ContractDs.ContractRow row = (ContractDs.ContractRow)target;
                ContractDef def = (ContractDef)source;

                row.ContractId = def.ContractId;
                row.ContractNo = def.ContractNo;
                row.NSLPONo = def.NSLPONo;
                row.TradingAgencyId = def.TradingAgencyId;
                row.SupplierAssignTypeId = def.SupplierAssignTypeId;
                row.ProductId = def.ProductId;
                row.DeptId = def.DeptId;
                row.SeasonId = def.Season.SeasonId;
                row.OfficeId = def.Office.OfficeId;
                row.MerchandiserId = def.Merchandiser.UserId;
                row.ProductTeamId = def.ProductTeam.ProductCodeId;
                row.PhaseId = def.PhaseId;
                row.CustomerId = def.Customer.CustomerId;
                if (def.BookingRefNo == String.Empty)
                    row.SetBookingRefNoNull();
                else
                    row.BookingRefNo = def.BookingRefNo;
                row.BookingReceivedDate = def.BookingReceivedDate;
                row.PiecesPerPack = def.PiecesPerPack;
                row.PackingUnitId = def.PackingUnit.PackingUnitId;
                row.PackingMethodId = def.PackingMethod.PackingMethodId;
                row.SetSplitCount = def.SetSplitCount;
                row.UKSupplierCode = def.UKSupplierCode;
                row.IsVirtualSetSplit = def.IsVirtualSetSplit;
                row.IsNextMfgOrder = def.IsNextMfgOrder;
                row.IsPOIssueToNextMfg = def.IsPOIssueToNextMfg;
                row.IsDualSourcingOrder = def.IsDualSourcingOrder;
                row.IsLDPOrder = def.IsLDPOrder;
                row.IsBizOrder = def.IsBizOrder;
                row.IsShortGame = def.IsShortGame;
                if (def.UKProductGroupCode == String.Empty)
                    row.SetUKProductGroupCodeNull();
                else
                    row.UKProductGroupCode = def.UKProductGroupCode;

                if (def.IsEnvSustainable == int.MinValue)
                    row.SetIsEnvSustainableNull();
                else
                    row.IsEnvSustainable = def.IsEnvSustainable;
            }
		}

		internal void ShipmentMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(ShipmentDs.ShipmentRow) &&
				target.GetType() == typeof(ShipmentDef))
			{
				ShipmentDs.ShipmentRow row = (ShipmentDs.ShipmentRow) source;
				ShipmentDef def = (ShipmentDef) target;

				def.ShipmentId = row.ShipmentId;
				def.ContractId = row.ContractId;
				def.DeliveryNo = row.DeliveryNo;
				if (!row.IsNSLPONoNull())
                    def.NSLPONo = row.NSLPONo;
				else
                    def.NSLPONo = String.Empty;
				if (!row.IsVendorIdNull())
					def.Vendor = vendorWorker.getVendorByKey(row.VendorId);
                if (!row.IsVMVendorIdNull())
                    def.VMVendor = vendorWorker.getVendorByKey(row.VMVendorId);
                if (!row.IsFactoryIdNull())
                    def.FactoryId = row.FactoryId;
                else
                    def.FactoryId = 0;
                def.SupplierAssignTypeId = row.SupplierAssignTypeId;
				def.TermOfPurchase = commonWorker.getTermOfPurchaseByKey(row.TermOfPurchaseId);
                def.OriginalTermOfPurchase = commonWorker.getTermOfPurchaseByKey(row.OriginalTermOfPurchaseId);
				def.PurchaseLocation = commonWorker.getPurchaseLocationByKey(row.PurchaseLocationId);
				if (!row.IsUKAtWarehouseDateNull())
					def.UKAtWarehouseDate = row.UKAtWarehouseDate;
				else
					def.UKAtWarehouseDate = DateTime.MinValue;
				def.CustomerAgreedAtWarehouseDate = row.CustomerAtWarehouseDate;
				def.SupplierAgreedAtWarehouseDate = row.SupplierAtWarehouseDate;
				def.ShipmentMethod = commonWorker.getShipmentMethodByKey(row.ShipmentMethodId);
                if (!row.IsAirFreightPaymentTypeIdNull())
                    def.AirFreightPaymentType = commonWorker.getAirFreightPaymentTypeByKey(row.AirFreightPaymentTypeId);
				if (!row.IsOtherAirFreightPaymentRemarkNull()) 
					def.OtherAirFreightPaymentRemark = row.OtherAirFreightPaymentRemark;
				else
                    def.OtherAirFreightPaymentRemark = String.Empty;
				if (!row.IsAirFreightPaymentRemarkNull()) 
					def.AirFreightPaymentRemark = row.AirFreightPaymentRemark;
				else
                    def.AirFreightPaymentRemark = String.Empty;
				def.AgencyCommissionPercentage = row.AgencyCommissionPercent;
				if (!row.IsPaymentTermIdNull())
					def.PaymentTerm = commonWorker.getPaymentTermByKey(row.PaymentTermId);
				if (!row.IsQuotaCategoryGroupIdNull())
					def.QuotaCategoryGroup = commonWorker.getQuotaCategoryGroupByKey(row.QuotaCategoryGroupId);
				if (!row.IsCountryOfOriginIdNull()) 
					def.CountryOfOrigin = generalWorker.getCountryOfOriginByKey(row.CountryOfOriginId);
				if (!row.IsShipFromCountryIdNull()) 
					def.ShipmentCountry = commonWorker.getShipmentCountryByKey(row.ShipFromCountryId);
				if (!row.IsShipmentPortIdNull()) 
					def.ShipmentPort = commonWorker.getShipmentPortByKey(row.ShipmentPortId);
				def.SellCurrency = generalWorker.getCurrencyByKey(row.SellCurrencyId);
				if (row.SellCurrencyId == row.BuyCurrencyId)
					def.BuyCurrency = def.SellCurrency;
				else
					def.BuyCurrency = generalWorker.getCurrencyByKey(row.BuyCurrencyId);
				def.POExchangeRate = row.POExchangeRate;
				def.USExchangeRate = row.USExchangeRate;
				def.NSLCommissionPercentage = row.NSLCommissionPercent;
				def.VatPercent = row.VatPercent;
                if (!row.IsMockShopSampleRemarkNull())
                    def.MockShopSampleRemark = row.MockShopSampleRemark;
				else
                    def.MockShopSampleRemark = String.Empty;
                if (!row.IsNotesFromMerchandiserNull())
                    def.NotesFromMerchandiser = row.NotesFromMerchandiser;
                else
                    def.NotesFromMerchandiser = "";

				def.TotalOrderQuantity = row.TotalOrderQty;
				def.TotalOrderAmount = row.TotalOrderAmt;
                def.TotalOrderAmountAfterDiscount = row.TotalOrderAmtAfterDiscount;
				def.TotalPOQuantity = row.TotalPOQty;
				def.TotalPOAmount = row.TotalPOAmt;
                def.TotalPOAmountAfterDiscount = row.TotalPOAmtAfterDiscount;
				def.TotalPONetFOBAmount = row.TotalPONetFOBAmt;
                def.TotalPONetFOBAmountAfterDiscount = row.TotalPONetFOBAmtAfterDiscount;
                def.TotalPOSupplierGarmentAmount = row.TotalPOSupplierGmtAmt;
                def.TotalPOSupplierGarmentAmountAfterDiscount = row.TotalPOSupplierGmtAmtAfterDiscount;
                def.TotalNetFOBAmount = row.TotalNetFOBAmt;
                def.TotalNetFOBAmountAfterDiscount = row.TotalNetFOBAmtAfterDiscount;
                def.TotalSupplierGarmentAmount = row.TotalSupplierGmtAmt;
                def.TotalSupplierGarmentAmountAfterDiscount = row.TotalSupplierGmtAmtAfterDiscount;
                def.TotalOtherCost = row.TotalOtherCost;
                def.TotalOrderFreightCost = row.TotalOrderFreightCost;
                def.TotalOrderDutyCost = row.TotalOrderDutyCost;
                def.TotalOPAUpcharge = row.TotalOPAUpcharge;
				def.TotalShippedQuantity = row.TotalShippedQty;
				def.TotalShippedAmount = row.TotalShippedAmt;
                def.TotalShippedAmountAfterDiscount = row.TotalShippedAmtAfterDiscount;
				def.TotalShippedNetFOBAmount = row.TotalShippedNetFOBAmt;
                def.TotalShippedNetFOBAmountAfterDiscount = row.TotalShippedNetFOBAmtAfterDiscount;
				def.TotalShippedSupplierGarmentAmount = row.TotalShippedSupplierGmtAmt;
                def.TotalShippedSupplierGarmentAmountAfterDiscount = row.TotalShippedSupplierGmtAmtAfterDiscount;
				def.TotalShippedOtherCost = row.TotalShippedOtherCost;
                def.TotalShippedFreightCost = row.TotalShippedFreightCost;
                def.TotalShippedDutyCost = row.TotalShippedDutyCost;
                def.TotalShippedOPAUpcharge = row.TotalShippedOPAUpcharge;
				def.SplitCount = row.SplitCount;
                def.IsRepeatOrder = row.IsRepeatOrder;
                if (!row.IsDelayReasonTypesNull())
                    def.DelayReasonTypes = row.DelayReasonTypes;
                else
                    def.DelayReasonTypes = 0;
                if (!row.IsDelayReasonOtherNull())
                    def.DelayReasonOther = row.DelayReasonOther;
                else
                    def.DelayReasonOther = String.Empty;
                def.WorkflowStatus = ContractWFS.getType(row.WorkflowStatusId);
                def.SpecialOrderTypeId = row.SpecialOrderTypeId;
                def.IsMockShopSample = row.IsMockShopSample;
				def.IsPressSample = row.IsPressSample;
                def.IsStudioSample = row.IsStudioSample;
				if (!row.IsSalesForecastSpecialGroupIdNull())
					def.SalesForecastSpecialGroupId = row.SalesForecastSpecialGroupId;
				else
					def.SalesForecastSpecialGroupId = 0;
                def.VendorPaymentDiscountPercent = row.VendorPaymentDiscountPercent;
                if (!row.IsCustomDocTypeNull())
                    def.CustomDocType = row.CustomDocType;
                else
                    def.CustomDocType = 0;
                def.IsVirtualSetSplit = row.IsVirtualSetSplit;
                if (!row.IsThirdPartyAgencyIdNull())
                    def.ThirdPartyAgencyId = row.ThirdPartyAgencyId;
                else
                    def.ThirdPartyAgencyId = 0;
                def.IsRatioPackOrder = row.IsRatioPackOrder;
                def.IsUKDiscount = row.IsUKDiscount;
                def.WithOPRFabric = row.WithOPRFabric;
                /*
                if (!row.IsWithOPRFabricNull())
                    def.WithOPRFabric = row.WithOPRFabric;
                else
                    def.WithOPRFabric = 0;
                */
                if (!row.IsCustomerDestinationIdNull())
                    def.CustomerDestinationId = row.CustomerDestinationId;
                else
                    def.CustomerDestinationId = 0;
                def.SellingUTSurchargePercent = row.SellingUTSurchargePercent;
                def.FobUTSurchangePercent = row.FobUTSurchargePercent;
                def.ImportDutyPercent = row.ImportDutyPercent;
                def.QuarterlyExchangeRate = row.QuarterlyExchangeRate;
				def.IsNSLVMTROrder = row.IsNSLVMTROrder;
                def.CMCost = row.CMCost;
                def.QACommissionPercent = row.QACommissionPercent;
                //def.AdditionalBankChargesPercent = row.AdditionalBankChargesPercent;
                if (!row.IsGTCommissionPercentNull())
                    def.GTCommissionPercent = row.GTCommissionPercent;
                else
                    def.GTCommissionPercent = 0;
                if (!row.IsColourNull())
                    def.Colour = row.Colour;
                else
                    def.Colour = String.Empty;
                def.WithQCCharge = row.WithQCCharge;
                def.EditLock = row.EditLock;
                def.PaymentLock = row.PaymentLock;
                if (!row.IsUKDiscountReasonIdNull())
                    def.UKDiscountReasonId = row.UKDiscountReasonId;
                else
                    def.UKDiscountReasonId = 0;
                def.NUKAirFreightPaymentPercent = row.NUKAirFreightPaymentPercent;
                def.NSLAirFreightPaymentPercent = row.NSLAirFreightPaymentPercent;
                def.FTYAirFreightPaymentPercent = row.FTYAirFreightPaymentPercent;
                def.NSLSZAirFreightPaymentPercent = row.NSLSZAirFreightPaymentPercent;
                if (!row.IsOtherAirFreightPaymentRemarkNull())
                    def.OtherAirFreightPaymentRemark = row.OtherAirFreightPaymentRemark;
                def.OtherAirFreightPaymentPercent = row.OtherAirFreightPaymentPercent;
                def.ShippingDocWFS = ShippingDocWFS.getType(row.DMSWorkflowStatusId);
                if (row.IsDocReviewedOnNull())
                    def.DocumentReviewedOn = DateTime.MinValue;
                else
                    def.DocumentReviewedOn = row.DocReviewedOn;
                if (row.IsDocReviewedByNull())
                    def.DocumentReviewedBy = 0;
                else
                    def.DocumentReviewedBy = row.DocReviewedBy;
                if (row.IsRejectPaymentReasonIdNull())
                    def.RejectPaymentReasonId = 0;
                else
                    def.RejectPaymentReasonId = row.RejectPaymentReasonId;
                def.LabTestIncome = row.LabTestIncome;
                def.IsChinaGBTestRequired = row.IsChinaGBTestRequired;
                if (!row.IsIsTradingAFNull())
                    def.IsTradingAirFreight = row.IsTradingAF == 1 ? true : false;
                else
                    def.IsTradingAirFreight = false;
                if (!row.IsTradingAFActualCostNull())
                    def.TradingAirFreightActualCost = row.TradingAFActualCost;
                else
                    def.TradingAirFreightActualCost = 0;
                if (!row.IsTradingAFReasonNull())
                    def.TradingAirFreightReason = row.TradingAFReason;
                else
                    def.TradingAirFreightReason = string.Empty;
                if (!row.IsTradingAFTypeIdNull())
                    def.TradingAirFreightTypeId = row.TradingAFTypeId;
                else
                    def.TradingAirFreightTypeId = -1;
                if (!row.IsTradingAFEstimationCostNull())
                    def.TradingAirFreightEstimationCost = row.TradingAFEstimationCost;
                else
                    def.TradingAirFreightEstimationCost = 0;
                if (!row.IsNSLRefNoNull())
                    def.NSLRefNo = row.NSLRefNo;
                else
                    def.NSLRefNo = string.Empty;
                if (!row.IsGSPFormTypeIdNull())
                    def.GSPFormTypeId = row.GSPFormTypeId;
                else
                    def.GSPFormTypeId = 0;
			}
			else if (source.GetType() == typeof(ShipmentDef) &&
				target.GetType() == typeof(ShipmentDs.ShipmentRow))
			{
				ShipmentDef def = (ShipmentDef) source;
				ShipmentDs.ShipmentRow row = (ShipmentDs.ShipmentRow) target;

                row.ShipmentId = def.ShipmentId;
                row.ContractId = def.ContractId;
                row.DeliveryNo = def.DeliveryNo;
                if (def.NSLPONo.Trim() != String.Empty) 
                    row.NSLPONo = def.NSLPONo; 
                else
                    row.SetNSLPONoNull();

                /*
                if (def.Vendor != null)
                    row.VendorId = def.Vendor.VendorId;
                else
                    row.VendorId = 0;
                if (def.VMVendor != null)
                    row.VMVendorId = def.VMVendor.VendorId;
                else
                    row.SetVMVendorIdNull();
                */

                row.SupplierAssignTypeId = def.SupplierAssignTypeId;
                row.TermOfPurchaseId = def.TermOfPurchase.TermOfPurchaseId;
                row.OriginalTermOfPurchaseId = def.OriginalTermOfPurchase.TermOfPurchaseId;
                row.PurchaseLocationId = def.PurchaseLocation.PurchaseLocationId;
                row.CustomerAtWarehouseDate = def.CustomerAgreedAtWarehouseDate;
                row.SupplierAtWarehouseDate = def.SupplierAgreedAtWarehouseDate;
                row.ShipmentMethodId = def.ShipmentMethod.ShipmentMethodId;
                if (def.AirFreightPaymentType != null)
                    row.AirFreightPaymentTypeId = def.AirFreightPaymentType.AirFreightPaymentTypeId;
                else
                    row.SetAirFreightPaymentTypeIdNull();
                row.NUKAirFreightPaymentPercent = def.NUKAirFreightPaymentPercent;
                row.NSLAirFreightPaymentPercent = def.NSLAirFreightPaymentPercent;
                row.FTYAirFreightPaymentPercent = def.FTYAirFreightPaymentPercent;
                row.NSLSZAirFreightPaymentPercent = def.NSLSZAirFreightPaymentPercent;
                row.OtherAirFreightPaymentPercent = def.OtherAirFreightPaymentPercent;
                if (def.OtherAirFreightPaymentRemark.Trim() != String.Empty)
                    row.OtherAirFreightPaymentRemark = def.OtherAirFreightPaymentRemark;
                else
                    row.SetOtherAirFreightPaymentRemarkNull();
                if (def.AirFreightPaymentRemark.Trim() != String.Empty)
                    row.AirFreightPaymentRemark = def.AirFreightPaymentRemark;
                else
                    row.SetAirFreightPaymentRemarkNull();
                row.AgencyCommissionPercent = def.AgencyCommissionPercentage;
                if (def.PaymentTerm != null)
                    row.PaymentTermId = def.PaymentTerm.PaymentTermId;
                else
                    row.SetPaymentTermIdNull();
                if (def.QuotaCategoryGroup != null)
                    row.QuotaCategoryGroupId = def.QuotaCategoryGroup.QuotaCategoryGroupId;
                else
                    row.SetQuotaCategoryGroupIdNull();
                if (def.CountryOfOrigin != null)
                    row.CountryOfOriginId = def.CountryOfOrigin.CountryOfOriginId;
                else
                    row.SetCountryOfOriginIdNull();
                if (def.ShipmentCountry != null)
                    row.ShipFromCountryId = def.ShipmentCountry.ShipmentCountryId;
                else
                    row.SetShipFromCountryIdNull();
                if (def.ShipmentPort != null)
                    row.ShipmentPortId = def.ShipmentPort.ShipmentPortId;
                else
                    row.SetShipmentPortIdNull();
                row.SellCurrencyId = def.SellCurrency.CurrencyId;
                row.BuyCurrencyId = def.BuyCurrency.CurrencyId;
                row.POExchangeRate = def.POExchangeRate;
                row.USExchangeRate = def.USExchangeRate;
                row.NSLCommissionPercent = def.NSLCommissionPercentage;
                row.VatPercent = def.VatPercent;
                if (def.MockShopSampleRemark.Trim() != String.Empty)
                    row.MockShopSampleRemark = def.MockShopSampleRemark;
                else
                    row.SetMockShopSampleRemarkNull();
                if (def.NotesFromMerchandiser.Trim() != String.Empty)
                    row.NotesFromMerchandiser = def.NotesFromMerchandiser;
                else
                    row.SetNotesFromMerchandiserNull();
                row.TotalOrderQty = def.TotalOrderQuantity;
                row.TotalOrderAmt = def.TotalOrderAmount;
                row.TotalOrderAmtAfterDiscount = def.TotalOrderAmountAfterDiscount;
                row.TotalPOQty = def.TotalPOQuantity;
                row.TotalPOAmt = def.TotalPOAmount;
                row.TotalPOAmtAfterDiscount = def.TotalPOAmountAfterDiscount;
                row.TotalPONetFOBAmt = def.TotalPONetFOBAmount;
                row.TotalPONetFOBAmtAfterDiscount = def.TotalPONetFOBAmountAfterDiscount;
                row.TotalPOSupplierGmtAmt = def.TotalPOSupplierGarmentAmount;
                row.TotalPOSupplierGmtAmtAfterDiscount = def.TotalPOSupplierGarmentAmountAfterDiscount;
                row.TotalNetFOBAmt = def.TotalNetFOBAmount;
                row.TotalNetFOBAmtAfterDiscount = def.TotalNetFOBAmountAfterDiscount;
                row.TotalSupplierGmtAmt = def.TotalSupplierGarmentAmount;
                row.TotalSupplierGmtAmtAfterDiscount = def.TotalSupplierGarmentAmountAfterDiscount;
                row.TotalOtherCost = def.TotalOtherCost;
                row.TotalOrderFreightCost = def.TotalOrderFreightCost;
                row.TotalOrderDutyCost = def.TotalOrderDutyCost;
                row.TotalOPAUpcharge = def.TotalOPAUpcharge;
                row.TotalShippedQty = def.TotalShippedQuantity;
                row.TotalShippedAmt = def.TotalShippedAmount;
                row.TotalShippedAmtAfterDiscount = def.TotalShippedAmountAfterDiscount;
                row.TotalShippedNetFOBAmt = def.TotalShippedNetFOBAmount;
                row.TotalShippedNetFOBAmtAfterDiscount = def.TotalShippedNetFOBAmountAfterDiscount;
                row.TotalShippedSupplierGmtAmt = def.TotalShippedSupplierGarmentAmount;
                row.TotalShippedSupplierGmtAmtAfterDiscount = def.TotalShippedSupplierGarmentAmountAfterDiscount;
                row.TotalShippedOtherCost = Math.Round(def.TotalShippedOtherCost, 2, MidpointRounding.AwayFromZero);
                row.TotalShippedFreightCost = def.TotalShippedFreightCost;
                row.TotalShippedDutyCost = def.TotalShippedDutyCost;
                row.TotalShippedOPAUpcharge = def.TotalShippedOPAUpcharge;
                row.SplitCount = def.SplitCount;
                row.IsVirtualSetSplit = def.IsVirtualSetSplit;
                row.IsRepeatOrder = def.IsRepeatOrder;
                row.SpecialOrderTypeId = def.SpecialOrderTypeId;
                row.IsMockShopSample = def.IsMockShopSample;
                row.IsPressSample = def.IsPressSample;
                row.IsStudioSample = def.IsStudioSample;
                if (def.DelayReasonTypes != int.MinValue)
                    row.DelayReasonTypes = def.DelayReasonTypes;
                else
                    row.SetDelayReasonTypesNull();
                if (def.DelayReasonOther.Trim() != String.Empty)
                    row.DelayReasonOther = def.DelayReasonOther;
                else
                    row.SetDelayReasonOtherNull();
                row.WorkflowStatusId = def.WorkflowStatus.Id;
                row.SalesForecastSpecialGroupId = def.SalesForecastSpecialGroupId;
                row.VendorPaymentDiscountPercent = def.VendorPaymentDiscountPercent;
                row.CustomDocType = def.CustomDocType;
                row.ThirdPartyAgencyId = def.ThirdPartyAgencyId;
                row.CMCost = def.CMCost;
                row.IsRatioPackOrder = def.IsRatioPackOrder;
                row.IsUKDiscount = def.IsUKDiscount;
                row.UKDiscountReasonId = def.UKDiscountReasonId;
                row.WithOPRFabric = def.WithOPRFabric;
                row.CustomerDestinationId = def.CustomerDestinationId;
                row.SellingUTSurchargePercent = def.SellingUTSurchargePercent;
                row.FobUTSurchargePercent = def.FobUTSurchangePercent;
                row.ImportDutyPercent = def.ImportDutyPercent;
                row.QuarterlyExchangeRate = def.QuarterlyExchangeRate;
                row.IsNSLVMTROrder = def.IsNSLVMTROrder;
                row.QACommissionPercent = def.QACommissionPercent;
                /*
                row.AdditionalBankChargesPercent = def.AdditionalBankChargesPercent;
                */
                row.GTCommissionPercent = def.GTCommissionPercent;
                if (def.Colour.Trim() != String.Empty)
                    row.Colour = def.Colour;
                else
                    row.SetColourNull();
                row.WithQCCharge = def.WithQCCharge;
                row.EditLock = def.EditLock;
                row.PaymentLock = def.PaymentLock;
                row.DMSWorkflowStatusId = def.ShippingDocWFS.Id;
                if (def.DocumentReviewedOn == DateTime.MinValue)
                    row.SetDocReviewedOnNull();
                else
                    row.DocReviewedOn = def.DocumentReviewedOn;
                if (def.DocumentReviewedBy == 0)
                    row.SetDocReviewedByNull();
                else
                    row.DocReviewedBy = def.DocumentReviewedBy;
                if (def.RejectPaymentReasonId == 0)
                    row.SetRejectPaymentReasonIdNull();
                else
                    row.RejectPaymentReasonId = def.RejectPaymentReasonId;
                row.LabTestIncome = def.LabTestIncome;
                /*
                row.IsTradingAF = def.IsTradingAirFreight ? 1 : 0;
                if (def.TradingAirFreightReason.Trim() == string.Empty)
                    row.SetTradingAFReasonNull();
                else
                    row.TradingAFReason = def.TradingAirFreightReason;
                if (def.TradingAirFreightTypeId == -1)
                    row.SetTradingAFTypeIdNull();
                else
                    row.TradingAFTypeId = def.TradingAirFreightTypeId;
                row.TradingAFEstimationCost = def.TradingAirFreightEstimationCost;
                */
                row.TradingAFActualCost = def.TradingAirFreightActualCost;
            }
			else if (source.GetType() == typeof(ShipmentDef) &&
				target.GetType() == typeof(ShipmentRef))
			{
				ShipmentDef def = (ShipmentDef) source;
				ShipmentRef rf = (ShipmentRef) target;
			
				rf.DeliveryNo = def.DeliveryNo;
                if (def.Vendor != null) rf.SupplierName = def.Vendor.Name; else rf.SupplierName = String.Empty;
				rf.NoOfSplit = def.SplitCount;
				rf.IsVirtualSetSplit = def.IsVirtualSetSplit;
				rf.TotalOrderQuantity = def.TotalOrderQuantity;
				rf.CurrencyDescription = def.SellCurrency.CurrencyCode;
				rf.TotalOrderAmount = def.TotalOrderAmount;
				rf.TotalOPAUpcharge = def.TotalOPAUpcharge;
				decimal buyExRate, sellExRate;
				buyExRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, def.BuyCurrency.CurrencyId, DateTime.Today);
				sellExRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, def.SellCurrency.CurrencyId, DateTime.Today);
				rf.Margin = def.TotalOrderAmount - (def.TotalNetFOBAmount * buyExRate / sellExRate) - def.TotalOtherCost - (def.AgencyCommissionPercentage / 100 * def.TotalOrderAmount);
				ContractDef contractDef = this.getContractByKey(def.ContractId);
				double leadTime = ((TimeSpan)(def.CustomerAgreedAtWarehouseDate.Subtract(contractDef.BookingReceivedDate))).TotalDays;
				if ((leadTime % 7) > 0)
				{
					if ((leadTime % 7) >= 4)
						rf.LeadTime = Convert.ToDecimal(Math.Round((leadTime / 7), 0));
					else
						rf.LeadTime = Convert.ToDecimal(Math.Round((leadTime / 7), 0) + 0.5);
				}
				else
				{
					rf.LeadTime = Convert.ToDecimal(leadTime / 7);
				}
				rf.CustomerAgreedAtWarehouseDate = def.CustomerAgreedAtWarehouseDate;
				rf.WorkflowStatusDescription = def.WorkflowStatus.Name;
				rf.WorkflowStatusId = def.WorkflowStatus.Id;
			}
		}

		internal void SplitShipmentMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(SplitShipmentDs.SplitShipmentRow) &&
				target.GetType() == typeof(SplitShipmentDef))
			{
				SplitShipmentDs.SplitShipmentRow row = (SplitShipmentDs.SplitShipmentRow) source;
				SplitShipmentDef def = (SplitShipmentDef) target;

				def.SplitShipmentId = row.SplitShipmentId;
				def.ShipmentId = row.ShipmentId;
				def.SplitSuffix = row.SplitSuffix;
				def.ProductId = row.ProductId;
                def.Product = productWorker.getProductByKey(row.ProductId);
				def.PiecesPerPack = row.PiecesPerPack;
				def.PackingUnit = commonWorker.getPackingUnitByKey(row.PackingUnitId);
				if (!row.IsVendorIdNull())
					def.Vendor = vendorWorker.getVendorByKey(row.VendorId);
				else
					def.Vendor = null;
				def.SupplierAgreedAtWarehouseDate = row.SupplierAtWarehouseDate;
				if (!row.IsPaymentTermIdNull())
					def.PaymentTerm = commonWorker.getPaymentTermByKey(row.PaymentTermId);
				else
					def.PaymentTerm = null;
				if (!row.IsQuotaCategoryGroupIdNull())
					def.QuotaCategoryGroup = commonWorker.getQuotaCategoryGroupByKey(row.QuotaCategoryGroupId);
				else
					def.QuotaCategoryGroup = null;

                if (!row.IsCountryOfOriginIdNull())
                    def.CountryOfOrigin = generalWorker.getCountryOfOriginByKey(row.CountryOfOriginId);
                else
                    def.CountryOfOrigin = null;
				def.SellCurrency = generalWorker.getCurrencyByKey(row.SellCurrencyId);
				def.BuyCurrency = generalWorker.getCurrencyByKey(row.BuyCurrencyId);
                def.InvoiceBuyExchangeRate = row.InvoiceBuyExchangeRate;
				def.POExchangeRate = row.POExchangeRate;
				if (!row.IsMockShopSampleRemarkNull()) 
					def.MockShopSampleRemark = row.MockShopSampleRemark;
				else
                    def.MockShopSampleRemark = String.Empty;
				if (!row.IsOtherPOTermRemarkNull()) 
					def.OtherPOTermRemark = row.OtherPOTermRemark;
				else
                    def.OtherPOTermRemark = String.Empty;
				if (!row.IsShippingRemarkNull()) 
					def.ShippingRemark = row.ShippingRemark;
				else
                    def.ShippingRemark = String.Empty;
                if (!row.IsSupplierInvoiceNoNull())
                    def.SupplierInvoiceNo = row.SupplierInvoiceNo;
                else
                    def.SupplierInvoiceNo = String.Empty;
				def.TotalOrderQuantity = row.TotalOrderQty;
				def.TotalOrderAmount = row.TotalOrderAmt;
                def.TotalOrderAmountAfterDiscount = row.TotalOrderAmtAfterDiscount;
				def.TotalPOQuantity = row.TotalPOQty;
				def.TotalPOAmount = row.TotalPOAmt;
                def.TotalPOAmountAfterDiscount = row.TotalPOAmtAfterDiscount;
				def.TotalPONetFOBAmount = row.TotalPONetFOBAmt;
                def.TotalPONetFOBAmountAfterDiscount = row.TotalPONetFOBAmtAfterDiscount;
                def.TotalPOSupplierGarmentAmount = row.TotalPOSupplierGmtAmt;
                def.TotalPOSupplierGarmentAmountAfterDiscount = row.TotalPOSupplierGmtAmtAfterDiscount;
                def.TotalNetFOBAmount = row.TotalNetFOBAmt;
                def.TotalNetFOBAmountAfterDiscount = row.TotalNetFOBAmtAfterDiscount;
                def.TotalSupplierGarmentAmount = row.TotalSupplierGmtAmt;
                def.TotalSupplierGarmentAmountAfterDiscount = row.TotalSupplierGmtAmtAfterDiscount;
                def.TotalOPAUpcharge = row.TotalOPAUpcharge;
				def.TotalShippedQuantity = row.TotalShippedQty;
				def.TotalShippedAmount = row.TotalShippedAmt;
                def.TotalShippedAmountAfterDiscount = row.TotalShippedAmtAfterDiscount;
				def.TotalShippedNetFOBAmount = row.TotalShippedNetFOBAmt;
                def.TotalShippedNetFOBAmountAfterDiscount = row.TotalShippedNetFOBAmtAfterDiscount;
				def.TotalShippedSupplierGarmentAmount = row.TotalShippedSupplierGmtAmt;
                def.TotalShippedSupplierGarmentAmountAfterDiscount = row.TotalShippedSupplierGmtAmtAfterDiscount;
                def.TotalShippedOPAUpcharge = row.TotalShippedOPAUpcharge;
                def.VendorPaymentDiscountPercent = row.VendorPaymentDiscountPercent;
				def.IsVirtualSetSplit = row.IsVirtualSetSplit;				
				def.IsKnitwearComponent = row.IsKnitwearComponent;			
				def.IsFobOrder = row.IsFobOrder;
                def.QACommissionPercent = row.QACommissionPercent;
                if (!row.IsColourNull())
                    def.Colour = row.Colour;
                else
                    def.Colour = String.Empty;
                def.LCAmount = row.LCAmt;
                if (!row.IsLCNoNull())
                    def.LCNo = row.LCNo;
                else
                    def.LCNo = String.Empty;
                if (!row.IsLCIssueDateNull())
                    def.LCIssueDate = row.LCIssueDate;
                else
                    def.LCIssueDate = DateTime.MinValue;
                if (!row.IsLCExpiryDateNull())
                    def.LCExpiryDate = row.LCExpiryDate;
                else
                    def.LCExpiryDate = DateTime.MinValue;
                def.IsLCPaymentChecked = row.IsLCPaymentChecked;
                if (!row.IsLCPaymentCheckedDateNull())
                    def.LCPaymentCheckedDate = row.LCPaymentCheckedDate;
                else
                    def.LCPaymentCheckedDate = DateTime.MinValue;
                def.PaymentLock = row.PaymentLock;
                if (!row.IsAccountDocReceiptDateNull())
                    def.AccountDocReceiptDate = row.AccountDocReceiptDate;
                if (!row.IsShippingDocReceiptDateNull())
                    def.ShippingDocReceiptDate = row.ShippingDocReceiptDate;
                if (!row.IsAPRefNoNull())
                    def.APRefNo = row.APRefNo;
                def.APAmt = row.APAmt;
                if (!row.IsAPDateNull())
                    def.APDate = row.APDate;
                def.APExchangeRate = row.APExchangeRate;
                def.IsILSQtyUploadAllowed = row.IsILSQtyUploadAllowed;
                def.ShippingDocWFS = ShippingDocWFS.getType(row.DMSWorkflowStatusId);
                if (row.IsDocReviewedOnNull())
                    def.DocumentReviewedOn = DateTime.MinValue;
                else
                    def.DocumentReviewedOn = row.DocReviewedOn;
                if (row.IsDocReviewedByNull())
                    def.DocumentReviewedBy = 0;
                else
                    def.DocumentReviewedBy = row.DocReviewedBy;
                if (row.IsRejectPaymentReasonIdNull())
                    def.RejectPaymentReasonId = 0;
                else
                    def.RejectPaymentReasonId = row.RejectPaymentReasonId;
                if (!row.IsShippingDocCheckedByNull())
                    def.ShippingDocCheckedBy = generalWorker.getUserByKey(row.ShippingDocCheckedBy);
                def.ShippingCheckedTotalNetAmount = row.ShippingCheckedTotalNetAmount;
                if (!row.IsShippingDocCheckedOnNull())
                    def.ShippingDocCheckedOn = row.ShippingDocCheckedOn;
                else
                    def.ShippingDocCheckedOn = DateTime.MinValue;
                def.LabTestIncome = row.LabTestIncome;
			}
			else if (source.GetType() == typeof(SplitShipmentDef) &&
				target.GetType() == typeof(SplitShipmentDs.SplitShipmentRow))
			{
				SplitShipmentDef def = (SplitShipmentDef) source;
				SplitShipmentDs.SplitShipmentRow row = (SplitShipmentDs.SplitShipmentRow) target;

				row.SplitShipmentId = def.SplitShipmentId;
				row.ShipmentId = def.ShipmentId;
				row.SplitSuffix = def.SplitSuffix;
				row.ProductId = def.ProductId;
				row.PiecesPerPack = def.PiecesPerPack;
				row.PackingUnitId = def.PackingUnit.PackingUnitId;
				if (def.Vendor != null)
					row.VendorId = def.Vendor.VendorId;
				else
					row.SetVendorIdNull();
				row.SupplierAtWarehouseDate = def.SupplierAgreedAtWarehouseDate;
				if (def.PaymentTerm != null)
					row.PaymentTermId = def.PaymentTerm.PaymentTermId;
				else 
					row.SetPaymentTermIdNull();
				if (def.QuotaCategoryGroup != null)
					row.QuotaCategoryGroupId = def.QuotaCategoryGroup.QuotaCategoryGroupId;
				else
					row.SetQuotaCategoryGroupIdNull();
				if (def.CountryOfOrigin != null)
					row.CountryOfOriginId = def.CountryOfOrigin.CountryOfOriginId;
				else
					row.CountryOfOriginId = 0;
				row.SellCurrencyId = def.SellCurrency.CurrencyId;
				row.BuyCurrencyId = def.BuyCurrency.CurrencyId;
                row.InvoiceBuyExchangeRate = def.InvoiceBuyExchangeRate;
				row.POExchangeRate = def.POExchangeRate;
                if (def.MockShopSampleRemark.Trim() != String.Empty)
					row.MockShopSampleRemark = def.MockShopSampleRemark;
				else
					row.SetMockShopSampleRemarkNull();
                if (def.OtherPOTermRemark.Trim() != String.Empty)
					row.OtherPOTermRemark = def.OtherPOTermRemark;
				else
					row.SetOtherPOTermRemarkNull();
                if (def.ShippingRemark.Trim() != String.Empty)
					row.ShippingRemark = def.ShippingRemark;
				else
					row.SetShippingRemarkNull();
                if (def.SupplierInvoiceNo != String.Empty)
                    row.SupplierInvoiceNo = def.SupplierInvoiceNo;
                else
                    row.SetSupplierInvoiceNoNull();
				row.TotalOrderQty = def.TotalOrderQuantity;
				row.TotalOrderAmt = def.TotalOrderAmount;
                row.TotalOrderAmtAfterDiscount = def.TotalOrderAmountAfterDiscount;
				row.TotalPOQty = def.TotalPOQuantity;
				row.TotalPOAmt = def.TotalPOAmount;
                row.TotalPOAmtAfterDiscount = def.TotalPOAmountAfterDiscount;
				row.TotalPONetFOBAmt = def.TotalPONetFOBAmount;
                row.TotalPONetFOBAmtAfterDiscount = def.TotalPONetFOBAmountAfterDiscount;
                row.TotalPOSupplierGmtAmt = def.TotalPOSupplierGarmentAmount;
                row.TotalPOSupplierGmtAmtAfterDiscount = def.TotalPOSupplierGarmentAmountAfterDiscount;
                row.TotalNetFOBAmt = def.TotalNetFOBAmount;
                row.TotalNetFOBAmtAfterDiscount = def.TotalNetFOBAmountAfterDiscount;
                row.TotalSupplierGmtAmt = def.TotalSupplierGarmentAmount;
                row.TotalSupplierGmtAmtAfterDiscount = def.TotalSupplierGarmentAmountAfterDiscount;
                row.TotalOPAUpcharge = def.TotalOPAUpcharge;
				row.TotalShippedQty = def.TotalShippedQuantity;
				row.TotalShippedAmt = def.TotalShippedAmount;
                row.TotalShippedAmtAfterDiscount = def.TotalShippedAmountAfterDiscount;
				row.TotalShippedNetFOBAmt = def.TotalShippedNetFOBAmount;
                row.TotalShippedNetFOBAmtAfterDiscount = def.TotalShippedNetFOBAmountAfterDiscount;
				row.TotalShippedSupplierGmtAmt = def.TotalShippedSupplierGarmentAmount;
                row.TotalShippedSupplierGmtAmtAfterDiscount = def.TotalShippedSupplierGarmentAmountAfterDiscount;
				row.TotalShippedOPAUpcharge = def.TotalShippedOPAUpcharge;
				row.VendorPaymentDiscountPercent = def.VendorPaymentDiscountPercent;
				row.IsVirtualSetSplit = def.IsVirtualSetSplit;
				row.IsKnitwearComponent = def.IsKnitwearComponent;
				row.IsFobOrder = def.IsFobOrder;
				row.QACommissionPercent = def.QACommissionPercent;
                if (def.Colour.Trim() != String.Empty)
                    row.Colour = def.Colour;
                else
                    row.SetColourNull();
                row.LCAmt = def.LCAmount;
                if (def.LCNo != String.Empty)
                    row.LCNo = def.LCNo;
                else
                    row.SetLCNoNull();
                if (def.LCIssueDate != DateTime.MinValue)
                    row.LCIssueDate = def.LCIssueDate;
                else
                    row.SetLCIssueDateNull();
                if (def.LCExpiryDate != DateTime.MinValue)
                    row.LCExpiryDate = def.LCExpiryDate;
                else
                    row.SetLCExpiryDateNull();
                row.IsLCPaymentChecked = def.IsLCPaymentChecked;
                if (def.LCPaymentCheckedDate != DateTime.MinValue)
                    row.LCPaymentCheckedDate = def.LCPaymentCheckedDate;
                else
                    row.SetLCPaymentCheckedDateNull();
                row.PaymentLock = def.PaymentLock;

                if (def.AccountDocReceiptDate != DateTime.MinValue)
                    row.AccountDocReceiptDate = def.AccountDocReceiptDate;
                else
                    row.SetAccountDocReceiptDateNull();
                if (def.ShippingDocReceiptDate != DateTime.MinValue)
                    row.ShippingDocReceiptDate = def.ShippingDocReceiptDate;
                else
                    row.SetShippingDocReceiptDateNull();
                if (def.APRefNo != String.Empty)
                    row.APRefNo = def.APRefNo;
                else
                    row.SetAPRefNoNull();
                if (def.APDate != DateTime.MinValue)
                    row.APDate = def.APDate;
                else
                    row.SetAPDateNull();
                row.APAmt = def.APAmt;
                row.APExchangeRate = def.APExchangeRate;
                row.IsILSQtyUploadAllowed = def.IsILSQtyUploadAllowed;
                row.DMSWorkflowStatusId = def.ShippingDocWFS.Id;
                if (def.DocumentReviewedOn == DateTime.MinValue)
                    row.SetDocReviewedOnNull();
                else
                    row.DocReviewedOn = def.DocumentReviewedOn;
                if (def.DocumentReviewedBy == 0)
                    row.SetDocReviewedByNull();
                else
                    row.DocReviewedBy = def.DocumentReviewedBy;
                if (def.RejectPaymentReasonId == 0)
                    row.SetRejectPaymentReasonIdNull();
                else
                    row.RejectPaymentReasonId = def.RejectPaymentReasonId;
                if (def.ShippingDocCheckedBy != null)
                    row.ShippingDocCheckedBy = def.ShippingDocCheckedBy.UserId;
                else
                    row.SetShippingDocCheckedByNull();
                if (def.ShippingDocCheckedOn == DateTime.MinValue)
                    row.SetShippingDocCheckedOnNull();
                else
                    row.ShippingDocCheckedOn = def.ShippingDocCheckedOn;
                row.ShippingCheckedTotalNetAmount = def.ShippingCheckedTotalNetAmount;
                row.LabTestIncome = def.LabTestIncome;
			}
			else if (source.GetType() == typeof(SplitShipmentDef) &&
				target.GetType() == typeof(SplitShipmentRef))
			{
				SplitShipmentDef def = (SplitShipmentDef) source;
				SplitShipmentRef rf = (SplitShipmentRef) target;

                rf.SplitShipmentId = def.SplitShipmentId;
                rf.Product = productWorker.getSplitProductByKey(def.ProductId);
                rf.SplitSuffix = def.SplitSuffix;
				
				if (def.Vendor != null)
				{
                    rf.VendorId = def.Vendor.VendorId;
                    rf.SupplierName = def.Vendor.Name;
				}
				else
				{
                    rf.VendorId = -1;
                    rf.SupplierName = "N/A";
				}
                rf.SupplierAgreedAtWarehouseDate = def.SupplierAgreedAtWarehouseDate;
                rf.TotalOrderQuantity = def.TotalOrderQuantity;
                rf.TotalOrderAmount = def.TotalOrderAmount;
                rf.TotalNetFOBAmount = def.TotalNetFOBAmount;
                rf.TotalOPAUpcharge = def.TotalOPAUpcharge;
                rf.IsVirtualSetSplit = def.IsVirtualSetSplit;
                rf.PiecesPerPack = def.PiecesPerPack;
                rf.IsKnitwearComponent = def.IsKnitwearComponent;
				rf.IsFobOrder = def.IsFobOrder;
			}
		}

		internal void ShipmentDetailMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(ShipmentDetailDs.ShipmentDetailRow) &&
				target.GetType() == typeof(ShipmentDetailDef))
			{
				ShipmentDetailDs.ShipmentDetailRow row = (ShipmentDetailDs.ShipmentDetailRow) source;
				ShipmentDetailDef def = (ShipmentDetailDef) target;

				def.ShipmentDetailId = row.ShipmentDetailId;
				def.ShipmentId = row.ShipmentId;
				def.SizeOption = commonWorker.getSizeOptionByKey(row.SizeOptionId);
				def.OrderQuantity = row.OrderQty;
				def.POQuantity = row.POQty;
				def.ShippedQuantity = row.ShippedQty;
				def.SellingPrice = row.SellingPrice;
				def.NetFOBPrice = row.NetFOBPrice;
				def.SupplierGarmentPrice = row.SupplierGmtPrice;
				def.ReducedSellingPrice = row.ReducedSellingPrice;
				def.ReducedNetFOBPrice = row.ReducedNetFOBPrice;
				def.ReducedSupplierGmtPrice = row.ReducedSupplierGmtPrice;
				def.OPAUpcharge = row.OPAUpcharge;
				def.TotalOtherCost = row.TotalOtherCost;
				def.TotalShippedOtherCost = row.TotalShippedOtherCost;

				if (!row.IsGSPFormTypeIdNull())
					def.GSPFormTypeId = row.GSPFormTypeId;
				else
					def.GSPFormTypeId = GSPFormType.eType.NoGSPRequired.GetHashCode();
				
				if (!row.IsShippingGSPFormTypeIdNull())
					def.ShippingGSPFormTypeId = row.ShippingGSPFormTypeId;
				else
					def.ShippingGSPFormTypeId = GSPFormType.eType.NoGSPRequired.GetHashCode();

                def.RatioPack = row.RatioPack;
				def.RetailSellingPrice = row.RetailSellingPrice;
                def.FreightCost = row.FreightCost;
                def.DutyCost = row.DutyCost;
                def.FobUTSurchargeUSD = row.FobUTSurchargeUSD;
                def.CMPriceUSD = row.CMPriceUSD;
                if (!row.IsColourNull())
                    def.Colour = row.Colour;
                else
                    def.Colour = String.Empty;

                def.OtherCost = (ArrayList) getOtherCostByShipmentDetailId(row.ShipmentDetailId);
			}
			else if (source.GetType() == typeof(ShipmentDetailDef) &&
				target.GetType() == typeof(ShipmentDetailDs.ShipmentDetailRow))
			{
				ShipmentDetailDef def = (ShipmentDetailDef) source;
				ShipmentDetailDs.ShipmentDetailRow row = (ShipmentDetailDs.ShipmentDetailRow) target;

				row.ShipmentDetailId = def.ShipmentDetailId;
				row.ShipmentId = def.ShipmentId;
				row.SizeOptionId = def.SizeOption.SizeOptionId;
				row.OrderQty = def.OrderQuantity;
				row.POQty = def.POQuantity;
				row.ShippedQty = def.ShippedQuantity;
				row.SellingPrice = def.SellingPrice;
				row.NetFOBPrice = def.NetFOBPrice;
				row.SupplierGmtPrice = def.SupplierGarmentPrice;
				row.FreightCost = def.FreightCost;
				row.DutyCost = def.DutyCost;
				row.ReducedSellingPrice = def.ReducedSellingPrice;
				row.ReducedNetFOBPrice = def.ReducedNetFOBPrice;
				row.ReducedSupplierGmtPrice = def.ReducedSupplierGmtPrice;
				row.OPAUpcharge = def.OPAUpcharge;
				row.TotalOtherCost = def.TotalOtherCost;
				row.TotalShippedOtherCost = def.TotalShippedOtherCost;
				row.GSPFormTypeId = def.GSPFormTypeId;
				row.RatioPack = def.RatioPack;
				row.RetailSellingPrice = def.RetailSellingPrice;
				row.FobUTSurchargeUSD = def.FobUTSurchargeUSD;
				row.CMPriceUSD = def.CMPriceUSD;
                if (def.Colour != String.Empty)
                    row.Colour = def.Colour;
                else
                    row.SetColourNull();
                row.ShippingGSPFormTypeId = def.ShippingGSPFormTypeId;
			}
		}

		internal void SplitShipmentDetailMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(SplitShipmentDetailDs.SplitShipmentDetailRow) &&
				target.GetType() == typeof(SplitShipmentDetailDef))
			{
				SplitShipmentDetailDs.SplitShipmentDetailRow row = (SplitShipmentDetailDs.SplitShipmentDetailRow) source;
				SplitShipmentDetailDef def = (SplitShipmentDetailDef) target;

				def.SplitShipmentDetailId = row.SplitShipmentDetailId;
				def.SplitShipmentId = row.SplitShipmentId;
				def.ShipmentDetailId = row.ShipmentDetailId;
				def.SizeOption = commonWorker.getSizeOptionByKey(row.SizeOptionId);
				def.OrderQuantity = row.OrderQty;
				def.POQuantity = row.POQty;
				def.ShippedQuantity = row.ShippedQty;
				def.SellingPrice = row.SellingPrice;
				def.NetFOBPrice = row.NetFOBPrice;
				def.SupplierGarmentPrice = row.SupplierGmtPrice;
                def.OPAUpcharge = row.OPAUpcharge;
                def.ReducedSellingPrice = row.ReducedSellingPrice;
                def.ReducedNetFOBPrice = row.ReducedNetFOBPrice;
                def.ReducedSupplierGmtPrice = row.ReducedSupplierGmtPrice;
			}
			else if (source.GetType() == typeof(SplitShipmentDetailDef) &&
				target.GetType() == typeof(SplitShipmentDetailDs.SplitShipmentDetailRow))
			{
				SplitShipmentDetailDef def = (SplitShipmentDetailDef) source;
				SplitShipmentDetailDs.SplitShipmentDetailRow row = (SplitShipmentDetailDs.SplitShipmentDetailRow) target;

				row.SplitShipmentDetailId = def.SplitShipmentDetailId;
				row.SplitShipmentId = def.SplitShipmentId;
				row.ShipmentDetailId = def.ShipmentDetailId;
				row.SizeOptionId = def.SizeOption.SizeOptionId;
				row.OrderQty = def.OrderQuantity;
				row.POQty = def.POQuantity;
				row.ShippedQty = def.ShippedQuantity;
				row.SellingPrice = def.SellingPrice;
				row.NetFOBPrice = def.NetFOBPrice;
				row.SupplierGmtPrice = def.SupplierGarmentPrice;
				row.OPAUpcharge = def.OPAUpcharge;
				row.ReducedSellingPrice = def.ReducedSellingPrice;
				row.ReducedNetFOBPrice = def.ReducedNetFOBPrice;
				row.ReducedSupplierGmtPrice = def.ReducedSupplierGmtPrice;
			}
		}

		internal void OtherCostMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(OtherCostDs.OtherCostRow) &&
				target.GetType() == typeof(OtherCostDef))
			{
				OtherCostDs.OtherCostRow row = (OtherCostDs.OtherCostRow) source;
				OtherCostDef def = (OtherCostDef) target;

				def.OtherCostId = row.OtherCostId;
				def.ShipmentDetailId = row.ShipmentDetailId;
				def.OtherCostType = commonWorker.getOtherCostTypeByKey(row.OtherCostTypeId);
				if (!row.IsVendorIdNull()) 
                    def.VendorId = row.VendorId;
				else 
                    def.VendorId = -1;
				def.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
				def.POExchangeRate = row.POExchangeRate;
				def.OtherCostAmount = row.OtherCostAmt;
			}
			else if (source.GetType() == typeof(OtherCostDef) &&
				target.GetType() == typeof(OtherCostDs.OtherCostRow))
			{
				OtherCostDef def = (OtherCostDef) source;
				OtherCostDs.OtherCostRow row = (OtherCostDs.OtherCostRow) target;

				row.OtherCostId = def.OtherCostId;
				row.ShipmentTypeId = ShipmentType.NORMAL.Id;
				row.ShipmentDetailId = def.ShipmentDetailId;
				row.OtherCostTypeId = def.OtherCostType.OtherCostTypeId;
				if (def.VendorId > 0) 
                    row.VendorId = def.VendorId;
				else 
                    row.SetVendorIdNull();
				row.CurrencyId = def.Currency.CurrencyId;
				row.POExchangeRate = def.POExchangeRate;
				row.OtherCostAmt = def.OtherCostAmount;
			}
		}

		internal void SplitOtherCostMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(OtherCostDs.OtherCostRow) &&
				target.GetType() == typeof(OtherCostDef))
			{
				OtherCostDs.OtherCostRow row = (OtherCostDs.OtherCostRow) source;
				SplitOtherCostDef def = (SplitOtherCostDef) target;

				def.OtherCostId = row.OtherCostId;
				def.SplitShipmentDetailId = row.ShipmentDetailId;
				def.OtherCostType = commonWorker.getOtherCostTypeByKey(row.OtherCostTypeId);
				def.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
				def.POExchangeRate = row.POExchangeRate;
				def.OtherCostAmount = row.OtherCostAmt;
			}
			else if (source.GetType() == typeof(SplitOtherCostDef) &&
				target.GetType() == typeof(OtherCostDs.OtherCostRow))
			{
				SplitOtherCostDef def = (SplitOtherCostDef) source;
				OtherCostDs.OtherCostRow row = (OtherCostDs.OtherCostRow) target;

				row.OtherCostId = def.OtherCostId;
				row.ShipmentTypeId = ShipmentType.SPLIT.Id;
				row.ShipmentDetailId = def.SplitShipmentDetailId;
				row.OtherCostTypeId = def.OtherCostType.OtherCostTypeId;
				row.CurrencyId = def.Currency.CurrencyId;
				row.POExchangeRate = def.POExchangeRate;
				row.OtherCostAmt = def.OtherCostAmount;
			}
		}

		#endregion


		public ShipmentDef getShipmentByContractNoAndDlyNo(string contractNo, int deliveryNo)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByContractNoAndDlyNo");
			ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
			ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;

			ShipmentDs dataSet = new ShipmentDs();
			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ShipmentDef def = new ShipmentDef();
			ShipmentMapping(dataSet.Shipment[0], def);
			return def;
		}

		public ArrayList getShipmentListByContractNo(string contractNo)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentListByContractNo");
			ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;

			ShipmentDs dataSet = new ShipmentDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ShipmentDs.ShipmentRow row in dataSet.Shipment)
			{
				ShipmentDef def = new ShipmentDef();
				ShipmentMapping(row, def);
				list.Add(def);
			}

			return list;
		}

        public ArrayList GetChinaGBTestShipmentList(int productId, int seasonId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetChinaGBTestShipmentList");
            ad.SelectCommand.Parameters["@ProductId"].Value = productId.ToString();
            ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId.ToString();


            ShipmentDs dataSet = new ShipmentDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ShipmentDs.ShipmentRow row in dataSet.Shipment)
            {
                ShipmentDef def = new ShipmentDef();
                ShipmentMapping(row, def);
                list.Add(def);
            }

            return list;
        }

        public ArrayList GetChinaGBTestNonDirectoryShipmentList(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetChinaGBTestNonDirectoryShipmentList");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            ShipmentDs dataSet = new ShipmentDs();

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ShipmentDs.ShipmentRow row in dataSet.Shipment)
            {
                ShipmentDef def = new ShipmentDef();
                ShipmentMapping(row, def);
                list.Add(def);
            }

            return list;
        }


	}
}
