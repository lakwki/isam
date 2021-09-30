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
    public class GenerateUKClaimDCNoteLiveHandler : IStartupHandler
	{
		private static readonly string DEFAULT_NAME = "GenerateUKClaimDCNoteLiveHandler";
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

			if (ts.lastStartTime > ts.lastRunTime || DateTime.Now.Subtract(ts.lastStartTime).TotalMinutes < 90) return;


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

                List<UKClaimDCNoteDef> list = UKClaimWorker.Instance.getUKClaimDCNoteMailList(1);

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
                    
                    this.uploadToDMS(def, fileName);

                    if (vendor.eAdviceAddr != null)
                        emailAddr = vendor.eAdviceAddr.Trim();

                    if (emailAddr != string.Empty)
                    {
                        NoticeHelper.sendUKClaimDCNote(def.OfficeId, vendor, def.DebitCreditIndicator, attachmentList, true, def.CreateUserId);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Next Claim Supplier Email NOT DEFINED");
                        NoticeHelper.sendUKClaimDCNoteMissingSupplierEmail(def.OfficeId, def.PartyName, def.DebitCreditIndicator, attachmentList, true, def.CreateUserId);
                    }

                    def.MailStatus = 2;
                    UKClaimWorker.Instance.updateUKClaimDCNote(def, 99999);
                    
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
		}

        private ArrayList getQueryStringList(int claimId, string docType)
        {
            UKClaimDef claimDef = UKClaimWorker.Instance.getUKClaimByKey(claimId);
            ArrayList queryStructs = new ArrayList();
            ArrayList returnList = new ArrayList();
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();

            queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
            queryStructs.Add(new QueryStructDef("Claim Type", claimDef.Type.DMSDescription));
            queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
            queryStructs.Add(new QueryStructDef("Item No", claimDef.ItemNo));
            queryStructs.Add(new QueryStructDef("Debit Note No", claimDef.UKDebitNoteNo));
            /*
            if (claimDef.ClaimRequestId == -1)
            {
                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                queryStructs.Add(new QueryStructDef("Claim Type", claimDef.Type.DMSDescription));
                queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Next Claim DN"));
                queryStructs.Add(new QueryStructDef("Item No", claimDef.ItemNo));
                queryStructs.Add(new QueryStructDef("Debit Note No", claimDef.UKDebitNoteNo));
            }
            else
            {
                QAIS.ClaimRequestDef def = svc.GetClaimRequestByKey(claimDef.ClaimRequestId);

                queryStructs.Add(new QueryStructDef("DOCUMENTTYPE", "UK Claim"));
                queryStructs.Add(new QueryStructDef("Supporting Doc Type", "Authorization Form"));
                queryStructs.Add(new QueryStructDef("Form No", def.FormNo));
                queryStructs.Add(new QueryStructDef("Item No", def.ItemNo));
                queryStructs.Add(new QueryStructDef("Claim Type", claimDef.Type.DMSDescription));
                if (def.ClaimType == QAIS.ClaimTypeEnum.MFRN)
                    queryStructs.Add(new QueryStructDef("MFRN Month", def.ClaimMonth));
            }
            */

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
                if (claimDef.UKDebitNoteNo != string.Empty)
                    returnList.Add(new QueryStructDef("Debit Note No", claimDef.UKDebitNoteNo));
            }
            return returnList;
        }

        private void uploadToDMS(UKClaimDCNoteDef def, string fileName)
        {
            string docType;
            if (def.DebitCreditIndicator == "D")
                docType = "DN To Supplier";
            else
                docType = "CN To Supplier";

            List<UKClaimDCNoteDetailDef> detailList = UKClaimWorker.Instance.getUKClaimDCNoteDetailListByDCNoteId(def.DCNoteId);
            foreach (UKClaimDCNoteDetailDef detailDef in detailList)
            {
                UKClaimDef ukClaimDef = UKClaimWorker.Instance.getUKClaimByKey(detailDef.ClaimId);
                if (ukClaimDef.HasUKDebitNote)
                {
                    string tsFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string outputFolder = WebConfig.getValue("appSettings", "CLAIM_DOC_FOLDER") + tsFolderName + "\\";
                    if (!System.IO.Directory.Exists(outputFolder))
                        System.IO.Directory.CreateDirectory(outputFolder);

                    ArrayList queryStructs = this.getQueryStringList(detailDef.ClaimId, docType);

                    ArrayList qList = DMSUtil.queryDocument(queryStructs);
                    long docId = 0;
                    ArrayList attachmentList = new ArrayList();

                    foreach (DocumentInfoDef docInfoDef in qList)
                    {
                        docId = docInfoDef.DocumentID;
                        foreach (AttachmentInfoDef attDef in docInfoDef.AttachmentInfos)
                        {
                            byte[] attContent = DMSUtil.getAttachment(attDef.AttachmentID);
                            System.IO.FileStream fs = new System.IO.FileStream(outputFolder + attDef.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                            fs.Write(attContent, 0, attContent.Length);
                            fs.Close();
                            attachmentList.Add(outputFolder + attDef.FileName);
                        }
                    }

                    if (File.Exists(outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower()))
                        File.Delete(outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower());

                    /*
                    if (docId > 0)
                    {
                        string path = outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower();
                        File.Copy(fileName, path);
                        attachmentList.Add(path);
                        string strReturn = DMSUtil.UpdateDocumentWithNewAttachment(docId, queryStructs, attachmentList);
                    }
                    else
                    {
                        string path = outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower();
                        File.Copy(fileName, path);
                        attachmentList.Add(path);
                        string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + detailDef.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);
                    }
                    */

                    string path = outputFolder + Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName).ToLower();
                    File.Copy(fileName, path);
                    attachmentList.Add(path);
                    string msg = DMSUtil.CreateDocument(0, "\\Hong Kong\\Tech\\UK Claim\\", "CLAIMID-" + detailDef.ClaimId.ToString(), "UK Claim", queryStructs, attachmentList);

                    FileUtility.clearFolder(outputFolder, false);
                }
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
            return GenerateUKClaimDCNoteLiveHandler.DEFAULT_NAME;
		}


	}
}
