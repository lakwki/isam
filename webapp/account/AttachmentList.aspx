<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="AttachmentList.aspx.cs" Inherits="com.next.isam.webapp.account.AttachmentList"  Title="DMS Uploaded Document" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" onContextMenu="return false;"  >
<head id="Head1" runat="server">
    <title>DMS Uploaded Document(s)</title>
    <script src="../common/common.js" type="text/javascript" ></script>   
</head>
<script type="text/javascript">
    window.focus();
</script>
<body>
    <form id="form1" runat="server"  >
        <table>
            <tr>
                <td class="tableHeader" colspan="2">&nbsp;Uploaded Document(s) from DMS&nbsp;</td>
            </tr>
            <tr>
                <td><b>Contract No:</b></td>
                <td><asp:Label runat="server" ID="lblContractNo" /></td>
            </tr>
            <tr>
                <td><b>Invoice No:</b></td>
                <td><asp:Label runat="server" ID="lblInvoiceNo" /></td>
            </tr>
            <tr>
                <td><b>Currency:</b></td>
                <td><asp:Label ID="lbl_Currency" runat="server" /></td>
            </tr>
            <tr>
                <td><b>Net Amt:</b></td>
                <td><asp:Label ID="lbl_NetAmt" runat="server" /></td>
            </tr>
        </table>
        <asp:GridView ID="gvAttachment" runat="server" AutoGenerateColumns ="false" 
                onrowcommand="gvAttachment_RowCommand" 
                onrowdatabound="gvAttachment_RowDataBound">
            <Columns >
                <asp:TemplateField HeaderText="File Name">
                    <ItemTemplate >
                        <asp:Label ID="lbl_FileName" runat="server" />&nbsp;&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Version No">
                    <ItemTemplate >
                        <asp:Label ID="lbl_MajorId" runat="server" />.<asp:Label ID="lbl_MinorId" runat="server" />.<asp:Label ID="lbl_BuildId" runat="server" />&nbsp;&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Upload Date">
                    <ItemTemplate >
                        <asp:Label ID="lbl_UploadDate" runat="server" />&nbsp;&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate >
                        <asp:ImageButton runat="server" CommandName="OpenAttachment" ID="imgOpen" ImageUrl="~/images/icon_edit.gif"  ToolTip="Open Document"/>&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate >
                        <asp:ImageButton runat="server" CommandName="MailDoc" ID="imgMail" ImageUrl="~/images/email.gif"  ToolTip="eMail the document to your mailbox"/>&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate >
                        <asp:Label ID="lbl_Review" runat="server" />&nbsp;&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>    
            <EmptyDataTemplate>
                <br />&nbsp;&nbsp; <span style="color:Red;">* No Uploaded Document</span>
            </EmptyDataTemplate>
        </asp:GridView>
        <br />
        <asp:LinkButton runat="server" CommandName="Review" ID="lnkReview" 
            Text="Mark As Reviewed" onclick="lnkReview_Click" />
    </form>
</body>    
</html>
