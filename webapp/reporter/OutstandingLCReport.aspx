<%@ Page Title="" Language="C#" Theme="DefaultTheme"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="OutstandingLCReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.OutstandingLCReport"  %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Outstanding L/C Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    
    function isFormValid() {
        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateFrom_txt_CustomerAtWHDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateTo_txt_CustomerAtWHDateTo_textbox").value == "" ) {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Customer At-Warehouse Date");
            return false;
        }


        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateFrom_txt_CustomerAtWHDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateTo_txt_CustomerAtWHDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateFrom_txt_CustomerAtWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateTo_txt_CustomerAtWHDateTo_textbox").value != "")) {
            alert("Invalid Supplier At Warehouse Date");
            return false;
        }
        return true;
    }

    function ddl_OfficeGroup_OnChange(obj) {
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
<table>
    <tr>
<!--
        <td colspan="2" class="tableHeader">Outstanding LC Report</td>
--> 
        <td>&nbsp;</td>   
    </tr>

    <tr>
        <td class="FieldLabel4">Office Group</td>
        <td><cc2:SmartDropDownList ID="ddl_OfficeGroup" runat="server" width="70px" onchange="ddl_OfficeGroup_OnChange(this);"  OnSelectedIndexChanged="ddl_OfficeGroup_SelectedIndexChanged"  AutoPostBack="true"/></td>
    </tr>
    <tr id="tr_HandlingOffice" runat="server" style="display:none;">
        <td class="FieldLabel4">Handling Office</td>
        <td><cc2:SmartDropDownList  ID="ddl_HandlingOffice" runat="server" width="70px" AutoPostBack="false" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Product Team</td>
        <td><uc1:UclSmartSelection ID="uclProductTeam" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Supplier</td>
        <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /> </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Country of Origin</td>
        <td><cc2:SmartDropDownList ID="ddl_CO" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Destination</td>
        <td><cc2:SmartDropDownList ID="ddl_Destination" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Purchase Term</td>
        <td><cc2:SmartDropDownList ID="ddl_PurchaseTerm" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:160px;">Customer At-Warehouse Date</td>
        <td><cc2:SmartCalendar ID="txt_CustomerAtWHDateFrom" runat="server" FromDateControl="txt_CustomerAtWHDateFrom"
                ToDateControl="txt_CustomerAtWHDateTo" /> To <cc2:SmartCalendar ID="txt_CustomerAtWHDateTo" runat="server" 
                FromDateControl="txt_CustomerAtWHDateFrom" ToDateControl="txt_CustomerAtWHDateTo" /></td>
    </tr>
    <tr>
    <td class="FieldLabel4_T" valign="top">Sort Order</td>
    <td class="CellWithBorder">
    <asp:RadioButtonList ID="rad_SortField" runat="server"  RepeatDirection="Horizontal" Width="400">
            <asp:ListItem Text="Office" Value="Office" Selected="True" />
            <asp:ListItem Text="Product Team" Value="ProductTeam" />
            <asp:ListItem Text="Contract No" Value="ContractNo" />
            <asp:ListItem Text="Delivery No" Value="DeliveryNo" />
        </asp:RadioButtonList> <br />
        <asp:RadioButtonList ID="rad_SortOrder" runat="server"  RepeatDirection="Horizontal">
            <asp:ListItem Text="Ascending" Value="asc" Selected="True" />
            <asp:ListItem Text="Descending" Value="desc" />
        </asp:RadioButtonList>
    </td>
</tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
            <asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();"  />
        </td>
    </tr>
</table>
</asp:Content>
