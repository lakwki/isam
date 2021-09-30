          SELECT MAX(r.RangePlanId) AS RangePlanId, MAX(r.ItemNo) AS ItemNo, MAX(r.Description) AS Description, MAX(u.Name) AS ProductTeamName, MAX(r.SeasonId) AS SeasonId,
          MAX(CAST(ISNULL(r.ActualSaleSeasonId,'999') AS varchar) + '-' + CAST(ISNULL(r.ActualSaleSeasonSplitId ,'999') AS varchar)) AS ActualSaleSeason,
          MAX(CAST(ISNULL(r.ExpectedSaleSeasonId,'999') AS varchar) + '-' + CAST(ISNULL(r.ExpectedSaleSeasonSplitId ,'999') AS varchar)) AS ExpectedSaleSeason,
          SUM(h.ShippedQty) AS TotalQty, SUM(h.ReducedSupplierGmtPrice * h.ShippedQty) AS TotalFOBCost, SUM(r.ActualFreightUSD * h.ShippedQty) AS ActualFreightUSD, MAX(g.BuyCurrencyId) AS SupplierCurrencyId,
          MIN(h.RetailSellingPrice) AS MinRetailSellingPrice, MAX(h.RetailSellingPrice) AS MaxRetailSellingPrice,
          SUM(h.ShippedQty * h.RetailSellingPrice * (0.7/(1+(ISNULL(d.VATRate,0)/100))*0.5 + 0.3*0.25/(1+(ISNULL(d.VATRate,0)/100))*0.6) ) AS TotalComm,
          SUM(FreightCPU * h.ShippedQty) AS TotalFreight, MAX(r.DutyPercent) AS DutyPercent, MAX(ISNULL(r.CustomerId, 1)) AS CustomerId,
          SUM(h.RetailSellingPrice * h.ShippedQty) AS TotalRetailAmt, MAX(r.ActualFreightUSD) AS ActualFreightUnitCostUSD,
          MIN(j.InvoiceDate) AS FirstInvoiceDate,
          Min(j.ShipmentId) AS FirstShipmentId,
		      r.OfficeId,
          MAX(ISNULL(ud.Description,'N/A')) AS UKDept,
          MAX(r.ActualLaunchDate) AS ActualLaunchedDate,
          MAX(r.PlannedLaunchDate) AS PlannedLaunchedDate,
          MAX(r.Comment) AS Comment,
          MAX(CASE WHEN sc.UKSupplierCode IS NULL THEN 0 ELSE 1 END) AS IsDuty,
          MIN(ISNULL(ex3.ExchangeRate, 0)) AS USDExchangeRate,
          MIN(ISNULL(ex2.ExchangeRate, 0)) AS GBPExchangeRate,
          MIN(ISNULL(ex1.ExchangeRate, 0)) AS SupplierCurrencyExchangeRate,
          ISNULL(SUM(h.ReducedSupplierGmtPrice * h.ShippedQty * ex1.ExchangeRate / ex3.ExchangeRate), 0) AS TotalFOBCostInUSD 
          FROM [RangePlan] r WITH (NOLOCK) 
          INNER JOIN [RangePlanLatestVersionId] v ON r.RangePlanId = v.RangePlanId AND r.WorkflowStatusId NOT IN (60) AND r.VersionId = v.VersionId 
				  AND  r.ItemNo =  '823848'
		      INNER JOIN [UKProductTeam] u WITH (NOLOCK) ON u.UKProductTeamId = r.UKProductTeamId
          INNER JOIN [NS-DB04].ISAM.dbo.Product e WITH (NOLOCK) ON e.ItemNo = r.ItemNo AND e.Status = 1
          INNER JOIN [NS-DB04].ISAM.dbo.Contract f WITH (NOLOCK) ON f.ProductId = e.ProductId AND f.Status = 1 AND f.CustomerId IN (27,28,99)
          INNER JOIN [NS-DB04].ISAM.dbo.Shipment g WITH (NOLOCK) ON f.ContractId = g.ContractId AND g.Status = 1 AND g.WorkflowStatusId = 8 AND g.IsMockShopSample = 0 AND g.IsPressSample = 0 AND g.IsStudioSample = 0
          INNER JOIN [NS-DB04].ISAM.dbo.Invoice j WITH (NOLOCK) ON g.ShipmentId = j.ShipmentId
          INNER JOIN [NS-DB04].ISAM.dbo.ShipmentDetail h WITH (NOLOCK) ON g.ShipmentId = h.ShipmentId AND h.Status = 1
          INNER JOIN [NS-DB04].ISAM.dbo.SizeOption i WITH (NOLOCK) ON i.SizeOptionId = h.SizeOptionId
          LEFT JOIN [UKDept] ud WITH (NOLOCK) ON ud.UKDeptId = r.UKDeptId
          LEFT JOIN [RangePlanDetail] d WITH (NOLOCK) ON d.RangePlanId = r.RangePlanId AND d.VersionId = r.VersionId AND d.Status = 1 AND LTRIM(RTRIM(d.SizeRange)) = LTRIM(RTRIM(i.SizeDesc)) AND h.SellingPrice = d.SupplierFOB
          LEFT JOIN [NS-DB04].ISAM.dbo.NSLedShipmentSeasonOverride seo ON seo.ShipmentId = g.ShipmentId
          LEFT JOIN [NS-DB04].ISAM.dbo.NSLedSelfBilledSupplierCode sc ON sc.UKSupplierCode = f.UKSupplierCode 
          LEFT JOIN [NS-DB04].ISAM.dbo.SunInterfaceLog sl WITH (NOLOCK) ON sl.SunInterfaceLogId = ISAM.dbo.fn_GetLatestActualSunInterfaceLogId(g.ShipmentId, 3)
          LEFT JOIN NSLDB.dbo.AccountFinancialCalender cal WITH (NOLOCK) ON cal.Period = sl.Period AND sl.FiscalYear = cal.BudgetYear AND cal.AppId = 13 AND cal.Status = 1
          LEFT JOIN [NS-DB04].ISAM.dbo.ExchangeRate ex3 WITH (NOLOCK) on ex3.CurrencyId = 3 AND ex3.ExchangeRateTypeId = 1 AND cal.StartDate BETWEEN ex3.EffectiveDateFrom AND ex3.EffectiveDateTo
          LEFT JOIN [NS-DB04].ISAM.dbo.ExchangeRate ex2 WITH (NOLOCK) on ex2.CurrencyId = 2 AND ex2.ExchangeRateTypeId = 1 AND cal.StartDate BETWEEN ex2.EffectiveDateFrom AND ex2.EffectiveDateTo
          LEFT JOIN [NS-DB04].ISAM.dbo.ExchangeRate ex1 WITH (NOLOCK) on ex1.CurrencyId = g.BuyCurrencyId AND ex1.ExchangeRateTypeId = 1 AND cal.StartDate BETWEEN ex1.EffectiveDateFrom AND ex1.EffectiveDateTo
          
          WHERE [ISAM].dbo.fn_isValidNSLedShipmentByItemSeasonDeliveryDate(r.ItemNo, g.CustomeratWarehouseDate, ISNULL(seo.SeasonId, f.SeasonId), r.SeasonId) = 1
          GROUP BY r.OfficeId, r.ItemNo


		  select 4503 * 9.1407824173 / 7.7497752565

		  select * from nsldb..officestructure where code = 'WJSPT'






		            SELECT  h.ShippedQty ,h.RetailSellingPrice,         h.ShippedQty * h.RetailSellingPrice * (0.7/(1+(ISNULL(d.VATRate,0)/100))*0.5 + 0.3*0.25/(1+(ISNULL(d.VATRate,0)/100))*0.6)  AS TotalComm
          FROM [RangePlan] r WITH (NOLOCK) 
          INNER JOIN [RangePlanLatestVersionId] v ON r.RangePlanId = v.RangePlanId AND r.WorkflowStatusId NOT IN (60) AND r.VersionId = v.VersionId 
				  AND  r.ItemNo =  '398388'
		      INNER JOIN [UKProductTeam] u WITH (NOLOCK) ON u.UKProductTeamId = r.UKProductTeamId
          INNER JOIN [NS-DB04].ISAM.dbo.Product e WITH (NOLOCK) ON e.ItemNo = r.ItemNo AND e.Status = 1
          INNER JOIN [NS-DB04].ISAM.dbo.Contract f WITH (NOLOCK) ON f.ProductId = e.ProductId AND f.Status = 1 AND f.CustomerId IN (27,28,99)
          INNER JOIN [NS-DB04].ISAM.dbo.Shipment g WITH (NOLOCK) ON f.ContractId = g.ContractId AND g.Status = 1 AND g.WorkflowStatusId = 8 AND g.IsMockShopSample = 0 AND g.IsPressSample = 0 AND g.IsStudioSample = 0
          INNER JOIN [NS-DB04].ISAM.dbo.Invoice j WITH (NOLOCK) ON g.ShipmentId = j.ShipmentId
          INNER JOIN [NS-DB04].ISAM.dbo.ShipmentDetail h WITH (NOLOCK) ON g.ShipmentId = h.ShipmentId AND h.Status = 1
          INNER JOIN [NS-DB04].ISAM.dbo.SizeOption i WITH (NOLOCK) ON i.SizeOptionId = h.SizeOptionId
          LEFT JOIN [UKDept] ud WITH (NOLOCK) ON ud.UKDeptId = r.UKDeptId
          LEFT JOIN [RangePlanDetail] d WITH (NOLOCK) ON d.RangePlanId = r.RangePlanId AND d.VersionId = r.VersionId AND d.Status = 1 AND LTRIM(RTRIM(d.SizeRange)) = LTRIM(RTRIM(i.SizeDesc)) AND h.SellingPrice = d.SupplierFOB
          LEFT JOIN [NS-DB04].ISAM.dbo.NSLedShipmentSeasonOverride seo ON seo.ShipmentId = g.ShipmentId
          LEFT JOIN [NS-DB04].ISAM.dbo.NSLedSelfBilledSupplierCode sc ON sc.UKSupplierCode = f.UKSupplierCode 
          LEFT JOIN [NS-DB04].ISAM.dbo.SunInterfaceLog sl WITH (NOLOCK) ON sl.SunInterfaceLogId = ISAM.dbo.fn_GetLatestActualSunInterfaceLogId(g.ShipmentId, 3)
          LEFT JOIN NSLDB.dbo.AccountFinancialCalender cal WITH (NOLOCK) ON cal.Period = sl.Period AND sl.FiscalYear = cal.BudgetYear AND cal.AppId = 13 AND cal.Status = 1
          LEFT JOIN [NS-DB04].ISAM.dbo.ExchangeRate ex3 WITH (NOLOCK) on ex3.CurrencyId = 3 AND ex3.ExchangeRateTypeId = 1 AND cal.StartDate BETWEEN ex3.EffectiveDateFrom AND ex3.EffectiveDateTo
          LEFT JOIN [NS-DB04].ISAM.dbo.ExchangeRate ex2 WITH (NOLOCK) on ex2.CurrencyId = 2 AND ex2.ExchangeRateTypeId = 1 AND cal.StartDate BETWEEN ex2.EffectiveDateFrom AND ex2.EffectiveDateTo
          LEFT JOIN [NS-DB04].ISAM.dbo.ExchangeRate ex1 WITH (NOLOCK) on ex1.CurrencyId = g.BuyCurrencyId AND ex1.ExchangeRateTypeId = 1 AND cal.StartDate BETWEEN ex1.EffectiveDateFrom AND ex1.EffectiveDateTo
          
          WHERE [ISAM].dbo.fn_isValidNSLedShipmentByItemSeasonDeliveryDate(r.ItemNo, g.CustomeratWarehouseDate, ISNULL(seo.SeasonId, f.SeasonId), r.SeasonId) = 1
