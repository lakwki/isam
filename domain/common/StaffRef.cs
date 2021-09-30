using System;
using com.next.common.domain;

namespace com.next.isam.domain.common
{
    [Serializable()]
    public class StaffRef : DomainData
    {
        private int staffId;
        private string name = "" ;
        private string email = "";
        private string phone = "";
        private string extension = "";
        private string title = "";
        private string department = "";
        private string company = "";
        private string office = "";
        private int status = 1;

        public StaffRef()
        {
        }

        public int StaffId
        {
            get { return staffId; }
            set { staffId = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        public string Office
        {
            get { return office; }
            set { office = value; }
        }

        public string Company
        {
            get { return company; }
            set { company = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
