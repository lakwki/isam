using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;

using System.Runtime.Remoting.Lifetime;

namespace com.next.isam.appserver.startup
{
	public class ILSUploadHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "ILSUploadHandler";
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
			if (DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 15) return;
			ts.lastStartTime=DateTime.Now;
			runNow();	
			ts.lastRunTime=DateTime.Now;
		}

		public void runNow() 
		{
            try
            {
                /*
                MailHelper.sendGeneralMessage("ISAM: ILSUploadXMLManager Start", DateTime.Now.ToString());
                */


                com.next.isam.appserver.ils.ILSUploadManager.Instance.processILSResultFiles();
                com.next.isam.appserver.ils.ILSUploadManager.Instance.sendIncompleteOutgoingILSMsg();
                com.next.isam.appserver.ils.ILSUploadManager.Instance.createQCCApprovalMessage();
                com.next.isam.appserver.ils.ILSUploadManager.Instance.processFiles();
                com.next.isam.appserver.ils.ILSUploadManager.Instance.processNSCFiles();
                com.next.isam.appserver.ils.ILSUploadManager.Instance.createOriginContractMessage("00000");
                
                com.next.isam.appserver.order.UKItemUploadManager.Instance.processFiles();


                //com.next.isam.appserver.ils.ILSUploadManager.Instance.uploadILSPackingListTemplate(string.Empty);
                /*
                MailHelper.sendGeneralMessage("ISAM: ILSUploadXMLManager Completed", DateTime.Now.ToString());
                */
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
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
			return ILSUploadHandler.DEFAULT_NAME;
		}
	}
}
