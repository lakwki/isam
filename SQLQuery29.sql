SELECT *, 
CASE OfficeId WHEN 1 THEN 1 
WHEN 19 THEN 2 
WHEN 2 THEN 3 
WHEN 16 THEN 4 
WHEN 7 THEN 5 
WHEN 9 THEN 6 
WHEN 3 THEN 7 
WHEN 8 THEN 8 
WHEN 13 THEN 9 
ELSE 99 END AS OfficeSortId 
FROM ( 
SELECT s.ItemNo, s.OfficeId, 
CASE WHEN SUM((CASE WHEN CommPercent = 40 AND DespatchQty > 0 THEN 1 ELSE 0 END)) > 0 THEN 1 ELSE 0 END AS IsEndOfLife, 

--This is the update for ultimate week count from the start of the product sell to selected week. 
CASE 
WHEN ISNULL(rpm.FiscalYear, 2020) != MIN(s.FiscalYear) THEN 
(MIN(s.FiscalYear) * 100 + 52) 
- CAST(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)) AS INT) + 1 
+ 
CAST(ISNULL(rpm.WeekStr, CAST(2020 AS varchar) + RIGHT('0' + CAST(53 AS varchar),2)) AS INT) 
- (ISNULL(rpm.FiscalYear, 2020) * 100 + 1) + 1 
+ (ISNULL(rpm.FiscalYear, 2020) - MIN(s.FiscalYear) - 1) * 52 
ELSE 
CAST(ISNULL(rpm.WeekStr, CAST(2020 AS varchar) + RIGHT('0' + CAST(53 AS varchar),2)) AS INT) 
- 
CAST(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)) AS INT) 
+ 1 
END AS WeekCount, 


CASE 
WHEN ISNULL(ISNULL(md.FiscalYear, rpm.FiscalYear), 2020) != MIN(s.FiscalYear) THEN 
(MIN(s.FiscalYear) * 100 + 52) 
- CAST(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)) AS INT) + 1 
+ 
CAST(ISNULL(ISNULL(md.LastFPWeekStr, rpm.WeekStr), CAST(2020 AS varchar) + RIGHT('0' + CAST(53 AS varchar),2)) AS INT) 
- (ISNULL(ISNULL(md.FiscalYear, rpm.FiscalYear), 2020) * 100 + 1) + 1 
+ (ISNULL(ISNULL(md.FiscalYear, rpm.FiscalYear), 2020) - MIN(s.FiscalYear) - 1) * 52 
ELSE 
CAST(ISNULL(ISNULL(md.LastFPWeekStr, rpm.WeekStr), CAST(2020 AS varchar) + RIGHT('0' + CAST(53 AS varchar),2)) AS INT) 
- 
CAST(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)) AS INT) 
+ 1 
END AS FPWeekCount, 


SUM(NetQty) AS QtySold, 
SUM(CASE WHEN CommPercent = 50 THEN NetQty ELSE 0 END) AS FPQty, 
SUM(CASE WHEN s.IsUK = 1 THEN NetQty ELSE 0 END) AS UKQty, 
SUM(NSCommAmtInUSD) AS TotalNSCommissionAmt, 
CONVERT(varchar(6), MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2))) AS InitialSaleWeek, 

1 AS SeasonCount, 
rp.SeasonId AS SeasonId, 

CONVERT(bit,MIN(CONVERT(int, s.IsDutiable))) AS HasDuty, 
MIN(s.FiscalYear) AS LaunchYear, 
CONVERT(int, RIGHT(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)), 2)) AS LaunchWeek 

FROM [NSLedSalesSummary] s 
INNER JOIN [NS-LED]..RangePlan rp ON rp.ItemNo = s.ItemNo AND rp.Status = 1 AND rp.IsRepeatItem = 0 AND (-1 = -1 OR rp.UKProductTeamId = -1) AND rp.WorkflowStatusId NOT IN (40, 60) 
AND s.OfficeId NOT IN (-100) 
AND (s.FiscalYear * 100 + s.FiscalWeek) <= (2020 * 100 + 53) 
AND (NOT EXISTS(SELECT * FROM NSLedRepeatItemParam WHERE ItemNo = s.ItemNo) OR (s.FiscalYear * 100 + s.FiscalWeek) < dbo.fn_GetNSLedRepeatItemMinFiscalWeek(s.ItemNo)) 

