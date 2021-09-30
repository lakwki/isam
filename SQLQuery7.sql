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
SELECT s.ItemNo, f.OfficeId, 
CASE WHEN SUM((CASE WHEN CommPercent = 40 AND DespatchQty > 0 THEN 1 ELSE 0 END)) > 0 THEN 1 ELSE 0 END AS IsEndOfLife, 

--This is the update for ultimate week count from the start of the product sell to selected week. 
CASE 
WHEN 2020 != MIN(f.FiscalYear) THEN 
(MIN(f.FiscalYear) * 100 + 52) 
- CAST(MIN(CAST(f.FiscalYear AS varchar) + RIGHT('0' + CAST(f.FiscalWeek AS varchar),2)) AS INT) + 1 
+ 
CAST(CAST(2020 AS varchar) + RIGHT('0' + CAST(26 AS varchar),2) AS INT) 
- (2020 * 100 + 1) + 1 
+ (2020 - MIN(f.FiscalYear) - 1) * 52 
ELSE 
CAST(CAST(2020 AS varchar) + RIGHT('0' + CAST(26 AS varchar),2) AS INT) 
- 
CAST(MIN(CAST(f.FiscalYear AS varchar) + RIGHT('0' + CAST(f.FiscalWeek AS varchar),2)) AS INT) 
+ 1 
END AS WeekCount, 


CASE 
WHEN ISNULL(CONVERT(int,LEFT(md.WeekStr,4)), 2020) != MIN(f.FiscalYear) THEN 
(MIN(f.FiscalYear) * 100 + 52) 
- CAST(MIN(CAST(f.FiscalYear AS varchar) + RIGHT('0' + CAST(f.FiscalWeek AS varchar),2)) AS INT) + 1 
+ 
CAST(ISNULL(md.WeekStr, CAST(2020 AS varchar) + RIGHT('0' + CAST(26 AS varchar),2)) AS INT) 
- (ISNULL(CONVERT(int,LEFT(md.WeekStr,4)), 2020) * 100 + 1) 
+ (ISNULL(CONVERT(int,LEFT(md.WeekStr,4)), 2020) - MIN(f.FiscalYear) - 1) * 52 
ELSE 
CAST(ISNULL(md.WeekStr, CAST(2020 AS varchar) + RIGHT('0' + CAST(26 AS varchar),2)) AS INT) 
- 
CAST(MIN(CAST(f.FiscalYear AS varchar) + RIGHT('0' + CAST(f.FiscalWeek AS varchar),2)) AS INT) 
END AS FPWeekCount, 


SUM(NetQty) AS QtySold, 
SUM(CASE WHEN CommPercent = 50 THEN NetQty ELSE 0 END) AS FPQty, 
SUM(CASE WHEN CountryOfSale IN ('UNITED KINGDOM', 'GB') THEN NetQty ELSE 0 END) AS UKQty, 
SUM(NSCommAmtInUSD) AS TotalNSCommissionAmt, 
CONVERT(varchar(6), MIN(CAST(f.FiscalYear AS varchar) + RIGHT('0' + CAST(f.FiscalWeek AS varchar),2))) AS InitialSaleWeek, 
tempS.SeasonCount AS SeasonCount, 
CONVERT(bit,MIN(CONVERT(int, f.IsDutiable))) AS HasDuty, 
MIN(f.FiscalYear) AS LaunchYear, 
CONVERT(int, RIGHT(MIN(CAST(f.FiscalYear AS varchar) + RIGHT('0' + CAST(f.FiscalWeek AS varchar),2)), 2)) AS LaunchWeek 

FROM [NSLedSales] s 
JOIN ( 
SELECT DISTINCT ItemNo 
FROM [NSLedSales] s 
JOIN [NSLedImportFile] f ON s.FileId = f.FileId 
WHERE (f.OfficeId NOT IN (-100)) AND f.FiscalYear = 2020 AND f.FiscalWeek = 26 AND s.ItemNo = '848244' 
) temp ON temp.ItemNo = s.ItemNo AND (s.DespatchQty <> 0 OR s.ReturnQty <> 0) 
JOIN [NSLedImportFile] f ON s.FileId = f.FileId 
AND CAST(f.FiscalYear AS varchar) + RIGHT('0' + CAST(f.FiscalWeek AS varchar),2) <= CAST(2020 AS varchar) + RIGHT('0' + CAST(26 AS varchar),2) 
LEFT JOIN vw_FirstNSLedMDWeek md ON md.ItemNo = s.ItemNo AND f.OfficeId = md.OfficeId 
LEFT JOIN (SELECT p.ItemNo, COUNT(distinct c.SeasonId) AS SeasonCount 
FROM [Product] p 
JOIN [Contract] c ON c.ProductId = p.ProductId AND c.CustomerId IN (27,28,99) 
GROUP BY p.ItemNo) tempS ON tempS.ItemNo = temp.ItemNo 
GROUP BY s.ItemNo, f.OfficeId, tempS.SeasonCount, md.WeekStr 

--HAVING @NSLedPhaseId = -1 OR ((CASE WHEN SUM((CASE WHEN CommPercent = 40 AND DespatchQty > 0 THEN 1 ELSE 0 END)) > 0 THEN 1 ELSE 0 END = 1) AND @NSLedPhaseId >= CASE WHEN (MAX(ActualSaleSeasonId) 

UNION ALL 
SELECT DISTINCT c.ItemNo, a.OfficeId, 0 AS IsEndOfLife, 0 AS WeekCount, 0 AS FPWeekCount, 0 AS QtySold, 0 AS FPQty, 0 AS UKQty, 0 AS TotalNSCommissionAmt, 0 AS InitialSaleWeek, 0 AS SeasonCount, 
0 AS HasDuty, 0 AS LaunchYear, 0 AS LaunchWeek 
FROM Contract a INNER JOIN Shipment b ON a.ContractId = b.ContractId AND a.Status = 1 AND b.Status = 1 AND b.WorkflowStatusid = 8 AND a.CustomerId = 27 
AND (a.OfficeId NOT IN (-100)) 
INNER JOIN Product c ON a.ProductId = c.ProductId 
LEFT JOIN NSLedSales d ON c.ItemNo = d.ItemNo 
LEFT JOIN NSLedImportFile e ON e.FileId = d.FileId 

WHERE (d.FileId IS NULL OR 
NOT EXISTS(SELECT * FROM NSLedSales x INNER JOIN NSLedImportFile y ON x.ItemNo = d.ItemNo AND x.FileId = y.FileId AND CAST(y.FiscalYear AS varchar) + RIGHT('0' + CAST(y.FiscalWeek AS varchar),2) <= CAST(2020 AS varchar) + RIGHT('0' + CAST(26 AS varchar), 2))) 

) z 
--WHERE z.ItemNo IN ('124711','383184','818469', '295108') 
ORDER BY OfficeSortId, IsEndOfLife DESC, WeekCount DESC, ItemNo 
