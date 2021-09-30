using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.claim;
using com.next.isam.domain.claim;
using com.next.infra.persistency.transactions;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.appserver.shipping;
using com.next.isam.domain.order;
using System.IO;
using com.next.isam.domain.types;
using com.next.isam.dataserver.worker;


namespace com.next.isam.appserver.startup
{
    public class BIAClearanceHandler : IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "BIAClearanceHandler";
        private QAIS.ClaimRequestService svc = null;
        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public BIAClearanceHandler()
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
            return BIAClearanceHandler.DEFAULT_NAME;
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
            svc = new QAIS.ClaimRequestService();

            this.runClearanceForBIA(3362, 99999);

        }


        private void runClearanceForBIA(int claimId, int userId)
        {
            List<string> childClaimIdList = UKClaimWorker.Instance.getUKClaimBIAMappingList(claimId);
            UKClaimBIADiscrepancyDef discrepancyDef = UKClaimWorker.Instance.getUKClaimBIADiscrepancyByKey(claimId);
            UKClaimDef ukClaimDef = UKClaimWorker.Instance.getUKClaimByKey(claimId);

            if (discrepancyDef.Amount != 0)
            {
                UKClaimDCNoteDef dcNoteDef = new UKClaimDCNoteDef();
                dcNoteDef.DCNoteId = -1;
                dcNoteDef.CreateUserId = userId;
                dcNoteDef.DCNoteDate = DateTime.Today;
                dcNoteDef.IsCustom = false;
                dcNoteDef.Remark = string.Empty;
                dcNoteDef.IsInterfaced = true;
                dcNoteDef.MailStatus = 0;

                dcNoteDef.OfficeId = ukClaimDef.OfficeId;
                dcNoteDef.PartyName = ukClaimDef.Vendor.Name;
                dcNoteDef.PartyAddress1 = ukClaimDef.Vendor.Address0;
                dcNoteDef.PartyAddress2 = ukClaimDef.Vendor.Address1;
                dcNoteDef.PartyAddress3 = ukClaimDef.Vendor.Address2;
                dcNoteDef.PartyAddress4 = ukClaimDef.Vendor.Address3;
                dcNoteDef.CurrencyId = ukClaimDef.Currency.CurrencyId;
                dcNoteDef.TotalAmount = discrepancyDef.Amount;
                dcNoteDef.VendorId = ukClaimDef.Vendor.VendorId;

                if (discrepancyDef.ActionType == BIAActionType.NS_PROVISION || discrepancyDef.ActionType == BIAActionType.NS_COST)
                {
                    dcNoteDef.DCNoteNo = string.Empty;
                    dcNoteDef.DebitCreditIndicator = "D";
                    dcNoteDef.SettledAmount = 0;
                }
                else
                {
                    dcNoteDef.DebitCreditIndicator = (discrepancyDef.ActionType == BIAActionType.SUPPLIER_RECHARGE ? "D" : "C");
                    dcNoteDef.DCNoteNo = AccountWorker.Instance.fillNextAdjustmentNoteNo(AdjustmentType.UKCLAIM, dcNoteDef.DCNoteDate, dcNoteDef.OfficeId, dcNoteDef.DebitCreditIndicator);
                    dcNoteDef.SettledAmount = discrepancyDef.Amount;
                }
                UKClaimWorker.Instance.updateUKClaimDCNote(dcNoteDef, userId);

                UKClaimDCNoteDetailDef detailDef = new UKClaimDCNoteDetailDef();
                detailDef.Amount = dcNoteDef.TotalAmount;
                detailDef.RechargeableAmount = dcNoteDef.SettledAmount;

                detailDef.DCNoteId = dcNoteDef.DCNoteId;
                detailDef.ClaimRefundId = -1;
                detailDef.ClaimId = claimId;
                detailDef.CurrencyId = ukClaimDef.Currency.CurrencyId;
                detailDef.DCNoteDetailId = -1;
                detailDef.LineRemark = string.Empty;

                UKClaimWorker.Instance.updateUKClaimDCNoteDetail(detailDef, userId);

            }

            foreach (string s in childClaimIdList)
            {
                UKClaimManager.Instance.generateFullRefundUKClaimDCNote(int.Parse(s), userId);
            }

        }



        #endregion
    }
}