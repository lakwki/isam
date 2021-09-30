<%@ Page Title="Non-Trade Expense - Debit Note" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme"  AutoEventWireup="true" CodeBehind="NonTradeRecharge.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NonTradeRecharge" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<asp:Panel ID="Panel1" runat="server" SkinID="sectionHeader_Accounts">Non-Trade Expense Recharge Debit Note Generation</asp:Panel>


<!-- popBox includes -->
<script src="../includes/popBox1.3.0.min.js" type="text/javascript"></script>
<link href="../includes/popBox1.3.0.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">
    var maxDescriptionLength = 3000;
    function initMaximumDescriptionLength() {
        //debugger;
        var hid;
        var objects = document.getElementsByTagName("input");
        for (i = 0; i < objects.length; i++)
            if ((objects[i].id).indexOf("hid_MaxDescLength") >= 0) {
                hid = objects[i];
                break;
            }
        hid.value=maxDescriptionLength;
    }

    function checkDescriptionMaxLength(tbox) {
        if (tbox.value.length == maxDescriptionLength) {
            tbox.value = tbox.value.substring(0,maxDescriptionLength-1);
            alert("Description cannot exceed " + maxDescriptionLength.toString() + " characters!");
            return false;
        }
    }
    var selectedValue = "";
    function getSelectedValue(checkbox) {
    //ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl02_cbx"
        prefix = checkbox.id.substring(0, 49);
        if (selectedValue=="")
        {   // initialize the selectedValue
            for (i = 2; ; i++) 
            {
                rowId = (i < 10 ? "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl0" + i : "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl" + i);
                if (prefix!=rowId+"_")
                {
                    if (cbx=document.getElementById(rowId + "_cbx")) {
                        if (cbx.checked)
                            selectedValue=document.getElementById(rowId + "_hf_Id").value
                    }
                    else
                        break;
                }
            }
        }

        if (selectedValue == "")
            selectedValue = document.getElementById(prefix + "hf_Id").value;
        else {
            if (checkbox.checked) {
                if (selectedValue != document.getElementById(prefix + "hf_Id").value) {
                    selectedValue = document.getElementById(prefix + "hf_Id").value;

                    for (i = 2; ; i++) {
                        controlId = (i < 10 ? "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl0" + i : "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl" + i) + "_hf_Id";

                        if (document.getElementById(controlId)) {
                            if (document.getElementById(controlId).value != selectedValue)
                                document.getElementById((i < 10 ? "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl0" + i : "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl" + i) + "_cbx").checked = false;
                        }
                        else
                            break;
                    }
                }
            }
            else {
                selectedValue = "";

                for (i = 2; ; i++) {
                    controlId = (i < 10 ? "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl0" + i : "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl" + i) + "_cbx";

                    if (document.getElementById(controlId)) {
                        if (document.getElementById(controlId).checked) {
                            selectedValue = document.getElementById((i < 10 ? "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl0" + i : "ctl00_ContentPlaceHolder1_gv_InvoiceDetail_ctl" + i) + "_hf_Id").value;
                            break;
                        }
                    }
                    else
                        break;
                }
            }
        }
    }

    function hideResultPanel()
    {
        pnl = document.getElementById('<%= pnl_Result.ClientID %>');
        if (pnl!=null)
            pnl.style.visibility = 'hidden';
            //pnl.style.display = 'none';
    }

