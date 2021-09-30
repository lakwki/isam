<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme"  AutoEventWireup="true" CodeBehind="WeeklyShipmentReport.aspx.cs" Inherits="com.next.isam.webapp.reporter.WeeklyShipmentReport" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<%@ Register Src="~/usercontrol/UclSortingOrder.ascx" TagName="UclSortingOrder" TagPrefix="uso" %>
<%@ Register Src="~/usercontrol/UclOfficeProductTeamSelection.ascx" TagName="UclOfficeProductTeamSelection" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_reports_weekly_shipment.gif" runat="server" id="imgHeaderText" />
-->
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Report">Weekly Shipment Report</asp:Panel>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
    function isFormValid() {

        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateFrom_txt_StockToWHDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateTo_txt_StockToWHDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateFrom_txt_CustomerAtWHDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_CustomerAtWHDateTo_txt_CustomerAtWHDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_BookingDateFrom_txt_BookingDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_BookingDateTo_txt_BookingDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_BookedInWHDateFrom_txt_BookedInWHDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_BookedInWHDateTo_txt_BookedInWHDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_SailingDateFrom_txt_SailingDateFrom_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_SailingDateTo_txt_SailingDateTo_textbox").value == "" &&
            document.getElementById("ctl00_ContentPlaceHolder1_txt_VoyageNo").value == "") {
            alert("Please enter search criteria on one of below search criteria.\r\n- Invoice No.\r\n" +
                "- Stock to Warehouse Date\r\n- Customer At-Warehouse Date\r\n- Booking Date\r\n- Booked in Warehouse Date\r\n" +
                "- Sailing Date\r\n - Voyage No");
            return false;
        }
        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateFrom_txt_StockToWHDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateTo_txt_StockToWHDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateFrom_txt_StockToWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_StockToWHDateTo_txt_StockToWHDateTo_textbox").value != "")) {
            alert("Invalid Stock to Warehouse Date.");
            return false;
        }

        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_BookingDateFrom_txt_BookingDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_BookingDateTo_txt_BookingDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_BookingDateFrom_txt_BookingDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_BookingDateTo_txt_BookingDateTo_textbox").value == "")) {
            alert("Invalid Booking Date.");
            return false;
        }
        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_BookedInWHDateFrom_txt_BookedInWHDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_BookedInWHDateTo_txt_BookedInWHDateTo_textbox").value != "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_BookedInWHDateFrom_txt_BookedInWHDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_BookedInWHDateTo_txt_BookedInWHDateTo_textbox").value == "")) {
            alert("Invalid Booked in Warehouse Date.");
            return false;
        }
        if ((document.getElementById("ctl00_ContentPlaceHolder1_txt_SailingDateFrom_txt_SailingDateFrom_textbox").value != "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_SailingDateTo_txt_SailingDateTo_textbox").value == "") ||
                (document.getElementById("ctl00_ContentPlaceHolder1_txt_SailingDateFrom_txt_SailingDateFrom_textbox").value == "" &&
                document.getElementById("ctl00_ContentPlaceHolder1_txt_SailingDateTo_txt_SailingDateTo_textbox").value != "")) {
            alert("Invalid Sailing Date.");
            return false;
        }
        if (GetCheckBoxSelectedCount('cbl_Customer') == 0) {
            alert("Please select one of the customers.");
            return false;
        }

        if (GetCheckBoxSelectedCount('cbl_ShipmentMethod') == 0) {
            alert("Please select one of the shipment method");
            return false;
        }

        return true;
    }

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
<td>&nbsp;</td>
</tr>
<tr>
    <td class="FieldLabel4" style="width:150px">Stock-to-Warehouse Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_StockToWHDateFrom" runat="server" FromDateControl="txt_StockToWHDateFrom" ToDateControl="txt_StockToWHDateTo" /> To 
        <cc2:SmartCalendar ID="txt_StockToWHDateTo" runat="server" FromDateControl="txt_StockToWHDateFrom" ToDateControl="txt_StockToWHDateTo" />
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
    <td class="FieldLabel4">Booking Date</td>
    <td>
        <cc2:SmartCalendar ID="txt_BookingDateFrom" runat="server" FromDateControl="txt_BookingDateFrom" ToDateControl="txt_BookingDateTo" /> To 
        <cc2:SmartCalendar ID="txt_BookingDateTo" runat="server" FromDateControl="txt_BookingDateFrom" ToDateControl="txt_BookingDateTo" />        
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Booked in Warehouse Date</td>
    <td><cc2:SmartCalendar ID="txt_BookedInWHDateFrom" runat="server" FromDateControl="txt_BookedInWHDateFrom" ToDateControl="txt_BookedInWHDateTo" /> To 
            <cc2:SmartCalendar ID="txt_BookedInWHDateTo" runat="server" FromDateControl="txt_BookedInWHDateFrom" ToDateControl="txt_BookedInWHDateTo" />
     </td>
