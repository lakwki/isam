using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using com.next.isam.appserver.order;
using System.Runtime.Remoting.Lifetime;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.isam.appserver.account;
using com.next.common.domain.types;
using com.next.common.web.commander;
using com.next.isam.appserver.common;
using com.next.infra.configuration.appserver;
using com.next.common.datafactory.worker;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.appserver.startup
{
	public class SunInterfaceSubmissionHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "SunInterfaceSubmissionHandler";
		private static readonly ArrayList timeRepository = new ArrayList();
				
		class TimeState 
		{
			public DateTime lastStartTime;
			public DateTime lastRunTime;
		}

		public void Initialize() 
		{
			/*
			for (int i = 0; i<runableHour.Length; i++) 
			{
				timeRepository.Add(runableHour[i]);
			}
			*/
			StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
		}

		//we don't need to use the state here
		private void workOnIt(object state)
		{
			TimeState ts = (TimeState)state;
			/*
			if (DateTime.Now.Hour ==7 && ts.needToRun!=1 && ts.needToRun!=2)
			{
				ts.needToRun=0;
			}
			*/
			if (DateTime.Now.Hour != 8) return;
			if (DateTime.Now.Minute < 45) return;
			if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 60) return;
			/*
			if (ts.needToRun == 0)
			{
				ts.lastStartTime=DateTime.Now;
				ts.needToRun = 1;
				runNow();	
				ts.lastRunTime=DateTime.Now;	
			}
			else if (DateTime.Now.Subtract(ts.lastRunTime).TotalMinutes>2 && ts.needToRun == 1)
			{
				ts.needToRun = 2;
				runReadXML();	
			}
			else
			{
				return;
			}
			*/
			ts.lastStartTime=DateTime.Now;
			runNow();	

			ts.lastRunTime=DateTime.Now;
		}

		public void runNow() 
		{
            /*
            AccountManager.Instance.submitSunInterfaceBatch(1, 2016, 8, SunInterfaceTypeRef.getSunMacroTypeIdList(), 1951);
            AccountManager.Instance.submitSunInterfaceBatch(2, 2016, 8, SunInterfaceTypeRef.getSunMacroTypeIdList(), 1763);

            return;
            */
            
            MailHelper.sendGeneralMessage("Sun Interface Submission Handler Start", DateTime.Now.ToString());

            AccountFinancialCalenderDef calDef = GeneralWorker.Instance.getAccountPeriodByDate(AppId.ISAM.Code, DateTime.Today);
            if (calDef.EndDate.AddDays(-7) == DateTime.Today)
            {
                StartupLoaderFactory.Instance.stopHandler("MockShopSunAccountUploadHandler");
                CommonManager.Instance.disableSystemParam(12, true);
                MailHelper.sendGeneralMessage("MockShopSunAccountUploadHandler has been stopped due to Mock Shop Sales Cut-Off", DateTime.Now.ToString());
            }

            if (calDef.EndDate == DateTime.Today)
            {
                StartupLoaderFactory.Instance.stopHandler("SunInterfaceSubmissionHandler");
                CommonManager.Instance.disableSystemParam(11, true);
                MailHelper.sendGeneralMessage("SunInterfaceSubmissionHandler has been stopped due to Sales Cut-Off", DateTime.Now.ToString());
            }
            else
            {
                SystemParameterRef def = CommonManager.Instance.getSystemParameterByKey(11);
                if (def.ParameterValue != "Y")
                {
                    submitUTOfficeRequests();
                    submitNonUTOfficeRequests();
                }
            }

            MailHelper.sendGeneralMessage("Sun Interface Submission Handler Completed", DateTime.Now.ToString());
        }

        private void submitUTOfficeRequests()
        {
            SunInterfaceQueueDef def = null;
            int[] officeIds;
            officeIds = new int[2] { 1, 2 };

            for (int i = 0; i <= officeIds.GetUpperBound(0); i++)
            {
                def = new SunInterfaceQueueDef();
                def.QueueId = -1;
                def.OfficeGroup = CommonManager.Instance.getReportOfficeGroupByKey(officeIds[i]);
                def.SunInterfaceTypeId = SunInterfaceTypeRef.Id.Purchase.GetHashCode();
                def.CategoryType = CategoryType.DAILY;
                if (officeIds[i] == 1 || officeIds[i] == 17)
                    def.User = CommonUtil.getUserByKey(246); // joey
                else if (officeIds[i] == 2)
                    def.User = CommonUtil.getUserByKey(264); // annie
                def.SourceId = 2;
                def.SubmitTime = DateTime.Now;
                def.FiscalYear = 0;
                def.Period = 0;
                def.PurchaseTerm = 0;
                def.UTurn = 2;
                AccountManager.Instance.submitSunInterfaceRequest(def);

                /*
                def = (SunInterfaceQueueDef)def.Clone();
                def.QueueId = -1;
                def.SubmitTime = DateTime.Now;
                def.UTurn = 2;
                AccountManager.Instance.submitSunInterfaceRequest(def);
                */

                def = (SunInterfaceQueueDef)def.Clone();
                def.QueueId = -1;
                def.CategoryType = CategoryType.REVERSAL;
                def.SubmitTime = DateTime.Now;
                def.UTurn = 2;
                AccountManager.Instance.submitSunInterfaceRequest(def);

                /*
                def = (SunInterfaceQueueDef)def.Clone();
                def.QueueId = -1;
                def.SubmitTime = DateTime.Now;
                def.UTurn = 1;
                AccountManager.Instance.submitSunInterfaceRequest(def);
                */
            }
        }

        private void submitNonUTOfficeRequests()
        {
            SunInterfaceQueueDef def = null;
            int[] officeIds;
            /*
            officeIds = new int[10] { 3, 4, 7, 8, 9, 13, 14, 16, 18, 19 };
            */
            officeIds = new int[7] { 3, 7, 8, 9, 13, 16, 19 };

            for (int i = 0; i <= officeIds.GetUpperBound(0); i++)
            {

                def = new SunInterfaceQueueDef();
                def.QueueId = -1;
                def.OfficeGroup = CommonManager.Instance.getReportOfficeGroupByKey(officeIds[i]);
                def.SunInterfaceTypeId = SunInterfaceTypeRef.Id.Purchase.GetHashCode();
                def.CategoryType = CategoryType.DAILY;
                if (officeIds[i] == 4 || officeIds[i] == 3 || officeIds[i] == 16)
                    def.User = CommonUtil.getUserByKey(264); // annie
                else if (officeIds[i] == 9 || officeIds[i] == 13 || officeIds[i] == 14 || officeIds[i] == 8 || officeIds[i] == 18 || officeIds[i] == 7 || officeIds[i] == 19)
                    def.User = CommonUtil.getUserByKey(1830); // jackson fok
                def.SourceId = 2;
                def.SubmitTime = DateTime.Now;
                def.FiscalYear = 0;
                def.Period = 0;
                def.PurchaseTerm = 0;
                def.UTurn = 0;
                AccountManager.Instance.submitSunInterfaceRequest(def);

                def = (SunInterfaceQueueDef)def.Clone();
                def.QueueId = -1;
                def.CategoryType = CategoryType.REVERSAL;
                def.SubmitTime = DateTime.Now;
                AccountManager.Instance.submitSunInterfaceRequest(def);
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
			return SunInterfaceSubmissionHandler.DEFAULT_NAME;
		}
	}
}
