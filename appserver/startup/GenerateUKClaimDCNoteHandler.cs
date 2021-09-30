using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using com.next.infra.configuration.appserver;
using com.next.infra.util;
using System.Runtime.Remoting.Lifetime;
using com.next.isam.appserver.account;
using com.next.isam.domain.claim;
using com.next.isam.reporter.accounts;
using com.next.common.domain.industry.vendor;
using com.next.isam.dataserver.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain.dms;
using com.next.common.web.commander;
using System.IO;

namespace com.next.isam.appserver.startup
{
    public class GenerateUKClaimDCNoteHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "GenerateUKClaimDCNoteHandler";
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
			int currentHour = DateTime.Now.Hour;

			if (ts.lastStartTime > ts.lastRunTime || DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 15) return;


			ts.lastStartTime=DateTime.Now;
			runNow();
			ts.lastRunTime=DateTime.Now;
		}

		public void runNow() 
		{
            string outputFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder");
            string emailAddr = string.Empty;

            try
            {
                List<UKClaimDCNoteDef> list = UKClaimWorker.Instance.getUKClaimDCNoteMailList(0);

                foreach (UKClaimDCNoteDef def in list)
                {
                    UKClaimDCNoteReport rpt = AccountReportManager.Instance.getUKClaimDCNote(def.DCNoteId);
                    string fileName = outputFolder + def.DCNoteNo.Replace('/', '-') + ".pdf";
                    VendorRef vendor = VendorWorker.Instance.getVendorByKey(def.VendorId);
                    ArrayList attachmentList = new ArrayList();
                    if (def.SettledAmount == def.TotalAmount && !def.IsCustom)
                        attachmentList = GetAttachmentList(def.DCNoteId);

                    rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, fileName);
                    attachmentList.Add(fileName);

                    if (vendor.eAdviceAddr != null)
                        emailAddr = vendor.eAdviceAddr.Trim();

                    if (emailAddr != string.Empty)
                    {
                        NoticeHelper.sendUKClaimDCNote(def.OfficeId, vendor, def.DebitCreditIndicator, attachmentList, false, def.CreateUserId);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Next Claim Supplier Email NOT DEFINED");
                        NoticeHelper.sendUKClaimDCNoteMissingSupplierEmail(def.OfficeId, def.PartyName, def.DebitCreditIndicator, attachmentList, false, def.CreateUserId);
                    }

                    def.MailStatus = 1;
                    UKClaimWorker.Instance.updateUKClaimDCNote(def, 99999);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            } 
		}

        private ArrayList GetAttachmentList(int dcNoteId)
        {
            string outputFolder = WebConfig.getValue("appSettings", "UK_CLAIM_OUTPUT_Folder");
            ArrayList returnList = new ArrayList();
            List<UKClaimDCNoteDetailDef> detailList = UKClaimWorker.Instance.getUKClaimDCNoteDetailListByDCNoteId(dcNoteId);
            foreach (UKClaimDCNoteDetailDef def in detailList)
            {
                if (def.ClaimRefundId == 0 && def.RechargeableAmount != 0)
                {
                    UKClaimDef ukClaimDef = UKClaimWorker.Instance.getUKClaimByKey(def.ClaimId);

                    if (ukClaimDef.HasUKDebitNote)
                    {
                        
                        ArrayList queryStructs = new ArrayList();
                        /*
                        QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
                        QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(ukClaimDef.ClaimRequestId);
                        */

                        queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                        queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
                        queryStructs.Add(new QueryStructDef("Claim Type", ukClaimDef.Type.DMSDescription));
                        queryStructs.Add(new QueryStructDef("Item No", ukClaimDef.ItemNo));
                        queryStructs.Add(new QueryStructDef("Debit Note No", ukClaimDef.UKDebitNoteNo));

                        ArrayList qList = DMSUtil.queryDocument(queryStructs);
                        bool isByQty = false;
                        if (qList.Count > 1)
                        {
                            isByQty = true;
                            queryStructs.Add(new QueryStructDef("Qty", ukClaimDef.Quantity.ToString()));
                            qList = DMSUtil.queryDocument(queryStructs);
                        }

                        DocumentInfoDef docDef = null;
                        foreach (DocumentInfoDef docInfoDef in qList)
                        {
                            docDef = docInfoDef;
                            break;
                        }

                        if (docDef != null)
                        {
                            foreach (AttachmentInfoDef attachInfoDef in docDef.AttachmentInfos)
                            {
                                string fileName = outputFolder + dcNoteId.ToString() + "-" + ukClaimDef.Type.DMSDescription + "-" + ukClaimDef.UKDebitNoteNo.ToString().Replace('/', '-') + (isByQty ? "-" + ukClaimDef.Quantity.ToString() + "PCS" : string.Empty) + ".pdf";
                                File.WriteAllBytes(fileName, DMSUtil.getAttachment(attachInfoDef.AttachmentID));
                                returnList.Add(fileName);
                            }
                        }
                    }
                }
            }
            return returnList;
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
            return GenerateUKClaimDCNoteHandler.DEFAULT_NAME;
		}


	}
}
