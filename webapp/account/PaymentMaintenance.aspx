<%@ Page Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="PaymentMaintenance.aspx.cs" Inherits="com.next.isam.webapp.account.PaymentMaintenance" Title="Payment Maintenance" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_accounts_receipt_settlement.gif" runat="server" id="imgHeaderText" />
<img src="../images/banner_workplace.gif" runat="server" />
-->
<asp:Panel runat="server" SkinID="SectionHeader_Accounts">Receipt And Settlement Maintenance</asp:Panel>
</asp:Content>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" >
var gridPayment;
var gridDNPayment;
var paymentTotalRowPrefix;
var dnPaymentTotalRowPrefix;


function isValid()
{
    if (document.getElementById("<%= txt_NSLInvNo.ClientID %>").value.indexOf("/") != -1)
    {
        if (!isInvoiceNoValid(document.getElementById("<%= txt_NSLInvNo.ClientID %>").value))
        {
            alert("Invalid Invoice No.");
            return false;
        }
    }
    else if (document.getElementById("<%= txt_NSLInvNo.ClientID %>").value.indexOf("-") != -1)
    {
        if (!isValidPONo(document.getElementById("<%= txt_NSLInvNo.ClientID %>").value))
        {
            alert("Invalid PO No.");
            return false;
        }
    }
    //else
    //{
    //    window.alert('Please enter an valid invoice no. or split shipment PO no.');
    //    return false
    //}
            
    return true;
}


function isValidPONo(poNo)
{
    if (isNaN(parseInt(poNo.substring(poNo.length-1, poNo.length))))
        return false;
   
    if (poNo.substring(poNo.length - 2, poNo.length - 1) != "-")
        return false;
        
    return true;
}


function checkAmount(gridControl, target) {
    gridId = getGridId(gridControl);
    controlId = gridControl.id.replace(gridId+'_','');
    rowId = controlId.substring(0, controlId.indexOf('_'));
    targetControl = document.getElementById(gridId + '_' + rowId + '_' + target);
    if (targetControl != null)
        if (targetControl.value == "" || targetControl.value == "0")
            alert("The amount is 0.");
}

function checkValidDate(control) {
    //Check date input is later than today or not. If yes, alert and restore default value.
    if (!control.value || !isDateValid(control.value))
        return;

    var dateArray = control.value.split("/");
    var inputDate = new Date(Number(dateArray[2]), Number(dateArray[1]) - 1, Number(dateArray[0]));

    if (inputDate > new Date()) {
        control.value = control.defaultValue;
        alert("The Receipt Date should not be later than today.");
    }
}


function updateTotal(ckb) {
    var amt;
    var sign = (ckb.checked ? 1 : -1);
    var totalControl;

    gridId = gridPayment.id;
    prefix = paymentTotalRowPrefix;
    controlId = ckb.id.replace(gridId+'_','');
    rowPrefix = gridId + '_' + controlId.substring(0, controlId.indexOf('_')) + '_' ;
    if ((totalControl=document.getElementById(prefix + 'lbl_TotalSalesAmt')) != null) 
        totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(document.getElementById(rowPrefix + "lbl_SalesAmt").innerText) * sign).toFixed(2);
    if ((totalControl=document.getElementById(prefix + 'lbl_TotalPurchaseAmt')) != null) 
        totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(document.getElementById(rowPrefix + "lbl_PurchaseAmt").innerText) * sign).toFixed(2);
    if ((totalControl=document.getElementById(prefix + 'lbl_TotalSalesCommAmt')) != null)
        totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(document.getElementById(rowPrefix + "lbl_SalesCommAmt").innerText) * sign).toFixed(2);
    if ((totalControl=document.getElementById(prefix + 'lbl_TotalARAmt')) != null)
        totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(document.getElementById(rowPrefix + "txt_ARAmt").value) * sign).toFixed(2);
    if ((totalControl=document.getElementById(prefix + 'lbl_TotalAPAmt')) != null)
        totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(document.getElementById(rowPrefix + "txt_APAmt").value) * sign).toFixed(2);
    if ((totalControl=document.getElementById(prefix + 'lbl_TotalCommAmt')) != null)
        totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(document.getElementById(rowPrefix + "txt_commAmt").value) * sign).toFixed(2);
    counter = document.getElementById('<%=lbl_TotalSelected.ClientID %>');
    if (counter!=null)
        counter.innerText = parseInt(counter.innerText) + 1 * sign;
    return;
}


