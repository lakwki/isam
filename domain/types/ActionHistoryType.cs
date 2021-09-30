using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class ActionHistoryType : DomainData
    {
        public static ActionHistoryType ORDER_COPY_UPLOAD = new ActionHistoryType(Code.OrderCopyUpload);
        public static ActionHistoryType PACKING_LIST_UPLOAD = new ActionHistoryType(Code.PackingListUpload);
        public static ActionHistoryType INVOICE_UPLOAD = new ActionHistoryType(Code.InvoiceUpload);
        public static ActionHistoryType DOCUMENT_UPLOAD = new ActionHistoryType(Code.DocumentUpload);
        public static ActionHistoryType MANIFEST_UPLOAD = new ActionHistoryType(Code.ManifestUpload);
        public static ActionHistoryType MERCHANDISER_UPDATES = new ActionHistoryType(Code.MerchandiserAmendment);
        public static ActionHistoryType SHIPPING_UPDATES = new ActionHistoryType(Code.ShippingAmendment);
        public static ActionHistoryType LC_APPLICATION = new ActionHistoryType(Code.LCApplication);
        public static ActionHistoryType AR_AP_MAINTENANCE = new ActionHistoryType(Code.ARAPMaintenance);
        public static ActionHistoryType E_INVOICE = new ActionHistoryType(Code.EInvoice);
        public static ActionHistoryType SUN_PURHCASE = new ActionHistoryType(Code.SUNPurchase);
        public static ActionHistoryType QCC_MSG = new ActionHistoryType(Code.QCCMessage);
        public static ActionHistoryType DISCREPANCY_MSG = new ActionHistoryType(Code.DISMessage);
        public static ActionHistoryType ORIGIN_MSG = new ActionHistoryType(Code.ORIMessage);
        public static ActionHistoryType DOCUMENT_PRINTOUT = new ActionHistoryType(Code.DocumentPrintOut);
        public static ActionHistoryType ILS_ORDER_CANCELLATION = new ActionHistoryType(Code.ILSOrderCancellation);
        public static ActionHistoryType AR_ADJUSTMENT = new ActionHistoryType(Code.ARAdjustment);
        public static ActionHistoryType AP_ADJUSTMENT = new ActionHistoryType(Code.APAdjustment);
        public static ActionHistoryType MISCELLANEOUS = new ActionHistoryType(Code.Miscellaneous);
        public static ActionHistoryType PACKING_LIST_RESET = new ActionHistoryType(Code.PackingListReset);
        public static ActionHistoryType CONTRACT_REINSTATE = new ActionHistoryType(Code.ContractReinstate);
        public static ActionHistoryType I_NET_GARMENT_SYNC = new ActionHistoryType(Code.SyncToiNetGarment);
        public static ActionHistoryType MARK_READY_TO_SEND_INVOICE = new ActionHistoryType(Code.MarkReadyToSendInvoice);
        public static ActionHistoryType CANCEL_INVOICE = new ActionHistoryType(Code.CancelInvoice);
        public static ActionHistoryType BOOKING_UPLOAD = new ActionHistoryType(Code.BookingUpload);
        public static ActionHistoryType SHIPPING_UPLOAD = new ActionHistoryType(Code.ShippingUpload);
        public static ActionHistoryType SEND_AR_DISCREPANCY_ALERT = new ActionHistoryType(Code.SendARDiscrepancyAlert);
        public static ActionHistoryType LATE_SELF_BILLING = new ActionHistoryType(Code.LateSelfBilling);
        public static ActionHistoryType MONTH_END = new ActionHistoryType(Code.MonthEnd);
        public static ActionHistoryType ILS_INVOICE_CANCELLATION = new ActionHistoryType(Code.ILSInvoiceCancellation);
        public static ActionHistoryType PRICE_DIFF_ALERT = new ActionHistoryType(Code.PriceDiffAlert);
        public static ActionHistoryType PRICE_DIFF_RESOLVED = new ActionHistoryType(Code.PriceDiffResolved);
        public static ActionHistoryType INVOICE_SENT = new ActionHistoryType(Code.InvoiceSent);
        private Code _code;

        private enum Code
        {
            OrderCopyUpload = 2,
            PackingListUpload = 3,
            InvoiceUpload = 4,
            DocumentUpload = 11,
            ManifestUpload = 10,
            MerchandiserAmendment = 1,
            ShippingAmendment = 5,
            LCApplication = 6,
            ARAPMaintenance = 7,
            EInvoice = 8,
            SUNPurchase = 9,
            QCCMessage = 12,
            DISMessage = 13,
            ORIMessage = 14,
            DocumentPrintOut = 15,
            ILSOrderCancellation = 16,
            ARAdjustment = 17,
            Miscellaneous = 18,
            PackingListReset = 19,
            APAdjustment = 20,
            ContractReinstate = 21,
            SyncToiNetGarment = 22,
            MarkReadyToSendInvoice = 23,
            CancelInvoice = 24,
            BookingUpload = 25,
            ShippingUpload = 26,
            SendARDiscrepancyAlert = 27,
            LateSelfBilling = 28,
            MonthEnd = 29,
            ILSInvoiceCancellation = 30,
            PriceDiffAlert = 31,
            PriceDiffResolved = 32,
            InvoiceSent = 33
        }

        private ActionHistoryType(Code code)
        {
            this._code = code;
        }

        public int Id
        {
            get
            {
                return Convert.ToUInt16(_code.GetHashCode());
            }
        }

        public string Description
        {
            get
            {
                switch (_code)
                {
                    case Code.OrderCopyUpload:
                        return "ILS - Order Copy Upload";
                    case Code.PackingListUpload:
                        return "ILS - Packing List Upload";
                    case Code.InvoiceUpload:
                        return "ILS - Self-Billed Invoice Upload";
                    case Code.DocumentUpload:
                        return "ILS - Documentation Upload";
                    case Code.ManifestUpload:
                        return "ILS - Manifest Details Upload";
                    case Code.MerchandiserAmendment:
                        return "NSS - Merchandiser Amendment";
                    case Code.ShippingAmendment:
                        return "ISAM - Shipping Amendment";
                    case Code.LCApplication:
                        return "ISAM - L/C Application";
                    case Code.ARAPMaintenance:
                        return "ISAM - A/R And A/P Record Updates";
                    case Code.EInvoice:
                        return "ISAM - E-Invoice Creation";
                    case Code.SUNPurchase:
                        return "SUN - Purchase Interface File Transferred";
                    case Code.QCCMessage:
                        return "ILS - QCC Message Sent To ILS";
                    case Code.DISMessage:
                        return "ILS - Discrepancy Message Sent To ILS";
                    case Code.ORIMessage:
                        return "ILS - Origin Message Sent To ILS";
                    case Code.DocumentPrintOut:
                        return "ISAM - Document - Print Out";
                    case Code.ILSOrderCancellation:
                        return "ILS - Order Cancellation";
                    case Code.ARAdjustment:
                        return "ISAM - A/R Adjustment Settled";
                    case Code.Miscellaneous :
                        return "ISAM - Miscellaneous";
                    case Code.PackingListReset :
                        return "ILS - Packing List Reset";
                    case Code.ContractReinstate:
                        return "ILS - Contract Reinstate";
                    case Code.APAdjustment:
                        return "ISAM - A/P Adjustment Settled";
                    case Code.SyncToiNetGarment :
                        return "ISAM - iNetGarment Synchronization";
                    case Code.MarkReadyToSendInvoice:
                        return "ISAM - Mark Ready To Send Invoice";
                    case Code.CancelInvoice:
                        return "ISAM - Cancel Invoice";
                    case Code.BookingUpload:
                        return "ISAM - Booking Information Upload";
                    case Code.ShippingUpload:
                        return "ISAM - Shipping Data Upload";
                    case Code.SendARDiscrepancyAlert:
                        return "ISAM - AR Discrepancy Alert";
                    case Code.LateSelfBilling:
                        return "ILS - Late Self-Billing";
                    case Code.MonthEnd:
                        return "ISAM - Month End";
                    case Code.ILSInvoiceCancellation:
                        return "ILS - Invoice Cancellation";
                    case Code.PriceDiffAlert:
                        return "ISAM - Price Diff Alert";
                    case Code.PriceDiffResolved:
                        return "ISAM - Price Diff Resolved";
                    case Code.InvoiceSent:
                        return "ISAM - Invoice Sent to Customer";
                    default:
                        return "ERROR";
                }
            }
        }

        public static ActionHistoryType getType(int id)
        {
            if (id == Code.OrderCopyUpload.GetHashCode()) return ActionHistoryType.ORDER_COPY_UPLOAD;
            else if (id == Code.PackingListUpload.GetHashCode()) return ActionHistoryType.PACKING_LIST_UPLOAD;
            else if (id == Code.InvoiceUpload.GetHashCode()) return ActionHistoryType.INVOICE_UPLOAD;
            else if (id == Code.DocumentUpload.GetHashCode()) return ActionHistoryType.DOCUMENT_UPLOAD;
            else if (id == Code.ManifestUpload.GetHashCode()) return ActionHistoryType.MANIFEST_UPLOAD;
            else if (id == Code.MerchandiserAmendment.GetHashCode()) return ActionHistoryType.MERCHANDISER_UPDATES;
            else if (id == Code.ShippingAmendment.GetHashCode()) return ActionHistoryType.SHIPPING_UPDATES;
            else if (id == Code.LCApplication.GetHashCode()) return ActionHistoryType.LC_APPLICATION;
            else if (id == Code.ARAPMaintenance.GetHashCode()) return ActionHistoryType.AR_AP_MAINTENANCE;
            else if (id == Code.EInvoice.GetHashCode()) return ActionHistoryType.E_INVOICE;
            else if (id == Code.SUNPurchase.GetHashCode()) return ActionHistoryType.SUN_PURHCASE;
            else if (id == Code.QCCMessage.GetHashCode()) return ActionHistoryType.QCC_MSG;
            else if (id == Code.DISMessage.GetHashCode()) return ActionHistoryType.DISCREPANCY_MSG;
            else if (id == Code.ORIMessage.GetHashCode()) return ActionHistoryType.ORIGIN_MSG;
            else if (id == Code.DocumentPrintOut.GetHashCode()) return ActionHistoryType.DOCUMENT_PRINTOUT;
            else if (id == Code.ILSOrderCancellation.GetHashCode()) return ActionHistoryType.ILS_ORDER_CANCELLATION;
            else if (id == Code.ARAdjustment.GetHashCode()) return ActionHistoryType.AR_ADJUSTMENT;
            else if (id == Code.APAdjustment.GetHashCode()) return ActionHistoryType.AP_ADJUSTMENT;
            else if (id == Code.Miscellaneous.GetHashCode()) return ActionHistoryType.MISCELLANEOUS;
            else if (id == Code.PackingListReset.GetHashCode()) return ActionHistoryType.PACKING_LIST_RESET;
            else if (id == Code.ContractReinstate.GetHashCode()) return ActionHistoryType.CONTRACT_REINSTATE;
            else if (id == Code.SyncToiNetGarment.GetHashCode()) return ActionHistoryType.I_NET_GARMENT_SYNC;
            else if (id == Code.MarkReadyToSendInvoice.GetHashCode()) return ActionHistoryType.MARK_READY_TO_SEND_INVOICE;
            else if (id == Code.CancelInvoice.GetHashCode()) return ActionHistoryType.CANCEL_INVOICE;
            else if (id == Code.BookingUpload.GetHashCode()) return ActionHistoryType.BOOKING_UPLOAD;
            else if (id == Code.ShippingUpload.GetHashCode()) return ActionHistoryType.SHIPPING_UPLOAD;
            else if (id == Code.SendARDiscrepancyAlert.GetHashCode()) return ActionHistoryType.SEND_AR_DISCREPANCY_ALERT;
            else if (id == Code.LateSelfBilling.GetHashCode()) return ActionHistoryType.LATE_SELF_BILLING;
            else if (id == Code.MonthEnd.GetHashCode()) return ActionHistoryType.MONTH_END;
            else if (id == Code.ILSInvoiceCancellation.GetHashCode()) return ActionHistoryType.ILS_INVOICE_CANCELLATION;
            else if (id == Code.PriceDiffAlert.GetHashCode()) return ActionHistoryType.PRICE_DIFF_ALERT;
            else if (id == Code.PriceDiffResolved.GetHashCode()) return ActionHistoryType.PRICE_DIFF_RESOLVED;
            else if (id == Code.InvoiceSent.GetHashCode()) return ActionHistoryType.INVOICE_SENT;
            else return null;
        }
    }
}
