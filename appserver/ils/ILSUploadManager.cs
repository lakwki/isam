using System;
using System.Xml;
using System.IO;
using System.Web;
using System.Collections;
using com.next.infra.persistency.transactions;
using com.next.infra.util;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.ils;
using com.next.isam.domain.order;
using com.next.isam.domain.types;
using com.next.isam.domain.account;
using com.next.isam.domain.shipping;
using com.next.isam.domain.common;
using com.next.common.datafactory.worker;
using com.next.common.domain.types;
using com.next.common.domain;
using System.Collections.Generic;

namespace com.next.isam.appserver.ils
{
    public class ILSUploadManager
    {
        private static ILSUploadManager _instance;

        private ILSUploadWorker ilsUploadWorker;
        private QAISWorker qaisWorker;
        private ShippingWorker shippingWorker;

        private DomainILSOrderCopyDef domainILSOrderCopyDef = null;
        private DomainILSPackingListDef domainILSPackingListDef = null;
        private DomainILSInvoiceDef domainILSInvoiceDef = null;
        private DomainILSCommissionInvoiceDef domainILSCommissionInvoiceDef = null;

        private DateTime importDate = DateTime.MinValue;
        private string fileNo = String.Empty;
        private string fileType = String.Empty;
        private string outputFileNo = String.Empty;
        private string contractNo = String.Empty;
        private string deliveryNo = String.Empty;
        private string containerNo = String.Empty;

        public ILSUploadManager()
        {
            ilsUploadWorker = ILSUploadWorker.Instance;
            qaisWorker = QAISWorker.Instance;
            shippingWorker = ShippingWorker.Instance;
        }

