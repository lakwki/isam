using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.next.common.domain;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.domain.types;

namespace com.next.isam.domain.nontrade
{
    [Serializable()]
    public partial class NTRoleRef : DomainData
    {
        public NTRoleRef()
        {
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}


