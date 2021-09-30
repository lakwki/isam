using System;
using com.next.common.domain;
using com.next.isam.domain.common;

namespace com.next.isam.domain.product
{
    /// <summary>
    /// Summary description for ProductDef.
    /// </summary>

    [Serializable()]
    public class ProductDef : DomainData
    {
        private int productId;
        private string itemNo;
        private string description1;
        private string description2;
        private string description3;
        private string description4;
        private string description5;
        private string shortDesc;
        private string colour;
        private string styleDesignRef;
        private DesignSourceRef designSource;
        private CartonTypeRef cartonType;
        private int qtyPerCarton;

        private int designerId;

        private string garmentWash;

        private string originalItemNo;
        private string originalDescription1;
        private string originalDescription2;
        private string originalDescription3;
        private string originalDescription4;
        private string originalDescription5;
        private string originalColour;
        private int status;

        public ProductDef()
        {
        }

        public int ProductId
        {
            get
            {
                return productId;
            }
            set
            {
                productId = value;
            }
        }

        public string ItemNo
        {
            get
            {
                if (itemNo != null) return itemNo; else return String.Empty;
            }
            set
            {
                itemNo = value;
            }
        }

        public string Description1
        {
            get
            {
                return description1;
            }
            set
            {
                description1 = value;
            }
        }

        public string Description2
        {
            get
            {
                return description2;
            }
            set
            {
                description2 = value;
            }
        }

        public string Description3
        {
            get
            {
                return description3;
            }
            set
            {
                description3 = value;
            }
        }

        public string Description4
        {
            get
            {
                return description4;
            }
            set
            {
                description4 = value;
            }
        }

        public string Description5
        {
            get
            {
                return description5;
            }
            set
            {
                description5 = value;
            }
        }

        public string ShortDesc
        {
            get
            {
                return shortDesc;
            }
            set
            {
                shortDesc = value;
            }
        }

        public string Colour
        {
            get
            {
                return colour;
            }
            set
            {
                colour = value;
            }
        }

        public string StyleDesignRef
        {
            get
            {
                return styleDesignRef;
            }
            set
            {
                styleDesignRef = value;
            }
        }

        public DesignSourceRef DesignSource
        {
            get
            {
                return designSource;
            }
            set
            {
                designSource = value;
            }
        }

        public int DesignerId
        {
            get
            {
                return designerId;
            }
            set
            {
                designerId = value;
            }
        }

        public CartonTypeRef CartonType
        {
            get
            {
                return cartonType;
            }
            set
            {
                cartonType = value;
            }
        }

        public int QtyPerCarton
        {
            get
            {
                return qtyPerCarton;
            }
            set
            {
                qtyPerCarton = value;
            }
        }


        public string OriginalItemNo
        {
            get
            {
                return originalItemNo;
            }
            set
            {
                originalItemNo = value;
            }
        }

        public string OriginalDescription1
        {
            get
            {
                return originalDescription1;
            }
            set
            {
                originalDescription1 = value;
            }
        }

        public string OriginalDescription2
        {
            get
            {
                return originalDescription2;
            }
            set
            {
                originalDescription2 = value;
            }
        }

        public string OriginalDescription3
        {
            get
            {
                return originalDescription3;
            }
            set
            {
                originalDescription3 = value;
            }
        }

        public string OriginalDescription4
        {
            get
            {
                return originalDescription4;
            }
            set
            {
                originalDescription4 = value;
            }
        }

        public string OriginalDescription5
        {
            get
            {
                return originalDescription5;
            }
            set
            {
                originalDescription5 = value;
            }
        }

        public string OriginalColour
        {
            get
            {
                return originalColour;
            }
            set
            {
                originalColour = value;
            }
        }

        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }


        public string GarmentWash
        {
            get
            {
                return garmentWash;
            }
            set
            {
                garmentWash = value;
            }
        }

        public string MasterDescription1 { get; set; }
        public string MasterDescription2 { get; set; }
        public string MasterDescription3 { get; set; }
        public string MasterDescription4 { get; set; }
        public string MasterDescription5 { get; set; }
        public int ProductDesignStyleId { get; set; }
        public string RetailDescription { get; set; }

    }
}