function ResetTotal() {   
    var control;
    var counter;
    if (gridPayment != null)
    {
        if ((control = document.getElementById(paymentTotalRowPrefix + 'lbl_TotalSalesAmt')) != null)
            control.innerText = 0;
        if ((control = document.getElementById(paymentTotalRowPrefix + 'lbl_TotalPurchaseAmt')) != null)
            control.innerText = 0;
        if ((control = document.getElementById(paymentTotalRowPrefix + 'lbl_TotalSalesCommAmt')) != null)
            control.innerText = 0;
        if ((control = document.getElementById(paymentTotalRowPrefix + 'lbl_TotalARAmt')) != null)
            control.innerText = 0;
        if ((control = document.getElementById(paymentTotalRowPrefix + 'lbl_TotalAPAmt')) != null)
            control.innerText = 0;
        if ((control = document.getElementById(paymentTotalRowPrefix + 'lbl_TotalCommAmt')) != null)
            control.innerText = 0;
    }
    if (gridDNPayment != null)
    {
        if ((control = document.getElementById(dnPaymentTotalRowPrefix + 'lbl_TotalDNAmt')) != null)   
            control.innerText = 0;
    }
        
    if ((counter = document.getElementById('<%= lbl_TotalSelected.ClientID %>'))!=null)
        counter.innerText = 0;
    if ((counter = document.getElementById('<%= lbl_TotalDNSelected.ClientID %>'))!=null)
        counter.innerText = 0;
}


