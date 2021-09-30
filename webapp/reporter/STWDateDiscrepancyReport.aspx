<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme"  AutoEventWireup="true" CodeBehind="STWDateDiscrepancyReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.STWDateDiscrepancyReport" Title="STW Date Discrepancy Report" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<%@ Register Src="~/usercontrol/UclSortingOrder.ascx" TagName="UclSortingOrder" TagPrefix="uso1"  %>
<%@ Register Src="~/usercontrol/UclOfficeProductTeamSelection.ascx" TagName="UclOfficeProductTeamSelection" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_reports_StwDateDiscrepancyReport.gif" runat="server" id="imgHeaderText" />
-->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!--
<table width="100%" cellpadding=0 cellspacing=0>
<tr>
    <td class='tableHeader' style='font-size:135%;color:#ff9900;font-stretch:narrower;height:25px;' width='70px' >&nbsp;&nbsp;Report&nbsp;&nbsp;</td>
    <td class="tableHeader" style=' font-size:80%;color:Black;font-weight:bold;' >STW Date Discrepancy Report</td>
</tr>
</table>
-->
<asp:Panel runat="server" SkinID="sectionHeader_Report">STW Date Discrepancy Report</asp:Panel>

<script type="text/javascript">
     function copyInvoiceNo()
     {        
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value.toUpperCase();
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value;        
     }  
     
     function isFormValid()
     {
     
        if (((document.getElementById("ctl00_ContentPlaceHolder1_rad_InvoiceDate").checked &&
              document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
              document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "") ||
             (document.getElementById("ctl00_ContentPlaceHolder1_rad_FiscalPeriod").checked &&
              (document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodFrom").value=="" || document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodTo").value==""))) &&
            (document.getElementById("ctl00_ContentPlaceHolder1_txt_IlsStwDateFrom_txt_IlsStwDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_IlsStwDateTo_txt_IlsStwDateTo_textbox").value == "" ) &&
            (document.getElementById("ctl00_ContentPlaceHolder1_txt_ActualStwDateFrom_txt_ActualStwDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_ActualStwDateTo_txt_ActualStwDateTo_textbox").value == "") )
            {
                alert("Please enter search criteria on one of below search criteria.\r\n    - Fiscal Period" +
                      "\r\n    - Invoice Date\r\n    - ILS STW Date\r\n    - Actual STW Date\r\n");            
                return false;
            }

            if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateFrom_txt_InvoiceDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceDateTo_txt_InvoiceDateTo_textbox").value == "")) {
                alert("Invalid Invoice Date.");
                return false;
            }

            if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_IlsStwDateFrom_txt_IlsStwDateFrom_textbox").value != "" && 
                document.getElementById("ctl00_ContentPlaceHolder1_txt_IlsStwDateTo_txt_IlsStwDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_IlsStwDateFrom_txt_IlsStwDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_IlsStwDateTo_txt_IlsStwDateTo_textbox").value != "")) {
                alert("Invalid ILS STW Date.");
                return false;
            }

            if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_ActualStwDateFrom_txt_ActualStwDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_ActualStwDateTo_txt_ActualStwDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_ActualStwDateFrom_txt_ActualStwDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_ActualStwDateTo_txt_ActualStwDateTo_textbox").value != "")) {
                alert("Invalid Actual STW Date");
                return false;
            }
            if (document.getElementById("ctl00_ContentPlaceHolder1_rad_FiscalPeriod").checked &&
            document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodFrom").value > document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodTo").value) {
                alert("Invalid fiscal period.");
                return false;
            }
            
            if (GetCheckBoxSelectedCount("ckb_Office") == 0) 
            {
                alert("Please select one of the office.");
                return false;
            }

            if (GetCheckBoxSelectedCount("ckb_ProdTeam") == 0) {
                alert("Please select one of the product team.");
                return false;
            }

            if (GetCheckBoxSelectedCount('cbl_Customer') == 0)
            {
                alert("Please select one of the customers.");
                return false;
            }
            
            if (GetCheckBoxSelectedCount('cbl_TradingAgency') == 0)
            {
                alert("Please select one of the trading agencies");
                return false;
            }
            
            return true;
     }     
     
