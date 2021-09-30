<%@ Page Title="Non-Trade Expense - Settlement" Language="C#" Theme="DefaultTheme" MasterPageFile="~/MasterPage/MainMaster.Master" AutoEventWireup="true" CodeBehind="NonTradeExpenseSettlement.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NonTradeExpenseSettlement" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_ImageHeader" runat="server">
<!--
<img src="../images/banner_accounts_receipt_settlement.gif" runat="server" id="imgHeaderText" />
<img src="../images/banner_workplace.gif" runat="server" />
-->
<asp:Panel runat="server" SkinID="SectionHeader_Accounts">Non-Trade Expense Settlement Maintenance</asp:Panel>
</asp:Content>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript" >

function openInvoiceWindow(invoiceId) {
    if (invoiceId == 0)
        window.open('NonTradeInvoice.aspx', 'NTInvoice', 'width=900,height=900, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();
    else
        window.open('NonTradeInvoice.aspx?InvoiceId=' + invoiceId, 'NTInvoice', 'width=900,height=900, top=0, left=0,scrollbars=1,resizable=1,status=1').focus();
}

function getTotalSettleAmount() {
   //debugger;
    var ret = window.showModalDialog('../common/PromptInput.aspx?InputField=[DECIMAL]Total Settlement Amount |[DATE]Settlement Date&Title=Non-Trade Expense Settlement', 'NTSettlement', 'DialogWidth:350px;DialogHeight:150px;scrollbars:no;resizable:yes;status:no');
    //var ret = window.open('../common/PromptInput.aspx?prompt=Total Settlement Amount |Settlement Date&Heading=Non-Trade Expense Settlement', 'NTSettlement', 'Width=3200px,Height=150px,resizable=1');
    if (ret !== undefined) {
        txtAmt = document.getElementById('<%= hid_SettlementTotalAmount.ClientID %>');
        txtAmt.value = ret[0];
        txtDate = document.getElementById('<%= hid_SettlementDate.ClientID %>');
        txtDate.value = ret[1];
    }
    return (ret !== undefined)
}

/*
function isLumpSumPaymentValid() {
    //debugger;
    currency = getSelectedCurrency();
    txtAmt = document.getElementById('<%= hid_SettlementTotalAmount.ClientID %>');
    txtDate = document.getElementById('<%= hid_SettlementDate.ClientID %>');
    ddlBankAcc = document.getElementById('<%= ddl_BankAccount.ClientID %>');
    msg = "";
    amt = 0;
    if (currency == undefined)
        msg = "Please select an entry first.";
    if (msg != "") {
        alert(msg);
        return false;
    }
    else {
        alert(txtAmt.value);
        return false;
    }
}

function isAccountNoValid(input) {
    return true;
}
*/

function checkAmount(gridControl, target) {
    gridId = getGridId(gridControl);
    controlId = gridControl.id.replace(gridId+'_','');
    rowId = controlId.substring(0, controlId.indexOf('_'));
    targetControl = document.getElementById(gridId + '_' + rowId + '_' + target);
    if (targetControl != null)
        if (targetControl.value == "" || targetControl.value == "0")
            alert("The amount is 0.");
}


function updateTotal(ckb) {
    var totalControl;
    var selected = (ckb.checked ? 1 : -1);

    grid = getGrid(ckb);
    prefix = grid.TotalRowPrefix;
    controlId = ckb.id.replace(grid.id + '_', '');
    rowPrefix = grid.id + '_' + controlId.substring(0, controlId.indexOf('_')) + '_';
    if (grid.id == '<%=gv_NTPayment.ClientID %>') {
        sign = (document.getElementById(rowPrefix + 'lbl_DocumentType').innerText == 'Credit Note' ? -1 : 1);
        if ((totalControl = document.getElementById(prefix + 'lbl_TotalNTSettleAmt')) != null)
            totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(document.getElementById(rowPrefix + "txt_NTSettleAmt").value) * sign * selected).toFixed(2);
        if ((totalControl = document.getElementById(prefix + 'lbl_TotalNTInvoiceAmt')) != null)
            totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(document.getElementById(rowPrefix + "lbl_NTInvoiceAmt").innerText) * sign * selected).toFixed(2);
        if ((totalControl = document.getElementById(prefix + 'lbl_TotalVATAmt')) != null)
            totalControl.innerText = (textToFloat(totalControl.innerText) + textToFloat(document.getElementById(rowPrefix + "lbl_VATAmt").innerText) * sign * selected).toFixed(2);
        if ((counter = document.getElementById('<%=lbl_TotalNTSelected.ClientID%>')) != null)
            counter.innerText = parseInt(counter.innerText) + 1 * selected;
    }
    return;
}


