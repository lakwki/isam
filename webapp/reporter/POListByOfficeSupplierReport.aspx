<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="POListByOfficeSupplierReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.POListByOfficeSupplierReport" Theme="DefaultTheme" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Src="../webservices/UclSmartSelection.ascx" TagName="UclSmartSelection" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        function refreshCheckBox(obj) {
            var checkBox, label, ckb_All;
            var options = obj.getElementsByTagName("SPAN");
            var allChecked = true;
            for (var i = 0; i < options.length; i++) {
                checkBox = options[i].getElementsByTagName("INPUT");
                label = options[i].getElementsByTagName("LABEL");
                if (label[0].innerText == "ALL")
                    ckb_All = checkBox[0];
                else
                    allChecked &= checkBox[0].checked;
            }
            if (ckb_All.checked && !allChecked)
                ckb_All.checked = false;
        }

        function clickAll(obj) {
            for (var o = obj; o != null; o = o.parentNode)
                if (o.id != undefined) {
                    var cblName = o.id.substring(o.id.indexOf("cbl_"), o.id.lastIndexOf("_"));
                    if (obj.checked)
                        CheckAll(cblName);
                    else
                        UncheckAll(cblName);
                    break;
                }
        }
    </script>

    <asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">P/O List By Office/Supplier Report</asp:Panel>
    <br />
    <table>
        <tr>
            <td class="FieldLabel4" style="width: 146px">Year</td>
            <td>
                <asp:DropDownList ID="ddl_Year" runat="server" SkinID="SmallDDL" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width: 146px">Period</td>
            <td>
                <asp:DropDownList ID="ddl_Period" runat="server" SkinID="SmallDDL" AutoPostBack="True">
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
            <td class="FieldLabel4" style="width: 146px">Office</td>
            <td style="width: 319px">
                <cc2:SmartDropDownList ID="ddlOffice" runat="server"></cc2:SmartDropDownList></td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width: 146px">Supplier</td>
            <td style="width: 350px">
                <uc1:UclSmartSelection ID="txt_Supplier" runat="server" width="300px" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width: 146px">Currency</td>
            <td>
                <cc2:SmartDropDownList ID="ddl_BaseCurrency" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel4" style="width: 146px;vertical-align:top ; background-position : top;">Payment Status</td>
            <td>
                <asp:CheckBoxList ID="cbl_ShipmentStatus" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4" class="CellWithBorder" />
            </td>
            <td>
                <asp:ImageButton ID="btn_Clear1" runat="server" ImageUrl="~/images/icon_clear_all.png" ToolTip="clear all" OnClientClick="UncheckAll('cbl_ShipmentStatus'); return false;" />&nbsp;
                <asp:ImageButton ID="btn_All1" runat="server" ImageUrl="~/images/icon_select_all.png" ToolTip="select all" OnClientClick="CheckAll('cbl_ShipmentStatus'); return false;" />
            </td>
        </tr>
        <%--    <tr>
            <td style="width: 146px">
                <asp:DropDownList runat="server" ID="ddlStatus" />
            </td>
        </tr>--%>
    </table>
    <br />
    <asp:Button ID="btn_Submit" runat="server" Text="Export" SkinID="LButton" OnClick="btn_Submit_Click" Visible="true" />
</asp:Content>
