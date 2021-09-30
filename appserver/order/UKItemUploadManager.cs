using System;
using System.Xml;
using System.IO;
using System.Web;
using System.Collections;
using com.next.infra.persistency.transactions;
using com.next.infra.util;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.types;
using com.next.isam.domain.common;
using com.next.common.datafactory.worker;
using com.next.common.domain.types;
using com.next.common.domain;

namespace com.next.isam.appserver.order
{
    public class UKItemUploadManager
    {
        private static UKItemUploadManager _instance;
        private UKItemWorker ukItemWorker;
        private DomainUKItemDef domainUKItemDef = null;
        private DateTime importDate = DateTime.MinValue;
        private int partNo;
        private string itemNo = String.Empty;

        public UKItemUploadManager()
        {
            ukItemWorker = UKItemWorker.Instance;
        }

        public static UKItemUploadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UKItemUploadManager();
                }
                return _instance;
            }
        }

        public void processFiles()
        {
            XmlTextReader reader = null;
            try
            {
                string xmlFolder = WebConfig.getValue("appSettings", "UKITEM-UPLOAD_XML_Folder");
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
                                        if (reader.Name == "Item") processItem(reader.ReadOuterXml());
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
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Duplicate File");
                            MailHelper.sendGeneralMessage("Duplicate UK Item File# [" + xmlFiles[i] + "]", String.Empty);
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


        private void processItem(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string itemDesc = String.Empty;
            string subGroup = String.Empty;
            string supplierCode = String.Empty;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "ItemNo")
                        {
                            itemNo = reader.ReadInnerXml();
                            ukItemWorker.removeUKItemPartDetails(itemNo);
                        }
                        else if (reader.Name == "Description") itemDesc = reader.ReadInnerXml();
                        else if (reader.Name == "SubGroup") subGroup = reader.ReadInnerXml();
                        else if (reader.Name == "SupplierCode") supplierCode = reader.ReadInnerXml();
                        else if (reader.Name == "Parts") processParts(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            DomainUKItemDef domainUKItemDef = new DomainUKItemDef();

            domainUKItemDef.UKItem.ItemNo = itemNo.Trim();
            domainUKItemDef.UKItem.Description = itemDesc.Trim();
            domainUKItemDef.UKItem.SubGroup = subGroup.Trim();
            domainUKItemDef.UKItem.SupplierCode = supplierCode.Trim();

            ukItemWorker.updateUKItem(domainUKItemDef.UKItem);
        }

        private void processParts(string xml)
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
                            if (reader.Name == "Part") processPart(reader.ReadOuterXml());
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

        private void processPart(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string reason = String.Empty;
            string invNo = String.Empty;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:

                        if (reader.Name == "PartNumber") partNo = int.Parse(reader.ReadInnerXml().Trim());
                        else if (reader.Name == "Current") processCurrentItem(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
        }

        private void processCurrentItem(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));

            string nameCode = String.Empty;
            string partDesc = String.Empty;
            string supplierCode = String.Empty;
            int qty = 0;
            string comment = String.Empty;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "PartNameCode") nameCode = reader.ReadInnerXml();
                        else if (reader.Name == "PartDescription") partDesc = reader.ReadInnerXml();
                        else if (reader.Name == "SupplierCode") supplierCode = reader.ReadInnerXml();
                        else if (reader.Name == "PartQuantity") qty = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "PartComments") comment = reader.ReadInnerXml();
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            UKItemPartRef def = new UKItemPartRef();
            def.ItemNo = itemNo;
            def.PartNo = partNo;
            def.NameCode = nameCode.Trim();
            def.Qty = qty;
            def.Comment = comment.Trim();
            def.SupplierCode = supplierCode.Trim();
            def.Description = partDesc.Trim();

            ukItemWorker.updateUKItemPart(def);
        }

    }
}