function ResetTotal() {   
    var control;
    var counter;

    if (gridNTPayment != null) {
        if ((control = document.getElementById(gridNTPayment.TotalRowPrefix + 'lbl_TotalNTInvoiceAmt')) != null)
            control.innerText = 0;
        if ((control = document.getElementById(gridNTPayment.TotalRowPrefix + 'lbl_TotalVATAmt')) != null)
            control.innerText = 0;
        if ((control = document.getElementById(gridNTPayment.TotalRowPrefix + 'lbl_TotalNTSettleAmt')) != null)
            control.innerText = 0;
    }
        
    if ((counter = document.getElementById('<%= lbl_TotalNTSelected.ClientID %>')) != null)
        counter.innerText = 0;
}


function getTotalAmt() {
    //debugger;
    countedTable = 0;
    footerPrefix = "";
    gridId = "";

    if (gridNTPayment != null) {
        var selectedTotalNTInvoiceAmt = 0;
        var selectedTotalNTSettleAmt = 0;
        var selectedTotalVATAmt = 0;
        var nodeList = gridNTPayment.getElementsByTagName('input');
        for (i = 0; i < nodeList.length; i++)
            if (nodeList[i].id.indexOf("ckb_update") > 0) {
                prefix = nodeList[i].id.replace("ckb_update", "");
                if (nodeList[i].checked) {
                    sign = (document.getElementById(prefix + 'lbl_DocumentType').innerText == 'Credit Note' ? -1 : 1);
                    if ((control = document.getElementById(prefix + 'lbl_NTInvoiceAmt')) != null)
                        selectedTotalNTInvoiceAmt += Math.round(textToFloat(control.innerText) * sign * 100, 2) / 100;
                    if ((control = document.getElementById(prefix + 'lbl_VATAmt')) != null)
                        selectedTotalVATAmt += Math.round(textToFloat(control.innerText) * sign * 100, 2) / 100;
                    if ((control = document.getElementById(prefix + 'txt_NTSettleAmt')) != null)
                        selectedTotalNTSettleAmt += Math.round(textToFloat(control.value) * sign * 100, 2) / 100;
                }
            }
        if ((control = document.getElementById(gridNTPayment.TotalRowPrefix + 'lbl_TotalNTInvoiceAmt')) != null)
            control.innerText = selectedTotalNTInvoiceAmt.toLocaleString();
        if ((control = document.getElementById(gridNTPayment.TotalRowPrefix + 'lbl_TotalVATAmt')) != null)
            control.innerText = selectedTotalVATAmt.toLocaleString();
        if ((control = document.getElementById(gridNTPayment.TotalRowPrefix + 'lbl_TotalNTSettleAmt')) != null)
            control.innerText = selectedTotalNTSettleAmt.toLocaleString();
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
        for (i = 0; i < elements.length; i++) {
            if (elements[i].value == '') {
                var elementName, fieldName, rowId;
                elementName = elements[i].id.substring(grid.id.length + 1, elements[i].id.length);
                rowId = elementName.substring(0, elementName.indexOf("_"));
                fieldName = elementName.substring(elementName.indexOf("_") + 1, elementName.len);
                if (fieldName == sourceName) 
                    if ((ckb = document.getElementById(grid.id + "_" + rowId + "_ckb_Update")) != null)
                        if (ckb.checked)
                            elements[i].value = newValue;
            }
        }
    }
}


