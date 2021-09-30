<%@ Page Title="ISAM - Supplier Order Status Enquiry Report" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="SupplierOrderStatusReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.SupplierOrderStatusReport" %>
 <%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
    <%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel runat="server" SkinID="sectionHeader_Report">Supplier Order Status Enquiry</asp:Panel>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function isFormValid() {

        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value == "" &&
             document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value == "" && 
             document.getElementById("ctl00_ContentPlaceHolder1_uclSupplier_txtName").value == "") {
            alert("Please enter search criteria on one of below search criteria.\r\n" +
                "- Customer At-Warehouse Date\r\n- Supplier");
            return false;
        }
        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateFrom_txt_AtWHDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_AtWHDateTo_txt_AtWHDateTo_textbox").value == "")) {
            alert("Invalid At-Warehouse Date.");
            return false;
        }
    }
    
</script>
<table>
<!--
    <tr>
        <td colspan="2" class="tableHeader">Supplier Order Status Enquiry</td>
    </tr>
-->    
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel4" style="width:150px;">Customer At Warehouse Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_AtWHDateFrom" runat="server" FromDateControl="txt_AtWHDateFrom" 
                ToDateControl="txt_AtWHDateTo" /> to <cc2:SmartCalendar ID="txt_AtWHDateTo" runat="server" FromDateControl="txt_AtWHDateFrom"
                ToDateControl="txt_AtWHDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel4">Office</td>
        <td><cc2:SmartDropDownList ID="ddl_Office" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Product Team</td>
        <td><uc1:UclSmartSelection ID="uclProductTeam" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Supplier</td>
        <td><uc1:UclSmartSelection ID="uclSupplier" runat="server" /></td>
    </tr>
    <tr>
        <td class="FieldLabel4">Payment Term</td>
        <td><cc2:SmartDropDownList ID="ddl_PaymentTerm" runat="server" /></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" OnClientClick="return isFormValid();" />
            <asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  OnClientClick="return isFormValid();" />
        </td>
    </tr>
</table>
</asp:Content>
