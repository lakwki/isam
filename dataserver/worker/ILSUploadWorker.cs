using System;
using System.Data;
using System.Collections;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.common.datafactory.worker;
using com.next.isam.dataserver.model.ils;
using com.next.isam.domain.ils;
using com.next.isam.domain.shipping;
using com.next.isam.dataserver.worker;
using com.next.common.domain.types;
using com.next.isam.domain.types;

namespace com.next.isam.dataserver.worker
{
    public class ILSUploadWorker : Worker
    {
		private static ILSUploadWorker _instance;
        private GeneralWorker generalWorker;

		public ILSUploadWorker()
		{
            generalWorker = GeneralWorker.Instance;
		}

		public static ILSUploadWorker Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new ILSUploadWorker();
				}
				return _instance;
			}
		}

        public string getILSParameter(string paramName)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                int fileNo;

                IDataSetAdapter ad = getDataSetAdapter("ILSParamApt", "GetILSParam");
                ad.SelectCommand.Parameters["@ParamName"].Value = paramName;
                ad.PopulateCommands();

                ILSParamDs dataSet = new ILSParamDs();
                ILSParamDs.ILSParamRow row = null;

                int recordsAffected = ad.Fill(dataSet);

                row = dataSet.ILSParam[0];
                fileNo = row.ParamValue;

                return fileNo.ToString().Trim().PadLeft(5, '0');

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

        public void updateInputFileNo(string paramName)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSParamApt", "GetILSParam");
                ad.SelectCommand.Parameters["@ParamName"].Value = @paramName;
                ad.PopulateCommands();

                ILSParamDs dataSet = new ILSParamDs();
                ILSParamDs.ILSParamRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                row = dataSet.ILSParam[0];
                if (row.ParamValue == 99999)
                    row.ParamValue = 1;
                else
                    row.ParamValue += 1;

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Param ERROR");
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


        public void updateILSFileSentLog(string fileNo, string type, DateTime createdOn, DateTime completedOn)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ILSFileSentLogDs dataSet = new ILSFileSentLogDs();
                ILSFileSentLogDs.ILSFileSentLogRow row = null;

                IDataSetAdapter ad = getDataSetAdapter("ILSFileSentLogApt", "GetILSFileSentLogByKey");
                ad.SelectCommand.Parameters["@FileNo"].Value = fileNo;

                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.ILSFileSentLog[0];
                    row.CompletedOn = completedOn;
                }
                else
                {
                    row = dataSet.ILSFileSentLog.NewILSFileSentLogRow();
                    row.OutputFileNo = fileNo;
                    row.Type = type;
                    row.CreatedOn = createdOn;
                    row.SetCompletedOnNull();
                    dataSet.ILSFileSentLog.AddILSFileSentLogRow(row);
                }

                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS File Sent Log ERROR");

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

        public ArrayList getOutstandingILSFileSentLog()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSFileSentLogApt", "GetOutstandingILSFileSentLog");

            ILSFileSentLogDs dataSet = new ILSFileSentLogDs();
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();

            foreach (ILSFileSentLogDs.ILSFileSentLogRow row in dataSet.ILSFileSentLog)
            {
                ILSFileSentLogDef def = new ILSFileSentLogDef();
                def.FileNo = row.OutputFileNo;
                def.Type = row.Type;
                def.CreatedOn = row.CreatedOn;
                def.CompletedOn = DateTime.MinValue;
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSOrderCopyDiscrepancyList(string fileNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyDiscrepancyApt", "GetILSOrderCopyDiscrepancy");
            ad.SelectCommand.Parameters["@FileNo"].Value = fileNo;

            ILSOrderCopyDiscrepancyDs dataSet = new ILSOrderCopyDiscrepancyDs();
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();

            foreach (ILSOrderCopyDiscrepancyDs.ILSOrderCopyDiscrepancyRow row in dataSet.ILSOrderCopyDiscrepancy)
            {
                ILSOrderCopyDiscrepancyDef def = new ILSOrderCopyDiscrepancyDef();
                ILSOrderCopyDiscrepancyMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSOrderCopyDetailList(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyDetailApt", "GetILSOrderCopyDetailList");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;

            ILSOrderCopyDetailDs dataSet = new ILSOrderCopyDetailDs();
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();

            foreach (ILSOrderCopyDetailDs.ILSOrderCopyDetailRow row in dataSet.ILSOrderCopyDetail)
            {
                ILSOrderCopyDetailDef def = new ILSOrderCopyDetailDef();
                ILSOrderCopyDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }


        public ArrayList getILSOrderCopyOriginList(string fileNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyOriginApt", "GetILSOrderCopyOrigin");
            ad.SelectCommand.Parameters["@FileNo"].Value = fileNo;

            ILSOrderCopyOriginDs dataSet = new ILSOrderCopyOriginDs();
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();

            foreach (ILSOrderCopyOriginDs.ILSOrderCopyOriginRow row in dataSet.ILSOrderCopyOrigin)
            {
                ILSOrderCopyOriginDef def = new ILSOrderCopyOriginDef();
                ILSOrderCopyOriginMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getMissingILSOrderCopyOriginList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyOriginApt", "GetMissingILSOrderCopyOrigin");

            ILSOrderCopyOriginDs dataSet = new ILSOrderCopyOriginDs();
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();

            foreach (ILSOrderCopyOriginDs.ILSOrderCopyOriginRow row in dataSet.ILSOrderCopyOrigin)
            {
                ILSOrderCopyOriginDef def = new ILSOrderCopyOriginDef();
                ILSOrderCopyOriginMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSUnitPriceMatrix(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSUnitPriceMatrixApt", "GetILSUnitPriceMatrix");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            ILSUnitPriceMatrixDs dataSet = new ILSUnitPriceMatrixDs();
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();

            foreach (ILSUnitPriceMatrixDs.ILSUnitPriceMatrixRow row in dataSet.ILSUnitPriceMatrix)
            {
                ILSUnitPriceMatrixDef def = new ILSUnitPriceMatrixDef();
                ILSUnitPriceMatrixMapping(row, def);
                list.Add(def);
            }
            return list;
        }


        public ArrayList getILSQCCApprovalList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSQCCApprovalApt", "GetILSQCCApproval");

            ILSQCCApprovalDs dataSet = new ILSQCCApprovalDs();
            int recordsAffected = ad.Fill(dataSet);
            ArrayList list = new ArrayList();

            foreach (ILSQCCApprovalDs.ILSQCCApprovalRow row in dataSet.ILSQCCApproval)
            {
                ILSQCCApprovalDef def = new ILSQCCApprovalDef();
                ILSQCCApprovalMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public string getOutputFileNo()
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                int fileNo;
                IDataSetAdapter ad = getDataSetAdapter("ILSParamApt", "GetILSParam");
                ad.SelectCommand.Parameters["@ParamName"].Value = "OUTPUT_FILE_NO";
                ad.PopulateCommands();

                ILSParamDs dataSet = new ILSParamDs();
                ILSParamDs.ILSParamRow row = null;

                int recordsAffected = ad.Fill(dataSet);

                row = dataSet.ILSParam[0];
                fileNo = row.ParamValue;

                if (fileNo == 99999)
                    row.ParamValue = 1;
                else
                    row.ParamValue += 1;
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Param ERROR");
                ctx.VoteCommit();

                return fileNo.ToString().Trim().PadLeft(5, '0');
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


        public ArrayList getILSDocumentListByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSDocumentApt", "GetILSDocumentListByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            ILSDocumentDs dataSet = new ILSDocumentDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSDocumentDs.ILSDocumentRow row in dataSet.ILSDocument)
            {
                ILSDocumentDef def = new ILSDocumentDef();
                this.ILSDocumentMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        private bool isDuplicateShipmentIdExists(int shipmentId, string contractNo, string deliveryNo, int orderRefId)
        {
            bool result = false;
            ILSOrderRefDef def = this.getILSOrderRefByShipmentId(shipmentId);
            if (def != null)
            {
                if (def.OrderRefId != orderRefId)
                {
                    NoticeHelper.sendShipmentIdConflictEmail(contractNo + "-" + deliveryNo, def.OrderRef, shipmentId);
                    result = true;
                }
            }
            return result;
        }

        public int getOrderRefIdByContractNoAndDeliveryNo(string contractNo, string deliveryNo, int nslDeliveryNo)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                int orderRefId = 0;
                int shipmentId = 0;
                IDataSetAdapter ad = getDataSetAdapter("ILSOrderRefApt", "GetILSOrderRefByContractNoAndDeliveryNo");
                ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
                ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;
                ad.PopulateCommands();
                ILSOrderRefDs dataSet = new ILSOrderRefDs();
                ILSOrderRefDs.ILSOrderRefRow row = null;
                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.ILSOrderRef[0];
                    orderRefId = row.OrderRefId;

                    if (nslDeliveryNo != -1)
                    {
                        shipmentId = this.getShipmentIdByContractNoAndDeliveryNo(contractNo, nslDeliveryNo);

                        if (shipmentId == 0)
                            row.SetShipmentIdNull();
                        else
                        {
                            if (isDuplicateShipmentIdExists(shipmentId, contractNo, deliveryNo, orderRefId))
                            {
                                if (!row.IsForced)
                                    row.SetShipmentIdNull();
                                NoticeHelper.sendShipmentIdConflictEmail(contractNo + "-" + deliveryNo, contractNo + "-" + deliveryNo, shipmentId);
                            }
                            else
                            {
                                if (!row.IsForced)
                                    row.ShipmentId = shipmentId;
                            }
                        }
                    }
                    row.ModifiedOn = DateTime.Now;
                    row.ModifiedBy = 99999;
                }
                else
                {
                    row = dataSet.ILSOrderRef.NewILSOrderRefRow();
                    orderRefId = this.getMaxOrderRefId() + 1;
                    row.OrderRefId = orderRefId;
                    row.ContractNo = contractNo;
                    row.DeliveryNo = deliveryNo;
                    shipmentId = this.getShipmentIdByContractNoAndDeliveryNo(contractNo, nslDeliveryNo == -1 ? int.Parse(deliveryNo) : nslDeliveryNo);
                    if (shipmentId == 0)
                        row.SetShipmentIdNull();
                    else
                    {
                        row.ShipmentId = shipmentId;
                        if (isDuplicateShipmentIdExists(shipmentId, contractNo, deliveryNo, orderRefId))
                        {
                            row.SetShipmentIdNull();
                        }
                    }
                    row.IsForced = false;
                    row.IsReset = false;
                    row.IsCancelled = false;
                    row.CreatedOn = DateTime.Now;
                    dataSet.ILSOrderRef.AddILSOrderRefRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Order Ref ERROR [getOrderRefIdByContractNoAndDeliveryNo]");

                if (nslDeliveryNo != -1 && nslDeliveryNo != int.Parse(deliveryNo))
                {
                    ILSOrderCopyDef orderCopyDef = this.getILSOrderCopyByKey(orderRefId);
                    if (orderCopyDef != null)
                    {
                        orderCopyDef.IsValid = false;
                        this.updateILSOrderCopy(orderCopyDef);
                    }
                }
                
                ctx.VoteCommit();
                return orderRefId;
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

        public void updateILSManifestDetail(ILSManifestDetailDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                def.OrderRefId = this.getOrderRefIdByContractNoAndDeliveryNo(def.ContractNo, def.DeliveryNo, -1);
                IDataSetAdapter ad = getDataSetAdapter("ILSManifestDetailApt", "GetILSManifestDetailByKey");
                ad.SelectCommand.Parameters["@ContainerNo"].Value = def.ContainerNo;
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                ILSManifestDetailDs dataSet = new ILSManifestDetailDs();
                ILSManifestDetailDs.ILSManifestDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSManifestDetail[0];
                    this.ILSManifestDetailMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSManifestDetail.NewILSManifestDetailRow();
                    this.ILSManifestDetailMapping(def, row);
                    dataSet.ILSManifestDetail.AddILSManifestDetailRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Manifest Detail ERROR");
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

        public void updateILSDocument(ILSDocumentDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                def.OrderRefId = this.getOrderRefIdByContractNoAndDeliveryNo(def.ContractNo, def.DeliveryNo, -1);
                IDataSetAdapter ad = getDataSetAdapter("ILSDocumentApt", "GetILSDocumentByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.SelectCommand.Parameters["@DocType"].Value = def.DocumentType;
                ad.SelectCommand.Parameters["@DocNo"].Value = def.DocumentNo;
                ad.PopulateCommands();

                ILSDocumentDs dataSet = new ILSDocumentDs();
                ILSDocumentDs.ILSDocumentRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSDocument[0];
                    this.ILSDocumentMapping(def, row);
                    row.IsUploaded = false;
                    row.LastImportDate = row.ImportDate;
                }
                else
                {
                    row = dataSet.ILSDocument.NewILSDocumentRow();
                    this.ILSDocumentMapping(def, row);
                    row.IsUploaded = false;
                    row.CreateDate = def.ImportDate;
                    dataSet.ILSDocument.AddILSDocumentRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Document ERROR");
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

        public void updateILSOrderCopy(ILSOrderCopyDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                def.OrderRefId = this.getOrderRefIdByContractNoAndDeliveryNo(def.ContractNo, def.DeliveryNo, -1);
                IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyApt", "GetILSOrderCopyByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                ILSOrderCopyDs dataSet = new ILSOrderCopyDs();
                ILSOrderCopyDs.ILSOrderCopyRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSOrderCopy[0];
                    this.ILSOrderCopyMapping(def, row);
                    row.LastImportDate = row.ImportDate;
                }
                else
                {
                    row = dataSet.ILSOrderCopy.NewILSOrderCopyRow();
                    this.ILSOrderCopyMapping(def, row);
                    row.CreateDate = def.ImportDate;
                    dataSet.ILSOrderCopy.AddILSOrderCopyRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Order Copy ERROR");
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

        public void updateILSOrderCopyDetail(ILSOrderCopyDetailDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyDetailApt", "GetILSOrderCopyDetailByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.SelectCommand.Parameters["@OptionNo"].Value = def.OptionNo;
                ad.PopulateCommands();

                ILSOrderCopyDetailDs dataSet = new ILSOrderCopyDetailDs();
                ILSOrderCopyDetailDs.ILSOrderCopyDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSOrderCopyDetail[0];
                    this.ILSOrderCopyDetailMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSOrderCopyDetail.NewILSOrderCopyDetailRow();
                    this.ILSOrderCopyDetailMapping(def, row);
                    dataSet.ILSOrderCopyDetail.AddILSOrderCopyDetailRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Order Copy Detail ERROR");
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

        public void updateILSPackingList(ILSPackingListDef def, int nslDeliveryNo)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                def.OrderRefId = this.getOrderRefIdByContractNoAndDeliveryNo(def.ContractNo, def.DeliveryNo, nslDeliveryNo);
                IDataSetAdapter ad = getDataSetAdapter("ILSPackingListApt", "GetILSPackingListByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                ILSPackingListDs dataSet = new ILSPackingListDs();
                ILSPackingListDs.ILSPackingListRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSPackingList[0];
                    this.ILSPackingListMapping(def, row);
                    row.LastImportDate = row.ImportDate;
                }
                else
                {
                    row = dataSet.ILSPackingList.NewILSPackingListRow();
                    def.IsUploaded = false;
                    this.ILSPackingListMapping(def, row);
                    row.CreateDate = def.ImportDate;
                    dataSet.ILSPackingList.AddILSPackingListRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Packing List ERROR");
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

        public void updateILSPackingListDetail(ILSPackingListDetailDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSPackingListDetailApt", "GetILSPackingListDetailByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.SelectCommand.Parameters["@OptionNo"].Value = def.OptionNo;
                ad.PopulateCommands();

                ILSPackingListDetailDs dataSet = new ILSPackingListDetailDs();
                ILSPackingListDetailDs.ILSPackingListDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSPackingListDetail[0];
                    this.ILSPackingListDetailMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSPackingListDetail.NewILSPackingListDetailRow();
                    this.ILSPackingListDetailMapping(def, row);
                    dataSet.ILSPackingListDetail.AddILSPackingListDetailRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Packing List Detail ERROR");
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

        public void updateILSPackingListCartonDetail(ILSPackingListCartonDetailDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSPackingListCartonDetailApt", "GetILSPackingListCartonDetailByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.SelectCommand.Parameters["@OptionNo"].Value = def.OptionNo;
                ad.SelectCommand.Parameters["@SeqNo"].Value = def.SeqNo;
                ad.PopulateCommands();

                ILSPackingListCartonDetailDs dataSet = new ILSPackingListCartonDetailDs();
                ILSPackingListCartonDetailDs.ILSPackingListCartonDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSPackingListCartonDetail[0];
                    this.ILSPackingListCartonDetailMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSPackingListCartonDetail.NewILSPackingListCartonDetailRow();
                    def.SeqNo = this.getMaxILSPackingListCartonDetailSeqNo(def.OrderRefId, def.OptionNo) + 1;
                    this.ILSPackingListCartonDetailMapping(def, row);
                    dataSet.ILSPackingListCartonDetail.AddILSPackingListCartonDetailRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Packing List Carton Detail ERROR");
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

        public void updateILSPackingListMixedCartonDetail(ILSPackingListMixedCartonDetailDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSPackingListMixedCartonDetailApt", "GetILSPackingListMixedCartonDetailByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.SelectCommand.Parameters["@CartonId"].Value = def.CartonId;
                ad.PopulateCommands();

                ILSPackingListMixedCartonDetailDs dataSet = new ILSPackingListMixedCartonDetailDs();
                ILSPackingListMixedCartonDetailDs.ILSPackingListMixedCartonDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSPackingListMixedCartonDetail[0];
                    this.ILSPackingListMixedCartonDetailMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSPackingListMixedCartonDetail.NewILSPackingListMixedCartonDetailRow();
                    this.ILSPackingListMixedCartonDetailMapping(def, row);
                    dataSet.ILSPackingListMixedCartonDetail.AddILSPackingListMixedCartonDetailRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Packing List Mixed Carton Detail ERROR");
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

        public void updateILSCommissionInvoice(ILSCommissionInvoiceDef def)
        {
            ILSInvoiceDef invoiceDef = this.getILSInvoiceByInvoiceNo(def.InvoiceNo.Substring(0, 14));
            if (invoiceDef == null)
            {
                NoticeHelper.sendILSCommissionInvoiceNotMatchEmail(def.InvoiceNo, def.FileNo);
                return;
            }

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                def.OrderRefId = invoiceDef.OrderRefId;
                IDataSetAdapter ad = getDataSetAdapter("ILSCommissionInvoiceApt", "GetILSCommissionInvoiceByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                ILSCommissionInvoiceDs dataSet = new ILSCommissionInvoiceDs();
                ILSCommissionInvoiceDs.ILSCommissionInvoiceRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSCommissionInvoice[0];
                    this.ILSCommissionInvoiceMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSCommissionInvoice.NewILSCommissionInvoiceRow();
                    def.Status = -1;
                    this.ILSCommissionInvoiceMapping(def, row);
                    dataSet.ILSCommissionInvoice.AddILSCommissionInvoiceRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Commission Invoice ERROR");
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

        /*
        public void updateILSCancelledInvoice(ILSCancelledInvoiceDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                def.OrderRefId = this.getOrderRefIdByContractNoAndDeliveryNo(def.ContractNo, def.DeliveryNo, -1);
                IDataSetAdapter ad = getDataSetAdapter("ILSCancelledInvoiceApt", "GetILSCancelledInvoiceByKey");

                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                ILSCancelledInvoiceDs dataSet = new ILSCancelledInvoiceDs();
                ILSCancelledInvoiceDs.ILSCancelledInvoiceRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSCancelledInvoice[0];
                    if ((row.TotalAmt != def.TotalAmount || row.Currency != def.Currency || row.InvoiceNo != def.InvoiceNo || row.InvoiceDate != def.InvoiceDate)
                        && row.IsCancelled == false)
                    {
                        IDataSetAdapter adHistory = getDataSetAdapter("ILSCancelledInvoiceChangeHistoryApt", "GetILSCancelledInvoiceChangeHistoryByKey");
                        adHistory.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                        adHistory.SelectCommand.Parameters["@SeqId"].Value = 0;
                        adHistory.PopulateCommands();

                        ILSCancelledInvoiceChangeHistoryDs historyDs = new ILSCancelledInvoiceChangeHistoryDs();
                        ILSCancelledInvoiceChangeHistoryDs.ILSCancelledInvoiceChangeHistoryRow historyRow = historyDs.ILSCancelledInvoiceChangeHistory.NewILSCancelledInvoiceChangeHistoryRow();

                        adHistory.Fill(dataSet);

                        historyRow.OrderRefId = row.OrderRefId;
                        historyRow.SeqId = this.getMaxILSCancelledInvoiceChangeHistorySeqId(row.OrderRefId) + 1;
                        historyRow.InvoiceNo = row.InvoiceNo;
                        historyRow.InvoiceDate = row.InvoiceDate;
                        historyRow.Currency = row.Currency;
                        historyRow.TotalAmt = row.TotalAmt;
                        historyRow.FileNo = row.FileNo;
                        historyRow.ImportDate = row.ImportDate;
                        historyRow.IsLatest = true;
                        historyRow.IsAdjustmentRequired = false;
                        if (row.TotalAmt != def.TotalAmount || row.Currency != def.Currency)
                            historyRow.IsAdjustmentRequired = true;
                        else
                        {
                            NoticeHelper.sendILSCancelledInvoiceNoChangeEmail(def.ContractNo + "-" + def.DeliveryNo, row.InvoiceNo, def.InvoiceNo, def.OrderRefId, def.Currency, def.TotalAmount);
                        }
                        historyDs.ILSCancelledInvoiceChangeHistory.AddILSCancelledInvoiceChangeHistoryRow(historyRow);
                        adHistory.Update(historyDs);
                        this.resetOutOfDateILSCancelledInvoiceChangeHistory(row.OrderRefId, historyRow.SeqId);
                    }
                    def.Status = -1;
                    this.ILSCancelledInvoiceMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSCancelledInvoice.NewILSCancelledInvoiceRow();
                    def.Status = -1;
                    def.ProcessedDate = DateTime.MinValue;
                    this.ILSCancelledInvoiceMapping(def, row);
                    dataSet.ILSCancelledInvoice.AddILSCancelledInvoiceRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Cancelled Invoice ERROR");
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
        */

        public void updateILSInvoice(ILSInvoiceDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                def.OrderRefId = this.getOrderRefIdByContractNoAndDeliveryNo(def.ContractNo, def.DeliveryNo, -1);
                ILSOrderRefDef orderRefDef = this.getILSOrderRefByKey(def.OrderRefId);
 
                IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceApt", "GetILSInvoiceByKey");

                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                ILSInvoiceDs dataSet = new ILSInvoiceDs();
                ILSInvoiceDs.ILSInvoiceRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSInvoice[0];
                    if ((row.TotalAmt != def.TotalAmount || row.Currency != def.Currency || row.InvoiceNo != def.InvoiceNo || row.InvoiceDate != def.InvoiceDate))
                        //&& row.IsCancelled == false)
                    {
                        IDataSetAdapter adHistory = getDataSetAdapter("ILSInvoiceChangeHistoryApt", "GetILSInvoiceChangeHistoryByKey");
                        adHistory.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                        adHistory.SelectCommand.Parameters["@SeqId"].Value = 0;
                        adHistory.PopulateCommands();

                        ILSInvoiceChangeHistoryDs historyDs = new ILSInvoiceChangeHistoryDs();
                        ILSInvoiceChangeHistoryDs.ILSInvoiceChangeHistoryRow historyRow = historyDs.ILSInvoiceChangeHistory.NewILSInvoiceChangeHistoryRow();

                        adHistory.Fill(dataSet);

                        historyRow.OrderRefId = row.OrderRefId;
                        historyRow.SeqId = this.getMaxILSInvoiceChangeHistorySeqId(row.OrderRefId) + 1;
                        historyRow.InvoiceNo = row.InvoiceNo;
                        historyRow.InvoiceDate = row.InvoiceDate;
                        historyRow.Currency = row.Currency;
                        historyRow.TotalAmt = row.TotalAmt;
                        historyRow.FileNo = row.FileNo;
                        historyRow.ImportDate = row.ImportDate;
                        historyRow.IsLatest = true;
                        historyRow.IsAdjustmentRequired = false;

                        if (def.TotalAmount >= 0)
                            NoticeHelper.sendILSInvoiceNoChangeEmail(def.ContractNo + "-" + def.DeliveryNo, row.InvoiceNo, def.InvoiceNo, def.OrderRefId, def.Currency, def.TotalAmount);
                        else
                        {
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(orderRefDef.ShipmentId, 0, ActionHistoryType.ILS_INVOICE_CANCELLATION, def.InvoiceNo, GeneralStatus.ACTIVE.Code, 99999));

                            NoticeHelper.sendILSCancelledInvoiceEmail(def.ContractNo + "-" + def.DeliveryNo, row.InvoiceNo, def.InvoiceNo, def.OrderRefId, def.Currency, def.TotalAmount);
                        }

                        /*
                        if (row.TotalAmt != def.TotalAmount || row.Currency != def.Currency)
                            historyRow.IsAdjustmentRequired = true;
                        else
                        {
                            NoticeHelper.sendILSInvoiceNoChangeEmail(def.ContractNo + "-" + def.DeliveryNo, row.InvoiceNo, def.InvoiceNo, def.OrderRefId, def.Currency, def.TotalAmount);
                        }
                        */

                        historyDs.ILSInvoiceChangeHistory.AddILSInvoiceChangeHistoryRow(historyRow);
                        adHistory.Update(historyDs);
                        this.resetOutOfDateILSInvoiceChangeHistory(row.OrderRefId, historyRow.SeqId);
                    }
                    if (def.Status == ILSInvoiceUploadStatus.PENDING.Id)
                    {
                        def.Status = -1;
                        row.SetStatusNull();
                    }

                    if (!row.IsStatusNull())
                    {
                        if (row.Status == ILSInvoiceUploadStatus.OPTION_MISMATCH.Id || row.Status == ILSInvoiceUploadStatus.QUANTITY_MISMATCH.Id)
                            def.Status = ILSInvoiceUploadStatus.PENDING.Id;
                        else
                            def.Status = row.Status;
                    }
                    this.ILSInvoiceMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSInvoice.NewILSInvoiceRow();
                    def.Status = -1;
                    def.ProcessedDate = DateTime.MinValue;
                    this.ILSInvoiceMapping(def, row);
                    dataSet.ILSInvoice.AddILSInvoiceRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Invoice ERROR");
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

        private void resetOutOfDateILSInvoiceChangeHistory(int orderRefId, int latestSeqId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceChangeHistoryApt", "GetOutOfDateILSInvoiceChangeHistory");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
            ad.SelectCommand.Parameters["@LatestSeqId"].Value = latestSeqId;
            ad.PopulateCommands();

            ILSInvoiceChangeHistoryDs dataSet = new ILSInvoiceChangeHistoryDs();
            int recordsAffected = ad.Fill(dataSet);
            
            foreach (ILSInvoiceChangeHistoryDs.ILSInvoiceChangeHistoryRow row in dataSet.ILSInvoiceChangeHistory)
            {
                row.IsAdjustmentRequired = false;
                row.IsLatest = false;
            }
            ad.Update(dataSet);
        }

        /*
        private void resetOutOfDateILSCancelledInvoiceChangeHistory(int orderRefId, int latestSeqId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSCancelledInvoiceChangeHistoryApt", "GetOutOfDateILSCancelledInvoiceChangeHistory");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
            ad.SelectCommand.Parameters["@LatestSeqId"].Value = latestSeqId;
            ad.PopulateCommands();

            ILSCancelledInvoiceChangeHistoryDs dataSet = new ILSCancelledInvoiceChangeHistoryDs();
            int recordsAffected = ad.Fill(dataSet);

            foreach (ILSCancelledInvoiceChangeHistoryDs.ILSCancelledInvoiceChangeHistoryRow row in dataSet.ILSCancelledInvoiceChangeHistory)
            {
                row.IsAdjustmentRequired = false;
                row.IsLatest = false;
            }
            ad.Update(dataSet);
        }
        */

        private int getMaxILSInvoiceChangeHistorySeqId(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceChangeHistoryApt", "GetMaxSeqId");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        /*
        private int getMaxILSCancelledInvoiceChangeHistorySeqId(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSCancelledInvoiceChangeHistoryApt", "GetMaxSeqId");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }
        */

        private int getMaxILSCommissionInvoiceDetailSeqId(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSCommissionInvoiceDetailApt", "GetMaxSeqId");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public void updateILSCommissionInvoiceDetail(ILSCommissionInvoiceDetailDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSCommissionInvoiceDetailApt", "GetILSCommissionInvoiceDetailByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.SelectCommand.Parameters["@SeqId"].Value = def.SeqId;
                ad.PopulateCommands();

                ILSCommissionInvoiceDetailDs dataSet = new ILSCommissionInvoiceDetailDs();
                ILSCommissionInvoiceDetailDs.ILSCommissionInvoiceDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSCommissionInvoiceDetail[0];
                    this.ILSCommissionInvoiceDetailMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSCommissionInvoiceDetail.NewILSCommissionInvoiceDetailRow();
                    def.SeqId = this.getMaxILSCommissionInvoiceDetailSeqId(def.OrderRefId) + 1;
                    this.ILSCommissionInvoiceDetailMapping(def, row);
                    dataSet.ILSCommissionInvoiceDetail.AddILSCommissionInvoiceDetailRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Commission Invoice Detail ERROR");
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

        public void updateILSInvoiceDetail(ILSInvoiceDetailDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceDetailApt", "GetILSInvoiceDetailByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.SelectCommand.Parameters["@OptionNo"].Value = def.OptionNo;
                ad.PopulateCommands();

                ILSInvoiceDetailDs dataSet = new ILSInvoiceDetailDs();
                ILSInvoiceDetailDs.ILSInvoiceDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSInvoiceDetail[0];
                    this.ILSInvoiceDetailMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSInvoiceDetail.NewILSInvoiceDetailRow();
                    this.ILSInvoiceDetailMapping(def, row);
                    dataSet.ILSInvoiceDetail.AddILSInvoiceDetailRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Invoice Detail ERROR");
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

        /*
        public void updateILSCancelledInvoiceDetail(ILSCancelledInvoiceDetailDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSCancelledInvoiceDetailApt", "GetILSCancelledInvoiceDetailByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.SelectCommand.Parameters["@OptionNo"].Value = def.OptionNo;
                ad.PopulateCommands();

                ILSCancelledInvoiceDetailDs dataSet = new ILSCancelledInvoiceDetailDs();
                ILSCancelledInvoiceDetailDs.ILSCancelledInvoiceDetailRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSCancelledInvoiceDetail[0];
                    this.ILSCancelledInvoiceDetailMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSCancelledInvoiceDetail.NewILSCancelledInvoiceDetailRow();
                    this.ILSCancelledInvoiceDetailMapping(def, row);
                    dataSet.ILSCancelledInvoiceDetail.AddILSCancelledInvoiceDetailRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Cancelled Invoice Detail ERROR");
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
        */

        /*
        public ILSCancelledInvoiceDef convertToILSCancelledInvoice(ILSInvoiceDef invoiceDef)
        {
            ILSCancelledInvoiceDef def = new ILSCancelledInvoiceDef();
            def.ContractNo = invoiceDef.ContractNo;
            def.Currency = invoiceDef.Currency;
            def.DeliveryNo = invoiceDef.DeliveryNo;
            def.FileNo = invoiceDef.FileNo;
            def.ImportDate = invoiceDef.ImportDate;
            def.InvoiceDate = invoiceDef.InvoiceDate;
            def.InvoiceNo = invoiceDef.InvoiceNo;
            def.IsCancelled = invoiceDef.IsCancelled;
            def.ItemNo = invoiceDef.ItemNo;
            def.OrderRefId = invoiceDef.OrderRefId;
            def.ProcessedDate = invoiceDef.ProcessedDate;
            def.Status = invoiceDef.Status;
            def.SupplierCode = invoiceDef.SupplierCode;
            def.TotalAmount = invoiceDef.TotalAmount;
            def.TotalQty = invoiceDef.TotalQty;
            def.TotalVAT = invoiceDef.TotalVAT;
            return def;
        }

        public ILSCancelledInvoiceDetailDef convertToILSCancelledInvoiceDetail(ILSInvoiceDetailDef invoiceDetailDef)
        {
            ILSCancelledInvoiceDetailDef def = new ILSCancelledInvoiceDetailDef();
            def.OptionNo = invoiceDetailDef.OptionNo;
            def.OrderRefId = invoiceDetailDef.OrderRefId;
            def.Price = invoiceDetailDef.Price;
            def.Qty = invoiceDetailDef.Qty;
            def.VATCode = invoiceDetailDef.VATCode;
            return def;
        }
        */

        public void updateILSManifest(ILSManifestDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                IDataSetAdapter ad = null;

                if (def.IsTranshipment)
                {
                    ad = getDataSetAdapter("ILSManifestApt", "GetILSManifestByKey");
                    ad.SelectCommand.Parameters["@ContainerNo"].Value = def.ContainerNo;
                    ad.SelectCommand.Parameters["@LegId"].Value = 1;
                }
                else
                {
                    ad = getDataSetAdapter("ILSManifestApt", "GetILSManifestByMaxLegId");
                    ad.SelectCommand.Parameters["@ContainerNo"].Value = def.ContainerNo;
                }
                ad.PopulateCommands();

                ILSManifestDs dataSet = new ILSManifestDs();
                ILSManifestDs.ILSManifestRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSManifest[0];
                    if (row.IsTranshipment == true && def.IsTranshipment == false)
                    {
                        def.LegId = 2;
                        row = dataSet.ILSManifest.NewILSManifestRow();
                        this.ILSManifestMapping(def, row);
                        row.CreateDate = def.ImportDate;
                        row.IsUploaded = true;
                        dataSet.ILSManifest.AddILSManifestRow(row);
                    }
                    else
                    {
                        def.LegId = row.LegId;
                        this.ILSManifestMapping(def, row);
                        row.IsUploaded = false;
                        row.LastImportDate = row.ImportDate;
                    }
                }
                else
                {
                    row = dataSet.ILSManifest.NewILSManifestRow();
                    def.LegId = 1;
                    this.ILSManifestMapping(def, row);
                    row.CreateDate = def.ImportDate;
                    row.IsUploaded = false;
                    dataSet.ILSManifest.AddILSManifestRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Manifest ERROR");
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

        public void removeILSOrderCopyDetails(int orderRefId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyDetailApt", "GetILSOrderCopyDetailList");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
                ad.PopulateCommands();

                ILSOrderCopyDetailDs dataSet = new ILSOrderCopyDetailDs();
                int recordsAffected = ad.Fill(dataSet);
                foreach (ILSOrderCopyDetailDs.ILSOrderCopyDetailRow row in dataSet.ILSOrderCopyDetail.Rows)
                {
                    row.Delete();
                }
                recordsAffected = ad.Update(dataSet);
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

        public void removeILSInvoice(int orderRefId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceApt", "GetILSInvoiceByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
                ad.PopulateCommands();

                ILSInvoiceDs dataSet = new ILSInvoiceDs();
                ILSInvoiceDs.ILSInvoiceRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSInvoice[0];
                    row.Delete();
                    recordsAffected = ad.Update(dataSet);
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

        public void removeILSInvoiceDetails(int orderRefId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceDetailApt", "GetILSInvoiceDetailList");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
                ad.PopulateCommands();

                ILSInvoiceDetailDs dataSet = new ILSInvoiceDetailDs();
                int recordsAffected = ad.Fill(dataSet);
                foreach (ILSInvoiceDetailDs.ILSInvoiceDetailRow row in dataSet.ILSInvoiceDetail.Rows)
                {
                    row.Delete();
                }
                recordsAffected = ad.Update(dataSet);
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

        /*
        public void removeILSCancelledInvoiceDetails(int orderRefId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSCancelledInvoiceDetailApt", "GetILSCancelledInvoiceDetailList");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
                ad.PopulateCommands();

                ILSCancelledInvoiceDetailDs dataSet = new ILSCancelledInvoiceDetailDs();
                int recordsAffected = ad.Fill(dataSet);
                foreach (ILSCancelledInvoiceDetailDs.ILSCancelledInvoiceDetailRow row in dataSet.ILSCancelledInvoiceDetail.Rows)
                {
                    row.Delete();
                }
                recordsAffected = ad.Update(dataSet);
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
        */

        public void removeILSCommissionInvoiceDetails(int orderRefId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSCommissionInvoiceDetailApt", "GetILSCommissionInvoiceDetailList");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
                ad.PopulateCommands();

                ILSCommissionInvoiceDetailDs dataSet = new ILSCommissionInvoiceDetailDs();
                int recordsAffected = ad.Fill(dataSet);
                foreach (ILSCommissionInvoiceDetailDs.ILSCommissionInvoiceDetailRow row in dataSet.ILSCommissionInvoiceDetail.Rows)
                {
                    row.Delete();
                }
                recordsAffected = ad.Update(dataSet);
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

        public void removeILSPackingList(int orderRefId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSPackingListApt", "GetILSPackingListByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
                ad.PopulateCommands();

                ILSPackingListDs dataSet = new ILSPackingListDs();
                ILSPackingListDs.ILSPackingListRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSPackingList[0];
                    row.Delete();
                    recordsAffected = ad.Update(dataSet);
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

        public void removeILSPackingListDetails(int orderRefId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSPackingListDetailApt", "GetILSPackingListDetailList");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
                ad.PopulateCommands();

                ILSPackingListDetailDs dataSet = new ILSPackingListDetailDs();
                int recordsAffected = ad.Fill(dataSet);
                foreach (ILSPackingListDetailDs.ILSPackingListDetailRow row in dataSet.ILSPackingListDetail.Rows)
                {
                    row.Delete();
                }
                recordsAffected = ad.Update(dataSet);
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

        public void removeILSPackingListCartonDetails(int orderRefId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSPackingListCartonDetailApt", "GetILSPackingListCartonDetailListByOrderRefId");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
                ad.PopulateCommands();

                ILSPackingListCartonDetailDs dataSet = new ILSPackingListCartonDetailDs();
                int recordsAffected = ad.Fill(dataSet);
                foreach (ILSPackingListCartonDetailDs.ILSPackingListCartonDetailRow row in dataSet.ILSPackingListCartonDetail.Rows)
                {
                    row.Delete();
                }
                recordsAffected = ad.Update(dataSet);
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

        public void removeILSPackingListMixedCartonDetails(int orderRefId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSPackingListMixedCartonDetailApt", "GetILSPackingListMixedCartonDetailListByOrderRefId");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
                ad.PopulateCommands();

                ILSPackingListMixedCartonDetailDs dataSet = new ILSPackingListMixedCartonDetailDs();
                int recordsAffected = ad.Fill(dataSet);
                foreach (ILSPackingListMixedCartonDetailDs.ILSPackingListMixedCartonDetailRow row in dataSet.ILSPackingListMixedCartonDetail.Rows)
                {
                    row.Delete();
                }
                recordsAffected = ad.Update(dataSet);
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

        public void updateILSOrderRef(ILSOrderRefDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("ILSOrderRefApt", "GetILSOrderRefByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                ILSOrderRefDs dataSet = new ILSOrderRefDs();
                ILSOrderRefDs.ILSOrderRefRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSOrderRef[0];
                    this.ILSOrderRefMapping(def, row);
                    recordsAffected = ad.Update(dataSet);
                }

                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Order Ref ERROR");
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

        public void updateILSCancelledOrder(ILSCancelledOrderDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                def.OrderRefId = this.getOrderRefIdByContractNoAndDeliveryNo(def.ContractNo, def.DeliveryNo, -1);
                IDataSetAdapter ad = getDataSetAdapter("ILSCancelledOrderApt", "GetILSCancelledOrderByKey");
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                ILSCancelledOrderDs dataSet = new ILSCancelledOrderDs();
                ILSCancelledOrderDs.ILSCancelledOrderRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.ILSCancelledOrder[0];
                    this.ILSCancelledOrderMapping(def, row);
                }
                else
                {
                    row = dataSet.ILSCancelledOrder.NewILSCancelledOrderRow();
                    this.ILSCancelledOrderMapping(def, row);
                    dataSet.ILSCancelledOrder.AddILSCancelledOrderRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update ILS Cancelled Order ERROR");

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(def.OrderRefId);
                ilsOrderRef.IsCancelled = true;
                this.updateILSOrderRef(ilsOrderRef);

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

        private int getMaxILSPackingListCartonDetailSeqNo(int orderRefId, string optionNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSPackingListCartonDetailApt", "GetMaxSeqNo");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
            ad.SelectCommand.Parameters["@OptionNo"].Value = optionNo;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        private int getMaxOrderRefId()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderRefApt", "GetMaxOrderRefId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public int getShipmentIdByContractNoAndDeliveryNo(string contractNo, int deliveryNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSParamApt", "GetShipmentIdByContractNoAndDeliveryNo");
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@DeliveryNo"].Value = deliveryNo;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected < 1) return 0;
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public ILSOrderRefDef getILSOrderRefByKey(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderRefApt", "GetILSOrderRefByKey");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;

            ILSOrderRefDs dataSet = new ILSOrderRefDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSOrderRefDef def = new ILSOrderRefDef();
            this.ILSOrderRefMapping(dataSet.ILSOrderRef[0], def);
            return def;
        }

        public ILSOrderRefDef getILSOrderRefByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderRefApt", "GetILSOrderRefByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            ILSOrderRefDs dataSet = new ILSOrderRefDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSOrderRefDef def = new ILSOrderRefDef();
            this.ILSOrderRefMapping(dataSet.ILSOrderRef[0], def);
            return def;
        }

        public ArrayList getILSOrderCopyWithPortAmendmentList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyApt", "GetILSOrderCopyWithPortAmendment");

            ILSOrderCopyDs dataSet = new ILSOrderCopyDs();
            int recordsAffected = ad.Fill(dataSet);

            ILSOrderCopyDef def = null;
            ArrayList list = new ArrayList();

            foreach (ILSOrderCopyDs.ILSOrderCopyRow row in dataSet.ILSOrderCopy)
            {
                def = new ILSOrderCopyDef();
                this.ILSOrderCopyMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSOrderCopyWithOfficeAmendmentList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyApt", "GetILSOrderCopyWithOfficeAmendment");

            ILSOrderCopyDs dataSet = new ILSOrderCopyDs();
            int recordsAffected = ad.Fill(dataSet);

            ILSOrderCopyDef def = null;
            ArrayList list = new ArrayList();

            foreach (ILSOrderCopyDs.ILSOrderCopyRow row in dataSet.ILSOrderCopy)
            {
                def = new ILSOrderCopyDef();
                this.ILSOrderCopyMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSOrderCopyWithOriginCountryAmendmentList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyApt", "GetILSOrderCopyWithOriginCountryAmendment");

            ILSOrderCopyDs dataSet = new ILSOrderCopyDs();
            int recordsAffected = ad.Fill(dataSet);

            ILSOrderCopyDef def = null;
            ArrayList list = new ArrayList();

            foreach (ILSOrderCopyDs.ILSOrderCopyRow row in dataSet.ILSOrderCopy)
            {
                def = new ILSOrderCopyDef();
                this.ILSOrderCopyMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ILSOrderCopyDef getILSOrderCopyByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyApt", "GetILSOrderCopyByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            ILSOrderCopyDs dataSet = new ILSOrderCopyDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSOrderCopyDef def = new ILSOrderCopyDef();
            this.ILSOrderCopyMapping(dataSet.ILSOrderCopy[0], def);
            return def;
        }

        public ILSPackingListDef getILSPackingListByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSPackingListApt", "GetILSPackingListByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            ILSPackingListDs dataSet = new ILSPackingListDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSPackingListDef def = new ILSPackingListDef();
            this.ILSPackingListMapping(dataSet.ILSPackingList[0], def);
            return def;
        }

        public ILSInvoiceDef getILSInvoiceByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceApt", "GetILSInvoiceByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            ILSInvoiceDs dataSet = new ILSInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSInvoiceDef def = new ILSInvoiceDef();
            this.ILSInvoiceMapping(dataSet.ILSInvoice[0], def);
            return def;
        }

        public ILSManifestDef getILSManifestByKey(string containerNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSManifestApt", "GetILSManifestByKey");
            ad.SelectCommand.Parameters["@ContainerNo"].Value = containerNo;
            ad.SelectCommand.Parameters["@LegId"].Value = 1;

            ILSManifestDs dataSet = new ILSManifestDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSManifestDef def = new ILSManifestDef();
            this.ILSManifestMapping(dataSet.ILSManifest[0], def);
            return def;
        }

        public ArrayList getILSManifestList(string voyageNo, DateTime departDate, string contractNo, string departPort, string vesselName)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSManifestApt", "GetILSManifestList");
            ad.SelectCommand.Parameters["@VoyageNo"].Value = voyageNo;
            if (departDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@DepartDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@DepartDate"].Value = departDate;

            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.Parameters["@DepartPort"].Value = departPort;
            ad.SelectCommand.Parameters["@vesselName"].Value = vesselName;

            ILSManifestDs dataSet = new ILSManifestDs();
            int recordsAffected = ad.Fill(dataSet);            

            ILSManifestDef def = null;
            ArrayList list = new ArrayList();

            foreach (ILSManifestDs.ILSManifestRow row in dataSet.ILSManifest)
            {
                def = new ILSManifestDef();
                this.ILSManifestMapping(row, def);
                list.Add(def);
            }
            
            return list;
        }

        public ILSPackingListDef getILSPackingListByKey(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSPackingListApt", "GetILSPackingListByKey");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;

            ILSPackingListDs dataSet = new ILSPackingListDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSPackingListDef def = new ILSPackingListDef();
            this.ILSPackingListMapping(dataSet.ILSPackingList[0], def);
            return def;
        }
        

        public ArrayList getOutstandingILSPackingList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSPackingListApt", "GetOutstandingILSPackingList");

            ILSPackingListDs dataSet = new ILSPackingListDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSPackingListDs.ILSPackingListRow row in dataSet.ILSPackingList)
            {
                ILSPackingListDef def = new ILSPackingListDef();
                this.ILSPackingListMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ILSPackingListDetailDef getILSPackingListDetailByKey(int orderRefId, string optionNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSPackingListDetailApt", "GetILSPackingListDetailByKey");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
            ad.SelectCommand.Parameters["@OptionNo"].Value = optionNo;

            ILSPackingListDetailDs dataSet = new ILSPackingListDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSPackingListDetailDef def = new ILSPackingListDetailDef();
            this.ILSPackingListDetailMapping(dataSet.ILSPackingListDetail[0], def);
            return def;
        }

        public ArrayList getILSPackingListDetail(int orderRefId, bool excludeZero)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSPackingListDetailApt", "GetILSPackingListDetailList");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;

            ILSPackingListDetailDs dataSet = new ILSPackingListDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSPackingListDetailDs.ILSPackingListDetailRow row in dataSet.ILSPackingListDetail)
            {
                if (!excludeZero || (excludeZero && row.Qty > 0))
                {
                    ILSPackingListDetailDef def = new ILSPackingListDetailDef();
                    this.ILSPackingListDetailMapping(row, def);
                    list.Add(def);
                }
            }
            return list;
        }

        public ILSOrderCopyDetailDef getILSOrderCopyDetailByKey(int orderRefId, string optionNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyDetailApt", "GetILSOrderCopyDetailByKey");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
            ad.SelectCommand.Parameters["@OptionNo"].Value = optionNo;

            ILSOrderCopyDetailDs dataSet = new ILSOrderCopyDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSOrderCopyDetailDef def = new ILSOrderCopyDetailDef();
            this.ILSOrderCopyDetailMapping(dataSet.ILSOrderCopyDetail[0], def);
            return def;
        }

        public ILSOrderCopyDef getILSOrderCopyByKey(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderCopyApt", "GetILSOrderCopyByKey");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;

            ILSOrderCopyDs dataSet = new ILSOrderCopyDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSOrderCopyDef def = new ILSOrderCopyDef();
            this.ILSOrderCopyMapping(dataSet.ILSOrderCopy[0], def);
            return def;
        }

        public ILSErrorRef getILSErrorByKey(int errorNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSErrorApt", "GetILSErrorByKey");
            ad.SelectCommand.Parameters["@ErrorNo"].Value = errorNo;

            ILSErrorDs dataSet = new ILSErrorDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSErrorRef def = new ILSErrorRef();
            this.ILSErrorMapping(dataSet.ILSError[0], def);
            return def;
        }

        public ILSManifestDetailDef getILSManifestDetailByKey(string containerNo, int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSManifestDetailApt", "getILSManifestDetailByKey");
            ad.SelectCommand.Parameters["@ContainerNo"].Value = containerNo;
            ad.SelectCommand.Parameters["@orderRefId"].Value = orderRefId;

            ILSManifestDetailDs dataSet = new ILSManifestDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSManifestDetailDef def = new ILSManifestDetailDef();
            this.ILSManifestDetailMapping(dataSet.ILSManifestDetail[0], def);
            return def;
        }

        public ArrayList getILSManifestDetailList(string containerNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSManifestDetailApt", "GetILSManifestDetailList");
            ad.SelectCommand.Parameters["@ContainerNo"].Value = containerNo;

            ILSManifestDetailDs dataSet = new ILSManifestDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSManifestDetailDs.ILSManifestDetailRow row in dataSet.ILSManifestDetail)
            {
                ILSManifestDetailDef def = new ILSManifestDetailDef();
                this.ILSManifestDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSManifestDetailListByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSManifestDetailApt", "GetILSManifestDetailByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;

            ILSManifestDetailDs dataSet = new ILSManifestDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSManifestDetailDs.ILSManifestDetailRow row in dataSet.ILSManifestDetail)
            {
                ILSManifestDetailDef def = new ILSManifestDetailDef();
                this.ILSManifestDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSPackingListCartonDetailList(int orderRefId, string optionNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSPackingListCartonDetailApt", "GetILSPackingListCartonDetailList");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;
            ad.SelectCommand.Parameters["@OptionNo"].Value = optionNo;

            ILSPackingListCartonDetailDs dataSet = new ILSPackingListCartonDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSPackingListCartonDetailDs.ILSPackingListCartonDetailRow row in dataSet.ILSPackingListCartonDetail)
            {
                ILSPackingListCartonDetailDef def = new ILSPackingListCartonDetailDef();
                this.ILSPackingListCartonDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ILSCancelledOrderDef getILSCancelledOrderByKey(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSCancelledOrderApt", "GetILSCancelledOrderByKey");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;

            ILSCancelledOrderDs dataSet = new ILSCancelledOrderDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSCancelledOrderDef def = new ILSCancelledOrderDef();
            this.ILSCancelledOrderMapping(dataSet.ILSCancelledOrder[0], def);
            return def;
        }

        public ArrayList getILSCancelledOrderList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSCancelledOrderApt", "GetILSCancelledOrderList");

            ILSCancelledOrderDs dataSet = new ILSCancelledOrderDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSCancelledOrderDs.ILSCancelledOrderRow row in dataSet.ILSCancelledOrder)
            {
                ILSCancelledOrderDef def = new ILSCancelledOrderDef();
                this.ILSCancelledOrderMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ILSInvoiceDef getILSInvoiceByKey(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceApt", "GetILSInvoiceByKey");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;

            ILSInvoiceDs dataSet = new ILSInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSInvoiceDef def = new ILSInvoiceDef();
            this.ILSInvoiceMapping(dataSet.ILSInvoice[0], def);
            return def;
        }

        public ILSInvoiceDef getILSReplacementInvoice(string invoiceNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceApt", "GetILSReplacementInvoice");
            ad.SelectCommand.Parameters["@InvoiceNo"].Value = invoiceNo;

            ILSInvoiceDs dataSet = new ILSInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSInvoiceDef def = new ILSInvoiceDef();
            this.ILSInvoiceMapping(dataSet.ILSInvoice[0], def);
            return def;
        }

        public ILSInvoiceDef getILSInvoiceByInvoiceNo(string invoiceNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceApt", "GetILSInvoiceByInvoiceNo");
            ad.SelectCommand.Parameters["@InvoiceNo"].Value = invoiceNo;

            ILSInvoiceDs dataSet = new ILSInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ILSInvoiceDef def = new ILSInvoiceDef();
            this.ILSInvoiceMapping(dataSet.ILSInvoice[0], def);
            return def;
        }

        public ArrayList getOutstandingILSInvoice()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceApt", "GetOutstandingILSInvoice");

            ILSInvoiceDs dataSet = new ILSInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSInvoiceDs.ILSInvoiceRow row in dataSet.ILSInvoice)
            {
                ILSInvoiceDef def = new ILSInvoiceDef();
                this.ILSInvoiceMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSOrderRefResetList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSOrderRefApt", "GetILSOrderRefResetList");

            ILSOrderRefDs dataSet = new ILSOrderRefDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSOrderRefDs.ILSOrderRefRow row in dataSet.ILSOrderRef)
            {
                ILSOrderRefDef def = new ILSOrderRefDef();
                this.ILSOrderRefMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSInvoiceResetList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceApt", "GetILSInvoiceResetList");

            ILSInvoiceDs dataSet = new ILSInvoiceDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSInvoiceDs.ILSInvoiceRow row in dataSet.ILSInvoice)
            {
                ILSInvoiceDef def = new ILSInvoiceDef();
                this.ILSInvoiceMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList getILSInvoiceDetailList(int orderRefId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ILSInvoiceDetailApt", "GetILSInvoiceDetailList");
            ad.SelectCommand.Parameters["@OrderRefId"].Value = orderRefId;

            ILSInvoiceDetailDs dataSet = new ILSInvoiceDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ILSInvoiceDetailDs.ILSInvoiceDetailRow row in dataSet.ILSInvoiceDetail)
            {
                ILSInvoiceDetailDef def = new ILSInvoiceDetailDef();
                this.ILSInvoiceDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public void updateILSMessageResult(ILSMessageResultDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ILSMessageResultDs dataSet = new ILSMessageResultDs();
                ILSMessageResultDs.ILSMessageResultRow row = null;

                if (def.OrderRefId == -1)
                    def.OrderRefId = this.getOrderRefIdByContractNoAndDeliveryNo(def.ContractNo, def.DeliveryNo, -1);

                IDataSetAdapter ad = getDataSetAdapter("ILSMessageResultApt", "GetILSMessageResultByKey");
                ad.SelectCommand.Parameters["@FileNo"].Value = def.FileNo;
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.ILSMessageResult[0];
                    row.ProcessedDate = def.ProcessedDate;
                    row.Status = def.Status;
                    if (def.Status == "Y")
                        row.SetErrorNoNull();
                    else
                        row.ErrorNo = def.ErrorNo;
                }
                else
                {
                    row = dataSet.ILSMessageResult.NewILSMessageResultRow();
                    row.FileNo = def.FileNo;
                    row.OrderRefId = def.OrderRefId;
                    row.Type = def.Type;
                    row.SentDate = def.SentDate;
                    row.SetProcessedDateNull();
                    row.SetStatusNull();
                    row.SetErrorNoNull();
                    dataSet.ILSMessageResult.AddILSMessageResultRow(row);
                }

                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("failed to update ILSMessageResult");

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


        public void updateILSMonthEndLog(ILSMonthEndLogDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ILSMonthEndLogDs dataSet = new ILSMonthEndLogDs();
                ILSMonthEndLogDs.ILSMonthEndLogRow row = null;
                
                IDataSetAdapter ad = getDataSetAdapter("ILSMonthEndLogApt", "GetILSMonthEndLogByKey");
                ad.SelectCommand.Parameters["@FileNo"].Value = def.FileNo;
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.ILSMonthEndLog[0];
                    this.ILSMonthEndLogMapping(def, row);
                    row.CreatedOn = DateTime.Now;                      
                }
                else
                {
                    row = dataSet.ILSMonthEndLog.NewILSMonthEndLogRow();
                    this.ILSMonthEndLogMapping(def, row);
                    row.CreatedOn = DateTime.Now;                      
                    dataSet.ILSMonthEndLog.AddILSMonthEndLogRow(row);
                }

                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("failed to update ILSMonthEndLog");

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


        public void updateILSMonthEndShipment(ILSMonthEndShipmentDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ILSMonthEndShipmentDs dataSet = new ILSMonthEndShipmentDs();
                ILSMonthEndShipmentDs.ILSMonthEndShipmentRow row = null;

                def.OrderRefId = this.getOrderRefIdByContractNoAndDeliveryNo(def.ContractNo, def.DeliveryNo, -1);
                IDataSetAdapter ad = getDataSetAdapter("ILSMonthEndShipmentApt", "GetILSMonthEndShipmentByKey");                
                ad.SelectCommand.Parameters["@OrderRefId"].Value = def.OrderRefId;
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.ILSMonthEndShipment[0];
                    this.ILSMonthEndShipmentMapping(def, row);
                    row.ModifiedOn = DateTime.Now;
                }
                else
                {
                    row = dataSet.ILSMonthEndShipment.NewILSMonthEndShipmentRow();
                    this.ILSMonthEndShipmentMapping(def, row);
                    row.CreatedOn = DateTime.Now;
                    row.ModifiedOn = DateTime.Now;
                    dataSet.ILSMonthEndShipment.AddILSMonthEndShipmentRow(row);
                }

                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("failed to update ILSMonthEndShipment");

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


        #region Mapping Functions

        private void ILSOrderCopyMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSOrderCopyDs.ILSOrderCopyRow) &&
                target.GetType() == typeof(ILSOrderCopyDef))
            {
                ILSOrderCopyDs.ILSOrderCopyRow row = (ILSOrderCopyDs.ILSOrderCopyRow)source;
                ILSOrderCopyDef def = (ILSOrderCopyDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.ContractNo = ilsOrderRef.ContractNo;
                def.DeliveryNo = ilsOrderRef.DeliveryNo;
                def.ItemNo = row.ItemNo;
                def.ItemDesc = row.ItemDesc;
                def.TransportMode = row.TransportMode;
                def.CountryOfOrigin = row.CountryOfOrigin;
                def.DepartCountry = row.DepartCountry;
                def.ExFactoryDate = row.ExFactoryDate;
                def.InWarehouseDate = row.InWarehouseDate;
                def.SupplierCode = row.SupplierCode;
                def.SupplierName = row.SupplierName;
                def.HangBox = row.HangBox;
                def.BuyingTerms = row.BuyingTerms;
                def.FinalDestination = row.FinalDest;
                def.Currency = row.Currency;
                def.NextFreightPercent = row.NextFreightPercent;
                def.SupplierFreightPercent = row.SupplierFreightPercent;
                if (!row.IsArrivalPortNull())
                    def.ArrivalPort = row.ArrivalPort;
                else
                    def.ArrivalPort = String.Empty;
                if (!row.IsFranchisePartnerCodeNull())
                    def.FranchisePartnerCode = row.FranchisePartnerCode;
                else
                    def.FranchisePartnerCode = String.Empty;
                def.Refurb = row.Refurb;
                def.FileNo = row.FileNo;
                def.ImportDate = row.ImportDate;
                if (!row.IsLastSentLoadingPortNull())
                    def.LastSentLoadingPort = row.LastSentLoadingPort;
                else
                    def.LastSentLoadingPort = String.Empty;
                if (!row.IsLastSentOfficeCodeNull())
                    def.LastSentOfficeCode = row.LastSentOfficeCode;
                else
                    def.LastSentOfficeCode = String.Empty;
                if (!row.IsLastSentQuotaNull())
                    def.LastSentQuota = row.LastSentQuota;
                else
                    def.LastSentQuota = String.Empty;
                if (!row.IsLastSentDocTypeNull())
                    def.LastSentDocType = row.LastSentDocType;
                else
                    def.LastSentDocType = String.Empty;
                if (!row.IsLastSentOriginCountryNull())
                    def.LastSentOriginCountry = row.LastSentOriginCountry;
                else
                    def.LastSentOriginCountry = String.Empty;
                def.IsValid = row.IsValid;
            }
            else if (source.GetType() == typeof(ILSOrderCopyDef) &&
                target.GetType() == typeof(ILSOrderCopyDs.ILSOrderCopyRow))
            {
                ILSOrderCopyDef def = (ILSOrderCopyDef)source;
                ILSOrderCopyDs.ILSOrderCopyRow row = (ILSOrderCopyDs.ILSOrderCopyRow)target;

                row.OrderRefId = def.OrderRefId;
                row.ItemNo = def.ItemNo;
                row.ItemDesc = def.ItemDesc;
                row.TransportMode = def.TransportMode;
                row.CountryOfOrigin = def.CountryOfOrigin;
                row.DepartCountry = def.DepartCountry;
                row.ExFactoryDate = def.ExFactoryDate;
                row.InWarehouseDate = def.InWarehouseDate;
                row.SupplierCode = def.SupplierCode;
                row.SupplierName = def.SupplierName;
                row.HangBox = def.HangBox;
                row.BuyingTerms = def.BuyingTerms;
                row.FinalDest = def.FinalDestination;
                row.Currency = def.Currency;
                row.NextFreightPercent = def.NextFreightPercent;
                row.SupplierFreightPercent = def.SupplierFreightPercent;
                if (def.ArrivalPort == String.Empty)
                    row.SetArrivalPortNull();
                else
                    row.ArrivalPort = def.ArrivalPort;
                if (def.FranchisePartnerCode == String.Empty)
                    row.SetFranchisePartnerCodeNull();
                else
                    row.FranchisePartnerCode = def.FranchisePartnerCode;
                row.Refurb = def.Refurb;
                row.FileNo = def.FileNo;
                row.ImportDate = def.ImportDate;
                if (def.LastSentLoadingPort == String.Empty)
                    row.SetLastSentLoadingPortNull();
                else
                    row.LastSentLoadingPort = def.LastSentLoadingPort;
                if (def.LastSentOfficeCode == String.Empty)
                    row.SetLastSentOfficeCodeNull();
                else
                    row.LastSentOfficeCode = def.LastSentOfficeCode;
                if (def.LastSentQuota == String.Empty)
                    row.SetLastSentQuotaNull();
                else
                    row.LastSentQuota = def.LastSentQuota;
                
                if (def.LastSentLoadingPort == String.Empty)
                    row.SetLastSentDocTypeNull();
                else
                    row.LastSentDocType = def.LastSentDocType;

                if (def.LastSentOriginCountry == String.Empty)
                    row.SetLastSentOriginCountryNull();
                else
                    row.LastSentOriginCountry = def.LastSentOriginCountry;
                row.IsValid = def.IsValid;
            }
        }

        private void ILSOrderCopyDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSOrderCopyDetailDs.ILSOrderCopyDetailRow) &&
                target.GetType() == typeof(ILSOrderCopyDetailDef))
            {
                ILSOrderCopyDetailDs.ILSOrderCopyDetailRow row = (ILSOrderCopyDetailDs.ILSOrderCopyDetailRow)source;
                ILSOrderCopyDetailDef def = (ILSOrderCopyDetailDef)target;

                def.OrderRefId = row.OrderRefId;
                def.OptionNo = row.OptionNo;
                def.optionDescription = row.OptionDesc;
                def.Price = row.Price;
                def.Qty = row.Qty;

            }
            else if (source.GetType() == typeof(ILSOrderCopyDetailDef) &&
                target.GetType() == typeof(ILSOrderCopyDetailDs.ILSOrderCopyDetailRow))
            {
                ILSOrderCopyDetailDef def = (ILSOrderCopyDetailDef)source;
                ILSOrderCopyDetailDs.ILSOrderCopyDetailRow row = (ILSOrderCopyDetailDs.ILSOrderCopyDetailRow)target;

                row.OrderRefId = def.OrderRefId;
                row.OptionNo = def.OptionNo;
                row.OptionDesc = def.optionDescription;
                row.Price = def.Price;
                row.Qty = def.Qty;
            }
        }

        private void ILSPackingListMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSPackingListDs.ILSPackingListRow) &&
                target.GetType() == typeof(ILSPackingListDef))
            {
                ILSPackingListDs.ILSPackingListRow row = (ILSPackingListDs.ILSPackingListRow)source;
                ILSPackingListDef def = (ILSPackingListDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.ContractNo = ilsOrderRef.ContractNo;
                def.DeliveryNo = ilsOrderRef.DeliveryNo;
                def.ItemNo = row.ItemNo;
                def.ItemDesc = row.ItemDesc;
                def.TransportMode = row.TransportMode;
                def.CountryOfOrigin = row.CountryOfOrigin;
                def.DepartPort = row.DepartPort;
                def.ExFactoryDate = row.ExFactoryDate;
                def.InWarehouseDate = row.InWarehouseDate;
                def.SupplierCode = row.SupplierCode;
                def.SupplierName = row.SupplierName;
                def.HangBox = row.HangBox;
                def.BuyingTerms = row.BuyingTerms;
                def.FinalDestination = row.FinalDest;
                def.PrepaidFreightCost = row.PrepaidFreightCosts;
                if (!row.IsHandoverDateNull())
                    def.HandoverDate = row.HandoverDate;
                else
                    def.HandoverDate = DateTime.MinValue;
                def.VendorLoaded = row.VendorLoaded;
                if (!row.IsArrivalPortNull())
                    def.ArrivalPort = row.ArrivalPort;
                else
                    def.ArrivalPort = String.Empty;
                if (!row.IsFranchisePartnerCodeNull())
                    def.FranchisePartnerCode = row.FranchisePartnerCode;
                else
                    def.FranchisePartnerCode = String.Empty;
                def.Refurb = row.Refurb;
                def.TotalPieces = row.TotalPieces;
                def.TotalCartons = row.TotalCartons;
                def.TotalGrossWeight = row.TotalGrossWeight;
                def.TotalNetWeight = row.TotalNetWeight;
                def.TotalVolume = row.TotalVolume;
                def.FileNo = row.FileNo;
                def.ImportDate = row.ImportDate;
                def.IsUploaded = row.IsUploaded;
                if (!row.IsNSLDeliveryNoNull())
                    def.NSLDeliveryNo = row.NSLDeliveryNo;
                else
                    def.NSLDeliveryNo = 0;
            }
            else if (source.GetType() == typeof(ILSPackingListDef) &&
                target.GetType() == typeof(ILSPackingListDs.ILSPackingListRow))
            {
                ILSPackingListDef def = (ILSPackingListDef)source;
                ILSPackingListDs.ILSPackingListRow row = (ILSPackingListDs.ILSPackingListRow)target;

                row.OrderRefId = def.OrderRefId;
                row.ItemNo = def.ItemNo;
                row.ItemDesc = def.ItemDesc;
                row.TransportMode = def.TransportMode;
                row.CountryOfOrigin = def.CountryOfOrigin;
                row.DepartPort = def.DepartPort;
                row.ExFactoryDate = def.ExFactoryDate;
                row.InWarehouseDate = def.InWarehouseDate;
                row.SupplierCode = def.SupplierCode;
                row.SupplierName = def.SupplierName;
                row.HangBox = def.HangBox;
                row.BuyingTerms = def.BuyingTerms;
                row.FinalDest = def.FinalDestination;
                row.PrepaidFreightCosts = def.PrepaidFreightCost;
                if (def.HandoverDate == DateTime.MinValue)
                    row.SetHandoverDateNull();
                else
                    row.HandoverDate = def.HandoverDate;
                row.VendorLoaded = def.VendorLoaded;
                if (def.ArrivalPort == String.Empty)
                    row.SetArrivalPortNull();
                else
                    row.ArrivalPort = def.ArrivalPort;
                if (def.FranchisePartnerCode == String.Empty)
                    row.SetFranchisePartnerCodeNull();
                else
                    row.FranchisePartnerCode = def.FranchisePartnerCode;
                row.Refurb = def.Refurb;
                row.TotalPieces = def.TotalPieces;
                row.TotalCartons = def.TotalCartons;
                row.TotalGrossWeight = def.TotalGrossWeight;
                row.TotalNetWeight = def.TotalNetWeight;
                row.TotalVolume = def.TotalVolume;
                row.FileNo = def.FileNo;
                row.ImportDate = def.ImportDate;
                row.IsUploaded = def.IsUploaded;
                row.NSLDeliveryNo = def.NSLDeliveryNo;
            }
        }

        private void ILSPackingListDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSPackingListDetailDs.ILSPackingListDetailRow) &&
                target.GetType() == typeof(ILSPackingListDetailDef))
            {
                ILSPackingListDetailDs.ILSPackingListDetailRow row = (ILSPackingListDetailDs.ILSPackingListDetailRow)source;
                ILSPackingListDetailDef def = (ILSPackingListDetailDef)target;

                def.OrderRefId = row.OrderRefId;
                def.OptionNo = row.OptionNo;
                def.optionDescription = row.OptionDesc;
                def.Qty = row.Qty;
                def.Weight = row.Weight;
                def.Volume = row.Volume;
            }
            else if (source.GetType() == typeof(ILSPackingListDetailDef) &&
                target.GetType() == typeof(ILSPackingListDetailDs.ILSPackingListDetailRow))
            {
                ILSPackingListDetailDef def = (ILSPackingListDetailDef)source;
                ILSPackingListDetailDs.ILSPackingListDetailRow row = (ILSPackingListDetailDs.ILSPackingListDetailRow)target;

                row.OrderRefId = def.OrderRefId;
                row.OptionNo = def.OptionNo;
                row.OptionDesc = def.optionDescription;
                row.Qty = def.Qty;
                row.Weight = def.Weight;
                row.Volume = def.Volume;
            }
        }

        private void ILSPackingListCartonDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSPackingListCartonDetailDs.ILSPackingListCartonDetailRow) &&
                target.GetType() == typeof(ILSPackingListCartonDetailDef))
            {
                ILSPackingListCartonDetailDs.ILSPackingListCartonDetailRow row = (ILSPackingListCartonDetailDs.ILSPackingListCartonDetailRow)source;
                ILSPackingListCartonDetailDef def = (ILSPackingListCartonDetailDef)target;

                def.OrderRefId = row.OrderRefId;
                def.OptionNo = row.OptionNo;
                def.SeqNo = row.SeqNo;
                def.CartonSize = row.CartonSize;
                def.CartonLength = row.CartonLength;
                def.CartonWidth = row.CartonWidth;
                def.CartonHeight = row.CartonHeight;
                def.Pieces = row.Pieces;
                def.NoOfCartons = row.NoOfCartons;
                def.FirstCarton = row.FirstCarton;
                def.LastCarton = row.LastCarton;
            }
            else if (source.GetType() == typeof(ILSPackingListCartonDetailDef) &&
                target.GetType() == typeof(ILSPackingListCartonDetailDs.ILSPackingListCartonDetailRow))
            {
                ILSPackingListCartonDetailDef def = (ILSPackingListCartonDetailDef)source;
                ILSPackingListCartonDetailDs.ILSPackingListCartonDetailRow row = (ILSPackingListCartonDetailDs.ILSPackingListCartonDetailRow)target;

                row.OrderRefId = def.OrderRefId;
                row.OptionNo = def.OptionNo;
                row.SeqNo = def.SeqNo;
                row.CartonSize = def.CartonSize;
                row.CartonLength = def.CartonLength;
                row.CartonWidth = def.CartonWidth;
                row.CartonHeight = def.CartonHeight;
                row.Pieces = def.Pieces;
                row.NoOfCartons = def.NoOfCartons;
                row.FirstCarton = def.FirstCarton;
                row.LastCarton = def.LastCarton;
            }
        }

        private void ILSPackingListMixedCartonDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSPackingListMixedCartonDetailDs.ILSPackingListMixedCartonDetailRow) &&
                target.GetType() == typeof(ILSPackingListMixedCartonDetailDef))
            {
                ILSPackingListMixedCartonDetailDs.ILSPackingListMixedCartonDetailRow row = (ILSPackingListMixedCartonDetailDs.ILSPackingListMixedCartonDetailRow)source;
                ILSPackingListMixedCartonDetailDef def = (ILSPackingListMixedCartonDetailDef)target;

                def.OrderRefId = row.OrderRefId;
                def.CartonId = row.CartonId;
                def.CartonSize = row.CartonSize;
                def.CartonLength = row.CartonLength;
                def.CartonWidth = row.CartonWidth;
                def.CartonHeight = row.CartonHeight;
            }
            else if (source.GetType() == typeof(ILSPackingListMixedCartonDetailDef) &&
                target.GetType() == typeof(ILSPackingListMixedCartonDetailDs.ILSPackingListMixedCartonDetailRow))
            {
                ILSPackingListMixedCartonDetailDef def = (ILSPackingListMixedCartonDetailDef)source;
                ILSPackingListMixedCartonDetailDs.ILSPackingListMixedCartonDetailRow row = (ILSPackingListMixedCartonDetailDs.ILSPackingListMixedCartonDetailRow)target;

                row.OrderRefId = def.OrderRefId;
                row.CartonId = def.CartonId;
                row.CartonSize = def.CartonSize;
                row.CartonLength = def.CartonLength;
                row.CartonWidth = def.CartonWidth;
                row.CartonHeight = def.CartonHeight;
            }
        }

        private void ILSManifestMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSManifestDs.ILSManifestRow) &&
                target.GetType() == typeof(ILSManifestDef))
            {
                ILSManifestDs.ILSManifestRow row = (ILSManifestDs.ILSManifestRow)source;
                ILSManifestDef def = (ILSManifestDef)target;

                def.ContainerNo = row.ContainerNo;
                def.LegId = row.LegId;
                def.VoyageNo = row.VoyageNo;
                def.VesselName = row.VesselName;
                def.PartnerContainerNo = row.PartnerContainerNo;
                def.TransportMode = row.TransportMode;
                def.DepartPort = row.DepartPort;
                def.DepartDate = row.DepartDate;
                def.ArrivalPort = row.ArrivalPort;
                def.ArrivalDate = row.ArrivalDate;
                def.IsTranshipment = row.IsTranshipment;
                def.TotalContracts = row.TotalContracts;
                def.TotalVolume = row.TotalVolume;
                def.TotalPieces = row.TotalPieces;
                def.TotalCartons = row.TotalCartons;
                def.FileNo = row.FileNo;
                def.ImportDate = row.ImportDate;
            }
            else if (source.GetType() == typeof(ILSManifestDef) &&
                target.GetType() == typeof(ILSManifestDs.ILSManifestRow))
            {
                ILSManifestDef def = (ILSManifestDef)source;
                ILSManifestDs.ILSManifestRow row = (ILSManifestDs.ILSManifestRow)target;

                row.ContainerNo = def.ContainerNo;
                row.LegId = def.LegId;
                row.VoyageNo = def.VoyageNo;
                row.VesselName = def.VesselName;
                row.PartnerContainerNo = def.PartnerContainerNo;
                row.TransportMode = def.TransportMode;
                row.DepartPort = def.DepartPort;
                row.DepartDate = def.DepartDate;
                row.ArrivalPort = def.ArrivalPort;
                row.ArrivalDate = def.ArrivalDate;
                row.IsTranshipment = def.IsTranshipment;
                row.TotalContracts = def.TotalContracts;
                row.TotalVolume = def.TotalVolume;
                row.TotalPieces = def.TotalPieces;
                row.TotalCartons = def.TotalCartons;
                row.FileNo = def.FileNo;
                row.ImportDate = def.ImportDate;
            }
        }

        private void ILSManifestDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSManifestDetailDs.ILSManifestDetailRow) &&
                target.GetType() == typeof(ILSManifestDetailDef))
            {
                ILSManifestDetailDs.ILSManifestDetailRow row = (ILSManifestDetailDs.ILSManifestDetailRow)source;
                ILSManifestDetailDef def = (ILSManifestDetailDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.ContainerNo = row.ContainerNo;
                def.OrderRefId = row.OrderRefId;
                def.ContractNo = ilsOrderRef.ContractNo;
                def.DeliveryNo = ilsOrderRef.DeliveryNo;
                if (!row.IsCTSDateNull())
                    def.ConfirmedToShipDate = row.CTSDate;
                else
                    def.ConfirmedToShipDate = DateTime.MinValue;
                if (!row.IsSOBDateNull())
                    def.ShippedOnBoardDate = row.SOBDate;
                else
                    def.ShippedOnBoardDate = DateTime.MinValue;
                if (!row.IsPADateNull())
                    def.PreAdviceDate = row.PADate;
                else
                    def.PreAdviceDate = DateTime.MinValue;
                if (!row.IsContainerPositionNull())
                    def.ContainerPosition = row.ContainerPosition;
                else
                    def.ContainerPosition = String.Empty;
                def.IsCancelled = row.IsCancelled;
            }
            else if (source.GetType() == typeof(ILSManifestDetailDef) &&
                target.GetType() == typeof(ILSManifestDetailDs.ILSManifestDetailRow))
            {
                ILSManifestDetailDef def = (ILSManifestDetailDef)source;
                ILSManifestDetailDs.ILSManifestDetailRow row = (ILSManifestDetailDs.ILSManifestDetailRow)target;

                row.ContainerNo = def.ContainerNo;
                row.OrderRefId = def.OrderRefId;
                if (def.ConfirmedToShipDate != DateTime.MinValue)
                    row.CTSDate = def.ConfirmedToShipDate;
                else
                    row.SetCTSDateNull();
                if (def.ShippedOnBoardDate != DateTime.MinValue)
                    row.SOBDate = def.ShippedOnBoardDate;
                else
                    row.SetSOBDateNull();
                if (def.PreAdviceDate != DateTime.MinValue)
                    row.PADate = def.PreAdviceDate;
                else
                    row.SetPADateNull();
                if (def.ContainerPosition != String.Empty)
                    row.ContainerPosition = def.ContainerPosition;
                else
                    row.SetContainerPositionNull();
                row.IsCancelled = def.IsCancelled;
            }
        }

        private void ILSDocumentMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSDocumentDs.ILSDocumentRow) &&
                target.GetType() == typeof(ILSDocumentDef))
            {
                ILSDocumentDs.ILSDocumentRow row = (ILSDocumentDs.ILSDocumentRow)source;
                ILSDocumentDef def = (ILSDocumentDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.ContractNo = ilsOrderRef.ContractNo;
                def.DeliveryNo = ilsOrderRef.DeliveryNo;
                def.DocumentType = row.DocType;
                def.DocumentNo = row.DocNo;
                def.DocumentCountry = row.DocCountry;
                if (!row.IsDocActualTypeNull())
                    def.ActualType = row.DocActualType;
                else
                    def.ActualType = String.Empty;
                if (!row.IsDocStartDateNull())
                    def.StartDate = row.DocStartDate;
                else
                    def.StartDate = DateTime.MinValue;
                if (!row.IsDocExpiryDateNull())
                    def.ExpiryDate = row.DocExpiryDate;
                else
                    def.ExpiryDate = DateTime.MinValue;
                if (!row.IsDocQuotaCatNull())
                    def.QuotaCategory = row.DocQuotaCat;
                else
                    def.QuotaCategory = String.Empty;
                def.Weight = row.Weight;
                def.Pieces = row.Pieces;
                def.FileNo = row.FileNo;
                def.ImportDate = row.ImportDate;
            }
            else if (source.GetType() == typeof(ILSDocumentDef) &&
                target.GetType() == typeof(ILSDocumentDs.ILSDocumentRow))
            {
                ILSDocumentDef def = (ILSDocumentDef)source;
                ILSDocumentDs.ILSDocumentRow row = (ILSDocumentDs.ILSDocumentRow)target;

                row.OrderRefId = def.OrderRefId;
                row.DocType = def.DocumentType;
                row.DocNo = def.DocumentNo;
                row.DocCountry = def.DocumentCountry;
                if (def.ActualType == String.Empty)
                    row.SetDocActualTypeNull();
                else
                    row.DocActualType = def.ActualType;
                if (def.StartDate == DateTime.MinValue)
                    row.SetDocStartDateNull();
                else
                    row.DocStartDate = def.StartDate;
                if (def.ExpiryDate == DateTime.MinValue)
                    row.SetDocExpiryDateNull();
                else
                    row.DocExpiryDate = def.ExpiryDate;
                if (def.QuotaCategory == String.Empty)
                    row.SetDocQuotaCatNull();
                else
                    row.DocQuotaCat = def.QuotaCategory;
                row.Weight = def.Weight;
                row.Pieces = def.Pieces;
                row.FileNo = def.FileNo;
                row.ImportDate = def.ImportDate;
            }
        }

        private void ILSCancelledOrderMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSCancelledOrderDs.ILSCancelledOrderRow) &&
                target.GetType() == typeof(ILSCancelledOrderDef))
            {
                ILSCancelledOrderDs.ILSCancelledOrderRow row = (ILSCancelledOrderDs.ILSCancelledOrderRow)source;
                ILSCancelledOrderDef def = (ILSCancelledOrderDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.ContractNo = ilsOrderRef.ContractNo;
                def.DeliveryNo = ilsOrderRef.DeliveryNo;
                def.FileNo = row.FileNo;
                def.ImportDate = row.ImportDate;
                def.Status = row.Status;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = String.Empty;
            }
            else if (source.GetType() == typeof(ILSCancelledOrderDef) &&
                target.GetType() == typeof(ILSCancelledOrderDs.ILSCancelledOrderRow))
            {
                ILSCancelledOrderDef def = (ILSCancelledOrderDef)source;
                ILSCancelledOrderDs.ILSCancelledOrderRow row = (ILSCancelledOrderDs.ILSCancelledOrderRow)target;

                row.OrderRefId = def.OrderRefId;
                row.FileNo = def.FileNo;
                row.ImportDate = def.ImportDate;
                row.Status = def.Status;
                if (def.Remark != String.Empty)
                    row.Remark = def.Remark;
                else
                    row.SetRemarkNull();
            }
        }

        private void ILSOrderRefMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSOrderRefDs.ILSOrderRefRow) &&
                target.GetType() == typeof(ILSOrderRefDef))
            {
                ILSOrderRefDs.ILSOrderRefRow row = (ILSOrderRefDs.ILSOrderRefRow)source;
                ILSOrderRefDef def = (ILSOrderRefDef)target;

                def.OrderRefId = row.OrderRefId;
                def.ContractNo = row.ContractNo;
                def.DeliveryNo = row.DeliveryNo;
                if (!row.IsShipmentIdNull())
                    def.ShipmentId = row.ShipmentId;
                else
                    def.ShipmentId = 0;
                def.IsForced = row.IsForced;
                def.IsReset = row.IsReset;
                def.IsCancelled = row.IsCancelled;
                if (!row.IsModifiedByNull())
                    def.UpdateUser = generalWorker.getUserByKey(row.ModifiedBy);
            }
            else if (source.GetType() == typeof(ILSOrderRefDef) &&
                target.GetType() == typeof(ILSOrderRefDs.ILSOrderRefRow))
            {
                ILSOrderRefDef def = (ILSOrderRefDef)source;
                ILSOrderRefDs.ILSOrderRefRow row = (ILSOrderRefDs.ILSOrderRefRow)target;

                row.OrderRefId = def.OrderRefId;
                row.ContractNo = def.ContractNo;
                row.DeliveryNo = def.DeliveryNo;
                if (def.ShipmentId != 0)
                    row.ShipmentId = def.ShipmentId;
                else
                    row.SetShipmentIdNull();
                row.IsForced = def.IsForced;
                row.IsReset = def.IsReset;
                row.IsCancelled = def.IsCancelled;
                if (row.RowState == DataRowState.Detached)
                    row.CreatedOn = DateTime.Now;
                if (def.UpdateUser != null)
                    row.ModifiedBy = def.UpdateUser.UserId;
            }
        }

        private void ILSInvoiceMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSInvoiceDs.ILSInvoiceRow) &&
                target.GetType() == typeof(ILSInvoiceDef))
            {
                ILSInvoiceDs.ILSInvoiceRow row = (ILSInvoiceDs.ILSInvoiceRow)source;
                ILSInvoiceDef def = (ILSInvoiceDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.ContractNo = ilsOrderRef.ContractNo;
                def.DeliveryNo = ilsOrderRef.DeliveryNo;
                def.ItemNo = row.ItemNo;
                def.SupplierCode = row.SupplierCode;
                def.InvoiceNo = row.InvoiceNo;
                def.InvoiceDate = row.InvoiceDate;
                def.Currency = row.Currency;
                def.TotalVAT = row.TotalVAT;
                def.TotalQty = row.TotalQty;
                def.TotalAmount = row.TotalAmt;
                def.FileNo = row.FileNo;
                def.ImportDate = row.ImportDate;
                if (!row.IsProcessedDateNull())
                    def.ProcessedDate = row.ProcessedDate;
                else
                    def.ProcessedDate = DateTime.MinValue;
                if (!row.IsStatusNull())
                    def.Status = row.Status;
                else
                    def.Status = -1;
                def.IsCancelled = row.IsCancelled;
            }
            else if (source.GetType() == typeof(ILSInvoiceDef) &&
                target.GetType() == typeof(ILSInvoiceDs.ILSInvoiceRow))
            {
                ILSInvoiceDef def = (ILSInvoiceDef)source;
                ILSInvoiceDs.ILSInvoiceRow row = (ILSInvoiceDs.ILSInvoiceRow)target;

                row.OrderRefId = def.OrderRefId;
                row.ItemNo = def.ItemNo;
                row.SupplierCode = def.SupplierCode;
                row.InvoiceNo = def.InvoiceNo;
                row.InvoiceDate = def.InvoiceDate;
                row.Currency = def.Currency;
                row.TotalVAT = def.TotalVAT;
                row.TotalQty = def.TotalQty;
                row.TotalAmt = def.TotalAmount;
                row.FileNo = def.FileNo;
                row.ImportDate = def.ImportDate;
                if (def.ProcessedDate != DateTime.MinValue)
                    row.ProcessedDate = def.ProcessedDate;
                else
                    row.ProcessedDate = def.ImportDate;
                if (def.Status != -1)
                    row.Status = def.Status;
                else
                    row.SetStatusNull();
                def.IsCancelled = false;
                if (def.TotalAmount < 0 && def.InvoiceNo.StartsWith("C"))
                    def.IsCancelled = true;
                row.IsCancelled = def.IsCancelled;
            }
        }

        private void ILSInvoiceDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSInvoiceDetailDs.ILSInvoiceDetailRow) &&
                target.GetType() == typeof(ILSInvoiceDetailDef))
            {
                ILSInvoiceDetailDs.ILSInvoiceDetailRow row = (ILSInvoiceDetailDs.ILSInvoiceDetailRow)source;
                ILSInvoiceDetailDef def = (ILSInvoiceDetailDef)target;

                def.OrderRefId = row.OrderRefId;
                def.OptionNo = row.OptionNo;
                def.Qty = row.Qty;
                def.Price = row.Price;
                if (!row.IsVATCodeNull())
                    def.VATCode = row.VATCode;
                else
                    def.VATCode = String.Empty;
            }
            else if (source.GetType() == typeof(ILSInvoiceDetailDef) &&
                target.GetType() == typeof(ILSInvoiceDetailDs.ILSInvoiceDetailRow))
            {
                ILSInvoiceDetailDef def = (ILSInvoiceDetailDef)source;
                ILSInvoiceDetailDs.ILSInvoiceDetailRow row = (ILSInvoiceDetailDs.ILSInvoiceDetailRow)target;

                row.OrderRefId = def.OrderRefId;
                row.OptionNo = def.OptionNo;
                row.Qty = def.Qty;
                row.Price = def.Price;
                if (def.VATCode == String.Empty)
                    row.VATCode = def.VATCode;
                else
                    row.SetVATCodeNull();
            }
        }
        /*
        private void ILSCancelledInvoiceMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSCancelledInvoiceDs.ILSCancelledInvoiceRow) &&
                target.GetType() == typeof(ILSCancelledInvoiceDef))
            {
                ILSCancelledInvoiceDs.ILSCancelledInvoiceRow row = (ILSCancelledInvoiceDs.ILSCancelledInvoiceRow)source;
                ILSCancelledInvoiceDef def = (ILSCancelledInvoiceDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.ContractNo = ilsOrderRef.ContractNo;
                def.DeliveryNo = ilsOrderRef.DeliveryNo;
                def.ItemNo = row.ItemNo;
                def.SupplierCode = row.SupplierCode;
                def.InvoiceNo = row.InvoiceNo;
                def.InvoiceDate = row.InvoiceDate;
                def.Currency = row.Currency;
                def.TotalVAT = row.TotalVAT;
                def.TotalQty = row.TotalQty;
                def.TotalAmount = row.TotalAmt;
                def.FileNo = row.FileNo;
                def.ImportDate = row.ImportDate;
                if (!row.IsProcessedDateNull())
                    def.ProcessedDate = row.ProcessedDate;
                else
                    def.ProcessedDate = DateTime.MinValue;
                if (!row.IsStatusNull())
                    def.Status = row.Status;
                else
                    def.Status = -1;
                def.IsCancelled = row.IsCancelled;
            }
            else if (source.GetType() == typeof(ILSCancelledInvoiceDef) &&
                target.GetType() == typeof(ILSCancelledInvoiceDs.ILSCancelledInvoiceRow))
            {
                ILSCancelledInvoiceDef def = (ILSCancelledInvoiceDef)source;
                ILSCancelledInvoiceDs.ILSCancelledInvoiceRow row = (ILSCancelledInvoiceDs.ILSCancelledInvoiceRow)target;

                row.OrderRefId = def.OrderRefId;
                row.ItemNo = def.ItemNo;
                row.SupplierCode = def.SupplierCode;
                row.InvoiceNo = def.InvoiceNo;
                row.InvoiceDate = def.InvoiceDate;
                row.Currency = def.Currency;
                row.TotalVAT = def.TotalVAT;
                row.TotalQty = def.TotalQty;
                row.TotalAmt = def.TotalAmount;
                row.FileNo = def.FileNo;
                row.ImportDate = def.ImportDate;
                if (def.ProcessedDate != DateTime.MinValue)
                    row.ProcessedDate = def.ProcessedDate;
                else
                    row.ProcessedDate = def.ImportDate;
                if (def.Status != -1)
                    row.Status = def.Status;
                else
                    row.SetStatusNull();
                def.IsCancelled = false;
                if (def.TotalAmount < 0 && def.InvoiceNo.StartsWith("C"))
                    def.IsCancelled = true;
                row.IsCancelled = def.IsCancelled;
            }
        }

        private void ILSCancelledInvoiceDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSCancelledInvoiceDetailDs.ILSCancelledInvoiceDetailRow) &&
                target.GetType() == typeof(ILSCancelledInvoiceDetailDef))
            {
                ILSCancelledInvoiceDetailDs.ILSCancelledInvoiceDetailRow row = (ILSCancelledInvoiceDetailDs.ILSCancelledInvoiceDetailRow)source;
                ILSCancelledInvoiceDetailDef def = (ILSCancelledInvoiceDetailDef)target;

                def.OrderRefId = row.OrderRefId;
                def.OptionNo = row.OptionNo;
                def.Qty = row.Qty;
                def.Price = row.Price;
                if (!row.IsVATCodeNull())
                    def.VATCode = row.VATCode;
                else
                    def.VATCode = String.Empty;
            }
            else if (source.GetType() == typeof(ILSCancelledInvoiceDetailDef) &&
                target.GetType() == typeof(ILSCancelledInvoiceDetailDs.ILSCancelledInvoiceDetailRow))
            {
                ILSCancelledInvoiceDetailDef def = (ILSCancelledInvoiceDetailDef)source;
                ILSCancelledInvoiceDetailDs.ILSCancelledInvoiceDetailRow row = (ILSCancelledInvoiceDetailDs.ILSCancelledInvoiceDetailRow)target;

                row.OrderRefId = def.OrderRefId;
                row.OptionNo = def.OptionNo;
                row.Qty = def.Qty;
                row.Price = def.Price;
                if (def.VATCode == String.Empty)
                    row.VATCode = def.VATCode;
                else
                    row.SetVATCodeNull();
            }
        }
        */

        private void ILSCommissionInvoiceMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSCommissionInvoiceDs.ILSCommissionInvoiceRow) &&
                target.GetType() == typeof(ILSCommissionInvoiceDef))
            {
                ILSCommissionInvoiceDs.ILSCommissionInvoiceRow row = (ILSCommissionInvoiceDs.ILSCommissionInvoiceRow)source;
                ILSCommissionInvoiceDef def = (ILSCommissionInvoiceDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.ItemNo = row.ItemNo;
                def.SupplierCode = row.SupplierCode;
                def.InvoiceNo = row.InvoiceNo;
                def.InvoiceDate = row.InvoiceDate;
                def.Currency = row.Currency;
                def.TotalVAT = row.TotalVAT;
                def.TotalAmount = row.TotalAmt;
                def.FileNo = row.FileNo;
                def.ImportDate = row.ImportDate;
                if (!row.IsStatusNull())
                    def.Status = row.Status;
                else
                    def.Status = -1;
            }
            else if (source.GetType() == typeof(ILSCommissionInvoiceDef) &&
                target.GetType() == typeof(ILSCommissionInvoiceDs.ILSCommissionInvoiceRow))
            {
                ILSCommissionInvoiceDef def = (ILSCommissionInvoiceDef)source;
                ILSCommissionInvoiceDs.ILSCommissionInvoiceRow row = (ILSCommissionInvoiceDs.ILSCommissionInvoiceRow)target;

                row.OrderRefId = def.OrderRefId;
                row.ItemNo = def.ItemNo;
                row.SupplierCode = def.SupplierCode;
                row.InvoiceNo = def.InvoiceNo;
                row.InvoiceDate = def.InvoiceDate;
                row.Currency = def.Currency;
                row.TotalVAT = def.TotalVAT;
                row.TotalAmt = def.TotalAmount;
                row.FileNo = def.FileNo;
                row.ImportDate = def.ImportDate;
                if (def.Status != -1)
                    row.Status = def.Status;
                else
                    row.SetStatusNull();
            }
        }

        private void ILSCommissionInvoiceDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSCommissionInvoiceDetailDs.ILSCommissionInvoiceDetailRow) &&
                target.GetType() == typeof(ILSCommissionInvoiceDetailDef))
            {
                ILSCommissionInvoiceDetailDs.ILSCommissionInvoiceDetailRow row = (ILSCommissionInvoiceDetailDs.ILSCommissionInvoiceDetailRow)source;
                ILSCommissionInvoiceDetailDef def = (ILSCommissionInvoiceDetailDef)target;

                def.OrderRefId = row.OrderRefId;
                def.SeqId = row.SeqId;
                def.LineDescription = row.LineDesc;
                def.Amount = row.Amt;
                if (!row.IsVATCodeNull())
                    def.VATCode = row.VATCode;
                else
                    def.VATCode = String.Empty;
            }
            else if (source.GetType() == typeof(ILSCommissionInvoiceDetailDef) &&
                target.GetType() == typeof(ILSCommissionInvoiceDetailDs.ILSCommissionInvoiceDetailRow))
            {
                ILSCommissionInvoiceDetailDef def = (ILSCommissionInvoiceDetailDef)source;
                ILSCommissionInvoiceDetailDs.ILSCommissionInvoiceDetailRow row = (ILSCommissionInvoiceDetailDs.ILSCommissionInvoiceDetailRow)target;

                row.OrderRefId = def.OrderRefId;
                row.SeqId = def.SeqId;
                row.LineDesc = def.LineDescription;
                row.Amt = def.Amount;
                if (def.VATCode == String.Empty)
                    row.VATCode = def.VATCode;
                else
                    row.SetVATCodeNull();
            }
        }

        private void ILSOrderCopyDiscrepancyMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSOrderCopyDiscrepancyDs.ILSOrderCopyDiscrepancyRow) &&
                target.GetType() == typeof(ILSOrderCopyDiscrepancyDef))
            {
                ILSOrderCopyDiscrepancyDs.ILSOrderCopyDiscrepancyRow row = (ILSOrderCopyDiscrepancyDs.ILSOrderCopyDiscrepancyRow)source;
                ILSOrderCopyDiscrepancyDef def = (ILSOrderCopyDiscrepancyDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.OrderRef = ilsOrderRef.OrderRef;
                def.ShipmentId = ilsOrderRef.ShipmentId;
                def.CurrencyCode = row.CurrencyCode;
                def.OptionNo = row.OptionNo;
                def.TransportMode = row.TransportMode;
                def.ILSTransportMode = row.ILSTransportMode;
                def.SellingPrice = row.Price;
                def.ILSSellingPrice = row.ILSPrice;
                def.ILSNextPercent = row.ILSNextPerc;
                def.NextPercent = row.NEXTPerc;
                def.ILSSupplierPercent = row.ILSSupplierPerc;
                def.SupplierPercent = row.SupplierPerc;
            }
        }

        private void ILSOrderCopyOriginMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSOrderCopyOriginDs.ILSOrderCopyOriginRow) &&
                target.GetType() == typeof(ILSOrderCopyOriginDef))
            {
                ILSOrderCopyOriginDs.ILSOrderCopyOriginRow row = (ILSOrderCopyOriginDs.ILSOrderCopyOriginRow)source;
                ILSOrderCopyOriginDef def = (ILSOrderCopyOriginDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.OrderRef = ilsOrderRef.OrderRef;
                def.ShipmentId = ilsOrderRef.ShipmentId;
                def.OriginContract = row.OriginContract;
                def.OfficeCode = row.OfficeCode;
                def.PortCode = row.PortCode;
                def.RequiredDocs = row.RequiredDocs;
                def.QuotaCats = row.QuotaCats;
                def.OriginCountry = row.OriginCountry;
            }
        }

        private void ILSUnitPriceMatrixMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSUnitPriceMatrixDs.ILSUnitPriceMatrixRow) &&
                target.GetType() == typeof(ILSUnitPriceMatrixDef))
            {
                ILSUnitPriceMatrixDs.ILSUnitPriceMatrixRow row = (ILSUnitPriceMatrixDs.ILSUnitPriceMatrixRow)source;
                ILSUnitPriceMatrixDef def = (ILSUnitPriceMatrixDef)target;

                def.NSLOptionNo = (row.IsNSLOptionNoNull() ? row.NUKOptionNo : row.NSLOptionNo);
                def.NSLSizeDesc = (row.IsNSLOptionNoNull() ? row.NUKSizeDesc : row.NSLSizeDesc);
                def.NSLPrice = (row.IsNSLPriceNull() ? 0 : row.NSLPrice);
                def.NSLQty = (row.IsNSLQtyNull() ? 0 : row.NSLQty);

                def.NUKOptionNo = (row.IsNUKOptionNoNull() ? row.NSLOptionNo : row.NUKOptionNo);
                def.NUKSizeDesc = (row.IsNUKOptionNoNull() ? row.NSLSizeDesc : row.NUKSizeDesc);
                def.NUKPrice = (row.IsNUKPriceNull() ? 0 : row.NUKPrice);
                def.NUKQty = (row.IsNUKQtyNull() ? 0 : row.NUKQty);

                def.FOBPrice = (row.IsFOBPriceNull() ? 0 : row.FOBPrice);
            }
        }

        private void ILSQCCApprovalMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSQCCApprovalDs.ILSQCCApprovalRow) &&
                target.GetType() == typeof(ILSQCCApprovalDef))
            {
                ILSQCCApprovalDs.ILSQCCApprovalRow row = (ILSQCCApprovalDs.ILSQCCApprovalRow)source;
                ILSQCCApprovalDef def = (ILSQCCApprovalDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.OrderRef = ilsOrderRef.OrderRef;
                def.ShipmentId = ilsOrderRef.ShipmentId;
                def.CompletedTime = row.CompletedTime;
                def.StartTime = row.StartTime;
                def.ContractId = row.ContractId;
                def.InspectionId = row.InspectionId;
                def.Status = row.Status;
            }
        }

        private void ILSErrorMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSErrorDs.ILSErrorRow) &&
                target.GetType() == typeof(ILSErrorRef))
            {
                ILSErrorDs.ILSErrorRow row = (ILSErrorDs.ILSErrorRow)source;
                ILSErrorRef def = (ILSErrorRef)target;

                def.ErrorNo = row.ErrorNo;
                def.Description = row.ErrorDesc;
            }
        }

        private void ILSMonthEndShipmentMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSMonthEndShipmentDs.ILSMonthEndShipmentRow) &&
                target.GetType() == typeof(ILSMonthEndShipmentDef))
            {
                ILSMonthEndShipmentDs.ILSMonthEndShipmentRow row = (ILSMonthEndShipmentDs.ILSMonthEndShipmentRow)source;
                ILSMonthEndShipmentDef def = (ILSMonthEndShipmentDef)target;

                ILSOrderRefDef ilsOrderRef = this.getILSOrderRefByKey(row.OrderRefId);

                def.OrderRefId = row.OrderRefId;
                def.ContractNo = ilsOrderRef.ContractNo;
                def.DeliveryNo = ilsOrderRef.DeliveryNo;
                def.InvoiceNo = row.InvoiceNo;
                def.LastStatus  = row.LastStatus;
                def.NUKExtractDate = row.NUKExtractDate;

            }
            else if (source.GetType() == typeof(ILSMonthEndShipmentDef) &&
                target.GetType() == typeof(ILSMonthEndShipmentDs.ILSMonthEndShipmentRow))
            {
                ILSMonthEndShipmentDef def = (ILSMonthEndShipmentDef)source;
                ILSMonthEndShipmentDs.ILSMonthEndShipmentRow row = (ILSMonthEndShipmentDs.ILSMonthEndShipmentRow)target;

                row.OrderRefId = def.OrderRefId;
                row.ContractNo = def.ContractNo;
                row.DeliveryNo = def.DeliveryNo;
                row.InvoiceNo = def.InvoiceNo;
                row.LastStatus = def.LastStatus;
                row.NUKExtractDate = def.NUKExtractDate;
                
            }
        }


        private void ILSMonthEndLogMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ILSMonthEndLogDs.ILSMonthEndLogRow) &&
                target.GetType() == typeof(ILSMonthEndLogDef))
            {
                ILSMonthEndLogDs.ILSMonthEndLogRow row = (ILSMonthEndLogDs.ILSMonthEndLogRow)source;
                ILSMonthEndLogDef def = (ILSMonthEndLogDef)target;
                
                def.OrderRefId = row.OrderRefId;
                def.FileNo = row.FileNo;
                def.ContractNo = row.ContractNo;
                def.DeliveryNo = row.DeliveryNo;
                def.Status = row.Status;                
                def.NUKExtractDate = row.NUKExtractDate;
            }
            else if (source.GetType() == typeof(ILSMonthEndLogDef) &&
                target.GetType() == typeof(ILSMonthEndLogDs.ILSMonthEndLogRow))
            {
                ILSMonthEndLogDef def = (ILSMonthEndLogDef)source;
                ILSMonthEndLogDs.ILSMonthEndLogRow row = (ILSMonthEndLogDs.ILSMonthEndLogRow)target;

                row.OrderRefId = def.OrderRefId;
                row.FileNo = def.FileNo;
                row.ContractNo = def.ContractNo;
                row.DeliveryNo = def.DeliveryNo;
                row.Status = def.Status;
                row.NUKExtractDate = def.NUKExtractDate;
            }
        }


        #endregion

    }
}