        public static ILSUploadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ILSUploadManager();
                }
                return _instance;
            }
        }

        public void processFiles()
        {
            XmlTextReader reader = null;
            try
            {
                string xmlFolder = WebConfig.getValue("appSettings", "ILS-UPLOAD_XML_Folder");
                string[] xmlFiles;
                xmlFiles = System.IO.Directory.GetFiles(xmlFolder);
                Array.Sort(xmlFiles);

                for (int i = 0; i < xmlFiles.Length; i++)
                {
                    System.IO.FileInfo fi = new FileInfo(xmlFiles[i]);
                    if (fi.Extension.ToLower() == ".xml")
                    {
                        if (System.IO.File.Exists(xmlFiles[i].Replace(".xml", ".dat")))
                            System.IO.File.Delete(xmlFiles[i].Replace(".xml", ".dat"));

                        if (System.IO.File.Exists(xmlFiles[i].Replace(".xml", ".dat")) == false)
                        {
                            string currentFileNo = xmlFiles[i].Substring(16, 5);
                            fileType = xmlFiles[i].Substring(22, 3);

                            if (currentFileNo != ilsUploadWorker.getILSParameter("INPUT_FILE_NO"))
                                return;

                            System.IO.StreamReader r;
                            System.IO.StreamWriter w;

                            r = System.IO.File.OpenText(xmlFiles[i]);
                            System.Text.StringBuilder s;
                            s = new System.Text.StringBuilder(r.ReadToEnd().ToString());
                            r.Close();

                            s.Replace("&", "&amp;");
                            s.Replace("&amp;amp;", "&amp;");

                            w = System.IO.File.CreateText(fi.DirectoryName + "\\" + fi.Name.Replace(".xml", String.Empty) + ".dat");
                            w.Write(s.ToString());
                            w.Flush();
                            w.Close();

                            reader = new XmlTextReader(fi.DirectoryName + "\\" + fi.Name.Replace(".xml", String.Empty) + ".dat");
                            reader.MoveToContent();
                            reader.Read();
                            while (!reader.EOF)
                            {
                                switch (reader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if (reader.Name == "Logistics") processLogistics(reader.ReadOuterXml());
                                        else reader.Read();
                                        break;
                                    default:
                                        reader.Read();
                                        break;
                                }
                            }
                            reader.Close();
                            ilsUploadWorker.updateInputFileNo("INPUT_FILE_NO");
                            System.Diagnostics.Debug.WriteLine(xmlFiles[i]);
                            System.IO.File.Delete(xmlFiles[i]);
                            System.IO.File.Delete(xmlFiles[i].Replace(".xml", ".dat"));
                            System.Diagnostics.Debug.WriteLine("Completed");
                            /*
                            MailHelper.sendGeneralMessage("ILS Message File# [" + fileNo + "] Was Processed Successfully", String.Empty);
                            */
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Duplicate File");
                            MailHelper.sendGeneralMessage("Duplicate ILS Message File# [" + fileNo + "]", String.Empty);
                        }
                    }
                }
                this.cancelOutstandingILSOrders();
                this.resetPackingLists();
                this.uploadOutstandingPackingLists();
                this.resetILSInvoiceUploadStatus();
                this.uploadOutstandingInvoices();
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
                if (reader != null)
                    reader.Close();
            }
        }

        public void processNSCFiles()
        {
            XmlTextReader reader = null;
            try
            {
                createQCCApprovalMessage();

                string xmlFolder = WebConfig.getValue("appSettings", "ILS-UPLOAD_XML_NSC_Folder");
                string[] xmlFiles;
                xmlFiles = System.IO.Directory.GetFiles(xmlFolder);
                Array.Sort(xmlFiles);

                for (int i = 0; i < xmlFiles.Length; i++)
                {
                    System.IO.FileInfo fi = new FileInfo(xmlFiles[i]);
                    if (fi.Extension.ToLower() == ".xml")
                    {
                        if (System.IO.File.Exists(xmlFiles[i].Replace(".xml", ".dat")))
                            System.IO.File.Delete(xmlFiles[i].Replace(".xml", ".dat"));

                        if (System.IO.File.Exists(xmlFiles[i].Replace(".xml", ".dat")) == false)
                        {
                            string currentFileNo = xmlFiles[i].Substring(20, 5);
                            fileType = xmlFiles[i].Substring(26, 3);

                            if (currentFileNo != ilsUploadWorker.getILSParameter("INPUT_NSC_FILE_NO"))
                                return;

                            System.IO.StreamReader r;
                            System.IO.StreamWriter w;

                            r = System.IO.File.OpenText(xmlFiles[i]);
                            System.Text.StringBuilder s;
                            s = new System.Text.StringBuilder(r.ReadToEnd().ToString());
                            r.Close();

                            s.Replace("&", "&amp;");
                            s.Replace("&amp;amp;", "&amp;");

                            w = System.IO.File.CreateText(fi.DirectoryName + "\\" + fi.Name.Replace(".xml", String.Empty) + ".dat");
                            w.Write(s.ToString());
                            w.Flush();
                            w.Close();

                            reader = new XmlTextReader(fi.DirectoryName + "\\" + fi.Name.Replace(".xml", String.Empty) + ".dat");
                            reader.MoveToContent();
                            reader.Read();
                            while (!reader.EOF)
                            {
                                switch (reader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if (reader.Name == "Logistics") processLogistics(reader.ReadOuterXml());
                                        else reader.Read();
                                        break;
                                    default:
                                        reader.Read();
                                        break;
                                }
                            }
                            reader.Close();
                            ilsUploadWorker.updateInputFileNo("INPUT_NSC_FILE_NO");
                            System.Diagnostics.Debug.WriteLine(xmlFiles[i]);
                            System.IO.File.Delete(xmlFiles[i]);
                            System.IO.File.Delete(xmlFiles[i].Replace(".xml", ".dat"));
                            System.Diagnostics.Debug.WriteLine("Completed");
                            MailHelper.sendGeneralMessage("ILS Message File# [" + fileNo + "] Was Processed Successfully", String.Empty);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Duplicate File");
                            MailHelper.sendGeneralMessage("Duplicate ILS Message File# [" + fileNo + "]", String.Empty);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
                if (reader != null)
                    reader.Close();
            }
        }


        public void processILSResultFiles()
        {
            XmlTextReader reader = null;
            try
            {
                fileType = "CTU";
                string xmlFolder = WebConfig.getValue("appSettings", "ILS-UPLOAD_XML_RESULT-Folder");
                string[] xmlFiles;
                xmlFiles = System.IO.Directory.GetFiles(xmlFolder);
                Array.Sort(xmlFiles);

                for (int i = 0; i < xmlFiles.Length; i++)
                {
                    System.IO.FileInfo fi = new FileInfo(xmlFiles[i]);
                    if (fi.Extension.ToLower() == ".xml")
                    {
                        if (System.IO.File.Exists(xmlFiles[i].Replace(".xml", ".dat")) == false)
                        {
                            System.IO.StreamReader r;
                            System.IO.StreamWriter w;

                            r = System.IO.File.OpenText(xmlFiles[i]);
                            System.Text.StringBuilder s;
                            s = new System.Text.StringBuilder(r.ReadToEnd().ToString());
                            r.Close();

                            s.Replace("&", "&amp;");
                            w = System.IO.File.CreateText(fi.DirectoryName + "\\" + fi.Name.Replace(".xml", String.Empty) + ".dat");
                            w.Write(s.ToString());
                            w.Flush();
                            w.Close();

                            reader = new XmlTextReader(fi.DirectoryName + "\\" + fi.Name.Replace(".xml", String.Empty) + ".dat");
                            reader.MoveToContent();
                            reader.Read();
                            while (!reader.EOF)
                            {
                                switch (reader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if (reader.Name == "Header") processHeader(reader.ReadOuterXml());
                                        else if (reader.Name == "GroupLevel")
                                        {
                                            if (reader.GetAttribute("KeyName") == "OrderRef")
                                            {
                                                string tmp = reader.GetAttribute("KeyValue");
                                                contractNo = tmp.Substring(0, tmp.IndexOf("-"));
                                                deliveryNo = tmp.Substring(tmp.IndexOf("-") + 1, 2);
                                                if (reader.GetAttribute("Processed") == "N")
                                                    processErrors(reader.ReadOuterXml());
                                                else
                                                {
                                                    ilsUploadWorker.updateILSMessageResult(this.createILSMessageResultDef("Y", 0));
                                                }
                                            }
                                            reader.Read();
                                        }
                                        else reader.Read();
                                        break;
                                    default:
                                        reader.Read();
                                        break;
                                }
                            }
                            reader.Close();
                            System.Diagnostics.Debug.WriteLine(xmlFiles[i]);
                            System.IO.File.Delete(xmlFiles[i]);
                            System.IO.File.Delete(xmlFiles[i].Replace(".xml", ".dat"));
                            System.Diagnostics.Debug.WriteLine("Completed");
                            ilsUploadWorker.updateILSFileSentLog(fileNo, null, DateTime.MinValue, DateTime.Now);
                            MailHelper.sendGeneralMessage("ILS Result Batch Completed Successfully (" + fileNo + ")", String.Empty);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Duplicate File");
                            MailHelper.sendGeneralMessage("Duplicate ILS Result Batch (" + fileNo + ")", String.Empty);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MailHelper.sendErrorAlert(e, String.Empty);
                if (reader != null)
                    reader.Close();
            }
        }

        private void processErrors(string xml)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                XmlTextReader reader = new XmlTextReader(new StringReader(xml));
                string orderRef = String.Empty;
                string errNo = String.Empty;
                string errDesc = "N/A";
                ILSErrorRef errorRef = null;

                reader.Read();
                while (!reader.EOF)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Key")
                            {
                                if (reader.GetAttribute("Name") == "OrderRef")
                                    orderRef = reader.GetAttribute("Value");
                                reader.Read();
                            }
                            else if (reader.Name == "Err")
                            {
                                errNo = reader.GetAttribute("ErrNbr");
                                errorRef = ilsUploadWorker.getILSErrorByKey(int.Parse(errNo));
                                reader.Read();
                            }
                            else reader.Read();
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                ilsUploadWorker.updateILSMessageResult(this.createILSMessageResultDef("N", int.Parse(errNo)));
                ctx.VoteCommit();
                if (errorRef != null) errDesc = errorRef.Description;
                NoticeHelper.sendILSResultErrorEmail(fileNo, orderRef, errDesc);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void processLogistics(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "ILS") processILS(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
        }

        private void processILS(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OrderCopy")
                        {
                            processOrderCopy(reader.ReadOuterXml());
                            this.createDiscrepancyMessage();
                        }
                        else if (reader.Name == "PackingList") processPackingList(reader.ReadOuterXml());
                        else if (reader.Name == "NSLInvoice") processNSLInvoice(reader.ReadOuterXml());
                        else if (reader.Name == "NSLManifest") processNSLManifest(reader.ReadOuterXml());
                        else if (reader.Name == "ContractStatusUpdate") processContractStatusUpdate(reader.ReadOuterXml());
                        else if (reader.Name == "Documentation") processDocumentation(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
        }

        private void processContractStatusUpdate(string xml)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                XmlTextReader reader = new XmlTextReader(new StringReader(xml));

                reader.Read();
                while (!reader.EOF)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Header") processHeader(reader.ReadOuterXml());
                            else if (reader.Name == "ContractDelivery") processContractStatusContractDelivery(reader.ReadOuterXml());
                            else reader.Read();
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                //finalizeCancelledOrderImport(fileNo);
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void processContractStatusContractDelivery(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string status = String.Empty;
            string reason = String.Empty;
            string invNo = String.Empty;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OrderRef") this.assignOrderRefNo(reader.ReadInnerXml());
                        else if (reader.Name == "NewStatus") status = reader.ReadInnerXml();
                        else if (reader.Name == "Reason") reason = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "InvNbr") invNo = reader.ReadInnerXml();
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            if (status == "09")
            {
                ILSCancelledOrderDef def = new ILSCancelledOrderDef();
                def.ContractNo = contractNo;
                def.DeliveryNo = deliveryNo;
                def.FileNo = fileNo;
                def.ImportDate = importDate;
                def.Status = GeneralStatus.INACTIVE.Code;
                ilsUploadWorker.updateILSCancelledOrder(def);
            }
            if (status == "18")
            {
                /*
                ILSPackingListResetDef def = new ILSPackingListResetDef();
                def.ContractNo = contractNo;
                def.DeliveryNo = deliveryNo;
                def.Reason = reason;
                def.FileNo = fileNo;
                def.ImportDate = importDate;
                ilsUploadWorker.updateILSCancelledOrder(def);
                */
            }
            if (status == "24" || status == "25" || status == "26")
            {
                ILSMonthEndShipmentDef def = new ILSMonthEndShipmentDef();
                def.ContractNo = contractNo;
                def.DeliveryNo = deliveryNo;
                def.InvoiceNo = invNo;
                def.LastStatus = status;
                def.NUKExtractDate = importDate;
                ilsUploadWorker.updateILSMonthEndShipment(def);

                ILSMonthEndLogDef logDef = new ILSMonthEndLogDef();
                logDef.FileNo = fileNo;
                logDef.OrderRefId = def.OrderRefId;
                logDef.ContractNo = contractNo;
                logDef.DeliveryNo = deliveryNo;
                logDef.Status = status;
                logDef.NUKExtractDate = importDate;
                logDef.CreatedOn = DateTime.Now;
                ilsUploadWorker.updateILSMonthEndLog(logDef);
            }
        }

        private void processDocumentation(string xml)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                XmlTextReader reader = new XmlTextReader(new StringReader(xml));

                reader.Read();
                while (!reader.EOF)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Header") processHeader(reader.ReadOuterXml());
                            else if (reader.Name == "ContractDelivery") processDocumentationContractDelivery(reader.ReadOuterXml());
                            else reader.Read();
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void processHeader(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "FileNbr") fileNo = reader.ReadInnerXml().Trim().PadLeft(5, '0');
                        else if (reader.Name == "DateTime")
                        {
                            string tmp;
                            tmp = reader.ReadInnerXml();
                            if (fileType == "NSC")
                                tmp = tmp.Substring(0, 4) + "-" + tmp.Substring(4, 2) + "-" + tmp.Substring(6, 2) + tmp.Substring(8, 3) + ":" + tmp.Substring(11, 2) + ":" + tmp.Substring(13, 2);
                            if (tmp.IndexOf('T') != -1)
                                importDate = DateTime.Parse(tmp.Replace("T", " "));
                            else
                                importDate = DateTime.Parse(tmp.Substring(0, 4) + "-" + tmp.Substring(4, 2) + "-" + tmp.Substring(6, 2));
                        }
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
        }

        private void processDocumentationContractDelivery(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OrderRef") this.assignOrderRefNo(reader.ReadInnerXml());
                        else if (reader.Name == "Document") processDocument(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
        }

        private void processDocument(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string docType = String.Empty;
            string docCountry = String.Empty;
            string docNo = String.Empty;
            string actualType = String.Empty;
            string startDate = String.Empty;
            string expiryDate = String.Empty;
            string quotaCat = String.Empty;
            decimal weight = 0;
            int pieces = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "DocType") docType = reader.ReadInnerXml();
                        else if (reader.Name == "DocCountry") docCountry = reader.ReadInnerXml();
                        else if (reader.Name == "DocNumber") docNo = reader.ReadInnerXml();
                        else if (reader.Name == "DocActualType") actualType = reader.ReadInnerXml();
                        else if (reader.Name == "DocStartDate") startDate = reader.ReadInnerXml();
                        else if (reader.Name == "DocExpDate") expiryDate = reader.ReadInnerXml();
                        else if (reader.Name == "DocQuotaCat") quotaCat = reader.ReadInnerXml();
                        else if (reader.Name == "Weight") weight = decimal.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "Pieces") pieces = int.Parse(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            ILSDocumentDef def = new ILSDocumentDef();
            def.ContractNo = contractNo;
            def.DeliveryNo = deliveryNo;
            def.DocumentType = docType;
            def.DocumentNo = docNo;
            def.DocumentCountry = docCountry;
            def.ActualType = actualType;
            if (startDate == String.Empty)
                def.StartDate = DateTime.MinValue;
            else
                def.StartDate = DateTime.Parse(startDate);
            if (expiryDate == String.Empty)
                def.ExpiryDate = DateTime.MinValue;
            else
                def.ExpiryDate = DateTime.Parse(expiryDate);
            def.QuotaCategory = quotaCat;
            def.Weight = weight;
            def.Pieces = pieces;
            def.FileNo = fileNo;
            def.ImportDate = importDate;
            ilsUploadWorker.updateILSDocument(def);

            ILSOrderRefDef ilsOrderRef = ilsUploadWorker.getILSOrderRefByKey(def.OrderRefId);
            if (ilsOrderRef.ShipmentId != 0)
                shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.DOCUMENT_UPLOAD, "N/A", GeneralStatus.ACTIVE.Code, 99999));
        }

        private void processNSLManifest(string xml)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                XmlTextReader reader = new XmlTextReader(new StringReader(xml));

                reader.Read();
                while (!reader.EOF)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Header") processHeader(reader.ReadOuterXml());
                            else if (reader.Name == "Container") processContainer(reader.ReadOuterXml());
                            else reader.Read();
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void processContainer(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "Confirm") processConfirmContainer(reader.ReadOuterXml());
                        else if (reader.Name == "Cancel") processCancelContainer(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
        }

        private void processCancelContainer(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "NEXTContainerLabel") containerNo = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            ILSManifestDef manifestDef = ilsUploadWorker.getILSManifestByKey(containerNo);
            ArrayList list = ilsUploadWorker.getILSManifestDetailList(containerNo);
            foreach (ILSManifestDetailDef def in list)
            {
                def.IsCancelled = true;
                ilsUploadWorker.updateILSManifestDetail(def);

                ILSOrderRefDef ilsOrderRef = ilsUploadWorker.getILSOrderRefByKey(def.OrderRefId);
                if (ilsOrderRef.ShipmentId != 0)
                    shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.MANIFEST_UPLOAD, "Cancellation Message", GeneralStatus.ACTIVE.Code, 99999));
            }
            if (manifestDef != null)
                ilsUploadWorker.updateILSManifest(manifestDef);
        }

        private void processConfirmContainer(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string voyageNo = String.Empty;
            string vesselName = String.Empty;
            string transportMode = String.Empty;
            string partnerContainerNo = String.Empty;
            string departPort = String.Empty;
            string departDate = String.Empty;
            string arrivalPort = String.Empty;
            string arrivalDate = String.Empty;
            int totalContracts = 0;
            decimal totalVolume = 0;
            int totalPieces = 0;
            int totalCartons = 0;
            string transhipmentIndicator = String.Empty;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "NEXTContainerLabel") containerNo = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "VoyageNbr") voyageNo = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "VoyageName") vesselName = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "TransMode") transportMode = reader.ReadInnerXml();
                        else if (reader.Name == "PartnerContainerNbr") partnerContainerNo = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "DepartLocn") departPort = reader.ReadInnerXml();
                        else if (reader.Name == "DepartDate") departDate = reader.ReadInnerXml();
                        else if (reader.Name == "ArrivalLocn") arrivalPort = reader.ReadInnerXml();
                        else if (reader.Name == "ArrivalDate") arrivalDate = reader.ReadInnerXml();
                        else if (reader.Name == "TranshipInd") transhipmentIndicator = reader.ReadInnerXml();
                        else if (reader.Name == "ContractDelivery") processContainerContractDelivery(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            ILSManifestDef def = new ILSManifestDef();
            def.ContainerNo = containerNo;
            def.VoyageNo = voyageNo;
            def.VesselName = vesselName;
            def.TransportMode = transportMode;
            def.PartnerContainerNo = partnerContainerNo;
            def.DepartPort = departPort;
            def.DepartDate = DateTime.Parse(departDate);
            def.ArrivalPort = arrivalPort;
            def.ArrivalDate = DateTime.Parse(arrivalDate);
            def.IsTranshipment = transhipmentIndicator == "T" ? true : false;
            def.TotalContracts = totalContracts;
            def.TotalVolume = totalVolume;
            def.TotalPieces = totalPieces;
            def.TotalCartons = totalCartons;
            def.FileNo = fileNo;
            def.ImportDate = importDate;
            ilsUploadWorker.updateILSManifest(def);
        }

        private void processContainerContractDelivery(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string ctsDate = String.Empty;
            string sobDate = String.Empty;
            string paDate = String.Empty;
            string position = String.Empty;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OrderRef") this.assignOrderRefNo(reader.ReadInnerXml());
                        else if (reader.Name == "CTSDate") ctsDate = reader.ReadInnerXml();
                        else if (reader.Name == "SOBDate") sobDate = reader.ReadInnerXml();
                        else if (reader.Name == "PADate") paDate = reader.ReadInnerXml();
                        else if (reader.Name == "ContainerPosn") position = reader.ReadInnerXml();
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            ILSManifestDetailDef def = new ILSManifestDetailDef();
            def.ContainerNo = containerNo;
            def.ContractNo = contractNo;
            def.DeliveryNo = deliveryNo;
            if (ctsDate != String.Empty)
                def.ConfirmedToShipDate = DateTime.Parse(ctsDate);
            else
                def.ConfirmedToShipDate = DateTime.MinValue;
            if (sobDate != String.Empty)
                def.ShippedOnBoardDate = DateTime.Parse(sobDate);
            else
                def.ShippedOnBoardDate = DateTime.MinValue;
            if (paDate != String.Empty)
                def.PreAdviceDate = DateTime.Parse(paDate);
            else
                def.PreAdviceDate = DateTime.MinValue;
            def.ContainerPosition = position;
            def.IsCancelled = false;
            ilsUploadWorker.updateILSManifestDetail(def);

            ILSOrderRefDef ilsOrderRef = ilsUploadWorker.getILSOrderRefByKey(def.OrderRefId);
            if (ilsOrderRef.ShipmentId != 0)
                shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.MANIFEST_UPLOAD, "Confirmation Message", GeneralStatus.ACTIVE.Code, 99999));
        }

        private void processOrderCopy(string xml)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                XmlTextReader reader = new XmlTextReader(new StringReader(xml));

                reader.Read();
                while (!reader.EOF)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Header") processHeader(reader.ReadOuterXml());
                            else if (reader.Name == "ContractDelivery") processOrderCopyContractDelivery(reader.ReadOuterXml());
                            else reader.Read();
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void processOrderCopyContractDelivery(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            domainILSOrderCopyDef = new DomainILSOrderCopyDef();

            string itemNo = String.Empty;
            string itemDesc = String.Empty;
            string transportMode = String.Empty;
            string countryOfOrigin = String.Empty;
            string departPort = String.Empty;
            string arrivalPort = String.Empty;
            string franchisePartnerCode = String.Empty;
            string exFactoryDate = String.Empty;
            string inWarehouseDate = String.Empty;
            string supplierCode = String.Empty;
            string supplierName = String.Empty;
            string hangBox = String.Empty;
            string buyingTerms = String.Empty;
            string finalDest = String.Empty;
            string currency = String.Empty;
            int nextPercent = 999;
            int supplierPercent = 999;
            string refurb = "N";

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OrderRef") this.assignOrderRefNo(reader.ReadInnerXml());
                        else if (reader.Name == "ItemNbr") itemNo = reader.ReadInnerXml();
                        else if (reader.Name == "ItemDesc") itemDesc = HttpUtility.HtmlDecode(reader.ReadInnerXml()).Replace("'", "''");
                        else if (reader.Name == "TransMode") transportMode = reader.ReadInnerXml();
                        else if (reader.Name == "OriginCountry") countryOfOrigin = reader.ReadInnerXml();
                        else if (reader.Name == "DepartCountry") departPort = reader.ReadInnerXml();
                        else if (reader.Name == "ArrivalLocn") arrivalPort = reader.ReadInnerXml();
                        else if (reader.Name == "FranchisePartnerCode") franchisePartnerCode = reader.ReadInnerXml();
                        else if (reader.Name == "ExFactDate") exFactoryDate = reader.ReadInnerXml();
                        else if (reader.Name == "IntoWhseDate") inWarehouseDate = reader.ReadInnerXml();
                        else if (reader.Name == "SupplierCode") supplierCode = reader.ReadInnerXml();
                        else if (reader.Name == "SupplierName") supplierName = HttpUtility.HtmlDecode(reader.ReadInnerXml()).Replace("'", "''");
                        else if (reader.Name == "HangBox") hangBox = reader.ReadInnerXml();
                        else if (reader.Name == "BuyingTerms") buyingTerms = reader.ReadInnerXml();
                        else if (reader.Name == "FinalDest")
                        {
                            finalDest = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                            if (finalDest.Trim() != String.Empty)
                            {
                                if (Char.IsNumber(finalDest, 0))
                                    refurb = "Y";
                            }
                        }
                        else if (reader.Name == "Currency") currency = reader.ReadInnerXml();
                        else if (reader.Name == "Option")
                        {
                            processOrderCopyOption(reader.ReadOuterXml());
                        }
                        else if (reader.Name == "FreightSplit")
                        {
                            XmlTextReader subReader = new XmlTextReader(new StringReader(reader.ReadOuterXml()));
                            subReader.Read();
                            while (!subReader.EOF)
                            {
                                switch (subReader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if (subReader.Name == "NEXTPerc") nextPercent = int.Parse(subReader.ReadInnerXml());
                                        else if (subReader.Name == "SupplierPerc") supplierPercent = int.Parse(subReader.ReadInnerXml());
                                        else subReader.Read();
                                        break;
                                    default:
                                        subReader.Read();
                                        break;
                                }
                            }
                        }
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            domainILSOrderCopyDef.OrderCopy.ContractNo = contractNo;
            domainILSOrderCopyDef.OrderCopy.DeliveryNo = deliveryNo;
            domainILSOrderCopyDef.OrderCopy.ItemNo = itemNo;
            domainILSOrderCopyDef.OrderCopy.ItemDesc = itemDesc;
            domainILSOrderCopyDef.OrderCopy.TransportMode = transportMode;
            domainILSOrderCopyDef.OrderCopy.CountryOfOrigin = countryOfOrigin;
            domainILSOrderCopyDef.OrderCopy.DepartCountry = departPort;
            domainILSOrderCopyDef.OrderCopy.ExFactoryDate = DateTime.Parse(exFactoryDate);
            domainILSOrderCopyDef.OrderCopy.InWarehouseDate = DateTime.Parse(inWarehouseDate);
            domainILSOrderCopyDef.OrderCopy.SupplierCode = supplierCode;
            domainILSOrderCopyDef.OrderCopy.SupplierName = supplierName;
            domainILSOrderCopyDef.OrderCopy.HangBox = hangBox;
            domainILSOrderCopyDef.OrderCopy.BuyingTerms = buyingTerms;
            domainILSOrderCopyDef.OrderCopy.FinalDestination = finalDest;
            domainILSOrderCopyDef.OrderCopy.Currency = currency;
            domainILSOrderCopyDef.OrderCopy.NextFreightPercent = nextPercent;
            domainILSOrderCopyDef.OrderCopy.SupplierFreightPercent = supplierPercent;
            domainILSOrderCopyDef.OrderCopy.ArrivalPort = arrivalPort;
            domainILSOrderCopyDef.OrderCopy.FranchisePartnerCode = franchisePartnerCode;
            domainILSOrderCopyDef.OrderCopy.Refurb = refurb;
            domainILSOrderCopyDef.OrderCopy.FileNo = fileNo;
            domainILSOrderCopyDef.OrderCopy.ImportDate = importDate;
            domainILSOrderCopyDef.OrderCopy.IsValid = false;

            ilsUploadWorker.updateILSOrderCopy(domainILSOrderCopyDef.OrderCopy);
            ilsUploadWorker.removeILSOrderCopyDetails(domainILSOrderCopyDef.OrderCopy.OrderRefId);

            foreach (ILSOrderCopyDetailDef def in domainILSOrderCopyDef.OrderCopyDetails)
            {
                def.OrderRefId = domainILSOrderCopyDef.OrderCopy.OrderRefId;
                ilsUploadWorker.updateILSOrderCopyDetail(def);
            }

            ILSOrderRefDef orderRefDef = ilsUploadWorker.getILSOrderRefByKey(domainILSOrderCopyDef.OrderCopy.OrderRefId);
            ILSCancelledOrderDef cancelledOrderDef = ilsUploadWorker.getILSCancelledOrderByKey(domainILSOrderCopyDef.OrderCopy.OrderRefId);

            if (cancelledOrderDef != null && orderRefDef.IsCancelled == true)
            {
                orderRefDef.IsCancelled = false;
                ilsUploadWorker.updateILSOrderRef(orderRefDef);
                if (orderRefDef.ShipmentId != 0)
                {
                    shippingWorker.updateActionHistory(new ActionHistoryDef(orderRefDef.ShipmentId, 0, ActionHistoryType.CONTRACT_REINSTATE, contractNo + "-" + deliveryNo, GeneralStatus.ACTIVE.Code, 99999));
                    NoticeHelper.sendContractReinstateByILSEmail(fileNo, orderRefDef, importDate);
                }
            }

            if (orderRefDef.ShipmentId != 0)
            {
                shippingWorker.updateActionHistory(new ActionHistoryDef(orderRefDef.ShipmentId, 0, ActionHistoryType.ORDER_COPY_UPLOAD, contractNo + "-" + deliveryNo, GeneralStatus.ACTIVE.Code, 99999));
            }
        }

        private void processOrderCopyOption(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            string optionNo = String.Empty;
            string optionDesc = String.Empty;
            decimal price = 0;
            int qty = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OptionNbr") optionNo = reader.ReadInnerXml();
                        else if (reader.Name == "SizeDesc") optionDesc = reader.ReadInnerXml();
                        else if (reader.Name == "OptionQty") qty = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "Price") price = decimal.Parse(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            ILSOrderCopyDetailDef def = new ILSOrderCopyDetailDef();
            def.OptionNo = optionNo;
            def.optionDescription = optionDesc;
            def.Qty = qty;
            def.Price = price;
            domainILSOrderCopyDef.OrderCopyDetails.Add(def);
        }

        private void processPackingList(string xml)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                XmlTextReader reader = new XmlTextReader(new StringReader(xml));

                reader.Read();
                while (!reader.EOF)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Header") processHeader(reader.ReadOuterXml());
                            else if (reader.Name == "ContractDelivery") processPackingListContractDelivery(reader.ReadOuterXml());
                            else reader.Read();
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void processPackingListContractDelivery(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            domainILSPackingListDef = new DomainILSPackingListDef();

            string itemNo = String.Empty;
            string itemDesc = String.Empty;
            string transportMode = String.Empty;
            string countryOfOrigin = String.Empty;
            string departPort = String.Empty;
            string arrivalPort = String.Empty;
            string franchisePartnerCode = String.Empty;
            string exFactoryDate = String.Empty;
            string inWarehouseDate = String.Empty;
            string supplierCode = String.Empty;
            string supplierName = String.Empty;
            string supplierInvoiceNo = String.Empty;
            string hangBox = String.Empty;
            string buyingTerms = String.Empty;
            string finalDest = String.Empty;
            string prepaidFreightCosts = String.Empty;
            string handoverDate = String.Empty;
            string vendorLoaded = String.Empty;
            int totalPieces = 0;
            int totalCartons = 0;
            decimal totalNetWeight = 0;
            decimal totalGrossWeight = 0;
            decimal totalVolume = 0;
            string nslDeliveryNo = String.Empty;
            string refurb = "N";
            bool cartonInfo = false;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OrderRef") assignOrderRefNo(reader.ReadInnerXml());
                        else if (reader.Name == "ItemNbr") itemNo = reader.ReadInnerXml();
                        else if (reader.Name == "ItemDesc") itemDesc = HttpUtility.HtmlDecode(reader.ReadInnerXml()).Replace("'", "''");
                        else if (reader.Name == "TransMode") transportMode = reader.ReadInnerXml();
                        else if (reader.Name == "OriginCountry") countryOfOrigin = reader.ReadInnerXml();
                        else if (reader.Name == "DepartPort") departPort = reader.ReadInnerXml();
                        else if (reader.Name == "ArrivalLocn") arrivalPort = reader.ReadInnerXml();
                        else if (reader.Name == "FranchisePartnerCode") franchisePartnerCode = reader.ReadInnerXml();
                        else if (reader.Name == "ExFactDate") exFactoryDate = reader.ReadInnerXml();
                        else if (reader.Name == "ArrivedAtPartDt") handoverDate = reader.ReadInnerXml();
                        else if (reader.Name == "IntoWhseDate") inWarehouseDate = reader.ReadInnerXml();
                        else if (reader.Name == "SupplierCode") supplierCode = reader.ReadInnerXml();
                        else if (reader.Name == "SupplierName") supplierName = HttpUtility.HtmlDecode(reader.ReadInnerXml()).Replace("'", "''");
                        else if (reader.Name == "SupplierInvNo") supplierInvoiceNo = reader.ReadInnerXml();
                        else if (reader.Name == "HangBox") hangBox = reader.ReadInnerXml();
                        else if (reader.Name == "BuyingTerms") buyingTerms = reader.ReadInnerXml();
                        else if (reader.Name == "FinalDest")
                        {
                            finalDest = reader.ReadInnerXml();
                            if (finalDest.Trim() != String.Empty)
                            {
                                if (Char.IsNumber(finalDest, 0))
                                    refurb = "Y";
                            }
                        }
                        else if (reader.Name == "PrePaidFreightCosts") prepaidFreightCosts = reader.ReadInnerXml();
                        else if (reader.Name == "OriginContractSeq")
                        {
                            string tmp;
                            tmp = reader.ReadInnerXml();
                            tmp = tmp.Trim();
                            if (tmp != "TB" && tmp != "NA" && tmp != String.Empty && tmp != "N/")
                            {
                                nslDeliveryNo = tmp;
                            }
                        }
                        else if (reader.Name == "VendorLoaded") vendorLoaded = reader.ReadInnerXml();
                        else if (reader.Name == "TotalPieces") totalPieces = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "TotalCartons") totalCartons = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "TotalNetWeight") totalNetWeight = decimal.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "TotalGrossWeight") totalGrossWeight = decimal.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "TotalVolume") totalVolume = decimal.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "Option")
                        {
                            processPackingListOption(reader.ReadOuterXml(), hangBox);
                        }
                        else if (reader.Name == "MixedCarton")
                        {
                            cartonInfo = true;
                            processPackingListMixedCarton(reader.ReadOuterXml());
                        }
                        else if (reader.Name == "PrePack")
                        {
                            XmlTextReader subReader = new XmlTextReader(new StringReader(reader.ReadOuterXml()));
                            subReader.Read();
                            while (!subReader.EOF)
                            {
                                switch (subReader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if (subReader.Name == "POption") processPackingListPrePackOption(subReader.ReadOuterXml());
                                        else subReader.Read();
                                        break;
                                    default:
                                        subReader.Read();
                                        break;
                                }
                            }
                        }
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            /*
            if (cartonInfo == false && hangBox == "B")
            {
                NoticeHelper.sendPackingListMissingBoxInfoEmail(fileNo, contractNo + "-" + deliveryNo, "N/A");
            }
            */

            domainILSPackingListDef.NSLDeliveryNo = (nslDeliveryNo == String.Empty ? int.Parse(deliveryNo) : int.Parse(nslDeliveryNo));
            domainILSPackingListDef.PackingList.ContractNo = contractNo;
            domainILSPackingListDef.PackingList.DeliveryNo = deliveryNo;
            domainILSPackingListDef.PackingList.ItemNo = itemNo;
            domainILSPackingListDef.PackingList.ItemDesc = itemDesc;
            domainILSPackingListDef.PackingList.TransportMode = transportMode;
            domainILSPackingListDef.PackingList.CountryOfOrigin = countryOfOrigin;
            domainILSPackingListDef.PackingList.DepartPort = departPort;
            domainILSPackingListDef.PackingList.ExFactoryDate = DateTime.Parse(exFactoryDate);
            domainILSPackingListDef.PackingList.InWarehouseDate = DateTime.Parse(inWarehouseDate);
            domainILSPackingListDef.PackingList.SupplierCode = supplierCode;
            domainILSPackingListDef.PackingList.SupplierName = supplierName;
            domainILSPackingListDef.PackingList.HangBox = hangBox;
            domainILSPackingListDef.PackingList.BuyingTerms = buyingTerms;
            domainILSPackingListDef.PackingList.FinalDestination = finalDest;
            domainILSPackingListDef.PackingList.PrepaidFreightCost = prepaidFreightCosts;
            if (handoverDate != String.Empty)
                domainILSPackingListDef.PackingList.HandoverDate = DateTime.Parse(handoverDate);
            else
                domainILSPackingListDef.PackingList.HandoverDate = DateTime.MinValue;
            domainILSPackingListDef.PackingList.VendorLoaded = vendorLoaded;
            domainILSPackingListDef.PackingList.ArrivalPort = arrivalPort;
            domainILSPackingListDef.PackingList.FranchisePartnerCode = franchisePartnerCode;
            domainILSPackingListDef.PackingList.Refurb = refurb;
            domainILSPackingListDef.PackingList.TotalPieces = totalPieces;
            domainILSPackingListDef.PackingList.TotalCartons = totalCartons;
            domainILSPackingListDef.PackingList.TotalGrossWeight = totalGrossWeight;
            domainILSPackingListDef.PackingList.TotalNetWeight = totalNetWeight;
            domainILSPackingListDef.PackingList.TotalVolume = totalVolume;
            domainILSPackingListDef.PackingList.FileNo = fileNo;
            domainILSPackingListDef.PackingList.ImportDate = importDate;
            domainILSPackingListDef.PackingList.NSLDeliveryNo = (nslDeliveryNo == String.Empty ? 0 : int.Parse(nslDeliveryNo));

            ilsUploadWorker.updateILSPackingList(domainILSPackingListDef.PackingList, domainILSPackingListDef.NSLDeliveryNo);
            ilsUploadWorker.removeILSPackingListDetails(domainILSPackingListDef.PackingList.OrderRefId);
            ilsUploadWorker.removeILSPackingListMixedCartonDetails(domainILSPackingListDef.PackingList.OrderRefId);
            ilsUploadWorker.removeILSPackingListCartonDetails(domainILSPackingListDef.PackingList.OrderRefId);

            foreach (ILSPackingListDetailDef def in domainILSPackingListDef.PackingListDetails)
            {
                def.OrderRefId = domainILSPackingListDef.PackingList.OrderRefId;
                ilsUploadWorker.updateILSPackingListDetail(def);
            }
            foreach (ILSPackingListMixedCartonDetailDef def in domainILSPackingListDef.PackingListMixedCartonDetails)
            {
                def.OrderRefId = domainILSPackingListDef.PackingList.OrderRefId;
                ilsUploadWorker.updateILSPackingListMixedCartonDetail(def);
            }
            foreach (ILSPackingListCartonDetailDef def in domainILSPackingListDef.PackingListCartonDetails)
            {
                def.OrderRefId = domainILSPackingListDef.PackingList.OrderRefId;
                ilsUploadWorker.updateILSPackingListCartonDetail(def);
            }
        }

        private void processPackingListOption(string xml, string hangBox)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string optionNo = String.Empty;
            string optionDesc = String.Empty;
            int qty = 0;
            decimal weight = 0;
            decimal volume = 0;
            bool cartonInfo = false;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OptionNbr") optionNo = reader.ReadInnerXml();
                        else if (reader.Name == "SizeDesc") optionDesc = reader.ReadInnerXml();
                        else if (reader.Name == "OptionQty") qty = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "Weight") weight = decimal.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "Volume") volume = decimal.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "Cartons")
                        {
                            cartonInfo = true;
                            processPackingListOptionCarton(reader.ReadOuterXml(), optionNo);
                        }
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            if (optionNo != "00")
            {
                ILSPackingListDetailDef def = new ILSPackingListDetailDef();
                def.OptionNo = optionNo;
                def.optionDescription = optionDesc;
                def.Qty = qty;
                def.Weight = weight;
                def.Volume = volume;
                domainILSPackingListDef.PackingListDetails.Add(def);
            }

            /* 2016-12-14, UK sends mixed carton info instead
            if (cartonInfo == false && hangBox == "B")
            {
                NoticeHelper.sendPackingListMissingBoxInfoEmail(fileNo, contractNo + "-" + deliveryNo, optionNo);
            }
            */
        }

        private void processPackingListMixedCarton(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            int cartonId = 0;
            string cartonSize = String.Empty;
            int cartonLength = 0;
            int cartonWidth = 0;
            int cartonHeight = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "CartonID") cartonId = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "BDCMType") cartonSize = reader.ReadInnerXml();
                        else if (reader.Name == "CartonLength") cartonLength = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "CartonWidth") cartonWidth = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "CartonHeight") cartonHeight = int.Parse(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            ILSPackingListMixedCartonDetailDef def = new ILSPackingListMixedCartonDetailDef();
            def.CartonId = cartonId;
            def.CartonSize = cartonSize;
            def.CartonLength = cartonLength;
            def.CartonWidth = cartonWidth;
            def.CartonHeight = cartonHeight;
            domainILSPackingListDef.PackingListMixedCartonDetails.Add(def);
        }

        private void processPackingListOptionCarton(string xml, string optionNo)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string cartonSize = String.Empty;
            int cartonLength = 0;
            int cartonWidth = 0;
            int cartonHeight = 0;
            int pieces = 0;
            int noOfCartons = 0;
            int firstCarton = 0;
            int lastCarton = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "BDCMType") cartonSize = reader.ReadInnerXml();
                        else if (reader.Name == "CartonLength") cartonLength = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "CartonWidth") cartonWidth = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "CartonHeight") cartonHeight = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "Pieces") pieces = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "NbrOfCartons") noOfCartons = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "FirstCarton") firstCarton = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "LastCarton") lastCarton = int.Parse(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            ILSPackingListCartonDetailDef def = new ILSPackingListCartonDetailDef();
            def.OptionNo = optionNo;
            def.SeqNo = -1;
            def.CartonSize = cartonSize;
            def.CartonLength = cartonLength;
            def.CartonWidth = cartonWidth;
            def.CartonHeight = cartonHeight;
            def.Pieces = pieces;
            def.NoOfCartons = noOfCartons;
            def.FirstCarton = firstCarton;
            def.LastCarton = lastCarton;
            domainILSPackingListDef.PackingListCartonDetails.Add(def);
        }

        private void processPackingListPrePackOption(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string optionNo = String.Empty;
            int qty = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OptionNbr") optionNo = reader.ReadInnerXml();
                        else if (reader.Name == "OptionQty") qty = int.Parse(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            ILSPackingListDetailDef def = new ILSPackingListDetailDef();
            def.OptionNo = optionNo;
            def.optionDescription = "PRE-PACK RATIO";
            def.Qty = qty;
            def.Weight = -1;
            def.Volume = -1;
            domainILSPackingListDef.PackingListDetails.Add(def);
        }

        private void processNSLInvoice(string xml)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                XmlTextReader reader = new XmlTextReader(new StringReader(xml));
                reader.Read();
                while (!reader.EOF)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Header") processHeader(reader.ReadOuterXml());
                            else if (reader.Name == "Invoice") processInvoice(reader.ReadOuterXml());
                            else reader.Read();
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void processInvoice(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "InvoiceHeader") processInvoiceHeader(reader.ReadOuterXml());
                        else if (reader.Name == "InvoiceLines") processInvoiceLines(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            if (fileType == "NSI")
            {
                ilsUploadWorker.updateILSInvoice(domainILSInvoiceDef.Invoice);
                ilsUploadWorker.removeILSInvoiceDetails(domainILSInvoiceDef.Invoice.OrderRefId);
                foreach (ILSInvoiceDetailDef def in domainILSInvoiceDef.InvoiceDetails)
                {
                    def.OrderRefId = domainILSInvoiceDef.Invoice.OrderRefId;
                    ilsUploadWorker.updateILSInvoiceDetail(def);
                }

                /*
                if (domainILSInvoiceDef.Invoice.TotalAmount >= 0)
                {
                    ilsUploadWorker.updateILSInvoice(domainILSInvoiceDef.Invoice);
                    ilsUploadWorker.removeILSInvoiceDetails(domainILSInvoiceDef.Invoice.OrderRefId);
                    foreach (ILSInvoiceDetailDef def in domainILSInvoiceDef.InvoiceDetails)
                    {
                        def.OrderRefId = domainILSInvoiceDef.Invoice.OrderRefId;
                        ilsUploadWorker.updateILSInvoiceDetail(def);
                    }
                }
                else
                {
                    ilsUploadWorker.updateILSCancelledInvoice(ilsUploadWorker.convertToILSCancelledInvoice(domainILSInvoiceDef.Invoice));
                    ilsUploadWorker.removeILSCancelledInvoiceDetails(domainILSInvoiceDef.Invoice.OrderRefId);
                    foreach (ILSInvoiceDetailDef def in domainILSInvoiceDef.InvoiceDetails)
                    {
                        def.OrderRefId = domainILSInvoiceDef.Invoice.OrderRefId;
                        ilsUploadWorker.updateILSCancelledInvoiceDetail(ilsUploadWorker.convertToILSCancelledInvoiceDetail(def));
                    }
                }
                */
            }
            else if (fileType == "NSC")
            {
                ilsUploadWorker.updateILSCommissionInvoice(domainILSCommissionInvoiceDef.Invoice);
                ilsUploadWorker.removeILSCommissionInvoiceDetails(domainILSCommissionInvoiceDef.Invoice.OrderRefId);
                foreach (ILSCommissionInvoiceDetailDef def in domainILSCommissionInvoiceDef.InvoiceDetails)
                {
                    def.OrderRefId = domainILSCommissionInvoiceDef.Invoice.OrderRefId;
                    ilsUploadWorker.updateILSCommissionInvoiceDetail(def);
                }
            }
        }

        private void processInvoiceHeader(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string itemNo = String.Empty;
            string supplierCode = String.Empty;
            string invoiceNo = String.Empty;
            string currencyCode = String.Empty;
            string invoiceDate = String.Empty;
            decimal totalVAT = 0;
            decimal totalAmt = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OrderRef") assignOrderRefNo(reader.ReadInnerXml());
                        else if (reader.Name == "ItemNbr") itemNo = reader.ReadInnerXml();
                        else if (reader.Name == "Currency") currencyCode = reader.ReadInnerXml();
                        else if (reader.Name == "InvDate") invoiceDate = reader.ReadInnerXml();
                        else if (reader.Name == "SupplierInvNbr") invoiceNo = reader.ReadInnerXml();
                        else if (reader.Name == "SupplierCode") supplierCode = reader.ReadInnerXml();
                        else if (reader.Name == "TotalVat" || reader.Name == "TotalVAT") totalVAT = decimal.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "InvTotal") totalAmt = decimal.Parse(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            if (fileType == "NSI")
            {
                domainILSInvoiceDef = new DomainILSInvoiceDef();
                domainILSInvoiceDef.Invoice.ContractNo = contractNo;
                domainILSInvoiceDef.Invoice.DeliveryNo = deliveryNo;
                domainILSInvoiceDef.Invoice.ItemNo = itemNo;
                domainILSInvoiceDef.Invoice.SupplierCode = supplierCode;
                domainILSInvoiceDef.Invoice.Currency = currencyCode;
                domainILSInvoiceDef.Invoice.InvoiceNo = invoiceNo;
                domainILSInvoiceDef.Invoice.InvoiceDate = DateTime.Parse(invoiceDate);
                domainILSInvoiceDef.Invoice.TotalQty = 0;
                domainILSInvoiceDef.Invoice.TotalVAT = totalVAT;
                domainILSInvoiceDef.Invoice.TotalAmount = totalAmt;
                domainILSInvoiceDef.Invoice.FileNo = fileNo;
                domainILSInvoiceDef.Invoice.ImportDate = importDate;
                domainILSInvoiceDef.Invoice.Status = -1;
            }
            else if (fileType == "NSC")
            {
                domainILSCommissionInvoiceDef = new DomainILSCommissionInvoiceDef();
                domainILSCommissionInvoiceDef.Invoice.ItemNo = itemNo;
                domainILSCommissionInvoiceDef.Invoice.SupplierCode = supplierCode;
                domainILSCommissionInvoiceDef.Invoice.Currency = currencyCode;
                domainILSCommissionInvoiceDef.Invoice.InvoiceNo = invoiceNo;
                domainILSCommissionInvoiceDef.Invoice.InvoiceDate = DateTime.Parse(invoiceDate);
                domainILSCommissionInvoiceDef.Invoice.TotalVAT = totalVAT;
                domainILSCommissionInvoiceDef.Invoice.TotalAmount = totalAmt;
                domainILSCommissionInvoiceDef.Invoice.FileNo = fileNo;
                domainILSCommissionInvoiceDef.Invoice.ImportDate = importDate;
                domainILSCommissionInvoiceDef.Invoice.Status = -1;
            }
        }

        private void processInvoiceLines(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "Line") processLine(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;

                }
            }
        }

        private void processLine(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "Stock") processStock(reader.ReadOuterXml());
                        else if (reader.Name == "NonStock") processNonStock(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
        }

        private void processStock(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string optionNo = String.Empty;
            string vatCode = String.Empty;
            int qty = 0;
            decimal price = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "OptionNbr") optionNo = reader.ReadInnerXml();
                        else if (reader.Name == "VATCode") vatCode = reader.ReadInnerXml();
                        else if (reader.Name == "Pieces") qty = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "SupplierPrice") price = decimal.Parse(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            ILSInvoiceDetailDef def = new ILSInvoiceDetailDef();
            def.OptionNo = optionNo;
            def.Qty = qty;
            def.Price = price;
            def.VATCode = vatCode;
            domainILSInvoiceDef.InvoiceDetails.Add(def);
            domainILSInvoiceDef.Invoice.TotalQty += qty;
        }

        private void processNonStock(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string desc = String.Empty;
            string vatCode = String.Empty;
            decimal netValue = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "Desc") desc = reader.ReadInnerXml();
                        else if (reader.Name == "NetValue") netValue = decimal.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "VatCode") vatCode = reader.ReadInnerXml();
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            ILSCommissionInvoiceDetailDef def = new ILSCommissionInvoiceDetailDef();
            def.SeqId = -1;
            def.LineDescription = desc;
            def.Amount = netValue;
            def.VATCode = vatCode;
            domainILSCommissionInvoiceDef.InvoiceDetails.Add(def);
        }

        public void sendIncompleteOutgoingILSMsg()
        {
            System.Diagnostics.Debug.WriteLine("Sending Incomplete ILS Outgoing Msg");

            ArrayList list = ilsUploadWorker.getOutstandingILSFileSentLog();
            if (list.Count > 0)
            {
                StringWriter sw = new StringWriter();
                string msgType = String.Empty;
                sw.WriteLine("Log Time : " + DateTime.Now.ToString());
                sw.WriteLine(String.Empty);

                sw.WriteLine("[Seq.  File No / Msg Type / Time Created / Minutes Since]");
                sw.WriteLine(String.Empty);

                int i = 1;
                TimeSpan ts;
                foreach (ILSFileSentLogDef def in list)
                {
                    if (def.Type == "QCC")
                        msgType = "QCC Approval";
                    else if (def.Type == "DIS")
                        msgType = "Discrepancy";
                    else if (def.Type == "ORI")
                        msgType = "Origin Contract Update";

                    ts = DateTime.Now.Subtract(def.CreatedOn);
                    sw.WriteLine(i.ToString() + ". " + def.FileNo.ToString() + " / " + msgType + " / " + def.CreatedOn.ToString() + " / " + Convert.ToInt32(ts.TotalMinutes).ToString());
                    i = i + 1;
                }
                NoticeHelper.sendIncompleteOutgoingILSMsgEmail(sw.ToString());
            }

        }

        public void createQCCApprovalMessage()
        {
            System.Diagnostics.Debug.WriteLine("Creating QCC Approval File");
            ArrayList list = ilsUploadWorker.getILSQCCApprovalList();

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                if (list.Count > 0)
                {
                    XmlTextWriter w = this.getOutputTextWriter();

                    foreach (ILSQCCApprovalDef def in list)
                    {
                        w.WriteStartElement("ContractDelivery");
                        w.WriteElementString("OrderRef", def.OrderRef);
                        w.WriteElementString("NewStatus", "04"); // Called-To-QCC
                        w.WriteElementString("DateTime1", def.StartTime.AddHours(5).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.0000000"));
                        w.WriteEndElement();

                        w.WriteStartElement("ContractDelivery");
                        w.WriteElementString("OrderRef", def.OrderRef);
                        w.WriteElementString("NewStatus", def.Status);
                        w.WriteElementString("DateTime1", def.CompletedTime.AddHours(5).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.0000000"));
                        w.WriteEndElement();

                        qaisWorker.updateInspection(def.ContractId, def.InspectionId);
                        ilsUploadWorker.updateILSMessageResult(this.createILSMessageResultDef(def.OrderRefId, "QCC"));

                        if (def.ShipmentId != -1)
                            shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.QCC_MSG, def.Status == "05" ? "PASS MESSAGE" : "FAIL MESSAGE", GeneralStatus.ACTIVE.Code, 99999));
                    }

                    this.closeOutputTextWriter(w);
                    ilsUploadWorker.updateILSFileSentLog(outputFileNo, "QCC", DateTime.Now, DateTime.MinValue);
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                NoticeHelper.sendGeneralMessage("QCC - ILS File Generation Failed", string.Empty);
                throw new ApplicationException("QCC - ILS File Generation Failed.");
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void createDiscrepancyMessage()
        {
            System.Diagnostics.Debug.WriteLine("Creating Order Copy Discrepany File");
            ArrayList list = ilsUploadWorker.getILSOrderCopyDiscrepancyList(fileNo);

            if (list.Count > 0)
            {
                XmlTextWriter w = this.getOutputTextWriter();
                string orderRef = String.Empty;
                bool isFirstOrder = false;
                bool isFirstPriceDis = false;
                bool isFirstOption = false;

                foreach (ILSOrderCopyDiscrepancyDef def in list)
                {
                    if (def.OrderRef != orderRef)
                    {
                        orderRef = def.OrderRef;
                        if (!isFirstOrder)
                            isFirstOrder = true;
                        else
                        {
                            if (isFirstPriceDis)
                            {
                                w.WriteEndElement(); //Price Check
                            }
                            w.WriteEndElement(); //Discrepancy
                            w.WriteEndElement(); //ContractDelivery
                        }

                        w.WriteStartElement("ContractDelivery");
                        w.WriteElementString("OrderRef", orderRef);
                        w.WriteElementString("NewStatus", "15");
                        w.WriteStartElement("Discrepancy");
                        isFirstPriceDis = false;
                        isFirstOption = false;
                        ilsUploadWorker.updateILSMessageResult(this.createILSMessageResultDef(def.OrderRefId, "DIS"));
                        shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.DISCREPANCY_MSG, "N/A", GeneralStatus.ACTIVE.Code, 99999));
                    }

                    if (!isFirstOption)
                    {
                        isFirstOption = true;
                        if (def.ILSTransportMode != def.TransportMode)
                        {
                            w.WriteStartElement("MOTCheck");
                            w.WriteElementString("NEXTTransMode", def.ILSTransportMode);
                            w.WriteElementString("NSLTransMode", def.TransportMode);
                            w.WriteEndElement(); //MOTCheck
                        }

                        if ((def.ILSNextPercent != def.NextPercent || def.ILSSupplierPercent != def.SupplierPercent) && def.ILSTransportMode == def.TransportMode)
                        {
                            w.WriteStartElement("FreightCheck");
                            w.WriteElementString("NEXTPerc", def.ILSNextPercent.ToString());
                            w.WriteElementString("NSLNEXTPerc", def.NextPercent.ToString());
                            w.WriteElementString("SupplierPerc", def.ILSSupplierPercent.ToString());
                            w.WriteElementString("NSLSupplierPerc", def.SupplierPercent.ToString());
                            w.WriteEndElement(); //FreightCheck
                        }
                    }

                    if (def.SellingPrice != def.ILSSellingPrice)
                    {
                        if (!isFirstPriceDis)
                        {
                            isFirstPriceDis = true;

                            w.WriteStartElement("PriceCheck");
                            w.WriteElementString("NSLCurrency", def.CurrencyCode);
                        }
                        w.WriteStartElement("DisOption");
                        w.WriteElementString("OptionNbr", def.OptionNo);
                        if (def.ILSSellingPrice == 0)
                            w.WriteElementString("NEXTPrice", "0");
                        else
                            w.WriteElementString("NEXTPrice", XmlConvert.ToString(def.ILSSellingPrice));
                        if (def.SellingPrice == 0)
                            w.WriteElementString("NSLPrice", "0");
                        else
                            w.WriteElementString("NSLPrice", XmlConvert.ToString(def.SellingPrice));
                        w.WriteEndElement(); //DisOption
                    }
                }
                this.closeOutputTextWriter(w);
                ilsUploadWorker.updateILSFileSentLog(outputFileNo, "DIS", DateTime.Now, DateTime.MinValue);
            }
        }

        public void createOriginContractMessage(string fileNo)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList amendmentList;

                if (fileNo != "MISSING")
                {
                    amendmentList = ilsUploadWorker.getILSOrderCopyWithOfficeAmendmentList();
                    foreach (ILSOrderCopyDef def in amendmentList)
                    {
                        def.IsValid = false;
                        ilsUploadWorker.updateILSOrderCopy(def);
                    }

                    amendmentList = ilsUploadWorker.getILSOrderCopyWithPortAmendmentList();
                    foreach (ILSOrderCopyDef def in amendmentList)
                    {
                        def.IsValid = false;
                        ilsUploadWorker.updateILSOrderCopy(def);
                    }

                    amendmentList = ilsUploadWorker.getILSOrderCopyWithOriginCountryAmendmentList();
                    foreach (ILSOrderCopyDef def in amendmentList)
                    {
                        def.IsValid = false;
                        ilsUploadWorker.updateILSOrderCopy(def);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Creating Order Copy Origin Contract File");
                ArrayList list = null;
                if (fileNo != "MISSING")
                    list = ilsUploadWorker.getILSOrderCopyOriginList(fileNo);
                else
                    list = ilsUploadWorker.getMissingILSOrderCopyOriginList();

                string delimiter = ",";
                string[] docs = null;
                string[] cats = null;
                if (list.Count > 0)
                {
                    XmlTextWriter w = this.getOutputTextWriter();

                    foreach (ILSOrderCopyOriginDef def in list)
                    {
                        w.WriteStartElement("ContractDelivery");
                        w.WriteElementString("OrderRef", def.OrderRef);
                        w.WriteElementString("NewStatus", "16");
                        w.WriteStartElement("OriginDetails");
                        w.WriteElementString("OriginContract", def.OriginContract);
                        w.WriteElementString("NSLOffice", (def.OfficeCode == "CA" ? "KH" : def.OfficeCode));
                        if (def.PortCode.Trim() != String.Empty)
                        {
                            w.WriteElementString("PortOfLoad", def.PortCode);
                        }

                        w.WriteStartElement("RequiredDocs");
                        docs = def.RequiredDocs.Split(delimiter.ToCharArray());
                        foreach (string s in docs)
                        {
                            w.WriteElementString("DocType", s);
                        }
                        w.WriteEndElement(); //RequiredDocs

                        w.WriteStartElement("Quota");
                        if (def.QuotaCats != String.Empty)
                        {
                            cats = def.QuotaCats.Split(delimiter.ToCharArray());
                            foreach (string s in cats)
                            {
                                w.WriteElementString("Category", s);
                            }
                        }
                        w.WriteEndElement(); //Quota
                        w.WriteElementString("OriginCountry", def.OriginCountry);
                        w.WriteEndElement(); //OriginDetails
                        w.WriteEndElement(); //ContractDelivery

                        if (fileNo != "MISSING")
                        {
                            ILSOrderCopyDef ilsOrderCopyDef = ilsUploadWorker.getILSOrderCopyByKey(def.OrderRefId);
                            ilsOrderCopyDef.LastSentLoadingPort = def.PortCode;
                            ilsOrderCopyDef.LastSentOfficeCode = def.OfficeCode;
                            ilsOrderCopyDef.LastSentQuota = def.QuotaCats;
                            ilsOrderCopyDef.LastSentDocType = def.RequiredDocs;
                            ilsOrderCopyDef.LastSentOriginCountry = def.OriginCountry;
                            ilsOrderCopyDef.IsValid = true;

                            ilsUploadWorker.updateILSOrderCopy(ilsOrderCopyDef);
                            shippingWorker.updateActionHistory(new ActionHistoryDef(def.ShipmentId, 0, ActionHistoryType.ORIGIN_MSG, "N/A", GeneralStatus.ACTIVE.Code, 99999));
                        }
                        ilsUploadWorker.updateILSMessageResult(this.createILSMessageResultDef(def.OrderRefId, "ORI"));
                    }
                    this.closeOutputTextWriter(w);
                    ilsUploadWorker.updateILSFileSentLog(outputFileNo, "ORI", DateTime.Now, DateTime.MinValue);
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void cancelOutstandingILSOrders()
        {
            ArrayList list = ilsUploadWorker.getILSCancelledOrderList();

            foreach (ILSCancelledOrderDef def in list)
            {
                this.cancelILSOrder(def);
            }
        }

        private void cancelILSOrder(ILSCancelledOrderDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ILSOrderRefDef ilsOrderRef = ilsUploadWorker.getILSOrderRefByKey(def.OrderRefId);
                if (ilsOrderRef.ShipmentId != 0)
                {
                    ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

                    if (shipmentDef.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
                    {
                        InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(shipmentDef.ShipmentId);
                        def.Remark = "SHIPMENT [ID: " + invoiceDef.ShipmentId.ToString() + "] IS INVOICED";
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.ILS_ORDER_CANCELLATION, "This Shipment Is Already Invoiced", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendInvoicedShipmentCancelledByILSEmail(def.FileNo, ilsOrderRef, invoiceDef.InvoiceNo, def.ImportDate);

                    }
                    else if (shipmentDef.WorkflowStatus.Id == ContractWFS.CANCELLED.Id)
                    {
                        InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(shipmentDef.ShipmentId);
                        def.Remark = "SHIPMENT [ID: " + invoiceDef.ShipmentId.ToString() + "] IS CANCELLED";
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.ILS_ORDER_CANCELLATION, "This Shipment Is Already Cancelled", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendCancelledShipmentCancelledByILSEmail(def.FileNo, ilsOrderRef, def.ImportDate);
                    }
                    else
                    {
                        ArrayList amendmentList = new ArrayList();
                        ArrayList shipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(ilsOrderRef.ShipmentId);

                        foreach (ShipmentDetailDef shipmentDetailDef in shipmentDetailList)
                        {
                            shipmentDetailDef.ShippedQuantity = 0;
                        }
                        OrderWorker.Instance.updateShipmentDetailList(shipmentDetailList, ActionHistoryType.ILS_ORDER_CANCELLATION, amendmentList, 99999);

                        if (shipmentDef.SplitCount > 0)
                        {
                            ArrayList splitShipmentList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(ilsOrderRef.ShipmentId);
                            foreach (SplitShipmentDef splitShipmentDef in splitShipmentList)
                            {
                                ArrayList splitShipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentDetailBySplitShipmentId(splitShipmentDef.SplitShipmentId);
                                foreach (SplitShipmentDetailDef splitShipmentDetailDef in splitShipmentDetailList)
                                {
                                    splitShipmentDetailDef.ShippedQuantity = 0;
                                }
                                OrderWorker.Instance.updateSplitShipmentDetailList(shipmentDef.ShipmentId, splitShipmentDetailList, ActionHistoryType.PACKING_LIST_UPLOAD, amendmentList, 99999);
                            }
                        }
                        ShippingWorker.Instance.updateShipmentSummaryTotal(ilsOrderRef.ShipmentId, 99999);
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.ILS_ORDER_CANCELLATION, "N/A", GeneralStatus.ACTIVE.Code, 99999));
                    }
                    def.Status = GeneralStatus.ACTIVE.Code;
                    ilsUploadWorker.updateILSCancelledOrder(def);
                }
                else
                {
                    NoticeHelper.sendOrderRefWithUnassignedShipmentIdEmail(def.FileNo, ActionHistoryType.ILS_ORDER_CANCELLATION.Description, ilsOrderRef.OrderRef, def.ImportDate);

                }
                ilsUploadWorker.removeILSInvoiceDetails(def.OrderRefId);
                ilsUploadWorker.removeILSInvoice(def.OrderRefId);
                ilsUploadWorker.removeILSPackingListCartonDetails(def.OrderRefId);
                ilsUploadWorker.removeILSPackingListDetails(def.OrderRefId);
                ilsUploadWorker.removeILSPackingList(def.OrderRefId);

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void uploadOutstandingPackingLists()
        {
            ArrayList list = ilsUploadWorker.getOutstandingILSPackingList();

            foreach (ILSPackingListDef def in list)
            {
                this.uploadPackingList(def);
            }
        }

        private void uploadPackingList(ILSPackingListDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ILSOrderRefDef ilsOrderRef = ilsUploadWorker.getILSOrderRefByKey(def.OrderRefId);
                if (ilsOrderRef.ShipmentId != 0)
                {
                    ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);
                    InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(ilsOrderRef.ShipmentId);

                    System.Diagnostics.Debug.WriteLine("ILSOrderRef ShipmentId : " + ilsOrderRef.ShipmentId.ToString());
                        
                    if (shipmentDef.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
                    {
                        def.IsUploaded = true;
                        ilsUploadWorker.updateILSPackingList(def, -1);
                        //if (!CustomerDestinationDef.isUTurnOrder(shipmentDef.CustomerDestinationId) && invoiceDef.IsSelfBilledOrder)
                        if (shipmentDef.TermOfPurchase.TermOfPurchaseId != TermOfPurchaseRef.Id.FOB_UT.GetHashCode() && invoiceDef.IsSelfBilledOrder)
                        {
                            ilsOrderRef.IsReset = true;
                            ilsUploadWorker.updateILSOrderRef(ilsOrderRef);
                        }
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.PACKING_LIST_UPLOAD, "This Shipment Is Already Invoiced", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendPackingListUploadAgainstInvoicedShipmentEmail(def.FileNo, ilsOrderRef, invoiceDef.InvoiceNo, def.ImportDate);
                    }
                    else if (shipmentDef.WorkflowStatus.Id == ContractWFS.CANCELLED.Id)
                    {
                        def.IsUploaded = true;
                        ilsUploadWorker.updateILSPackingList(def, -1);
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.PACKING_LIST_UPLOAD, "This Shipment Is Already Cancelled", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendPackingListUploadAgainstCancelledShipmentEmail(def.FileNo, ilsOrderRef, def.ImportDate);
                    }
                    else if (!invoiceDef.IsILSQtyUploadAllowed)
                    {
                        def.IsUploaded = true;
                        ilsUploadWorker.updateILSPackingList(def, -1);
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.PACKING_LIST_UPLOAD, "ILS Upload Is Turned Off", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendPackingListUploadTurnedOffEmail(def.FileNo, ilsOrderRef, def.ImportDate);
                    }
                    else
                    {
                        invoiceDef.ILSActualAtWarehouseDate = def.HandoverDate;

                        ArrayList amendmentList = new ArrayList();
                        ArrayList packingDetailList = ilsUploadWorker.getILSPackingListDetail(def.OrderRefId, true);
                        ArrayList shipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(ilsOrderRef.ShipmentId);

                        foreach (ShipmentDetailDef shipmentDetailDef in shipmentDetailList)
                        {
                            shipmentDetailDef.ShippedQuantity = 0;
                            foreach (ILSPackingListDetailDef packingListDetailDef in packingDetailList)
                            {
                                if (packingListDetailDef.OptionNo == shipmentDetailDef.SizeOption.SizeOptionNo)
                                {
                                    shipmentDetailDef.ShippedQuantity = packingListDetailDef.Qty;
                                }
                            }
                        }
                        OrderWorker.Instance.updateShipmentDetailList(shipmentDetailList, ActionHistoryType.PACKING_LIST_UPLOAD, amendmentList, 99999);
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.PACKING_LIST_UPLOAD, "N/A", GeneralStatus.ACTIVE.Code, 99999));

                        if (shipmentDef.SplitCount > 0)
                        {
                            ArrayList splitShipmentList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(ilsOrderRef.ShipmentId);
                            foreach (SplitShipmentDef splitShipmentDef in splitShipmentList)
                            {
                                ArrayList splitShipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentDetailBySplitShipmentId(splitShipmentDef.SplitShipmentId);
                                if (splitShipmentDef.IsILSQtyUploadAllowed)
                                {
                                    foreach (SplitShipmentDetailDef splitShipmentDetailDef in splitShipmentDetailList)
                                    {
                                        splitShipmentDetailDef.ShippedQuantity = 0;
                                        foreach (ILSPackingListDetailDef packingListDetailDef in packingDetailList)
                                        {
                                            if (packingListDetailDef.OptionNo == splitShipmentDetailDef.SizeOption.SizeOptionNo)
                                            {
                                                splitShipmentDetailDef.ShippedQuantity = packingListDetailDef.Qty;
                                            }
                                        }
                                    }
                                }
                                OrderWorker.Instance.updateSplitShipmentDetailList(shipmentDef.ShipmentId, splitShipmentDetailList, ActionHistoryType.PACKING_LIST_UPLOAD, amendmentList, 99999);
                                shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, splitShipmentDef.SplitShipmentId, ActionHistoryType.PACKING_LIST_UPLOAD, "N/A", GeneralStatus.ACTIVE.Code, 99999));
                            }
                        }
                        ShippingWorker.Instance.updateShipmentSummaryTotal(ilsOrderRef.ShipmentId, 99999);
                        ShippingWorker.Instance.updateInvoice(invoiceDef, ActionHistoryType.PACKING_LIST_UPLOAD, null, 99999);
                        def.IsUploaded = true;
                        ilsUploadWorker.updateILSPackingList(def, -1);
                    }
                }
                else
                {
                    NoticeHelper.sendOrderRefWithUnassignedShipmentIdEmail(def.FileNo, ActionHistoryType.PACKING_LIST_UPLOAD.Description, ilsOrderRef.OrderRef, def.ImportDate);
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void uploadOutstandingInvoices()
        {
            ArrayList list = ilsUploadWorker.getOutstandingILSInvoice();

            foreach (ILSInvoiceDef def in list)
            {
                this.uploadInvoice(def);
            }
        }

        private void resetPackingLists()
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);

            ShipmentDef shipmentDef = null;
            InvoiceDef invoiceDef = null;
            ILSPackingListDef ilsPackingListDef = null;
            ILSInvoiceDef ilsInvoiceDef = null;

            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList list = ilsUploadWorker.getILSOrderRefResetList();

                foreach (ILSOrderRefDef def in list)
                {
                    invoiceDef = shippingWorker.getInvoiceByKey(def.ShipmentId);
                    ilsPackingListDef = ilsUploadWorker.getILSPackingListByKey(def.OrderRefId);
                    ilsInvoiceDef = ilsUploadWorker.getILSInvoiceByKey(def.OrderRefId);

                    //if (def.ShipmentId == 944310 || def.ShipmentId == 944920)
                    if (!AccountWorker.Instance.isSunInterfaceLogExists(SunInterfaceTypeRef.Id.Purchase.GetHashCode(), def.ShipmentId, 0)
                        && AccountWorker.Instance.getCutOffStatus(def.ShipmentId).CutOffStatusId != CutOffStatus.ACTUAL.GetHashCode()
                        && invoiceDef.APDate == DateTime.MinValue
                        && invoiceDef.ARDate == DateTime.MinValue)
                    {

                        shippingWorker.resetShippedQty(def.ShipmentId);

                        shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(def.ShipmentId);
                        shipmentDef.EditLock = false;
                        if (shipmentDef.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
                            shipmentDef.WorkflowStatus = ContractWFS.PO_PRINTED;
                        OrderWorker.Instance.updateShipmentList(ConvertUtility.createArrayList(shipmentDef));

                        if (ilsPackingListDef != null)
                        {
                            ilsPackingListDef.IsUploaded = false;
                            ilsUploadWorker.updateILSPackingList(ilsPackingListDef, shipmentDef.DeliveryNo);
                        }

                        if (ilsInvoiceDef != null)
                        {
                            ilsInvoiceDef.Status = ILSInvoiceUploadStatus.PENDING.Id;
                            ilsUploadWorker.updateILSInvoice(ilsInvoiceDef);
                        }
                        invoiceDef.InvoicePrefix = String.Empty;
                        invoiceDef.InvoiceSeqNo = 0;
                        invoiceDef.InvoiceYear = 0;
                        invoiceDef.InvoiceDate = DateTime.MinValue;
                        invoiceDef.InvoiceUploadDate = DateTime.MinValue;
                        invoiceDef.InvoiceUploadUser = null;
                        invoiceDef.NSLCommissionAmt = 0;
                        invoiceDef.InvoiceBuyExchangeRate = 0;
                        invoiceDef.InvoiceSellExchangeRate = 0;
                        invoiceDef.ILSActualAtWarehouseDate = DateTime.MinValue;
                        shippingWorker.updateInvoice(invoiceDef, ActionHistoryType.PACKING_LIST_RESET, null, 99999);

                        //shippingWorker.cancelInvoice(invoiceDef, ActionHistoryType.PACKING_LIST_RESET, 99999);
                        shippingWorker.updateActionHistory(new ActionHistoryDef(shipmentDef.ShipmentId, 0, ActionHistoryType.PACKING_LIST_RESET, "N/A", GeneralStatus.ACTIVE.Code, 99999));
                    }
                    def.IsReset = false;
                    ilsUploadWorker.updateILSOrderRef(def);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void resetILSInvoiceUploadStatus()
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ArrayList list = ilsUploadWorker.getILSInvoiceResetList();

                foreach (ILSInvoiceDef def in list)
                {
                    def.Status = ILSInvoiceUploadStatus.PENDING.Id;
                    ilsUploadWorker.updateILSInvoice(def);
                }

                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private void uploadInvoice(ILSInvoiceDef def)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                if (def.IsCancelled)
                    return;

                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                ILSOrderRefDef ilsOrderRef = ilsUploadWorker.getILSOrderRefByKey(def.OrderRefId);
                InvoiceDef existingInvoice = ShippingWorker.Instance.getInvoiceByInvoiceNo(
                                                                ShippingWorker.getInvoicePrefix(def.InvoiceNo),
                                                                ShippingWorker.getInvoiceSeq(def.InvoiceNo),
                                                                ShippingWorker.getInvoiceYear(def.InvoiceNo));
                if (ilsOrderRef.ShipmentId != 0)
                {
                    ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);
                    InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(ilsOrderRef.ShipmentId);

                    if (shipmentDef.WorkflowStatus.Id == ContractWFS.INVOICED.Id)
                    {
                        def.Status = ILSInvoiceUploadStatus.ALREADY_INVOICED.Id;
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Invoice upload failure: This Shipment Is Already Invoiced", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendInvoiceUploadAgainstInvoicedShipmentEmail(def.FileNo, ilsOrderRef, def.InvoiceNo, DateTime.Now);
                    }
                    else if (shipmentDef.WorkflowStatus.Id == ContractWFS.CANCELLED.Id)
                    {
                        def.Status = ILSInvoiceUploadStatus.CANCELLED.Id;
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Invoice upload failure: This Shipment Is Already Cancelled", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendInvoiceUploadAgainstCancelledShipmentEmail(def.FileNo, ilsOrderRef, DateTime.Now);
                    }
                    else if (!invoiceDef.IsSelfBilledOrder)
                    {
                        def.Status = ILSInvoiceUploadStatus.NOT_SELFBILLED_ORDER.Id;
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Invoice upload failure: Not a self-billed order", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendInvoiceUploadNotSelfBilledEmail(def.FileNo, ilsOrderRef, DateTime.Now);
                    }

                    else if (invoiceDef.ILSActualAtWarehouseDate == DateTime.MinValue)
                    {
                        def.Status = ILSInvoiceUploadStatus.MISSING_PACKING_LIST_DATA.Id;
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Invoice upload failure: Missing Packing List Data", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendInvoiceUploadMissingPackingListEmail(def.FileNo, ilsOrderRef, DateTime.Now);
                    }
                    else if (def.InvoiceDate < DateTime.Today.AddDays(-60))
                    {
                        def.Status = ILSInvoiceUploadStatus.INVOICE_DATE_TOLERANCE_ISSUE.Id;
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Invoice upload failure: Invoice Date Not Within (60 DAYS) Tolerence ", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendInvoiceUploadInvoiceDateToleranceEmail(def.FileNo, ilsOrderRef, def.InvoiceDate, DateTime.Now);
                    }
                    else if (def.Currency != (CurrencyId.getCommonName(shipmentDef.SellCurrency.CurrencyId)))
                    {
                        def.Status = ILSInvoiceUploadStatus.CURRENCY_MISMATCH.Id;
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Invoice upload failure: Currency Mismatch", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendInvoiceUploadCurrencyMismatchEmail(def.FileNo, ilsOrderRef, DateTime.Now);
                    }
                    else if (shipmentDef.WorkflowStatus != ContractWFS.APPROVED
                             && shipmentDef.WorkflowStatus != ContractWFS.PO_PRINTED)
                    {
                        def.Status = 1500 + shipmentDef.WorkflowStatus.Id;
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Invoice upload failure: Not Yet Approved By Merchandiser(s)", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendInvoiceUploadAgainstNotYetApprovedShipmentEmail(def.FileNo, ilsOrderRef, shipmentDef.WorkflowStatus, DateTime.Now);
                    }
                    else if (shipmentDef.Vendor.VendorId == 3154 && shipmentDef.FactoryId == -99999)
                    {
                        def.Status = ILSInvoiceUploadStatus.MISSING_FACTORYID_FOR_NML.Id;
                        shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Invoice upload failure: Missing Factory Id For NML", GeneralStatus.INACTIVE.Code, 99999));
                        NoticeHelper.sendInvoiceUploadMissingFactoryIdForNMLEmail(def.FileNo, ilsOrderRef, DateTime.Now);
                    }
                    else if (existingInvoice != null)
                    {
                        if (existingInvoice.ShipmentId != ilsOrderRef.ShipmentId)
                        {
                            def.Status = ILSInvoiceUploadStatus.INVOICE_NO_BEING_USED.Id;
                            shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Invoice upload failure: Invoice No. Has Been Used By Another Shipment", GeneralStatus.INACTIVE.Code, 99999));
                            NoticeHelper.sendInvoiceUploadInvoiceNoUsedByAnotherShipmentEmail(def.FileNo, ilsOrderRef, existingInvoice, DateTime.Now, def.InvoiceNo);
                        }
                    }
                    else
                    {
                        ILSPackingListDef packingListDef = ilsUploadWorker.getILSPackingListByKey(def.OrderRefId);
                        ArrayList packinglistDetailList = ilsUploadWorker.getILSPackingListDetail(def.OrderRefId, true);
                        ArrayList shipmentDetailList = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(ilsOrderRef.ShipmentId);

                        int totalQty = 0;
                        foreach (ShipmentDetailDef shipmentDetailDef in shipmentDetailList)
                        {
                            if (shipmentDetailDef.ShippedQuantity != 0)
                            {
                                bool isOptionFound = false;
                                foreach (ILSPackingListDetailDef packingListDetailDef in packinglistDetailList)
                                {
                                    if (packingListDetailDef.OptionNo == shipmentDetailDef.SizeOption.SizeOptionNo)
                                    {
                                        totalQty += shipmentDetailDef.ShippedQuantity;
                                        isOptionFound = true;
                                    }
                                }
                                if (!isOptionFound && def.Status == -1)
                                    def.Status = ILSInvoiceUploadStatus.OPTION_MISMATCH.Id;
                            }
                        }

                        foreach (ILSPackingListDetailDef packingListDetailDef in packinglistDetailList)
                        {
                            if (packingListDetailDef.Qty != 0)
                            {
                                bool isOptionFound = false;
                                foreach (ShipmentDetailDef shipmentDetailDef in shipmentDetailList)
                                {
                                    if (packingListDetailDef.OptionNo == shipmentDetailDef.SizeOption.SizeOptionNo)
                                        isOptionFound = true;
                                }
                                if (!isOptionFound && def.Status == -1)
                                    def.Status = ILSInvoiceUploadStatus.OPTION_MISMATCH.Id;
                            }
                        }

                        if (def.Status == ILSInvoiceUploadStatus.OPTION_MISMATCH.Id)
                        {
                            shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Size Option(s) Discrepancy Encountered", GeneralStatus.INACTIVE.Code, 99999));
                            NoticeHelper.sendInvoiceUploadOptionMismatchEmail(def.FileNo, ilsOrderRef, DateTime.Now);
                        }
                        
                        else if (packingListDef.TotalPieces != totalQty)
                        {
                            def.Status = ILSInvoiceUploadStatus.QUANTITY_MISMATCH.Id;
                            shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, "Total Quantity Mismatch", GeneralStatus.INACTIVE.Code, 99999));
                            NoticeHelper.sendInvoiceUploadQuantityMismatchEmail(def.FileNo, ilsOrderRef, DateTime.Now);
                        }
                        /*
                        if (def.OrderRefId == 604966 || def.OrderRefId == 612704)
                            def.Status = -1;
                        */
                        if (def.Status == -1)
                        {
                            ArrayList amendmentList = new ArrayList();

                            if (invoiceDef.ActualAtWarehouseDate != DateTime.MinValue)
                                invoiceDef.InvoiceDate = invoiceDef.ActualAtWarehouseDate;
                            else
                                invoiceDef.InvoiceDate = def.InvoiceDate;

                            shipmentDef.USExchangeRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, invoiceDef.InvoiceDate);
                            shipmentDef.EditLock = true;
                            shipmentDef.PaymentLock = true;
                            shipmentDef.WorkflowStatus = ContractWFS.INVOICED;
                            OrderWorker.Instance.updateShipmentList(ConvertUtility.createArrayList(shipmentDef), 99999);

                            /*
                            AccountFinancialCalenderDef invoiceCalDef = GeneralWorker.Instance.getAccountPeriodByDate(AppId.NSS.Code, invoiceDef.InvoiceDate);
                            AccountFinancialCalenderDef cutOffCalDef = AccountWorker.Instance.getLatestCutOffPeriod();

                            if (AccountWorker.Instance.getCutOffStatus(invoiceDef.ShipmentId).CutOffStatusId == CutOffStatus.UNDEFINED.GetHashCode()
                                && (cutOffCalDef.ToString() == invoiceCalDef.ToString()))
                            {
                                AccountFinancialCalenderDef calDef = null;
                                if (cutOffCalDef.Period == 12)
                                    calDef = GeneralWorker.Instance.getAccountPeriodByYearPeriod(AppId.NSS.Code, cutOffCalDef.BudgetYear + 1, 1);
                                else
                                    calDef = GeneralWorker.Instance.getAccountPeriodByYearPeriod(AppId.NSS.Code, cutOffCalDef.BudgetYear, cutOffCalDef.Period + 1);
                                shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.LATE_SELF_BILLING, "Invoice date (" + DateTimeUtility.getDateString(invoiceDef.InvoiceDate) + ") was auto-amended to 1st date of next fiscal period (" + calDef.ToString() + ")", GeneralStatus.ACTIVE.Code));
                                invoiceDef.InvoiceDate = calDef.StartDate;
                            }
                            */
                            invoiceDef.InvoiceUploadDate = DateTime.Now;
                            invoiceDef.InvoiceUploadUser = GeneralWorker.Instance.getUserByKey(99999);
                            invoiceDef.IsSelfBilledOrder = true;
                            invoiceDef.InvoicePrefix = ShippingWorker.getInvoicePrefix(def.InvoiceNo);
                            invoiceDef.InvoiceSeqNo = ShippingWorker.getInvoiceSeq(def.InvoiceNo);
                            invoiceDef.InvoiceYear = ShippingWorker.getInvoiceYear(def.InvoiceNo);
                            invoiceDef.NSLCommissionAmt = Math.Round(shipmentDef.NSLCommissionPercentage / 100 * shipmentDef.TotalShippedAmount, 2, MidpointRounding.AwayFromZero);
                            invoiceDef.InvoiceBuyExchangeRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, shipmentDef.BuyCurrency.CurrencyId, invoiceDef.InvoiceDate);
                            invoiceDef.InvoiceSellExchangeRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, shipmentDef.SellCurrency.CurrencyId, invoiceDef.InvoiceDate);
                            ShippingWorker.Instance.updateInvoice(invoiceDef, ActionHistoryType.INVOICE_UPLOAD, amendmentList, 99999);
                            shippingWorker.updateActionHistory(new ActionHistoryDef(ilsOrderRef.ShipmentId, 0, ActionHistoryType.INVOICE_UPLOAD, def.InvoiceNo, GeneralStatus.ACTIVE.Code, 99999));

                            ArrayList splitShipments = (ArrayList)OrderSelectWorker.Instance.getSplitShipmentByShipmentId(ilsOrderRef.ShipmentId);
                            foreach (SplitShipmentDef splitShipmentDef in splitShipments)
                            {
                                splitShipmentDef.InvoiceBuyExchangeRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, splitShipmentDef.BuyCurrency.CurrencyId, def.InvoiceDate);
                                OrderWorker.Instance.updateSplitShipmentList(ConvertUtility.createArrayList(splitShipmentDef), 99999);
                            }

                            def.Status = GeneralStatus.ACTIVE.Code;
                        }
                    }
                    if (def.ProcessedDate == DateTime.MinValue)
                        def.ProcessedDate = DateTime.Now;
                }
                else
                {
                    def.Status = ILSInvoiceUploadStatus.UNASSIGNED_SHIPMENT.Id;
                    NoticeHelper.sendOrderRefWithUnassignedShipmentIdEmail(def.FileNo, ActionHistoryType.INVOICE_UPLOAD.Description, ilsOrderRef.OrderRef, def.ImportDate);
                }
                ilsUploadWorker.updateILSInvoice(def);
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private ILSMessageResultDef createILSMessageResultDef(string status, int errorNo)
        {
            ILSMessageResultDef def = new ILSMessageResultDef();
            def.FileNo = fileNo;
            def.OrderRefId = -1;
            def.ContractNo = contractNo;
            def.DeliveryNo = deliveryNo;
            def.Status = status;
            def.ProcessedDate = DateTime.Now;
            def.ErrorNo = errorNo;
            return def;
        }

        private ILSMessageResultDef createILSMessageResultDef(int orderRefId, string type)
        {
            ILSMessageResultDef def = new ILSMessageResultDef();
            def.FileNo = outputFileNo;
            def.OrderRefId = orderRefId;
            def.Type = type;
            def.SentDate = DateTime.Now;
            return def;
        }

        public ILSManifestDef getManifestByKey(string containerNo)
        {
            return ilsUploadWorker.getILSManifestByKey(containerNo);
        }

        public ICollection getManifestDetailListByKey(string containerNo)
        {
            return ilsUploadWorker.getILSManifestDetailList(containerNo);
        }

        public ArrayList getManifestList(string voyageNo, DateTime departDate, string contractNo, string departPort)
        {
            return (ArrayList)getManifestList(voyageNo, departDate, contractNo, departPort, "");
        }

        public ArrayList getManifestList(string voyageNo, DateTime departDate, string contractNo, string departPort, string vesselName)
        {
            return (ArrayList)ilsUploadWorker.getILSManifestList(voyageNo, departDate, contractNo, departPort, vesselName);
        }


        private XmlTextWriter getOutputTextWriter()
        {
            outputFileNo = ilsUploadWorker.getOutputFileNo();
            string outputMsgFolder = WebConfig.getValue("appSettings", "ILS-OUTPUT-MSG-FOLDER");
            string filePath = outputMsgFolder + "\\" + "I.NSL000." + outputFileNo + ".CTU." + DateTime.Now.ToString("yyyyMMdd.hhmmss") + ".xml";

            XmlTextWriter w = new XmlTextWriter(filePath, System.Text.Encoding.UTF8);
            w.WriteStartDocument();
            w.WriteComment("FROM UK FILE " + fileNo);
            w.WriteStartElement("NEXT");
            w.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            w.WriteAttributeString("xsi:noNamespaceSchemaLocation", "ContractStatusUpdate_V1_1_7.xsd");

            w.WriteStartElement("Logistics");
            w.WriteStartElement("ILS");
            w.WriteStartElement("ContractStatusUpdate");

            w.WriteStartElement("Header");
            w.WriteElementString("Direction", "I");
            w.WriteElementString("PartnerCode", "NSL000");
            w.WriteElementString("FileNbr", outputFileNo);
            w.WriteElementString("DateTime", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.0000000"));
            w.WriteEndElement(); //Header
            return w;
        }

        private void closeOutputTextWriter(XmlTextWriter w)
        {
            w.WriteEndElement(); //ContractStatusUpdate
            w.WriteEndElement(); //ILS
            w.WriteEndElement(); //Logistics
            w.WriteEndElement(); //NEXT
            w.WriteEndDocument();
            w.Flush();
            w.Close();
        }

        private void assignOrderRefNo(string s)
        {
            s = s.Trim();
            int l;
            l = s.Length;
            contractNo = s.Substring(0, s.IndexOf("-"));
            deliveryNo = s.Substring(s.IndexOf("-") + 1, 2);
        }

        public void uploadILSPackingListTemplate(string filePath)
        {
            filePath = @"d:\downloads\1.xlsx";
            DataUploadWorker dataUploadWorker = DataUploadWorker.Instance;
            List<List<string>> ilsData = dataUploadWorker.getWorkSheet(filePath, "Sheet1");
            foreach (List<string> col in ilsData)
            {
                foreach (string s in col)
                {
                    System.Diagnostics.Debug.Print(s);
                }
            }
        }

    }
}
