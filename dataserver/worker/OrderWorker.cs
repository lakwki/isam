using System;
using System.Collections;
using System.Data;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.isam.dataserver.model.order;
using com.next.isam.domain.order;
using com.next.isam.domain.shipping;
using com.next.isam.domain.types;
using com.next.isam.domain.common;
using com.next.infra.util;

namespace com.next.isam.dataserver.worker
{
	public class OrderWorker : Worker
	{
		private static OrderWorker _instance;
		private GeneralWorker generalWorker;
		private CommonWorker commonWorker;
		private VendorWorker vendorWorker;
		private OrderSelectWorker orderSelectWorker;
		
		protected OrderWorker()
		{
			generalWorker = GeneralWorker.Instance;
			commonWorker = CommonWorker.Instance;
			vendorWorker = VendorWorker.Instance;
			orderSelectWorker = OrderSelectWorker.Instance;
		}

		public static OrderWorker Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new OrderWorker();
				}
				return _instance;
			}
		}

        public void updateChinaGBTestIndicator(int shipmentId, bool isChinaGBTestRequired)
        {
			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ShipmentDs dataSet = new ShipmentDs();
                ShipmentDs.ShipmentRow row = null;

				IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByKey");
				ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();
				ad.PopulateCommands();

				int recordsAffected = ad.Fill(dataSet);

				if (recordsAffected > 0) 
				{
					row = dataSet.Shipment[0];

                    if (row.IsChinaGBTestRequired != isChinaGBTestRequired)
                    {
                        ShippingWorker.Instance.updateActionHistory(SynchronizationWorker.Instance.getNewActionHistoryDef(row.ShipmentId, 0, AmendmentType.CHINA_GB_TEST_REQUIREMENT, row.IsChinaGBTestRequired ? "TRUE" : "FALSE", isChinaGBTestRequired ? "TRUE" : "FALSE"));
                        row.IsChinaGBTestRequired = isChinaGBTestRequired;
                        this.sealStamp(row, 99999, Stamp.UPDATE);
                        recordsAffected = ad.Update(dataSet);

                        if (recordsAffected < 1)
                            throw new DataAccessException("Update Shipment ERROR");
                    }
				}
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
        }

		public void updateShipmentList(ICollection shipmentDefs)
		{
			if (shipmentDefs == null || shipmentDefs.Count == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

				foreach (ShipmentDef def in shipmentDefs) 
				{
					ShipmentDs dataSet = new ShipmentDs();
					ShipmentDs.ShipmentRow row = null;
					
					IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByKey");
					ad.SelectCommand.Parameters["@ShipmentId"].Value = def.ShipmentId.ToString();
					ad.PopulateCommands();

					int recordsAffected = ad.Fill(dataSet);

					if (def.ShipmentId > 0) 
					{
						row = dataSet.Shipment[0];
						orderSelectWorker.ShipmentMapping(def, row);
                        recordsAffected = ad.Update(dataSet);
					} 
					if (recordsAffected < 1) 
						throw new DataAccessException("Update Shipment ERROR");
				}
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

		public void updateShipmentList(ICollection shipmentDefs, int userId)
		{
			if (shipmentDefs == null || shipmentDefs.Count == 0 || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

				foreach (ShipmentDef def in shipmentDefs) 
				{
					ShipmentDs dataSet = new ShipmentDs();
					ShipmentDs.ShipmentRow row = null;
					
					IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByKey");
					ad.SelectCommand.Parameters["@ShipmentId"].Value = def.ShipmentId.ToString();

					ad.PopulateCommands();

					int recordsAffected = ad.Fill(dataSet);

					if (def.ShipmentId > 0) 
					{
						row = dataSet.Shipment[0];

						orderSelectWorker.ShipmentMapping(def, row);
						sealStamp(dataSet.Shipment[0], userId, Stamp.UPDATE);
                        recordsAffected = ad.Update(dataSet);
					} 
					if (recordsAffected < 1) 
						throw new DataAccessException("Update Shipment ERROR");
				}
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

		public void updateShipmentDetailList(ICollection shipmentDetailDefs, ActionHistoryType actionType, ArrayList amendmentList, int userId)
		{
			if (shipmentDetailDefs == null || shipmentDetailDefs.Count == 0 || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try 
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                int shipmentId = 0;

				foreach (ShipmentDetailDef def in shipmentDetailDefs) 
				{
					ShipmentDetailDs dataSet = new ShipmentDetailDs();
					ShipmentDetailDs.ShipmentDetailRow row = null;
					
					IDataSetAdapter ad = getDataSetAdapter("ShipmentDetailApt", "GetShipmentDetailByKey");
					ad.SelectCommand.Parameters["@ShipmentDetailId"].Value = def.ShipmentDetailId.ToString();
					ad.PopulateCommands();
					int recordsAffected = ad.Fill(dataSet);
                    shipmentId = def.ShipmentId;

					if (def.ShipmentDetailId > 0) 
					{
						row = dataSet.ShipmentDetail[0];

                        if (row.ShippedQty != def.ShippedQuantity)
                        {
                            if (amendmentList != null)
                                amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, 0, actionType, AmendmentType.ACTUAL_QUANTITY, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.ShippedQty.ToString(), def.ShippedQuantity.ToString(), userId));
                        }

						orderSelectWorker.ShipmentDetailMapping(def, row);
						sealStamp(dataSet.ShipmentDetail[0], userId, Stamp.UPDATE);
                        recordsAffected = ad.Update(dataSet);
					} 
					if (recordsAffected < 1) 
						throw new DataAccessException("Update Shipment Detail ERROR");
				}

				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

        public void addSplitShipmentDMSActionLog(ICollection splitShipmentDefs, int userId)
        {
            if (splitShipmentDefs == null || splitShipmentDefs.Count == 0 || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                string remark;
                UserRef dummyUser = new UserRef();
                dummyUser.UserId = int.MinValue;
                dummyUser.DisplayName = string.Empty;

                foreach (SplitShipmentDef def in splitShipmentDefs)
                {
                    SplitShipmentDs dataSet = new SplitShipmentDs();
                    SplitShipmentDs.SplitShipmentRow row = null;

                    IDataSetAdapter ad = getDataSetAdapter("SplitShipmentApt", "GetSplitShipmentByKey");
                    ad.SelectCommand.Parameters["@SplitShipmentId"].Value = def.SplitShipmentId.ToString();
                    ad.PopulateCommands();

                    int recordsAffected = ad.Fill(dataSet);

                    if (recordsAffected > 0)
                    {
                        if (def.SplitShipmentId > 0)
                        {
                            row = dataSet.SplitShipment[0];

                            DateTime prevReciptDate = (row.IsShippingDocReceiptDateNull() ? DateTime.MinValue : row.ShippingDocReceiptDate);
                            DateTime newReciptDate = def.ShippingDocReceiptDate;
                            if (prevReciptDate != newReciptDate)
                            {
                                remark = "Shipping Doc. Receipt Date : " + (DateTimeUtility.getDateString(prevReciptDate))
                                    + " -> " + (newReciptDate == null ? "" : (DateTimeUtility.getDateString(newReciptDate)));
                                ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(def.ShipmentId, def.SplitShipmentId, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                            }

                            ShippingDocWFS prevDocWFS = ShippingDocWFS.getType(row.DMSWorkflowStatusId);
                            ShippingDocWFS newDocWFS = def.ShippingDocWFS;
                            if (newDocWFS.Id != prevDocWFS.Id)
                            {
                                remark = "Shipping Doc. Status : " + (prevDocWFS == null ? "" : prevDocWFS.Name)
                                               + " -> " + (newDocWFS == null ? "" : newDocWFS.Name);
                                ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(def.ShipmentId, def.SplitShipmentId, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                            }

                            DateTime prevCheckDate = (row.IsShippingDocCheckedOnNull() ? DateTime.MinValue : row.ShippingDocCheckedOn);
                            DateTime newCheckDate = def.ShippingDocCheckedOn;
                            bool isShipDocChecked = (prevCheckDate == DateTime.MinValue && newCheckDate != DateTime.MinValue);
                            bool isShipDocUnChecked = (prevCheckDate != DateTime.MinValue && newCheckDate == DateTime.MinValue);
                            if (isShipDocChecked || isShipDocUnChecked || newDocWFS.Id != prevDocWFS.Id)
                            {
                                //UserRef prevCheckBy = (row.IsShippingDocCheckedByNull() ? dummyUser : generalWorker.getUserByKey(row.ShippingDocCheckedBy));
                                //UserRef newCheckBy = (def.ShippingDocCheckedBy == null ? dummyUser : def.ShippingDocCheckedBy);
                                //if (prevCheckBy.DisplayName != newCheckBy.DisplayName)
                                {
                                    remark = "Shipping Doc. Checked By : " + (row.IsShippingDocCheckedByNull() ? "" : generalWorker.getUserByKey(row.ShippingDocCheckedBy).DisplayName) 
                                            + " -> " + (def.ShippingDocCheckedBy == null ? "" : def.ShippingDocCheckedBy.DisplayName);
                                    ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(def.ShipmentId, def.SplitShipmentId, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                                }
                                //if (prevCheckDate != newCheckDate)
                                {
                                    remark = "Shipping Doc. Checked Date : " + (prevCheckDate == DateTime.MinValue ? "" : prevCheckDate.ToString("dd/MM/yyyy HH:mm:ss"))
                                            + " -> " + (newCheckDate == null ? "" : (newCheckDate == DateTime.MinValue ? "" : newCheckDate.ToString("dd/MM/yyyy HH:mm:ss")));
                                    ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(def.ShipmentId, def.SplitShipmentId, ActionHistoryType.SHIPPING_UPDATES, null, remark, userId));
                                }
                            }
                        }
                    }
                    else
                        throw new DataAccessException("Adding Log for Split Shipment ERROR");
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }			
        }

        public void updateSplitShipmentList(ICollection splitShipmentDefs, int userId)
		{
			if (splitShipmentDefs == null || splitShipmentDefs.Count == 0 || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            ArrayList amendmentList = new ArrayList();

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

				foreach (SplitShipmentDef def in splitShipmentDefs) 
				{
					SplitShipmentDs dataSet = new SplitShipmentDs();
					SplitShipmentDs.SplitShipmentRow row = null;
					
					IDataSetAdapter ad = getDataSetAdapter("SplitShipmentApt", "GetSplitShipmentByKey");
					ad.SelectCommand.Parameters["@SplitShipmentId"].Value = def.SplitShipmentId.ToString();
					ad.PopulateCommands();

					int recordsAffected = ad.Fill(dataSet);

					if (def.SplitShipmentId > 0) 
					{
						row = dataSet.SplitShipment[0];

                        if (!row.IsSupplierInvoiceNoNull())
                        {
                            if (row.SupplierInvoiceNo != def.SupplierInvoiceNo)
                                amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, def.SplitShipmentId, ActionHistoryType.SHIPPING_UPDATES, AmendmentType.SUPPLIER_INVOICE_NO, (row.IsSupplierInvoiceNoNull() ? string.Empty : row.SupplierInvoiceNo), def.SupplierInvoiceNo, userId));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(def.SupplierInvoiceNo))
                                amendmentList.Add(getNewActionHistoryDef(def.ShipmentId, def.SplitShipmentId, ActionHistoryType.SHIPPING_UPDATES, AmendmentType.SUPPLIER_INVOICE_NO, string.Empty, def.SupplierInvoiceNo, userId));
                        }

                        orderSelectWorker.SplitShipmentMapping(def, row);
						sealStamp(dataSet.SplitShipment[0], userId, Stamp.UPDATE);
                        recordsAffected = ad.Update(dataSet);

                        foreach (ActionHistoryDef historyDef in amendmentList)
                        {
                            ShippingWorker.Instance.updateActionHistory(historyDef);
                        }

                        if (amendmentList.Count > 0)
                        {
                            AccountWorker.Instance.doReversal(def.ShipmentId, amendmentList);
                        }
					} 
					if (recordsAffected < 1) 
						throw new DataAccessException("Update Split Shipment ERROR");
				}
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

		public void updateSplitShipmentDetailList(int shipmentId, ICollection splitShipmentDetailDefs, ActionHistoryType actionType, ArrayList amendmentList, int userId)
		{
			if (splitShipmentDetailDefs == null || splitShipmentDetailDefs.Count == 0 || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                int splitShipmentId = 0;

				foreach (SplitShipmentDetailDef def in splitShipmentDetailDefs) 
				{
					SplitShipmentDetailDs dataSet = new SplitShipmentDetailDs();
					SplitShipmentDetailDs.SplitShipmentDetailRow row = null;
					
					IDataSetAdapter ad = getDataSetAdapter("SplitShipmentDetailApt", "GetSplitShipmentDetailByKey");
					ad.SelectCommand.Parameters["@SplitShipmentDetailId"].Value = def.SplitShipmentDetailId.ToString();

					ad.PopulateCommands();

					int recordsAffected = ad.Fill(dataSet);

					if (def.SplitShipmentDetailId > 0) 
					{
						row = dataSet.SplitShipmentDetail[0];
                        splitShipmentId = def.SplitShipmentId;
                        if (row.ShippedQty != def.ShippedQuantity)
                        {
                            if (amendmentList != null)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, def.SplitShipmentId, actionType, AmendmentType.ACTUAL_QUANTITY, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.ShippedQty.ToString(), def.ShippedQuantity.ToString(), userId));
                        }
						orderSelectWorker.SplitShipmentDetailMapping(def, row);
						sealStamp(dataSet.SplitShipmentDetail[0], userId, Stamp.UPDATE);
                        recordsAffected = ad.Update(dataSet);
					} 
					if (recordsAffected < 1) 
						throw new DataAccessException("Update Split Shipment Detail ERROR");
				}

				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

		public void updateOtherCostList(ICollection otherCostDefs, int userId)
		{
			if (otherCostDefs == null || otherCostDefs.Count == 0 || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

				foreach (OtherCostDef def in otherCostDefs) 
				{
					OtherCostDs dataSet = new OtherCostDs();
					OtherCostDs.OtherCostRow row = null;
					
					IDataSetAdapter ad = getDataSetAdapter("OtherCostApt", "GetOtherCostByKey");
					ad.SelectCommand.Parameters["@OtherCostId"].Value = def.OtherCostId.ToString();
					ad.PopulateCommands();

					int recordsAffected = ad.Fill(dataSet);

					if (def.OtherCostId > 0) 
					{
						row = dataSet.OtherCost[0];
						orderSelectWorker.OtherCostMapping(def, row);
						sealStamp(dataSet.OtherCost[0], userId, Stamp.UPDATE);
                        recordsAffected = ad.Update(dataSet);
					} 
					if (recordsAffected < 1) 
						throw new DataAccessException("Update Other Cost ERROR");
				}
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

		public void updateSplitOtherCostList(ICollection splitOtherCostDefs, int userId)
		{
			if (splitOtherCostDefs == null || splitOtherCostDefs.Count == 0 || userId == 0)
				return;

			TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

			try
			{
				ctx.Enter();
				TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

				foreach (SplitOtherCostDef def in splitOtherCostDefs) 
				{
					OtherCostDs dataSet = new OtherCostDs();
					OtherCostDs.OtherCostRow row = null;
					
					IDataSetAdapter ad = getDataSetAdapter("OtherCostApt", "GetSplitOtherCostByKey");
					ad.SelectCommand.Parameters["@SplitOtherCostId"].Value = def.OtherCostId.ToString();
					ad.PopulateCommands();

					int recordsAffected = ad.Fill(dataSet);

					if (def.OtherCostId > 0) 
					{
						row = dataSet.OtherCost[0];
						orderSelectWorker.SplitOtherCostMapping(def, row);
						sealStamp(dataSet.OtherCost[0], userId, Stamp.UPDATE);
                        recordsAffected = ad.Update(dataSet);
					} 
					if (recordsAffected < 1) 
						throw new DataAccessException("Update Split Other Cost ERROR");
				}
				ctx.VoteCommit();
			}
			catch(Exception e) 
			{
				Console.WriteLine("ERROR " + e.Message);
				ctx.VoteRollback();
				throw e;
			}
			finally 
			{
				ctx.Exit();
			}			
		}

        private ActionHistoryDef getNewActionHistoryDef(int shipmentId, int splitShipmentId, ActionHistoryType actionType, AmendmentType amendType, string sourceValue, string targetValue, int userId)
        {
            string s = amendType.Description + " : " + sourceValue + " -> " + targetValue;
            return new ActionHistoryDef(shipmentId, splitShipmentId, actionType, amendType, s, userId);
        }

        public int updateInvoiceLock(string contractNo, int deliveryNo, string level, string action)
		{
			try
			{
				IDataSetAdapter ad = getDataSetAdapter("InvoiceLockApt", "SetInvoicePOLock");
				ad.SelectCommand.DbCommand.CommandTimeout = 180;
				ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
				ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;
				ad.SelectCommand.Parameters["@Level"].Value = level;
				ad.SelectCommand.Parameters["@Action"].Value = action;

				DataSet dataSet = new DataSet();
				int recordsAffected = ad.Fill(dataSet);

				int a = (int)(dataSet.Tables[0].Rows[0][0]);
				if (recordsAffected < 1) return 0;
				
				if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
					return 0;
				else
					return (int)(dataSet.Tables[0].Rows[0][0]);		
			}
			catch(Exception e) 
			{
                MailHelper.sendErrorAlert(e, String.Empty);
				return 0;
			}
		}

        public void updateShipmentQuantityAmount(int shipmentId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ShipmentDef shipment = orderSelectWorker.getShipmentByKey(shipmentId);
                ArrayList shipmentDetails = (ArrayList) orderSelectWorker.getShipmentDetailByShipmentId(shipmentId);

                shipment.TotalShippedQuantity = 0;
                shipment.TotalShippedAmount = 0;
                shipment.TotalShippedNetFOBAmount = 0;
                shipment.TotalShippedSupplierGarmentAmount = 0;
                shipment.TotalShippedOPAUpcharge = 0;
                shipment.TotalShippedOtherCost = 0;
                shipment.TotalShippedFreightCost = 0;
                shipment.TotalShippedDutyCost = 0;

                foreach (ShipmentDetailDef def in shipmentDetails)
                {
                    shipment.TotalShippedQuantity += def.ShippedQuantity;
                    shipment.TotalShippedAmount += def.ShippedQuantity * def.SellingPrice;
                    shipment.TotalShippedNetFOBAmount += def.ShippedQuantity * def.NetFOBPrice;
                    shipment.TotalShippedSupplierGarmentAmount += def.ShippedQuantity * def.SupplierGarmentPrice;
                    shipment.TotalShippedOPAUpcharge += def.ShippedQuantity * def.OPAUpcharge;
                    shipment.TotalShippedOtherCost += def.ShippedQuantity * def.TotalShippedOtherCost;
                    shipment.TotalShippedFreightCost += def.ShippedQuantity * def.FreightCost;
                    shipment.TotalShippedDutyCost += def.ShippedQuantity * def.DutyCost;
                }

                updateShipmentList(ConvertUtility.createArrayList(shipment), userId);

                //update split shipment detail shipped quantity...
                if (shipment.SplitCount > 0)
                {
                    ArrayList splitShipmentList = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(shipmentId);
                    ArrayList splitShipmentDetailList;
                    ArrayList updateSplitShipmentDetailList = new ArrayList();
                    foreach (SplitShipmentDef splitShipment in splitShipmentList)
                    {
                        splitShipmentDetailList = (ArrayList) orderSelectWorker.getSplitShipmentDetailBySplitShipmentId(splitShipment.SplitShipmentId);

                        foreach (ShipmentDetailDef shipDetail in shipmentDetails)
                        {
                            foreach (SplitShipmentDetailDef splitShipDetail in splitShipmentDetailList)
                            {
                                if (shipDetail.SizeOption.SizeOptionId == splitShipDetail.SizeOption.SizeOptionId)
                                {
                                    if (splitShipDetail.ShippedQuantity != shipDetail.ShippedQuantity)
                                    {
                                        splitShipDetail.ShippedQuantity = shipDetail.ShippedQuantity;
                                        updateSplitShipmentDetailList.Add(splitShipDetail);
                                    }
                                    break;
                                }
                            }
                        }
                        this.updateSplitShipmentDetailList(shipmentId, updateSplitShipmentDetailList, ActionHistoryType.SHIPPING_UPDATES, new ArrayList(), userId);
                    }
                }

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }			
        }

        public void updateSplitShipmentQuantityAmount(int splitShipmentId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                SplitShipmentDef splitShipment = orderSelectWorker.getSplitShipmentByKey(splitShipmentId);
                ArrayList splitShipmentDetails = (ArrayList)orderSelectWorker.getSplitShipmentDetailBySplitShipmentId(splitShipmentId);

                splitShipment.TotalShippedQuantity = 0;
                splitShipment.TotalShippedAmount = 0;
                splitShipment.TotalShippedNetFOBAmount = 0;
                splitShipment.TotalShippedSupplierGarmentAmount = 0;
                splitShipment.TotalShippedOPAUpcharge = 0;

                foreach (SplitShipmentDetailDef def in splitShipmentDetails)
                {
                    splitShipment.TotalShippedQuantity += def.ShippedQuantity ;
                    splitShipment.TotalShippedAmount += def.ShippedQuantity * def.SellingPrice;
                    splitShipment.TotalShippedNetFOBAmount += def.ShippedQuantity * def.NetFOBPrice;
                    splitShipment.TotalShippedSupplierGarmentAmount += def.ShippedQuantity * def.SupplierGarmentPrice;
                    splitShipment.TotalShippedOPAUpcharge += def.ShippedQuantity * def.OPAUpcharge;
                }

                updateSplitShipmentList(ConvertUtility.createArrayList(splitShipment), userId);

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }


        public void updateContractList(ICollection contractDefs)
        {
            if (contractDefs == null || contractDefs.Count == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                foreach (ContractDef def in contractDefs)
                {
                    ContractDs dataSet = new ContractDs();
                    ContractDs.ContractRow row = null;

                    IDataSetAdapter ad = getDataSetAdapter("ContractApt", "GetContractByKey");
                    ad.SelectCommand.Parameters["@ContractId"].Value = def.ContractId.ToString();
                    ad.PopulateCommands();

                    int recordsAffected = ad.Fill(dataSet);

                    if (def.ContractId > 0)
                    {
                        row = (ContractDs.ContractRow)dataSet.Contract.Rows[0];
                        orderSelectWorker.ContractMapping(def, row);
                        recordsAffected = ad.Update(dataSet);
                    }
                    if (recordsAffected < 1)
                        throw new DataAccessException("Update Contract ERROR");
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

	}
}