$(document).ready(function () { 
<% 
    foreach(GridViewRow r in gv_InvoiceDetail.Rows)
    {
        if (r.RowType == DataControlRowType.DataRow)
        {
%>
            $('#<% =((TextBox)r.FindControl("txt_Description")).ClientID %>').popBox({width:580, newlineString: '\n' });  
<%
        }
    }

%>

}); 


    function openInvoiceWindow(invoiceId) {
            window.open('NonTradeInvoice.aspx?InvoiceId=' + invoiceId, 'NTInvoice', 'width=900,height=900, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();
    }


</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:HiddenField id="hid_MaxDescLength" runat="server" Value="0" />
<table>
    <tr>
        <td>
        <table width="800px" cellspacing="2" cellpadding="2">
            <tr>
                <td class="FieldLabel2" style="width:100px">&nbsp;Office&nbsp;</td>
                <td style="width:1px;">&nbsp;</td>
                <td style="width:100px;">
                    <cc2:SmartDropDownList ID="ddl_Office" OnSelectedIndexChanged="ddl_office_SelectedIndexChanged" runat="server"  AutoPostBack="true"/>
                </td>
                <td class="FieldLabel2" style="width:100px">&nbsp;Company&nbsp;</td>
                <td style="width:1px;">&nbsp;</td>
                <td style="width:300px;">
                    <cc2:SmartDropDownList ID="ddl_Company" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel2" style="width:100px">&nbsp;Debit / Credit Note Date&nbsp;</td>
                <td style="width:1px;">&nbsp;</td>
                <td colspan="4"><cc2:SmartCalendar ID="txtIssueDate" runat="server" /></td>
            </tr>
            <tr>
                <td colspan="6">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="6"><asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="Search_OnClick"  CausesValidation="false"/>
                    <asp:Button ID="btn_Reset" runat="server" Text="Reset" OnClick="Reset_OnClick"  CausesValidation="false"/>
                    <asp:Button ID="btn_Generate" runat="server" Text ="Generate D/C Note" SkinID="LButton" OnClick="Generate_OnClick" OnClientClick="hideResultPanel();"  />
                    <asp:Button ID="btn_Draft" runat="server" Text ="Print Draft Version" SkinID="LButton" OnClick="Draft_OnClick"  />
                </td>
            </tr>
        </table>
        </td>
    </tr>
    <tr>
        <td><asp:CustomValidator ID="valInvoiceDetail" runat="server" ErrorMessage="Fail in generate Debit Note:" OnServerValidate="valInvoiceDetail_OnServerValidate"/></td>
    </tr>
    <tr>
        <td>
            
            <asp:Label ID="lbl_ErrorMessage" runat="server" text="Error" style="color:Red; font-size:8pt; font-weight:bold;" visible="false" />
        </td>
    </tr>
    <tr>
        <td>
        <asp:Panel ID="pnl_Result" runat="server" Visible="true">
            <table style="border: 1px solid #B0B0B0;" cellpadding="0" cellspacing="0"><tr><td>
            <asp:GridView ID="gv_InvoiceDetail" runat="server" OnRowDataBound="gv_InvoiceDetail_DataBound" >
                <Columns>
                    <asp:TemplateField >
                        <ItemTemplate>
                            <asp:CheckBox id="cbx" runat="server" onclick="getSelectedValue(this);"   /><asp:HiddenField ID="hf_Id" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>                    
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label id="lbl_Name" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Attention">
                        <ItemTemplate>
                            <asp:TextBox ID="txt_Attention" runat="server" MaxLength="50" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Department" >
                        <ItemTemplate>
                            <cc2:SmartDropDownList ID="ddl_RechargeDept" runat="server" Visible="false"  />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Invoice No./<br>Customer No." ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Left" >
                        <ItemTemplate>
                            <asp:LinkButton ID="lnk_Invoice" runat="server" text=""/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Currency" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" >
                        <ItemTemplate>
                            <asp:Label ID="lbl_Currency" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Amount" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Right" >
                        <ItemTemplate >
                            <%# DataBinder.Eval(Container, "DataItem.Amount")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Recharge Currency" HeaderStyle-Width="80px" >
                        <ItemTemplate >
                        <cc2:SmartDropDownList ID="ddl_RechargeCurrency" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                            <asp:TextBox ID="txt_Description" runat="server" TextMode="MultiLine" skinId="TextBox_160" onkeydown="return checkDescriptionMaxLength(this);"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate >No record found.</EmptyDataTemplate>
            </asp:GridView>        
            </td></tr></table>
        </asp:Panel>
        </td>
    </tr>
</table>

<br /><br />
<script type="text/javascript">
    initMaximumDescriptionLength()
</script>
</asp:Content>
