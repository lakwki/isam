using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using com.next.common.domain;
using com.next.common.domain.types;
using com.next.isam.appserver.claim;
using com.next.isam.domain.claim;
using CrystalDecisions.Shared;
using com.next.isam.reporter.accounts;
using com.next.common.web.commander;

namespace com.next.isam.webapp.webservices
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Reporter : System.Web.Services.WebService
    {
        private string alias = "isam_eb";

        [WebMethod]
        public Byte[] getUKClaimOutstandingReport(int officeId, int vendorId, int termOfPurchaseId, DateTime cutOffDate, int rptOptionId, int ncOptionId, decimal gbpRate, decimal eurRate, int userId, string reportCode)
        {
            string url = @"http://localhost/" + alias + "/reporter/OutstandingClaimListReport.aspx";
            url += "?officeId=" + Convert.ToString(officeId);
            url += "&handlingOfficeId=-1";
            url += "&handlingOfficeName=";
            url += "&termOfPurchaseId=" + Convert.ToString(termOfPurchaseId);
            url += "&orderType=" + Convert.ToString(termOfPurchaseId);
            url += "&cutOffDate=" + Convert.ToString(cutOffDate);
            url += "&eurRate=0";
            url += "&gbpRate=0";
            url += "&gbpAccrualAmt=0";
            url += "&eurAccrualAmt=0";
            url += "&usdAccrualAmt=0";
            url += "&rptOption=" + Convert.ToString(rptOptionId);
            url += "&ncOption=" + Convert.ToString(ncOptionId);
            url += "&vendorId=" + Convert.ToString(vendorId);
            url += "&userId=" + userId.ToString();
            url += "&reportCode=" + reportCode;

            return this.getByteContents(url);
        }

        [WebMethod]
        public Byte[] getMFRNQtyAnalysisReport(int fiscalYear, int periodFrom, int periodTo, string reportCode)
        {
            string url = @"http://localhost/" + alias + "/reporter/MFRNQtyAnalysisReport.aspx";
            url += "?fiscalYear=" + Convert.ToString(fiscalYear);
            url += "&periodFrom=" + Convert.ToString(periodFrom);
            url += "&periodTo=" + Convert.ToString(periodTo);
            url += "&reportCode=" + reportCode;

            return this.getByteContents(url);
        }

        [WebMethod]

        public Byte[] getUKClaimPhasingReportByOfficeClaimReason(int fiscalYear, int periodFrom, int periodTo, int vendorId, int officeId, int userId, string reportCode, string exportFormat)
        {
            string vendorDesc = (vendorId == -1 ? "ALL" : IndustryUtil.getVendorByKey(vendorId).Name);

            Stream stream;
            if (exportFormat == "PDF")
            {
                stream = AccountReportManager.Instance.getUKClaimPhasingReportByOfficeClaimReason(fiscalYear, periodFrom, periodTo, vendorId, vendorDesc, officeId, userId, reportCode).ExportToStream(ExportFormatType.PortableDocFormat);
                return this.ReadToEnd(stream);
            }
            else if (exportFormat == "EXCEL")
            {
                stream = AccountReportManager.Instance.getUKClaimPhasingReportByOfficeClaimReason(fiscalYear, periodFrom, periodTo, vendorId, vendorDesc, officeId, userId, reportCode).ExportToStream(ExportFormatType.Excel);
                return this.ReadToEnd(stream);
            }
            else
                return null;
        }

        public Byte[] getUKClaimPhasingReportByOfficeClaimReason(int fiscalYear, int periodFrom, int periodTo, int vendorId, int userId, string reportCode, string exportFormat)
        {
            return getUKClaimPhasingReportByOfficeClaimReason(fiscalYear, periodFrom, periodTo, vendorId, -1, userId, reportCode, exportFormat);
        }


        [WebMethod]
        public Byte[] getUKClaimPhasingReportByOffice(int fiscalYear, int periodFrom, int periodTo, int vendorId, int userId, string reportCode, string exportFormat)
        {
            string vendorDesc = (vendorId == -1 ? "ALL" : IndustryUtil.getVendorByKey(vendorId).Name);

            Stream stream;
            if (exportFormat == "PDF")
            {
                stream = AccountReportManager.Instance.getUKClaimPhasingReportByOffice(fiscalYear, periodFrom, periodTo, vendorId, vendorDesc, userId, reportCode).ExportToStream(ExportFormatType.PortableDocFormat);
                return this.ReadToEnd(stream);
            }
            else if (exportFormat == "EXCEL")
            {
                stream = AccountReportManager.Instance.getUKClaimPhasingReportByOffice(fiscalYear, periodFrom, periodTo, vendorId, vendorDesc, userId, reportCode).ExportToStream(ExportFormatType.Excel);
                return this.ReadToEnd(stream);
            }
            else
                return null;
        }

        [WebMethod]
        public Byte[] getUKClaimPhasingReportByOfficeClaimType(int fiscalYear, int periodFrom, int periodTo, int vendorId, int officeId, int userId, string reportCode, string exportFormat)
        {
            string vendorDesc = (vendorId == -1 ? "ALL" : IndustryUtil.getVendorByKey(vendorId).Name);

            Stream stream;
            if (exportFormat == "PDF")
            {
                stream = AccountReportManager.Instance.getUKClaimPhasingReportByOfficeClaimType(fiscalYear, periodFrom, periodTo, vendorId, vendorDesc, officeId, userId, reportCode).ExportToStream(ExportFormatType.PortableDocFormat);
                return this.ReadToEnd(stream);
            }
            else if (exportFormat == "EXCEL")
            {
                stream = AccountReportManager.Instance.getUKClaimPhasingReportByOfficeClaimType(fiscalYear, periodFrom, periodTo, vendorId, vendorDesc, officeId, userId, reportCode).ExportToStream(ExportFormatType.Excel);
                return this.ReadToEnd(stream);
            }
            else
                return null;
        }

        public Byte[] getUKClaimPhasingReportByOfficeClaimType(int fiscalYear, int periodFrom, int periodTo, int vendorId, int userId, string reportCode, string exportFormat)
        {
            return getUKClaimPhasingReportByOfficeClaimType(fiscalYear, periodFrom, periodTo, vendorId, -1, userId, reportCode, exportFormat);
        }

        [WebMethod]
        public Byte[] getUKClaimPhasingReportBySupplier(int fiscalYear, int periodFrom, int periodTo, int vendorId, int officeId, int reportType, int userId, string reportCode)
        {
            //string vendorDesc = (vendorId == -1 ? "ALL" : IndustryUtil.getVendorByKey(vendorId).Name);
            string url = @"http://localhost/" + alias + "/reporter/UKClaimPhasingReport.aspx";
            url += "?fiscalYear=" + Convert.ToString(fiscalYear);
            url += "&period=" + Convert.ToString(periodFrom) + "," + Convert.ToString(periodTo);
            url += "&vendorId=" + Convert.ToString(vendorId);
            url += "&officeId=" + Convert.ToString(officeId);
            url += "&docType=" + reportType.ToString();
            url += "&userId=" + userId.ToString();
            url += "&reportCode=" + reportCode;

            return this.getByteContents(url);
        }

        [WebMethod]
        public Byte[] getUKClaimPhasingReportByProductTeam(int fiscalYear, int periodFrom, int periodTo, int vendorId, int officeId, int reportType, int userId, string reportCode)
        {
            string url = @"http://localhost/" + alias + "/reporter/UKClaimPhasingByProductTeamReport.aspx";
            url += "?fiscalYear=" + Convert.ToString(fiscalYear);
            url += "&period=" + Convert.ToString(periodFrom) + "," + Convert.ToString(periodTo);
            url += "&vendorId=" + Convert.ToString(vendorId);
            url += "&officeId=" + Convert.ToString(officeId);
            url += "&docType=" + reportType.ToString();
            url += "&userId=" + userId.ToString();
            url += "&reportCode=" + reportCode;

            return this.getByteContents(url);
        }

 

        [WebMethod]
        public Byte[] getUKClaimAuditLogReport(int officeId, DateTime issueDateFrom, DateTime issueDateTo, int productTeamId, int vendorId, int claimTypeId, int userId, string reportCode, string exportFormat)
        {
            TypeCollector officeIdList = TypeCollector.Inclusive;
            if (officeId != -1)
                officeIdList.append(officeId);
            else
            {
                ArrayList userOfficeList = CommonUtil.getOfficeListByUserId(userId, OfficeStructureType.PRODUCTCODE.Type);
                foreach (OfficeRef oRef in userOfficeList)
                    officeIdList.append(oRef.OfficeId);
            }

            string officeDesc = (officeId == -1 ? "ALL" : CommonUtil.getOfficeRefByKey(officeId).Description);
            string productTeamDesc = (productTeamId == -1 ? "ALL" : CommonUtil.getProductCodeDefByKey(productTeamId).CodeDescription);
            string vendorDesc = (vendorId == -1 ? "ALL" : IndustryUtil.getVendorByKey(vendorId).Name);
            string claimTypeDesc = (claimTypeId == -1 ? "ALL" : UKClaimType.getType(claimTypeId).Name);

            Stream stream;
            if (exportFormat == "PDF")
            {
                stream = AccountReportManager.Instance.getUKClaimAuditLogReport(issueDateFrom, issueDateTo, officeIdList, officeDesc, productTeamId, productTeamDesc, vendorId, vendorDesc, claimTypeId, claimTypeDesc, userId, reportCode).ExportToStream(ExportFormatType.PortableDocFormat);
                return this.ReadToEnd(stream);
            }
            else if (exportFormat == "EXCEL")
            {
                stream = stream = AccountReportManager.Instance.getUKClaimAuditLogReport(issueDateFrom, issueDateTo, officeIdList, officeDesc, productTeamId, productTeamDesc, vendorId, vendorDesc, claimTypeId, claimTypeDesc, userId, reportCode).ExportToStream(ExportFormatType.Excel);
                return this.ReadToEnd(stream);
            }
            else
                return null;

        }


        [WebMethod]
        public byte[] getUKClaimSummaryReport(DateTime ukDNDateFrom, DateTime ukDNDateTo, int fiscalYear, int periodFrom, int periodTo, object[] officeIds, string officeDesc, int productTeamId, string productTeamDesc,
            int vendorId, string vendorDesc, object[] claimTypeIds, string claimTypeDesc, int claimReasonId, string claimReasonDesc, int printUserId, string reportCode, string exportFormat)
        {
            TypeCollector officeIdList = TypeCollector.Inclusive;
            foreach (int id in officeIds)
                officeIdList.append(id);

            /*
            TypeCollector claimReasonIdList = TypeCollector.Inclusive;
            foreach (int id in claimReasonIds)
                claimReasonIdList.append(id);
            */
            TypeCollector claimTypeIdList = TypeCollector.Inclusive;
            foreach (int id in claimTypeIds)
                claimTypeIdList.append(id);
            

            Stream stream;
            if (exportFormat == "PDF")
            {
                stream = AccountReportManager.Instance.getUKClaimSummaryReport(ukDNDateFrom, ukDNDateTo, fiscalYear, periodFrom, periodTo, officeIdList, officeDesc, productTeamId, productTeamDesc,
                    vendorId, vendorDesc, claimTypeIdList, claimTypeDesc, claimReasonId, claimReasonDesc, printUserId, reportCode).ExportToStream(ExportFormatType.PortableDocFormat);
                return this.ReadToEnd(stream);
            }
            else if (exportFormat == "EXCEL")
            {
                stream = AccountReportManager.Instance.getUKClaimSummaryReport(ukDNDateFrom, ukDNDateTo, fiscalYear, periodFrom, periodTo, officeIdList, officeDesc, productTeamId, productTeamDesc,
                    vendorId, vendorDesc, claimTypeIdList, claimTypeDesc, claimReasonId, claimReasonDesc, printUserId, reportCode).ExportToStream(ExportFormatType.Excel);
                return this.ReadToEnd(stream);
            }
            else
                return null;
        }

        [WebMethod]
        [SoapInclude(typeof(UKClaimType))]
        [XmlInclude(typeof(UKClaimType))]
        public object[] getUKClaimTypeList()
        {
            List<UKClaimType> list = UKClaimType.getCollectionValues();
            object[] typeList = new object[list.Count];
            int i = 0;
            foreach (object o in list)
            {
                typeList[i] = o;
                i++;
            }
            return typeList;
        }


        [WebMethod]
        [SoapInclude(typeof(UKClaimReasonDef))]
        [XmlInclude(typeof(UKClaimReasonDef))]
        public object[] getUKClaimReasonList()
        {
            List<UKClaimReasonDef> list = UKClaimManager.Instance.getUKClaimReasonList(GeneralCriteria.ALL);
            object[] reasonList = new object[list.Count];
            int i = 0;
            foreach (object o in list)
            {
                reasonList[i] = o;
                i++;
            }
            return reasonList;

        }

        private Byte[] getByteContents(string url)
        {
            WebRequest req = HttpWebRequest.Create(url);

            WebResponse resp = req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string s = sr.ReadToEnd();
            sr.Close();

            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(s);

        }

        private byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

    }
}





