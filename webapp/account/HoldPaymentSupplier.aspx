<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="HoldPaymentSupplier.aspx.cs" Inherits="com.next.isam.webapp.account.HoldPaymentSupplier"  Title="Hold Payment For Supplier" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head id="Head1" runat="server">
    <title>Hold/Unhold Payment for Supplier</title>
</head>
<body>
    <form id="form2" runat="server">
        <br />
        <b>You are going to hold / unhold payments for below supplier:</b>
        <br /><br />
        <table width="100%">
            <tr>
                <td colspan="5">
                    <asp:label ID="lbl_VendorName" runat="server" Text="" style="font-size:medium;" ForeColor="AliceBlue"/>
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <asp:label ID="lbl_HoldStatus" runat="server" Text="" style="font-size:small; color:green" />
                </td>
            </tr>
            <tr>
                <td colspan="5"><br />Remark</td>
            </tr>
            <tr>
                <td colspan="5"><asp:TextBox runat="server" ID="txt_Remark" Rows="4" TextMode="MultiLine" style="width:90%;" /></td>
            </tr>
            <tr>
                <td>
                    <asp:button ID="btn_Hold" runat="server" Text="Hold" ToolTip="Hold Payment" 
                        onclick="btn_Hold_Click"/>
                </td>
                <td>
                    <asp:Button ID="btn_Unhold" runat="server" Text="Un-Hold" 
                        tooltip="Un-Hold Payment" onclick="btn_Unhold_Click"/>
                </td>
                <td colspan="3">
                    <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" OnClientClick="window.close();"/>
                </td>
            </tr>
        </table>
    </form>
</body>    
</html>