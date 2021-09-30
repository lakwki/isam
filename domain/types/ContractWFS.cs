using System;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;


namespace com.next.isam.domain.types
{
	[Serializable()]
	public class ContractWFS : DomainData
	{
		public static ContractWFS ENQUIRY = new ContractWFS(Code.Enquiry);
		public static ContractWFS PENDING_FOR_SUBMIT = new ContractWFS(Code.PendingForSubmit);
		public static ContractWFS PENDING_FOR_APPROVAL = new ContractWFS(Code.PendingForApproval);
		public static ContractWFS APPROVED = new ContractWFS(Code.Approved);
		public static ContractWFS INVOICED = new ContractWFS(Code.Invoiced);
		public static ContractWFS AMEND = new ContractWFS(Code.Amend);
		public static ContractWFS CANCELLED = new ContractWFS(Code.Cancelled);
		public static ContractWFS PENDING_FOR_CANCEL_APPROVAL = new ContractWFS(Code.PendingForCancelApproval);
		public static ContractWFS PO_PRINTED = new ContractWFS(Code.POPrinted);
		public static ContractWFS REJECTED = new ContractWFS(Code.Rejected);
		
		private Code _code;
		
		private enum Code 
		{
			Enquiry = 0,
			PendingForSubmit = 1,
			PendingForApproval = 2,
			PendingForCancelApproval = 3,
			Amend = 4,
			Rejected = 5,
			Approved = 6,
			POPrinted = 7,
			Invoiced = 8,
			Cancelled = 9,
		}

