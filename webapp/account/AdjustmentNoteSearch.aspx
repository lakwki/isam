<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="AdjustmentNoteSearch.aspx.cs" Inherits="com.next.isam.webapp.account.AdjustmentNoteSearch" Title="Adjustment Note Search" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <!--
    <img src="../images/banner_workplace.gif" runat="server" id="imgHeaderText" />
-->
    <asp:Panel runat="server" SkinID="sectionHeader_Accounts">Debit / Credit Note Search</asp:Panel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- popBox includes -->
<script src="../includes/popBox1.3.0.min.js" type="text/javascript"></script>
<link href="../includes/popBox1.3.0.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">
    var maxDescriptionLength = 3000;
    function initMaximumDescriptionLength() {
        var hid;
        var objects = document.getElementsByTagName("input");
        for (i = 0; i < objects.length; i++)
            if ((objects[i].id).indexOf("hid_MaxDescLength") >= 0) {
                hid = objects[i];
                break;
            }
        hid.value = maxDescriptionLength;
    }

    function checkDescriptionMaxLength(tbox) {
        if (tbox.value.length == maxDescriptionLength) {
            tbox.value = tbox.value.substring(0, maxDescriptionLength - 1);
            alert("Description cannot exceed " + maxDescriptionLength.toString() + " characters!");
            return false;
        }
    }

    function showProgressBarX() {
        var left = window.screen.availWidth / 3 - 100;
        var top = window.screen.availHeight / 3 - 100

        waitDialog = window.showModelessDialog("../common/ProgressBarX.htm", window, "dialogHeight: 30px; dialogWidth: 200px; center:yes; edge: sunken; center: Yes; help: No; resizable: yes; status: No; scroll: No; dialogLeft:" + left + "px; dialogTop:" + top + "px;");
    }

    $(document).ready(function () { 
    <% 
        foreach(GridViewRow r in gv_Request.Rows)
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

</script>
<asp:HiddenField id="hid_MaxDescLength" runat="server" Value="0" />
<table cellpadding="2px">
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:120px;">Office</td>
        <td><cc2:smartdropdownlist id="ddl_Office" runat="server" Width="200px"></cc2:smartdropdownlist></td>
    </tr>
    <tr>
        <td class="FieldLabel2">Document Type</td>
        <td><cc2:smartdropdownlist id="ddl_AdjustmentType" runat="server" Width="200px"></cc2:smartdropdownlist></td>
    </tr>
    <tr runat="server">
        <td class="FieldLabel2" style="vertical-align:top;background-position: top;">Issue Date</td>
        <td>
            <asp:RadioButton ID="radMonth" runat="server" GroupName="Date" Text="Month"/>&nbsp;
            <cc2:smartdropdownlist id="ddl_Year" runat="server" Width="80px"></cc2:smartdropdownlist>&nbsp;
            <cc2:smartdropdownlist id="ddl_Month" runat="server" Width="80px"/>
            <br />
            <asp:RadioButton ID="radPeriod" runat="server" GroupName="Date" Text="Period"/>&nbsp;
            <cc2:smartdropdownlist id="ddl_FiscalYear" runat="server" Width="80px"></cc2:smartdropdownlist>&nbsp;
            <cc2:smartdropdownlist id="ddl_Period" runat="server" Width="80px"/>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <asp:Button ID="btn_Search" runat="server" Text="Search" onclick="btn_Search_Click" />&nbsp;&nbsp;<asp:Button ID="btn_Send" runat="server" Text="Send to Supplier" Visible="false" OnClick="btn_Send_Click" SkinID="LButton" />
        </td>
    </tr>
</table>
<br /><br />
<asp:GridView ID="gv_Request" runat="server" AutoGenerateColumns="false" 
        onrowdatabound="gv_Request_RowDataBound" 
        onrowcommand="gv_Request_RowCommand">
    <Columns>
        <asp:TemplateField HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top" Visible="false">
            <HeaderTemplate>
                <asp:CheckBox ID="chb_All" runat="server" OnCheckedChanged="CheckBoxAll_CheckedChanged" AutoPostBack="true" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chb_Mail" runat="server" CommandName="SendEmail" Visible="false" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:LinkButton ID="lnk_Excel" runat="server" CommandName="OutputExcel"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="DR/CR Note No." HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:LinkButton ID="lnk_AdjustmentNoteNo" runat="server" CommandName="OutputPDF"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Adjustment Type" ControlStyle-Width="120px" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label ID="lbl_AdjustmentType" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Office" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label ID="lbl_Office" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="IssueDate" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top" ControlStyle-Width="100px">
            <ItemTemplate>
                <asp:Label ID="lbl_IssueDate" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Issued To" ControlStyle-Width="200px" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:Label ID="lbl_IssuedTo" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Currency" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label ID="lbl_Currency" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total Amt" ControlStyle-Width="100px" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Label ID="lbl_TotalAmt" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:ImageButton runat="server" ID="lnk_Attachment" ToolTip="View Email Attachment" ImageUrl="~/images/pdf.gif"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:TextBox ID="txt_Description" runat="server" TextMode="MultiLine" skinId="TextBox_160" onkeydown="return checkDescriptionMaxLength(this);" Visible="false" ToolTip="Type here for custom Remark, leave it empty for default remark"/><br />
                <asp:LinkButton runat="server" ID="lnk_UpdateCustomRemark" CommandName="UpdateCustomRemark" Text="Update" Visible="false"/>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top" ControlStyle-Width="100px">
            <ItemTemplate>
                <asp:LinkButton runat="server" ID="lnk_SendUKClaimDCNote" CommandName="SendUKClaimDCNote" Visible="false"/>
                <asp:LinkButton runat="server" ID="lnk_SendPurchaseDCNote" CommandName="SendPurchaseDCNote" Visible="false"/>
                <asp:LinkButton runat="server" ID="lnk_SendAdvancePaymentDCNote" CommandName="SendAdvancePaymentInterestDCNote" Visible="false"/>
                <asp:LinkButton runat="server" ID="lnk_SendGenericDCNote" CommandName="SendGenericDCNote" Visible="false"/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Button ID="btn_VoidDN" Text="Void" runat="server" CausesValidation="false" Enabled="true" Visible="false" ToolTip="Void the D/N in case the vendor failed to settle, &#013;the system will settle it via NS Cost" />
                <asp:Button ID="btn_VoidOtherChargeDN" Text="Void" runat="server" CausesValidation="false" Enabled="true" Visible="false" ToolTip="Void the D/N in case the vendor failed to settle, &#013;the system will settle it via NS Cost" CommandName="VoidOtherChargeDN" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
            <ItemTemplate>
                <asp:Button ID="btn_ChangeCurrency" Text="Change Currency" CommandName="ChangeCurrency" runat="server" CausesValidation="false" SkinID="XLButton" Enabled="true" Visible="false" ToolTip="Change Currency" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>   

<script type="text/javascript">
    initMaximumDescriptionLength()
</script>

</asp:Content>