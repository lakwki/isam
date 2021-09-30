using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;

namespace com.next.isam.domain.order
{
    [Serializable()]
    public class UTurnOrderParameter
    {
        private int uTurnOrderParameterId;
        private DateTime effectiveDateFrom;
        private DateTime effectiveDateTo;
        private int parameterTypeId;
        private double parameterValue;
        private int status;

        public int UTurnOrderParameterId
        {
            get { return uTurnOrderParameterId; }
            set { uTurnOrderParameterId = value; }
        }

        public DateTime EffectiveDateFrom
        {
            get { return effectiveDateFrom; }
            set { effectiveDateFrom = value; }
        }

        public DateTime EffectiveDateTo
        {
            get { return effectiveDateTo; }
            set { effectiveDateTo = value; }
        }

        public int ParameterTypeId
        {
            get { return parameterTypeId; }
            set { parameterTypeId = value; }
        }

        public double ParameterValue
        {
            get { return parameterValue; }
            set { parameterValue = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