function getTotalAmt() {
    ttlSalesAmt = 0;
    countedTable = 0;
    footerPrefix = "";
    gridId = "";
    if (gridPayment != null)
    {
        var selectedTotalSalesAmt=0;
        var selectedTotalPurchaseAmt=0;
        var selectedTotalSalesCommAmt=0;
        var selectedTotalARAmt=0;
        var selectedTotalAPAmt=0;
        var selectedTotalCommAmt=0;
        var selectedTotalDNAmt=0;
        var prefix, i;
        var amt;
        var control;
        var nodeList = gridPayment.getElementsByTagName('input');

        for (i = 0; i < nodeList.length; i++)
            if (nodeList[i].id.indexOf("ckb_update") > 0) {
                if (nodeList[i].checked) {
                    prefix = nodeList[i].id.replace("ckb_update","");
                    if ((control=document.getElementById(prefix+'lbl_SalesAmt')) != null)
                        selectedTotalSalesAmt += Math.round(textToFloat(control.innerText)*100,2)/100 ;
                    if ((control=document.getElementById(prefix+'lbl_PurchaseAmt')) != null)
                        selectedTotalPurchaseAmt += Math.round(textToFloat(control.innerText)*100,2)/100 ;
                    if ((control=document.getElementById(prefix+'lbl_SalesCommAmt')) != null)
                        selectedTotalSalesCommAmt += Math.round(textToFloat(control.innerText)*100,2)/100 ;

                    if ((control=document.getElementById(prefix+'txt_APAmt')) != null)
                        selectedTotalAPAmt += Math.round(textToFloat(control.value)*100,2)/100 ;
                    if ((control=document.getElementById(prefix+'txt_ARAmt')) != null)
                        selectedTotalARAmt += Math.round(textToFloat(control.value)*100,2)/100;
                    if ((control=document.getElementById(prefix+'txt_commAmt')) != null)
                        selectedTotalCommAmt += Math.round(textToFloat(control.value)*100,2)/100;
                }
            }
        footerPrefix = paymentTotalRowPrefix;
        if ((control=document.getElementById(footerPrefix + 'lbl_TotalSalesAmt')) != null)
            control.innerText = selectedTotalSalesAmt.toLocaleString();
        if ((control=document.getElementById(footerPrefix + 'lbl_TotalPurchaseAmt')) != null)
            control.innerText = selectedTotalPurchaseAmt.toLocaleString();
        if ((control=document.getElementById(footerPrefix + 'lbl_TotalSalesCommAmt')) != null)
            control.innerText = selectedTotalSalesCommAmt.toLocaleString();
        if ((control=document.getElementById(footerPrefix + 'lbl_TotalARAmt')) != null)
            control.innerText = selectedTotalARAmt.toLocaleString();
        if ((control=document.getElementById(footerPrefix + 'lbl_TotalAPAmt')) != null)
            control.innerText = selectedTotalAPAmt.toLocaleString();
        if ((control=document.getElementById(footerPrefix + 'lbl_TotalCommAmt')) != null)
            control.innerText = selectedTotalCommAmt.toLocaleString();
        }
                
    if (gridDNPayment != null)
    {
        var selectedTotalDNAmt = 0;
        var nodeList = gridDNPayment.getElementsByTagName('input');
        for (i = 0; i < nodeList.length; i++)
            if (nodeList[i].id.indexOf("ckb_update") > 0) {
                prefix = nodeList[i].id.replace("ckb_update","");
                if (nodeList[i].checked && (control=document.getElementById(prefix+'lbl_DNAmt')) != null)
                    selectedTotalDNAmt += Math.round(textToFloat(control.innerText)*100,2)/100 ;
            }    
        footerPrefix = dnPaymentTotalRowPrefix;
        if ((control=document.getElementById(footerPrefix + 'lbl_TotalDNAmt')) != null)
            control.innerText = selectedTotalDNAmt.toLocaleString();
           
    }
    updateSelectedCount();
}


function getGridId(gridControl) {
    tables = document.getElementsByTagName('table');
    i = tables.length;
    while (--i >= 0)
        if (tables[i].id != '' && gridControl.id.indexOf(tables[i].id + '_') >= 0)
            break;
    return tables[i].id;
}

function getGrid(gridControl) {
    tables = document.getElementsByTagName('table');
    i = tables.length;
    while (--i >= 0)
        if (tables[i].id != '' && gridControl.id.indexOf(tables[i].id + '_') >= 0)
            break;
    return tables[i];
}


function copyValue(sourceControl) {
    var sourceName;
    grid = getGrid(sourceControl);
    sourceName = sourceControl.id.substring(grid.id.length + 1, sourceControl.id.length)
    sourceName = sourceName.substring(sourceName.indexOf("_") + 1, sourceName.len);
    newValue = sourceControl.value;
    if (newValue != '') {
        elements = grid.getElementsByTagName('input');
        for (i = 0; i < elements.length; i++)
            if (elements[i].value == '') {
                var fieldName;
                fieldName = elements[i].id.substring(grid.id.length + 1, elements[i].id.length);
                fieldName = fieldName.substring(fieldName.indexOf("_") + 1, fieldName.len);
                if (fieldName==sourceName)
                    elements[i].value = newValue;
            }
    }
}


