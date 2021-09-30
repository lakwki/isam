using System;
using System.Collections;
using System.Data;
using com.next.infra.persistency.dataaccess;
using com.next.common.datafactory.worker;
using com.next.common.datafactory.worker.industry;
using com.next.common.datafactory.model.general.officestruct;
using com.next.common.domain;
using com.next.isam.dataserver.model.shipping;
using com.next.isam.dataserver.model.nss;
using com.next.isam.domain.shipping;
using com.next.isam.domain.types;
using com.next.isam.domain.ils;
using com.next.isam.domain.nontrade;
using com.next.common.domain.types;

namespace com.next.isam.dataserver.worker
{
    public class AlertNotificationWorker : Worker
    {
        private static AlertNotificationWorker _instance;
        private GeneralWorker generalWorker;
        private CommonWorker commonWorker;
        private ProductWorker productWorker;
        public char Delimiter = char.Parse("|");

        protected AlertNotificationWorker()
        {
            generalWorker = GeneralWorker.Instance;
            commonWorker = CommonWorker.Instance;
            productWorker = ProductWorker.Instance;
        }

        public static AlertNotificationWorker Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AlertNotificationWorker();
                return _instance;
            }
        }

        public ArrayList getInvoiceUploadFailList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetInvoiceUploadFailList");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;

            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }

        public int getInvoiceUploadFailCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetInvoiceUploadFailCount");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }


        /*
        public ArrayList getVendorMissingAdvisingBankList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetVendorMissingAdvisingBankList");

            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;

            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }
        
        public int getVendorMissingAdvisingBankCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetVendorMissingAdvisingBankCount");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public int getVendorMissingAdvisingBankCount_Fast(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetVendorMissingAdvisingBankCount_Fast");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;

            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            return ad.Fill(dataSet);
        }
        */

        public ArrayList getOutstandingBookingList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetOutstandingBookingList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;

            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected >= 1)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }

        public int getOutstandingBookingCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetOutstandingBookingCount");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public ArrayList getOutstandingDocumentList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetOutstandingDocumentList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }

        public int getOutstandingDocumentCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetOutstandingDocumentCount");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public ArrayList getOutstandingDocumentOffshoreList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetOutstandingDocumentOffshoreList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }


        public int getOutstandingDocumentOffshoreCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetOutstandingDocumentOffshoreCount");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public ArrayList getOutstandingResubmitDocumentList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetOutstandingResubmitDocumentList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }

        public int getOutstandingResubmitDocumentCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetOutstandingResubmitDocumentCount");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public ArrayList getUTOrderOutstandingToInvoiceList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetUTOrderOutstandingToInvoiceListList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }

        public int getUTOrderOutstandingToInvoiceCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetUTOrderOutstandingToInvoiceListCount");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        /*
        public ArrayList getMissingSunAccountCodeSupplierList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetMissingSunAccountCodeSupplierList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }


        public int getMissingSunAccountCodeSupplierCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetMissingSunAccountCodeSupplierCount");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }


        public ArrayList getMissingPaymentAdviceEMailSupplierList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetMissingPaymentAdviceEMailSupplierList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }


        public int getMissingPaymentAdviceEMailSupplierCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetMissingPaymentAdviceEMailSupplierCount");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }
        */

        public AlertTable getGenericVendorAlertDetailList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            AlertTable table = new AlertTable();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetVendorAlertDetailList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                table.AddColumn("ShipmentId");
                table.AddColumn("VendorId");
                table.AddColumn("OfficeId");
                table.AddColumn("PurchaseTermId");
                table.AddColumn("ProductId");
                table.AddColumn("MerchandiserId");
                table.AddColumn("WorkFlowStatusId");
                table.AddColumn("Remark");
                table.AddColumn("Contract No");
                table.AddColumn("Delivery No");
                table.AddColumn("Customer AWH Date");
                table.AddColumn("Desc");

                table.AddColumn("Vendor Name");
                table.AddColumn("Merchandiser");
                table.AddColumn("Item No");
                table.AddColumn("WorkFlowStatus");
                table.AddColumn("Office");
                table.AddColumn("Purchase Term");
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    string[] row = table.NewRow();
                    row[table.ColumnId("ShipmentId")] = (r.IsShipmentIdNull() ? -1 : r.ShipmentId).ToString();
                    row[table.ColumnId("VendorId")] = (r.IsVendorIdNull() ? -1 : r.VendorId).ToString();
                    row[table.ColumnId("OfficeId")] = (r.IsOfficeIdNull() ? -1 : r.OfficeId).ToString();
                    row[table.ColumnId("PurchaseTermId")] = (r.IsTermOfPurchaseIdNull() ? -1 : r.TermOfPurchaseId).ToString();
                    row[table.ColumnId("ProductId")] = (r.IsProductIdNull() ? -1 : r.ProductId).ToString();
                    row[table.ColumnId("MerchandiserId")] = (r.IsMerchandiserIdNull() ? -1 : r.MerchandiserId).ToString();
                    row[table.ColumnId("WorkFlowStatusId")] = (r.IsWorkflowStatusIdNull() ? -1 : r.WorkflowStatusId).ToString();

                    row[table.ColumnId("Remark")] = (r.IsRemarkNull() ? string.Empty : r.Remark);
                    row[table.ColumnId("Contract No")] = (r.IsContractNoNull() ? string.Empty : r.ContractNo);
                    row[table.ColumnId("Delivery No")] = (r.IsDeliveryNoNull() ? -1 : r.DeliveryNo).ToString();
                    row[table.ColumnId("Customer AWH Date")] = (r.IsCustomerAtWarehouseDateNull() ? DateTime.MinValue : r.CustomerAtWarehouseDate).ToString("dd/MM/yyyy");
                    row[table.ColumnId("Desc")] = (r.IsDescriptionNull() ? string.Empty : r.Description);

                    table.Rows.Add(row);
                }
            }
            return table;
        }

        public ArrayList getVendorAlertDetailList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetVendorAlertDetailList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }


        public ArrayList getVendorAlertCount(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetVendorAlertCount");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = departmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            DataSet dataSet = new DataSet();
            int recordsAffected = ad.Fill(dataSet);

            ArrayList vendorAlert = new ArrayList();
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
            {
                vendorAlert.Add(0);
                vendorAlert.Add(0);
                vendorAlert.Add(0);
                vendorAlert.Add(0);
            }
            else
            {
                vendorAlert.Add(dataSet.Tables[0].Rows[0][0]);
                vendorAlert.Add(dataSet.Tables[0].Rows[0][1]);
                vendorAlert.Add(dataSet.Tables[0].Rows[0][2]);
                vendorAlert.Add(dataSet.Tables[0].Rows[0][3]);
            }
            return vendorAlert;
        }

        public AlertTable getVendorCompanyList(int userId, TypeCollector officeIdList, string departmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            AlertTable table = new AlertTable();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetVendorCompanyList");
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                table.AddColumn("VendorId");
                table.AddColumn("OfficeId");
                table.AddColumn("PurchaseTermId");
                table.AddColumn("Remark");
                table.AddColumn("Vendor Name");
                table.AddColumn("Office");
                table.AddColumn("Purchase Term");
                table.AddColumn("EpicorSupplierId");
                table.AddColumn("EpicorCompanyId");
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    string[] row = table.NewRow();
                    row[table.ColumnId("VendorId")] = (r.IsVendorIdNull() ? -1 : r.VendorId).ToString();
                    row[table.ColumnId("OfficeId")] = (r.IsOfficeIdNull() ? -1 : r.OfficeId).ToString();
                    row[table.ColumnId("PurchaseTermId")] = (r.IsTermOfPurchaseIdNull() ? -1 : r.TermOfPurchaseId).ToString();
                    row[table.ColumnId("Remark")] = (r.IsRemarkNull() ? string.Empty : r.Remark);
                    string[] remark = (row[table.ColumnId("Remark")] + "|").Split('|');
                    row[table.ColumnId("EpicorSupplierId")] = remark[0];
                    row[table.ColumnId("EpicorCompanyId")] = remark[1];
                    table.Rows.Add(row);
                }
            }
            return table;
        }

        public ArrayList getEziBuyOSPaymentList_ToBeRemoved(int UserId, TypeCollector OfficeIdList, string DepartmentCode)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetEziBuyOSPaymentList");
            ad.SelectCommand.Parameters["@UserId"].Value = UserId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(OfficeIdList.IsInclusive, OfficeIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = DepartmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow AlertRow in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(AlertRow, rf);
                    rf.ApprovalDate = getGoldSealApprovalDate(rf.ShipmentId);
                    list.Add(rf);
                }
            }
            return list;
        }

        public ArrayList getEziBuyOSPaymentList(int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector departmentIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetEziBuyOSPaymentList");
            ad.SelectCommand.Parameters["@UserId"].Value = UserId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(OfficeIdList.IsInclusive, OfficeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = DepartmentCode;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected >= 1)
            {
                TypeCollector shipmentIdList = TypeCollector.Inclusive;
                for (int i = 0; i < ds.AlertNotification.Count; i++)
                {
                    AlertNotificationDs.AlertNotificationRow AlertRow = (AlertNotificationDs.AlertNotificationRow)ds.AlertNotification.Rows[i];
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(AlertRow, rf);
                    shipmentIdList.append(rf.ShipmentId);
                    list.Add(rf);
                }
                loadGoldSealApprovalDate(list, shipmentIdList);
            }
            return list;
        }


        public int getEziBuyOSPaymentListCount(int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector departmentIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetEziBuyOSPaymentListCount");
            ad.SelectCommand.Parameters["@UserId"].Value = UserId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(OfficeIdList.IsInclusive, OfficeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = DepartmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
                return 0;
            else
                return (int)(dataSet.Tables[0].Rows[0][0]);
        }

        public int getEziBuyOSPaymentListCount_Fast(int UserId, TypeCollector OfficeIdList, string DepartmentCode, TypeCollector departmentIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetEziBuyOSPaymentCount_Fast");
            ad.SelectCommand.Parameters["@UserId"].Value = UserId;
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(OfficeIdList.IsInclusive, OfficeIdList.Values);
            ad.SelectCommand.CustomParameters["@DepartmentIdList"] = CustomDataParameter.parse(departmentIdList.IsInclusive, departmentIdList.Values);
            ad.SelectCommand.Parameters["@DepartmentCode"].Value = DepartmentCode;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            return recordsAffected;
        }

        public void loadGoldSealApprovalDate(ArrayList osPaymentList, TypeCollector shipmentIdList)
        {
            AlertNotificationRef alert = new AlertNotificationRef();

            NssProgressDs ds = new NssProgressDs();
            IDataSetAdapter ad = getDataSetAdapter("ProductionProgressApt", "GetGoldSealProgressByShipmentIdList");
            ad.SelectCommand.CustomParameters["@ShipmentIdList"] = CustomDataParameter.parse(shipmentIdList.IsInclusive, shipmentIdList.Values);
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationRef rf in osPaymentList)
                    foreach (NssProgressDs.ProgressRow row in ds.Progress)
                        if (rf.ShipmentId == row.ShipmentId)
                        {
                            if (row.IsActualDateNull())
                                rf.ApprovalDate = DateTime.MinValue;
                            else
                                rf.ApprovalDate = row.ActualDate;
                            break;
                        }
            }
            return;
        }


        public DateTime getGoldSealApprovalDate(int shipmentId)
        {
            NssProgressDs ds = new NssProgressDs();
            IDataSetAdapter ad = getDataSetAdapter("ProductionProgressApt", "GetGoldSealProgressByShipmentId");
            ad.SelectCommand.Parameters["@ShipmentId"].Value = shipmentId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
                return (ds.Progress[0].IsActualDateNull() ? DateTime.MinValue : ds.Progress[0].ActualDate);
            else
                return DateTime.MinValue;
        }


        public ArrayList getNTInvoiceApprovalGroupCount(int userId, TypeCollector officeIdList)
        {
            // get the count of invoice for :
            // Row  0 - Department Approval
            // Row  1 - 1st Level Accounts Approval 
            // Row  2 - 2nd Level Accounts Approval 
            // Row  3 - Accounts Approved
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetNTInvoiceApprovalGroupCount");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList osInv = new ArrayList();
            if (Convert.IsDBNull(dataSet.Tables[0].Rows[0][0]))
            {
                osInv.Add(0);
                osInv.Add(0);
                osInv.Add(0);
                osInv.Add(0);
            }
            else
            {
                osInv.Add(dataSet.Tables[0].Rows[0][0]);
                osInv.Add(dataSet.Tables[0].Rows[0][1]);
                osInv.Add(dataSet.Tables[0].Rows[0][2]);
                osInv.Add(dataSet.Tables[0].Rows[0][3]);
            }
            return osInv;

        }

        public ArrayList getNTInvoiceApprovalDetailList(int userId, TypeCollector officeIdList, int groupId)
        {
            //GroupId : 1 - Department Approval
            //          2 - 1st Level Accounts Approval 
            //          3 - 2nd Level Accounts Approval 
            //          4 - Accounts Approved (pending for generating Sun Interface)

            AlertNotificationDs dataSet = new AlertNotificationDs();
            ArrayList list = new ArrayList();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetNTInvoiceApprovalDetailList");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@GroupId"].Value = groupId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow row in dataSet.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(row, rf);
                    list.Add(rf);
                }
            }
            return list;
        }

        public ArrayList getNTInvoiceApprovalGroupList(int userId, TypeCollector officeIdList, int groupId)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetNTInvoiceApprovalGroupList");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@GroupId"].Value = groupId;
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }


        public ArrayList getNTVendorGroupCount(int userId, TypeCollector officeIdList)
        {
            DataSet dataSet = new DataSet();

            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetNTVendorGroupCount");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            ad.SelectCommand.DbCommand.CommandTimeout = 120;
            int recordsAffected = ad.Fill(dataSet);

            ArrayList countList = new ArrayList();
            if (recordsAffected == 0)
            {
                countList.Add(0);
                countList.Add(0);
            }
            else
            {
                countList.Add(dataSet.Tables[0].Rows[0][0]);
                countList.Add(dataSet.Tables[0].Rows[0][1]);
            }
            return countList;
        }

        public ArrayList getNTVendorDetailGroupList(int userId, TypeCollector officeIdList)
        {
            ArrayList list = new ArrayList();
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationApt", "GetNTVendorDetailGroupList");
            ad.SelectCommand.CustomParameters["@OfficeIdList"] = CustomDataParameter.parse(officeIdList.IsInclusive, officeIdList.Values);
            ad.SelectCommand.Parameters["@UserId"].Value = userId;
            AlertNotificationDs ds = new AlertNotificationDs();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            int recordsAffected = ad.Fill(ds);
            if (recordsAffected > 0)
            {
                foreach (AlertNotificationDs.AlertNotificationRow r in ds.AlertNotification)
                {
                    AlertNotificationRef rf = new AlertNotificationRef();
                    AlertNotificationMapping(r, rf);
                    list.Add(rf);
                }
            }
            return list;
        }

        internal void AlertNotificationMapping(Object source, Object target)
        {
            AlertNotificationDs.AlertNotificationRow row = (AlertNotificationDs.AlertNotificationRow)source;
            AlertNotificationRef rf = (AlertNotificationRef)target;

            rf.ShipmentId = (row.IsShipmentIdNull() ? -1 : row.ShipmentId);
            rf.ContractId = (row.IsContractIdNull() ? -1 : row.ContractId);
            rf.ContractNo = (row.IsContractNoNull() ? string.Empty : row.ContractNo);
            rf.DeliveryNo = (row.IsDeliveryNoNull() ? -1 : row.DeliveryNo);
            rf.Office = generalWorker.getOfficeRefByKey(row.OfficeId);
            rf.Vendor = VendorWorker.Instance.getVendorByKey((row.IsVendorIdNull() ? -1 : row.VendorId));
            rf.Product = productWorker.getProductByKey(row.IsProductIdNull() ? -1 : row.ProductId);
            rf.Merchandiser = generalWorker.getUserByKey(row.IsMerchandiserIdNull() ? -1 : row.MerchandiserId);
            rf.WorkflowStatus = ContractWFS.getType(row.IsWorkflowStatusIdNull() ? -1 : row.WorkflowStatusId);
            rf.CustomerAtWarehouseDate = (row.IsCustomerAtWarehouseDateNull() ? DateTime.MinValue : row.CustomerAtWarehouseDate);
            rf.SupplierAtWarehouseDate = (row.IsSupplierAtWarehouseDateNull() ? DateTime.MinValue : row.SupplierAtWarehouseDate);
            rf.ShippingDocReceiptDate = (row.IsShippingDocReceiptDateNull() ? DateTime.MinValue : row.ShippingDocReceiptDate);
            rf.SupplierInvoiceNo = (row.IsSupplierInvoiceNoNull() ? String.Empty : row.SupplierInvoiceNo);
            rf.IsUploadDMSDocument = (row.IsIsUploadDMSDocumentNull() ? false : row.IsUploadDMSDocument);
            rf.InvoiceNo = (row.IsInvoiceNoNull() ? String.Empty : row.InvoiceNo);
            rf.InvoiceDate = (row.IsInvoiceDateNull() ? DateTime.MinValue : row.InvoiceDate);
            rf.InvoiceUploadUser = generalWorker.getUserByKey(row.IsInvoiceUploadUserIdNull() ? -1 : row.InvoiceUploadUserId);
            rf.ILSInvoiceStatus = ILSInvoiceUploadStatus.getType(row.IsILSInvoiceStatusNull() ? -1 : row.ILSInvoiceStatus);
            rf.TermOfPurchase = commonWorker.getTermOfPurchaseByKey(row.IsTermOfPurchaseIdNull() ? -1 : row.TermOfPurchaseId);
            rf.ActualAtWarehouseDate = (row.IsActualAtWarehouseDateNull() ? DateTime.MinValue : row.ActualAtWarehouseDate);
            rf.ShippingDocCheckedDate = (row.IsShippingDocCheckedOnNull() ? DateTime.MinValue : row.ShippingDocCheckedOn);
            rf.TotalShippedAmt = (row.IsTotalShippedAmtNull() ? -1 : row.TotalShippedAmt);
            rf.TotalShippedSupplierGmtAmt = (row.IsTotalShippedSupplierGmtAmtNull() ? -1 : row.TotalShippedSupplierGmtAmt);
            rf.ApprovalDate = (row.IsApprovalDateNull() ? DateTime.MinValue : row.ApprovalDate);
            rf.Remark = (row.IsRemarkNull() ? string.Empty : row.Remark);
            rf.Description = (row.IsDescriptionNull() ? string.Empty : row.Description);
            rf.AlertType = (row.IsAlertTypeNull() ? "SHIPMENT" : row.AlertType);
            rf.Currency = (row.IsCurrencyIdNull() ? null : generalWorker.getCurrencyByKey(row.CurrencyId));
            if (rf.AlertType == "NONTRADE")
            {
                rf.GroupId = (row.IsGroupIdNull() ? -1 : row.GroupId);
                rf.DocumentId = (row.IsDocumentIdNull() ? -1 : row.DocumentId);
                rf.Amount = (row.IsAmountNull() ? 0 : row.Amount);
                rf.Date = (row.IsDateNull() ? DateTime.MinValue : row.Date);
                UserRef user = new UserRef();
                if (!row.IsUserIdNull())
                    user = generalWorker.getUserByKey(row.UserId);
                else
                    user.DisplayName = string.Empty;
                rf.User = user;

                NTVendorDef ntVendor = null;
                if (!row.IsVendorIdNull())
                    ntVendor = NonTradeWorker.Instance.getNTVendorByKey(row.VendorId);
                if (ntVendor != null)
                    rf.Remark += Delimiter + ntVendor.VendorName + Delimiter + (ntVendor.ExpenseType != null ? ntVendor.ExpenseType.Description : "");
            }

        }

        public ArrayList getProductDepartmentList(int userId)
        {
            IDataSetAdapter ad = getDataSetAdapter("AlertNotificationSupportApt", "GetProductDepartmentIdByUserId");
            ad.SelectCommand.Parameters["@UserId"].Value = userId.ToString();
            ad.SelectCommand.DbCommand.CommandTimeout = 120;

            OfficeStructureDs dataSet = new OfficeStructureDs();
            int recordsAffected = ad.Fill(dataSet);
            if (recordsAffected < 1) return null;

            ArrayList list = new ArrayList();
            foreach (OfficeStructureDs.OfficeStructureRow row in dataSet.OfficeStructure)
            {
                ProductDepartmentRef rf = new ProductDepartmentRef();
                rf.Code = row.Code;
                rf.Description = row.Description;
                rf.ProductDepartmentId = row.OfficeStructureId;
                rf.Status = GeneralStatus.ACTIVE;
                rf.OfficeId = GeneralCriteria.ALL;
                list.Add(rf);
            }
            return list;
        }


    }

    #region Alert Table class

    [Serializable]
    public class AlertTable
    {
        //public AlertTable () {}
        const string undefineString = "[Undefine]";

        public AlertTable()
        {
            int NumberOfColumn = 10;    // default
            this.ColumnCount = 0;
            this.ColumnName = new string[NumberOfColumn];
            this.Rows = new ArrayList();
            AddColumn(undefineString);
        }

        public AlertTable CopyColumns()
        {
            AlertTable newTable = new AlertTable();
            for (int i = 0; i < this.ColumnCount; i++)
                newTable.AddColumn(this.ColumnName[i]);
            return newTable;
        }

        public string[] NewRow()
        {
            string[] row = new string[this.ColumnCount];
            row[0] = undefineString;
            return row;
        }

        //public void initColumn(
        Hashtable ht = new Hashtable();

        private string formatColumnName(string columnName)
        {
            return columnName.ToUpper().Trim().Replace("  ", " ").Replace(" ", "_");
        }


        public int AddColumn(string columnName)
        {
            string key = formatColumnName(columnName);
            if (ht[key] == null)
            {
                if (this.ColumnCount >= this.ColumnName.Length)
                {
                    string[] oldColumnName = this.ColumnName;
                    this.ColumnName = new string[this.ColumnCount + 1];
                    oldColumnName.CopyTo(this.ColumnName, 0);
                }
                ht.Add(key, this.ColumnCount);
                this.ColumnName[this.ColumnCount] = columnName.Trim().Replace("  ", " ");
                return ++this.ColumnCount;
            }
            else
                return (int)ht[key];
        }

        public int ColumnId(string columnName)
        {
            string key = formatColumnName(columnName);
            return (ht[key] == null ? 0 : (int)ht[key]); ;
        }

        public int ColumnCount { get; set; }
        public string[] ColumnName { get; set; }
        public ArrayList Rows { get; set; }
    }
    #endregion

}

