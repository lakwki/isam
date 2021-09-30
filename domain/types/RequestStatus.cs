using System;
using System.Collections.Generic;
using System.Text;
using com.next.common.domain;
using com.next.common.domain.types;

namespace com.next.isam.domain.types
{	

    public class RequestStatus : DomainData
    {
		private int _statusid;

        public static RequestStatus PENDING = new RequestStatus(statusid.PENDING.GetHashCode());
        public static RequestStatus CANCEL = new RequestStatus(statusid.CANCEL.GetHashCode());
        public static RequestStatus APPROVE = new RequestStatus(statusid.APPROVE.GetHashCode());
        public static RequestStatus PROCESSED = new RequestStatus(statusid.PROCESSED.GetHashCode());
        public static RequestStatus REJECTED = new RequestStatus(statusid.REJECTED.GetHashCode());

		private enum statusid 
		{
			PENDING = 1,
			APPROVE = 2,
			CANCEL	= 3,
			PROCESSED = 4,
			REJECTED = 5,
		}

        public RequestStatus(int statusid)
		{
			this._statusid = statusid;
		}
		
		public int StatusId 
		{
			get 
			{
				return this._statusid;
			}
		}

		public string StatusName
		{
			get 
			{ 				
				if (_statusid == statusid.PENDING.GetHashCode())
					return "PENDING";
				else if (_statusid == statusid.CANCEL.GetHashCode())
					return "CANCEL";
				else if (_statusid == statusid.APPROVE.GetHashCode())
					return "APPROVE";
				else if (_statusid == statusid.PROCESSED.GetHashCode())
					return "PROCESSED";
				else if (_statusid == statusid.REJECTED.GetHashCode())
					return "REJECTED";
				else
					return "";
			}
		}
    }
}
