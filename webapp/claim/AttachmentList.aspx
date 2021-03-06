<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="AttachmentList.aspx.cs" Inherits="com.next.isam.webapp.claim.AttachmentList" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" onContextMenu="return false;"  >
<head id="Head1" runat="server">
    <title>DMS Uploaded Document(s)</title>
    <script language="javascript" src="../includes/common.js"></script>
</head>
<script type="text/javascript">
    window.focus();
</script>
<body>
    <form id="form1" runat="server"  >
        <table cellspacing="2" cellpadding="2" border="0">
            <tr>
                <td colspan="2" class="tableHeader">Attachment List</td>
            </tr>
            <tr>
                <td width="100"><b>Next D/N No:</b></td>
                <td width="250"><asp:Label runat="server" ID="lblUKDNNo" /></td>
            </tr>
            <tr>
                <td ><b>Status:</b></td>
                <td ><asp:Label ID="lblStatus" runat="server" /></td>
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
                <asp:TemplateField HeaderText="Type"  HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate >
                        <asp:Label ID="lbl_Type" runat="server" />&nbsp;&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Version No"  HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate >
                        <asp:Label ID="lbl_MajorId" runat="server" />.<asp:Label ID="lbl_MinorId" runat="server" />.<asp:Label ID="lbl_BuildId" runat="server" />&nbsp;&nbsp;
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Upload Date"  HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate >
                        <asp:Label ID="lbl_UploadDate" runat="server" />&nbsp;&nbsp;
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
        <asp:LinkButton runat="server" ID="lnkAddDirectorCopy" 
                Text="Add Director Signed Document" onclick="lnkAddDirectorCopy_Click"  />
        <asp:LinkButton runat="server" ID="lnkAddCOOCopy" Text="Add COO Signed Document" 
                onclick="lnkAddCOOCopy_Click"  /><br />
        <asp:LinkButton runat="server" ID="lnkRefund" 
                Text="Add Next Claim Refund Supporting" onclick="lnkRefund_Click" /><br />
        <asp:LinkButton runat="server" ID="lnkCNToSupplier" 
                Text="Add CN To Supplier Copy" onclick="lnkCNToSupplier_Click" />
        <asp:LinkButton runat="server" ID="lnkAddCOOInstructions" 
                Text="Add COO Instructions Copy" onclick="lnkAddCOOInstructions_Click" ToolTip="eg. provision for claims or write-off claims not related to technical" /><br />
        <asp:LinkButton runat="server" ID="lnkOtherDoc" 
                Text="Add Other Supporting Document" 
                ToolTip="eg. supporting for void debit/credit note" 
                onclick="lnkOtherDoc_Click" /><br />

        <asp:Button runat="server" ID="btnReject" Text="Reject Next Claim" 
                ToolTip="Supporting document has to be uploaded to DMS" 
                onclick="btnReject_Click"/><br />

        </p>
    </form>
</body>    
</html>
