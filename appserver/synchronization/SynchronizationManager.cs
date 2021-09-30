using System;
using System.Collections;
using com.next.isam.dataserver.worker;
using com.next.isam.dataserver.model.sync;
using com.next.isam.dataserver.model.nss;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.shipping;
using com.next.isam.domain.ils;
using com.next.common.domain.types;
using com.next.infra.persistency.transactions;
using com.next.infra.util;
using com.next.common.appserver;
using com.next.isam.domain.common;
using com.next.common.domain;
using com.next.common.datafactory.worker;
using com.next.isam.domain.account;


namespace com.next.isam.appserver.synchronization
{
    public class SynchronizationManager
    {
        public static SynchronizationManager _instance;
        private SynchronizationWorker synchronizationWorker;
        private OrderSelectWorker orderSelectWorker;
        private OrderWorker orderWorker;
        private AccountWorker accountWorker;
        private ShippingWorker shippingWorker;
        private ILSUploadWorker ilsUploadWorker;
        private AdvancePaymentWorker advancePaymentWorker;
        private const int GBTEST_QTY_THRESHOLD = 500;

        public SynchronizationManager()
        {
            synchronizationWorker = SynchronizationWorker.Instance;
            orderSelectWorker = OrderSelectWorker.Instance;
            accountWorker = AccountWorker.Instance;
            shippingWorker = ShippingWorker.Instance;
            ilsUploadWorker = ILSUploadWorker.Instance;
            orderWorker = OrderWorker.Instance;
            advancePaymentWorker = AdvancePaymentWorker.Instance;
        }

