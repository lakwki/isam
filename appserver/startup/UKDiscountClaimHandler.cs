using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.domain.claim;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.claim;

namespace com.next.isam.appserver.startup
{
    public class UKDiscountClaimHandler : MailHelper, IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "UKDiscountClaimHandler";
        private static readonly ArrayList timeRepository = new ArrayList();
        private static string styleStr =
          "<LINK type=\"text/css\" rel=\"stylesheet\"> " +
          "<STYLE TYPE=\"text/css\"> " +
          "TD {font-family: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; font-size: 12px;} " +
          ".gridHeader {	FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold;	background-color: #ccccff;} " +
          ".colHeader{ FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold; background-color: #f5f5f5;	text-align: Center; vertical-align : middle; } " +
          ".dataCellStr{ FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold;	text-align: Center;	vertical-align : middle;} " +
          ".dataCellNum{ FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold;	text-align: Right;	vertical-align : middle;} " +
          "span { font-family: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; font-size: 12px;}" +
          "</STYLE>";

        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public void Initialize()
        {
            StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
        }

        //we don't need to use the state here
        private void workOnIt(object state)
        {
            TimeState ts = (TimeState)state;
            int currentHour = DateTime.Now.Hour;
            DayOfWeek day = DateTime.Today.DayOfWeek;

            if (currentHour != 22) return;
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public void runNow()
        {
            try
            {
                List<UKDiscountClaimDef> outstandingList = UKClaimManager.Instance.getOutstandingUKDiscountClaimList();

                foreach (UKDiscountClaimDef def in outstandingList)
                {
                    UKClaimManager.Instance.setDiscountClaimWorkflowStatus(def.ClaimId);
                    UKClaimManager.Instance.setDiscountClaimRefundWorkflowStatus(def.ClaimId);

                    UKDiscountClaimLogDef logDef = new UKDiscountClaimLogDef();
                    logDef.ClaimId = def.ClaimId;
                    logDef.LogText = "Update UK Discount Claim DN";
                    logDef.FromStatusId = UKDiscountClaimWFS.OUTSTANDING.Id;
                    logDef.ToStatusId = UKDiscountClaimWFS.CLEARED.Id;
                    logDef.UserId = 99999;
                    logDef.LogDate = DateTime.Now;
                    UKClaimWorker.Instance.updateUKDiscountClaimLog(logDef, 99999);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        private Timer load(TimeSpan ts)
        {
            TimeState state = new TimeState();
            TimerCallback tcall = new TimerCallback(this.workOnIt);
            Timer timer = new Timer(tcall, state, new TimeSpan(0), ts);
            return timer;
        }

        public string handlerName()
        {
            return UKDiscountClaimHandler.DEFAULT_NAME;
        }

    }
}
