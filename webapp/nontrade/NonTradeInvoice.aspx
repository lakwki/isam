<%@ Page Title="" Language="C#" Theme="DefaultTheme"  AutoEventWireup="true"    CodeBehind="NonTradeInvoice.aspx.cs" Inherits="com.next.isam.webapp.nontrade.NonTradeInvoice" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="com.next.infra.smartwebcontrol" Namespace="com.next.infra.smartwebcontrol" TagPrefix="cc2" %>
<%@ Register src="../webservices/UclSmartSelection.ascx" tagname="UclSmartSelection" tagprefix="uc1" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Non-Trade Expense</title>
    <link href="../includes/RevampStyle.css" type="text/css" rel="stylesheet"/>
    <script src="../common/common.js" type="text/javascript"></script>
    <script type="text/javascript" src="../webservices/UclSmartSelection.js" ></script>

    <script type="text/javascript" >
        window.history.forward(1);
        function getTotalAmount(amount) {            
            if (updateTotalAmount)
                document.getElementById("txt_Amount").value = amount;
        }
        function getTotalVATAmount(amount) {
            if (updateTotalAmount)
                document.getElementById("txt_TotalVAT").value = amount;
        }

        updateTotalAmount = false;
        itemCount = 1;
        function controlTotalAmount(amount) {
            if (document.getElementById("btn_Discard"))
                return;

            if ((document.getElementById("txt_Amount").value == "" || document.getElementById("txt_Amount").value == "0" || document.getElementById("txt_Amount").value == amount) && itemCount == 1) {
                updateTotalAmount = true;
                document.getElementById("txt_Amount").readOnly = true;
                document.getElementById("txt_Amount").style.color = "gray";
                document.getElementById("txt_Amount").style.border = "1px solid #AAAAAA";
            }
            else {
                document.getElementById("txt_Amount").readOnly = false;
                document.getElementById("txt_Amount").style.color = "black";
                document.getElementById("txt_Amount").style.border = "1px solid #DDDDDD";
                updateTotalAmount = false;
            }
        }

        function controlTotalVATAmount(amount) {
            if (document.getElementById("btn_Discard"))
                return;
            if ((document.getElementById("txt_TotalVAT").value == "" || document.getElementById("txt_TotalVAT").value == "0" || document.getElementById("txt_TotalVAT").value == amount) && itemCount == 1) {
                updateTotalAmount = true;
                document.getElementById("txt_TotalVAT").readOnly = true;
                document.getElementById("txt_TotalVAT").style.color = "gray";
                document.getElementById("txt_TotalVAT").style.border = "1px solid #AAAAAA";
            }
            else {
                document.getElementById("txt_TotalVAT").readOnly = false;
                document.getElementById("txt_TotalVAT").style.color = "black";
                document.getElementById("txt_TotalVAT").style.border = "1px solid #DDDDDD";
                updateTotalAmount = false;
            }
        }

        function checkAmount(amount) {
            if (isNaN(amount))
                alert("Invalid amount");
            else if (amount == "")
                alert("Please enter the Amount.");
        }

        function calculateTotalAmount() {
            if (document.getElementById("hf_CalcTotalAmt").value == "0")
                return;

            i = 0;
            totalAmount = 0;
            controlId = "rep_InvoiceDetail_ctl00_txt_Amount";
            
            while (document.getElementById(controlId)) {
                if (document.getElementById(controlId).value != "" && !isNaN(document.getElementById(controlId).value)) {
                    totalAmount += parseFloat(document.getElementById(controlId).value);
                    totalAmount = parseFloat(totalAmount.toFixed(2));
                }
                i += 2;
                controlId = "rep_InvoiceDetail_ctl" + ( i< 10 ? ("0" + i) : i) + "_txt_Amount";
            }

            i = 0;
            controlId = "rep_RechargeDetail_ctl00_txt_RechargeAmt";
            while (document.getElementById(controlId)) {
                if (document.getElementById(controlId).value != "" && !isNaN(document.getElementById(controlId).value)) {
                    totalAmount += parseFloat(document.getElementById(controlId).value);
                    totalAmount = parseFloat(totalAmount.toFixed(2));
                }
                i += 2;
                controlId = "rep_RechargeDetail_ctl" + (i < 10 ? ("0" + i) : i) + "_txt_RechargeAmt";
            }
            document.getElementById("txt_Amount").value = totalAmount;
        }

        function getDueDate(invoiceDate) {
            if (invoiceDate != "" && document.getElementById("hf_PaymentTermDays").value != "") {
                var parts = invoiceDate.match(/(\d+)/g);
                d = new Date(parts[2], parts[1] - 1, parts[0]);
                d.setDate(d.getDate() + parseInt(document.getElementById("hf_PaymentTermDays").value));
                document.getElementById("txt_DueDate").value = d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getYear();                
            }
        }

        function dateSelectionChanged(sender, args) {
            d = sender.get_selectedDate()            
            getDueDate(d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getYear());
        }

        function calendarShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 10005;
        }

        function getVendorType(detailTypeId) {
            if (detailTypeId == 4)
                document.getElementById("txt_VendorType").value = 2;
            else if (detailTypeId == 5)
                document.getElementById("txt_VendorType").value = 1;
            else if (detailTypeId == 6)
                document.getElementById("txt_VendorType").value = 6;
            else if (detailTypeId == 7)
                document.getElementById("txt_VendorType").value = 4;
            else if (detailTypeId == 8)
                document.getElementById("txt_VendorType").value = 7;
            else if (detailTypeId == 9)
                document.getElementById("txt_VendorType").value = 5;
            else
                document.getElementById("txt_VendorType").value = "";
        }

        function addItemDescription(sourceId) {
            //rep_InvoiceDetail_ctl00_row_ItemDesc1
            prefix = sourceId.substring(0, 24);

            if (document.getElementById(prefix + "row_ItemDesc3").style.display == "none")
                document.getElementById(prefix + "row_ItemDesc3").style.display = "block";
            else if (document.getElementById(prefix + "row_ItemDesc4").style.display == "none")
                document.getElementById(prefix + "row_ItemDesc4").style.display = "block";
            else if (document.getElementById(prefix + "row_ItemDesc5").style.display == "none")
                document.getElementById(prefix + "row_ItemDesc5").style.display = "block";
        }
        function addRechargeItemDescription(sourceId) {
            prefix = sourceId.substring(0, 25);
            
            if (document.getElementById(prefix + "row_RechargeItemDesc3").style.display == "none")
                document.getElementById(prefix + "row_RechargeItemDesc3").style.display = "block";
            else if (document.getElementById(prefix + "row_RechargeItemDesc4").style.display == "none")
                document.getElementById(prefix + "row_RechargeItemDesc4").style.display = "block";
            else if (document.getElementById(prefix + "row_RechargeItemDesc5").style.display == "none")
                document.getElementById(prefix + "row_RechargeItemDesc5").style.display = "block";
        }

        var eventName = "";
        function processEvent() {
            if (eventName != "")
                return false;
            else {
                eventName = "save";
                return true;
            }
        }
    </script>
    </head>
