using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class PaymentAdviceDef : DomainData
    {
        private int paymentAdviceId;
        private string sunSupplierId;
        private string supplierName;
        private DateTime payDate;
        private string bankName;
        private string fileName;
        private DateTime uploadDate;
        private bool mailed;
        private string epicorSupplierId;
        private int isChecked;
        private string company;
        

        public PaymentAdviceDef()
        {

        }

        public PaymentAdviceDef(string sunSupplierId, string supplierName, DateTime payDate, string bankName, string fileName, string company, int isChecked)
        {
            this.paymentAdviceId = -1;
            this.sunSupplierId = sunSupplierId;
            this.supplierName = supplierName;
            this.payDate = payDate;
            this.bankName = bankName;
            this.fileName = fileName;
            uploadDate = DateTime.Now;
            mailed = false;
            this.epicorSupplierId = string.Empty;
            this.company = company;
            this.isChecked = isChecked;
        }

        public int PaymentAdviceId
        {
            get { return paymentAdviceId; }
            set { paymentAdviceId = value; }
        }

        public string SUNSupplierId
        {
            get { return sunSupplierId; }
            set { sunSupplierId = value; }
        }

        public string SupplierName
        {
            get { return supplierName; }
            set { supplierName = value; }
        }

        public DateTime PayDate
        {
            get { return payDate; }
            set { payDate = value; }
        }

        public string BankName
        {
            get { return bankName; }
            set { bankName = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public DateTime UploadDate
        {
            get { return uploadDate; }
            set { uploadDate = value; }
        }

        public bool Mailed
        {
            get { return mailed; }
            set { mailed = value; }
        }

        public string EpicorSupplierId
        {
            get { return epicorSupplierId; }
            set { epicorSupplierId = value; }
        }

        public int IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

        public string Company
        {
            get { return company; }
            set { company = value; }
        }

    }
}
