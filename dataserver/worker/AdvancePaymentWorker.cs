using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain;
using com.next.isam.domain.account;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.dataserver.model.account;
using com.next.infra.util;
using com.next.isam.domain.common;
using com.next.isam.dataserver.model.common;

namespace com.next.isam.dataserver.worker
{
    public class AdvancePaymentWorker : Worker
    {
		private static AdvancePaymentWorker _instance;
        private GeneralWorker generalWorker;
        private VendorWorker vendorWorker;

        public AdvancePaymentWorker()
		{
            generalWorker = GeneralWorker.Instance;
            vendorWorker = VendorWorker.Instance;
		}

        public static AdvancePaymentWorker Instance
		{
			get 
			{
				if (_instance == null)
				{
                    _instance = new AdvancePaymentWorker();
				}
				return _instance;
			}
		}

        public AdvancePaymentDef getAdvancePaymentByKey(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentApt", "GetAdvancePaymentByKey");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;

            AdvancePaymentDs dataSet = new AdvancePaymentDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            AdvancePaymentDef def = new AdvancePaymentDef();

            this.AdvancePaymentMapping(dataSet.AdvancePayment[0], def);
            return def;
        }

        public AdvancePaymentOrderDetailDef getAdvancePaymentOrderDetailByKey(int paymentId, int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentOrderDetailApt", "GetAdvancePaymentOrderDetailByKey");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            AdvancePaymentOrderDetailDs dataSet = new AdvancePaymentOrderDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            AdvancePaymentOrderDetailDef def = new AdvancePaymentOrderDetailDef();

            this.AdvancePaymentOrderDetailMapping(dataSet.AdvancePaymentOrderDetail[0], def);
            return def;
        }

        public List<AdvancePaymentDef> getAdvancePaymentByCriteria(int vendorId, int officeId, string paymentNo, string contractNo, string LCBillRefNo, DateTime fromDate, DateTime toDate, int paymentStatusId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentApt", "GetAdvancePaymentByCriteria");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@PaymentNo"].Value = paymentNo;
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@LCBillRefNo"].Value = LCBillRefNo;
            ad.SelectCommand.Parameters["@FromDate"].Value = fromDate;
            ad.SelectCommand.Parameters["@ToDate"].Value = toDate;
            if (fromDate == DateTime.MinValue && toDate == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@FromDate"].Value = DBNull.Value;
                ad.SelectCommand.Parameters["@ToDate"].Value = DBNull.Value;
            }
            else if (fromDate != DateTime.MinValue && toDate == DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@FromDate"].Value = fromDate;
                ad.SelectCommand.Parameters["@ToDate"].Value = fromDate;
            }
            else if (fromDate == DateTime.MinValue && toDate != DateTime.MinValue)
            {
                ad.SelectCommand.Parameters["@FromDate"].Value = toDate;
                ad.SelectCommand.Parameters["@ToDate"].Value = toDate;
            }
            ad.SelectCommand.Parameters["@PaymentStatusId"].Value = paymentStatusId;
            AdvancePaymentDs dataSet = new AdvancePaymentDs();
            int recordsAffected = ad.Fill(dataSet);

