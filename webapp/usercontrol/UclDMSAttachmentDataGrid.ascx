<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UclDMSAttachmentDataGrid.ascx.cs" Inherits="com.next.isam.webapp.usercontrol.UclDMSAttachmentDataGrid" %>
<asp:GridView ID="gvAttachment" runat="server" AutoGenerateColumns ="false" onrowdatabound="gvAttachment_RowDataBound" OnRowCommand="gvAttachment_RowCommand">
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
        <br />&nbsp;&nbsp; <span style="color:Red;">* No Uploaded Document</span>
    </EmptyDataTemplate>
</asp:GridView>
