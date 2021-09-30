<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="ViewAdvancePaymentLog.aspx.cs" Inherits="com.next.isam.webapp.account.ViewAdvancePaymentLog" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml" onContextMenu="return false;"  >
<head id="Head1" runat="server">
    <title>Advance Payment Audit Log</title>
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
    <script language="javascript" src="../includes/common.js"></script>

<script type="text/javascript">
    window.focus();
</script>

</head>
<body>
    <form id="form1" runat="server" >
        <table cellspacing="2" cellpadding="2" border="0">
            <tr>
                <td colspan="2" class="tableHeader">Advance Payment Audit Log</td>
            </tr>
            <tr>
                <td width="100" class="FieldLabel2"><b>Advance Payment No.:</b></td>
                <td width="250"><asp:Label runat="server" ID="lblPaymentNo" /></td>
            </tr>
            <tr>
                <td class="FieldLabel2"><b>Vendor:</b></td>
                <td><asp:Label runat="server" ID="lblVendor" /></td>
            </tr>
        </table>
        <br />
        <asp:GridView ID="gvLog" runat="server" AutoGenerateColumns ="false" 
            onrowdatabound="gvLog_RowDataBound" CellPadding="3">
            <Columns >
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Action" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="User">
                    <ItemTemplate >
                        <asp:Label ID="lbl_User" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Log Date">
                    <ItemTemplate >
                        <asp:Label ID="lbl_Date" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>    
        </asp:GridView>
    </form>
</body>    
</html>
