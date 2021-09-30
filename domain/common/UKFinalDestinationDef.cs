using System;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class UKFinalDestinationDef : DomainData
    {
        private int ukFinalDestinationId;
        private string ukFinalDestinationCode;
        private int ukFinalDestinationCountryId;
        private string ukFinalDestinationDesc;


        public UKFinalDestinationDef() 
        { 

        }

        public int UKFinalDestinationId
        {
            get { return ukFinalDestinationId; }
            set { ukFinalDestinationId = value; }
        }

        public string UKFinalDestinationCode
        {
            get { return ukFinalDestinationCode; }
            set { ukFinalDestinationCode = value; }
        }

        public int UKFinalDestinationCountryId
        {
            get { return ukFinalDestinationCountryId; }
            set { ukFinalDestinationCountryId = value; }
        }

        public string UKFinalDestinationDesc
        {
            get { return ukFinalDestinationDesc; }
            set { ukFinalDestinationDesc = value; }
        }
    }
}
