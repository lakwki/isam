using System;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class StandardMSCourierChargeDef : DomainData
    {
        private int chargeId;
        private int officeId;
        private int deptId;
        private int currencyId;
        private decimal chargeRate;

        public StandardMSCourierChargeDef() 
        { 

        }

        public int ChargeId 
        { 
            get { return chargeId; }
            set { chargeId = value; } 
        }

        public int OfficeId 
        { 
            get { return officeId; }
            set { officeId = value; } 
        }

        public int DeptId
        { 
            get { return deptId; } 
            set { deptId = value; } 
        }

        public int CurrencyId
        { 
            get { return currencyId; }
            set { currencyId = value; } 
        }

        public decimal ChargeRate
        { 
            get { return chargeRate; }
            set { chargeRate = value; } 
        }

    }
}
