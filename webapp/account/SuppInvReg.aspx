<%@ Page Language="C#" Theme="DefaultTheme" MaintainScrollPositionOnPostback="true"  MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="SuppInvReg.aspx.cs" Inherits="com.next.isam.webapp.account.SuppInvReg" Title="Supplier Invoice Registration for Payment" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="swc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Supplier Invoice Registration for Payment</asp:panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript">
function UpdateCount(obj, targetControl)
{
    if (obj.checked)
    {
        document.getElementById(targetControl).value = 1 + parseInt(document.getElementById(targetControl).value);           
        document.getElementById("txt_Total").value = 1 + parseInt(document.getElementById("txt_Total").value);           
    }
    else
    {
           document.getElementById(targetControl).value = parseInt(document.getElementById(targetControl).value) - 1;
           document.getElementById("txt_Total").value =  parseInt(document.getElementById("txt_Total").value) -1;           
    }
}
function ResetCount()
{
    document.getElementById("txt_DIR").value =  0;           
    document.getElementById("txt_ME").value =  0;           
    document.getElementById("txt_RET").value =  0;           
    document.getElementById("txt_JP").value =  0;           
    document.getElementById("txt_FRA").value =  0;       
    document.getElementById("txt_CH").value =  0;       
    document.getElementById("txt_Total").value =  0;           
}

function ShowMaxCount()
{
    document.getElementById("txt_DIR").value = <%= iDIRMax %>;           
    document.getElementById("txt_ME").value =  <%= iMEMax %>;           
    document.getElementById("txt_RET").value =  <%= iRETMax %>;           
    document.getElementById("txt_JP").value =  <%= iJPMax %>;           
    document.getElementById("txt_FRA").value =  <%= iFRAMax %>;   
    document.getElementById("txt_CH").value =  <%= iCHMax %>;      
    document.getElementById("txt_Total").value =  <%= iDIRMax+iMEMax+ iRETMax+iJPMax+iFRAMax+iCHMax %>;          
}

function isFormValid()
{
    if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvNo").value  == "" || !isInvoiceNoValid(document.getElementById("ctl00_ContentPlaceHolder1_txt_InvNo").value))
        return false;
    else
    {
        if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvNo").value.indexOf("/") == -1)
            return false;
        else if (document.getElementById("ctl00_ContentPlaceHolder1_txt_InvNo").value.indexOf("/") == document.getElementById("ctl00_ContentPlaceHolder1_txt_InvNo").value.lastIndexOf("/"))
            return false;
        return true;
    }
}

     function copyNSLInvoiceNo()
     {        
        document.getElementById("ctl00_ContentPlaceHolder1_txt_InvNoFrom").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvNoFrom").value.toUpperCase();
         document.getElementById("ctl00_ContentPlaceHolder1_txt_InvNoTo").value = document.getElementById("ctl00_ContentPlaceHolder1_txt_InvNoFrom").value;        
     }      
    
