using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.claim;
using com.next.isam.domain.claim;
using com.next.isam.domain.account;
using com.next.infra.persistency.transactions;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.appserver.shipping;
using com.next.isam.appserver.account;
using com.next.isam.domain.order;
using System.IO;

namespace com.next.isam.appserver.startup
{
    public class UKClaimMappingHandler : IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "UKClaimMappingHandler";
        private QAIS.ClaimRequestService svc = null;
        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public UKClaimMappingHandler()
        {
        }

        public void Initialize()
        {
            StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
        }

        private void workOnIt(object state)
        {
            TimeState ts = (TimeState)state;

            if (DateTime.Now.Hour != 3) return;
            if (DateTime.Now.Minute < 30) return;
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;

            /*
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 30) return;
            */

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public string handlerName()
        {
            return UKClaimMappingHandler.DEFAULT_NAME;
        }

        private Timer load(TimeSpan ts)
        {
            TimeState state = new TimeState();
            TimerCallback tcall = new TimerCallback(this.workOnIt);
            Timer timer = new Timer(tcall, state, new TimeSpan(0), ts);
            return timer;
        }

        #region TIMERJOBS

        public void runNow()
        {
            try
            {
                
                //AccountManager.Instance.submitSunInterfaceBatch(9, 2019, 1, SunInterfaceTypeRef.getSunMacroTypeIdList(), 1981);
                //AccountManager.Instance.submitSunInterfaceBatch(3, 2019, 1, SunInterfaceTypeRef.getSunMacroTypeIdList(), 1722);

                svc = new QAIS.ClaimRequestService();
                
                this.processMappings();
                this.setSubmittedForMappedUKClaimList();
                this.setSubmittedForMappedUKClaimListForByPass();
                this.setCancelledForMappedUKClaimList();
                this.createDummyClaimRequests();
                this.processMappings();

                UKClaimManager.Instance.populateUKClaimRechargeNR();
                UKClaimManager.Instance.generateFullRefundUKClaimDCNote(DateTime.Today, 99999);
                this.createBillInAdvanceClaims();
                this.sendUKClaimDiscrepancyList();

                /*
                this.createDummyClaimRequestsForDuplicates();
                */

                /*
                this.uploadCOO();
                */
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, string.Empty);
            }


        }

        private void sendUKClaimDiscrepancyList()
        {
            List<UKClaimDef> list = UKClaimManager.Instance.getUKClaimDiscrepancyList();
            if (list.Count > 0)
            {
                NoticeHelper.sendUKClaimDiscrepancyListEmail(list);
            }
        }

