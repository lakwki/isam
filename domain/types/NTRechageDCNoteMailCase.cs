using System;
using com.next.common.domain;
using System.Collections.Generic;
using com.next.common.domain.industry.vendor;
using com.next.common.datafactory.worker.industry;
using com.next.isam.domain.nontrade;

namespace com.next.isam.domain.types
{
	[Serializable()]
	public class NTRechageDCNoteMailCase : DomainData
	{
        public static NTRechageDCNoteMailCase GroupCentral = new NTRechageDCNoteMailCase(Type.GroupCentral);
        public static NTRechageDCNoteMailCase APSupplier = new NTRechageDCNoteMailCase(Type.APSupplier);
        public static NTRechageDCNoteMailCase Retail = new NTRechageDCNoteMailCase(Type.Retail);
        public static NTRechageDCNoteMailCase BrandFinance = new NTRechageDCNoteMailCase(Type.BrandFinance);
        public static NTRechageDCNoteMailCase NSLUK = new NTRechageDCNoteMailCase(Type.NSLUK);
        public static NTRechageDCNoteMailCase NTVendor = new NTRechageDCNoteMailCase(Type.NTVendor);

        public enum Type
        {
            GroupCentral = 1,
            APSupplier = 2,
            Retail = 3,
            BrandFinance = 4,
            NSLUK = 5,
            NTVendor = 6
        }
        private Type _type;
        public NTRechageDCNoteMailCase(Type type)
        {
            this._type = type;
        }


        private static string[] toList = {
            "Alison_Palmer@next.co.uk;",
            "",
            "NSLAccountpayables@next.co.uk;invoicestonext@next.co.uk",
            "Hannah_Pugh@next.co.uk;Lucy_Eden@next.co.uk",
            "Natalie_Yue@nextsl.com.hk;candice_ng@nextsl.com.hk",
            ""
        };
        private static string[] ccList = {
            "ivan_chong@nextsl.com.hk;Teresa_Wong@nextsl.com.hk;katalin_toth@next.co.uk",
            "",
            "",
            "",
            "",
            ""
        };

        public string ToList
        {
            get 
            {
                return toList[_type.GetHashCode() - 1];            
            }
        }
        public string CcList
        {
            get
            {
                return ccList[_type.GetHashCode() - 1];
            }
        }

        public string getFromNSLSubject(NTRechargeDCNoteDef def, string vendorName)
        {
            string subject = "";
            string rechargeDCNoteNo = def.RechargeDCNoteNo;

            if (this == NTRechageDCNoteMailCase.GroupCentral) // Case 1
            {
                subject = String.Format("NSL {0} Note to Next Group PLC - {1}", def.DCIndicatorEng, rechargeDCNoteNo);
            }
            else if (this == NTRechageDCNoteMailCase.APSupplier || this == NTRechageDCNoteMailCase.NTVendor) // Case 2, 6
            {
                subject = String.Format("NSL {0} Note to {1} - {2}", def.DCIndicatorEng, vendorName, rechargeDCNoteNo);
            }
            else if (this == NTRechageDCNoteMailCase.BrandFinance) // Case 4
            {
                subject = String.Format("NSL {0} Note to Brand Finance - {1}", def.DCIndicatorEng, rechargeDCNoteNo);
            }
            else if (this == NTRechageDCNoteMailCase.NSLUK) // Case 5
            {
                subject = String.Format("NSL {0} Note to NSLUK - {1}", def.DCIndicatorEng, rechargeDCNoteNo);
            }

            return subject;
        }

        public string getForAPSubject(NTRechargeDCNoteDef def, string supplierId = "")
        {
            string subject = "";
            if (this == NTRechageDCNoteMailCase.Retail) // Case 3
            {
                string rechargeDCNoteNo = def.RechargeDCNoteNo;
                string officeCode = def.Office.OfficeCode;
                subject = String.Format("{0} - DEBIT NOTE - {1} (Supplier ID: {2})", officeCode, rechargeDCNoteNo, supplierId);
            }
            return subject;
        }
	}
}
