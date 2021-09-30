using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using com.next.isam.dataserver.model.product;
using com.next.isam.dataserver.model.shipping;
using com.next.isam.dataserver.model.common;
using com.next.isam.dataserver.model.order;
using com.next.isam.dataserver.model.nss;
using com.next.isam.dataserver.model.sync;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.common;
using com.next.isam.domain.shipping;
using com.next.isam.domain.account;
using com.next.infra.persistency.transactions;
using com.next.infra.persistency.dataaccess;
using com.next.infra.util;
using com.next.isam.dataserver.model.account;
using com.next.common.web.commander;


namespace com.next.isam.dataserver.worker
{
    public class SynchronizationWorker : Worker
    {
        private static SynchronizationWorker _instance;
        private CommonWorker commonWorker;
        private GeneralWorker generalWorker;
        private ProductWorker productWorker;
        private AccountWorker accountWorker;
        private AdvancePaymentWorker advancePaymentWorker;

        public SynchronizationWorker()
        {
            commonWorker = CommonWorker.Instance;
            generalWorker = GeneralWorker.Instance;
            productWorker = ProductWorker.Instance;
            accountWorker = AccountWorker.Instance;
            advancePaymentWorker = AdvancePaymentWorker.Instance;
        }

        public static SynchronizationWorker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SynchronizationWorker();
                }
                return _instance;
            }
        }

        public ArrayList getSyncShipmentList()
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncShipmentApt", "GetSyncShipmentList");
            NssShipmentDs dataSet = new NssShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();
            foreach (NssShipmentDs.ShipmentRow row in dataSet.Shipment)
            {
                ShipmentSyncRef def = new ShipmentSyncRef();
                def.ShipmentId = row.ShipmentId;
                def.WorkflowStatus = ContractWFS.getType(row.WorkflowStatusId);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getSyncAdvancePaymentKeyList()
        {
            IDataSetAdapter ad = getDataSetAdapter("NSSAdvancePaymentApt", "GetAdvancePaymentKeyList");
            DataSet dataset = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataset);
            ArrayList list = new ArrayList();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                list.Add(int.Parse(row["PaymentId"].ToString()));
            }
            return list;
        }

        public ArrayList getSyncLGPaymentKeyList()
        {
            IDataSetAdapter ad = getDataSetAdapter("NSSLGPaymentApt", "GetLGPaymentKeyList");
            DataSet dataset = new DataSet();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataset);
            ArrayList list = new ArrayList();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                list.Add(int.Parse(row["PaymentId"].ToString()));
            }
            return list;
        }

        public void complete(int shipmentId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                NssShipmentDs dataSet = new NssShipmentDs();
                NssShipmentDs.ShipmentRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("SyncShipmentApt", "GetShipment");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);
                row = dataSet.Shipment[0];
                row.Sync = false;
                row.SetDMSUploadDateNull();
                row.LastSyncDate = DateTime.Now;
                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Completing Synchronization Error");
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

        public void AdvancePaymentComplete(int paymentId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                NssAdvancePaymentDs dataSet = new NssAdvancePaymentDs();
                NssAdvancePaymentDs.AdvancePaymentRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("NSSAdvancePaymentApt", "GetAdvancePaymentByKey");
                ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId.ToString();
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);
                row = dataSet.AdvancePayment[0];
                row.Sync = 0;
                sealStamp(row, 99999, Stamp.UPDATE);
                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Completing Advance Payment Synchronization Error");
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

        public void LGPaymentComplete(int paymentId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                NssLGPaymentDs dataSet = new NssLGPaymentDs();
                NssLGPaymentDs.LGPaymentRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("NSSLGPaymentApt", "GetLGPaymentByKey");
                ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId.ToString();
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);
                row = dataSet.LGPayment[0];
                row.Sync = 0;
                sealStamp(row, 99999, Stamp.UPDATE);
                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Completing LG Payment Synchronization Error");
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

        #region Get NSS Source Data Functions

        public NssContractDs getSyncContract(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncContractApt", "GetContract");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            NssContractDs dataSet = new NssContractDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssShipmentDs getSyncShipment(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncShipmentApt", "GetShipment");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            NssShipmentDs dataSet = new NssShipmentDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssShipmentDetailDs getSyncShipmentDetail(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncShipmentDetailApt", "GetShipmentDetail");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            NssShipmentDetailDs dataSet = new NssShipmentDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssShipmentDetailDs getSyncShipmentDetailByKey(int shipmentDetailId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncShipmentDetailApt", "GetShipmentDetailByKey");
            ad.SelectCommand.Parameters["@ShipmentDetailId"].Value = shipmentDetailId.ToString();

            NssShipmentDetailDs dataSet = new NssShipmentDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssSplitShipmentDs getSyncSplitShipment(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncSplitShipmentApt", "GetSplitShipment");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            NssSplitShipmentDs dataSet = new NssSplitShipmentDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssSplitShipmentDs getSyncSplitShipmentByKey(int splitShipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncSplitShipmentApt", "GetSplitShipmentByKey");
            ad.SelectCommand.Parameters["@SplitShipmentId"].Value = splitShipmentId.ToString();

            NssSplitShipmentDs dataSet = new NssSplitShipmentDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssSplitShipmentDetailDs getSyncSplitShipmentDetail(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncSplitShipmentDetailApt", "GetSplitShipmentDetail");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            NssSplitShipmentDetailDs dataSet = new NssSplitShipmentDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssSplitShipmentDetailDs getSyncSplitShipmentDetailByKey(int splitShipmentDetailId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncSplitShipmentDetailApt", "GetSplitShipmentDetailByKey");
            ad.SelectCommand.Parameters["@SplitShipmentDetailId"].Value = splitShipmentDetailId.ToString();

            NssSplitShipmentDetailDs dataSet = new NssSplitShipmentDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssSizeOptionDs getSyncSizeOption(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncSizeOptionApt", "GetSizeOption");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            NssSizeOptionDs dataSet = new NssSizeOptionDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssProductDs getSyncProduct(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncProductApt", "GetProduct");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            NssProductDs dataSet = new NssProductDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssOtherCostDs getSyncOtherCost(int shipmentId)
        {
            try
            {
                IDataSetAdapter ad = getDataSetAdapter("SyncOtherCostApt", "GetOtherCost");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

                NssOtherCostDs dataSet = new NssOtherCostDs();
                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected < 1) return null;
                return dataSet;
            }
            catch (Exception e)
            {
                MailHelper.sendGeneralMessage("Error occurred when synchronizing other cost", e.Message);
                throw new ApplicationException("Error occurred when synchronizing other cost");
            }

        }

        public NssOtherCostDs getSyncSplitOtherCost(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SyncOtherCostApt", "GetSplitOtherCost");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            NssOtherCostDs dataSet = new NssOtherCostDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssAdvancePaymentDs getSyncAdvancePayment(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NSSAdvancePaymentApt", "GetAdvancePaymentByKey");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId.ToString();

            NssAdvancePaymentDs dataSet = new NssAdvancePaymentDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssAdvancePaymentOrderDetailDs getSyncAdvancePaymentOrderDetail(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NSSAdvancePaymentOrderDetailApt", "GetAdvancePaymentOrderDetailByPaymentId");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId.ToString();

            NssAdvancePaymentOrderDetailDs dataSet = new NssAdvancePaymentOrderDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssLGPaymentDs getSyncLGPayment(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NSSLGPaymentApt", "GetLGPaymentByKey");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId.ToString();

            NssLGPaymentDs dataSet = new NssLGPaymentDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public NssLGPaymentOrderDetailDs getSyncLGPaymentOrderDetail(int paymentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NSSLGPaymentOrderDetailApt", "GetLGPaymentOrderDetailByPaymentId");
            ad.SelectCommand.Parameters["@PaymentId"].Value = paymentId.ToString();

            NssLGPaymentOrderDetailDs dataSet = new NssLGPaymentOrderDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;
            return dataSet;
        }

        public string getAdvancePaymentRejectReasonNameByKey(int reasonId)
        {
            IDataSetAdapter ad = getDataSetAdapter("NSSAdvancePaymentRejectReasonApt", "GetAdvancePaymentRejectReasonNameByKey");
            ad.SelectCommand.Parameters["@RejectReasonId"].Value = reasonId.ToString();
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return "";
            else
                return (dataSet.Tables[0].Rows[0][0].ToString());
        }

        #endregion

        #region Synchronize Data to ISAM functions

        public ActionHistoryDef getNewActionHistoryDef(int shipmentId, int splitShipmentId, AmendmentType amendType, string sourceValue, string targetValue)
        {
            string s = amendType.Description + " : " + sourceValue + " -> " + targetValue;
            return new ActionHistoryDef(shipmentId, splitShipmentId, ActionHistoryType.MERCHANDISER_UPDATES, amendType, s, 99999);
        }

        public void syncContract(NssContractDs ds, int shipmentId, bool isInitial, ArrayList amendmentList)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                NssContractDs.ContractRow sourceRow = ds.Contract[0];
                ContractDs dataSet = new ContractDs();
                ContractDs.ContractRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("ContractApt", "GetContractByKey");
                ad.SelectCommand.Parameters["@ContractId"].Value = sourceRow.ContractId;
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);
                ArrayList contractAmendmentList = new ArrayList();

                if (recordsAffected > 0)
                {
                    row = dataSet.Contract[0];

                    if (!isInitial)
                    {
                        if (row.OfficeId != sourceRow.OfficeId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.OFFICE, generalWorker.getOfficeRefByKey(row.OfficeId).OfficeCode, generalWorker.getOfficeRefByKey(sourceRow.OfficeId).OfficeCode));
                        if (row.TradingAgencyId != sourceRow.TradingAgencyId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.TRADING_AGENCY, TradingAgencyId.getName(row.TradingAgencyId), TradingAgencyId.getName(sourceRow.TradingAgencyId)));
                        if (row.CustomerId != sourceRow.CustomerId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.CUSTOMER, commonWorker.getCustomerByKey(row.CustomerId).CustomerCode, commonWorker.getCustomerByKey(sourceRow.CustomerId).CustomerCode));
                        if (row.DeptId != sourceRow.DeptId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.PRODUCT_DEPARTMENT, generalWorker.getProductDepartmentByKey(row.DeptId).Code, generalWorker.getProductDepartmentByKey(sourceRow.DeptId).Code));
                        if (row.ProductTeamId != sourceRow.ProductTeamId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.PRODUCT_TEAM, generalWorker.getProductCodeDefByKey(row.ProductTeamId).Code, generalWorker.getProductCodeDefByKey(sourceRow.ProductTeamId).Code));
                        if (row.SeasonId != sourceRow.SeasonId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.SEASON, generalWorker.getSeasonByKey(row.SeasonId).Code, generalWorker.getSeasonByKey(sourceRow.SeasonId).Code));
                        if (row.ProductId != sourceRow.ProductId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.ITEM, productWorker.getProductRefByKey(row.ProductId).ItemNo, productWorker.getProductRefByKey(sourceRow.ProductId).ItemNo));
                        if ((row.IsIsShortGameNull() ? 0 : 1) != (sourceRow.IsIsShortGameNull() ? 0 : 1))
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.SHORT_GAME_INDICATOR, (row.IsIsShortGameNull() ? 0 : 1) == 1 ? "True" : "False", (sourceRow.IsIsShortGameNull() ? 0 : 1) == 1 ? "True" : "False"));
                    }
                    ContractMapping(sourceRow, row);
                }
                else
                {
                    row = dataSet.Contract.NewContractRow();
                    ContractMapping(sourceRow, row);
                    dataSet.Contract.AddContractRow(row);
                }
                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Synchronizing Contract Error");

                if (amendmentList.Count > 0)
                {
                    ArrayList shipmentList = (ArrayList)OrderSelectWorker.Instance.getShipmentByContractId(sourceRow.ContractId);

                    foreach (ShipmentDef shipmentDef in shipmentList)
                    {
                        if (shipmentDef.ShipmentId != shipmentId)
                            AccountWorker.Instance.doReversal(shipmentDef.ShipmentId, amendmentList);
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

        public void syncShipment(NssShipmentDs ds, NssShipmentDetailDs detailDs, int shipmentId, bool isInitial, ArrayList amendmentList, bool isNSLSZOrder, int otherCurrencyId)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                NssShipmentDs.ShipmentRow sourceRow = ds.Shipment[0];
                ShipmentDs dataSet = new ShipmentDs();
                ShipmentDs.ShipmentRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("ShipmentApt", "GetShipmentByKey");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = sourceRow.ShipmentId;
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.Shipment[0];

                    if (sourceRow.VendorId == 0)
                    {
                        NoticeHelper.sendMissingVendorIdDataSyncError(shipmentId);
                    }

                    if (!isInitial)
                    {
                        if (otherCurrencyId > 0)
                        {
                            if (row.BuyCurrencyId != otherCurrencyId)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.BUY_CURRENCY, generalWorker.getCurrencyByKey(row.BuyCurrencyId).CurrencyCode, generalWorker.getCurrencyByKey(otherCurrencyId).CurrencyCode));
                        }
                        else
                        {
                            if (row.BuyCurrencyId != sourceRow.BuyCurrencyId)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.BUY_CURRENCY, generalWorker.getCurrencyByKey(row.BuyCurrencyId).CurrencyCode, generalWorker.getCurrencyByKey(sourceRow.BuyCurrencyId).CurrencyCode));
                        }
                        if (row.SellCurrencyId != sourceRow.SellCurrencyId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.SELL_CURRENCY, generalWorker.getCurrencyByKey(row.SellCurrencyId).CurrencyCode, generalWorker.getCurrencyByKey(sourceRow.SellCurrencyId).CurrencyCode));
                        if (row.CustomerDestinationId != sourceRow.CustomerDestinationId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.DESTINATION, commonWorker.getCustomerDestinationByKey(row.CustomerDestinationId).DestinationDesc, commonWorker.getCustomerDestinationByKey(sourceRow.CustomerDestinationId).DestinationDesc));
                        if (row.VendorId != (isNSLSZOrder ? 3933 : sourceRow.VendorId))
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.VENDOR, VendorWorker.Instance.getVendorByKey(row.VendorId).Name, VendorWorker.Instance.getVendorByKey((isNSLSZOrder ? 3933 : sourceRow.VendorId)).Name));
                        if (row.TermOfPurchaseId != sourceRow.TermOfPurchaseId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.TERM_OF_PURCHASE, commonWorker.getTermOfPurchaseByKey(row.TermOfPurchaseId).TermOfPurchaseDescription, commonWorker.getTermOfPurchaseByKey(sourceRow.TermOfPurchaseId).TermOfPurchaseDescription));
                        if (row.PaymentTermId != sourceRow.PaymentTermId)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.PAYMENT_TERM, commonWorker.getPaymentTermByKey(row.PaymentTermId).PaymentTermDescription, commonWorker.getPaymentTermByKey(sourceRow.PaymentTermId).PaymentTermDescription));
                        if (row.NSLCommissionPercent != sourceRow.NSLCommissionPercent)
                            amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.SALES_COMMISSION_PERCENT, row.NSLCommissionPercent.ToString() + "%", sourceRow.NSLCommissionPercent.ToString() + "%"));
                        if (!sourceRow.IsQaCommissionPercentNull())
                        {
                            if (row.QACommissionPercent != sourceRow.QaCommissionPercent)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.QA_COMMISSION_PERCENT, row.QACommissionPercent.ToString("0.00") + "%", sourceRow.QaCommissionPercent.ToString("0.00") + "%"));
                        }
                        if (!sourceRow.IsGTCommissionPercentNull())
                        {
                            if (row.GTCommissionPercent != sourceRow.GTCommissionPercent)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.GT_COMMISSION_PERCENT, row.QACommissionPercent.ToString("0.00") + "%", sourceRow.GTCommissionPercent.ToString("0.00") + "%"));
                        }
                        if (!sourceRow.IsVendorPaymentDiscountPercentNull())
                        {
                            if (row.VendorPaymentDiscountPercent != sourceRow.VendorPaymentDiscountPercent)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.VENDOR_PAYMENT_DISCOUNT, row.VendorPaymentDiscountPercent.ToString("0.00") + "%", sourceRow.VendorPaymentDiscountPercent.ToString("0.00") + "%"));
                        }
                        if (!sourceRow.IsLabTestIncomeNull())
                        {
                            if (row.LabTestIncome != sourceRow.LabTestIncome)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.LAB_TEST_INCOME, row.LabTestIncome.ToString("0.0000"), sourceRow.LabTestIncome.ToString("0.0000")));
                        }
                        if (!sourceRow.IsGSPFormTypeIdNull())
                        {
                            if (row.IsGSPFormTypeIdNull())
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.GSPFORMTYPE, GSPFormType.getName(0), GSPFormType.getName(sourceRow.GSPFormTypeId)));
                            else if (row.GSPFormTypeId != sourceRow.GSPFormTypeId)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.GSPFORMTYPE, GSPFormType.getName(row.GSPFormTypeId), GSPFormType.getName(sourceRow.GSPFormTypeId)));
                        }
                    }
                    ShipmentMapping(sourceRow, row, detailDs, isNSLSZOrder, otherCurrencyId);
                }
                else
                {
                    row = dataSet.Shipment.NewShipmentRow();
                    ShipmentMapping(sourceRow, row, detailDs, isNSLSZOrder, otherCurrencyId);
                    dataSet.Shipment.AddShipmentRow(row);
                }
                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Synchronizing Shipment Error");

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

        public void syncShipmentDetail(NssShipmentDetailDs ds, int shipmentId, bool isInitial, ArrayList amendmentList, bool isNSLSZOrder, int otherCurrencyId)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                ShipmentDetailDs dataSet = null;
                ShipmentDetailDs.ShipmentDetailRow row = null;

                foreach (NssShipmentDetailDs.ShipmentDetailRow sourceRow in ds.ShipmentDetail.Rows)
                {
                    dataSet = new ShipmentDetailDs();
                    row = null;
                    IDataSetAdapter ad = getDataSetAdapter("ShipmentDetailApt", "GetShipmentDetailByKey");
                    ad.SelectCommand.Parameters["@ShipmentDetailId"].Value = sourceRow.ShipmentDetailId;
                    ad.PopulateCommands();
                    int recordsAffected = ad.Fill(dataSet);
                    if (recordsAffected > 0)
                    {
                        row = dataSet.ShipmentDetail[0];
                        if (!isInitial)
                        {
                            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(row.ShipmentId);
                            if (row.Status == GeneralStatus.ACTIVE.Code && sourceRow.Status == GeneralStatus.INACTIVE.Code && row.ShippedQty != 0)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.ACTUAL_QUANTITY, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.ShippedQty.ToString(), "0"));
                            else
                            {
                                if (row.SellingPrice != sourceRow.SellingPrice)
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.SELLING_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.SellingPrice.ToString("0.00"), sourceRow.SellingPrice.ToString("0.00")));
                                if (row.NetFOBPrice != (isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice)))
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.NET_FOB_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.NetFOBPrice.ToString("0.00"), (isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice)).ToString("0.00")));
                                if (row.SupplierGmtPrice != (isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice)))
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.SUPPLIER_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.SupplierGmtPrice.ToString("0.00"), (isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice)).ToString("0.00")));
                                if (row.ReducedSellingPrice != (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice))
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.REDUCED_SELLING_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.ReducedSellingPrice.ToString("0.00"), (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice).ToString("0.00")));
                                if (row.ReducedNetFOBPrice != (isNSLSZOrder ? (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice) : (sourceRow.ReducedNetFOBPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice) : sourceRow.ReducedNetFOBPrice)))
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.REDUCED_NET_FOB_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.ReducedNetFOBPrice.ToString("0.00"), (isNSLSZOrder ? (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice) : (sourceRow.ReducedNetFOBPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice) : sourceRow.ReducedNetFOBPrice)).ToString("0.00")));
                                if (row.ReducedSupplierGmtPrice != (isNSLSZOrder ? (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice) : (sourceRow.ReducedSupplierGmtPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice) : sourceRow.ReducedSupplierGmtPrice)))
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, 0, AmendmentType.REDUCED_SUPPLIER_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.ReducedSupplierGmtPrice.ToString("0.00"), (isNSLSZOrder ? (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice) : (sourceRow.ReducedSupplierGmtPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice) : sourceRow.ReducedSupplierGmtPrice)).ToString("0.00")));
                            }
                        }
                        ShipmentDetailMapping(row, sourceRow, isNSLSZOrder, otherCurrencyId);
                    }
                    else
                    {
                        row = dataSet.ShipmentDetail.NewShipmentDetailRow();
                        ShipmentDetailMapping(row, sourceRow, isNSLSZOrder, otherCurrencyId);
                        dataSet.ShipmentDetail.AddShipmentDetailRow(row);
                    }
                    recordsAffected = ad.Update(dataSet);

                    if (recordsAffected < 1)
                        throw new DataAccessException("Synchronizing Shipment Detail Error");
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

        public void syncSplitShipment(NssSplitShipmentDs ds, NssSplitShipmentDetailDs detailDs, int shipmentId, bool isInitial, ArrayList amendmentList, bool isNSLSZOrder, int otherCurrencyId)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                SplitShipmentDs dataSet = null;
                SplitShipmentDs.SplitShipmentRow row = null;

                foreach (NssSplitShipmentDs.SplitShipmentRow sourceRow in ds.SplitShipment.Rows)
                {
                    dataSet = new SplitShipmentDs();
                    row = null;
                    IDataSetAdapter ad = getDataSetAdapter("SplitShipmentApt", "GetSplitShipmentByKey");
                    ad.SelectCommand.Parameters["@SplitShipmentId"].Value = sourceRow.SplitShipmentId;
                    ad.PopulateCommands();
                    int recordsAffected = ad.Fill(dataSet);
                    if (recordsAffected > 0)
                    {
                        row = dataSet.SplitShipment[0];

                        if (!isInitial)
                        {
                            if (row.VendorId != sourceRow.VendorId)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.VENDOR, VendorWorker.Instance.getVendorByKey(row.VendorId).Name, VendorWorker.Instance.getVendorByKey(sourceRow.VendorId).Name));
                            if (otherCurrencyId > 0)
                            {
                                if (row.BuyCurrencyId != otherCurrencyId)
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.BUY_CURRENCY, generalWorker.getCurrencyByKey(row.BuyCurrencyId).CurrencyCode, generalWorker.getCurrencyByKey(otherCurrencyId).CurrencyCode));
                            }
                            else
                            {
                                if (row.BuyCurrencyId != sourceRow.BuyCurrencyId)
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.BUY_CURRENCY, generalWorker.getCurrencyByKey(row.BuyCurrencyId).CurrencyCode, generalWorker.getCurrencyByKey(sourceRow.BuyCurrencyId).CurrencyCode));
                            }

                            if (row.SellCurrencyId != sourceRow.SellCurrencyId)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.SELL_CURRENCY, generalWorker.getCurrencyByKey(row.SellCurrencyId).CurrencyCode, generalWorker.getCurrencyByKey(sourceRow.SellCurrencyId).CurrencyCode));
                            if (row.PaymentTermId != sourceRow.PaymentTermId)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.PAYMENT_TERM, commonWorker.getPaymentTermByKey(row.PaymentTermId).PaymentTermDescription, commonWorker.getPaymentTermByKey(sourceRow.PaymentTermId).PaymentTermDescription));

                            if (!sourceRow.IsQaCommissionPercentNull())
                            {
                                if (row.QACommissionPercent != sourceRow.QaCommissionPercent)
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.QA_COMMISSION_PERCENT, row.QACommissionPercent.ToString("0.00") + "%", sourceRow.QaCommissionPercent.ToString("0.00") + "%"));
                            }
                            if (!sourceRow.IsVendorPaymentDiscountPercentNull())
                            {
                                if (row.VendorPaymentDiscountPercent != sourceRow.VendorPaymentDiscountPercent)
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.VENDOR_PAYMENT_DISCOUNT, row.VendorPaymentDiscountPercent.ToString("0.00") + "%", sourceRow.VendorPaymentDiscountPercent.ToString("0.00") + "%"));
                            }
                            if (!sourceRow.IsLabTestIncomeNull())
                            {
                                if (row.LabTestIncome != sourceRow.LabTestIncome)
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.LAB_TEST_INCOME, row.LabTestIncome.ToString("0.0000"), sourceRow.LabTestIncome.ToString("0.0000")));
                            }

                        }
                        SplitShipmentMapping(row, sourceRow, detailDs, isNSLSZOrder, otherCurrencyId);
                    }
                    else
                    {
                        AccountWorker.Instance.createDailySunInterfaceEntries(shipmentId, sourceRow.SplitShipmentId);
                        row = dataSet.SplitShipment.NewSplitShipmentRow();
                        SplitShipmentMapping(row, sourceRow, detailDs, isNSLSZOrder, otherCurrencyId);
                        dataSet.SplitShipment.AddSplitShipmentRow(row);

                        ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, ActionHistoryType.MERCHANDISER_UPDATES, "Initial data transfer", GeneralStatus.ACTIVE.Code, 99999));
                    }
                    recordsAffected = ad.Update(dataSet);

                    if (recordsAffected < 1)
                        throw new DataAccessException("Synchronizing Split Shipment Error");
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

        public void syncSplitShipmentDetail(NssSplitShipmentDetailDs ds, int shipmentId, bool isInitial, ArrayList amendmentList, bool isNSLSZOrder, int otherCurrencyId)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                SplitShipmentDetailDs dataSet = null;
                SplitShipmentDetailDs.SplitShipmentDetailRow row = null;

                foreach (NssSplitShipmentDetailDs.SplitShipmentDetailRow sourceRow in ds.SplitShipmentDetail.Rows)
                {
                    dataSet = new SplitShipmentDetailDs();
                    row = null;
                    IDataSetAdapter ad = getDataSetAdapter("SplitShipmentDetailApt", "GetSplitShipmentDetailByKey");
                    ad.SelectCommand.Parameters["@SplitShipmentDetailId"].Value = sourceRow.SplitShipmentDetailId;
                    ad.PopulateCommands();
                    int recordsAffected = ad.Fill(dataSet);
                    if (recordsAffected > 0)
                    {
                        row = dataSet.SplitShipmentDetail[0];
                        if (!isInitial)
                        {
                            if (row.SellingPrice != sourceRow.SellingPrice)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.SELLING_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.SellingPrice.ToString("0.00"), sourceRow.SellingPrice.ToString("0.00")));
                            if (row.NetFOBPrice != (isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice)))
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.NET_FOB_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.NetFOBPrice.ToString("0.00"), (isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice)).ToString("0.00")));
                            if (row.SupplierGmtPrice != (isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice)))
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.SUPPLIER_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.SupplierGmtPrice.ToString("0.00"), (isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice)).ToString("0.00")));
                            if (row.ReducedSellingPrice != (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice))
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.REDUCED_SELLING_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.ReducedSellingPrice.ToString("0.00"), (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice).ToString("0.00")));
                            if (row.ReducedNetFOBPrice != (isNSLSZOrder ? (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice) : (sourceRow.ReducedNetFOBPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice) : sourceRow.ReducedNetFOBPrice)))
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.REDUCED_NET_FOB_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.ReducedNetFOBPrice.ToString("0.00"), (isNSLSZOrder ? (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice) : (sourceRow.ReducedNetFOBPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice) : sourceRow.ReducedNetFOBPrice)).ToString("0.00")));
                            if (row.ReducedSupplierGmtPrice != (isNSLSZOrder ? (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice) : (sourceRow.ReducedSupplierGmtPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice) : sourceRow.ReducedSupplierGmtPrice)))
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, sourceRow.SplitShipmentId, AmendmentType.REDUCED_SUPPLIER_PRICE, "[Option #" + commonWorker.getSizeOptionByKey(row.SizeOptionId).SizeOptionNo + "]: " + row.ReducedSupplierGmtPrice.ToString("0.00"), (isNSLSZOrder ? (sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice) : (sourceRow.ReducedSupplierGmtPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice) : sourceRow.ReducedSupplierGmtPrice)).ToString("0.00")));
                        }
                        SplitShipmentDetailMapping(row, sourceRow, isNSLSZOrder, otherCurrencyId);
                    }
                    else
                    {
                        row = dataSet.SplitShipmentDetail.NewSplitShipmentDetailRow();
                        SplitShipmentDetailMapping(row, sourceRow, isNSLSZOrder, otherCurrencyId);
                        dataSet.SplitShipmentDetail.AddSplitShipmentDetailRow(row);
                    }
                    recordsAffected = ad.Update(dataSet);

                    if (recordsAffected < 1)
                        throw new DataAccessException("Synchronizing Split Shipment Detail Error");
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

        public void syncOtherCost(NssOtherCostDs ds, bool isSetSplit, int shipmentId, bool isInitial, ArrayList amendmentList)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                OtherCostDs dataSet = null;
                OtherCostDs.OtherCostRow row = null;
                SplitShipmentDetailDef splitDetailDef = null;
                ShipmentDetailDef detailDef = null;

                foreach (NssOtherCostDs.OtherCostRow sourceRow in ds.OtherCost.Rows)
                {
                    dataSet = new OtherCostDs();
                    row = null;
                    IDataSetAdapter ad = getDataSetAdapter(isSetSplit ? "SplitOtherCostApt" : "OtherCostApt", isSetSplit ? "GetSplitOtherCostByKey" : "GetOtherCostByKey");
                    ad.SelectCommand.Parameters["@OtherCostId"].Value = sourceRow.OtherCostId;
                    ad.PopulateCommands();
                    int recordsAffected = ad.Fill(dataSet);
                    if (recordsAffected > 0)
                    {
                        row = dataSet.OtherCost[0];
                        if (!isInitial)
                        {
                            splitDetailDef = null;
                            detailDef = null;

                            if (isSetSplit)
                                splitDetailDef = OrderSelectWorker.Instance.getSplitShipmentDetailByKey(sourceRow.ShipmentDetailId);
                            else
                                detailDef = OrderSelectWorker.Instance.getShipmentDetailByKey(sourceRow.ShipmentDetailId);


                            if (sourceRow.OtherCostTypeId != row.OtherCostTypeId && row.Status == 1 && sourceRow.Status == 1)
                            {
                                if (OtherCostTypeRef.getAmendmentTypeIdByOtherCostTypeId(row.OtherCostTypeId) != -1)
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, isSetSplit ? splitDetailDef.SplitShipmentId : 0, AmendmentType.getType(OtherCostTypeRef.getAmendmentTypeIdByOtherCostTypeId(row.OtherCostTypeId)), "[Option #" + (isSetSplit ? splitDetailDef.SizeOption.SizeOptionNo : detailDef.SizeOption.SizeOptionNo) + "]: " + row.OtherCostAmt.ToString("0.00"), "0.00"));

                                if (OtherCostTypeRef.getAmendmentTypeIdByOtherCostTypeId(sourceRow.OtherCostTypeId) != -1)
                                    amendmentList.Add(getNewActionHistoryDef(shipmentId, isSetSplit ? splitDetailDef.SplitShipmentId : 0, AmendmentType.getType(OtherCostTypeRef.getAmendmentTypeIdByOtherCostTypeId(sourceRow.OtherCostTypeId)), "[Option #" + (isSetSplit ? splitDetailDef.SizeOption.SizeOptionNo : detailDef.SizeOption.SizeOptionNo) + "]: " + "0.00", sourceRow.OtherCostAmt.ToString("0.00")));
                            }
                            else
                            {

                                if (row.OtherCostAmt != sourceRow.OtherCostAmt || (row.Status == 1 && sourceRow.Status == 0))
                                {
                                    string targetValue = sourceRow.OtherCostAmt.ToString("0.00");
                                    if (row.Status == 1 && sourceRow.Status == 0) targetValue = "NIL";

                                    if (OtherCostTypeRef.getAmendmentTypeIdByOtherCostTypeId(sourceRow.OtherCostTypeId) != -1)
                                        amendmentList.Add(getNewActionHistoryDef(shipmentId, isSetSplit ? splitDetailDef.SplitShipmentId : 0, AmendmentType.getType(OtherCostTypeRef.getAmendmentTypeIdByOtherCostTypeId(sourceRow.OtherCostTypeId)), "[Option #" + (isSetSplit ? splitDetailDef.SizeOption.SizeOptionNo : detailDef.SizeOption.SizeOptionNo) + "]: " + row.OtherCostAmt.ToString("0.00"), targetValue));

                                }
                            }
                        }
                        OtherCostMapping(row, sourceRow);
                    }
                    else
                    {
                        row = dataSet.OtherCost.NewOtherCostRow();

                        if (!isInitial)
                        {
                            splitDetailDef = null;
                            detailDef = null;
                            if (isSetSplit)
                                splitDetailDef = OrderSelectWorker.Instance.getSplitShipmentDetailByKey(sourceRow.ShipmentDetailId);
                            else
                                detailDef = OrderSelectWorker.Instance.getShipmentDetailByKey(sourceRow.ShipmentDetailId);

                            string targetValue = sourceRow.OtherCostAmt.ToString("0.00");

                            if (OtherCostTypeRef.getAmendmentTypeIdByOtherCostTypeId(sourceRow.OtherCostTypeId) != -1)
                                amendmentList.Add(getNewActionHistoryDef(shipmentId, isSetSplit ? splitDetailDef.SplitShipmentId : 0, AmendmentType.getType(OtherCostTypeRef.getAmendmentTypeIdByOtherCostTypeId(sourceRow.OtherCostTypeId)), "[Option #" + (isSetSplit ? splitDetailDef.SizeOption.SizeOptionNo : detailDef.SizeOption.SizeOptionNo) + "]: " + "NIL", targetValue));
                        }
                        OtherCostMapping(row, sourceRow);
                        dataSet.OtherCost.AddOtherCostRow(row);
                    }
                    recordsAffected = ad.Update(dataSet);

                    if (recordsAffected < 1)
                        throw new DataAccessException("Synchronizing Other Cost Error");
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

        public void syncProduct(NssProductDs ds, int shipmentId, bool isInitial, ArrayList amendmentList)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                ProductDs dataSet = null;
                ProductDs.ProductRow row = null;

                foreach (NssProductDs.ProductRow sourceRow in ds.Product.Rows)
                {
                    dataSet = new ProductDs();
                    row = null;
                    IDataSetAdapter ad = getDataSetAdapter("ProductApt", "GetProductByKey");
                    ad.SelectCommand.Parameters["@ProductId"].Value = sourceRow.ProductId;
                    ad.PopulateCommands();
                    int recordsAffected = ad.Fill(dataSet);
                    if (recordsAffected > 0)
                    {
                        row = dataSet.Product[0];
                        ProductMapping(row, sourceRow);
                    }
                    else
                    {
                        row = dataSet.Product.NewProductRow();
                        ProductMapping(row, sourceRow);
                        dataSet.Product.AddProductRow(row);
                    }
                    recordsAffected = ad.Update(dataSet);

                    if (recordsAffected < 1)
                        throw new DataAccessException("Synchronizing Product Error");
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

        public void syncSizeOption(NssSizeOptionDs ds, int shipmentId, bool isInitial, ArrayList amendmentList)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                SizeOptionDs dataSet = null;
                SizeOptionDs.SizeOptionRow row = null;

                foreach (NssSizeOptionDs.SizeOptionRow sourceRow in ds.SizeOption.Rows)
                {
                    dataSet = new SizeOptionDs();
                    row = null;
                    IDataSetAdapter ad = getDataSetAdapter("SizeOptionApt", "GetSizeOptionByKey");
                    ad.SelectCommand.Parameters["@SizeOptionId"].Value = sourceRow.SizeOptionId;
                    ad.PopulateCommands();
                    int recordsAffected = ad.Fill(dataSet);
                    if (recordsAffected > 0)
                    {
                        row = dataSet.SizeOption[0];
                        SizeOptionMapping(row, sourceRow);
                    }
                    else
                    {
                        row = dataSet.SizeOption.NewSizeOptionRow();
                        SizeOptionMapping(row, sourceRow);
                        dataSet.SizeOption.AddSizeOptionRow(row);
                    }
                    recordsAffected = ad.Update(dataSet);

                    if (recordsAffected < 1)
                        throw new DataAccessException("Synchronizing SizeOption Error");
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

        public void syncInvoice(DomainShipmentSyncDef domainDef, int shipmentId, bool isInitial, ArrayList amendmentList)
        {
            if (domainDef == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                InvoiceDs dataSet = new InvoiceDs();
                InvoiceDs.InvoiceRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceByKey");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.Invoice[0];
                    decimal originalAmt = row.ChoiceOrderTotalShippedAmt;
                    decimal originalSupplierAmt = row.ChoiceOrderTotalShippedSupplierGmtAmt;
                    decimal originalCommissionAmt = row.ChoiceOrderNSLCommissionAmt;
                    InvoiceMapping(domainDef, row);
                    row.ModifiedBy = 99999;
                    row.ModifiedOn = DateTime.Now;
                    if (row.ChoiceOrderTotalShippedAmt != originalAmt)
                        amendmentList.Add(getNewActionHistoryDef(row.ShipmentId, 0, AmendmentType.CHOICE_ORDER_TOTAL_SHIPPED_AMT, originalAmt.ToString(), row.ChoiceOrderTotalShippedAmt.ToString()));
                    if (row.ChoiceOrderTotalShippedSupplierGmtAmt != originalSupplierAmt)
                        amendmentList.Add(getNewActionHistoryDef(row.ShipmentId, 0, AmendmentType.CHOICE_ORDER_TOTAL_SHIPPED_SUPPLIER_GMT_AMT, originalSupplierAmt.ToString(), row.ChoiceOrderTotalShippedSupplierGmtAmt.ToString()));
                    if (row.ChoiceOrderNSLCommissionAmt != originalCommissionAmt)
                        amendmentList.Add(getNewActionHistoryDef(row.ShipmentId, 0, AmendmentType.CHOICE_ORDER_NSL_COMM_AMT, originalCommissionAmt.ToString(), row.ChoiceOrderNSLCommissionAmt.ToString()));
                }
                else
                {
                    AccountWorker.Instance.createDailySunInterfaceEntries(shipmentId, 0);
                    row = dataSet.Invoice.NewInvoiceRow();
                    row.ShipmentId = shipmentId;
                    InvoiceMapping(domainDef, row);
                    row.CreatedBy = 99999;
                    row.CreatedOn = DateTime.Now;
                    dataSet.Invoice.AddInvoiceRow(row);
                }
                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Synchronizing Invoice Error");

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

        public void syncAdvancePayment(NssAdvancePaymentDs ds, ArrayList amendmentList, bool isRejected)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                AdvancePaymentDs dataSet = null;
                AdvancePaymentDs.AdvancePaymentRow row = null;

                foreach (NssAdvancePaymentDs.AdvancePaymentRow sourceRow in ds.AdvancePayment.Rows)
                {
                    dataSet = new AdvancePaymentDs();
                    row = null;
                    IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentApt", "GetAdvancePaymentByKey");
                    ad.SelectCommand.Parameters["@PaymentId"].Value = sourceRow.PaymentId;
                    ad.PopulateCommands();
                    int recordsAffected = ad.Fill(dataSet);
                    if (recordsAffected > 0)
                    {
                        row = dataSet.AdvancePayment[0];
                        if (row.PaymentNo != sourceRow.PaymentNo)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "PAYMENT NO: " + row.PaymentNo + " --> " + sourceRow.PaymentNo));
                        }
                        if (row.OfficeId != sourceRow.OfficeId)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "OFFICE: " + generalWorker.getOfficeRefByKey(row.OfficeId).OfficeCode + " --> " + generalWorker.getOfficeRefByKey(sourceRow.OfficeId).OfficeCode));
                        }
                        if (row.VendorId != sourceRow.VendorId)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "VENDOR: " + VendorWorker.Instance.getVendorByKey(row.VendorId).Name + " --> " + VendorWorker.Instance.getVendorByKey(sourceRow.VendorId).Name));
                        }
                        string target = "N/A";
                        string source = "N/A";
                        if (!row.IsPaymentDateNull())
                        {
                            target = String.Format("{0:MM/dd/yyyy}", row.PaymentDate);
                        }
                        if (!sourceRow.IsPaymentDateNull())
                        {
                            source = String.Format("{0:MM/dd/yyyy}", sourceRow.PaymentDate);
                        }
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "PAYMENT DATE: " + target + " --> " + source));
                        }
                        target = "N/A";
                        source = "N/A";
                        if (!row.IsPayableToNull())
                        {
                            target = row.PayableTo;
                        }
                        source = sourceRow.PayableTo;
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "PAYABLE TO: " + target + " --> " + source));
                        }
                        if (row.CurrencyId != sourceRow.CurrencyId)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "CURRENCY: " + generalWorker.getCurrencyByKey(row.CurrencyId).CurrencyCode + " --> " + generalWorker.getCurrencyByKey(sourceRow.CurrencyId).CurrencyCode));
                        }
                        if (row.TotalAmt != sourceRow.TotalAmt)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "TOTAL AMT: " + row.TotalAmt + " --> " + sourceRow.TotalAmt));
                        }
                        if (row.InterestChargedAmt != sourceRow.InterestChargedAmt)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "INTEREST CHARGED AMT: " + Math.Round(row.InterestChargedAmt, 2) + " --> " + Math.Round(sourceRow.InterestChargedAmt, 2)));
                        }
                        source = "N/A";
                        target = "N/A";
                        if (!row.IsUploadedByNull())
                        {
                            target = CommonUtil.getUserByKey(row.UploadedBy).DisplayName;
                        }
                        if (!sourceRow.IsUploadedByNull())
                        {
                            source = CommonUtil.getUserByKey(sourceRow.UploadedBy).DisplayName;
                        }
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "UPLOADER: " + target + " --> " + source));
                        }
                        source = "N/A";
                        target = "N/A";
                        if (!row.IsApprovedByNull())
                        {
                            target = CommonUtil.getUserByKey(row.ApprovedBy).DisplayName;
                        }
                        if (!sourceRow.IsApprovedByNull())
                        {
                            source = CommonUtil.getUserByKey(sourceRow.ApprovedBy).DisplayName;
                        }
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "APPROVER: " + target + " --> " + source));
                        }
                        source = "N/A";
                        target = "N/A";
                        if (!row.IsRejectByNull())
                        {
                            target = CommonUtil.getUserByKey(row.RejectBy).DisplayName;
                        }
                        if (!sourceRow.IsRejectByNull())
                        {
                            source = CommonUtil.getUserByKey(sourceRow.RejectBy).DisplayName;
                        }
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "REJECTOR: " + target + " --> " + source));
                        }
                        source = "N/A";
                        target = "N/A";
                        if (!row.IsRejectReasonIdNull()) 
                        {
                            target = getAdvancePaymentRejectReasonNameByKey(row.RejectReasonId);
                        }
                        if (!sourceRow.IsRejectReasonIdNull())
                        {
                            source = getAdvancePaymentRejectReasonNameByKey(sourceRow.RejectReasonId);
                        }
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "REJECT REASON: " + target + " --> " + source));
                        }
                        target = "N/A";
                        source = "N/A";
                        if (!row.IsRemarkNull()) 
                        {
                            target = row.Remark;
                        }
                        if (!sourceRow.IsRemarkNull()) 
                        {
                            source = sourceRow.Remark;
                        }
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "PAYMENT REMARK: " + target + " --> " + source));
                        }
                        if (row.WorkflowStatusId != sourceRow.WorkflowStatusId)
                        {
                            target = AdvancePaymentWFS.getType(row.WorkflowStatusId).Name;
                            source = AdvancePaymentWFS.getType(sourceRow.WorkflowStatusId).Name;
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "WORKFLOW STATUS: " + target + " --> " + source));
                        }
                        if (row.Status != sourceRow.Status)
                        {
                            string before = "On";
                            string after = "On";
                            if (row.Status == 0) { before = "Off"; }
                            if (sourceRow.Status == 0) { after = "Off"; }
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "PAYMENT STATUS: " + before + " --> " + after));
                        }
                        target = "N/A";
                        source = "N/A";
                        if (!row.IsInitiatedByNull())
                        {
                            target = CommonUtil.getUserByKey(row.InitiatedBy).DisplayName;
                        }
                        source = CommonUtil.getUserByKey(sourceRow.InitiatedBy).DisplayName;
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "INITIATOR: " + target + " --> " + source));
                        }

                        target = "N/A";
                        source = "N/A";
                        if (!row.IsIsC19Null())
                        {
                            target = row.IsC19 == 1 ? "True" : "False";
                        }
                        if (!sourceRow.IsIsC19Null())
                        {
                            source = sourceRow.IsC19 == 1 ? "True" : "False";
                        }
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "IS C19: " + target + " --> " + source));
                        }

                        target = "N/A";
                        source = "N/A";
                        if (!row.IsFLRefNoNull())
                        {
                            target = row.FLRefNo;
                        }
                        if (!sourceRow.IsNSLRefNoNull())
                        {
                            source = sourceRow.NSLRefNo;
                        }
                        if (source != target)
                        {
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "FL REF NO.: " + target + " --> " + source));
                        }

                        AdvancePaymentMapping(row, sourceRow);
                    }
                    else
                    {
                        row = dataSet.AdvancePayment.NewAdvancePaymentRow();
                        row.IsInterfaced = 0;
                        row.InterestRate = 0;
                        AdvancePaymentMapping(row, sourceRow);
                        dataSet.AdvancePayment.AddAdvancePaymentRow(row);
                        amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "INITIAL PAYMENT DATA TRANSFER"));
                    }

                    if (isRejected)
                        row.Status = 0;

                    recordsAffected = ad.Update(dataSet);

                    if (recordsAffected < 1)
                        throw new DataAccessException("Synchronizing Advance Payment Error");
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

        public void syncAdvancePaymentOrderDetail(NssAdvancePaymentOrderDetailDs ds, ArrayList amendmentList, int paymentId, bool isRejected)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                AdvancePaymentOrderDetailDs dataSet = null;
                AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow row = null;
                AdvancePaymentDef paymentDef = advancePaymentWorker.getAdvancePaymentByKey(paymentId);

                if (isRejected)
                {
                    List<AdvancePaymentOrderDetailDef> orderDetailList = advancePaymentWorker.getAdvancePaymentOrderDetailList(paymentId);
                    List<AdvancePaymentOrderDetailDef> updatedList = new List<AdvancePaymentOrderDetailDef>();
                    foreach (AdvancePaymentOrderDetailDef detailDef in orderDetailList)
                    {
                        if (detailDef.ActualValue != 0)
                        {
                            detailDef.ActualValue = 0;
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(paymentId, "ACTUAL DEDUCT AMT: " + detailDef.ActualValue + " --> 0  [REJECTED]"));
                            updatedList.Add(detailDef);

                            ArrayList existShipmentDeduction = ShippingWorker.Instance.getShipmentDeductionByLogicalKey(detailDef.ShipmentId, PaymentDeductionType.FABRIC_ADVANCE.Id, paymentDef.PaymentNo);
                            ShipmentDeductionDef deduction = null;
                            foreach (ShipmentDeductionDef item in existShipmentDeduction)
                            {
                                decimal originalAmt = item.Amount;
                                deduction = item;
                                deduction.Amount = 0;
                                if (originalAmt != deduction.Amount)
                                    amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(paymentId, "EXISTING SHIPMENT DEDUCTION: [SHIPMENT ID:" + deduction.ShipmentId + "] [AMT:" + originalAmt + "-->" + deduction.Amount + "]" + (isRejected ? " [REJECTED]" : "")));
                            }
                            ShippingWorker.Instance.updateShipmentDeduction(deduction, 99999);
                        }
                    }

                    if (updatedList.Count > 0)
                        advancePaymentWorker.updateAdvancePaymentOrderDetailList(paymentId, updatedList, 99999);
                }
                else
                {
                    foreach (NssAdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow sourceRow in ds.AdvancePaymentOrderDetail.Rows)
                    {
                        dataSet = new AdvancePaymentOrderDetailDs();
                        row = null;
                        IDataSetAdapter ad = getDataSetAdapter("AdvancePaymentOrderDetailApt", "GetAdvancePaymentOrderDetailByKey");
                        ad.SelectCommand.Parameters["@PaymentId"].Value = sourceRow.PaymentId;
                        ad.SelectCommand.Parameters["@ShipmentId"].Value = sourceRow.ShipmentId;
                        ad.PopulateCommands();
                        int recordsAffected = ad.Fill(dataSet);
                        if (recordsAffected > 0)
                        {
                            row = dataSet.AdvancePaymentOrderDetail[0];
                            if (row.ShipmentId != sourceRow.ShipmentId)
                            {
                                amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "SHIPMENT: " + row.ShipmentId + " --> " + sourceRow.ShipmentId));
                            }
                            if (row.ExpectedDeductAmt != sourceRow.ExpectedDeductAmt)
                            {
                                amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "EXPECTED DEDUCT AMT: " + row.ExpectedDeductAmt + " --> " + sourceRow.ExpectedDeductAmt));
                            }
                            /*
                            if (row.ActualDeductAmt != sourceRow.ActualDeductAmt)
                            {
                                amendmentList.Add(NewAdvancePaymentActionHistory(sourceRow.PaymentId, "ACTUAL DEDUCT AMT: " + row.ActualDeductAmt + " --> " + sourceRow.ActualDeductAmt));
                            }
                            */
                            string target = "N/A";
                            string source = "N/A";
                            if (!row.IsSettlementDateNull())
                            {
                                target = String.Format("{0:MM/dd/yyyy}", row.SettlementDate);
                            }
                            if (!sourceRow.IsSettlementDateNull())
                            {
                                source = String.Format("{0:MM/dd/yyyy}", sourceRow.SettlementDate);
                            }
                            if (source != target)
                            {
                                amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "SETTLEMENT DATE: " + target + " --> " + source));
                            }
                            target = "N/A";
                            source = "N/A";
                            if (!row.IsRemarkNull())
                            {
                                target = row.Remark;
                            }
                            if (!sourceRow.IsRemarkNull())
                            {
                                source = sourceRow.Remark;
                            }
                            if (source != target)
                            {
                                amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "PAYMENT ORDER DETAIL (" + sourceRow.ShipmentId + ") REMARK: " + target + " --> " + source));
                            }
                            if (row.Status != sourceRow.Status)
                            {
                                string before = "On";
                                string after = "On";
                                if (row.Status == 0) { before = "Off"; }
                                if (sourceRow.Status == 0) { after = "Off"; }
                                amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "PAYMENT ORDER (" + sourceRow.ShipmentId + ") STATUS: " + before + " --> " + after));
                            }

                            AdvancePaymentOrderDetailMapping(row, sourceRow);
                            if (isRejected)
                            {
                                amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "ACTUAL DEDUCT AMT: " + row.ActualDeductAmt + " --> 0  [REJECTED]"));
                                row.ActualDeductAmt = 0;
                            }
                        }
                        else
                        {
                            row = dataSet.AdvancePaymentOrderDetail.NewAdvancePaymentOrderDetailRow();
                            row.ActualDeductAmt = sourceRow.ExpectedDeductAmt;
                            AdvancePaymentOrderDetailMapping(row, sourceRow);
                            if (isRejected)
                            {
                                amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "ACTUAL DEDUCT AMT: " + row.ActualDeductAmt + " --> 0  [REJECTED]"));
                                row.ActualDeductAmt = 0;
                            }
                            dataSet.AdvancePaymentOrderDetail.AddAdvancePaymentOrderDetailRow(row);
                        }
                        recordsAffected = ad.Update(dataSet);

                        if (recordsAffected < 1)
                            throw new DataAccessException("Synchronizing Advance Payment Order Detail Error");
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

        public void syncLGPayment(NssLGPaymentDs ds)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                LetterOfGuaranteeDs dataSet = null;
                LetterOfGuaranteeDs.LetterOfGuaranteeRow row = null;
                foreach (NssLGPaymentDs.LGPaymentRow sourceRow in ds.LGPayment.Rows)
                {
                    dataSet = new LetterOfGuaranteeDs();
                    IDataSetAdapter ad = getDataSetAdapter("LetterOfGuaranteeApt", "GetLetterOfGuaranteeByKey");
                    ad.SelectCommand.Parameters["@LGId"].Value = sourceRow.PaymentId;
                    ad.PopulateCommands();

                    int recordsAffected = ad.Fill(dataSet);

                    if (recordsAffected > 0)
                    {
                        row = dataSet.LetterOfGuarantee[0];
                        this.LetterOfGuaranteeMapping(row, sourceRow);
                    }
                    else
                    {
                        row = dataSet.LetterOfGuarantee.NewLetterOfGuaranteeRow();
                        this.LetterOfGuaranteeMapping(row, sourceRow);
                        dataSet.LetterOfGuarantee.AddLetterOfGuaranteeRow(row);
                    }
                    recordsAffected = ad.Update(dataSet);
                    if (recordsAffected < 1)
                        throw new DataAccessException("Update LetterOfGuarantee ERROR");
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

        public void syncLGPaymentOrderDetail(NssLGPaymentOrderDetailDs ds)
        {
            if (ds == null)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                LGDetailDs dataSet = null;
                LGDetailDs.LGDetailRow row = null;
                foreach (NssLGPaymentOrderDetailDs.LGPaymentOrderDetailRow sourceRow in ds.LGPaymentOrderDetail.Rows)
                {
                    dataSet = new LGDetailDs();
                    IDataSetAdapter ad = getDataSetAdapter("LGDetailApt", "GetLGDetail");
                    ad.SelectCommand.Parameters["@ShipmentId"].Value = sourceRow.ShipmentId;
                    ad.SelectCommand.Parameters["@SplitShipmentId"].Value = 0;
                    ad.PopulateCommands();

                    int recordsAffected = ad.Fill(dataSet);
                    if (recordsAffected > 0)
                    {
                        row = dataSet.LGDetail[0];
                        this.LGDetailMapping(row, sourceRow);
                    }
                    else
                    {
                        row = dataSet.LGDetail.NewLGDetailRow();
                        this.LGDetailMapping(row, sourceRow);
                        dataSet.LGDetail.AddLGDetailRow(row);
                    }
                    recordsAffected = ad.Update(dataSet);
                    if (recordsAffected < 1)
                        throw new DataAccessException("Update LGDetail ERROR");
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

        #endregion

        #region Synchronize Data to NSS functions

        public void SyncNssShipment(int shipmentId)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(shipmentId);
            InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(shipmentId);
            int otherCurrencyId = 0;
            decimal otherCurrencyRate = 1;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                NssShipmentDs dataSet = new NssShipmentDs();
                NssShipmentDs.ShipmentRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("SyncShipmentApt", "GetShipment");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.Shipment[0];
                    if (!row.IsOtherCurrencyIdNull())
                        otherCurrencyId = row.OtherCurrencyId;
                    if (invoiceDef.QCCRemark.Trim() != String.Empty)
                        row.QccRemark = invoiceDef.QCCRemark;
                    else
                        row.SetQccRemarkNull();

                    if (shipmentDef.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
                    {
                        row.WorkflowStatusId = shipmentDef.WorkflowStatus.Id;
                        row.InvoicePrefix = invoiceDef.InvoicePrefix;
                        row.InvoiceSeq = invoiceDef.InvoiceSeqNo;
                        row.InvoiceYear = invoiceDef.InvoiceYear;
                        row.ActualAtWarehouseDate = invoiceDef.InvoiceDate;
                        row.InvoiceExchangeRate = invoiceDef.InvoiceSellExchangeRate;
                        row.USExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, invoiceDef.InvoiceDate);
                        if (otherCurrencyId != 0)
                        {
                            row.OtherCcyToBuyCcyRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, otherCurrencyId, invoiceDef.InvoiceDate) / invoiceDef.InvoiceSellExchangeRate;
                            otherCurrencyRate = row.OtherCcyToBuyCcyRate;
                        }
                        if (!row.EditLock) row.EditLock = shipmentDef.EditLock;
                    }
                    else if (row.WorkflowStatusId == ContractWFS.INVOICED.Id)
                    {
                        row.WorkflowStatusId = ContractWFS.PO_PRINTED.Id;
                        row.SetInvoicePrefixNull();
                        row.SetInvoiceSeqNull();
                        row.SetInvoiceYearNull();
                        row.SetActualAtWarehouseDateNull();
                        row.InvoiceExchangeRate = 0;
                        row.USExchangeRate = 0;
                        row.OtherCcyToBuyCcyRate = 0;
                        row.EditLock = shipmentDef.EditLock;
                    }
                    row.ActualAFTradingCostInUSD = shipmentDef.TradingAirFreightActualCost;
                    row.TotalShippedAmt = shipmentDef.TotalShippedAmount;
                    row.TotalShippedDutyCost = shipmentDef.TotalShippedDutyCost;
                    row.TotalShippedFreightCost = shipmentDef.TotalShippedFreightCost;
                    row.TotalShippedOPAUpcharge = shipmentDef.TotalShippedOPAUpcharge;
                    row.TotalShippedOtherCost = shipmentDef.TotalShippedOtherCost;
                    row.TotalShippedQty = shipmentDef.TotalShippedQuantity;
                    row.TotalShippedSupplierGmtAmt = 0;
                    row.TotalShippedNetFOBAmt = 0;
                    if (otherCurrencyId > 0)
                    {
                        row.TotalNetFOBAmt = 0;
                        row.TotalSupplierGmtAmt = 0;
                        row.TotalPONetFOBAmt = 0;
                    }

                    NssShipmentDetailDs ds = getSyncShipmentDetail(shipmentDef.ShipmentId);

                    ArrayList shipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(shipmentId);
                    foreach (ShipmentDetailDef detailDef in shipmentDetailList)
                    {
                        foreach (NssShipmentDetailDs.ShipmentDetailRow r in ds.ShipmentDetail)
                        {
                            if (r.SizeOptionId == detailDef.SizeOption.SizeOptionId && r.Status == 1)
                            {
                                if (otherCurrencyId > 0)
                                {
                                    row.TotalShippedSupplierGmtAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.ShippedQuantity;
                                    row.TotalShippedNetFOBAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.ShippedQuantity;
                                    row.TotalNetFOBAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.OrderQuantity;
                                    row.TotalSupplierGmtAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.OrderQuantity;
                                    row.TotalPONetFOBAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.POQuantity;
                                }
                                else
                                {
                                    row.TotalShippedSupplierGmtAmt += r.SupplierGmtPrice * detailDef.ShippedQuantity;
                                    row.TotalShippedNetFOBAmt += r.NetFOBPrice * detailDef.ShippedQuantity;
                                }
                                break;
                            }
                        }
                        this.SyncNssShipmentDetail(detailDef, otherCurrencyId, otherCurrencyRate);
                    }

                    recordsAffected = ad.Update(dataSet);

                    if (recordsAffected < 1)
                        throw new DataAccessException("Synchronization To NSS [Shipment] Error");

                    if (shipmentDef.SplitCount > 0)
                    {
                        ArrayList splitShipmentList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(shipmentId);
                        foreach (SplitShipmentDef splitShipmentDef in splitShipmentList)
                        {
                            this.SyncNssSplitShipment(splitShipmentDef, otherCurrencyId, otherCurrencyRate);
                            ArrayList splitShipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentDetailBySplitShipmentId(splitShipmentDef.SplitShipmentId);

                            foreach (SplitShipmentDetailDef splitShipmentDetailDef in splitShipmentDetailList)
                            {
                                this.SyncNssSplitShipmentDetail(splitShipmentDetailDef, otherCurrencyId, otherCurrencyRate);
                            }

                        }
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

        private void SyncNssShipmentDetail(ShipmentDetailDef shipmentDetailDef, int otherCurrencyId, decimal otherCurrencyRate)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(shipmentDetailDef.ShipmentId);

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                NssShipmentDetailDs dataSet = new NssShipmentDetailDs();
                NssShipmentDetailDs.ShipmentDetailRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("SyncShipmentDetailApt", "GetShipmentDetailByKey");
                ad.SelectCommand.Parameters["@ShipmentDetailId"].Value = shipmentDetailDef.ShipmentDetailId.ToString();
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);
                row = dataSet.ShipmentDetail[0];

                row.ShippingGSPFormTypeId = shipmentDetailDef.ShippingGSPFormTypeId;

                if (shipmentDef.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
                {
                    if (otherCurrencyId > 0)
                    {
                        row.NetFOBPrice = otherCurrencyRate * row.SupplierGmtPriceOtherCcy;
                        row.SupplierGmtPrice = otherCurrencyRate * row.SupplierGmtPriceOtherCcy;
                    }
                    row.ShippedQty = shipmentDetailDef.ShippedQuantity;
                }
                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Synchronization To NSS [Shipment Detail] Error");
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

        private void SyncNssSplitShipment(SplitShipmentDef splitShipmentDef, int otherCurrencyId, decimal otherCurrencyRate)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                NssSplitShipmentDs dataSet = new NssSplitShipmentDs();
                NssSplitShipmentDs.SplitShipmentRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("SyncSplitShipmentApt", "GetSplitShipmentByKey");
                ad.SelectCommand.Parameters["@SplitShipmentId"].Value = splitShipmentDef.SplitShipmentId.ToString();
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);
                row = dataSet.SplitShipment[0];

                row.TotalShippedAmt = splitShipmentDef.TotalShippedAmount;
                row.TotalShippedOPAUpcharge = splitShipmentDef.TotalShippedOPAUpcharge;
                row.TotalShippedQty = splitShipmentDef.TotalShippedQuantity;
                row.TotalShippedSupplierGmtAmt = 0;
                row.TotalShippedNetFOBAmt = 0;
                if (otherCurrencyId > 0)
                {
                    row.TotalNetFOBAmt = 0;
                    row.TotalSupplierGmtAmt = 0;
                    row.TotalPONetFOBAmt = 0;
                }

                NssSplitShipmentDetailDs ds = getSyncSplitShipmentDetail(row.ShipmentId);

                ArrayList splitShipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentDetailBySplitShipmentId(splitShipmentDef.SplitShipmentId);
                foreach (SplitShipmentDetailDef detailDef in splitShipmentDetailList)
                {
                    foreach (NssSplitShipmentDetailDs.SplitShipmentDetailRow r in ds.SplitShipmentDetail)
                    {
                        if (r.SizeOptionId == detailDef.SizeOption.SizeOptionId && r.Status == 1)
                        {
                            if (otherCurrencyId > 0)
                            {
                                row.TotalShippedSupplierGmtAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.ShippedQuantity;
                                row.TotalShippedNetFOBAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.ShippedQuantity;
                                row.TotalNetFOBAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.OrderQuantity;
                                row.TotalSupplierGmtAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.OrderQuantity;
                                row.TotalPONetFOBAmt += Math.Round(otherCurrencyRate * r.SupplierGmtPriceOtherCcy, 2, MidpointRounding.AwayFromZero) * detailDef.POQuantity;
                            }
                            else
                            {
                                row.TotalShippedSupplierGmtAmt += r.SupplierGmtPrice * detailDef.ShippedQuantity;
                                row.TotalShippedNetFOBAmt += r.NetFOBPrice * detailDef.ShippedQuantity;
                            }
                            break;
                        }
                    }
                }

                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Synchronization To NSS [Split Shipment] Error");
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

        private void SyncNssSplitShipmentDetail(SplitShipmentDetailDef splitShipmentDetailDef, int otherCurrencyId, decimal otherCurrencyRate)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                NssSplitShipmentDetailDs dataSet = new NssSplitShipmentDetailDs();
                NssSplitShipmentDetailDs.SplitShipmentDetailRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("SyncSplitShipmentDetailApt", "GetSplitShipmentDetailByKey");
                ad.SelectCommand.Parameters["@SplitShipmentDetailId"].Value = splitShipmentDetailDef.SplitShipmentDetailId.ToString();
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);
                row = dataSet.SplitShipmentDetail[0];

                if (otherCurrencyId > 0)
                {
                    row.NetFOBPrice = otherCurrencyRate * row.SupplierGmtPriceOtherCcy;
                    row.SupplierGmtPrice = otherCurrencyRate * row.SupplierGmtPriceOtherCcy;
                }
                row.ShippedQty = splitShipmentDetailDef.ShippedQuantity;
                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Synchronization To NSS [Split Shipment Detail] Error");
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

        #endregion

        #region Mapping Functions


        public bool isBlankTeeOrder(NssContractDs.ContractRow contractRow, NssShipmentDs.ShipmentRow shipmentRow, NssProductDs.ProductRow productRow)
        {
            string itemNo = productRow.ItemNo.ToUpper();
            string contractNo = contractRow.ContractNo.ToUpper();
            bool returnVal = false;

            if (itemNo.Contains("BLANK-TEE")
                || contractNo.StartsWith("XX")
                || (contractNo.StartsWith("XF") && itemNo.Contains("BLANK"))
                || (itemNo.Contains("BLANK") && itemNo.Contains("TEE"))
                || (itemNo.Contains("BLANK") && itemNo.IndexOf(" T", itemNo.IndexOf("BLANK") + 5) != -1)
            )
                returnVal = true;
            return returnVal;
        }

        private void InvoiceMapping(DomainShipmentSyncDef domainDef, InvoiceDs.InvoiceRow row)
        {
            NssContractDs.ContractRow contractRow = domainDef.ContractDataSet.Contract[0];
            NssShipmentDs.ShipmentRow shipmentRow = domainDef.ShipmentDataSet.Shipment[0];
            NssProductDs.ProductRow productRow = domainDef.ProductDataSet.Product[0];

            decimal courierCharge = 0;
            if (shipmentRow.IsMockShopSample == 1 || shipmentRow.IsStudioSample == 1)
            {
                /* 2016-09-20 add new product teams 
                if ((contractRow.ProductTeamId != 51074 &&
                     contractRow.ProductTeamId != 51076 &&
                     contractRow.ProductTeamId != 51077 &&
                     contractRow.ProductTeamId != 51078 &&
                     contractRow.ProductTeamId != 51079 &&
                     contractRow.ProductTeamId != 51080 &&
                     contractRow.ProductTeamId != 51081 &&
                     contractRow.ProductTeamId != 51082 &&
                     contractRow.ProductTeamId != 51083 &&
                     contractRow.ProductTeamId != 51084 &&
                     contractRow.ProductTeamId != 51088 &&
                     contractRow.ProductTeamId != 51175 &&
                     contractRow.ProductTeamId != 51176 &&
                     contractRow.ProductTeamId != 51177 &&
                     contractRow.ProductTeamId != 51178 &&
                     contractRow.ProductTeamId != 51182)) //NCGL, NCHA, NCHB, NCHE, NCHF, NCHL, NCHP, NCHS, HCHU, NCHZ, NCMJW, NCHDF, NCHKF, NCHLG, NCHG, NCHN
                */
                if ((contractRow.ProductTeamId != 51074 &&
                     contractRow.ProductTeamId != 51076 &&
                     contractRow.ProductTeamId != 51077 &&
                     contractRow.ProductTeamId != 51078 &&
                     contractRow.ProductTeamId != 51079 &&
                     contractRow.ProductTeamId != 51080 &&
                     contractRow.ProductTeamId != 51081 &&
                     contractRow.ProductTeamId != 51082 &&
                     contractRow.ProductTeamId != 51083 &&
                     contractRow.ProductTeamId != 51084 &&
                     contractRow.ProductTeamId != 51088 &&
                     contractRow.ProductTeamId != 51175 &&
                     contractRow.ProductTeamId != 51176 &&
                     contractRow.ProductTeamId != 51177 &&
                     contractRow.ProductTeamId != 51178 &&
                     contractRow.ProductTeamId != 51182 &&
                     
                     contractRow.ProductTeamId != 81112 &&
                     contractRow.ProductTeamId != 81113 &&
                     contractRow.ProductTeamId != 81114 &&
                     contractRow.ProductTeamId != 81116 &&
                     contractRow.ProductTeamId != 81117 &&
                     contractRow.ProductTeamId != 81118 &&
                     contractRow.ProductTeamId != 81119 &&
                     contractRow.ProductTeamId != 81121 &&
                     contractRow.ProductTeamId != 81122 &&
                     contractRow.ProductTeamId != 81124))
                     //&& contractRow.ProductTeamId != 81134

                /*
                NCGL, NCHA, NCHB, NCHE, NCHF, NCHL, NCHP, NCHS, HCHU, NCHZ, NCMJW, NCHDF, NCHKF, NCHLG, NCHG, NCHN
                HACCS, HBATH, HBED, HDFURN, HDTEXT, HKFURN, HKITC, HTOWEL, HUFURN, HUPHY, NCWACC
                */
                {
                    StandardMSCourierChargeDef chargeDef = commonWorker.getStandardMSCourierCharge(contractRow.OfficeId, contractRow.DeptId, shipmentRow.SellCurrencyId);
                    if (chargeDef != null)
                        courierCharge = chargeDef.ChargeRate;
                }
            }

            if (row.RowState == DataRowState.Detached)
            {
                row.NSLCommissionAmt = 0;
                row.InvoiceSellExchangeRate = 0;
                row.InvoiceBuyExchangeRate = 0;
                row.ARExchangeRate = 0;
                row.ARAmt = 0;
                row.APExchangeRate = 0;
                row.APAmt = 0;
                row.LCAmt = 0;
                row.IsLCPaymentChecked = GeneralCriteria.FALSE;
                row.ExportLicenceFee = 0;
                row.QuotaCharge = 0;
                row.IsSelfBilledOrder = false;
                row.IsSyncToFactory = false;
                row.BookingQty = 0;
                row.ImportDutyActualAmt = 0;
                row.IsInputVATChecked = GeneralCriteria.FALSE;
                row.InputVATActualAmt = 0;
                row.IsInputVATChecked = GeneralCriteria.FALSE;
                row.OutputVATActualAmt = 0;
                row.IsOutputVATChecked = GeneralCriteria.FALSE;
                row.DFDocumentationCharge = 0;
                row.DFTransportationCharge = 0;
                row.PiecesPerDeliveryUnit = 0;
                row.SalesScanAmt = 0;
                row.PurchaseScanAmt = 0;
                row.IsILSQtyUploadAllowed = true;
                row.ImportDutyCurrencyId = CurrencyId.RMB.Id;
                row.InputVATCurrencyId = CurrencyId.RMB.Id;
                row.OutputVATCurrencyId = CurrencyId.RMB.Id;
                row.IsImportDutyChecked = 0;
                row.IsInputVATChecked = 0;
                row.IsOutputVATChecked = 0;
                row.IsReadyToSendInvoice = false;
                row.NSLCommissionSettlementExchangeRate = 0;
                row.NSLCommissionSettlementAmt = 0;
                row.ChoiceOrderTotalShippedAmt = 0;
                row.ChoiceOrderTotalShippedSupplierGmtAmt = 0;
                row.ChoiceOrderNSLCommissionAmt = 0;
                row.IsUploadDMSDocument = false;
                row.ShippingCheckedTotalNetAmount = 0;
            }

            if (shipmentRow.WorkflowStatusId != ContractWFS.INVOICED.Id)
            {
                if (!productRow.IsDesc1Null())
                    row.ItemDesc1 = productRow.Desc1;
                else
                    row.SetItemDesc1Null();
                if (!productRow.IsDesc2Null())
                    row.ItemDesc2 = productRow.Desc2;
                else
                    row.SetItemDesc2Null();
                if (!productRow.IsDesc3Null())
                    row.ItemDesc3 = productRow.Desc3;
                else
                    row.SetItemDesc3Null();
                if (!productRow.IsDesc4Null())
                    row.ItemDesc4 = productRow.Desc4;
                else
                    row.SetItemDesc4Null();
                if (!productRow.IsDesc5Null())
                    row.ItemDesc5 = productRow.Desc5;
                else
                    row.SetItemDesc5Null();
                if (!productRow.IsColourNull())
                    row.ItemColour = productRow.Colour;
                else
                    row.SetItemColourNull();
                row.PackingMethodId = contractRow.PackingMethodId;
                if (!shipmentRow.IsCustomerDestinationIdNull())
                    row.CustomerDestinationId = shipmentRow.CustomerDestinationId;
                else
                    row.SetCustomerDestinationIdNull();
                if (!shipmentRow.IsShipFromCountryIdNull())
                    row.ShipFromCountryId = shipmentRow.ShipFromCountryId;
                else
                    row.SetShipFromCountryIdNull();
                row.CourierChargeToNUK = courierCharge;
            }

            int countryOfOriginId = 0;
            if (!shipmentRow.IsCountryOfOriginIdNull())
                countryOfOriginId = shipmentRow.CountryOfOriginId;

            CustomerDef custDef = commonWorker.getCustomerByKey(contractRow.CustomerId);

            row.IsSelfBilledOrder = true;
            if ((!custDef.IsSelfBilling && contractRow.CustomerId != CustomerDef.Id.ns_led.GetHashCode() && contractRow.CustomerId != CustomerDef.Id.manu_led.GetHashCode()) ||
                ((contractRow.CustomerId == CustomerDef.Id.ns_led.GetHashCode() || contractRow.CustomerId == CustomerDef.Id.manu_led.GetHashCode()) && !commonWorker.getNSLedSelfBilledSupplierCodeList().Contains(contractRow.UKSupplierCode)) ||
                contractRow.ContractId == 822586 || // "PC0056725" requested by Shiraz 2020-11-25
                contractRow.OfficeId == OfficeId.UK.Id ||
                shipmentRow.IsPressSample == 1 ||
                shipmentRow.IsMockShopSample == 1 ||
                shipmentRow.IsStudioSample == 1 ||
                this.isBlankTeeOrder(contractRow, shipmentRow, productRow) ||
                (!shipmentRow.IsSpecialOrderTypeIdNull() && shipmentRow.SpecialOrderTypeId != 0) ||
                //(CustomerDestinationDef.isUTurnOrder(shipmentRow.CustomerDestinationId) ||
                shipmentRow.TermOfPurchaseId == TermOfPurchaseRef.Id.FOB_UT.GetHashCode() || 
                (countryOfOriginId == CountryOfOriginRef.eCountryId.Morocco.GetHashCode())) // && contractRow.TradingAgencyId == TradingAgencyId.NSLVMOT.AgencyId && contractRow.OfficeId == OfficeId.TR.Id 
                row.IsSelfBilledOrder = false;

            /*
            if (shipmentRow.SpecialOrderTypeId = 1)
            if (contractRow.ContractNo.Length > 2 && contractRow.ContractNo.Substring(0, 2).ToUpper() == "XX" && productRow.ItemNo.ToUpper().IndexOf("BLANK") != -1)
                row.IsSelfBilledOrder = false;
            */

            if (!contractRow.IsUKSupplierCodeNull() && (contractRow.UKSupplierCode == "D83110" || contractRow.UKSupplierCode == "D82650"))
                row.IsSelfBilledOrder = true;

            if (!shipmentRow.IsDMSUploadDateNull())
                row.IsUploadDMSDocument = true;

            if (row.IsAccountDocReceiptDateNull() && !shipmentRow.IsDMSUploadDateNull() && (contractRow.OfficeId == OfficeId.HK.Id || contractRow.OfficeId == OfficeId.DG.Id || contractRow.OfficeId == OfficeId.SL.Id) && shipmentRow.WorkflowStatusId == ContractWFS.INVOICED.Id && shipmentRow.ActualAtWarehouseDate >= new DateTime(2011, 4, 3) && !row.IsShippingDocCheckedOnNull())
            {
                row.AccountDocReceiptDate = shipmentRow.DMSUploadDate;

                ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(shipmentRow.ShipmentId);
                if (shipmentDef.ShippingDocWFS == ShippingDocWFS.NOT_READY)
                {
                    shipmentDef.ShippingDocWFS = ShippingDocWFS.ACCEPTED;
                    OrderWorker.Instance.updateShipmentList(ConvertUtility.createArrayList(shipmentDef));
                }

                DailySunInterfaceDef dailyDef = accountWorker.getDailySunInterface(shipmentRow.ShipmentId, 0, SunInterfaceTypeRef.Id.Purchase.GetHashCode());
                if (dailyDef != null && dailyDef.ExtractedDate == DateTime.MinValue)
                {
                    dailyDef.IsActive = true;
                    accountWorker.updateDailySunInterface(dailyDef);
                }

                if (domainDef.SplitShipmentDataSet != null)
                {
                    foreach (NssSplitShipmentDs.SplitShipmentRow splitRow in domainDef.SplitShipmentDataSet.SplitShipment.Rows)
                    {
                        SplitShipmentDef splitDef = OrderSelectWorker.Instance.getSplitShipmentByKey(splitRow.SplitShipmentId);
                        if (splitDef.AccountDocReceiptDate == DateTime.MinValue && !shipmentRow.IsDMSUploadDateNull())
                        {
                            splitDef.AccountDocReceiptDate = shipmentRow.DMSUploadDate;
                            splitDef.ShippingDocWFS = ShippingDocWFS.ACCEPTED;
                            OrderWorker.Instance.updateSplitShipmentList(ConvertUtility.createArrayList(splitDef), 99999);
                        }
                        dailyDef = accountWorker.getDailySunInterface(shipmentRow.ShipmentId, splitRow.SplitShipmentId, SunInterfaceTypeRef.Id.Purchase.GetHashCode());
                        if (dailyDef != null && dailyDef.ExtractedDate == DateTime.MinValue)
                        {
                            dailyDef.IsActive = true;
                            accountWorker.updateDailySunInterface(dailyDef);
                        }
                    }
                }
            }

            decimal utFOBSurchargeCalculatedAmt = 0;
            decimal importDutyCalculatedAmt = 0;
            decimal inputVATCalculatedAmt = 0;
            decimal outputVATCalculatedAmt = 0;

            //if (CustomerDestinationDef.isUTurnOrder(shipmentRow.CustomerDestinationId) && shipmentRow.WorkflowStatusId != ContractWFS.CANCELLED.Id)
            if (shipmentRow.TermOfPurchaseId == TermOfPurchaseRef.Id.FOB_UT.GetHashCode() && shipmentRow.WorkflowStatusId != ContractWFS.CANCELLED.Id)
            {
                CurrencyRef currency = generalWorker.getCurrencyByKey(shipmentRow.BuyCurrencyId);
                ShipmentCountryRef shipmentCountry = null;

                if (!shipmentRow.IsShipFromCountryIdNull())
                    shipmentCountry = commonWorker.getShipmentCountryByKey(shipmentRow.ShipFromCountryId);

                if (domainDef.ShipmentDetailDataSet != null)
                {
                    foreach (NssShipmentDetailDs.ShipmentDetailRow r in domainDef.ShipmentDetailDataSet.ShipmentDetail.Rows)
                    {
                        if (r.Status == 1)
                            utFOBSurchargeCalculatedAmt += r.FobUTSurcchargeUSD * r.POQty;
                    }
                }

                if (currency.CurrencyCode == CurrencyId.USD.Name && shipmentRow.QuarterlyExchangeRate != 0)
                {
                    importDutyCalculatedAmt = shipmentRow.TotalPONetFOBAmt * (shipmentRow.ImportDutyPercent / 100) * (1 / shipmentRow.QuarterlyExchangeRate);
                    if (shipmentCountry != null)
                    {
                        if (shipmentCountry.OPSKey == "HK")
                        {
                            inputVATCalculatedAmt = ((shipmentRow.TotalPONetFOBAmt * (1 / shipmentRow.QuarterlyExchangeRate)) + importDutyCalculatedAmt) * (shipmentRow.VatPercent / 100);
                            outputVATCalculatedAmt = (((shipmentRow.TotalPOAmt + (shipmentRow.TotalPOAmt * (shipmentRow.NSLCommissionPercent / 100))) * (1 / shipmentRow.QuarterlyExchangeRate)) + importDutyCalculatedAmt) * (shipmentRow.VatPercent / 100);
                        }
                        else
                        {
                            inputVATCalculatedAmt = ((shipmentRow.TotalPONetFOBAmt * (1 / shipmentRow.QuarterlyExchangeRate))) * (shipmentRow.VatPercent / 100);
                            outputVATCalculatedAmt = (((shipmentRow.TotalPOAmt + (shipmentRow.TotalPOAmt * (shipmentRow.NSLCommissionPercent / 100))) * (1 / shipmentRow.QuarterlyExchangeRate))) * (shipmentRow.VatPercent / 100);
                        }
                    }
                }
                else
                {
                    importDutyCalculatedAmt = shipmentRow.TotalPONetFOBAmt * (shipmentRow.ImportDutyPercent / 100);
                    if (shipmentCountry != null)
                    {
                        if (shipmentCountry.OPSKey == "HK")
                        {
                            inputVATCalculatedAmt = ((shipmentRow.TotalPONetFOBAmt) + importDutyCalculatedAmt) * (shipmentRow.VatPercent / 100);
                            outputVATCalculatedAmt = (((shipmentRow.TotalPOAmt + (shipmentRow.TotalPOAmt * (shipmentRow.NSLCommissionPercent / 100)))) + importDutyCalculatedAmt) * (shipmentRow.VatPercent / 100);
                        }
                        else
                        {
                            inputVATCalculatedAmt = ((shipmentRow.TotalPONetFOBAmt)) * (shipmentRow.VatPercent / 100);
                            outputVATCalculatedAmt = (((shipmentRow.TotalPOAmt + (shipmentRow.TotalPOAmt * (shipmentRow.NSLCommissionPercent / 100))))) * (shipmentRow.VatPercent / 100);
                        }
                    }
                }
            }
            row.ImportDutyCalculatedAmt = importDutyCalculatedAmt;
            row.InputVATCalculatedAmt = inputVATCalculatedAmt;
            row.OutputVATCalculatedAmt = outputVATCalculatedAmt;

            if (shipmentRow.WorkflowStatusId == ContractWFS.INVOICED.Id)
            {
                row.InvoiceSellExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, shipmentRow.SellCurrencyId, row.InvoiceDate);
                row.InvoiceBuyExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.INVOICE, shipmentRow.BuyCurrencyId, row.InvoiceDate);
                ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(shipmentRow.ShipmentId);
                row.NSLCommissionAmt = Math.Round(shipmentDef.NSLCommissionPercentage / 100 * shipmentDef.TotalShippedAmount, 2, MidpointRounding.AwayFromZero);

                if (ShippingWorker.Instance.isChoiceActualAmountAutoUpdate(contractRow.CustomerId, contractRow.OfficeId))
                {
                    row.ChoiceOrderTotalShippedAmt = shipmentDef.TotalShippedAmount;
                    row.ChoiceOrderTotalShippedSupplierGmtAmt = shipmentDef.TotalShippedSupplierGarmentAmount;
                    row.ChoiceOrderNSLCommissionAmt = Math.Round(shipmentDef.NSLCommissionPercentage / 100 * shipmentDef.TotalShippedAmount, 2, MidpointRounding.AwayFromZero);
                }

            }

            row.Status = shipmentRow.Status;
        }

        private void SizeOptionMapping(SizeOptionDs.SizeOptionRow row, NssSizeOptionDs.SizeOptionRow sourceRow)
        {
            row.SizeOptionId = sourceRow.SizeOptionId;
            row.SizeOptionNo = sourceRow.SizeOptionNo;
            row.SizeDesc = sourceRow.SizeDesc;
            if (!sourceRow.IsEffectiveDateFromNull())
                row.EffectiveDateFrom = sourceRow.EffectiveDateFrom;
            if (!sourceRow.IsEffectiveDateToNull())
                row.EffectiveDateTo = sourceRow.EffectiveDateTo;
            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        private void ProductMapping(ProductDs.ProductRow row, NssProductDs.ProductRow sourceRow)
        {
            row.ProductId = sourceRow.ProductId;
            row.ItemNo = sourceRow.ItemNo;
            if (!sourceRow.IsParentIdNull())
                row.ParentId = sourceRow.ParentId;
            if (!sourceRow.IsSplitSuffixNull())
                row.SplitSuffix = sourceRow.SplitSuffix;
            if (!sourceRow.IsDesc1Null())
                row.Desc1 = sourceRow.Desc1;
            else
                row.SetDesc1Null();
            if (!sourceRow.IsDesc2Null())
                row.Desc2 = sourceRow.Desc2;
            else
                row.SetDesc2Null();
            if (!sourceRow.IsDesc3Null())
                row.Desc3 = sourceRow.Desc3;
            else
                row.SetDesc3Null();
            if (!sourceRow.IsDesc4Null())
                row.Desc4 = sourceRow.Desc4;
            else
                row.SetDesc4Null();
            if (!sourceRow.IsDesc5Null())
                row.Desc5 = sourceRow.Desc5;
            else
                row.SetDesc5Null();
            if (!sourceRow.IsShortDescNull())
                row.ShortDesc = sourceRow.ShortDesc;
            else
                row.SetShortDescNull();
            if (!sourceRow.IsColourNull())
                row.Colour = sourceRow.Colour;
            else
                row.SetColourNull();
            if (!sourceRow.IsDesignSourceIdNull())
                row.DesignSourceId = sourceRow.DesignSourceId;
            else
                row.SetDesignSourceIdNull();
            if (!sourceRow.IsDesignerIdNull())
                row.DesignerId = sourceRow.DesignerId;
            else
                row.SetDesignerIdNull();
            if (!sourceRow.IsDesignRefNull())
                row.DesignRef = sourceRow.DesignRef;
            else
                row.SetDesignRefNull();
            if (!sourceRow.IsGarmentWashNull())
                row.GarmentWash = sourceRow.GarmentWash;
            else
                row.SetGarmentWashNull();
            if (!sourceRow.IsCartonTypeIdNull())
                row.CartonTypeId = sourceRow.CartonTypeId;
            else
                row.SetCartonTypeIdNull();
            if (!sourceRow.IsQtyPerCartonNull())
                row.QtyPerCarton = sourceRow.QtyPerCarton;
            else
                row.SetQtyPerCartonNull();
            if (!sourceRow.IsProductDesignStyleIdNull())
                row.ProductDesignStyleId = sourceRow.ProductDesignStyleId;
            else
                row.SetProductDesignStyleIdNull();

            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        private void OtherCostMapping(OtherCostDs.OtherCostRow row, NssOtherCostDs.OtherCostRow sourceRow)
        {
            row.OtherCostId = sourceRow.OtherCostId;
            row.ShipmentTypeId = sourceRow.ShipmentTypeId;
            row.ShipmentDetailId = sourceRow.ShipmentDetailId;
            row.OtherCostTypeId = sourceRow.OtherCostTypeId;
            if (!sourceRow.IsVendorIdNull())
                row.VendorId = sourceRow.VendorId;
            row.CurrencyId = sourceRow.CurrencyId;
            row.POExchangeRate = sourceRow.POExchangeRate;
            row.OtherCostAmt = sourceRow.OtherCostAmt;
            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        private void SplitShipmentDetailMapping(SplitShipmentDetailDs.SplitShipmentDetailRow row, NssSplitShipmentDetailDs.SplitShipmentDetailRow sourceRow, bool isNSLSZOrder, int otherCurrencyId)
        {
            row.SplitShipmentDetailId = sourceRow.SplitShipmentDetailId;
            row.SplitShipmentId = sourceRow.SplitShipmentId;
            row.ShipmentDetailId = sourceRow.ShipmentDetailId;
            row.SizeOptionId = sourceRow.SizeOptionId;
            row.SellingPrice = sourceRow.SellingPrice;
            row.NetFOBPrice = isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice);
            row.SupplierGmtPrice = isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice);
            row.OrderQty = sourceRow.OrderQty;
            row.POQty = sourceRow.POQty;
            if (!sourceRow.IsOPAUpchargeNull())
                row.OPAUpcharge = sourceRow.OPAUpcharge;
            if (row.RowState == DataRowState.Detached)
            {
                row.ShippedQty = 0;
            }
            if (!sourceRow.IsReducedSellingPriceNull())
                row.ReducedSellingPrice = sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice;
            else
                row.ReducedSellingPrice = sourceRow.SellingPrice;
            if (!sourceRow.IsReducedNetFOBPriceNull())
                row.ReducedNetFOBPrice = sourceRow.ReducedNetFOBPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice) : sourceRow.ReducedNetFOBPrice;
            else
                row.ReducedNetFOBPrice = (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice);
            if (isNSLSZOrder) row.ReducedNetFOBPrice = row.ReducedSellingPrice;
            if (!sourceRow.IsReducedSupplierGmtPriceNull())
                row.ReducedSupplierGmtPrice = sourceRow.ReducedSupplierGmtPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice) : sourceRow.ReducedSupplierGmtPrice;
            else
                row.ReducedSupplierGmtPrice = (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice);
            if (isNSLSZOrder) row.ReducedSupplierGmtPrice = row.ReducedSellingPrice;
            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        private void SplitShipmentMapping(SplitShipmentDs.SplitShipmentRow row, NssSplitShipmentDs.SplitShipmentRow sourceRow, NssSplitShipmentDetailDs detailDs, bool isNSLSZOrder, int otherCurrencyId)
        {
            row.SplitShipmentId = sourceRow.SplitShipmentId;
            row.ShipmentId = sourceRow.ShipmentId;
            row.SplitSuffix = sourceRow.SplitSuffix;
            row.ProductId = sourceRow.ProductId;
            row.PiecesPerPack = sourceRow.PiecesPerPack;
            row.PackingUnitId = sourceRow.PackingUnitId;
            /*
            if (!sourceRow.IsVendorIdNull())
                row.VendorId = sourceRow.VendorId;
            */
            row.VendorId = sourceRow.VendorId;
            row.SupplierAtWarehouseDate = sourceRow.SupplierAtWarehouseDate;
            if (!sourceRow.IsPaymentTermIdNull())
                row.PaymentTermId = sourceRow.PaymentTermId;
            if (!sourceRow.IsQuotaCategoryGroupIdNull())
                row.QuotaCategoryGroupId = sourceRow.QuotaCategoryGroupId;
            if (!sourceRow.IsCountryOfOriginIdNull())
                row.CountryOfOriginId = sourceRow.CountryOfOriginId;
            row.SellCurrencyId = sourceRow.SellCurrencyId;
            if (otherCurrencyId > 0)
                row.BuyCurrencyId = otherCurrencyId;
            else
                row.BuyCurrencyId = sourceRow.BuyCurrencyId;
            if (!sourceRow.IsPOExchangeRateNull())
                row.POExchangeRate = sourceRow.POExchangeRate;
            else
                row.POExchangeRate = 0;
            if (!sourceRow.IsMockShopSampleRemarkNull())
                row.MockShopSampleRemark = sourceRow.MockShopSampleRemark;
            if (!sourceRow.IsOtherPOTermRemarkNull())
                row.OtherPOTermRemark = sourceRow.OtherPOTermRemark;
            if (!sourceRow.IsShipDeptRemarkNull())
                row.ShippingRemark = sourceRow.ShipDeptRemark;
            row.TotalOrderQty = sourceRow.TotalOrderQty;
            row.TotalOrderAmt = sourceRow.TotalOrderAmt;
            row.TotalPOQty = sourceRow.TotalPOQty;
            row.TotalPOAmt = sourceRow.TotalPOAmt;
            row.TotalPONetFOBAmt = sourceRow.TotalNetFOBAmt;
            row.TotalNetFOBAmt = sourceRow.TotalNetFOBAmt;
            row.TotalSupplierGmtAmt = sourceRow.TotalSupplierGmtAmt;
            row.TotalOPAUpcharge = sourceRow.TotalOPAUpcharge;

            row.TotalPOSupplierGmtAmt = 0;
            row.TotalPOSupplierGmtAmtAfterDiscount = 0;
            row.TotalOrderAmtAfterDiscount = 0;
            row.TotalPOAmtAfterDiscount = 0;
            row.TotalPONetFOBAmtAfterDiscount = 0;
            row.TotalNetFOBAmtAfterDiscount = 0;
            row.TotalSupplierGmtAmtAfterDiscount = 0;

            if (detailDs != null)
            {
                foreach (NssSplitShipmentDetailDs.SplitShipmentDetailRow r in detailDs.SplitShipmentDetail.Rows)
                {
                    if (r.IsReducedSellingPriceNull()) r.ReducedSellingPrice = 0;
                    if (r.IsReducedNetFOBPriceNull()) r.ReducedNetFOBPrice = 0;
                    if (r.IsReducedSupplierGmtPriceNull()) r.ReducedSupplierGmtPrice = 0;

                    if (r.Status == GeneralCriteria.TRUE && r.SplitShipmentId == sourceRow.SplitShipmentId)
                    {
                        row.TotalPOSupplierGmtAmt += (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.SupplierGmtPrice) * r.POQty;
                        row.TotalOrderAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.OrderQty;
                        row.TotalPOAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.POQty;
                        if (isNSLSZOrder)
                        {
                            row.TotalPONetFOBAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.POQty;
                            row.TotalPOSupplierGmtAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.POQty;
                            row.TotalNetFOBAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.OrderQty;
                            row.TotalSupplierGmtAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.OrderQty;
                        }
                        else
                        {
                            row.TotalPONetFOBAmtAfterDiscount += (r.ReducedNetFOBPrice == 0 ? (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.NetFOBPrice) : r.ReducedNetFOBPrice) * r.POQty;
                            row.TotalPOSupplierGmtAmtAfterDiscount += (r.ReducedSupplierGmtPrice == 0 ? (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.SupplierGmtPrice) : r.ReducedSupplierGmtPrice) * r.POQty;
                            row.TotalNetFOBAmtAfterDiscount += (r.ReducedNetFOBPrice == 0 ? (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.NetFOBPrice) : r.ReducedNetFOBPrice) * r.OrderQty;
                            row.TotalSupplierGmtAmtAfterDiscount += (r.ReducedSupplierGmtPrice == 0 ? (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.SupplierGmtPrice) : r.ReducedSupplierGmtPrice) * r.OrderQty;
                        }
                    }
                }
            }

            if (row.RowState == DataRowState.Detached)
            {
                row.InvoiceBuyExchangeRate = 0;
                row.TotalShippedQty = 0;
                row.TotalShippedAmt = 0;
                row.TotalShippedAmtAfterDiscount = 0;
                row.TotalShippedNetFOBAmt = 0;
                row.TotalShippedNetFOBAmtAfterDiscount = 0;
                row.TotalShippedSupplierGmtAmt = 0;
                row.TotalShippedSupplierGmtAmtAfterDiscount = 0;
                row.TotalShippedOPAUpcharge = 0;
                row.LCAmt = 0;
                row.IsLCPaymentChecked = GeneralCriteria.FALSE;
                row.PaymentLock = false;
                row.APAmt = 0;
                row.APExchangeRate = 0;
                row.IsILSQtyUploadAllowed = true;
                row.DMSWorkflowStatusId = ShippingDocWFS.NOT_READY.Id;
                row.ShippingCheckedTotalNetAmount = 0;
            }

            if (!sourceRow.IsVendorPaymentDiscountPercentNull())
                row.VendorPaymentDiscountPercent = sourceRow.VendorPaymentDiscountPercent;
            else
                row.VendorPaymentDiscountPercent = 0;
            if (!sourceRow.IsLabTestIncomeNull())
                row.LabTestIncome = sourceRow.LabTestIncome;
            else
                row.LabTestIncome = 0;
            if (!sourceRow.IsIsVirtualSetSplitNull())
                row.IsVirtualSetSplit = sourceRow.IsVirtualSetSplit;
            else
                row.IsVirtualSetSplit = GeneralCriteria.FALSE;
            if (!sourceRow.IsIsKnitwearComponentNull())
                row.IsKnitwearComponent = sourceRow.IsKnitwearComponent;
            else
                row.IsKnitwearComponent = GeneralCriteria.FALSE;
            if (!sourceRow.IsIsFobOrderNull())
                row.IsFobOrder = sourceRow.IsFobOrder;
            else
                row.IsFobOrder = GeneralCriteria.FALSE;
            if (!sourceRow.IsQaCommissionPercentNull())
                row.QACommissionPercent = sourceRow.QaCommissionPercent;
            else
                row.QACommissionPercent = 0;
            if (!sourceRow.IsColourNull())
                row.Colour = sourceRow.Colour;
            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        private void ShipmentDetailMapping(ShipmentDetailDs.ShipmentDetailRow row, NssShipmentDetailDs.ShipmentDetailRow sourceRow, bool isNSLSZOrder, int otherCurrencyId)
        {
            row.ShipmentDetailId = sourceRow.ShipmentDetailId;
            row.ShipmentId = sourceRow.ShipmentId;
            row.SizeOptionId = sourceRow.SizeOptionId;
            row.SellingPrice = sourceRow.SellingPrice;
            row.NetFOBPrice = isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice);
            row.SupplierGmtPrice = isNSLSZOrder ? sourceRow.SellingPrice : (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice);
            row.OrderQty = sourceRow.OrderQty;
            row.POQty = sourceRow.POQty;
            row.TotalOtherCost = sourceRow.TotalOtherCost;
            row.TotalShippedOtherCost = sourceRow.TotalOtherCost;
            row.OPAUpcharge = sourceRow.OPAUpcharge;
            if (row.RowState == DataRowState.Detached)
            {
                row.ShippedQty = 0;
            }
            if (!sourceRow.IsGSPFormTypeIdNull())
                row.GSPFormTypeId = sourceRow.GSPFormTypeId;
            if (!sourceRow.IsRatioPackNull())
                row.RatioPack = sourceRow.RatioPack;
            else
                row.RatioPack = 0;
            if (!sourceRow.IsRetailSellingPriceNull())
                row.RetailSellingPrice = sourceRow.RetailSellingPrice;
            else
                row.RetailSellingPrice = 0;
            if (!sourceRow.IsReducedSellingPriceNull())
                row.ReducedSellingPrice = sourceRow.ReducedSellingPrice == 0 ? sourceRow.SellingPrice : sourceRow.ReducedSellingPrice;
            else
                row.ReducedSellingPrice = sourceRow.SellingPrice;
            if (!sourceRow.IsReducedNetFOBPriceNull())
                row.ReducedNetFOBPrice = sourceRow.ReducedNetFOBPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice) : sourceRow.ReducedNetFOBPrice;
            else
                row.ReducedNetFOBPrice = (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.NetFOBPrice);
            if (isNSLSZOrder) row.ReducedNetFOBPrice = row.ReducedSellingPrice;
            if (!sourceRow.IsReducedSupplierGmtPriceNull())
                row.ReducedSupplierGmtPrice = sourceRow.ReducedSupplierGmtPrice == 0 ? (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice) : sourceRow.ReducedSupplierGmtPrice;
            else
                row.ReducedSupplierGmtPrice = (otherCurrencyId > 0 ? sourceRow.SupplierGmtPriceOtherCcy : sourceRow.SupplierGmtPrice);
            if (isNSLSZOrder) row.ReducedSupplierGmtPrice = row.ReducedSellingPrice;
            if (!sourceRow.IsFreightCostNull())
                row.FreightCost = sourceRow.FreightCost;
            else
                row.FreightCost = 0;
            if (!sourceRow.IsDutyCostNull())
                row.DutyCost = sourceRow.DutyCost;
            else
                row.DutyCost = 0;
            row.NetSellingPrice = 0;
            row.NetBuyingPrice = 0;
            if (!sourceRow.IsFobUTSurcchargeUSDNull())
                row.FobUTSurchargeUSD = sourceRow.FobUTSurcchargeUSD;
            else
                row.FobUTSurchargeUSD = 0;
            if (!sourceRow.IsCMPriceUSDNull())
                row.CMPriceUSD = sourceRow.CMPriceUSD;
            else
                row.CMPriceUSD = 0;
            if (!sourceRow.IsColourNull())
                if (sourceRow.Colour.Trim() == String.Empty)
                    row.SetColourNull();
                else
                    row.Colour = sourceRow.Colour;
            else
                row.SetColourNull();
            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        private void ShipmentMapping(NssShipmentDs.ShipmentRow sourceRow, ShipmentDs.ShipmentRow row, NssShipmentDetailDs detailDs, bool isNSLSZOrder, int otherCurrencyId)
        {
            row.ShipmentId = sourceRow.ShipmentId;
            row.ContractId = sourceRow.ContractId;
            row.DeliveryNo = sourceRow.DeliveryNo;
            if (!sourceRow.IsNSLPONoNull())
                row.NSLPONo = sourceRow.NSLPONo;
            row.VendorId = isNSLSZOrder ? 3933 : sourceRow.VendorId;
            if (isNSLSZOrder)
                row.VMVendorId = sourceRow.VendorId;
            else
                row.SetVMVendorIdNull();
            if (!sourceRow.IsFactoryIdNull())
                row.FactoryId = sourceRow.FactoryId;
            row.SupplierAssignTypeId = sourceRow.SupplierAssignTypeId;
            row.TermOfPurchaseId = isNSLSZOrder ? TermOfPurchaseRef.Id.FOB.GetHashCode() : sourceRow.TermOfPurchaseId;
            row.OriginalTermOfPurchaseId = sourceRow.TermOfPurchaseId;
            row.PurchaseLocationId = sourceRow.PurchaseLocationId;
            if (!sourceRow.IsUKAtWarehouseDateNull())
                row.UKAtWarehouseDate = sourceRow.UKAtWarehouseDate;
            row.CustomerAtWarehouseDate = sourceRow.CustomerAtWarehouseDate;
            row.SupplierAtWarehouseDate = sourceRow.SupplierAtWarehouseDate;
            row.ShipmentMethodId = sourceRow.ShipmentMethodId;
            if (!sourceRow.IsAirFreightPaymentTypeIdNull())
                row.AirFreightPaymentTypeId = sourceRow.AirFreightPaymentTypeId;
            row.NUKAirFreightPaymentPercent = sourceRow.NUKAirFreightPaymentPercent;
            row.NSLAirFreightPaymentPercent = sourceRow.NSLAirFreightPaymentPercent;
            row.FTYAirFreightPaymentPercent = sourceRow.FTYAirFreightPaymentPercent;
            if (!sourceRow.IsNSLSZAirFreightPaymentPercentNull())
                row.NSLSZAirFreightPaymentPercent = sourceRow.NSLSZAirFreightPaymentPercent;
            else
                row.NSLSZAirFreightPaymentPercent = 0;
            row.OtherAirFreightPaymentPercent = sourceRow.OtherAirFreightPaymentPercent;
            if (!sourceRow.IsOtherAirFreightPaymentRemarkNull())
                row.OtherAirFreightPaymentRemark = sourceRow.OtherAirFreightPaymentRemark;
            if (!sourceRow.IsAirFreightPaymentRemarkNull())
                row.AirFreightPaymentRemark = sourceRow.AirFreightPaymentRemark;
            row.AgencyCommissionPercent = sourceRow.AgencyCommissionPercent;
            if (!sourceRow.IsPaymentTermIdNull())
                row.PaymentTermId = sourceRow.PaymentTermId;
            if (!sourceRow.IsQuotaCategoryGroupIdNull())
                row.QuotaCategoryGroupId = sourceRow.QuotaCategoryGroupId;
            if (!sourceRow.IsCountryOfOriginIdNull())
                row.CountryOfOriginId = sourceRow.CountryOfOriginId;
            if (!sourceRow.IsShipFromCountryIdNull())
                row.ShipFromCountryId = sourceRow.ShipFromCountryId;
            if (!sourceRow.IsShipmentPortIdNull())
                row.ShipmentPortId = sourceRow.ShipmentPortId;
            row.SellCurrencyId = sourceRow.SellCurrencyId;
            row.BuyCurrencyId = (otherCurrencyId > 0 ? otherCurrencyId : sourceRow.BuyCurrencyId);
            row.POExchangeRate = sourceRow.POExchangeRate;
            row.USExchangeRate = sourceRow.USExchangeRate;
            if (!sourceRow.IsVatPercentNull())
                row.VatPercent = sourceRow.VatPercent;
            else
                row.VatPercent = 0;
            row.NSLCommissionPercent = sourceRow.NSLCommissionPercent;
            if (!sourceRow.IsMockShopSampleRemarkNull())
                row.MockShopSampleRemark = sourceRow.MockShopSampleRemark;
            else
                row.SetMockShopSampleRemarkNull();
            if (!sourceRow.IsShipDeptRemarkNull())
                row.NotesFromMerchandiser = sourceRow.ShipDeptRemark;
            else
                row.SetNotesFromMerchandiserNull();
            row.TotalOrderQty = sourceRow.TotalOrderQty;
            row.TotalOrderAmt = sourceRow.TotalOrderAmt;
            row.TotalPOQty = sourceRow.TotalPOQty;
            row.TotalPOAmt = sourceRow.TotalPOAmt;
            row.TotalPONetFOBAmt = sourceRow.TotalPONetFOBAmt;
            row.TotalNetFOBAmt = sourceRow.TotalNetFOBAmt;
            row.TotalSupplierGmtAmt = sourceRow.TotalSupplierGmtAmt;
            row.TotalOtherCost = sourceRow.TotalOtherCost;
            if (!sourceRow.IsTotalOrderFreightCostNull())
                row.TotalOrderFreightCost = sourceRow.TotalOrderFreightCost;
            else
                row.TotalOrderFreightCost = 0;
            if (!sourceRow.IsTotalOrderDutyCostNull())
                row.TotalOrderDutyCost = sourceRow.TotalOrderDutyCost;
            else
                row.TotalOrderDutyCost = 0;
            if (!sourceRow.IsTotalOPAUpchargeNull())
                row.TotalOPAUpcharge = sourceRow.TotalOPAUpcharge;
            else
                row.TotalOPAUpcharge = 0;

            row.TotalPOSupplierGmtAmt = 0;
            row.TotalPOSupplierGmtAmtAfterDiscount = 0;
            row.TotalOrderAmtAfterDiscount = 0;
            row.TotalPOAmtAfterDiscount = 0;
            row.TotalPONetFOBAmtAfterDiscount = 0;
            row.TotalNetFOBAmtAfterDiscount = 0;
            row.TotalSupplierGmtAmtAfterDiscount = 0;

            if (detailDs != null)
            {
                foreach (NssShipmentDetailDs.ShipmentDetailRow r in detailDs.ShipmentDetail.Rows)
                {
                    if (r.IsReducedSellingPriceNull()) r.ReducedSellingPrice = 0;
                    if (r.IsReducedNetFOBPriceNull()) r.ReducedNetFOBPrice = 0;
                    if (r.IsReducedSupplierGmtPriceNull()) r.ReducedSupplierGmtPrice = 0;

                    if (r.Status == GeneralCriteria.TRUE)
                    {
                        row.TotalPOSupplierGmtAmt += (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.SupplierGmtPrice) * r.POQty;
                        row.TotalOrderAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.OrderQty;
                        row.TotalPOAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.POQty;
                        if (isNSLSZOrder)
                        {
                            row.TotalPONetFOBAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.POQty;
                            row.TotalPOSupplierGmtAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.SellingPrice : r.ReducedSellingPrice) * r.POQty;
                            row.TotalNetFOBAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.ReducedSellingPrice : r.ReducedSellingPrice) * r.OrderQty;
                            row.TotalSupplierGmtAmtAfterDiscount += (r.ReducedSellingPrice == 0 ? r.ReducedSellingPrice : r.ReducedSellingPrice) * r.OrderQty;
                        }
                        else
                        {
                            row.TotalPONetFOBAmtAfterDiscount += (r.ReducedNetFOBPrice == 0 ? r.NetFOBPrice : (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.ReducedNetFOBPrice)) * r.POQty;
                            row.TotalPOSupplierGmtAmtAfterDiscount += (r.ReducedSupplierGmtPrice == 0 ? r.SupplierGmtPrice : (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.ReducedSupplierGmtPrice)) * r.POQty;
                            row.TotalNetFOBAmtAfterDiscount += (r.ReducedNetFOBPrice == 0 ? r.NetFOBPrice : (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.ReducedNetFOBPrice)) * r.OrderQty;
                            row.TotalSupplierGmtAmtAfterDiscount += (r.ReducedSupplierGmtPrice == 0 ? r.SupplierGmtPrice : (otherCurrencyId > 0 ? r.SupplierGmtPriceOtherCcy : r.ReducedSupplierGmtPrice)) * r.OrderQty;
                        }
                    }
                }
            }

            if (row.RowState == DataRowState.Detached)
            {
                row.TotalShippedQty = 0;
                row.TotalShippedAmt = 0;
                row.TotalShippedAmtAfterDiscount = 0;
                row.TotalShippedNetFOBAmt = 0;
                row.TotalShippedNetFOBAmtAfterDiscount = 0;
                row.TotalShippedSupplierGmtAmt = 0;
                row.TotalShippedSupplierGmtAmtAfterDiscount = 0;
                row.TotalShippedOtherCost = 0;
                row.TotalShippedFreightCost = 0;
                row.TotalShippedDutyCost = 0;
                row.TotalShippedOPAUpcharge = 0;
                row.DMSWorkflowStatusId = ShippingDocWFS.NOT_READY.Id;
                row.EditLock = false;
                row.PaymentLock = false;
                row.IsChinaGBTestRequired = false;
                row.WorkflowStatusId = sourceRow.WorkflowStatusId;
            }
            else if (row.WorkflowStatusId != ContractWFS.INVOICED.Id)
                row.WorkflowStatusId = sourceRow.WorkflowStatusId;

            row.SplitCount = sourceRow.SplitCount;
            if (!sourceRow.IsIsRepeatOrderNull())
                row.IsRepeatOrder = sourceRow.IsRepeatOrder;
            else
                row.IsRepeatOrder = GeneralCriteria.FALSE;
            if (!sourceRow.IsDelayReasonTypesNull())
                row.DelayReasonTypes = sourceRow.DelayReasonTypes;
            if (!sourceRow.IsDelayReasonOtherNull())
                row.DelayReasonOther = sourceRow.DelayReasonOther;
            if (!sourceRow.IsIsMockShopSampleNull())
                row.IsMockShopSample = sourceRow.IsMockShopSample;
            else
            {
                row.IsMockShopSample = GeneralCriteria.FALSE;
                sourceRow.IsMockShopSample = GeneralCriteria.FALSE;
            }
            if (!sourceRow.IsIsPressSampleNull())
                row.IsPressSample = sourceRow.IsPressSample;
            else
            {
                row.IsPressSample = GeneralCriteria.FALSE;
                sourceRow.IsPressSample = GeneralCriteria.FALSE;
            }
            if (!sourceRow.IsIsStudioSampleNull())
                row.IsStudioSample = sourceRow.IsStudioSample;
            else
            {
                row.IsStudioSample = GeneralCriteria.FALSE;
                sourceRow.IsStudioSample = GeneralCriteria.FALSE;
            }
            if (!sourceRow.IsSalesForecastSpecialGroupIdNull())
                row.SalesForecastSpecialGroupId = sourceRow.SalesForecastSpecialGroupId;
            else
                row.SetSalesForecastSpecialGroupIdNull();
            
            if (!sourceRow.IsSpecialOrderTypeIdNull())
                row.SpecialOrderTypeId = sourceRow.SpecialOrderTypeId;
            else
                row.SpecialOrderTypeId = 0;

            if (!sourceRow.IsVendorPaymentDiscountPercentNull())
                row.VendorPaymentDiscountPercent = sourceRow.VendorPaymentDiscountPercent;
            else
                row.VendorPaymentDiscountPercent = 0;
            if (!sourceRow.IsLabTestIncomeNull())
                row.LabTestIncome = sourceRow.LabTestIncome;
            else
                row.LabTestIncome = 0;
            if (!sourceRow.IsCustomDocTypeNull())
                row.CustomDocType = sourceRow.CustomDocType;
            if (!sourceRow.IsIsVirtualSetSplitNull())
                row.IsVirtualSetSplit = sourceRow.IsVirtualSetSplit;
            else
                row.IsVirtualSetSplit = GeneralCriteria.FALSE;
            if (!sourceRow.IsThirdPartyAgencyIdNull())
                row.ThirdPartyAgencyId = sourceRow.ThirdPartyAgencyId;
            if (!sourceRow.IsIsRatioPackOrderNull())
                row.IsRatioPackOrder = sourceRow.IsRatioPackOrder;
            else
                row.IsRatioPackOrder = GeneralCriteria.FALSE;
            if (!sourceRow.IsRatioPackTypeNull())
                row.RatioPackType = sourceRow.RatioPackType;
            if (!sourceRow.IsIsUKDiscountNull())
                row.IsUKDiscount = sourceRow.IsUKDiscount;
            else
                row.IsUKDiscount = GeneralCriteria.FALSE;
            if (!sourceRow.IsUKDiscountReasonIdNull())
                row.UKDiscountReasonId = sourceRow.UKDiscountReasonId;
            if (!sourceRow.IswithOPRFabricNull())
                row.WithOPRFabric = sourceRow.withOPRFabric;
            else
                row.WithOPRFabric = GeneralCriteria.FALSE;
            if (!sourceRow.IsCustomerDestinationIdNull())
                row.CustomerDestinationId = sourceRow.CustomerDestinationId;
            if (!sourceRow.IsSellingUTSurchargePercentNull())
                row.SellingUTSurchargePercent = sourceRow.SellingUTSurchargePercent;
            else
                row.SellingUTSurchargePercent = 0;
            if (!sourceRow.IsFobUTSurchargePercentNull())
                row.FobUTSurchargePercent = sourceRow.FobUTSurchargePercent;
            else
                row.FobUTSurchargePercent = 0;
            if (!sourceRow.IsImportDutyPercentNull())
                row.ImportDutyPercent = sourceRow.ImportDutyPercent;
            else
                row.ImportDutyPercent = 0;
            if (!sourceRow.IsQuarterlyExchangeRateNull())
                row.QuarterlyExchangeRate = sourceRow.QuarterlyExchangeRate;
            else
                row.QuarterlyExchangeRate = 0;
            if (!sourceRow.IsIsNSLVMTROrderNull())
                row.IsNSLVMTROrder = sourceRow.IsNSLVMTROrder;
            else
                row.IsNSLVMTROrder = GeneralCriteria.FALSE;
            if (!sourceRow.IsCMCostNull())
                row.CMCost = sourceRow.CMCost;
            else
                row.CMCost = 0;
            if (!sourceRow.IsQaCommissionPercentNull())
                row.QACommissionPercent = sourceRow.QaCommissionPercent;
            else
                row.QACommissionPercent = 0;
            if (!sourceRow.IsGTCommissionPercentNull())
                row.GTCommissionPercent = sourceRow.GTCommissionPercent;
            else
                row.GTCommissionPercent = 0;
            /*
            if (!sourceRow.IsAdditionalBankChargesPercentNull())
                row.AdditionalBankChargesPercent = sourceRow.AdditionalBankChargesPercent;
            else
                row.AdditionalBankChargesPercent = 0;
            */
            if (!sourceRow.IsColourNull())
                row.Colour = sourceRow.Colour;
            if (!sourceRow.IsWithQcChargeNull())
                row.WithQCCharge = sourceRow.WithQcCharge;
            else
                row.WithQCCharge = 0;

            if (!sourceRow.IsIsTradingAFNull())
                row.IsTradingAF = sourceRow.IsTradingAF;
            else
                row.IsTradingAF = 0;
            if (!sourceRow.IsTradingAFReasonNull())
                row.TradingAFReason = sourceRow.TradingAFReason;
            else
                row.SetTradingAFReasonNull();
            if (!sourceRow.IsTradingAFTypeIdNull())
                row.TradingAFTypeId = sourceRow.TradingAFTypeId;
            else
                row.SetTradingAFTypeIdNull();

            if (!sourceRow.IsEstimationAmtInUSDNull())
                row.TradingAFEstimationCost = sourceRow.EstimationAmtInUSD;
            else
                row.TradingAFEstimationCost = 0;

            if (!sourceRow.IsNSLRefNoNull() && sourceRow.NSLRefNo.Trim() != string.Empty)
                row.NSLRefNo = sourceRow.NSLRefNo;
            else
                row.SetNSLRefNoNull();

            if (!sourceRow.IsGSPFormTypeIdNull())
                row.GSPFormTypeId = sourceRow.GSPFormTypeId;
            else
                row.SetGSPFormTypeIdNull();

            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        private void ContractMapping(NssContractDs.ContractRow sourceRow, ContractDs.ContractRow row)
        {
            row.ContractId = sourceRow.ContractId;
            row.ContractNo = sourceRow.ContractNo;
            row.NSLPONo = sourceRow.NSLPONo;
            row.TradingAgencyId = sourceRow.TradingAgencyId;
            row.SupplierAssignTypeId = sourceRow.SupplierAssignTypeId;
            row.ProductId = sourceRow.ProductId;
            row.SeasonId = sourceRow.SeasonId;
            row.OfficeId = sourceRow.OfficeId;
            row.MerchandiserId = sourceRow.MerchandiserId;
            row.DeptId = sourceRow.DeptId;
            row.ProductTeamId = sourceRow.ProductTeamId;
            row.PhaseId = sourceRow.PhaseId;
            row.CustomerId = sourceRow.CustomerId;
            if (!sourceRow.IsBookingRefNoNull())
                row.BookingRefNo = sourceRow.BookingRefNo;
            row.BookingReceivedDate = sourceRow.BookingReceivedDate;
            row.PiecesPerPack = sourceRow.PiecesPerPack;
            row.PackingUnitId = sourceRow.PackingUnitId;
            row.PackingMethodId = sourceRow.PackingMethodId;
            row.SetSplitCount = sourceRow.SetSplitCount;
            if (!sourceRow.IsUKSupplierCodeNull())
                row.UKSupplierCode = sourceRow.UKSupplierCode;
            if (!sourceRow.IsIsVirtualSetSplitNull())
                row.IsVirtualSetSplit = sourceRow.IsVirtualSetSplit;
            else
                row.IsVirtualSetSplit = GeneralCriteria.FALSE;
            if (!sourceRow.IsIsNextMfgOrderNull())
                row.IsNextMfgOrder = sourceRow.IsNextMfgOrder;
            else
                row.IsNextMfgOrder = GeneralCriteria.FALSE;
            if (!sourceRow.IsIsPOIssueToNextMfgNull())
                row.IsPOIssueToNextMfg = sourceRow.IsPOIssueToNextMfg;
            else
                row.IsPOIssueToNextMfg = GeneralCriteria.FALSE;
            if (!sourceRow.IsIsDualSourcingOrderNull())
                row.IsDualSourcingOrder = sourceRow.IsDualSourcingOrder;
            else
                row.IsDualSourcingOrder = GeneralCriteria.FALSE;
            if (!sourceRow.IsIsLDPOrderNull())
                row.IsLDPOrder = sourceRow.IsLDPOrder;
            else
                row.IsLDPOrder = GeneralCriteria.FALSE;
            if (!sourceRow.IsIsBizOrderNull())
                row.IsBizOrder = sourceRow.IsBizOrder;
            else
                row.IsBizOrder = GeneralCriteria.FALSE;

            if (!sourceRow.IsIsShortGameNull())
                row.IsShortGame = sourceRow.IsShortGame;
            else
                row.IsShortGame = GeneralCriteria.FALSE;

            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;

            if (!sourceRow.IsIsEnvSustainableNull())
                row.IsEnvSustainable = sourceRow.IsEnvSustainable;
            else
                row.IsEnvSustainable = 0;
        }

        private void AdvancePaymentMapping(AdvancePaymentDs.AdvancePaymentRow row, NssAdvancePaymentDs.AdvancePaymentRow sourceRow)
        {
            row.PaymentId = sourceRow.PaymentId;
            row.PaymentTypeId = 1;
            row.PaymentNo = sourceRow.PaymentNo;
            row.OfficeId = sourceRow.OfficeId;
            row.VendorId = sourceRow.VendorId;
            row.CurrencyId = sourceRow.CurrencyId;
            row.TotalAmt = sourceRow.TotalAmt;
            row.InterestChargedAmt = sourceRow.InterestChargedAmt;
            if (!sourceRow.IsPaymentDateNull())
            {
                row.PaymentDate = sourceRow.PaymentDate;
            }
            if (!sourceRow.IsUploadedByNull())
            {
                row.UploadedBy = sourceRow.UploadedBy;
            }
            if (!sourceRow.IsUploadedDateNull())
            {
                row.UploadedDate = sourceRow.UploadedDate;
            }
            if (!sourceRow.IsSubmittedByNull())
            {
                row.SubmittedBy = sourceRow.SubmittedBy;
            }
            if (!sourceRow.IsSubmittedDateNull())
            {
                row.SubmittedDate = sourceRow.SubmittedDate;
            }
            if (!sourceRow.IsApprovedByNull())
            {
                row.ApprovedBy = sourceRow.ApprovedBy;
            }
            if (!sourceRow.IsApprovedDateNull())
            {
                row.ApprovedDate = sourceRow.ApprovedDate;
            }
            if (!sourceRow.IsRejectByNull())
            {
                row.RejectBy = sourceRow.RejectBy;
            }
            if (!sourceRow.IsRejectDateNull())
            {
                row.RejectDate = sourceRow.RejectDate;
            }
            if (!sourceRow.IsRejectReasonIdNull())
            {
                row.RejectReasonId = sourceRow.RejectReasonId;
            }
            if (!sourceRow.IsRemarkNull())
            {
                row.Remark = sourceRow.Remark;
            }
            row.WorkflowStatusId = sourceRow.WorkflowStatusId;
            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
            row.PayableTo = sourceRow.PayableTo;
            row.InitiatedBy = sourceRow.InitiatedBy;
            row.InterestRate = sourceRow.InterestRate;
            row.IsC19 = sourceRow.IsC19;
            if (!sourceRow.IsNSLRefNoNull())
                row.FLRefNo = sourceRow.NSLRefNo;
            else
                row.SetFLRefNoNull();
        }

        private void AdvancePaymentOrderDetailMapping(AdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow row, NssAdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow sourceRow)
        {
            row.PaymentId = sourceRow.PaymentId;
            row.ShipmentId = sourceRow.ShipmentId;
            row.ExpectedDeductAmt = sourceRow.ExpectedDeductAmt;
            /*
            row.ActualDeductAmt = sourceRow.ActualDeductAmt;
            if (!sourceRow.IsSettlementDateNull())
            {
                row.SettlementDate = sourceRow.SettlementDate;
            }
            */
            if (!sourceRow.IsRemarkNull())
            {
                row.Remark = sourceRow.Remark;
            }
            row.IsInitial = 1;
            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        private void LetterOfGuaranteeMapping(LetterOfGuaranteeDs.LetterOfGuaranteeRow row, NssLGPaymentDs.LGPaymentRow sourceRow)
        {
            row.LGId = sourceRow.PaymentId;
            row.LGNo = sourceRow.PaymentNo;
            if (!sourceRow.IsUploadedDateNull())
                row.LGDate = sourceRow.UploadedDate;
            row.OfficeId = sourceRow.OfficeId;
            row.VendorId = sourceRow.VendorId;
            if (!sourceRow.IsUploadedByNull())
                row.UploadedBy = sourceRow.UploadedBy;
            if (!sourceRow.IsUploadedDateNull())
                row.UploadedDate = sourceRow.UploadedDate;
            if (!sourceRow.IsSubmittedByNull())
                row.SubmittedBy = sourceRow.SubmittedBy;
            if (!sourceRow.IsSubmittedDateNull())
                row.SubmittedDate = sourceRow.SubmittedDate;
            row.Remark = sourceRow.LGRemark;
            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        private void LGDetailMapping(LGDetailDs.LGDetailRow row, NssLGPaymentOrderDetailDs.LGPaymentOrderDetailRow sourceRow)
        {
            row.LGId = sourceRow.PaymentId;
            row.ShipmentId = sourceRow.ShipmentId;
            row.SplitShipmentId = 0;
            row.Status = sourceRow.Status;
            row.CreatedBy = sourceRow.CreatedBy;
            row.CreatedOn = sourceRow.CreatedOn;
            if (!sourceRow.IsModifiedByNull())
                row.ModifiedBy = sourceRow.ModifiedBy;
            if (!sourceRow.IsModifiedOnNull())
                row.ModifiedOn = sourceRow.ModifiedOn;
        }

        #endregion

    }
}
