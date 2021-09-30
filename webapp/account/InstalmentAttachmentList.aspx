<%@ Page Language="C#"  Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="InstalmentAttachmentList.aspx.cs" Inherits="com.next.isam.webapp.account.InstalmentAttachmentList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>DMS Uploaded Document(s)</title>
</head>
<script language="javascript" src="../includes/common.js"></script>
<script type="text/javascript">
    window.focus();
</script>
<body>
    <form id="form1" runat="server">
   <table cellspacing="2" cellpadding="2" border="0">
            <tr>
                <td colspan="2" class="tableHeader">Attachment List</td>
            </tr>
            <tr>
                <td width="100"><b>Payment No:</b></td>
                <td width="250"><asp:Label runat="server" ID="lblPaymentNo" /></td>
            </tr>
            <tr>
                <td ><b>Supplier Name:</b></td>
                <td ><asp:Label ID="lblSupplierName" runat="server" /></td>
            </tr>
        </table>
        <br />
        <asp:GridView ID="gvAttachment" runat="server" AutoGenerateColumns ="false" 
                onrowdatabound="gvAttachment_RowDataBound" OnRowCommand="gvAttachment_RowCommand">
            <Columns >
                <asp:TemplateField HeaderText="File Name"  HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate >
                        <asp:LinkButton ID="lnk_FileName" runat="server" CommandName="OpenAttachment" ToolTip="Open Document"/>
                        <asp:Label ID="lbl_FileName" runat="server" />&nbsp;&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Upload Date"  HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate >
                        <asp:Label ID="lbl_UploadDate" runat="server" />&nbsp;&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate >
                        <asp:ImageButton runat="server" CommandName="RemoveDoc" ID="imgRemove" ImageUrl="~/images/icon_delete.gif" ToolTip="Remove Doc" />&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate >
                        <asp:ImageButton runat="server" CommandName="ReviseDoc" ID="imgEdit" ImageUrl="~/images/icon_edit.gif" ToolTip="Revise Doc" />&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate >
                        <asp:ImageButton runat="server" CommandName="MailDoc" ID="imgMail" ImageUrl="~/images/email.gif"  ToolTip="eMail the document to your mailbox"/>&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>    
            <EmptyDataTemplate>
                <br />&nbsp;&nbsp; <span style="color:Red;">* No Uploaded Document</span>
            </EmptyDataTemplate>
        </asp:GridView>
        <p>
        <asp:LinkButton runat="server" ID="lnkInstalmentDoc" Text="Add Supporting Document" onclick="lnkInstalmentDoc_Click"  />

        </p>
    </form>
</body>
</html>
