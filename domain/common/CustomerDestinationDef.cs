using System;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class CustomerDestinationDef : DomainData
    {

        public enum Id
        {
            uk = 1,
            dubai = 2,
            tokyo = 3,
            shanghai = 4,
            beijing = 5,
            russia = 6,
            cyprus = 7,
            lipsy_wh1 = 8,
            lipsy_wh3 = 9,
            lipsy_wh4 = 10,
            lipsy_wh5 = 11,
            lipsy_wh10 = 12,
            lipsy_b1 = 29,
            lipsy_f5 = 30,
            lipsy_f6 = 31,
            auckland = 13,
            hk = 14,
            nz = 15,
            b1 = 16,
            ca = 17,
            b2 = 18,
            w2 = 19,
            shanghai_hempel = 20,
            uk_sb = 21,
            dubai_sb = 22,
            japan_sb = 23,
            shanghai_sb = 24,
            beijing_sb = 25,
            russia_sb = 26,
            cyprus_sb = 27,
            singapore = 999,
            sbm = 28,
            cambodia = 32,
            uk_fuk_retail = 33,
            uk_fuk_directory = 34,
            dubai_fuk = 35,
            japan_fuk = 36,
            shanghai_fuk = 37,
            beijing_fuk = 38,
            russia_fuk = 39,
            cyprus_fuk = 40,
            indonesia_hempel = 41,
            japan_hempel = 42,
            uk_retail_gl = 43,
            uk_directory_gl = 44,
            dubai_gl = 45,
            japan_gl = 46,
            russia_gl = 47,
            cyprus_gl = 48,
            uk_retail_sb = 49,
            uk_directory_sb = 50,
            uk_retail_misirli = 51,
            uk_directory_misirli = 52,
            cyprus_misirli = 53,
            dubai_misirli = 54,
            japan_misirli = 55,
            russia_misirli = 56,
            uk_retail_bi = 57,
            uk_directory_bi = 58,
            cyprus_bi = 59,
            dubai_bi = 60,
            japan_bi = 61,
            russia_bi = 62,
            uk_retail_as = 63,
            uk_directory_as = 64,
            cyprus_as = 65,
            dubai_as = 66,
            japan_as = 67,
            russia_as = 68,
            uk_directory = 69,
            china_shanghai = 70,
            uk_retail_blues = 71,
            uk_directory_blues = 72,
            dubai_blues = 73,
            japan_blues = 74,
            russia_blues = 75,
            cyprus_blues = 76,
            uk_retail_william = 77,
            uk_directory_william = 78,
            dubai_william = 79,
            japan_william = 80,
            russia_william = 81,
            cyprus_william = 82,
            uk_retail_fabric = 84,
            uk_directory_fabric = 85,
            dubai_fabric = 86,
            japan_fabric = 87,
            russia_fabric = 88,
            cyprus_fabric = 89,
            shanghai_fabric = 90,
            uk_retail_tvm = 91,
            uk_directory_tvm = 92,
            uk_retail_arnold = 93,
            uk_directory_arnold = 94,
            uk_retail_bioworld = 95,
            uk_directory_bioworld = 96,
            uk_retail_paul_dennicci = 97,
            uk_directory_paul_dennicci = 98,
            uk_retail_difuzed = 99,
            uk_directory_difuzed = 100,
            lm_oasis_dir = 101,
            lm_ted_baker_dir = 102,
            lm_oasis_ret = 105,
            lm_ted_baker_ret = 106,
            uk_retail_brand_design = 107,
            uk_directory_brand_design = 108,
            nsled_dir = 109,
            nsled_ret = 110,
            poetic_dir = 111,
            poetic_ret = 112,
            forever_new_australia = 119,
            forever_new_new_zealand = 120,
            forever_new_south_africa = 121,
            forever_new_india = 122,
            forever_new_canada = 123,
            forever_new_uk = 124,
            forever_new_singapore = 125,
            forever_new_us = 126,
            forever_new_germany = 127,
            uk_retail_cortina = 128,
            uk_directory_cortina = 129,
            uk_retail_sicem = 130,
            uk_directory_sicem = 131,
            uk_retail_cooneen = 132,
            uk_directory_cooneen = 133,
            uk_retail_label_mix = 134,
            uk_directory_label_mix = 135,
            uk_retail_little_bird = 136,
            uk_directory_little_bird = 137,
            uk_retail_myleene_klass = 138,
            uk_directory_myleene_klass = 139,
            lm_ted_baker_wholesales = 140,
            lm_ted_baker_alshaya = 141,
            lm_nine_ret = 142,
            lm_nine_dir = 143,
            lm_mintie_ret = 144,
            lm_mintie_dir = 145,
            lm_fat_face_ret = 146,
            lm_fat_face_dir = 147,
            dubai_tvm = 148,
            ntn_retail_uk = 149,
            ntn_retail_dubai = 150,
            ntn_directory_uk = 151,
            ntn_directory_dubai = 152,
            lm_laura_ashley_ret = 153,
            lm_laura_ashley_dir = 154,
            lm_mint_velvet_ret = 155,
            lm_mint_velvet_dir = 156,
            lm_hype_ret = 157,
            lm_hype_dir = 158,
            dubai_cooneen = 159

        }

        public static bool isUTurnOrder(int type)
        {
            if (type == Id.shanghai.GetHashCode()) return true;
            else if (type == Id.beijing.GetHashCode()) return true;
            else if (type == Id.shanghai_hempel.GetHashCode()) return true;
            else if (type == Id.shanghai_sb.GetHashCode()) return true;
            else if (type == Id.beijing_sb.GetHashCode()) return true;
            else if (type == Id.sbm.GetHashCode()) return true;
            else if (type == Id.china_shanghai.GetHashCode()) return true;
            else return false;
        }

        public static bool isDFOrder(int type)
        {
            if (type == Id.dubai.GetHashCode()) return true;
            else if (type == Id.tokyo.GetHashCode()) return true;
            else if (type == Id.russia.GetHashCode()) return true;
            else if (type == Id.cyprus.GetHashCode()) return true;
            else if (type == Id.cambodia.GetHashCode()) return true;
            else return false;
        }

        private int customerDestinationId;
        private int customerId;
        private string destinationCode;
        private string destinationDesc;
        private string consignee;
        private string deliveryTo;
        private int uTurnOrder;
        private string franchisePartnercode;
        private string udf_DestinationCode;

        public CustomerDestinationDef() 
        { 

        }

        public int CustomerDestinationId 
        { 
            get { return customerDestinationId; } 
            set { customerDestinationId = value; } 
        }

        public int CustomerId 
        { 
            get { return customerId; } 
            set { customerId = value; } 
        }

        public string DestinationCode 
        { 
            get { return destinationCode; } 
            set { destinationCode = value; } 
        }

        public string DestinationDesc 
        { 
            get { return destinationDesc; } 
            set { destinationDesc = value; } 
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

        public int UTurnOrder 
        { 
            get { return uTurnOrder; } 
            set { uTurnOrder = value; } 
        }

        public string FranchisePartnercode 
        { 
            get { return franchisePartnercode; } 
            set { franchisePartnercode = value; } 
        }

        public string UDF_DestinationCode
        {
            get { return udf_DestinationCode; }
            set { udf_DestinationCode = value; }
        }

        public static string getDescription(int id)
        {
            if (id == Id.uk.GetHashCode()) return "UNITED KINGDOM";
            else if (id == Id.dubai.GetHashCode()) return "DUBAI";
            else if (id == Id.tokyo.GetHashCode()) return "TOKYO";
            else if (id == Id.shanghai.GetHashCode()) return "CHINA-SHANGHAI";
            else if (id == Id.beijing.GetHashCode()) return "CHINA-BEIJING";
            else if (id == Id.sbm.GetHashCode()) return "CHINA-SBM";
            else if (id == Id.russia.GetHashCode()) return "RUSSIA";
            else if (id == Id.cyprus.GetHashCode()) return "CYPRUS";
            else if (id == Id.lipsy_wh1.GetHashCode()) return "LIPSY WAREHOUSE";
            else if (id == Id.lipsy_wh3.GetHashCode()) return "LIPSY CONCESSIONS";
            else if (id == Id.lipsy_wh4.GetHashCode()) return "LIPSY INTERNET";
            else if (id == Id.lipsy_wh5.GetHashCode()) return "LIPSY EUROPEAN SALES";
            else if (id == Id.lipsy_wh10.GetHashCode()) return "LIPSY RETAIL";
            else if (id == Id.lipsy_b1.GetHashCode()) return "USA WAREHOUSE - UK";
            else if (id == Id.lipsy_f5.GetHashCode()) return "AL HOKAIR-FRANCHISE WEAREHOUSE";
            else if (id == Id.lipsy_f6.GetHashCode()) return "LMI FRANCHISE WAREHOUSE";
            else if (id == Id.auckland.GetHashCode()) return "AUCKLAND";
            else if (id == Id.hk.GetHashCode()) return "HONG KONG";
            else if (id == Id.nz.GetHashCode()) return "PALMERSTON NORTH";
            else if (id == Id.b1.GetHashCode()) return "B1 USA WAREHOUSE";
            else if (id == Id.ca.GetHashCode()) return "C1 DOUBLE-J (CANADA)";
            else if (id == Id.b2.GetHashCode()) return "B2 USA WAREHOUSE";
            else if (id == Id.w2.GetHashCode()) return "WS WORK ROOM ON 5th WAREHOUSE";
            else if (id == Id.shanghai_hempel.GetHashCode()) return "CHINA-SHANGHAI (HEMPEL)";
            else if (id == Id.uk_sb.GetHashCode()) return "UNITED KINGDOM (S&B)";
            else if (id == Id.dubai_sb.GetHashCode()) return "DUBAI (S&B)";
            else if (id == Id.japan_sb.GetHashCode()) return "JAPAN (S&B)";
            else if (id == Id.shanghai_sb.GetHashCode()) return "SHANGHAI (S&B)";
            else if (id == Id.beijing_sb.GetHashCode()) return "BEIJING (S&B)";
            else if (id == Id.russia_sb.GetHashCode()) return "RUSSIA (S&B)";
            else if (id == Id.cyprus_sb.GetHashCode()) return "CYPRUS (S&B)";
            else if (id == Id.cambodia.GetHashCode()) return "CAMBODIA";
            else if (id == Id.uk_fuk_retail.GetHashCode()) return "UNITED KINGDOM - RETAIL (FUK)";
            else if (id == Id.uk_fuk_directory.GetHashCode()) return "UNITED KINGDOM - DIRECTORY (FUK)";
            else if (id == Id.dubai_fuk.GetHashCode()) return "DUBAI (FUK)";
            else if (id == Id.japan_fuk.GetHashCode()) return "JAPAN (FUK)";
            else if (id == Id.shanghai_fuk.GetHashCode()) return "SHANGHAI (FUK)";
            else if (id == Id.beijing_fuk.GetHashCode()) return "BEIJING (FUK)";
            else if (id == Id.russia_fuk.GetHashCode()) return "RUSSIA (FUK)";
            else if (id == Id.cyprus_fuk.GetHashCode()) return "CYPRUS (FUK)";
            else if (id == Id.indonesia_hempel.GetHashCode()) return "INDONESIA (HEMPEL)";
            else if (id == Id.japan_hempel.GetHashCode()) return "JAPAN (HEMPEL)";
            else if (id == Id.uk_retail_gl.GetHashCode()) return "UK-RETAIL (GL)";
            else if (id == Id.uk_directory_gl.GetHashCode()) return "UK-DIRECTORY (GL)";
            else if (id == Id.dubai_gl.GetHashCode()) return "DUBAI (GL)";
            else if (id == Id.japan_gl.GetHashCode()) return "JAPAN (GL)";
            else if (id == Id.russia_gl.GetHashCode()) return "RUSSIA (GL)";
            else if (id == Id.cyprus_gl.GetHashCode()) return "CYRUS (GL)";
            else if (id == Id.uk_retail_sb.GetHashCode()) return "UK-RETAIL (S&B)";
            else if (id == Id.uk_directory_sb.GetHashCode()) return "UK-DIRECTORY (S&B)";
            else if (id == Id.uk_retail_misirli.GetHashCode()) return "UK-RETAIL (MISIRLI)";
            else if (id == Id.uk_directory_misirli.GetHashCode()) return "UK-DIRECTORY (MISIRLI)";
            else if (id == Id.cyprus_misirli.GetHashCode()) return "CYPRUS (MISIRLI)";
            else if (id == Id.dubai_misirli.GetHashCode()) return "DUBAI (MISIRLI)";
            else if (id == Id.japan_misirli.GetHashCode()) return "JAPAN (MISIRLI)";
            else if (id == Id.russia_misirli.GetHashCode()) return "RUSSIA (MISIRLI)";
            else if (id == Id.uk_retail_bi.GetHashCode()) return "UK-RETAIL (BI)";
            else if (id == Id.uk_directory_bi.GetHashCode()) return "UK-DIRECTORY (BI)";
            else if (id == Id.cyprus_bi.GetHashCode()) return "CYPRUS (BI)";
            else if (id == Id.dubai_bi.GetHashCode()) return "DUBAI (BI)";
            else if (id == Id.japan_bi.GetHashCode()) return "JAPAN (BI)";
            else if (id == Id.russia_bi.GetHashCode()) return "RUSSIA (BI)";
            else if (id == Id.uk_retail_as.GetHashCode()) return "UK-RETAIL (AS)";
            else if (id == Id.uk_directory_as.GetHashCode()) return "UK-DIRECTORY (AS)";
            else if (id == Id.cyprus_as.GetHashCode()) return "CYPRUS (AS)";
            else if (id == Id.dubai_as.GetHashCode()) return "DUBAI (AS)";
            else if (id == Id.japan_as.GetHashCode()) return "JAPAN (AS)";
            else if (id == Id.russia_as.GetHashCode()) return "RUSSIA (AS)";
            else if (id == Id.uk_directory.GetHashCode()) return "UNITED KINGDOM (DIRECTORY)";
            else if (id == Id.china_shanghai.GetHashCode()) return "CHINA SHANGHAI";
            else if (id == Id.uk_retail_blues.GetHashCode()) return "UK-RETAIL (BLUES)";
            else if (id == Id.uk_directory_blues.GetHashCode()) return "UK-DIRECTORY (BLUES)";
            else if (id == Id.dubai_blues.GetHashCode()) return "DUBAI (BLUES)";
            else if (id == Id.japan_blues.GetHashCode()) return "JAPAN (BLUES)";
            else if (id == Id.russia_blues.GetHashCode()) return "RUSSIA (BLUES)";
            else if (id == Id.cyprus_blues.GetHashCode()) return "CYPRUS (BLUES)";
            else if (id == Id.uk_retail_william.GetHashCode()) return "UK-RETAIL (WILLIAM)";
            else if (id == Id.uk_directory_william.GetHashCode()) return "UK-DIRECTORY (WILLIAM)";
            else if (id == Id.dubai_william.GetHashCode()) return "DUBAI (WILLIAM)";
            else if (id == Id.japan_william.GetHashCode()) return "JAPAN (WILLIAM)";
            else if (id == Id.russia_william.GetHashCode()) return "RUSSIA (WILLIAM)";
            else if (id == Id.cyprus_william.GetHashCode()) return "CYPRUS (WILLIAM)";
            else if (id == Id.uk_retail_fabric.GetHashCode()) return "UK-RETAIL (FABRIC)";
            else if (id == Id.uk_directory_fabric.GetHashCode()) return "UK-DIRECTORY (FABRIC)";
            else if (id == Id.dubai_fabric.GetHashCode()) return "DUBAI (FABRIC)";
            else if (id == Id.japan_fabric.GetHashCode()) return "JAPAN (FABRIC)";
            else if (id == Id.russia_fabric.GetHashCode()) return "RUSSIA (FABRIC)";
            else if (id == Id.cyprus_fabric.GetHashCode()) return "CYPRUS (FABRIC)";
            else if (id == Id.shanghai_fabric.GetHashCode()) return "CHINA SHANGHAI (FABRIC)";
            else if (id == Id.uk_retail_tvm.GetHashCode()) return "UK-RETAIL (TVM)";
            else if (id == Id.uk_directory_tvm.GetHashCode()) return "UK-DIRECTORY (TVM)";
            else if (id == Id.uk_retail_arnold.GetHashCode()) return "UK-RETAIL (ARNOLD)";
            else if (id == Id.uk_directory_arnold.GetHashCode()) return "UK-DIRECTORY (ARNOLD)";
            else if (id == Id.uk_retail_bioworld.GetHashCode()) return "UK-RETAIL (BIOWORLD)";
            else if (id == Id.uk_directory_bioworld.GetHashCode()) return "UK-DIRECTORY (BIOWORLD)";
            else if (id == Id.uk_retail_paul_dennicci.GetHashCode()) return "UK-RETAIL (PAUL DENNICCI)";
            else if (id == Id.uk_directory_paul_dennicci.GetHashCode()) return "UK-DIRECTORY (PAUL DENNICCI)";
            else if (id == Id.uk_retail_difuzed.GetHashCode()) return "UK-RETAIL (DIFUZED)";
            else if (id == Id.uk_directory_difuzed.GetHashCode()) return "UK-DIRECTORY (DIFUZED)";
            else if (id == Id.lm_oasis_dir.GetHashCode()) return "LM-OASIS (DIR)";
            else if (id == Id.lm_ted_baker_dir.GetHashCode()) return "LM-TED BAKER (DIR)";
            else if (id == Id.lm_oasis_ret.GetHashCode()) return "LM-OASIS (RET)";
            else if (id == Id.lm_ted_baker_ret.GetHashCode()) return "LM-TED BAKER (RET)";
            else if (id == Id.uk_retail_brand_design.GetHashCode()) return "UK-RETAIL (BRAND DESIGN)";
            else if (id == Id.uk_directory_brand_design.GetHashCode()) return "UK-DIRECTORY (BRAND DESIGN)";

            else if (id == Id.nsled_dir.GetHashCode()) return "NS-LED (DIR)";
            else if (id == Id.nsled_ret.GetHashCode()) return "NS-LED (RET)";
            else if (id == Id.poetic_dir.GetHashCode()) return "POETIC BRANDS (DIR)";
            else if (id == Id.poetic_ret.GetHashCode()) return "POETIC BRANDS (RET)";

            else if (id == Id.forever_new_australia.GetHashCode()) return "FOREVER NEW (AUSTRALIA)";
            else if (id == Id.forever_new_new_zealand.GetHashCode()) return "FOREVER NEW (NEW ZEALAND)";
            else if (id == Id.forever_new_south_africa.GetHashCode()) return "FOREVER NEW (SOUTH AFRICA)";
            else if (id == Id.forever_new_india.GetHashCode()) return "FOREVER NEW (INDIA)";
            else if (id == Id.forever_new_canada.GetHashCode()) return "FOREVER NEW (CANADA)";
            else if (id == Id.forever_new_uk.GetHashCode()) return "FOREVER NEW (UK)";
            else if (id == Id.forever_new_singapore.GetHashCode()) return "FOREVER NEW (SINGAPORE)";
            else if (id == Id.forever_new_us.GetHashCode()) return "FOREVER NEW (US)";
            else if (id == Id.forever_new_germany.GetHashCode()) return "FOREVER NEW (GERMANY)";

            else if (id == Id.uk_retail_cortina.GetHashCode()) return "UK-RETAIL (DIFUZED)";
            else if (id == Id.uk_directory_cortina.GetHashCode()) return "UK-DIRECTORY (DIFUZED)";

            else if (id == Id.uk_retail_sicem.GetHashCode()) return "UK-RETAIL (SICEM)";
            else if (id == Id.uk_directory_sicem.GetHashCode()) return "UK-DIRECTORY (SICEM)";
            else if (id == Id.uk_retail_cooneen.GetHashCode()) return "UK-RETAIL (COONEEN)";
            else if (id == Id.uk_directory_cooneen.GetHashCode()) return "UK-DIRECTORY (COONEEN)";
            else if (id == Id.dubai_cooneen.GetHashCode()) return "DUBAI (COONEEN)";

            else if (id == Id.uk_retail_label_mix.GetHashCode()) return "UK-RETAIL (LABEL MIX)";
            else if (id == Id.uk_directory_label_mix.GetHashCode()) return "UK-DIRECTORY (LABEL MIX)";
            else if (id == Id.uk_retail_little_bird.GetHashCode()) return "UK-RETAIL (LITTLE BIRD)";
            else if (id == Id.uk_directory_little_bird.GetHashCode()) return "UK-DIRECTORY (LITTLE BIRD)";
            else if (id == Id.uk_retail_myleene_klass.GetHashCode()) return "UK-RETAIL (MYLEENE KISS)";
            else if (id == Id.uk_directory_myleene_klass.GetHashCode()) return "UK-DIRECTORY (MYLEENE KISS)";
            else if (id == Id.lm_ted_baker_wholesales.GetHashCode()) return "LM-TED BAKER (WHOLESALES)";
            else if (id == Id.lm_ted_baker_alshaya.GetHashCode()) return "LM-TED BAKER (ALSHAYA)";
            else if (id == Id.lm_nine_ret.GetHashCode()) return "LM-NINE BY SAVANNAH MILLER (RET)";
            else if (id == Id.lm_nine_dir.GetHashCode()) return "LM-NINE BY SAVANNAH MILLER (DIR)";
            else if (id == Id.lm_mintie_ret.GetHashCode()) return "LM-MINTIE (RET)";
            else if (id == Id.lm_mintie_dir.GetHashCode()) return "LM-MINTIE (DIR)";
            else if (id == Id.lm_fat_face_ret.GetHashCode()) return "LM-FAT FACE (RET)";
            else if (id == Id.lm_fat_face_dir.GetHashCode()) return "LM-FAT FACE (DIR)";

            else if (id == Id.dubai_tvm.GetHashCode()) return "DUBAI (TVM FASHION)";
            else if (id == Id.ntn_retail_uk.GetHashCode()) return "UK-RETAIL (NTN)";
            else if (id == Id.ntn_retail_dubai.GetHashCode()) return "DUBAI (NTN RETAIL)";
            else if (id == Id.ntn_directory_uk.GetHashCode()) return "UK-DIRECTORY (NTN)";
            else if (id == Id.ntn_directory_dubai.GetHashCode()) return "DUBAI (NTN DIRECTORY)";

            else if (id == Id.lm_laura_ashley_ret.GetHashCode()) return "LM-LAURA ASHLEY (RET)";
            else if (id == Id.lm_laura_ashley_dir.GetHashCode()) return "LM-LAURA ASHLEY (DIR)";
            else if (id == Id.lm_mint_velvet_ret.GetHashCode()) return "LM-MINT VELVET (RET)";
            else if (id == Id.lm_mint_velvet_dir.GetHashCode()) return "LM-MINT VELVET (DIR)";
            else if (id == Id.lm_hype_ret.GetHashCode()) return "LM-HYPE (RET)";
            else if (id == Id.lm_hype_dir.GetHashCode()) return "LM-HYPE (DIR)";

            else return "N/A";
        }

        public static TypeCollector getCollection()
        {
            TypeCollector values = TypeCollector.Inclusive;
            values.append(Id.beijing.GetHashCode());
            values.append(Id.shanghai.GetHashCode());
            values.append(Id.sbm.GetHashCode());

            values.append(Id.uk.GetHashCode());
            values.append(Id.dubai.GetHashCode());
            values.append(Id.tokyo.GetHashCode());
            values.append(Id.russia.GetHashCode());
            values.append(Id.cyprus.GetHashCode());
            values.append(Id.auckland.GetHashCode());
            values.append(Id.cambodia.GetHashCode());
            values.append(Id.hk.GetHashCode());
            values.append(Id.nz.GetHashCode());
            values.append(Id.b1.GetHashCode());
            values.append(Id.b2.GetHashCode());
            values.append(Id.ca.GetHashCode());
            values.append(Id.w2.GetHashCode());

            values.append(Id.singapore.GetHashCode());

            values.append(Id.lipsy_wh1.GetHashCode());
            values.append(Id.lipsy_wh3.GetHashCode());
            values.append(Id.lipsy_wh4.GetHashCode());
            values.append(Id.lipsy_wh5.GetHashCode());
            values.append(Id.lipsy_wh10.GetHashCode());
            values.append(Id.lipsy_b1.GetHashCode());
            values.append(Id.lipsy_f5.GetHashCode());
            values.append(Id.lipsy_f6.GetHashCode());

            values.append(Id.shanghai_hempel.GetHashCode());

            values.append(Id.uk_sb.GetHashCode());
            values.append(Id.dubai_sb.GetHashCode());
            values.append(Id.japan_sb.GetHashCode());
            values.append(Id.shanghai_sb.GetHashCode());
            values.append(Id.beijing_sb.GetHashCode());
            values.append(Id.russia_sb.GetHashCode());
            values.append(Id.cyprus_sb.GetHashCode());

            values.append(Id.uk_fuk_retail.GetHashCode());
            values.append(Id.uk_fuk_directory.GetHashCode());
            values.append(Id.dubai_fuk.GetHashCode());
            values.append(Id.japan_fuk.GetHashCode());

            values.append(Id.shanghai_fuk.GetHashCode());
            /*
            values.append(Id.beijing_fuk.GetHashCode());
            */
            values.append(Id.russia_fuk.GetHashCode());
            values.append(Id.cyprus_fuk.GetHashCode());

            values.append(Id.indonesia_hempel.GetHashCode());
            values.append(Id.japan_hempel.GetHashCode());

            values.append(Id.uk_retail_gl.GetHashCode());
            values.append(Id.uk_directory_gl.GetHashCode());
            values.append(Id.dubai_gl.GetHashCode());
            values.append(Id.japan_gl.GetHashCode());
            values.append(Id.russia_gl.GetHashCode());
            values.append(Id.cyprus_gl.GetHashCode());

            values.append(Id.uk_retail_sb.GetHashCode());
            values.append(Id.uk_directory_sb.GetHashCode());

            values.append(Id.uk_retail_misirli.GetHashCode());
            values.append(Id.uk_directory_misirli.GetHashCode());
            values.append(Id.cyprus_misirli.GetHashCode());
            values.append(Id.dubai_misirli.GetHashCode()); 
            values.append(Id.japan_misirli.GetHashCode());
            values.append(Id.russia_misirli.GetHashCode());

            values.append(Id.uk_retail_bi.GetHashCode());
            values.append(Id.uk_directory_bi.GetHashCode());
            values.append(Id.cyprus_bi.GetHashCode());
            values.append(Id.dubai_bi.GetHashCode());
            values.append(Id.japan_bi.GetHashCode());
            values.append(Id.russia_bi.GetHashCode());

            values.append(Id.uk_retail_as.GetHashCode());
            values.append(Id.uk_directory_as.GetHashCode());
            values.append(Id.cyprus_as.GetHashCode());
            values.append(Id.dubai_as.GetHashCode());
            values.append(Id.japan_as.GetHashCode());
            values.append(Id.russia_as.GetHashCode());

            values.append(Id.uk_directory.GetHashCode());
            values.append(Id.china_shanghai.GetHashCode());

            values.append(Id.uk_retail_blues.GetHashCode());
            values.append(Id.uk_directory_blues.GetHashCode());
            values.append(Id.dubai_blues.GetHashCode());
            values.append(Id.japan_blues.GetHashCode());
            values.append(Id.russia_blues.GetHashCode());
            values.append(Id.cyprus_blues.GetHashCode());

            values.append(Id.uk_retail_william.GetHashCode());
            values.append(Id.uk_directory_william.GetHashCode());
            values.append(Id.dubai_william.GetHashCode());
            values.append(Id.japan_william.GetHashCode());
            values.append(Id.russia_william.GetHashCode());
            values.append(Id.cyprus_william.GetHashCode());

            values.append(Id.uk_retail_fabric.GetHashCode());
            values.append(Id.uk_directory_fabric.GetHashCode());
            values.append(Id.dubai_fabric.GetHashCode());
            values.append(Id.japan_fabric.GetHashCode());
            values.append(Id.russia_fabric.GetHashCode());
            values.append(Id.cyprus_fabric.GetHashCode());
            values.append(Id.shanghai_fabric.GetHashCode());
            values.append(Id.uk_retail_tvm.GetHashCode());
            values.append(Id.uk_directory_tvm.GetHashCode());
            values.append(Id.uk_retail_arnold.GetHashCode());
            values.append(Id.uk_directory_arnold.GetHashCode());

            values.append(Id.uk_retail_bioworld.GetHashCode());
            values.append(Id.uk_directory_bioworld.GetHashCode());
            values.append(Id.uk_retail_paul_dennicci.GetHashCode());
            values.append(Id.uk_directory_paul_dennicci.GetHashCode());
            values.append(Id.uk_retail_difuzed.GetHashCode());
            values.append(Id.uk_directory_difuzed.GetHashCode());

            values.append(Id.lm_oasis_dir.GetHashCode());
            values.append(Id.lm_ted_baker_dir.GetHashCode());
            values.append(Id.lm_oasis_ret.GetHashCode());
            values.append(Id.lm_ted_baker_ret.GetHashCode());

            values.append(Id.uk_retail_brand_design.GetHashCode());
            values.append(Id.uk_directory_brand_design.GetHashCode());

            values.append(Id.nsled_dir.GetHashCode());
            values.append(Id.nsled_ret.GetHashCode());

            values.append(Id.poetic_dir.GetHashCode());
            values.append(Id.poetic_ret.GetHashCode());

            values.append(Id.forever_new_australia.GetHashCode());
            values.append(Id.forever_new_new_zealand.GetHashCode());
            values.append(Id.forever_new_south_africa.GetHashCode());
            values.append(Id.forever_new_india.GetHashCode());
            values.append(Id.forever_new_canada.GetHashCode());
            values.append(Id.forever_new_uk.GetHashCode());
            values.append(Id.forever_new_singapore.GetHashCode());
            values.append(Id.forever_new_us.GetHashCode());
            values.append(Id.forever_new_germany.GetHashCode());

            values.append(Id.uk_retail_cortina.GetHashCode());
            values.append(Id.uk_directory_cortina.GetHashCode());

            values.append(Id.uk_retail_sicem.GetHashCode());
            values.append(Id.uk_directory_sicem.GetHashCode());
            values.append(Id.uk_retail_cooneen.GetHashCode());
            values.append(Id.uk_directory_cooneen.GetHashCode());
            values.append(Id.dubai_cooneen.GetHashCode());

            values.append(Id.uk_retail_label_mix.GetHashCode());
            values.append(Id.uk_directory_label_mix.GetHashCode());
            values.append(Id.uk_retail_little_bird.GetHashCode());
            values.append(Id.uk_directory_little_bird.GetHashCode());
            values.append(Id.uk_retail_myleene_klass.GetHashCode());
            values.append(Id.uk_directory_myleene_klass.GetHashCode());

            values.append(Id.lm_ted_baker_wholesales.GetHashCode());
            values.append(Id.lm_ted_baker_alshaya.GetHashCode());
            values.append(Id.lm_nine_ret.GetHashCode());
            values.append(Id.lm_nine_dir.GetHashCode());
            values.append(Id.lm_mintie_ret.GetHashCode());
            values.append(Id.lm_mintie_dir.GetHashCode());
            values.append(Id.lm_fat_face_ret.GetHashCode());
            values.append(Id.lm_fat_face_dir.GetHashCode());
            values.append(Id.dubai_tvm.GetHashCode());
            values.append(Id.ntn_retail_uk.GetHashCode());
            values.append(Id.ntn_retail_dubai.GetHashCode());
            values.append(Id.ntn_directory_uk.GetHashCode());
            values.append(Id.ntn_directory_dubai.GetHashCode());

            values.append(Id.lm_laura_ashley_ret.GetHashCode());
            values.append(Id.lm_laura_ashley_dir.GetHashCode());
            values.append(Id.lm_mint_velvet_ret.GetHashCode());
            values.append(Id.lm_mint_velvet_dir.GetHashCode());
            values.append(Id.lm_hype_ret.GetHashCode());
            values.append(Id.lm_hype_dir.GetHashCode());

            return values;
        }

        public static TypeCollector getCollectionForChina()
        {
            TypeCollector values = TypeCollector.Inclusive;
            values.append(Id.beijing.GetHashCode());
            values.append(Id.shanghai.GetHashCode());
            values.append(Id.shanghai_hempel.GetHashCode());
            values.append(Id.shanghai_sb.GetHashCode());
            values.append(Id.beijing_sb.GetHashCode());
            values.append(Id.shanghai_fuk.GetHashCode());
            /*
            values.append(Id.beijing_fuk.GetHashCode());
            */
            values.append(Id.sbm.GetHashCode());
            values.append(Id.china_shanghai.GetHashCode());
            return values;
        }

        public static TypeCollector getCollectionForNonChina()
        {
            TypeCollector values = TypeCollector.Inclusive;

            values.append(Id.uk.GetHashCode());
            values.append(Id.dubai.GetHashCode());
            values.append(Id.tokyo.GetHashCode());
            values.append(Id.russia.GetHashCode());
            values.append(Id.cyprus.GetHashCode());
            values.append(Id.auckland.GetHashCode());
            values.append(Id.cambodia.GetHashCode());
            values.append(Id.hk.GetHashCode());
            values.append(Id.nz.GetHashCode());
            values.append(Id.b1.GetHashCode());
            values.append(Id.b2.GetHashCode());
            values.append(Id.w2.GetHashCode());
            values.append(Id.ca.GetHashCode());
            values.append(Id.singapore.GetHashCode());

            values.append(Id.lipsy_wh1.GetHashCode());
            values.append(Id.lipsy_wh3.GetHashCode());
            values.append(Id.lipsy_wh4.GetHashCode());
            values.append(Id.lipsy_wh5.GetHashCode());
            values.append(Id.lipsy_wh10.GetHashCode());
            values.append(Id.lipsy_b1.GetHashCode());
            values.append(Id.lipsy_f5.GetHashCode());
            values.append(Id.lipsy_f6.GetHashCode());

            values.append(Id.uk_sb.GetHashCode());
            values.append(Id.dubai_sb.GetHashCode());
            values.append(Id.japan_sb.GetHashCode());
            values.append(Id.russia_sb.GetHashCode());
            values.append(Id.cyprus_sb.GetHashCode());

            values.append(Id.uk_fuk_retail.GetHashCode());
            values.append(Id.uk_fuk_directory.GetHashCode());
            values.append(Id.dubai_fuk.GetHashCode());
            values.append(Id.japan_fuk.GetHashCode());
            values.append(Id.russia_fuk.GetHashCode());
            values.append(Id.cyprus_fuk.GetHashCode());

            values.append(Id.indonesia_hempel.GetHashCode());
            values.append(Id.japan_hempel.GetHashCode());

            values.append(Id.uk_retail_gl.GetHashCode());
            values.append(Id.uk_directory_gl.GetHashCode());
            values.append(Id.dubai_gl.GetHashCode());
            values.append(Id.japan_gl.GetHashCode());
            values.append(Id.russia_gl.GetHashCode());
            values.append(Id.cyprus_gl.GetHashCode());

            values.append(Id.uk_retail_sb.GetHashCode());
            values.append(Id.uk_directory_sb.GetHashCode());

            values.append(Id.uk_retail_misirli.GetHashCode());
            values.append(Id.uk_directory_misirli.GetHashCode());
            values.append(Id.cyprus_misirli.GetHashCode());
            values.append(Id.dubai_misirli.GetHashCode());
            values.append(Id.japan_misirli.GetHashCode());
            values.append(Id.russia_misirli.GetHashCode());

            values.append(Id.uk_retail_bi.GetHashCode());
            values.append(Id.uk_directory_bi.GetHashCode());
            values.append(Id.cyprus_bi.GetHashCode());
            values.append(Id.dubai_bi.GetHashCode());
            values.append(Id.japan_bi.GetHashCode());
            values.append(Id.russia_bi.GetHashCode());

            values.append(Id.uk_retail_as.GetHashCode());
            values.append(Id.uk_directory_as.GetHashCode());
            values.append(Id.cyprus_as.GetHashCode());
            values.append(Id.dubai_as.GetHashCode());
            values.append(Id.japan_as.GetHashCode());
            values.append(Id.russia_as.GetHashCode());
            values.append(Id.uk_directory.GetHashCode());

            values.append(Id.uk_retail_blues.GetHashCode());
            values.append(Id.uk_directory_blues.GetHashCode());
            values.append(Id.dubai_blues.GetHashCode());
            values.append(Id.japan_blues.GetHashCode());
            values.append(Id.russia_blues.GetHashCode());
            values.append(Id.cyprus_blues.GetHashCode());

            values.append(Id.uk_retail_william.GetHashCode());
            values.append(Id.uk_directory_william.GetHashCode());
            values.append(Id.dubai_william.GetHashCode());
            values.append(Id.japan_william.GetHashCode());
            values.append(Id.russia_william.GetHashCode());
            values.append(Id.cyprus_william.GetHashCode());

            values.append(Id.uk_retail_fabric.GetHashCode());
            values.append(Id.uk_directory_fabric.GetHashCode());
            values.append(Id.dubai_fabric.GetHashCode());
            values.append(Id.japan_fabric.GetHashCode());
            values.append(Id.russia_fabric.GetHashCode());
            values.append(Id.cyprus_fabric.GetHashCode());
            values.append(Id.shanghai_fabric.GetHashCode());

            values.append(Id.uk_retail_tvm.GetHashCode());
            values.append(Id.uk_directory_tvm.GetHashCode());
            values.append(Id.uk_retail_arnold.GetHashCode());
            values.append(Id.uk_directory_arnold.GetHashCode());
            values.append(Id.uk_retail_bioworld.GetHashCode());
            values.append(Id.uk_directory_bioworld.GetHashCode());
            values.append(Id.uk_retail_paul_dennicci.GetHashCode());
            values.append(Id.uk_directory_paul_dennicci.GetHashCode());
            values.append(Id.uk_retail_difuzed.GetHashCode());
            values.append(Id.uk_directory_difuzed.GetHashCode());

            values.append(Id.lm_oasis_dir.GetHashCode());
            values.append(Id.lm_ted_baker_dir.GetHashCode());
            values.append(Id.lm_oasis_ret.GetHashCode());
            values.append(Id.lm_ted_baker_ret.GetHashCode());

            values.append(Id.uk_retail_brand_design.GetHashCode());
            values.append(Id.uk_directory_brand_design.GetHashCode());

            values.append(Id.nsled_dir.GetHashCode());
            values.append(Id.nsled_ret.GetHashCode());

            values.append(Id.poetic_dir.GetHashCode());
            values.append(Id.poetic_ret.GetHashCode());

            values.append(Id.forever_new_australia.GetHashCode());
            values.append(Id.forever_new_new_zealand.GetHashCode());
            values.append(Id.forever_new_south_africa.GetHashCode());
            values.append(Id.forever_new_india.GetHashCode());
            values.append(Id.forever_new_canada.GetHashCode());
            values.append(Id.forever_new_uk.GetHashCode());
            values.append(Id.forever_new_singapore.GetHashCode());
            values.append(Id.forever_new_us.GetHashCode());
            values.append(Id.forever_new_germany.GetHashCode());

            values.append(Id.uk_retail_cortina.GetHashCode());
            values.append(Id.uk_directory_cortina.GetHashCode());

            values.append(Id.uk_retail_sicem.GetHashCode());
            values.append(Id.uk_directory_sicem.GetHashCode());
            values.append(Id.uk_retail_cooneen.GetHashCode());
            values.append(Id.uk_directory_cooneen.GetHashCode());
            values.append(Id.dubai_cooneen.GetHashCode());

            values.append(Id.uk_retail_label_mix.GetHashCode());
            values.append(Id.uk_directory_label_mix.GetHashCode());
            values.append(Id.uk_retail_little_bird.GetHashCode());
            values.append(Id.uk_directory_little_bird.GetHashCode());
            values.append(Id.uk_retail_myleene_klass.GetHashCode());
            values.append(Id.uk_directory_myleene_klass.GetHashCode());

            values.append(Id.lm_ted_baker_wholesales.GetHashCode());
            values.append(Id.lm_ted_baker_alshaya.GetHashCode());
            values.append(Id.lm_nine_ret.GetHashCode());
            values.append(Id.lm_nine_dir.GetHashCode());
            values.append(Id.lm_mintie_ret.GetHashCode());
            values.append(Id.lm_mintie_dir.GetHashCode());
            values.append(Id.lm_fat_face_ret.GetHashCode());
            values.append(Id.lm_fat_face_dir.GetHashCode());
            values.append(Id.dubai_tvm.GetHashCode());
            values.append(Id.ntn_retail_uk.GetHashCode());
            values.append(Id.ntn_retail_dubai.GetHashCode());
            values.append(Id.ntn_directory_uk.GetHashCode());
            values.append(Id.ntn_directory_dubai.GetHashCode());

            values.append(Id.lm_laura_ashley_ret.GetHashCode());
            values.append(Id.lm_laura_ashley_dir.GetHashCode());
            values.append(Id.lm_mint_velvet_ret.GetHashCode());
            values.append(Id.lm_mint_velvet_dir.GetHashCode());
            values.append(Id.lm_hype_ret.GetHashCode());
            values.append(Id.lm_hype_dir.GetHashCode());

            return values;
        }

        public static TypeCollector getCollectionForLipsy()
        {
            TypeCollector values = TypeCollector.Inclusive;

            values.append(Id.lipsy_wh1.GetHashCode());
            values.append(Id.lipsy_wh3.GetHashCode());
            values.append(Id.lipsy_wh4.GetHashCode());
            values.append(Id.lipsy_wh5.GetHashCode());
            values.append(Id.lipsy_wh10.GetHashCode());
            values.append(Id.lipsy_b1.GetHashCode());
            values.append(Id.lipsy_f5.GetHashCode());
            values.append(Id.lipsy_f6.GetHashCode());
            return values;
        }
    }
}