<body>
<form id="form1" runat="server" >
<asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
<asp:panel ID="pnl_ShipmentHeading" runat="server"  SkinID="sectionHeader1" >
<table><tr><td style="height:22px; font-weight:bold ; width:100%">Non-Trade Expense</td>
<td><asp:TextBox ID="txt_Status" runat="server" Text="DRAFT" ReadOnly="true" SkinID="TextBox_160" /></td>
</tr></table>
</asp:panel> 
<asp:HiddenField ID="txt_VendorType" runat="server" />
<asp:HiddenField ID="txt_VendorStatus" runat="server" Value="1" />
<asp:HiddenField ID="txt_UserOfficeId" runat="server" Value="-1" />
<asp:HiddenField ID="hf_Vendor" runat="server" Value="" />
<asp:HiddenField ID="hf_PaymentTermDays" runat="server" Value="" />
<asp:HiddenField ID="hf_CalcTotalAmt" runat="server" Value="0" />
<asp:HiddenField ID="ctl00_ContentPlaceHolder1_ddl_Office" runat="server" Value="" />
<asp:ValidationSummary ID="validationSummary1" runat="server" />
<asp:CustomValidator ID="val_InvoiceDetail" runat="server" OnServerValidate="val_InvoiceDetail_Validate" ControlToValidate="txt_Amount" ValidateEmptyText="true" Display="None" />
<div id="div_monthEnd" runat="server" visible="false">
    <span style="color:#009933; font-weight :bold; margin : 10px; font-size : 12pt;"><asp:Label ID="lbl_Office" runat="server" EnableTheming="false"  /> office has started the month end process. Invoice cannot be modified and submitted at the moment.</span>