		private ContractWFS(Code code)
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
					case Code.Enquiry :
						return "ENQUIRY";
					case Code.PendingForSubmit :
						return "DRAFT";
					case Code.PendingForApproval :
						return "PENDING FOR APPROVAL";
					case Code.Approved :
						return "APPROVED";
					case Code.Invoiced :
						return "INVOICED";
					case Code.Amend :
						return "AMENDED";
					case Code.Cancelled :
						return "CANCELLED";
					case Code.PendingForCancelApproval :
						return "PENDING FOR CANCEL";
					case Code.POPrinted :
						return "PO GENERATED";
					case Code.Rejected :
						return "REJECTED";
					default:
						return "ERROR";
				}				
			}
		}



		public static string getShortName (int id)
		{
			if (id == Code.Enquiry.GetHashCode()) return "Enquiry";
			else if (id == Code.PendingForSubmit.GetHashCode()) return "Draft";
			else if (id == Code.PendingForApproval.GetHashCode()) return "Pend. for Appv.";
			else if (id == Code.Approved.GetHashCode()) return "Approved";
			else if (id == Code.Invoiced.GetHashCode()) return "Invoiced";
			else if (id == Code.Amend.GetHashCode()) return "Amended";
			else if (id == Code.Cancelled.GetHashCode()) return "Cancelled";
			else if (id == Code.PendingForCancelApproval.GetHashCode()) return "Pend. for Cncl.";
			else if (id == Code.POPrinted.GetHashCode()) return "PO Gen.";
			else if (id == Code.Rejected.GetHashCode()) return "Rejected";
			else return "Error";
		}

		public static ContractWFS getType(int id) 
		{
			if (id == Code.Enquiry.GetHashCode()) return ContractWFS.ENQUIRY;
			else if (id == Code.PendingForSubmit.GetHashCode()) return ContractWFS.PENDING_FOR_SUBMIT;
			else if (id == Code.PendingForApproval.GetHashCode()) return ContractWFS.PENDING_FOR_APPROVAL;
			else if (id == Code.Approved.GetHashCode()) return ContractWFS.APPROVED;
			else if (id == Code.Invoiced.GetHashCode()) return ContractWFS.INVOICED;
			else if (id == Code.Amend.GetHashCode()) return ContractWFS.AMEND;
			else if (id == Code.Cancelled.GetHashCode()) return ContractWFS.CANCELLED;
			else if (id == Code.PendingForCancelApproval.GetHashCode()) return ContractWFS.PENDING_FOR_CANCEL_APPROVAL;
			else if (id == Code.POPrinted.GetHashCode()) return ContractWFS.PO_PRINTED;
			else if (id == Code.Rejected.GetHashCode()) return ContractWFS.REJECTED;
			else return null;
		}


		
		public enum StatusType 
		{
			All = 1,
			Actual = 2,
			Booked = 3,
			ActualAndBooked = 4,
			Cancelled = 5,
			InProgress = 6,
		}
		
		public enum ReportType 
		{
			All = 1,
			Actual = 2,
			Booked = 3,
			ActualAndBooked = 4,
			Cancelled = 5,
			ApprovedAndPOPrintedAndInvoiced = 6,
			ApprovedAndPOPrintedAndInvoicedAndCancelled = 7,
			Unconfirmed = 8,
			NotYetShipped = 9,
			ShippedAndNotYetShipped = 10,
			CancelledAndPendingForCancell = 11,
		}

		public static TypeCollector getCollectionForReport(ReportType type) 
		{
			TypeCollector values = TypeCollector.Inclusive;

			if (type.GetHashCode() == ReportType.All.GetHashCode())
			{
				values.append(ContractWFS.PENDING_FOR_SUBMIT.Id);
				values.append(ContractWFS.PENDING_FOR_APPROVAL.Id);
				values.append(ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id);
				values.append(ContractWFS.REJECTED.Id);
				values.append(ContractWFS.AMEND.Id);
				values.append(ContractWFS.INVOICED.Id);
				values.append(ContractWFS.APPROVED.Id);
				values.append(ContractWFS.PO_PRINTED.Id);
			}
			else if (type.GetHashCode() == ReportType.Actual.GetHashCode())
			{
				values.append(ContractWFS.INVOICED.Id);
			}
			else if (type.GetHashCode() == ReportType.Booked.GetHashCode())
			{
				values.append(ContractWFS.PENDING_FOR_APPROVAL.Id);
				values.append(ContractWFS.AMEND.Id);
				values.append(ContractWFS.APPROVED.Id);
				values.append(ContractWFS.PO_PRINTED.Id);
			}
			else if (type.GetHashCode() == ReportType.ActualAndBooked.GetHashCode())
			{
				values.append(ContractWFS.PENDING_FOR_APPROVAL.Id);
				values.append(ContractWFS.AMEND.Id);
				values.append(ContractWFS.APPROVED.Id);
				values.append(ContractWFS.PO_PRINTED.Id);
				values.append(ContractWFS.INVOICED.Id);
			}
			else if (type.GetHashCode() == ReportType.Cancelled.GetHashCode())
			{
				values.append(ContractWFS.CANCELLED.Id);
			}
			else if (type.GetHashCode() == ReportType.ApprovedAndPOPrintedAndInvoiced.GetHashCode())
			{
				values.append(ContractWFS.APPROVED.Id);
				values.append(ContractWFS.PO_PRINTED.Id);
				values.append(ContractWFS.INVOICED.Id);
			}
			else if (type.GetHashCode() == ReportType.ApprovedAndPOPrintedAndInvoicedAndCancelled.GetHashCode())
			{
				values.append(ContractWFS.APPROVED.Id);
				values.append(ContractWFS.PO_PRINTED.Id);
				values.append(ContractWFS.INVOICED.Id);
				values.append(ContractWFS.CANCELLED.Id);
			}
			else if (type.GetHashCode() == ReportType.Unconfirmed.GetHashCode())
			{
				values.append(ContractWFS.PENDING_FOR_SUBMIT.Id);
			}
			else if (type.GetHashCode() == ReportType.NotYetShipped.GetHashCode())
			{
				values.append (ContractWFS.APPROVED.Id);
				values.append (ContractWFS.PO_PRINTED.Id);
				values.append (ContractWFS.AMEND.Id);
				values.append (ContractWFS.PENDING_FOR_APPROVAL.Id);
				values.append (ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id);
				values.append (ContractWFS.PENDING_FOR_SUBMIT.Id);
			}
			else if (type.GetHashCode() == ReportType.ShippedAndNotYetShipped.GetHashCode())
			{
				values.append (ContractWFS.APPROVED.Id);
				values.append (ContractWFS.PO_PRINTED.Id);
				values.append (ContractWFS.AMEND.Id);
				values.append (ContractWFS.PENDING_FOR_APPROVAL.Id);
				values.append (ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id);
				values.append (ContractWFS.PENDING_FOR_SUBMIT.Id);
				values.append(ContractWFS.INVOICED.Id);
			}
			else if (type.GetHashCode() == ReportType.CancelledAndPendingForCancell.GetHashCode())
			{
				values.append (ContractWFS.PENDING_FOR_CANCEL_APPROVAL.Id);
				values.append(ContractWFS.CANCELLED.Id);
			}
			else
			{
				values.append(GeneralCriteria.FALSE);
			}

			return values;
		}

        public static ICollection getCollectionValue()
        {
            ArrayList ary = new ArrayList();
            ary.Add(ContractWFS.ENQUIRY);
            ary.Add(ContractWFS.PENDING_FOR_SUBMIT);
            ary.Add(ContractWFS.PENDING_FOR_APPROVAL);
            ary.Add(ContractWFS.PENDING_FOR_CANCEL_APPROVAL);
            ary.Add(ContractWFS.AMEND);
            ary.Add(ContractWFS.REJECTED);
            ary.Add(ContractWFS.APPROVED);
            ary.Add(ContractWFS.PO_PRINTED);
            ary.Add(ContractWFS.INVOICED);
            ary.Add(ContractWFS.CANCELLED);
            return ary;                			
        }

		public static bool checkStatusType(int ContractWFSId, StatusType type) 
		{
			if (type.GetHashCode() == StatusType.All.GetHashCode())
			{
				if (ContractWFSId == Code.PendingForSubmit.GetHashCode()
					|| ContractWFSId == Code.Amend.GetHashCode()
					|| ContractWFSId == Code.Rejected.GetHashCode() 
					|| ContractWFSId == Code.PendingForApproval.GetHashCode() 
					|| ContractWFSId == Code.PendingForCancelApproval.GetHashCode()
					|| ContractWFSId == Code.Approved.GetHashCode()
					|| ContractWFSId == Code.POPrinted.GetHashCode()
					|| ContractWFSId == Code.Invoiced.GetHashCode())
					return true;
				else
					return false;
			}
			else if (type.GetHashCode() == StatusType.Actual.GetHashCode())
			{
				if (ContractWFSId == Code.Invoiced.GetHashCode())
					return true;
				else
					return false;
			}
			else if (type.GetHashCode() == StatusType.Booked.GetHashCode())
			{
				if (ContractWFSId == Code.PendingForApproval.GetHashCode() 
					|| ContractWFSId == Code.Amend.GetHashCode()
					|| ContractWFSId == Code.Approved.GetHashCode()
					|| ContractWFSId == Code.POPrinted.GetHashCode())
					return true;
				else
					return false;
			}
			else if (type.GetHashCode() == StatusType.ActualAndBooked.GetHashCode())
			{
				if (ContractWFSId == Code.PendingForApproval.GetHashCode() 
					|| ContractWFSId == Code.Amend.GetHashCode()
					||ContractWFSId == Code.Invoiced.GetHashCode() 
					|| ContractWFSId == Code.Approved.GetHashCode()
					|| ContractWFSId == Code.POPrinted.GetHashCode())
					return true;
				else
					return false;
			}
			else if (type.GetHashCode() == StatusType.Cancelled.GetHashCode())
			{
				if (ContractWFSId == Code.Cancelled.GetHashCode())
					return true;
				else
					return false;
			}
			else if (type.GetHashCode() == StatusType.InProgress.GetHashCode())
			{
				if (ContractWFSId == Code.PendingForSubmit.GetHashCode()
					|| ContractWFSId == Code.Amend.GetHashCode()
					|| ContractWFSId == Code.Rejected.GetHashCode() 
					|| ContractWFSId == Code.PendingForApproval.GetHashCode() 
					|| ContractWFSId == Code.PendingForCancelApproval.GetHashCode()
					|| ContractWFSId == Code.Approved.GetHashCode()
					|| ContractWFSId == Code.POPrinted.GetHashCode())
					return true;
				else
					return false;
			}
			else


			{
				return false;
			}
		}
	}
}
