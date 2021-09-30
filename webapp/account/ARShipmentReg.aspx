<%@ Page Language="C#" Theme="DefaultTheme" MaintainScrollPositionOnPostback="true"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="ARShipmentReg.aspx.cs" Inherits="com.next.isam.webapp.account.ARShipmentReg" Title="A/R Shipment Registration" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">A/R Shipment Registration</asp:Panel>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
function UpdateCount(obj)
{
    if (obj.checked)
    {
        document.getElementById("txt_Total").value = 1 + parseInt(document.getElementById("txt_Total").value);           
    }
    else
    {
           document.getElementById("txt_Total").value =  parseInt(document.getElementById("txt_Total").value) -1;           
    }
}
function ResetCount()
{
    document.getElementById("txt_Total").value =  0;           
}

function ShowMaxCount()
{
    document.getElementById("txt_Total").value = GetCheckBoxSelectedCount('ckb_Inv');          
}
function copyNSLInvoiceNo() {
    document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value.toUpperCase();
    document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvoiceNoFrom").value;
}
</script>
    <table width="800px">
    <tr><td >&nbsp;</td></tr>
</table>
<table width="550px">
    <tr>
        <td class="FieldLabel3" >
        <asp:Panel ID="pnl_reg" runat="server" DefaultButton="btn_Add"  CssClass="FieldLabel2">
            Invoice No.&nbsp;&nbsp;&nbsp;
        </asp:Panel>
        </td>
        <td>
            <asp:TextBox ID="txt_InvNo" runat="server"  />
            <asp:Button ID="btn_Add" runat="server" Text="Submit" OnClick="btn_Add_Click"
                OnClientClick="if (!isInvoiceNoValid(document.getElementById('ctl00_ContentPlaceHolder1_txt_InvNo').value)) {alert('Please enter a valid invoice number.');return false;}" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
                <table width="550">  
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="pnl_SearchHeader" runat="server" CssClass="header2">
                            <div style="padding:5px; vertical-align: middle;">             
                            <div style="float: left; vertical-align: middle;">
                                <asp:ImageButton ID="Image1" runat="server" ImageUrl="~/images/expand.jpg" AlternateText="(Show Details...)"/>
                            </div>
                            <div style="float: left;">&nbsp;&nbsp;Search</div>
                        </div>
                        </asp:Panel>
                        <asp:Panel ID="pnl_SearchCriteria" runat="server" CssClass="pnl">
                            <table style="margin-left:10px;"  width="500px"
                                cellpadding="5" cellspacing="0" >
                                <tr>
                                    <td class="FieldLabel2">Invoice Date</td>
                                    <td  colspan="3">
                                        <cc2:SmartCalendar ID="txt_InvDateFrom" ToDateControl="txt_InvDateTo" runat="server" />&nbsp;To&nbsp;<cc2:SmartCalendar ID="txt_InvDateTo" FromDateControl="txt_InvDateFrom" runat="server" />
                                    </td>

                                </tr>
                                <tr>
                                    <td class="FieldLabel2">Office</td>
                                    <td colspan="3">
                                        <cc2:SmartDropDownList ID="ddl_Office" runat="server" width="125px"/>
                                    </td>              
                                </tr> 
                                <tr>
                                    <td class="FieldLabel2" >Trading Agency</td>
                                    <td colspan="3">
                                        <cc2:SmartDropDownList ID="ddl_TradingAgency" runat="server" width="125px"/>
                                    </td>          
                               </tr>
                               <tr>
                                    <td class="FieldLabel2">Invoice No.</td>
                                    <td colspan="3"><asp:TextBox ID="txt_InvoiceNoFrom" runat="server" onblur="copyNSLInvoiceNo();" /> To <asp:TextBox ID="txt_InvoiceNoTo" runat="server" /></td>
                               </tr>
                                <tr>
                                    <td colspan="4"><asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="btn_Search_Click" />
                                        <asp:Button ID="btn_Reset" runat="server" Text="Reset" OnClick="btn_Reset_Click" />
                                    </td>
                                </tr>
                           </table>            
                        </asp:Panel>
                            <cc1:CollapsiblePanelExtender ID="cpe_Searching" runat="Server"
                                TargetControlID="pnl_SearchCriteria"
                                ExpandControlID="Image1"
                                CollapseControlID="Image1" 
                                Collapsed="false"                    
                                ImageControlID="Image1"    
                                ExpandedText="(Hide Details...)"
                                CollapsedText="(Show Details...)"
                                ExpandedImage="~/images/collapse.jpg"
                                CollapsedImage="~/images/expand.jpg"
                                SuppressPostBack="true" />

                    </td>
                </tr>
                </table>        

        </td>
    </tr>
     <tr>
        <td class="FieldLabel3" colspan="2">
            No. of Invoice Selected&nbsp;&nbsp;&nbsp;
            <input type="text" id="txt_Total" value="0" readonly="readonly" class="XSTextBox" />
        </td>
    </tr>
</table>

<br />
<asp:Panel ID="pnl_Result" runat="server" Visible="false" >
<asp:Label ID="lbl_Warning" runat="server" Visible="false" style="color:#ff9900; font-weight :bolder;" Text="Only the first 300 invoices can be displayed." /><br />
<br />
<asp:LinkButton ID="lnk_SelectAll" runat="server" Text="Select All" OnClientClick="CheckAll('ckb_Inv'); ShowMaxCount(); return false;" />&nbsp;&nbsp;&nbsp;
<asp:LinkButton ID="lnk_DeselectAll" runat="server" Text="Deselect All" OnClientClick="UncheckAll('ckb_Inv');ResetCount(); return false;" />&nbsp;&nbsp;&nbsp;
<asp:Button ID="btn_Submit" runat="server" Text="Submit" OnClick="btn_Submit_Click" />
    <asp:GridView ID="gv_Inv" runat="server" AutoGenerateColumns ="false" OnRowDataBound="InvDataBound">
    <Columns >
        <asp:TemplateField HeaderText ="Invoice Number">
            <ItemTemplate>
                <asp:CheckBox ID="ckb_Inv" runat="server" onclick="UpdateCount(this);"  />                
                <asp:Label ID="lbl_invNo" runat ="server" Text ='<%# Eval("InvoiceNo") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Invoice Date">
            <ItemTemplate >
                <asp:Label ID="lbl_InvDate" runat="server" Text='<%# Eval("InvoiceDate","{0:d}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supplier Name">
            <ItemTemplate >
                <asp:Label ID="lbl_SuppName" runat="server" Text='<%# Eval("Vendor.Name") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supp. Inv. No.">
            <ItemTemplate >
                <asp:Label ID="lbl_SuppInvNo" runat="server" Text='<%# Eval("SupplierInvoiceNo") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Contract No.">
            <ItemTemplate >
                <asp:Label ID="lbl_ContractNo" runat="server" Text='<%# Eval("ContractNo") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ccy">
            <ItemTemplate >
                <asp:Label ID="lbl_ccy" runat="server" Text='<%# Eval("SellCurrency.CurrencyCode") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  HeaderText="Inv. Amount">
            <ItemTemplate >
                <asp:Label ID="lbl_InvAmt" runat="server" Text='<%# Eval("InvoiceAmount","{0:#,##0.00}") %>' />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Right" />
        </asp:TemplateField>
    </Columns>    
    </asp:GridView>
</asp:Panel>
</asp:Content>