            List<AdvancePaymentDef> list = new List<AdvancePaymentDef>();
            foreach (AdvancePaymentDs.AdvancePaymentRow row in dataSet.AdvancePayment)
            {
                AdvancePaymentDef def = new AdvancePaymentDef();
                AdvancePaymentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<GenericDataSummaryDef> getOutstandingAdvancePaymentByCurrency(int vendorId, int officeId)
        {
            IDataSetAdapter dataSetAdapter = getDataSetAdapter("GenericDataSummaryApt", "GetOutstandingAdvancePaymentByCurrency");
            dataSetAdapter.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            dataSetAdapter.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            GenericDataSummaryDs ds = new GenericDataSummaryDs();
            int num = dataSetAdapter.Fill(ds);
            List<GenericDataSummaryDef> list = new List<GenericDataSummaryDef>();
            foreach (GenericDataSummaryDs.GenericDataSummaryRow row in ds.GenericDataSummary)
            {
                GenericDataSummaryDef def = new GenericDataSummaryDef();
                CommonWorker.Instance.GenericDataSummaryMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<AdvancePaymentOrderDetailDef> getAdvancePaymentOrderDetailList(int paymentId)
        {
            return getAdvancePaymentOrderDetailList(paymentId, GeneralCriteria.ALL);
        }

        public List<AdvancePaymentOrderDetailDef> getAdvancePaymentOrderDetailByShipmentId(int shipmentId)
        {
            return getAdvancePaymentOrderDetailList(GeneralCriteria.ALL, shipmentId);
        }

        public List<AdvancePaymentOrderDetailDef> getAdvancePaymentOrderDetailList(int paymentId, int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentOrderDetailApt", "GetAdvancePaymentOrderDetailList");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            AdvancePaymentOrderDetailDs dataSet = new AdvancePaymentOrderDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            List<AdvancePaymentOrderDetailDef> list = new List<AdvancePaymentOrderDetailDef>();

            foreach (AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow row in dataSet.AdvancePaymentOrderDetail)
            {
                AdvancePaymentOrderDetailDef def = new AdvancePaymentOrderDetailDef();
                AdvancePaymentOrderDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public AdvancePaymentInstalmentDetailDef getAdvancePaymentInstalmentDetailByKey(int paymentId, DateTime paymentDate)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentInstalmentDetailApt", "GetAdvancePaymentInstalmentDetailByKey");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;
            ad.SelectCommand.Parameters["@PaymentDate"].Value = paymentDate;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            AdvancePaymentInstalmentDetailDs dataSet = new AdvancePaymentInstalmentDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            AdvancePaymentInstalmentDetailDef def = new AdvancePaymentInstalmentDetailDef();

            this.AdvancePaymentInstalmentDetailMapping(dataSet.AdvancePaymentInstalmentDetail[0], def);
            return def;

        }

        public AdvancePaymentBalanceSettlementDef getAdvancePaymentBalanceSettlementByKey(int paymentId, DateTime paymentDate)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentBalanceSettlementApt", "GetAdvancePaymentBalanceSettlementByKey");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;
            ad.SelectCommand.Parameters["@PaymentDate"].Value = paymentDate;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            AdvancePaymentBalanceSettlementDs dataSet = new AdvancePaymentBalanceSettlementDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            AdvancePaymentBalanceSettlementDef def = new AdvancePaymentBalanceSettlementDef();

            this.AdvancePaymentBalanceSettlementMapping(dataSet.AdvancePaymentBalanceSettlement[0], def);
            return def;

        }

        public List<AdvancePaymentInstalmentDetailDef> getAdvancePaymentInstalmentDetailList(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentInstalmentDetailApt", "GetAdvancePaymentInstalmentDetailList");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            AdvancePaymentInstalmentDetailDs dataSet = new AdvancePaymentInstalmentDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            List<AdvancePaymentInstalmentDetailDef> list = new List<AdvancePaymentInstalmentDetailDef>();

            foreach (AdvancePaymentInstalmentDetailDs.AdvancePaymentInstalmentDetailRow row in dataSet.AdvancePaymentInstalmentDetail)
            {
                AdvancePaymentInstalmentDetailDef def = new AdvancePaymentInstalmentDetailDef();
                AdvancePaymentInstalmentDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<AdvancePaymentActionHistoryDef> getAdvancePaymentActionHistoryList(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentActionHistoryApt", "GetAdvancePaymentActionHistoryList");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            AdvancePaymentActionHistoryDs dataSet = new AdvancePaymentActionHistoryDs();
            int recordsAffected = ad.Fill(dataSet);

            List<AdvancePaymentActionHistoryDef> list = new List<AdvancePaymentActionHistoryDef>();

            foreach (AdvancePaymentActionHistoryDs.AdvancePaymentActionHistoryRow row in dataSet.AdvancePaymentActionHistory)
            {
                AdvancePaymentActionHistoryDef def = new AdvancePaymentActionHistoryDef();
                def.ActionBy = row.ActionBy;
                def.ActionHistoryId = row.ActionHistoryId;
                def.ActionOn = row.ActionOn;
                def.Description = row.Description;
                def.PaymentId = row.PaymentId;
                def.Status = row.Status;
                list.Add(def);
            }
            return list;
        }

        public List<AdvancePaymentBalanceSettlementDef> getAdvancePaymentBalanceSettlementList(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentBalanceSettlementApt", "GetAdvancePaymentBalanceSettlementList");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            AdvancePaymentBalanceSettlementDs dataSet = new AdvancePaymentBalanceSettlementDs();
            int recordsAffected = ad.Fill(dataSet);

            List<AdvancePaymentBalanceSettlementDef> list = new List<AdvancePaymentBalanceSettlementDef>();

            foreach (AdvancePaymentBalanceSettlementDs.AdvancePaymentBalanceSettlementRow row in dataSet.AdvancePaymentBalanceSettlement)
            {
                AdvancePaymentBalanceSettlementDef def = new AdvancePaymentBalanceSettlementDef();
                AdvancePaymentBalanceSettlementMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        private int getMaxAdvancePaymentActionHistoryId()
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentActionHistoryApt", "GetMaxActionHistoryId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public AdvancePaymentSummaryDef getAdvancePaymentSummaryDef(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentApt", "GetAdvancePaymentSummary");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            AdvancePaymentSummaryDef def = new AdvancePaymentSummaryDef();
            def.Balance = 0;
            def.Variance = 0;
            def.NoRecoveryPlanBalance = 0;

            if (!Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
            {
                def.Balance = (decimal)(dataSet.Tables[0].Rows[0][1]);
                def.Variance = (decimal)(dataSet.Tables[0].Rows[0][2]);
                def.NoRecoveryPlanBalance = def.Balance - ((decimal)(dataSet.Tables[0].Rows[0][3]));
            }
            return def;
        }

        private int getMaxAdvancePaymentId(int paymentTypeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentApt", "GetMaxAdvancePaymentId");
            ad.SelectCommand.Parameters["@PaymentTypeId"].Value = paymentTypeId;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            int i = 0;
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                i = 0;
            else
                i = (int)(dataSet.Tables[0].Rows[0][0]);
            if (paymentTypeId == 2 && i == 0)
                i += 100000;
            return i;
        }

        private string getNextAdvancePaymentNo(int year)
        {
            IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentApt", "GetMaxAdvancePaymentNo");
            string prefix = "ADVI" + year.ToString().Substring(2, 2);
            ad.SelectCommand.Parameters["@PaymentNoPrefix"].Value = prefix;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            int i = 0;
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                i = 0;
            else
                i = int.Parse(((string)(dataSet.Tables[0].Rows[0][0])).Replace(prefix, string.Empty));
            i += 1;
            return prefix + (i.ToString().PadLeft(3, '0'));
        }

        public void updateAdvancePayment(AdvancePaymentDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentApt", "GetAdvancePaymentByKey");
                ad.SelectCommand.Parameters["@PaymentId"].Value = def.PaymentId;
                ad.PopulateCommands();

                AdvancePaymentDs dataSet = new AdvancePaymentDs();
                AdvancePaymentDs.AdvancePaymentRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.AdvancePayment[0];
                    this.AdvancePaymentMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.AdvancePayment.NewAdvancePaymentRow();
                    def.PaymentId = this.getMaxAdvancePaymentId(def.PaymentTypeId) + 1;
                    if (def.PaymentTypeId == 2) // By Instalment
                        def.PaymentNo = this.getNextAdvancePaymentNo(DateTime.Today.Year);
                    this.AdvancePaymentMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.AdvancePayment.AddAdvancePaymentRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Advance Payment ERROR");
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

        public void addAdvancePaymentOrderDetail(AdvancePaymentOrderDetailDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentOrderDetailApt", "GetAdvancePaymentOrderDetailByKey");
                ad.SelectCommand.Parameters["@PaymentId"].Value = def.PaymentId;
                ad.SelectCommand.Parameters["@ShipmentId"].Value = def.ShipmentId;
                ad.PopulateCommands();

                AdvancePaymentOrderDetailDs dataSet = new AdvancePaymentOrderDetailDs();
                AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected <= 0)
                {
                    row = dataSet.AdvancePaymentOrderDetail.NewAdvancePaymentOrderDetailRow();
                    this.AdvancePaymentOrderDetailMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.AdvancePaymentOrderDetail.AddAdvancePaymentOrderDetailRow(row);
                    recordsAffected = ad.Update(dataSet);
                    if (recordsAffected < 1)
                        throw new DataAccessException("Add Advance Payment Order Detail ERROR");
                }
                else
                {
                    row = dataSet.AdvancePaymentOrderDetail[0];
                    if (row.Status == 0)
                    {
                        def.Status = 1;
                    }
                    this.AdvancePaymentOrderDetailMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                    recordsAffected = ad.Update(dataSet);
                    if (recordsAffected < 1)
                        throw new DataAccessException("Re-Add Advance Payment Order Detail ERROR");
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                System.Diagnostics.Debug.WriteLine(e.Message);
                MailHelper.sendErrorAlert(e, "");
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public void updateAdvancePaymentInstalmentDetail(AdvancePaymentInstalmentDetailDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentInstalmentDetailApt", "GetAdvancePaymentInstalmentDetailByKey");
                ad.SelectCommand.Parameters["@PaymentId"].Value = def.PaymentId;
                ad.SelectCommand.Parameters["@PaymentDate"].Value = def.PaymentDate;
                ad.PopulateCommands();

                AdvancePaymentInstalmentDetailDs dataSet = new AdvancePaymentInstalmentDetailDs();
                AdvancePaymentInstalmentDetailDs.AdvancePaymentInstalmentDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.AdvancePaymentInstalmentDetail[0];
                    this.AdvancePaymentInstalmentDetailMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.AdvancePaymentInstalmentDetail.NewAdvancePaymentInstalmentDetailRow();
                    this.AdvancePaymentInstalmentDetailMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.AdvancePaymentInstalmentDetail.AddAdvancePaymentInstalmentDetailRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Advance Payment Instalment Detail ERROR");
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

        public void updateAdvancePaymentBalanceSettlement(AdvancePaymentBalanceSettlementDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentBalanceSettlementApt", "GetAdvancePaymentBalanceSettlementByKey");
                ad.SelectCommand.Parameters["@PaymentId"].Value = def.PaymentId;
                ad.SelectCommand.Parameters["@PaymentDate"].Value = def.PaymentDate;
                ad.PopulateCommands();

                AdvancePaymentBalanceSettlementDs dataSet = new AdvancePaymentBalanceSettlementDs();
                AdvancePaymentBalanceSettlementDs.AdvancePaymentBalanceSettlementRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.AdvancePaymentBalanceSettlement[0];
                    this.AdvancePaymentBalanceSettlementMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.AdvancePaymentBalanceSettlement.NewAdvancePaymentBalanceSettlementRow();
                    this.AdvancePaymentBalanceSettlementMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.AdvancePaymentBalanceSettlement.AddAdvancePaymentBalanceSettlementRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Advance Balance Settlement Detail ERROR");
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

        public void updateAdvancePaymentFabricCost(AdvancePaymentDef def, List<AdvancePaymentOrderDetailDef> list, int userId)
        {
            updateAdvancePayment(def, userId);
            updateAdvancePaymentOrderDetailList(def.PaymentId, list, userId);
        }

        public void updateAdvancePaymentOrderDetailList(int paymentId, List<AdvancePaymentOrderDetailDef> list, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                ShippingWorker shippingWorker = ShippingWorker.Instance;
                SynchronizationWorker synchronizationWorker = SynchronizationWorker.Instance;
                if (list != null && list.Count > 0)
                {
                    AdvancePaymentDef paymentDef = this.getAdvancePaymentByKey(paymentId);
                    string deductionDocumentNo = (paymentDef != null) ? paymentDef.PaymentNo : String.Empty;
                    ArrayList amendmentList = new ArrayList();
                    foreach (AdvancePaymentOrderDetailDef updateDef in list)
                    {
                        updateDef.PaymentId = paymentId;
                        this.addAdvancePaymentOrderDetail(updateDef, userId);

                        // Shipment Deductions
                        ArrayList existShipmentDeduction = shippingWorker.getAllShipmentDeductionByLogicalKey(updateDef.ShipmentId, PaymentDeductionType.FABRIC_ADVANCE.Id, paymentDef.PaymentNo);
                        if (existShipmentDeduction.Count == 0)
                        {
                            ShipmentDeductionDef newDeduction = new ShipmentDeductionDef();
                            newDeduction.ShipmentDeductionId = -1;
                            newDeduction.ShipmentId = updateDef.ShipmentId;
                            newDeduction.DeductionType = PaymentDeductionType.FABRIC_ADVANCE;
                            newDeduction.DocumentNo = deductionDocumentNo;
                            newDeduction.Amount = updateDef.ExpectedValue;
                            newDeduction.Status = 1;
                            amendmentList.Add(this.getNewAdvancePaymentActionHistory(paymentId, "NEW SHIPMENT DEDUCTION: [SHIPMENT ID:" + updateDef.ShipmentId + "] [AMT:" + updateDef.ExpectedValue + "]", userId));
                            shippingWorker.updateShipmentDeduction(newDeduction, userId);
                        }
                        else
                        {
                            foreach (ShipmentDeductionDef SDDef in existShipmentDeduction)
                            {
                                if (SDDef.Status == 0) // To re-Activate record(s)
                                {
                                    SDDef.Status = 1;
                                    SDDef.Amount = updateDef.ExpectedValue;
                                    shippingWorker.updateShipmentDeduction(SDDef, userId);
                                }
                            }
                        }

                    }

                    // AdvancePaymentActionHistory for Shipment Deductions
                    foreach (AdvancePaymentActionHistoryDef historyDef in amendmentList)
                    {
                        this.updateAdvancePaymentActionHistory(historyDef);
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

        public void updateAdvancePaymentActionHistory(AdvancePaymentActionHistoryDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentActionHistoryApt", "GetAdvancePaymentActionHistoryByKey");
                ad.SelectCommand.Parameters["@ActionHistoryId"].Value = def.ActionHistoryId;
                ad.PopulateCommands();

                AdvancePaymentActionHistoryDs dataSet = new AdvancePaymentActionHistoryDs();
                AdvancePaymentActionHistoryDs.AdvancePaymentActionHistoryRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.AdvancePaymentActionHistory[0];
                    this.AdvancePaymentActionHistoryMapping(def, row);
                    recordsAffected = ad.Update(dataSet);
                }
                else
                {
                    row = dataSet.AdvancePaymentActionHistory.NewAdvancePaymentActionHistoryRow();
                    def.ActionHistoryId = this.getMaxAdvancePaymentActionHistoryId() + 1;
                    this.AdvancePaymentActionHistoryMapping(def, row);
                    dataSet.AdvancePaymentActionHistory.AddAdvancePaymentActionHistoryRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Advance Payment Action History ERROR");
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

        public void deleteAdvancePaymentOrderDetail(int paymentId, int shipmentId, int userId)
        {
            if (paymentId <= 0 || shipmentId <= 0 || userId == 0)
                return;
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                AdvancePaymentOrderDetailDs dataSet = new AdvancePaymentOrderDetailDs();
                AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentOrderDetailApt", "GetAdvancePaymentOrderDetailByKey");
                ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId;
                ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected >= 1)
                {
                    row = dataSet.AdvancePaymentOrderDetail[0];
                    if (row.IsInitial == 0 && row.Status != 0)
                    {
                        row.Status = 0;
                        sealStamp(row, userId, Stamp.UPDATE);
                        recordsAffected = ad.Update(dataSet);
                        if (recordsAffected < 1)
                            throw new DataAccessException("UpdateAdvancePaymentOrderDetail ERROR");
                        else
                        {
                            // Then, update the row of shipment deduction
                            ShippingWorker.Instance.deleteShipmentDeduction(paymentId, shipmentId, userId);
                        }
                    }
                }
                else
                    throw new DataAccessException("UpdateAdvancePaymentOrderDetail ERROR (No Record Found)");

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

        public AdvancePaymentActionHistoryDef getNewAdvancePaymentActionHistory(int paymentId, string description, int userId)
        {
            AdvancePaymentActionHistoryDef newhistory = new AdvancePaymentActionHistoryDef();
            newhistory.PaymentId = paymentId;
            newhistory.Description = description;
            newhistory.ActionBy = userId;
            newhistory.ActionOn = DateTime.Now;
            newhistory.Status = 1;
            return newhistory;
        }

        public AdvancePaymentActionHistoryDef getNewAdvancePaymentActionHistory(int paymentId, string description)
        {
            return getNewAdvancePaymentActionHistory(paymentId, description, 99999);
        }


        public bool isNSLRefNoInFLContract(string NSLRefNo)
        {
            bool exist = false;
            if (!string.IsNullOrEmpty(NSLRefNo))
            {
                IDataSetAdapter ad = null;
                ad = getDataSetAdapter("FLContractApt", "GetFLContractByCriteria");
                ad.SelectCommand.Parameters["@NSLRefNo"].Value = NSLRefNo;
                ad.PopulateCommands();

                DataSet ds = new DataSet();
                ad.SelectCommand.DbCommand.CommandTimeout = 120;
                int recordsAffected = ad.Fill(ds);
                exist = (recordsAffected > 0);
            }
            return exist;
        }

        #region mapping functions

        internal void AdvancePaymentMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(AdvancePaymentDs.AdvancePaymentRow) &&
                target.GetType() == typeof(AdvancePaymentDef))
            {
                AdvancePaymentDs.AdvancePaymentRow row = (AdvancePaymentDs.AdvancePaymentRow)source;
                AdvancePaymentDef def = (AdvancePaymentDef)target;

                def.PaymentId = row.PaymentId;
                def.PaymentNo = row.PaymentNo;
                def.OfficeId = row.OfficeId;
                if (row.IsPaymentDateNull())
                    def.PaymentDate = DateTime.MinValue;
                else
                    def.PaymentDate = row.PaymentDate;
                def.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
                def.PaymentTypeId = row.PaymentTypeId;
                if (!row.IsRejectByNull())
                    def.RejectedBy = generalWorker.getUserByKey(row.RejectBy);
                if (!row.IsRejectDateNull())
                    def.RejectedOn = row.RejectDate;
                else
                    def.RejectedOn = DateTime.MinValue;
                if (!row.IsUploadedByNull())
                    def.UploadedBy = generalWorker.getUserByKey(row.UploadedBy);
                if (!row.IsUploadedDateNull())
                    def.UploadedOn = row.UploadedDate;
                else
                    def.UploadedOn = DateTime.MinValue;
                if (!row.IsApprovedByNull())
                    def.ApprovedBy = generalWorker.getUserByKey(row.ApprovedBy);
                if (!row.IsApprovedDateNull())
                    def.ApprovedOn = row.ApprovedDate;
                else
                    def.ApprovedOn = DateTime.MinValue;
                if (!row.IsRejectReasonIdNull())
                    def.RejectReasonId = row.RejectReasonId;
                else
                    def.RejectReasonId = -1;
                def.WorkflowStatusId = row.WorkflowStatusId;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                if (!row.IsSubmittedByNull())
                    def.SubmittedBy = generalWorker.getUserByKey(row.SubmittedBy);
                if (!row.IsSubmittedDateNull())
                    def.SubmittedOn = row.SubmittedDate;
                else
                    def.SubmittedOn = DateTime.MinValue;
                def.TotalAmount = row.TotalAmt;
                def.Vendor = vendorWorker.getVendorByKey(row.VendorId);
                def.Status = row.Status;
                def.CreatedBy = generalWorker.getUserByKey(row.CreatedBy);
                if (!row.IsInitiatedByNull())
                    def.InitiatedBy = generalWorker.getUserByKey(row.InitiatedBy);
                if (!row.IsSettlementDateNull())
                    def.SettlementDate = row.SettlementDate;
                else
                    def.SettlementDate = DateTime.MinValue;
                def.InterestChargedAmt = row.InterestChargedAmt;
                def.InterestRate = row.InterestRate;
                def.IsInterfaced = row.IsInterfaced;
                if (!row.IsPayableToNull())
                {
                    def.PayableTo = row.PayableTo;
                }
                if (!row.IsIsC19Null())
                    def.IsC19 = row.IsC19;
                else
                    def.IsC19 = 0;
                if (!row.IsFLRefNoNull())
                {
                    def.FLRefNo = row.FLRefNo;
                }
            }
            else if (source.GetType() == typeof(AdvancePaymentDef) &&
                target.GetType() == typeof(AdvancePaymentDs.AdvancePaymentRow))
            {
                AdvancePaymentDef def = (AdvancePaymentDef)source;
                AdvancePaymentDs.AdvancePaymentRow row = (AdvancePaymentDs.AdvancePaymentRow)target;

                row.PaymentId = def.PaymentId;
                row.PaymentTypeId = def.PaymentTypeId;
                row.PaymentNo = def.PaymentNo;
                row.OfficeId = def.OfficeId;
                if (def.PaymentDate == DateTime.MinValue)
                    row.SetPaymentDateNull();
                else
                    row.PaymentDate = def.PaymentDate;
                if (def.Remark.Trim() != string.Empty)
                    row.Remark = def.Remark;
                else
                    row.SetRemarkNull();
                row.SubmittedDate = def.SubmittedOn;
                row.SubmittedBy = def.SubmittedBy.UserId;
                row.CurrencyId = def.Currency.CurrencyId;
                row.TotalAmt = def.TotalAmount;
                row.VendorId = def.Vendor.VendorId;
                row.Status = def.Status;
                row.WorkflowStatusId = def.WorkflowStatusId;
                row.InterestChargedAmt = def.InterestChargedAmt;
                row.InterestRate = def.InterestRate;
                row.IsInterfaced = def.IsInterfaced;
                row.PayableTo = def.PayableTo;
                row.InitiatedBy = def.InitiatedBy.UserId;
                row.IsC19 = def.IsC19;
                if (def.FLRefNo.Trim() != string.Empty)
                    row.FLRefNo = def.FLRefNo;
                else
                    row.SetFLRefNoNull();
            }
        }

        internal void AdvancePaymentOrderDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow) &&
                target.GetType() == typeof(AdvancePaymentOrderDetailDef))
            {
                AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow row = (AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow)source;
                AdvancePaymentOrderDetailDef def = (AdvancePaymentOrderDetailDef)target;

                def.PaymentId = row.PaymentId;
                def.ShipmentId = row.ShipmentId;
                def.ExpectedValue = row.ExpectedDeductAmt;
                def.ActualValue = row.ActualDeductAmt;
                if (!row.IsSettlementDateNull())
                    def.SettlementDate = row.SettlementDate;
                else
                    def.SettlementDate = DateTime.MinValue;
                def.IsInitial = Convert.ToBoolean(row.IsInitial);
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(AdvancePaymentOrderDetailDef) &&
                target.GetType() == typeof(AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow))
            {
                AdvancePaymentOrderDetailDef def = (AdvancePaymentOrderDetailDef)source;
                AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow row = (AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow)target;

                row.PaymentId = def.PaymentId;
                row.ShipmentId = def.ShipmentId;
                row.ExpectedDeductAmt = def.ExpectedValue;
                row.ActualDeductAmt = def.ActualValue;
                row.IsInitial = (def.IsInitial) ? 1 : 0;
                if (def.Remark.Trim() != string.Empty)
                    row.Remark = def.Remark;
                else
                    row.SetRemarkNull();
                if (def.SettlementDate != DateTime.MinValue)
                    row.SettlementDate = def.SettlementDate;
                else
                    row.SetSettlementDateNull();
                row.Status = def.Status;
            }
        }

        internal void AdvancePaymentInstalmentDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(AdvancePaymentInstalmentDetailDs.AdvancePaymentInstalmentDetailRow) &&
                target.GetType() == typeof(AdvancePaymentInstalmentDetailDef))
            {
                AdvancePaymentInstalmentDetailDs.AdvancePaymentInstalmentDetailRow row = (AdvancePaymentInstalmentDetailDs.AdvancePaymentInstalmentDetailRow)source;
                AdvancePaymentInstalmentDetailDef def = (AdvancePaymentInstalmentDetailDef)target;

                def.PaymentId = row.PaymentId;
                def.PaymentDate = row.PaymentDate;
                def.ExpectedAmount = row.ExpectedAmt;
                def.PaymentAmount = row.PaymentAmt;
                if (!row.IsSettlementDateNull())
                    def.SettlementDate = row.SettlementDate;
                else
                    def.SettlementDate = DateTime.MinValue;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                def.InterestRate = row.InterestRate;
                def.InterestAmt = row.InterestAmt;
                def.RemainingTotalAmt = row.RemainingTotalAmt;
                if (!row.IsInterestFromDateNull())
                    def.InterestFromDate = row.InterestFromDate;
                else
                    def.InterestFromDate = DateTime.MinValue;
                if (!row.IsInterestToDateNull())
                    def.InterestToDate = row.InterestToDate;
                else
                    def.InterestToDate = DateTime.MinValue;

                if (!row.IsDCNoteDateNull())
                    def.DCNoteDate = row.DCNoteDate;
                else
                    def.DCNoteDate = DateTime.MinValue;
                if (!row.IsDCNoteNoNull())
                    def.DCNoteNo = row.DCNoteNo;
                else
                    def.DCNoteNo = string.Empty;
                def.IsDCNoteInterfaced = row.IsDCNoteInterfaced;
                if (!row.IsMailStatusNull())
                    def.MailStatus = row.MailStatus;
                else
                    def.MailStatus = 0;

                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(AdvancePaymentInstalmentDetailDef) &&
                target.GetType() == typeof(AdvancePaymentInstalmentDetailDs.AdvancePaymentInstalmentDetailRow))
            {
                AdvancePaymentInstalmentDetailDef def = (AdvancePaymentInstalmentDetailDef)source;
                AdvancePaymentInstalmentDetailDs.AdvancePaymentInstalmentDetailRow row = (AdvancePaymentInstalmentDetailDs.AdvancePaymentInstalmentDetailRow)target;

                row.PaymentId = def.PaymentId;
                row.PaymentDate = def.PaymentDate;
                row.ExpectedAmt = def.ExpectedAmount;
                row.PaymentAmt = def.PaymentAmount;

                if (def.Remark.Trim() != string.Empty)
                    row.Remark = def.Remark;
                else
                    row.SetRemarkNull();
                if (def.SettlementDate != DateTime.MinValue)
                    row.SettlementDate = def.SettlementDate;
                else
                    row.SetSettlementDateNull();
                row.InterestRate = def.InterestRate;
                row.InterestAmt = def.InterestAmt;
                row.RemainingTotalAmt = def.RemainingTotalAmt;
                if (def.DCNoteDate != DateTime.MinValue)
                    row.DCNoteDate = def.DCNoteDate;
                else
                    row.SetDCNoteDateNull();
                if (def.InterestFromDate != DateTime.MinValue)
                    row.InterestFromDate = def.InterestFromDate;
                else
                    row.SetInterestFromDateNull();
                if (def.InterestToDate != DateTime.MinValue)
                    row.InterestToDate = def.InterestToDate;
                else
                    row.SetInterestToDateNull();

                if (def.DCNoteNo.Trim() != string.Empty)
                    row.DCNoteNo = def.DCNoteNo;
                else
                    row.SetDCNoteNoNull();
                if (def.IsDCNoteInterfaced)
                    row.IsDCNoteInterfaced = def.IsDCNoteInterfaced;
                else if (row.RowState == DataRowState.Detached)
                    row.IsDCNoteInterfaced = false;
                row.MailStatus = def.MailStatus;
                row.Status = def.Status;
            }
        }

        internal void AdvancePaymentBalanceSettlementMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(AdvancePaymentBalanceSettlementDs.AdvancePaymentBalanceSettlementRow) &&
                target.GetType() == typeof(AdvancePaymentBalanceSettlementDef))
            {
                AdvancePaymentBalanceSettlementDs.AdvancePaymentBalanceSettlementRow row = (AdvancePaymentBalanceSettlementDs.AdvancePaymentBalanceSettlementRow)source;
                AdvancePaymentBalanceSettlementDef def = (AdvancePaymentBalanceSettlementDef)target;

                def.PaymentId = row.PaymentId;
                def.PaymentDate = row.PaymentDate;
                def.ExpectedAmount = row.ExpectedAmt;
                def.PaymentAmount = row.PaymentAmt;
                if (!row.IsSettlementDateNull())
                    def.SettlementDate = row.SettlementDate;
                else
                    def.SettlementDate = DateTime.MinValue;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(AdvancePaymentBalanceSettlementDef) &&
                target.GetType() == typeof(AdvancePaymentBalanceSettlementDs.AdvancePaymentBalanceSettlementRow))
            {
                AdvancePaymentBalanceSettlementDef def = (AdvancePaymentBalanceSettlementDef)source;
                AdvancePaymentBalanceSettlementDs.AdvancePaymentBalanceSettlementRow row = (AdvancePaymentBalanceSettlementDs.AdvancePaymentBalanceSettlementRow)target;

                row.PaymentId = def.PaymentId;
                row.PaymentDate = def.PaymentDate;
                row.ExpectedAmt = def.ExpectedAmount;
                row.PaymentAmt = def.PaymentAmount;

                if (def.Remark.Trim() != string.Empty)
                    row.Remark = def.Remark;
                else
                    row.SetRemarkNull();
                if (def.SettlementDate != DateTime.MinValue)
                    row.SettlementDate = def.SettlementDate;
                else
                    row.SetSettlementDateNull();
                row.Status = def.Status;
            }
        }

        private void AdvancePaymentActionHistoryMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(AdvancePaymentActionHistoryDs.AdvancePaymentActionHistoryRow) &&
                target.GetType() == typeof(AdvancePaymentActionHistoryDef))
            {
                AdvancePaymentActionHistoryDs.AdvancePaymentActionHistoryRow row = (AdvancePaymentActionHistoryDs.AdvancePaymentActionHistoryRow)source;
                AdvancePaymentActionHistoryDef def = (AdvancePaymentActionHistoryDef)target;

                def.ActionHistoryId = row.ActionHistoryId;
                if (!row.IsPaymentIdNull())
                {
                    def.PaymentId = row.PaymentId;
                }
                if (!row.IsDescriptionNull())
                {
                    def.Description = row.Description;
                }
                if (!row.IsActionByNull())
                {
                    def.ActionBy = row.ActionBy;
                }
                if (!row.IsActionOnNull())
                {
                    def.ActionOn = row.ActionOn;
                }
                if (!row.IsStatusNull())
                {
                    def.Status = row.Status;
                }
            }
            else if (source.GetType() == typeof(AdvancePaymentActionHistoryDef) &&
                target.GetType() == typeof(AdvancePaymentActionHistoryDs.AdvancePaymentActionHistoryRow))
            {
                AdvancePaymentActionHistoryDef def = (AdvancePaymentActionHistoryDef)source;
                AdvancePaymentActionHistoryDs.AdvancePaymentActionHistoryRow row = (AdvancePaymentActionHistoryDs.AdvancePaymentActionHistoryRow)target;

                row.ActionHistoryId = def.ActionHistoryId;
                if (def.PaymentId != int.MinValue)
                {
                    row.PaymentId = def.PaymentId;
                }
                else
                    row.SetPaymentIdNull();
                if (def.Description != String.Empty)
                {
                    row.Description = def.Description;
                }
                else
                    row.SetDescriptionNull();
                if (def.ActionBy != int.MinValue)
                {
                    row.ActionBy = def.ActionBy;
                }
                else
                    row.SetActionByNull();
                if (def.ActionOn != DateTime.MinValue)
                {
                    row.ActionOn = def.ActionOn;
                }
                else
                    row.SetActionOnNull();
                if (def.Status != int.MinValue)
                {
                    row.Status = def.Status;
                }
                else
                    row.SetStatusNull();
            }
        }

        #endregion
    }
}