</div>
<table width="900px" cellspacing="2" cellpadding="2">        
        <tr>
            <td>
                <table width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="5">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2" style="width:140px">&nbsp;Office&nbsp;</td>
                        <td>&nbsp;</td>
                        <td style="width:350px;">
                            <cc2:SmartDropDownList  ID="ddl_Office" runat="server" OnSelectedIndexChanged="OfficeSelectedIndexChange" AutoPostBack="true"  />                            
                            <span style="display:none;"><cc2:SmartDropDownList ID="ddl_Department" runat="server" /></span>                            
                                <cc2:SmartDropDownList ID="ddl_BusinessEntity" runat="server" OnSelectedIndexChanged="BusinessEntitySelectedIndexChange" AutoPostBack="true"   />                            
                        </td>
                        <td class="FieldLabel2" style="width :120px">&nbsp;NSL Ref. No.</td>
                        <td>&nbsp;</td>
                        <td style="width:300px;">
                            <asp:TextBox ID="txt_NSLRefNo" runat="server" ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2" style="vertical-align :top ; background-position : top ;">&nbsp;Vendor</td>
                        <td>&nbsp;</td>
                        <td><asp:CustomValidator ID="val_Vendor" runat="server" OnServerValidate="val_Vendor_Validate" ControlToValidate="txt_InvoiceReceivedDate" ValidateEmptyText="true"  Display="None" />
                        <div id="div_sup" runat="server" onmouseover="document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office').value = document.getElementById('ddl_Office').value;">
                            <uc1:UclSmartSelection ID="txt_SupplierName" runat="server" AutoPostBack="true" OnSelectionChanged="txtSupplierNameChange"  />&nbsp;           
                            </div>
                            <%--<div id="div_newSup" runat="server" visible ="false" >
                                <table style="border: 1px solid #AAAAAA;">
                                    <tr>
                                        <td class="FieldLabel">Vendor Name</td>
                                        <td><asp:TextBox ID="txt_VendorName" runat="server" SkinID="TextBox300"  /></td>
                                        <td class="FieldLabel">Telephone No.</td>
                                        <td><asp:TextBox ID="txt_VendorTel" runat="server" /></td>
                                    </tr>
                                                                       
                                    <tr>
                                        <td class="FieldLabel">Address</td>
                                        <td><asp:TextBox ID="txt_VendorAddr" runat="server" SkinID="TextBox300" /></td>                                        
                                        <td class="FieldLabel">Fax No.</td>
                                        <td><asp:TextBox ID="txt_VendorFax" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td class="FieldLabel">Expense Type</td>
                                        <td><cc2:SmartDropDownList ID="ddl_vendorExpenseType" runat="server" /></td>
                                        <td class="FieldLabel">Default Payment Term</td>
                                        <td><cc2:SmartDropDownList ID="ddl_VendorPaymentTerm" runat="server" /></td>
                                    </tr> 
                                    <tr>
                                        <td class="FieldLabel">Country</td>
                                        <td><cc2:SmartDropDownList ID="ddl_Country" runat="server" /></td>
                                        <td><asp:LinkButton ID="lnk_selectVendor" runat="server" Text="select vendor" OnClick="lnk_selectVendor_Click" CausesValidation="false"  /></td>
                                    </tr>
                                </table>
                            </div>--%> 
                        </td>
                        <td class="FieldLabel2" >&nbsp;Invoice Received Date</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_InvoiceReceivedDate" SkinID="DateTextBox" runat="server"  onblur="formatDateString(this);" /><asp:ImageButton 
                                id="btn_cal_InvoiceReceivedDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" CausesValidation="false"   />
                            <cc1:CalendarExtender ID="ce_InvoiceReceivedDate" TargetControlID="txt_InvoiceReceivedDate" runat="server" Format="dd/MM/yyyy"
                                FirstDayOfWeek="Sunday" PopupButtonID="btn_cal_InvoiceReceivedDate"  /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Document Type</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="ddl_DocumentType" runat="server" style="width:150px;">
                                <asp:ListItem Text="Invoice / Debit Note" Value="D" />
                                <asp:ListItem Text="Credit Note" Value="C" />
                            </asp:DropDownList>
                        </td>
                        <td class="FieldLabel2">&nbsp;Submitted By</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_SubmittedBy" runat="server" ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" />&nbsp;at&nbsp;
                            <asp:TextBox ID="txt_SubmittedOn" runat="server" ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" SkinID="DateTextBox" />
                        </td>
                    </tr>          
                    <tr>
                        <td class="FieldLabel2">&nbsp;<span id="lbl_InvoiceNo">Invoice/Debit No.</span></td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_InvoiceNo" runat="server" />
                            <asp:CustomValidator ID="val_InvoiceDate" runat="server" ControlToValidate="txt_InvoiceDate" Display="None" ValidateEmptyText="true" OnServerValidate="val_InvoiceDate_Validate" />
                            <asp:CustomValidator ID="val_InvoiceNo" runat="server" OnServerValidate="val_InvoiceNo_Validate" ControlToValidate="txt_InvoiceNo" ValidateEmptyText="true" Display="None" />
                        </td>
                        <td class="FieldLabel2" style="width:120px;">&nbsp;Customer/Account No.</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_CustomerNo" runat="server" /></td>
                    </tr>          
                    <tr>
                         <td class="FieldLabel2">&nbsp;<span id="lbl_InvoiceDate" >Invoice Date</span>&nbsp;</td>
                         <td>&nbsp;</td>
                        <td>
                             <asp:TextBox ID="txt_InvoiceDate" SkinID="DateTextBox" runat="server"  onblur="formatDateString(this); getDueDate(this.value);" /><asp:ImageButton 
                                id="btn_cal_InvoiceDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" CausesValidation="false"   />
                            <cc1:CalendarExtender ID="ce_InvoiceDate" TargetControlID="txt_InvoiceDate" runat="server" Format="dd/MM/yyyy"  OnClientShown="calendarShown"
                                FirstDayOfWeek="Sunday" PopupButtonID="btn_cal_InvoiceDate"  OnClientDateSelectionChanged="dateSelectionChanged"    />                                                        
                        </td>
                        <td class="FieldLabel2">&nbsp;Due Date</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_DueDate" SkinID="DateTextBox" runat="server"  onblur="formatDateString(this);" /><asp:ImageButton 
                                id="btn_cal_DueDate" runat="server" ImageAlign="Middle" ImageUrl="~/images/calendar.gif" CausesValidation="false"   />
                            <cc1:CalendarExtender ID="ce_DueDate" TargetControlID="txt_DueDate" runat="server" Format="dd/MM/yyyy"
                                FirstDayOfWeek="Sunday" PopupButtonID="btn_cal_DueDate"  />           
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Currency</td>
                        <td>&nbsp;</td>
                        <td><cc2:SmartDropDownList id="ddl_Currency" runat="server" 
                                onselectedindexchanged="ddl_Currency_SelectedIndexChanged" AutoPostBack="true"/> </td>                        
                        <td class="FieldLabel2">&nbsp;Total Invoice Amount</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_Amount" runat="server" onfocus="if (this.readOnly) blur();" />                            
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Payment Method</td>
                        <td>&nbsp;</td>
                        <td><cc2:SmartDropDownList ID="ddl_PaymentMethod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_PaymentMethod_SelectedIndexChange" /></td>
                        <div id="div_VAT" runat="server" visible="false" >
                        <td  style="width:120px;" class="FieldLabel2">&nbsp;Total VAT</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_TotalVAT" runat="server" /></td>
                        </div>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <asp:UpdatePanel ID="up_InvoiceDetail" runat="server" >
                            <ContentTemplate >  
                            <table cellspacing="0" cellpadding="0">
                        <tr>
                        <td class="FieldLabel2" style="vertical-align: top ; background-position :top ; width : 130px;">&nbsp;Cost Centre&nbsp;
                        <asp:ImageButton ID="btn_add" runat="server" ImageUrl="../images/icon_s_add.gif" ToolTip="Add" OnClick="AddCostCenter_Click" CausesValidation="false" OnClientClick="itemCount++;"  /></td>
                        <td>&nbsp;</td>
                        <td>
                          
                            <table style="border: 1px solid #AAAAAA;" >                                
                                <asp:Repeater ID="rep_InvoiceDetail" runat="server" OnItemDataBound="InvoiceDetailDataBound" OnItemCommand="InvoiceDetailItemCommand">
                                    <ItemTemplate>
                                        <tr>    
                                            <td>
                                                <asp:ImageButton ID="btn_copy" runat="server" ImageUrl="../images/icon_s_copy.gif" CommandName="copy" CausesValidation="false" OnClientClick="itemCount++;"
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Copy" style="padding:1px;" /><asp:ImageButton ID="btn_remove" runat="server" ImageUrl="../images/icon_remove.gif" 
                                                CommandName="remove" CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" style="padding:1px;" CausesValidation="false" OnClientClick="itemCount--;"  />
                                            </td>
                                            <td class="FieldLabel" style="width:100px;">&nbsp;Expense Type</td>
                                            <td style="width:280px;"><cc2:SmartDropDownList ID="ddl_ExpenseType" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="ddl_ExpenseType_SelectedIndexChange" />

                                            </td>
                                            <td class="FieldLabel" style="width :80px;">&nbsp;Amount</td>
                                            <td><asp:TextBox ID="txt_Amount" runat="server" skinId="DateTextBox"  Text='<%# Eval("Amount") %>'  onkeyup="getTotalAmount(this.value);" onfocus="controlTotalAmount(this.value);" onblur="checkAmount(this.value); calculateTotalAmount();" /></td>                                            
                                            <td class="FieldLabel" style="width:30px;" id="row_VAT1" runat="server" visible="false" >&nbsp;VAT</td>
                                            <td ID="row_VAT2" runat="server" Visible ="false" ><asp:TextBox ID="txt_VAT" runat="server" SkinID="DateTextBox" Text='<%# Eval("VAT") %>' onkeyup="getTotalVATAmount(this.value);" onfocus="controlTotalVATAmount(this.value);"   /></td>
                                        </tr>
                                        <tr id="row_CostCenter_User" runat="server" visible="false" >
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;User</td>
                                            <td colspan="5"><uc1:UclSmartSelection ID="txt_User" runat="server" AutoPostBack="true" OnSelectionChanged="txt_User_SelectionChange" /></td>
                                        </tr>
                                        <tr id="row_CostCenter" runat="server">
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Cost Centre</td>
                                            <td colspan="5"><cc2:SmartDropDownList ID="ddl_CostCenter" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="ddl_CostCenter_SelectedIndexChange" /></td>                                            
                                        </tr>
                                        <tr id="row_CostCenter_ProductTeam" runat="server" visible="false">
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Product Team</td>
                                            <td colspan="5"><uc1:UclSmartSelection ID="ddl_ProductTeam" runat="server" /></td>
                                        </tr>
                                        <tr id="row_CostCenter_Season" runat="server" visible="false" >
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Season</td>
                                            <td colspan="5"><cc2:SmartDropDownList ID="ddl_Season" runat="server" /></td>
                                        </tr>
                                        <tr id="row_CostCenter_ItemNo" runat="server" visible="false">
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Item No</td>
                                            <td colspan="5"><asp:TextBox ID="txt_ItemNo" runat="server" MaxLength="15" /></td>
                                        </tr>
                                        <tr id="row_CostCenter_DevSampleType" runat="server" visible="false" >
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Dev Sample Type</td>
                                            <td><cc2:SmartDropDownList ID="ddl_DevSampleType" runat="server" /> </td>
                                        </tr>
                                        <tr id="row_CostCenter_ExpenseNature" runat="server" visible="false" >
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Nature</td>
                                            <td><cc2:SmartDropDownList ID="ddl_Nature" runat="server" /> </td>
                                        </tr>
                                        <tr id="row_CostCenter_TradingAF" runat="server" visible="false" >
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Contract-Dly No.</td>
                                            <td><asp:TextBox ID="txtContractNo" runat="server" style="width:100px;" />&nbsp;-&nbsp;<asp:TextBox ID="txtDeliveryNo" runat="server" style="width:20px;" />&nbsp;&nbsp;
                                                <span style="font-size :7pt; font-style: italic ; color:#888888;">contract No. + delivery No.</span>
                                            </td>
                                        </tr>
                                        <tr id="row_CostCenter_Consumption" runat="server" visible="true">
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Consumption&nbsp;Detail&nbsp;</td>
                                            <td colspan="5">
                                                <i>Unit</i>&nbsp;:&nbsp;<cc2:SmartDropDownList ID="ddl_ConsumptionUnit" runat="server" />&nbsp;&nbsp;&nbsp;
                                                <i><asp:label ID="lblConsumptionQty" runat="server" Text="No. of Units Consumed : "/></i><asp:TextBox ID="txtConsumptionQty" runat="server" MaxLength="10" style="width:70px;" />&nbsp;&nbsp;&nbsp;
                                                <i><asp:label ID="lblConsumptionUnitCost" runat="server" Text="Unit Cost : "/></i><asp:TextBox ID="txtConsumptionUnitCost" runat="server" MaxLength="10"  style="width:70px;" />
                                                <i><asp:label ID="lblFuelType" runat="server" Text="Fuel Type : " /></i><cc2:SmartDropDownList ID="ddl_FuelType" runat="server" />&nbsp;&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                        <tr id="row_CostCenter_Quantity" runat="server" visible="false">
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Quantity</td>
                                            <td colspan="5"><asp:TextBox ID="txt_Quantity" runat="server" MaxLength="4" /></td>
                                        </tr>
                                        <tr id="row_ItemDesc1" runat="server">
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Item Description</td>
                                            <td colspan="5"><asp:ImageButton ID="btn_addItemDescription1" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc"
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Add" CausesValidation="false" />                                               
                                                <asp:TextBox ID="txt_ItemDescription1" runat="server" Text='<%# Eval("ItemDescription1") %>' MaxLength="250" SkinID="TextBox_250" />
                                                    <span style="font-size :7pt; font-style: italic ; color:#888888;"><asp:Label ID="lbl_ItemDescHint" runat="server" EnableTheming="false"  />
                                                    
                                                    </span>
                                                </td>
                                        </tr>
                                        <tr id="row_ItemDesc2" runat="server">
                                            <td></td>
                                            <td ></td>
                                            <td colspan="5"><asp:ImageButton ID="btn_addItemDescription2" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc"
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>'  ToolTip="Add" CausesValidation="false"  />              
                                                <asp:ImageButton ID="btn_removeItemDesc2" runat="server" ImageUrl="../images/icon_remove.gif" commandName="removeItemDesc" 
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" CausesValidation="false"  />                                  
                                                <asp:TextBox ID="txt_ItemDescription2" runat="server" Text='<%# Eval("ItemDescription2") %>' MaxLength="250" SkinID="TextBox_250" /></td>
                                        </tr>
                                        <tr id="row_ItemDesc3" runat="server">
                                            <td></td>
                                            <td></td>
                                            <td colspan="5"><asp:ImageButton ID="btn_addItemDescription3" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc"
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>'  ToolTip="Add" CausesValidation="false" />
                                                <asp:ImageButton ID="btn_removeItemDesc3" runat="server" ImageUrl="../images/icon_remove.gif" commandName="removeItemDesc" 
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" CausesValidation="false"  />
                                                <asp:TextBox ID="txt_ItemDescription3" runat="server" Text='<%# Eval("ItemDescription3") %>' MaxLength="250" SkinID="TextBox_250" /></td>
                                        </tr>
                                        <tr id="row_ItemDesc4" runat="server">
                                            <td></td>
                                            <td></td>
                                            <td colspan="5"><asp:ImageButton ID="btn_addItemDescription4" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc"
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>'  ToolTip="Add" CausesValidation="false" />
                                                <asp:ImageButton ID="btn_removeItemDesc4" runat="server" ImageUrl="../images/icon_remove.gif" commandName="removeItemDesc" 
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" CausesValidation="false"  />
                                                <asp:TextBox ID="txt_ItemDescription4" runat="server" Text='<%# Eval("ItemDescription4") %>' MaxLength="250" SkinID="TextBox_250" /></td>
                                        </tr>
                                        <tr id="row_ItemDesc5" runat="server">
                                            <td></td>
                                            <td></td>
                                            <td colspan="5"><asp:ImageButton ID="btn_addItemDescription5" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc"
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>'  ToolTip="Add" CausesValidation="false" />
                                                <asp:ImageButton ID="btn_removeItemDesc5" runat="server" ImageUrl="../images/icon_remove.gif" commandName="removeItemDesc" 
                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" CausesValidation="false"  />
                                                <asp:TextBox ID="txt_ItemDescription5" runat="server" Text='<%# Eval("ItemDescription5") %>' MaxLength="250" SkinID="TextBox_250" /></td>
                                        </tr>
                                        <tr id="row_CostCenter_SegmentField7" runat="server" visible="false">
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Segment&nbsp;Value&nbsp;7</td>
                                            <td colspan="5"><cc2:SmartDropDownList ID="ddl_SegmentValue7" runat="server" /></td>
                                        </tr>
                                        <tr id="row_CostCenter_SegmentField8" runat="server" visible="false">
                                            <td></td>
                                            <td class="FieldLabel">&nbsp;Segment&nbsp;Value&nbsp;8</td>
                                            <td colspan="5"><cc2:SmartDropDownList ID="ddl_SegmentValue8" runat="server" /></td>
                                        </tr>
                                    </ItemTemplate>
                                    <SeparatorTemplate>                                        
                                        <tr>
                                            <td style="height:5px; font-size:1px; border-bottom : 1px solid #AAAAAA;" colspan="7">&nbsp;</td>
                                        </tr>
                                    </SeparatorTemplate>
                                </asp:Repeater>                                                                                                                                                                   
                        </table>       
                        
                        </td> 
                    </tr>
                    </table>
                    </ContentTemplate>
                        </asp:UpdatePanel>
                        </td>
                    </tr>
                   <tr>
                        <td colspan="6">
                        <asp:UpdatePanel runat="server" ID="up_recharge">
                           <ContentTemplate>    
                            <table cellspacing="0" cellpadding="0" >
                            <tr>
                        <td class="FieldLabel2" style="vertical-align:top ; background-position :top; width: 130px;">&nbsp;Recharge
                        <asp:ImageButton ID="btn_AddRechargeDetail" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="add" ToolTip="Add" style="padding:1px;" OnClick="AddRechargeDetail_Click" CausesValidation="false" OnClientClick="itemCount++;" /></td>
                        <td>&nbsp;</td>
                        <td>
                                <table style="border: 1px solid #AAAAAA; ">                                
                                    <asp:Repeater ID="rep_RechargeDetail" runat="server"  OnItemDataBound="RechargeDetailDataBound" OnItemCommand="RechargeDetailItemCommand">
                                        <ItemTemplate >
                                                    <tr>
                                                        <td>
                                                        <asp:ImageButton ID="btn_copy" runat="server" ImageUrl="../images/icon_s_copy.gif" CommandName="copy" CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' CausesValidation="false"  ToolTip="Copy" 
                                                            OnClientClick="itemCount++;" style="padding:1px;" /><asp:ImageButton ID="btn_remove" runat="server" ImageUrl="../images/icon_remove.gif" CommandName="remove" 
                                                            CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>'  CausesValidation="false" OnClientClick="itemCount--;" ToolTip="Remove" style="padding:1px;" /></td>
                                                            <td class="FieldLabel" style="width:100px;">&nbsp;Expense Type</td>
                                                            <td style="width:280px;"><cc2:SmartDropDownList ID="ddl_ExpenseType" runat="server" OnSelectedIndexChanged="ddl_Recharge_ExpenseType_SelectedIndexChange" AutoPostBack="true"  /></td>
                                                        <td class="FieldLabel" style="width:80px;">&nbsp;Amount</td>
                                                        <td><asp:TextBox ID="txt_RechargeAmt" runat="server" SkinID="DateTextBox"  Text='<%# Eval("Amount") %>'  onkeyup="getTotalAmount(this.value);" onfocus="controlTotalAmount(this.value);" onblur="checkAmount(this.value); calculateTotalAmount();" /></td>
                                                        <td class="FieldLabel" style="width:30px;" id="row_VAT1" runat="server" visible="false" >&nbsp;VAT</td>
                                                        <td id="row_VAT2" runat="server" visible="false" ><asp:TextBox ID="txt_VAT" runat="server" SkinID="DateTextBox" Text='<%# Eval("VAT") %>' onkeyup="getTotalVATAmount(this.value);" onfocus="controlTotalVATAmount(this.value);" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                            <td class="FieldLabel">&nbsp;Recharge Party</td>
                                                            <td>
                                                                <cc2:SmartDropDownList ID="ddl_RechargeType" runat="server" OnSelectedIndexChanged="ddl_RechargeType_SelectedIndexChange" AutoPostBack="true" onchange="getVendorType(this.value);"   />
                                                            </td>
                                                            <td class="FieldLabel" id="row_RechargeCcy1" runat="server" visible ="false">&nbsp;Recharge Ccy</td>
                                                            <td id="row_RechargeCcy2" runat="server" visible="false" colspan="3" >
                                                            <asp:DropDownList ID="ddl_RechargeCurrency" runat="server" SkinID="SmallDDL" >
                                                                <asp:ListItem Text="GBP" Value="2" />
                                                                <asp:ListItem Text="USD" Value="3" Selected="True"  />
                                                                <asp:ListItem Text="EUR" Value="12" />
                                                            </asp:DropDownList>                                                            
                                                            </td>
                                                    </tr>  
                                                    <tr id="row_Recharge_Vendor" runat="server" visible="false">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Vendor</td>
                                                        <td onmouseover="document.getElementById('ctl00_ContentPlaceHolder1_ddl_Office').value = document.getElementById('ddl_Office').value;">
                                                            <uc1:UclSmartSelection ID="txt_Vendor" runat="server" />
                                                        </td>
                                                        <td class="FieldLabel" id="row_Intercomm1" runat="server" visible ="false">&nbsp;Inter-co Office</td>
                                                        <td id="row_Intercomm2" runat="server" visible="false" colspan="3" >
                                                            <cc2:SmartDropDownList ID="ddl_Intercomm" runat="server" Width="120px" />
                                                        </td>
                                                    </tr>
                                                    <tr id="row_Recharge_Customer" runat="server" visible="false" >
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Customer</td>
                                                        <td><cc2:SmartDropDownList ID="ddl_Customer" runat="server" /></td>
                                                        <td class="FieldLabel" id="row_Intercomm3" runat="server" visible ="false">&nbsp;Inter-co Office</td>
                                                        <td id="row_Intercomm4" runat="server" visible="false" colspan="3" >
                                                            <cc2:SmartDropDownList ID="ddl_Intercomm_Cust" runat="server" Width="120px" />
                                                        </td>
                                                    </tr>
                                                    <tr id="row_Recharge_ContactPerson" runat="server" visible="false">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Contact Person</td>
                                                        <td colspan="5"><asp:TextBox ID="txt_ContactPerson" runat="server" MaxLength="100" SkinID="TextBox300" /></td>
                                                    </tr>
                                                    <tr id="row_Recharge_Office" runat="server" visible="false" >
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;NS Office</td>
                                                        <td><cc2:SmartDropDownList ID="ddl_Office" runat="server" OnSelectedIndexChanged="ddl_RechargeOffice_SelectedIndexChange" AutoPostBack="true"  onchange="if (document.getElementById('ddl_Office').value == '1' && this.value == '19') { alert('No need to recharge to CA office\nInput the correct expense type under HK office');this.value='-1';} " />
                                                            <cc2:SmartDropDownList ID="ddl_BusinessEntity" runat="server" OnSelectedIndexChanged="ddl_RechargeBusinessEntity_SelectedIndexChange" AutoPostBack="true"   />
                                                        </td>
                                                    </tr>
                                                    <tr id="row_Recharge_User" runat="server" visible="false" >
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;User</td>
                                                        <td colspan="5"><uc1:UclSmartSelection ID="txt_User" runat="server" OnSelectionChanged="txt_RechargeUser_SelectionChange" AutoPostBack="true"  /></td>
                                                    </tr>
                                                    <tr id="row_Recharge_CostCenter" runat="server" visible="false">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Cost Centre</td>
                                                        <td colspan="5"><cc2:SmartDropDownList ID="ddl_CostCenter" runat="server" /></td>
                                                    </tr>
                                                    <tr id="row_Recharge_ProductTeam" runat="server" visible="false">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Product Team</td>
                                                        <td colspan="5"><uc1:UclSmartSelection ID="txt_ProductTeam" runat="server" /></td>
                                                    </tr>
                                                    <tr id="row_Recharge_Season" runat="server" visible="false">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Season</td>
                                                        <td><cc2:SmartDropDownList ID="ddl_Season" runat="server" /></td>
                                                    </tr>
                                                    <tr id="row_Recharge_ItemNo" runat="server" visible="false">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Item No</td>
                                                        <td colspan="5"><asp:TextBox ID="txt_ItemNo" runat="server" MaxLength="15" /></td>
                                                    </tr>
                                                    <tr id="row_Recharge_DevSampleType" runat="server" visible="false" >
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Dev Sample Type</td>
                                                        <td><cc2:SmartDropDownList ID="ddl_DevSampleType" runat="server" /> </td>
                                                    </tr>
                                                    <tr id="row_Recharge_ExpenseNature" runat="server" visible="false" >
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Nature</td>
                                                        <td><cc2:SmartDropDownList ID="ddl_Nature" runat="server" /> </td>
                                                    </tr>
                                                    <tr id="row_Recharge_TradingAF" runat="server" visible="false" >
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Contract-Dly No.</td>
                                                        <td><asp:TextBox ID="txtContractNo" runat="server" style="width:100px;" />&nbsp;-&nbsp;<asp:TextBox ID="txtDeliveryNo" runat="server" style="width:20px;" />&nbsp;&nbsp;
                                                            <span style="font-size :7pt; font-style: italic ; color:#888888;">contract No. + delivery No.</span>
                                                        </td>
                                                    </tr>
                                                    <tr id="row_Recharge_Consumption" runat="server" visible="true">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Consumption&nbsp;Detail&nbsp;</td>
                                                        <td colspan="5">
                                                            <i>Unit</i>&nbsp;:&nbsp;<cc2:SmartDropDownList ID="ddl_ConsumptionUnit" runat="server" />&nbsp;&nbsp;&nbsp;
                                                            <i><asp:label ID="lblConsumptionQty" runat="server" Text="No. of Units Consumed : "/></i><asp:TextBox ID="txtConsumptionQty" runat="server" MaxLength="10" style="width:70px;" />&nbsp;&nbsp;&nbsp;
                                                            <i><asp:label ID="lblConsumptionUnitCost" runat="server" Text="Unit Cost : "/></i><asp:TextBox ID="txtConsumptionUnitCost" runat="server" MaxLength="10" style="width:70px;" />
                                                            <i><asp:label ID="lblFuelType" runat="server" Text="Fuel Type : " /></i><cc2:SmartDropDownList ID="ddl_FuelType" runat="server" />&nbsp;&nbsp;&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr id="row_Recharge_Quantity" runat="server" visible="false">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Quantity</td>
                                                        <td colspan="5"><asp:TextBox ID="txt_Quantity" runat="server" MaxLength="4" /></td>
                                                    </tr>
                                                    <tr id="row_RechargeItemDesc1">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Item Description</td>
                                                        <td colspan="5"><asp:ImageButton ID="btn_addItemDescription1" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc" 
                                                            CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Add" CausesValidation="false" />
                                                            <asp:TextBox ID="txt_ItemDescription1" runat="server" Text='<%# Eval("ItemDescription1") %>' MaxLength="250" SkinID="TextBox_250" />
                                                            <span style="font-size :7pt; font-style: italic ; color:#888888;"><asp:Label ID="lbl_ItemDescHint" runat="server" EnableTheming="false"   /></span>
                                                            </td>
                                                    </tr>
                                                    <tr id="row_RechargeItemDesc2" runat="server">
                                                        <td></td>
                                                        <td></td>
                                                        <td colspan="5"><asp:ImageButton ID="btn_addItemDescription2" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc" 
                                                            CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Add" CausesValidation="false" />
                                                            <asp:ImageButton ID="btn_removeItemDesc2" runat="server" ImageUrl="../images/icon_remove.gif" commandName="removeItemDesc" 
                                                            CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" CausesValidation="false"  />
                                                            <asp:TextBox ID="txt_ItemDescription2" runat="server" Text='<%# Eval("ItemDescription2") %>' MaxLength="250" SkinID="TextBox_250" /></td>
                                                    </tr>
                                                    <tr id="row_RechargeItemDesc3" runat="server" >
                                                        <td></td>
                                                        <td></td>
                                                        <td colspan="5"><asp:ImageButton ID="btn_addItemDescription3" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc" 
                                                            CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Add" CausesValidation="false" />
                                                            <asp:ImageButton ID="btn_removeItemDesc3" runat="server" ImageUrl="../images/icon_remove.gif" commandName="removeItemDesc" 
                                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" CausesValidation="false"  />
                                                            <asp:TextBox ID="txt_ItemDescription3" runat="server" Text='<%# Eval("ItemDescription3") %>' MaxLength="250" SkinID="TextBox_250" /></td>
                                                    </tr>
                                                    <tr id="row_RechargeItemDesc4" runat="server" >
                                                        <td></td>
                                                        <td></td>
                                                        <td colspan="5"><asp:ImageButton ID="btn_addItemDescription4" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc" 
                                                            CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Add" CausesValidation="false" />
                                                            <asp:ImageButton ID="btn_removeItemDesc4" runat="server" ImageUrl="../images/icon_remove.gif" commandName="removeItemDesc" 
                                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" CausesValidation="false"  />
                                                            <asp:TextBox ID="txt_ItemDescription4" runat="server" Text='<%# Eval("ItemDescription4") %>' MaxLength="250" SkinID="TextBox_250" /></td>
                                                    </tr>
                                                    <tr id="row_RechargeItemDesc5" runat="server">
                                                        <td></td>
                                                        <td></td>
                                                        <td colspan="5"><asp:ImageButton ID="btn_addItemDescription5" runat="server" ImageUrl="../images/icon_s_add.gif" CommandName="addItemDesc" 
                                                            CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Add" CausesValidation="false" />
                                                            <asp:ImageButton ID="btn_removeItemDesc5" runat="server" ImageUrl="../images/icon_remove.gif" commandName="removeItemDesc" 
                                                                CommandArgument='<%#  DataBinder.Eval(Container, "ItemIndex") %>' ToolTip="Remove" CausesValidation="false"  />
                                                            <asp:TextBox ID="txt_ItemDescription5" runat="server" Text='<%# Eval("ItemDescription5") %>' MaxLength="250" SkinID="TextBox_250" /></td>
                                                    </tr>
                                                    <tr id="row_Recharge_SegmentField7" runat="server" visible="false">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Segment&nbsp;Value&nbsp;7</td>
                                                        <td colspan="5"><cc2:SmartDropDownList ID="ddl_SegmentValue7" runat="server" /></td>
                                                    </tr>
                                                    <tr id="row_Recharge_SegmentField8" runat="server" visible="false">
                                                        <td></td>
                                                        <td class="FieldLabel">&nbsp;Segment&nbsp;Value&nbsp;8</td>
                                                        <td colspan="5"><cc2:SmartDropDownList ID="ddl_SegmentValue8" runat="server" /></td>
                                                    </tr>
                                        </ItemTemplate>       
                                        <SeparatorTemplate >
                                            <tr>
                                                <td style="height : 5px; font-size:1px; border-bottom: 1px solid #AAAAAA;" colspan="7">&nbsp;</td>
                                            </tr>
                                        </SeparatorTemplate>                             
                                    </asp:Repeater>                                   
                                    
                                </table>
                        </td>                        
                    </tr>
                            </table>
                                </ContentTemplate> 
                                </asp:UpdatePanel> 
                        </td>
                   </tr>
                    
                    <tr>
                        <td class="FieldLabel2">&nbsp;Payment Year / Month</td>
                        <td>&nbsp;</td>
                        <td colspan="4">
                        <asp:DropDownList ID="ddl_PaymentMonthFrom" runat="server" SkinID="XSDDL" onchange="document.getElementById('ddl_PaymentMonthTo').selectedIndex = this.selectedIndex;">
                            <asp:ListItem Text="Jan" Value="1" /><asp:ListItem Text="Feb" Value="2" /><asp:ListItem Text="Mar" Value="3" />
                            <asp:ListItem Text="Apr" Value="4" /><asp:ListItem Text="May" Value="5" /><asp:ListItem Text="Jun" Value="6" />
                            <asp:ListItem Text="Jul" Value="7" /><asp:ListItem Text="Aug" Value="8" /><asp:ListItem Text="Sep" Value="9" />
                            <asp:ListItem Text="Oct" Value="10" /><asp:ListItem Text="Nov" Value="11" /><asp:ListItem Text="Dec" Value="12" />
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddl_PaymentYearFrom" runat="server" SkinId="XSDDL" onchange="document.getElementById('ddl_PaymentYearTo').selectedIndex = this.selectedIndex;"  />&nbsp;&nbsp;To&nbsp;                        
                        <asp:DropDownList ID="ddl_PaymentMonthTo" runat="server" SkinID="XSDDL">
                             <asp:ListItem Text="Jan" Value="1" /><asp:ListItem Text="Feb" Value="2" /><asp:ListItem Text="Mar" Value="3" />
                            <asp:ListItem Text="Apr" Value="4" /><asp:ListItem Text="May" Value="5" /><asp:ListItem Text="Jun" Value="6" />
                            <asp:ListItem Text="Jul" Value="7" /><asp:ListItem Text="Aug" Value="8" /><asp:ListItem Text="Sep" Value="9" />
                            <asp:ListItem Text="Oct" Value="10" /><asp:ListItem Text="Nov" Value="11" /><asp:ListItem Text="Dec" Value="12" />
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddl_PaymentYearTo" runat="server" SkinId="XSDDL" />                                                            
                        </td>
                    </tr>
                             
                    <tr>
                        <td class="FieldLabel2">&nbsp;Reject Reason</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:TextBox ID="txt_RejectReason" runat="server" SkinID="XLTextBox" MaxLength="200" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2" style="vertical-align:top ; background-position : top;">&nbsp;Remark</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:TextBox ID="txt_Remark" runat="server" SkinId="XLTextBox" TextMode="MultiLine" Rows="3" MaxLength="1000" onKeyUp="checkMultiLineTextboxMaxLength(this,1000)" onChange="checkMultiLineTextboxMaxLength(this,1000)"  /></td>
                    </tr>
                    <tr id="row_ReleaseReason" runat="server" visible="false" >
                        <td class="FieldLabel2">&nbsp;Release Reason</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><asp:TextBox ID="txt_ReleaseReason" runat="server" SkinID="XLTextBox" MaxLength="100" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2"  style="vertical-align :top ; background-position : top ;">&nbsp;Invoice Upload</td>
                        <td>&nbsp;</td>
                        <td colspan="4">
                            <div id="div_InvoiceUpload" style="display:none;">
                                <asp:FileUpload ID="fu_InvoiceUpload" runat="server" Width="400"/>&nbsp;<asp:Button ID="btn_UploadInvoice" runat="server" OnClick="btn_UploadInvoice_Click" Text="Upload" CausesValidation="false"/>
                            </div> 
                            <asp:ImageButton ID="btn_upload" runat="server" ImageUrl="~/images/upload2.jpg" CausesValidation="false" ToolTip="Upload Files" OnClientClick="div_InvoiceUpload.style.display = 'block'; return false;"  />
                            <asp:GridView ID="gv_InvoiceUpload" runat="server" OnRowCommand="InvoiceUpload_ItemCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="File Name" ItemStyle-Width="350px" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                                        <ItemTemplate >
                                            <asp:ImageButton ID="btn_remove" runat="server" ImageUrl="../images/icon_remove.gif" CommandName="remove" OnClientClick="return confirm('Are you sure to delete this file?');"
                                            CommandArgument='<%#  DataBinder.Eval(Container, "RowIndex") %>' ToolTip="Remove" style="padding:1px;" CausesValidation="false"  />&nbsp;
                                            <asp:LinkButton ID="lnk_FileLink" runat="server" CommandArgument='<%#  DataBinder.Eval(Container, "RowIndex") %>' CausesValidation="false"  OnClick="lnk_FileLink_Click" Text='<%# DataBinder.Eval(Container, "DataItem.FileName")%>'   />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Upload Date" ItemStyle-Width="150px">
                                        <ItemTemplate>
                                            <%# DataBinder.Eval(Container, "DataItem.LastModifyDate")%>   
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>                                
                                <EmptyDataTemplate >No files uploaded.</EmptyDataTemplate>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2"  style="vertical-align :top ; background-position : top ;">Procurement Request No</td>
                        <td>&nbsp;</td>
                        <td colspan="4" style="vertical-align :top ; background-position : top ;">
                            <asp:TextBox runat="server" ID="txtProcurementRequestNo" />&nbsp;<asp:Button 
                                ID="btnProcurement" runat="server" Text="Open Detail" CausesValidation="false" CommandArgument="http://ns-s15/apds/request/procurementedit.aspx?"
                                onclick="btnProcurement_Click" SkinID="LButton" />
                                <asp:CustomValidator ID="val_Procurement" runat="server" 
                                ControlToValidate="txtProcurementRequestNo" Display="None" 
                                ValidateEmptyText="true" onservervalidate="val_Procurement_ServerValidate" />
                        </td>
                    </tr>


                    <tr>
                        <td class="FieldLabel2">&nbsp;Approver</td>
                        <td>&nbsp;</td>
                        <td colspan="4"><cc2:SmartDropDownList ID="ddl_Approver" runat="server" />
                        <asp:CustomValidator ID="val_Approver" runat="server" OnServerValidate="val_Approver_Validate" ControlToValidate="ddl_Approver" ValidateEmptyText="true" Display="None" />
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        </tr>

                    <tr>
                        <td colspan="6"><hr width="90%"  size="1" 
                                style="margin-top: 0px; background-color: #666666;" noshade="noshade" />                        
                          </td>
                    </tr>
                    <tr>
                        <td >&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Settlement Date</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_SettlementDate" SkinID="DateTextBox" runat="server"   ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" />
                        </td>
                        <td class="FieldLabel2">&nbsp;Settlement Amount</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_SettleAmt" runat="server" ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" /></td>
                    </tr>                    
                    <%--<tr>
                        <td class="FieldLabel2">&nbsp;Settlement Ref.</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_SettlementRefNo" runat="server" /></td>
                    </tr>--%>

                   <tr>
                        <td class="FieldLabel2">&nbsp;Settlement Bank Acc</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_SettlementBankAcc" runat="server"  ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();"  />
                        </td>
                        <td class="FieldLabel2">&nbsp;Cheque No.</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_ChequeNo" runat="server" ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" /></td>
                    </tr>
                    <tr id="row_PayByHK" runat="server" visible="false">
                        <td class="FieldLabel2">&nbsp;Pay by HK Office</td>
                        <td>&nbsp;</td>
                        <td><asp:CheckBox ID="ckb_PayByHK" runat="server" Visible="True" Enabled="false"  /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Epicor Interface Date</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txt_SUNInterfaceDate" runat="server" ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" />
                        </td>
                        <td class="FieldLabel2">&nbsp;Fiscal Year & Period</td>
                        <td>&nbsp;</td>
                        <td>Year&nbsp;<asp:TextBox ID="txt_FiscalYear" runat="server" ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" SkinID="TextBox_50" />
                            Period&nbsp;<asp:TextBox ID="txt_FiscalPeriod" runat="server" ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" SkinID="TextBox_50" />
                        </td>
                    </tr>
                    <tr id="row_LogoInterface" runat="server" visible="false">
                        <td class="FieldLabel2">&nbsp;Logo Interface Date</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_LogoInterfaceDate" runat="server" ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2">&nbsp;Journal No.</td>
                        <td>&nbsp;</td>
                        <td><asp:TextBox ID="txt_JournalNo" runat="server"  SkinID= "DateTextBox"  ReadOnly="true" CssClass="readOnlyField" onfocus="this.blur();" MaxLength="10" /></td>
                    </tr>
                    <tr>
                        <td class="FieldLabel2" style="vertical-align:top ; background-position:top ;">&nbsp;History</td>
                        <td>&nbsp;</td>
                        <td colspan="4">
                        <asp:ImageButton ID="btn_History" runat="server" ImageUrl="~/Images/view1.jpg" OnClientClick="return false;" />
                        <div id="flyout4" style="display: none; overflow: hidden; z-index: 2; background-color: #FFFFFF; border: solid 1px #D0D0D0;"></div>        
                                          <div id="div_History" class="animationLayer" style="width:600px; height:250px;overflow-x:scroll;overflow-y:scroll;
                                              opacity: 0; filter: progid:DXImageTransform.Microsoft.Alpha(opacity=0); ">
                                                        <div style="text-align:left;">
                                                        <table width="96%">
                                                            <tr>
                                                                <td><span class="header2" >History</span></td>
                                                                <td><a onclick="div_History.style.visibility='hidden';" style="cursor:pointer;float:right ;" >
                                                                <img src="../images/close.png" alt="close" /></a>
                                                              </td>
                                                            </tr>
                                                        </table>
                                                        </div>
                                                            <asp:GridView ID="gv_ActionHistory" runat="server" OnRowDataBound="ActionHistoryDataBound" Width="580px">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Action Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="120px">
                                                                            <ItemTemplate >
                                                                                <%# DataBinder.Eval(Container, "DataItem.ActionOn")%>                                
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Action By" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lbl_ActionBy" runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Detail" ItemStyle-HorizontalAlign="Left">
                                                                            <ItemTemplate >
                                                                                <%# DataBinder.Eval(Container, "DataItem.Description")%>       
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>                                                                
                                                            </asp:GridView>                                                                                                                                                         
                                                                <br />
                                                        </div>

                                                 <cc1:AnimationExtender id="ae_History" runat="server" TargetControlID="btn_History">
                                                    <Animations>
                                                        <OnClick>
                                                            <Sequence>
                                                                <%-- Disable the button so it can't be clicked again --%>
                                                                <EnableAction Enabled="false" />
                                                                
                                                                <%-- Position the wire frame on top of the button and show it --%>
                                                                <ScriptAction Script="Cover($get('btn_History'), $get('flyout4'));" />
                                                                <StyleAction AnimationTarget="flyout4" Attribute="display" Value="block"/>
                                                                
                                                                <%-- Move the wire frame from the button's bounds to the info panel's bounds --%>
                                                                <Parallel AnimationTarget="flyout4" Duration=".3" Fps="25">
                                                                    <Move Horizontal="-10" Vertical="-200" />
                                                                    <Resize Width="600" Height="250" />
                                                                    <Color PropertyKey="backgroundColor" StartValue="#AAAAAA" EndValue="#FFFFFF" />
                                                                </Parallel>
                                                                
                                                                <%-- Move the info panel on top of the wire frame, fade it in, and hide the frame --%>
                                                                <ScriptAction Script="Cover($get('flyout4'), $get('div_History'), true);" />
                                                                <StyleAction AnimationTarget="div_History" Attribute="display" Value="block" />
                                                                <ScriptAction Script="div_History.style.visibility='visible';" />
                                                                <FadeIn AnimationTarget="div_History" Duration=".2"/>
                                                                <StyleAction AnimationTarget="flyout4" Attribute="display" Value="none"/>
                                                                
                                                                <%-- Flash the text/border red and fade in the "close" button --%>
                                                                <Parallel AnimationTarget="div_History" Duration=".5">
                                                                    <Color PropertyKey="color" StartValue="#666666" EndValue="#FF0000" />
                                                                    <Color PropertyKey="borderColor" StartValue="#666666" EndValue="#FF0000" />
                                                                </Parallel>
                                                                <Parallel AnimationTarget="div_History" Duration=".5">
                                                                    <Color PropertyKey="color" StartValue="#FF0000" EndValue="#666666" />
                                                                    <Color PropertyKey="borderColor" StartValue="#FF0000" EndValue="#666666" />
                                                                </Parallel>
                                                                <EnableAction Enabled="true" />
                                                            </Sequence>
                                                        </OnClick>
                                                    </Animations>
                                                </cc1:AnimationExtender>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                    </tr>       
                    <tr>
                        <td colspan="6">
                            <asp:Button ID="btn_Save" runat="server" Text="Save" OnClick="btn_Save_Click" Visible="false" OnClientClick="return processEvent();" />
                            <asp:Button ID="btn_Discard" runat="server" Text="Discard" Visible = "false" OnClick ="btn_Discard_Click" />
                                <asp:Button ID="btn_Submit" runat="server" Text="Submit" OnClick="btn_Submit_Click" Visible="false"   />
                                <asp:Button ID="btn_Copy" runat="server" Text="Copy" OnClick="btn_Copy_Click"  />
                            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" OnClick="btn_Cancel_Click"  Visible="false" OnClientClick="return confirm('Are you sure to cancel this invoice?');"  />
                            <asp:Button ID="btn_Approve" runat="server" Text="Approve" OnClick="btn_Approve_Click"  Visible="false"  />
                            <asp:Button ID="btn_Reject" runat="server" Text="Reject" OnClick="btn_Reject_Click"  Visible="false"  />
                            <asp:Button ID="btn_AccountReceive" runat="server" Text="Acc Receive" SkinID="MButton" OnClick="btn_AccountReceive_Click"   Visible="false" />
                            <asp:Button ID="btn_AccountReject" runat="server" Text="Acc Reject"  SkinID="MButton"  Visible="false" />
                            <asp:Button ID="btn_AccountApprove" runat="server" Text="Acc Approve"  SkinID="MButton" OnClick="btn_AccountApprove_Click"  Visible="false"  />
                            <asp:Button ID="btn_AccountEvaluate" runat="server" Text="Acc Evaluate" SkinID="MButton" OnClick="btn_AccountEvaluate_Click"  Visible="false"  />      
                            <asp:Button ID="btn_Release" runat="server" Text="Release" OnClick="btn_Release_Click" Visible="false" />                      
                            <asp:Button ID="btn_New" runat="server" Text="New" CausesValidation="false" OnClick="btn_New_Click" />
                            <asp:Button ID="btn_Close" runat="server" Text="Close" OnClientClick="window.close(); return false;" CausesValidation="false"  />
                        </td>
                    </tr>
                </table>
            
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                
            </td>
        </tr>
    </table>
    