AND [NS-LED].dbo.fn_getNSLedPhaseId(rp.ActualSaleSeasonId, rp.ActualSaleSeasonSplitId) IN (-1,13,12,11,10,9,8,7,6,5,4,3,2,1) 
INNER JOIN [NS-LED]..RangePlanLatestVersionId rpv ON rp.RangePlanId = rpv.RangePlanId AND rp.VersionId = rpv.VersionId 
CROSS APPLY dbo.fn_GetMinWeekFromNSLedRepeatItem(s.ItemNo, s.FiscalYear, s.FiscalWeek) rpm 
LEFT JOIN NSLedFirstMDWeek md ON md.ItemNo = s.ItemNo AND s.OfficeId = md.OfficeId AND md.Seasonid = rp.SeasonId 

GROUP BY s.ItemNo, s.OfficeId, md.LastFPWeekStr, rp.SeasonId, md.FiscalYear, rpm.WeekStr, rpm.FiscalYear 
--HAVING s.ItemNo = '478941' 


UNION ALL 

SELECT s.ItemNo, s.OfficeId, 
CASE WHEN SUM((CASE WHEN CommPercent = 40 AND DespatchQty > 0 THEN 1 ELSE 0 END)) > 0 THEN 1 ELSE 0 END AS IsEndOfLife, 

--This is the update for ultimate week count from the start of the product sell to selected week. 
CASE 
WHEN 2020 != MIN(s.FiscalYear) THEN 
(MIN(s.FiscalYear) * 100 + 52) 
- CAST(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)) AS INT) + 1 
+ 
CAST(CAST(2020 AS varchar) + RIGHT('0' + CAST(53 AS varchar),2) AS INT) 
- (2020 * 100 + 1) + 1 
+ (2020 - MIN(s.FiscalYear) - 1) * 52 
ELSE 
CAST(CAST(2020 AS varchar) + RIGHT('0' + CAST(53 AS varchar),2) AS INT) 
- 
CAST(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)) AS INT) 
+ 1 
END AS WeekCount, 


CASE 
WHEN ISNULL(md.FiscalYear, 2020) != MIN(s.FiscalYear) THEN 
(MIN(s.FiscalYear) * 100 + 52) 
- CAST(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)) AS INT) + 1 
+ 
CAST(ISNULL(md.LastFPWeekStr, CAST(2020 AS varchar) + RIGHT('0' + CAST(53 AS varchar),2)) AS INT) 
- (ISNULL(md.FiscalYear, 2020) * 100 + 1) + 1 
+ (ISNULL(md.FiscalYear, 2020) - MIN(s.FiscalYear) - 1) * 52 
ELSE 
CAST(ISNULL(md.LastFPWeekStr, CAST(2020 AS varchar) + RIGHT('0' + CAST(53 AS varchar),2)) AS INT) 
- 
CAST(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)) AS INT) 
+ 1 
END AS FPWeekCount, 


SUM(NetQty) AS QtySold, 
SUM(CASE WHEN CommPercent = 50 THEN NetQty ELSE 0 END) AS FPQty, 
SUM(CASE WHEN s.IsUK = 1 THEN NetQty ELSE 0 END) AS UKQty, 
SUM(NSCommAmtInUSD) AS TotalNSCommissionAmt, 
CONVERT(varchar(6), MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2))) AS InitialSaleWeek, 

2 AS SeasonCount, 
rp.SeasonId AS SeasonId, 

CONVERT(bit,MIN(CONVERT(int, s.IsDutiable))) AS HasDuty, 
MIN(s.FiscalYear) AS LaunchYear, 
CONVERT(int, RIGHT(MIN(CAST(s.FiscalYear AS varchar) + RIGHT('0' + CAST(s.FiscalWeek AS varchar),2)), 2)) AS LaunchWeek 

FROM [NSLedSalesSummary] s 
INNER JOIN [NS-LED]..RangePlan rp ON rp.ItemNo = s.ItemNo AND rp.Status = 1 AND rp.IsRepeatItem = 1 AND (-1 = -1 OR rp.UKProductTeamId = -1) AND rp.WorkflowStatusId NOT IN (40, 60) 
AND s.OfficeId NOT IN (-100) 
AND (s.FiscalYear * 100 + s.FiscalWeek) <= (2020 * 100 + 53) 