        private void createDummyClaimRequests()
        {
            TypeCollector officeIds = TypeCollector.Inclusive;
            officeIds.append(-1);
            List<UKClaimDef> list = UKClaimManager.Instance.getUKClaimEarlyArrivalList(officeIds);
            foreach (UKClaimDef def in list)
            {
                //object[] activeList = svc.GetClaimRequestListByTypeMapping(-1, def.Type.Id, def.Vendor.VendorId, def.ItemNo, (def.Type.Id == UKClaimType.MFRN.Id ? def.ClaimMonth : def.UKDebitNoteNo));
                object[] activeList = svc.GetClaimRequestListByTypeMapping(-1, def.Type.Id, def.Vendor.VendorId, def.ItemNo, (def.Type.Id == UKClaimType.MFRN.Id ? def.UKDebitNoteNo : def.UKDebitNoteNo));
                if (activeList.Length == 0)
                {
                    int userId = 99999;
                    if (def.OfficeId == OfficeId.HK.Id || def.OfficeId == OfficeId.CA.Id || def.OfficeId == OfficeId.TH.Id)
                    {
                        userId = 1691;
                        if (def.TermOfPurchaseId != 1 && def.OfficeId == OfficeId.HK.Id)
                        {
                            /*
                            userId = 1691;
                            */
                            userId = 31650;
                        }
                        else
                        {
                            ArrayList osList = CommonUtil.getProductCodeListByCriteria(2090, -1, GeneralCriteria.ALL, -1);
                            foreach (ProductCodeRef osDef in osList)
                            {
                                if (osDef.ProductCodeId == def.ProductTeamId && (osDef.Code != "NCHB" || osDef.Code != "HBED"))
                                {
                                    userId = 1912;
                                    break;
                                }
                            }

                            osList = CommonUtil.getProductCodeListByCriteria(31688, -1, GeneralCriteria.ALL, -1);
                            foreach (ProductCodeRef osDef in osList)
                            {
                                if (osDef.ProductCodeId == def.ProductTeamId && (osDef.Code != "NCHB" || osDef.Code != "HBED"))
                                {
                                    userId = 1912;
                                    break;
                                }
                            }

                        }
                    }
                    if (def.OfficeId == OfficeId.TH.Id)
                    {
                        userId = 1691;
                        ArrayList osList = CommonUtil.getProductCodeListByCriteria(1828, -1, GeneralCriteria.ALL, -1);
                        foreach (ProductCodeRef osDef in osList)
                        {
                            if (osDef.ProductCodeId == def.ProductTeamId)
                            {
                                userId = 1912;
                                break;
                            }
                        }
                    }
                    else if (def.OfficeId == OfficeId.DG.Id)
                    {
                        if (def.Vendor.VendorId == 7692 || def.Vendor.VendorId == 7413 || def.Vendor.VendorId == 7881 || def.Vendor.VendorId == 8114 || def.Vendor.VendorId == 7816)
                            userId = 32223;
                        else if (def.HandlingOfficeId == OfficeId.HK.Id)
                            userId = 1691;
                        else if (def.HandlingOfficeId == OfficeId.VN.Id)
                            userId = 32237;
                        else if (def.HandlingOfficeId == OfficeId.SH.Id)
                            userId = 30725;
                        else
                            userId = 32223;
                    }
                    else if (def.OfficeId == OfficeId.TR.Id || def.OfficeId == OfficeId.EG.Id) userId = 32587;
                    else if (def.OfficeId == OfficeId.ND.Id) userId = 32558;
                    else if (def.OfficeId == OfficeId.BD.Id) userId = 31876;
                    else if (def.OfficeId == OfficeId.PK.Id) userId = 31932;
                    else if (def.OfficeId == OfficeId.VN.Id) userId = 32237;
                    else if (def.OfficeId == OfficeId.SH.Id) userId = 30725;
                    else if (def.OfficeId == OfficeId.SL.Id) userId = 30470;
                    else if (def.OfficeId == OfficeId.IND.Id) userId = 31924;

                    decimal amt = def.Amount * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, def.UKDebitNoteDate) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.GBP.Id, def.UKDebitNoteDate);
                    if (userId == 1691 && def.Currency.CurrencyId == CurrencyId.USD.Id)
                        amt = def.Amount / 1.6m;
                    svc.CreateDummyClaimRequest(def.Type.Id, def.Vendor.VendorId, def.ItemNo, def.ContractNo, def.UKDebitNoteNo, def.UKDebitNoteDate, def.ClaimMonth, def.Quantity, amt, def.Remark, userId);
                }
            }
        }

        private void createDummyClaimRequestsForDuplicates()
        {
            TypeCollector officeIds = TypeCollector.Inclusive;
            officeIds.append(-1);
            List<UKClaimDef> list = UKClaimManager.Instance.getUKClaimEarlyArrivalList(officeIds);
            foreach (UKClaimDef def in list)
            {
                //object[] activeList = svc.GetClaimRequestListByTypeMapping(-1, def.Type.Id, def.Vendor.VendorId, def.ItemNo, (def.Type.Id == UKClaimType.MFRN.Id ? def.ClaimMonth : def.UKDebitNoteNo));
                object[] activeList = svc.GetClaimRequestListByTypeMapping(-1, def.Type.Id, def.Vendor.VendorId, def.ItemNo, (def.Type.Id == UKClaimType.MFRN.Id ? def.UKDebitNoteNo : def.UKDebitNoteNo));
                int userId = 99999;
                if (def.OfficeId == OfficeId.HK.Id || def.OfficeId == OfficeId.CA.Id || def.OfficeId == OfficeId.TH.Id)
                {
                    userId = 1691;
                    if (def.TermOfPurchaseId != 1 && def.OfficeId == OfficeId.HK.Id)
                    {
                        /*
                        userId = 31650;
                        */
                        userId = 1691;
                    }
                    else
                    {
                        ArrayList osList = CommonUtil.getProductCodeListByCriteria(1286, -1, GeneralCriteria.ALL, -1);
                        foreach (ProductCodeRef osDef in osList)
                        {
                            if (osDef.ProductCodeId == def.ProductTeamId && (osDef.Code != "NCHB" || osDef.Code != "HBED"))
                            {
                                userId = 1912;
                                break;
                            }
                        }
                        osList = CommonUtil.getProductCodeListByCriteria(31688, -1, GeneralCriteria.ALL, -1);
                        foreach (ProductCodeRef osDef in osList)
                        {
                            if (osDef.ProductCodeId == def.ProductTeamId && (osDef.Code != "NCHB" || osDef.Code != "HBED"))
                            {
                                userId = 1912;
                                break;
                            }
                        }
                    }
                }
                if (def.OfficeId == OfficeId.TH.Id)
                {
                    userId = 1691;
                    ArrayList osList = CommonUtil.getProductCodeListByCriteria(1516, -1, GeneralCriteria.ALL, -1);
                    foreach (ProductCodeRef osDef in osList)
                    {
                        if (osDef.ProductCodeId == def.ProductTeamId)
                        {
                            userId = 1912;
                            break;
                        }
                    }
                }
                else if (def.OfficeId == OfficeId.DG.Id)
                {
                    if (def.Vendor.VendorId == 7692 || def.Vendor.VendorId == 7413 || def.Vendor.VendorId == 7881 || def.Vendor.VendorId == 8114 || def.Vendor.VendorId == 7816)
                        userId = 32223;
                    else if (def.HandlingOfficeId == OfficeId.HK.Id)
                        userId = 1691;
                    else if (def.HandlingOfficeId == OfficeId.VN.Id)
                        userId = 32237;
                    else if (def.HandlingOfficeId == OfficeId.SH.Id)
                        userId = 30725;
                    else
                        userId = 32223;
                }
                else if (def.OfficeId == OfficeId.TR.Id || def.OfficeId == OfficeId.EG.Id) userId = 31546;
                else if (def.OfficeId == OfficeId.ND.Id) userId = 32558;
                else if (def.OfficeId == OfficeId.BD.Id) userId = 31876;
                else if (def.OfficeId == OfficeId.PK.Id) userId = 31932;
                else if (def.OfficeId == OfficeId.VN.Id) userId = 32237;
                else if (def.OfficeId == OfficeId.SH.Id) userId = 30725;
                else if (def.OfficeId == OfficeId.SL.Id) userId = 30470;
                else if (def.OfficeId == OfficeId.IND.Id) userId = 31924;

                decimal amt = def.Amount * CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, def.Currency.CurrencyId, def.UKDebitNoteDate) / CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.GBP.Id, def.UKDebitNoteDate);
                if (userId == 1691 && def.Currency.CurrencyId == CurrencyId.USD.Id)
                    amt = def.Amount / decimal.Parse("1.6");
                svc.CreateDummyClaimRequest(def.Type.Id, def.Vendor.VendorId, def.ItemNo, def.ContractNo, def.UKDebitNoteNo, def.UKDebitNoteDate, def.ClaimMonth, def.Quantity, amt, def.Remark, userId);

                UKClaimLogDef log = null;
                log = new UKClaimLogDef(def.ClaimId, "Auto Claim Request Mapping", 99999, ClaimWFS.NEW.Id, def.WorkflowStatus.Id);

                UKClaimWorker.Instance.updateUKClaimLog(log, 99999);
            }
        }

        private void setSubmittedForMappedUKClaimList()
        {
            try
            {
                List<UKClaimDef> list = UKClaimManager.Instance.getNewlyMappedUKClaimList();
                foreach (UKClaimDef def in list)
                {
                    QAIS.ClaimRequestDef requestDef = null;
                    /*
                    List<QAIS.ClaimRequestDef> requestList = new List<QAIS.ClaimRequestDef>();
                    object[] activeList = svc.GetActiveClaimRequestListByType(-1, def.Type.Id, def.Vendor.VendorId, def.ItemNo, (def.Type.Id == UKClaimType.MFRN.Id ? def.ClaimMonth : def.UKDebitNoteNo));
                    if (activeList.Length == 1)
                    {
                        foreach (object o in activeList)
                            if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                                requestDef = ((QAIS.ClaimRequestDef)o);
                    }
                    */

                    requestDef = svc.GetClaimRequestByKey(def.ClaimRequestId);
                    if (!(((requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.NSRechargePercent == 0)
                        || (requestDef.WorkflowStatusId == 10)))
                        requestDef = null;

                    if (requestDef != null)
                    {
                        /*
                         * remove sample image requirement @2012-07-23
                        if ((((svc.IsSampleImageAvailable(def.ClaimRequestId) && requestDef.IsAuthorized) || (def.Type.Id == UKClaimType.MFRN.Id && requestDef.IsAuthorized)) && (requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7)) || requestDef.WorkflowStatusId == 10)
                        */

                        if ((((requestDef.IsAuthorized) || (def.Type.Id == UKClaimType.MFRN.Id && requestDef.IsAuthorized)) && (requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7)) || requestDef.WorkflowStatusId == 10)
                        {
                            if (requestDef.ItemNo == def.ItemNo && requestDef.Vendor.VendorId == def.Vendor.VendorId)
                            {
                                this.mapUKClaim(def, requestDef.WorkflowStatusId, requestDef.RequestId, true);
                                if (requestDef.WorkflowStatusId != 10)
                                    this.updateClaimRequest(requestDef, false);
                            }
                        }
                        /*
                        this.updateDMSIndexFields(def);
                        */
                    }
                }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
        }

        private void setSubmittedForMappedUKClaimListForByPass()
        {
            try
            {
                List<UKClaimDef> list = UKClaimManager.Instance.getNewlyMappedUKClaimList();
                foreach (UKClaimDef def in list)
                {
                    if (((def.OfficeId == OfficeId.EG.Id || def.OfficeId == OfficeId.TR.Id) && def.UKDebitNoteReceivedDate >= new DateTime(2013, 6, 3))
                        || (def.OfficeId == OfficeId.SH.Id && def.Type == UKClaimType.MFRN))
                    {
                        QAIS.ClaimRequestDef requestDef = null;

                        requestDef = svc.GetClaimRequestByKey(def.ClaimRequestId);
                        if ((requestDef.IsAuthorized == true && def.OfficeId == OfficeId.SH.Id) || (def.OfficeId != OfficeId.SH.Id))
                        {
                            if (!(((requestDef.WorkflowStatusId == 1 || requestDef.WorkflowStatusId == 2
                                    || requestDef.WorkflowStatusId == 3 || requestDef.WorkflowStatusId == 4
                                    || requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7) && requestDef.NSRechargePercent == 0)
                                || (requestDef.WorkflowStatusId == 10)))
                                requestDef = null;

                            if (requestDef != null)
                            {
                                if (requestDef.ItemNo == def.ItemNo && requestDef.Vendor.VendorId == def.Vendor.VendorId)
                                {
                                    this.mapUKClaim(def, requestDef.WorkflowStatusId, requestDef.RequestId, true);
                                    if (requestDef.WorkflowStatusId != 10)
                                        this.updateClaimRequest(requestDef, false);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
        }

        private void setCancelledForMappedUKClaimList()
        {
            try
            {

                object[] requestList = svc.GetPendingCancellationClaimRequestList();
                foreach (object o in requestList)
                {
                    if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                    {
                        QAIS.ClaimRequestDef requestDef = null;
                        requestDef = ((QAIS.ClaimRequestDef)o);
                        UKClaimDef def = UKClaimManager.Instance.getUKClaimByClaimRequestId(requestDef.RequestId);
                        if (def != null && def.WorkflowStatus.Id == ClaimWFS.CANCELLED.Id)
                        {
                            this.updateClaimRequest(requestDef, true);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
        }

        private void createBillInAdvanceClaims()
        {
            try
            {

                object[] requestList = svc.GetOutstandingBIAClaimRequestList();
                foreach (object o in requestList)
                {
                    if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                    {
                        QAIS.ClaimRequestDef requestDef = null;
                        requestDef = ((QAIS.ClaimRequestDef)o);
                        UKClaimDef def = new UKClaimDef();
                        def.ClaimId = -1;
                        def.Amount = requestDef.BIAAmount;
                        def.Currency = CommonUtil.getCurrencyByKey(requestDef.BIACurrencyId);
                        def.Type = UKClaimType.BILL_IN_ADVANCE;
                        if (requestDef.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                            def.ClaimMonth = requestDef.ClaimMonth;
                        def.ClaimRequestId = -1;
                        def.ContractNo = requestDef.ContractNo;
                        def.ItemNo = requestDef.ItemNo;
                        def.Vendor = IndustryUtil.getVendorByKey(requestDef.Vendor.VendorId);
                        def.WorkflowStatus = ClaimWFS.SUBMITTED;
                        def.GUId = Guid.NewGuid().ToString();
                        ArrayList shipmentProductList = ShipmentManager.Instance.GetShipmentProductByItemNo(requestDef.ItemNo, requestDef.ContractNo, requestDef.Vendor.VendorId);
                        foreach (ShipmentProductDef shipmentProductDef in shipmentProductList)
                        {
                            if (shipmentProductDef.VendorId == requestDef.Vendor.VendorId)
                            {
                                def.OfficeId = shipmentProductDef.OfficeId;
                                def.TermOfPurchaseId = shipmentProductDef.TermOfPurchaseId;
                                def.HandlingOfficeId = shipmentProductDef.HandlingOfficeId;
                                def.ProductTeamId = shipmentProductDef.ProductTeamId;
                                break;
                            }
                        }
                        def.HasUKDebitNote = false;
                        def.Quantity = requestDef.FaultQty;
                        def.UKDebitNoteReceivedDate = DateTime.MinValue;
                        def.DebitNoteNo = requestDef.RequestId.ToString();
                        def.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + "Raised For " + requestDef.ClaimType.ToString() + " : " + requestDef.FormNo;

                        UKClaimManager.Instance.updateUKClaimDef(def, 99999);
                        svc.MarkBIATransmitCompleted(requestDef.RequestId, 99999);
                    }
                }

            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
        }

        private void processMappings()
        {
            try
            {
                List<UKClaimDef> list = UKClaimManager.Instance.getNotYetMappedUKClaimList();
                foreach (UKClaimDef def in list)
                {
                    QAIS.ClaimRequestDef requestDef = null; ;
                    List<QAIS.ClaimRequestDef> requestList = new List<QAIS.ClaimRequestDef>();
                    //object[] activeList = svc.GetClaimRequestListByTypeMapping(-1, def.Type.Id, def.Vendor.VendorId, def.ItemNo, (def.Type.Id == UKClaimType.MFRN.Id ? def.ClaimMonth : def.UKDebitNoteNo));
                    object[] activeList = svc.GetClaimRequestListByTypeMapping(-1, def.Type.Id, def.Vendor.VendorId, def.ItemNo, (def.Type.Id == UKClaimType.MFRN.Id ? def.UKDebitNoteNo : def.UKDebitNoteNo));
                    if (activeList.Length > 1)
                    {
                        NoticeHelper.sendDuplicateUKDNRequestMail(def.Type.Id, def.UKDebitNoteNo);
                        int cnt = 0;
                        foreach (object o in activeList)
                        {
                            if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                            {
                                requestDef = ((QAIS.ClaimRequestDef)o);
                                if (UKClaimManager.Instance.getUKClaimByClaimRequestId(requestDef.RequestId) == null)
                                    cnt += 1;
                            }
                        }
                        if (cnt != 1) requestDef = null;
                    }
                    else if (activeList.Length == 1)
                    {
                        foreach (object o in activeList)
                            if (o.GetType() == typeof(QAIS.ClaimRequestDef))
                                requestDef = ((QAIS.ClaimRequestDef)o);
                    }

                    if (requestDef != null)
                    {
                        UKClaimDef existingDef = UKClaimWorker.Instance.getUKClaimByClaimRequestId(requestDef.RequestId);
                        if (existingDef == null)
                        {
                            if ((((requestDef.IsAuthorized) || (def.Type.Id == UKClaimType.MFRN.Id && requestDef.IsAuthorized)) && (requestDef.WorkflowStatusId == 6 || requestDef.WorkflowStatusId == 7)))
                            {
                                this.mapUKClaim(def, requestDef.WorkflowStatusId, requestDef.RequestId, true);
                                this.updateClaimRequest(requestDef, false);
                            }
                            else
                            {
                                this.mapUKClaim(def, requestDef.WorkflowStatusId, requestDef.RequestId, false);
                            }
                            this.updateDMSIndexFields(def);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
        }


        private void mapUKClaim(UKClaimDef ukClaimDef, int workflowStatusId, int claimRequestId, bool isConfirmed)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ukClaimDef.ClaimRequestId = claimRequestId;

                if (isConfirmed)
                {
                    if (workflowStatusId == 8)
                    {
                        ukClaimDef.WorkflowStatus = ClaimWFS.USER_SIGNED_OFF;
                        if (ukClaimDef.Remark == string.Empty)
                            ukClaimDef.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier";
                        else
                            ukClaimDef.Remark += ("\n" + "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier");
                    }
                    else
                    {
                        ukClaimDef.WorkflowStatus = ClaimWFS.SUBMITTED;
                        if (workflowStatusId != 10)
                        {
                            if (ukClaimDef.Remark == string.Empty)
                                ukClaimDef.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier";
                            else
                                ukClaimDef.Remark += ("\n" + "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier");
                            /* 2013-11-12, in case on hold, not to re-enable
                            ukClaimDef.IsReadyForSettlement = true;
                            */
                        }

                        if (workflowStatusId == 10)
                            ukClaimDef.IsReadyForSettlement = false;
                    }
                    /*
                    else if (workflowStatusId == 6 || workflowStatusId == 7 || workflowStatusId == 10)
                    {
                        ukClaimDef.WorkflowStatus = ClaimWFS.SUBMITTED;
                        if (workflowStatusId == 6 || workflowStatusId == 7)
                        {
                            if (ukClaimDef.Remark == string.Empty)
                                ukClaimDef.Remark = "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier";
                            else
                                ukClaimDef.Remark += ("\n" + "[" + DateTime.Today.ToString("dd/MM") + "] " + "Will Debit Supplier");
                            ukClaimDef.IsReadyForSettlement = true;
                        }

                        if (workflowStatusId == 10)
                            ukClaimDef.IsReadyForSettlement = false;
                    }
                    */
                }
                UKClaimWorker.Instance.updateUKClaim(ukClaimDef, 99999);

                UKClaimLogDef log = null;
                log = new UKClaimLogDef(ukClaimDef.ClaimId, "Auto Claim Request Mapping", 99999, ClaimWFS.NEW.Id, ukClaimDef.WorkflowStatus.Id);

                UKClaimWorker.Instance.updateUKClaimLog(log, 99999);

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                MailHelper.sendErrorAlert(e, String.Empty);
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void updateClaimRequest(QAIS.ClaimRequestDef def, bool isCancelled)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                svc.SetClaimRequestStatus(def.RequestId, (isCancelled ? 99 : 9), 99999);
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                MailHelper.sendErrorAlert(e, String.Empty);
            }
            finally
            {
                ctx.Exit();
            }
        }


        private void updateDMSIndexFields(UKClaimDef ukClaim)
        {
            long ukDNDocId = 0;
            long authFormId = 0;
            ArrayList updatedQueryStructs = new ArrayList();
            updatedQueryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            updatedQueryStructs.Add(new QueryStructDef("Item No", ukClaim.ItemNo));
            updatedQueryStructs.Add(new QueryStructDef("Debit Note No", ukClaim.UKDebitNoteNo));
            if (ukClaim.Type.Id == UKClaimType.REJECT.Id || ukClaim.Type.Id == UKClaimType.REWORK.Id)
                updatedQueryStructs.Add(new QueryStructDef("Qty", ukClaim.Quantity.ToString()));

            ArrayList queryStructs = new ArrayList();

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaim.Type.DMSDescription));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Item No", ukClaim.ItemNo));
            queryStructs.Add(new QueryStructDef("Debit Note No", ukClaim.UKDebitNoteNo));
            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            foreach (DocumentInfoDef docInfoDef in qList)
                ukDNDocId = docInfoDef.DocumentID;

            queryStructs.Clear();
            QAIS.ClaimRequestDef req = svc.GetClaimRequestByKey(ukClaim.ClaimRequestId);

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Authorization Form"));
            queryStructs.Add(new QueryStructDef("Item No", req.ItemNo));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaim.Type.DMSDescription));
            if (req.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                queryStructs.Add(new QueryStructDef("MFRN Month", req.ClaimMonth));
            else
                queryStructs.Add(new QueryStructDef("Form No", req.FormNo));
            qList = DMSUtil.queryDocument(queryStructs);
            foreach (DocumentInfoDef docInfoDef in qList)
            {
                authFormId = docInfoDef.DocumentID;
                foreach (FieldInfoDef fiDef in docInfoDef.FieldInfos)
                {
                    if (fiDef.FieldName != "Supporting Doc Type" && fiDef.FieldName != "Item No" && fiDef.FieldName != "Debit Note No" && fiDef.FieldName != "Qty")
                        updatedQueryStructs.Add(new QueryStructDef(fiDef.FieldName, fiDef.Content));
                }
                break;
            }

            if (authFormId != 0 && ukDNDocId != 0)
            {
                string strReturn = DMSUtil.UpdateDocument(ukDNDocId, updatedQueryStructs);
            }
            else if (req.WorkflowStatusId == 0 && ukDNDocId != 0)
            {
                updatedQueryStructs.Add(new QueryStructDef("MFRN Month", ukClaim.ClaimMonth));
                updatedQueryStructs.Add(new QueryStructDef("Form No", ukClaim.UKDebitNoteNo));
                updatedQueryStructs.Add(new QueryStructDef("Supplier Name", ukClaim.Vendor.Name));
                string strReturn = DMSUtil.UpdateDocument(ukDNDocId, updatedQueryStructs);
            }

            this.updateDMSIndexFieldsForOthers(ukClaim);
        }


        private void updateDMSIndexFieldsForOthers(UKClaimDef ukClaim)
        {
            ArrayList ukDocIds = new ArrayList();
            long authFormId = 0;
            ArrayList updatedQueryStructs = new ArrayList();
            updatedQueryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim Refund Supporting Doc"));
            updatedQueryStructs.Add(new QueryStructDef("Item No", ukClaim.ItemNo));
            updatedQueryStructs.Add(new QueryStructDef("Debit Note No", ukClaim.UKDebitNoteNo));

            ArrayList queryStructs = new ArrayList();

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaim.Type.DMSDescription));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim Refund Supporting Doc"));
            queryStructs.Add(new QueryStructDef("Item No", ukClaim.ItemNo));
            queryStructs.Add(new QueryStructDef("Debit Note No", ukClaim.UKDebitNoteNo));
            ArrayList qList = DMSUtil.queryDocument(queryStructs);
            if (qList.Count > 0)
            {
                foreach (DocumentInfoDef docInfoDef in qList)
                    ukDocIds.Add(docInfoDef.DocumentID.ToString());

                queryStructs.Clear();
                QAIS.ClaimRequestDef req = svc.GetClaimRequestByKey(ukClaim.ClaimRequestId);

                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Authorization Form"));
                queryStructs.Add(new QueryStructDef("Item No", req.ItemNo));
                queryStructs.Add(new QueryStructDef("Claim Type", ukClaim.Type.DMSDescription));
                if (req.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                    queryStructs.Add(new QueryStructDef("MFRN Month", req.ClaimMonth));
                else
                    queryStructs.Add(new QueryStructDef("Form No", req.FormNo));
                qList = DMSUtil.queryDocument(queryStructs);
                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    authFormId = docInfoDef.DocumentID;
                    foreach (FieldInfoDef fiDef in docInfoDef.FieldInfos)
                    {
                        if (fiDef.FieldName != "Supporting Doc Type" && fiDef.FieldName != "Item No" && fiDef.FieldName != "Debit Note No")
                            updatedQueryStructs.Add(new QueryStructDef(fiDef.FieldName, fiDef.Content));
                    }
                    break;
                }

                if (authFormId != 0 && ukDocIds.Count != 0)
                {
                    string strReturn = string.Empty;
                    foreach (string s in ukDocIds)
                    {
                        strReturn = DMSUtil.UpdateDocument(long.Parse(s), updatedQueryStructs);
                    }
                }
                else if (req.WorkflowStatusId == 0 && ukDocIds.Count != 0)
                {
                    updatedQueryStructs.Add(new QueryStructDef("MFRN Month", ukClaim.ClaimMonth));
                    updatedQueryStructs.Add(new QueryStructDef("Form No", ukClaim.UKDebitNoteNo));
                    updatedQueryStructs.Add(new QueryStructDef("Supplier Name", ukClaim.Vendor.Name));
                    string strReturn = string.Empty;
                    foreach (string s in ukDocIds)
                    {
                        strReturn = DMSUtil.UpdateDocument(long.Parse(s), updatedQueryStructs);
                    }
                }
            }
        }


        private ArrayList getQueryStringList(string docType, UKClaimDef ukClaimDef)
        {
            ArrayList queryStructs = new ArrayList();
            ArrayList returnList = new ArrayList();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
            QAIS.ClaimRequestDef def = svc.GetClaimRequestByKey(ukClaimDef.ClaimRequestId);

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Authorization Form"));
            queryStructs.Add(new QueryStructDef("Form No", def.FormNo));
            queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
            queryStructs.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription));
            if (def.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                queryStructs.Add(new QueryStructDef("MFRN Month", def.ClaimMonth));

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
                if (ukClaimDef.UKDebitNoteNo != string.Empty)
                    returnList.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));
            }
            else
            {
                returnList.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                returnList.Add(new QueryStructDef("Supporting Doc Type", docType));
                returnList.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription));
                returnList.Add(new QueryStructDef("Item No", ukClaimDef.ItemNo));
                returnList.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));
                returnList.Add(new QueryStructDef("MFRN Month", ukClaimDef.ClaimMonth));
                returnList.Add(new QueryStructDef("Form No", ukClaimDef.UKDebitNoteNo));
                returnList.Add(new QueryStructDef("Supplier Name", ukClaimDef.Vendor.Name));

            }
            return returnList;
        }

        private void uploadCOO()
        {
            string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);

            string filename = string.Empty;

            List<UKClaimDef> ukclaimList = UKClaimManager.Instance.getUKClaimDebugList();
            foreach (UKClaimDef ukClaimDef in ukclaimList)
            {

                filename = "D:\\common\\block\\refund\\bd\\SKMBT_C36013013014590.pdf";
                ArrayList queryStructs = this.getQueryStringList("Signed Form From NS", ukClaimDef);

                ArrayList qList = DMSUtil.queryDocument(queryStructs);
                long docId = 0;
                ArrayList attachmentList = new ArrayList();

                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    docId = docInfoDef.DocumentID;
                }

                if (docId > 0)
                {
                    string path = outputFolder + "SKMBT_C36013013014590.pdf";
                    System.IO.File.Copy(filename, path, true);
                    attachmentList.Add(path);
                    string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                }
                else
                {
                    string path = outputFolder + "SKMBT_C36013013014590.pdf";
                    System.IO.File.Copy(filename, path, true);
                    attachmentList.Add(path);
                    string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + ukClaimDef.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);
                }

                UKClaimLogDef logDef = new UKClaimLogDef(ukClaimDef.ClaimId, "Add Attachment [" + "Signed Form From NS" + "] (" + "SKMBT_C36013013014590.pdf" + ")", 616, ukClaimDef.WorkflowStatus.Id, ukClaimDef.WorkflowStatus.Id);
                UKClaimManager.Instance.updateUKClaimLogDef(logDef, 616);

                queryStructs = this.getQueryStringList("Signed Copy By Director", ukClaimDef);

                qList = DMSUtil.queryDocument(queryStructs);
                docId = 0;
                attachmentList = new ArrayList();

                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    docId = docInfoDef.DocumentID;
                }

                if (docId > 0)
                {
                    string path = outputFolder + "SKMBT_C36013013014590.pdf";
                    System.IO.File.Copy(filename, path, true);
                    attachmentList.Add(path);
                    string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                }
                else
                {
                    string path = outputFolder + "SKMBT_C36013013014590.pdf";
                    System.IO.File.Copy(filename, path, true);
                    attachmentList.Add(path);
                    string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + ukClaimDef.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);
                }

                logDef = new UKClaimLogDef(ukClaimDef.ClaimId, "Add Attachment [" + "Signed Copy By Director" + "] (" + "SKMBT_C36013013014590.pdf" + ")", 616, ukClaimDef.WorkflowStatus.Id, ukClaimDef.WorkflowStatus.Id);
                UKClaimManager.Instance.updateUKClaimLogDef(logDef, 616);

                queryStructs = this.getQueryStringList("Signed Copy By COO", ukClaimDef);

                qList = DMSUtil.queryDocument(queryStructs);
                docId = 0;
                attachmentList = new ArrayList();

                foreach (DocumentInfoDef docInfoDef in qList)
                {
                    docId = docInfoDef.DocumentID;
                }

                if (docId > 0)
                {
                    string path = outputFolder + "SKMBT_C36013013014590.pdf";
                    System.IO.File.Copy(filename, path, true);
                    attachmentList.Add(path);
                    string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                }
                else
                {
                    string path = outputFolder + "SKMBT_C36013013014590.pdf";
                    System.IO.File.Copy(filename, path, true);
                    attachmentList.Add(path);
                    string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + ukClaimDef.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);
                }

                logDef = new UKClaimLogDef(ukClaimDef.ClaimId, "Add Attachment [" + "Signed Copy By COO" + "] (" + "SKMBT_C36013013014590.pdf" + ")", 616, ukClaimDef.WorkflowStatus.Id, ukClaimDef.WorkflowStatus.Id);
                UKClaimManager.Instance.updateUKClaimLogDef(logDef, 616);

            }
            FileUtility.clearFolder(outputFolder, false);
        }

        #endregion
    }
}