function updateTotalAmt(sourceControl)
{
    controlId = sourceControl.id.replace(gridPayment.id + "_", "");
    rowId = controlId.substring(0, controlId.indexOf("_")+1);
    type = controlId.replace(rowId, "");
    if (document.getElementById(gridPayment.id + "_" + rowId + "ckb_update").checked)
    {
        lblTotal = null;
        if (type == "txt_ARAmt")
            lblTotal = document.getElementById(paymentTotalRowPrefix + 'lbl_TotalARAmt');
        else if (type == "txt_APAmt")
            lblTotal = document.getElementById(paymentTotalRowPrefix + 'lbl_TotalAPAmt');
        else if (type == "txt_commAmt")
            lblTotal = document.getElementById(paymentTotalRowPrefix + 'lbl_TotalCommAmt');
 
        if (lblTotal != null)
            lblTotal.innerText = (textToFloat(lblTotal.innerText) - textToFloat(document.getElementById("temp").value) + textToFloat(sourceControl.value)).toFixed(2);
    }
}
        

function redirectButton(objFrom, toButtonName)
{
    var toButton = document.getElementById('ctl00_ContentPlaceHolder1_' + toButtonName);
    var path = toButton.click();
    return false;
}


function validateAmount(numericString) {
    amt = numericString.replace(',', '');
    return (amt == NaN || amt == '' ? '0' : amt)
}

function textToFloat(numericString) {
    amt = numericString.replace(',', '');
    return parseFloat(amt == NaN || amt == '' ? '0' : amt)
}


function initGrid() {
    gridPayment = null;
    gridDNPayment = null;
    paymentTotalRowPrefix = "";
    dnPaymentTotalRowPrefix = "";
    gridPayment = document.getElementById('<%= gv_Payment.ClientID %>');
    if (gridPayment != null) {
        lbls = gridPayment.getElementsByTagName("SPAN");
        for (i = lbls.length - 1; i >= 0 && paymentTotalRowPrefix == ""; i--)
            if (lbls[i].id.indexOf("lbl_TotalSalesAmt") >= 0)
                paymentTotalRowPrefix = lbls[i].id.replace("lbl_TotalSalesAmt", "");
    }

    gridDNPayment = document.getElementById('<%= gv_DNPayment.ClientID %>');
    if (gridDNPayment != null) {
        lbls = gridDNPayment.getElementsByTagName("SPAN");
        for (i = lbls.length - 1; i >= 0 && dnPaymentTotalRowPrefix == ""; i--)
            if (lbls[i].id.indexOf("lbl_TotalDNAmt") >= 0)
                dnPaymentTotalRowPrefix = lbls[i].id.replace("lbl_TotalDNAmt", "");
    }

}


function updateSelectedCount() {
    var selected;
    if (gridPayment != null) {
        selected = 0;
        inputs = gridPayment.getElementsByTagName("input");
        for (j = 0; j < inputs.length; j++)
            selected += (inputs[j].name.indexOf("ckb_update") > 0 && inputs[j].checked ? 1 : 0);
        counter = document.getElementById('<%=lbl_TotalSelected.ClientID %>');
        counter.innerText = selected;
    }
    if (gridDNPayment != null) {
        selected = 0
        inputs = gridDNPayment.getElementsByTagName("input");
        for (j = 0; j < inputs.length; j++)
            selected += (inputs[j].name.indexOf("ckb_update") > 0 && inputs[j].checked ? 1 : 0);
        counter = document.getElementById('<%=lbl_TotalDNSelected.ClientID %>');
        counter.innerText = selected;
    }
}        


function updateDNTotal(ckb) {
    var amt;
    var sign = (ckb.checked ? 1 : -1);
    var totalControl;

    totalControl = document.getElementById(dnPaymentTotalRowPrefix + "lbl_TotalDNAmt");
    control = document.getElementById(ckb.id.replace(/ckb_update/, "lbl_DNAmt"));
    if (control != null && totalControl != null) 
        totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(control.innerText) * sign).toFixed(2);
    if ((counter = document.getElementById('<%=lbl_TotalDNSelected.ClientID%>'))!=null)
        counter.innerText = parseInt(counter.innerText) + 1 * sign;

    return;
}

