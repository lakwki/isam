<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="PaymentUpdate.aspx.cs" Inherits="com.next.isam.webapp.account.PaymentUpdate" Title="Payment Record Update" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">Payment Record Update</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table width="800px">
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel2">Bank</td>
		<TD>
		    <asp:radiobuttonlist id="radBankRange" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="false">
				<asp:ListItem Value="HSBC" Selected="True">HSBC</asp:ListItem>
				<asp:ListItem Value="SCB">SCB</asp:ListItem>
			</asp:radiobuttonlist>
		</TD>
    </tr>
    <tr>
        <td class="FieldLabel2">Source File</td>
        <td>
            <INPUT id="updFile" type="file" name="updFile" runat="server">
			<asp:customvalidator id="valFile" runat="server" Enabled="False" 
                ErrorMessage="CustomValidator" onservervalidate="valFile_ServerValidate"></asp:customvalidator>
	    </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Target Database</td>
        <td>
            <asp:dropdownlist id="cbxSunDB" runat="server" CssClass="cbx" Width="300px"></asp:dropdownlist>
        </td>
    </tr>
    <tr>
        <td colspan="2">&nbsp;</td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btnProcess" runat="server" Text="Process" 
                onclick="btnProcess_Click" />&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnListPending" SkinID="LButton"  runat="server" Text="List Pending Job(s)" />&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnCancel" runat="server" Text="Cancel Job(s)" />
            <asp:label id="lblBankFileName" runat="server" Visible="False"></asp:label>
        </td>
    </tr>
</table>
<br />
<div>
    <asp:Repeater id="repQueueList" runat="server">
	    <HeaderTemplate>
		    <table cellSpacing="0" cellPadding="2" width="720" border="1" bordercolorlight="#dcdcdc" bordercolordark="#ffffff">
			    <TR>
				    <td align="center" bgcolor="#f5f5f5"></td>
				    <TD align="center" bgcolor="#f5f5f5">File Name</TD>
				    <TD align="center" bgcolor="#f5f5f5">User Name</TD>
				    <TD align="center" bgcolor="#f5f5f5">Upload Date</TD>
				    <TD align="center" bgcolor="#f5f5f5">Status</TD>
				    <TD align="center" bgcolor="#f5f5f5">Bank Name</TD>
				    <TD align="center" bgcolor="#f5f5f5">Target SUN-DB</TD>
			    </TR>
	    </HeaderTemplate>
	    <ItemTemplate>
		    <tr>
			    <td align="center">
				    <asp:CheckBox Runat="server" ID="chkSelected"></asp:CheckBox>
				</td>
			    <td align="center">
				    <asp:Label Runat="server" ID="lblUploadFileName"></asp:Label>
				</td>
			    <td align="center">
				    <asp:Label Runat="server" ID="lblUserName"></asp:Label>
				</td>
			    <td align="center">
				    <asp:Label Runat="server" ID="lblUploadDate"></asp:Label>
				</td>
			    <td align="center">
				    <asp:Label Runat="server" ID="lblFinishDate"></asp:Label>
				</td>
			    <td align="center">
				    <asp:Label Runat="server" ID="lblBankName"></asp:Label>
				</td>
			    <td align="center">
				    <asp:Label Runat="server" ID="lblSunDB" Font-Bold="True"></asp:Label>
				</td>
		    </tr>
	    </ItemTemplate>
	    <FooterTemplate>
		    </table>
	    </FooterTemplate>
    </asp:Repeater>
</div>
</asp:Content>
