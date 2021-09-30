<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="PaymentFileConversion.aspx.cs" Inherits="com.next.isam.webapp.account.PaymentFileConversion" Title="Payment File Conversion" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">Payment File Conversion</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="800px">
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td class="FieldLabel2">Bank</td>
        <td>
            <asp:RadioButtonList ID="rad_Bank" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" RepeatLayout="Flow" 
                OnSelectedIndexChanged="BankSelectIndexChanged">
                <asp:ListItem Text ="HSBC" Value="HSBC" Selected="True" />
                <asp:ListItem Text="SCB" Value="SCB" />
            </asp:RadioButtonList>
            
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Source File</td>
        <td>
            <asp:FileUpload ID="FileUpload1" Width="400px" runat="server"  /></td>
    </tr>
    <tr id="row_ChargeMethod" runat="server">
        <td class="FieldLabel2">Charge Method</td>
        <td>
            <asp:DropDownList ID="ddl_ChargeMethod" runat="server">
                <asp:ListItem Text="SHA - Share" Value="1" />
                <asp:ListItem Text="BEN - Beneficiary" Value="2" />
                <asp:ListItem Text="OUR - NSL" Value="3" />
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td><asp:Button ID="btn_Process" runat="server" Text="Process" OnClick="btn_Process_Click" /></td>
    </tr>
</table>

</asp:Content>
