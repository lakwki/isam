using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.types
{
    public class RejectPaymentReason : DomainData
    {
        public static RejectPaymentReason NoReason = new RejectPaymentReason(Code.NoReason);
        public static RejectPaymentReason OtherReasons = new RejectPaymentReason(Code.OtherReasons);

        public static RejectPaymentReason DocDiscrepancy = new RejectPaymentReason(Code.DocDiscrepancy);
        public static RejectPaymentReason IncorrectDoc = new RejectPaymentReason(Code.IncorrectDoc);
        public static RejectPaymentReason MerchRequestUpdate = new RejectPaymentReason(Code.MerchRequestUpdate);
        public static RejectPaymentReason ShippingRequestReturn = new RejectPaymentReason(Code.ShippingRequestReturn);
        public static RejectPaymentReason IncorrectCurrency = new RejectPaymentReason(Code.IncorrectCurrency);
        public static RejectPaymentReason DoNotAcceptMultiInvoices = new RejectPaymentReason(Code.DoNotAcceptMultiInvoices);
        public static RejectPaymentReason CannotOpenPDF = new RejectPaymentReason(Code.CannotOpenPDF);
        public static RejectPaymentReason IncorrectSupplierInvoiceNo = new RejectPaymentReason(Code.IncorrectSupplierInvoiceNo);
        public static RejectPaymentReason Unknown = new RejectPaymentReason(Code.Unknown);
        public static RejectPaymentReason IncorrectSupplierInvoiceAmount = new RejectPaymentReason(Code.IncorrectSupplierInvoiceAmount);
        public static RejectPaymentReason IncorrectSupplierName = new RejectPaymentReason(Code.IncorrectSupplierName);
        public static RejectPaymentReason IncompleteShippingDocuments = new RejectPaymentReason(Code.IncompleteShippingDocuments);
        public static RejectPaymentReason UnclearDocuments = new RejectPaymentReason(Code.UnclearDocuments);
        public static RejectPaymentReason MissingSupplierInvoiceDocument = new RejectPaymentReason(Code.MissingSupplierInvoiceDocument);
        public static RejectPaymentReason ReviewMistake = new RejectPaymentReason(Code.ReviewMistake);
        public static RejectPaymentReason IncorrectPaymentTerm = new RejectPaymentReason(Code.IncorrectPaymentTerm);
        public static RejectPaymentReason WrongInvoicingParty = new RejectPaymentReason(Code.WrongInvoicingParty);

        private Code _code;

        private enum Code
        {
            NoReason = 0,
            OtherReasons = 1, 
            /*
            PriceAmended = 2,
            QuantityAmended = 3,
            CurrencyAmended = 4,
            ExchangeRateAmended = 5,
            */
            DocDiscrepancy = 2,
            IncorrectDoc = 3,
            MerchRequestUpdate = 4,
            ShippingRequestReturn = 5,
            IncorrectCurrency = 6,
            DoNotAcceptMultiInvoices =7,
            CannotOpenPDF = 8,
            IncorrectSupplierInvoiceNo = 9,
            IncorrectSupplierInvoiceAmount = 10,
            IncorrectSupplierName = 11,
            IncompleteShippingDocuments = 12,
            UnclearDocuments = 13,
            MissingSupplierInvoiceDocument = 14,
            ReviewMistake = 15,
            IncorrectPaymentTerm = 16,
            WrongInvoicingParty = 17,
            Unknown = 99
        }

        private RejectPaymentReason(Code code)
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

        public string Name
        {
            get
            {
                switch (_code)
                {
                    case Code.NoReason:
                        return "No Reason";
                    case Code.DocDiscrepancy:
                        return "Document discrepancy with system information";
                    case Code.IncorrectDoc:
                        return "Incorrect document attached";
                    case Code.MerchRequestUpdate:
                        return "Merchandiser request to update system value";
                    case Code.ShippingRequestReturn:
                        return "Shipping request to return document";
                    case Code.IncorrectCurrency:
                        return "Incorrect currency";
                    case Code.DoNotAcceptMultiInvoices:
                        return "Do Not Accept Multi Invoices";
                    case Code.CannotOpenPDF:
                        return "Cannot open PDF file";
                    case Code.IncorrectSupplierInvoiceNo:
                        return "Incorrect supplier invoice no.";
                    case Code.IncorrectSupplierInvoiceAmount:
                        return "Discrepancy with supplier invoice amount";
                    case Code.IncorrectSupplierName:
                        return "Incorrect supplier name";
                    case Code.OtherReasons:
                        return "Other Reasons";
                    case Code.IncompleteShippingDocuments:
                        return "Incomplete Shipping Documents";
                    case Code.UnclearDocuments:
                        return "Documents uploaded are unclear";
                    case Code.MissingSupplierInvoiceDocument:
                        return "Missing the document of supplier invoice";
                    case Code.ReviewMistake:
                        return "Mark as “ reviewed” by mistake (as the shipment is missing GB test result or failure/applied release lock)";
                    case Code.IncorrectPaymentTerm:
                        return "Incorrect Payment Term";
                    case Code.WrongInvoicingParty:
                        return "Wrong invoicing party - the sales invoices has to be invoiced to Next Sourcing Ltd";
                    case Code.Unknown:
                        return "Unknown";
                    default:
                        return "ERROR";
                }
            }
        }

        public static RejectPaymentReason getReason(int id)
        {
            if (id == Code.NoReason.GetHashCode()) return RejectPaymentReason.NoReason;
            else if (id == Code.OtherReasons.GetHashCode()) return RejectPaymentReason.OtherReasons;
            else if (id == Code.DocDiscrepancy.GetHashCode()) return RejectPaymentReason.DocDiscrepancy;
            else if (id == Code.IncorrectDoc.GetHashCode()) return RejectPaymentReason.IncorrectDoc;
            else if (id == Code.MerchRequestUpdate.GetHashCode()) return RejectPaymentReason.MerchRequestUpdate;
            else if (id == Code.ShippingRequestReturn.GetHashCode()) return RejectPaymentReason.ShippingRequestReturn;
            else if (id == Code.IncorrectCurrency.GetHashCode()) return RejectPaymentReason.IncorrectCurrency;
            else if (id == Code.DoNotAcceptMultiInvoices.GetHashCode()) return RejectPaymentReason.DoNotAcceptMultiInvoices;
            else if (id == Code.CannotOpenPDF.GetHashCode()) return RejectPaymentReason.CannotOpenPDF;
            else if (id == Code.IncorrectSupplierInvoiceNo.GetHashCode()) return RejectPaymentReason.IncorrectSupplierInvoiceNo;
            else if (id == Code.IncorrectSupplierInvoiceAmount.GetHashCode()) return RejectPaymentReason.IncorrectSupplierInvoiceAmount;
            else if (id == Code.IncorrectSupplierName.GetHashCode()) return RejectPaymentReason.IncorrectSupplierName;
            else if (id == Code.IncompleteShippingDocuments.GetHashCode()) return RejectPaymentReason.IncompleteShippingDocuments;
            else if (id == Code.UnclearDocuments.GetHashCode()) return RejectPaymentReason.UnclearDocuments;
            else if (id == Code.MissingSupplierInvoiceDocument.GetHashCode()) return RejectPaymentReason.MissingSupplierInvoiceDocument;
            else if (id == Code.ReviewMistake.GetHashCode()) return RejectPaymentReason.ReviewMistake;
            else if (id == Code.IncorrectPaymentTerm.GetHashCode()) return RejectPaymentReason.IncorrectPaymentTerm;
            else if (id == Code.WrongInvoicingParty.GetHashCode()) return RejectPaymentReason.WrongInvoicingParty;
            else if (id == Code.Unknown.GetHashCode()) return RejectPaymentReason.Unknown;
            else return null;
        }

        public static ArrayList getCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(RejectPaymentReason.DocDiscrepancy);
            list.Add(RejectPaymentReason.IncorrectDoc);
            list.Add(RejectPaymentReason.MerchRequestUpdate);
            list.Add(RejectPaymentReason.ShippingRequestReturn);
            list.Add(RejectPaymentReason.IncorrectCurrency);
            list.Add(RejectPaymentReason.DoNotAcceptMultiInvoices);
            list.Add(RejectPaymentReason.CannotOpenPDF);
            list.Add(RejectPaymentReason.IncorrectSupplierInvoiceNo);
            list.Add(RejectPaymentReason.IncorrectSupplierInvoiceAmount);
            list.Add(RejectPaymentReason.IncorrectSupplierName);
            list.Add(RejectPaymentReason.IncompleteShippingDocuments);
            list.Add(RejectPaymentReason.UnclearDocuments);
            list.Add(RejectPaymentReason.MissingSupplierInvoiceDocument);
            list.Add(RejectPaymentReason.ReviewMistake);
            list.Add(RejectPaymentReason.IncorrectPaymentTerm);
            list.Add(RejectPaymentReason.WrongInvoicingParty);
            return list;
        }

    }
}
