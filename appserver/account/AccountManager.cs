using System;
using System.Collections;
using System.Collections.Generic;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.infra.persistency.transactions;
using com.next.infra.util;
using com.next.isam.domain.common;
using com.next.isam.domain.order;
using com.next.isam.domain.shipping;
using com.next.isam.domain.claim;
using com.next.common.domain.types;
using com.next.common.domain;
using com.next.common.appserver;
using com.next.isam.appserver.claim;
using com.next.common.domain.industry.vendor;
using com.next.common.datafactory.worker.industry;
using com.next.common.datafactory.worker;
using com.next.isam.reporter.dataserver;
using com.next.isam.reporter.accounts;
using CrystalDecisions.CrystalReports.Engine;
using com.next.isam.domain.product;

namespace com.next.isam.appserver.account
{
    public class AccountManager
    {
        private static AccountManager _instance;
        private AccountWorker accountWorker;
        private AdvancePaymentWorker advancePaymentWorker;
        private CommonWorker commonWorker;
        private ShippingWorker shippingWorker;
        private OrderSelectWorker orderSelectWorker;
        private OrderWorker orderWorker;
        private GeneralWorker generalWorker;
        private int NSSAdminUserId = 99999;

        public AccountManager()
        {
            accountWorker = AccountWorker.Instance;
            advancePaymentWorker = AdvancePaymentWorker.Instance;
            commonWorker = CommonWorker.Instance;
            shippingWorker = ShippingWorker.Instance;
            orderSelectWorker = OrderSelectWorker.Instance;
            orderWorker = OrderWorker.Instance;
            generalWorker = GeneralWorker.Instance;
        }

