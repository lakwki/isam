          SELECT MAX(r.RangePlanId) AS RangePlanId, MAX(r.ItemNo) AS ItemNo, MAX(r.Description) AS Description, MAX(u.Name) AS ProductTeamName, MAX(r.SeasonId) AS SeasonId,
          MAX(CAST(ISNULL(r.ActualSaleSeasonId,'999') AS varchar) + '-' + CAST(ISNULL(r.ActualSaleSeasonSplitId ,'999') AS varchar)) AS ActualSaleSeason,
          SUM(h.ShippedQty) AS TotalQty, SUM(h.ReducedSupplierGmtPrice * h.ShippedQty) AS TotalFOBCost, SUM(r.ActualFreightUSD * h.ShippedQty) AS ActualFreightUSD, MAX(g.BuyCurrencyId) AS SupplierCurrencyId,
          MIN(h.RetailSellingPrice) AS MinRetailSellingPrice, MAX(h.RetailSellingPrice) AS MaxRetailSellingPrice,
          SUM(h.ShippedQty * h.RetailSellingPrice * (0.7/(1+(ISNULL(d.VATRate,0)/100))*0.5 + 0.3*0.25/(1+(ISNULL(d.VATRate,0)/100))*0.6) ) AS TotalComm,
          SUM(FreightCPU * h.ShippedQty) AS TotalFreight, MAX(r.DutyPercent) AS DutyPercent, MAX(ISNULL(r.CustomerId, 1)) AS CustomerId,
          SUM(h.RetailSellingPrice * h.ShippedQty) AS TotalRetailAmt, MAX(r.ActualFreightUSD) AS ActualFreightUnitCostUSD,
          MIN(j.InvoiceDate) AS FirstInvoiceDate
          FROM [RangePlan] r
          INNER JOIN [RangePlanLatestVersionId] v ON r.RangePlanId = v.RangePlanId AND r.VersionId = v.VersionId 
          INNER JOIN [UKProductTeam] u ON u.UKProductTeamId = r.UKProductTeamId
          INNER JOIN [NS-DB04].ISAM.dbo.Product e ON e.ItemNo = r.ItemNo AND e.Status = 1
          INNER JOIN [NS-DB04].ISAM.dbo.Contract f ON f.ProductId = e.ProductId AND f.Status = 1 AND f.CustomerId IN (27,28,99)
          INNER JOIN [NS-DB04].ISAM.dbo.Shipment g ON f.ContractId = g.ContractId AND g.Status = 1 AND g.WorkflowStatusId = 8 AND g.IsMockShopSample = 0 AND g.IsPressSample = 0 AND g.IsStudioSample = 0
          INNER JOIN [NS-DB04].ISAM.dbo.Invoice j ON g.ShipmentId = j.ShipmentId
          INNER JOIN [NS-DB04].ISAM.dbo.ShipmentDetail h ON g.ShipmentId = h.ShipmentId AND h.Status = 1
          INNER JOIN [NS-DB04].ISAM.dbo.SizeOption i ON i.SizeOptionId = h.SizeOptionId
          LEFT JOIN [RangePlanDetail] d ON d.RangePlanId = r.RangePlanId AND d.VersionId = r.VersionId AND d.Status = 1 AND d.SizeRange = i.SizeOptionNo
          GROUP BY r.OfficeId, r.ItemNo