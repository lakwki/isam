using System;
using com.next.common.domain;

namespace com.next.isam.domain.ils
{
    [Serializable()]
    public class ILSErrorRef : DomainData
    {
        private int errorNo;
        private string description;

        public ILSErrorRef()
		{

		}

        public int ErrorNo 
        { 
            get { return errorNo; } 
            set { errorNo = value; } 
        }

		public string Description 
        { 
            get { return description; }
            set { description = value; } 
        }

    }
}
