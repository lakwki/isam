<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="CustomerReceiptReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.CustomerReceiptReport" Title="Customer Receipt Report" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_reports_customer_receipt.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Customer Receipt Report</asp:Panel>     
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript">
     function copyInvoiceNo()
     {        
        //document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value.toUpperCase();
        //document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value;
     }  
     
     function isFormValid()
     {
     
        if (//document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value == "" && 
            //document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value == "" &&
            //document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateFrom_txt_textbox").value == "" &&
            //document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceUploadDateTo_txt_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_ReceiptDateFrom_txt_ReceiptDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_ReceiptDateTo_txt_ReceiptDateTo_textbox").value == "" &&                        
            document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodFrom").value == "-1" &&
            document.getElementById("ctl00_ContentPlaceHolder1_ddl_PeriodTo").value == "-1" &&
            document.getElementById("ctl00_ContentPlaceHolder1_ddl_Year").value == "-1")
            {
                alert("Please enter search criteria on one of below search criteria.\r\n- Invoice No.\r\n" +
                "- Invoice Date\r\n- Invoice Application Date\r\n- Fiscal Period");            
                return false;
            }
            //if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value != ""  &&
            //    (!isInvoiceNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value) ||
            //!isInvoiceNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value)))
            //{
            //alert("Invalid invoice no.");
            //return false;
            //}
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
            
//            if (GetCheckBoxSelectedCount('cbl_ShipmentMethod') == 0)
//            {
//                alert("Please select one of the shipment method");
//                return false;
//            }

            return true;
     }

     function ddl_Office_OnChange(obj) {
         var uclPrefix = "<%= uclProductTeam.ClientID %>";
         if (obj[obj.selectedIndex].text.indexOf("+") > 0) {
             clearSmartSelection(null, uclPrefix, null);
             disableSmartSelection(uclPrefix);
         }
         else {
             enableSmartSelection(uclPrefix);
         }
     }
     

</script>
<table >
<tr>
<td colspan="3">&nbsp;</td>
</tr>
<!--
<tr>
    <td>Invoice No</td>
    <td><asp:TextBox ID="txt_InvoiceNoFrom" runat="server" onblur="copyInvoiceNo();" />&nbsp;To&nbsp;<asp:TextBox ID="txt_InvoiceNoTo" runat="server" /></td>
</tr>
-->
<tr>
    <td class="FieldLabel4" style="width:150px;">Version</td>
    <td>
        <asp:RadioButtonList ID="rad_Version" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Text="Epicor" Value="EPICOR" Selected="True" />
            <asp:ListItem Text="Sun" Value ="SUN" />
        </asp:RadioButtonList>
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Customer Receipt Date</td>
    <td>
        <asp:RadioButtonList ID="radDateType" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Selected="True" Text="Both" Value="-1"></asp:ListItem>
            <asp:ListItem Text="Sales" Value="1"></asp:ListItem>
            <asp:ListItem Text="Sales Commission" Value="2"></asp:ListItem>
        </asp:RadioButtonList>
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Receipt Date</td>
    <td><cc2:SmartCalendar ID="txt_ReceiptDateFrom" runat="server" FromDateControl="txt_ReceiptDateFrom" ToDateControl="txt_ReceiptDateTo" />&nbsp;To&nbsp;<cc2:SmartCalendar 
        id="txt_ReceiptDateTo" runat="server" FromDateControl="txt_ReceiptDateFrom" ToDateControl="txt_ReceiptDateTo" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Fiscal Period</td>
    <td>
        Year&nbsp;&nbsp;<asp:DropDownList  ID="ddl_Year" runat="server" SkinId="SmallDDL" />&nbsp;&nbsp;&nbsp;
        Period From&nbsp;<asp:DropDownList id="ddl_PeriodFrom" runat="server" SkinID="SmallDDL">
        <asp:ListItem Text="--All--" Value="-1" Selected="True" />
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
        </asp:DropDownList>  &nbsp;To&nbsp;
        <asp:DropDownList ID="ddl_PeriodTo" runat="server" SkinID="SmallDDL">
                <asp:ListItem Text="--All--" Value="-1" Selected="True" />
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
<tr>
    <td class="FieldLabel4">Customer</td>
    <td style="border: 1px solid #C0C0C0">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>
    <td>
        <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Base Currency</td>
    <td><cc2:SmartDropDownList ID="ddl_BaseCurrency" runat ="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Currency</td>
    <td><cc2:SmartDropDownList ID="ddl_Currency" runat ="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4" style="width:150px;">Receipt Reference No. </td>
    <td><cc2:SmartDropDownList ID="ddl_ReceiptReferenceNo" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Order Type</td>
    <td>
        <asp:DropDownList ID="ddl_SampleOrder" runat="server" SkinId="LargeDDL">
            <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS/STUDIO SAMPLE ORDER)" Value="-1" />
            <asp:ListItem Text="MAINLINE ORDER" Selected="True" Value="0" />
            <asp:ListItem Text="MOCK SHOP/PRESS/STUDIO SAMPLE ORDER" Value="1" />
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td colspan="2">
        <asp:UpdatePanel ID="updatePanel3" runat="server" >
            <ContentTemplate>                                
        <table style="width:100%;" cellpadding="0" >
            <tr>
                <td class="FieldLabel4" style="width:150px;">Office Group</td>
                <td><cc2:SmartDropDownList  ID="ddl_Office" OnSelectedIndexChanged="ddl_Office_SelectedIndexChanged" onChange="ddl_Office_OnChange(this);" runat="server" AutoPostBack="True" /></td>
            </tr>   
            <tr>
                <td class="FieldLabel4">Handling Office</td>
                <td><asp:DropDownList ID="ddl_HandlingOffice" runat="server" >
                    <asp:ListItem Text="-- ALL --" Value="-1" />
                    <asp:ListItem Text="DG" Value="17" />
                    <asp:ListItem Text="HK" Value="1" />
                    <asp:ListItem Text="SH" Value="2" />
                    <asp:ListItem Text="VN" Value="16" />
                </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel4">Department</td>
                <td><cc2:SmartDropDownList ID="ddl_Department" runat="server" /></td>
            </tr>
        </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddl_Office" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </td>
</tr>

<tr>
    <td class="FieldLabel4">Season</td>
    <td><cc2:SmartDropDownList ID="ddl_Season" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Product Team</td>
<!--
<td><input type='text' id'txt_ProductTeam' value=''/></td>    
-->

    <td><uc1:uclsmartselection id="uclProductTeam" runat="server"></uc1:uclsmartselection></td>

</tr>
<tr>
    <td class="FieldLabel4">Supplier</td>
    <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Trading Agency</td>
    <td style="border: 1px solid #C0C0C0" colspan="2">
        <asp:CheckBoxList ID="cbl_TradingAgency" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>
    <td>
        <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_TradingAgency'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_TradingAgency'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Purchase Term</td>
    <td><cc2:smartdropdownlist ID="ddl_PurchaseTerm" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Payment Term</td>
    <td>
        <cc2:SmartDropDownList  ID="ddl_PaymentTerm" runat="server" />
    </td>
</tr>
<!--
<tr>
    <td>Shipment Method</td>
    <td style="border: 1px solid #C0C0C0">
    <asp:CheckBoxList ID="cbl_ShipmentMethod" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table"  >
        </asp:CheckBoxList>
    </td>
</tr>
-->
<tr>
<td colspan="2">&nbsp;</td>
</tr>
<tr>
<td colspan="2"><asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
</td>
</tr>
</table>
</asp:Content>
