<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master"  Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="TradingAFReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.TradingAFReport" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<%@ Register Src="~/usercontrol/UclOfficeSelection.ascx" TagName="UclOfficeSelection" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Trading A/F Report</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function ChangePanelStatus(img) {
        tbl = document.getElementById(img.id.replace("img_", "tbl_"));
        if (img.alt == "" || img.alt == "SHOW") {
            img.src = "../images/summy.jpg";
            img.title = "Hide options";
            img.alt = "HIDE";
            tbl.style.display = "block";
        }
        else {
            img.src = "../images/summy_reverse.jpg";
            img.title = "Show options";
            img.alt = "SHOW";
            tbl.style.display = "none";
        }
    }

    function copyValue(obj) {
        var target = document.getElementById(obj.id.replace("From", "To"));
        if (target.value.trim() == "")
            target.value = obj.value.trim();
    }

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
<asp:UpdatePanel ID="updatePanel3" runat="server">
<ContentTemplate >
<table>
<tr>
<td colspan="2">&nbsp;</td>
</tr>
<tr>
    <td colspan="2">
        <uc2:UclOfficeSelection ID="uclOfficeSelection" runat="server" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Customer At-Warehouse Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_CustomerAtWHDateFrom" runat="server" FromDateControl="txt_CustomerAtWHDateFrom" ToDateControl="txt_CustomerAtWHDateTo" /> To 
        <cc2:SmartCalendar ID="txt_CustomerAtWHDateTo" runat="server" FromDateControl="txt_CustomerAtWHDateFrom" ToDateControl="txt_CustomerAtWHDateTo" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Invoice Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_InvoiceDateFrom" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" /> To 
        <cc2:SmartCalendar ID="txt_InvoiceDateTo" runat="server" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" />        
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Supplier</td>
    <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
</tr>
<tr id="tr_CountryOfOrigin">
    <td class="FieldLabel4_T" valign="top">Country of Origin</td>
    <td>
        <asp:Panel ID="pnl_CO" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_CO" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;Country of Origin</div>
        </asp:Panel>
        <table id="tbl_CO" style="display:none;">
            <tr>
                <td><asp:CheckBoxList ID="cbl_CO" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder"/></td>
                <td><asp:ImageButton ID="lnkCO" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_CO'); return false;" /></td>
            </tr>
        </table>
    </td>
</tr>
<tr id="tr_LoadingPort">
    <td class="FieldLabel4_T" valign="top">Loading Port</td>
    <td>
        <asp:Panel ID="pnl_LoadingPort" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_LoadingPort" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;Loading Port</div>
        </asp:Panel>
        <table>
          <tr id="tbl_LoadingPort" style="display:none;">
            <td><asp:CheckBoxList ID="cbl_LoadingPort" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder"/></td>
            <td><asp:ImageButton ID="lnkLoadingPort" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_LoadingPort'); return false;" /></td>
          </tr>
        </table>
    </td>
</tr>
<tr id="tr_ShipmentMethod">
    <td class="FieldLabel4_T" valign="top">Shipment Method</td>
     <td>
        <asp:Panel ID="pnl_ShipmentMethod" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_ShipmentMethod" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;Shipment Method</div>
        </asp:Panel>
        <table id="tbl_ShipmentMethod" style="display:none;">
          <tr>
            <td><asp:CheckBoxList ID="cbl_ShipmentMethod" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder" onclick="refreshCheckBox(this)"/></td>
            <td><asp:ImageButton ID="lnkShipmentMethod" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_ShipmentMethod'); return false;" /></td>
          </tr>
        </table>
    </td>
</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
  <asp:Button ID="btn_Submit" runat="server" Text="Print Preview" SkinID="LButton"  OnClick="btn_Submit_Click" />
  <asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click" />
  <asp:Label runat="server" ID="lbltest" style="color:White;"/>
  <asp:Label runat="server" ID="Label1" />
</asp:Content>
