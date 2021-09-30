using System;
using System.IO;
using System.Xml.Serialization;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using com.next.common.datafactory.worker;
using com.next.common.domain;

namespace com.next.isam.reporter.helper
{
	public class ReportHelper
	{
		public ReportHelper()
		{
		}

		public static DataSet convertObjectToDataSet(object obj, string dataSetName)
		{
			StringWriter writer = xmlSerializeObject(obj);
			DataSet ds = new DataSet(dataSetName);

			StringReader reader = new StringReader(writer.ToString());
			ds.ReadXml(reader);

			return ds;
		}

		public static StringWriter xmlSerializeObject(object obj)
		{
			StringWriter writer = new StringWriter();
			XmlSerializer serializer;
			serializer = 	new XmlSerializer(obj.GetType());
			serializer.Serialize(writer, obj);

			return writer;
		}

		public static void setCommonDisplayValue(ReportDocument rpt, int userId, string reportCode)
		{
			setCommonDisplayValue(rpt, userId, reportCode, null);
		}

		public static void setCommonDisplayValue(ReportDocument rpt, int userId, string reportCode, string criteria)
		{
			UserRef user = GeneralWorker.Instance.getUserByKey(userId);
			string displayName = (user!=null)? user.DisplayName : "Unknown user";
			TimeDiffRef tdRef = (user!=null)? GeneralWorker.Instance.getTimeDiffByOffice(user.Department.Office.OfficeId) : null;
			string printedTime = (tdRef!=null)?
				DateTime.Now.AddHours(tdRef.HourDiff).ToString("dd/MM/yyyy HH:mm") + " (" + tdRef.TimeZoneCode + ")" :
				DateTime.Now.ToString("dd/MM/yyyy HH:mm");

			if (criteria!=null) rpt.SetParameterValue("Criteria", criteria);
			rpt.SetParameterValue("ReportIdentity", "Printed on: " + printedTime + " / Printed by: " + displayName + " / " + reportCode);
		}

		public static void export(ReportClass rpt, System.Web.HttpResponse response,  ExportFormatType format, string reportFilename)
		{
            using (rpt)
            {
                rpt.ExportToHttpResponse(format, response, true, reportFilename);
            }
		}

        public static void export(Byte[] rptContents, ExportFormatType format, System.Web.HttpResponse response)
        {
            string filename = "download";
            if (format == ExportFormatType.Excel)
            {
                filename += ".xls";
                response.ContentType = "application/vnd.ms-excel";
            }
            else if (format == ExportFormatType.PortableDocFormat)
            {
                filename += ".pdf";
                response.ContentType = "application/pdf";
            }
            
            string disHeader = "attachment; filename=\"" + filename + "\"";
            response.AppendHeader("Content-Disposition", disHeader);
            response.AppendHeader("Content-Location", "undefined");
            response.BinaryWrite(rptContents);
            response.End();
        }

	}
}
