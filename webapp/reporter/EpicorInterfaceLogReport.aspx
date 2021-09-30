<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="EpicorInterfaceLogReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.EpicorInterfaceLogReport" Title="Epicor Interface Log" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<script type="text/javascript">
    function isFormValid() {

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_InterfaceDateFrom_txt_InterfaceDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InterfaceDateTo_txt_InterfaceDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_InterfaceDateFrom_txt_InterfaceDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_InterfaceDateTo_txt_InterfaceDateTo_textbox").value != "")) {
            alert("Invalid Interface Date.");
            return false;
        }


        return true;
    }
</script>
<asp:Panel runat="server" SkinID="sectionHeader_Report">Epicor Interface Log</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table>
<tr>
<td>&nbsp;</td>
</tr>


<tr>
    <td class="FieldLabel4">Interface Date</td>
    <td><cc2:SmartCalendar ID="txt_InterfaceDateFrom" runat="server" FromDateControl="txt_InterfaceDateFrom"
        ToDateControl="txt_InterfaceDateTo"  />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_InterfaceDateTo" runat ="server" 
        FromDateControl="txt_InterfaceDateFrom" ToDateControl="txt_InterfaceDateTo"  />
    </td>
</tr>

<tr>
    <td class="FieldLabel4_T" valign="top">Office</td>
    <td class="CellWithBorder" style="text-align:right;">
        <asp:CheckBoxList ID="cbl_Office" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" 
                OnSelectedIndexChanged="cbl_Office_SelectedIndexChanged" AutoPostBack="true"
                RepeatLayout="Table"  RepeatColumns="4" style="text-align:left;" />
        <asp:LinkButton ID="lnk_SelectAllOffice" runat="server" Text="Select All" OnClientClick="CheckAll('cbl_Office');" autopostback="true"/>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="Lnk_DeselectAllOffice" runat="server" Text="Clear" OnClientClick="UncheckAll('cbl_Office');" autopostback="true"/>
        &nbsp;
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
</asp:Content>
