<%@ Page Language="C#" Theme="DefaultTheme" AutoEventWireup="true" CodeBehind="ExportLicenceCopy.aspx.cs" Inherits="com.next.isam.webapp.shipping.ExportLicenceCopy" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Custom Document Copy</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table width="750px" >
        <tr>
            <td class="tableHeader" ><asp:Label ID="lbl_Title" runat="server" Text="Custom Document Copy" /></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
    </table>
    <asp:UpdatePanel runat="server" ID="up_Doc">
    <ContentTemplate>
    <table width="750">
        <tr>
            <td></td>
            <td class="FieldLabel2">Contract No.</td>
            <td class="FieldLabel2">Dly No.</td>
            <td class="FieldLabel2">PO Qty</td>
            <td class="FieldLabel2">Ship Method</td>            
        </tr>
        <tr>
            <td class="FieldLabel2">From:</td>
            <td><asp:TextBox ID="txt_FromContract" runat="server" ReadOnly="true" CssClass="nonEditableTextbox" /></td>
            <td><asp:TextBox ID="txt_FromDlyNo" runat="server" ReadOnly="true" CssClass="nonEditableTextbox" /></td>            
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="4">
                <asp:GridView ID="gv_source" runat="server" AutoGenerateColumns="false"  Width="550">
                    <Columns >
                        <asp:TemplateField HeaderText="Document No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_CONo" runat="server" Text='<%# Eval("DocumentNo") %>'  />
                            </ItemTemplate>                
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="C/O">
                            <ItemTemplate >
                                <asp:Label ID="lbl_CO" runat="server" Text='<%# Eval("Country.Name") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Expiry Date">
                            <ItemTemplate >
                                <asp:Label ID="lbl_ExpiryDate" runat="server" Text='<%# Eval("ExpiryDate","{0:d}") == "01/01/0001" ? "": Eval("ExpiryDate","{0:d}")  %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty on Doc">
                            <ItemTemplate >
                                <asp:Label ID="lbl_City" runat="server" Text='<%# Eval("Qty","{0:#,###.##}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty for Order">
                            <ItemTemplate>
                                <asp:Label ID="lbl_CityOrder" runat="server" Text='<%# Eval("OrderQty","{0:#,###.##}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Eqv. PO Qty">
                            <ItemTemplate>
                                <asp:Label ID="lbl_POCity" runat="server" Text='<%# Eval("POQty","{0:#,###.##}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>        
                </asp:GridView>            
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="FieldLabel2">To:</td>
            <td><asp:TextBox ID="txt_ToContract" runat="server" />
                <asp:Button ID="btn_ViewDetail" runat="server" onclick="btn_ViewDetail_Click" 
                    Text="Search" />
            </td>
            <td><cc2:SmartDropDownList ID="ddl_DlyNo" runat="server" 
                    onselectedindexchanged="ddl_DlyNo_SelectedIndexChanged" AutoPostBack="True" /></td>
            <td><asp:TextBox ID="txt_POQty" runat="server" /></td>
            <td><asp:TextBox ID="txt_ShipMethod" runat="server" /></td>
            <td>&nbsp;</td>
        </tr>        
        <tr>
            <td>&nbsp;</td>
            <td colspan="4">
                <asp:GridView ID="gv_Licence" runat="server" AutoGenerateColumns="false"  Width="550">
                    <Columns >
                        <asp:TemplateField HeaderText="Document No.">
                            <ItemTemplate>
                                <asp:Label ID="lbl_CONo" runat="server" Text='<%# Eval("DocumentNo") %>'  />
                            </ItemTemplate>                
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="C/O">
                            <ItemTemplate >
                                <asp:Label ID="lbl_CO" runat="server" Text='<%# Eval("Country.Name") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Expiry Date">
                            <ItemTemplate >
                                <asp:Label ID="lbl_ExpiryDate" runat="server" Text='<%# Eval("ExpiryDate","{0:d}") == "01/01/0001" ? "": Eval("ExpiryDate","{0:d}")  %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty on Doc">
                            <ItemTemplate >
                                <asp:Label ID="lbl_City" runat="server" Text='<%# Eval("Qty","{0:#,###.##}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty for Order">
                            <ItemTemplate>
                                <asp:Label ID="lbl_CityOrder" runat="server" Text='<%# Eval("OrderQty","{0:#,###.##}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Eqv. PO Qty">
                            <ItemTemplate>
                                <asp:Label ID="lbl_POCity" runat="server" Text='<%# Eval("POQty","{0:#,###.##}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>        
                </asp:GridView>
            </td>
        </tr>
    </table>
    <br />
        <asp:CustomValidator ID="valCustom" runat="server" OnServerValidate="serverValidator" ControlToValidate="txt_FromDlyNo"></asp:CustomValidator>
        <br />
    <br />
    <asp:Button ID="btn_Submit" runat="server" Text="Submit"  onclick="btn_Submit_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
    </ContentTemplate>
    </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
