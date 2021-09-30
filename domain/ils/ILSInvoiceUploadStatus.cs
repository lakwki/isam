using System;
using com.next.common.domain;
using System.Collections;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSInvoiceUploadStatus : DomainData
    {
        public static ILSInvoiceUploadStatus UNASSIGNED_SHIPMENT = new ILSInvoiceUploadStatus(Code.UnassignedShipment);
        public static ILSInvoiceUploadStatus DRAFT = new ILSInvoiceUploadStatus(Code.Draft);
        public static ILSInvoiceUploadStatus PENDING_FOR_APPROVAL = new ILSInvoiceUploadStatus(Code.PendingForApproval);
        public static ILSInvoiceUploadStatus PENDING_FOR_CANCEL_APPROVAL = new ILSInvoiceUploadStatus(Code.PendingForCancelApproval);
        public static ILSInvoiceUploadStatus AMENDED = new ILSInvoiceUploadStatus(Code.Amended);
        public static ILSInvoiceUploadStatus REJECTED = new ILSInvoiceUploadStatus(Code.Rejected);
        public static ILSInvoiceUploadStatus CANCELLED = new ILSInvoiceUploadStatus(Code.Cancelled);
        public static ILSInvoiceUploadStatus ALREADY_INVOICED = new ILSInvoiceUploadStatus(Code.AlreadyInvoiced);
        public static ILSInvoiceUploadStatus QUANTITY_MISMATCH = new ILSInvoiceUploadStatus(Code.QuantityMismatch);
        public static ILSInvoiceUploadStatus CURRENCY_MISMATCH = new ILSInvoiceUploadStatus(Code.CurrencyMismatch);
        public static ILSInvoiceUploadStatus OPTION_MISMATCH = new ILSInvoiceUploadStatus(Code.OptionMismatch);
        public static ILSInvoiceUploadStatus MISSING_PACKING_LIST_DATA = new ILSInvoiceUploadStatus(Code.MissingPackingListData);
        public static ILSInvoiceUploadStatus INVOICE_DATE_TOLERANCE_ISSUE = new ILSInvoiceUploadStatus(Code.InvoiceDateToleranceIssue);
        public static ILSInvoiceUploadStatus INVOICE_NO_BEING_USED = new ILSInvoiceUploadStatus(Code.InvoiceNoIsBeingUsed);
        public static ILSInvoiceUploadStatus PENDING = new ILSInvoiceUploadStatus(Code.Pending);
        public static ILSInvoiceUploadStatus MISSING_FACTORYID_FOR_NML = new ILSInvoiceUploadStatus(Code.MissingFactoryIdForNML);
        public static ILSInvoiceUploadStatus NOT_SELFBILLED_ORDER = new ILSInvoiceUploadStatus(Code.NotSelfBilledOrder);

        private Code _code;

        private enum Code
        {
            UnassignedShipment = 1500,
            Draft = 1501,
            PendingForApproval = 1502,
            PendingForCancelApproval = 1503,
            Amended = 1504,
            Rejected = 1505,
            Cancelled = 1509,
            AlreadyInvoiced = 1601,
            QuantityMismatch = 1701,
            CurrencyMismatch = 1801,
            OptionMismatch = 1901,
            MissingPackingListData = 2001,
            InvoiceDateToleranceIssue = 2101,
            InvoiceNoIsBeingUsed = 2201,
            MissingFactoryIdForNML = 2301,
            NotSelfBilledOrder = 2401,
            Pending = 9999
        }

        private ILSInvoiceUploadStatus(Code code)
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

        public string AlertText
        {
            get
            {
                switch (_code)
                {
                    case Code.UnassignedShipment:
                        return "This shipment cannot be found in NSS. Please contact NUK to clarify.";
                    case Code.Draft:
                        return "This shipment is stated in DRAFT status. Please submit and approve the shipment.";
                    case Code.PendingForApproval:
                        return "This shipment is stated in PENDING FOR APPROVAL status. Please submit and approve the shipment.";
                    case Code.PendingForCancelApproval:
                        return "This shipment is stated in PENDING FOR CANCEL status. Please submit and approve the shipment.";
                    case Code.Amended:
                        return "This shipment is stated in AMENDED status. Please submit and approve the shipment.";
                    case Code.Rejected:
                        return "This shipment is stated in REJECTED status. Please submit and approve the shipment.";
                    case Code.Cancelled:
                        return "This shipment is stated in CANCELLED status. Please submit and approve the shipment.";
                    case Code.AlreadyInvoiced:
                        return "This shipment has already been invoiced manually (NOT via Self-Billing). Please contact IT for investigation.";
                    case Code.QuantityMismatch:
                        return "This shipment has Quantity Discrepancy with NUK record. Please contact NUK to clarify.";
                    case Code.CurrencyMismatch:
                        return "This shipment has Currency Discrepancy with NUK record. Please contact NUK to clarify.";
                    case Code.OptionMismatch:
                        return "This shipment has Size Breakdown Discrepancy with NUK record. Please contact NUK to clarify.";
                    case Code.MissingPackingListData:
                        return "Packing List Data is missing for this shipment. Please contact NUK to clarify.";
                    case Code.InvoiceDateToleranceIssue:
                        return "Invoice Date of this shipment is over 60 Days tolerance. Please contact NUK / IT to clarify.";
                    case Code.InvoiceNoIsBeingUsed:
                        return "The Invoice No. has been used by another shipment";
                    case Code.Pending:
                        return "This invoice is currently suspended for upload by IT.";
                    case Code.MissingFactoryIdForNML:
                        return "This shipment’s factory is NOT yet confirmed by NML. Please notify NML to assign the factory.";
                    case Code.NotSelfBilledOrder:
                        return "This shipment is not a self-billed order";
                    default:
                        return "ERROR";
                }
            }
        }

        public static ILSInvoiceUploadStatus getType(int id)
        {
            if (id == Code.UnassignedShipment.GetHashCode()) return ILSInvoiceUploadStatus.UNASSIGNED_SHIPMENT;
            else if (id == Code.Draft.GetHashCode()) return ILSInvoiceUploadStatus.DRAFT;
            else if (id == Code.PendingForApproval.GetHashCode()) return ILSInvoiceUploadStatus.PENDING_FOR_APPROVAL;
            else if (id == Code.PendingForCancelApproval.GetHashCode()) return ILSInvoiceUploadStatus.PENDING_FOR_CANCEL_APPROVAL;
            else if (id == Code.Amended.GetHashCode()) return ILSInvoiceUploadStatus.AMENDED;
            else if (id == Code.Rejected.GetHashCode()) return ILSInvoiceUploadStatus.REJECTED;
            else if (id == Code.Cancelled.GetHashCode()) return ILSInvoiceUploadStatus.CANCELLED;
            else if (id == Code.AlreadyInvoiced.GetHashCode()) return ILSInvoiceUploadStatus.ALREADY_INVOICED;
            else if (id == Code.QuantityMismatch.GetHashCode()) return ILSInvoiceUploadStatus.QUANTITY_MISMATCH;
            else if (id == Code.CurrencyMismatch.GetHashCode()) return ILSInvoiceUploadStatus.CURRENCY_MISMATCH;
            else if (id == Code.OptionMismatch.GetHashCode()) return ILSInvoiceUploadStatus.OPTION_MISMATCH;
            else if (id == Code.MissingPackingListData.GetHashCode()) return ILSInvoiceUploadStatus.MISSING_PACKING_LIST_DATA;
            else if (id == Code.InvoiceDateToleranceIssue.GetHashCode()) return ILSInvoiceUploadStatus.INVOICE_DATE_TOLERANCE_ISSUE;
            else if (id == Code.InvoiceNoIsBeingUsed.GetHashCode()) return ILSInvoiceUploadStatus.INVOICE_NO_BEING_USED;
            else if (id == Code.Pending.GetHashCode()) return ILSInvoiceUploadStatus.PENDING;
            else if (id == Code.MissingFactoryIdForNML.GetHashCode()) return ILSInvoiceUploadStatus.MISSING_FACTORYID_FOR_NML;
            else if (id == Code.NotSelfBilledOrder.GetHashCode()) return ILSInvoiceUploadStatus.NOT_SELFBILLED_ORDER;
            else return null;
        }
    }
}
