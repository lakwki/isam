using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class CustomerTypeRef : DomainData
    {
        public enum Type
        {
            retail = 1,
            directory = 2,
            franchise = 3,
            discountstore = 4,
            lipsy = 5,
            globaltextiles = 6,
            daysgroup = 7,
            bravado = 8,
            ezibuy = 9,
            nexthempel = 10,
            smithbrooks = 11,
            fashionuk = 12,
            globallicensing = 13,
            misirli = 14,
            brand = 15,
            AS = 16,
            brandalliance = 17,
            blues = 18,
            william = 19,
            fabricflavours = 21,
            nsled = 22,
            manuled = 23,
            arnoldwills = 24,
            tvm_fashion = 25,
            bioworld = 26,
            paul_dennicci = 27,
            difuzed = 28,
            lm_oasis = 29,
            lm_ted_baker = 30,
            branddesign = 31,
            poetic_brands = 32,
            forever_new = 33,
            cortina = 34,
            sicem = 35,
            cooneen = 36,
            lm_label_mix = 37,
            lm_little_bird = 38,
            lm_myleene_klass = 39,
            lm_nine_by_savannah = 40,
            lm_mintie = 41,
            lm_fat_face = 42,
            lm_laura_ashley = 43,
            lm_mint_velvet = 44,
            lm_hype = 45,
            rnsled = 99
        }

        public static bool isFranchise(int type)
        {
            if (type == Type.franchise.GetHashCode()) return true;
            else return false;
        }

        public static bool isDiscountStore(int type)
        {
            if (type == Type.discountstore.GetHashCode()) return true;
            else return false;
        }

        private int customerTypeId;
        private string customerTypeDescription;

        public CustomerTypeRef()
        {
        }

        public int CustomerTypeId
        {
            get { return customerTypeId; }
            set { customerTypeId = value; }
        }

        public string CustomerTypeDescription
        {
            get { return customerTypeDescription; }
            set { customerTypeDescription = value; }
        }

        public string CustomerTypeCode
        {
            get { return customerTypeDescription.Substring(0, 1); }
        }

    }
}
