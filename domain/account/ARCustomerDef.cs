using System;
using com.next.common.domain;

namespace com.next.isam.domain.account
{
    [Serializable()]
    public class ARCustomerDef : DomainData
    {
        private int customerId;
        private string epicorCode;
        private string name;
        private string addr1;
        private string addr2;
        private string addr3;
        private string addr4;

        public int CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public string EpicorCode
        {
            get { return epicorCode; }
            set { epicorCode = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Addr1
        {
            get { return addr1; }
            set { addr1 = value; }
        }

        public string Addr2
        {
            get { return addr2; }
            set { addr2 = value; }
        }
        public string Addr3
        {
            get { return addr3; }
            set { addr3 = value; }
        }
        public string Addr4
        {
            get { return addr4; }
            set { addr4 = value; }
        }
    }
}
