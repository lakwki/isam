using System;
using System.Collections.Generic;
using System.Text;
using com.next.isam.dataserver.worker;

namespace com.next.isam.appserver.account
{
    public class BankReconciliationManager
    {
        private static BankReconciliationManager _instance;

        private BankReconciliationWorker worker;

        public BankReconciliationManager()
        {
            worker = BankReconciliationWorker.Instance;
        }

        public static BankReconciliationManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BankReconciliationManager();
                return _instance;
            }
        }

        public void processBankReconciliation()
        {
            worker.processBankReconciliation();
        }
    }
}