        public static SynchronizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SynchronizationManager();
                }
                return _instance;
            }
        }


        public void beginSynchronization()
        {
            ArrayList shipmentList = synchronizationWorker.getSyncShipmentList();
            int shipmentId = 0;

            foreach (ShipmentSyncRef shipmentRef in shipmentList)
            {
                try
                {
                    shipmentId = shipmentRef.ShipmentId;
                    if (shipmentRef.WorkflowStatus.Id == ContractWFS.INVOICED.Id ||
                        shipmentRef.WorkflowStatus.Id == ContractWFS.APPROVED.Id ||
                        shipmentRef.WorkflowStatus.Id == ContractWFS.PO_PRINTED.Id ||
                        shipmentRef.WorkflowStatus.Id == ContractWFS.CANCELLED.Id ||
                        shipmentRef.WorkflowStatus.Id == ContractWFS.PENDING_FOR_APPROVAL.Id ||
                        shipmentRef.WorkflowStatus.Id == ContractWFS.PENDING_FOR_SUBMIT.Id ||
                        shipmentRef.WorkflowStatus.Id == ContractWFS.AMEND.Id ||
                        shipmentRef.WorkflowStatus.Id == ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id)
                    {
                        this.synchronizeShipmentToISAM(shipmentId);
                        /*
                        synchronizationWorker.complete(shipmentId);
                        MailHelper.sendGeneralMessage("Shipment Id [#" + shipmentId.ToString() + "] Was Synchronized From NSS To ISAM", String.Empty);
                        */
                    }
                }
                catch (Exception e)
                {
                    MailHelper.sendGeneralMessage("Shipment Id [#" + shipmentId.ToString() + "] Was Synchronized From NSS To ISAM UNSUCESSFULLY", String.Empty);
                    MailHelper.sendErrorAlert(e, String.Empty);
                }
            }
        }

        public void syncAdvancePayment()
        {
            ArrayList paymentKeyList = synchronizationWorker.getSyncAdvancePaymentKeyList();
            int paymentIdTemp = 0;

            foreach (int paymentId in paymentKeyList)
            {
                try
                {
                    paymentIdTemp = paymentId;
                    synchronizeAdvancePaymentToISAM(paymentId);
                    synchronizationWorker.AdvancePaymentComplete(paymentId);
                }
                catch (Exception e)
                {
                    MailHelper.sendGeneralMessage("Payment Id [#" + paymentIdTemp.ToString() + "] Was Synchronized From NSS To ISAM UNSUCESSFULLY", String.Empty);
                    MailHelper.sendErrorAlert(e, String.Empty);
                }
            }
        }

        public void syncLetterOfGuarantee()
        {
            ArrayList paymentKeyList = synchronizationWorker.getSyncLGPaymentKeyList();
            int paymentIdTemp = 0;

            foreach (int paymentId in paymentKeyList)
            {
                try
                {
                    paymentIdTemp = paymentId;
                    synchronizeLetterOfGuaranteeToISAM(paymentId);
                    synchronizationWorker.LGPaymentComplete(paymentId);
                }
                catch (Exception e)
                {
                    MailHelper.sendGeneralMessage("LG Payment Id [#" + paymentIdTemp.ToString() + "] Was Synchronized From NSS To ISAM UNSUCESSFULLY", String.Empty);
                    MailHelper.sendErrorAlert(e, String.Empty);
                }
            }
        }

        private void updateChinaGBTestShipments(int contractId, int shipmentId, int productId, int vendorId, string ukSubGroup)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ContractDef contractDef = orderSelectWorker.getContractByKey(contractId);
                ProductDepartmentRef deptRef = GeneralWorker.Instance.getProductDepartmentByKey(contractDef.DeptId);


                NssContractDs otherContractDs = null;
                int totalQty = 0;
                

                ShipmentDef shipmentDef = orderSelectWorker.getShipmentByKey(shipmentId);

                if (shipmentDef.CustomerDestinationId == CustomerDestinationDef.Id.china_shanghai.GetHashCode()
                    || shipmentDef.CustomerDestinationId == CustomerDestinationDef.Id.shanghai_fabric.GetHashCode()
                    || shipmentDef.CustomerDestinationId == CustomerDestinationDef.Id.shanghai_fuk.GetHashCode()
                    || shipmentDef.CustomerDestinationId == CustomerDestinationDef.Id.shanghai.GetHashCode())
                {
                    orderWorker.updateChinaGBTestIndicator(shipmentId, true);
                }
                else if (deptRef.Code != "CW")
                {
                    orderWorker.updateChinaGBTestIndicator(shipmentId, false);
                    /*
                    if (contractDef.ProductId == 184592) // itemno = "421681" , subgroup = 'UF04"
                        orderWorker.updateChinaGBTestIndicator(shipmentId, true);
                    else
                        orderWorker.updateChinaGBTestIndicator(shipmentId, false);
                    */
                }
                /*
                else if (contractDef.ProductId == 184592) // itemno = "421681" , subgroup = 'UF04"
                {
                    orderWorker.updateChinaGBTestIndicator(shipmentId, true);
                }
                */
                else
                {
                    ArrayList shipmentList = orderSelectWorker.GetChinaGBTestShipmentList(productId, contractDef.Season.SeasonId);

                    if (GeneralManager.Instance.isChinaGBTestRequired(contractDef.Customer.CustomerId, contractDef.Season.SeasonId, contractDef.ProductTeam.ProductCodeId, vendorId, ukSubGroup, productId))
                    {
                        foreach (ShipmentDef def in shipmentList)
                        {
                            otherContractDs = synchronizationWorker.getSyncContract(def.ShipmentId);
                            if (GeneralManager.Instance.isChinaGBTestRequired(contractDef.Customer.CustomerId, contractDef.Season.SeasonId, otherContractDs.Contract[0].ProductTeamId, def.Vendor.VendorId, otherContractDs.Contract[0].IsUKSubGroupNull() ? string.Empty : otherContractDs.Contract[0].UKSubGroup, otherContractDs.Contract[0].ProductId))
                                totalQty += def.TotalOrderQuantity;
                        }

                        if (totalQty >= GBTEST_QTY_THRESHOLD)
                        {
                            foreach (ShipmentDef def in shipmentList)
                            {
                                if (def.CustomerDestinationId == CustomerDestinationDef.Id.china_shanghai.GetHashCode()
                                    || def.CustomerDestinationId == CustomerDestinationDef.Id.shanghai_fabric.GetHashCode())
                                    orderWorker.updateChinaGBTestIndicator(def.ShipmentId, true);
                                else if (!def.IsChinaGBTestRequired)
                                {
                                    otherContractDs = synchronizationWorker.getSyncContract(def.ShipmentId);

                                    if (GeneralManager.Instance.isChinaGBTestRequired(contractDef.Customer.CustomerId, contractDef.Season.SeasonId, otherContractDs.Contract[0].ProductTeamId, def.Vendor.VendorId, otherContractDs.Contract[0].IsUKSubGroupNull() ? string.Empty : otherContractDs.Contract[0].UKSubGroup, otherContractDs.Contract[0].ProductId))
                                        orderWorker.updateChinaGBTestIndicator(def.ShipmentId, true);
                                }
                            }
                        }
                        else
                        {
                            foreach (ShipmentDef def in shipmentList)
                            {
                                if (def.CustomerDestinationId == CustomerDestinationDef.Id.china_shanghai.GetHashCode()
                                    || def.CustomerDestinationId == CustomerDestinationDef.Id.shanghai_fabric.GetHashCode())
                                    orderWorker.updateChinaGBTestIndicator(def.ShipmentId, true);

                                else if (def.IsChinaGBTestRequired)
                                {
                                    otherContractDs = synchronizationWorker.getSyncContract(def.ShipmentId);
                                    if (GeneralManager.Instance.isChinaGBTestRequired(contractDef.Customer.CustomerId, contractDef.Season.SeasonId, otherContractDs.Contract[0].ProductTeamId, def.Vendor.VendorId, otherContractDs.Contract[0].IsUKSubGroupNull() ? string.Empty : otherContractDs.Contract[0].UKSubGroup, otherContractDs.Contract[0].ProductId))
                                        orderWorker.updateChinaGBTestIndicator(def.ShipmentId, false);
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (ShipmentDef def in shipmentList)
                        {
                            if (def.CustomerDestinationId == CustomerDestinationDef.Id.china_shanghai.GetHashCode()
                                || def.CustomerDestinationId == CustomerDestinationDef.Id.shanghai_fabric.GetHashCode())
                                orderWorker.updateChinaGBTestIndicator(def.ShipmentId, true);

                            else if (def.IsChinaGBTestRequired)
                            {
                                otherContractDs = synchronizationWorker.getSyncContract(def.ShipmentId);
                                if (!GeneralManager.Instance.isChinaGBTestRequired(otherContractDs.Contract[0].CustomerId, contractDef.Season.SeasonId, otherContractDs.Contract[0].ProductTeamId, def.Vendor.VendorId, otherContractDs.Contract[0].IsUKSubGroupNull() ? string.Empty : otherContractDs.Contract[0].UKSubGroup, otherContractDs.Contract[0].ProductId))
                                    orderWorker.updateChinaGBTestIndicator(def.ShipmentId, false);
                            }
                        }
                    }


                    foreach (ShipmentDef def in orderSelectWorker.GetChinaGBTestNonDirectoryShipmentList(shipmentId))
                    {
                        if (def.CustomerDestinationId != CustomerDestinationDef.Id.china_shanghai.GetHashCode()
                            && def.CustomerDestinationId != CustomerDestinationDef.Id.shanghai_fabric.GetHashCode())
                            orderWorker.updateChinaGBTestIndicator(def.ShipmentId, false);
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
            
        }

        public void synchronizeShipmentToISAM(int shipmentId)
        {
            DomainShipmentSyncDef domainDef = this.getDomainShipmentSyncDef(shipmentId);
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                bool isInitial = true;

                InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(shipmentId);
                if (invoiceDef == null)
                    shippingWorker.updateActionHistory(new ActionHistoryDef(shipmentId, 0, ActionHistoryType.MERCHANDISER_UPDATES, "Initial data transfer", GeneralStatus.ACTIVE.Code, 99999));
                else
                    isInitial = false;

                ArrayList amendmentList = new ArrayList();

                NssContractDs.ContractRow contractRow = domainDef.ContractDataSet.Contract[0];
                NssShipmentDs.ShipmentRow shipmentRow = domainDef.ShipmentDataSet.Shipment[0];
                NssProductDs.ProductRow productRow = domainDef.ProductDataSet.Product[0];

                if (synchronizationWorker.isBlankTeeOrder(contractRow, shipmentRow, productRow))
                    NoticeHelper.sendGeneralMessage("ISAM Data Sync : This is a blank tee order " + contractRow.ContractNo + "-" + shipmentRow.DeliveryNo.ToString(), string.Empty);

                bool isNSLSZOrder = false;
                int isNextMfgOrder = 0;
                int isPOIssueToNextMfg = 0;
                int otherCurrencyId = 0;
                bool isItemNoAmended = false;
                int originalProductId = 0;

                ContractDef contract = orderSelectWorker.getContractByKey(contractRow.ContractId);
                if (contract != null) originalProductId = contract.ProductId;

                if (!contractRow.IsIsNextMfgOrderNull()) isNextMfgOrder = contractRow.IsNextMfgOrder;
                if (!contractRow.IsIsPOIssueToNextMfgNull()) isPOIssueToNextMfg = contractRow.IsPOIssueToNextMfg;
                if (!shipmentRow.IsOtherCurrencyIdNull()) otherCurrencyId = shipmentRow.OtherCurrencyId;

                if (isNextMfgOrder == 1 && isPOIssueToNextMfg == 1)
                    isNSLSZOrder = true;
                
                synchronizationWorker.syncProduct(domainDef.ProductDataSet, shipmentId, isInitial, amendmentList);
                synchronizationWorker.syncSizeOption(domainDef.SizeOptionDataSet, shipmentId, isInitial, amendmentList);
                synchronizationWorker.syncContract(domainDef.ContractDataSet, shipmentId, isInitial, amendmentList);
                synchronizationWorker.syncShipment(domainDef.ShipmentDataSet, domainDef.ShipmentDetailDataSet, shipmentId, isInitial, amendmentList, isNSLSZOrder, otherCurrencyId);
                synchronizationWorker.syncShipmentDetail(domainDef.ShipmentDetailDataSet, shipmentId, isInitial, amendmentList, isNSLSZOrder, otherCurrencyId);
                synchronizationWorker.syncSplitShipment(domainDef.SplitShipmentDataSet, domainDef.SplitShipmentDetailDataSet, shipmentId, isInitial, amendmentList, isNSLSZOrder, otherCurrencyId);
                synchronizationWorker.syncSplitShipmentDetail(domainDef.SplitShipmentDetailDataSet, shipmentId, isInitial, amendmentList, isNSLSZOrder, otherCurrencyId);
                synchronizationWorker.syncOtherCost(domainDef.OtherCostDataSet, false, shipmentId, isInitial, amendmentList);
                synchronizationWorker.syncOtherCost(domainDef.SplitOtherCostDataSet, true, shipmentId, isInitial, amendmentList);
                shippingWorker.updateShipmentSummaryTotal(shipmentId, 99999);
                synchronizationWorker.syncInvoice(domainDef, shipmentId, isInitial, amendmentList);
                
                foreach (ActionHistoryDef historyDef in amendmentList)
                {
                    /*
                    if (historyDef.AmendmentType == AmendmentType.ITEM)
                        isItemNoAmended = true;
                    */
                    ShippingWorker.Instance.updateActionHistory(historyDef);
                }

                ILSInvoiceDef ilsInvoiceDef = ilsUploadWorker.getILSInvoiceByShipmentId(shipmentId);
                if (ilsInvoiceDef != null && ilsInvoiceDef.IsCancelled == false)
                {
                    if (ilsInvoiceDef.Status == ILSInvoiceUploadStatus.OPTION_MISMATCH.Id
                        || ilsInvoiceDef.Status == ILSInvoiceUploadStatus.QUANTITY_MISMATCH.Id
                        || ilsInvoiceDef.Status == ILSInvoiceUploadStatus.CANCELLED.Id
                        || ilsInvoiceDef.Status == ILSInvoiceUploadStatus.MISSING_FACTORYID_FOR_NML.Id
                        || ilsInvoiceDef.Status == ILSInvoiceUploadStatus.NOT_SELFBILLED_ORDER.Id
                        || ilsInvoiceDef.Status == ILSInvoiceUploadStatus.CURRENCY_MISMATCH.Id)
                    {
                        ILSPackingListDef ilsPackingListDef = ilsUploadWorker.getILSPackingListByKey(ilsInvoiceDef.OrderRefId);
                        ilsPackingListDef.IsUploaded = false;
                        ilsInvoiceDef.Status = ILSInvoiceUploadStatus.PENDING.Id;
                        ilsUploadWorker.updateILSPackingList(ilsPackingListDef, -1);
                        ilsUploadWorker.updateILSInvoice(ilsInvoiceDef);
                    }
                }

                if (amendmentList.Count > 0)
                {
                    accountWorker.doReversal(shipmentId, amendmentList);
                }

                /*
                this.updateChinaGBTestShipments(contractRow.ContractId, shipmentId, contractRow.ProductId, domainDef.ShipmentDataSet.Shipment[0].VendorId, contractRow.IsUKSubGroupNull() ? string.Empty : contractRow.UKSubGroup);

                if (isItemNoAmended)
                    this.updateChinaGBTestShipments(contractRow.ContractId, shipmentId, originalProductId, domainDef.ShipmentDataSet.Shipment[0].VendorId, contractRow.IsUKSubGroupNull() ? string.Empty : contractRow.UKSubGroup);
                */

                ctx.VoteCommit();

                if (shipmentRow.WorkflowStatusId == ContractWFS.INVOICED.Id)
                    synchronizationWorker.SyncNssShipment(shipmentId);

                synchronizationWorker.complete(shipmentId);
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

        public void synchronizeAdvancePaymentToISAM(int paymentId)
        {
            DomainAdvancePaymentSyncDef domainDef = this.getDomainAdvancePaymentSyncDef(paymentId);
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList amendmentList = new ArrayList();

                NssAdvancePaymentDs.AdvancePaymentRow payment = domainDef.Payment.AdvancePayment[0];
                bool isRejected = (payment.WorkflowStatusId == NSSAdvancePaymentWFS.REJECTED.Id);
                synchronizationWorker.syncAdvancePayment(domainDef.Payment, amendmentList, isRejected);
                synchronizationWorker.syncAdvancePaymentOrderDetail(domainDef.PaymentOrderDetail, amendmentList, paymentId, isRejected);

                AdvancePaymentDef paymentDef = AdvancePaymentWorker.Instance.getAdvancePaymentByKey(paymentId);

                if (!isRejected && domainDef.PaymentOrderDetail != null)
                {
                    ArrayList existShipmentDeduction = new ArrayList();
                    foreach (NssAdvancePaymentOrderDetailDs.AdvancePaymentOrderDetailRow sourceRow in domainDef.PaymentOrderDetail.AdvancePaymentOrderDetail.Rows)
                    {
                        int userId = sourceRow.CreatedBy;
                        if (!sourceRow.IsModifiedByNull()) 
                        {
                            userId = sourceRow.ModifiedBy;
                        }
                        existShipmentDeduction = shippingWorker.getShipmentDeductionByLogicalKey(sourceRow.ShipmentId, PaymentDeductionType.FABRIC_ADVANCE.Id, paymentDef.PaymentNo);
                        ShipmentDeductionDef deduction = new ShipmentDeductionDef();
                        if (existShipmentDeduction.Count == 0)
                        {
                            deduction.ShipmentDeductionId = -1;
                            deduction.ShipmentId = sourceRow.ShipmentId;
                            deduction.DeductionType = PaymentDeductionType.FABRIC_ADVANCE;
                            deduction.DocumentNo = paymentDef.PaymentNo;
                            deduction.Amount = sourceRow.ExpectedDeductAmt;
                            if (isRejected)
                                deduction.Amount = 0;
                            deduction.Status = 1;
                            amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "NEW SHIPMENT DEDUCTION: [SHIPMENT ID:" + sourceRow.ShipmentId + "] [AMT:" + deduction.Amount + "]" + (isRejected ? " [REJECTED]" : "")));
                            shippingWorker.updateShipmentDeduction(deduction, userId);
                        }
                        /*
                        else
                        {
                            foreach (ShipmentDeductionDef item in existShipmentDeduction)
                            {
                                decimal originalAmt = item.Amount;
                                deduction = item;
                                deduction.Amount = (isRejected ? 0 : sourceRow.ExpectedDeductAmt);
                                if (originalAmt != deduction.Amount)
                                    amendmentList.Add(advancePaymentWorker.getNewAdvancePaymentActionHistory(sourceRow.PaymentId, "EXISTING SHIPMENT DEDUCTION: [SHIPMENT ID:" + sourceRow.ShipmentId + "] [AMT:" + originalAmt + "-->" + deduction.Amount + "]" + (isRejected ? " [REJECTED]" : "")));
                            }
                        }
                        shippingWorker.updateShipmentDeduction(deduction, userId);
                        */
                    }
                }

                foreach (AdvancePaymentActionHistoryDef historyDef in amendmentList)
                {
                    advancePaymentWorker.updateAdvancePaymentActionHistory(historyDef);
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

        public void synchronizeLetterOfGuaranteeToISAM(int paymentId)
        {
            DomainLetterOfGuaranteeSyncDef domainDef = this.getDomainLetterOfGuaranteeSyncDef(paymentId);
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList amendmentList = new ArrayList();

                //NssLGPaymentDs.LGPaymentRow payment = domainDef.Payment.LGPayment[0];

                synchronizationWorker.syncLGPayment(domainDef.Payment);
                synchronizationWorker.syncLGPaymentOrderDetail(domainDef.PaymentOrderDetail);

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

        private DomainShipmentSyncDef getDomainShipmentSyncDef(int shipmentId)
        {
            DomainShipmentSyncDef domainDef = new DomainShipmentSyncDef();
            domainDef.ContractDataSet = synchronizationWorker.getSyncContract(shipmentId);
            domainDef.ShipmentDataSet = synchronizationWorker.getSyncShipment(shipmentId);
            domainDef.ShipmentDetailDataSet = synchronizationWorker.getSyncShipmentDetail(shipmentId);
            domainDef.OtherCostDataSet = synchronizationWorker.getSyncOtherCost(shipmentId);
            domainDef.SplitShipmentDataSet = synchronizationWorker.getSyncSplitShipment(shipmentId);
            domainDef.SplitShipmentDetailDataSet = synchronizationWorker.getSyncSplitShipmentDetail(shipmentId);
            domainDef.SplitOtherCostDataSet = synchronizationWorker.getSyncSplitOtherCost(shipmentId);
            domainDef.ProductDataSet = synchronizationWorker.getSyncProduct(shipmentId);
            domainDef.SizeOptionDataSet = synchronizationWorker.getSyncSizeOption(shipmentId);

            NssShipmentDs.ShipmentRow shipmentRow = domainDef.ShipmentDataSet.Shipment[0];
            int isUKDiscount = 0;
            if (!shipmentRow.IsIsUKDiscountNull()) isUKDiscount = shipmentRow.IsUKDiscount;

            if (isUKDiscount == 0)
            {
                if (domainDef.ShipmentDetailDataSet != null)
                {
                    foreach (NssShipmentDetailDs.ShipmentDetailRow row in domainDef.ShipmentDetailDataSet.ShipmentDetail)
                    {
                        row.ReducedSellingPrice = 0;
                        row.ReducedNetFOBPrice = 0;
                        row.ReducedSupplierGmtPrice = 0;
                    }
                }

                if (domainDef.SplitShipmentDetailDataSet != null)
                {
                    foreach (NssSplitShipmentDetailDs.SplitShipmentDetailRow row in domainDef.SplitShipmentDetailDataSet.SplitShipmentDetail)
                    {
                        row.ReducedSellingPrice = 0;
                        row.ReducedNetFOBPrice = 0;
                        row.ReducedSupplierGmtPrice = 0;
                    }
                }
            }
            return domainDef;
        }

        private DomainAdvancePaymentSyncDef getDomainAdvancePaymentSyncDef(int paymentId)
        {
            DomainAdvancePaymentSyncDef domainDef = new DomainAdvancePaymentSyncDef();
            domainDef.Payment = synchronizationWorker.getSyncAdvancePayment(paymentId);
            domainDef.PaymentOrderDetail = synchronizationWorker.getSyncAdvancePaymentOrderDetail(paymentId);

            return domainDef;
        }

        private DomainLetterOfGuaranteeSyncDef getDomainLetterOfGuaranteeSyncDef(int paymentId)
        {
            DomainLetterOfGuaranteeSyncDef domainDef = new DomainLetterOfGuaranteeSyncDef();
            domainDef.Payment = synchronizationWorker.getSyncLGPayment(paymentId);
            domainDef.PaymentOrderDetail = synchronizationWorker.getSyncLGPaymentOrderDetail(paymentId);

            return domainDef;
        }

    }
}
