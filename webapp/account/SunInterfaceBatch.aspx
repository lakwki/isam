<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="SunInterfaceBatch.aspx.cs" Inherits="com.next.isam.webapp.account.SunInterfaceBatch" Title="Generate Sun Interfaces via Sun Macro" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Generate Epicor Interface Batch</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="800px">
    <tr>
        <td colspan="2">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:120px;">Office Group</td>
        <td><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="250px" 
                AutoPostBack="True" onselectedindexchanged="ddl_Office_SelectedIndexChanged"></cc2:smartdropdownlist></td>
    </tr>
    <tr id="trFiscalYear">
        <td class="FieldLabel2">Fiscal Year</td>
        <td>
            <cc2:smartdropdownlist id="ddl_Year" runat="server" Width="250px" AutoPostBack="True" onselectedindexchanged="ddl_Year_SelectedIndexChanged"></cc2:smartdropdownlist>
        </td>
    </tr>
    <tr id="trPeriod">
        <td class="FieldLabel2">Period</td>
        <td>
            <cc2:smartdropdownlist id="ddl_Period" runat="server" Width="250px" 
                AutoPostBack="True" onselectedindexchanged="ddl_Period_SelectedIndexChanged">
                <asp:ListItem Text="Period 1" Value="1" />
                <asp:ListItem Text="Period 2" Value="2" />
                <asp:ListItem Text="Period 3" Value="3" />
                <asp:ListItem Text="Period 4" Value="4" />
                <asp:ListItem Text="Period 5" Value="5" />
                <asp:ListItem Text="Period 6" Value="6" />
                <asp:ListItem Text="Period 7" Value="7" />
                <asp:ListItem Text="Period 8" Value="8" />
                <asp:ListItem Text="Period 9" Value="9" />
                <asp:ListItem Text="Period 10" Value="10" />
                <asp:ListItem Text="Period 11" Value="11" />
                <asp:ListItem Text="Period 12" Value="12" />
            </cc2:smartdropdownlist>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" valign="top">Interface Types:</td>
        <td>
            <font color="green"><b>Below interfaces will be included:</b></font><br />
        </td>
    </tr>
    <tr>
        <td></td>
        <td><asp:CheckBoxList id="cblInterfaceType" runat="server"></asp:CheckBoxList></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <asp:Button ID="btn_Process" runat="server" Text="Process" 
                onclick="btn_Process_Click" />
            <asp:CustomValidator ID="valCustom" runat="server" Display="None" 
                ErrorMessage="Duplicate interface submission is not allowed" 
                onservervalidate="valCustom_ServerValidate"></asp:CustomValidator>
        </td>
    </tr>
</table>
</asp:Content>
