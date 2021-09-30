<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="ViewLog.aspx.cs" Inherits="com.next.isam.webapp.claim.ViewLog" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml" onContextMenu="return false;"  >
<head id="Head1" runat="server">
    <title>Audit Log</title>
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
                <td colspan="2" class="tableHeader">Audit Log</td>
            </tr>
            <tr>
                <td width="100" class="FieldLabel2"><b>Next D/N No.:</b></td>
                <td width="250"><asp:Label runat="server" ID="lblUKDNNo" /></td>
            </tr>
            <tr>
                <td class="FieldLabel2"><b>Item No:</b></td>
                <td><asp:Label runat="server" ID="lblItemNo" /></td>
            </tr>
            <tr>
                <td class="FieldLabel2"><b>Status:</b></td>
                <td><asp:Label ID="lblStatus" runat="server" /></td>
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
                <asp:TemplateField HeaderText="From Status">
                    <ItemTemplate >
                        <asp:Label ID="lbl_FromStatus" runat="server" />&nbsp;&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="To Status">
                    <ItemTemplate >
                        <asp:Label ID="lbl_ToStatus" runat="server" />
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
