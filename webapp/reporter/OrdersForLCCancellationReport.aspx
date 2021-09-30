<%@ Page Title="" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="OrdersForLCCancellationReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.OrdersForLCCancellationReport" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <!--
<img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Orders for L/C Cancellation Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">

        function isFormValid() {
            if (document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateFrom_txt_CustomerAtWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateTo_txt_CustomerAtWHDateTo_textbox").value == "") {
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

    </script>
    <table>
        <tr>
            <!--
        <td colspan="2" class="tableHeader">Outstanding LC Report</td>
-->
            <td>&nbsp;</td>
        </tr>

        <tr>
            <td class="FieldLabel4">Office</td>
            <td>
                <cc2:SmartDropDownList ID="ddl_Office" runat="server" Width="70px" AutoPostBack="true" /></td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width: 160px;">Customer At-Warehouse Date</td>
            <td>
                <cc2:SmartCalendar ID="txt_CustomerAtWHDateFrom" runat="server" FromDateControl="txt_CustomerAtWHDateFrom"
                    ToDateControl="txt_CustomerAtWHDateTo" />
                To
                <cc2:SmartCalendar ID="txt_CustomerAtWHDateTo" runat="server"
                    FromDateControl="txt_CustomerAtWHDateFrom" ToDateControl="txt_CustomerAtWHDateTo" />
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btn_Export" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" OnClientClick="return isFormValid();" />
            </td>
        </tr>
    </table>
</asp:Content>
