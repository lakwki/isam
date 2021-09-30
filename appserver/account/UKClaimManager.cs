using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using com.next.infra.util;
using com.next.infra.persistency.transactions;
using com.next.common.datafactory.worker;
using com.next.common.domain.types;
using com.next.isam.domain.claim;
using com.next.isam.domain.types;
using com.next.isam.dataserver.worker;
using com.next.isam.dataserver.model.claim;
using CrystalDecisions.CrystalReports.Engine;
using com.next.common.domain.industry.vendor;
using com.next.common.datafactory.worker.industry;
using com.next.isam.reporter.accounts;
using com.next.isam.reporter.dataserver;
using com.next.isam.domain.account;
using com.next.isam.appserver.helper;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using com.next.isam.domain.common;

namespace com.next.isam.appserver.claim
{
	public class UKClaimManager
	{
		private static UKClaimManager _instance;
		private UKClaimWorker claimWorker;
		private CommonWorker commonWorker;
		private GeneralWorker generalWorker;
        private AccountWorker accountWorker;

		protected UKClaimManager()
		{
			claimWorker = UKClaimWorker.Instance;
			commonWorker = CommonWorker.Instance;
			generalWorker = GeneralWorker.Instance;
            accountWorker = AccountWorker.Instance;
		}

		public static UKClaimManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new UKClaimManager();
				}
				return _instance;
			}
		}

        public List<UKClaimDef> getUKClaimListByCriteria(int claimTypeId, int officeId, int handlingOfficeId, string ukDebitNoteNo, int vendorId, string itemNo, string contractNo, string debitNoteNo, DateTime fromDate, DateTime toDate, DateTime fromReceivedDate, DateTime toReceivedDate, TypeCollector workflowStatusList, int termOfPurchaseId)
		{
            return claimWorker.getUKClaimListByCriteria(claimTypeId, officeId, handlingOfficeId, ukDebitNoteNo, vendorId, itemNo, contractNo, debitNoteNo, fromDate, toDate, fromReceivedDate, toReceivedDate, workflowStatusList, termOfPurchaseId);
		}

        public List<UKClaimRefundDef> getUKClaimRefundListByCriteria(int claimTypeId, int officeId, int handlingOfficeId, string ukDebitNoteNo, int vendorId, string itemNo, string contractNo, string debitNoteNo, DateTime fromDate, DateTime toDate, DateTime fromReceivedDate, DateTime toReceivedDate, TypeCollector workflowStatusList, int termOfPurchaseId)
        {
            return claimWorker.getUKClaimRefundListByCriteria(claimTypeId, officeId, handlingOfficeId, ukDebitNoteNo, vendorId, itemNo, contractNo, debitNoteNo, fromDate, toDate, fromReceivedDate, toReceivedDate, workflowStatusList, termOfPurchaseId);
        }

        public bool isHomeAndBeautyProductTeam(int productTeamId)
        {
            return claimWorker.isHomeAndBeautyProductTeam(productTeamId);
        }

        public List<UKClaimDef> getUKClaimDiscrepancyList()
        {
            return claimWorker.getUKClaimDiscrepancyList();
        }

        public List<UKClaimDef> getUKClaimEarlyArrivalList(TypeCollector officeIds)
        {
            return claimWorker.getUKClaimEarlyArrivalList(officeIds);
        }

        public List<UKClaimDef> getUKClaimDebugList()
        {
            return claimWorker.getUKClaimDebugList();
        }

        public List<UKClaimDef> getUKClaimListByBIAId(int parentId)
        {
            return claimWorker.getUKClaimListByBIAId(parentId);
        }

        public List<UKClaimDef> getUKClaimApprovalList(TypeCollector officeIds)
        {
            return claimWorker.getUKClaimApprovalList(officeIds);
        }

        public List<UKClaimDef> getUKClaimCOOApprovalList(TypeCollector officeIds, int vendorId)
        {
            return claimWorker.getUKClaimCOOApprovalList(officeIds, vendorId);
        }

        public List<UKClaimDef> getUKClaimReviewList(TypeCollector officeIds, int workflowStatusId)
        {
            return claimWorker.getUKClaimReviewList(officeIds, workflowStatusId);
        }

        public void mailUKClaimDN(ArrayList list, int userId)
        {
            string outputFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder");

            StringBuilder sb = new StringBuilder();
            ArrayList attachmentList = new ArrayList();
            int cnt = 0;
            foreach(string claimId in list)
            {
                UKClaimDef def = claimWorker.getUKClaimByKey(int.Parse(claimId));
                if (def.HasUKDebitNote)
                {
                    sb.AppendLine(" - " + def.UKDebitNoteNo);
                    cnt += 1;

                    ArrayList queryStructs = new ArrayList();
                    queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                    queryStructs.Add(new QueryStructDef("Claim Type", def.Type.DMSDescription));
                    queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
                    queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
                    queryStructs.Add(new QueryStructDef("Debit Note No", def.UKDebitNoteNo));

                    ArrayList qList = DMSUtil.queryDocument(queryStructs);
                    DocumentInfoDef docDef = null;
                    foreach (DocumentInfoDef docInfoDef in qList)
                    {
                        docDef = docInfoDef;
                        break;
                    }

                    if (docDef != null)
                    {
                        foreach (AttachmentInfoDef attachInfoDef in docDef.AttachmentInfos)
                        {
                            string fileName = outputFolder + def.ClaimId.ToString() + "-" + def.UKDebitNoteNo.ToString() + ".pdf";
                            File.WriteAllBytes(fileName, DMSUtil.getAttachment(attachInfoDef.AttachmentID));
                            attachmentList.Add(fileName);
                        }
                    }
                }
            }
            NoticeHelper.sendUKClaimDNArchive(attachmentList, sb.ToString(), cnt, userId);
        }

        public void deleteUKClaim(int claimId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.deleteUKClaim(claimId, userId);

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


        public List<UKClaimDef> getOutstandingUKClaimList(int officeId, int vendorId, int termOfPurchaseId, DateTime cutoffDate, int handlingOfficeId, int ncOptionId)
        {
            return getOutstandingUKClaimList(officeId, vendorId, termOfPurchaseId, cutoffDate, handlingOfficeId, ncOptionId, TypeCollector.Exclusive);
        }

        public List<UKClaimDef> getOutstandingUKClaimList(int officeId, int vendorId, int termOfPurchaseId, DateTime cutoffDate, int handlingOfficeId, int ncOptionId, TypeCollector wfsExcludingList)
        {
            return claimWorker.getOutstandingUKClaimList(officeId, vendorId, termOfPurchaseId, cutoffDate, handlingOfficeId, ncOptionId, wfsExcludingList);
        }

        public List<GenericDataSummaryDef> getOutstandingUKClaimByCurrency(int vendorId, int officeId)
        {
            return claimWorker.getOutstandingUKClaimByCurrency(vendorId, officeId);
        }

        public List<UKClaimDef> getUKClaimListByTypeMapping(int claimId, int claimTypeId, int vendorId, string itemNo, string ukDebitNoteNo, int qty)
        {
            return claimWorker.getUKClaimListByTypeMapping(claimId, claimTypeId, vendorId, itemNo, ukDebitNoteNo, qty);
        }

        public List<UKClaimDef> getUKClaimListByTypeMapping(int claimId, int claimTypeId, int vendorId, string itemNo, string ukDebitNoteNo)
        {
            return claimWorker.getUKClaimListByTypeMapping(claimId, claimTypeId, vendorId, itemNo, ukDebitNoteNo);
        }

        public List<UKClaimPhasingDef> getUKClaimPhasingReport(int fiscalYear, int periodFrom, int periodTo, int officeId, int vendorId)
        {
            return getUKClaimPhasingReport(fiscalYear, periodFrom, periodTo, officeId, vendorId, 0);
        }

        public List<UKClaimPhasingDef> getUKClaimPhasingReport(int fiscalYear, int periodFrom, int periodTo, int officeId, int vendorId, int isGroupByReason)
        {
            return claimWorker.getUKClaimPhasingReport(fiscalYear, periodFrom, periodTo, officeId, vendorId, isGroupByReason);
        }

        public List<UKClaimPhasingByProductTeamDef> getUKClaimPhasingByProductTeamReport(int fiscalYear, int periodFrom, int periodTo, int officeId, int vendorId)
        {
            return claimWorker.getUKClaimPhasingByProductTeamReport(fiscalYear, periodFrom, periodTo, officeId, vendorId);
        }

        public UKClaimDef getUKClaimByKey(int claimId)
        {
            return claimWorker.getUKClaimByKey(claimId);
        }

        public UKClaimDCNoteDetailDef getUKClaimDCNoteDetailByLogicalKey(int claimId, int claimRefundId)
        {
            return claimWorker.getUKClaimDCNoteDetailByLogicalKey(claimId, claimRefundId);
        }

        public UKClaimDCNoteDef getUKClaimDCNoteByDCNoteNo(string dcNoteNo)
        {
            return UKClaimWorker.Instance.getUKClaimDCNoteByDCNoteNo(dcNoteNo);
        }

        /*
        public DomainUKClaimDef getDomainUKClaimByKey(int claimId)
        {
            DomainUKClaimDef domainDef = new DomainUKClaimDef();
            domainDef.Claim = claimWorker.getUKClaimByKey(claimId);
            domainDef.ClaimRefunds = claimWorker.getUKClaimRefundListByClaimId(claimId);
            return domainDef;
        }
        */

        public void deleteUKClaimRefund(int claimRefundId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.deleteUKClaimRefund(claimRefundId, userId);

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

        public void setClaimWorkflowStatus(int claimId, int workflowStatusId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.setClaimWorkflowStatus(claimId, workflowStatusId, userId);

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
        public void updateDomainUKClaimDef(DomainUKClaimDef domainDef, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.updateUKClaim(domainDef.Claim, userId);
                foreach (UKClaimRefundDef def in domainDef.ClaimRefunds)
                {
                    claimWorker.updateUKClaimRefund(def, userId);
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

        public List<string> getUKClaimBIAMappingByKey(int claimId)
        {
            return UKClaimWorker.Instance.getUKClaimBIAMappingByKey(claimId);
        }

        public void updateUKClaimDef(UKClaimDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimDef orgClaim = claimWorker.getUKClaimByKey(def.ClaimId);
                claimWorker.updateUKClaim(def, userId);

                string actionDesc = string.Empty;
                int lastWFSId = (orgClaim == null ? -1 : orgClaim.WorkflowStatus.Id);
                if (lastWFSId == -1)
                    actionDesc = "Create New Next Claim DN";
                else
                    if (lastWFSId == ClaimWFS.NEW.Id && def.WorkflowStatus.Id == ClaimWFS.SUBMITTED.Id)
                        actionDesc = "Update Next Claim DN With Auto Claim Request Mapping";
                    else
                        actionDesc = "Update Next Claim DN";

                if (orgClaim != null && orgClaim.Type == UKClaimType.OTHERS && def.Type != UKClaimType.OTHERS)
                {
                    NoticeHelper.sendGeneralMessage("Next Claim Update Temp Type -> " + def.Type.Name + "(" + def.UKDebitNoteNo + ")", "N/A");
                }

                UKClaimLogDef log = new UKClaimLogDef(def.ClaimId, actionDesc, userId, lastWFSId, def.WorkflowStatus.Id);
                claimWorker.updateUKClaimLog(log, userId);

                if (def.ClaimRequestId != -1 && def.WorkflowStatus.Id == ClaimWFS.SUBMITTED.Id)
                {
                    mapToBIAClaim(def.ClaimId, userId);
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

        public void mapToBIAClaim(int claimId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                UKClaimDef def = claimWorker.getUKClaimByKey(claimId);
                UKClaimDef biaDef = claimWorker.getBIAUKClaimByClaimRequestId(def.ClaimRequestId);
                if (biaDef != null)
                {
                    claimWorker.updateUKClaimBIAMapping(def.ClaimId, biaDef.ClaimId, userId);
                    UKClaimBIADiscrepancyDef discrepancyDef = new UKClaimBIADiscrepancyDef();
                    discrepancyDef.ClaimId = biaDef.ClaimId;
                    discrepancyDef.ActionType = BIAActionType.NS_PROVISION;
                    discrepancyDef.Amount = def.Amount - biaDef.Amount;
                    if (discrepancyDef.Amount > 0)
                        discrepancyDef.ActionType = BIAActionType.SUPPLIER_RECHARGE;
                    discrepancyDef.Remark = string.Empty;
                    discrepancyDef.IsLocked = false;
                    claimWorker.updateUKClaimBIADiscrepancy(discrepancyDef, userId);

                    UKClaimLogDef log = new UKClaimLogDef(def.ClaimId, "Mapped to BIA Claim", userId, def.WorkflowStatus.Id, def.WorkflowStatus.Id);
                    claimWorker.updateUKClaimLog(log, userId);

                    this.generateFullRefundUKClaimDCNote(def.ClaimId, userId);
                    def.WorkflowStatus = ClaimWFS.DEBIT_NOTE_TO_SUPPLIER;
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

        public void updateUKClaimBIADiscrepancyDef(UKClaimBIADiscrepancyDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimDef ukClaimDef = claimWorker.getUKClaimByKey(def.ClaimId);
                List<string> actualClaimIdList = claimWorker.getUKClaimBIAMappingList(def.ClaimId);

                if (actualClaimIdList.Count == 1)
                {
                    UKClaimDCNoteDetailDef dcNoteDetailDef = claimWorker.getUKClaimDCNoteDetailByLogicalKey(int.Parse(actualClaimIdList[0]), 0);
                    UKClaimDCNoteDef dcNoteDef = claimWorker.getUKClaimDCNoteByKey(dcNoteDetailDef.DCNoteId);
                    dcNoteDef.DCNoteDate = DateTime.Today;
                    dcNoteDef.IsInterfaced = false;
                    claimWorker.updateUKClaimDCNote(dcNoteDef, userId);
                }

                UKClaimBIADiscrepancyDef existingDef = claimWorker.getUKClaimBIADiscrepancyByKey(def.ClaimId);
                if (existingDef != null && existingDef.IsLocked == false && def.IsLocked == true && def.Amount != 0)
                {
                    UKClaimDCNoteDef dcNoteDef = new UKClaimDCNoteDef();
                    dcNoteDef.DCNoteId = -1;
                    dcNoteDef.CreateUserId = userId;
                    dcNoteDef.DCNoteDate = DateTime.Today;
                    dcNoteDef.IsCustom = false;
                    dcNoteDef.Remark = string.Empty;
                    if (actualClaimIdList.Count == 1)
                    {
                        dcNoteDef.IsCustom = true;
                        UKClaimDef actualUKClaimDef = claimWorker.getUKClaimByKey(int.Parse(actualClaimIdList[0]));
                        dcNoteDef.Remark = actualUKClaimDef.UKDebitNoteNo;
                    }
                    dcNoteDef.IsInterfaced = false;
                    dcNoteDef.MailStatus = 0;
                    dcNoteDef.OfficeId = ukClaimDef.OfficeId;
                    dcNoteDef.PartyName = ukClaimDef.Vendor.Name;
                    dcNoteDef.PartyAddress1 = ukClaimDef.Vendor.Address0;
                    dcNoteDef.PartyAddress2 = ukClaimDef.Vendor.Address1;
                    dcNoteDef.PartyAddress3 = ukClaimDef.Vendor.Address2;
                    dcNoteDef.PartyAddress4 = ukClaimDef.Vendor.Address3;
                    dcNoteDef.CurrencyId = ukClaimDef.Currency.CurrencyId;
                    dcNoteDef.TotalAmount = def.Amount;
                    dcNoteDef.VendorId = ukClaimDef.Vendor.VendorId;

                    if (def.ActionType == BIAActionType.NS_PROVISION || def.ActionType == BIAActionType.NS_COST)
                    {
                        dcNoteDef.DCNoteNo = string.Empty;
                        dcNoteDef.DebitCreditIndicator = "D";
                        dcNoteDef.SettledAmount = 0;
                    }
                    else
                    {
                        dcNoteDef.DebitCreditIndicator = (def.ActionType == BIAActionType.SUPPLIER_RECHARGE ? "D" : "C");
                        dcNoteDef.DCNoteNo = "N/A";
                        dcNoteDef.SettledAmount = def.Amount;
                    }
                    claimWorker.updateUKClaimDCNote(dcNoteDef, 99999);

                    UKClaimDCNoteDetailDef detailDef = new UKClaimDCNoteDetailDef();
                    detailDef.Amount = dcNoteDef.TotalAmount;
                    detailDef.RechargeableAmount = dcNoteDef.SettledAmount;
                    detailDef.DCNoteId = dcNoteDef.DCNoteId;
                    detailDef.ClaimRefundId = -1;
                    detailDef.ClaimId = def.ClaimId;
                    detailDef.CurrencyId = ukClaimDef.Currency.CurrencyId;
                    detailDef.DCNoteDetailId = -1;
                    detailDef.LineRemark = string.Empty;

                    claimWorker.updateUKClaimDCNoteDetail(detailDef, userId);
                }

                claimWorker.updateUKClaimBIADiscrepancy(def, userId);

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

        /// <summary>
        /// Void DN to suppiler
        /// </summary>
        /// <param name="def"></param>
        /// <param name="voidType">Void by NS Cost / Change to other supplier</param>
        /// <param name="vendorId">Nullable</param>
        /// <param name="userId"></param>
        public void voidUKClaimDN(UKClaimDCNoteDef def, DebitNoteVoidType voidType, int? vendorId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                if (def != null && !def.IsVoid)
                {
                    int originalDCNoteId = def.DCNoteId;
                    string originalDCNoteNo = def.DCNoteNo;
                    def.IsVoid = true;
                    claimWorker.updateUKClaimDCNote(def, userId);

                    UKClaimDCNoteDef dcNoteDef = (UKClaimDCNoteDef)def.Clone();
                    dcNoteDef.DCNoteId = -1;
                    dcNoteDef.DCNoteNo = string.Empty;
                    dcNoteDef.IsVoid = false;
                    dcNoteDef.IsInterfaced = false;
                    string debitCreditIndicator = dcNoteDef.DebitCreditIndicator;
                    dcNoteDef.Remark = String.Format("VOID-AS-{0}|{1}-VOID", voidType.getVoidTypeValueInDC(debitCreditIndicator), originalDCNoteNo);
                    dcNoteDef.IsCustom = false;

                    if (voidType == DebitNoteVoidType.NS_PROVISION)
                        dcNoteDef.SettledAmount = 0;

                    if (voidType == DebitNoteVoidType.CHANGE_TO_OTHER_SUPPLIER && vendorId.HasValue)
                    {
                        dcNoteDef.DCNoteDate = DateTime.Today;
                        dcNoteDef.DCNoteNo = accountWorker.fillNextAdjustmentNoteNo(AdjustmentType.UKCLAIM, dcNoteDef.DCNoteDate, dcNoteDef.OfficeId, dcNoteDef.DebitCreditIndicator);

                        dcNoteDef.VendorId = vendorId.GetValueOrDefault();
                        VendorRef newVendor = VendorWorker.Instance.getVendorByKey(vendorId.GetValueOrDefault());
                        dcNoteDef.PartyName = newVendor.Name;
                        dcNoteDef.PartyAddress1 = newVendor.Address0;
                        dcNoteDef.PartyAddress2 = newVendor.Address1;
                        dcNoteDef.PartyAddress3 = newVendor.Address2;
                        dcNoteDef.PartyAddress4 = newVendor.Address3;
                    }

                    claimWorker.updateUKClaimDCNote(dcNoteDef, userId);
                    List<UKClaimDCNoteDetailDef> detailList = claimWorker.getUKClaimDCNoteDetailListByDCNoteId(originalDCNoteId);
                    foreach (UKClaimDCNoteDetailDef dDef in detailList)
                    {
                        UKClaimDCNoteDetailDef detailDef = (UKClaimDCNoteDetailDef)dDef.Clone();
                        detailDef.DCNoteId = dcNoteDef.DCNoteId;
                        detailDef.DCNoteDetailId = -1;

                        if (voidType == DebitNoteVoidType.NS_PROVISION) 
                            detailDef.RechargeableAmount = 0;

                        detailDef.ClaimRefundId = -1;
                        detailDef.LineRemark = string.Empty;
                        claimWorker.updateUKClaimDCNoteDetail(detailDef, userId);

                        UKClaimDef claimDef = claimWorker.getUKClaimByKey(dDef.ClaimId);
                        if (voidType == DebitNoteVoidType.NS_PROVISION)
                            claimDef.SettlementOption = UKClaimSettlemtType.PROVISION;
                        claimWorker.updateUKClaim(claimDef, userId);
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


        public void updateUKClaimRefundDef(UKClaimRefundDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.updateUKClaimRefund(def, userId);

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

        public void updateUKClaimAndRefundList(UKClaimDef claim, List<UKClaimRefundDef> claimRefundList, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.updateUKClaim(claim, userId);
                
                // delete the refund record if it does not exist in the input Next Claim Refund List

                ArrayList updateRefundIdList = new ArrayList();
                for (int i = 0; i < claimRefundList.Count; i++)
                    if (claimRefundList[i].ClaimRefundId > 0)
                        updateRefundIdList.Add(claimRefundList[i].ClaimRefundId);

                List<UKClaimRefundDef> orgRefundList = claimWorker.getUKClaimRefundListByClaimId(claim.ClaimId);

                for (int i = 0; i < orgRefundList.Count; i++)
                    if (((UKClaimRefundDef)orgRefundList[i]).Status == 1 && !updateRefundIdList.Contains(orgRefundList[i].ClaimRefundId))
                        deleteUKClaimRefund(orgRefundList[i].ClaimRefundId, userId);

                foreach (UKClaimRefundDef def in claimRefundList)
                {
                    def.ClaimId = claim.ClaimId;
                    def.Status = (def.Status < 0 ? 1 : def.Status);
                    if (def.Status == 1)
                        claimWorker.updateUKClaimRefund(def, userId);
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

        public void updateUKClaimDCNote(UKClaimDCNoteDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.updateUKClaimDCNote(def, userId);

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

        public void changeUKClaimDCNoteCurrency(UKClaimDCNoteDef def, int newCurrencyId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                /*
                decimal oldEXRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.CurrencyId, def.DCNoteDate);
                decimal newExRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, newCurrencyId, def.DCNoteDate);
                def.TotalAmount = Math.Round((def.TotalAmount * oldEXRate / newExRate), 2);
                def.SettledAmount = Math.Round((def.SettledAmount * oldEXRate / newExRate), 2);
                */
                def.RevisedCurrencyId = newCurrencyId;
                def.IsInterfaced = false;
                claimWorker.updateUKClaimDCNote(def, userId);

                SunInterfaceQueueDef queueDef = new SunInterfaceQueueDef();
                queueDef.QueueId = -1;
                queueDef.OfficeGroup = commonWorker.getReportOfficeGroupByKey(def.OfficeId);
                queueDef.FiscalYear = 0;
                queueDef.Period = 0;
                queueDef.PurchaseTerm = 0;
                queueDef.UTurn = 0;
                queueDef.CategoryType = CategoryType.ACTUAL;
                queueDef.SourceId = 1;
                queueDef.SubmitTime = DateTime.Now;
                queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.UKClaimRechargeToSupplier_ChangeCurrency.GetHashCode();
                queueDef.User = generalWorker.getUserByKey(userId);
                accountWorker.updateSunInterfaceQueue(queueDef);

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

        public void updateUKClaimLogDef(UKClaimLogDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.updateUKClaimLog(def, userId);

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

        public UKClaimRefundDef getUKClaimRefundByKey(int claimRefundId)
        {
            return claimWorker.getUKClaimRefundByKey(claimRefundId);
        }

        public UKClaimBIADiscrepancyDef getUKClaimBIADiscrepancyByKey(int claimId)
        {
            return claimWorker.getUKClaimBIADiscrepancyByKey(claimId);
        }

        public UKClaimBIADiscrepancyDef getUKClaimBIADiscrepancyByChildId(int childId)
        {
            return claimWorker.getUKClaimBIADiscrepancyByChildId(childId);
        }

        public UKClaimDef getUKClaimByGuid(string guid)
        {
            return claimWorker.getUKClaimByGuid(guid);
        }

        public UKClaimDef getUKClaimByClaimRequestId(int claimRequestId)
        {
            return claimWorker.getUKClaimByClaimRequestId(claimRequestId);
        }

        public UKClaimDef getBIAUKClaimByClaimRequestId(int claimRequestId)
        {
            return claimWorker.getBIAUKClaimByClaimRequestId(claimRequestId);
        }

        public List<UKClaimDef> getNotYetMappedUKClaimList()
        {
            return claimWorker.getNotYetMappedUKClaimList();
        }

        public List<UKClaimDef> getNewlyMappedUKClaimList()
        {
            return claimWorker.getNewlyMappedUKClaimList();
        }

        public void sendUKClaimReviewNotification(UKClaimDef ukClaim)
        {
            if (!string.IsNullOrEmpty(ukClaim.GUId))
            {
                ArrayList claimDetail = claimWorker.buildUKClaimDNArray(ukClaim);
                string UKClaimDN = TableHelper.Instance.generateHtmlTable(claimDetail);
                NoticeHelper.sendUKClaimDNReviewNoticeToQCAdminEmail(UKClaimDN, ukClaim.GUId);
            }
        }

        public List<UKClaimLogDef> getUKClaimLogListByClaimId(int claimId)
        {
            return claimWorker.getUKClaimLogListByClaimId(claimId);
        }

        public List<UKClaimRefundDef> getUKClaimRefundListByClaimId(int claimId)
        {
            return claimWorker.getUKClaimRefundListByClaimId(claimId);
        }

        public List<UKClaimRefundDef> getUKClaimRefundCOOApprovalList(TypeCollector officeIds, int vendorId)
        {
            return claimWorker.getUKClaimRefundCOOApprovalList(officeIds, vendorId);
        }

        public void populateUKClaimRecharge(int officeId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimRechargeDs ds = claimWorker.getOutstandingUKClaimRechargeList(officeId);
                foreach (UKClaimRechargeDs.UKClaimRechargeRow r in ds.UKClaimRecharge.Rows)
                {
                    claimWorker.updateUKClaimRecharge(r.ClaimId, r.ClaimRefundId, r.OfficeId, r.VendorId, r.CurrencyId, r.Amount, r.RechargeableAmt);
                }
                ctx.VoteCommit();
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

        public void populateUKClaimRechargeNR()
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKClaimRechargeDs ds = claimWorker.getOutstandingUKClaimRechargeNRList();
                foreach (UKClaimRechargeDs.UKClaimRechargeRow r in ds.UKClaimRecharge.Rows)
                {
                    claimWorker.updateUKClaimRecharge(r.ClaimId, r.ClaimRefundId, r.OfficeId, r.VendorId, r.CurrencyId, r.Amount, r.RechargeableAmt);
                }
                ctx.VoteCommit();
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

        public ReportClass generateUKClaimDCNote(int officeGroupId, int termOfPurchaseId, bool isDraft, DateTime issueDate, bool sunMacroEnabled, int userId)
        {
            List<UKClaimDef> updatedList = new List<UKClaimDef>();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
            ReportClass report = null;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                if (!isDraft)
                {
                    List<UKClaimDef> toBeCancelledUKClaimList = claimWorker.getToBeCancelledUKClaimList(officeGroupId, -1);
                    foreach (UKClaimDef cancelledUKClaimDef in toBeCancelledUKClaimList)
                    {
                        int currentStatusId = cancelledUKClaimDef.WorkflowStatus.Id;
                        claimWorker.setClaimWorkflowStatus(cancelledUKClaimDef.ClaimId, ClaimWFS.CANCELLED.Id, userId);
                        UKClaimLogDef logDef = new UKClaimLogDef(cancelledUKClaimDef.ClaimId, "Mark Cancelled For Full Refund", userId, currentStatusId, ClaimWFS.CANCELLED.Id);
                        claimWorker.updateUKClaimLog(logDef, userId);
                    }

                    this.generateFullRefundUKClaimDCNote(officeGroupId, termOfPurchaseId, issueDate, userId);
                }

                List<UKClaimDCNoteDef> list = claimWorker.getOutstandingUKClaimDCNoteList(officeGroupId, termOfPurchaseId, userId);
                UKClaimDCNoteReportDs dataSet = new UKClaimDCNoteReportDs();

                UKClaimDef ukClaimDef = null;
                int currentOfficeId = -1;
                int currentCurrencyId = -1;

                foreach (UKClaimDCNoteDef def in list)
                {
                    bool isNew = true;

                    int refundId = 0;
                    int biaClaimId = 0;

                    if (def.DebitCreditIndicator == "D" && def.DCNoteId < -10000)
                    {
                        int.TryParse(def.DCNoteNo, out biaClaimId);
                        def.DCNoteNo = string.Empty;
                    }

                    if (def.DebitCreditIndicator == "C")
                    {
                        int.TryParse(def.DCNoteNo, out refundId);
                        def.DCNoteNo = string.Empty;
                    }

                    if (def.DCNoteId < 0)
                        def.DCNoteId = -1;
                    else
                        isNew = false;

                    if (!isDraft)
                    {
                        def.DCNoteDate = issueDate;
                        if (def.SettledAmount != 0)
                            def.DCNoteNo = accountWorker.fillNextAdjustmentNoteNo(AdjustmentType.UKCLAIM, def.DCNoteDate, def.OfficeId, def.DebitCreditIndicator);
                        else
                            def.DCNoteNo = string.Empty;
                    }
                    claimWorker.updateUKClaimDCNote(def, userId);

                    List<UKClaimDCNoteDetailDef> detailList = null;

                    if (isNew)
                    {
                        if (def.DebitCreditIndicator == "D" && biaClaimId == 0)
                        {
                            List<UKClaimDCNoteDetailDef> tmpList = claimWorker.getOutstandingUKClaimDCNoteDetailList(def.DebitCreditIndicator, def.OfficeId, def.CurrencyId, def.VendorId, userId);
                            detailList = new List<UKClaimDCNoteDetailDef>();
                            foreach (UKClaimDCNoteDetailDef tmpDef in tmpList)
                            {
                                UKClaimDef tmpClaimDef = claimWorker.getUKClaimByKey(tmpDef.ClaimId);
                                if (tmpClaimDef.Type != UKClaimType.BILL_IN_ADVANCE)
                                    detailList.Add(tmpDef);
                            }
                        }
                        else if (def.DebitCreditIndicator == "D" && biaClaimId > 0)
                        {
                            List<UKClaimDCNoteDetailDef> tmpList = claimWorker.getOutstandingUKClaimDCNoteDetailList(def.DebitCreditIndicator, def.OfficeId, def.CurrencyId, def.VendorId, userId);
                            detailList = new List<UKClaimDCNoteDetailDef>();
                            foreach (UKClaimDCNoteDetailDef tmpDef in tmpList)
                            {
                                if (tmpDef.ClaimId == biaClaimId)
                                {
                                    detailList.Add(tmpDef);
                                    break;
                                }
                            }
                        }
                        else
                            detailList = claimWorker.getOutstandingRefundUKClaimDCNoteDetailList(refundId, userId);
                    }
                    else
                        detailList = claimWorker.getUKClaimDCNoteDetailListByDCNoteId(def.DCNoteId);

                    foreach (UKClaimDCNoteDetailDef detailDef in detailList)
                    {
                        detailDef.DCNoteId = def.DCNoteId;
                        claimWorker.updateUKClaimDCNoteDetail(detailDef, userId);
                        if (!isDraft)
                        {
                            if (!(detailDef.ClaimId == 0 && detailDef.ClaimRefundId == 0))
                            {
                                ukClaimDef = claimWorker.getUKClaimByKey(detailDef.ClaimId);
                                if (ukClaimDef.Type.Id == UKClaimType.BILL_IN_ADVANCE.Id && detailDef.ClaimRefundId == 0)
                                    ukClaimDef.UKDebitNoteNo = def.DCNoteNo;

                                if (detailDef.ClaimRefundId > 0)
                                {
                                    UKClaimRefundDef refundDef = claimWorker.getUKClaimRefundByKey(detailDef.ClaimRefundId);
                                    if (refundDef.Remark == string.Empty)
                                        refundDef.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + "Issued " + (detailDef.ClaimRefundId > 0 ? "Credit" : "Debit") + " Note To Supplier";
                                    else
                                        refundDef.Remark += ("\n" + "[" + DateTime.Today.ToString("dd/MM") + "] " + "Issued " + (detailDef.ClaimRefundId > 0 ? "Credit" : "Debit") + " Note To Supplier");
                                    claimWorker.updateUKClaimRefund(refundDef, userId);
                                }
                                else
                                {
                                    string remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + "Issued " + (detailDef.ClaimRefundId > 0 ? "Credit" : "Debit") + " Note To Supplier";
                                    if (def.isVoidAsNSCost)
                                        remark = "[" + DateTime.Today.ToString("dd/MM") + "] Void Debit Note";

                                    if (ukClaimDef.Remark == string.Empty)
                                        ukClaimDef.Remark = remark;
                                    else
                                        ukClaimDef.Remark += ("\n" + remark);
                                }

                                UKClaimWorker.Instance.updateUKClaim(ukClaimDef, userId);

                                if (detailDef.ClaimRefundId == 0)
                                    claimWorker.setClaimWorkflowStatus(detailDef.ClaimId, ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id, userId);

                                if (detailDef.ClaimId > 0 && detailDef.ClaimRefundId == 0)
                                    updatedList.Add(ukClaimDef);  

                                UKClaimLogDef logDef = new UKClaimLogDef(detailDef.ClaimId, (detailDef.ClaimRefundId > 0 ? "Refund " : string.Empty) + "D/C Note Generated (" + def.DCNoteNo + ")", userId, ukClaimDef.WorkflowStatus.Id, ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id);
                                claimWorker.updateUKClaimLog(logDef, userId);
                            }
                        }
                    }

                    if (def.SettledAmount != 0)
                    {
                        UKClaimDCNoteReportDs ds = ReporterWorker.Instance.getUKClaimDCNote(def.DCNoteId);
                        foreach (UKClaimDCNoteReportDs.UKClaimDCNoteReportRow r in ds.UKClaimDCNoteReport)
                        {
                            if (r.SettledAmt != 0)
                            {
                                UKClaimDCNoteReportDs.UKClaimDCNoteReportRow newRow = dataSet.UKClaimDCNoteReport.NewUKClaimDCNoteReportRow();

                                ukClaimDef = claimWorker.getUKClaimByKey(r.ClaimId);
                                newRow.DebitCreditIndicator = r.DebitCreditIndicator;
                                newRow.ClaimId = r.ClaimId;
                                newRow.ClaimRefundId = r.ClaimRefundId;
                                if (ukClaimDef.Type.Id == UKClaimType.BILL_IN_ADVANCE.Id && ukClaimDef.DebitNoteNo != string.Empty)
                                {
                                    if (claimWorker.getClaimRequestDefByKey(int.Parse(ukClaimDef.DebitNoteNo)).ClaimType.ToString() == UKClaimType.MFRN.Name)
                                        newRow.UKDebitNoteNo = "MFRN: " + claimWorker.getClaimRequestDefByKey(int.Parse(ukClaimDef.DebitNoteNo)).FormNo;
                                    else
                                        newRow.UKDebitNoteNo = claimWorker.getClaimRequestDefByKey(int.Parse(ukClaimDef.DebitNoteNo)).FormNo;
                                }
                                else
                                    if (!r.IsUKDebitNoteNoNull()) newRow.UKDebitNoteNo = r.UKDebitNoteNo;
                                newRow.ClaimType = ukClaimDef.Type.Id.ToString();
                                newRow.Currency = r.Currency;
                                newRow.CurrencyId = r.CurrencyId;
                                newRow.OfficeId = r.OfficeId;
                                newRow.DCNoteDate = r.DCNoteDate;
                                newRow.DCNoteNo = r.DCNoteNo;
                                newRow.DCNoteId = r.DCNoteId;
                                if (!r.IsPartyAddress1Null()) newRow.PartyAddress1 = r.PartyAddress1;
                                if (!r.IsPartyAddress2Null()) newRow.PartyAddress2 = r.PartyAddress2;
                                if (!r.IsPartyAddress3Null()) newRow.PartyAddress3 = r.PartyAddress3;
                                if (!r.IsPartyAddress4Null()) newRow.PartyAddress4 = r.PartyAddress4;
                                newRow.PartyName = r.PartyName;
                                newRow.Amount = r.Amount;
                                newRow.RechargeableAmt = r.RechargeableAmt;
                                newRow.SettledAmt = r.SettledAmt;
                                newRow.TotalAmt = r.TotalAmt;
                                newRow.IsCustom = r.IsCustom;
                                newRow.TermOfPurchaseId = r.TermOfPurchaseId;
                                if (!r.IsRemarkNull()) newRow.Remark = r.Remark;
                                if (!r.IsLineRemarkNull()) newRow.LineRemark = r.LineRemark;
                                newRow.HasUKDN = r.HasUKDN;
                                dataSet.UKClaimDCNoteReport.AddUKClaimDCNoteReportRow(newRow);
                            }
                        }
                    }

                    if (def.OfficeId != currentOfficeId || def.CurrencyId != currentCurrencyId)
                    {
                        UKClaimDCNoteReportDs.BenificiaryAccountRow row = dataSet.BenificiaryAccount.NewBenificiaryAccountRow();
                        DebitNoteToNUKParamDef paramDef = ReporterWorker.Instance.getDebitNoteToNUKParamByKey(def.OfficeId, def.CurrencyId);
                        row.BenificiaryAccountNo = ReporterWorker.Instance.getBeneficiaryAccountNoList(def.OfficeId, def.CurrencyId);
                        row.SupplierCode = paramDef.SupplierCode;
                        row.BeneficiaryName = paramDef.BeneficiaryName;
                        row.BankName = paramDef.BankName;
                        row.BankAddress = paramDef.BankAddress;
                        row.SwiftCode = paramDef.SwiftCode;
                        row.CurrencyId = def.CurrencyId;
                        row.OfficeId = def.OfficeId;
                        row.CurrencyCode = GeneralWorker.Instance.getCurrencyByKey(def.CurrencyId).CurrencyCode;
                        row.OfficeName = GeneralWorker.Instance.getOfficeRefByKey(def.OfficeId).Description.Replace("Office", string.Empty);
                        dataSet.BenificiaryAccount.AddBenificiaryAccountRow(row);

                        currentOfficeId = def.OfficeId;
                        currentCurrencyId = def.CurrencyId;
                    }
                }

                if (!isDraft)
                {
                    SunInterfaceQueueDef queueDef = new SunInterfaceQueueDef();
                    queueDef.QueueId = -1;
                    queueDef.OfficeGroup = commonWorker.getReportOfficeGroupByKey(officeGroupId);
                    queueDef.FiscalYear = 0;
                    queueDef.Period = 0;
                    queueDef.PurchaseTerm = 0;
                    queueDef.UTurn = 0;
                    queueDef.CategoryType = CategoryType.ACTUAL;
                    queueDef.SourceId = (sunMacroEnabled ? 2 : 1);
                    queueDef.SubmitTime = DateTime.Now;
                    queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.UKClaimRechargeToSupplier.GetHashCode();
                    queueDef.User = generalWorker.getUserByKey(userId);
                    accountWorker.updateSunInterfaceQueue(queueDef);
                }

                report = new UKClaimDCNoteReport();
                report.SetDataSource(dataSet);
                report.SetParameterValue("IsDraft", (isDraft ? "Y" : "N"));

                if (isDraft)
                    ctx.VoteRollback();
                else
                    ctx.VoteCommit();
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

            if (!isDraft)
            {
                foreach (UKClaimDef claimDef in updatedList)
                {
                    if (UKClaimType.isTechTeamRelated(claimDef.Type.Id))
                        svc.SetClaimRequestStatus(claimDef.ClaimRequestId, 11, userId);
                }
            }
            return report;
        }

        public void generateFullRefundUKClaimDCNote(int officeGroupId, int termOfPurchaseId, DateTime issueDate, int userId)
        {
            List<UKClaimDef> cancelledList = new List<UKClaimDef>();
            List<UKClaimDef> invoicedList = new List<UKClaimDef>();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                List<UKClaimDCNoteDef> list = claimWorker.getOutstandingFullRefundUKClaimDCNoteList(officeGroupId, termOfPurchaseId, userId);

                UKClaimDef ukClaimDef = null;

                foreach (UKClaimDCNoteDef def in list)
                {
                    int claimId = 0;
                    if (def.DCNoteId < 0)
                    {
                        def.DCNoteId = -1;
                        claimId = int.Parse(def.DCNoteNo);
                        def.DCNoteNo = string.Empty;
                    }
                    def.DCNoteDate = issueDate;
                    claimWorker.updateUKClaimDCNote(def, userId);

                    List<UKClaimDCNoteDetailDef> detailList = null;

                    detailList = claimWorker.getOutstandingFullRefundUKClaimDCNoteDetailList(claimId, userId);

                    foreach (UKClaimDCNoteDetailDef detailDef in detailList)
                    {
                        detailDef.DCNoteId = def.DCNoteId;
                        claimWorker.updateUKClaimDCNoteDetail(detailDef, userId);
                        if (!(detailDef.ClaimId == 0 && detailDef.ClaimRefundId == 0))
                        {
                            ukClaimDef = claimWorker.getUKClaimByKey(detailDef.ClaimId);
                            if (ukClaimDef.WorkflowStatus.Id != ClaimWFS.CANCELLED.Id)
                                claimWorker.setClaimWorkflowStatus(detailDef.ClaimId, ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id, userId);
                            if (detailDef.ClaimId > 0 && detailDef.ClaimRefundId == 0)
                            {
                                List<UKClaimDef> fullRefundList = claimWorker.getToBeCancelledUKClaimList(officeGroupId, detailDef.ClaimId);
                                if (fullRefundList.Count > 0 && ukClaimDef.ClaimRequestId > 0)
                                    cancelledList.Add(ukClaimDef);
                                else
                                    invoicedList.Add(ukClaimDef);
                            }

                            if (ukClaimDef.WorkflowStatus.Id != ClaimWFS.CANCELLED.Id)
                            {
                                UKClaimLogDef logDef = new UKClaimLogDef(detailDef.ClaimId, (detailDef.ClaimRefundId > 0 ? "Refund " : string.Empty) + "D/C Note Generated (" + def.DCNoteNo + ")", userId, ukClaimDef.WorkflowStatus.Id, ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id);
                                claimWorker.updateUKClaimLog(logDef, userId);
                            }
                        }
                    }
                }
                ctx.VoteCommit();
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

            foreach (UKClaimDef claimDef in cancelledList)
            {
                if (UKClaimType.isTechTeamRelated(claimDef.Type.Id))
                    svc.SetClaimRequestStatus(claimDef.ClaimRequestId, 99, userId);
            }
            foreach (UKClaimDef claimDef in invoicedList)
            {
                if (UKClaimType.isTechTeamRelated(claimDef.Type.Id))
                    svc.SetClaimRequestStatus(claimDef.ClaimRequestId, 11, userId);
            }
        }

        public void generateFullRefundUKClaimDCNote(int claimId, int userId)
        {
            List<UKClaimDef> cancelledList = new List<UKClaimDef>();
            List<UKClaimDef> invoicedList = new List<UKClaimDef>();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                UKClaimDCNoteDef def = claimWorker.getOutstandingFullRefundUKClaimDCNoteByClaimId(claimId, userId);

                UKClaimDef ukClaimDef = null;

                if (def.DCNoteId < 0)
                {
                    def.DCNoteId = -1;
                    claimId = int.Parse(def.DCNoteNo);
                    def.DCNoteNo = string.Empty;
                }
                def.DCNoteDate = DateTime.Today;
                claimWorker.updateUKClaimDCNote(def, userId);

                List<UKClaimDCNoteDetailDef> detailList = null;

                detailList = claimWorker.getOutstandingFullRefundUKClaimDCNoteDetailListByClaimId(claimId, userId);

                foreach (UKClaimDCNoteDetailDef detailDef in detailList)
                {
                    detailDef.DCNoteId = def.DCNoteId;
                    claimWorker.updateUKClaimDCNoteDetail(detailDef, userId);
                    if (!(detailDef.ClaimId == 0 && detailDef.ClaimRefundId == 0))
                    {
                        ukClaimDef = claimWorker.getUKClaimByKey(detailDef.ClaimId);
                        if (ukClaimDef.WorkflowStatus.Id != ClaimWFS.CANCELLED.Id)
                            claimWorker.setClaimWorkflowStatus(detailDef.ClaimId, ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id, userId);
                        if (detailDef.ClaimId > 0 && detailDef.ClaimRefundId == 0)
                            invoicedList.Add(ukClaimDef);

                        if (ukClaimDef.WorkflowStatus.Id != ClaimWFS.CANCELLED.Id)
                        {
                            UKClaimLogDef logDef = new UKClaimLogDef(detailDef.ClaimId, (detailDef.ClaimRefundId > 0 ? "Refund " : string.Empty) + "D/C Note Generated (" + def.DCNoteNo + ")", userId, ukClaimDef.WorkflowStatus.Id, ClaimWFS.DEBIT_NOTE_TO_SUPPLIER.Id);
                            claimWorker.updateUKClaimLog(logDef, userId);
                        }
                    }
                }

                ctx.VoteCommit();
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

            foreach (UKClaimDef claimDef in invoicedList)
            {
                if (UKClaimType.isTechTeamRelated(claimDef.Type.Id))
                    svc.SetClaimRequestStatus(claimDef.ClaimRequestId, 11, userId);
            }
        }

        public void generateFullRefundUKClaimDCNote(DateTime issueDate, int userId)
        {
            List<UKClaimDef> cancelledList = new List<UKClaimDef>();
            List<UKClaimDef> invoicedList = new List<UKClaimDef>();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                List<UKClaimDCNoteDef> list = claimWorker.getFullRefundUKClaimDCNoteList(userId);

                UKClaimDef ukClaimDef = null;
                DateTime refundMaxReceivedDate = DateTime.MinValue;

                foreach (UKClaimDCNoteDef def in list)
                {
                    int claimId = 0;
                    claimId = int.Parse(def.DCNoteNo);

                    List<UKClaimDCNoteDetailDef> detailList = null;
                    detailList = claimWorker.getOutstandingFullRefundUKClaimDCNoteDetailList(claimId, userId);
                    foreach (UKClaimDCNoteDetailDef detailDef in detailList)
                    {
                        if (detailDef.ClaimRefundId != 0)
                        {
                            UKClaimRefundDef refundDef = claimWorker.getUKClaimRefundByKey(detailDef.ClaimRefundId);
                            if (refundDef.ReceivedDate != DateTime.MinValue && refundMaxReceivedDate < refundDef.ReceivedDate)
                                refundMaxReceivedDate = refundDef.ReceivedDate;
                        }
                    }

                    if (def.DCNoteId < 0)
                    {
                        def.DCNoteId = -1;
                        def.DCNoteNo = string.Empty;
                    }
                    def.DCNoteDate = (refundMaxReceivedDate != DateTime.MinValue ? refundMaxReceivedDate : issueDate);
                    def.IsInterfaced = true;
                    claimWorker.updateUKClaimDCNote(def, userId);

                    foreach (UKClaimDCNoteDetailDef detailDef in detailList)
                    {
                        detailDef.DCNoteId = def.DCNoteId;
                        claimWorker.updateUKClaimDCNoteDetail(detailDef, userId);
                        if (!(detailDef.ClaimId == 0 && detailDef.ClaimRefundId == 0))
                        {
                            ukClaimDef = claimWorker.getUKClaimByKey(detailDef.ClaimId);
                            if (detailDef.ClaimId > 0 && detailDef.ClaimRefundId == 0)
                            {
                                cancelledList.Add(ukClaimDef);
                                claimWorker.setClaimWorkflowStatus(detailDef.ClaimId, ClaimWFS.CANCELLED.Id, userId);
                                UKClaimLogDef logDef = new UKClaimLogDef(detailDef.ClaimId, "Mark Cancelled For Full Refund", userId, ukClaimDef.WorkflowStatus.Id, ClaimWFS.CANCELLED.Id);
                                claimWorker.updateUKClaimLog(logDef, userId);
                            }
                        }
                    }
                }

                ctx.VoteCommit();
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

            foreach (UKClaimDef claimDef in cancelledList)
            {
                if (UKClaimType.isTechTeamRelated(claimDef.Type.Id))
                    svc.SetClaimRequestStatus(claimDef.ClaimRequestId, 99, userId);
            }
        }

        public void sendUKClaimDNToSupplier(int dcNoteId)
        {
            string outputFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder");
            string emailAddr = string.Empty;

            try
            {
                UKClaimDCNoteDef def = UKClaimWorker.Instance.getUKClaimDCNoteByKey(dcNoteId);
                VendorRef vendor = VendorWorker.Instance.getVendorByKey(def.VendorId);
                ArrayList attachmentList = new ArrayList();

                UKClaimDCNoteReport rpt = AccountReportManager.Instance.getUKClaimDCNote(def.DCNoteId);
                string fileName = outputFolder + def.DCNoteNo.Replace('/', '-') + ".pdf";
                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, fileName);

                if (def.DebitCreditIndicator == "D")
                {
                    if (def.SettledAmount == def.TotalAmount && !def.IsCustom)
                        attachmentList = GetAttachmentList(def);

                    this.uploadToDMS(def, fileName);
                    attachmentList.Add(fileName);
                }
                else
                {
                    attachmentList = GetAttachmentList(def);
                    this.uploadToDMS(def, fileName);
                    attachmentList.Add(fileName);
                }

                if (vendor.eAdviceAddr != null)
                    emailAddr = vendor.eAdviceAddr.Trim();

                if (attachmentList.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Next Claim (No Attachment Found)");
                    NoticeHelper.sendUKClaimDCNoteNoAttachmentEmail(def.OfficeId, def.PartyName, def.DebitCreditIndicator, attachmentList, true, def.CreateUserId);
                }
                else if (emailAddr.Trim() == string.Empty)
                {
                    System.Diagnostics.Debug.WriteLine("Next Claim Supplier Email NOT DEFINED");
                    NoticeHelper.sendUKClaimDCNoteMissingSupplierEmail(def.OfficeId, def.PartyName, def.DebitCreditIndicator, attachmentList, true, def.CreateUserId);
                }
                else
                {
                    NoticeHelper.sendUKClaimDCNote(def.OfficeId, vendor, def.DebitCreditIndicator, attachmentList, true, def.CreateUserId);
                    def.MailStatus = 2;
                    UKClaimWorker.Instance.updateUKClaimDCNote(def, 99999);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        private ArrayList getQueryStringList(int claimId, string docType)
        {
            UKClaimDef claimDef = UKClaimWorker.Instance.getUKClaimByKey(claimId);
            ArrayList queryStructs = new ArrayList();
            ArrayList returnList = new ArrayList();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", claimDef.Type.DMSDescription));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Item No", claimDef.ItemNo));
            queryStructs.Add(new QueryStructDef("Debit Note No", claimDef.UKDebitNoteNo));
            /*
            if (claimDef.ClaimRequestId == -1)
            {
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                queryStructs.Add(new QueryStructDef("Claim Type", claimDef.Type.DMSDescription));
                queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
                queryStructs.Add(new QueryStructDef("Item No", claimDef.ItemNo));
                queryStructs.Add(new QueryStructDef("Debit Note No", claimDef.UKDebitNoteNo));
            }
            else
            {
                QAIS.ClaimRequestDef def = svc.GetClaimRequestByKey(claimDef.ClaimRequestId);

                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Authorization Form"));
                queryStructs.Add(new QueryStructDef("Form No", def.FormNo));
                queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
                queryStructs.Add(new QueryStructDef("Claim Type", claimDef.Type.DMSDescription));
                if (def.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                    queryStructs.Add(new QueryStructDef("MFRN Month", def.ClaimMonth));
            }
            */

            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            DocumentInfoDef docDef = null;
            if (qList.Count > 0)
            {
                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    docDef = docInfoDef;
                    break;
                }

                returnList.Add(new QueryStructDef("Supporting Doc Type", docType));
                foreach (FieldInfoDef fiDef in docDef.FieldInfos)
                {
                    if (fiDef.FieldName != "Supporting Doc Type" && fiDef.FieldName != "Debit Note No" && fiDef.FieldName != "Qty")
                        returnList.Add(new QueryStructDef(fiDef.FieldName, fiDef.Content));
                }
                if (claimDef.UKDebitNoteNo != string.Empty)
                    returnList.Add(new QueryStructDef("Debit Note No", claimDef.UKDebitNoteNo));
            }
            return returnList;
        }

        private void uploadToDMS(UKClaimDCNoteDef def, string fileName)
        {
            string docType;
            if (def.DebitCreditIndicator == "D")
                docType = "DN To Supplier";
            else
                docType = "CN To Supplier";

            List<UKClaimDCNoteDetailDef> detailList = UKClaimWorker.Instance.getUKClaimDCNoteDetailListByDCNoteId(def.DCNoteId);
            foreach (UKClaimDCNoteDetailDef detailDef in detailList)
            {
                string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
                if (!System.IO.Directory.Exists(outputFolder))
                    System.IO.Directory.CreateDirectory(outputFolder);

                ArrayList queryStructs = this.getQueryStringList(detailDef.ClaimId, docType);

                ArrayList qList = DMSUtil.queryDocument(queryStructs);
                long docId = 0;
                ArrayList attachmentList = new ArrayList();

                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    docId = docInfoDef.DocumentID;
                    foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                    {
                        byte[] attContent = DMSUtil.getAttachment(attDef.AttachmentID);
                        System.IO.FileStream fs = new System.IO.FileStream(outputFolder + attDef.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                        fs.Write(attContent, 0, attContent.Length);
                        fs.Close();
                        attachmentList.Add(outputFolder + attDef.FileName);
                    }
                }

                if (File.Exists(outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower()))
                    File.Delete(outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower());

                /*
                if (docId > 0)
                {
                    string path = outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower();
                    File.Copy(fileName, path);
                    attachmentList.Add(path);
                    string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                }
                else
                {
                    string path = outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower();
                    File.Copy(fileName, path);
                    attachmentList.Add(path);
                    string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + detailDef.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);
                }
                */

                string path = outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower();
                File.Copy(fileName, path);
                attachmentList.Add(path);
                string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + detailDef.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);

                FileUtility.clearFolder(outputFolder, false);
            }

        }

        public List<UKClaimLogDef> getRefundSupportingUploadLog(int claimId)
        {
            return claimWorker.getRefundSupportingUploadLog(claimId);
        }

        public List<UKDiscountClaimLogDef> getDiscountRefundSupportingUploadLog(int claimId)
        {
            return claimWorker.getDiscountRefundSupportingUploadLog(claimId);
        }

        public bool IsSignedCNCopyUploaded(UKClaimDCNoteDef dcNoteDef)
        {
            string outputFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder");
            ArrayList returnList = new ArrayList();
            List<UKClaimDCNoteDetailDef> detailList = UKClaimWorker.Instance.getUKClaimDCNoteDetailListByDCNoteId(dcNoteDef.DCNoteId);
            foreach (UKClaimDCNoteDetailDef def in detailList)
            {
                if (def.RechargeableAmount != 0)
                {
                    UKClaimDef ukClaimDef = UKClaimWorker.Instance.getUKClaimByKey(def.ClaimId);

                    if (ukClaimDef.HasUKDebitNote || (!ukClaimDef.HasUKDebitNote && dcNoteDef.DebitCreditIndicator == "C"))
                    {
                        ArrayList queryStructs = new ArrayList();

                        queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                        if (!ukClaimDef.HasUKDebitNote && dcNoteDef.DebitCreditIndicator == "C")
                            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "CN To Supplier"));
                        else
                            queryStructs.Add(new QueryStructDef("Supporting Doc Type", (def.ClaimRefundId != 0 ? "CN To Supplier" : "Next Claim DN")));
                        queryStructs.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription));
                        queryStructs.Add(new QueryStructDef("Item No", ukClaimDef.ItemNo));
                        queryStructs.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));

                        ArrayList qList = DMSUtil.queryDocument(queryStructs);
                        bool isByQty = false;
                        if (qList.Count > 1)
                        {
                            isByQty = true;
                            queryStructs.Add(new QueryStructDef("Qty", ukClaimDef.Quantity.ToString()));
                            qList = DMSUtil.queryDocument(queryStructs);
                        }

                        DocumentInfoDef docDef = null;
                        foreach (DocumentInfoDef docInfoDef in qList)
                        {
                            docDef = docInfoDef;
                            return true;
                        }

                    }
                }
            }
            return false;
        }

        private ArrayList GetAttachmentList(UKClaimDCNoteDef dcNoteDef)
        {
            string outputFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder");
            ArrayList returnList = new ArrayList();
            List<UKClaimDCNoteDetailDef> detailList = UKClaimWorker.Instance.getUKClaimDCNoteDetailListByDCNoteId(dcNoteDef.DCNoteId);
            foreach (UKClaimDCNoteDetailDef def in detailList)
            {
                if (def.RechargeableAmount != 0)
                {
                    UKClaimDef ukClaimDef = UKClaimWorker.Instance.getUKClaimByKey(def.ClaimId);

                    if (ukClaimDef.HasUKDebitNote || (!ukClaimDef.HasUKDebitNote && dcNoteDef.DebitCreditIndicator == "C"))
                    {
                        ArrayList queryStructs = new ArrayList();
                        /*
                        QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
                        QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(ukClaimDef.ClaimRequestId);
                        */

                        queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                        if (!ukClaimDef.HasUKDebitNote && dcNoteDef.DebitCreditIndicator == "C")
                            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "CN To Supplier"));
                        else
                            queryStructs.Add(new QueryStructDef("Supporting Doc Type", (def.ClaimRefundId != 0 ? "CN To Supplier" : "Next Claim DN")));
                        queryStructs.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription));
                        queryStructs.Add(new QueryStructDef("Item No", ukClaimDef.ItemNo));
                        queryStructs.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));

                        ArrayList qList = DMSUtil.queryDocument(queryStructs);
                        bool isByQty = false;
                        if (qList.Count > 1)
                        {
                            isByQty = true;
                            queryStructs.Add(new QueryStructDef("Qty", ukClaimDef.Quantity.ToString()));
                            qList = DMSUtil.queryDocument(queryStructs);
                        }

                        DocumentInfoDef docDef = null;
                        foreach (DocumentInfoDef docInfoDef in qList)
                        {
                            docDef = docInfoDef;
                            break;
                        }

                        if (docDef != null)
                        {
                            foreach (AttachmentInfoDef attachInfoDef in docDef.AttachmentInfos)
                            {
                                string fileName = string.Empty;
                                if (def.ClaimRefundId != 0)
                                    fileName = outputFolder + dcNoteDef.DCNoteNo.Replace('/', '-') + ".pdf";
                                else
                                    fileName = outputFolder + dcNoteDef.DCNoteId.ToString() + "-" + ukClaimDef.Type.DMSDescription + "-" + ukClaimDef.UKDebitNoteNo.ToString().Replace('/', '-') + (isByQty ? "-" + ukClaimDef.Quantity.ToString() + "PCS" : string.Empty) + ".pdf";
                                File.WriteAllBytes(fileName, DMSUtil.getAttachment(attachInfoDef.AttachmentID));
                                returnList.Add(fileName);
                            }
                        }
                    }
                }
            }
            return returnList;
        }

        public List<UKClaimReasonDef> getUKClaimReasonList(int ClaimTypeId)
        {
            return QAISWorker.Instance.getUKClaimReasonList(ClaimTypeId);
        }

        public UKClaimDCNoteDef getUKClaimDCNoteByKey(int dcNoteId)
        {
            return UKClaimWorker.Instance.getUKClaimDCNoteByKey(dcNoteId);
        }

        public List<UKDiscountClaimDef> getOutstandingUKDiscountClaimReport(int officeId, int vendorId, int termOfPurchaseId, DateTime cutoffDate, int handlingOfficeId)
        {
            return claimWorker.getOutstandingUKDiscountClaimReport(officeId, vendorId, termOfPurchaseId, cutoffDate, handlingOfficeId);
        }

        public List<UKDiscountClaimDef> getUKDiscountClaimListByCriteria(int officeId, int handlingOfficeId, string ukDebitNoteNo, int vendorId, string itemNo, string contractNo, DateTime fromDate, DateTime toDate, DateTime fromReceivedDate, DateTime toReceivedDate, bool nextDNNo, bool appliedUKDiscount)
        {
            return claimWorker.getUKDiscountClaimListByCriteria(officeId, handlingOfficeId, ukDebitNoteNo, vendorId, itemNo, contractNo, fromDate, toDate, fromReceivedDate, toReceivedDate, nextDNNo, appliedUKDiscount);
        }

        public List<UKDiscountClaimRefundDef> getUKDiscountClaimRefundListByCriteria(int officeId, int handlingOfficeId, string ukDebitNoteNo, int vendorId, string itemNo, string contractNo, DateTime fromDate, DateTime toDate, DateTime fromReceivedDate, DateTime toReceivedDate)
        {
            return claimWorker.getUKDiscountClaimRefundListByCriteria(officeId, handlingOfficeId, ukDebitNoteNo, vendorId, itemNo, contractNo, fromDate, toDate, fromReceivedDate, toReceivedDate);
        }

        public void deleteUKDiscountClaim(int claimId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.deleteUKDiscountClaim(claimId, userId);

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

        public UKDiscountClaimDef getUKDiscountClaimByKey(int claimId)
        {
            return claimWorker.getUKDiscountClaimByKey(claimId);
        }

        public void deleteUKDiscountClaimRefund(int claimRefundId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.deleteUKDiscountClaimRefund(claimRefundId, userId);

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

        public void updateUKDiscountClaimDef(UKDiscountClaimDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                UKDiscountClaimDef orgClaim = claimWorker.getUKDiscountClaimByKey(def.ClaimId);
                claimWorker.updateUKDiscountClaim(def, userId);

                string actionDesc = string.Empty;
                int lastWFSId = (orgClaim == null ? -1 : orgClaim.WorkflowStatus.Id);
                if (lastWFSId == -1)
                    actionDesc = "Create New UK Discount Claim DN";
                else
                    actionDesc = "Update Next UK Discount Claim DN";

                UKDiscountClaimLogDef log = new UKDiscountClaimLogDef();
                log.ClaimId = def.ClaimId;
                log.LogText = actionDesc;
                log.UserId = userId;
                log.FromStatusId = lastWFSId;
                log.ToStatusId = def.WorkflowStatusId;
                log.LogDate = DateTime.Now;

                claimWorker.updateUKDiscountClaimLog(log, userId);

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

        public void updateUKDiscountClaimRefundDef(UKDiscountClaimRefundDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.updateUKDiscountClaimRefund(def, userId);

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

        public void updateUKDiscountClaimLogDef(UKDiscountClaimLogDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                claimWorker.updateUKDiscountClaimLog(def, userId);

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

        public UKDiscountClaimRefundDef getUKDiscountClaimRefundByKey(int claimRefundId)
        {
            return claimWorker.getUKDiscountClaimRefundByKey(claimRefundId);
        }

        public List<UKDiscountClaimLogDef> getUKDiscountClaimLogListByClaimId(int claimId)
        {
            return claimWorker.getUKDiscountClaimLogListByClaimId(claimId);
        }

        public List<UKDiscountClaimRefundDef> getUKDiscountClaimRefundListByClaimId(int claimId)
        {
            return claimWorker.getUKDiscountClaimRefundListByClaimId(claimId);
        }

        public List<UKDiscountClaimDef> getOutstandingUKDiscountClaimList()
        {
            return claimWorker.getOutstandingUKDiscountClaimList();
        }

        public void setDiscountClaimWorkflowStatus(int claimId)
        {
            claimWorker.setUKDiscountClaimWorkflowStatus(claimId);
        }

        public void setDiscountClaimRefundWorkflowStatus(int claimId)
        {
            claimWorker.setUKDiscountClaimRefundWorkflowStatus(claimId);
        }

	}
}
