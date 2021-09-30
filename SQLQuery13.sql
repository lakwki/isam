                 SELECT a.ClaimId, a.Amount INTO #mike
          FROM UKClaim a INNER JOIN UKClaimRefund b ON a.ClaimId = b.ClaimId And a.Status = 1 AND b.Status = 1 AND a.HasUKDN = 0 AND a.VendorId NOT IN (3933)
          INNER JOIN UKClaimDCNoteDetail c ON b.ClaimId = c.ClaimId AND b.ClaimRefundId = c.ClaimRefundId AND c.Status = 1  
          INNER JOIN UKClaimDCNote d ON c.DCNoteId = d.DCNoteId AND d.Status = 1 AND d.DCNoteNo IS NOT NULL AND d.DCNoteDate <= '2020-08-05'
          INNER JOIN UKClaimDCNoteDetail e ON a.ClaimId = e.ClaimId AND e.ClaimRefundId = 0 AND e.Status = 1  
          INNER JOIN UKClaimDCNote f ON e.DCNoteId = f.DCNoteId AND f.Status = 1 AND f.DCNoteNo IS NOT NULL AND f.DCNoteDate <= '2020-08-05'
          GROUP BY a.ClaimId, a.Amount
          HAVING a.Amount = SUM(b.Amount)  
	   
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
          WHERE ('2020-08-05' >= ISNULL(CASE WHEN a.ClaimTypeId = 9 THEN CONVERT(DATETIME, CONVERT(VARCHAR, a.CreatedOn, 112)) ELSE a.UKDebitNoteReceivedDate END, '1990-01-01') 
		  AND (c.DCNoteDate IS NULL OR c.DCNoteDate > '2020-08-05' OR (a.HasUKDN = 0 AND NOT EXISTS(SELECT * FROM UKClaimBIADiscrepancy WHERE ClaimId = a.ClaimId AND IsLocked = 1 AND Status = 1)) 
		  OR EXISTS(SELECT * FROM UKClaimBIADiscrepancy ad INNER JOIN UKClaimBIAMapping am ON ad.ClaimId = am.ParentId WHERE am.ClaimId = a.ClaimId AND ad.IsLocked = 0 AND ad.Status = 1 AND am.Status = 1)))
        
          AND (a.VendorId = 4972)

          --AND ISNULL(c.IsInterfaced, 0) = 0
          --AND a.ClaimId NOT IN (SELECT ClaimId FROM vw_FullUKClaimRefund_OSRpt)
          --AND a.WorkflowStatusId NOT IN (9)
          AND a.ClaimId NOT IN (SELECT ClaimId FROM #mike)
          AND a.Status = 1
          --AND a.ProductTeamId IN (SELECT OfficeStructureId FROM vw_HomeProductTeamId)
     

	 drop table #mike