<cc1:ModalPopupExtender ID="rejectPopupExtender" runat="server" 
            TargetControlID="btn_AccountReject"
            PopupControlID="divRejectReason" 
            BackgroundCssClass="modalBackground" 
            OkControlID="btn_SubmitReject"                          
            CancelControlID="btn_CancelReject" />

            <asp:Panel ID="divRejectReason" runat="server"  CssClass="infoLayer"
     style="display:none;width:450; height:400px; z-index:1; margin : 10px; vertical-align : middle ;">
     <div style="width:450; height:400px;  margin : 10px; vertical-align : middle ;">


         <table>
            <tr>
                <td colspan="2"><span style="font-weight: bold ; font-size : 10pt;">Non-Trade Expense - Reject Reason</span><br />
                    <span >Please input / select the reject reason.</span>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel2" style="width:100px; vertical-align :top ; background-position : top ;">&nbsp;Reason</td>
                <td><asp:TextBox ID="txt_AccRejectReason" runat="server" SkinID="TextBox_250" TextMode="MultiLine" Rows="3" MaxLength="200" onKeyUp="checkMultiLineTextboxMaxLength(this,200)" onChange="checkMultiLineTextboxMaxLength(this,200)"  /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">
                    <table style="border: 1px solid #AAAAAA; width:100%;" cellspacing="0">
                        <tr>
                            <td style="font-weight : bold; border-bottom: 1px solid #AAAAAA; ">Notify user to correct below item(s).</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBoxList ID="cbl_rejectReason" runat="server" RepeatDirection="Horizontal" RepeatColumns="2" RepeatLayout="Table">
                                    <asp:ListItem Text="Invoice Amount" Value="Invoice Amount" />
                                    <asp:ListItem Text="Allocation" Value="Allocation" />
                                    <asp:ListItem Text="Recharge Person" Value="Recharge Person" />
                                    <asp:ListItem Text="Recharge Amount" Value="Recharge Amount" />
                                    <asp:ListItem Text="Description" Value="Description" />
                                    <asp:ListItem Text="Expense Type" Value="Expense Type" />
                                    <asp:ListItem Text="Invoice Number" Value="Invoice Number" />
                                    <asp:ListItem Text="Invoice Date" Value="Invoice Date" />
                                    <asp:ListItem Text="Currency" Value="Currency" />
                                    <asp:ListItem Text="Invoice Upload" Value="Invoice Upload" />
                                    <asp:ListItem Text="Due Date" Value="Due Date" />
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                    </table>    
                </td>
            </tr>
            <tr><td>&nbsp;</td></tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btn_SubmitReject" runat="server" style="display:none;"  EnableTheming="false"    />
                    <asp:Button ID="btn_doSubmitReject" runat="server" OnClick="btn_AccountReject_Click" Text="Submit"   />
                    <asp:Button ID="btn_CancelReject" runat="server" Text="Cancel" CausesValidation="false"  />
                </td>
            </tr>
         </table>


                                          
      </div>
</asp:Panel>
    </form>
</body>
</html>

