using System;
using System.Collections.Generic;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.nontrade
{

    [Serializable()]
    public class NTInvoiceDetailType : DomainData
    {

        public static NTInvoiceDetailType USER = new NTInvoiceDetailType(Code.user);
        public static NTInvoiceDetailType OFFICE = new NTInvoiceDetailType(Code.office);
        public static NTInvoiceDetailType GARMENT_VENDOR = new NTInvoiceDetailType(Code.garmentvendor);
        public static NTInvoiceDetailType FABRIC_VENDOR = new NTInvoiceDetailType(Code.fabricvendor);
        public static NTInvoiceDetailType TRIM_VENDOR = new NTInvoiceDetailType(Code.trimvendor);
        public static NTInvoiceDetailType NON_CLOTHING_VENDOR = new NTInvoiceDetailType(Code.nonclothingvendor);
        public static NTInvoiceDetailType PACKAGING_VENDOR = new NTInvoiceDetailType(Code.packagingvendor);
        public static NTInvoiceDetailType LAUNDRY_VENDOR = new NTInvoiceDetailType(Code.laundryvendor);
        public static NTInvoiceDetailType CUSTOMER = new NTInvoiceDetailType(Code.customer);
        public static NTInvoiceDetailType COSTCENTER = new NTInvoiceDetailType(Code.costcenter);
        public static NTInvoiceDetailType NT_VENDOR = new NTInvoiceDetailType(Code.ntvendor);

        private Code _code;

        private NTInvoiceDetailType(Code code)
        {
            this._code = code;
        }


        private enum Code
        {
            costcenter = 1,
            user = 2,
            office = 3,
            garmentvendor = 4,
            fabricvendor = 5,
            trimvendor = 6,
            nonclothingvendor = 7,
            packagingvendor = 8,
            laundryvendor = 9,
            customer = 10,
            ntvendor = 11
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
                    case Code.user:
                        return "User";
                    case Code.office:
                        return "NS Office";
                    case Code.garmentvendor:
                        return "Garment Vendor";
                    case Code.fabricvendor:
                        return "Fabric Vendor";
                    case Code.trimvendor:
                        return "Trim Vendor";
                    case Code.nonclothingvendor:
                        return "Non-Clothing Vendor";
                    case Code.packagingvendor:
                        return "Packaging Vendor";
                    case Code.laundryvendor:
                        return "Laundry Vendor";
                    case Code.customer :
                        return "Customer";
                    case Code.costcenter :
                        return "Cost Centre";
                    case Code.ntvendor :
                        return "NT Vendor";
                    default:
                        return "UNDEFINED";
                }
            }
        }

        public static NTInvoiceDetailType getType(int id)
        {
            if (id == Code.user.GetHashCode()) return NTInvoiceDetailType.USER;
            else if (id == Code.office.GetHashCode()) return NTInvoiceDetailType.OFFICE;
            else if (id == Code.garmentvendor.GetHashCode()) return NTInvoiceDetailType.GARMENT_VENDOR;
            else if (id == Code.fabricvendor.GetHashCode()) return NTInvoiceDetailType.FABRIC_VENDOR;
            else if (id == Code.trimvendor.GetHashCode()) return NTInvoiceDetailType.TRIM_VENDOR;
            else if (id == Code.nonclothingvendor.GetHashCode()) return NTInvoiceDetailType.NON_CLOTHING_VENDOR;
            else if (id == Code.packagingvendor.GetHashCode()) return NTInvoiceDetailType.PACKAGING_VENDOR;
            else if (id == Code.laundryvendor.GetHashCode()) return NTInvoiceDetailType.LAUNDRY_VENDOR;
            else if (id == Code.customer.GetHashCode()) return NTInvoiceDetailType.CUSTOMER;
            else if (id == Code.costcenter.GetHashCode()) return NTInvoiceDetailType.COSTCENTER;
            else if (id == Code.ntvendor.GetHashCode()) return NTInvoiceDetailType.NT_VENDOR;
            else return null;
        }

        public static ArrayList getRechargeTypeCollectionValues()
        {
            ArrayList list = new ArrayList();
            list.Add(NTInvoiceDetailType.USER);
            list.Add(NTInvoiceDetailType.OFFICE);
            list.Add(NTInvoiceDetailType.GARMENT_VENDOR);
            list.Add(NTInvoiceDetailType.FABRIC_VENDOR);
            list.Add(NTInvoiceDetailType.TRIM_VENDOR);
            list.Add(NTInvoiceDetailType.NON_CLOTHING_VENDOR);
            list.Add(NTInvoiceDetailType.PACKAGING_VENDOR);
            list.Add(NTInvoiceDetailType.LAUNDRY_VENDOR);
            list.Add(NTInvoiceDetailType.CUSTOMER);
            list.Add(NTInvoiceDetailType.NT_VENDOR);
            return list;
        }

        public static TypeCollector getTypeCollector()
        {
            TypeCollector coll = TypeCollector.Inclusive;
            coll.append(NTInvoiceDetailType.USER.Id);
            coll.append(NTInvoiceDetailType.OFFICE.Id);
            coll.append(NTInvoiceDetailType.GARMENT_VENDOR.Id);
            coll.append(NTInvoiceDetailType.FABRIC_VENDOR.Id);
            coll.append(NTInvoiceDetailType.TRIM_VENDOR.Id);
            coll.append(NTInvoiceDetailType.NON_CLOTHING_VENDOR.Id);
            coll.append(NTInvoiceDetailType.PACKAGING_VENDOR.Id);
            coll.append(NTInvoiceDetailType.LAUNDRY_VENDOR.Id);
            coll.append(NTInvoiceDetailType.CUSTOMER.Id);
            coll.append(NTInvoiceDetailType.COSTCENTER.Id);
            coll.append(NTInvoiceDetailType.NT_VENDOR.Id);
            return coll;
        }

    }

}