</script>
<asp:UpdatePanel ID="updatePanel3" runat="server">
    <ContentTemplate >
        <table>
            <tr>
                <td style="width:115px;">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:UpdatePanel ID="updatePanel2" runat="server">
                        <ContentTemplate >            
                        <table>
                            <tr>
                                <td class="FieldLabel4" style="width:150px;">Invoice Date</td>
                                <td><asp:RadioButton ID="rad_InvoiceDate" runat="server" GroupName="rad_SelectCriteria" AutoPostBack="true" Checked="true" Text="By Range : &nbsp;" OnCheckedChanged="rad_InvoiceDate_CheckedChanged" /></td>
                                <td><cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />&nbsp;To&nbsp;&nbsp;<cc2:SmartCalendar 
                                    id="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" /></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td><asp:RadioButton ID="rad_FiscalPeriod" runat="server" GroupName="rad_SelectCriteria" AutoPostBack="true" Text="By Period : " OnCheckedChanged="rad_FiscalPeriod_CheckedChanged" /></td>
                                <td>
                                    &nbsp;Year&nbsp;&nbsp;<asp:DropDownList  ID="ddl_Year" runat="server" SkinId="SmallDDL" Enabled="false" />&nbsp;&nbsp;&nbsp;
                                    Period From&nbsp;&nbsp;<asp:DropDownList id="ddl_PeriodFrom" runat="server"  width="30px"
                                        SkinID="SmallDDL" Enabled="false" 
                                        onselectedindexchanged="ddl_PeriodFrom_SelectedIndexChanged" AutoPostBack="True">
                                    <asp:ListItem Text="1" Value="1" />
                                    <asp:ListItem Text="2" Value="2" />
                                    <asp:ListItem Text="3" Value="3" />
                                    <asp:ListItem Text="4" Value="4" />
                                    <asp:ListItem Text="5" Value="5" />
                                    <asp:ListItem Text="6" Value="6" />
                                    <asp:ListItem Text="7" Value="7" />
                                    <asp:ListItem Text="8" Value="8" />
                                    <asp:ListItem Text="9" Value="9" />
                                    <asp:ListItem Text="10" Value="10" />
                                    <asp:ListItem Text="11" Value="11" />
                                    <asp:ListItem Text="12" Value="12" />
                                    </asp:DropDownList>  &nbsp;To&nbsp;&nbsp;
                                    <asp:DropDownList ID="ddl_PeriodTo" runat="server" SkinID="SmallDDL" Enabled="false" width="30px">
                                            <asp:ListItem Text="1" Value="1" />
                                            <asp:ListItem Text="2" Value="2" />
                                            <asp:ListItem Text="3" Value="3" />
                                            <asp:ListItem Text="4" Value="4" />
                                            <asp:ListItem Text="5" Value="5" />
                                            <asp:ListItem Text="6" Value="6" />
                                            <asp:ListItem Text="7" Value="7" />
                                            <asp:ListItem Text="8" Value="8" />
                                            <asp:ListItem Text="9" Value="9" />
                                            <asp:ListItem Text="10" Value="10" />
                                            <asp:ListItem Text="11" Value="11" />
                                            <asp:ListItem Text="12" Value="12" />
                                    </asp:DropDownList> 
                                </td>
                            </tr>
                        </table>
                  </ContentTemplate>
                  <Triggers >
                     <asp:AsyncPostBackTrigger ControlID="rad_InvoiceDate" 
                        EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="rad_FiscalPeriod" 
                        EventName="CheckedChanged" />
                  </Triggers>
                </asp:UpdatePanel> 
                </td>
            </tr>
            <tr>
                <td class="FieldLabel4" >ILS Stock-To-Warehouse Date</td>
                <td><cc2:SmartCalendar ID="txt_IlsStwDateFrom" runat="server" FromDateControl="txt_IlsStwDateFrom"
                    ToDateControl="txt_IlsStwDateTo" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_IlsStwDateTo" runat ="server" 
                    FromDateControl="txt_IlsStwDateFrom" ToDateControl="txt_IlsStwDateTo" />
                </td>
            </tr>
<tr>
    <td class="FieldLabel4">Actual Stock-To-Warehouse Date</td>
    <td><cc2:SmartCalendar ID="txt_ActualStwDateFrom" runat="server" FromDateControl="txt_ActualStwDateFrom"
        ToDateControl="txt_ActualStwDateTo" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_ActualStwDateTo" runat ="server" 
        FromDateControl="txt_ActualStwDateFrom" ToDateControl="txt_ActualStwDateTo" /></td>
</tr>
<tr style='display:none;'>
    <td class="FieldLabel4">Base Currency</td>
    <td><cc2:SmartDropDownList ID="ddl_BaseCurrency" runat="server" /></td>
</tr>
<tr>
    <td colspan="3">
        <uc2:UclOfficeProductTeamSelection ID="uclOfficeProductTeamSelection" runat="server" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Country of Origin</td>
    <td><cc2:SmartDropDownList ID="ddl_CountryOfOrigin" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Shipment Port</td>
    <td><cc2:SmartDropDownList ID="ddl_ShipmentPort" runat ="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Customer</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="350" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>    
     <td>
        <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Trading Agency</td>
    <td class="CellWithBorder"><asp:CheckBoxList ID="cbl_TradingAgency" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="350" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>
    <td>
        <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_TradingAgency'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_TradingAgency'); return false;" />
    </td>
</tr>
<tr>
<td>&nbsp;</td>
</tr>
<tr style='display:none;'>
    <td class="FieldLabel4">Sorting By</td>
    <td>
        <uso1:UclSortingOrder ID="ucl_SortingOrder1" runat="server" />
    </td>
</tr>
<tr>
<td colspan="2">
</td>
</tr>
</table>

</ContentTemplate>
</asp:UpdatePanel>
<asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />

</asp:Content>
