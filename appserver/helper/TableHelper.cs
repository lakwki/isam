using System;
using System.IO;
using System.Data;
using System.Collections;

namespace com.next.isam.appserver.helper
{
    public class TableHelper
    {
        private static TableHelper _instance;
        
        public TableHelper() { }

        public static TableHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TableHelper();
                }
                return _instance;
            }
        }


        //private static GeneralWorker gworker = GeneralWorker.Instance;

        public ArrayList SourceTable { get; set; }

        public void ImportDataTable(DataTable dt)
        {
            ArrayList import = new ArrayList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ArrayList row = new ArrayList();
                for (int j = 0; j < dt.Rows[i].Table.Columns.Count; j++) 
                {
                    row.Add(dt.Rows[i][j]);
                    //((DataColumn)dt.Rows[i][j]).caption
                }
                import.Add(row);
            }
        }


        public string generateHtmlTable(ArrayList tableRow)
        {
            string fontSize="12";
            string headerBackColor = "#CCCCFF";
            string dataBackColor = "#FFFFFF";

            int lastStyleRow = 0;
            string sHtml = "<TABLE BORDER=1 CELLPADDING=2 CELLSPACING=0>\n";
            for (int r = 0; r < tableRow.Count; r++)
            {
                if (((ArrayList)tableRow[r]).Count > 0)
                    switch (((ArrayList)tableRow[r])[0].ToString())
                    {
                        case "STYLE":
                            lastStyleRow = r;
                            break;
                        case "HEADER":
                            sHtml += "<TR STYLE='FONT-SIZE:" + fontSize + ";BACKGROUND-COLOR:" + headerBackColor + ";'>\n";
                            for (int i = 1; i < ((ArrayList)tableRow[lastStyleRow]).Count; i++)
                            {
                                sHtml += "<TH " + (((ArrayList)tableRow[lastStyleRow])[i]).ToString() + ">";
                                sHtml += (((ArrayList)tableRow[r])[i]).ToString();
                                sHtml += "</TH>\n";
                            }
                            sHtml += "</TR>\n";
                            break;
                        case "DATA":
                            sHtml += "<TR STYLE='FONT-SIZE:" + fontSize + ";BACKGROUND-COLOR:" + dataBackColor + ";'>\n";
                            //sHtml += "<TR>\n";
                            for (int i = 1; i < ((ArrayList)tableRow[lastStyleRow]).Count; i++)
                            {
                                sHtml += "<TD " + (((ArrayList)tableRow[lastStyleRow])[i]).ToString() + ">";
                                if (i < ((ArrayList)tableRow[r]).Count)
                                    sHtml += ((((ArrayList)tableRow[r])[i]) == null ? "" : (((ArrayList)tableRow[r])[i]).ToString());
                                sHtml += "</TD>\n";
                            }
                            sHtml += "</TR>\n";
                            break;
                        default:
                            sHtml += "<TR STYLE='FONT-SIZE:" + fontSize + ";BACKGROUND-COLOR:#FF0000;'>\n";
                            for (int i = 0; i < ((ArrayList)tableRow[r]).Count; i++)
                                sHtml += "<TD>" + (((ArrayList)tableRow[r])[i]).ToString() + "</TD>\n";
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

            string fileFolder = "";// WebConfig.getValue("appSettings", "LC_APPLICATION");
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
                textBody += "\n\r";
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

    }

}
