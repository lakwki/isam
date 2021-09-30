using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using com.next.common.domain;

namespace com.next.isam.domain.types
{
    [Serializable()]
    public class PaymentType : DomainData
    {
        public static PaymentType FABRIC = new PaymentType(Code.fabric);
        public static PaymentType INSTALMENT = new PaymentType(Code.Instalment);

        private Code _code;

        private enum Code
        {
            fabric = 1,
            Instalment = 2
        }

        private PaymentType(Code code)
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

        public string Name
        {
            get
            {
                switch (_code)
                {
                    case Code.fabric:
                        return "Fabric Cost";
                    case Code.Instalment:
                        return "By Instalment";
                    default:
                        return "ERROR";
                }
            }
        }

        public static PaymentType getType(int id)
        {
            if (id == Code.fabric.GetHashCode()) return PaymentType.FABRIC;
            else if (id == Code.Instalment.GetHashCode()) return PaymentType.INSTALMENT;
            else return null;
        }
    }
}