</script>
    <table width="800px">
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td colspan="5">
            <table width="800"> 
                <tr>
                    <td>
                    
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate >
                        <div id="div_InvReg" class="pnl_header">&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnk_reg" runat="server" Text="Invoice Registration" OnClick="lnk_reg_Click" CssClass="header2" /><br />
                            <div id="div_InvRegContent" runat="server" >
                   <table width="750px">
                       <tr>
                            <td class="FieldLabel2" style="width: 72px" >Invoice No.</td>
                            <td>
                            <asp:Panel ID="pnl_invReg" runat="server" DefaultButton="btn_Add">
                            <asp:TextBox ID="txt_InvNo" runat="server" />
                                <asp:Button ID="btn_Add" runat="server" Text="Submit" onclick="btn_Add_Click" OnClientClick="if (!isFormValid()) {alert('Please enter a valid invoice number.');return false;}" />
                            </asp:Panel>
                            </td>
                            <td class="FieldLabel2">Office Filter</td>
                            <td>
                                <swc:SmartDropDownList ID="ddl_OfficeFilter"  runat="server" />
                                <asp:Button ID="btn_Refresh" runat="server" Text="Refresh" 
                                    onclick="btn_Refresh_Click" /></td>
                        </tr>
                        <tr>
                            <td>&nbsp;<br /></td>
                        </tr>
                   </table>       
                   </div>                     
                        </div>
                        <div id="div_Search" class="pnl_header">&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnk_Search" runat="server" CssClass="header2" Text="Search" OnClick="lnk_Search_Click" /><br />
                            <div id="div_SearchContent" runat="server" visible="false">
                            <table style="margin-left:10px;" 
                                cellpadding="5" cellspacing="0" >
                                <tr>
                                    <td class="FieldLabel2_H2" style="width:50px;">Invoice Date</td>
                                    <td style="width:450px;">
                                        <swc:SmartCalendar ID="txt_InvDateFrom" ToDateControl="txt_InvDateTo" runat="server" />&nbsp;to&nbsp;<swc:SmartCalendar ID="txt_InvDateTo" FromDateControl="txt_InvDateFrom" runat="server" />
                                    </td>
                                    <td class="FieldLabel2_H2" style="width:50px;">Invoice No.</td>
                                    <td colspan ="5"><asp:TextBox ID="txt_InvNoFrom" runat="server" onblur = "copyNSLInvoiceNo();" />&nbsp;to&nbsp;
                                        <asp:TextBox ID="txt_InvNoTo" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2_H2" style="width:50px;">Invoice Application Date</td>
                                    <td>
                                        <swc:SmartCalendar ID="txt_UploadDateFrom" ToDateControl="txt_UploadDateTo" runat="server" />&nbsp;to&nbsp;<swc:SmartCalendar ID="txt_UploadDateTo" FromDateControl="txt_UploadDateFrom" runat="server" />
                                    </td>                        
                                    <td class="FieldLabel2_H2" style="width:50px;">Office</td>
                                    <td><swc:SmartDropDownList id="ddl_Office" runat="server" /></td>
                                    <td class="FieldLabel2_H2" style="width:50px;">Trading Agency</td>
                                    <td><swc:SmartDropDownList ID="ddl_TradingAgency" runat="server" /></td>
                                    <td class="FieldLabel2_H2" style="width:50px;">Purchase Term</td>
                                    <td><swc:SmartDropDownList ID="ddl_PurchaseTerm" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2_H2" style="width:50px">Scanned Date</td>
                                    <td>
                                        <swc:SmartCalendar ID="txt_ScannedDateFrom" ToDateControl="txt_ScannedDateTo" runat="server" />&nbsp;to&nbsp;<swc:SmartCalendar 
                                        ID="txt_ScannedDateTo" FromDateControl="txt_ScannedDateFrom" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FieldLabel2_H2">Invoice Status</td>
                                    <td colspan="5">
                                        <asp:RadioButton  ID="rad_All" runat="server" Text="ALL&nbsp;&nbsp;" Checked="true" GroupName="InvStatus"  />
                                        <asp:RadioButton ID="rad_Outstanding" runat="server"  Text="Outstanding&nbsp;&nbsp;" GroupName="InvStatus" />
                                        <asp:RadioButton ID="rad_Registered" runat="server" Text="Registered&nbsp;&nbsp;" GroupName="InvStatus" />
                                        <asp:RadioButton ID="rad_Interfaced" runat="server" Text="Interfaced to SUN&nbsp;&nbsp;" GroupName="InvStatus" />                            
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="8"><asp:Button ID="btn_Search" runat="server" Text="Search" 
                                            onclick="btn_Search_Click" />
                                        <asp:Button ID="btn_Reset" runat="server" OnClick="btn_Reset_Click" Text="Reset" />
                                    </td>
                                </tr>
                           </table>
                            </div>
                        </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btn_Add" />
                            <asp:PostBackTrigger ControlID="btn_Refresh" />
                            <asp:PostBackTrigger ControlID="btn_Search" />
                            <asp:PostBackTrigger ControlID="btn_Reset" />
                        </Triggers>
                        </asp:UpdatePanel>
                    
                    </td>
                </tr> 

                <tr>
                    <td></td>
                </tr>
                </table>        
        </td>
    </tr>
    <tr>
        <td class="FieldLabel3">No. of Invoice Selected:</td>
        <td width="250">DIR&nbsp;&nbsp;<input type="text" value="0" class="XSTextBox" id="txt_DIR" readonly="readonly" />&nbsp;&nbsp;&nbsp;
                FRA&nbsp;&nbsp;<input type="text" value="0" class="XSTextBox" id="txt_FRA" readonly="readonly"  />&nbsp;&nbsp;&nbsp;
                RET&nbsp;&nbsp;<input type="text" value="0" class="XSTextBox" id="txt_RET" readonly="readonly" />&nbsp;&nbsp;&nbsp;<br />
                &nbsp;CH&nbsp;&nbsp;<input type="text" value="0" class="XSTextBox" id="txt_CH" readonly="readonly" />&nbsp;&nbsp;&nbsp;
                &nbsp;&nbsp;&nbsp;JP&nbsp;&nbsp;<input type="text" value="0" class="XSTextBox" id="txt_JP" readonly="readonly" />&nbsp;&nbsp;&nbsp;
                &nbsp;ME&nbsp;&nbsp;<input type="text" value="0" class="XSTextBox" id="txt_ME" readonly="readonly" />&nbsp;&nbsp;&nbsp;
        </td>
        <td width="400" colspan="2">
            Total&nbsp;&nbsp;<input type="text" value="0" class="XSTextBox" id="txt_Total" readonly="readonly" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel3">No. of Scanned Invoice:</td>
        <td><asp:Label ID="lbl_ScannedCount" runat="server" Text="0"/></td>
    </tr>
