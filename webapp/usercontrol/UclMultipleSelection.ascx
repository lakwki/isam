<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UclMultipleSelection.ascx.cs" Inherits="com.next.isam.webapp.usercontrol.UclMultipleSelection" %>
<table border="0" cellspacing="0" cellpadding="0">
    <tr>
        <td>
            <table cellspacing="0" cellpadding="0" style="vertical-align:top;">
            <tr>
                <td>
                    <asp:panel ID="pnl_Title" runat="server" style="background-color:#F0F0F0;vertical-align:middle; text-align: center; height:15px;Display:none;border:1px solid Silver;border-bottom:none;">
                        <span style="font-weight:bolder; color:black; " >
                            <asp:Label ID="lbl_Title" runat="server" Text=""/>
                        </span>
                    </asp:panel>
                    <asp:panel id="pnl_Item" runat="server" style=" height:70px; overflow:auto;border:1px solid Silver; "> 
                        <asp:GridView ID="gv_Item" runat="server" ShowHeader="false" Width="100%">
                            <Columns >
                                <asp:TemplateField>
                                    <ItemTemplate >
                                        <asp:CheckBox ID="ckb_Item" runat="server" Checked='<%# Eval("Selected") %>'/>
                                        <asp:Label ID="lbl_ItemDesc" runat="server" Text='<%# Eval("Text") %>'/> 
                                        <asp:Label ID="lbl_ItemId" runat="server" Text='<%# Eval("Value") %>' style='display:none;'/>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                            </Columns>            
                        </asp:GridView>
                    </asp:panel>
                </td>
            </tr>
            </table>
        </td>
        <td>&nbsp;</td>
        <td valign="top">
            <asp:ImageButton ID="img_AllItem" runat="server" OnClick="SelectAll_OnClick" ImageUrl="../images/icon_selectall.jpg"  ToolTip="Select All" />
            <br /> 
            <asp:ImageButton Id="img_ClearItem" runat="server" OnClick="DeselectAll_OnClick" ImageUrl="../images/icon_deselectall.jpg" ToolTip="Clear All" />
        </td>
    </tr>
</table>