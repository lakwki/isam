using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using com.next.infra.util;
using com.next.infra.configuration.appserver;
using com.next.common.domain;
using com.next.common.domain.industry.vendor;
using com.next.common.datafactory.worker;
using com.next.isam.domain.account;
using com.next.isam.domain.types;
using com.next.isam.domain.shipping;
using com.next.isam.domain.order;
using com.next.isam.domain.ils;
using com.next.common.domain.types;
using com.next.isam.domain.common;
using com.next.isam.domain.claim;
using com.next.isam.domain.product;
using com.next.isam.domain.nontrade;
using com.next.isam.dataserver.worker;
using com.next.common.web.commander;
using com.next.infra.util;
using com.next.common.datafactory.worker.industry;

namespace com.next.isam.dataserver.worker
{
    public class NoticeHelper : MailHelper
    {
        public NoticeHelper() { }

        private static new string sender = ApplicationServerConfigurator.getValue("mail", "adminName") + "<" + ApplicationServerConfigurator.getValue("mail", "adminMailAddress") + ">";
        private static GeneralWorker gworker = GeneralWorker.Instance;
        private static NssWorker nworker = NssWorker.Instance;
        private static string LOCAL_SERVER_NAME = ApplicationServerConfigurator.getValue("mail", "localServerName");

        private static string styleStr =
            "<LINK type=\"text/css\" rel=\"stylesheet\"> " +
            "<STYLE TYPE=\"text/css\"> " +
            "TD {font-family: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; font-size: 12px;} " +
            ".gridHeader {	FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold;	background-color: #ccccff;} " +
            ".colHeader{ FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold; background-color: #f5f5f5;	text-align: Center; vertical-align : middle; } " +
            ".dataCellStr{ FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold;	text-align: Center;	vertical-align : middle;} " +
            ".dataCellNum{ FONT-SIZE: 12px; FONT-FAMILY: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; COLOR: #000000; font-weight: bold;	text-align: Right;	vertical-align : middle;} " +
            "span { font-family: Tahoma, Verdana, Geneva, Arial, Helvetica, sans-serif; font-size: 12px;}" +
            "</STYLE>";

        public static void sendGeneralMessage(string subject, string content)
        {
            MailHelper.sendGeneralMessage(subject, content);
        }


        public static void sendMissingVendorIdDataSyncError(int shipmentId)
        {
            MessageContent mc = getContent("MissingVendorIdDataSyncError");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "ricky_ng@nextsl.com.hk;kawai_fung@nextsl.com.hk";
            mc.Subject = "Missing Vendor Id For Shipment Id = " + shipmentId.ToString();
            Mail.send(mc);
        }

        public static void sendErrorAlert(Exception exp, UserRef userRef)
        {
            ArrayList locList = new ArrayList();

            MessageContent mc = getContent("ErrorAlert");
            mc.Priority = System.Net.Mail.MailPriority.High;
            mc.From = sender;
            string username = userRef != null ? userRef.DisplayName : String.Empty;
            username += " / " + LOCAL_SERVER_NAME;
            mc.setBodyParams(exp.Message, exp.InnerException, exp.Source, exp.StackTrace, exp.ToString(), username);
            Mail.send(mc);
        }

