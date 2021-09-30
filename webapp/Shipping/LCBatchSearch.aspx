<%@ Page Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="LCBatchSearch.aspx.cs" Inherits="com.next.isam.webapp.shipping.LCBatchSearch" Title="ISAM" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc1" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_shipping_lc_maint.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Shipping">L/C Maintenance</asp:Panel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="800px" cellspacing="0" cellpadding="2">
    <tr>
        <td width="100px">&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel2">&nbsp;Office</td>
        <td><cc1:smartdropdownlist id="ddl_Office" runat="server" Width="120px"></cc1:smartdropdownlist></td>
    </tr>
    <tr>
        <td class="FieldLabel2">&nbsp;Supplier</td>
        <td>
<!--
        <asp:TextBox ID="txt_Supplier" runat="server" SkinID="TextBoxLarge" />
                <asp:ImageButton ID="btn_ClearSupplier" runat="server" ImageUrl="~/images/icon_clear.gif" />
-->
        <uc1:UclSmartSelection  ID="txt_SupplierName" runat="server" width="300px"/>

        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">&nbsp;L/C Batch No.</td>
        <td><asp:TextBox ID="txt_BatchNoFrom" runat="server" SkinID="DateTextBox" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="txt_BatchNoTo" runat="server" SkinID="DateTextBox" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">&nbsp;L/C Applied Date</td>
        <td>
            <cc1:smartcalendar id="txt_AppliedDateFrom"  runat="server" Width="120px" FromDateControl="txt_AppliedDateFrom"
				ToDateControl="txt_AppliedDateTo"></cc1:smartcalendar>&nbsp;&nbsp;to&nbsp;
			<cc1:smartcalendar id="txt_AppliedDateTo" runat="server" Width="120px"  FromDateControl="txt_AppliedDateFrom"
				ToDateControl="txt_AppliedDateTo" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">&nbsp;L/C No.</td>
        <td><asp:TextBox ID="txt_LCNoFrom" runat="server" SkinID="DateTextBox" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;to&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="txt_LCNoTo" runat="server" SkinID="DateTextBox" />
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <asp:Button ID="btn_Search" runat="server" Text="Search" 
                onclick="btn_Search_Click" />&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btn_Extract" runat="server" Text="Extract" 
                onclick="btn_Extract_Click" />
        </td>
    </tr>
</table>
<br />
<!--
<asp:LinkButton ID="btn_Refresh" runat="server" Text="Refresh" />
<asp:TextBox ID="txt_return" runat="server" Text="" value=""/>
-->


<asp:LinkButton ID="btn_SelectAll" runat="server" Text="Select All" 
        OnClientClick="CheckAll('ckb_LCBatch'); return false;" />&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="btn_DeselectAll" runat="server" Text="Deselect All" 
        OnClientClick="UncheckAll('ckb_LCBatch'); return false;" />

<asp:GridView ID="gv_LCBatch" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_LCBatch_RowDataBound" >
    <Columns>
        <asp:TemplateField >
            <ItemTemplate>
                <asp:CheckBox ID="ckb_LCBatch" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="L/C Batch No.">
            <ItemTemplate >
                <asp:LinkButton ID="lnk_LCBatchNo" runat="server" OnClientClick="javascript:window.open('LCBatchUpdate.aspx','LCBatch','width=700,height=400,scrollbars=1,resizable=1,status=1'); return false;" Text='' />
                <asp:Label ID="lbl_LCBatchId" runat="server" style="display:none;"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Office" >
            <ItemTemplate >
                <asp:Label ID="lbl_Office" runat="server" Text='' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supplier">
            <ItemTemplate >
                <asp:Label ID="lbl_Supplier" runat="server" Text='' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="No. of Shipment">
            <ItemTemplate>
                <asp:Label ID="lbl_NoOfShip" runat="server" Text='' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ccy">
            <ItemTemplate >
                <asp:Label ID="lbl_Currency" runat="server" Text='' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="PO Amount">
            <ItemTemplate>
                <asp:Label ID="lbl_POAmt" runat="server" Text='' style='text-align:right;' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText ="L/C Applied Date">
            <ItemTemplate >
                <asp:Label ID="lbl_ApplicationDate" runat="server" Text='' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Applied By">
            <ItemTemplate >
                <asp:Label ID="lbl_AppliedBy" runat="server" Text='' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate >
                <asp:Label ID="lbl_Status" runat="server" Text='' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