</table>
<br />
<asp:Panel ID="pnl_Result" runat="server" visible="false">
&nbsp;&nbsp;
<asp:ImageButton id="lnk_SelectAll" runat="server" ImageUrl="../images/icon_selectall.jpg" AlternateText="Select All" OnClientClick="CheckAll('ckb_inv');ShowMaxCount(); return false;" />
&nbsp;&nbsp;
<asp:ImageButton id="lnk_DeSelectAll" runat="server" ImageUrl="../images/icon_deselectall.jpg" AlternateText="De-Select All" OnClientClick="UncheckAll('ckb_inv');ResetCount(); return false;" />
    &nbsp;
    <asp:Button ID="btn_Submit" runat="server" Text="Submit" 
        onclick="btn_Submit_Click" />&nbsp;&nbsp;&nbsp;<asp:Label ID="lbl_result" runat="server" Visible="false" />
<asp:GridView ID="gv_Inv" runat="server" AutoGenerateColumns="false" OnRowDataBound="InvDataBound" AllowSorting="True">
    <Columns>
        <asp:TemplateField HeaderText="Trading Agency">
            <ItemTemplate >
                <asp:CheckBox ID="ckb_inv" runat="server" />
                <asp:Label ID="lbl_TradingAgency" runat="server" Text='<%# Eval("TradingAgency.ShortName") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Invoice Number">
            <ItemTemplate>
                <asp:Label ID="lbl_InvoiceNumber" runat="server" Text='<%# Eval("InvoiceNo") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Seq No">
            <ItemTemplate>
                <asp:Label ID="lbl_SeqNo" runat="server" Text='<%# Eval("SequenceNo") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Invoice Date">
            <ItemTemplate >
                <asp:Label ID="lbl_InvoiceDate" runat="server" Text='<%# Eval("InvoiceDate","{0:d}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supplier Name">
            <ItemTemplate >
                <asp:Label ID="lbl_SuppName" runat="server" Text='<%# Eval("Vendor.Name") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supp. SUN COA" >
            <ItemTemplate >
                <asp:Label ID="lbl_SuppSUNCOA"  runat="server" Text='<%# Eval("Vendor.SunAccountCode") %>'  />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Prod. Team" >
            <ItemTemplate >
                <asp:Label ID="lbl_ProdTeam" runat="server" Text='<%# Eval("ProductTeam.Code") %>' />
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
            <ItemTemplate>
                <asp:Label ID="lbl_Ccy" runat="server" Text='<%# Eval("BuyCurrency.CurrencyCode") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Inv. Amount">
            <ItemTemplate>
                <asp:Label ID="lbl_InvAmt" runat="server" Text='<%# Eval("InvoiceAmount", "{0:#,##0.00}") %>' />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Right" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Supp. Inv. Amount">
            <ItemTemplate >
                <asp:Label ID="lbl_SuppInvAmt" runat="server" Text='<%# Eval("TotalShippedSupplierGarmentAmount", "{0:#,##0.00}") %>' />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Right" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Purc. Doc Recd. Date by Acc">
            <ItemTemplate >
                <asp:Label ID="lbl_DocRecDateByAcc" runat="server" Text='<%# Eval("PurchaseScanDate","{0:d}") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField  HeaderText="SUN Interface Extract Date">
            <ItemTemplate >
                <asp:Label ID="lbl_ExtractDate" runat="server"  />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Scan User Name">
            <ItemTemplate>
                <asp:Label ID="lbl_ScanUserID" runat="server" Text='<%# Eval("PurchaseScanBy.DisplayName") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Scan Date">
            <ItemTemplate>
                <asp:Label ID="lbl_ScanDate" runat="server" Text='<%# Eval("PurchaseScanDate","{0:d}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate >Record not found.&nbsp;&nbsp;</EmptyDataTemplate>
</asp:GridView>
</asp:Panel>
</asp:Content>
