        SELECT distinct 
        c.ProductTeamId
        FROM Invoice a
        INNER JOIN Shipment b ON a.ShipmentId = b.ShipmentId AND b.Status = 1 

        INNER JOIN Contract c ON b.ContractId = c.ContractId AND c.OfficeId IN (SELECT OfficeId FROM ReportOfficeGroupMapping WHERE OfficeGroupId = 3) AND c.Status = 1

                                 AND NOT (c.CustomerId IN (20, 24) AND b.IsMockShopSample = 0 AND b.IsStudioSample = 0)
        INNER JOIN Product d ON c.ProductId = d.ProductId AND d.ItemNo NOT IN ('000000')
        INNER JOIN NSLDB..AccountFinancialCalender x ON x.AppId = 9 AND GETDATE() BETWEEN x.StartDate AND x.EndDate 
        INNER JOIN DailySunInterface e ON e.SunInterfaceTypeId = 3 AND e.IsActive = 1 AND e.ShipmentId = a.ShipmentId AND e.SplitShipmentId = 0
        INNER JOIN SystemParameter sp ON sp.ParameterId = 12  
        LEFT JOIN ExchangeRate y ON y.CurrencyId = 3 AND y.ExchangeRateTypeId = 1 AND x.StartDate BETWEEN y.EffectiveDateFrom AND y.EffectiveDateTo
        LEFT JOIN ExchangeRate z ON z.CurrencyId = 4 AND z.ExchangeRateTypeId = 1 AND x.StartDate BETWEEN z.EffectiveDateFrom AND z.EffectiveDateTo
        LEFT JOIN ExchangeRate zc ON zc.CurrencyId = b.BuyCurrencyId AND zc.ExchangeRateTypeId = 1 AND x.StartDate BETWEEN zc.EffectiveDateFrom AND zc.EffectiveDateTo
        LEFT JOIN CutOffSales co ON a.ShipmentId = co.ShipmentId 
		where c.ProductTeamId not in (select officestructureid from nsldb..officestructureaccountcode)


		221
261
262
NULL
263
264
342
231
211
267
232

		select * from nsldb..officestructureaccountcode a inner join nsldb..officestructure b on a.officestructureid = b.officestructureid and a.t1 = '232'

		select  * from nsldb..officestructure where code like 'LMTB%'

		select distinct t1 from nsldb..officestructureaccountcode 

		select * from nsldb..officestructure where officestructureid = 83192
				select * from nsldb..officestructureaccountcode where officestructureid = 83191


				LMTBWNTY

		select * from nsldb..officestructure where officestructureid = 89172
				select * from nsldb..officestructureaccountcode where officestructureid =82191

				insert nsldb01.nsldb.dbo.officestructureaccountcode
		select officestructureid, '232', 1, 616, getdate(), null, null from nsldb..officestructure where officestructuretypeid = 50 and status = 1 and code in ( 'LMTBWNTY','LMTBWLIN') and officestructureid not in (select officestructureid from nsldb..officestructureaccountcode)


		select * from nsldb..officestructureaccountcode where officestructureid in (
		81191,
81192,
82191,
82192,
83191,
83192,
84191,
84192,
87191,
87192,
88191,
88192,
89191,
89192,
810191,
810192,
813191,
813192,
814191,
814192,
816191,
816192,
818191,
818192,
819191,
819192)
