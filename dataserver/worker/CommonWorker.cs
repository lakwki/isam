using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using com.next.common.domain;
using com.next.common.datafactory.worker;
using com.next.common.web;
using com.next.infra.persistency.dataaccess;
using com.next.isam.dataserver.model.common;
using com.next.isam.dataserver.model.order;
using com.next.isam.domain.common;
using com.next.isam.domain.types;
using com.next.common.domain.types;
using com.next.common.datafactory.model.general;
using com.next.infra.persistency.transactions;


namespace com.next.isam.dataserver.worker
{
	public class CommonWorker : Worker
	{
        private static CommonWorker _instance;
		private GeneralWorker generalWorker;

		protected CommonWorker()
		{
			generalWorker = GeneralWorker.Instance;
		}

		public static CommonWorker Instance
		{
			get 
			{
                if (_instance == null)
				    _instance = new CommonWorker();
                return _instance;
			}
		}

		#region Air Freight Payment Type
		public ICollection getAirFreightPaymentTypeList()
		{
			IDataSetAdapter ad = getDataSetAdapter("AirFreightPaymentTypeApt", "GetAirFreightPaymentTypeList");

			AirFreightPaymentTypeDs dataSet = new AirFreightPaymentTypeDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(AirFreightPaymentTypeDs.AirFreightPaymentTypeRow row in dataSet.AirFreightPaymentType)
			{
				AirFreightPaymentTypeRef aRef = new AirFreightPaymentTypeRef();
				AirFreightPaymentTypeMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public AirFreightPaymentTypeRef getAirFreightPaymentTypeByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("AirFreightPaymentTypeApt", "GetAirFreightPaymentTypeByKey");
			ad.SelectCommand.Parameters["@AirFreightPaymentTypeId"].Value = key.ToString();

			AirFreightPaymentTypeDs dataSet = new AirFreightPaymentTypeDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			AirFreightPaymentTypeRef aRef = new AirFreightPaymentTypeRef();
			AirFreightPaymentTypeMapping(dataSet.AirFreightPaymentType[0], aRef);
			return aRef;
		}

		#endregion

        #region Currency
        public ICollection getNewCurrencyListForExchangeRate()
        {
            IDataSetAdapter ad = getDataSetAdapter("CurrencyForExchangeRateApt", "GetCurrencyListForExchangeRate");

            CurrencyDs dataSet = new CurrencyDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (CurrencyDs.CurrencyRow row in dataSet.Currency)
            {
                if (row.CurrencyId != CurrencyId.EUD.Id && row.CurrencyId != CurrencyId.UKP.Id)
                {
                    CurrencyRef aRef = new CurrencyRef();
                    CurrencyMapping(row, aRef);
                    list.Add(aRef);
                }
            }
            return list;

        }

        public ICollection getCurrencyListForExchangeRate()
        {
            IDataSetAdapter ad = getDataSetAdapter("CurrencyForExchangeRateApt", "GetCurrencyListForExchangeRate");

			CurrencyDs dataSet = new CurrencyDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(CurrencyDs.CurrencyRow row in dataSet.Currency)
			{
				CurrencyRef aRef = new CurrencyRef();
                CurrencyMapping(row, aRef);
				list.Add(aRef);
			}
			return list;

        }

        public ICollection getEffectiveCurrencyList()
        {
            IDataSetAdapter ad = getDataSetAdapter("CurrencyForExchangeRateApt", "GetEffectiveCurrencyList");

            CurrencyDs dataSet = new CurrencyDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (CurrencyDs.CurrencyRow row in dataSet.Currency)
            {
                CurrencyRef aRef = new CurrencyRef();
                CurrencyMapping(row, aRef);
                list.Add(aRef);
            }
            return list;
        }


        #endregion

        #region Customer Type
        public ICollection getCustomerTypeList()
		{
			IDataSetAdapter ad = getDataSetAdapter("CustomerTypeApt", "GetCustomerTypeList");

			CustomerTypeDs dataSet = new CustomerTypeDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(CustomerTypeDs.CustomerTypeRow row in dataSet.CustomerType)
			{
				CustomerTypeRef aRef = new CustomerTypeRef();
				CustomerTypeMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public CustomerTypeRef getCustomerTypeByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("CustomerTypeApt", "GetCustomerTypeByKey");
			ad.SelectCommand.Parameters["@CustomerTypeId"].Value = key.ToString();

			CustomerTypeDs dataSet = new CustomerTypeDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			CustomerTypeRef aRef = new CustomerTypeRef();
			CustomerTypeMapping(dataSet.CustomerType[0], aRef);
			return aRef;
		}

		#endregion

		#region Customer Destination
        public ICollection getCustomerDestinationList()
        {
            IDataSetAdapter ad = getDataSetAdapter("CustomerDestinationApt", "GetCustomerDestinationList");

			CustomerDestinationDs dataSet = new CustomerDestinationDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(CustomerDestinationDs.CustomerDestinationRow row in dataSet.CustomerDestination)
			{
				CustomerDestinationDef cdDef = new CustomerDestinationDef();
				CustomerDestinationMapping(row, cdDef);
				list.Add(cdDef);
			}
			return list;
        }

		public ICollection getCustomerDestinationByCustomerId(int customerId)
		{
			IDataSetAdapter ad = getDataSetAdapter("CustomerDestinationApt", "GetCustomerDestinationByCustomerId");
			ad.SelectCommand.Parameters["@CustomerId"].Value = customerId.ToString();

			CustomerDestinationDs dataSet = new CustomerDestinationDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(CustomerDestinationDs.CustomerDestinationRow row in dataSet.CustomerDestination)
			{
				CustomerDestinationDef cdDef = new CustomerDestinationDef();
				CustomerDestinationMapping(row, cdDef);
				list.Add(cdDef);
			}
			return list;
		}

		public CustomerDestinationDef getCustomerDestinationByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("CustomerDestinationApt", "GetCustomerDestinationByKey");
			ad.SelectCommand.Parameters["@CustomerDestinationId"].Value = key.ToString();

			CustomerDestinationDs dataSet = new CustomerDestinationDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			CustomerDestinationDef cdDef = new CustomerDestinationDef();
			CustomerDestinationMapping(dataSet.CustomerDestination[0], cdDef);
			return cdDef;
		}

		public CustomerDestinationDef getCustomerDestinationByShipmentId(int shipmentId)
		{
			IDataSetAdapter ad = getDataSetAdapter("CustomerDestinationApt", "GetCustomerDestinationByShipmentId");
			ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

			CustomerDestinationDs dataSet = new CustomerDestinationDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			CustomerDestinationDef cdDef = new CustomerDestinationDef();
			CustomerDestinationMapping(dataSet.CustomerDestination[0], cdDef);
			return cdDef;
		}

		#endregion

        #region UK Final Destination

        public UKFinalDestinationDef getUKFinalDestinationByCode(string destinationCode)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKFinalDestinationApt", "GetUKFinalDestinationByCode");
            ad.SelectCommand.Parameters["@DestinationCode"].Value = destinationCode;

            UKFinalDestinationDs dataSet = new UKFinalDestinationDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            UKFinalDestinationDef aRef = new UKFinalDestinationDef();
            UKFinalDestinationMapping(dataSet.UKFinalDestination[0], aRef);
            
            return aRef;
        }

        #endregion

