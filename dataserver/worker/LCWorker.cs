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
using com.next.isam.dataserver.model.shipping;
using com.next.isam.dataserver.model.order;
using com.next.isam.domain.shipping;
using com.next.isam.domain.types;
using com.next.isam.domain.common;

namespace com.next.isam.dataserver.worker

{
    public class LCWorker : Worker
    {
        private static LCWorker _instance;
        private GeneralWorker generalWorker;
        private CommonWorker commonWorker;
        private ProductWorker productWorker;


        protected LCWorker()
        {
            generalWorker = GeneralWorker.Instance;
            commonWorker = CommonWorker.Instance;
            productWorker = ProductWorker.Instance;
        }


        public static LCWorker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LCWorker();
                }
                return _instance;
            }
        }


        public string generateHtmlTable(ArrayList tableRow)
        {
            int i;
            int r;
            int StyleRow;
            string sHtml;

            StyleRow = 0;
            sHtml = "<TABLE BORDER=1 CELLPADDING=2 CELLSPACING=0>\n";
            for (r = 0; r < tableRow.Count; r++)
            {
                if (((ArrayList)tableRow[r]).Count > 0)
                    switch (((ArrayList)tableRow[r])[0].ToString())
                    {
                        case "STYLE":
                            StyleRow = r;
                            break;
                        case "HEADER":
                            sHtml += "<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#CCCCFF;'>\n";
                            for (i = 1; i < ((ArrayList)tableRow[StyleRow]).Count; i++)
                            {
                                sHtml += "<TH " + (string)(((ArrayList)tableRow[StyleRow])[i]) + ">";
                                sHtml += (string)(((ArrayList)tableRow[r])[i]);
                                sHtml += "</TH>\n";
                            }
                            sHtml += "</TR>\n";
                            break;
                        case "DETAIL":
                            sHtml += "<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#FFFFFF;'>\n";
                            for (i = 1; i < ((ArrayList)tableRow[StyleRow]).Count; i++)
                            {
                                sHtml += "<TD " + (string)(((ArrayList)tableRow[StyleRow])[i]) + ">";
                                if (i < ((ArrayList)tableRow[r]).Count)
                                    sHtml += (string)(((ArrayList)tableRow[r])[i]);
                                sHtml += "</TD>\n";
                            }
                            sHtml += "</TR>\n";
                            break;
                        default:
                            sHtml += "<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#FF0000;'>\n";
                            for (i = 0; i < ((ArrayList)tableRow[r]).Count; i++)
                                sHtml += "<TD>" + (string)(((ArrayList)tableRow[r])[i]) + "</TD>\n";
                            sHtml += "</TR>\n";
                            break;
                    }
            }
            sHtml = sHtml + "</table>\n";
            return sHtml;
        }


        public string exportReportArrayToText(ArrayList reportArray, string fileNamePrefix, int userId)
        {
            ArrayList columns;
            string textLine;
            int i, j;

            string fileFolder = WebConfig.getValue("appSettings", "LC_APPLICATION");
            string fileName = fileFolder + fileNamePrefix + ".txt";
            if (File.Exists(fileName))
                File.Delete(fileName);
            StreamWriter s = File.CreateText(fileName);
            textLine = String.Empty;
            for (i = 0; i < reportArray.Count; i++)
            {
                columns = (ArrayList)reportArray[i];
                textLine = String.Empty;
                for (j = 0; j < columns.Count; textLine = textLine + columns[j++]) { }
                s.WriteLine(textLine);
            }
            s.Close();
            return fileName;
        }


        public string convertReportArrayToText(ArrayList reportArray)
        {
            ArrayList columns;
            string textBody;
            int i, j;

            textBody = String.Empty;
            for (i = 0; i < reportArray.Count; i++)
            {
                columns = (ArrayList)reportArray[i];
                if (columns.Count > 0)
                    for (j = 0; j < columns.Count; j++)
                        textBody += columns[j];
                textBody += "\n";
            }
            return textBody;
        }

        public string convertReportArrayToHtml(ArrayList reportArray)
        {
            ArrayList rowItems, columnWidth;
            int maxColumn, itemWidth, lineWidth, maxLineWidth;
            string htmlBody;
            int i, j, k;
            int maxColumnLine;
            int colSpan;
            bool inTable;
            string rowStyle;

            maxColumn = 0;
            maxLineWidth = 0;
            maxColumnLine = 0;

            for (i = 0; i < reportArray.Count; i++)
            {
                lineWidth = 0;
                for (j = 0; j < ((ArrayList)reportArray[i]).Count; j++)
                    if (j > 0 || ((((ArrayList)reportArray[i])[0].ToString() != "<TableHeader>") && (((ArrayList)reportArray[i])[0].ToString() != "<TableDetail>")))
                        lineWidth += ((ArrayList)reportArray[i])[j].ToString().Length;
                if (lineWidth > maxLineWidth) maxLineWidth = lineWidth;
                if (((ArrayList)reportArray[i]).Count > maxColumn)
                {
                    maxColumn = ((ArrayList)reportArray[i]).Count;
                    maxColumnLine = i;
                }
            }

            columnWidth = new ArrayList();
            for (i = 0; i < ((ArrayList)reportArray[maxColumnLine]).Count; i++)
                columnWidth.Add(((ArrayList)reportArray[maxColumnLine])[i].ToString().Length);
            columnWidth.Add(0);
            //<TR STYLE='FONT-SIZE:12;BACKGROUND-COLOR:#FFFFFF;'>\n"

            rowStyle = "";
            htmlBody = "<table border=0 STYLE='FONT-SIZE:11;' >";
            inTable = false;
            for (i = 0; i < reportArray.Count; i++)
            {
                rowItems = (ArrayList)reportArray[i];
                if (rowItems.Count == 0) rowItems.Add("&nbsp;");
                if (inTable)
                    switch (rowItems[0].ToString())
                    {
                        case "<TableHeader>":
                            //rowStyle = "BACKGROUND-COLOR:#CCCCFF;";
                            rowStyle = "";
                            break;
                        case "<TableDetail>":
                            //rowStyle = "BACKGROUND-COLOR:#FFFFFF;";
                            rowStyle = "";
                            break;
                        default:
                            htmlBody += "</table></td>";
                            rowStyle = "";
                            inTable = false;
                            break;
                    }
                else
                    switch (rowItems[0].ToString())
                    {
                        case "<TableHeader>":
                            htmlBody += "<tr><td colspan='" + maxColumn.ToString() + "'><table border=0 STYLE='FONT-SIZE:11;'>";
                            //rowStyle = "BACKGROUND-COLOR:#CCCCFF;";
                            rowStyle = "";
                            inTable = true;
                            break;
                        case "<TableDetail>":
                            //rowStyle = "BACKGROUND-COLOR:#FFFFFF;";
                            rowStyle = "";
                            inTable = true;
                            break;
                        default:
                            rowStyle = "";
                            inTable = false;
                            break;
                    }

                htmlBody += "<tr>";
                if (inTable)
                {
                    k = 0;
                    for (j = 1; j < rowItems.Count; j++)
                    {
                        if (rowItems.Count == 3)
                        {   // sub header
                            colSpan = (j == 1 ? 2 : (maxColumn - j));
                            htmlBody += "<td colspan='" + colSpan.ToString() + "' style='" + rowStyle + "'>";
                        }
                        else
                        {
                            htmlBody += "<td colspan='1' style='" + rowStyle + "'>";
                        }

                        htmlBody += rowItems[j].ToString();
                        htmlBody += "</td>";
                    }
                }
                else
                {
                    k = 0;
                    for (j = 0; j < rowItems.Count; j++)
                    {
                        itemWidth = 0;
                        for (colSpan = 1; k < columnWidth.Count; colSpan++)
                        {
                            itemWidth += (int)columnWidth[k++];
                            if (rowItems[j].ToString().Length <= itemWidth) break;
                        }
                        htmlBody += "<td colspan='" + colSpan + "'>";
                        htmlBody += rowItems[j].ToString();
                        htmlBody += "</td>";
                    }
                }

                //                    htmlBody += "<td>&nbsp;</td>"; // blank line

                htmlBody += "</tr>";
            }
            htmlBody += "</table>";

            return htmlBody;
        }


        public int getLCShipment(ArrayList list, int vendorId, string itemNo, ArrayList officeIdList, ArrayList productTeamIdList, int countryOfOriginId, DateTime fromAtWarehouseDate, DateTime toAtWarehouseDate,
            DateTime fromLCApplicationDate, DateTime toLCApplicationDate, int fromLCApplicationNo, int toLCApplicationNo, int lcAppSubmitStatus, int NumberOfRecordToGet)
        {
            int i;

            //ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("LCShipmentApt", "GetLCShipment");

            if (vendorId == -1)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            if (itemNo == String.Empty)
                ad.SelectCommand.Parameters["@ItemNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;

            //if (officeIdList == String.Empty)
            //    ad.SelectCommand.Parameters["@OfficeId"].Value = DBNull.Value ;
            //else
            //    ad.SelectCommand.Parameters["@OfficeId"].Value = officeIdList ;

            if (countryOfOriginId == -1)
                ad.SelectCommand.Parameters["@CountryOfOriginId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@CountryOfOriginId"].Value = countryOfOriginId;

            if (fromAtWarehouseDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromAtWarehouseDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromAtWarehouseDate"].Value = fromAtWarehouseDate;

            if (toAtWarehouseDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToAtWarehouseDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToAtWarehouseDate"].Value = toAtWarehouseDate;


            if (fromLCApplicationDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromLCApplicationDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromLCApplicationDate"].Value = fromLCApplicationDate;

            if (toLCApplicationDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToLCApplicationDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToLCApplicationDate"].Value = toLCApplicationDate;

            if (fromLCApplicationNo == int.MinValue)
                ad.SelectCommand.Parameters["@FromLCApplicationNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromLCApplicationNo"].Value = fromLCApplicationNo;

            if (toLCApplicationNo == int.MinValue)
                ad.SelectCommand.Parameters["@ToLCApplicationNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToLCApplicationNo"].Value = toLCApplicationNo;

            TypeCollector officeIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < officeIdList.Count; i++)
                officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);


            TypeCollector productTeamIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < productTeamIdList.Count; i++)
                productTeamIdCollector.append(int.Parse(productTeamIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@ProductTeamIdList"] = CustomDataParameter.parse(productTeamIdCollector.IsInclusive, productTeamIdCollector.Values);

            if (lcAppSubmitStatus == -1)
                ad.SelectCommand.Parameters["@LCAppSubmitStatus"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCAppSubmitStatus"].Value = lcAppSubmitStatus;

            LCShipmentDs ds = new LCShipmentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);

            int recordCount = 0;
            foreach (LCShipmentDs.LCShipmentRow lcShipment in ds.LCShipment)
            {
                if (recordCount++ >= NumberOfRecordToGet && NumberOfRecordToGet > 0) break;
                LCShipmentRef rf = new LCShipmentRef();
                LCShipmentMapping(lcShipment, rf);
                list.Add(rf);
            }
            return recordsAffected;
            //return list;
        }


        public ArrayList getLCApplicationShipmentByLCBatchId(int lcBatchId)
        {
            ArrayList list = new ArrayList();
            LCShipmentDs ds = new LCShipmentDs();

            IDataSetAdapter ad = getDataSetAdapter("LCShipmentApt", "GetLCApplicationShipmentByLCBatchId");
            ad.SelectCommand.Parameters["@LCBatchId"].Value = lcBatchId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            foreach (LCShipmentDs.LCShipmentRow lcShipment in ds.LCShipment)
            {
                LCShipmentRef rf = new LCShipmentRef();
                LCShipmentMapping(lcShipment, rf);
                list.Add(rf);
            }
            return list;
        }


        public ArrayList getLCApplicationShipment(int vendorId, TypeCollector officeIdList, TypeCollector departmentIdList, TypeCollector productTeamIdList, TypeCollector lcWorkflowStatusIdList,
                    DateTime fromLCApplicationDate, DateTime toLCApplicationDate, string fromLCApplicationNo, string toLCApplicationNo)
        {
            int i;
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("LCShipmentApt", "GetLCApplicationShipmentByCriteria");

            if (vendorId == -1)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DeptIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.CustomParameters["@ProdTeamIdList"] = CustomDataParameter.parse(productTeamIdList.IsInclusive, productTeamIdList.Values);
            ad.SelectCommand.CustomParameters["@LCWorkflowStatusIdList"] = CustomDataParameter.parse(lcWorkflowStatusIdList.IsInclusive, lcWorkflowStatusIdList.Values);

            if (fromLCApplicationDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromLCAppDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromLCAppDate"].Value = fromLCApplicationDate;

            if (toLCApplicationDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToLcAppDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToLCAppDate"].Value = toLCApplicationDate;

            if (fromLCApplicationNo == String.Empty)
                ad.SelectCommand.Parameters["@FromLCAppNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromLCAppNo"].Value = fromLCApplicationNo;

            if (toLCApplicationNo == String.Empty)
                ad.SelectCommand.Parameters["@ToLCAppNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToLCAppNo"].Value = toLCApplicationNo;

            //if (lcApplicationStatusIdList == String.Empty)
            //    ad.SelectCommand.Parameters["@LCWFS"].Value = DBNull.Value;
            //else
            //    ad.SelectCommand.Parameters["@LCWFS"].Value = lcApplicationStatusIdList;

            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            LCShipmentDs ds = new LCShipmentDs();

            int recordsAffected = ad.Fill(ds);

            foreach (LCShipmentDs.LCShipmentRow lcShipment in ds.LCShipment)
            {
                LCShipmentRef rf = new LCShipmentRef();
                LCShipmentMapping(lcShipment, rf);
                list.Add(rf);
            }
            return list;
        }

        public int getLCAdvancePayment(ArrayList list, int vendorId, string itemNo, string contractNo, ArrayList officeIdList, ArrayList productTeamIdList, DateTime fromAtWarehouseDate, DateTime toAtWarehouseDate,
            DateTime fromLCIssueDate, DateTime toLCIssueDate, string fromLCNo, string toLCNo, int fromLCBatchNo, int toLCBatchNo, string fromAdvancePaymentNo, string toAdvancePaymentNo, 
            string fromNSLRefNo, string toNSLRefNo, DateTime fromActionDate, DateTime toActionDate, int isC19Order, int NumberOfRecordToGet)
        {
            int i;

            //ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("LCAdvancePaymentApt", "GetLCAdvancePaymentList");
            
            if (vendorId == -1)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            if (itemNo == String.Empty)
                ad.SelectCommand.Parameters["@ItemNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ItemNo"].Value = itemNo;

            if (contractNo == String.Empty)
                ad.SelectCommand.Parameters["@ContractNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ContractNo"].Value = contractNo;

            if (fromAtWarehouseDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromAtWarehouseDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromAtWarehouseDate"].Value = fromAtWarehouseDate;

            if (toAtWarehouseDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToAtWarehouseDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToAtWarehouseDate"].Value = toAtWarehouseDate;

            if (fromLCIssueDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromLCIssueDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromLCIssueDate"].Value = fromLCIssueDate;

            if (toLCIssueDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToLCIssueDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToLCIssueDate"].Value = toLCIssueDate;

            if (fromLCNo == String.Empty)
                ad.SelectCommand.Parameters["@FromLCNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromLCNo"].Value = fromLCNo;

            if (toLCNo == String.Empty)
                ad.SelectCommand.Parameters["@ToLCNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToLCNo"].Value = toLCNo;

            if (fromLCBatchNo == int.MinValue)
                ad.SelectCommand.Parameters["@FromLCBatchNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromLCBatchNo"].Value = fromLCBatchNo;

            if (toLCBatchNo == int.MinValue)
                ad.SelectCommand.Parameters["@ToLCBatchNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToLCBatchNo"].Value = toLCBatchNo;

            if (fromAdvancePaymentNo == string.Empty)
                ad.SelectCommand.Parameters["@FromAdvancePaymentNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromAdvancePaymentNo"].Value = fromAdvancePaymentNo;
            if (toAdvancePaymentNo == string.Empty)
                ad.SelectCommand.Parameters["@ToAdvancePaymentNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToAdvancePaymentNo"].Value = toAdvancePaymentNo;

            if (fromNSLRefNo == String.Empty)
                ad.SelectCommand.Parameters["@FromNSLRefNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromNSLRefNo"].Value = fromNSLRefNo;
            if (toNSLRefNo == String.Empty)
                ad.SelectCommand.Parameters["@ToNSLRefNo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToNSLRefNo"].Value = toNSLRefNo;

            if (fromActionDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@FromActionDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@FromActionDate"].Value = fromActionDate;

            if (toActionDate == DateTime.MinValue)
                ad.SelectCommand.Parameters["@ToActionDate"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@ToActionDate"].Value = toActionDate;

            if (isC19Order == -1)
                ad.SelectCommand.Parameters["@IsC19Order"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@IsC19Order"].Value = isC19Order;

            TypeCollector officeIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < officeIdList.Count; i++)
                officeIdCollector.append(Convert.ToInt32(officeIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdCollector.IsInclusive, officeIdCollector.Values);

            TypeCollector productTeamIdCollector = TypeCollector.Inclusive;
            for (i = 0; i < productTeamIdList.Count; i++)
                productTeamIdCollector.append(int.Parse(productTeamIdList[i].ToString()));
            ad.SelectCommand.CustomParameters["@ProductTeamIdList"] = CustomDataParameter.parse(productTeamIdCollector.IsInclusive, productTeamIdCollector.Values);
            
            LCAdvancePaymentDs ds = new LCAdvancePaymentDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);

            int recordCount = 0;
            foreach (LCAdvancePaymentDs.LCAdvancePaymentRow lcap in ds.LCAdvancePayment)
            {
                if (recordCount++ >= NumberOfRecordToGet && NumberOfRecordToGet > 0) break;
                LCAdvancePaymentRef rf = new LCAdvancePaymentRef();
                LCAdvancePaymentMapping(lcap, rf);
                list.Add(rf);
            }
            return recordsAffected;
            //return list;
        }


        private int getMaxLCApplicationId()
        {
            IDataSetAdapter ad = getDataSetAdapter("LCApplicationApt", "GetMaxLCApplicationId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }


        private int getMaxLCApplicationNo()
        {
            IDataSetAdapter ad = getDataSetAdapter("LCApplicationApt", "GetMaxLCApplicationNo");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public LCApplicationRef getLCApplicationByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCApplicationApt", "GetLCApplicationByKey");
            ad.SelectCommand.Parameters["@LCApplicationId"].Value = key.ToString();

            LCApplicationDs dataSet = new LCApplicationDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            LCApplicationRef rf = new LCApplicationRef();
            LCApplicationMapping(dataSet.LCApplication[0], rf);
            return rf;
        }

        public DataTable getTableLCApplicationByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCApplicationApt", "GetLCApplicationByKey");
            ad.SelectCommand.Parameters["@LCApplicationId"].Value = key.ToString();

            LCApplicationDs dataSet = new LCApplicationDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            LCApplicationRef rf = new LCApplicationRef();
            return dataSet.LCApplication;

        }

        public void updateLCApplication(LCApplicationRef rf, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("LCApplicationApt", "GetLCApplicationByKey");
                ad.SelectCommand.Parameters["@LCApplicationId"].Value = rf.LCApplicationId;
                ad.PopulateCommands();

                LCApplicationDs dataSet = new LCApplicationDs();
                LCApplicationDs.LCApplicationRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.LCApplication[0];
                    this.LCApplicationMapping(rf, row);
                }
                else
                {
                    row = dataSet.LCApplication.NewLCApplicationRow();
                    rf.LCApplicationId = this.getMaxLCApplicationId() + 1;
                    rf.LCApplicationNo = this.getMaxLCApplicationNo() + 1;
                    rf.Status = 1;
                    this.LCApplicationMapping(rf, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.LCApplication.AddLCApplicationRow(row);
                }

                recordsAffected = ad.Update(dataSet);

                if (recordsAffected < 1)
                    throw new DataAccessException("Update LC Application ERROR");
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


        public LCApplicationDef getLCApplicationShipmentByKey(int lcApplicationId, int shipmentId, int splitShipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCApplicationShipmentApt", "GetLCApplicationShipmentByKey");
            ad.SelectCommand.Parameters["@LCApplicationId"].Value = lcApplicationId;
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ad.SelectCommand.Parameters["@SplitShipmentId"].Value = splitShipmentId;

            LCApplicationShipmentDs dataSet = new LCApplicationShipmentDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            LCApplicationDef df = new LCApplicationDef();
            LCApplicationShipmentMapping(dataSet.LCApplicationShipment[0], df);
            return df;
        }


        public LCApplicationDef getLCApplicationShipmentByShipmentId(int shipmentId, int splitShipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCApplicationShipmentApt", "GetLCApplicationShipmentByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            //if (splitShipmentId == int.MinValue)
            //    splitShipmentId = GeneralCriteria.ALL;
            ad.SelectCommand.Parameters["@SplitShipmentId"].Value = splitShipmentId;

            LCApplicationShipmentDs dataSet = new LCApplicationShipmentDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            LCApplicationDef df = new LCApplicationDef();
            LCApplicationShipmentMapping(dataSet.LCApplicationShipment[0], df);
            return df;
        }

        public ArrayList getLCApplicationShipmentDetail(LCApplicationDef lcApplicationShipment)
        {
            int i;

            IDataSetAdapter ad = getDataSetAdapter("LCApplicationShipmentDetailApt", "GetLCApplicationShipmentDetail");
            ad.SelectCommand.Parameters["@LCApplicationId"].Value = lcApplicationShipment.LCApplicationId;
            ad.SelectCommand.Parameters["@ShipmentId"].Value = lcApplicationShipment.ShipmentId;
            ad.SelectCommand.Parameters["@SplitShipmentId"].Value = lcApplicationShipment.SplitShipmentId;
            ad.PopulateCommands();

            LCApplicationShipmentDetailDs dsAppDetail = new LCApplicationShipmentDetailDs();
            int recordsAffected = ad.Fill(dsAppDetail);
            if (recordsAffected < 1) return null;

            ArrayList LCApplicationShipmentDetail = new ArrayList();
            for (i = 0; i < dsAppDetail.Tables[0].Rows.Count; i++)
            {
                LCApplicationShipmentDetailDef df = new LCApplicationShipmentDetailDef();
                this.LCApplicationShipmentDetailMapping(dsAppDetail.Tables[0].Rows[i], df);
                LCApplicationShipmentDetail.Add(df);
            }
            return LCApplicationShipmentDetail;
        }

        public ArrayList getLCShipmentDetailByShipmentId(int shipmentId, int splitShipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCApplicationShipmentDetailApt", "GetLCShipmentDetailByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ad.SelectCommand.Parameters["@SplitShipmentId"].Value = splitShipmentId;

            LCApplicationShipmentDetailDs dsAppDetail = new LCApplicationShipmentDetailDs();
            int recordsAffected = ad.Fill(dsAppDetail);
            //if (recordsAffected < 1) return null;

            ArrayList LCApplicationShipmentDetail = new ArrayList();
            for (int i = 0; i < dsAppDetail.Tables[0].Rows.Count; i++)
            {
                LCApplicationShipmentDetailDef df = new LCApplicationShipmentDetailDef();
                this.LCApplicationShipmentDetailMapping(dsAppDetail.Tables[0].Rows[i], df);
                LCApplicationShipmentDetail.Add(df);
            }
            return LCApplicationShipmentDetail;
        }


        public void updateLCApplicationShipment(LCApplicationDef def, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("LCApplicationShipmentApt", "GetLCApplicationShipmentByKey");
                ad.SelectCommand.Parameters["@LCApplicationId"].Value = def.LCApplicationId;
                ad.SelectCommand.Parameters["@ShipmentId"].Value = def.ShipmentId;
                ad.SelectCommand.Parameters["@SplitShipmentId"].Value = def.SplitShipmentId;
                ad.PopulateCommands();

                LCApplicationShipmentDs dataSet = new LCApplicationShipmentDs();
                LCApplicationShipmentDs.LCApplicationShipmentRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.LCApplicationShipment[0];
                    LCBatchRef lcBatch;
                    string fromVal, toVal;
                    int fromId, toId;
                    string userName = generalWorker.getUserByKey(userId).DisplayName;

                    lcBatch = null;
                    fromId = (row.IsLCBatchIdNull() ? int.MinValue : row.LCBatchId);
                    if (fromId != int.MinValue) lcBatch = getLCBatchByKey(fromId);
                    fromVal = (lcBatch == null ? "" : lcBatch.LCBatchNo.ToString("LCB00000#"));

                    lcBatch = null;
                    toId = (def.LCBatchId == null || def.LCBatchId == int.MinValue ? int.MinValue : def.LCBatchId);
                    if (toId != int.MinValue) lcBatch = getLCBatchByKey(toId);
                    toVal = (lcBatch == null ? "" : lcBatch.LCBatchNo.ToString("LCB00000#"));

                    if (fromVal != toVal)
                    {
                        // log the action into ActionHistory
                        ActionHistoryDef action = (new ActionHistoryDef(row.ShipmentId, row.SplitShipmentId, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C Batch No. : " + fromVal + " -> " + toVal, userId));
                        ShippingWorker.Instance.updateActionHistory(action);
                    }

                    if ((row.IsDeducedFabricCostNull() ? 0 : row.DeducedFabricCost) != (def.DeducedFabricCost == decimal.MinValue ? 0 : def.DeducedFabricCost))
                    {
                        string fromAmt = (row.IsDeducedFabricCostNull() || row.DeducedFabricCost == decimal.MinValue ? "0" : row.DeducedFabricCost.ToString());
                        string toAmt = (def.DeducedFabricCost == decimal.MinValue ? "0" : def.DeducedFabricCost.ToString());
                        ActionHistoryDef action = (new ActionHistoryDef(row.ShipmentId, row.SplitShipmentId, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "Deducted Fabric Cost : " + fromAmt + " -> " + toAmt, userId));
                        ShippingWorker.Instance.updateActionHistory(action);
                    }

                    this.LCApplicationShipmentMapping(def, row);
                }
                else
                {
                    row = dataSet.LCApplicationShipment.NewLCApplicationShipmentRow();
                    this.LCApplicationShipmentMapping(def, row);
                    dataSet.LCApplicationShipment.AddLCApplicationShipmentRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update LC Application Shipment ERROR");

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


        public void updateLCApplicationShipmentDetail(LCApplicationDef lcApplicationShipment, ArrayList lcShipmentDetailList)
        {
            int i;
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("LCApplicationShipmentDetailApt", "GetLCApplicationShipmentDetail");
                ad.SelectCommand.Parameters["@LCApplicationId"].Value = lcApplicationShipment.LCApplicationId;
                ad.SelectCommand.Parameters["@ShipmentId"].Value = lcApplicationShipment.ShipmentId;
                ad.SelectCommand.Parameters["@SplitShipmentId"].Value = lcApplicationShipment.SplitShipmentId;
                ad.PopulateCommands();

                LCApplicationShipmentDetailDs dsDetail = new LCApplicationShipmentDetailDs();
                int recordsAffected = ad.Fill(dsDetail);
                LCApplicationShipmentDetailDs.LCApplicationShipmentDetailRow NewDetail;
                for (i = 0; i < lcShipmentDetailList.Count; i++)
                {
                    NewDetail = dsDetail.LCApplicationShipmentDetail.NewLCApplicationShipmentDetailRow();
                    this.LCApplicationShipmentDetailMapping(lcShipmentDetailList[i], NewDetail);
                    NewDetail.LCApplicationId = lcApplicationShipment.LCApplicationId;
                    NewDetail.Status = 1;
                    dsDetail.LCApplicationShipmentDetail.AddLCApplicationShipmentDetailRow(NewDetail);

                }
                recordsAffected = ad.Update(dsDetail);
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


        public void MarkDeleteLCApplicationShipmentDetail(LCApplicationDef lcApplicationShipment)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("LCApplicationShipmentDetailApt", "GetLCApplicationShipmentDetail");
                ad.SelectCommand.Parameters["@LCApplicationId"].Value = lcApplicationShipment.LCApplicationId;
                ad.SelectCommand.Parameters["@ShipmentId"].Value = lcApplicationShipment.ShipmentId;
                ad.SelectCommand.Parameters["@SplitShipmentId"].Value = lcApplicationShipment.SplitShipmentId;
                ad.PopulateCommands();

                LCApplicationShipmentDetailDs dsDetail = new LCApplicationShipmentDetailDs();
                int recordsAffected = ad.Fill(dsDetail);
                foreach (LCApplicationShipmentDetailDs.LCApplicationShipmentDetailRow row in dsDetail.LCApplicationShipmentDetail)
                    row.Status = 0;
                recordsAffected = ad.Update(dsDetail);
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


        private int getMaxLCBatchId()
        {
            IDataSetAdapter ad = getDataSetAdapter("LCBatchApt", "GetMaxLCBatchId");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }


        private int getMaxLCBatchNo()
        {
            IDataSetAdapter ad = getDataSetAdapter("LCBatchApt", "GetMaxLCBatchNo");
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }


        public LCBatchRef getLCBatchByKey(int key)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCBatchApt", "GetLCBatchByKey");
            ad.SelectCommand.Parameters["@LCBatchId"].Value = key.ToString();

            LCBatchDs dataSet = new LCBatchDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            LCBatchRef rf = new LCBatchRef();
            LCBatchMapping(dataSet.LCBatch[0], rf);
            return rf;
        }

        public LCBatchRef getLCBatchByShipmentId(int shipmentId)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCBatchApt", "GetLCBatchByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId.ToString();

            LCBatchDs dataSet = new LCBatchDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            LCBatchRef rf = new LCBatchRef();
            LCBatchMapping(dataSet.LCBatch[0], rf);
            return rf;
        }


        public LCBatchRef getLCBatchByCriteria(int lcBatchId, int lcBatchNo, int issuingBankId, int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCBatchApt", "GetLCBatchByCriteria");
            ad.SelectCommand.Parameters["@LCBatchId"].Value = lcBatchId;
            ad.SelectCommand.Parameters["@LCBatchNo"].Value = lcBatchNo;
            ad.SelectCommand.Parameters["@IssuingBankId"].Value = issuingBankId;
            ad.SelectCommand.Parameters["@CreatedBy"].Value = userId;

            LCBatchDs dataSet = new LCBatchDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            LCBatchRef rf = new LCBatchRef();
            LCBatchMapping(dataSet.LCBatch[0], rf);
            return rf;
        }


        public LCBatchSummaryRef getLCBatchSummaryByLCBatchId(int lcBatchId)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCBatchSummaryApt", "GetLCBatchSummaryByKey");
            ad.SelectCommand.Parameters["@LCBatchId"].Value = lcBatchId;

            LCBatchSummaryDs dataSet = new LCBatchSummaryDs();
            int recordsAffected = ad.Fill(dataSet);

            if (recordsAffected < 1) return null;

            LCBatchSummaryRef rf = new LCBatchSummaryRef();
            LCBatchSummaryMapping(dataSet.LCBatchSummary[0], rf);
            return rf;
        }


        public ArrayList getLCBatchSummaryByCriteria(TypeCollector officeIdList, int vendorId, int lcBatchNoFrom, int lcBatchNoTo, DateTime lcAppliedDateFrom, DateTime lcAppliedDateTo, string lcNoFrom, string lcNoTo)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCBatchSummaryApt", "GetLCBatchSummaryByCriteria");

            if (vendorId == -1)
                ad.SelectCommand.Parameters["@VendorId"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@VendorId"].Value = vendorId;

            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);

            if (lcAppliedDateFrom == DateTime.MinValue)
                ad.SelectCommand.Parameters["@LCApprovalDateFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCApprovalDateFrom"].Value = lcAppliedDateFrom;

            if (lcAppliedDateTo == DateTime.MinValue)
                ad.SelectCommand.Parameters["@LCApprovalDateTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCApprovalDateTo"].Value = lcAppliedDateTo;

            if (lcBatchNoFrom < 0)
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoFrom"].Value = lcBatchNoFrom;

            if (lcBatchNoTo < 0)
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCBatchNoTo"].Value = lcBatchNoTo;

            if (lcNoFrom == "")
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoFrom"].Value = lcNoFrom;

            if (lcNoTo == "")
                ad.SelectCommand.Parameters["@LCNoTo"].Value = DBNull.Value;
            else
                ad.SelectCommand.Parameters["@LCNoTo"].Value = lcNoTo;

            LCBatchSummaryDs dataSet = new LCBatchSummaryDs();
            ArrayList list = new ArrayList();
            int recordsAffected = ad.Fill(dataSet);
            foreach (LCBatchSummaryDs.LCBatchSummaryRow LCSummary in dataSet.LCBatchSummary)
            {
                LCBatchSummaryRef rf = new LCBatchSummaryRef();
                LCBatchSummaryMapping(LCSummary, rf);
                list.Add(rf);
            }
            return list;
        }

        public ArrayList getLCBatchDetailByLCBatchId(int lcBatchId)
        {
            return getLCBatchDetailByLCBatchId(lcBatchId, TypeCollector.Inclusive);
        }


        public ArrayList getLCBatchDetailByLCBatchId(int lcBatchId, TypeCollector shipmentIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCBatchDetailApt", "GetLCBatchDetailByLCBatchId");
            ad.SelectCommand.Parameters["@LCBatchId"].Value = lcBatchId;
            ad.SelectCommand.CustomParameters["@ShipmentIdList"] = CustomDataParameter.parse(shipmentIdList.IsInclusive, shipmentIdList.Values);

            LCBatchDetailDs dataSet = new LCBatchDetailDs();
            ArrayList list = new ArrayList();
            int recordsAffected = ad.Fill(dataSet);
            foreach (LCBatchDetailDs.LCBatchDetailRow LCBatchDetail in dataSet.LCBatchDetail)
            {
                LCBatchDetailRef rf = new LCBatchDetailRef();
                LCBatchDetailMapping(LCBatchDetail, rf);
                list.Add(rf);
            }
            return list;
        }

        public ArrayList getLCBatchProductTeamDetailByLCBatchId(int lcBatchId)
        {
            IDataSetAdapter ad = getDataSetAdapter("LCBatchDetailApt", "GetLCBatchProductTeamDetailByLCBatchId");
            ad.SelectCommand.Parameters["@LCBatchId"].Value = lcBatchId;

            LCBatchDetailDs dataSet = new LCBatchDetailDs();
            ArrayList list = new ArrayList();
            int recordsAffected = ad.Fill(dataSet);
            foreach (LCBatchDetailDs.LCBatchDetailRow LCBatchDetail in dataSet.LCBatchDetail)
            {
                LCBatchDetailRef rf = new LCBatchDetailRef();
                LCBatchDetailMapping(LCBatchDetail, rf);
                list.Add(rf);
            }
            return list;
        }


        public ArrayList getLCBatchDetailByApplicationIdList(TypeCollector lcApplicationIdList)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("LCBatchDetailApt", "getLCBatchDetailByApplicationIdList");
                ad.SelectCommand.CustomParameters["@LCApplicationIdList"] = CustomDataParameter.parse(lcApplicationIdList.IsInclusive, lcApplicationIdList.Values);

                LCBatchDetailDs dataSet = new LCBatchDetailDs();
                ArrayList list = new ArrayList();
                int recordsAffected = ad.Fill(dataSet);
                foreach (LCBatchDetailDs.LCBatchDetailRow LCBatchDetail in dataSet.LCBatchDetail)
                {
                    LCBatchDetailRef rf = new LCBatchDetailRef();
                    LCBatchDetailMapping(LCBatchDetail, rf);
                    list.Add(rf);
                }

                ctx.VoteCommit();
                return list;
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


        public ArrayList getLCBatchDetailByShipmentIdList(TypeCollector shipmentIdList, TypeCollector splitShipmentIdList, TypeCollector workflowStatusIdList)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.NotSupported);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                shipmentIdList.append(-1);
                splitShipmentIdList.append(-1);
                //IDataSetAdapter ad = getDataSetAdapter("LCBatchDetailApt", "getLCBatchDetailByApplicationIdList");
                IDataSetAdapter ad = getDataSetAdapter("LCBatchDetailApt", "getLCBatchDetailForApplyByShipmentIdList");
                ad.SelectCommand.CustomParameters["@ShipmentIdList"] = CustomDataParameter.parse(shipmentIdList.IsInclusive, shipmentIdList.Values);
                ad.SelectCommand.CustomParameters["@SplitShipmentIdList"] = CustomDataParameter.parse(splitShipmentIdList.IsInclusive, splitShipmentIdList.Values);
                ad.SelectCommand.CustomParameters["@WorkflowStatusIdList"] = CustomDataParameter.parse(workflowStatusIdList.IsInclusive, workflowStatusIdList.Values);

                LCBatchDetailDs dataSet = new LCBatchDetailDs();
                ArrayList list = new ArrayList();
                int recordsAffected = ad.Fill(dataSet);
                foreach (LCBatchDetailDs.LCBatchDetailRow LCBatchDetail in dataSet.LCBatchDetail)
                {
                    LCBatchDetailRef rf = new LCBatchDetailRef();
                    LCBatchDetailMapping(LCBatchDetail, rf);
                    list.Add(rf);
                }

                ctx.VoteCommit();
                return list;
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


        public void updateLCBatch(LCBatchRef rf, int userId)
        {
            int nLCBatchId;

            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("LCBatchApt", "GetLCBatchByKey");
                ad.SelectCommand.Parameters["@LCBatchId"].Value = rf.LCBatchId;
                ad.PopulateCommands();

                LCBatchDs dataSet = new LCBatchDs();
                LCBatchDs.LCBatchRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.LCBatch[0];
                    this.LCBatchMapping(rf, row);
                    recordsAffected = ad.Update(dataSet);
                    if (recordsAffected < 1)
                        throw new DataAccessException("Update LC Batch ERROR");
                }
                else
                {
                    row = dataSet.LCBatch.NewLCBatchRow();
                    rf.LCBatchId = this.getMaxLCBatchId() + 1;
                    nLCBatchId = rf.LCBatchId;
                    rf.LCBatchNo = this.getMaxLCBatchNo() + 1;
                    rf.Status = 1;
                    rf.CreatedOn = DateTime.Now;

                    this.LCBatchMapping(rf, row);
                    sealStamp(row, userId, Stamp.INSERT);
                    dataSet.LCBatch.AddLCBatchRow(row);
                    recordsAffected = ad.Update(dataSet);

                    if (recordsAffected < 1)
                        throw new DataAccessException("Update LC Batch ERROR");
                    else
                    {
                        ad.SelectCommand.Parameters["@LCBatchId"].Value = nLCBatchId;
                        if (ad.Fill(dataSet) == 1)
                        {
                            if (dataSet.LCBatch[0].CreatedBy == userId)
                            {
                                dataSet.LCBatch[0].Status = 1;
                                recordsAffected = ad.Update(dataSet);
                            }
                        }
                        else
                            throw new DataAccessException("Update LC Batch ERROR");
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


        public void updateShipmentLCInfo(LCShipmentRef lcInfo, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                IDataSetAdapter ad = getDataSetAdapter("LCShipmentApt", "getInvoiceByShipmentId");
                ad.SelectCommand.Parameters["@LCBatchId"].Value = lcInfo.LCBatch.LCBatchId;
                ad.SelectCommand.Parameters["@ShipmentId"].Value = lcInfo.ShipmentId;
                ad.SelectCommand.Parameters["@SplitShipmentId"].Value = lcInfo.SplitShipmentId;
                ad.PopulateCommands();

                LCApplicationShipmentDs dataSet = new LCApplicationShipmentDs();
                LCApplicationShipmentDs.LCApplicationShipmentRow row = null;

                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    row = dataSet.LCApplicationShipment[0];
                    this.LCApplicationShipmentMapping(lcInfo, row);
                }
                else
                {
                    row = dataSet.LCApplicationShipment.NewLCApplicationShipmentRow();
                    this.LCApplicationShipmentMapping(lcInfo, row);
                    dataSet.LCApplicationShipment.AddLCApplicationShipmentRow(row);
                }
                recordsAffected = ad.Update(dataSet);
                if (recordsAffected < 1)
                    throw new DataAccessException("Update LC Application Shipment L/C Information ERROR");
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


        public string exportArrayToCSV(ArrayList tableOfValue, string fileNamePrefix, int userId)
        {
            ArrayList columns;
            string textLine;
            string columnValue;
            int i, j;

            string fileFolder = WebConfig.getValue("appSettings", "LC_APPLICATION");
            string fileName = fileFolder + fileNamePrefix + "_" + userId.ToString() + "_" + DateTime.Now.ToString("yyMMddHHmmss") + ".csv";
            if (File.Exists(fileName))
                File.Delete(fileName);
            StreamWriter s = File.CreateText(fileName);
            textLine = String.Empty;
            for (i = 0; i < tableOfValue.Count; i++)
            {
                columns = (ArrayList)tableOfValue[i];
                for (j = 0; j < columns.Count; j++)
                {
                    textLine = (j == 0 ? String.Empty : textLine + ",");
                    if (columns[j].GetType() == "".GetType())
                    {
                        columnValue = (string)columns[j];
                        if (columnValue.LastIndexOf(",") > 0)
                            columnValue = "\"" + columnValue + "\"";
                        textLine = textLine + columnValue;
                    }
                    else if (columns[j].GetType() == DateTime.Now.GetType())
                        textLine = textLine + DateTimeUtility.getDateString((DateTime)columns[j]);
                    else if (columns[j].GetType() == ((Decimal)0).GetType())
                        textLine = textLine + "\"" + ((decimal)columns[j]).ToString() + "\"";
                    else
                        textLine = textLine + columns[j].ToString();
                }
                s.WriteLine(textLine);
            }
            s.Close();
            return fileName;
        }


        public void updateInvoiceLCInfo(LCShipmentRef lcInfo, ArrayList amendmentList, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                InvoiceDs dataSet = new InvoiceDs();

                IDataSetAdapter ad = getDataSetAdapter("InvoiceApt", "GetInvoiceByKey");
                ad.SelectCommand.Parameters["@ShipmentId"].Value = lcInfo.ShipmentId;
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    InvoiceDs.InvoiceRow row = dataSet.Invoice[0];
                    //string userName = " (By : " + generalWorker.getUserByKey(userId).DisplayName + ")";
                    string fromVal, toVal;
                    fromVal = (row.IsLCNoNull() ? "" : row.LCNo.Trim());
                    toVal = (lcInfo.LCNo == null ? "" : lcInfo.LCNo.Trim());
                    if (amendmentList != null && fromVal != toVal)
                        amendmentList.Add(new ActionHistoryDef(row.ShipmentId, 0, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C No. : " + fromVal + " -> " + toVal, userId));
                    fromVal = (row.IsLCIssueDateNull() ? "" : DateTimeUtility.getDateString(row.LCIssueDate));
                    toVal = (lcInfo.LCIssueDate == null ? "" : DateTimeUtility.getDateString(lcInfo.LCIssueDate));
                    if (amendmentList != null && fromVal != toVal)
                        amendmentList.Add(new ActionHistoryDef(row.ShipmentId, 0, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C Issue Date : " + fromVal + " -> " + toVal, userId));
                    fromVal = (row.IsLCExpiryDateNull() ? "" : DateTimeUtility.getDateString(row.LCExpiryDate));
                    toVal = (lcInfo.LCExpiryDate == null ? "" : DateTimeUtility.getDateString(lcInfo.LCExpiryDate));
                    if (amendmentList != null && fromVal != toVal)
                        amendmentList.Add(new ActionHistoryDef(row.ShipmentId, 0, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C Expiry Date : " + fromVal + " -> " + toVal, userId));
                    fromVal = row.LCAmt.ToString();
                    toVal = lcInfo.LCAmt.ToString();
                    if (amendmentList != null && fromVal != toVal)
                        amendmentList.Add(new ActionHistoryDef(row.ShipmentId, 0, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C Amount : " + fromVal + " -> " + toVal, userId));

                    this.InvoiceLCInfoMapping(lcInfo, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                    recordsAffected = ad.Update(dataSet);
                }
                else
                    throw new DataAccessException("Update Invoice L/C Information ERROR");

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


        public void updateSplitShipmentLCInfo(LCShipmentRef lcInfo, ArrayList amendmentList, int userId)
        {
            TransactionContext ctx = TransactionContextFactory.GetContext(TransactionAffinity.Required);
            try
            {
                ctx.Enter();
                TransactionContext currentCtx = TransactionContextFactory.GetCurrentContext();

                SplitShipmentDs dataSet = new SplitShipmentDs();

                IDataSetAdapter ad = getDataSetAdapter("SplitShipmentApt", "GetSplitShipmentByKey");
                ad.SelectCommand.Parameters["@SplitShipmentId"].Value = lcInfo.SplitShipmentId;
                ad.PopulateCommands();
                int recordsAffected = ad.Fill(dataSet);
                if (recordsAffected > 0)
                {
                    SplitShipmentDs.SplitShipmentRow row = dataSet.SplitShipment[0];
                    //string fromVal = (row.IsLCNoNull() ? "" : row.LCNo.Trim());
                    //string toVal = (lcInfo.LCNo == null ? "" : lcInfo.LCNo.Trim());
                    //if (amendmentList != null && fromVal != toVal)
                    //    amendmentList.Add(new ActionHistoryDef(row.ShipmentId, row.SplitShipmentId, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C No. (" + row.SplitSuffix + ") : " + fromVal + " -> " + toVal + " (By : " + generalWorker.getUserByKey(userId).DisplayName + ")", userId));

                    string userName = generalWorker.getUserByKey(userId).DisplayName;
                    string fromVal, toVal;
                    fromVal = (row.IsLCNoNull() ? "" : row.LCNo.Trim());
                    toVal = (lcInfo.LCNo == null ? "" : lcInfo.LCNo.Trim());
                    if (amendmentList != null && fromVal != toVal)
                        amendmentList.Add(new ActionHistoryDef(row.ShipmentId, row.SplitShipmentId, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C No. : " + fromVal + " -> " + toVal + " (By : " + userName + ")", userId));
                    fromVal = (row.IsLCIssueDateNull() ? "" : DateTimeUtility.getDateString(row.LCIssueDate));
                    toVal = (lcInfo.LCIssueDate == null ? "" : DateTimeUtility.getDateString(lcInfo.LCIssueDate));
                    if (amendmentList != null && fromVal != toVal)
                        amendmentList.Add(new ActionHistoryDef(row.ShipmentId, row.SplitShipmentId, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C Issue Date : " + fromVal + " -> " + toVal + " (By : " + userName + ")", userId));
                    fromVal = (row.IsLCExpiryDateNull() ? "" : DateTimeUtility.getDateString(row.LCExpiryDate));
                    toVal = (lcInfo.LCExpiryDate == null ? "" : DateTimeUtility.getDateString(lcInfo.LCExpiryDate));
                    if (amendmentList != null && fromVal != toVal)
                        amendmentList.Add(new ActionHistoryDef(row.ShipmentId, row.SplitShipmentId, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C Expiry Date : " + fromVal + " -> " + toVal + " (By : " + userName + ")", userId));
                    fromVal = row.LCAmt.ToString();
                    toVal = lcInfo.LCAmt.ToString();
                    if (amendmentList != null && fromVal != toVal)
                        amendmentList.Add(new ActionHistoryDef(row.ShipmentId, row.SplitShipmentId, ActionHistoryType.LC_APPLICATION, AmendmentType.LC_Number, "L/C Amount : " + fromVal + " -> " + toVal + " (By : " + userName + ")", userId));


                    this.SplitShipmentLCInfoMapping(lcInfo, row);
                    sealStamp(row, userId, Stamp.UPDATE);
                    recordsAffected = ad.Update(dataSet);
                }
                else
                    throw new DataAccessException("Update Invoice L/C Information ERROR");
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


        #region mapping functions

        internal void LCApplicationShipmentMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(LCApplicationShipmentDs.LCApplicationShipmentRow) &&
                target.GetType() == typeof(LCApplicationDef))
            {
                LCApplicationShipmentDs.LCApplicationShipmentRow row = (LCApplicationShipmentDs.LCApplicationShipmentRow)source;
                LCApplicationDef def = (LCApplicationDef)target;

                LCApplicationRef rf = this.getLCApplicationByKey(row.LCApplicationId);
                def.LCApplicationId = row.LCApplicationId;
                def.LCApplicationNo = rf.LCApplicationNo;
                def.ShipmentId = row.ShipmentId;
                def.SplitShipmentId = row.SplitShipmentId;
                if (!row.IsLCBatchIdNull())
                    def.LCBatchId = row.LCBatchId;
                else
                    def.LCBatchId = 0;
                def.Customer = commonWorker.getCustomerByKey(row.CustomerId);
                def.CustomerDestination = commonWorker.getCustomerDestinationByKey(row.CustomerDestinationId);
                def.Product = productWorker.getProductByKey(row.ProductId);
                def.TotalPOAmount = row.TotalPOAmt;
                def.TotalPOQuantity = row.TotalPOQty;
                def.QACommissionPercent = row.QACommissionPercent;
                def.VendorPaymentDiscountPercent = row.VendorPaymentDiscountPercent;
                def.LabTestIncome = row.LabTestIncome;
                def.ShipmentMethod = commonWorker.getShipmentMethodByKey(row.ShipmentMethodId);
                def.CustomerAtWarehouseDate = row.CustomerAtWarehouseDate;
                def.SupplierAtWarehouseDate = row.SupplierAtWarehouseDate;
                def.WorkflowStatus = LCWFS.getType(row.WorkflowStatusId);
                if (!row.IsLCApproverNull())
                    def.LCApprover = generalWorker.getUserByKey(row.LCApprover);
                if (!row.IsLCApprovalDateNull())
                    def.LCApprovalDate = row.LCApprovalDate;
                else
                    def.LCApprovalDate = DateTime.MinValue;

                BankBranchRef bankBranch = null;
                if (def.BankBranch == null)
                    def.BankBranch = new BankBranchRef();
                if (row.BankBranchId != null)
                    bankBranch = commonWorker.getBankBranchByKey(row.BankBranchId);
                if (bankBranch == null)
                {
                    def.BankBranch.BankId = -1;
                    def.BankBranch.BankBranchId = (row.BankBranchId < 0 ? -1 : row.BankBranchId);
                    def.BankBranch.Address1 = "N/A";
                    def.BankBranch.Address2 = def.BankBranch.Address3 = def.BankBranch.Address4 = "";
                    def.BankBranch.BranchName = def.BankBranch.ContactPerson = "N/A";

                    def.BankBranch.Country = new CountryRef();
                    def.BankBranch.Country.CountryId = -1;
                    def.BankBranch.Country.Name = "N/A";
                }
                else
                {
                    def.BankBranch.BankId = bankBranch.BankId;
                    def.BankBranch.BankBranchId = bankBranch.BankBranchId;
                    def.BankBranch.Address1 = bankBranch.Address1;
                    def.BankBranch.Address2 = bankBranch.Address2;
                    def.BankBranch.Address3 = bankBranch.Address3;
                    def.BankBranch.Address4 = bankBranch.Address4;
                    def.BankBranch.BranchName = bankBranch.BranchName;
                    def.BankBranch.ContactPerson = bankBranch.ContactPerson;

                    def.BankBranch.Country = new CountryRef();
                    def.BankBranch.Country.CountryId = bankBranch.Country.CountryId;
                    def.BankBranch.Country.Name = bankBranch.Country.Name;
                }

                BankRef bank = null;
                if (def.AdvisingBank == null)
                    def.AdvisingBank = new BankRef();
                if (def.BankBranch.BankId != -1)
                    bank = CommonWorker.Instance.getBankByKey(def.BankBranch.BankId);
                if (bank != null)
                {
                    def.AdvisingBank.BankId = bank.BankId;
                    def.AdvisingBank.BankName = bank.BankName;
                }
                else
                {
                    def.AdvisingBank.BankId = -1;
                    def.AdvisingBank.BankName = "N/A";
                }

                def.Status = row.Status;
                def.PackingMethod = commonWorker.getPackingMethodByKey(row.PackingMethodId);
                def.PackingUnit = commonWorker.getPackingUnitByKey(row.PackingUnitId);
                def.CountryOfOrigin = generalWorker.getCountryOfOriginByKey(row.CountryOfOriginId);
                def.ShipmentPort = commonWorker.getShipmentPortByKey(row.ShipmentPortId);
                def.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
                def.PaymentTerm = commonWorker.getPaymentTermByKey(row.PaymentTermId);
                def.TermOfPurchase = commonWorker.getTermOfPurchaseByKey(row.TermOfPurchaseId);
                def.PurchaseLocation = commonWorker.getPurchaseLocationByKey(row.PurchaseLocationId);
                def.Vendor = VendorWorker.Instance.getVendorByKey(row.VendorId);
                if (!row.IsLCCancellationDateNull())
                    def.LCCancellationDate = row.LCCancellationDate;
                else
                    def.LCCancellationDate = DateTime.MinValue;
                if (!row.IsDeducedFabricCostNull())
                    def.DeducedFabricCost = row.DeducedFabricCost;
                else
                    def.DeducedFabricCost = decimal.MinValue;
            }
            else if (source.GetType() == typeof(LCApplicationDef) &&
                target.GetType() == typeof(LCApplicationShipmentDs.LCApplicationShipmentRow))
            {
                LCApplicationDef def = (LCApplicationDef)source;
                LCApplicationShipmentDs.LCApplicationShipmentRow row = (LCApplicationShipmentDs.LCApplicationShipmentRow)target;

                row.LCApplicationId = def.LCApplicationId;
                row.ShipmentId = def.ShipmentId;
                row.SplitShipmentId = def.SplitShipmentId;
                if (def.LCBatchId > 0)
                    row.LCBatchId = def.LCBatchId;
                else
                    row.SetLCBatchIdNull();
                row.CustomerId = def.Customer.CustomerId;
                row.CustomerDestinationId = def.CustomerDestination.CustomerDestinationId;
                row.ProductId = def.Product.ProductId;
                row.TotalPOAmt = def.TotalPOAmount;
                row.TotalPOQty = def.TotalPOQuantity;
                row.QACommissionPercent = def.QACommissionPercent;
                row.VendorPaymentDiscountPercent = def.VendorPaymentDiscountPercent;
                row.LabTestIncome = def.LabTestIncome;
                row.ShipmentMethodId = def.ShipmentMethod.ShipmentMethodId;
                row.CustomerAtWarehouseDate = def.CustomerAtWarehouseDate;
                row.SupplierAtWarehouseDate = def.SupplierAtWarehouseDate;

                row.WorkflowStatusId = def.WorkflowStatus.Id;

                if (def.LCApprover != null)
                    row.LCApprover = def.LCApprover.UserId;
                else
                    row.SetLCApproverNull();

                if (def.LCApprovalDate != DateTime.MinValue)
                    row.LCApprovalDate = def.LCApprovalDate;
                else
                    row.SetLCApprovalDateNull();

                if (def.BankBranch == null)
                    row.BankBranchId = -1;
                else
                    row.BankBranchId = (def.BankBranch.BankBranchId < 0 ? -1 : def.BankBranch.BankBranchId);

                row.Status = def.Status;

                row.PackingMethodId = def.PackingMethod.PackingMethodId;
                row.PackingUnitId = def.PackingUnit.PackingUnitId;
                row.CountryOfOriginId = def.CountryOfOrigin.CountryOfOriginId;
                row.ShipmentPortId = def.ShipmentPort.ShipmentPortId;
                row.CurrencyId = def.Currency.CurrencyId;
                row.PaymentTermId = def.PaymentTerm.PaymentTermId;
                row.TermOfPurchaseId = def.TermOfPurchase.TermOfPurchaseId;
                row.PurchaseLocationId = def.PurchaseLocation.PurchaseLocationId;
                row.VendorId = def.Vendor.VendorId;
                if (def.LCCancellationDate != DateTime.MinValue)
                    row.LCCancellationDate = def.LCCancellationDate;
                else
                    row.SetLCCancellationDateNull();
                if (def.DeducedFabricCost == decimal.MinValue)
                    row.SetDeducedFabricCostNull();
                else
                    row.DeducedFabricCost = def.DeducedFabricCost;
            }
        }


        internal void LCApplicationMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(LCApplicationDs.LCApplicationRow) &&
                target.GetType() == typeof(LCApplicationRef))
            {
                LCApplicationDs.LCApplicationRow row = (LCApplicationDs.LCApplicationRow)source;
                LCApplicationRef rf = (LCApplicationRef)target;
                rf.LCApplicationId = row.LCApplicationId;
                rf.LCApplicationNo = row.LCApplicationNo;
                rf.CreateDate = row.CreatedOn;
                rf.CreateUser = generalWorker.getUserByKey(row.CreatedBy);
                rf.Status = row.Status;
            }
            else if (source.GetType() == typeof(LCApplicationRef) &&
                target.GetType() == typeof(LCApplicationDs.LCApplicationRow))
            {
                LCApplicationRef rf = (LCApplicationRef)source;
                LCApplicationDs.LCApplicationRow row = (LCApplicationDs.LCApplicationRow)target;

                row.LCApplicationId = rf.LCApplicationId;
                row.LCApplicationNo = rf.LCApplicationNo;
                row.CreatedOn = rf.CreateDate;
                row.CreatedBy = rf.CreateUser.UserId;
                row.Status = rf.Status;
            }
        }


        internal void LCShipmentMapping(Object source, Object target)
        {

            if (source.GetType() == typeof(LCShipmentDs.LCShipmentRow) &&
                target.GetType() == typeof(LCShipmentRef))
            {
                LCApplicationDef lcApplication;

                LCShipmentDs.LCShipmentRow row = (LCShipmentDs.LCShipmentRow)source;
                LCShipmentRef rf = (LCShipmentRef)target;
                rf.ShipmentId = row.ShipmentId;
                rf.SplitShipmentId = row.SplitShipmentId;
                rf.SplitSuffix = row.SplitSuffix;
                rf.ContractId = row.ContractId;
                rf.ContractNo = row.ContractNo;
                rf.DeliveryNo = row.DeliveryNo;
                rf.Vendor = VendorWorker.Instance.getVendorByKey(row.VendorId);
                rf.PaymentTerm = commonWorker.getPaymentTermByKey(row.PaymentTermId);
                rf.CustomerAtWarehouseDate = row.CustomerAtWarehouseDate;
                rf.SupplierAtWarehouseDate = row.SupplierAtWarehouseDate;
                rf.PackingMethod = commonWorker.getPackingMethodByKey(row.PackingMethodId);
                rf.PackingUnit = commonWorker.getPackingUnitByKey(row.PackingUnitId);
                rf.CountryOfOrigin = generalWorker.getCountryOfOriginByKey(row.CountryOfOriginId);
                rf.ShipmentMethod = commonWorker.getShipmentMethodByKey(row.ShipmentMethodId);
                rf.ShipmentPort = commonWorker.getShipmentPortByKey(row.ShipmentPortId);
                rf.TermOfPurchase = commonWorker.getTermOfPurchaseByKey(row.TermOfPurchaseId);
                rf.PurchaseLocation = commonWorker.getPurchaseLocationByKey(row.PurchaseLocationId);
                rf.CustomerDestination = commonWorker.getCustomerDestinationByKey(row.CustomerDestinationId);
                rf.TotalPOQuantity = row.TotalPOQty;
                rf.ShipmentTotalPOAmount = row.ShipmentTotalPOAmount;
                rf.TotalPOAmt = (row.IsTotalPOAmtNull() ? int.MinValue : row.TotalPOAmt);
                rf.Customer = commonWorker.getCustomerByKey(row.CustomerId);
                rf.Product = productWorker.getProductByKey(row.ProductId);
                rf.QACommissionPercent = row.QACommissionPercent;
                rf.VendorPaymentDiscountPercent = row.VendorPaymentDiscountPercent;
                rf.LabTestIncome = row.LabTestIncome;
                if (row.IsBankBranchIdNull())
                {
                    rf.BankBranch = null;
                    //rf.BankBranch = new BankBranchRef();
                    rf.AdvisingBank = null;
                }
                else
                {
                    rf.BankBranch = commonWorker.getBankBranchByKey(row.BankBranchId);
                    if (rf.BankBranch == null)
                        rf.AdvisingBank = null;
                    else
                        rf.AdvisingBank = commonWorker.getBankByKey(rf.BankBranch.BankId);
                }
                rf.ShipmentWorkflowStatus = ContractWFS.getType(row.ShipmentWorkflowStatusId);
                if (row.WorkflowStatusId > 0) rf.WorkflowStatus = LCWFS.getType(row.WorkflowStatusId);
                rf.LCApplication = getLCApplicationByKey(row.LCApplicationId);
                if (rf.LCApplication == null) rf.LCApplication = new LCApplicationRef();
                rf.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
                rf.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
                rf.LCBatch = getLCBatchByKey(row.LCBatchId);
                if (rf.LCBatch == null) rf.LCBatch = new LCBatchRef();

                if (!row.IsLCNoNull())
                    rf.LCNo = row.LCNo;
                else
                    rf.LCNo = String.Empty;

                if (!row.IsLCIssueDateNull())
                    rf.LCIssueDate = row.LCIssueDate;
                else
                    rf.LCIssueDate = DateTime.MinValue;

                if (!row.IsLCExpiryDateNull())
                    rf.LCExpiryDate = row.LCExpiryDate;
                else
                    rf.LCExpiryDate = DateTime.MinValue;

                if (!row.IsLCAmtNull())
                    rf.LCAmt = row.LCAmt;
                else
                    rf.LCAmt = 0;

                //rf.ContractLCIssued = row.ContractLCIssued;
                rf.ContractLCIssued = (row.IsOtherShipmentIdWithLCNoNull() && row.IsOtherSplitShipmentIdWithLCNoNull() ? 0 : 1);
                rf.OtherShipmentIdWithLCNo = (row.IsOtherShipmentIdWithLCNoNull() ? 0 : row.OtherShipmentIdWithLCNo);
                rf.OtherSplitShipmentIdWithLCNo = (row.IsOtherSplitShipmentIdWithLCNoNull() ? 0 : row.OtherSplitShipmentIdWithLCNo);
                //rf.OtherDelivery = new List<LCApplicationShipmentDetailDef>();
                //ArrayList list = getLCShipmentDetailByShipmentId(rf.OtherShipmentIdWithLCNo, rf.OtherSplitShipmentIdWithLCNo);
                //foreach (LCApplicationShipmentDetailDef def in list)
                //    rf.OtherDelivery.Add(def);

                lcApplication = getLCApplicationShipmentByKey(row.LCApplicationId, row.ShipmentId, row.SplitShipmentId);
                if (lcApplication != null)
                {
                    rf.LCApprover = lcApplication.LCApprover;
                    rf.LCApprovalDate = lcApplication.LCApprovalDate;
                }
                if (!row.IsLCCancellationDateNull())
                    rf.LCCancellationDate = row.LCCancellationDate;
                else
                    rf.LCCancellationDate = DateTime.MinValue;
            }
            else if (source.GetType() == typeof(LCShipmentRef) &&
                target.GetType() == typeof(LCShipmentDs.LCShipmentRow))
            {
                LCShipmentRef rf = (LCShipmentRef)source;
                LCShipmentDs.LCShipmentRow row = (LCShipmentDs.LCShipmentRow)target;

                row.ShipmentId = rf.ShipmentId;
                row.SplitShipmentId = rf.SplitShipmentId;
                row.SplitSuffix = rf.SplitSuffix;
                row.ContractId = rf.ContractId;
                row.ContractNo = rf.ContractNo;
                row.DeliveryNo = rf.DeliveryNo;
                row.VendorId = rf.Vendor.VendorId;
                row.PaymentTermId = rf.PaymentTerm.PaymentTermId;
                row.ItemNo = rf.Product.ItemNo;
                row.CustomerAtWarehouseDate = rf.CustomerAtWarehouseDate;
                row.SupplierAtWarehouseDate = rf.SupplierAtWarehouseDate;
                row.PackingMethodId = rf.PackingMethod.PackingMethodId;
                row.PackingUnitId = rf.PackingUnit.PackingUnitId;
                row.CountryOfOriginId = rf.CountryOfOrigin.CountryOfOriginId;
                row.ShipmentMethodId = rf.ShipmentMethod.ShipmentMethodId;
                row.ShipmentPortId = rf.ShipmentPort.ShipmentPortId;
                row.TermOfPurchaseId = rf.TermOfPurchase.TermOfPurchaseId;
                row.PurchaseLocationId = rf.PurchaseLocation.PurchaseLocationId;
                row.CustomerDestinationId = rf.CustomerDestination.CustomerDestinationId;
                row.TotalPOQty = rf.TotalPOQuantity;
                row.ShipmentTotalPOAmount = rf.ShipmentTotalPOAmount;
                if (rf.TotalPOAmt == decimal.MinValue)
                    row.TotalPOAmt = 0;
                else
                    row.TotalPOAmt = rf.TotalPOAmt;
                row.CustomerId = rf.Customer.CustomerId;
                row.ProductId = rf.Product.ProductId;
                row.QACommissionPercent = rf.QACommissionPercent;
                row.VendorPaymentDiscountPercent = rf.VendorPaymentDiscountPercent;
                row.LabTestIncome = rf.LabTestIncome;
                if (rf.BankBranch == null)
                    row.SetBankBranchIdNull();
                else
                    row.BankBranchId = rf.BankBranch.BankBranchId;
                row.ShipmentWorkflowStatusId = rf.ShipmentWorkflowStatus.Id;
                row.WorkflowStatusId = rf.WorkflowStatus.Id;
                row.LCApplicationId = rf.LCApplication.LCApplicationId;
                row.OfficeId = rf.Office.OfficeId;
                row.CurrencyId = rf.Currency.CurrencyId;
                row.LCBatchId = rf.LCBatch.LCBatchId;
                if (rf.LCNo != String.Empty)
                    row.LCNo = rf.LCNo;
                else
                    row.SetLCNoNull();
                if (rf.LCIssueDate != DateTime.MinValue)
                    row.LCIssueDate = rf.LCIssueDate;
                else
                    row.SetLCIssueDateNull();
                if (rf.LCExpiryDate != DateTime.MinValue)
                    row.LCExpiryDate = rf.LCExpiryDate;
                else
                    row.SetLCExpiryDateNull();
                if (rf.LCAmt == 0)
                    row.SetLCAmtNull();
                else
                    row.LCAmt = rf.LCAmt;
                //row.ContractLCIssued = rf.ContractLCIssued;
                if (rf.LCCancellationDate != DateTime.MinValue)
                    row.LCCancellationDate = rf.LCCancellationDate;
                else
                    row.SetLCCancellationDateNull();
            }
        }


        internal void LCApplicationShipmentDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(LCApplicationShipmentDetailDs.LCApplicationShipmentDetailRow) &&
                target.GetType() == typeof(LCApplicationShipmentDetailDef))
            {
                LCApplicationShipmentDetailDs.LCApplicationShipmentDetailRow row = (LCApplicationShipmentDetailDs.LCApplicationShipmentDetailRow)source;
                LCApplicationShipmentDetailDef def = (LCApplicationShipmentDetailDef)target;

                def.LCApplicationId = row.LCApplicationId;
                def.ShipmentId = row.ShipmentId;
                def.SplitShipmentId = row.SplitShipmentId;
                def.ShipmentDetailId = row.ShipmentDetailId;
                def.SizeOptionId = row.SizeOptionId;
                if (row.IsColourNull())
                    def.Colour = string.Empty;
                else
                    def.Colour = row.Colour;
                def.POQty = row.POQty;
                def.ReducedSupplierGmtPrice = row.ReducedSupplierGmtPrice;
                def.Status = row.Status;
            }
            else
                if (source.GetType() == typeof(LCApplicationShipmentDetailDef) &&
                    target.GetType() == typeof(LCApplicationShipmentDetailDs.LCApplicationShipmentDetailRow))
            {
                LCApplicationShipmentDetailDef def = (LCApplicationShipmentDetailDef)source;
                LCApplicationShipmentDetailDs.LCApplicationShipmentDetailRow row = (LCApplicationShipmentDetailDs.LCApplicationShipmentDetailRow)target;
                row.LCApplicationId = def.LCApplicationId;
                row.ShipmentId = def.ShipmentId;
                row.SplitShipmentId = def.SplitShipmentId;
                row.ShipmentDetailId = def.ShipmentDetailId;
                row.SizeOptionId = def.SizeOptionId;
                if (def.Colour == string.Empty)
                    row.Colour = null;
                else
                    row.Colour = def.Colour;
                row.POQty = def.POQty;
                row.ReducedSupplierGmtPrice = def.ReducedSupplierGmtPrice;
                row.Status = def.Status;
            }

        }


        internal void LCBatchMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(LCBatchDs.LCBatchRow) &&
                target.GetType() == typeof(LCBatchRef))
            {
                LCBatchDs.LCBatchRow row = (LCBatchDs.LCBatchRow)source;
                LCBatchRef rf = (LCBatchRef)target;

                rf.LCBatchId = row.LCBatchId;
                rf.LCBatchNo = row.LCBatchNo;
                rf.GroupId = row.GroupId;
                rf.IssuingBankId = row.IssuingBankId;
                rf.CreatedBy = row.CreatedBy;
                rf.CreatedOn = row.CreatedOn;
                rf.Status = row.Status;
            }
            else if (source.GetType() == typeof(LCBatchRef) &&
                target.GetType() == typeof(LCBatchDs.LCBatchRow))
            {
                LCBatchRef rf = (LCBatchRef)source;
                LCBatchDs.LCBatchRow row = (LCBatchDs.LCBatchRow)target;

                row.LCBatchId = rf.LCBatchId;
                row.LCBatchNo = rf.LCBatchNo;
                row.GroupId = rf.GroupId;
                row.IssuingBankId = rf.IssuingBankId;
                row.CreatedBy = rf.CreatedBy;
                row.CreatedOn = rf.CreatedOn;
                row.Status = rf.Status;
            }
        }


        internal void LCBatchSummaryMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(LCBatchSummaryDs.LCBatchSummaryRow) &&
                target.GetType() == typeof(LCBatchSummaryRef))
            {
                LCBatchSummaryDs.LCBatchSummaryRow row = (LCBatchSummaryDs.LCBatchSummaryRow)source;
                LCBatchSummaryRef rf = (LCBatchSummaryRef)target;

                rf.LCBatch = new LCBatchRef();
                if (row.LCBatchId > 0)
                {
                    rf.LCBatch.LCBatchId = row.LCBatchId;
                    rf.LCBatch.LCBatchNo = row.LCBatchNo;
                    rf.LCBatch.GroupId = row.GroupId;
                    rf.LCBatch.IssuingBankId = row.IssuingBankId;
                    rf.LCBatch.CreatedOn = row.CreatedOn;
                    rf.LCBatch.CreatedBy = row.CreatedBy;
                }
                rf.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
                rf.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
                rf.Vendor = VendorWorker.Instance.getVendorByKey(row.VendorId);
                rf.WorkflowStatus = LCWFS.getType(row.WorkflowStatusId);
                rf.NoOfShipment = row.NoOfShipment;
                rf.TotalPOAmount = row.TotalPOAmount;
            }
            else if (source.GetType() == typeof(LCBatchSummaryRef) &&
                target.GetType() == typeof(LCBatchSummaryDs.LCBatchSummaryRow))
            {
                LCBatchSummaryRef rf = (LCBatchSummaryRef)source;
                LCBatchSummaryDs.LCBatchSummaryRow row = (LCBatchSummaryDs.LCBatchSummaryRow)target;

                row.LCBatchId = rf.LCBatch.LCBatchId;
                row.LCBatchNo = rf.LCBatch.LCBatchNo;
                row.GroupId = rf.LCBatch.GroupId;
                row.IssuingBankId = rf.LCBatch.IssuingBankId;
                row.CreatedBy = rf.LCBatch.CreatedBy;
                row.CreatedOn = rf.LCBatch.CreatedOn;
                row.CurrencyId = rf.Currency.CurrencyId;
                row.OfficeId = rf.Office.OfficeId;
                row.VendorId = rf.Vendor.VendorId;
                row.WorkflowStatusId = rf.WorkflowStatus.Id;
                row.NoOfShipment = rf.NoOfShipment;
                row.TotalPOAmount = rf.TotalPOAmount;
            }
        }


        internal void LCBatchDetailMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(LCBatchDetailDs.LCBatchDetailRow) &&
                target.GetType() == typeof(LCBatchDetailRef))
            {
                LCBatchDetailDs.LCBatchDetailRow row = (LCBatchDetailDs.LCBatchDetailRow)source;
                LCBatchDetailRef rf = (LCBatchDetailRef)target;

                //rf.LCBatchId = row.LCBatchId;
                //rf.LCBatchNo = row.LCBatchNo;
                rf.LCApplicationId = row.LCApplicationId;
                rf.ShipmentId = row.ShipmentId;
                rf.Customer = commonWorker.getCustomerByKey(row.CustomerId);
                rf.CustomerDestination = commonWorker.getCustomerDestinationByKey(row.CustomerDestinationId);
                rf.ShipmentMethod = commonWorker.getShipmentMethodByKey(row.ShipmentMethodId);
                rf.QACommissionPercent = row.QACommissionPercent;
                rf.VendorPaymentDiscountPercent = row.VendorPaymentDiscountPercent;
                rf.LabTestIncome = row.LabTestIncome;

                rf.Product = productWorker.getProductByKey(row.ProductId);
                //rf.Product = new ProductDef();
                //rf.Product.ProductId = row.ProductId;
                //rf.Product.ItemNo = row.ItemNo;

                rf.PackingUnit = commonWorker.getPackingUnitByKey(row.PackingUnitId);
                rf.POQty = row.POQty;
                rf.ReducedSupplierGmtPrice = row.ReducedSupplierGmtPrice;
                rf.SupplierAtWarehouseDate = row.SupplierAtWarehouseDate;
                rf.Vendor = VendorWorker.Instance.getVendorByKey(row.VendorId);
                rf.Currency = generalWorker.getCurrencyByKey(row.CurrencyId);
                rf.WorkflowStatusId = row.WorkflowStatusId;

                rf.BankBranch = commonWorker.getBankBranchByKey(row.BankBranchId);
                if (rf.BankBranch == null)
                {
                    rf.BankBranch = new BankBranchRef();
                    rf.BankBranch.BankId = -1;
                    rf.BankBranch.BankBranchId = (row.BankBranchId < 0 ? -1 : row.BankBranchId);
                    rf.BankBranch.Address1 = "N/A";
                    rf.BankBranch.Address2 = rf.BankBranch.Address3 = rf.BankBranch.Address4 = "";
                    rf.BankBranch.BranchName = rf.BankBranch.ContactPerson = "N/A";

                    rf.BankBranch.Country = new CountryRef();
                    rf.BankBranch.Country.CountryId = -1;
                    rf.BankBranch.Country.Name = "N/A";
                }

                rf.AdvisingBank = CommonWorker.Instance.getBankByKey(rf.BankBranch.BankId);
                if (rf.AdvisingBank == null)
                {
                    rf.AdvisingBank = new BankRef();
                    rf.AdvisingBank.BankId = -1;
                    rf.AdvisingBank.BankName = "N/A";
                }
                rf.TermOfPurchase = commonWorker.getTermOfPurchaseByKey(row.TermOfPurchaseId);
                rf.PurchaseLocation = commonWorker.getPurchaseLocationByKey(row.PurchaseLocationId);
                rf.LCBatch = getLCBatchByKey(row.LCBatchId);
                rf.PaymentTerm = commonWorker.getPaymentTermByKey(row.PaymentTermId);
                if (row.IsProductTeamIdNull())
                {
                    rf.ProductTeam = new ProductCodeDef();
                    rf.ProductTeam.ProductCodeId = -1;
                }
                else
                {
                    if (row.ProductTeamId > 0)
                        rf.ProductTeam = generalWorker.getProductCodeDefByKey(row.ProductTeamId);
                    else
                    {
                        rf.ProductTeam = new ProductCodeDef();
                        rf.ProductTeam.ProductCodeId = -1;
                    }
                }
                rf.ExpectedDeductAmt = row.IsExpectedDeductAmtNull() ? 0 : row.ExpectedDeductAmt;
                rf.ActualDeductAmt = row.IsActualDeductAmtNull() ? 0 : row.ActualDeductAmt;
            }
            else if (source.GetType() == typeof(LCBatchDetailRef) &&
                target.GetType() == typeof(LCBatchDetailDs.LCBatchDetailRow))
            {
                LCBatchDetailRef rf = (LCBatchDetailRef)source;
                LCBatchDetailDs.LCBatchDetailRow row = (LCBatchDetailDs.LCBatchDetailRow)target;

                //rf.LCBatchId = row.LCBatchId;
                //rf.LCBatchNo = row.LCBatchNo;
                rf.LCApplicationId = row.LCApplicationId;
                rf.ShipmentId = row.ShipmentId;
                rf.Customer = commonWorker.getCustomerByKey(row.CustomerId);
                rf.CustomerDestination = commonWorker.getCustomerDestinationByKey(row.CustomerDestinationId);
                rf.ShipmentMethod = commonWorker.getShipmentMethodByKey(row.ShipmentMethodId);

                //row.LCBatchId = rf.LCBatchId;
                row.LCApplicationId = rf.LCApplicationId;
                row.ShipmentId = rf.ShipmentId;
                row.CustomerId = rf.Customer.CustomerId;
                row.CustomerDestinationId = rf.CustomerDestination.CustomerDestinationId;
                row.ShipmentMethodId = rf.ShipmentMethod.ShipmentMethodId;
                row.QACommissionPercent = rf.QACommissionPercent;
                row.VendorPaymentDiscountPercent = rf.VendorPaymentDiscountPercent;
                row.LabTestIncome = rf.LabTestIncome;

                row.ProductId = rf.Product.ProductId;

                //row.ItemNo = rf.Product.ItemNo;

                row.PackingUnitId = rf.PackingUnit.PackingUnitId;
                row.POQty = rf.POQty;
                row.ReducedSupplierGmtPrice = rf.ReducedSupplierGmtPrice;
                row.SupplierAtWarehouseDate = rf.SupplierAtWarehouseDate;
                row.VendorId = rf.Vendor.VendorId;
                row.CurrencyId = rf.Currency.CurrencyId;
                row.WorkflowStatusId = rf.WorkflowStatusId;

                //row.AdvisingBankId = rf.AdvisingBank.BankId;
                row.BankBranchId = rf.BankBranch.BankBranchId;
                row.TermOfPurchaseId = rf.TermOfPurchase.TermOfPurchaseId;
                row.PurchaseLocationId = rf.PurchaseLocation.PurchaseLocationId;
                row.LCBatchId = rf.LCBatch.LCBatchId;
                row.PaymentTermId = rf.PaymentTerm.PaymentTermId;
                if (rf.ProductTeam.ProductCodeId == -1)
                    row.SetProductTeamIdNull();
                else
                    row.ProductTeamId = rf.ProductTeam.ProductCodeId;
            }
        }


        private void InvoiceLCInfoMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(LCShipmentRef) &&
                target.GetType() == typeof(InvoiceDs.InvoiceRow))
            {
                LCShipmentRef LCInfo = (LCShipmentRef)source;
                InvoiceDs.InvoiceRow row = (InvoiceDs.InvoiceRow)target;

                row.LCNo = LCInfo.LCNo;
                if (LCInfo.LCAmt < 0)
                    row.LCAmt = 0;
                else
                    row.LCAmt = LCInfo.LCAmt;

                if (LCInfo.LCIssueDate != DateTime.MinValue)
                    row.LCIssueDate = LCInfo.LCIssueDate;
                else
                    row.SetLCIssueDateNull();

                if (LCInfo.LCExpiryDate != DateTime.MinValue)
                    row.LCExpiryDate = LCInfo.LCExpiryDate;
                else
                    row.SetLCExpiryDateNull();
            }
        }


        private void SplitShipmentLCInfoMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(LCShipmentRef) &&
                target.GetType() == typeof(SplitShipmentDs.SplitShipmentRow))
            {
                LCShipmentRef LCInfo = (LCShipmentRef)source;
                SplitShipmentDs.SplitShipmentRow row = (SplitShipmentDs.SplitShipmentRow)target;

                row.LCNo = LCInfo.LCNo;
                if (LCInfo.LCAmt < 0)
                    row.LCAmt = 0;
                else
                    row.LCAmt = LCInfo.LCAmt;

                if (LCInfo.LCIssueDate != DateTime.MinValue)
                    row.LCIssueDate = LCInfo.LCIssueDate;
                else
                    row.SetLCIssueDateNull();

                if (LCInfo.LCExpiryDate != DateTime.MinValue)
                    row.LCExpiryDate = LCInfo.LCExpiryDate;
                else
                    row.SetLCExpiryDateNull();
            }
        }


        public void LCShipmentCopyToLCApplication(Object source, Object target)
        {
            if (target.GetType() == typeof(LCShipmentRef) && source.GetType() == typeof(LCApplicationDef))
            {
                LCApplicationDef df = (LCApplicationDef)source;
                LCShipmentRef rf = (LCShipmentRef)target;

                rf.ShipmentId = df.ShipmentId;
                rf.SplitShipmentId = df.SplitShipmentId;
                rf.CustomerAtWarehouseDate = df.CustomerAtWarehouseDate;
                rf.SupplierAtWarehouseDate = df.SupplierAtWarehouseDate;
                rf.ShipmentMethod = df.ShipmentMethod;
                //rf.ShipmentPort = commonWorker.getShipmentPortByKey(ShipmentPortId);
                rf.CustomerDestination = df.CustomerDestination;
                rf.TotalPOQuantity = df.TotalPOQuantity;
                rf.TotalPOAmt = df.TotalPOAmount;
                rf.Customer = df.Customer;
                rf.Product = df.Product;
                rf.QACommissionPercent = df.QACommissionPercent;
                rf.VendorPaymentDiscountPercent = df.VendorPaymentDiscountPercent;
                rf.LabTestIncome = df.LabTestIncome;

                rf.BankBranch = df.BankBranch;
                rf.WorkflowStatus = df.WorkflowStatus;
                rf.LCApplication.LCApplicationId = df.LCApplicationId;
                rf.LCApplication.LCApplicationNo = df.LCApplicationNo;
                //rf.LCBatch.LCBatchId = df.LCBatch.LCBatchId ;
                rf.LCBatch = getLCBatchByKey(df.LCBatchId);
                rf.LCApprover = df.LCApprover;
                rf.LCApprovalDate = df.LCApprovalDate;
                rf.Status = df.Status;

                //rf.ContractId = ContractId;
                //rf.ContractNo = ContractNo;
                //rf.DeliveryNo = DeliveryNo;
                rf.CountryOfOrigin = df.CountryOfOrigin; // generalWorker.getCountryOfOriginByKey(CountryOfOriginId);
                rf.PackingMethod = df.PackingMethod; //commonWorker.getPackingMethodByKey(PackingMethodId);
                //rf.Office = generalWorker.getOfficeRefByKey(OfficeId);
                rf.Currency = df.Currency; //generalWorker.getCurrencyByKey(CurrencyId);

                rf.ContractNo = df.ContractNo;
                rf.DeliveryNo = df.DeliveryNo;
                rf.PaymentTerm = df.PaymentTerm;
                rf.ShipmentPort = df.ShipmentPort;
                rf.PackingMethod = df.PackingMethod;
                rf.PackingUnit = df.PackingUnit;
                rf.PurchaseLocation = df.PurchaseLocation;
                rf.ShipmentPort = df.ShipmentPort;
                rf.TermOfPurchase = df.TermOfPurchase;
                rf.Vendor = df.Vendor;
                rf.AdvisingBank = df.AdvisingBank;
                rf.Office = df.Office;
                rf.LCCancellationDate = df.LCCancellationDate;
                //rf.ShipmentWorkflowStatus = 
                //rf.SplitSuffix=;
                //rf.Vendor =  VendorWorker.Instance.getVendorByKey(VendorId);
            }
            else if (source.GetType() == typeof(LCShipmentRef) && target.GetType() == typeof(LCApplicationDef))
            {
                LCApplicationDef df = (LCApplicationDef)target;
                LCShipmentRef rf = (LCShipmentRef)source;

                df.LCApplicationId = rf.LCApplication.LCApplicationId;
                df.LCApplicationNo = rf.LCApplication.LCApplicationNo;
                df.ShipmentId = rf.ShipmentId;
                df.SplitShipmentId = rf.SplitShipmentId;
                df.LCBatchId = rf.LCBatch.LCBatchId;
                df.Customer = rf.Customer;
                df.CustomerDestination = rf.CustomerDestination;
                df.Product = rf.Product;
                df.TotalPOAmount = rf.TotalPOAmt;
                df.TotalPOQuantity = rf.TotalPOQuantity;
                df.QACommissionPercent = rf.QACommissionPercent;
                df.VendorPaymentDiscountPercent = rf.VendorPaymentDiscountPercent;
                df.LabTestIncome = rf.LabTestIncome;

                df.ShipmentMethod = rf.ShipmentMethod;
                df.CustomerAtWarehouseDate = rf.CustomerAtWarehouseDate;
                df.SupplierAtWarehouseDate = rf.SupplierAtWarehouseDate;
                df.WorkflowStatus = rf.WorkflowStatus;
                df.BankBranch = rf.BankBranch;

                df.LCApprover = rf.LCApprover;
                df.LCApprovalDate = rf.LCApprovalDate;
                df.Status = rf.Status;

                df.CountryOfOrigin = rf.CountryOfOrigin; // generalWorker.getCountryOfOriginByKey(CountryOfOriginId);
                df.PackingMethod = rf.PackingMethod; //commonWorker.getPackingMethodByKey(PackingMethodId);
                //rf.Office = generalWorker.getOfficeRefByKey(OfficeId);
                df.Currency = rf.Currency; //generalWorker.getCurrencyByKey(CurrencyId);

                df.ContractNo = rf.ContractNo;
                df.DeliveryNo = rf.DeliveryNo;
                df.PaymentTerm = rf.PaymentTerm;
                df.ShipmentPort = rf.ShipmentPort;
                df.PackingMethod = rf.PackingMethod;
                df.PackingUnit = rf.PackingUnit;
                df.PurchaseLocation = rf.PurchaseLocation;
                df.ShipmentPort = rf.ShipmentPort;
                df.TermOfPurchase = rf.TermOfPurchase;
                df.Vendor = rf.Vendor;
                df.AdvisingBank = rf.AdvisingBank;
                df.Office = rf.Office;
                df.LCCancellationDate = rf.LCCancellationDate;
            }
        }

        internal void LCAdvancePaymentMapping(Object source, Object target)
        {
            if (source.GetType() == typeof(LCAdvancePaymentDs.LCAdvancePaymentRow) &&
                target.GetType() == typeof(LCAdvancePaymentRef))
            {
                LCAdvancePaymentDs.LCAdvancePaymentRow row = (LCAdvancePaymentDs.LCAdvancePaymentRow)source;
                LCAdvancePaymentRef rf = (LCAdvancePaymentRef)target;
                rf.ContractNo = row.ContractNo;
                rf.DeliveryNo = row.DeliveryNo;
                rf.Vendor = VendorWorker.Instance.getVendorByKey(row.VendorId);
                rf.Product = productWorker.getProductByKey(row.ProductId);
                rf.SupplierAtWarehouseDate = row.SupplierAtWarehouseDate;
                rf.TotalPoQty = row.TotalPoQty;
                rf.TotalShippedQty = row.TotalShippedQty;
                rf.TotalShippedAmt = row.TotalShippedAmt;
                if (!row.IsInvoiceDateNull())
                    rf.InvoiceDate = row.InvoiceDate;
                rf.ShipmentStatus = ContractWFS.getType(row.ShippmentStatusId);
                if (!row.IsApDateNull())
                    rf.ApDate = row.ApDate;
                rf.LCApplicationNo = row.LCApplicationNo;
                rf.LCBatchNo = row.LCBatchNo;
                if (!row.IsLCNoNull())
                    rf.LCNo = row.LCNo;
                if (!row.IsLCIssueDateNull())
                    rf.LCIssueDate = row.LCIssueDate;
                if (!row.IsLCExpiryDateNull())
                    rf.LCExpiryDate = row.LCExpiryDate;
                rf.PaymentNo = row.PaymentNo;
                rf.ExpectedDeductAmt = row.ExpectedDeductAmt;
                rf.ActualDeductAmt = row.ActualDeductAmt;
                if (!row.IsNSLRefNoNull())
                    rf.NSLRefNo = row.NSLRefNo;
                rf.AdvancePaymentStatus = NSSAdvancePaymentWFS.getType(row.AdvancePaymentStatusId);
                if (!row.IsApprovedDateNull())
                    rf.ApprovedDate = row.ApprovedDate;
                if (!row.IsRejectDateNull())
                    rf.RejectDate = row.RejectDate;
                if (!row.IsSubmittedDateNull())
                    rf.SubmittedDate = row.SubmittedDate;
                rf.PaymentTermId = row.PaymentTermId;
                if (!row.IsPaymentDeductionAmtInLCNull())
                    rf.PaymentDeductionAmtInLC = row.PaymentDeductionAmtInLC;
                else 
                    rf.PaymentDeductionAmtInLC = decimal.MinValue;
            }

        }

        #endregion

    }

}
