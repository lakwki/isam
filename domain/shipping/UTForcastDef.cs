using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;

namespace com.next.isam.domain.shipping
{
    [Serializable()]
    public class UTForcastDef : DomainData
    {
        private string customer;
        private string productTeam;
        private string season;
        private int phaseId;
        private string office;
        private string supplier;
        private string itemNo;
        private string contractNo;
        private int deliveryNo;
        private DateTime customerAtWarehouseDate;
        private int totalPOQty;
        private string unit;
        private string co;
        private string loadingPort;
        private string customerDestination;
        private string sellCurrency;
        private string supplierInvoiceNo;
        private string packingMethod;

        public UTForcastDef()
        {

        }

        public string Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        public string ProductTeam
        {
            get { return productTeam; }
            set { productTeam = value; }
        }

        public string Season
        {
            get { return season; }
            set { season = value; }
        }

        public int PhaseId
        {
            get { return phaseId; }
            set { phaseId = value; }
        }

        public string Office
        {
            get { return office; }
            set { office = value; }
        }

        public string Supplier
        {
            get { return supplier; }
            set { supplier = value; }
        }

        public string ItemNo
        {
            get { return itemNo; }
            set { itemNo = value; }
        }

        public string ContractNo
        {
            get { return contractNo; }
            set { contractNo = value; }
        }

        public int DeliveryNo
        {
            get { return deliveryNo; }
            set { deliveryNo = value; }
        }

        public DateTime CustomerAtWarehouseDate
        {
            get { return customerAtWarehouseDate; }
            set { customerAtWarehouseDate = value; }
        }

        public int TotalPOQty
        {
            get { return totalPOQty; }
            set { totalPOQty = value; }
        }

        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        public string CO
        {
            get { return co; }
            set { co = value; }
        }

        public string LoadingPort
        {
            get { return loadingPort; }
            set { loadingPort = value; }
        }

        public string CustomerDestination
        {
            get { return customerDestination; }
            set { customerDestination = value; }
        }

        public string SellCurrency
        {
            get { return sellCurrency; }
            set { sellCurrency = value; }
        }

        public string SupplierInvoiceNo
        {
            get { return supplierInvoiceNo; }
            set { supplierInvoiceNo = value; }
        }

        public string PackingMethod
        {
            get { return packingMethod; }
            set { packingMethod = value; }
        }
    }
}