function updateTotalAmt(gridControl) {
    grid = getGrid(gridControl);
    controlId = gridControl.id.replace(grid.id + "_", "");
    rowId = controlId.substring(0, controlId.indexOf("_") + 1);
    type = controlId.replace(rowId, "");

    if (document.getElementById(grid.id + "_" + rowId + "ckb_update").checked) {
        sign = (document.getElementById(grid.id + "_" + rowId + 'lbl_DocumentType').innerText == 'Credit Note' ? -1 : 1);
        if (grid.id == "<%= gv_NTPayment.ClientID %>") {
            if (type == "txt_NTSettleAmt")
                lblTotal = document.getElementById(grid.TotalRowPrefix + 'lbl_TotalNTSettleAmt');
            if (lblTotal != null)
                lblTotal.innerText = (textToFloat(lblTotal.innerText) - textToFloat(document.getElementById("temp").value) + textToFloat(gridControl.value) * sign).toFixed(2);
        }
    }
}
        

function redirectButton(objFrom, toButtonName)
{
    var toButton = document.getElementById('ctl00_ContentPlaceHolder1_' + toButtonName);
    var path = toButton.click();
    return false;
}


function textToFloat(numericString) {
    amt = numericString;
    while (amt.indexOf(',')>=0)
        amt = amt.replace(',', '');
    return parseFloat(amt == NaN || amt == '' ? '0' : amt)
}


var gridNTPayment;
function initGrid() {
    gridNTPayment = document.getElementById('<%= gv_NTPayment.ClientID %>');
    if (gridNTPayment != null) {
        totalRowPrefix = "";
        lbls = gridNTPayment.getElementsByTagName("SPAN");
        for (i = lbls.length - 1, totalRowPrefix = ""; i >= 0 && totalRowPrefix == ""; i--)
            if (lbls[i].id.indexOf("lbl_Total") >= 0)
                totalRowPrefix = lbls[i].id.substring(0, lbls[i].id.indexOf("lbl_Total"));
        gridNTPayment.TotalRowPrefix = totalRowPrefix;
    }
}


function updateSelectedCount() {
    var selected;
    if (gridNTPayment != null) {
        selected = 0
        inputs = gridNTPayment.getElementsByTagName("input");
        for (j = 0; j < inputs.length; j++)
            selected += (inputs[j].name.indexOf("ckb_update") > 0 && inputs[j].checked ? 1 : 0);
        if ((counter = document.getElementById('<%= lbl_TotalNTSelected.ClientID %>')) != null)
            counter.innerText = selected;
    }
}

function getSelectedCurrency() {
    var currency;
    if ((grid=gridNTPayment) != null) {
        inputs = grid.getElementsByTagName("input");
        for (j = 0; j < inputs.length; j++) {
            if ((ckb=inputs[j]).name.indexOf("ckb_update") > 0) {
                if (ckb.checked) {
                    rowId = ckb.id.replace("ckb_update", "");
                    lbl = document.getElementById(rowId + "lbl_Currency");
                    if (currency == undefined)
                        currency = lbl.innerText;
                    else if (lbl.innerText != currency) {
                        currency = "";
                        break;
                    }
                }
            }
        }
    }
    return currency;
}


function openAttachments(o, id) {
    window.open('AttachmentList.aspx?invoiceId=' + id, 'AttachmentList', 'status=1,width=400,height=500');
}       

</script>