</script>

    <table cellpadding="2" cellspacing="0" width="800px">
    <tr>
        <td></td>
        <td>&nbsp;</td>
        <td></td>
        <td></td>
    </tr>
    <tr>
        <td></td>
        <td class="FieldLabel2" style="width:150px;">Type</td>
        <td><asp:DropDownList ID="ddl_type" runat="server" style="width:220px;">
                <asp:ListItem Text ="" Value="" />
                <asp:ListItem Text="Receipt" Value="ar" Selected="True" />
                <asp:ListItem Text="Settlement" Value="ap" />
                <asp:ListItem Text="L/C Settlement" Value="ap2" />
                <asp:ListItem Text="Settlement (Simplified)" Value="ap3" />
                <asp:ListItem Text="Sales Commission" Value="salesComm" />
            </asp:DropDownList>
        </td>
        <td align="right"><a href="LC SETTLEMENT UPLOAD FILE.csv"><b>L/C Settlement Template</b></a><br /><a href="Settlement - Simplified Template.csv"><b>Settlement Template - Simplified</b></a></td>
    </tr>
    <tr>
        <td colspan="3">
            <fieldset style="width:380px">
            <asp:Panel ID="pnl_AddInvoice" runat="server" DefaultButton="btn_Add" >
            <table cellpadding="1" cellspacing="1" >
                <tr>
                    <td class="FieldLabel2" style="width:150px;">Invoice No. / Split PO NO.</td>
                    <td>
                        
                            <asp:TextBox ID="txt_NSLInvNo" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td rowspan="3" style="vertical-align:middle;">
                            <asp:Button ID="btn_Add" runat="server" Text="Add" onclick="btn_Add_Click" 
                                OnClientClick="return isValid();" CausesValidation="False" />
                    </td>
                </tr>
                <tr style="height:3px;">
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="FieldLabel2" style="width:150px;">L/C Bill Ref. No.</td>
                    <td>
                        <asp:TextBox ID="txt_LCBillRefNo" runat="server" />&nbsp;&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
                        </asp:Panel> 
            </fieldset>
        </td>
        <td></td>
    </tr>
    <tr>
        <td></td>
        <td class="FieldLabel2" style="width:150px;">Upload File</td>
        <td>
            <asp:FileUpload ID="file_ARAP" runat="server" Width="300px" CssClass="" />
            <!--
            <div  style="position:relative;left:-50px;"><asp:Button ID="Button1" runat="server" Text="Browse" onClientclick='redirectButton(this,"file_ARAP");' /></div>
            -->
            <asp:Button ID="btn_Upload" runat="server" Text="Upload" 
                onclick="btn_Upload_Click"/>
        </td>
        <td></td>
    </tr>