        #region Other Cost
        public ICollection getOtherCostTypeList()
		{
			IDataSetAdapter ad = getDataSetAdapter("OtherCostTypeApt", "GetOtherCostTypeList");

			OtherCostTypeDs dataSet = new OtherCostTypeDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(OtherCostTypeDs.OtherCostTypeRow row in dataSet.OtherCostType)
			{
				OtherCostTypeRef aRef = new OtherCostTypeRef();
				OtherCostTypeMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public OtherCostTypeRef getOtherCostTypeByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("OtherCostTypeApt", "GetOtherCostTypeByKey");
			ad.SelectCommand.Parameters["@OtherCostTypeId"].Value = key.ToString();

			OtherCostTypeDs dataSet = new OtherCostTypeDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			OtherCostTypeRef aRef = new OtherCostTypeRef();
			OtherCostTypeMapping(dataSet.OtherCostType[0], aRef);
			return aRef;
		}

		#endregion

		#region Packing Method
		public ICollection getPackingMethodList()
		{
			IDataSetAdapter ad = getDataSetAdapter("PackingMethodApt", "GetPackingMethodList");

			PackingMethodDs dataSet = new PackingMethodDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(PackingMethodDs.PackingMethodRow row in dataSet.PackingMethod)
			{
				PackingMethodRef aRef = new PackingMethodRef();
				PackingMethodMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public PackingMethodRef getPackingMethodByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("PackingMethodApt", "GetPackingMethodByKey");
			ad.SelectCommand.Parameters["@PackingMethodId"].Value = key.ToString();

			PackingMethodDs dataSet = new PackingMethodDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			PackingMethodRef aRef = new PackingMethodRef();
			PackingMethodMapping(dataSet.PackingMethod[0], aRef);
			return aRef;
		}

		#endregion

		#region Packing Unit
		public ICollection getPackingUnitList()
		{
			IDataSetAdapter ad = getDataSetAdapter("PackingUnitApt", "GetPackingUnitList");

			PackingUnitDs dataSet = new PackingUnitDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(PackingUnitDs.PackingUnitRow row in dataSet.PackingUnit)
			{
				PackingUnitRef aRef = new PackingUnitRef();
				PackingUnitMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public PackingUnitRef getPackingUnitByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("PackingUnitApt", "GetPackingUnitByKey");
			ad.SelectCommand.Parameters["@PackingUnitId"].Value = key.ToString();

			PackingUnitDs dataSet = new PackingUnitDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			PackingUnitRef aRef = new PackingUnitRef();
			PackingUnitMapping(dataSet.PackingUnit[0], aRef);
			return aRef;
		}
		
		#endregion

		#region Payment Term
		
		public ICollection getPaymentTermList()
		{
			return generalWorker.getPaymentTermList();
		}

		public PaymentTermRef getPaymentTermByKey(int key)
		{
			return generalWorker.getPaymentTermByKey(key);
		}
		
		#endregion
		
		#region Purchase Location
		public ICollection getPurchaseLocationList()
		{
			IDataSetAdapter ad = getDataSetAdapter("PurchaseLocationApt", "GetPurchaseLocationList");

			PurchaseLocationDs dataSet = new PurchaseLocationDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(PurchaseLocationDs.PurchaseLocationRow row in dataSet.PurchaseLocation)
			{
				PurchaseLocationRef aRef = new PurchaseLocationRef();
				PurchaseLocationMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public PurchaseLocationRef getPurchaseLocationByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("PurchaseLocationApt", "GetPurchaseLocationByKey");
			ad.SelectCommand.Parameters["@PurchaseLocationId"].Value = key.ToString();

			PurchaseLocationDs dataSet = new PurchaseLocationDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			PurchaseLocationRef aRef = new PurchaseLocationRef();
			PurchaseLocationMapping(dataSet.PurchaseLocation[0], aRef);
			return aRef;
		}

		#endregion

		#region Shipment Method
		public ICollection getShipmentMethodList()
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentMethodApt", "GetShipmentMethodList");

			ShipmentMethodDs dataSet = new ShipmentMethodDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ShipmentMethodDs.ShipmentMethodRow row in dataSet.ShipmentMethod)
			{
				ShipmentMethodRef aRef = new ShipmentMethodRef();
				ShipmentMethodMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public ShipmentMethodRef getShipmentMethodByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentMethodApt", "GetShipmentMethodByKey");
			ad.SelectCommand.Parameters["@ShipmentMethodId"].Value = key.ToString();

			ShipmentMethodDs dataSet = new ShipmentMethodDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ShipmentMethodRef aRef = new ShipmentMethodRef();
			ShipmentMethodMapping(dataSet.ShipmentMethod[0], aRef);
			return aRef;
		}
		#endregion

		#region Shipment Country
		public ICollection getShipmentCountryList()
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentCountryApt", "GetShipmentCountryList");

			ShipmentCountryDs dataSet = new ShipmentCountryDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ShipmentCountryDs.ShipmentCountryRow row in dataSet.ShipmentCountry)
			{
				ShipmentCountryRef aRef = new ShipmentCountryRef();
				ShipmentCountryMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public ShipmentCountryRef getShipmentCountryByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentCountryApt", "GetShipmentCountryByKey");
			ad.SelectCommand.Parameters["@ShipmentCountryId"].Value = key.ToString();

			ShipmentCountryDs dataSet = new ShipmentCountryDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ShipmentCountryRef aRef = new ShipmentCountryRef();
			ShipmentCountryMapping(dataSet.ShipmentCountry[0], aRef);
			return aRef;
		}
		#endregion

		#region Shipment Port
		public ICollection getShipmentPortList(int shipmentCountryId)
		{
            IDataSetAdapter ad = getDataSetAdapter("ShipmentPortApt", "GetShipmentPortByShipmentCountryId");

			ShipmentPortDs dataSet = new ShipmentPortDs();
			ad.SelectCommand.Parameters["@ShipmentCountryId"].Value = shipmentCountryId.ToString();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(ShipmentPortDs.ShipmentPortRow row in dataSet.ShipmentPort)
			{
				ShipmentPortRef aRef = new ShipmentPortRef();
				ShipmentPortMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public ShipmentPortRef getShipmentPortByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("ShipmentPortApt", "GetShipmentPortByKey");
			ad.SelectCommand.Parameters["@ShipmentPortId"].Value = key.ToString();

			ShipmentPortDs dataSet = new ShipmentPortDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			ShipmentPortRef aRef = new ShipmentPortRef();
			ShipmentPortMapping(dataSet.ShipmentPort[0], aRef);
			return aRef;
		}

        public ICollection getShipmentPortList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentPortApt", "GetShipmentPortList");

            ShipmentPortDs dataSet = new ShipmentPortDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ShipmentPortDs.ShipmentPortRow row in dataSet.ShipmentPort)
            {
                ShipmentPortRef aRef = new ShipmentPortRef();
                ShipmentPortMapping(row, aRef);
                list.Add(aRef);
            }
            return list;
        }

        public ShipmentPortRef getShipmentPortByCode(string code)
        {
            IDataSetAdapter ad = getDataSetAdapter("ShipmentPortApt", "GetShipmentPortByCode");
            ad.SelectCommand.Parameters["@OfficialCode"].Value = code;

            ShipmentPortDs dataSet = new ShipmentPortDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected == 0) return null;

            ShipmentPortRef aRef = new ShipmentPortRef();
            ShipmentPortMapping(dataSet.ShipmentPort[0], aRef);
            return aRef;

        }

		#endregion
		
		#region Term Of Purchase
		public ICollection getTermOfPurchaseList()
		{
			IDataSetAdapter ad = getDataSetAdapter("TermOfPurchaseApt", "GetTermOfPurchaseList");

			TermOfPurchaseDs dataSet = new TermOfPurchaseDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(TermOfPurchaseDs.TermOfPurchaseRow row in dataSet.TermOfPurchase)
			{
				TermOfPurchaseRef aRef = new TermOfPurchaseRef();
				TermOfPurchaseMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}


		public ICollection getTermOfPurchaseListForVM()
		{
			IDataSetAdapter ad = getDataSetAdapter("TermOfPurchaseApt", "GetTermOfPurchaseList");

			TermOfPurchaseDs dataSet = new TermOfPurchaseDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(TermOfPurchaseDs.TermOfPurchaseRow row in dataSet.TermOfPurchase)
			{
				if (row.TermOfPurchaseId != TermOfPurchaseRef.Id.FOB.GetHashCode())
				{
					TermOfPurchaseRef aRef = new TermOfPurchaseRef();
					TermOfPurchaseMapping(row, aRef);
					list.Add(aRef);
				}
			}
			return list;
		}


		public TermOfPurchaseRef getTermOfPurchaseByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("TermOfPurchaseApt", "GetTermOfPurchaseByKey");
			ad.SelectCommand.Parameters["@TermOfPurchaseId"].Value = key.ToString();

			TermOfPurchaseDs dataSet = new TermOfPurchaseDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			TermOfPurchaseRef aRef = new TermOfPurchaseRef();
			TermOfPurchaseMapping(dataSet.TermOfPurchase[0], aRef);
			return aRef;
		}
		#endregion

		#region Option
		/*
		public ICollection getSizeOptionList(string sizeDescription, DateTime effectiveDate)
		{
			IDataSetAdapter ad = getDataSetAdapter("SizeOptionApt", "GetSizeOptionList");
			ad.SelectCommand.Parameters["@SizeDescription"].Value = sizeDescription;
			ad.SelectCommand.Parameters["@EffectiveDate"].Value = effectiveDate.ToShortDateString();
			// like operator
			SizeOptionDs dataSet = new SizeOptionDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(SizeOptionDs.SizeOptionRow row in dataSet.SizeOption)
			{
				SizeOptionRef aRef = new SizeOptionRef();
				SizeOptionMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}
		*/