<table cellpadding="2" cellspacing="0" width="800px">
    <col width="180px" /><col />
    <tr>
        <td class="FieldLabel2">Office</td>
        <td> <cc2:SmartDropDownList ID="ddl_Office" runat="server"  OnSelectedIndexChanged="ddl_Office_OnSelectedIndexChanged" AutoPostBack="true" /></td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:160px;">Supplier Invoice No./Account No.</td>
        <td><asp:Panel ID="pnl_AddInvoice" runat="server" DefaultButton="btn_Search" >
                <asp:TextBox ID="txt_AccountNoFrom" runat="server" style="width:140px;" />&nbsp;To&nbsp;
                <asp:TextBox ID="txt_AccountNoTo" runat="server"  style="width:140px;"/>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:150px;">Invoice Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_InvoiceDateFrom" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" runat="server"/>
            &nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_InvoiceDateTo" FromDateControl="txt_InvoiceDateFrom" ToDateControl="txt_InvoiceDateTo" runat="server"/>  
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:150px;">Due Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_DueDateFrom" FromDateControl="txt_DueDateFrom" ToDateControl="txt_DueDateTo" runat="server"/>
            &nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_DueDateTo" FromDateControl="txt_DueDateFrom" ToDateControl="txt_DueDateTo" runat="server"/>  
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:150px;">Settlement Date</td>
        <td>
            <cc2:SmartCalendar ID="txt_SettlementDateFrom" FromDateControl="txt_SettlementDateFrom" ToDateControl="txt_SettlementDateTo" runat="server"/>
            &nbsp;To&nbsp;
            <cc2:SmartCalendar ID="txt_SettlementDateTo" FromDateControl="txt_SettlementDateFrom" ToDateControl="txt_SettlementDateTo" runat="server"/>  
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2" style="width:150px;">Vendor</td>
        <td>
            <uc1:UclSmartSelection ID="txt_SupplierName" runat="server" AutoPostBack="true" />&nbsp;
        </td>
    </tr>
    <tr>
        <td class="FieldLabel2">Document Type</td>
        <td>
            <cc2:SmartDropDownList ID="ddl_DocumentType" runat="server"/>                            
        </td>  
    </tr>
    <tr>
        <td class="FieldLabel2">Payment Method</td>
        <td>
            <cc2:SmartDropDownList ID="ddl_PaymentMethod" runat="server" Width="65"/>                            
        </td>  
    </tr>
    <tr>
        <td class="FieldLabel2">Currency</td>
        <td>
            <cc2:SmartDropDownList ID="ddl_Currency" runat="server" OnSelectedIndexChanged="ddl_Currency_OnSelectedIndexChanged" AutoPostBack="true"   Width="65"/>                            
        </td>  
    </tr>
    <tr>
        <td class="FieldLabel2">Status</td>
        <td>
            <cc2:SmartDropDownList ID="ddl_Status" runat="server" />                            
        </td>  
    </tr>
    <tr id="tr_FirstApprover" runat="server">
        <td class="FieldLabel2">First Approver</td>
        <td>
            <cc2:SmartDropDownList ID="ddl_FirstApprover" runat="server"/>                            
        </td>  
    </tr>

</table>
<br />
<input type="hidden" value="0" id="temp" />
<asp:Label ID="lbl_Msg" runat="server" Text="Save completed."  Visible="false" style="color:#ff9900; font-weight :bolder;" /><br />
<asp:Label ID="lbl_Error" runat="server" style="color:Red; font-weight:bolder;" Visible="false" />
    <asp:Panel ID="pnl_Buttons" runat="server">
        <table>
            <tr>
                <td>
                    <asp:Button ID="btn_Search" runat="server" Text="Search" onclick="btn_Search_Click" CausesValidation="False" />&nbsp;
                    <asp:Button ID="btn_Save" runat="server" Text="Save" onclick="btn_Save_Click"  CausesValidation="False" />&nbsp;
                    <asp:Button ID="btn_Reset" runat="server" Text="Reset" onclick="btn_Reset_Click" CausesValidation="False" />&nbsp;
                    <asp:Button ID="btn_Export" runat="server" Text="Export" onclick="btn_Export_Click" CausesValidation="False" />&nbsp;
                </td>
                <td >
                    <asp:Panel ID="pnl_PayLumpSumButtons" runat="server" Visible="false">
                        <b>NSL Bank Account :</b>&nbsp;
                        <cc2:SmartDropDownList ID="ddl_BankAccount" runat="server" />&nbsp;
                        <asp:Button ID="btn_LumpSumSave" runat="server" Text="Pay Using Different Currency"  SkinID="XXLButton"
                            OnClick="btn_LumpSumSave_Click" OnClientClick="javascript:return getTotalSettleAmount();" CausesValidation="false" 
                            ToolTip=
