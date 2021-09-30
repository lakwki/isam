using System.Data;
using com.next.infra.persistency.dataaccess;
using com.next.common.datafactory.worker;
using com.next.isam.domain.common;
namespace com.next.isam.dataserver.worker
{

	public class CarbonFootprintWorker : Worker
	{
        private static CarbonFootprintWorker _instance;
		private CommonWorker commonWorker;
		private GeneralWorker generalWorker;

        protected CarbonFootprintWorker()
		{
			commonWorker = CommonWorker.Instance;
			generalWorker = GeneralWorker.Instance;
		}

        public static CarbonFootprintWorker Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new CarbonFootprintWorker();
				}
				return _instance;
			}
		}

        public DataSet getCarbonFootprintReportDataByCategory(CarbonFootprintReportDataCategory category, int officeId, int fiscalYear, int fiscalPeriod)
        {
            string commandName = string.Empty;
            if (category == CarbonFootprintReportDataCategory.Electricity)
                commandName = "GetElectricityConsumption";
            else if (category == CarbonFootprintReportDataCategory.Water)
                commandName = "GetWaterConsumption";
            else if (category == CarbonFootprintReportDataCategory.CompanyPetrol)
                commandName = "GetCompanyPetrolMileage";
            else if (category == CarbonFootprintReportDataCategory.CompanyDiesel)
                commandName = "GetCompanyDieselMileage";
            else if (category == CarbonFootprintReportDataCategory.CarRental)
                commandName = "GetCarRental";
            else if (category == CarbonFootprintReportDataCategory.AirTravelLong)
                commandName = "GetAirTravelLong";
            else if (category == CarbonFootprintReportDataCategory.AirTravelShort)
                commandName = "GetAirTravelShort";
            else if (category == CarbonFootprintReportDataCategory.Bus)
                commandName = "GetBus";
            else if (category == CarbonFootprintReportDataCategory.Taxi)
                commandName = "GetTaxi";
            else if (category == CarbonFootprintReportDataCategory.Train)
                commandName = "GetTrain";

            IDataSetAdapter ad = getDataSetAdapter("CFNonTradeApt", commandName);

            ad.SelectCommand.Parameters["@FiscalYear"].Value = fiscalYear;
            ad.SelectCommand.Parameters["@FiscalPeriod"].Value = fiscalPeriod;
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId;

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            return dataSet;
        }

	}
}
