<%@ Page Language="C#" AutoEventWireup="true" Theme="DefaultTheme" EnableEventValidation="false" CodeBehind="InvoiceRemarkGenerator.aspx.cs" Inherits="com.next.isam.webapp.shipping.InvoiceRemarkGenerator" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Invoice Remark Generator</title>
    <script type="text/javascript" >
        function getTotalQty()
        {
            var nodelist = document.getElementsByTagName("input");
            var iTotal = 0;
               for (i = 0; i <nodelist.length ; i++)
               {
                    if (nodelist[i].type == "text" )
                        if (nodelist[i].name.indexOf("txt_Quantity") != -1)
                            iTotal += parseInt(nodelist[i].value);
               }
               
               document.getElementById("lbl_TotalQty").value = iTotal ;
        }
        
        function getTotalPack()
        {
            var nodelist = document.getElementsByTagName("input");
            var iTotal = 0;
               for (i = 0; i <nodelist.length ; i++)
               {
                    if (nodelist[i].type == "text" )
                        if (nodelist[i].name.indexOf("txt_NoOfPack") != -1)
                            iTotal += parseInt(nodelist[i].value);                            
               }  
               
               document.getElementById("txt_TotalPack").value = iTotal ;
        }
        
    </script>
</head>
<body style="margin:20px 20px 20px 20px;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div>
        <table>
            <tr>
                <td>
                    <asp:GridView ID="gv_InvRemark" runat="server" AutoGenerateColumns="false" OnRowDataBound="InvRemarkDataBound" OnRowDeleting="RemarkRowDeleting" OnRowCommand="RemarkRowCommand">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="lnk_Delete" runat="server" ImageUrl="~/images/icon_delete.gif" CommandName="Delete" />
                                </ItemTemplate>
                                <HeaderTemplate >
                                    <asp:ImageButton ID="lnk_AddRow" runat="server" ImageUrl="../images/icon_newrequest.gif" CommandName="Add" />
                                </HeaderTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Contract">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt_ProductType" runat="server" Text='<%# Eval("Contract") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>                            
                            <asp:TemplateField HeaderText="Quantity">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt_Quantity" runat="server" MaxLength="5" SkinID="SmallTextBox"  Text='<%# Eval("Quantity") %>' />
                                    <asp:DropDownList ID="dd_QtyUnit" runat="server" SkinID="XSDDL">
                                        <asp:ListItem Text="PCS" Value="PCS" Selected="True" />
                                        <asp:ListItem Text="PACK" Value="PACK" />
                                        <asp:ListItem Text="SET" Value="SET" />
                                        <asp:ListItem Text="PAIR" Value ="PAIR" />
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="No. of Package">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt_NoOfPack" MaxLength="5" runat="server" SkinID="SmallTextBox"  Text='<%# Eval("NoOfPackage") %>' />
                                    <asp:DropDownList ID="ddl_PackUnit" runat="server" SkinID="XSDDL"  >
                                        <asp:ListItem Text="CTN" Value="CTN" Selected="True" />
                                    </asp:DropDownList> 
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <%--            <tr>
                <td class="header2" style="width:190px;text-align:right;">Total</td>
                <td style="width:80px;"><input type="text" id="lbl_TotalQty" size="10" class="nonEditableTextbox" readonly="readonly" value="0" /></td>
                <td style="width:80px;"><input type="text" id="txt_TotalPack" size="10" class="nonEditableTextbox" readonly="readonly"  value="0" /></td>
            </tr>--%>
            <tr>
                <td>
                    <asp:CustomValidator ID="CustomValidator1" OnServerValidate="ServerValidate" runat="server" 
                        ErrorMessage="Please provide correct spare part information."></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td>
                            <asp:Button ID="btn_Paste" runat="server" Text="Paste" onclick="btn_Paste_Click" />
                            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" onclick="btn_Cancel_Click" />
                </td>
            </tr>

        </table>
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>
    <table>
     </table>
    </form>
</body>
</html>