"Step 1: select entries that you want to pay (all must be in same currency, otherwise, system does not allow you to proceed.) 
Step 2: You are prompted to enter the total settlement amount, and date.
Step 3: System will allocate your input amount to all selected entries proportionally based on invoice amount.
"/>
                        <asp:HiddenField ID="hid_SettlementTotalAmount" runat="server"/>
                        <asp:HiddenField ID="hid_SettlementDate" runat="server"/>
                    </asp:Panel>
                </td>
            </tr>
        </table>     
    </asp:Panel>
<asp:Panel ID="pnl_Result" runat="server" Visible="false" >
    <asp:LinkButton ID="lnk_SelectAll" runat="server" Text="Select All" OnClientClick="CheckAll('ckb_update'); getTotalAmt(); return false;" />&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="lnk_DeselectAll" runat="server" Text="Deselect All" OnClientClick="UncheckAll('ckb_update'); ResetTotal(); return false;" />&nbsp;&nbsp;&nbsp;
    <asp:Panel ID="pnl_TotalNTSelected" runat="server" Visible="false">
        <table>
            <tr>
                <td style="width:150px" colspan="2">No. of selected NT Invoice: &nbsp;<asp:Label ID="lbl_TotalNTSelected" runat="server" /></td>
                <td>&nbsp;&nbsp;</td>
                <td><asp:CustomValidator ID="val_NTpayment" runat="server" OnServerValidate="val_NTPayment_validate" ErrorMessage="Invalid data input." style="font-size:medium;" />
                <td>&nbsp;</td>
            </tr>
        </table>
    </asp:Panel>


    <asp:Panel id="pnl_NTPayment" runat="server">
        <asp:GridView ID="gv_NTPayment" runat="server" ShowFooter="true" AllowSorting="true" OnSorting="SettlementOnSort" 
            CellPadding="2" AutoGenerateColumns="false" OnRowDataBound="NTPaymentRowDataBound" >
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>   
                        <table>
                            <tr>
                                <td><asp:CheckBox ID="ckb_update" runat="server" Checked="true" onclick="updateTotal(this);" /></td>
                                <td><asp:ImageButton ID="lnk_Attachment" runat="server" ImageUrl="~/images/icon_edit.gif" ToolTip="View Document" CausesValidation="False"/></td>
                                <td><asp:Image ID="img_PayByHK" runat="server" ImageUrl="..\images\icon_hk.gif" ToolTip="Pay By HK Office"/></td>
                            </tr>
                        </table>             
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Document<br/>Type" SortExpression="DocumentType"  HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_DocumentType" runat="server" Text="" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Supplier<br/>Invoice No." SortExpression="InvoiceNo" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnk_SupplierInvoiceNo" runat="server" Text='<%# Eval("ntInvoice.InvoiceNo") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Account No." SortExpression="AccountNo" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_AccountNo_old"   runat="server" Text='' />
                        <asp:LinkButton ID="lnk_AccountNo" runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Office" SortExpression="Office" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Office"   runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Vendor" SortExpression="Vendor" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Vendor"   runat="server" Text='' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Invoice<br/>Date" SortExpression="InvoiceDate" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_InvoiceDate" SkinID="DateTextBox" runat="server" Text='<%# Eval("ntInvoice.InvoiceDate","{0:d}") %>' MaxLength="10" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="NSL<br/>Ref. No." SortExpression="NSLRefNo" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_NSLInvoiceNo"   runat="server" Text='<%# Eval("ntInvoice.NSLInvoiceNo") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Ccy" SortExpression="Currency" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_Currency"   runat="server" Text='<%# Eval("ntInvoice.Currency.Name") %>'/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Payment<br/>Method" SortExpression="PaymentMethod" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_PaymentTerm"   runat="server" Text='<%# Eval("ntInvoice.PaymentMethod.Name") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Cheque No." SortExpression="ChequeNo" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Panel id="pnl_ChequeNo" runat="server"> 
                            <asp:TextBox ID="txt_ChequeNo" SkinID="DateTextBox" runat="server" Text='' MaxLength="10" 
                                onblur="copyValue(this);" />
                        </asp:Panel>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lbl_Total" runat="server" Text="Total"  AssociatedKey="GRID_TOTAL"  />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Invoice<br/>Amount">
                    <ItemTemplate>
                        <asp:Label ID="lbl_NTInvoiceAmt"   runat="server" Text='<%# Eval("ntInvoice.Amount","{0:#,##0.00}") %>' 
                            onblur="updateTotalAmt(this);" />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lbl_TotalNTInvoiceAmt" runat="server"  Text="0" />
                    </FooterTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <FooterStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="VAT">
                    <ItemTemplate>
                        <asp:Label ID="lbl_VATAmt"   runat="server" Text='<%# Eval("ntInvoice.TotalVAT","{0:#,##0.00}") %>' 
                            onblur="updateTotalAmt(this);" />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lbl_TotalVATAmt" runat="server"  Text="0" />
                    </FooterTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <FooterStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Settled<br/>Amount" >
                    <ItemTemplate>
                        <table>
                            <tr>
                                <td><asp:Label ID="lbl_SettleCurrency" runat="server" Text='' style="display:none;"/></td>
                                <td>
                                    <asp:TextBox ID="txt_NTSettleAmt" SkinID= "TextBox_50" runat="server" Text='' onfocus="document.getElementById('temp').value = this.value;"
                                        onblur="updateTotalAmt(this);"  />
                                </td>
                                <td><asp:Label ID="lbl_NTSettleAmt" runat="server" Text='' style="display:none;"/></td>
                            </tr>
                        </table>
                    </ItemTemplate>
                    <FooterTemplate >
                        <asp:Label ID="lbl_TotalNTSettleAmt" runat="server" Text="0" />
                    </FooterTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Settlement<br/>Date" SortExpression="SettlementDate" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_NTSettleDate" SkinID="DateTextBox" runat="server" Text='' MaxLength="10" 
                        onblur="formatDateString(this); if (isDateValid(this.value)) copyValue(this);" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Settlement<br/>Ref. No." Visible="false">
                    <ItemTemplate >
                        <asp:TextBox ID="txt_NTSettleRef" SkinID="DateTextBox" runat="server" Text='' 
                            onblur="checkAmount(this,'txt_NTSettleAmt');copyValue(this);" MaxLength="10" />
                    </ItemTemplate>
                </asp:TemplateField>
               <asp:TemplateField HeaderText="1 st Approver" SortExpression="FirstApprover" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_1stApprover"   runat="server" Text=''/>
                    </ItemTemplate>
                </asp:TemplateField>
               <asp:TemplateField HeaderText="Due Date" SortExpression="DueDate" HeaderStyle-ForeColor="CornflowerBlue">
                    <ItemTemplate>
                        <asp:Label ID="lbl_DueDate"   runat="server" Text=''/>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                No record found.
            </EmptyDataTemplate>
        </asp:GridView>
    </asp:Panel>
        <table>
            <tr>
                <td><asp:Image ID="Image1" runat="server" ImageUrl="..\images\icon_hk.gif" ToolTip="Pay By HK Office" /></td>
                <td> - Pay by Hong Kong Office</td>
            </tr>
        </table>

</asp:Panel>

<script type="text/javascript">
    //debugger;
    initGrid();
    getTotalAmt();
</script>

</asp:Content>
