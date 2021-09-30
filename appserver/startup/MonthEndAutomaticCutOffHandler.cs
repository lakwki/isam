using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using com.next.infra.configuration.appserver;
using System.Runtime.Remoting.Lifetime;
using com.next.infra.util;
using com.next.common.domain;
using com.next.isam.appserver.account;
using com.next.isam.domain.types;
using com.next.common.web.commander;
using com.next.common.appserver;
using com.next.common.domain.types;


namespace com.next.isam.appserver.startup
{
    public class MonthEndAutomaticCutOffHandler : IStartupHandler 
    {
        private static readonly string DEFAULT_NAME = "MonthEndAutomaticCutOffHandler";

        class TimeState
        {
            public DateTime lastStartTime;
            public DateTime lastRunTime;
        }

        public MonthEndAutomaticCutOffHandler()
        {
        }

        public void Initialize()
        {
            StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
        }

        private void workOnIt(object state)
        {
            TimeState ts = (TimeState)state;

            DateTime now = DateTime.Now;
            DateTime actionStartTime = AccountManager.Instance.getAutomaticCutoffStartTime(now);
            DateTime actionEndTime = AccountManager.Instance.getAutomaticCutoffEndTime(now);
            TimeSpan interval = TimeSpan.Parse("00:10:00.000");  // 10 minutes
            DateTime lastSessionTime = actionEndTime.Subtract(interval);
            bool isAnyOutstandingAtTheEnd = (now >= lastSessionTime && now <= actionEndTime && AccountManager.Instance.getOutstandingOfficeForMonthEndClosing().Count > 0);

            if ((now >= actionStartTime && now <= actionEndTime))
            {
                if (ts.lastStartTime != null && now >= ts.lastStartTime.Add(interval) || isAnyOutstandingAtTheEnd)
                {
                    ts.lastStartTime = DateTime.Now;

                    //runNow();
                    startCutoff(isAnyOutstandingAtTheEnd);

                    ts.lastRunTime = DateTime.Now;
                }
            }
        }

        public string handlerName()
        {
            return MonthEndAutomaticCutOffHandler.DEFAULT_NAME;
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
            startCutoff(true);
        }

        private void startCutoff(bool isLastSection)
        {
            try
            {
                AccountFinancialCalenderDef currentPeriod = com.next.common.web.commander.CommonUtil.getAccountPeriodByDate(13, DateTime.Now);
                int fiscalYear = currentPeriod.BudgetYear - (currentPeriod.Period == 1 ? 1 : 0);
                int period = (currentPeriod.Period == 1 ? 12 : currentPeriod.Period - 1);

                MailHelper.sendGeneralMessage("ISAM: Month-End Closing - Automatic Cutoff Handler Started", "Cutoff for " + fiscalYear.ToString() + " P" + period.ToString() + " at " + DateTime.Now.ToString());
                AccountManager.Instance.runAutoSalesCutoff(fiscalYear, period, isLastSection);
                MailHelper.sendGeneralMessage("ISAM: Month-End Closing - Automatic Cutoff Handler Completed", "Completed at " + DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
            }
        }
        #endregion


    }
}