		public ICollection getSizeOptionList(string sizeDescription, int seasonId, string itemNo)
		{
			IDataSetAdapter ad = getDataSetAdapter("SizeOptionApt", "GetSizeOptionList");
			ad.SelectCommand.Parameters["@SizeDescription"].Value = sizeDescription;
			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId.ToString();
			ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
			// like operator
			SizeOptionDs dataSet = new SizeOptionDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(SizeOptionDs.SizeOptionRow row in dataSet.SizeOption)
			{
				SizeOptionRef aRef = new SizeOptionRef();
				SizeOptionMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public ICollection getSizeOptionListFromMappingBySeasonItem(int seasonId, string itemNo)
		{
			IDataSetAdapter ad = getDataSetAdapter("SizeOptionApt", "GetSizeOptionListBySeasonItem");
			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId.ToString();
			ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
			// like operator
			SizeOptionDs dataSet = new SizeOptionDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(SizeOptionDs.SizeOptionRow row in dataSet.SizeOption)
			{
				SizeOptionRef aRef = new SizeOptionRef();
				SizeOptionMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public SizeOptionRef getSizeOptionBySeasonItem(int seasonId, string itemNo, int sizeOptionId)
		{
			IDataSetAdapter ad = getDataSetAdapter("SizeOptionApt", "GetSizeOptionBySeasonItem");
			ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId.ToString();
			ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
			ad.SelectCommand.Parameters["@SizeOptionId"].Value = sizeOptionId.ToString();

			SizeOptionDs dataSet = new SizeOptionDs();

			ad.SelectCommand.DbCommand.CommandTimeout = 120;
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			SizeOptionRef aRef = new SizeOptionRef();
			SizeOptionMapping(dataSet.SizeOption[0], aRef);
			return aRef;
		}

		public SizeOptionRef getSizeOptionByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("SizeOptionApt", "GetSizeOptionByKey");
			ad.SelectCommand.Parameters["@SizeOptionId"].Value = key.ToString();

			SizeOptionDs dataSet = new SizeOptionDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			SizeOptionRef aRef = new SizeOptionRef();
			SizeOptionMapping(dataSet.SizeOption[0], aRef);
			return aRef;
		}
		#endregion

		#region Quota Category
		public ICollection getQuotaCategoryList()
		{
			IDataSetAdapter ad = getDataSetAdapter("QuotaCategoryApt", "GetQuotaCategoryList");

			QuotaCategoryDs dataSet = new QuotaCategoryDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(QuotaCategoryDs.QuotaCategoryRow row in dataSet.QuotaCategory)
			{
				QuotaCategoryRef aRef = new QuotaCategoryRef();
				QuotaCategoryMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public QuotaCategoryRef getQuotaCategoryByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("QuotaCategoryApt", "GetQuotaCategoryByKey");
			ad.SelectCommand.Parameters["@QuotaCategoryId"].Value = key.ToString();

			QuotaCategoryDs dataSet = new QuotaCategoryDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			QuotaCategoryRef aRef = new QuotaCategoryRef();
			QuotaCategoryMapping(dataSet.QuotaCategory[0], aRef);
			return aRef;
		}
		#endregion

        #region StandardMSCourierCharge

        public StandardMSCourierChargeDef getStandardMSCourierCharge(int officeId, int deptId, int currencyId)
        {
            IDataSetAdapter ad = getDataSetAdapter("StandardMSCourierChargeApt", "GetStandardMSCourierCharge");
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId.ToString();
            ad.SelectCommand.Parameters["@DeptId"].Value = deptId.ToString();
            ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId.ToString();

            StandardMSCourierChargeDs dataSet = new StandardMSCourierChargeDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            StandardMSCourierChargeDef def = new StandardMSCourierChargeDef();
            StandardMSCourierChargeMapping(dataSet.StandardMSCourierCharge[0], def);
            return def;
        }
        #endregion

		#region Quota Category Group
		public ICollection getQuotaCategoryGroupList()
		{
			IDataSetAdapter ad = getDataSetAdapter("QuotaCategoryGroupApt", "GetQuotaCategoryGroupList");
			//ad.SelectCommand.Parameters["@QuotaCategoryGroupDescription"].Value = sizeDescription;
			// like operator
			QuotaCategoryGroupDs dataSet = new QuotaCategoryGroupDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(QuotaCategoryGroupDs.QuotaCategoryGroupRow row in dataSet.QuotaCategoryGroup)
			{
				QuotaCategoryGroupRef aRef = new QuotaCategoryGroupRef();
				QuotaCategoryGroupMapping(row, aRef);
				list.Add(aRef);
			}
			return list;
		}

		public QuotaCategoryGroupRef getQuotaCategoryGroupByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("QuotaCategoryGroupApt", "GetQuotaCategoryGroupByKey");
			ad.SelectCommand.Parameters["@QuotaCategoryGroupId"].Value = key.ToString();

			QuotaCategoryGroupDs dataSet = new QuotaCategoryGroupDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			QuotaCategoryGroupRef aRef = new QuotaCategoryGroupRef();
			QuotaCategoryGroupMapping(dataSet.QuotaCategoryGroup[0], aRef);
			return aRef;
		}
		#endregion

        #region ExchangeRate
        public decimal getExchangeRate(ExchangeRateType exchangeRateType, int currencyId, DateTime dt)
		{
			IDataSetAdapter ad = getDataSetAdapter("ExchangeRateApt", "GetExchangeRate");
			ad.SelectCommand.Parameters["@ExchangeRateTypeId"].Value = exchangeRateType.Id;
			ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;
			ad.SelectCommand.Parameters["@EffectiveDate"].Value = dt.ToShortDateString();
			ad.SelectCommand.Parameters["@EffectiveTypeId"].Value = -1;

			ExchangeRateDs dataSet = new ExchangeRateDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected == 1) 
				return Convert.ToDecimal(dataSet.ExchangeRate[0].ExchangeRate);
			else
			{
				ad = getDataSetAdapter("ExchangeRateApt", "GetCurrentExchangeRate");
				ad.SelectCommand.Parameters["@ExchangeRateTypeId"].Value = exchangeRateType.Id;
				ad.SelectCommand.Parameters["@CurrencyId"].Value = currencyId;
				recordsAffected = ad.Fill(dataSet);
				return Convert.ToDecimal(dataSet.ExchangeRate[0].ExchangeRate);
			}
        }

        public decimal getSeasonalExchangeRate(int seasonId, int fromCurrencyId, int toCurrencyId)
        {
            if (fromCurrencyId == toCurrencyId)
                return 1;

            IDataSetAdapter ad = getDataSetAdapter("SeasonalExchangeRateApt", "GetSeasonalExchangeRate");
            ad.SelectCommand.Parameters["@SeasonId"].Value = seasonId;
            ad.SelectCommand.Parameters["@FromCurrencyId"].Value = fromCurrencyId;
            ad.SelectCommand.Parameters["@ToCurrencyId"].Value = toCurrencyId;

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected > 0)
                return Convert.ToDecimal(dataSet.Tables[0].Rows[0][0]);
            else
                return 0;
        }
        #endregion

        #region Staff
        public ICollection getStaffListByName(string name)
        {
            IDataSetAdapter ad = getDataSetAdapter("StaffApt", "GetStaffList");
            ad.SelectCommand.Parameters["@Name"].Value = name.Replace(' ', '%');

            StaffDs dataSet = new StaffDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (StaffDs.StaffDirectoriesRow row in dataSet.StaffDirectories)
            {
                StaffRef aDef = new StaffRef();
                StaffMapping(row, aDef);
                list.Add(aDef);
            }
            return list;
        }

        #endregion

        #region Customer
        public ICollection getCustomerList(int payFlag)
		{
			IDataSetAdapter ad = getDataSetAdapter("CustomerApt", "GetCustomerList");
            ad.SelectCommand.Parameters["@IsPaymentRequired"].Value = payFlag;

			CustomerDs dataSet = new CustomerDs();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(CustomerDs.CustomerRow row in dataSet.Customer)
			{
				CustomerDef aDef = new CustomerDef();
				CustomerMapping(row, aDef);
				list.Add(aDef);
			}
			return list;
		}

		public CustomerDef getCustomerByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("CustomerApt", "GetCustomerByKey");
			ad.SelectCommand.Parameters["@CustomerId"].Value = key.ToString();

			CustomerDs dataSet = new CustomerDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			CustomerDef aDef = new CustomerDef();
			CustomerMapping(dataSet.Customer[0], aDef);
			return aDef;
		}
		#endregion

        #region TradingAgency
        public ICollection getTradingAgencyList()
        {
            IDataSetAdapter ad = getDataSetAdapter("TradingAgencyApt", "GetTradingAgencyList");
            TradingAgencyDs dataSet = new TradingAgencyDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (TradingAgencyDs.TradingAgencyRow row in dataSet.TradingAgency)
            {
                TradingAgencyDef aDef = new TradingAgencyDef();
                TradingAgencyMapping(row, aDef);
                list.Add(aDef);
            }
            return list;
        }

        public TradingAgencyDef getTradingAgencyByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("TradingAgencyApt", "GetTradingAgencyByKey");
            ad.SelectCommand.Parameters["@TradingAgencyId"].Value = key.ToString();

            TradingAgencyDs dataSet = new TradingAgencyDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            TradingAgencyDef def = new TradingAgencyDef();
            TradingAgencyMapping(dataSet.TradingAgency[0], def);
            
            return def;
        }
        #endregion

		#region WeightUnit method
		
		public ICollection getPriceUnitTypeList() 
		{
			ArrayList result = new ArrayList();
			result.Add(PriceUnitType.LENGTH);
			result.Add(PriceUnitType.WEIGHT);
			return result;
		}	
		
		#endregion

		#region OrderType method
		
		public ICollection getOrderTypeList() 
		{
			ArrayList result = new ArrayList();
			result.Add(OrderType.FOB);
			result.Add(OrderType.VM);
			return result;
		}	
		
		#endregion

		#region NextMfgVendorId

		public int getNextMfgSZVendorId()
		{
			return 3933;
		}

		public int getNextMfgLKVendorId()
		{
			return 3154;
		}

		public int getUnconfirmedVendorId()
		{
			return 1572;
		}

		#endregion

		#region SZUserIdList

		public int getSZUserIdList(int logonUserId)
		{
			int isSZOnly = 0;
			
			if (logonUserId == 1022 || logonUserId == 1026)
				isSZOnly = 1;

			return isSZOnly;
		}

		#endregion

        #region ReportOfficeGroup
        public ICollection getReportOfficeGroupList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ReportOfficeGroupApt", "GetReportOfficeGroupList");

