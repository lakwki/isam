using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;


namespace com.next.isam.domain.common
{
    public class SystemParameterRef
    {
        public SystemParameterRef()
        {
        }

        public int ParameterId { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string Remark { get; set; }

    }
}
