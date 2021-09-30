using System;
using System.Collections ;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.types
{
	[Serializable()]
	public class DocumentType : DomainData
	{
        public static DocumentType EXPORT_LICENCE = new DocumentType(Code.ExportLicence);
        public static DocumentType CERTIFICATE_OF_ORIGIN = new DocumentType(Code.CertificateOfOrigin);
        public static DocumentType OTHER = new DocumentType(Code.Other );
        public static DocumentType GSP_FORM = new DocumentType(Code.GSPForm);
        public static DocumentType JOINT_LICENCE = new DocumentType(Code.JL);


		private Code _code;
		
		private enum Code 
		{
			ExportLicence = 1,
			CertificateOfOrigin = 2,
            Other = 3,
			GSPForm = 4,
            JL = 5
		}

        private DocumentType(Code code)
		{
			this._code = code;
		}

		public int Id
		{
			get 
			{
				return Convert.ToUInt16(_code.GetHashCode());
			}
		}

        public string ShortName
        {
            get
            {
                switch (_code)
                {
                    case Code.ExportLicence:
                        return "EL";
                    case Code.CertificateOfOrigin:
                        return "CO";
                    case Code.Other :
                        return "OTHER";
                    case Code.GSPForm:
                        return "GSP Form";
                    case Code.JL :
                        return "JL";
                    default:
                        return "ERROR";
                }
            }
        }

        public string Name 
		{
			get 
			{ 
				switch (_code)
				{
					case Code.ExportLicence :
						return "Export Licence";
					case Code.CertificateOfOrigin :
						return "Certificate Of Origin";
                    case Code.Other :
                        return "Other";
					case Code.GSPForm :
						return "GSP Form";
                    case Code.JL :
                        return "Joint Licence";
					default:
						return "ERROR";
				}				
			}
		}

        public static DocumentType getType(int id) 
		{
			if (id == Code.ExportLicence.GetHashCode()) return DocumentType.EXPORT_LICENCE;
			else if (id == Code.CertificateOfOrigin.GetHashCode()) return DocumentType.CERTIFICATE_OF_ORIGIN;
			else if (id == Code.GSPForm.GetHashCode()) return DocumentType.GSP_FORM;
            else if (id == Code.Other.GetHashCode()) return DocumentType.OTHER ;
            else if (id == Code.JL.GetHashCode()) return DocumentType.JOINT_LICENCE;
			else return null;
		}

        public static ICollection getDocumentTypeList()
        {
            ArrayList list = new ArrayList();
            list.Add(CERTIFICATE_OF_ORIGIN);
            list.Add(EXPORT_LICENCE);
            list.Add(OTHER);
            list.Add(GSP_FORM);
            list.Add(JOINT_LICENCE);

            return list;
        }
	}
}
