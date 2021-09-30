<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="DNAttachmentList.aspx.cs" Inherits="com.next.isam.webapp.claim.DNAttachmentList" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" onContextMenu="return false;"  >
<head id="Head1" runat="server">
    <title>Debit Note Email Attachment List</title>
    <script language="javascript" src="../includes/common.js"></script>
</head>
<script type="text/javascript">
    window.focus();
</script>
<body>
    <form id="form1" runat="server"  >
        <table cellspacing="2" cellpadding="2" border="0">
            <tr>
                <td colspan="2" class="tableHeader">Email Attachment List</td>
            </tr>
            <tr>
                <td width="100"><b>D/N No:</b></td>
                <td width="250"><asp:Label runat="server" ID="lblDNNo" /></td>
            </tr>
            <tr>
                <td ><b>Vendor</b></td>
                <td ><asp:Label ID="lblVendor" runat="server" /></td>
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
            </Columns>    
            <EmptyDataTemplate>
                <br />&nbsp;&nbsp; <span style="color:Red;">* No Attachment(s)</span>
            </EmptyDataTemplate>
        </asp:GridView>
        <p>
        </p>
    </form>
</body>    
</html>
