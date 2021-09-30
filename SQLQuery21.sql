
SELECT MAX(r.RangePlanId) AS RangePlanId, MAX(r.ItemNo) AS ItemNo, MAX(r.Description) AS Description, MAX(u.Name) AS ProductTeamName, MAX(r.SeasonId) AS SeasonId, 
MAX(CAST(ISNULL(r.ActualSaleSeasonId,'999') AS varchar) + '-' + CAST(ISNULL(r.ActualSaleSeasonSplitId ,'999') AS varchar)) AS ActualSaleSeason, 
SUM(h.ShippedQty) AS TotalQty, SUM(h.ReducedSupplierGmtPrice * h.ShippedQty) AS TotalFOBCost, SUM(r.ActualFreightUSD * h.ShippedQty) AS ActualFreightUSD, MAX(g.BuyCurrencyId) AS SupplierCurrencyId, 
MIN(h.RetailSellingPrice) AS MinRetailSellingPrice, MAX(h.RetailSellingPrice) AS MaxRetailSellingPrice, 
SUM(h.ShippedQty * h.RetailSellingPrice * (0.7/(1+(ISNULL(d.VATRate,0)/100))*0.5 + 0.3*0.25/(1+(ISNULL(d.VATRate,0)/100))*0.6) ) AS TotalComm, 
SUM(FreightCPU * h.ShippedQty) AS TotalFreight, MAX(r.DutyPercent) AS DutyPercent, MAX(ISNULL(r.CustomerId, 1)) AS CustomerId, 
SUM(h.RetailSellingPrice * h.ShippedQty) AS TotalRetailAmt, MAX(r.ActualFreightUSD) AS ActualFreightUnitCostUSD, 
MIN(j.InvoiceDate) AS FirstInvoiceDate 
,r.OfficeId, 
MAX(ISNULL(ud.Description,'N/A')) AS UKDept 
FROM [RangePlan] r 
INNER JOIN [RangePlanLatestVersionId] v ON r.RangePlanId = v.RangePlanId AND r.VersionId = v.VersionId AND (r.SeasonId = -1 OR (-1 = -1 AND IsRepeatItem = 0)) 
AND (r.OfficeId IN (3)) 
AND ('' = '' OR r.ItemNo = '') 
INNER JOIN [UKProductTeam] u ON u.UKProductTeamId = r.UKProductTeamId 
INNER JOIN [NS-DB04].ISAM.dbo.Product e ON e.ItemNo = r.ItemNo AND e.Status = 1 
INNER JOIN [NS-DB04].ISAM.dbo.Contract f ON f.ProductId = e.ProductId AND f.Status = 1 AND f.CustomerId IN (27,28,99) 
INNER JOIN [NS-DB04].ISAM.dbo.Shipment g ON f.ContractId = g.ContractId AND g.Status = 1 AND g.WorkflowStatusId = 8 AND g.IsMockShopSample = 0 AND g.IsPressSample = 0 AND g.IsStudioSample = 0 
INNER JOIN [NS-DB04].ISAM.dbo.Invoice j ON g.ShipmentId = j.ShipmentId 
AND [ISAM].dbo.fn_isValidNSLedShipmentByItemSeasonDeliveryDate(r.ItemNo, g.CustomeratWarehouseDate, f.SeasonId, r.SeasonId ) = 1 
INNER JOIN [NS-DB04].ISAM.dbo.ShipmentDetail h ON g.ShipmentId = h.ShipmentId AND h.Status = 1 
INNER JOIN [NS-DB04].ISAM.dbo.SizeOption i ON i.SizeOptionId = h.SizeOptionId 
LEFT JOIN [UKDept] ud ON ud.UKDeptId = r.UKDeptId 
LEFT JOIN [RangePlanDetail] d ON d.RangePlanId = r.RangePlanId AND d.VersionId = r.VersionId AND d.Status = 1 AND d.SizeRange = i.SizeOptionNo 
GROUP BY r.OfficeId, r.ItemNo 
HAVING (MAX(dbo.fn_getNSLedPhaseId(r.ActualSaleSeasonId, r.ActualSaleSeasonSplitId)) IN (-1,9,8,7,6,5,4,3,2,1)) 



sp_helptext fn_isValidNSLedShipmentByItemSeasonDeliveryDate

 DECLARE @min_item_delivery_date datetime  
 DECLARE @min_item_season_id datetime  
 DECLARE @min_season_id int  
 DECLARE @b bit  
 SELECT @min_item_delivery_date = MIN(DeliveryDate), @min_item_season_id = MIN(SeasonId) FROM NSLedRepeatItemParam WHERE ItemNo = @itemNo GROUP BY ItemNo  

 SELECT  MIN(DeliveryDate), MIN(SeasonId) FROM NSLedRepeatItemParam WHERE ItemNo = '478941' GROUP BY ItemNo  

 SELECT  MIN(SeasonId) FROM NSLedRepeatItemParam WHERE ItemNo = '478941' AND '2019-09-01' >= DeliveryDate GROUP BY ItemNo      
 SET @min_item_season_id = ISNULL(@min_item_season_id,0)  
 SET @min_season_id = ISNULL(@min_season_id,0)  
  
 IF (@min_item_season_id = 0   
  OR (@min_season_id > 0 AND @p_seasonId = @min_season_id)  
  OR (@min_item_season_id > 0 AND @seasonId < dbo.fn_getMinNSLedRepeatItemSeasonId(@itemNo) AND @seasonId = @p_seasonId))  
   SET @b = 1  
 ELSE  
   SET @b = 0  
 RETURN @b  
  END     


   select dbo.fn_getMinNSLedRepeatItemSeasonId('478941')