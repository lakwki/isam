<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UclProductTeamSelection.ascx.cs" Inherits="com.next.isam.webapp.usercontrol.UclProductTeamSelection" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<table>
    <tr>
        <td style="width:150px;">Office</td>
        <td><asp:CheckBoxList ID="cbl_Office" RepeatDirection="Horizontal" RepeatColumns ="6" runat="server" /></td>
    </tr>
    <tr>
        <td>Product Team</td>
        <td>
                <div style="border: 1px solid #CCCCCC; "  style="overflow-y:scroll; height:50px;">
                    <asp:Repeater ID="rep_SelectedList" runat="server"  OnItemDataBound="SelectedList_DataBound"  OnItemCommand="SelectedList_ItemCommand">
                    <ItemTemplate>
                        <%--<div style="border-right : solid 1px #EECCEE; border-right-color: #C0C0C0; border-right-style: solid; border-bottom-style: solid; border-right-width: 1px; border-bottom-width: 1px; border-bottom-color: #C0C0C0;">--%>

                        <asp:Label ID="lbl_ProdTeam" runat="server"  />
                        <asp:ImageButton ID="btn_Remove" runat="server" ImageAlign="Middle"  ImageUrl="~/images/delete_icon.gif" CommandName="Delete" />
        &nbsp;&nbsp;&nbsp;<br />
                    </ItemTemplate>            
                </asp:Repeater>
                </div>
                <asp:DropDownList ID="ddl_ProductTeam" runat="server" />        
                    <asp:Button ID="Add" runat="server" Text="Add" OnClick="btn_Add_Click" />
        </td>
    </tr>
</table>