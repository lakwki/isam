using System;
using System.Collections;
using System.Threading;
using com.next.infra.configuration.appserver;
using System.Runtime.Remoting.Lifetime;
using com.next.infra.util;
using com.next.isam.reporter.invoice;
using com.next.isam.reporter.shipping;
using com.next.isam.dataserver.worker;
using com.next.isam.appserver.Shipping;
using com.next.isam.reporter.helper;
using com.next.isam.domain.order;
using com.next.infra.persistency.transactions;
using com.next.infra.persistency.dataaccess;
using com.next.isam.domain.types;
using com.next.common.domain.dms;
using com.next.common.web.commander;

namespace com.next.isam.appserver.startup
{
	public class DMSUploadNSInvoiceHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "DMSUploadNSInvoiceHandler";

		class TimeState
		{
			public DateTime lastStartTime;
			public DateTime lastRunTime;
		}

        public DMSUploadNSInvoiceHandler()
		{
		}

		public void Initialize() 
		{
			StartupLoaderFactory.Instance.ProcessLoader(DEFAULT_NAME, new LoadStartupHandler(load));
		}

		private void workOnIt(object state)
		{
            TimeState ts = (TimeState)state;

            if (ts.lastStartTime > ts.lastRunTime ||
                DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 30)
                return;

            ts.lastStartTime = DateTime.Now;
            runNow();
            ts.lastRunTime = DateTime.Now;
        }

		public string handlerName() 
		{
            return DMSUploadNSInvoiceHandler.DEFAULT_NAME;
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
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                MailHelper.sendGeneralMessage("DMS Upload NS Invoice Started", DateTime.Now.ToString());
                string outputFolder = WebConfig.getValue("appSettings", "DMS_UPLOAD_Folder");
                ArrayList amendmentList = new ArrayList();
                ArrayList list = ShippingWorker.Instance.getInvoiceListForDMSUpload();
                foreach (InvoiceDef def in list)
                {

                    string fileName = outputFolder + def.InvoiceNo.Replace("/", "_") + ".pdf";
                    InvoiceRpt rpt = InvoiceReportManager.Instance.getInvoiceReport(ConvertUtility.createArrayList(def.ShipmentId), 99999);
                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, fileName);
                    rpt.Dispose();

                    ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(def.ShipmentId);
                    ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);

                    ArrayList queryStructs = new ArrayList();
                    queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "Shipping - UK Contract"));
                    queryStructs.Add(new QueryStructDef("UK Contract - Delivery", contractDef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString()));
                    ArrayList qList = DMSUtil.queryDocument(queryStructs);
                    long docId = 0;
                    foreach (DocumentInfoDef docInfoDef in qList)
                    {
                        docId = docInfoDef.DocumentID;
                    }

                    if (docId > 0)
                    {
                        ArrayList queryStructs_ForUpdate = new ArrayList();

                        queryStructs_ForUpdate.Add(new QueryStructDef("UK Contract - Delivery", contractDef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString()));

                        string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs_ForUpdate, ConvertUtility.createArrayList(fileName));

                        def.IsUploadDMSDocument = false;
                        def.LastSendDMSDocumentDate = DateTime.Now;
                        ShippingWorker.Instance.updateInvoice(def, ActionHistoryType.SHIPPING_UPDATES, amendmentList, 99999);

                        shipmentDef.ShippingDocWFS = ShippingDocWFS.READY;
                        OrderWorker.Instance.updateShipmentList(ConvertUtility.createArrayList(shipmentDef), 99999);

                        ArrayList splitShipmentList = (ArrayList) OrderSelectWorker.Instance.getSplitShipmentByShipmentId(shipmentDef.ShipmentId);
                        foreach (SplitShipmentDef splitShipmentDef in splitShipmentList)
                        {
                            splitShipmentDef.ShippingDocWFS = ShippingDocWFS.READY;
                        }
                        OrderWorker.Instance.updateSplitShipmentList(splitShipmentList, 99999);
                    }
                }

                MailHelper.sendGeneralMessage("DMS Upload NS Invoice Completed", DateTime.Now.ToString());
                ctx.VoteCommit();                
			}
			catch(Exception e) 
			{
                ctx.VoteRollback();
				MailHelper.sendErrorAlert(e, String.Empty);
			}
            finally
            {
                ctx.Exit();
            }
		}


		#endregion
	}
}