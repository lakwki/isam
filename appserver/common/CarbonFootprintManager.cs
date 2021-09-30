using System;
using System.Data;
using System.Collections;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.dataserver.worker;
using com.next.isam.domain.common;
using com.next.common.datafactory.worker;
using com.next.infra.persistency.transactions;

namespace com.next.isam.appserver.common
{
    public class CarbonFootprintManager
    {
        private static CarbonFootprintManager _instance;
        private CarbonFootprintWorker carbonFootprintWorker;
        private GeneralWorker generalWorker;

        public CarbonFootprintManager()
        {
            carbonFootprintWorker = CarbonFootprintWorker.Instance;
            generalWorker = GeneralWorker.Instance;
        }

        public static CarbonFootprintManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CarbonFootprintManager();
                }
                return _instance;
            }
        }

        public DataSet getCarbonFootprintReportDataByCategory(CarbonFootprintReportDataCategory category, int officeId, int fiscalYear, int fiscalPeriod)
        {
            return carbonFootprintWorker.getCarbonFootprintReportDataByCategory(category, officeId, fiscalYear, fiscalPeriod);
        }


    }
}
