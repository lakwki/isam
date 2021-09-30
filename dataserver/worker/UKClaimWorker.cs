using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using com.next.infra.util;
using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.common.datafactory.worker;
using com.next.isam.domain.claim;
using com.next.isam.dataserver.model.claim;
using com.next.isam.domain.types;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.types;
using com.next.isam.domain.common;
using com.next.isam.dataserver.model.common;

namespace com.next.isam.dataserver.worker
{
	public class UKClaimWorker : Worker
	{
		private static UKClaimWorker _instance;
		private CommonWorker commonWorker;
		private GeneralWorker generalWorker;

        protected UKClaimWorker()
		{
			commonWorker = CommonWorker.Instance;
			generalWorker = GeneralWorker.Instance;
		}

        public static UKClaimWorker Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new UKClaimWorker();
				}
				return _instance;
			}
		}

        public List<UKClaimDef> getUKClaimListByCriteria(int claimTypeId, int officeId, int handlingOfficeId, string ukDebitNoteNo, int vendorId, string itemNo, string contractNo, string debitNoteNo, DateTime fromDate, DateTime toDate, DateTime fromReceivedDate, DateTime toReceivedDate, TypeCollector workflowStatusList, int termOfPurchaseId)
		{
			IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimListByCriteria");

			ad.SelectCommand.Parameters["@UKDebitNoteNo"].Value = ukDebitNoteNo;
            ad.SelectCommand.Parameters["@DebitNoteNo"].Value = debitNoteNo;
			ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.Parameters["@ClaimTypeId"].Value = claimTypeId;
			if (fromDate == DateTime.MinValue)
				ad.SelectCommand.Parameters["@FromDate"].Value = SqlDateTime.MinValue;
			else
				ad.SelectCommand.Parameters["@FromDate"].Value = fromDate;
			if (toDate == DateTime.MinValue)
				ad.SelectCommand.Parameters["@ToDate"].Value = SqlDateTime.MaxValue;
			else
				ad.SelectCommand.Parameters["@ToDate"].Value = toDate;

            if (fromReceivedDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromReceivedDate"].Value = SqlDateTime.MinValue;
            else
                ad.SelectCommand.Parameters["@FromReceivedDate"].Value = fromReceivedDate;
            if (toReceivedDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToReceivedDate"].Value = SqlDateTime.MaxValue;
            else
                ad.SelectCommand.Parameters["@ToReceivedDate"].Value = toReceivedDate;

			ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
			ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.CustomParameters["@WorkflowStatusList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);

            ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.MailSQL = true;
            UKClaimDs dataSet = new UKClaimDs();
			int recordsAffected = ad.Fill(dataSet);

			List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
			{
                UKClaimDef def = new UKClaimDef();
				UKClaimMapping(row, def);
				list.Add(def);
			}
			return list;
		}

        public List<UKClaimRefundDef> getUKClaimRefundListByCriteria(int claimTypeId, int officeId, int handlingOfficeId, string ukDebitNoteNo, int vendorId, string itemNo, string contractNo, string debitNoteNo, DateTime fromDate, DateTime toDate, DateTime fromReceivedDate, DateTime toReceivedDate, TypeCollector workflowStatusList, int termOfPurchaseId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimRefundApt", "GetUKClaimRefundListByCriteria");

            ad.SelectCommand.Parameters["@UKDebitNoteNo"].Value = ukDebitNoteNo;
            ad.SelectCommand.Parameters["@DebitNoteNo"].Value = debitNoteNo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.Parameters["@ClaimTypeId"].Value = claimTypeId;
            if (fromDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromDate"].Value = SqlDateTime.MinValue;
            else
                ad.SelectCommand.Parameters["@FromDate"].Value = fromDate;
            if (toDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToDate"].Value = SqlDateTime.MaxValue;
            else
                ad.SelectCommand.Parameters["@ToDate"].Value = toDate;

            if (fromReceivedDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromReceivedDate"].Value = SqlDateTime.MinValue;
            else
                ad.SelectCommand.Parameters["@FromReceivedDate"].Value = fromReceivedDate;
            if (toReceivedDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToReceivedDate"].Value = SqlDateTime.MaxValue;
            else
                ad.SelectCommand.Parameters["@ToReceivedDate"].Value = toReceivedDate;

            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;
            ad.SelectCommand.CustomParameters["@WorkflowStatusList"] = CustomDataParameter.parse(workflowStatusList.IsInclusive, workflowStatusList.Values);

            ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = termOfPurchaseId;

            UKClaimRefundDs dataSet = new UKClaimRefundDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimRefundDef> list = new List<UKClaimRefundDef>();

            foreach (UKClaimRefundDs.UKClaimRefundRow row in dataSet.UKClaimRefund)
            {
                UKClaimRefundDef def = new UKClaimRefundDef();
                UKClaimRefundMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getUKClaimEarlyArrivalList(TypeCollector officeIds)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimEarlyArrivalList");

            ad.SelectCommand.CustomParameters["@OfficeIds"] = CustomDataParameter.parse(officeIds.IsInclusive, officeIds.Values);

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getUKClaimDiscrepancyList()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimDiscrepancyList");

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getUKClaimDebugList()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimDebugList");

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getUKClaimListByBIAId(int parentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimListByBIAId");

            ad.SelectCommand.Parameters["@ParentId"].Value = parentId;

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getUKClaimListByRequestId(TypeCollector claimRequestIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimListByRequestId");

            ad.SelectCommand.CustomParameters["@ClaimRequestIdList"] = CustomDataParameter.parse(claimRequestIdList.IsInclusive, claimRequestIdList.Values);

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getUKClaimApprovalList(TypeCollector officeIds)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimApprovalList");

            ad.SelectCommand.CustomParameters["@OfficeIds"] = CustomDataParameter.parse(officeIds.IsInclusive, officeIds.Values);

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getUKClaimCOOApprovalList(TypeCollector officeIds, int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimCOOApprovalList");

            ad.SelectCommand.CustomParameters["@OfficeIds"] = CustomDataParameter.parse(officeIds.IsInclusive, officeIds.Values);
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public void deleteUKClaim(int claimId, int userId)
        {
            if (claimId <= 0 || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimDs dataSet = new UKClaimDs();
                UKClaimDs.UKClaimRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimByKey");
                ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaim[0];
                    row.Status = 0;
                    row.SetClaimRequestIdNull();
                    sealStamp(dataSet.UKClaim[0], userId, Stamp.UPDATE);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaim ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public List<UKClaimDef> getUKClaimListByTypeMapping(int claimId, int claimTypeId, int vendorId, string itemNo, string ukDebitNoteNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimListByTypeMapping");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@ClaimTypeId"].Value = claimTypeId;
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@UKDebitNoteNo"].Value = ukDebitNoteNo;

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getUKClaimListByTypeMapping(int claimId, int claimTypeId, int vendorId, string itemNo, string ukDebitNoteNo, int qty)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimListByTypeMappingByQty");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@ClaimTypeId"].Value = claimTypeId;
            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@UKDebitNoteNo"].Value = ukDebitNoteNo;
            ad.SelectCommand.Parameters["@Qty"].Value = qty;

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }


        public List<UKClaimPhasingDef> getUKClaimPhasingReport(int fiscalYear, int periodFrom, int periodTo, int officeId, int vendorId, int isGroupByReason)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimPhasingApt", "GetUKClaimPhasingReport");

            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@PeriodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@PeriodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@GroupByReason"].Value = (isGroupByReason == 1 ? 1 : 0);
            ad.SelectCommand.MailSQL = true;
            UKClaimPhasingDs dataSet = new UKClaimPhasingDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimPhasingDef> list = new List<UKClaimPhasingDef>();

            foreach (UKClaimPhasingDs.UKClaimPhasingRow row in dataSet.UKClaimPhasing)
            {
                UKClaimPhasingDef def = new UKClaimPhasingDef();
                UKClaimPhasingMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimPhasingByProductTeamDef> getUKClaimPhasingByProductTeamReport(int fiscalYear, int periodFrom, int periodTo, int officeId, int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimPhasingApt", "GetUKClaimPhasingByProductTeamReport");

            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@PeriodFrom"].Value = periodFrom;
            ad.SelectCommand.Parameters["@PeriodTo"].Value = periodTo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.MailSQL = true;
            UKClaimPhasingByProductTeamDs dataSet = new UKClaimPhasingByProductTeamDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimPhasingByProductTeamDef> list = new List<UKClaimPhasingByProductTeamDef>();

            foreach (UKClaimPhasingByProductTeamDs.UKClaimPhasingRow row in dataSet.UKClaimPhasing)
            {
                UKClaimPhasingByProductTeamDef def = new UKClaimPhasingByProductTeamDef();
                UKClaimPhasingByProductTeamMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getUKClaimReviewList(TypeCollector officeIds, int workflowStatusId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimReviewList");

            ad.SelectCommand.CustomParameters["@OfficeIds"] = CustomDataParameter.parse(officeIds.IsInclusive, officeIds.Values);
            ad.SelectCommand.Parameters["@WorkflowStatusId"].Value = workflowStatusId;

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getOutstandingUKClaimList(int officeId, int vendorId, int termOfPurchaseId, DateTime cutoffDate, int handlingOfficeId, int ncOptionId, TypeCollector wfsExcludingList)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetOutstandingUKClaimList");

            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@CutOffDate"].Value = cutoffDate;
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.Parameters["@NCOptionId"].Value = ncOptionId;
            ad.SelectCommand.CustomParameters["@WFSExcludingList"] = CustomDataParameter.parse(wfsExcludingList.IsInclusive, wfsExcludingList.Values);
            ad.SelectCommand.MailSQL = true;

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public UKClaimRechargeDs getOutstandingUKClaimRechargeList(int officeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimRechargeApt", "GetOutstandingUKClaimRechargeList");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;

            UKClaimRechargeDs dataSet = new UKClaimRechargeDs();
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public UKClaimRechargeDs getOutstandingUKClaimRechargeNRList()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimRechargeApt", "GetOutstandingUKClaimRechargeNRList");

            UKClaimRechargeDs dataSet = new UKClaimRechargeDs();
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

        public List<UKClaimDCNoteDetailDef> getOutstandingUKClaimDCNoteDetailList(string debitCreditIndicator, int officeId, int currencyId, int vendorId, int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteDetailApt", "GetOutstandingUKClaimDCNoteDetailList");

            ad.SelectCommand.Parameters["@DebitCreditIndicator"].Value = debitCreditIndicator;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;

            UKClaimDCNoteDetailDs dataSet = new UKClaimDCNoteDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDCNoteDetailDef> list = new List<UKClaimDCNoteDetailDef>();

            foreach (UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow row in dataSet.UKClaimDCNoteDetail)
            {
                UKClaimDCNoteDetailDef def = new UKClaimDCNoteDetailDef();
                UKClaimDCNoteDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDCNoteDetailDef> getOutstandingFullRefundUKClaimDCNoteDetailList(int claimId, int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteDetailApt", "GetOutstandingFullRefundUKClaimDCNoteDetailList");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;

            UKClaimDCNoteDetailDs dataSet = new UKClaimDCNoteDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDCNoteDetailDef> list = new List<UKClaimDCNoteDetailDef>();

            foreach (UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow row in dataSet.UKClaimDCNoteDetail)
            {
                UKClaimDCNoteDetailDef def = new UKClaimDCNoteDetailDef();
                UKClaimDCNoteDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDCNoteDetailDef> getOutstandingFullRefundUKClaimDCNoteDetailListByClaimId(int claimId, int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteDetailApt", "GetOutstandingFullRefundUKClaimDCNoteDetailListByClaimId");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;

            UKClaimDCNoteDetailDs dataSet = new UKClaimDCNoteDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDCNoteDetailDef> list = new List<UKClaimDCNoteDetailDef>();

            foreach (UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow row in dataSet.UKClaimDCNoteDetail)
            {
                UKClaimDCNoteDetailDef def = new UKClaimDCNoteDetailDef();
                UKClaimDCNoteDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDCNoteDetailDef> getOutstandingRefundUKClaimDCNoteDetailList(int claimRefundId, int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteDetailApt", "GetOutstandingRefundUKClaimDCNoteDetailList");

            ad.SelectCommand.Parameters["@ClaimRefundId"].Value = claimRefundId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;

            UKClaimDCNoteDetailDs dataSet = new UKClaimDCNoteDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDCNoteDetailDef> list = new List<UKClaimDCNoteDetailDef>();

            foreach (UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow row in dataSet.UKClaimDCNoteDetail)
            {
                UKClaimDCNoteDetailDef def = new UKClaimDCNoteDetailDef();
                UKClaimDCNoteDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDCNoteDetailDef> getUKClaimDCNoteDetailListByDCNoteId(int dcNoteId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteDetailApt", "GetUKClaimDCNoteDetailListByDCNoteId");

            ad.SelectCommand.Parameters["@DCNoteId"].Value = dcNoteId;

            UKClaimDCNoteDetailDs dataSet = new UKClaimDCNoteDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDCNoteDetailDef> list = new List<UKClaimDCNoteDetailDef>();

            foreach (UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow row in dataSet.UKClaimDCNoteDetail)
            {
                UKClaimDCNoteDetailDef def = new UKClaimDCNoteDetailDef();
                UKClaimDCNoteDetailMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public QAIS.ClaimRequestDef getClaimRequestDefByKey(int claimRequestId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
                QAIS.ClaimRequestDef def = svc.GetClaimRequestByKey(claimRequestId);

                ctx.VoteCommit();
                return def;
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        
        public UKClaimDef getUKClaimByKey(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimByKey");
            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId.ToString();

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimDs.UKClaimRow row = dataSet.UKClaim[0];
            UKClaimDef def = new UKClaimDef();
            UKClaimMapping(row, def);
            return def;
        }
        
        public UKClaimDef getNotYetMappedBIAUKClaimByClaimRequestId(int claimRequestId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetNotYetMappedBIAUKClaimByClaimRequestId");
            ad.SelectCommand.Parameters["@ClaimRequestId"].Value = claimRequestId.ToString();

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimDs.UKClaimRow row = dataSet.UKClaim[0];
            UKClaimDef def = new UKClaimDef();
            UKClaimMapping(row, def);
            return def;
        }

        public UKClaimDCNoteDef getUKClaimDCNoteByKey(int dcNoteId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteApt", "GetUKClaimDCNoteByKey");
            ad.SelectCommand.Parameters["@DCNoteId"].Value = dcNoteId;

            UKClaimDCNoteDs dataSet = new UKClaimDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimDCNoteDs.UKClaimDCNoteRow row = dataSet.UKClaimDCNote[0];
            UKClaimDCNoteDef def = new UKClaimDCNoteDef();
            UKClaimDCNoteMapping(row, def);
            return def;
        }

        public UKClaimBIADiscrepancyDef getUKClaimBIADiscrepancyByKey(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimBIADiscrepancyApt", "GetUKClaimBIADiscrepancyByKey");
            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;

            UKClaimBIADiscrepancyDs dataSet = new UKClaimBIADiscrepancyDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimBIADiscrepancyDs.UKClaimBIADiscrepancyRow row = dataSet.UKClaimBIADiscrepancy[0];
            UKClaimBIADiscrepancyDef def = new UKClaimBIADiscrepancyDef();
            UKClaimBIADiscrepancyMapping(row, def);
            return def;
        }

        public UKClaimBIADiscrepancyDef getUKClaimBIADiscrepancyByChildId(int childId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimBIADiscrepancyApt", "GetUKClaimBIADiscrepancyByChildId");
            ad.SelectCommand.Parameters["@ChildId"].Value = childId;

            UKClaimBIADiscrepancyDs dataSet = new UKClaimBIADiscrepancyDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimBIADiscrepancyDs.UKClaimBIADiscrepancyRow row = dataSet.UKClaimBIADiscrepancy[0];
            UKClaimBIADiscrepancyDef def = new UKClaimBIADiscrepancyDef();
            UKClaimBIADiscrepancyMapping(row, def);
            return def;
        }


        public UKClaimDCNoteDef getUKClaimDCNoteByDCNoteNo(string dcNoteNo)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteApt", "GetUKClaimDCNoteByDCNoteNo");
            ad.SelectCommand.Parameters["@DCNoteNo"].Value = dcNoteNo;

            UKClaimDCNoteDs dataSet = new UKClaimDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimDCNoteDs.UKClaimDCNoteRow row = dataSet.UKClaimDCNote[0];
            UKClaimDCNoteDef def = new UKClaimDCNoteDef();
            UKClaimDCNoteMapping(row, def);
            return def;
        }

        public UKClaimDef getUKClaimByGuid(string guid)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimByGuid");
            ad.SelectCommand.Parameters["@Guid"].Value = guid;

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimDs.UKClaimRow row = dataSet.UKClaim[0];
            UKClaimDef def = new UKClaimDef();
            UKClaimMapping(row, def);
            return def;
        }

        public UKClaimDef getUKClaimByClaimRequestId(int claimRequestId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimByClaimRequestId");
            ad.SelectCommand.Parameters["@ClaimRequestId"].Value = claimRequestId;

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimDs.UKClaimRow row = dataSet.UKClaim[0];
            UKClaimDef def = new UKClaimDef();
            UKClaimMapping(row, def);
            return def;
        }

        public UKClaimDef getBIAUKClaimByClaimRequestId(int claimRequestId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetBIAUKClaimByClaimRequestId");
            ad.SelectCommand.Parameters["@ClaimRequestId"].Value = claimRequestId.ToString();

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimDs.UKClaimRow row = dataSet.UKClaim[0];
            UKClaimDef def = new UKClaimDef();
            UKClaimMapping(row, def);
            return def;
        }

        public List<UKClaimDef> getNotYetMappedUKClaimList()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetNotYetMappedUKClaimList");

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getNewlyMappedUKClaimList()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetNewlyMappedUKClaimList");

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDef> getToBeCancelledUKClaimList(int officeGroupId, int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetToBeCancelledUKClaimList");
            ad.SelectCommand.Parameters["@OfficeGroupId"].Value = officeGroupId;
            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;

            UKClaimDs dataSet = new UKClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDef> list = new List<UKClaimDef>();

            foreach (UKClaimDs.UKClaimRow row in dataSet.UKClaim)
            {
                UKClaimDef def = new UKClaimDef();
                UKClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public ArrayList buildUKClaimDNArray(UKClaimDef claim)
        {
            // Table Header style
            ArrayList headerContent = new ArrayList();
            ArrayList headerStyle = new ArrayList();

            headerStyle.Add("STYLE"); headerContent.Add("HEADER");
            headerStyle.Add(""); headerContent.Add("Next D/N No.");
            headerStyle.Add(""); headerContent.Add("Claim Type");
            headerStyle.Add(""); headerContent.Add("Item No.");
            headerStyle.Add(""); headerContent.Add("Contract No.");
            headerStyle.Add(""); headerContent.Add("Vendor");
            headerStyle.Add("colspan=2"); headerContent.Add("Amount");

            // Detail row style
            ArrayList detailStyle = new ArrayList();
            detailStyle.Add("STYLE");
            detailStyle.Add("align='center'");
            detailStyle.Add("align='center'");
            detailStyle.Add("align='center'");
            detailStyle.Add("align='center'");
            detailStyle.Add("align='center'");
            detailStyle.Add("align='center' style='border-right-style:none;'");
            detailStyle.Add("align='right' style='border-left-style:none;'");


            // Content of detail row
            ArrayList rowDetail = new ArrayList();
            rowDetail.Add("DATA");
            rowDetail.Add(claim.UKDebitNoteNo);
            rowDetail.Add(claim.Type.Name);
            rowDetail.Add(claim.ItemNo);
            rowDetail.Add(claim.ContractNo);
            rowDetail.Add(claim.Vendor.Name);
            rowDetail.Add(claim.Currency.CurrencyCode);
            rowDetail.Add(claim.Amount.ToString("N02"));

            // Build the array for Next Claim DN
            ArrayList dnTable = new ArrayList();
            dnTable.Clear();
            dnTable.Add(headerStyle);
            dnTable.Add(headerContent);
            dnTable.Add(detailStyle);
            dnTable.Add(rowDetail);

            return dnTable;
        }

		private int getMaxClaimId()
		{
			IDataSetAdapter ad =  getDataSetAdapter("UKClaimApt", "GetMaxClaimId");
			DataSet dataSet = new DataSet();
			int recordsAffected = ad.Fill(dataSet);
			return ConvertUtility.trimDbInt(dataSet.Tables[0].Rows[0][0]);
		}

        public void deleteUKClaimRefund(int claimRefundId, int userId)
        {
            if (claimRefundId <= 0 || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimRefundDs dataSet = new UKClaimRefundDs();
                UKClaimRefundDs.UKClaimRefundRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimRefundApt", "GetUKClaimRefundByKey");
                ad.SelectCommand.Parameters["@ClaimRefundId"].Value = claimRefundId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected == 1)
                {
                    row = dataSet.UKClaimRefund[0];
                    row.Status = 0;
                    sealStamp(dataSet.UKClaimRefund[0], userId, Stamp.UPDATE);
                    recordsAffected = ad.Update(dataSet);
                    if (recordsAffected < 1)
                        throw new DataAccessException("UpdateUKClaimRefund ERROR");
                }
                else
                    throw new DataAccessException("UpdateUKClaimRefund ERROR (No Record Found)");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }


        public void setClaimWorkflowStatus(int claimId, int workflowStatusId, int userId)
        {
            if (claimId <= 0 || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimDs dataSet = new UKClaimDs();
                UKClaimDs.UKClaimRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimByKey");
                ad.SelectCommand.Parameters["@ClaimId"].Value = claimId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaim[0];
                    row.WorkflowStatusId = workflowStatusId;
                    sealStamp(dataSet.UKClaim[0], userId, Stamp.UPDATE);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaim ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public void updateUKClaim(UKClaimDef def, int userId)
        {
            if (def == null || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimDs dataSet = new UKClaimDs();
                UKClaimDs.UKClaimRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetUKClaimByKey");
                ad.SelectCommand.Parameters["@ClaimId"].Value = def.ClaimId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaim[0];
                    this.UKClaimMapping(def, row);
                    sealStamp(dataSet.UKClaim[0], userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.UKClaim.NewUKClaimRow();
                    row.WorkflowStatusId = ClaimWFS.NEW.Id;
                    int newId = this.getMaxClaimId() + 1;
                    def.ClaimId = newId;
                    row.Status = 1;
                    if (def.OfficeId == OfficeId.SH.Id)
                    {
                        if (def.Vendor.VendorId == 10483 // SH Footwear assigned to HK office
                            || def.Vendor.VendorId == 12996
                            || def.Vendor.VendorId == 12971
                            || def.Vendor.VendorId == 13391
                            || def.Vendor.VendorId == 9027
                            || def.Vendor.VendorId == 13450
                            || def.Vendor.VendorId == 10494
                            || def.Vendor.VendorId == 14040
                            || def.Vendor.VendorId == 14125
                            || def.Vendor.VendorId == 12800
                            || def.Vendor.VendorId == 6809
                            || def.Vendor.VendorId == 13591
                            || def.Vendor.VendorId == 10582)
                        {
                            def.OfficeId = OfficeId.HK.Id;
                            def.HandlingOfficeId = OfficeId.HK.Id;
                        }
                    }
                    this.UKClaimMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.UKClaim.AddUKClaimRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaim ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        private int getMaxLogId()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimLogApt", "GetMaxClaimLogId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            return ConvertUtility.trimDbInt(dataSet.Tables[0].Rows[0][0]);
        }

        public List<UKClaimLogDef> getUKClaimLogListByClaimId(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimLogApt", "GetUKClaimLogListByClaimId");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;

            UKClaimLogDs dataSet = new UKClaimLogDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimLogDef> list = new List<UKClaimLogDef>();

            foreach (UKClaimLogDs.UKClaimLogRow row in dataSet.UKClaimLog)
            {
                UKClaimLogDef def = new UKClaimLogDef();
                UKClaimLogMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public UKClaimLogDef getUKClaimLogByKey(int logId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimLogApt", "GetUKClaimLogByKey");
            ad.SelectCommand.Parameters["@LogId"].Value = logId.ToString();

            UKClaimLogDs dataSet = new UKClaimLogDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimLogDs.UKClaimLogRow row = dataSet.UKClaimLog[0];
            UKClaimLogDef def = new UKClaimLogDef();
            UKClaimLogMapping(row, def);
            return def;
        }

        public UKClaimDCNoteDetailDef getUKClaimDCNoteDetailByLogicalKey(int claimId, int claimRefundId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteDetailApt", "GetUKClaimDCNoteDetailByLogicalKey");
            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;
            ad.SelectCommand.Parameters["@ClaimRefundId"].Value = claimRefundId;

            UKClaimDCNoteDetailDs dataSet = new UKClaimDCNoteDetailDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow row = dataSet.UKClaimDCNoteDetail[0];
            UKClaimDCNoteDetailDef def = new UKClaimDCNoteDetailDef();
            UKClaimDCNoteDetailMapping(row, def);
            return def;
        }

        public void updateUKClaimLog(UKClaimLogDef def, int userId)
        {
            if (def == null || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();


                UKClaimLogDs dataSet = new UKClaimLogDs();
                UKClaimLogDs.UKClaimLogRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimLogApt", "GetUKClaimLogByKey");
                ad.SelectCommand.Parameters["@LogId"].Value = def.LogId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaimLog[0];
                    this.UKClaimLogMapping(def, row);
                }
                else
                {
                    row = dataSet.UKClaimLog.NewUKClaimLogRow();
                    int newId = this.getMaxLogId() + 1;
                    def.LogId = newId;
                    this.UKClaimLogMapping(def, row);
                    dataSet.UKClaimLog.AddUKClaimLogRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaimLog ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public List<string> getUKClaimBIAMappingList(int parentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimBIAMappingApt", "GetUKClaimBIAMappingList");

            ad.SelectCommand.Parameters["@ParentId"].Value = parentId;

            UKClaimBIAMappingDs dataSet = new UKClaimBIAMappingDs();
            int recordsAffected = ad.Fill(dataSet);

            List<string> list = new List<string>();

            foreach (UKClaimBIAMappingDs.UKClaimBIAMappingRow row in dataSet.UKClaimBIAMapping)
            {
                list.Add(row.ClaimId.ToString());
            }
            return list;
        }

        public bool isHomeAndBeautyProductTeam(int productTeamId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimApt", "GetHomeAndBeautyProductTeam");

            ad.SelectCommand.Parameters["@ProductTeamId"].Value = productTeamId;

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
                return true;
            else
                return false;
        }

        public List<string> getUKClaimBIAMappingByKey(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimBIAMappingApt", "GetUKClaimBIAMappingByKey");
            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId.ToString();

            UKClaimBIAMappingDs dataSet = new UKClaimBIAMappingDs();
            int recordsAffected = ad.Fill(dataSet);
            List<string> list = new List<string>();

            foreach (UKClaimBIAMappingDs.UKClaimBIAMappingRow row in dataSet.UKClaimBIAMapping)
            {
                list.Add(row.ClaimId.ToString());
            }
            return list;
        }

        public void updateUKClaimBIAMapping(int claimId, int parentId, int userId)
        {
            if (claimId <= 0 || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimBIAMappingDs dataSet = new UKClaimBIAMappingDs();
                UKClaimBIAMappingDs.UKClaimBIAMappingRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimBIAMappingApt", "GetUKClaimBIAMappingByKey");
                ad.SelectCommand.Parameters["@ClaimId"].Value = claimId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaimBIAMapping[0];
                    row.ParentId = parentId;
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.UKClaimBIAMapping.NewUKClaimBIAMappingRow();
                    row.ClaimId = claimId;
                    row.ParentId = parentId;
                    row.Status = GeneralStatus.ACTIVE.Code;
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.UKClaimBIAMapping.AddUKClaimBIAMappingRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaimBIAMapping ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public void updateUKClaimBIADiscrepancy(UKClaimBIADiscrepancyDef def, int userId)
        {
            if (def == null || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimBIADiscrepancyDs dataSet = new UKClaimBIADiscrepancyDs();
                UKClaimBIADiscrepancyDs.UKClaimBIADiscrepancyRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimBIADiscrepancyApt", "GetUKClaimBIADiscrepancyByKey");
                ad.SelectCommand.Parameters["@ClaimId"].Value = def.ClaimId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaimBIADiscrepancy[0];
                    this.UKClaimBIADiscrepancyMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.UKClaimBIADiscrepancy.NewUKClaimBIADiscrepancyRow();
                    this.UKClaimBIADiscrepancyMapping(def, row); 
                    row.Status = GeneralStatus.ACTIVE.Code;
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.UKClaimBIADiscrepancy.AddUKClaimBIADiscrepancyRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaimBIADiscrepancy ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public void updateUKClaimDCNote(UKClaimDCNoteDef def, int userId)
        {
            if (def == null || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimDCNoteDs dataSet = new UKClaimDCNoteDs();
                UKClaimDCNoteDs.UKClaimDCNoteRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteApt", "GetUKClaimDCNoteByKey");
                ad.SelectCommand.Parameters["@DCNoteId"].Value = def.DCNoteId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaimDCNote[0];
                    this.UKClaimDCNoteMapping(def, row);
                    sealStamp(dataSet.UKClaimDCNote[0], userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.UKClaimDCNote.NewUKClaimDCNoteRow();
                    int newId = this.getMaxDCNoteId() + 1;
                    def.DCNoteId = newId;
                    row.Status = 1;
                    this.UKClaimDCNoteMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.UKClaimDCNote.AddUKClaimDCNoteRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaimDCNote ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public void updateUKClaimDCNoteDetail(UKClaimDCNoteDetailDef def, int userId)
        {
            if (def == null || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimDCNoteDetailDs dataSet = new UKClaimDCNoteDetailDs();
                UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteDetailApt", "GetUKClaimDCNoteDetailByKey");
                ad.SelectCommand.Parameters["@DCNoteDetailId"].Value = def.DCNoteDetailId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaimDCNoteDetail[0];
                    this.UKClaimDCNoteDetailMapping(def, row);
                    sealStamp(dataSet.UKClaimDCNoteDetail[0], userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.UKClaimDCNoteDetail.NewUKClaimDCNoteDetailRow();
                    int newId = this.getMaxDCNoteDetailId() + 1;
                    def.DCNoteDetailId = newId;
                    row.Status = 1;
                    this.UKClaimDCNoteDetailMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.UKClaimDCNoteDetail.AddUKClaimDCNoteDetailRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaimDCNoteDetail ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        /*
        public void updateUKClaimDCNoteDetailListForVoid(int dcNoteId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                List<UKClaimDCNoteDetailDef> detailList = getUKClaimDCNoteDetailListByDCNoteId(dcNoteId);
                foreach (UKClaimDCNoteDetailDef dDef in detailList)
                {
                    UKClaimDCNoteDetailDef detailDef = (UKClaimDCNoteDetailDef)dDef.Clone();
                    detailDef.DCNoteId = dcNoteId;
                    detailDef.DCNoteDetailId = -1;
                    detailDef.RechargeableAmount = 0;
                    detailDef.ClaimRefundId = -1;
                    detailDef.LineRemark = string.Empty;
                    updateUKClaimDCNoteDetail(detailDef, userId);
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
        */

        private int getMaxDCNoteId()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteApt", "GetMaxDCNoteId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            return ConvertUtility.trimDbInt(dataSet.Tables[0].Rows[0][0]);
        }

        private int getMaxDCNoteDetailId()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimDCNoteDetailApt", "GetMaxDCNoteDetailId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            return ConvertUtility.trimDbInt(dataSet.Tables[0].Rows[0][0]);
        }

        private int getMaxClaimRefundId()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimRefundApt", "GetMaxClaimRefundId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            return ConvertUtility.trimDbInt(dataSet.Tables[0].Rows[0][0]);
        }


        public void updateUKClaimRefund(UKClaimRefundDef def, int userId)
        {
            if (def == null || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimRefundDs dataSet = new UKClaimRefundDs();
                UKClaimRefundDs.UKClaimRefundRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimRefundApt", "GetUKClaimRefundByKey");
                ad.SelectCommand.Parameters["@ClaimRefundId"].Value = def.ClaimRefundId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaimRefund[0];
                    this.UKClaimRefundMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.UKClaimRefund.NewUKClaimRefundRow();
                    int newId = this.getMaxClaimRefundId() + 1;
                    def.ClaimRefundId = newId;
                    this.UKClaimRefundMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.UKClaimRefund.AddUKClaimRefundRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaimRefund ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public bool isMultiUKClaimDCNote(int dcNoteId)
        {
            List<UKClaimDCNoteDetailDef> list = this.getUKClaimDCNoteDetailListByDCNoteId(dcNoteId);
            foreach (UKClaimDCNoteDetailDef def in list)
            {
                if (def.ClaimId == 0 && def.ClaimRefundId == 0)
                    return true;
            }

            return false;
        }

        public void updateUKClaimRecharge(int claimId, int claimRefundId, int officeId, int vendorId, int currencyId, decimal amt, decimal rechargeAmt)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimRechargeDs dataSet = new UKClaimRechargeDs();
                UKClaimRechargeDs.UKClaimRechargeRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKClaimRechargeApt", "GetUKClaimRechargeByKey");
                ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;
                ad.SelectCommand.Parameters["@ClaimRefundId"].Value = claimRefundId;
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKClaimRecharge[0];
                    row.CurrencyId = currencyId;
                    row.OfficeId = officeId;
                    row.VendorId = vendorId;
                    row.Amount = amt;
                    row.RechargeableAmt = rechargeAmt;
                }
                else
                {
                    row = dataSet.UKClaimRecharge.NewUKClaimRechargeRow();
                    row.ClaimId = claimId;
                    row.ClaimRefundId = claimRefundId;
                    row.CurrencyId = currencyId;
                    row.OfficeId = officeId;
                    row.VendorId = vendorId;
                    row.Amount = amt;
                    row.RechargeableAmt = rechargeAmt;
                    dataSet.UKClaimRecharge.AddUKClaimRechargeRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKClaimRecharge ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public UKClaimRefundDef getUKClaimRefundByKey(int claimRefundId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimRefundApt", "GetUKClaimRefundByKey");
            ad.SelectCommand.Parameters["@ClaimRefundId"].Value = claimRefundId.ToString();

            UKClaimRefundDs dataSet = new UKClaimRefundDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKClaimRefundDs.UKClaimRefundRow row = dataSet.UKClaimRefund[0];
            UKClaimRefundDef def = new UKClaimRefundDef();
            UKClaimRefundMapping(row, def);
            return def;
        }

        public List<UKClaimRefundDef> getUKClaimRefundListByClaimId(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimRefundApt", "GetUKClaimRefundListByClaimId");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;

            UKClaimRefundDs dataSet = new UKClaimRefundDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimRefundDef> list = new List<UKClaimRefundDef>();

            foreach (UKClaimRefundDs.UKClaimRefundRow row in dataSet.UKClaimRefund)
            {
                UKClaimRefundDef def = new UKClaimRefundDef();
                UKClaimRefundMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimRefundDef> getUKClaimRefundCOOApprovalList(TypeCollector officeIds, int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimRefundApt", "GetUKClaimRefundCOOApprovalList");

            ad.SelectCommand.CustomParameters["@OfficeIds"] = CustomDataParameter.parse(officeIds.IsInclusive, officeIds.Values);
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            UKClaimRefundDs dataSet = new UKClaimRefundDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimRefundDef> list = new List<UKClaimRefundDef>();

            foreach (UKClaimRefundDs.UKClaimRefundRow row in dataSet.UKClaimRefund)
            {
                UKClaimRefundDef def = new UKClaimRefundDef();
                UKClaimRefundMapping(row, def);
                list.Add(def);
            }
            return list;
        }


        public List<UKClaimDCNoteDef> getOutstandingUKClaimDCNoteList(int officeGroupId, int termOfPurchaseId, int userId)
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("UKClaimDCNoteApt", "GetOutstandingUKClaimDCNoteList");

            ad.SelectCommand.Parameters["@OfficeGroupId"].Value = officeGroupId;
            ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            
            UKClaimDCNoteDs dataSet = new UKClaimDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDCNoteDef> list = new List<UKClaimDCNoteDef>();

            foreach (UKClaimDCNoteDs.UKClaimDCNoteRow row in dataSet.UKClaimDCNote)
            {
                UKClaimDCNoteDef def = new UKClaimDCNoteDef();
                this.UKClaimDCNoteMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<GenericDataSummaryDef> getOutstandingUKClaimByCurrency(int vendorId, int officeId)
        {
            IDataSetAdapter dataSetAdapter = getDataSetAdapter("GenericDataSummaryApt", "GetOutstandingUKClaimByCurrency");
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

        public List<UKClaimDCNoteDef> getOutstandingFullRefundUKClaimDCNoteList(int officeGroupId, int termOfPurchaseId, int userId)
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("UKClaimDCNoteApt", "GetOutstandingFullRefundUKClaimDCNoteList");

            ad.SelectCommand.Parameters["@OfficeGroupId"].Value = officeGroupId;
            ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            UKClaimDCNoteDs dataSet = new UKClaimDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDCNoteDef> list = new List<UKClaimDCNoteDef>();

            foreach (UKClaimDCNoteDs.UKClaimDCNoteRow row in dataSet.UKClaimDCNote)
            {
                UKClaimDCNoteDef def = new UKClaimDCNoteDef();
                this.UKClaimDCNoteMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public UKClaimDCNoteDef getOutstandingFullRefundUKClaimDCNoteByClaimId(int claimId, int userId)
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("UKClaimDCNoteApt", "GetOutstandingFullRefundUKClaimDCNoteByClaimId");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            UKClaimDCNoteDs dataSet = new UKClaimDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0)
                return null;
            else
            {
                UKClaimDCNoteDs.UKClaimDCNoteRow row = dataSet.UKClaimDCNote[0];
                UKClaimDCNoteDef def = new UKClaimDCNoteDef();
                this.UKClaimDCNoteMapping(row, def);
                return def;
            }
        }

        public List<UKClaimDCNoteDef> getFullRefundUKClaimDCNoteList(int userId)
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("UKClaimDCNoteApt", "GetFullRefundUKClaimDCNoteList");

            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            UKClaimDCNoteDs dataSet = new UKClaimDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDCNoteDef> list = new List<UKClaimDCNoteDef>();

            foreach (UKClaimDCNoteDs.UKClaimDCNoteRow row in dataSet.UKClaimDCNote)
            {
                UKClaimDCNoteDef def = new UKClaimDCNoteDef();
                this.UKClaimDCNoteMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimDCNoteDef> getUKClaimDCNoteMailList(int mailStatus)
        {
            IDataSetAdapter ad = null;
            ad = getDataSetAdapter("UKClaimDCNoteApt", "GetUKClaimDCNoteMailList");
            ad.SelectCommand.Parameters["@MailStatus"].Value = mailStatus;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            UKClaimDCNoteDs dataSet = new UKClaimDCNoteDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimDCNoteDef> list = new List<UKClaimDCNoteDef>();

            foreach (UKClaimDCNoteDs.UKClaimDCNoteRow row in dataSet.UKClaimDCNote)
            {
                UKClaimDCNoteDef def = new UKClaimDCNoteDef();
                this.UKClaimDCNoteMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKClaimLogDef> getRefundSupportingUploadLog(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKClaimLogApt", "GetRefundSupportingUploadLog");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;

            UKClaimLogDs dataSet = new UKClaimLogDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKClaimLogDef> list = new List<UKClaimLogDef>();

            foreach (UKClaimLogDs.UKClaimLogRow row in dataSet.UKClaimLog)
            {
                UKClaimLogDef def = new UKClaimLogDef();
                UKClaimLogMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKDiscountClaimLogDef> getDiscountRefundSupportingUploadLog(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimLogApt", "GetRefundSupportingUploadLog");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;

            UKDiscountClaimLogDs dataSet = new UKDiscountClaimLogDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKDiscountClaimLogDef> list = new List<UKDiscountClaimLogDef>();

            foreach (UKDiscountClaimLogDs.UKDiscountClaimLogRow row in dataSet.UKDiscountClaimLog)
            {
                UKDiscountClaimLogDef def = new UKDiscountClaimLogDef();
                UKDiscountClaimLogMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKDiscountClaimDef> getOutstandingUKDiscountClaimReport(int officeId, int vendorId, int termOfPurchaseId, DateTime cutoffDate, int handlingOfficeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimApt", "GetOutstandingUKDiscountClaimReport");

            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = termOfPurchaseId;
            ad.SelectCommand.Parameters["@CutOffDate"].Value = cutoffDate;
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;
            ad.SelectCommand.MailSQL = true;

            UKDiscountClaimDs dataSet = new UKDiscountClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKDiscountClaimDef> list = new List<UKDiscountClaimDef>();

            foreach (UKDiscountClaimDs.UKDiscountClaimRow row in dataSet.UKDiscountClaim)
            {
                UKDiscountClaimDef def = new UKDiscountClaimDef();
                UKDiscountClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKDiscountClaimDef> getUKDiscountClaimListByCriteria(int officeId, int handlingOfficeId, string ukDebitNoteNo, int vendorId, string itemNo, string contractNo, DateTime fromDate, DateTime toDate, DateTime fromReceivedDate, DateTime toReceivedDate, bool nextDNNo, bool appliedUKDiscount)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimApt", "GetUKDiscountClaimListByCriteria");

            ad.SelectCommand.Parameters["@UKDebitNoteNo"].Value = ukDebitNoteNo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;
            if (fromDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromDate"].Value = SqlDateTime.MinValue;
            else
                ad.SelectCommand.Parameters["@FromDate"].Value = fromDate;
            if (toDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToDate"].Value = SqlDateTime.MaxValue;
            else
                ad.SelectCommand.Parameters["@ToDate"].Value = toDate;

            if (fromReceivedDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromReceivedDate"].Value = SqlDateTime.MinValue;
            else
                ad.SelectCommand.Parameters["@FromReceivedDate"].Value = fromReceivedDate;
            if (toReceivedDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToReceivedDate"].Value = SqlDateTime.MaxValue;
            else
                ad.SelectCommand.Parameters["@ToReceivedDate"].Value = toReceivedDate;

            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;

            if (nextDNNo)
                ad.SelectCommand.Parameters["@NextDNNo"].Value = 1;
            else
                ad.SelectCommand.Parameters["@NextDNNo"].Value = -1;

            if (appliedUKDiscount)
                ad.SelectCommand.Parameters["@AppliedUKDiscount"].Value = 1;
            else
                ad.SelectCommand.Parameters["@AppliedUKDiscount"].Value = -1;

            UKDiscountClaimDs dataSet = new UKDiscountClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKDiscountClaimDef> list = new List<UKDiscountClaimDef>();

            foreach (UKDiscountClaimDs.UKDiscountClaimRow row in dataSet.UKDiscountClaim)
            {
                UKDiscountClaimDef def = new UKDiscountClaimDef();
                UKDiscountClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKDiscountClaimRefundDef> getUKDiscountClaimRefundListByCriteria(int officeId, int handlingOfficeId, string ukDebitNoteNo, int vendorId, string itemNo, string contractNo, DateTime fromDate, DateTime toDate, DateTime fromReceivedDate, DateTime toReceivedDate)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimRefundApt", "GetUKDiscountClaimRefundListByCriteria");

            ad.SelectCommand.Parameters["@UKDebitNoteNo"].Value = ukDebitNoteNo;
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;
            ad.SelectCommand.Parameters["@HandlingOfficeId"].Value = handlingOfficeId;
            if (fromDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromDate"].Value = SqlDateTime.MinValue;
            else
                ad.SelectCommand.Parameters["@FromDate"].Value = fromDate;
            if (toDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToDate"].Value = SqlDateTime.MaxValue;
            else
                ad.SelectCommand.Parameters["@ToDate"].Value = toDate;

            if (fromReceivedDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromReceivedDate"].Value = SqlDateTime.MinValue;
            else
                ad.SelectCommand.Parameters["@FromReceivedDate"].Value = fromReceivedDate;
            if (toReceivedDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToReceivedDate"].Value = SqlDateTime.MaxValue;
            else
                ad.SelectCommand.Parameters["@ToReceivedDate"].Value = toReceivedDate;

            ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;

            UKDiscountClaimRefundDs dataSet = new UKDiscountClaimRefundDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKDiscountClaimRefundDef> list = new List<UKDiscountClaimRefundDef>();

            foreach (UKDiscountClaimRefundDs.UKDiscountClaimRefundRow row in dataSet.UKDiscountClaimRefund)
            {
                UKDiscountClaimRefundDef def = new UKDiscountClaimRefundDef();
                UKDiscountClaimRefundMapping(row, def);
                list.Add(def);
            }
            return list;
        }


        public void deleteUKDiscountClaim(int claimId, int userId)
        {
            if (claimId <= 0 || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKDiscountClaimDs dataSet = new UKDiscountClaimDs();
                UKDiscountClaimDs.UKDiscountClaimRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimApt", "GetUKDiscountClaimByKey");
                ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKDiscountClaim[0];
                    row.Status = 0;
                    sealStamp(dataSet.UKDiscountClaim[0], userId, Stamp.UPDATE);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKDiscountClaim ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }


        public UKDiscountClaimDef getUKDiscountClaimByKey(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimApt", "GetUKDiscountClaimByKey");
            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId.ToString();

            UKDiscountClaimDs dataSet = new UKDiscountClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKDiscountClaimDs.UKDiscountClaimRow row = dataSet.UKDiscountClaim[0];
            UKDiscountClaimDef def = new UKDiscountClaimDef();
            UKDiscountClaimMapping(row, def);
            return def;
        }

        private int getMaxDiscountClaimId()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimApt", "GetMaxDiscountClaimId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            return ConvertUtility.trimDbInt(dataSet.Tables[0].Rows[0][0]);
        }

        public void deleteUKDiscountClaimRefund(int claimRefundId, int userId)
        {
            if (claimRefundId <= 0 || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKDiscountClaimRefundDs dataSet = new UKDiscountClaimRefundDs();
                UKDiscountClaimRefundDs.UKDiscountClaimRefundRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimRefundApt", "GetUKDiscountClaimRefundByKey");
                ad.SelectCommand.Parameters["@ClaimRefundId"].Value = claimRefundId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected == 1)
                {
                    row = dataSet.UKDiscountClaimRefund[0];
                    row.Status = 0;
                    sealStamp(dataSet.UKDiscountClaimRefund[0], userId, Stamp.UPDATE);
                    recordsAffected = ad.Update(dataSet);
                    if (recordsAffected < 1)
                        throw new DataAccessException("UpdateUKDiscountClaimRefund ERROR");
                }
                else
                    throw new DataAccessException("UpdateUKDiscountClaimRefund ERROR (No Record Found)");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public void updateUKDiscountClaim(UKDiscountClaimDef def, int userId)
        {
            if (def == null || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKDiscountClaimDs dataSet = new UKDiscountClaimDs();
                UKDiscountClaimDs.UKDiscountClaimRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimApt", "GetUKDiscountClaimByKey");
                ad.SelectCommand.Parameters["@ClaimId"].Value = def.ClaimId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKDiscountClaim[0];
                    this.UKDiscountClaimMapping(def, row);
                    sealStamp(dataSet.UKDiscountClaim[0], userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.UKDiscountClaim.NewUKDiscountClaimRow();
                    row.WorkflowStatusId = ClaimWFS.NEW.Id;
                    int newId = this.getMaxDiscountClaimId() + 1;
                    def.ClaimId = newId;
                    row.Status = 1;
                    this.UKDiscountClaimMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.UKDiscountClaim.AddUKDiscountClaimRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKDiscountClaim ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        private int getMaxDiscountLogId()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimLogApt", "GetMaxDiscountClaimLogId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            return ConvertUtility.trimDbInt(dataSet.Tables[0].Rows[0][0]);
        }

        public List<UKDiscountClaimLogDef> getUKDiscountClaimLogListByClaimId(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimLogApt", "GetUKDiscountClaimLogListByClaimId");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;

            UKDiscountClaimLogDs dataSet = new UKDiscountClaimLogDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKDiscountClaimLogDef> list = new List<UKDiscountClaimLogDef>();

            foreach (UKDiscountClaimLogDs.UKDiscountClaimLogRow row in dataSet.UKDiscountClaimLog)
            {
                UKDiscountClaimLogDef def = new UKDiscountClaimLogDef();
                UKDiscountClaimLogMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public void updateUKDiscountClaimLog(UKDiscountClaimLogDef def, int userId)
        {
            if (def == null || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKDiscountClaimLogDs dataSet = new UKDiscountClaimLogDs();
                UKDiscountClaimLogDs.UKDiscountClaimLogRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimLogApt", "GetUKDiscountClaimLogByKey");
                ad.SelectCommand.Parameters["@LogId"].Value = def.LogId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKDiscountClaimLog[0];
                    this.UKDiscountClaimLogMapping(def, row);
                }
                else
                {
                    row = dataSet.UKDiscountClaimLog.NewUKDiscountClaimLogRow();
                    int newId = this.getMaxDiscountLogId() + 1;
                    def.LogId = newId;
                    this.UKDiscountClaimLogMapping(def, row);
                    dataSet.UKDiscountClaimLog.AddUKDiscountClaimLogRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKDiscountClaimLog ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public void updateUKDiscountClaimRefund(UKDiscountClaimRefundDef def, int userId)
        {
            if (def == null || userId == 0)
                return;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKDiscountClaimRefundDs dataSet = new UKDiscountClaimRefundDs();
                UKDiscountClaimRefundDs.UKDiscountClaimRefundRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimRefundApt", "GetUKDiscountClaimRefundByKey");
                ad.SelectCommand.Parameters["@ClaimRefundId"].Value = def.ClaimRefundId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKDiscountClaimRefund[0];
                    this.UKDiscountClaimRefundMapping(def, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.UKDiscountClaimRefund.NewUKDiscountClaimRefundRow();
                    int newId = this.getMaxClaimRefundId() + 1;
                    def.ClaimRefundId = newId;
                    this.UKDiscountClaimRefundMapping(def, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.UKDiscountClaimRefund.AddUKDiscountClaimRefundRow(row);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKDiscountClaimRefund ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }

        public UKDiscountClaimRefundDef getUKDiscountClaimRefundByKey(int claimRefundId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimRefundApt", "GetUKDiscountClaimRefundByKey");
            ad.SelectCommand.Parameters["@ClaimRefundId"].Value = claimRefundId.ToString();

            UKDiscountClaimRefundDs dataSet = new UKDiscountClaimRefundDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            UKDiscountClaimRefundDs.UKDiscountClaimRefundRow row = dataSet.UKDiscountClaimRefund[0];
            UKDiscountClaimRefundDef def = new UKDiscountClaimRefundDef();
            UKDiscountClaimRefundMapping(row, def);
            return def;
        }

        public List<UKDiscountClaimRefundDef> getUKDiscountClaimRefundListByClaimId(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimRefundApt", "GetUKDiscountClaimRefundListByClaimId");

            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;

            UKDiscountClaimRefundDs dataSet = new UKDiscountClaimRefundDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKDiscountClaimRefundDef> list = new List<UKDiscountClaimRefundDef>();

            foreach (UKDiscountClaimRefundDs.UKDiscountClaimRefundRow row in dataSet.UKDiscountClaimRefund)
            {
                UKDiscountClaimRefundDef def = new UKDiscountClaimRefundDef();
                UKDiscountClaimRefundMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public List<UKDiscountClaimDef> getOutstandingUKDiscountClaimList()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimApt", "GetOutstandingUKDiscountClaimList");

            UKDiscountClaimDs dataSet = new UKDiscountClaimDs();
            int recordsAffected = ad.Fill(dataSet);

            List<UKDiscountClaimDef> list = new List<UKDiscountClaimDef>();

            foreach (UKDiscountClaimDs.UKDiscountClaimRow row in dataSet.UKDiscountClaim)
            {
                UKDiscountClaimDef def = new UKDiscountClaimDef();
                UKDiscountClaimMapping(row, def);
                list.Add(def);
            }
            return list;
        }

        public void setUKDiscountClaimWorkflowStatus(int claimId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKDiscountClaimDs dataSet = new UKDiscountClaimDs();
                UKDiscountClaimDs.UKDiscountClaimRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimApt", "GetUKDiscountClaimByKey");
                ad.SelectCommand.Parameters["@ClaimId"].Value = claimId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKDiscountClaim[0];
                    row.WorkflowStatusId = UKDiscountClaimWFS.CLEARED.Id;
                    sealStamp(dataSet.UKDiscountClaim[0], 99999, Stamp.UPDATE);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKDiscountClaim ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }


        public void setUKDiscountClaimRefundWorkflowStatus(int claimId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKDiscountClaimRefundDs dataSet = new UKDiscountClaimRefundDs();
                UKDiscountClaimRefundDs.UKDiscountClaimRefundRow row = null;
                IDataSetAdapter ad = getDataSetAdapter("UKDiscountClaimRefundApt", "GetUKDiscountClaimRefundListByClaimId");
                ad.SelectCommand.Parameters["@ClaimId"].Value = claimId.ToString();
                ad.PopulateCommands();

                int recordsAffected = ad.Fill(dataSet);

                if (recordsAffected > 0)
                {
                    row = dataSet.UKDiscountClaimRefund[0];
                    row.WorkflowStatusId = UKDiscountClaimWFS.CLEARED.Id;
                    sealStamp(dataSet.UKDiscountClaimRefund[0], 99999, Stamp.UPDATE);
                }

                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("UpdateUKDiscountClaimRefund ERROR");

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            { ctx.Exit(); }
        }


		/*
		 * Mapping Methods, This takes care of the swapping between
		 * DataSet and Domain Object. And its supposed to be temporary.
		 * Should aim for the auto mapping using XML
		 * */
		#region Mapping

		private void UKClaimMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(UKClaimDs.UKClaimRow) &&
				target.GetType() == typeof(UKClaimDef))
			{
                UKClaimDs.UKClaimRow row = (UKClaimDs.UKClaimRow)source;
				UKClaimDef def = (UKClaimDef) target;

				def.ClaimId = row.ClaimId;
                def.Type = UKClaimType.getType(row.ClaimTypeId);
                if (!row.IsItemNoNull())
                    def.ItemNo = row.ItemNo;
                else
                    def.ItemNo = string.Empty;
                if (!row.IsContractNoNull())
                    def.ContractNo = row.ContractNo;
                else
                    def.ContractNo = string.Empty;
                def.OfficeId = row.OfficeId;
                def.HandlingOfficeId = row.HandlingOfficeId;
                def.Vendor = VendorWorker.Instance.getVendorByKey(row.VendorId);
                if (!row.IsSZVendorIdNull())
                    def.SZVendor = VendorWorker.Instance.getVendorByKey(row.SZVendorId);
                else
                    def.SZVendor = null;
                def.ProductTeamId = row.ProductTeamId;
                def.TermOfPurchaseId = row.TermOfPurchaseId;
                def.Quantity = row.Qty;
                def.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
                def.Amount = row.Amount;
                def.HasUKDebitNote = row.HasUKDN;
                if (!row.IsUKDebitNoteNoNull())
                    def.UKDebitNoteNo = row.UKDebitNoteNo;
                else
                    def.UKDebitNoteNo = string.Empty;
                if (!row.IsUKDebitNoteDateNull())
                    def.UKDebitNoteDate = row.UKDebitNoteDate;
                else
                    def.UKDebitNoteDate = DateTime.MinValue;
                if (!row.IsUKDebitNoteReceivedDateNull())
                    def.UKDebitNoteReceivedDate = row.UKDebitNoteReceivedDate;
                else
                    def.UKDebitNoteReceivedDate = DateTime.MinValue;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                if (!row.IsClaimRequestIdNull())
                    def.ClaimRequestId = row.ClaimRequestId;
                else
                    def.ClaimRequestId = -1;
                if (!row.IsDebitNoteNoNull())
                    def.DebitNoteNo = row.DebitNoteNo;
                else
                    def.DebitNoteNo = string.Empty;
                if (!row.IsDebitNoteDateNull())
                    def.DebitNoteDate = row.DebitNoteDate;
                else
                    def.DebitNoteDate = DateTime.MinValue;
                def.DebitNoteAmount = row.DebitNoteAmt;
                if (!row.IsClaimMonthNull())
                    def.ClaimMonth = row.ClaimMonth;
                else
                    def.ClaimMonth = string.Empty;
                if (!row.IsPnLAccountCodeNull())
                    def.PnLAccountCode = row.PnLAccountCode;
                else
                    def.PnLAccountCode = string.Empty;
                def.IsInterfaced = row.IsInterfaced;
                def.IsRechargeInterfaced = row.IsRechargeInterfaced;
                def.IsReadyForSettlement = row.IsReadyForSettlement;
                def.SettlementOption = UKClaimSettlemtType.getType(row.SettlementOptionId);
                if (!row.IsPaymentOfficeIdNull())
                    def.PaymentOfficeId = row.PaymentOfficeId;
                else
                    def.PaymentOfficeId = row.OfficeId;
                def.WorkflowStatus = ClaimWFS.getType(row.WorkflowStatusId);
                def.WorkflowStatusId = def.WorkflowStatus.Id;
                def.GUId = row.GUId;
                def.CreateDate = row.CreatedOn;
			}
			else if (source.GetType() == typeof(UKClaimDef) &&
				target.GetType() == typeof(UKClaimDs.UKClaimRow))
			{
				UKClaimDef def = (UKClaimDef) source;
                UKClaimDs.UKClaimRow row = (UKClaimDs.UKClaimRow)target;

				row.ClaimId = def.ClaimId;
                row.ClaimTypeId = def.Type.Id;
                if (def.ItemNo.Trim() != string.Empty)
                    row.ItemNo = def.ItemNo;
                else
                    row.SetItemNoNull();
                if (def.ContractNo.Trim() != string.Empty)
                    row.ContractNo = def.ContractNo;
                else
                    row.SetContractNoNull();
                row.OfficeId = def.OfficeId;
                row.HandlingOfficeId = def.HandlingOfficeId;
                row.VendorId = def.Vendor.VendorId;
                if (def.SZVendor != null)
                    row.SZVendorId = def.SZVendor.VendorId;
                else
                    row.SetSZVendorIdNull();
                row.ProductTeamId = def.ProductTeamId;
                row.TermOfPurchaseId = def.TermOfPurchaseId;
                row.Qty = def.Quantity;
                row.CurrencyId = def.Currency.CurrencyId;
                row.Amount = def.Amount;
                row.HasUKDN = def.HasUKDebitNote;
                if (def.UKDebitNoteNo.Trim() != string.Empty)
                    row.UKDebitNoteNo = def.UKDebitNoteNo;
                else
                    row.SetUKDebitNoteNoNull();
                if (def.UKDebitNoteDate != DateTime.MinValue)
                    row.UKDebitNoteDate = def.UKDebitNoteDate;
                else
                    row.SetUKDebitNoteDateNull();
                if (def.UKDebitNoteReceivedDate != DateTime.MinValue)
                    row.UKDebitNoteReceivedDate = def.UKDebitNoteReceivedDate;
                else
                    row.SetUKDebitNoteReceivedDateNull();
                if (def.Remark.Trim() != string.Empty)
                    row.Remark = def.Remark;
                else
                    row.SetRemarkNull();
                if (def.ClaimRequestId != -1)
                    row.ClaimRequestId = def.ClaimRequestId;
                else
                    row.SetClaimRequestIdNull();
                if (def.DebitNoteNo.Trim() != string.Empty)
                    row.DebitNoteNo = def.DebitNoteNo;
                else
                    row.SetDebitNoteNoNull();
                if (def.DebitNoteDate != DateTime.MinValue)
                    row.DebitNoteDate = def.DebitNoteDate;
                else
                    row.SetDebitNoteDateNull();
                if (def.ClaimMonth == string.Empty)
                    row.SetClaimMonthNull();
                else
                    row.ClaimMonth = def.ClaimMonth;
                if (def.PnLAccountCode == string.Empty)
                    row.SetPnLAccountCodeNull();
                else
                    row.PnLAccountCode = def.PnLAccountCode;
                row.DebitNoteAmt = def.DebitNoteAmount;
                row.IsInterfaced = def.IsInterfaced;
                row.IsRechargeInterfaced = def.IsRechargeInterfaced;
                row.IsReadyForSettlement = def.IsReadyForSettlement;
                row.SettlementOptionId = def.SettlementOption.Id;
                if (def.PaymentOfficeId == -1)
                    row.PaymentOfficeId = def.OfficeId;
                else
                    row.PaymentOfficeId = def.PaymentOfficeId;
                row.WorkflowStatusId = def.WorkflowStatus.Id;
                row.GUId = def.GUId;
			}
		}

        private void UKClaimLogMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKClaimLogDs.UKClaimLogRow) &&
                target.GetType() == typeof(UKClaimLogDef))
            {
                UKClaimLogDs.UKClaimLogRow row = (UKClaimLogDs.UKClaimLogRow)source;
                UKClaimLogDef def = (UKClaimLogDef)target;

                def.LogId = row.LogId;
                def.ClaimId = row.ClaimId;
                def.LogText = row.LogText;
                if (!row.IsFromStatusIdNull())
                    def.FromStatusId = row.FromStatusId;
                else
                    def.FromStatusId = -1;
                def.ToStatusId = row.ToStatusId;
                def.UserId = row.UserId;
                def.LogDate = row.LogDate;
            }
            else if (source.GetType() == typeof(UKClaimLogDef) &&
                target.GetType() == typeof(UKClaimLogDs.UKClaimLogRow))
            {
                UKClaimLogDef def = (UKClaimLogDef)source;
                UKClaimLogDs.UKClaimLogRow row = (UKClaimLogDs.UKClaimLogRow)target;

                row.LogId = def.LogId;
                row.ClaimId = def.ClaimId;
                row.LogText = def.LogText;
                if (def.FromStatusId == -1)
                    row.SetFromStatusIdNull();
                else
                    row.FromStatusId = def.FromStatusId;
                row.ToStatusId = def.ToStatusId;
                row.UserId = def.UserId;
                row.LogDate = def.LogDate;
            }
        }

        private void UKClaimRefundMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKClaimRefundDs.UKClaimRefundRow) &&
                target.GetType() == typeof(UKClaimRefundDef))
            {
                UKClaimRefundDs.UKClaimRefundRow row = (UKClaimRefundDs.UKClaimRefundRow)source;
                UKClaimRefundDef def = (UKClaimRefundDef)target;

                def.ClaimRefundId = row.ClaimRefundId;
                def.ClaimId = row.ClaimId;
                if (row.IsReceivedDateNull())
                    def.ReceivedDate = DateTime.MinValue;
                else
                    def.ReceivedDate = row.ReceivedDate;
                def.Amount = row.Amount;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                def.IsInterfaced = row.IsInterfaced;
                def.IsRechargeInterfaced = row.IsRechargeInterfaced;
                if (!row.IsCreditNoteNoNull())
                    def.CreditNoteNo = row.CreditNoteNo;
                else
                    def.CreditNoteNo = string.Empty;
                if (!row.IsCreditNoteDateNull())
                    def.CreditNoteDate = row.CreditNoteDate;
                else
                    def.CreditNoteDate = DateTime.MinValue;
                def.CreditNoteAmount = row.CreditNoteAmt;
                def.IsReadyForSettlement = row.IsReadyForSettlement;
                def.SettlementOption = UKClaimSettlemtType.getType(row.SettlementOptionId);
                if (!row.IsPnLAccountCodeNull())
                    def.PnLAccountCode = row.PnLAccountCode;
                else
                    def.PnLAccountCode = string.Empty;
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(UKClaimRefundDef) &&
                target.GetType() == typeof(UKClaimRefundDs.UKClaimRefundRow))
            {
                UKClaimRefundDef def = (UKClaimRefundDef)source;
                UKClaimRefundDs.UKClaimRefundRow row = (UKClaimRefundDs.UKClaimRefundRow)target;

                row.ClaimRefundId = def.ClaimRefundId;
                row.ClaimId = def.ClaimId;
                row.ReceivedDate = def.ReceivedDate;
                if (def.Remark.Trim() == string.Empty)
                    row.SetRemarkNull();
                else
                    row.Remark = def.Remark.Trim();
                row.IsInterfaced = def.IsInterfaced;
                row.IsRechargeInterfaced = def.IsRechargeInterfaced;
                if (def.CreditNoteNo == string.Empty)
                    row.SetCreditNoteNoNull();
                else
                    row.CreditNoteNo = def.CreditNoteNo;
                if (def.CreditNoteDate == DateTime.MinValue)
                    row.SetCreditNoteDateNull();
                else
                    row.CreditNoteDate = def.CreditNoteDate;
                row.CreditNoteAmt = def.CreditNoteAmount;
                row.Amount = def.Amount;
                row.IsReadyForSettlement = def.IsReadyForSettlement;
                row.SettlementOptionId = def.SettlementOption.Id;
                if (def.PnLAccountCode == string.Empty)
                    row.SetPnLAccountCodeNull();
                else
                    row.PnLAccountCode = def.PnLAccountCode;
                row.Status = def.Status;
            }
        }

        private void UKClaimPhasingMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKClaimPhasingDs.UKClaimPhasingRow) &&
                target.GetType() == typeof(UKClaimPhasingDef))
            {
                UKClaimPhasingDs.UKClaimPhasingRow row = (UKClaimPhasingDs.UKClaimPhasingRow)source;
                UKClaimPhasingDef def = (UKClaimPhasingDef)target;

                def.OfficeId = row.OfficeId;
                def.VendorId = row.VendorId;
                def.CurrencyId = row.CurrencyId;
                def.Name = row.Name;
                def.ClaimTypeId = row.ClaimTypeId;
                def.ClaimReason = row.ClaimReason;
                def.P01Amount = row.P01Amt;
                def.P02Amount = row.P02Amt;
                def.P03Amount = row.P03Amt;
                def.P04Amount = row.P04Amt;
                def.P05Amount = row.P05Amt;
                def.P06Amount = row.P06Amt;
                def.P07Amount = row.P07Amt;
                def.P08Amount = row.P08Amt;
                def.P09Amount = row.P09Amt;
                def.P10Amount = row.P10Amt;
                def.P11Amount = row.P11Amt;
                def.P12Amount = row.P12Amt;
                def.TotalAmount = row.TotalAmt;

                def.P01NSAmount = row.P01NSAmt;
                def.P02NSAmount = row.P02NSAmt;
                def.P03NSAmount = row.P03NSAmt;
                def.P04NSAmount = row.P04NSAmt;
                def.P05NSAmount = row.P05NSAmt;
                def.P06NSAmount = row.P06NSAmt;
                def.P07NSAmount = row.P07NSAmt;
                def.P08NSAmount = row.P08NSAmt;
                def.P09NSAmount = row.P09NSAmt;
                def.P10NSAmount = row.P10NSAmt;
                def.P11NSAmount = row.P11NSAmt;
                def.P12NSAmount = row.P12NSAmt;
                def.TotalNSAmount = row.TotalNSAmt;

                def.P01VendorAmount = row.P01VendorAmt;
                def.P02VendorAmount = row.P02VendorAmt;
                def.P03VendorAmount = row.P03VendorAmt;
                def.P04VendorAmount = row.P04VendorAmt;
                def.P05VendorAmount = row.P05VendorAmt;
                def.P06VendorAmount = row.P06VendorAmt;
                def.P07VendorAmount = row.P07VendorAmt;
                def.P08VendorAmount = row.P08VendorAmt;
                def.P09VendorAmount = row.P09VendorAmt;
                def.P10VendorAmount = row.P10VendorAmt;
                def.P11VendorAmount = row.P11VendorAmt;
                def.P12VendorAmount = row.P12VendorAmt;
                def.TotalVendorAmount = row.TotalVendorAmt;
                
                def.P01AmountInUSD = row.P01AmtInUSD;
                def.P02AmountInUSD = row.P02AmtInUSD;
                def.P03AmountInUSD = row.P03AmtInUSD;
                def.P04AmountInUSD = row.P04AmtInUSD;
                def.P05AmountInUSD = row.P05AmtInUSD;
                def.P06AmountInUSD = row.P06AmtInUSD;
                def.P07AmountInUSD = row.P07AmtInUSD;
                def.P08AmountInUSD = row.P08AmtInUSD;
                def.P09AmountInUSD = row.P09AmtInUSD;
                def.P10AmountInUSD = row.P10AmtInUSD;
                def.P11AmountInUSD = row.P11AmtInUSD;
                def.P12AmountInUSD = row.P12AmtInUSD;
                def.TotalAmountInUSD = row.TotalAmtInUSD;

                def.P01NSAmountInUSD = row.P01NSAmtInUSD;
                def.P02NSAmountInUSD = row.P02NSAmtInUSD;
                def.P03NSAmountInUSD = row.P03NSAmtInUSD;
                def.P04NSAmountInUSD = row.P04NSAmtInUSD;
                def.P05NSAmountInUSD = row.P05NSAmtInUSD;
                def.P06NSAmountInUSD = row.P06NSAmtInUSD;
                def.P07NSAmountInUSD = row.P07NSAmtInUSD;
                def.P08NSAmountInUSD = row.P08NSAmtInUSD;
                def.P09NSAmountInUSD = row.P09NSAmtInUSD;
                def.P10NSAmountInUSD = row.P10NSAmtInUSD;
                def.P11NSAmountInUSD = row.P11NSAmtInUSD;
                def.P12NSAmountInUSD = row.P12NSAmtInUSD;
                def.TotalNSAmountInUSD = row.TotalNSAmtInUSD;

                def.P01VendorAmountInUSD = row.P01VendorAmtInUSD;
                def.P02VendorAmountInUSD = row.P02VendorAmtInUSD;
                def.P03VendorAmountInUSD = row.P03VendorAmtInUSD;
                def.P04VendorAmountInUSD = row.P04VendorAmtInUSD;
                def.P05VendorAmountInUSD = row.P05VendorAmtInUSD;
                def.P06VendorAmountInUSD = row.P06VendorAmtInUSD;
                def.P07VendorAmountInUSD = row.P07VendorAmtInUSD;
                def.P08VendorAmountInUSD = row.P08VendorAmtInUSD;
                def.P09VendorAmountInUSD = row.P09VendorAmtInUSD;
                def.P10VendorAmountInUSD = row.P10VendorAmtInUSD;
                def.P11VendorAmountInUSD = row.P11VendorAmtInUSD;
                def.P12VendorAmountInUSD = row.P12VendorAmtInUSD;
                def.TotalVendorAmountInUSD = row.TotalVendorAmtInUSD;

                def.LYP01AmountInUSD = row.LYP01AmtInUSD;
                def.LYP02AmountInUSD = row.LYP02AmtInUSD;
                def.LYP03AmountInUSD = row.LYP03AmtInUSD;
                def.LYP04AmountInUSD = row.LYP04AmtInUSD;
                def.LYP05AmountInUSD = row.LYP05AmtInUSD;
                def.LYP06AmountInUSD = row.LYP06AmtInUSD;
                def.LYP07AmountInUSD = row.LYP07AmtInUSD;
                def.LYP08AmountInUSD = row.LYP08AmtInUSD;
                def.LYP09AmountInUSD = row.LYP09AmtInUSD;
                def.LYP10AmountInUSD = row.LYP10AmtInUSD;
                def.LYP11AmountInUSD = row.LYP11AmtInUSD;
                def.LYP12AmountInUSD = row.LYP12AmtInUSD;
                def.LYTotalAmountInUSD = row.LYTotalAmtInUSD;

                def.LYP01NSAmountInUSD = row.LYP01NSAmtInUSD;
                def.LYP02NSAmountInUSD = row.LYP02NSAmtInUSD;
                def.LYP03NSAmountInUSD = row.LYP03NSAmtInUSD;
                def.LYP04NSAmountInUSD = row.LYP04NSAmtInUSD;
                def.LYP05NSAmountInUSD = row.LYP05NSAmtInUSD;
                def.LYP06NSAmountInUSD = row.LYP06NSAmtInUSD;
                def.LYP07NSAmountInUSD = row.LYP07NSAmtInUSD;
                def.LYP08NSAmountInUSD = row.LYP08NSAmtInUSD;
                def.LYP09NSAmountInUSD = row.LYP09NSAmtInUSD;
                def.LYP10NSAmountInUSD = row.LYP10NSAmtInUSD;
                def.LYP11NSAmountInUSD = row.LYP11NSAmtInUSD;
                def.LYP12NSAmountInUSD = row.LYP12NSAmtInUSD;
                def.LYTotalNSAmountInUSD = row.LYTotalNSAmtInUSD;

                def.LYP01VendorAmountInUSD = row.LYP01VendorAmtInUSD;
                def.LYP02VendorAmountInUSD = row.LYP02VendorAmtInUSD;
                def.LYP03VendorAmountInUSD = row.LYP03VendorAmtInUSD;
                def.LYP04VendorAmountInUSD = row.LYP04VendorAmtInUSD;
                def.LYP05VendorAmountInUSD = row.LYP05VendorAmtInUSD;
                def.LYP06VendorAmountInUSD = row.LYP06VendorAmtInUSD;
                def.LYP07VendorAmountInUSD = row.LYP07VendorAmtInUSD;
                def.LYP08VendorAmountInUSD = row.LYP08VendorAmtInUSD;
                def.LYP09VendorAmountInUSD = row.LYP09VendorAmtInUSD;
                def.LYP10VendorAmountInUSD = row.LYP10VendorAmtInUSD;
                def.LYP11VendorAmountInUSD = row.LYP11VendorAmtInUSD;
                def.LYP12VendorAmountInUSD = row.LYP12VendorAmtInUSD;
                def.LYTotalVendorAmountInUSD = row.LYTotalVendorAmtInUSD;

                if (!row.IsLatestShipmentDateNull())
                    def.LatestShipmentDate = row.LatestShipmentDate;
                else
                    def.LatestShipmentDate = DateTime.MinValue;
            }
        }


        private void UKClaimPhasingByProductTeamMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKClaimPhasingByProductTeamDs.UKClaimPhasingRow) &&
                target.GetType() == typeof(UKClaimPhasingByProductTeamDef))
            {
                UKClaimPhasingByProductTeamDs.UKClaimPhasingRow row = (UKClaimPhasingByProductTeamDs.UKClaimPhasingRow)source;
                UKClaimPhasingByProductTeamDef def = (UKClaimPhasingByProductTeamDef)target;

                def.OfficeId = row.OfficeId;
                def.ProductTeamId = row.ProductTeamId;
                def.CurrencyId = row.CurrencyId;
                def.Name = row.Name;
                def.ClaimTypeId = row.ClaimTypeId;
                def.P01Amount = row.P01Amt;
                def.P02Amount = row.P02Amt;
                def.P03Amount = row.P03Amt;
                def.P04Amount = row.P04Amt;
                def.P05Amount = row.P05Amt;
                def.P06Amount = row.P06Amt;
                def.P07Amount = row.P07Amt;
                def.P08Amount = row.P08Amt;
                def.P09Amount = row.P09Amt;
                def.P10Amount = row.P10Amt;
                def.P11Amount = row.P11Amt;
                def.P12Amount = row.P12Amt;
                def.TotalAmount = row.TotalAmt;
                def.P01AmountInUSD = row.P01AmtInUSD;
                def.P02AmountInUSD = row.P02AmtInUSD;
                def.P03AmountInUSD = row.P03AmtInUSD;
                def.P04AmountInUSD = row.P04AmtInUSD;
                def.P05AmountInUSD = row.P05AmtInUSD;
                def.P06AmountInUSD = row.P06AmtInUSD;
                def.P07AmountInUSD = row.P07AmtInUSD;
                def.P08AmountInUSD = row.P08AmtInUSD;
                def.P09AmountInUSD = row.P09AmtInUSD;
                def.P10AmountInUSD = row.P10AmtInUSD;
                def.P11AmountInUSD = row.P11AmtInUSD;
                def.P12AmountInUSD = row.P12AmtInUSD;
                def.TotalAmountInUSD = row.TotalAmtInUSD;

                def.P01NSAmountInUSD = row.P01NSAmtInUSD;
                def.P02NSAmountInUSD = row.P02NSAmtInUSD;
                def.P03NSAmountInUSD = row.P03NSAmtInUSD;
                def.P04NSAmountInUSD = row.P04NSAmtInUSD;
                def.P05NSAmountInUSD = row.P05NSAmtInUSD;
                def.P06NSAmountInUSD = row.P06NSAmtInUSD;
                def.P07NSAmountInUSD = row.P07NSAmtInUSD;
                def.P08NSAmountInUSD = row.P08NSAmtInUSD;
                def.P09NSAmountInUSD = row.P09NSAmtInUSD;
                def.P10NSAmountInUSD = row.P10NSAmtInUSD;
                def.P11NSAmountInUSD = row.P11NSAmtInUSD;
                def.P12NSAmountInUSD = row.P12NSAmtInUSD;
                def.TotalNSAmountInUSD = row.TotalNSAmtInUSD;

                def.P01VendorAmountInUSD = row.P01VendorAmtInUSD;
                def.P02VendorAmountInUSD = row.P02VendorAmtInUSD;
                def.P03VendorAmountInUSD = row.P03VendorAmtInUSD;
                def.P04VendorAmountInUSD = row.P04VendorAmtInUSD;
                def.P05VendorAmountInUSD = row.P05VendorAmtInUSD;
                def.P06VendorAmountInUSD = row.P06VendorAmtInUSD;
                def.P07VendorAmountInUSD = row.P07VendorAmtInUSD;
                def.P08VendorAmountInUSD = row.P08VendorAmtInUSD;
                def.P09VendorAmountInUSD = row.P09VendorAmtInUSD;
                def.P10VendorAmountInUSD = row.P10VendorAmtInUSD;
                def.P11VendorAmountInUSD = row.P11VendorAmtInUSD;
                def.P12VendorAmountInUSD = row.P12VendorAmtInUSD;
                def.TotalVendorAmountInUSD = row.TotalVendorAmtInUSD;

                def.LYP01AmountInUSD = row.LYP01AmtInUSD;
                def.LYP02AmountInUSD = row.LYP02AmtInUSD;
                def.LYP03AmountInUSD = row.LYP03AmtInUSD;
                def.LYP04AmountInUSD = row.LYP04AmtInUSD;
                def.LYP05AmountInUSD = row.LYP05AmtInUSD;
                def.LYP06AmountInUSD = row.LYP06AmtInUSD;
                def.LYP07AmountInUSD = row.LYP07AmtInUSD;
                def.LYP08AmountInUSD = row.LYP08AmtInUSD;
                def.LYP09AmountInUSD = row.LYP09AmtInUSD;
                def.LYP10AmountInUSD = row.LYP10AmtInUSD;
                def.LYP11AmountInUSD = row.LYP11AmtInUSD;
                def.LYP12AmountInUSD = row.LYP12AmtInUSD;
                def.LYTotalAmountInUSD = row.LYTotalAmtInUSD;

                def.LYP01NSAmountInUSD = row.LYP01NSAmtInUSD;
                def.LYP02NSAmountInUSD = row.LYP02NSAmtInUSD;
                def.LYP03NSAmountInUSD = row.LYP03NSAmtInUSD;
                def.LYP04NSAmountInUSD = row.LYP04NSAmtInUSD;
                def.LYP05NSAmountInUSD = row.LYP05NSAmtInUSD;
                def.LYP06NSAmountInUSD = row.LYP06NSAmtInUSD;
                def.LYP07NSAmountInUSD = row.LYP07NSAmtInUSD;
                def.LYP08NSAmountInUSD = row.LYP08NSAmtInUSD;
                def.LYP09NSAmountInUSD = row.LYP09NSAmtInUSD;
                def.LYP10NSAmountInUSD = row.LYP10NSAmtInUSD;
                def.LYP11NSAmountInUSD = row.LYP11NSAmtInUSD;
                def.LYP12NSAmountInUSD = row.LYP12NSAmtInUSD;
                def.LYTotalNSAmountInUSD = row.LYTotalNSAmtInUSD;

                def.LYP01VendorAmountInUSD = row.LYP01VendorAmtInUSD;
                def.LYP02VendorAmountInUSD = row.LYP02VendorAmtInUSD;
                def.LYP03VendorAmountInUSD = row.LYP03VendorAmtInUSD;
                def.LYP04VendorAmountInUSD = row.LYP04VendorAmtInUSD;
                def.LYP05VendorAmountInUSD = row.LYP05VendorAmtInUSD;
                def.LYP06VendorAmountInUSD = row.LYP06VendorAmtInUSD;
                def.LYP07VendorAmountInUSD = row.LYP07VendorAmtInUSD;
                def.LYP08VendorAmountInUSD = row.LYP08VendorAmtInUSD;
                def.LYP09VendorAmountInUSD = row.LYP09VendorAmtInUSD;
                def.LYP10VendorAmountInUSD = row.LYP10VendorAmtInUSD;
                def.LYP11VendorAmountInUSD = row.LYP11VendorAmtInUSD;
                def.LYP12VendorAmountInUSD = row.LYP12VendorAmtInUSD;
                def.LYTotalVendorAmountInUSD = row.LYTotalVendorAmtInUSD;

                if (!row.IsLatestShipmentDateNull())
                    def.LatestShipmentDate = row.LatestShipmentDate;
                else
                    def.LatestShipmentDate = DateTime.MinValue;
            }
        }


        private void UKClaimDCNoteMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKClaimDCNoteDs.UKClaimDCNoteRow) &&
                target.GetType() == typeof(UKClaimDCNoteDef))
            {
                UKClaimDCNoteDs.UKClaimDCNoteRow row = (UKClaimDCNoteDs.UKClaimDCNoteRow)source;
                UKClaimDCNoteDef def = (UKClaimDCNoteDef)target;

                def.DCNoteId = row.DCNoteId;
                if (row.IsDCNoteNoNull())
                    def.DCNoteNo = string.Empty;
                else
                    def.DCNoteNo = row.DCNoteNo;
                if (row.IsDCNoteDateNull())
                    def.DCNoteDate = DateTime.MinValue;
                else
                    def.DCNoteDate = row.DCNoteDate;
                def.OfficeId = row.OfficeId;
                def.CurrencyId = row.CurrencyId;
                if (row.IsRevisedCurrencyIdNull())
                    def.RevisedCurrencyId = -1;
                else
                    def.RevisedCurrencyId = row.RevisedCurrencyId;
                def.SettledAmount = row.SettledAmt;
                def.TotalAmount = row.TotalAmt;
                def.DebitCreditIndicator = row.DebitCreditIndicator;
                def.VendorId = row.VendorId;
                def.PartyName = row.PartyName;
                if (!row.IsPartyAddress1Null())
                    def.PartyAddress1 = row.PartyAddress1;
                else
                    def.PartyAddress1 = string.Empty;
                if (!row.IsPartyAddress2Null())
                    def.PartyAddress2 = row.PartyAddress2;
                else
                    def.PartyAddress2 = string.Empty;
                if (!row.IsPartyAddress3Null())
                    def.PartyAddress3 = row.PartyAddress3;
                else
                    def.PartyAddress3 = string.Empty;
                if (!row.IsPartyAddress4Null())
                    def.PartyAddress4 = row.PartyAddress4;
                else
                    def.PartyAddress4 = string.Empty;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                def.IsCustom = row.IsCustom;
                def.IsInterfaced = row.IsInterfaced;
                def.MailStatus = row.MailStatus;
                if (!row.IsSettlementDateNull())
                    def.SettlementDate = row.SettlementDate;
                else
                    def.SettlementDate = DateTime.MinValue;
                def.IsVoid = row.IsVoid;
                def.CreateUserId = row.CreatedBy;

            }
            else if (source.GetType() == typeof(UKClaimDCNoteDef) &&
                target.GetType() == typeof(UKClaimDCNoteDs.UKClaimDCNoteRow))
            {
                UKClaimDCNoteDef def = (UKClaimDCNoteDef)source;
                UKClaimDCNoteDs.UKClaimDCNoteRow row = (UKClaimDCNoteDs.UKClaimDCNoteRow)target;

                row.DCNoteId = def.DCNoteId;
                if (def.DCNoteNo == string.Empty)
                {
                    row.DCNoteNo = "NA/" + row.DCNoteId.ToString().PadLeft(10, '0');
                    def.DCNoteNo = row.DCNoteNo;
                }
                else if (def.DCNoteNo == "N/A")
                    row.SetDCNoteNoNull();
                else
                    row.DCNoteNo = def.DCNoteNo;

                if (def.DCNoteDate == DateTime.MinValue)
                    row.SetDCNoteDateNull();
                else
                    row.DCNoteDate = def.DCNoteDate;
                row.OfficeId = def.OfficeId;
                row.CurrencyId = def.CurrencyId;
                if (def.RevisedCurrencyId == -1)
                    row.SetRevisedCurrencyIdNull();
                else
                    row.RevisedCurrencyId = def.RevisedCurrencyId;
                row.SettledAmt = def.SettledAmount;
                row.TotalAmt = def.TotalAmount;
                row.DebitCreditIndicator = def.DebitCreditIndicator;
                row.VendorId = def.VendorId;
                row.PartyName = def.PartyName;
                if (def.PartyAddress1.Trim() != string.Empty)
                    row.PartyAddress1 = def.PartyAddress1;
                else
                    row.SetPartyAddress1Null();
                if (def.PartyAddress2.Trim() != string.Empty)
                    row.PartyAddress2 = def.PartyAddress2;
                else
                    row.SetPartyAddress2Null();
                if (def.PartyAddress3.Trim() != string.Empty)
                    row.PartyAddress3 = def.PartyAddress3;
                else
                    row.SetPartyAddress3Null();
                if (def.PartyAddress4.Trim() != string.Empty)
                    row.PartyAddress4 = def.PartyAddress4;
                else
                    row.SetPartyAddress4Null();
                if (def.Remark.Trim() != string.Empty)
                    row.Remark = def.Remark;
                else
                    row.SetRemarkNull();
                if (def.SettlementDate == DateTime.MinValue)
                    row.SetSettlementDateNull();
                else
                    row.SettlementDate = def.SettlementDate;
                row.IsCustom = def.IsCustom;
                row.IsInterfaced = def.IsInterfaced;
                row.IsVoid = def.IsVoid;
                row.MailStatus = def.MailStatus;
            }
        }

        private void UKClaimBIADiscrepancyMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKClaimBIADiscrepancyDs.UKClaimBIADiscrepancyRow) &&
                target.GetType() == typeof(UKClaimBIADiscrepancyDef))
            {
                UKClaimBIADiscrepancyDs.UKClaimBIADiscrepancyRow row = (UKClaimBIADiscrepancyDs.UKClaimBIADiscrepancyRow)source;
                UKClaimBIADiscrepancyDef def = (UKClaimBIADiscrepancyDef)target;

                def.ClaimId = row.ClaimId;
                def.ActionType = BIAActionType.getType(row.ActionId);
                def.Amount = row.Amount;
                if (row.IsRemarkNull())
                    def.Remark = string.Empty;
                else
                    def.Remark = row.Remark;
                def.IsLocked = row.IsLocked;
            }
            else if (source.GetType() == typeof(UKClaimBIADiscrepancyDef) &&
                target.GetType() == typeof(UKClaimBIADiscrepancyDs.UKClaimBIADiscrepancyRow))
            {
                UKClaimBIADiscrepancyDef def = (UKClaimBIADiscrepancyDef)source;
                UKClaimBIADiscrepancyDs.UKClaimBIADiscrepancyRow row = (UKClaimBIADiscrepancyDs.UKClaimBIADiscrepancyRow)target;

                row.ClaimId = def.ClaimId;
                row.ActionId = def.ActionType.Id;
                row.Amount = def.Amount;
                if (def.Remark.Trim() == string.Empty)
                    row.SetRemarkNull();
                else
                    row.Remark = def.Remark;
                row.IsLocked = def.IsLocked;
            }
        }

        private void UKClaimDCNoteDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow) &&
                target.GetType() == typeof(UKClaimDCNoteDetailDef))
            {
                UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow row = (UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow)source;
                UKClaimDCNoteDetailDef def = (UKClaimDCNoteDetailDef)target;

                if (!row.IsDCNoteIdNull())
                    def.DCNoteId = row.DCNoteId;
                else
                    def.DCNoteId = -1;
                def.DCNoteDetailId = row.DCNoteDetailId;
                def.ClaimId = row.ClaimId;
                def.ClaimRefundId = row.ClaimRefundId;
                def.CurrencyId = row.CurrencyId;
                def.Amount = row.Amount;
                def.RechargeableAmount = row.RechargeableAmt;
                if (!row.IsLineRemarkNull())
                    def.LineRemark = row.LineRemark;
                else
                    def.LineRemark = string.Empty;
            }
            else if (source.GetType() == typeof(UKClaimDCNoteDetailDef) &&
                target.GetType() == typeof(UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow))
            {
                UKClaimDCNoteDetailDef def = (UKClaimDCNoteDetailDef)source;
                UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow row = (UKClaimDCNoteDetailDs.UKClaimDCNoteDetailRow)target;

                if (def.DCNoteId == -1)
                    row.SetDCNoteIdNull();
                else
                    row.DCNoteId = def.DCNoteId;
                row.DCNoteDetailId = def.DCNoteDetailId;
                row.ClaimId = def.ClaimId;
                row.ClaimRefundId = def.ClaimRefundId;
                row.Amount = def.Amount;
                row.RechargeableAmt = def.RechargeableAmount;
                row.CurrencyId = def.CurrencyId;
                if (def.LineRemark == string.Empty)
                    row.SetLineRemarkNull();
                else
                    row.LineRemark = def.LineRemark;
            }
        }

        private void UKDiscountClaimMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKDiscountClaimDs.UKDiscountClaimRow) &&
                target.GetType() == typeof(UKDiscountClaimDef))
            {
                UKDiscountClaimDs.UKDiscountClaimRow row = (UKDiscountClaimDs.UKDiscountClaimRow)source;
                UKDiscountClaimDef def = (UKDiscountClaimDef)target;

                def.ClaimId = row.ClaimId;
                if (!row.IsItemNoNull())
                    def.ItemNo = row.ItemNo;
                else
                    def.ItemNo = string.Empty;
                if (!row.IsContractNoNull())
                    def.ContractNo = row.ContractNo;
                else
                    def.ContractNo = string.Empty;
                def.OfficeId = row.OfficeId;
                def.PaymentOfficeId = row.PaymentOfficeId;
                def.HandlingOfficeId = row.HandlingOfficeId;
                def.VendorId = row.VendorId;
                def.Vendor = VendorWorker.Instance.getVendorByKey(row.VendorId);
                if (!row.IsProductTeamIdNull())
                    def.ProductTeamId = row.ProductTeamId;
                if (!row.IsTermOfPurchaseIdNull())
                    def.TermOfPurchaseId = row.TermOfPurchaseId;
                def.Qty = row.Qty;
                def.CurrencyId = row.CurrencyId;
                def.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
                def.Amount = row.Amount;
                def.HasUKDN = row.HasUKDN;
                if (!row.IsUKDebitNoteNoNull())
                    def.UKDebitNoteNo = row.UKDebitNoteNo;
                else
                    def.UKDebitNoteNo = string.Empty;
                if (!row.IsUKDebitNoteDateNull())
                    def.UKDebitNoteDate = row.UKDebitNoteDate;
                else
                    def.UKDebitNoteDate = DateTime.MinValue;
                if (!row.IsUKDebitNoteReceivedDateNull())
                    def.UKDebitNoteReceivedDate = row.UKDebitNoteReceivedDate;
                else
                    def.UKDebitNoteReceivedDate = DateTime.MinValue;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                def.IsInterfaced = row.IsInterfaced;

                def.WorkflowStatus = UKDiscountClaimWFS.getType(row.WorkflowStatusId);
                def.WorkflowStatusId = row.WorkflowStatusId;

                if (!row.IsIsUKDiscountNull())
                    def.IsUKDiscount = row.IsUKDiscount;
                else
                    def.IsUKDiscount = false;
                def.IssueDate = row.CreatedOn;
            }
            else if (source.GetType() == typeof(UKDiscountClaimDef) &&
                target.GetType() == typeof(UKDiscountClaimDs.UKDiscountClaimRow))
            {
                UKDiscountClaimDef def = (UKDiscountClaimDef)source;
                UKDiscountClaimDs.UKDiscountClaimRow row = (UKDiscountClaimDs.UKDiscountClaimRow)target;

                row.ClaimId = def.ClaimId;
                if (def.ItemNo.Trim() != string.Empty)
                    row.ItemNo = def.ItemNo;
                else
                    row.SetItemNoNull();
                if (def.ContractNo.Trim() != string.Empty)
                    row.ContractNo = def.ContractNo;
                else
                    row.SetContractNoNull();
                row.OfficeId = def.OfficeId;
                row.PaymentOfficeId = def.PaymentOfficeId;
                row.HandlingOfficeId = def.HandlingOfficeId;
                row.VendorId = def.VendorId;
                if (def.ProductTeamId == -1)
                    row.SetProductTeamIdNull();
                else
                    row.ProductTeamId = def.ProductTeamId;
                if (def.TermOfPurchaseId == -1)
                    row.SetTermOfPurchaseIdNull();
                else
                    row.TermOfPurchaseId = def.TermOfPurchaseId;
                row.Qty = def.Qty;
                row.CurrencyId = def.CurrencyId;
                row.Amount = def.Amount;
                row.HasUKDN = def.HasUKDN;
                if (def.UKDebitNoteNo.Trim() != string.Empty)
                    row.UKDebitNoteNo = def.UKDebitNoteNo;
                else
                    row.SetUKDebitNoteNoNull();
                if (def.UKDebitNoteDate != DateTime.MinValue)
                    row.UKDebitNoteDate = def.UKDebitNoteDate;
                else
                    row.SetUKDebitNoteDateNull();
                if (def.UKDebitNoteReceivedDate != DateTime.MinValue)
                    row.UKDebitNoteReceivedDate = def.UKDebitNoteReceivedDate;
                else
                    row.SetUKDebitNoteReceivedDateNull();
                if (def.Remark.Trim() != string.Empty)
                    row.Remark = def.Remark;
                else
                    row.SetRemarkNull();
                row.IsInterfaced = def.IsInterfaced;
                row.WorkflowStatusId = def.WorkflowStatusId;
            }
        }

        private void UKDiscountClaimRefundMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKDiscountClaimRefundDs.UKDiscountClaimRefundRow) &&
                target.GetType() == typeof(UKDiscountClaimRefundDef))
            {
                UKDiscountClaimRefundDs.UKDiscountClaimRefundRow row = (UKDiscountClaimRefundDs.UKDiscountClaimRefundRow)source;
                UKDiscountClaimRefundDef def = (UKDiscountClaimRefundDef)target;

                def.ClaimRefundId = row.ClaimRefundId;
                def.ClaimId = row.ClaimId;
                if (!row.IsReceivedDateNull())
                    def.ReceivedDate = row.ReceivedDate;
                else
                    def.ReceivedDate = DateTime.MinValue;
                def.Amount = row.Amount;
                if (!row.IsRemarkNull())
                    def.Remark = row.Remark;
                else
                    def.Remark = string.Empty;
                def.IsInterfaced = row.IsInterfaced;
                def.WorkflowStatus = UKDiscountClaimWFS.getType(row.WorkflowStatusId);
                def.WorkflowStatusId = row.WorkflowStatusId;
                def.Status = row.Status;
            }
            else if (source.GetType() == typeof(UKDiscountClaimRefundDef) &&
                target.GetType() == typeof(UKDiscountClaimRefundDs.UKDiscountClaimRefundRow))
            {
                UKDiscountClaimRefundDef def = (UKDiscountClaimRefundDef)source;
                UKDiscountClaimRefundDs.UKDiscountClaimRefundRow row = (UKDiscountClaimRefundDs.UKDiscountClaimRefundRow)target;

                row.ClaimRefundId = def.ClaimRefundId;
                row.ClaimId = def.ClaimId;
                if (def.ReceivedDate != DateTime.MinValue)
                    row.ReceivedDate = def.ReceivedDate;
                else
                    row.SetReceivedDateNull();
                row.Amount = def.Amount;
                if (def.Remark.Trim() == string.Empty)
                    row.SetRemarkNull();
                else
                    row.Remark = def.Remark;
                row.IsInterfaced = def.IsInterfaced;
                row.WorkflowStatusId = def.WorkflowStatus.Id;
                row.Status = def.Status;
            }
        }

        private void UKDiscountClaimLogMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKDiscountClaimLogDs.UKDiscountClaimLogRow) &&
                target.GetType() == typeof(UKDiscountClaimLogDef))
            {
                UKDiscountClaimLogDs.UKDiscountClaimLogRow row = (UKDiscountClaimLogDs.UKDiscountClaimLogRow)source;
                UKDiscountClaimLogDef def = (UKDiscountClaimLogDef)target;

                def.LogId = row.LogId;
                def.ClaimId = row.ClaimId;
                def.LogText = row.LogText;
                if (!row.IsFromStatusIdNull())
                    def.FromStatusId = row.FromStatusId;
                else
                    def.FromStatusId = -1;
                def.ToStatusId = row.ToStatusId;
                def.UserId = row.UserId;
                def.LogDate = row.LogDate;
            }
            else if (source.GetType() == typeof(UKDiscountClaimLogDef) &&
                target.GetType() == typeof(UKDiscountClaimLogDs.UKDiscountClaimLogRow))
            {
                UKDiscountClaimLogDef def = (UKDiscountClaimLogDef)source;
                UKDiscountClaimLogDs.UKDiscountClaimLogRow row = (UKDiscountClaimLogDs.UKDiscountClaimLogRow)target;

                row.LogId = def.LogId;
                row.ClaimId = def.ClaimId;
                row.LogText = def.LogText;
                if (def.FromStatusId == -1)
                    row.SetFromStatusIdNull();
                else
                    row.FromStatusId = def.FromStatusId;
                row.ToStatusId = def.ToStatusId;
                row.UserId = def.UserId;
                row.LogDate = def.LogDate;
            }
        }

        #endregion
	}
}
