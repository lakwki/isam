<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UclSortingOrder.ascx.cs" Inherits="com.next.isam.webapp.usercontrol.UclSortingOrder" %>
<table>
    <tr>
        <td></td>
        <td></td>
        <td>Selected : </td>
    </tr>
    <tr>
        <td>
            <asp:ListBox ID="ListBox1" Width="150px" runat="server"></asp:ListBox>
        </td>
        <td>
            <asp:Button ID="btn_ToRight" runat="server" Text=">>"  Width="20"
                onclick="btn_ToRight_Click" SkinID="XSButton" /><br />
            <asp:Button ID="btn_ToLeft" runat="server" Width="20" Text="<<" 
                onclick="btn_ToLeft_Click" SkinID="XSButton" />
        </td>
        <td>
            <asp:ListBox ID="ListBox2" runat="server" Width="150px" />
    </tr>
</table>