</table>
<br />
<input type="hidden" value="0" id="temp" />
<asp:Label ID="lbl_Msg" runat="server" Text="Save completed."  Visible="false" style="color:#ff9900; font-weight :bolder;" /><br />
<asp:Label ID="lbl_Error" runat="server" style="color:Red; font-weight:bolder;" Visible="false" />
<asp:Panel ID="pnl_Result" runat="server" Visible="false" >
    <asp:LinkButton ID="lnk_SelectAll" runat="server" Text="Select All" OnClientClick="CheckAll('ckb_update'); getTotalAmt(); return false;" />&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="lnk_DeselectAll" runat="server" Text="Deselect All" OnClientClick="UncheckAll('ckb_update'); ResetTotal(); return false;" />&nbsp;&nbsp;&nbsp;
    <asp:CustomValidator ID="val_payment" runat="server" OnServerValidate="val_payment_validate" ErrorMessage="Invalid data input." /><br />
    <asp:Panel ID="pnl_TotalSelected" runat="server" Visible="false">No. of selected invoices: <asp:Label ID="lbl_TotalSelected" runat="server" /></asp:Panel>
    <asp:Panel ID="pnl_TotalDNSelected" runat="server" Visible="false">No. of selected Debit Note: <asp:Label ID="lbl_TotalDNSelected" runat="server" /></asp:Panel>
    <asp:Button ID="btn_Save" runat="server" Text="Save" onclick="btn_Save_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btn_Reset" runat="server" Text="Reset" onclick="btn_Reset_Click" />
    <asp:Panel id="pnl_Payment" runat="server">
        <asp:GridView ID="gv_Payment" runat="server" ShowFooter="true" 
            OnRowDeleting="PaymentRowDelete" CellPadding="2" AutoGenerateColumns="false" OnRowDataBound="PaymentRowDataBound" >
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>                
                        <asp:CheckBox ID="ckb_update" runat="server" Checked="true" onclick="updateTotal(this);"  />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Invoice No.">
                    <ItemTemplate>
                        <asp:Label ID="lbl_NSLInvNo"   runat="server" Text='<%# Eval("InvoiceNo") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Seq No">
                    <ItemTemplate>
                        <asp:Label ID="lbl_SeqNo" runat="server" Text='<%# Eval("SequenceNo") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Supplier Invoice No.">
                    <ItemTemplate>
                        <asp:Label ID="lbl_VendorInvNo"   runat="server" Text='<%# Eval("SupplierInvoiceNo") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Ccy">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Currency"   runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Payment Term">
                    <ItemTemplate>
                        <asp:Label ID="lbl_PaymentTerm"   runat="server" Text='<%# Eval("PaymentTerm.PaymentTermDescription") %>' />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lbl_Total" runat="server" Text="Total" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Sales Amount">
                    <ItemTemplate>
                        <asp:Label ID="lbl_SalesAmt"   runat="server" Text='<%# Eval("TotalShippedAmount","{0:#,##0.00}") %>' />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lbl_TotalSalesAmt" runat="server"  Text="0" />
                    </FooterTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <FooterStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Purchase Amount">
                    <ItemTemplate>
                        <asp:Label ID="lbl_PurchaseAmt"   runat="server" Text='<%# Eval("TotalShippedSupplierGarmentAmountAfterDiscount","{0:#,##0.00}") %>' />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lbl_TotalPurchaseAmt" runat="server"  Text="0" />
                    </FooterTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <FooterStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Sales Commission">
                    <ItemTemplate >
                        <asp:Label ID="lbl_SalesCommAmt" runat="server" Text='<%# Eval("NSLCommissionAmount","{0:#,##0.00}") %>' />
                    </ItemTemplate>
                    <FooterTemplate >
                        <asp:Label ID="lbl_TotalSalesCommAmt" runat="server" Text="0" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Receipt Amount">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_ARAmt" SkinID="DateTextBox" runat="server" Text='<%# Eval("ARAmount","{0:#,##0.00}") %>' onfocus="document.getElementById('temp').value = this.value;"
                            onblur="updateTotalAmt(this);"  />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <FooterStyle HorizontalAlign="Right"  />
                    <FooterTemplate>
                        <asp:Label ID="lbl_TotalARAmt" runat="server" Text="0" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Receipt Date">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_ARDate" SkinID="DateTextBox" runat="server" Text='<%# Eval("ARDate","{0:d}") %>' MaxLength="10" 
                        onblur="formatDateString(this); checkValidDate(this); checkAmount(this, 'txt_ARAmt'); copyValue(this);" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText ="Receipt Reference No.">
                    <ItemTemplate >
                        <asp:TextBox ID="txt_ARRefNo" SkinID="DateTextBox" runat="server" MaxLength="20" Text='<%# Eval("ARRefNo") %>' onblur="checkAmount(this, 'txt_ARAmt');copyValue(this);" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Settled Amount">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_APAmt" SkinID="DateTextBox" runat="server" Text='<%# Eval("APAmount","{0:#,##0.00}") %>' onfocus="document.getElementById('temp').value = this.value;"
                            onblur="updateTotalAmt(this);"  />
                    </ItemTemplate>
                    <FooterTemplate >
                        <asp:Label ID="lbl_TotalAPAmt" runat="server" Text="0" />
                    </FooterTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Settled Date">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_APDate" SkinID="DateTextBox" runat="server" Text='<%# Eval("APDate","{0:d}") %>' MaxLength="10" 
                        onblur="formatDateString(this);checkAmount(this,'txt_APAmt');copyValue(this);" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText ="Settled Reference No.">
                    <ItemTemplate >
                        <asp:TextBox ID="txt_APRefNo" SkinID="DateTextBox" runat="server" MaxLength="20" Text='<%# Eval("APRefNo") %>' onblur="checkAmount(this,'txt_APAmt');copyValue(this);" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Commission Settled Amount">
                    <ItemTemplate >
                        <asp:TextBox ID="txt_commAmt" SkinID="DateTextBox" runat="server" Text='<%# Eval("NSLCommissionSettlementAmount","{0:#,##0.00}") %>'
                            onblur="updateTotalAmt(this);" onfocus="document.getElementById('temp').value = this.value;" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <FooterStyle HorizontalAlign="Right"  />
                    <FooterTemplate>
                        <asp:Label ID="lbl_TotalCommAmt" runat="server" Text="0" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Commission Settled Date">
                    <ItemTemplate >
                        <asp:TextBox ID="txt_commDate" SkinID="DateTextBox" runat="server" Text='<%# Eval("NSLCommissionSettlementDate","{0:d}") %>' MaxLength="10" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Commission Settled Reference No.">
                    <ItemTemplate >
                        <asp:TextBox ID="txt_commRefNo" SkinID="DateTextBox" runat="server" Text='<%# Eval("NSLCommissionSettlementRefNo") %>' 
                            onblur="checkAmount(this,'txt_commAmt');copyValue(this);" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel><br />
    <asp:Panel id="pnl_DNPayment" runat="server">
        <asp:GridView ID="gv_DNPayment" runat="server" ShowFooter="true" 
            OnRowDeleting="DNPaymentRowDelete" CellPadding="2" AutoGenerateColumns="false" OnRowDataBound="DNPaymentRowDataBound" >
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>                
                        <asp:CheckBox ID="ckb_update" runat="server" Checked="true" onclick="updateDNTotal(this);"  />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Debit Note No.">
                    <ItemTemplate>
                        <asp:Label ID="lbl_NSLDNNo"   runat="server" Text='<%# Eval("DCNoteNo") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Ccy">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Currency"   runat="server" />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lbl_Total" runat="server" Text="Total" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Debit Note Amount">
                    <ItemTemplate>
                        <asp:Label ID="lbl_DNAmt"   runat="server" Text='<%# Eval("TotalAmount","{0:#,##0.00}") %>' />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lbl_TotalDNAmt" runat="server"  Text="0" />
                    </FooterTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <FooterStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Settled Amount" visible="false">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_DNSettleAmt" SkinID="DateTextBox" runat="server" Text='<%# Eval("SettledAmount","{0:#,##0.00}") %>' onfocus="document.getElementById('temp').value = this.value;"
                            onblur="updateTotalDNAmt(this, 'txt_DNSettleAmt');" style="display:none;" />
                        <asp:Label ID="lbl_DNSettleAmt" runat="server" Text='<%# Eval("SettledAmount","{0:#,##0.00}") %>' />
                    </ItemTemplate>
                    <FooterTemplate >
                        <asp:Label ID="lbl_TotalDNSettleAmt" runat="server" Text="0" />
                    </FooterTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Settled Date">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_Date" SkinID="DateTextBox" runat="server" Text='<%# Eval("SettlementDate","{0:d}") %>' MaxLength="10" 
                        onblur="formatDateString(this);copyValue(this);" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel><br />
</asp:Panel>

<script type="text/javascript">
    //debugger;
    initGrid();
    getTotalAmt();
</script>

</asp:Content>
