using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

using com.next.infra.persistency.dataaccess;
using com.next.infra.persistency.transactions;
using com.next.infra.util;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.dataserver.worker;
using com.next.isam.dataserver.model.shipping;
using com.next.isam.dataserver.model.order;
using com.next.isam.domain.common;
using com.next.isam.domain.shipping;
using com.next.isam.domain.types;
using com.next.isam.reporter.lcreport;
using com.next.isam.reporter.dataserver;


namespace com.next.isam.appserver.shipping
{
    public class LCManager
    {
        private static LCManager _instance;
        private LCWorker lcWorker;
        private GeneralWorker generalWorker;

        public LCManager()
        {
            lcWorker = LCWorker.Instance;
            generalWorker = GeneralWorker.Instance;
        }

        public static LCManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LCManager();
                }
                return _instance;
            }
        }

        public ArrayList getLCShipmentDetailByShipmentId(int shipmentId, int splitShipmentId)
        {
            return lcWorker.getLCShipmentDetailByShipmentId(shipmentId, splitShipmentId);
        }

        public void createLCApplication(ArrayList lcApplicationDefs, int userId, ArrayList officeCodeList)
        {
            int i, SubtotalCount, FirstSubtotalRow;
            decimal UnitPrice, NextUnitPrice;
            int ShipmentId, NextShipmentId;
            int QtySubtotal;
            LCApplicationShipmentDetailDef ThisRow, NextRow;
            string tableHTML;
            string styleBackgroundColor;
            bool appExist = false;

            //ArrayList SummissionList = new ArrayList();

            ArrayList lcShipmentDetailList;
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                LCApplicationRef app = new LCApplicationRef();
                app.LCApplicationId = int.MinValue;
                app.CreateUser = generalWorker.getUserByKey(userId);
                app.CreateDate = DateTime.Now;
                app.Status = GeneralStatus.ACTIVE.Code;
                LCWorker.Instance.updateLCApplication(app, userId);

                // create the style of LC Application detail table for sending eMail
                ArrayList SubmitTable = new ArrayList();
                ArrayList detailStyle = new ArrayList();
                ArrayList headerContent = new ArrayList();
                ArrayList headerStyle = new ArrayList();

                // create Table Header in eMail
                headerStyle.Add("STYLE"); headerContent.Add("HEADER");
                headerStyle.Add("valign='center'"); headerContent.Add("Application No.");
                headerStyle.Add("valign='center'"); headerContent.Add("Contract No.");
                headerStyle.Add("valign='center'"); headerContent.Add("Dly");
                headerStyle.Add("valign='center'"); headerContent.Add("Customer");
                headerStyle.Add("valign='center'"); headerContent.Add("Item No.");
                headerStyle.Add("valign='center'"); headerContent.Add("At Warehouse Date");
                headerStyle.Add("valign='center'"); headerContent.Add("Country Of Origin");
                headerStyle.Add("valign='center'"); headerContent.Add("Purchase Term");
                headerStyle.Add("valign='center'"); headerContent.Add("PO Qty.");
                headerStyle.Add("valign='center' colspan=2 "); headerContent.Add("PO Amount");
                headerStyle.Add("valign='center'"); headerContent.Add("Unit Price");
                headerStyle.Add("valign='center'"); headerContent.Add("Qty.");
                headerStyle.Add("valign='center'"); headerContent.Add("Supplier Name");
                headerStyle.Add("valign='center'"); headerContent.Add("Advising Bank");

                //for (j = 0; j < officeCodeList.Count; j++)
                {
                    SubmitTable.Add(headerStyle);
                    SubmitTable.Add(headerContent);

                    LCApplicationDef appShipment;
                    foreach (LCApplicationDef submittedAppShipment in lcApplicationDefs)
                    {
                        //if (appShipment.Office.OfficeCode == officeCodeList[j].ToString())
                        {
                            LCApplicationDef existingAppShipment = LCWorker.Instance.getLCApplicationShipmentByShipmentId(submittedAppShipment.ShipmentId, submittedAppShipment.SplitShipmentId);
                            if (existingAppShipment == null)
                            {   // Add new LC Application Shipment record
                                appShipment = submittedAppShipment;

                                appShipment.LCApplicationId = app.LCApplicationId;
                                appShipment.LCApplicationNo = app.LCApplicationNo;
                                appShipment.Status = GeneralStatus.ACTIVE.Code;
                                appShipment.WorkflowStatus = LCWFS.NEW;
                                appShipment.LCBatchId = int.MinValue;
                                LCWorker.Instance.updateLCApplicationShipment(appShipment, userId);
                                lcShipmentDetailList = LCWorker.Instance.getLCShipmentDetailByShipmentId(appShipment.ShipmentId, appShipment.SplitShipmentId);
                                LCWorker.Instance.updateLCApplicationShipmentDetail(appShipment, lcShipmentDetailList);

                                // log the action into ActionHistory
                                ActionHistoryDef action = new ActionHistoryDef();
                                action.ShipmentId = appShipment.ShipmentId;
                                action.SplitShipmentId = appShipment.SplitShipmentId;
                                action.ActionDate = DateTime.Now;
                                action.ActionHistoryType = ActionHistoryType.LC_APPLICATION;
                                action.Remark = "Submit L/C Application : Application No=" + appShipment.LCApplicationNo.ToString() + ", Shipment=" + appShipment.ContractNo + "-" + appShipment.DeliveryNo.ToString(); // +", User ID=" + userId.ToString();
                                action.Status = 1;
                                action.User = generalWorker.getUserByKey(userId);
                                ShippingWorker.Instance.updateActionHistory(action);
                            }
                            else
                            {   // get the existing LC Application Shipment record
                                appExist = true;
                                appShipment = existingAppShipment;
                                appShipment.ContractNo = submittedAppShipment.ContractNo;
                                appShipment.DeliveryNo = submittedAppShipment.DeliveryNo;
                                lcShipmentDetailList = LCWorker.Instance.getLCShipmentDetailByShipmentId( existingAppShipment.ShipmentId, existingAppShipment.SplitShipmentId);
                            }

                            // Prepare LC Application detail table for sending eMail
                            styleBackgroundColor = (existingAppShipment != null ? "background-color:#FFFF99;" : "");
                            QtySubtotal = 0;
                            FirstSubtotalRow = 0;
                            SubtotalCount = 0;
                            UnitPrice = ((LCApplicationShipmentDetailDef)lcShipmentDetailList[0]).ReducedSupplierGmtPrice;
                            ShipmentId = ((LCApplicationShipmentDetailDef)lcShipmentDetailList[0]).ShipmentId;
                            for (i = 0; i < lcShipmentDetailList.Count; i++)
                            {
                                ThisRow = ((LCApplicationShipmentDetailDef)lcShipmentDetailList[i]);
                                if (i >= lcShipmentDetailList.Count - 1)
                                {
                                    NextShipmentId = -1;
                                    NextUnitPrice = -1;
                                }
                                else
                                {
                                    NextRow = ((LCApplicationShipmentDetailDef)lcShipmentDetailList[i + 1]);
                                    NextShipmentId = NextRow.ShipmentId;
                                    NextUnitPrice = NextRow.ReducedSupplierGmtPrice;
                                }

                                QtySubtotal += ThisRow.POQty;
                                if (ThisRow.ShipmentId != NextShipmentId || ThisRow.ReducedSupplierGmtPrice != NextUnitPrice)
                                {
                                    SubtotalCount++;
                                    ArrayList rowDetail = new ArrayList();
                                    rowDetail.Add("DETAIL");
                                    if (SubtotalCount == 1)
                                    {   // First row of break down for each shipment
                                        rowDetail.Add((existingAppShipment == null ? "&nbsp;&nbsp;&nbsp;" : "* ") + appShipment.LCApplicationNo.ToString().PadLeft(6, char.Parse("0")));
                                        rowDetail.Add(appShipment.ContractNo);
                                        rowDetail.Add(appShipment.DeliveryNo.ToString());
                                        rowDetail.Add(appShipment.Customer.CustomerDescription);
                                        rowDetail.Add(appShipment.Product.ItemNo);
                                        //rowDetail.Add(appShipment.CustomerAtWarehouseDate.ToString("dd/MM/yyyy"));
                                        rowDetail.Add(appShipment.SupplierAtWarehouseDate.ToString("dd/MM/yyyy"));
                                        rowDetail.Add(appShipment.CountryOfOrigin.Name);
                                        rowDetail.Add(appShipment.TermOfPurchase.TermOfPurchaseDescription);
                                        rowDetail.Add(appShipment.TotalPOQuantity.ToString("N0"));
                                        rowDetail.Add(appShipment.Currency.CurrencyCode);
                                        rowDetail.Add(appShipment.TotalPOAmount.ToString("N02"));
                                        rowDetail.Add(ThisRow.ReducedSupplierGmtPrice.ToString("N02"));
                                        rowDetail.Add(QtySubtotal.ToString("N0"));
                                        rowDetail.Add(appShipment.Vendor.Name);
                                        rowDetail.Add(appShipment.AdvisingBank.BankName);
                                        SubmitTable.Add(rowDetail);
                                        FirstSubtotalRow = SubmitTable.Count;
                                    }
                                    else
                                    {   // Other row of break down for each shipment

                                        // style for the break down info
                                        if (ThisRow.ShipmentId != NextShipmentId)
                                        {
                                            detailStyle = new ArrayList();
                                            detailStyle.Add("STYLE");
                                            detailStyle.Add("rowspan='1' align='right' valign='center' style='border-top-style:none;" + styleBackgroundColor + "'");
                                            detailStyle.Add("rowspan='1' align='right' valign='center' style='border-top-style:none;" + styleBackgroundColor + "'");
                                            SubmitTable.Add(detailStyle);
                                        }
                                        else
                                            if (SubtotalCount == 2)
                                        {
                                            detailStyle = new ArrayList();
                                            detailStyle.Add("STYLE");
                                            detailStyle.Add("rowspan='1' align='right' valign='center' style='border-top-style:none;border-bottom-style:none;" + styleBackgroundColor + "'");
                                            detailStyle.Add("rowspan='1' align='right' valign='center' style='border-top-style:none;border-bottom-style:none;" + styleBackgroundColor + "'");
                                            SubmitTable.Add(detailStyle);
                                        }
                                        // figure of break down
                                        rowDetail.Add(ThisRow.ReducedSupplierGmtPrice.ToString("N02"));
                                        rowDetail.Add(QtySubtotal.ToString("N0"));
                                        SubmitTable.Add(rowDetail);
                                    }
                                    QtySubtotal = 0;

                                    if (ThisRow.ShipmentId != NextShipmentId)
                                    {
                                        ArrayList rowStyle = new ArrayList();
                                        // Insert style for shipment level info (the first line of price break down)
                                        rowStyle.Add("STYLE");
                                        for (int r = 0; r < 8; r++)
                                            rowStyle.Add("rowspan='" + SubtotalCount.ToString() + "' align='center' valign='center' style='" + styleBackgroundColor + "'");
                                        rowStyle.Add("rowspan='" + SubtotalCount.ToString() + "' align='right'  valign='center' style='" + styleBackgroundColor + "'");
                                        rowStyle.Add("rowspan='" + SubtotalCount.ToString() + "' align='center' valign='center' style='border-right-style:none;" + styleBackgroundColor + "'");
                                        rowStyle.Add("rowspan='" + SubtotalCount.ToString() + "' align='right'  valign='center' style='border-left-style:none;" + styleBackgroundColor + "'");
                                        if (SubtotalCount == 1)
                                        {
                                            rowStyle.Add("rowspan='1' align='right' valign='center' style='" + styleBackgroundColor + "'");
                                            rowStyle.Add("rowspan='1' align='right' valign='center' style='" + styleBackgroundColor + "'");
                                        }
                                        else
                                        {
                                            rowStyle.Add("rowspan='1' align='right' valign='center' style='border-bottom-style:none;" + styleBackgroundColor + "'");
                                            rowStyle.Add("rowspan='1' align='right' valign='center' style='border-bottom-style:none;" + styleBackgroundColor + "'");
                                        }
                                        rowStyle.Add("rowspan='" + SubtotalCount.ToString() + "' align='left' valign='center' style='" + styleBackgroundColor + "'");
                                        rowStyle.Add("rowspan='" + SubtotalCount.ToString() + "' align='left' valign='center' style='" + styleBackgroundColor + "'");
                                        SubmitTable.Insert(FirstSubtotalRow - 1, rowStyle);
                                        SubtotalCount = 0;
                                        FirstSubtotalRow = 0;
                                    }
                                }
                                //Next LC Application Detail row
                            }
                            //Same office code
                        }
                        // Next Shipment row
                    }
                    //SubmitTable.Clear();
                    // Next Office
                }
                ctx.VoteCommit();

                if (appExist)
                {
                    detailStyle = new ArrayList();
                    detailStyle.Add("STYLE");
                    detailStyle.Add("colspan='15' align='left' valign='center' ");
                    SubmitTable.Add(detailStyle);

                    ArrayList rowDetail = new ArrayList();
                    rowDetail.Add("DETAIL");
                    rowDetail.Add("* - This shipment has been included into another L/C Application. Your application has been ignored.");
                    SubmitTable.Add(rowDetail);
                }

                // send notice to application user
                tableHTML = lcWorker.generateHtmlTable(SubmitTable);
                // send mail to LC application user (merchandiser)

                string eMailAddr;
                string ccMailAddress = "";
                SystemParameterRef sysPara;
                for (i = 0; i < officeCodeList.Count; i++)
                {
                    sysPara = CommonWorker.Instance.getSystemParameterByName(officeCodeList[i].ToString() + "_SHIPPING_EMAIL");
                    if (sysPara == null)
                        eMailAddr = "";
                    else
                        eMailAddr = sysPara.ParameterValue.TrimEnd();
                    //eMailAddr = CommonWorker.Instance.getSystemParameterByName(officeCodeList[i].ToString() + "_SHIPPING_EMAIL").ParameterValue.TrimEnd(); 
                    if (!(ccMailAddress.IndexOf(eMailAddr) >= 0))
                        ccMailAddress += eMailAddr + (eMailAddr == "" || eMailAddr.EndsWith(";") ? "" : ";");
                }

                NoticeHelper.sendLCSubmissionBatchMail(tableHTML, app.LCApplicationNo, userId, ccMailAddress);

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


        public string resendSubmitNoticeForLCApplication_TEST2(ArrayList lcApplicationDefs, ArrayList officeCodeList)
        {
            int i;
            LCApplicationShipmentDetailDef ThisRow;

            ArrayList lcShipmentDetailList;

            LCApplicationRef rf = lcWorker.getLCApplicationByKey(((LCApplicationDef)lcApplicationDefs[0]).LCApplicationId);

            // create the style of LC Application detail table for sending eMail
            ArrayList SubmitTable = new ArrayList();
            ArrayList RowContent = new ArrayList();

            // create Table Header
            RowContent.Add("DATA");
            RowContent.Add("Application No.");
            RowContent.Add("Contract No.");
            RowContent.Add("Dly");
            RowContent.Add("Customer");
            RowContent.Add("Item No.");
            RowContent.Add("At Warehouse Date");
            RowContent.Add("Country Of Origin");
            RowContent.Add("Purchase Term");
            RowContent.Add("PO Qty.");
            RowContent.Add("PO Amount");
            RowContent.Add("Size Option");
            RowContent.Add("Unit Price");
            RowContent.Add("Qty.");
            RowContent.Add("Supplier Name");
            RowContent.Add("Advising Bank");
            SubmitTable.Add(RowContent);

            foreach (LCApplicationDef def in lcApplicationDefs)
            {
                lcShipmentDetailList = LCWorker.Instance.getLCApplicationShipmentDetail(def);
                for (i = 0; i < lcShipmentDetailList.Count; i++)
                {
                    ThisRow = ((LCApplicationShipmentDetailDef)lcShipmentDetailList[i]);
                    RowContent = new ArrayList();
                    RowContent.Add("DATA");

                    RowContent.Add(def.LCApplicationNo.ToString().PadLeft(6, char.Parse("0")));
                    RowContent.Add(def.ContractNo);
                    RowContent.Add(def.DeliveryNo.ToString());
                    RowContent.Add(def.Customer.CustomerDescription);
                    RowContent.Add(def.Product.ItemNo);
                    RowContent.Add(def.SupplierAtWarehouseDate.ToString("dd/MM/yyyy"));
                    RowContent.Add(def.CountryOfOrigin.Name);
                    RowContent.Add(def.TermOfPurchase.TermOfPurchaseDescription);
                    RowContent.Add(def.TotalPOQuantity.ToString("N0"));
                    RowContent.Add(def.Currency.CurrencyCode);
                    RowContent.Add(def.TotalPOAmount.ToString("N02"));
                    RowContent.Add(ThisRow.SizeOptionId.ToString());
                    RowContent.Add(ThisRow.ReducedSupplierGmtPrice.ToString("N02"));
                    RowContent.Add(ThisRow.POQty.ToString("N0"));
                    RowContent.Add(def.Vendor.Name);
                    RowContent.Add(def.AdvisingBank.BankName);
                    SubmitTable.Add(RowContent);
                }

            }

            return com.next.isam.appserver.helper.TableHelper.Instance.generateHtmlTable(SubmitTable);
        }


        public void createLCBatch(LCBatchRef lcBatch, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                if (lcBatch.Status == GeneralStatus.ACTIVE.Code)
                {
                    LCBatchRef rf = new LCBatchRef();
                    lcBatch.LCBatchId = -1;
                    LCWorker.Instance.updateLCBatch(lcBatch, userId);
                }
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


        public void createLCBatchList(ArrayList lcBatchRefs, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                LCBatchRef rf = new LCBatchRef();

                foreach (LCBatchAssignRef rfBatchKey in lcBatchRefs)
                {
                    if (rfBatchKey.LCBatch.Status == GeneralStatus.ACTIVE.Code)
                    {
                        rfBatchKey.LCBatch.LCBatchId = -1;
                        LCWorker.Instance.updateLCBatch(rfBatchKey.LCBatch, userId);
                    }
                }
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


        public void approveLCApplication(ArrayList lcShipmentRefs, ArrayList lcBatchRefs, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                string tableHTML;
                int i,j;
                int diff, detailStart;
                int userSeq;
                bool inserted;

                ArrayList approvalList = new ArrayList();

                LCApplicationDef lcApplication = new LCApplicationDef();

                foreach (LCShipmentRef lcShipment in lcShipmentRefs)
                    if ((lcShipment.WorkflowStatus.Id == LCWFS.NEW.Id) || (lcShipment.WorkflowStatus.Id == LCWFS.REJECTED.Id))
                        if (lcShipment.LCBatch.GroupId >= 0 && lcShipment.LCBatch.LCBatchNo <= 0)
                            lcShipment.LCBatch.Status = GeneralStatus.ACTIVE.Code;  // mark the flag in the array to create new LC batch for the shipment

                createLCBatchList(lcBatchRefs, userId);

                foreach (LCShipmentRef lcShipment in lcShipmentRefs)
                {
                    // update the LCBatchId and status of the LC ApplicationShipment
                    // LCShipment.LCBatch = (LCBatchRef)lcBatchRefs[LCShipment.LCBatch.LCBatchId];

                    if (lcShipment.LCBatch.Status == GeneralStatus.ACTIVE.Code)
                    {
                        lcWorker.LCShipmentCopyToLCApplication(lcShipment, lcApplication);
                        lcApplication.LCApprover = generalWorker.getUserByKey(userId);
                        lcApplication.LCApprovalDate = DateTime.Now;
                        lcApplication.WorkflowStatus = LCWFS.APPROVED;
                        lcApplication.Status = GeneralStatus.ACTIVE.Code;
                        lcWorker.updateLCApplicationShipment(lcApplication, userId);

                        // add the shipment detail into approval array grouping by the LC application user (merchandiser)
                        for (i = 0; i < approvalList.Count; i++)
                            if (((LCShipmentRef)((ArrayList)approvalList[i])[0]).LCApplication.CreateUser.UserId == lcShipment.LCApplication.CreateUser.UserId)
                                break ;

                        if (i < approvalList.Count)
                            userSeq = i;
                        else // user does not exist in the approval array
                            userSeq = approvalList.Add(new ArrayList());

                        ((ArrayList)approvalList[userSeq]).Add(lcShipment);

                        // log the action into ActionHistory
                        ActionHistoryDef action = new ActionHistoryDef();
                        action.ShipmentId = lcApplication.ShipmentId;
                        action.SplitShipmentId = lcApplication.SplitShipmentId;
                        action.ActionDate = DateTime.Now;
                        action.ActionHistoryType = ActionHistoryType.LC_APPLICATION;
                        action.Remark = "Approve L/C Application : LC Batch No=" + lcShipment.LCBatch.LCBatchNo.ToString() + ", Application No=" + lcApplication.LCApplicationNo.ToString() + ", Shipment=" + lcApplication.ContractNo + "-" + lcApplication.DeliveryNo.ToString();     // +", User ID=" + userId.ToString();
                        action.Status = 1;
                        action.User = generalWorker.getUserByKey(userId);
                        ShippingWorker.Instance.updateActionHistory(action);
                    }
                }
                ctx.VoteCommit();

                // send notice to application user
                // create Table Header

                ArrayList headerContent = new ArrayList();
                ArrayList headerStyle = new ArrayList();
                headerStyle.Add("STYLE"); headerContent.Add("HEADER");
                headerStyle.Add(""); headerContent.Add("Application No.");
                headerStyle.Add(""); headerContent.Add("Contract No.");
                headerStyle.Add(""); headerContent.Add("Dly");
                headerStyle.Add(""); headerContent.Add("Item No.");
                headerStyle.Add(""); headerContent.Add("At Warehouse Date");
                headerStyle.Add(""); headerContent.Add("PO Qty.");
                headerStyle.Add("colspan=2"); headerContent.Add("PO Amount");
                headerStyle.Add(""); headerContent.Add("Supplier Name");

                ArrayList detailStyle = new ArrayList();
                detailStyle.Add("STYLE");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='right'");
                detailStyle.Add("align='center' style='border-right-style:none;'");
                detailStyle.Add("align='right' style='border-left-style:none;'");
                detailStyle.Add("");

                ArrayList approveTable = new ArrayList();
                for (i = 0; i < approvalList.Count; i++)
                {
                    approveTable.Clear();
                    approveTable.Add(headerStyle);
                    approveTable.Add(headerContent);
                    approveTable.Add(detailStyle);
                    detailStart = approveTable.Count;// -1;

                    foreach (LCShipmentRef app in (ArrayList) approvalList[i])
                    {
                        // Create row for the approved LC application 
                        ArrayList rowDetail = new ArrayList();
                        rowDetail.Add("DETAIL");
                        rowDetail.Add(app.LCApplication.LCApplicationNo.ToString().PadLeft(6, char.Parse("0")));
                        rowDetail.Add(app.ContractNo);
                        rowDetail.Add(app.DeliveryNo.ToString());
                        rowDetail.Add(app.Product.ItemNo);
                        rowDetail.Add(app.CustomerAtWarehouseDate.ToString("dd/MM/yyyy"));
                        rowDetail.Add(app.TotalPOQuantity.ToString("N0"));
                        rowDetail.Add(app.Currency.CurrencyCode);
                        rowDetail.Add(app.TotalPOAmt.ToString("N02"));
                        rowDetail.Add(app.Vendor.Name);

                        // Insert the row of detail into approval list in order
                        inserted = false;
                        for (j = detailStart; j < approveTable.Count; j++)
                        {
                            inserted = false;
                            diff = 0;
                            if (diff == 0 && (diff = string.Compare(rowDetail[9].ToString(), ((ArrayList)approveTable[j])[9].ToString())) > 0) continue;  // vendor Name
                            if (diff == 0 && (diff = DateTime.Compare( DateTime.Parse(rowDetail[5].ToString()), DateTime.Parse(((ArrayList)approveTable[j])[5].ToString()))) > 0) continue;    // At Warehouse Date
                            if (diff == 0 && (diff = string.Compare(rowDetail[2].ToString(), ((ArrayList)approveTable[j])[2].ToString())) > 0) continue;   // Contract No
                            if (diff == 0 && (diff = string.Compare(rowDetail[3].ToString(), ((ArrayList)approveTable[j])[3].ToString())) > 0) continue;   // Delivery No
                            approveTable.Insert(j, rowDetail);
                            inserted = true;
                            break;
                        }
                        if (!inserted) approveTable.Add(rowDetail);
                    }
                    tableHTML = lcWorker.generateHtmlTable(approveTable);
                    // send mail to LC application user (merchandiser)
                    NoticeHelper.sendLCApprovalBatchMail(tableHTML, ((LCShipmentRef)((ArrayList)approvalList[i])[0]).LCApplication.CreateUser.UserId, userId);
                }
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


        public void rejectLCApplication(ArrayList lcShipmentRefs, int userId)//ArrayList lcBatchRefs, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                string tableHTML;
                int i, userSeq;

                ArrayList rejectedList = new ArrayList();

                LCApplicationDef lcApplication = new LCApplicationDef();

                //Remark as we do not assign an LC Batch to the rejected application
                //foreach (LCShipmentRef lcShipment in lcShipmentRefs)
                //    if ((lcShipment.WorkflowStatus.Id == LCWFS.APPROVED.Id) || (lcShipment.WorkflowStatus.Id == LCWFS.NEW.Id))
                //        lcShipment.LCBatch.Status = 1;  // mark the flag in the array to create new LC batch for the rejected application 
                //
                //createLCBatchList(lcBatchRefs, userId);

                // group the application by application user
                foreach (LCShipmentRef lcShipment in lcShipmentRefs)
                {
                    //if (lcShipment.LCBatch.Status == 1)
                    if ((lcShipment.WorkflowStatus.Id == LCWFS.APPROVED.Id) || (lcShipment.WorkflowStatus.Id == LCWFS.NEW.Id))
                    {
                        lcWorker.LCShipmentCopyToLCApplication(lcShipment, lcApplication);
                        lcApplication.LCApprover = generalWorker.getUserByKey(userId);
                        lcApplication.LCApprovalDate = DateTime.Now;
                        lcApplication.WorkflowStatus = LCWFS.REJECTED;
                        lcApplication.Status = GeneralStatus.INACTIVE.Code;
                        lcWorker.updateLCApplicationShipment(lcApplication, userId);
                        lcWorker.MarkDeleteLCApplicationShipmentDetail(lcApplication);


                        // add the shipment detail into reject array grouping by the LC application user (merchandiser)
                        for (i = 0; i < rejectedList.Count; i++)
                            if (((LCShipmentRef)((ArrayList)rejectedList[i])[0]).LCApplication.CreateUser.UserId == lcShipment.LCApplication.CreateUser.UserId)
                                break;

                        if (i < rejectedList.Count)
                            userSeq = i;
                        else // user does not exist in the rejected array
                            userSeq = rejectedList.Add(new ArrayList());

                        ((ArrayList)rejectedList[userSeq]).Add(lcShipment);

                        // log the action into ActionHistory
                        ActionHistoryDef action = new ActionHistoryDef();
                        action.ShipmentId = lcApplication.ShipmentId;
                        action.SplitShipmentId = lcApplication.SplitShipmentId;
                        action.ActionDate = DateTime.Now;
                        action.ActionHistoryType = ActionHistoryType.LC_APPLICATION;
                        action.Remark = "Reject L/C Application : LC Batch No=" + lcShipment.LCBatch.LCBatchNo.ToString() + ", Application No=" + lcApplication.LCApplicationNo.ToString() + ", Shipment=" + lcApplication.ContractNo + "-" + lcApplication.DeliveryNo.ToString();      // +", User ID=" + userId.ToString();
                        action.Status = 1;
                        action.User = generalWorker.getUserByKey(userId);
                        ShippingWorker.Instance.updateActionHistory(action);
                    }
                }
                ctx.VoteCommit();

                // send notice to application user
                // create Table Header

                ArrayList headerContent = new ArrayList();
                ArrayList headerStyle = new ArrayList();
                headerStyle.Add("STYLE"); headerContent.Add("HEADER");
                headerStyle.Add(""); headerContent.Add("Application No.");
                headerStyle.Add(""); headerContent.Add("Contract No.");
                headerStyle.Add(""); headerContent.Add("Dly");
                headerStyle.Add(""); headerContent.Add("Item No.");
                headerStyle.Add(""); headerContent.Add("At Warehouse Date");
                headerStyle.Add(""); headerContent.Add("PO Qty.");
                headerStyle.Add("colspan=2"); headerContent.Add("PO Amount");
                headerStyle.Add(""); headerContent.Add("Supplier Name");

                ArrayList detailStyle = new ArrayList();
                detailStyle.Add("STYLE");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='center'");
                detailStyle.Add("align='right'");
                detailStyle.Add("align='center' style='border-right-style:none;'");
                detailStyle.Add("align='right' style='border-left-style:none;'");
                detailStyle.Add("");

                ArrayList rejectTable = new ArrayList();
                for (i=0; i < rejectedList.Count; i++)
                {
                    rejectTable.Clear();
                    rejectTable.Add(headerStyle);
                    rejectTable.Add(headerContent);
                    rejectTable.Add(detailStyle);
                    foreach(LCShipmentRef app in (ArrayList)rejectedList[i])
                    {
                        // Create reject list detail
                        ArrayList rowDetail = new ArrayList();
                        rowDetail.Add("DETAIL");
                        rowDetail.Add(app.LCApplication.LCApplicationNo.ToString().PadLeft(6, char.Parse("0")));
                        rowDetail.Add(app.ContractNo);
                        rowDetail.Add(app.DeliveryNo.ToString());
                        rowDetail.Add(app.Product.ItemNo);
                        rowDetail.Add(app.CustomerAtWarehouseDate.ToString("dd/MM/yyyy"));
                        rowDetail.Add(app.TotalPOQuantity.ToString("N0"));
                        rowDetail.Add(app.Currency.CurrencyCode);
                        rowDetail.Add(app.TotalPOAmt.ToString("N02"));
                        rowDetail.Add(app.Vendor.Name);
                        rejectTable.Add(rowDetail);
                    }
                    tableHTML = lcWorker.generateHtmlTable(rejectTable);
                    NoticeHelper.sendLCRejectBatchMail(tableHTML, ((LCShipmentRef)((ArrayList)rejectedList[i])[0]).LCApplication.CreateUser.UserId, userId);
                }
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


        public void applyLCApplication(ArrayList lcShipmentRefs, int userId)
        {
            string mailBody;
            int currentLCBatchNo;
            ArrayList currentLCBatch;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                //string filePath;
                LCApplicationDef lcApplication = new LCApplicationDef();

                TypeCollector lcApplicationIdList = TypeCollector.Inclusive;
                TypeCollector workflowStatusIdList = TypeCollector.Inclusive;
                TypeCollector shipmentIdList = TypeCollector.Inclusive;
                TypeCollector splitShipmentIdList = TypeCollector.Inclusive;

                // extract LC detail report data
                foreach (LCShipmentRef LCShipment in lcShipmentRefs)
                {
                    if (LCShipment.WorkflowStatus.Id == LCWFS.APPROVED.Id)
                    {
                        if (LCShipment.SplitShipmentId > 0)
                            splitShipmentIdList.append(LCShipment.SplitShipmentId);
                        else
                            shipmentIdList.append(LCShipment.ShipmentId);
                    }
                }
                workflowStatusIdList.append(LCWFS.APPROVED.Id);
                //ArrayList lcReportArray = convertLCDetailToReportArray(lcWorker.getLCBatchDetailByShipmentIdList(shipmentIdList, splitShipmentIdList, workflowStatusIdList), false, false);
                ArrayList LCBatchDetail = lcWorker.getLCBatchDetailByShipmentIdList(shipmentIdList, splitShipmentIdList, workflowStatusIdList);

                currentLCBatch = new ArrayList();
                currentLCBatchNo = ((LCBatchDetailRef)LCBatchDetail[0]).LCBatch.LCBatchNo;
                for (int i = 0; i < LCBatchDetail.Count; i++)
                {
                    if ((currentLCBatchNo != ((LCBatchDetailRef)LCBatchDetail[i]).LCBatch.LCBatchNo))
                    {
                        mailBody = lcWorker.convertReportArrayToHtml(convertLCDetailToReportArray(currentLCBatch, false, false));
                        NoticeHelper.sendLCApplicationAppliedMail(mailBody, userId, "LCB" + currentLCBatchNo.ToString().PadLeft(6, char.Parse("0")));
                        currentLCBatchNo = ((LCBatchDetailRef)LCBatchDetail[i]).LCBatch.LCBatchNo;
                        currentLCBatch.Clear();
                    }

                    if ((currentLCBatchNo == ((LCBatchDetailRef)LCBatchDetail[i]).LCBatch.LCBatchNo))
                    {
                        currentLCBatch.Add(LCBatchDetail[i]);
                    }
                }
                if (currentLCBatch.Count>0)
                {
                    mailBody = lcWorker.convertReportArrayToHtml(convertLCDetailToReportArray(currentLCBatch, false, false));
                    NoticeHelper.sendLCApplicationAppliedMail(mailBody, userId, "LCB" + currentLCBatchNo.ToString().PadLeft(6, char.Parse("0")));
                    currentLCBatch.Clear();
                }

                // update workflow status for the applied application
                foreach (LCShipmentRef LCShipment in lcShipmentRefs)
                {
                    if (LCShipment.WorkflowStatus.Id == LCWFS.APPROVED.Id)
                    {
                        lcWorker.LCShipmentCopyToLCApplication(LCShipment, lcApplication);
                        lcApplication.WorkflowStatus = LCWFS.APPLIED;
                        lcApplication.Status = GeneralStatus.ACTIVE.Code;
                        lcWorker.updateLCApplicationShipment(lcApplication, userId);

                        // log the action into ActionHistory
                        ActionHistoryDef action = new ActionHistoryDef();
                        action.ShipmentId = lcApplication.ShipmentId;
                        action.SplitShipmentId = lcApplication.SplitShipmentId;
                        action.ActionDate = DateTime.Now;
                        action.ActionHistoryType = ActionHistoryType.LC_APPLICATION;
                        action.Remark = "Apply L/C : LC Batch No=" + LCShipment.LCBatch.LCBatchNo.ToString() + ", Application No=" + lcApplication.LCApplicationNo.ToString() + ", Shipment=" + lcApplication.ContractNo + "-" + lcApplication.DeliveryNo.ToString();       // + ", User ID=" + userId.ToString();
                        action.Status = 1;
                        action.User = generalWorker.getUserByKey(userId);
                        ShippingWorker.Instance.updateActionHistory(action);
                    }
                }

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


        public void cancelLCApplicationInLCBatch(ArrayList lcShipmentRefs, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                if (lcShipmentRefs.Count > 0)
                {
                    LCBatchRef lcBatch = null;
                    LCApplicationDef lcApp = new LCApplicationDef();
                    ArrayList cancelledLCShipment = new ArrayList();

                    foreach (LCShipmentRef lcShipment in lcShipmentRefs)
                    {
                        lcWorker.LCShipmentCopyToLCApplication(lcShipment, lcApp);
                        lcApp.Status = GeneralStatus.ACTIVE.Code;
                        lcApp.WorkflowStatus = LCWFS.LC_CANCELLED;
                        lcWorker.updateLCApplicationShipment(lcApp, userId);

                        // log the action into ActionHistory
                        ActionHistoryDef action = new ActionHistoryDef();
                        action.ShipmentId = lcApp.ShipmentId;
                        action.SplitShipmentId = 0;
                        action.ActionDate = DateTime.Now;
                        action.ActionHistoryType = ActionHistoryType.LC_APPLICATION;
                        action.Remark = "Cancel L/C Application : LC Batch No = " + lcShipment.LCBatch.LCBatchNo.ToString() + "; LC Application No = " + lcApp.LCApplicationNo.ToString();
                        action.Status = GeneralStatus.ACTIVE.Code; ;
                        action.User = generalWorker.getUserByKey(userId);
                        ShippingWorker.Instance.updateActionHistory(action);
                        cancelledLCShipment.Add(lcShipment);

                        if (lcBatch == null)
                            lcBatch = lcShipment.LCBatch;
                    }

                    if (cancelledLCShipment.Count > 0)
                        LCManager.Instance.extractLCBatchDetail(lcBatch.LCBatchId, userId, cancelledLCShipment, "Cancelled");
                }
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

        public void extractLCBatchDetail(ArrayList lcBatchIdList, int userId)
        {
            foreach (int lcBatchId in lcBatchIdList)
                extractLCBatchDetail(lcBatchId, userId, null, null);
        }

        public void extractLCBatchDetail(int lcBatchId, int userId, ArrayList lcShipmentList, string remark)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                // generate LC batch detail report for the list of shipments
                ArrayList shipmentIds = new ArrayList();
                string shipments = string.Empty;
                if (lcShipmentList != null && lcShipmentList.Count > 0)
                {
                    shipments = "It includes the following shipment :";
                    foreach (LCShipmentRef rf in lcShipmentList)
                    {
                        shipmentIds.Add(rf.ShipmentId);
                        shipments += string.Concat("<br> - ", rf.ContractNo, "-", rf.DeliveryNo);
                    }
                }
                ArrayList lcBatchDetail = lcWorker.getLCBatchDetailByLCBatchId(lcBatchId, TypeCollector.Inclusive.append(shipmentIds));
                updateLcApplicationFabricCost(lcBatchDetail, userId);
                ArrayList batchDetail = groupLCBatchDetailForExtraction(lcBatchDetail);
                if (batchDetail.Count > 0)
                {
                    LCBatchDetailRef firstDetail = ((LCBatchDetailRef)batchDetail[0]);
                    string supplierName = firstDetail.Vendor.Name;
                    string lcBatchNo = "LCB" + firstDetail.LCBatch.LCBatchNo.ToString().PadLeft(6, char.Parse("0"));
                    ArrayList lcReportArray = convertLCDetailToReportArray(batchDetail, true, false);
                    string filePath = lcWorker.exportReportArrayToText(lcReportArray, lcBatchNo + (shipmentIds.Count > 0 ? "_" + remark : ""), userId);
                    ArrayList attachments = new ArrayList();
                    attachments.Add(filePath);
                    NoticeHelper.sendLCExtractBatchMail(attachments, lcBatchNo, supplierName, (string.IsNullOrEmpty(remark) ? "" : " (" + remark + ")"), shipments, userId);
                }
                ctx.VoteCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR " + e.Message);
                NoticeHelper.sendErrorMessage(e, "Error in LC Batch Id " + lcBatchId.ToString() + (!string.IsNullOrEmpty(remark) ? " (" + remark + ")" : ""));
                ctx.VoteRollback();
                throw e;
            }
            finally
            {
                ctx.Exit();
            }
        }

        private string lcBatchDetailGroupKey(LCBatchDetailRef dtl)
        {
            string key = string.Empty;
            key += "|" + dtl.PurchaseLocation.PurchaseLocationId.ToString("0000");
            key += "|" + dtl.QACommissionPercent.ToString("00.00");
            key += "|" + dtl.VendorPaymentDiscountPercent.ToString("00.00");
            key += "|" + dtl.LabTestIncome.ToString("0000000.00");
            key += "|" + dtl.CustomerDestination.DestinationCode;
            key += "|" + dtl.Customer.CustomerId.ToString("0000");
            key += "|" + dtl.SupplierAtWarehouseDate.ToString("yyyyMMdd");
            key += "|" + dtl.Product.ItemNo;
            key += "|" + dtl.ReducedSupplierGmtPrice.ToString("0000000.00");

            key += "|" + dtl.LCApplicationId.ToString();
            key += "|" + dtl.LCBatch.LCBatchId.ToString();
            key += "|" + dtl.ShipmentMethod.ShipmentMethodId.ToString();
            key += "|" + dtl.Product.ProductId.ToString();
            key += "|" + dtl.PackingUnit.PackingUnitId.ToString();
            key += "|" + dtl.Vendor.VendorId.ToString();
            key += "|" + dtl.Currency.CurrencyId.ToString();
            key += "|" + dtl.TermOfPurchase.TermOfPurchaseId.ToString();
            key += "|" + dtl.PaymentTerm.PaymentTermId.ToString();
            key += "|" + dtl.ProductTeam.ProductCodeId.ToString();
            key += "|";
            return key;
        }

        private ArrayList groupLCBatchDetailForExtraction(ArrayList lcBatchDetail)
        {
            string key;
            SortedList<string, LCBatchDetailRef> list = new SortedList<string, LCBatchDetailRef>();
            foreach (LCBatchDetailRef dtl in lcBatchDetail)
                if (list.ContainsKey(key = lcBatchDetailGroupKey(dtl)))
                {
                    list[key].POQty += dtl.POQty;
                    list[key].ExpectedDeductAmt += dtl.ExpectedDeductAmt;
                    list[key].ActualDeductAmt += dtl.ActualDeductAmt;
                }
                else
                    list.Add(key, (LCBatchDetailRef)dtl.Clone());
            return new ArrayList((ICollection)list.Values);
        }

        /*
        private void updateLcApplicationFabricCost(ArrayList lcBatchDetail, int userId)
        {
            foreach (LCBatchDetailRef dtl in lcBatchDetail)
                if (dtl.ActualDeductAmt > 0)
                {
                    LCApplicationDef lcShipment = LCWorker.Instance.getLCApplicationShipmentByKey(dtl.LCApplicationId, dtl.ShipmentId, 0);
                    lcShipment.DeducedFabricCost = dtl.ActualDeductAmt;
                    lcWorker.updateLCApplicationShipment(lcShipment, userId);
                }
        }
        */
        private void updateLcApplicationFabricCost(ArrayList lcBatchDetail, int userId)
        {
            string key = string.Empty;
            SortedList<string, decimal> deduction = new SortedList<string, decimal>();
            foreach (LCBatchDetailRef dtl in lcBatchDetail)
            {
                key = lcBatchDetailGroupKey(dtl);
                if (!deduction.ContainsKey(key))
                    deduction.Add(key, dtl.ActualDeductAmt);
                else
                    deduction[key] += dtl.ActualDeductAmt;
            }

            foreach (LCBatchDetailRef dtl in lcBatchDetail)
                if (dtl.ActualDeductAmt > 0)
                {
                    LCApplicationDef lcShipment = LCWorker.Instance.getLCApplicationShipmentByKey(dtl.LCApplicationId, dtl.ShipmentId, 0);
                    key = lcBatchDetailGroupKey(dtl);
                    lcShipment.DeducedFabricCost = (lcShipment.TotalPOAmount < deduction[key] ? lcShipment.TotalPOAmount : deduction[key]);
                    deduction[key] -= lcShipment.DeducedFabricCost;
                    lcWorker.updateLCApplicationShipment(lcShipment, userId);
                }
        }


        public ArrayList convertLCDetailToReportArray(ArrayList lcBatchDetailRefs, bool textOnly, bool isExtractionFormat)
        {
            string groupKey, group1Key, group2Key, groupDesc;
            int colWidth1, colWidth2, colWidth3, colWidth4, colWidth5, colWidth6;
            int Header1Width1, Header2Width1;
            //int LineNoOfTotalAmount;
            ArrayList TotalAmountRow, ProductTeamRow;
            ArrayList ProductTeamList = new ArrayList();
            string ProductTeamDesc = string.Empty;
            decimal TotalAmount;
            string QAPercent, VPPercent, LabTestIncome;
            string BatchCurrency;
            bool group1Changed; //, group2Changed;
            ArrayList reportColumn;

            ArrayList reportContent = new ArrayList();

            // build the array of LC batch detail for exporting to file
            Header1Width1 = 18;
            //Header1Width2 = 80;
            //Header2Width1 = 30;
            Header2Width1 = 22;

            // detail column heading
            //colWidth1 = 10; colWidth2 = 6; colWidth3 = 7; colWidth4 = 8; colWidth5 = 11;
            //colWidth1 = 15; colWidth2 = 8; colWidth3 = 10; colWidth4 = 8; colWidth5 = 12;
            //colWidth1 = 15; colWidth2 = 6; colWidth3 = 8; colWidth4 = 8; colWidth5 = 9; colWidth6 = 28;
            colWidth1 = 12; colWidth2 = 5; colWidth3 = 7; colWidth4 = 7; colWidth5 = 9; colWidth6 = 11;

            reportColumn = new ArrayList();
            BatchCurrency = "";
            //LineNoOfTotalAmount = -1;
            TotalAmountRow = null;
            ProductTeamRow = null;

            ArrayList lcBatchProductTeam = lcWorker.getLCBatchProductTeamDetailByLCBatchId(((LCBatchDetailRef)lcBatchDetailRefs[0]).LCBatch.LCBatchId);
            foreach (LCBatchDetailRef dtl in lcBatchProductTeam)
                if (dtl.ProductTeam.ProductCodeId > 0)
                    if (!ProductTeamList.Contains(dtl.ProductTeam.Code))
                    {
                        ProductTeamList.Add(dtl.ProductTeam.Code);
                        ProductTeamDesc += (ProductTeamDesc == string.Empty ? "" : ", ") + dtl.ProductTeam.CodeDescription;
                    }

            TotalAmount = 0;
            group1Key = String.Empty;
            group2Key = String.Empty;
            decimal amt = 0;
            decimal deduction = 0;
            string  lastItemNo = string.Empty;
            DateTime lastDlyDate = DateTime.MinValue;
            foreach (LCBatchDetailRef lc in lcBatchDetailRefs)
            {
                if (isExtractionFormat)
                {
                    groupKey = lc.ShipmentMethod.ShipmentMethodDescription;
                }
                else
                {   // Create LC Application Table for current LC Batch Id
                    // Group Header 1 - Group by LC Batch, Payment Term, Term of Shipment
                    groupKey = lc.LCBatch.LCBatchNo
                            + '|' + lc.PaymentTerm.PaymentTermId.ToString()
                            + '|' + lc.TermOfPurchase.TermOfPurchaseId.ToString()
                            + '|' + lc.PurchaseLocation.PurchaseLocationId.ToString()
                            + '|' + lc.Currency.CurrencyCode;
                }
                if (groupKey != group1Key)
                {   // New group of LC Batch, Payment Term, Purchase Term and Purchase Location
                    BatchCurrency = lc.Currency.CurrencyCode;

                    //if (LineNoOfTotalAmount > 0)
                    //{
                    //    ((ArrayList)reportContent[LineNoOfTotalAmount]).Add(BatchCurrency + " " + TotalAmount.ToString("N02"));
                    //    TotalAmount = 0;
                    //    LineNoOfTotalAmount = -1;
                    //}
                    if (TotalAmountRow != null)
                    {
                        TotalAmountRow.Add(BatchCurrency + " " + TotalAmount.ToString("N02"));
                        TotalAmount = 0;
                        TotalAmountRow = null;
                    }

                    if (reportContent.Count > 0)
                    {   // Blank Line
                        reportContent.Add(new ArrayList());
                        reportContent.Add(new ArrayList());
                    }

                    if (isExtractionFormat)
                    {
                        reportColumn = new ArrayList();
                        reportColumn.Add("THE FOLLOWING SHIPMENTS SHOULD BE SHIPPED BY " + groupKey);
                        reportContent.Add(reportColumn);
                    }
                    else
                    {
                        reportColumn = new ArrayList();
                        reportColumn.Add(("L/C Batch No.").PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        reportColumn.Add("LCB" + lc.LCBatch.LCBatchNo.ToString().PadLeft(6, char.Parse("0")));
                        reportContent.Add(reportColumn);

                        reportColumn = new ArrayList();
                        reportColumn.Add(("Supplier Name").PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        reportColumn.Add(lc.Vendor.Name);
                        reportContent.Add(reportColumn);

                        reportColumn = new ArrayList();
                        reportColumn.Add("Supplier Address".PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        groupDesc = lc.Vendor.Address0.Trim();
                        groupDesc += (groupDesc == "" || lc.Vendor.Address1.Trim() == "" ? "" : ", " + lc.Vendor.Address1.Trim());
                        groupDesc += (groupDesc == "" || lc.Vendor.Address2.Trim() == "" ? "" : ", " + lc.Vendor.Address2.Trim());
                        groupDesc += (groupDesc == "" || lc.Vendor.Address3.Trim() == "" ? "" : ", " + lc.Vendor.Address3.Trim());
                        //if (groupDesc.Length < 80) groupDesc.Remove(groupDesc.IndexOf("\n\r"), 2);
                        groupDesc = groupDesc.Replace(",,", ",");
                        reportColumn.Add(groupDesc);
                        reportContent.Add(reportColumn);

                        reportColumn = new ArrayList();
                        reportColumn.Add(("Advising Bank").PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        reportColumn.Add(lc.AdvisingBank.BankName);
                        reportContent.Add(reportColumn);

                        reportColumn = new ArrayList();
                        reportColumn.Add("Bank Address".PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        groupDesc = lc.BankBranch.Address1.Trim();
                        groupDesc += (groupDesc == "" || lc.BankBranch.Address2.Trim() == "" ? "" : ", " + lc.BankBranch.Address2.Trim());
                        groupDesc += (groupDesc == "" || lc.BankBranch.Address3.Trim() == "" ? "" : ", " + lc.BankBranch.Address3.Trim());
                        groupDesc += (groupDesc == "" || lc.BankBranch.Address4.Trim() == "" ? "" : ", " + lc.BankBranch.Address4.Trim());
                        groupDesc = groupDesc.Replace(",,", ",");
                        reportColumn.Add(groupDesc);
                        reportContent.Add(reportColumn);

                        reportColumn = new ArrayList();
                        reportColumn.Add(("Payment Term").PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        reportColumn.Add(lc.PaymentTerm.PaymentTermDescription);
                        reportContent.Add(reportColumn);

                        reportColumn = new ArrayList();
                        reportColumn.Add(("Issuing Bank").PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        reportColumn.Add((IssuingBank.getType(lc.LCBatch.IssuingBankId).FullName));
                        reportContent.Add(reportColumn);

                        reportColumn = new ArrayList();
                        reportColumn.Add(("Product Team").PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        reportColumn.Add(ProductTeamDesc);
                        reportContent.Add(reportColumn);
                        ProductTeamRow = (ArrayList)reportContent[reportContent.Count - 1];

                        reportColumn = new ArrayList();
                        reportColumn.Add(("Term of Shipment").PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        reportColumn.Add(lc.TermOfPurchase.TermOfPurchaseDescription + " " + lc.PurchaseLocation.PurchaseLocationDescription);
                        reportContent.Add(reportColumn);

                        reportColumn = new ArrayList();
                        reportColumn.Add(("Total Amount").PadRight(Header1Width1, char.Parse(" ")) + ": ");
                        //reportColumn.Add("");
                        // Total Amount will be added in the group ending
                        //LineNoOfTotalAmount = reportContent.Count;
                        reportContent.Add(reportColumn);
                        TotalAmountRow = (ArrayList)reportContent[reportContent.Count - 1];
                    }
                    group1Key = groupKey;
                    group2Key = String.Empty;
                    group1Changed = true;
                }
                else
                    group1Changed = false;

                // Group Header 2 - group by Customer, Destination, QA Commission, Vendor Payment Discount and LabTestIncome
                groupKey = lc.Customer.CustomerCode + "|" + lc.CustomerDestination.DestinationCode
                            + '|' + lc.QACommissionPercent.ToString("N02") +'|' + lc.VendorPaymentDiscountPercent.ToString("N02")
                            + '|' + lc.LabTestIncome.ToString("N04");
                if (groupKey != group2Key || group1Changed)
                {
                    // new group of QA commission, customer code and customer destination
                    if (!group1Changed)
                        reportContent.Add(new ArrayList());

                    // Show The QA Commission Line if it is not zero
                    if (lc.QACommissionPercent != 0)
                    {
                        reportColumn = new ArrayList();
                        if (lc.QACommissionPercent == Convert.ToInt16(lc.QACommissionPercent))
                            // no decimal places
                            QAPercent = lc.QACommissionPercent.ToString("N0");
                        else
                            QAPercent = lc.QACommissionPercent.ToString("N02");

                        if (isExtractionFormat)
                            groupDesc = QAPercent + "% QA Commission will be deducted from payment";
                        else
                            groupDesc = QAPercent + " Percent Discount Q" + QAPercent;
                        reportColumn.Add(groupDesc);
                        reportContent.Add(reportColumn);
                    }

                    // Show The Vendor Payment Discount Line if it is not zero
                    if (lc.VendorPaymentDiscountPercent != 0)
                    {
                        reportColumn = new ArrayList();
                        if (lc.VendorPaymentDiscountPercent == Convert.ToInt16(lc.VendorPaymentDiscountPercent))
                            // no decimal places
                            VPPercent = lc.VendorPaymentDiscountPercent.ToString("N0");
                        else
                            VPPercent = lc.VendorPaymentDiscountPercent.ToString("N02");

                        if (isExtractionFormat)
                            groupDesc = VPPercent + " Percent Discount";
                        else
                            groupDesc = VPPercent + " Percent Discount";
                        reportColumn.Add(groupDesc);
                        reportContent.Add(reportColumn);
                    }

                    // Show The Lab Test Income Line if it is not zero
                    if (lc.LabTestIncome != 0)
                    {
                        reportColumn = new ArrayList();
                        LabTestIncome = lc.LabTestIncome.ToString("N4");
                        groupDesc = "Lab Test Income " + LabTestIncome + " Per Unit";
                        reportColumn.Add(groupDesc);
                        reportContent.Add(reportColumn);
                    }


                    // column heading for LC detail
                    reportColumn = new ArrayList();
                    if (!textOnly) reportColumn.Add("<TableHeader>");

                    reportColumn = new ArrayList();
                    if (!textOnly) reportColumn.Add("<TableHeader>");
                    reportColumn.Add("".PadRight(colWidth1, char.Parse(" ")) + " ");
                    reportColumn.Add("".PadLeft(colWidth2, char.Parse(" ")) + " ");
                    reportColumn.Add("".PadRight(colWidth3, char.Parse(" ")) + " ");
                    reportColumn.Add("".PadLeft(colWidth4, char.Parse(" ")) + " ");
                    reportColumn.Add("".PadLeft(colWidth5, char.Parse(" ")) + " ");
                    reportColumn.Add("FABRIC COST".PadLeft(colWidth6, char.Parse(" ")));
                    reportContent.Add(reportColumn);

                    reportColumn = new ArrayList();
                    if (!textOnly) reportColumn.Add("<TableHeader>");
                    reportColumn.Add("".PadRight(colWidth1, char.Parse(" ")) + " ");
                    reportColumn.Add("".PadLeft(colWidth2, char.Parse(" ")) + " ");
                    reportColumn.Add("".PadRight(colWidth3, char.Parse(" ")) + " ");
                    reportColumn.Add("".PadLeft(colWidth4, char.Parse(" ")) + " ");
                    reportColumn.Add("".PadLeft(colWidth5, char.Parse(" ")) + " ");
                    reportColumn.Add("DEDUCTION".PadLeft(colWidth6, char.Parse(" ")));
                    reportContent.Add(reportColumn);

                    reportColumn = new ArrayList();
                    if (!textOnly) reportColumn.Add("<TableHeader>");
                    reportColumn.Add("ITEM".PadRight(colWidth1, char.Parse(" ")) + " ");
                    reportColumn.Add("QTY".PadLeft(colWidth2, char.Parse(" ")) + " ");
                    reportColumn.Add("UNIT".PadRight(colWidth3, char.Parse(" ")) + " ");
                    reportColumn.Add("U/PRC".PadLeft(colWidth4, char.Parse(" ")) + " ");
                    reportColumn.Add("DATE".PadLeft(colWidth5, char.Parse(" ")) + " ");
                    reportColumn.Add("(USD)".PadLeft(colWidth6, char.Parse(" ")));
                    reportContent.Add(reportColumn);

                    // customer and desctination section
                    reportColumn = new ArrayList();
                    if (!textOnly) reportColumn.Add("<TableHeader>");
                    if (lc.Customer.CustomerId == 34 || lc.Customer.CustomerId == 35)
                        groupDesc = lc.CustomerDestination.DestinationDesc + " ORDER";
                    else
                        groupDesc = lc.Customer.CustomerCode + " ORDER";
                    groupDesc = groupDesc.PadRight(Header2Width1, char.Parse(" "));
                    reportColumn.Add(groupDesc);

                    groupDesc = " DESTINATION:" + lc.CustomerDestination.DestinationDesc;
                    reportColumn.Add(groupDesc);
                    reportContent.Add(reportColumn);

                    group2Key = groupKey;

                    lastItemNo = string.Empty;
                    lastDlyDate = DateTime.MinValue;
                }

                //LC Batch Detail

                reportColumn = new ArrayList();

                //string deductAmt = amt == 0 ? "--" : amt.ToString();
                string deductAmt = string.Empty;

                if (lastItemNo != lc.Product.ItemNo || lastDlyDate != lc.SupplierAtWarehouseDate)
                    //deduction = lc.ExpectedDeductAmt;
                    deduction = lc.ActualDeductAmt;
                lastItemNo = lc.Product.ItemNo;
                lastDlyDate = lc.SupplierAtWarehouseDate;
                if (lc.POQty * lc.ReducedSupplierGmtPrice < deduction)
                {
                    deduction -= (lc.POQty * lc.ReducedSupplierGmtPrice);
                    amt = (lc.POQty * lc.ReducedSupplierGmtPrice);
                }
                else
                {
                    amt = deduction;
                    deduction = 0;
                }

                if (lc.Currency.CurrencyId != 3)
                {
                    decimal usdExRate = 0m;
                    decimal exRate = 0m;
                    usdExRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, CurrencyId.USD.Id, lc.SupplierAtWarehouseDate);
                    exRate = CommonWorker.Instance.getExchangeRate(ExchangeRateType.INVOICE, lc.Currency.CurrencyId, lc.SupplierAtWarehouseDate);
                    amt = Math.Round(amt * Math.Round(exRate / usdExRate, 4), 2);
                }

                int spaces = colWidth6 - amt.ToString("#.00").Length;
                int padLeft = spaces / 2 + amt.ToString("#.00").Length;
                deductAmt = amt == 0 ? "--" : amt.ToString("#.00");

                if (!textOnly) reportColumn.Add("<TableDetail>");
                reportColumn.Add(lc.Product.ItemNo.PadRight(colWidth1, char.Parse(" ")) + " ");
                reportColumn.Add(lc.POQty.ToString().PadLeft(colWidth2, char.Parse(" ")) + " ");
                reportColumn.Add(lc.PackingUnit.PackingUnitDescription.PadRight(colWidth3, char.Parse(" ")) + " ");
                reportColumn.Add(lc.ReducedSupplierGmtPrice.ToString("F02").PadLeft(colWidth4, char.Parse(" ")) + " ");
                reportColumn.Add(lc.SupplierAtWarehouseDate.ToString("dd/MM/yy").PadLeft(colWidth5, char.Parse(" ")) + " ");
                reportColumn.Add(deductAmt.PadLeft(colWidth6, char.Parse(" ")) + " ");
                reportContent.Add(reportColumn);

                TotalAmount += lc.ReducedSupplierGmtPrice * lc.POQty - amt;
                if (!ProductTeamList.Contains(lc.ProductTeam.CodeDescription))
                    ProductTeamList.Add(lc.ProductTeam.CodeDescription);
            }
            //if (LineNoOfTotalAmount > 0)
            //    ((ArrayList)reportContent[LineNoOfTotalAmount]).Add(BatchCurrency + " " + TotalAmount.ToString("N02"));
            TotalAmountRow.Add(BatchCurrency + " " + TotalAmount.ToString("N02"));
            //string ProductTeamDesc = string.Empty;
            //foreach (string desc in ProductTeamList)
            //    ProductTeamDesc += (ProductTeamDesc == string.Empty ? "" : ", ") + desc;
            //ProductTeamRow.Add(ProductTeamDesc);
            return reportContent;
        }


        public void updateShipmentLCInfo(ArrayList lcShipmentRefs, int userId)
        {
            int IncompleteRow;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                LCApplicationDef lcApplication = new LCApplicationDef();
                ArrayList amendmentList = new ArrayList();

                IncompleteRow = 0;
                foreach (LCShipmentRef lcInfo in lcShipmentRefs)
                {
                    if (lcInfo.WorkflowStatus.Id == LCWFS.APPLIED.Id || lcInfo.WorkflowStatus.Id == LCWFS.COMPLETED.Id || lcInfo.WorkflowStatus.Id == LCWFS.LC_CANCELLED.Id)
                    {
                        if (lcInfo.SplitShipmentId <= 0)
                            lcWorker.updateInvoiceLCInfo(lcInfo, amendmentList, userId);
                        else
                            lcWorker.updateSplitShipmentLCInfo(lcInfo, amendmentList, userId);

                        lcApplication = lcWorker.getLCApplicationShipmentByKey(lcInfo.LCApplication.LCApplicationId, lcInfo.ShipmentId, lcInfo.SplitShipmentId);
                        lcWorker.LCShipmentCopyToLCApplication(lcInfo, lcApplication);
                        lcApplication.Status = GeneralStatus.ACTIVE.Code;
                        if (lcInfo.WorkflowStatus.Id != LCWFS.LC_CANCELLED.Id)
                        {
                            if (lcInfo.LCExpiryDate == DateTime.MinValue || lcInfo.LCIssueDate == DateTime.MinValue || lcInfo.LCNo == "" || lcInfo.LCAmt == decimal.MinValue)
                            {
                                lcApplication.WorkflowStatus = LCWFS.APPLIED;
                                IncompleteRow++;
                            }
                            else
                                lcApplication.WorkflowStatus = LCWFS.COMPLETED;
                        }
                        lcWorker.updateLCApplicationShipment(lcApplication, userId);
                    }
                }
                if (amendmentList.Count > 0)
                {
                    foreach (ActionHistoryDef actionHistory in amendmentList)
                    {
                        ShippingWorker.Instance.updateActionHistory(actionHistory);
                    }

                    // log the action into ActionHistory
                    ActionHistoryDef action = new ActionHistoryDef();
                    action.ShipmentId = -1;
                    action.SplitShipmentId = -1;
                    action.ActionDate = DateTime.Now;
                    action.ActionHistoryType = ActionHistoryType.LC_APPLICATION;
                    action.Remark = "Update L/C Batch : LC Batch No=" + ((LCShipmentRef)lcShipmentRefs[0]).LCBatch.LCBatchNo.ToString() + ", L/C Batch Status=" + (IncompleteRow > 0 ? LCWFS.APPLIED.Name : LCWFS.COMPLETED.Name);     //    + ", User ID=" + userId.ToString();
                    action.Status = 1;
                    action.User = generalWorker.getUserByKey(userId);
                    ShippingWorker.Instance.updateActionHistory(action);
                }
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


        public void updateLCInfoOfNewApplication(ArrayList lcShipmentRefs, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.RequiresNew);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                LCApplicationDef lcApplication; // = new LCApplicationDef();
                ArrayList amendmentList = new ArrayList();

                foreach (LCShipmentRef lcInfo in lcShipmentRefs)
                {
                    if (lcInfo.WorkflowStatus.Id == LCWFS.NEW.Id)
                    {
                        if (lcInfo.SplitShipmentId <= 0)
                            lcWorker.updateInvoiceLCInfo(lcInfo, amendmentList, userId);
                        else
                            lcWorker.updateSplitShipmentLCInfo(lcInfo, amendmentList, userId);

                        lcApplication = lcWorker.getLCApplicationShipmentByShipmentId(lcInfo.ShipmentId, lcInfo.SplitShipmentId);
                        if (!string.IsNullOrEmpty(lcInfo.LCNo))
                        {
                            //lcApplication.WorkflowStatus = LCWFS.COMPLETED;
                            //lcApplication.LCBatchId = lcInfo.LCBatch.LCBatchId;
                            lcApplication.Status = 0;
                            amendmentList.Add(new ActionHistoryDef(lcInfo.ShipmentId, lcInfo.SplitShipmentId, ActionHistoryType.LC_APPLICATION, null, "L/C Application #" + lcInfo.LCApplication.LCApplicationNo.ToString() + " is removed", userId));
                        }
                        lcWorker.updateLCApplicationShipment(lcApplication, userId);
                    }
                }
                if (amendmentList.Count > 0)
                    foreach (ActionHistoryDef actionHistory in amendmentList)
                        ShippingWorker.Instance.updateActionHistory(actionHistory);

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
    }
}