</tr>
<tr>
    <td class="FieldLabel4">Departure Date</td>
    <td><cc2:SmartCalendar ID="txt_SailingDateFrom" runat="server" FromDateControl="txt_SailingDateFrom" ToDateControl="txt_SailingDateTo" /> To 
            <cc2:SmartCalendar ID="txt_SailingDateTo" runat="server" FromDateControl="txt_SailingDateFrom" ToDateControl="txt_SailingDateTo" />
    
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Voyage No.</td>
    <td><asp:TextBox ID="txt_VoyageNo" runat="server" /></td>
</tr>
<tr style="display:none;">
    <td>Office</td>
    <td><cc2:SmartDropDownList ID="ddl_Office" runat="server" /></td>
</tr>
<%--
<tr>
    <td>Product Team</td>
    <td><ucp:UclProductTeamSelection ID="uclProductTeam" runat="server" /></td>
</tr>
--%>
<tr>
    <td colspan="3">
        <uc2:UclOfficeProductTeamSelection ID="ucl_OfficeProdTeamSelection" runat="server" />
    </td>
</tr>
<tr>
    <td class="FieldLabel4">Handling Office</td>
    <td><asp:DropDownList ID="ddl_HandlingOffice" runat="server" >
        <asp:ListItem Text="-- ALL --" Value="-1" />
        <asp:ListItem Text="DG" Value="17" />
        <asp:ListItem Text="HK" Value="1" />
        <asp:ListItem Text="SH" Value ="2" />
        <asp:ListItem Text="VN" Value="16" />
    </asp:DropDownList></td>
</tr>
<tr>
    <td class="FieldLabel4">Supplier</td>
    <td><uc1:UclSmartSelection ID="txt_Supplier" runat="server" /></td>
</tr>
<tr>
    <td class="FieldLabel4_T" rowspan="4" valign="top">Order Type</td>
    <td>
        <asp:DropDownList ID="ddl_SZOrder" runat="server" SkinID="LargeDDL">
                <asp:ListItem Value="-1">ALL (NSL SZ ORDER + NON-NSL SZ ORDER)</asp:ListItem>
                <asp:ListItem Value="0">NON-NSL SZ ORDER</asp:ListItem>
                <asp:ListItem Value="1">NSL SZ ORDER</asp:ListItem>
            </asp:DropDownList>
    </td>
</tr>
<tr>
    <td>
            <asp:DropDownList ID="ddl_DualSource" runat="server" SkinID="LargeDDL">
                <asp:ListItem Value="-1">All (Dual Sourcing + Non-Dual Sourcing)</asp:ListItem>
                <asp:ListItem Value="0">Non-Dual Sourcing</asp:ListItem>
                <asp:ListItem Value="1">Dual Sourcing</asp:ListItem>
            </asp:DropDownList>
    </td>
</tr>
<tr>
    <td>
        <asp:DropDownList ID="ddl_LDPOrder" runat="server" SkinID="LargeDDL">
            <asp:ListItem Value="-1">ALL (LDP + NON-LDP ORDER)</asp:ListItem>
            <asp:ListItem value="1">LDP ORDER</asp:ListItem>
            <asp:ListItem Value="0">NON-LDP ORDER</asp:ListItem>
        </asp:DropDownList>
    </td>
</tr>
<tr>
    <td>
        <asp:DropDownList ID="ddl_SampleOrder" runat="server" SkinId="LargeDDL">
            <asp:ListItem Text="ALL (MAINLINE + MOCK SHOP/PRESS/STUDIO SAMPLE ORDER)" Value="-1" />
            <asp:ListItem Text="MAINLINE ORDER" Selected="True" Value="0" />
            <asp:ListItem Text="MOCK SHOP/PRESS/STUDIO SAMPLE ORDER" Value="1" />
        </asp:DropDownList>
    </td>
</tr>