        public static void sendILSInvoiceNoChangeEmail(string orderRef, string oldInvoiceNo, string newInvoiceNo, int orderRefId, string currency, decimal invoiceAmt)
        {
            MessageContent mc = getContent("ILSInvoiceNoChangeEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.Subject = "Change of Invoice No. From ILS [Order Ref.: " + orderRef + "]";
            mc.setBodyParams(orderRef, oldInvoiceNo, newInvoiceNo, orderRefId.ToString(), currency, invoiceAmt.ToString("#,###.00"));
            Mail.send(mc);
        }

        public static void sendILSCancelledInvoiceEmail(string orderRef, string oldInvoiceNo, string newInvoiceNo, int orderRefId, string currency, decimal invoiceAmt)
        {
            MessageContent mc = getContent("ILSCancelledInvoiceEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.Subject = "ILS Invoice No. Has Been Cancelled By ILS [Order Ref.: " + orderRef + "]";
            mc.setBodyParams(orderRef, oldInvoiceNo, newInvoiceNo, orderRefId.ToString(), currency, invoiceAmt.ToString("#,###.00"));
            Mail.send(mc);
        }

        public static void sendILSCancelledInvoiceNoChangeEmail(string orderRef, string oldInvoiceNo, string newInvoiceNo, int orderRefId, string currency, decimal invoiceAmt)
        {
            MessageContent mc = getContent("ILSCancelledInvoiceNoChangeEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.Subject = "Change of Cancelled Invoice No. From ILS [Order Ref.: " + orderRef + "]";
            mc.setBodyParams(orderRef, oldInvoiceNo, newInvoiceNo, orderRefId.ToString(), currency, invoiceAmt.ToString("#,###.00"));
            Mail.send(mc);
        }

        public static void sendILSCommissionInvoiceNotMatchEmail(string invoiceNo, string fileNo)
        {
            MessageContent mc = getContent("ILSCommissionInvoiceNotMatchEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.setBodyParams(invoiceNo, fileNo);
            Mail.send(mc);
        }

        public static void sendOrderRefWithUnassignedShipmentIdEmail(string fileNo, string actionText, string orderRef, DateTime d)
        {
            MessageContent mc = getContent("OrderRefWithUnassignedShipmentIdEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk;cliff_wong@nextsl.com.hk";
            mc.Subject = "Unassigned Shipment Id Detected For Order Ref# [" + orderRef + "]";
            mc.setBodyParams(fileNo, actionText, orderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendShipmentIdConflictEmail(string orderRef, string anotherOrderRef, int shipmentId)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(shipmentId);
            ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);

            MessageContent mc = getContent("ShipmentIdConflictEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Shipment Id Conflicts For Order Ref# [" + orderRef + "]";
            mc.setBodyParams(orderRef, anotherOrderRef, contractDef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0'));
            Mail.send(mc);
        }

        public static void sendInvoicedShipmentCancelledByILSEmail(string fileNo, ILSOrderRefDef ilsOrderRef, string invoiceNo, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("InvoicedShipmentCancelledByILSEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "NUK Cancelled an Invoiced Shipment [Invoice#: " + invoiceNo + ", Contract#: " + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0');
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, invoiceNo, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendCancelledShipmentCancelledByILSEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("CancelledShipmentCancelledByILSEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "NUK Cancelled a Cancelled Shipment [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendContractReinstateByILSEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("ContractReinstateByILSEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Contract Reinstate By ILS [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendPackingListUploadAgainstInvoicedShipmentEmail(string fileNo, ILSOrderRefDef ilsOrderRef, string invoiceNo, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("PackingListUploadAgainstInvoicedShipmentEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "P/L Upload Against an Invoiced Shipment [Invoice#: " + invoiceNo + ", Contract#: " + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0');
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, invoiceNo, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendPackingListUploadAgainstCancelledShipmentEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("PackingListUploadAgainstCancelledShipmentEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "P/L Upload Against a Cancelled Shipment [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendPackingListUploadTurnedOffEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("PackingListUploadTurnedOffEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "P/L Upload Suspended [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendInvoiceUploadInvoiceDateToleranceEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime invoiceDate, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("InvoiceUploadInvoiceDateToleranceEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Failure Due To Invoice Date Not Within 60 Days Tolerance [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, invoiceDate.ToString("dd/MM/yyyy HH:mm:ss"), d.ToString("dd/MM/yyyy hh:mm:ss"));
            Mail.send(mc);
        }

        public static void sendInvoiceUploadMissingPackingListEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("InvoiceUploadMissingPackingListEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Failure Due To Missing Packing List Data [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendInvoiceUploadCurrencyMismatchEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("InvoiceUploadCurrencyMismatchEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Failure Due To Currency Mismatch [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendInvoiceUploadMissingFactoryIdForNMLEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("InvoiceUploadMissingFactoryIdForNMLEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Failure Due To Missing Factory Id For NML [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendInvoiceUploadNotSelfBilledEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("InvoiceUploadNotSelfBilledEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Failure - Not a Self-Billed Order [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendInvoiceUploadAgainstInvoicedShipmentEmail(string fileNo, ILSOrderRefDef ilsOrderRef, string invoiceNo, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("InvoiceUploadAgainstInvoicedShipmentEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Against an Invoiced Shipment [Invoice#: " + invoiceNo + ", Contract#: " + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0');
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, invoiceNo, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendInvoiceUploadAgainstCancelledShipmentEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("InvoiceUploadAgainstCancelledShipmentEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Against a Cancelled Shipment [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendInvoiceUploadAgainstNotYetApprovedShipmentEmail(string fileNo, ILSOrderRefDef ilsOrderRef, ContractWFS workflowStatus, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);

            MessageContent mc = getContent("InvoiceUploadNotYetApprovedEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Against Not-Yet Approved Shipment [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, workflowStatus.Name, d.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendInvoiceUploadInvoiceNoUsedByAnotherShipmentEmail(string fileNo, ILSOrderRefDef ilsOrderRef, InvoiceDef existingInvoice, DateTime d, string ilsInvoiceNo)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);
            ShipmentDef existingShipmentDef = OrderSelectWorker.Instance.getShipmentByKey(existingInvoice.ShipmentId);
            ContractDef existingContractDef = OrderSelectWorker.Instance.getContractByKey(existingShipmentDef.ContractId);

            MessageContent mc = getContent("InvoiceUploadInvoiceNoUsedByAnotherShipmentEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice No Being Used By Another Shipment [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, existingContractDef.ContractNo + "-" + existingShipmentDef.DeliveryNo.ToString().PadLeft(2, '0'), d.ToString("dd/MM/yyyy HH:mm:ss"), ilsInvoiceNo);
            Mail.send(mc);
        }

        public static void sendInvoiceUploadOptionMismatchEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);
            ArrayList shipmentDetails = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(ilsOrderRef.ShipmentId);
            ArrayList ilsInvoiceDetails = ILSUploadWorker.Instance.getILSInvoiceDetailList(ilsOrderRef.OrderRefId);
            bool isOptionFound = false;

            string html = "<TABLE BORDER=1><TR><TD CLASS='gridHeader'>&nbsp;&nbsp;ILS Option No&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Qty In ILS&nbsp;&nbsp;</TD></TR>";
            foreach (ILSInvoiceDetailDef ilsInvoiceDetailDef in ilsInvoiceDetails)
            {
                isOptionFound = false;
                foreach (ShipmentDetailDef shipmentDetailDef in shipmentDetails)
                {
                    if (shipmentDetailDef.SizeOption.SizeOptionNo == ilsInvoiceDetailDef.OptionNo)
                        isOptionFound = true;
                }
                if (!isOptionFound)
                    html += "<TR><TD CLASS='dataCellStr'>" + ilsInvoiceDetailDef.OptionNo + "</TD><TD CLASS='dataCellStr'>"
                         + ilsInvoiceDetailDef.Qty.ToString("#,##0") + "</TD></TR>";
            }
            html += "</TABLE>";

            MessageContent mc = getContent("InvoiceUploadOptionMismatchEmail");
            mc.IsHtmlFormat = true;
            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Failure Due To Option Mismatch [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"), html);

            Mail.send(mc);
        }

        public static void sendInvoiceUploadQuantityMismatchEmail(string fileNo, ILSOrderRefDef ilsOrderRef, DateTime d)
        {
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(ilsOrderRef.ShipmentId);
            ArrayList shipmentDetails = (ArrayList)OrderSelectWorker.Instance.getShipmentDetailByShipmentId(ilsOrderRef.ShipmentId);
            ArrayList ilsInvoiceDetails = ILSUploadWorker.Instance.getILSInvoiceDetailList(ilsOrderRef.OrderRefId);
            int totalILSQty = 0;
            int totalISAMQty = 0;
            bool isOptionFound = false;

            string html = "<TABLE BORDER=1><TR><TD CLASS='gridHeader'>&nbsp;&nbsp;Option No&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Qty In ILS&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Qty In ISAM&nbsp;&nbsp;</TD></TR>";
            foreach (ILSInvoiceDetailDef ilsInvoiceDetailDef in ilsInvoiceDetails)
            {
                totalILSQty += ilsInvoiceDetailDef.Qty;
                html += "<TR><TD CLASS='dataCellStr'>" + ilsInvoiceDetailDef.OptionNo + "</TD><TD CLASS='dataCellStr'>"
                     + ilsInvoiceDetailDef.Qty.ToString("#,##0") + "</TD><TD CLASS='dataCellStr'>";
                isOptionFound = false;
                foreach (ShipmentDetailDef shipmentDetailDef in shipmentDetails)
                {
                    if (shipmentDetailDef.SizeOption.SizeOptionNo == ilsInvoiceDetailDef.OptionNo)
                    {
                        isOptionFound = true;
                        totalISAMQty += shipmentDetailDef.ShippedQuantity;
                        html += shipmentDetailDef.ShippedQuantity.ToString("#,##0") + "</TD></TR>";
                    }
                }
                if (!isOptionFound)
                    html += "0</TD></TR>";
            }
            html += "<TR><TD CLASS='dataCellStr'>TOTAL</TD><TD CLASS='dataCellStr'>"
                     + totalILSQty.ToString("#,##0") + "</TD><TD CLASS='dataCellStr'>" + totalISAMQty.ToString("#,##0") + "</TD></TR></TABLE>";

            MessageContent mc = getContent("InvoiceUploadQuantityMismatchEmail");
            mc.IsHtmlFormat = true;
            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Invoice Upload Failure Due To Qty Mismatch [" + ilsOrderRef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString().PadLeft(2, '0') + "]";
            mc.setBodyParams(fileNo, ilsOrderRef.OrderRef, d.ToString("dd/MM/yyyy HH:mm:ss"), html);

            Mail.send(mc);
        }

        public static void sendIncompleteOutgoingILSMsgEmail(string list)
        {
            MessageContent mc = getContent("IncompleteOutgoingILSMsgEmail");
            mc.From = sender;
            mc.Subject = "Incomplete ILS Outgoing Msg List As Of " + DateTime.Now.ToString();
            mc.setBodyParams(list);
            Mail.send(mc);
        }

        public static void sendILSResultErrorEmail(string fileNo, string orderRef, string errDesc)
        {
            MessageContent mc = getContent("ILSResultErrorEmail");
            mc.From = sender;
            mc.Subject = "Update Error From ILS [" + orderRef + "]";
            mc.setBodyParams(fileNo, orderRef, errDesc);
            Mail.send(mc);
        }

        public static void sendDMSUploadDocEmail(string officeCode, string filename, string path, int actionId)
        {
            MessageContent mc = getContent("DMSUploadDocEmail");
            mc.From = sender;
            mc.Subject = officeCode + " [" + filename + "] was " + (actionId == 1 ? "updated" : "uploaded") + " successfully";
            ArrayList attachments = new ArrayList();
            attachments.Add(path);
            mc.Attachments = attachments;
            Mail.send(mc);
        }

        public static void sendDMSAPIUploadError(string src, string errorText, string path)
        {
            MessageContent mc = getContent("DMSAPIUploadErrorEmail");
            mc.From = sender;
            mc.Subject = "DMS API Upload Error (Source : " + src + ")";
            ArrayList attachments = new ArrayList();
            attachments.Add(path);
            mc.Attachments = attachments;
            mc.setBodyParams(errorText);
            Mail.send(mc);
        }


        public static void sendPackingListMissingBoxInfoEmail(string fileNo, string orderRef, string optionNo)
        {
            MessageContent mc = getContent("PackingListMissingBoxInfoEmail");
            mc.From = sender;
            mc.Subject = "Missing Carton Details in ILS Packing List [" + orderRef + "]";
            mc.setBodyParams(fileNo, orderRef, optionNo);
            Mail.send(mc);
        }

        public static void sendNSSDiscrepancyMail(string result, ArrayList attachmentList)
        {
            try
            {
                MessageContent mc = getContent("NSSDiscrepancyEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                mc.To = "michael_lau@nextsl.com.hk;tobylo@nextsl.com.hk;alan_foo@nextsl.com.hk;cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk";
                mc.Subject = "ISAM - NSS Discrepancy As Of " + DateTimeUtility.getDateString(DateTime.Today);
                mc.Attachments = attachmentList;
                mc.setBodyParams(result);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendEziBuyInvoiceMail(string filePath, string inboundDeliveryNo)
        {
            try
            {
                ArrayList attachments = new ArrayList();
                MessageContent mc = new MessageContent();
                mc.StyleSheet = styleStr;
                mc.From = "nsl_logistic@nextsl.com.hk";
                mc.To = "ezibuy.commercialinvoice@bbssmm.com";
                mc.Cc = "jessie@nextsl.com.hk; catherine@nextsl.com.hk; may@nextsl.com.hk; tammy_lee@nextsl.com.hk; daisy_wong@nextsl.com.hk; toby_lo@nextsl.com.hk; michael_lau@nextsl.com.hk; nsl_logistic@nextsl.com.hk; peter@globaltextiles.co.nz; phil@globaltextiles.co.nz;";
                mc.Bcc = "cliff_wong@nextsl.com.hk; louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";
                mc.Subject = inboundDeliveryNo;
                attachments.Add(filePath);
                mc.Attachments = attachments;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendEziBuyInvoiceMail(string filePath, string alertFilePath, ArrayList invoiceList, DateTime genDate)
        {
            try
            {
                string html = "<TABLE BORDER=1><TR><TD CLASS='gridHeader'>&nbsp;&nbsp;PO Number&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Inbound Delivery No&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;NSL Invoice Number&nbsp;&nbsp;</TD></TR>";
                ArrayList attachments = new ArrayList();
                MessageContent mc = getContent("EziBuyInvoiceEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                //mc.To = "michael_lau@nextsl.com.hk";
                mc.To = "peter@globaltextiles.co.nz; phil@globaltextiles.co.nz; natasha@globaltextiles.co.nz"; // Peter Meadowcroft, Phil Sheenhan, Natasha Kimura
                mc.Cc = "jessie@nextsl.com.hk; catherine@nextsl.com.hk; may@nextsl.com.hk; aska_ya@sh.nextsl.com.cn; anton@nextsl.com.lk; semra_mutlu@nextsl.com.tr; tammy_lee@nextsl.com.hk; flora_fungty@nextsl.com.hk; daphne_yip@nextsl.com.hk; daisy_wong@nextsl.com.hk; jenny_chow@nextsl.com.hk;toby_lo@nextsl.com.hk; michael_lau@nextsl.com.hk; ";
                mc.Bcc = "cliff_wong@nextsl.com.hk; louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";
                mc.Subject = "NSL invoices – [" + DateTimeUtility.getDateString(genDate) + "]";
                attachments.Add(alertFilePath);
                if (invoiceList.Count > 0)
                {
                    attachments.Add(filePath);
                    foreach (InvoiceDef def in invoiceList)
                    {
                        ContractDef contractDef = OrderSelectWorker.Instance.getContractByShipmentId(def.ShipmentId);
                        html += "<TR><TD CLASS='dataCellStr'>" + contractDef.NSLPONo + "</TD><TD CLASS='dataCellStr'>"
                             + contractDef.BookingRefNo + "</TD><TD CLASS='dataCellStr'>" + def.InvoiceNo + "</TD></TR>";
                    }
                }
                html += "</TABLE>";
                mc.Attachments = attachments;
                mc.setBodyParams(DateTimeUtility.getDateString(genDate), html);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendEziBuyInvoiceEmptyMail(string alertFilePath, DateTime genDate)
        {
            try
            {
                ArrayList attachments = new ArrayList();
                MessageContent mc = getContent("EziBuyInvoiceEmptyEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                mc.To = "peter@globaltextiles.co.nz; phil@globaltextiles.co.nz; natasha@globaltextiles.co.nz"; // Peter Meadowcroft, Phil Sheenhan, Natasha Kimura
                mc.Cc = "sulia_leung@nextsl.com.hk; jessie@nextsl.com.hk; catherine@nextsl.com.hk; may@nextsl.com.hk; aska_ya@sh.nextsl.com.cn; anton@nextsl.com.lk; semra_mutlu@nextsl.com.tr; tammy_lee@nextsl.com.hk; flora_fungty@nextsl.com.hk; daphne_yip@nextsl.com.hk; daisy_wong@nextsl.com.hk; jenny_chow@nextsl.com.hk;toby_lo@nextsl.com.hk; michael_lau@nextsl.com.hk; ";
                mc.Bcc = "cliff_wong@nextsl.com.hk; louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";
                mc.Subject = "NSL invoices – [" + DateTimeUtility.getDateString(genDate) + "]";
                attachments.Add(alertFilePath);
                mc.Attachments = attachments;
                mc.setBodyParams(String.Empty);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendSunInterfaceMail(SunInterfaceQueueDef def, string filePath)
        {
            try
            {
                string tmp = def.OfficeGroup.ShortName + " ";
                if (def.CategoryType.Id != CategoryType.REVERSAL.Id && def.CategoryType.Id != CategoryType.DAILY.Id)
                    tmp += def.Period.ToString().PadLeft(2, '0') + "/" + def.FiscalYear.ToString() + " " + def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                else
                    tmp += def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                if (def.UTurnText != string.Empty)
                    tmp += (" " + def.UTurnText);
                MessageContent mc = getContent("SunInterfaceEmail");
                mc.From = sender;
                mc.To = def.User.EmailAddress;
                mc.Cc = "suninterface@nextsl.com.hk";

                if (((1 == 1) || (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.Purchase.GetHashCode() && (def.CategoryType.Id == CategoryType.DAILY.Id || def.CategoryType.Id == CategoryType.REVERSAL.Id))) && def.SourceId == 2)
                {
                    ArrayList officeList = CommonWorker.Instance.getOfficeListByReportOfficeGroupId(def.OfficeGroup.OfficeGroupId);
                    foreach (OfficeRef o in officeList)
                    {
                        string emailList = (";" + CommonWorker.Instance.getSystemParameterByName(o.OfficeCode.ToString().ToUpper() + "_CUTOFF_INTERFACE_EMAIL_CC").ParameterValue);
                        mc.Cc += emailList;

                        /*
                        if (o.OfficeId == OfficeId.HK.Id || o.OfficeId == OfficeId.DG.Id || o.OfficeId == OfficeId.CA.Id || o.OfficeId == OfficeId.TH.Id)
                            mc.To += ";cathytse@nextsl.com.hk;avis_lam@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.SH.Id)
                            mc.To += ";ivan_chong@nextsl.com.hk;winnie_lui@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.VN.Id)
                            mc.To += ";ruby_lai@nextsl.com.hk;winnie_lui@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.PK.Id)
                            mc.To += ";ivan_chong@nextsl.com.hk;carmen_cheung@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.SL.Id)
                            mc.To += ";candice_ng@nextsl.com.hk;carmen_cheung@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.IND.Id || o.OfficeId == OfficeId.ND.Id)
                            mc.To += ";ricky_chan@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.TR.Id || o.OfficeId == OfficeId.EG.Id || o.OfficeId == OfficeId.BD.Id)
                            mc.To += ";flora_fungty@nextsl.com.hk;cindy_mok@nextsl.com.hk;candice_ng@nextsl.com.hk;vanessa_ng@nextsl.com.hk";
                        */
                    }

                }
                /*
                if (def.SourceId == 2)
                    mc.Cc = "suninterface@nextsl.com.hk";
                */
                mc.Subject = "Epicor Interface File (" + tmp + ")";
                mc.setBodyParams(def.User.DisplayName, tmp, def.QueueId.ToString(), def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), def.SourceId == 1 ? "N" : "Y");
                mc.Attachments = ConvertUtility.createArrayList(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendEpicorInterfaceMail(SunInterfaceQueueDef def, ArrayList fileNameList)
        {
            try
            {
                string tmp = def.OfficeGroup.ShortName + " ";
                if (def.CategoryType.Id != CategoryType.REVERSAL.Id && def.CategoryType.Id != CategoryType.DAILY.Id)
                    tmp += def.Period.ToString().PadLeft(2, '0') + "/" + def.FiscalYear.ToString() + " " + def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                else
                    tmp += def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                if (def.UTurnText != string.Empty)
                    tmp += (" " + def.UTurnText);
                MessageContent mc = getContent("EpicorInterfaceEmail");
                mc.From = sender;
                mc.To = def.User.EmailAddress;
                mc.Cc = "suninterface@nextsl.com.hk";

                if (((1 == 1) || (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.Purchase.GetHashCode() && (def.CategoryType.Id == CategoryType.DAILY.Id || def.CategoryType.Id == CategoryType.REVERSAL.Id))) && def.SourceId == 2)
                {
                    ArrayList officeList = CommonWorker.Instance.getOfficeListByReportOfficeGroupId(def.OfficeGroup.OfficeGroupId);
                    foreach (OfficeRef o in officeList)
                    {
                        string emailList = (";" + CommonWorker.Instance.getSystemParameterByName(o.OfficeCode.ToString().ToUpper() + "_CUTOFF_INTERFACE_EMAIL_CC").ParameterValue);
                        mc.Cc += emailList;
                        /*
                        if (o.OfficeId == OfficeId.HK.Id || o.OfficeId == OfficeId.DG.Id || o.OfficeId == OfficeId.CA.Id || o.OfficeId == OfficeId.TH.Id)
                            mc.To += ";cathytse@nextsl.com.hk;avis_lam@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.SH.Id)
                            mc.To += ";ivan_chong@nextsl.com.hk;winnie_lui@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.VN.Id)
                            mc.To += ";ruby_lai@nextsl.com.hk;winnie_lui@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.PK.Id)
                            mc.To += ";ivan_chong@nextsl.com.hk;carmen_cheung@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.SL.Id)
                            mc.To += ";candice_ng@nextsl.com.hk;carmen_cheung@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.IND.Id || o.OfficeId == OfficeId.ND.Id)
                            mc.To += ";ricky_chan@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.TR.Id || o.OfficeId == OfficeId.EG.Id || o.OfficeId == OfficeId.BD.Id)
                            mc.To += ";flora_fungty@nextsl.com.hk;cindy_mok@nextsl.com.hk;candice_ng@nextsl.com.hk;vanessa_ng@nextsl.com.hk";
                        */
                    }

                }

                mc.Subject = "Epicor Interface File (" + tmp + ")";
                mc.setBodyParams(def.User.DisplayName, tmp, def.QueueId.ToString(), def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), def.SourceId == 1 ? "N" : "Y");
                mc.Attachments = fileNameList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendEpicorInterfaceMail(NTSunInterfaceQueueDef def, ArrayList fileNameList)
        {
            try
            {
                string tmp = def.Office.OfficeCode + " ";
                tmp += def.Period.ToString().PadLeft(2, '0') + "/" + def.FiscalYear.ToString() + " " + def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                MessageContent mc = getContent("EpicorInterfaceEmail");
                mc.From = sender;
                mc.To = def.User.EmailAddress;
                mc.Cc = "suninterface@nextsl.com.hk";
                ArrayList ccList = new ArrayList();
                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeExpenseInvoice.GetHashCode())
                {
                    ArrayList companyList = NonTradeWorker.Instance.getNTUserCompanyList(def.User.UserId, def.Office.OfficeId, NTRoleType.FIRST_APPROVER.Id);

                    foreach (CompanyRef companyRef in companyList)
                    {
                        ArrayList userList = NonTradeWorker.Instance.getNTUserList(NTRoleType.SECOND_APPROVER.Id, companyRef.CompanyId, def.Office.OfficeId);
                        foreach (UserRef u in userList)
                        {
                            if (!ccList.Contains(u.UserId))
                            {
                                ccList.Add(u.UserId);
                                mc.Cc += (";" + u.EmailAddress);
                            }
                        }
                    }
                }

                mc.Subject = "Epicor Interface File (" + tmp + ")";
                mc.setBodyParams(def.User.DisplayName, tmp, def.QueueId.ToString(), def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), def.SourceId == 1 ? "N" : "Y");
                mc.Attachments = fileNameList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendSunInterfaceMail(NTSunInterfaceQueueDef def, ArrayList fileNameList)
        {
            try
            {
                string tmp = def.Office.OfficeCode + " ";
                tmp += def.Period.ToString().PadLeft(2, '0') + "/" + def.FiscalYear.ToString() + " " + def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                MessageContent mc = getContent("SunInterfaceEmail");
                mc.From = sender;
                mc.To = def.User.EmailAddress;
                mc.Cc = "suninterface@nextsl.com.hk";
                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeExpenseInvoice.GetHashCode())
                {
                    ArrayList companyList = NonTradeWorker.Instance.getNTUserCompanyList(def.User.UserId, -1, NTRoleType.FIRST_APPROVER.Id);

                    foreach (CompanyRef companyRef in companyList)
                    {
                        ArrayList userList = NonTradeWorker.Instance.getNTUserList(NTRoleType.SECOND_APPROVER.Id, companyRef.CompanyId, def.Office.OfficeId);
                        foreach (UserRef u in userList)
                            mc.Cc += (";" + u.EmailAddress);
                    }
                }

                mc.Subject = "Epicor Interface File (" + tmp + ")";
                mc.setBodyParams(def.User.DisplayName, tmp, def.QueueId.ToString(), def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), def.SourceId == 1 ? "N" : "Y");
                mc.Attachments = fileNameList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendSunInterfaceMail(string overrideRecipientList, NTSunInterfaceQueueDef def, ArrayList fileNameList)
        {
            try
            {
                string tmp = def.Office.OfficeCode + " ";
                tmp += def.Period.ToString().PadLeft(2, '0') + "/" + def.FiscalYear.ToString() + " " + def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                MessageContent mc = getContent("SunInterfaceEmail");
                mc.From = sender;
                mc.To = overrideRecipientList;
                mc.Cc = def.User.EmailAddress + ";suninterface@nextsl.com.hk";
                mc.Subject = "Epicor Interface File (" + tmp + ")";
                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeExpenseInvoice.GetHashCode())
                {
                    ArrayList companyList = NonTradeWorker.Instance.getNTUserCompanyList(def.User.UserId, -1, NTRoleType.FIRST_APPROVER.Id);

                    foreach (CompanyRef companyRef in companyList)
                    {
                        ArrayList userList = NonTradeWorker.Instance.getNTUserList(NTRoleType.SECOND_APPROVER.Id, companyRef.CompanyId, def.Office.OfficeId);
                        foreach (UserRef u in userList)
                            mc.Cc += (";" + u.EmailAddress);
                    }
                }
                if (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.NonTradeBankPayment.GetHashCode())
                {
                    mc.To = def.User.EmailAddress;
                    mc.Cc = overrideRecipientList + ";suninterface@nextsl.com.hk";
                }

                mc.setBodyParams(def.User.DisplayName, tmp, def.QueueId.ToString(), def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), def.SourceId == 1 ? "N" : "Y");
                mc.Attachments = fileNameList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendSunInterfaceSupportingMail(string overrideRecipientList, NTSunInterfaceQueueDef def, ArrayList fileNameList)
        {
            try
            {
                string tmp = def.Office.OfficeCode + " ";
                tmp += def.Period.ToString().PadLeft(2, '0') + "/" + def.FiscalYear.ToString() + " " + def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                MessageContent mc = getContent("SunInterfaceSupportingEmail");
                mc.From = sender;
                mc.To = overrideRecipientList;
                mc.Cc = def.User.EmailAddress + ";suninterface@nextsl.com.hk";
                mc.Subject = "Supporting File Of Epicor Interface File (" + tmp + ")";
                mc.setBodyParams(def.User.DisplayName, tmp, def.QueueId.ToString(), def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), def.SourceId == 1 ? "N" : "Y");
                mc.Attachments = fileNameList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendEmptySunInterfaceMail(SunInterfaceQueueDef def)
        {
            try
            {
                string tmp = def.OfficeGroup.ShortName + " ";
                if (def.CategoryType.Id != CategoryType.REVERSAL.Id)
                    tmp += def.FiscalYear.ToString() + "/" + def.Period.ToString().PadLeft(2, '0') + " " + def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                else
                    tmp += def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                if (def.UTurnText != string.Empty)
                    tmp += (" " + def.UTurnText);
                MessageContent mc = getContent("EmptySunInterfaceEmail");
                mc.From = sender;
                mc.To = def.User.EmailAddress;
                mc.Cc = "suninterface@nextsl.com.hk";

                if (((1 == 1) || (def.SunInterfaceTypeId == SunInterfaceTypeRef.Id.Purchase.GetHashCode() && (def.CategoryType.Id == CategoryType.DAILY.Id || def.CategoryType.Id == CategoryType.REVERSAL.Id))) && def.SourceId == 2)
                {
                    ArrayList officeList = CommonWorker.Instance.getOfficeListByReportOfficeGroupId(def.OfficeGroup.OfficeGroupId);
                    foreach (OfficeRef o in officeList)
                    {
                        string emailList = (";" + CommonWorker.Instance.getSystemParameterByName(o.OfficeCode.ToString().ToUpper() + "_CUTOFF_INTERFACE_EMAIL_CC").ParameterValue);
                        mc.Cc += emailList;
                        /*
                        if (o.OfficeId == OfficeId.HK.Id || o.OfficeId == OfficeId.DG.Id || o.OfficeId == OfficeId.CA.Id || o.OfficeId == OfficeId.TH.Id)
                            mc.To += ";cathytse@nextsl.com.hk;avis_lam@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.SH.Id)
                            mc.To += ";ivan_chong@nextsl.com.hk;winnie_lui@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.VN.Id)
                            mc.To += ";ruby_lai@nextsl.com.hk;winnie_lui@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.PK.Id)
                            mc.To += ";ivan_chong@nextsl.com.hk;carmen_cheung@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.SL.Id)
                            mc.To += ";candice_ng@nextsl.com.hk;carmen_cheung@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.IND.Id || o.OfficeId == OfficeId.ND.Id)
                            mc.To += ";ricky_chan@nextsl.com.hk";
                        else if (o.OfficeId == OfficeId.TR.Id || o.OfficeId == OfficeId.EG.Id || o.OfficeId == OfficeId.BD.Id)
                            mc.To += ";flora_fungty@nextsl.com.hk;cindy_mok@nextsl.com.hk;candice_ng@nextsl.com.hk;vanessa_ng@nextsl.com.hk";
                        */
                    }

                }
                mc.Subject = "Epicor Interface File (" + tmp + ")";
                mc.setBodyParams(def.User.DisplayName, tmp, def.QueueId.ToString(), def.SubmitTime.ToString("dd/MM/yyyy HH:mm:ss"), DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendEmptySunInterfaceMail(NTSunInterfaceQueueDef def)
        {
            try
            {
                string tmp = def.Office.OfficeCode + " ";
                tmp += def.FiscalYear.ToString() + def.Period.ToString().PadLeft(2, '0') + " " + def.CategoryType.Name + " " + SunInterfaceTypeRef.getDescription(def.SunInterfaceTypeId);
                MessageContent mc = getContent("EmptySunInterfaceEmail");
                mc.From = sender;
                mc.To = def.User.EmailAddress;
                mc.Subject = "Epicor Interface File (" + tmp + ")";
                mc.setBodyParams(def.User.DisplayName, tmp, def.QueueId.ToString(), def.SubmitTime.ToString("MM/dd/yyyy hh:mm:ss"), DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendInvoiceBatchMail(EInvoiceBatchDef def, ArrayList filePath, UserRef user)
        {
            try
            {
                MessageContent mc = getContent("InvoiceBatchEmail");
                mc.From = sender;
                mc.To = user.EmailAddress;
                mc.Subject = "Invoice Batch File : " + def.EInvoiceBatchNo;
                mc.setBodyParams(user.DisplayName, def.EInvoiceBatchNo);
                mc.Attachments = filePath;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendLCRejectBatchMail(string rejectedTable, int applicationUser, int userId)
        {
            UserRef user;
            UserRef appUser;

            try
            {
                user = GeneralWorker.Instance.getUserByKey(userId);
                appUser = GeneralWorker.Instance.getUserByKey(applicationUser);
                MessageContent mc = getContent("LCApplicationRejectByShippingEmail");
                mc.From = sender;
                mc.To = appUser.EmailAddress;
                mc.setBodyParams(appUser.DisplayName, rejectedTable, user.DisplayName);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendLCApplicationAppliedMail(ArrayList fileList, int userId)
        {
            string BatchString;
            UserRef User;

            try
            {
                User = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("LCApplicationAppliedEmail");
                mc.From = sender;
                mc.To = User.EmailAddress;
                //mc.Subject = "";
                BatchString = "";
                mc.setBodyParams(User.DisplayName, BatchString);
                mc.Attachments = fileList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendLCApplicationAppliedMail(string mailDetail, int userId)
        {
            string BatchString;
            UserRef User;

            try
            {
                User = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("LCApplicationAppliedEmail");
                mc.From = sender;
                mc.To = User.EmailAddress;
                //mc.Subject = "";
                BatchString = mailDetail;
                mc.setBodyParams(User.DisplayName, BatchString);
                //mc.Attachments = fileList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendLCApplicationAppliedMail(string mailDetail, int userId, string lcBatchNo)
        {
            string batchString;
            UserRef user;

            try
            {
                user = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("LCApplicationAppliedEmail");
                mc.From = sender;
                mc.To = user.EmailAddress;
                mc.Subject = "Applied L/C Application in LC Batch " + lcBatchNo;
                batchString = mailDetail;
                mc.setBodyParams(user.DisplayName, batchString);
                //mc.Attachments = fileList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }


        public static void sendSupplierDocRejectEmail(int shipmentId, int userId)
        {
            try
            {
                ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(shipmentId);
                InvoiceDef invoiceDef = ShippingWorker.Instance.getInvoiceByKey(shipmentId);
                ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
                ProductDef productDef = ProductWorker.Instance.getProductByKey(contractDef.ProductId);
                string ccList = CommonWorker.Instance.getSystemParameterByName(contractDef.Office.OfficeCode + "_REJECT_SUPPLIER_DOC_EMAIL").ParameterValue;

                MessageContent mc = getContent("SupplierDocRejectEmail");
                mc.From = sender;
                mc.To = ccList;
                mc.Cc = GeneralWorker.Instance.getUserByKey(userId).EmailAddress;
                mc.Subject = "Supplier Document of " + invoiceDef.InvoiceNo + " has been rejected";
                mc.setBodyParams(invoiceDef.InvoiceUploadUser.DisplayName, invoiceDef.InvoiceNo, DateTime.Now.ToString(), RejectPaymentReason.getReason(shipmentDef.RejectPaymentReasonId).Name, contractDef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString(), productDef.ItemNo, shipmentDef.Vendor.Name);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendLCSubmissionBatchMail(string submissionDetail, int applicationNo, int submitUserId, string ccMailAddress)
        {
            UserRef submitUser;
            try
            {
                submitUser = GeneralWorker.Instance.getUserByKey(submitUserId);
                MessageContent mc = getContent("LCApplicationSubmittedEmail");
                mc.From = sender;
                mc.To = submitUser.EmailAddress;
                string AppNo = applicationNo.ToString().PadLeft(6, char.Parse("0"));
                mc.Subject = "L/C Application " + AppNo + " has been submitted to Shipping Department.";
                mc.setBodyParams(submitUser.DisplayName, AppNo, submissionDetail);
                mc.Cc = ccMailAddress;
                mc.Bcc = "tobylo@nextsl.com.hk; michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk; louis_cheng@nextsl.com.hk; ";
                //mc.Attachments = fileList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendLCApprovalBatchMail(string approvedDetail, int applicationUserId, int approverId)
        {
            UserRef appUser;
            UserRef approver;

            try
            {
                appUser = GeneralWorker.Instance.getUserByKey(applicationUserId);
                approver = GeneralWorker.Instance.getUserByKey(approverId);
                MessageContent mc = getContent("LCApplicationApprovedByShippingEmail");
                mc.From = sender;
                mc.To = appUser.EmailAddress;
                mc.setBodyParams(appUser.DisplayName, approvedDetail, approver.DisplayName);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendLCExtractBatchMail(ArrayList fileList, string lcBatchNo, string supplierName, string remark, string shipmentList, int userId)
        {
            try
            {
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("LCApplicationExtractedByShippingEmail");
                mc.From = sender;
                mc.To = user.EmailAddress;
                mc.Subject = "L/C Application Data - " + lcBatchNo + (!string.IsNullOrEmpty(remark) ? remark : "");
                mc.setBodyParams(user.DisplayName, supplierName, remark, shipmentList);
                mc.Attachments = fileList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static string getErrorHtml(string msg, int lineNo)
        {
            string err = "";
            if (lineNo != -1 && lineNo != -99)
                err = "<TR><TD class=\"dataCellStr\" style=\"COLOR: blue;\"> LineNum " + lineNo.ToString() + " : </TD><TD class=\"dataCellStr\" style=\"COLOR: red;\"> " + msg + "</TD></TR>";
            else if (lineNo == -99)
                err = "<TR><TD style=\"WIDTH:100px FONT-WEIGHT: bold; COLOR: blue;\"> Mismatch Data : </TD><TD style=\"WIDTH:620px;  COLOR: red;\"> " + msg + "</TD></TR>";
            else
                err = "<TR><TD style=\"WIDTH:100px FONT-WEIGHT: bold; COLOR: blue;\"> File Type Error : </TD><TD style=\"WIDTH:620px;  COLOR: red;\"> " + msg + "</TD></TR>";
            return err;
        }

        public static void sendBankRecProcessErrorMessage(Exception e, UserRef userRef, string otherInfo)
        {
            MessageContent mc = getContent("ProcessBankReconciliationErrorAlert");
            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.To = userRef.EmailAddress;
            mc.setBodyParams(userRef.DisplayName, otherInfo, e.ToString());
            Mail.send(mc);
        }

        public static void sendNSLedPurchaseUploadAlert(string alertText, string itemNo, int officeId)
        {
            MessageContent mc = getContent("NSLedPurchaseUploadAlert");
            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            OfficeRef o = GeneralWorker.Instance.getOfficeRefByKey(officeId);
            string emailList = (";" + CommonWorker.Instance.getSystemParameterByName(o.OfficeCode.ToString().ToUpper() + "_CUTOFF_INTERFACE_EMAIL_CC").ParameterValue);
            mc.To += emailList;

            mc.Subject = "NS-LED Purchase Upload Error - Item #" + itemNo;
            mc.setBodyParams(alertText);

            Mail.send(mc);
        }

        public static void sendBankReconciliationMail(UserRef userRef, string bankName, string updateStatus, string error, string chargeFilePath)
        {
            MessageContent mc = getContent("eBankingRecon");
            mc.StyleSheet = styleStr;

            mc.From = sender;
            mc.To = userRef.EmailAddress;
            mc.Bcc = "cliff_wong@nextsl.com.hk; michael_lau@nextsl.com.hk; accounts@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";
            mc.Subject = bankName.ToUpper() + " statement reconciliation result";

            if (bankName != "SCB")
                mc.Attachments = ConvertUtility.createArrayList(chargeFilePath);
            mc.setBodyParams(userRef.DisplayName, bankName, error, updateStatus);
            string s = styleStr + "<html><body>" + mc.Body.ToString() + "</body></html>";
            Mail.send(mc);
        }

        public static void sendErrorMessage(Exception e, string additionalInfo)
        {
            MessageContent mc = getContent("SystemErrorAlert");
            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk;";
            mc.setBodyParams(Environment.MachineName, e.Message + "   " + additionalInfo);
            Mail.send(mc);
        }

        public static void sendeBankingFile(UserRef userRef, string bankName, string filePath, string sourceFile)
        {
            string[] sourceFileContents = sourceFile.Split('\\');
            string sourceFileName = sourceFileContents[sourceFileContents.Length - 1].ToString();

            string[] targetFileContents = filePath.Split('\\');
            string targetFileName = targetFileContents[targetFileContents.Length - 1].ToString();

            MessageContent mc = getContent("eBankingSendFile");
            mc.StyleSheet = styleStr;

            mc.From = sender;
            mc.To = userRef.EmailAddress;
            mc.Bcc = "cliff_wong@nextsl.com.hk; michael_lau@nextsl.com.hk; accounts@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";
            string subject = "Import Payment File For " + ((bankName == "HSBC") ? "HSBC Net" : "SCB WebBank");
            mc.Subject = subject;
            mc.Attachments = ConvertUtility.createArrayList(filePath);
            mc.setBodyParams(userRef.DisplayName, bankName, targetFileName, sourceFileName);
            Mail.send(mc);
        }

        public static void sendeBankingFileError(UserRef userRef, string bankName, string error, string fileName)
        {
            MessageContent mc = getContent("eBankingSendFileError");
            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.To = userRef.EmailAddress;
            mc.Bcc = "cliff_wong@nextsl.com.hk; toby_lo@nextsl.com.hk; michael_lau@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";
            mc.Subject = "Error - " + bankName + "'s eBanking file error!";
            mc.setBodyParams(userRef.DisplayName, fileName, error);
            Mail.send(mc);
        }

        public static void sendPaymentAdvice(VendorRef vendor, string filePath, int isChecked, string company, PaymentAdviceDef paymentAdvice)
        {
            try
            {
                MessageContent mc = getContent("PaymentAdvice");
                mc.StyleSheet = styleStr;
                mc.From = sender;

                if (isChecked == 1)
                {
                    mc.To = vendor.eAdviceAddr;
                    mc.Cc = "accounts@nextsl.com.hk";

                    SystemParameterRef sysPara = null;
                    if (vendor.OfficeId > 0 && GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId) != null)
                        sysPara = CommonWorker.Instance.getSystemParameterByName(GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId).OfficeCode.ToUpper().Trim() + "_PAYMENT_ADVICE_EMAIL_CC");
                    if (sysPara != null && vendor.VendorId != 6862) // U.S Apparel Requested By Annie Yick @2019-05-31
                        mc.Cc += ";" + sysPara.ParameterValue.TrimEnd();
                }
                else if (isChecked == 0)
                {
                    mc.To = "accounts@nextsl.com.hk";
                }

                mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

                string vendorName = vendor.Name;
                if (vendorName.ToLower().Contains("do not use"))
                {
                    vendorName = paymentAdvice.SupplierName;
                }

                if (company != string.Empty)
                    mc.Subject = "NSL Payment Advice " + company + " [" + vendorName + "]";
                else
                    mc.Subject = "NSL Payment Advice [" + vendorName + "]";
                mc.setBodyParams(vendorName);
                mc.Attachments = ConvertUtility.createArrayList(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send Payment Advice Failure [" + vendor.Name + "]", e, "");
            }
        }

        public static void sendNTPaymentAdvice(string supplierName, string filePath, int isChecked, string company, string email)
        {
            try
            {
                MessageContent mc = getContent("PaymentAdvice");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                mc.To = email;

                mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";
                if (company != string.Empty)
                    mc.Subject = "NSL Payment Advice " + company + " [" + supplierName + "]";
                else
                    mc.Subject = "NSL Payment Advice [" + supplierName + "]";
                if (email == string.Empty)
                {
                    mc.Subject = mc.Subject + " [No Email Address]";
                    mc.To = "accounts@nextsl.com.hk";
                }
                else
                    mc.Cc = "accounts@nextsl.com.hk";
                mc.setBodyParams(supplierName);
                mc.Attachments = ConvertUtility.createArrayList(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send Payment Advice Failure [" + supplierName + "]", e, "");
            }
        }

        public static void sendPurchaseDCNote(int officeId, VendorRef vendor, string dcIndicator, string AdjustmentNoteNo, string filePath, int userId)
        {
            try
            {
                MessageContent mc = getContent("PurchaseDCNote");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);

                string ccList = "DN_ToSupplier@nextsl.com.hk";      // +";" + user.EmailAddress;
                SystemParameterRef sysPara = null;
                sysPara = CommonWorker.Instance.getSystemParameterByName(GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode.ToUpper().Trim() + "_PAYMENT_ADVICE_EMAIL_CC");
                if (sysPara != null)
                    ccList += ";" + sysPara.ParameterValue.TrimEnd();
                mc.To = vendor.eAdviceAddr;
                mc.Cc = ccList;

                mc.Subject = "NSL e-" + (dcIndicator == "D" ? "Debit" : "Credit") + " Note " + AdjustmentNoteNo + " for [" + vendor.Name + "]";
                mc.setBodyParams(dcIndicator == "D" ? "Debit" : "Credit", vendor.Name);
                mc.Attachments = ConvertUtility.createArrayList(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send Purchase DC Note Failure [" + vendor.Name + "]", e, "");
            }
        }

        public static void sendAdvancePaymentInterestDCNoteReport(int officeId, VendorRef vendor, string dcIndicator, string AdjustmentNoteNo, string filePath, int userId)
        {
            try
            {
                MessageContent mc = getContent("AdvancePaymentDCNote");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);

                string ccList = "DN_ToSupplier@nextsl.com.hk";      // +";" + user.EmailAddress;
                SystemParameterRef sysPara = null;
                sysPara = CommonWorker.Instance.getSystemParameterByName(GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode.ToUpper().Trim() + "_PAYMENT_ADVICE_EMAIL_CC");
                if (sysPara != null)
                    ccList += ";" + sysPara.ParameterValue.TrimEnd();
                mc.To = vendor.eAdviceAddr;
                mc.Cc = ccList;

                mc.Subject = "NSL e-" + (dcIndicator == "D" ? "Debit" : "Credit") + " Note (Advance Payment Interest) " + AdjustmentNoteNo + " for [" + vendor.Name + "]";
                mc.setBodyParams(dcIndicator == "D" ? "Debit" : "Credit", vendor.Name);
                mc.Attachments = ConvertUtility.createArrayList(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send AdvancePayment DC Note Failure [" + vendor.Name + "]", e, "");
            }
        }

        public static void sendGenericDCNoteReportToSupplier(int officeId, string partyName, string email, string dcIndicator, string AdjustmentNoteNo, string filePath, int userId)
        {
            try
            {
                MessageContent mc = getContent("GenericDCNoteToParty");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);

                string ccList = "DN_ToSupplier@nextsl.com.hk";
                if (partyName != "NEXT MANUFACTURING (PVT) LTD-TESTING")
                {
                    SystemParameterRef sysPara = null;
                    sysPara = CommonWorker.Instance.getSystemParameterByName(GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode.ToUpper().Trim() + "_PAYMENT_ADVICE_EMAIL_CC");
                    if (sysPara != null)
                        ccList += ";" + sysPara.ParameterValue.TrimEnd();
                }
                mc.To = email;
                mc.Cc = ccList;

                mc.Subject = "NSL e-" + (dcIndicator == "D" ? "Debit" : "Credit") + " Note (Other Charge) " + AdjustmentNoteNo + " for [" + partyName + "]";
                mc.setBodyParams(dcIndicator == "D" ? "Debit" : "Credit", partyName);
                mc.Attachments = ConvertUtility.createArrayList(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send Generic DC Note To Party Failure [" + partyName + "]", e, "");
            }
        }

        public static void sendUKClaimDCNote(int officeId, VendorRef vendor, string dcIndicator, ArrayList attachments, bool isLive, int userId)
        {
            try
            {
                MessageContent mc = getContent("UKClaimDCNote");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);

                string ccList = "DN_ToSupplier@nextsl.com.hk" + ";" + user.EmailAddress;
                SystemParameterRef sysPara = null;
                sysPara = CommonWorker.Instance.getSystemParameterByName(GeneralWorker.Instance.getOfficeRefByKey(officeId).OfficeCode.ToUpper().Trim() + "_UK_CLAIM_EMAIL_CC");
                if (sysPara != null)
                    ccList += ";" + sysPara.ParameterValue.TrimEnd();
                if (isLive)
                {
                    mc.To = vendor.eAdviceAddr;
                    mc.Cc = ccList;
                    if (vendor.VendorId == 3154)
                        mc.Cc = (mc.Cc + ";DineshS@NextSL.com.lk");
                }
                else
                {
                    /*
                    mc.To = "daphne_yip@nextsl.com.hk;joffee_lee@nextsl.com.hk";
                    mc.Cc = user.EmailAddress;
                    */
                    mc.To = "michael_lau@nextsl.com.hk";
                }
                mc.Bcc = "michael_lau@nextsl.com.hk";
                mc.Subject = "NSL " + (dcIndicator == "D" ? "Debit" : "Credit") + " Note for Next Claim [" + vendor.Name + "]";
                mc.setBodyParams(vendor.Name, dcIndicator == "D" ? "Debit" : "Credit");
                mc.Attachments = attachments;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send Next Claim DC Note Failure [" + vendor.Name + "]", e, "");
            }
        }

        public static void sendNTRechargeDCNoteCover(NTRechageDCNoteMailCase caseType, NTRechargeDCNoteDef def, DebitNoteToNUKParamDef paramDef, string attention, List<int> approverIdList, ArrayList attachments)
        {
            if (caseType != null)
            {
                MessageContent mc = null;
                string strCc = getNTRechargeDCNoteMailCClist(approverIdList, caseType);
                try
                {
                    // Subject handling
                    if (caseType == NTRechageDCNoteMailCase.APSupplier) // Case 2
                    {
                        VendorRef vendorRef = VendorWorker.Instance.getVendorByKey(def.ToVendorId);
                        mc = createNTDebitNoteFromNSLEmail<VendorRef>(def, caseType, vendorRef, attachments);
                    }
                    else if (caseType == NTRechageDCNoteMailCase.NTVendor)  // Case 6
                    {
                        NTVendorDef ntVendorDef = NonTradeWorker.Instance.getNTVendorByKey(def.ToNTVendorId);
                        mc = createNTDebitNoteFromNSLEmail<NTVendorDef>(def, caseType, ntVendorDef, attachments);
                    }
                    else if (caseType == NTRechageDCNoteMailCase.Retail) // Case 3 (Type 1)
                    {
                        mc = createNTDebitNoteForAPEmail(def, caseType, paramDef.SupplierCode, attachments);
                    }
                    else  // Case 1, 4, 5
                    {
                        mc = createNTDebitNoteFromNSLEmail<Object>(def, caseType, null, attachments);
                    }


                    if (mc == null || String.IsNullOrEmpty(mc.To) || String.IsNullOrEmpty(strCc))
                    {
                        System.Diagnostics.Debug.WriteLine("Non-Trade Recharge DC Note Email NOT DEFINED");
                        throw new Exception();
                    }

                    mc.Cc = strCc;
                    mc.setBodyParams(attention, def.DCIndicatorEng);
                    Mail.send(mc);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    MailHelper.sendErrorAlert("Send NT Debit Note Failure", e, "");
                    throw new ApplicationException("Non-Trade Recharge DC Note Email NOT DEFINED");
                }
            }
        }

        /// <summary>
        /// Email Template Type 1
        /// </summary>
        /// <param name="attachments"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static MessageContent createNTDebitNoteForAPEmail(NTRechargeDCNoteDef def, NTRechageDCNoteMailCase caseType, string vendorCode, ArrayList attachments)
        {
            string mailType = "NTDebitNoteForAPEmail";
            MessageContent mc = getContent(mailType);
            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.Attachments = attachments;
            mc.Subject = caseType.getForAPSubject(def, vendorCode);
            mc.To = caseType.ToList;
            return mc;
        }
        /// <summary>
        /// Email Template Type 2
        /// </summary>
        /// <param name="attachments"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static MessageContent createNTDebitNoteFromNSLEmail<V>(NTRechargeDCNoteDef def, NTRechageDCNoteMailCase caseType, V vDef, ArrayList attachments)
        {
            string mailType = "NTDebitNoteFromNSLEmail";
            MessageContent mc = getContent(mailType);
            string vendorName = "";
            string eAdviceEmailAddr = "";
            if (vDef != null)
            {
                if (caseType == NTRechageDCNoteMailCase.NTVendor && typeof(V) == typeof(NTVendorDef))
                {
                    vendorName = (vDef as NTVendorDef).VendorName;
                    eAdviceEmailAddr = (vDef as NTVendorDef).EAdviceEmail;
                }
                else if (caseType == NTRechageDCNoteMailCase.APSupplier && typeof(V) == typeof(VendorRef))
                {
                    vendorName = (vDef as VendorRef).Name;
                    eAdviceEmailAddr = (vDef as VendorRef).eAdviceAddr;
                } // else, bypass
                if (!String.IsNullOrEmpty(eAdviceEmailAddr))
                    eAdviceEmailAddr += ";";
            }
            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.Attachments = attachments;
            mc.Subject = caseType.getFromNSLSubject(def, vendorName);
            mc.To = eAdviceEmailAddr + caseType.ToList;
            return mc;
        }

        private static string getNTRechargeDCNoteMailCClist(List<int> approverIdList, NTRechageDCNoteMailCase caseType)
        {
            string strAddress = "";
            foreach (int approverId in approverIdList)
            {
                var userA = CommonUtil.getUserByKey(approverId);
                if (userA != null)
                {
                    strAddress += (strAddress == "" ? "" : ";") + userA.EmailAddress;
                }
            }

            if (!String.IsNullOrEmpty(strAddress))
                strAddress += ";";
            strAddress += caseType.CcList;

            return strAddress;
        }

        public static void sendDuplicateUKDNRequestMail(int claimTypeId, string ukDNNo)
        {
            try
            {
                MessageContent mc = getContent("DuplicateUKDNRequestMail");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                mc.To = "michael_lau@nextsl.com.hk";
                mc.setBodyParams(UKClaimType.getType(claimTypeId).Name, ukDNNo);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send Next Claim D/N Archive Failure", e, "");
            }
        }
        /*
        public static void sendOverILSExceptionLimitEmail(UserRef userRef, int shipmentId, string currencyCode, decimal settledAmt, string invoiceNo)
        {
            MessageContent mc = getContent("OverILSExceptionLimitEmail");
            ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(shipmentId);
            ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);
            StringBuilder sbHtml = new StringBuilder();
            string strItemNo = ProductWorker.Instance.getProductByKey(contractDef.ProductId).ItemNo;
            string strOptionNo = String.Empty;
            string strNSLPrice = String.Empty;
            string strNUKPrice = String.Empty;
            string officeCode = string.Empty;

            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.Cc = "AC_ILSDiscrepancy@nextsl.com.hk";

            officeCode = OfficeId.getName(contractDef.Office.OfficeId != OfficeId.DG.Id ? contractDef.Office.OfficeId : shipmentDef.SalesForecastSpecialGroupId);

            SystemParameterRef sysPara = null;
            UserInfoDef userDef = GeneralWorker.Instance.getUserInfoByKey(userRef.UserId);
            if (userDef.Status == "A")
                mc.To = userRef.EmailAddress;
            else
            {
                sysPara = CommonWorker.Instance.getSystemParameterByName(officeCode + "_OVER_ILS_EXCEPTION_MAIL_TO");
                mc.To = (sysPara != null ? sysPara.ParameterValue.TrimEnd() : userRef.EmailAddress);
            }

            
            //sysPara = CommonWorker.Instance.getSystemParameterByName("ALL_OVER_ILS_EXCEPTION_MAIL_CC");
            //mc.Cc += (sysPara != null ? ";" + sysPara.ParameterValue.TrimEnd() : "");
            //sysPara = CommonWorker.Instance.getSystemParameterByName(officeCode + "_OVER_ILS_EXCEPTION_MAIL_CC");
            //mc.Cc += (sysPara != null ? ";" + sysPara.ParameterValue.TrimEnd() : "");
            //mc.Cc += (sysPara != null ? ";" + "AC_ILSDiscrepancy@nextsl.com.hk" : string.Empty);

            mc.Bcc = "cliff_wong@nextsl.com.hk; toby_lo@nextsl.com.hk; michael_lau@nextsl.com.hk; louis_cheng@nextsl.com.hk;";

            mc.Subject = "Large Discrepancies When Comparing Actual Receipt Amount From Next Against NSS Invoiced Amount [" + invoiceNo + "]";
            //mc.Subject = contractDef.Office.OfficeCode + "-Price Difference [" + invoiceNo + "]";

            string invoiceAmt = shipmentDef.SellCurrency.CurrencyCode + " " + shipmentDef.TotalShippedAmount.ToString("#,###.00");
            string arAmt = currencyCode + " " + settledAmt.ToString("#,###.00");

            sbHtml.Append("<TABLE BORDER=1 CELLPADDING=2 CELLSPACING=0>");
            sbHtml.Append("<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#CCCCFF;'>");
            sbHtml.Append("<TD WIDTH=150px>&nbsp;</TD>");
            sbHtml.Append("<TD WIDTH=150px align=center><b>Option No</b></TD>");
            sbHtml.Append("<TD WIDTH=150px align=center><b>NSS</b></TD>");
            sbHtml.Append("<TD WIDTH=150px align=center><b>NEXT</b></TD>");
            sbHtml.Append("</TR>");
            sbHtml.Append("<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#FFFFFF;'>");
            sbHtml.Append("<TD><b>Invoice / Receipt Amt</b></TD>");
            sbHtml.Append("<TD>&nbsp;</TD>");
            sbHtml.Append("<TD align=center>" + invoiceAmt + "</TD>");
            sbHtml.Append("<TD align=center>" + arAmt + "</TD>");
            sbHtml.Append("</TR>");
            sbHtml.Append("<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#FFFFFF;'>");
            sbHtml.Append("<TD valign='TOP'><b>Unit Price</b></TD>");

            ArrayList priceList = ILSUploadWorker.Instance.getILSUnitPriceMatrix(shipmentId);
            bool isFirst = true;
            foreach (ILSUnitPriceMatrixDef priceDef in priceList)
            {
                if (isFirst)
                {
                    strOptionNo = priceDef.NSLOptionNo + "(" + priceDef.NSLSizeDesc + ")";
                    strNSLPrice = (priceDef.NSLPrice == 0 ? "N/A" : priceDef.NSLPrice.ToString("#,##0.00"));
                    strNUKPrice = (priceDef.NUKPrice == 0 ? "N/A" : priceDef.NUKPrice.ToString("#,##0.00"));
                }
                else
                {
                    strOptionNo += "<BR/>" + priceDef.NSLOptionNo + "  (" + priceDef.NSLSizeDesc + ")";
                    strNSLPrice += "<BR/>" + (priceDef.NSLPrice == 0 ? "N/A" : priceDef.NSLPrice.ToString("#,##0.00"));
                    strNUKPrice += "<BR/>" + (priceDef.NUKPrice == 0 ? "N/A" : priceDef.NUKPrice.ToString("#,##0.00"));
                }
                isFirst = false;
            }

            sbHtml.Append("<TD align='left'>" + strOptionNo + "</TD>");
            sbHtml.Append("<TD align='center'>" + strNSLPrice + "</TD>");
            sbHtml.Append("<TD align='center'>" + strNUKPrice + "</TD>");
            sbHtml.Append("</TR>");
            sbHtml.Append("</Table>");

            mc.setBodyParams(userRef.DisplayName, strItemNo, contractDef.ContractNo, shipmentDef.DeliveryNo.ToString(), invoiceNo, invoiceAmt, arAmt, sbHtml.ToString());
            Mail.send(mc);
        }
        */

        public static void sendOverILSExceptionLimitEmail(UserRef userRef, int shipmentId, string currencyCode, decimal settledAmt, string invoiceNo, string reminder)
        {
            try
            {
                MessageContent mc = getContent("OverILSExceptionLimitEmail");
                ShipmentDef shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(shipmentId);
                ContractDef contractDef = OrderSelectWorker.Instance.getContractByKey(shipmentDef.ContractId);

                if (contractDef.Customer.CustomerId == CustomerDef.Id.ns_led.GetHashCode())
                    return;

                StringBuilder sbHtml = new StringBuilder();
                string strItemNo = ProductWorker.Instance.getProductByKey(contractDef.ProductId).ItemNo;
                string strOptionNo = String.Empty;
                string strNSLPrice = String.Empty;
                string strNUKPrice = String.Empty;
                string officeCode = string.Empty;

                mc.StyleSheet = styleStr;
                mc.From = "AC_ILSDiscrepancy@nextsl.com.hk";
                mc.Cc = "AC_ILSDiscrepancy@nextsl.com.hk";

                officeCode = OfficeId.getName(contractDef.Office.OfficeId != OfficeId.DG.Id ? contractDef.Office.OfficeId : shipmentDef.SalesForecastSpecialGroupId);

                SystemParameterRef sysPara = null;
                UserInfoDef userDef = GeneralWorker.Instance.getUserInfoByKey(userRef.UserId);
                if (userDef.Status == "A")
                    mc.To = userRef.EmailAddress;
                else
                {
                    sysPara = CommonWorker.Instance.getSystemParameterByName(officeCode + "_OVER_ILS_EXCEPTION_MAIL_TO");
                    mc.To = (sysPara != null ? sysPara.ParameterValue.TrimEnd() : userRef.EmailAddress);
                }

                mc.Bcc = "cliff_wong@nextsl.com.hk; michael_lau@nextsl.com.hk";

                mc.Subject = contractDef.Office.OfficeCode + "-Price Difference [" + invoiceNo + "]";
                if (reminder != null)
                {
                    mc.Subject += reminder;
                    ArrayList MMlist = (ArrayList)GeneralWorker.Instance.getMMListByProductTeamId(contractDef.ProductTeam.ProductCodeId);
                    if (MMlist != null)
                    {
                        foreach (UserInfoDef temp in MMlist)
                        {
                            mc.Cc += ";" + temp.EmailAddress;
                        }
                    }
                }

                string invoiceAmt = shipmentDef.SellCurrency.CurrencyCode + " " + shipmentDef.TotalShippedAmount.ToString("#,###.00");
                string arAmt = currencyCode + " " + settledAmt.ToString("#,###.00");

                sbHtml.Append("<TABLE BORDER=1 CELLPADDING=2 CELLSPACING=0>");
                sbHtml.Append("<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#CCCCFF;'>");
                sbHtml.Append("<TD WIDTH=150px>&nbsp;</TD>");
                sbHtml.Append("<TD WIDTH=150px align=center><b>Option No</b></TD>");
                sbHtml.Append("<TD WIDTH=150px align=center><b>NSS</b></TD>");
                sbHtml.Append("<TD WIDTH=150px align=center><b>NEXT</b></TD>");
                sbHtml.Append("</TR>");
                sbHtml.Append("<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#FFFFFF;'>");
                sbHtml.Append("<TD><b>Invoice / Receipt Amt</b></TD>");
                sbHtml.Append("<TD>&nbsp;</TD>");
                sbHtml.Append("<TD align=center>" + invoiceAmt + "</TD>");
                sbHtml.Append("<TD align=center>" + arAmt + "</TD>");
                sbHtml.Append("</TR>");
                sbHtml.Append("<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#FFFFFF;'>");
                sbHtml.Append("<TD valign='TOP'><b>Unit Price</b></TD>");

                ArrayList priceList = ILSUploadWorker.Instance.getILSUnitPriceMatrix(shipmentId);
                bool isFirst = true;
                foreach (ILSUnitPriceMatrixDef priceDef in priceList)
                {
                    if (isFirst)
                    {
                        strOptionNo = priceDef.NSLOptionNo + "(" + priceDef.NSLSizeDesc + ")";
                        strNSLPrice = (priceDef.NSLPrice == 0 ? "N/A" : priceDef.NSLPrice.ToString("#,##0.00"));
                        strNUKPrice = (priceDef.NUKPrice == 0 ? "N/A" : priceDef.NUKPrice.ToString("#,##0.00"));
                    }
                    else
                    {
                        strOptionNo += "<BR/>" + priceDef.NSLOptionNo + "  (" + priceDef.NSLSizeDesc + ")";
                        strNSLPrice += "<BR/>" + (priceDef.NSLPrice == 0 ? "N/A" : priceDef.NSLPrice.ToString("#,##0.00"));
                        strNUKPrice += "<BR/>" + (priceDef.NUKPrice == 0 ? "N/A" : priceDef.NUKPrice.ToString("#,##0.00"));
                    }
                    isFirst = false;
                }

                sbHtml.Append("<TD align='center'>" + strOptionNo + "</TD>");
                sbHtml.Append("<TD align='center'>" + strNSLPrice + "</TD>");
                sbHtml.Append("<TD align='center'>" + strNUKPrice + "</TD>");
                sbHtml.Append("</TR>");
                sbHtml.Append("</Table>");

                mc.setBodyParams(userRef.DisplayName, strItemNo, contractDef.ContractNo, shipmentDef.DeliveryNo.ToString(), invoiceNo, invoiceAmt, arAmt, sbHtml.ToString());
                Mail.send(mc);
                /*
                if (reminder == null)
                    ShippingWorker.Instance.updateActionHistory(new ActionHistoryDef(shipmentId, 0, ActionHistoryType.PRICE_DIFF_ALERT, "Send Price Difference Alert", 1, 99999));
                */
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        public static void sendReplacementInvoiceEmail(int userId, string invoiceNo)
        {
            MessageContent mc = getContent("ReplacementInvoiceEmail");
            ILSInvoiceDef ilsInvoiceDef = ILSUploadWorker.Instance.getILSInvoiceByInvoiceNo(invoiceNo);
            if (ilsInvoiceDef == null)
                ilsInvoiceDef = ILSUploadWorker.Instance.getILSReplacementInvoice(invoiceNo);

            string contractNo = string.Empty;
            ILSOrderRefDef ilsOrderRefDef = null;
            InvoiceDef invoiceDef = null;
            ShipmentDef shipmentDef = null;

            if (ilsInvoiceDef != null)
            {
                ilsOrderRefDef = ILSUploadWorker.Instance.getILSOrderRefByKey(ilsInvoiceDef.OrderRefId);
                if (ilsOrderRefDef.ShipmentId <= 0)
                {
                    throw new ApplicationException("Unassigned Shipment for Invoice " + invoiceNo + ",  contractNo : " + ilsOrderRefDef.ContractNo);
                }
                invoiceDef = ShippingWorker.Instance.getInvoiceByKey(ilsOrderRefDef.ShipmentId);
                shipmentDef = OrderSelectWorker.Instance.getShipmentByKey(invoiceDef.ShipmentId);
                contractNo = ilsOrderRefDef.ContractNo + "-" + shipmentDef.DeliveryNo.ToString();
            }

            if (invoiceDef != null)
            {
                UserRef userRef = GeneralWorker.Instance.getUserByKey(userId);
                mc.StyleSheet = styleStr;
                mc.From = sender;
                mc.To = userRef.EmailAddress;
                mc.Bcc = "cliff_wong@nextsl.com.hk; michael_lau@nextsl.com.hk; louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";
                mc.Subject = "Replacement Invoice No. Of " + invoiceNo + " (Contract no.:" + contractNo + ")";
                mc.setBodyParams(userRef.DisplayName, invoiceNo, "C" + invoiceNo, contractNo, invoiceDef.InvoiceNo == String.Empty ? "[Not Uploaded Yet On ISAM]" : invoiceDef.InvoiceNo);
                Mail.send(mc);
            }
        }

        public static void sendUKClaimDCNoteMissingSupplierEmail(int officeId, string vendorName, string dcIndicator, ArrayList attachments, bool isLive, int userId)
        {
            try
            {
                string ccList = "DN_ToSupplier@nextsl.com.hk;carmen_cheung@nextsl.com.hk";
                /*
                if (officeId == OfficeId.HK.Id || officeId == OfficeId.DG.Id)
                    ccList = ccList + ";" + "joeywong@NextSL.com.hk";
                else if (officeId == OfficeId.CA.Id || officeId == OfficeId.IND.Id || officeId == OfficeId.ND.Id || officeId == OfficeId.PK.Id || officeId == OfficeId.TR.Id || officeId == OfficeId.EG.Id || officeId == OfficeId.BD.Id)
                    ccList = ccList + ";" + "Jenny_Chow@nextsl.com.hk";
                else if (officeId == OfficeId.SL.Id || officeId == OfficeId.SH.Id || officeId == OfficeId.TH.Id || officeId == OfficeId.VN.Id)
                    ccList = ccList + ";" + "annieyick@NextSL.com.hk";
                */

                MessageContent mc = getContent("UKClaimDCNoteMissingSupplierEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                if (isLive)
                {
                    ccList = (ccList + ";" + user.EmailAddress);
                    mc.Cc = ccList;
                }
                else
                    mc.To = "michael_lau@nextsl.com.hk";
                mc.Subject = "NSL " + (dcIndicator == "D" ? "Debit" : "Credit") + " Note for Next Claim [" + vendorName + "] (No Email Address)";
                mc.setBodyParams(vendorName, dcIndicator == "D" ? "Debit" : "Credit");
                mc.Attachments = attachments;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendUKClaimDCNoteNoAttachmentEmail(int officeId, string vendorName, string dcIndicator, ArrayList attachments, bool isLive, int userId)
        {
            try
            {
                string ccList = "DN_ToSupplier@nextsl.com.hk;carmen_cheung@nextsl.com.hk";
                /*
                if (officeId == OfficeId.HK.Id || officeId == OfficeId.DG.Id)
                    ccList = ccList + ";" + "joeywong@NextSL.com.hk";
                else if (officeId == OfficeId.CA.Id || officeId == OfficeId.IND.Id || officeId == OfficeId.ND.Id || officeId == OfficeId.PK.Id || officeId == OfficeId.TR.Id || officeId == OfficeId.EG.Id || officeId == OfficeId.BD.Id)
                    ccList = ccList + ";" + "Jenny_Chow@nextsl.com.hk";
                else if (officeId == OfficeId.SL.Id || officeId == OfficeId.SH.Id || officeId == OfficeId.TH.Id || officeId == OfficeId.VN.Id)
                    ccList = ccList + ";" + "annieyick@NextSL.com.hk";
                */

                MessageContent mc = getContent("UKClaimDCNoteNoAttachmentEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                if (isLive)
                {
                    ccList = (ccList + ";" + user.EmailAddress);
                    mc.Cc = ccList;
                }
                else
                    mc.To = "michael_lau@nextsl.com.hk";
                mc.Subject = "NSL " + (dcIndicator == "D" ? "Debit" : "Credit") + " Note for Next Claim [" + vendorName + "] (No Email Address)";
                mc.setBodyParams(vendorName, dcIndicator == "D" ? "Debit" : "Credit");
                mc.Attachments = attachments;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendPurchaseDCNoteMissingSupplierEmail(int officeId, VendorRef vendor, string dcIndicator, string AdjustmentNoteNo, string filePath, int userId)
        {
            try
            {
                MessageContent mc = getContent("PurchaseDCNoteMissingSupplierEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;

                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                string ccList = "DN_ToSupplier@nextsl.com.hk"; // "; " + user.EmailAddress;

                string vendorName = string.Empty;
                if (vendor != null)
                {
                    SystemParameterRef sysPara = null;
                    if (vendor.OfficeId > 0 && GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId) != null)
                        sysPara = CommonWorker.Instance.getSystemParameterByName(GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId).OfficeCode.ToUpper().Trim() + "_PAYMENT_ADVICE_EMAIL_CC");

                    if (sysPara != null)
                        ccList += (ccList == string.Empty ? string.Empty : ";") + sysPara.ParameterValue.TrimEnd();
                    vendorName = vendor.Name;
                }
                if (string.IsNullOrEmpty(vendorName))
                    vendorName = "Unknown Vendor";

                mc.Subject = "NSL e-" + (dcIndicator == "D" ? "Debit" : "Credit") + " Note " + AdjustmentNoteNo + " for [" + vendorName + "] (No Email Address)";
                mc.setBodyParams((dcIndicator == "D" ? "Debit" : "Credit"), vendorName);
                if (!string.IsNullOrEmpty(filePath))
                    mc.Attachments = ConvertUtility.createArrayList(filePath);
                mc.Cc += ccList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendAdvancePaymentInterestDCNoteMissingSupplierEmail(int officeId, VendorRef vendor, string dcIndicator, string AdjustmentNoteNo, string filePath, int userId)
        {
            try
            {
                MessageContent mc = getContent("AdvancePaymentInterestDCNoteMissingSupplierEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;

                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                string ccList = "DN_ToSupplier@nextsl.com.hk"; // "; " + user.EmailAddress;

                string vendorName = string.Empty;
                if (vendor != null)
                {
                    SystemParameterRef sysPara = null;
                    if (vendor.OfficeId > 0 && GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId) != null)
                        sysPara = CommonWorker.Instance.getSystemParameterByName(GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId).OfficeCode.ToUpper().Trim() + "_PAYMENT_ADVICE_EMAIL_CC");

                    if (sysPara != null)
                        ccList += (ccList == string.Empty ? string.Empty : ";") + sysPara.ParameterValue.TrimEnd();
                    vendorName = vendor.Name;
                }
                if (string.IsNullOrEmpty(vendorName))
                    vendorName = "Unknown Vendor";

                mc.Subject = "NSL e-" + (dcIndicator == "D" ? "Debit" : "Credit") + " Note (Advance Payment Interest) " + AdjustmentNoteNo + " for [" + vendorName + "] (No Email Address)";
                mc.setBodyParams((dcIndicator == "D" ? "Debit" : "Credit"), vendorName);
                if (!string.IsNullOrEmpty(filePath))
                    mc.Attachments = ConvertUtility.createArrayList(filePath);
                mc.Cc += ccList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendGenericDCNoteMissingSupplierEmail(int officeId, VendorRef vendor, string dcIndicator, string AdjustmentNoteNo, string filePath, int userId)
        {
            try
            {
                MessageContent mc = getContent("GenericDCNoteMissingSupplierEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;

                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                string ccList = "DN_ToSupplier@nextsl.com.hk"; // "; " + user.EmailAddress;

                string vendorName = string.Empty;
                if (vendor != null)
                {
                    SystemParameterRef sysPara = null;
                    if (vendor.OfficeId > 0 && GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId) != null)
                        sysPara = CommonWorker.Instance.getSystemParameterByName(GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId).OfficeCode.ToUpper().Trim() + "_PAYMENT_ADVICE_EMAIL_CC");

                    if (sysPara != null)
                        ccList += (ccList == string.Empty ? string.Empty : ";") + sysPara.ParameterValue.TrimEnd();
                    vendorName = vendor.Name;
                }
                if (string.IsNullOrEmpty(vendorName))
                    vendorName = "Unknown Vendor";

                mc.Subject = "NSL e-" + (dcIndicator == "D" ? "Debit" : "Credit") + " Note (Other Charge) " + AdjustmentNoteNo + " for [" + vendorName + "] (No Email Address)";
                mc.setBodyParams((dcIndicator == "D" ? "Debit" : "Credit"), vendorName);
                if (!string.IsNullOrEmpty(filePath))
                    mc.Attachments = ConvertUtility.createArrayList(filePath);
                mc.Cc += ccList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendPaymentAdviceMissingSupplierEmail(string vendorName, string filePath, string ccList, string company)
        {
            try
            {
                MessageContent mc = getContent("PaymentAdviceMissingSupplierEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                if (!string.IsNullOrEmpty(ccList))
                    mc.Cc += (mc.Cc == string.Empty ? string.Empty : ";") + ccList;
                if (string.IsNullOrEmpty(vendorName))
                    vendorName = "Unknown Vendor";
                if (company != string.Empty)
                    mc.Subject = "NSL Payment Advice " + company + " [" + vendorName + "] (No Email Address)";
                else
                    mc.Subject = "NSL Payment Advice [" + vendorName + "] (No Email Address)";
                mc.setBodyParams(vendorName);
                mc.Attachments = ConvertUtility.createArrayList(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendPaymentAdviceMissingSupplierEmail(string vendorName, string filePath, string company)
        {
            sendPaymentAdviceMissingSupplierEmail(vendorName, filePath, string.Empty, company);
        }

        public static void sendPaymentAdviceMissingSupplierEmail(VendorRef vendor, string filePath, string company)
        {
            try
            {
                if (vendor != null)
                {
                    string ccList = string.Empty;
                    SystemParameterRef sysPara = null;
                    if (vendor.OfficeId > 0 && GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId) != null)
                        sysPara = CommonWorker.Instance.getSystemParameterByName(GeneralWorker.Instance.getOfficeRefByKey(vendor.OfficeId).OfficeCode.ToUpper().Trim() + "_PAYMENT_ADVICE_EMAIL_CC");

                    if (sysPara != null)
                        ccList += (ccList == string.Empty ? string.Empty : ";") + sysPara.ParameterValue.TrimEnd();
                    sendPaymentAdviceMissingSupplierEmail(vendor.Name, filePath, ccList, company);
                }
                else
                    sendPaymentAdviceMissingSupplierEmail("Unknown Vendor", filePath, string.Empty, company);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }


        public static void sendUploadSupplierInvoiceNoMail(ArrayList attachmentList, int userId)
        {
            sendUploadSupplierInvoiceNoMail(attachmentList, userId, null, null);
        }

        public static void sendUploadSupplierInvoiceNoMail(ArrayList attachmentList, int userId, string attachedTable)
        {
            sendUploadSupplierInvoiceNoMail(attachmentList, userId, attachedTable, null);
        }

        public static void sendUploadSupplierInvoiceNoMail(ArrayList attachmentList, int userId, string attachedTable, ArrayList officeCodeList)
        {
            try
            {
                string additionalMessage = string.Empty;
                string[] section = attachmentList[0].ToString().Split(char.Parse("\\"));
                string fileName = section[section.Length - 1].ToString();
                SystemParameterRef sysPara = null;
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("UploadSupplierInvoiceNoEmail");
                mc.From = sender;
                mc.To = user.EmailAddress;

                if (officeCodeList != null)
                {
                    foreach (string officeCode in officeCodeList)
                    {
                        if (officeCode != "")
                            sysPara = CommonWorker.Instance.getSystemParameterByName(officeCode.ToUpper().Trim() + "_UPLOAD_SUPPLIER_INVOICE_NO_EMAIL_TO");
                        if (sysPara != null)
                            mc.To += ";" + sysPara.ParameterValue.TrimEnd();
                    }
                }
                mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

                if (string.IsNullOrEmpty(attachedTable))
                    additionalMessage = " successfully.<br><br>";
                else
                {
                    additionalMessage = ".<br>"
                    + "Please note below supplier invoice no. cannot be uploaded properly."
                    + "Please contact data source provider.";
                }

                mc.Subject = "Supplier invoice no. has been upload into ISAM";
                mc.setBodyParams(user.DisplayName, fileName, attachedTable, additionalMessage);
                mc.Attachments = attachmentList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendDailyBookingUploadMail(string subject, string originalMessage, string mailTo, string mailCC, ArrayList attachmentList)
        {
            try
            {
                string additionalMessage = string.Empty;
                string[] section = attachmentList[0].ToString().Split(char.Parse("\\"));
                string fileName = section[section.Length - 1].ToString();
                //SystemParameterRef sysPara = null;
                MessageContent mc = getContent("ShippingDailyBookingUploadEmail");
                mc.From = sender;
                mc.To = mailTo;
                mc.Cc = mailCC;
                mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

                mc.Subject = subject;
                mc.setBodyParams(originalMessage);
                mc.Attachments = attachmentList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendUploadSupplierInvoiceNoFailMail(ArrayList attachmentList, int userId, string uploadFormatTable)
        {
            try
            {

                string[] section = attachmentList[0].ToString().Split(char.Parse("\\"));
                string fileName = section[section.Length - 1].ToString();
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("UploadSupplierInvoiceNoFailEmail");
                mc.From = sender;
                mc.To = user.EmailAddress;
                mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

                mc.Subject = "Supplier invoice no. cannot be uploaded into ISAM";
                mc.setBodyParams(user.DisplayName, fileName, uploadFormatTable);
                mc.Attachments = attachmentList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendDMSRevisedDocUploadFailureEmail(string fileName, string path, string officeCode, DateTime d)
        {
            MessageContent mc = getContent("DMSRevisedDocUploadFailureEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk; helpdesk_support@nextsl.com.hk; toby_lo@nextsl.com.hk";
            mc.Subject = "Document Replacement Upload Failure [" + fileName + "]";
            mc.setBodyParams(path, officeCode, d.ToString("dd/MM/yyyy HH:mm:ss"));
            mc.Attachments.Add(path);
            Mail.send(mc);
        }

        public static void sendDMSDocUploadInvalidFileFailureEmail(int officeId, string fileName, string path, DateTime d)
        {
            MessageContent mc = getContent("DMSDocUploadInvalidFileFailureEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = OfficeId.getEmailReceiver(officeId);
            //mc.To = "michael_lau@nextsl.com.hk";
            mc.Cc = "michael_lau@nextsl.com.hk; helpdesk_support@nextsl.com.hk;";
            mc.Subject = "Document Upload Failure [" + fileName + "]";
            mc.setBodyParams(path, d.ToString("dd/MM/yyyy HH:mm:ss"));
            mc.Attachments.Add(path);
            Mail.send(mc);
        }

        public static void sendDMSDocUploadPdfRepairFailureEmail(int officeId, string fileName, string path, DateTime d)
        {
            MessageContent mc = getContent("DMSDocUploadPdfRepairFailureEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk";
            mc.Subject = "Document Upload Pdf Repair Failure [" + fileName + "]";
            mc.setBodyParams(path, d.ToString("dd/MM/yyyy HH:mm:ss"));
            mc.Attachments.Add(path);
            Mail.send(mc);
        }

        public static void sendDMSDocUploadOfficeDiscrepancyFailureEmail(int officeId, string fileName, string path, DateTime d, int systemOfficeId)
        {
            MessageContent mc = getContent("DMSDocUploadOfficeDiscrepancyFailureEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = OfficeId.getEmailReceiver(officeId);
            //mc.To = "michael_lau@nextsl.com.hk";
            mc.Cc = "michael_lau@nextsl.com.hk; helpdesk_support@nextsl.com.hk; toby_lo@nextsl.com.hk";
            mc.Subject = "Document Upload Failure [" + fileName + "]";
            mc.setBodyParams(path, d.ToString("dd/MM/yyyy HH:mm:ss"), OfficeId.getName(officeId), OfficeId.getName(systemOfficeId));
            mc.Attachments.Add(path);
            Mail.send(mc);
        }

        public static void sendDMSDocUploadShipmentNotFoundFailureEmail(int officeId, string fileName, string path, DateTime d)
        {
            MessageContent mc = getContent("DMSDocUploadShipmentNotFoundFailureEmail");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.To = OfficeId.getEmailReceiver(officeId);
            //mc.To = "michael_lau@nextsl.com.hk";
            mc.Cc = "michael_lau@nextsl.com.hk; helpdesk_support@nextsl.com.hk; toby_lo@nextsl.com.hk";
            mc.Subject = "Document Upload Failure [" + fileName + "]";
            mc.setBodyParams(path, d.ToString("dd/MM/yyyy HH:mm:ss"));
            mc.Attachments.Add(path);
            Mail.send(mc);
        }

        public static void sendUKClaimDNReviewNoticeToQCAdminEmail(string htmlUKClaimDNTable, string guId)
        {
            MessageContent mc = getContent("UKClaimDNReviewNoticeToQCAdminEmail");
            mc.IsHtmlFormat = true;
            mc.Subject = "Review Next Claim Debit Note";
            mc.From = sender;
            mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

            SystemParameterRef sysParam;
            sysParam = CommonWorker.Instance.getSystemParameterByName("UKCLAIM_QCADMIN_EMAIL");
            mc.To = (sysParam != null ? sysParam.ParameterValue.TrimEnd() : "michael_lau@nextsl.com.hk;cliff_wong@nextsl.com.hk");
            sysParam = CommonWorker.Instance.getSystemParameterByName("UKCLAIM_QCADMIN_NAME");
            string attension = (sysParam != null ? sysParam.ParameterValue.TrimEnd() : "Michael");
            string url = "http:////" + Environment.MachineName + "//isam//claim//ukclaimReview.aspx?GUID=" + guId;
            mc.setBodyParams(attension, htmlUKClaimDNTable, url, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            Mail.send(mc);
        }

        public static void sendGeneralNoticeToAdmin(string subject, string message, string moduleName)
        {
            sendGeneralNoticeToAdmin(subject, message, moduleName, null);
        }

        public static void sendGeneralNoticeToAdmin(string subject, string message, string moduleName, ArrayList attachmentList)
        {
            sendGeneralNotice("michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk", subject, message, moduleName, attachmentList);
        }

        public static void sendGeneralNotice(string eMailAddress, string subject, string message, string moduleName, ArrayList attachmentList)
        {
            try
            {
                MessageContent mc = getContent("SendGeneralNotice");
                mc.From = sender;
                mc.To = eMailAddress;
                //mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

                mc.Subject = subject;
                mc.setBodyParams("All", message, moduleName);
                if (attachmentList != null)
                    mc.Attachments = attachmentList;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }

        }

        public static void sendUKClaimDNArchive(ArrayList attachments, string contents, int count, int userId)
        {
            try
            {
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("UKClaimDNArchive");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                mc.To = user.EmailAddress;
                mc.Bcc = "michael_lau@nextsl.com.hk";
                mc.setBodyParams(user.DisplayName, count, contents);
                mc.Attachments = attachments;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send Next Claim D/N Archive Failure", e, "");
            }
        }

        public static void sendSupplierDocMailArchive(string contractShipment, string docType, string filePath, int userId)
        {
            string recipients = GeneralWorker.Instance.getUserByKey(userId).EmailAddress;

            MessageContent mc = getContent("EmptyMail");
            mc.Subject = docType + " of Shipment " + contractShipment;
            mc.From = "NSS Admin<nssadmin@nextsl.com.hk>";
            mc.Bcc = "michael_lau@nextsl.com.hk";
            mc.To = recipients;
            mc.Attachments.Add(filePath);
            Mail.send(mc);
        }

        public static void sendNextClaimMailArchive(UKClaimDef def, string docType, string filePath, int userId)
        {
            string recipients = GeneralWorker.Instance.getUserByKey(userId).EmailAddress;

            MessageContent mc = getContent("EmptyMail");
            string claimType = def.Type.Name;
            if (def.Type == UKClaimType.MFRN)
                claimType = def.Type.Name + " [" + def.ClaimMonth + "]";
            mc.Subject = docType + " of Next Claim - " + def.UKDebitNoteNo + " (" + claimType + ")";
            mc.From = "NSS Admin<nssadmin@nextsl.com.hk>";
            mc.Bcc = "michael_lau@nextsl.com.hk";
            mc.To = recipients;
            mc.Attachments.Add(filePath);
            Mail.send(mc);
        }

        public static void sendUKDiscountClaimMailArchive(UKDiscountClaimDef def, string docType, string filePath, int userId)
        {
            string recipients = GeneralWorker.Instance.getUserByKey(userId).EmailAddress;

            MessageContent mc = getContent("EmptyMail");
            mc.Subject = docType + " of UK Discount Claim - " + def.UKDebitNoteNo;
            mc.From = "NSS Admin<nssadmin@nextsl.com.hk>";
            mc.Bcc = "michael_lau@nextsl.com.hk";
            mc.To = recipients;
            mc.Attachments.Add(filePath);
            Mail.send(mc);
        }

        public static void sendInstalmentDocMailArchive(AdvancePaymentDef def, string docType, string filePath, int userId)
        {
            string recipients = GeneralWorker.Instance.getUserByKey(userId).EmailAddress;

            MessageContent mc = getContent("EmptyMail");
            mc.Subject = "Supporting Document of Advance Payment - [" + def.PaymentNo + "]";
            mc.From = "NSS Admin<nssadmin@nextsl.com.hk>";
            mc.Bcc = "michael_lau@nextsl.com.hk;louis_cheng@nextsa.com.hk;";
            mc.To = recipients;
            mc.Attachments.Add(filePath);
            Mail.send(mc);
        }

        public static void sendNewNTVendorNotification(NTVendorDef def, int createdBy)
        {
            MessageContent mc = getContent("NewNTVendorNotification");
            mc.IsHtmlFormat = true;
            mc.Subject = "Non-Trade Expense: New vendor profile has been submitted";
            mc.From = sender;
            mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

            UserRef userDef = GeneralWorker.Instance.getUserByKey(createdBy);
            string userText = userDef.DisplayName + " [" + userDef.Department.Description + "]";

            mc.To = CommonWorker.Instance.getSystemParameterByName(def.Office.OfficeCode + "_NEW_NT_VENDOR_EMAIL").ParameterValue;
            mc.Cc = userDef.EmailAddress + ";" + (def.CreatedBy != null ? def.CreatedBy.EmailAddress : string.Empty);
            /*
            if (userDef.Department.Office.OfficeId == OfficeId.HK.Id || userDef.Department.Office.OfficeId == OfficeId.CA.Id || userDef.Department.Office.OfficeId == OfficeId.TH.Id || userDef.Department.Office.OfficeId == OfficeId.VN.Id)
                mc.To = "annieyick@nextsl.com.hk;joeywong@nextsl.com.hk;cheime_wong@nextsl.com.hk;jenny_chow@nextsl.com.hk";
            else if (userDef.Department.Office.OfficeId == OfficeId.SH.Id)
                mc.To = "ivan_chong@nextsl.com.hk;winnie_lui@nextsl.com.hk;";
            else if (userDef.Department.Office.OfficeId == OfficeId.BD.Id || userDef.Department.Office.OfficeId == OfficeId.IND.Id || userDef.Department.Office.OfficeId == OfficeId.ND.Id)
                mc.To = "shirley_yip@nextsl.com.hk;carmen_cheung@nextsl.com.hk;";
            else if (userDef.Department.Office.OfficeId == OfficeId.SL.Id || userDef.Department.Office.OfficeId == OfficeId.PK.Id)
                mc.To = "ivan_chong@nextsl.com.hk;carmen_cheung@nextsl.com.hk;";
            else if (userDef.Department.Office.OfficeId == OfficeId.TR.Id || userDef.Department.Office.OfficeId == OfficeId.EG.Id)
                mc.To = "flora_fungty@nextsl.com.hk;vanessa_ng@nextsl.com.hk";
            else if (userDef.Department.Office.OfficeId == OfficeId.UK.Id)
                mc.To = "teresa_wong@nextsl.com.hk;ruby_lai@nextsl.com.hk";
            else
                mc.To = "louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";
            */

            mc.setBodyParams(userText, def.VendorName, def.Address, def.Office.OfficeCode, (def.ExpenseType == null ? "" : def.ExpenseType.ExpenseType));
            Mail.send(mc);

        }

        public static void sendNTInvoicePendingForApprovalNotification(ArrayList ntInvoiceList, UserRef approvalUser)
        {
            MessageContent mc = getContent("NTInvoicePendingForApprovalNotification");
            mc.IsHtmlFormat = true;
            mc.Subject = "[Reminder] Some non-trade expense submission is waiting for your approval";
            mc.From = sender;
            mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

            mc.To = approvalUser == null ? "michael_lau@nextsl.com.hk" : approvalUser.EmailAddress;

            string invoiceDetailText = "<TABLE BORDER=1 CELLPADDING=2 CELLSPACING=0>";
            invoiceDetailText += "<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#CCCCFF;font-family:Calibri;'><td>NS Ref No.</td><td>Invoice No. / Customer No.</td><td>Vendor</td><td>Invoice Date</td><td>Currency</td><td>Amount</td><td>Submitted by</td></tr>";

            foreach (NTInvoiceDef invoiceDef in ntInvoiceList)
            {
                invoiceDetailText += string.Format("<tr style='font-size:12;font-family:Calibri;'><td style='width:120px;'>{0}</td><td style='width:120px;'>{1}</td><td style='width:200px;'>{2}</td><td style='width:100px;'>{3}</td><td style='width:80px;'>{4}</td><td style='width:80px;'>{5}</td><td>{6}</td></tr>",
                    invoiceDef.NSLInvoiceNo,
                    (invoiceDef.InvoiceNo == string.Empty ? invoiceDef.CustomerNo : (invoiceDef.CustomerNo == string.Empty ? invoiceDef.InvoiceNo : invoiceDef.InvoiceNo + " / " + invoiceDef.CustomerNo)),
                    invoiceDef.NTVendor.VendorName, invoiceDef.InvoiceDate.ToString("dd/MM/yyyy"), invoiceDef.Currency.CurrencyCode, invoiceDef.Amount.ToString(), invoiceDef.ModifiedBy.DisplayName);
            }

            invoiceDetailText += "</table>";

            mc.setBodyParams(approvalUser.DisplayName, invoiceDetailText);
            Mail.send(mc);
        }

        public static void sendNTInvoiceRejectNotification(NTInvoiceDef invoiceDef, int rejectedBy)
        {
            MessageContent mc = getContent("NTInvoiceRejectNotification");
            mc.IsHtmlFormat = true;
            mc.Subject = "Non-Trade Expense: Your non-trade expense invoice has been rejected";
            mc.From = sender;
            mc.To = invoiceDef.CreatedBy.EmailAddress;
            mc.Cc = GeneralWorker.Instance.getUserByKey(rejectedBy).EmailAddress;
            mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

            mc.setBodyParams(invoiceDef.CreatedBy.DisplayName, invoiceDef.ModifiedBy.DisplayName, invoiceDef.NSLInvoiceNo, invoiceDef.NTVendor.VendorName,
                invoiceDef.InvoiceNo, invoiceDef.CustomerNo, invoiceDef.InvoiceDate.ToString("dd/MM/yyyy"), invoiceDef.RejectReason);
            Mail.send(mc);

        }

        public static void sendNewNTVendorConfirmation(NTVendorDef vendorDef, int userId)
        {
            MessageContent mc = getContent("NewNTVendorConfirmation");
            mc.IsHtmlFormat = true;
            mc.Subject = "Non-Trade Expense: New Vendor " + vendorDef.VendorName + " is ready to use";
            mc.From = sender;
            mc.To = vendorDef.CreatedBy.EmailAddress;

            UserRef modifiedBy = GeneralWorker.Instance.getUserByKey(userId);
            mc.Cc = modifiedBy.EmailAddress;
            mc.Bcc = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

            mc.setBodyParams(vendorDef.CreatedBy.DisplayName, vendorDef.VendorName);
            Mail.send(mc);
        }

        public static void sendNTVendorApprovalNotification(NTVendorDef vendorDef, int userId)
        {
            MessageContent mc = getContent("NTVendorApprovalNotification");
            mc.IsHtmlFormat = true;
            mc.Subject = "Non-Trade Expense: Vendor [" + vendorDef.VendorName + "] has been approved";
            mc.From = sender;

            UserRef createdUser = vendorDef.CreatedBy;
            if (createdUser == null)
                createdUser = GeneralWorker.Instance.getUserByKey(userId);

            mc.To = createdUser.EmailAddress;

            UserRef modifiedBy = GeneralWorker.Instance.getUserByKey(userId);
            mc.Cc = modifiedBy.EmailAddress + ";" + CommonWorker.Instance.getSystemParameterByName(vendorDef.Office.OfficeCode + "_NEW_NT_VENDOR_EMAIL").ParameterValue;

            mc.setBodyParams(createdUser.DisplayName, vendorDef.VendorName);
            Mail.send(mc);
        }

        public static void sendNTVendorRejectNotification(NTVendorDef vendorDef, int userId)
        {
            MessageContent mc = getContent("NTVendorRejectNotification");
            mc.IsHtmlFormat = true;
            mc.Subject = "Non-Trade Expense: Vendor [" + vendorDef.VendorName + "] has been rejected";
            mc.From = sender;

            UserRef createdUser = vendorDef.CreatedBy;
            if (createdUser == null)
                createdUser = GeneralWorker.Instance.getUserByKey(userId);

            mc.To = createdUser.EmailAddress;

            UserRef modifiedBy = GeneralWorker.Instance.getUserByKey(userId);
            mc.Cc = modifiedBy.EmailAddress;


            mc.setBodyParams(createdUser.DisplayName, modifiedBy.DisplayName, vendorDef.VendorName, vendorDef.Remark);
            Mail.send(mc);
        }

        public static void sendNTVendorCancelledNotification(NTVendorDef vendorDef, int userId)
        {
            MessageContent mc = getContent("NTVendorCancelledNotification");
            mc.IsHtmlFormat = true;
            mc.Subject = "Non-Trade Expense: Vendor [" + vendorDef.VendorName + "] has been cancelled";
            mc.From = sender;

            UserRef createdUser = vendorDef.CreatedBy;
            if (createdUser == null)
                createdUser = GeneralWorker.Instance.getUserByKey(userId);

            mc.To = createdUser.EmailAddress;

            UserRef modifiedBy = GeneralWorker.Instance.getUserByKey(userId);
            mc.Cc = modifiedBy.EmailAddress;


            mc.setBodyParams(createdUser.DisplayName, modifiedBy.DisplayName, vendorDef.VendorName);
            Mail.send(mc);
        }

        public static void sendNTVendorAmendmentRequest(NTVendorDef def, string description, int userId)
        {
            MessageContent mc = getContent("NTVendorAmendmentRequest");
            mc.IsHtmlFormat = true;
            mc.Subject = "Non-Trade Vendor Profile Amendment Request";
            mc.From = sender;

            mc.To = CommonWorker.Instance.getSystemParameterByName(def.Office.OfficeCode + "_NEW_NT_VENDOR_EMAIL").ParameterValue;

            UserRef user = GeneralWorker.Instance.getUserByKey(userId);
            mc.Cc = user.EmailAddress;

            mc.setBodyParams(user.DisplayName, def.Office.OfficeCode, def.VendorName, description);
            Mail.send(mc);

        }

        public static void sendUKClaimDiscrepancyListEmail(List<UKClaimDef> list)
        {
            QAIS.ClaimRequestService svc = new QAIS.ClaimRequestService();
            string html = "<TABLE BORDER=1><TR><TD CLASS='gridHeader'>&nbsp;&nbsp;Office&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Next D/N No&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Claim Type (ISAM)&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Claim Type (QAIS)&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Item (ISAM)&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Item (QAIS)&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Vendor (ISAM)&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Vendor (QAIS)&nbsp;&nbsp;</TD></TR>";
            List<int> officeIdList = new List<int>();

            foreach (UKClaimDef def in list)
            {
                QAIS.ClaimRequestDef requestDef = svc.GetClaimRequestByKey(def.ClaimRequestId);
                if (!officeIdList.Contains(def.OfficeId))
                    officeIdList.Add(def.OfficeId);

                html += "<TR><TD CLASS='dataCellStr'>" + OfficeId.getName(def.OfficeId) + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + (def.Type == UKClaimType.MFRN ? def.UKDebitNoteNo + "(" + def.ClaimMonth + ")" : def.UKDebitNoteNo) + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + def.Type.Name + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + requestDef.ClaimType.ToString() + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + def.ItemNo + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + requestDef.ItemNo + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + def.Vendor.Name + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + requestDef.Vendor.Name + "</TD></TR>";
            }
            html += "</TABLE>";

            MessageContent mc = getContent("UKClaimDiscrepancyListEmail");
            mc.IsHtmlFormat = true;
            mc.StyleSheet = styleStr;
            mc.From = sender;

            mc.To = "carmen_cheung@nextsl.com.hk;";
            foreach (int i in officeIdList)
            {
                mc.To = mc.To + (";" + OfficeId.getNextClaimRecipientList(i));
            }

            mc.Bcc = "michael_lau@nextsl.com.hk";
            mc.Subject = "Next Claim Discrepancies as of " + DateTime.Now.ToString("dd/MM/yyyy");
            mc.setBodyParams(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), html);

            Mail.send(mc);
        }

        public static void sendUKClaimMFRNUploadAlertEmail(List<UKClaimDef> list)
        {
            string html = "<TABLE BORDER=1><TR><TD CLASS='gridHeader'>&nbsp;&nbsp;Office&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Next D/N No&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Claim Type&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Office&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Payment Office&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Item&nbsp;&nbsp;</TD><TD CLASS='gridHeader'>&nbsp;&nbsp;Vendor&nbsp;&nbsp;</TD></TR>";
            List<int> officeIdList = new List<int>();

            foreach (UKClaimDef def in list)
            {
                if (!officeIdList.Contains(def.OfficeId))
                    officeIdList.Add(def.OfficeId);

                html += "<TR><TD CLASS='dataCellStr'>" + OfficeId.getName(def.OfficeId) + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + (def.Type == UKClaimType.MFRN ? def.UKDebitNoteNo + "(" + def.ClaimMonth + ")" : def.UKDebitNoteNo) + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + def.Type.Name + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + OfficeId.getName(def.OfficeId) + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + OfficeId.getName(def.PaymentOfficeId) + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + def.ItemNo + "</TD>";
                html += "<TD CLASS='dataCellStr'>" + def.Vendor.Name + "</TD></TR>";
            }
            html += "</TABLE>";

            MessageContent mc = getContent("UKClaimUploadAlertEmail");
            mc.IsHtmlFormat = true;
            mc.StyleSheet = styleStr;
            mc.From = sender;
            mc.To = "carmen_cheung@nextsl.com.hk;";
            foreach (int i in officeIdList)
            {
                mc.To = mc.To + (";" + OfficeId.getNextClaimRecipientList(i));
            }
            mc.Bcc = "michael_lau@nextsl.com.hk;louis_cheng@nextsl.com.hk";
            mc.Subject = "Next Claim Upload Alert";
            mc.setBodyParams(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), html);

            Mail.send(mc);
        }

        public static void sendCutoffSalesDiscrepancyReport(OfficeRef office, string cutOffPeriod, int originalMonthEndStatusId, DateTime updateTime, string filePath, int userId)
        {
            try
            {
                string receiverName = "All";
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("CutoffSalesDiscrepancyListEmail");
                mc.From = sender;
                if (user != null && !string.IsNullOrEmpty(user.EmailAddress))
                {
                    mc.To = user.EmailAddress;
                    receiverName = user.DisplayName;
                }
                SystemParameterRef param = CommonWorker.Instance.getSystemParameterByName(office.OfficeCode + "_CUTOFF_DISCREPANCY_EMAIL_TO");
                if (param != null)
                    if (!string.IsNullOrEmpty(param.ParameterValue))
                    {
                        mc.To = param.ParameterValue;
                        receiverName = "All";
                    }
                param = CommonWorker.Instance.getSystemParameterByName(office.OfficeCode + "_CUTOFF_DISCREPANCY_EMAIL_CC");
                if (param != null)
                    if (!string.IsNullOrEmpty(param.ParameterValue))
                        mc.Cc = param.ParameterValue;
                mc.Subject = "ISAM: Month-End Closing Discrepancy List (" + cutOffPeriod + " - " + office.OfficeCode + ")";
                string status = (originalMonthEndStatusId == office.MonthEndStatusId ? MonthEndStatus.getStatus(office.MonthEndStatusId).Description : MonthEndStatus.getStatus(originalMonthEndStatusId).Description + " -> " + MonthEndStatus.getStatus(office.MonthEndStatusId).Description);
                mc.setBodyParams(receiverName, updateTime.ToShortDateString() + " " + updateTime.ToLocalTime().ToString("hh:mm:ss"), office.OfficeCode, cutOffPeriod, status);
                mc.Attachments.Add(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendMonthEndSlippageReport(OfficeRef office, string cutOffPeriod, DateTime updateTime, string filePath, int userId)
        {
            try
            {
                string receiverName = "All";
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("MonthEndSlippageReportEmail");
                mc.From = sender;
                if (user != null && !string.IsNullOrEmpty(user.EmailAddress))
                {
                    mc.To = user.EmailAddress;
                    receiverName = user.DisplayName;
                }
                SystemParameterRef param = CommonWorker.Instance.getSystemParameterByName(office.OfficeCode + "_CUTOFF_SLIPPAGE_EMAIL_TO");
                if (param != null)
                    if (!string.IsNullOrEmpty(param.ParameterValue))
                    {
                        mc.To = param.ParameterValue;
                        receiverName = "All";
                    }
                param = CommonWorker.Instance.getSystemParameterByName(office.OfficeCode + "_CUTOFF_SLIPPAGE_EMAIL_CC");
                if (param != null)
                    if (!string.IsNullOrEmpty(param.ParameterValue))
                        mc.Cc = param.ParameterValue;
                mc.Subject = "ISAM: Month-End Closing Slippage List (" + cutOffPeriod + " - " + office.OfficeCode + ")";
                mc.setBodyParams(receiverName, updateTime.ToShortDateString() + " " + updateTime.ToLocalTime().ToString("HH:mm:ss"), office.OfficeCode, cutOffPeriod);

                mc.Attachments.Add(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendMonthEndStatusNotice(OfficeRef office, string cutOffPeriod, int originalMonthEndStatusId, DateTime updateTime, UserRef user)
        {
            try
            {
                MessageContent mc = getContent("MonthEndStatusNotificationEmail");
                mc.From = sender;
                if (user != null && !string.IsNullOrEmpty(user.EmailAddress))
                    mc.To = user.EmailAddress;
                SystemParameterRef paramTo = CommonWorker.Instance.getSystemParameterByName(office.OfficeCode + "_CUTOFF_STATUS_EMAIL");

                if (paramTo != null)
                    if (!string.IsNullOrEmpty(paramTo.ParameterValue))
                        mc.To = paramTo.ParameterValue;
                SystemParameterRef paramCC = CommonWorker.Instance.getSystemParameterByName(office.OfficeCode + "_CUTOFF_STATUS_EMAIL_CC");

                if (paramCC != null)
                    if (!string.IsNullOrEmpty(paramCC.ParameterValue))
                        mc.Cc = (string.IsNullOrEmpty(mc.Cc) ? "" : ";") + paramCC.ParameterValue;
                string originalStatus = MonthEndStatus.getStatus(originalMonthEndStatusId).Description;
                string newStatus = MonthEndStatus.getStatus(office.MonthEndStatusId).Description;
                mc.Subject = "ISAM: Month-End Closing Notification Email (" + cutOffPeriod + " - " + office.OfficeCode + " - " + (office.MonthEndStatusId == MonthEndStatus.FAILED.Id ? " ***** " + newStatus + " ***** " : newStatus) + ")";
                if (originalMonthEndStatusId == MonthEndStatus.FAILED.Id)
                    originalStatus = "<label style='color:Red; background-color:Yellow; font-weight:bold;'>" + originalStatus + "</label>";
                if (office.MonthEndStatusId == MonthEndStatus.FAILED.Id)
                    newStatus = "<label style='color:Red; background-color:Yellow; font-weight:bold;'>" + newStatus + "</label>";
                string status = (originalMonthEndStatusId == office.MonthEndStatusId ? newStatus : originalStatus + " -> " + newStatus);
                mc.setBodyParams(user.DisplayName, updateTime.ToShortDateString() + " " + updateTime.ToLocalTime().ToString("HH:mm:ss"), office.OfficeCode, cutOffPeriod, status);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        /*
        public static void sendMonthEndStatusResetNotice(string offices, string cutOffPeriod, DateTime updateTime)
        {
            try
            {
                MessageContent mc = getContent("MonthEndStatusNotificationEmail");
                SystemParameterRef paramTo = CommonWorker.Instance.getSystemParameterByName("SYSTEM_CUTOFF_EMAIL");
                if (paramTo != null)
                    if (!string.IsNullOrEmpty(paramTo.ParameterValue))
                        mc.To = paramTo.ParameterValue;
                mc.From = sender;
                mc.Subject = "ISAM: Month-End Closing Notification Email (" + cutOffPeriod + " - " + MonthEndStatus.READY.Description + ")";
                string status = "Any Status -> " + MonthEndStatus.READY.Description;
                mc.setBodyParams("ISAM Admin", updateTime.ToShortDateString() + " " + updateTime.ToLocalTime().ToString("hh:mm:ss"), offices , cutOffPeriod, status);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }
        */

        public static void sendMonthEndStatusResetNotice(string offices, string cutOffPeriod, DateTime updateTime)
        {
            string subject = "ISAM: Month-End Closing Notification Email (" + cutOffPeriod + " - " + MonthEndStatus.READY.Description + ")";
            string description = "Any Status -> " + MonthEndStatus.READY.Description;
            sendMonthEndGeneralNotice(offices, cutOffPeriod, subject, description, updateTime);
        }


        public static void sendMonthEndGeneralNotice(string offices, string cutOffPeriod, string subject, string description, DateTime updateTime)
        {
            try
            {
                MessageContent mc = getContent("MonthEndStatusNotificationEmail");
                SystemParameterRef paramTo = CommonWorker.Instance.getSystemParameterByName("SYSTEM_CUTOFF_EMAIL");
                if (paramTo != null)
                    if (!string.IsNullOrEmpty(paramTo.ParameterValue))
                        mc.To = paramTo.ParameterValue;
                mc.From = sender;
                mc.Subject = subject + " (" + cutOffPeriod + ")";
                mc.setBodyParams("ISAM Admin", updateTime.ToShortDateString() + " " + updateTime.ToLocalTime().ToString("hh:mm:ss"), offices, cutOffPeriod, description);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }


        public static void sendMonthEndGeneralMessage(OfficeRef office, int fiscalYear, int period, string subject, string content)
        {
            string heading = "Sales Cutoff " + fiscalYear.ToString() + " P" + period.ToString() + " for " + office.OfficeCode + " office<br>\n---------------------------------------------<br>\n";
            NoticeHelper.sendGeneralMessage(subject, heading + "<br>" + content + "<br>");
        }

        public static void sendNUKSalesCutOffWarning(int fiscalYear, int fiscalPeriod, ArrayList list)
        {
            MessageContent mc = getContent("NUKSalesCutOffShipmentWarning");
            mc.IsHtmlFormat = true;
            mc.Subject = "ISAM : NUKSales Cut off - Shipment cannot be identified";
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk; alan_foo@nextsl.com.hk";

            string content = "<TABLE BORDER=1 CELLPADDING=2 CELLSPACING=0>";
            content += "<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#CCCCFF;font-family:Calibri;'><td>Contract No</td><td>Delivery No</td><td>NSL Invoice No</td><td>NUK Extract Date</td></tr>";

            foreach (NUKSalesDef sDef in list)
            {
                content += string.Format("<tr style='font-size:12;font-family:Calibri;'><td style='width:120px;'>{0}</td><td>{1}</td><td style='width:200px;'>{2}</td><td>{3}</td></tr>",
                    sDef.ContractNo, sDef.DeliveryNo, sDef.NSLInvoiceNo, sDef.OkToMoveDate.ToString("dd/MM/yyyy"));
            }

            content += "</table>";

            mc.setBodyParams(fiscalYear, fiscalPeriod, content);
            Mail.send(mc);

        }


        public static void sendNUKSalesCutOffNotification(int fiscalYear, int fiscalPeriod, string filePath)
        {
            MessageContent mc = getContent("NUKSalesCutOffShipmentNotification");
            mc.IsHtmlFormat = true;
            mc.Subject = "ISAM: NUK Sales Cut-Off " + fiscalYear.ToString() + " P" + fiscalPeriod.ToString();
            mc.From = sender;
            mc.To = "michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk;"
                + "candice_ng@nextsl.com.hk;isabella_chiu@nextsl.com.hk;ivan_chong@nextsl.com.hk;teresa_wong@nextsl.com.hk;carmen_cheung@nextsl.com.hk;emily_fan@nextsl.com.hk;";

            mc.setBodyParams(fiscalYear, fiscalPeriod);
            mc.Attachments = ConvertUtility.createArrayList(filePath);
            Mail.send(mc);

        }

        public static void sendNUKSalesShippingReport(int fiscalYear, int fiscalPeriod, DateTime ilsAWHDateFrom, DateTime ilsAWHDateTo, string filePath)
        {
            MessageContent mc = getContent("NUKSalesShippingReportEmail");
            mc.IsHtmlFormat = true;
            mc.Subject = "ISAM: NUK Sales Cut-Off " + fiscalYear.ToString() + " P" + fiscalPeriod.ToString();
            mc.From = sender;
            mc.To = "Cindy_Li@sh.nextsl.com.cn";
            /*
            mc.Cc = "jessie@nextsl.com.hk";
            */

            mc.setBodyParams(fiscalYear, fiscalPeriod, DateTimeUtility.getDateString(ilsAWHDateFrom), DateTimeUtility.getDateString(ilsAWHDateTo));
            mc.Attachments = ConvertUtility.createArrayList(filePath);
            Mail.send(mc);

        }

        public static void sendMockShopReport(int officeId, int fiscalYear, int fiscalPeriod, int userId, string filePath)
        {
            MessageContent mc = getContent("MockShopReportEmail");
            mc.IsHtmlFormat = true;
            mc.Subject = "ISAM: " + OfficeId.getName(officeId) + " Mock Shop D/N " + fiscalYear.ToString() + " P" + fiscalPeriod.ToString();
            mc.From = sender;

            UserRef user = GeneralWorker.Instance.getUserByKey(userId);
            mc.To = user.EmailAddress;
            mc.Bcc = "michael_lau@nextsl.com.hk";
            mc.setBodyParams(user.DisplayName);

            mc.Attachments = ConvertUtility.createArrayList(filePath);
            Mail.send(mc);
        }

        public static void sendStudioReport(int officeId, int fiscalYear, int fiscalPeriod, int userId, string filePath)
        {
            MessageContent mc = getContent("StudioReportEmail");
            mc.IsHtmlFormat = true;
            mc.Subject = "ISAM: " + OfficeId.getName(officeId) + " Studio Sample D/N " + fiscalYear.ToString() + " P" + fiscalPeriod.ToString();
            mc.From = sender;

            UserRef user = GeneralWorker.Instance.getUserByKey(userId);
            mc.To = user.EmailAddress;
            mc.Bcc = "michael_lau@nextsl.com.hk";
            mc.setBodyParams(user.DisplayName);

            mc.Attachments = ConvertUtility.createArrayList(filePath);
            Mail.send(mc);
        }

        public static void sendUTDebitNote(int officeId, int fiscalYear, int fiscalPeriod, int userId, bool isDraft, string filePath)
        {
            MessageContent mc = getContent("UTDebitNoteEmail");
            mc.IsHtmlFormat = true;
            UTContractDCNoteDef dnDef = AccountWorker.Instance.getUTContractDCNoteByLogicalKey(fiscalYear, fiscalPeriod, officeId);

            mc.Subject = (isDraft ? "[DRAFT]" : dnDef.DCNoteNo) + " NSL Debit Note for UT  - Commission and Service Charge";
            mc.From = sender;

            UserRef user = GeneralWorker.Instance.getUserByKey(userId);
            mc.To = isDraft ? user.EmailAddress : "Andrew_Wu@next.cn;Amy_Shen@next.cn;Lucy_Xi@next.cn";

            mc.Cc = "QADN_ToSupplier@nextsl.com.hk";
            mc.Bcc = "michael_lau@nextsl.com.hk";
            mc.setBodyParams(fiscalYear.ToString() + " Period " + fiscalPeriod.ToString());

            mc.Attachments = ConvertUtility.createArrayList(filePath);
            Mail.send(mc);
        }

        public static void sendQACommissionDebitNote(int officeId, int fiscalYear, int fiscalPeriod, int userId, bool isDraft, string filePath, int vendorId)
        {
            try
            {
                QACommissionDNDef dnDef = AccountWorker.Instance.getQACommissionDNByLogicalKey(fiscalYear, fiscalPeriod, officeId, vendorId);

                VendorRef vendor = VendorWorker.Instance.getVendorByKey(vendorId);
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);

                string supplierEmail = String.Empty;
                if (vendor.eAdviceAddr != null)
                {
                    supplierEmail = vendor.eAdviceAddr.Trim();
                }
                string toEmail = String.Empty;
                if (isDraft == true)
                {
                    toEmail = user.EmailAddress;
                }
                else if (isDraft == false && supplierEmail == "")
                {
                    toEmail = user.EmailAddress;
                }
                else if (isDraft == false && supplierEmail != "")
                {
                    toEmail = supplierEmail;
                }
                if (supplierEmail != "")
                {
                    MessageContent mc = getContent("UTQACommissionEmail");
                    mc.IsHtmlFormat = true;
                    mc.Subject = ((dnDef == null) ? "[DRAFT]" : dnDef.DNNo) + " NSL Debit Note for QA Commission " + vendor.Name;
                    mc.From = sender;

                    mc.To = toEmail;
                    mc.Cc = "QADN_ToSupplier@nextsl.com.hk";
                    mc.Bcc = "michael_lau@nextsl.com.hk";
                    mc.setBodyParams(vendor.Name);

                    mc.Attachments = ConvertUtility.createArrayList(filePath);
                    Mail.send(mc);
                }
                else
                {
                    MessageContent mc = getContent("UTQACommissionMissingSupplierEmail");
                    mc.StyleSheet = styleStr;
                    mc.From = sender;
                    mc.To = toEmail;
                    mc.Cc = "QADN_ToSupplier@nextsl.com.hk";
                    mc.Bcc = "michael_lau@nextsl.com.hk";
                    mc.Subject = "NSL UT QA Commission Note for [" + vendor.Name + "] (No Email Address)";
                    mc.setBodyParams(vendor.Name);
                    mc.Attachments = ConvertUtility.createArrayList(filePath);
                    Mail.send(mc);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send UT QA Commission Note Missing Supplier", e, "");
            }
        }

        public static void sendUTForecastReport(string filePath, string from, string to)
        {
            try
            {
                MessageContent mc = getContent("UTForecastReportEmail");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                mc.To = "finance@next.cn; direct_shipment@next.cn";
                mc.Bcc = "michael_lau@nextsl.com.hk;louis_cheng@nextsl.com.hk;cliff_wong@nextsl.com.hk";
                mc.Subject = "UT Forecast Report [" + from + " To " + to + "][HK&SH]";
                mc.setBodyParams(DateTime.Now.ToString("MMM"));
                mc.Attachments = ConvertUtility.createArrayList(filePath);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send UTForcastReport Fail", e, "");
            }
        }

        public static void sendLogoXMLFile(int fiscalYear, int fiscalPeriod, int userId, string filePath)
        {
            MessageContent mc = getContent("LogoXMLFileEmail");
            mc.IsHtmlFormat = true;
            mc.Subject = "ISAM: " + " Logo XML File " + fiscalYear.ToString() + " P" + fiscalPeriod.ToString();
            mc.From = sender;

            UserRef user = GeneralWorker.Instance.getUserByKey(userId);
            mc.To = user.EmailAddress;
            mc.Bcc = "michael_lau@nextsl.com.hk;cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk";
            mc.setBodyParams(user.DisplayName);

            mc.Attachments = ConvertUtility.createArrayList(filePath);
            Mail.send(mc);
        }

        public static void sendAdvancePaymentFailureAlert(string contract, string reason, string remark, int userId)
        {
            MessageContent mc = getContent("AdvancePaymentFailureAlertEmail");
            mc.IsHtmlFormat = true;
            mc.Subject = "ISAM : Advance Payment Synchronization Failure";
            mc.From = sender;

            UserRef user = GeneralWorker.Instance.getUserByKey(userId);
            mc.To = user.DisplayName;
            mc.setBodyParams(user.DisplayName, contract, reason, remark);

            Mail.send(mc);
        }

        public static void sendILSDiffDCNoteReport(int officeId, int fiscalYear, int fiscalPeriod, int userId, string filePath)
        {
            MessageContent mc = getContent("ILSDiffDCNoteReportEmail");
            mc.IsHtmlFormat = true;
            mc.Subject = "ISAM: " + OfficeId.getName(officeId) + " ILS Diff DC Note D/N " + fiscalYear.ToString() + " P" + fiscalPeriod.ToString();
            mc.From = sender;

            UserRef user = GeneralWorker.Instance.getUserByKey(userId);
            mc.To = user.EmailAddress;
            mc.Bcc = "michael_lau@nextsl.com.hk;cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk";
            mc.setBodyParams(user.DisplayName);

            mc.Attachments = ConvertUtility.createArrayList(filePath);
            Mail.send(mc);
        }

        public static void sendGenericDCNoteReportToUser(int officeId, int fiscalYear, int fiscalPeriod, int userId, string dcNoteIndicator, string filePath)
        {
            MessageContent mc = getContent("GenericDCNoteReportEmail");
            mc.IsHtmlFormat = true;
            mc.Subject = "ISAM: " + OfficeId.getName(officeId) + " Other Charges " + (dcNoteIndicator == "D" ? "Debit" : "Credit") + " Note " + fiscalYear.ToString() + " P" + fiscalPeriod.ToString();
            mc.From = sender;

            UserRef user = GeneralWorker.Instance.getUserByKey(userId);
            mc.To = user.EmailAddress;
            mc.Bcc = "michael_lau@nextsl.com.hk;cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk";
            mc.setBodyParams(user.DisplayName, dcNoteIndicator == "D" ? "debit" : "credit");

            mc.Attachments = ConvertUtility.createArrayList(filePath);
            Mail.send(mc);
        }

        public static void sendThirdPartyCustomerDNToUK(int officeId, string supplierId, int fiscalYear, int fiscalPeriod, string invoiceNo, int userId, string filePath)
        {
            MessageContent mc = getContent("ThirdPartyCustomerDNToUKEmail");
            mc.IsHtmlFormat = true;

            mc.Subject = "NSL Invoice - " + invoiceNo + " (Supplier code: " + supplierId + ")";
            /*
            mc.Subject = "NSL " + OfficeId.getName(officeId) + " Office Commission Invoices (Supplier ID: " + supplierId + ")";
            */

            mc.From = sender;

            //UserRef user = GeneralWorker.Instance.getUserByKey(userId);
            mc.To = "invoicestonext@next.co.uk";
            mc.Cc = "carmen_cheung@nextsl.com.hk";
            //mc.To = "louis_cheng@nextsl.com.hk";
            mc.Bcc = "michael_lau@nextsl.com.hk;cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk";
            //mc.setBodyParams(user.DisplayName);

            mc.Attachments = ConvertUtility.createArrayList(filePath);
            Mail.send(mc);
        }

        public static void sendUKDiscountClaimAlert(int userId, string contractDelivery)
        {
            try
            {
                UserRef user = GeneralWorker.Instance.getUserByKey(userId);
                MessageContent mc = getContent("UKDiscountClaimAlert");
                mc.StyleSheet = styleStr;
                mc.From = sender;
                mc.Subject = "Release Lock Request For UK Discount Contract - " + contractDelivery;
                mc.To = user.EmailAddress;
                mc.Bcc = "michael_lau@nextsl.com.hk;louis_cheng@nextsl.com.hk";
                mc.setBodyParams(user.DisplayName);
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert("Send UK Discount Claim Alert Failure", e, "");
            }
        }

        public static void sendEpicorUploadImportLogAlert(ArrayList epicorList)
        {
            MessageContent mc = getContent("EpicorInterfaceUploadAlert");
            mc.IsHtmlFormat = true;
            mc.Subject = "Epicor Interface Upload Alert";
            mc.From = sender;
            mc.To = "tobylo@nextsl.com.hk; michael_lau@nextsl.com.hk; cliff_wong@nextsl.com.hk; louis_cheng@nextsl.com.hk;";

            string text = "<TABLE BORDER=1 CELLPADDING=2 CELLSPACING=0>";
            text += "<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#CCCCFF;font-family:Calibri;'>";
            text += "<td>Epicor Upload Import Log Id</td>";
            text += "<td>Epicor Company</td>";
            text += "<td>Interface Module</td>";
            text += "<td>NS Source Type</td>";
            text += "<td>Document Type</td>";
            text += "<td>Group Id</td>";
            text += "<td>Original File Name</td>";
            text += "<td>File Name</td>";
            text += "<td>Uploaded On</td>";
            text += "<td>Created On</td>";
            text += "<td>Created By</td>";
            text += "<td>Process Queue</td></tr>";

            foreach (EpicorUploadImportLogDef def in epicorList)
            {
                UserRef user = GeneralWorker.Instance.getUserByKey(def.CreatedBy);

                text += "<tr style='font-size:12;font-family:Calibri;'>";
                text += "<td style='width:80px;'>" + def.EpicorUploadImportLogId.ToString() + "</td>";
                text += "<td style='width:80px;'>" + def.EpicorCompany + "</td>";
                text += "<td style='width:80px;'>" + def.InterfaceModule + "</td>";
                text += "<td style='width:80px;'>" + def.NSSourceType + "</td>";
                text += "<td style='width:80px;'>" + def.DocumentType + "</td>";
                text += "<td style='width:80px;'>" + def.GroupId + "</td>";
                text += "<td style='width:200px;'>" + def.OriginalFileName + "</td>";
                text += "<td style='width:200px;'>" + def.FileName + "</td>";
                text += "<td style='width:120px;'>" + def.UploadedOn.ToString() + "</td>";
                text += "<td style='width:120px;'>" + def.CreatedOn.ToString() + "</td>";
                text += "<td style='width:120px;'>" + user.DisplayName + "</td>";
                text += "<td style='width:80px;'>" + def.ProcessQueue + "</td>";
                text += "</tr>";
            }
            text += "</table>";

            mc.setBodyParams(text);
            Mail.send(mc);
        }

        public static void sendInvoiceWithBCEmail(ArrayList filePath, string toAddress, params string[] obj)
        {
            try
            {
                MessageContent mc = getContent("InvoiceWithBCEmail");
                mc.From = sender;
                mc.To = toAddress;
                mc.setSubjectParams(obj);
                mc.Attachments = filePath;
                mc.IsHtmlFormat = true;
                Mail.send(mc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MailHelper.sendErrorAlert(e, "");
            }
        }

        public static void sendNssWeeklySalesReportToSimon(int fiscalYear, int fiscalPeriod, int weekNo, DateTime reportDate, ArrayList filePath, string summaryTable, string remark)
        {
            MessageContent mc = getContent("SendNssWeeklySalesReportToSimon");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.Bcc = "toby_lo@nextsl.com.hk;michael_lau@nextsl.com.hk;cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk";
            mc.To = "toby_lo@nextsl.com.hk";
            mc.Subject = string.Format("{0}-P{1} (Week {2}) sales result as of {3} ", fiscalYear.ToString(), fiscalPeriod.ToString(), weekNo.ToString(), reportDate.ToString("dd-MMM-yyyy"));
            mc.setBodyParams(fiscalYear.ToString(), fiscalPeriod.ToString(), weekNo.ToString(), reportDate.ToString("dd-MMM-yyyy"), remark, summaryTable);
            mc.Attachments = filePath;
            Mail.send(mc);
        }

        public static void sendNssWeeklySalesReportToIT(DateTime reportDate, ArrayList filePath)
        {
            MessageContent mc = getContent("SendNssWeeklySalesReportToIT");
            mc.IsHtmlFormat = true;
            mc.From = sender;
            mc.Bcc = "toby_lo@nextsl.com.hk;michael_lau@nextsl.com.hk;cliff_wong@nextsl.com.hk;louis_cheng@nextsl.com.hk";
            mc.Subject = string.Format("Weekly Sales Report as of {0} ", reportDate.ToString("dd-MMM-yyyy"));
            mc.setBodyParams(reportDate.ToString("dd-MMM-yyyy"));
            mc.Attachments = filePath;
            Mail.send(mc);
        }

        /*
        #region getContent
        
        public static MessageContent getContent(string messageKey)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(WebConfig.getValue("projectSource", "bin") + "mail.config");

            if (ds.Tables.IndexOf(messageKey) >= 0 && ds.Tables[messageKey].Rows.Count > 0)
            {
                MessageContent mc = new MessageContent();
                DataRow dr = ds.Tables[messageKey].Rows[0];

                mc.To = (string)dr["DefaultRecipient"];
                mc.Subject = (string)dr["Subject"];
                mc.Body = (string)dr["Body"];
                mc.Cc = (string)dr["Cc"];
                mc.Bcc = (string)dr["Bcc"];
                return mc;
            }
            else
            {
                throw new Exception("Message Key Invalid: " + messageKey);
            }
        }

        #endregion
        */
    }
}
