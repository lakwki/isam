using System;
using com.next.common.domain;
using com.next.isam.domain.product;
using com.next.isam.domain.common;

namespace com.next.isam.domain.order
{
	[Serializable()]
	public class ShipmentRef : DomainData
	{
		private int shipmentId;
		private int deliveryNo;
		private string supplierName;
		private int noOfSplit;
		private int isVirtualSetSplit;
		private int totalOrderQuantity;
		private string currencyDescription;
		private decimal totalOrderAmount;
		private decimal totalOPAUpcharge;
		private decimal margin;
		private decimal leadTime;
		private DateTime customerAgreedAtWarehouseDate;
		private int workflowStatusId;
		private string workflowStatusDescription;

		public ShipmentRef()
		{
		}

		public int ShipmentId 
        { 
            get { return shipmentId; } 
            set { shipmentId = value; } 
        }

		public int DeliveryNo 
        { 
            get { return deliveryNo; } 
            set { deliveryNo = value; } 
        }

		public string SupplierName 
        { 
            get { return supplierName; } 
            set { supplierName = value; } 
        }

		public int NoOfSplit 
        { 
            get { return noOfSplit; } 
            set { noOfSplit = value; } 
        }

		public int IsVirtualSetSplit 
        { 
            get { return isVirtualSetSplit; } 
            set { isVirtualSetSplit = value; } 
        }

		public int TotalOrderQuantity 
        { 
            get { return totalOrderQuantity; } 
            set { totalOrderQuantity = value; } 
        }

		public string CurrencyDescription 
        { 
            get { return currencyDescription; } 
            set { currencyDescription = value; } 
        }

		public decimal TotalOrderAmount 
        { 
            get { return totalOrderAmount; } 
            set { totalOrderAmount = value; } 
        }

		public decimal TotalOPAUpcharge 
        { 
            get { return totalOPAUpcharge; } 
            set { totalOPAUpcharge = value; } 
        }

		public decimal Margin 
        { 
            get { return margin; } 
            set { margin = value; } 
        }

		public decimal LeadTime 
        { 
            get { return leadTime; } 
            set { leadTime = value; } 
        } 

		public DateTime CustomerAgreedAtWarehouseDate 
        { 
            get { return customerAgreedAtWarehouseDate; } 
            set { customerAgreedAtWarehouseDate = value; } 
        }

		public int WorkflowStatusId 
        { 
            get { return workflowStatusId; } 
            set { workflowStatusId = value; } 
        }

		public string WorkflowStatusDescription 
        { 
            get { return workflowStatusDescription; } 
            set { workflowStatusDescription = value; } 
        }
	}
}
