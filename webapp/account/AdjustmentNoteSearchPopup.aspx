<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="AdjustmentNoteSearchPopup.aspx.cs" Inherits="com.next.isam.webapp.account.AdjustmentNoteSearchPopup" Title="Change Currency" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<!DOCTYPE html>

<script type="text/javascript">

    function Close() {
        if (window.opener != null && !window.opener.closed) {
            window.opener.document.all.btn_Search.click();
            window.close();
            //window.opener.location = window.opener.location['href'];
        }
    }

    function Confirm() {
        var confirm_value = document.createElement("INPUT");
        confirm_value.type = "hidden";
        confirm_value.name = "confirm_value";
        if (confirm("Are you sure to proceed?")) {
            confirm_value.value = "Yes";
        } else {
            confirm_value.value = "No";
        }
        document.forms[0].appendChild(confirm_value);
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Change Currency</title>
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <table>
                <tr>
                    <td class="FieldLabel2" style="width: 120px">Type</td>
                    <td style="width: 319px">
                        <asp:label ID="lbl_type" runat="server"></asp:label>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel2" style="width: 120px">DC Note No.</td>
                    <td style="width: 319px">
                        <asp:label ID="lbl_dcNoteNo" runat="server"></asp:label>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel2" style="width: 120px">Vendor</td>
                    <td style="width: 319px">
                        <asp:label ID="lbl_vendor" runat="server"></asp:label>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel2" style="width: 120px">Amount</td>
                    <td style="width: 319px">
                        <asp:label ID="lbl_Amount" runat="server"></asp:label>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel4" style="width: 146px">
                        <asp:Label ID="lbl_Currency" runat="server">Currency</asp:Label></td>
                    <td>
                        <cc2:smartdropdownlist id="ddl_BaseCurrency" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:Button runat="server" ID="btn_Confirm" Text="Confirm" OnClick="btn_Confirm_Click" OnClientClick="Confirm()" SkinID="LButton" />
        </div>
    </form>
</body>
</html>
