<%@ Page Title="ISAM - Other Charges Debit/Credit Note" Language="C#" MasterPageFile="~/MasterPage/MainMaster.Master" Theme="DefaultTheme" AutoEventWireup="true"
    CodeBehind="OtherChargesDebitNote.aspx.cs" Inherits="com.next.isam.webapp.account.OtherChargesDebitNote" %>

<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_ImageHeader" runat="server">
    <!--
<img src="../images/banner_account_mockshopdebitnote.gif" runat="server" id="imgHeaderText" />
<img src="../images/banner_workplace.gif" runat="server" id="img1" />
-->
    <asp:Panel ID="Panel1" runat="server" SkinID="SectionHeader_Accounts">Other Charges Debit / Credit Note</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- popBox includes -->
    <script src="../includes/popBox1.3.0.min.js" type="text/javascript"></script>
    <link href="../includes/popBox1.3.0.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        function isValid() {
            if (document.getElementById("ctl00_ContentPlaceHolder1_txt_DebitNoteDate_txt_DebitNoteDate_textbox").value == "") {
                alert("Please enter debit / credit note date.");
                return false;
            }
            else if (document.getElementById("ctl00_ContentPlaceHolder1_ddlRechargeType").value == -1) {
                alert("Please select Recharge Type");
                return false;
            }
            return true;
        }

        $(document).ready(function () {
<% 
        foreach (GridViewRow r in gv_excel.Rows)
        {
            if (r.RowType == DataControlRowType.DataRow)
            {
%>
            $('#<% =((TextBox)r.FindControl("txt_Description")).ClientID %>').popBox({ width: 580, newlineString: '\n' });  
            <%
            }
        }
%>
<% 
        foreach (GridViewRow r in gv_excel.Rows)
        {
            if (r.RowType == DataControlRowType.DataRow)
            {
%>
            $('#<% =((TextBox)r.FindControl("txt_Attention")).ClientID %>').popBox({ width: 580, newlineString: '\n' });  
            <%
            }
        }
%>
        });
    </script>
    <br />
    <table width="800px">
        <tr>
            <td style="width: 120px" class="FieldLabel2">Office</td>
            <td>
                <cc1:SmartDropDownList ID="ddl_Office" runat="server" /></td>
        </tr>
        <tr>
            <td class="FieldLabel2">Doc Type</td>
            <td style="width: 400px">
                <asp:DropDownList ID="ddlDocType" AutoPostBack="True" AppendDataBoundItems="true" runat="server" Width="200px">
                    <asp:ListItem Text="Debit Note" Value="0" />
                    <asp:ListItem Text="Credit Note" Value="1" />
                </asp:DropDownList>
            </td>
            <td style="width: 250px" align="right"><a href="OTHER CHARGES DNCN UPLOAD FILE.xlsx"><b>Other Charge DN/CN Upload Template</b></a></td>
        </tr>
        <tr>
            <td class="FieldLabel2">Issue DN/CN Note Date</td>
            <td>
                <cc1:SmartCalendar ID="txt_DebitNoteDate" runat="server" AutoPostBack="true" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Issued To</td>
            <td>
                <cc1:SmartDropDownList ID="ddlIssuedTo" runat="server" AutoPostBack="true" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Recharge Type</td>
            <td>
                <cc1:SmartDropDownList ID="ddlRechargeType" runat="server" AutoPostBack="true" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Inter-co office</td>
            <td>
                <cc1:SmartDropDownList ID="ddl_PaymentOffice" runat="server" Width="120px" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Upload File</td>
            <td>
                <asp:FileUpload ID="uplFile" runat="server" />
                <asp:Button runat="server" ID="btnLoadExcel" Text="Upload" CssClass="btn" CausesValidation="false" OnClick="btn_upload" SkinID="LButton" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel2">Auto-Upload to Epicor</td>
            <td>
                <asp:CheckBox ID="chbEpicor" runat="server" Checked="false" />
            </td>
        </tr>
    </table>
    <br />

    <asp:Label runat="server" ID="lbl_errorMsg" ForeColor="Red" AutoPostBack="true" SkinID="AlertLabel"></asp:Label>
    <asp:GridView ID="gv_excel" runat="server" OnRowDataBound="gv_excel_RowDataBound">
        <Columns>

            <asp:TemplateField HeaderText="" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:CheckBox ID="chb_check" runat="server" Checked="true" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Id" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.ID")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Name" ControlStyle-Width="120px" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.Name")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Name (System)" ControlStyle-Width="120px" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <%# DataBinder.Eval(Container, "DataItem.VPSName")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Attention" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <asp:TextBox ID="txt_Attention" runat="server" TextMode="MultiLine" SkinID="TextBox_160" Text='ACCOUNTS DEPT' />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Description" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <asp:TextBox ID="txt_Description" runat="server" TextMode="MultiLine" SkinID="TextBox_160" Text='<%# DataBinder.Eval(Container, "DataItem.Description")%>' />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Original Ccy" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:Label ID="lbl_OriCurrency" runat="server" />
                    <%# DataBinder.Eval(Container, "DataItem.OriCurrencyCode")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Amount" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:Label ID="lbl_OtherAmt" runat="server" />
                    <%# DataBinder.Eval(Container, "DataItem.Amount")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Billing Ccy" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:Label ID="lbl_BilCurrency" runat="server" />
                    <%# DataBinder.Eval(Container, "DataItem.BilCurrencyCode")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Billing Amt" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <asp:TextBox ID="txt_BillingAmt" runat="server" TextMode="SingleLine" SkinID="TextBox_95" Text='<%# DataBinder.Eval(Container, "DataItem.BilAmount")%>' />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Ex Rate" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:Label ID="lbl_ExRate" runat="server" />
                    <%# DataBinder.Eval(Container, "DataItem.ExchangeRate")%>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Vendor Office" HeaderStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top">
                <ItemTemplate>
                    <asp:Label ID="lbl_VendorOffice" runat="server" />
                    <%# DataBinder.Eval(Container, "DataItem.VendorOffice")%>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>
    <br />
    <asp:Button ID="btn_Submit" runat="server" Text="Issue DN/CN" OnClientClick="return isValid();" OnClick="btn_Submit_Click" SkinID="LButton" Visible="false" />



</asp:Content>