<tr id="tr_OrderType_Trial" style="display:none;">
    <td class="FieldLabel4_T" valign="top">Order Type</td>
    <td>
        <asp:Panel ID="pnl_OrderType" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_OrderType" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;Order Type</div>
        </asp:Panel>
        <table id="tbl_OrderType">
            <tr>
                <td>
                <asp:CheckBoxList ID="cbl_SampleOrder" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder">
                    <asp:ListItem Value="-1" Text="ALL"/>
                    <asp:ListItem Value="0"  Text="MAINLINE ORDER" Selected="True"/>
                    <asp:ListItem Value="1"  Text="MOCK SHOP/PRESS SAMPLE ORDER"/>
                </asp:CheckBoxList>
                </td>
                <td><asp:ImageButton ID="lnk_SampleOrder" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_SampleOrder'); return false;" /></td>
            </tr>
            <tr>
                <td>
                <asp:CheckBoxList ID="cbl_SZOrder" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder">
                    <asp:ListItem Value="-1" Text="ALL" Selected="True"/>
                    <asp:ListItem Value="1"  Text="NSL SZ ORDER" Selected="True"/>
                    <asp:ListItem Value="0"  Text="NON-NSL SZ ORDER" Selected="True"/>
                </asp:CheckBoxList>
                </td>
                <td><asp:ImageButton ID="lnk_SZOrder" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_SZOrder'); return false;" /></td>
            </tr>
            <tr>
                <td>
                <asp:CheckBoxList ID="cbl_DualSourcingOrder" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder">
                    <asp:ListItem Value="-1" Text="ALL" Selected="True"/>
                    <asp:ListItem Value="1"  Text="DUAL SOURCING ORDER" Selected="True"/>
                    <asp:ListItem Value="0"  Text="NON-DUAL SOURCING ORDER" Selected="True"/>
                </asp:CheckBoxList>
                </td>
                <td><asp:ImageButton ID="lnk_DualSourcingOrder" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_DualSourcingOrder'); return false;" /></td>
            </tr>
            <tr>
                <td>
                <asp:CheckBoxList ID="cbl_LDPOrder" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder">
                    <asp:ListItem Value="-1" Text="ALL" Selected="True"/>
                    <asp:ListItem value="1"  Text="LDP ORDER" Selected="True"/>
                    <asp:ListItem value="0"  Text="NON-LDP ORDER" Selected="True"/>
                </asp:CheckBoxList>
                </td>
                <td><asp:ImageButton ID="lnk_LDPOrder" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_LDPOrder'); return false;" /></td>
            </tr>
            <!-- Trial Version 2
           <tr style="display:none;">
                <td>
                <asp:CheckBoxList ID="cbl_OrderType" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder">
                    <asp:ListItem Value="-1" Text="ALL"/>
                    <asp:ListItem Value="S1"  Text="NSL SZ" Selected="True"/>
                    <asp:ListItem Value="D1"  Text="DUAL SOURCING" Selected="True"/>
                    <asp:ListItem value="L1"  Text="LDP" Selected="True"/>
                    <asp:ListItem Value="M1"  Text="MOCK SHOP/PRESS SAMPLE"/>
                    <asp:ListItem Value="S2"  Text="NON-NSL SZ" Selected="True"/>
                    <asp:ListItem Value="D2"  Text="NON-DUAL SOURCING" Selected="True"/>
                    <asp:ListItem value="L2"  Text="NON-LDP" Selected="True"/>
                    <asp:ListItem Value="M2"  Text="MAINLINE" Selected="True"/>
                </asp:CheckBoxList>
                </td>
                <td><asp:ImageButton ID="lnk_OrderType" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_OrderType'); return false;" /></td>
            </tr>
        -->
        </table>
    </td>
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

<tr id="tr_Destination">
    <td class="FieldLabel4_T" valign="top">Destination</td>
    <td>
        <asp:Panel ID="pnl_Destination" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_Destination" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;Destination</div>
        </asp:Panel>
        <table id="tbl_Destination" style="display:none;">
          <tr>
            <td><asp:CheckBoxList ID="cbl_Destination" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="3"  class="CellWithBorder"/></td>
            <td><asp:ImageButton ID="lnkDestination" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_Destination'); return false;" /></td>
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

<tr id="tr_PackingMethod">
    <td class="FieldLabel4_T" valign="top">Packing Method</td>
     <td>
        <asp:Panel ID="pnl_PackingMethod" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_PackingMethod" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;Packing Method</div>
        </asp:Panel>
        <table id="tbl_PackingMethod" style="display:none;">
           <tr>
            <td><asp:CheckBoxList ID="cbl_PackingMethod" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder"/></td>
            <td><asp:ImageButton ID="lnkPackingMethod" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_PackingMethod'); return false;" /></td>
          </tr>
        </table>
    </td>
