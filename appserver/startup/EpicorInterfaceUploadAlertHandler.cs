using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using System.Collections.Generic;
using com.next.isam.domain.common;
using com.next.isam.dataserver.worker;

namespace com.next.isam.appserver.startup
{
    public class EpicorInterfaceUploadAlertHandler : MailHelper, IStartupHandler
    {
        private static readonly string DEFAULT_NAME = "EpicorInterfaceUploadAlertHandler";
        private static readonly ArrayList timeRepository = new ArrayList();

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
            //int currentHour = DateTime.Now.Hour;
            //DayOfWeek day = DateTime.Today.DayOfWeek;

            //if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday) return;
            //if (currentHour != 8) return;
            if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 30) return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

        public void runNow()
        {
            try
            {
                ArrayList list = EpicorInterfaceWorker.Instance.getNullImportStatusList();
                if (list.Count > 0)
                    NoticeHelper.sendEpicorUploadImportLogAlert(list);
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
            return EpicorInterfaceUploadAlertHandler.DEFAULT_NAME;
        }


    }
}
