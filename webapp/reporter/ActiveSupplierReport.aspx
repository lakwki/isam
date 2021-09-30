<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ActiveSupplierReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.ActiveSupplierReport" Title="ISAM - Active Supplier Report" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<script type="text/javascript" >

function isFormValid()
{    
    if (document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateFrom_txt_DeliveryDateFrom_textbox").value == "" ||
        document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateTo_txt_DeliveryDateTo_textbox").value == "")
     {
        alert("Please enter the range of delivery date.");
        return false;        
     }
     
     if (document.getElementById("ctl00_ContentPlaceHolder1_txt_NoOfDelivery").value == "")
     {
        alert("Please enter min. no. of delivery.");
        return false;
     }
     else 
     {
        delNo = document.getElementById("ctl00_ContentPlaceHolder1_txt_NoOfDelivery").value;
        if (isNaN(delNo) || parseInt(delNo) <= 0)
        {
            alert("Invalid delivery no.");
            return false;
        }            
     }
     
     return true;
}

function dateDiff()
{    
     start = document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateFrom_txt_DeliveryDateFrom_textbox").value;     
     end = document.getElementById("ctl00_ContentPlaceHolder1_txt_DeliveryDateTo_txt_DeliveryDateTo_textbox").value;
        one_day=1000*60*60*24;
        

    diff =  Math.ceil((Date.parseLocale(end, 'dd/MM/yyyy')- Date.parseLocale(start, 'dd/MM/yyyy'))/ one_day);
    
     lbl_diff = document.getElementById("lbl_dayDiff");
     lbl_dayDiff.innerHTML = diff.toString();
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
<!--
<img src="../images/banner_reports_active_supplier_report.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel runat="server" SkinID="sectionHeader_Report">Active Supplier Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:Panel ID="pnl" runat="server" DefaultButton="btn_Submit">
<table>
<tr>
<td>&nbsp;</td>
</tr>
<tr>
    <td class="FieldLabel4">Delivery Date</td>
    <td><cc2:SmartCalendar ID="txt_DeliveryDateFrom" runat="server" FromDateControl="txt_DeliveryDateFrom"
        ToDateControl="txt_DeliveryDateTo" />&nbsp;to&nbsp;
        <cc2:SmartCalendar ID="txt_DeliveryDateTo" runat="server" FromDateControl="txt_DeliveryDateFrom" 
        ToDateControl="txt_DeliveryDateTo" />&nbsp;&nbsp;
        (<span id="lbl_dayDiff">180</span>&nbsp;days)
        </td>
</tr>
<tr>
    <td class="FieldLabel4">Less Than</td>
    <td><asp:TextBox ID="txt_NoOfDelivery" runat ="server" Text="5" SkinID="SmallTextBox" />&nbsp;Deliveries</td>
</tr>
<tr>
    <td class="FieldLabel4" style="width:150px;">Office Group</td>
    <td><cc2:SmartDropDownList  ID="ddl_Office" onchange="ddl_Office_OnChange(this);" runat="server"  /></td>
</tr>
<tr>
    <td class="FieldLabel4">Product Team</td>
    <td><uc1:uclsmartselection id="uclProductTeam" runat="server"></uc1:uclsmartselection></td>
</tr>
<tr>
    <td class="FieldLabel4">Supplier</td>
    <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4">Order Type</td>
    <td>
          <asp:DropDownList ID="ddl_SampleOrder" runat="server" SkinId="LargeDDL">
            <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS/STUDIO SAMPLE ORDER)" Value="-1"   />
            <asp:ListItem Text="MAINLINE ORDER" Value="0" Selected="True" />
            <asp:ListItem Text="MOCK SHOP/PRESS/STUDIO SAMPLE ORDER" Value="1" />
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td class="FieldLabel4_T" style="vertical-align:top ;">Customer</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4">
        </asp:CheckBoxList>
    </td>    
    <td>
        <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_Customer'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_Customer'); return false;" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4_T" style="vertical-align:top;">Workflow Status</td>
    <td class="CellWithBorder">
        <asp:CheckBoxList ID="cbl_wfs" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4">
            <asp:ListItem Text="DRAFT" Value="1" />
            <asp:ListItem Text="PENDING FOR APPROVAL" Value="2" Selected="True" />
            <asp:ListItem Text="PENDING FOR CANCEL" Value="3" />
            <asp:ListItem Text="AMENDED" Value="4" />
            <asp:ListItem Text="REJECTED" Value="5" />
            <asp:ListItem Text="APPROVED" Value="6" Selected="True"  />
            <asp:ListItem Text="PO GENERATED" Value="7" Selected="True"  />
            <asp:ListItem Text="INVOICED" Value="8" />
            <asp:ListItem Text="CANCELLED" Value="9" />
        </asp:CheckBoxList>
    </td>
    <td>
        <asp:ImageButton ID="btn_Clear2" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_wfs'); return false;" />&nbsp;
        <asp:ImageButton ID="btn_All2" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_wfs'); return false;" />
    </td>
</tr>
<tr>
<td>&nbsp;</td>
</tr>
<tr>
<td colspan="2">
</td>
</tr>
</table>

<asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
<asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
</asp:Panel>
</asp:Content>
