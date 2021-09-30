SELECT a.ClaimId, a.Amount INTO #mike
FROM UKClaim a INNER JOIN UKClaimRefund b ON a.ClaimId = b.ClaimId And a.Status = 1 AND b.Status = 1 AND a.HasUKDN = 0 AND a.VendorId NOT IN (3933) 
INNER JOIN UKClaimDCNoteDetail c ON b.ClaimId = c.ClaimId AND b.ClaimRefundId = c.ClaimRefundId AND c.Status = 1 
INNER JOIN UKClaimDCNote d ON c.DCNoteId = d.DCNoteId AND d.Status = 1 AND d.DCNoteNo IS NOT NULL AND d.DCNoteDate <= '2020-08-05 00:00:00' 
INNER JOIN UKClaimDCNoteDetail e ON a.ClaimId = e.ClaimId AND e.ClaimRefundId = 0 AND e.Status = 1 
INNER JOIN UKClaimDCNote f ON e.DCNoteId = f.DCNoteId AND f.Status = 1 AND f.DCNoteNo IS NOT NULL AND f.DCNoteDate <= '2020-08-05 00:00:00' 
GROUP BY a.ClaimId, a.Amount 
HAVING a.Amount = SUM(b.Amount) 

SELECT * FROM 
( 
SELECT DISTINCT a.ClaimId AS ClaimId, a.ClaimTypeId, a.ClaimMonth, a.ItemNo, a.ContractNo, a.OfficeId, a.HandlingOfficeId, a.VendorId, a.SZVendorId, a.ProductTeamId, a.TermOfPurchaseId, a.Qty, a.CurrencyId, a.Amount AS Amount, a.HasUKDN, 
a.UKDebitNoteNo, a.UKDebitNoteDate, a.UKDebitNoteReceivedDate, a.Remark, a.ClaimRequestId, a.DebitNoteNo, a.DebitNoteDate, a.DebitNoteAmt, a.IsInterfaced, 
a.IsRechargeInterfaced, a.WorkflowStatusId, LEFT(v.Name, 100) AS GUId, a.IsReadyForSettlement, a.SettlementOptionId, 
CASE WHEN bm.ParentId IS NULL THEN CONVERT(VARCHAR, a.ClaimId) ELSE CONVERT(varchar, bm.ParentId) + '-' END AS PnLAccountCode, a.PaymentOfficeId, 
a.Status, a.CreatedOn, a.CreatedBy, a.ModifiedOn, a.ModifiedBy 
FROM UKClaim a 
INNER JOIN NSLIndustry..Vendor v ON a.VendorId = v.VendorId AND a.VendorId NOT IN (3933) 
LEFT JOIN UKClaimDCNoteDetail b ON a.ClaimId = b.ClaimId AND b.ClaimRefundId = 0 AND b.Status = 1 
LEFT JOIN UKClaimDCNote c ON b.DCNoteId = c.DCNoteId AND c.Status = 1 AND a.WorkflowStatusId IN (5, 9) 
LEFT JOIN UKClaimBIAMapping bm ON a.ClaimId = bm.ClaimId AND bm.Status = 1 
LEFT JOIN [NS-DB01].QCIS.DBO.ClaimRequest as q ON q.RequestId=a.ClaimRequestId 
WHERE ('2020-08-05 00:00:00' >= ISNULL(CASE WHEN a.ClaimTypeId = 9 THEN CONVERT(DATETIME, CONVERT(VARCHAR, a.CreatedOn, 112)) ELSE a.UKDebitNoteReceivedDate END, '1990-01-01') AND (c.DCNoteDate IS NULL OR c.DCNoteDate > '2020-08-05 00:00:00' OR (a.HasUKDN = 0 AND NOT EXISTS(SELECT * FROM UKClaimBIADiscrepancy WHERE ClaimId = a.ClaimId AND IsLocked = 1 AND Status = 1)) OR EXISTS(SELECT * FROM UKClaimBIADiscrepancy ad INNER JOIN UKClaimBIAMapping am ON ad.ClaimId = am.ParentId WHERE am.ClaimId = a.ClaimId AND ad.IsLocked = 0 AND ad.Status = 1 AND am.Status = 1))) 
AND (a.OfficeId IN (SELECT OfficeId FROM ReportOfficeGroupMapping WHERE OfficeGroupId = 19) OR 19 = -1) 
AND (4972 = -1 OR a.VendorId = 4972) 
AND (-1 = -1 OR a.OfficeId <> 17 OR a.HandlingOfficeId = -1) 
AND (a.TermOfPurchaseId = -1 OR -1 = -1) 
--AND ISNULL(c.IsInterfaced, 0) = 0 
--AND a.ClaimId NOT IN (SELECT ClaimId FROM vw_FullUKClaimRefund_OSRpt) 
--AND a.WorkflowStatusId NOT IN (9) 
AND a.ClaimId NOT IN (SELECT ClaimId FROM #mike) 
AND a.Status = 1 
--AND a.ProductTeamId IN (SELECT OfficeStructureId FROM vw_HomeProductTeamId) 
--AND (-1 IN (-1,1)) 
AND (q.WorkflowStatusId IS NULL OR q.WorkflowStatusId NOT IN (-100)) 

UNION ALL 

SELECT DISTINCT a.ClaimRefundId * -1 AS ClaimId, b.ClaimTypeId, b.ClaimMonth, b.ItemNo, b.ContractNo, b.OfficeId, b.HandlingOfficeId, b.VendorId, b.SZVendorId, b.ProductTeamId, b.TermOfPurchaseId, b.Qty, b.CurrencyId, a.Amount * -1 AS Amount, b.HasUKDN, 
b.UKDebitNoteNo, b.UKDebitNoteDate, CASE WHEN b.UKDebitNoteReceivedDate IS NULL THEN NULL ELSE a.ReceivedDate END AS UKDebitNoteReceivedDate, a.Remark, b.ClaimRequestId, b.DebitNoteNo, b.DebitNoteDate, b.DebitNoteAmt, b.IsInterfaced, 
b.IsRechargeInterfaced, b.WorkflowStatusId, LEFT(v.Name, 100) AS GUId, a.IsReadyForSettlement, a.SettlementOptionId, CONVERT(VARCHAR, a.ClaimId) + '-' AS PnLAccountCode, b.PaymentOfficeId, b.Status, a.CreatedOn, a.CreatedBy, a.ModifiedOn, a.ModifiedBy 
FROM UKClaimRefund a INNER JOIN UKClaim b ON a.ClaimId = b.ClaimId AND a.Status = 1 AND b.Status = 1 AND b.VendorId NOT IN (3933) 
INNER JOIN NSLIndustry..Vendor v ON b.VendorId = v.VendorId 
LEFT JOIN UKClaimDCNoteDetail z ON a.ClaimId = z.ClaimId AND z.ClaimRefundId = a.ClaimRefundId AND z.Status = 1 
LEFT JOIN UKClaimDCNote c ON z.DCNoteId = c.DCNoteId AND c.Status = 1 AND b.WorkflowStatusId IN (5, 9) 
LEFT JOIN [NS-DB01].QCIS.DBO.ClaimRequest as q ON q.RequestId=b.ClaimRequestId 
WHERE (b.OfficeId IN (SELECT OfficeId FROM ReportOfficeGroupMapping WHERE OfficeGroupId = 19) OR 19 = -1) 
AND (-1 = -1 OR b.OfficeId <> 17 OR b.HandlingOfficeId = -1) 
AND (4972 = -1 OR b.VendorId = 4972) 
AND (b.TermOfPurchaseId = -1 OR -1 = -1) 
AND ('2020-08-05 00:00:00' >= ISNULL(CASE WHEN CASE WHEN b.ClaimTypeId = 9 THEN CONVERT(DATETIME, CONVERT(VARCHAR, b.CreatedOn, 112)) ELSE b.UKDebitNoteReceivedDate END IS NULL THEN NULL ELSE a.ReceivedDate END, '1990-01-01') AND (c.DCNoteDate IS NULL OR c.DCNoteDate > '2020-08-05 00:00:00' OR (b.HasUKDN = 0 AND NOT EXISTS(SELECT * FROM UKClaimBIADiscrepancy WHERE ClaimId = a.ClaimId AND IsLocked = 1 AND Status = 1)) OR EXISTS(SELECT * FROM UKClaimBIADiscrepancy ad INNER JOIN UKClaimBIAMapping am ON ad.ClaimId = am.ParentId WHERE am.ClaimId = a.ClaimId AND ad.IsLocked = 0 AND ad.Status = 1 AND am.Status = 1))) 
--AND ISNULL(c.IsInterfaced, 0) = 0 
--AND a.ClaimId NOT IN (SELECT ClaimId FROM vw_FullUKClaimRefund_OSRpt) 
--AND b.WorkflowStatusId NOT IN (9) 
AND a.ClaimId NOT IN (SELECT ClaimId FROM #WithoutUKDNCancelled) 
--AND b.ProductTeamId IN (SELECT OfficeStructureId FROM vw_HomeProductTeamId) 
AND (-1 IN (-1,2)) 
AND (q.WorkflowStatusId NOT IN (-100)) 
) AS x 

ORDER BY OfficeId, GUId, PnLAccountCode, UKDebitNoteDate, ClaimTypeId, UKDebitNoteNo, ClaimId DESC 

DROP TABLE #WithoutUKDNCancelled 
