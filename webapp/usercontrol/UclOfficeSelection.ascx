<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UclOfficeSelection.ascx.cs" Inherits="com.next.isam.webapp.usercontrol.UclOfficeSelection" %>
<table style="vertical-align:top ;"  cellspacing="0">
<tr>
    <td class="FieldLabel4_T" style="width:150px; vertical-align:top ;">Office</td>
    <td>        
        <div style="background-color:#F0F0F0;vertical-align:middle; text-align: center; height:15px;"><span style="font-weight:bolder; color:black; " >Office</span></div>
        <div style="overflow-y:scroll;height:100px;"> 
        <asp:GridView ID="gv_Office" runat="server" Width="100px" ShowHeader="false" >
            <Columns >
                <asp:TemplateField>
                    <ItemTemplate >
                        <asp:CheckBox ID="ckb_Office" runat="server" Text='<%# Eval("OfficeCode") %>' Checked="true"   />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Left" />
                </asp:TemplateField>
                <asp:TemplateField Visible="false">
                    <ItemTemplate>
                        <asp:Label ID="lbl_OfficeId" runat="server" Text='<%# Eval("OfficeId") %>' />
                    </ItemTemplate>
                </asp:TemplateField>                
                <asp:TemplateField Visible="false">
                    <ItemTemplate>
                        <asp:Label ID="lbl_OfficeDesc" runat="server" Text='<%# Eval("Description") %>' />
                    </ItemTemplate>
                </asp:TemplateField>                
            </Columns>            
        </asp:GridView>
        </div>
        <asp:LinkButton ID="lnk_AllOffice" runat="server" OnClientClick="CheckAll('ckb_Office'); return false;" Text="Select All" />&nbsp;&nbsp;&nbsp;<asp:LinkButton 
        Id="lnk_ClearOffice" runat="server" OnClientClick="UncheckAll('ckb_Office'); return false;" Text="Clear" />
    </td>
</tr>

</table>