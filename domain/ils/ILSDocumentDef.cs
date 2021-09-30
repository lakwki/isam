using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSDocumentDef : DomainData
    {
        private int orderRefId;
        private string contractNo;
        private string deliveryNo;
        private string docType;
        private string docNo;
        private string docCountry;
        private string docActualType;
        private DateTime docStartDate;
        private DateTime docExpiryDate;
        private string docQuotaCat;
        private decimal weight;
        private int pieces;
        private string fileNo;
        private DateTime importDate;

        public ILSDocumentDef()
		{

		}

        public int OrderRefId 
        { 
            get { return orderRefId; } 
            set { orderRefId = value; } 
        }

		public string ContractNo 
        { 
            get { return contractNo; } 
            set { contractNo = value; } 
        }

        public string DeliveryNo 
        { 
            get { return deliveryNo; } 
            set { deliveryNo = value; } 
        }

		public string DocumentType 
        { 
            get { return docType; } 
            set { docType = value; } 
        }

        public string DocumentNo 
        { 
            get { return docNo; } 
            set { docNo = value; } 
        }

        public string DocumentCountry 
        { 
            get { return docCountry; } 
            set { docCountry = value; } 
        }

        public string ActualType 
        { 
            get { return docActualType; } 
            set { docActualType = value; } 
        }

        public DateTime StartDate 
        { 
            get { return docStartDate; } 
            set { docStartDate = value; } 
        }

        public DateTime ExpiryDate 
        { 
            get { return docExpiryDate; } 
            set { docExpiryDate = value; } 
        }

        public string QuotaCategory 
        { 
            get { return docQuotaCat; } 
            set { docQuotaCat = value; } 
        }

        public decimal Weight 
        { 
            get { return weight; } 
            set { weight = value; } 
        }

        public int Pieces 
        { 
            get { return pieces; } 
            set { pieces = value; } 
        }

        public string FileNo 
        { 
            get { return fileNo; } 
            set { fileNo = value; } 
        }

        public DateTime ImportDate 
        { 
            get { return importDate; } 
            set { importDate = value; } 
        }
    }
}