            ReportOfficeGroupDs dataSet = new ReportOfficeGroupDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ReportOfficeGroupDs.ReportOfficeGroupRow row in dataSet.ReportOfficeGroup)
            {
                ReportOfficeGroupRef rf = new ReportOfficeGroupRef();
                rf.OfficeGroupId = row.OfficeGroupId;
                rf.GroupName = row.GroupName;
                rf.ShortName = row.ShortName;
                list.Add(rf);
            }
            return list;
        }

        public ArrayList getReportOfficeGroupListByAccessibleOfficeIdList(TypeCollector officeIdList)
        {
            return this.getReportOfficeGroupListByAccessibleOfficeIdList(officeIdList, false);
        }

        public ArrayList getReportOfficeGroupListByAccessibleOfficeIdList(TypeCollector officeIdList, bool hideHandlingOfficeGroup)
        {
            IDataSetAdapter ad = getDataSetAdapter("ReportOfficeGroupApt", "GetReportOfficeGroupListByAccessibleOfficeIdList");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);

            ReportOfficeGroupDs dataSet = new ReportOfficeGroupDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ReportOfficeGroupDs.ReportOfficeGroupRow row in dataSet.ReportOfficeGroup)
            {
                if (!((row.OfficeGroupId == 104 || row.OfficeGroupId == 105) && hideHandlingOfficeGroup))
                {
                    ReportOfficeGroupRef rf = new ReportOfficeGroupRef();
                    rf.OfficeGroupId = row.OfficeGroupId;
                    rf.GroupName = row.GroupName;
                    rf.ShortName = row.ShortName;
                    list.Add(rf);
                }
            }
            return list;
        }


        public ReportOfficeGroupRef getReportOfficeGroupByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("ReportOfficeGroupApt", "GetReportOfficeGroupByKey");
            ad.SelectCommand.Parameters["@OfficeGroupId"].Value = key.ToString();

            ReportOfficeGroupDs dataSet = new ReportOfficeGroupDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ReportOfficeGroupRef rf = new ReportOfficeGroupRef();
            rf.OfficeGroupId = dataSet.ReportOfficeGroup[0].OfficeGroupId;
            rf.GroupName = dataSet.ReportOfficeGroup[0].GroupName;
            rf.ShortName = dataSet.ReportOfficeGroup[0].ShortName;
            return rf;
        }

        public bool IsInReportOfficeGroup(int reportOfficeGroupId, int officeId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ReportOfficeGroupMappingApt", "GetReportOfficeGroupMappingByKey"); 
            ad.SelectCommand.Parameters["@OfficeGroupId"].Value = reportOfficeGroupId.ToString();
            ad.SelectCommand.Parameters["@OfficeId"].Value = officeId.ToString();

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1)
                return false;
            else
                return true;
        }

        public ArrayList getOfficeListByReportOfficeGroupId(int reportOfficeGroupId)
        {
            IDataSetAdapter ad = getDataSetAdapter("ReportOfficeGroupMappingApt", "GetReportOfficeGroupMappingByGroupId");
            ad.SelectCommand.Parameters["@OfficeGroupId"].Value = reportOfficeGroupId.ToString();

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                list.Add(generalWorker.getOfficeRefByKey(int.Parse(row[1].ToString())));
            }
            return list;
        }

        public OfficeRef getDGHandlingOffice(int officeId)
        {
            OfficeRef office;
            office = GeneralWorker.Instance.getOfficeRefByKey(officeId);

            if (officeId == OfficeId.DG.Id)
            {
                office.OfficeCode = "DG";
                office.Description = "Dongguan Office";
            }
            return office;
        }

        #endregion

        #region Bank
        public ICollection getBankList()
        {
            IDataSetAdapter ad = getDataSetAdapter("BankApt", "GetBankList");

            BankDs dataSet = new BankDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (BankDs.BankRow row in dataSet.Bank)
            {
                BankRef rf = new BankRef();
                BankMapping(row, rf);
                list.Add(rf);
            }
            return list;
        }

        public ICollection getBankListByVendorId(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("BankApt", "GetBankListByVendorId");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId.ToString();
            BankDs dataSet = new BankDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (BankDs.BankRow row in dataSet.Bank)
            {
                BankRef rf = new BankRef();
                BankMapping(row, rf);
                list.Add(rf);
            }
            return list;
        }

        public BankRef getBankByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("BankApt", "GetBankByKey");
            ad.SelectCommand.Parameters["@BankId"].Value = key.ToString();

            BankDs dataSet = new BankDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            BankRef rf = new BankRef();
            BankMapping(dataSet.Bank[0], rf);
            return rf;
        }

        public BankBranchRef getBankBranchByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("BankBranchApt", "GetBankBranchByKey");
            ad.SelectCommand.Parameters["@BankBranchId"].Value = key.ToString();

            BankBranchDs dataSet = new BankBranchDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            BankBranchRef rf = new BankBranchRef();
            BankBranchMapping(dataSet.BankBranch[0], rf);
            return rf;
        }

        public BankBranchRef getBankBranchByVendorId(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("BankBranchApt", "GetBankBranchByVendorId");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId.ToString();

            BankBranchDs dataSet = new BankBranchDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            BankBranchRef rf = new BankBranchRef();
            BankBranchMapping(dataSet.BankBranch[0], rf);
            return rf;
        }

        public ArrayList getBankBranchListByVendorId(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("BankBranchApt", "GetBankBranchByVendorId");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId.ToString();

            BankBranchDs dataSet = new BankBranchDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (BankBranchDs.BankBranchRow row in dataSet.BankBranch)
            {
                BankBranchRef rf = new BankBranchRef();
                BankBranchMapping(row, rf);
                list.Add(rf);
            }
            return list;                                                
        }

        public ICollection getBankBranchListByBankId(int bankId)
        {
            IDataSetAdapter ad = getDataSetAdapter("BankBranchApt", "GetBankBranchListByBankId");
            ad.SelectCommand.Parameters["@BankId"].Value = bankId.ToString();
            BankBranchDs dataSet = new BankBranchDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (BankBranchDs.BankBranchRow row in dataSet.BankBranch)
            {
                BankBranchRef rf = new BankBranchRef();
                BankBranchMapping(row, rf);
                list.Add(rf);
            }
            return list;
        }

        public ICollection getVendorBankMappingByVendorId(int vendorId)
        {
            IDataSetAdapter ad = getDataSetAdapter("VendorBankMappingApt", "GetVendorBankMappingByVendorId");
            ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            VendorBankMappingDs dataSet = new VendorBankMappingDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (VendorBankMappingDs.VendorBankMappingRow row in dataSet.VendorBankMapping)
            {
                VendorBankMappingRef rf = new VendorBankMappingRef();
                VendorBankMapping(row, rf);
                list.Add(rf);
            }

            return list;
        }

        private int getMaxBankId()
        {
            IDataSetAdapter ad = getDataSetAdapter("BankApt", "GetMaxBankId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        private int getMaxBankBranchId()
        {
            IDataSetAdapter ad = getDataSetAdapter("BankBranchApt", "GetMaxBankBranchId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public void updateBank(BankRef bankRef, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("BankApt", "GetBankByKey");
                ad.SelectCommand.Parameters["@BankId"].Value = bankRef.BankId;
                ad.PopulateCommands();
                
                BankDs dataSet = new BankDs();
                BankDs.BankRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.Bank[0];
                    this.BankMapping(bankRef, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.Bank.NewBankRow();
                    bankRef.BankId = this.getMaxBankId() + 1;
                    this.BankMapping(bankRef, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    sealStamp(row, userId, Stamp.UPDATE);

                    dataSet.Bank.AddBankRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Bank ERROR");
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public void updateSystemParameter(SystemParameterRef paramRef)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("SystemParameterApt", "GetSystemParameterByKey");
                ad.SelectCommand.Parameters["@ParameterId"].Value = paramRef.ParameterId;
                ad.PopulateCommands();

                SystemParameterDs dataSet = new SystemParameterDs();
                SystemParameterDs.SystemParameterRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.SystemParameter[0];
                    this.SystemParameterMapping(paramRef, row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update System Parameter ERROR");
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }


        public void updateBankBranch(BankBranchRef branchRef, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("BankBranchApt", "GetBankBranchByKey");
                ad.SelectCommand.Parameters["@BankBranchId"].Value = branchRef.BankBranchId;
                ad.PopulateCommands();

                BankBranchDs dataSet = new BankBranchDs();
                BankBranchDs.BankBranchRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.BankBranch[0];
                    this.BankBranchMapping(branchRef, row);                    
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.BankBranch.NewBankBranchRow() ;
                    branchRef.BankBranchId = this.getMaxBankBranchId() + 1;
                    this.BankBranchMapping(branchRef, row);                    
                    sealStamp(row, userId, Stamp.INSERT);
                    sealStamp(row, userId, Stamp.UPDATE);

                    dataSet.BankBranch.AddBankBranchRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Bank ERROR");
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        public void updateVendorBankMapping(VendorBankMappingRef vendorBankRef, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("VendorBankMappingApt", "GetVendorBankMappingByVendorId");
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorBankRef.VendorId;
                ad.PopulateCommands();

                VendorBankMappingDs dataSet = new VendorBankMappingDs();
                VendorBankMappingDs.VendorBankMappingRow  row = null;

                int recordsAffected = ad.Fill(dataSet);

                foreach (VendorBankMappingDs.VendorBankMappingRow r in dataSet.VendorBankMapping)
                {
                    if (r.BankId == vendorBankRef.BankId && r.BankBranchId == vendorBankRef.BankBranchId)
                    {
                        r.Status = vendorBankRef.Status;
                        row = r;
                        break;
                    }
                }

                if (row == null)
                {
                    row = dataSet.VendorBankMapping.NewVendorBankMappingRow();
                    this.VendorBankMapping(vendorBankRef, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    sealStamp(row, userId, Stamp.UPDATE);

                    dataSet.VendorBankMapping.AddVendorBankMappingRow(row);
                }
                else
                {
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update Bank ERROR");
                ctx.VoteCommit();

            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

#endregion

        #region System Parameter

        public List<SystemParameterRef> getSystemParametersByName(string paramName)
        {
            List<SystemParameterRef> list = new List<SystemParameterRef>();
            IDataSetAdapter ad = getDataSetAdapter("SystemParameterApt", "GetSystemParameterByName");
            ad.SelectCommand.Parameters["@ParameterName"].Value = paramName;

            SystemParameterDs dataSet = new SystemParameterDs();
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
                foreach (SystemParameterDs.SystemParameterRow row in dataSet.SystemParameter.Rows)
                {
                    SystemParameterRef rf = new SystemParameterRef();
                    SystemParameterMapping(row, rf);
                    list.Add(rf);
                }
            return list;
        }

        public SystemParameterRef getSystemParameterByName(string paramName)
        {
            List<SystemParameterRef> list = getSystemParametersByName(paramName);
            return (list.Count > 0 ? list[0] : null);
        }

        public SystemParameterRef getSystemParameterByKey(int paramId)
        {
            IDataSetAdapter ad = getDataSetAdapter("SystemParameterApt", "GetSystemParameterByKey");
            ad.SelectCommand.Parameters["@ParameterId"].Value = paramId;

            SystemParameterDs dataSet = new SystemParameterDs();
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected < 1) return null;

            SystemParameterRef rf = new SystemParameterRef();
            SystemParameterMapping(dataSet.SystemParameter[0], rf);

            return rf;
        }

        private void SystemParameterMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(SystemParameterDs.SystemParameterRow) &&
                target.GetType() == typeof(SystemParameterRef))
            {
                SystemParameterDs.SystemParameterRow row = (SystemParameterDs.SystemParameterRow)source;
                SystemParameterRef paraRef = (SystemParameterRef)target;

                paraRef.ParameterId = row.ParameterId;
                paraRef.ParameterName = row.ParameterName;
                if (!row.IsParameterValueNull())
                    paraRef.ParameterValue = row.ParameterValue;
                else
                    paraRef.ParameterValue = String.Empty;
                if (!row.IsRemarkNull())
                    paraRef.Remark = row.Remark;
                else
                    paraRef.Remark = String.Empty;
            }
            else if (source.GetType() == typeof(SystemParameterRef) &&
                target.GetType() == typeof(SystemParameterDs.SystemParameterRow))
            {
                SystemParameterRef paraRef = (SystemParameterRef)source;
                SystemParameterDs.SystemParameterRow row = (SystemParameterDs.SystemParameterRow)target;

                row.ParameterId = paraRef.ParameterId;
                row.ParameterName = paraRef.ParameterName;
                if (paraRef.ParameterValue == String.Empty)
                    row.SetParameterValueNull();
                else
                    row.ParameterValue = paraRef.ParameterValue;
                if (paraRef.Remark == String.Empty)
                    row.SetRemarkNull();
                else
                    row.Remark = paraRef.Remark;
            }
        }

        #endregion

        #region UK Discount Reason
        public ICollection getUKDiscountReasonList()
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountReasonApt", "GetUKDiscountReasonList");

            UKDiscountReasonDs dataSet = new UKDiscountReasonDs();

            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (UKDiscountReasonDs.UKDiscountReasonRow row in dataSet.UKDiscountReason)
            {
                UKDiscountReasonRef aRef = new UKDiscountReasonRef();
                UKDiscountReasonMapping(row, aRef);
                list.Add(aRef);
            }
            return list;
        }

        public UKDiscountReasonRef getUKDiscountReasonByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("UKDiscountReasonApt", "GetUKDiscountReasonByKey");
            ad.SelectCommand.Parameters["@UKDiscountReasonId"].Value = key.ToString();

            UKDiscountReasonDs dataSet = new UKDiscountReasonDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            UKDiscountReasonRef aRef = new UKDiscountReasonRef();
            UKDiscountReasonMapping(dataSet.UKDiscountReason[0], aRef);
            return aRef;
        }
        #endregion

		#region CartonType
		public ICollection getCartonTypeList(int PackingMethodId)
		{
			IDataSetAdapter ad = getDataSetAdapter("GetCartonTypeApt", "GetCartonTypeList");

			CartonTypeDs dataSet = new CartonTypeDs();
			ad.SelectCommand.Parameters["@PackingMethodId"].Value = PackingMethodId.ToString();
			int recordsAffected = ad.Fill(dataSet);

			ArrayList list = new ArrayList();
			
			foreach(CartonTypeDs.CartonTypeRow row in dataSet.CartonType)
			{
				CartonTypeRef aRef = new CartonTypeRef();
				CartonTypeMapping(row, aRef);
				list.Add(aRef);
			}
			return list;

		}

		public CartonTypeRef getCartonTypeByKey(int key)
		{
			IDataSetAdapter ad = getDataSetAdapter("GetCartonTypeApt", "GetCartonTypeByKey");
			ad.SelectCommand.Parameters["@CartonTypeId"].Value = key.ToString();

			CartonTypeDs dataSet = new CartonTypeDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected < 1) return null;

			CartonTypeRef aRef = new CartonTypeRef();
			CartonTypeMapping(dataSet.CartonType[0], aRef);
			return aRef;
		}

		private void CartonTypeMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(CartonTypeDs.CartonTypeRow) &&
				target.GetType() == typeof(CartonTypeRef))
			{
				CartonTypeDs.CartonTypeRow row = (CartonTypeDs.CartonTypeRow) source;
				CartonTypeRef aRef = (CartonTypeRef) target;

				aRef.CartonTypeId = row.CartonTypeId;
				aRef.CartonTypeDesc = row.CartonTypeDesc;
				aRef.PackingMethodId = row.PackingMethodId;
			} 
		}

		#endregion
	
		#region QuarterlyExchangeRate

		public decimal getQuarterlyExchangeRate(int fromCurrencyId, int toCurrencyId, DateTime effectiveDate)
		{
			IDataSetAdapter ad = getDataSetAdapter("QuarterlyExchangeRateApt", "GetQuarterlyExchangeRate");
			ad.SelectCommand.Parameters["@FromCurrencyId"].Value = fromCurrencyId;
			ad.SelectCommand.Parameters["@ToCurrencyId"].Value = toCurrencyId;
			ad.SelectCommand.Parameters["@EffectiveDate"].Value = effectiveDate.ToShortDateString();

			QuarterlyExchangeRateDs dataSet = new QuarterlyExchangeRateDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected == 1) 
				return Convert.ToDecimal(dataSet.QuarterlyExchangeRate[0].ExchangeRate);
			else
				return 0;
		}

		#endregion

		#region UTurnOrderParameter

		public decimal getUTurnOrderParameter(int parameterTypeId, DateTime effectiveDate)
		{
			IDataSetAdapter ad = getDataSetAdapter("UTurnOrderParameterApt", "GetUTurnOrderParameter");
			ad.SelectCommand.Parameters["@ParameterTypeId"].Value = parameterTypeId;
			ad.SelectCommand.Parameters["@EffectiveDate"].Value = effectiveDate.ToShortDateString();

			UTurnOrderParameterDs dataSet = new UTurnOrderParameterDs();
			int recordsAffected = ad.Fill(dataSet);

			if (recordsAffected == 1) 
				return Convert.ToDecimal(dataSet.UTurnOrderParameter[0].ParameterValue);
			else
				return 0;
		}

		#endregion

        #region ThirdPartyAgency

        private void ThirdPartyAgencyMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ThirdPartyAgencyDs.ThirdPartyAgencyRow) &&
                target.GetType() == typeof(ThirdPartyAgencyRef))
            {
                ThirdPartyAgencyDs.ThirdPartyAgencyRow row = (ThirdPartyAgencyDs.ThirdPartyAgencyRow)source;
                ThirdPartyAgencyRef aRef = (ThirdPartyAgencyRef)target;

                aRef.ThirdPartyAgencyId = row.ThirdPartyAgencyId;
                aRef.Description = row.Description;
                aRef.Name = row.Name;
                aRef.AgencyCommissionPercentage = row.AgencyCommissionPercentage;
                aRef.OfficeId = row.OfficeId;
            }
        }

        public ICollection getThirdPartyAgencyList()
        {
            IDataSetAdapter ad = getDataSetAdapter("ThirdPartyAgencyApt", "GetThirdPartyAgencyList");

            ThirdPartyAgencyDs dataSet = new ThirdPartyAgencyDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (ThirdPartyAgencyDs.ThirdPartyAgencyRow row in dataSet.ThirdPartyAgency)
            {

                ThirdPartyAgencyRef aRef = new ThirdPartyAgencyRef();
                ThirdPartyAgencyMapping(row, aRef);
                list.Add(aRef);
            }
            return list;
        }


        public ThirdPartyAgencyRef getThirdPartyAgencyByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("ThirdPartyAgencyApt", "GetThirdPartyAgencyKey");
            ad.SelectCommand.Parameters["@ThirdPartyAgencyId"].Value = key.ToString();

            ThirdPartyAgencyDs dataSet = new ThirdPartyAgencyDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            ThirdPartyAgencyRef aRef = new ThirdPartyAgencyRef();
            ThirdPartyAgencyMapping(dataSet.ThirdPartyAgency[0], aRef);
            return aRef;
        }
        #endregion

        #region Budget Year
        public ArrayList getBudgetYearList()
        {
            IDataSetAdapter ad = getDataSetAdapter("AccountFinancialCalenderApt", "GetAllBudgetYear");
            ad.SelectCommand.Parameters["@AppId"].Value = 9;

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList al = new ArrayList();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                al.Add(row["budgetyear"].ToString());
            }

            return al;
        }
        #endregion

        #region File Upload
        public FileUploadDef getFileUploadByKey(int fileId)
        {
            IDataSetAdapter ad = getDataSetAdapter("FileUploadApt", "GetFileUploadByKey");
            ad.SelectCommand.Parameters["@FileId"].Value = fileId;

            FileUploadDs dataSet = new FileUploadDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            FileUploadDef aRef = new FileUploadDef();
            FileUploadMapping(dataSet.FileUpload[0], aRef);
            return aRef;
        }

        public ArrayList getFileUploadByClaimId(int claimId)
        {
            IDataSetAdapter ad = getDataSetAdapter("FileUploadApt", "GetFileUploadByClaimId");
            ad.SelectCommand.Parameters["@ClaimId"].Value = claimId;

            FileUploadDs dataSet = new FileUploadDs();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList list = new ArrayList();

            foreach (FileUploadDs.FileUploadRow row in dataSet.FileUpload)
            {

                FileUploadDef  aRef = new FileUploadDef();
                FileUploadMapping(row, aRef);
                list.Add(aRef);
            }
            return list;
        }

        private int getMaxFileId()
        {
            IDataSetAdapter ad = getDataSetAdapter("FileUploadApt", "GetMaxFileId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }


        public void updateFileUpload(FileUploadDef fileUploadDef, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("FileUploadApt", "GetFileUploadByKey");
                ad.SelectCommand.Parameters["@FileId"].Value = fileUploadDef.FileId;
                ad.PopulateCommands();

                FileUploadDs dataSet = new FileUploadDs();
                FileUploadDs.FileUploadRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.FileUpload[0];
                    this.FileUploadMapping(fileUploadDef, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                }
                else
                {
                    row = dataSet.FileUpload.NewFileUploadRow();
                    fileUploadDef.FileId = this.getMaxFileId() + 1;
                    this.FileUploadMapping(fileUploadDef, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    sealStamp(row, userId, Stamp.UPDATE);

                    dataSet.FileUpload.AddFileUploadRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update FileUpload ERROR");
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }


        #endregion

        public void GenericDataSummaryMapping(object source, object target)
        {
            if (source.GetType() == typeof(GenericDataSummaryDs.GenericDataSummaryRow) && target.GetType() == typeof(GenericDataSummaryDef))
            {
                GenericDataSummaryDs.GenericDataSummaryRow row = (GenericDataSummaryDs.GenericDataSummaryRow)source;
                GenericDataSummaryDef def = (GenericDataSummaryDef)target;
                def.Id1 = (row.IsId1Null() ? (-1) : row.Id1);
                def.Id2 = (row.IsId2Null() ? (-1) : row.Id2);
                def.Id3 = (row.IsId2Null() ? (-1) : row.Id3);
                def.Id4 = (row.IsId2Null() ? (-1) : row.Id4);
                def.Id5 = (row.IsId2Null() ? (-1) : row.Id5);
                def.Num1 = (row.IsNum1Null() ? decimal.MinValue : row.Num1);
                def.Num2 = (row.IsNum2Null() ? decimal.MinValue : row.Num2);
                def.Num3 = (row.IsNum3Null() ? decimal.MinValue : row.Num3);
                def.Num4 = (row.IsNum4Null() ? decimal.MinValue : row.Num4);
                def.Num5 = (row.IsNum5Null() ? decimal.MinValue : row.Num5);
                def.Num6 = (row.IsNum6Null() ? decimal.MinValue : row.Num6);
                def.Num7 = (row.IsNum7Null() ? decimal.MinValue : row.Num7);
                def.Num8 = (row.IsNum8Null() ? decimal.MinValue : row.Num8);
                def.Num9 = (row.IsNum9Null() ? decimal.MinValue : row.Num9);
                def.Num10 = (row.IsNum10Null() ? decimal.MinValue : row.Num10);
                def.String1 = (row.IsString1Null() ? string.Empty : row.String1);
                def.String2 = (row.IsString2Null() ? string.Empty : row.String2);
                def.String3 = (row.IsString3Null() ? string.Empty : row.String3);
                def.String4 = (row.IsString4Null() ? string.Empty : row.String4);
                def.String5 = (row.IsString5Null() ? string.Empty : row.String5);
                def.Date1 = (row.IsDate1Null() ? DateTime.MinValue : row.Date1);
                def.Date2 = (row.IsDate2Null() ? DateTime.MinValue : row.Date2);
                def.Date3 = (row.IsDate3Null() ? DateTime.MinValue : row.Date3);
                def.Date4 = (row.IsDate4Null() ? DateTime.MinValue : row.Date4);
                def.Date5 = (row.IsDate5Null() ? DateTime.MinValue : row.Date5);
            }
        }

        public List<string> getNSLedSelfBilledSupplierCodeList()
        {
            List<string> list = new List<string>();
            DataSet dataset = new DataSet();
            IDataSetAdapter ad = getDataSetAdapter("NSLedSelfBilledSupplierCodeApt", "GetNSLedSelfBilledSupplierCodeList");
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataset);
            if (recordsAffected > 0)
                foreach (DataRow row in dataset.Tables[0].Rows)
                    list.Add(row["UKSupplierCode"].ToString());
            return list;
        }

        public SizeOptionRef getNSLedSizeOption(string itemNo, string sizeOptionNo)
        {
            TypeCollector inclusive = TypeCollector.Inclusive;
            inclusive.append(CustomerDef.Id.ns_led.GetHashCode());
            inclusive.append(CustomerDef.Id.manu_led.GetHashCode());
            IDataSetAdapter dataSetAdapter = getDataSetAdapter("SizeOptionApt", "GetNSLedSizeOption");
            dataSetAdapter.SelectCommand.Parameters["@ItemNo"].Value = itemNo;
            dataSetAdapter.SelectCommand.Parameters["@SizeOptionNo"].Value = sizeOptionNo;
            dataSetAdapter.SelectCommand.CustomParameters["@CustomerIdList"] = CustomDataParameter.parse(inclusive.IsInclusive, inclusive.Values);
            SizeOptionDs ds = new SizeOptionDs();
            int recordsAffected = dataSetAdapter.Fill(ds);
            if (recordsAffected < 1)
            {
                return null;
            }
            SizeOptionRef sizeOptionRef = new SizeOptionRef();
            SizeOptionMapping(ds.SizeOption[0], sizeOptionRef);
            return sizeOptionRef;
        }

        public void setParameterStatus(int paramId, bool disabled)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                SystemParameterRef paraRef = this.getSystemParameterByKey(paramId);
                if (disabled)
                    paraRef.ParameterValue = "Y";
                else
                    paraRef.ParameterValue = "N";
                this.updateSystemParameter(paraRef);
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }



        #region Mapping Functions
        private void AirFreightPaymentTypeMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(AirFreightPaymentTypeDs.AirFreightPaymentTypeRow) &&
                target.GetType() == typeof(AirFreightPaymentTypeRef))
            {
                AirFreightPaymentTypeDs.AirFreightPaymentTypeRow row = (AirFreightPaymentTypeDs.AirFreightPaymentTypeRow)source;
                AirFreightPaymentTypeRef aRef = (AirFreightPaymentTypeRef)target;

                aRef.AirFreightPaymentTypeId = row.AirFreightPaymentTypeId;
                aRef.AirFreightPaymentTypeDescription = row.AirFreightPaymentTypeDesc;
            }
        }

        private void CustomerTypeMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(CustomerTypeDs.CustomerTypeRow) &&
                target.GetType() == typeof(CustomerTypeRef))
            {
                CustomerTypeDs.CustomerTypeRow row = (CustomerTypeDs.CustomerTypeRow)source;
                CustomerTypeRef aRef = (CustomerTypeRef)target;

                aRef.CustomerTypeId = row.CustomerTypeId;
                aRef.CustomerTypeDescription = row.CustomerTypeDesc;
            }
        }

        private void OtherCostTypeMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(OtherCostTypeDs.OtherCostTypeRow) &&
                target.GetType() == typeof(OtherCostTypeRef))
            {
                OtherCostTypeDs.OtherCostTypeRow row = (OtherCostTypeDs.OtherCostTypeRow)source;
                OtherCostTypeRef aRef = (OtherCostTypeRef)target;

                aRef.OtherCostTypeId = row.OtherCostTypeId;
                aRef.OtherCostTypeDescription = row.OtherCostTypeDesc;
                aRef.OPSKey = row.OtherCostTypeOPSKey;
                if (row.IsSunAccountCodeNull())
                    aRef.SunAccountCode = string.Empty;
                else
                    aRef.SunAccountCode = row.SunAccountCode;
            }
        }

        private void StandardMSCourierChargeMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(StandardMSCourierChargeDs.StandardMSCourierChargeRow) &&
                target.GetType() == typeof(StandardMSCourierChargeDef))
            {
                StandardMSCourierChargeDs.StandardMSCourierChargeRow row = (StandardMSCourierChargeDs.StandardMSCourierChargeRow)source;
                StandardMSCourierChargeDef def = (StandardMSCourierChargeDef)target;

                def.ChargeId = row.ChargeId;
                def.OfficeId = row.OfficeId;
                def.DeptId = row.DeptId;
                def.CurrencyId = row.CurrencyId;
                def.ChargeRate = row.ChargeRate;
            }
        }

        private void PackingMethodMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(PackingMethodDs.PackingMethodRow) &&
                target.GetType() == typeof(PackingMethodRef))
            {
                PackingMethodDs.PackingMethodRow row = (PackingMethodDs.PackingMethodRow)source;
                PackingMethodRef rf = (PackingMethodRef)target;

                rf.PackingMethodId = row.PackingMethodId;
                rf.PackingMethodDescription = row.PackingMethodDesc;
                rf.PackingMethodCode = row.PackingMethodCode;
                rf.OPSKey = row.PackingMethodOPSKey;
                rf.Refurb = row.Refurb;
            }
        }

        private void PackingUnitMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(PackingUnitDs.PackingUnitRow) &&
                target.GetType() == typeof(PackingUnitRef))
            {
                PackingUnitDs.PackingUnitRow row = (PackingUnitDs.PackingUnitRow)source;
                PackingUnitRef aRef = (PackingUnitRef)target;

                aRef.PackingUnitId = row.PackingUnitId;
                aRef.PackingUnitDescription = row.PackingUnitDesc;
                aRef.OPSKey = row.PackingUnitOPSKey;
            }
        }

        private void PurchaseLocationMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(PurchaseLocationDs.PurchaseLocationRow) &&
                target.GetType() == typeof(PurchaseLocationRef))
            {
                PurchaseLocationDs.PurchaseLocationRow row = (PurchaseLocationDs.PurchaseLocationRow)source;
                PurchaseLocationRef aRef = (PurchaseLocationRef)target;

                aRef.PurchaseLocationId = row.PurchaseLocationId;
                aRef.PurchaseLocationDescription = row.PurchaseLocationDesc;
            }
        }

        private void ShipmentMethodMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ShipmentMethodDs.ShipmentMethodRow) &&
                target.GetType() == typeof(ShipmentMethodRef))
            {
                ShipmentMethodDs.ShipmentMethodRow row = (ShipmentMethodDs.ShipmentMethodRow)source;
                ShipmentMethodRef aRef = (ShipmentMethodRef)target;

                aRef.ShipmentMethodId = row.ShipmentMethodId;
                aRef.ShipmentMethodDescription = row.ShipmentMethodDesc;
                aRef.OPSKey = row.ShipmentMethodOPSKey;
            }
        }

        private void ShipmentCountryMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ShipmentCountryDs.ShipmentCountryRow) &&
                target.GetType() == typeof(ShipmentCountryRef))
            {
                ShipmentCountryDs.ShipmentCountryRow row = (ShipmentCountryDs.ShipmentCountryRow)source;
                ShipmentCountryRef aRef = (ShipmentCountryRef)target;

                aRef.ShipmentCountryId = row.ShipmentCountryId;
                aRef.ShipmentCountryDescription = row.ShipmentCountryDesc;
                aRef.OPSKey = row.ShipmentCountryOPSKey;
            }
        }

        private void ShipmentPortMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(ShipmentPortDs.ShipmentPortRow) &&
                target.GetType() == typeof(ShipmentPortRef))
            {
                ShipmentPortDs.ShipmentPortRow row = (ShipmentPortDs.ShipmentPortRow)source;
                ShipmentPortRef aRef = (ShipmentPortRef)target;

                aRef.ShipmentPortId = row.ShipmentPortId;
                aRef.ShipmentPortDescription = row.ShipmentPortDesc;
                aRef.ShipmentCountry = getShipmentCountryByKey(row.ShipmentCountryId);
                aRef.OPSKey = row.ShipmentPortOPSKey;
                if (!row.IsOfficialCodeNull())
                    aRef.OfficialCode = row.OfficialCode;
                else
                    aRef.OfficialCode = string.Empty;
            }
        }

        private void TermOfPurchaseMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(TermOfPurchaseDs.TermOfPurchaseRow) &&
                target.GetType() == typeof(TermOfPurchaseRef))
            {
                TermOfPurchaseDs.TermOfPurchaseRow row = (TermOfPurchaseDs.TermOfPurchaseRow)source;
                TermOfPurchaseRef aRef = (TermOfPurchaseRef)target;

                aRef.TermOfPurchaseId = row.TermOfPurchaseId;
                aRef.TermOfPurchaseDescription = row.TermOfPurchaseDesc;
                aRef.OrderType = row.OrderType;
            }
        }

        private void UKDiscountReasonMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKDiscountReasonDs.UKDiscountReasonRow) &&
                target.GetType() == typeof(UKDiscountReasonRef))
            {
                UKDiscountReasonDs.UKDiscountReasonRow row = (UKDiscountReasonDs.UKDiscountReasonRow)source;
                UKDiscountReasonRef aRef = (UKDiscountReasonRef)target;

                aRef.UKDiscountReasonId = row.UKDiscountReasonId;
                aRef.UKDiscountReason = row.UKDiscountReason;
            }
        }

        private void SizeOptionMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(SizeOptionDs.SizeOptionRow) &&
                target.GetType() == typeof(SizeOptionRef))
            {
                SizeOptionDs.SizeOptionRow row = (SizeOptionDs.SizeOptionRow)source;
                SizeOptionRef aRef = (SizeOptionRef)target;

                aRef.SizeOptionId = row.SizeOptionId;
                aRef.SizeOptionNo = row.SizeOptionNo;
                aRef.EffectiveDateFrom = row.EffectiveDateFrom;
                aRef.EffectiveDateTo = row.EffectiveDateTo;
                aRef.SizeDescription = row.SizeDesc;
            }
        }

        private void QuotaCategoryGroupMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(QuotaCategoryGroupDs.QuotaCategoryGroupRow) &&
                target.GetType() == typeof(QuotaCategoryGroupRef))
            {
                QuotaCategoryGroupDs.QuotaCategoryGroupRow row = (QuotaCategoryGroupDs.QuotaCategoryGroupRow)source;
                QuotaCategoryGroupRef aRef = (QuotaCategoryGroupRef)target;

                aRef.QuotaCategoryGroupId = row.QuotaCategoryGroupId;
                aRef.QuotaCategoryGroupDescription = row.QuotaCategoryGroupDesc;
                aRef.OPSKey = row.QuotaCategoryGroupOPSKey;

                aRef.OPAUpchargeHKD = row.OPAUpchargeHKD;
                aRef.OPAUpchargeUSD = row.OPAUpchargeUSD;
            }
        }

        private void QuotaCategoryMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(QuotaCategoryDs.QuotaCategoryRow) &&
                target.GetType() == typeof(QuotaCategoryRef))
            {
                QuotaCategoryDs.QuotaCategoryRow row = (QuotaCategoryDs.QuotaCategoryRow)source;
                QuotaCategoryRef aRef = (QuotaCategoryRef)target;

                aRef.QuotaCategoryId = row.QuotaCategoryId;
                aRef.QuotaCategoryNo = row.QuotaCategoryNo;
                aRef.QuotaCategoryDesc = row.QuotaCategoryDesc;
                if (!row.IsQuotaCategoryFullDescNull())
                    aRef.QuotaCategoryFullDesc = row.QuotaCategoryFullDesc;
                else
                    aRef.QuotaCategoryFullDesc = String.Empty;
                aRef.OPSKey = row.QuotaCategoryOPSKey;
            }
        }

        private void CustomerMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(CustomerDs.CustomerRow) &&
                target.GetType() == typeof(CustomerDef))
            {
                CustomerDs.CustomerRow row = (CustomerDs.CustomerRow)source;
                CustomerDef aDef = (CustomerDef)target;

                aDef.CustomerId = row.CustomerId;
                aDef.CustomerCode = row.CustomerCode;
                aDef.CustomerDescription = row.CustomerDesc;
                aDef.CustomerType = this.getCustomerTypeByKey(row.CustomerTypeId);
                aDef.InvoicePrefix = row.InvoicePrefix;
                if (!row.IsAddress1Null()) aDef.Address1 = row.Address1;
                if (!row.IsAddress2Null()) aDef.Address2 = row.Address2;
                if (!row.IsAddress3Null()) aDef.Address3 = row.Address3;
                if (!row.IsAddress4Null()) aDef.Address4 = row.Address4;
                if (!row.IsTelNoNull()) aDef.TelNo = row.TelNo;
                if (!row.IsFaxNoNull()) aDef.FaxNo = row.FaxNo;
                if (!row.IsTelexNoNull()) aDef.TelexNo = row.TelexNo;
                if (!row.IsContactNull()) aDef.Contact = row.Contact;
                if (!row.IsConsigneeNull()) aDef.Consignee = row.Consignee;
                if (!row.IsDeliveryToNull()) aDef.DeliveryTo = row.DeliveryTo;
                if (!row.IsCustomerOPSKeyNull()) aDef.OPSKey = row.CustomerOPSKey;
                aDef.SUNAccountCode = row.IsSUNAccountCodeNull() ? string.Empty : row.SUNAccountCode;
                aDef.EpicorCustomerId = row.IsEpicorCustIdNull() ? string.Empty : row.EpicorCustId;
                aDef.ShortCode = row.CustomerShortCode;
                aDef.IsPaymentRequired = row.IsPaymentRequired;
                aDef.IsSelfBilling = row.IsSelfBilling;
            }
        }


        private void StaffMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(StaffDs.StaffDirectoriesRow) &&
                target.GetType() == typeof(StaffRef))
            {
                StaffDs.StaffDirectoriesRow row = (StaffDs.StaffDirectoriesRow)source;
                StaffRef aDef = (StaffRef)target;

                aDef.StaffId = row.StaffId;
                row.StaffFirstName = row.StaffFirstName.ToLower();
                row.StaffLastName = row.StaffLastName.ToLower();
                if (row.StaffFirstName.Length > 1)
                    row.StaffFirstName = row.StaffFirstName.Substring(0, 1).ToUpper() + row.StaffFirstName.Substring(1);
                else
                    row.StaffFirstName = row.StaffFirstName.ToUpper();
                if (row.StaffLastName.Length > 1)
                    row.StaffLastName = row.StaffLastName.Substring(0, 1).ToUpper() + row.StaffLastName.Substring(1);
                else
                    row.StaffLastName = row.StaffLastName.ToUpper();

                if (!row.IsStaffInitialNameNull())
                    aDef.Name =  row.StaffFirstName + " " + row.StaffLastName + (row.StaffInitialName.Trim() != String.Empty ? ", " + row.StaffInitialName : String.Empty);
                if (!row.IsStaffLastNameNull() && !row.IsStaffFirstNameNull() && row.IsStaffInitialNameNull())
                    aDef.Name = row.StaffFirstName + " " + row.StaffLastName;
                if (!row.IsEMailAddressNull()) aDef.Email = row.EMailAddress;
                if (!row.IsCountryCodeNull())
                    if (row.CountryCode.Trim() != String.Empty)
                        aDef.Phone = row.CountryCode + "-" + row.BusinessNo;
                    else
                        aDef.Phone = row.BusinessNo;
                else
                    aDef.Phone = row.BusinessNo;
                if (!row.IsExtensionNull()) aDef.Extension = row.Extension;
                if (!row.IsTitleNull()) aDef.Title = row.Title;
                if (!row.IsDepartmentNull()) aDef.Department = row.Department;
                if (!row.IsCompanyNull()) aDef.Company = row.Office + " - " + row.Company;
                if (!row.IsOfficeNull()) aDef.Office = row.Office;
                aDef.Status = row.Status;
            }
        }

        private void TradingAgencyMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(TradingAgencyDs.TradingAgencyRow) &&
                target.GetType() == typeof(TradingAgencyDef))
            {
                TradingAgencyDs.TradingAgencyRow row = (TradingAgencyDs.TradingAgencyRow)source;
                TradingAgencyDef def = (TradingAgencyDef)target;

                def.TradingAgencyId = row.TradingAgencyId;
                def.Name = row.Name;
                def.ShortName = row.ShortName;
                if (!row.IsAddress1Null()) def.Address1 = row.Address1; else def.Address1 = String.Empty;
                if (!row.IsAddress2Null()) def.Address2 = row.Address2; else def.Address2 = String.Empty;
                if (!row.IsAddress3Null()) def.Address3 = row.Address3; else def.Address3 = String.Empty;
                if (!row.IsAddress4Null()) def.Address4 = row.Address4; else def.Address4 = String.Empty;
                if (!row.IsAddress4Null()) def.Country = generalWorker.getCountryByKey(row.CountryId);
                if (!row.IsTelNoNull()) def.TelNo = row.TelNo;
                if (!row.IsFaxNoNull()) def.FaxNo = row.FaxNo;
                if (!row.IsRemarkNull()) def.Remark = row.Remark; else def.Remark = String.Empty;
                if (!row.IsBankNameNull()) def.BankName = row.BankName; else def.BankName = String.Empty;
                if (!row.IsBankAddress1Null()) def.BankAddress1 = row.BankAddress1; else def.BankAddress1 = String.Empty;
                if (!row.IsBankAddress2Null()) def.BankAddress2 = row.BankAddress2; else def.BankAddress2 = String.Empty;
                if (!row.IsBankAddress3Null()) def.BankAddress3 = row.BankAddress3; else def.BankAddress3 = String.Empty;
                if (!row.IsBankAddress4Null()) def.BankAddress4 = row.BankAddress4; else def.BankAddress4 = String.Empty;
                if (!row.IsBankAccountNameNull()) def.BankAccountName = row.BankAccountName; else def.BankAccountName = String.Empty;
                if (!row.IsAccountNoNull()) def.AccountNo = row.AccountNo; else def.AccountNo = String.Empty;
            }
        }

        private void CustomerDestinationMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(CustomerDestinationDs.CustomerDestinationRow) &&
                target.GetType() == typeof(CustomerDestinationDef))
            {
                CustomerDestinationDs.CustomerDestinationRow row = (CustomerDestinationDs.CustomerDestinationRow)source;
                CustomerDestinationDef aDef = (CustomerDestinationDef)target;

                aDef.CustomerDestinationId = row.CustomerDestinationId;
                aDef.CustomerId = row.CustomerId;
                aDef.DestinationCode = row.DestinationCode;
                aDef.DestinationDesc = row.DestinationDesc;
                aDef.Consignee = row.Consignee;
                aDef.DeliveryTo = row.DeliveryTo;
                aDef.UTurnOrder = row.UTurnOrder;

                if (!row.IsFranchisePartnercodeNull())
                    aDef.FranchisePartnercode = row.FranchisePartnercode;
                else
                    aDef.FranchisePartnercode = String.Empty;
            }
        }

        private void BankMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(BankDs.BankRow) &&
                target.GetType() == typeof(BankRef))
            {
                BankDs.BankRow row = (BankDs.BankRow)source;
                BankRef rf = (BankRef)target;

                rf.BankId = row.BankId;
                rf.BankName = row.BankName;
                rf.Status = row.Status;
                rf.Branches = (ArrayList)this.getBankBranchListByBankId(row.BankId);
            }
            else if (source.GetType() == typeof(BankRef) &&
                target.GetType() == typeof(BankDs.BankRow))
            {
                BankDs.BankRow row = (BankDs.BankRow)target;
                BankRef rf = (BankRef)source;

                row.BankId = rf.BankId;
                row.BankName = rf.BankName;
                row.Status = rf.Status;
            }
        }

        private void BankBranchMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(BankBranchDs.BankBranchRow) &&
                target.GetType() == typeof(BankBranchRef))
            {
                BankBranchDs.BankBranchRow row = (BankBranchDs.BankBranchRow)source;
                BankBranchRef rf = (BankBranchRef)target;

                rf.BankBranchId = row.BankBranchId;
                rf.BankId = row.BankId;
                rf.BranchName = row.BranchName;
                if (!row.IsAddress1Null())
                    rf.Address1 = row.Address1;
                else
                    rf.Address1 = String.Empty;
                if (!row.IsAddress2Null())
                    rf.Address2 = row.Address2;
                else
                    rf.Address2 = String.Empty;
                if (!row.IsAddress3Null())
                    rf.Address3 = row.Address3;
                else
                    rf.Address3 = String.Empty;
                if (!row.IsAddress4Null())
                    rf.Address4 = row.Address4;
                else
                    rf.Address4 = String.Empty;
                if (!row.IsCountryIdNull())
                    rf.Country = generalWorker.getCountryByKey(row.CountryId);
                if (!row.IsContactPersonNull())
                    rf.ContactPerson = row.ContactPerson;
                else
                    rf.ContactPerson = String.Empty;
                if (!row.IsPhoneNoNull())
                    rf.Phone = row.PhoneNo;
                else
                    rf.Phone = String.Empty;

                rf.Status = row.Status;
            }
            else if (source.GetType() == typeof(BankBranchRef) &&
                target.GetType() == typeof(BankBranchDs.BankBranchRow))
            {
                BankBranchDs.BankBranchRow row = (BankBranchDs.BankBranchRow)target;
                BankBranchRef rf = (BankBranchRef)source;

                row.BankBranchId = rf.BankBranchId;
                row.BankId = rf.BankId;
                row.BranchName = rf.BranchName;
                if (rf.Address1 != String.Empty)
                    row.Address1 = rf.Address1;
                else
                    row.SetAddress1Null();
                if (rf.Address2 != String.Empty)
                    row.Address2 = rf.Address2;
                else
                    row.SetAddress2Null();
                if (rf.Address3 != String.Empty)
                    row.Address3 = rf.Address3;
                else
                    row.SetAddress3Null();
                if (rf.Address4 != String.Empty)
                    row.Address4 = rf.Address4;
                else
                    row.SetAddress4Null();
                row.CountryId = rf.Country.CountryId;
                if (rf.ContactPerson != String.Empty)
                    row.ContactPerson = rf.ContactPerson;
                else
                    row.SetContactPersonNull();
                if (rf.Phone != String.Empty)
                    row.PhoneNo = rf.Phone;
                else
                    row.SetPhoneNoNull();
                row.Status = rf.Status;
            }
        }

        private void VendorBankMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(VendorBankMappingDs.VendorBankMappingRow) &&
                target.GetType() == typeof(VendorBankMappingRef))
            {
                VendorBankMappingDs.VendorBankMappingRow row = (VendorBankMappingDs.VendorBankMappingRow)source;
                VendorBankMappingRef rf = (VendorBankMappingRef)target;

                rf.VendorId = row.VendorId;
                rf.BankId = row.BankId;
                rf.BankBranchId = row.BankBranchId;
                rf.Status = row.Status;
            }
            else if (source.GetType() == typeof(VendorBankMappingRef)  &&
                target.GetType() == typeof(VendorBankMappingDs.VendorBankMappingRow))
            {
                VendorBankMappingDs.VendorBankMappingRow  row = (VendorBankMappingDs.VendorBankMappingRow)target;
                VendorBankMappingRef  rf = (VendorBankMappingRef)source;

                row.VendorId = rf.VendorId;
                row.BankId = rf.BankId;
                row.BankBranchId = rf.BankBranchId;
                row.Status = rf.Status;
            }
        }

        private void UKFinalDestinationMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(UKFinalDestinationDs.UKFinalDestinationRow) &&
                target.GetType() == typeof(UKFinalDestinationDef))
            {
                UKFinalDestinationDs.UKFinalDestinationRow row = (UKFinalDestinationDs.UKFinalDestinationRow)source;
                UKFinalDestinationDef aDef = (UKFinalDestinationDef)target;

                aDef.UKFinalDestinationId = row.UKFinalDestinationId;
                if (!row.IsUKFinalDestinationCodeNull())
                    aDef.UKFinalDestinationCode = row.UKFinalDestinationCode;
                else
                    aDef.UKFinalDestinationCode = string.Empty;
                if (!row.IsUKFinalDestinationCountryIdNull())
                    aDef.UKFinalDestinationCountryId = row.UKFinalDestinationCountryId;
                else
                    aDef.UKFinalDestinationCountryId = GeneralCriteria.ALL;
                if (!row.IsUKFinalDestinationDescNull())
                    aDef.UKFinalDestinationDesc = row.UKFinalDestinationDesc;
                else
                    aDef.UKFinalDestinationDesc = string.Empty;
            }
        }
		private void CurrencyMapping(Object source, Object target)
		{
			if (source.GetType() == typeof(CurrencyDs.CurrencyRow) &&
				target.GetType() == typeof(CurrencyRef))
			{
				CurrencyDs.CurrencyRow row = (CurrencyDs.CurrencyRow)source;
				CurrencyRef def = (CurrencyRef) target;

				def.CurrencyId=row.CurrencyId;
				if (!row.IsCurrencyCodeNull()) def.CurrencyCode=row.CurrencyCode;
				if (!row.IsNameNull()) def.Name=row.Name;
			}
		}

        private void FileUploadMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(FileUploadDs.FileUploadRow) &&
                    target.GetType() == typeof(FileUploadDef))
            {
                FileUploadDs.FileUploadRow row = (FileUploadDs.FileUploadRow)source;
                FileUploadDef def = (FileUploadDef)target;

                def.FileId = row.FileId;
                if (!row.IsFileDescriptionNull())
                    def.FileDescription = row.FileDescription;
                if (!row.IsPhysicalFileNameNull())
                    def.PhysicalFileName = row.PhysicalFileName;
                if (!row.IsShipmentIdNull())
                    def.ShipmentId = row.ShipmentId;
                else
                    def.ShipmentId = -1;
                if (!row.IsClaimIdNull())
                    def.ClaimId = row.ClaimId;
                else
                    def.ClaimId = -1;
                def.Status = row.Status;

                def.CreatedBy = generalWorker.getUserByKey(row.CreatedBy);
                def.CreatedOn = row.CreatedOn;
            }
            else if (source.GetType() == typeof(FileUploadDef) &&
                target.GetType() == typeof(FileUploadDs.FileUploadRow))
            {
                FileUploadDs.FileUploadRow row = (FileUploadDs.FileUploadRow)target;
                FileUploadDef  def = (FileUploadDef)source;

                row.FileId = def.FileId;
                if (def.FileDescription != null)
                    row.FileDescription = def.FileDescription;
                else
                    row.SetFileDescriptionNull();
                if (def.PhysicalFileName != null)
                    row.PhysicalFileName = def.PhysicalFileName;
                else
                    row.SetPhysicalFileNameNull();
                if (def.ShipmentId > 0)
                    row.ShipmentId = def.ShipmentId;
                else
                    row.SetShipmentIdNull();
                if (def.ClaimId > 0)
                    row.ClaimId = def.ClaimId;
                else
                    row.SetClaimIdNull();
                row.Status = def.Status;
                row.CreatedBy = def.CreatedBy.UserId;
                row.CreatedOn = def.CreatedOn;
            }

        }

        #endregion
    }
}