        public static AccountManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AccountManager();
                }
                return _instance;
            }
        }

        public void processQueues()
        {
            foreach (SunInterfaceQueueDef queueDef in accountWorker.getSunInterfaceQueueList())
            {
                if (commonWorker.getSystemParameterByKey(220).ParameterValue == "Y")
                    break;
                generateInterface(queueDef);
            }
        }

        public void submitSunInterfaceBatch(int officeId, int fiscalYear, int period, ArrayList typeList, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                SunInterfaceQueueDef def = null;
                int[] sunInterfaceTypeIds = new int[SunInterfaceTypeRef.getSunMacroTypeIdList().Count];
                int idx = 0; 
                foreach (int tId in SunInterfaceTypeRef.getSunMacroTypeIdList())
                {
                    sunInterfaceTypeIds[idx] = tId;
                    idx++;
                }

                foreach (int s in typeList)
                {
                    for (int i = 0; i <= sunInterfaceTypeIds.GetUpperBound(0); i++)
                    {
                        if (s == sunInterfaceTypeIds[i])
                        {
                            if (!(s == SunInterfaceTypeRef.Id.QCC.GetHashCode() && officeId == OfficeId.SL.Id)
                                && !(s == SunInterfaceTypeRef.Id.SlippageReversal.GetHashCode() && officeId != OfficeId.ND.Id))
                            {

                                def = new SunInterfaceQueueDef();
                                def.QueueId = -1;
                                def.OfficeGroup = commonWorker.getReportOfficeGroupByKey(officeId);
                                def.SunInterfaceTypeId = sunInterfaceTypeIds[i];
                                def.CategoryType = CategoryType.ACTUAL;
                                def.User = generalWorker.getUserByKey(userId);
                                if ((officeId == OfficeId.HK.Id || officeId == OfficeId.SH.Id || officeId == OfficeId.DG.Id || officeId == 101) && (sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.Sales.GetHashCode() || sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.SalesCommission.GetHashCode()))
                                    def.UTurn = 2;
                                else
                                    def.UTurn = 0;

                                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.Sales.GetHashCode() && def.CategoryType == CategoryType.ACCRUAL && period == 12)
                                    def.SourceId = 1;
                                else if (s == SunInterfaceTypeRef.Id.ProvisionForFabricLiabilities.GetHashCode())
                                    def.SourceId = 2;
                                else if (s == SunInterfaceTypeRef.Id.QCC.GetHashCode())
                                    def.SourceId = 2;
                                else
                                    def.SourceId = 2;
                                def.SubmitTime = DateTime.Now;
                                def.FiscalYear = fiscalYear;
                                def.Period = period;
                                def.PurchaseTerm = 0;
                                accountWorker.updateSunInterfaceQueue(def);

                            }

                            if ((officeId == OfficeId.HK.Id || officeId == OfficeId.SH.Id || officeId == OfficeId.DG.Id || officeId == 101) && (sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.Sales.GetHashCode() || sunInterfaceTypeIds[i] == SunInterfaceTypeRef.Id.SalesCommission.GetHashCode()))
                            {
                                def = (SunInterfaceQueueDef)def.Clone();
                                def.QueueId = -1;
                                def.UTurn = 1;
                                accountWorker.updateSunInterfaceQueue(def);
                                def.UTurn = 0;
                            }

                            if (s != SunInterfaceTypeRef.Id.GTCommission.GetHashCode() 
                                && s != SunInterfaceTypeRef.Id.ProvisionForFabricLiabilities.GetHashCode() 
                                && s != SunInterfaceTypeRef.Id.TemporaryPayment.GetHashCode()
                                && !(s == SunInterfaceTypeRef.Id.SlippageReversal.GetHashCode() && officeId != OfficeId.ND.Id))
                            {
                                def = (SunInterfaceQueueDef)def.Clone();
                                def.QueueId = -1;
                                def.CategoryType = CategoryType.ACCRUAL;
                                
                                if (s == SunInterfaceTypeRef.Id.QCC.GetHashCode())
                                    def.SourceId = 2;
                                else
                                    def.SourceId = 2;
                                accountWorker.updateSunInterfaceQueue(def);
                            }

                            if (s != SunInterfaceTypeRef.Id.GTCommission.GetHashCode() && s != SunInterfaceTypeRef.Id.TemporaryPayment.GetHashCode()
                                && s != SunInterfaceTypeRef.Id.SlippageReversal.GetHashCode())
                            {
                                def = (SunInterfaceQueueDef)def.Clone();
                                def.QueueId = -1;
                                def.CategoryType = CategoryType.REALIZED;
                                if (s == SunInterfaceTypeRef.Id.ProvisionForFabricLiabilities.GetHashCode())
                                    def.SourceId = 2;
                                else if (s == SunInterfaceTypeRef.Id.QCC.GetHashCode())
                                    def.SourceId = 2;
                                else
                                    def.SourceId = 2;
                                accountWorker.updateSunInterfaceQueue(def);
                            }

                            if (s != SunInterfaceTypeRef.Id.TemporaryPayment.GetHashCode() && s != SunInterfaceTypeRef.Id.SellingPriceDiscount.GetHashCode()
                                && s != SunInterfaceTypeRef.Id.SlippageReversal.GetHashCode())
                            {
                                def = (SunInterfaceQueueDef)def.Clone();
                                def.QueueId = -1;
                                def.FiscalYear = 0;
                                def.Period = 0;
                                def.CategoryType = CategoryType.REVERSAL;
                                if (s == SunInterfaceTypeRef.Id.ProvisionForFabricLiabilities.GetHashCode())
                                    def.SourceId = 2;
                                else if (s == SunInterfaceTypeRef.Id.QCC.GetHashCode())
                                    def.SourceId = 2;
                                else
                                    def.SourceId = 2;
                                accountWorker.updateSunInterfaceQueue(def);
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

        }

        private void generateInterface(SunInterfaceQueueDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList list = accountWorker.getInterfaceData(def);
                ArrayList filteredList = new ArrayList();

                EpicorInterfaceWorker.Instance.initialize(def);

                foreach (SunInterfaceLogDef logDef in list)
                {
                    logDef.QueueId = def.QueueId;
                    logDef.CategoryId = def.CategoryType.Id;

                    if (logDef.CategoryId == CategoryType.REVERSAL.Id)
                    {
                        logDef.IsReversalEntry = false;
                        /*
                        if (logDef.OfficeId == OfficeId.TH.Id) // 2018-01-25 Merging TH to HK
                            logDef.OfficeId = OfficeId.HK.Id;
                        */

                        if (logDef.CurrencyId == CurrencyId.USD.Id)
                        {
                            logDef.BaseAmount = logDef.OtherAmount;
                            logDef.FullBaseAmount = logDef.FullOtherAmount;
                        }
                    }

                    if (logDef.BaseAmount == 0 && logDef.OtherAmount != 0)
                        NoticeHelper.sendGeneralMessage("Exchange Rate Not Defined While Generating Sun Interface", "QueueId: " + def.QueueId.ToString() + ", ShipmentId: " + logDef.ShipmentId.ToString() + ", Amount = 0, Exchange Rate Not Defined?");
                    else if (accountWorker.isSunInterfaceDisabled(logDef.ShipmentId)) //  && logDef.CategoryId != CategoryType.REVERSAL.Id
                        NoticeHelper.sendGeneralMessage("Sun Interface Generation is Disabled", "QueueId: " + def.QueueId.ToString() + ", ShipmentId: " + logDef.ShipmentId.ToString() + ", Amount = 0, Exchange Rate Not Defined?");
                    else
                    {
                        /*
                        if (accountWorker.isLDPSuppliers(logDef.UKSupplierCode) && logDef.TradingAgencyId == TradingAgencyId.NSLVMOT.AgencyId && logDef.OfficeId == OfficeId.TR.Id)
                        {
                            logDef.BaseAmount = Math.Round(logDef.OtherAmount * commonWorker.getExchangeRate(ExchangeRateType.INVOICE, logDef.CurrencyId, logDef.TransactionDate) / commonWorker.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.GBP.Id, logDef.TransactionDate), 2, MidpointRounding.AwayFromZero);
                            logDef.FullBaseAmount = logDef.BaseAmount;
                        }
                        */

                        /*
                        if (logDef.TransactionDate <= new DateTime(2011, 1, 29))
                        {
                            if (logDef.OfficeId == OfficeId.DG.Id)
                                logDef.OfficeId = 1;
                            if (logDef.OfficeId == OfficeId.VN.Id)
                                logDef.OfficeId = 4;
                            else if (logDef.OfficeId == OfficeId.EG.Id)
                                logDef.OfficeId = 9;
                        }
                        */

                        if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.Purchase.GetHashCode() && logDef.OfficeId == OfficeId.BD.Id && logDef.Quantity > 0
                            && (logDef.OtherAmount / logDef.Quantity) == 0.01m
                            && (logDef.IsSampleOrder || (logDef.ShipmentId > 0 && OrderSelectWorker.Instance.getShipmentByKey(logDef.ShipmentId).IsPressSample == 1)))
                        {
                            logDef.BaseAmount = 0;
                            logDef.OtherAmount = 0;
                            logDef.FullBaseAmount = 0;
                            logDef.FullOtherAmount = 0;
                            if (logDef.CategoryId == CategoryType.REVERSAL.Id)
                                filteredList.Add(logDef);
                        }
                        else
                            filteredList.Add(logDef);

                        /* TODO:EPICOR */
                        if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.Purchase.GetHashCode() && logDef.SupplierInvoiceNo.Trim() == string.Empty)
                            logDef.SupplierInvoiceNo = accountWorker.getNextMissingSuplierInvoiceNo();
                        
                        accountWorker.updateSunInterfaceLog(logDef);

                        if (logDef.CategoryId != CategoryType.REVERSAL.Id && logDef.CategoryId != CategoryType.ACCRUAL.Id && logDef.CategoryId != CategoryType.CONSOLIDATED.Id)
                        {
                            DailySunInterfaceDef dailyDef = accountWorker.getDailySunInterface(logDef.ShipmentId, logDef.SplitShipmentId, def.SunInterfaceTypeId);
                            if (dailyDef != null)
                            {
                                dailyDef.IsActive = false;
                                dailyDef.ExtractedDate = DateTime.Now;
                                accountWorker.updateDailySunInterface(dailyDef);
                            }
                        }

                        if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKClaim.GetHashCode() && int.Parse(logDef.ReferenceNo) > 0)
                        {
                            UKClaimDef ukClaimDef = null;
                            ukClaimDef = UKClaimWorker.Instance.getUKClaimByKey(int.Parse(logDef.ReferenceNo));
                            ukClaimDef.IsInterfaced = true;
                            UKClaimWorker.Instance.updateUKClaim(ukClaimDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKClaim.GetHashCode() && int.Parse(logDef.ReferenceNo) < 0)
                        {
                            UKClaimRefundDef ukClaimRefundDef = null;
                            ukClaimRefundDef = UKClaimWorker.Instance.getUKClaimRefundByKey(Math.Abs(int.Parse(logDef.ReferenceNo)));
                            ukClaimRefundDef.IsInterfaced = true;
                            UKClaimWorker.Instance.updateUKClaimRefund(ukClaimRefundDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKDiscountClaim.GetHashCode() && int.Parse(logDef.ReferenceNo) > 0)
                        {
                            UKDiscountClaimDef ukDiscountClaimDef = null;
                            ukDiscountClaimDef = UKClaimWorker.Instance.getUKDiscountClaimByKey(int.Parse(logDef.ReferenceNo));
                            ukDiscountClaimDef.IsInterfaced = 1;
                            UKClaimWorker.Instance.updateUKDiscountClaim(ukDiscountClaimDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKDiscountClaim.GetHashCode() && int.Parse(logDef.ReferenceNo) < 0)
                        {
                            UKDiscountClaimRefundDef ukDiscountClaimRefundDef = null;
                            ukDiscountClaimRefundDef = UKClaimWorker.Instance.getUKDiscountClaimRefundByKey(Math.Abs(int.Parse(logDef.ReferenceNo)));
                            ukDiscountClaimRefundDef.IsInterfaced = 1;
                            UKClaimWorker.Instance.updateUKDiscountClaimRefund(ukDiscountClaimRefundDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKDiscountClaimClearing.GetHashCode() && int.Parse(logDef.ReferenceNo) > 0)
                        {
                            UKDiscountClaimDef ukDiscountClaimDef = null;
                            ukDiscountClaimDef = UKClaimWorker.Instance.getUKDiscountClaimByKey(int.Parse(logDef.ReferenceNo));
                            ukDiscountClaimDef.IsInterfaced = 2;
                            UKClaimWorker.Instance.updateUKDiscountClaim(ukDiscountClaimDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKDiscountClaimClearing.GetHashCode() && int.Parse(logDef.ReferenceNo) < 0)
                        {
                            UKDiscountClaimRefundDef ukDiscountClaimRefundDef = null;
                            ukDiscountClaimRefundDef = UKClaimWorker.Instance.getUKDiscountClaimRefundByKey(Math.Abs(int.Parse(logDef.ReferenceNo)));
                            ukDiscountClaimRefundDef.IsInterfaced = 2;
                            UKClaimWorker.Instance.updateUKDiscountClaimRefund(ukDiscountClaimRefundDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKClaimRechargeToSupplier.GetHashCode()
                                || logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UKClaimRechargeToSupplier_ChangeCurrency.GetHashCode())
                        {
                            UKClaimDCNoteDef dcNoteDef = null;
                            dcNoteDef = UKClaimWorker.Instance.getUKClaimDCNoteByDCNoteNo(logDef.DebitCreditNoteNo);
                            dcNoteDef.IsInterfaced = true;
                            UKClaimWorker.Instance.updateUKClaimDCNote(dcNoteDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.APAdjustment.GetHashCode()
                                 || logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.APAdjustment_ChangeCurrency.GetHashCode())
                        {
                            AdjustmentDetailDef adjDetailDef = null;
                            adjDetailDef = accountWorker.getAdjustmentDetailByKey(int.Parse(logDef.ReferenceNo));
                            adjDetailDef.IsInterfaced = true;
                            accountWorker.updateAdjustmentDetail(adjDetailDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.AdvancePayment.GetHashCode())
                        {
                            AdvancePaymentDef advPaymentDef = null;
                            advPaymentDef = advancePaymentWorker.getAdvancePaymentByKey(int.Parse(logDef.ReferenceNo));
                            advPaymentDef.IsInterfaced = 1;
                            advancePaymentWorker.updateAdvancePayment(advPaymentDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.AdvancePaymentInterest.GetHashCode())
                        {
                            int paymentId = int.Parse(logDef.ReferenceNo.Split('|')[0]);
                            DateTime paymentDate = DateTimeUtility.getDate(logDef.ReferenceNo.Split('|')[1]);
                            AdvancePaymentInstalmentDetailDef instalmentDef = advancePaymentWorker.getAdvancePaymentInstalmentDetailByKey(paymentId, paymentDate);

                            instalmentDef.IsDCNoteInterfaced = true;
                            advancePaymentWorker.updateAdvancePaymentInstalmentDetail(instalmentDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.OtherChargeDCNote.GetHashCode())
                        {
                            GenericDCNoteDef genericDCNoteDef = null;
                            genericDCNoteDef = accountWorker.getGenericDCNoteById(int.Parse(logDef.ReferenceNo));
                            genericDCNoteDef.IsInterfaced = true;
                            string s;
                            accountWorker.updateGenericDCNote(genericDCNoteDef, def.User.UserId, out s);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.MockShop.GetHashCode())
                        {
                            MockShopDCNoteDef mockShopDCNoteDef = null;
                            mockShopDCNoteDef = accountWorker.getMockShopDCNoteByDCNoteNo(logDef.DebitCreditNoteNo);
                            mockShopDCNoteDef.IsInterfaced = true;
                            accountWorker.updateMockShopDCNote(mockShopDCNoteDef, -1, -1, 99999);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.StudioSample.GetHashCode())
                        {
                            StudioDCNoteDef studioDCNoteDef = null;
                            studioDCNoteDef = accountWorker.getStudioDCNoteByDCNoteNo(logDef.DebitCreditNoteNo);
                            studioDCNoteDef.IsInterfaced = true;
                            accountWorker.updateStudioDCNote(studioDCNoteDef, -1, -1, 99999);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NSLedSales_Duty.GetHashCode() || logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NSLedSales_NonDuty.GetHashCode())
                        {
                            NSLedImportFileDef nsLedFileDef = null;
                            nsLedFileDef = accountWorker.getNSLedImportFileByKey(int.Parse(logDef.ReferenceNo));
                            nsLedFileDef.IsInterfaced = 1;
                            accountWorker.updateNSLedImportFile(nsLedFileDef, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.QACommission.GetHashCode())
                        {
                            QACommissionDNDef qaDNDef = null;
                            qaDNDef = accountWorker.getQACommissionDNByShipmentId(logDef.ShipmentId);
                            qaDNDef.IsInterfaced = true;
                            accountWorker.updateQACommissionDN(qaDNDef, def.OfficeGroup.OfficeGroupId, def.User.UserId);
                        }

                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.UTContractDN.GetHashCode())
                        {
                            UTContractDCNoteDef utDNDef = null;
                            utDNDef = accountWorker.getUTContractDCNoteByKey(int.Parse(logDef.ReferenceNo));
                            utDNDef.IsInterfaced = true;
                            accountWorker.updateUTContractDCNote(utDNDef, def.OfficeGroup.OfficeGroupId, def.User.UserId);
                        }
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.ARAdjustmentForEziBuy.GetHashCode())
                        {
                            AdjustmentDetailDef adjDetailDef = null;
                            adjDetailDef = accountWorker.getAdjustmentDetail(AdjustmentType.SALES_ADJUSTMENT, logDef.ShipmentId, logDef.SplitShipmentId);
                            adjDetailDef.IsInterfaced = true;
                            accountWorker.updateAdjustmentDetail(adjDetailDef, def.User.UserId);
                        }
                        /*
                        else if (logDef.SunInterfaceTypeId == SunInterfaceTypeRef.Id.ARAdjustmentForNext.GetHashCode())
                        {
                            AdjustmentDetailDef adjDetailDef = null;
                            adjDetailDef = accountWorker.getAdjustmentDetail(AdjustmentType.SALES_ADJUSTMENT, logDef.ShipmentId, logDef.SplitShipmentId);
                            adjDetailDef.IsInterfaced = true;
                            accountWorker.updateAdjustmentDetail(adjDetailDef, def.User.UserId);
                        }
                        */
                    }

                    if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.Purchase.GetHashCode() && def.CategoryType.Id != CategoryType.ACCRUAL.Id)
                    {
                        string tmpSupplierInvoiceNo = logDef.SupplierInvoiceNo.Trim();

                        if (tmpSupplierInvoiceNo.Length > (def.CategoryType.Id == CategoryType.REVERSAL.Id ? 18 : 20))
                        {
                            logDef.DebitCreditNoteNo = logDef.ShipmentId.ToString() + "-" + tmpSupplierInvoiceNo.Substring(0, (def.CategoryType.Id == CategoryType.REVERSAL.Id ? 18 : 20) - (logDef.ShipmentId.ToString().ToString().Length + 1));
                            tmpSupplierInvoiceNo = logDef.DebitCreditNoteNo;
                        }

                        string supplierInvoiceNoSuffix = accountWorker.getSupplierInvoiceNoSuffix(tmpSupplierInvoiceNo);
                        if (supplierInvoiceNoSuffix != string.Empty)
                        {
                            logDef.DebitCreditNoteNo = tmpSupplierInvoiceNo + supplierInvoiceNoSuffix;
                        }
                        accountWorker.updateSunInterfaceLog(logDef);
                    }



                    if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.Purchase.GetHashCode() && (logDef.CustomerId == CustomerDef.Id.ns_led.GetHashCode() || logDef.CustomerId == CustomerDef.Id.manu_led.GetHashCode()))
                    {
                        ShipmentDef shipmentDef = orderSelectWorker.getShipmentByKey(logDef.ShipmentId);
                        ProductDef product = ProductWorker.Instance.getProductByKey(logDef.ProductId);

                        decimal actualFreightUnitCostInUSD = accountWorker.getRangePlanActualFreightUnitCostInUSD(logDef.OfficeId, product.ItemNo, this.getNSLedRangePlanSeasonIdByDeliveryDate(logDef.OfficeId, product.ItemNo, shipmentDef.CustomerAgreedAtWarehouseDate));

                        decimal dutyPercent = accountWorker.getRangePlanDutyPercent(logDef.OfficeId, product.ItemNo, this.getNSLedRangePlanSeasonIdByDeliveryDate(logDef.OfficeId, product.ItemNo, shipmentDef.CustomerAgreedAtWarehouseDate));
                        logDef.ReferenceNo = actualFreightUnitCostInUSD.ToString() + "|" + dutyPercent.ToString();
                        accountWorker.updateSunInterfaceLog(logDef);
                    }
                    
                }

                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.AccountReceivable.GetHashCode())
                    accountWorker.deleteOutstandingILSDiffDCNoteShipment(def.FiscalYear, def.Period, def.OfficeGroup.OfficeGroupId, 1);

                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.SalesCommissionSettlement.GetHashCode())
                    accountWorker.deleteOutstandingILSDiffDCNoteShipment(def.FiscalYear, def.Period, def.OfficeGroup.OfficeGroupId, 2);

                /*
                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.Purchase.GetHashCode() && def.CategoryType.Id != CategoryType.ACCRUAL.Id)
                {
                    bool isRepeat = false;
                    int repeatCount = 0;
                    for (int i = 0; i < filteredList.Count; i++)
                    {
                        bool isContinued = true;
                        SunInterfaceLogDef currentLogDef = (SunInterfaceLogDef)filteredList[i];

                        if (i != filteredList.Count - 1)
                        {
                            if (currentLogDef.SupplierInvoiceNo == ((SunInterfaceLogDef)filteredList[i + 1]).SupplierInvoiceNo)
                                isRepeat = true;
                            else
                                isContinued = false;
                        }

                        if (isRepeat)
                        {
                            repeatCount += 1;
                            currentLogDef.DebitCreditNoteNo = currentLogDef.SupplierInvoiceNo + "#" + repeatCount.ToString();
                            accountWorker.updateSunInterfaceLog(currentLogDef);
                        }

                        if (!isContinued)
                        {
                            isRepeat = false;
                            repeatCount = 0;
                        }
                    }
                }
                */

                if (filteredList.Count > 0)
                {
                    ArrayList safList = accountWorker.convertToSAFList(filteredList, def.SunInterfaceTypeId, def.CategoryType.Id, def.OfficeGroup.OfficeGroupId);

                    string fileName = string.Empty;

                    /*
                    if (def.SourceId == 1)
                    {
                        if (def.CategoryType.Id == CategoryType.REVERSAL.Id)
                        {
                            foreach (SAFDef safDef in safList)
                            {
                                safDef.JournalType = safDef.JournalType.Substring(0, 1) + "R" + safDef.JournalType.Substring(2, 1);
                            }
                        }
                    }

                    fileName = accountWorker.exportToSunInterfaceFile(def, safList, 1);
                    
                    if ((def.SunInterfaceTypeId != SunInterfaceTypeRef.Id.MockShopSales.GetHashCode() && def.SunInterfaceTypeId != SunInterfaceTypeRef.Id.MockShopSalesCommission.GetHashCode()) || def.CategoryType.Id == CategoryType.REVERSAL.Id)
                        NoticeHelper.sendSunInterfaceMail(def, fileName);
                    */


                    /*
                    if (def.SourceId == 2)
                    {
                        bool isFirstLog = true;
                        int seqNo = 0;
                        
                        foreach (SAFDef safDef in safList)
                        {
                            if (def.CategoryType.Id == CategoryType.REVERSAL.Id)
                                safDef.JournalType = safDef.JournalType.Substring(0, 1) + "MR";
                            else
                                safDef.JournalType = safDef.JournalType.Substring(0, 1) + "M" + safDef.JournalType.Substring(2, 1);
                            
                            //if (safDef.JournalType.Substring(1, 2) == "MV" && safDef.T6 != String.Empty)
                            //    safDef.T6 = "O";
                            
                            if (isFirstLog)
                            {
                                seqNo = accountWorker.getTransactionRefNo(safDef.JournalType + safDef.PeriodString);
                                //if (def.CategoryType.Id == CategoryType.REVERSAL.Id)
                                //    seqNo += 1000;
                            }
                            safDef.TransactionRefSeqNo = seqNo;
                            isFirstLog = false;
                        }
                        
                        
                        //fileName = accountWorker.exportToSunInterfaceFile(def, safList, def.SourceId);
                    }
                    */
                }
                /*
                else
                    NoticeHelper.sendEmptySunInterfaceMail(def);
                */

                def.CompleteTime = DateTime.Now;
                def.Status = 1;
                accountWorker.updateSunInterfaceQueue(def);

                ctx.VoteCommit();

                ArrayList fileList = new ArrayList();

                List<string> files = EpicorInterfaceWorker.Instance.GLInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);
                files = EpicorInterfaceWorker.Instance.APInvoiceInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);
                files = EpicorInterfaceWorker.Instance.ARInvoiceInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);
                files = EpicorInterfaceWorker.Instance.PaymentInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);
                files = EpicorInterfaceWorker.Instance.ReceiptInterfaceFile.Export(def.SourceId == 1 ? false : true, def.User.UserId);
                fileList.AddRange(files);

                if (fileList.Count > 0)
                    NoticeHelper.sendEpicorInterfaceMail(def, fileList);

                EpicorInterfaceWorker.Instance.dispose();

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

        public ArrayList getOutstandingSunInterfaceQueue()
        {
            return accountWorker.getSunInterfaceQueueList();
        }

        public bool isSunInterfaceBatchGenerated(int officeId, int fiscalYear, int period)
        {
            if (accountWorker.getSunInterfaceQueueList(officeId, fiscalYear, period, SunInterfaceTypeRef.Id.Sales.GetHashCode(), CategoryType.ACTUAL.Id, 2, 2).Count > 0)
                return true;
            else
                return false;
        }

        public ArrayList getRecentSunInterfaceQueue()
        {
            return accountWorker.getRecentSunInterfaceQueueList();
        }

        public SunInterfaceQueueDef getSunInterfaceQueueByKey(int queueId)
        {
            return accountWorker.getSunInterfaceQueueByKey(queueId);
        }

        public SunInterfaceQueueDef getSunInterfaceQueue(int officeId, int fiscalYear, int period, int sunInterfaceTypeId, int categoryId, int uTurn, int sourceId)
        {
            ArrayList list = accountWorker.getSunInterfaceQueueList(officeId, fiscalYear, period, sunInterfaceTypeId, categoryId, uTurn, sourceId);
            if (list.Count == 0)
                return null;
            else
                return (SunInterfaceQueueDef)list[0];
        }

        /*
        public void holdPayment(ArrayList list, bool isPaymentHold, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList shipmentList = new ArrayList();
                ArrayList splitShipmentList = new ArrayList();

                foreach (string idString in list)
                {
                    string[] id = idString.Split(',');
                    int shipmentId = int.Parse(id[0]);
                    int splitShipmentId = int.Parse(id[1]);

                    if (splitShipmentId != 0)
                    {
                        SplitShipmentDef splitDef = orderSelectWorker.getSplitShipmentByKey(splitShipmentId);
                        splitDef.IsPaymentHold = isPaymentHold;
                        splitShipmentList.Add(splitDef);
                    }
                    else
                    {
                        ShipmentDef def = orderSelectWorker.getShipmentByKey(shipmentId);
                        def.IsPaymentHold = isPaymentHold;
                        shipmentList.Add(def);
                    }
                }

                if (shipmentList.Count > 0)
                    orderWorker.updateShipmentList(shipmentList, userId);
                if (splitShipmentList.Count > 0)
                    orderWorker.updateSplitShipmentList(splitShipmentList, userId);

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
        */

        public void holdPayment(int vendorId, bool isPaymentHold, string remark, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                accountWorker.updateSupplierPaymentStatus(vendorId, isPaymentHold, remark, userId);

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

        public void markDMSDocumentAsReviewed(int shipmentId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                ShipmentDef def = orderSelectWorker.getShipmentByKey(shipmentId);
                def.DocumentReviewedBy = userId;
                def.DocumentReviewedOn = DateTime.Now;
                def.ShippingDocWFS = ShippingDocWFS.REVIEWED;
                orderWorker.updateShipmentList(ConvertUtility.createArrayList(def), userId);
                ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(shipmentId, 0, ActionHistoryType.MISCELLANEOUS, "Supplier Doc(s) Reviewed By " + generalWorker.getUserByKey(userId).DisplayName, GeneralCriteria.TRUE, userId));

                ArrayList splitShipments = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(shipmentId);
                foreach (SplitShipmentDef splitDef in splitShipments)
                {
                    splitDef.DocumentReviewedBy = userId;
                    splitDef.DocumentReviewedOn = DateTime.Now;
                    splitDef.ShippingDocWFS = ShippingDocWFS.REVIEWED;
                    orderWorker.updateSplitShipmentList(ConvertUtility.createArrayList(splitDef), userId);
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


        public void updateDMSWorkflowStatus(ArrayList list, int dmsWorkflowStatusId, int userId)
        {
            updateDMSWorkflowStatus(list, dmsWorkflowStatusId, RejectPaymentReason.Unknown.Id, userId);
        }

        public void updateDMSWorkflowStatus(ArrayList list, int dmsWorkflowStatusId, int rejectPaymentReasonId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                foreach (string idString in list)
                {
                    string[] id = idString.Split(',');
                    int shipmentId = int.Parse(id[0]);
                    int splitShipmentId = int.Parse(id[1]);
                    InvoiceDef invoiceDef = shippingWorker.getInvoiceByKey(shipmentId);

                    if (RejectPaymentReason.getReason(rejectPaymentReasonId) == null)
                        rejectPaymentReasonId = RejectPaymentReason.Unknown.Id;
                    if (splitShipmentId != 0)
                    {
                        SplitShipmentDef splitDef = orderSelectWorker.getSplitShipmentByKey(splitShipmentId);
                        splitDef.ShippingDocWFS = ShippingDocWFS.getType(dmsWorkflowStatusId);
                        splitDef.RejectPaymentReasonId = (splitDef.ShippingDocWFS == ShippingDocWFS.REJECTED ? rejectPaymentReasonId : RejectPaymentReason.NoReason.Id);
                        if (splitDef.ShippingDocWFS == ShippingDocWFS.REJECTED)
                        {
                            splitDef.AccountDocReceiptDate = DateTime.MinValue;
                            splitDef.ShippingDocCheckedOn = DateTime.MinValue;
                            splitDef.ShippingDocCheckedBy = null;
                            splitDef.ShippingCheckedTotalNetAmount = 0;
                            splitDef.DocumentReviewedBy = 0;
                            splitDef.DocumentReviewedOn = DateTime.MinValue;
                        }
                        orderWorker.updateSplitShipmentList(ConvertUtility.createArrayList(splitDef), userId);

                        DailySunInterfaceDef dailyDef = accountWorker.getDailySunInterface(shipmentId, splitShipmentId, SunInterfaceTypeRef.Id.Purchase.GetHashCode());
                        resetActiveDailySunInterface(dailyDef);

                        dailyDef = accountWorker.getDailySunInterface(shipmentId, 0, SunInterfaceTypeRef.Id.Purchase.GetHashCode());
                        resetActiveDailySunInterface(dailyDef);

                        if (splitDef.ShippingDocWFS == ShippingDocWFS.REJECTED)
                        {
                            ShipmentDef def = orderSelectWorker.getShipmentByKey(shipmentId);
                            def.ShippingDocWFS = ShippingDocWFS.getType(dmsWorkflowStatusId);
                            def.RejectPaymentReasonId = (def.ShippingDocWFS == ShippingDocWFS.REJECTED ? rejectPaymentReasonId : RejectPaymentReason.NoReason.Id);
                            def.DocumentReviewedBy = 0;
                            def.DocumentReviewedOn = DateTime.MinValue;
                            orderWorker.updateShipmentList(ConvertUtility.createArrayList(def), userId);

                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(shipmentId, splitShipmentId, ActionHistoryType.MISCELLANEOUS, "Supplier Doc(s) Rejected By " + generalWorker.getUserByKey(userId).DisplayName + " [Reason: " + RejectPaymentReason.getReason(rejectPaymentReasonId).Name + "]", GeneralCriteria.TRUE, userId));
                            NoticeHelper.sendSupplierDocRejectEmail(shipmentId, userId);
                        }
                        else if (splitDef.ShippingDocWFS == ShippingDocWFS.ACCEPTED)
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(shipmentId, splitShipmentId, ActionHistoryType.MISCELLANEOUS, "Supplier Doc(s) Accepted By " + generalWorker.getUserByKey(userId).DisplayName, GeneralCriteria.TRUE, userId));
                    }
                    else
                    {
                        ShipmentDef def = orderSelectWorker.getShipmentByKey(shipmentId);
                        def.ShippingDocWFS = ShippingDocWFS.getType(dmsWorkflowStatusId);
                        def.RejectPaymentReasonId = (def.ShippingDocWFS == ShippingDocWFS.REJECTED ? rejectPaymentReasonId : RejectPaymentReason.NoReason.Id);
                        def.DocumentReviewedBy = 0;
                        def.DocumentReviewedOn = DateTime.MinValue;
                        orderWorker.updateShipmentList(ConvertUtility.createArrayList(def), userId);

                        DailySunInterfaceDef dailyDef = accountWorker.getDailySunInterface(shipmentId, 0, SunInterfaceTypeRef.Id.Purchase.GetHashCode());
                        resetActiveDailySunInterface(dailyDef);

                        if (def.ShippingDocWFS == ShippingDocWFS.REJECTED)
                        {
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(shipmentId, 0, ActionHistoryType.MISCELLANEOUS, "Supplier Doc(s) Rejected By " + generalWorker.getUserByKey(userId).DisplayName + " [Reason: " + RejectPaymentReason.getReason(rejectPaymentReasonId).Name + "]", GeneralCriteria.TRUE, userId));
                            NoticeHelper.sendSupplierDocRejectEmail(shipmentId, userId);
                        }
                        else if (def.ShippingDocWFS == ShippingDocWFS.ACCEPTED)
                            ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(shipmentId, 0, ActionHistoryType.MISCELLANEOUS, "Supplier Doc(s) Accepted By " + generalWorker.getUserByKey(userId).DisplayName, GeneralCriteria.TRUE, userId));
                    }

                    if (ShippingDocWFS.getType(dmsWorkflowStatusId) == ShippingDocWFS.REJECTED)
                    {
                        invoiceDef.ShippingDocCheckedOn = DateTime.MinValue;
                        invoiceDef.ShippingDocCheckedBy = null;
                        invoiceDef.ShippingCheckedTotalNetAmount = 0;
                        invoiceDef.AccountDocReceiptDate = DateTime.MinValue;
                        shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.MISCELLANEOUS, new ArrayList(), 99999);
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

        private void resetActiveDailySunInterface(DailySunInterfaceDef def)
        {
            if (def != null && def.ExtractedDate == DateTime.MinValue && def.IsActive == true)
            {
                def.IsActive = false;
                accountWorker.updateDailySunInterface(def);
            }
        }

        public void submitSunInterfaceRequest(SunInterfaceQueueDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                accountWorker.updateSunInterfaceQueue(def);

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

        public void markDMSComplete(int officeId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                accountWorker.markDMSComplete(officeId);

                NoticeHelper.sendGeneralMessage("Mark DMS Complete - " + OfficeId.getName(officeId) + " Office", generalWorker.getUserByKey(userId).DisplayName);

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

        public SunInterfaceLogDef getLatestLogByShipmentId(int sunInterfaceTypeId, int shipmentId, int splitShipmentId)
        {
            return accountWorker.getLatestLogByShipmentId(sunInterfaceTypeId, shipmentId, splitShipmentId);
        }

        public SunInterfaceLogDef getInitialLogByShipmentId(int sunInterfaceTypeId, int shipmentId, int splitShipmentId)
        {
            return accountWorker.getInitialLogByShipmentId(sunInterfaceTypeId, shipmentId, splitShipmentId);
        }

        public DailySunInterfaceDef getDailySunInterface(int shipmentId, int splitShipmentId, int sunInterfaceTypeId)
        {
            return accountWorker.getDailySunInterface(shipmentId, splitShipmentId, sunInterfaceTypeId);
        }

        public ArrayList getReversalLogListByShipmentId(int sunInterfaceTypeId, int shipmentId)
        {
            return accountWorker.getReversalLogListByShipmentId(sunInterfaceTypeId, shipmentId);
        }

        public ArrayList getBankReconciliationRequestList()
        {
            return accountWorker.getBankReconciliationRequestList(GeneralCriteria.ALL);
        }

        public EInvoiceBatchDef getEInvoiceBatchByKey(int key)
        {
            return accountWorker.getEInvoiceBatchByKey(key);
        }

        public ArrayList getUploadedARAPData(int userId, string sourcePath, string fileType, out ArrayList ccyDiscrepancyList, out ArrayList updatedList, out bool isFileSplit)
        {
            return accountWorker.getUploadedARAPData(userId, sourcePath, fileType, out ccyDiscrepancyList, out updatedList, out isFileSplit);
        }

        public ArrayList getPaymentReferenceCodeList()
        {
            return accountWorker.getPaymentReferenceCodeList();
        }

        public void updateBankReconciliationRequest(BankReconciliationRequestDef request)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                accountWorker.updateBankReconciliationRequest(request);

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

        public ArrayList updateARAP(ArrayList list, int userId)
        {
            InvoiceDef invoiceDef;
            SplitShipmentDef splitShipmentDef;
            ArrayList splitShipmentList;
            ArrayList amendmentList = new ArrayList();
            ArrayList updateFailedList = new ArrayList();
            ArrayList tempList;
            DateTime max = DateTime.MinValue;
            DailySunInterfaceDef dailySunInterfaceDef = null;
            bool isUpdated = false;
            int retryCount = 0;
            /*
            bool ilsSalesDiscrepancy = false;
            bool ilsSalesCommissionDiscrepancy = false;
            */


            foreach (ContractShipmentListJDef def in list)
            {
                isUpdated = false;
                retryCount = 0;

                while (isUpdated == false && retryCount < 3)
                {
                    TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
                    try
                    {
                        ctx.Enter();
                        TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                        if (def.InvoiceNo.Contains("/"))
                        {
                            decimal orig_ARAmt = int.MinValue;
                            tempList = shippingWorker.getInvoiceByInvoiceNo(def.InvoicePrefix, def.InvoiceSeq, def.InvoiceYear, def.SequenceNo == int.MinValue ? -1 : def.SequenceNo);
                            invoiceDef = (InvoiceDef)tempList[0];
                            if (invoiceDef.ARAmt != def.ARAmount || invoiceDef.ARDate != def.ARDate || invoiceDef.ARRefNo != def.ARRefNo)
                            {
                                /*
                                ShipmentDef shipmentDef = orderSelectWorker.getShipmentByKey(invoiceDef.ShipmentId);
                                if (invoiceDef.ARDate == DateTime.MinValue && invoiceDef.IsSelfBilledOrder)
                                {
                                    DailySunInterfaceDef dsiDef = accountWorker.getDailySunInterface(invoiceDef.ShipmentId, 0, SunInterfaceTypeRef.Id.ILSSalesDiscrepancy.GetHashCode());
                                    dsiDef.IsActive = true;
                                    accountWorker.updateDailySunInterface(dsiDef);

                                    ilsSalesDiscrepancy = true;
                                }
                                */

                                orig_ARAmt = invoiceDef.ARAmt;
                                invoiceDef.ARAmt = def.ARAmount;
                                invoiceDef.ARDate = def.ARDate;
                                invoiceDef.ARRefNo = def.ARRefNo;

                                if (def.ARDate != DateTime.MinValue)
                                    invoiceDef.ARExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.SellCurrency.CurrencyId, def.ARDate);
                                shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.AR_AP_MAINTENANCE, "A/R Settlement Information Updated", GeneralCriteria.TRUE, userId));
                            }

                            if (invoiceDef.APAmt != def.APAmount || invoiceDef.APDate != def.APDate || invoiceDef.APRefNo != def.APRefNo)
                            {
                                invoiceDef.APAmt = def.APAmount;
                                invoiceDef.APDate = def.APDate;
                                invoiceDef.APRefNo = def.APRefNo;
                                if (def.APDate != DateTime.MinValue)
                                    invoiceDef.APExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.BuyCurrency.CurrencyId, def.APDate);
                                shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.AR_AP_MAINTENANCE, "A/P Settlement Information Updated", GeneralCriteria.TRUE, userId));

                                dailySunInterfaceDef = AccountManager.Instance.getDailySunInterface(def.ShipmentId, 0, SunInterfaceTypeRef.Id.AccountPayable.GetHashCode());
                                if (dailySunInterfaceDef != null)
                                {
                                    if (invoiceDef.APAmt == 0 && invoiceDef.APDate == DateTime.MinValue && invoiceDef.APRefNo == "")
                                        resetActiveDailySunInterface(dailySunInterfaceDef);
                                    else
                                        setDailySunInterfaceActive(dailySunInterfaceDef);
                                }
                            }

                            if (invoiceDef.NSLCommissionSettlementAmt != def.NSLCommissionSettlementAmount ||
                                invoiceDef.NSLCommissionSettlementDate != def.NSLCommissionSettlementDate ||
                                invoiceDef.NSLCommissionRefNo != def.NSLCommissionSettlementRefNo)
                            {
                                /*
                                if (invoiceDef.NSLCommissionSettlementDate == DateTime.MinValue && invoiceDef.IsSelfBilledOrder)
                                {
                                    DailySunInterfaceDef dsiDef = accountWorker.getDailySunInterface(invoiceDef.ShipmentId, 0, SunInterfaceTypeRef.Id.ILSSalesCommissionDiscrepancy.GetHashCode());
                                    dsiDef.IsActive = true;
                                    accountWorker.updateDailySunInterface(dsiDef);

                                    ilsSalesCommissionDiscrepancy = true;
                                }
                                */

                                invoiceDef.NSLCommissionSettlementAmt = def.NSLCommissionSettlementAmount;
                                invoiceDef.NSLCommissionSettlementDate = def.NSLCommissionSettlementDate;
                                invoiceDef.NSLCommissionRefNo = def.NSLCommissionSettlementRefNo;

                                if (def.NSLCommissionSettlementDate != DateTime.MinValue)
                                    invoiceDef.NSLCommissionSettlementExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.SellCurrency.CurrencyId, def.NSLCommissionSettlementDate);
                                shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.AR_AP_MAINTENANCE, "Sales Commission Settlement Information Updated", GeneralCriteria.TRUE, userId));
                            }

                            shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.AR_AP_MAINTENANCE, amendmentList, userId);

                            if (orig_ARAmt != int.MinValue && orig_ARAmt != def.ARAmount)
                                accountWorker.doAdjustmentAfterSettlement(invoiceDef.ShipmentId, 0, userId);
                        }
                        else
                        {
                            splitShipmentDef = orderSelectWorker.getSplitShipmentByPONo(def.InvoiceNo.Substring(0, def.InvoiceNo.IndexOf("-") - 1),
                            def.InvoiceNo.Substring(def.InvoiceNo.IndexOf("-") - 1, 1), Convert.ToInt32(def.InvoiceNo.Substring(def.InvoiceNo.IndexOf("-") + 1)));

                            splitShipmentDef.APAmt = def.APAmount;
                            splitShipmentDef.APDate = def.APDate;
                            splitShipmentDef.APRefNo = def.APRefNo;
                            splitShipmentDef.APExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.BuyCurrency.CurrencyId, def.APDate);

                            orderWorker.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipmentDef), userId);
                            shippingWorker.updateActionHistory(new ActionHistoryDef(splitShipmentDef.ShipmentId, splitShipmentDef.SplitShipmentId, ActionHistoryType.AR_AP_MAINTENANCE, "A/P Settlement Information Updated", GeneralCriteria.TRUE, userId));
                            accountWorker.doAdjustmentAfterSettlement(splitShipmentDef.ShipmentId, splitShipmentDef.SplitShipmentId, userId);

                            dailySunInterfaceDef = AccountManager.Instance.getDailySunInterface(def.ShipmentId, splitShipmentDef.SplitShipmentId, SunInterfaceTypeRef.Id.AccountPayable.GetHashCode());
                            if (dailySunInterfaceDef != null)
                            {
                                if (splitShipmentDef.APAmt == 0 && splitShipmentDef.APDate == DateTime.MinValue && splitShipmentDef.APRefNo == "")
                                    resetActiveDailySunInterface(dailySunInterfaceDef);
                                else
                                    setDailySunInterfaceActive(dailySunInterfaceDef);
                            }

                            splitShipmentList = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(splitShipmentDef.ShipmentId);
                            invoiceDef = shippingWorker.getInvoiceByKey(splitShipmentDef.ShipmentId);
                            invoiceDef.APAmt = 0;
                            invoiceDef.APDate = DateTime.MinValue;
                            invoiceDef.APRefNo = String.Empty;
                            max = DateTime.MinValue;
                            bool isSplitUpdated = true;
                            int splitCount = 0;

                            foreach (SplitShipmentDef splitShipment in splitShipmentList)
                            {
                                if (splitShipment.IsVirtualSetSplit != 1)
                                {
                                    splitCount++;
                                    invoiceDef.APAmt += splitShipment.APAmt;
                                    if (splitShipment.APDate == DateTime.MinValue)
                                    {
                                        isSplitUpdated = false;
                                        break;
                                    }
                                    else if (splitShipment.APDate > max)
                                    {
                                        max = splitShipment.APDate;
                                        invoiceDef.APDate = splitShipment.APDate;
                                        invoiceDef.APRefNo = splitShipment.APRefNo;
                                    }
                                }
                            }
                            if (splitCount > 0 && isSplitUpdated)
                            {
                                if (invoiceDef.APDate != DateTime.MinValue)
                                    invoiceDef.APExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.BuyCurrency.CurrencyId, invoiceDef.APDate);
                                shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.AR_AP_MAINTENANCE, amendmentList, userId);
                            }
                            shippingWorker.updateActionHistory(new ActionHistoryDef(splitShipmentDef.ShipmentId, splitShipmentDef.SplitShipmentId, ActionHistoryType.AR_AP_MAINTENANCE, "A/P Settlement Information Updated", GeneralCriteria.TRUE, userId));
                        }
                        foreach (ActionHistoryDef actionHistoryDef in amendmentList)
                        {
                            shippingWorker.updateActionHistory(actionHistoryDef);
                        }

                        ctx.VoteCommit();
                        isUpdated = true;
                    }
                    catch (Exception e)
                    {
                        retryCount++;
                        ctx.VoteRollback();
                        NoticeHelper.sendErrorMessage(e, "AR / AP Update Failed - " + def.InvoiceNo);
                    }
                    finally
                    {
                        ctx.Exit();
                    }
                }

                if (!isUpdated)
                {
                    updateFailedList.Add(def.InvoiceNo);
                }
            }
            /*
            if (ilsSalesDiscrepancy)
                accountWorker.submitDailySunInterfaces(SunInterfaceTypeRef.Id.ILSSalesDiscrepancy.GetHashCode(), userId);
            if (ilsSalesCommissionDiscrepancy)
                accountWorker.submitDailySunInterfaces(SunInterfaceTypeRef.Id.ILSSalesCommissionDiscrepancy.GetHashCode(), userId);
            */
            return updateFailedList;
        }

        public ArrayList updateARAPDN(ArrayList list, int userId)
        {
            InvoiceDef invoiceDef;
            SplitShipmentDef splitShipmentDef;
            ArrayList splitShipmentList;
            ArrayList amendmentList = new ArrayList();
            ArrayList updateFailedList = new ArrayList();
            ArrayList tempList;
            DateTime max = DateTime.MinValue;
            DailySunInterfaceDef dailySunInterfaceDef = null;
            bool isUpdated = false;
            int retryCount = 0;
            ContractShipmentListJDef def = null;
            UKClaimDCNoteDef dnDef = null;

            foreach (Object obj in list)
            {
                isUpdated = false;
                retryCount = 0;

                while (isUpdated == false && retryCount < 3)
                {
                    TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
                    try
                    {
                        ctx.Enter();
                        TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                        def = null;
                        dnDef = null;
                        if (obj.GetType() == typeof(ContractShipmentListJDef))
                            def = (ContractShipmentListJDef)obj;
                        else
                            dnDef = (UKClaimDCNoteDef)obj;

                        if (def != null)
                        {
                            if (def.InvoiceNo.Contains("/"))
                            {
                                #region Invoice
                                decimal orig_ARAmt = int.MinValue;
                                tempList = shippingWorker.getInvoiceByInvoiceNo(def.InvoicePrefix, def.InvoiceSeq, def.InvoiceYear, def.SequenceNo == int.MinValue ? -1 : def.SequenceNo);
                                invoiceDef = (InvoiceDef)tempList[0];
                                if (invoiceDef.ARAmt != def.ARAmount || invoiceDef.ARDate != def.ARDate || invoiceDef.ARRefNo != def.ARRefNo)
                                {
                                    orig_ARAmt = invoiceDef.ARAmt;
                                    invoiceDef.ARAmt = def.ARAmount;
                                    invoiceDef.ARDate = def.ARDate;
                                    invoiceDef.ARRefNo = def.ARRefNo;

                                    if (def.ARDate != DateTime.MinValue)
                                        invoiceDef.ARExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.SellCurrency.CurrencyId, def.ARDate);
                                    shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.AR_AP_MAINTENANCE, "A/R Settlement Information Updated", GeneralCriteria.TRUE, userId));
                                }

                                if (invoiceDef.APAmt != def.APAmount || invoiceDef.APDate != def.APDate || invoiceDef.APRefNo != def.APRefNo)
                                {
                                    invoiceDef.APAmt = def.APAmount;
                                    invoiceDef.APDate = def.APDate;
                                    invoiceDef.APRefNo = def.APRefNo;
                                    if (def.APDate != DateTime.MinValue)
                                        invoiceDef.APExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.BuyCurrency.CurrencyId, def.APDate);
                                    shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.AR_AP_MAINTENANCE, "A/P Settlement Information Updated", GeneralCriteria.TRUE, userId));

                                    dailySunInterfaceDef = AccountManager.Instance.getDailySunInterface(def.ShipmentId, 0, SunInterfaceTypeRef.Id.AccountPayable.GetHashCode());
                                    if (dailySunInterfaceDef != null)
                                    {
                                        if (invoiceDef.APAmt == 0 && invoiceDef.APDate == DateTime.MinValue && invoiceDef.APRefNo == "")
                                            resetActiveDailySunInterface(dailySunInterfaceDef);
                                        else
                                            setDailySunInterfaceActive(dailySunInterfaceDef);
                                    }
                                }


                                if (invoiceDef.NSLCommissionSettlementAmt != def.NSLCommissionSettlementAmount ||
                                    invoiceDef.NSLCommissionSettlementDate != def.NSLCommissionSettlementDate ||
                                    invoiceDef.NSLCommissionRefNo != def.NSLCommissionSettlementRefNo)
                                {
                                    invoiceDef.NSLCommissionSettlementAmt = def.NSLCommissionSettlementAmount;
                                    invoiceDef.NSLCommissionSettlementDate = def.NSLCommissionSettlementDate;
                                    invoiceDef.NSLCommissionRefNo = def.NSLCommissionSettlementRefNo;

                                    if (def.NSLCommissionSettlementDate != DateTime.MinValue)
                                        invoiceDef.NSLCommissionSettlementExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.SellCurrency.CurrencyId, def.NSLCommissionSettlementDate);
                                    shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.AR_AP_MAINTENANCE, "Sales Commission Settlement Information Updated", GeneralCriteria.TRUE, userId));
                                }

                                shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.AR_AP_MAINTENANCE, amendmentList, userId);

                                if (orig_ARAmt != int.MinValue && orig_ARAmt != def.ARAmount)
                                    accountWorker.doAdjustmentAfterSettlement(invoiceDef.ShipmentId, 0, userId);
                                #endregion
                            }
                            else
                            {
                                #region split shipment
                                splitShipmentDef = orderSelectWorker.getSplitShipmentByPONo(def.InvoiceNo.Substring(0, def.InvoiceNo.IndexOf("-") - 1),
                                def.InvoiceNo.Substring(def.InvoiceNo.IndexOf("-") - 1, 1), Convert.ToInt32(def.InvoiceNo.Substring(def.InvoiceNo.IndexOf("-") + 1)));

                                splitShipmentDef.APAmt = def.APAmount;
                                splitShipmentDef.APDate = def.APDate;
                                splitShipmentDef.APRefNo = def.APRefNo;
                                splitShipmentDef.APExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.BuyCurrency.CurrencyId, def.APDate);

                                orderWorker.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipmentDef), userId);
                                shippingWorker.updateActionHistory(new ActionHistoryDef(splitShipmentDef.ShipmentId, splitShipmentDef.SplitShipmentId, ActionHistoryType.AR_AP_MAINTENANCE, "A/P Settlement Information Updated", GeneralCriteria.TRUE, userId));
                                accountWorker.doAdjustmentAfterSettlement(splitShipmentDef.ShipmentId, splitShipmentDef.SplitShipmentId, userId);

                                dailySunInterfaceDef = AccountManager.Instance.getDailySunInterface(def.ShipmentId, splitShipmentDef.SplitShipmentId, SunInterfaceTypeRef.Id.AccountPayable.GetHashCode());
                                if (dailySunInterfaceDef != null)
                                {
                                    if (splitShipmentDef.APAmt == 0 && splitShipmentDef.APDate == DateTime.MinValue && splitShipmentDef.APRefNo == "")
                                        resetActiveDailySunInterface(dailySunInterfaceDef);
                                    else
                                        setDailySunInterfaceActive(dailySunInterfaceDef);
                                }

                                splitShipmentList = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(splitShipmentDef.ShipmentId);
                                invoiceDef = shippingWorker.getInvoiceByKey(splitShipmentDef.ShipmentId);
                                invoiceDef.APAmt = 0;
                                invoiceDef.APDate = DateTime.MinValue;
                                invoiceDef.APRefNo = String.Empty;
                                max = DateTime.MinValue;
                                bool isSplitUpdated = true;
                                int splitCount = 0;

                                foreach (SplitShipmentDef splitShipment in splitShipmentList)
                                {
                                    if (splitShipment.IsVirtualSetSplit != 1)
                                    {
                                        splitCount++;
                                        invoiceDef.APAmt += splitShipment.APAmt;
                                        if (splitShipment.APDate == DateTime.MinValue)
                                        {
                                            isSplitUpdated = false;
                                            break;
                                        }
                                        else if (splitShipment.APDate > max)
                                        {
                                            max = splitShipment.APDate;
                                            invoiceDef.APDate = splitShipment.APDate;
                                            invoiceDef.APRefNo = splitShipment.APRefNo;
                                        }
                                    }
                                }
                                if (splitCount > 0 && isSplitUpdated)
                                {
                                    if (invoiceDef.APDate != DateTime.MinValue)
                                        invoiceDef.APExchangeRate = commonWorker.getExchangeRate(ExchangeRateType.PAYABLE_RECEIVABLE, def.BuyCurrency.CurrencyId, invoiceDef.APDate);
                                    shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.AR_AP_MAINTENANCE, amendmentList, userId);
                                }
                                shippingWorker.updateActionHistory(new ActionHistoryDef(splitShipmentDef.ShipmentId, splitShipmentDef.SplitShipmentId, ActionHistoryType.AR_AP_MAINTENANCE, "A/P Settlement Information Updated", GeneralCriteria.TRUE, userId));
                                #endregion
                            }
                        }
                        else if (dnDef != null)
                        {
                            #region UK Claim Debit Note
                            UKClaimDCNoteDef liveDNDef = UKClaimWorker.Instance.getUKClaimDCNoteByDCNoteNo(dnDef.DCNoteNo);
                            if ((liveDNDef.SettlementDate == DateTime.MinValue && dnDef.SettlementDate != DateTime.MinValue) || liveDNDef.SettlementDate != dnDef.SettlementDate)
                            {
                                string msg = "Next Claim Debit Note '" + dnDef.DCNoteNo + "' Settlement Date " + (liveDNDef.SettlementDate == DateTime.MinValue ? "NULL" : liveDNDef.SettlementDate.ToString("dd/MM/yyyy")) + " -> " + dnDef.SettlementDate.ToString("dd/MM/yyyy");
                                amendmentList.Add(new ActionHistoryDef(0, 0, ActionHistoryType.AR_AP_MAINTENANCE, msg, GeneralCriteria.TRUE, userId));
                                liveDNDef.SettlementDate = dnDef.SettlementDate;
                            }
                            UKClaimManager.Instance.updateUKClaimDCNote(liveDNDef, userId);
                            #endregion
                        }

                        foreach (ActionHistoryDef actionHistoryDef in amendmentList)
                            shippingWorker.updateActionHistory(actionHistoryDef);

                        ctx.VoteCommit();
                        isUpdated = true;
                    }
                    catch (Exception e)
                    {
                        retryCount++;
                        ctx.VoteRollback();
                        if (def != null)
                            NoticeHelper.sendErrorMessage(e, "AR / AP Update Failed - " + def.InvoiceNo);
                        else
                            NoticeHelper.sendErrorMessage(e, "DN settlement Update Failed - " + dnDef.DCNoteNo);
                    }
                    finally
                    {
                        ctx.Exit();
                    }
                }

                if (!isUpdated)
                {
                    if (def != null)
                        updateFailedList.Add(def.InvoiceNo);
                    else
                        if (dnDef != null)
                            updateFailedList.Add(dnDef.DCNoteNo);
                }
            }
            return updateFailedList;
        }

        public void createEInvoiceBatch(ArrayList invoiceList, int officeId, int userId)
        {
            TradingAgencyDef agency = null;
            EInvoiceBatchDef invBatchDef = null;
            EInvoiceBatchDef invBatchDef2 = null;
            TradingAgencyDef agency2 = null;
            UserRef user = null;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                OfficeRef officeRef = GeneralManager.Instance.getOfficeRefByKey(officeId);
                InvoiceDef invoiceDef = null;
                ArrayList amendmentList = new ArrayList();
                user = GeneralManager.Instance.getUserByKey(userId);

                ContractShipmentListJDef temp = (ContractShipmentListJDef)invoiceList[0];
                CustomerDef cust = temp.Customer;
                agency = temp.TradingAgency;
                invBatchDef = new EInvoiceBatchDef(officeRef.OfficeCode + cust.OPSKey + DateTime.Now.ToString("yyMMddHHmm"), user, DateTime.Today.Date);
                accountWorker.createEInvoiceBatch(invBatchDef, userId);


                foreach (ContractShipmentListJDef def in invoiceList)
                {
                    invoiceDef = shippingWorker.getInvoiceByKey(def.ShipmentId);

                    if (def.Customer.OPSKey != cust.OPSKey)
                    {
                        if (invBatchDef2 == null)
                        {
                            invBatchDef2 = new EInvoiceBatchDef(officeRef.OfficeCode + def.Customer.OPSKey + DateTime.Now.ToString("yyMMddHHmm"), user, DateTime.Today.Date);
                            accountWorker.createEInvoiceBatch(invBatchDef2, userId);
                            agency2 = def.TradingAgency;
                        }
                        invoiceDef.EInvoiceBatchId = invBatchDef2.EInvoiceBatchId;
                    }
                    else
                        invoiceDef.EInvoiceBatchId = invBatchDef.EInvoiceBatchId;

                    shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.E_INVOICE, amendmentList, userId);
                    amendmentList.Add(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.E_INVOICE, "Invoice Batch Generated : " + invBatchDef.EInvoiceBatchNo, GeneralCriteria.TRUE, userId));
                }

                foreach (ActionHistoryDef actionHistory in amendmentList)
                {
                    shippingWorker.updateActionHistory(actionHistory);
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

            ArrayList fileList = new ArrayList();
            fileList.Add(accountWorker.generateInvoiceBatchFile(invBatchDef, agency));

            if (invBatchDef2 != null)
                fileList.Add(accountWorker.generateInvoiceBatchFile(invBatchDef2, agency2));

            NoticeHelper.sendInvoiceBatchMail(invBatchDef, fileList, user);
        }

        public void removeFromEInvoiceBatch(ArrayList list, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                InvoiceDef invoiceDef = null;
                ArrayList amendmentList = new ArrayList();
                ArrayList fileList = new ArrayList();
                Hashtable generateList = new Hashtable();

                if (list != null)
                {
                    foreach (ContractShipmentListJDef def in list)
                    {
                        invoiceDef = shippingWorker.getInvoiceByKey(def.ShipmentId);

                        if (!generateList.ContainsKey(invoiceDef.EInvoiceBatchId))
                            generateList.Add(invoiceDef.EInvoiceBatchId, def.TradingAgency);

                        invoiceDef.EInvoiceBatchId = 0;

                        shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.E_INVOICE, null, userId);

                        amendmentList.Add(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.E_INVOICE, null, "Removed From Invoice Batch", userId));
                    }

                    foreach (ActionHistoryDef actionHistory in amendmentList)
                    {
                        shippingWorker.updateActionHistory(actionHistory);
                    }
                }

                EInvoiceBatchDef invoiceBatch = null;
                //generate invoice batch file !!!
                foreach (DictionaryEntry entry in generateList)
                {
                    invoiceBatch = accountWorker.getEInvoiceBatchByKey(Convert.ToInt32(entry.Key));
                    fileList.Add(accountWorker.generateInvoiceBatchFile(invoiceBatch, (TradingAgencyDef)entry.Value));
                }

                NoticeHelper.sendInvoiceBatchMail(invoiceBatch, fileList, GeneralManager.Instance.getUserByKey(userId));

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

        public void registerAR(ArrayList list, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList amendmentList = new ArrayList();
                InvoiceDef invoice = null;

                foreach (ContractShipmentListJDef def in list)
                {
                    invoice = shippingWorker.getInvoiceByKey(def.ShipmentId);
                    invoice.SalesScanDate = DateTime.Now;
                    invoice.SalesScanAmount = def.TotalShippedAmount;

                    shippingWorker.updateInvoice(invoice, ActionHistoryType.AR_AP_MAINTENANCE, amendmentList, userId);
                    amendmentList.Add(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.AR_AP_MAINTENANCE, null, "A/R REGISTERED", userId));
                }

                foreach (ActionHistoryDef actionHistory in amendmentList)
                {
                    shippingWorker.updateActionHistory(actionHistory);
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

        public void registerAP(ArrayList list, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                InvoiceDef invoiceDef = null;
                ShipmentDef shipmentDef = null;
                ArrayList splitShipmentList = null;
                DailySunInterfaceDef dailySunInterfaceDef = null;
                ArrayList amendmentList = new ArrayList();
                GeneralManager generalManager = GeneralManager.Instance;

                foreach (ContractShipmentListJDef def in list)
                {
                    invoiceDef = shippingWorker.getInvoiceByKey(def.ShipmentId);

                    invoiceDef.PurchaseScanDate = DateTime.Now;
                    invoiceDef.PurchaseScanBy = generalManager.getUserByKey(userId);
                    invoiceDef.AccountDocReceiptDate = DateTime.Now;

                    shipmentDef = orderSelectWorker.getShipmentByKey(def.ShipmentId);
                    invoiceDef.PurchaseScanAmount = shipmentDef.TotalShippedSupplierGarmentAmountAfterDiscount;

                    shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.AR_AP_MAINTENANCE, null, userId);
                    amendmentList.Add(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.AR_AP_MAINTENANCE, null, "A/P REGISTERED", userId));

                    dailySunInterfaceDef = AccountManager.Instance.getDailySunInterface(def.ShipmentId, 0, SunInterfaceTypeRef.Id.Purchase.GetHashCode());
                    setDailySunInterfaceActive(dailySunInterfaceDef);

                    if (shipmentDef.SplitCount > 0)
                    {
                        splitShipmentList = (ArrayList)orderSelectWorker.getSplitShipmentByShipmentId(def.ShipmentId);

                        foreach (SplitShipmentDef splitShipment in splitShipmentList)
                        {
                            if (splitShipment.IsVirtualSetSplit == 1)
                                continue;
                            splitShipment.AccountDocReceiptDate = DateTime.Now;
                            orderWorker.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipment), userId);

                            dailySunInterfaceDef = AccountManager.Instance.getDailySunInterface(def.ShipmentId, splitShipment.SplitShipmentId, SunInterfaceTypeRef.Id.Purchase.GetHashCode());
                            setDailySunInterfaceActive(dailySunInterfaceDef);
                        }
                    }
                }

                foreach (ActionHistoryDef actionHistory in amendmentList)
                {
                    shippingWorker.updateActionHistory(actionHistory);
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

        private void setDailySunInterfaceActive(DailySunInterfaceDef def)
        {
            if (def != null && def.ExtractedDate == DateTime.MinValue)
            {
                def.IsActive = true;
                accountWorker.updateDailySunInterface(def);
            }
        }

        public void changePurchaseAdjustmentCurrency(AdjustmentNoteDef def, int newCurrencyId, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                /*
                decimal oldEXRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.CurrencyId, def.IssueDate);
                decimal newExRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, newCurrencyId, def.IssueDate);
                def.Amount = Math.Round((def.Amount * oldEXRate / newExRate), 2);
                */

                def.RevisedCurrencyId = newCurrencyId;
                accountWorker.updateAdjustmentNote(def, userId);

                AdjustmentDetailDef detailDef = AccountWorker.Instance.getAdjustmentDetailByAdjustmentNoteId(def.AdjustmentNoteId);
                detailDef.IsInterfaced = false;
                accountWorker.updateAdjustmentDetail(detailDef, userId);

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
                queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.APAdjustment_ChangeCurrency.GetHashCode();
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


        public ReportClass generateAdjustmentNote(int officeGroupId, int adjustmentTypeId, bool isDraft, DateTime issueDate, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                ArrayList list = accountWorker.getOutstandingAdjustmentNoteList(officeGroupId, adjustmentTypeId);
                ARAdjustmentNoteDs arDataSet = new ARAdjustmentNoteDs();
                APAdjustmentNoteDs apDataSet = new APAdjustmentNoteDs();
                int shipmentId = -1;
                int splitShipmentId = -1;
                int customerId = -1;
                int i;

                foreach (AdjustmentNoteDef def in list)
                {
                    i = def.AdjustmentNoteNo.IndexOf('|');
                    if (def.AdjustmentType.Id == AdjustmentType.SALES_ADJUSTMENT.Id)
                        customerId = Math.Abs(int.Parse(def.AdjustmentNoteNo.Substring(0, i)));
                    else
                    {
                        shipmentId = int.Parse(def.AdjustmentNoteNo.Substring(0, i));
                        splitShipmentId = int.Parse(def.AdjustmentNoteNo.Substring(i + 1));
                    }

                    AdjustmentNoteDef.fillPartyInfo(def, customerId);

                    if (!isDraft)
                    {
                        def.IssueDate = issueDate;
                        def.AdjustmentNoteNo = accountWorker.fillNextAdjustmentNoteNo(AdjustmentType.getType(adjustmentTypeId), def.IssueDate, def.OfficeId, def.DebitCreditIndicator);
                    }
                    else
                        def.AdjustmentNoteNo = "N/A";
                    def.AdjustmentNoteId = -1;
                    accountWorker.updateAdjustmentNote(def, userId);
                    ArrayList detailList = accountWorker.getOutstandingAdjustmentDetailList(officeGroupId, adjustmentTypeId, def.VendorId, def.CurrencyId, shipmentId, splitShipmentId);
                    foreach (AdjustmentDetailDef detailDef in detailList)
                    {
                        detailDef.AdjustmentNoteId = def.AdjustmentNoteId;
                        accountWorker.updateAdjustmentDetail(detailDef, userId);
                        if (!isDraft)
                        {
                            if (def.AdjustmentType.Id == AdjustmentType.SALES_ADJUSTMENT.Id)
                                shippingWorker.updateActionHistory(new ActionHistoryDef(detailDef.ShipmentId, detailDef.SplitShipmentId, ActionHistoryType.AR_ADJUSTMENT, def.AdjustmentType.Name + " Settled [#" + def.AdjustmentNoteNo + "]", GeneralCriteria.TRUE, userId));
                            else if (def.AdjustmentType.Id == AdjustmentType.PURCHASE_ADJUSTMENT.Id)
                                shippingWorker.updateActionHistory(new ActionHistoryDef(detailDef.ShipmentId, detailDef.SplitShipmentId, ActionHistoryType.AP_ADJUSTMENT, def.AdjustmentType.Name + " Settled [#" + def.AdjustmentNoteNo + "]", GeneralCriteria.TRUE, userId));
                        }
                    }

                    if (def.AdjustmentType.Id == AdjustmentType.SALES_ADJUSTMENT.Id)
                    {
                        ARAdjustmentNoteDs ds = ReporterWorker.Instance.getARAdjustmentList(def.AdjustmentNoteId);
                        foreach (ARAdjustmentNoteDs.ARAdjustmentNoteRow r in ds.ARAdjustmentNote)
                        {
                            ARAdjustmentNoteDs.ARAdjustmentNoteRow newRow = arDataSet.ARAdjustmentNote.NewARAdjustmentNoteRow();
                            newRow.AdjustmentNoteId = r.AdjustmentNoteId;
                            newRow.AdjustmentDetailId = r.AdjustmentDetailId;
                            newRow.ShipmentId = r.ShipmentId;
                            newRow.SplitShipmentId = r.SplitShipmentId;
                            newRow.AdjustmentNoteNo = r.AdjustmentNoteNo;
                            newRow.ContractNo = r.ContractNo;
                            newRow.Currency = r.Currency;
                            newRow.DebitCreditIndicator = r.DebitCreditIndicator;
                            newRow.DeliveryNo = r.DeliveryNo;
                            newRow.InvoiceDate = r.InvoiceDate;
                            newRow.InvoiceNo = r.InvoiceNo;
                            newRow.IssueDate = r.IssueDate;
                            newRow.ItemNo = r.ItemNo;
                            newRow.MasterDebitCreditIndicator = r.MasterDebitCreditIndicator;
                            newRow.NSLAmt = r.NSLAmt;
                            newRow.NUKAmt = r.NUKAmt;
                            newRow.OrderRef = r.OrderRef;
                            newRow.PartyName = r.PartyName;
                            newRow.PartyAddress1 = r.IsPartyAddress1Null() ? String.Empty : r.PartyAddress1;
                            newRow.PartyAddress2 = r.IsPartyAddress2Null() ? String.Empty : r.PartyAddress2;
                            newRow.PartyAddress3 = r.IsPartyAddress3Null() ? String.Empty : r.PartyAddress3;
                            newRow.PartyAddress4 = r.IsPartyAddress4Null() ? String.Empty : r.PartyAddress4;
                            arDataSet.ARAdjustmentNote.AddARAdjustmentNoteRow(newRow);
                        }
                    }
                    else
                    {
                        APAdjustmentNoteDs ds = ReporterWorker.Instance.getAPAdjustmentList(def.AdjustmentNoteId);
                        foreach (APAdjustmentNoteDs.APAdjustmentNoteRow r in ds.APAdjustmentNote)
                        {
                            APAdjustmentNoteDs.APAdjustmentNoteRow newRow = apDataSet.APAdjustmentNote.NewAPAdjustmentNoteRow();
                            newRow.AdjustmentNoteId = r.AdjustmentNoteId;
                            newRow.AdjustmentDetailId = r.AdjustmentDetailId;
                            newRow.ShipmentId = r.ShipmentId;
                            newRow.SplitShipmentId = r.SplitShipmentId;
                            newRow.AdjustmentNoteNo = r.AdjustmentNoteNo;
                            newRow.ContractNo = r.ContractNo;
                            newRow.CurrencyId = r.CurrencyId;
                            newRow.CurrencyCode = CurrencyId.getName(r.CurrencyId);
                            newRow.OfficeId = r.OfficeId;
                            newRow.TradingAgencyId = r.TradingAgencyId;
                            newRow.DebitCreditIndicator = r.DebitCreditIndicator;
                            newRow.DeliveryNo = r.DeliveryNo;
                            newRow.InvoiceDate = r.InvoiceDate;
                            newRow.InvoiceNo = r.InvoiceNo;
                            newRow.IssueDate = r.IssueDate;
                            newRow.ItemNo = r.ItemNo;
                            newRow.MasterDebitCreditIndicator = r.MasterDebitCreditIndicator;
                            newRow.RevisedAmt = r.RevisedAmt;
                            newRow.Amt = r.Amt;
                            newRow.PackingUnit = r.PackingUnit;
                            newRow.PartyName = r.PartyName;
                            newRow.PartyAddress1 = r.IsPartyAddress1Null() ? String.Empty : r.PartyAddress1;
                            newRow.PartyAddress2 = r.IsPartyAddress2Null() ? String.Empty : r.PartyAddress2;
                            newRow.PartyAddress3 = r.IsPartyAddress3Null() ? String.Empty : r.PartyAddress3;
                            newRow.PartyAddress4 = r.IsPartyAddress4Null() ? String.Empty : r.PartyAddress4;
                            newRow.Size = r.Size;
                            newRow.SellingPrice = r.SellingPrice;
                            newRow.NetFOBPrice = r.NetFOBPrice;
                            newRow.SupplierGmtPrice = r.SupplierGmtPrice;
                            newRow.ShippedQty = r.ShippedQty;
                            newRow.RevisedSellingPrice = r.RevisedSellingPrice;
                            newRow.RevisedNetFOBPrice = r.RevisedNetFOBPrice;
                            newRow.RevisedSupplierGmtPrice = r.RevisedSupplierGmtPrice;
                            newRow.RevisedShippedQty = r.RevisedShippedQty;
                            newRow.QACommissionPercent = r.QACommissionPercent;
                            newRow.QACommissionAmt = r.QACommissionAmt;
                            newRow.VendorPaymentDiscountPercent = r.VendorPaymentDiscountPercent;
                            newRow.VendorPaymentDiscountAmt = r.VendorPaymentDiscountAmt;
                            newRow.LabTestIncome = r.LabTestIncome;
                            apDataSet.APAdjustmentNote.AddAPAdjustmentNoteRow(newRow);
                        }
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
                    queueDef.SourceId = 1;
                    queueDef.SubmitTime = DateTime.Now;
                    if (adjustmentTypeId == AdjustmentType.PURCHASE_ADJUSTMENT.Id)
                        queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.APAdjustment.GetHashCode();
                    else
                        queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.ARAdjustmentForEziBuy.GetHashCode();
                    queueDef.User = generalWorker.getUserByKey(userId);
                    accountWorker.updateSunInterfaceQueue(queueDef);
                }

                ReportClass report = null;

                if (adjustmentTypeId == AdjustmentType.SALES_ADJUSTMENT.Id)
                {
                    report = new ARAdjustmentNote();
                    report.SetDataSource(arDataSet);
                }
                else
                {
                    report = new APAdjustmentNote();
                    report.SetDataSource(apDataSet);
                }

                report.SetParameterValue("IsDraft", (isDraft ? "Y" : "N"));

                if (isDraft)
                    ctx.VoteRollback();
                else
                {
                    ctx.VoteCommit();
                }
                return report;
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

        public List<GenericDataSummaryDef> getOutstandingAdvancePaymentByCurrency(int vendorId, int officeId)
        {
            return advancePaymentWorker.getOutstandingAdvancePaymentByCurrency(vendorId, officeId);
        }

        public ArrayList getAdjustmentNoteList(int reportOfficeGroupId, int adjustmentTypeId, int dateType, int year, int periodMonth)
        {
            DateTime fromDate;
            DateTime toDate;
            if (dateType == 1)
            {
                fromDate = new DateTime(year, periodMonth, 1);
                toDate = (new DateTime(year, periodMonth, 1)).AddMonths(1);
            }
            else
            {
                AccountFinancialCalenderDef def = generalWorker.getAccountPeriodByYearPeriod(AppId.NSS.Code, year, periodMonth);
                fromDate = def.StartDate;
                toDate = def.EndDate.AddDays(1);
            }

            TypeCollector officeList = TypeCollector.createNew(reportOfficeGroupId);
            ArrayList olist = commonWorker.getOfficeListByReportOfficeGroupId(reportOfficeGroupId);
            foreach (OfficeRef oRef in olist)
            {
                officeList.append(oRef.OfficeId);
            }

            return accountWorker.getAdjustmentNoteList(officeList, adjustmentTypeId, fromDate, toDate);
        }

        public void generateMockShopDebitNote(int officeId, int fiscalYear, int period, DateTime debitNoteDate, bool isDraft, int userId,
            out ArrayList mockShopDCNoteList, out ArrayList mockShopDCNoteShipmentList, out ArrayList mockShopDCShipmentDetailList)
        {
            mockShopDCNoteList = new ArrayList();
            mockShopDCShipmentDetailList = new ArrayList();
            mockShopDCNoteShipmentList = accountWorker.getOutstandingMockShopShipmentList(officeId, fiscalYear, period);

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                if (mockShopDCNoteShipmentList.Count == 0)
                    return;

                string tempKey = "";
                Hashtable debitNoteList = new Hashtable();

                foreach (MockShopDCNoteShipmentDef def in mockShopDCNoteShipmentList)
                {
                    tempKey = accountWorker.getDebitNotePrefix(officeId, def.TradingAgencyId, fiscalYear, period) + def.SellCurrencyId.ToString();
                    MockShopDCNoteDef noteDef;
                    if (!debitNoteList.ContainsKey(tempKey))
                    {
                        noteDef = new MockShopDCNoteDef();
                        noteDef.DCNoteIndicator = "D";
                        noteDef.DCNoteDate = debitNoteDate;
                        noteDef.FiscalYear = fiscalYear;
                        noteDef.Period = period;
                        noteDef.SellCurrencyId = def.SellCurrencyId;
                        noteDef.TotalCourierCharge = 0;
                        noteDef.TotalShippedAmt = 0;
                        noteDef.TotalNSLCommissionAmt = 0;
                        noteDef.IsInterfaced = false;
                        noteDef.OfficeId = def.OfficeId;

                        noteDef.Status = 1;

                        debitNoteList.Add(tempKey, noteDef);
                        mockShopDCNoteList.Add(noteDef);

                        if (isDraft)
                        {
                            noteDef.DCNoteNo = tempKey + debitNoteList.Count.ToString();
                            noteDef.DCNoteId = debitNoteList.Count;
                        }
                        else
                        {
                            accountWorker.updateMockShopDCNote(noteDef, officeId, def.TradingAgencyId, userId);
                        }
                    }

                    noteDef = (MockShopDCNoteDef)debitNoteList[tempKey];
                    def.DCNoteId = noteDef.DCNoteId;
                    noteDef.TotalNSLCommissionAmt += def.NSLCommissionAmt;
                    noteDef.TotalShippedAmt += def.TotalShippedAmount;
                    noteDef.TotalCourierCharge += def.TotalCourierCharge;

                    if (!isDraft)
                    {
                        //assign DCNoteShipmentId & save
                        accountWorker.updateMockShopDCNoteShipment(def, userId);
                    }

                    ArrayList list = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(def.ShipmentId);
                    MockShopDCNoteShipmentDetailDef detailDef = null;
                    foreach (ShipmentDetailDef shipmentDetailDef in list)
                    {
                        detailDef = new MockShopDCNoteShipmentDetailDef();
                        detailDef.DCNoteShipmentDetailId = -1;
                        detailDef.DCNoteShipmentId = def.DCNoteShipmentId;
                        detailDef.ShipmentId = def.ShipmentId;
                        detailDef.ShipmentDetailId = shipmentDetailDef.ShipmentDetailId;
                        detailDef.SizeOptionId = shipmentDetailDef.SizeOption.SizeOptionId;
                        detailDef.SellingPrice = shipmentDetailDef.SellingPrice;
                        detailDef.ShippedQty = shipmentDetailDef.ShippedQuantity;
                        detailDef.Status = 1;

                        mockShopDCShipmentDetailList.Add(detailDef);

                        if (!isDraft)
                            accountWorker.updateMockShopDCNoteShipmentDetail(detailDef);
                    }
                }

                if (!isDraft)
                {
                    foreach (MockShopDCNoteDef noteDef in debitNoteList.Values)
                    {
                        accountWorker.updateMockShopDCNote(noteDef, noteDef.OfficeId, -1, userId);
                    }

                    submitSunInterfaceRequestsForMockShop(officeId, fiscalYear, period, userId);

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

        public void generateStudioDebitNote(int officeId, int fiscalYear, int period, DateTime debitNoteDate, bool isDraft, int userId,
            out ArrayList studioDCNoteList, out ArrayList studioDCNoteShipmentList, out ArrayList studioDCShipmentDetailList)
        {
            studioDCNoteList = new ArrayList();
            studioDCShipmentDetailList = new ArrayList();
            studioDCNoteShipmentList = accountWorker.getOutstandingStudioShipmentList(officeId, fiscalYear, period);

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                if (studioDCNoteShipmentList.Count == 0)
                    return;

                string tempKey = "";
                Hashtable debitNoteList = new Hashtable();

                foreach (StudioDCNoteShipmentDef def in studioDCNoteShipmentList)
                {
                    tempKey = accountWorker.getDebitNotePrefix(officeId, def.TradingAgencyId, fiscalYear, period) + def.SellCurrencyId.ToString();
                    StudioDCNoteDef noteDef;
                    if (!debitNoteList.ContainsKey(tempKey))
                    {
                        noteDef = new StudioDCNoteDef();
                        noteDef.DCNoteIndicator = "D";
                        noteDef.DCNoteDate = debitNoteDate;
                        noteDef.FiscalYear = fiscalYear;
                        noteDef.Period = period;
                        noteDef.SellCurrencyId = def.SellCurrencyId;
                        noteDef.TotalCourierCharge = 0;
                        noteDef.TotalShippedAmt = 0;
                        noteDef.TotalNSLCommissionAmt = 0;
                        noteDef.IsInterfaced = false;
                        noteDef.OfficeId = def.OfficeId;

                        noteDef.Status = 1;

                        debitNoteList.Add(tempKey, noteDef);
                        studioDCNoteList.Add(noteDef);

                        if (isDraft)
                        {
                            noteDef.DCNoteNo = tempKey + debitNoteList.Count.ToString();
                            noteDef.DCNoteId = debitNoteList.Count;
                        }
                        else
                        {
                            accountWorker.updateStudioDCNote(noteDef, officeId, def.TradingAgencyId, userId);
                        }
                    }

                    noteDef = (StudioDCNoteDef)debitNoteList[tempKey];
                    def.DCNoteId = noteDef.DCNoteId;
                    noteDef.TotalNSLCommissionAmt += def.NSLCommissionAmt;
                    noteDef.TotalShippedAmt += def.TotalShippedAmount;
                    noteDef.TotalCourierCharge += def.TotalCourierCharge;

                    if (!isDraft)
                    {
                        //assign DCNoteShipmentId & save
                        accountWorker.updateStudioDCNoteShipment(def, userId);
                    }

                    ArrayList list = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(def.ShipmentId);
                    StudioDCNoteShipmentDetailDef detailDef = null;
                    foreach (ShipmentDetailDef shipmentDetailDef in list)
                    {
                        detailDef = new StudioDCNoteShipmentDetailDef();
                        detailDef.DCNoteShipmentDetailId = -1;
                        detailDef.DCNoteShipmentId = def.DCNoteShipmentId;
                        detailDef.ShipmentId = def.ShipmentId;
                        detailDef.ShipmentDetailId = shipmentDetailDef.ShipmentDetailId;
                        detailDef.SizeOptionId = shipmentDetailDef.SizeOption.SizeOptionId;
                        detailDef.SellingPrice = shipmentDetailDef.SellingPrice;
                        detailDef.ShippedQty = shipmentDetailDef.ShippedQuantity;
                        detailDef.Status = 1;

                        studioDCShipmentDetailList.Add(detailDef);

                        if (!isDraft)
                            accountWorker.updateStudioDCNoteShipmentDetail(detailDef);
                    }
                }

                if (!isDraft)
                {
                    foreach (StudioDCNoteDef noteDef in debitNoteList.Values)
                    {
                        accountWorker.updateStudioDCNote(noteDef, noteDef.OfficeId, -1, userId);
                    }

                    submitSunInterfaceRequestsForStudioSample(officeId, fiscalYear, period, userId);

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

        public void generateUTDebitNote(int officeId, int fiscalYear, int period, DateTime debitNoteDate, bool isDraft, int userId,
            out ArrayList UTDCNoteList, out ArrayList UTDCNoteShipmentList, out ArrayList qaDCNoteList, out ArrayList qaDCNoteShipmentList)
        {

            UTDCNoteList = new ArrayList();
            UTDCNoteShipmentList = accountWorker.getOutstandingUTShipmentList(officeId, fiscalYear, period);

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                qaDCNoteList = new ArrayList();
                qaDCNoteShipmentList = new ArrayList();

                if (UTDCNoteShipmentList.Count == 0)
                    return;

                string tempKey = string.Empty;
                Hashtable debitNoteList = new Hashtable();

                foreach (UTContractDCNoteShipmentDef def in UTDCNoteShipmentList)
                {
                    tempKey = accountWorker.getDebitNotePrefix(officeId, 0, fiscalYear, period) + def.SellCurrencyId.ToString();

                    UTContractDCNoteDef noteDef;
                    if (!debitNoteList.ContainsKey(tempKey))
                    {
                        noteDef = new UTContractDCNoteDef();
                        noteDef.DCNoteIndicator = "D";
                        noteDef.DCNoteDate = debitNoteDate;
                        noteDef.FiscalYear = fiscalYear;
                        noteDef.Period = period;
                        noteDef.SellCurrencyId = def.SellCurrencyId;
                        noteDef.TotalCommission = 0;
                        noteDef.TotalMargin = 0;
                        noteDef.TotalServiceFee = 0;
                        noteDef.TotalSupplierCommission = 0;
                        noteDef.TotalSupplierGmtAmt = 0;
                        noteDef.IsInterfaced = false;
                        noteDef.OfficeId = def.OfficeId;
                        noteDef.Status = 1;

                        debitNoteList.Add(tempKey, noteDef);
                        UTDCNoteList.Add(noteDef);

                        accountWorker.updateUTContractDCNote(noteDef, officeId, userId);
                    }

                    noteDef = (UTContractDCNoteDef)debitNoteList[tempKey];
                    def.DCNoteId = noteDef.DCNoteId;
                    noteDef.TotalCommission += def.Commission;
                    noteDef.TotalMargin += def.Margin;
                    noteDef.TotalServiceFee += def.ServiceFee;
                    noteDef.TotalSupplierGmtAmt += def.SupplierGmtAmt;
                    noteDef.TotalSupplierCommission += def.SupplierCommission;

                    accountWorker.updateUTContractDCNoteShipment(def, userId);
                }

                foreach (UTContractDCNoteDef noteDef in debitNoteList.Values)
                {
                    accountWorker.updateUTContractDCNote(noteDef, noteDef.OfficeId, userId);
                }


                qaDCNoteList = new ArrayList();
                qaDCNoteShipmentList = accountWorker.getOutstandingQACommissionDNShipmentList(officeId, fiscalYear, period);

                string tempVendorKey = "";
                Hashtable debitNoteListQA = new Hashtable();
                foreach (QACommissionDNShipmentDef def in qaDCNoteShipmentList)
                {
                    tempVendorKey = def.VendorId.ToString();
                    QACommissionDNDef noteDef;
                    if (!debitNoteListQA.ContainsKey(tempVendorKey))
                    {
                        noteDef = new QACommissionDNDef();
                        noteDef.DNDate = debitNoteDate;
                        noteDef.FiscalYear = fiscalYear;
                        noteDef.Period = period;
                        noteDef.VendorId = def.VendorId;
                        noteDef.CurrencyId = def.CurrencyId;
                        noteDef.TotalQACommission = 0;
                        noteDef.IsInterfaced = false;
                        noteDef.Status = 1;
                        ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(def.ShipmentId);
                        ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
                        noteDef.OfficeId = contractDef.Office.OfficeId;

                        string dbnoteKey = accountWorker.getDebitNotePrefix(officeId, 0, fiscalYear, period) + def.CurrencyId.ToString();
                        debitNoteListQA.Add(tempVendorKey, noteDef);
                        qaDCNoteList.Add(noteDef);

                        accountWorker.updateQACommissionDN(noteDef, officeId, userId);
                    }

                    noteDef = (QACommissionDNDef)debitNoteListQA[tempVendorKey];
                    def.DNId = noteDef.DNId;
                    noteDef.TotalQACommission += def.QACommissionAmount;

                    accountWorker.updateQACommissionDNShipment(def, userId);
                }

                foreach (QACommissionDNDef noteDef in debitNoteListQA.Values)
                {
                    accountWorker.updateQACommissionDN(noteDef, noteDef.OfficeId, userId);
                }

                if (isDraft)
                    ctx.VoteRollback();
                else
                {
                    SunInterfaceQueueDef queueDef = new SunInterfaceQueueDef();
                    queueDef.QueueId = -1;
                    queueDef.OfficeGroup = commonWorker.getReportOfficeGroupByKey(officeId);
                    queueDef.FiscalYear = fiscalYear;
                    queueDef.Period = period;
                    queueDef.PurchaseTerm = 0;
                    queueDef.UTurn = 0;
                    queueDef.CategoryType = CategoryType.ACTUAL;
                    queueDef.SourceId = 2;
                    queueDef.SubmitTime = DateTime.Now;
                    queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.QACommission.GetHashCode();
                    queueDef.User = generalWorker.getUserByKey(userId);
                    accountWorker.updateSunInterfaceQueue(queueDef);

                    queueDef = (SunInterfaceQueueDef)queueDef.Clone();
                    queueDef.QueueId = -1;
                    queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.UTContractDN.GetHashCode();
                    accountWorker.updateSunInterfaceQueue(queueDef);

                    ctx.VoteCommit();
                }
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


        public void generateILSDiffDebitNote(int officeId, int fiscalYear, int period, DateTime debitNoteDate, bool isDraft, int userId,

            out ArrayList ilsDiffDCNoteList, out ArrayList ilsDiffDCNoteShipmentList)
        {
            ilsDiffDCNoteList = new ArrayList();
            ilsDiffDCNoteShipmentList = accountWorker.getOutstandingILSDiffShipmentList(officeId, fiscalYear, period);

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                if (ilsDiffDCNoteShipmentList.Count == 0)
                    return;

                string tempKey = "";
                Hashtable debitNoteList = new Hashtable();

                foreach (ILSDiffDCNoteShipmentDef def in ilsDiffDCNoteShipmentList)
                {
                    tempKey = accountWorker.getDebitNotePrefix(officeId, 0, fiscalYear, period) + def.CurrencyId.ToString();
                    ILSDiffDCNoteDef noteDef;
                    if (!debitNoteList.ContainsKey(tempKey))
                    {
                        noteDef = new ILSDiffDCNoteDef();
                        noteDef.DCNoteIndicator = "D";
                        noteDef.DCNoteDate = debitNoteDate;
                        noteDef.OfficeId = def.OfficeId;
                        noteDef.FiscalYear = fiscalYear;
                        noteDef.Period = period;
                        noteDef.CurrencyId = def.CurrencyId;
                        noteDef.TotalDiffAmt = 0;
                        noteDef.TotalSalesDiffAmt = 0;
                        noteDef.TotalSalesCommissionDiffAmt = 0;
                        noteDef.IsInterfaced = false;

                        noteDef.Status = 1;

                        debitNoteList.Add(tempKey, noteDef);
                        ilsDiffDCNoteList.Add(noteDef);

                        if (isDraft)
                        {
                            noteDef.DCNoteNo = tempKey + debitNoteList.Count.ToString();
                            noteDef.DCNoteId = debitNoteList.Count;
                        }
                        else
                        {
                            accountWorker.updateILSDiffDCNote(noteDef, officeId, userId);
                        }
                    }

                    noteDef = (ILSDiffDCNoteDef)debitNoteList[tempKey];
                    def.DCNoteId = noteDef.DCNoteId;
                    if (def.ILSType == 1)
                    {
                        noteDef.TotalSalesDiffAmt += def.ILSDiffAmt;
                        noteDef.TotalDiffAmt += def.ILSDiffAmt;
                    }
                    else if (def.ILSType == 2)
                    {
                        noteDef.TotalSalesCommissionDiffAmt += def.ILSDiffAmt;
                        noteDef.TotalDiffAmt += def.ILSDiffAmt;
                    }

                    if (noteDef.TotalDiffAmt < 0)
                    {
                        noteDef.DCNoteIndicator = "C";
                    }

                    if (!isDraft)
                    {
                        //assign DCNoteShipmentId & save
                        accountWorker.updateILSDiffDCNoteShipment(def, userId);
                    }
                }

                if (!isDraft)
                {
                    foreach (ILSDiffDCNoteDef noteDef in debitNoteList.Values)
                    {
                        accountWorker.updateILSDiffDCNote(noteDef, noteDef.OfficeId, userId);
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

        private void submitSunInterfaceRequestsForMockShop(int officeId, int fiscalYear, int period, int userId)
        {
            SunInterfaceQueueDef queueDef = new SunInterfaceQueueDef();
            queueDef.QueueId = -1;
            queueDef.OfficeGroup = commonWorker.getReportOfficeGroupByKey(officeId);
            queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.MockShopSales.GetHashCode();
            queueDef.CategoryType = CategoryType.ACTUAL;
            queueDef.User = generalWorker.getUserByKey(userId);
            queueDef.SourceId = 2;
            queueDef.SubmitTime = DateTime.Now;
            queueDef.FiscalYear = fiscalYear;
            queueDef.Period = period;
            queueDef.PurchaseTerm = 0;
            queueDef.UTurn = 0;
            submitSunInterfaceRequest(queueDef);

            queueDef = (SunInterfaceQueueDef)queueDef.Clone(); 
            queueDef.QueueId = -1;
            queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.MockShopSalesCommission.GetHashCode();
            submitSunInterfaceRequest(queueDef);

            queueDef = (SunInterfaceQueueDef)queueDef.Clone();
            queueDef.QueueId = -1;
            queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.MockShop.GetHashCode();
            submitSunInterfaceRequest(queueDef);
        }

        private void submitSunInterfaceRequestsForStudioSample(int officeId, int fiscalYear, int period, int userId)
        {
            SunInterfaceQueueDef queueDef = new SunInterfaceQueueDef();
            queueDef.QueueId = -1;
            queueDef.OfficeGroup = commonWorker.getReportOfficeGroupByKey(officeId);
            queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.StudioSampleSales.GetHashCode();
            queueDef.CategoryType = CategoryType.ACTUAL;
            queueDef.User = generalWorker.getUserByKey(userId);
            queueDef.SourceId = 2;
            queueDef.SubmitTime = DateTime.Now;
            queueDef.FiscalYear = fiscalYear;
            queueDef.Period = period;
            queueDef.PurchaseTerm = 0;
            queueDef.UTurn = 0;
            submitSunInterfaceRequest(queueDef);

            queueDef = (SunInterfaceQueueDef)queueDef.Clone();
            queueDef.QueueId = -1;
            queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.StudioSampleSalesCommission.GetHashCode();
            submitSunInterfaceRequest(queueDef);

            queueDef = (SunInterfaceQueueDef)queueDef.Clone();
            queueDef.QueueId = -1;
            queueDef.SunInterfaceTypeId = SunInterfaceTypeRef.Id.StudioSample.GetHashCode();
            submitSunInterfaceRequest(queueDef);
        }

        public void sendPurchaseDCNoteToSupplier(AdjustmentNoteDef adjustmentNote, string filePath, int userId)
        {
            try
            {
                AdjustmentNoteDef def = accountWorker.GetAdjustmentNoteByKey(adjustmentNote.AdjustmentNoteId);
                if (def.MailStatus == 0)
                {
                    string emailAddr = string.Empty;
                    VendorRef vendor = VendorWorker.Instance.getVendorByKey(adjustmentNote.VendorId);
                    if (vendor.eAdviceAddr != null)
                        emailAddr = vendor.eAdviceAddr.Trim();
                    if (!string.IsNullOrEmpty(emailAddr))
                    {
                        NoticeHelper.sendPurchaseDCNote(adjustmentNote.OfficeId, vendor, adjustmentNote.DebitCreditIndicator, adjustmentNote.AdjustmentNoteNo, filePath, userId);
                        adjustmentNote.MailStatus = 1;
                        accountWorker.updateAdjustmentNote(adjustmentNote, userId);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Vendor Email NOT DEFINED");
                        NoticeHelper.sendPurchaseDCNoteMissingSupplierEmail(adjustmentNote.OfficeId, vendor, adjustmentNote.DebitCreditIndicator, adjustmentNote.AdjustmentNoteNo, filePath, userId);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public void sendGenericDCNoteToParty(GenericDCNoteDef def, string filePath, string partyName, string email, int userId)
        {
            try
            {
                if (def.MailStatus == 0)
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        NoticeHelper.sendGenericDCNoteReportToSupplier(def.OfficeId, partyName, email, def.DebitCreditIndicator, def.DCNoteNo, filePath, userId);
                        def.MailStatus = 1;
                        string s = string.Empty;
                        accountWorker.updateGenericDCNote(def, userId, out s);
                    }
                    else
                    {
                        throw new ApplicationException("Error sending generic d/c note to party - empty email address");
                    }
                    /*
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Email Address for Generic D/C Note NOT DEFINED");
                        NoticeHelper.sendGenericDCNoteMissingSupplierEmail(def.OfficeId, vendor, def.DebitCreditIndicator, def.DCNoteNo, filePath, userId);
                    }
                    */
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public void sendAdvancePaymentInterestDCNoteToSupplier(int paymentId, DateTime paymentDate, string filePath, int userId)
        {
            try
            {
                AdvancePaymentInstalmentDetailDef detailDef = advancePaymentWorker.getAdvancePaymentInstalmentDetailByKey(paymentId, paymentDate);
                AdvancePaymentDef def = advancePaymentWorker.getAdvancePaymentByKey(paymentId);

                if (detailDef.MailStatus == 0)
                {
                    string emailAddr = string.Empty;
                    VendorRef vendor = VendorWorker.Instance.getVendorByKey(def.Vendor.VendorId);
                    if (vendor.eAdviceAddr != null)
                        emailAddr = vendor.eAdviceAddr.Trim();
                    if (!string.IsNullOrEmpty(emailAddr))
                    {
                        NoticeHelper.sendAdvancePaymentInterestDCNoteReport(def.OfficeId, vendor, detailDef.InterestAmt >= 0 ? "D" : "C", detailDef.DCNoteNo, filePath, userId);
                        detailDef.MailStatus = 1;
                        advancePaymentWorker.updateAdvancePaymentInstalmentDetail(detailDef, userId);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Vendor Email NOT DEFINED");
                        NoticeHelper.sendAdvancePaymentInterestDCNoteMissingSupplierEmail(def.OfficeId, vendor, detailDef.InterestAmt >= 0 ? "D" : "C", detailDef.DCNoteNo, filePath, userId);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        /*
        public void getMockShopDebitNote(int dcNoteId, out ArrayList mockShopDCNoteList, 
            out ArrayList mockShopDCNoteShipmentList, out ArrayList mockShopDCShipmentDetailList)
        {
            mockShopDCNoteList = null;
            mockShopDCNoteShipmentList = null;
            mockShopDCShipmentDetailList = null;

            AccountWorker accountWorker = AccountWorker.Instance;

            mockShopDCNoteList = ConvertUtility.createArrayList(accountWorker.getMockShopDCNoteByKey(dcNoteId));
            mockShopDCNoteShipmentList = accountWorker.getMockShopDCNoteShipmentByDCNoteId(dcNoteId);
            mockShopDCShipmentDetailList = accountWorker.getMockShopDCNoteShipmentDetailByDCNoteId(dcNoteId);
        }
        */        

        public string getMockShopDebitNoteNoByShipmentId(int shipmentId)
        {
            MockShopDCNoteDef def = accountWorker.getMockShopDCNoteByShipmentId(shipmentId);
            if (def == null)
                return String.Empty;
            else
                return def.DCNoteNo;
        }

        public string getStudioSampleDebitNoteNoByShipmentId(int shipmentId)
        {
            StudioDCNoteDef def = accountWorker.getStudioDCNoteByShipmentId(shipmentId);
            if (def == null)
                return String.Empty;
            else
                return def.DCNoteNo;
        }

        public bool isReleaseLockOpened(int shipmentId)
        {
            return accountWorker.isReleaseLockOpened(shipmentId);
        }

        public ArrayList getPaymentStatusEnquiryList(string invoicePrefix, int invoiceSeqFrom, int invoiceSeqTo, int invoiceYear, int officeId, int handlingOfficeId, DateTime invoiceDateFrom, DateTime invoiceDateTo, DateTime subDocDateFrom,
            DateTime subDocDateTo, DateTime interfaceDateFrom, DateTime interfaceDateTo, int paymentTermId, int tradingAgencyId, int vendorId,
            TypeCollector paymentStatusList, string contractNo)
        {
            return accountWorker.getPaymentStatusEnquiryList(invoicePrefix, invoiceSeqFrom, invoiceSeqTo, invoiceYear, officeId, handlingOfficeId, invoiceDateFrom, invoiceDateTo, subDocDateFrom, subDocDateTo,
                interfaceDateFrom, interfaceDateTo, paymentTermId, tradingAgencyId, vendorId, paymentStatusList, contractNo);
        }

        public void sendDiscrepancyAlert(UserRef merchandiser, int shipmentId, string currencyCode, decimal settledAmt, string invoiceNo, int userId)
        {
            NoticeHelper.sendOverILSExceptionLimitEmail(merchandiser, shipmentId, currencyCode, settledAmt, invoiceNo, null);
            shippingWorker.updateActionHistory(new ActionHistoryDef(shipmentId, 0, ActionHistoryType.SEND_AR_DISCREPANCY_ALERT, "Sent by " + generalWorker.getUserByKey(userId).DisplayName, GeneralCriteria.TRUE, userId));
        }

        public ArrayList GetSupplierProductByItemNo(string itemNo)
        {
            return accountWorker.GetSupplierProductByItemNo(itemNo);
        }

        
        #region Month End - Cutoff

        public void sendMonthEndReport(OfficeRef office, AccountFinancialCalenderDef fiscalPeriod, ReportClass rpt, int userId)
        {
            sendMonthEndReport(office, fiscalPeriod.BudgetYear, fiscalPeriod.Period, rpt, userId);
        }

        public void sendMonthEndReport(OfficeRef office, int fiscalYear, int period, ReportClass rpt, int userId)
        {
            try
            {
                UserRef usr = generalWorker.getUserByKey(userId);
                OfficeRef currentOffice = generalWorker.getOfficeRefByKey(office.OfficeId);
                string filePath = "";
                if (rpt.GetType() == typeof(reporter.accounts.CutoffSalesDiscrepancyReport))
                {   // Cutoff Sales Discrepancy Report
                    string folder = WebConfig.getValue("appSettings", "MONTH_END_FOLDER");
                    if (folder == null)
                        folder = string.Empty;
                    filePath = folder + (folder.EndsWith("\\") ? "" : "\\") + "CutoffSalesDiscrepancy_" + office.OfficeCode + "_" + fiscalYear.ToString() + " P" + period.ToString() + ".xls";
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, filePath);
                    rpt.Dispose();
                    NoticeHelper.sendCutoffSalesDiscrepancyReport(office, fiscalYear.ToString() + " P" + period.ToString(), currentOffice.MonthEndStatusId, DateTime.Now, filePath, userId);
                }
                else if (rpt.GetType() == typeof(reporter.accounts.MonthEndSlippageReport))
                {   // Month End Slippage Report
                    string folder = WebConfig.getValue("appSettings", "MONTH_END_FOLDER");
                    if (folder == null)
                        folder = string.Empty;
                    filePath = folder + (folder.EndsWith("\\") ? "" : "\\") + "MonthEndSlippage_" + office.OfficeCode + "_" + fiscalYear.ToString() + " P" + period.ToString() + ".xls";
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, filePath);
                    rpt.Dispose();
                    NoticeHelper.sendMonthEndSlippageReport(office, fiscalYear.ToString() + " P" + period.ToString(), DateTime.Now, filePath, userId);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public void updateMonthEndStatus(OfficeRef office, string fiscalPeriod, int userId)
        {
            try
            {
                OfficeRef currentOffice = generalWorker.getOfficeRefByKey(office.OfficeId);
                if (office.MonthEndStatusId != currentOffice.MonthEndStatusId)
                {
                    UserRef usr = generalWorker.getUserByKey(userId);
                    string newStatus = MonthEndStatus.getStatus(office.MonthEndStatusId).ShortName;
                    string currentStatus = MonthEndStatus.getStatus(currentOffice.MonthEndStatusId).ShortName;
                    ActionHistoryDef action = new ActionHistoryDef(-1, -1, ActionHistoryType.MONTH_END, null, "Month End Status for " + office.OfficeCode.ToString() + " office : " + currentStatus + " -> " + newStatus + " (by " + usr.DisplayName + ")", userId);
                    shippingWorker.updateActionHistory(action);
                    NoticeHelper.sendMonthEndStatusNotice(office, fiscalPeriod, currentOffice.MonthEndStatusId, DateTime.Now, usr);
                    /*
                    GeneralManager.Instance.updateOfficeList(ConvertUtility.createArrayList(office), userId);
                    */
                    GeneralManager.Instance.updateMonthEndStatus(office.OfficeId, office.MonthEndStatusId, userId);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public void resetMonthEndStatus(ArrayList officeList, string fiscalPeriod, int userId)
        {
            try
            {
                string offices = string.Empty;
                foreach (OfficeRef office in officeList)
                {
                    offices += (offices == string.Empty ? office.OfficeCode : "," + office.OfficeCode);
                    GeneralManager.Instance.updateMonthEndStatus(office.OfficeId, office.MonthEndStatusId, userId);
                }
                ActionHistoryDef action = new ActionHistoryDef(-1, -1, ActionHistoryType.MONTH_END, null, "Month End Status for the active office (" + offices + ") is reset to " + MonthEndStatus.READY.ShortName, 99999);
                shippingWorker.updateActionHistory(action);
                NoticeHelper.sendMonthEndStatusResetNotice(offices, fiscalPeriod, DateTime.Now);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public void salesCutoff(OfficeRef office, int fiscalYear, int period, int userId)
        {
            bool completed = false;
            if (processSalesCutoff(office, fiscalYear, period, userId))
                if (SendSlippageMail(office, fiscalYear, period, userId))
                    if (SubmitInterfaceBatch(office, fiscalYear, period, userId))
                        completed = true;
            office.MonthEndStatusId = (completed ? MonthEndStatus.CAPTURED.Id : MonthEndStatus.FAILED.Id);
            updateMonthEndStatus(office, fiscalYear.ToString() + " P" + period.ToString(), NSSAdminUserId);
        }

        public bool processSalesCutoff(OfficeRef office, int fiscalYear, int period, int userId)
        {
            string action = string.Empty;
            try
            {
                action = "Executing Cutoff Commarnd";
                executeSalesCutoffProcedures(office, fiscalYear, period, userId);
                return true;
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(" - Sales Cutoff for " + office.OfficeCode + " office - Action : " + action, e, "NSS Admin");
                return false;
            }
            finally
            {
            }
        }

        public bool SendSlippageMail(OfficeRef office, int fiscalYear, int period, int userId)
        {
            string action = string.Empty;
            try
            {
                action = "Generating Slippage";
                MonthEndSlippageReport rpt = AccountReportManager.Instance.getMonthEndSlippageReport(office.OfficeCode, fiscalYear, period, userId);
                action = "Sending Month End Slippage Report";
                AccountManager.Instance.sendMonthEndReport(office, fiscalYear, period, rpt, userId);
                return true;
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(" - Sending Slippage Report for " + office.OfficeCode + " office - Action : " + action, e, "NSS Admin");
                return false;
            }
            finally
            {
            }
        }

        public bool SubmitInterfaceBatch(OfficeRef office, int fiscalYear, int period, int userId)
        {
            string action = string.Empty;
            try
            {
                action = "Getting Interface Summitter";
                UserInfoDef submitUser = null;
                SystemParameterRef param = commonWorker.getSystemParameterByName(office.OfficeCode + "_CUTOFF_INTERFACE_SUBMITTER");
                if (param != null)
                {
                    string email = param.ParameterValue.Trim();
                    email = (email.Contains(";") ? email.Substring(0, email.IndexOf(";")) : email);
                    submitUser = generalWorker.getUserInfoByEmail(email);
                }
                if (submitUser != null)
                {
                    action = "Submit Interface Batch";
                    submitSunInterfaceBatch(office.OfficeId, fiscalYear, period, SunInterfaceTypeRef.getSunMacroTypeIdList(), submitUser.UserId);
                }
                else
                {
                    string subject = "ISAM: Month-End Closing - Sales Cutoff Failure - ****** Fail to Submit Interface batch for " + office.OfficeCode + " office ******";
                    string content = "Submitter cannot be identified while submitting the Interface batch for " + office.OfficeCode + " office.";
                    NoticeHelper.sendMonthEndGeneralNotice(office.OfficeCode, fiscalYear.ToString() + " P" + period.ToString(), subject, content, DateTime.Now);
                }
                return true;
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(" - Submit Interface Batch for " + office.OfficeCode + " office - Action : " + action, e, "NSS Admin");
                return false;
            }
            finally
            {
            }
        }

        public ArrayList monthEndStatusId_NotYetFinished = new ArrayList() { MonthEndStatus.READY.Id, MonthEndStatus.START.Id, MonthEndStatus.VERIFY.Id, MonthEndStatus.READYTOCAPTURE.Id };
        public ArrayList monthEndStatusId_Finished = new ArrayList() { MonthEndStatus.CAPTURED.Id, MonthEndStatus.COMPLETED.Id, MonthEndStatus.FAILED.Id };
 
        public List<OfficeRef> getOfficeforMonthEndClosing()
        {
            List<OfficeRef> officeList = new List<OfficeRef>();
            foreach (OfficeRef office in GeneralManager.Instance.getOfficeList())
            {
                office.MonthEndStatusId = generalWorker.getMonthEndStatusId(office.OfficeId);   // get the updated value of MonthEndStatusId (avoid GeneralManger get the OfficeRef from cache
                if (office.MonthEndStatusId != MonthEndStatus.NOTREADY.Id)
                    officeList.Add(office);
            }
            return officeList;
        }

        public List<OfficeRef> getOutstandingOfficeForMonthEndClosing()
        {
            List<OfficeRef> officeList = new List<OfficeRef>();
            foreach (OfficeRef office in getOfficeforMonthEndClosing())
                if (monthEndStatusId_NotYetFinished.Contains(office.MonthEndStatusId))    // the sales figure has not been captured yet
                    officeList.Add(office);
            return officeList;
        }

        public void runAutoSalesCutoff(int fiscalYear, int period, bool cutAllTogether)
        {
            string action = string.Empty;
            string officeList = string.Empty;
            string pendingOfficeList = string.Empty;
            SortedList<int, OfficeRef> selectedOffice = new SortedList<int, OfficeRef>();
            try
            {
                foreach (OfficeRef office in getOutstandingOfficeForMonthEndClosing())
                {
                    if (office.OfficeId != OfficeId.TR.Id || cutAllTogether)
                        selectedOffice.Add(office.OfficeId, office);

                    if (office.MonthEndStatusId != MonthEndStatus.READYTOCAPTURE.Id)
                    {
                        if (selectedOffice.ContainsKey(office.OfficeId))
                        {   // Month end status -> 'ReadyToCapture' : stop uploading invoice & sync operation
                            action = "Set Ready for office " + office.OfficeCode;
                            officeList += (officeList == string.Empty ? string.Empty : ", ") + office.OfficeCode;
                            office.MonthEndStatusId = MonthEndStatus.READYTOCAPTURE.Id;
                            updateMonthEndStatus(office, fiscalYear.ToString() + " P" + period.ToString(), NSSAdminUserId);
                        }
                        else
                        {   // pending for cutoff until cutForAllOutstandingOffice is set to true
                            pendingOfficeList += (pendingOfficeList == string.Empty ? string.Empty : ", ") + office.OfficeCode;




                        }
                    }
                }
                if (officeList != string.Empty)
                {
                    NoticeHelper.sendGeneralMessage("ISAM: Month-End Closing - Automatic Cutoff is Activated (Period " + fiscalYear.ToString() + " P" + period.ToString() + ")",
                        "Month End Automatic Cutoff is activated for " + officeList + " office " + "<br>" + (pendingOfficeList != string.Empty ? "(Office " + pendingOfficeList + " is pending for cutoff)" : ""));
                    if (!cutAllTogether)
                        return;     // Do not start cutoff in first section, only set the month end status for each office 
                }

                action = "Scaning Office for cutoff";
                foreach (OfficeRef office in selectedOffice.Values)
                    if (office.MonthEndStatusId == MonthEndStatus.READYTOCAPTURE.Id)
                    {
                        action = "Sales Cutoff for " + office.OfficeCode + " office in period " + fiscalYear.ToString() + " P" + period.ToString();
                        MailHelper.sendGeneralMessage("ISAM: Month-End Closing - Automatic Cutoff", action);
                        salesCutoff(office, fiscalYear, period, NSSAdminUserId);

                        if (!cutAllTogether)
                            break;  // In each section, cut off for a single office only
                    }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(" - Month-End Closing - AutoSalesCutoff action : " + action, e, "NSS Admin");
            }
            finally
            {
            }
        }


        public DateTime getAutomaticCutoffStartTime(DateTime time)
        {
            DateTime cutoffTime = DateTime.MinValue;
            SystemParameterRef param = commonWorker.getSystemParameterByName("AUTOMATIC_CUTOFF_START_TIME");
            if (param != null)
                if (!string.IsNullOrEmpty(param.ParameterValue.Trim()))
                    DateTime.TryParse(param.ParameterValue.Trim(), out cutoffTime);
            if (cutoffTime == DateTime.MinValue)
            {
                AccountFinancialCalenderDef currentPeriod = com.next.common.web.commander.CommonUtil.getAccountPeriodByDate(13, time);
                cutoffTime = getAutomaticCutoffDefaultStartTime(currentPeriod);
            }
            return cutoffTime;
        }

        public DateTime getAutomaticCutoffDefaultStartTime(AccountFinancialCalenderDef period)
        {
            DateTime actionTime, startTime, endTime;
            actionTime = startTime = endTime = DateTime.MinValue;
            if (period != null)
            {
                /*
                ArrayList holidayDefList = CommonUtil.getHolidayDefListByRange(period.StartDate, period.EndDate);
                List<DateTime> holiday = new List<DateTime>();
                foreach (HolidayDef day in holidayDefList)
                    holiday.Add(day.OnDate);
                */
                // get the first week date in the current period
                for (DateTime d = period.StartDate; d < period.EndDate && actionTime == DateTime.MinValue; d = d.AddDays(1))
                    actionTime = (d.DayOfWeek != DayOfWeek.Sunday && d.DayOfWeek != DayOfWeek.Saturday ? d : DateTime.MinValue);    // && !holiday.Contains(d))  // First Working day
                if (actionTime != DateTime.MinValue)
                    actionTime = actionTime.Add(TimeSpan.Parse("23:00:00.000"));
            }
            return actionTime;
        }

        public DateTime getAutomaticCutoffEndTime(DateTime time)
        {
            DateTime cutoffEndTime = DateTime.MinValue;
            SystemParameterRef param = commonWorker.getSystemParameterByName("AUTOMATIC_CUTOFF_END_TIME");
            if (param != null)
                if (!string.IsNullOrEmpty(param.ParameterValue.Trim()))
                    DateTime.TryParse(param.ParameterValue.Trim(), out cutoffEndTime);
            if (cutoffEndTime == DateTime.MinValue)
            {
                AccountFinancialCalenderDef currentPeriod = com.next.common.web.commander.CommonUtil.getAccountPeriodByDate(13, time);
                DateTime cutoffStartTime = getAutomaticCutoffStartTime(time);
                if (cutoffStartTime != DateTime.MinValue)
                    cutoffEndTime = cutoffStartTime.Date.AddDays(1).Add(TimeSpan.Parse("04:00:00.000"));
            }
            return cutoffEndTime;
        }

        public void executeSalesCutoffProcedures(OfficeRef office, int fiscalYear, int period, int userId)
        {
            string action = string.Empty;
            string userName = string.Empty;
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
            //TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                UserRef usr = generalWorker.getUserByKey(userId);
                userName = (usr == null ? "All" : (string.IsNullOrEmpty(usr.DisplayName) ? "All" : usr.DisplayName));

                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();
                action = "SalesCutoff";
                accountWorker.executeCutoffProcedure(action, office, fiscalYear, period, userId);
                action = "GenerateCutoffSlippage";
                accountWorker.executeCutoffProcedure(action, office, fiscalYear, period, userId);
                action = "MoveSlippageShipmentToNextPeriod";
                accountWorker.executeCutoffProcedure(action, office, fiscalYear, period, userId);
                action = "SalesCutOffCaptureTemporaryPayment";
                accountWorker.executeCutoffProcedure(action, office, fiscalYear, period, userId);
                action = "UpdateSlippageShipmentAWHDateInNSS";
                accountWorker.executeCutoffProcedure(action, office, fiscalYear, period, userId);
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                MailHelper.sendErrorAlert(" - Month-End Closing - Sales Cutoff for " + (office != null ? office.OfficeCode : "[NULL]") + " office - Action : " + action, e, userName);
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public void captureNUKSales(int fiscalYear, int fiscalPeriod)
        {

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                AccountFinancialCalenderDef cDef = generalWorker.getAccountPeriodByYearPeriod(AppId.ISAM.Code, fiscalYear, fiscalPeriod);

                ArrayList notIncludedShipmentList = accountWorker.captureNUKSales(fiscalYear, fiscalPeriod);

                if (notIncludedShipmentList.Count > 0)
                    NoticeHelper.sendNUKSalesCutOffWarning(fiscalYear, fiscalPeriod, notIncludedShipmentList);

                string filePath = AccountReportManager.Instance.GetNUKSalesCutOffSummary(fiscalYear, fiscalPeriod, cDef.StartDate, cDef.EndDate);

                NoticeHelper.sendNUKSalesCutOffNotification(fiscalYear, fiscalPeriod, filePath);

                TypeCollector officeList = TypeCollector.createNew(OfficeId.HK.Id);
                officeList.append(OfficeId.SH.Id);
                officeList.append(OfficeId.TH.Id);
                officeList.append(OfficeId.VN.Id);
                officeList.append(OfficeId.CA.Id);

                filePath = AccountReportManager.Instance.GetNUKSalesShippingReport(fiscalYear, fiscalPeriod, officeList, cDef.EndDate.AddDays(-2), cDef.EndDate, cDef.StartDate, cDef.EndDate);

                NoticeHelper.sendNUKSalesShippingReport(fiscalYear, fiscalPeriod, cDef.EndDate.AddDays(-2), cDef.EndDate, filePath);

                SystemParameterRef def = commonWorker.getSystemParameterByName("CAPTURE_NUK_SALES_READY");
                def.ParameterValue = "Y";
                commonWorker.updateSystemParameter(def);

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

        #endregion Month End - Cutoff

        public UKPaymentSupplierDef getUKPaymentSupplierByKey(string code)
        {
            return accountWorker.getUKPaymentSupplierByKey(code);
        }

        public string getUKPaymentSupplierEpicorCode(int officeId, int currencyId)
        {
            return accountWorker.getUKPaymentSupplierEpicorCode(officeId, currencyId);
        }

        public string getUKPaymentSupplierNo(int officeId, int currencyId)
        {
            return accountWorker.getUKPaymentSupplierNo(officeId, currencyId);
        }

        public CutOffStatusRef getCutOffStatus(int shipmentId)
        {
            return accountWorker.getCutOffStatus(shipmentId);
        }

        #region Advance Payment
        public AdvancePaymentDef getAdvancePaymentByKey(int paymentId)
        {
            return advancePaymentWorker.getAdvancePaymentByKey(paymentId);
        }

        public AdvancePaymentSummaryDef getAdvancePaymentSummaryDef(int paymentId)
        {
            return advancePaymentWorker.getAdvancePaymentSummaryDef(paymentId);
        }

        public List<AdvancePaymentDef> getAdvancePaymentByCriteria(int vendorId, int officeId, string paymentNo, string contractNo, string LCBillRefNo, DateTime fromDate, DateTime toDate, int paymentStatusId)
        {
            return advancePaymentWorker.getAdvancePaymentByCriteria(vendorId, officeId, paymentNo, contractNo, LCBillRefNo, fromDate, toDate, paymentStatusId);
        }

        public bool isNSLRefNoInFLContract(string FLRefNo)
        {
            return advancePaymentWorker.isNSLRefNoInFLContract(FLRefNo.Trim());
        }
        #endregion Advance Payment

        #region Advance Payment Fabric Cost
        public AdvancePaymentOrderDetailDef getAdvancePaymentOrderDetailByKey(int paymentId, int shipmentId)
        {
            return advancePaymentWorker.getAdvancePaymentOrderDetailByKey(paymentId, shipmentId);
        }

        public List<AdvancePaymentOrderDetailDef> getAdvancePaymentOrderDetailList(int paymentId)
        {
            return advancePaymentWorker.getAdvancePaymentOrderDetailList(paymentId);
        }

        public void updateAdvancePaymentAndOrderDetail(AdvancePaymentDef advancePaymentDef, List<AdvancePaymentOrderDetailDef> list, List<AdvancePaymentBalanceSettlementDef> balances, List<AdvancePaymentActionHistoryDef> historys, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                advancePaymentWorker.updateAdvancePaymentFabricCost(advancePaymentDef, list, userId);

                foreach (AdvancePaymentBalanceSettlementDef balanceSin in balances)
                {
                    balanceSin.PaymentId = advancePaymentDef.PaymentId;
                    advancePaymentWorker.updateAdvancePaymentBalanceSettlement(balanceSin, userId);
                }

                foreach (AdvancePaymentActionHistoryDef history in historys)
                {
                    history.PaymentId = advancePaymentDef.PaymentId;
                    advancePaymentWorker.updateAdvancePaymentActionHistory(history);
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

        public void deleteAdvancePaymentOrderDetail(int paymentId, int shipmentId, AdvancePaymentActionHistoryDef history, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                advancePaymentWorker.deleteAdvancePaymentOrderDetail(paymentId, shipmentId, userId);
                history.PaymentId = paymentId;
                advancePaymentWorker.updateAdvancePaymentActionHistory(history);

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
        #endregion Advance Payment Fabric Cost

        #region Advance Payment Instalment
        public AdvancePaymentInstalmentDetailDef getAdvancePaymentInstalmentDetailByKey(int paymentId, DateTime paymentDate)
        {
            return advancePaymentWorker.getAdvancePaymentInstalmentDetailByKey(paymentId, paymentDate);
        }

        public List<AdvancePaymentInstalmentDetailDef> getAdvancePaymentInstalmentDetailList(int paymentId)
        {
            return advancePaymentWorker.getAdvancePaymentInstalmentDetailList(paymentId);
        }

        public List<AdvancePaymentActionHistoryDef> getAdvancePaymentActionHistoryList(int paymentId)
        {
            return advancePaymentWorker.getAdvancePaymentActionHistoryList(paymentId);
        }

        public void updateAdvancePaymentAndInstalment(AdvancePaymentDef payment, List<AdvancePaymentInstalmentDetailDef> instalments, List<AdvancePaymentActionHistoryDef> historys, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                advancePaymentWorker.updateAdvancePayment(payment, userId);

                foreach (AdvancePaymentInstalmentDetailDef instalSin in instalments)
                {
                    instalSin.PaymentId = payment.PaymentId;
                    advancePaymentWorker.updateAdvancePaymentInstalmentDetail(instalSin, userId);
                }

                foreach (AdvancePaymentActionHistoryDef history in historys)
                {
                    history.PaymentId = payment.PaymentId;
                    advancePaymentWorker.updateAdvancePaymentActionHistory(history);
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

        public void updateAdvancePaymentInstalmentDetail(AdvancePaymentInstalmentDetailDef def, int userId)
        {
            advancePaymentWorker.updateAdvancePaymentInstalmentDetail(def, userId);
        }

        #endregion Advance Payment Instalment

        #region Advance Payment Balance Settlement
        public List<AdvancePaymentBalanceSettlementDef> getAdvancePaymentBalanceSettlementList(int paymentId)
        {
            return advancePaymentWorker.getAdvancePaymentBalanceSettlementList(paymentId);
        }

        public AdvancePaymentBalanceSettlementDef getAdvancePaymentBalanceSettlementByKey(int paymentId, DateTime paymentDate)
        {
            return advancePaymentWorker.getAdvancePaymentBalanceSettlementByKey(paymentId, paymentDate);
        }
        #endregion Advance Payment Balance Settlement

        public EpicorGLJournalDetailListDef getTurkeyEpicorGLJournalDetailList(int fiscalYear, int period, TypeCollector segValueList)
        {
            EpicorGLJournalDetailListDef list = accountWorker.getTurkeyEpicorGLJournalDetailList(fiscalYear, period, segValueList);
            if (list.EpicorGLJournalDetailList.Count == 0)
                return null;
            else
                return list;
        }

        public void insertTurkeyEpicorGLJournalRecord(List<EpicorGLJournalDetailDef> list, int fiscalYear, int period, int userId)
        {
            accountWorker.insertTurkeyEpicorGLJournalRecord(list, fiscalYear, period, userId);
        }

        public EpicorGLExtractLogListDef getTurkeyEpicorGLJournalRecordCount(int fiscalYear, int period)
        {
            return accountWorker.getTurkeyEpicorGLJournalRecordCount(fiscalYear, period);
        }

        public ArrayList getLogoInterfaceRequestList()
        {
            return accountWorker.getLogoInterfaceRequestList();
        }

        public void updateLogoInterfaceRequest(LogoInterfaceRequestDef def)
        {
            accountWorker.updateLogoInterfaceRequest(def);
        }

        public List<NSLedImportFileDef> getNSLedFiscalYearList()
        {
            return accountWorker.getNSLedFiscalYearList();
        }

        public List<NSLedImportFileDef> getNSLedFiscalWeekList(int year)
        {
            return accountWorker.getNSLedFiscalWeekList(year);
        }

        public void getNSLedRepeatItemParamRange(string itemNo, int seasonId, out int fromWeek, out int toWeek)
        {
            List<NSLedRepeatItemParamDef> list = accountWorker.getNSLedRepeatItemParamRange(itemNo, seasonId);

            fromWeek = 0;
            toWeek = 999999;

            foreach (NSLedRepeatItemParamDef def in list)
            {
                if (def.SeasonId == seasonId)
                    fromWeek = def.FiscalYear * 100 + def.FiscalWeek;
                else
                    toWeek = def.FiscalYear * 100 + def.FiscalWeek;
            }
        }

        public void fillNSLedCostDataByWeek(NSLedSalesInfoDef def, int seasonId)
        {
            accountWorker.fillNSLedCostDataByWeek(def, seasonId);
        }

        public NSLedCostDataByWeekDef getNSLedCostDataByWeek(string itemNo, int fiscalYear, int fiscalPeriod, int fiscalWeek, int seasonId, int isInitialOnly)
        {
            return accountWorker.getNSLedCostDataByWeek(itemNo, fiscalYear, fiscalPeriod, fiscalWeek, seasonId, isInitialOnly);
        }

        public List<int> getNSLedItemShippedPeriodList(string itemNo, int seasonId)
        {
            return accountWorker.getNSLedItemShippedPeriodList(itemNo, seasonId);
        }
       
        public List<NSLedSalesDef> getNSLedSalesCountry()
        {
            return accountWorker.getNSLedSalesCountry();
        }

        public ArrayList getRangePlanPhaseIdList()
        {
            ArrayList list = new ArrayList();
            string seasonSplit = accountWorker.getLatestNSLedRangePlanSeasonSplit();
            int seasonId = Convert.ToInt32(seasonSplit.Split('-')[0]);
            int splitId = Convert.ToInt32(seasonSplit.Split('-')[1]);
            int phaseId = SeasonRef.getNSLedPhaseId(seasonId, splitId);
            for (int i = phaseId; i >= 1; i--)
                list.Add(i);
            return list;
        }

        public ArrayList getNSLedSales(int officeId, string invoiceNo, int fiscalYear, int fiscalWeek, string itemNo, string countryOfSale)
        {
            return accountWorker.getNSLedSales(officeId, invoiceNo, fiscalYear, fiscalWeek, itemNo, countryOfSale);
        }

        public void updateNSLedSales(NSLedImportFileDef importFile, List<NSLedSalesDef> sales, int userId)
        {
            string fileName = (importFile != null ? importFile.Filename : string.Empty);
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                accountWorker.updateNSLedImportFile(importFile, userId);
                accountWorker.updateNSLedSalesDetailList(importFile.FileId, sales, userId);

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                NoticeHelper.sendErrorMessage(e, "Fail in uploading NS-LED file '" + fileName + "' (User ID:" + userId.ToString());
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public List<NSLedProfitabilitiesDef> getNSLedProfitabilities(TypeCollector officeIdList, int fiscalYear, int fiscalWeek)
        {
            return getNSLedProfitabilities(officeIdList, fiscalYear, fiscalWeek, TypeCollector.Inclusive, true, true, true, -1, -1);
        }

        public List<NSLedFinanceReportBFValuesDef> getNSLedFinanceReportBFValues(int officeId, int fiscalYear)
        {
            return accountWorker.getNSLedFinanceReportBFValuesList(officeId, fiscalYear);
        }

        public List<NSLedFinanceReportCYValuesDef> getNSLedFinanceReportCYValues(int officeId, int fiscalYear)
        {
            return accountWorker.getNSLedFinanceReportCYValuesList(officeId, fiscalYear);
        }

        public List<NSLedProfitabilitiesDef> getNSLedProfitabilities(TypeCollector officeIdList, int fiscalYear, int fiscalWeek, TypeCollector phaseId, bool isStillSelling, bool isNotYetLaunched, bool isEndOfLife, int ukProductTeamId, int isDuty)
        {
            return accountWorker.getNSLedProfitabilities(officeIdList, fiscalYear, fiscalWeek, phaseId, isStillSelling, isNotYetLaunched, isEndOfLife, ukProductTeamId, isDuty);
        }

        public List<NSLedSalesInfoDef> getNSLedSalesInfo(int fiscalYear, int fiscalWeek)
        {
            return accountWorker.getNSLedSalesInfo(fiscalYear, fiscalWeek);
        }

        public int getLGDetail(int shipmentId, int splitShipmentId)
        {
            return accountWorker.getLGDetail(shipmentId, splitShipmentId);
        }

        public LetterOfGuaranteeDef getLetterOfGuaranteeByKey(int key)
        {
            return accountWorker.getLetterOfGuaranteeByKey(key);
        }

        public void updateAdvancePaymentActionHistory(AdvancePaymentActionHistoryDef def)
        {
            advancePaymentWorker.updateAdvancePaymentActionHistory(def);
        }

        public ARCustomerDef getARCustomerByCode(string code)
        {
            return accountWorker.getARCustomerByCode(code);
        }

        public GenericDCNoteDef getGenericDCNoteById(int id)
        {
            return accountWorker.getGenericDCNoteById(id);
        }

        public GenericDCNoteDef getGenericDCNoteByNoteNo(string no)
        {
            return accountWorker.getGenericDCNoteByNoteNo(no);
        }

        public void updateGenericDCNote(GenericDCNoteDef def, int userId, out string dcNoteNo)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                accountWorker.updateGenericDCNote(def, userId, out dcNoteNo);
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                NoticeHelper.sendErrorMessage(e, "Error occured in updating Generic D/C Note");
                throw e;
            }
            finally
            {
                ctx.Exit();
            }

        }

        public ArrayList getThirdPartyCommissionInvoiceToUK(int fiscalYear, int period)
        {
            return accountWorker.getThirdPartyCommissionInvoiceToUK(fiscalYear, period);
        }

        public int getNSLedRangePlanSeasonIdByFiscalWeek(int officeId, string itemNo, int fiscalYear, int fiscalWeek)
        {
            return accountWorker.getNSLedRangePlanSeasonIdByFiscalWeek(officeId, itemNo, fiscalYear, fiscalWeek);
        }


        public int getNSLedRangePlanSeasonIdByDeliveryDate(int officeId, string itemNo, DateTime deliveryDate)
        {
            return accountWorker.getNSLedRangePlanSeasonIdByDeliveryDate(officeId, itemNo, deliveryDate);
        }

        public bool isSupplierPaymentHold(int vendorId)
        {
            return accountWorker.isSupplierPaymentHold(vendorId);
        }

        public string getSupplierPaymentHoldRemark(int vendorId)
        {
            return accountWorker.getSupplierPaymentHoldRemark(vendorId);
        }

    }
}