</tr>

<tr id="tr_TermOfPurchase">
    <td class="FieldLabel4_T" valign="top">Term of Purchase</td>
     <td>
        <asp:Panel ID="pnl_TermOfPurchase" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_TermOfPurchase" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;Term of Purchase</div>
        </asp:Panel>
        <table id="tbl_TermOfPurchase" style="display:none;">
          <tr>
            <td><asp:CheckBoxList ID="cbl_TermOfPurchase" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder"/></td>
            <td><asp:ImageButton ID="lnkTermOfPurchase" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_TermOfPurchase'); return false;" /></td>
          </tr>
        </table>
    </td>
</tr>

<tr id="tr_OPRType">
    <td class="FieldLabel4_T" valign="top">OPR Type</td>
     <td>
        <asp:Panel ID="pnl_OPRType" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_OPRType" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;OPR Type</div>
        </asp:Panel>
        <table id="tbl_OPRType" style="display:none;">
          <tr>
            <td><asp:CheckBoxList ID="cbl_OPRType" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder"/></td>
            <td><asp:ImageButton ID="lnkOPRType" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_OPRType'); return false;" /></td>
          </tr>
        </table>
    </td>
</tr>

<tr id="tr_Customer">
    <td class="FieldLabel4_T" valign="top">Customer</td>
     <td>
        <asp:Panel ID="pnl_Customer" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_Customer" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;Customer</div>
        </asp:Panel>
        <table id="tbl_Customer" style="display:none;">
          <tr>
            <td><asp:CheckBoxList ID="cbl_Customer" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder"/></td>
            <td><asp:ImageButton ID="lnkCustomer" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_Customer'); return false;" /></td>
          </tr>
        </table>
    </td>
</tr>

<tr id="tr_ShipmentStatus">
    <td class="FieldLabel4_T" valign="top">PO Status</td>
     <td>
        <asp:Panel ID="pnl_ShipmentStatus" runat="server" skinid="sectionHeader1" Width="480px" height="22">
            <div style="float:left; padding:1px;"><img id="img_ShipmentStatus" onclick="ChangePanelStatus(this);" style='cursor:hand;' src="../images/summy_reverse.jpg"  title='Show options' alt ="SHOW" /></div>
            <div style="float:left; padding:4px;">&nbsp;&nbsp;PO Status</div>
        </asp:Panel>
        <table id="tbl_ShipmentStatus" style="display:none;">
          <tr>
            <td><asp:CheckBoxList ID="cbl_ShipmentStatus" runat="server" TextAlign="Right" RepeatDirection="Horizontal" Width="500" RepeatLayout="Table" RepeatColumns="4"  class="CellWithBorder"/></td>
            <td><asp:ImageButton ID="lnkShipmentStatus" ImageUrl="~/images/icon_clear.gif" runat="server" OnClientClick="UncheckAll('cbl_ShipmentStatus'); return false;" /></td>
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

<tr>
    <td class="FieldLabel4">Payment Term</td>
    <td >
        <cc2:SmartDropDownList ID="ddl_PaymentTerm" runat="server" >
            <asp:ListItem Value="-1" Text="--All--"></asp:ListItem>
            <asp:ListItem Value="1" Text="O/A"></asp:ListItem>
            <asp:ListItem Value="2" Text="L/C"></asp:ListItem>
        </cc2:SmartDropDownList>
    </td>
</tr>
<tr>
    <td class="FieldLabel4">LC No.</td>
    <td>
        <asp:TextBox ID="txt_LCNoFrom" runat='server' value='' onblur="copyValue(this);" /> 
        &nbsp;To&nbsp;
        <asp:TextBox ID="txt_LCNoTo" runat="server" value='' />
    </td>
</tr>

<tr>
    <td class="FieldLabel4_T" valign="top">Sort By</td>
    <td class="CellWithBorder">
        <uso:UclSortingOrder ID="ucl_SortingOrder" runat="server" />
    </td>
</tr>
<tr>
    <td></td>
    <td>
        
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
        <asp:Button ID="btn_Reset" runat="server" Text="Export to Excel" SkinID="LButton" OnClick="btn_Export_Click"  OnClientClick="return isFormValid();" />

</asp:Content>