AND [NS-LED].dbo.fn_getNSLedPhaseId(rp.ActualSaleSeasonId, rp.ActualSaleSeasonSplitId) IN (-1,13,12,11,10,9,8,7,6,5,4,3,2,1) 
INNER JOIN [NS-LED]..RangePlanLatestVersionId rpv ON rp.RangePlanId = rpv.RangePlanId AND rp.VersionId = rpv.VersionId 
INNER JOIN NSLedRepeatItemParam rip ON rip.ItemNo = s.ItemNo AND rip.SeasonId = rp.SeasonId AND (s.FiscalYear * 100 + s.FiscalWeek) >= (rip.FiscalYear * 100 + rip.FiscalWeek) 
LEFT JOIN NSLedFirstMDWeek md ON md.ItemNo = s.ItemNo AND s.OfficeId = md.OfficeId AND md.Seasonid = rp.SeasonId 

GROUP BY s.ItemNo, s.OfficeId, md.LastFPWeekStr, rp.SeasonId, md.FiscalYear 


UNION ALL 
SELECT DISTINCT c.ItemNo, a.OfficeId, 0 AS IsEndOfLife, 0 AS WeekCount, 0 AS FPWeekCount, 0 AS QtySold, 0 AS FPQty, 0 AS UKQty, 0 AS TotalNSCommissionAmt, 0 AS InitialSaleWeek, 0 AS SeasonCount, 0 AS SeasonId, 
0 AS HasDuty, 0 AS LaunchYear, 0 AS LaunchWeek 
FROM Contract a INNER JOIN Shipment b ON a.ContractId = b.ContractId AND a.Status = 1 AND b.Status = 1 AND b.WorkflowStatusid = 8 AND a.CustomerId IN (27, 28) 
AND (a.OfficeId NOT IN (-100)) 
INNER JOIN Invoice p ON p.ShipmentId = b.ShipmentId AND NOT(a.CustomerId = 28 AND YEAR(p.InvoiceDate) < 2019) 
INNER JOIN Product c ON a.ProductId = c.ProductId 
LEFT JOIN NSLedSalesSummary d ON c.ItemNo = d.ItemNo 

WHERE (d.ItemNo IS NULL OR 
NOT EXISTS(SELECT * FROM NSLedSalesSummary y WHERE ItemNo = c.ItemNo AND y.[Week] <= (2020 * 100 + 53))) 

UNION ALL 
SELECT DISTINCT c.ItemNo, a.OfficeId, 0 AS IsEndOfLife, 0 AS WeekCount, 0 AS FPWeekCount, 0 AS QtySold, 0 AS FPQty, 0 AS UKQty, 0 AS TotalNSCommissionAmt, 0 AS InitialSaleWeek, 2 AS SeasonCount, r.SeasonId AS SeasonId, 
0 AS HasDuty, 0 AS LaunchYear, 0 AS LaunchWeek 
FROM NSLedRepeatItemParam r INNER JOIN Product c ON r.ItemNo = c.ItemNo 
INNER JOIN Contract a ON a.ProductId = c.ProductId 
AND a.OfficeId NOT IN (-100) 
INNER JOIN Shipment b ON a.ContractId = b.ContractId AND a.Status = 1 AND b.Status = 1 AND b.WorkflowStatusid = 8 AND a.CustomerId IN (27, 28) 
INNER JOIN Invoice p ON p.ShipmentId = b.ShipmentId AND NOT(a.CustomerId = 28 AND YEAR(p.InvoiceDate) < 2019) 
LEFT JOIN NSLedSalesSummary d ON c.ItemNo = d.ItemNo 

WHERE (d.ItemNo IS NULL OR 
NOT EXISTS(SELECT * FROM NSLedSalesSummary y WHERE ItemNo = c.ItemNo AND y.[Week] <= (2020 * 100 + 53) AND y.[Week] >= (r.FiscalYear * 100 + r.FiscalWeek))) 

) z 
WHERE z.ItemNo IN ('311620') 
ORDER BY 3
