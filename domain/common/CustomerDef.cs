using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class CustomerDef : DomainData
    {

        public enum Id
        {
            directory = 1,
            retail = 2,
            middleeast = 3,
            japan = 4,
            ntn_directory = 5,
            ntn_retail = 6,
            lime = 7,
            choice = 8,
            lipsy = 9,
            gt = 10,
            days = 11,
            bravado = 12,
            ezibuy = 13,
            smithbrooks = 15,
            hempel = 16,
            fashionUK = 17,
            global_licensing = 18,
            misirli = 19,
            brand_international = 20,
            aykroyd_sons = 21,
            brand_alliance = 22,
            blues_clothing = 23,
            william_lamb = 24,
            fabric_flavours = 26,
            ns_led = 27,
            manu_led = 28,
            arnold_wills = 29,
            tvm_fashion = 30,
            bioworld = 31,
            paul_dennicci = 32,
            difuzed = 33,
            lm_oasis = 34,
            lm_ted_baker = 35,
            brand_design = 36,
            poetic_brands = 37,
            forever_new = 38,
            cortina = 39,
            sicem = 40,
            cooneen = 41, 
            lm_label_mix = 42,
            lm_little_bird = 43,
            lm_myleene_klass = 44,
            lm_nine = 45,
            lm_mintie = 46,
            lm_fat_face = 47,
            lm_laura_ashley = 48,
            lm_mint_velvet = 49,
            lm_hype = 50,
            retail_ns_led = 99
        }


        private int customerId;
        private string customerCode;
        private string customerDescription;
        private CustomerTypeRef customerType;
        private string invoicePrefix;
        private string address1;
        private string address2;
        private string address3;
        private string address4;
        private string telNo;
        private string faxNo;
        private string telexNo;
        private string contact;
        private string consignee;
        private string deliveryTo;
        private string opsKey;
        private string shortCode;
        private string sunAccountCode;
        private string epicorCustomerId;
        private bool isPaymentRequired;
        private bool isSelfBilling;

        public CustomerDef()
        {
        }

        public int CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public string CustomerCode
        {
            get { return customerCode; }
            set { customerCode = value; }
        }

        public string CustomerDescription
        {
            get { return customerDescription; }
            set { customerDescription = value; }
        }

        public CustomerTypeRef CustomerType
        {
            get { return customerType; }
            set { customerType = value; }
        }

        public string InvoicePrefix
        {
            get { return invoicePrefix; }
            set { invoicePrefix = value; }
        }

        public string Address1
        {
            get { return address1; }
            set { address1 = value; }
        }

        public string Address2
        {
            get { return address2; }
            set { address2 = value; }
        }

        public string Address3
        {
            get { return address3; }
            set { address3 = value; }
        }

        public string Address4
        {
            get { return address4; }
            set { address4 = value; }
        }

        public string TelNo
        {
            get { return telNo; }
            set { telNo = value; }
        }

        public string FaxNo
        {
            get { return faxNo; }
            set { faxNo = value; }
        }

        public string TelexNo
        {
            get { return telexNo; }
            set { telexNo = value; }
        }

        public string Contact
        {
            get { return contact; }
            set { contact = value; }
        }

        public string Consignee
        {
            get { return consignee; }
            set { consignee = value; }
        }

        public string DeliveryTo
        {
            get { return deliveryTo; }
            set { deliveryTo = value; }
        }

        public string OPSKey
        {
            get { return opsKey; }
            set { opsKey = value; }
        }

        public string ShortCode
        {
            get { return shortCode; }
            set { shortCode = value; }
        }

        public string SUNAccountCode
        {
            get { return sunAccountCode; }
            set { sunAccountCode = value; }
        }

        public string EpicorCustomerId
        {
            get { return epicorCustomerId; }
            set { epicorCustomerId = value; }
        }

        public bool IsPaymentRequired
        {
            get { return isPaymentRequired; }
            set { isPaymentRequired = value; }
        }

        public bool IsSelfBilling
        {
            get { return isSelfBilling; }
            set { isSelfBilling = value; }
        }

        public static string getCode(int id)
        {
            if (id == Id.directory.GetHashCode()) return "DIRECTORY";
            else if (id == Id.retail.GetHashCode()) return "RETAIL";
            else if (id == Id.middleeast.GetHashCode()) return " ALSHAYA";
            else if (id == Id.japan.GetHashCode()) return "XEBIO";
            else if (id == Id.ntn_retail.GetHashCode()) return "NTN (RETAIL)";
            else if (id == Id.ntn_directory.GetHashCode()) return "NTN (DIRECTORY)";
            else if (id == Id.lime.GetHashCode()) return "LIME";
            else if (id == Id.choice.GetHashCode()) return "CHOICE";
            else if (id == Id.lipsy.GetHashCode()) return "LIPSY";
            else if (id == Id.gt.GetHashCode()) return "GLOBAL TEXTILES";
            else if (id == Id.days.GetHashCode()) return "DAYSGROUP";
            else if (id == Id.bravado.GetHashCode()) return "BRAVADO";
            else if (id == Id.ezibuy.GetHashCode()) return "EZIBUY";
            else if (id == Id.smithbrooks.GetHashCode()) return "SMITH AND BROOKS";
            else if (id == Id.hempel.GetHashCode()) return "HEMPEL";
            else if (id == Id.fashionUK.GetHashCode()) return "FASHION UK";
            else if (id == Id.global_licensing.GetHashCode()) return "GLOBAL LICENSING";
            else if (id == Id.misirli.GetHashCode()) return "MISIRLI";
            else if (id == Id.brand_international.GetHashCode()) return "BRAND INTERNATIONAL";
            else if (id == Id.aykroyd_sons.GetHashCode()) return "AYKROYD & SONS LTD";
            else if (id == Id.brand_alliance.GetHashCode()) return "BRAND ALLIANCE";
            else if (id == Id.blues_clothing.GetHashCode()) return "BLUES CLOTHING";
            else if (id == Id.william_lamb.GetHashCode()) return "WILLIAM LAMB";
            else if (id == Id.fabric_flavours.GetHashCode()) return "FABRIC FLAVOURS";
            else if (id == Id.ns_led.GetHashCode()) return "DIRECTORY (NS LED)";
            else if (id == Id.manu_led.GetHashCode()) return "DIRECTORY (MANU LED)";
            else if (id == Id.arnold_wills.GetHashCode()) return "ARNOLD WILLS";
            else if (id == Id.tvm_fashion.GetHashCode()) return "TVM FASHION LAB LTD";
            else if (id == Id.bioworld.GetHashCode()) return "BIOWORLD INTERNATIONAL LIMITED";
            else if (id == Id.paul_dennicci.GetHashCode()) return "PAUL DENNICCI LTD";
            else if (id == Id.difuzed.GetHashCode()) return "DIFUZED B.V.";
            else if (id == Id.lm_oasis.GetHashCode()) return "LM - OASIS";
            else if (id == Id.lm_ted_baker.GetHashCode()) return "LM - TED BAKER";
            else if (id == Id.retail_ns_led.GetHashCode()) return "RETAIL (NS LED)";
            else if (id == Id.brand_design.GetHashCode()) return "BRANDDESIGN SA";
            else if (id == Id.poetic_brands.GetHashCode()) return "POETIC BRANDS";
            else if (id == Id.cortina.GetHashCode()) return "CORTINA";
            else if (id == Id.sicem.GetHashCode()) return "SICEM";
            else if (id == Id.cooneen.GetHashCode()) return "COONEEN";
            else if (id == Id.lm_label_mix.GetHashCode()) return "LM - LABEL MIX";
            else if (id == Id.lm_little_bird.GetHashCode()) return "LM - LITTLE BIRD";
            else if (id == Id.lm_myleene_klass.GetHashCode()) return "LM - MYLEENE KISS";
            else if (id == Id.lm_nine.GetHashCode()) return "LM - NINE BY SAVANNAH MILLER";
            else if (id == Id.lm_mintie.GetHashCode()) return "LM - MINTIE";
            else if (id == Id.lm_fat_face.GetHashCode()) return "LM - FAT FACE";
            else if (id == Id.lm_laura_ashley.GetHashCode()) return "LM - LAURA ASHLEY";
            else if (id == Id.lm_mint_velvet.GetHashCode()) return "LM - MINT VELVET";
            else if (id == Id.lm_hype.GetHashCode()) return "LM - HYPE";
            else return "N/A";
        }

        public static bool isNextUK(int id)
        {
            if (id == Id.directory.GetHashCode()
                || id == Id.retail.GetHashCode()
                || id == Id.ntn_retail.GetHashCode()
                || id == Id.ntn_directory.GetHashCode()
                || id == Id.lime.GetHashCode()
                || id == Id.smithbrooks.GetHashCode()
                || id == Id.aykroyd_sons.GetHashCode()
                || id == Id.william_lamb.GetHashCode()
                || id == Id.blues_clothing.GetHashCode())
                return true;
            else
                return false;
        }

        public static bool isNextRetail(int id)
        {
            if (id == Id.retail.GetHashCode() || id == Id.ntn_retail.GetHashCode() || id == Id.lime.GetHashCode())
                return true;
            else
                return false;
        }

        public static bool isLM(int id)
        {
            if (id == Id.lm_oasis.GetHashCode() || id == Id.lm_ted_baker.GetHashCode() || id == Id.lm_label_mix.GetHashCode() || id == Id.lm_little_bird.GetHashCode() 
                || id == Id.lm_myleene_klass.GetHashCode() || id == Id.lm_nine.GetHashCode()
                || id == Id.lm_mintie.GetHashCode() || id == Id.lm_fat_face.GetHashCode() || id == Id.lm_laura_ashley.GetHashCode() || id == Id.lm_mint_velvet.GetHashCode() || id == Id.lm_hype.GetHashCode())
                return true;
            else
                return false;
        }
    }
}
