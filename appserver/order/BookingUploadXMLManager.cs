using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.IO;
using com.next.infra.util;
using com.next.isam.dataserver.worker;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.infra.persistency.transactions;
using com.next.common.web.commander;
using System.Web;

namespace com.next.isam.appserver.order
{
    public class BookingUploadXMLManager
    {
        private static BookingUploadXMLManager _instance;
        private BookingUploadXMLWorker bookingUploadXMLWorker;

        private string batchNo = "";
        private string txNo = "";
        private int contractId = 0;
        private bool isStage4 = false;
        private string deliveryNo = "";
        private int deliverySeq = 0;
        private int ttlOption = 0;
        private int ttlQty = 0;

        public BookingUploadXMLManager()
        {
            bookingUploadXMLWorker = BookingUploadXMLWorker.Instance;
        }

        public static BookingUploadXMLManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BookingUploadXMLManager();
                }
                return _instance;
            }
        }

        public void readXMLFile()
        {
            XmlTextReader reader = null;
            double nextStartTime = DateTime.Now.ToUniversalTime().AddHours(8).AddHours(1).ToOADate();
            try
            {
                //string result = (new System.Net.WebClient()).DownloadString("http://c023/TMS/api/dmx/start/15");

                string xmlFolder = WebConfig.getValue("appSettings", "E-BOOKING-UPLOAD_XML_Folder");
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
                            s.Replace("&amp;amp;", "&amp;");
                            w = System.IO.File.CreateText(fi.DirectoryName + "\\" + fi.Name.Replace(".xml", "") + ".dat");
                            w.Write(s.ToString());
                            w.Flush();
                            w.Close();

                            reader = new XmlTextReader(fi.DirectoryName + "\\" + fi.Name.Replace(".xml", "") + ".dat");

                            reader.MoveToContent();
                            reader.Read();
                            while (!reader.EOF)
                            {
                                switch (reader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if (reader.Name == "record-type-000") processRecord000(reader.ReadOuterXml());
                                        else if (reader.Name == "record-type-001") processRecord001(reader.ReadOuterXml());
                                        else if (reader.Name == "record-type-200") processRecord200(reader.ReadOuterXml());
                                        else if (reader.Name == "record-type-203")
                                        {
                                            if (isStage4) processRecord203(reader.ReadOuterXml());
                                            else reader.Read();
                                        }
                                        else if (reader.Name == "record-type-204") processRecord204(reader.ReadOuterXml());
                                        else if (reader.Name == "record-type-998")
                                        {
                                            if (isStage4) processRecord998(reader.ReadOuterXml());
                                            else reader.Read();
                                        }
                                        else reader.Read();
                                        break;
                                    default:
                                        reader.Read();
                                        break;
                                }
                            }
                            reader.Close();
                            finalizeImport(batchNo);
                            System.Diagnostics.Debug.WriteLine(xmlFiles[i]);
                            System.IO.File.Delete(xmlFiles[i]);
                            System.IO.File.Delete(xmlFiles[i].Replace(".xml", ".dat"));
                            System.Diagnostics.Debug.WriteLine("Completed");
                            MailHelper.sendGeneralMessage("E-Booking Batch Completed Successfully (" + batchNo + ")", "");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Duplicate File");
                            MailHelper.sendGeneralMessage("Duplicate E-Booking Batch (" + batchNo + ")", "");
                        }
                    }
                }

                //result = (new System.Net.WebClient()).DownloadString("http://c023/TMS/api/dmx/end/15/Success/" + nextStartTime);

                //bookingUploadXMLWorker.insertEBookingBatch("exec nsp_DataClone_forDashboard");

            }
            catch (Exception e)
            {
                string result = (new System.Net.WebClient()).DownloadString("http://c023/TMS/api/dmx/end/15/Fail/" + nextStartTime);
                MailHelper.sendErrorAlert(" - E-booking Upload", e, "NSS Admin");
                if (reader != null)
                    reader.Close();
            }
        }

        private void finalizeImport(string batchNo)
        {

            System.Diagnostics.Debug.WriteLine("Finalizing......");
            try
            {
                bookingUploadXMLWorker.insertEBookingBatch("EXEC sp_import_booking '" + batchNo + "'");
                //MailHelper.sendGeneralMessage("Finished Calling sp_import_booking", DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("Mail") == -1 && e.Message.IndexOf("MAPI") == -1)
                {
                    throw new Exception(e.Message);
                }

            }
        }


        private void updateEbookingSummary()
        {
            System.Diagnostics.Debug.WriteLine("Updating Summary......");
            try
            {
                bookingUploadXMLWorker.insertEBookingBatch("EXEC nsp_DataClone_forDashboard");
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("Mail") == -1 && e.Message.IndexOf("MAPI") == -1)
                {
                    throw new Exception(e.Message);
                }

            }
        }

        private void processRecord000(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            string fileName = "";
            string extractDate = "";
            string extractTime = "";

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "batch-number") batchNo = reader.ReadInnerXml();
                        else if (reader.Name == "file-name") fileName = reader.ReadInnerXml();
                        else if (reader.Name == "extract-date")
                        {
                            string[] d;
                            extractDate = reader.ReadInnerXml();
                            d = extractDate.Split("/".ToCharArray());
                            extractDate = d[2] + "-" + d[1] + "-" + d[0];
                        }
                        else if (reader.Name == "extract-time") extractTime = reader.ReadInnerXml();
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            reader.Close();
            bookingUploadXMLWorker.deleteEBookingBatch(batchNo);
            sw.WriteLine("INSERT EBookingBatch VALUES ('{0}', '{1}', '{2}', '{3}')", batchNo, fileName, extractDate + " " + extractTime, System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
        }

        private void processRecord001(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            string txCode = "";
            string txDate = "";
            string txTime = "";

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "transaction-number") txNo = reader.ReadInnerXml();
                        else if (reader.Name == "transaction-code") txCode = reader.ReadInnerXml();
                        else if (reader.Name == "date")
                        {
                            string[] d;
                            txDate = reader.ReadInnerXml();
                            d = txDate.Split("/".ToCharArray());
                            txDate = d[2] + "-" + d[1] + "-" + d[0];
                        }
                        else if (reader.Name == "time") txTime = reader.ReadInnerXml();
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            reader.Close();
            sw.WriteLine("INSERT EBookingTransaction VALUES ('{0}', '{1}', '{2}', '{3}', {4})", batchNo, txNo, txCode, txDate + " " + txTime, "null");
            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
        }

        private void processRecord200(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            string officeCode = "";
            string contractNo = "";
            string bookingRefNo = "";
            string bookingDate = "";
            string initialOrderIndicator = "";
            string datePlaced = "";
            string designNo = "";
            string itemNo = "";
            string itemDesc = "";
            int versionNo = 0;
            string season = "";
            int phase = 0;
            string fabricComposition = "";
            string gender = "";
            string colour = "";
            string paymentTerm = "";
            string termOfPurchase = "";
            string purchaseLocation = "";
            string partialShipmentIndicator = "";
            string transhipmentIndicator = "";
            string currency = "";
            string chainName = "";
            string subGroup = "";
            int piecesPerPack = 0;
            string hangBoxIndicator = "";
            int stageId = 4;
            deliverySeq = 1;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "enquiry-stage")
                        {
                            stageId = int.Parse(reader.ReadInnerXml());
                            isStage4 = (stageId == 4) ? true : false;
                        }
                        if (reader.Name == "unique-contract-id")
                        {
                            string s = reader.ReadInnerXml();
                            if (s == String.Empty)
                                contractId = 0;
                            else
                                contractId = int.Parse(s);
                        }
                        else if (reader.Name == "office-code") officeCode = reader.ReadInnerXml();
                        else if (reader.Name == "booking-reference-no") bookingRefNo = reader.ReadInnerXml();
                        else if (reader.Name == "booking-form-generation-date")
                        {
                            string[] d;
                            bookingDate = reader.ReadInnerXml();
                            if (bookingDate == "01/01/0001") bookingDate = string.Empty;
                            if (bookingDate != "")
                            {
                                d = bookingDate.Split("/".ToCharArray());
                                bookingDate = d[2] + "-" + d[1] + "-" + d[0];
                            }
                        }
                        else if (reader.Name == "contract-number")
                        {
                            contractNo = reader.ReadInnerXml().Trim();
                            if (contractNo == "UH8440507") isStage4 = false;
                            System.Diagnostics.Debug.WriteLine(contractNo);
                        }
                        else if (reader.Name == "initial-order-indicator") initialOrderIndicator = reader.ReadInnerXml();
                        else if (reader.Name == "date-placed")
                        {
                            string[] d;
                            datePlaced = reader.ReadInnerXml();
                            if (datePlaced == "01/01/0001") datePlaced = string.Empty;
                            if (datePlaced != "")
                            {
                                d = datePlaced.Split("/".ToCharArray());
                                datePlaced = d[2] + "-" + d[1] + "-" + d[0];
                            }
                        }
                        else if (reader.Name == "design-number") designNo = reader.ReadInnerXml();
                        else if (reader.Name == "item-number") itemNo = reader.ReadInnerXml();
                        else if (reader.Name == "item-description") itemDesc = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "season") season = reader.ReadInnerXml();
                        else if (reader.Name == "version-number")
                        {
                            string tmp = reader.ReadInnerXml();
                            if (tmp.Trim() == "")
                                versionNo = 0;
                            else
                                versionNo = int.Parse(tmp);
                        }
                        else if (reader.Name == "phase")
                        {
                            string tmp = reader.ReadInnerXml();
                            if (tmp.Trim() == "")
                                phase = 0;
                            else
                                phase = int.Parse(tmp);
                        }
                        else if (reader.Name == "fabric-composition") fabricComposition = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "gender") gender = reader.ReadInnerXml();
                        else if (reader.Name == "colour") colour = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "payment-terms") paymentTerm = reader.ReadInnerXml();
                        else if (reader.Name == "purchase-location") purchaseLocation = reader.ReadInnerXml();
                        else if (reader.Name == "terms-of-purchase") termOfPurchase = reader.ReadInnerXml();
                        else if (reader.Name == "partial-shipment-indicator") partialShipmentIndicator = reader.ReadInnerXml();
                        else if (reader.Name == "transhipment-indicator") transhipmentIndicator = reader.ReadInnerXml();
                        else if (reader.Name == "currency") currency = reader.ReadInnerXml();
                        else if (reader.Name == "chain-name") chainName = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "sub-group") subGroup = reader.ReadInnerXml();
                        else if (reader.Name == "units-per-cost-price")
                        {
                            string tmp = reader.ReadInnerXml();
                            if (tmp.Trim() == "0*")
                            {
                                if (itemNo == "695369")
                                    piecesPerPack = 10;
                                else
                                    piecesPerPack = 1;
                            }
                            else
                                piecesPerPack = int.Parse(tmp);
                        }
                        /*
                        else if (reader.Name == "units-per-cost-price") piecesPerPack = int.Parse(reader.ReadInnerXml());
                        */
                        else if (reader.Name == "hang-box-indicator") hangBoxIndicator = reader.ReadInnerXml();
                        else if (reader.Name == "record-type-201")
                        {
                            processRecord201(reader.ReadOuterXml(), deliverySeq);
                            deliverySeq += 1;
                        }
                        else if (reader.Name == "record-type-205")
                        {
                            if (isStage4) processRecord205(reader.ReadOuterXml());
                            else reader.Read();
                        }
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            reader.Close();

            sw.WriteLine("INSERT EBookingContract VALUES ('{0}', '{1}', {2}, {3}, '{4}', '{5}', {6}, '{7}', ", batchNo, txNo, contractId.ToString(), stageId.ToString(), officeCode, bookingRefNo, bookingDate == "" ? "null" : "'" + bookingDate + "'", contractNo);
            sw.WriteLine("{0}, {1}, '{2}', '{3}', '{4}', '{5}', ", initialOrderIndicator == "Y" ? "1" : "0", datePlaced == "" ? "null" : "'" + datePlaced + "'", designNo, itemNo, itemDesc.Replace("'", "''"), colour.Replace("'", "''"));
            sw.WriteLine("{0}, '{1}', {2}, '{3}', '{4}', '{5}', ", versionNo.ToString(), season, phase.ToString(), fabricComposition.Replace("'", "''"), gender, paymentTerm);
            sw.WriteLine("'{0}', '{1}', {2}, {3}, '{4}', '{5}', ", termOfPurchase, purchaseLocation, transhipmentIndicator == "Y" ? "1" : "0", partialShipmentIndicator == "Y" ? "1" : "0", currency, chainName);
            sw.WriteLine("'{0}', {1}, {2})", subGroup, piecesPerPack.ToString(), hangBoxIndicator == "H" ? "1" : "0");

            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());

            /*
			if (isStage4)
			{
				sw.WriteLine("INSERT EBookingContract VALUES ('{0}', '{1}', {2}, '{3}', {4}, '{5}', {6}, '{7}', ", batchNo, txNo, contractId.ToString(), stageId.ToString(), officeCode, bookingRefNo, bookingDate == "" ? "null" : "'" + bookingDate + "'", contractNo);
				sw.WriteLine("{0}, {1}, '{2}', '{3}', '{4}', '{5}', ", initialOrderIndicator == "Y" ? "1" : "0", datePlaced == "" ? "null" : "'" + datePlaced + "'", designNo, itemNo, itemDesc.Replace("'","''"), colour);
				sw.WriteLine("{0}, '{1}', {2}, '{3}', '{4}', '{5}', ", versionNo.ToString(), season, phase.ToString(), fabricComposition.Replace("'","''"), gender, paymentTerm);
				sw.WriteLine("'{0}', '{1}', {2}, {3}, '{4}', '{5}', ", termOfPurchase, purchaseLocation, transhipmentIndicator == "Y" ? "1" : "0", partialShipmentIndicator == "Y" ? "1" : "0", currency, chainName);
				sw.WriteLine("'{0}', {1}, {2})", subGroup, piecesPerPack.ToString(), hangBoxIndicator == "H" ? "1" : "0");

				System.Diagnostics.Debug.WriteLine(sw.ToString());
				bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
			}
			else
			{
				sw.WriteLine("DELETE EBookingTransaction WHERE BatchNo = '{0}' AND TransactionNo = '{1}'", batchNo, txNo);
				System.Diagnostics.Debug.WriteLine(sw.ToString());
				bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
			}
            */
        }

        private void processRecord201(string xml, int deliverySeq)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            string originalDeliveryDate = String.Empty;
            string revisedDeliveryDate = String.Empty;
            string shipmentDestination = String.Empty;
            string countryOfOrigin = String.Empty;
            string countryOfDespatch = String.Empty;
            string transportMode = String.Empty;
            string airPayment = String.Empty;
            string destinationCode = String.Empty;
            string refurbTypeCode = String.Empty;

            ttlOption = 0;
            ttlQty = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "delivery-number") deliveryNo = reader.ReadInnerXml();
                        else if (reader.Name == "original-delivery-date")
                        {
                            string[] d;
                            originalDeliveryDate = reader.ReadInnerXml();
                            if (originalDeliveryDate != "")
                            {
                                d = originalDeliveryDate.Split("/".ToCharArray());
                                originalDeliveryDate = d[2] + "-" + d[1] + "-" + d[0];
                            }
                        }
                        else if (reader.Name == "delivery-date")
                        {
                            string[] d;
                            revisedDeliveryDate = reader.ReadInnerXml();
                            if (revisedDeliveryDate != "")
                            {
                                d = revisedDeliveryDate.Split("/".ToCharArray());
                                revisedDeliveryDate = d[2] + "-" + d[1] + "-" + d[0];
                            }
                        }
                        else if (reader.Name == "destination") shipmentDestination = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "country-of-origin") countryOfOrigin = reader.ReadInnerXml();
                        else if (reader.Name == "country-of-despatch") countryOfDespatch = reader.ReadInnerXml();
                        else if (reader.Name == "transport-mode") transportMode = reader.ReadInnerXml();
                        else if (reader.Name == "air-payment") airPayment = reader.ReadInnerXml();
                        else if (reader.Name == "os-destination-code") destinationCode = reader.ReadInnerXml();
                        else if (reader.Name == "refurb-type") refurbTypeCode = reader.ReadInnerXml();
                        else if (reader.Name == "record-type-202")
                        {
                            if (isStage4) processRecord202(reader.ReadOuterXml());
                            else reader.Read();
                        }
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            reader.Close();

            if (deliveryNo == String.Empty)
                deliveryNo = "00";

            sw.WriteLine("INSERT EBookingShipment VALUES ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, ", batchNo, txNo, contractId.ToString(), deliverySeq, int.Parse(deliveryNo), (originalDeliveryDate == String.Empty ? "NULL" : "'" + originalDeliveryDate + "'"), (revisedDeliveryDate == String.Empty ? "NULL" : "'" + revisedDeliveryDate + "'"));
            sw.WriteLine("'{0}', '{1}', '{2}', '{3}', '{4}', {5}, {6}, null, '{7}', '{8}')", shipmentDestination, countryOfOrigin, countryOfDespatch, transportMode, airPayment, ttlOption.ToString(), ttlQty.ToString(), destinationCode.Trim(), refurbTypeCode.Trim());
            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
        }

        private void processRecord202(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            string optionNo = String.Empty;
            string sizeDesc = String.Empty;
            decimal costPrice = 0;
            decimal sellingPrice = 0;
            decimal vat = 0;
            decimal quota = 0;
            int quantity = 0;
            int revisedQuantity = 0;
            string tmp = String.Empty;
            //bool isPriceEmpty = false;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "option") optionNo = reader.ReadInnerXml();
                        else if (reader.Name == "size") sizeDesc = reader.ReadInnerXml();
                        else if (reader.Name == "cost-price")
                        {
                            tmp = reader.ReadInnerXml();
                            if (tmp.Trim() == "")
                                //isPriceEmpty = true;
                                costPrice = 0;
                            else
                                costPrice = decimal.Parse(tmp.Substring(0, 11) + "." + tmp.Substring(11));
                        }
                        else if (reader.Name == "selling-price")
                        {
                            decimal d;
                            tmp = reader.ReadInnerXml();
                            if (tmp.Trim() == "" || !decimal.TryParse(tmp, out d))
                            {

                                //isPriceEmpty = true;
                                sellingPrice = 0;
                                NoticeHelper.sendGeneralMessage("*** Ebooking **** Misssing / Invalid Selling Price " + batchNo + "-" + txNo, xml);
                            }
                            else
                                sellingPrice = decimal.Parse(tmp.Substring(0, 11) + "." + tmp.Substring(11));
                        }
                        else if (reader.Name == "vat")
                        {
                            tmp = reader.ReadInnerXml();
                            if (tmp.Trim() == "")
                                vat = 0;
                            else
                                vat = decimal.Parse(tmp.Substring(0, 2) + "." + tmp.Substring(2));
                        }
                        else if (reader.Name == "quota")
                        {
                            tmp = reader.ReadInnerXml();
                            if (tmp.Trim() == "")
                                quota = 0;
                            else
                                quota = decimal.Parse(tmp.Substring(0, 10) + "." + tmp.Substring(10));
                        }
                        else if (reader.Name == "quantity")
                        {
                            quantity = int.Parse(reader.ReadInnerXml());
                        }
                        else if (reader.Name == "revised-quantity")
                        {
                            revisedQuantity = int.Parse(reader.ReadInnerXml());
                            if (revisedQuantity > 0)
                            {
                                ttlOption += 1;
                                ttlQty += revisedQuantity;
                            }
                        }
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            reader.Close();

            //if (deliveryNo.Trim() != String.Empty)
            //if (!isPriceEmpty)
            //{
            sw.WriteLine("INSERT EBookingSizeOption VALUES ('{0}', '{1}', {2}, {3}, '{4}', '{5}', ", batchNo, txNo, contractId.ToString(), deliverySeq, optionNo, sizeDesc);
            sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, null)", costPrice, sellingPrice, vat, quota, quantity, revisedQuantity);

            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
            //}
        }

        private void processRecord203(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            string messageType = "";
            int messageLine = 0;
            string messageText = "";

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "message-type") messageType = reader.ReadInnerXml();
                        else if (reader.Name == "message-line-number") messageLine = int.Parse(reader.ReadInnerXml());
                        else if (reader.Name == "message-text") messageText = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            sw.WriteLine("INSERT EBookingMessage VALUES ('{0}', '{1}', {2}, '{3}', {4}, '{5}')", batchNo, txNo, contractId.ToString(), messageType, messageLine.ToString(), messageText);

            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
        }

        private void processRecord204(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            string partyType = String.Empty;
            string partyCode = String.Empty;
            string partyName = String.Empty;
            string address1 = String.Empty;
            string address2 = String.Empty;
            string address3 = String.Empty;
            string address4 = String.Empty;
            string contact = String.Empty;
            string tel = String.Empty;
            string fax = String.Empty;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "party-type") partyType = reader.ReadInnerXml();
                        else if (reader.Name == "party-code") partyCode = reader.ReadInnerXml();
                        else if (reader.Name == "party-name") partyName = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "address-line-1") address1 = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "address-line-2") address2 = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "address-line-3") address3 = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "address-line-4") address4 = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "contact") contact = HttpUtility.HtmlDecode(reader.ReadInnerXml());
                        else if (reader.Name == "telephone") tel = reader.ReadInnerXml();
                        else if (reader.Name == "fax") fax = reader.ReadInnerXml();
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }

            sw.WriteLine("INSERT EBookingParty VALUES ('{0}', '{1}', {2}, '{3}', '{4}', '{5}', ", batchNo, txNo, contractId.ToString(), partyType, partyCode, partyName.Replace("'", "''"));
            sw.WriteLine("'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')", address1.Replace("'", "''"), address2.Replace("'", "''"), address3.Replace("'", "''"), address4.Replace("'", "''"), contact.Replace("'", "''"), tel, fax);

            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
        }

        private void processRecord205(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            string packType = String.Empty;

            reader.Read();
            while (!reader.EOF)
            {

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "pack-type") packType = reader.ReadInnerXml();
                        else if (reader.Name == "record-type-206") processRecord206(reader.ReadOuterXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            reader.Close();
            sw.WriteLine("UPDATE EBookingShipment SET RatioPackType = '{0}'  WHERE  batchNo = '{1}' and transactionNo = '{2}'", packType, batchNo, txNo);

            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
        }

        private void processRecord206(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            string optionNo = String.Empty;
            int quantity = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "option") optionNo = reader.ReadInnerXml();
                        else if (reader.Name == "quantity") quantity = int.Parse(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }

            }
            reader.Close();
            sw.WriteLine("UPDATE eBookingSizeOption SET RatioPack = {0} WHERE batchNo = '{1}' and transactionNo = '{2}' and optionNo = '{3}'", quantity, batchNo, txNo, optionNo);

            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());

        }

        private void processRecord998(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            StringWriter sw = new StringWriter();

            int recordCount = 0;

            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "no-of-records") recordCount = int.Parse(reader.ReadInnerXml());
                        else reader.Read();
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            reader.Close();
            sw.WriteLine("UPDATE EBookingTransaction SET NoOfRecord = '{0}' WHERE BatchNo = '{1}' AND TransactionNo = '{2}'", recordCount.ToString(), batchNo, txNo);

            System.Diagnostics.Debug.WriteLine(sw.ToString());
            bookingUploadXMLWorker.insertEBookingBatch(sw.ToString());
        }
    }
}