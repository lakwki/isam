<%@ Page Language="C#" AutoEventWireup="true"  Theme="DefaultTheme" CodeBehind="GetRejectReason.aspx.cs" Inherits="com.next.isam.webapp.account.GetRejectReason" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >

<head id="Head1" runat="server">
    <title>Reject Reason</title>
    <script src="../common/common.js" type="text/javascript" ></script>   
</head>

<script type="text/javascript">
    window.focus();
    window.returnValue = -1;

    function ConfirmSelection() {
        window.returnValue = parseInt(document.all.ddl_Reason.item(document.all.ddl_Reason.selectedIndex).value);
        window.close();
    }

    function CancelSelection() {
        window.returnValue = -1;
        window.close();
    }
    

</script>
<body>
    <form id="form1" runat="server">
    <!-- <asp:Panel runat="server" SkinID="sectionHeader1" style='height:30px;'>Reject Reason</asp:Panel> -->
    <br />
        <table>
            <tr>
                <td colspan="3">
                    <asp:panel SkinID="sectionHeader2" runat="server" Font-Size="Medium" Font-Bold="true">Please select the reject reason :</asp:panel>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:DropDownList ID="ddl_Reason" runat="server"  style="width:250px;" />
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnComfirm" runat="server" text="Confirm" 
                        OnClientClick="ConfirmSelection(); return false;" onclick="btnComfirm_Click"/>
                    &nbsp;&nbsp;
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>
    
    </form>
</body>


</html>
